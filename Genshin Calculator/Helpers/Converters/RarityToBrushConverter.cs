using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace Genshin_Calculator.Helpers.Converters;

public class RarityToBrushConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        int rarity = (int)value;
        return SetBackgroundRarity(rarity);
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }

    private static LinearGradientBrush SetBackgroundRarity(int rarity)
    {
        var gradient = new LinearGradientBrush
        {
            StartPoint = new Point(0, 0),
            EndPoint = new Point(1, 1),
            GradientStops = rarity switch
            {
                4 =>
                [
                    new GradientStop(Color.FromArgb(144, 105, 84, 83), 0),
                    new GradientStop(Color.FromArgb(144, 161, 112, 78), 0.39),
                    new GradientStop(Color.FromArgb(144, 228, 171, 82), 1)
                ],
                3 =>
                [
                    new GradientStop(Color.FromArgb(144, 89, 84, 130), 0),
                    new GradientStop(Color.FromArgb(144, 120, 102, 157), 0.39),
                    new GradientStop(Color.FromArgb(144, 183, 133, 201), 1)
                ],
                2 =>
                [
                    new GradientStop(Color.FromArgb(144, 81, 84, 116), 0),
                    new GradientStop(Color.FromArgb(144, 80, 104, 135), 0.39),
                    new GradientStop(Color.FromArgb(144, 75, 160, 180), 1),
                ],
                1 =>
                [
                    new GradientStop(Color.FromArgb(144, 72, 87, 92), 0),
                    new GradientStop(Color.FromArgb(144, 72, 107, 103), 0.39),
                    new GradientStop(Color.FromArgb(144, 98, 152, 113), 1),
                ],
                _ =>
                [
                    new GradientStop(Color.FromArgb(144, 79, 88, 100), 0),
                    new GradientStop(Color.FromArgb(144, 95, 102, 115), 0.39),
                    new GradientStop(Color.FromArgb(144, 135, 147, 156), 1),
                ],
            },
        };
        return gradient;
    }
}