using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;
using Minesweeper.Controls;
using System.Windows.Media;
using System.Diagnostics;
using System.Collections;
using Typography.OpenFont.Tables;

namespace Minesweeper.src
{
    public class MinesweeperGame
    {
        public const int WIN = 1;
        public const int LOSE = 0;

        private int numRows;
        private int numColumns;
        private int numMines;
        private int numSafeCells;
        private int numRevealedCells;
        private int numFlagsLeft;
        private bool inProgress;
        private Cell[,] Cells;
        private Grid gameGrid;

        private List<Cell> IgnoredCells;

        public event EventHandler GameStarted;
        public event EventHandler GameLost;
        public event EventHandler GameWon;
        public event EventHandler GameReset;
        public event EventHandler CellLeftClickDown;
        public event EventHandler CellLeftClickUp;
        public event EventHandler<IntEventArgs> UpdateNumFlagsLeft;

        public MinesweeperGame(Difficulty selectedDifficulty)
        {
            this.numColumns = selectedDifficulty.Width;
            this.numRows = selectedDifficulty.Height;
            this.numMines = (selectedDifficulty.Mines < numColumns * numRows) ? selectedDifficulty.Mines : (numColumns * numRows) - 1;
            initializeVariables();
        }

        #region Initializer Methods
        public void initializeGame(ref Grid gameGrid)
        {
            initializeVariables();
            initializeCellsGrid();
            initializeCells(ref gameGrid);
        }

        public void initializeVariables()
        {
            inProgress = false;
            numFlagsLeft = numMines;
            numSafeCells = (numColumns * numRows) - numMines;
            numRevealedCells = 0;
            UpdateNumFlagsLeft?.Invoke(this, new IntEventArgs(numFlagsLeft)); // Raise UI event
        }

        public void initializeCellsGrid()
        {
            // Initialize Cell grid
            Cells = new Cell[numRows, numColumns];
            for (int i = 0; i < numRows; i++)
                for (int j = 0; j < numColumns; j++)
                    Cells[i, j] = new Cell(i, j);
        }

        private void initializeCells(ref Grid gameGrid)
        {
            this.gameGrid = gameGrid;

            // Enable gamegrid
            gameGrid.IsEnabled = true;

            // Clear gameGrid contents
            gameGrid.Children.Clear();
            gameGrid.RowDefinitions.Clear();
            gameGrid.ColumnDefinitions.Clear();

            gameGrid.InvalidateVisual();

            // Iterate through rows
            for (int row = 0; row < numRows; row++)
            {
                // Add Row to gameGrid
                gameGrid.RowDefinitions.Add(new RowDefinition());

                // Iterate through columns
                for (int col = 0; col < numColumns; col++)
                {
                    if (row == 0)
                        gameGrid.ColumnDefinitions.Add(new ColumnDefinition()); // Add Column to gameGrid

                    // Create button template for blank cell
                    Button button = new Button();
                    button.Style = Application.Current.Resources["CellButtonStyle"] as Style;
                    button.Content = "";
                    // Add Button event handlers
                    button.PreviewMouseLeftButtonDown += Cell_PreviewMouseLeftButtonDown;
                    button.PreviewMouseLeftButtonUp += Cell_PreviewMouseLeftButtonUp;
                    button.PreviewMouseRightButtonDown += Cell_PreviewMouseRightButtonDown;

                    // Assign button to cell
                    Cells[row, col].button = button;

                    // Add cell event handlers
                    Cells[row, col].FlagUpdate += Cell_FlagUpdate;

                    // Set row and column properties for grid placement
                    gameGrid.Children.Add(button);
                    Grid.SetRow(button, row);
                    Grid.SetColumn(button, col);
                }
            }
        }

