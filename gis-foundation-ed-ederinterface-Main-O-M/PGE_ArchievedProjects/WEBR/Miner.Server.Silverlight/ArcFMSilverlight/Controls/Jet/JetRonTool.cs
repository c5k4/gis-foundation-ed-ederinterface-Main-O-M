using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Json;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Xml.Linq;
using ESRI.ArcGIS.Client;
using ESRI.ArcGIS.Client.Geometry;
using ESRI.ArcGIS.Client.Symbols;
using ESRI.ArcGIS.Client.Tasks;
using Miner.Server.Client.Events;
using Miner.Server.Client.Toolkit;
using Miner.Server.Client.Toolkit.Events;
using System.Xml;

namespace ArcFMSilverlight
{
    public class JetRonTool : Control, IActiveControl
    {
        protected const string MapCursor = @"/Images/coordinfo_cursor.png";
        private const int SIZE_WIDTH = 480;  //size increased due to addition of Use last equip location checkbox - INC000004150734
        private const int SIZE_HEIGHT = 360;  //size increased due to addition of Use last equip location checkbox - INC000004150734
        private const int SIZE_SAC_HEIGHT = 140;  //size increased due to addition of Use last equip location checkbox - INC000004150734
        private int _height = SIZE_SAC_HEIGHT;
        protected const string PanCursor = @"/Images/cursor_pan.png";
        private FloatableWindow _fw = null;
        private Grid _mapArea;
        private Map _map;
        public delegate void SetPreviousControl();
        public SetPreviousControl PreviousControl { get; set; }
        private string _generateOperatingNumbersUrl;
        private string _generateCgc12Url;
        private JobReserveOperatingNumber _jobReserveOperatingNumber;
        private JetJobsTool _jetJobsTool;
        private JetEquipmentModel _jetEquipmentModel;
        //private JetJobsManager _jetJobsManager;
        private JetEquipment _jetEquipment;
        //private ChangeJobNumberJetEquipment _ChangeJobNumberJetEquipment;
        private GeometryService _geometryService;
        private GeometryService _geometryServiceLocator; // argh, need another service or complex/error-likely code
        private GraphicsLayer _graphicsLayer = null;
        private Locator _locator = null;
        private MapTools _mapTools = null;
        private int _tolerance = 40; //TODO: page.config this value
        private IList<string> _operatingNumberUrls = new List<string>();

        private bool _operatingNumberExists = false;
        private bool _operatingNumberSearchFailed = false;

        private bool _operatingNumberFinished = false;
        private bool _cgc12Finished = false;

        //private bool _operatingNumberServiceIsDown = false;
        //private bool _cgc12ServiceIsDown = false;

        private int _remainingOperatingNumberQueries = 0;
        //private string _equipmentServiceUrl;   //added to populate last equip location info - INC000004150734
        public JobReserveOperatingNumber JobReserveOperatingNumber
        {
            get
            {
                return _jobReserveOperatingNumber;
            }
        }


        public JetRonTool(Map map, JetJobsTool jetJobsTool, Grid mapArea, XElement element, JetEquipmentModel jetEquipmentModel, MapTools mapTools)
        {
            _jetJobsTool = jetJobsTool;
            _map = map;
            _mapArea = mapArea;
            _generateOperatingNumbersUrl = element.Attribute("generateOperatingNumbersUrl").Value;
            _generateCgc12Url = element.Attribute("generateCgc12Url").Value;
            _jetEquipmentModel = jetEquipmentModel;
            _geometryService = new GeometryService(ConfigUtility.GeometryServiceURL);
            _geometryServiceLocator = new GeometryService(ConfigUtility.GeometryServiceURL);
            _mapTools = mapTools;

            InitializeOperatingNumberSearch(element);
            _jetEquipmentModel.EquipmentTypeIdLoadedSuccess += new EventHandler(_jetEquipmentModel_EquipmentTypeIdLoadedSuccess);
            _jetEquipmentModel.EquipmentTypeIdLoadedFailed += new EventHandler(_jetEquipmentModel_EquipmentTypeIdLoadedFailed);

            _jetEquipmentModel.EquipmentSaveSuccess += new EventHandler(_jetEquipmentModel_EquipmentSaveSuccess);
            _jetEquipmentModel.EquipmentSaveFailed += new EventHandler(_jetEquipmentModel_EquipmentSaveFailed);
            //_equipmentServiceUrl = element.Element("EquipmentService").Attribute("Url").Value + "/" + element.Element("EquipmentService").Attribute("LayerId").Value; //added to populate last equip location info - INC000004150734

            //_jetEquipmentModel.OnQueryValidateJobNumComplete += new EventHandler(QueryTaskValidJobNumExecuteCompleted);
        }

        private void InitializeOperatingNumberSearch(XElement element)
        {
            XElement operatingNumberSearchElement =
                element.Parent.Element("Searches")
                    .Elements()
                    .Where(e => e.Attribute("Title").Value == "Operating Number")
                    .First();

            foreach (XElement searchLayerElement in operatingNumberSearchElement.Elements("SearchLayer"))
            {
                _operatingNumberUrls.Add(searchLayerElement.Attribute("Url").Value + "/" +
                                         searchLayerElement.Attribute("LayerId").Value);
            }

        }

