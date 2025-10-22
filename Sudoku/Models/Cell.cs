using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Sudoku.Models
{
    public class Cell : INotifyPropertyChanged
    {
        private int? _value;
        private bool _isGiven;
        private bool _isError;
        private bool _isSelected;

        public int Row { get; }
        public int Col { get; }
        public int Index { get; }

        public int? Value
        {
            get => _value;
            set
            {
                if(_value != value)
                {
                    _value = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool IsGiven
        {
            get => _isGiven;
            set
            {
                if(_isGiven != value)
                {
                    _isGiven = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool IsError
        {
            get => _isError;
            set
            {
                if (_isError != value)
                {
                    _isError = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                if (_isSelected != value)
                {
                    _isSelected = value;
                    OnPropertyChanged();
                }
            }
        }

        public Cell(int row, int col)
        {
            Row = row;
            Col = col;
            Index = row * 9 + col;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
