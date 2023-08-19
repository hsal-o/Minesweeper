using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;
using Minesweeper.Controls;
using System.Windows.Media;

namespace Minesweeper.src
{
    public class MinesweeperGame
    {
        private int numRows;
        private int numColumns;
        private int mineCount;
        private int numSafeCells;
        private int numExposedCells;
        private int numFlagsLeft;
        private bool inProgress;
        private Cell[,] Cells;

        private Grid gameGrid;

        public static int WIN = 1;
        public static int LOSE = 0;

        public MinesweeperGame()
        {
            numRows = 16;
            numColumns = 30;
            mineCount = 99;
            numSafeCells = (numRows * numColumns) - mineCount;
            numExposedCells = 0;
            inProgress = false;
            Cells = new Cell[numRows, numColumns];
        }

        public void initializeGame(ref Grid gameGrid)
        {
            //resetVariableCounts();
            //emojiButton.setSymbol("🙂");
            //gameGrid.IsEnabled = true;

            initializeGrid();
            initializeMines();
            initializeCells(ref gameGrid);
        }
        public void initializeGrid()
        {
            Array.Clear(Cells, 0, Cells.Length); // Clear cells

            // Initialize Cell grid
            for (int i = 0; i < numRows; i++)
                for (int j = 0; j < numColumns; j++)
                    Cells[i, j] = new Cell();
        }

        private void initializeCells(ref Grid gameGrid)
        {
            this.gameGrid = gameGrid;

            // Clear gameGrid contents
            gameGrid.Children.Clear();
            gameGrid.RowDefinitions.Clear();
            gameGrid.ColumnDefinitions.Clear();

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
                    button.PreviewMouseLeftButtonDown += cellButton_PreviewMouseLeftButtonDown;
                    button.PreviewMouseLeftButtonUp += cellButton_PreviewMouseLeftButtonUp;
                    button.PreviewMouseRightButtonDown += cellButton_PreviewMouseRightButtonDown;
                    Cells[row, col].FlagUpdate += updateNumFlagsLeft;

                    // Assign button to cell
                    Cells[row, col].button = button;

                    // Set row and column properties for grid placement
                    gameGrid.Children.Add(button);
                    Grid.SetRow(button, row);
                    Grid.SetColumn(button, col);

                    // Check if current cell was predetermined to have a mine to update neighbors' counts
                    if (Cells[row, col].hasMine)
                    {
                        Cells[row, col].MineClicked += handleMineClicked;

                        // Loop through current cell's neighbors
                        for (int _r = -1; _r <= 1; _r++)
                        {
                            for (int _c = -1; _c <= 1; _c++)
                            {
                                int newRow = row + _r;
                                int newCol = col + _c;

                                // If neighbor in range
                                if (newRow >= 0 && newRow < numRows && newCol >= 0 && newCol < numColumns)
                                    Cells[newRow, newCol].count++; // Update neighbor count
                            }
                        }
                    }
                }
            }
        }

        private void initializeMines()
        {
            Random random = new Random();
            for (int i = 0; i < mineCount; i++)
            {
                // Grab random cell
                int row = random.Next(0, numRows);
                int col = random.Next(0, numColumns);

                // If cell does not already have mine
                if (!Cells[row, col].hasMine)
                    Cells[row, col].hasMine = true; // Set mine and continue
                else
                    i--; // Try again
            }
        }

        public void resetVariables()
        {
            //secondsCount = 0;
            //numFlagsLeft = mineCount;
            //numSafeCells = (Columns * Rows) - mineCount;
            //numExposedCells = 0;

            //txt_secondCounter.Text = secondsCount.ToString("D3");
        }


        private void startGame()
        {
            inProgress = true;
            //timer.Start();
        }

        private void loseGame()
        {
            //timer.Stop();
            inProgress = false;
            gameGrid.IsEnabled = false;
            exposeAllMines(LOSE);
            //emojiButton.setSymbol("☠️");
        }

        private void winGame()
        {
            //timer.Stop();
            inProgress = false;
            gameGrid.IsEnabled = false;
            exposeAllMines(WIN);
            //emojiButton.setSymbol("😎");
        }


        private void exposeCell(int row, int col)
        {
            // Break if out of range
            if (row < 0 || row >= numRows || col < 0 || col >= numColumns || Cells[row, col].isExposed)
                return;

            if (!inProgress)
            {
                startGame();
            }

            // Expose button
            Cells[row, col].expose();

            if (!Cells[row, col].hasMine)
            {
                numExposedCells++;
                if (numExposedCells == numSafeCells)
                {
                    winGame();
                }
            }

            // If the clicked cell is empty
            if (Cells[row, col].isEmpty())
            {
                exposeCell(row, col - 1); // Left neighbor
                exposeCell(row - 1, col - 1); // Top Left neighbor
                exposeCell(row - 1, col); // Top neighbor
                exposeCell(row - 1, col + 1); // Top Right neighbor
                exposeCell(row, col + 1); // Right neighbor
                exposeCell(row + 1, col + 1); // Bottom Right neighbor
                exposeCell(row + 1, col); // Bottom neighbor
                exposeCell(row + 1, col - 1); // Bottom Left neighbor
            }
        }

        private void handleMineClicked(object sender, EventArgs e)
        {
            loseGame();
        }

        private void updateNumFlagsLeft(object sender, EventArgs e)
        {
            Cell cell = (Cell)sender;

            if (cell.isFlagged)
                numFlagsLeft--;
            else
                numFlagsLeft++;
        }

        private void cellButton_PreviewMouseRightButtonDown(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            int row = (int)button.GetValue(Grid.RowProperty);
            int col = (int)button.GetValue(Grid.ColumnProperty);

            if (!Cells[row, col].isFlagged && numFlagsLeft == 0)
                return;


            // Change borders of clicked button
            Cells[row, col].updatedFlagged();
        }

        private void cellButton_PreviewMouseLeftButtonUp(object sender, RoutedEventArgs e)
        {
            //emojiButton.setSymbol("🙂");

            Button button = (Button)sender;
            int row = (int)button.GetValue(Grid.RowProperty);
            int col = (int)button.GetValue(Grid.ColumnProperty);

            if (Cells[row, col].isFlagged)
                return;

            // Reveal Cell
            exposeCell(row, col);
        }

        private void cellButton_PreviewMouseLeftButtonDown(object sender, RoutedEventArgs e)
        {
            //emojiButton.setSymbol("😮");

            Button button = (Button)sender;
            int row = (int)button.GetValue(Grid.RowProperty);
            int col = (int)button.GetValue(Grid.ColumnProperty);

            if (Cells[row, col].isFlagged)
                return;

            // Change borders of clicked button
            Cells[row, col].exposeBorders();
        }


        private void exposeAllMines(int GAME_RESULT)
        {
            Cell[] cellsWithMines = Cells.Cast<Cell>().Where(cell => cell.hasMine && !cell.isFlagged).ToArray();
            foreach (Cell cell in cellsWithMines)
            {
                cell.button.Content = GAME_RESULT == WIN ? Application.Current.Resources["Flag_Symbol"] : Application.Current.Resources["Mine_Symbol"];
            }

            if (GAME_RESULT == LOSE)
            {
                Cell[] cellsWithWrongFlags = Cells.Cast<Cell>().Where(cell => !cell.hasMine && cell.isFlagged).ToArray();
                foreach (Cell cell in cellsWithWrongFlags)
                {
                    cell.exposeBorders();
                    cell.button.Background = Application.Current.Resources["Red"] as Brush;
                }
            }
        }
    }
}
