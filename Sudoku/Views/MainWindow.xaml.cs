using Sudoku.ViewModels;
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

namespace Sudoku.Views
{
    public partial class MainWindow : Window
    {
        private readonly GameViewModel _vm;

        public MainWindow()
        {
            InitializeComponent();
            _vm = DataContext as GameViewModel ?? new GameViewModel();
            DataContext = _vm;
        }

        private void btnResume_Click(object sender, RoutedEventArgs e)
        {
            var game = new GameView(_vm);
            game.Show();
            this.Close();
        }

        private void btnEasy_Click(object sender, RoutedEventArgs e) => OpenNew("easy");

        private void btnMedium_Click(object sender, RoutedEventArgs e) => OpenNew("medium");

        private void btnHard_Click(object sender, RoutedEventArgs e) => OpenNew("hard");

        private void OpenNew(string difficulty)
        {
            var vm = new GameViewModel();
            vm.NewGameCommand.Execute(difficulty);
            var game = new GameView(vm);
            game.Show();
            this.Close();
        }
    }
}
