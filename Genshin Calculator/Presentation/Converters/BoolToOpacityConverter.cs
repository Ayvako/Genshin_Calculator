using System;

using System.Globalization;
using System.Windows.Data;

namespace Genshin_Calculator.Presentation.Converters
{
    public class BoolToOpacityConverter : IValueConverter
    {
        public double TrueOpacity { get; set; } = 0.4;

        public double FalseOpacity { get; set; } = 1.0;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool isCollected)
            {
                return isCollected ? this.TrueOpacity : this.FalseOpacity;
            }

            return this.FalseOpacity;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}