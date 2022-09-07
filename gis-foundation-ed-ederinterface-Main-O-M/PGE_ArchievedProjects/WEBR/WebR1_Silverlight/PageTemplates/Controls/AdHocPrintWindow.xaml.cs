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


namespace PageTemplates
{
    public partial class AdHocPrintWindow : ChildWindow
    {
        private static Map _adHocMap;
        private ESRI.ArcGIS.Client.Geometry.Envelope _env;
        private PGnE.Printing.HiResPrint _hiResPrint = null;
        private Logger logger = LogManager.GetCurrentClassLogger();
        private static bool _isAdHocMapTypeScaleRatioSet = false;
        private static double _adHocResolutionRatio = double.NaN;
        private static double _newResolution = double.NaN;
        private static int _dpiToPrintAt;
        private static double _templateScale = double.NaN;
        private static double _initialAdHocScale = double.NaN;
        private static string _adHocScaleLayerUrl;
        private string _printJobId;
        private ESRI.ArcGIS.Client.Geometry.MapPoint _centerPoint;
        private static string _printAreaLayerID = "PrintAreaLayer";
        private double _pageWidth = double.NaN;
        private double _pageHeight = double.NaN;
        private Envelope _printAreaExtent = null;
        private bool _isConfiguredScale = false;
        private double _oldResolution = double.NaN;
        private double _unitConversionFactor;

        private TextSymbol _textSymbol;
        private MapPoint mapPoint;
        private Envelope initialExtent;
        private MapPoint centerPoint;
        private double initialScale;
        private static LineSymbol traceSymbol;
       private int _panValue = 0; //INC000004049426 & INC000004413542

        //START - WEBR Stability (Enable pan)
       private bool isMouseLeftButtonDown;
       private double originalMouseLeftButtonDownXCoord;
       private double originalMouseLeftButtonDownYCoord;
       private MapPoint originalMapPoint;
       //  ~~  Start - AG modified (Enable Pan and Center) 2020/08/19
       private bool isCenterToolActive = true;
       //  ~~  End - AG modified (Enable Pan and Center) 2020/08/19
        //END - WEBR Stability (Enable pan)

        #region Properties

        public ESRI.ArcGIS.Client.Geometry.MapPoint CenterPoint
        {
            get { return _centerPoint; }
            set { _centerPoint = value; }
        }

        public Map AdHocMap
        {
            get { return _adHocMap; }
            set { _adHocMap = value; }
        }
        public ESRI.ArcGIS.Client.Geometry.Envelope CurrentExtent
        {
            get { return _env; }
            set { _env = value; }
        }
        public string UserEmail { get; set; }
        public string StoredDisplayName { get; set; }
        public bool PanVisibility { get; set; }  //INC000004049426 & INC000004413542

        #endregion Properties

        //public AdHocPrintWindow()//(bool panVisibility, int panValue)
        public AdHocPrintWindow(bool panVisibility, int panValue)
        {
            InitializeComponent();
            ahMapControl.Progress += new EventHandler<ProgressEventArgs>(ahMapControl_Progress);
            //ahMapControl.ExtentChanged += new EventHandler<ExtentEventArgs>(ahMapControl_ExtentChanged);
            this.Loaded += new RoutedEventHandler(AdHocPrintWindow_Loaded);
            //ahMapControl.Unloaded += new RoutedEventHandler(ahMapControl_Unloaded);
            //Add ssbl
            List<string> scalelist = new List<string>();
            //ahMapControl.Unloaded += new RoutedEventHandler(ahMapControl_Unloaded);
            string config = Application.Current.Host.InitParams["TemplatesConfig"];
            config = HttpUtility.HtmlDecode(config).Replace("***", ",");
            XElement xe = XElement.Parse(config);

            var ScaleOptionNodes = from son in xe.Descendants("ScaleOptions")
                                   select son;
            //var a = ScaleOptionNodes.ToList();
            foreach (XElement ScaleOptionNode in ScaleOptionNodes.Elements())
            {
                string displayText = ScaleOptionNode.Attribute("DisplayText").Value;
                string value = ScaleOptionNode.Attribute("Value").Value;
                scalelist.Add(displayText);

            }
            ZoomToScaleTextbox.ItemsSource = scalelist;

            //INC000004049426 & INC000004413542
            if (panVisibility)
                PanButtons.Visibility = Visibility.Visible;
            else
                PanButtons.Visibility = Visibility.Collapsed;
            _panValue = panValue;
        }

