using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;

using ESRI.ArcGIS.Client;
using ESRI.ArcGIS.Client.Geometry;

#if SILVERLIGHT
using Miner.Server.Client.Tasks;
#elif WPF
using Miner.Mobile.Client.Tasks;
#endif

#if SILVERLIGHT
namespace Miner.Server.Client
#elif WPF
namespace Miner.Mobile.Client
#endif
{
    internal class GraphicsLayerEventArgs : EventArgs
    {
        private readonly GraphicsLayer _layer;

        public GraphicsLayerEventArgs(GraphicsLayer layer)
        {
            _layer = layer;
        }

        public GraphicsLayer Layer
        {
            get { return _layer; }
        }
    }

    internal static class GraphicsLayers
    {
        private static string Graphics = "GRAPHICS";

        private static readonly Dictionary<string, GraphicsLayerInformation> CachedGraphicsLayers =
            new Dictionary<string, GraphicsLayerInformation>();

        private static readonly List<SolidColorBrush> ColorRamp =
            new List<SolidColorBrush>
                {
                    new SolidColorBrush(Color.FromArgb(255, 168, 0, 0)),
                    new SolidColorBrush(Color.FromArgb(255, 199, 130, 40)),
                    new SolidColorBrush(Color.FromArgb(255, 105, 100, 46)),
                    new SolidColorBrush(Color.FromArgb(255, 84, 168, 0)),
                    new SolidColorBrush(Color.FromArgb(255, 47, 92, 0)),
                    new SolidColorBrush(Color.FromArgb(255, 0, 161, 120)),
                    new SolidColorBrush(Color.FromArgb(255, 0, 90, 138)),
                    new SolidColorBrush(Color.FromArgb(255, 44, 0, 156)),
                    new SolidColorBrush(Color.FromArgb(255, 113, 0, 189)),
                    new SolidColorBrush(Color.FromArgb(255, 194, 0, 149))
                };

        private static int _currentColorIndex;
        private static readonly object Padlock = new object();

        public static event EventHandler<GraphicsLayerEventArgs> LayerCreated;

        private static void OnLayerCreated(GraphicsLayerEventArgs e)
        {
            EventHandler<GraphicsLayerEventArgs> handler = LayerCreated;
            if (handler != null)
            {
                handler(null, e);
            }
        }

        public static GraphicsLayer LoadLayer(Map map, IResultSet resultSet)
        {
            if (map == null || resultSet == null) return null;
            return LoadLayer(map, resultSet.Name, resultSet.DisplayFieldName);
        }

        public static GraphicsLayer FindLayer(ILayerName layerName)
        {
            return layerName == null ? null : FindLayer(layerName.Name);
        }

        private static GraphicsLayer LoadLayer(Map map, string key, string displayFieldName)
        {
            if (map == null || string.IsNullOrEmpty(key)) return null;
            GraphicsLayerInformation layerInformation;
            key = key.ToLowerInvariant();
            lock (Padlock)
            {
                if (!CachedGraphicsLayers.TryGetValue(key, out layerInformation))
                {
                    layerInformation = Create(map, key, displayFieldName);
                    CachedGraphicsLayers.Add(key, layerInformation);
                }
            }
            return layerInformation.GraphicsLayer;
        }

        private static GraphicsLayer FindLayer(string key)
        {
            GraphicsLayerInformation layerInformation;
            key = key.ToLowerInvariant();
            lock (Padlock)
            {
                if (!CachedGraphicsLayers.TryGetValue(key, out layerInformation))
                {
                    return null;
                }
            }
            return layerInformation.GraphicsLayer;
        }

        public static Brush LayerBrush(IResultSet resultSet)
        {
            return LayerBrush(resultSet.Name);
        }

        public static Brush LayerBrush(ILayerName layerName)
        {
            return LayerBrush(layerName.Name);
        }

        public static string DisplayFieldName(ILayerName layerName)
        {
            return DisplayFieldName(layerName.Name);
        }

        public static string DisplayFieldName(GraphicsLayer graphicsLayer)
        {
            GraphicsLayerInformation layerInfo = FindLayerInformation(graphicsLayer);
            return layerInfo == null ? null : layerInfo.LayerDisplayFieldName;
        }

        private static Brush LayerBrush(string key)
        {
            GraphicsLayerInformation layerInfo = FindLayerInformation(key);
            return layerInfo == null ? null : layerInfo.ColorBrush;
        }

        private static string DisplayFieldName(string key)
        {
            GraphicsLayerInformation layerInfo = FindLayerInformation(key);
            return layerInfo == null ? null : layerInfo.LayerDisplayFieldName;
        }

        private static GraphicsLayerInformation FindLayerInformation(string key)
        {
            if (string.IsNullOrEmpty(key)) return null;
            GraphicsLayerInformation layerInformation;
            key = key.ToLowerInvariant();
            return !CachedGraphicsLayers.TryGetValue(key, out layerInformation)
                       ? null
                       : layerInformation;
        }

        public static Brush LayerBrush(GraphicsLayer graphicsLayer)
        {
            GraphicsLayerInformation layerInfo = FindLayerInformation(graphicsLayer);
            return layerInfo == null ? null : layerInfo.ColorBrush;
        }

        private static GraphicsLayerInformation FindLayerInformation(GraphicsLayer graphicsLayer)
        {
            if (graphicsLayer == null) return null;
            return (from layerInfo in CachedGraphicsLayers.Values
                    where ReferenceEquals(layerInfo.GraphicsLayer, graphicsLayer)
                    select layerInfo).FirstOrDefault();
        }

        public static Envelope FullEnvelope()
        {
            return AllLayers()
                .Where(l => l.Graphics.Count() > 0)
                .Aggregate<GraphicsLayer, Envelope>(
                    null,
                    (current, layer) =>
                    current == null
                        ? layer.FullExtent
                        : layer.FullExtent.Union(current));
        }

        public static int TotalCount()
        {
            return AllLayers().Sum(layer => layer.Graphics.Count);
        }

        public static void ClearAll()
        {
            foreach (GraphicsLayer layer in AllLayers())
            {
                layer.ClearGraphics();
            }
        }

        public static IEnumerable<Graphic> AllGraphics()
        {
            return AllLayers().SelectMany(layer => layer.Graphics);
        }

        public static void Refresh()
        {
            foreach (GraphicsLayer layer in AllLayers())
            {
                layer.Refresh();
            }
        }

        internal static IEnumerable<GraphicsLayer> AllLayers()
        {
            return (from layerInfo in CachedGraphicsLayers.Values
                    select layerInfo.GraphicsLayer).ToArray();
        }

        private static GraphicsLayerInformation Create(Map map, string key, string displayFieldName)
        {
            var layer = new GraphicsLayer { ID = key + Graphics };
            layer.MouseLeftButtonDown += GraphicsLayerClick;
            map.Layers.Add(layer);

            if (MarkerManager.GlobalGroupRadius > 0)
            {
                layer.Clusterer = new SelectionClusterer(key);
            }

            OnLayerCreated(new GraphicsLayerEventArgs(layer));
            SolidColorBrush colorBrush = ColorRamp[_currentColorIndex++];
            if (_currentColorIndex == ColorRamp.Count)
            {
                _currentColorIndex = 0;
            }
            return new GraphicsLayerInformation(layer, colorBrush, displayFieldName);
        }

        private static void GraphicsLayerClick(object sender, GraphicMouseButtonEventArgs e)
        {
            if (e.Graphic.Selected)
            {
                e.Graphic.UnSelect();
            }
            else
            {
                e.Graphic.Select();
            }
        }

        #region Nested type: GraphicsLayerInformation

        private class GraphicsLayerInformation
        {
            private readonly SolidColorBrush _colorBrush;
            private readonly string _displayFieldName;
            private readonly GraphicsLayer _graphicsLayer;

            public GraphicsLayerInformation(GraphicsLayer graphicsLayer, SolidColorBrush colorBrush,
                                            string displayFieldName)
            {
                _graphicsLayer = graphicsLayer;
                _colorBrush = colorBrush;
                _displayFieldName = displayFieldName;
            }

            public GraphicsLayer GraphicsLayer
            {
                get { return _graphicsLayer; }
            }

            public SolidColorBrush ColorBrush
            {
                get { return _colorBrush; }
            }

            public string LayerDisplayFieldName
            {
                get { return _displayFieldName; }
            }
        }

        #endregion
    }
}