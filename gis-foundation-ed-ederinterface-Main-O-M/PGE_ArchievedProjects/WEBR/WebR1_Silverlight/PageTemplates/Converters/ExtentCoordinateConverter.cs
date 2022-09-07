using System;
using System.Globalization;
using System.Windows.Data;
using ESRI.ArcGIS.Client.Geometry;

namespace PageTemplates.Converters
{
    public class ExtentCoordinateConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return string.Empty;
            if (!(value is Envelope)) throw new ArgumentException("Value not an extent", "value");

            const int precision = 2;
            CoordinateType coordType;

            try
            {
                coordType = (CoordinateType)Enum.Parse(typeof(CoordinateType), parameter.ToString(), true);
            }
            catch (Exception)
            {
                throw new ArgumentException("Parameter is not an CoordinateType", "parameter");
            }

            var v = (Envelope)value;
            string label;

            switch (coordType)
            {
                case CoordinateType.LowerLeft:
                    label = Math.Round(v.XMin, precision) + "," + Math.Round(v.YMin, precision);
                    break;
                case CoordinateType.LowerRight:
                    label = Math.Round(v.XMax, precision) + "," + Math.Round(v.YMin, precision);
                    break;
                case CoordinateType.UpperLeft:
                    label = Math.Round(v.XMin, precision) + "," + Math.Round(v.YMax, precision);
                    break;
                case CoordinateType.UpperRight:
                    label = Math.Round(v.XMax, precision) + "," + Math.Round(v.YMax, precision);
                    break;
                case CoordinateType.Center:
                    MapPoint center = v.GetCenter();
                    label = "Center: " + Math.Round(center.X, precision) + ", " + Math.Round(center.Y, precision);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return label;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public enum CoordinateType
        {
            LowerLeft,
            LowerRight,
            UpperLeft,
            UpperRight,
            Center
        }
    }
}