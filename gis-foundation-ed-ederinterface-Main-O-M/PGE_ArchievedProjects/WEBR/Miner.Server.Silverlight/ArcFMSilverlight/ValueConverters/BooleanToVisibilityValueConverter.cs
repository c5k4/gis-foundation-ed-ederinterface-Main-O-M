using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows;

namespace ArcFMSilverlight.ValueConverters
{
    public class BooleanToVisibilityValueConverter : IValueConverter
    {

        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return Visibility.Collapsed;

            bool isVis = false;
            bool.TryParse(value.ToString(), out isVis);

            return isVis ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
