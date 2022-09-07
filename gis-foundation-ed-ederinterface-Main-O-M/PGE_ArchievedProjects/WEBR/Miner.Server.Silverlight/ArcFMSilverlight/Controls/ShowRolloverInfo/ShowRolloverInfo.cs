using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Xml.Linq;
using ArcFMSilverlight.Behaviors;
using ArcFMSilverlight.Butterfly;
using ArcFMSilverlight.Controls.Butterfly;
using ArcFMSilverlight.Controls.DeviceSettings;
using ArcFMSilverlight.Controls.OutageHistory;
using ESRI.ArcGIS.Client;
using ESRI.ArcGIS.Client.FeatureService;
using ESRI.ArcGIS.Client.FeatureService.Symbols;
using ESRI.ArcGIS.Client.Geometry;
using ESRI.ArcGIS.Client.Tasks;
using Miner.Server.Client.Toolkit;
//***********ENOS2EDGIS Start*******************
using System.Windows.Browser;
using System.Collections.ObjectModel;
using ArcFMSilverlight.Controls.Generation;
using Miner.Server.Client.Tasks;
//*************ENOS2EDGIS End******************


//ENOSChange Start
using ArcFMSilverlight.Controls.GenerationOnTransformer;
using ArcFMSilverlight.Controls.GenerationOnFeeder;

//ENOSChange End
namespace ArcFMSilverlight.Controls.ShowRolloverInfo
{
    public class ShowRolloverInfo 
    {
        public const string ROLLOVER_FEATURELAYER_PREFIX = "Rollover_";
        private IList<string> _featureLayerURLs = new List<string>();
        private IList<string> _polygonFeatureLayerURLs = new List<string>();
        private IList<string> _lineFeatureLayerURLs = new List<string>();
        private IList<string> _pointFeatureLayerURLs = new List<string>();
        private IDictionary<string, string> _subtypeLookupUrl = new Dictionary<string, string>();
        private Map _map;
        private SimpleFillSymbol _simpleFillSymbol = new SimpleFillSymbol();
        private SimpleLineSymbol _simpleLineSymbol = new SimpleLineSymbol();
        private SimpleMarkerSymbol _simpleMarkerSymbol = new SimpleMarkerSymbol();
        private SimpleRenderer _simpleLineRenderer = new SimpleRenderer();
        private SimpleRenderer _simpleMarkerRenderer = new SimpleRenderer();
        private SimpleRenderer _simpleFillRenderer = new SimpleRenderer();
        private OutFields _outFields = new OutFields();
        private int _featureLayersToLoad = 0;
        private int _featureLayersToInitialize = 0;
        private ContextMenu _contextMenu = null;
        private FeatureLayer _featureLayer;
        private bool? _rolloverIsOn = false;
        private bool _cached = false;
        private string _layerName;
        private string _currentStoredView;
        //private string currentFeatLayerURL = "";
        private IDictionary<string, IList<int>> _storedDisplayRolloverLayers = new Dictionary<string, IList<int>>();
        private string _trCGC;
        private MapPoint _mapMouseMapPoint;
        private IdentifyControl _identifyControl;
        private Graphic _selectedGraphic = null;

        private int _lineSymbolWidth = 15;
        private int _pointSymbolWidth = 30;
        private IDictionary<string, IDataGridCustomButton> _customButtons;

        private DeviceSettings.DeviceSettings _deviceSettings;
        private Tlm.Tlm _tlm;
        private MenuItem _loadingInfo;
        private MenuItem _zoomToCircuit;
        private MenuItem _showDetails;  //INC000004469254
        public bool AutoStart { private set; get; }
        public int AutoStartDelaySeconds { private set; get; }
        private OutageHistoryCustomButton _outageHistoryCustomButton;
        //CIT non-identify and grid button
        private CIT_CustomButton _ConductorInTrenchCustomButton;
        private ButterflyCustomButton _butterflyCustomButton;
        private FilterRibbonPanel _filterRibbonPanel;
        private IDictionary<string, FeatureLayer> _featureLayers = new Dictionary<string, FeatureLayer>();

        public PositionMapTip PositionMapTip { get; set; }

        private ComboBox _locateComboBox;
        private int _selectedLocateIndex;
        
            private AttributesViewerControl _attributesViewer;  //INC000004469254

        //********Enos2EDGIS Start, added a new private variable***********
        private ObservableCollection<Miner.Server.Client.RelationshipInformation> _relationshipInformation = null;
        //********************Enos2EDGIS End********************************

        /************ENOSChange Starts***********************/
        private MenuItem _showGeneration;
        private MenuItem _generationOnFeeder;
        private GenOnTransformerTool genOnTransformer;       
        private Grid _mapArea = null;
        private ShowGenerationCustomButton _showGenerationCustomButton;
        private GenOnFeederCustomButton _genFeederCustomButton;
        private string _servicePointURL;
        private string _generationInfoURL;
        // private string _objectID;
        private List<Graphic> generations = new List<Graphic>();
        private List<Graphic> servicePointsOnTransformer = new List<Graphic>();
        private int genrationQueryCounter = 0;
        private int servicePointFeatureCount;
       // private static string _serviceLocationURL;
        
        /************ENOSChange Ends***********************/
        /***********PLC Changes RK 07/25/2017***************/
        private MenuItem _pttReport;
        private string _pttReportUrl;
        /***********PLC Changes Ends***************/

        public bool ShowLoadingInfoFilter;
        //TODO: refactor this so that all we're passing in is map/element/custombuttons

        private Dictionary<string, string> _detailsMenuMatchLayers = new Dictionary<string, string>();
        private List<string> _detailsMenuMatchSV= new List<string>();

        //****************ENOS2EDGIS, "relationshipInformation" parameter added****************************
        public ShowRolloverInfo(Map map, XElement element, IDictionary<string, IDataGridCustomButton> customButtons,
            Tlm.Tlm tlm, DeviceSettings.DeviceSettings deviceSettings, IdentifyControl identifyControl,
            FilterRibbonPanel filterRibbonPanel, ObservableCollection<Miner.Server.Client.RelationshipInformation> relationshipInformation, AttributesViewerControl AttributesViewer, Grid mapArea) //ENOSChange Grid mapArea added
        //**************************************ENOS2EDGIS End**********************************************

