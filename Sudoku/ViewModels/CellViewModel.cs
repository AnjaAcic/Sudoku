using Sudoku.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Sudoku.ViewModels
{
    public class CellViewModel : NotifyBase
    {
        public Cell Model { get; }
        public string Value 
        {
            get => Model.Value?.ToString() ?? string.Empty;
            set 
            {
                if (int.TryParse(value, out int v) && v >= 1 && v <= 9)
                    Model.Value = v;
                else
                    Model.Value = null;
                Raise(nameof(Value));
                Raise(nameof(HasError));
            }

        }

        public Thickness BorderThickness
        {
            get
            {
                int idx = Model.Index; 
                int row = idx / 9;
                int col = idx % 9;

                double left = (col % 3 == 0) ? 2 : 1;
                double top = (row % 3 == 0) ? 2 : 1;
                double right = (col == 8) ? 2 : 1;
                double bottom = (row == 8) ? 2 : 1;

                return new Thickness(left, top, right, bottom);
            }
        }

        public bool IsFixed => Model.IsFixed ?? false;
        public bool HasError => Model.HasError ?? false;

        public CellViewModel(Cell model)
        {
            Model = model;
        }
    }
}
