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

        public MainWindow()
        {
            InitializeComponent();
            var vm = new MainWindowViewModel();
            vm.OnStartGame += StartGame;
            DataContext = vm;
        }

        private void StartGame(GameViewModel gameVm)
        {
            var game = new GameView(gameVm);
            game.Show();
            this.Close();
        }

    }
}
