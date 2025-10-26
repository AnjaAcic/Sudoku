using Sudoku.Helpers;
using Sudoku.Models;
using Sudoku.Services;
using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using System.Windows.Threading;

namespace Sudoku.ViewModels
{
    public class GameViewModel : BaseViewModel
    {

        public Board Board { get; } = new Board();
        private readonly UndoService _undo = new UndoService();
        private readonly DispatcherTimer _timer;
       
        public GameViewModel()
        {
            NewGameCommand = new RelayCommand(o => 
            {
                if (Enum.TryParse(o?.ToString(), true, out Difficulty diff))
                    NewGame(diff);
            });
            CellClickCommand = new RelayCommand(o => SelectCell(ConvertToInt(o)));
            NumberCommand = new RelayCommand(o => EnterNumber(ConvertToInt(o)));
            UndoCommand = new RelayCommand(o => Undo(), o => _undo.CanUndo);
            HintCommand = new RelayCommand(o => Hint());
            RestartCommand = new RelayCommand(o => Reset());
            EraseCommand = new RelayCommand(o => EnterNumber(0));
            BackCommand = new RelayCommand(o => GoToMain());
            PauseCommand = new RelayCommand(o => PauseGame());
            ResumeCommand = new RelayCommand(o => ResumeGame());

            Score = 0;

            _timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
            _timer.Tick += (s, e) => ElapsedSeconds++;

            _timer.Start();
        }

        private void GoToMain()
        {
            IsPaused = false;
            IsGameOver = false;
            Pause();

            if(!IsGameOver)
                Save();

            OnRequestMainMenu?.Invoke(this, EventArgs.Empty);
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
        public ICommand RestartCommand { get; }
        public ICommand EraseCommand { get; }
        public ICommand BackCommand { get; }
        public ICommand PauseCommand { get; }
        public ICommand ResumeCommand { get; }


        private bool _showErrors = true;
        public bool ShowErrors
        {
            get => _showErrors;
            set { _showErrors = value; OnPropertyChanged(); Board.ValidateAll(_showErrors); }
        }

        public string TimerDisplay => TimeSpan.FromSeconds(_elapsedSeconds).ToString(@"mm\:ss");

        private int _elapsedSeconds;
        public int ElapsedSeconds
        {
            get => _elapsedSeconds;
            private set { _elapsedSeconds = value; OnPropertyChanged(nameof(TimerDisplay)); }
        }


        private int _mistakes;
        public int Mistakes
        {
            get => _mistakes;
            set 
            { 
                _mistakes = value; 
                OnPropertyChanged(nameof(MistakeDisplay)); 
                OnPropertyChanged(nameof(Mistakes)); 
            }
        }

        public string MistakeDisplay => $"{Mistakes}/3";


        private Difficulty _difficulty;
        public Difficulty Difficulty
        {
            get => _difficulty;
            set { _difficulty = value; OnPropertyChanged(); }
        }


        private int _selectedIndex = -1;
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


        private bool _isGameOver;
        public bool IsGameOver
        {
            get => _isGameOver;
            set
            {
                _isGameOver = value;
                OnPropertyChanged();
            }
        }


        private int _score;
        public int Score
        {
            get => _score;
            set
            {
                _score = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(ScoreDisplay));
            }
        }

        public string ScoreDisplay => _score.ToString();


        private string _gameOverMessage;
        public string GameOverMessage
        {
            get => _gameOverMessage;
            set
            {
                _gameOverMessage = value;
                OnPropertyChanged();
            }
        }


        private int _starCount;
        public int StarCount
        {
            get => _starCount;
            set
            {
                _starCount = value;
                OnPropertyChanged();
            }
        }

        private bool _isPaused;

        public bool IsPaused
        {
            get { return _isPaused; }
            set 
            {    
                _isPaused = value;
                OnPropertyChanged();
            }
        }

