using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Windows;
using System.Windows.Browser;
using System.Windows.Controls;
using System.Windows.Input;
using System.Xml.Linq;
using ESRI.ArcGIS.Client;
using ESRI.ArcGIS.Client.FeatureService.Symbols;
using ESRI.ArcGIS.Client.Geometry;
using NLog;
using PGnE.Printing;
using Miner.Server.Client.Symbols;
using ESRI.ArcGIS.Client.Symbols;
using ArcFM.Silverlight.PGE.CustomTools;
using ESRI.ArcGIS.Client.Toolkit;
using System.Windows.Media;


namespace ArcFMSilverlight
{
    public class PONSAdHocPrint
    {
        private static Map _adHocMap;
        private Logger logger = LogManager.GetCurrentClassLogger();
        private static int _dpiToPrintAt;
        private double _unitConversionFactor;
        private MapPoint centerPoint;
        private Envelope initialExtent;

        private TextSymbol _textSymbol;
        private MapPoint mapPoint;
        private static LineSymbol traceSymbol;
        private Map _ponsMap = new Map();

        #region public Properties
        public Map AdHocMap
        {
            get { return _adHocMap; }
            set { _adHocMap = value; }
        }
        public string EDMasterLayers { get; set; }
        public string SelectedStoredDisplayName { get; set; }
        //public double Scale { get; set; }
        public string AdHocTemplateNameSelected { get; set; }
        #endregion Properties

        public void PONSPrintMapAddLayers(HiResPrint _hiResPrint)
        {
            if (_adHocMap != null)
            {
                traceSymbol = new ESRI.ArcGIS.Client.Symbols.SimpleLineSymbol
                {
                    Color = new SolidColorBrush(Color.FromArgb(255, 255, 0, 0)),
                    Width = 4,
                    Style = ESRI.ArcGIS.Client.Symbols.SimpleLineSymbol.LineStyle.Solid
                };

                LoadMapLayers(_adHocMap.Layers);
                LoadEDMasterLayers();
                _ponsMap.Extent = _adHocMap.Extent;

                int _cntr = 0;
                foreach (Layer l in _ponsMap.Layers)
                {
                    if (l.ID == null)
                    {
                        l.ID = "GraphicsLayer" + _cntr.ToString();
                        _cntr++;
                    }
                }
            }
        }

        public void LoadMapLayers(LayerCollection mainMapCollection)
        {
            Layer _newLayer = null;
            _ponsMap.Layers = new LayerCollection();

            foreach (Layer l in mainMapCollection)
            {
                try
                {
                    _newLayer = CloneLayer(l);
                }
                catch (Exception e)
                {
                    // Clone failed
                    _newLayer = null;
                }

                if (_newLayer != null)
                {
                    _ponsMap.Layers.Add(_newLayer);
                    _ponsMap.Extent = _adHocMap.Extent;

                    logger.Info("PONS PRINTING: Adding layer to AdHoc map. Layer ID: " + _newLayer.ID);
                }
            }
            _ponsMap.UpdateLayout();
            logger.Info("PONS AH DESIRED PRINT EXTENT: " + _ponsMap.Extent);
            logger.Info("PONS AH ACTUAL EXTENT: " + _ponsMap.Extent);
            if (_ponsMap.SpatialReference != null && _ponsMap.SpatialReference.WKID == 102100)
            {
                _unitConversionFactor = 39.37 / 12; //meters to feet
            }
            else
            {
                _unitConversionFactor = 1;
            }
        }

