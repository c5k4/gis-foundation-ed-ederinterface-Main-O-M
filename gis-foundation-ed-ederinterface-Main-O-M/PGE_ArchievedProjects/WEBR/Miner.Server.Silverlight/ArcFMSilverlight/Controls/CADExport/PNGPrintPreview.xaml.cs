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
using PageTemplates;
using Miner.Server.Client.Symbols;
using ESRI.ArcGIS.Client.Symbols;
using ArcFM.Silverlight.PGE.CustomTools;
using ESRI.ArcGIS.Client.Toolkit;
using System.Windows.Media;

namespace ArcFMSilverlight.Controls.CADExport
{
    public partial class PNGPrintPreview : ChildWindow
    {
        private static Map _schematicMap;
        private ESRI.ArcGIS.Client.Geometry.Envelope _env;
        private ESRI.ArcGIS.Client.Geometry.Envelope _savedExtent;
        private double _pngPageWidth = double.NaN;
        private double _unitConversionFactor;
        private DateTime _datetime;
        public string strPrimaryURL = string.Empty;
        public string strSecondaryURL = string.Empty;
        public string strDcviewURL { get; set; }
        public string strDuctURL = string.Empty;
        public string strStreetLightURL { get; set; }
        public string strLandbaseURL = string.Empty;
        public string lstLandBaseVisibleLayers { get; set; }
        private double _pngPageHeight = double.NaN;
        public string StoredDisplayName { get; set; }
        public string TemplateName { get; set; }
        public double exportScale { get; set; }
        public string exportDpi { get; set; }
        public List<string> exportMapTypeList{ get; set; }
        private Size _textSize;
        private const string CAD_EXPORT_GRAPHICS_LAYER = "SchematicsExportGraphics";
        private GraphicsLayer _graphicsLayer;
        private TextSymbol _textSymbol;

        public ESRI.ArcGIS.Client.Geometry.Envelope SavedExtent
        {
            get { return _savedExtent; }
            set { _savedExtent = value; }
        }
        public ESRI.ArcGIS.Client.Geometry.Envelope CurrentExtent
        {
            get { return _env; }
            set { _env = value; }
        }
        public Map SchematicMap
        {
            get { return _schematicMap; }
            set { _schematicMap = value; }
        }

        private CADExportControl _cadExportTool;
        public CADExportControl CadExportTool
        {
            get
            {
                return _cadExportTool;

            }

            set
            {
                _cadExportTool = value;
            }

        }
        public PNGPrintPreview()
        {
            InitializeComponent();
           // pngMapControl.Progress += new EventHandler<ProgressEventArgs>(pngMapControl_Progress);
            pngMapControl.Progress += new EventHandler<ProgressEventArgs>(pngMapControl_Progress);
         
            this.Loaded += new RoutedEventHandler(PNGPrintPreviewWindow_Loaded);
            string text = "Map Extent Saved";
            int fontSize = 25;
            FontFamily fontFamily = new System.Windows.Media.FontFamily("Arial");
            _textSize = GetTextSize(text, fontFamily, fontSize);

            _textSymbol = new TextSymbol()
            {
                FontFamily = new System.Windows.Media.FontFamily("Arial"),
                Foreground = new System.Windows.Media.SolidColorBrush(Colors.Cyan),
                FontSize = 25,
                Text = text,
                OffsetX = _textSize.Width / 2,
                OffsetY = _textSize.Height / 2
            };

            getLayerUrl();

        }
       
        void PNGPrintPreviewWindow_Loaded(object sender, RoutedEventArgs e)
        {
            IsLoading(true);
            if (StoredDisplayName.ToUpper().Contains("DIST") && StoredDisplayName.ToUpper().Contains("50"))
            {
                populateCbMaptypes();
                cboExportMapType.Visibility = Visibility.Visible;
                lblmaptype.Visibility = Visibility.Visible;
                mapControlSizeChange(TemplateName);
              
            }
            else
            {
                cboExportMapType.Visibility = Visibility.Collapsed;
                lblmaptype.Visibility = Visibility.Collapsed;
                mapControlSizeChange(TemplateName);
                LoadMapLayers(SchematicMap.Layers);
                // ScalepngMap(exportScale);
                pngMapControl.Extent = _env;
            }
           

            _graphicsLayer = new GraphicsLayer();
            _graphicsLayer.ID = CAD_EXPORT_GRAPHICS_LAYER;
            if (!pngMapControl.Layers.Contains(_graphicsLayer))
            {
                pngMapControl.Layers.Add(_graphicsLayer);
            }
           
            IsLoading(false);
            
            
        }

