using System;
using System.Windows;
using System.Windows.Data;

using ESRI.ArcGIS.Client;

namespace ArcFMSilverlight.ValueConverters
{
    public class LayerFilterValueConverter : IValueConverter
    {

        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null) return Visibility.Collapsed;

            Layer layer = value as Layer;
            if (layer == null) return Visibility.Collapsed;

            if (layer is GraphicsLayer)
            {
                if ((layer is FeatureLayer) == false) return Visibility.Collapsed;
            }

            return Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
