using System;
using System.Windows.Data;

using ESRI.ArcGIS.Client;
using ESRI.ArcGIS.Client.Bing;

namespace ArcFMSilverlight.ValueConverters
{
    public class CloneLayersValueConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null) return null;

            LayerCollection layers = value as LayerCollection;
            if (layers == null) return null;

            if (parameter == null) return CloneAllLayers(layers);

            int index = -1;
            if (int.TryParse(parameter.ToString(), out index))
            {
                return FirstValidLayer(layers, index);
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        private object CloneAllLayers(LayerCollection layers)
        {
            LayerCollection copy = new LayerCollection();

            foreach (Layer layer in layers)
            {
                Layer clone = CloneLayer(layer);
                if (clone != null)
                {
                    copy.Add(clone);
                }
            }
            return copy;
        }

        private object FirstValidLayer(LayerCollection layers, int index)
        {
            for (int i = index; i < layers.Count; i++)
            {
                Layer layer = CloneLayer(layers[i]);
                if (layer != null)
                {
                    return layer;
                }
            }
            return null;
        }

        private Layer CloneLayer(Layer layer)
        {
            if (layer is ArcGISDynamicMapServiceLayer)
            {
                ArcGISDynamicMapServiceLayer dynamic = (ArcGISDynamicMapServiceLayer)layer;
                return new ArcGISDynamicMapServiceLayer
                {
                    ID = dynamic.ID,
                    Url = dynamic.Url,
                    ProxyURL = dynamic.ProxyURL,
                    LayerDefinitions = dynamic.LayerDefinitions,
                    VisibleLayers = dynamic.VisibleLayers
                };
            }
            else if (layer is ArcGISTiledMapServiceLayer)
            {
                ArcGISTiledMapServiceLayer tiled = (ArcGISTiledMapServiceLayer)layer;
                return new ArcGISTiledMapServiceLayer
                {
                    ID = tiled.ID,
                    Url = tiled.Url,
                    ProxyURL = tiled.ProxyURL
                };
            }
            else if (layer is ArcGISImageServiceLayer)
            {
                ArcGISImageServiceLayer image = (ArcGISImageServiceLayer)layer;
                return new ArcGISImageServiceLayer
                {
                    ID = image.ID,
                    Url = image.Url,
                    ProxyURL = image.ProxyURL
                };
            }
            else if (layer is TileLayer)
            {
                var bingLayer = (TileLayer)layer;
                return new TileLayer
                {
                    LayerStyle = bingLayer.LayerStyle,
                    ServerType = bingLayer.ServerType,
                    Token = bingLayer.Token
                };
            }

            return null;
        }
    }
}
