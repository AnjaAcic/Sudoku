using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sudoku.Services
{
    public enum Difficulty { Easy, Medium, Hard}
    public interface ISudokuGenerator
    {
        (int?[,], int[,]) Generate(Difficulty difficulty);
        // Prvi tuple: pocetna tabla s prazninama, drugi: kompletno rjeenje
    }
}
