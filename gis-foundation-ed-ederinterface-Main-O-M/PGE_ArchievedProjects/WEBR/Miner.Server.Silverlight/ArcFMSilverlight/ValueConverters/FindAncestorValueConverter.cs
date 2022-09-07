using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace ArcFMSilverlight.ValueConverters
{
    public class FindAncestorValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return null;
            if (parameter == null) return null;

            DependencyObject d = value as DependencyObject;
            if (d == null) return null;

            string ancestor = parameter.ToString();

            while (d.GetType().FullName.Contains(ancestor) == false)
            {
                d = VisualTreeHelper.GetParent(d);
            }
            return d;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