        public void populateCbMaptypes()
        {
            if (exportMapTypeList.Count > 0)
            {
                IList<string> codedDescriptions = exportMapTypeList;
                IList<string> codedValues = exportMapTypeList;
                Dictionary<string, string> feederDomainDictionary =
                    codedValues.Zip(codedDescriptions, (k, v) => new { k, v })
                        .ToDictionary(x => x.k, x => x.v);
                Dictionary<string, string> placeHolder = new Dictionary<string, string>();
                Dictionary<string, string> feederDictionary = placeHolder.Concat(feederDomainDictionary).ToDictionary(x => x.Key, x => x.Value);
                cboExportMapType.ItemsSource = feederDictionary;
                cboExportMapType.SelectedIndex = 0;
            }
            else
            {
                MessageBox.Show("Please select atleast one map type");
                this.Close();
            }
        }

        private void getLayerUrl()
        {
            if (Application.Current.Host.InitParams.ContainsKey("Config"))
            {
                string config = Application.Current.Host.InitParams["Config"];
                //var attribute = "";
                config = System.Windows.Browser.HttpUtility.HtmlDecode(config).Replace("***", ",");
                XElement elements = XElement.Parse(config);
                foreach (XElement element in elements.Elements())
                {
                    if (element.Name.LocalName == "Layers")
                    {
                        if (element.HasElements)
                        {
                            var i = 0;

                            foreach (XElement childelement in element.Elements())
                            {
                                if (childelement.Attribute("MapServiceName").Value.ToString() == "Elec Dist 50 Scale Primary View")
                                {
                                    strPrimaryURL = childelement.Attribute("Url").Value.ToString();
                                }
                                else if (childelement.Attribute("MapServiceName").Value.ToString() == "Elec Dist 50 Scale Secondary View")
                                {
                                    strSecondaryURL = childelement.Attribute("Url").Value.ToString();
                                }
                                else if (childelement.Attribute("MapServiceName").Value.ToString() == "Elec Dist 50 Scale Duct View")
                                {
                                    strDuctURL = childelement.Attribute("Url").Value.ToString();
                                }
                                else if (childelement.Attribute("MapServiceName").Value.ToString() == "Commonlandbase")
                                {
                                    strLandbaseURL = childelement.Attribute("Url").Value.ToString();
                                }

                            }
                        }
                    }
                }
            }
        }

        private void exportMapTypeCbox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //LayerCollection lyr = pngMapControl.Layers;
            //if (lyr != null)
            //{
            //    if (lyr.Count > 0)
            //    {
            //        foreach (Layer l in lyr)
            //        {
            //            pngMapControl.Layers.Remove(l);
            //        }
            //    }
            //}

            pngMapControl.Progress += new EventHandler<ProgressEventArgs>(pngMapControl_Progress1);
            string selectedMapType = cboExportMapType.SelectedValue.ToString();
            LayerCollection layers = GetLayers(selectedMapType);


            LoadMapLayers(layers);
            pngMapControl.Extent = _env;
        }

        void pngMapControl_Progress1(object sender, ProgressEventArgs e)
        {
            if (e.Progress == 100)
            {
                IsLoading(false);
                ScalepngMap(exportScale);
                pngMapControl.Progress -= pngMapControl_Progress1;
            }
            else IsLoading(true);
        }

