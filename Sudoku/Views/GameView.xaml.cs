using System;
using System.Collections.Generic;
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
using System.Windows.Threading;

namespace Sudoku.Views
{
    /// <summary>
    /// Interaction logic for GameView.xaml
    /// </summary>
    public partial class GameView : Window
    {
        private DispatcherTimer gameTimer;
        private TimeSpan elapsedTime;
        private int mistakeCount;
        private bool isPaused;

        public GameView()
        {
            InitializeComponent();
            InitializeGame();
        }

        private void InitializeGame()
        {
            elapsedTime = TimeSpan.Zero;
            mistakeCount = 0;
            isPaused = false;

            gameTimer = new DispatcherTimer();
            gameTimer.Interval = TimeSpan.FromSeconds(1);
            gameTimer.Tick += Timer_Tick;
            gameTimer.Start();
        }

        private void Timer_Tick(object? sender, EventArgs e)
        {
            if(!isPaused)
            {
                elapsedTime = elapsedTime.Add(TimeSpan.FromSeconds(1));
            }
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            
        }

        private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {

        }

        private void btnPause_Click(object sender, RoutedEventArgs e)
        {
            isPaused = true;
            PauseOverlay.Visibility = Visibility.Collapsed;
            gameTimer.Stop();

            PauseTime.Text = elapsedTime.ToString(@"mm\ss");
            PauseMistake.Text = $"{mistakeCount}/3";
        }

        private void btnResume_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnExit_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
