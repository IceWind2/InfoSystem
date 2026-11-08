using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace InfoSystem
{
    public class EnumToBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null && value.GetType().IsEnum)
            {
                return Equals(value, parameter);
            }
            else
            {
                return DependencyProperty.UnsetValue;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool boolValue && boolValue)
            {
                return parameter;
            }
            else
            {
                return DependencyProperty.UnsetValue;
            }
        }
    }
}
