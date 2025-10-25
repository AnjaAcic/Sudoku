using Sudoku.ViewModels;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;

namespace Sudoku.Views
{
    public partial class GameView : Window
    {
        private readonly GameViewModel _vm;
        private bool _isPaused;

        public GameView(GameViewModel? vm = null)
        {
            InitializeComponent();
            _vm = vm ?? new GameViewModel();
            DataContext = _vm;

            _vm.PropertyChanged += Vm_PropertyChanged;
        }

        private void Vm_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if(e.PropertyName == nameof(_vm.IsPaused))
            {
                if (_vm.IsPaused)
                    AnimateOverlay(PauseOverlay, Visibility.Visible, 0, 1);
                else
                    AnimateOverlay(PauseOverlay, Visibility.Collapsed, 1, 0);
            }
        }

        private void btnPause_Click(object sender, RoutedEventArgs e)
        {
            if (_isPaused) return;
            _isPaused = true;
            _vm.Pause();

            AnimateOverlay(PauseOverlay, Visibility.Visible, 0, 1);
        }

        private void btnResume_Click(object sender, RoutedEventArgs e)
        {
            if (!_isPaused) return;
            _isPaused = false;
            _vm.Resume();

            AnimateOverlay(PauseOverlay, Visibility.Collapsed, 1, 0);
        }

        private void btnRestart_Click(object sender, RoutedEventArgs e)
        {
            _vm.Reset();
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            new MainWindow().Show();
            Close();
        }

        private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !int.TryParse(e.Text, out int value) || value < 1 || value > 9;

            if (!e.Handled)
            {
                if (sender is TextBox tb && tb.Tag is int index)
                {
                    _vm.SelectCell(index);
                    _vm.EnterNumber(value);
                }
            }
        }

        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (sender is TextBox tb && int.TryParse(tb.Tag?.ToString(), out int index))
                _vm.SelectCell(index);
        }

        private void AnimateOverlay(UIElement element, Visibility target, double from, double to)
        {
            element.Visibility = Visibility.Visible;
            var anim = new DoubleAnimation(from, to, TimeSpan.FromMilliseconds(250));
            if (target == Visibility.Collapsed)
                anim.Completed += (s, e) => element.Visibility = Visibility.Collapsed;
            element.BeginAnimation(OpacityProperty, anim);
        }

        public void ShowGameOverOverlay()
        {
            GameOverOverlay.Visibility = Visibility.Visible;
            var anim = new DoubleAnimation(0, 1, TimeSpan.FromMilliseconds(300));
            GameOverOverlay.BeginAnimation(OpacityProperty, anim);
        }


        private void TextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Back || e.Key == Key.Delete)
            {
                if (sender is TextBox tb && tb.Tag is int index)
                {
                    _vm.SelectCell(index);
                    _vm.EnterNumber(0); 
                    e.Handled = true;
                }
            }
        }

    }
}