        {
            _customButtons = customButtons;
            _map = map;
            InitializeFromXML(element);
            InitializeRenderers();
            _tlm = tlm;
            _deviceSettings = deviceSettings;
            _identifyControl = identifyControl;
            _filterRibbonPanel = filterRibbonPanel;
            //******ENOS2EDGIS Start******************
            _relationshipInformation = relationshipInformation;
            //******ENOS2EDGIS End********************
            /********ENOSChange Starts************/
            _mapArea = mapArea;
            /********ENOSChange Ends************/

            _attributesViewer = AttributesViewer;
        }

        private void InitializeContextMenu()
        {
            if (_contextMenu == null)
            {
                _contextMenu = new ContextMenu();
                var menuItemCopy = new MenuItem() { Header = "Copy" };
                menuItemCopy.Click += new RoutedEventHandler(menuItemCopy_Click);
                _contextMenu.Items.Add(menuItemCopy);
                foreach (KeyValuePair<string, IDataGridCustomButton> dataGridCustomButton in _customButtons)
                {
                    var menuItemDataGridButton = new MenuItem() { Header = dataGridCustomButton.Key };
                    menuItemDataGridButton.Click += new RoutedEventHandler(menuItemDataGridButton_Click);
                    if (dataGridCustomButton.Value.Visible)
                    {
                        _contextMenu.Items.Add(menuItemDataGridButton);
                    }
                    if (dataGridCustomButton.Value is OutageHistoryCustomButton)
                    {
                        _outageHistoryCustomButton = dataGridCustomButton.Value as OutageHistoryCustomButton;
                    }
                    //CIT non-identify and grid method
                    if (dataGridCustomButton.Value is CIT_CustomButton)
                    {
                        _ConductorInTrenchCustomButton = dataGridCustomButton.Value as CIT_CustomButton;
                    }
                    if (dataGridCustomButton.Value is ButterflyCustomButton)
                    {
                        _butterflyCustomButton = dataGridCustomButton.Value as ButterflyCustomButton;
                    }
                    //ENOSChange Start
                    if (dataGridCustomButton.Value is ShowGenerationCustomButton)
                    {
                        _showGenerationCustomButton = dataGridCustomButton.Value as ShowGenerationCustomButton;
                        
                    }
                    if (dataGridCustomButton.Value is GenOnFeederCustomButton)
                    {
                        _genFeederCustomButton = dataGridCustomButton.Value as GenOnFeederCustomButton;

                    }
                    //ENOSChange End
                }

                //Command for showing loading information
                _loadingInfo = new MenuItem() { Header = "Show Loading Information" };
                _loadingInfo.Click += new RoutedEventHandler(loadingInfo_Click);
                _contextMenu.Items.Add(_loadingInfo);

                _zoomToCircuit = new MenuItem() { Header = "Zoom To Circuit" };
                _zoomToCircuit.Click += new RoutedEventHandler(_zoomToCircuit_Click);
                _contextMenu.Items.Add(_zoomToCircuit);

                var menuItemIdentify = new MenuItem() { Header = "Identify" };
                menuItemIdentify.Click += new RoutedEventHandler(menuItemIdentify_Click);
                _contextMenu.Items.Add(menuItemIdentify);

                /***********PLC Changes RK 07/21/2017***************/
                _pttReport = new MenuItem() { Header = "PTT Data" };
                _pttReport.Click += new RoutedEventHandler(_pttReport_Click);
                _contextMenu.Items.Add(_pttReport);
                /***********PLC Changes Ends***************/                
                 _showDetails = new MenuItem() { Header = "Details" };
                _showDetails.Click += new RoutedEventHandler(showDetails_Click);
                _contextMenu.Items.Add(_showDetails);

                _contextMenu.Opened += new RoutedEventHandler(_contextMenu_Opened);
            }

        }

