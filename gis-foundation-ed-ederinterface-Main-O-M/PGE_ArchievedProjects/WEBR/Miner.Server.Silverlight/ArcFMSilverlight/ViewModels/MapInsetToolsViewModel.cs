using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Json;
using System.Linq;
using System.Net;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;

using ESRI.ArcGIS.Client;
using ESRI.ArcGIS.Client.Bing;
using ESRI.ArcGIS.Client.Toolkit.DataSources;
using ESRI.ArcGIS.Client.Toolkit.Primitives;
using ESRI.ArcGIS.Client.Geometry;
using ESRI.ArcGIS.Client.Symbols;

using Miner.Server.Client;
using Miner.Server.Client.Query;
using Miner.Server.Client.Symbols;
using Miner.Server.Client.Toolkit;

namespace ArcFMSilverlight.ViewModels
{
    public class MapInsetToolsViewModel : BasicMapToolsViewModel
    {
        #region Member Variables

        private Map _map;
        private Map _insetMap;
        private LayerVisibility _insetTOC;
        private HashSet<string> _featureLayers;
        private bool _identifyChecked;
        private bool _visible;

        private double _resolution = 1;
        private double _unscaledMarkerSize = 10;

        private const double _minimumResolution = 0.0000001;

        private const string InsetMapMarker = "InsetMapMarker";
        private const string TextLayerID = "TextTemporary";
        private const string IdentifyCursor = @"/Miner.Server.Client.Toolkit;component/Images/cursor_identify.png";


        #endregion Member Variables

        public MapInsetToolsViewModel()
            : base()
        {
            IdentifyCommand = new DelegateCommand(Identify);

            _featureLayers = new HashSet<string>();
        }

        #region public events

        public event EventHandler<ResultEventArgs> RetrievedResults;
        public event EventHandler<CancelEventArgs> RetrievingResults;

        #endregion

        #region Public Properties

        public ICommand IdentifyCommand { get; set; }

        public LayerVisibility InsetTOC
        {
            get { return _insetTOC; }
            set
            {
                _insetTOC = value;
            }
        }

        public Map InsetMap
        {
            get { return _insetMap; }
            set
            {
                _insetMap = value;
                InitZoomBox(_insetMap);
                AddLayersToInset(_map, _insetMap);
                _insetMap.ExtentChanged += InsetMap_ExtentChanged;
                OnPropertyChanged("InsetMap");
            }
        }

        public Map Map
        {
            get { return _map; }
            set
            {
                UnsubscribeMapEvents(_map);
                RemoveInsetMarker(_map);

                _map = value;
                SubscribeMapEvents(_map);
                AddInsetMarker(_map);
                AddLayersToInset(_map, _insetMap);
                OnPropertyChanged("Map");
            }
        }

        public bool IdentifyChecked
        {
            get { return _identifyChecked; }
            set
            {
                _identifyChecked = value;
                OnPropertyChanged("IdentifyChecked");
            }
        }

        public bool Visible
        {
            get { return _visible; }
            set
            {
                _visible = value;
                Layer layer = Map.Layers[InsetMapMarker];
                if (layer != null)
                {
                    layer.Visible = value;
                }
                OnPropertyChanged("Visible");
            }
        }

        public double Resolution
        {
            get { return _resolution; }
            set
            {
                _resolution = value;
            }
        }

        public double UnscaledMarkerSize
        {
            get { return _unscaledMarkerSize; }
            set
            {
                _unscaledMarkerSize = value;
            }
        }

        public bool UseClientSideVisibility { get; set; }

        //DA #190905 - ME Q1 2020 -- START
        public string LIDARCorStoredDisplay { get; set; }
        public int LIDARCorLayerID { get; set; }
        public event EventHandler<IsVisibleEventArgs> LIDARCorVisiblilityChangedEvnt;
        //DA #190905 - ME Q1 2020 -- END

        #endregion Public Properties

        #region Protected Methods

