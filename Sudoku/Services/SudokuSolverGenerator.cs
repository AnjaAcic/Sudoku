using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sudoku.Services
{
    public class SudokuSolverGenerator
    {
        private static Random _rand = new Random();

        public static bool Solve(int?[,] board)
        {
            for (int r = 0; r < 9; r++)
            {
                for (int c = 0; c < 9; c++)
                {
                    if (board[r, c] == null)
                    {
                        for (int v = 1; v <= 9; v++)
                        {
                            if (IsSafe(board, r, c, v))
                            {
                                board[r, c] = v;
                                if (Solve(board)) return true;
                                board[r, c] = null;
                            }
                        }
                        return false;
                    }
                }
            }
            return true;
        }

        public static bool IsSafe(int?[,] board, int row, int col, int val)
        {
            for (int i = 0; i < 9; i++)
            {
                if (board[row, i] == val) return false;
                if (board[i, col] == val) return false;
            }

            int sr = (row / 3) * 3, sc = (col / 3) * 3;
            for(int r = sr; r < sr+3; r++)
            {
                for(int c = sc; c < sc+3; c++)
                {
                    if (board[r, c] == val) return false;
                }
            }

            return true;
        }

        public static int?[,] GeneratePuzzle(string difficulty)
        {
            int?[,] board = new int?[9,9];

            FillDiagolanBlocks(board);

            Solve(board);

            int removals = difficulty.ToLower() switch
            {
                "easy" => 36,
                "medium" => 48,
                "hard" => 54,
                _ => 48
            };

            RemoveNumbers(board, removals);

            return board;
        }

        private static void RemoveNumbers(int?[,] board, int count)
        {
            int removed = 0;
            while(removed < count)
            {
                int r = _rand.Next(9), c = _rand.Next(9);
                if (board[r,c] != null)
                {
                    board[r, c] = null;
                    removed++;
                }
            }
        }

        private static void FillDiagolanBlocks(int?[,] board)
        {
            for (int i = 0; i < 9; i++)
                FillBlock(board, i, i);
        }

        private static void FillBlock(int?[,] board, int row, int col)
        {
            var nums = Enumerable.Range(1, 9).ToArray();
            Shuffle(nums);
            int idx = 0;
            for (int r = 0; r < 9; r++)
                for (int c = 0; c < 9; c++)
                    board[r, c] = nums[idx++];
        }

        private static void Shuffle(int[] nums)
        {
            for (int i = nums.Length - 1; i > 0; i--)
            {
                int j = _rand.Next(i + 1);
                var t = nums[i];
                nums[i] = nums[j];
                nums[j] = t;
            }
        }
    }
}
