using System;
using System.Globalization;
using System.Reflection;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace Genshin_Calculator.Helpers.Converters;

public class ImagePathToSourceConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is string relativePath && !string.IsNullOrEmpty(relativePath))
        {
            try
            {
                string assemblyName = Assembly.GetExecutingAssembly().GetName().Name!;
                string uriString = $"pack://application:,,,/{assemblyName};component/{relativePath}";
                BitmapImage img = new();
                img.BeginInit();
                img.UriSource = new Uri(uriString, UriKind.Absolute);
                img.CacheOption = BitmapCacheOption.OnLoad;
                img.EndInit();
                img.Freeze();
                return img;
            }
            catch
            {
                return null;
            }
        }

        return null;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => throw new NotImplementedException();
}