        private void initializeSafeCells(int origin_y, int origin_x)
        {
            List<Cell> neighboringSafeCells = new List<Cell>();

            // Collect neighboring Cells
            for (int r = -1; r <= 1; r++)
            {
                for (int c = -1; c <= 1; c++)
                {
                    // Skip over origin cell
                    if (c == 0 && r == 0)
                        continue;

                    // Calculate neighbor cell positions
                    int nr = origin_y + r;
                    int nc = origin_x + c;

                    // Check if neighbor cell is in range
                    if (nr < 0 || nr >= numRows || nc < 0 || nc >= numColumns)
                        continue;

                    neighboringSafeCells.Add(Cells[nr, nc]);
                }
            }

            // If there are enough mines to the point where some of the immediate neighboring cells to the origin MUST be mines,
            // calculate the number of neighbors that MUST then be mines and randomly choose which neighbors WILL be mines.
            // Origin cell counts as a safe cell, hence the + 1.
            int numNeighborMines = (neighboringSafeCells.Count + 1) - ((numRows * numColumns) - numMines);
            if (numNeighborMines > 0)
            {
                Random random = new Random();
                for (int i = 0; i < numNeighborMines; i++)
                {
                    int randomIndex = random.Next(0, neighboringSafeCells.Count);

                    Debug.WriteLine("numNeighborMines: " + numNeighborMines + ", randomIndex: " + randomIndex + ", neighboringSafeCells.Count: " + neighboringSafeCells.Count);

                    neighboringSafeCells.Remove(neighboringSafeCells[randomIndex]);
                }
            }

            foreach (Cell safeNeighbor in neighboringSafeCells)
            {
                safeNeighbor.isSafeCell = true;
            }

            // Make Origin cell safe
            Cells[origin_y, origin_x].isSafeCell = true;
        }

        private void initializeMines()
        {
            List<Cell> availableCells = Cells.Cast<Cell>().Where(cell => !cell.isSafeCell).ToList();

            Random random = new Random();
            for (int i = 0; i < numMines; i++)
            {
                int randIndex = random.Next(0, availableCells.Count);

                availableCells[randIndex].hasMine = true;

                availableCells.Remove(availableCells[randIndex]);
            }
        }

        private void initializeClues()
        {
            Cell[] cellsWithMines = Cells.Cast<Cell>().Where(cell => cell.hasMine).ToArray();

            foreach (Cell cell in cellsWithMines)
            {
                // Add mine click event handler
                cell.MineClicked += Cell_MineClicked;

                int row = cell.row;
                int column = cell.column;

                // Loop through current cell's neighbors to increase their count
                for (int _r = -1; _r <= 1; _r++)
                {
                    for (int _c = -1; _c <= 1; _c++)
                    {
                        int newRow = row + _r;
                        int newCol = column + _c;

                        // If neighbor in valid range
                        if (newRow >= 0 && newRow < numRows && newCol >= 0 && newCol < numColumns)
                            Cells[newRow, newCol].count++; // Update neighbor count
                    }
                }
            }
        }
        #endregion

        public void resetDifficulty(Difficulty selectedDifficulty, ref Grid gameGrid)
        {
            this.numColumns = selectedDifficulty.Width;
            this.numRows = selectedDifficulty.Height;
            this.numMines = (selectedDifficulty.Mines < numColumns * numRows) ? selectedDifficulty.Mines : (numColumns * numRows) - 1;
            initializeVariables();

            Debug.WriteLine("resetDifficulty! numColumns: " + numColumns + ", numRows: " + numRows + ", numMines: " + numMines);

            restartGame(ref gameGrid);
        }

        #region Game State methods
        public void restartGame(ref Grid gameGrid)
        {
            GameReset?.Invoke(this, EventArgs.Empty); // Raise UI event
            initializeGame(ref gameGrid);
            inProgress = false;
        }

        private void startGame(int row, int col)
        {
            initializeSafeCells(row, col);
            initializeMines();
            initializeClues();

            GameStarted?.Invoke(this, EventArgs.Empty); // Raise UI event
            inProgress = true;
        }

        private void loseGame()
        {
            GameLost?.Invoke(this, EventArgs.Empty); // Raise UI event
            inProgress = false;
            gameGrid.IsEnabled = false;
            revealAllMines(LOSE);
        }

        private void winGame()
        {
            GameWon?.Invoke(this, EventArgs.Empty); // Raise UI event
            inProgress = false;
            gameGrid.IsEnabled = false;
            revealAllMines(WIN);
        }
        #endregion

