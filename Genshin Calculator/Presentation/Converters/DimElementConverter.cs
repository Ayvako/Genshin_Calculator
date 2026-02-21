using System;
using System.Globalization;
using System.Windows.Data;

namespace Genshin_Calculator.Presentation.Converters;

public class DimElementConverter : IMultiValueConverter
{
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        bool isChecked = (bool)values[0];
        int count = (int)values[1];

        if (count == 0)
        {
            return 1.0;
        }

        return isChecked ? 1.0 : 0.35;
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        => null!;
}