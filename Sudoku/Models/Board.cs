using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace Sudoku.Models
{
    public class Board : INotifyPropertyChanged
    {
        public Cell[,] Cells { get; private set; } = new Cell[9, 9];
        public int[,] Solution { get; set; } = new int[9, 9];

        public Board() 
        {
            for (int i = 0; i < 9; i++)
                for (int j = 0; j < 9; j++)
                    Cells[i, j] = new Cell();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

    }
}
