using System;
using System.Globalization;
using System.Windows.Data;
using ESRI.ArcGIS.Client.Geometry;

namespace PageTemplates.Converters
{
    public class ExtentCenterPointValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return string.Empty;
            if (!(value is Envelope)) throw new ArgumentException("Value not an envelope", "value");

            var t2 = (Envelope)value;

            MapPoint center = t2.GetCenter();

            string centerCoordinates = "Center: " + Math.Round(center.X, 2) + ", " + Math.Round(center.Y, 2);

            return centerCoordinates;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