        /***********PLC Changes RK 07/25/2017***************/
        void _pttReport_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_selectedGraphic != null)
                {
                    string poleSapEquipId = _selectedGraphic.Attributes["SAPEQUIPID"].ToString();
                    if (poleSapEquipId != null && poleSapEquipId != "0")
                    {
                        string pttUrl = _pttReportUrl + poleSapEquipId;
                        HtmlPage.Window.Navigate(new Uri(pttUrl), "_blank");
                        ConfigUtility.UpdateStatusBarText("");
                    }
                }
                else
                {
                }
            }
            catch (Exception ex)
            {
                ConfigUtility.UpdateStatusBarText("Unable to open PTT Data: " + ex.Message);
            }
        }
        /***********PLC Changes ends***************/

        void _zoomToCircuit_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string feederID = _selectedGraphic.Attributes["CIRCUITID"].ToString();
                string feederName = _selectedGraphic.Attributes["FEEDERNAME"].ToString();
                _filterRibbonPanel.ZoomToCircuit(feederID, feederName, true);
            }
            catch (Exception ex)
            {
                ConfigUtility.UpdateStatusBarText("Unable to zoom to circuit: " + ex.Message);
            }
        }

        void menuItemIdentify_Click(object sender, RoutedEventArgs e)
        {
            Envelope env = new Envelope(_mapMouseMapPoint.X - MainPage._rightClickExtentWidth, _mapMouseMapPoint.Y - MainPage._rightClickExtentWidth,
                _mapMouseMapPoint.X + MainPage._rightClickExtentWidth, _mapMouseMapPoint.Y + MainPage._rightClickExtentWidth);

            _identifyControl.Identify(env);
        }

        void loadingInfo_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string globalID = GetGlobalID();
                string feederID = _selectedGraphic.Attributes["CIRCUITID"].ToString();
                string objectID = _selectedGraphic.Attributes["OBJECTID"].ToString();
                string operatingNumber = "";
                if (_selectedGraphic.Attributes["OPERATINGNUMBER"] != null)
                {
                    operatingNumber = _selectedGraphic.Attributes["OPERATINGNUMBER"].ToString();
                }

                LoadingInformation.loadingInfo.GetLoadingInformation(objectID, operatingNumber, _layerName, globalID, feederID, _map);
            }
            catch (Exception ex)
            {
                ConfigUtility.UpdateStatusBarText("Unable to get loading information: " + ex.Message);
            }
        }

        void showDetails_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_selectedGraphic.Attributes["GLOBALID"] != null && _currentStoredView != null)
                {
                    ShowDetailsMenu detailsMenu = new ShowDetailsMenu();
                    detailsMenu.GlobalId = _selectedGraphic.Attributes["GLOBALID"].ToString();
                    ArcGISDynamicMapServiceLayer layer = _map.Layers[_currentStoredView] as ArcGISDynamicMapServiceLayer;
                    if (layer != null)
                        detailsMenu.ServiceUrl = layer.Url;
                    detailsMenu.LayerId = ConfigUtility.GetLayerIDFromLayerName(ConfigUtility.CurrentMap, layer.Url, _layerName);
                    if (detailsMenu.LayerId == -1)
                    {
                        //handle Closed and Opened layers as there layer names might be diferent in stored views
                        if (_layerName.EndsWith(" - Closed"))
                            detailsMenu.LayerId = ConfigUtility.GetLayerIDFromLayerName(ConfigUtility.CurrentMap, layer.Url, _layerName.Substring(0, _layerName.LastIndexOf(" - Closed")));
                        else if (_layerName.EndsWith(" - Opened"))
                            detailsMenu.LayerId = ConfigUtility.GetLayerIDFromLayerName(ConfigUtility.CurrentMap, layer.Url, _layerName.Substring(0, _layerName.LastIndexOf(" - Opened")));

                        //handle Primary Overhead / UnderGround Conductor for Circuit Map and Circuit Map 50 Scale Stored View
                        if (_detailsMenuMatchSV.Contains(_currentStoredView))
                        {
                            if (_selectedGraphic.Attributes.ContainsKey("SUBTYPE"))
                            {

                                if (_detailsMenuMatchLayers.ContainsKey(_layerName + ";" + Convert.ToInt16(_selectedGraphic.Attributes["SUBTYPE"]).ToString()))
                                {
                                    detailsMenu.LayerId = ConfigUtility.GetLayerIDFromLayerName(ConfigUtility.CurrentMap, layer.Url, _detailsMenuMatchLayers[_layerName + ";" + Convert.ToInt16(_selectedGraphic.Attributes["SUBTYPE"]).ToString()]);
                                }
                            }
                        }
                    }

                    if (detailsMenu.LayerId > 0)
                    {
                        detailsMenu.DetailsComplete += DetailsControl_WorkCompleted;
                        detailsMenu.LocateAsync();

                        ArcFMSilverlight.Controls.DataGridContextMenu.DataGridContextMenu dgcm = new ArcFMSilverlight.Controls.DataGridContextMenu.DataGridContextMenu();   //To get Print button
                        dgcm.DetailsButton_Click(sender, e);
                    }
                    else
                    {
                        ConfigUtility.UpdateStatusBarText("Unable to show Details: Layer is not found in selected Stored View.");
                    }
                }
            }
            catch (Exception ex)
            {
                ConfigUtility.UpdateStatusBarText("Unable to show Details: " + ex.Message);
            }
        }

        void menuItemDataGridButton_Click(object sender, RoutedEventArgs e)
        {
            IDataGridCustomButton customButton = _customButtons[((MenuItem)sender).Header.ToString()];
            customButton.Attributes = _selectedGraphic.Attributes;
            //*************************ENOS2EDGIS Start********************************************************
            //Enos2EDGIS, If user clicked on 'More Info...'
            if (customButton.Name == "More Info...")
            {
                //Enos2EDGIS, for feature classes othr than service location , open the SETTINGS Ui directly
                if (_layerName != "Service Location")
                {
                    //customButton.Open(_selectedGraphic.Attributes["GLOBALID"].ToString() + (_layerName == "Primary Meter" ? _selectedGraphic.Attributes["CGC12"].ToString() : string.Empty), _layerName);
                    customButton.Open(_selectedGraphic.Attributes["GLOBALID"].ToString(), _layerName);
                }
                else
                {
                    //Service location do not have settings data, so we need to get Generation data through service point id.
                    int objectId = GetObjectIDValue(_selectedGraphic.Attributes);
                    if (objectId >= 0)
                    {
                        SpatialReference spatialReference = _selectedGraphic.Geometry.SpatialReference;
                        int layerID = 0;
                        //for service location first we need to get the related generation data, and then open settings ui for those generations.
                        Miner.Server.Client.IRelationshipService relationshipService = new Miner.Server.Client.RelationshipService();
                        string decodedService = string.Empty;
                        if (_featureLayer != null)
                        {
                            string serviceURL = _featureLayer.Url;
                            //if the service URL is a feature server, try to get a correspoinding map server URL
                            if (serviceURL != string.Empty && serviceURL.ToUpper().Contains("FEATURESERVER"))
                            {
                                if (_subtypeLookupUrl.ContainsKey(_featureLayer.Url))
                                {
                                    string subTypeUrl = _subtypeLookupUrl[_featureLayer.Url];
                                    serviceURL = subTypeUrl.Substring(0, subTypeUrl.LastIndexOf("/"));
                                    bool converted = Int32.TryParse(subTypeUrl.Substring(subTypeUrl.LastIndexOf("/") + 1), out layerID);
                                }
                            }
                            decodedService = HttpUtility.UrlDecode(serviceURL);
                        }
                        //Get the relationship for service location where it has muliple relationship and one of the relationship should be with Service Point
                        IEnumerable<Miner.Server.Client.RelationshipInformation> relationshipInfos = from relatedInfo in _relationshipInformation
                                                                                                     where (relatedInfo.LayerId == layerID) && relatedInfo.RelationshipNames.Any(x => x.ToUpper().IndexOf("SERVICEPOINT", StringComparison.CurrentCultureIgnoreCase) != -1) &&
                                                                                                            relatedInfo.RelationshipIds.Count == 2 && decodedService != null &&
                                                                                                           decodedService.ToLower().Contains(relatedInfo.Service.ToLower())
                                                                                                     select relatedInfo;

                        relationshipService.DefinitionExpression = string.Empty;

                        relationshipService.ExecuteCompleted += (s, i) => RelationshipServiceCompleted(sender, i);

                        relationshipService.GetRelationshipsAsync(relationshipInfos, new int[] { objectId }, spatialReference);
                        //ENOS2EDGIS, showing Gen Type Domain values 
                        GetGenTypeDomainValues();
                    }
                }
            }
               //********************ENOSChange Start******************************
            else if (customButton.Name == "Show Generation")
            {
                string globalId = _selectedGraphic.Attributes["GLOBALID"].ToString();
                // ENOSTOEDGIS - Changes for adding CGC to caption Starts
                try
                {
                    _trCGC = Convert.ToString(_selectedGraphic.Attributes["CGC12"]);
                    if (string.IsNullOrEmpty(_trCGC) || string.IsNullOrWhiteSpace(_trCGC))
                    {
                        _trCGC = "NA";
                    }
                }
                catch (Exception exp)
                {
                }
                if (globalId != null)
                {
                    Query query = new Query(); //query on service point layer          
                    query.OutFields.AddRange(new string[] { "GLOBALID", "SERVICEPOINTID", "STREETNUMBER", "STREETNAME1", "STREETNAME2", "CITY", "STATE", "COUNTY" }); //Adding City to Address 
                    query.Where = "TRANSFORMERGUID='" + globalId + "'";
                    QueryTask queryTask = new QueryTask(_servicePointURL);
                    queryTask.ExecuteCompleted += new EventHandler<QueryEventArgs>(spQueryTask_ExecuteCompleted);
                    queryTask.ExecuteAsync(query);
                }
            }
            else if (customButton.Name == "Generation On Feeder")
            {
                string circuiId = _selectedGraphic.Attributes["CIRCUITID"].ToString();
                GetGenOnFeeder getGeneration = new GetGenOnFeeder();
                getGeneration.genGenOnFeeder_Click(circuiId);
            }

                 //********************ENOSChange end******************************
            else
            {
                customButton.Open(_selectedGraphic.Attributes["GLOBALID"].ToString() + (_layerName == "Primary Meter" ? _selectedGraphic.Attributes["CGC12"].ToString() : string.Empty), _layerName);
            }
            //ENOS2EDGIS, Commented below line 
            // customButton.Open(_selectedGraphic.Attributes["GLOBALID"].ToString() + (_layerName == "Primary Meter" ? _selectedGraphic.Attributes["CGC12"].ToString() : string.Empty), _layerName);
            //**************************************ENOS2EDGIS End**********************************************************
        }

        /****************************ENOSChange Starts*******************************/
       

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
                MessageBox.Show("No Generations are attached to this service location", "No Generations Found", MessageBoxButton.OK);
            }
        }

        void getGenerationInfo(string _servicePtGlobalID)
        {

            GetGenTypeDomainValues();
            Query query = new Query(); //query on generation info layer
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
            /******ENOS2SAP PhaseIII Changes Start*******/
            
            showGenInfoData(servicePointsOnTransformer);
            //servicePointsOnTransformer.Clear();
            ConfigUtility.DivisionCodedDomains = null;

            /******ENOS2SAP PhaseIII Changes End*******/
        }

        void showGenInfoData(IList<Graphic> generations)
        {
            genOnTransformer = new GenOnTransformerTool(_mapArea, generations, _deviceSettings.DeviceSettingURL, true, _map);

            /******ENOS2SAP PhaseII Changes Start*******/
            genOnTransformer._generations = servicePointsOnTransformer;
            genOnTransformer._cgcNumber = _trCGC;
          //  genOnTransformer.OpenDialog(_trCGC);

            /******ENOS2SAP PhaseII Changes End*******/
        }      
        

        /*****************************ENOSChange Ends*******************************/

        //*********************ENOS2EDGIS Start***************************************************
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
        //ENOS2EDGIS, Gets the Object Id feild name 
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
                MessageBox.Show("No Generations are attached to this service location", "No Generatins Found", MessageBoxButton.OK);
            }
            else if ((args != null) && (args.Results != null))
            {
                //As we Queried for a single Service location, we are expecting the results count will be 1
                if (args.Results.Count == 1)
                {
                    foreach (var resultsByObjectId in args.Results)
                    {
                        int slOID = resultsByObjectId.Key;
                        foreach (IResultSet result in resultsByObjectId.Value)
                        {
                            //If only one generation record is found, then directly open the settings UI app
                            if (result.Features.Count == 1)
                            {
                                IDataGridCustomButton customButton = _customButtons[((MenuItem)sender).Header.ToString()];
                                customButton.Attributes = result.Features[0].Attributes;
                                customButton.Open(result.Features[0].Attributes["GLOBALID"].ToString(), "EDGIS.GenerationInfo");
                            }
                            else
                            {
                                //But if we have more than one related generation info records for a single service location, show them in a child window.
                                GenerationInfoWindow genInfoWindow = new GenerationInfoWindow();
                                genInfoWindow.DeviceSettingURL = _deviceSettings.DeviceSettingURL;
                                genInfoWindow.ServiceLocationOID = slOID.ToString();
                                genInfoWindow.InitializeViewModel(result.Features);
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

                        }
                    }
                }
            }
        }

        //*********************************ENOS2EDGIS End *****************************************

        void menuItemCopy_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedGraphic != null)
            {
                string clipboardText = GetAttributesAsText();
                Clipboard.SetText(clipboardText);
            }
        }

        private string GetAttributesAsText()
        {
            string attributesAsString = "Feature:" + _layerName + ";";

            for (int i = 0; i < _featureLayer.LayerInfo.Fields.Count; i++)
            {
                Field field = _featureLayer.LayerInfo.Fields[i];
                KeyValuePair<string, object> keyValuePair = _selectedGraphic.Attributes.ElementAt(i);
                if (field.Name.ToUpper().StartsWith("SUBTYPE"))
                {
                    attributesAsString += field.Alias + ":" + GetSubtypeString() + ",";
                }
                else if (field.Domain != null && field.Domain is CodedValueDomain)
                {
                    if (keyValuePair.Value == null || keyValuePair.Value == DBNull.Value) continue;  // Code Change for INC000004021419 by S2NN @ 06082015
                    attributesAsString += field.Alias + ":" + (field.Domain as CodedValueDomain).CodedValues[keyValuePair.Value] + ",";
                }
                else
                {
                    attributesAsString += field.Alias + ":" + keyValuePair.Value.GetDatabaseStringValue() + ",";
                }

            }

            attributesAsString = attributesAsString.Substring(0, attributesAsString.Length - 1);

            return attributesAsString;
        }
        private string GetGlobalID()
        {
            return _selectedGraphic.Attributes["GLOBALID"].ToString();
        }

        private int GetSubtype()
        {
            if (_selectedGraphic.Attributes.ContainsKey("SUBTYPECD"))
                return Convert.ToInt32(_selectedGraphic.Attributes["SUBTYPECD"]);

            if (_selectedGraphic.Attributes.ContainsKey("SUBTYPE"))
                return Convert.ToInt32(_selectedGraphic.Attributes["SUBTYPE"]);

            return -1;
        }

        private string GetSubtypeString()
        {
            int subtype = GetSubtype();
            string subtypeDesc = "No Subtypes";
            if (subtype > -1 && ((MapTipPopup)_featureLayer.MapTip)._subtypeCodeDescriptions.Count > 0)
            {// Code Change for INC000004021419 by S2NN @ 06082015
                try
                {
                    subtypeDesc =
                        ((MapTipPopup)_featureLayer.MapTip)._subtypeCodeDescriptions[
                            subtype];
                }
                catch { subtypeDesc = string.Empty; }
            }
            return subtypeDesc;
        }

        void _contextMenu_Opened(object sender, RoutedEventArgs e)
        {
            string serviceURL = _featureLayer.Url.Substring(0, _featureLayer.Url.LastIndexOf("/"));

            _tlm.IsManuallyEnabled = _tlm.LayerIsTlm(_layerName);
            ((IDataGridCustomButton)_outageHistoryCustomButton).IsManuallyEnabled = (_layerName.ToUpper() ==
                                                                                     "TRANSFORMER");
            _outageHistoryCustomButton.Attributes = _selectedGraphic.Attributes;
            //CIT non-identify + grid tool
            if (_ConductorInTrenchCustomButton != null)
            {
                ((IDataGridCustomButton)_ConductorInTrenchCustomButton).IsManuallyEnabled = (_layerName.ToUpper() ==
                                                                                         "PRIMARY UNDERGROUND CONDUCTOR");
                _ConductorInTrenchCustomButton.Attributes = _selectedGraphic.Attributes;
            }
            ((IDataGridCustomButton)_butterflyCustomButton).IsManuallyEnabled = ButterflyTool.ButterflyRolloverLayerNames.Contains(_layerName);

            //******************ENOSChange Start**************
            ((IDataGridCustomButton)_showGenerationCustomButton).IsManuallyEnabled = (_layerName.ToUpper() ==
                                                                                     "TRANSFORMER");
            ((IDataGridCustomButton)_genFeederCustomButton).IsManuallyEnabled = (GetGenOnFeeder.GenFeederLayerName.Contains(_layerName));
            //******************ENOSChange End****************
            bool selectionIsDeviceSettingsEnabled = false;
            // Check for settings validity
            if (_deviceSettings._deviceSettingsLayerMappings.ContainsKey(serviceURL))
            {
                string mappedServiceURL = _deviceSettings._deviceSettingsLayerMappings[serviceURL];
                selectionIsDeviceSettingsEnabled = _deviceSettings.SelectionIsDeviceSettingsEnabled(mappedServiceURL, _layerName);
            }
            _deviceSettings.IsManuallyEnabled = selectionIsDeviceSettingsEnabled;

            SetContextMenuItemsEnabled();
        }


        void SetContextMenuItemsEnabled()
        {
            foreach (IDataGridCustomButton dataGridCustomButton in _customButtons.Values)
            {
                if (dataGridCustomButton.Visible)
                {
                    MenuItem menuItem =
                        _contextMenu.Items.Cast<MenuItem>().Where<MenuItem>(mi => mi.Header.ToString() == dataGridCustomButton.Name).First();
                    if (dataGridCustomButton.IsManuallyEnabled)
                    {
                        menuItem.Visibility = Visibility.Visible;
                        //menuItem.IsEnabled = true;
                        //menuItem.Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Black);
                        //VisualStateManager.GoToState(menuItem, "Normal", true);
                    }
                    else
                    {
                        menuItem.Visibility = Visibility.Collapsed;
                        //menuItem.Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Gray);
                        //menuItem.IsEnabled = false;
                        //VisualStateManager.GoToState(menuItem, "Disabled", true);
                    }
                }
            }

            if (_selectedGraphic != null &&
                _selectedGraphic.Attributes.ContainsKey("CIRCUITID"))
            {
                _loadingInfo.Visibility = Visibility.Visible;
                _zoomToCircuit.Visibility = Visibility.Visible;
                //                SetMenuItemEnabled(_loadingInfo, true);
            }
            else
            {
                _loadingInfo.Visibility = Visibility.Collapsed;
                _zoomToCircuit.Visibility = Visibility.Collapsed;
                //                SetMenuItemEnabled(_loadingInfo, false);
            }
            /***********PLC Changes RK 07/25/2017***************/
            //Condition to enable and disable PTT Data menu item
            if (_selectedGraphic != null && _selectedGraphic.Attributes.ContainsKey("PLDBID"))
            {
                if (_selectedGraphic.Attributes["SAPEQUIPID"] != null && Convert.ToInt64(_selectedGraphic.Attributes["SAPEQUIPID"]) != 0)
                {
                    SetMenuItemEnabled(_pttReport, true);
                }
                else
                {
                    SetMenuItemEnabled(_pttReport, false);
                }
            }
            else
            {
                SetMenuItemEnabled(_pttReport, false);
            }
            /***********PLC Changes Ends***************/           

             //INC000004469254
            if (_selectedGraphic != null && _selectedGraphic.Attributes.ContainsKey("GLOBALID"))
                _showDetails.Visibility = Visibility.Visible;
            else
                _showDetails.Visibility = Visibility.Collapsed;

            //Condition to enable and disable Show Loading Information menu item
            if (ShowLoadingInfoFilter == true)
            {
                SetMenuItemEnabled(_loadingInfo, true);
            }
            else
            {
                SetMenuItemEnabled(_loadingInfo, false);
            }

        }

        private void SetMenuItemEnabled(MenuItem menuItem, bool setEnabled)
        {
            if (setEnabled)
            {
                menuItem.IsEnabled = true;
                menuItem.Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Black);
                VisualStateManager.GoToState(menuItem, "Normal", true);
            }
            else
            {
                menuItem.Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Gray);
                menuItem.IsEnabled = false;
                VisualStateManager.GoToState(menuItem, "Disabled", true);
            }
        }
               

        private void InitializeFromXML(XElement element)
        {
            AutoStart = Convert.ToBoolean(element.Attribute("AutoStart").Value);
            AutoStartDelaySeconds = Convert.ToInt32(element.Attribute("AutoStartDelaySeconds").Value);
            _pointSymbolWidth = Convert.ToInt32(element.Attribute("PointTolerance").Value);
            _lineSymbolWidth = Convert.ToInt32(element.Attribute("LineTolerance").Value);
            foreach (XElement layerElement in element.Element("PolygonLayers").Elements())
            {
                _polygonFeatureLayerURLs.Add(layerElement.Attribute("Url").Value + layerElement.Attribute("LayerId").Value);
                _subtypeLookupUrl.Add(_polygonFeatureLayerURLs.Last(), layerElement.Attribute("LinkedSubtypeFC").Value);
            }

            foreach (XElement layerElement in element.Element("LineLayers").Elements())
            {
                _lineFeatureLayerURLs.Add(layerElement.Attribute("Url").Value + layerElement.Attribute("LayerId").Value);
                _subtypeLookupUrl.Add(_lineFeatureLayerURLs.Last(), layerElement.Attribute("LinkedSubtypeFC").Value);
            }
            foreach (XElement layerElement in element.Element("PointLayers").Elements())
            {
                _pointFeatureLayerURLs.Add(layerElement.Attribute("Url").Value + layerElement.Attribute("LayerId").Value);
                _subtypeLookupUrl.Add(_pointFeatureLayerURLs.Last(), layerElement.Attribute("LinkedSubtypeFC").Value);
            }
            _featureLayerURLs = _polygonFeatureLayerURLs.Concat(_lineFeatureLayerURLs).ToList();
            _featureLayerURLs = _featureLayerURLs.Concat(_pointFeatureLayerURLs).ToList();

            foreach (XElement storedDisplayRolloverElement in element.Element("StoredDisplayRollovers").Elements())
            {
                string storedDisplay = storedDisplayRolloverElement.Attribute("StoredDisplay").Value;
                string layersCSV = storedDisplayRolloverElement.Attribute("LayerIdsCSV").Value;
                IList<string> layersArray = layersCSV.Split(',').ToList();
                _storedDisplayRolloverLayers.Add(storedDisplay, layersArray.Select(x => Int32.Parse(x)).ToList());
            }

            _outFields.Add("*");
            /***********PLC Changes RK 07/25/2017***************/
            _pttReportUrl = element.Element("PTTDATA").Attribute("Url").Value;
            /***********PLC Changes End***************/

            //**************************ENOSChange Start*************************************
            _servicePointURL = element.Element("ShowRollOverServicePoint").Attribute("URL").Value;
            _generationInfoURL = element.Element("ShowRollOverSetingGenInfo").Attribute("URL").Value;
            //***************************ENOSChange End*******************************************

            //handle Primary Overhead / UnderGround Conductor for Circuit Map and Circuit Map 50 Scale Stored View
            _detailsMenuMatchSV = element.Element("DetailsMenuMatchLayers").Attribute("StoredViewNames").Value.ToString().Split(',').ToList();
            try
            {
                foreach (XElement _detailsMenuMatchLayersElement in element.Element("DetailsMenuMatchLayers").Elements())
                {
                    string publicationLayerName = _detailsMenuMatchLayersElement.Attribute("PublicationLayerName").Value;
                    List<string> subTypeList = _detailsMenuMatchLayersElement.Attribute("SubType").Value.ToString().Split(',').ToList();
                    string SVLayerName = _detailsMenuMatchLayersElement.Attribute("SVLayerName").Value;
                    foreach (string subtype in subTypeList)
                    {
                        _detailsMenuMatchLayers.Add(publicationLayerName + ";" + subtype, SVLayerName);
                    }
                }
            }
            catch
            {
            }
        }

        private void InitializeRenderers()
        {
            _simpleLineSymbol.Color = new SolidColorBrush(Color.FromArgb(0, 255, 0, 0));
            _simpleLineSymbol.Width = _lineSymbolWidth;
            _simpleLineRenderer.Symbol = _simpleLineSymbol;
            _simpleMarkerSymbol.Color = _simpleLineSymbol.Color;
            _simpleMarkerSymbol.Size = _pointSymbolWidth;
            _simpleMarkerRenderer.Symbol = _simpleMarkerSymbol;
            _simpleFillSymbol.Color = Color.FromArgb(0, 255, 0, 0);
            _simpleFillSymbol.BorderBrush = _simpleLineSymbol.Color;
            _simpleFillSymbol.BorderStyle = ESRI.ArcGIS.Client.Symbols.SimpleLineSymbol.LineStyle.Solid;
            _simpleFillSymbol.Style = SimpleFillSymbol.SimpleFillStyle.Solid;
            _simpleFillRenderer.Symbol = _simpleFillSymbol;
        }

        public void Show(LocateControl locateControl)
        {
            Grid lcGrid = (Grid)VisualTreeHelper.GetChild(locateControl, 0);
            _locateComboBox = ((ComboBox)lcGrid.FindName("PART_LocateTypeComboBox"));
            _selectedLocateIndex = _locateComboBox.SelectedIndex;

            if (!_cached)
            {
                InitializeContextMenu();
                LoadLayers();
            }
            else
            {
                UnHide();
            }
            _rolloverIsOn = true;
        }

        private void UnHide()
        {
            SetLayersVisibilityByStoredView();
        }

        private void LoadLayers()
        {

            // Set StatusBar
            ConfigUtility.StatusBar.Text = "Loading...";
            foreach (string featureLayerUrL in _featureLayerURLs)
            {
                // Add FeatureLayer
                FeatureLayer fl = new FeatureLayer();
                fl.Url = featureLayerUrL;
                fl.Mode = FeatureLayer.QueryMode.OnDemand;
                fl.OutFields = _outFields;
                if (_lineFeatureLayerURLs.Contains(featureLayerUrL))
                {
                    fl.Renderer = _simpleLineRenderer;
                }
                else if (_polygonFeatureLayerURLs.Contains(featureLayerUrL))
                {
                    fl.Renderer = _simpleFillRenderer;
                }
                else
                {
                    fl.Renderer = _simpleMarkerRenderer;
                }

                fl.Initialized += new EventHandler<EventArgs>(fl_Initialized);
                _featureLayersToLoad++;
                _map.Layers.Add(fl);
            }
        }

        public void StoredViewChange(string storedViewName)
        {
            _currentStoredView = storedViewName;

            SetLayersVisibilityByStoredView();
        }

        void fl_Initialized(object sender, EventArgs e)
        {
            FeatureLayer l = sender as FeatureLayer;
            if (l.LayerInfo != null)
            {
                _featureLayers.Add(l.Url, l);
                // Contact MapServer to get field order & visibility. Bit of a drag on AGS but the FS doesn't show either.
                FeatureLayer mapServerFl = new FeatureLayer();
                mapServerFl.Url = l.Url.Replace("FeatureServer", "MapServer");
                mapServerFl.Initialized += new EventHandler<EventArgs>(mapServerFl_Initialized);
                _featureLayersToInitialize++;
                mapServerFl.Initialize();
            }
            else
            {
                ConfigUtility.StatusBar.Text = "Error contacting FeatureLayer";
            }
            if (--_featureLayersToLoad == 0)
            {
                _cached = true;
                // Reset Status Bar...
                ConfigUtility.StatusBar.Text = "";
                //TODO: need to figure out when the map is loaded in order to SetLayersVisibilityByStoredView();
            }
        }

        void mapServerFl_Initialized(object sender, EventArgs e)
        {
            List<Field> fields = new List<Field>();
            FeatureLayer mapServerFeatureLayer = sender as FeatureLayer;
            FeatureLayer featureLayer = _featureLayers[mapServerFeatureLayer.Url.Replace("MapServer", "FeatureServer")];
            List<Field> featureServerFields = featureLayer.LayerInfo.Fields;
            // Apply Field Order & Mask
            foreach (Field sourceField in mapServerFeatureLayer.LayerInfo.Fields)
            {
                if (featureServerFields.Where(f => f.Name == sourceField.Name).ToList().Count > 0)
                {
                    fields.Add(featureServerFields.Where(f => f.Name == sourceField.Name).ToList().First());
                }
            }

            AddFeatureLayerMapTip(featureLayer, fields);

            if (--_featureLayersToInitialize == 0)
            {
                // Reset LocateControl dropdown here...it got reset somehow by adding layers. TODO: figure out why.
                _locateComboBox.SelectedIndex = _selectedLocateIndex;
            }
        }

        private void AddFeatureLayerMapTip(FeatureLayer fl, List<Field> fields)
        {

            fl.MapTip = new MapTipPopup(fields, fl.OutFields,
                fl.DisplayName, fl.LayerInfo.Name, _subtypeLookupUrl[fl.Url]);

            PositionMapTip.wireHandlers(fl);

            fl.ID = ROLLOVER_FEATURELAYER_PREFIX + fl.LayerInfo.Name;
            fl.MouseRightButtonDown += new GraphicsLayer.MouseButtonEventHandler(l_MouseRightButtonDown);
            fl.MouseRightButtonUp += new GraphicsLayer.MouseButtonEventHandler(l_MouseRightButtonUp);
            SetLayerVisibilityByStoredView(fl);

        }

        private bool FeatureLayerIsVisible(FeatureLayer featureLayer)
        {
            if (featureLayer.Where == null && featureLayer.Visible)
            {
                return true;
            }
            return false;
        }

        private void SetLayerVisibilityByStoredView(FeatureLayer fl, bool update = false)
        {
            if (!_rolloverIsOn.HasValue) return;

            if (_storedDisplayRolloverLayers.ContainsKey(_currentStoredView))
            {
                bool setVisible = _storedDisplayRolloverLayers[_currentStoredView].Contains(fl.LayerInfo.Id);
                /*******************PLC Changes Starts*************************/
                if (fl.ID == "Rollover_PLD_INFO")
                {
                   
                        if (_map.Layers["PLCINFO"].Visible == true)
                        {
                            SetFeatureLayerVisibility(fl, true, update);

                        }
                        else
                        {
                            SetFeatureLayerVisibility(fl, false, update);
                        }
                   
                }else{

                if (FeatureLayerIsVisible(fl) != setVisible)
                {
                            
                        SetFeatureLayerVisibility(fl, setVisible, update);              
                }
            }
                /*******************PLC Changes Ends*************************/

            }
            else
            {
                Hide();
            }
        }

        private void SetLayersVisibilityByStoredView()
        {
            IList<Layer> layers = _map.Layers.Where(l => l is FeatureLayer && l.ID != null && l.ID.StartsWith(ROLLOVER_FEATURELAYER_PREFIX)).ToList();
            foreach (Layer layer in layers)
            {
                SetLayerVisibilityByStoredView(layer as FeatureLayer, true);
            }

        }

        void l_MouseRightButtonUp(object sender, GraphicMouseButtonEventArgs e)
        {
            _mapMouseMapPoint = _map.ScreenToMap(e.GetPosition(_map));
            ContextMenuService.SetContextMenu((DependencyObject)sender, _contextMenu);
            _featureLayer = ((FeatureLayer)sender);
            _layerName = ((FeatureLayer)sender).LayerInfo.Name;
            //_layerID = ((FeatureLayer)sender).LayerInfo.Id;
            _selectedGraphic = e.Graphic;
            _contextMenu.IsOpen = true;
        }


        void l_MouseRightButtonDown(object sender, GraphicMouseButtonEventArgs e)
        {
            e.Handled = true;
        }

        public void Hide()
        {
            _featureLayersToLoad = 0;
            ConfigUtility.StatusBar.Text = "";
            IList<Layer> layers = _map.Layers.Where(l => l is FeatureLayer).ToList();
            foreach (Layer layer in layers)
            {
                FeatureLayer fl = layer as FeatureLayer;
                if (_featureLayerURLs.Contains(fl.Url))
                {
                    /*******PLC Changes to avoid setting visibility of PLCINFO feature service to false  RK 07/21/2017******/
                    if (fl.ID != "PLCINFO")
                    {
                        SetFeatureLayerVisibility(fl, false, true);
                    }
                    /*******PLC Changes End******/
                }
            }
            _rolloverIsOn = false;
        }


        // Ordinarily we would remove/readd the layers but there is a bug somewhere else in WEBR
        // Causing the app to reload if you set visibility on the featurelayer itself
        private void SetFeatureLayerVisibility(FeatureLayer fl, bool setVisible, bool update = true)
        {
            if (setVisible)
            {
                //fl.Where = null;
                fl.Visible = true;
            }
            else
            {
                //fl.Where = "0=1";
                fl.Visible = false;
                if (update)
                {
                    fl.Update();
                }
            }
        }

        //INC000004469254 - Details on right click
        public void DetailsControl_WorkCompleted(object sender, Miner.Server.Client.ResultEventArgs e)
        {
            try
            {
                if (e != null && e.Results != null)
                {
                    System.Collections.Generic.IEnumerable<Graphic> filteredGraphics;
                    System.Collections.ObjectModel.ObservableCollection<Miner.Server.Client.Tasks.IResultSet> newResults = new

        System.Collections.ObjectModel.ObservableCollection<Miner.Server.Client.Tasks.IResultSet>();
                    System.Collections.Generic.List<Miner.Server.Client.Tasks.IResultSet> oldResults = e.Results.ToList();

                    System.Collections.Generic.List<Miner.Server.Client.Tasks.IResultSet> oldResults1 = e.Results.ToList();

                    oldResults1.Clear();
                    Miner.Server.Client.Tasks.IResultSet ir = new Miner.Server.Client.Tasks.ResultSet();
                    Miner.Server.Client.Tasks.IResultSet ir1;
                    for (int iResultSet_count = 0; iResultSet_count < oldResults.Count; ++iResultSet_count)
                    {
                        ir = oldResults[iResultSet_count];
                        ir1 = new Miner.Server.Client.Tasks.ResultSet() { SpatialReference = ir.SpatialReference };
                        foreach (Graphic graphic in ir.Features)
                            ir1.Features.Add(graphic);
                        oldResults1.Add(ir1);

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
                                }
                            }
                        }
                        //Nothing filtered. Takes all ir.Features.
                        filteredGraphics = from result in ir.Features
                                           /* where result.Attributes.Values.Contains(queryString) */
                                           select result;

                        System.Collections.Generic.List<Graphic> _myGraphics = filteredGraphics.ToList();

                        ir.Features.Clear();

                        foreach (Graphic g in _myGraphics)
                        {
                            ir.Features.Add((Miner.Server.Client.GraphicPlus)g);
                        }

                        newResults.Add(ir);
                        e.Results = newResults;
                    }

                    ObservableCollection<Miner.Server.Client.Tasks.IResultSet> _newResultSets = new ObservableCollection<Miner.Server.Client.Tasks.IResultSet>();

                    //add code for proposed
                    if (e.Results.Any())
                    {
                        var newQueryList = new List<Tuple<string, List<int>>>(); //<Map Service Url, List of Object IDs>

                        foreach (Miner.Server.Client.Tasks.ResultSet result in e.Results)
                        {
                            _newResultSets.Add(result);
                        }
                        if (_newResultSets.Any())
                        // All original results are not proposed - send them to the attribute viewer
                        {
                            SendResultsToRowDetails(_newResultSets);
                        }
                    }
                    else
                    {
                        SendResultsToRowDetails(e.Results);
                    }
                }
            }
            catch (Exception ex)
            {
                ConfigUtility.UpdateStatusBarText("Unable to show Details: " + ex.Message);
            }
        }

        private void SendResultsToRowDetails(IEnumerable<Miner.Server.Client.Tasks.IResultSet> results)
        {
            var newIndex = new Dictionary<string, Miner.Server.Client.Tasks.IResultSet>();
            foreach (var set in results)
            {
                if (set.ID < 0) continue;

                var key = set.Service + "/" + set.ID + "/" + set.Name;
                if (!newIndex.ContainsKey(key))
                {
                    newIndex.Add(key, set);
                }
            }

            var union = new List<Miner.Server.Client.Tasks.IResultSet>();

            foreach (var key in newIndex.Keys)
            {
                union.Add(newIndex[key]);
            }

            ObservableCollection<Miner.Server.Client.Tasks.IResultSet> Results = new ObservableCollection<Miner.Server.Client.Tasks.IResultSet>(union);
            if (Results.Count > 0)
            {
                Results[0].Features.FirstOrDefault<Graphic>().Select();
                _attributesViewer.DisplayDetailsOnRightClick(Results);
            }
        }

    }

    public class DictionaryValuePickerConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return "Null";

            IDictionary<object, string> dictionaryValues = null;

            if (parameter is CodedValueDomain)
            {
                CodedValueDomain codedValueDomain = parameter as CodedValueDomain;
                dictionaryValues = codedValueDomain.CodedValues;
            }
            else
            {
                dictionaryValues = parameter as IDictionary<object, string>;
            }

            if (dictionaryValues.Count > 0 && dictionaryValues.ContainsKey(value))
            {
                return dictionaryValues[value];
            }

            return value;

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    /***************PLC Changes Start********************/
    //Convert Pole Height to foot from inches in onHover Popup
    public class PoleHeightValuePickerConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) { return "Null"; }
            else
            {
                double height = System.Convert.ToDouble(value);
                if (height != 0)
                {
                    value = height / 12;
                    return value;
                }
                else
                {
                    return value;
                }
            }
                  
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    /***************PLC Changes End********************/

    public static class StringExtension
    {
        public static string GetDatabaseStringValue(this object value, bool useQuotes = true)
        {
            if (value == null)
            {
                return "null";
            }
            return Convert.ToString(value);
        }
    }

    


}
