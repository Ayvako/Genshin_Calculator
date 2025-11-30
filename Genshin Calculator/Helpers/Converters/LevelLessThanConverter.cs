using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Genshin_Calculator.Helpers.Converters
{
    class LevelLessThanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int numeric = this.ParseLevel(value);

            int max = System.Convert.ToInt32(parameter);
            return numeric < max;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();

        private int ParseLevel(object value)
        {
            if (value == null)
                return 0;

            string s = value.ToString()!;
            if (s.EndsWith("+"))
                s = s.TrimEnd('+');

            if (int.TryParse(s, out int num))
                return num;

            return 0;
        }

    }
}