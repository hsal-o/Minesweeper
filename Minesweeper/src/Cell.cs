using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows;

namespace Minesweeper.src
{
    public class Cell
    {
        private int row { get; set; }
        public int column { get; set; }
        public int count { get; set; }
        public bool hasMine { get; set; }
        public bool isExposed { get; set; }
        public bool isFlagged { get; set; }
        public Button button { get; set; }

        public event EventHandler MineClicked;
        public event EventHandler FlagUpdate;

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

            FlagUpdate?.Invoke(this, EventArgs.Empty);

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

            // If the clicked cell has a mine
            if (hasMine)
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
}
