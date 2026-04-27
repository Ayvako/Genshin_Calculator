using System;
using System.Globalization;
using System.Windows.Data;

namespace Genshin_Calculator.Presentation.Converters;

/// <summary>
/// Возвращает true, если числовое значение строго больше порога из ConverterParameter.
/// Принимает int, double и строки вида "20★" (суффикс ★ обрезается).
/// Использование: Converter={StaticResource GreaterThanConverter}, ConverterParameter=1
/// Заменяет: GreaterThanZeroConverter (параметр 0) и LevelGreaterThanOneConverter (параметр 1).
/// </summary>
public class GreaterThanConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        double threshold = ParseDouble(parameter);
        double numeric = ParseDouble(value);
        return numeric > threshold;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => throw new NotImplementedException();

    private static double ParseDouble(object? value)
    {
        if (value is null)
        {
            return 0;
        }

        if (value is int i)
        {
            return i;
        }

        if (value is double d)
        {
            return d;
        }

        if (value is float f)
        {
            return f;
        }

        string s = value.ToString()!.TrimEnd('★').Trim();

        return double.TryParse(s, NumberStyles.Any, CultureInfo.InvariantCulture, out double result)
            ? result
            : 0;
    }
}