using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO.IsolatedStorage;
using System.Linq;
using System.ServiceModel.Channels;
using System.Windows;
using System.Windows.Browser;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Printing;
using System.Xml.Linq;
using System.Net;
using System.Json;
using System.Windows.Media.Imaging;
using System.Windows.Data;
using ArcFMSilverlight.Butterfly;
using ArcFMSilverlight.Controls.Butterfly;
using ArcFMSilverlight.Controls.DataGridContextMenu;
using ArcFMSilverlight.Controls.DeviceSettings;
using ArcFMSilverlight.Controls.Legend;
using ArcFMSilverlight.Controls.OutageHistory;
using ArcFMSilverlight.Controls.ShowRolloverInfo;
using ArcFMSilverlight.Controls.Tlm;
using ArcFMSilverlight.ViewModels;
using ESRI.ArcGIS.Client;
using ESRI.ArcGIS.Client.Bing.RouteService;
using ESRI.ArcGIS.Client.FeatureService;
using ESRI.ArcGIS.Client.Toolkit;
using ESRI.ArcGIS.Client.Toolkit.Primitives;
using ESRI.ArcGIS.Client.Toolkit.DataSources;
using ESRI.ArcGIS.Client.WebMap;
using ESRI.ArcGIS.Client.Geometry;
using ESRI.ArcGIS.Client.Tasks;
using ESRI.ArcGIS.Client.FeatureService.Symbols;
using Microsoft.Practices.Prism.Modularity;
using Miner.Server.Client.Tasks;
using NLog.LayoutRenderers.Wrappers;
using ESRISymbols = ESRI.ArcGIS.Client.Symbols;

using Miner.Server.Client;
using Miner.Server.Client.Query;
using Miner.Server.Client.Toolkit;
using Miner.Silverlight.Logging.Client;

using NLog;

using ArcFM.Silverlight.PGE.CustomTools;
using PageTemplates;
using System.ServiceModel.DomainServices.Client.ApplicationServices;
using System.Windows.Controls.Primitives;
using CodedValueDomain = ESRI.ArcGIS.Client.FeatureService.CodedValueDomain;
using Domain = ESRI.ArcGIS.Client.FeatureService.Domain;
using ArcFMSilverlight.Controls.Tracing;
using System.Text.RegularExpressions;

using ESRI.SilverlightViewer.Controls;
//*************ENOS2EDGIS Start***************
using ArcFMSilverlight.Controls.Generation;
using System.ServiceModel;
//************ENOS2EDGIS End*******************
/**********************ENOSChange Start********************/
using ArcFMSilverlight.Controls.GenerationOnTransformer;
using ArcFMSilverlight.Controls.GenerationOnFeeder;
/**********************ENOSChange end********************/
using System.Security;


namespace ArcFMSilverlight
{
    public partial class MainPage
    {
        private Dictionary<string, List<Tuple<string, string, string>>> _proposedLayerIdConversionDictionary = new Dictionary<string, List<Tuple<string, string,

string>>>();
        private Dictionary<string, List<Tuple<string, string, string>>> _differentiatingRulesDictionary = new Dictionary<string, List<Tuple<string, string, string>>>

();
        private PageTemplateViewer _pageTemplateViewer;
        private List<string> _badLayers = new List<string>();
        private SelectionOption _selectionType;
        private CADExportTool _cadExportTool;
        private LegendTool _legendTool;
        private XElement _legendConfig;
        private XElement _butterflyConfig;
        private ButterflyTool _butterflyTool;
        private GraphicsLayer _pointLayer;
        private GraphicsLayer _textLayer;
        private GraphicsLayer _saveTextLayer;
        private GraphicsLayer _savePointLayer;
        private double _resolution = 1;
        private double _unscaledMarkerSize = 10;
        private bool _pageTemplateViewerClosed;
        private bool _electricTraceEnabledBeforeExport = false;
        private bool _gasTraceEnabledBeforeExport = false;
        private bool _waterTraceEnabledBeforeExport = false;
        private bool _waitingForWebmap = false;
        private bool _usingWebmap = false;
        private bool _doShowRollover = false;
        private XElement _editorConfig;
        private Dictionary<string, int> _webMapIndices = new Dictionary<string, int>(); // Indices where webmap layers should be inserted into MapControl.Layers
        private List<WebmapPasswordPrompt> _webMapPrompts = new List<WebmapPasswordPrompt>();
        private bool _pageLoaded = false;
        private bool _enableSaveToDB;
        private bool _traceBuferStartup = true;
        private CADExportControlParameters _cadExportParameters = new CADExportControlParameters();
        private DelineationPrintTool _delineationPrintTool;
        private DelineationPrintControlParameters _delineationPrintParameters = new DelineationPrintControlParameters();
        private const string TextLayerID = "TextTemporary";
        private const string BufferLayerID = "BufferLayer";
        private const string InsetMapMarkerID = "InsetMapMarker";
        private int _windowCount;
        static public int _rightClickExtentWidth;

        private string _layerExtent = string.Empty;
        private SpatialReference _baseMapSpatialReference;
        private SpatialReference _extentSpatialReference;

        private const double _meterstoinches = 39.3700787;
        private Dictionary<string, double> _mapUnitsToInches = new Dictionary<string, double>()
                                                                   {
                                                                       {"esriMillimeters", _meterstoinches / 1000.0},
                                                                       {"esriCentimeters", _meterstoinches / 100.0},
                                                                       {"esriDecimeters", _meterstoinches / 10.0},
                                                                       {"esriMeters", _meterstoinches},
                                                                       {"esriKilometers", _meterstoinches * 1000},
                                                                       {"esriInches", 1.0},
                                                                       {"esriFeet", 12.0},
                                                                       {"esriYards", 36.0},
                                                                       {"esriMiles", 63360.0},
                                                                       {"esriNauticalMiles", 72913.4}
                                                                   };

        private Logger logger = LogManager.GetCurrentClassLogger();

        //This list will define what layers should have an objectclassID to layerID mapping built on load
        private Dictionary<string, bool> ObjIDToLayerIDMapToBuild = new Dictionary<string, bool>();

        private Dictionary<string, string> _storedViews = new Dictionary<string, string>();

        private IsolatedStorageSettings appSettings = IsolatedStorageSettings.ApplicationSettings;
        private ApplicationCacheManager _applicationCacheManager = new ApplicationCacheManager();
        private string _wipLabelLayerName = string.Empty;
        private string _wipPolygonLayerName = string.Empty;
        private List<string> _wipLabelFieldNames = new List<string>();
        private string _notesLayerName = string.Empty;  //INC000003946726
        private List<string> _notesLabelFieldNames = new List<string>();  //INC000003946726
        private DataGridContextMenu _dataGridContextMenu = null;
        //private IList<SearchItem> _searchItems = null;
        private DeviceSettings _deviceSettings = new DeviceSettings();
        private Tlm _tlm = new Tlm();
        private DataGridCustomButtonManager _dataGridCustomButtonManager = new DataGridCustomButtonManager();

        private string _defaultStoredView = string.Empty;
        private int _loadedServiceLayerCount = 0;
        private Object _loadedServiceLayerLock = new Object();
        private ShowRolloverInfo _showRolloverInfo;

        private int layerSearchCount = 0;
        private List<TaskBase> CurrentlyExecutingTasks = new List<TaskBase>();
        private ObservableCollection<IResultSet> _newResultSets = new ObservableCollection<IResultSet>();
        private ObservableCollection<IResultSet> identifyobservables = new ObservableCollection<IResultSet>();
        MapPoint environmentLocation;

        private bool isNoGeographicalResult = false;//INC000004022645: S2NN, October 15,2015
        //Adding PONS Tool Variable
        private PONSDialogTool _ponsDialogTool;


        //********************ENOS2EDGIS Start********************/
        // ENOSTOEDGIS - Changes for adding CGC to caption 
        private string _trCGC;
        //ENOS2EDGIS,Added new variables to know whether its a service point search or not
        private bool _IsThisSPpSearch = false;
        private string _SPID = string.Empty;
        //ENOS2EDGIS,Added a new variable to get Query text string from the Search box.
        private const string LocateSetting = "Locator";
        //ENOS2EDGIS
        private const string FC_SERVICELOCATION = "Service Location";
        private Dictionary<string, KeyValuePair<string, string>> _graphicsMoreInfo = new Dictionary<string, KeyValuePair<string, string>>();
        GraphicsLayer _currentGraphicLayer = null;
        string _currentGraphicLayerName = string.Empty;
        Graphic _hoveredGraphic;
        //********************ENOS2EDGIS End********************/

        private Dictionary<string, string> escToPanProperties = new Dictionary<string, string>();  //INC000003956225

        /********************PLC Changes********************/
        private string sapEuipIdQueryString = string.Empty;
        private string sapLatQueryString = string.Empty;
        private string sapLongQueryString = string.Empty;
        private string outageIdQueryString = string.Empty;
        private bool zoomToLocationFlag = true;
        //private string sapPmOrderQueryString = string.Empty;
        /********************PLC Changes Ends********************/

        //INC000004403856
        private string _cmcsCircuitIDLayersService = string.Empty;
        private string _cmcsCircuitIDLayerIds = string.Empty;
        private string _cmcsCircuitSourceUrl = string.Empty;
        private List<int> _cmcsMapNo_LayerId = new List<int>();
        private List<string> _cmcsMapNo_LayerName = new List<string>();
        private List<string> _cmcsMapNo_LayerField = new List<string>();
        private string _cmcsMapNumberUrl = string.Empty;
        private XElement _cmcsMapDimensions;

        /******************ENOSChange Start********************/
        private string _servicePointURL;
        private string _generationInfoURL;
        private string _serviceLocationURL;
        private GenOnTransformerTool genOnTransformer;
        private Grid _mapArea = null;
        private List<Graphic> generations = new List<Graphic>();
        private List<Graphic> servicePointsOnTransformer = new List<Graphic>();
        private int genrationQueryCounter = 0;
        private int servicePointFeatureCount;
        private ENOSFEEDERTool _enosFeederDialogTool;
        private IList<string> _genFeederLayerName = new List<string>();
        /******************ENOSChange End********************/

        //Adding StandardPrint
        private StandardMapPrintTool _StandardMapPrintTool;
        //INC000004049426 & INC000004413542
        private bool _adHocPanVisibility;
        private int _panValue = 0;

        //INC000004479909
        private AddRemoveDataTool _addRemoveDataTool;
        private XElement _addDataConfig;

        //INC000004230808
        private bool _isOpenedFrmEDERWEBR = false;
        private double _scaleOpenedFrmEDERWEBR;

        //INC000005346851 - Share current URL
        private string XQueryString = string.Empty;
        private string YQueryString = string.Empty;
        //CIT Variables *start
        public static int _currentOBJECTID = 0;
        public static string _currentGLOBALID = string.Empty;
        public static string _currentCircuitName = string.Empty;
        public static bool engineers_group = false;
        public static string FilledDuctValue = string.Empty;
        private IList list_Items = null;
        private List<string> values_menu = new List<string>();
        private IList list_Items_new = null;
        private List<string> values_menu_new = new List<string>();
        private Conductor_in_Trench _Conductor_in_TrenchTool;
        public static string _circuitname_CIT = string.Empty;
        //CIT Variables *end


        //banner variables start

        private string isBannerEnabled = string.Empty;
        private string bannerType = string.Empty;
        private string bannerStaticMessage = string.Empty;
        private string bannerDynamicMessage = string.Empty;
        private string bannerServiceUrl = string.Empty;

        //banner variables end

        //DA #190905 - ME Q1 2020 --- START
        private string LIDARCorStoredDisplayName;
        private int LIDARCorLayerId;
        private bool ToggleLIDARCorrCheckedUnchecked = false;
        //DA #190905 - ME Q1 2020 --- END


        //DA# 200103 ME Q1 2020
        private Visibility JETToolVisibility = System.Windows.Visibility.Collapsed;

        //DA# 191201 ME Q1 2020 - START
        private POLContactInfo _POLContactInfo;
        private XElement _POLConfig;
        //DA# 191201 ME Q1 2020 - END

        public MainPage()
        {

            StoreUserDetails();
            logger.Info("Application Started");

            InitializeComponent();
            Loaded += MainPage_Loaded;

            /********************PLC Changes********************/
            if (HtmlPage.Document.QueryString.Count > 0 && HtmlPage.Document != null)
            {
                if (HtmlPage.Document.QueryString.ContainsKey("SAPEQUIPID"))
                {
                    sapEuipIdQueryString = HtmlPage.Document.QueryString["SAPEQUIPID"];
                }
                else if (HtmlPage.Document.QueryString.ContainsKey("LAT") && HtmlPage.Document.QueryString.ContainsKey("LONG"))
                {
                    sapLatQueryString = HtmlPage.Document.QueryString["LAT"];
                    sapLongQueryString = HtmlPage.Document.QueryString["LONG"];
                }
                else if (HtmlPage.Document.QueryString.ContainsKey("OUTAGEID"))
                {
                    outageIdQueryString = HtmlPage.Document.QueryString["OUTAGEID"];
                }

                //INC000005346851
                else if (HtmlPage.Document.QueryString.ContainsKey("X") && HtmlPage.Document.QueryString.ContainsKey("Y"))
                {
                    XQueryString = HtmlPage.Document.QueryString["X"];
                    YQueryString = HtmlPage.Document.QueryString["Y"];
                }

                //INC000004230808
                if (HtmlPage.Document.QueryString.ContainsKey("SCALE"))
                {
                    _isOpenedFrmEDERWEBR = true;
                    try
                    {
                        _scaleOpenedFrmEDERWEBR = Convert.ToDouble(HtmlPage.Document.QueryString["SCALE"].ToString());
                    }
                    catch
                    {
                        _isOpenedFrmEDERWEBR = false;
                    }
                }
            }
            /********************PLC Changes Ends********************/

            if (!DesignerProperties.IsInDesignTool)
            {
                GetShowRollover();
                _applicationCacheManager.DeleteApplicationStorageMain();
                LoadConfiguration();
                //_applicationCacheManager.CheckApplicationStorageVersion();
                MapControl.Progress += new EventHandler<ProgressEventArgs>(MapControl_Progress);

                MEFHelper.CreateCatalog();
                MEFHelper.Composable.Add(new ComposeExportToExcel());
                MEFHelper.Composable.Add(new ComposePageTemplates());
                MEFHelper.Initialize();

                // Need to set this here so Text shows up when page first loads.
                // Otherwise it shows up when editor is first viewed.
                MapControl.Layers.CollectionChanged += Layers_CollectionChanged;
                Editor.Map = MapControl;
                Editor.Rotator = AngleRotator;

                //Wip Editor
                WIPEditor.MapProperty = MapControl;
                WIPEditor.StatusBar = StatusBar;
                ConfigUtility.StatusBar = StatusBar;
                //*****************PLC Changes  RK 07/21/2017*********************//
                //PLC
                WIPEditor.PLC.MapProperty = MapControl;

                //*****************PLC Changes End*********************//
                //Notes
                Notes.MapProperty = MapControl;
                Notes.StatusBar = StatusBar;

                //Binding was not properly working, setting Map here
                CoordinatesTool.Map = MapControl;
                SAPRWTool.Map = MapControl;
                MeasureTool.Map = MapControl;
                //*****************PLC Changes  RK 07/21/2017*********************//
                WIPEditor.PLC.MeasureTool = MeasureTool;
                MeasureTool.PLCWidgetObj = WIPEditor.PLC;

                //*****************PLC Changes End*********************//
                _cadExportTool = new CADExportTool(CADExportToggleButton, MapControl, MeasureTool.GeometryService, this.MapArea, this.Tools);
                _cadExportParameters.GeometryService = MeasureTool.GeometryService;

                _cadExportTool.CADExportControl.Initialize(_cadExportParameters);
                _cadExportTool.PreviousControl = Tools.ViewModel.EnablePan;
                //CIT
                _Conductor_in_TrenchTool = new Conductor_in_Trench(this.MapArea, MapControl);

                //_delineationPrintTool = new DelineationPrintTool(GenerateDelineationPDFButton, MapControl, MeasureTool.GeometryService, this.MapArea, this.Tools);
                _delineationPrintTool = new DelineationPrintTool(DelineationPrintButton, MapControl, MeasureTool.GeometryService, this.MapArea, this.Tools);
                _delineationPrintParameters.GeometryService = MeasureTool.GeometryService;
                _delineationPrintTool.DelineationPrintControl.Initialize(_delineationPrintParameters);
                _delineationPrintTool.PreviousControl = Tools.ViewModel.EnablePan;
                DelineationPrintButton.IsEnabled = false;

                //PONS Tool 
                _ponsDialogTool = new PONSDialogTool(NotificationToggleButton, MapControl, this.MapArea);
                //For PONS Ad Hoc Print - Added EDMaster layers
                if (_storedViews.Keys.Contains("ED EDMaster"))
                    _ponsDialogTool.PONSDialogControl.EDMasterLayers = _storedViews["ED EDMaster"].ToString();

                //INC000004479909
                _addRemoveDataTool = new AddRemoveDataTool(AddDataToggleButton, RemoveDataToggleButton, MapControl, this.MapArea, _addDataConfig, Tools);

                _StandardMapPrintTool = new StandardMapPrintTool(StandardPrintToggleButton, MapControl, MeasureTool.GeometryService, this.MapArea, this.Tools);
                //For PONS Ad Hoc Print - Added EDMaster layers
                if (_storedViews.Keys.Contains("ED EDMaster"))
                    _ponsDialogTool.PONSDialogControl.EDMasterLayers = _storedViews["ED EDMaster"].ToString();

                _legendTool = new LegendTool(MapControl, this.MapArea, Tools.LegendToggleControl.LegendToggleButton, _legendConfig);
                Tools.LegendToggleControl.Initialize(_legendTool);
                _butterflyTool = new ButterflyTool(MapControl, MapArea, _butterflyConfig);


                //ENOSChange, ENOSFeeder Tool 
                _enosFeederDialogTool = new ENOSFEEDERTool(GenFeederToggleButton, MapControl, MeasureTool.GeometryService, this.MapArea, this.Tools, _deviceSettings.DeviceSettingURL);

                //DA# 191201 ME Q1 2020 - START
                _POLContactInfo = new POLContactInfo(POLToggleButton, MapControl, _POLConfig);
                //DA# 191201 ME Q1 2020 - END

                //Traces
                SetTraceMaps();
                MapArea.SizeChanged += new SizeChangedEventHandler(TraceResultsPopup_AdjustSize);

                Tools.MapInset.ViewModel.RetrievingResults += Control_Working;
                Tools.MapInset.ViewModel.RetrievedResults += Control_WorkCompletedNoZoom;

                //DA #190905 - ME Q1 2020 --- START
                Tools.MapInset.ViewModel.LIDARCorStoredDisplay = LIDARCorStoredDisplayName;
                Tools.MapInset.ViewModel.LIDARCorLayerID = LIDARCorLayerId;
                Tools.MapInset.ViewModel.LIDARCorVisiblilityChangedEvnt += new System.EventHandler<IsVisibleEventArgs>(LIDARCorVisiblilityChanged);
                //DA #190905 - ME Q1 2020 --- END

                MapControl.ExtentChanging += MapControlExtentChanging;
                MapControl.ExtentChanged += MapControlExtentChanged;
                
                SetLayersEventHandlers();

                SetEditingTabVisibility();

                //Export to Excel
                AttributesViewer.ExportToExcelStarted += AttributesViewer_ExportToExcelStarted;
                AttributesViewer.ExportToExcelFinished += AttributesViewer_ExportToExcelFinished;
                AttributesViewer.UserLanID = WebContext.Current.User.Name.Replace("PGE\\", "").ToString();
                AttributesViewer.StatusBar = StatusBar;
                AttributesViewer.InitializeFieldSequenceFeatureLayer();

                ColorPicker.ItemsSource = new SolidColorBrush[]
                {
                    new SolidColorBrush(Colors.LightGray),
                    new SolidColorBrush(Colors.Gray),
                    new SolidColorBrush(Colors.DarkGray),
                    new SolidColorBrush(Colors.Black),
                    new SolidColorBrush(Colors.White),
                    new SolidColorBrush(Colors.Red),
                    new SolidColorBrush(Colors.Brown),
                    new SolidColorBrush(Colors.Orange),
                    new SolidColorBrush(Colors.Yellow),
                    new SolidColorBrush(Colors.Green),
                    new SolidColorBrush(Colors.Blue),
                    new SolidColorBrush(Colors.Cyan),
                    new SolidColorBrush(Colors.Purple),
                    new SolidColorBrush(Colors.Magenta)
                };
                FlashColorPicker.ItemsSource = new SolidColorBrush[]
                {
                    new SolidColorBrush(Colors.LightGray),
                    new SolidColorBrush(Colors.Gray),
                    new SolidColorBrush(Colors.DarkGray),
                    new SolidColorBrush(Colors.Black),
                    new SolidColorBrush(Colors.White),
                    new SolidColorBrush(Colors.Red),
                    new SolidColorBrush(Colors.Brown),
                    new SolidColorBrush(Colors.Orange),
                    new SolidColorBrush(Colors.Yellow),
                    new SolidColorBrush(Colors.Green),
                    new SolidColorBrush(Colors.Blue),
                    new SolidColorBrush(Colors.Cyan),
                    new SolidColorBrush(Colors.Purple),
                    new SolidColorBrush(Colors.Magenta)
                };
                FlowAnimationColorPicker.ItemsSource = new SolidColorBrush[]
                {
                    new SolidColorBrush(Colors.LightGray),
                    new SolidColorBrush(Colors.Gray),
                    new SolidColorBrush(Colors.DarkGray),
                    new SolidColorBrush(Colors.Black),
                    new SolidColorBrush(Colors.White),
                    new SolidColorBrush(Colors.Red),
                    new SolidColorBrush(Colors.Brown),
                    new SolidColorBrush(Colors.Orange),
                    new SolidColorBrush(Colors.Yellow),
                    new SolidColorBrush(Colors.Green),
                    new SolidColorBrush(Colors.Blue),
                    new SolidColorBrush(Colors.Cyan),
                    new SolidColorBrush(Colors.Purple),
                    new SolidColorBrush(Colors.Magenta)
                };


                // Trigger the below event to check the search Type. Address Search

                Locate.Loaded += new RoutedEventHandler(Locate_Loaded);
                MapControl.MouseRightButtonDown += new MouseButtonEventHandler(MapControl_MouseRightButtonDown);
                MapControl.MouseRightButtonUp += new MouseButtonEventHandler(MapControl_MouseRightButtonUp);
                _contextMenu = new ContextMenu();
                var menuItemEnvironment = new MenuItem() { Header = "Environment" };
                menuItemEnvironment.Click += new RoutedEventHandler(menuItemEnvironment_Click);
                _contextMenu.Items.Add(menuItemEnvironment);
                ContextMenuService.SetContextMenu(MapControl, _contextMenu);
                var menuItemIdentify = new MenuItem() { Header = "Identify" };
                menuItemIdentify.Click += new RoutedEventHandler(menuItemIdentify_Click);
                _contextMenu.Items.Add(menuItemIdentify);
                ContextMenuService.SetContextMenu(MapControl, _contextMenu);

                //*****************ENOS2EDGISAdded Start************************************
                //ENOS2EDGIS,Added 'Environment' & 'Indentify' menu items for graphics right click menu,In silverlight, as we can't add a child to another parent
                //who was already added to another parent, we created new variables 
                _graphicLayerContextMenu = new ContextMenu();
                var menuItemEnvironmentForGraphic = new MenuItem() { Header = "Environment" };
                menuItemEnvironmentForGraphic.Click += new RoutedEventHandler(menuItemEnvironment_Click);
                _graphicLayerContextMenu.Items.Add(menuItemEnvironmentForGraphic);
                ContextMenuService.SetContextMenu(MapControl, _contextMenu);
                var menuItemIdentifyForGraphic = new MenuItem() { Header = "Identify" };
                menuItemIdentifyForGraphic.Click += new RoutedEventHandler(menuItemIdentify_Click);
                _graphicLayerContextMenu.Items.Add(menuItemIdentifyForGraphic);

                //*****************ENOS2EDGISAdded End**************************************

                this.KeyDown += new KeyEventHandler(Key_Down);  //INC000003956225
            }


            // exceute banner method
            showBannerMessage();
        }

        ///
        ///Code added to display banner message
        ///
        private void showBannerMessage()
        {
            if (isBannerEnabled != null)
            {
                if (isBannerEnabled.ToUpper() == "TRUE")
                {
                    bannerMessage.Visibility = Visibility.Visible;

                    if (bannerType != null)
                    {
                        if (bannerType.ToUpper() == "STATIC")
                        {
                            // show static message
                            if (bannerStaticMessage != null)
                                bannerMessage.Content = bannerStaticMessage;

                        }
                        else if (bannerType.ToUpper() == "DYNAMIC")
                        {
                            if (Application.Current.Host.InitParams.ContainsKey("GisDataDate") && (Application.Current.Host.InitParams["GisDataDate"] != string.Empty))
                            {
                                string date = Application.Current.Host.InitParams["GisDataDate"];
                                bannerMessage.Content = bannerDynamicMessage + date;
                            }
                            else
                            {
                                bannerMessage.Visibility = Visibility.Collapsed;

                            }


                        }
                    }
                }
                else if (isBannerEnabled == "FALSE")
                {
                    bannerMessage.Visibility = Visibility.Collapsed;
                }
            }

        }


        void populateBannerProperties(XElement element)
        {
            if (element != null)
            {

                if (element.Attribute("isEnabled") != null)
                    isBannerEnabled = element.Attribute("isEnabled").Value;

                if (element.Attribute("type") != null)
                    bannerType = element.Attribute("type").Value;

                if (element.Attribute("staticMessage") != null)
                    bannerStaticMessage = element.Attribute("staticMessage").Value;

                if (element.Attribute("dynamicMessage") != null)
                    bannerDynamicMessage = element.Attribute("dynamicMessage").Value;

                if (element.Attribute("WCFService_URL") != null)
                    bannerServiceUrl = element.Attribute("WCFService_URL").Value;

            }
        }






        //code end for banner message
        //INC000003956225 - Press Escape (ESC) button to switch to the Pan tool
        void Key_Down(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                if (IfEscapeToPanActive())
                {
                    Tools.ViewModel.IsActive = true;
                    Tools.ViewModel.EnablePan();
                    Console.WriteLine("testing 123");
                }
            }
        }


        void menuItemIdentify_Click(object sender, RoutedEventArgs e)
        {
            Envelope env = new Envelope(environmentLocation.X - _rightClickExtentWidth, environmentLocation.Y - _rightClickExtentWidth,
                environmentLocation.X + _rightClickExtentWidth, environmentLocation.Y + _rightClickExtentWidth);

            Identify.Identify(env);
        }