        protected virtual void OnRetrievingResults(CancelEventArgs args)
        {
            EventHandler<CancelEventArgs> handler = RetrievingResults;
            if (handler != null)
            {
                handler(this, args);
            }
        }

        protected virtual void OnRetrievedResults(ResultEventArgs args)
        {
            EventHandler<ResultEventArgs> handler = RetrievedResults;
            if (handler != null)
            {
                handler(this, args);
            }
        }

        #endregion Protected Methods

        #region Public Methods

        public void UpdateExtent(Envelope extent)
        {
            if (InsetMap.Extent == null)
            {
                InsetMap.Extent = _map.Extent;
            }
            InsetMap.ZoomTo(extent);
        }

        public void RemoveInsetMarker(Map map)
        {
            if (map == null) return;

            Layer layer = map.Layers[InsetMapMarker];
            if (layer == null) return;

            map.Layers.Remove(layer);
        }

        #endregion Public Methods

        #region Event Handlers

        protected override void ZoomBox_DrawComplete(object sender, DrawEventArgs e)
        {
            if (e == null) return;

            Envelope envelope = e.Geometry as Envelope;
            if (envelope == null) return;

            if (IdentifyChecked)
            {
                IEnumerable<Layer> layers = from layer in InsetMap.Layers
                                            where !(layer is ArcGISImageServiceLayer) &&
                                            !(layer is GraphicsLayer) && layer.Visible
                                            select layer;

                if (layers.Any())
                {
                    VisibleLayerIdentify id = new VisibleLayerIdentify();
                    id.UseClientLayerVisibility = UseClientSideVisibility;
                    id.IdentifyComplete += new EventHandler<ResultEventArgs>(Identify_IdentifyComplete);
                    id.IdentifyAsync(InsetMap, envelope);
                    OnRetrievingResults(new CancelEventArgs());
                }
            }
            else
            {
                base.ZoomBox_DrawComplete(sender, e);
            }
        }

        void InsetMap_ExtentChanged(object sender, ExtentEventArgs e)
        {
            GraphicsLayer layer = Map.Layers[InsetMapMarker] as GraphicsLayer;
            if (layer == null)
            {
                layer = CreateInsetMarker();
                Map.Layers.Add(layer);
            }
            layer.Visible = Visible;
            layer.Graphics[0].Geometry = e.NewExtent;

            InsetMap.MinimumResolution = _minimumResolution;

            var factor = 1 / InsetMap.Resolution;
            if (factor != 1)
            {
                var markerSize = factor * _unscaledMarkerSize;
                foreach (var graphicLayer in InsetMap.Layers.Where(l => l is GraphicsLayer))
                {
                    foreach (var graphic in (graphicLayer as GraphicsLayer).Graphics.Where(f => f.Symbol is CustomMarkerSymbol))
                    {
                        (graphic.Symbol as CustomMarkerSymbol).Size = markerSize;
                    }
                }
            }
        }

        void Identify_IdentifyComplete(object sender, ResultEventArgs e)
        {
            // Publish ResultsRetrieved event
            OnRetrievedResults(e);
        }