        private LayerCollection GetLayers(string currentMapType)
        {
            ArcGISDynamicMapServiceLayer oppLayer = new ArcGISDynamicMapServiceLayer();

            //primary.Url = "http://edgisappqa01:6080/arcgis/rest/services/Data/Primary_50/MapServer";
            //primary.Visible = true;

            ArcGISDynamicMapServiceLayer landbaseLayer = new ArcGISDynamicMapServiceLayer();
            landbaseLayer.Url = strLandbaseURL;
            landbaseLayer.Visible = true;
            int[] myVisibleLayers = lstLandBaseVisibleLayers.Split(',').Select(s => Convert.ToInt32(s)).ToArray();
            landbaseLayer.VisibleLayers = myVisibleLayers;
            


            if (currentMapType == "Primary View" || currentMapType.ToUpper() == "PRIMARY")
            {
                oppLayer.Url = strPrimaryURL;
                oppLayer.Visible = true;
            }
            else if (currentMapType == "Secondary View" || currentMapType.ToUpper() == "SECONDARY")
            {
                oppLayer.Url = strSecondaryURL;
                oppLayer.Visible = true;
            }
            else if (currentMapType.ToUpper() == "DUCT VIEW" || currentMapType.ToUpper() == "DUCTVIEW")
            {
                oppLayer.Url = strDuctURL;
                oppLayer.Visible = true;
            }
            else if (currentMapType.ToUpper() == "DC VIEW" || currentMapType.ToUpper() == "DCVIEW")
            {
                oppLayer.Url = strDcviewURL;
                oppLayer.Visible = true;
            }

            else if (currentMapType == "StreetLight" || currentMapType.ToUpper() == "STREETLIGHT")
            {
                oppLayer.Url = strStreetLightURL;
                oppLayer.Visible = true;
            }


            LayerCollection lstLyr = new LayerCollection();

            lstLyr.Add(landbaseLayer);
            lstLyr.Add(oppLayer);
            LayerCollection lyrEnum = lstLyr;
            return lyrEnum;

        }

        public void mapControlSizeChange(string templateName)
        {
            if (templateName.Contains("_") && templateName.Contains("x") || templateName.Contains("X") &&
              (templateName.Contains("8-5") || templateName.Contains("11") || templateName.Contains("4")) &&
              (templateName.Contains("Landscape") || templateName.Contains("Portrait")))
            {
                string[] templateNameProperties = templateName.Split('_');
                string[] pageSize;
                if (templateName.Contains("X"))
                {
                    pageSize = templateNameProperties[1].Split('X');
                }
                else
                {
                     pageSize = templateNameProperties[1].Split('x');
                }
                string orientation = templateNameProperties[2];

                if (pageSize.Length == 2 && (orientation == "Portrait" || orientation == "Landscape"))
                {
                    //Template mxd filenames cannot contain periods in their 
                    //page size designation and work with the printing service.
                    //IE: x_8.5x11_x will break it. Convention needs to use a hyphen instead "-".
                    //Correct naming convention where decimal is needed: x_8-5x11_x
                    //This replacement is needed to parse the string into a double.
                    for (int i = 0; i < pageSize.Count(); i++)
                    {
                        if (pageSize[i].Contains("-")) pageSize[i] = pageSize[i].Replace("-", ".");
                    }

                    double longSide = double.NaN;
                    double shortSide = double.NaN;
                    double.TryParse(pageSize[0], out longSide);
                    double.TryParse(pageSize[1], out shortSide);

                    if (longSide != double.NaN && shortSide != double.NaN)
                    {
                        if (longSide < shortSide)
                        {
                            double temp = longSide;
                            longSide = shortSide;
                            shortSide = temp;
                        }

                        _pngPageWidth = (orientation == "Portrait") ? shortSide : longSide;
                        _pngPageHeight = (orientation == "Portrait") ? longSide : shortSide;

                        // avoiding cut-off between ad-hoc print window and ad-hoc print pdf (INC000004022811)
                        //_pngPageWidth = _pngPageWidth - 1;
                        //_pngPageHeight = _pngPageHeight - 1;

                        CalculatePrintArea(_pngPageWidth, _pngPageHeight, pngMapControl);

                    }
                }
            }
        }

        static public Size GetTextSize(string text, FontFamily fontFamily, int fontSize)
        {
            Size size = new Size(0, 0);

            TextBlock l = new TextBlock();
            l.FontFamily = fontFamily;
            l.FontSize = fontSize;
            //l.FontStyle = FontStyle ; 
            //l.FontWeight = font.Weight; 
            l.Text = text;
            size.Height = l.ActualHeight;
            size.Width = l.ActualWidth;

            return size;
        }

