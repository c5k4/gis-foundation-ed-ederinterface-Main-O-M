using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace ArcFMSilverlight.Behaviors
{
    public class DoubleToVisibilityConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double sourceValue = 0d;
            try
            {
                sourceValue = System.Convert.ToDouble(value);
            }
            catch
            {
            }
            double targetValue = 0d;
            try
            {
                targetValue = System.Convert.ToDouble(parameter);
            }
            catch
            {
            }
            return sourceValue > targetValue ? Visibility.Collapsed : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}