        public void NewGame(Difficulty difficulty)
        {
            _difficulty = difficulty;
            Difficulty = difficulty;
            _undo.Clear();
            ElapsedSeconds = 0;
            Mistakes = 0;
            Score = 0;
            var arr = SudokuSolverGenerator.GeneratePuzzle(difficulty);
            Board.LoadFromArray(arr);
            Board.ValidateAll(ShowErrors);
            Resume();
            Save();
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

            Board.ValidateAll(ShowErrors);

            if (ShowErrors && number != 0 && cell.IsError) {
                Mistakes++;
                Score -= 5;
            }
            else if(number != 0 && !cell.IsError)
            {
                Score += 10;
            }

            CheckGameOver();

            _undo.Push(new Move(SelectedIndex, old, cell.Value));
            (UndoCommand as RelayCommand)?.RaiseCanExecuteChanged();

            Save();

        }

        public void Undo()
        {
            var move = _undo.Pop();
            if (move is null) return;
            var cell = Board.GetCell(move.Index);
            cell.Value = move.OldValue;
            Board.ValidateAll(ShowErrors);
            (UndoCommand as RelayCommand)?.RaiseCanExecuteChanged();

            Save();
        }

        public void Hint()
        {
            var arr = Board.ToArray();
            var copy = new int?[9][];
            Array.Copy(arr, copy, arr.Length);

            if (SudokuSolverGenerator.Solve(copy))
            {
                for (int r = 0; r < 9; r++)
                    for (int c = 0; c < 9; c++)
                    {
                        if (arr[r][c] == null)
                        {
                            var idx = r * 9 + c;
                            if(idx == SelectedIndex)
                            {
                                var cell = Board.GetCell(idx);
                                var old = cell.Value;
                                cell.Value = copy[r][c];
                                _undo.Push(new Move(idx, old, cell.Value));
                                Board.ValidateAll(ShowErrors);
                                (UndoCommand as RelayCommand)?.RaiseCanExecuteChanged();
                                return;
                            }  
                        }
                    }
            }
        }

        public void Save()
        {

            PersistenceService.Save(Board, Difficulty, Score, ElapsedSeconds, Mistakes, IsGameOver && !Board.IsComplete() ? false : IsGameOver);
        }

        public void Load()
        {
            var dto = PersistenceService.Load();
            if (dto == null) return;

            Difficulty = dto.Difficulty;
            ElapsedSeconds = dto.ElapsedSeconds;
            Mistakes = dto.Mistakes;
            Score = dto.Score;
            Board.LoadFromArray(dto.Cells, dto.Given);
            Board.ValidateAll(ShowErrors);
        }

        public void Reset()
        {
            Board.ResetToGiven();
            _undo.Clear();

            ElapsedSeconds = 0;
            Mistakes = 0;
            Score = 0;
            Difficulty = _difficulty;
            IsGameOver = false;

            IsPaused = false;

            Board.ValidateAll(ShowErrors);

            Resume();

            (UndoCommand as RelayCommand)?.RaiseCanExecuteChanged();
        }

        public void Pause()
        {
            if(_timer.IsEnabled)
                _timer.Stop();
        } 
        public void Resume(){
            if(!IsGameOver)
                _timer.Start();
        } 

        private void CheckGameOver()
        {
            if (Mistakes >= 3)
            {
                EndGame(false);
            }
            else if (Board.IsComplete())
            {
                EndGame(true);
            }
        }

        private void EndGame(bool isWin)
        {
            Pause();
            IsGameOver = true;

            GameOverMessage = isWin ? "Congratulations! You solved it!" : "Game Over! 3 mistakes reached!";

            if (isWin)
            {
                if (Mistakes == 0) 
                    StarCount = 3;
                else if (Mistakes == 1) 
                    StarCount = 2;
                else 
                    StarCount = 1;
            }
            else
            {
                StarCount = 0;
            }

            PersistenceService.Delete();

            OnGameOver?.Invoke(this, EventArgs.Empty);
        }

        private void PauseGame()
        {
            Pause();
            IsPaused = true;
        }

        private void ResumeGame()
        {
            Resume();
            IsPaused = false;
        }

        public event EventHandler? OnRequestMainMenu;
        public event EventHandler? OnGameOver;
    }
}
