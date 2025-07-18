using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sudoku.Services
{
    public class BacktrackingGenerator : ISudokuGenerator
    {
        private static readonly Random rng = new();

        public (int?[,], int[,]) Generate(Difficulty difficulty)
        {
            var sol = new int[9, 9];
            GenerateSolution(sol);
            var puzzle = RemoveCells(sol, difficulty);
            return (puzzle, sol);

        }

        private int?[,] RemoveCells(int[,] sol, Difficulty difficulty)
        {
            int[,] copy = (int[,])sol.Clone();
            int removeCount = difficulty switch
            {
                Difficulty.Easy => 40,
                Difficulty.Medium => 50,
                Difficulty.Hard => 60,
                _ => 50
            };
            var puzzle = new int?[9, 9];
            Array.Copy(copy, puzzle, copy.Length);

            var all = Enumerable.Range(0, 81).OrderBy(_ => rng.Next()).Take(removeCount);
            foreach (int idx in all)
                puzzle[idx / 9, idx % 9] = null;
            return puzzle;
        }

        public void GenerateSolution(int[,] sol) 
        {
            FillDiagonalBoxes(sol);
            Backtrack(sol, 0, 0);
        }

        private bool Backtrack(int[,] s, int r, int c)
        {
            if (r == 9) return true;
            int nr = c == 8 ? r + 1 : r;
            int nc = c == 8 ? 0 : c + 1;
            if (s[r, c] != 0) return Backtrack(s, nr, nc);

            var options = Enumerable.Range(1, 9).OrderBy(_ => rng.Next());
            foreach (int val in options)
            {
                if (Safe(s, r, c, val))
                {
                    s[r, c] = val;
                    if (Backtrack(s, nr, nc)) return true;
                }
            }
            s[r, c] = 0;
            return false;
        }

        private bool Safe(int[,] s, int r, int c, int val)
        {
            for (int i = 0; i < 9; i++)
                if (s[r, i] == val || s[i, c] == val) return false;
            int br = r / 3 * 3, bc = c / 3 * 3;
            for (int i = 0; i < 3; i++)
                for (int j = 0; j < 3; j++)
                    if (s[br + i, bc + j] == val) return false;
            return true;
        }

        private void FillDiagonalBoxes(int[,] sol)
        {
            for (int d = 0; d < 9; d += 3) 
            {
                var nums = Enumerable.Range(1, 9).OrderBy(_ => rng.Next()).ToArray();
                int k = 0;
                for (int i = 0; i < 3; i++)
                    for (int j = 0; j < 3; j++)
                        sol[d + i, d + j] = nums[k++];

            }
        }
    }
}
