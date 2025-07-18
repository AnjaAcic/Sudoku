using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sudoku.Services
{
    public interface IStorageService
    {
        void Save(int?[,] puzzle, int[,] solution, string path);
        (int?[,], int[,]) Load(string path);
    }
}
