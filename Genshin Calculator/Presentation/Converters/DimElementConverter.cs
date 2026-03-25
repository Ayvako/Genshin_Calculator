using System;
using System.Globalization;
using System.Windows.Data;

namespace Genshin_Calculator.Presentation.Converters;

public class DimElementConverter : IMultiValueConverter
{
    public double TrueOpacity { get; set; } = 0.4;

    public double FalseOpacity { get; set; } = 1.0;

    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        bool isChecked = (bool)values[0];
        int count = (int)values[1];

        if (count == 0)
        {
            return this.FalseOpacity;
        }

        return isChecked ? this.FalseOpacity : this.TrueOpacity;
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        => null!;
}