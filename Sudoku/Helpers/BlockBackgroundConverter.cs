using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace Sudoku.Helpers
{
    public class BlockBackgroundConverter : IValueConverter
    {
        private static readonly Brush[] BlockColors = new Brush[]
        {
            new SolidColorBrush((Color)ColorConverter.ConvertFromString("#F4E5F7")), // svetlo ljubičasta
            new SolidColorBrush((Color)ColorConverter.ConvertFromString("#E5F0FA")), // svetlo plava
            new SolidColorBrush((Color)ColorConverter.ConvertFromString("#E5FAF0")), // svetlo zelena
            new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFF5E5")), // svetlo narandžasta
            new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FDE5F0")), // svetlo roze
            new SolidColorBrush((Color)ColorConverter.ConvertFromString("#E5FAF9")), // svetlo tirkizna
            new SolidColorBrush((Color)ColorConverter.ConvertFromString("#F5F5E5")), // svetlo žuta
            new SolidColorBrush((Color)ColorConverter.ConvertFromString("#EDE5FA")), // svetlo ljubičasta 2
            new SolidColorBrush((Color)ColorConverter.ConvertFromString("#F0FAE5"))  // svetlo zelena 2
        };

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int index)
            {
                int row = index / 9;
                int col = index % 9;

                int blockRow = row / 3;
                int blockCol = col / 3;
                int blockIndex = blockRow * 3 + blockCol;

                return BlockColors[blockIndex];
            }

            return Brushes.Transparent;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
