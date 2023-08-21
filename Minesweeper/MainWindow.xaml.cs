using System;
using System.Diagnostics;
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


        public MainWindow()
        {
            InitializeComponent();

            game = new MinesweeperGame();
            game.GameStarted += Minesweeper_GameStarted;
            game.GameLost += Minesweeper_GameLost;
            game.GameWon += Minesweeper_GameWon;
            game.GameReset += Minesweeper_GameReset;
            game.UpdateNumFlagsLeft += Minesweeper_UpdateNumFlagsLeft;
            game.CellLeftClickDown += Minesweeper_CellLeftClickDown;
            game.CellLeftClickUp += Minesweeper_CellLeftClickUp;
            game.initializeGame(ref gameGrid);


            InitializeTimer();

            SizeToContent = SizeToContent.WidthAndHeight; // Auto-fit window
        }

        private void Minesweeper_UpdateNumFlagsLeft(object sender, IntEventArgs args)
        {
            updateNumFlagsLeft(args.value);

            Debug.WriteLine("args.value: " + args.value);
        }

        private void Minesweeper_CellLeftClickDown(object sender, EventArgs e)
        {
            emojiButton.setSymbol("😮‍");
        }

        private void Minesweeper_CellLeftClickUp(object sender, EventArgs e)
        {
            emojiButton.setSymbol("🙂");
        }

        private void Minesweeper_GameStarted(object sender, EventArgs e)
        {
            emojiButton.setSymbol("🙂");
            Debug.WriteLine("Game Started!");
            secondsCount = 1;
            updateTimer();
            timer.Start();
        }

        private void Minesweeper_GameReset(object sender, EventArgs e)
        {
            emojiButton.setSymbol("🙂");
            Debug.WriteLine("Game Reset!");
            timer.Stop();
            secondsCount = 0;
            updateTimer();
        }

        private void Minesweeper_GameLost(object sender, EventArgs e)
        {
            emojiButton.setSymbol("☠️");
            Debug.WriteLine("Game Lost!");
            timer.Stop();
        }

        private void Minesweeper_GameWon(object sender, EventArgs e)
        {
            emojiButton.setSymbol("😎");
            updateNumFlagsLeft(0);
            Debug.WriteLine("Game Won!");
            timer.Stop();
        }


        private void Timer_Tick(object sender, EventArgs e)
        {
            if (secondsCount < 999)
                secondsCount++;

            updateTimer();
        }

        private void updateTimer()
        {
            txt_secondCounter.Text = secondsCount.ToString("D3");
        }

        private void updateNumFlagsLeft(int value)
        {
            txt_NumFlagsLeft.Text = value.ToString("D3");
        }

        private void InitializeTimer()
        {
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += Timer_Tick;
        }
        
        private void emojiButton_Click(object sender, EventArgs e)
        {
            game.restartGame(ref gameGrid);
        }
        
    }
}
