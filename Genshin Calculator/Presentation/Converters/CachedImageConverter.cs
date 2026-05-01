using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace Genshin_Calculator.Presentation.Converters;

public class CachedImageConverter : IValueConverter
{
    private static readonly Dictionary<string, WeakReference<BitmapImage>> Cache = [];

    public object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        string? path = value.ToString();

        if (Cache.TryGetValue(path, out var weak) && weak.TryGetTarget(out var cached))
        {
            return cached;
        }

        var bitmap = new BitmapImage();
        bitmap.BeginInit();
        bitmap.UriSource = new Uri(path, UriKind.RelativeOrAbsolute);
        bitmap.CacheOption = BitmapCacheOption.OnLoad;
        bitmap.DecodePixelWidth = 58;
        bitmap.EndInit();
        bitmap.Freeze();

        Cache[path] = new WeakReference<BitmapImage>(bitmap);
        return bitmap;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => throw new NotImplementedException();
}