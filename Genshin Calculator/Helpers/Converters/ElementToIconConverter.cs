using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using Genshin_Calculator.Helpers.Enums;

namespace Genshin_Calculator.Helpers.Converters;

public class ElementToIconConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is Element element)
        {
            var iconPath = ResourcePaths.Element(element);

            return new BitmapImage(iconPath);
        }

        return null!;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}