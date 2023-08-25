using System;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using Minesweeper.Controls;
using Minesweeper.src;


namespace Minesweeper
{
    public partial class MainWindow : Window
    {
        private DispatcherTimer timer;
        private int secondsCount;

        private DifficultyModes difficultyModes;
        private MinesweeperGame game;

        public MainWindow()
        {
            InitializeComponent();

            difficultyModes = new DifficultyModes();
            initializeGame(difficultyModes.BeginnerMode);

            initializeTimer();

            SizeToContent = SizeToContent.WidthAndHeight; // Auto-fit window
        }

        public void initializeGame(Difficulty selectedDifficulty)
        {
            game = new MinesweeperGame(selectedDifficulty);
            game.GameStarted += Minesweeper_GameStarted;
            game.GameLost += Minesweeper_GameLost;
            game.GameWon += Minesweeper_GameWon;
            game.GameReset += Minesweeper_GameReset;
            game.UpdateNumFlagsLeft += Minesweeper_UpdateNumFlagsLeft;
            game.CellLeftClickDown += Minesweeper_CellLeftClickDown;
            game.CellLeftClickUp += Minesweeper_CellLeftClickUp;
            game.initializeGame(ref gameGrid);
        }

        public void resetGameDifficulty(Difficulty selectedDifficulty)
        {
            game.resetDifficulty(selectedDifficulty, ref gameGrid);
        }

        #region Minesweeper UI EventHandlers
        private void Minesweeper_UpdateNumFlagsLeft(object sender, IntEventArgs args)
        {
            // Update Number of remaining flags UI
            updateNumFlagsLeftUI(args.value);
        }

        private void Minesweeper_CellLeftClickDown(object sender, EventArgs e)
        {
            // Update Emoji
            updateEmojiSymbol(EmojiButton.OpenMouth_Emoji);
        }

        private void Minesweeper_CellLeftClickUp(object sender, EventArgs e)
        {
            // Update Emoji
            updateEmojiSymbol(EmojiButton.Smile_Emoji);
        }

        private void Minesweeper_GameStarted(object sender, EventArgs e)
        {
            // Update Emoji
            updateEmojiSymbol(EmojiButton.Smile_Emoji);

            // Update timer
            secondsCount = 1; // Game starts with 1 second
            updateTimerUI();
            timer.Start();
        }

        private void Minesweeper_GameReset(object sender, EventArgs e)
        {
            // Update Emoji
            updateEmojiSymbol(EmojiButton.Smile_Emoji);

            // Update Timer
            timer.Stop();
            secondsCount = 0;
            updateTimerUI();
        }

        private void Minesweeper_GameLost(object sender, EventArgs e)
        {
            // Update Emoji
            updateEmojiSymbol(EmojiButton.Lose_Emoji);

            // Update Timer
            timer.Stop();
        }

        private void Minesweeper_GameWon(object sender, EventArgs e)
        {
            updateEmojiSymbol(EmojiButton.Win_Emoji);
            updateNumFlagsLeftUI(0);
            Debug.WriteLine("Game Won!");
            timer.Stop();
        }
        #endregion

        #region UI methods
        private void initializeTimer()
        {
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += timer_Tick;
        }
        private void updateTimerUI()
        {
            txt_secondCounter.Text = secondsCount.ToString("D3");
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            if (secondsCount < 999)
                secondsCount++;

            updateTimerUI();
        }

        // Method to directly update Emoji
        private void updateEmojiSymbol(string value)
        {
            emojiButton.setSymbol(value);
        }
        private void emojiButton_Click(object sender, EventArgs e)
        {
            game.restartGame(ref gameGrid);
        }

        private void updateNumFlagsLeftUI(int value)
        {
            txt_NumFlagsLeft.Text = value.ToString("D3");
        }
        #endregion

        #region Button click events
        private void Button_GameClick(object sender, RoutedEventArgs e)
        {
            GameSettingsWindow gameSettingsWindow = new GameSettingsWindow();
            gameSettingsWindow.Show();
        }

        private void Button_HelpClick(object sender, RoutedEventArgs e)
        {

        }
        #endregion
    }
}
