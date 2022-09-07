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
using ESRI.ArcGIS.Client.Tasks;

namespace PageTemplates.Controls
{
    public partial class CMCSPrintWindow : ChildWindow
    {
        private static Map _cmcsMap;
        private ESRI.ArcGIS.Client.Geometry.Envelope _env;
        private PGnE.Printing.HiResPrint _hiResPrint = null;
        private Logger logger = LogManager.GetCurrentClassLogger();
        private static bool _isCMCSMapTypeScaleRatioSet = false;
        private static double _cmcsResolutionRatio = double.NaN;
        private static double _newResolution = double.NaN;
        private static int _dpiToPrintAt;
        private static double _templateScale = double.NaN;
        private string _printJobId;
        private ESRI.ArcGIS.Client.Geometry.MapPoint _centerPoint;
        private double _pageWidth = double.NaN;
        private double _pageHeight = double.NaN;
        private bool _isConfiguredScale = false;
        private double _oldResolution = double.NaN;
        private double _unitConversionFactor;

        private TextSymbol _textSymbol;
        private MapPoint mapPoint;
        private Envelope initialExtent;
        private MapPoint centerPoint;
        private double initialScale;
        private static LineSymbol traceSymbol;

        private string _circuitSourceUrl;
        private string _circuitIDLayerIds;
        private string _circuitIDLayersService;
        private string _mapNoUrl;
        private List<int> _mapNo_LayerId = new List<int>();
        private List<string> _mapNo_LayerName = new List<string>();
        private List<string> _mapNo_LayerField = new List<string>();

        //START - WEBR Stability (Enable pan)  
        private bool isMouseLeftButtonDown;
        private double originalMouseLeftButtonDownXCoord;
        private double originalMouseLeftButtonDownYCoord;
        private MapPoint originalMapPoint;
        //  ~~  Start - AG modified (Enable Pan and Center) 2020/08/25
        private bool isCenterToolActive = true;
        //  ~~  End - AG modified (Enable Pan and Center) 2020/08/25

        #region Properties

        private int _panValue; //INC000004049426 & INC000004413542

        public ESRI.ArcGIS.Client.Geometry.MapPoint CenterPoint
        {
            get { return _centerPoint; }
            set { _centerPoint = value; }
        }

        public Map CMCSMap
        {
            get { return _cmcsMap; }
            set { _cmcsMap = value; }
        }
        public ESRI.ArcGIS.Client.Geometry.Envelope CurrentExtent
        {
            get { return _env; }
            set { _env = value; }
        }
        public string UserEmail { get; set; }
        public string StoredDisplayName { get; set; }
        public XElement MapDimensionsElement { get; set; }

        public string CircuitSourceUrl
        {
            get { return _circuitSourceUrl; }
            set { _circuitSourceUrl = value; }
        }

        public string CircuitIDLayerIds
        {
            get { return _circuitIDLayerIds; }
            set { _circuitIDLayerIds = value; }
        }

        public string CircuitIDLayersService
        {
            get { return _circuitIDLayersService; }
            set { _circuitIDLayersService = value; }
        }

        public string MapNoUrl
        {
            get { return _mapNoUrl; }
            set { _mapNoUrl = value; }
        }

        public List<int> MapNo_LayerIdList
        {
            get { return _mapNo_LayerId; }
            set { _mapNo_LayerId = value; }
        }

        public List<string> MapNo_LayerNameList
        {
            get { return _mapNo_LayerName; }
            set { _mapNo_LayerName = value; }
        }

        public List<string> MapNo_LayerFieldList
        {
            get { return _mapNo_LayerField; }
            set { _mapNo_LayerField = value; }
        }

        #endregion Properties

//        public CMCSPrintWindow()//(bool panVisibility, int panValue)
        public CMCSPrintWindow(bool panVisibility, int panValue)
        {
            InitializeComponent();
            cmcsMapControl.Progress += new EventHandler<ProgressEventArgs>(cmcsMapControl_Progress);
            this.Loaded += new RoutedEventHandler(CMCSPrintWindow_Loaded);

            //INC000004049426 & INC000004413542
            if (panVisibility)
                PanButtons.Visibility = Visibility.Visible;
            else
                PanButtons.Visibility = Visibility.Collapsed;
            _panValue = panValue;
        }

        void cmcsMapControl_ExtentChanged(object sender, ExtentEventArgs e)
        {
            Map mapControl = sender as Map;
            if (mapControl != null)
            {
                mapControl.MinimumResolution = System.Double.Epsilon;
            }

            if (mapControl != null) _oldResolution = mapControl.Resolution;
        }

