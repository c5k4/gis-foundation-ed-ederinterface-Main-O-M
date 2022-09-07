#region
using System.Diagnostics;
using System;
using System.Xml;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
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
using ESRI.ArcGIS.Client.Toolkit.DataSources;
using ESRI.ArcGIS.Client.WebMap;
using ESRI.ArcGIS.Client.Geometry;
using ESRI.ArcGIS.Client.Tasks;
using ESRI.ArcGIS.Client.FeatureService.Symbols;
using Microsoft.Practices.Prism.Modularity;
using NLog.LayoutRenderers.Wrappers;
using ESRISymbols = ESRI.ArcGIS.Client.Symbols;
using Miner.Silverlight.Logging.Client;
using NLog;
using ArcFM.Silverlight.PGE.CustomTools;
using PageTemplates;
using System.ServiceModel.DomainServices.Client.ApplicationServices;
using System.Windows.Controls.Primitives;
using CodedValueDomain = ESRI.ArcGIS.Client.FeatureService.CodedValueDomain;
using Domain = ESRI.ArcGIS.Client.FeatureService.Domain;
using Polygon = ESRI.ArcGIS.Client.Geometry.Polygon;
using PointCollection = ESRI.ArcGIS.Client.Geometry.PointCollection;
using ArcFMSilverlight.Controls.Tracing;
using System.Text.RegularExpressions;
using ESRI.SilverlightViewer.Controls;
using System.Reflection;
using System.IO;
using System.Threading;
using System.ServiceModel;
using System.ServiceModel.Description;
using ArcFMSilverlight.Controls;
using ESRI.ArcGIS.Client.Symbols;
using ESRI.ArcGIS.Client.Actions;
using Miner.Server.Client.Events;
using Miner.Server.Client.Toolkit.Events;
using Miner.Server.Client.Toolkit;
using PGnE.Printing;
using Miner.Server.Client.Symbols;
#endregion

namespace ArcFMSilverlight
{
    public partial class PONSDialog : UserControl
    {
        private Map _map;
        private Logger logger = LogManager.GetCurrentClassLogger();
        FloatableWindow _fw = null;
        string StrPLANNEDSHUTDOWNID = "";
        public string PLANNEDSHUTDOWNID { get; set; }
        string TranSSD = string.Empty;
        string endpointaddress = string.Empty;
        ObservableCollection<Averycustomerdata> AveryCustomerResult = new ObservableCollection<Averycustomerdata>();
        ObservableCollection<Averycustomerdata> AveryCustomerList = new ObservableCollection<Averycustomerdata>();
        List<ExportFileName> GeneratedExportFileName = new List<ExportFileName>();
        ExportFileName FileName = new ExportFileName();
        private static List<SubStationSearchData> objSBCDataResult = new List<SubStationSearchData>();
        private static string PriUGConductorFCID = "1021";
        private static string PriOHConductorFCID = "1023";
        string DeviceType = string.Empty;
        string SearchType = string.Empty;
        string Trancgs = string.Empty;
        string FedderID = string.Empty;
        string CircuitFedderID = string.Empty;
        string startdeviceID = string.Empty;
        string StartDeviceFeederID = string.Empty;
        string StartDeviceCircuit2 = string.Empty;
        string EndDeviceCircuit2 = string.Empty;
        string StartDeviceName = string.Empty;
        string DeviceFeederID = string.Empty;
        string StartDeviceGUID = string.Empty;
        string StartdeviceobjectID = string.Empty;
        string EnddeviceID = string.Empty;
        string EndDeviceName = string.Empty;
        string EndDeviceGUID = string.Empty;
        string ENddeviceobjectID = string.Empty;
        string EndDeviceFeederID = string.Empty;
        string strFlag = string.Empty;
        string StartDeviceFeederGUID = string.Empty;
        string EndDeviceFeederGUID = string.Empty;
        int Start_TO_FEATURE_FCID;
        int End_TO_FEATURE_FCID;
        int ORDER_NUM; int MIN_BRANCH; int MAX_BRANCH; int TREELEVEL; int TO_FEATURE_FEEDERINFO; int TO_FEATURE_EID; int TO_FEATURE_OID; int TO_FEATURE_FCID;
        int TO_CIRCUITID;
        string FEEDERID_tracing = string.Empty;
        string FEEDERFEDBY = string.Empty;
        string ServiceLocation_GUIDstring;
        // string ServiceLocation_EndGUIDstring;
        List<string> listStartdeviceSL = new List<string>();
        List<string> listEnddeviceSL = new List<string>();
        string StrServicePointID_StartDevice = string.Empty;
        string StrServicePointID_EndDevice = string.Empty;
        string strDeviceServicePointListResult = string.Empty;
        List<string> ListStrServicePointID_StartDevice = new List<string>();
        List<string> ListStrServicePointID_EndDevice = new List<string>();
        int Divisioncode;
        private Searchdata childWindow = new Searchdata();
        ArrayList objList = new ArrayList();
        List<DeviceSearchlist> Searchdevice = new List<DeviceSearchlist>();
        int checknumberdevice;
        FeatureSet TransformerfeatureSet;
        ArrayList pdfobjList = new ArrayList();
        ObservableCollection<Pdfcustomerdata> objcoll = new ObservableCollection<Pdfcustomerdata>();
        ObservableCollection<Pdfcustomerdata> CustomerResult = new ObservableCollection<Pdfcustomerdata>();
        ObservableCollection<Pdfcustomerdata> CustomerList = new ObservableCollection<Pdfcustomerdata>();
        ObservableCollection<Addcustomerdata> AddCustomerResult = new ObservableCollection<Addcustomerdata>();
        ObservableCollection<Addcustomerdata> AddCustomerList = new ObservableCollection<Addcustomerdata>();
        
       // string ServicePointLayerID = "-1";
        string cgs = string.Empty;
        string strDivisionCode = string.Empty;
        string strcircuit = string.Empty;
        public int BufferDistance;
        private string _GeometryService;
        private string _CircuitIDService;
        string strStartdevicePointID = string.Empty;
        //Read from Page.config

        private string _TransformerService = string.Empty;
        private string _PrimaryMeterService = "";
        private string _PrimaryMeterSSD = "";
        private string _DivisionService;
        private string _SubstationService = string.Empty;
        private string _CircuitService = string.Empty;
        private string _TracingNetworkService = string.Empty;
        private string _ServicepointIDService = string.Empty;
        private string _ServiceLocationService = string.Empty;
        string _WCFService_URL = "";
        private int _Switch_FEATURE_FCID;
        private int _Dynamicprotectivedevice_FEATURE_FCID;
        private int _FUSE_FEATURE_FCID;
        private int _Transformer_FEATURE_FCID;
        private int _PrimaryMeter_FEATURE_FCID;
        private int _OpenPoint_FEATURE_FCID;
        private int _PriUGConductor_FEATURE_FCID;
        private int _PrimaryOverheadConductor_FEATURE_FCID;
        private string _ServiceLocation_FEATURE_FCID;
        private double _PONSPolyScale;
        Ponsservicelayercall objPonsServiceCall;
        GeometryService _geometryService;
        QueryTask _queryTask;
        public bool ClearSession = false;
        Dictionary<string, string> CGCTNumMapping = new Dictionary<string, string>();

         //Polygon Selection
         private GraphicsLayer _graphicsLayer;
         private Draw _drawControl = null;
         private Polygon _polygon;
         private ESRI.ArcGIS.Client.Symbols.SimpleFillSymbol _fillSymbol;
         public double AreaTotal { get; set; }
         private Size _textSize;
         private TextSymbol _textSymbol;
         private readonly ESRI.ArcGIS.Client.Symbols.SimpleMarkerSymbol _markerSymbol;
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
         public string PONS_GRAPHICS_LAYER = "PONSGraphics";
         private GeometryService _geometryServicePolygon;
         public bool isPolygonDrawn = false;
         private const string PONS_POLYGON_GRAPHIC_LAYER = "PONSPolygonGraphics";

         //ad hoc print
         public string EDMasterLayers { get; set; }
         public string SelectedStoredDisplayName { get; set; }
         private Map _ponsAdHocMap = new Map();
         private PGnE.Printing.HiResPrint _hiResPrint = null;
        //Add -Jan2018
         private int SelectAllcustomercount;
         private int SelectAllMetercustomercount;
         public int customercount_checkbox;
         ObservableCollection<Pdfcustomerdata> SelectAllCustomerResult = new ObservableCollection<Pdfcustomerdata>();
         ObservableCollection<Pdfcustomerdata> SelectAllCustomerList = new ObservableCollection<Pdfcustomerdata>();
        public PONSDialog(Map map)
        {
            string strtomorrowdate = "";

            try
            {
                InitializeComponent();
                _map = map;
                LoadLayerConfiguration();
                LoadConfiguration();
                Binddivision();
                IntializeSubstationLayer();
                //Adding code to initialise PONS service
              
                objPonsServiceCall = new Ponsservicelayercall();
                string prefixUrl = objPonsServiceCall.GetPrefixUrl();
                endpointaddress = prefixUrl + _WCFService_URL;

                objPonsServiceCall = new Ponsservicelayercall();
                string strPSLURL = objPonsServiceCall.GetPSLDataAddress();
                GetPSLContactData(strPSLURL);

                DateTime today = DateTime.Now;
                DateTime tomorrow = today.AddDays(1);
                strtomorrowdate = Convert.ToString((tomorrow.Year) + "/" + (tomorrow.Month) + "/" + (tomorrow.Day));
                datePickerFstShutDateOff.Text = strtomorrowdate;
                datePickerFstShutDateOff.BlackoutDates.Add(new CalendarDateRange(new DateTime(0001, 1, 1), DateTime.Now));
                datePickerScndOff.BlackoutDates.Add(new CalendarDateRange(new DateTime(0001, 1, 1), DateTime.Now));

                childWindow.Closed += new EventHandler(childWindow_Closed);
                //Polygon Selection
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
                _geometryServicePolygon = new GeometryService(_GeometryService);
            }
            catch (Exception ex)
            {
                logger.FatalException("Error loading configuration: ", ex);
            }
        }

        private void btnCancelTab1_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            _fw.Visibility = Visibility.Collapsed;
            ClearSession = true;
            dddivisionSearch.IsEnabled = true;
            DisableDrawPolygon();
        }

