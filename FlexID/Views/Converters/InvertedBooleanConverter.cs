using System;
using System.Globalization;
using System.Windows.Data;

namespace FlexID.VIews.Converters
{
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
}
