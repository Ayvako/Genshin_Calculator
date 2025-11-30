using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Genshin_Calculator.Helpers.Converters
{
    class LevelComparisonConverter : IValueConverter
    {
        // parameter format: "operator|maxValue"
        // examples:
        // ">1"
        // "<10"
        // "<90"
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || parameter == null)
                return false;

            if (!int.TryParse(value.ToString(), out int number))
                return false;

            string param = parameter.ToString().Trim();

            // Operator
            char op = param[0];

            // numeric threshold
            if (!int.TryParse(param.Substring(1), out int threshold))
                return false;

            return op switch
            {
                '>' => number > threshold,
                '<' => number < threshold,
                '=' => number == threshold,
                _ => false,
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}