        private void OKButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            try
            {
                double currentScale = Math.Round(_map.Resolution * 96);
                if (dddivisionSearch.SelectedIndex == 0 && currentScale > _PONSPolyScale)
                {
                    MessageBox.Show("Please either select division or zoom to scale <= " + _PONSPolyScale.ToString() + "");
                    return;
                }
                ddsubstation.Items.Clear();
                ddbank.Items.Clear();
                ddcircuit.Items.Clear();
                dddivisionSearch.IsEnabled = false;
                string sWhere = dddivisionSearch.SelectedItem.ToString();
                int intDivCode = GetDivisionCodes(sWhere);
                string strDivCode = intDivCode.ToString();
                objPonsServiceCall = new Ponsservicelayercall();
                string strPSLURL = objPonsServiceCall.GetCircuitIDBankSubstation(strDivCode);
                GetSubstationBankCircuitData(strPSLURL);
               
                tabItem1.Visibility = Visibility.Collapsed;
                tabItem2.Visibility = Visibility.Visible;
                tabItem4.Visibility = Visibility.Collapsed;
                tabItem5.Visibility = Visibility.Collapsed;
                tabItem6.Visibility = Visibility.Collapsed;
                tabItem7.Visibility = Visibility.Collapsed;
                tabItem8.Visibility = Visibility.Collapsed;
                tabItem9.Visibility = Visibility.Collapsed;
                btnNext.IsEnabled = true;
                //change code-AfterUAT
                RdBetweenDevice.IsChecked = false;   //changed after map selection option added
                RdBetweenDevice.IsChecked = true;
                DeviceEntry.Visibility = Visibility.Visible;
                BtnBeyond.Visibility = Visibility.Visible;
                stacktrans.Visibility = Visibility.Collapsed;
                DeviceEntryTransformer.Visibility = Visibility.Collapsed;
                stackcircuit.Visibility = Visibility.Collapsed;
                stackcircuit1.Visibility = Visibility.Collapsed;
                if (SearchlistDataGrid.SelectedItems.Count > 0)
                {
                    btnDeleteGetCustomer.IsEnabled = true;
                }
                else
                {
                    btnDeleteGetCustomer.IsEnabled = false;
                }
                timePickerFrstOff.Value = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 12, 00, 00);
                timePickerFrstOn.Value = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 12, 30, 00);
                timePickerScndOff.Value = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 12, 00, 00);
                timePickerScndOn.Value = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 12, 30, 00);
                DateTime selecteddate = datePickerFstShutDateOff.SelectedDate.Value;
                string strtomorrowdate = Convert.ToString(selecteddate.Month) + "/" + Convert.ToString(selecteddate.Day) + "/" + Convert.ToString(selecteddate.Year);
                datePickerScndOff.Text = strtomorrowdate;
                _fw.Width = 660;
                _fw.Height = 455;
                //map selection  -   validations based on scale and division
                SearchCancelButton.IsEnabled = true;
                DeviceButton.IsEnabled = true;
                if (dddivisionSearch.SelectedIndex == 0 && currentScale <= _PONSPolyScale)
                {
                    RdSelectMap.IsChecked = false;
                    RdSelectMap.IsChecked = true;
                    RdBetweenDevice.IsEnabled = false;
                    RdTranformer.IsEnabled = false;
                    RdCircuit.IsEnabled = false;
                    RdSelectMap.IsEnabled = true;
                }
                else if (dddivisionSearch.SelectedIndex > 0 && currentScale > _PONSPolyScale)
                {
                    RdBetweenDevice.IsEnabled = true;
                    RdTranformer.IsEnabled = true;
                    RdCircuit.IsEnabled = true;
                    RdSelectMap.IsEnabled = false;
                }
                else if (dddivisionSearch.SelectedIndex > 0 && currentScale <= _PONSPolyScale)
                {
                    RdBetweenDevice.IsEnabled = true;
                    RdTranformer.IsEnabled = true;
                    RdCircuit.IsEnabled = true;
                    RdSelectMap.IsEnabled = true;
                }
            }
            catch (Exception ex)
            {
                logger.FatalException("Error in PONS in adding search: ", ex);
            }

        }

        private void btnNextEmail_Click(object sender, System.Windows.RoutedEventArgs e)
        {

            if (ch2Report.IsChecked == false && ch3backletter.IsChecked == false && ch5Report.IsChecked == false && ch4Map.IsChecked == false)
            {
                MessageBox.Show("No item has selected, please click on previous to select the option.");
                btnNext.IsEnabled = false;
            }
            if (ch2Report.IsChecked == true)
            {
                tabItem1.Visibility = Visibility.Collapsed;
                tabItem2.Visibility = Visibility.Collapsed;
                tabItem4.Visibility = Visibility.Collapsed;
                tabItem5.Visibility = Visibility.Collapsed;
                tabItem6.Visibility = Visibility.Collapsed;
                tabItem7.Visibility = Visibility.Collapsed;
                tabItem8.Visibility = Visibility.Visible;
                tabItem9.Visibility = Visibility.Collapsed;
            }
            else if (ch5Report.IsChecked == true)
            {
                tabItem1.Visibility = Visibility.Collapsed;
                tabItem2.Visibility = Visibility.Collapsed;
                tabItem4.Visibility = Visibility.Collapsed;
                tabItem5.Visibility = Visibility.Collapsed;
                tabItem6.Visibility = Visibility.Collapsed;
                tabItem7.Visibility = Visibility.Collapsed;
                tabItem8.Visibility = Visibility.Visible;
                tabItem9.Visibility = Visibility.Collapsed;

            }
            else if (ch4Map.IsChecked == true)
            {
                tabItem1.Visibility = Visibility.Collapsed;
                tabItem2.Visibility = Visibility.Collapsed;
                tabItem4.Visibility = Visibility.Collapsed;
                tabItem5.Visibility = Visibility.Collapsed;
                tabItem6.Visibility = Visibility.Collapsed;
                tabItem7.Visibility = Visibility.Collapsed;
                tabItem8.Visibility = Visibility.Collapsed;
                tabItem10.Visibility = Visibility.Visible;
                tabItem9.Visibility = Visibility.Collapsed;

            }
            else if (ch3backletter.IsChecked == true)
            {
                tabItem1.Visibility = Visibility.Collapsed;
                tabItem2.Visibility = Visibility.Collapsed;
                tabItem4.Visibility = Visibility.Collapsed;
                tabItem5.Visibility = Visibility.Collapsed;
                tabItem6.Visibility = Visibility.Collapsed;
                tabItem7.Visibility = Visibility.Collapsed;
                tabItem8.Visibility = Visibility.Collapsed;
                tabItem9.Visibility = Visibility.Visible;
            }

            SetTabFocus(btnPrintCustList);

        }
        //Search close
        private void SearchCancelButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            txtTransformer.Text = string.Empty;
            txt_StartDevice.Text = string.Empty;
            txt_EndDevice.Text = string.Empty;

            if (ddsubstation.Items.Count > 0)
                ddsubstation.SelectedIndex = 0;

            if (ddbank.Items.Count > 0)
            {
                ddbank.Items.Clear();
                ddbank.Items.Add("Select...");
            }

            if (ddcircuit.Items.Count > 0)
            {
                ddcircuit.Items.Clear();
                ddcircuit.Items.Add("Select...");
            }

            //map selection
            if (RdSelectMap.IsChecked == true)
            {
                ClearLayers();                
                SelectPolygonEvent(false);
            }

        }
        public void GetFloatingWindowHeight(FloatableWindow fw)
        {
            _fw = fw;
        }
        // For close the window  20.10.2015
        public void Stoppreviouspage()
        {
            if (ClearSession)
            {
                tabItem1.Visibility = Visibility.Visible;
                tabItem2.Visibility = Visibility.Collapsed;
                tabItem4.Visibility = Visibility.Collapsed;
                tabItem5.Visibility = Visibility.Collapsed;
                tabItem6.Visibility = Visibility.Collapsed;
                tabItem7.Visibility = Visibility.Collapsed;
                tabItem8.Visibility = Visibility.Collapsed;
                tabItem9.Visibility = Visibility.Collapsed;
                _fw.Width = 660;
                _fw.Height = 455;               
                ClearSession = false;
                Clearallvalue();
                dddivisionSearch.IsEnabled = true;
                SetTabFocus(dddivisionSearch); 
            }
        }
        //Device search vaildation
        void BindDevicesearchdata()
        {
            if (RdBetweenDevice.IsChecked == true)
            {
                SearchType = "1";
                if (txt_StartDevice.Text.Trim() == "" && txt_EndDevice.Text.Trim() == "")
                {
                    SetTabFocus(RdBetweenDevice);
                    MessageBox.Show("Start Device is mandatory.");
                    SetTabFocus(txt_StartDevice);
                }
                else
                {
                    if (txt_StartDevice.Text.Trim() == "" && txt_EndDevice.Text.Trim() != "")
                    {
                        SetTabFocus(RdBetweenDevice);
                        MessageBox.Show("Start Device is mandatory.");
                        SetTabFocus(txt_StartDevice);
                    }
                    else
                    {
                        Bindstartdevice();
                    }
                }
            }
            GetCustomer.Visibility = Visibility.Visible;
        }
        //Device search 
        void Bindstartdevice()
        {
            string startdevice = "";
            string EndDevice = "";
            try
            {
                if (txt_StartDevice.Text.Trim() != "")
                {
                    startdevice = txt_StartDevice.Text.Trim().ToString().ToUpper();
                }
                if (txt_EndDevice.Text.Trim() != "")
                {
                    EndDevice = txt_EndDevice.Text.Trim().ToString().ToUpper();
                }
                if (startdevice == EndDevice)
                {
                    EndDevice = string.Empty;
                }
                busyIndicator.IsBusy = true;
                busyIndicator.BusyContent = "Fetching Data...";
                RdBetweenDevice.IsChecked = true;
                objPonsServiceCall = new Ponsservicelayercall();
                // Adding code to check the # character
                if (startdevice.Contains('#') )
                {
                    startdevice = startdevice.Replace('#', '^');
                }
                if (EndDevice.Contains('#'))
                {
                    EndDevice = EndDevice.Replace('#', '^');
                }

                string strDeviceSearchURL = objPonsServiceCall.GetDevicedataCustomerSearchURL(startdevice, EndDevice, Divisioncode);
                GetDeviceSearchOne(strDeviceSearchURL);
            }
            catch (Exception ex)
            {
                busyIndicator_Customer.IsBusy = false;
                busyIndicator.IsBusy = false;
                logger.FatalException("Error in PONS in Bindstartdevice search: ", ex);
            }
        }

        //Start Device & End device
        #region Start Device & End device Search

        public void GetDeviceSearchOne(string strURL)
        {
            try
            {
                WebClient client = new WebClient();
                UriBuilder uriBuilder = new UriBuilder(strURL);
                client.DownloadStringCompleted += new DownloadStringCompletedEventHandler(Client_Devicestartcusdata);
                client.DownloadStringAsync(uriBuilder.Uri);
            }
            catch (Exception ex)
            {
                busyIndicator_Customer.IsBusy = false;
                busyIndicator.IsBusy = false;
                logger.FatalException("Error in GetDeviceSearchOne", ex);
            }
        }
        //search Device in diffrent condition
        void Client_Devicestartcusdata(object sender, DownloadStringCompletedEventArgs e)
        {
           
            childWindow.ED_Count = 0;
            childWindow.SD_Count = 0;
            string startdevice = txt_StartDevice.Text.Trim().ToString().ToUpper();
            string EndDevice = txt_EndDevice.Text.Trim().ToString().ToUpper();
            if (startdevice == EndDevice)
            {
                EndDevice = string.Empty;
            }
            List<Devicestartcustomerdata> objShutIDResult = new List<Devicestartcustomerdata>();
            List<string> objStartdeviceIDResult = new List<string>();
            string result = e.Result;
            if (!string.IsNullOrEmpty(result))
            {
                objShutIDResult = JsonHelper.Deserialize<List<Devicestartcustomerdata>>(result);
                try
                {
                    var Deviceinfo = objShutIDResult;
                    if (Deviceinfo.Count > 0)
                    {
                        //Start
                        var startdevicequery_where1 = from a in Deviceinfo
                                                      where a.OPERATINGNUMBER == startdevice.ToString()
                                                      select new
                                                      {
                                                          _circuit = a.circuitid,
                                                          _circuit2 = a.circuitid2,
                                                          _GUID = a.globalid,
                                                          _objectid = a.objectid,
                                                          _operation = a.OPERATINGNUMBER,
                                                          _Devicetype = a.DeviceName
                                                      };

                        int devicecount = startdevicequery_where1.Count();

                        if (devicecount == 0)
                        {
                            busyIndicator.IsBusy = false;
                            MessageBox.Show("Start Device not found.");
                            RdBetweenDevice.IsChecked = false;
                            RdBetweenDevice.IsChecked = true;
                            return;
                        }

                        //End
                        var Enddevicequery_where1 = from a in Deviceinfo
                                                    where a.OPERATINGNUMBER.Equals(EndDevice.ToString())
                                                    select a;
                        int Enddevicecount = Enddevicequery_where1.Count();

                        if (Enddevicecount == 0 && EndDevice != string.Empty)
                        {
                            busyIndicator.IsBusy = false;
                            SetTabFocus(RdBetweenDevice);
                            MessageBox.Show("End Device not found.");
                            SetTabFocus(txt_StartDevice);
                            RdBetweenDevice.IsChecked = false;
                            RdBetweenDevice.IsChecked = true;
                            return;
                        }

                        //only start device
                        if (EndDevice == "")
                        {
                            if (startdevicequery_where1.Count() > 0)
                            {

                                if (startdevicequery_where1.Count() == 1)
                                {
                                    StartDeviceName = startdevicequery_where1.ElementAt(0)._Devicetype;
                                    StartDeviceFeederID = startdevicequery_where1.ElementAt(0)._circuit.ToString();
                                    StartDeviceCircuit2 = startdevicequery_where1.ElementAt(0)._circuit2.ToString();
                                    startdeviceID = startdevicequery_where1.ElementAt(0)._operation.ToString();
                                    StartDeviceGUID = startdevicequery_where1.ElementAt(0)._GUID.ToString();
                                    StartdeviceobjectID = startdevicequery_where1.ElementAt(0)._objectid.ToString();
                                    SearchlistDataGrid.ItemsSource = LoadCollectionData();
                                    var DistinctItems = Searchdevice.GroupBy(x => x.DeviceID).Select(y => y.First());
                                    SearchlistDataGrid.ItemsSource = null;
                                    ThreadPool.QueueUserWorkItem((state) =>
                                    {
                                        SearchlistDataGrid.Dispatcher.BeginInvoke(() => SearchlistDataGrid.ItemsSource = DistinctItems.ToList());
                                    });
                                }

                                if (startdevicequery_where1.Count() > 1)
                                {
                                    busyIndicator.IsBusy = true ;
                                    RdBetweenDevice.IsChecked = false;
                                    RdBetweenDevice.IsChecked = true;
                                    childWindow.Rdtype = 1;
                                    childWindow.GetChDivisionnumber = Divisioncode;
                                    childWindow.GetChOPERATINGNUMBER = txt_StartDevice.Text.Trim().ToString().ToUpper();
                                    childWindow.GetChEndOPERATINGNUMBER = EndDevice;
                                    childWindow.SearchlistCircuitDataGrid.Visibility = Visibility.Collapsed;
                                    childWindow.SearchlistDataGrid.Visibility = Visibility.Collapsed;
                                    childWindow.AdddataButton.Visibility = Visibility.Collapsed;
                                    childWindow.OKButton.Visibility = Visibility.Collapsed;
                                    childWindow.Show();
                                }
                            }
                            else
                            {
                                    busyIndicator.IsBusy = false;
                                    SetTabFocus(RdBetweenDevice);
                                    MessageBox.Show("No Device found.");
                                    SetTabFocus(txt_StartDevice);
                                    RdBetweenDevice.IsChecked = false;
                                    RdBetweenDevice.IsChecked = true;
                            }
                        }
                        //Start and End device
                        else if (txt_StartDevice.Text.Trim() != "" && EndDevice != "")
                        {
                            if (Enddevicequery_where1.Count() > 0)
                            {
                                //End Device
                                EndDeviceName = Enddevicequery_where1.ElementAt(0).DeviceName;
                                EndDeviceFeederID = Enddevicequery_where1.ElementAt(0).circuitid.ToString();
                                EnddeviceID = Enddevicequery_where1.ElementAt(0).OPERATINGNUMBER.ToString();
                                EndDeviceGUID = Enddevicequery_where1.ElementAt(0).globalid.ToString();
                                ENddeviceobjectID = Enddevicequery_where1.ElementAt(0).objectid.ToString();
                                EndDeviceCircuit2 = Enddevicequery_where1.ElementAt(0).circuitid2.ToString();
                                //Start Device

                                StartDeviceName = startdevicequery_where1.ElementAt(0)._Devicetype;
                                StartDeviceFeederID = startdevicequery_where1.ElementAt(0)._circuit.ToString();
                                StartDeviceCircuit2 = startdevicequery_where1.ElementAt(0)._circuit2.ToString();
                                startdeviceID = startdevicequery_where1.ElementAt(0)._operation.ToString();
                                StartDeviceGUID = startdevicequery_where1.ElementAt(0)._GUID.ToString();
                                StartdeviceobjectID = startdevicequery_where1.ElementAt(0)._objectid.ToString();
                                //Check Both Feeder Id 
                                if (startdevicequery_where1.Count() == 1 && Enddevicequery_where1.Count() == 1)
                                {
                                    if (StartDeviceFeederID == EndDeviceFeederID || StartDeviceFeederID == EndDeviceCircuit2 || StartDeviceCircuit2 == EndDeviceFeederID)
                                    {
                                        SearchlistDataGrid.ItemsSource = LoadCollectionData();
                                        var DistinctItems = Searchdevice.GroupBy(x => x.DeviceID).Select(y => y.First());
                                        SearchlistDataGrid.ItemsSource = null;
                                        ThreadPool.QueueUserWorkItem((state) =>
                                        {
                                            SearchlistDataGrid.Dispatcher.BeginInvoke(() => SearchlistDataGrid.ItemsSource = DistinctItems.ToList());
                                        });
                                    }
                                    else
                                    {
                                        SetTabFocus(RdBetweenDevice);
                                        MessageBox.Show("Please select Start and End device on same feeder");
                                        SetTabFocus(txt_StartDevice);                                        
                                        busyIndicator.IsBusy = false;
                                    }

                                }
                                //Check  more than one start device
                                else if (startdevicequery_where1.Count() > 1 && Enddevicequery_where1.Count() == 1)
                                {

                                    //Change code-13/01/2016
                                    //Check feeder ID in both device
                                    EndDeviceFeederID = Enddevicequery_where1.ElementAt(0).circuitid.ToString();
                                    var objstadevicequery = from a in startdevicequery_where1
                                                            where a._circuit.Equals(EndDeviceFeederID.ToString()) || a._circuit.Equals(EndDeviceCircuit2.ToString())
                                                            select a;
                                    int objcount = objstadevicequery.ToList().Count;
                                    if (objcount == 1)
                                    {

                                        StartDeviceName = objstadevicequery.ElementAt(0)._Devicetype;
                                        StartDeviceFeederID = objstadevicequery.ElementAt(0)._circuit.ToString();
                                        startdeviceID = objstadevicequery.ElementAt(0)._operation.ToString();
                                        StartDeviceGUID = objstadevicequery.ElementAt(0)._GUID.ToString();
                                        StartdeviceobjectID = objstadevicequery.ElementAt(0)._objectid.ToString();
                                        SearchlistDataGrid.ItemsSource = LoadCollectionData();
                                        var DistinctItems = Searchdevice.GroupBy(x => x.DeviceID).Select(y => y.First());
                                        SearchlistDataGrid.ItemsSource = null;
                                        ThreadPool.QueueUserWorkItem((state) =>
                                        {
                                            SearchlistDataGrid.Dispatcher.BeginInvoke(() => SearchlistDataGrid.ItemsSource = DistinctItems.ToList());
                                        });
                                    }
                                    else
                                    {
                                        busyIndicator.IsBusy = true;
                                        RdBetweenDevice.IsChecked = true;
                                        //End Device
                                        EndDeviceName = Enddevicequery_where1.ElementAt(0).DeviceName;
                                        EndDeviceFeederID = Enddevicequery_where1.ElementAt(0).circuitid.ToString();
                                        EnddeviceID = Enddevicequery_where1.ElementAt(0).OPERATINGNUMBER.ToString();
                                        EndDeviceGUID = Enddevicequery_where1.ElementAt(0).globalid.ToString();
                                        ENddeviceobjectID = Enddevicequery_where1.ElementAt(0).objectid.ToString();
                                        EndDeviceCircuit2 = Enddevicequery_where1.ElementAt(0).circuitid2.ToString();
                                        childWindow.Rdtype = 1;
                                        childWindow.SD_Count = startdevicequery_where1.Count();
                                        childWindow.ED_Count = Enddevicequery_where1.Count();
                                        childWindow.GetChDivisionnumber = Divisioncode;
                                        childWindow.GetChOPERATINGNUMBER = txt_StartDevice.Text.Trim().ToString().ToUpper();
                                        childWindow.GetChEndOPERATINGNUMBER = txt_EndDevice.Text.Trim().ToString().ToUpper();
                                        childWindow.SearchlistCircuitDataGrid.Visibility = Visibility.Collapsed;
                                        childWindow.SearchlistDataGrid.Visibility = Visibility.Collapsed;
                                        childWindow.AdddataButton.Visibility = Visibility.Collapsed;
                                        childWindow.OKButton.Visibility = Visibility.Collapsed;
                                        childWindow.SearchlistEndDeviceDataGrid.Visibility = Visibility.Collapsed;
                                        childWindow.SearchlistDeviceDataGrid.Visibility = Visibility.Visible;
                                        childWindow.Show();
                                    }
                                }
                                //Check  more than one end device
                                else if (startdevicequery_where1.Count() == 1 && Enddevicequery_where1.Count() > 1)
                                {

                                    //Change logic for feeder-13/01/2016
                                    StartDeviceFeederID = startdevicequery_where1.ElementAt(0)._circuit.ToString();
                                    var Enddevicequery = from a in Enddevicequery_where1
                                                         where a.circuitid.Equals(StartDeviceFeederID.ToString())
                                                         select a;
                                    int objEnddevicecount = Enddevicequery.ToList().Count();
                                    if (objEnddevicecount == 1)
                                    {
                                        EndDeviceName = Enddevicequery.ElementAt(0).DeviceName;
                                        EndDeviceFeederID = Enddevicequery.ElementAt(0).circuitid.ToString();
                                        EnddeviceID = Enddevicequery.ElementAt(0).OPERATINGNUMBER.ToString();
                                        EndDeviceGUID = Enddevicequery.ElementAt(0).globalid.ToString();
                                        ENddeviceobjectID = Enddevicequery.ElementAt(0).objectid.ToString();
                                        EndDeviceCircuit2 = Enddevicequery_where1.ElementAt(0).circuitid2.ToString();
                                        SearchlistDataGrid.ItemsSource = LoadCollectionData();
                                        var DistinctItems = Searchdevice.GroupBy(x => x.DeviceID).Select(y => y.First());
                                        SearchlistDataGrid.ItemsSource = null;
                                        ThreadPool.QueueUserWorkItem((state) =>
                                        {
                                            SearchlistDataGrid.Dispatcher.BeginInvoke(() => SearchlistDataGrid.ItemsSource = DistinctItems.ToList());
                                        });
                                    }
                                    else
                                    {
                                        //busyIndicator.IsBusy = true;
                                        RdBetweenDevice.IsChecked = true;
                                        //Start Device

                                        StartDeviceName = startdevicequery_where1.ElementAt(0)._Devicetype;
                                        StartDeviceFeederID = startdevicequery_where1.ElementAt(0)._circuit.ToString();
                                        StartDeviceCircuit2 = startdevicequery_where1.ElementAt(0)._circuit2.ToString();
                                        startdeviceID = startdevicequery_where1.ElementAt(0)._operation.ToString();
                                        StartDeviceGUID = startdevicequery_where1.ElementAt(0)._GUID.ToString();
                                        StartdeviceobjectID = startdevicequery_where1.ElementAt(0)._objectid.ToString();
                                        childWindow.Rdtype = 1;
                                        childWindow.GetChDivisionnumber = Divisioncode;
                                        childWindow.SD_Count = startdevicequery_where1.Count();
                                        childWindow.ED_Count = Enddevicequery_where1.Count();
                                        childWindow.GetChOPERATINGNUMBER = txt_StartDevice.Text.Trim().ToString().ToUpper();
                                        childWindow.GetChEndOPERATINGNUMBER = EndDevice;
                                        childWindow.SearchlistCircuitDataGrid.Visibility = Visibility.Collapsed;
                                        childWindow.SearchlistDataGrid.Visibility = Visibility.Collapsed;
                                        childWindow.AdddataButton.Visibility = Visibility.Collapsed;
                                        childWindow.OKButton.Visibility = Visibility.Collapsed;
                                        childWindow.SearchlistDeviceDataGrid.Visibility = Visibility.Collapsed;
                                        childWindow.SearchlistEndDeviceDataGrid.Visibility = Visibility.Visible;
                                        childWindow.Show();
                                    }
                                }
                                //Check  more than one end device and start device
                                else if (startdevicequery_where1.Count() > 1 || Enddevicequery_where1.Count() > 1)
                                {
                                    RdBetweenDevice.IsChecked = true;
                                    childWindow.Rdtype = 1;
                                    childWindow.GetChDivisionnumber = Divisioncode;
                                    childWindow.SD_Count = startdevicequery_where1.Count();
                                    childWindow.ED_Count = Enddevicequery_where1.Count();
                                    childWindow.GetChOPERATINGNUMBER = txt_StartDevice.Text.Trim().ToString().ToUpper();
                                    childWindow.GetChEndOPERATINGNUMBER = EndDevice;
                                    childWindow.SearchlistCircuitDataGrid.Visibility = Visibility.Collapsed;
                                    childWindow.SearchlistDataGrid.Visibility = Visibility.Collapsed;
                                    childWindow.AdddataButton.Visibility = Visibility.Collapsed;
                                    childWindow.OKButton.Visibility = Visibility.Collapsed;
                                    childWindow.SearchlistDeviceDataGrid.Visibility = Visibility.Visible;
                                    childWindow.SearchlistEndDeviceDataGrid.Visibility = Visibility.Visible;
                                    childWindow.Show();
                                }
                            }
                            else
                            {
                                SetTabFocus(RdBetweenDevice);
                                MessageBox.Show("No Device found.");
                                SetTabFocus(txt_StartDevice);
                                busyIndicator.IsBusy = false;
                                RdBetweenDevice.IsChecked = false;
                                RdBetweenDevice.IsChecked = true;
                            }
                        }
                    }
                    else
                    {
                        SetTabFocus(RdBetweenDevice);
                        MessageBox.Show("No Device found.");
                        SetTabFocus(txt_StartDevice);
                        busyIndicator_Customer.IsBusy = false;
                        busyIndicator.IsBusy = false;
                        RdBetweenDevice.IsChecked = false;
                        RdBetweenDevice.IsChecked = true;
                    }
                }
                catch (Exception ex)
                {
                    busyIndicator.IsBusy = false;
                    logger.FatalException("Error in PONS in  search: ", ex);
                }
            }
            else
            {                
                busyIndicator_Customer.IsBusy = false;
                busyIndicator.IsBusy = false;
                RdBetweenDevice.IsChecked = false;
                RdBetweenDevice.IsChecked = true;
                SetTabFocus(RdBetweenDevice);
                MessageBox.Show("No Device found.");
                SetTabFocus(txt_StartDevice);
            }
        }
        # endregion
        //Change -21/12/2015
        private void DeviceButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            try
            {
                if (RdBetweenDevice.IsChecked == false && RdTranformer.IsChecked == false && RdCircuit.IsChecked == false && RdSelectMap.IsChecked == false)
                {
                    MessageBox.Show("Please select the search option");
                    return;
                }
                if (RdBetweenDevice.IsChecked == true)
                {
                    //bool chtextbox = (ConfigUtility.HasSpecialCharacters(txt_StartDevice.Text.Trim()) || ConfigUtility.HasSpecialCharacters(txt_EndDevice.Text.Trim()));
                    //if (chtextbox == false)
                    {
                        BindDevicesearchdata();
                        RdBetweenDevice.IsChecked = true;
                    }
                }
                else if (RdTranformer.IsChecked == true)
                {
                    //bool chtextbox = ConfigUtility.HasSpecialCharacters(txtTransformer.Text.Trim());
                    //if (chtextbox == false)
                    //{
                    if (txtTransformer.Text.Trim() != "")
                    {
                        BindPrimaryMeterSearchdata();
                        tabItem1.Visibility = Visibility.Collapsed;
                        tabItem2.Visibility = Visibility.Visible;
                        tabItem4.Visibility = Visibility.Collapsed;
                        tabItem5.Visibility = Visibility.Collapsed;
                        tabItem6.Visibility = Visibility.Collapsed;
                        tabItem7.Visibility = Visibility.Collapsed;
                        tabItem8.Visibility = Visibility.Collapsed;
                        tabItem9.Visibility = Visibility.Collapsed;
                    }
                    else
                    {
                        MessageBox.Show("Please enter CGC number/TNum");
                    }
                    //}
                    //else
                    //{
                    //    MessageBox.Show("Please enter correct format");
                    //}
                }
                else if (RdCircuit.IsChecked == true)
                {
                    if (ddsubstation.SelectedItem != null && ddsubstation.SelectedItem.ToString() == "Select...")
                    {
                        MessageBox.Show("Please select a Substation.");
                        return;
                    }
                    if (ddcircuit.Items.Count > 1)
                    {
                        if (ddcircuit.SelectedItem.ToString() != "Select...")
                        {
                            SubstationCircuitsearch();
                        }
                        else
                        {
                            MessageBox.Show("Please select a circuit.");
                            return;
                        }
                    }
                    tabItem1.Visibility = Visibility.Collapsed;
                    tabItem2.Visibility = Visibility.Visible;
                    tabItem4.Visibility = Visibility.Collapsed;
                    tabItem5.Visibility = Visibility.Collapsed;
                    tabItem6.Visibility = Visibility.Collapsed;
                    tabItem7.Visibility = Visibility.Collapsed;
                    tabItem8.Visibility = Visibility.Collapsed;
                    tabItem9.Visibility = Visibility.Collapsed;
                }

                //Polygon Selection
                else if (RdSelectMap.IsChecked == true)
                {
                    if (_polygon == null)
                    {
                        MessageBox.Show("Please draw a polygon on map.");
                        return;
                    }
                    SearchType = "4";
                    PolygonSearch();

                    tabItem1.Visibility = Visibility.Collapsed;
                    tabItem2.Visibility = Visibility.Visible;
                    tabItem4.Visibility = Visibility.Collapsed;
                    tabItem5.Visibility = Visibility.Collapsed;
                    tabItem6.Visibility = Visibility.Collapsed;
                    tabItem7.Visibility = Visibility.Collapsed;
                    tabItem8.Visibility = Visibility.Collapsed;
                    tabItem9.Visibility = Visibility.Collapsed;
                } 

            }
            catch (Exception ex)
            {
                _fw.Height = 455;
                CustWiz.Height = 455;
                tabItem1.Visibility = Visibility.Collapsed;
                tabItem2.Visibility = Visibility.Visible;
                tabItem4.Visibility = Visibility.Collapsed;
                tabItem5.Visibility = Visibility.Collapsed;
                tabItem6.Visibility = Visibility.Collapsed;
                tabItem7.Visibility = Visibility.Collapsed;
                tabItem8.Visibility = Visibility.Collapsed;
                tabItem9.Visibility = Visibility.Collapsed;
                busyIndicator_Customer.IsBusy = false;
                logger.FatalException("Error in PONS in DeviceButton_Click search: ", ex);
            }
        }

        //search tool
        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            if (RdBetweenDevice != null)
            {
                if (RdBetweenDevice.IsChecked == true)
                {
                    DeviceEntry.Visibility = Visibility.Visible;
                    BtnBeyond.Visibility = Visibility.Visible;
                    stacktrans.Visibility = Visibility.Collapsed;
                    DeviceEntryTransformer.Visibility = Visibility.Collapsed;
                    stackcircuit.Visibility = Visibility.Collapsed;
                    stackcircuit1.Visibility = Visibility.Collapsed;
                    txtTransformer.Text = string.Empty;
                    if (ddsubstation.SelectedIndex > -1)
                        ddsubstation.SelectedIndex = 0;
                    ddbank.Items.Clear();
                    ddbank.Items.Add("Select...");
                    if (ddbank.SelectedIndex > -1)
                        ddbank.SelectedIndex = 0;
                    ddcircuit.Items.Clear();
                    ddcircuit.Items.Add("Select...");
                    if (ddcircuit.SelectedIndex > -1)
                        ddcircuit.SelectedIndex = 0;
                    SelectPolygonEvent(false);
                    SetTabFocus(txt_StartDevice);
                }
                if (RdTranformer.IsChecked == true)
                {
                    DeviceEntry.Visibility = Visibility.Collapsed;
                    BtnBeyond.Visibility = Visibility.Collapsed;
                    stacktrans.Visibility = Visibility.Visible;
                    DeviceEntryTransformer.Visibility = Visibility.Visible;
                    stackcircuit.Visibility = Visibility.Collapsed;
                    stackcircuit1.Visibility = Visibility.Collapsed;
                    txt_StartDevice.Text = string.Empty;
                    txt_EndDevice.Text = string.Empty;
                    if (ddsubstation.SelectedIndex > -1)
                        ddsubstation.SelectedIndex = 0;
                    ddbank.Items.Clear();
                    ddbank.Items.Add("Select...");
                    if (ddbank.SelectedIndex > -1)
                        ddbank.SelectedIndex = 0;
                    ddcircuit.Items.Clear();
                    ddcircuit.Items.Add("Select...");
                    if (ddcircuit.SelectedIndex > -1)
                        ddcircuit.SelectedIndex = 0;
                    SelectPolygonEvent(false);
                    SetTabFocus(txtTransformer);
                }
                if (RdCircuit.IsChecked == true)
                {
                    DeviceEntry.Visibility = Visibility.Collapsed;
                    stacktrans.Visibility = Visibility.Collapsed;
                    BtnBeyond.Visibility = Visibility.Collapsed;
                    DeviceEntryTransformer.Visibility = Visibility.Collapsed;
                    stackcircuit.Visibility = Visibility.Visible;
                    stackcircuit1.Visibility = Visibility.Visible;
                    txt_StartDevice.Text = string.Empty;
                    txt_EndDevice.Text = string.Empty;
                    txtTransformer.Text = string.Empty;
                    SelectPolygonEvent(false);
                    SetTabFocus(ddsubstation);
                }

                //Polygon Selection
                if (RdSelectMap.IsChecked == true)
                {
                    DeviceEntry.Visibility = Visibility.Collapsed;
                    stacktrans.Visibility = Visibility.Collapsed;
                    BtnBeyond.Visibility = Visibility.Collapsed;
                    DeviceEntryTransformer.Visibility = Visibility.Collapsed;
                    stackcircuit.Visibility = Visibility.Collapsed;
                    stackcircuit1.Visibility = Visibility.Collapsed;
                    txt_StartDevice.Text = string.Empty;
                    txt_EndDevice.Text = string.Empty;
                    txtTransformer.Text = string.Empty;
                    if (ddsubstation.SelectedIndex > -1)
                        ddsubstation.SelectedIndex = 0;
                    ddbank.Items.Clear();
                    ddbank.Items.Add("Select...");
                    if (ddbank.SelectedIndex > -1)
                        ddbank.SelectedIndex = 0;
                    ddcircuit.Items.Clear();
                    ddcircuit.Items.Add("Select...");
                    if (ddcircuit.SelectedIndex > -1)
                        ddcircuit.SelectedIndex = 0;
                    SetTabFocus(RdSelectMap);
                    SelectPolygonEvent(true);
                }
            }

        }
        //Bind Division and Substation data in search tool
        #region  Bind Division and Substation data in search tool
        private void dddivisionSearch_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                string sWhere = dddivisionSearch.SelectedItem.ToString();
                int intDivCode = GetDivisionCodes(sWhere);
                Divisioncode = intDivCode;
            }
            catch (Exception ex)
            {
                logger.FatalException("Error in PONS dddivisionSearch_SelectionChanged", ex);
            }
        }
        void Binddivision()
        {
            QueryTask queryTask = new QueryTask(_DivisionService);
            queryTask.ExecuteCompleted += QueryTask_ExecuteCompleted;
            queryTask.Failed += QueryTask_Failed;
            ESRI.ArcGIS.Client.Tasks.Query query = new ESRI.ArcGIS.Client.Tasks.Query();
            //Specify fields to return from initial query
            query.OutFields.AddRange(new string[] { "DIVISION" });
            // This query will just populate the drop-down, so no need to return geometry
            query.ReturnGeometry = false;
            // Return all features
            query.Where = "1=1";
            queryTask.ExecuteAsync(query, "initial");
        }
        private void DivisionQuery_ExecuteCompleted(object sender, ESRI.ArcGIS.Client.Tasks.QueryEventArgs args)
        {
            tabItem1.Visibility = Visibility.Collapsed;
            tabItem2.Visibility = Visibility.Visible;
            tabItem4.Visibility = Visibility.Collapsed;
            tabItem5.Visibility = Visibility.Collapsed;
            tabItem6.Visibility = Visibility.Collapsed;
            tabItem7.Visibility = Visibility.Collapsed;
            tabItem8.Visibility = Visibility.Collapsed;
            tabItem9.Visibility = Visibility.Collapsed;
            btnNext.IsEnabled = true;
            RdBetweenDevice.IsChecked = true;
            DeviceEntry.Visibility = Visibility.Visible;
            BtnBeyond.Visibility = Visibility.Visible;
            stacktrans.Visibility = Visibility.Collapsed;
            DeviceEntryTransformer.Visibility = Visibility.Collapsed;
            stackcircuit.Visibility = Visibility.Collapsed;
            stackcircuit1.Visibility = Visibility.Collapsed;
            if (SearchlistDataGrid.SelectedItems.Count > 0)
            {
                btnDeleteGetCustomer.IsEnabled = true;
            }
            else
            {
                btnDeleteGetCustomer.IsEnabled = false;
            }

            _fw.Width = 660;
            _fw.Height = 455;
        }
        private void QueryTask_ExecuteCompleted(object sender, ESRI.ArcGIS.Client.Tasks.QueryEventArgs args)
        {

            FeatureSet featureSet = args.FeatureSet;
            // If initial query to populate states combo box
            if ((args.UserState as string) == "initial")
            {
                // Just show on initial load
                //dddivision.Items.Add("Select...");
                dddivisionSearch.Items.Add("Select...");
                List<string> alphabetizedRegions = new List<string>();
                foreach (Graphic graphic in featureSet.Features)
                {
                    string cbiContent = null;

                    cbiContent = "Null : DIVISION = " + graphic.Attributes["DIVISION"].ToString();
                    if (graphic.Attributes["DIVISION"] != null)
                    {
                        cbiContent = graphic.Attributes["DIVISION"].ToString();
                    }
                    //Add region names to a list... will alphabetize them below.
                    if (!alphabetizedRegions.Contains(cbiContent))
                    {
                        alphabetizedRegions.Add(cbiContent);
                    }
                }
                //Sort regions and add to Division Search Filter ComboBox
                if (alphabetizedRegions.Count > 0)
                {
                    alphabetizedRegions.Sort();
                    foreach (string regionName in alphabetizedRegions)
                    {
                        //dddivision.Items.Add(regionName);
                        dddivisionSearch.Items.Add(regionName);
                    }
                    alphabetizedRegions.Clear();
                }
            }
            //dddivision.SelectedIndex = 0;
            dddivisionSearch.SelectedIndex = 0;
            return;
        }
        private void QueryTask_Failed(object sender, TaskFailedEventArgs args)
        {
            MessageBox.Show("Query failed: " + args.Error);
        }

        private void substationqueryTask_ExecuteCompleted(object sender, ESRI.ArcGIS.Client.Tasks.QueryEventArgs args)
        {
            FeatureSet featureSet = args.FeatureSet;
            try
            {
                // If initial query to populate states combo box
                if ((args.UserState as string) == "initial")
                {
                    ddsubstation.Items.Clear();
                    // Just show on initial load
                    ddsubstation.Items.Add("Select...");
                    List<string> alphabetizedSubstationName = new List<string>();
                    Dictionary<string, string> alphabetizedSubstationID = new Dictionary<string, string>();
                    List<Graphic> graphics = featureSet.Features.OrderBy(o => o.Attributes["NAME"]).ToList();
                    foreach (Graphic graphic in graphics)
                    {
                        string cbiContent = null;
                        string strSubstationID = null;

                        if (graphic.Attributes["SUBSTATIONID"] != null)
                        {
                            cbiContent = graphic.Attributes["STATIONNUMBER"] + "   " + Convert.ToString( graphic.Attributes["NAME"]);
                            strSubstationID = Convert.ToString(graphic.Attributes["SUBSTATIONID"]);
                        }

                        if (cbiContent != null || strSubstationID != null)
                        {

                            //Add region names to a list... will alphabetize them below.
                            if (!alphabetizedSubstationName.Contains(cbiContent))
                            {
                                alphabetizedSubstationName.Add(cbiContent);
                            }
                            if (!alphabetizedSubstationID.ContainsKey(cbiContent))
                            {
                                alphabetizedSubstationID.Add(cbiContent, strSubstationID);
                            }
                        }
                    }

                    //Sort regions and add to Division Search Filter ComboBox
                    if (alphabetizedSubstationID.Count > 0)
                    {
                        //alphabetizedSubstationName.Sort();

                        for (int i = 0; i < alphabetizedSubstationID.Count; i++)
                        {
                            //if (alphabetizedSubstationName[i] != null || alphabetizedSubstationID[i] != null)
                            {
                                ddsubstation.Items.Add(new Item(Convert.ToString( alphabetizedSubstationName[i]),Convert.ToString(alphabetizedSubstationID[alphabetizedSubstationName[i]])));
                            }

                        }

                        alphabetizedSubstationName.Clear();
                        alphabetizedSubstationID.Clear();


                    }
                }
                ddsubstation.SelectedIndex = 0;

                SetTabFocus(txt_StartDevice);

                //return;
            }
            catch (Exception ex)
            {
                busyIndicator.IsBusy = false;
                logger.FatalException("Error in PONS substationqueryTask_ExecuteCompleted", ex);
            }
        }

        private void SetTabFocus(Control focuscntrl)
        {
            HtmlPage.Plugin.Focus();
            focuscntrl.Focus();                    
        }
        private void subbankqueryTask_ExecuteCompleted(object sender, ESRI.ArcGIS.Client.Tasks.QueryEventArgs args)
        {
            FeatureSet featureSet = args.FeatureSet;
            try
            {
                // If initial query to populate states combo box
                if ((args.UserState as string) == "initial")
                {
                    ddcircuit.Items.Clear();
                    // Just show on initial load
                    ddcircuit.Items.Add("Select...");
                    List<string> alphabetizedCircuitIDName = new List<string>();
                    foreach (Graphic graphic in featureSet.Features)
                    {
                        string strCircuitID = null;

                        if (graphic.Attributes["CIRCUITID"] == null)
                        {
                            strCircuitID = "Null : CIRCUITID";
                        }
                        else
                        {
                            strCircuitID = Convert.ToString(graphic.Attributes["CIRCUITID"]);
                        }
                        alphabetizedCircuitIDName.Add(strCircuitID);
                    }

                    if (alphabetizedCircuitIDName != null)
                    {
                        for (int i = 0; i < alphabetizedCircuitIDName.Count; i++)
                        {
                            if (alphabetizedCircuitIDName[i] != null)
                            {
                                ddcircuit.Items.Add(Convert.ToString(alphabetizedCircuitIDName[i]));
                            }
                        }
                    }
                }
                ddcircuit.SelectedIndex = 0;
                return;
            }
            catch (Exception ex)
            {
                busyIndicator.IsBusy = false;
                logger.FatalException("Error in PONS subbankqueryTask_ExecuteCompleted", ex);
            }
        }
        private void IntializeSubstationLayer()
        {
            FeatureLayer pFeatureLayer = new FeatureLayer();
            pFeatureLayer.Url = _SubstationService;
            pFeatureLayer.Initialized += new EventHandler<EventArgs>(pFeatureLayer_Initialized);
            pFeatureLayer.Initialize();
        }
        private void pFeatureLayer_Initialized(object sender, System.EventArgs e)
        {
            try
            {
                FeatureLayer _featureLayer = sender as FeatureLayer;
                FeatureLayerInfo pLayerInfo = _featureLayer.LayerInfo;
                foreach (ESRI.ArcGIS.Client.Field field in pLayerInfo.Fields)
                {
                    if (field.Domain != null)
                    {
                        if (field.Name.ToUpper() == "DIVISION")
                        {
                            if (field.Domain is CodedValueDomain)
                            {
                                ConfigUtility.DivisionCodedDomains = field.Domain as CodedValueDomain;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }
        private void substationqueryTask_Failed(object sender, TaskFailedEventArgs args)
        {
            MessageBox.Show("Query failed: " + args.Error);
            busyIndicator.IsBusy = false;
            logger.FatalException("Error in PONS subbankqueryTask_ExecuteCompleted", null);

        }
        private void substationBankCircuitTask_ExecuteCompleted(object sender, ESRI.ArcGIS.Client.Tasks.QueryEventArgs args)
        {
            FeatureSet featureSet = args.FeatureSet;

            // If initial query to populate states combo box
            if ((args.UserState as string) == "initial")
            {
                ddbank.Items.Clear();
                ddbank.Items.Add("Select...");
                ddcircuit.Items.Clear();
                ddcircuit.Items.Add("Select...");
                List<string> alphabetizedBANKCDName = new List<string>();
                List<string> alphabetizedCircuitID = new List<string>();
                foreach (Graphic graphic in featureSet.Features)
                {
                    string strBankCD = null;
                    string strCircuitID = null;
                    if (graphic.Attributes["BANKCD"] == null)
                    {
                        strBankCD = "Null : BANKCD";
                    }
                    else
                    {
                        strBankCD = Convert.ToString(graphic.Attributes["BANKCD"]);
                    }
                    if (graphic.Attributes["CIRCUITID"] == null)
                    {
                        strCircuitID = "Null : CIRCUITID";
                    }
                    else
                    {
                        strCircuitID = Convert.ToString(graphic.Attributes["CIRCUITID"]);
                    }

                    //Add region names to a list... will alphabetize them below.
                    if (!alphabetizedBANKCDName.Contains(strBankCD))
                    {
                        alphabetizedBANKCDName.Add(strBankCD);
                    }
                    if (!alphabetizedCircuitID.Contains(strCircuitID))
                    {
                        alphabetizedCircuitID.Add(strCircuitID);
                    }
                }

                //Sort regions and add to Division Search Filter ComboBox
                if (alphabetizedBANKCDName.Count > 0)
                {
                    alphabetizedBANKCDName.Sort();

                    //foreach (string regionName in alphabetizedSubstationName)
                    for (int i = 0; i < alphabetizedBANKCDName.Count; i++)
                    {
                        if (Convert.ToString(alphabetizedBANKCDName[i]) != null)
                        {
                            ddbank.Items.Add(Convert.ToString(alphabetizedBANKCDName[i]));
                        }

                    }

                    alphabetizedBANKCDName.Clear();
                }
                if (alphabetizedCircuitID.Count > 0)
                {
                    alphabetizedCircuitID.Sort();

                    //foreach (string regionName in alphabetizedSubstationName)
                    for (int i = 0; i < alphabetizedCircuitID.Count; i++)
                    {
                        if (Convert.ToString(alphabetizedCircuitID[i]) != null)
                        {
                            ddcircuit.Items.Add(Convert.ToString(alphabetizedCircuitID[i]));
                        }

                    }

                    alphabetizedCircuitID.Clear();
                }
            }
            ddcircuit.SelectedIndex = 0;
            ddbank.SelectedIndex = 0;
            return;
            //Remove the first entry if "Select..."
            //if (dddivision.Items[0].ToString().Contains("Select..."))

            //    dddivision.Items.RemoveAt(0);

        }
        private void dddivision_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //string sWhere = dddivision.SelectedItem.ToString();
            //string qr = string.Empty;
            //if (dddivision.SelectedItem.ToString().Contains("Select..."))
            //    return;
            //QueryTask substationqueryTask = new QueryTask(_SubstationService);
            //substationqueryTask.ExecuteCompleted += substationqueryTask_ExecuteCompleted;
            //substationqueryTask.Failed += substationqueryTask_Failed;
            //ESRI.ArcGIS.Client.Tasks.Query query = new ESRI.ArcGIS.Client.Tasks.Query();
            ////query.OutFields.Add("NAME,SUBSTATIONID");
            //query.OutFields.Add("NAME,STATIONNUMBER,SUBSTATIONID");
            //query.ReturnGeometry = false;
            //int intDivCode = GetDivisionCodes(sWhere);
            //qr = "DIVISION = " + intDivCode;
            //query.Where = qr;
            //substationqueryTask.ExecuteAsync(query, "initial");
        }
        private void ddcircuit_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (ddbank.Items.Count > 0 && ddcircuit.Items.Count > 0)
                {
                    //ddbank.SelectedIndex = ddcircuit.SelectedIndex;
                }
            }
            catch(Exception ex)
            {
                logger.FatalException("Error in PONS ddcircuit_SelectionChanged", ex);
            }
        }
        private void ddbank_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (ddbank.SelectedItem != null)
                {
                    string strbank = ddbank.SelectedItem.ToString();
                    Item sWhere = null;
                    sWhere = ddsubstation.SelectedItem as Item;
                    string strSubstation = sWhere.Value.ToString();

                    //string strbank = sWhere.Name.ToString();
                    if (objSBCDataResult != null && ddcircuit.Items.Count > 0)
                    {
                        ddcircuit.Items.Clear();
                        ddcircuit.Items.Add("Select...");
                        foreach (SubStationSearchData sval in objSBCDataResult)
                        {
                            if (sval.SubStationName.ToUpper() == strSubstation.ToUpper() && sval.BANK.ToUpper() == strbank.ToUpper())
                            {
                                ddcircuit.Items.Add(sval.CircuitID);
                            }
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                logger.FatalException("Error in PONS ddbank_SelectionChanged", ex);
            }
        }
        private int GetDivisionCodes(string strDivision)
        {
            short intDivCodes = -1;
            if (ConfigUtility.DivisionCodedDomains != null)
            {
                foreach (KeyValuePair<object, string> codedValue in ConfigUtility.DivisionCodedDomains.CodedValues)
                {
                    if (codedValue.Value.ToString().ToUpper() == strDivision.ToUpper())
                    {
                        intDivCodes = (short)codedValue.Key;
                        break;
                    }
                }
            }
            return intDivCodes;
        }
        #endregion
        private void ddsubstation_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                //Adding function to pull the bank for selected substation
                if (ddsubstation.SelectedItem != null)
                {
                    Item sWhere = null;
                    sWhere = ddsubstation.SelectedItem as Item;
                    string strSubstation = sWhere.Value.ToString();
                    if (objSBCDataResult != null && ddbank.Items.Count > 0)
                    {
                        ddbank.Items.Clear();
                        ddbank.Items.Add("Select...");
                        ddcircuit.Items.Clear();
                        ddcircuit.Items.Add("Select...");
                        foreach (SubStationSearchData sval in objSBCDataResult)
                        {
                            if (sval.SubStationName.ToUpper() == strSubstation.ToUpper())
                            {
                                if (!ddbank.Items.Contains(sval.BANK))
                                {
                                    ddbank.Items.Add(sval.BANK);
                                }
                            }
                        }
                    }
                }
                
            }
            catch (Exception ex)
            {
                busyIndicator.IsBusy = false;
                logger.FatalException("Error in PONS subbankqueryTask_ExecuteCompleted", ex);
            }
        }
        private void substationqueryTaskID_ExecuteCompleted(object sender, ESRI.ArcGIS.Client.Tasks.QueryEventArgs args)
        {
            FeatureSet featureSet = args.FeatureSet;
            if (featureSet.Features == null)
                return;
            Graphic selectedFeature = featureSet.Features[0];
            _geometryService = new GeometryService(_GeometryService);
            _geometryService.BufferCompleted += GeometryService_BufferCompleted;
            _geometryService.Failed += GeometryService_Failed;
            ESRI.ArcGIS.Client.Tasks.BufferParameters bufferParams = new ESRI.ArcGIS.Client.Tasks.BufferParameters()
            {
                BufferSpatialReference = _map.SpatialReference,
                OutSpatialReference = _map.SpatialReference,
                Unit = LinearUnit.Foot,
            };
            bufferParams.Distances.Add(BufferDistance);
            bufferParams.Features.Add(selectedFeature);
            _geometryService.BufferAsync(bufferParams);
            // If initial query to populate states combo box

        }
        private void GeometryService_Failed(object sender, TaskFailedEventArgs args)
        {
            MessageBox.Show("Geometry service failed: " + args.Error);
        }
        void GeometryService_BufferCompleted(object sender, GraphicsEventArgs args)
        {
            try
            {
                Graphic bufferGraphic = new Graphic();
                bufferGraphic.Geometry = args.Results[0].Geometry;
                _queryTask = new QueryTask(_CircuitIDService);
                _queryTask.ExecuteCompleted += QueryTask_getCircuitID_ExecuteCompleted;
                _queryTask.Failed += QueryTask_Failed;
                ESRI.ArcGIS.Client.Tasks.Query query = new ESRI.ArcGIS.Client.Tasks.Query();
                query.ReturnGeometry = false;
                query.OutSpatialReference = _map.SpatialReference;
                query.Geometry = bufferGraphic.Geometry;
                query.OutFields.Add("CIRCUITID");
                _queryTask.ExecuteAsync(query);
            }
            catch
            {
            }
        }
        private void QueryTask_getCircuitID_ExecuteCompleted(object sender, QueryEventArgs args)
        {
            if (args.FeatureSet.Features.Count < 1)
            {
                MessageBox.Show("No Circuit found");
                return;
            }
            else
            {
                try
                {
                    ddbank.Items.Clear();
                    ddbank.Items.Add("Select...");
                    ddcircuit.Items.Clear();
                    ddcircuit.Items.Add("Select...");
                    List<string> alphabetizedBANKCDName = new List<string>();
                    List<string> alphabetizedCircuitID = new List<string>();
                    foreach (Graphic graphic in args.FeatureSet.Features)
                    {
                        string strBankCD = null;
                        string strCircuitID = null;
                        if (graphic.Attributes["BANKCD"] == null)
                        {
                            strBankCD = "Null : BANKCD";
                        }
                        else
                        {
                            strBankCD = Convert.ToString(graphic.Attributes["BANKCD"]);
                        }
                        if (graphic.Attributes["CIRCUITID"] == null)
                        {
                            strCircuitID = "Null : CIRCUITID";
                        }
                        else
                        {
                            strCircuitID = Convert.ToString(graphic.Attributes["CIRCUITID"]);
                        }

                        //Add region names to a list... will alphabetize them below.
                        if (!alphabetizedBANKCDName.Contains(strBankCD))
                        {
                            alphabetizedBANKCDName.Add(strBankCD);
                        }
                        if (!alphabetizedCircuitID.Contains(strCircuitID))
                        {
                            alphabetizedCircuitID.Add(strCircuitID);
                        }
                    }

                    //Sort regions and add to Division Search Filter ComboBox
                    if (alphabetizedBANKCDName.Count > 0)
                    {
                        alphabetizedBANKCDName.Sort();

                        //foreach (string regionName in alphabetizedSubstationName)
                        for (int i = 0; i < alphabetizedCircuitID.Count; i++)
                        {
                            if (alphabetizedCircuitID[i].ToString() != null)
                            {
                                ddbank.Items.Add(i + 1);
                            }

                        }

                        alphabetizedBANKCDName.Clear();
                    }
                    if (alphabetizedCircuitID.Count > 0)
                    {
                        alphabetizedCircuitID.Sort();

                        //foreach (string regionName in alphabetizedSubstationName)
                        for (int i = 0; i < alphabetizedCircuitID.Count; i++)
                        {
                            if (alphabetizedCircuitID[i].ToString() != null)
                            {
                                ddcircuit.Items.Add(alphabetizedCircuitID[i].ToString());
                            }

                        }

                        alphabetizedCircuitID.Clear();
                    }
                }
                catch
                {
                }
            }
            ddcircuit.SelectedIndex = 0;
            ddbank.SelectedIndex = 0;
            return;


        }
        //
        //Adding code for first Search beyond or between devices[Start]
        public void identify_ValidateInput()
        {

        }
        //Adding code for first Search beyond or between devices[End]

        // Transformer data search
        #region Transformer data search

        void BindPrimaryMeterSearchdata()
        {
            try
            {

                string par = txtTransformer.Text;
                string strPMSSD = _PrimaryMeterSSD;
                QueryTask PrimaryMeterTask = new QueryTask(_PrimaryMeterService);
                PrimaryMeterTask.ExecuteCompleted += PrimaryMeterTask_ExecuteCompleted;
                PrimaryMeterTask.Failed += PrimaryMeterTask_Failed;
                ESRI.ArcGIS.Client.Tasks.Query query = new ESRI.ArcGIS.Client.Tasks.Query();
                query.OutFields.Add("CGC12,CIRCUITID," + strPMSSD);

                if (par.Trim().Length == 12)
                {                    
                    query.Where = "CGC12 =" + par + " AND DIVISION = " + Divisioncode;                                        
                }
                else
                {
                    query.Where = "OPERATINGNUMBER ='" + par.Trim().ToUpper() + "' AND DIVISION = " + Divisioncode;
                }

                PrimaryMeterTask.ExecuteAsync(query);

            }
            catch (Exception ex)
            {
                logger.FatalException("Error in PONS BindTransformersearchdata", ex);
            }
        }

        private void PrimaryMeterTask_ExecuteCompleted(object sender, ESRI.ArcGIS.Client.Tasks.QueryEventArgs args)
        {
            //Adding Code
            string strPMSSD = _PrimaryMeterSSD;
            FeatureSet PrimarymeterfeatureSet = args.FeatureSet;
            if (PrimarymeterfeatureSet != null && PrimarymeterfeatureSet.Features.Count > 0)
            {
                SearchType = "2";
                _fw.Height = 455;
                CustWiz.Height = 455;
                DeviceType = "Primary Meter";
                //Adding code for handling the listbox
                if (PrimarymeterfeatureSet.Features.Count > 1)
                {
                    childWindow._deviceType = DeviceType;
                    childWindow.GetChDivisionnumber = Divisioncode; 
                    childWindow._TransformerService = _PrimaryMeterService; 
                    childWindow.Value = Convert.ToString(txtTransformer.Text);
                    childWindow.Rdtype = 2;
                    childWindow.SearchlistCircuitDataGrid.Visibility = Visibility.Collapsed;
                    childWindow.SearchlistDataGrid.Visibility = Visibility.Collapsed;                   
                    childWindow.OKButton.Visibility = Visibility.Collapsed;
                    childWindow.Show();
                }
                else
                {
                    Trancgs = PrimarymeterfeatureSet.Features[0].Attributes["CGC12"].ToString();
                    FedderID = PrimarymeterfeatureSet.Features[0].Attributes["CIRCUITID"].ToString();
                    if (PrimarymeterfeatureSet.Features[0].Attributes[strPMSSD] != null)
                    {
                        TranSSD = PrimarymeterfeatureSet.Features[0].Attributes[strPMSSD].ToString();
                    }
                    else
                    {
                        TranSSD = "";
                    }
                    SearchlistDataGrid.ItemsSource = null;
                    SearchlistDataGrid.ItemsSource = LoadCollectionData();
                    txtTransformer.Text = "";
                    SetTabFocus(txtTransformer);

                }
            }
            else
            {
                BindTransformersearchdata();
            }
        }
        private void PrimaryMeterTask_Failed(object sender, TaskFailedEventArgs args)
        {
            MessageBox.Show("Query failed: " + args.Error);
            busyIndicator.IsBusy = false;
        }
         
        void BindTransformersearchdata()
        {
            try
            {
                QueryTask TransqueryTask = new QueryTask(_TransformerService);
                TransqueryTask.ExecuteCompleted += Transquery_ExecuteCompleted;
                TransqueryTask.Failed += TransqueryTask_Failed;
                ESRI.ArcGIS.Client.Tasks.Query query = new ESRI.ArcGIS.Client.Tasks.Query();
                string par = txtTransformer.Text;
                if (par.Trim().Length == 12)
                    query.Where = "CGC12 =" + par + " AND DIVISION = " + Divisioncode;
                else
                    query.Where = "OPERATINGNUMBER = '" + par.Trim().ToUpper() + "' AND DIVISION = " + Divisioncode;
                query.OutFields.Add("CGC12,CIRCUITID,SOURCESIDEDEVICEID");
                TransqueryTask.ExecuteAsync(query);
            }
            catch (Exception ex)
            {
                logger.FatalException("Error in PONS BindTransformersearchdata", ex);
            }
        }
        List<DeviceSearchlist> DeviceList = new List<DeviceSearchlist>();
        private void Transquery_ExecuteCompleted(object sender, ESRI.ArcGIS.Client.Tasks.QueryEventArgs args)
        {
            //Adding Code
            TransformerfeatureSet = args.FeatureSet;
            if (TransformerfeatureSet != null && TransformerfeatureSet.Features.Count > 0)
            {
                SearchType = "2";
                _fw.Height = 455;
                CustWiz.Height = 455;
                DeviceType = "Transformer";
                //SearchlistDataGrid.ItemsSource = featureSet.Features;
                //Adding code for handling the listbox
                if (TransformerfeatureSet.Features.Count > 1)
                {
                    childWindow._deviceType = DeviceType;
                    childWindow._TransformerService = _TransformerService; 
                    childWindow.GetChDivisionnumber = Divisioncode; 
                    childWindow.Value = Convert.ToString(txtTransformer.Text);
                    childWindow.Rdtype = 2;
                    childWindow.SearchlistCircuitDataGrid.Visibility = Visibility.Collapsed;
                    childWindow.SearchlistDataGrid.Visibility = Visibility.Collapsed;
                    //childWindow.AdddataButton.Visibility = Visibility.Visible;
                    childWindow.OKButton.Visibility = Visibility.Collapsed;
                    childWindow.Show();
                }
                else
                {
                    Trancgs = Convert.ToString( TransformerfeatureSet.Features[0].Attributes["CGC12"]);
                    FedderID = Convert.ToString( TransformerfeatureSet.Features[0].Attributes["CIRCUITID"]);
                    TranSSD = Convert.ToString(TransformerfeatureSet.Features[0].Attributes["SOURCESIDEDEVICEID"]);
                    SearchlistDataGrid.ItemsSource = null;
                    if (!CGCTNumMapping.ContainsKey(Trancgs)  == true)                    
                        CGCTNumMapping.Add(Trancgs, txtTransformer.Text.Trim().ToUpper());
                    
                    SearchlistDataGrid.ItemsSource = LoadCollectionData();
                    txtTransformer.Text = "";
                }
            }
            else
            {
                SetTabFocus(RdTranformer);
                              
                if (txtTransformer.Text.Trim().Length == 12)
                    MessageBox.Show("CGC number does not exist.");
                else
                    MessageBox.Show("Operating number does not exist.");

                SetTabFocus(txtTransformer);
                tabItem1.Visibility = Visibility.Collapsed;
                tabItem2.Visibility = Visibility.Visible;          
                tabItem4.Visibility = Visibility.Collapsed;
                tabItem5.Visibility = Visibility.Collapsed;
                tabItem6.Visibility = Visibility.Collapsed;
                tabItem7.Visibility = Visibility.Collapsed;
                tabItem8.Visibility = Visibility.Collapsed;
                tabItem9.Visibility = Visibility.Collapsed;
                _fw.Height = 455;
                CustWiz.Height = 455;
            }
        }
        private void TransqueryTask_Failed(object sender, TaskFailedEventArgs args)
        {
            MessageBox.Show("Query failed: " + args.Error);
            busyIndicator.IsBusy = false;
        }

        void BindStartDevicedatasearch()
        {
            SearchType = "1";
            SearchlistDataGrid.ItemsSource = null;
            SearchlistDataGrid.ItemsSource = LoadCollectionData();

        }
        //Data populate from child window to Main window
        void childWindow_Closed(object sender, EventArgs e)
        {
            if (childWindow.DialogResult == true)
            {

                SearchType = "" + childWindow.SearchType + "";
                if (SearchType == "1")
                {

                    RdBetweenDevice.IsChecked = false;
                    RdBetweenDevice.IsChecked = true;
                    StartDeviceFeederID = "" + childWindow.Setstartfeeder + "";
                    StartDeviceName = "" + childWindow.Setstartdevicetype + "";
                    startdeviceID = "" + childWindow.SetStartOpno + "";
                    StartDeviceGUID = "" + childWindow.Setstartguid + "";
                    StartDeviceCircuit2 = childWindow.SetStartcircuitID2;
                    if (txt_EndDevice.Text.Trim() == "" || txt_StartDevice.Text.Trim().ToString().ToUpper() == txt_EndDevice.Text.Trim().ToString().ToUpper())
                    {
                        BindStartDevicedatasearch();
                    }                   
                    else
                    {
                        EndDeviceFeederID = "" + childWindow.Setendfeeder + "";
                        EndDeviceName = "" + childWindow.Setenddevicetype + "";
                        EnddeviceID = "" + childWindow.SetEndOpno + "";
                        EndDeviceGUID = "" + childWindow.Setendguid + "";
                        EndDeviceCircuit2 = childWindow.SetendcircuitID2;
                        BindStartDevicedatasearch();

                        //if (StartDeviceFeederID.ToString() == EndDeviceFeederID.ToString())
                        //{
                        //    BindStartDevicedatasearch();
                        //}
                        //else
                        //{
                        //    // MessageBox.Show("FeederID should be same in both device");
                        //    RdBetweenDevice.IsChecked = false;
                        //    RdBetweenDevice.IsChecked = true;
                        //}
                    }
                }
                if (SearchType == "3")
                {
                    CircuitFedderID = "" + childWindow.CircuitID + "";
                    Bindsearchdata();
                }
                if (SearchType == "2")
                {
                    FedderID = "" + childWindow.DeviceID + "";
                    Trancgs = "" + childWindow.TransID + "";
                    SearchlistDataGrid.ItemsSource = null;
                    SearchlistDataGrid.ItemsSource = LoadCollectionData();
                    txtTransformer.Text = string.Empty;
                    busyIndicator.IsBusy = false;
                    RdTranformer.IsChecked = false;
                    RdTranformer.IsChecked = true;
                }

            }
            else
            {
                busyIndicator.IsBusy = false;
                RdBetweenDevice.IsChecked = false;
                RdBetweenDevice.IsChecked = true;
                DeviceEntry.Visibility = Visibility.Visible;
                BtnBeyond.Visibility = Visibility.Visible;
                stacktrans.Visibility = Visibility.Collapsed;
                DeviceEntryTransformer.Visibility = Visibility.Collapsed;
                stackcircuit.Visibility = Visibility.Collapsed;
                stackcircuit1.Visibility = Visibility.Collapsed;

            }
        }
        void Bindsearchdata()
        {
            if (CircuitFedderID != "")
            {
                SearchlistDataGrid.ItemsSource = null;
                SearchlistDataGrid.ItemsSource = LoadCollectionData();
                //var DistinctItems = Searchdevice.GroupBy(x => x.DeviceID).Select(y => y.First());
                //SearchlistDataGrid.ItemsSource = null;
                //SearchlistDataGrid.ItemsSource = DistinctItems.ToList();
                //SearchlistDataGrid.Items.Add(DistinctItems.ToList());
            }
        }

        #endregion
        //Add data in the list
        private List<DeviceSearchlist> LoadCollectionData()
        {
            List<DeviceSearchlist> DistinctSearchData = new List<DeviceSearchlist>();
            try
            {
                if (SearchType == "1")
                {

                    if (txt_EndDevice.Text.Trim() == "" && txt_StartDevice.Text.Trim() != "")
                    {
                        Searchdevice.Add(new DeviceSearchlist()
                        {
                            DeviceName = "Customers fed by start device from '" + StartDeviceName.ToString() + "'  '" + startdeviceID.ToString() + "' on feeder '" + StartDeviceFeederID.ToString() + "'",
                            DeviceID = " '" + startdeviceID.ToString() + "'",
                            GUID = StartDeviceGUID.ToString(),
                            EndGUID = EndDeviceGUID
                        });
                        RdBetweenDevice.IsChecked = false;
                        RdBetweenDevice.IsChecked = true;
                    }


                    if (txt_EndDevice.Text.Trim() != "" && txt_StartDevice.Text.Trim() != "")
                    {
                        if (txt_EndDevice.Text.Trim().ToString().ToUpper() == txt_StartDevice.Text.Trim().ToString().ToUpper())
                        {
                            Searchdevice.Add(new DeviceSearchlist()
                            {
                                DeviceName = "Customers fed by start device from '" + StartDeviceName.ToString() + "'  '" + startdeviceID.ToString() + "' on feeder '" + StartDeviceFeederID.ToString() + "'",
                                DeviceID = " '" + startdeviceID.ToString() + "'",
                                GUID = StartDeviceGUID.ToString(),
                                EndGUID = StartDeviceGUID
                            });
                        }
                        else
                        {
                            Searchdevice.Add(new DeviceSearchlist()
                            {

                                DeviceName = "Customers fed by between '" + StartDeviceName.ToString() + "'  '" + startdeviceID.ToString() + "' to '" + EndDeviceName.ToString() + "'  '" + EnddeviceID.ToString() + "' on feeder '" + StartDeviceFeederID.ToString() + "'",
                                DeviceID = " '" + startdeviceID.ToString() + "'-  '" + EnddeviceID.ToString() + "'",
                                GUID = StartDeviceGUID.ToString(),
                                EndGUID = EndDeviceGUID.ToString()
                            });
                        }
                        RdBetweenDevice.IsChecked = false;
                        RdBetweenDevice.IsChecked = true;

                        SetTabFocus(txt_StartDevice);
                    }
                    if (StartDeviceName != "" || EnddeviceID != "")
                    {
                        switch (StartDeviceName)
                        {
                            case "Switch":
                                Start_TO_FEATURE_FCID = _Switch_FEATURE_FCID; break;
                            case "Fuse":
                                Start_TO_FEATURE_FCID = _FUSE_FEATURE_FCID; break;
                            case "PrimaryOverheadConductor":
                                Start_TO_FEATURE_FCID = _PrimaryOverheadConductor_FEATURE_FCID; break;
                            case "Transformer":
                                Start_TO_FEATURE_FCID = _Transformer_FEATURE_FCID; break;
                            case "OpenPoint":
                                Start_TO_FEATURE_FCID = _OpenPoint_FEATURE_FCID; break;
                            case "PriUGConductor":
                                Start_TO_FEATURE_FCID = _PriUGConductor_FEATURE_FCID; break;
                            case "Dynamicprotectivedevice":
                                Start_TO_FEATURE_FCID = _Dynamicprotectivedevice_FEATURE_FCID; break;

                        }

                        switch (EndDeviceName)
                        {
                            case "Switch":
                                End_TO_FEATURE_FCID = _Switch_FEATURE_FCID; break;
                            case "Fuse":
                                End_TO_FEATURE_FCID = _FUSE_FEATURE_FCID; break;
                            case "PrimaryOverheadConductor":
                                End_TO_FEATURE_FCID = _PrimaryOverheadConductor_FEATURE_FCID; break;
                            case "Transformer":
                                End_TO_FEATURE_FCID = _Transformer_FEATURE_FCID; break;
                            case "OpenPoint":
                                End_TO_FEATURE_FCID = _OpenPoint_FEATURE_FCID; break;
                            case "PriUGConductor":
                                End_TO_FEATURE_FCID = _PriUGConductor_FEATURE_FCID; break;
                            case "Dynamicprotectivedevice":
                                End_TO_FEATURE_FCID = _Dynamicprotectivedevice_FEATURE_FCID; break;

                        }
                    }
                    TracingStartDevice(StartDeviceGUID, StartDeviceFeederID, Start_TO_FEATURE_FCID);
                }
                if (RdTranformer.IsChecked == true)
                {
                    if (txtTransformer.Text.Length == 12)
                    {
                        if (DeviceType == "Transformer")
                        {

                            Searchdevice.Add(new DeviceSearchlist()
                            {

                                DeviceName = "Customers fed by Transformer '" + Trancgs.ToString() + "' on feeder '" + FedderID.ToString() + "'",
                                CGC12 = Trancgs.ToString(),
                                DeviceID = Trancgs.ToString() + "^" + TranSSD.ToString()
                            });
                        }
                        else if (DeviceType == "Primary Meter")
                        {
                            Searchdevice.Add(new DeviceSearchlist()
                            {

                                DeviceName = "Customers fed by Primary Meter '" + Trancgs.ToString() + "' on feeder '" + FedderID.ToString() + "'",
                                CGC12 = Trancgs.ToString(),
                                DeviceID = Trancgs.ToString() + "^" + TranSSD.ToString()
                            });
                        }
                    }
                    else
                    {                        

                        if (DeviceType == "Transformer")
                        {

                            Searchdevice.Add(new DeviceSearchlist()
                            {

                                DeviceName = "Customers fed by Transformer '" + txtTransformer.Text.ToUpper().Trim() + "' on feeder '" + FedderID.ToString() + "'",
                                CGC12 = Trancgs.ToString(),
                                DeviceID = Trancgs.ToString() + "^" + TranSSD.ToString()
                            });
                        }
                        else if (DeviceType == "Primary Meter")
                        {
                            Searchdevice.Add(new DeviceSearchlist()
                            {

                                DeviceName = "Customers fed by Primary Meter '" + txtTransformer.Text.ToUpper().Trim() + "' on feeder '" + FedderID.ToString() + "'",
                                CGC12 = Trancgs.ToString(),
                                DeviceID = Trancgs.ToString() + "^" + TranSSD.ToString()
                            });
                        }
                    }

                    SetTabFocus(txtTransformer);

                }

                if (RdCircuit.IsChecked == true)
                {
                    Searchdevice.Add(new DeviceSearchlist()
                    {
                        DeviceName = "Customers fed by substation  '" + ddsubstation.SelectedValue.ToString() + "' on feeder '" + CircuitFedderID.ToString() + "'",
                        DeviceID = CircuitFedderID.ToString()
                        //DeviceID = "'" + CircuitFedderID.ToString() + "'"
                    });
                    SetTabFocus(ddsubstation);
                    GetRelatedFeeder(CircuitFedderID);
                }

                if (RdSelectMap.IsChecked == true)   //Select by polygon
                {
                    for (int i = 0; i < _polygonSearchList.Count; i++)
                    {
                        if (_polygonSearchList[i].LayerName == "Transformer")
                        {
                            if (_polygonSearchList[i].OperatingNum != "Null")
                            {
                                Searchdevice.Add(new DeviceSearchlist()
                                {
                                    DeviceName = "Customers fed by Transformer '" + _polygonSearchList[i].OperatingNum.ToString() + "' on feeder '" + _polygonSearchList[i].FeederId.ToString() + "'",
                                    CGC12 = _polygonSearchList[i].CGC.ToString(),
                                    DeviceID = _polygonSearchList[i].CGC.ToString() + "^" + _polygonSearchList[i].SSD.ToString()
                                });
                            }
                            else
                            {
                                Searchdevice.Add(new DeviceSearchlist()
                                {
                                    DeviceName = "Customers fed by Transformer '" + _polygonSearchList[i].CGC.ToString() + "' on feeder '" + _polygonSearchList[i].FeederId.ToString() + "'",
                                    CGC12 = _polygonSearchList[i].CGC.ToString(),
                                    DeviceID = _polygonSearchList[i].CGC.ToString() + "^" + _polygonSearchList[i].SSD.ToString()
                                });
                            }
                        }
                        else if (_polygonSearchList[i].LayerName == "Primary Meter")
                        {
                            Searchdevice.Add(new DeviceSearchlist()
                            {
                                DeviceName = "Customers fed by Primary Meter '" + _polygonSearchList[i].CGC.ToString() + "' on feeder '" + _polygonSearchList[i].FeederId.ToString() + "'",
                                CGC12 = _polygonSearchList[i].CGC.ToString(),
                                DeviceID = _polygonSearchList[i].CGC.ToString() + "^" + _polygonSearchList[i].SSD.ToString()
                            });
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                logger.FatalException("Error in PONS LoadCollectionData", ex);
            }
            DistinctSearchData = Searchdevice.GroupBy(x => x.DeviceID).Select(y => y.First()).ToList();
            Searchdevice = DistinctSearchData;
            if (Searchdevice.Count > 0)
                GetCustomer.IsEnabled = true;
            else
                GetCustomer.IsEnabled = false;
            return Searchdevice;
        }
        string sFeederID_Cur = string.Empty;
        private void GetRelatedFeeder(string CircuitFedderID)
        {
            QueryTask FeederIDQuery = new QueryTask(_TracingNetworkService);
            FeederIDQuery.ExecuteCompleted += FeederIDQueryTask_ExecuteCompleted;
            FeederIDQuery.Failed += FeederIDQueryTask_Failed;

            ESRI.ArcGIS.Client.Tasks.Query query = new ESRI.ArcGIS.Client.Tasks.Query();
            //if (strCircuitArr.Count > 0)
            {
                query.OutFields.AddRange(new string[] { "TO_CIRCUITID" });
                query.Where = "FEEDERID = '" + (sFeederID_Cur = CircuitFedderID) + "' AND TO_CIRCUITID IS NOT NULL";
            }
            if (!strCircuitDic.ContainsKey(sFeederID_Cur))
                strCircuitDic.Add(sFeederID_Cur, new ArrayList());
            FeederIDQuery.ExecuteAsync(query);
        }
        List<DeviceSearchlist> selectedList = new List<DeviceSearchlist>();

        //Add
        private void Chcountcustomer_Click(object sender, RoutedEventArgs e)
        {



            CheckBox chbkDelete = null;
            chbkDelete = sender as CheckBox;
            bool? res = chbkDelete.IsChecked;


            int count_check;

            count_check = customercount_checkbox;
            Pdfcustomerdata objPDF = new Pdfcustomerdata();
            Averycustomerdata objAvery = new Averycustomerdata();
            Addcustomerdata objAddPSLCust = new Addcustomerdata();
            int i = Convert.ToInt32(CustCount.Text);
            if (Chcountcustomer.IsChecked == true)
            {

                foreach (Pdfcustomerdata p in SelectAllCustomerResult)
                {
                    if ((bool)res)
                    {


                        objPDF = CustomerDataGrid.SelectedItem as Pdfcustomerdata;
                        CustomerList.Remove(p);
                        objAvery = CustomerDataGrid.SelectedItem as Averycustomerdata;
                        AveryCustomerList.Remove(objAvery);
                        objAddPSLCust = CustomerDataGrid.SelectedItem as Addcustomerdata;
                        AddCustomerList.Remove(objAddPSLCust);


                        //var a = CustomerList.ToList();

                        bool ch = p.Checked;
                        chbkDelete = CustomerDataGrid.Columns[0].GetCellContent(p) as CheckBox;
                        if (chbkDelete != null)
                        {
                            chbkDelete.IsChecked = true;

                            // CustomerList.Add(p);
                        }
                        else
                        {
                            p.Checked = true;
                            // CustomerList.Add(p);
                        }
                    }
                    MetCount.Text = SelectAllMetercustomercount.ToString();
                    CustCount.Text = SelectAllcustomercount.ToString();

                }

                var a = CustomerList.ToList();
                btnNextsearchlist.IsEnabled = true;
            }
            else
            {

                foreach (Pdfcustomerdata p in SelectAllCustomerResult)
                {


                    objPDF = CustomerDataGrid.SelectedItem as Pdfcustomerdata;
                    CustomerList.Add(p);
                    AveryCustomerList.Add(CustomerDataGrid.SelectedItem as Averycustomerdata);
                    AddCustomerList.Add(CustomerDataGrid.SelectedItem as Addcustomerdata);

                    chbkDelete = CustomerDataGrid.Columns[0].GetCellContent(p) as CheckBox;
                    if (chbkDelete != null)
                    {
                        chbkDelete.IsChecked = false;
                    }

                    else { p.Checked = false; }

                }
                // CustomerList.Clear();

                MetCount.Text = "0";
                CustCount.Text = "0";
                btnNextsearchlist.IsEnabled = false;

            }


        }
        //private void CustomerDataGrid_LoadingRow(object sender, DataGridRowEvent
        //Datagrid checkbox change
        private void radioSelectdevice_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int count_check;
                count_check = customercount_checkbox;
                Pdfcustomerdata objPDF = new Pdfcustomerdata();
                Averycustomerdata objAvery = new Averycustomerdata();
                Addcustomerdata objAddPSLCust = new Addcustomerdata();
                int i = Convert.ToInt32(CustCount.Text);

                CheckBox chbkDelete = null;
                chbkDelete = sender as CheckBox;
                bool? res = chbkDelete.IsChecked;
                if ((bool)res)
                {
                    chbkDelete.IsThreeState = false;
                    objPDF = CustomerDataGrid.SelectedItem as Pdfcustomerdata;
                    CustomerList.Remove(objPDF);
                    objAvery = CustomerDataGrid.SelectedItem as Averycustomerdata;
                    AveryCustomerList.Remove(objAvery);
                    objAddPSLCust = CustomerDataGrid.SelectedItem as Addcustomerdata;
                    AddCustomerList.Remove(objAddPSLCust);

                    var mtrList = CustomerResult.Where(m => m.Checked == true).Select(m => m.ServicepointId).Distinct();

                    CustCount.Text = (i + 1).ToString();
                    MetCount.Text = mtrList.Count().ToString();
                    if (count_check == (i + 1))
                    {
                        Chcountcustomer.IsChecked = true;
                    }
                    else
                    {
                        Chcountcustomer.IsChecked = false;
                    }
                }
                else
                {
                    chbkDelete.IsThreeState = false;
                    objPDF = CustomerDataGrid.SelectedItem as Pdfcustomerdata;
                    CustomerList.Add(CustomerDataGrid.SelectedItem as Pdfcustomerdata);
                    AveryCustomerList.Add(CustomerDataGrid.SelectedItem as Averycustomerdata);
                    AddCustomerList.Add(CustomerDataGrid.SelectedItem as Addcustomerdata);
                    CustCount.Text = (i - 1).ToString();

                    //CustomerDataGrid.RowStyle SolidColorBrush(Colors.Gray);

                    var mtrList = CustomerResult.Where(m => m.Checked == true).Select(m => m.ServicepointId).Distinct(); ;
                    MetCount.Text = mtrList.Count().ToString();

                    //Chcountcustomer.Background = new SolidColorBrush(Colors.Brown);
                    //Chcountcustomer.Opacity = 20;// Attributes.Add("style", "background-color:Red");
                    if (count_check == (i - 1))
                    {
                        Chcountcustomer.IsChecked = true;
                    }
                    else
                    {
                        Chcountcustomer.IsChecked = false;
                    }

                }
                int ic = Convert.ToInt32(CustCount.Text);
                if (ic > 0)
                {
                    btnNextsearchlist.IsEnabled = true;
                }
                else
                {
                    btnNextsearchlist.IsEnabled = false;
                }
            }
            catch (Exception ex)
            {
                logger.FatalException("Error in PONS radioSelectdevice_Click", ex);
            }
        }
        //Get servicePointID from DeviceID
        //Change- 30/12/2015
        # region Search-1  for customer list
        void TracingStartDevice(string globalID, string DeviceFeederID, int Device_TO_FEATURE_FCID)
        {
            try
            {
                StrServicePointID_StartDevice = string.Empty;
                strDeviceServicePointListResult = string.Empty;

                int itemCount = ListStrServicePointID_StartDevice.Count;
                int itemendlistcount = ListStrServicePointID_EndDevice.Count;
                if (itemendlistcount > 0)
                {

                    for (int i = itemendlistcount - 1; i >= 0; i--)
                    {


                        ListStrServicePointID_EndDevice.RemoveAt(i);
                    }
                }
                if (itemCount > 0)
                {

                    for (int i = itemCount - 1; i >= 0; i--)
                    {


                        ListStrServicePointID_StartDevice.RemoveAt(i);
                    }
                }
                QueryTask StartTracingTask = new QueryTask(_TracingNetworkService);
                StartTracingTask.ExecuteCompleted += StartTracingTask_ExecuteCompleted;
                StartTracingTask.Failed += StartTracingTask_Failed;
                ESRI.ArcGIS.Client.Tasks.Query query = new ESRI.ArcGIS.Client.Tasks.Query();
                query.OutFields.AddRange(new string[] { "ORDER_NUM", "MIN_BRANCH", "MAX_BRANCH", "TREELEVEL", "TO_FEATURE_FEEDERINFO", "TO_FEATURE_FCID", "TO_FEATURE_EID", "TO_FEATURE_OID", "FEEDERID", "FEEDERFEDBY", "TO_CIRCUITID" });
                if (!string.IsNullOrEmpty(globalID))
                {
                    query.Where = "TO_FEATURE_GLOBALID ='" + globalID + "'AND FEEDERID ='" + DeviceFeederID + "' AND TO_FEATURE_FCID =" + Device_TO_FEATURE_FCID;
                }
                StartTracingTask.ExecuteAsync(query);
               
            }
            catch (Exception ex)
            {
                busyIndicator.IsBusy = false;
                
                logger.FatalException("Error in PONS TracingStartDevice", ex);
            }
        }
        private void StartTracingTask_ExecuteCompleted(object sender, ESRI.ArcGIS.Client.Tasks.QueryEventArgs args)
        {
            FeatureSet StarttracinginfoFeatureSet = args.FeatureSet;
            try
            {
                if (StarttracinginfoFeatureSet != null && StarttracinginfoFeatureSet.Features.Count > 0)
                {
                    ORDER_NUM = Convert.ToInt32(StarttracinginfoFeatureSet.Features[0].Attributes["ORDER_NUM"]);
                    MIN_BRANCH = Convert.ToInt32(StarttracinginfoFeatureSet.Features[0].Attributes["MIN_BRANCH"]);
                    MAX_BRANCH = Convert.ToInt32(StarttracinginfoFeatureSet.Features[0].Attributes["MAX_BRANCH"]);
                    TREELEVEL = Convert.ToInt32(StarttracinginfoFeatureSet.Features[0].Attributes["TREELEVEL"]);
                    TO_FEATURE_FEEDERINFO = Convert.ToInt32(StarttracinginfoFeatureSet.Features[0].Attributes["TO_FEATURE_FEEDERINFO"]);
                    TO_FEATURE_EID = Convert.ToInt32(StarttracinginfoFeatureSet.Features[0].Attributes["TO_FEATURE_EID"]);
                    TO_FEATURE_OID = Convert.ToInt32(StarttracinginfoFeatureSet.Features[0].Attributes["TO_FEATURE_OID"]);
                    TO_FEATURE_FCID = Convert.ToInt32(StarttracinginfoFeatureSet.Features[0].Attributes["TO_FEATURE_FCID"]);
                    FEEDERID_tracing =Convert.ToString(  StarttracinginfoFeatureSet.Features[0].Attributes["FEEDERID"]);
                    TO_CIRCUITID = Convert.ToInt32(StarttracinginfoFeatureSet.Features[0].Attributes["TO_CIRCUITID"]);
                    GetServiceLocation_StartDevice(FEEDERID_tracing, MIN_BRANCH, MAX_BRANCH, TREELEVEL, ORDER_NUM, TO_FEATURE_FCID);
                }
                else
                {
                    busyIndicator.IsBusy = false;
                }
            }
            catch (Exception ex)
            {
                busyIndicator.IsBusy = false;
               
                logger.FatalException("Error in PONS StartTracingTask_ExecuteCompleted", ex);
            }
        }
        private void StartTracingTask_Failed(object sender, TaskFailedEventArgs args)
        {
            busyIndicator_Customer.IsBusy = false;
        }
        //Get Service location from Start Device
        void GetServiceLocation_StartDevice(string FEEDERID, int MIN_BRANCH, int MAX_BRANCH, int TREELEVEL, int ORDER_NUM, int TO_FEATURE_FCID)
        {
            try
            {
                QueryTask StartServiceLocationTask = new QueryTask(_TracingNetworkService);
                StartServiceLocationTask.ExecuteCompleted += StartServiceLocationTask_ExecuteCompleted;
                StartServiceLocationTask.Failed += StartServiceLocationTask_Failed;
                string Tran_PriMtr_TO_FEATURE_FCID = _ServiceLocation_FEATURE_FCID;
                ESRI.ArcGIS.Client.Tasks.Query query = new ESRI.ArcGIS.Client.Tasks.Query();
                if (!string.IsNullOrEmpty(FEEDERID))
                {
                    query.OutFields.AddRange(new string[] { "TO_FEATURE_FCID,TO_FEATURE_GLOBALID,TO_CIRCUITID" });
                    query.Where = "(TO_FEATURE_FCID IN (" + Tran_PriMtr_TO_FEATURE_FCID + "," + Start_TO_FEATURE_FCID + "," + End_TO_FEATURE_FCID + ")"+                        
                        " OR (TO_FEATURE_FCID IN (" + PriUGConductorFCID + "," + PriOHConductorFCID + ") AND TO_CIRCUITID IS NOT NULL)) "+
                        "AND MIN_BRANCH >= " + MIN_BRANCH + " AND MAX_BRANCH <= " + MAX_BRANCH
                            + " AND TREELEVEL >= " + TREELEVEL + " AND ORDER_NUM <= " + ORDER_NUM + " AND FEEDERID = '" + FEEDERID + "'";
                }
                StartServiceLocationTask.ExecuteAsync(query);
            }
            catch (Exception ex)
            {
                busyIndicator.IsBusy = false;
               
                logger.FatalException("Error in PONS GetServiceLocation_StartDevice", ex);
            }
        }

        private void getCGCQuery_ExecuteCompleted(object sender, QueryEventArgs e)
        {
            completedCountTask++;
            FeatureSet cgcset = e.FeatureSet;
            if (e.FeatureSet.Count() > 0)
            {
                foreach(Graphic feature in e.FeatureSet.Features)
                {
                    if (feature.Attributes["CGC12"] != null)
                    {
                        if (!_cgc12List.Contains(Convert.ToString(feature.Attributes["CGC12"])))
                            _cgc12List.Add(Convert.ToString(feature.Attributes["CGC12"]));
                    }
                }
                
            }
           
            if (completedCountTask == countQueryTask)
            {
                DeviceSearchItem devicesearchitem = new DeviceSearchItem();
                devicesearchitem.CGCList = _cgc12List;
                devicesearchitem.Key = StartDeviceGUID + EndDeviceGUID;
                _deviceSearchItemList.Add(devicesearchitem);
                busyIndicator.IsBusy = false; 
                RdBetweenDevice.IsChecked = true;
                txt_StartDevice.Text = "";
                txt_EndDevice.Text = "";
                SetTabFocus(txt_StartDevice);
            }
        }
        void getCGCQuery_Failed(object sender, TaskFailedEventArgs e)
        {
            completedCountTask++;
        }
       
        int countQueryTask;
        int completedCountTask;
        List<string> _cgc12List;
        List<DeviceSearchItem> _deviceSearchItemList = new List<DeviceSearchItem>();
        private void getDeviceCGC(Dictionary<string,string> finaldifferenceFeature)
        {
            try
            {
                countQueryTask = 0;
                completedCountTask = 0;
                var FCIDValues = finaldifferenceFeature.Values.Distinct();
                _cgc12List = new List<string>();
                foreach (string fcid in FCIDValues)
                {
                    QueryTask getCGCQuery = null;
                    
                    if(fcid == _Transformer_FEATURE_FCID.ToString())
                      getCGCQuery = new QueryTask(_TransformerService);
                    else if(fcid == _PrimaryMeter_FEATURE_FCID.ToString())
                      getCGCQuery = new QueryTask(_PrimaryMeterService);

                    getCGCQuery.ExecuteCompleted += getCGCQuery_ExecuteCompleted;
                    getCGCQuery.Failed += getCGCQuery_Failed;
                    ESRI.ArcGIS.Client.Tasks.Query query = new ESRI.ArcGIS.Client.Tasks.Query();
                    string SERVICEPOINTID = ConfigUtility.SERVICEPOINTID;
                    query.OutFields.AddRange(new string[] { "CGC12" });
                    query.ReturnGeometry = false;
                    string whereInString = string.Empty;
                    int iCount = 0;
                    Dictionary<string,string> guid = finaldifferenceFeature.Where(p => p.Value == fcid).Distinct().ToDictionary(p => p.Key, p => p.Value);
                    foreach(string key in guid.Keys)
                    {
                        if (iCount == 0)
                            whereInString = "'" + key + "'";
                        else
                            whereInString = whereInString + ",'" + key + "'";

                        iCount++;
                    }
                    
                    query.Where = "GLOBALID IN (" + whereInString + ")";
                    getCGCQuery.ExecuteAsync(query);
                    countQueryTask++;
                }
            }
            catch (Exception ex)
            {                
                busyIndicator.IsBusy = false;               
                logger.FatalException("Error in PONS GetServiceLocation_StartDevice", ex);  
            }
        }
        private void ProcessDeviceResulst()
        {
            try
            {
                Dictionary<string, string> finaldifferenceFeature = null;
                if (!string.IsNullOrEmpty(txt_StartDevice.Text.Trim()) && !string.IsNullOrEmpty(txt_EndDevice.Text.Trim()))
                {
                    if (startGUIDList.ContainsKey(EndDeviceGUID))
                    {
                        finaldifferenceFeature = startGUIDList.Except(endGUIDList).ToDictionary(p => p.Key, p => p.Value);
                        finaldifferenceFeature = finaldifferenceFeature.Where(p => p.Value == _Transformer_FEATURE_FCID.ToString() || p.Value == _PrimaryMeter_FEATURE_FCID.ToString()).ToDictionary(p => p.Key, p => p.Value);
                    }
                    else if (endGUIDList.ContainsKey(StartDeviceGUID))
                    {
                        finaldifferenceFeature = endGUIDList.Except(startGUIDList).ToDictionary(p => p.Key, p => p.Value);
                        finaldifferenceFeature = finaldifferenceFeature.Where(p => p.Value == _Transformer_FEATURE_FCID.ToString() || p.Value == _PrimaryMeter_FEATURE_FCID.ToString()).ToDictionary(p => p.Key, p => p.Value);
                    }
                    else if (!startGUIDList.ContainsKey(EndDeviceGUID) || !endGUIDList.ContainsKey(StartDeviceGUID))
                    {
                        if (StartDeviceFeederID != EndDeviceFeederID && StartDeviceFeederID != EndDeviceCircuit2 && EndDeviceFeederID != StartDeviceCircuit2)
                        {
                            MessageBox.Show("Both Devices are not in the same Feeder");
                        }
                        else
                        {
                            MessageBox.Show("The end device is not downstream of the start device.\nPlease select start and end devices on the same branch of the same circuit.", "PONS", MessageBoxButton.OK);
                        }
                        SearchlistDataGrid.ItemsSource = null;
                        for (int j = Searchdevice.Count; j > 0; j--)
                        {
                            if (StartDeviceGUID == Searchdevice[j - 1].GUID && EndDeviceGUID == Searchdevice[j - 1].EndGUID)
                            {
                                Searchdevice.RemoveAt(j-1);
                            }
                        }

                        SearchlistDataGrid.ItemsSource = Searchdevice;
                        var DistinctItems = Searchdevice.GroupBy(x => x.DeviceID).Select(y => y.First());
                        SearchlistDataGrid.ItemsSource = null;
                        SearchlistDataGrid.ItemsSource = DistinctItems.ToList();

                        startGUIDList.Clear();
                        endGUIDList.Clear();
                        busyIndicator.IsBusy = false;

                        return;
                    }
                }
                else if (!string.IsNullOrEmpty(txt_StartDevice.Text.Trim()))
                {
                    finaldifferenceFeature = startGUIDList.Where(p => p.Value == _Transformer_FEATURE_FCID.ToString() || p.Value == _PrimaryMeter_FEATURE_FCID.ToString()).ToDictionary(p => p.Key, p => p.Value); ;
                }
                if (finaldifferenceFeature.Count() > 0)
                    getDeviceCGC(finaldifferenceFeature);
                else
                {
                    busyIndicator.IsBusy = false;
                    SetTabFocus(txt_StartDevice);
                }
            }
            catch (Exception ex)
            {
                busyIndicator.IsBusy = false;
                logger.FatalException("Error in PONS GetServiceLocation_StartDevice", ex);
            }
        }
        Dictionary<string, string> startGUIDList = new Dictionary<string, string>();
        Dictionary<string, string> endGUIDList = new Dictionary<string, string>();

        private void StartServiceLocationTask_ExecuteCompleted(object sender, ESRI.ArcGIS.Client.Tasks.QueryEventArgs args)
        {
            listStartdeviceSL.Clear();
            startGUIDList.Clear();
           
            string ServiceLocation_StartGUIDstring = string.Empty;
            FeatureSet StartDeviceServiceLocationFeatureSet = args.FeatureSet;
            try
            {
                if (StartDeviceServiceLocationFeatureSet != null && StartDeviceServiceLocationFeatureSet.Features.Count > 0)
                {
                  //  arrFFF_FeederID_Trace.Clear();//Add this code
                    foreach (Graphic graphic in StartDeviceServiceLocationFeatureSet.Features)
                    {
                        if(!startGUIDList.ContainsKey(Convert.ToString(graphic.Attributes["TO_FEATURE_GLOBALID"])))
                            startGUIDList.Add(Convert.ToString(graphic.Attributes["TO_FEATURE_GLOBALID"]), Convert.ToString(graphic.Attributes["TO_FEATURE_FCID"]));  
                        if(!string.IsNullOrEmpty(Convert.ToString(graphic.Attributes["TO_CIRCUITID"])) && !arrFFF_FeederID_Trace.Contains(Convert.ToString(graphic.Attributes["TO_CIRCUITID"])))
                        {

                            arrFFF_FeederID_Trace.Add(Convert.ToString(graphic.Attributes["TO_CIRCUITID"]));
                        }
                    }
                }

                if (!string.IsNullOrEmpty(txt_EndDevice.Text.Trim()))
                {
                    if (txt_StartDevice.Text.Trim().ToString().ToUpper() == txt_EndDevice.Text.Trim().ToString().ToUpper())
                    {
                       
                        ProcessDeviceResulst();
                    }
                    else
                    {
                        switch (EndDeviceName)
                        {
                            case "Switch":
                                End_TO_FEATURE_FCID = _Switch_FEATURE_FCID; break;
                            case "Fuse":
                                End_TO_FEATURE_FCID = _FUSE_FEATURE_FCID; break;
                            case "PrimaryOverheadConductor":
                                End_TO_FEATURE_FCID = _PrimaryOverheadConductor_FEATURE_FCID; break;
                            case "Transformer":
                                End_TO_FEATURE_FCID = _Transformer_FEATURE_FCID; break;
                            case "OpenPoint":
                                End_TO_FEATURE_FCID = _OpenPoint_FEATURE_FCID; break;
                            case "PriUGConductor":
                                End_TO_FEATURE_FCID = _PriUGConductor_FEATURE_FCID; break;
                            case "Dynamicprotectivedevice":
                                End_TO_FEATURE_FCID = _Dynamicprotectivedevice_FEATURE_FCID; break;

                        }
                        TracingEndDevice(EndDeviceGUID, EndDeviceFeederID, End_TO_FEATURE_FCID);
                    }
                }

                if (string.IsNullOrEmpty(txt_EndDevice.Text.Trim()))
                {
                    ProcessDeviceResulst();
                }
               
            }
            catch (Exception ex)
            {
                busyIndicator.IsBusy = false;
                logger.FatalException("Error in PONS ", ex);
            }
        }
        private void StartServiceLocationTask_Failed(object sender, TaskFailedEventArgs args)
        {
            busyIndicator.IsBusy = false;
        }
        
        void GetServicePoint(string whereInString)
        {
            try
            {
                QueryTask getServicePointResultsQuery = new QueryTask(_ServicepointIDService);
                getServicePointResultsQuery.ExecuteCompleted += getServicePointResultsQuery_ExecuteCompleted;
                getServicePointResultsQuery.Failed += getServicePointResultsQuery_Failed;
                ESRI.ArcGIS.Client.Tasks.Query query = new ESRI.ArcGIS.Client.Tasks.Query();
                string SERVICEPOINTID = ConfigUtility.SERVICEPOINTID;
                query.OutFields.AddRange(new string[] { ConfigUtility.SERVICEPOINTID });
                query.ReturnGeometry = false;
                //No transformer GUID need be specified
                query.Where = "SERVICELOCATIONGUID IN ('" + whereInString + "') AND SERVICEPOINTID IS NOT NULL";
                getServicePointResultsQuery.ExecuteAsync(query);
            }
            catch
            {
                busyIndicator.IsBusy = false;
                return;
            }

        }



        private void getServicePointResultsQuery_ExecuteCompleted(object sender, QueryEventArgs e)
        {
             strStartdevicePointID = string.Empty;
            int itemCount = ListStrServicePointID_StartDevice.Count;
            if (itemCount > 0)
            {
                for (int i = itemCount - 1; i >= 0; i--)
                {
                    ListStrServicePointID_StartDevice.RemoveAt(i);
                }
            }
            FeatureSet StartDeviceServicePointFeatureSet = e.FeatureSet;
            if (StartDeviceServicePointFeatureSet != null && StartDeviceServicePointFeatureSet.Features.Count > 0)
            {
                for (int i = 0; i < StartDeviceServicePointFeatureSet.Features.Count; i++)
                {
                    ListStrServicePointID_StartDevice.Add(StartDeviceServicePointFeatureSet.Features[i].Attributes[ConfigUtility.SERVICEPOINTID].ToString());
                }
                for (int i = 0; i < ListStrServicePointID_StartDevice.Count; i++)
                {
                    if (i == 0)
                    {
                        strStartdevicePointID = ListStrServicePointID_StartDevice.ElementAt(i);
                    }
                    else
                    {
                        strStartdevicePointID = String.Concat(strStartdevicePointID, ",", ListStrServicePointID_StartDevice.ElementAt(i));
                    }
                }
                StrServicePointID_StartDevice = string.Empty;
                StrServicePointID_StartDevice = strStartdevicePointID.ToString();
                if (!string.IsNullOrEmpty(txt_EndDevice.Text.Trim()))
                {
                    if (txt_StartDevice.Text.Trim().ToString().ToUpper() == txt_EndDevice.Text.Trim().ToString().ToUpper())
                    {
                        // busyIndicator_Customer.IsBusy = false;
                        busyIndicator.IsBusy = false;
                        RdBetweenDevice.IsChecked = true;
                    }
                    else
                    {
                        switch (EndDeviceName)
                        {
                            case "Switch":
                                End_TO_FEATURE_FCID = _Switch_FEATURE_FCID; break;
                            case "Fuse":
                                End_TO_FEATURE_FCID = _FUSE_FEATURE_FCID; break;
                            case "PrimaryOverheadConductor":
                                End_TO_FEATURE_FCID = _PrimaryOverheadConductor_FEATURE_FCID; break;
                            case "Transformer":
                                End_TO_FEATURE_FCID = _Transformer_FEATURE_FCID; break;
                            case "OpenPoint":
                                End_TO_FEATURE_FCID = _OpenPoint_FEATURE_FCID; break;
                            case "PriUGConductor":
                                End_TO_FEATURE_FCID = _PriUGConductor_FEATURE_FCID; break;
                            case "Dynamicprotectivedevice":
                                End_TO_FEATURE_FCID = _Dynamicprotectivedevice_FEATURE_FCID; break;

                        }
                        TracingEndDevice(EndDeviceGUID, EndDeviceFeederID, End_TO_FEATURE_FCID);
                    }
                }
                else
                {
                    busyIndicator.IsBusy = false;
                    RdBetweenDevice.IsChecked = true;
                }
            }
            else
            {
               // busyIndicator_Customer.IsBusy = false;
                busyIndicator.IsBusy = false;
                RdBetweenDevice.IsChecked = true;
            }

        }
        void getServicePointResultsQuery_Failed(object sender, TaskFailedEventArgs e)
        {
            //busyIndicator_Customer.IsBusy = false;
        }
        //Tracing service Location -End Device
        void TracingEndDevice(string globalID, string DeviceFeederID, int Device_TO_FEATURE_FCID)
        {
            int itemCount = listEnddeviceSL.Count;
            if (itemCount > 0)
            {
                for (int i = itemCount - 1; i >= 0; i--)
                {
                    listEnddeviceSL.RemoveAt(i);
                }
            }
            QueryTask EndTracingTask = new QueryTask(_TracingNetworkService);
            EndTracingTask.ExecuteCompleted += EndTracingTask_ExecuteCompleted;
            EndTracingTask.Failed += EndTracingTask_Failed;
            ESRI.ArcGIS.Client.Tasks.Query query = new ESRI.ArcGIS.Client.Tasks.Query();
            query.OutFields.AddRange(new string[] { "ORDER_NUM", "MIN_BRANCH", "MAX_BRANCH", "TREELEVEL", "TO_FEATURE_FEEDERINFO", "TO_FEATURE_FCID", "TO_FEATURE_EID", "TO_FEATURE_OID", "FEEDERID", "FEEDERFEDBY", "TO_CIRCUITID" });
            if (!string.IsNullOrEmpty(globalID))
            {
                query.Where = "TO_FEATURE_GLOBALID ='" + globalID + "'AND FEEDERID ='" + DeviceFeederID + "' AND TO_FEATURE_FCID =" + Device_TO_FEATURE_FCID + "";
            }
            EndTracingTask.ExecuteAsync(query);
        }

        private void EndTracingTask_ExecuteCompleted(object sender, ESRI.ArcGIS.Client.Tasks.QueryEventArgs args)
        {
            FeatureSet StarttracinginfoFeatureSet = args.FeatureSet;
            if (StarttracinginfoFeatureSet != null && StarttracinginfoFeatureSet.Features.Count > 0)
            {
                int E_ORDER_NUM = Convert.ToInt32(StarttracinginfoFeatureSet.Features[0].Attributes["ORDER_NUM"]);
                int E_MIN_BRANCH = Convert.ToInt32(StarttracinginfoFeatureSet.Features[0].Attributes["MIN_BRANCH"]);
                int E_MAX_BRANCH = Convert.ToInt32(StarttracinginfoFeatureSet.Features[0].Attributes["MAX_BRANCH"]);
                int E_TREELEVEL = Convert.ToInt32(StarttracinginfoFeatureSet.Features[0].Attributes["TREELEVEL"]);
                int E_TO_FEATURE_FEEDERINFO = Convert.ToInt32(StarttracinginfoFeatureSet.Features[0].Attributes["TO_FEATURE_FEEDERINFO"]);
                int E_TO_FEATURE_EID = Convert.ToInt32(StarttracinginfoFeatureSet.Features[0].Attributes["TO_FEATURE_EID"]);
                int E_TO_FEATURE_OID = Convert.ToInt32(StarttracinginfoFeatureSet.Features[0].Attributes["TO_FEATURE_OID"]);
                int E_TO_FEATURE_FCID = Convert.ToInt32(StarttracinginfoFeatureSet.Features[0].Attributes["TO_FEATURE_FCID"]);
                string E_FEEDERID_tracing = Convert.ToString( StarttracinginfoFeatureSet.Features[0].Attributes["FEEDERID"]);
                int E_TO_CIRCUTID = Convert.ToInt32(StarttracinginfoFeatureSet.Features[0].Attributes["TO_CIRCUITID"]);
                GetServiceLocation_EndDevice(E_FEEDERID_tracing, E_MIN_BRANCH, E_MAX_BRANCH, E_TREELEVEL, E_ORDER_NUM, E_TO_FEATURE_FCID);
            }
            else
            {
                busyIndicator.IsBusy = false;
                RdBetweenDevice.IsChecked = true;
            }
        }

        private void EndTracingTask_Failed(object sender, TaskFailedEventArgs args)
        {
            busyIndicator_Customer.IsBusy = false;
        }
        //Get Service location  to Service Point ID-End Device
        void GetServiceLocation_EndDevice(string FEEDERID, int MIN_BRANCH, int MAX_BRANCH, int TREELEVEL, int ORDER_NUM, int TO_FEATURE_FCID)
        {
            try
            {
                QueryTask EndServiceLocationTask = new QueryTask(_TracingNetworkService);
                EndServiceLocationTask.ExecuteCompleted += EndServiceLocationTask_ExecuteCompleted;
                EndServiceLocationTask.Failed += EndServiceLocationTask_Failed;
                ESRI.ArcGIS.Client.Tasks.Query query = new ESRI.ArcGIS.Client.Tasks.Query();
                string Tran_PriMtr_TO_FEATURE_FCID = _ServiceLocation_FEATURE_FCID;
                if (!string.IsNullOrEmpty(FEEDERID))
                {
                    query.OutFields.AddRange(new string[] { "TO_FEATURE_GLOBALID" });
                    query.OutFields.AddRange(new string[] { "TO_FEATURE_FCID" });
                    query.OutFields.AddRange(new string[] { "TO_CIRCUITID" });

                    query.Where = "(TO_FEATURE_FCID IN (" + Tran_PriMtr_TO_FEATURE_FCID + "," + Start_TO_FEATURE_FCID + "," + End_TO_FEATURE_FCID + ")"+                        
                        " OR (TO_FEATURE_FCID IN (" + PriUGConductorFCID + "," + PriOHConductorFCID + ") AND TO_CIRCUITID IS NOT NULL)) "+
                        "AND MIN_BRANCH >= " + MIN_BRANCH + " AND MAX_BRANCH <= " + MAX_BRANCH
                            + " AND TREELEVEL >= " + TREELEVEL + " AND ORDER_NUM <= " + ORDER_NUM + " AND FEEDERID = '" + FEEDERID + "'"; 
                }
                EndServiceLocationTask.ExecuteAsync(query);
            }
            catch (Exception ex)
            {
                busyIndicator_Customer.IsBusy = false;
                busyIndicator.IsBusy = false;
                logger.FatalException("Error in PONS GetServiceLocation_EndDevice", ex);
            }
        }

        private void EndServiceLocationTask_ExecuteCompleted(object sender, ESRI.ArcGIS.Client.Tasks.QueryEventArgs args)
        {
            string ServiceLocation_EndGUIDstring = string.Empty;
            
            try
            {
                FeatureSet EndDeviceServiceLocationFeatureSet = args.FeatureSet;
                endGUIDList.Clear();
                if (EndDeviceServiceLocationFeatureSet != null && EndDeviceServiceLocationFeatureSet.Features.Count > 0)
                {

                    foreach (Graphic graphic in EndDeviceServiceLocationFeatureSet.Features)
                    {
                        if (!endGUIDList.ContainsKey(Convert.ToString(graphic.Attributes["TO_FEATURE_GLOBALID"])))
                            endGUIDList.Add(Convert.ToString(graphic.Attributes["TO_FEATURE_GLOBALID"]), Convert.ToString(graphic.Attributes["TO_FEATURE_FCID"]));  
                            
                    }
                }

                ProcessDeviceResulst();
            }
            catch (Exception ex)
            {
                busyIndicator_Customer.IsBusy = false;
                busyIndicator.IsBusy = false;
                logger.FatalException("Error in PONS EndServiceLocationTask_ExecuteCompleted", ex);
            }
        }
        private void EndServiceLocationTask_Failed(object sender, TaskFailedEventArgs args)
        {
            busyIndicator.IsBusy = false;
        }

        //Get Service Location to Service Point ID-End Device
        void GetEndServicePoint(string whereInString)
        {
            try
            {
                QueryTask getEndServicePointResultsQuery = new QueryTask(_ServicepointIDService);
                getEndServicePointResultsQuery.ExecuteCompleted += getEndServicePointResultsQuery_ExecuteCompleted;
                getEndServicePointResultsQuery.Failed += getEndServicePointResultsQuery_Failed;
                ESRI.ArcGIS.Client.Tasks.Query query = new ESRI.ArcGIS.Client.Tasks.Query();
                query.OutFields.AddRange(new string[] { "SERVICEPOINTID" });
                query.ReturnGeometry = false;
                query.Where = "SERVICELOCATIONGUID IN ('" + whereInString + "') AND SERVICEPOINTID IS NOT NULL";
                getEndServicePointResultsQuery.ExecuteAsync(query);


            }
            catch (Exception ex)
            {
                busyIndicator.IsBusy = false;
                logger.FatalException("Error in PONS GetEndServicePoint", ex);
            }
        }

        private void getEndServicePointResultsQuery_ExecuteCompleted(object sender, QueryEventArgs e)
        {
            string strSPointID = string.Empty;
            FeatureSet EndDeviceServicePointFeatureSet = e.FeatureSet;
            try
            {
                if (EndDeviceServicePointFeatureSet != null && EndDeviceServicePointFeatureSet.Features.Count > 0)
                {

                    for (int i = 0; i < EndDeviceServicePointFeatureSet.Features.Count; i++)
                    {
                        ListStrServicePointID_EndDevice.Add(EndDeviceServicePointFeatureSet.Features[i].Attributes["SERVICEPOINTID"].ToString());
                    }
                    for (int i = 0; i < ListStrServicePointID_EndDevice.Count; i++)
                    {
                        if (i == 0)
                        {
                            strSPointID = ListStrServicePointID_EndDevice.ElementAt(i);
                        }
                        else
                        {
                            strSPointID = String.Concat(strSPointID, ",", ListStrServicePointID_EndDevice.ElementAt(i));
                        }
                    }
                    StrServicePointID_EndDevice = strSPointID.ToString();

                    //Between two device service point
                    List<string> DeviceServicePointList = new List<string>();
                    if (ListStrServicePointID_StartDevice.Count > ListStrServicePointID_EndDevice.Count)
                    {
                        var Resultbetween_device = ListStrServicePointID_StartDevice.Except(ListStrServicePointID_EndDevice, StringComparer.OrdinalIgnoreCase).ToList();


                        DeviceServicePointList = Resultbetween_device.ToList();
                    }
                    else
                    {

                        var Resultbetween_Edevice = ListStrServicePointID_EndDevice.Except(ListStrServicePointID_StartDevice, StringComparer.OrdinalIgnoreCase).ToList();


                        DeviceServicePointList = Resultbetween_Edevice.ToList();
                    }
                    string strDeviceServicePointList = string.Empty;
                    for (int i = 0; i < DeviceServicePointList.Count; i++)
                    {
                        if (i == 0)
                        {
                            strDeviceServicePointList = DeviceServicePointList.ElementAt(i);
                        }
                        else
                        {
                            strDeviceServicePointList = String.Concat(strDeviceServicePointList, ",", DeviceServicePointList.ElementAt(i));
                        }
                    }
                    strDeviceServicePointListResult = strDeviceServicePointList.ToString();
                }
                busyIndicator.IsBusy = false;
            }
            catch (Exception ex)
            {
                busyIndicator.IsBusy = false;
                logger.FatalException("Error in PONS getEndServicePointResultsQuery_ExecuteCompleted", ex);
            }

        }

        void getEndServicePointResultsQuery_Failed(object sender, TaskFailedEventArgs e)
        {
            busyIndicator.IsBusy = false;
        }

        #endregion
        //Adding Code for Search Result[Start]
        private void ClientCustomerResultHandle_Async(List<TransformercustomerdataComb> objCustomerResult)
        {
           
            List<TransformercustomerdataComb> objTransResult = new List<TransformercustomerdataComb>();


            //objTransResult = objCustomerResult.OrderBy(c => c.TransSSD, new CustomComparer())
            //                                  .ThenBy(c => c.CGC12)
            //                                  .ThenBy(c => (c.ORD == 1 ? (c.CustMailStName1 + c.CustMailStName2) : (c.CustMailStName1 + c.CustMailStName2)))
            //                                  .ThenBy(c => (c.ORD == 1 ? (c.CustMailStNum) : (c.CustMailStNum)))                                              
            //                                  .ToList();

            foreach (TransformercustomerdataComb tc in objCustomerResult.OrderBy(c => c.TransSSD, new CustomComparer())
                                              .ThenBy(c => c.CGC12, new CustomComparer())
                                              .ThenBy(c => (c.CustMailStName1 + c.CustMailStName2))
                                              .ThenBy(c => (c.CustMailStNum))                                        
                                              .ToList())
            {
                if (objCustomerResult.Count(c => c.ServicepointId == tc.ServicepointId) > 1)
                {
                    if (tc.ORD == 1) continue;
                    
                    if (chk_serviceAddress.IsChecked == true)
                    {
                        TransformercustomerdataComb mailObj = objCustomerResult.First(c => c.ServicepointId == tc.ServicepointId && c.ORD != tc.ORD);
                        if (mailObj != null)
                        {
                            tc.AverySTNameNumber = mailObj.AverySTNameNumber;
                            tc.GridADDCol = mailObj.GridADDCol;
                            tc.AddressCust = mailObj.AddressCust;
                            tc.GridADDCol = mailObj.GridADDCol;
                            tc.CustMail1 = mailObj.CustMail1;
                            tc.CustMail2 = mailObj.CustMail2;
                            tc.CustMailStName1 = mailObj.CustMailStName1;
                            tc.CustMailStName2 = mailObj.CustMailStName2;
                            tc.CustMailStNum = mailObj.CustMailStNum;
                            tc.CustomerName = mailObj.CustomerName;
                            objTransResult.Add(tc);
                        }
                        else
                            objTransResult.Add(tc);
                    }
                    else
                    {
                        try
                        {
                            tc.CustMail1 = "PG&E Customer at";
                            tc.CustMail2 = "";
                            tc.CustomerName = "PG&E Customer at";
                            objTransResult.Add(objCustomerResult.First(c => c.ServicepointId == tc.ServicepointId && c.ORD != tc.ORD));
                            // objTransResult.Add(objCustomerResult.First(c => c.ServicepointId == tc.ServicepointId ));
                            objTransResult.Add(tc);
                        }
                        catch {
                            tc.CustMail1 = "PG&E Customer at";
                            tc.CustMail2 = "";
                            tc.CustomerName = "PG&E Customer at";
                          
                             objTransResult.Add(objCustomerResult.First(c => c.ServicepointId == tc.ServicepointId ));
                            objTransResult.Add(tc);
                        }
                       
                    }
                    
                }
                else
                {
                    objTransResult.Add(tc);
                }
            }
            //Add
            objTransResult = objTransResult.Distinct().ToList();
            //
            objCustomerResult = objTransResult;
            if (objTransResult.Count != 0)
            {
                try
                {
                    CustomerDataGrid.ItemsSource = null;
                    CustomerResult.Clear();
                    SelectAllCustomerResult.Clear();
                    AveryCustomerResult.Clear();
                    AddCustomerResult.Clear();
                    if (objTransResult != null && objTransResult.Count != 0)
                    {
                        var Sd = objTransResult;
                        for (int i = 0; i < Sd.Count(); i++)
                        {                          
                            Pdfcustomerdata pdfCustomer = new Pdfcustomerdata();                                                       
                            pdfCustomer.SlNo = i + 1;
                            pdfCustomer.CustType = Sd.ElementAt(i).CustType;
                            pdfCustomer.ServicepointId = Sd.ElementAt(i).ServicepointId;
                            pdfCustomer.MeterNum = Sd.ElementAt(i).MeterNum;
                            pdfCustomer.GridADDCol = Sd.ElementAt(i).GridADDCol;
                            pdfCustomer.CustomerName = Sd.ElementAt(i).CustomerName;
                            pdfCustomer.CustMail1 = Sd.ElementAt(i).CustMail1;
                            pdfCustomer.CustMail2 = Sd.ElementAt(i).CustMail2;
                            pdfCustomer.AverySTNameNumber = Sd.ElementAt(i).AverySTNameNumber;
                            pdfCustomer.AddressCust = Sd.ElementAt(i).AddressCust;
                            if (CGCTNumMapping.ContainsKey(Sd.ElementAt(i).CGC12.ToString()) == true)
                                pdfCustomer.CGC12 = CGCTNumMapping[Sd.ElementAt(i).CGC12.ToString()];
                            else 
                                pdfCustomer.CGC12 = Sd.ElementAt(i).CGC12;
                            pdfCustomer.TransSSD = Sd.ElementAt(i).TransSSD;
                            pdfCustomer.Checked = true;
                            pdfCustomer.CustPhone = Sd.ElementAt(i).CustAreaCode + Sd.ElementAt(i).CustPhone;
                            pdfCustomer.ORD = Sd.ElementAt(i).ORD;
                            pdfCustomer.PREMISEID = Sd.ElementAt(i).PREMISEID;
                            pdfCustomer.PREMISETYPE = Sd.ElementAt(i).PREMISETYPE;
                            CustomerResult.Add(pdfCustomer);
                            SelectAllCustomerResult.Add(pdfCustomer);
                        }
                        

                        CustomerDataGrid.ItemsSource = CustomerResult.ToList();
                        CreateAveryReport_Async(objCustomerResult);
                        CreateAddReport_Async(objCustomerResult);
                        int finalcount = CustomerResult.Count;
                        //add Dec-2017
                        customercount_checkbox = finalcount;
                        Chcountcustomer.IsChecked = true;
                        SelectAllcustomercount = finalcount;
                        //SelectAllCustomerResult = CustomerResult;
                        //
                        CustCount.Text = Convert.ToString(finalcount);

                        var mtrList = CustomerResult.Select(mtCount => mtCount.ServicepointId).Distinct().ToList();
                        SelectAllMetercustomercount = mtrList.Count();
                        MetCount.Text = mtrList.Count().ToString();

                        if (finalcount == 0)
                        {
                            btnNextsearchlist.IsEnabled = false;
                        }
                        else
                        {
                            btnNextsearchlist.IsEnabled = true;
                        }
                        busyIndicator_Customer.IsBusy = false;
                    }
                    busyIndicator_Customer.IsBusy = false;
                }
                catch (Exception ex)
                {
                    btnNextsearchlist.IsEnabled = false;
                    busyIndicator_Customer.IsBusy = false;
                    logger.FatalException("Error in PONS Customer_CombData", ex);
                }
            }
            else
            {
                MessageBox.Show("No Customer Data in the table");
                btnNextsearchlist.IsEnabled = true;// false;
                busyIndicator_Customer.IsBusy = false;
            }
            SetTabFocus(btnNextsearchlist);
        }

        //Adding new function for search one
        private string GetServicePointsFS()
        {
            string strServicePoint = "";
            if (StrServicePointID_StartDevice != "" && strDeviceServicePointListResult != "")
            {

                strServicePoint = strDeviceServicePointListResult;
            }
            else if (StrServicePointID_StartDevice != "")
            {
                strServicePoint = StrServicePointID_StartDevice;
            }
            return strServicePoint;
        }

        private ArrayList arrFFF_FeederID_Trace = new ArrayList();

        //Get Customer Data from search 
        private void GetCustomer_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                CustomerList.Clear();
                AddCustomerList.Clear();
                AveryCustomerList.Clear();
                CustomerDataGrid.ItemsSource = null;
                CustCount.Text = "0";
                MetCount.Text = "0";
                ArrayList arrCGC = new ArrayList();
                ArrayList arrCircuit = new ArrayList();
                string cgsno = "";
                string strcircuitno = "";
                string strDivisionCode = "";
                string strFlag = "";
                string cgs = string.Empty;
                string servicepointId = string.Empty;
                string strcircuit = string.Empty;

                //add below code
                arrCircuit.Clear();
                arrCGC.Clear();
                //
                if (SearchlistDataGrid.Items.Count > 0)
                {
                    busyIndicator_Customer.IsBusy = true;
                    if (_deviceSearchItemList.Count > 0)
                    {
                        foreach (DeviceSearchItem item in _deviceSearchItemList)
                        {
                            foreach (string cgc in item.CGCList)
                            {
                                servicepointId = servicepointId + cgc + ",";
                            }
                        }
                        if (servicepointId != "")
                            servicepointId = servicepointId.Remove(servicepointId.Length - 1, 1);
                    }
                   
                    arrCGC = GetCGCNumber();
                    arrCircuit.AddRange(GetCircuiteIds());
                    arrCircuit.AddRange(arrFFF_FeederID_Trace);
                    if (arrCGC.Count > 0)
                    {
                        foreach (object o in arrCGC)
                        {
                            cgsno = o.ToString();
                            cgs = cgs + cgsno + ",";
                        }
                        cgs = cgs.TrimEnd(cgs[cgs.Length - 1]);
                    }
                    if (arrCircuit.Count > 0)
                    {
                        foreach (object o in arrCircuit)
                        {
                            strcircuitno = o.ToString();
                            strcircuit = strcircuit + strcircuitno + ",";
                        }
                        strcircuit = strcircuit.TrimEnd(strcircuit[strcircuit.Length - 1]);
                    }
                    //Adding two more parameter
                    string strSelectedDivName = dddivisionSearch.SelectedItem.ToString();
                    if (chk_serviceAddress.IsChecked == false)
                    {
                        strFlag = "0";
                    }
                    else
                    {
                        strFlag = "1";
                    }
                    if (strSelectedDivName != "Select...")
                    {
                        strDivisionCode = GetDivisionCodes(strSelectedDivName).ToString();
                    }
                    if (cgs != "" || strcircuit != "" || _deviceSearchItemList.Count > 0)
                    {
                        busyIndicator_Customer.IsBusy = true;
                        EndpointAddress endPoint = new EndpointAddress(endpointaddress);
                        BasicHttpBinding binding = new BasicHttpBinding();
                        binding.MaxBufferSize = 2147483647;
                        binding.MaxReceivedMessageSize = 2147483647;
                        binding.TransferMode = TransferMode.Buffered;
                        IServicePons cusservice = new ChannelFactory<IServicePons>(binding, endPoint).CreateChannel();
                        cusservice.BeginGetCustomerdataComb(cgs, strcircuit, servicepointId, strDivisionCode, strFlag,
                        delegate(IAsyncResult result)
                        {
                            List<TransformercustomerdataComb> CombindUserList = ((IServicePons)result.AsyncState).EndGetCustomerdataComb(result);
                            this.Dispatcher.BeginInvoke(
                            delegate()
                            {
                                ClientCustomerResultHandle_Async(CombindUserList);

                            }
                            );
                        }, cusservice);
                    }
                    else
                    {
                        btnNextsearchlist.IsEnabled = false;
                        busyIndicator_Customer.IsBusy = false;
                    }
                    tabItem1.Visibility = Visibility.Collapsed;
                    tabItem2.Visibility = Visibility.Collapsed;
                    tabItem4.Visibility = Visibility.Visible;
                    tabItem5.Visibility = Visibility.Collapsed;
                    tabItem6.Visibility = Visibility.Collapsed;
                    tabItem7.Visibility = Visibility.Collapsed;
                    tabItem8.Visibility = Visibility.Collapsed;
                    tabItem9.Visibility = Visibility.Collapsed;

                    if (MapEventIsOn == true)
                    {
                        SelectPolygonEvent(false);
                    }
                }
                else
                {
                    busyIndicator_Customer.IsBusy = false;
                    MessageBox.Show("At least one result should be available in Search List.");
                    return;
                }
            }
            catch (Exception ex)
            {
                busyIndicator_Customer.IsBusy = false;
                logger.FatalException("Error in PONS GetCustomer_Click", ex);
            }
            finally
            {
            }
        }

        #region for Web Service Code Start
        public void SendEmailToPSL(string strURL)
        {
            try
            {
                WebClient client = new WebClient();
                UriBuilder uriBuilder = new UriBuilder(strURL);
                client.DownloadStringAsync(uriBuilder.Uri);
            }
            catch (Exception ex)
            {
                logger.FatalException("Error in GetTransformercusdataComb", ex);
            }
        }
        public void SendSequeceEmailDataSend(string strURL)
        {
            try
            {
                WebClient client = new WebClient();
                UriBuilder uriBuilder = new UriBuilder(strURL);
                client.DownloadStringCompleted += new DownloadStringCompletedEventHandler(AddPlannedShutdownID);
                client.DownloadStringAsync(uriBuilder.Uri);
            }
            catch (Exception ex)
            {
                logger.FatalException("Error in GetTransformercusdataComb", ex);
            }
        }

        void AddPlannedShutdownID(object sender, DownloadStringCompletedEventArgs e)
        {
            try
            {
                string result = "";
                string currentUser = WebContext.Current.User.Name;
                string[] UserName = currentUser.Split('\\');
                string strUSERNAME = UserName[1].ToString();
                List<ShutDownID> objShutIDResult = new List<ShutDownID>();
                result = e.Result;
                objShutIDResult = JsonHelper.Deserialize<List<ShutDownID>>(result);
                if (objShutIDResult != null)
                {
                    StrPLANNEDSHUTDOWNID = objShutIDResult[0].SDownID.ToString();
                }
                string shutdownID = "";
                if (StrPLANNEDSHUTDOWNID != "")
                {
                    shutdownID = StrPLANNEDSHUTDOWNID;
                }
                string strSecondTimeOFF = "";
                string strSecondTimeON = "";
                string strSecondDateOff = "";

                string[] arrstrSecondTimeOff = null;
                string[] arrstrSecondTimeOn = null;

                string strFrstTimeOFF = "";
                string strFrstTimeON = "";
                string strDateOff = "";
                string[] strTimeOff = null;
                string[] strTimeOn = null;

                string strDateTime = "";
                string strLocationDecs = "";
                string recName = "";
                string emailTo = "";
                string strSelect = "Select...";
                recName = ddPSC.SelectedItem.ToString();
                if (recName != strSelect)
                {
                    recName = ddPSC.SelectedItem.ToString();
                }
                else
                {
                    MessageBox.Show("Please select the PSC Name.");
                    return;
                }
                string[] s = (string[])(ddPSC.SelectedItem as Item).Value.Split(':');
                string FistName = s[0];
                string LastName = s[1];
                string LAN = s[2];
                string Division = s[3];
                int intDivCode = GetDivisionCodes(Division);
                string strContactID = s[4];
                emailTo = LAN.Trim();
                //Adding email parameter for PSC

                if (timePickerFrstOff.GetSelectedValue() != null)
                {
                    strFrstTimeOFF = timePickerFrstOff.GetSelectedValue().ToString();
                }
                if (timePickerFrstOn.GetSelectedValue() != null)
                {
                    strFrstTimeON = timePickerFrstOn.GetSelectedValue().ToString();
                }
                //Adding checks for blank time values
                if (strFrstTimeOFF== "" && strFrstTimeON == "")
                {
                    MessageBox.Show("Please enter shutdowntime off and shutdowntime on both.");
                    return;
                }
                if (strFrstTimeOFF != "")
                {
                    strTimeOff = strFrstTimeOFF.Split(' ');
                    string[] times = strTimeOff[1].ToString().Split(':');
                    strFrstTimeOFF = times[0] + ":" + times[1] + " " + strTimeOff[2].ToString();
                    
                }
                if (strFrstTimeON != "")
                {
                    strTimeOn = strFrstTimeON.Split(' ');
                    string[] times = strTimeOn[1].ToString().Split(':');
                    strFrstTimeON = times[0] + ":" + times[1] + " " + strTimeOn[2].ToString();
                    //strFrstTimeON = strTimeOn[1].ToString() + " " + strTimeOn[2].ToString();
                }
                //Adding Second date and Time
                System.Globalization.DateTimeFormatInfo mfi = new System.Globalization.DateTimeFormatInfo();
                if (datePickerFstShutDateOff.SelectedDate != null)
                {
                    strDateOff = datePickerFstShutDateOff.SelectedDate.Value.DayOfWeek + " " + datePickerFstShutDateOff.SelectedDate.Value.Day + ", " + mfi.GetMonthName(datePickerFstShutDateOff.SelectedDate.Value.Month).ToString() + " " + datePickerFstShutDateOff.SelectedDate.Value.Year;
                }
                if (timePickerScndOff.GetSelectedValue() != null)
                {
                    strSecondTimeOFF = timePickerScndOff.GetSelectedValue().ToString();
                }
                if (timePickerScndOn.GetSelectedValue() != null)
                {
                    strSecondTimeON = timePickerScndOn.GetSelectedValue().ToString();
                }
                if (strSecondTimeOFF != "")
                {
                    arrstrSecondTimeOff = strSecondTimeOFF.Split(' ');
                    string[] times = arrstrSecondTimeOff[1].ToString().Split(':');
                    strSecondTimeOFF = times[0] + ":" + times[1] + " " + arrstrSecondTimeOff[2].ToString();
                    //strSecondTimeOFF = arrstrSecondTimeOff[1].ToString() + " " + arrstrSecondTimeOff[2].ToString();
                }
                if (strSecondTimeON != "")
                {
                    arrstrSecondTimeOn = strSecondTimeON.Split(' ');
                    string[] times = arrstrSecondTimeOn[1].ToString().Split(':');
                    strSecondTimeON = times[0] + ":" + times[1] + " " + arrstrSecondTimeOn[2].ToString();
                    //strSecondTimeON = arrstrSecondTimeOn[1].ToString() + " " + arrstrSecondTimeOn[2].ToString();
                }

                if (datePickerScndOff.SelectedDate != null)
                {
                    strSecondDateOff = datePickerScndOff.SelectedDate.Value.DayOfWeek + " " + datePickerScndOff.SelectedDate.Value.Day + ", " + mfi.GetMonthName(datePickerScndOff.SelectedDate.Value.Month).ToString() + " " + datePickerScndOff.SelectedDate.Value.Year;
                }

                if (AddDate.IsChecked == true)
                {
                    strDateTime = "From " + strDateOff + " Time Off: " + strFrstTimeOFF + " and Time On: " + strFrstTimeON + " And " + "From " + strSecondDateOff + " Time Off: " + strSecondTimeOFF + " and Time On: " + strSecondTimeON;
                }
                else
                {
                    strDateTime = "From " + strDateOff + " Time Off: " + strFrstTimeOFF + " and Time On: " + strFrstTimeON;
                }
                strLocationDecs = " " + txtoutagearea.Text.ToString();

                //Adding code for insert the customer info into the PSL database[Start]
                DateTime firstShutOffDate = (DateTime)datePickerFstShutDateOff.SelectedDate;
                DateTime firstShutOffTime = (DateTime)timePickerFrstOff.Value;
                DateTime firstTurnOnTime = (DateTime)timePickerFrstOn.Value;
                DateTime SecondShutOffDate = default(DateTime);
                
                if (datePickerScndOff.SelectedDate != null && AddDate.IsChecked == true)
                {
                    SecondShutOffDate = (DateTime)datePickerScndOff.SelectedDate;
                }
                DateTime SecondShutOffTime = default(DateTime);
                if (timePickerScndOff.Value != null && AddDate.IsChecked == true)
                {
                    SecondShutOffTime = (DateTime)timePickerScndOff.Value;
                }
                DateTime SecondTurnOnTime = default(DateTime);
                if (timePickerScndOn.Value != null & AddDate.IsChecked == true)
                {
                    SecondTurnOnTime = (DateTime)timePickerScndOn.Value;
                }                

                if (AddDate.IsChecked == true)
                {
                    if (datePickerScndOff.SelectedDate != null && timePickerScndOff.Value != null && timePickerScndOn.Value != null)
                    {
                        SecondShutOffDate = (DateTime)datePickerScndOff.SelectedDate;
                        SecondShutOffTime = (DateTime)timePickerScndOff.Value;
                        SecondTurnOnTime = (DateTime)timePickerScndOn.Value;
                    }
                }

                int numshutdownID = Convert.ToInt32(shutdownID);
                List<ArcFMSilverlight.StagingdataCust> uiDataCollection = new List<ArcFMSilverlight.StagingdataCust>();
                EndpointAddress endPoint = new EndpointAddress(endpointaddress);
                BasicHttpBinding binding = new BasicHttpBinding();
                binding.MaxBufferSize = 2147483647;
                binding.MaxReceivedMessageSize = 2147483647;
                binding.TransferMode = TransferMode.Buffered;
                IServicePons cusservice = new ChannelFactory<IServicePons>(binding, endPoint).CreateChannel();

                foreach (Addcustomerdata data in AddCustomerResult)
                {
                    StagingdataCust customerData = new StagingdataCust();
                    customerData.AddrLine1 = data.AddrLine1;
                    customerData.AddrLine2 = data.AddrLine2;
                    
                    customerData.CGCTNum = data.CGCTNum;
                    customerData.SSD = data.SSD;
                    customerData.Phone = data.Phone;
                    customerData.AccountID = data.AccountID;
                    customerData.SPID = data.SPID;

                    customerData.AddrLine3 = data.AddrLine3;
                    customerData.AddrLine4 = data.AddrLine4;
                    customerData.AddrLine5 = data.AddrLine5;
                    customerData.SPAddrLine1 = data.SPAddrLine1;
                    customerData.SPAddrLine2 = data.SPAddrLine2;
                    customerData.SPAddrLine3 = data.SPAddrLine3;
                   
                    customerData.CustType = data.CustType;
                    customerData.meter_number = data.meter_number;
                    uiDataCollection.Add(customerData);

                }
                string strDiveCode = intDivCode.ToString();
                string _Status = "";
                if (uiDataCollection.Count <= 20000)
                {
                    cusservice.BeginSendtoPSL(uiDataCollection, numshutdownID, strContactID, strDiveCode, firstShutOffDate.ToString(), firstShutOffTime.ToString(), firstTurnOnTime.ToString(), SecondShutOffDate.ToString(), SecondShutOffTime.ToString(), SecondTurnOnTime.ToString(), strLocationDecs, strUSERNAME,
                        delegate(System.IAsyncResult sresult)
                        {
                            _Status = ((IServicePons)sresult.AsyncState).EndSendtoPSL(sresult).ToString();
                            this.Dispatcher.BeginInvoke(
                            delegate()
                            {
                                if (_Status != "")
                                {
                                    SendEmailNotification(_Status, strUSERNAME, shutdownID, strDateTime, strLocationDecs);
                                }
                            }
                            );
                        }, cusservice);
                }
                else
                {
                    busyEmailIndicator.IsBusy = false;
                    MessageBox.Show("System is not able to send more than 20000 letters to PSL.");
                    return;
                }
            }
            catch (Exception ex)
            {
                busyEmailIndicator.IsBusy = false;
                logger.FatalException("Error in PONS ", ex);
            }
        }
        public void SendEmailNotification(string strStatus, string strUSERNAME, string shutdownID, string strDateTime, string strLocationDecs)
        {
            string emailTo = "";
            string stremailURL = "";
            try
            {
                string[] s = (string[])(ddPSC.SelectedItem as Item).Value.Split(':');
                string FistName = s[0];
                string LastName = s[1];
                string LAN = s[2];
                string Division = s[3];
                int intDivCode = GetDivisionCodes(Division);
                string strContactID = s[4];
                emailTo = LAN.Trim();

                if (strStatus.ToUpper() == "Succcess".ToUpper())
                {
                    if (ChPSCEmail.IsChecked == true && strUSERNAME.ToUpper() != LAN.ToUpper())
                    {
                        stremailURL = objPonsServiceCall.GetSendEmailAddress("1", emailTo + "@pge.com", shutdownID, strUSERNAME, "", strDateTime, strLocationDecs);
                        SendEmailToPSL(stremailURL);
                    }
                    else
                    {
                        stremailURL = objPonsServiceCall.GetSendEmailAddress("0", emailTo + "@pge.com", shutdownID, strUSERNAME, "", strDateTime, strLocationDecs);
                        SendEmailToPSL(stremailURL);
                    }

                    MessageBox.Show("Notification request has been sent. \n If you need to refer to this request, the request ID is " + shutdownID);
                    busyEmailIndicator.IsBusy = false;
                }
                else
                {
                    busyEmailIndicator.IsBusy = false;
                    MessageBox.Show("No data has sent to PSL");
                    return;
                }
            }
            catch (Exception ex)
            {
                busyEmailIndicator.IsBusy = false;
                logger.FatalException("Error in PONS SendEmailNotification", ex);
            }
        }
        public void GetTransformercusdataComb(string strURL)
        {
            try
            {
                WebClient client = new WebClient();
                UriBuilder uriBuilder = new UriBuilder(strURL);
                client.DownloadStringCompleted += new DownloadStringCompletedEventHandler(ClientCustomerResultHandle);
                client.DownloadStringAsync(uriBuilder.Uri);
            }
            catch (Exception ex)
            {
                logger.FatalException("Error in GetTransformercusdataComb", ex);
            }
        }

        //Adding function to populate the dropdown for substation, bank and Circuit

        public void GetSubstationBankCircuitData(string strURL)
        {
            try
            {
                WebClient client = new WebClient();
                UriBuilder uriBuilder = new UriBuilder(strURL);
                client.DownloadStringCompleted += new DownloadStringCompletedEventHandler(fillDropdownSubBankCircuit);
                client.DownloadStringAsync(uriBuilder.Uri);
            }
            catch (Exception ex)
            {
                logger.FatalException("Error in GetSubstationBankCircuitData", ex);
            }
        }

       
        private void fillDropdownSubBankCircuit(object sender, DownloadStringCompletedEventArgs e)
        {
            try 
            {
                string result = "";
                result = e.Result;
                objSBCDataResult = new List<SubStationSearchData>();
                objSBCDataResult = JsonHelper.Deserialize<List<SubStationSearchData>>(result);
                string strSelect = "Select...";
                Item itemSub1 = new Item(strSelect, "");
                Item itemBank1 = new Item(strSelect, "");
                Item itemCircuit1 = new Item(strSelect, "");
                // To fill Substation
                if (objSBCDataResult.Count > 0)
                {
                    foreach (SubStationSearchData sbc in objSBCDataResult)
                    {
                        string strSubName = sbc.SubStationName;
                        string strsubstation = sbc.SubStationID + "     " + sbc.SubStationName ;
                        string strbank = sbc.BANK;
                        string strcircuit = sbc.CircuitID;

                        if (itemSub1.Name.Contains(strsubstation) == false)
                        {
                            itemSub1 = new Item(strsubstation, strSubName);
                            ddsubstation.Items.Add(itemSub1);
                            strsubstation = "";
                        }
                       
                        itemBank1 = new Item(strbank, "");
                        itemCircuit1 = new Item(strcircuit, "");
                        ddbank.Items.Add(itemBank1);
                        ddcircuit.Items.Add(itemCircuit1);
                    }
                }
                ddsubstation.SelectedIndex = 0;
                ddbank.SelectedIndex = 0;
                ddcircuit.SelectedIndex = 0;
            }
            catch(Exception ex)
            {

            }
        }

        public void GetBanKData(string strURL)
        {
            try
            {
                WebClient client = new WebClient();
                UriBuilder uriBuilder = new UriBuilder(strURL);
                client.DownloadStringCompleted += new DownloadStringCompletedEventHandler(fillDropdownBank);
                client.DownloadStringAsync(uriBuilder.Uri);
            }
            catch (Exception ex)
            {
                logger.FatalException("Error in GetBanKData", ex);
            }
        }
        private void fillDropdownBank(object sender, DownloadStringCompletedEventArgs e)
        {
            try
            {
                string result = "";
                result = e.Result;
                List<SubStationSearchData> objBDataResult = new List<SubStationSearchData>();
                objBDataResult = JsonHelper.Deserialize<List<SubStationSearchData>>(result);
                string strSelect = "Select...";
                Item itemBank1 = new Item(strSelect, "");
                // To fill Bank
                if (objBDataResult.Count > 0)
                {
                    foreach (SubStationSearchData sbc in objBDataResult)
                    {
                        string strbank = sbc.BANK;
                        itemBank1 = new Item(strbank, "");
                        ddbank.Items.Add(itemBank1);
                    }
                }
                ddbank.SelectedIndex = 0;
            }
            catch (Exception ex)
            {

            }
        }

        public void GetCircuitData(string strURL)
        {
            try
            {
                WebClient client = new WebClient();
                UriBuilder uriBuilder = new UriBuilder(strURL);
                client.DownloadStringCompleted += new DownloadStringCompletedEventHandler(fillDropdownCircuit);
                client.DownloadStringAsync(uriBuilder.Uri);
            }
            catch (Exception ex)
            {
                logger.FatalException("Error in GetCircuitData", ex);
            }
        }
        
        private void fillDropdownCircuit(object sender, DownloadStringCompletedEventArgs e)
        {
            try
            {
                string result = "";
                result = e.Result;
                List<SubStationSearchData> objCDataResult = new List<SubStationSearchData>();
                objCDataResult = JsonHelper.Deserialize<List<SubStationSearchData>>(result);
                string strSelect = "Select...";
                Item itemCircuit1 = new Item(strSelect, "");
                // To fill Substation
                if (objCDataResult.Count > 0)
                {
                    foreach (SubStationSearchData sbc in objCDataResult)
                    {
                         string strcircuit = sbc.CircuitID;
                         itemCircuit1 = new Item(strcircuit, "");
                         ddcircuit.Items.Add(itemCircuit1);
                        
                    }
                }
                ddcircuit.SelectedIndex = 0;
            }
            catch (Exception ex)
            {

            }
        }

        public void GetPSLContactData(string strURL)
        {
            try
            {
                WebClient client = new WebClient();
                UriBuilder uriBuilder = new UriBuilder(strURL);
                client.DownloadStringCompleted += new DownloadStringCompletedEventHandler(PSCNameNotification);
                client.DownloadStringAsync(uriBuilder.Uri);
            }
            catch (Exception ex)
            {
                logger.FatalException("Error in GetPSLContactData", ex);
            }
        }
        
        private void PSCNameNotification(object sender, DownloadStringCompletedEventArgs e)
        {
            string result = "";
            result = e.Result;
            List<PSLData> objPSLDataResult = new List<PSLData>();
            objPSLDataResult = JsonHelper.Deserialize<List<PSLData>>(result);
            string strSelect = "Select...";
            Item item1 = new Item(strSelect, "");
            ddPSC.Items.Add(item1);
            if (objPSLDataResult.Count > 0)
            {
                foreach (PSLData psc in objPSLDataResult)
                {
                    string s = psc.FName + ":" + psc.LName + ":" + psc.LanID + ":" + psc.Division + ":" + psc.ContactID;
                    item1 = new Item("[" + psc.Division + "]     " + psc.FName + " " + psc.LName, s);
                    ddPSC.Items.Add(item1);
                }
            }
            ddPSC.SelectedIndex = 0;
        }
        
        private void ClientCustomerResultHandle(object sender, DownloadStringCompletedEventArgs e)
        {
            string result = "";
            List<TransformercustomerdataComb> objTransResult = new List<TransformercustomerdataComb>();
            result = e.Result;
            if (!string.IsNullOrEmpty(result) && objTransResult.Count != 0)
            {


                using (MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(result)))
                {
                    DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(List<TransformercustomerdataComb>));
                    if (serializer.ToString() == null)
                    {
                        busyIndicator_Customer.IsBusy = false;
                    }
                    else
                    {
                        objTransResult = (List<TransformercustomerdataComb>)serializer.ReadObject(ms);
                    }
                }
                try
                {
                    CustomerDataGrid.ItemsSource = null;
                    int itemCount = CustomerResult.Count;
                    if (itemCount > 0)
                    {
                        for (int i = itemCount - 1; i >= 0; i--)
                        {
                            CustomerResult.RemoveAt(i);
                        }
                    }
                    int AveryCount = AveryCustomerResult.Count;
                    if (AveryCount > 0)
                    {
                        for (int i = AveryCount - 1; i >= 0; i--)
                        {
                            AveryCustomerResult.RemoveAt(i);
                        }
                    }
                    int intPSLCustCnt = AddCustomerResult.Count;
                    if (intPSLCustCnt > 0)
                    {
                        for (int i = intPSLCustCnt - 1; i >= 0; i--)
                        {
                            AddCustomerResult.RemoveAt(i);
                        }
                    }
                    if (objTransResult != null && objTransResult.Count != 0)
                    {
                        var Sd = objTransResult;
                        for (int i = 0; i < Sd.Count(); i++)
                        {
                            CustomerResult.Add(new Pdfcustomerdata()
                            {
                                SlNo = i + 1,
                                CustType = Sd.ElementAt(i).CustType,
                                ServicepointId = Sd.ElementAt(i).ServicepointId,
                                MeterNum = Sd.ElementAt(i).MeterNum,
                                GridADDCol = Sd.ElementAt(i).GridADDCol,
                                CustMail1 = Sd.ElementAt(i).CustMail1,
                                AverySTNameNumber = Sd.ElementAt(i).AverySTNameNumber,
                                AddressCust = Sd.ElementAt(i).AddressCust,
                                CGC12 = Sd.ElementAt(i).CGC12,
                                TransSSD = Sd.ElementAt(i).TransSSD,
                                CustPhone = Sd.ElementAt(i).CustAreaCode + Sd.ElementAt(i).CustPhone,
                                ORD = Sd.ElementAt(i).ORD ,
                                PREMISEID=Sd.ElementAt(i).PREMISEID,
                                PREMISETYPE=Sd.ElementAt(i).PREMISETYPE
                            });
                        }
                        CustomerDataGrid.ItemsSource = CustomerResult.ToList();
                        CreateAveryReport(e);
                        CreateAddReport(e);
                        int finalcount = CustomerResult.Count;
                        CustCount.Text = Convert.ToString(finalcount);

                        var mtrList = CustomerList.Select(mtCount => mtCount.ServicepointId).Distinct().ToList();
                        MetCount.Text = mtrList.Count().ToString();

                        busyIndicator_Customer.IsBusy = false;
                    }
                    busyIndicator_Customer.IsBusy = false;
                }
                catch (Exception ex)
                {
                    busyIndicator_Customer.IsBusy = false;
                    logger.FatalException("Error in PONS Customer_CombData", ex);
                }
            }
            else
            {
                MessageBox.Show("No Customer Data in the table");
                busyIndicator_Customer.IsBusy = false;
            }

        }

        private void CreateAveryReport_Async(List<TransformercustomerdataComb> CombCustomerList)
        {
            List<TransformercustomerdataComb> objTransResult = new List<TransformercustomerdataComb>();
            objTransResult = CombCustomerList;
            var Sd = objTransResult;
            int maxlength = 25;
            try
            {
                for (int i = 0; i < Sd.Count; i++)
                {
                    Averycustomerdata averydata = new Averycustomerdata();
                    averydata.SlNo = i + 1;

                    if (Sd.ElementAt(i).CustMailStNum.ToString() != "")
                        averydata.City = Sd.ElementAt(i).CustMailStNum;

                    if (averydata.City != "" && Sd.ElementAt(i).CustMailStName1 != "")
                    {
                        if (Sd.ElementAt(i).CustMailStName1.ToString().Length > maxlength)
                            averydata.City = averydata.City + " " + Sd.ElementAt(i).CustMailStName1.Substring(0, maxlength);
                        else
                            averydata.City = averydata.City + " " + Sd.ElementAt(i).CustMailStName1;
                    }
                    else if (Sd.ElementAt(i).CustMailStName1 != "")
                    {
                        if (Sd.ElementAt(i).CustMailStName1.ToString().Length > maxlength)
                            averydata.City = Sd.ElementAt(i).CustMailStName1.Substring(0, maxlength);
                        else
                            averydata.City = Sd.ElementAt(i).CustMailStName1;
                    }
                    if (averydata.City != "" && Sd.ElementAt(i).CustMailStName2 != "")
                    {
                        if (Sd.ElementAt(i).CustMailStName2.ToString().Length > maxlength)
                            averydata.City = averydata.City + Environment.NewLine + Sd.ElementAt(i).CustMailStName2.Substring(0, maxlength);
                        else
                            averydata.City = averydata.City + Environment.NewLine + Sd.ElementAt(i).CustMailStName2;
                    }
                    else if (Sd.ElementAt(i).CustMailStName2 != "")
                    {
                        if (Sd.ElementAt(i).CustMailStName2.ToString().Length > maxlength)
                            averydata.City = Sd.ElementAt(i).CustMailStName2.Substring(0, maxlength);
                        else
                            averydata.City = Sd.ElementAt(i).CustMailStName2;
                    }

                  

                    if (Sd.ElementAt(i).AddressCust.ToString().Length > maxlength)
                        averydata.Address = Sd.ElementAt(i).AddressCust.Substring(0, maxlength);
                    else
                        averydata.Address = Sd.ElementAt(i).AddressCust;

                    //Check Customer Name not more than 12 char
                    if (Sd.ElementAt(i).CustMail1.ToString().Trim() == "" && Sd.ElementAt(i).CustMail2.ToString().Trim() == "")
                        averydata.CustName = "PG&E Customer at";
                    else if (Sd.ElementAt(i).CustMail1.ToString().Trim() != "" || Sd.ElementAt(i).CustMail1 != "null")
                    {
                        if (Sd.ElementAt(i).CustMail1.ToString().Length > maxlength)
                        {
                            averydata.CustName = Sd.ElementAt(i).CustMail1.ToString().Substring(0, maxlength);
                        }
                        else
                        {
                            averydata.CustName = Sd.ElementAt(i).CustMail1.ToString();
                        }
                        if (Sd.ElementAt(i).CustMail2 != "")
                        {
                            if (Sd.ElementAt(i).CustMail2.ToString().Length > maxlength && Sd.ElementAt(i).CustMail1.ToString().Length > maxlength)
                            {
                                averydata.CustName = Sd.ElementAt(i).CustMail1.ToString().Substring(0, maxlength) + Environment.NewLine + Sd.ElementAt(i).CustMail2.ToString().Substring(0, maxlength);
                            }
                            else
                            {
                                averydata.CustName = Sd.ElementAt(i).CustMail1.ToString() + Environment.NewLine + Sd.ElementAt(i).CustMail2.ToString();
                            }
                        }
                        else if (Sd.ElementAt(i).CustMail2.ToString().Trim() != "")
                        {
                            if (Sd.ElementAt(i).CustMail2.ToString().Length > maxlength)
                            {
                                averydata.CustName = Sd.ElementAt(i).CustMail2.ToString().Substring(0, maxlength);
                            }
                            else
                            {
                                averydata.CustName = Sd.ElementAt(i).CustMail2.ToString();
                            }
                        }

                        AveryCustomerResult.Add(averydata);
                    }
                }
            }
            catch (Exception ex)
            {
                
                //throw;
            }

            
        }
       
        private void CreateAddReport_Async(List<TransformercustomerdataComb> CombCustomerList)
        {
            List<TransformercustomerdataComb> objTransResult = new List<TransformercustomerdataComb>();
            objTransResult = CombCustomerList;
            var Sd = objTransResult;
            for (int i = 0; i < Sd.Count; i++)
            {

                Addcustomerdata addCustomer = new Addcustomerdata();
                
                addCustomer.SlNo = i + 1;
                addCustomer.AddrLine1 = Sd.ElementAt(i).CustMail1;
                addCustomer.AddrLine2 = Sd.ElementAt(i).CustMail2;
                
                addCustomer.CGCTNum = Sd.ElementAt(i).CGC12;
                addCustomer.SSD = Sd.ElementAt(i).TransSSD;
                addCustomer.Phone = Sd.ElementAt(i).CustPhone;
                addCustomer.AccountID = Sd.ElementAt(i).SerActID;
                addCustomer.SPID = Sd.ElementAt(i).ServicepointId;
               
                addCustomer.CustType = Sd.ElementAt(i).CustType;
                addCustomer.meter_number = Sd.ElementAt(i).MeterNum;
                addCustomer.division = Sd.ElementAt(i).SerDivision;
                addCustomer.ORD = Sd.ElementAt(i).ORD;
              
                if(Sd.ElementAt(i).CustMailStNum != "")
                    addCustomer.AddrLine3 = Sd.ElementAt(i).CustMailStNum + " " + Sd.ElementAt(i).CustMailStName1;
                else
                    addCustomer.AddrLine3 = Sd.ElementAt(i).CustMailStName1;
                
                addCustomer.AddrLine4 = Sd.ElementAt(i).CustMailStName2 ;
                addCustomer.AddrLine5 = Sd.ElementAt(i).AddressCust;

                if (Sd.ElementAt(i).SerStNumber != "")
                    addCustomer.SPAddrLine1 = Sd.ElementAt(i).SerStNumber + " " + Sd.ElementAt(i).SerStName1;
                else
                    addCustomer.SPAddrLine1 = Sd.ElementAt(i).SerStName1;

                //addCustomer.SPAddrLine1 = Sd.ElementAt(i).CustMailStName1;
                addCustomer.SPAddrLine2 = Sd.ElementAt(i).SerStName2;
                addCustomer.SPAddrLine3 = Sd.ElementAt(i).SerCity + ", " + Sd.ElementAt(i).SerState + ", " + Sd.ElementAt(i).ServiceZip ;

                if (CGCTNumMapping.ContainsKey(Sd.ElementAt(i).CGC12.ToString()) == true)
                    addCustomer.CGCTNum = CGCTNumMapping[Sd.ElementAt(i).CGC12.ToString()];
                else
                    addCustomer.CGCTNum = Sd.ElementAt(i).CGC12;

                if (Sd.ElementAt(i).ORD == 1)
                {
                    var address1 = from cust in objTransResult
                                   where cust.ORD == 2 && cust.MeterNum == Sd.ElementAt(i).MeterNum && cust.ServicepointId == Sd.ElementAt(i).ServicepointId
                                   select cust;
                    if (address1.Count() > 0)
                    {
                        if (address1.First().CustMailStNum != "")
                            addCustomer.SPAddrLine1 = address1.First().CustMailStNum + " " + address1.First().CustMailStName1;
                        else
                            addCustomer.SPAddrLine1 = address1.First().CustMailStName1;
                        addCustomer.SPAddrLine2 = address1.First().CustMailStName2;
                        addCustomer.SPAddrLine3 = address1.First().AddressCust;
                    }
                }                 
                AddCustomerResult.Add(addCustomer); 
            }

        }
        
        private void CreateAveryReport(DownloadStringCompletedEventArgs e)
        {
            string result = "";
            List<TransformercustomerdataComb> objTransResult = new List<TransformercustomerdataComb>();
            result = e.Result;
            objTransResult = JsonHelper.Deserialize<List<TransformercustomerdataComb>>(result);
            var Sd = objTransResult;
            for (int i = 0; i < Sd.Count; i++)
            {
                AveryCustomerResult.Add(new Averycustomerdata()
                {
                    SlNo = i + 1,
                    CustName = Sd.ElementAt(i).CustMail1,
                    City = Sd.ElementAt(i).SerStName1,
                    Address = Sd.ElementAt(i).AddressCust,
                });
            }
            //var objAveryCus = AveryCustomerResult.ToList();
            //foreach (var item in objAveryCus)
            //{
            //    Averyobjcoll.Add(item);
            //}
        }
        private void CreateAddReport(DownloadStringCompletedEventArgs e)
        {
            string result = "";
            List<TransformercustomerdataComb> objTransResult = new List<TransformercustomerdataComb>();
            result = e.Result;
            objTransResult = JsonHelper.Deserialize<List<TransformercustomerdataComb>>(result);
            var Sd = objTransResult;
            for (int i = 0; i < Sd.Count; i++)
            {
                AddCustomerResult.Add(new Addcustomerdata()
                {
                    SlNo = i + 1,
                    AddrLine1 = Sd.ElementAt(i).CustMail1,
                    AddrLine2 = Sd.ElementAt(i).CustMail2,
                    AddrLine3 = Sd.ElementAt(i).CustMailStNum,
                    AddrLine4 = Sd.ElementAt(i).CustMailStName1,
                    AddrLine5 = Sd.ElementAt(i).CustMailCity + " " + Sd.ElementAt(i).MailState,
                    CGCTNum = Sd.ElementAt(i).CGC12,
                    SSD = Sd.ElementAt(i).TransSSD,
                    Phone = Sd.ElementAt(i).CustPhone,
                    AccountID = Sd.ElementAt(i).SerActID,
                    SPID = Sd.ElementAt(i).ServicepointId,
                    SPAddrLine1 = Sd.ElementAt(i).SerStNumber,
                    SPAddrLine2 = Sd.ElementAt(i).SerStName1,
                    SPAddrLine3 = Sd.ElementAt(i).SerCity + " " + Sd.ElementAt(i).SerState + " " + Sd.ElementAt(i).ServiceZip,
                    CustType = Sd.ElementAt(i).CustType,
                    meter_number = Sd.ElementAt(i).MeterNum,
                    division = Sd.ElementAt(i).SerDivision
                    
                });
            }

        }

        #endregion for Web Service Code

        public ArrayList GetCGCNumber()
        {
            ArrayList strCGCArr = new ArrayList();
            try
            {
                //Adding the code for reading the CGC number from the result collection
                string strListCGC = "";
                string strDeviceDescription = "";
                string strDeviceIDName = "";
                //for (int i = 0; i < SearchlistDataGrid.Items.Count; i++)
                //{
                //    SearchlistDataGrid.SelectAll();
                //}
                for (int i = 0; i < SearchlistDataGrid.Items.Count; i++)
                {
                    //DeviceSearchlist objAttribute = SearchlistDataGrid.SelectedItems[i] as DeviceSearchlist;
                    DeviceSearchlist objAttribute = SearchlistDataGrid.Items[i] as DeviceSearchlist;
                    strDeviceDescription = objAttribute.DeviceName.ToString();
                    strDeviceIDName = objAttribute.DeviceID.ToString();
                    if (objAttribute != null && (strDeviceDescription.Contains("Transformer") == true || strDeviceDescription.Contains("Primary Meter") == true) && strDeviceIDName.Contains('^'))
                    {
                        strListCGC = objAttribute.DeviceID.ToString();
                        strCGCArr.Add(strListCGC);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.FatalException("Error in PONS GetCGCNumber", ex);
            }
            return strCGCArr;
        }
        //Adding code for getcustomer for all Circuit Number
        
        //bool isFFF_Running = true;
        public ArrayList GetCircuiteIds()
        {
            ArrayList strCircuitArr = new ArrayList();
            try
            {
                //Adding the code for reading the CGC number from the result collection
                string strListCircuit = "";
                string strDeviceDescription = "";
                //string sFeederID_Query = string.Empty;
                //for (int i = 0; i < SearchlistDataGrid.Items.Count; i++)
                //{
                //    SearchlistDataGrid.SelectAll();
                //}
                for (int i = 0; i < SearchlistDataGrid.Items.Count; i++)
                {
                    //DeviceSearchlist objAttribute = SearchlistDataGrid.SelectedItems[i] as DeviceSearchlist;
                    DeviceSearchlist objAttribute = SearchlistDataGrid.Items[i] as DeviceSearchlist;
                    strDeviceDescription = objAttribute.DeviceName.ToString();
                    if (objAttribute != null && strDeviceDescription.Contains("substation") == true)
                    {
                        strListCircuit = objAttribute.DeviceID.ToString();
                        strCircuitArr.Add(strListCircuit);
                        //sFeederID_Query += ",'" + strListCircuit + "'";
                        if (strCircuitDic.ContainsKey(strListCircuit))
                            if (strCircuitDic[strListCircuit].Count > 0)
                                strCircuitArr.AddRange(strCircuitDic[strListCircuit]);
                    }
                }



                //sFeederID_Query = "(" + sFeederID_Query.Substring(1) + ")";

                //QueryTask FeederIDQuery = new QueryTask(_TracingNetworkService);
                //FeederIDQuery.ExecuteCompleted += FeederIDQueryTask_ExecuteCompleted;
                //FeederIDQuery.Failed += FeederIDQueryTask_Failed;
               
                //ESRI.ArcGIS.Client.Tasks.Query query = new ESRI.ArcGIS.Client.Tasks.Query();
                //if (strCircuitArr.Count>0)
                //{
                //    query.OutFields.AddRange(new string[] { "TO_CIRCUITID" });
                //    query.Where = "FEEDERID IN " + sFeederID_Query + " AND TO_CIRCUITID IS NOT NULL";
                //}
                //FeederIDQuery.ExecuteAsync(query);
            }
            catch (Exception ex)
            {
                logger.FatalException("Error in PONS GetCircuiteIds", ex);
            }

            return strCircuitArr;
        }

        Dictionary<string, ArrayList> strCircuitDic = new Dictionary<string, ArrayList>();
        private void FeederIDQueryTask_ExecuteCompleted(object sender, ESRI.ArcGIS.Client.Tasks.QueryEventArgs args)
        {
            ArrayList arrFFFID = new ArrayList();
            if (args.FeatureSet.Features.Count > 0)
            {
                foreach (Graphic graphic in args.FeatureSet.Features)
                {
                    string sFFFID = Convert.ToString(graphic.Attributes["TO_CIRCUITID"]);
                    if (sFFFID.Length > 0 && !arrFFFID.Contains(sFFFID))
                        arrFFFID.Add(sFFFID);
                }
            }
            strCircuitDic[sFeederID_Cur] = arrFFFID;
            //isFFF_Running = false;
        }

        private void FeederIDQueryTask_Failed(object sender, TaskFailedEventArgs args)
        {
            //isFFF_Running = false;
        }

        void Ponsservice_AddOutageToStagingCompleted(object sender, AsyncCompletedEventArgs e)
        {
            busyEmailIndicator.IsBusy = false;
        }
        private void btnSend_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                busyEmailIndicator.IsBusy = true;
                objPonsServiceCall = new Ponsservicelayercall();
                string strSequenceURL = objPonsServiceCall.GetSequenceAddress();
                SendSequeceEmailDataSend(strSequenceURL);
            }
            catch (Exception ex)
            {
                busyEmailIndicator.IsBusy = false;
                logger.FatalException("Error in PONS btnSend_Click", ex);
            }
        }
        //Email to Print
        private void btnPrevious6_Click(object sender, RoutedEventArgs e)
        {
            btnNext.IsEnabled = true;
            tabItem1.Visibility = Visibility.Collapsed;
            tabItem2.Visibility = Visibility.Collapsed;
            tabItem4.Visibility = Visibility.Collapsed;
            tabItem6.Visibility = Visibility.Visible;
            tabItem5.Visibility = Visibility.Collapsed;
            tabItem7.Visibility = Visibility.Collapsed;
            tabItem8.Visibility = Visibility.Collapsed;
            tabItem9.Visibility = Visibility.Collapsed;
            _fw.Width = 660;
            _fw.Height = 455;
            SetTabFocus(PrintDeviceButton);
        }
        private void btnNextsearchlist_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (CustomerList.Count > 0)
                {
                    int count = 0;
                    foreach (Pdfcustomerdata Emp in CustomerList)
                    {
                        Pdfcustomerdata emp = (from ep in CustomerResult
                                               where ep.SlNo == Emp.SlNo
                                               select ep).First();
                        CustomerResult.Remove(emp);
                        count++;
                    }
                    var R = CustomerResult.ToList();
                }

                if (AveryCustomerList.Count > 0)
                {
                    int countAvery = 0;
                    foreach (Pdfcustomerdata Avery in CustomerList)
                    {
                        Averycustomerdata avery = (from av in AveryCustomerResult
                                                   where av.SlNo == Avery.SlNo
                                                   select av).First();
                        AveryCustomerResult.Remove(avery);
                        countAvery++;
                    }
                    var AveryLab = AveryCustomerResult.ToList();
                    AveryCustomerList.Clear();
                }
                if (AddCustomerList.Count > 0)
                {
                    int countAddCust = 0;
                    foreach (Pdfcustomerdata Custdata in CustomerList)
                    {
                        Addcustomerdata custdata = (from ep in AddCustomerResult
                                                    where ep.SlNo == Custdata.SlNo
                                                    select ep).First();
                        AddCustomerResult.Remove(custdata);
                        countAddCust++;
                    }
                    var APSLCustomer = AddCustomerResult.ToList();
                    AddCustomerList.Clear();
                }
                CustomerList.Clear();
            }
            catch (Exception ex)
            {
                logger.FatalException("Error in PONS btnNextsearchlist_Click", ex);
            }
            tabItem1.Visibility = Visibility.Collapsed;
            tabItem2.Visibility = Visibility.Collapsed;
            tabItem4.Visibility = Visibility.Collapsed;
            tabItem5.Visibility = Visibility.Visible;
            tabItem6.Visibility = Visibility.Collapsed;
            tabItem7.Visibility = Visibility.Collapsed;
            tabItem8.Visibility = Visibility.Collapsed;
            tabItem9.Visibility = Visibility.Collapsed;

            SetTabFocus(btnoutage);

            //tabItem5.IsEnabled = true;
        }
        //outage to Print
        private void btnoutage_Click(object sender, RoutedEventArgs e)
        {
            if (AddDate.IsChecked == false)
            {
                if (datePickerFstShutDateOff.SelectedDate == null || datePickerFstShutDateOff.SelectedDate.ToString() == "")
                {
                    MessageBox.Show("Enter value for shutdown date.");
                    return;
                }
                if (timePickerFrstOff.Value == null || timePickerFrstOff.Value.ToString() == "")
                {
                    MessageBox.Show("Enter value for shutdown time.");
                    return;
                }
                if (timePickerFrstOn.Value == null || timePickerFrstOn.Value.ToString() == "")
                {
                    MessageBox.Show("Enter value for restoration time.");
                    return;
                }
                //if (timePickerFrstOff.Value.Value.TimeOfDay >= timePickerFrstOn.Value.Value.TimeOfDay)
                //{
                //    MessageBox.Show("Shutoff time must be before restoration.");
                //    return;
                //}
            }
            if (AddDate.IsChecked == true)
            {
                if (timePickerScndOff.Value == null || timePickerScndOff.Value.ToString() == "")
                {
                    MessageBox.Show("Enter value for  additional shutdown time.");
                    return;
                }
                if (datePickerScndOff.SelectedDate == null || datePickerScndOff.SelectedDate.ToString() == "")
                {
                    MessageBox.Show("Enter value for  additional restoration date.");
                    return;
                }
                if (timePickerScndOn.Value == null || timePickerScndOn.Value.ToString() == "")
                {
                    MessageBox.Show("Enter value for additional restoration time.");
                    return;
                }
                //if (timePickerScndOff.Value.Value.TimeOfDay >= timePickerScndOn.Value.Value.TimeOfDay)
                //{
                //    MessageBox.Show("Additional shutoff time must be before restoration.");
                //    return;
                //}
                //if (datePickerFstShutDateOff.SelectedDate.Value == datePickerScndOff.SelectedDate.Value)
                //{
                //    if (timePickerScndOff.Value.Value.TimeOfDay < timePickerFrstOn.Value.Value.TimeOfDay)
                //    {
                //        MessageBox.Show("Additional outage must occur after and not overlap with the first outage");
                //        return;
                //    }
                //}

                //if (datePickerFstShutDateOff.SelectedDate.Value > datePickerScndOff.SelectedDate.Value)
                //{
                //    MessageBox.Show("Additional outage must occur after and not overlap with the first outage");
                //    return;
                //}
            }

            tabItem1.Visibility = Visibility.Collapsed;
            tabItem2.Visibility = Visibility.Collapsed;
            tabItem4.Visibility = Visibility.Collapsed;
            tabItem5.Visibility = Visibility.Collapsed;
            tabItem6.Visibility = Visibility.Visible;
            tabItem7.Visibility = Visibility.Collapsed;
            tabItem8.Visibility = Visibility.Collapsed;
            tabItem9.Visibility = Visibility.Collapsed;
            _fw.Width = 660;
            _fw.Height = 455;
            if (isPolygonDrawn)
            {
                ch4Map.IsEnabled = true;
                ch4Map.IsChecked = true;
            }
            else
            {
                ch4Map.IsEnabled = false;
                ch4Map.IsChecked = false;
            }
            SetTabFocus(PrintDeviceButton); 
        }

        private void addDate_changed(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("changed");
        }

        private void btnPre4_Click(object sender, RoutedEventArgs e)
        {
            if (CustomerList.Count>0)
            {
                var confirmResult = MessageBox.Show("You have marked customer to be excluded, if you go to the previous screen, these customers will no longer be excluded. Do you want to continue?", "PONS", System.Windows.MessageBoxButton.OKCancel);
                if (confirmResult == System.Windows.MessageBoxResult.OK)
                {
                    tabItem1.Visibility = Visibility.Collapsed;
                    tabItem2.Visibility = Visibility.Visible;
                    tabItem4.Visibility = Visibility.Collapsed;
                    tabItem5.Visibility = Visibility.Collapsed;
                    tabItem6.Visibility = Visibility.Collapsed;
                    tabItem7.Visibility = Visibility.Collapsed;
                    tabItem8.Visibility = Visibility.Collapsed;
                    tabItem9.Visibility = Visibility.Collapsed;
                    SetTabFocus(GetCustomer);
                }
                else
                {
                    return;
                }
            }
            else
            {
                tabItem1.Visibility = Visibility.Collapsed;
                tabItem2.Visibility = Visibility.Visible;
                tabItem4.Visibility = Visibility.Collapsed;
                tabItem5.Visibility = Visibility.Collapsed;
                tabItem6.Visibility = Visibility.Collapsed;
                tabItem7.Visibility = Visibility.Collapsed;
                tabItem8.Visibility = Visibility.Collapsed;
                tabItem9.Visibility = Visibility.Collapsed;
                SetTabFocus(GetCustomer);
            }
            if (RdSelectMap.IsChecked == true)
            {
                RdSelectMap.IsChecked = false;
                RdSelectMap.IsChecked = true;
            }
        }
        private void btnPre3_Click(object sender, RoutedEventArgs e)
        {
                tabItem1.Visibility = Visibility.Visible;
                tabItem2.Visibility = Visibility.Collapsed;
                tabItem4.Visibility = Visibility.Collapsed;
                tabItem5.Visibility = Visibility.Collapsed;
                tabItem6.Visibility = Visibility.Collapsed;
                tabItem7.Visibility = Visibility.Collapsed;
                tabItem8.Visibility = Visibility.Collapsed;
                tabItem9.Visibility = Visibility.Collapsed;
                _fw.Width = 660;
                _fw.Height = 455;
                SetTabFocus(OKButton);
                if (MapEventIsOn == true)
                {
                    SelectPolygonEvent(false);
                }
        }
        //clear all data-22/03/2016
        private void Clearallvalue()
        {
            try
            {
                AveryCustomerResult.Clear();
                CustomerResult.Clear();
                AddCustomerResult.Clear();
                SearchlistDataGrid.ItemsSource = null;
                txtTransformer.Text = string.Empty;
                CGCTNumMapping.Clear();
                _deviceSearchItemList.Clear();
                //Rdpdf.IsChecked = false;
                Rdpdf.IsChecked = true;
                
                if (Rdpdf.IsChecked == true)
                {
                    Rdpdf.IsChecked = false;
                    Rdxls.IsChecked = true;
                    Rdpdf.IsChecked = true;
                }
                else
                {                   
                    Rdpdf.IsChecked = true;                 
                }
                dddivisionSearch.IsEnabled = true;
                GetCustomer.IsEnabled = false;                
                DateTime tomorrow = DateTime.Now.AddDays(1);
                string strtomorrowdate = Convert.ToString(tomorrow.Month) + "/" + Convert.ToString(tomorrow.Day) + "/" + Convert.ToString(tomorrow.Year);
                datePickerFstShutDateOff.Text = strtomorrowdate;

                datePickerFstShutDateOff.BlackoutDates.Add(new CalendarDateRange(new DateTime(0001, 1, 1), DateTime.Now));
                datePickerScndOff.BlackoutDates.Add(new CalendarDateRange(new DateTime(0001, 1, 1), DateTime.Now));

                DateTime selecteddate = datePickerFstShutDateOff.SelectedDate.Value;
                strtomorrowdate = Convert.ToString(selecteddate.Month) + "/" + Convert.ToString(selecteddate.Day) + "/" + Convert.ToString(selecteddate.Year);
              
                datePickerScndOff.Text = strtomorrowdate;

                timePickerFrstOff.Value = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 12, 00, 00);
                timePickerFrstOn.Value = new DateTime(DateTime.Now.Year,DateTime.Now.Month,DateTime.Now.Day,12,30,00);

                timePickerScndOff.Value = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 12, 00, 00);
                timePickerScndOn.Value = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 12, 30, 00);
              
                ddbank.Items.Clear();// = 0;
                ddcircuit.Items.Clear();// = 0;
                txt_StartDevice.Text = string.Empty;
                txt_EndDevice.Text = string.Empty;
                
                CustomerDataGrid.DataContext = null;
                txtoutagearea.Text = string.Empty;
             
                
                AddDate.IsChecked = false;
               
               
                if (Searchdevice.Count > 0)
                {
                    for (int j = Searchdevice.Count - 1; j >= 0; j--)
                    {

                        Searchdevice.RemoveAt(j);

                    }
                    SearchlistDataGrid.ItemsSource = Searchdevice;
                    SearchlistDataGrid.ItemsSource = null;
                }

                //Map selection
                DisableDrawPolygon();
            }
            catch
            {
            }
        }
        //Print to outage
        private void PrintCancelButton_Click(object sender, RoutedEventArgs e)
        {
            tabItem1.Visibility = Visibility.Collapsed;
            tabItem2.Visibility = Visibility.Collapsed;
            tabItem4.Visibility = Visibility.Collapsed;
            tabItem5.Visibility = Visibility.Visible;
            tabItem6.Visibility = Visibility.Collapsed;
            tabItem7.Visibility = Visibility.Collapsed;
            tabItem8.Visibility = Visibility.Collapsed;
            tabItem9.Visibility = Visibility.Collapsed;
            _fw.Width = 660;
            _fw.Height = 455;
          
            SetTabFocus(btnoutage);
        }
        //outage to tab 4(customer list)
        private void btnreset5_Click(object sender, RoutedEventArgs e)
        {   
                 tabItem1.Visibility = Visibility.Collapsed;
                 tabItem2.Visibility = Visibility.Collapsed;
                 tabItem4.Visibility = Visibility.Visible;
                 tabItem5.Visibility = Visibility.Collapsed;
                 tabItem6.Visibility = Visibility.Collapsed;
                 tabItem7.Visibility = Visibility.Collapsed;
                 tabItem8.Visibility = Visibility.Collapsed;
                 tabItem9.Visibility = Visibility.Collapsed;

                 SetTabFocus(btnNextsearchlist);
             
        }
        #region config

        //Read data from Page.config
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
                                    if (childelement.Name.LocalName == "TransformerService")
                                    {
                                        attribute = childelement.Attribute("Name").Value;
                                        if (attribute != null)
                                        {
                                            _TransformerService = attribute;
                                        }
                                    }
                                    if (childelement.Name.LocalName == "PrimaryMeterService")
                                    {
                                        attribute = childelement.Attribute("Name").Value;
                                        if (attribute != null)
                                        {
                                            _PrimaryMeterService = attribute;
                                        }
                                    }
                                    if (childelement.Name.LocalName == "PrimaryMeterSSD")
                                    {
                                        attribute = childelement.Attribute("Name").Value;
                                        if (attribute != null)
                                        {
                                            _PrimaryMeterSSD = attribute;
                                        }
                                    }

                                    if (childelement.Name.LocalName == "DivisionService")
                                    {
                                        attribute = childelement.Attribute("url").Value;
                                        if (attribute != null)
                                        {
                                            _DivisionService = attribute;
                                        }
                                    }
                                    if (childelement.Name.LocalName == "GeometryService")
                                    {
                                        attribute = childelement.Attribute("url").Value;
                                        if (attribute != null)
                                        {
                                            _GeometryService = attribute;
                                        }
                                    }
                                    if (childelement.Name.LocalName == "CircuitID")
                                    {
                                        attribute = childelement.Attribute("url").Value;
                                        if (attribute != null)
                                        {
                                            _CircuitIDService = attribute;
                                        }
                                    }
                                    if (childelement.Name.LocalName == "SubstationService")
                                    {
                                        attribute = childelement.Attribute("url").Value;
                                        if (attribute != null)
                                        {
                                            _SubstationService = attribute;
                                        }
                                    }
                                    if (childelement.Name.LocalName == "CircuitService")
                                    {
                                        attribute = childelement.Attribute("url").Value;
                                        if (attribute != null)
                                        {
                                            _CircuitService = attribute;
                                        }
                                    }
                                    if (childelement.Name.LocalName == "TracingNetworkService")
                                    {
                                        attribute = childelement.Attribute("url").Value;
                                        if (attribute != null)
                                        {
                                            _TracingNetworkService = attribute;
                                        }
                                    }
                                    if (childelement.Name.LocalName == "ServicepointIDService")
                                    {
                                        attribute = childelement.Attribute("url").Value;
                                        if (attribute != null)
                                        {
                                            _ServicepointIDService = attribute;
                                        }
                                    }

                                    if (childelement.Name.LocalName == "ServiceLocationService")
                                    {
                                        attribute = childelement.Attribute("url").Value;
                                        if (attribute != null)
                                        {
                                            _ServiceLocationService = attribute;
                                        }
                                    }
                                    //Read FCID from Page.config
                                    if (childelement.Name.LocalName == "Switch_FEATURE_FCID")
                                    {
                                        attribute = childelement.Attribute("ID").Value;
                                        if (attribute != null)
                                        {
                                            _Switch_FEATURE_FCID = Convert.ToInt32(attribute);
                                        }
                                    }
                                    if (childelement.Name.LocalName == "Dynamicprotectivedevice_FEATURE_FCID")
                                    {
                                        attribute = childelement.Attribute("ID").Value;
                                        if (attribute != null)
                                        {
                                            _Dynamicprotectivedevice_FEATURE_FCID = Convert.ToInt32(attribute);
                                        }
                                    }
                                    if (childelement.Name.LocalName == "FUSE_FEATURE_FCID")
                                    {
                                        attribute = childelement.Attribute("ID").Value;
                                        if (attribute != null)
                                        {
                                            _FUSE_FEATURE_FCID = Convert.ToInt32(attribute);
                                        }
                                    }
                                    if (childelement.Name.LocalName == "BufferDistance")
                                    {
                                        attribute = childelement.Attribute("ID").Value;
                                        if (attribute != null)
                                        {
                                            BufferDistance = Convert.ToInt32(attribute);
                                        }
                                    }

                                    if (childelement.Name.LocalName == "Transformer_FEATURE_FCID")
                                    {
                                        attribute = childelement.Attribute("ID").Value;
                                        if (attribute != null)
                                        {
                                            _Transformer_FEATURE_FCID = Convert.ToInt32(attribute);
                                        }
                                    }
                                    if (childelement.Name.LocalName == "PRIMARYMETER_FEATURE_FCID")
                                    {
                                        attribute = childelement.Attribute("ID").Value;
                                        if (attribute != null)
                                        {
                                            _PrimaryMeter_FEATURE_FCID = Convert.ToInt32(attribute);
                                        }
                                    }
                                    if (childelement.Name.LocalName == "Transformer_FEATURE_FCID")
                                    {
                                        attribute = childelement.Attribute("ID").Value;
                                        if (attribute != null)
                                        {
                                            _Transformer_FEATURE_FCID = Convert.ToInt32(attribute);
                                        }
                                    }
                                    if (childelement.Name.LocalName == "OpenPoint_FEATURE_FCID")
                                    {
                                        attribute = childelement.Attribute("ID").Value;
                                        if (attribute != null)
                                        {
                                            _OpenPoint_FEATURE_FCID = Convert.ToInt32(attribute);
                                        }
                                    }
                                    if (childelement.Name.LocalName == "PriUGConductor_FEATURE_FCID")
                                    {
                                        attribute = childelement.Attribute("ID").Value;
                                        if (attribute != null)
                                        {
                                            _PriUGConductor_FEATURE_FCID = Convert.ToInt32(attribute);
                                        }
                                    }
                                    if (childelement.Name.LocalName == "PrimaryOverheadConductor_FEATURE_FCID")
                                    {
                                        attribute = childelement.Attribute("ID").Value;
                                        if (attribute != null)
                                        {
                                            _PrimaryOverheadConductor_FEATURE_FCID = Convert.ToInt32(attribute);
                                        }
                                    }

                                    if (childelement.Name.LocalName == "ServiceLocation_FEATURE_FCID")
                                    {
                                        attribute = childelement.Attribute("ID").Value;
                                        if (attribute != null)
                                        {
                                            _ServiceLocation_FEATURE_FCID = Convert.ToString(attribute);
                                        }
                                    }

                                    if (childelement.Name.LocalName == "PONSPolyScale")
                                    {
                                        attribute = childelement.Attribute("Value").Value;
                                        if (attribute != null)
                                        {
                                            _PONSPolyScale = Convert.ToDouble(attribute);
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
                    LogHelper.InitializeNLog(element);
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
                logger.FatalException("Error in PONS ReadDivisionCodes", e);
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

                        case "Searches":
                            //   AddSearches(element);
                            break;
                        case "SearchableLayers":
                            //  AddSearchableLayers(element);
                            break;
                        case "Tracing":
                            //  AddTracing(element);
                            break;
                        case "GeographicSearchFilter":
                            // _gsfConfig = element;
                            break;
                        case "DivisionCodes":
                            ReadDivisionCodes(element);
                            break;



                    }
                }
                catch (Exception ex)
                {
                    logger.FatalException("Error in PONS radioSelectdevice_Click", ex);

                }
            }
        }

        # endregion
        private void PrintDeviceButton_Click(object sender, RoutedEventArgs e)
        {

            _fw.Width = 660;
            _fw.Height = 455;
            //Adding Checks for selection of checkes [Start]
            if (Ch1send.IsChecked == false && ch2Report.IsChecked == false && ch3backletter.IsChecked == false && ch5Report.IsChecked == false && ch4Map.IsChecked == false)
            {
                MessageBox.Show("Please select at least one option.");
                return;
            }
            if (Ch1send.IsChecked == true)
            {
                tabItem1.Visibility = Visibility.Collapsed;
                tabItem2.Visibility = Visibility.Collapsed;
                tabItem4.Visibility = Visibility.Collapsed;
                tabItem5.Visibility = Visibility.Collapsed;
                tabItem6.Visibility = Visibility.Collapsed;
                tabItem7.Visibility = Visibility.Visible;
                tabItem8.Visibility = Visibility.Collapsed;
                tabItem9.Visibility = Visibility.Collapsed;
            }
            else if (ch2Report.IsChecked == true)
            {
                tabItem1.Visibility = Visibility.Collapsed;
                tabItem2.Visibility = Visibility.Collapsed;
                tabItem4.Visibility = Visibility.Collapsed;
                tabItem5.Visibility = Visibility.Collapsed;
                tabItem6.Visibility = Visibility.Collapsed;
                tabItem7.Visibility = Visibility.Collapsed;
                tabItem8.Visibility = Visibility.Visible;
                tabItem9.Visibility = Visibility.Collapsed;

            }
            else if (ch5Report.IsChecked == true)
            {
                tabItem1.Visibility = Visibility.Collapsed;
                tabItem2.Visibility = Visibility.Collapsed;
                tabItem4.Visibility = Visibility.Collapsed;
                tabItem5.Visibility = Visibility.Collapsed;
                tabItem6.Visibility = Visibility.Collapsed;
                tabItem7.Visibility = Visibility.Collapsed;
                tabItem8.Visibility = Visibility.Visible;
                tabItem9.Visibility = Visibility.Collapsed;

            }
            else if (ch4Map.IsChecked == true)
            {
                tabItem1.Visibility = Visibility.Collapsed;
                tabItem2.Visibility = Visibility.Collapsed;
                tabItem4.Visibility = Visibility.Collapsed;
                tabItem5.Visibility = Visibility.Collapsed;
                tabItem6.Visibility = Visibility.Collapsed;
                tabItem7.Visibility = Visibility.Collapsed;
                tabItem8.Visibility = Visibility.Collapsed;
                tabItem10.Visibility = Visibility.Visible;
                tabItem9.Visibility = Visibility.Collapsed;

            }
            else if (ch3backletter.IsChecked == true)
            {
                tabItem1.Visibility = Visibility.Collapsed;
                tabItem2.Visibility = Visibility.Collapsed;
                tabItem4.Visibility = Visibility.Collapsed;
                tabItem5.Visibility = Visibility.Collapsed;
                tabItem6.Visibility = Visibility.Collapsed;
                tabItem7.Visibility = Visibility.Collapsed;
                tabItem8.Visibility = Visibility.Collapsed;
                tabItem9.Visibility = Visibility.Visible;
            }

            SetTabFocus(btnNext);

        }
        //Adding code to print the Customer report in xls,pdf and txt format// 
        private void btnPrintCustList_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (Rdxls.IsChecked == false && Rdpdf.IsChecked == false && Rdtxt.IsChecked == false && RdReport.IsChecked == false)
                {
                    MessageBox.Show("At least one file format should be selected to preview and print.");
                    return;
                }
                if (Rdxls.IsChecked == true)
                {
                    CustReportExportToExcel();
                }
                else if (Rdpdf.IsChecked == true)
                {
                    CustomerReportExportToPDF();
                }
                else if (Rdtxt.IsChecked == true)
                {
                    CustomerReportExportToTXT();
                }
                else if (RdReport.IsChecked == true)
                {
                    CustomerReportExportToReport();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                logger.FatalException("Error in PONS ", ex);
                return;
            }

        }
        private void CustomerReportExportToReport()
        {
            try
            {
                ExportCustomerData("RPT");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error in export data.");
                logger.FatalException("Error in PONS radioSelectdevice_Click", ex);
            }
        }
        private void CustomerReportExportToTXT()
        {
            try
            {
                ExportCustomerData("txt");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error in export data.");
                logger.FatalException("Error in PONS radioSelectdevice_Click", ex);
            }
        }
        private void CustReportExportToExcel()
        {
            try
            {
                ExportCustomerData("xml");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error in export data.");
                logger.FatalException("Error in PONS radioSelectdevice_Click", ex);
            }
        }
        private void CustomerReportExportToPDF()
        {
            try
            {
                ExportCustomerData("pdf");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error in export data.");
                logger.FatalException("Error in PONS radioSelectdevice_Click", ex);
            }
        }

        /// <summary>
        /// Adding Function for Export PDF
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        public void ExportCustomerData(string strFileFormat)
        {

            string strFrstTimeOFF = "";
            string strFrstTimeON = "";
            string strDateTimeTXT = "";
            string strLocationDecsTXT = "";
            string strDateOff = "";
            string strDateOn = "";
            string[] strTimeOff = null;
            string[] strTimeOn = null;

            string strSecondTimeOFF = "";
            string strSecondTimeON = "";
            string strSecondDateOff = "";

            string[] arrstrSecondTimeOff = null;
            string[] arrstrSecondTimeOn = null;
            string strDateTime = "";
            try
            {
                System.Globalization.DateTimeFormatInfo mfi = new System.Globalization.DateTimeFormatInfo();
                if (timePickerFrstOff.GetSelectedValue() != null)
                {
                    strFrstTimeOFF = timePickerFrstOff.GetSelectedValue().ToString();
                }
                if (timePickerFrstOn.GetSelectedValue() != null)
                {
                    strFrstTimeON = timePickerFrstOn.GetSelectedValue().ToString();
                }
                if (strFrstTimeOFF != "")
                {
                    strTimeOff = strFrstTimeOFF.Split(' ');  
                    string[] times = strTimeOff[1].ToString().Split(':');
                    strFrstTimeOFF = times[0] + ":" + times[1] + " " + strTimeOff[2].ToString();
                }
                if (strFrstTimeON != "")
                {
                    strTimeOn = strFrstTimeON.Split(' ');
                    string[] times = strTimeOn[1].ToString().Split(':');
                    strFrstTimeON = times[0] + ":" + times[1] + " " + strTimeOn[2].ToString();
                }

                if (datePickerFstShutDateOff.SelectedDate != null)
                {
                    strDateOff = datePickerFstShutDateOff.SelectedDate.Value.DayOfWeek + ", " + mfi.GetMonthName(datePickerFstShutDateOff.SelectedDate.Value.Month).ToString() + " " + datePickerFstShutDateOff.SelectedDate.Value.Day + ", " + datePickerFstShutDateOff.SelectedDate.Value.Year;

                }
                // Wednesday 23, March 2016 Time Off: 12:30:00 AM To  & Time ON: 1:30:00 AM

                //Tuesday, March 22, 2016 from 12:00 AM to 12:30 AM and
                //Adding Second date
                if (timePickerScndOff.GetSelectedValue() != null)
                {
                    strSecondTimeOFF = timePickerScndOff.GetSelectedValue().ToString();
                }
                if (timePickerScndOn.GetSelectedValue() != null)
                {
                    strSecondTimeON = timePickerScndOn.GetSelectedValue().ToString();
                }
                if (strSecondTimeOFF != "")
                {
                    arrstrSecondTimeOff = strSecondTimeOFF.Split(' ');
                    string[] times = arrstrSecondTimeOff[1].ToString().Split(':');
                    strSecondTimeOFF = times[0] + ":" + times[1] + " " + arrstrSecondTimeOff[2].ToString();
                }
                if (strSecondTimeON != "")
                {
                    arrstrSecondTimeOn = strSecondTimeON.Split(' ');
                    string[] times = arrstrSecondTimeOn[1].ToString().Split(':');
                    strSecondTimeON = times[0] + ":" + times[1] + " " + arrstrSecondTimeOn[2].ToString();
                }

                if (datePickerScndOff.SelectedDate != null)
                {
                    strSecondDateOff = datePickerScndOff.SelectedDate.Value.DayOfWeek + ", " + mfi.GetMonthName(datePickerScndOff.SelectedDate.Value.Month).ToString() + " " + datePickerScndOff.SelectedDate.Value.Day + ", " + datePickerScndOff.SelectedDate.Value.Year;
                }
                if (AddDate.IsChecked == true)
                {
                    if (Rdpdf.IsChecked == true)
                    {
                        strDateTimeTXT = "  " + strDateOff + " from " + strFrstTimeOFF + " to " + strFrstTimeON + " and " + "\n   " + "                             " + strSecondDateOff + " from " + strSecondTimeOFF + " to " + strSecondTimeON;
                    }
                    else
                    {
                        strDateTimeTXT = "  " + strDateOff + " from " + strFrstTimeOFF + " to " + strFrstTimeON + " and " + "\n " + "           " + strSecondDateOff + " from " + strSecondTimeOFF + " to " + strSecondTimeON;
                    }
                }
                else
                {
                    strDateTimeTXT = "  " + strDateOff + " from " + strFrstTimeOFF + " to " + strDateOn + " " + strFrstTimeON;
                }
                strLocationDecsTXT = "  " + txtoutagearea.Text.ToString();
                List<TransformercustomerdataComb> uiDataCollection = new List<TransformercustomerdataComb>();

                foreach (Pdfcustomerdata data in CustomerResult)
                {
                    TransformercustomerdataComb customerData = new TransformercustomerdataComb();
                    customerData.CustType = data.CustType;

                    if (data.CustMail1.ToString().Trim() == "" && data.CustMail2.ToString().Trim() == "")
                        customerData.CustMail1 = "PG&E Customer at";
                    else if (data.CustMail1.ToString().Trim() != "" || data.CustMail1 != "null")
                    {
                        customerData.CustMail1 = data.CustMail1;
                        if (data.CustMail2 != "" || data.CustMail2 != "null")
                        {
                            customerData.CustMail1 = data.CustMail1 + " " + data.CustMail2;
                        }
                    }
                    else if (data.CustMail2.ToString().Trim() != "" || data.CustMail2 != "null")
                    {
                        customerData.CustMail1 = data.CustMail2;
                    }
                    customerData.AddressCust = data.AddressCust;
                    customerData.CustMailStNum = data.CustMailStNum;
                    customerData.CustMailStName1 = data.CustMailStName1;
                    customerData.CustMailStName2 = data.CustMailStName2;
                    customerData.CustMailCity = data.CustMailCity;
                    customerData.SerCity = data.SerCity;
                    customerData.SerState = data.SerState;
                    customerData.AverySTNameNumber = data.AverySTNameNumber;
                    customerData.CustMailZipCode = data.CustMailZipCode;
                    customerData.CGC12 = data.CGC12;
                    customerData.TransSSD = data.TransSSD;
                    customerData.CustPhone = data.CustPhone;
                    customerData.PREMISEID = data.PREMISEID;
                    customerData.PREMISETYPE = data.PREMISETYPE;
                    uiDataCollection.Add(customerData);
                }
                string currentUser = WebContext.Current.User.Name;
                string[] UserName = currentUser.Split('\\');
                string userName = UserName[1].ToString();
                EndpointAddress endPoint = new EndpointAddress(endpointaddress);
                BasicHttpBinding Httpbindings = new BasicHttpBinding();
                Httpbindings.MaxBufferSize = 2147483647;
                Httpbindings.MaxReceivedMessageSize = 2147483647;
                Httpbindings.SendTimeout = TimeSpan.FromSeconds(300);
                IServicePons cusservice = new ChannelFactory<IServicePons>(Httpbindings, endPoint).CreateChannel();
                try
                {
                    if (uiDataCollection.Count <= 20000)
                    {
                        cusservice.BeginGetExportdataFile(strDateTimeTXT, strLocationDecsTXT, "Customer Notification List", uiDataCollection, strFileFormat, userName,
                         delegate(IAsyncResult result)
                         {
                             string ExporthostUrl;
                             string _hostUrl = ((IServicePons)result.AsyncState).EndGetExportdataFile(result);
                             this.Dispatcher.BeginInvoke(
                             delegate()
                             {
                                 ExporthostUrl = _hostUrl.ToString();
                                 Client_PdfCusdata(ExporthostUrl);
                             }
                             );
                         }, cusservice);
                    }
                    else 
                    {
                        busyEmailIndicator.IsBusy = false;
                        MessageBox.Show("System is not able to print pdf more than 20000.");
                        return;
                    }
                }
                catch (TimeoutException timeProblem)
                {
                    Console.WriteLine("The service operation timed out. " + timeProblem.Message);
                }
                catch (FaultException fault)
                {
                    Console.WriteLine("SampleFault fault occurred: {0}", fault.Message);

                }
                catch (CommunicationException commProblem)
                {
                    Console.WriteLine("There was a communication problem. " + commProblem.Message);
                }
            }
            catch (Exception ex)
            {
                logger.FatalException("Error in PONS ExportCustomerData", ex);
            }
        }
        public void GetURLOfFile(IAsyncResult result)
        {
            try
            {
                string strURL = ((IServicePons)result.AsyncState).EndGetExportdataFile(result);
                Client_PdfCusdata(strURL);
            }
            catch (TimeoutException timeProblem)
            {
                Console.WriteLine("The service operation timed out. " + timeProblem.Message);

            }
            catch (FaultException fault)
            {
                Console.WriteLine("SampleFault fault occurred: {0}", fault.Message);

            }
            catch (CommunicationException commProblem)
            {
                Console.WriteLine("There was a communication problem. " + commProblem.Message);
            }
        }
        void Client_PdfCusdata(string strURL)
        {
            try
            {
                if (strURL != null)
                {
                    string hostUrl = strURL;
                    HtmlPage.Window.Navigate(new Uri(hostUrl), "_blank", "scrollbars=yes,toolbar=no,location=no,status=no,menubar=no,resizable=yes,width=1000px,height=600px");
                }
                else
                {
                    strURL = "";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                logger.FatalException("Error in PONS ", ex);
                return;
            }
        }

        private void btnNextCustList_Click(object sender, RoutedEventArgs e)
        {
            _fw.Width = 660;
            _fw.Height = 455;

            //Adding Checks for selection of checkes [Start]
            if (ch4Map.IsChecked == true)
            {
                tabItem1.Visibility = Visibility.Collapsed;
                tabItem2.Visibility = Visibility.Collapsed;
                tabItem4.Visibility = Visibility.Collapsed;
                tabItem5.Visibility = Visibility.Collapsed;
                tabItem6.Visibility = Visibility.Collapsed;
                tabItem7.Visibility = Visibility.Collapsed;
                tabItem8.Visibility = Visibility.Collapsed;
                tabItem9.Visibility = Visibility.Collapsed;
                tabItem10.Visibility = Visibility.Visible;
                if (ch3backletter.IsChecked == true)
                {
                    btnNextMapCustList.IsEnabled = true;
                }
                else
                {
                    btnNextMapCustList.IsEnabled = false;
                }

            }
            else if (ch3backletter.IsChecked == true)
            {
                tabItem1.Visibility = Visibility.Collapsed;
                tabItem2.Visibility = Visibility.Collapsed;
                tabItem4.Visibility = Visibility.Collapsed;
                tabItem5.Visibility = Visibility.Collapsed;
                tabItem6.Visibility = Visibility.Collapsed;
                tabItem7.Visibility = Visibility.Collapsed;
                tabItem8.Visibility = Visibility.Collapsed;
                tabItem10.Visibility = Visibility.Collapsed;
                tabItem9.Visibility = Visibility.Visible;
                btnNextMapCustList.IsEnabled = true;
            }
            else
            {
                MessageBox.Show("No item has selected, please click on previous to select the option.");
                btnNextCustList.IsEnabled = false;
            }

            SetTabFocus(btnAveryPrint);
        }
        //Add-sept2017
        private void btnNextMapCustList_Click(object sender, RoutedEventArgs e)
        {
            _fw.Width = 660;
            _fw.Height = 455;

            //Adding Checks for selection of checkes [Start]
            if (ch3backletter.IsChecked == true)
            {
                tabItem1.Visibility = Visibility.Collapsed;
                tabItem2.Visibility = Visibility.Collapsed;
                tabItem4.Visibility = Visibility.Collapsed;
                tabItem5.Visibility = Visibility.Collapsed;
                tabItem6.Visibility = Visibility.Collapsed;
                tabItem7.Visibility = Visibility.Collapsed;
                tabItem8.Visibility = Visibility.Collapsed;
                tabItem10.Visibility = Visibility.Collapsed;
                tabItem9.Visibility = Visibility.Visible;
            }
            else
            {
                MessageBox.Show("No item has selected, please click on previous to select the option.");
                btnNextCustList.IsEnabled = false;
            }

            SetTabFocus(btnAveryPrint);
        }

        private void btnMapPre_Click(object sender, RoutedEventArgs e)
        {
            _fw.Width = 660;
            _fw.Height = 455;
            if (ch2Report.IsChecked == false && Ch1send.IsChecked == false)
            {
                tabItem1.Visibility = Visibility.Collapsed;
                tabItem2.Visibility = Visibility.Collapsed;
                tabItem4.Visibility = Visibility.Collapsed;
                tabItem5.Visibility = Visibility.Collapsed;
                tabItem6.Visibility = Visibility.Visible;
                tabItem7.Visibility = Visibility.Collapsed;
                tabItem8.Visibility = Visibility.Collapsed;
                tabItem9.Visibility = Visibility.Collapsed;
                tabItem10.Visibility = Visibility.Collapsed;
            }

            else if (ch2Report.IsChecked == true || ch5Report.IsChecked == true)
            {
                tabItem1.Visibility = Visibility.Collapsed;
                tabItem2.Visibility = Visibility.Collapsed;
                //tabItem3.Visibility = Visibility.Collapsed;
                tabItem4.Visibility = Visibility.Collapsed;
                tabItem5.Visibility = Visibility.Collapsed;
                tabItem6.Visibility = Visibility.Collapsed;
                tabItem7.Visibility = Visibility.Collapsed;
                tabItem8.Visibility = Visibility.Visible;
                tabItem9.Visibility = Visibility.Collapsed;
                tabItem10.Visibility = Visibility.Collapsed;
            }
            else if (Ch1send.IsChecked == true && ch2Report.IsChecked == false || ch5Report.IsChecked == false)
            {
                tabItem1.Visibility = Visibility.Collapsed;
                tabItem2.Visibility = Visibility.Collapsed;
                //tabItem3.Visibility = Visibility.Collapsed;
                tabItem4.Visibility = Visibility.Collapsed;
                tabItem5.Visibility = Visibility.Collapsed;
                tabItem6.Visibility = Visibility.Collapsed;
                tabItem7.Visibility = Visibility.Visible;
                tabItem8.Visibility = Visibility.Collapsed;
                tabItem9.Visibility = Visibility.Collapsed;
                tabItem10.Visibility = Visibility.Collapsed;
            }

            SetTabFocus(btnNextCustList);
        }
        private void btnAveryPre_Click(object sender, RoutedEventArgs e)
        {
            _fw.Width = 660;
            _fw.Height = 455;
            if (ch2Report.IsChecked == false && Ch1send.IsChecked == false && ch4Map.IsChecked == true)
            {
                tabItem1.Visibility = Visibility.Collapsed;
                tabItem2.Visibility = Visibility.Collapsed;
                tabItem4.Visibility = Visibility.Collapsed;
                tabItem5.Visibility = Visibility.Collapsed;
                tabItem6.Visibility = Visibility.Collapsed;
                tabItem7.Visibility = Visibility.Collapsed;
                tabItem8.Visibility = Visibility.Collapsed;
                tabItem9.Visibility = Visibility.Collapsed;
                tabItem10.Visibility = Visibility.Visible;
            }
            else if (ch2Report.IsChecked == false && Ch1send.IsChecked == false && ch4Map.IsChecked == false && ch5Report.IsChecked == false)
            {
                tabItem1.Visibility = Visibility.Collapsed;
                tabItem2.Visibility = Visibility.Collapsed;
                tabItem4.Visibility = Visibility.Collapsed;
                tabItem5.Visibility = Visibility.Collapsed;
                tabItem6.Visibility = Visibility.Visible;
                tabItem7.Visibility = Visibility.Collapsed;
                tabItem10.Visibility = Visibility.Collapsed;
                tabItem9.Visibility = Visibility.Collapsed;
                tabItem8.Visibility = Visibility.Collapsed;
            }
            else if (ch2Report.IsChecked == false && Ch1send.IsChecked == true && ch4Map.IsChecked == false && ch5Report.IsChecked == false)
            {
                tabItem1.Visibility = Visibility.Collapsed;
                tabItem2.Visibility = Visibility.Collapsed;
                tabItem4.Visibility = Visibility.Collapsed;
                tabItem5.Visibility = Visibility.Collapsed;
                tabItem6.Visibility = Visibility.Collapsed;
                tabItem7.Visibility = Visibility.Visible;
                tabItem10.Visibility = Visibility.Collapsed;
                tabItem9.Visibility = Visibility.Collapsed;
                tabItem8.Visibility = Visibility.Collapsed;
            }
            else if (ch2Report.IsChecked == true && ch4Map.IsChecked == true)
            {
                tabItem1.Visibility = Visibility.Collapsed;
                tabItem2.Visibility = Visibility.Collapsed;
                //tabItem3.Visibility = Visibility.Collapsed;
                tabItem4.Visibility = Visibility.Collapsed;
                tabItem5.Visibility = Visibility.Collapsed;
                tabItem6.Visibility = Visibility.Collapsed;
                tabItem7.Visibility = Visibility.Collapsed;
                tabItem8.Visibility = Visibility.Collapsed;
                tabItem9.Visibility = Visibility.Collapsed;
                tabItem10.Visibility = Visibility.Visible;
            }
            else if (ch4Map.IsChecked == false)
            {
                tabItem1.Visibility = Visibility.Collapsed;
                tabItem2.Visibility = Visibility.Collapsed;
                //tabItem3.Visibility = Visibility.Collapsed;
                tabItem4.Visibility = Visibility.Collapsed;
                tabItem5.Visibility = Visibility.Collapsed;
                tabItem6.Visibility = Visibility.Collapsed;
                tabItem7.Visibility = Visibility.Collapsed;
                tabItem9.Visibility = Visibility.Collapsed;
                tabItem8.Visibility = Visibility.Visible;
                tabItem10.Visibility = Visibility.Collapsed;
            }
            else if (Ch1send.IsChecked == true && ch4Map.IsChecked == true)
            {
                tabItem1.Visibility = Visibility.Collapsed;
                tabItem2.Visibility = Visibility.Collapsed;
                //tabItem3.Visibility = Visibility.Collapsed;
                tabItem4.Visibility = Visibility.Collapsed;
                tabItem5.Visibility = Visibility.Collapsed;
                tabItem6.Visibility = Visibility.Collapsed;
                tabItem7.Visibility = Visibility.Collapsed;
                tabItem8.Visibility = Visibility.Collapsed;
                tabItem9.Visibility = Visibility.Collapsed;
                tabItem10.Visibility = Visibility.Visible;
            }

            SetTabFocus(btnNextCustList);
        }


        //Print to Email
        private void btnPrintPre_Click(object sender, RoutedEventArgs e)
        {
            //btnNextCustList.IsEnabled = true;
            if (Ch1send.IsChecked == true)
            {
                tabItem1.Visibility = Visibility.Collapsed;
                tabItem2.Visibility = Visibility.Collapsed;
                tabItem4.Visibility = Visibility.Collapsed;
                tabItem5.Visibility = Visibility.Collapsed;
                tabItem6.Visibility = Visibility.Collapsed;
                tabItem7.Visibility = Visibility.Visible;
                tabItem8.Visibility = Visibility.Collapsed;
                tabItem9.Visibility = Visibility.Collapsed;
            }
            else
            {
                tabItem1.Visibility = Visibility.Collapsed;
                tabItem2.Visibility = Visibility.Collapsed;
                tabItem4.Visibility = Visibility.Collapsed;
                tabItem5.Visibility = Visibility.Collapsed;
                tabItem6.Visibility = Visibility.Visible;
                tabItem7.Visibility = Visibility.Collapsed;
                tabItem8.Visibility = Visibility.Collapsed;
                tabItem9.Visibility = Visibility.Collapsed;
            }
            _fw.Width = 660;
            _fw.Height = 455;
            SetTabFocus(btnNext);
        }
        private void btnAveryClose_Click(object sender, RoutedEventArgs e)
        {
            _fw.Visibility = Visibility.Collapsed;
            ClearSession = true;
            Clearallvalue();
        }
        private void btnMapClose_Click(object sender, RoutedEventArgs e)
        {
            _fw.Visibility = Visibility.Collapsed;
            ClearSession = true;
            Clearallvalue();
        }
        //Avery Print
        private void btnAveryPrint_Click(object sender, RoutedEventArgs e)
        {

            //Change for vaildation on 08-01-2016
            string strNumber = txt_number.Text;
            if (ConfigUtility.IsItNumber(strNumber) || strNumber == "")
            {
                ExportAveryCustomerData();
            }
            else
            {
                MessageBox.Show("Please enter interger(0-29) value only");
                return;
            }

        }

        public void ExportAveryCustomerData()
        {
            try
            {
                List<TransformercustomerdataComb> uiDataCollection = new List<TransformercustomerdataComb>();
                EndpointAddress endPoint = new EndpointAddress(endpointaddress);
                BasicHttpBinding binding = new BasicHttpBinding();
                binding.MaxBufferSize = 2147483647;
                binding.MaxReceivedMessageSize = 2147483647;
                binding.TransferMode = TransferMode.Buffered;
                IServicePons cusservice = new ChannelFactory<IServicePons>(binding, endPoint).CreateChannel();
                foreach (Averycustomerdata data in AveryCustomerResult)
                {
                    TransformercustomerdataComb customerData = new TransformercustomerdataComb();
                    customerData.CustMail1 = data.CustName;
                    customerData.CustMailStName1 = data.City;
                    customerData.AddressCust = data.Address;
                    uiDataCollection.Add(customerData);
                }
                int skips = 0;
                if (txt_number.Text != "")
                {
                    string strNumber = txt_number.Text;
                    int inCnt = Convert.ToInt32(strNumber);
                    if (inCnt >= 0 && inCnt < 30)
                    {
                        if (inCnt < 30)
                        {
                            skips = inCnt;
                        }
                    }
                    else
                    {
                        MessageBox.Show("Please enter the valid number (1-29) to skip label print.");
                        return;
                    }
                }
                string currentUser = WebContext.Current.User.Name;
                string[] UserName = currentUser.Split('\\');
                string strUserName = UserName[1].ToString();
                try
                {
                    cusservice.BeginGetAverydata(uiDataCollection, skips, strUserName,
                     delegate(IAsyncResult result)
                     {
                         string ExporthostUrl;
                         string _hostUrl = ((IServicePons)result.AsyncState).EndGetAverydata(result);
                         this.Dispatcher.BeginInvoke(
                         delegate()
                         {
                             ExporthostUrl = _hostUrl.ToString();
                             Client_PdfCusdata(ExporthostUrl);
                         }
                         );
                     }, cusservice);

                }
                catch (Exception ex)
                {
                    logger.FatalException("Error in PONS ExportAveryCustomerData", ex);
                }
            }
            catch (Exception ex)
            {
                logger.FatalException("Error in PONS ExportAveryCustomerData", ex);
            }
        }

        public string WCFServiceURL { get; set; }

 
        private void SearchlistDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            if (SearchlistDataGrid.SelectedItems.Count > 0)
            {
                btnDeleteGetCustomer.IsEnabled = true;
                SetTabFocus(btnDeleteGetCustomer);
            }
            else
            {
                btnDeleteGetCustomer.IsEnabled = false;
            }

        }
        private void Every_Checked(object sender, RoutedEventArgs e)
        {
            if (Ch1send == null && ch2Report == null && ch3backletter == null && ch5Report == null && ch4Map == null)
                return;

            if (Ch1send.IsChecked == false && ch2Report.IsChecked == false && ch3backletter.IsChecked == false && ch5Report.IsChecked == false && ch4Map.IsChecked == false)
            {
                PrintDeviceButton.IsEnabled = false;
                //btnNext.IsEnabled = true;
                //btnNextCustList.IsEnabled = true; 
            }
            else
            {
                PrintDeviceButton.IsEnabled = true;
            }
            if (Ch1send.IsChecked == true)
            {
                PrintDeviceButton.IsEnabled = true;
                btnSend.IsEnabled = true;
            }
            else
                btnSend.IsEnabled = false;

            if (ch3backletter.IsChecked == false)
            {
                btnNext.IsEnabled = false;

            }
            else
            {
                PrintDeviceButton.IsEnabled = true;
                btnNext.IsEnabled = true;

            }
            if (ch2Report.IsChecked == false)
            {
                btnNext.IsEnabled = false;
                Rdpdf.IsEnabled = false;
                Rdtxt.IsEnabled = false;
                Rdxls.IsEnabled = false;
                Rdpdf.IsChecked = false;
            }
            else
            {
                PrintDeviceButton.IsEnabled = true;
                btnNext.IsEnabled = true;
                Rdpdf.IsEnabled = true;
                Rdtxt.IsEnabled = true;
                Rdxls.IsEnabled = true;
                Rdpdf.IsChecked = true;
            }
            if (ch3backletter.IsChecked == true)
            {
                PrintDeviceButton.IsEnabled = true;
                btnNextMapCustList.IsEnabled = true;
            }
            else
                btnNextMapCustList.IsEnabled = false;

            //Add-Sept17
            if (ch5Report.IsChecked == true)
            {
                RdReport.Visibility = Visibility.Visible;
                btnNext.IsEnabled = true;
                RdReport.IsEnabled = true;
                RdReport.IsChecked = true;
            }
            else
            {
                RdReport.Visibility = Visibility.Collapsed;
                RdReport.IsEnabled = false;
                RdReport.IsChecked = false;
            }
            if (ch4Map.IsChecked == true)
            {
                btnNext.IsEnabled = true;
                btnMapPrint.IsEnabled = true;
                btnNextCustList.IsEnabled = true;
            }
            else if (ch3backletter.IsChecked == true && ch4Map.IsChecked == false)
            {
                PrintDeviceButton.IsEnabled = true;
                btnNext.IsEnabled = true;
                btnNextCustList.IsEnabled = true;
            }
            else if (ch3backletter.IsChecked == true && Ch1send.IsChecked == true)
            {
                PrintDeviceButton.IsEnabled = true;
                btnNext.IsEnabled = true;
                btnNextCustList.IsEnabled = true;
            }
            else
            {
                // btnAdhocPrint.IsEnabled = false;
                btnMapPrint.IsEnabled = false;
                btnNextCustList.IsEnabled = false;
            }
        }


        private void btnDeleteGetCustomer_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string str = "";
                string str1 = "";
                if (SearchlistDataGrid.SelectedIndex != -1)
                {
                    for (int i = SearchlistDataGrid.SelectedItems.Count - 1; i >= 0; i--)
                    {
                        DeviceSearchlist objAttribute = SearchlistDataGrid.SelectedItems[i] as DeviceSearchlist;
                        str = objAttribute.DeviceName.ToString();
                        str1 = objAttribute.DeviceID.ToString();
                        Searchdevice.Remove(objAttribute);
                        if (objAttribute.CGC12 != null)
                            CGCTNumMapping.Remove(objAttribute.CGC12);  
                        if (str.IndexOf("start device") != -1 || str.IndexOf("Customers fed by between") !=-1)
                        {
                            txt_StartDevice.Text = "";
                            txt_EndDevice.Text = "";
                            DeviceSearchItem removeitem = null;
                            foreach (DeviceSearchItem item in _deviceSearchItemList)
                            {
                                if (item.Key == objAttribute.GUID + objAttribute.EndGUID)
                                    removeitem = item;
                            }
                            if (removeitem != null)
                            {
                                _deviceSearchItemList.Remove(removeitem);
                            }
                        }
                        arrFFF_FeederID_Trace.Clear();//Add this code
                        
                    }
                    SearchlistDataGrid.ItemsSource = null;
                    for (int j = Searchdevice.Count; j > 0; j--)
                    {
                        if (str == Searchdevice[j - 1].DeviceName && str1 == Searchdevice[j - 1].DeviceID)
                        {
                            Searchdevice.RemoveAt(j);
                        }
                    }
                    SearchlistDataGrid.ItemsSource = Searchdevice;
                    var DistinctItems = Searchdevice.GroupBy(x => x.DeviceID).Select(y => y.First());
                    SearchlistDataGrid.ItemsSource = null;
                    SearchlistDataGrid.ItemsSource = DistinctItems.ToList();
                    if (DistinctItems.Count() > 0)
                        GetCustomer.IsEnabled = true;
                    else
                    {
                        GetCustomer.IsEnabled = false;
                        RdBetweenDevice.IsChecked = true;
                        SetTabFocus(txt_StartDevice);
                    }

                }

            }
            catch (Exception ex)
            {
                logger.FatalException("Error in PONS ", ex);
            }
        }
        private void btnCloseResult_Click(object sender, RoutedEventArgs e)
        {
            _fw.Visibility = Visibility.Collapsed;
            ClearSession = true;
            DisableDrawPolygon();
        }
        private void btnCloseCustResult_Click(object sender, RoutedEventArgs e)
        {
            _fw.Visibility = Visibility.Collapsed;
            ClearSession = true;
            DisableDrawPolygon();
        }
        private void btnClosePrintResult_Click(object sender, RoutedEventArgs e)
        {
            _fw.Visibility = Visibility.Collapsed;
            ClearSession = true;
            DisableDrawPolygon();
        }
        private void btnClosePrintCustList_Click(object sender, RoutedEventArgs e)
        {
            _fw.Visibility = Visibility.Collapsed;
            ClearSession = true;
            Clearallvalue();
        }
        private void btnCloseoutage_Click(object sender, RoutedEventArgs e)
        {
            _fw.Visibility = Visibility.Collapsed;
            ClearSession = true;
            DisableDrawPolygon();
        }
        private void btnCloseEmail_click(object sender, RoutedEventArgs e)
        {
            _fw.Visibility = Visibility.Collapsed;
            ClearSession = true;
            DisableDrawPolygon();
        }

        
        //
        #region Transformer
        #endregion
        #region Substation-Circuit

        void SubstationCircuitsearch()
        {
            try
            {
                SearchType = "3";
                CircuitFedderID = ddcircuit.SelectedItem.ToString();
                getCircuitID(CircuitFedderID);
            }
            catch (Exception ex)
            {
                MessageBox.Show("No record found." + ex.Message);
                logger.FatalException("Error in PONS ", ex);
            }
        }

        void getCircuitID(string TransCircuitID)
        {
            try
            {
                if (TransCircuitID != null)
                {
                    SearchlistDataGrid.ItemsSource = null;
                    SearchlistDataGrid.ItemsSource = LoadCollectionData();
                    var DistinctItems = Searchdevice.GroupBy(x => x.DeviceID).Select(y => y.First());
                    SearchlistDataGrid.ItemsSource = null;
                    SearchlistDataGrid.ItemsSource = DistinctItems.ToList();
                }
                else
                {
                    MessageBox.Show("No record found for selected circuitid.");
                    return;
                }

            }
            catch (Exception ex)
            {
                logger.FatalException("Error in PONS ", ex);
            }
        }
        void BindCircuitsearch()
        {
            try
            {
                SearchType = "3";
                CircuitFedderID = ddcircuit.SelectedItem.ToString();
                getCircuitIDByFEEDERFEDBY(CircuitFedderID);
            }
            catch (Exception ex)
            {
                MessageBox.Show("No record found." + ex.Message);
                logger.FatalException("Error in PONS ", ex);
            }
        }
        void getCircuitIDByFEEDERFEDBY(string TransCircuitID)
        {
            try
            {
                QueryTask FeederByTask = new QueryTask(_TracingNetworkService);
                FeederByTask.ExecuteCompleted += FeederByTask_ExecuteCompleted;
                FeederByTask.Failed += SubstationqueryTask_Failed;
                ESRI.ArcGIS.Client.Tasks.Query query = new ESRI.ArcGIS.Client.Tasks.Query();
                string par = TransCircuitID;
                //dd.Devicename = "Circuit";
                query.Where = "FEEDERFEDBY  ='" + par + "'" + " AND ROWNUM < 2";
                query.OutFields.Add("FEEDERID");
                //query.OutFields.Add("*");
                FeederByTask.ExecuteAsync(query);
            }
            catch (Exception ex)
            {
                logger.FatalException("Error in PONS ", ex);
            }
        }

        void BindSubstationsearchdata()
        {
            try
            {
                QueryTask SubstationqueryTaskCheck = new QueryTask(_CircuitService);
                SubstationqueryTaskCheck.ExecuteCompleted += SubstationqueryTask_ExecuteCompletedBind;
                SubstationqueryTaskCheck.Failed += SubstationqueryTask_Failed;
                ESRI.ArcGIS.Client.Tasks.Query query = new ESRI.ArcGIS.Client.Tasks.Query();
                string par = ddsubstation.SelectedItem.ToString();
                //dd.Devicename = "Circuit";
                query.Where = "SUBSTATIONNAME  =" + "'" + par + "'";
                query.OutFields.Add("CIRCUITID");
                //query.OutFields.Add("*");
                SubstationqueryTaskCheck.ExecuteAsync(query);
            }
            catch (Exception ex)
            {
                logger.FatalException("Error in PONS BindSubstationsearchdata ", ex);
            }
        }
        private void SubstationqueryTask_ExecuteCompletedBind(object sender, ESRI.ArcGIS.Client.Tasks.QueryEventArgs args)
        {
            try
            {
                FeatureSet featureSet = args.FeatureSet;
                checknumberdevice = featureSet.Features.Count;
                if (featureSet != null && featureSet.Features.Count > 0)
                {
                    // SearchlistCircuitDataGrid.ItemsSource = featureSet.Features;
                    SearchType = "3";
                    _fw.Height = 455;
                    CustWiz.Height = 455;
                    if (args.FeatureSet.Features.Count > 0)
                    {
                        if (CircuitFedderID != "")
                        {
                            SearchlistDataGrid.ItemsSource = null;
                            SearchlistDataGrid.ItemsSource = LoadCollectionData();
                            //var DistinctItems = Searchdevice.GroupBy(x => x.DeviceID).Select(y => y.First());
                            //SearchlistDataGrid.ItemsSource = null;
                            //SearchlistDataGrid.ItemsSource = DistinctItems.ToList();
                        }

                        if (checknumberdevice > 1)
                        {
                            
                            childWindow.GetSubstationid = ddsubstation.SelectedValue.ToString();
                            // childWindow.GetcircuitId = ddcircuit.SelectedItem.ToString();
                            childWindow.Rdtype = 3;
                            childWindow.SearchlistCircuitDataGrid.Visibility = Visibility.Collapsed;
                            childWindow.SearchlistDataGrid.Visibility = Visibility.Collapsed;
                            childWindow.AdddataButton.Visibility = Visibility.Collapsed;
                            childWindow.OKButton.Visibility = Visibility.Collapsed;

                            childWindow.Show();
                        }
                    }
                    GetCustomer.Visibility = Visibility.Visible;
                }
                else
                {
                    MessageBox.Show("No Circuit returned from query");
                    return;
                }
            }
            catch (Exception ex)
            {
                logger.FatalException("Error in PONS SubstationqueryTask_ExecuteCompletedBind", ex);
            }
        }

        private void SubstationqueryTask_Failed(object sender, TaskFailedEventArgs args)
        {
            MessageBox.Show("Query failed: " + args.Error);
        }
        public void substationBankCircuitTask_Task(object sender, TaskFailedEventArgs args)
        {
            MessageBox.Show("Query failed: " + args.Error);
        }
        public void FeederByTask_ExecuteCompleted(object sender, ESRI.ArcGIS.Client.Tasks.QueryEventArgs args)
        {
            FeatureSet featureSet = args.FeatureSet;
            try
            {
                if (featureSet.Features.Count > 0)
                {
                    foreach (Graphic graphic in featureSet.Features)
                    {
                        if (graphic.Attributes["FEEDERID"] == null)
                        {
                            CircuitFedderID = "Null : FEEDERID";
                        }
                        else
                        {
                            CircuitFedderID = Convert.ToString( graphic.Attributes["FEEDERID"]);
                        }

                    }
                    //}
                    SearchlistDataGrid.ItemsSource = null;
                    SearchlistDataGrid.ItemsSource = LoadCollectionData();
                    var DistinctItems = Searchdevice.GroupBy(x => x.DeviceID).Select(y => y.First());
                    SearchlistDataGrid.ItemsSource = null;
                    SearchlistDataGrid.ItemsSource = DistinctItems.ToList();
                }
                else
                {
                    MessageBox.Show("No record found for selected circuitid.");
                    return;
                }

            }
            catch (Exception ex)
            {
                logger.FatalException("Error in PONS ", ex);
            }

        }
        #endregion
        //
        #region Map Selection and select service point
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
                if (RdSelectMap.IsChecked == true)
                {
                    this._map.MouseMove += _map_MouseMovePONS;
                    this._map.MouseLeftButtonDown += _map_MouseLeftButtonDownPONS;
                    this.MapEventIsOn = true;
                }
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
            catch(Exception ex)
            {
                logger.Error("PONS -_map_MouseMovePONS: " + ex.Message);
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
                    _layers.Last().Layer.ID = PONS_POLYGON_GRAPHIC_LAYER;
                    Layer graphicLayer = _map.Layers[PONS_POLYGON_GRAPHIC_LAYER];
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
                    _layers.Last().Layer.ID = PONS_POLYGON_GRAPHIC_LAYER;
                    Layer graphicLayer = _map.Layers[PONS_POLYGON_GRAPHIC_LAYER];
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
                    _graphicsLayer.Graphics.Clear();
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
            catch(Exception ex)
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
                logger.Error("PONS-MapMouseDoubleClickPONS: " + ex.Message);
            }
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
                _graphicsLayer = new GraphicsLayer();
                _graphicsLayer.ID = PONS_GRAPHICS_LAYER;
                if (!_map.Layers.Contains(_graphicsLayer))
                {
                    _map.Layers.Add(_graphicsLayer);
                }
                //_drawControl.DrawComplete += DrawControl_DrawComplete;
            }
            _drawControl.IsEnabled = true;
            _polygon = null;
        }

        public void ClearGraphics()
        {
            if (_graphicsLayer != null)
                _graphicsLayer.ClearGraphics();
            if (_drawControl != null)
                _drawControl.IsEnabled = false;
            ClearLayers();
            _polygon = null;
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
                RefreshPolygonGraphics();
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
                logger.Error("PONS-GeometryServiceAreaFinalSegmentDistanceProjectCompletedPONS: " + ex.Message);
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
                RefreshPolygonGraphics();
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
                logger.Error("PONS-GeometryServiceMoveDistanceProjectCompletedPONS: " + ex.Message);
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

        public void ClearLayers()
        {
            foreach (var layer in _layers)
            {
                layer.Layer.ClearGraphics();
                _map.Layers.Remove(layer.Layer);
            }

            _layers.Clear();
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

        private void DrawControl_DrawComplete()
        {
            try
            {
                LoadMapLayers(_map.Layers);   //get map at this time for ad hoc print
                
                var extent = new Envelope(_polygon.Extent.XMin, _polygon.Extent.YMin, _polygon.Extent.XMax, _polygon.Extent.YMax);
                extent.XMax += 10;
                extent.YMax += 10;
                extent.XMin -= 10;
                extent.YMin -= 10;
                _ponsAdHocMap.Extent = extent;

                isPolygonDrawn = true;
                DeviceButton.IsEnabled = false;
                busyIndicator.IsBusy = true;
                busyIndicator.BusyContent = "Processing...";
                _polygonSearchList.Clear();
                var identifyParameters = new IdentifyParameters();
                identifyParameters.Geometry = _polygon;
                identifyParameters.LayerIds.Add(Convert.ToInt16(_TransformerService.Substring(_TransformerService.LastIndexOf("/") + 1)));
                identifyParameters.LayerIds.Add(Convert.ToInt16(_PrimaryMeterService.Substring(_PrimaryMeterService.LastIndexOf("/") + 1)));
                identifyParameters.MapExtent = _map.Extent;
                identifyParameters.LayerOption = LayerOption.all;
                identifyParameters.Height = (int)_map.ActualHeight;
                identifyParameters.Width = (int)_map.ActualWidth;
                identifyParameters.SpatialReference = _map.SpatialReference;
                identifyParameters.Tolerance = 5;

                var identifyTask = new IdentifyTask(_TransformerService.Substring(0, _TransformerService.LastIndexOf("/")));
                identifyTask.Failed += new EventHandler<TaskFailedEventArgs>(identifyTaskPolygon_Failed);
                identifyTask.ExecuteCompleted += new EventHandler<IdentifyEventArgs>(identifyTaskPolygon_ExecuteCompleted);
                identifyTask.ExecuteAsync(identifyParameters);
            }
            catch (Exception ex)
            {
                logger.Error("PONS-DrawControl_DrawComplete: " + ex.Message);
            }
        }

        private void identifyTaskPolygon_ExecuteCompleted(object sender, IdentifyEventArgs e)
        {
            try
            {
                if (e.IdentifyResults != null)
                {
                    for (int i = 0; i < e.IdentifyResults.Count; i++)
                    {
                        _polygonSearchList.Add(new PolygonSearchItem()
                        {
                            LayerName = e.IdentifyResults[i].LayerName.ToString(),
                            CGC = Convert.ToString(e.IdentifyResults[i].Feature.Attributes["CGC12"]),
                            SSD = Convert.ToString(e.IdentifyResults[i].Feature.Attributes["Source Side Device ID"]),
                            FeederId = Convert.ToString(e.IdentifyResults[i].Feature.Attributes["Circuit ID"]),
                            Division = ((e.IdentifyResults[i].Feature.Attributes["DIVISION"] != null) ? (Convert.ToString(e.IdentifyResults[i].Feature.Attributes["DIVISION"])) : (Convert.ToString(e.IdentifyResults[i].Feature.Attributes["Division"]))),
                            OperatingNum = Convert.ToString(e.IdentifyResults[i].Feature.Attributes["Operating Number"])
                        });
                    }
                }
                DeviceButton.IsEnabled = true;
                busyIndicator.IsBusy = false;
            }
            catch (Exception err)
            {
                logger.Error("Failed to find Transformers and Primary Meter within polygon " + err.Message);
                ConfigUtility.StatusBar.Text = "Customer Selection by polygon has failed. " + err.Message;
                DeviceButton.IsEnabled = true;
                busyIndicator.IsBusy = false;
            }

        }

        private void identifyTaskPolygon_Failed(object sender, TaskFailedEventArgs e)
        {
            logger.Error("Failed to find Transformers and Primary Meter within polygon " + e.Error);
            ConfigUtility.StatusBar.Text = "Customer Selection by polygon has failed. " + e.Error.Message;
            DeviceButton.IsEnabled = true;
            busyIndicator.IsBusy = false;
        }

        private ESRI.ArcGIS.Client.Geometry.Polygon GetPolygonFromEnvelope(ESRI.ArcGIS.Client.Geometry.Geometry geometry)
        {
            Graphic g = new Graphic();
            Polygon p = new ESRI.ArcGIS.Client.Geometry.Polygon();
            p.SpatialReference = geometry.SpatialReference;

            PointCollection pColl = new ESRI.ArcGIS.Client.Geometry.PointCollection();
            MapPoint pLeftBottom = new MapPoint();
            pLeftBottom.X = geometry.Extent.XMin;
            pLeftBottom.Y = geometry.Extent.YMin;
            pColl.Add(pLeftBottom);
            MapPoint pLeftTop = new MapPoint();
            pLeftTop.X = geometry.Extent.XMin;
            pLeftTop.Y = geometry.Extent.YMax;
            pColl.Add(pLeftTop);
            MapPoint pRightTop = new MapPoint();
            pRightTop.X = geometry.Extent.XMax;
            pRightTop.Y = geometry.Extent.YMax;
            pColl.Add(pRightTop);
            MapPoint pRightBottom = new MapPoint();
            pRightBottom.X = geometry.Extent.XMax;
            pRightBottom.Y = geometry.Extent.YMin;
            pColl.Add(pRightBottom);
            MapPoint pLeftBottomEnd = new MapPoint();
            pLeftBottomEnd.X = geometry.Extent.XMin;
            pLeftBottomEnd.Y = geometry.Extent.YMin;
            pColl.Add(pLeftBottomEnd);

            p.Rings.Add(pColl);

            return p;
        }

        private void PolygonSearch()
        {
            try
            {
                if (_polygonSearchList.Count > 0)
                {
                    SearchlistDataGrid.ItemsSource = null;
                    SearchlistDataGrid.ItemsSource = LoadCollectionData();
                    var DistinctItems = Searchdevice.GroupBy(x => x.DeviceID).Select(y => y.First());
                    SearchlistDataGrid.ItemsSource = null;
                    SearchlistDataGrid.ItemsSource = DistinctItems.ToList();
                }
                else
                {
                    MessageBox.Show("No record found for selected polygon.");
                    return;
                }
            }
            catch (Exception ex)
            {
                logger.FatalException("Error in PONS ", ex);
            }
        }

        public void Map_ExtentChanged_Polygon(object sender, ExtentEventArgs e)   //to handle scale change for map selection option
        {
            try
            {
                double currentScale = Math.Round(_map.Resolution * 96);
                if (currentScale <= _PONSPolyScale)
                {
                    RdSelectMap.IsEnabled = true;
                }
                else
                {
                    if (RdSelectMap.IsChecked == true)
                    {
                        RdBetweenDevice.IsChecked = true;
                    }
                    RdSelectMap.IsEnabled = false;
                }
                if (RdBetweenDevice.IsEnabled == false && RdTranformer.IsEnabled == false && RdCircuit.IsEnabled == false && RdSelectMap.IsEnabled == false)
                {
                    RdBetweenDevice.IsChecked = false;
                    DeviceEntry.Visibility = Visibility.Collapsed;
                    stacktrans.Visibility = Visibility.Collapsed;
                    BtnBeyond.Visibility = Visibility.Collapsed;
                    DeviceEntryTransformer.Visibility = Visibility.Collapsed;
                    stackcircuit.Visibility = Visibility.Collapsed;
                    stackcircuit1.Visibility = Visibility.Collapsed;
                    txt_StartDevice.Text = string.Empty;
                    txt_EndDevice.Text = string.Empty;
                    txtTransformer.Text = string.Empty;
                    if (ddsubstation.SelectedIndex > -1)
                        ddsubstation.SelectedIndex = 0;
                    ddbank.Items.Clear();
                    ddbank.Items.Add("Select...");
                    if (ddbank.SelectedIndex > -1)
                        ddbank.SelectedIndex = 0;
                    ddcircuit.Items.Clear();
                    ddcircuit.Items.Add("Select...");
                    if (ddcircuit.SelectedIndex > -1)
                        ddcircuit.SelectedIndex = 0;
                    SearchCancelButton.IsEnabled = false;
                    DeviceButton.IsEnabled = false;
                }
                else
                {
                    SearchCancelButton.IsEnabled = true;
                    DeviceButton.IsEnabled = true;
                }
                if (RdSelectMap.IsChecked == true && tabItem2.Visibility == System.Windows.Visibility.Visible)
                {
                    RdSelectMap.IsChecked = false;
                    RdSelectMap.IsChecked = true;
                }
            }
            catch (Exception ex)
            {
                logger.Error("PONS-Map_ExtentChanged_Polygon: " + ex.Message);
            }
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
            DependencyProperty.Register("IsDrawPolygonActive", typeof(bool), typeof(PONSDialog), new PropertyMetadata(OnIsDrawPolygonActiveChanged));

        private static void OnIsDrawPolygonActiveChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (PONSDialog)d;
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

        #region Adhoc Print
        private void btnMapPrint_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                btnMapPrint.IsEnabled = false;
                busyIndicatorMapPrint.IsBusy = true;
                busyIndicatorMapPrint.BusyContent = "Map printing...";
                PONSAdHocPrint ponsAdHoc = new PONSAdHocPrint();
                ponsAdHoc.AdHocMap = _ponsAdHocMap;
                ponsAdHoc.EDMasterLayers = EDMasterLayers;
                ponsAdHoc.SelectedStoredDisplayName = SelectedStoredDisplayName;
                ConfigUtility.StatusBar.Text = "Map printing...";
                if (RdLan.IsChecked == true)
                {
                    ponsAdHoc.AdHocTemplateNameSelected = "AdHocMap_8.5x11_Landscape";
                }
                else
                {
                    ponsAdHoc.AdHocTemplateNameSelected = "AdHocMap_8.5x11_Portrait";
                }
                ponsAdHoc.ReadTemplatesConfig();
                _hiResPrint = ponsAdHoc.HiResPrintObject();
                if (_hiResPrint != null)
                {
                    _hiResPrint.PrintJobSubmitted += new EventHandler<PrintJobSubmittedEventArgs>(_hiResPrint_PrintJobSubmitted);
                    _hiResPrint.PrintJobCompleted += new EventHandler<EventArgs>(_hiResPrint_PrintJobCompleted);
                    ponsAdHoc.PONSPrintMapAddLayers(_hiResPrint);
                    ponsAdHoc.PONSPrintAdHocMap(_hiResPrint);
                }
            }
            catch(Exception err)
            {
                logger.Error("PONS - Failed to Map print" + err.Message);
                ConfigUtility.StatusBar.Text = "PONS - Map print has failed. " + err.Message;
                btnMapPrint.IsEnabled = true;
                busyIndicator.IsBusy = false;
            }
        }

        void _hiResPrint_PrintJobSubmitted(object sender, PrintJobSubmittedEventArgs e)
        {
            _hiResPrint.PrintJobSubmitted -= _hiResPrint_PrintJobSubmitted;
        }

        void _hiResPrint_PrintJobCompleted(object sender, EventArgs e)
        {
            btnMapPrint.IsEnabled = true;
            busyIndicatorMapPrint.IsBusy = false;
            _hiResPrint.PrintJobCompleted -= _hiResPrint_PrintJobCompleted;
            logger.Info("PRINTING: Print job completed.");
        }

        private void LoadMapLayers(LayerCollection mainMapCollection)
        {
            Layer _newLayer = null;
            _ponsAdHocMap.Layers = new LayerCollection();

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
                    _ponsAdHocMap.Layers.Add(_newLayer);
                    _ponsAdHocMap.Extent = _map.Extent;
                }
            }
            _ponsAdHocMap.UpdateLayout();
        }

        private void RefreshPolygonGraphics()
        {
            Layer graphicLayer = _ponsAdHocMap.Layers[PONS_POLYGON_GRAPHIC_LAYER];
            if(graphicLayer != null){
                _ponsAdHocMap.Layers.Remove(graphicLayer);
                if (_layers.Last().Layer.ID == PONS_POLYGON_GRAPHIC_LAYER)
                {
                    Layer _newLayer = null;
                    try
                    {
                        _newLayer = CloneLayer(_layers.Last().Layer);
                    }
                    catch (Exception e)
                    {
                        // Clone failed
                        _newLayer = null;
                    }
                    if (_newLayer != null)
                    {
                        _ponsAdHocMap.Layers.Add(_newLayer);
                    }
                }
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

                        foreach (System.Reflection.FieldInfo fi in dependencyObjectType.GetFields(BindingFlags.Static | BindingFlags.Public))
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
                            clone.Symbol = new ESRI.ArcGIS.Client.Symbols.SimpleLineSymbol
                            {
                                Color = new SolidColorBrush(Color.FromArgb(255, 255, 0, 0)),
                                Width = 4,
                                Style = ESRI.ArcGIS.Client.Symbols.SimpleLineSymbol.LineStyle.Solid
                            };
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

        #endregion
        public class ExportFileName
        {
            public string FileName { set; get; }
        }

        private void AddDate_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                if (AddDate.IsChecked == true)
                {
                    DateTime selecteddate = datePickerFstShutDateOff.SelectedDate.Value;
                    string strtomorrowdate = Convert.ToString(selecteddate.Month) + "/" + Convert.ToString(selecteddate.Day) + "/" + Convert.ToString(selecteddate.Year);                    
                    datePickerScndOff.Text = strtomorrowdate;
                }
                else
                {
                    datePickerScndOff.Text = "";
                }
            }
            catch (Exception)
            {
                
                //throw;
            }
           
        }

        private void CustWiz_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if(tabItem2.Visibility == System.Windows.Visibility.Visible)
                {
                    DeviceButton_Click(null, null);
                }

            }
        }
    }

    public static class GlobalValiables
    {
        public static string USERNAME;
        public static string PLANNEDSHUTDOWNID;
    }
    public class Addcustomerdata
    {
        public int SlNo { get; set; }
        public string SubmitUser { set; get; }
        public string SubmitDT { set; get; }
        public string SubmitTM { set; get; }
        public string Vicinity { set; get; }
        public string AddrLine1 { set; get; }// Customer Name 1
        public string AddrLine2 { set; get; }// Customer Name 2
        public string AddrLine3 { set; get; }// Street Number and St Name1
        public string AddrLine4 { set; get; }//St Name1
        public string AddrLine5 { set; get; }// City, State, Zip
        public string CGCTNum { set; get; }
        public string SSD { set; get; }
        public string Phone { set; get; }
        public string Work1DT { set; get; }
        public string Work1TMStart { set; get; }
        public string Work1TMEnd { set; get; }
        public string Work2DT { set; get; }
        public string Work2TMStart { set; get; }
        public string Work2TMEnd { set; get; }
        public string AccountID { set; get; }
        public string OutageID { set; get; }
        public string ContactID { set; get; }
        public string SPID { set; get; }
        public string SPAddrLine1 { set; get; }
        public string SPAddrLine2 { set; get; }
        public string SPAddrLine3 { set; get; }
        public string CustType { set; get; }
        public string meter_number { set; get; }
        public string division { set; get; }
        public string cn_fld1 { set; get; }
        public string cn_fld2 { set; get; }
        public string cn_fld3 { set; get; }
        public int ORD { set; get; }
        public string PREMISEID { set; get; }
      
        public string PREMISETYPE { set; get; }
    }

    public class Averycustomerdata
    {
        public int SlNo { get; set; }
        public string CustName { set; get; }
        public string Address { set; get; }
        public string City { set; get; }
        public string State { set; get; }
        public string ZipCode { set; get; }
    }

    public class DeviceSearchlist
    {
        private string objectID;
        public string OBJECTID
        {
            get { return objectID; }
            set { objectID = value; }
        }

        private string gid;
        public string GLOBALID
        {
            get { return gid; }
            set { gid = value; }
        }
        private string cid;
        public string CIRCUITID
        {
            get { return cid; }
            set { cid = value; }
        }
        public string DeviceName { get; set; }
        public string DeviceID { get; set; }
        public string CGC12 { get; set; }

        public string GUID { get; set; }
        public string EndGUID { get; set; }
    }

    public class clscuscount
    {
        private int _intcount;
        public int cuscountval
        {
            set
            {
                _intcount = value;
            }
            get
            {
                return _intcount;
            }
        }
    }

    public class Pdfcustomerdata
    {
        public int SlNo { get; set; }
        public string AverySTNameNumber { get; set; }
        public string AddressCust { get; set; }
        public string CustType { set; get; }
        public string MeterNum { set; get; }
        public string ServicepointId { set; get; }
        public string CustomerName { set; get; }
        public string CustMail1 { set; get; }
        public string CustMail2 { set; get; }
        public string SerStNumber { set; get; }
        public string SerStName1 { set; get; }
        public string SerStName2 { set; get; }
        public string CustMailStNum { set; get; }
        public string CustMailStName1 { set; get; }
        public string CustMailStName2 { set; get; }
        public string ServiceZip { set; get; }
        public string CustMailZipCode { set; get; }
        public string SerCity { set; get; }
        public string CustMailCity { set; get; }
        public string SerState { set; get; }
        public string MailState { set; get; }
        public string CGC12 { set; get; }
        public string TransOperatingNum { set; get; }
        public string PMeterOperatingNum { set; get; }
        public string TransSSD { set; get; }
        public string CustAreaCode { set; get; }
        public string CustPhone { set; get; }
        public string GridADDCol { set; get; }
        public bool Checked { set; get; }
        public int ORD { set; get; }
        public string PREMISEID { set; get; }
       
        public string PREMISETYPE { set; get; }
    }
    //Export to txt
    public static class ExportTXT
    {
        public static void ExportToTXT(this DataGrid dg, string strDateTime, string strDecLoc)
        {
            ExportDataGridToTXT(dg, strDateTime, strDecLoc);
        }

        public static void ExportDataGridToTXT(DataGrid dGrid, string strDateTime, string strDecLoc)
        {
            SaveFileDialog objSFD = new SaveFileDialog() { DefaultExt = "txt", Filter = "TEXT Files (*.txt)|*.txt", FilterIndex = 1 };

            //objSFD.DefaultExt = "txt";
            int padf1 = 8;
            int padf2 = 20;
            int padf3 = 20;
            int padf4 = 80;
            int padf5 = 20;
            int padf6 = 20;
            int padf7 = 20;

            int padsum = padf1 + padf2 + padf3 + padf4 + padf5 + padf6 + padf7;
            try
            {
                if (objSFD.ShowDialog() == true)
                {
                    StringBuilder strBuilder = new StringBuilder();
                    if (dGrid.ItemsSource == null)
                        return;
                    //strBuilder.AppendLine("Interruption Date:");
                    List<string> lstFields = new List<string>();
                    strBuilder.AppendLine("Interruption Date: " + strDateTime + " weather permitting");
                    strBuilder.AppendLine("Interruption Area:" + strDecLoc);
                    strBuilder.AppendLine();
                    strBuilder.AppendLine();
                    strBuilder.AppendLine(" Customer Notification List ".PadLeft(padsum / 2) + DateTime.Today.Date.ToShortDateString().PadRight(5));
                    strBuilder.AppendLine();
                    if (dGrid.HeadersVisibility == DataGridHeadersVisibility.Column || dGrid.HeadersVisibility == DataGridHeadersVisibility.All)
                    {
                        foreach (DataGridColumn dgcol in dGrid.Columns)
                            lstFields.Add(dgcol.Header.ToString());

                    }
                    string strCustNameAdd = "";
                    strCustNameAdd = lstFields[4].ToString();
                    if (strCustNameAdd.Length > 60)
                    {
                        strCustNameAdd = strCustNameAdd.Replace(" ", "/n");

                    }

                    strBuilder.AppendLine(lstFields[1].ToString().PadRight(padf1) + lstFields[2].ToString().PadRight(padf2) + lstFields[3].ToString().PadRight(padf3) + strCustNameAdd.PadRight(padf4) + lstFields[5].ToString().PadRight(padf5) + lstFields[6].ToString().PadRight(padf6) + lstFields[7].ToString().PadRight(padf7));

                    for (int i = 0; i < padsum; i++)
                    {
                        strBuilder.Append("_");
                    }
                    strBuilder.AppendLine();
                    if (dGrid.ItemsSource != null)
                    {
                        foreach (object data in dGrid.ItemsSource)
                        {
                            lstFields.Clear();
                            strBuilder.AppendLine();
                            foreach (DataGridColumn col in dGrid.Columns)
                            {

                                string strValue = "";
                                System.Windows.Data.Binding objBinding = null;
                                if (col is DataGridBoundColumn)
                                    objBinding = (col as DataGridBoundColumn).Binding;
                                if (col is DataGridTemplateColumn)
                                {
                                    DependencyObject objDO = (col as DataGridTemplateColumn).CellTemplate.LoadContent();
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
                                            strValue = pi.GetValue(data, null).ToString();
                                    }
                                    if (objBinding.Converter != null)
                                    {
                                        if (strValue != "" && strValue != null)
                                            strValue = objBinding.Converter.Convert(strValue, typeof(string), objBinding.ConverterParameter, objBinding.ConverterCulture).ToString();
                                        // else
                                        //strValue = objBinding.Converter.Convert(data, typeof(string), objBinding.ConverterParameter, objBinding.ConverterCulture).ToString();
                                    }
                                }
                                lstFields.Add(strValue);
                            }
                            strBuilder.Append(lstFields[1].ToString().PadRight(padf1) + lstFields[2].ToString().PadRight(padf2) + lstFields[3].ToString().PadRight(padf3) + lstFields[4].ToString().PadRight(padf4) + lstFields[5].ToString().PadRight(padf5) + lstFields[6].ToString().PadRight(padf6) + lstFields[7].ToString().PadRight(padf7));
                        }
                    }
                    StreamWriter sw = new StreamWriter(objSFD.OpenFile());
                    sw.Write(strBuilder.ToString());
                    sw.Close();

                }
            }
            catch
            {
                MessageBox.Show("No Record found!");

            }
        }
    }

    public class CustomComparer : IComparer<string>
    {
        public int Compare(string x, string y)
        {
            string s1 = x as string;
            if (s1 == null)
            {
                return 0;
            }
            string s2 = y as string;
            if (s2 == null)
            {
                return 0;
            }

            int len1 = s1.Length;
            int len2 = s2.Length;
            int marker1 = 0;
            int marker2 = 0;

            // Walk through two the strings with two markers.
            while (marker1 < len1 && marker2 < len2)
            {
                char ch1 = s1[marker1];
                char ch2 = s2[marker2];

                // Some buffers we can build up characters in for each chunk.
                char[] space1 = new char[len1];
                int loc1 = 0;
                char[] space2 = new char[len2];
                int loc2 = 0;

                // Walk through all following characters that are digits or
                // characters in BOTH strings starting at the appropriate marker.
                // Collect char arrays.
                do
                {
                    space1[loc1++] = ch1;
                    marker1++;

                    if (marker1 < len1)
                    {
                        ch1 = s1[marker1];
                    }
                    else
                    {
                        break;
                    }
                } while (char.IsDigit(ch1) == char.IsDigit(space1[0]));

                do
                {
                    space2[loc2++] = ch2;
                    marker2++;

                    if (marker2 < len2)
                    {
                        ch2 = s2[marker2];
                    }
                    else
                    {
                        break;
                    }
                } while (char.IsDigit(ch2) == char.IsDigit(space2[0]));

                // If we have collected numbers, compare them numerically.
                // Otherwise, if we have strings, compare them alphabetically.
                string str1 = new string(space1);
                string str2 = new string(space2);

                int result;

                if (char.IsDigit(space1[0]) && char.IsDigit(space2[0]))
                {
                    try
                    {
                        int thisNumericChunk = int.Parse(str1);
                        int thatNumericChunk = int.Parse(str2);
                        result = thisNumericChunk.CompareTo(thatNumericChunk);
                    }
                    catch
                    {
                        result = str1.CompareTo(str2);
                    }
                }
                else
                {
                    result = str1.CompareTo(str2);
                }

                if (result != 0)
                {
                    return result;
                }
            }
            return len1 - len2;
        }

    }
   
    public static class ExportExcel
    {
        public static void Export(this DataGrid dg, string strDateTime, string strDecLoc)
        {
            ExportDataGridExcel(dg, strDateTime, strDecLoc);
        }

        public static void ExportDataGridExcel(DataGrid dGrid, string strDateTime, string strDecLoc)
        {
            SaveFileDialog objSFD = new SaveFileDialog() { DefaultExt = "csv", Filter = "Excel XML (*.xml)|*.xml", FilterIndex = 1 };
            //objSFD.DefaultExt = "xls";
            int i = 1;
            if (objSFD.ShowDialog() == true)
            {
                StringBuilder strBuilder = new StringBuilder();

                if (dGrid.ItemsSource == null)
                    return;

                List<string> lstFields = new List<string>();
                lstFields.Add(FormatHeaderField("S.No"));
                if (dGrid.HeadersVisibility == DataGridHeadersVisibility.Column || dGrid.HeadersVisibility == DataGridHeadersVisibility.All)
                {

                    foreach (DataGridColumn dgcol in dGrid.Columns)
                    {
                        if (dgcol.Header.ToString() != "")
                            lstFields.Add(FormatHeaderField(dgcol.Header.ToString()));
                    }

                    BuildStringOfRow(strBuilder, lstFields);

                }
                foreach (object data in dGrid.ItemsSource)
                {
                    lstFields.Clear();
                    foreach (DataGridColumn col in dGrid.Columns)
                    {
                        string strValue = "";
                        strValue = i.ToString();

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
                                    strValue = pi.GetValue(data, null).ToString();
                            }
                            if (objBinding.Converter != null)
                            {
                                if (strValue != "")
                                    strValue = objBinding.Converter.Convert(strValue, typeof(string), objBinding.ConverterParameter, objBinding.ConverterCulture).ToString();
                                //else
                                //    strValue = objBinding.Converter.Convert(data, typeof(string), objBinding.ConverterParameter, objBinding.ConverterCulture).ToString();
                            }
                        }
                        lstFields.Add(FormatField(strValue));
                    }
                    BuildStringOfRow(strBuilder, lstFields);
                    i++;
                }

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
                sw.WriteLine("<Column ss:Index=\"2\" ss:AutoFitWidth=\"0\" ss:Width=\"44\"/>");
                sw.WriteLine("<Column ss:Index=\"3\" ss:AutoFitWidth=\"0\" ss:Width=\"72\"/>");
                sw.WriteLine("<Column ss:Index=\"4\" ss:AutoFitWidth=\"0\" ss:Width=\"100\"/>");
                sw.WriteLine("<Column ss:Index=\"5\" ss:AutoFitWidth=\"0\" ss:Width=\"359\"/>");
                sw.WriteLine("<Row>" + String.Format("<Cell ss:StyleID=\"head\" ss:MergeAcross=\"2\"><Data ss:Type=\"String" + "\">{0}</Data></Cell>", "Interruption Date:") + String.Format("<Cell ss:MergeAcross=\"1\"><Data ss:Type=\"String" + "\">{0}</Data></Cell>", strDateTime) + "</Row>");
                sw.WriteLine("<Row>" + String.Format("<Cell ss:StyleID=\"head\" ss:MergeAcross=\"2\"><Data ss:Type=\"String" + "\">{0}</Data></Cell>", "Interruption Area:") + String.Format("<Cell><Data ss:Type=\"String" + "\">{0}</Data></Cell>", strDecLoc) + "</Row>");
                sw.WriteLine("<Row>" + String.Format("<Cell></Cell><Cell></Cell><Cell></Cell><Cell></Cell><Cell ss:StyleID=\"date\"><Data ss:Type=\"String" + "\">{0}</Data></Cell>", DateTime.Now.ToLocalTime().ToLongDateString()) + "</Row>");
                sw.WriteLine("<Row></Row>");
                sw.WriteLine("<Row>" + String.Format("<Cell ss:StyleID=\"title\" ss:MergeAcross=\"4\"><Data ss:Type=\"String" + "\">{0}</Data></Cell>", "Customer Notification Wizard") + "</Row>");
                sw.WriteLine("<Row></Row>");
                sw.Write(strBuilder.ToString());
                sw.WriteLine("</Table>");
                sw.WriteLine("</Worksheet>");
                sw.WriteLine("</Workbook>");
                sw.Close();
            }
        }

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

    }

    public class Item
    {
        public string Name;
        public string Value;

        public Item(string name, string value)
        {
            Name = name; Value = value;
        }
        public override string ToString()
        {
            // Generates the text shown in the combo box
            return Name;
        }
    }

    public class DeviceSearchItem
    {
        public string Key { get; set; }
        public List<string> CGCList { get; set; }
    }

    public class PolygonSearchItem
    {
        public string LayerName;
        public string CGC;
        public string SSD;
        public string FeederId;
        public string Division;
        public string OperatingNum;
    }

}

