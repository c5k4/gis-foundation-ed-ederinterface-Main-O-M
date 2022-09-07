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
using ArcFMSilverlight.Controls.GenerationOnFeeder;
using System.Text;
using System.Reflection;
namespace ArcFMSilverlight
{


    public partial class FEEDERToolPage : UserControl
    {
        #region declarations
        //ENOS2SAP After Release -- Start
        public double total_gensize_Feeder = 0;
        public double total_gensize_SYNCH = 0;
        public double total_gensize_INVEXT = 0;
        public double total_gensize_INVINC = 0;
        public double total_gensize_INDCT = 0;
        public double total_gensize_MIXD = 0;
        public double total_nameplate = 0;
        //ENOS2SAP After Release -- End
        //ENOS Tariff Change -- Start
        public double total_nameplate_SYNCH = 0;
        public double total_nameplate_INVEXT = 0;
        public double total_nameplate_INVINC = 0;
        public double total_nameplate_INDCT = 0;
        public double total_nameplate_MIXD = 0;
        public double total_nameplate_StandByGen = 0;
        //ENOS Tariff Change -- End

        FloatableWindow _fw = null;
        private MapTools _mapTools;
        private Map _map;
        private string endpointaddress = string.Empty;
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
        private string _selectedCircuitId;
        private string _feederName;

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
        private string _deviceSettingsURL;
        string _WCFService_URL = "";
        private ENOSFEEDERTool _enosFEEDERTool;
        List<GenFeederDetails> GenDetailsinfo = new List<GenFeederDetails>();
        List<DataToExportDetails> DataToExportList = new List<DataToExportDetails>(); //ENOS Tariff Change
        private GetGenOnFeeder _GetGenOnFeeder = null;
        public bool isCalledFromTool = false;
        #endregion
        public FEEDERToolPage(Map map, MapTools mapTools, string deviceSettingURL, ENOSFEEDERTool ENOSFEEDERTool)
        {
            InitializeComponent();
            _map = map;
            _enosFEEDERTool = ENOSFEEDERTool;
            //  ReadTemplatesConfig();            
            LoadConfiguration();
            LoadLayerConfiguration_jetwcf();
            _deviceSettingsURL = deviceSettingURL;
            objPonsServiceCall = new Ponsservicelayercall();
            string prefixUrl = objPonsServiceCall.GetPrefixUrl();
            endpointaddress = prefixUrl + _WCFService_URL;

        }
        private void LoadLayerConfiguration_jetwcf()
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

        #region Config
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

                        switch (element.Name.LocalName)
                        {
                            case "ENOSFeeder":


                                //if (element.Name.LocalName == "ENOSFeeder")
                                //{
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

                                    }

                                }