        void cmcsMapControl_Progress(object sender, ProgressEventArgs e)
        {
            if (e.Progress == 100)
            {
                IsLoading(false);
                cmcsMapControl.Progress -= cmcsMapControl_Progress;
                cmcsComboScaleSelection.SelectedIndex = cmcsComboScaleSelection.Items.Count() - 1;
                cmcsMapControl.MinimumResolution = 0.0000000123;
            }
            else IsLoading(true);
        }

        void cmcsMapControl_Progress1(object sender, ProgressEventArgs e)
        {
            if (e.Progress == 100)
            {
                IsLoading(false);
                cmcsMapControl.Progress -= cmcsMapControl_Progress1;
            }
            else IsLoading(true);
        }

        void CMCSPrintWindow_Loaded(object sender, RoutedEventArgs e)
        {
            IsLoading(true);

            ClearAppResource("MapTypes");
            ClearAppResource("GridLayers");
            ClearAppResource("ScaleOptions");
            ClearAppResource("PrintServiceUrls");
            ClearAppResource("UpdateDelay");

            ReadTemplatesConfig();

            if (_cmcsMap != null)
            {
                if (!double.IsNaN(_cmcsMap.Resolution) && !double.IsNaN(_cmcsMap.Scale))
                {
                    _cmcsResolutionRatio = _cmcsMap.Resolution / _cmcsMap.Scale;

                    if (!double.IsNaN(_cmcsResolutionRatio)) _isCMCSMapTypeScaleRatioSet = true;
                }

                traceSymbol = new ESRI.ArcGIS.Client.Symbols.SimpleLineSymbol
                {
                    Color = new SolidColorBrush(Color.FromArgb(255, 255, 0, 0)),
                    Width = 4,
                    Style = ESRI.ArcGIS.Client.Symbols.SimpleLineSymbol.LineStyle.Solid
                };

                LoadMapLayers(_cmcsMap.Layers);
                cmcsMapControl.Extent = _cmcsMap.Extent;

                //To fix the bug Significant shift in the Print PDF 

                initialExtent = new Envelope();
                centerPoint = new MapPoint();
                centerPoint.X = _cmcsMap.Extent.XMin + (_cmcsMap.Extent.XMax - _cmcsMap.Extent.XMin) / 2;
                centerPoint.Y = _cmcsMap.Extent.YMin + (_cmcsMap.Extent.YMax - _cmcsMap.Extent.YMin) / 2;

                initialExtent.XMin = _cmcsMap.Extent.XMin + (_cmcsMap.Extent.XMax - _cmcsMap.Extent.XMin) / 2 - 200;
                initialExtent.XMax = _cmcsMap.Extent.XMin + (_cmcsMap.Extent.XMax - _cmcsMap.Extent.XMin) / 2 + 200;
                initialExtent.YMin = _cmcsMap.Extent.YMin + (_cmcsMap.Extent.YMax - _cmcsMap.Extent.YMin) / 2 - 200;
                initialExtent.YMax = _cmcsMap.Extent.YMin + (_cmcsMap.Extent.YMax - _cmcsMap.Extent.YMin) / 2 + 200;

                initialScale = _cmcsMap.Resolution * 96 * _unitConversionFactor;

                //ahMapControl.Width = MapPanel.Width;
                //ahMapControl.Height = MapPanel.Height;

                Border border = VisualTreeHelper.GetChild(_cmcsMap, 0) as Border;
                GraphicsLayer _graphicsLayer = new GraphicsLayer();
                _graphicsLayer.ID = "TextGraphic";
                cmcsMapControl.Layers.Add(_graphicsLayer);

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


            if (_isCMCSMapTypeScaleRatioSet)
            {
                PrintCMCSMapButton.IsEnabled = true;
            }
            else
            {
                MessageBox.Show("Error getting Scale ratio from base map.", "Map Scale Ratio / Resolution Error", MessageBoxButton.OK);
                this.CancelButton_Click(sender, e);
            }

            IsLoading(false);
            cmcsComboPageSizeSelection.SelectionChanged += new SelectionChangedEventHandler(comboPageSizeSelection_SelectionChanged);
            cmcsComboScaleSelection.SelectionChanged += new SelectionChangedEventHandler(comboScaleSelection_SelectionChanged);

            //comboScaleSelection.SelectedIndex = comboScaleSelection.Items.Count() - 1;

            cmcsMapControl.SizeChanged += new SizeChangedEventHandler(cmcsMapControl_SizeChanged);

            AutoPopulateFiels();
        }

        private void AutoPopulateFiels()
        {
            PopulateSubstationAndCircuitNo();
            PopulateMapNo();

            if (Application.Current.Host.InitParams.ContainsKey("CurrentUserFullName"))
                txtPreparedBy.Text = Application.Current.Host.InitParams["CurrentUserFullName"].ToString();
            datePickerPublicationDate.SelectedDate = DateTime.Today;
            datePickerOperatorEntryDate.SelectedDate = DateTime.Today;
        }

        private void PopulateSubstationAndCircuitNo()
        {
            try
            {
                //Find circuit ID
                var identifyParameters = new IdentifyParameters();
                identifyParameters.Geometry = CMCSMap.Extent;
                identifyParameters.LayerIds.AddRange(CircuitIDLayerIds.Split(',').Select(int.Parse).ToArray());
                identifyParameters.MapExtent = CurrentExtent;
                identifyParameters.LayerOption = LayerOption.all;
                identifyParameters.Height = (int)CMCSMap.ActualHeight;
                identifyParameters.Width = (int)CMCSMap.ActualWidth;
                identifyParameters.SpatialReference = CMCSMap.SpatialReference;
                identifyParameters.Tolerance = 7;

                var identifyTask = new IdentifyTask(CircuitIDLayersService);
                identifyTask.Failed += new EventHandler<TaskFailedEventArgs>(identifyTaskCircuitID_Failed);
                identifyTask.ExecuteCompleted += new EventHandler<IdentifyEventArgs>(identifyTaskCircuitID_ExecuteCompleted);
                identifyTask.ExecuteAsync(identifyParameters);
            }
            catch (Exception err)
            {
                logger.Error("Failed to find Circuit Point: " + err.Message.ToString());
                txtSubNameCircuitNo.Text = "";
            }
        }

        private void identifyTaskCircuitID_ExecuteCompleted(object sender, IdentifyEventArgs e)
        {
            try
            {
                if (e.IdentifyResults != null)
                {
                    if (e.IdentifyResults.Count > 0)
                    {
                        string circuitId1 = e.IdentifyResults[0].Feature.Attributes["Circuit ID"].ToString();
                        bool populateCircuitNo = true;
                        for (var i = 1; i < e.IdentifyResults.Count; i++)
                        {
                            string circuitId2 = e.IdentifyResults[i].Feature.Attributes["Circuit ID"].ToString();
                            if (circuitId2 != circuitId1)
                            {
                                populateCircuitNo = false;
                                break;
                            }
                            else
                                continue;
                        }

                        if (populateCircuitNo)
                        {
                            //find circuit name and substation name from circuit source table
                            var queryTask = new QueryTask(CircuitSourceUrl);
                            queryTask.ExecuteCompleted += new EventHandler<QueryEventArgs>(queryTaskCircuitSubstationNames_ExecuteCompleted);
                            queryTask.Failed += new EventHandler<TaskFailedEventArgs>(queryTaskCircuitSubstationNames_Failed);
                            Query query = new Query();
                            query.Where = "CIRCUITID='" + circuitId1 + "'";
                            query.OutFields.AddRange(new string[] { "CIRCUITID", "SUBSTATIONNAME" });
                            queryTask.ExecuteAsync(query);
                        }
                        else
                            txtSubNameCircuitNo.Text = "";
                    }
                }
            }
            catch (Exception err)
            {
                logger.Error("Failed to find Circuit Name and Substation Name: " + err.Message.ToString());
                txtSubNameCircuitNo.Text = "";
            }

        }

        private void identifyTaskCircuitID_Failed(object sender, TaskFailedEventArgs e)
        {
            logger.Error("Failed to find Circuit Point: " + e.Error);
            txtSubNameCircuitNo.Text = "";
        }

        private void queryTaskCircuitSubstationNames_ExecuteCompleted(object sender, QueryEventArgs e)
        {
            string subNameAndCircuitNo = "";
            if (e != null)
            {
                if (e.FeatureSet.Features.Count > 0)
                {
                    if (e.FeatureSet.Features[0].Attributes["SUBSTATIONNAME"] != null)
                        subNameAndCircuitNo += e.FeatureSet.Features[0].Attributes["SUBSTATIONNAME"].ToString();
                    if (e.FeatureSet.Features[0].Attributes["CIRCUITID"] != null)
                        subNameAndCircuitNo += " " + e.FeatureSet.Features[0].Attributes["CIRCUITID"].ToString().Substring(e.FeatureSet.Features[0].Attributes["CIRCUITID"].ToString().Length - 4);
                }
            }
            txtSubNameCircuitNo.Text = subNameAndCircuitNo;
        }

        private void queryTaskCircuitSubstationNames_Failed(object sender, TaskFailedEventArgs e)
        {
            logger.Error("Failed to find Circuit Name and Substation Name: " + e.Error);
            txtSubNameCircuitNo.Text = "";
        }

        void cmcsMapControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            try
            {
                if (e.NewSize.Width > e.NewSize.Height)
                {
                    initialExtent.XMax = centerPoint.X + cmcsMapControl.Resolution * e.NewSize.Height / 2;
                    initialExtent.XMin = centerPoint.X - cmcsMapControl.Resolution * e.NewSize.Height / 2;
                    initialExtent.YMin = centerPoint.Y - cmcsMapControl.Resolution * e.NewSize.Height / 2;
                    initialExtent.YMax = centerPoint.Y + cmcsMapControl.Resolution * e.NewSize.Height / 2;
                }
                else
                {
                    initialExtent.XMax = centerPoint.X + cmcsMapControl.Resolution * e.NewSize.Width / 2;
                    initialExtent.XMin = centerPoint.X - cmcsMapControl.Resolution * e.NewSize.Width / 2;
                    initialExtent.YMin = centerPoint.Y - cmcsMapControl.Resolution * e.NewSize.Width / 2;
                    initialExtent.YMax = centerPoint.Y + cmcsMapControl.Resolution * e.NewSize.Width / 2;
                }

                cmcsMapControl.Progress += new EventHandler<ProgressEventArgs>(cmcsMapControl_Progress1);
                cmcsMapControl.ZoomTo(initialExtent);
            }
            catch (Exception)
            {

                throw;
            }
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
                BusyIndicatorCMCS.BusyContent = "Loading Map...";
                BusyIndicatorCMCS.IsBusy = true;
                IsEnabled = false;
            }
            else
            {
                BusyIndicatorCMCS.IsBusy = false;
                IsEnabled = true;
            }
        }

