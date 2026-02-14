using Microsoft.UI.Xaml.Data;

namespace FlexID.Views.Converters;

public class ContourMaxMinConverter : IValueConverter
{
    public object? Convert(object value, Type targetType, object parameter, string language)
    {
        try
        {
            return string.Format("{0:0.00E+00}", (double)value);
        }
        catch
        {
            return null;
        }
    }

    public object? ConvertBack(object value, Type targetType, object parameter, string language)
    {
        if (value is string input)
        {
            try
            {
                return double.Parse(input);
            }
            catch
            {
                return null;
            }
        }
        return null;
    }
}