        #region Methods
        private void revealCell(int row, int col)
        {
            // Break if out of range
            if (row < 0 || row >= numRows || col < 0 || col >= numColumns || Cells[row, col].isRevealed)
                return;

            // Start game if this is first cell revealed
            if (!inProgress)
                startGame(row, col);

            // Reveal button
            Cells[row, col].reveal();

            // Check if game is won
            if (!Cells[row, col].hasMine)
            {
                numRevealedCells++;
                if (numRevealedCells == numSafeCells)
                    winGame();
            }

            // If the clicked cell is empty
            if (Cells[row, col].isEmpty())
            {
                revealCell(row, col - 1); // Left neighbor
                revealCell(row - 1, col - 1); // Top Left neighbor
                revealCell(row - 1, col); // Top neighbor
                revealCell(row - 1, col + 1); // Top Right neighbor
                revealCell(row, col + 1); // Right neighbor
                revealCell(row + 1, col + 1); // Bottom Right neighbor
                revealCell(row + 1, col); // Bottom neighbor
                revealCell(row + 1, col - 1); // Bottom Left neighbor
            }
        }

        private void revealAllMines(int GAME_RESULT)
        {
            // If game is Won, place a flag on every bomb, else if game is Lost, visually reveal all bombs
            Cell[] cellsWithMines = Cells.Cast<Cell>().Where(cell => cell.hasMine && !cell.isFlagged).ToArray();
            foreach (Cell cell in cellsWithMines)
            {
                cell.button.Content = GAME_RESULT == WIN ? Application.Current.Resources["Flag_Symbol"] : Application.Current.Resources["Mine_Symbol"];
            }

            // If game is lost, reveal all flags that were misplaced and wrong
            if (GAME_RESULT == LOSE)
            {
                Cell[] cellsWithWrongFlags = Cells.Cast<Cell>().Where(cell => !cell.hasMine && cell.isFlagged).ToArray();
                foreach (Cell cell in cellsWithWrongFlags)
                {
                    cell.hideBorders();
                    cell.button.Background = Application.Current.Resources["Red"] as Brush;
                }
            }
        }
        #endregion

        #region Cell Eventhandlers
        private void Cell_MineClicked(object sender, EventArgs e)
        {
            loseGame();
        }

        private void Cell_FlagUpdate(object sender, EventArgs e)
        {
            if (((Cell)sender).isFlagged)
                numFlagsLeft--;
            else
                numFlagsLeft++;

            UpdateNumFlagsLeft?.Invoke(this, new IntEventArgs(numFlagsLeft)); // Raise UI event
        }

        private void Cell_PreviewMouseRightButtonDown(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            int row = (int)button.GetValue(Grid.RowProperty);
            int col = (int)button.GetValue(Grid.ColumnProperty);

            // If user is trying to flag without any flags remaining, ignore
            if (!Cells[row, col].isFlagged && numFlagsLeft == 0)
                return;

            // Change borders of clicked button
            Cells[row, col].updatedFlagged();
        }

        private void Cell_PreviewMouseLeftButtonUp(object sender, RoutedEventArgs e)
        {
            CellLeftClickUp?.Invoke(this, EventArgs.Empty); // Raise UI event

            Button button = (Button)sender;
            int row = (int)button.GetValue(Grid.RowProperty);
            int col = (int)button.GetValue(Grid.ColumnProperty);

            // If cell is flagged, ignore
            if (Cells[row, col].isFlagged)
                return;

            // Reveal Cell
            revealCell(row, col);
        }

        private void Cell_PreviewMouseLeftButtonDown(object sender, RoutedEventArgs e)
        {
            CellLeftClickDown?.Invoke(this, EventArgs.Empty); // Raise UI event

            Button button = (Button)sender;
            int row = (int)button.GetValue(Grid.RowProperty);
            int col = (int)button.GetValue(Grid.ColumnProperty);

            // If cell is flagged, ignore
            if (Cells[row, col].isFlagged)
                return;

            // Change borders of clicked button
            Cells[row, col].hideBorders();
        }
        #endregion

    }
}