        void _jetEquipmentModel_EquipmentSaveFailed(object sender, EventArgs e)
        {
            _jobReserveOperatingNumber.ShowMessage("Save Equipment failed", "Save failed");
        }

        void _jetEquipmentModel_EquipmentSaveSuccess(object sender, EventArgs e)
        {
            // Need to refresh list and set active equipment
            // Assume that the CW is not open

            //string operatingNumber = "";
            //string cgc12 = "";
            //if (_jetEquipment != null)
            //{
            //    if (!String.IsNullOrEmpty(_jetEquipment.OperatingNumber))
            //    {
            //        operatingNumber = _jetEquipment.OperatingNumber;
            //    }
            //    if (!String.IsNullOrEmpty(_jetEquipment.Cgc12))
            //    {
            //        cgc12 = _jetEquipment.Cgc12;
            //    }
            //}

            //_jetJobsTool.StoreSelectedEquipmentList(operatingNumber, cgc12);
            //this.IsActive = false;
            EquipmentEditingDone();
            _jetJobsTool.JetJobsManager.EquipmentJobdataload();
        }

        public void EquipmentEditingDone()
        {
            string operatingNumber = "";
            string cgc12 = "";
            if (_jetEquipment != null)
            {
                if (!String.IsNullOrEmpty(_jetEquipment.OperatingNumber))
                {
                    operatingNumber = _jetEquipment.OperatingNumber;
                }
                if (!String.IsNullOrEmpty(_jetEquipment.Cgc12))
                {
                    cgc12 = _jetEquipment.Cgc12;
                }
            }

            _jetJobsTool.StoreSelectedEquipmentList(operatingNumber, cgc12);
            this.IsActive = false;
        }


        // Using a DependencyProperty as the backing store for IsActive.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsActiveProperty =
            DependencyProperty.Register("IsActive", typeof(bool), typeof(CADExportTool), new PropertyMetadata(OnIsActiveChanged));

        private static void OnIsActiveChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var cadExportTool = (CADExportTool)d;

            var isActive = (bool)e.NewValue;

            if (isActive == false)
            {
            }
            else
            {
                EventAggregator.GetEvent<ControlActivatedEvent>().Publish(cadExportTool);
                CursorSet.SetID(cadExportTool.Map, MapCursor);
            }
        }

        public void OnControlActivated(DependencyObject control)
        {
            if (control == this) return;
            IsActive = false;
        }

        public void Initialize()
        {
            SimpleMarkerSymbol simplemarker = new SimpleMarkerSymbol();

            simplemarker.Style = SimpleMarkerSymbol.SimpleMarkerStyle.Diamond;
            simplemarker.Size = 20;
            simplemarker.Color = new SolidColorBrush(Colors.Red);
            SimpleRenderer simpleRenderer = new SimpleRenderer() { Symbol = simplemarker };
            _graphicsLayer = new GraphicsLayer() { ID = "JetGraphics", Renderer = simpleRenderer };
            _graphicsLayer.ID = "JetGraphics";
            _geometryService.ProjectCompleted += new EventHandler<GraphicsEventArgs>(_geometryService_ProjectCompleted);
            _geometryService.Failed += new EventHandler<TaskFailedEventArgs>(_geometryService_Failed);
            _geometryServiceLocator.ProjectCompleted += new EventHandler<GraphicsEventArgs>(_geometryServiceLocator_ProjectCompleted);
            _geometryServiceLocator.Failed += new EventHandler<TaskFailedEventArgs>(_geometryServiceLocator_Failed);
            _map.Layers.Add(_graphicsLayer);
            _locator = new Locator(ConfigUtility.LocatorService);
            _locator.LocationToAddressCompleted += new EventHandler<AddressEventArgs>(_locator_LocationToAddressCompleted);
            _locator.Failed += new EventHandler<TaskFailedEventArgs>(_locator_Failed);
            PreviousControl = _mapTools.ViewModel.EnablePan;
        }

        void _geometryServiceLocator_Failed(object sender, TaskFailedEventArgs e)
        {
            _jobReserveOperatingNumber.ShowMessage("Locator failed", "Locator failed");
        }

        void _geometryServiceLocator_ProjectCompleted(object sender, GraphicsEventArgs e)
        {
            var point = (MapPoint)e.Results[0].Geometry;

            if (_locator.IsBusy)
            {
                _locator.CancelAsync();
            }
            _locator.LocationToAddressAsync(point, _tolerance);

        }

        void _geometryService_Failed(object sender, TaskFailedEventArgs e)
        {
            _jobReserveOperatingNumber.ShowMessage("Failed to Project Geometry", "Project failed");
        }

        void _geometryService_ProjectCompleted(object sender, GraphicsEventArgs e)
        {
            if (((MapPoint)e.Results[0].Geometry).SpatialReference.WKID == 4326)
            {
                var point = (MapPoint)e.Results[0].Geometry;
                _jetEquipment.Latitude = point.Y;
                _jobReserveOperatingNumber.TxtLatitude.Text = point.Y.ToString();
                _jetEquipment.Longitude = point.X;
                _jobReserveOperatingNumber.TxtLongitude.Text = point.X.ToString();
            }

        }

