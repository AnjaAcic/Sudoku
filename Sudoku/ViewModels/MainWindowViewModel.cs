using Sudoku.Helpers;
using Sudoku.Models;
using Sudoku.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Sudoku.ViewModels
{
    public class MainWindowViewModel : BaseViewModel
    {
        private readonly GameViewModel _gameVm = new GameViewModel();
        private const string Filename = "sudoku_game.json";

        private bool _canResume;

        public bool CanResume 
        { 
            get { return _canResume; }
            set { _canResume = value; }
        }


        public MainWindowViewModel()
        {
            ResumeCommand = new RelayCommand(_ => ResumeGame(), _ => CanResume);
            EasyCommand = new RelayCommand(_ => StartNew(Difficulty.Easy));
            MediumCommand = new RelayCommand(_ => StartNew(Difficulty.Medium));
            HardCommand = new RelayCommand(_ => StartNew(Difficulty.Hard));

            ChechIfCanResume();
        }

        public ICommand ResumeCommand { get; }
        public ICommand EasyCommand { get; }
        public ICommand MediumCommand { get; }
        public ICommand HardCommand { get; }

        public event Action<GameViewModel>? OnStartGame;

        private void StartNew(Difficulty diff)
        {
            _gameVm.NewGameCommand.Execute(diff);
            OnStartGame?.Invoke(_gameVm);
        }

        private void ResumeGame()
        {
            if (File.Exists(Filename))
            {
                try
                {
                    _gameVm.Load(Filename);
                    OnStartGame?.Invoke(_gameVm);
                }
                catch
                {
                    CanResume = false;
                }
            }
        }

        private void ChechIfCanResume()
        {
            if (!File.Exists(Filename))
            {
                CanResume = false;
                return;
            }

            try
            {
                var json = File.ReadAllText(Filename);
                var dto = JsonSerializer.Deserialize<SaveDto>(json);
                CanResume = dto != null && !dto.IsGameOver;
            }
            catch
            {
                CanResume = false;
            }
        }
    }

}
