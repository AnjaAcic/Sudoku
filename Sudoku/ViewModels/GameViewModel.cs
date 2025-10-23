using Sudoku.Helpers;
using Sudoku.Models;
using Sudoku.Services;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using System.Windows.Threading;

namespace Sudoku.ViewModels
{
    public class GameViewModel : INotifyPropertyChanged
    {
        public Board Board { get; } = new Board();
        private readonly UndoService _undo = new UndoService();
        private readonly DispatcherTimer _timer;
        private int _elapsedSeconds;
        private bool _showErrors = true;
        private int _selectedIndex = -1;
        private string _difficulty = "medium";
        private string _initialDifficulty = "medium";
        private int _mistakes;

        public GameViewModel()
        {
            NewGameCommand = new RelayCommand(o => NewGame((o?.ToString() ?? "medium")));
            CellClickCommand = new RelayCommand(o => SelectCell(ConvertToInt(o)));
            NumberCommand = new RelayCommand(o => EnterNumber(ConvertToInt(o)));
            UndoCommand = new RelayCommand(o => Undo(), o => _undo.CanUndo);
            HintCommand = new RelayCommand(o => Hint());
            SaveCommand = new RelayCommand(o => Save(o?.ToString() ?? "sudoku_save.json"));
            LoadCommand = new RelayCommand(o => Load(o?.ToString() ?? "sudoku_save.json"));
            ResetCommand = new RelayCommand(o => Reset());
            EraseCommand = new RelayCommand(o => EnterNumber(0));


            _timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
            _timer.Tick += (s, e) => ElapsedSeconds++;

            NewGame("medium");
            _timer.Start();
        }

        private int ConvertToInt(object? o)
        {
            if (o == null) return -1;
            return int.TryParse(o.ToString(), out int result) ? result : 1;
        } 

        public ICommand NewGameCommand { get; }
        public ICommand CellClickCommand { get; }
        public ICommand NumberCommand { get; }
        public ICommand UndoCommand { get; }
        public ICommand HintCommand { get; }
        public ICommand SaveCommand { get; }
        public ICommand LoadCommand { get; }
        public ICommand ResetCommand { get; }
        public ICommand EraseCommand { get; }


        public bool ShowErrors
        {
            get => _showErrors;
            set { _showErrors = value; OnPropertyChanged(); Board.ValidateAll(_showErrors); }
        }

        public string TimerDisplay => TimeSpan.FromSeconds(_elapsedSeconds).ToString(@"mm\:ss");

        public int ElapsedSeconds
        {
            get => _elapsedSeconds;
            private set { _elapsedSeconds = value; OnPropertyChanged(nameof(TimerDisplay)); }
        }

        public int Mistakes
        {
            get => _mistakes;
            set { _mistakes = value; OnPropertyChanged(nameof(MistakeDisplay)); }
        }

        public string MistakeDisplay => $"{Mistakes}/3";

        public string Difficulty
        {
            get => _difficulty;
            set { _difficulty = value; OnPropertyChanged(); }
        }

        public int SelectedIndex
        {
            get => _selectedIndex;
            set
            {
                if (_selectedIndex >= 0) Board.GetCell(_selectedIndex).IsSelected = false;
                _selectedIndex = value;
                if (_selectedIndex >= 0) Board.GetCell(_selectedIndex).IsSelected = true;
                OnPropertyChanged();
            }
        }

        public void NewGame(string difficulty)
        {
            Difficulty = difficulty;
            _initialDifficulty = difficulty;
            _undo.Clear();
            ElapsedSeconds = 0;
            Mistakes = 0;
            var arr = SudokuSolverGenerator.GeneratePuzzle(difficulty);
            Board.LoadFromArray(arr);
            Board.ValidateAll(ShowErrors);
        }

        public void SelectCell(int index)
        {
            if (index < 0 || index >= 81) return;
            SelectedIndex = index;
        }

        public void EnterNumber(int number)
        {
            if (SelectedIndex < 0) return;
            var cell = Board.GetCell(SelectedIndex);
            if (cell.IsGiven) return;

            var old = cell.Value;
            cell.Value = number == 0 ? null : (int?)number;

            _undo.Push(new Move(SelectedIndex, old, cell.Value));
            (UndoCommand as RelayCommand)?.RaiseCanExecuteChanged();

            Board.ValidateAll(ShowErrors);
        }

        public void Undo()
        {
            var move = _undo.Pop();
            if (move is null) return;
            var cell = Board.GetCell(move.Index);
            cell.Value = move.OldValue;
            Board.ValidateAll(ShowErrors);
            (UndoCommand as RelayCommand)?.RaiseCanExecuteChanged();
        }

        public void Hint()
        {
            var arr = Board.ToArray();
            var copy = new int?[9, 9];
            Array.Copy(arr, copy, arr.Length);

            if (SudokuSolverGenerator.Solve(copy))
            {
                for (int r = 0; r < 9; r++)
                    for (int c = 0; c < 9; c++)
                    {
                        if (arr[r, c] == null)
                        {
                            var idx = r * 9 + c;
                            var cell = Board.GetCell(idx);
                            var old = cell.Value;
                            cell.Value = copy[r, c];
                            _undo.Push(new Move(idx, old, cell.Value));
                            Board.ValidateAll(ShowErrors);
                            (UndoCommand as RelayCommand)?.RaiseCanExecuteChanged();
                            return;
                        }
                    }
            }
        }

        public void Save(string path)
        {
            PersistenceService.Save(Board, Difficulty, ElapsedSeconds, Mistakes, path);
        }

        public void Load(string path)
        {
            var dto = PersistenceService.Load(path);
            Difficulty = dto.Difficulty;
            ElapsedSeconds = dto.ElapsedSeconds;
            Mistakes = dto.Mistakes;
            Board.LoadFromArray(dto.Cells);
            Board.ValidateAll(ShowErrors);
        }

        public void Reset()
        {
            Board.ResetToGiven();
            _undo.Clear();
            ElapsedSeconds = 0;
            Mistakes = 0;
            Difficulty = _initialDifficulty;
            Board.ValidateAll(ShowErrors);
            (UndoCommand as RelayCommand)?.RaiseCanExecuteChanged();
        }

        public void Pause() => _timer.Stop();
        public void Resume() => _timer.Start();

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