        private void CalculatePrintArea(double pageWidth, double pageHeight, Map map)
        {
            pngMapControl.Width = pageWidth * 96;
            pngMapControl.Height = pageHeight * 96;
            //ahMapControl.ZoomTo(initialExtent); 
        }

        public void LoadMapLayers(LayerCollection mainMapCollection)
        {

           
            Layer _newLayer = null;
            pngMapControl.Layers = new LayerCollection();

            foreach (Layer l in mainMapCollection)
            {
                try
                {
                    //if (!LayerIsPopulatedMinerPrivate(l))
                    //{
                   _newLayer = CloneLayer(l);
                    //}
                    //else
                    //{
                    //    _newLayer = null;
                    //}
                }
                catch (Exception e)
                {
                    // Clone failed
                    _newLayer = null;
                }

                if (_newLayer != null)
                {
                    // remove the commonlandbase from the Schematic ad-hoc print
                    if (StoredDisplayName.ToUpper().Contains("SCHEMATICS"))
                    {
                        if (_newLayer.ID != null)
                        {
                            if (_newLayer.ID.ToUpper() == "COMMONLANDBASE")
                            {
                                _newLayer.Visible = false;
                            }
                        }
                    }

                    pngMapControl.Layers.Add(_newLayer);
                    pngMapControl.Extent = _env;
                   // ScalepngMap(exportScale);
                   
                }
            }
            //ahMapControl.Extent = _env;
            pngMapControl.UpdateLayout();

            if (pngMapControl.SpatialReference.WKID == 102100)
            {
                _unitConversionFactor = 39.37 / 12; //meters to feet
            }
            else
            {
                _unitConversionFactor = 1;
            }
        }

        private void ScalepngMap(double userSelectedScale)
        {

            IsLoading(true);
                //_newResolution = _adHocResolutionRatio * _templateScale;
                //we are making a couple assumptions here 1. the dpi is 96 2. the map units are feet
            double _newResolution = userSelectedScale / (96 * _unitConversionFactor);
                pngMapControl.ZoomToResolution(_newResolution);
              //  pngMapControlChanged(_newResolution);
                IsLoading(false);
                //logger.Info("PRINTING: AdHoc Map Zoomed to new resolution. Resolution: " + _newResolution.ToString());
            
        }
        

       
        void pngMapControl_Progress(object sender, ProgressEventArgs e)
        {
            if (e.Progress == 100)
            {
               
                pngMapControl.Progress -= pngMapControl_Progress;
                //comboScaleSelection.SelectedIndex = comboScaleSelection.Items.Count() - 1;
                pngMapControl.MinimumResolution = 0.0000000123;
                //ahMapControl.ZoomTo(initialExtent);
                if (StoredDisplayName.ToUpper().Contains("DIST") && StoredDisplayName.ToUpper().Contains("50"))
                {
                    pngMapControl.Extent = _env;
                }
                else
                {
                    ScalepngMap(exportScale);
                }
                saveMapExtent.IsEnabled = true;
                //ScaleAdHocMap(initialScale.ToString());  
                IsLoading(false);
            }
            else IsLoading(true);
        }

