using System;

using System.Globalization;
using System.Windows.Data;

namespace Genshin_Calculator.Presentation.Converters;

public class IconProxyConverter : IMultiValueConverter
{
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        if (values.Length != 2)
        {
            return null!;
        }

        var item = values[0];
        if (values[1] is not IValueConverter converter)
        {
            return null!;
        }

        return converter.Convert(item, targetType, parameter, culture);
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}