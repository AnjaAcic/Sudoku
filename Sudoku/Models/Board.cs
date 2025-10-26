using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace Sudoku.Models
{
    public class Board
    {
        public ObservableCollection<Cell> Cells { get; } = new ObservableCollection<Cell>();

        public Board()
        {
            for (int r = 0; r < 9; r++)
                for (int c = 0; c < 9; c++)
                    Cells.Add(new Cell(r, c));
        }

        public Cell GetCell(int row, int col) => Cells[row * 9 + col];
        public Cell GetCell(int index) => Cells[index];

        public int?[][] ToArray()
        {
            var arr = new int?[9][];
            for (int i = 0; i < 9; i++)
                arr[i] = new int?[9];

            foreach (var cell in Cells)
                arr[cell.Row][cell.Col] = cell.Value;
            return arr;
        }

        public void LoadFromArray(int?[][] arr)
        {
            for (int r = 0; r < 9; r++)
                for (int c = 0; c < 9; c++)
                {
                    var cell = GetCell(r, c);
                    cell.Value = arr[r][c];
                    cell.IsGiven = arr[r][c].HasValue;
                    cell.IsError = false;
                }
        }


        public void LoadFromArray(int?[][] values, bool[][] given)
        {
            for(int r = 0; r < 9; r++)
                for(int c =0; c < 9; c++)
                {
                    var cell = GetCell(r, c);
                    cell.Value = values[r][c];
                    cell.IsGiven = given[r][c];
                    cell.IsError = false;
                }
        }

        public bool[][] ToGivenArray()
        {
            var arr = new bool[9][];
            for (int i = 0; i < 9; i++)
                arr[i] = new bool[9];

            foreach (var cell in Cells)
            {
                arr[cell.Row][cell.Col] = cell.IsGiven; 
            }

            return arr;
        }

        public void ResetToGiven()
        {
            foreach (var cell in Cells)
                if (!cell.IsGiven)
                    cell.Value = null;
        }


        public bool IsComplete()
        {
            foreach (var cell in Cells)
            {
                if (!cell.Value.HasValue || cell.IsError)
                    return false;
            }
            return true;
        }

        public void ValidateAll(bool showErrors)
        {
            foreach (var cell in Cells)
                cell.IsError = false;

            for(int r = 0; r < 9; r++)
                for(int c = 0; c < 9; c++)
                {
                    var cell = GetCell(r, c);
                    if (!cell.Value.HasValue)
                        continue;
                    int val = cell.Value.Value;

                    for(int cc = 0; cc < 9; cc++)
                    {
                        if (cc == c)
                            continue;
                        var other = GetCell(r, cc);
                        if(other.Value == val)
                        {
                            cell.IsError = showErrors;
                            other.IsError = showErrors;
                        }
                    }

                    for (int rr = 0; rr < 9; rr++)
                    {
                        if (rr == r)
                            continue;
                        var other = GetCell(rr, c);
                        if (other.Value == val)
                        {
                            cell.IsError = showErrors;
                            other.IsError = showErrors;
                        }
                    }

                    int sr = (r / 3) * 3, sc = (c / 3) * 3;
                    for (int rr = sr; rr < sr + 3; rr++)
                        for (int cc = sc; cc < sc + 3; cc++)
                        {
                            if (rr == r && cc == c) continue;
                            var other = GetCell(rr, cc);
                            if (other.Value == val)
                            {
                                cell.IsError = showErrors;
                                other.IsError = showErrors;
                            }
                        }
                }
        }

    }
}
