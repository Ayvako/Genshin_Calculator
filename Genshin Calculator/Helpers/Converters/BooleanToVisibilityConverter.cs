using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Genshin_Calculator.Helpers.Converters;

public class BooleanToVisibilityConverter : IValueConverter
{
    public bool IsInverted { get; set; }

    public bool UseHidden { get; set; }

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        bool boolValue = value is bool b && b;

        if (this.IsInverted)
        {
            boolValue = !boolValue;
        }

        if (boolValue)
        {
            return Visibility.Visible;
        }

        return this.UseHidden ? Visibility.Hidden : Visibility.Collapsed;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is Visibility visibility)
        {
            bool result = visibility == Visibility.Visible;
            return this.IsInverted ? !result : result;
        }

        return false;
    }
}