        private void IsLoading(bool isLoadingInProgress)
        {

            //EnableDisablePrintButton();

            if (isLoadingInProgress)
            {
                BusyIndicator.BusyContent = "Loading Map...";
                BusyIndicator.IsBusy = true;
                IsEnabled = false;
            }
            else
            {
                BusyIndicator.IsBusy = false;
                IsEnabled = true;
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
                            if (fi.FieldType == typeof (DependencyProperty))
                            {
                                if (fi.Name.ToLower() == "textproperty")
                                {
                                    var dp = fi.GetValue(graphic.Symbol) as DependencyProperty;
                                    string dpValue = graphic.Symbol.GetValue(dp).ToString();
                                    
                                    if (!string.IsNullOrEmpty(dpValue))
                                    {
                                        //MessageBox.Show(dpValue);
                                        string dpValueXaml =
                                            "<TextBlock FontFamily=\"Arial\" FontSize=\"12\" Foreground=\"Black\" xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\"><Run>"+ dpValue + "</Run>\r\n</TextBlock>";
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
                            //clone.Symbol = traceSymbol;
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

       

        private void SaveMapExtentButton_Click(object sender, RoutedEventArgs e)
        {
            _cadExportTool.SchematicsPrintPreviewExtent= pngMapControl.Extent;
            Graphic graphicText = new Graphic()
            {
                Geometry = pngMapControl.Extent.GetCenter(),
                Symbol = _textSymbol,
            };

            if (!pngMapControl.Layers.Contains(_graphicsLayer))
            {
                pngMapControl.Layers.Add(_graphicsLayer);
            }

            _graphicsLayer.Graphics.Add(graphicText);
            saveMapExtent.IsEnabled = false;
            _cadExportTool.printPreviewBtn.Background = new SolidColorBrush(Colors.Green);
            this.Close();
          
          
        }
        private void ScrollViewer_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            MapPoint pannedLocation = pngMapControl.ScreenToMap(e.GetPosition(pngMapControl));
            pngMapControl.PanTo(pannedLocation);
        }
        void ahMapControl_ExtentChanged(object sender, ExtentEventArgs e)
        {
          
            Map mapControl = sender as Map;
            if (mapControl != null)
            {
                mapControl.MinimumResolution = System.Double.Epsilon;
            }

        }

        private void pngMapControl_ExtentChanged(object sender, ExtentEventArgs e)
        {
            Map mapControl = sender as Map;
            if (mapControl != null)
            {
                mapControl.MinimumResolution = System.Double.Epsilon;
            }
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

          
    }

    public static class CloneExtension
    {
        // Clones a dependency object.
        public static T Clone<T>(this T source) where T : DependencyObject
        {
            Type t = source.GetType(); // can be different from typeof(T)
            var clone = (T)Activator.CreateInstance(t);

            // Loop on CLR properties (except name, parent and graphics)
            foreach (PropertyInfo propertyInfo in t.GetProperties())
            {
                if (propertyInfo.Name == "Name" || propertyInfo.Name == "Parent" || propertyInfo.Name == "Graphics" || propertyInfo.Name == "ChildLayers" ||
                        !propertyInfo.CanRead || propertyInfo.GetGetMethod() == null ||
                        propertyInfo.GetIndexParameters().Length > 0)
                    continue;

                try
                {
                    Object value = propertyInfo.GetValue(source, null);
                    if (value != null)
                    {
                        if (propertyInfo.PropertyType.GetInterface("IList", true) != null && !propertyInfo.PropertyType.IsArray)
                        {
                            // Collection ==> loop on items and clone them (we suppose the collection itself is already initialized!)
                            var count = (int)propertyInfo.PropertyType.InvokeMember("get_Count", BindingFlags.InvokeMethod, null, value, null);
                            propertyInfo.PropertyType.InvokeMember("Clear", BindingFlags.InvokeMethod, null, propertyInfo.GetValue(clone, null), null); // without this line, text can be duplicated due to inlines objects added after text is set

                            for (int index = 0; index < count; index++)
                            {
                                object itemValue = propertyInfo.PropertyType.InvokeMember("get_Item", BindingFlags.InvokeMethod, null, propertyInfo.GetValue(source, null), new object[] { index });
                                propertyInfo.PropertyType.InvokeMember("Add", BindingFlags.InvokeMethod, null, propertyInfo.GetValue(clone, null), new[] { CloneDependencyObject(itemValue) });
                            }
                        }
                        else if (propertyInfo.CanWrite && propertyInfo.GetSetMethod() != null)
                        {
                            propertyInfo.SetValue(clone, CloneDependencyObject(value), null);
                        }
                    }
                }
                catch (Exception) { }
            }

            // Copy some useful attached properties (not done by reflection)
            if (source is UIElement)
            {
                DependencyProperty attachedProperty = ESRI.ArcGIS.Client.ElementLayer.EnvelopeProperty; // needed for ElementLayer
                SetDependencyProperty(attachedProperty, source, clone);
            }

            return clone;
        }

        static private object CloneDependencyObject(object source)
        {
            return source is DependencyObject && !(source is ControlTemplate) ? (source as DependencyObject).Clone() : source;
        }

        static private void SetDependencyProperty(DependencyProperty dp, DependencyObject source, DependencyObject clone)
        {
            Object value = source.GetValue(dp);
            if (value != null)
                clone.SetValue(dp, CloneDependencyObject(value));
        }
    }    
}

