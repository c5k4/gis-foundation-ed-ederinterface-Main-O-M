using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using NLog;
using ESRI.ArcGIS.Client;
using ESRI.ArcGIS.Client.Actions;
using ESRI.ArcGIS.Client.Geometry;
using ESRI.ArcGIS.Client.Symbols;
using ESRI.ArcGIS.Client.Tasks;
using AreaUnit = ESRI.ArcGIS.Client.Actions.AreaUnit;
using PointCollection = ESRI.ArcGIS.Client.Geometry.PointCollection;
using Polygon = ESRI.ArcGIS.Client.Geometry.Polygon;
using System.Xml.Linq;
using System.Collections.ObjectModel;
using System.IO;
using System.Net;
using System.Windows.Browser;
using Miner.Server.Client.Toolkit;
using System.ServiceModel;
using System.Threading;
using System.Windows.Threading;
namespace ArcFMSilverlight
{


    public partial class StandardPrintPage : UserControl
    {
        #region declarations
        bool firstprint = false;
        bool chkfirstprint = false;
        FloatableWindow _fw = null;
        private MapTools _mapTools;
        private Map _map;
        string endpointaddress = string.Empty;
        //Zoom to Circuit--start
        private string _circuitSourceURL;
        private const string DEFINITION_CIRCUIT = "CIRCUITID='{0}'";
        private const int FONT_SIZE = 35;
        private FontFamily _fontFamily = new FontFamily("Arial");
        private Envelope _primaryUGLayerExtentGeometry = null;
        private Envelope _primaryOHLayerExtentGeometry = null;
        private bool _primaryUGReturned;
        private bool _primaryOHReturned;
        private IDictionary<string, string> _circuitIDDictionary = new Dictionary<string, string>();
        private string _primaryOhUrl = "";
        private string _primaryUgUrl = "";
        private string _circuitPointUrl = "";
        private IList<Graphic> _primaryUgGraphics;
        private IList<Graphic> _primaryOhGraphics;
        private Graphic _circuitPointGraphic = null;
        private GraphicsLayer _graphicsLayer = new GraphicsLayer();
        private GraphicsLayer _graphicsLayerPoint = new GraphicsLayer();
        private GraphicsLayer _graphicsLayerText = new GraphicsLayer();
        private bool _createNewSelection = false;
        private bool _zoomToAllSelected = false;
        private Envelope _selectedEnvelope = null;
        private string _circuitIdSelected = "";
        private Envelope _secondaryUGLayerExtentGeometry = null;
        private Envelope _secondaryOHLayerExtentGeometry = null;
        private Envelope _secondaryBusBarLayerExtentGeometry = null;
        private bool _secondaryUGReturned;
        private bool _secondaryOHReturned;
        private bool _secondaryBusBarReturned;
        private string _secondaryOhUrl = "";
        private string _secondaryUgUrl = "";
        private string _secondaryBusBarUrl = "";
        private IList<Graphic> _secondaryUgGraphics;
        private IList<Graphic> _secondaryOhGraphics;
        private IList<Graphic> _secondaryBusBarGraphics;
        private IList<Graphic> _lineGraphics = new List<Graphic>();
        private int _scaleLimit;
        //Zoom to Circuit--end

        private List<Tuple<string, string, string, Tuple<string, string, string, string, string>>> _sessionGridLayers;
        private string _scaleFieldName;
        private string _regionFieldName;
        private string _divisionFieldName;
        private string _districtFieldName;
        private string _gridnumberFieldName;
        private string _serviceAreaMapServiceUrl = "";
        private int _serviceAreaLayerId = -1;
        private IDictionary<string, object> _serviceAreaAttributes = null;
        List<IdentifyResult> _gridNumberResults = null;
        public static Logger logger = LogManager.GetCurrentClassLogger();
        private double _PONSPolyScale;
        //Map Polygon

        private GraphicsLayer _MappolygongraphicsLayer = new GraphicsLayer();
        //Polygon Selection
        //private GraphicsLayer _graphicsLayer;
        private Draw _drawControl = null;
        private Polygon _polygon;
        private ESRI.ArcGIS.Client.Symbols.SimpleFillSymbol _fillSymbol;
        public double AreaTotal { get; set; }
        private Size _textSize;
        private TextSymbol _textSymbol;
        private ESRI.ArcGIS.Client.Symbols.SimpleMarkerSymbol _markerSymbol;
        public FillSymbol FillSymbol { get; set; }
        public LineSymbol LineSymbol { get; set; }
        private List<double> SegmentLengths;
        public MeasureAction.Mode MeasureMode
        { get; set; }
        private List<PolygonSearchItem> _polygonSearchList = new List<PolygonSearchItem>();
        private const string ExportCursor = @"/Images/cursor_measure.png";
        private bool _firstTime;
        private bool _finishingSegment;
        private bool _finishingArea;
        private bool _finishMeasure;
        private bool _finishedSegment;
        private bool _isMeasuring;
        private bool _clearAll;
        private List<SegmentLayer> _layers = new List<SegmentLayer>();
        private const double MetersToMiles = 0.0006213700922;
        private const double MetersToFeet = 3.280839895;
        private const double SqMetersToSqMiles = 0.0000003861003;
        private const double SqMetersToSqFeet = 10.76391;
        private const double SqMetersToHectare = 0.0001;
        private const double SqMetersToAcre = 0.00024710538;
        private const int TotalDistanceSymbolPosition = 0;
        private const int TotalAreaSymbolPosition = 1;
        private int _graphicCount, _areaAsyncCalls, _mouseMove;
        private double _mapDistance;
        private double _distanceFactor = 0.3048;
        public string POLY_GRAPHICS_LAYER = "StandGraphics";
        private GeometryService _geometryServicePolygon;
        public bool isPolygonDrawn = false;
        private const string STAND_POLYGON_GRAPHIC_LAYER = "StandPolygonGraphics";
        private string _GeometryService_Standard;
        List<ComboBoxItem> gridNumberCBIs = new List<ComboBoxItem>();
        List<string> SelectGridlist = new List<string>();
        List<string> SelectGridScale = new List<string>();
        Ponsservicelayercall objPonsServiceCall;
        string _WCFService_URL = "";
        private int _timelimit;
          private string _printmsg;
        #endregion
        public StandardPrintPage(Map map, MapTools mapTools)
        {
            InitializeComponent();
            _map = map;
            ClearAppResource("MapTypes");
            ClearAppResource("GridLayers");
            ClearAppResource("ScaleOptions");
            ClearAppResource("PDFRootUrl");
            ClearAppResource("PDFMaps");
            ReadTemplatesConfig();
            LoadLayerConfiguration();
            LoadConfiguration();
            // _circuitSourceURL = "http://edgiswwwprd02/arcgis/rest/services/Data/ElectricDistribution/MapServer/294";
            //  _GeometryService_Standard = "http://edgiswwwprd02/arcgis/rest/services/Utilities/Geometry/GeometryServer";
            objPonsServiceCall = new Ponsservicelayercall();
            string prefixUrl = objPonsServiceCall.GetPrefixUrl();
            endpointaddress = prefixUrl + _WCFService_URL;
            comboTemplateSelection.SelectionChanged += new SelectionChangedEventHandler(comboTemplateSelection_SelectionChanged);
            PopulateTemplateSelectionComboBox();

            comboGridLayerSelection.SelectionChanged += new SelectionChangedEventHandler(comboGridLayerSelection_SelectionChanged);
            PopulateGridLayerSelectionComboBox();

            comboGridNumberSelection.SelectionChanged += new SelectionChangedEventHandler(comboGridNumberSelection_SelectionChanged);

            InitializeDrawPolygon();
            InitializeZoomToCircuit();
        }

