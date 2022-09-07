using System;
using System.Globalization;
using System.Windows.Data;

using ESRI.ArcGIS.Client.Tasks;

namespace ArcFMSilverlight.ValueConverters
{
    public class LinearUnitEnumConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return 0;

            switch ((LinearUnit)value)
            {
                case LinearUnit.Foot:
                    return 0;
                case LinearUnit.InternationalYard:
                    return 1;
                case LinearUnit.Meter:
                    return 2;
                default:
                    return 0;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return LinearUnit.Foot;

            switch ((int)value)
            {
                case 0:
                    return LinearUnit.Foot;
                case 1:
                    return LinearUnit.InternationalYard;
                case 2:
                    return LinearUnit.Meter;
                default:
                    return LinearUnit.Foot;
            }
        }

        #endregion
    }
}