        public void LoadMapLayers(LayerCollection mainMapCollection)
        {


            Layer _newLayer = null;
            cmcsMapControl.Layers = new LayerCollection();

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
                    // remove the commonlandbase from the Schematic ad-hoc print
                    if (StoredDisplayName.ToUpper().Contains("SCHEMATICS"))
                    {
                        if (_newLayer.ID.ToUpper() == "COMMONLANDBASE")
                        {
                            _newLayer.Visible = false;
                        }
                    }

                    cmcsMapControl.Layers.Add(_newLayer);
                    cmcsMapControl.Extent = _env;

                    logger.Info("PRINTING: Adding layer to CMCS map. Layer ID: " + _newLayer.ID);
                }
            }
            cmcsMapControl.UpdateLayout();
            logger.Info("CMCS DESIRED PRINT EXTENT: " + _env);
            logger.Info("CMCS ACTUAL EXTENT: " + cmcsMapControl.Extent);
            if (cmcsMapControl.SpatialReference.WKID == 102100)
            {
                _unitConversionFactor = 39.37 / 12; //meters to feet
            }
            else
            {
                _unitConversionFactor = 1;
            }
        }

        /// <summary>
        /// This method scales the page template viewer map for CMCS maps
        /// on subsequent launches of an CMCS map template.
        /// </summary>
        /// <param name="userSelectedScale"></param>
        private void ScaleCMCSMap(string userSelectedScale)
        {
            if (double.TryParse(userSelectedScale, out _templateScale))
            {
                //we are making a couple assumptions here 1. the dpi is 96 2. the map units are feet
                _newResolution = _templateScale / (96 * _unitConversionFactor);
                cmcsMapControl.ZoomToResolution(_newResolution);

                logger.Info("PRINTING: CMCS Map Zoomed to new resolution. Resolution: " + _newResolution.ToString());
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

            _templates2.Insert(0, "CMCSMap_8.5x11_Landscape");
            int index = 1;
            foreach (string templatename in _templates)
            {
                if (templatename != "CMCSMap_8.5x11_Landscape" && templatename.Contains("CMCSMap"))
                {
                    _templates2.Insert(index, templatename);
                    index++;
                }
            }

            cmcsComboPageSizeSelection.Items.Clear();
            cmcsComboPageSizeSelection.ItemsSource = from t in _templates2
                                                     where (t != "MAP_ONLY")
                                                     select t;

            cmcsComboPageSizeSelection.SelectedIndex = 0;
        }

        private void PopulateMapScaleCombo()
        {
            if (Application.Current.Resources.Contains("ScaleOptions"))
            {
                List<Tuple<string, string>> scaleSelectionItems = (List<Tuple<string, string>>)Application.Current.Resources["ScaleOptions"];

                foreach (var scaleItem in scaleSelectionItems)
                {
                    cmcsComboScaleSelection.Items.Add(new ComboBoxItem { Content = scaleItem.Item1, Tag = scaleItem.Item2 });
                }
            }
            cmcsComboScaleSelection.Items.Add(new ComboBoxItem { Content = "Custom Scale", Tag = _cmcsMap.Scale });
        }

        void comboScaleSelection_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string selectedScaleName = ((ComboBoxItem)cmcsComboScaleSelection.SelectedItem).Content.ToString().ToLower();

            if (selectedScaleName == "custom scale")
            {
                _isConfiguredScale = false;
                //return;
            }
            else
            {
                _isConfiguredScale = true;
            }
            string userSelectedScale = ((ComboBoxItem)cmcsComboScaleSelection.SelectedItem).Tag.ToString();
            if (_isConfiguredScale)
            {
                initialScale = Convert.ToDouble(userSelectedScale);
                ScaleCMCSMap(userSelectedScale);
            }
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

                        _pageWidth = ReadMapDimensions(templateName, "Width");
                        _pageHeight = ReadMapDimensions(templateName, "Height");

                        CalculatePrintArea(_pageWidth, _pageHeight, cmcsMapControl);

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

            logger.Error("Naming convention error of configured CMCS Map Templates." + Environment.NewLine
                + "Required Format: <Name>_<Length>x<Width>_<Portrait or Landscape>");
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            // pass in jobid _hiResPrint.CancelPrint(jobid);
            if (!string.IsNullOrEmpty(_printJobId))
            {
                _hiResPrint.CancelPrint(_printJobId);
                _printJobId = null;
                CancelCMCSButton.Content = "Close";
                EnableDisableControls(true);
                PrintCMCSMapButton.Content = "Print CMCS";
                ResetLayers();
            }
            else
            {
                this.Close();
            }
        }
        private Dictionary<string, double> _minres;
        private void ResetLayers()
        {
            if (_minres != null)
            {
                foreach (Layer l in cmcsMapControl.Layers)
                {
                    if (_minres.ContainsKey(l.ID))
                    {
                        l.MinimumResolution = _minres[l.ID];
                    }
                }
            }
        }
        private void PrintCMCSMapButton_Click(object sender, RoutedEventArgs e)
        {
            _hiResPrint.PrintJobSubmitted += new EventHandler<PrintJobSubmittedEventArgs>(_hiResPrint_PrintJobSubmitted);
            _hiResPrint.PrintJobCompleted += new EventHandler<EventArgs>(_hiResPrint_PrintJobCompleted);

            _hiResPrint.PrintJobCancelling += new EventHandler<EventArgs>(_hiResPrint_PrintJobCancelling);
            _hiResPrint.PrintJobCancelled += new EventHandler<EventArgs>(_hiResPrint_PrintJobCancelled);

            //Convert "." back to "-" to match the actual name of the template to be printed.
            string cmcsTemplateNameOnServer = cmcsComboPageSizeSelection.SelectedItem.ToString().Contains(".") ?
                cmcsComboPageSizeSelection.SelectedItem.ToString().Replace(".", "-") :
                cmcsComboPageSizeSelection.SelectedItem.ToString();
            //Change the minres since ESRI does not convert it right
            _minres = new Dictionary<string, double>();
            foreach (Layer l in cmcsMapControl.Layers)
            {
                double scale = cmcsMapControl.Resolution;
                if (l.MinimumResolution < scale)
                {
                    _minres.Add(l.ID, l.MinimumResolution);
                    l.MinimumResolution = 0;
                }
            }
            Dictionary<string, string> customTextElemets = getCustomTextElements();
            _hiResPrint.Print(cmcsMapControl.Scale, cmcsMapControl, cmcsTemplateNameOnServer, cmcsMapControl.Extent, customTextElemets, Utilites.PrintFormat.PDF, _dpiToPrintAt);

            logger.Info("PRINTING: CMCS print request sent to server.");

            PrintCMCSMapButton.Content = "Generating PDF...";
            CancelCMCSButton.Content = "Cancel Print";
            PrintCMCSMapButton.IsEnabled = false;
            EnableDisableControls(false);
        }

        void _hiResPrint_PrintJobCancelled(object sender, EventArgs e)
        {
            logger.Info("PRINTING: CMCS cancelled.");
        }

        void _hiResPrint_PrintJobCancelling(object sender, EventArgs e)
        {
            //PrintCMCSMapButton.Content = "Print CMCS";
            //PrintCMCSMapButton.IsEnabled = true;
            logger.Info("PRINTING: CMCS cancel sent to server.");
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
            PrintCMCSMapButton.Content = "Print CMCS";
            PrintCMCSMapButton.IsEnabled = true;
            EnableDisableControls(true);

            CancelCMCSButton.Content = "Close";
            _printJobId = string.Empty;

            logger.Info("PRINTING: Print job completed.");
        }

        private void CalculatePrintArea(double pageWidth, double pageHeight, Map map)
        {
            cmcsMapControl.Width = pageWidth * 96;
            cmcsMapControl.Height = pageHeight * 96;
            cmcsMapBorder.Width = (pageWidth * 96) + 20;   //Border distance with map is taken as 20      
        }

        private void ScrollViewer_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            MapPoint pannedLocation = cmcsMapControl.ScreenToMap(e.GetPosition(cmcsMapControl));
            cmcsMapControl.PanTo(pannedLocation);
        }

        private void EnableDisableControls(bool isEnable)
        {
            cmcsMapContainer.IsEnabled = isEnable;
            cmcsComboPageSizeSelection.IsEnabled = isEnable;
            cmcsComboScaleSelection.IsEnabled = isEnable;
            PrintCMCSMapButton.IsEnabled = isEnable;

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

        private void datePickerPublicationDate_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void PopulateMapNo()
        {
            try
            {
                //Find map number
                var identifyParameters = new IdentifyParameters();
                identifyParameters.Geometry = CMCSMap.Extent;
                identifyParameters.LayerIds.AddRange(MapNo_LayerIdList);
                identifyParameters.MapExtent = CurrentExtent;
                identifyParameters.LayerOption = LayerOption.all;
                identifyParameters.Height = (int)CMCSMap.ActualHeight;
                identifyParameters.Width = (int)CMCSMap.ActualWidth;
                identifyParameters.SpatialReference = CMCSMap.SpatialReference;
                identifyParameters.Tolerance = 7;

                var identifyTask = new IdentifyTask(MapNoUrl);
                identifyTask.Failed += new EventHandler<TaskFailedEventArgs>(identifyTaskMapNo_Failed);
                identifyTask.ExecuteCompleted += new EventHandler<IdentifyEventArgs>(identifyTaskMapNo_ExecuteCompleted);
                identifyTask.ExecuteAsync(identifyParameters);
            }
            catch (Exception err)
            {
                logger.Error("Failed to find Map Number: " + err.Message.ToString());
                txtMapNo.Text = "";
            }
        }

        private void identifyTaskMapNo_ExecuteCompleted(object sender, IdentifyEventArgs e)
        {
            try
            {
                if (e.IdentifyResults != null)
                {
                    if (e.IdentifyResults.Count > 0)
                    {
                        for (var i = 0; i < MapNo_LayerNameList.Count; i++)
                        {
                            if (e.IdentifyResults[0].LayerName == MapNo_LayerNameList[i])
                            {
                                txtMapNo.Text = e.IdentifyResults[0].Feature.Attributes[MapNo_LayerFieldList[i]].ToString();
                                break;
                            }
                        }
                    }
                }
            }
            catch (Exception err)
            {
                logger.Error("Failed to find Map Number: " + err.Message.ToString());
                txtMapNo.Text = "";
            }

        }

        private void identifyTaskMapNo_Failed(object sender, TaskFailedEventArgs e)
        {
            logger.Error("Failed to find Map Number: " + e.Error);
            txtMapNo.Text = "";
        }

        private Dictionary<string, string> getCustomTextElements()
        {
            Dictionary<string, string> customElemets = new Dictionary<string, string>();
            customElemets.Add("RecNo", txtRecNo.Text);
            customElemets.Add("PublicationDate", datePickerPublicationDate.Text);
            customElemets.Add("TDNo", txtTDNo.Text);
            customElemets.Add("USEventNo", txtUSEventNo.Text);
            customElemets.Add("GISTagNo", txtGISTagNo.Text);
            customElemets.Add("ControlCenterAOR", txtControlCenterAOR.Text);
            customElemets.Add("SubNameCircuitNo", txtSubNameCircuitNo.Text);
            customElemets.Add("PreparedBy", txtPreparedBy.Text);
            customElemets.Add("EnggApprovalBy", txtEnggApprovalBy.Text);
            customElemets.Add("MapNo", txtMapNo.Text);
            customElemets.Add("PurposeOfWork", txtPurposeOfWork.Text);
            customElemets.Add("JobNo", txtJobNo.Text);
            customElemets.Add("MAT", txtMAT.Text);
            customElemets.Add("Location", txtLocation.Text);
            customElemets.Add("Town", txtTown.Text);
            customElemets.Add("OperatorEntryDate", datePickerOperatorEntryDate.Text);
            customElemets.Add("LOC1", txtLOC1.Text);
            customElemets.Add("EquipNo1", txtEquipNo1.Text);
            customElemets.Add("SerialNo1", txtSerialNo1.Text);
            customElemets.Add("Manuf1", txtManuf1.Text);
            customElemets.Add("DateManuf1", txtDateManuf1.Text);
            customElemets.Add("Location1", txtLocation1.Text);
            customElemets.Add("Town1", txtTown1.Text);
            customElemets.Add("LOC2", txtLOC2.Text);
            customElemets.Add("EquipNo2", txtEquipNo2.Text);
            customElemets.Add("SerialNo2", txtSerialNo2.Text);
            customElemets.Add("Manuf2", txtManuf2.Text);
            customElemets.Add("DateManuf2", txtDateManuf2.Text);
            customElemets.Add("Location2", txtLocation2.Text);
            customElemets.Add("Town2", txtTown2.Text);
            customElemets.Add("LOC3", txtLOC3.Text);
            customElemets.Add("EquipNo3", txtEquipNo3.Text);
            customElemets.Add("SerialNo3", txtSerialNo3.Text);
            customElemets.Add("Manuf3", txtManuf3.Text);
            customElemets.Add("DateManuf3", txtDateManuf3.Text);
            customElemets.Add("Location3", txtLocation3.Text);
            customElemets.Add("Town3", txtTown3.Text);
            customElemets.Add("LOC4", txtLOC4.Text);
            customElemets.Add("EquipNo4", txtEquipNo4.Text);
            customElemets.Add("SerialNo4", txtSerialNo4.Text);
            customElemets.Add("Manuf4", txtManuf4.Text);
            customElemets.Add("DateManuf4", txtDateManuf4.Text);
            customElemets.Add("Location4", txtLocation4.Text);
            customElemets.Add("Town4", txtTown4.Text);
            customElemets.Add("LOC5", txtLOC5.Text);
            customElemets.Add("EquipNo5", txtEquipNo5.Text);
            customElemets.Add("SerialNo5", txtSerialNo5.Text);
            customElemets.Add("Manuf5", txtManuf5.Text);
            customElemets.Add("DateManuf5", txtDateManuf5.Text);
            customElemets.Add("Location5", txtLocation5.Text);
            customElemets.Add("Town5", txtTown5.Text);
            customElemets.Add("WorkCompletedBy", txtWorkCompletedBy.Text);
            customElemets.Add("CompletionDate", datePickerCompletionDate.Text);
            customElemets.Add("RecordedBy", txtRecordedBy.Text);
            customElemets.Add("RecordedDate", datePickerRecordedDate.Text);
            customElemets.Add("MappedBy", txtMappedBy.Text);
            customElemets.Add("MappedDate", datePickerMappedDate.Text);
            customElemets.Add("ForwardTo", txtForwardTo.Text);
            customElemets.Add("ForwardDate", datePickerForwardedDate.Text);
            customElemets.Add("SheetNo", txtSheetNo.Text);
            customElemets.Add("TotalSheetsCount", txtTotalSheetsCount.Text);

            return customElemets;
        }

        private double ReadMapDimensions(string template, string paramter)
        {
            double parameterValue = 0;
            if (MapDimensionsElement.HasElements)
            {
                foreach (XElement childElement in MapDimensionsElement.Elements())
                {
                    if (childElement.Attribute("Name").Value.ToString() == template)
                    {
                        parameterValue = Convert.ToDouble(childElement.Attribute(paramter).Value.ToString());
                    }
                }
            }
            return parameterValue;
        }

        //INC000004049426 and INC000004413542 - enable pan in ad hoc
        private void PanClick(object sender, RoutedEventArgs e)
        {
            if (_panValue > 0)
            {
                Envelope extent = cmcsMapControl.Extent;
                if (extent == null) return;
                MapPoint center = extent.GetCenter();

                switch ((sender as Button).Tag.ToString())
                {
                    case "W":
                        cmcsMapControl.PanTo(new MapPoint(getCenterPoint(_panValue, center.X, extent.XMin), center.Y)); break;
                    case "E":
                        cmcsMapControl.PanTo(new MapPoint(getCenterPoint(_panValue, center.X, extent.XMax), center.Y)); break;
                    case "N":
                        cmcsMapControl.PanTo(new MapPoint(center.X, getCenterPoint(_panValue, center.Y, extent.YMax))); break;
                    case "S":
                        cmcsMapControl.PanTo(new MapPoint(center.X, getCenterPoint(_panValue, center.Y, extent.YMin))); break;
                    case "NE":
                        cmcsMapControl.PanTo(new MapPoint(getCenterPoint(_panValue, center.X, extent.XMax), getCenterPoint(_panValue, center.Y, extent.YMax))); break;
                    case "SE":
                        cmcsMapControl.PanTo(new MapPoint(getCenterPoint(_panValue, center.X, extent.XMax), getCenterPoint(_panValue, center.Y, extent.YMin))); break;
                    case "SW":
                        cmcsMapControl.PanTo(new MapPoint(getCenterPoint(_panValue, center.X, extent.XMin), getCenterPoint(_panValue, center.Y, extent.YMin))); break;
                    case "NW":
                        cmcsMapControl.PanTo(new MapPoint(getCenterPoint(_panValue, center.X, extent.XMin), getCenterPoint(_panValue, center.Y, extent.YMax))); break;
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
        //private void cmcsMapControl_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        //{
        //    isMouseLeftButtonDown = true;
        //    originalMapPoint = cmcsMapControl.Extent.GetCenter();
        //    //MapPoint mp = cmcsMapControl.ScreenToMap(new Point(e.GetPosition(cmcsMapControl).X, e.GetPosition(cmcsMapControl).Y));
        //    originalMouseLeftButtonDownXCoord = e.GetPosition(cmcsMapControl).X;// mp.X;
        //    originalMouseLeftButtonDownYCoord = e.GetPosition(cmcsMapControl).Y;// mp.Y;
        //}
        //private void cmcsMapControl_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        //{
        //    isMouseLeftButtonDown = false;
        //}
        //private void cmcsMapControl_MouseMove(object sender, MouseEventArgs e)
        //{
        //    if (isMouseLeftButtonDown)
        //    {
        //        //Point p = new Point(e.GetPosition(cmcsMapControl).X, e.GetPosition(cmcsMapControl).Y);
        //        //MapPoint mp = cmcsMapControl.ScreenToMap(p);
        //        double xDiff = cmcsMapControl.Resolution * (originalMouseLeftButtonDownXCoord - e.GetPosition(cmcsMapControl).X); // originalMouseLeftButtonDownXCoord - mp.X;
        //        double yDiff = cmcsMapControl.Resolution * (originalMouseLeftButtonDownYCoord - e.GetPosition(cmcsMapControl).Y); // originalMouseLeftButtonDownYCoord - mp.Y;

        //        MapPoint newPoint = new MapPoint(originalMapPoint.X + xDiff, originalMapPoint.Y - yDiff);

        //        cmcsMapControl.PanDuration = new TimeSpan(0, 0, 0, 0, 500);
        //        cmcsMapControl.PanTo(newPoint);
        //    }
        //}
        //END - WEBR Stability (Enable pan)


        // BEGIN AG modified on 2020/08/25

        //START - WEBR Stability (Enable pan)
        private void cmcsMapControl_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!isCenterToolActive)
            {
                isMouseLeftButtonDown = true;
                originalMapPoint = cmcsMapControl.Extent.GetCenter();
                //MapPoint mp = cmcsMapControl.ScreenToMap(new Point(e.GetPosition(cmcsMapControl).X, e.GetPosition(cmcsMapControl).Y));
                originalMouseLeftButtonDownXCoord = e.GetPosition(cmcsMapControl).X;// mp.X;
                originalMouseLeftButtonDownYCoord = e.GetPosition(cmcsMapControl).Y;// mp.Y;
            }
        }

        private void cmcsMapControl_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (isCenterToolActive)
            {
                // center map on mouse up location
                MapPoint pannedLocation = cmcsMapControl.ScreenToMap(e.GetPosition(cmcsMapControl));
                cmcsMapControl.PanTo(pannedLocation);
            }
            else
            {
                //  Pan is active
                isMouseLeftButtonDown = false;
            }
        }

        private void cmcsMapControl_MouseMove(object sender, MouseEventArgs e)
        {
            if (!isCenterToolActive & isMouseLeftButtonDown)
            {
                //Point p = new Point(e.GetPosition(cmcsMapControl).X, e.GetPosition(cmcsMapControl).Y);
                //MapPoint mp = cmcsMapControl.ScreenToMap(p);
                double xDiff = cmcsMapControl.Resolution * (originalMouseLeftButtonDownXCoord - e.GetPosition(cmcsMapControl).X); // originalMouseLeftButtonDownXCoord - mp.X;
                double yDiff = cmcsMapControl.Resolution * (originalMouseLeftButtonDownYCoord - e.GetPosition(cmcsMapControl).Y); // originalMouseLeftButtonDownYCoord - mp.Y;

                MapPoint newPoint = new MapPoint(originalMapPoint.X + xDiff, originalMapPoint.Y - yDiff);

                cmcsMapControl.PanDuration = new TimeSpan(0, 0, 0, 0, 500);
                cmcsMapControl.PanTo(newPoint);
            }
        }
        //END - WEBR Stability (Enable pan)


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

        // ~~  END AG modified 2020/08/25
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