        public void SetInstallCdCbo()
        {
            if (_jobReserveOperatingNumber != null)
            {
                _jobReserveOperatingNumber.cboInstallType.ItemsSource = JetEquipmentModel._jetInstallTypes;
                _jobReserveOperatingNumber.cboInstallType.DisplayMemberPath = "Value";
                _jobReserveOperatingNumber.cboInstallType.SelectedIndex = 0;
            }
        }

        void _jetEquipmentModel_EquipmentTypeIdLoadedFailed(object sender, EventArgs e)
        {
            _jobReserveOperatingNumber.ShowMessage("Load EquipmentTypeId failed", "Load failed");
        }

        void _jetEquipmentModel_EquipmentTypeIdLoadedSuccess(object sender, EventArgs e)
        {
            if (_jobReserveOperatingNumber != null)
            {
                _jobReserveOperatingNumber.EquipmentIdTypeValues = JetEquipmentModel._equipmentIdTypeValues;
            }
        }

        /// <summary>
        /// Geometry Service URL.
        /// </summary>
        public static readonly DependencyProperty GeometryServiceProperty = DependencyProperty.Register("GeometryService", typeof(string),
            typeof(SAPNotificationTool), null);

        [Category("Measure Properties")]
        [Description("Geometry Service URL")]
        [EditorBrowsable(EditorBrowsableState.Always)]
        public string GeometryService
        {
            get { return (string)GetValue(GeometryServiceProperty); }
            set
            {
                SetValue(GeometryServiceProperty, value);
            }
        }
        /// <summary>
        /// Current map.
        /// </summary>
        public static readonly DependencyProperty MapProperty = DependencyProperty.Register(
            "Map",
            typeof(Map),
            typeof(JetJobsTool),
            new PropertyMetadata(OnMapChanged));

        [Category("Export Properties")]
        [Description("Associated Map")]
        [EditorBrowsable(EditorBrowsableState.Always)]
        public Map Map
        {
            get { return (Map)GetValue(MapProperty); }
            set { SetValue(MapProperty, value); }
        }

        public bool SaveAndContinueMode
        {
            set
            {
                if (value)
                {
                    _height = SIZE_SAC_HEIGHT;
                    _fw.UpdateLayout();
                }
                else
                {
                    _fw.Height = _height = SIZE_HEIGHT;
                    _fw.UpdateLayout();
                }
            }
        }

