using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using Genshin_Calculator.Helpers;
using Genshin_Calculator.Models.Enums;

namespace Genshin_Calculator.Presentation.Converters;

public class WeaponToIconConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is WeaponType element)
        {
            var iconPath = ResourcePaths.Weapon(element);

            return new BitmapImage(iconPath);
        }

        return null!;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}