        private static Layer CloneLayer(Layer layer)
        {
            Layer toLayer;

            var featureLayer = layer as FeatureLayer;

            //avoid feature layers
            if ((layer is FeatureLayer) && (featureLayer.Graphics.Count == 0 || layer.ID.ToString().IndexOf("Rollover_") == 0))
                return null;

            if (layer is GraphicsLayer && (featureLayer == null || featureLayer.Url == null || featureLayer.Mode != FeatureLayer.QueryMode.OnDemand || featureLayer.ID.Equals("WIP Label")))
            {
                // Clone the layer and the graphics
                var fromLayer = layer as GraphicsLayer;

                if (fromLayer.Graphics.Count > 0)
                {
                    if (fromLayer.Graphics[0].Attributes.Keys.Contains("LAYERNAME") && !featureLayer.ID.Equals("WIP Label"))
                    {
                        return null;
                    }
                }

                var printLayer = new GraphicsLayer
                {
                    Renderer = fromLayer.Renderer,
                    Clusterer = fromLayer.Clusterer == null ? null : fromLayer.Clusterer.Clone(),
                    ShowLegend = fromLayer.ShowLegend,
                    RendererTakesPrecedence = fromLayer.RendererTakesPrecedence,
                    ProjectionService = fromLayer.ProjectionService
                };

                toLayer = printLayer;

                var graphicCollection = new GraphicCollection();
                foreach (var graphic in fromLayer.Graphics)
                {
                    var clone = new Graphic();

                    foreach (var kvp in graphic.Attributes)
                    {
                        if (kvp.Value is DependencyObject)
                        {
                            // If the attribute is a dependency object --> clone it
                            var clonedkvp = new KeyValuePair<string, object>(kvp.Key, (kvp.Value as DependencyObject).Clone());
                            clone.Attributes.Add(clonedkvp);
                        }
                        else
                            clone.Attributes.Add(kvp);
                    }
                    clone.Geometry = graphic.Geometry;
                    if (graphic.Symbol is CustomTextSymbol)
                    {
                        CustomTextSymbol cts = (CustomTextSymbol)graphic.Symbol;
                        clone.Symbol = PageTemplates.Utilities.ConvertTextXamltoTextSymbol(cts.Xaml);
                        clone.Attributes.Remove("TextGraphic");
                    }
                    else if (graphic.Symbol is WipCustomTextSymbol)
                    {
                        WipCustomTextSymbol wts = (WipCustomTextSymbol)graphic.Symbol;
                        clone.Symbol = PageTemplates.Utilities.ConvertTextXamltoTextSymbol(wts.Xaml);
                        //ESRI's print service does not support attributes if the symbol is a TextSymbol
                        clone.Attributes.Clear();
                    }
                    else if (graphic.Symbol != null && graphic.Symbol.GetType().ToString() == "ArcFMSilverlight.RotatingTextSymbol")
                    {
                        var dependencyObjectType = graphic.Symbol.GetType();

                        foreach (FieldInfo fi in dependencyObjectType.GetFields(BindingFlags.Static | BindingFlags.Public))
                        {
                            if (fi.FieldType == typeof(DependencyProperty))
                            {
                                if (fi.Name.ToLower() == "textproperty")
                                {
                                    var dp = fi.GetValue(graphic.Symbol) as DependencyProperty;
                                    string dpValue = graphic.Symbol.GetValue(dp).ToString();

                                    if (!string.IsNullOrEmpty(dpValue))
                                    {
                                        //MessageBox.Show(dpValue);
                                        string dpValueXaml =
                                            "<TextBlock FontFamily=\"Arial\" FontSize=\"12\" Foreground=\"Black\" xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\"><Run>" + dpValue + "</Run>\r\n</TextBlock>";
                                        clone.Symbol = PageTemplates.Utilities.ConvertTextXamltoTextSymbol(dpValueXaml);
                                    }

                                }
                            }
                        }
                        clone.Attributes.Clear();
                    }

                    else
                    {
                        if (layer.ID != "PGE_Trace_Graphics")
                            clone.Symbol = graphic.Symbol;
                        else
                        {
                            clone.Symbol = traceSymbol;
                        }
                    }

                    clone.Selected = graphic.Selected;
                    clone.TimeExtent = graphic.TimeExtent;
                    clone.MapTip = graphic.MapTip;
                    graphicCollection.Add(clone);
                }

                printLayer.Graphics = graphicCollection;

                toLayer.ID = layer.ID;
                toLayer.Opacity = layer.Opacity;
                toLayer.Visible = layer.Visible;
                toLayer.MaximumResolution = layer.MaximumResolution;
                toLayer.MinimumResolution = layer.MinimumResolution;
            }
            else
            {
                // Clone other layer types
                toLayer = layer.Clone();

                if (layer is GroupLayerBase)
                {
                    // Clone sublayers (not cloned in Clone() to avoid issue with graphicslayer)
                    var childLayers = new LayerCollection();
                    foreach (Layer subLayer in (layer as GroupLayerBase).ChildLayers)
                    {
                        if (!LayerIsPopulatedMinerPrivate(subLayer))
                        {
                            var toSubLayer = CloneLayer(subLayer);

                            if (toSubLayer != null)
                            {
                                toSubLayer.InitializationFailed += (s, e) => { }; // to avoid crash if bad layer
                                childLayers.Add(toSubLayer);
                            }
                        }
                    }
                    ((GroupLayerBase)toLayer).ChildLayers = childLayers;
                }
            }
            return toLayer;
        }

