using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows.Data;

namespace RedAlert.Converters
{
    public class BoolToStringConverter : IValueConverter
    {
        const string ENABLED = "Enabled";
        const string DISABLED = "Disabled";

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ((bool)value) ? ENABLED : DISABLED;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ((string)value).Equals(ENABLED);
        }
    }
}