        void MapControl_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            environmentLocation = MapControl.ScreenToMap(e.GetPosition(MapControl));
            ContextMenuService.SetContextMenu((DependencyObject)sender, _contextMenu);
        }

        void menuItemEnvironment_Click(object sender, RoutedEventArgs e)
        {
            EnvironmentalInfo.environementInfo.getEnvironmentalInformation(environmentLocation, MapControl);
        }

        //CIT method for identify tool
        void menuItemDuctFilled_Click(object sender, RoutedEventArgs e)
        {
            IList<int> list_ducts = new List<int>();
            try
            {
                //Conductor_in_Trench duct_object = new Conductor_in_Trench(this.MapArea, MapControl);
                _Conductor_in_TrenchTool.comboDuctFilledSelection.ItemsSource = _Conductor_in_TrenchTool.fillDuctValue();
                _Conductor_in_TrenchTool.OpenDialog(this.MapArea, MapControl, _Conductor_in_TrenchTool);
            }
            catch (Exception ee)
            {
                throw ee;
            }
        }

        ContextMenu _contextMenu = null;
        //*************ENOS2EDGIS Start***************
        ContextMenu _graphicLayerContextMenu = null;
        //**************ENOS2EDGIS End*****************
        void MapControl_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            //throw new NotImplementedException();
            environmentLocation = MapControl.ScreenToMap(e.GetPosition(MapControl));
            ContextMenuService.SetContextMenu((DependencyObject)sender, _contextMenu);
        }

        void TraceResultsPopup_AdjustSize(object sender, SizeChangedEventArgs e)
        {
            try
            {
                double adjustment = (LayoutRoot.ActualHeight - AttributeGridContainer.ActualHeight
                    - RibbonLayoutPanel.ActualHeight - LayerRootGridZero.ActualHeight - LayerRootGridOne.ActualHeight) * 0.2;
                if (adjustment < 42) { adjustment = 42; }

                double maxHeight = (LayoutRoot.ActualHeight - AttributeGridContainer.ActualHeight
                    - RibbonLayoutPanel.ActualHeight - LayerRootGridZero.ActualHeight - LayerRootGridOne.ActualHeight) - adjustment;

                TraceResultsList.MaxHeight = maxHeight;
            }
            catch (Exception ex) { }

            try
            {
                ShowLayersToInclude.Margin = ShowLayersToInclude.Margin;
            }
            catch (Exception ex) { }
        }

        /// <summary>
        /// Determines the domain mapping for all fields within the publication map service
        /// </summary>
        /// <param name="featLayerInfoDict"></param>
        /// <param name="e"></param>
        private void GetPublicationDomainMapping(IDictionary<int, FeatureLayerInfo> featLayerInfoDict, Exception e)
        {
            foreach (KeyValuePair<int, FeatureLayerInfo> kvp in featLayerInfoDict)
            {
                FeatureLayerInfo layerInfo = kvp.Value;

                //Get relationships information
                ConfigUtility.AddRelationships(TracingHelper.PGEElectricTracingURL, layerInfo.Id, layerInfo.Relationships.ToList());

                //Get domain information for feature classes with and without subtypes.
                if (layerInfo.FeatureTypes != null)
                {
                    foreach (KeyValuePair<object, FeatureType> kvp2 in layerInfo.FeatureTypes)
                    {
                        foreach (KeyValuePair<string, Domain> kvp3 in kvp2.Value.Domains)
                        {
                            if (kvp3.Value is CodedValueDomain)
                            {
                                CodedValueDomain codedValueDomain = kvp3.Value as CodedValueDomain;
                                foreach (KeyValuePair<object, string> kvp4 in codedValueDomain.CodedValues)
                                {
                                    ConfigUtility.AddCodedValueDomain(TracingHelper.PGEElectricTracingURL, layerInfo.Id, kvp2.Value.Id.ToString(),
                                        kvp3.Key, codedValueDomain.Name, kvp4.Key, kvp4.Value);
                                }
                            }
                            else
                            {

                            }
                        }
                        ConfigUtility.AddCodedValueDomain(TracingHelper.PGEElectricTracingURL, layerInfo.Id, kvp2.Value.Id.ToString(),
                                        layerInfo.TypeIdField, layerInfo.TypeIdField, kvp2.Value.Id, kvp2.Value.Name);
                        ConfigUtility.AddCodedValueDomain(TracingHelper.PGEElectricTracingURL, layerInfo.Id, kvp2.Value.Id.ToString(),
                                        layerInfo.TypeIdField, layerInfo.TypeIdField, kvp2.Value.Id, kvp2.Value.Name);
                    }
                    int classID = ConfigUtility.GetClassIDFromLayerID(TracingHelper.PGEElectricTracingURL, layerInfo.Id);
                    ConfigUtility.AddNewSubtypeField(classID, layerInfo.TypeIdField);
                }
                else if (layerInfo.Fields != null)
                {
                    foreach (ESRI.ArcGIS.Client.Field field in layerInfo.Fields)
                    {
                        if (field.Domain != null)
                        {
                            if (field.Domain is CodedValueDomain)
                            {
                                CodedValueDomain codedValueDomain = field.Domain as CodedValueDomain;
                                foreach (KeyValuePair<object, string> kvp4 in codedValueDomain.CodedValues)
                                {
                                    ConfigUtility.AddCodedValueDomain(TracingHelper.PGEElectricTracingURL, layerInfo.Id, "",
                                        field.Name, codedValueDomain.Name, kvp4.Key, kvp4.Value);
                                }
                            }
                        }
                    }
                    int classID = ConfigUtility.GetClassIDFromLayerID(TracingHelper.PGEElectricTracingURL, layerInfo.Id);
                    ConfigUtility.AddNewSubtypeField(classID, "");
                }

                //Build field Alias information.
                if (layerInfo.Fields != null)
                {
                    foreach (ESRI.ArcGIS.Client.Field field in layerInfo.Fields)
                    {
                        ConfigUtility.AddAliasFieldNameMap(TracingHelper.PGEElectricTracingURL, layerInfo.Id, field.Name, field.Alias);
                    }
                }
            }
        }

        #region Object Class ID to Layer ID mapping

        private static Object LoadingClassIDsLock = new Object();

        private void GetLayerFieldInfo(IDictionary<int, FeatureLayerInfo> featLayerInfoDict, Exception e)
        {
            try
            {
                string mapServerURL = "";
                if (featLayerInfoDict.Count <= 0) { return; }

                //Determine which mapserver url this belongs to
                foreach (Layer layer in MapControl.Layers)
                {
                    if (layer is ArcGISDynamicMapServiceLayer)
                    {
                        bool correctLayer = true;
                        ArcGISDynamicMapServiceLayer dynamicMapServiceLayer = layer as ArcGISDynamicMapServiceLayer;

                        FeatureLayerInfo testLayer = null;
                        foreach (LayerInfo layerInfo in dynamicMapServiceLayer.Layers)
                        {

                            bool foundLayer = false;
                            foreach (KeyValuePair<int, FeatureLayerInfo> kvp in featLayerInfoDict)
                            {
                                try
                                {
                                    testLayer = kvp.Value;
                                    if (layerInfo.ID == testLayer.Id && layerInfo.Name == testLayer.Name)
                                    {
                                        foundLayer = true;
                                        break;
                                    }

                                }
                                catch (Exception ex) { }
                            }
                            if (!foundLayer)
                            {
                                correctLayer = false;
                                break;
                            }
                        }

                        //In the case of schematics, it has a 250 scale stored display which is exactly the same except for reference scale.  This caused the 250
                        //scale not get picked up here properly.  Now it should be as we will continue processing through the rest of the layers if one determined to be
                        //correct was one that has already been added.
                        if (correctLayer && !ConfigUtility.LayerIDToFieldMapping.ContainsKey(dynamicMapServiceLayer.Url))
                        {
                            mapServerURL = dynamicMapServiceLayer.Url;
                            break;
                        }
                        else { correctLayer = false; }
                    }
                }

                if (String.IsNullOrEmpty(mapServerURL)) { return; }

                foreach (KeyValuePair<int, FeatureLayerInfo> kvp in featLayerInfoDict)
                {
                    FeatureLayerInfo featLayerInfo = kvp.Value;

                    if (!ConfigUtility.LayerIDToFieldMapping.ContainsKey(mapServerURL))
                    {
                        ConfigUtility.LayerIDToFieldMapping.Add(mapServerURL, new Dictionary<int, List<string>>());
                    }

                    if (!ConfigUtility.LayerIDToFieldMapping[mapServerURL].ContainsKey(featLayerInfo.Id))
                    {
                        ConfigUtility.LayerIDToFieldMapping[mapServerURL].Add(featLayerInfo.Id, new List<string>());
                    }

                    if (featLayerInfo.Fields == null) { continue; }

                    foreach (ESRI.ArcGIS.Client.Field field in featLayerInfo.Fields)
                    {
                        ConfigUtility.LayerIDToFieldMapping[mapServerURL][featLayerInfo.Id].Add(field.FieldName);
                    }
                }
            }
            catch (Exception ex)
            {
                //If a layer is having issues initializing this can fail. We'll ignore the error and the user will need to refresh
            }
        }

        private void LoadClassIDMapping(Layer layer)
        {
            try
            {
                if (layer == null || !(layer is ArcGISDynamicMapServiceLayer)) { return; }
                ArcGISDynamicMapServiceLayer dynamicMapServiceLayer = layer as ArcGISDynamicMapServiceLayer;

                //We will first make the call to build a field mapping for each layer as well.
                dynamicMapServiceLayer.GetAllDetails(GetLayerFieldInfo);

            }
            catch (Exception ex)
            {

            }
        }


        #endregion

        void LayerVisibilityTree_LayerVisibilityChanged(object sender, IsVisibleEventArgs e)
        {
            if (sender is LayerItem)
            {
                LayerItem li = (LayerItem)sender;
                if (li.Label.Equals("Annotation"))
                {
                    if (!e.IsVisible)
                    {
                        logger.Info("Annotation Layer turned off:" + DateTime.Now.ToString());
                    }
                }
                else if (li.Label.Equals("WIP"))
                {
                    if (!e.IsVisible)
                    {
                        logger.Info("WIP Layer turned off:" + DateTime.Now.ToString());
                    }
                }
                else if (li.Label.Equals("Satellite"))
                {
                    if (!e.IsVisible)
                    {
                        AdHocTemplateButton.IsEnabled = true;
                        CMCSTemplateButton.IsEnabled = true; //INC000004403856                        
                    }
                    else
                    {
                        AdHocTemplateButton.IsEnabled = false;
                        CMCSTemplateButton.IsEnabled = false; //INC000004403856

                    }
                }
            }
        }

        private void SetLayersEventHandlers()
        {
            // method could be called twice, so remove handlers first
            foreach (var layer in MapControl.Layers)
            {
                layer.Initialized -= Layer_Initialized;
                layer.Initialized += Layer_Initialized;
                layer.InitializationFailed -= Layer_InitializationFailed;
                layer.InitializationFailed += Layer_InitializationFailed;
            }
        }

        private void SetTraceMaps()
        {
            if (_waitingForWebmap == true)
            {
                ElectricTrace.Map = null;
                GasTrace.Map = null;
                GasTraceTemporaryMarks.Map = null;
                GasTraceTemporaryMarks.PlacingTemporaryMark -= TraceTemporaryMarks_PlacingTemporaryMark;
                GasTraceTemporaryMarks.PlacedTemporaryMark -= TraceTemporaryMarks_PlacedTemporaryMark;
                WaterTrace.Map = null;
                WaterTraceTemporaryMarks.Map = null;
                WaterTraceTemporaryMarks.PlacingTemporaryMark -= TraceTemporaryMarks_PlacingTemporaryMark;
                WaterTraceTemporaryMarks.PlacedTemporaryMark -= TraceTemporaryMarks_PlacedTemporaryMark;
            }
            else
            {
                ElectricTrace.Map = MapControl;
                GasTrace.Map = MapControl;
                GasTraceTemporaryMarks.Map = MapControl;
                // method could be called twice, so remove handlers first
                GasTraceTemporaryMarks.PlacingTemporaryMark -= TraceTemporaryMarks_PlacingTemporaryMark;
                GasTraceTemporaryMarks.PlacedTemporaryMark -= TraceTemporaryMarks_PlacedTemporaryMark;
                GasTraceTemporaryMarks.PlacingTemporaryMark += TraceTemporaryMarks_PlacingTemporaryMark;
                GasTraceTemporaryMarks.PlacedTemporaryMark += TraceTemporaryMarks_PlacedTemporaryMark;
                WaterTrace.Map = MapControl;
                WaterTraceTemporaryMarks.Map = MapControl;
                WaterTraceTemporaryMarks.PlacingTemporaryMark -= TraceTemporaryMarks_PlacingTemporaryMark;
                WaterTraceTemporaryMarks.PlacedTemporaryMark -= TraceTemporaryMarks_PlacedTemporaryMark;
                WaterTraceTemporaryMarks.PlacingTemporaryMark += TraceTemporaryMarks_PlacingTemporaryMark;
                WaterTraceTemporaryMarks.PlacedTemporaryMark += TraceTemporaryMarks_PlacedTemporaryMark;
            }
        }

        void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            _pageLoaded = true;
            foreach (WebmapPasswordPrompt webmapPasswordPrompt in _webMapPrompts)
            {
                webmapPasswordPrompt.Show();
            }

            StatusBar.Text = "Loading...";
            StatusBar.UpdateLayout();
            logger.Info("Main page successfully loaded");
        }

        void AttributesViewer_ExportToExcelFinished(object sender, EventArgs e)
        {
            BusyIndicator.BusyContent = string.Empty;
            BusyIndicator.IsBusy = false;

            SelectionComboBox.Visibility = Visibility.Visible;
            LockingTools.Visibility = Visibility.Visible;
            Identify.IsEnabled = true;
            Locate.IsEnabled = true;

            ElectricTrace.IsEnabled = _electricTraceEnabledBeforeExport;
            GasTrace.IsEnabled = _gasTraceEnabledBeforeExport;
            GasTraceTemporaryMarks.IsEnabled = _gasTraceEnabledBeforeExport;
            WaterTrace.IsEnabled = _waterTraceEnabledBeforeExport;
            WaterTraceTemporaryMarks.IsEnabled = _waterTraceEnabledBeforeExport;

            AttributesViewer.IsEnabled = true;
            CoordinatesTool.IsEnabled = true;
            Editor.IsEnabled = true;
            MeasureTool.IsEnabled = true;
            AdHocTemplateButton.IsEnabled = true;
            CMCSTemplateButton.IsEnabled = true; //INC000004403856            
            StandardTemplateButton.IsEnabled = true;
            DelineationPrintButton.IsEnabled = true;

            logger.Info("Export to Excel complete");
        }

        void AttributesViewer_ExportToExcelStarted(object sender, EventArgs e)
        {
            logger.Info("Export to Excel started");
            SelectionComboBox.Visibility = Visibility.Collapsed;
            LockingTools.Visibility = Visibility.Collapsed;
            Identify.IsEnabled = false;
            Locate.IsEnabled = false;

            _electricTraceEnabledBeforeExport = ElectricTrace.IsEnabled;
            ElectricTrace.IsEnabled = false;

            _gasTraceEnabledBeforeExport = GasTrace.IsEnabled;
            GasTrace.IsEnabled = false;
            GasTraceTemporaryMarks.IsEnabled = false;

            _waterTraceEnabledBeforeExport = WaterTrace.IsEnabled;
            WaterTrace.IsEnabled = false;
            WaterTraceTemporaryMarks.IsEnabled = false;

            AttributesViewer.IsEnabled = false;
            CoordinatesTool.IsEnabled = false;
            Editor.IsEnabled = false;
            MeasureTool.IsEnabled = false;
            AdHocTemplateButton.IsEnabled = false;
            CMCSTemplateButton.IsEnabled = false; //INC000004403856
            StandardTemplateButton.IsEnabled = false;
            DelineationPrintButton.IsEnabled = false;

            BusyIndicator.Width = this.ActualWidth;
            BusyIndicator.Height = this.ActualHeight;

            BusyIndicator.BusyContent = "Exporting data to Excel...";
            BusyIndicator.IsBusy = true;
        }

        private void Editor_Loaded(object sender, RoutedEventArgs e)
        {
            var savetoDb = GetChildObject<FrameworkElement>(Editor, "SaveToDB") as Button;
            if (savetoDb != null) savetoDb.IsEnabled = _enableSaveToDB;
        }

        void TraceTemporaryMarks_PlacedTemporaryMark(object sender, EventArgs e)
        {
            BusyIndicator.BusyContent = string.Empty;
            BusyIndicator.IsBusy = false;
        }

        void TraceTemporaryMarks_PlacingTemporaryMark(object sender, CancelEventArgs e)
        {
            BusyIndicator.BusyContent = "Placing/Removing a temporary mark...";
            BusyIndicator.IsBusy = true;
        }

        void MapControl_Progress(object sender, ProgressEventArgs e)
        {
            try
            {
                if (e.Progress == 100)
                {
                    MapControl.Progress -= MapControl_Progress;

                    MapIsLoaded();
                }

            }
            catch (Exception ex)
            {
                throw;
            }
        }


        private void SelectDefaultStoredView()
        {
            if (string.IsNullOrEmpty(_defaultStoredView)) return;
            Tools.StoredViewControl.DefaultStoredView = _defaultStoredView;
            Tools.StoredViewControl.LocateControl = this.Locate;
            Tools.StoredViewControl.ApplyTemplate();
            Tools.StoredViewControl.SetLocateControlLocates();
        }

        void MapControlExtentChanged(object sender, ExtentEventArgs e)
        {
            Map mapControl = sender as Map;
            if (mapControl != null)
            {
                mapControl.MinimumResolution = 0.0000000123;

                if (mapControl.Layers["Satellite"] != null)
                {

                    if (mapControl.Scale < 6000 && mapControl.Layers["Satellite"].Visible)
                    {
                        mapControl.Layers["Commonlandbase"].Visible = false;

                    }
                    //else
                    //{
                    //    mapControl.Layers["Commonlandbase"].Visible = true;
                    //}
                }
            }
            MarkerManager.ShowMarkers();

            var factor = 1 / MapControl.Resolution;
            if (factor != 1)
            {
                AttributesViewer.MarkerSize = factor * _unscaledMarkerSize;
            }

            ScaleWipCustomTextSymbol();
        }

        private void ScaleWipCustomTextSymbol()
        {
            FeatureLayer wipLayer = MapControl.Layers[_wipLabelLayerName] as FeatureLayer;
            if (wipLayer == null) return;

            foreach (Graphic graphic in wipLayer.Graphics)
            {
                WIPEditor.ResizeSymbol(graphic.Symbol as WipCustomTextSymbol);
            }
        }

        static void MapControlExtentChanging(object sender, ExtentEventArgs e)
        {
            if (e == null) return;
            if (e.NewExtent == null) return;
            if (e.OldExtent == null) return;

            if (e.NewExtent.Height != e.OldExtent.Height)
            {
                MarkerManager.HideMarkers();
            }
        }

        private void AttributesViewer_Cleared(object sender, EventArgs e)
        {
            StatusBar.Text = null;
        }

        private void AttributesViewer_OperationCanceled(object sender, EventArgs e)
        {
            logger.Info("Attribute viewer - operation canceled");

            Locate.Cancel();
            //Identify.Cancel();
            CancelIdentify();
            ElectricTrace.Cancel();
            GasTrace.Cancel();
            GasTraceTemporaryMarks.Cancel();
            WaterTraceTemporaryMarks.Cancel();
            WaterTrace.Cancel();

            SelectionComboBox.Visibility = Visibility.Visible;
            LockingTools.Visibility = Visibility.Visible;
            AttributesViewer.LoadingResults = false;

            AttributesViewer.Results = AttributesViewer.Results.AddToSelection(null);

            if (AttributesViewer.Results != null)
            {
                if (AttributesViewer.Results.Any(result => result.ExceededThreshold))
                {
                    StatusBar.Text = "Exceeded result threshold.";
                    logger.Warn(StatusBar.Text);
                }
                else
                {
                    StatusBar.Text = "Found " + AttributesViewer.Results.Sum(result => result.Features.Count) + " Features";
                    logger.Info(StatusBar.Text);
                }
            }

            if (AttributesViewer.Results == null || !AttributesViewer.Results.Any())
            {
                NoneResultLabel.Visibility = Visibility.Visible;
            }
        }

        private void Control_Working(object sender, CancelEventArgs cancelEventArgs)
        {
            SelectionComboBox.Visibility = Visibility.Collapsed;
            LockingTools.Visibility = Visibility.Collapsed;
            NoneResultLabel.Visibility = Visibility.Collapsed;
            AttributesViewer.ClearGraphics();
            AttributesViewer.LoadingResults = true;

            // Fix the identify tool,sometime layervisibilty tree control does not have all the map layers
            //so refresh the layervisibilty tree control forecfully to contains all map layers
            if (sender is IdentifyControl)
            {
                // get the TOC control
                Legend TOC = this.Tools.LayerControl.TOC;
                //Initialize the LayerVisibilityTree
                LayerVisibilityTree.InitializeTree(MapControl);
                //Add all the LayersItem of TOC
                AddItems(TOC.LayerItems);
            }

        }

        //add all LayerItem from TOC.
        private void AddItems(ObservableCollection<LayerItemViewModel> layerItems)
        {
            // get all the layerItems from TOC and add them into LayerVisibilityTree.
            foreach (LayerItemViewModel childItem in layerItems)
            {
                if (childItem.IsEnabled && childItem.IsVisible)
                {
                    var item = new LayerItem(childItem.Layer, childItem.Label, childItem.SubLayerID, childItem.IsVisible, childItem.IsEnabled, MapControl);
                    LayerVisibilityTree.AddItemToRoot(item, MapControl);
                    this.Tools.LayerControl.Refresh(childItem, item, MapControl);
                }
            }
        }


        private void Control_WorkCompletedNoZoom(object sender, ResultEventArgs e)
        {
            if (sender is IdentifyControl)
            {
                string _where = "";
                string _currentLayerURL = "";
                string _fieldname = "";
                layerSearchCount = 0;
                CurrentlyExecutingTasks.Clear();
                identifyobservables.Clear();
                //QueryTask _queryTask = new QueryTask(
                foreach (ResultSet result in e.Results)
                {
                    if (result.Name.Contains("SAP ")) continue;

                    if (!result.Service.Contains("Tlm") && !result.Service.Contains("PublicationFS") && !result.Service.Contains("CommonLandbase") && !

result.Service.Contains("Schematics"))
                    {
                        _where = "";
                        _fieldname = "OBJECTID";
                        _currentLayerURL = result.Service + "/" + result.ID.ToString(); ;

                        foreach (Graphic graphic in result.Features)
                        {
                            //Get Field Alias from fields, Perfect Fix is to have same Alias name for all layer in mxd.
                            if (graphic.Attributes[_fieldname] == null)
                                _fieldname = "Object ID";

                            if (_where == "")
                                _where = _where + graphic.Attributes[_fieldname];
                            else
                                _where = _where + "," + graphic.Attributes[_fieldname];
                        }

                        if (_where != "" && !_where.ToUpper().Contains("NULL"))
                        {
                            ESRI.ArcGIS.Client.Tasks.Query _query = new ESRI.ArcGIS.Client.Tasks.Query();
                            _query.ReturnGeometry = true;
                            _query.OutFields.Add("*");
                            if (ConfigUtility.IdentifyFieldMapping.ContainsKey(_currentLayerURL))
                            {
                                //This Condition is valid for Layers having Join Ex: Support Strcture JUA Label & Pri UG/OH Conductors in Circuit Map SD

                                _query.Where = ConfigUtility.IdentifyFieldMapping[_currentLayerURL] + " IN (" + _where + ")";
                            }
                            else
                            {
                                _query.Where = "OBJECTID IN (" + _where + ")";
                            }

                            QueryTask queryIdentifyTask = new QueryTask(_currentLayerURL);
                            queryIdentifyTask.ExecuteCompleted += new EventHandler<QueryEventArgs>(queryIdentifyTask_ExecuteCompleted);
                            queryIdentifyTask.Failed += new EventHandler<ESRI.ArcGIS.Client.Tasks.TaskFailedEventArgs>(queryIdentifyTask_Failed);

                            queryIdentifyTask.ExecuteAsync(_query);
                            CurrentlyExecutingTasks.Add(queryIdentifyTask);
                        }

                    }
                    else
                    {
                        identifyobservables.Add(result);
                    }
                }

                if (CurrentlyExecutingTasks.Count == 0)
                {

                    bool zoom = AttributesViewer.AutoZoomToResults;
                    AttributesViewer.AutoZoomToResults = false;
                    Control_WorkCompleted(sender, new ResultEventArgs(identifyobservables));
                    AttributesViewer.AutoZoomToResults = zoom;
                }
            }
            else
            {
                bool zoom = AttributesViewer.AutoZoomToResults;
                AttributesViewer.AutoZoomToResults = false;
                Control_WorkCompleted(sender, e);
                AttributesViewer.AutoZoomToResults = zoom;
            }
        }

        #region Identify Fix

        void queryIdentifyTask_Failed(object sender, ESRI.ArcGIS.Client.Tasks.TaskFailedEventArgs e)
        {
            layerSearchCount++;
            //throw new NotImplementedException();
            if (CurrentlyExecutingTasks.Count == layerSearchCount)
            {
                bool zoom = AttributesViewer.AutoZoomToResults;
                AttributesViewer.AutoZoomToResults = false;
                IdentifyControl id = null;
                Control_WorkCompleted(id, new ResultEventArgs(identifyobservables));
                AttributesViewer.AutoZoomToResults = zoom;
            }
        }

        void queryIdentifyTask_ExecuteCompleted(object sender, QueryEventArgs e)
        {
            //throw new NotImplementedException();
            layerSearchCount++;

            QueryTask queryTask = sender as QueryTask;
            if ((queryTask == null) || (e.FeatureSet == null))
            {
                return;
            }


            string url = queryTask.Url;
            string mapServer = url.Substring(0, url.LastIndexOf("/"));
            int layerID = -1;
            string layerIDString = url.Substring(url.LastIndexOf("/") + 1);
            Int32.TryParse(layerIDString, out layerID);
            string layerAlias = null;

            foreach (Layer layer in ConfigUtility.CurrentMap.Layers)
            {
                if (layer is ArcGISDynamicMapServiceLayer)
                {
                    ArcGISDynamicMapServiceLayer dynamicLayer = layer as ArcGISDynamicMapServiceLayer;
                    if (mapServer == dynamicLayer.Url)
                    {
                        foreach (LayerInfo layerInfo in dynamicLayer.Layers)
                        {
                            if (layerInfo.ID == layerID)
                            {
                                layerAlias = layerInfo.Name;
                                break; //INC000004465554
                            }
                        }
                    }
                }

                else if (layer is FeatureLayer && mapServer.Contains("WIPRedlines_Data") && layer.ID != "1")
                {
                    /***********PLC Changes RK 07/21/2017***************/
                    FeatureLayer flayer = layer as FeatureLayer;
                    if (flayer.Url.Contains("WIPRedlines_Data") && flayer.LayerInfo.Id == layerID)
                    {
                        layerAlias = flayer.LayerInfo.Name;
                        break;
                    }
                    /***********PLC Changes End***************/
                }
                /***********PLC Changes RK 07/21/2017***************/
                else if (layer is FeatureLayer && mapServer.Contains("PLCINFO") && layer.ID != "1")
                {
                    FeatureLayer flayer = layer as FeatureLayer;
                    if (flayer.LayerInfo != null)
                    {
                        layerAlias = flayer.LayerInfo.Name;
                    }

                }
                /***********PLC Changes End***************/
            }

            if (e.FeatureSet.Features.Count > 0)
            {
                ResultSet results = new ResultSet(e.FeatureSet);
                results.Service = mapServer;
                results.ID = layerID;
                results.Name = layerAlias;
                if (layerAlias != "")
                {
                    identifyobservables.Add(results);
                }
            }

            if (CurrentlyExecutingTasks.Count == layerSearchCount)
            {
                bool zoom = AttributesViewer.AutoZoomToResults;
                AttributesViewer.AutoZoomToResults = false;
                IdentifyControl id = null;
                Control_WorkCompleted(id, new ResultEventArgs(identifyobservables));
                AttributesViewer.AutoZoomToResults = zoom;
            }
        }

        void CancelIdentify()
        {
            Identify.Cancel();

            foreach (TaskBase task in CurrentlyExecutingTasks)
            {
                if (task != null && task.IsBusy)
                {
                    task.CancelAsync();
                }
            }
            CurrentlyExecutingTasks.Clear();
        }

        #endregion

        private void Control_WorkCompleted(object sender, ResultEventArgs e)
        {
            if (e == null) return;

            SelectionComboBox.Visibility = Visibility.Visible;
            LockingTools.Visibility = Visibility.Visible;
            AttributesViewer.LoadingResults = false;
            AttributesViewer.IsTracing = sender is ElectricTraceControl || sender is GasTraceControl ||
                                         sender is WaterTraceControl;

            /************** Handle Proposed Results ********************************/

            // This list will contain the non-proposed map service url with layer id,
            // and a list of object id's to query for on that layer, to be added to
            // any existing results that are not proposed.

            if (e.Results.Any())
            {
                var newQueryList = new List<Tuple<string, List<int>>>(); //<Map Service Url, List of Object IDs>

                /***** Remove this - testing only *****
                int totalFeatureCount = 0;
                /***** End Remove this - testing only *****/

                foreach (Miner.Server.Client.Tasks.ResultSet result in e.Results)
                {
                    /* Testing
                     * totalFeatureCount += result.Features.Count;
                     */
                    if (result.Name != null && result.Name.Contains("Proposed") && !result.Service.Contains("PublicationFS"))   //Bug fix for PublicationFS Proposed layers
                    {
                        List<Tuple<string, string, string>> layerConversionList = GetLayerIdConversionList(result);

                        if (layerConversionList != null)
                        {
                            Tuple<string, string, string> layerConversion = (from tuple in layerConversionList
                                                                             where tuple.Item2 == result.ID.ToString()
                                                                             select new Tuple<string, string, string>(tuple.Item1, tuple.Item2, tuple.Item3))
                                .FirstOrDefault();
                            if (layerConversion != null)
                            {
                                if (layerConversion.Item3 != "Multiple")
                                {
                                    //One to one relationship. All features will be from the same non-proposed layer
                                    int nonProposedLayerId = ModifyResultLayerId(layerConversion.Item3, result);

                                    if (nonProposedLayerId != result.ID) //ID updated
                                    {
                                        var nonProposedObjectIdsList = new List<int>();

                                        for (int i = 0; i < result.Features.Count; i++)
                                        {
                                            object statusValue;
                                            result.Features[i].Attributes.TryGetValue("STATUS", out statusValue);

                                            if (statusValue == null || statusValue.ToString() == "0")
                                            // Feature is actually proposed or has null status - leave it in this result.
                                            {
                                                continue;
                                            }

                                            // Feature is not proposed.
                                            //Add feature object id to list of non-proposed object ids.
                                            object objectId;
                                            result.Features[i].Attributes.TryGetValue("OBJECTID", out objectId);

                                            int objectIdAsInt = -1;
                                            bool parsedOk = objectId != null &&
                                                            int.TryParse(objectId.ToString(), out objectIdAsInt);

                                            if (parsedOk && objectIdAsInt != -1)
                                            {
                                                nonProposedObjectIdsList.Add(objectIdAsInt);

                                                // Need to remove this non-proposed feature from this proposed ResultSet.
                                                result.Features.RemoveAt(i);
                                                // Decrement iterator as collection indexing has changed.
                                                i--;

                                                // If all features have been removed from ResultSet, must break from loop.
                                                if (result.Features.Count == 0) break;
                                            }
                                        }

                                        /* If result still has features here, they are proposed and need to be in the collection
                                         * that will be passed to the attribute viewer. */
                                        if (result.Features.Count > 0) _newResultSets.Add(result);

                                        // List of Features to query for on non-proposed layers.
                                        if (nonProposedObjectIdsList.Count > 0)
                                            newQueryList.Add(new Tuple<string, List<int>>(
                                                result.Service + "/" + nonProposedLayerId, nonProposedObjectIdsList));
                                    }
                                }

                                    /*
                                    When a one-to-many situation exists, it's possible the features grouped as "proposed"
                                    belong to differing non-proposed layers. Effectively, the proposed result features
                                    may need to be broken up into 2 or more new results, and then queried for.
                                */
                                else if (layerConversion.Item3 == "Multiple")
                                {
                                    //for INC000004215813 - Opened Switch/Fuse/DPD in ElectricDistribution and EDMaster
                                    string[] mapServiceUrl = result.Service.Split('/');
                                    string mapName = mapServiceUrl[mapServiceUrl.Count() - 2];
                                    if (_differentiatingRulesDictionary.ContainsKey(mapName + "-" + layerConversion.Item1))
                                    {
                                        List<Tuple<string, string, string>> differentiatingRulesList =
                                            new List<Tuple<string, string, string>>(
                                                _differentiatingRulesDictionary[mapName + "-" + layerConversion.Item1]);

                                        foreach (var listItem in differentiatingRulesList)
                                        {
                                            List<int> objectIdsThatMatchRule = new List<int>();
                                            string field = listItem.Item1; //Field name used in definition query
                                            string[] fieldValuesToBeTrue = listItem.Item2.Split(',');
                                            //Definition query filtering values
                                            string newLayerId = listItem.Item3;
                                            //Layer ID to use if filtering values are in the definition query.

                                            for (int i = 0; i < result.Features.Count; i++)
                                            {
                                                object statusValue;
                                                result.Features[i].Attributes.TryGetValue("STATUS", out statusValue);

                                                if (statusValue == null || statusValue.ToString() == "0")
                                                // Feature is actually proposed or has null status - leave it in this result.
                                                {
                                                    continue;
                                                }

                                                if (result.Features[i].Attributes.ContainsKey(field))
                                                {
                                                    object theKeysValue;
                                                    result.Features[i].Attributes.TryGetValue(field, out theKeysValue);

                                                    if (theKeysValue != null &&
                                                        fieldValuesToBeTrue.Contains(theKeysValue.ToString()))
                                                    {
                                                        /* Feature matches the differentiating rule. Pull the object ID for the feature
                                                        * and add it to the collection of object ids to be used in the "newQueryList"
                                                        * list. Finally, remove this feature from this proposed "result".
                                                        */

                                                        // Feature is not proposed.
                                                        //Add feature object id to list of non-proposed object ids.
                                                        object objectId;
                                                        result.Features[i].Attributes.TryGetValue("OBJECTID", out objectId);

                                                        int objectIdAsInt = -1;
                                                        bool parsedOk = objectId != null &&
                                                                        int.TryParse(objectId.ToString(), out objectIdAsInt);

                                                        if (parsedOk && objectIdAsInt != -1)
                                                        {
                                                            objectIdsThatMatchRule.Add(objectIdAsInt);

                                                            // Need to remove this non-proposed feature from this proposed ResultSet.
                                                            result.Features.RemoveAt(i);
                                                            // Decrement iterator as collection indexing just changed.
                                                            i--;

                                                            // If all features have been removed from ResultSet, must break from loop.
                                                            if (result.Features.Count == 0) break;
                                                        }
                                                    }
                                                }
                                            }

                                            if (objectIdsThatMatchRule.Count > 0)
                                                newQueryList.Add(
                                                    new Tuple<string, List<int>>(result.Service + "/" + newLayerId,
                                                        objectIdsThatMatchRule));
                                        }
                                    }

                                    /* If result still has features here, they are proposed and need to be in the collection
                                     * that will be passed to the attribute viewer. */
                                    if (result.Features.Count > 0) _newResultSets.Add(result);
                                }
                                else
                                {
                                    // Item3 is not recognized - pass the result as is to the attribute viewer.
                                    if (result.Features.Count > 0) _newResultSets.Add(result);
                                }
                            }
                        }
                    }
                    else if (result.Service.ToUpper().Contains("GEOCODESERVER"))
                    {
                        _newResultSets.Add(result);
                        ZoomtoAddress(result.Features[0].Geometry.Extent);
                    }
                    else //Result set is not proposed - add as is to _newResultSets
                    {
                        //TODO: Make this configurable, a DoNotIdentifyService list. Actually we never want the 
                        //      IdentifyControl to use these services. is this configable?
                        if (!result.Service.ToUpper().Contains("TLM") && !result.Service.Contains("PublicationFS"))
                        {
                            _newResultSets.Add(result);
                        }
                    }
                }

                /* Testing
                MessageBox.Show("Total Features before sorting: " + totalFeatureCount);
                */

                if (newQueryList.Any())
                {
                    QueryForNonProposedFeatures(newQueryList);
                }
                else if (_newResultSets.Any())
                // All original results are not proposed - send them to the attribute viewer
                {
                    SendResultsToAttibuteViewer(_newResultSets);
                    _newResultSets.Clear();
                }
            }
            else
            {
                SendResultsToAttibuteViewer(e.Results);
            }

            SetupDataGridContextMenu();
        }

        void SetupDataGridContextMenu()
        {
            if (_dataGridContextMenu == null)
            {
                _dataGridContextMenu = new DataGridContextMenu(this.AttributesViewer, _dataGridCustomButtonManager.CustomButtons);
                _dataGridContextMenu.DataGridBound += new EventHandler(_dataGridContextMenu_DataGridBound);
                _dataGridContextMenu.SubDataGridRowSelected += _dataGridCustomButtonManager.HandleSubDataGridRowSelected;
            }
            ScrollableTabControl scrollableTabControl = GetChildObject<ScrollableTabControl>(this.AttributesViewer, "PART_TabControl");
            // Bug...if Layer view is visible then contextmenu doesn't get attached
            Grid grid = VisualTreeHelper.GetChild(this.Tools, 0) as Grid;
            StackPanel stackPanel = VisualTreeHelper.GetChild(grid, 1) as StackPanel;
            if (stackPanel != null)
            {
                Border layerVisibilityBorder = VisualTreeHelper.GetChild(stackPanel, 1) as Border;
                if (layerVisibilityBorder.Visibility == Visibility.Visible)
                {
                    _dataGridContextMenu.LayerVisibilityControlIsVisible = true;
                }
            }
            scrollableTabControl.Loaded += new RoutedEventHandler(_dataGridContextMenu.tabContainerLoaded);

        }

        void _dataGridContextMenu_DataGridBound(object sender, EventArgs e)
        {
            _dataGridCustomButtonManager.AttributeDataGrid = _dataGridContextMenu.AttributeViewDataGrid;
        }

        private List<Tuple<string, string, string>> GetLayerIdConversionList(ResultSet result)
        {
            string[] mapServiceUrl = result.Service.Split('/');
            string mapType = mapServiceUrl[mapServiceUrl.Count() - 2];

            if (_proposedLayerIdConversionDictionary.ContainsKey(mapType)) //Dictionary<Map Name, List<Tuple<ProposedLayerName, ProposedID, NonProposedLayerIDs>>
            {
                return new List<Tuple<string, string, string>>(_proposedLayerIdConversionDictionary[mapType]);
            }

            return null;
        }

        private int ModifyResultLayerId(string nonProposedLayerId, ResultSet result)
        {
            int newLayerId;
            bool parsedOk = int.TryParse(nonProposedLayerId, out newLayerId);

            if (!parsedOk) return result.ID;

            logger.Info("PROPOSEDLAYER: Layer ID " + result.ID + " replaced with: " + newLayerId);
            return newLayerId;
        }

        private int _queryCount;
        private void QueryForNonProposedFeatures(List<Tuple<string, List<int>>> nonProposedFeaturesList)
        {
            _queryCount = nonProposedFeaturesList.Count();
            foreach (var listItem in nonProposedFeaturesList)
            {
                QueryTask queryTask = new QueryTask(listItem.Item1);
                queryTask.ExecuteCompleted += new EventHandler<QueryEventArgs>(queryTask_ExecuteCompleted);
                queryTask.Failed += new EventHandler<ESRI.ArcGIS.Client.Tasks.TaskFailedEventArgs>(queryTask_Failed);

                ESRI.ArcGIS.Client.Tasks.Query query = new ESRI.ArcGIS.Client.Tasks.Query { ObjectIDs = listItem.Item2.ToArray() };
                //**Rolback step for Isolation Trace fix.
                //query.ReturnGeometry = true;
                query.OutFields.Add("*");
                queryTask.ExecuteAsync(query);
            }
        }

        void queryTask_Failed(object sender, ESRI.ArcGIS.Client.Tasks.TaskFailedEventArgs e)
        {
            QueryTask qt = sender as QueryTask;
            if (qt != null)
            {
                string failedLayerUrl = qt.Url;

                string message = "There was an error getting non-proposed results for the Attribue Table from this Map Service Layer: " + failedLayerUrl +

Environment.NewLine +
                                 "The query returned the following error message: " + e.Error;

                MessageBox.Show(message, "Error encountered retreiving Non-Proposed results", MessageBoxButton.OK);
            }
            IncrementReturnedAsynQueries();
        }

        void queryTask_ExecuteCompleted(object sender, QueryEventArgs e)
        {
            QueryTask queryTask = sender as QueryTask;
            string url = queryTask.Url;
            string mapServer = url.Substring(0, url.LastIndexOf("/"));
            int layerID = -1;
            string layerIDString = url.Substring(url.LastIndexOf("/") + 1);
            Int32.TryParse(layerIDString, out layerID);
            string layerAlias = null;


            foreach (Layer layer in ConfigUtility.CurrentMap.Layers)
            {
                if (layer is ArcGISDynamicMapServiceLayer)
                {
                    ArcGISDynamicMapServiceLayer dynamicLayer = layer as ArcGISDynamicMapServiceLayer;
                    if (mapServer == dynamicLayer.Url)
                    {
                        foreach (LayerInfo layerInfo in dynamicLayer.Layers)
                        {
                            if (layerInfo.ID == layerID)
                            {
                                layerAlias = layerInfo.Name;
                                break;
                            }
                        }
                    }
                }
            }

            if (e.FeatureSet.Features.Count > 0)
            {
                ResultSet results = new ResultSet(e.FeatureSet);
                results.Service = mapServer;
                results.ID = layerID;
                results.Name = layerAlias;
                _newResultSets.Add(results);
            }

            //if (qt != null)
            //{
            //    string serviceUrl = qt.Url.Substring(0,qt.Url.LastIndexOf('/') + 1);
            //    string textId = qt.Url.Substring(qt.Url.LastIndexOf('/') + 1);
            //    int layerId = Convert.ToInt32(textId);
            //    resultSet.ID = layerId;
            //    resultSet.Service = serviceUrl;

            //    resultSet.Service = mapServer;
            //    resultSet.ID = layerID;
            //    resultSet.Name = layerAlias;
            //}

            //_newResultSets.Add(results);    

            IncrementReturnedAsynQueries();
        }

        private int _returnedAsyncQueries;
        private void IncrementReturnedAsynQueries()
        {
            _returnedAsyncQueries++;

            if (_returnedAsyncQueries.ToString() == _queryCount.ToString())
            {
                //All queries have returned - send results to Attribute Viewer and reset.
                SendResultsToAttibuteViewer(_newResultSets);
                _returnedAsyncQueries = 0;
                _newResultSets.Clear();
            }
        }

        private void SendResultsToAttibuteViewer(IEnumerable<IResultSet> results)
        {
            switch (_selectionType)
            {
                case SelectionOption.CreateNewSelection:
                    AttributesViewer.Results = AttributesViewer.Results.CreateNewSelection(results);
                    break;
                case SelectionOption.AddToSelection:
                    AttributesViewer.Results = AttributesViewer.Results.AddToSelection(results);
                    break;
                case SelectionOption.RemoveFromSelection:
                    AttributesViewer.Results = AttributesViewer.Results.RemoveFromSelection(results);
                    break;
                case SelectionOption.SelectFromSelection:
                    AttributesViewer.Results = AttributesViewer.Results.SelectFromSelection(results);
                    break;
            }

            if (AttributesViewer.Results != null)
            {
                if (AttributesViewer.Results.Any(result => result.ExceededThreshold))
                {
                    StatusBar.Text = "Exceeded result threshold.";
                    logger.Warn(StatusBar.Text);
                }
                else
                {
                    StatusBar.Text = "Found " + AttributesViewer.Results.Sum(result => result.Features.Count) + " Features";
                    //INC000004022645: S2NN, October 15,2015
                    StatusBar.Text += isNoGeographicalResult ? " Showing all results discarding Geographic Filter as there is no result found in selected Region" :

string.Empty;
                    isNoGeographicalResult = false;
                    logger.Info(StatusBar.Text);
                }

                //*******************************************ENOS2EDGIS Start********************************************************
                //ENOS2EDGIS, If the search type is 'Service point Id' search, then get the SPID entered, this is usefull to filter the related service points & generation infos
                // in the attribute viewer for feature types of Service location, Primary meter & Transformer
                if (AttributesViewer.IsThisSPSearch)
                {
                    var spidString = SettingHelper.ReadSetting(LocateSetting, "LastQuery", "Value", "System.String");
                    if (spidString != null)
                    {
                        _SPID = spidString.ToString();
                        AttributesViewer.SPID = _SPID;
                    }
                    else
                    {
                        _SPID = string.Empty;
                        AttributesViewer.SPID = null;
                    }
                }
                else
                    AttributesViewer.SPID = string.Empty;

                //ENOS2EDGIS, Once after the search results are added to Graphic layer, here we are assigning right click menu items for the 'Service Location', 'Primary Meter' & 
                // 'Transformer' graphc layers
                foreach (var graphicLayer in _graphicsMoreInfo)
                {
                    if (AttributesViewer.Map.Layers.Any(x => (x is GraphicsLayer && x.ID != null && x.ID.Replace("GRAPHICS", "") == graphicLayer.Key.ToLowerInvariant())))
                    {
                        GraphicsLayer layerToEnableMoreInfo = AttributesViewer.Map.Layers.FirstOrDefault(x => x is GraphicsLayer && x.ID != null && x.ID.Replace("GRAPHICS", "") == graphicLayer.Key.ToLowerInvariant()) as GraphicsLayer;
                        layerToEnableMoreInfo.MouseRightButtonDown += new GraphicsLayer.MouseButtonEventHandler(GraphicLayer_MouseRightButtonDown);
                        layerToEnableMoreInfo.MouseRightButtonUp += new GraphicsLayer.MouseButtonEventHandler(GraphicLayer_MouseRightButtonUp);
                    }
                }
                //*******************************************ENOS2EDGIS End********************************************************

                //ENOSChange, Once after the search results are added to Graphic layer, here we are assigning right click menu items for 
                ///the Layers mentioned under tag "GenerationOnFeeder" in page config
                foreach (var graphicLayer in GetGenOnFeeder.GenFeederLayerName)
                {
                    if (AttributesViewer.Map.Layers.Any(x => (x is GraphicsLayer && x.ID != null && x.ID.Replace("GRAPHICS", "") == graphicLayer.ToLowerInvariant())))
                    {
                        GraphicsLayer layerToEnableGenOnFeeder = AttributesViewer.Map.Layers.FirstOrDefault(x => x is GraphicsLayer && x.ID != null && x.ID.Replace("GRAPHICS", "") == graphicLayer.ToLowerInvariant()) as GraphicsLayer;
                        layerToEnableGenOnFeeder.MouseRightButtonDown += new GraphicsLayer.MouseButtonEventHandler(GenFeederGraphicLayer_MouseRightButtonDown);
                        layerToEnableGenOnFeeder.MouseRightButtonUp += new GraphicsLayer.MouseButtonEventHandler(GenFeederGraphicLayer_MouseRightButtonUp);
                    }
                }

                //*******************************************ENOSChange End********************************************************

            }

            if (AttributesViewer.Results == null || !AttributesViewer.Results.Any())
            {
                NoneResultLabel.Visibility = Visibility.Visible;
            }

            if (_newResultSets.Any()) _newResultSets.Clear();
        }

        //*************************************ENOS2EDGIS Start*****************************************
        //ENOS2EDGIS, new event for Graphic layer mouse right button down 
        private void GraphicLayer_MouseRightButtonDown(object sender, GraphicMouseButtonEventArgs e)
        {
            e.Handled = true;
        }
        //ENOS2EDGIS, new event for Graphic layer mouse right button up 
        private void GraphicLayer_MouseRightButtonUp(object sender, GraphicMouseButtonEventArgs e)
        {
            _hoveredGraphic = e.Graphic;
            //Get the current graphic layer on which user right clicked.
            environmentLocation = MapControl.ScreenToMap(e.GetPosition(MapControl));
            _currentGraphicLayerName = e.Graphic.Attributes["LAYERNAME"].ToString();
            //Run this code only for generation related graphics(i.e. service location, primary meter & transformer)
            if (_graphicsMoreInfo.ContainsKey(_currentGraphicLayerName))
            {
                foreach (KeyValuePair<string, IDataGridCustomButton> dataGridCustomButton in _dataGridCustomButtonManager.CustomButtons)
                {
                    //ENOSChange
                    removeExistingCustomButton(dataGridCustomButton);
                    if (dataGridCustomButton.Key == "More Info...")
                    {
                        if (dataGridCustomButton.Value.Visible)
                        {
                            //Add 'More Info...' menu item to the graphic layer context menu
                            var menuItemDataGridButton = new MenuItem() { Header = dataGridCustomButton.Key };
                            menuItemDataGridButton.Visibility = System.Windows.Visibility.Visible;
                            menuItemDataGridButton.Click += new RoutedEventHandler(moreInfoMenuButton_Click);

                            if (_currentGraphicLayerName == FC_SERVICELOCATION)
                            {
                                if (_graphicLayerContextMenu.Items.Count == 2)
                                {
                                    //This condition is to enable 'More Info' only for those service locations where gencategory is either 1 or 2.
                                    if (_hoveredGraphic != null && _hoveredGraphic.Attributes != null && _hoveredGraphic.Attributes["GENCATEGORY"] != null &&

(_hoveredGraphic.Attributes["GENCATEGORY"].ToString().ToUpper() == "PRIMARY" || _hoveredGraphic.Attributes["GENCATEGORY"].ToString().ToUpper() == "SECONDARY"))
                                    {
                                        menuItemDataGridButton.IsEnabled = true;
                                    }
                                    else
                                        menuItemDataGridButton.IsEnabled = false;
                                }
                            }
                            else
                            {
                                menuItemDataGridButton.IsEnabled = true;
                            }
                            _graphicLayerContextMenu.Items.Add(menuItemDataGridButton);
                        }
                    }

                    //*************ENOSChange Start******************
                    if (dataGridCustomButton.Key == "Show Generation")
                    {
                        if (dataGridCustomButton.Value.Visible)
                        {
                            //This condition is to show 'Show Generation' only for Transformer layer
                            if (_currentGraphicLayerName == "Transformer")
                            {
                                //Add 'More Info...' menu item to the graphic layer context menu
                                var showGenerationButton = new MenuItem() { Header = dataGridCustomButton.Key };
                                showGenerationButton.Visibility = System.Windows.Visibility.Visible;
                                showGenerationButton.Click += new RoutedEventHandler(menuItemShowGeneration_Click);
                                showGenerationButton.IsEnabled = true;
                                _graphicLayerContextMenu.Items.Add(showGenerationButton);
                            }
                        }

                    }

                    //************ENOSChange End*************************
                }
                //CIT filter for engineers group + identify code
                if (engineers_group == true)
                {
                    list_Items_new = _graphicLayerContextMenu.Items.ToList();
                    for (int count = 0; count < list_Items_new.Count; count++)
                    {
                        string value = (list_Items_new[count] as MenuItem).Header.ToString();
                        values_menu_new.Add(value);
                    }
                    if (values_menu_new.Contains("Update FilledDuct"))
                    {
                        int position = values_menu_new.IndexOf("Update FilledDuct");
                        _graphicLayerContextMenu.Items.RemoveAt(position);
                    }
                    values_menu_new.Clear();
                }
                //Set the prepared context menu to graphic layer
                ContextMenuService.SetContextMenu((DependencyObject)sender, _graphicLayerContextMenu);
                _currentGraphicLayer = ((GraphicsLayer)sender);
                _hoveredGraphic = e.Graphic;

                _graphicLayerContextMenu.IsOpen = true;
                //In this block, we are checking whether the graphic layer has device settings enabled or not.
                FeatureLayer _featLayerOfGraphicLayer = GetFeatureLayerFromMapByName(_currentGraphicLayerName);
                if (_featLayerOfGraphicLayer != null)
                {
                    string serviceURL = _featLayerOfGraphicLayer.Url.Substring(0, _featLayerOfGraphicLayer.Url.LastIndexOf("/"));

                    _tlm.IsManuallyEnabled = _tlm.LayerIsTlm(_currentGraphicLayerName);

                    bool selectionIsDeviceSettingsEnabled = false;
                    // Check for settings validity
                    if (_deviceSettings._deviceSettingsLayerMappings.ContainsKey(serviceURL))
                    {
                        string mappedServiceURL = _deviceSettings._deviceSettingsLayerMappings[serviceURL];
                        selectionIsDeviceSettingsEnabled = _deviceSettings.SelectionIsDeviceSettingsEnabled(mappedServiceURL, _currentGraphicLayerName);
                    }
                    _deviceSettings.IsManuallyEnabled = selectionIsDeviceSettingsEnabled;
                }
            }
        }


        //ENOS2EDGIS, This event fires when user clicked on 'More Info...' in the right click context menu of graphic
        void moreInfoMenuButton_Click(object sender, RoutedEventArgs e)
        {
            IDataGridCustomButton customButton = _dataGridCustomButtonManager.CustomButtons[((MenuItem)sender).Header.ToString()];
            customButton.Attributes = _hoveredGraphic.Attributes;
            //Service location does not have a settings ui, for any service location we need to open 'Generation Info' settings UI, for that reason , open settings ui 
            //directly for other graphics of type Primary meter or transformer.
            if (_currentGraphicLayerName != "Service Location")
            {
                // Fix for primary meter settings open from map
                //customButton.Open(_hoveredGraphic.Attributes["GLOBALID"].ToString() + (_currentGraphicLayerName == "Primary Meter" ? _hoveredGraphic.Attributes["CGC12"].ToString() : string.Empty), _currentGraphicLayerName);
                customButton.Open(_hoveredGraphic.Attributes["GLOBALID"].ToString(), _currentGraphicLayerName);
            }
            else
            {
                //but for servcie location, we need to get the related generation info records first, and then show them to user and let user decide which generation 
                //they want to open SETTINGS UI
                int objectId = GetObjectIDValue(_hoveredGraphic.Attributes);
                if (objectId >= 0)
                {
                    SpatialReference spatialReference = MapControl.SpatialReference;
                    //for service location first we need to get the related generation data, and then open settings ui for those generations.
                    IRelationshipService relationshipService = new RelationshipService();
                    string decodedService = string.Empty;

                    //Get the feature layer from the graphic layer name
                    //ENOSTOEDGIS - issue resolution
                    //FeatureLayer _featLayerOfGraphicLayer = GetFeatureLayerFromMapByName(_currentGraphicLayerName);
                    int layerID = 0;

                    //ENOSTOEDGIS - issue resolution
                    //if (_featLayerOfGraphicLayer != null)
                    {
                        string serviceURL = null;
                        //ENOSTOEDGIS - issue resolution
                        //string serviceURL = _featLayerOfGraphicLayer.Url.Substring(0, _featLayerOfGraphicLayer.Url.LastIndexOf("/"));
                        //if the service URL is a feature server, try to get a correspoinding map server URL
                        //if (serviceURL.ToUpper().Contains("FEATURESERVER"))
                        {
                            if (_graphicsMoreInfo.ContainsKey(_currentGraphicLayerName))
                            {
                                serviceURL = _graphicsMoreInfo[_currentGraphicLayerName].Key;
                                layerID = Convert.ToInt32(_graphicsMoreInfo[_currentGraphicLayerName].Value);
                            }
                        }
                        decodedService = HttpUtility.UrlDecode(serviceURL);
                    }

                    //Get the relationship for service location where it has muliple relationship and one of the relationship should be with Service Point
                    IEnumerable<RelationshipInformation> relationshipInfos = from relatedInfo in AttributesViewer.RelationshipInformation
                                                                             where (relatedInfo.LayerId == layerID) && relatedInfo.RelationshipNames.Any(x => x.ToUpper().IndexOf("SERVICEPOINT", StringComparison.CurrentCultureIgnoreCase) != -1) &&
                                                                                    relatedInfo.RelationshipIds.Count == 2 && decodedService != null &&
                                                                                   decodedService.ToLower().Contains(relatedInfo.Service.ToLower())
                                                                             select relatedInfo;

                    //If this is a service point search, we don't need other service points other than the one entered by user, so here we are preparing 
                    if (_IsThisSPpSearch && _SPID != string.Empty)
                    {
                        //Apply definition expression if user is searching for a service point ID.
                        relationshipService.DefinitionExpression = "SERVICEPOINTID=" + _SPID;
                        foreach (RelationshipInformation relItem in relationshipInfos)
                        {
                            //Here, We  need to apply for definition expression for 'SERVICEPOINT,GENERATIONINFO' 
                            if (relItem.RelationshipNames.Count == 2)
                            {
                                relItem.ApplyDefinitionExpression = true;
                            }
                            else
                            {
                                relItem.ApplyDefinitionExpression = false;
                            }
                        }
                    }
                    else
                    {
                        relationshipService.DefinitionExpression = string.Empty;
                    }
                    relationshipService.ExecuteCompleted += (s, i) =>
                        RelationshipServiceCompleted(sender, i);

                    relationshipService.GetRelationshipsAsync(relationshipInfos, new int[] { objectId }, spatialReference);
                    //ENOS2EDGIS, showing Gen Type Domain values 
                    GetGenTypeDomainValues();
                }
            }
        }

        //ENOS2EDGIS, Gets the Object Id feild name 
        private static int GetObjectIDValue(IDictionary<string, object> attributes)
        {
            int objectID = -1;

            string oidFieldName = GetObjectIDFieldName(attributes);
            if (!string.IsNullOrEmpty(oidFieldName))
            {
                Int32.TryParse(attributes[oidFieldName].ToString(), out objectID);
            }
            return objectID;
        }
        //ENOS2EDGIS,Gets the Object Id feild name
        private static string GetObjectIDFieldName(IDictionary<string, object> attributes)
        {
            string oidFieldName = string.Empty;
            if (attributes != null)
            {
                if (attributes.ContainsKey("OBJECTID"))
                {
                    oidFieldName = "OBJECTID";
                }
                else if (attributes.ContainsKey("OBJECT ID"))
                {
                    oidFieldName = "OBJECT ID";
                }
                else if (attributes.ContainsKey("Object ID"))
                {
                    oidFieldName = "Object ID";
                }
                else if (attributes.ContainsKey("ObjectID"))
                {
                    oidFieldName = "ObjectID";
                }
                else if (attributes.ContainsKey("ObjectId"))
                {
                    oidFieldName = "ObjectId";
                }
                else if (attributes.ContainsKey("Object Id"))
                {
                    oidFieldName = "Object Id";
                }
                else if (attributes.ContainsKey("OID"))
                {
                    oidFieldName = "OID";
                }
            }
            return oidFieldName;
        }
        //ENOS2EDGIS, this event will be fired after querying relationships is successful
        private void RelationshipServiceCompleted(object sender, Miner.Server.Client.RelationshipEventArgs args)
        {
            if ((args == null) || (args.Results == null) || (args.Results.Count == 0))
            {
                MessageBox.Show("No Generations are attached to this service location", "No Generations Found", MessageBoxButton.OK);
            }
            else if ((args != null) && (args.Results != null))
            {
                //As we searched for a single Service location, we are expecting the results count will be 1
                if (args.Results.Count == 1)
                {
                    foreach (var resultsByObjectId in args.Results)
                    {
                        //Get the OID of Service location for which we queried the related generation records
                        int slOID = resultsByObjectId.Key;
                        foreach (IResultSet result in resultsByObjectId.Value)
                        {
                            //If only one generation record is found, then directly open the settings UI app
                            if (result.Features.Count == 1)
                            {
                                IDataGridCustomButton customButton = _dataGridCustomButtonManager.CustomButtons[((MenuItem)sender).Header.ToString()];
                                customButton.Attributes = result.Features[0].Attributes;
                                //Changes made for system test bug fixing
                                customButton.Open(result.Features[0].Attributes["GLOBALID"].ToString(), "EDGIS.GenerationInfo");
                            }
                            else
                            {
                                //But if we have more than one related generation info records for a single service location, show them in a child window.
                                GenerationInfoWindow genInfoWindow = new GenerationInfoWindow();
                                genInfoWindow.DeviceSettingURL = _deviceSettings.DeviceSettingURL;
                                genInfoWindow.ServiceLocationOID = slOID.ToString();
                                genInfoWindow.InitializeViewModel(result.Features);
                                genInfoWindow.Closed += new EventHandler(genInfoWindow_Closed);
                                genInfoWindow.Show();
                            }
                        }
                    }
                }
                else
                {
                    foreach (var resultsByObjectId in args.Results)
                    {
                        foreach (IResultSet result in resultsByObjectId.Value)
                        {
                            MessageBox.Show("Is This Even Possible ?");
                        }
                    }
                }
            }
        }

        //ENOS2EDGIS, When user closes generation info child window.
        void genInfoWindow_Closed(object sender, EventArgs e)
        {
            GenerationInfoWindow genInfoWindow = sender as GenerationInfoWindow;
        }


        private FeatureLayer GetFeatureLayerFromMapByName(string layerName)
        {
            try
            {
                if (MapControl.Layers.Count > 0)
                {
                    return MapControl.Layers.FirstOrDefault(x => x is FeatureLayer && ((FeatureLayer)x).LayerInfo.Name == layerName) as FeatureLayer;
                }
                return null;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        //*************************************ENOS2EDGIS End*******************************************


        //*************************************ENOSChange Start**********************************

        private void removeExistingCustomButton(KeyValuePair<string, IDataGridCustomButton> dataGridCustomButton)
        {
            if (dataGridCustomButton.Key != "Environment" && dataGridCustomButton.Key != "Identify")
            {
                if ((_graphicLayerContextMenu.Items.Count > 0 && _graphicLayerContextMenu.Items.Any(x => x is MenuItem && ((MenuItem)x).Header.ToString() == dataGridCustomButton.Key)))
                {
                    var item = _graphicLayerContextMenu.Items.FirstOrDefault(x => x is MenuItem && ((MenuItem)x).Header.ToString() == dataGridCustomButton.Key);
                    if (item != null)
                        _graphicLayerContextMenu.Items.Remove(item);
                }
            }
        }
        private void GenFeederGraphicLayer_MouseRightButtonDown(object sender, GraphicMouseButtonEventArgs e)
        {
            e.Handled = true;
        }

        private void GenFeederGraphicLayer_MouseRightButtonUp(object sender, GraphicMouseButtonEventArgs e)
        {
            _hoveredGraphic = e.Graphic;
            _currentGraphicLayerName = e.Graphic.Attributes["LAYERNAME"].ToString();

            if (GetGenOnFeeder.GenFeederLayerName.Contains(_currentGraphicLayerName))
            {
                foreach (KeyValuePair<string, IDataGridCustomButton> dataGridCustomButton in _dataGridCustomButtonManager.CustomButtons)
                {
                    removeExistingCustomButton(dataGridCustomButton);
                    if (dataGridCustomButton.Key == "Generation On Feeder")
                    {
                        if (dataGridCustomButton.Value.Visible)
                        {
                            if (GetGenOnFeeder.GenFeederLayerName.Contains(_currentGraphicLayerName))
                            {
                                //Add 'More Info...' menu item to the graphic layer context menu
                                var genFeederButton = new MenuItem() { Header = dataGridCustomButton.Key };
                                genFeederButton.Visibility = System.Windows.Visibility.Visible;
                                genFeederButton.Click += new RoutedEventHandler(menuItemGenOnFeeder_Click);
                                genFeederButton.IsEnabled = true;
                                //_graphicLayerContextMenu.Items.Add(genFeederButton);
                                //CIT identify
                                if (engineers_group == true)
                                {
                                    _graphicLayerContextMenu.Items.Insert(2, genFeederButton);
                                }
                                else
                                {
                                    _graphicLayerContextMenu.Items.Add(genFeederButton);
                                }
                            }
                        }
                    }
                }
            }
            //CIT identify tool *start
            if (engineers_group == true)
            {
                if (_currentGraphicLayerName.ToUpper() == "PRIMARY UNDERGROUND CONDUCTOR")
                {
                    _currentOBJECTID = Convert.ToInt32(e.Graphic.Attributes["OBJECTID"]);
                    _currentGLOBALID = e.Graphic.Attributes["GLOBALID"].ToString();
                    _currentCircuitName = Convert.ToString(e.Graphic.Attributes["CIRCUITNAME"]);
                    //Conductor_in_Trench obj_check = new Conductor_in_Trench();
                    list_Items = _graphicLayerContextMenu.Items.ToList();
                    for (int count = 0; count < list_Items.Count; count++)
                    {
                        string value = (list_Items[count] as MenuItem).Header.ToString();
                        values_menu.Add(value);
                    }
                    if (!values_menu.Contains("Update FilledDuct"))
                    {
                        var menuItemFilledDuctForGraphic = new MenuItem() { Header = "Update FilledDuct" };
                        menuItemFilledDuctForGraphic.Click += new RoutedEventHandler(menuItemDuctFilled_Click);
                        _graphicLayerContextMenu.Items.Insert(3, menuItemFilledDuctForGraphic);
                    }
                    values_menu.Clear();
                    //obj_check.conduit_Subtype_Value();
                }
            }
            //CIT identify tool *end
            //Set the prepared context menu to graphic layer
            ContextMenuService.SetContextMenu((DependencyObject)sender, _graphicLayerContextMenu);
            _currentGraphicLayer = ((GraphicsLayer)sender);
            _hoveredGraphic = e.Graphic;
            _graphicLayerContextMenu.IsOpen = true;
        }


        public void menuItemShowGeneration_Click(object sender, RoutedEventArgs e)
        {
            // IDataGridCustomButton customButton = _dataGridCustomButtonManager.CustomButtons[((MenuItem)sender).Header.ToString()];
            // customButton.Attributes = _hoveredGraphic.Attributes;
            string globalId = _hoveredGraphic.Attributes["GLOBALID"].ToString();
            // ENOSTOEDGIS - Changes for adding CGC to caption Starts
            try
            {
                _trCGC = Convert.ToString(_hoveredGraphic.Attributes["CGC12"]);
                if (string.IsNullOrEmpty(_trCGC) || string.IsNullOrWhiteSpace(_trCGC))
                {
                    _trCGC = "NA";
                }
            }
            catch (Exception exp)
            {
            }
            // ENOSTOEDGIS - Changes for adding CGC to caption Ends
            if (globalId != null)
            {
                ESRI.ArcGIS.Client.Tasks.Query query = new ESRI.ArcGIS.Client.Tasks.Query(); //query on service point layer          
                query.OutFields.AddRange(new string[] { "GLOBALID", "SERVICEPOINTID", "STREETNUMBER", "STREETNAME1", "STREETNAME2", "CITY", "STATE", "COUNTY" });//Adding City to Address 
                query.Where = "TRANSFORMERGUID='" + globalId + "'";
                QueryTask queryTask = new QueryTask(_servicePointURL);
                queryTask.ExecuteCompleted += new EventHandler<QueryEventArgs>(spQueryTask_ExecuteCompleted);
                queryTask.ExecuteAsync(query);
            }
        }

        void spQueryTask_ExecuteCompleted(object sender, QueryEventArgs e)
        {
            if ((e.FeatureSet).Features.Count > 0)
            {
                // genrationQueryCounter = 0;
                string _servicePtGlobalID = string.Empty;
                servicePointFeatureCount = (e.FeatureSet).Features.Count;

                for (int i = 0; i < (e.FeatureSet).Features.Count; i++)
                {
                    _servicePtGlobalID += "'" + ((e.FeatureSet).Features[i].Attributes)["GLOBALID"].ToString() + "',";

                    servicePointsOnTransformer.Add((e.FeatureSet).Features[i]);
                }
                _servicePtGlobalID = _servicePtGlobalID.Substring(0, _servicePtGlobalID.Length - 1);
                getGenerationInfo(_servicePtGlobalID);

            }
            else
            {
                //ENOSChangeFixes
                MessageBox.Show("No Generation found.", "No Generations Found", MessageBoxButton.OK);
            }
        }

        void getGenerationInfo(string _servicePtGlobalID)
        {
            GetGenTypeDomainValues();
            ESRI.ArcGIS.Client.Tasks.Query query = new ESRI.ArcGIS.Client.Tasks.Query(); //query on generation info layer
            query.ReturnGeometry = true;
            query.OutFields.AddRange(new string[] { "*" });
            query.Where = "SERVICEPOINTGUID IN (" + _servicePtGlobalID + ")";
            QueryTask queryTask = new QueryTask(_generationInfoURL);
            queryTask.ExecuteCompleted += new EventHandler<QueryEventArgs>(genInfoQueryTask_ExecuteCompleted);
            queryTask.Failed += new EventHandler<ESRI.ArcGIS.Client.Tasks.TaskFailedEventArgs>(genInfoQueryTask_ExecuteFailed);
            queryTask.ExecuteAsync(query);
        }

        private void GetGenTypeDomainValues()
        {
            FeatureLayer pFeatureLayer = new FeatureLayer();
            pFeatureLayer.Url = _generationInfoURL;
            pFeatureLayer.Initialized += new EventHandler<EventArgs>(GenInfoFeatureLayer_Initialized);
            pFeatureLayer.Initialize();
        }
        private void GenInfoFeatureLayer_Initialized(object sender, System.EventArgs e)
        {
            FeatureLayer _featureLayer = sender as FeatureLayer;
            FeatureLayerInfo pLayerInfo = _featureLayer.LayerInfo;
            foreach (ESRI.ArcGIS.Client.Field field in pLayerInfo.Fields)
            {
                if (field.Domain != null)
                {
                    if (field.Name.ToUpper() == "GENTYPE")
                    {
                        if (field.Domain is CodedValueDomain)
                        {
                            ConfigUtility.DivisionCodedDomains = field.Domain as CodedValueDomain;
                        }
                    }
                    /****************************ENOS Tariff Change- Start***************************/
                    if (field.Name.ToUpper() == "METHODOFLIMITEDEXPORT")
                    {
                        if (field.Domain is CodedValueDomain)
                        {
                            ConfigUtility.GenInfoLimitedExportCodedDomains = field.Domain as CodedValueDomain;
                        }
                    }
                    /****************************ENOS Tariff Change- End****************************/
                }
            }
        }

        void genInfoQueryTask_ExecuteFailed(object sender, ESRI.ArcGIS.Client.Tasks.TaskFailedEventArgs e)
        {
            ConfigUtility.DivisionCodedDomains = null;
            ConfigUtility.GenInfoLimitedExportCodedDomains = null; //ENOS Tariff Change
        }

        void genInfoQueryTask_ExecuteCompleted(object sender, QueryEventArgs e)
        {
            int isGeneration = 0;
            if ((e.FeatureSet).Features.Count > 0)
            {
                for (int i = 0; i < (e.FeatureSet).Features.Count; i++)
                {
                    generations.Add((e.FeatureSet).Features[i]);
                }
            }
            if (generations.Count > 0)
            {
                //foreach (var spGraphic in servicePointsOnTransformer)
                for (int i = 0; i < servicePointsOnTransformer.Count; i++)
                {
                    //foreach (var graphic in generations)
                    for (int j = 0; j < generations.Count; j++)
                    {

                        if (servicePointsOnTransformer[i].Attributes["GLOBALID"].ToString().Replace("}", "").Replace("{", "").ToUpper() == generations[j].Attributes["SERVICEPOINTGUID"].ToString().Replace("}", "").Replace("{", "").ToUpper())
                        {
                            servicePointsOnTransformer[i].Attributes["GENTYPE"] = ConfigUtility.DivisionCodedDomains.CodedValues[generations[j].Attributes["GENTYPE"].ToString()];
                            double genSize = 0;
                            double machineKW = 0;
                            double inverterKW = 0;
                            if (generations[j].Attributes["EFFRATINGMACHKW"] != null && generations[j].Attributes["EFFRATINGINVKW"] != null)
                            {
                                machineKW = Convert.ToDouble(generations[j].Attributes["EFFRATINGMACHKW"]);
                                inverterKW = Convert.ToDouble(generations[j].Attributes["EFFRATINGINVKW"]);
                            }
                            else if (generations[j].Attributes["EFFRATINGMACHKW"] == null && generations[j].Attributes["EFFRATINGINVKW"] != null)
                            {
                                inverterKW = Convert.ToDouble(generations[j].Attributes["EFFRATINGINVKW"]);
                            }
                            else if (generations[j].Attributes["EFFRATINGMACHKW"] != null && generations[j].Attributes["EFFRATINGINVKW"] == null)
                            {
                                machineKW = Convert.ToDouble(generations[j].Attributes["EFFRATINGMACHKW"]);
                            }
                            genSize = machineKW + inverterKW;
                            servicePointsOnTransformer[i].Attributes["GENSIZE"] = genSize.ToString();
                            servicePointsOnTransformer[i].Attributes["PROJECT/REF"] = generations[j].Attributes["SAPEGINOTIFICATION"];
                            servicePointsOnTransformer[i].Attributes["GENGLOBALID"] = generations[j].Attributes["GLOBALID"];
                            servicePointsOnTransformer[i].Attributes["FEEDERNUMBER"] = null;
                            /****************************ENOS Tariff Change- Start****************************/
                            servicePointsOnTransformer[i].Attributes["DERATED"] = generations[j].Attributes["DERATED"];
                            if (generations[j].Attributes["METHODOFLIMITEDEXPORT"] != null)
                            {
                                servicePointsOnTransformer[i].Attributes["METHODOFLIMITEDEXPORT"] = ConfigUtility.GenInfoLimitedExportCodedDomains.CodedValues[generations[j].Attributes["METHODOFLIMITEDEXPORT"].ToString()];
                            }
                            else
                            {
                                servicePointsOnTransformer[i].Attributes["METHODOFLIMITEDEXPORT"] = generations[j].Attributes["METHODOFLIMITEDEXPORT"];
                            }
                            /****************************ENOS Tariff Change- End****************************/
                            break;
                        }
                        else
                        {
                            isGeneration++;

                        }
                        if (isGeneration == generations.Count)
                        {
                            isGeneration = 0;
                            servicePointsOnTransformer[i].Attributes["GENTYPE"] = null;
                            servicePointsOnTransformer[i].Attributes["GENSIZE"] = null;
                            servicePointsOnTransformer[i].Attributes["PROJECT/REF"] = null;
                            servicePointsOnTransformer[i].Attributes["GENGLOBALID"] = null;
                            servicePointsOnTransformer[i].Attributes["FEEDERNUMBER"] = null;
                        }
                    }

                }
            }
            showGenInfoData(servicePointsOnTransformer);
            //servicePointsOnTransformer.Clear();
            ConfigUtility.DivisionCodedDomains = null;

        }

        //void showGenInfoData(IList<Graphic> generations)
        //{
        //    _mapArea = this.MapArea;
        //    genOnTransformer = new GenOnTransformerTool(_mapArea, generations, _deviceSettings.DeviceSettingURL,true, MapControl);
        //    genOnTransformer.OpenDialog();
        //}
        void showGenInfoData(IList<Graphic> generations)
        {
            _mapArea = this.MapArea;
            /****************************ENOS2SAP PhaseIII Start****************************/
            genOnTransformer = new GenOnTransformerTool(_mapArea, generations, _deviceSettings.DeviceSettingURL, true, MapControl);
            genOnTransformer._generations = servicePointsOnTransformer;
            genOnTransformer._cgcNumber = _trCGC;
            //// ENOSTOEDGIS - Changes for adding CGC to caption                      
            //genOnTransformer.OpenDialog(_trCGC);
            /****************************ENOS2SAP PhaseIII End****************************/
        }
        void menuItemGenOnFeeder_Click(object sender, RoutedEventArgs e)
        {
            ConfigUtility.UpdateStatusBarText("Loading Generation on Feeder...");
            string circuiId = _hoveredGraphic.Attributes["CIRCUITID"].ToString();
            GetGenOnFeeder getGeneration = new GetGenOnFeeder();
            getGeneration.genGenOnFeeder_Click(circuiId);
        }
        //*****************************ENOSChange End******************************************

        private void LoadConfiguration()
        {
            try
            {
                logger.Info("Loading configuration started");
                if (Application.Current.Host.InitParams.ContainsKey("Config"))
                {
                    string config = Application.Current.Host.InitParams["Config"];
                    config = HttpUtility.HtmlDecode(config).Replace("***", ",");
                    XElement element = XElement.Parse(config);
                    LogHelper.InitializeNLog(element);
                    LoggingMenuHelper.SetupHelpMenu(HelpLinkButton, element, this.MapArea);

                    LoadConfiguration(element);
                }
            }
            catch (Exception e)
            {
                logger.FatalException("Error loading configuration: ", e);
            }

            logger.Info("Loading configuration complete");
            string currentUser = WebContext.Current.User.Name;
        }

        private void LoadIdentifyMapping(XElement element)
        {
            ConfigUtility.IdentifyFieldMapping.Clear();

            _rightClickExtentWidth = Convert.ToInt32(element.Attribute("rightClickExtentWidth").Value);

            foreach (XElement childelement in element.Elements())
            {
                var key = "";
                var value = "";
                key = childelement.Attribute("url").Value + "/" + childelement.Attribute("LayerId").Value;
                value = childelement.Attribute("MappingField").Value;
                if (key != null && value != null)
                {
                    ConfigUtility.IdentifyFieldMapping.Add(key, value);
                }
            }
        }

        private void LoadObjClassIDToLayerIDMap(XElement element)
        {
            foreach (XElement mapServerElement in element.Elements())
            {
                XAttribute attribute = mapServerElement.Attribute("Name");
                XAttribute isSchematicsLayer = mapServerElement.Attribute("IsSchematicsLayer");
                bool isSchematics = Boolean.Parse(isSchematicsLayer.Value);
                if (attribute != null)
                {
                    ObjIDToLayerIDMapToBuild.Add(attribute.Value, isSchematics);
                }
            }
        }

        private void LoadConfiguration(XElement config)
        {
            foreach (XElement element in config.Elements())
            {
                try
                {

                    switch (element.Name.LocalName)
                    {
                        case "ObjectClassIDToLayerIDMap":
                            LoadObjClassIDToLayerIDMap(element);
                            break;
                        case "IdentifyMapping":
                            LoadIdentifyMapping(element);
                            break;
                        case "BingKeys":
                            AddBingKeys(element);
                            break;
                        case "Layers":
                            MapControl.Extent = ConfigUtility.EnvelopeFromXElement(element, "Extent");
                            _layerExtent = ConfigUtility.GetExtentLayer(element);
                            AddLayers(element);
                            break;
                        case "RelatedData":
                            AddRelatedData(element);
                            break;
                        case "Searches":
                            AddSearches(element);
                            break;
                        case "SearchableLayers":
                            AddSearchableLayers(element);
                            break;
                        case "Editor":
                            AddEditor(element);
                            break;
                        case "Tracing":
                            AddTracing(element);
                            break;
                        case "AttributeViewer":
                            AddAttributeViewer(element);
                            break;
                        case "Tools":
                            AddTools(element);
                            break;
                        case "LayerVisibility":
                            AddLayerVisibility(element);
                            break;
                        case "GeographicSearchFilter":
                            _gsfConfig = element;
                            break;
                        case "DivisionCodes":
                            ReadDivisionCodes(element);
                            break;
                        case "EditDB":
                            _enableSaveToDB = ConfigUtility.IsItemAllowed(element);
                            break;
                        //************************ENOS2EDGIS Start****************
                        //ENOS2EDGIS , read config section 'ShowMoreInfoOnGraphicLayers' in page.config
                        case "ShowMoreInfoOnGraphicLayers":
                            ReadMoreInfoGraphicLayers(element);
                            break;
                        //*************************ENOS2EDGIS End***************
                        case "WIPEditor":
                            //DA# 200103 ME Q1 2020
                            if (ConfigUtility.IsItemAllowed(element.Element("V6")) || ConfigUtility.IsItemAllowed(element.Element("V5")))
                            {
                                if (ConfigUtility.IsItemAllowed(element.Element("V6")))
                                {
                                    WIPEditor.PLC.IsPLCV6OCalc = true;
                                    WIPEditor.PLC.SelectPole.Visibility = System.Windows.Visibility.Collapsed;
                                    WIPEditor.PLC.NewPole.Visibility = System.Windows.Visibility.Collapsed;
                                    WIPEditor.PLC.CreatePoleMeasure.Visibility = System.Windows.Visibility.Collapsed;
                                    WIPEditor.PLC.CreatePoleLatLong.Visibility = System.Windows.Visibility.Collapsed;
                                    WIPEditor.PLC.MovePole.Visibility = System.Windows.Visibility.Collapsed;
                                    WIPEditor.PLC.DeletePole.Visibility = System.Windows.Visibility.Collapsed;
                                    WIPEditor.PLC.ShowPLDStatus.Visibility = System.Windows.Visibility.Collapsed;
                                }
                                else
                                {
                                    WIPEditor.PLC.IsPLCV6OCalc = false;
                                    WIPEditor.PLC.SelectPole.Visibility = System.Windows.Visibility.Visible;
                                    WIPEditor.PLC.NewPole.Visibility = System.Windows.Visibility.Visible;
                                    WIPEditor.PLC.CreatePoleMeasure.Visibility = System.Windows.Visibility.Visible;
                                    WIPEditor.PLC.CreatePoleLatLong.Visibility = System.Windows.Visibility.Visible;
                                    WIPEditor.PLC.MovePole.Visibility = System.Windows.Visibility.Visible;
                                    WIPEditor.PLC.DeletePole.Visibility = System.Windows.Visibility.Visible;
                                    WIPEditor.PLC.ShowPLDStatus.Visibility = System.Windows.Visibility.Visible;
                                }
                                WIPEditorTab.Visibility = System.Windows.Visibility.Visible;
                            }
                            else
                            {
                                WIPEditorTab.Visibility = System.Windows.Visibility.Collapsed;
                            }
                            if (ConfigUtility.IsItemAllowed(element.Element("V5")))
                            {
                                JETToolVisibility = System.Windows.Visibility.Visible;
                            }

                            break;
                        case "Notes":      //INC000003946726
                            if (ConfigUtility.IsItemAllowed(element))
                            {
                                NotesRibbon.Visibility = System.Windows.Visibility.Visible;
                            }
                            else
                            {
                                NotesRibbon.Visibility = System.Windows.Visibility.Collapsed;
                            }
                            break;
                        case "DWGExporter":
                            if (ConfigUtility.IsItemAllowed(element))
                            {
                                Export.Visibility = System.Windows.Visibility.Visible;
                            }
                            else
                            {
                                Export.Visibility = System.Windows.Visibility.Collapsed;
                            }
                            break;
                        case "StoredView":
                            _defaultStoredView = SetDefaultStoredView(element);
                            break;
                        case "ProposedLayerIDLookups":
                            ReadProposedLayerIDs(element);
                            break;
                        case "CADExport":
                            ReadCADExportProperties(element);
                            break;
                        case "DelineationPrintExporter":
                            if (ConfigUtility.IsItemAllowed(element))
                            {
                                DelineationPrintButton.Visibility = System.Windows.Visibility.Visible;
                                //DelineationPrintPanel.Visibility = System.Windows.Visibility.Visible;
                            }
                            else
                            {
                                DelineationPrintButton.Visibility = System.Windows.Visibility.Collapsed;
                                //DelineationPrintPanel.Visibility = System.Windows.Visibility.Collapsed;
                            }
                            break;
                        case "DelineationPrint":
                            ReadDelineationPrintProperties(element);
                            break;
                        case "DeviceSettings":
                            _deviceSettings.ReadConfiguration(element);
                            break;
                        case "TLM":
                            _tlm.ReadConfiguration(element);
                            break;
                        case "Schematics":
                            SchematicsRibbonPanel.InitializeConfiguration(MapControl, element);
                            break;
                        case "ShowRolloverInfo":
                            //*******************ENOS2EDGIS Start***************************
                            //Enos2EDGIS, sending a new input paramter 'AttributesViewer.RelationshipInformation', we are making this change because,
                            //when user right clicked on any 'Service Location', 'Primary meter' & 'Transformer' , now user can see ' More Info...',
                            //clicking that menu item should open the corresponding SETTINGS UI.
                            _showRolloverInfo = new ShowRolloverInfo(MapControl, element,
                                _dataGridCustomButtonManager.CustomButtons, _tlm, _deviceSettings, Identify, FilterRibbonPanel, AttributesViewer.RelationshipInformation, AttributesViewer, this.MapArea);//INC000004469254 - AttributesViewer added, ENOSChange: this.MapArea added
                            Tools.ShowRolloverInfo = _showRolloverInfo;
                            _showRolloverInfo.ShowLoadingInfoFilter = false;
                            //*******************ENOS2EDGIS End***************************
                            /**********************************ENOSChange Start *************************/
                            _servicePointURL = element.Element("ShowRollOverServicePoint").Attribute("URL").Value;
                            _generationInfoURL = element.Element("ShowRollOverSetingGenInfo").Attribute("URL").Value;
                            ShowGenerationCustomButton.ReadConfiguration(_servicePointURL, _generationInfoURL, this.MapArea, _deviceSettings.DeviceSettingURL);
                            /**********************************ENOSChange End *************************/
                            break;
                        case "ApplicationVersion":
                            _applicationCacheManager.Initialize(element);
                            break;
                        case "LegendTool":
                            _legendConfig = element;
                            break;
                        case "Favorites":
                            Tools.FavoritesControl.Initialize(element, Tools, MapControl);
                            Tools.BookmarksControl.Initialize(element, Tools.StoredViewControl, MapControl);
                            break;
                        case "OutageHistory":
                            OutageHistoryParametersWindow.ReadConfiguration(element);
                            break;
                        case "ToolsFilter":
                            FilterRibbonPanel.InitializeConfiguration(MapControl, element);
                            break;
                        case "Ufm":
                            UfmRibbonPanel.InitializeConfiguration(MapControl, Tools, element, UfmTab, MapArea);
                            break;
                        case "ButterflyDiagram":
                            _butterflyConfig = element;
                            break;
                        case "Jet":
                            //TODO: JET Uncomment
                            JetRibbonPanel.Visibility = JETToolVisibility; //WIPEditorTab.Visibility;           // DA#200103 Me Q1 2020  
                            JetToggleTool.InitializeConfiguration(element, MapControl, MapArea, Tools);
                            break;
                        case "Notification":
                            if (ConfigUtility.IsItemAllowed(element))
                            {
                                Notification.Visibility = System.Windows.Visibility.Visible;
                            }
                            else
                            {
                                Notification.Visibility = System.Windows.Visibility.Collapsed;
                            }
                            break;
                        case "EscapeToPan":     //INC000003956225
                            ReadEscapeToPanProperties(element);
                            break;
                        //*****************PLC Changes RK 07/21/2017*********************//
                        case "PLC":
                            ReadPLCProperties(element);
                            break;
                        //*****************PLC Changes End*********************//
                        case "CMCSTemplate":    //INC000004403856
                            if (ConfigUtility.IsItemAllowed(element))
                            {
                                CMCSTemplateButton.Visibility = System.Windows.Visibility.Visible;
                            }
                            else
                            {
                                CMCSTemplateButton.Visibility = System.Windows.Visibility.Collapsed;
                            }
                            break;
                        case "CMCS":     //INC000004403856
                            ReadCMCSProperties(element);
                            break;
                        case "SearchSingleZoomLevel": //INC000004378902
                            ReadSearchSingleZoomLevelProp(element);
                            break;
                        case "GenerationOnFeeder": //ENOSChange                                                   
                            GetGenOnFeeder.ReadConfiguration(element, this.MapArea, _deviceSettings.DeviceSettingURL, MapControl);
                            GetGeneration.ReadConfiguration(MapControl);
                            FeederFromMap.ReadFeederConfiguration(MapControl, this.MapArea, _deviceSettings.DeviceSettingURL);
                            break;
                        case "GenFeederVisiblity": //ENOSChange
                            if (ConfigUtility.IsItemAllowed(element))
                            {
                                GenerationFeeder.Visibility = System.Windows.Visibility.Visible;
                            }
                            else
                            {
                                GenerationFeeder.Visibility = System.Windows.Visibility.Collapsed;
                            }
                            break;
                        case "AdHocPrint":     //INC000004049426 & INC000004413542
                            _adHocPanVisibility = Convert.ToBoolean(element.Attribute("PanVisibility").Value);
                            _panValue = Convert.ToInt16(element.Attribute("PanValue").Value);
                            break;
                        case "ShowLoadingInfoFilter":
                            if (ConfigUtility.IsItemAllowed(element))
                            {
                                _showRolloverInfo.ShowLoadingInfoFilter = true;
                            }
                            else
                            {
                                _showRolloverInfo.ShowLoadingInfoFilter = false;
                            }
                            break;
                        //CIT engineers filter *start
                        case "CIT_Engineer_Filter":
                            if (ConfigUtility.IsItemAllowed(element))
                            {
                                engineers_group = true;
                            }
                            else
                            {
                                engineers_group = false;
                            }
                            break;
                        //CIT engineers filter *end
                        case "CIT":
                            ReadCITFilledValues(element);
                            _circuitname_CIT = element.Element("PrimaryUG").Attribute("URL").Value;
                            break;
                        case "AddRemoveDataFilter":   //INC000004479909
                            if (ConfigUtility.IsItemAllowed(element))
                            {
                                AddRemoveData.Visibility = System.Windows.Visibility.Visible;
                            }
                            else
                            {
                                AddRemoveData.Visibility = System.Windows.Visibility.Collapsed;
                            }
                            break;
                        case "AddRemoveData":  //INC000004479909
                            _addDataConfig = element;
                            break;
                        case "ShareCurrentURLBtn":    //INC000005346851
                            if (ConfigUtility.IsItemAllowed(element))
                            {
                                Tools.ShareCurrentURLBtn.Visibility = System.Windows.Visibility.Visible;
                                Tools.ShareUrlLine.Visibility = System.Windows.Visibility.Visible;
                            }
                            else
                            {
                                Tools.ShareCurrentURLBtn.Visibility = System.Windows.Visibility.Collapsed;
                                Tools.ShareUrlLine.Visibility = System.Windows.Visibility.Collapsed;
                            }
                            break;
                        case "ShareCurrentUrl": //INC000005346851
                            ReadShareCurrentUrlProp(element);
                            break;

                        case "BannerMessage":
                            populateBannerProperties(element);
                            break;
                        case "LIDARCorrections": //DA #190905 - ME Q1 2020
                            LIDARCorrectionsProperties(element);
                            break;

                            //DA# 191201 ME Q1 2020 - START
                        case "POLFilter":   
                            if (ConfigUtility.IsItemAllowed(element))
                            {
                                POLRibbon.Visibility = System.Windows.Visibility.Visible;
                            }
                            else
                            {
                                POLRibbon.Visibility = System.Windows.Visibility.Collapsed;
                            }
                            break;        
                         case "POL":  
                            _POLConfig = element;
                            break;
                            //DA# 191201 ME Q1 2020 - END

                        default:
                            break;
                    }
                }
                catch (Exception exception)
                {

                    throw;
                }
            }
        }


        //*****************PLC Changes RK 07/21/2017*********************//
        private void ReadPLCProperties(XElement element)
        {
            WIPEditor.PLC.WIPServiceUrl = element.Element("WIPService").Attribute("url").Value;
            WIPEditor.PLC.PMOrderLayerId = element.Element("WIPService").Attribute("pmOrderLayerId").Value;
            WIPEditor.PLC.PLCDynamicServiceUrl = element.Element("PLDService").Attribute("url").Value;
            WIPEditor.PLC.ElevationServiceUrl = element.Element("ElevationGPService").Attribute("url").Value;
            WIPEditor.PLC.SnowLoadServiceUrl = element.Element("SnowLoadDistService").Attribute("url").Value;
            WIPEditor.PLC.SnowLoadLayerId = element.Element("SnowLoadDistService").Attribute("snowLoadLayerId").Value;
            WIPEditor.PLC.GeoServiceUrl = element.Element("GeometryService").Attribute("url").Value;
            WIPEditor.PLC.PLCLayerName = element.Element("PLDService").Attribute("PLCServiceName").Value;
            WIPEditor.PLC.WIPLayerName = element.Element("WIPService").Attribute("WIPServiceName").Value;
            WIPEditor.PLC.PLCFeatureServiceUrl = element.Element("PLDFeatureService").Attribute("url").Value;
            WIPEditor.PLC.PLCLayerId = element.Element("PLDService").Attribute("PLCLayerId").Value;
            WIPEditor.PLC.PGEDataBlock = element.Element("PGEDataBlockFields").Attribute("fields").Value;
            WIPEditor.PLC.AuthorizationEIHeader = element.Element("EIAuthorization").Attribute("value").Value;
            WIPEditor.PLC.StartAnalysisEIServiceUrl = element.Element("StartAnalysisEIService").Attribute("url").Value;
            WIPEditor.PLC.RemovePoleFromOrderEIServiceUrl = element.Element("PoleRemovedFromOrderEIService").Attribute("url").Value;
            WIPEditor.PLC.PLCLabelsLayerName = element.Element("PLCLabelsService").Attribute("PLCLabelsServiceName").Value;
            WIPEditor.PLC.GetZOrderDataServiceUrl = element.Element("CallGetZOrderDataService").Attribute("url").Value;
        }
        //*****************PLC Changes End*********************//

        private void ReadCADExportProperties(XElement element)
        {
            Dictionary<string, string> cadExportScales = new Dictionary<string, string>();
            Dictionary<string, string> cadExportSchematicScales = new Dictionary<string, string>();
            Dictionary<string, string> mapTypes = new Dictionary<string, string>();
            Dictionary<string, string> edLayers = new Dictionary<string, string>();
            Dictionary<string, string> edMasterLayers = new Dictionary<string, string>();
            Dictionary<string, string> landbaseLayer = new Dictionary<string, string>();
            Dictionary<string, string> schematicColors = new Dictionary<string, string>();
            Dictionary<string, string> schematicExportType = new Dictionary<string, string>();
            Dictionary<string, string> edExportType = new Dictionary<string, string>();
            Dictionary<string, string> cadExportPNGSchematicScales = new Dictionary<string, string>();
            List<string> schematicSearchLayers = new List<string>();

            foreach (XElement scaleElement in element.Element("Scales").Elements())
            {
                cadExportScales.Add(scaleElement.Attribute("Level").Value, "1:" + scaleElement.Attribute("Level").Value);
            }

            foreach (XElement scaleElement in element.Element("SchematicScales").Elements())
            {
                cadExportSchematicScales.Add(scaleElement.Attribute("Level").Value, "1:" + scaleElement.Attribute("Level").Value);
            }

            foreach (XElement scaleElement in element.Element("PNGSchematicScales").Elements())
            {
                cadExportPNGSchematicScales.Add(scaleElement.Attribute("Level").Value, "1:" + scaleElement.Attribute("Level").Value);
            }

            //foreach (XElement scaleElement in element.Element("EDExportTypes").Elements())
            //{
            //    edExportType.Add(scaleElement.Attribute("type").Value, scaleElement.Attribute("type").Value);
            //}

            //foreach (XElement scaleElement in element.Element("SchematicExportTypes").Elements())
            //{
            //    schematicExportType.Add(scaleElement.Attribute("type").Value, scaleElement.Attribute("type").Value);
            //}

            foreach (XElement scaleElement in element.Element("MapTypes").Elements())
            {
                mapTypes.Add(scaleElement.Attribute("StoredDisplay").Value, scaleElement.Attribute("StoredDisplay").Value);
            }

            //_cadExportParameters.SchematicExportType = schematicExportType;
            //_cadExportParameters.EDExportType = edExportType; 
            _cadExportParameters.Scales = cadExportScales;
            _cadExportParameters.SchematicScales = cadExportSchematicScales;
            _cadExportParameters.SchematicPNGScales = cadExportPNGSchematicScales;
            _cadExportParameters.MapTypes = mapTypes;
            _cadExportParameters.DefaultScale = element.Element("DefaultScale").Attribute("Level").Value;
            _cadExportParameters.DefaultSchematicScale = element.Element("DefaultSchematicScale").Attribute("Level").Value;
            _cadExportParameters.ExportRESTService = element.Element("ExportRESTService").Attribute("URL").Value;
            _cadExportParameters.ExportWYSWYGRESTService = element.Element("ExportWYSWYGRESTService").Attribute("URL").Value;
            _cadExportParameters.ExportSCHEMATICSRESTService = element.Element("ExportSCHEMATICSRESTService").Attribute("URL").Value;
            _cadExportParameters.SchematicsGridLayer = element.Element("SchematicsGridLayer").Attribute("URL").Value;
            _cadExportParameters.SchematicsGridLayerIds = element.Element("SchematicsGridLayer").Attribute("LayerIds").Value;   //UMG layer divided into 5 - INC000004402997, INC000004402999
            _cadExportParameters.MaximumAreaSquareFeet = Convert.ToInt32(element.Element("MaximumArea").Attribute("SquareFeet").Value);
            _cadExportParameters.CircuitMaximumAreaSquareFeet = Convert.ToInt32(element.Element("CircuitMaximumAreaSquareFeet").Attribute("SquareFeet").Value);
            ////////////////////////////////
            _cadExportParameters.DefaultSchematic250Scale = element.Element("DefaultSchematic250Scale").Attribute("Level").Value;
            // _cadExportParameters.ED50ExportRestService = element.Element("CADED50ExportRESTService").Attribute("URL").Value;
            _cadExportParameters.SchematicsPngDpi = element.Element("SchematicsPNGDpi").Attribute("value").Value;
            _cadExportParameters.SchematicsPNGExportService = element.Element("SchematicPNGExportService").Attribute("URL").Value;
            _cadExportParameters.Schematics250PNGExportService = element.Element("Schematic250PNGExportService").Attribute("URL").Value;
            _cadExportParameters.DCURL = element.Element("DCViewMAPService").Attribute("URL").Value;
            _cadExportParameters.StreetLightURL = element.Element("StreetLightMAPService").Attribute("URL").Value;
            _cadExportParameters.strFMEPNGExportUrl = element.Element("FMEPNGExportURL").Attribute("URL").Value;

            _cadExportParameters.strButterflyLayers = element.Element("PNGButterflyVisibleLayers").Attribute("LayerIDs").Value;
            _cadExportParameters.strLandbaseLayers = element.Element("PNGLandbaseVisibleLayers").Attribute("LayerIDs").Value;

            /////////////////////////////////////

            foreach (XElement scaleElement in element.Element("ElectricDistribution").Elements())
            {
                edLayers.Add(scaleElement.Attribute("CADLayerName").Value, scaleElement.Attribute("EDLayerIds").Value);
            }
            _cadExportParameters.EDLayers = edLayers;


            foreach (XElement scaleElement in element.Element("EDEDMaster").Elements())
            {
                edMasterLayers.Add(scaleElement.Attribute("CADLayerName").Value, scaleElement.Attribute("EDLayerIds").Value);
            }
            _cadExportParameters.EDMasterLayers = edMasterLayers;


            foreach (XElement scaleElement in element.Element("Landbase").Elements())
            {
                landbaseLayer.Add(scaleElement.Attribute("CADLayerName").Value, scaleElement.Attribute("EDLayerIds").Value);
            }
            _cadExportParameters.LandbaseLayers = landbaseLayer;

            foreach (XElement scaleElement in element.Element("SchematicColor").Elements())
            {
                schematicColors.Add(scaleElement.Attribute("code").Value, scaleElement.Attribute("value").Value);
            }
            _cadExportParameters.SchematicColors = schematicColors;

            foreach (XElement scaleElement in element.Element("SchematicSearch").Elements())
            {
                schematicSearchLayers.Add(scaleElement.Attribute("url").Value);
            }
            _cadExportParameters.SchematicSearchLayers = schematicSearchLayers;


            _cadExportParameters.BufferGeometryFeet = Convert.ToInt32(element.Element("BufferGeometry").Attribute("Feet").Value);
            _cadExportParameters.SchematicExportPath = element.Element("SchematicExportPath").Attribute("Path").Value;
            _cadExportParameters.SchematicSDEFilePath = element.Element("SchematicSDEFilePath").Attribute("Path").Value;
            _cadExportParameters.SchematicGPService = element.Element("SchematicGPService").Attribute("URL").Value;
            _cadExportParameters.SchematicQueryScale = Convert.ToDouble(element.Element("SchematicQueryScale").Attribute("scale").Value);

            _cadExportParameters.AnnotationElements = element;

        }

        //*******************ENOS2EDGIS Start****************************
        private void ReadMoreInfoGraphicLayers(XElement element)
        {
            foreach (XElement moreInfoLayer in element.Elements())
            {
                _graphicsMoreInfo.Add(moreInfoLayer.Attribute("LayerName").Value, new KeyValuePair<string, string>(moreInfoLayer.Attribute("url").Value, moreInfoLayer.Attribute("ID").Value));
            }
        }
        //******************ENOS2EDGIS End******************************
        private void ReadDelineationPrintProperties(XElement element)
        {
            Dictionary<string, string> layouts = new Dictionary<string, string>();

            _delineationPrintParameters.DelineationPrintService = element.Element("DelineationPrintService").Attribute("URL").Value;
            if (string.IsNullOrEmpty(_delineationPrintParameters.DelineationPrintService))
            {
                Layer layer = new FeatureLayer { Url = _delineationPrintParameters.DelineationPrintService };// ConfigUtility.LayerFromXElement(layerElement);
                layer.Initialized += new EventHandler<EventArgs>(ServiceLayer_Initialized);
                layer.Visible = false;// Convert.ToBoolean(layerElement.Attribute("Visible").Value);
                ((FeatureLayer)layer).PropertyChanged += new PropertyChangedEventHandler(ServiceLayer_PropertyChanged);

                if (layer != null)
                {
                    MapControl.Layers.Insert(MapControl.Layers.Count, layer);
                    _loadedServiceLayerCount++;
                    logger.Info("Layer Added: " + layer.ID);
                }
                //GetStoredViews(layerElement);
            }

            foreach (XElement layoutElement in element.Element("Layouts").Elements())
            {
                layouts.Add(layoutElement.Attribute("Name").Value, layoutElement.Attribute("Name").Value);
            }
            _delineationPrintParameters.Layouts = layouts;
            _delineationPrintParameters.DefaultLayout = element.Element("DefaultLayout").Attribute("Name").Value;

            //_delineationPrintParameters.BufferGeometryFeet = Convert.ToInt32(element.Element("BufferGeometry").Attribute("Feet").Value);
            //_delineationPrintParameters.CircuitMaximumAreaSquareFeet = Convert.ToInt32(element.Element("CircuitMaximumAreaSquareFeet").Attribute("SquareFeet").Value);
            _delineationPrintParameters.MaximumAreaSquareFeet = Convert.ToInt32(element.Element("MaximumArea").Attribute("SquareFeet").Value);
            //_delineationPrintParameters.GeometryService = element.Element("GeometryService").Attribute("URL").Value;
            //_delineationPrintParameters.WarningScale = Convert.ToInt32(element.Element("Warning").Attribute("Scale").Value);
            _delineationPrintParameters.WarningMessage = element.Element("Warning").Attribute("Message").Value;
        }

        private string SetDefaultStoredView(XElement element)
        {
            if (element == null) return string.Empty;
            XAttribute attr = element.Attribute("DefaultStoredDisplay");
            if (attr == null) return string.Empty;
            if (string.IsNullOrEmpty(attr.Value)) return string.Empty;
            return attr.Value;
        }

        private void ReprojectExtentStart()
        {
            if (_layerExtent == string.Empty) return;

            // get layer from map control
            Layer layer = null;
            foreach (Layer temp in MapControl.Layers)
            {
                if (temp.ID == _layerExtent)
                {
                    layer = temp;
                    break;
                }
            }
            if (layer == null) return;

            // layers need to be initialized to get the spatial reference.
            // If a layer is not initialized, add event handler for 
            // Layer.Initialized event.

            // get spatial reference from layer
            if (layer.IsInitialized == true)
            {
                _extentSpatialReference = layer.SpatialReference;
            }
            else
            {
                layer.Initialized += ExtentLayerInitialized;
            }

            // get spatial reference from base map
            layer = MapControl.Layers[0];
            if (layer.IsInitialized == true)
            {
                _baseMapSpatialReference = layer.SpatialReference;
            }
            else
            {
                layer.Initialized += ExtentLayerInitialized;
            }

            // if both layers  are initialized, reproject extents, otherwise wait 
            // until they are initialized. ReprojectExtent() will be called
            // in Layer.Initialzed event handler when  both layers are initialized.
            if (_extentSpatialReference != null && _baseMapSpatialReference != null)
            {
                ReprojectExtent();
            }
        }

        private void ReprojectExtent()
        {
            Envelope extents = MapControl.Extent;

            // add min point to list
            MapPoint mapPoint = new MapPoint(extents.XMin, extents.YMin, _extentSpatialReference);
            var graphic = new Graphic() { Geometry = mapPoint };
            var graphics = new List<Graphic> { graphic };

            // add max point to list
            mapPoint = new MapPoint(extents.XMax, extents.YMax, _extentSpatialReference);
            graphic = new Graphic() { Geometry = mapPoint };
            graphics.Add(graphic);

            // use geometry service to reproject extent
            GeometryService geoServ = new GeometryService(MeasureTool.GeometryService);
            geoServ.ProjectCompleted += GeoServProjectComplete;
            geoServ.ProjectAsync(graphics, _baseMapSpatialReference);
        }

        private void ExtentLayerInitialized(object sender, EventArgs e)
        {
            var layer = sender as Layer;

            if (layer.ID == _layerExtent)
            {
                if (layer.InitializationFailure == null)
                {
                    _extentSpatialReference = layer.SpatialReference;
                }
            }
            else
            {
                // basemap
                if (layer.InitializationFailure == null)
                {
                    _baseMapSpatialReference = layer.SpatialReference;
                }
                else
                {
                    // layer failed to initialize. Get next layer and try again
                    var baseLayer = MapControl.Layers.FirstOrDefault(l => l.InitializationFailure == null);
                    if (baseLayer != null)
                    {
                        if (baseLayer.IsInitialized)
                        {
                            _baseMapSpatialReference = baseLayer.SpatialReference;
                        }
                        else
                        {
                            _baseMapSpatialReference = null;
                            baseLayer.Initialized += ExtentLayerInitialized;
                        }
                    }
                }
            }

            // if both layers have been initialzed, project map extent
            if (_extentSpatialReference != null && _baseMapSpatialReference != null)
            {
                ReprojectExtent();
            }
        }


        private void GeoServProjectComplete(object sender, ESRI.ArcGIS.Client.Tasks.GraphicsEventArgs args)
        {
            if (args.Results.Count != 2) return;

            var results = args.Results;
            Graphic minGraphic = results[0];
            Graphic maxGraphic = results[1];
            Envelope envelope = new Envelope(minGraphic.Geometry.Extent.XMin,
                minGraphic.Geometry.Extent.YMin, maxGraphic.Geometry.Extent.XMax,
                maxGraphic.Geometry.Extent.YMax);
            MapControl.Extent = envelope;

            Tools.InsetMap.ZoomTo(envelope);
        }

        private void AddLayerVisibility(XElement layerVisibility)
        {
            if (layerVisibility == null) return;
            if (layerVisibility.HasElements == false) return;

            foreach (XElement layerVisibilityElement in layerVisibility.Elements())
            {
                XName name = layerVisibilityElement.Name;

                switch (name.LocalName)
                {
                    case "ClientSideVisibility":
                        XAttribute visible = layerVisibilityElement.Attribute("Visible");
                        var clientSideVisibility = bool.Parse(visible.Value);

                        Tools.LayerControl.UseClientSideVisibility = clientSideVisibility;
                        Tools.MapInset.ViewModel.UseClientSideVisibility = clientSideVisibility;
                        Identify.UseClientLayerVisibility = clientSideVisibility;

                        XAttribute hideItems = layerVisibilityElement.Attribute("ScaleRange");
                        var hideInvisibleItems = bool.Parse(hideItems.Value);

                        Identify.HideInvisibleItems = hideInvisibleItems;
                        break;
                    default:
                        break;
                }
            }
        }

        private void AddTools(XElement toolsElement)
        {
            if (toolsElement == null) return;
            if (toolsElement.HasElements == false) return;

            foreach (XElement toolElement in toolsElement.Elements())
            {
                XName name = toolElement.Name;

                switch (name.LocalName)
                {
                    case "Measure":
                        XAttribute projection = toolElement.Attribute("ProjectionWKID");
                        MeasureTool.MeasuredProjection = int.Parse(projection.Value);
                        break;
                    case "GeometryService":
                        XAttribute url = toolElement.Attribute("Url");
                        if (IsValidAbsoluteUrl(url.Value) == true)
                        {
                            MeasureTool.GeometryService = url.Value;
                            CoordinatesTool.GeometryService = url.Value;
                            Editor.GeometryService = url.Value;
                            AttributesViewer.GeometryService = url.Value;
                            Locate.GeometryService = url.Value;
                            WIPEditor.GeometryServiceUrl = url.Value;
                            ConfigUtility.GeometryServiceURL = url.Value;
                        }
                        break;
                    case "RuralViewCommonLandbase":
                        Tools.RuralView_CommonLandBase_LayerID = Convert.ToInt32(toolElement.Attribute("LayerId").Value);
                        Tools.StoredViewControl.ED_CommonLandBase_LayerID = Convert.ToInt32(toolElement.Attribute("LayerId").Value);
                        break;
                    default:
                        break;
                }
            }
        }

        private bool IsValidAbsoluteUrl(string url)
        {
            if (string.IsNullOrEmpty(url) == true) return false;

            bool isValid = true;

            try
            {
                Uri uriTest = new Uri(url);
            }
            catch (UriFormatException)
            {
                isValid = false;
            }

            return isValid;
        }

        private void AddAttributeViewer(XElement attributeViewer)
        {
            if (attributeViewer == null) return;
            if (attributeViewer.HasElements == false) return;

            foreach (XElement attributeViewerElement in attributeViewer.Elements())
            {
                switch (attributeViewerElement.Name.LocalName)
                {
                    case "Selection":
                        XAttribute color = attributeViewerElement.Attribute("SelectionColor");
                        string template = "<SolidColorBrush xmlns='http://schemas.microsoft.com/client/2007'";
                        template += " xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml'";
                        template += " Color='" + color.Value + "'/>";
                        SolidColorBrush brush = (SolidColorBrush)XamlReader.Load(template);
                        AttributesViewer.SelectionColor = brush;

                        XAttribute size = attributeViewerElement.Attribute("MarkerSize");
                        AttributesViewer.UnscaledMarkerSize = Tools.UnscaledMarkerSize = _unscaledMarkerSize = double.Parse(size.Value);
                        break;
                    case "ZoomBuffer":
                        XAttribute sizeAttribute = attributeViewerElement.Attribute("Size");
                        double buffSize = 0;
                        if (sizeAttribute != null)
                        {
                            if (double.TryParse(sizeAttribute.Value, out buffSize) == true)
                            {
                                AttributesViewer.ZoomBuffer = buffSize;
                            }
                        }
                        break;
                    case "FieldSequence":    //INC000004380375
                        XAttribute urlAttribute = attributeViewerElement.Attribute("Url");
                        if (urlAttribute != null)
                        {
                            AttributesViewer.FieldSequenceUrl = urlAttribute.Value;
                        }
                        break;
                };
            }
        }

        private bool loadedPublicationDomainMapping = false;
        private void AddTracing(XElement tracing)
        {
            if (tracing.HasElements == false)
            {
                TracingTab.Visibility = Visibility.Collapsed;
                return;
            }

            double tolerance = 0;
            XAttribute toleranceAttribute = tracing.Attribute("Tolerance");
            if (toleranceAttribute != null)
            {
                if (double.TryParse(toleranceAttribute.Value, out tolerance) == true)
                {
                    ElectricTrace.Tolerance = tolerance;
                    GasTrace.Tolerance = tolerance;
                    WaterTrace.Tolerance = tolerance;
                    GasTraceTemporaryMarks.Tolerance = tolerance;
                    WaterTraceTemporaryMarks.Tolerance = tolerance;
                }

            }

            foreach (XElement traceElement in tracing.Elements())
            {
                XName name = traceElement.Name;
                XAttribute MapServiceID = traceElement.Attribute("MapServiceName");
                toleranceAttribute = traceElement.Attribute("Tolerance");

                switch (name.LocalName)
                {
                    case "ClassIDToLayerIDMap":
                        foreach (XElement ClassIDMappingUrlElement in traceElement.Elements())
                        {
                            XAttribute MapServerURL = ClassIDMappingUrlElement.Attribute("value");
                            string URL = MapServerURL.Value.ToString();
                            foreach (XElement classIDToLayerIdElement in ClassIDMappingUrlElement.Elements())
                            {
                                XAttribute ClassID = classIDToLayerIdElement.Attribute("ClassID");
                                XAttribute LayerIDs = classIDToLayerIdElement.Attribute("LayerIDs");

                                int classID = -1;
                                try
                                {
                                    classID = Int32.Parse(ClassID.Value);
                                }
                                catch { }

                                string layerIDs = LayerIDs.Value;
                                string[] IDs = Regex.Split(layerIDs, ",");

                                foreach (string id in IDs)
                                {
                                    int layerID = Int32.Parse(id);
                                    ConfigUtility.AddLayerIDToMap(URL, classID, layerID);
                                }
                            }
                        }
                        break;
                    case "PGETraces":
                        foreach (XElement pgeTracingElement in traceElement.Elements())
                        {
                            XName name2 = pgeTracingElement.Name;
                            XAttribute tracingTableURL = pgeTracingElement.Attribute("TracingTableURL");
                            XAttribute tracingTableLayerID = pgeTracingElement.Attribute("LayerID");
                            switch (name2.LocalName)
                            {
                                case "PGEProtectiveDevices":
                                    foreach (XElement protDeviceElement in pgeTracingElement.Elements())
                                    {
                                        XAttribute Name = protDeviceElement.Attribute("Name");
                                        XAttribute ClassID = protDeviceElement.Attribute("ClassID");
                                        XAttribute SchemClassID = protDeviceElement.Attribute("SchemClassID");

                                        int classID = -1;
                                        try
                                        {
                                            classID = Int32.Parse(ClassID.Value);
                                            TracingHelper.PGEProtectiveDevicesByClassID.Add(Name.Value, classID);
                                        }
                                        catch { }

                                        try
                                        {
                                            int schemClassID = Int32.Parse(SchemClassID.Value);
                                            TracingHelper.PGESchemProtDeviceLookup.Add(classID, schemClassID);
                                        }
                                        catch { }
                                        /*
                                        CheckBox newProtDevice = new CheckBox();
                                        newProtDevice.Content = Name.Value;
                                        ProtectiveDevicesStackPanel.Children.Add(newProtDevice);
                                        */
                                    }

                                    break;
                                case "PGEDefaultVisibleClasses":
                                    foreach (XElement protDeviceElement in pgeTracingElement.Elements())
                                    {
                                        XAttribute Name = protDeviceElement.Attribute("Name");
                                        XAttribute ClassID = protDeviceElement.Attribute("ClassID");
                                        XAttribute SchemClassID = protDeviceElement.Attribute("SchemClassID");

                                        try
                                        {
                                            int classID = Int32.Parse(ClassID.Value);
                                            TracingHelper.DefaultVisibleClassIDs.Add(classID);
                                        }
                                        catch { }
                                        try
                                        {
                                            int schemClassID = Int32.Parse(SchemClassID.Value);
                                            TracingHelper.DefaultVisibleClassIDs.Add(schemClassID);
                                        }
                                        catch { }
                                    }
                                    break;
                                case "PGEResultsInformation":
                                    foreach (XElement traceResultInformation in pgeTracingElement.Elements())
                                    {
                                        XAttribute Name = traceResultInformation.Attribute("Name");
                                        XAttribute ClassID = traceResultInformation.Attribute("ClassID");
                                        XAttribute FieldNames = traceResultInformation.Attribute("FieldNames");
                                        XAttribute ShowSubtype = traceResultInformation.Attribute("DisplaySubtype");
                                        int classID = -1;
                                        try
                                        {
                                            classID = Int32.Parse(ClassID.Value);
                                            if (ShowSubtype.Value.ToUpper() == "TRUE") { TracingHelper.ClassIDsToDisplaySubtype.Add(classID); }

                                            //ME Q4 2019 Release DA# 190806
                                            //1. When DisplaySubtype is false, feature layer name will be shown in tracing results.
                                            //2. When DisplaySubtype is true, it will check for CustomDisplayField. If CustomDisplayField is not present, it will show subtype name.
                                            //3. If DisplaySubtype is true and CustomDisplayField is present, custom display field will be shown for the codes mentioned in DisplayFieldCode attribute.
                                            //4. If DisplaySubtype is true and CustomDisplayField is present without DisplayFieldCode attribute, CustomDisplayField value will be shown for all codes.
                                            if (traceResultInformation.Attribute("CustomDisplayField") != null)
                                            {
                                                string customDisplayField = traceResultInformation.Attribute("CustomDisplayField").Value.ToString();
                                                if (traceResultInformation.Attribute("DisplayFieldCode") != null)
                                                {
                                                    string[] displayFieldCodes = Regex.Split(traceResultInformation.Attribute("DisplayFieldCode").Value, ",");
                                                    foreach (string domainCode in displayFieldCodes)
                                                    {
                                                        try
                                                        {
                                                            TracingHelper.PGETraceResultCustomDisplayField[classID + "-" + customDisplayField].Add(domainCode.ToUpper());
                                                        }
                                                        catch
                                                        {
                                                            TracingHelper.PGETraceResultCustomDisplayField.Add(classID + "-" + customDisplayField, new List<string>());
                                                            TracingHelper.PGETraceResultCustomDisplayField[classID + "-" + customDisplayField].Add(domainCode.ToUpper());
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    if (!TracingHelper.PGETraceResultCustomDisplayField.ContainsKey(classID + "-" + customDisplayField))
                                                        TracingHelper.PGETraceResultCustomDisplayField.Add(classID + "-" + customDisplayField, new List<string>());
                                                }
                                            }

                                            string[] fieldNames = Regex.Split(FieldNames.Value, ",");
                                            foreach (string fieldName in fieldNames)
                                            {
                                                try
                                                {
                                                    TracingHelper.PGETraceResultFieldsByClassID[classID].Add(fieldName.ToUpper());
                                                }
                                                catch
                                                {
                                                    TracingHelper.PGETraceResultFieldsByClassID.Add(classID, new List<string>());
                                                    TracingHelper.PGETraceResultFieldsByClassID[classID].Add(fieldName.ToUpper());
                                                }
                                            }
                                        }
                                        catch { }

                                        //Process the relatedtablecount sections
                                        foreach (XElement relRecordCheck in traceResultInformation.Elements())
                                        {
                                            foreach (XElement relTable in relRecordCheck.Elements())
                                            {
                                                XAttribute relName = relTable.Attribute("Name");
                                                XAttribute relFieldName = relTable.Attribute("FieldName");
                                                XAttribute relFieldValue = relTable.Attribute("FieldValues");

                                                TracingHelper.TraceRelRecordCheck newRecordCheck = new TracingHelper.TraceRelRecordCheck();
                                                newRecordCheck.RelationshipName = relName.Value;
                                                newRecordCheck.FieldName = relFieldName.Value;
                                                Dictionary<int, string> newFieldValues = new Dictionary<int, string>();
                                                string[] fieldValues = Regex.Split(relFieldValue.Value, ";");
                                                foreach (string fieldValue in fieldValues)
                                                {
                                                    string[] values = Regex.Split(fieldValue, ",");
                                                    newFieldValues.Add(Int32.Parse(values[0]), values[1]);
                                                }
                                                newRecordCheck.FieldValues = newFieldValues;
                                                TracingHelper.PGETraceResultRelatedCountsByClassID.Add(classID, newRecordCheck);
                                            }
                                        }
                                    }
                                    break;
                                case "PGELoadingInformation":
                                    TracingHelper.LoadingInformationTracingTableURL = tracingTableURL.Value;
                                    TracingHelper.LoadingInformationTracingTableLayerID = tracingTableLayerID.Value;
                                    foreach (XElement LoadingInformationElement in pgeTracingElement.Elements())
                                    {
                                        if (LoadingInformationElement.Name == "LoadingInformation")
                                        {
                                            //Add TransformerLayerID and PrimaryMeterLayerID for INC000003997867
                                            XAttribute faultDutyIbalLayerID = LoadingInformationElement.Element("FaultDutyIbal").Attribute("LayerId");
                                            XAttribute servicePointLayerID = LoadingInformationElement.Element("ServicePoint").Attribute("LayerId");
                                            //****************ENOS2EDGIS Start****************************
                                            //XAttribute generationLayerID = LoadingInformationElement.Element("Generation").Attribute("LayerId");
                                            //****************ENOS2EDGIS End****************************
                                            XAttribute serviceLocationFCID = LoadingInformationElement.Attribute("ServiceLocationFCID");
                                            XAttribute TransformerFCID = LoadingInformationElement.Attribute("TransformerFCID");
                                            XAttribute PrimaryGenFCID = LoadingInformationElement.Attribute("PrimaryGenerationFCID");
                                            XAttribute SecondaryGenFCID = LoadingInformationElement.Attribute("SecondaryGenerationFCID");
                                            XAttribute TransformerUnitLayerID = LoadingInformationElement.Element("TransformerUnit").Attribute("LayerId");
                                            XAttribute TransformerLayerID = LoadingInformationElement.Element("Transformer").Attribute("LayerId");
                                            XAttribute PrimaryMeterLayerID = LoadingInformationElement.Element("PrimaryMeter").Attribute("LayerId");

                                            //***************ENOS2EDGIS Start*******************
                                            //ENOS2EDGIS, Added GenerationInfo LayerId
                                            XAttribute GenerationInfoID = LoadingInformationElement.Element("GenerationInfo").Attribute("LayerId");
                                            //***************ENOS2EDGIS End*********************

                                            LoadingInformation.FaultDutyIbalLayerID = faultDutyIbalLayerID.Value;
                                            LoadingInformation.ServicePointLayerID = servicePointLayerID.Value;
                                            //***************ENOS2EDGIS Start*******************
                                            //LoadingInformation.GenerationLayerID = generationLayerID.Value;
                                            //***************ENOS2EDGIS End*******************
                                            LoadingInformation.ServiceLocationFCID = serviceLocationFCID.Value;
                                            LoadingInformation.TransformerFCID = TransformerFCID.Value;
                                            LoadingInformation.PrimaryGenerationFCID = PrimaryGenFCID.Value;
                                            LoadingInformation.SecondaryGenerationFCID = SecondaryGenFCID.Value;
                                            LoadingInformation.TransformerUnitLayerID = TransformerUnitLayerID.Value;
                                            LoadingInformation.FDILayersCSV = LoadingInformationElement.Attribute("FdiLayerNamesCSV").Value;
                                            LoadingInformation.TransformerLayerID = TransformerLayerID.Value;
                                            LoadingInformation.PrimaryMeterLayerID = PrimaryMeterLayerID.Value;

                                            //***************ENOS2EDGIS Start*******************
                                            //ENOS2EDGIS, Assigned GenerationInfoLayerId to LoadingInformation class GenerationInfoLayerID value
                                            LoadingInformation.GenerationInfoLayerID = GenerationInfoID.Value;
                                            //***************ENOS2EDGIS End*********************
                                        }
                                    }
                                    break;
                                case "PGEUndergroundTraces":
                                    TracingHelper.UndergroundNetworkURL = tracingTableURL.Value;
                                    TracingHelper.UndergroundNetworkLayerID = tracingTableLayerID.Value;
                                    XAttribute conduitFCID = pgeTracingElement.Attribute("ConduitFCID");
                                    TracingHelper.ConduitFCID = conduitFCID.Value;
                                    XAttribute subsurfaceStructureFCID = pgeTracingElement.Attribute("SubsurfaceStructureFCID");
                                    TracingHelper.SubsurfaceStructureFCID = subsurfaceStructureFCID.Value;
                                    XAttribute subsurfaceStructureSubtypes = pgeTracingElement.Attribute("Subtypes");
                                    TracingHelper.UndergroundSubsurfaceStructureSubtypes = Regex.Split(subsurfaceStructureSubtypes.Value, ",").ToList();
                                    break;
                                case "PGEElectricTraces":
                                    TracingHelper.PGEElectricTracingURL = tracingTableURL.Value;
                                    TracingHelper.PGEElecCachedTracingTableLayerID = tracingTableLayerID.Value;
                                    foreach (XElement elecTracingElement in pgeTracingElement.Elements())
                                    {
                                        XAttribute urlID = elecTracingElement.Attribute("Url");
                                        TracingHelper.PGEElectricTracingURLs.Add(urlID.Value);
                                    }
                                    if (!loadedPublicationDomainMapping)
                                    {
                                        ArcGISDynamicMapServiceLayer publicationMapService = new ArcGISDynamicMapServiceLayer();
                                        publicationMapService.Url = TracingHelper.PGEElectricTracingURL;
                                        publicationMapService.GetAllDetails(GetPublicationDomainMapping);
                                        loadedPublicationDomainMapping = true;
                                    }
                                    break;
                                case "PGESubstationTraces":
                                    TracingHelper.PGESubstationTracingURL = tracingTableURL.Value;
                                    TracingHelper.PGESubCachedTracingTableLayerID = tracingTableLayerID.Value;
                                    foreach (XElement subTracingElement in pgeTracingElement.Elements())
                                    {
                                        XAttribute urlID = subTracingElement.Attribute("Url");
                                        TracingHelper.PGESubstationTracingURLs.Add(urlID.Value);
                                    }
                                    break;
                                case "PGESchematicsTraces":
                                    foreach (XElement schemTracingElement in pgeTracingElement.Elements())
                                    {
                                        XName schameEleName = schemTracingElement.Name;
                                        XAttribute urlID = schemTracingElement.Attribute("Url");
                                        XAttribute SchemElecTracesUrl = schemTracingElement.Attribute("ElectricTracesUrl");
                                        switch (schameEleName.LocalName)
                                        {
                                            case "PGESchematicsTrace":
                                                TracingHelper.PGESchemTracingURLs.Add(urlID.Value);
                                                break;
                                        }
                                    }
                                    break;
                                //TraceResultLayerAliasName
                                case "TraceResultLayerAliasName":
                                    foreach (XElement schemTracingElement in pgeTracingElement.Elements())
                                    {
                                        XName schameEleName = schemTracingElement.Name;
                                        XAttribute clsID = schemTracingElement.Attribute("ClassID");
                                        XAttribute lyrName = schemTracingElement.Attribute("Name");
                                        XAttribute lyrAliasName = schemTracingElement.Attribute("AliasName");
                                        TracingHelper.PGETraceResultLayerAliasNames.Add(clsID.Value, lyrName.Value + ";" + lyrAliasName.Value);
                                    }
                                    break;
                            }
                        }
                        InitTraceSettings();
                        break;
                    case "ElectricTrace":
                        SetElectricTraceServices(MapServiceID.Value);
                        if ((toleranceAttribute != null) && (double.TryParse(toleranceAttribute.Value, out tolerance) == true))
                        {
                            ElectricTrace.Tolerance = tolerance;
                        }
                        break;
                    case "GasTrace":
                        GasTrace.GasTraceService = MapServiceID.Value;
                        GasTraceTemporaryMarks.GasTraceService = MapServiceID.Value;
                        if ((toleranceAttribute != null) && (double.TryParse(toleranceAttribute.Value, out tolerance) == true))
                        {
                            GasTrace.Tolerance = tolerance;
                            GasTraceTemporaryMarks.Tolerance = tolerance;
                        }
                        break;
                    case "WaterTrace":
                        WaterTrace.WaterTraceService = MapServiceID.Value;
                        WaterTraceTemporaryMarks.WaterTraceService = MapServiceID.Value;
                        if ((toleranceAttribute != null) && (double.TryParse(toleranceAttribute.Value, out tolerance) == true))
                        {
                            WaterTrace.Tolerance = tolerance;
                            WaterTraceTemporaryMarks.Tolerance = tolerance;
                        }
                        break;
                }
            }

            //If the publication class ID to layer ID map was just built, we need to initialize the tracing layer now.
            if (ConfigUtility.ObjectClassIDMapping.ContainsKey(TracingHelper.LoadingInformationTracingTableURL))
            {
                ConfigUtility.tracingLayer = new ArcGISDynamicMapServiceLayer();
                ConfigUtility.tracingLayer.Url = TracingHelper.LoadingInformationTracingTableURL;
                ConfigUtility.tracingLayer.Initialize();
            }

            SetTracingTabVisibility();
        }

        private void SetElectricTraceServices(string layerIDs)
        {
            if (string.IsNullOrEmpty(layerIDs)) return;

            List<string> electricTraceServices = new List<string>();

            char[] delimiterChars = { ',', ':', ';' };
            string[] layers = layerIDs.Split(delimiterChars);
            bool processedTraceLayer = false;
            foreach (string layer in layers)
            {
                if (string.IsNullOrEmpty(layer) == true) continue;
                electricTraceServices.Add(layer);
                if (processedTraceLayer == false)
                {
                    ElectricTrace.ElectricTraceService = layer;
                    processedTraceLayer = true;
                }

            }

            Tools.StoredViewControl.ElectricTrace = ElectricTrace;
            Tools.StoredViewControl.ElectricTraceLayers = electricTraceServices;
        }

        private void SetTracingTabVisibility()
        {
            bool electricTraceConfigured = MapControl.Layers[ElectricTrace.ElectricTraceService] != null;
            bool waterTraceConfigured = MapControl.Layers[WaterTrace.WaterTraceService] != null;
            bool gasTraceConfigured = MapControl.Layers[GasTrace.GasTraceService] != null;

            if (electricTraceConfigured == true || gasTraceConfigured == true || waterTraceConfigured == true)
            {
                TracingTab.Visibility = Visibility.Visible;
                if (electricTraceConfigured == false)
                {
                    ElectricTraceRibbon.Visibility = Visibility.Collapsed;
                }
                if (gasTraceConfigured == false)
                {
                    GasTraceRibbon.Visibility = Visibility.Collapsed;
                    GasTraceTemporaryMarksRibbon.Visibility = Visibility.Collapsed;
                }
                if (waterTraceConfigured == false)
                {
                    WaterTraceRibbon.Visibility = Visibility.Collapsed;
                    WaterTraceTemporaryMarksRibbon.Visibility = Visibility.Collapsed;
                }
            }
            else
            {
                TracingTab.Visibility = Visibility.Collapsed;
            }
        }

        private void SetEditingTabVisibility()
        {
            foreach (var layer in MapControl.Layers)
            {
                if (layer is FeatureLayer)
                {
                    EditingTab.Visibility = Visibility.Visible;
                    break;
                }
            }
        }

        private void AddEditor(XElement editor)
        {
            if (editor.HasElements == false) return;

            if (_waitingForWebmap)
            {
                // A webmap is being used as the base map and no layers have been loaded yet.
                // Wait until layers have been loaded.
                _editorConfig = editor;
                return;
            }

            foreach (XElement editorElement in editor.Elements())
            {
                switch (editorElement.Name.LocalName)
                {
                    case "EditorLayer":
                        var layerIDs = new List<string>();
                        var hasFeaturelayers = false;

                        XAttribute attribute = editorElement.Attribute("PointServiceName");
                        if (attribute != null)
                        {
                            var pointLayerIDs = attribute.Value.Split(',');
                            layerIDs.AddRange(pointLayerIDs);

                            if (pointLayerIDs.Any())
                            {
                                _pointLayer = MapControl.Layers[pointLayerIDs[0]] as GraphicsLayer;
                                if (_pointLayer != null)
                                {
                                    AttributesViewer.PointLayer = AngleRotator.PointLayer = _pointLayer;
                                    hasFeaturelayers = true;
                                }
                            }
                        }

                        attribute = editorElement.Attribute("PolylineServiceName");
                        if (attribute != null)
                        {
                            var polylineLayerIDs = attribute.Value.Split(',');
                            layerIDs.AddRange(polylineLayerIDs);

                            if (polylineLayerIDs.Any())
                            {
                                var polylineLayer = MapControl.Layers[polylineLayerIDs[0]] as GraphicsLayer;
                                if (polylineLayer != null)
                                {
                                    hasFeaturelayers = true;
                                }
                            }
                        }

                        attribute = editorElement.Attribute("PolygonServiceName");
                        if (attribute != null)
                        {
                            var polylineLayerIDs = attribute.Value.Split(',');
                            layerIDs.AddRange(polylineLayerIDs);

                            if (polylineLayerIDs.Any())
                            {
                                var polylineLayer = MapControl.Layers[polylineLayerIDs[0]] as GraphicsLayer;
                                if (polylineLayer != null)
                                {
                                    hasFeaturelayers = true;
                                }
                            }
                        }

                        Editor.LayerIDs = layerIDs.ToArray();

                        attribute = editorElement.Attribute("TextServiceName");
                        if (attribute == null) continue;

                        Editor.TextLayerID = attribute.Value;

                        _textLayer = MapControl.Layers[attribute.Value] as GraphicsLayer;
                        if (_textLayer != null)
                        {
                            AttributesViewer.TextLayer = AngleRotator.TextLayer = _textLayer;
                            hasFeaturelayers = true;
                        }

                        if (hasFeaturelayers)
                        {
                            EditingTab.Visibility = Visibility.Visible;
                        }
                        break;
                    case "PolygonShape":
                        XAttribute shapes = editorElement.Attribute("Shapes");
                        if (shapes != null)
                        {
                            Editor.Shapes = shapes.Value.Split(',');
                        }
                        break;
                    case "PointSymbol":
                        AttributesViewer.Resolution = Tools.Resolution = AngleRotator.Resolution = _resolution = double.Parse(editorElement.Attribute

("ReferenceResolution").Value);
                        WIPEditor.Resolution = _resolution;
                        break;

                    case "WIPEditorLayer":
                        var WIPlayerIDList = new List<string>();
                        hasFeaturelayers = false;

                        attribute = editorElement.Attribute("WIPServiceName");
                        if (attribute != null)
                        {
                            _wipPolygonLayerName = attribute.Value;
                            WIPEditor.WipPolygonLayerName = _wipPolygonLayerName;
                            WIPlayerIDList.Add(_wipPolygonLayerName);

                            if (!string.IsNullOrEmpty(_wipPolygonLayerName))
                            {
                                var WIPLayer = MapControl.Layers[_wipPolygonLayerName] as FeatureLayer;
                                if (WIPLayer != null)
                                {
                                    hasFeaturelayers = true;
                                }
                            }
                        }
                        //WIPTemplatePicker.LayerIDs = WIPlayerIDList.ToArray();
                        WIPEditor.LayerIDs = WIPlayerIDList.ToArray();
                        if (hasFeaturelayers)
                        {
                            WIPEditorTab.Visibility = Visibility.Visible;
                        }

                        attribute = editorElement.Attribute("WIPLabelServiceName");
                        if (attribute != null)
                        {
                            _wipLabelLayerName = attribute.Value;
                            WIPEditor.WipLabelLayerName = _wipLabelLayerName;
                            if (string.IsNullOrEmpty(_wipLabelLayerName))
                            {
                                //WipLabelEdit.IsEnabled = false;
                            }
                        }

                        attribute = editorElement.Attribute("WIPLabelFieldsName");
                        if (attribute != null)
                        {
                            var fields = attribute.Value.Split(',');
                            foreach (var field in fields)
                            {
                                _wipLabelFieldNames.Add(field);
                            }
                            WIPEditor.WipLabelFieldNames = _wipLabelFieldNames;
                        }

                        //INC000004019908, INC000004273558
                        attribute = editorElement.Attribute("WIPOutlineServiceName");
                        if (attribute != null)
                        {
                            WIPEditor.WipOutlineLayerName = attribute.Value;
                        }

                        break;

                    case "NotesLayer":
                        var NoteslayerIDList = new List<string>();
                        hasFeaturelayers = false;

                        attribute = editorElement.Attribute("NotesServiceName");
                        if (attribute != null)
                        {
                            _notesLayerName = attribute.Value;
                            Notes.NotesLayerName = _notesLayerName;
                            NoteslayerIDList.Add(_notesLayerName);

                            if (!string.IsNullOrEmpty(_notesLayerName))
                            {
                                var NotesLayer = MapControl.Layers[_notesLayerName] as FeatureLayer;
                                if (NotesLayer != null)
                                {
                                    hasFeaturelayers = true;
                                }
                            }
                        }
                        Notes.LayerIDs = NoteslayerIDList.ToArray();
                        if (hasFeaturelayers)
                        {
                            NotesRibbon.Visibility = Visibility.Visible;
                        }

                        attribute = editorElement.Attribute("NotesLabelFieldsName");
                        if (attribute != null)
                        {
                            var fields = attribute.Value.Split(',');
                            foreach (var field in fields)
                            {
                                _notesLabelFieldNames.Add(field);
                            }
                            Notes.NotesLabelFieldNames = _notesLabelFieldNames;
                        }

                        attribute = editorElement.Attribute("NotesMapServiceUrl");
                        if (attribute != null)
                        {
                            Notes.NotesMapServiceUrl = attribute.Value;
                        }

                        attribute = editorElement.Attribute("NotesFeatureServiceUrl");
                        if (attribute != null)
                        {
                            Notes.NotesFeatureServiceUrl = attribute.Value;
                        }

                        break;

                    case "WIPDeletion":    //CAP #114707237 ---ME Q3 2019 Release
                        if (ConfigUtility.IsItemAllowed(editorElement))
                        {
                            WIPEditor.IsMemOfDeleteADGrps = true;
                        }
                        else
                        {
                            WIPEditor.IsMemOfDeleteADGrps = false;
                        }
                        break;
                }
            }
        }

        private void AddSearches(XElement searches)
        {
            if (searches.HasElements == false) return;

            Tools.StoredViewControl.LocateControl = Locate;

            foreach (XElement searchElement in searches.Elements())
            {
                SearchItem search = ConfigUtility.SearchItemFromXElement(searchElement);

                //********************** ENOSChange Start***************************
                if (search.Title == "Service Point Search")
                {
                    for (int i = 0; i < search.SearchLayers.Count; i++)
                    {
                        GetGenOnFeeder.SearchLayers.Add(search.SearchLayers[i].Url.ToString() + "/" + search.SearchLayers[i].ID.ToString());
                    }
                }
                //********************** ENOSChange End***************************
                if (search != null)
                {
                    search.IsCustom = true;
                    Tools.StoredViewControl.AddSearch(search);
                }

                if (search is CustomAddressSearch)
                {
                    ((CustomAddressSearch)search).layersInMap = MapControl.Layers;
                }
            }
        }

        /// <summary>
        /// Configures Searchable Layers based on values in the page.config
        /// shutoff display searchable annotation layers in the search drop down
        /// </summary>
        /// <param name="searchableLayers"></param>
        private void AddSearchableLayers(XElement searchableLayers)
        {
            if (searchableLayers.HasElements == false) return;

            foreach (XElement element in searchableLayers.Elements())
            {
                if (!ConfigUtility.IsItemAllowed(element)) continue;

                XAttribute url = element.Attribute("url");
                if (url == null) continue;

                XAttribute ids = element.Attribute("ids");
                if (ids == null) continue;

                foreach (var id in ids.Value.Split(','))
                {

                    var layer = new Miner.Server.Client.Tasks.SearchLayer { ID = Convert.ToInt32(id), Url = url.Value };
                    Locate.SearchableLayers.Add(layer);
                }
            }
        }

        private void AddRelatedData(XElement element)
        {
            if (element.HasElements == false) return;

            foreach (XElement path in element.Elements())
            {
                RelationshipInformation data = ConfigUtility.RelatedDataFromXElement(path);
                if (data != null)
                {
                    AttributesViewer.RelationshipInformation.Add(data);
                }
            }
        }

        private void AddBingKeys(XElement bingKeys)
        {
            if (bingKeys.HasAttributes == false) return;

            var attribute = bingKeys.Attribute("Key");
            if (attribute != null)
            {
                ConfigUtility.BingKey = attribute.Value;
            }

        }

        private void AddLayers(XElement layers)
        {
            if (layers.HasElements == false) return;

            XElement firstLayer = layers.Elements().First();
            if (firstLayer.Name == "WebMap")
            {
                // a webmap is the basemap. Need to load it before anything else 
                _waitingForWebmap = true;
                _usingWebmap = true;
                SetTraceMaps();
                firstLayer.Remove();
                AddWebMap(firstLayer, 0, layers);
            }
            else
            {
                //  basemap is not a webmap, proceed as normal
                _waitingForWebmap = false;
                ProcessLayers(layers);
            }
        }

        private void GetStoredViews(XElement layerElement)
        {
            var attribute = layerElement.Attribute("StoredDisplayNames");
            if (attribute == null || string.IsNullOrEmpty(attribute.Value)) return;

            string[] storedViewsCSV = attribute.Value.Split(',');

            attribute = layerElement.Attribute("MapServiceName");
            if (attribute == null) return;

            string layerID = attribute.Value;

            foreach (string storedView in storedViewsCSV)
            {
                if (_storedViews.ContainsKey(storedView))
                {
                    _storedViews[storedView] = _storedViews[storedView] + "," + layerID;
                }
                else
                {
                    _storedViews.Add(storedView, layerID);
                }
            }

        }

        private void ProcessLayers(XElement layers)
        {
            lock (_loadedServiceLayerLock)
            {
                int index = MapControl.Layers.Count;
                // "TextTemporary", "InsetMapMarker", and "BufferLayers" may already exist. They
                // should be the last layers, so skip them
                if (MapControl.Layers[TextLayerID] != null)
                {
                    --index;
                }
                if (MapControl.Layers[BufferLayerID] != null)
                {
                    --index;
                }
                if (MapControl.Layers[InsetMapMarkerID] != null)
                {
                    --index;
                }

                foreach (XElement layerElement in layers.Elements())
                {

                    //Check for Active Directory Access
                    if (!ConfigUtility.IsItemAllowed(layerElement)) continue;

                    if (layerElement.Name == "Layer")
                    {
                        Layer layer = ConfigUtility.LayerFromXElement(layerElement);
                        layer.Initialized += new EventHandler<EventArgs>(ServiceLayer_Initialized);
                        layer.Visible = Convert.ToBoolean(layerElement.Attribute("Visible").Value);
                        if (layer is ArcGISDynamicMapServiceLayer)
                        {
                            ((ArcGISDynamicMapServiceLayer)layer).PropertyChanged += new PropertyChangedEventHandler(ServiceLayer_PropertyChanged);
                        }
                        /*****************PLC Changes start 08/22/2017 RK***************/
                        //Added PLCINFO to add event handler
                        else if (layer.ID.Contains("WIP(Search)") || layer.ID.Contains("WIP Cloud") || layer.ID.Contains("WIP Label") || layer.ID.Contains("PLCINFO"))
                        {
                            ((FeatureLayer)layer).PropertyChanged += new PropertyChangedEventHandler(ServiceLayer_PropertyChanged);
                        }
                        /*****************PLC Changes end 08/22/2017 RK***************/
                        if (layer != null)
                        {
                            MapControl.Layers.Insert(index++, layer);
                            _loadedServiceLayerCount++;
                            logger.Info("Layer Added: " + layer.ID);
                        }
                        GetStoredViews(layerElement);
                    }
                    else if (layerElement.Name == "WebMap")
                    {
                        AddWebMap(layerElement, index, null);
                    }
                }



                SortStoredViews(layers);
                Tools.StoredViewControl.ViewList = _storedViews;
                ReprojectExtentStart();

                _waitingForWebmap = false;
                if (_usingWebmap == true)
                {
                    SetTraceMaps();
                }
                if (_editorConfig != null)
                {
                    // A webmap is being used for a basemap.
                    // Loading editor config has been delayed until after webmap
                    // has loaded and other layers have been configured, i.e. now
                    AddEditor(_editorConfig);
                }
            }
        }

        private void SortStoredViews(XElement layers)
        {
            IList<string> storedDisplays = new List<string>();
            IDictionary<string, string> storedDisplaysDictionary = new Dictionary<string, string>();
            storedDisplaysDictionary.Add(_storedViews.First());
            foreach (XElement layerElement in layers.Elements())
            {
                //Check for Active Directory Access
                if (!ConfigUtility.IsItemAllowed(layerElement)) continue;

                if (layerElement.Name == "Layer")
                {
                    storedDisplays.Add(layerElement.Attribute("MapServiceName").Value);
                }
            }
            foreach (string storedDisplay in storedDisplays)
            {
                if (_storedViews.ContainsKey(storedDisplay))
                {
                    storedDisplaysDictionary.Add(storedDisplay, _storedViews[storedDisplay]);
                }
            }
            _storedViews = storedDisplaysDictionary.ToDictionary(t => t.Key, t => t.Value);
            //            _storedViews = _storedViews.OrderBy(x => x.Key).ToDictionary(t => t.Key, t => t.Value);
        }


        // This event is typically raised when someone changes the stored display. Note that it fires immediately
        // i.e. before the layers have actually loaded / been made visible
        void ServiceLayer_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (sender is ArcGISDynamicMapServiceLayer && e.PropertyName == "Visible")
            {
                Layer layer = sender as Layer;
                if (layer.ID.ToUpper().Contains("SCHEMATICS"))
                {
                    if (layer.Visible)
                    {
                        SchematicsRibbonPanel.SchematicsDynamicMapServiceLayer = layer as ArcGISDynamicMapServiceLayer;
                        SchematicsTab.Visibility = System.Windows.Visibility.Visible;
                        SchematicsRibbonPanel.CadExportTool = _cadExportTool.CADExportControl;

                    }
                    else if (!Tools.StoredViewControl._selectedStoredView.StoredViewName.ToUpper().Contains("SCHEMATICS"))
                    {
                        if (RibbonTabControl.SelectedContent == SchematicsRibbonPanel)
                        {
                            RibbonTabControl.SelectedIndex = 0;
                        }
                        SchematicsTab.Visibility = System.Windows.Visibility.Collapsed;

                    }
                    Tools.StoredViewControl.SetLocateControlLocates();
                }

                //To Visible Scale and Schematic Option
                /***********************************31Oct2016 Start*******************************/
                if (Tools.StoredViewControl._selectedStoredView.StoredViewName.ToUpper().Contains("SCHEMATICS"))
                {
                    _cadExportTool.CADExportControl.lblSD.Content = "SCHEMATICS";
                    _cadExportTool.CADExportControl.lblSD.Visibility = System.Windows.Visibility.Visible;
                    _cadExportTool.CADExportControl.cboExportFormat.Visibility = System.Windows.Visibility.Visible;
                    _cadExportTool.CADExportControl.lblExportFormat.Visibility = System.Windows.Visibility.Visible;
                    _cadExportTool.CADExportControl.mapTypeScroll.Visibility = System.Windows.Visibility.Collapsed;
                    _cadExportTool.CADExportControl.cboExportFormat.SelectedItem = _cadExportTool.CADExportControl.cboExportFormat.Items[0];
                    _cadExportTool.CADExportControl.cboScale.ItemsSource = _cadExportParameters.SchematicScales;
                    _cadExportTool.CADExportControl.cboScale.DisplayMemberPath = "Value";
                    _cadExportTool.CADExportControl.cboScale.SelectedValuePath = "Key";
                    if (Tools.StoredViewControl._selectedStoredView.StoredViewName.ToUpper().Contains("250"))
                    {
                        _cadExportTool.CADExportControl.cboScale.SelectedItem = _cadExportParameters.SchematicScales.Where(s => s.Key ==

_cadExportParameters.DefaultSchematic250Scale).First();
                    }
                    else
                    {
                        _cadExportTool.CADExportControl.cboScale.SelectedItem = _cadExportParameters.SchematicScales.Where(s => s.Key ==

_cadExportParameters.DefaultSchematicScale).First();
                    }
                    _cadExportTool.CADExportControl.lblWYSWYG.Visibility = System.Windows.Visibility.Collapsed;
                    _cadExportTool.CADExportControl.lblCircuits.Visibility = System.Windows.Visibility.Visible;
                    _cadExportTool.CADExportControl.cboWYSWYG.Visibility = System.Windows.Visibility.Collapsed;
                    _cadExportTool.CADExportControl.lstCircuitIds.Visibility = System.Windows.Visibility.Visible;
                    _cadExportTool.CADExportControl.lstAll.Visibility = System.Windows.Visibility.Visible;
                    _cadExportTool.CADExportControl.cboSchematicFeederFilter.Visibility = System.Windows.Visibility.Visible;
                    _cadExportTool.CADExportControl.cboSchematicVoltageFilter.Visibility = Visibility.Visible;
                    _cadExportTool.CADExportControl.lblSchematicFilterVoltage.Visibility = Visibility.Visible;
                    _cadExportTool.CADExportControl.lblSchematicFilterFeeder.Visibility = Visibility.Visible;

                    // _cadExportTool.CADExportControl.cboSchematicFilter.Visibility = System.Windows.Visibility.Visible;
                    //  _cadExportTool.CADExportControl.cboSchematicFilter.SelectedItem = _cadExportTool.CADExportControl.cboSchematicFilter.Items[0];
                    //  _cadExportTool.CADExportControl.lblSelectFilter.Visibility = Visibility.Visible;
                    //_cadExportTool.CADExportControl.lblVoltage.Visibility = System.Windows.Visibility.Visible;
                    //_cadExportTool.CADExportControl.cboVoltageFilter.Visibility = System.Windows.Visibility.Visible;
                    //_cadExportTool.CADExportControl.cboFeederType.Visibility = System.Windows.Visibility.Visible;
                    //_cadExportTool.CADExportControl.lblFeederType.Visibility = System.Windows.Visibility.Visible;

                    if (MapControl.Scale <= _cadExportParameters.SchematicQueryScale && _cadExportTool._fw != null && layer.ID.ToUpper().Contains("SCHEMATICS"))
                    {
                        _cadExportTool.CADExportControl.PopulateCircuitIds();


                    }



                    if (_cadExportTool._fw != null)
                    {
                        _cadExportTool.CADExportControl.Height = 420;
                        _cadExportTool.CADExportControl.stdwgPanel.Height = 420;
                        _cadExportTool._fw.Height = 420;
                    }
                }
                else if (Tools.StoredViewControl._selectedStoredView.StoredViewName.ToUpper().Contains("DIST") &&

Tools.StoredViewControl._selectedStoredView.StoredViewName.ToUpper().Contains("50"))
                {
                    _cadExportTool.CADExportControl.lblSD.Content = "";
                    _cadExportTool.CADExportControl.lblSD.Visibility = System.Windows.Visibility.Collapsed;
                    _cadExportTool.CADExportControl.cboExportFormat.Visibility = System.Windows.Visibility.Visible;
                    _cadExportTool.CADExportControl.lblExportFormat.Visibility = System.Windows.Visibility.Visible;
                    _cadExportTool.CADExportControl.cboExportFormat.SelectedItem = _cadExportTool.CADExportControl.cboExportFormat.Items[0];
                    _cadExportTool.CADExportControl.mapTypeScroll.Visibility = System.Windows.Visibility.Visible;
                    _cadExportTool.CADExportControl.lblWYSWYG.Visibility = System.Windows.Visibility.Collapsed;
                    _cadExportTool.CADExportControl.lblCircuits.Visibility = System.Windows.Visibility.Collapsed;
                    _cadExportTool.CADExportControl.cboWYSWYG.Visibility = System.Windows.Visibility.Collapsed;
                    _cadExportTool.CADExportControl.lblFilterType.Visibility = System.Windows.Visibility.Collapsed;
                    _cadExportTool.CADExportControl.lblFilterValue.Visibility = System.Windows.Visibility.Collapsed;
                    _cadExportTool.CADExportControl.lstCircuitIds.Visibility = System.Windows.Visibility.Collapsed;
                    _cadExportTool.CADExportControl.cboScale.ItemsSource = _cadExportParameters.Scales;
                    _cadExportTool.CADExportControl.cboScale.DisplayMemberPath = "Value";
                    _cadExportTool.CADExportControl.cboScale.SelectedValuePath = "Key";
                    _cadExportTool.CADExportControl.cboScale.SelectedItem = _cadExportParameters.Scales.Where(s => s.Key == _cadExportParameters.DefaultScale).First();
                    //_cadExportTool.CADExportControl.Height = 250;
                    //_cadExportTool.CADExportControl.stdwgPanel.Height = 280;
                    _cadExportTool.CADExportControl.lstAll.Visibility = System.Windows.Visibility.Collapsed;
                    // _cadExportTool.CADExportControl.cboSchematicFilter.Visibility = System.Windows.Visibility.Collapsed;
                    _cadExportTool.CADExportControl.cboSchematicFeederFilter.Visibility = Visibility.Collapsed;
                    _cadExportTool.CADExportControl.cboSchematicVoltageFilter.Visibility = Visibility.Collapsed;
                    _cadExportTool.CADExportControl.lblSchematicFilterVoltage.Visibility = Visibility.Collapsed;
                    _cadExportTool.CADExportControl.lblSchematicFilterFeeder.Visibility = Visibility.Collapsed;
                    //_cadExportTool.CADExportControl.lblSelectFilter.Visibility = Visibility.Collapsed;
                    //_cadExportTool.CADExportControl.lblSchematicFilterType.Visibility = Visibility.Collapsed;
                    //_cadExportTool.CADExportControl.cboSchematicFilter.SelectedItem = _cadExportTool.CADExportControl.cboSchematicFilter.Items[0];
                    //_cadExportTool.CADExportControl.lblVoltage.Visibility = System.Windows.Visibility.Collapsed;
                    //_cadExportTool.CADExportControl.cboVoltageFilter.Visibility = System.Windows.Visibility.Collapsed;
                    //_cadExportTool.CADExportControl.cboFeederType.Visibility = System.Windows.Visibility.Collapsed;
                    //_cadExportTool.CADExportControl.lblFeederType.Visibility = System.Windows.Visibility.Collapsed;
                    if (_cadExportTool._fw != null && layer.ID.ToUpper().Contains("50") && layer.ID.ToUpper().Contains("50"))
                    {
                        _cadExportTool.CADExportControl.populateMapType();
                        _cadExportTool.CADExportControl.Height = 405;
                        _cadExportTool.CADExportControl.stdwgPanel.Height = 405;
                        _cadExportTool._fw.Height = 405;

                    }
                }
                else
                {
                    _cadExportTool.CADExportControl.lblSD.Content = "ED";
                    _cadExportTool.CADExportControl.lblSD.Visibility = System.Windows.Visibility.Visible;
                    _cadExportTool.CADExportControl.cboExportFormat.SelectedItem = _cadExportTool.CADExportControl.cboExportFormat.Items[0];
                    _cadExportTool.CADExportControl.cboExportFormat.Visibility = System.Windows.Visibility.Collapsed;
                    _cadExportTool.CADExportControl.lblExportFormat.Visibility = System.Windows.Visibility.Collapsed;
                    _cadExportTool.CADExportControl.mapTypeScroll.Visibility = System.Windows.Visibility.Collapsed;
                    _cadExportTool.CADExportControl.lblWYSWYG.Visibility = System.Windows.Visibility.Visible;
                    _cadExportTool.CADExportControl.lblCircuits.Visibility = System.Windows.Visibility.Collapsed;
                    _cadExportTool.CADExportControl.cboWYSWYG.Visibility = System.Windows.Visibility.Visible;
                    _cadExportTool.CADExportControl.lstCircuitIds.Visibility = System.Windows.Visibility.Collapsed;
                    _cadExportTool.CADExportControl.lblFilterType.Visibility = System.Windows.Visibility.Collapsed;
                    _cadExportTool.CADExportControl.lblFilterValue.Visibility = System.Windows.Visibility.Collapsed;
                    _cadExportTool.CADExportControl.cboScale.ItemsSource = _cadExportParameters.Scales;
                    _cadExportTool.CADExportControl.cboScale.DisplayMemberPath = "Value";
                    _cadExportTool.CADExportControl.cboScale.SelectedValuePath = "Key";
                    _cadExportTool.CADExportControl.cboScale.SelectedItem = _cadExportParameters.Scales.Where(s => s.Key == _cadExportParameters.DefaultScale).First();
                    _cadExportTool.CADExportControl.Height = 250;
                    _cadExportTool.CADExportControl.stdwgPanel.Height = 280;
                    _cadExportTool.CADExportControl.lstAll.Visibility = System.Windows.Visibility.Collapsed;
                    //_cadExportTool.CADExportControl.cboSchematicFilter.Visibility = System.Windows.Visibility.Collapsed;
                    _cadExportTool.CADExportControl.cboSchematicFeederFilter.Visibility = Visibility.Collapsed;
                    _cadExportTool.CADExportControl.cboSchematicVoltageFilter.Visibility = Visibility.Collapsed;
                    _cadExportTool.CADExportControl.lblSchematicFilterVoltage.Visibility = Visibility.Collapsed;
                    _cadExportTool.CADExportControl.lblSchematicFilterFeeder.Visibility = Visibility.Collapsed;
                    //_cadExportTool.CADExportControl.lblSelectFilter.Visibility = Visibility.Collapsed;
                    //_cadExportTool.CADExportControl.lblSchematicFilterType.Visibility = Visibility.Collapsed;
                    //_cadExportTool.CADExportControl.cboSchematicFilter.SelectedItem = _cadExportTool.CADExportControl.cboSchematicFilter.Items[0];
                    //_cadExportTool.CADExportControl.lblVoltage.Visibility = System.Windows.Visibility.Collapsed;
                    //_cadExportTool.CADExportControl.cboVoltageFilter.Visibility = System.Windows.Visibility.Collapsed;
                    //_cadExportTool.CADExportControl.cboFeederType.Visibility = System.Windows.Visibility.Collapsed;
                    //_cadExportTool.CADExportControl.lblFeederType.Visibility = System.Windows.Visibility.Collapsed;

                    if (_cadExportTool._fw != null)
                        _cadExportTool._fw.Height = 270;
                }
            }
            //INC000003946726
            if ((sender as Layer).ID == "WIP Notes" && e.PropertyName == "Visible")
            {
                if (((sender as Layer)).Visible)
                    Notes.ToggleNotes.IsChecked = true;
                else
                    Notes.ToggleNotes.IsChecked = false;
            }

            //INC000004273558, INC000004019908
            if (((sender as Layer).ID == "WIP(Search)" || (sender as Layer).ID == "WIP Cloud") && e.PropertyName == "Visible")
            {
                if ((MapControl.Layers["WIP(Search)"].Visible == true || MapControl.Layers["WIP Cloud"].Visible == true))
                    MapControl.Layers["WIP Outline"].Visible = true;
                else
                    MapControl.Layers["WIP Outline"].Visible = false;
            }

            //*****************PLC Changes Start 08/22/2017*********************//
            if ((sender as Layer).ID == "PLCINFO" && e.PropertyName == "Visible")
            {
                FeatureLayer flPLC = null;
                DynamicMapServiceLayer plcLabelLayer = null;
                IList<Layer> layers = MapControl.Layers.Where(l => l is FeatureLayer).ToList();
                IList<Layer> dynamiclayers = MapControl.Layers.Where(l => l is DynamicMapServiceLayer).ToList();
                foreach (Layer layer in layers)
                {

                    if (layer.ID == "Rollover_PLD_INFO")
                    {
                        flPLC = layer as FeatureLayer;
                    }
                }
                foreach (Layer layer in dynamiclayers)
                {

                    if (layer.ID == "PLCINFO_Labels")
                    {
                        plcLabelLayer = layer as DynamicMapServiceLayer;
                    }
                }
                if (((sender as Layer)).Visible)
                {
                    WIPEditor.PLC.TogglePLCInfoLayer.IsChecked = true;
                    if (flPLC != null)
                    {
                        flPLC.Visible = true;
                    }
                    if (plcLabelLayer != null)
                    {
                        plcLabelLayer.Visible = true;
                        plcLabelLayer.Refresh();
                    }
                }
                else
                {
                    WIPEditor.PLC.TogglePLCInfoLayer.IsChecked = false;
                    if (flPLC != null)
                    {
                        flPLC.Visible = false;
                        flPLC.Update();
                    }
                    if (plcLabelLayer != null)
                    {
                        plcLabelLayer.Visible = false;
                        plcLabelLayer.Refresh();
                    }
                }
            }
            if ((sender as Layer).ID == "PLCINFO_Labels" && e.PropertyName == "Visible")
            {
                FeatureLayer flPLC = null;
                IList<Layer> layers = MapControl.Layers.Where(l => l is FeatureLayer).ToList();
                foreach (Layer layer in layers)
                {

                    if (layer.ID == "PLCINFO")
                    {
                        flPLC = layer as FeatureLayer;
                    }
                }
                if (flPLC.Visible == true)
                {
                    ((sender as Layer)).Visible = true;
                    ((sender as ArcGISDynamicMapServiceLayer)).Refresh();
                }
                else
                {
                    ((sender as Layer)).Visible = false;
                    ((sender as ArcGISDynamicMapServiceLayer)).Refresh();
                }


            }
            //*****************PLC Changes Ends 08/22/2017*********************//

            //For PONS Ad Hoc Print - Added EDMaster layers
            if (this.Tools.StoredViewControl._selectedStoredView != null)
                _ponsDialogTool.PONSDialogControl.SelectedStoredDisplayName = this.Tools.StoredViewControl._selectedStoredView.ToString();
            else
                _ponsDialogTool.PONSDialogControl.SelectedStoredDisplayName = null;

            //INC000004378902 - Set zoom scale for zoom to feature from grid
            if (AttributesViewer != null && this.Tools.StoredViewControl._selectedStoredView != null)
                AttributesViewer.selectedStoredView = this.Tools.StoredViewControl._selectedStoredView.StoredViewName.ToString();

            //DA #190905 - ME Q1 2020 -- start
            if ((sender as Layer).ID == LIDARCorStoredDisplayName && e.PropertyName == "Visible")
            {
                if (MapControl.Layers[LIDARCorStoredDisplayName].Visible == true)
                    ToggleLIDARCorrectionLayer.IsEnabled = true;
                else
                    ToggleLIDARCorrectionLayer.IsEnabled = false;
            }
            //DA #190905 - ME Q1 2020 -- end
        }
        /***********************************31Oct2016 Ends*******************************/
        private void MapIsLoaded()
        {
            MapControl.KeyUp += new System.Windows.Input.KeyEventHandler(MapControl_KeyUp);
            if (_gsfConfig != null)
            {
                AddGeographicSearchFilter(_gsfConfig);
            }

            //Selecting Default stored view
            SelectDefaultStoredView();
            if (_dataGridCustomButtonManager.AttributesViewer == null)
            {
                MoreInfoPseudoCustomButton moreInfoPseudoCustomButton = new MoreInfoPseudoCustomButton(_tlm, _deviceSettings);
                _dataGridCustomButtonManager.CustomButtons.Add(((IDataGridCustomButton)moreInfoPseudoCustomButton).Name, moreInfoPseudoCustomButton);
                OutageHistoryCustomButton outageHistoryCustomButton = new OutageHistoryCustomButton();
                _dataGridCustomButtonManager.CustomButtons.Add(((IDataGridCustomButton)outageHistoryCustomButton).Name, outageHistoryCustomButton);
                ButterflyCustomButton butterflyCustomButton = new ButterflyCustomButton(_butterflyTool);
                _dataGridCustomButtonManager.CustomButtons.Add(((IDataGridCustomButton)butterflyCustomButton).Name, butterflyCustomButton);
                /***************************ENOSChange Start************************/
                ShowGenerationCustomButton showGenCustomButton = new ShowGenerationCustomButton();
                _dataGridCustomButtonManager.CustomButtons.Add(((IDataGridCustomButton)showGenCustomButton).Name, showGenCustomButton);
                GenOnFeederCustomButton genFeederCustomButton = new GenOnFeederCustomButton();
                _dataGridCustomButtonManager.CustomButtons.Add(((IDataGridCustomButton)genFeederCustomButton).Name, genFeederCustomButton);
                //CIT for non-identify, grid tool *start
                if (engineers_group == true)
                {
                    CIT_CustomButton filledDuctButton = new CIT_CustomButton(_Conductor_in_TrenchTool);
                    filledDuctButton.map_cit = MapControl;
                    filledDuctButton.mapArea_grid = this.MapArea;
                    _dataGridCustomButtonManager.CustomButtons.Add(((IDataGridCustomButton)filledDuctButton).Name, filledDuctButton);
                    //CIT for non-identify, grid tool *end
                }
                /***************************ENOSChange End************************/
                _dataGridCustomButtonManager.CreateButtons(this.AttributesViewer);

            }
            StatusBar.Text = "";
            AdHocTemplateButton.IsEnabled = true;
            CMCSTemplateButton.IsEnabled = true; //INC000004403856            
            StandardTemplateButton.IsEnabled = true;
            DelineationPrintButton.IsEnabled = true;
            Tools.IsEnabled = true;
            IdentifyToggle.IsEnabled = true;
            Identify.IsEnabled = true;
            this.Locate.EnableControls();
            this.Locate.IsEnabled = true;


            //InitTracingSettings(null, null);
            ShowTraceOptions.IsChecked = true;
            ShowTraceOptions.IsChecked = false;
            this.Tools.StoredViewControl.ShowRolloverInfoButton = Tools.ShowRolloverInfoToggleButton;
            this.Tools.StoredViewControl.StoredViewChangeAction += _showRolloverInfo.StoredViewChange;
            this.Tools.StoredViewControl.StoredViewChangeAction += FilterRibbonPanel.SetStateFromStoredDisplayChange;
            this.Tools.StoredViewControl.StoredViewChangeAction += UfmRibbonPanel.SetUfmVisibility;
            this.Tools.StoredViewControl.StoredViewChangeAction += _cadExportTool.CADExportControl.SetDefaultScale;
            _showRolloverInfo.StoredViewChange(this.Tools.StoredViewControl._selectedStoredView.StoredViewName);
            _showRolloverInfo.PositionMapTip = PositionMapTip;

            if (_showRolloverInfo.AutoStart)
            {
                System.Windows.Threading.DispatcherTimer rowDetailsDispatcherTimer = new System.Windows.Threading.DispatcherTimer();
                rowDetailsDispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, _showRolloverInfo.AutoStartDelaySeconds * 1000); // Milliseconds
                rowDetailsDispatcherTimer.Tick += new EventHandler(Target);
                rowDetailsDispatcherTimer.Start();
            }


            if (zoomToLocationFlag)
            {
                if (!string.IsNullOrEmpty(sapEuipIdQueryString) || (!string.IsNullOrEmpty(sapLatQueryString) && !string.IsNullOrEmpty(sapLongQueryString)) || !string.IsNullOrEmpty(outageIdQueryString) /*|| !string.IsNullOrEmpty(sapPmOrderQueryString)*/)
                {
                    Tools.BookmarksControl.FirstTime = false;
                }
                zoomToLocationFromSAP();
                zoomToLocationFlag = false;
            }

            Tools.FavoritesControl.Load();
            Tools.BookmarksControl.Load();

            //TODO: JET                 
            //RibbonTabControl.SelectedIndex = 3;
            //JetToggleTool.JobsToggleButton.IsChecked = true;

        }

        private void GetShowRollover()
        {
            ApplicationCacheManager cacheManager = new ApplicationCacheManager();
            if (cacheManager.ReadLocallValue("ShowRollover") != "false")
            {
                _doShowRollover = true;
            }

        }
        private void Target(object sender, EventArgs eventArgs)
        {
            ((System.Windows.Threading.DispatcherTimer)sender).Stop();

            if (_doShowRollover)
            {
                Tools.StoredViewControl.ShowRolloverInfoButton.IsChecked = true;
            }
        }

        #region Addresss Auto Complete

        void Locate_Loaded(object sender, RoutedEventArgs e)
        {

            LocateControl lc = Locate as LocateControl;
            Grid lcGrid = (Grid)VisualTreeHelper.GetChild(lc, 0);
            ComboBox searchBox = (ComboBox)lcGrid.FindName("PART_LocateTypeComboBox");
            searchBox.SelectionChanged += new SelectionChangedEventHandler(searchBox_SelectionChanged);
            ConfigUtility.CurrentMap = MapControl;
            searchBox.SelectedItem = "Operating Number";
        }

        private void searchBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox searchBox = sender as ComboBox;
            if (searchBox.SelectedItem is CustomAddressSearch)
            {
                LocateControl lc = Locate as LocateControl;
                Grid lcGrid = (Grid)VisualTreeHelper.GetChild(lc, 0);
                AutoCompleteBox autoBox = (AutoCompleteBox)lcGrid.FindName("PART_QueryTextBox");
                //autoBox.KeyUp += new KeyEventHandler(autoBox_KeyUp);
                autoBox.Populating += new PopulatingEventHandler(autoBox_Populating);
                //autoBox.MinimumPopulateDelay = 50;
                autoBox.MinimumPrefixLength = 5;

            }
            else
            {
                LocateControl lc = Locate as LocateControl;
                Grid lcGrid = (Grid)VisualTreeHelper.GetChild(lc, 0);
                AutoCompleteBox autoBox = (AutoCompleteBox)lcGrid.FindName("PART_QueryTextBox");
                autoBox.Populating -= new PopulatingEventHandler(autoBox_Populating);
            }
            //throw new NotImplementedException();
        }

        private void autoBox_Populating(object sender, PopulatingEventArgs e)
        {
            e.Cancel = true;

            Dictionary<string, string> address;
            string SearchText = "";

            SearchString(e.Parameter, out address, out SearchText);

            CallLocateAddress(address, SearchText);
        }

        public void CallLocateAddress(Dictionary<string, string> address, string query)
        {

            StatusBar.Text = "Searching Address..";
            Locator _locatorTask = new Locator(ConfigUtility.LocatorService);
            _locatorTask.AddressToLocationsCompleted += new EventHandler<AddressToLocationsEventArgs>(loc_AddressToLocationsCompleted);
            _locatorTask.Failed += new EventHandler<ESRI.ArcGIS.Client.Tasks.TaskFailedEventArgs>(loc_Failed);

            var addressParams = new AddressToLocationsParameters();
            SpatialReference LocalMapSpatialRef = MapControl.SpatialReference;
            addressParams.OutSpatialReference = LocalMapSpatialRef;

            if (query == "")
            {
                addressParams.Address.Add("Address", address["Address"]);
                addressParams.Address.Add("City", address["City"]);
                addressParams.Address.Add("State", address["State"]);
                addressParams.Address.Add("Zip", address["Zip"]);
            }
            else
            {
                addressParams.Address.Add("SingleLine", query);
            }

            _locatorTask.AddressToLocationsAsync(addressParams);

        }

        private void SearchString(string searchParam, out Dictionary<string, string> address, out string searchText)
        {
            address = new Dictionary<string, string>();
            searchText = "";
            string[] searchParams = searchParam.Split(',');
            int number2 = 0;
            address.Add("Address", "");
            address.Add("City", "");
            address.Add("State", "CA");
            address.Add("Zip", "");

            switch (searchParams.Length)
            {
                case 1:
                    if (int.TryParse(searchParam.Trim(), out number2))
                    {
                        address["Zip"] = searchParam.Trim();
                    }
                    else
                        searchText = searchParam.Trim() + ",CA";
                    break;
                case 4:
                    if (int.TryParse(searchParams[3].Trim(), out number2) && searchParams[2].Trim().Length == 2)
                    {
                        if (searchParams[0].Trim() != "")
                        {
                            address["Address"] = searchParams[0].Trim();
                        }
                        if (searchParams[1].Trim() != "")
                            address["City"] = searchParams[1].Trim();
                        if (searchParams[3].Trim() != "")
                            address["Zip"] = searchParams[3].Trim();
                    }
                    break;
                case 3:
                    if (!int.TryParse(searchParams[2].Trim(), out number2) && searchParams[2].Trim().Length == 2)
                    {
                        if (searchParams[0].Trim() != "")
                        {

                            address["Address"] = searchParams[0].Trim();
                        }

                        if (searchParams[1].Trim() != "" && !int.TryParse(searchParams[1].Trim(), out number2))
                            address["City"] = searchParams[1].Trim();


                    }
                    else if (int.TryParse(searchParams[2].Trim(), out number2))
                    {
                        if (searchParams[0].Trim() != "")
                        {
                            address["Address"] = searchParams[0].Trim();
                        }

                        address["Zip"] = searchParams[2].Trim();
                    }
                    else
                        searchText = searchParam;
                    break;

                case 2:
                    if (!int.TryParse(searchParams[1].Trim(), out number2) && searchParams[1].Trim().Length != 2)
                    {
                        if (searchParams[0].Trim() != "")
                        {
                            address["Address"] = searchParams[0].Trim();
                        }

                        if (searchParams[1].Trim() != "" && !int.TryParse(searchParams[1].Trim(), out number2))
                            address["City"] = searchParams[1].Trim();

                    }
                    else
                        searchText = searchParam;

                    break;
            }
        }

        private void loc_Failed(object sender, ESRI.ArcGIS.Client.Tasks.TaskFailedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void loc_AddressToLocationsCompleted(object sender, AddressToLocationsEventArgs e)
        {

            List<AddressCandidate> returnedCandidates = e.Results;
            var filteredGraphics = returnedCandidates.OrderByDescending(x => x.Score).ToList();

            List<string> names = new List<string>();

            LocateControl lc = Locate as LocateControl;
            Grid lcGrid = (Grid)VisualTreeHelper.GetChild(lc, 0);
            AutoCompleteBox autoBox = (AutoCompleteBox)lcGrid.FindName("PART_QueryTextBox");

            foreach (AddressCandidate addca in filteredGraphics)
            {

                //if (!names.Contains(addca.Address) && (addca.Address.Contains("CA,") || addca.Address.Contains(", CA")))
                //{
                //    if (autoBox.Text.ToUpper().Contains("AND"))
                //        names.Add(addca.Address.Replace("&", "and"));
                //    else
                //        names.Add(addca.Address);
                //}
                names.Add(addca.Address);
            }

            autoBox.ItemsSource = null;
            autoBox.ItemsSource = names;
            autoBox.PopulateComplete();
            StatusBar.Text = "";

        }
        #endregion

        void MapControl_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Escape && Identify.IsActive)
            {
                DrawMode drawMode = Identify.DrawMode;
                Identify.DrawMode = DrawMode.None;
                Identify.DrawMode = drawMode;
                //Identify.Cancel();
                CancelIdentify();
            }
        }

        void ServiceLayer_Initialized(object sender, EventArgs e)
        {
            lock (_loadedServiceLayerLock)
            {
                // i.e. all layers have been loaded...really no map interaction should happen before this point
                if (--_loadedServiceLayerCount <= 0)
                {
                    LayerVisibilityTree.InitializeTree(MapControl);
                    LayerVisibilityTree.InitializeTree(Tools.InsetMap);
                    LayerVisibilityTree.LayerVisibilityChanged += new EventHandler<IsVisibleEventArgs>(LayerVisibilityTree_LayerVisibilityChanged);
                }
            }
        }

        private void AddWebMap(XElement layerElement, int index, XElement layers)
        {
            // Delay updating maps in the Tools until after webmaps have loaded.
            if (_waitingForWebmap == true)
            {
                Tools.Map = null;
            }

            Document doc;
            string mapID;
            string mapDisplayName;
            bool privateMap;
            bool visible = true;
            string tokenServerUrl = "https://www.arcgis.com/sharing";
            ConfigUtility.WebmapDocFromXElement(layerElement, out doc, out mapID, out privateMap, ref tokenServerUrl, ref visible, out mapDisplayName);
            doc.GetMapCompleted += WebMapGetMapCompleted;

            if (mapID != null)
            {
                // save index where webmap layers will be inserted into MapControl
                _webMapIndices[mapID] = index;

                string proxyUrl = string.Empty;
                var attribute = layerElement.Attribute("ProxyUrl");
                if (attribute != null)
                {
                    proxyUrl = attribute.Value.ToString();
                }

                if (privateMap == true)
                {
                    // need a password to access webmap, create a prompt that will be
                    // displayed after page has been loaded
                    WebmapPasswordPrompt wmp = new WebmapPasswordPrompt(doc, mapID, mapDisplayName, proxyUrl, tokenServerUrl, layers, visible);
                    wmp.Closed += WebMapPromptClosed;
                    if (_pageLoaded)
                    {
                        wmp.Show();
                    }
                    else
                    {
                        _webMapPrompts.Add(wmp);
                    }
                }
                else
                {
                    object[] parms = new object[] { visible, layers, proxyUrl };
                    doc.GetMapAsync(mapID, parms);
                }
            }
        }

        void WebMapPromptClosed(object sender, EventArgs e)
        {
            //            Locate.EnableControls();
            WebmapPasswordPrompt wmp = (WebmapPasswordPrompt)sender;

            if (wmp.DialogResult == true)
            {
                Document doc = wmp.Doc;
                string mapID = wmp.MapID;

                // for a cleaner solution, use IdentityManager in ArcGIS API for SL 3.0
                string tokenURL = wmp.TokenServerUrl + "/generateToken?username=" +
                                    wmp.Username.Text + "&password=" +
                                  wmp.Password.Password + "&client=requestip&f=json";
                WebClient webClient = new WebClient();
                webClient.OpenReadCompleted +=
                    (s, a) =>
                    {
                        if (a.Error != null) return;

                        JsonValue jsonReponse = JsonObject.Load(a.Result);
                        a.Result.Close();

                        if (jsonReponse.ContainsKey("token"))
                        {
                            // token generation successful, get webmap
                            string token = jsonReponse["token"];
                            doc.Token = token;
                            object[] parms = new object[] { wmp.Visible, wmp.Layers };
                            doc.GetMapAsync(mapID, parms);
                        }
                        else
                        {
                            // token generation failed, prompt for username/password
                            JsonValue errorData = jsonReponse["error"];
                            JsonArray details =
                                (JsonArray)errorData["details"];
                            wmp.ErrorBlock.Text = "WebMap error: " +
                                             errorData["message"] + " " +
                                             string.Join<JsonValue>(" ", details);
                            wmp.Show();
                        }
                    };
                webClient.OpenReadAsync(new Uri(tokenURL));
            }
            else
            {
                StatusBar.Text = "Password not entered for WebMap " + wmp.MapIDBlock.Text
                                 + ". WebMap not loaded.";
            }
        }

        private void WebMapGetMapCompleted(object sender, GetMapCompletedEventArgs e)
        {
            var userParams = e.UserState as object[];

            if (e.Error != null)
            {
                StatusBar.Text = "WebMap error: " + e.Error.Message;

                if (userParams != null && userParams[1] != null)
                {
                    // this layer would have been the basemap, start over
                    var layers = userParams[1] as XElement;
                    var first = layers.Elements().First();
                    first = layers.Elements().First();
                    _waitingForWebmap = false;
                    AddLayers(layers);
                    SetTracingTabVisibility();
                    if (first.Name != "WebMap")
                    {
                        Tools.Map = MapControl;
                    }
                }
            }
            else
            {
                Map tempMap = e.Map;
                bool visible = userParams != null && userParams[0] is bool ? (bool)userParams[0] : true;
                string proxyUrl = userParams != null && userParams[2] is string ? (string)userParams[2] : string.Empty;

                int index = _webMapIndices[e.ItemInfo.ID];
                var layerCollection = new LayerCollection();

                // An exception is thrown when adding a layer to a map when it is
                // already associated with another map.
                foreach (Layer layer in tempMap.Layers)
                {
                    if (string.IsNullOrEmpty(proxyUrl) == false)
                    {
                        if (layer is ArcGISDynamicMapServiceLayer)
                        {
                            ArcGISDynamicMapServiceLayer dynamicLayer = layer as ArcGISDynamicMapServiceLayer;
                            if (string.IsNullOrEmpty(dynamicLayer.ProxyURL) == true)
                            {
                                dynamicLayer.ProxyURL = proxyUrl;
                            }
                        }
                        else if (layer is ArcGISTiledMapServiceLayer)
                        {
                            ArcGISTiledMapServiceLayer tiledLayer = layer as ArcGISTiledMapServiceLayer;
                            if (string.IsNullOrEmpty(tiledLayer.ProxyURL) == true)
                            {
                                tiledLayer.ProxyURL = proxyUrl;
                            }
                        }
                        else if (layer is FeatureLayer)
                        {
                            FeatureLayer featureLayer = layer as FeatureLayer;
                            if (string.IsNullOrEmpty(featureLayer.ProxyUrl) == true)
                            {
                                featureLayer.ProxyUrl = proxyUrl;
                            }

                            //make sure that bitmap graphics symbols are proxied
                            AddProxyToBitmapSymbol(featureLayer);
                        }
                    }

                    if (visible == false)
                    {
                        layer.Visible = false;
                    }
                    layerCollection.Add(layer);
                }
                tempMap.Layers.Clear();

                UpdateWebMapLayerNames(layerCollection, e.DocumentValues);

                foreach (Layer layer in layerCollection)
                {
                    MapControl.Layers.Insert(index++, layer);
                    logger.Info("Layer Added: " + layer.ID);
                }
                UpdateWebMapIndices(e.ItemInfo.ID, index - _webMapIndices[e.ItemInfo.ID]);

                if (userParams != null && userParams[1] != null)
                {
                    //  this webmap is the basemap, add the rest of the
                    // layers to the map
                    ProcessLayersAndUpdateControls(userParams[1] as XElement);
                }

                Tools.Map = MapControl;
                Tools.LayerControl.InitialzeLayers(Tools.LayerControl, MapControl);
            }
        }

        private void AddProxyToBitmapSymbol(FeatureLayer featureLayer)
        {
            if (featureLayer == null) return;

            GraphicsLayer graphicsLayer = featureLayer as GraphicsLayer;
            if ((graphicsLayer == null) || (graphicsLayer.Graphics == null) || (graphicsLayer.Graphics.Count <= 0)) return;

            foreach (Graphic graphic in graphicsLayer)
            {
                PictureMarkerSymbol pictureMarkerSymbol = graphic.Symbol as PictureMarkerSymbol;
                if (pictureMarkerSymbol != null)
                {
                    BitmapImage imageSource = pictureMarkerSymbol.Source as BitmapImage;
                    if (imageSource.UriSource.ToString().Contains("proxy.ashx") == false)
                    {
                        imageSource.UriSource = new Uri(featureLayer.ProxyUrl + "?" + imageSource.UriSource.ToString());
                    }
                }
            }
        }

        private static void UpdateWebMapLayerNames(LayerCollection mapLayers, IDictionary<string, object> docValues)
        {
            if (docValues.ContainsKey("operationalLayers"))
            {
                var layers = (IEnumerable<object>)docValues["operationalLayers"];
                foreach (var temp in layers)
                {
                    var layer = (IDictionary<string, object>)temp;
                    if (layer.ContainsKey("itemId") &&
                        layer.ContainsKey("title"))
                    {
                        var id = (string)layer["itemId"];
                        if (mapLayers[id] != null)
                        {
                            mapLayers[id].ID = (string)layer["title"];
                        }
                    }
                    else if (layer.ContainsKey("id") && layer.ContainsKey("title"))
                    {

                        var id = (string)layer["id"];
                        if (mapLayers[id] != null)
                        {
                            mapLayers[id].ID = (string)layer["title"];
                        }
                    }
                }
            }

            if (docValues.ContainsKey("baseMap"))
            {
                var baseMap = (IDictionary<string, object>)docValues["baseMap"];
                if (baseMap.ContainsKey("title") &&
                    baseMap.ContainsKey("baseMapLayers"))
                {
                    var baseMapLayers = (IList<object>)baseMap["baseMapLayers"];
                    if (baseMapLayers.Count > 0)
                    {
                        var baseMapData = (IDictionary<string, object>)baseMapLayers[0];
                        if (baseMapData.ContainsKey("id"))
                        {
                            var id = (string)baseMapData["id"];
                            if (mapLayers[id] != null)
                            {
                                mapLayers[id].ID = (string)baseMap["title"];
                            }
                        }
                    }
                }
            }
        }

        // After a webmap basemap has loaded, load the rest of the layers
        // and update controls
        private void ProcessLayersAndUpdateControls(XElement layers)
        {
            ProcessLayers(layers);
            _waitingForWebmap = false;
            SetTraceMaps();
            SetTracingTabVisibility();
            SetEditingTabVisibility();
            SetLayersEventHandlers();
            Identify.MapChanged(Identify.Map);

        }

        private void UpdateWebMapIndices(string webMapID, int count)
        {

            // adjust indices further down the list by the number of layers in the
            // current webmap
            int index = _webMapIndices[webMapID];
            _webMapIndices.Remove(webMapID);

            foreach (string ID in _webMapIndices.Keys.ToList())
            {
                if (_webMapIndices[ID] > index)
                {
                    _webMapIndices[ID] += count;
                }
            }

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


        void Layers_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add && (e.NewItems[0] as Layer).ID == TextLayerID)
            {
                AngleRotator.TempLayer = e.NewItems[0] as GraphicsLayer;
            }
        }

        private void Layer_InitializationFailed(object sender, EventArgs e)
        {
            Layer layer = sender as Layer;
            if (layer == null) return;

            MapControl.Layers.Remove(layer);
            _badLayers.Add(layer.ID);

            string layerNames = string.Join(", ", _badLayers.ToArray());
            string message = string.Format("Layer(s) failed to load: {0}. Service doesn't exist or can't be contacted.", layerNames);
            StatusBar.Text = message;
            logger.Warn(message);
        }

        private void Layer_Initialized(object sender, EventArgs e)
        {
            if (sender is WmsLayer)
            {
                var layer = sender as WmsLayer;
                var layers = new List<string>();
                foreach (var layerInfo in layer.LayerList)
                {
                    GetWMSLayers(layerInfo, layers);
                    logger.Info("Layer {0} initialized", layerInfo.Name);
                }

                layer.Layers = layers.ToArray();
            }
            else if (sender is FeatureLayer)
            {
                var layer = sender as FeatureLayer;
                if (!string.IsNullOrEmpty(_wipLabelLayerName))
                {
                    if (layer.ID == _wipLabelLayerName)
                    {
                        layer.RendererTakesPrecedence = false;
                        WIPEditor.WipLabelLayer = layer;
                        //layer.UpdateCompleted += WIPLabelUpdateCompleted;
                        //layer.EndSaveEdits += WipLabellayer_EndSaveEdits;
                        //layer.SaveEditsFailed += LayerOnSaveEditsFailed;
                    }
                }
                if (layer.ID == _wipPolygonLayerName)
                {
                    WIPEditor.WipPolygonLayer = layer;
                    //layer.EndSaveEdits += LayerOnEndSaveEdits;
                    //layer.SaveEditsFailed += LayerOnSaveEditsFailed;
                    //layer.UpdateCompleted += LayerOnUpdateCompleted;
                }
            }
            else if (sender is ArcGISDynamicMapServiceLayer)
            {
                //After configuration is loaded, let's build our ObjectClassID -> MapServer mapping
                LoadClassIDMapping(sender as ArcGISDynamicMapServiceLayer);
            }
        }

        private void SelectionComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SelectionComboBox != null)
            {
                var prefix = "Selection Management: ";
                switch (SelectionComboBox.SelectedIndex)
                {
                    case 0:
                        _selectionType = SelectionOption.CreateNewSelection;
                        ToolTipService.SetToolTip(SelectionComboBox, prefix + "Replace");
                        break;
                    case 1:
                        _selectionType = SelectionOption.AddToSelection;
                        ToolTipService.SetToolTip(SelectionComboBox, prefix + "Add");
                        break;
                    case 2:
                        _selectionType = SelectionOption.RemoveFromSelection;
                        ToolTipService.SetToolTip(SelectionComboBox, prefix + "Remove");
                        break;
                    case 3:
                        _selectionType = SelectionOption.SelectFromSelection;
                        ToolTipService.SetToolTip(SelectionComboBox, prefix + "Intersect");
                        break;
                }
            }
        }

        private void LockSelected_Click(object sender, RoutedEventArgs e)
        {
            foreach (var resultSet in AttributesViewer.Results)
            {
                foreach (var graphic in resultSet.Features.Where(g => g.Selected))
                {
                    graphic.Attributes["Locked"] = (graphic as GraphicPlus).Locked = true;
                }
            }
        }

        private void UnlockSelected_Click(object sender, RoutedEventArgs e)
        {
            foreach (var resultSet in AttributesViewer.Results)
            {
                foreach (var graphic in resultSet.Features.Where(g => g.Selected))
                {
                    graphic.Attributes["Locked"] = (graphic as GraphicPlus).Locked = false;
                }
            }
        }

        private void RemoveLocks_Click(object sender, RoutedEventArgs e)
        {
            foreach (var resultSet in AttributesViewer.Results)
            {
                foreach (var graphic in resultSet.Features)
                {
                    graphic.Attributes["Locked"] = (graphic as GraphicPlus).Locked = false;
                }
            }
        }

        private void Trace_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {

            Image traceTypeImage = sender as Image;
            string traceTypeName = traceTypeImage.Name;

            List<int> protectiveDeviceList = TraceProtectiveDevice.GetProtectiveDeviceList(traceTypeName);

            foreach (Miner.Server.Client.Tasks.ProtectiveDevice protectiveDevice in ElectricTrace.ProtectiveDevices)
            {
                if (protectiveDeviceList.Contains(protectiveDevice.LayerID))
                {
                    protectiveDevice.IsEnabled = true;
                }
                else
                {
                    protectiveDevice.IsEnabled = false;
                }
            }
        }

        private void GetStartupExtent()
        {
            return;
            //            Tools.SaveStartupExtent.AppSettings = appSettings;

            //if (!appSettings.Contains("extent")) return;

            //Envelope savedExtent = appSettings["extent"] as Envelope;
            //if (savedExtent == null) return;

            ////if (savedExtent.SpatialReference == MapControl.SpatialReference)
            //if (SpatialReference.AreEqual(savedExtent.SpatialReference, MapControl.Layers[0].SpatialReference, false))
            //{
            //    MapControl.Extent = savedExtent;
            //}
        }

        #region Public Methods

        static public T GetChildObject<T>(DependencyObject obj, string name) where T : FrameworkElement
        {
            for (int i = 0; i <= VisualTreeHelper.GetChildrenCount(obj) - 1; i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(obj, i);
                if (child is T && (((T)child).Name == name | string.IsNullOrEmpty(name)))
                    return (T)child;

                var grandChild = GetChildObject<T>(child, name);
                if (grandChild != null)
                    return grandChild;
            }
            return null;
        }

        #endregion Public Methods

        #region Variables for Geographic Search Filter

        private XElement _gsfConfig;

        #endregion Variables for Geographic Search Filter

        #region Geographic Search Filter

        /// <summary>
        /// This method extracts configuration settings from page.config to
        /// be passed into a query for the Geographic Search Filter.
        /// </summary>
        /// <param name="geoSearchFilters">The XElement from the page.config
        /// file that contains the configuration of the geographic search
        /// filters</param>
        private void AddGeographicSearchFilter(XElement geoSearchFilters)
        {
            if (geoSearchFilters == null) return;
            if (geoSearchFilters.HasElements == false) return;

            int filterCount = 0;
            int totalFilters = geoSearchFilters.Elements("Filter").Count();

            foreach (XElement geoSearchFilter in geoSearchFilters.Elements())
            {
                string label = geoSearchFilter.Attribute("FilterLabel").Value.ToString();
                string fieldName = geoSearchFilter.Attribute("FieldNameToDisplay").Value.ToString();
                string filterField = geoSearchFilter.Attribute("SearchFilterField").Value.ToString();
                string url = geoSearchFilter.Attribute("Url").Value.ToString();
                string where = geoSearchFilter.Attribute("Where").Value.ToString();
                string outfields = geoSearchFilter.Attribute("Outfields").Value.ToString();
                filterCount++;

                /*GeoSearchFilter.QueryForGeographicFilters(
                    label, 
                    fieldName, 
                    filterField, 
                    url, 
                    where, 
                    outfields, 
                    filterCount, 
                    totalFilters);
                */
                GeoSearchFilter.IdentifyForGeographicFilters(
                    label,
                    fieldName,
                    filterField,
                    url,
                    where,
                    outfields,
                    filterCount,
                    totalFilters,
                    MapControl);
            }
        }

        /// <summary>
        /// This method filters the results returned from a search based
        /// to a geographic regionName as chosen by the user via the 
        /// Geographic Search Filter control.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e">Result Set</param>
        private void FilterControl_WorkCompleted(object sender, ResultEventArgs e)
        {
            if (e == null || e.Results == null) { AttributesViewer.LoadingResults = false; return; }
            this.AttributesViewer.AutoZoomToResults = true;
            System.Collections.Generic.IEnumerable<Graphic> filteredGraphics;
            System.Collections.ObjectModel.ObservableCollection<Miner.Server.Client.Tasks.IResultSet> newResults = new

System.Collections.ObjectModel.ObservableCollection<Miner.Server.Client.Tasks.IResultSet>();
            System.Collections.Generic.List<Miner.Server.Client.Tasks.IResultSet> oldResults = e.Results.ToList();

            System.Collections.Generic.List<Miner.Server.Client.Tasks.IResultSet> oldResults1 = e.Results.ToList();

            //INC000004022645: S2NN, October 15,2015
            isNoGeographicalResult = false;
            int iNoDivision = 0;
            string regionName = GeoSearchFilter.SelectedRegion;
            string regionType = GeoSearchFilter.RegionType;
            int index = GeoSearchFilter.SelectedIndex;
        nodivision:
            if (isNoGeographicalResult) regionName = "All Areas";

            this.AttributesViewer.AutoZoomToResults = true;
            Miner.Server.Client.Tasks.IResultSet ir1 = new Miner.Server.Client.Tasks.ResultSet();

            //*****************To fix Address Seach Issue****************
            //***********************************************************

            LocateControl lc = sender as LocateControl;
            Grid lcGrid = (Grid)VisualTreeHelper.GetChild(lc, 0);
            string searchType = "Search Device";


            Object selectObj = ((ComboBox)lcGrid.FindName("PART_LocateTypeComboBox")).SelectedItem;

            if (selectObj is CustomAddressSearch)
            {
                searchType = "Search Address";
            }

            else if (selectObj is CustomLatLongSearch)
            {
                searchType = "CustomLatLong";
            }

            try
            {
                if (!isNoGeographicalResult)
                {
                    oldResults1.Clear();
                }
                //INC000004022645: S2NN, October 15,2015
                Miner.Server.Client.Tasks.IResultSet ir = new Miner.Server.Client.Tasks.ResultSet();

                //foreach (Miner.Server.Client.Tasks.IResultSet ir in oldResults)
                for (int iResultSet_count = 0; iResultSet_count < oldResults.Count; ++iResultSet_count)
                {
                    //string regionName = GeoSearchFilter.SelectedRegion;
                    //string regionType = GeoSearchFilter.RegionType;
                    //int index = GeoSearchFilter.SelectedIndex;

                    //INC000004022645: S2NN, October 15,2015
                    ir = oldResults[iResultSet_count];
                    if (!isNoGeographicalResult)
                    {
                        ir1 = new Miner.Server.Client.Tasks.ResultSet() { SpatialReference = ir.SpatialReference };
                        foreach (Graphic graphic in ir.Features)
                            ir1.Features.Add(graphic);
                        oldResults1.Add(ir1);
                    }
                    else if (iNoDivision == 1)
                    {
                        ir1 = oldResults1[iResultSet_count];
                        foreach (Graphic graphic in ir1.Features)
                            ir.Features.Add(graphic);
                    }

                    foreach (Miner.Server.Client.GraphicPlus gp in ir.Features)
                    {
                        if (gp.Attributes.ContainsKey("DIVISION"))
                        {
                            string divisionName = null;
                            string divisionCodeAsString = "";
                            if (gp.Attributes["DIVISION"] != null)
                            {
                                divisionCodeAsString = gp.Attributes["DIVISION"].ToString();
                            }
                            int divisionCode;
                            bool isInt = int.TryParse(divisionCodeAsString, out divisionCode);

                            if (isInt)
                            {
                                if (Application.Current.Resources.Contains("DivisionCodes") && Application.Current.Resources["DivisionCodes"] != null)
                                {
                                    Dictionary<int, string> appDivsionCodes = (Dictionary<int, string>)Application.Current.Resources["DivisionCodes"];
                                    if (appDivsionCodes.ContainsKey(divisionCode))
                                    {
                                        divisionName = appDivsionCodes[divisionCode];
                                    }
                                }

                                //Locate comparison is case-sensitive. Need to use uppercase.
                                gp.Attributes["DIVISION"] = divisionName.ToUpper();
                                logger.Info("GEOSEARCHFILTER: Domain value inserted: " + divisionName + ", Division Code: " + divisionCode);
                            }
                            else
                            {
                                //MessageBox.Show("Division ID cannot be parsed into an integer. ", "Warning: Parsing Division Domain ID", MessageBoxButton.OK);
                                logger.Warn("WRN_GEOSEARCHFILTER: Domain code is not int. Div Code: " + divisionCode);
                            }
                        }
                    }

                    if (!string.IsNullOrEmpty(regionName) && regionName != "All Areas" && searchType == "Search Device")
                    {
                        //Filters on selection from GSF combobox selection.
                        filteredGraphics = from result in ir.Features
                                           where
                                            (result.Attributes[regionType] != null) &&
                                            (result.Attributes[regionType].ToString() == regionName)
                                           select result;
                        //logger.Info("GEOSEARCHFILTER: Results filtered on: " + regionName);
                        if (filteredGraphics.Count() == 0)
                        {
                            logger.Info("GEOSEARCHFILTER: Results not filtered, there is no result for given region");
                        }
                        else
                        {
                            logger.Info("GEOSEARCHFILTER: Results filtered on: " + regionName);
                        }
                    }
                    else if (searchType == "Search Address")
                    {
                        //Nothing filtered. Takes all ir.Features.
                        filteredGraphics = from result in ir.Features
                                           orderby result.Attributes["SCORE"] descending
                                           /* where result.Attributes.Values.Contains(queryString) */
                                           select result;
                        this.AttributesViewer.AutoZoomToResults = false;
                        logger.Info("GEOSEARCHFILTER: Results not filtered.");
                    }

                    else if (searchType == "CustomLatLong")
                    {
                        filteredGraphics = from result in ir.Features
                                           /* where result.Attributes.Values.Contains(queryString) */
                                           select result;
                        CustomLatLongSearch seachitem = ((CustomLatLongSearch)((((ComboBox)lcGrid.FindName("PART_LocateTypeComboBox")).SelectedItem)));
                        CoordinatePairs coordinatePairs = seachitem.Coordinates;
                        //coordinatePairs.MapCoordinatePoint = ir.Features[0].Geometry as MapPoint ;
                        //coordinatePairs.WGSPoint = wgsPoint;

                        InfoWindow infoWindow = new InfoWindow();
                        infoWindow.Name = "InfoWindowsearch" + _windowCount++;
                        //_dictionaryCoordinatePoints.Add(infoWindow.Name, coordinatePairs);

                        string ElementLayoutGrid = "PART_ControlLayout";
                        //string LatLongTemplate = "LatlonTemplate";

                        Grid _layoutGrid;
                        _layoutGrid = GetTemplateChild(ElementLayoutGrid) as Grid;
                        infoWindow.Anchor = coordinatePairs.MapCoordinatePoint;
                        infoWindow.Map = MapControl;
                        infoWindow.IsOpen = true;
                        infoWindow.ContentTemplate = CreateLocationTemplate(seachitem.IsWgs) as DataTemplate;
                        infoWindow.MouseLeftButtonDown += new MouseButtonEventHandler(infoWindow_MouseLeftButtonDown);

                        Border border = VisualTreeHelper.GetChild(MapControl, 0) as Border;
                        Grid grid = VisualTreeHelper.GetChild(border, 0) as Grid;
                        grid.Children.Add(infoWindow);

                        if (seachitem.IsWgs)
                        {
                            LatLongDisplayObj lld = new LatLongDisplayObj(coordinatePairs.WGSPoint.Y, coordinatePairs.WGSPoint.X);
                            infoWindow.Content = lld;
                        }
                        else
                        {
                            infoWindow.Content = coordinatePairs.MapCoordinatePoint;
                        }

                        ZoomtoAddress(coordinatePairs.MapCoordinatePoint.Extent as Envelope);

                        SelectionComboBox.Visibility = Visibility.Visible;
                        LockingTools.Visibility = Visibility.Visible;
                        AttributesViewer.LoadingResults = false;
                        StatusBar.Text = "";
                        SendResultsToAttibuteViewer(e.Results);
                        SetupDataGridContextMenu();

                        return;

                    }
                    else
                    {
                        //Nothing filtered. Takes all ir.Features.
                        filteredGraphics = from result in ir.Features
                                           /* where result.Attributes.Values.Contains(queryString) */
                                           select result;

                        logger.Info("GEOSEARCHFILTER: Results not filtered.");
                    }

                    System.Collections.Generic.List<Graphic> _myGraphics = filteredGraphics.ToList();

                    ir.Features.Clear();

                    foreach (Graphic g in _myGraphics)
                    {
                        ir.Features.Add((Miner.Server.Client.GraphicPlus)g);
                    }

                    newResults.Add(ir);
                    e.Results = newResults;
                    logger.Info("GEOSEARCHFILTER: e.Results updated.");
                }
                //INC000004022645: S2NN, October 15,2015
                if (isNoGeographicalResult || (!string.IsNullOrEmpty(regionName) && regionName != "All Areas" && searchType == "Search Device"))
                {
                    int icount = 0;//, icount1 = 0;

                    for (int i = 0; i < newResults.Count; ++i)
                    {
                        icount += newResults[i].Features.Count;
                    }

                    if (icount == 0)
                    {
                        isNoGeographicalResult = !isNoGeographicalResult;
                        if (isNoGeographicalResult)
                        {
                            iNoDivision = 1;
                            newResults = new System.Collections.ObjectModel.ObservableCollection<Miner.Server.Client.Tasks.IResultSet>();
                            goto nodivision;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error("ERR_GEOSEARCHFILTER: Results Filter exception: " + ex);
            }

            //INC000004378902
            AttributesViewer.selectedStoredView = this.Tools.StoredViewControl._selectedStoredView.StoredViewName.ToString();
            AttributesViewer.isSearch = true;
            AttributesViewer.isSingleSearch = false;
            MapControl.ExtentChanged += new EventHandler<ExtentEventArgs>(Map_ExtentChanged_Search);
            try
            {
                Tools.ZoomToScaleControl.lastScaleListValue = Tools.ZoomToScaleControl.GetLastScaleListValue();
                this.Control_WorkCompleted(sender, e);
                AttributesViewer.isSearch = false;
            }
            catch
            {
                MapControl.ExtentChanged -= new EventHandler<ExtentEventArgs>(Map_ExtentChanged_Search);
            }
        }

        private void Map_ExtentChanged_Search(object sender, ExtentEventArgs e)
        {
            MapControl.ExtentChanged -= new EventHandler<ExtentEventArgs>(Map_ExtentChanged_Search);
            if (AttributesViewer.isSingleSearch)
            {
                Tools.ZoomToScaleControl.SetScaleValue(MapControl.Resolution);
            }
        }

        private DataTemplate CreateLocationTemplate(bool IsWgs)
        {
            string template;
            if (!IsWgs)
            {

                template = @"
                            <DataTemplate
            					xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation"">
                                <StackPanel Orientation=""Vertical"" Margin=""2"">
                                    <TextBlock Text=""Location:"" />
                                    <TextBlock Text=""{Binding X, StringFormat=X\=\{0:0.0000\}}"" />
                                    <TextBlock Text=""{Binding Y, StringFormat=Y\=\{0:0.0000\}}"" />
                                </StackPanel>
                            </DataTemplate>";
            }
            else
            {
                template = @"
                            <DataTemplate
            					xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation"">
                                <StackPanel Orientation=""Vertical"" Margin=""2"">
                                    <TextBlock Text=""Location:"" />
                                    <TextBlock Text=""{Binding Lat, StringFormat=Lat\=\{0:0.0000\}}"" />
                                    <TextBlock Text=""{Binding Long, StringFormat=Long\=\{0:0.0000\}}"" />
                                </StackPanel>
                            </DataTemplate>";
            }

            return XamlReader.Load(template) as DataTemplate;
        }

        void infoWindow_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            InfoWindow infoWindow = sender as InfoWindow;
            if (infoWindow != null)
            {
                e.Handled = true;
                Border border = VisualTreeHelper.GetChild(MapControl, 0) as Border;
                (border.Child as Grid).Children.Remove(infoWindow);
                //_dictionaryCoordinatePoints.Remove(infoWindow.Name);
            }
            //throw new NotImplementedException();
        }

        #endregion Geographic Search Filter

        #region Variables for PageTemplates

        private ChildWindow _templateWindow;
        PrintPreview _ppWindow;
        IList<IPageTemplate> _allPageTemplates = new System.Collections.Generic.List<IPageTemplate>();
        private List<bool> _layerVisibilities = new List<bool>();

        #endregion Variables for PageTemplates

        public void ZoomtoAddress(ESRI.ArcGIS.Client.Geometry.Envelope addressGraphic)
        {
            Envelope envelope = new Envelope();

            envelope.XMax = addressGraphic.Extent.XMax + 200;
            envelope.XMin = addressGraphic.Extent.XMin - 200;
            envelope.YMax = addressGraphic.Extent.YMax + 200;
            envelope.YMin = addressGraphic.Extent.YMin - 200;

            MapControl.ZoomTo(envelope);

        }

        #region Page Templates Code

        private int _cntr = 0;
        /// <summary>
        /// Launches the PrintPreview child window where user selects may types
        /// and/or Grid Layers and Grid Numbers to view in the Page Template
        /// Viewer window.
        /// </summary>
        /// <param name="sender">Not used</param>
        /// <param name="e">Not used</param>
        private void TemplateButtonClick(object sender, RoutedEventArgs e)
        {
            this.ColorPicker.SelectedIndex = 5;
            this.TracebufferChk.IsChecked = true;
            this.FlowAnimationChk.IsChecked = true;

            Button btnSender = sender as Button;
            if (btnSender.Name == "AdHocTemplateButton")
            {
                //MessageBox.Show("Ad Hoc Template Button Clicked");
                //AdHocPrintWindow ahpw = new AdHocPrintWindow();//(_adHocPanVisibility, _panValue);
                AdHocPrintWindow ahpw = new AdHocPrintWindow(_adHocPanVisibility, _panValue);   //INC000004049426 & INC000004413542
                ahpw.Width = ActualWidth - 20d;
                ahpw.Height = ActualHeight - 20d;
                ahpw.Closed += new EventHandler(ahpw_Closed);
                ahpw.AdHocMap = MapControl;
                ahpw.CurrentExtent = MapControl.Extent;
                ahpw.CenterPoint = MapControl.Extent.GetCenter();
                ahpw.UserEmail = WebContext.Current.User.Name.Replace("PGE\\", "") + "@pge.com";
                ahpw.StoredDisplayName = this.Tools.StoredViewControl._selectedStoredView.StoredViewName;
                ahpw.Show();
            }
            else if (btnSender.Name == "CMCSTemplateButton")   //INC000004403856
            {
                //PageTemplates.Controls.CMCSPrintWindow cmcspw = new PageTemplates.Controls.CMCSPrintWindow();
                PageTemplates.Controls.CMCSPrintWindow cmcspw = new PageTemplates.Controls.CMCSPrintWindow(_adHocPanVisibility, _panValue);   //INC000004049426 & INC000004413542
                cmcspw.Width = ActualWidth - 20d;
                cmcspw.Height = ActualHeight - 20d;
                cmcspw.Closed += new EventHandler(cmcspw_Closed);

                cmcspw.CMCSMap = MapControl;
                cmcspw.CurrentExtent = MapControl.Extent;
                cmcspw.CenterPoint = MapControl.Extent.GetCenter();
                cmcspw.UserEmail = WebContext.Current.User.Name.Replace("PGE\\", "") + "@pge.com";
                cmcspw.StoredDisplayName = this.Tools.StoredViewControl._selectedStoredView.StoredViewName;
                cmcspw.CircuitSourceUrl = _cmcsCircuitSourceUrl;
                cmcspw.CircuitIDLayerIds = _cmcsCircuitIDLayerIds;
                cmcspw.CircuitIDLayersService = _cmcsCircuitIDLayersService;
                cmcspw.MapNo_LayerIdList = _cmcsMapNo_LayerId;
                cmcspw.MapNo_LayerNameList = _cmcsMapNo_LayerName;
                cmcspw.MapNo_LayerFieldList = _cmcsMapNo_LayerField;
                cmcspw.MapNoUrl = _cmcsMapNumberUrl;
                cmcspw.MapDimensionsElement = _cmcsMapDimensions;
                cmcspw.Show();
            }
            else if (btnSender.Name == "StandardTemplateButton")
            {
                StandardMapPrintWindow smpw = new StandardMapPrintWindow();
                smpw.CurrentMap = MapControl;
                smpw.Closed += new EventHandler(smpw_Closed);
                smpw.Show();
            }

            //Get map layer opacities to be used in page template viewer.
            Dictionary<string, double> _layerOpacities = new Dictionary<string, double>();

            foreach (Layer l in MapControl.Layers)
            {
                if (l.ID == null)
                {
                    l.ID = "GraphicsLayer" + _cntr.ToString();
                    _cntr++;
                }
                if (!_layerOpacities.ContainsKey(l.ID))
                {
                    _layerOpacities.Add(l.ID, l.Opacity);
                }
            }

            if (Application.Current.Resources.Contains("MapLayerOpacities"))
            {
                Application.Current.Resources.Remove("MapLayerOpacities");
            }
            Application.Current.Resources.Add("MapLayerOpacities", _layerOpacities);

            /*
            PrintPreview pp = new PrintPreview();
            pp.Map = MapControl;
            pp.Closed += new EventHandler(PrintPreviewChildWindow_Closed);

            pp.Show();*/
        }

        void smpw_Closed(object sender, EventArgs e)
        {
            //MessageBox.Show("Standard Map Print Window Closed");
        }

        void ahpw_Closed(object sender, EventArgs e)
        {
            GC.Collect();
        }

        //INC000004403856
        void cmcspw_Closed(object sender, EventArgs e)
        {
            GC.Collect();
        }

        /// <summary>
        /// Handles the closing of the PrintPreview child window and initiates
        /// the launch of the Page Template viewer window.
        /// </summary>
        /// <param name="sender">Print Preview control</param>
        /// <param name="e">Not used</param>
        void PrintPreviewChildWindow_Closed(object sender, EventArgs e)
        {
            _ppWindow = (PrintPreview)sender;

            if (_ppWindow.DialogResult == true)
            {
                FilterMEFPageTemplates(_ppWindow.MapTypeSelection);
                if (_ppWindow.MapTypeSelection == "Ad Hoc")
                {
                    _savePointLayer = _pointLayer;
                    _saveTextLayer = _textLayer;
                }
                else
                {
                    _savePointLayer = _pointLayer;
                    _saveTextLayer = _textLayer;
                    //toggleFeatureLayerVisibility(false);
                }
                PrintPreviewButtonClick();
            }
        }

        /// <summary>
        /// This method is used to maintain Layer visibility settings of the main
        /// MapControl within the viewer. This allows the user to return from the
        /// page template viewer and have the same visibility settings on Layers
        /// that they had before accessing the page template viewer.
        /// </summary>
        /// <param name="isVisible">Bool value that sets layer visibility</param>
        private void toggleFeatureLayerVisibility(bool isVisible)
        {
            if (_ppWindow.MapTypeSelection != "Ad Hoc")
            {
                if (isVisible && _layerVisibilities.Count > 0)
                {
                    for (int i = 0; i < MapControl.Layers.Count - 1; i++)
                    {
                        MapControl.Layers[i].Visible = _layerVisibilities[i];
                    }
                    _layerVisibilities.Clear();
                }
                else
                {
                    foreach (Layer l in MapControl.Layers)
                    {
                        _layerVisibilities.Add(l.Visible);
                        l.Visible = isVisible;

                        /*if (l is FeatureLayer || l is GraphicsLayer)
                        {
                            l.Visible = isVisible;
                        }*/

                    }
                }
            }
        }

        /// <summary>
        /// This method filters the PageTemplates contained in the MEF Composable
        /// object to contain only those selected from the PrintPreview dialog.
        /// (IE: "Distribution" templates)
        /// </summary>
        /// <param name="templateTypeName">The template type to filter for</param>
        private void FilterMEFPageTemplates(string templateTypeName)
        {
            IComposable _icmo = MEFHelper.Composable.ElementAt(1);
            Miner.Server.Client.ComposePageTemplates cpts = _icmo as Miner.Server.Client.ComposePageTemplates;

            IList<IPageTemplate> _enumKeepTemplates = new System.Collections.Generic.List<IPageTemplate>();

            foreach (IPageTemplate curPT in cpts.PageTemplates.ToList())
            {
                if (curPT.Name.Contains(templateTypeName))
                {
                    _enumKeepTemplates.Add(curPT);
                }

                if (!_allPageTemplates.Contains(curPT))
                {
                    _allPageTemplates.Add(curPT);
                }
            }

            cpts.PageTemplates = _enumKeepTemplates as IEnumerable<IPageTemplate>;
        }

        /// <summary>
        /// This method removes the PageTemplate filter applied to the MEF
        /// Composable object and restores all PageTemplates to the collection. 
        /// </summary>
        private void RemoveMEFPageTemplateFilter()
        {
            IComposable _icmbRestore = MEFHelper.Composable.ElementAt(1);
            Miner.Server.Client.ComposePageTemplates cptRestore = _icmbRestore as Miner.Server.Client.ComposePageTemplates;

            cptRestore.PageTemplates = _allPageTemplates;
        }

        /// <summary>
        /// Launches the Page Template Viewer child window. Templates selectable 
        /// are based off of the filtered MEF catalog for Standard Map Types. If
        /// Ad Hoc, all currently visible layers will be passed through to the viewer.
        /// </summary>
        private void PrintPreviewButtonClick()
        {
            if (_templateWindow == null)
            {
                _templateWindow = new ChildWindow
                {
                    HasCloseButton = true,
                    Title = "PG&E Print",
                    Content = _pageTemplateViewer = new PageTemplateViewer(_savePointLayer as GraphicsLayer, _saveTextLayer as GraphicsLayer, _resolution,

_unscaledMarkerSize, AngleRotator.TypeIdField)
                    {
                        Map = MapControl
                    },
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Center
                };
            }
            _templateWindow.Width = ActualWidth - 20d;
            _templateWindow.Height = ActualHeight - 20d;
            _templateWindow.Closed += TemplateWindow_Closed;
            _templateWindow.Show();

            _pageTemplateViewerClosed = false;

            TextBlock text = GetElement(_pageTemplateViewer, typeof(TextBlock)) as TextBlock;
            text.Text = "Map Size:";

        }


        /// <summary>
        /// Closes down the Page Template viewer window and restores the 
        /// MEF Page Template Catalog.
        /// </summary>
        /// <param name="sender">Not used</param>
        /// <param name="e">Not used</param>
        void TemplateWindow_Closed(object sender, EventArgs e)
        {
            if (!_pageTemplateViewerClosed)
            {
                _pageTemplateViewerClosed = true;

                var templates = (_pageTemplateViewer.DataContext as PageTemplateLibraryViewModel).Templates;
                foreach (var template in templates)
                {
                    template.ClearMapLayers();
                }
                GC.Collect();

                MapControlExtentChanged(null, null);
                RemoveMEFPageTemplateFilter();
                _templateWindow = null;
            }

            toggleFeatureLayerVisibility(true);

        }

        #endregion Page Templates Code

        #region webMapStuff

        private void MapControlMouseClick(object sender, Map.MouseEventArgs e)
        {
            if (CoordinatesTool.IsActive) return;

            var popupData = WebmapPopupGetDataTemplate();
            WebmapDisplayPopup(e.MapPoint, popupData);

        }

        private void WebmapDisplayPopup(MapPoint mapPoint, Stack<WebmapPopupInfo> popupData)
        {
            if (popupData.Count > 0)
            {
                var data = popupData.Pop();
                var qt = new QueryTask(string.Format("{0}/{1}", data.url, data.layerID));
                qt.ExecuteCompleted += (s, qe) =>
                {
                    if (qe.FeatureSet.Features.Count > 0)
                    {
                        Graphic g = qe.FeatureSet.Features[0];
                        CreateInfoWindow(mapPoint, data.dt, g.Attributes);
                    }
                    else
                    {
                        WebmapDisplayPopup(mapPoint, popupData);
                    }
                };

                var query = new ESRI.ArcGIS.Client.Tasks.Query();
                double buffer = 5 * MapControl.Resolution;
                var inputEnvelope = new Envelope(mapPoint.X - buffer,
                    mapPoint.Y - buffer,
                    mapPoint.X + buffer,
                    mapPoint.Y + buffer);
                inputEnvelope.SpatialReference = MapControl.SpatialReference;
                query.Geometry = inputEnvelope;
                query.OutSpatialReference = MapControl.SpatialReference;
                query.OutFields.Add("*");

                qt.ExecuteAsync(query);
            }
        }

        private Stack<WebmapPopupInfo> WebmapPopupGetDataTemplate()
        {
            string layerUrl;
            var popupData = new Stack<WebmapPopupInfo>();
            foreach (Layer layer in MapControl.Layers)
            {
                if (layer.Visible == true && layer.GetValue(Document.PopupTemplatesProperty) != null)
                {
                    double scaleConversion;
                    IDictionary<int, DataTemplate> idict =
                        layer.GetValue(Document.PopupTemplatesProperty) as IDictionary<int, DataTemplate>;
                    if (idict == null) continue;

                    LayerInfo[] layerInfo;
                    int[] visibleLayers = null;
                    if (layer is ArcGISDynamicMapServiceLayer)
                    {
                        var alayer = layer as ArcGISDynamicMapServiceLayer;
                        layerInfo = alayer.Layers;
                        visibleLayers = alayer.VisibleLayers;
                        layerUrl = alayer.Url;
                        scaleConversion = _mapUnitsToInches.ContainsKey(alayer.Units)
                                               ? _mapUnitsToInches[alayer.Units]
                                               : _mapUnitsToInches["esriMeters"];
                    }
                    else if (layer is ArcGISTiledMapServiceLayer)
                    {
                        var alayer = layer as ArcGISTiledMapServiceLayer;
                        layerInfo = alayer.Layers;
                        layerUrl = alayer.Url;
                        scaleConversion = _mapUnitsToInches.ContainsKey(alayer.Units)
                                                ? _mapUnitsToInches[alayer.Units]
                                                : _mapUnitsToInches["esriMeters"];
                    }
                    else
                    {
                        continue;
                    }

                    double mapScale = MapControl.Resolution * scaleConversion * 96;

                    foreach (var linfo in layerInfo)
                    {
                        if (idict.ContainsKey(linfo.ID) && // id present in dictionary
                            (visibleLayers == null || visibleLayers.Contains(linfo.ID)) && // layer visibility set to true in TOC
                            ((mapScale > linfo.MaxScale && mapScale < linfo.MinScale) || // in scale range
                            (linfo.MaxScale == 0.0 && linfo.MinScale == 0.0) || // no scale dependency
                            (mapScale > linfo.MaxScale && linfo.MinScale == 0.0))) // minscale == 0.0 = infinitiy
                        {
                            var data = new WebmapPopupInfo(idict[linfo.ID], layerUrl, linfo.ID);
                            popupData.Push(data);
                        }
                    }
                }
            }

            return popupData;
        }

        private void CreateInfoWindow(MapPoint mapPoint, DataTemplate dt, IDictionary<string, object> content)
        {
            InfoWindow infoWindow = new InfoWindow();
            infoWindow.Name = "WebmapInfoWindow" + _windowCount++;

            infoWindow.Anchor = mapPoint;
            infoWindow.Map = MapControl;
            infoWindow.IsOpen = true;
            infoWindow.ContentTemplate = dt;
            infoWindow.Content = content;
            infoWindow.MouseLeftButtonDown += WebmapPopupInfoWindowMouseLeftButtonDown;

            Border border = VisualTreeHelper.GetChild(MapControl, 0) as Border;
            Grid grid = VisualTreeHelper.GetChild(border, 0) as Grid;
            grid.Children.Add(infoWindow);
        }

        private void WebmapPopupInfoWindowMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var infoWindow = sender as InfoWindow;
            if (infoWindow != null)
            {
                e.Handled = true;
                infoWindow.IsOpen = false;
                Border border = VisualTreeHelper.GetChild(MapControl, 0) as Border;
                Grid grid = VisualTreeHelper.GetChild(border, 0) as Grid;
                grid.Children.Remove(infoWindow);
            }
        }

        private struct WebmapPopupInfo
        {
            public DataTemplate dt;
            public string url;
            public int layerID;

            public WebmapPopupInfo(DataTemplate dt, string url, int id)
            {
                this.dt = dt;
                this.url = url;
                this.layerID = id;
            }
        }

        #endregion webMapStuff

        private UIElement GetElement(UIElement parent, Type targetType)
        {
            if (parent == null) return null;

            if (parent.GetType() == targetType)
            {
                return parent;
            }
            UIElement result = null;
            int count = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < count; i++)
            {
                UIElement child = (UIElement)VisualTreeHelper.GetChild(parent, i);
                if (GetElement(child, targetType) != null)
                {
                    result = GetElement(child, targetType);
                    break;
                }
            }
            return result;
        }



        private void FlashTraceResult(object sender, RoutedEventArgs e)
        {
            PGETracingControl.FlashFeature_Click(TraceResultsList, null);
        }

        private void ZoomToTraceResult(object sender, RoutedEventArgs e)
        {
            PGETracingControl.ZoomToFeature_Click(TraceResultsList, null);
        }

        private void SelectTraceResult(object sender, RoutedEventArgs e)
        {
            PGETracingControl.SelectFeature_Click(TraceResultsList, null);
        }

        private void LoadingInformationResult(object sender, RoutedEventArgs e)
        {
            PGETracingControl.LoadingInformation_Click(TraceResultsList, null);
        }

        private void TraceResultsList_MouseButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            e.Handled = true;
        }

        private ContextMenu contextMenu = null;

        private void selectTraceResultsUnderClick(MouseButtonEventArgs e)
        {
            IEnumerable childrenUnderMouse = VisualTreeHelper.FindElementsInHostCoordinates(e.GetPosition(LayoutRoot), TraceResultsList);
            foreach (UIElement element in childrenUnderMouse)
            {
                if (element is ListBoxItem)
                {
                    ListBoxItem item = element as ListBoxItem;
                    item.IsSelected = true;
                    //int index = TraceResultsList.Items.IndexOf(((TreeViewItem)element).content);
                    //TraceResultsList.SelectedIndex = index;
                }
            }
        }

        private void TraceResultsList_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            TraceItem selectedItem = TraceResultsList.SelectedItem as TraceItem;
            if (selectedItem != null)
            {
                if (selectedItem.ChildrenVisibleCurrently == System.Windows.Visibility.Visible)
                {
                    selectedItem.ChildrenVisibleCurrently = System.Windows.Visibility.Collapsed;
                    ShowTraceResults.IsChecked = false;
                    ShowTraceResults.IsChecked = true;
                    TraceResultsPopup.UpdateLayout();
                    ((ListBoxItem)TraceResultsList.ItemContainerGenerator.ContainerFromItem(TraceResultsList.SelectedItem)).Focus();
                }
                else { selectedItem.ChildrenVisibleCurrently = System.Windows.Visibility.Visible; }
            }
        }

        private void TraceResultsList_MouseRightButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            selectTraceResultsUnderClick(e);

            contextMenu = new ContextMenu();
            MenuItem FlashFeature = new MenuItem();
            FlashFeature.Click += new RoutedEventHandler(FlashTraceResult);
            FlashFeature.Header = "Flash";
            MenuItem ZoomToFeature = new MenuItem();
            ZoomToFeature.Click += new RoutedEventHandler(ZoomToTraceResult);
            ZoomToFeature.Header = "Zoom To";
            MenuItem SelectFeature = new MenuItem();
            SelectFeature.Click += new RoutedEventHandler(SelectTraceResult);
            SelectFeature.Header = "Add To Selection";
            MenuItem LoadingInformation = new MenuItem();
            LoadingInformation.Click += new RoutedEventHandler(LoadingInformationResult);
            LoadingInformation.Header = "Loading Information";

            contextMenu.Items.Add(FlashFeature);
            contextMenu.Items.Add(ZoomToFeature);
            contextMenu.Items.Add(SelectFeature);
            contextMenu.Items.Add(LoadingInformation);
            //ContextMenuService.SetContextMenu((DependencyObject)sender, contextMenu);
            contextMenu.IsOpen = true;
            contextMenu.HorizontalOffset = e.GetPosition(LayoutRoot).X;
            contextMenu.VerticalOffset = e.GetPosition(LayoutRoot).Y;
        }

        private void InitTracingSettings(object sender, EventArgs e)
        {
            if (_traceBuferStartup)
            {
                try
                {
                    Boolean isChecked = Boolean.Parse(SettingHelper.ReadSetting("PGE_Tracing", "FeederFedChk", "Value").ToString());
                    FeederFedChk.IsChecked = isChecked;
                }
                catch { FeederFedChk.IsChecked = true; }

                try
                {
                    Boolean isChecked = Boolean.Parse(SettingHelper.ReadSetting("PGE_Tracing", "TraceBufferSwitch", "Value").ToString());
                    TracebufferChk.IsChecked = isChecked;
                }
                catch { TracebufferChk.IsChecked = true; }

                try
                {
                    Boolean isChecked = Boolean.Parse(SettingHelper.ReadSetting("PGE_Tracing", "FlowSwitch", "Value").ToString());
                    FlowAnimationChk.IsChecked = isChecked;
                }
                catch { FlowAnimationChk.IsChecked = true; }

                try
                {
                    string bufferSize = SettingHelper.ReadSetting("PGE_Tracing", "TraceBufferSize", "Value").ToString();
                    TraceBufferSizeText.Text = bufferSize;
                }
                catch { TraceBufferSizeText.Text = "3"; }

                _traceBuferStartup = false;

                //INC000004024075
                ColorPicker.SelectedIndex = 5;
                FlashColorPicker.SelectedIndex = 8;
                FlowAnimationColorPicker.SelectedIndex = 10;

                BindingExpression binding = ColorPicker.GetBindingExpression(ComboBox.SelectedIndexProperty);
                if (null != binding) binding.UpdateSource();

                binding = TracebufferChk.GetBindingExpression(CheckBox.IsCheckedProperty);
                if (null != binding) binding.UpdateSource();

                binding = FeederFedChk.GetBindingExpression(CheckBox.IsCheckedProperty);
                if (null != binding) binding.UpdateSource();

                PGETracingControl.LoadingInfo = LoadingInfo;

                ColorPicker.UpdateLayout();
                TracebufferChk.UpdateLayout();
            }
        }

        private void InitTraceSettings()
        {
            /*
            Dictionary<string, bool> deviceChecked = new Dictionary<string, bool>();
            object protectiveDevices = SettingHelper.ReadSetting("PGE_Tracing", "ProtectiveDevices", "Value", "System.String");
            if (protectiveDevices != null)
            {
                string[] protDevices = Regex.Split(protectiveDevices.ToString(), ";");
                foreach (string protDevice in protDevices)
                {
                    if (string.IsNullOrEmpty(protDevice)) { continue; }
                    string[] device = Regex.Split(protDevice, ",");
                    deviceChecked.Add(device[0], Boolean.Parse(device[1]));
                }
            }
            foreach (UIElement element in ProtectiveDevicesStackPanel.Children)
            {
                if (element is CheckBox)
                {
                    CheckBox checkBox = element as CheckBox;
                    string protDeviceName = checkBox.Content.ToString();
                    if (deviceChecked.ContainsKey(protDeviceName))
                    {
                        checkBox.IsChecked = deviceChecked[protDeviceName];
                    }
                    else
                    {
                        //We haven't cached this device before.  Let's do that now.
                        checkBox.IsChecked = true;
                    }
                }
            }
            */
        }

        private void ReadDivisionCodes(XElement divisionCodesElement)
        {
            try
            {
                if (Application.Current.Resources.Contains("DivisionCodes"))
                {
                    Application.Current.Resources.Remove("DivisionCodes");
                }

                if (divisionCodesElement == null) return;
                if (divisionCodesElement.HasElements == false) return;
                Dictionary<int, string> divisionCodes = new Dictionary<int, string>();

                foreach (XElement divisionCodeElement in divisionCodesElement.Elements())
                {
                    divisionCodes.Add((int)divisionCodeElement.Attribute("ID"), (string)divisionCodeElement.Attribute("Value"));
                }

                Application.Current.Resources.Add("DivisionCodes", divisionCodes);
            }
            catch (Exception e)
            {
                MessageBox.Show("Exception Details: " + Environment.NewLine + e.Message, "Failed Reading Division Codes From Configuration", MessageBoxButton.OK);
            }
        }

        private void ReadProposedLayerIDs(XElement proposedLayerIdLookupsElement)
        {
            try
            {
                if (proposedLayerIdLookupsElement == null || !proposedLayerIdLookupsElement.HasElements) return;

                foreach (XElement mapTypeElement in proposedLayerIdLookupsElement.Elements())
                {
                    if (!mapTypeElement.HasElements) continue;

                    string mapName = mapTypeElement.Name.ToString();
                    List<Tuple<string, string, string>> proposedLayerIdConversionList = new List<Tuple<string, string, string>>();


                    foreach (XElement proposedLayerIdLookupElement in mapTypeElement.Elements())
                    {
                        proposedLayerIdConversionList.Add(new Tuple<string, string, string>(
                            (string)proposedLayerIdLookupElement.Attribute("LayerName"),
                            (string)proposedLayerIdLookupElement.Attribute("ProposedLayerID"),
                            (string)proposedLayerIdLookupElement.Attribute("NonProposedLayerID")));

                        if (proposedLayerIdLookupElement.HasElements)
                        {
                            List<Tuple<string, string, string>> differentiatingRuleList = new List<Tuple<string, string, string>>();

                            foreach (XElement differentiatingRuleElement in proposedLayerIdLookupElement.Elements())
                            {
                                differentiatingRuleList.Add(new Tuple<string, string, string>(
                                    (string)differentiatingRuleElement.Attribute("AttributeField"),
                                    (string)differentiatingRuleElement.Attribute("RequiredValues"),
                                    (string)differentiatingRuleElement.Attribute("NonProposedLayerID")));
                            }
                            //Dictionary<ProposedLayerName, List<Tuple<AttributeField, RequiredValues, NonProposedLayerID>>
                            _differentiatingRulesDictionary.Add(mapName + "-" + (string)proposedLayerIdLookupElement.Attribute("LayerName"), differentiatingRuleList);

                            //for INC000004215813 - Opened Switch/Fuse/DPD in ElectricDistribution and EDMaster
                        }
                    }
                    //Dictionary<Map Name, List<Tuple<LayerName, ProposedID, NonProposedLayerIDs>>
                    _proposedLayerIdConversionDictionary.Add(mapName, proposedLayerIdConversionList);
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("Exception Details: " + Environment.NewLine + e.Message, "Failed Reading Proposed Layer ID Conversions from Configuration",

MessageBoxButton.OK);
            }
        }

        private void TraceResultsList_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (sender is TreeView)
            {
                TreeView treeView = sender as TreeView;
                if (treeView.SelectedItem is TraceItem)
                {
                    TraceItem currentTraceItem = treeView.SelectedItem as TraceItem;
                    if (!currentTraceItem.FieldInformationObtained)
                    {
                        TracingHelper.GetTraceItemInformation(currentTraceItem, PGETracingControl.tracingHelper.CurrentResultsAreSchematics);
                    }
                }
            }
        }

        private void ListBox_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (sender is ListBox)
            {
                if (((ListBox)sender).Visibility == System.Windows.Visibility.Collapsed)
                {
                    ShowTraceResults.IsChecked = false;
                    ShowTraceResults.IsChecked = true;
                }
            }
        }

        private void TraceResultsList_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Left || e.Key == Key.Right)
            {
                Visibility newVisibility = Visibility.Visible;
                if (e.Key == Key.Left) { newVisibility = Visibility.Collapsed; }

                TraceItem selectedItem = TraceResultsList.SelectedItem as TraceItem;
                if (selectedItem != null)
                {
                    selectedItem.ChildrenVisibleCurrently = newVisibility;
                    if (newVisibility == System.Windows.Visibility.Collapsed)
                    {
                        ShowTraceResults.IsChecked = false;
                        ShowTraceResults.IsChecked = true;
                        TraceResultsPopup.UpdateLayout();
                        ((ListBoxItem)TraceResultsList.ItemContainerGenerator.ContainerFromItem(TraceResultsList.SelectedItem)).Focus();
                    }
                }
            }
        }

        private void TraceResultsList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            /*
            //Take away visiblity for any that were unselected.
            IList removedItems = e.RemovedItems;
            if (removedItems.Count > 0)
            {
                if (removedItems[0] is TraceItem)
                {
                    if (((TraceItem)removedItems[0]).ChildrenVisibleCurrently == System.Windows.Visibility.Visible)
                    {
                        ((TraceItem)removedItems[0]).ChildrenVisibleCurrently = System.Windows.Visibility.Collapsed;
                        ShowTraceResults.IsChecked = false;
                        ShowTraceResults.IsChecked = true;
                        TraceResultsPopup.UpdateLayout();
                        ((ListBoxItem)TraceResultsList.ItemContainerGenerator.ContainerFromItem(TraceResultsList.SelectedItem)).Focus();
                    }
                }
            }
            */

            IList selectedItems = e.AddedItems;
            if (selectedItems.Count > 0)
            {
                if (selectedItems[0] is TraceItem)
                {
                    TracingHelper.GetTraceItemInformation(((TraceItem)selectedItems[0]), PGETracingControl.tracingHelper.CurrentResultsAreSchematics);
                    //if (!traceResultListClicked) { ((TraceItem)selectedItems[0]).ChildrenVisibleCurrently = System.Windows.Visibility.Visible; }
                }
            }
        }

        //INC000003956225
        private void ReadEscapeToPanProperties(XElement element)
        {
            escToPanProperties.Clear();
            if (element.HasElements)
            {
                foreach (XElement childElement in element.Elements())
                {
                    if (childElement.Name.ToString() == "Tool")
                    {
                        escToPanProperties.Add(childElement.Attribute("Name").Value, childElement.Attribute("Active").Value);
                    }
                }
            }
        }

        //INC000003956225
        private bool IfEscapeToPanActive()
        {
            List<bool?> flagList = new List<bool?>() { Tools.ZoomOutRectButton.IsChecked, Tools.ZoomInRectButton.IsChecked, IdentifyToggle.IsChecked,
            Editor.IsActive, PGETracingControl.IsPGEUpstreamActive, PGETracingControl.IsPGEDownstreamActive, PGETracingControl.IsPGEProtectiveDownstreamActive,
            PGETracingControl.IsPGEProtectiveUpstreamActive, PGETracingControl.IsPGEUndergroundUpstreamActive,
            PGETracingControl.IsPGEUndergroundDownstreamActive, ElectricTrace.IsActive,  MeasureTool.IsActive, 
            CoordinatesTool.IsActive};

            int index = flagList.IndexOf(true);

            if (index >= 0 && escToPanProperties.ElementAt(index).Value == "true")
                return true;
            else
                return false;
        }

        /***********PLC Changes****************/
        private string getSupportStrLayerUrl(string layerName, string layerID)
        {

            string url = "";
            if (Application.Current.Host.InitParams.ContainsKey("Config"))
            {
                string config = Application.Current.Host.InitParams["Config"];
                //var attribute = "";
                config = System.Windows.Browser.HttpUtility.HtmlDecode(config).Replace("***", ",");
                XElement elements = XElement.Parse(config);
                foreach (XElement element in elements.Elements())
                {
                    if (element.Name.LocalName == "PLC")
                    {
                        if (element.HasElements)
                        {
                            foreach (XElement layerElement in element.Elements())
                            {

                                if (layerElement.Name == layerID)
                                {

                                    url = layerElement.FirstAttribute.Value + "/" + layerElement.LastAttribute.Value;

                                }

                            }
                        }
                    }
                }

            }

            return url;
        }


        private void zoomToLocationFromSAP()
        {

            if ((sapEuipIdQueryString != null && sapEuipIdQueryString != "") || (outageIdQueryString != "" && outageIdQueryString != null))
            {
                ESRI.ArcGIS.Client.Tasks.Query _query = new ESRI.ArcGIS.Client.Tasks.Query();
                _query.SpatialRelationship = SpatialRelationship.esriSpatialRelIntersects;
                _query.ReturnGeometry = true;
                //_query.Geometry = GetPolygonFromEnvelope(_map.Extent);
                _query.OutFields.AddRange(new string[] { "*" });
                string url = string.Empty;
                if (sapEuipIdQueryString != null && sapEuipIdQueryString != "")
                {
                    url = getSupportStrLayerUrl("Electric Distribution", "SupportStructureService");
                    _query.Where = "SAPEQUIPID='" + sapEuipIdQueryString + "'";

                }
                else if (outageIdQueryString != null && outageIdQueryString != "")
                {
                    // url = "http://edgisappqa01:6080/arcgis/rest/services/Data/RealTime/MapServer/5";
                    url = getSupportStrLayerUrl("RealTime", "OutageAreasService");
                    _query.Where = "OUTAGE_NO='" + outageIdQueryString + "'";
                }
                QueryTask _sapEquipIDqueryTask = new QueryTask(url);
                _sapEquipIDqueryTask.ExecuteCompleted += new EventHandler<QueryEventArgs>(_polequeryTask_ExecuteCompleted);
                _sapEquipIDqueryTask.Failed += new EventHandler<ESRI.ArcGIS.Client.Tasks.TaskFailedEventArgs>(_sapEquipIDqueryTask_Failed);
                _sapEquipIDqueryTask.ExecuteAsync(_query);
            }
            else if (sapLatQueryString != null && sapLatQueryString != "" && sapLongQueryString != "" && sapLongQueryString != null)
            {

                var point = new MapPoint(Convert.ToDouble(sapLongQueryString), Convert.ToDouble(sapLatQueryString), new SpatialReference(4326));

                var graphic = new Graphic
                {
                    Geometry = point
                };

                var graphicList = new List<Graphic> { graphic };

                var geometryService = new GeometryService(MeasureTool.GeometryService);
                geometryService.Failed += new EventHandler<ESRI.ArcGIS.Client.Tasks.TaskFailedEventArgs>(_sapEquipIDqueryTask_Failed);
                geometryService.ProjectCompleted += GeometryServiceNavigateProjectCompleted;
                geometryService.ProjectAsync(graphicList, MapControl.SpatialReference, point);

                MapPoint myMapPoint = new MapPoint();
                myMapPoint.X = Convert.ToDouble(sapLatQueryString);
                myMapPoint.Y = Convert.ToDouble(sapLongQueryString);
            }

            //INC000005346851
            else if (XQueryString != null && XQueryString != "" && YQueryString != "" && YQueryString != null && _isOpenedFrmEDERWEBR)
            {
                var point = new MapPoint(Convert.ToDouble(XQueryString), Convert.ToDouble(YQueryString), MapControl.SpatialReference);
                ZoomToPoint(point, _scaleOpenedFrmEDERWEBR);
            }

        }



        private void GeometryServiceNavigateProjectCompleted(object sender, GraphicsEventArgs e)
        {
            try
            {
                var point = (MapPoint)e.Results[0].Geometry;

                //INC000004230808 - Open WEBR from EDER (or ArcFM)
                if (_isOpenedFrmEDERWEBR)
                {
                    ZoomToPoint(point, _scaleOpenedFrmEDERWEBR);
                }
                else
                {
                    Envelope envelope = new Envelope();

                    envelope.XMax = point.Extent.XMax + 200;
                    envelope.XMin = point.Extent.XMin - 200;
                    envelope.YMax = point.Extent.YMax + 200;
                    envelope.YMin = point.Extent.YMin - 200;

                    MapControl.ZoomTo(envelope);
                    System.Windows.Media.SolidColorBrush colorBrush = new System.Windows.Media.SolidColorBrush();
                    colorBrush.Color = System.Windows.Media.Color.FromArgb(255, 255, 0, 0);
                    SimpleMarkerSymbol simpleMarkerSymbol = new SimpleMarkerSymbol();
                    simpleMarkerSymbol.Color = colorBrush;
                    simpleMarkerSymbol.Size = 12;
                    simpleMarkerSymbol.Style = SimpleMarkerSymbol.SimpleMarkerStyle.Circle;
                    GraphicsLayer graphicLayer = null;
                    graphicLayer = MapControl.Layers["QueryStringZoomGraphicLayer"] as GraphicsLayer;
                    if (graphicLayer == null)
                    {
                        graphicLayer = new GraphicsLayer { ID = "QueryStringZoomGraphicLayer" };
                        MapControl.Layers.Add(graphicLayer);
                    }
                    else
                    {
                        graphicLayer.Graphics.Clear();
                    }
                    Graphic poleGraphic = new Graphic()
                    {
                        Geometry = point,
                        Symbol = simpleMarkerSymbol
                    };

                    graphicLayer.Graphics.Add(poleGraphic);
                    //MapControl.Layers.Add(graphicLayer);
                }

            }
            catch (Exception ex)
            {
                throw new Exception("CoordinatesControl: An error occurred. " + ex.Message);
            }
        }

        private void ZoomToPoint(ESRI.ArcGIS.Client.Geometry.Geometry point, double scale)
        {
            double zoomResolution = (scale) / 96;
            // Calculate the bounding extents of the zoom centered on the users point click on the Map.
            double xMin = point.Extent.XMin - (MapControl.ActualWidth * zoomResolution * 0.5);
            double yMin = point.Extent.YMin - (MapControl.ActualHeight * zoomResolution * 0.5);
            double xMax = point.Extent.XMin + (MapControl.ActualWidth * zoomResolution * 0.5);
            double yMax = point.Extent.YMin + (MapControl.ActualHeight * zoomResolution * 0.5);

            // Construct an Envelope from the bounding extents.
            ESRI.ArcGIS.Client.Geometry.Envelope zoomEnvelope = new ESRI.ArcGIS.Client.Geometry.Envelope(xMin, yMin, xMax, yMax);
            zoomEnvelope.SpatialReference = MapControl.SpatialReference;

            MapControl.ZoomTo(zoomEnvelope);
        }

        void _sapEquipIDqueryTask_Failed(object sender, ESRI.ArcGIS.Client.Tasks.TaskFailedEventArgs e)
        {
            //throw new NotImplementedException();
        }

        void _polequeryTask_ExecuteCompleted(object sender, QueryEventArgs e)
        {
            if (e.FeatureSet.Features.Count > 0)
            {
                //if(e.FeatureSet.Features[0].Geometry.
                Envelope envelope = new Envelope();

                envelope.XMax = e.FeatureSet.Features[0].Geometry.Extent.XMax + 200;
                envelope.XMin = e.FeatureSet.Features[0].Geometry.Extent.XMin - 200;
                envelope.YMax = e.FeatureSet.Features[0].Geometry.Extent.YMax + 200;
                envelope.YMin = e.FeatureSet.Features[0].Geometry.Extent.YMin - 200;

                MapControl.ZoomTo(envelope);

                GraphicsLayer graphicLayer = null;
                graphicLayer = MapControl.Layers["QueryStringZoomGraphicLayer"] as GraphicsLayer;
                if (graphicLayer == null)
                {
                    graphicLayer = new GraphicsLayer { ID = "QueryStringZoomGraphicLayer" };
                    MapControl.Layers.Add(graphicLayer);
                }
                else
                {
                    graphicLayer.Graphics.Clear();
                }
                Graphic poleGraphic = null;
                if (e.FeatureSet.Features[0].Geometry is ESRI.ArcGIS.Client.Geometry.MapPoint)
                {
                    System.Windows.Media.SolidColorBrush colorBrush = new System.Windows.Media.SolidColorBrush();
                    colorBrush.Color = System.Windows.Media.Color.FromArgb(255, 255, 0, 0);
                    SimpleMarkerSymbol simpleMarkerSymbol = new SimpleMarkerSymbol();
                    simpleMarkerSymbol.Color = colorBrush;
                    simpleMarkerSymbol.Size = 12;
                    simpleMarkerSymbol.Style = SimpleMarkerSymbol.SimpleMarkerStyle.Circle;

                    poleGraphic = new Graphic()
                    {
                        Geometry = e.FeatureSet.Features[0].Geometry,
                        Symbol = simpleMarkerSymbol
                    };
                }
                else if (e.FeatureSet.Features[0].Geometry is ESRI.ArcGIS.Client.Geometry.Polygon)
                {
                    SimpleFillSymbol FillSymbol = new SimpleFillSymbol
                    {
                        Fill = new SolidColorBrush(Color.FromArgb(0x22, 255, 255, 255)),
                        BorderBrush = new SolidColorBrush(Color.FromArgb(255, 255, 0, 0)),
                        BorderThickness = 2
                    };

                    poleGraphic = new Graphic()
                    {
                        Geometry = e.FeatureSet.Features[0].Geometry,
                        Symbol = FillSymbol
                    };

                    if (MapControl.Layers["RealTime"].Visible == false)
                    {
                        MapControl.Layers["RealTime"].Visible = true;
                    }
                }
                graphicLayer.Graphics.Add(poleGraphic);



            }
            else
            {
                if (sapEuipIdQueryString != null && sapEuipIdQueryString != "")
                {

                    MessageBox.Show("No Pole found for SAPEQUIPID: " + sapEuipIdQueryString);
                }
                else if (outageIdQueryString != null && outageIdQueryString != "")
                {
                    MessageBox.Show("No Pole found for OUTAGEID: " + outageIdQueryString);
                }

            }
        }

        /***********PLC Changes Ends****************/

        //CIT Config Values for FilledDuct
        private void ReadCITFilledValues(XElement element)
        {
            try
            {
                FilledDuctValue = element.FirstAttribute.Value;
            }
            catch (Exception ex)
            {
            }
        }


        //INC000004403856
        private void ReadCMCSProperties(XElement element)
        {
            if (element.HasElements)
            {
                _cmcsCircuitIDLayerIds = element.Element("CircuitIDLayers").Attribute("LayerId").Value;
                _cmcsCircuitIDLayersService = element.Element("CircuitIDLayers").Attribute("url").Value;
                _cmcsCircuitSourceUrl = element.Element("CircuitSourceTable").Attribute("url").Value + "/" + element.Element("CircuitSourceTable").Attribute

("LayerId").Value;
                _cmcsMapNumberUrl = element.Element("MapNumer").Attribute("Url").Value;
                if (element.Element("MapNumer").HasElements == true)
                {
                    foreach (XElement mapNoElement in element.Element("MapNumer").Elements())
                    {
                        _cmcsMapNo_LayerId.Add(Convert.ToInt16(mapNoElement.Attribute("LayerId").Value));
                        _cmcsMapNo_LayerName.Add(mapNoElement.Attribute("LayerName").Value);
                        _cmcsMapNo_LayerField.Add(mapNoElement.Attribute("Field").Value);
                    }
                }
                if (element.Element("MapDimensions") != null)
                    _cmcsMapDimensions = element.Element("MapDimensions");
            }
        }

        //INC000004378902
        private void ReadSearchSingleZoomLevelProp(XElement element)
        {
            if (element.HasElements)
            {
                foreach (XElement childElement in element.Elements())
                {
                    if (childElement.Name.ToString() == "StoredDisplay")
                    {
                        AttributesViewer.searchSingleZoomLevelProp.Add(childElement.Attribute("Name").Value, childElement.Attribute("Scale").Value);
                    }
                }
            }
        }

        //INC000005346851 - starts
        private void ReadShareCurrentUrlProp(XElement element)
        {
            Tools.ShareCurrentUrlSPIndex.Clear();
            if (element.HasElements)
            {
                foreach (XElement childElement in element.Elements())
                {
                    if (childElement.Name.ToString() == "StoredDisplay")
                    {
                        Tools.ShareCurrentUrlSPIndex.Add(childElement.Attribute("Index").Value, childElement.Attribute("Name").Value);
                    }
                }
            }
            if (HtmlPage.Document.QueryString.ContainsKey("SP") && Tools.ShareCurrentUrlSPIndex.ContainsKey(HtmlPage.Document.QueryString["SP"]))
            {
                _defaultStoredView = Tools.ShareCurrentUrlSPIndex[HtmlPage.Document.QueryString["SP"]];
            }
        }
        //INC000005346851 - ends

        //DA# 190506 - WEBR Access Management---START
        private void StoreUserDetails()
        {
            try
            {
                string wcfServiceURL = LoadConfiguration_LoginDataWCF();
                if (wcfServiceURL != null)
                {
                    string machineName = String.Empty;
                    if (Application.Current.Host.InitParams.ContainsKey("CurrentUserMachineName"))
                        machineName = Convert.ToString(Application.Current.Host.InitParams["CurrentUserMachineName"]);
                    string LANID = Convert.ToString(WebContext.Current.User.Name.Replace("PGE\\", "")).ToUpper();

                    Ponsservicelayercall objPonsServiceCall = new Ponsservicelayercall();
                    string prefixUrl = objPonsServiceCall.GetPrefixUrl();
                    string endpointaddress = prefixUrl + wcfServiceURL;
                    EndpointAddress endPoint = new EndpointAddress(endpointaddress);
                    BasicHttpBinding httpbinding = new BasicHttpBinding();
                    httpbinding.MaxBufferSize = 2147483647;
                    httpbinding.MaxReceivedMessageSize = 2147483647;
                    httpbinding.TransferMode = TransferMode.Buffered;
                    httpbinding.SendTimeout = TimeSpan.FromSeconds(300);
                    IServicePons cusservice = new ChannelFactory<IServicePons>(httpbinding, endPoint).CreateChannel();
                    try
                    {
                        cusservice.BeginStoreUserDetails(LANID, machineName,
                         delegate(IAsyncResult result)
                         {
                             bool isSuccessful = ((IServicePons)result.AsyncState).EndStoreUserDetails(result);
                             this.Dispatcher.BeginInvoke(
                             delegate()
                             {
                                 if (!isSuccessful)
                                 {
                                     logger.Info("Error in storing login user details in database.");
                                     ConfigUtility.UpdateStatusBarText("Error in storing User details.");
                                 }
                             }
                             );
                         }, cusservice);
                    }
                    catch (Exception ex)
                    {
                        logger.Info("Error in storing login user details in database:" + ex.Message.ToString());
                        ConfigUtility.UpdateStatusBarText("Error in storing User details.");
                    }
                }
                else
                {
                    logger.Info("Error in getting WCF service URL for storing login user details.");
                    ConfigUtility.UpdateStatusBarText("Error in storing User details.");
                }
            }
            catch (Exception ex)
            {
                logger.Info("Error in storing login user details:" + ex.Message.ToString());
                ConfigUtility.UpdateStatusBarText("Error in storing User details.");
            }
        }

        private string LoadConfiguration_LoginDataWCF()
        {
            try
            {
                if (Application.Current.Host.InitParams.ContainsKey("Config"))
                {
                    string config = Application.Current.Host.InitParams["Config"];
                    var attribute = "";
                    config = HttpUtility.HtmlDecode(config).Replace("***", ",");
                    XElement elements = XElement.Parse(config);
                    foreach (XElement element in elements.Elements())
                    {
                        if (element.Name.LocalName == "PONSInformation")
                        {
                            if (element.HasElements)
                            {
                                foreach (XElement childelement in element.Elements())
                                {

                                    if (childelement.Name.LocalName == "PONSService")
                                    {
                                        attribute = childelement.Attribute("WCFService_URL").Value;
                                        if (attribute != null)
                                        {
                                            return attribute;
                                        }
                                    }

                                }

                            }
                        }
                    }
                }

            }
            catch
            {
            }
            return null;
        }
        //DA# 190506 - WEBR Access Management---END

        private void LIDARCorrectionsProperties(XElement element)
        {
            LIDARCorStoredDisplayName = element.Attribute("StoredDisplayName").Value;
            LIDARCorLayerId = Convert.ToInt32(element.Attribute("LayerId").Value);
        }

        //DA #190905 - ME Q1 2020 --- START
        private void LIDARCorVisiblilityChanged(object sender, IsVisibleEventArgs e)
        {
            if (ToggleLIDARCorrCheckedUnchecked == false) // to check if visibility has been changed form TOC only and not from WIP tab
            {
                ToggleLIDARCorrectionLayer.Checked -= LIDARCorVisibleToggleBtn_CheckedUnchecked;
                ToggleLIDARCorrectionLayer.Unchecked -= LIDARCorVisibleToggleBtn_CheckedUnchecked;
                if (e.IsVisible == true)
                    ToggleLIDARCorrectionLayer.IsChecked = true;
                else
                    ToggleLIDARCorrectionLayer.IsChecked = false;

                ToggleLIDARCorrectionLayer.Checked += LIDARCorVisibleToggleBtn_CheckedUnchecked;
                ToggleLIDARCorrectionLayer.Unchecked += LIDARCorVisibleToggleBtn_CheckedUnchecked;
            }
        }

        public void LIDARCorVisibleToggleBtn_CheckedUnchecked(object sender, RoutedEventArgs e)
        {
            try
            {
                ToggleLIDARCorrCheckedUnchecked = true;
                if (ConfigUtility.StatusBar != null)
                    ConfigUtility.StatusBar.Text = string.Empty;
                if (MapControl != null)
                {
                    Layer storedDisplayLayer = MapControl.Layers[LIDARCorStoredDisplayName];
                    if (storedDisplayLayer != null && storedDisplayLayer is ArcGISDynamicMapServiceLayer)
                    {
                        ArcGISDynamicMapServiceLayer dynamicMapServiceLayer = (ArcGISDynamicMapServiceLayer)storedDisplayLayer;
                        if (ToggleLIDARCorrectionLayer.IsChecked == true)
                            dynamicMapServiceLayer.SetLayerVisibility(LIDARCorLayerId, true);
                        else
                            dynamicMapServiceLayer.SetLayerVisibility(LIDARCorLayerId, false);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error("LIDAR Corrections ON/OFF Exception: " + ex.Message);
            }
            finally
            {
                ToggleLIDARCorrCheckedUnchecked = false;
            }
        }

        //DA #190905 - ME Q1 2020 --- END

    }
}
