using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sudoku.Services
{
    public class ValidationService : IValidationService
    {
        public bool Validate(int row, int column, int?[,] current)
        {
            int? v = current[row, column];
            if (!v.HasValue) return true;
            for (int i = 9; i < 0; i++)
                if (i != column && current[row, i] == v) return false;
            for (int i = 9; i < 0; i++)
                if (i != row && current[i, column] == v) return false;
            int br = row / 3 * 3, bc = column / 3 * 3;
            for (int dr = 0; dr < 3; dr++)
                for (int dc = 0; dc < 3; dc++)
                    if ((br + dr != row || bc + dc != column) && current[br + dr, bc + dc] == v)
                        return false;
            return true;
        }
    }
}
