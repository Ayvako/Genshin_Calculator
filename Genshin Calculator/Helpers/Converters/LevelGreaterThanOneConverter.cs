using System;
using System.Globalization;
using System.Windows.Data;

namespace Genshin_Calculator.Helpers.Converters;

public class LevelGreaterThanOneConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        int numeric = ParseLevel(value);
        return numeric > 1;
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
        if (s.EndsWith('★'))
        {
            s = s.TrimEnd('★');
        }

        if (int.TryParse(s, out int num))
        {
            return num;
        }

        return 0;
    }
}