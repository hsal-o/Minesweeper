using System;
using System.Buffers.Text;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Minesweeper
{
    public class Cell
    {
        public int row { get; set; }
        public int column { get; set; }
        public int count { get; set; }
        public bool hasMine { get; set; }
        public bool isExposed { get; set; }
        public bool isFlagged { get; set; }
        public Button button { get; set; }
        public event EventHandler MineClicked;


        public Cell()
        {
            row = 0;
            column = 0;
            count = 0;
            hasMine = false;
            isExposed = false;
            isFlagged = false;
            button = null;
        }

        public bool isEmpty()
        {
            return count == 0;
        }

        public void updatedFlagged()
        {
            isFlagged = !isFlagged;

            if (isFlagged)
                button.Content = Application.Current.Resources["Flag_Symbol"];
            else
                button.Content = "";
        }

        public void exposeBorders()
        {
            Border buttonLeftBorder = button.Template.FindName("ButtonLeftBorder", button) as Border;
            if (buttonLeftBorder != null)
            {
                buttonLeftBorder.Visibility = Visibility.Visible;
                buttonLeftBorder.BorderBrush = Application.Current.Resources["CellBackgroundDark"] as Brush;
                buttonLeftBorder.BorderThickness = new Thickness(1, 0, 0, 0);
            }

            Border buttonTopBorder = button.Template.FindName("ButtonTopBorder", button) as Border;
            if (buttonTopBorder != null)
            {
                buttonTopBorder.Visibility = Visibility.Visible;
                buttonTopBorder.BorderBrush = Application.Current.Resources["CellBackgroundDark"] as Brush;
                buttonTopBorder.BorderThickness = new Thickness(0, 1, 0, 0);
            }

            Border buttonRightBorder = button.Template.FindName("ButtonRightBorder", button) as Border;
            if (buttonRightBorder != null)
            {
                buttonRightBorder.Visibility = Visibility.Hidden;
            }

            Border buttonBottomBorder = button.Template.FindName("ButtonBottomBorder", button) as Border;
            if (buttonBottomBorder != null)
            {
                buttonBottomBorder.Visibility = Visibility.Hidden;
            }
        }

        public void expose()
        {
            if (isFlagged)
                return;

            isExposed = true;
            button.IsEnabled = false;

            exposeBorders();

            if (hasMine) // If the clicked cell has a mine
            {
                // Handle mine click
                MineClicked?.Invoke(this, EventArgs.Empty);

                button.Content = Application.Current.Resources["Mine_Symbol"];
                button.Background = Application.Current.Resources["Red"] as Brush;
            }
            else
            {
                if (count > 0) // If the cell has a neighboring mine
                {
                    button.Content = count;
                    button.Foreground = Application.Current.Resources[count + "_Color"] as Brush;
                }
                else
                {
                    // empty cell
                }
            }
        }
    }


    public partial class MainWindow : Window
    {
        private const int Columns = 20;
        private const int Rows = 20;
        private const int mineCount = 20;
        private Cell[,] Cells = new Cell[Rows, Columns];

        public MainWindow()
        {
            InitializeComponent();
            initializeGameGrid();
            SizeToContent = SizeToContent.WidthAndHeight; // Auto-fit window
        }

        private void initializeCellGrid()
        {
            Array.Clear(Cells); // Clear cells

            // Initialize Cell grid
            for (int i = 0; i < Rows; i++)
                for (int j = 0; j < Columns; j++)
                    Cells[i, j] = new Cell();
        }

        private void initializeMines()
        {
            Random random = new Random();
            for (int i = 0; i < mineCount; i++)
            {
                // Grab random cell
                int row = random.Next(0, Rows);
                int col = random.Next(0, Columns);

                // If cell does not already have mine
                if (!Cells[row, col].hasMine)
                    Cells[row, col].hasMine = true; // Set mine and continue
                else
                    i--; // Try again
            }
        }

        private void initializeCells()
        {
            // Clear gameGrid contents
            gameGrid.Children.Clear();
            gameGrid.RowDefinitions.Clear();
            gameGrid.ColumnDefinitions.Clear();

            // Iterate through rows
            for (int row = 0; row < Rows; row++)
            {
                // Add Row to gameGrid
                gameGrid.RowDefinitions.Add(new RowDefinition());

                // Iterate through columns
                for (int col = 0; col < Columns; col++)
                {
                    if (row == 0)
                        gameGrid.ColumnDefinitions.Add(new ColumnDefinition()); // Add Column to gameGrid

                    // Create button template for blank cell
                    Button button = new Button();
                    button.Style = Resources["CellButtonStyle"] as Style;
                    button.Content = ""; 
                    button.PreviewMouseLeftButtonDown += cellButton_PreviewMouseLeftButtonDown;
                    button.PreviewMouseLeftButtonUp += cellButton_PreviewMouseLeftButtonUp;
                    button.PreviewMouseRightButtonDown += cellButton_PreviewMouseRightButtonDown;

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
                                if (newRow >= 0 && newRow < Rows && newCol >= 0 && newCol < Columns)
                                    Cells[newRow, newCol].count++; // Update neighbor count
                            }
                        }
                    }
                }
            }
        }

        private void initializeGameGrid()
        {
            gameGrid.IsEnabled = true;
            initializeCellGrid();
            initializeMines();
            initializeCells();
        }

        private void exposeAllMines()
        {
            Cell[] cellsWithMines = Cells.Cast<Cell>().Where(cell => cell.hasMine).ToArray();
            foreach (Cell cell in cellsWithMines)
            {
                //cell.expose();
                cell.button.Content = Application.Current.Resources["Mine_Symbol"];
            }
        }

        private void gameOver()
        {
            gameGrid.IsEnabled = false;
            exposeAllMines();

            emojiSymbol.Text = "☠️";
        }

        private void handleMineClicked(object sender, EventArgs e)
        {
            // Call the gameOver() function here
            gameOver();
        }

        private void cellButton_PreviewMouseLeftButtonDown(object sender, RoutedEventArgs e)
        {
            emojiSymbol.Text = "😮";

            Button button = (Button)sender;
            int row = (int)button.GetValue(Grid.RowProperty);
            int col = (int)button.GetValue(Grid.ColumnProperty);

            if (Cells[row, col].isFlagged)
                return;

            // Change borders of clicked button
            Cells[row, col].exposeBorders();
        }

        private void cellButton_PreviewMouseLeftButtonUp(object sender, RoutedEventArgs e)
        {
            emojiSymbol.Text = "🙂";

            Button button = (Button)sender;
            int row = (int)button.GetValue(Grid.RowProperty);
            int col = (int)button.GetValue(Grid.ColumnProperty);

            if (Cells[row, col].isFlagged)
                return;

            // Reveal Cell
            exposeCell(row, col); 
        }

        private void cellButton_PreviewMouseRightButtonDown(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            int row = (int)button.GetValue(Grid.RowProperty);
            int col = (int)button.GetValue(Grid.ColumnProperty);

            // Change borders of clicked button
            Cells[row, col].updatedFlagged();
        }

        private void exposeCell(int row, int col)
        {
            // Break if out of range
            if (row < 0 || row >= Rows || col < 0 || col >= Columns || Cells[row, col].isExposed)
                return;

            // Expose button
            Cells[row, col].expose();

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

    }
}
