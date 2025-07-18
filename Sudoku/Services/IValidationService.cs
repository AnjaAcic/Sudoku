using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sudoku.Services
{
    public interface IValidationService
    {
        bool Validate(int row, int column, int?[,] current);
    }
}