        private static void OnMapChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
        }

        public void EnableLocationCapture()
        {
            _mapTools.ViewModel.EnablePan();
            CursorSet.SetID(_map, MapCursor);
            _map.MouseClick += new EventHandler<ESRI.ArcGIS.Client.Map.MouseEventArgs>(_map_MouseClick);
        }

        public void ClearGraphics()
        {
            ConfigUtility.UpdateStatusBarText("");
            CursorSet.SetID(_map, PanCursor);
            _graphicsLayer.ClearGraphics();
            _map.MouseClick -= _map_MouseClick;
        }

        void _map_MouseClick(object sender, Map.MouseEventArgs e)
        {
            ConfigUtility.UpdateStatusBarText("Getting Map Location...");
            _graphicsLayer.ClearGraphics();
            _jobReserveOperatingNumber.CloseSetMapLocationTtTb();

            Graphic graphicPoint = new Graphic()
            {
                Geometry = e.MapPoint,
            };
            _graphicsLayer.Graphics.Add(graphicPoint);

            SpatialReference sp = new SpatialReference(4326);
            //if (_geometryService.IsBusy)
            //{
            _geometryService.CancelAsync();
            //}
            _geometryService.ProjectAsync(_graphicsLayer.Graphics, sp, e.MapPoint);

            // Have to get 102100 for the reverse geocoder to work
            var pointClone = e.MapPoint.Clone();
            Graphic graphicPoint2 = new Graphic()
            {
                Geometry = pointClone,
            };
            GraphicCollection gc = new GraphicCollection() { graphicPoint2 };
            SpatialReference sp2 = new SpatialReference(102100);

            _geometryServiceLocator.CancelAsync();
            _geometryServiceLocator.ProjectAsync(gc, sp2, pointClone);

        }

        void _locator_Failed(object sender, TaskFailedEventArgs e)
        {
            _jobReserveOperatingNumber.ShowMessage("Failed to find an address for the point selected.\nPlease Try again", "Address Failure");

            _jetEquipment.Address = "";
            _jetEquipment.City = "";
            // Binding fail
            _jobReserveOperatingNumber.TxtAddress.Text = "";
            _jobReserveOperatingNumber.TxtCity.Text = "";
        }

        void _locator_LocationToAddressCompleted(object sender, AddressEventArgs e)
        {
            ConfigUtility.UpdateStatusBarText("");

            Address address = e.Address;
            Dictionary<string, object> attributes = address.Attributes;

            // Parameter names of address candidates may differ between geocode services
            string addressString = Convert.ToString(attributes["Address"]);
            string city = Convert.ToString(attributes["City"]);

            // Binding fail
            // Will not override existing
            if (_jobReserveOperatingNumber.TxtAddress.Text == "")
            {
                _jobReserveOperatingNumber.TxtAddress.Text = addressString;
            }
            if (_jobReserveOperatingNumber.TxtCity.Text == "")
            {
                _jobReserveOperatingNumber.TxtCity.Text = city;
            }

        }

        public void DisableLoationCapture()
        {
            // Is this enough?
            CursorSet.SetID(_map, PanCursor);
        }

        public void GenerateIds()
        {
            JetEquipmentType jetEquipmentType =
                ((KeyValuePair<short, JetEquipmentType>)_jobReserveOperatingNumber.cboEquipmentType.SelectedItem).Value;

            _cgc12Finished = _operatingNumberFinished = false;
            _jobReserveOperatingNumber.TxtCgc12.Text = _jobReserveOperatingNumber.TxtOperatingNumber.Text = "";

            _jetEquipment.HasOperatingNumber = jetEquipmentType.HasOperatingNumberBool;
            _jetEquipment.HasCgc12 = jetEquipmentType.HasCgc12Bool;
            _jetEquipment.EquipTypeId = jetEquipmentType.EquipTypeId;
            _jetEquipment.CustOwnedBool = _jobReserveOperatingNumber.chkCustomerOwned.IsChecked.Value;

            if (_jetEquipment.HasCgc12)
            {
                GenerateCgc12();
            }
            if (_jetEquipment.HasOperatingNumber)
            {
                GenerateOperatingNumbers();
            }
        }

        public void GenerateCgc12()
        {
            ConfigUtility.UpdateStatusBarText("Generating CGC12...");

            string prefixUrl;
            prefixUrl = GetPrefixUrl();
            UriBuilder uriBuilder = new UriBuilder(prefixUrl + _generateCgc12Url);
            WebClient client = new WebClient();

            client.DownloadStringCompleted += (s, args) =>
            {
                JsonObject cgc12 = (JsonObject)JsonObject.Parse(args.Result);
                // Not sure why quotes are coming back...
                _jobReserveOperatingNumber.TxtCgc12.Text = cgc12["cgc12"].ToString().Replace("\"", "");
                _jobReserveOperatingNumber.LblCgc12.Visibility = Visibility.Visible;

                _cgc12Finished = true;

                if (!_jetEquipment.HasOperatingNumber || (_jetEquipment.HasOperatingNumber && _operatingNumberFinished))
                {
                    GenerateIdQueriesFinished();
                }
            };
            client.DownloadStringAsync(uriBuilder.Uri);
        }

        private static string GetPrefixUrl()
        {
            string prefixUrl;
            var pageUri = "";
            if (System.Windows.Browser.HtmlPage.Document.DocumentUri.Query != null && System.Windows.Browser.HtmlPage.Document.DocumentUri.Query != "")
                pageUri = System.Windows.Browser.HtmlPage.Document.DocumentUri.ToString().Replace(System.Windows.Browser.HtmlPage.Document.DocumentUri.Query.ToString(), "");
            else
                pageUri = System.Windows.Browser.HtmlPage.Document.DocumentUri.ToString();
            if (pageUri.Contains("Default.aspx"))
            {
                prefixUrl = pageUri.Substring(0, pageUri.IndexOf("Default.aspx"));
            }
            else
            {
                prefixUrl = pageUri;
            }
            return prefixUrl;
        }

        public void GenerateOperatingNumbers(int numOperatingNumbers = 1)
        {
            ConfigUtility.UpdateStatusBarText("Generating Operating Number...");

            string queryStringSuffix = "?equipmentId=" + _jetEquipment.EquipTypeId + "&isCustomer=" + _jetEquipment.CustOwnedBool +
                                       "&numOperatingNumbers=1";

            string prefixUrl = GetPrefixUrl();
            UriBuilder uriBuilder = new UriBuilder(prefixUrl + _generateOperatingNumbersUrl + queryStringSuffix);
            WebClient client = new WebClient();

            client.DownloadStringCompleted += (s, args) =>
            {
                try
                {
                    JsonArray operatingNumbers = (JsonArray)JsonArray.Parse(args.Result);
                    _jobReserveOperatingNumber.TxtOperatingNumber.Text = operatingNumbers[0];
                    _jobReserveOperatingNumber.LblOperatingNumber.Visibility = Visibility.Visible;

                    CheckExistingOperatingNumber();
                }
                catch (Exception)
                {
                    _jobReserveOperatingNumber.ShowMessage("Error Generating Operating Number", "Error");
                }
            };
            client.DownloadStringAsync(uriBuilder.Uri);

        }

        void CheckExistingOperatingNumber()
        {
            ConfigUtility.UpdateStatusBarText("Checking Operating Number [ " + _jobReserveOperatingNumber.TxtOperatingNumber.Text + " ]");

            _remainingOperatingNumberQueries = _operatingNumberUrls.Count;
            _operatingNumberExists = false;
            _operatingNumberSearchFailed = false;

            foreach (string operatingNumberUrl in _operatingNumberUrls)
            {
                QueryTask opnumQueryTask = new QueryTask(operatingNumberUrl);
                opnumQueryTask.ExecuteCompleted += OpnumQueryTaskOnExecuteCompleted;
                opnumQueryTask.Failed += new EventHandler<TaskFailedEventArgs>(opnumQueryTask_Failed);
                Query query = new Query()
                {
                    Where = "OPERATINGNUMBER='" + _jobReserveOperatingNumber.TxtOperatingNumber.Text + "'"
                };
                opnumQueryTask.ExecuteAsync(query);
            }
        }

        private void opnumQueryTask_Failed(object sender, TaskFailedEventArgs e)
        {
            // Technically we could cancel existing queries
            _operatingNumberSearchFailed = true;

            if (--_remainingOperatingNumberQueries == 0)
            {
                if (!_jetEquipment.HasCgc12 || (_jetEquipment.HasCgc12 && _cgc12Finished))
                {
                    GenerateIdQueriesFinished();
                }
            }
        }

        private void OpnumQueryTaskOnExecuteCompleted(object sender, QueryEventArgs queryEventArgs)
        {
            if (queryEventArgs.FeatureSet.Features != null &&
                queryEventArgs.FeatureSet.Features.Count > 0)
            {
                _operatingNumberExists = true;
            }

            if (--_remainingOperatingNumberQueries == 0)
            {
                GenerateIdQueriesFinished();
            }
        }

        void GenerateIdQueriesFinished()
        {
            ConfigUtility.UpdateStatusBarText("");
            _jobReserveOperatingNumber.BusyIndicator.IsBusy = false;
            _jobReserveOperatingNumber.SaveContinueButton.IsEnabled = true;

            if (_operatingNumberSearchFailed)
            {
                _jobReserveOperatingNumber.ShowMessage("Operating Number Search failed", "Failed Search");
            }
            else if (_operatingNumberExists)
            {
                MessageBoxResult messageBoxResult = _jobReserveOperatingNumber.ShowMessage("Generated Operating Number exists\nPlease click OK to retry", "Operating Number exists", MessageBoxButton.OKCancel);
                if (messageBoxResult == MessageBoxResult.OK)
                {
                    GenerateOperatingNumbers();
                }
            }
            else // happy path
            {
                SaveAndContinueMode = false;
                _jobReserveOperatingNumber.SaveAndContinue = false;
                SetInstallTypeCbo();
            }
        }

        // Logic for configuring the Install Dropdown. Subtypes might have been cleaner
        //Equipment Type	Odd / Even Restriction	Alpha	Lock Install Type	Dart Equipment
        //Subsurface Switch
        //(Switch SubtypeCD=5)	Even	 	Subsurface	Switch
        //Oil Switch (w/Padmount) Transformer)	Even	 	 Unlocked	Switch
        //Sectionalizer	Even	 	 Unlocked	Sectionalizer
        //Recloser	Even	 	 Unlocked	Recloser
        //Interrupter	Even	 	 Unlocked	Interrupter
        //Subsurface Fuse	Even	 	Subsurface	Fuse
        //Underground Fuse	Even	 	Underground	Fuse
        //Underground Switch	Even	 	Underground	Switch
        //OH Fused Cutouts
        //(clarify by ByPassSwitch attribute)	Odd	 	Overhead	Fuse
        //PMH Switch	Odd	 	Padmounted	Switch
        //PMH Fuse	Odd	 	Padmounted	Fuse
        //Disconnects	Odd	 	 Unlocked	Switch
        //OH Switch	Odd	 	Overhead	Switch
        //Regulator	 	R	 Unlocked	Regulator
        //Capacitor	 	C	 Unlocked	Capacitor
        //Booster	 	B	 Unlocked	Booster
        //Stepdown	 	 	 Unlocked	Stepdown
        //Transformer	 	T	Exclude Overhead	Transformer
        //ESNA J-box	 	J	 Unlocked	 

        public void SetInstallTypeCbo(bool useExisting = false)
        {
            bool isLocked = false;

            _jobReserveOperatingNumber.cboInstallType.ItemsSource =
                JetEquipmentModel._jetInstallTypes;

            if (useExisting)
            {
                _jobReserveOperatingNumber.cboInstallType.SelectedItem = new KeyValuePair<string, string>(_jetEquipment.InstallCd,
                            _jetEquipment.InstallCdName);
            }
            else // new equipment
            {
                _jobReserveOperatingNumber.cboInstallType.SelectedItem = null;
            }

            if (_jetEquipment.EquipTypeName.StartsWith("PM") || _jetEquipment.EquipTypeName.ToString() == "Padmounted Switch")  //addition of Padmountes Switch equip type - INC000004174075
            {
                _jobReserveOperatingNumber.cboInstallType.SelectedItem = new KeyValuePair<string, string>("PM",
                    "Padmounted");
                isLocked = true;
            }
            else if (_jetEquipment.EquipTypeName.StartsWith("OH") || _jetEquipment.EquipTypeName.ToString() == "Recloser(TripSaver)")// addition  Trip saver
            {
                _jobReserveOperatingNumber.cboInstallType.SelectedItem = new KeyValuePair<string, string>("OH",
                    "Overhead");
                isLocked = true;
            }
            else if (_jetEquipment.EquipTypeName.StartsWith("Subsurface"))
            {
                _jobReserveOperatingNumber.cboInstallType.SelectedItem = new KeyValuePair<string, string>("SS",
                    "Subsurface");
                isLocked = true;
            }
            // Underground Transformers are different, Unlocked to SS/PM/UG
            else if (_jetEquipment.EquipTypeName.StartsWith("UG Transformer"))
            {
                _jobReserveOperatingNumber.cboInstallType.ItemsSource =
                    JetEquipmentModel._jetInstallTypes.Where(k => k.Key != "OH");
                if (useExisting)
                {
                    // Would be invalid, let's reset
                    if (_jetEquipment.EquipTypeName == "OH")
                    {
                        _jobReserveOperatingNumber.cboInstallType.SelectedItem = null;
                    }
                    else
                    {
                        _jobReserveOperatingNumber.cboInstallType.SelectedItem = new KeyValuePair<string, string>(_jetEquipment.InstallCd,
                            _jetEquipment.InstallCdName);
                    }
                }
            }
            //TODO
            else if (_jetEquipment.EquipTypeName.StartsWith("Underground") || _jetEquipment.EquipTypeName.StartsWith("Primary Meter"))
            {
                _jobReserveOperatingNumber.cboInstallType.SelectedItem = new KeyValuePair<string, string>("UG",
                    "Underground");
                isLocked = true;
            }

            if (_jobReserveOperatingNumber.cboInstallType.SelectedItem == null)
            {
                _jobReserveOperatingNumber.cboInstallType.SelectedIndex = 0;
                _jobReserveOperatingNumber.cboInstallType.IsEnabled = true;
            }
            else
            {
                if (isLocked)
                {
                    _jobReserveOperatingNumber.cboInstallType.IsEnabled = false;
                }
                else
                {
                    _jobReserveOperatingNumber.cboInstallType.IsEnabled = true;
                }
            }

        }

        public void ShowReserveOperatingNumber(bool createNew = true)
        {
            _jetJobsTool.IsActive = false;
            IsActive = true;
            if (createNew)
            {
                _jetEquipment = new JetEquipment()
                {
                    JobNumber = _jetJobsTool.SelectedJob.JobNumber,
                    EntryDate = DateTime.Now,
                    CustOwned = "0",
                    LastEquipLoc = "0"  //to uncheck initially use last equip location checkbox - INC000004150734
                };
                SaveAndContinueMode = true;
                _jobReserveOperatingNumber.SaveAndContinue = true;
                SetRonUi();
                _jobReserveOperatingNumber.SetForm1Valid();
            }
            else // Edit
            {
                _jetEquipment = _jetJobsTool.JetJobsManager.equipmentListBox.SelectedItem as JetEquipment;
                SaveAndContinueMode = false;
                _jobReserveOperatingNumber.SaveAndContinue = false;
                SetRonUi();
                SetInstallTypeCbo(true);
            }
            _jobReserveOperatingNumber.LayoutRoot.DataContext = _jetEquipment;
        }

        public void SaveEquipment()
        {
            // Bring the props down locally
            //GetRonUi();
            //if (_jetEquipment.ObjectId > 0) //Edit Equipment
            //{
            //    try
            //    {
            //        EquipmentEditingDone();
            //        //call WCF
            //        _jetJobsTool.JetJobsManager.EditEquipment(_jetEquipment);
            //    }
            //    catch
            //    {
            //    }
            //}
            //else //Reserve new equipment
            //{
            //    _jetEquipmentModel.Save(_jetEquipment);  
            //}

            //WEBR Stability - Replace Feature Service with WCF
            try
            {
                // Bring the props down locally
                GetRonUi();
                _jetEquipment.UserAudit = WebContext.Current.User.Name.Replace("PGE\\", "").ToUpper();
                EquipmentEditingDone();
                //call WCF
                _jetJobsTool.JetJobsManager.AddEditEquipment(_jetEquipment);
            }
            catch
            {
                _jetEquipmentModel_EquipmentSaveFailed(new object(), new EventArgs());
            }
        }

        //Code change-TCS-INC000004186986
        //public void updateJobnumberEquipment()
        //{
        //    // Bring the props down locally

        //    _jetEquipmentModel.SaveJobNumber(_jetJobsTool.JetJobsManager.JobEditWindow.JetJob.PreviousJobNumber, _jetJobsTool.JetJobsManager.JobEditWindow.JetJob.JobNumber);

        //}

        //binding fail
        void SetRonUi()
        {
            _jobReserveOperatingNumber.TxtOperatingNumber.Text = Convert.ToString(_jetEquipment.OperatingNumber);
            _jobReserveOperatingNumber.LblOperatingNumber.Visibility = !String.IsNullOrEmpty(_jetEquipment.OperatingNumber) ?
                Visibility.Visible : Visibility.Collapsed;
            _jobReserveOperatingNumber.TxtCgc12.Text = Convert.ToString(_jetEquipment.Cgc12);
            _jobReserveOperatingNumber.LblCgc12.Visibility = !String.IsNullOrEmpty(_jetEquipment.Cgc12) ?
                Visibility.Visible : Visibility.Collapsed;

            _jobReserveOperatingNumber.TxtAddress.Text = Convert.ToString(_jetEquipment.Address);
            _jobReserveOperatingNumber.TxtCity.Text = Convert.ToString(_jetEquipment.City);
            _jobReserveOperatingNumber.TxtLatitude.Text = _jetEquipment.Latitude.ToString();
            _jobReserveOperatingNumber.TxtLongitude.Text = _jetEquipment.Longitude.ToString();
            _jobReserveOperatingNumber.TxtSketchLocation.Text = Convert.ToString(_jetEquipment.SketchLoc);
            if (!String.IsNullOrEmpty(_jetEquipment.InstallCd))
            {
                _jobReserveOperatingNumber.cboInstallType.SelectedItem = new KeyValuePair<string, string>(_jetEquipment.InstallCd, _jetEquipment.InstallCdName);
            }
            if (_jetEquipment.EquipTypeId > 0)
            {
                _jobReserveOperatingNumber.cboEquipmentType.SelectedItem = new KeyValuePair<short, JetEquipmentType>(_jetEquipment.EquipTypeId, JetEquipmentModel._equipmentIdTypeValues[_jetEquipment.EquipTypeId]);
            }
            _jobReserveOperatingNumber.chkCustomerOwned.IsChecked = _jetEquipment.CustOwnedBool;
            _jobReserveOperatingNumber.chkLastLocation.IsChecked = _jetEquipment.LastEquipLocBool; //to populate last equip location info - INC000004150734
        }


        void GetRonUi()
        {
            _jetEquipment.Address = _jobReserveOperatingNumber.TxtAddress.Text;
            _jetEquipment.City = _jobReserveOperatingNumber.TxtCity.Text;
            _jetEquipment.Latitude = Convert.ToDouble(_jobReserveOperatingNumber.TxtLatitude.Text);
            _jetEquipment.Longitude = Convert.ToDouble(_jobReserveOperatingNumber.TxtLongitude.Text);
            _jetEquipment.SketchLoc = _jobReserveOperatingNumber.TxtSketchLocation.Text;
            _jetEquipment.InstallCd = Convert.ToString(((KeyValuePair<string, string>)_jobReserveOperatingNumber.cboInstallType.SelectedValue).Key);
            _jetEquipment.EquipTypeId = Convert.ToInt16(((KeyValuePair<short, JetEquipmentType>)_jobReserveOperatingNumber.cboEquipmentType.SelectedValue).Key);
            _jetEquipment.CustOwnedBool = _jobReserveOperatingNumber.chkCustomerOwned.IsChecked.Value;
            _jetEquipment.OperatingNumber = _jobReserveOperatingNumber.TxtOperatingNumber.Text;
            _jetEquipment.Cgc12 = _jobReserveOperatingNumber.TxtCgc12.Text;
            _jetEquipment.LastEquipLocBool = _jobReserveOperatingNumber.chkLastLocation.IsChecked.Value; //to populate last equip location info - INC000004150734
        }

        //to enable/disable Copy last equipment location checkbox - INC000004150734
        public bool ChkLastLocEnability()
        {
            if (_jetJobsTool.JetJobsManager.equipmentListBox.ItemsSource != null)
                return true;
            else
                return false;
        }

        //Commented for WEBR Stability - Replace Feature Service with WCF
        //To get last equipment location - INC000004150734
        //public void GetLastEquipLoc()
        //{
        //    QueryTask queryTask = new QueryTask(_equipmentServiceUrl);
        //    Query query = new Query();
        //    query.OutFields.Add("ADDRESS");
        //    query.OutFields.Add("CITY");
        //    query.OutFields.Add("LATITUDE");
        //    query.OutFields.Add("LONGITUDE");
        //    query.Where = "JOBNUMBER = '" + _jetJobsTool.SelectedJob.JobNumber + "'";
        //    query.OrderByFields = new List<OrderByField>() { new OrderByField("LASTMODIFIEDDATE", SortOrder.Descending) };
        //    queryTask.ExecuteCompleted += QueryTaskLastEquip_Completed;
        //    queryTask.ExecuteAsync(query);
        //}

        //INC000004150734
        //private void QueryTaskLastEquip_Completed(object sender, QueryEventArgs queryEventArgs)
        //{
        //    if (queryEventArgs.FeatureSet != null && queryEventArgs.FeatureSet.Features != null && queryEventArgs.FeatureSet.Features.Count > 0 && queryEventArgs.FeatureSet.Features[0].Attributes.Keys.Count > 0)
        //    {
        //        //populate last equipment location
        //        _jobReserveOperatingNumber.TxtAddress.Text = queryEventArgs.FeatureSet.Features[0].Attributes["ADDRESS"].ToString();
        //        _jobReserveOperatingNumber.TxtCity.Text = queryEventArgs.FeatureSet.Features[0].Attributes["CITY"].ToString();
        //        _jobReserveOperatingNumber.TxtLatitude.Text = queryEventArgs.FeatureSet.Features[0].Attributes["LATITUDE"].ToString();
        //        _jobReserveOperatingNumber.TxtLongitude.Text = queryEventArgs.FeatureSet.Features[0].Attributes["LONGITUDE"].ToString();
        //    }
        //}
                
        //WEBR Stability - Replace Feature Service with WCF
        public string GetSelectedJobNumber()
        {
            return _jetJobsTool.SelectedJob.JobNumber;
        }

        public void UpdateJobNumberEquipment()
        {
            EquipmentEditingDone();
            //call WCF
            _jetJobsTool.JetJobsManager.EditJobNumInEquipment(_jetJobsTool.JetJobsManager.JobEditWindow.JetJob.PreviousJobNumber, _jetJobsTool.JetJobsManager.JobEditWindow.JetJob.JobNumber);
        }

        //WEBR Stability - Replace Feature Service with WCF
        public string GetWCFEndPointAddress()
        {
            return _jetJobsTool.JetJobsManager.GetWCFEndPointAddress(); ;
        }

        #region IActiveControl Members

        private bool _isActive = false;
        public bool IsActive
        {
            get { return _isActive; }
            set { setActive(value); }
        }

        public void setActive(bool isActive, bool openJetJobsTool = true)
        {
            _isActive = isActive;
            if (getFloatableWindow() == null) return;

            if (!isActive)
            {
                // Clear everything
                _fw.Visibility = System.Windows.Visibility.Collapsed;
                ClearGraphics();
                if (openJetJobsTool)
                {
                    _jetJobsTool.IsActive = true;
                }
                this.SaveAndContinueMode = true;
                _jobReserveOperatingNumber.SaveAndContinue = true;
            }
            else // Active
            {
                // Reset in case the widget gets lost. Not ideal.
                _fw.Height = _mapArea.ActualHeight < SIZE_HEIGHT ? _mapArea.ActualHeight - 100 : _height;
                //                _fw.Height = _height;
                _fw.Width = SIZE_WIDTH;
                _fw.Visibility = System.Windows.Visibility.Visible;
            }
        }



        private void SetVerticalAlignment(double height)
        {
            if (height >= SIZE_HEIGHT)
            {
                _fw.VerticalAlignment = System.Windows.VerticalAlignment.Center;
            }
            else
            {
                _fw.VerticalAlignment = System.Windows.VerticalAlignment.Top;
                _fw.Height = height - 100;
            }
        }

        private FloatableWindow getFloatableWindow()
        {
            if (_fw == null)
            {
                _fw = new FloatableWindow();
                _fw.ParentLayoutRoot = _mapArea;
                _mapArea.SizeChanged += new SizeChangedEventHandler(_mapArea_SizeChanged);
                _fw.Width = SIZE_WIDTH;
                _fw.Height = _height;
                _fw.Title = "Reserve Equipment";
                _fw.HorizontalAlignment = System.Windows.HorizontalAlignment.Right;
                SetVerticalAlignment(_mapArea.ActualHeight);
                _fw.ResizeMode = ResizeMode.NoResize;
                _jobReserveOperatingNumber = new JobReserveOperatingNumber(this);
                if (JetEquipmentModel._equipmentIdTypeValues.Count > 0)
                {
                    _jobReserveOperatingNumber.EquipmentIdTypeValues = JetEquipmentModel._equipmentIdTypeValues;
                }
                if (JetEquipmentModel._jetInstallTypes.Count > 0)
                {
                    _jobReserveOperatingNumber.cboInstallType.ItemsSource = JetEquipmentModel._jetInstallTypes;
                    _jobReserveOperatingNumber.cboInstallType.DisplayMemberPath = "Value";
                    _jobReserveOperatingNumber.cboInstallType.SelectedIndex = 0;
                }
                _fw.Content = _jobReserveOperatingNumber;
                _fw.Closing += new EventHandler<CancelEventArgs>(_fw_Closing);
                _fw.SizeChanged += new SizeChangedEventHandler(_fw_SizeChanged);
                _fw.Show();
                //TODO: offset http://floatablewindow.codeplex.com/discussions/279618
                //BUG: I cannot seem to modify the position of this window, come hell or high water. 
                // Presumably this is because we are setting ourselves to the MapArea, not a Canvas
                setActive(true);
            }
            return _fw;
        }

        void _mapArea_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            SetVerticalAlignment(e.NewSize.Height);
        }

        void _fw_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            const double MIN_HEIGHT = 50;
            const double MIN_WIDTH = 30;

            if (_fw.ActualHeight <= MIN_HEIGHT || _fw.Width <= MIN_WIDTH)
            {
                //_legendControl.Legend.MinWidth = _fw.ActualWidth;
                //_legendControl.Legend.MinHeight = _fw.ActualHeight;
            }
            else
            {
                //_legendControl.Legend.MinWidth = _fw.ActualWidth - 30;
                //_legendControl.Legend.MinHeight = _fw.ActualHeight - 50;
            }
            //_legendControl.Legend.UpdateLayout();
        }

        void _fw_Closing(object sender, CancelEventArgs e)
        {
            ClearGraphics();
            _jobReserveOperatingNumber.CloseSetMapLocationTtTb();
            //            _jetToggleButton.IsChecked = false;
            _jobReserveOperatingNumber.SaveAndContinue = true;
            this.SaveAndContinueMode = true;
            _jetJobsTool.IsActive = true;
            this.IsActive = false;
            // Always cancel the real "closing" because "unclosing" seems tricky
            e.Cancel = true;
            _fw.Visibility = System.Windows.Visibility.Collapsed;
            //if (this.PreviousControl != null) this.PreviousControl();
        }

        #endregion


    }
}
