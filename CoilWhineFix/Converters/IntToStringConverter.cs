using System.Globalization;
using System.Windows.Data;

namespace CoilWhineFix.converters
{
    public class IntToStringConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            var intValue = value as int? ?? 1;
            return intValue.ToString(culture);
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value == null) return 1;
            return int.TryParse((string)value, out var result) ? result : 1;
        }
    }
}