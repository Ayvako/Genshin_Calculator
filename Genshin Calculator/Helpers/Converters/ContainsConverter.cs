using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows.Data;

namespace Genshin_Calculator.Helpers.Converters;

public class ContainsConverter : IMultiValueConverter
{
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        if (values[0] is ObservableCollection<string> collection && values[1] is string item)
        {
            return collection.Contains(item);
        }

        return false;
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}