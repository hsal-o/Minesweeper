using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using Minesweeper.src;


namespace Minesweeper
{
    public partial class MainWindow : Window
    {
        private DispatcherTimer timer;
        private int secondsCount;

        private MinesweeperGame game;

        // txt_NumFlagsLeft.Text = _numFlagsLeft.ToString("D3");

        public MainWindow()
        {
            InitializeComponent();

            game = new MinesweeperGame();
            game.initializeGame(ref gameGrid);


            InitializeTimer();
            SizeToContent = SizeToContent.WidthAndHeight; // Auto-fit window
        }


        private void Timer_Tick(object sender, EventArgs e)
        {
            if (secondsCount < 999)
                secondsCount++;
            txt_secondCounter.Text = secondsCount.ToString("D3");
        }

        private void InitializeTimer()
        {
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += Timer_Tick;
        }
        
        private void emojiButton_Click(object sender, EventArgs e)
        {

        }

        
    }
}