        void Layers_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (Layer layer in e.NewItems)
                {
                    if (layer is FeatureLayer) continue;
                    if (layer.ID == InsetMapMarker) continue;

                    if (_featureLayers.Contains(layer.ID)) continue;

                    SubscribeLayerEvents(layer);
                    AddLayerToInset(layer);
                }
            }
            if (e.OldItems != null)
            {
                foreach (Layer layer in e.OldItems)
                {
                    UnsubscribeLayerEvents(layer);

                    RemoveLayerFromInset(layer);
                }
            }
        }

        void Graphics_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            GraphicsLayer graphicsLayer = Map.Layers.FirstOrDefault(layer => (layer is GraphicsLayer) &&
                (layer as GraphicsLayer).Graphics == sender) as GraphicsLayer;
            if (graphicsLayer == null) return;
            if (string.IsNullOrEmpty(graphicsLayer.ID)) return;

            GraphicsLayer insetGraphicsLayer = InsetMap.Layers[graphicsLayer.ID] as GraphicsLayer;
            if (insetGraphicsLayer != null)
            {
                switch (e.Action)
                {
                    case NotifyCollectionChangedAction.Add:
                        foreach (Graphic graphic in e.NewItems)
                        {
                            insetGraphicsLayer.Graphics.Add(GraphicClone(graphic));
                        }
                        break;
                    case NotifyCollectionChangedAction.Remove:
                        for (int i = 0; i < e.OldItems.Count; i++)
                        {
                            insetGraphicsLayer.Graphics.RemoveAt(e.OldStartingIndex + i);
                        }
                        break;
                    case NotifyCollectionChangedAction.Reset:
                        insetGraphicsLayer.Graphics.Clear();
                        break;
                }
            }
        }

        void Layer_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Visible")
            {
                Layer layer = sender as Layer;
                Layer insetLayer = InsetMap.Layers[layer.ID];
                if (insetLayer != null)
                {
                    insetLayer.Visible = layer.Visible;
                }
            }
        }

        void LayerVisibilityTree_LayerVisibilityChanged(object sender, IsVisibleEventArgs e)
        {
            //DA #190905 - ME Q1 2020 -- START
            if (sender != null)
            {
                var layerItem = sender as LayerItem;
                if (layerItem != null)
                {
                    if (layerItem.Layer.ID == LIDARCorStoredDisplay && layerItem.SubLayerID == LIDARCorLayerID)
                    {
                        if (LIDARCorVisiblilityChangedEvnt != null)
                        {
                            this.LIDARCorVisiblilityChangedEvnt(new object(), e);
                        }
                    }
                }
            }
            //DA #190905 - ME Q1 2020 -- END

            if (!this.Visible) return;

            if (e.IsEnabledChanged && e.IsVisible)
            {
                var layerItem = sender as LayerItem;
                if (layerItem.Layer.ID == layerItem.Label) return;

                if (InsetTOC.TOC != null)
                {
                    foreach (var childLayerItem in InsetTOC.TOC.LayerItems)
                    {
                        if (layerItem.Layer.ID == childLayerItem.Layer.ID)
                        {
                            Refresh(childLayerItem, layerItem.Label, e.IsVisible);
                            break;
                        }
                    }
                }
            }
        }

        void WebClient_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            if (e.Error != null) return;

            var json = JsonObject.Parse(e.Result);
            if (json.ContainsKey("name"))
            {
                var name = json["name"].ToString();

                if (name.StartsWith("\""))
                {
                    name = name.Substring(1);
                }

                if (name.EndsWith("\""))
                {
                    name = name.Substring(0, name.Length - 1);
                }

                _featureLayers.Add(name.ToLowerInvariant() + "GRAPHICS");
            }
        }

        void Layer_Initialized(object sender, EventArgs e)
        {
            if (sender is WmsLayer)
            {
                var layer = sender as WmsLayer;
                var layers = new List<string>();
                foreach (var layerInfo in layer.LayerList)
                {
                    GetWMSLayers(layerInfo, layers);
                }

                layer.Layers = layers.ToArray();
            }
        }

        #endregion Event Handlers

        #region Private Methods

        private void Refresh(LayerItemViewModel layerItem, string sublayer, bool isVisible)
        {
            if (layerItem.LayerItems != null)
            {
                foreach (var childLayerItem in layerItem.LayerItems)
                {
                    if (childLayerItem.Label == sublayer)
                    {
                        if (childLayerItem.IsEnabled != isVisible)
                        {
                            childLayerItem.IsEnabled = isVisible;
                        }
                        return;
                    }
                    else
                    {
                        Refresh(childLayerItem, sublayer, isVisible);
                    }
                }
            }
        }

        private void AddLayersToInset(ESRI.ArcGIS.Client.Map map, ESRI.ArcGIS.Client.Map insetMap)
        {
            if (map == null) return;
            if (insetMap == null) return;

            foreach (Layer layer in map.Layers.Where(l => l is FeatureLayer))
            {
                // Esri web maps contain feature layers with a null ID and null Url, skip them
                if (layer.ID == null && (layer as FeatureLayer).Url == null) continue;

                var webClient = new WebClient();

                webClient.DownloadStringCompleted += WebClient_DownloadStringCompleted;
                webClient.DownloadStringAsync(new Uri((layer as FeatureLayer).Url + "?f=json"));
            }

            foreach (Layer layer in map.Layers.Where(l => (l is FeatureLayer) == false))
            {
                AddLayerToInset(layer);
            }
            insetMap.Extent = map.Extent;

            LayerVisibilityTree.LayerVisibilityChanged += new EventHandler<IsVisibleEventArgs>(LayerVisibilityTree_LayerVisibilityChanged);
        }

        private void AddLayerToInset(Layer layer)
        {
            if (layer == null) return;
            if (InsetMap == null) return;
            if (layer.ID != null && layer.ID == TextLayerID) return;

            var cloneLayer = LayerClone(layer);
            InsetMap.Layers.Add(cloneLayer);
        }

        private void RemoveLayerFromInset(Layer layer)
        {
            if (InsetMap == null) return;
            if (layer == null) return;

            var insetLayer = InsetMap.Layers[layer.ID];
            if (insetLayer != null)
            {
                InsetMap.Layers.Remove(insetLayer);
            }
        }

        private void AddInsetMarker(Map map)
        {
            if (map == null) return;

            var layer = map.Layers[InsetMapMarker];
            // If there is already a layer, no need to add one.
            if (layer != null) return;

            layer = CreateInsetMarker();
            // Find the location to place the marker, we need it to be above the "normal" layers
            // and below all of graphics Layers
            for (int i = 0; i < map.Layers.Count; i++)
            {
                if (map.Layers[i] is GraphicsLayer)
                {
                    map.Layers.Insert(i, layer);
                    break;
                }
            }
        }

        private GraphicsLayer CreateInsetMarker()
        {
            GraphicsLayer layer = new GraphicsLayer { ID = InsetMapMarker };
            layer.Graphics.Add(new Graphic
            {
                Symbol = new SimpleFillSymbol
                {
                    BorderBrush = new SolidColorBrush(Colors.Red),
                    BorderThickness = 2,
                }
            });
            return layer;
        }

        private void UnsubscribeMapEvents(ESRI.ArcGIS.Client.Map map)
        {
            if (map == null) return;

            map.Layers.CollectionChanged -= Layers_CollectionChanged;
            foreach (Layer layer in map.Layers)
            {
                UnsubscribeLayerEvents(layer);

                RemoveLayerFromInset(layer);
            }
        }

        private void UnsubscribeLayerEvents(Layer layer)
        {
            if (layer == null) return;

            layer.PropertyChanged -= Layer_PropertyChanged;
            GraphicsLayer graphicsLayer = layer as GraphicsLayer;
            if (graphicsLayer != null)
            {
                graphicsLayer.Graphics.CollectionChanged -= Graphics_CollectionChanged;
            }
        }

        private void SubscribeMapEvents(ESRI.ArcGIS.Client.Map map)
        {
            if (map == null) return;

            map.Layers.CollectionChanged += Layers_CollectionChanged;
            foreach (Layer layer in map.Layers.Where(l => (l is FeatureLayer) == false))
            {
                SubscribeLayerEvents(layer);
            }
        }

        private void SubscribeLayerEvents(Layer layer)
        {
            if (layer == null) return;

            layer.PropertyChanged += Layer_PropertyChanged;
            GraphicsLayer graphicsLayer = layer as GraphicsLayer;
            if (graphicsLayer != null)
            {
                graphicsLayer.Graphics.CollectionChanged += Graphics_CollectionChanged;
            }
        }

        private Graphic GraphicClone(Graphic source)
        {
            if (source == null) return null;

            Graphic graphic = new Graphic();
            graphic.Symbol = source.Symbol is CustomMarkerSymbol ? (source.Symbol as CustomMarkerSymbol).Clone() : source.Symbol;
            graphic.Selected = source.Selected;
            graphic.Geometry = ESRI.ArcGIS.Client.Geometry.Geometry.Clone(source.Geometry);
            graphic.MapTip = source.MapTip;
            graphic.TimeExtent = source.TimeExtent;

            Binding binding = new Binding("Selected") { Source = source, Mode = BindingMode.TwoWay };
            BindingOperations.SetBinding(graphic, Graphic.SelectedProperty, binding);

            foreach (var attribute in source.Attributes)
            {
                graphic.Attributes.Add(attribute);
            }
            return graphic;
        }

        private GraphicsLayer GraphicsLayerClone(GraphicsLayer source)
        {
            GraphicsLayer layer = new GraphicsLayer();
            layer.ID = source.ID;

            foreach (Graphic graphic in source.Graphics)
            {
                Graphic g = GraphicClone(graphic);
                layer.Graphics.Add(g);
            }
            return layer;
        }

        private Layer LayerClone(Layer source)
        {
            Layer layer = null;
            if (source is ArcGISTiledMapServiceLayer)
            {
                var tiledLayer = (ArcGISTiledMapServiceLayer)source;
                layer = new ArcGISTiledMapServiceLayer
                {
                    Url = tiledLayer.Url,
                    ProxyURL = tiledLayer.ProxyURL
                };
            }
            else if (source is ArcGISDynamicMapServiceLayer)
            {
                var dynamicLayer = (ArcGISDynamicMapServiceLayer)source;
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
                var imageLayer = (ArcGISImageServiceLayer)source;
                layer = new ArcGISImageServiceLayer
                {
                    Url = imageLayer.Url,
                    ProxyURL = imageLayer.ProxyURL
                };
            }
            else if (source is GraphicsLayer)
            {
                layer = GraphicsLayerClone(((GraphicsLayer)source));
            }
            else if (source is TileLayer)
            {
                var bingLayer = (TileLayer)source;
                layer = new TileLayer
                {
                    LayerStyle = bingLayer.LayerStyle,
                    ServerType = bingLayer.ServerType,
                    Token = bingLayer.Token
                };
            }
            else if (source is WmsLayer)
            {
                var wmsLayer = (WmsLayer)source;
                layer = new WmsLayer
                {
                    Url = wmsLayer.Url,
                    ProxyUrl = wmsLayer.ProxyUrl,
                    Version = wmsLayer.Version
                };
                layer.Initialized += Layer_Initialized;
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

        private void GetWMSLayers(WmsLayer.LayerInfo layerInfo, List<string> layers)
        {
            if (!string.IsNullOrEmpty(layerInfo.Name))
            {
                layers.Add(layerInfo.Name);
            }

            if (layerInfo.ChildLayers.Any())
            {
                foreach (var childLayerInfo in layerInfo.ChildLayers)
                {
                    GetWMSLayers(childLayerInfo, layers);
                }
            }
        }

        protected override void InitZoomBox(Map map)
        {
            base.InitZoomBox(map);
            PanCommand.Execute(null);
        }

        protected override void ZoomIn(object parameter)
        {
            base.ZoomIn(InsetMap);
            IdentifyChecked = false;
        }

        protected override void ZoomOut(object parameter)
        {
            base.ZoomOut(InsetMap);
            IdentifyChecked = false;
        }

        protected override void Pan(object parameter)
        {
            base.Pan(InsetMap);
            IdentifyChecked = false;

        }

        private void Identify(object parameter)
        {
            ZoomBox.IsEnabled = true;
            ZoomInChecked = false;
            ZoomOutChecked = false;
            PanChecked = false;
            CursorSet.SetID(InsetMap, IdentifyCursor);
        }

        #endregion Private Methods
    }
}