        private void InitializeDrawPolygon()
        {
            int fontSize = 25;
            FontFamily fontFamily = new System.Windows.Media.FontFamily("Arial");
            _textSize = GetTextSize(fontFamily, fontSize);
            _textSymbol = new TextSymbol()
            {
                FontFamily = new System.Windows.Media.FontFamily("Arial"),
                Foreground = new System.Windows.Media.SolidColorBrush(Colors.Cyan),
                FontSize = 25,
                //Text = text,
                OffsetX = _textSize.Width / 2,
                OffsetY = _textSize.Height / 2
            };
            LineSymbol = new ESRI.ArcGIS.Client.Symbols.SimpleLineSymbol
            {
                Color = new SolidColorBrush(Color.FromArgb(255, 255, 0, 0)),
                Width = 2,
                Style = ESRI.ArcGIS.Client.Symbols.SimpleLineSymbol.LineStyle.Solid
            };
            FillSymbol = new ESRI.ArcGIS.Client.Symbols.SimpleFillSymbol
            {
                Fill = new SolidColorBrush(Color.FromArgb(0x22, 255, 255, 255)),
                BorderBrush = new SolidColorBrush(Color.FromArgb(255, 255, 0, 0)),
                BorderThickness = 2
            };
            _markerSymbol = new ESRI.ArcGIS.Client.Symbols.SimpleMarkerSymbol
            {
                Color = new SolidColorBrush(Color.FromArgb(0x66, 255, 0, 0)),
                Size = 5,
                Style = ESRI.ArcGIS.Client.Symbols.SimpleMarkerSymbol.SimpleMarkerStyle.Circle
            };
            SegmentLengths = new List<double>();
            MeasureMode = MeasureAction.Mode.Polygon;
            _geometryServicePolygon = new GeometryService(_GeometryService_Standard);
        }
        #region Config
        //Page config
        private void LoadLayerConfiguration()
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
                                            _WCFService_URL = attribute;
                                        }
                                    }



                                    if (childelement.Name.LocalName == "CircuitService")
                                    {
                                        attribute = childelement.Attribute("url").Value;
                                        if (attribute != null)
                                        {
                                            // _CircuitService = attribute;
                                        }
                                    }



                                }

                            }
                            // Print



                        }
                    }
                }

            }
            catch
            {
            }
        }
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
                    // LogHelper.InitializeNLog(element);
                    //LoadConfiguration(element);
                }

                if (Application.Current.Host.InitParams.ContainsKey("Config"))
                {
                    string config = Application.Current.Host.InitParams["Config"];
                    var attribute = "";
                    config = HttpUtility.HtmlDecode(config).Replace("***", ",");
                    XElement elements = XElement.Parse(config);
                    foreach (XElement element in elements.Elements())
                    {
                        if (element.Name.LocalName == "StandardMapPrint")
                        {
                            if (element.HasElements)
                            {
                                foreach (XElement childelement in element.Elements())
                                {

                                    if (childelement.Name.LocalName == "primaryOH")
                                    {
                                        attribute = childelement.Attribute("url").Value;
                                        if (attribute != null)
                                        {
                                            _primaryOhUrl = attribute;
                                        }
                                    }

                                    if (childelement.Name.LocalName == "primaryUG")
                                    {
                                        attribute = childelement.Attribute("url").Value;
                                        if (attribute != null)
                                        {
                                            _primaryUgUrl = attribute;
                                        }
                                    }
                                    if (childelement.Name.LocalName == "secondaryOH")
                                    {
                                        attribute = childelement.Attribute("url").Value;
                                        if (attribute != null)
                                        {
                                            _secondaryOhUrl = attribute;
                                        }
                                    }
                                    if (childelement.Name.LocalName == "secondaryUG")
                                    {
                                        attribute = childelement.Attribute("url").Value;
                                        if (attribute != null)
                                        {
                                            _secondaryUgUrl = attribute;
                                        }
                                    }

                                    if (childelement.Name.LocalName == "secondaryBusBar")
                                    {
                                        attribute = childelement.Attribute("url").Value;
                                        if (attribute != null)
                                        {
                                            _secondaryBusBarUrl = attribute;
                                        }
                                    }

                                    if (childelement.Name.LocalName == "CircuitService")
                                    {
                                        attribute = childelement.Attribute("url").Value;
                                        if (attribute != null)
                                        {
                                            _circuitSourceURL = attribute;
                                        }
                                    }
                                    if (childelement.Name.LocalName == "circuitPoint")
                                    {
                                        attribute = childelement.Attribute("url").Value;
                                        if (attribute != null)
                                        {
                                            _circuitPointUrl = attribute;
                                        }
                                    }

                                    if (childelement.Name.LocalName == "GeometryService_Standard")
                                    {
                                        attribute = childelement.Attribute("url").Value;
                                        if (attribute != null)
                                        {
                                            _GeometryService_Standard = attribute;
                                        }
                                    }
                                    if (childelement.Name.LocalName == "MapScale")
                                    {
                                        attribute = childelement.Attribute("id").Value;
                                        if (attribute != null)
                                        {
                                            _scaleLimit = Convert.ToInt32(attribute);
                                        }
                                    }
                                    if (childelement.Name.LocalName == "Printtimer")
                                    {
                                        attribute = childelement.Attribute("id").Value;
                                        if (attribute != null)
                                        {
                                            _timelimit = Convert.ToInt32(attribute);
                                        }
                                    }
                                    if (childelement.Name.LocalName == "Printmsg")
                                    {
                                        attribute = childelement.Attribute("id").Value;
                                        if (attribute != null)
                                        {
                                            _printmsg = attribute;
                                        }
                                    }
                                }

                            }

                        }
                    }
                }
            }
            catch (Exception e)
            {
                logger.FatalException("Error loading configuration: ", e);
            }

            logger.Info("Loading configuration complete");
            string currentUser = WebContext.Current.User.Name;
        }
        //Config
        private void ReadTemplatesConfig()
        {
            try
            {
                if (Application.Current.Host.InitParams.ContainsKey("TemplatesConfig"))
                {
                    string config = Application.Current.Host.InitParams["TemplatesConfig"];
                    config = HttpUtility.HtmlDecode(config).Replace("***", ",");
                    XElement xe = XElement.Parse(config);

                    if (!xe.HasElements) return;

                    List<Tuple<string, string, string, Tuple<string, string, string, string, string>>> gridLayerOptions = new List<Tuple<string, string, string, Tuple<string, string, string, string, string>>>();

                    XElement _gle = xe.Element("GridLayers");
                    if (_gle != null)
                    {
                        XAttribute _xatt = _gle.Attribute("ServiceAreaMapService");
                        if (_xatt != null)
                        {
                            _serviceAreaMapServiceUrl = _xatt.Value;
                        }
                        else
                        {
                            _serviceAreaMapServiceUrl = "";
                        }

                        _xatt = _gle.Attribute("ServiceAreaLayerId");

                        if (_xatt != null)
                        {
                            _serviceAreaLayerId = Convert.ToInt32(_xatt.Value.ToString());
                        }
                        else
                        {
                            _serviceAreaLayerId = -1;
                        }
                    }

                    var GridLayerNodes = from gln in xe.Descendants("GridLayers")
                                         select gln;

                    foreach (XElement GridLayerNode in GridLayerNodes.Elements())
                    {
                        string name = GridLayerNode.Attribute("Name").Value;
                        string service = GridLayerNode.Attribute("MapService").Value;
                        string layerId = GridLayerNode.Attribute("LayerId").Value;

                        string scalefieldname = GridLayerNode.Attribute("ScaleFieldName").Value;//7
                        string regionfieldname = GridLayerNode.Attribute("RegionFieldName").Value;//8
                        string divisionfieldname = GridLayerNode.Attribute("DivisionFieldName").Value;//9
                        string districtfieldname = GridLayerNode.Attribute("DistrictFieldName").Value;//10
                        string gridnumberfieldname = GridLayerNode.Attribute("GridNumberFieldName").Value;//11

                        if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(service))
                        {
                            MessageBox.Show("Configuration error: Grid Layer Name and MapService cannot be null.");
                        }
                        else
                        {
                            Tuple<string, string, string, string, string> gridlayerFields = new Tuple<string, string, string, string, string>(scalefieldname, regionfieldname, divisionfieldname, districtfieldname, gridnumberfieldname);
                            Tuple<string, string, string, Tuple<string, string, string, string, string>> GridLayerItem = new Tuple<string, string, string, Tuple<string, string, string, string, string>>(name, service, layerId, gridlayerFields);

                            gridLayerOptions.Add(GridLayerItem);
                        }
                    }

                    Application.Current.Resources.Add("GridLayers", gridLayerOptions);

                    List<Tuple<string, string, string>> pdfMaps = new List<Tuple<string, string, string>>();

                    var PDFMapsNodes = from pdfm in xe.Descendants("PDFMaps")
                                       select pdfm;
                    string rooturl = xe.Element("PDFMaps").Attribute("RootUrl").Value;

                    foreach (var pdfmapNode in PDFMapsNodes.Elements())
                    {
                        string mapType = pdfmapNode.Attribute("MapType").Value;
                        string mapSize = pdfmapNode.Attribute("MapSize").Value;
                        string gridCellScaleFilters = pdfmapNode.Attribute("GridNumberScaleFilters").Value;
                        Tuple<string, string, string> pdfMap = new Tuple<string, string, string>(mapType, mapSize, gridCellScaleFilters);
                        pdfMaps.Add(pdfMap);
                    }

                    if (pdfMaps.Count > 0) pdfMaps.Sort();
                    Application.Current.Resources.Add("PDFRootUrl", rooturl);
                    Application.Current.Resources.Add("PDFMaps", pdfMaps);
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
        }
        #endregion
        #region Bind combox

        void comboTemplateSelection_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (comboTemplateSelection.SelectedIndex < 0) return;

            //comboPageSize.Items.Clear();
            comboPageSize.ItemsSource = null;

            List<Tuple<string, string, string>> pdfMaps = (List<Tuple<string, string, string>>)Application.Current.Resources["PDFMaps"];
            var selectedTemplateItem = comboTemplateSelection.SelectedValue as ComboBoxItem;

            var templateList = from t in pdfMaps
                               where t.Item1.ToString() == selectedTemplateItem.Content.ToString()
                               select t;


            if ((templateList == null) || (templateList.Count() <= 0))
            {
                ObservableCollection<string> sizesListObs = new ObservableCollection<string>();
                sizesListObs.Add("24x36");
                comboPageSize.ItemsSource = sizesListObs;
            }
            else
            {
                var selectedTemplate = templateList.FirstOrDefault();

                var sizeList = selectedTemplate.Item2;
                List<string> sizesList = new List<string>();
                foreach (string size in sizeList.Split(new char[] { ',', ';' }))
                {
                    sizesList.Add(size);
                }
                sizesList.Sort();
                ObservableCollection<string> sizesListObs = new ObservableCollection<string>(sizesList);
                comboPageSize.ItemsSource = sizesListObs;
            }

            comboPageSize.SelectedIndex = 0;

            // Filter Grid Numbers
            if (_gridNumberResults != null)
            {
                FilterGridNumbers();
            }
        }
        private void FilterGridNumbers()
        {
            //comboGridNumberSelection.Items.Clear();
            comboGridNumberSelection.ItemsSource = null;

            gridNumberCBIs.Clear();
           
            SelectGridlist.Clear();

            foreach (IdentifyResult gridNumberResult in _gridNumberResults)
            {
                var attributes = gridNumberResult.Feature.Attributes;

                string resultScaleFilter = attributes[_scaleFieldName].ToString();//SCALE
                ComboBoxItem cbi = comboTemplateSelection.SelectedItem as ComboBoxItem;
                string cbiScales = cbi.Tag.ToString();
                string[] cbiScalesArray = cbiScales.Split(',');

                if (cbiScalesArray.Contains(resultScaleFilter.ToString()))
                {
                    var resultValue = attributes[_gridnumberFieldName];//Plat Number
                    var resultScaleValue = attributes[_scaleFieldName];//Plat Number
                    if (resultValue != null)
                    {

                        SelectGridlist.Add(resultValue.ToString() + "#" + resultScaleValue.ToString());
                        SelectGridScale.Add(resultScaleValue.ToString());
                        gridNumberCBIs.Add(new ComboBoxItem { Content = resultValue, Tag = gridNumberResult.Feature });
                    }
                }
            }

            if (gridNumberCBIs.Count > 0)
            {
                gridNumberCBIs.Add(new ComboBoxItem { Content = "ALL", Tag = "0" });
               
                //var gd = SelectGridlist.ToList();
                // var Gr_Scale = SelectGridScale.ToList();
                gridNumberCBIs.Sort((x, y) => string.Compare(x.Tag.ToString(), y.Tag.ToString()));


                comboGridNumberSelection.ItemsSource = gridNumberCBIs;
               
                comboGridNumberSelection.SelectedIndex = 0;
                var gd = gridNumberCBIs.ToList();
                EnableDisablePrintButton();
            }
            else
            {
               // MessageBox.Show("No grid present in this layer.");
                PrintStandardMapButton.IsEnabled = false;
            }
        }

        void comboGridNumberSelection_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                var selectedGridNumber = comboGridNumberSelection.SelectedValue as ComboBoxItem;
                if (selectedGridNumber == null) return;
                if (comboGridNumberSelection.SelectedIndex == 0)
                {
                    var selectedGridNumberAll = comboGridNumberSelection.Items[1] as ComboBoxItem;
                    //   var gridData = (selectedGridNumber.Tag as Graphic).Attributes;
                    //Call to Identify on Service Area Layer
                    GetOverlappingServiceAreaInfo(selectedGridNumberAll.Tag as Graphic);
                }
                else
                {

                    var gridData = (selectedGridNumber.Tag as Graphic).Attributes;
                    if (gridData == null) return;

                    //Call to Identify on Service Area Layer
                    GetOverlappingServiceAreaInfo(selectedGridNumber.Tag as Graphic);
                }

            }
            catch { }

            // EnableDisablePrintButton();
        }
        private void PopulateTemplateSelectionComboBox()
        {
            comboTemplateSelection.Items.Clear();

            List<Tuple<string, string, string>> pdfMaps = (List<Tuple<string, string, string>>)Application.Current.Resources["PDFMaps"];

            if (pdfMaps != null && pdfMaps.Count > 0)
            {
                foreach (var pdfMap in pdfMaps)
                {
                    //comboTemplateSelection.Items.Add(new ComboBoxItem { Content = pdfMap.Item1, Tag = pdfMap.Item2 });
                    comboTemplateSelection.Items.Add(new ComboBoxItem { Content = pdfMap.Item1, Tag = pdfMap.Item3 });
                }

                if (comboTemplateSelection.Items.Count > 0)
                {
                    comboTemplateSelection.SelectedIndex = 0;
                }
            }
        }

        private void PopulateGridLayerSelectionComboBox()
        {
            comboGridLayerSelection.Items.Clear();
            comboGridNumberSelection.ItemsSource = null;
            _sessionGridLayers = (List<Tuple<string, string, string, Tuple<string, string, string, string, string>>>)Application.Current.Resources["GridLayers"];

            if (_sessionGridLayers != null && _sessionGridLayers.Count > 0)
            {
                List<ComboBoxItem> gridLayerCBIs = new List<ComboBoxItem>();
                for (int i = 0; i < _sessionGridLayers.Count(); i++)
                {
                    gridLayerCBIs.Add(new ComboBoxItem { Content = _sessionGridLayers[i].Item1, Tag = _sessionGridLayers[i] });
                }

                if (gridLayerCBIs.Count > 0)
                {
                    //                    gridLayerCBIs.Sort((x, y) => string.Compare(x.Content.ToString(), y.Content.ToString()));
                    comboGridLayerSelection.ItemsSource = gridLayerCBIs;
                    comboGridLayerSelection.SelectedIndex = 0;
                }
            }
        }
        #endregion

        #region  Circuit combo box

        private void PART_AutoCompleteTextBlock_OnPopulating(object sender, PopulatingEventArgs e)
        {
            var queryTask =
                new QueryTask(_circuitSourceURL);
            queryTask.ExecuteCompleted += new EventHandler<QueryEventArgs>(Cicuit_QueryTask_ExecuteCompleted);
            queryTask.Failed += new EventHandler<TaskFailedEventArgs>(Circuit_QueryTask_Failed);
            Query query = new Query();
            string userInput = e.Parameter.TrimStart().ToUpper();
            //TODO: Fix BUG 24559. Input could be MANCHESTER 1101 or SAN CARLOS 1101, needs to search after SAN CARLOS
            string substationLike = userInput.Contains(" ") ? userInput.Substring(0, userInput.LastIndexOf(" ")) : userInput;
            query.Where = "SUBSTATIONNAME like '" + substationLike + "%' and LENGTH(circuitid) = 9 ";
            if (SearchStringEndsWithCircuitID(userInput))
            {
                string circuitIDLike = userInput.Substring(userInput.LastIndexOf(" ") + 1);
                query.Where += " and substr(CIRCUITID, -4, 4) like '" + circuitIDLike + "%'";
            }

            query.OutFields.Add("CIRCUITID");
            query.OutFields.Add("SUBSTATIONNAME");
            queryTask.ExecuteAsync(query);
        }

        private bool SearchStringEndsWithCircuitID(string searchString)
        {
            int n;
            return (searchString.Contains(" ") &&
                    int.TryParse(searchString.Substring(searchString.LastIndexOf(" ") + 1), out n));
        }

        private void Circuit_QueryTask_Failed(object sender, TaskFailedEventArgs e)
        {
            // Log it??
        }

        private void Cicuit_QueryTask_ExecuteCompleted(object sender, QueryEventArgs e)
        {
            //Weed out duplicates e.g. MILLBRAE 0401 either with distinct or groupby/first
            List<string> circuitIdList = e.FeatureSet.Features.
                Select(f => Convert.ToString(f.Attributes["SUBSTATIONNAME"]) + " " + Convert.ToString(f.Attributes["CIRCUITID"]).
                        Substring(Convert.ToString(f.Attributes["CIRCUITID"]).Length - 4)).ToList();
            circuitIdList = circuitIdList.Distinct().ToList();
            circuitIdList.Sort();
            _circuitIDDictionary = e.FeatureSet.Features.GroupBy(g => g.Attributes["SUBSTATIONNAME"] + Convert.ToString(g.Attributes["CIRCUITID"]).
                        Substring(Convert.ToString(g.Attributes["CIRCUITID"]).Length - 4)).Select(y => y.First()).
                ToDictionary(f => Convert.ToString(f.Attributes["SUBSTATIONNAME"]) + " " + Convert.ToString(f.Attributes["CIRCUITID"]).
                        Substring(Convert.ToString(f.Attributes["CIRCUITID"]).Length - 4), f => Convert.ToString(f.Attributes["CIRCUITID"]));
            PART_FilterAutoCompleteTextBlock.ItemsSource = circuitIdList;
            PART_FilterAutoCompleteTextBlock.PopulateComplete();
        }

        private void PART_AutoCompleteTextBlock_OnKeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Escape:
                    e.Handled = true;
                    PART_FilterAutoCompleteTextBlock.Visibility = Visibility.Collapsed;
                    break;
                case Key.Enter:
                    break;
                default:
                    break;
            }
        }

        private void PART_AutoCompleteTextBlock_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                string circuitId = _circuitIDDictionary[e.AddedItems[0].ToString()];
                ResetFilterGraphics();
                ZoomToCircuit(circuitId, e.AddedItems[0].ToString());
            }
        }
        #endregion
        #region ZoomToCircuits code
        private void InitializeZoomToCircuit()
        {

            _graphicsLayer.ID = "Stand_CircuitGraphics";
            _graphicsLayerPoint.ID = "Stand_CircuitPointGraphic";
            // _scaleLimit = 5000;
        }

        public void ResetFilterGraphics()
        {
            _graphicsLayer.ClearGraphics();
            _graphicsLayerPoint.ClearGraphics();
            _graphicsLayerText.ClearGraphics();
            _selectedEnvelope = null;
            _circuitIdSelected = "";
            _circuitPointGraphic = null;
            _lineGraphics.Clear();
        }

        public void ZoomToCircuit(string circuitId, string circuitName, bool resetGraphics = false)
        {
            if (_createNewSelection || resetGraphics)
            {
                ResetFilterGraphics();
            }
            ConfigUtility.UpdateStatusBarText("Zooming to Circuit [ " + circuitName + " ]...");
            _circuitIdSelected = circuitId;
            getCircuitPoint();
        }

        private void SetExtentFromDefinitionQuery(string definition)
        {
            _primaryUGLayerExtentGeometry = null;
            _primaryOHLayerExtentGeometry = null;
            _primaryOHReturned = false;
            _primaryUGReturned = false;
            _primaryOhGraphics = null;
            _primaryUgGraphics = null;
            Query query = new Query();
            query.ReturnGeometry = true;
            query.Where = definition;
            QueryTask queryTaskOh = new QueryTask(_primaryOhUrl);
            queryTaskOh.ExecuteCompleted += new EventHandler<QueryEventArgs>(queryTaskOh_ExecuteCompleted);
            queryTaskOh.ExecuteAsync(query);
            QueryTask queryTaskUg = new QueryTask(_primaryUgUrl);
            queryTaskUg.ExecuteCompleted += new EventHandler<QueryEventArgs>(queryTaskUg_ExecuteCompleted);
            queryTaskUg.ExecuteAsync(query);
        }

        private void getCircuitPoint()
        {
            Query query = new Query();
            query.ReturnGeometry = true;
            query.Where = string.Format(DEFINITION_CIRCUIT, _circuitIdSelected);
            QueryTask queryTaskCP = new QueryTask(_circuitPointUrl);
            queryTaskCP.ExecuteCompleted += new EventHandler<QueryEventArgs>(queryTaskCP_ExecuteCompleted);
            queryTaskCP.ExecuteAsync(query);
        }

        private void SetRenderColor(IList<Graphic> graphics)
        {
            foreach (Graphic graphic in graphics)
            {
                Graphic lineGraphic = new Graphic();
                lineGraphic.Geometry = graphic.Geometry;
                lineGraphic.Symbol = new SimpleLineSymbol()
                {
                    Color = new SolidColorBrush(Color.FromArgb(255, 255, 0, 0)),
                    Width = 4
                };
                _lineGraphics.Add(lineGraphic);
            }
        }

        private void queryTaskUg_ExecuteCompleted(object sender, QueryEventArgs e)
        {
            _primaryUGReturned = true;
            FeatureSet featureSet = e.FeatureSet;
            _primaryUGLayerExtentGeometry = GetGeometry(featureSet);
            _primaryUgGraphics = featureSet.Features;
            SetRenderColor(_primaryUgGraphics);
            if (_primaryOHReturned)
                CheckForSecondaryNetwork();
        }

        private void queryTaskOh_ExecuteCompleted(object sender, QueryEventArgs e)
        {
            _primaryOHReturned = true;
            FeatureSet featureSet = e.FeatureSet;
            _primaryOHLayerExtentGeometry = GetGeometry(featureSet);
            _primaryOhGraphics = featureSet.Features;
            SetRenderColor(_primaryOhGraphics);
            if (_primaryUGReturned)
                CheckForSecondaryNetwork();
        }

        private void CheckForSecondaryNetwork()
        {
            if (_primaryOhGraphics.Count > 0 || _primaryUgGraphics.Count > 0)
                ZoomToExtent(_primaryOHLayerExtentGeometry, _primaryUGLayerExtentGeometry, null);
            else
            {
                _secondaryUGLayerExtentGeometry = null;
                _secondaryOHLayerExtentGeometry = null;
                _secondaryBusBarLayerExtentGeometry = null;
                _secondaryOHReturned = false;
                _secondaryUGReturned = false;
                _secondaryBusBarReturned = false;
                _secondaryOhGraphics = null;
                _secondaryUgGraphics = null;
                _secondaryBusBarGraphics = null;
                Query query = new Query();
                query.ReturnGeometry = true;
                query.Where = string.Format(DEFINITION_CIRCUIT, _circuitIdSelected);
                QueryTask queryTaskSecOh = new QueryTask(_secondaryOhUrl);
                queryTaskSecOh.ExecuteCompleted += new EventHandler<QueryEventArgs>(queryTaskSecOh_ExecuteCompleted);
                queryTaskSecOh.ExecuteAsync(query);
                QueryTask queryTaskSecUg = new QueryTask(_secondaryUgUrl);
                queryTaskSecUg.ExecuteCompleted += new EventHandler<QueryEventArgs>(queryTaskSecUg_ExecuteCompleted);
                queryTaskSecUg.ExecuteAsync(query);
                query.Where = string.Format(DEFINITION_CIRCUIT, _circuitIdSelected) + " AND SUBTYPECD=2"; //SUBTYPECD=2 for Secondary
                QueryTask queryTaskSecBusBar = new QueryTask(_secondaryBusBarUrl);
                queryTaskSecBusBar.ExecuteCompleted += new EventHandler<QueryEventArgs>(queryTaskSecBusBar_ExecuteCompleted);
                queryTaskSecBusBar.ExecuteAsync(query);
            }
        }

        private void queryTaskSecUg_ExecuteCompleted(object sender, QueryEventArgs e)
        {
            _secondaryUGReturned = true;
            FeatureSet featureSet = e.FeatureSet;
            _secondaryUGLayerExtentGeometry = GetGeometry(featureSet);
            _secondaryUgGraphics = featureSet.Features;
            SetRenderColor(_secondaryUgGraphics);
            if (_secondaryOHReturned && _secondaryBusBarReturned)
                ZoomToExtent(_secondaryOHLayerExtentGeometry, _secondaryUGLayerExtentGeometry, _secondaryBusBarLayerExtentGeometry);
        }

        private void queryTaskSecOh_ExecuteCompleted(object sender, QueryEventArgs e)
        {
            _secondaryOHReturned = true;
            FeatureSet featureSet = e.FeatureSet;
            _secondaryOHLayerExtentGeometry = GetGeometry(featureSet);
            _secondaryOhGraphics = featureSet.Features;
            SetRenderColor(_secondaryOhGraphics);
            if (_secondaryUGReturned && _secondaryBusBarReturned)
                ZoomToExtent(_secondaryOHLayerExtentGeometry, _secondaryUGLayerExtentGeometry, _secondaryBusBarLayerExtentGeometry);
        }

        private void queryTaskSecBusBar_ExecuteCompleted(object sender, QueryEventArgs e)
        {
            _secondaryBusBarReturned = true;
            FeatureSet featureSet = e.FeatureSet;
            _secondaryBusBarLayerExtentGeometry = GetGeometry(featureSet);
            _secondaryBusBarGraphics = featureSet.Features;
            SetRenderColor(_secondaryBusBarGraphics);
            if (_secondaryUGReturned && _secondaryOHReturned)
                ZoomToExtent(_secondaryOHLayerExtentGeometry, _secondaryUGLayerExtentGeometry, _secondaryBusBarLayerExtentGeometry);
        }

        private void queryTaskCP_ExecuteCompleted(object sender, QueryEventArgs e)
        {
            FeatureSet featureSet = e.FeatureSet;
            if (featureSet.Features.Count > 0)
                _circuitPointGraphic = featureSet.Features[0];
            SetExtentFromDefinitionQuery(string.Format(DEFINITION_CIRCUIT, _circuitIdSelected));
        }

        private Envelope GetGeometry(FeatureSet featureSet)
        {
            Envelope env = null;
            foreach (Graphic feature in featureSet.Features)
            {
                if (env == null)
                    env = feature.Geometry.Extent.Clone();
                else
                    env = env.Union(feature.Geometry.Extent);
            }

            return env;
        }

        private void SetSelectedEnvelope(Envelope newEnvelope)
        {
            if (_selectedEnvelope != null && _zoomToAllSelected)
            {
                _selectedEnvelope = _selectedEnvelope.Union(newEnvelope);
            }
            else
            {
                _selectedEnvelope = newEnvelope;
            }

        }

        private void ZoomToExtent(Envelope envelope1, Envelope envelope2, Envelope envelope3)
        {
            const double EXPAND_FACTOR = 1.25;

            if (envelope1 != null)
            {
                SetSelectedEnvelope(envelope1);
                if (envelope2 != null)
                {
                    _selectedEnvelope = _selectedEnvelope.Union(envelope2);
                }
                if (envelope3 != null)
                {
                    _selectedEnvelope = _selectedEnvelope.Union(envelope3);
                }
            }
            else if (envelope2 != null) // envelope1 is null
            {
                SetSelectedEnvelope(envelope2);
                if (envelope3 != null)
                {
                    _selectedEnvelope = _selectedEnvelope.Union(envelope3);
                }
            }
            else if (envelope3 != null) // envelopes 1 and 2 are null
            {
                SetSelectedEnvelope(envelope3);
            }

            if (_selectedEnvelope != null)
            {
                double zoomScale = Convert.ToDouble(1900);
                double zoomResolution;
                zoomResolution = (zoomScale) / 96;
                double xMin = _selectedEnvelope.Extent.GetCenter().X - (_map.ActualWidth * zoomResolution * 0.5);
                double yMin = _selectedEnvelope.Extent.GetCenter().Y - (_map.ActualHeight * zoomResolution * 0.5);
                double xMax = _selectedEnvelope.Extent.GetCenter().X + (_map.ActualWidth * zoomResolution * 0.5);
                double yMax = _selectedEnvelope.Extent.GetCenter().Y + (_map.ActualHeight * zoomResolution * 0.5);

                // Construct an Envelope from the bounding extents.
                ESRI.ArcGIS.Client.Geometry.Envelope zoomEnvelope = new ESRI.ArcGIS.Client.Geometry.Envelope(xMin, yMin, xMax, yMax);
                zoomEnvelope.SpatialReference = _map.SpatialReference;

                _map.ZoomTo(zoomEnvelope);
                //_map.ZoomTo(_selectedEnvelope.Expand(EXPAND_FACTOR));
                BindgridNumber(_selectedEnvelope.Extent);
            }
            else
            {
                MessageBox.Show("No features found for this CircuitID");
            }

            if (_lineGraphics.Count > 0)
            {
                CreateCircuitGraphics();
            }
            ConfigUtility.UpdateStatusBarText("");
        }

        private void CreateCircuitGraphics()
        {
            if (!_map.Layers.Contains(_graphicsLayer))
            {
                _map.Layers.Add(_graphicsLayer);
                _map.Layers.Add(_graphicsLayerPoint);
                _map.Layers.Add(_graphicsLayerText);
            }
            _graphicsLayer.Graphics.AddRange(_lineGraphics);

            if (_circuitPointGraphic != null)
            {
                Graphic pointGraphic = new Graphic();
                pointGraphic.Geometry = _circuitPointGraphic.Geometry;
                pointGraphic.Symbol = new ESRI.ArcGIS.Client.Symbols.SimpleMarkerSymbol()
                {
                    Color = new SolidColorBrush(Color.FromArgb(255, 255, 0, 0)),
                    Size = 40,
                    Style = ESRI.ArcGIS.Client.Symbols.SimpleMarkerSymbol.SimpleMarkerStyle.Triangle
                };
                if (!(_graphicsLayerPoint.Graphics.Contains(pointGraphic)))
                    _graphicsLayerPoint.Graphics.Add(pointGraphic);
            }

            // Need centroid of extent
            if (_selectedEnvelope != null)
            {
                TextSymbol textSymbol = new TextSymbol()
                {
                    FontFamily = _fontFamily,
                    Foreground = new SolidColorBrush(Color.FromArgb(255, 255, 0, 0)),
                    FontSize = FONT_SIZE,
                    Text = PART_FilterAutoCompleteTextBlock.Text
                };

                Size textSize = CADExportControl.GetTextSize(textSymbol.Text, _fontFamily, FONT_SIZE);
                textSymbol.OffsetX = textSize.Width / 2;
                textSymbol.OffsetY = textSize.Height / 2;

                Graphic textGraphic = new Graphic() { Symbol = textSymbol, Geometry = _selectedEnvelope.GetCenter() };
                textGraphic.Geometry = _selectedEnvelope.GetCenter();
                _graphicsLayerText.Graphics.Add(textGraphic);
            }

            _lineGraphics.Clear();
        }

        #endregion

        #region  Selection Polygon on Map and grid id
        public bool MapEventIsOn = false;

        public void SelectPolygonEvent(bool isActive)
        {
            if (isActive == true)
            {
                isDrawPolygonActive = true;
                _drawControl.DrawMode = DrawMode.None;
            }
            else
            {
                isDrawPolygonActive = false;
            }
            //_polygon = null;
        }

        private void StartListeningToMapEvents()
        {
            if (!MapEventIsOn)
            {

                this._map.MouseMove += _map_MouseMovePONS;
                this._map.MouseLeftButtonDown += _map_MouseLeftButtonDownPONS;
                this.MapEventIsOn = true;

            }
        }

        private void StopListeningToMapEvents()
        {
            if (MapEventIsOn)
            {
                this._map.MouseMove -= _map_MouseMovePONS;
                this._map.MouseLeftButtonDown -= _map_MouseLeftButtonDownPONS;
                this.MapEventIsOn = false;
            }
        }
        private void _map_MouseMovePONS(object sender, MouseEventArgs e)
        {
            try
            {
                var map = sender as Map;
                if (map == null) return;

                if (!_layers.Any() || _layers.Last().OriginPoint == null || !_isMeasuring) return;

                if (_finishMeasure) return;

                _graphicCount = _layers.Last().Layer.Graphics.Count;
                int g = _graphicCount - 1;

                MapPoint p = _map.ScreenToMap(e.GetPosition(_map));

                // Update the total distance geometry for Polyline mode.
                if (MeasureMode == MeasureAction.Mode.Polyline)
                {
                    var totSymGeo = _layers.Last().Layer.Graphics[TotalDistanceSymbolPosition].Geometry;
                    if (totSymGeo == null) return;

                    totSymGeo = p;

                    _layers.Last().Layer.Graphics[TotalDistanceSymbolPosition].Geometry = totSymGeo;
                }

                // We could use a geometry service.
                #region Geometry Service Length

                var mapDistance = _layers.Last().OriginPoint.DistanceTo(p);
                if (_firstTime)
                {
                    _firstTime = false;
                    _mapDistance = mapDistance;

                    var geometryService = new GeometryService(_geometryServicePolygon.Url);
                    geometryService.Failed += _geometryService_Failed_PONS;
                    geometryService.ProjectCompleted += GeometryServiceMoveDistanceProjectCompletedPONS;


                    var graphics = new List<Graphic>();
                    var graphic1 = new Graphic();
                    var graphic2 = new Graphic();
                    graphic1.Geometry = _layers.Last().OriginPoint;
                    graphic2.Geometry = p;
                    graphics.Add(graphic1);
                    graphics.Add(graphic2);

                    geometryService.ProjectAsync(graphics, _map.SpatialReference);
                }
                else
                {
                    var dist = _distanceFactor * mapDistance;

                    var symb = _layers.Last().Layer.Graphics[g].Symbol as RotatingTextSymbol;
                    if (symb != null)
                    {
                        var displayDistance = ConvertMetersToActiveUnits(dist, ESRI.ArcGIS.Client.Actions.DistanceUnit.Feet);

                        symb.Text = string.Format("{0}{1}", RoundToSignificantDigit(displayDistance), PrettyUnits(ESRI.ArcGIS.Client.Actions.DistanceUnit.Feet));
                        _layers.Last().Layer.Graphics[g].Symbol = symb;
                    }

                    _layers.Last().TempTotalLength = _layers.Last().TotalLength + dist;

                    if (MeasureMode == MeasureAction.Mode.Polyline)
                    {
                        var totSym = _layers.Last().Layer.Graphics[TotalDistanceSymbolPosition].Symbol as RotatingTextSymbol;
                        if (totSym == null) return;

                        totSym.Text = string.Format("Total = {0}{1}", RoundToSignificantDigit(_layers.Last().TempTotalLength), PrettyUnits(ESRI.ArcGIS.Client.Actions.DistanceUnit.Feet), PrettyUnits(ESRI.ArcGIS.Client.Actions.DistanceUnit.Feet));

                        _layers.Last().Layer.Graphics[TotalDistanceSymbolPosition].Symbol = totSym;
                    }
                }

                #endregion Geometry Service Length

                // Or we could estimate.
                #region Estimate Length

                var midpoint = new MapPoint((p.X + _layers.Last().OriginPoint.X) / 2, (p.Y + _layers.Last().OriginPoint.Y) / 2);
                var polypoints = new PointCollection();
                Polygon poly;

                if (MeasureMode == MeasureAction.Mode.Polygon && _layers.Last().Points.Count > 2)
                {
                    Graphic graphic = _layers.Last().Layer.Graphics[0];
                    poly = graphic.Geometry as Polygon;

                    if (poly != null)
                    {
                        polypoints = poly.Rings[0];
                        int lastPt = polypoints.Count - 1;
                        polypoints[lastPt] = p;
                    }
                }
                _layers.Last().Layer.Graphics[g - 2].Geometry = midpoint;

                var line = _layers.Last().Layer.Graphics[g - 1].Geometry as ESRI.ArcGIS.Client.Geometry.Polyline;
                if (line != null) line.Paths[0][1] = p;
                _layers.Last().Layer.Graphics[g].Geometry = midpoint;

                double angle = Math.Atan2((p.X - _layers.Last().OriginPoint.X), (p.Y - _layers.Last().OriginPoint.Y)) / Math.PI * 180 - 90;
                if (angle > 90 || angle < -90) angle -= 180;

                var symbol = _layers.Last().Layer.Graphics[g].Symbol as RotatingTextSymbol;
                if (symbol != null)
                {
                    symbol.Angle = angle;
                    _layers.Last().Layer.Graphics[g].Symbol = symbol;
                }

                if (MeasureMode == MeasureAction.Mode.Polyline)
                {
                    var totSym = _layers.Last().Layer.Graphics[TotalDistanceSymbolPosition].Symbol as RotatingTextSymbol;
                    if (totSym == null) return;

                    _layers.Last().Layer.Graphics[TotalDistanceSymbolPosition].Symbol = totSym;
                }

                #endregion Estimate Length

                if (MeasureMode != MeasureAction.Mode.Polygon) return;

                if (polypoints.Count <= 2) return;

                poly = _layers.Last().Layer.Graphics[0].Geometry as Polygon;
                if (poly == null) return;

                MapPoint anchor = poly.Extent.GetCenter();

                _layers.Last().Layer.Graphics[TotalAreaSymbolPosition].Geometry = anchor;

                // Don't fire too many in mouse move.
                if (_areaAsyncCalls >= 1) return;

                _mouseMove++;
                if (_mouseMove >= 50)
                {
                    _mouseMove = 0;
                    var polyGraphic = new Graphic { Symbol = new ESRI.ArcGIS.Client.Symbols.SimpleFillSymbol(), Geometry = poly };
                    polyGraphic.Geometry.SpatialReference = map.SpatialReference;
                    var polyGraphicList = new List<Graphic> { polyGraphic };
                    _areaAsyncCalls++;
                }
            }
            catch (Exception ex)
            {
                logger.Error("StandardPrint -_map_MouseMove: " + ex.Message);
            }
        }

        private void _map_MouseLeftButtonDownPONS(object sender, MouseButtonEventArgs e)
        {
            try
            {
                e.Handled = true;

                _firstTime = true;

                if (_clearAll == true)
                {
                    _layers.Add(new SegmentLayer());
                    _layers.Last().Layer.ID = STAND_POLYGON_GRAPHIC_LAYER;
                    Layer graphicLayer = _map.Layers[STAND_POLYGON_GRAPHIC_LAYER];
                    if (graphicLayer != null)
                    {
                        _map.Layers.Remove(graphicLayer);
                    }
                    _map.Layers.Add(_layers.Last().Layer);
                    _clearAll = false;
                }
                else if (!_layers.Any())
                {
                    _layers.Add(new SegmentLayer());
                    _layers.Last().Layer.ID = STAND_POLYGON_GRAPHIC_LAYER;
                    Layer graphicLayer = _map.Layers[STAND_POLYGON_GRAPHIC_LAYER];
                    if (graphicLayer != null)
                    {
                        _map.Layers.Remove(graphicLayer);
                    }
                    _map.Layers.Add(_layers.Last().Layer);
                }

                Point pt = e.GetPosition(null);

                if (!_finishedSegment && Math.Abs(pt.X - _layers.Last().LastClick.X) < 2 && Math.Abs(pt.Y - _layers.Last().LastClick.Y) < 2)
                {
                    _finishMeasure = true;
                    MapMouseDoubleClickPONS(sender, e);
                    _MappolygongraphicsLayer.Graphics.Clear();
                    ClearPreviousLayers();
                    _clearAll = true;
                }

                else if (!(_finishingSegment || _finishingArea))
                {
                    _finishedSegment = false;

                    var map = sender as Map;
                    if (map == null) return;

                    if (_layers.Last().Points.Count == 0)
                    {
                        if (MeasureMode == MeasureAction.Mode.Polygon)
                        {
                            var areaGraphic = new Graphic
                            {
                                Symbol = FillSymbol
                            };

                            _layers.Last().Layer.Graphics.Add(areaGraphic);
                            var areaTotalGraphic = new Graphic
                            {
                                Symbol = new RotatingTextSymbol()
                            };

                            // Bump the Z up so fill will be underneath.
                            areaTotalGraphic.SetZIndex(2);

                            _layers.Last().Layer.Graphics.Add(areaTotalGraphic);
                        }
                    }

                    if (_layers.Last().OriginPoint != null) _layers.Last().PrevOrigin = _layers.Last().OriginPoint;

                    _layers.Last().OriginPoint = map.ScreenToMap(e.GetPosition(map));
                    _layers.Last().EndPoint = map.ScreenToMap(e.GetPosition(map));

                    var line = new ESRI.ArcGIS.Client.Geometry.Polyline();
                    var points = new PointCollection { _layers.Last().OriginPoint, _layers.Last().EndPoint };

                    line.Paths.Add(points);
                    _layers.Last().Points.Add(_layers.Last().EndPoint);

                    if (_layers.Last().Points.Count == 2) _layers.Last().Points.Add(_layers.Last().EndPoint);
                    _layers.Last().LineCount++;

                    if (MeasureMode == MeasureAction.Mode.Polygon && _layers.Last().Points.Count > 2)
                    {
                        var poly = new Polygon();
                        poly.Rings.Add(_layers.Last().Points);
                        _layers.Last().Layer.Graphics[0].Geometry = poly;
                        _layers.Last().Layer.Graphics[0].Symbol = _fillSymbol;
                    }

                    if (MeasureMode == MeasureAction.Mode.Polyline)
                    {
                        var totalSymbol = new RotatingTextSymbol();
                        totalSymbol.OffsetY += 25;

                        var totalTextGraphic = new Graphic
                        {
                            Geometry = _layers.Last().OriginPoint,
                            Symbol = totalSymbol
                        };

                        // Bump the Z up so lines will be underneath.
                        totalTextGraphic.SetZIndex(2);

                        _layers.Last().Layer.Graphics.Add(totalTextGraphic);
                    }

                    var marker = new Graphic
                    {
                        Geometry = _layers.Last().EndPoint,
                        Symbol = _markerSymbol
                    };

                    _layers.Last().Layer.Graphics.Add(marker);

                    var lineGraphic = new Graphic
                    {
                        Geometry = line,
                        Symbol = LineSymbol
                    };

                    _layers.Last().Layer.Graphics.Add(lineGraphic);

                    var textGraphic = new Graphic
                    {
                        Geometry = _layers.Last().EndPoint,
                        Symbol = new RotatingTextSymbol()
                    };

                    textGraphic.SetZIndex(1);

                    _layers.Last().Layer.Graphics.Add(textGraphic);

                    if (_layers.Last().Points.Count > 1)
                    {
                        var graphics = new List<Graphic>();
                        var graphic1 = new Graphic();
                        var graphic2 = new Graphic();
                        graphic1.Geometry = _layers.Last().PrevOrigin;
                        graphic2.Geometry = _layers.Last().EndPoint;
                        graphics.Add(graphic1);
                        graphics.Add(graphic2);
                    }
                    else
                    {
                        _layers.Last().TotalLength = 0;
                        SegmentLengths = new List<double>();
                    }

                    _isMeasuring = true;
                }
                _layers.Last().LastClick = pt;
            }
            catch (Exception ex)
            {
                logger.Error("PONS -_map_MouseLeftButtonDownPONS: " + ex.Message);
            }
        }
        private void MapMouseDoubleClickPONS(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (e != null)
                {
                    e.Handled = true;
                }

                if (!_isMeasuring) return;
                if (!_layers.Any()) return;

                var map = sender as Map;
                if (map == null) return;

                int lastone = _layers.Last().Layer.Graphics.Count - 1;
                if (lastone < 0)
                {
                    ResetValues();
                    return;
                }

                _firstTime = true;
                _finishedSegment = true;
                _layers.Last().Layer.Graphics.RemoveAt(lastone);

                if (MeasureMode == MeasureAction.Mode.Polygon)
                {
                    if (_layers.Last().Layer.Graphics.Count < 1)
                    {
                        ResetValues();
                        return;
                    }

                    var poly1 = _layers.Last().Layer.Graphics[0].Geometry as Polygon;
                    if (poly1 == null)
                    {
                        ResetValues();
                        return;
                    }

                    MapPoint firstpoint = poly1.Rings[0][0];
                    poly1.Rings[0].Add(new MapPoint(firstpoint.X, firstpoint.Y, firstpoint.SpatialReference));

                    _layers.Last().Layer.Graphics[0].Geometry = poly1;
                    _layers.Last().Layer.Graphics[0].Symbol = _fillSymbol;

                    if (_layers.Last().Points.Count > 2)
                    {
                        var midpoint = new MapPoint((firstpoint.X + _layers.Last().OriginPoint.X) / 2, (firstpoint.Y + _layers.Last().OriginPoint.Y) / 2);

                        double angle = Math.Atan2((firstpoint.X - _layers.Last().OriginPoint.X), (firstpoint.Y - _layers.Last().OriginPoint.Y)) / Math.PI * 180 - 90;
                        if (angle > 90 || angle < -90) angle -= 180;

                        var symb = new RotatingTextSymbol { Angle = angle };

                        var textGraphic = new Graphic
                        {
                            Geometry = midpoint,
                            Symbol = symb
                        };

                        textGraphic.SetZIndex(1);

                        _layers.Last().Layer.Graphics.Add(textGraphic);

                        _finishingSegment = true;
                        _finishMeasure = true;

                        var geometryService = new GeometryService(_geometryServicePolygon.Url);
                        geometryService.Failed += _geometryService_Failed_PONS;
                        geometryService.ProjectCompleted += GeometryServiceAreaFinalSegmentDistanceProjectCompletedPONS;

                        var graphics = new List<Graphic>();
                        var graphic1 = new Graphic();
                        var graphic2 = new Graphic();
                        graphic1.Geometry = _layers.Last().OriginPoint;
                        graphic2.Geometry = firstpoint;
                        graphics.Add(graphic1);
                        graphics.Add(graphic2);
                        geometryService.ProjectAsync(graphics, _map.SpatialReference);

                        _finishingArea = true;

                        // Get the final area.
                        var areaGeometryService = new GeometryService(_geometryServicePolygon.Url);
                        areaGeometryService.Failed += _geometryService_Failed_PONS;
                        areaGeometryService.ProjectCompleted += GeometryServiceFinalAreaProjectCompletedPONS;

                        var polyGraphic = new Graphic { Geometry = poly1 };
                        polyGraphic.Symbol = _fillSymbol;
                        polyGraphic.Geometry.SpatialReference = map.SpatialReference;

                        //Set Extract Polygon

                        _polygon = poly1;

                        var polyGraphicList = new List<Graphic> { polyGraphic };

                        areaGeometryService.ProjectAsync(polyGraphicList, _map.SpatialReference);
                        DrawControl_DrawComplete();
                    }
                }
                else
                {
                    if (!_finishingSegment) ResetValues();
                }
            }
            catch (Exception ex)
            {
                logger.Error("MapMouseDoubleClickPONS: " + ex.Message);
            }
        }
        private void GeometryServiceFinalAreaProjectCompletedPONS(object sender, GraphicsEventArgs e)
        {
            try
            {
                var poly = e.Results[0].Geometry as Polygon;

                var geometryService = new GeometryService(_geometryServicePolygon.Url);
                geometryService.Failed += _geometryService_Failed_PONS;
                geometryService.AreasAndLengthsCompleted += GeometryServiceFinalAreaCompletedPONS;

                var polyGraphic = new Graphic { Symbol = new ESRI.ArcGIS.Client.Symbols.SimpleFillSymbol(), Geometry = poly };
                polyGraphic.Geometry.SpatialReference = _map.SpatialReference;

                var polyGraphicList = new List<Graphic> { polyGraphic };

                geometryService.AreasAndLengthsAsync(polyGraphicList, LinearUnit.Meter, LinearUnit.Meter, string.Empty);
            }
            catch (Exception ex)
            {
                logger.Error("PONS-GeometryServiceFinalAreaProjectCompletedPONS: " + ex.Message);
            }
        }
        private void GeometryServiceFinalAreaCompletedPONS(object sender, AreasAndLengthsEventArgs e)
        {
            try
            {
                // Re-enable clicks
                _finishingArea = false;

                var area = Math.Abs(e.Results.Areas[0]);

                // Set the property now that we have the final area.
                AreaTotal = area;
                _layers.Last().AreaTotal = area;

                if (_layers.Last().Layer.Graphics.Count < 2) return;

                var totSym = _layers.Last().Layer.Graphics[TotalAreaSymbolPosition].Symbol as RotatingTextSymbol;
                if (totSym != null) totSym.Text = string.Format("Area = {0}{1}", RoundToSignificantDigit(ConvertUnits(area, ESRI.ArcGIS.Client.Actions.AreaUnit.SquareFeet)), PrettyUnits(ESRI.ArcGIS.Client.Actions.AreaUnit.SquareFeet));

                _layers.Last().Layer.Graphics[TotalAreaSymbolPosition].Symbol = totSym;

                if (!_finishingSegment && MeasureMode != MeasureAction.Mode.Radius) ResetValues();
                // RefreshPolygonGraphics();
            }
            catch (Exception ex)
            {
                logger.Error("PONS-GeometryServiceFinalAreaCompletedPONS: " + ex.Message);
            }
        }
        private void GeometryServiceAreaFinalSegmentDistanceProjectCompletedPONS(object sender, GraphicsEventArgs e)
        {
            try
            {
                var geometryService = new GeometryService(_geometryServicePolygon.Url);
                geometryService.Failed += _geometryService_Failed_PONS;
                geometryService.DistanceCompleted += GeometryServiceAreaFinalSegmentDistanceCompletedPONS;

                int g = _layers.Last().Layer.Graphics.Count - 1;

                var distParams = new DistanceParameters
                {
                    DistanceUnit = LinearUnit.Meter,
                    Geodesic = true
                };

                geometryService.DistanceAsync(e.Results[0].Geometry, e.Results[1].Geometry, distParams, g);
            }
            catch (Exception ex)
            {
                logger.Error("GeometryServiceAreaFinalSegmentDistanceProjectCompletedPONS: " + ex.Message);
            }
        }
        private void GeometryServiceAreaFinalSegmentDistanceCompletedPONS(object sender, DistanceEventArgs e)
        {
            try
            {
                // Re-enable clicks
                _finishingSegment = false;

                var g = (int)e.UserState;
                var layer = _layers.Last().Layer.Graphics.Count < g + 1 ? _layers[_layers.Count - 2] : _layers.Last();

                var dist = e.Distance;

                SegmentLengths.Add(dist);
                layer.SegmentLengths.Add(dist);

                // Used to trigger Property Change.
                SegmentLengths = new List<double>(SegmentLengths);

                if (layer.Layer.Graphics.Count < g + 1) return;

                var symb = layer.Layer.Graphics[g].Symbol as RotatingTextSymbol;
                if (symb == null) return;

                var displayDistance = ConvertMetersToActiveUnits(dist, ESRI.ArcGIS.Client.Actions.DistanceUnit.Feet); ;

                symb.Text = string.Format("{0}{1}", RoundToSignificantDigit(displayDistance), PrettyUnits(ESRI.ArcGIS.Client.Actions.DistanceUnit.Feet));
                layer.Layer.Graphics[g].Symbol = symb;

                if (!_finishingArea && MeasureMode != MeasureAction.Mode.Radius)
                {
                    ResetValues();
                }
                //  RefreshPolygonGraphics();
            }
            catch (Exception ex)
            {
                logger.Error("PONS-GeometryServiceAreaFinalSegmentDistanceCompletedPONS: " + ex.Message);
            }
        }
        private void GeometryServiceMoveDistanceProjectCompletedPONS(object sender, GraphicsEventArgs e)
        {
            try
            {
                var geometryService = new GeometryService(_geometryServicePolygon.Url);
                geometryService.Failed += _geometryService_Failed_PONS;
                geometryService.DistanceCompleted += GeometryServiceMoveDistanceCompletedPONS;

                int g = _layers.Last().Layer.Graphics.Any() ? _layers.Last().Layer.Graphics.Count - 1 : _layers[_layers.Count - 2].Layer.Graphics.Count - 1;

                var distParams = new DistanceParameters
                {
                    DistanceUnit = LinearUnit.Meter,
                    Geodesic = true
                };

                geometryService.DistanceAsync(e.Results[0].Geometry, e.Results[1].Geometry, distParams, g);
            }
            catch (Exception ex)
            {
                logger.Error("GeometryServiceMoveDistanceProjectCompletedPONS: " + ex.Message);
            }
        }

        private void GeometryServiceMoveDistanceCompletedPONS(object sender, DistanceEventArgs e)
        {
            try
            {
                var dist = e.Distance;
                _distanceFactor = dist / _mapDistance;

                var g = (int)e.UserState;

                if (_layers.Last().Layer.Graphics.Count < g + 1) return;

                var symb = _layers.Last().Layer.Graphics[g].Symbol as RotatingTextSymbol;
                if (symb != null)
                {
                    var displayDistance = ConvertMetersToActiveUnits(dist, ESRI.ArcGIS.Client.Actions.DistanceUnit.Feet);

                    symb.Text = string.Format("{0}{1}", RoundToSignificantDigit(displayDistance), PrettyUnits(ESRI.ArcGIS.Client.Actions.DistanceUnit.Feet));
                    _layers.Last().Layer.Graphics[g].Symbol = symb;
                }

                _layers.Last().TempTotalLength = _layers.Last().TotalLength + dist;

                if (MeasureMode != MeasureAction.Mode.Polyline) return;

                var totSym = _layers.Last().Layer.Graphics[TotalDistanceSymbolPosition].Symbol as RotatingTextSymbol;
                if (totSym == null) return;

                totSym.Text = string.Format("Total = {0}{1}", RoundToSignificantDigit(ConvertMetersToActiveUnits(_layers.Last().TempTotalLength, ESRI.ArcGIS.Client.Actions.DistanceUnit.Feet)), PrettyUnits(ESRI.ArcGIS.Client.Actions.DistanceUnit.Feet));

                _layers.Last().Layer.Graphics[TotalDistanceSymbolPosition].Symbol = totSym;
            }
            catch (Exception ex)
            {
                logger.Error("PONS-GeometryServiceMoveDistanceCompletedPONS: " + ex.Message);
            }
        }

        private void _geometryService_Failed_PONS(object sender, TaskFailedEventArgs e)
        {
            _geometryServicePolygon.Failed -= _geometryService_Failed_PONS;
            MessageBox.Show("An error occurred in Polygon Drawing (geoprocessing)");
        }

        private void ResetValues()
        {
            _layers.Last().MeasureMode = MeasureMode;
            _isMeasuring = false;
            _finishMeasure = false;
        }
        private void ClearPreviousLayers()
        {
            //This is to retain previously created grahpic 

            if (_layers.Count > 1)
            {

                for (int i = 0; i < _layers.Count - 1; i++)
                {
                    var layer = _layers[i];
                    layer.Layer.ClearGraphics();
                    _map.Layers.Remove(layer.Layer);
                }
                _layers.RemoveAt(0);
            }
        }
        private void InitializeDrawGraphics()
        {
            if (_drawControl == null)
            {
                _drawControl = new Draw(_map);
                _drawControl.LineSymbol = LineSymbol;
                _fillSymbol = new ESRI.ArcGIS.Client.Symbols.SimpleFillSymbol
                {
                    BorderBrush = new SolidColorBrush(Colors.Red),
                    BorderThickness = 2,
                    Fill = new SolidColorBrush(Colors.Red) { Opacity = 0.25 }
                };
                _drawControl.DrawMode = DrawMode.Polygon;
                _MappolygongraphicsLayer = new GraphicsLayer();
                _MappolygongraphicsLayer.ID = POLY_GRAPHICS_LAYER;
                if (!_map.Layers.Contains(_MappolygongraphicsLayer))
                {
                    _map.Layers.Add(_MappolygongraphicsLayer);
                }
                //_drawControl.DrawComplete += DrawControl_DrawComplete;
            }
            _drawControl.IsEnabled = true;
            _polygon = null;
        }
        public void DisableDrawPolygon()
        {
            if (MapEventIsOn == true)
            {
                SelectPolygonEvent(false);
            }
            ClearGraphics();
            isPolygonDrawn = false;
        }
        public void ClearGraphics()
        {
            if (_MappolygongraphicsLayer != null)
                _MappolygongraphicsLayer.ClearGraphics();
            if (_drawControl != null)
                _drawControl.IsEnabled = false;
            ClearLayers();
            _polygon = null;
        }
        public void ClearLayers()
        {
            foreach (var layer in _layers)
            {
                layer.Layer.ClearGraphics();
                _map.Layers.Remove(layer.Layer);
            }

            _layers.Clear();
        }
        private void DrawControl_DrawComplete()
        {
            try
            {
                //LoadMapLayers(_map.Layers);   //get map at this time for ad hoc print
                var selected = comboGridLayerSelection.SelectedItem as ComboBoxItem;
                if (selected == null) return;

                EnableDisableAll(false);
                SetGridLayerFieldNames(selected);
                var extent = new Envelope(_polygon.Extent.XMin, _polygon.Extent.YMin, _polygon.Extent.XMax, _polygon.Extent.YMax);


                isPolygonDrawn = true;
                //DeviceButton.IsEnabled = false;
                BusyIndicator.IsBusy = true;
                BusyIndicator.BusyContent = "Processing...";
                //_polygonSearchList.Clear();
                var identifyParameters = new IdentifyParameters();
                identifyParameters.Geometry = _polygon;
                var layerId = GetLayerId(selected);
                if (layerId < 0) return;

                identifyParameters.LayerIds.Add(layerId);
                identifyParameters.MapExtent = _map.Extent;
                identifyParameters.LayerOption = LayerOption.all;
                identifyParameters.Height = (int)_map.ActualHeight;
                identifyParameters.Width = (int)_map.ActualWidth;
                identifyParameters.SpatialReference = _map.SpatialReference;
                identifyParameters.Tolerance = 5;

                //var identifyTask = new IdentifyTask(_TransformerService.Substring(0, _TransformerService.LastIndexOf("/")));
                //identifyTask.Failed += new EventHandler<TaskFailedEventArgs>(identifyTaskPolygon_Failed);
                //identifyTask.ExecuteCompleted += new EventHandler<IdentifyEventArgs>(identifyTaskPolygon_ExecuteCompleted);
                //identifyTask.ExecuteAsync(identifyParameters);

                var url = (selected.Tag as Tuple<string, string, string, Tuple<string, string, string, string, string>>).Item2;
                var identifyTask = new IdentifyTask(url);
                identifyTask.Failed += new EventHandler<TaskFailedEventArgs>(identifyTask_Failed);
                identifyTask.ExecuteCompleted += new EventHandler<IdentifyEventArgs>(identifyTask_ExecuteCompleted);
                identifyTask.ExecuteAsync(identifyParameters);



            }
            catch (Exception ex)
            {
                logger.Error("PONS-DrawControl_DrawComplete: " + ex.Message);
            }
        }
        public void Map_ExtentChanged(object sender, ExtentEventArgs e)   //to handle scale change for map selection option
        {
            EnableDisableRadioButtons();
            if (RdGrid.IsChecked == true)
            {
                BindgridNumber(_map.Extent);
            }
        }


        #endregion
        #region IActiveControl Members

        private bool _isDrawPolygonActive = false;

        public bool isDrawPolygonActive
        {
            get { return _isDrawPolygonActive; }
            set { setDrawPolygonActive(value); }
        }

        private void setDrawPolygonActive(bool isDrawPolygonActive)
        {
            _isDrawPolygonActive = isDrawPolygonActive;
            //if (_fw == null) return;

            if (!isDrawPolygonActive)
            {
                // Clear everything
                //ClearGraphics();
                CursorSet.SetID(_map, "Arrow");
                StopListeningToMapEvents();
            }
            else // Active
            {
                CursorSet.SetID(_map, ExportCursor);
                InitializeDrawGraphics();
                StartListeningToMapEvents();
            }
        }

        // Using a DependencyProperty as the backing store for IsActive.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsDrawPolygonActiveProperty =
            DependencyProperty.Register("IsDrawPolygonActive", typeof(bool), typeof(StandardPrintPage), new PropertyMetadata(OnIsDrawPolygonActiveChanged));

        private static void OnIsDrawPolygonActiveChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (StandardPrintPage)d;
            var IsDrawPolygonActive = (bool)e.NewValue;

            if (IsDrawPolygonActive == false)
            {
                CursorSet.SetID(control._map, "Arrow");
            }
            else
            {
                if (control._map == null) return;
                CursorSet.SetID(control._map, ExportCursor);
            }
        }

        #endregion IActiveControl Members

        void BindgridNumber(Envelope extent)
        {
           
            SelectGridlist.Clear();
            var selected = comboGridLayerSelection.SelectedItem as ComboBoxItem;
            if (selected == null) return;

            EnableDisableAll(false);
            SetGridLayerFieldNames(selected);

            var identifyParameters = new IdentifyParameters();
            identifyParameters.Geometry = extent;

            var layerId = GetLayerId(selected);
            if (layerId < 0) return;

            identifyParameters.LayerIds.Add(layerId);
            identifyParameters.MapExtent = extent;
            identifyParameters.LayerOption = LayerOption.all;
            identifyParameters.Height = (int)_map.ActualHeight;
            identifyParameters.Width = (int)_map.ActualWidth;
            identifyParameters.SpatialReference = _map.SpatialReference;
            identifyParameters.Tolerance = 5;

            var url = (selected.Tag as Tuple<string, string, string, Tuple<string, string, string, string, string>>).Item2;
            var identifyTask = new IdentifyTask(url);
            identifyTask.Failed += new EventHandler<TaskFailedEventArgs>(identifyTask_Failed);
            identifyTask.ExecuteCompleted += new EventHandler<IdentifyEventArgs>(identifyTask_ExecuteCompleted);
            identifyTask.ExecuteAsync(identifyParameters);
        }
        private void RadioButton_StdPrintChecked(object sender, RoutedEventArgs e)
        {
            if (Rdcircuit != null)
            {
                ResetFilterGraphics();
                if (Rdcircuit.IsChecked == true)
                {
                    PART_FilterAutoCompleteTextBlock.Text = "";
                    PART_FilterAutoCompleteTextBlock.Visibility = Visibility.Visible;
                    //PART_CircuitPopup_print.IsOpen = true;
                    comboGridNumberSelection.Width = .1;
                    isPolygonDrawn = false;
                    SelectPolygonEvent(false);
                    ClearGraphics();
                    comboGridNumberSelection.ItemsSource = null;
                    PrintStandardMapButton.IsEnabled = false;
                    GridID.Text = "Enter Circuit Name:";
                }
                if (RdGrid.IsChecked == true)
                {
                    PART_FilterAutoCompleteTextBlock.Text = "";
                    GridID.Text = "Select Grid Number:";
                    comboGridNumberSelection.ItemsSource = null;
                   // PART_CircuitPopup_print.IsOpen = false;
                    PART_FilterAutoCompleteTextBlock.Visibility = Visibility.Collapsed;
                    comboGridNumberSelection.Width = 160;
                    isPolygonDrawn = false;
                    SelectPolygonEvent(false);
                    // Filter Grid Numbers
                    if (_map.Scale > 5000)
                    {
                        // FilterGridNumbers();
                    }
                    ClearGraphics();
                    BindgridNumber(_map.Extent);

                }
                if (Rdpoly.IsChecked == true)
                {
                    PART_FilterAutoCompleteTextBlock.Text = "";
                    GridID.Text = "Select Grid Number:";
                    comboGridNumberSelection.ItemsSource = null;
                    PART_FilterAutoCompleteTextBlock.Visibility = Visibility.Collapsed;
                    ClearGraphics();
                    comboGridNumberSelection.Width = 160;
                    PrintStandardMapButton.IsEnabled = false;
                    isPolygonDrawn = true;
                    SelectPolygonEvent(true);
                }
            }

        }
        public void GetFloatingWindowHeight(FloatableWindow fw)
        {
            _fw = fw;
        }
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            // this.DialogResult = false;

            _fw.Visibility = Visibility.Collapsed;
            DisableDrawPolygon();
            ClearGraphics();
            chkfirstprint = false;
            busyEmailIndicator.IsBusy = false;
            BusyIndicator.IsBusy = false;
            ClearAppResource("MapTypes");
            ClearAppResource("GridLayers");
            ClearAppResource("ScaleOptions");
            ClearAppResource("PDFRootUrl");
            ClearAppResource("PDFMaps");
            ReadTemplatesConfig();
            if (Rdcircuit.IsChecked == true || Rdpoly.IsChecked == true)
            {
                comboGridNumberSelection.ItemsSource = null;
                PART_FilterAutoCompleteTextBlock.Text = "";
                PrintStandardMapButton.IsEnabled = false;
            }
            else if (RdGrid.IsChecked == true)
            {

                BindgridNumber(_map.Extent);
                if (comboGridNumberSelection.Items.Count > 0)
                {
                    PrintStandardMapButton.IsEnabled = true;

                }
                else { PrintStandardMapButton.IsEnabled = false; }
            }

        }

        void Circuitfirstprint()
        {

            var selectedTemplate = comboTemplateSelection.SelectedValue as ComboBoxItem;
            var size = comboPageSize.SelectedValue.ToString();
            var template = comboTemplateSelection.SelectedValue as ComboBoxItem;
            List<ArcFMSilverlight.StandardMapPrintGridList> _GetgridList = new List<ArcFMSilverlight.StandardMapPrintGridList>();
            if (comboGridNumberSelection.SelectionBoxItem.ToString() == "ALL")
            {


                string urlpath =

                       _serviceAreaAttributes[_regionFieldName] + "\\"
                       + _serviceAreaAttributes[_divisionFieldName] + "\\"
                       + _serviceAreaAttributes[_districtFieldName] + "\\";

                string Mapsize = "_" + size + "_";
                string Printtemplate = template.Content.ToString();

                SelectGridlist = SelectGridlist.Distinct().ToList();

                string currentUser = WebContext.Current.User.Name;
                string[] UserName = currentUser.Split('\\');
                string userName = UserName[1].ToString();
                EndpointAddress endPoint = new EndpointAddress(endpointaddress);
                BasicHttpBinding httpbinding = new BasicHttpBinding();
                httpbinding.MaxBufferSize = 2147483647;
                httpbinding.MaxReceivedMessageSize = 2147483647;
                httpbinding.TransferMode = TransferMode.Buffered;
                httpbinding.SendTimeout = TimeSpan.FromSeconds(300);
                IServicePons cusservice = new ChannelFactory<IServicePons>(httpbinding, endPoint).CreateChannel();


                try
                {

                    cusservice.BeginGetStandardPrint(SelectGridlist, urlpath, Printtemplate, Mapsize, userName,
                     delegate(IAsyncResult result)
                     {

                         string ExporthostUrl;
                         string _hostUrl = ((IServicePons)result.AsyncState).EndGetStandardPrint(result);
                         this.Dispatcher.BeginInvoke(
                         delegate()
                         {
                             ExporthostUrl = _hostUrl.Replace("$", "\\").ToString();

                             SendEmailNotification("Succcess", userName, "", "", ExporthostUrl);
                         }
                         );
                     }, cusservice);

                }
                catch (Exception ex)
                {
                    //busyEmailIndicator.IsBusy = false;
                    logger.FatalException("Error in print gridlist", ex);
                }

            }
        }

        private void PrintStandardMapButton_Click(object sender, RoutedEventArgs e)
        {
            PrintStandardMapButton.IsEnabled = false;
            chkfirstprint = true;
            busyEmailIndicator.IsBusy = true;
            if (Rdcircuit.IsChecked == true)
            {
                if (firstprint == true)
                {
                    try
                    {

                        var selectedGridNumber = comboGridNumberSelection.SelectedValue as ComboBoxItem;
                        if (selectedGridNumber == null) return;
                        if (comboGridNumberSelection.SelectedIndex == 0)
                        {
                            var selectedGridNumberAll = comboGridNumberSelection.Items[1] as ComboBoxItem;

                            //Call to Identify on Service Area Layer
                            GetOverlappingServiceAreaInfo(selectedGridNumberAll.Tag as Graphic);

                         
                        }
                        else
                        {

                            var gridData = (selectedGridNumber.Tag as Graphic).Attributes;
                            if (gridData == null) return;

                            //Call to Identify on Service Area Layer
                            GetOverlappingServiceAreaInfo(selectedGridNumber.Tag as Graphic);
                        }



                    }
                    catch { busyEmailIndicator.IsBusy = false;
                    PrintStandardMapButton.IsEnabled = true;
                    }
                }
                else
                {
                    
                        if (PART_FilterAutoCompleteTextBlock.Text == "" || PART_FilterAutoCompleteTextBlock == null)
                        {
                            comboGridNumberSelection.ItemsSource = null;
                            string sMessage = "Please Enter Circuit Name.";
                            PrintStandardMapButton.IsEnabled = true;
                            busyEmailIndicator.IsBusy = false;
                            logger.Warn(sMessage);
                             MessageBox.Show(sMessage);
                            return;
                        }

                    
                }
            }
            if (firstprint == false)
            {
                if (comboGridNumberSelection.Items.Count > 0)
                {
                    try
                    {
                        var selectedTemplate = comboTemplateSelection.SelectedValue as ComboBoxItem;
                        var size = comboPageSize.SelectedValue.ToString();
                        var template = comboTemplateSelection.SelectedValue as ComboBoxItem;
                        List<ArcFMSilverlight.StandardMapPrintGridList> _GetgridList = new List<ArcFMSilverlight.StandardMapPrintGridList>();

                        //SCALE
                        //REGION
                        //Division
                        //DISTRICT
                        //Plat Number

                        //Run Checks on Availability of Configured Attributes


                        if (!(_serviceAreaAttributes.ContainsKey(_regionFieldName)) && (_serviceAreaAttributes.ContainsKey(_divisionFieldName)) && (_serviceAreaAttributes.ContainsKey(_districtFieldName)))
                        {

                            busyEmailIndicator.IsBusy = false;
                            string sMessage = "Configured Attributes are not present on the Service Area Layer -- Please Check Configuration";
                            PrintStandardMapButton.IsEnabled = true;
                            logger.Warn(sMessage);
                            //  MessageBox.Show(sMessage);
                            return;
                        }


                        if (comboGridNumberSelection.SelectionBoxItem.ToString() == "ALL")
                        {


                            string urlpath =

                                   _serviceAreaAttributes[_regionFieldName] + "\\"
                                   + _serviceAreaAttributes[_divisionFieldName] + "\\"
                                   + _serviceAreaAttributes[_districtFieldName] + "\\";

                            string Mapsize = "_" + size + "_";
                            string Printtemplate = template.Content.ToString();

                            SelectGridlist = SelectGridlist.Distinct().ToList();

                            string currentUser = WebContext.Current.User.Name;
                            string[] UserName = currentUser.Split('\\');
                            string userName = UserName[1].ToString();
                            EndpointAddress endPoint = new EndpointAddress(endpointaddress);
                            BasicHttpBinding httpbinding = new BasicHttpBinding();
                            httpbinding.MaxBufferSize = 2147483647;
                            httpbinding.MaxReceivedMessageSize = 2147483647;
                            httpbinding.TransferMode = TransferMode.Buffered;
                            httpbinding.SendTimeout = TimeSpan.FromSeconds(300);
                            IServicePons cusservice = new ChannelFactory<IServicePons>(httpbinding, endPoint).CreateChannel();


                            try
                            {

                                cusservice.BeginGetStandardPrint(SelectGridlist, urlpath, Printtemplate, Mapsize, userName,
                                 delegate(IAsyncResult result)
                                 {

                                     string ExporthostUrl;
                                     string _hostUrl = ((IServicePons)result.AsyncState).EndGetStandardPrint(result);
                                     this.Dispatcher.BeginInvoke(
                                     delegate()
                                     {
                                         ExporthostUrl = _hostUrl.Replace("$", "\\").ToString();

                                         SendEmailNotification("Succcess", userName, "", "", ExporthostUrl);
                                     }
                                     );
                                 }, cusservice);

                                DispatcherTimer rowDetailsDispatcherTimer = new DispatcherTimer();
                                rowDetailsDispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, _timelimit); // Milliseconds
                                rowDetailsDispatcherTimer.Tick += new EventHandler(PdfDispatcherTimer_Tick);
                                rowDetailsDispatcherTimer.Start();
                            }
                            catch (Exception ex)
                            {
                                PrintStandardMapButton.IsEnabled = true;
                                busyEmailIndicator.IsBusy = false;
                                logger.FatalException("Error in print gridlist", ex);
                            }

                        }
                        else
                        {


                            var selectedGridNumber = comboGridNumberSelection.SelectedValue as ComboBoxItem;
                            var gridData = (selectedGridNumber.Tag as Graphic).Attributes;
                            //Run Checks on the Grid Layer for availability of configured Attributes
                            if (!(gridData.ContainsKey(_scaleFieldName)) && (gridData.ContainsKey(_gridnumberFieldName)))
                            {
                                string sMessage = "Configured Attributes are not present on the Grid Layer -- Please Check Configuration";
                                busyEmailIndicator.IsBusy = false;
                                logger.Warn(sMessage);
                                // MessageBox.Show(sMessage);
                                return;
                            }
                            string url = Application.Current.Resources["PDFRootUrl"] + "/"
                                    + template.Content + "-"
                                    + gridData[_scaleFieldName] + "/"
                                    + _serviceAreaAttributes[_regionFieldName] + "/"
                                    + _serviceAreaAttributes[_divisionFieldName] + "/"
                                    + _serviceAreaAttributes[_districtFieldName] + "/"
                                    + template.Content + "_"
                                    + gridData[_gridnumberFieldName] + "_"
                                    + size + "_"
                                    + gridData[_scaleFieldName] + ".pdf";

                            //string url = @"http://vm-pgeweb101.miner.com/PDFMaps/DistributionMap-500/CentralValley/Kern/Kern/DistributionMap_2926274_24x36_500.pdf";
                            Uri uri = new Uri(url);
                            HtmlPage.Window.Navigate(uri, "_blank");
                            busyEmailIndicator.IsBusy = false;
                            PrintStandardMapButton.IsEnabled = true;
                        }
                    }
                    catch
                    {
                        busyEmailIndicator.IsBusy = false;
                        string sMessage = "Configured Attributes are not present on the Grid Layer -- Please Check Configuration";
                        PrintStandardMapButton.IsEnabled = true;
                        logger.Warn(sMessage);
                        //  MessageBox.Show(sMessage);
                        return;
                    }
                }
                else
                {
                    string sMessage = "Please zoom map below scale 1900. ";
                    busyEmailIndicator.IsBusy = false;
                    PrintStandardMapButton.IsEnabled = true;
                    logger.Warn(sMessage);
                    MessageBox.Show(sMessage);
                    return;
                }
            }

        }
        void PdfDispatcherTimer_Tick(object sender, EventArgs e)
        {
            ((DispatcherTimer)sender).Stop();
            if (busyEmailIndicator.IsBusy == true)
            {

                MessageBox.Show(_printmsg);
                PrintStandardMapButton.IsEnabled = true;
                busyEmailIndicator.IsBusy = false;
            }
        }
        //ssbl
        public void SendEmailNotification(string strStatus, string strUSERNAME, string shutdownID, string strDateTime, string strLocationDecs)
        {
            string emailTo = "";
            string stremailURL = "";
            try
            {


                emailTo = strUSERNAME;

                if (strStatus.ToUpper() == "Succcess".ToUpper())
                {
                    if (strLocationDecs == "No Pdf Map file in this folder")
                    {

                        busyEmailIndicator.IsBusy = false;
                        PrintStandardMapButton.IsEnabled = true;
                        MessageBox.Show("No Pdf file found  in this location");
                    }
                    else
                    {
                        stremailURL = objPonsServiceCall.GetStandardPrintSendEmailAddress("1", strUSERNAME + "@pge.com", strUSERNAME, "", strDateTime, strLocationDecs);
                        SendEmailToPrint(stremailURL, strLocationDecs);

                    }

                  
                }
                else
                {
                    busyEmailIndicator.IsBusy = false;
                    MessageBox.Show("No data has sent to Printing");
                    return;
                }
            }
            catch (Exception ex)
            {
                busyEmailIndicator.IsBusy = false;
                logger.FatalException("Error in  SendEmail", ex);
            }
        }
        public void SendEmailToPrint(string strURL, string strLocationDecs)
        {
            try
            {
                WebClient client = new WebClient();
                UriBuilder uriBuilder = new UriBuilder(strURL);
                client.DownloadStringAsync(uriBuilder.Uri);
               
                if (busyEmailIndicator.IsBusy == true)
                {
                    PrintStandardMapButton.IsEnabled = true;
                    busyEmailIndicator.IsBusy = false;
                    MessageBox.Show("Your Standard Map Print is completed and mail has been sent. Please check.");
                }
               
            }
            catch (Exception ex)
            {
                logger.FatalException("Error in Print", ex);
                busyEmailIndicator.IsBusy = false;
                PrintStandardMapButton.IsEnabled = true;
            }
        }
        #region Select grid Number
        void comboGridLayerSelection_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                comboGridNumberSelection.ItemsSource = null;
                var selected = comboGridLayerSelection.SelectedItem as ComboBoxItem;
                if (selected == null) return;

                EnableDisableAll(false);
                SetGridLayerFieldNames(selected);

                var identifyParameters = new IdentifyParameters();

                //Bug fix -  INC000004672579
                if (Rdpoly.IsChecked == true && _polygon != null)     
                {
                    identifyParameters.Geometry = _polygon;
                }
                else if (RdGrid.IsChecked == true)
                {
                    identifyParameters.Geometry = _map.Extent;
                }
                else if (Rdcircuit.IsChecked == true && _selectedEnvelope != null)
                {
                    identifyParameters.Geometry = _selectedEnvelope.Extent;
                }

                var layerId = GetLayerId(selected);
                if (layerId < 0) return;

                identifyParameters.LayerIds.Add(layerId);
                identifyParameters.MapExtent = _map.Extent;
                //identifyParameters.LayerOption = LayerOption.visible;
                identifyParameters.LayerOption = LayerOption.all;
                identifyParameters.Height = (int)_map.ActualHeight;
                identifyParameters.Width = (int)_map.ActualWidth;
                identifyParameters.SpatialReference = _map.SpatialReference;
                identifyParameters.Tolerance = 5;

                var url = (selected.Tag as Tuple<string, string, string, Tuple<string, string, string, string, string>>).Item2;
                var identifyTask = new IdentifyTask(url);
                identifyTask.Failed += new EventHandler<TaskFailedEventArgs>(identifyTask_Failed);
                identifyTask.ExecuteCompleted += new EventHandler<IdentifyEventArgs>(identifyTask_ExecuteCompleted);
                identifyTask.ExecuteAsync(identifyParameters);
            }
            catch { BusyIndicator.IsBusy = false; }

        }
        void identifyTask_ExecuteCompleted(object sender, IdentifyEventArgs e)
        {
            if (e.IdentifyResults != null)
            {
                comboGridNumberSelection.ItemsSource = null;
                _gridNumberResults = e.IdentifyResults;

                FilterGridNumbers();

                EnableDisableAll(true);
            }
        }
        private void ClearAppResource(string resourceName)
        {
            if (Application.Current.Resources.Contains(resourceName))
                Application.Current.Resources.Remove(resourceName);
        }

        private int GetLayerId(ComboBoxItem selected)
        {
            if (selected == null) return -1;

            var layerIdString = (selected.Tag as Tuple<string, string, string, Tuple<string, string, string, string, string>>).Item3;
            int layerId = -1;
            if (Int32.TryParse(layerIdString, out layerId) == true)
            {
                return layerId;
            }
            return -1;
        }

        private void SetGridLayerFieldNames(ComboBoxItem selected)
        {
            if (selected == null) return;
            //(selected.Tag as Tuple<string, string, string, string, string, string, Tuple<string, string, string, string, string>>).Item2;

            Tuple<string, string, string, string, string> fieldElements = (selected.Tag as Tuple<string, string, string, Tuple<string, string, string, string, string>>).Item4;

            _scaleFieldName = fieldElements.Item1;
            _regionFieldName = fieldElements.Item2;
            _divisionFieldName = fieldElements.Item3;
            _districtFieldName = fieldElements.Item4;
            _gridnumberFieldName = fieldElements.Item5;

        }

        private void GetOverlappingServiceAreaInfo(Graphic inGraphic)
        {

            var selected = comboGridLayerSelection.SelectedItem as ComboBoxItem;
            if ((selected == null) || (inGraphic == null))
            {
                string sMessage = "No Selected Grid Feature";
                logger.Warn(sMessage);
                return;
            }

            if ((_serviceAreaLayerId < 0) || (_serviceAreaMapServiceUrl.Length < 1))
            {
                string sMessage = "Check Configuration: Templates.config -- Service Area Resource configuration are incorrect";
                logger.Warn(sMessage);
                return;
            }

            EnableDisableAll(false);
            SetGridLayerFieldNames(selected);

            var identifyParameters = new IdentifyParameters();
            identifyParameters.Geometry = inGraphic.Geometry;

            var layerId = _serviceAreaLayerId;
            if (layerId < 0) return;

            identifyParameters.LayerIds.Add(layerId);
            identifyParameters.MapExtent = _map.Extent;
            identifyParameters.LayerOption = LayerOption.all;
            identifyParameters.Height = (int)_map.ActualHeight;
            identifyParameters.Width = (int)_map.ActualWidth;
            identifyParameters.SpatialReference = _map.SpatialReference;
            identifyParameters.Tolerance = 5;
            identifyParameters.ReturnGeometry = false;

            var url = _serviceAreaMapServiceUrl;
            var identifyTask = new IdentifyTask(url);
            identifyTask.Failed += new EventHandler<TaskFailedEventArgs>(identifyTask_Failed);

            
                if (firstprint == true)
                {
                    //for (int i = 0; i < 10; i++)
                    //{
                    //    if (firstprint == true)
                    //    {
                    //        continue;
                    //    }


                    //}
                    identifyTask.ExecuteCompleted += new EventHandler<IdentifyEventArgs>(GetServiceAreaidentifyTask_ExecuteCompleted_First);
                    identifyTask.ExecuteAsync(identifyParameters);
                }
            
            else
            {
                identifyTask.ExecuteCompleted += new EventHandler<IdentifyEventArgs>(GetServiceAreaidentifyTask_ExecuteCompleted);
                identifyTask.ExecuteAsync(identifyParameters);
            }
        }
        void GetServiceAreaidentifyTask_ExecuteCompleted(object sender, IdentifyEventArgs e)
        {
            List<IdentifyResult> results = new List<IdentifyResult>();


            if (e.IdentifyResults == null)
            {
                _serviceAreaAttributes = null;
                firstprint = true;
            }
            else
            {
                
                results = e.IdentifyResults;
                if (results.Count == 0)
                { firstprint = true; }
                else { firstprint = false; }
                foreach (IdentifyResult result in results)
                {
                    _serviceAreaAttributes = result.Feature.Attributes;
                }
            }

            EnableDisableAll(true);

        }

        void GetServiceAreaidentifyTask_ExecuteCompleted_First(object sender, IdentifyEventArgs e)
        {
            List<IdentifyResult> results = new List<IdentifyResult>();


            if (e.IdentifyResults == null)
            {
                _serviceAreaAttributes = null;
                firstprint = true;
            }
            else
            {

                results = e.IdentifyResults;
                if (results.Count == 0)
                { firstprint = true; }
                else { firstprint = false; }
                foreach (IdentifyResult result in results)
                {
                    _serviceAreaAttributes = result.Feature.Attributes;
                }
            }

            EnableDisableAll(true);
            if (chkfirstprint == true)
            {
                Circuitfirstprint();
            }
        }
        void identifyTask_Failed(object sender, TaskFailedEventArgs e)
        {
            comboGridNumberSelection.Items.Clear();
            EnableDisableAll(true);
        }
        private void EnableDisableAll(bool isEnabled)
        {
            IsEnabled = isEnabled;
            EnableDisablePrintButton();

            if (isEnabled == false)
            {
                BusyIndicator.BusyContent = "Loading...";
                BusyIndicator.IsBusy = true;
            }
            else
            {
                BusyIndicator.IsBusy = false;
            }
        }
        #endregion
        private void EnableDisablePrintButton()
        {
            if ((comboTemplateSelection.SelectedIndex >= 0) &&
                (comboGridLayerSelection.SelectedIndex >= 0) &&
                (comboGridNumberSelection.SelectedIndex >= 0) &&
                (comboPageSize.SelectedIndex >= 0))
            {
                PrintStandardMapButton.IsEnabled = true;
            }
            else
            {
                PrintStandardMapButton.IsEnabled = false;
            }
        }

        #region Common method
        private static string PrettyUnits(ESRI.ArcGIS.Client.Actions.AreaUnit unit)
        {
            string display = string.Empty;

            // (Alt+0178) is the ASCII for superscript 2

            switch (unit)
            {
                case ESRI.ArcGIS.Client.Actions.AreaUnit.SquareMiles:
                    display = "mi²";
                    break;
                case ESRI.ArcGIS.Client.Actions.AreaUnit.SquareKilometers:
                    display = "km²";
                    break;
                case ESRI.ArcGIS.Client.Actions.AreaUnit.SquareFeet:
                    display = "ft²";
                    break;
                case ESRI.ArcGIS.Client.Actions.AreaUnit.SquareMeters:
                    display = "m²";
                    break;
                case ESRI.ArcGIS.Client.Actions.AreaUnit.Acres:
                    display = "a";
                    break;
                case ESRI.ArcGIS.Client.Actions.AreaUnit.Hectares:
                    display = "ha";
                    break;
            }

            return display;
        }
        private static double ConvertUnits(double item, ESRI.ArcGIS.Client.Actions.AreaUnit units)
        {
            if (double.IsNaN(item)) return double.NaN;
            double convertedItem = double.NaN;

            //  Assume from meters.
            switch (units)
            {
                case ESRI.ArcGIS.Client.Actions.AreaUnit.SquareMiles:
                    convertedItem = item * SqMetersToSqMiles;
                    break;
                case ESRI.ArcGIS.Client.Actions.AreaUnit.SquareFeet:
                    convertedItem = item * SqMetersToSqFeet;
                    break;
                case ESRI.ArcGIS.Client.Actions.AreaUnit.SquareKilometers:
                    convertedItem = item / 1000000;
                    break;
                case ESRI.ArcGIS.Client.Actions.AreaUnit.SquareMeters:
                    convertedItem = item;
                    break;
                case ESRI.ArcGIS.Client.Actions.AreaUnit.Hectares:
                    convertedItem = item * SqMetersToHectare;
                    break;
                case ESRI.ArcGIS.Client.Actions.AreaUnit.Acres:
                    convertedItem = item * SqMetersToAcre;
                    break;
                default:
                    break;
            }

            return convertedItem;
        }

        private double DistanceTo(MapPoint start, MapPoint end)
        {
            double x = Math.Abs(start.X - end.X);
            double y = Math.Abs(start.Y - end.Y);
            double dist = Math.Sqrt((x * x) + (y * y));

            return dist;
        }

        private double ConvertMetersToActiveUnits(double dist, ESRI.ArcGIS.Client.Actions.DistanceUnit DistanceUnits)
        {
            const double metersToMiles = 0.0006213700922;
            const double metersToFeet = 3.280839895;

            double displayMeasurement = dist;

            // Assume from meters.
            switch (DistanceUnits)
            {
                case ESRI.ArcGIS.Client.Actions.DistanceUnit.Miles:
                    displayMeasurement = dist * metersToMiles;
                    break;
                case ESRI.ArcGIS.Client.Actions.DistanceUnit.Kilometers:
                    displayMeasurement = dist / 1000;
                    break;
                case ESRI.ArcGIS.Client.Actions.DistanceUnit.Feet:
                    displayMeasurement = dist * metersToFeet;
                    break;
                case ESRI.ArcGIS.Client.Actions.DistanceUnit.Meters:
                    displayMeasurement = dist;
                    break;
            }

            return displayMeasurement;
        }

        private string PrettyUnits(ESRI.ArcGIS.Client.Actions.DistanceUnit unit)
        {
            string display = string.Empty;

            switch (unit)
            {
                case ESRI.ArcGIS.Client.Actions.DistanceUnit.Miles:
                    display = "mi";
                    break;
                case ESRI.ArcGIS.Client.Actions.DistanceUnit.Kilometers:
                    display = "km";
                    break;
                case ESRI.ArcGIS.Client.Actions.DistanceUnit.Feet:
                    display = "ft";
                    break;
                case ESRI.ArcGIS.Client.Actions.DistanceUnit.Meters:
                    display = "m";
                    break;
            }

            return display;
        }

        private double RoundToSignificantDigit(double value)
        {
            int SignificantDigits = 2;
            int roundDigits = (SignificantDigits - 2) >= 0 ? SignificantDigits - 2 : 0;

            if (value == 0) return 0;
            else if (value < .01)
            {
                roundDigits = SignificantDigits;
                while (roundDigits < 15 && Math.Round(value, ++roundDigits) == 0) ;
            }
            else if (value < 1) roundDigits = SignificantDigits;
            else if (value < 10) roundDigits = SignificantDigits;
            else if (value < 100) roundDigits = SignificantDigits - 1;

            double round = value >= 100 ? Math.Round(value) : Math.Round(value, roundDigits);

            return round;
        }

        static private Size GetTextSize(FontFamily fontFamily, int fontSize)
        {
            Size size = new Size(0, 0);

            TextBlock l = new TextBlock();
            l.FontFamily = fontFamily;
            l.FontSize = fontSize;
            //l.Text = text;
            size.Height = l.ActualHeight;
            size.Width = l.ActualWidth;

            return size;
        }

        public void EnableDisableRadioButtons()
        {
            try
            {
                double currentScale = Math.Round(_map.Resolution * 96);
                if (currentScale <= _scaleLimit)
                {
                    Rdpoly.IsEnabled = true;
                    RdGrid.IsEnabled = true;
                    
                    if (Rdcircuit.IsChecked == true)
                    {
                        if (PART_FilterAutoCompleteTextBlock.Text == "")
                        {
                            RdGrid.IsChecked = true;
                            comboGridNumberSelection.Width = 160;
                        }
                        else
                        {
                            comboGridNumberSelection.Width = .1;
                        }
                    }
                    else
                    {
                        RdGrid.IsChecked = true;
                        comboGridNumberSelection.Width = 160;
                    }
                }
                else
                {
                    Rdpoly.IsEnabled = false;
                    RdGrid.IsEnabled = false;
                    if (Rdpoly.IsChecked == true || RdGrid.IsChecked == true)
                    {
                        Rdcircuit.IsChecked = true;
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error("Standard Print-Map_ExtentChanged: " + ex.Message);
            }
        }
        #endregion

        public class StandardMapPrintGridList
        {

            public string Gridlist { set; get; }
        }
    }
}
