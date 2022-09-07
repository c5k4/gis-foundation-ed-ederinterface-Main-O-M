using System;
using System.Globalization;
using System.Windows.Data;

namespace PageTemplates.Converters
{
    public class RoundDoubleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return string.Empty;
            if (!(value is double)) throw new ArgumentException("Value not a double", "value");


            int precision;
            try
            {
                precision = System.Convert.ToInt32(parameter);
            }
            catch (Exception)
            {
                throw new ArgumentException("Parameter is not an int", "parameter");
            }

            var v = (double)value;

            double newValue = Math.Round(v, precision);

            return newValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