        private static bool LayerIsPopulatedMinerPrivate(Layer layer)
        {
            if (layer is GraphicsLayer)
            {
                GraphicsLayer gl = layer as GraphicsLayer;

                if (gl.Graphics.Count > 0)
                {
                    Graphic graphic = gl.Graphics[0];
                    string graphicSymbolType = graphic.Symbol.GetType().ToString();
                    if (graphicSymbolType.Contains("Miner"))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public void ReadTemplatesConfig()
        {
            try
            {
                //Changes by Girish: When switching between Standard and AdHoc printing it was not populating the 
                //combo boxes, because it was bypassing ReadTemplatesConfig(). and not clearing AppResources.

                ClearAppResource("MapTypes");
                ClearAppResource("GridLayers");
                ClearAppResource("ScaleOptions");
                ClearAppResource("PrintServiceUrls");
                ClearAppResource("UpdateDelay");

                if (Application.Current.Host.InitParams.ContainsKey("TemplatesConfig"))
                {
                    string config = Application.Current.Host.InitParams["TemplatesConfig"];
                    config = HttpUtility.HtmlDecode(config).Replace("***", ",");
                    XElement xe = XElement.Parse(config);

                    if (!xe.HasElements) return;

                    /*****  Print Service Urls  *****/
                    List<Tuple<string, string, string, string>> printServiceUrls = new List<Tuple<string, string, string, string>>();
                    var printServiceUrlNodes = from psun in xe.Descendants("PrintServiceUrls")
                                               select psun;

                    foreach (XElement printServiceUrlNode in printServiceUrlNodes.Elements())
                    {
                        string name = printServiceUrlNode.Attribute("Name").Value;
                        string url = printServiceUrlNode.Attribute("Url").Value;
                        string dpi = printServiceUrlNode.Attribute("DPI").Value;
                        string updateDelay = printServiceUrlNode.Attribute("UpdateDelay").Value;
                        Tuple<string, string, string, string> PrintServiceNameAndUrl = new Tuple<string, string, string, string>(name, url, dpi, updateDelay);
                        printServiceUrls.Add(PrintServiceNameAndUrl);
                    }

                    Application.Current.Resources.Add("PrintServiceUrls", printServiceUrls);

                    var extractSendServiceNode = from essn in xe.Descendants("ExtractSendService")
                                                 select essn;
                    if (!Application.Current.Resources.Contains("ExtractSendService"))
                    {
                        Application.Current.Resources.Add("ExtractSendService", extractSendServiceNode.First());
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString(), "Failed Reading Templates Configuration", MessageBoxButton.OK);
            }
        }

        private void ClearAppResource(string resourceName)
        {
            if (Application.Current.Resources.Contains(resourceName))
                Application.Current.Resources.Remove(resourceName);
        }

        public void LoadEDMasterLayers()
        {
            string[] layersCSV = EDMasterLayers.Split(',');
            if (SelectedStoredDisplayName.ToUpper().Contains("SCHEMATICS"))
                EnableFeatureLayers();

            foreach (Layer layer in _ponsMap.Layers)
            {
                HandleTiledLayerVisibility(layer);
                if (!(layer is ArcGISDynamicMapServiceLayer)) continue;
                HandleDynamicLayerVisibility(layer, layersCSV);
            }
        }

        private void EnableFeatureLayers()
        {
            IList<string> _nonSchematicsLayers = _ponsMap.Layers.Where(l => l is FeatureLayer && l.Visible == true &&
                !l.ID.Contains("Rollover_") && !l.ID.Contains("Redline")).Select(l => l.ID).ToList();

            if (_nonSchematicsLayers == null) return;

            foreach (string layerID in _nonSchematicsLayers)
            {
                _ponsMap.Layers[layerID].Visible = true;
            }
        }

        private void HandleTiledLayerVisibility(Layer layer)
        {
            if (layer is ArcGISTiledMapServiceLayer)
            {
                layer.Visible = true;
            }
        }

        void HandleDynamicLayerVisibility(Layer layer, string[] layersCSV)
        {
            if (layersCSV.Contains(layer.ID))
            {
                layer.MaximumResolution = double.PositiveInfinity;
                if (layer.ID.Contains("Satellite") && !layer.Visible)
                {
                    layer.Visible = false;
                }
                else if ((layer.ID.Contains("Electric Transmission") || layer.ID.Contains("RealTime") || layer.ID.Contains("SAP") || layer.ID.Contains("TLM") || layer.ID.Contains("WIP Notes")) && !layer.Visible)
                {
                    layer.Visible = false;
                }
                else if (layer.ID.Contains("Satellite") && layer.Visible)
                {
                    ArcGISTiledMapServiceLayer agsStreets = _ponsMap.Layers["Streets"] as ArcGISTiledMapServiceLayer;
                    agsStreets.Visible = false;
                }
                else if (layer.ID.Contains("Commonlandbase"))
                {
                    ArcGISDynamicMapServiceLayer agsStreets = _ponsMap.Layers["Satellite"] as ArcGISDynamicMapServiceLayer;

                    if (agsStreets != null && agsStreets.Visible == true)
                    {
                        layer.Visible = false;
                    }
                    else
                    {
                        layer.Visible = true;
                    }
                }
                else if (!layer.ID.Contains("WIP(Search)"))
                {
                    layer.Visible = true;
                    logger.Info("PONS - Layer: " + layer.ID + " turned on.");
                }
            }
            else
            {
                layer.Visible = false;
                layer.MaximumResolution = 0;
                logger.Info("PONS - Layer: " + layer.ID + " turned off.");
            }

        }

        public HiResPrint HiResPrintObject()
        {
            HiResPrint _hiResPrint = null;
            string extractSendURL = "";
            int minimumAsyncSize = 0;
            if (Application.Current.Resources.Contains("ExtractSendService"))
            {
                XElement element = Application.Current.Resources["ExtractSendService"] as XElement;
                extractSendURL = element.Attribute("Url").Value;
                minimumAsyncSize = Convert.ToInt32(element.Attribute("MinimumAsyncSize").Value);
            }
            if (Application.Current.Resources.Contains("PrintServiceUrls"))
            {
                List<Tuple<string, string, string, string>> printServiceUrls = (List<Tuple<string, string, string, string>>)Application.Current.Resources["PrintServiceUrls"];
                foreach (var psu in printServiceUrls)
                {
                    // See Templates.config <PrintServiceUrls> node for explantaion of Print Services.
                    if (psu.Item1.ToLower() == "default")
                    {
                        string _printtaskURL = psu.Item2;
                        string dpi = psu.Item3;
                        bool dpiToIntResult = Int32.TryParse(dpi, out _dpiToPrintAt);
                        if (!dpiToIntResult)
                        {
                            MessageBox.Show(
                                "Could not parse DPI configured value to int.",
                                "Error Parsing DPI value for Print",
                                MessageBoxButton.OK);
                            logger.Error("PRINTING: Parse error. Could not parse configured DPI value: " + dpi + " to int.");
                        }

                        string updateDelay = psu.Item4;
                        int updateDelayMilliseconds;
                        bool updateDelayToIntResult = Int32.TryParse(updateDelay, out updateDelayMilliseconds);
                        Application.Current.Resources.Add("UpdateDelay", updateDelayMilliseconds);
                        if (!updateDelayToIntResult)
                        {
                            MessageBox.Show(
                                "Could not parse Update Delay configured value to int.",
                                "Error Parsing Update Delay value for Print",
                                MessageBoxButton.OK);
                            logger.Error("PRINTING: Parse error. Could not parse configured Update Delay value: " + updateDelay + " to int.");
                        }


                        _hiResPrint = new HiResPrint(new Uri(_printtaskURL), new Uri(extractSendURL), null,
                            minimumAsyncSize);

                        break;
                    }
                }
            }
            return _hiResPrint;
        }

        public void PONSPrintAdHocMap(HiResPrint _hiResPrint)
        {
            //Convert "." back to "-" to match the actual name of the template to be printed.
            string adHocTemplateNameOnServer = AdHocTemplateNameSelected.Contains(".") ?
                AdHocTemplateNameSelected.Replace(".", "-") : AdHocTemplateNameSelected;
            //Change the minres since ESRI does not convert it right
            _minres = new Dictionary<string, double>();
            foreach (Layer l in _ponsMap.Layers)
            {
                double scale = _ponsMap.Resolution;
                if (l.MinimumResolution < scale)
                {
                    _minres.Add(l.ID, l.MinimumResolution);
                    l.MinimumResolution = 0;
                }
            }
            //_hiResPrint.Print(ahMapControl.Scale, ahMapControl, comboPageSizeSelection.SelectedItem.ToString(), ahMapControl.Extent, null, Utilites.PrintFormat.PDF, 96);
            _hiResPrint.Print(0, _ponsMap, adHocTemplateNameOnServer, _ponsMap.Extent, null, Utilites.PrintFormat.PDF, _dpiToPrintAt);

            logger.Info("PRINTING: Ad Hoc print request sent to server.");
        }

        private Dictionary<string, double> _minres;      
    }
   
}
