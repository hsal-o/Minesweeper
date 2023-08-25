using Minesweeper.src;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Minesweeper
{
    public partial class GameSettingsWindow : Window
    {
        private DifficultyVM vm;
        public GameSettingsWindow()
        {
            InitializeComponent();
            vm = (DifficultyVM) this.DataContext;
            SizeToContent = SizeToContent.WidthAndHeight; // Auto-fit window
        }

        private void newGameButton_Click(object sender, RoutedEventArgs e)
        {
            Difficulty selectedDifficulty = vm.DifficultyList.FirstOrDefault(difficulty => difficulty.IsSelected);

            if(selectedDifficulty != null)
            {
                Debug.WriteLine("not Null!!");
                MainWindow mainWindow = Application.Current.MainWindow as MainWindow;
                if(mainWindow != null)
                {
                    mainWindow.resetGameDifficulty(selectedDifficulty);
                }
            }
            else
                Debug.WriteLine("we Null :/");

            Close();
        }
    }
}