        //Add


        private void ZoomToScaleTextbox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (e != null)
                {
                    if (e.AddedItems.Count > 0)
                    {
                        ZoomToScale(e.AddedItems[0].ToString());
                    }
                }
            }
            catch { }
        }

        private void ZoomToScale(string scaleValue)
        {
            try
            {
                if (scaleValue.Contains(":"))
                {
                    char[] delimiterChars = { ':' };
                    string[] scale = scaleValue.Split(delimiterChars);
                    scaleValue = scale[1].ToString();
                }
                ScaleAdHocMap(scaleValue);
            }
            catch { }
        }

        private void ZoomToScaleTextbox_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            try
            {
                if (e != null)
                {
                    if (e.Key.ToString() == "Enter")
                    {
                        ZoomToScale(ZoomToScaleTextbox.Text.ToString());
                    }
                }
            }
            catch { }

        }

        private void ZoomToScaleTextbox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (e != null)
            {
                ZoomToScale(ZoomToScaleTextbox.Text.ToString());
            }
        }

        void ahMapControl_ExtentChanged(object sender, ExtentEventArgs e)
        {
            if (Math.Round(ahMapControl.Resolution, 3) != Math.Round(_oldResolution, 3))
            {
                //SelectCustomScale();
            }

            Map mapControl = sender as Map;
            if (mapControl != null)
            {
                mapControl.MinimumResolution = System.Double.Epsilon;
            }

            if (mapControl != null) _oldResolution = mapControl.Resolution;
            //check each layer for on demand features that may have loaded
            //WIP Label is not a feature layer commenting out for now
            //foreach (Layer l in mapControl.Layers)
            //{
            //    if (l is FeatureLayer)
            //    {
            //        if (((FeatureLayer)l).Mode == FeatureLayer.QueryMode.OnDemand)
            //        {
            //            foreach (var graphic in ((GraphicsLayer)l).Graphics)
            //            {
            //                if (graphic.Symbol is WipCustomTextSymbol)
            //                {
            //                    WipCustomTextSymbol wts = (WipCustomTextSymbol)graphic.Symbol;
            //                    graphic.Symbol = Utilities.ConvertTextXamltoTextSymbol(wts.Xaml);
            //                }
            //            }
            //        }
            //    }
            //}
        }

        void SelectCustomScale()
        {
            if (_isConfiguredScale)
            {
                _isConfiguredScale = false;
                return;
            }
            if (comboScaleSelection.Items.Count <= 0) return;
            ComboBoxItem item = (ComboBoxItem)comboScaleSelection.Items[comboScaleSelection.Items.Count() - 1];
            item.Tag = ahMapControl.Scale;

            comboScaleSelection.SelectedIndex = comboScaleSelection.Items.Count() - 1;
        }

        void ahMapControl_Progress(object sender, ProgressEventArgs e)
        {
            if (e.Progress == 100)
            {
                IsLoading(false);
                ahMapControl.Progress -= ahMapControl_Progress;
                comboScaleSelection.SelectedIndex = comboScaleSelection.Items.Count() - 1;
                ahMapControl.MinimumResolution = 0.0000000123;
                //ahMapControl.ZoomTo(initialExtent);
                //ScaleAdHocMap(initialScale.ToString());  
            }
            else IsLoading(true);
        }

        void ahMapControl_Progress1(object sender, ProgressEventArgs e)
        {
            if (e.Progress == 100)
            {
                IsLoading(false);
                ahMapControl.Progress -= ahMapControl_Progress1;
            }
            else IsLoading(true);
        }

        void AdHocPrintWindow_Loaded(object sender, RoutedEventArgs e)
        {
            IsLoading(true);

            //Changes by Girish: When switching between Standard and AdHoc printing it was not populating the 
            //combo boxes, because it was bypassing ReadTemplatesConfig(). and not clearing AppResources.

            ClearAppResource("MapTypes");
            ClearAppResource("GridLayers");
            ClearAppResource("ScaleOptions");
            ClearAppResource("PrintServiceUrls");
            ClearAppResource("UpdateDelay");

            ReadTemplatesConfig();

            if (_adHocMap != null)
            {
                if (!double.IsNaN(_adHocMap.Resolution) && !double.IsNaN(_adHocMap.Scale))
                {
                    _adHocResolutionRatio = _adHocMap.Resolution / _adHocMap.Scale;

                    if (!double.IsNaN(_adHocResolutionRatio)) _isAdHocMapTypeScaleRatioSet = true;
                }

                traceSymbol = new ESRI.ArcGIS.Client.Symbols.SimpleLineSymbol
                {
                    Color = new SolidColorBrush(Color.FromArgb(255, 255, 0, 0)),
                    Width = 4,
                    Style = ESRI.ArcGIS.Client.Symbols.SimpleLineSymbol.LineStyle.Solid
                };

                LoadMapLayers(_adHocMap.Layers);
                ahMapControl.Extent = _adHocMap.Extent;

                //To fix the bug Significant shift in the Print PDF 

                initialExtent = new Envelope();
                centerPoint = new MapPoint();
                centerPoint.X = _adHocMap.Extent.XMin + (_adHocMap.Extent.XMax - _adHocMap.Extent.XMin) / 2;
                centerPoint.Y = _adHocMap.Extent.YMin + (_adHocMap.Extent.YMax - _adHocMap.Extent.YMin) / 2;

                initialExtent.XMin = _adHocMap.Extent.XMin + (_adHocMap.Extent.XMax - _adHocMap.Extent.XMin) / 2 - 200;
                initialExtent.XMax = _adHocMap.Extent.XMin + (_adHocMap.Extent.XMax - _adHocMap.Extent.XMin) / 2 + 200;
                initialExtent.YMin = _adHocMap.Extent.YMin + (_adHocMap.Extent.YMax - _adHocMap.Extent.YMin) / 2 - 200;
                initialExtent.YMax = _adHocMap.Extent.YMin + (_adHocMap.Extent.YMax - _adHocMap.Extent.YMin) / 2 + 200;

                initialScale = _adHocMap.Resolution * 96 * _unitConversionFactor;

                //ahMapControl.Width = MapPanel.Width;
                //ahMapControl.Height = MapPanel.Height;

                Border border = VisualTreeHelper.GetChild(_adHocMap, 0) as Border;
                GraphicsLayer _graphicsLayer = new GraphicsLayer();
                _graphicsLayer.ID = "TextGraphic";
                ahMapControl.Layers.Add(_graphicsLayer);

                foreach (UIElement infoelement in (border.Child as Grid).Children)
                {
                    if (infoelement is InfoWindow)
                    {

                        mapPoint = ((InfoWindow)(infoelement)).Anchor as MapPoint;

                        _textSymbol = new TextSymbol()
                        {
                            FontFamily = new FontFamily("Arial"),
                            Foreground = new SolidColorBrush(Colors.Black),
                            FontSize = 12,
                        };


                        if (((InfoWindow)(infoelement)).Content is MapPoint)
                        {
                            _textSymbol.Text = "X=" + Math.Round(((MapPoint)(((InfoWindow)(infoelement)).Content)).X, 4) + Environment.NewLine +
                                                "Y=" + Math.Round(((MapPoint)(((InfoWindow)(infoelement)).Content)).Y, 4);
                        }
                        else
                        {
                            _textSymbol.Text = "Lat=" + Math.Round(Convert.ToDouble(((LatLongDisplayObj)(((ContentControl)((InfoWindow)(infoelement))).Content)).Lat), 4) +
                                Environment.NewLine +
                                "Long=" + Math.Round(Convert.ToDouble(((LatLongDisplayObj)(((ContentControl)((InfoWindow)(infoelement))).Content)).Long), 4);
                        }


                        Graphic graphicText = new Graphic()
                        {
                            Geometry = mapPoint,
                            Symbol = _textSymbol,
                        };
                        //var graphic1 = new Graphic { Symbol = new SimpleMarkerSymbol(), Geometry = p };

                        Graphic graphicPoint = new Graphic()
                        {
                            Geometry = mapPoint,
                            Symbol = new ESRI.ArcGIS.Client.Symbols.SimpleMarkerSymbol(),

                        };

                        _graphicsLayer.Graphics.Add(graphicPoint);
                        _graphicsLayer.Graphics.Add(graphicText);
                    }
                }

            }

            GetMapTemplates();
            PopulateMapScaleCombo();


            if (_isAdHocMapTypeScaleRatioSet)
            {
                //ScaleAdHocMap();
                PrintAdHocMapButton.IsEnabled = true;
            }
            else
            {
                MessageBox.Show("Error getting Scale ratio from base map.", "Map Scale Ratio / Resolution Error", MessageBoxButton.OK);
                this.CancelButton_Click(sender, e);
            }

            IsLoading(false);
            comboPageSizeSelection.SelectionChanged += new SelectionChangedEventHandler(comboPageSizeSelection_SelectionChanged);
            comboScaleSelection.SelectionChanged += new SelectionChangedEventHandler(comboScaleSelection_SelectionChanged);

            //comboScaleSelection.SelectedIndex = comboScaleSelection.Items.Count() - 1;

            ahMapControl.SizeChanged += new SizeChangedEventHandler(ahMapControl_SizeChanged);
        }

        void ahMapControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            try
            {
                if (e.NewSize.Width > e.NewSize.Height)
                {
                    initialExtent.XMax = centerPoint.X + ahMapControl.Resolution * e.NewSize.Height / 2;
                    initialExtent.XMin = centerPoint.X - ahMapControl.Resolution * e.NewSize.Height / 2;
                    initialExtent.YMin = centerPoint.Y - ahMapControl.Resolution * e.NewSize.Height / 2;
                    initialExtent.YMax = centerPoint.Y + ahMapControl.Resolution * e.NewSize.Height / 2;
                }
                else
                {
                    initialExtent.XMax = centerPoint.X + ahMapControl.Resolution * e.NewSize.Width / 2;
                    initialExtent.XMin = centerPoint.X - ahMapControl.Resolution * e.NewSize.Width / 2;
                    initialExtent.YMin = centerPoint.Y - ahMapControl.Resolution * e.NewSize.Width / 2;
                    initialExtent.YMax = centerPoint.Y + ahMapControl.Resolution * e.NewSize.Width / 2;
                }

                ahMapControl.Progress += new EventHandler<ProgressEventArgs>(ahMapControl_Progress1);
                ahMapControl.ZoomTo(initialExtent);
            }
            catch (Exception)
            {

                throw;
            }

            //ScaleAdHocMap(initialScale.ToString());  
            //throw new NotImplementedException();            
        }

        public void ReadTemplatesConfig()
        {
            try
            {
                if (Application.Current.Host.InitParams.ContainsKey("TemplatesConfig"))
                {
                    string config = Application.Current.Host.InitParams["TemplatesConfig"];
                    config = HttpUtility.HtmlDecode(config).Replace("***", ",");
                    XElement xe = XElement.Parse(config);

                    if (!xe.HasElements) return;

                    /*****  Scale Options  *****/
                    List<Tuple<string, string>> scaleOptions = new List<Tuple<string, string>>();
                    var scaleOptionNodes = from son in xe.Descendants("ScaleOptions")
                                           select son;

                    foreach (XElement scaleOptionNode in scaleOptionNodes.Elements())
                    {
                        string displayText = scaleOptionNode.Attribute("DisplayText").Value;
                        string value = scaleOptionNode.Attribute("Value").Value;
                        Tuple<string, string> scaleOption = new Tuple<string, string>(displayText, value);
                        scaleOptions.Add(scaleOption);
                    }

                    Application.Current.Resources.Add("ScaleOptions", scaleOptions);

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

        public void LoadMapLayers(LayerCollection mainMapCollection)
        {


            Layer _newLayer = null;
            ahMapControl.Layers = new LayerCollection();

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
                        if (_newLayer.ID.ToUpper() == "COMMONLANDBASE")
                        {
                            _newLayer.Visible = false;
                        }
                    }

                    ahMapControl.Layers.Add(_newLayer);
                    ahMapControl.Extent = _env;

                    logger.Info("PRINTING: Adding layer to AdHoc map. Layer ID: " + _newLayer.ID);
                }
            }
            //ahMapControl.Extent = _env;
            ahMapControl.UpdateLayout();
            logger.Info("AH DESIRED PRINT EXTENT: " + _env);
            logger.Info("AH ACTUAL EXTENT: " + ahMapControl.Extent);
            if (ahMapControl.SpatialReference.WKID == 102100)
            {
                _unitConversionFactor = 39.37 / 12; //meters to feet
            }
            else
            {
                _unitConversionFactor = 1;
            }
        }

        /// <summary>
        /// This method scales the page template viewer map for Ad Hoc maps
        /// on subsequent launches of an Ad Hoc map template.
        /// </summary>
        /// <param name="userSelectedScale"></param>
        private void ScaleAdHocMap(string userSelectedScale)
        {
            if (double.TryParse(userSelectedScale, out _templateScale))
            {
                //_newResolution = _adHocResolutionRatio * _templateScale;
                //we are making a couple assumptions here 1. the dpi is 96 2. the map units are feet
                _newResolution = _templateScale / (96 * _unitConversionFactor);
                ahMapControl.ZoomToResolution(_newResolution);

                logger.Info("PRINTING: AdHoc Map Zoomed to new resolution. Resolution: " + _newResolution.ToString());
            }
        }

        private void GetMapTemplates()
        {
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


                        _hiResPrint = new HiResPrint(new Uri(_printtaskURL), new Uri(extractSendURL), this.UserEmail,
                            minimumAsyncSize);

                        _hiResPrint.PrintTemplates += new EventHandler<PrintTemplatesEventArgs>(_hiResPrint_PrintTemplates);
                        _hiResPrint.GetLayoutsAsync();

                        break;
                    }
                }
            }
        }

        void _hiResPrint_PrintTemplates(object sender, PrintTemplatesEventArgs e)
        {
            System.Collections.Generic.List<string> _templates2 = new List<string>();
            System.Collections.Generic.List<string> _templates = e.PrintTemplates.ToList();

            //This loop replaces "-" with "." so that page sizes resemble numbers.
            //This is a UX enhancement only so 8.5 appears instead of 8-5 in the combobox
            for (int i = 0; i < _templates.Count(); i++)
            {
                if (_templates[i].Contains("-")) _templates[i] = _templates[i].Replace("-", ".");
            }

            _templates2.Insert(0, "AdHocMap_8.5x11_Portrait");
            _templates2.Insert(1, "AdHocMap_8.5x11_Landscape");
            int index = 2;
            foreach (string templatename in _templates)
            {
                if (templatename != "AdHocMap_8.5x11_Landscape" && templatename != "AdHocMap_8.5x11_Portrait" && templatename.Contains("AdHocMap"))  //For INC000004403856 -- CMCS template addition
                {

                    _templates2.Insert(index, templatename);
                    index++;
                }

            }


            comboPageSizeSelection.Items.Clear();
            comboPageSizeSelection.ItemsSource = from t in _templates2
                                                 where (t != "MAP_ONLY")
                                                 select t;

            comboPageSizeSelection.SelectedIndex = 0;
        }

        private void PopulateMapScaleCombo()
        {
            if (Application.Current.Resources.Contains("ScaleOptions"))
            {
                List<Tuple<string, string>> scaleSelectionItems = (List<Tuple<string, string>>)Application.Current.Resources["ScaleOptions"];

                foreach (var scaleItem in scaleSelectionItems)
                {
                    comboScaleSelection.Items.Add(new ComboBoxItem { Content = scaleItem.Item1, Tag = scaleItem.Item2 });
                }
            }
            comboScaleSelection.Items.Add(new ComboBoxItem { Content = "Custom Scale", Tag = _adHocMap.Scale });
        }

        void comboScaleSelection_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //Ad Hoc subsequent times through
            string selectedScaleName = ((ComboBoxItem)comboScaleSelection.SelectedItem).Content.ToString().ToLower();

            if (selectedScaleName == "custom scale")
            {
                _isConfiguredScale = false;
                //return;
            }
            else
            {
                _isConfiguredScale = true;
            }
            string userSelectedScale = ((ComboBoxItem)comboScaleSelection.SelectedItem).Tag.ToString();
            if (_isConfiguredScale)
            {
                initialScale = Convert.ToDouble(userSelectedScale);
                ScaleAdHocMap(userSelectedScale);
            }
            //CalculatePrintArea(_pageWidth, _pageHeight, ahMapControl);

        }

        void comboPageSizeSelection_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox cb = sender as ComboBox;
            string templateName = (string)cb.SelectedItem;
            templateName = templateName.ToLower();

            if (templateName.Contains("_") && templateName.Contains("x") &&
               (templateName.Contains("8-5") || templateName.Contains("11") || templateName.Contains("4")) &&
               (templateName.Contains("landscape") || templateName.Contains("portrait")))
            {
                string[] templateNameProperties = templateName.Split('_');

                string[] pageSize = templateNameProperties[1].Split('x');
                string orientation = templateNameProperties[2];

                if (pageSize.Length == 2 && (orientation == "portrait" || orientation == "landscape"))
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

                        _pageWidth = (orientation == "portrait") ? shortSide : longSide;
                        _pageHeight = (orientation == "portrait") ? longSide : shortSide;

                        // avoiding cut-off between ad-hoc print window and ad-hoc print pdf (INC000004022811)
                        _pageWidth = _pageWidth - 1;
                        _pageHeight = _pageHeight - 2;

                        CalculatePrintArea(_pageWidth, _pageHeight, ahMapControl);

                    }
                }
                else ReportNamingConventionError();
            }
            else
            {
                ReportNamingConventionError();
            }

            this.Cursor = Cursors.Arrow;
        }

        private void ReportNamingConventionError()
        {
            MessageBox.Show("Naming convention error found in template names returned from the server.",
                "Server Template Naming Convention Error", MessageBoxButton.OK);

            logger.Error("Naming convention error of configured Ad Hoc Map Templates." + Environment.NewLine
                + "Required Format: <Name>_<Length>x<Width>_<Portrait or Landscape>");
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            // pass in jobid _hiResPrint.CancelPrint(jobid);
            if (!string.IsNullOrEmpty(_printJobId))
            {
                _hiResPrint.CancelPrint(_printJobId);
                _printJobId = null;
                CancelButton.Content = "Close";
            }
            else
            {
                //this.DialogResult = false;
                this.Close();
            }
        }
        private Dictionary<string, double> _minres;
        private void ResetLayers()
        {
            if (_minres != null)
            {
                foreach (Layer l in ahMapControl.Layers)
                {
                    if (_minres.ContainsKey(l.ID))
                    {
                        l.MinimumResolution = _minres[l.ID];
                    }
                }
            }
        }
        private void PrintAdHocMapButton_Click(object sender, RoutedEventArgs e)
        {
            _hiResPrint.PrintJobSubmitted += new EventHandler<PrintJobSubmittedEventArgs>(_hiResPrint_PrintJobSubmitted);
            _hiResPrint.PrintJobCompleted += new EventHandler<EventArgs>(_hiResPrint_PrintJobCompleted);

            _hiResPrint.PrintJobCancelling += new EventHandler<EventArgs>(_hiResPrint_PrintJobCancelling);
            _hiResPrint.PrintJobCancelled += new EventHandler<EventArgs>(_hiResPrint_PrintJobCancelled);

            //Convert "." back to "-" to match the actual name of the template to be printed.
            string adHocTemplateNameOnServer = comboPageSizeSelection.SelectedItem.ToString().Contains(".") ?
                comboPageSizeSelection.SelectedItem.ToString().Replace(".", "-") :
                comboPageSizeSelection.SelectedItem.ToString();
            //Change the minres since ESRI does not convert it right
            _minres = new Dictionary<string, double>();
            foreach (Layer l in ahMapControl.Layers)
            {
                double scale = ahMapControl.Resolution;
                if (l.MinimumResolution < scale)
                {
                    _minres.Add(l.ID, l.MinimumResolution);
                    l.MinimumResolution = 0;
                }
            }
            //_hiResPrint.Print(ahMapControl.Scale, ahMapControl, comboPageSizeSelection.SelectedItem.ToString(), ahMapControl.Extent, null, Utilites.PrintFormat.PDF, 96);
            _hiResPrint.Print(ahMapControl.Scale, ahMapControl, adHocTemplateNameOnServer, ahMapControl.Extent, null, Utilites.PrintFormat.PDF, _dpiToPrintAt);

            logger.Info("PRINTING: Ad Hoc print request sent to server.");

            PrintAdHocMapButton.Content = "Generating PDF...";
            CancelButton.Content = "Cancel Print";
            PrintAdHocMapButton.IsEnabled = false;
            EnableDisableControls(false);
        }

        void _hiResPrint_PrintJobCancelled(object sender, EventArgs e)
        {
            ResetLayers();
            // close child window???
            CancelButton.Content = "Close";
            EnableDisableControls(true);
        }

        void _hiResPrint_PrintJobCancelling(object sender, EventArgs e)
        {
            PrintAdHocMapButton.Content = "Print Ad Hoc Map";
            PrintAdHocMapButton.IsEnabled = true;
            logger.Info("PRINTING: AdHoc cancel sent to server.");

            //_hiResPrint.PrintJobCancelling -= _hiResPrint_PrintJobCancelling;
        }

        void _hiResPrint_PrintJobSubmitted(object sender, PrintJobSubmittedEventArgs e)
        {
            _printJobId = e.JobId;
            _hiResPrint.PrintJobSubmitted -= _hiResPrint_PrintJobSubmitted;
        }

        void _hiResPrint_PrintJobCompleted(object sender, EventArgs e)
        {
            _hiResPrint.PrintJobCompleted -= _hiResPrint_PrintJobCompleted;
            ResetLayers();
            PrintAdHocMapButton.Content = "Print Ad Hoc Map";
            PrintAdHocMapButton.IsEnabled = true;
            EnableDisableControls(true);

            CancelButton.Content = "Close";
            _printJobId = string.Empty;


            logger.Info("PRINTING: Print job completed.");
        }

        private void CalculatePrintArea(double pageWidth, double pageHeight, Map map)
        {
            ahMapControl.Width = pageWidth * 96;
            ahMapControl.Height = pageHeight * 96;
            //ahMapControl.ZoomTo(initialExtent); 
        }

        private void ScrollViewer_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            MapPoint pannedLocation = ahMapControl.ScreenToMap(e.GetPosition(ahMapControl));
            ahMapControl.PanTo(pannedLocation);
        }

        private void EnableDisableControls(bool isEnable)
        {
            ahMapContainer.IsEnabled = isEnable;
            comboPageSizeSelection.IsEnabled = isEnable;
            comboScaleSelection.IsEnabled = isEnable;
            PrintAdHocMapButton.IsEnabled = isEnable;

            //INC000004049426 and INC000004413542 - enable pan in ad hoc
            PanN.IsEnabled = isEnable;
            PanS.IsEnabled = isEnable;
            PanE.IsEnabled = isEnable;
            PanW.IsEnabled = isEnable;
            PanNE.IsEnabled = isEnable;
            PanNW.IsEnabled = isEnable;
            PanSE.IsEnabled = isEnable;
            PanSW.IsEnabled = isEnable;
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
                        clone.Symbol = Utilities.ConvertTextXamltoTextSymbol(cts.Xaml);
                        clone.Attributes.Remove("TextGraphic");
                    }
                    else if (graphic.Symbol is WipCustomTextSymbol)
                    {
                        WipCustomTextSymbol wts = (WipCustomTextSymbol)graphic.Symbol;
                        clone.Symbol = Utilities.ConvertTextXamltoTextSymbol(wts.Xaml);
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
                                        clone.Symbol = Utilities.ConvertTextXamltoTextSymbol(dpValueXaml);
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



        //INC000004049426 and INC000004413542 - enable pan in ad hoc
        private void PanClick(object sender, RoutedEventArgs e)
        {
            if (_panValue > 0)
            {
                Envelope extent = ahMapControl.Extent;
                if (extent == null) return;
                MapPoint center = extent.GetCenter();

                switch ((sender as Button).Tag.ToString())
                {
                    case "W":
                        ahMapControl.PanTo(new MapPoint(getCenterPoint(_panValue, center.X, extent.XMin), center.Y)); break;
                    case "E":
                        ahMapControl.PanTo(new MapPoint(getCenterPoint(_panValue, center.X, extent.XMax), center.Y)); break;
                    case "N":
                        ahMapControl.PanTo(new MapPoint(center.X, getCenterPoint(_panValue, center.Y, extent.YMax))); break;
                    case "S":
                        ahMapControl.PanTo(new MapPoint(center.X, getCenterPoint(_panValue, center.Y, extent.YMin))); break;
                    case "NE":
                        ahMapControl.PanTo(new MapPoint(getCenterPoint(_panValue, center.X, extent.XMax), getCenterPoint(_panValue, center.Y, extent.YMax))); break;
                    case "SE":
                        ahMapControl.PanTo(new MapPoint(getCenterPoint(_panValue, center.X, extent.XMax), getCenterPoint(_panValue, center.Y, extent.YMin))); break;
                    case "SW":
                        ahMapControl.PanTo(new MapPoint(getCenterPoint(_panValue, center.X, extent.XMin), getCenterPoint(_panValue, center.Y, extent.YMin))); break;
                    case "NW":
                        ahMapControl.PanTo(new MapPoint(getCenterPoint(_panValue, center.X, extent.XMin), getCenterPoint(_panValue, center.Y, extent.YMax))); break;
                    default: break;
                }
            }
        }

        private double getCenterPoint(int panValue, double center, double pt)
        {
            double output = 0;
            for (var i = 0; i < panValue; i++)
            {
                output = (center + pt) / 2;
                pt = output;
            }
            return output;
        }

        //START - WEBR Stability (Enable pan)

        // ~~  Start AG modified 2020/08/19  -->  Enable Pan and also keep center map

        private void ahMapControl_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!isCenterToolActive)
            {
                isMouseLeftButtonDown = true;
                originalMapPoint = ahMapControl.Extent.GetCenter();
                //MapPoint mp = ahMapControl.ScreenToMap(new Point(e.GetPosition(ahMapControl).X, e.GetPosition(ahMapControl).Y));
                originalMouseLeftButtonDownXCoord = e.GetPosition(ahMapControl).X;
                originalMouseLeftButtonDownYCoord = e.GetPosition(ahMapControl).Y;
            }
        }
        private void ahMapControl_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (isCenterToolActive)
            {
                // center map on mouse up location
                MapPoint pannedLocation = ahMapControl.ScreenToMap(e.GetPosition(ahMapControl));
                ahMapControl.PanTo(pannedLocation);
            }
            else
            {
                //  Pan is active
                isMouseLeftButtonDown = false;
            }
        }
        private void ahMapControl_MouseMove(object sender, MouseEventArgs e)
        {
            if (!isCenterToolActive & isMouseLeftButtonDown)
            {
                //Point p = new Point(e.GetPosition(ahMapControl).X, e.GetPosition(ahMapControl).Y);
                //MapPoint mp = ahMapControl.ScreenToMap(p);

                // original
                double xDiff = ahMapControl.Resolution * (originalMouseLeftButtonDownXCoord - e.GetPosition(ahMapControl).X);
                double yDiff = ahMapControl.Resolution * (originalMouseLeftButtonDownYCoord - e.GetPosition(ahMapControl).Y);

                // modified
                //double xDiff = (originalMouseLeftButtonDownXCoord - e.GetPosition(ahMapControl).X);
                //double yDiff = (originalMouseLeftButtonDownYCoord - e.GetPosition(ahMapControl).Y);

                // original 
                MapPoint newPoint = new MapPoint(originalMapPoint.X + xDiff, originalMapPoint.Y - yDiff);

                // modified
                //MapPoint newPoint = new MapPoint(originalMapPoint.X + xDiff, originalMapPoint.Y + yDiff);

                ahMapControl.PanDuration = new TimeSpan(0, 0, 0, 0, 500);
                ahMapControl.PanTo(newPoint);
            }
        }

        // AG modified on 2020/08/19
        private void btnPan_Click(object sender, RoutedEventArgs e)
        {
            isCenterToolActive = false;
            btnCenter.IsEnabled = true;
            btnPan.IsEnabled = false;
        }

        private void btnCenter_Click(object sender, RoutedEventArgs e)
        {
            isCenterToolActive = true;
            btnPan.IsEnabled = true;
            btnCenter.IsEnabled = false;
        }

        // ~~  End AG modified 2020/08/19
        //END - WEBR Stability (Enable pan)

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

