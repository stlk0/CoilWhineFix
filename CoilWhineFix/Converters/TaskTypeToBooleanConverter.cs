using System.Globalization;
using System.Windows.Data;
using CoilWhineFix.Views;

namespace CoilWhineFix.converters
{
    public class TaskTypeToBooleanConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value == null || parameter == null)
                return false;

            return (TaskType)value == (TaskType)parameter;
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            return value is true ? parameter ?? Binding.DoNothing : Binding.DoNothing;
        }
    }
}