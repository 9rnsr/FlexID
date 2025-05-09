using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace FlexID.Views.Converters
{
    [ValueConversion(typeof(MessageBoxButton), typeof(bool))]
    public class DefaultButtonConverter : IValueConverter
    {
        public object Parameter { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (targetType != typeof(bool))
                throw new InvalidOperationException("The target must be a boolean");

            return (MessageBoxButton)value == (MessageBoxButton)parameter;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
