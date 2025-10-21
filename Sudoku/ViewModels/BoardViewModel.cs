using Sudoku.Helpers;
using Sudoku.Models;
using Sudoku.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


 //TODO
namespace Sudoku.ViewModels
{
    public class BoardViewModel : NotifyBase
    {
        public ObservableCollection<CellViewModel> Cells { get; }

        public RelayCommand NewGameCommand { get; }
        public RelayCommand SaveCommand { get; }
        public RelayCommand LoadCommand { get; }
        public RelayCommand HintCommand { get; }
        public RelayCommand UndoCommand { get; }

        private readonly ISudokuGenerator _gen;
        private readonly IStorageService _sto;
        private readonly IValidationService _val;
        private readonly UndoRedoManager _undo;
        private readonly TimerService _timer;
        private int[,] _solution;

        public string Elapsed => _timer.Elapsed;

        public BoardViewModel()
        {
            Cells = new ObservableCollection<CellViewModel>(
                Enumerable.Range(0, 81).Select(i =>
                {
                    var cell = new Cell() { Index = i };
                    return new CellViewModel(cell);
                }));
        }

        public BoardViewModel(ISudokuGenerator gen,
            IValidationService val,
            IStorageService sto,
            UndoRedoManager undo,
            TimerService timer)
        { 
            _gen = gen;
            _val = val;
            _sto = sto;
            _undo = undo;
            _timer = timer;

            Cells = new ObservableCollection<CellViewModel>(
                Enumerable.Range(0, 81).Select(_ => new CellViewModel(new Cell())));

            NewGameCommand = new RelayCommand(_ => StartNew());
            SaveCommand = new RelayCommand(_ => Save());
            LoadCommand = new RelayCommand(_ => Load());
            HintCommand = new RelayCommand(_ => Hint());
            UndoCommand = new RelayCommand(_ => Undo(), _ => _undo.CanUndo);

            foreach (var (vm, i) in Cells.Select((c, i) => (c, i)))
                vm.Model.PropertyChanged += (_, e) =>
                {
                    if (e.PropertyName == nameof(Cell.Value))
                        OnCellChanged(i);
                };

            StartNew();
        }

        private void OnCellChanged(int idx)
        {
            var v = Cells[idx].Model.Value;
           // _undo.Do(new Edit(idx, v, old => Cells[idx].Model.Value = old));
            Validate(idx);
        }

        private void Validate(int idx)
        {
            var cur = new int?[9, 9];
            for (int i = 0; i < 81; i++) cur[i / 9, i % 9] = Cells[i].Model.Value;
            bool ok = _val.Validate(idx / 9, idx % 9, cur);
            Cells[idx].Model.HasError = !ok;
        }

        private void StartNew()
        {
            var (puz, sol) = _gen.Generate(Difficulty.Medium);
            _solution = sol;
            for (int i = 0; i < 81; i++)
            {
                Cells[i].Model.Value = puz[i / 9, i % 9];
                Cells[i].Model.IsFixed = puz[i / 9, i % 9].HasValue;
                Cells[i].Model.HasError = false;
            }
            _undo.Clear();
            _timer.Reset();
            _timer.Start();
            Raise(nameof(Elapsed));
        }

        private void Save()
        {
            var puz = new int?[9, 9];
            for (int i = 0; i < 81; i++) puz[i / 9, i % 9] = Cells[i].Model.Value;
            _sto.Save(puz, _solution, "save.json");
        }

        private void Load()
        {
            var (puz, sol) = _sto.Load("save.json");
            _solution = sol;
            for (int i = 0; i < 81; i++)
                Cells[i].Model.Value = puz[i / 9, i % 9];
            _timer.Reset();
            _timer.Start();
        }

        private void Hint()
        {
            var empties = Cells.Where(c => !c.Model.Value.HasValue).ToList();
            if (!empties.Any()) return;
            var vm = empties[new Random().Next(empties.Count)];
            int idx = Cells.IndexOf(vm);
            int correct = _solution[idx / 9, idx % 9];
            vm.Model.Value = correct;
        }

        private void Undo()
        {
            _undo.Undo();
            Raise(nameof(UndoCommand));
        }
    }
}
