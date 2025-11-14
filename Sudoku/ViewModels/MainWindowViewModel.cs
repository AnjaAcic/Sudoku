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

        private bool _canResume;
        public bool CanResume
        {
            get => _canResume;
            set
            {
                if (_canResume != value)
                {
                    _canResume = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(CanResume));
                }
            }
        }

        public MainWindowViewModel()
        {

            ResumeCommand = new RelayCommand(_ => ResumeGame(), _ => CanResume);
            EasyCommand = new RelayCommand(_ => StartNew(Difficulty.Easy));
            MediumCommand = new RelayCommand(_ => StartNew(Difficulty.Medium));
            HardCommand = new RelayCommand(_ => StartNew(Difficulty.Hard));

            _gameVm.OnGameOver += (s, e) => UpdateCanResume();
            UpdateCanResume();
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
            var dto = PersistenceService.Load();
            if (dto == null)
            {
                CanResume = false;
                return;
            }

            _gameVm.Load();
            OnStartGame?.Invoke(_gameVm);
            UpdateCanResume();
        }

        private void UpdateCanResume()
        {
            var dto = PersistenceService.Load();
            CanResume = dto != null && !dto.IsGameOver;
            OnPropertyChanged(nameof(CanResume));
            (ResumeCommand as RelayCommand)?.RaiseCanExecuteChanged();
        }

    }

}
