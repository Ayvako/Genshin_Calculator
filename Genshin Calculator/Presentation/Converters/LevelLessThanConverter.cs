using System;
using System.Globalization;
using System.Windows.Data;

namespace Genshin_Calculator.Presentation.Converters;

public class LevelLessThanConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        int numeric = ParseLevel(value);

        int max = System.Convert.ToInt32(parameter);
        return numeric < max;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => throw new NotImplementedException();

    private static int ParseLevel(object value)
    {
        if (value == null)
        {
            return 0;
        }

        string s = value.ToString()!;
        if (s.EndsWith('+'))
        {
            s = s.TrimEnd('+');
        }

        if (int.TryParse(s, out int num))
        {
            return num;
        }

        return 0;
    }
}