                                //}
                                //ReadDivisionCodes(element);
                                break;
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
            query.Where = "SUBSTATIONNAME like '" + substationLike + "%'"; //and LENGTH(circuitid) = 9 ";
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
            PART_FeederAutoCompleteTextBlock.ItemsSource = circuitIdList;
            PART_FeederAutoCompleteTextBlock.PopulateComplete();
        }

        private void PART_AutoCompleteTextBlock_OnKeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Escape:
                    e.Handled = true;
                    //PARTFeeder_CircuitPopup.IsOpen = false;
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
                _selectedCircuitId = _circuitIDDictionary[e.AddedItems[0].ToString()].Trim();
                _feederName = e.AddedItems[0].ToString();
                ResetFilterGraphics();
                //ZoomToCircuit(_selectedCircuitId, _feederName);
            }
        }
        #endregion
        #region ZoomToCircuits code
        //private void InitializeZoomToCircuit()
        //{

        //    _graphicsLayer.ID = "Stand_CircuitGraphics";
        //    _graphicsLayerPoint.ID = "Stand_CircuitPointGraphic";
        //   // _scaleLimit = 5000;
        //}

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
            //   ConfigUtility.UpdateStatusBarText("Zooming to Circuit [ " + circuitName + " ]...");
            BusyIndicator.IsBusy = true;
            BusyIndicator.BusyContent = "Fetching Data...";


            if (validateFeederId(PART_FeederAutoCompleteTextBlock.Text.Trim()))
            {
                _circuitIdSelected = PART_FeederAutoCompleteTextBlock.Text.Trim();
                _selectedCircuitId = PART_FeederAutoCompleteTextBlock.Text.Trim();
            }
            else
            {
                _circuitIdSelected = circuitId.Trim();
                _selectedCircuitId = circuitId.Trim();
            }

            ConfigUtility.UpdateStatusBarText("Zooming to Circuit [ " + _circuitIdSelected + " ]...");
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
                _map.ZoomTo(_selectedEnvelope.Expand(EXPAND_FACTOR));
                //BindgridNumber(_selectedEnvelope.Extent);
            }
            else
            {
                MessageBox.Show("No features found for this CircuitID");
            }

            if (_lineGraphics.Count > 0)
            {
                CreateCircuitGraphics();
            }
            //  ConfigUtility.UpdateStatusBarText("");
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
                    Text = PART_FeederAutoCompleteTextBlock.Text
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





        public void GetFloatingWindowHeight(FloatableWindow fw)
        {
            _fw = fw;
        }

        //ENOS2SAP After Release --Start

        //ENOS Tariff Change- Start

        private void ExportToExcel_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog objSFD = new SaveFileDialog() { DefaultExt = "csv", Filter = "Excel XML (*.xml)|*.xml", FilterIndex = 1 };

            if (objSFD.ShowDialog() == true)
            {
                StreamWriter sw = new StreamWriter(objSFD.OpenFile());
                sw.WriteLine("<?xml version=\"1.0\" " + "encoding=\"utf-8\"?>");
                sw.WriteLine("<?mso-application progid" + "=\"Excel.Sheet\"?>");
                sw.WriteLine("<Workbook xmlns=\"urn:" + "schemas-microsoft-com:office:spreadsheet\">");
                sw.WriteLine("<DocumentProperties " + "xmlns=\"urn:schemas-microsoft-com:" + "office:office\">");
                sw.WriteLine("<Created>" + DateTime.Now.ToLocalTime().ToLongDateString() + "</Created>");
                sw.WriteLine("</DocumentProperties>");
                sw.WriteLine("<Styles>");
                sw.WriteLine("<Style ss:ID=\"head\" ss:Name=\"Normal\"><Alignment ss:Vertical=\"Bottom\"/><Borders/><Font ss:Underline=\"Single\" ss:Bold=\"1\"/><Interior/><NumberFormat/><Protection/></Style>");
                sw.WriteLine("<Style ss:ID=\"title\"><Alignment ss:Horizontal=\"Center\"/><Borders/><Font ss:Size=\"13\" ss:Bold=\"1\"/><Interior/><NumberFormat/><Protection/></Style>");
                sw.WriteLine("<Style ss:ID=\"date\"><Alignment ss:Horizontal=\"Right\"/><Borders/><Font/><Interior/><NumberFormat/><Protection/></Style>");
                sw.WriteLine("<Style ss:ID=\"rowhead\"><Alignment ss:Horizontal=\"Left\"/><Borders/><Font ss:Color=\"#ffffff\" ss:Bold=\"1\"/><Interior ss:Color=\"#4f81BD\" ss:Pattern=\"Solid\"/><NumberFormat/><Protection/></Style>");
                sw.WriteLine("<Style ss:ID=\"data\"><Borders><Border ss:Color=\"#4f81BD\" ss:Position=\"Left\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\" />" +
                         "<Border ss:Color=\"#4f81BD\" ss:Position=\"Right\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\" />" +
                          "<Border ss:Color=\"#4f81BD\" ss:Position=\"Top\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\" />" +
                         "<Border ss:Color=\"#4f81BD\" ss:Position=\"Bottom\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\" /></Borders></Style>");
                sw.WriteLine("</Styles>");
                sw.WriteLine("<Worksheet ss:Name=\"Customer Information\" " + "xmlns:ss=\"urn:schemas-microsoft-com:office:spreadsheet\">");
                sw.WriteLine("<Table>");
                sw.WriteLine("<Column ss:Index=\"1\" ss:AutoFitWidth=\"0\" ss:Width=\"30\"/>");
                sw.WriteLine("<Column ss:Index=\"2\" ss:AutoFitWidth=\"0\" ss:Width=\"100\"/>");
                sw.WriteLine("<Column ss:Index=\"3\" ss:AutoFitWidth=\"0\" ss:Width=\"72\"/>");
                sw.WriteLine("<Column ss:Index=\"4\" ss:AutoFitWidth=\"0\" ss:Width=\"100\"/>");
                sw.WriteLine("<Column ss:Index=\"5\" ss:AutoFitWidth=\"0\" ss:Width=\"100\"/>");
                //sw.WriteLine("<Row>" + String.Format("<Cell ss:StyleID=\"head\" ss:MergeAcross=\"2\"><Data ss:Type=\"String" + "\">{0}</Data></Cell>", "Interruption Date:") + String.Format("<Cell ss:MergeAcross=\"1\"><Data ss:Type=\"String" + "\">{0}</Data></Cell>", strDateTime) + "</Row>");
                //sw.WriteLine("<Row>" + String.Format("<Cell ss:StyleID=\"head\" ss:MergeAcross=\"2\"><Data ss:Type=\"String" + "\">{0}</Data></Cell>", "Interruption Area:") + String.Format("<Cell><Data ss:Type=\"String" + "\">{0}</Data></Cell>", strDecLoc) + "</Row>");
                //sw.WriteLine("<Row>" + String.Format("<Cell></Cell><Cell></Cell><Cell></Cell><Cell></Cell><Cell ss:StyleID=\"date\"><Data ss:Type=\"String" + "\">{0}</Data></Cell>", DateTime.Now.ToLocalTime().ToLongDateString()) + "</Row>");
                sw.WriteLine("<Row></Row>");
                //sw.WriteLine("<Row>" + String.Format("<Cell ss:StyleID=\"title\" ss:MergeAcross=\"4\"><Data ss:Type=\"String" + "\">{0}</Data></Cell>", "Customer Notification Wizard") + "</Row>");
                sw.WriteLine("<Row></Row>");
                sw.Write(strBuilder.ToString());
                sw.WriteLine(AddGenSizeRowtoExcel("Total NP Rating (kW)", Math.Round(total_nameplate, 3).ToString()));
                //sw.WriteLine(AddGenSizeRowtoExcel("Total Generation on Feeder (kW)", total_gensize_Feeder.ToString()));
                sw.WriteLine(AddGenSizeRowtoExcel("Total Effective Rating (kW)", total_gensize_Feeder.ToString())); //ENOS Tariff Name Change 
                sw.WriteLine(AddGenSizeRowtoExcel("Inverter Based NP (kW)", total_nameplate_INVEXT.ToString())); //ENOS Tariff Name Change
                sw.WriteLine(AddGenSizeRowtoExcel("Synchronous (kW)", total_nameplate_SYNCH.ToString()));
                sw.WriteLine(AddGenSizeRowtoExcel("Induction (kW)", total_nameplate_INDCT.ToString()));
                // sw.WriteLine(AddGenSizeRowtoExcel("Mixed (kW)", total_nameplate_MIXD.ToString()));
                sw.WriteLine(AddGenSizeRowtoExcel("Standby Generation (kW)", total_nameplate_StandByGen.ToString()));//ENOS Tariff Change
                sw.WriteLine("</Table>");
                sw.WriteLine("</Worksheet>");
                sw.WriteLine("</Workbook>");
                sw.Close();
            }

        }
        private void PrepareDataToExport()
        {
            string strValue = "";
            StringBuilder strBuilder = new StringBuilder();

            if (genOnFeederGrid.ItemsSource == null)
                return;

            List<string> lstFields = new List<string>();
            //lstFields.Add(FormatHeaderField("S.No"));
            if (genOnFeederGrid.HeadersVisibility == DataGridHeadersVisibility.Column || genOnFeederGrid.HeadersVisibility == DataGridHeadersVisibility.All)
            {

                foreach (DataGridColumn dgcol in genOnFeederGrid.Columns)
                {
                    if (dgcol.Header.ToString() != "")
                        lstFields.Add(FormatHeaderField(dgcol.Header.ToString()));
                }

                BuildStringOfRow(strBuilder, lstFields);

            }
            foreach (object data in genOnFeederGrid.ItemsSource)
            {
                lstFields.Clear();
                foreach (DataGridColumn col in genOnFeederGrid.Columns)
                {

                    System.Windows.Data.Binding objBinding = null;
                    if (col is DataGridBoundColumn)
                        objBinding = (col as DataGridBoundColumn).Binding;
                    if (col is DataGridTemplateColumn)
                    {
                        //This is a template column...
                        //    let us see the underlying dependency object
                        DependencyObject objDO =
                            (col as DataGridTemplateColumn).CellTemplate.LoadContent();
                        FrameworkElement oFE = (FrameworkElement)objDO;
                        System.Reflection.FieldInfo oFI = oFE.GetType().GetField("TextProperty");
                        if (oFI != null)
                        {
                            if (oFI.GetValue(null) != null)
                            {
                                if (oFE.GetBindingExpression((DependencyProperty)oFI.GetValue(null)) != null)
                                    objBinding = oFE.GetBindingExpression((DependencyProperty)oFI.GetValue(null)).ParentBinding;
                            }
                        }
                    }
                    if (objBinding != null)
                    {
                        if (objBinding.Path.Path != "")
                        {

                            PropertyInfo pi = data.GetType().GetProperty(objBinding.Path.Path);
                            if (pi != null)
                            {
                                strValue += "'" + ((ArcFMSilverlight.GenFeederDetails)(data)).ProjectReference.ToString() + "',";
                            }
                        }
                    }
                }
            }

            strValue = strValue.Remove(strValue.Length - 1);


            EndpointAddress endPoint = new EndpointAddress(endpointaddress);
            BasicHttpBinding httpbinding = new BasicHttpBinding();
            httpbinding.MaxBufferSize = 2147483647;
            httpbinding.MaxReceivedMessageSize = 2147483647;
            httpbinding.TransferMode = TransferMode.Buffered;
            httpbinding.SendTimeout = TimeSpan.FromSeconds(300);
            IServicePons cusservice = new ChannelFactory<IServicePons>(httpbinding, endPoint).CreateChannel();

            try
            {

                cusservice.BegingetDataToExportFromSettings(strValue,
                 delegate(IAsyncResult result)
                 {
                     List<DataToExportDetails> CombindjobList = ((IServicePons)result.AsyncState).EndgetDataToExportFromSettings(result);
                     this.Dispatcher.BeginInvoke(
                     delegate()
                     {
                         ClientDataExportjobResultHandle_Async(CombindjobList);
                     }
                     );
                 }, cusservice);
            }
            catch { }
        }

        StringBuilder strBuilder = new StringBuilder();
        private void ClientDataExportjobResultHandle_Async(List<DataToExportDetails> objCustomerResult)
        {

            try
            {
                //ENOS Tariff Change -- Start  
                total_gensize_Feeder = 0;
                total_nameplate_SYNCH = 0;
                total_nameplate_INVEXT = 0;
                total_nameplate_INVINC = 0;
                total_nameplate_INDCT = 0;
                total_nameplate_MIXD = 0;
                total_nameplate_StandByGen = 0;
                total_nameplate = 0;
                //ENOS Tariff Change -- End

                //List<DataToExportDetails> DataToExportList = new List<DataToExportDetails>();
                DataToExportList.Clear();
                for (int i = 0; i < DataToExportDetailsResult.Count; i++)
                {

                    foreach (DataToExportDetails row in objCustomerResult)
                    {
                        DataToExportDetails objDataToExport = new DataToExportDetails();
                        if (row.ProjectReference.ToString() == DataToExportDetailsResult[i].ProjectReference.ToString())
                        {
                            objDataToExport.Address = DataToExportDetailsResult[i].Address;
                            objDataToExport.METERNUMBER = DataToExportDetailsResult[i].METERNUMBER;
                            objDataToExport.SPID = DataToExportDetailsResult[i].SPID;
                            objDataToExport.CGC12 = DataToExportDetailsResult[i].CGC12;
                            //objDataToExport.GenSize = DataToExportDetailsResult[i].GenSize;
                            objDataToExport.GenSize = row.GenSize;
                            //objDataToExport.Nameplate = DataToExportDetailsResult[i].Nameplate;
                            objDataToExport.Nameplate = row.Nameplate;
                            objDataToExport.ProjectReference = DataToExportDetailsResult[i].ProjectReference;
                            objDataToExport.FEEDERNUM = DataToExportDetailsResult[i].FEEDERNUM;
                            objDataToExport.GenType = row.GenType;
                            objDataToExport.TechType = row.TechType.ToString();
                            objDataToExport.EquipmentType = row.EquipmentType.ToString();
                            objDataToExport.SAPEquipmentID = row.SAPEquipmentID.ToString();
                            objDataToExport.EquipSAPEquipmentID = row.EquipSAPEquipmentID.ToString();
                            objDataToExport.Derated = row.Derated.ToString();
                            //objDataToExport.LimitedExport = row.LimitedExport.ToString();
                            objDataToExport.LimitedExport = DataToExportDetailsResult[i].LimitedExport;
                            objDataToExport.ProgramType = row.ProgramType.ToString();
                            objDataToExport.ExportToGrid = row.ExportToGrid.ToString();
                            objDataToExport.StandByGen = row.StandByGen.ToString();
                            if (objDataToExport.StandByGen == "Y")
                            {
                                total_nameplate_StandByGen += Convert.ToDouble(objDataToExport.Nameplate);
                            }
                            else //(objDataToExport.StandByGen != "Y")
                            {
                                //{
                                //    total_nameplate_StandByGen += objDataToExport.Nameplate;
                                //}
                                if (objDataToExport.GenSize != 0)
                                {
                                    total_gensize_Feeder += Convert.ToDouble(objDataToExport.GenSize);
                                }

                                if (objDataToExport.Nameplate != 0)
                                {
                                    total_nameplate += Convert.ToDouble(objDataToExport.Nameplate);

                                    if (objDataToExport.GenType == "INVEXT" || objDataToExport.GenType == "INVINC")
                                    {
                                        total_nameplate_INVEXT += Convert.ToDouble(objDataToExport.Nameplate);
                                    }
                                    else if (objDataToExport.GenType == "SYNCH")
                                    {
                                        total_nameplate_SYNCH += Convert.ToDouble(objDataToExport.Nameplate);
                                    }
                                    else if (objDataToExport.GenType == "INDCT")
                                    {
                                        total_nameplate_INDCT += Convert.ToDouble(objDataToExport.Nameplate);
                                    }
                                    //else if (objDataToExport.GenType == "MIXD")
                                    //{
                                    //    total_nameplate_MIXD += Convert.ToDouble(objDataToExport.Nameplate);
                                    //}
                                }
                            }
                            DataToExportList.Add(objDataToExport);
                        }

                        //if (objDataToExport != null)
                        //    DataToExportList.Add(objDataToExport);
                    }
                }

                if (GenDetailsinfo.Count > 0)
                {
                    genOnFeederGrid.ItemsSource = null;
                    genOnFeederGrid.ItemsSource = GenDetailsinfo;

                    try
                    {
                        txtSumGenSize_Feeder.Text = "Total NP Rating (kW): " + Math.Round(total_nameplate, 3) + "\n";
                        //txtSumGenSize_Feeder.Text += "Total Generation on Feeder (kW): " + Math.Round(total_gensize_Feeder, 3) + "\n";
                        txtSumGenSize_Feeder.Text += "Total Effective Rating (kW): " + Math.Round(total_gensize_Feeder, 3) + "\n"; //ENOS Tariff change 
                        if (total_nameplate_INVEXT != 0)
                        {
                            txtSumGenSize_Feeder.Text += "Inverter Based NP (kW): " + Math.Round(total_nameplate_INVEXT, 3) + "\n"; //ENOS Tariff NP added in name 
                        }
                        if (total_nameplate_SYNCH != 0)
                        {
                            txtSumGenSize_Feeder.Text += "Synchronous (kW): " + Math.Round(total_nameplate_SYNCH, 3) + "\n";
                        }
                        if (total_nameplate_INDCT != 0)
                        {
                            txtSumGenSize_Feeder.Text += "Induction (kW): " + Math.Round(total_nameplate_INDCT, 3) + "\n";
                        }
                        if (total_nameplate_MIXD != 0)
                        {
                            //txtSumGenSize_Feeder.Text += "Mixed (kW): " + Math.Round(total_nameplate_MIXD, 3) + "\n";
                        }
                        if (total_nameplate_StandByGen != 0)
                        {
                            txtSumGenSize_Feeder.Text += "Standby Generation (kW):" + Math.Round(total_nameplate_StandByGen, 3);
                        }
                    }
                    catch (Exception ex)
                    {
                        logger.Error("Error while displaying gensize: " + ex.Message);
                    }

                }

                List<string> lstFields = new List<string>();

                lstFields.Add(FormatHeaderField("Service Point ID"));
                lstFields.Add(FormatHeaderField("Meter Number"));
                lstFields.Add(FormatHeaderField("CGC12"));
                lstFields.Add(FormatHeaderField("Eff.Rating(kW)"));
                lstFields.Add(FormatHeaderField("NP Rating(kW)"));
                lstFields.Add(FormatHeaderField("Project Reference"));
                lstFields.Add(FormatHeaderField("Feeder Number"));
                lstFields.Add(FormatHeaderField("Gen Type"));
                lstFields.Add(FormatHeaderField("Tech Type"));
                lstFields.Add(FormatHeaderField("Equipment Type"));
                lstFields.Add(FormatHeaderField("Generator SAPID"));
                lstFields.Add(FormatHeaderField("Gen Equip SAPID"));
                lstFields.Add(FormatHeaderField("Derated"));
                lstFields.Add(FormatHeaderField("Limited Export"));
                lstFields.Add(FormatHeaderField("Program Type"));
                lstFields.Add(FormatHeaderField("Export To Grid"));
                lstFields.Add(FormatHeaderField("StandByGen"));
                lstFields.Add(FormatHeaderField("Address"));

                //foreach (PropertyInfo p in typeof(DataToExportDetails).GetProperties())
                //{
                //    lstFields.Add(FormatHeaderField(p.Name));

                //}
                BuildStringOfRow(strBuilder, lstFields);
                foreach (DataToExportDetails data in DataToExportList)
                {
                    lstFields.Clear();
                    lstFields.Add(FormatField(data.SPID));

                    lstFields.Add(FormatField(data.METERNUMBER));

                    lstFields.Add(FormatField(data.CGC12));

                    lstFields.Add(FormatField(data.GenSize.ToString()));

                    lstFields.Add(FormatField(data.Nameplate.ToString()));

                    lstFields.Add(FormatField(data.ProjectReference));

                    lstFields.Add(FormatField(data.FEEDERNUM));

                    lstFields.Add(FormatField(data.GenType));

                    lstFields.Add(FormatField(data.TechType));

                    lstFields.Add(FormatField(data.EquipmentType));

                    lstFields.Add(FormatField(data.SAPEquipmentID.ToString()));

                    lstFields.Add(FormatField(data.EquipSAPEquipmentID.ToString()));

                    lstFields.Add(FormatField(data.Derated));

                    lstFields.Add(FormatField(data.LimitedExport));

                    lstFields.Add(FormatField(data.ProgramType));

                    lstFields.Add(FormatField(data.ExportToGrid));

                    lstFields.Add(FormatField(data.StandByGen));

                    lstFields.Add(FormatField(data.Address));
                    BuildStringOfRow(strBuilder, lstFields);

                }
            }

            catch (Exception ex)
            {
                logger.Error("Error while Exporting Data: " + ex.Message);
            }
        }
        //ENOS Tariff Change- End

        private string AddGenSizeRowtoExcel(string genType, string sum)
        {
            if (!string.IsNullOrEmpty(sum) && sum != "0")
            {
                string rowtoAdd = null;
                rowtoAdd = "<Row><Cell ss:StyleID=\"rowhead\"><Data ss:Type=\"String\">" + genType + "</Data></Cell>";
                rowtoAdd += "<Cell ss:StyleID=\"rowhead\"><Data ss:Type=\"String\"></Data></Cell>";
                rowtoAdd += "<Cell ss:StyleID=\"rowhead\"><Data ss:Type=\"String\"></Data></Cell>";
                rowtoAdd += "<Cell ss:StyleID=\"rowhead\"><Data ss:Type=\"String\">" + sum + "</Data></Cell>";
                rowtoAdd += "<Cell ss:StyleID=\"rowhead\"><Data ss:Type=\"String\"></Data></Cell>";
                rowtoAdd += "<Cell ss:StyleID=\"rowhead\"><Data ss:Type=\"String\"></Data></Cell>";
                rowtoAdd += "<Cell ss:StyleID=\"rowhead\"><Data ss:Type=\"String\"></Data></Cell>";
                rowtoAdd += "<Cell ss:StyleID=\"rowhead\"><Data ss:Type=\"String\"></Data></Cell></Row>";
                return rowtoAdd;
            }
            else
            {
                return null;
            }
        }
        //ENOS2SAP After Release --End

        private static void BuildStringOfRow(StringBuilder strBuilder, List<string> lstFields)
        {
            strBuilder.AppendLine("<Row>");
            strBuilder.AppendLine(String.Join("\r\n", lstFields.ToArray()));
            strBuilder.AppendLine("</Row>");
        }

        private static void BuildStringOfRowHeader(StringBuilder strBuilder, string strHeading)
        {
            strBuilder.AppendLine("<Row>");
            strBuilder.AppendLine("<Cell ss:MergeAcross=\"8\"><Data ss:Type=\"String" + "\">" + strHeading + "</Data></Cell>");
            strBuilder.AppendLine("</Row>");
        }

        private static string FormatField(string data)
        {
            return String.Format("<Cell ss:StyleID=\"data\"><Data ss:Type=\"String" + "\">{0}</Data></Cell>", data);
        }

        private static string FormatHeaderField(string data)
        {
            return String.Format("<Cell ss:StyleID=\"rowhead\"><Data ss:Type=\"String" + "\">{0}</Data></Cell>", data);
        }
        //private void CancelButton_Click(object sender, RoutedEventArgs e)
        //{        

        //    _fw.Visibility = Visibility.Collapsed;          
        //    //ClearGraphics();          
        //    PART_FeederAutoCompleteTextBlock.Text = "";
        //    genOnFeederGrid.ClearValue(DataGrid.ItemsSourceProperty);
        //    GenOnFeederStackPanel.Visibility = System.Windows.Visibility.Collapsed;

        //}
        public string _circuitId = string.Empty;
        private void SearchFeederButton_Click(object sender, RoutedEventArgs e)
        {

            if (PART_FeederAutoCompleteTextBlock.Text != "" && PART_FeederAutoCompleteTextBlock.Text != null)
            {
                strBuilder.Clear(); //ENOS Tariff Changes   
                DataToExportDetailsResult.Clear(); //ENOS Tariff Change
                _circuitId = _selectedCircuitId;
                SearchFeederButton.IsEnabled = false;
                ZoomToCircuit(_selectedCircuitId, _feederName);
                GetGenOnFeeder getGenOnFeederObj = new GetGenOnFeeder();
                ConfigUtility.UpdateStatusBarText("Getting Generation Data...");
                GendatafromFeederdata(_selectedCircuitId);
                getGenOnFeederObj.getGenOnFeederMethod(this, _selectedCircuitId, true);
            }
            else
            {
                SearchFeederButton.IsEnabled = true;
                MessageBox.Show("Please Select Feeder Name.");
            }
        }
        //call wcf 
        public void GendatafromFeederdata(string FeederID)
        {

            GenDetailsinfo.Clear();
            EndpointAddress endPoint = new EndpointAddress(endpointaddress);
            BasicHttpBinding httpbinding = new BasicHttpBinding();
            httpbinding.MaxBufferSize = 2147483647;
            httpbinding.MaxReceivedMessageSize = 2147483647;
            httpbinding.TransferMode = TransferMode.Buffered;
            httpbinding.SendTimeout = TimeSpan.FromSeconds(300);
            IServicePons cusservice = new ChannelFactory<IServicePons>(httpbinding, endPoint).CreateChannel();

            try
            {

                cusservice.BeginGetGenFeederDetails(FeederID,
                 delegate(IAsyncResult result)
                 {
                     List<GenFeederDetails> CombindjobList = ((IServicePons)result.AsyncState).EndGetGenFeederDetails(result);
                     this.Dispatcher.BeginInvoke(
                     delegate()
                     {
                         ClientJetjobResultHandle_Async(CombindjobList);
                     }
                     );
                 }, cusservice);
            }
            catch { }


        }


        List<DataToExportDetails> DataToExportDetailsResult = new List<DataToExportDetails>(); //ENOS Tariff Change

        private void ClientJetjobResultHandle_Async(List<GenFeederDetails> objCustomerResult)
        {

            GenDetailsinfo.Clear();
            //ENOS2SAP After Release -- Start 
            total_gensize_Feeder = 0;
            total_gensize_SYNCH = 0;
            total_gensize_INVEXT = 0;
            total_gensize_INVINC = 0;
            total_gensize_INDCT = 0;
            total_gensize_MIXD = 0;
            total_nameplate = 0;
            //ENOS2SAP After Release -- End
            //ENOS Tariff Change -- Start      
            total_nameplate_SYNCH = 0;
            total_nameplate_INVEXT = 0;
            total_nameplate_INVINC = 0;
            total_nameplate_INDCT = 0;
            total_nameplate_MIXD = 0;
            total_nameplate_StandByGen = 0;
            //ENOS Tariff Change -- End


            /****************************ENOS2SAP PhaseIV End****************************/
            foreach (GenFeederDetails tc in objCustomerResult)
            {
                //ENOS2SAP After Release -- Start
                try
                {
                    //ENOS Tariff Change- Start
                    DataToExportDetails objDataToExport = new DataToExportDetails();
                    objDataToExport.Address = tc.Address;
                    objDataToExport.METERNUMBER = tc.METERNUMBER;
                    objDataToExport.SPID = tc.SPID;
                    objDataToExport.CGC12 = tc.CGC12;
                    objDataToExport.GenSize = tc.GenSize;
                    objDataToExport.Nameplate = tc.Nameplate;
                    objDataToExport.ProjectReference = tc.ProjectReference;
                    objDataToExport.FEEDERNUM = tc.FEEDERNUM;
                    objDataToExport.LimitedExport = tc.LimitedExport;
                    if (objDataToExport != null)
                        DataToExportDetailsResult.Add(objDataToExport);
                    //ENOS Tariff Change- End

                    GenDetailsinfo.Add(tc);

                    //if (tc.GenSize != 0)
                    //{
                    //    total_gensize_Feeder += Convert.ToDouble(tc.GenSize);
                    //}

                    //if(tc.Nameplate != 0)
                    //{
                    //    total_nameplate += Convert.ToDouble(tc.Nameplate);

                    //    if (tc.GenType == "INVEXT" || tc.GenType == "INVINC")
                    //    {
                    //        total_nameplate_INVEXT += Convert.ToDouble(tc.Nameplate);
                    //    }
                    //    else if (tc.GenType == "SYNCH")
                    //    {
                    //        total_nameplate_SYNCH += Convert.ToDouble(tc.Nameplate);
                    //    }
                    //    else if (tc.GenType == "INDCT")
                    //    {
                    //        total_nameplate_INDCT += Convert.ToDouble(tc.Nameplate);
                    //    }
                    //    else if (tc.GenType == "MIXD")
                    //    {
                    //        total_nameplate_MIXD += Convert.ToDouble(tc.Nameplate);
                    //    }
                    //}
                    //if (tc.Nameplate != 0)
                    //{
                    //    total_nameplate += Convert.ToDouble(tc.Nameplate);
                    //}
                }
                catch (Exception ex)
                {
                    logger.Error("Error while calculating gensize: " + ex.Message);
                }
                //ENOS2SAP After Release -- End
            }


            if (GenDetailsinfo.Count > 0)
            {
                genOnFeederGrid.ItemsSource = null;
                genOnFeederGrid.ItemsSource = GenDetailsinfo;

                //ENOS2SAP After Release ---Start
                try
                {
                    //txtSumGenSize_Feeder.Text = "Total NP Rating (kW): " + Math.Round(total_nameplate, 3) + "\n";  
                    ////txtSumGenSize_Feeder.Text += "Total Generation on Feeder (kW): " + Math.Round(total_gensize_Feeder, 3) + "\n";
                    //txtSumGenSize_Feeder.Text += "Total Effective Rating (kW): " + Math.Round(total_gensize_Feeder, 3) + "\n"; //ENOS Tariff change 
                    //if (total_nameplate_INVEXT != 0)
                    //{
                    //    txtSumGenSize_Feeder.Text += "Inverter Based NP (kW): " + Math.Round(total_nameplate_INVEXT, 3) + "\n"; //ENOS Tariff NP added in name 
                    //}
                    //if (total_nameplate_SYNCH != 0)
                    //{
                    //    txtSumGenSize_Feeder.Text += "Synchronous (kW): " + Math.Round(total_nameplate_SYNCH, 3) + "\n";
                    //}
                    //if (total_nameplate_INDCT != 0)
                    //{
                    //    txtSumGenSize_Feeder.Text += "Induction (kW): " + Math.Round(total_nameplate_INDCT, 3) + "\n";
                    //}
                    //if (total_nameplate_MIXD != 0)
                    //{
                    //    txtSumGenSize_Feeder.Text += "Mixed (kW): " + Math.Round(total_nameplate_MIXD, 3) + "\n";
                    //}

                    //Showing Feeder Id - ENOS Tariff Change
                    txtFeederId.Text = "Feeder Number: " + _circuitId.Trim();
                }
                catch (Exception ex)
                {
                    logger.Error("Error while displaying gensize: " + ex.Message);
                }
                //ENOS2SAP After Release ---End
                //************ENOSChangeFixes Start**************
                if (isCalledFromTool == true)
                {
                    // _enosFEEDERTool = new ENOSFEEDERTool(GenFeederToggleButton, _map, string geometryService, Grid mapArea, MapTools mapTools, string DeviceSettingUrl);
                    _enosFEEDERTool.OpenDialog(_circuitId);
                    showDataGrid();
                    SearchFeederButton.IsEnabled = true;
                }
                else
                {
                    showDataGrid();

                    SearchFeederButton.IsEnabled = true;
                }
                PrepareDataToExport(); //ENOS Tariff Change
            }
            else
            {
                SearchFeederButton.IsEnabled = true;
                ClearGrid();
                MessageBox.Show("No Generations are attached to selected data.", "No Generations Found", MessageBoxButton.OK);
            }

        }

        private bool validateFeederId(string circutId)
        {
            foreach (char c in circutId)
            {
                if (c < '0' || c > '9')
                    return false;
            }

            return true;
        }

        public void ClearGrid()
        {
            ConfigUtility.UpdateStatusBarText("");
            GenOnFeederStackPanel.Visibility = System.Windows.Visibility.Collapsed;
            SettingsButton.Visibility = System.Windows.Visibility.Collapsed;
            ExportToExcel_GenOnFeeder.Visibility = System.Windows.Visibility.Collapsed;
            //_fw.Height = 360;
            FeederToolBorder.Height = 100;
            //ENOSChangeFixes
            // _selectedCircuitId = string.Empty; 
            _feederName = string.Empty;
            BusyIndicator.IsBusy = false;
            BusyIndicator.BusyContent = "";
            genOnFeederGrid.ItemsSource = null;
            GenDetailsinfo.Clear();
            DataToExportList.Clear(); //ENOS Tariff Change
            //ENOS2SAP After Release ---Start
            txtSumGenSize_Feeder.Text = "";
            //ENOS2SAP After Release ---End
            //ENOS Tariff Change-Start
            txtFeederId.Text = "";
            //ENOS Tariff Change-Start
        }

        public void showData(bool isCalledFromTool)
        {
            //_genOnFeederGridData.Clear(); //ENOSChangeFixes

            //if (GenDetailsinfo.Count > 0)
            //{

            //    genOnFeederGrid.ItemsSource = GenDetailsinfo;
            //    //************ENOSChangeFixes Start**************
            //    showDataGrid();
            //}
            //else {
            //    ClearGrid();
            //    MessageBox.Show("No Generations are attached to selected data.", "No Generations Found", MessageBoxButton.OK);
            //}
            //************ENOSChangeFixes End**************
        }

        //************ENOSChangeFixes Start**************
        private void showDataGrid()
        {

            GenOnFeederStackPanel.Visibility = System.Windows.Visibility.Visible;
            SettingsButton.Visibility = System.Windows.Visibility.Visible;
            ExportToExcel_GenOnFeeder.Visibility = System.Windows.Visibility.Visible;
            //_fw.Height = 360;
            FeederToolBorder.Height = 380;
            ConfigUtility.UpdateStatusBarText("");
            _feederName = string.Empty;
            BusyIndicator.IsBusy = false;
            BusyIndicator.BusyContent = "";
        }
        //************ENOSChangeFixes End**************

        private void genOnFeederGrid_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.MouseRightButtonDown += new MouseButtonEventHandler(GenFeeder_MouseRightButtonDown);

            e.Row.MouseLeftButtonUp += new MouseButtonEventHandler(GenFeeder_MouseLeftButtonUp);

        }

        void GenFeeder_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            ConfigUtility.UpdateStatusBarText("Zooming to feature...");
            GenOnFeeder generationRow = ((sender) as DataGridRow).DataContext as GenOnFeeder;
            GetGenOnFeeder genFeederObj = new GetGenOnFeeder();
            //  genFeederObj.LocateServicePoint(generationRow.SPID, generationRow.SLGUID, generationRow.TransGUID, generationRow.PMGUID);
            genFeederObj.LocateServicePoint(((ArcFMSilverlight.GenFeederDetails)(((sender) as DataGridRow).DataContext)).SPID, ((ArcFMSilverlight.GenFeederDetails)(((sender) as DataGridRow).DataContext)).SLGUID, ((ArcFMSilverlight.GenFeederDetails)(((sender) as DataGridRow).DataContext)).TransGUID, ((ArcFMSilverlight.GenFeederDetails)(((sender) as DataGridRow).DataContext)).PMGUID);
        }

        private void GenFeeder_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            if ((sender) as DataGridRow != null)
            {
                genOnFeederGrid.SelectedItem = ((sender) as DataGridRow).DataContext;
            }
        }

        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            // this.DialogResult = null;
            if (genOnFeederGrid.SelectedItem != null)
            {
                GenOnFeeder selectedData = genOnFeederGrid.SelectedItem != null ? genOnFeederGrid.SelectedItem as GenOnFeeder : null;
                if (((ArcFMSilverlight.GenFeederDetails)(genOnFeederGrid.SelectedItem)).GENGLOBALID != "NA")
                {
                    //GenerationOnTransformer obj = new GenerationOnTransformer();
                    //obj.DeviceSettingURL = _deviceSettingsURL;
                    //obj.OpenSettings(selectedData.GENGLOBALID, "EDGIS.GENERATIONINFO");
                    //((ArcFMSilverlight.GenFeederDetails)(genOnFeederGrid.SelectedItem)).Address
                    OpenSettingsUI(((ArcFMSilverlight.GenFeederDetails)(genOnFeederGrid.SelectedItem)).GENGLOBALID);

                    //OpenSettingsUI(selectedData.GENGLOBALID);
                }
                else
                {
                    MessageBox.Show("No Generations are attached to this service point.", "No Generations Found", MessageBoxButton.OK);
                }
            }
        }
        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (genOnFeederGrid.SelectedItem != null)
            {

                GenOnFeeder selectedData = genOnFeederGrid.SelectedItem != null ? genOnFeederGrid.SelectedItem as GenOnFeeder : null;
                if (((ArcFMSilverlight.GenFeederDetails)(genOnFeederGrid.SelectedItem)).GENGLOBALID != "NA")
                {
                    //GenerationOnTransformer obj = new GenerationOnTransformer();
                    //obj.DeviceSettingURL = _deviceSettingsURL;
                    //obj.OpenSettings(selectedData.GENGLOBALID, "EDGIS.GENERATIONINFO");
                    //((ArcFMSilverlight.GenFeederDetails)(genOnFeederGrid.SelectedItem)).Address
                    OpenSettingsUI(((ArcFMSilverlight.GenFeederDetails)(genOnFeederGrid.SelectedItem)).GENGLOBALID);

                    //OpenSettingsUI(selectedData.GENGLOBALID);
                }
                else
                {
                    MessageBox.Show("No Generations are attached to this service point.", "No Generations Found", MessageBoxButton.OK);
                }
            }
        }



        private void OpenSettingsUI(string genGlobalID)
        {
            GenerationOnTransformer obj = new GenerationOnTransformer();
            obj.DeviceSettingURL = _deviceSettingsURL;
            obj.OpenSettings(genGlobalID, "EDGIS.GENERATIONINFO");
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


        #endregion

        public class StandardMapPrintGridList
        {

            public string Gridlist { set; get; }
        }
    }
}
