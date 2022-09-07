using System.Collections.Generic;
using System.Linq;

using ESRI.ArcGIS.Client;
using ESRI.ArcGIS.Client.Geometry;
using ESRI.ArcGIS.Client.Tasks;

#if SILVERLIGHT
namespace Miner.Server.Client
#elif WPF
namespace Miner.Mobile.Client
#endif
{
    internal static class LayerExtensions
    {
        internal static LayerCollection Clone(this LayerCollection source)
        {
            LayerCollection layers = new LayerCollection();
            foreach (Layer layer in source)
            {
                layers.Add(layer.Clone());
            }
            return layers;
        }

        internal static Layer Clone(this Layer source)
        {
            Layer layer = null;
            if ((source is GraphicsLayer) && (!(source is FeatureLayer)))
            {
                layer = ((GraphicsLayer)source).Clone();
            }
            else if (source is ElementLayer)
            {
                // TODO: What do we do here?
            }
            else if (source is ArcGISTiledMapServiceLayer)
            {
                ArcGISTiledMapServiceLayer tiledLayer = (ArcGISTiledMapServiceLayer)source;
                layer = new ArcGISTiledMapServiceLayer
                {
                    Url = tiledLayer.Url,
                    ProxyURL = tiledLayer.ProxyURL
                };
            }
            else if (source is ArcGISDynamicMapServiceLayer)
            {
                ArcGISDynamicMapServiceLayer dynamicLayer = (ArcGISDynamicMapServiceLayer)source;
                layer = new ArcGISDynamicMapServiceLayer
                {
                    Url = dynamicLayer.Url,
                    ProxyURL = dynamicLayer.ProxyURL,
                    VisibleLayers = dynamicLayer.VisibleLayers,
                    LayerDefinitions = dynamicLayer.LayerDefinitions
                };
            }
            else if (source is ArcGISImageServiceLayer)
            {
                ArcGISImageServiceLayer imageLayer = (ArcGISImageServiceLayer)source;
                layer = new ArcGISImageServiceLayer
                {
                    Url = imageLayer.Url,
                    ProxyURL = imageLayer.ProxyURL,
                    ImageFormat=imageLayer.ImageFormat,
                    NoData = imageLayer.NoData
                };
            }
            else if (source is FeatureLayer)
            {
                FeatureLayer featureLayer = new FeatureLayer
                {
                    Url = ((FeatureLayer)source).Url,
                    DisableClientCaching = true,
                    AutoSave = false,
                };
                OutFields outFields = new OutFields();
                outFields.Add("*");
                featureLayer.OutFields = outFields;

                layer = featureLayer;
            }

            if (layer != null)
            {
                layer.ID = source.ID;
                layer.MaximumResolution = source.MaximumResolution;
                layer.MinimumResolution = source.MinimumResolution;
                layer.Opacity = source.Opacity;
                layer.Visible = source.Visible;
                layer.Effect = source.Effect;
            }
            return layer;
        }

        internal static GraphicsLayer Clone(this GraphicsLayer source)
        {
            GraphicsLayer layer = new GraphicsLayer();
            //layer.Clusterer = source.Clusterer;
            layer.ID = source.ID;
            foreach (Graphic graphic in source.Graphics)
            {
                Graphic g = graphic.Clone();
                layer.Graphics.Add(g);
            }
            return layer;
        }

        internal static EditGraphic Clone(this EditGraphic source)
        {
            if (source == null) return null;
            EditGraphic graphic = new EditGraphic(source);
            graphic.Geometry = Geometry.Clone(source.Geometry);
            graphic.Guid = source.Guid;
            return graphic;
        }

        internal static Graphic Clone(this Graphic source)
        {
            if (source == null) return null;

            Graphic graphic = new Graphic();
            graphic.Symbol = source.Symbol;
            graphic.Selected = source.Selected;
            graphic.Geometry = Geometry.Clone(source.Geometry);
            graphic.MapTip = source.MapTip;
            graphic.TimeExtent = source.TimeExtent;

            foreach (var attribute in source.Attributes)
            {
                graphic.Attributes.Add(attribute);
            }
            return graphic;
        }

        internal static string Url(this Layer source)
        {
            string url = null;
            ArcGISDynamicMapServiceLayer dynamic = source as ArcGISDynamicMapServiceLayer;
            if (dynamic != null)
            {
                url = dynamic.Url;
            }
            else
            {
                ArcGISTiledMapServiceLayer tiled = source as ArcGISTiledMapServiceLayer;
                if (tiled != null)
                {
                    url = tiled.Url;
                }
                else
                {
                    ArcGISImageServiceLayer image = source as ArcGISImageServiceLayer;
                    if (image != null)
                    {
                        url = image.Url;
                    }
                    else
                    {
                        FeatureLayer featureLayer = source as FeatureLayer;
                        if (featureLayer != null)
                        {
                            url = featureLayer.Url;
                        }
                    }
                }
            }
            return url;

        }

        internal static string ProxyUrl(this Layer source)
        {
            string proxyUrl = null;
            ArcGISDynamicMapServiceLayer dynamic = source as ArcGISDynamicMapServiceLayer;
            if (dynamic != null)
            {
                proxyUrl = dynamic.ProxyURL;
            }
            else
            {
                ArcGISTiledMapServiceLayer tiled = source as ArcGISTiledMapServiceLayer;
                if (tiled != null)
                {
                    proxyUrl = tiled.ProxyURL;
                }
                else
                {
                    ArcGISImageServiceLayer image = source as ArcGISImageServiceLayer;
                    if (image != null)
                    {
                        proxyUrl = image.ProxyURL;
                    }
                    else
                    {
                        FeatureLayer featureLayer = source as FeatureLayer;
                        if (featureLayer != null)
                        {
                            proxyUrl = featureLayer.ProxyUrl;
                        }
                    }
                }
            }
            return proxyUrl;
        }

        internal static IEnumerable<LayerDefinition> LayerDefinitions(this Layer source)
        {
            if (source == null) return null;
            ArcGISDynamicMapServiceLayer dynamicLayer = source as ArcGISDynamicMapServiceLayer;
            if (dynamicLayer == null) return null;

            return dynamicLayer.LayerDefinitions;
        }

        internal static Layer LayerFromUrl(this Map source, string url)
        {
            if (source == null) return null;
            if (string.IsNullOrEmpty(url)) return null;

            return source.Layers.FirstOrDefault(layer => layer.Url() == url);
        }

        internal static string UrlFromID(this Map source, string id)
        {
            if (source == null) return null;
            if (string.IsNullOrEmpty(id)) return null;

            Layer selectedLayer = (from layer in source.Layers
                                  where (layer.ID != null) && (layer.ID.ToUpper() == id.ToUpper())
                                  select layer as Layer).FirstOrDefault();
            if (selectedLayer == null) return null;

            return selectedLayer.Url();
        }

        internal static EditGraphic ReplaceGraphic(this GraphicsLayer source, Graphic graphic)
        {
            if (source == null) return null;

            EditGraphic editGraphic = graphic as EditGraphic;
            if ((editGraphic == null) && (graphic != null))
            {
                editGraphic = new EditGraphic(graphic);
                // This nulls the layer in event args. You can't add actual text to the text item.
                // Removing these lines seems to allow all operations.  I am not sure if this needs to change.
                //source.Graphics.Remove(graphic);
                //source.Graphics.Add(editGraphic);
            }
            return editGraphic;
        }
    }
}
