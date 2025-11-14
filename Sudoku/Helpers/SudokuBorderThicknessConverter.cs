using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using Sudoku.Models;

namespace Sudoku.Helpers
{
    public class SudokuBorderThicknessConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Cell cell)
            {
                int row = cell.Row;
                int col = cell.Col;

                double thin = 1;    
                double thick = 2;   

                double left = (col % 3 == 0) ? thick : thin;
                double top = (row % 3 == 0) ? thick : thin;
                double right = ((col + 1) % 3 == 0) ? thick : thin;
                double bottom = ((row + 1) % 3 == 0) ? thick : thin;

                return new Thickness(left, top, right, bottom);
            }
            return new Thickness(1);
        }


        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}
