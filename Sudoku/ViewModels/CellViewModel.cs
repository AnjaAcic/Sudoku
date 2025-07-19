using Sudoku.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

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

        public bool IsFixed => Model.IsFixed ?? false;
        public bool HasError => Model.HasError ?? false;

        public CellViewModel(Cell model)
        {
            Model = model;
        }
    }
}
