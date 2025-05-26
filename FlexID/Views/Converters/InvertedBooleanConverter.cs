namespace FlexID.VIews.Converters;

using System.Globalization;
using System.Windows.Data;

public class InvertedBooleanConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is bool v)
            return !v;

        throw new NotSupportedException();
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is bool v)
            return !v;

        throw new NotSupportedException();
    }
}
