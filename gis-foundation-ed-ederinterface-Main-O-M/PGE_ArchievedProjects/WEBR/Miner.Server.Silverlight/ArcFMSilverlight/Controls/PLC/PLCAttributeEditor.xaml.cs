using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using ESRI.ArcGIS.Client;
using System.Text;
using ESRI.ArcGIS.Client.Tasks;
using NLog;
using System.Windows.Browser;
using System.Security;
using ESRI.ArcGIS.Client;
using System.ComponentModel;

namespace ArcFMSilverlight
{
    public partial class PLCAttributeEditor : ChildWindow
    {

        private object _ownerLanId = null;
        private object _poleData = null;
        private string _poleLayerName = string.Empty;
        private string _poleLabelsLayerName = string.Empty;
        private Map _MapProperty = null;
        private Graphic _graphic = null;
        private Graphic _selectPolegraphic = null;
        private FeatureLayer _poleLayer = null;
        private Logger logger = LogManager.GetCurrentClassLogger();
        private string _poleFeatureServiceUrl = string.Empty;
        private string _poleDynamicServiceUrl = string.Empty;
        private string _plcDynamicLayerId = string.Empty;
        private string _plcWIPServiceUrl = string.Empty;
        private string _pgeDataFields = string.Empty;
        private static string poleMarkerID = "PoleGraphicID";
        private object _orderNumber = null;
        private object _sapEquipId = null;
        private object _poleNumber = null;
        private object _sketchLocation = null;
        private object _latitude = null;
        private object _longitude = null;
        private object _copyPoleNo = null;
        private object _snowLoadDist = null;
        private object _matCode = null;
        private object _elevation = null;
        private object _description = null;
        private object _lanID = null;
        private object _creationDate = null;
        private object _notificationNo = null;
        private string _wipOrderNumber = string.Empty;
        private object _objectIdSelected = null;
        private object _newPoleObjectId = null;
        private bool _isDelete = false;
        private bool _isCopy = false;
        private bool _isSelect = false;
        private bool _isNewPole = false;
        private string _authorizationHeader = string.Empty;
        private string _saveEIServiceUrl = string.Empty;
        private string _removePoleEIServiceUrl = string.Empty;
        private PLCWidget _plcWidgetObj = null;
        private string _sketchLocationValue = string.Empty;


        //DA# 200103 ME Q1 2020 - START
        private ESRI.ArcGIS.Client.Geometry.Geometry _selectedPoleGeom;
        private Dictionary<string, string> _ocalcV6PMOrderNotificNoList;
        //DA# 200103 ME Q1 2020 - END

        public PLCAttributeEditor()
        {
            InitializeComponent();

        }

        public string PoleLabelsLayerName
        {
            get { return _poleLabelsLayerName; }
            set { _poleLabelsLayerName = value; }
        }

        public object ObjectIdSelected
        {
            get { return _objectIdSelected; }
            set { _objectIdSelected = value; }
        }

        public PLCWidget PLCWidegetObj
        {
            get { return _plcWidgetObj; }
            set { _plcWidgetObj = value; }
        }

        public FeatureLayer PoleLayer
        {
            get { return _poleLayer; }
            set { _poleLayer = value; }
        }

        public string WipOrderNumber
        {
            get { return _wipOrderNumber; }
            set { _wipOrderNumber = value; }
        }

        public bool IsCopy
        {
            get { return _isCopy; }
            set { _isCopy = value; }
        }


        public string PoleLayerName
        {
            get { return _poleLayerName; }
            set { _poleLayerName = value; }
        }

        public Map MapProperty
        {
            get { return _MapProperty; }
            set { _MapProperty = value; }
        }

        public Graphic SelectPolegraphic
        {
            get { return _selectPolegraphic; }
            set { _selectPolegraphic = value; }
        }

        public Graphic Graphic
        {
            get { return _graphic; }
            set { _graphic = value; }
        }

        public string PoleFeatureServiceUrl
        {
            get { return _poleFeatureServiceUrl; }
            set { _poleFeatureServiceUrl = value; }
        }

        public string PoleDynamicServiceUrl
        {
            get { return _poleDynamicServiceUrl; }
            set { _poleDynamicServiceUrl = value; }
        }

        public string PLCDynamicLayerId
        {
            get { return _plcDynamicLayerId; }
            set { _plcDynamicLayerId = value; }
        }

        public string PLCWIPServiceUrl
        {
            get { return _plcWIPServiceUrl; }
            set { _plcWIPServiceUrl = value; }
        }
        public string PGEDataFields
        {
            get { return _pgeDataFields; }
            set { _pgeDataFields = value; }
        }
        public string AuthorizationHeader
        {
            get { return _authorizationHeader; }
            set { _authorizationHeader = value; }
        }
        public string SaveEIServiceUrl
        {
            get { return _saveEIServiceUrl; }
            set { _saveEIServiceUrl = value; }
        }

        public string RemovePoleEIServiceUrl
        {
            get { return _removePoleEIServiceUrl; }
            set { _removePoleEIServiceUrl = value; }
        }

        //DA# 200103 ME Q1 2020 - START
        public ESRI.ArcGIS.Client.Geometry.Geometry SelectedPoleGeom
        {
            get { return _selectedPoleGeom; }
            set { _selectedPoleGeom = value; }
        }
        //DA# 200103 ME Q1 2020 - END

        // Pole Popup Save button click function
        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string orderNoSelected = Convert.ToString(((System.Collections.Generic.KeyValuePair<string, string>)(cboOrderNo.SelectedItem)).Value);
                if (orderNoSelected != "Null" && orderNoSelected != null && orderNoSelected != "")
                {
                    if (sketchLocTxt.Text != "" && sketchLocTxt.Text != null)
                    {
                        string SktchLocation = getsketchLocation();

                        if (SktchLocation != null && sketchLocTxt.Text.Length <= 14)
                        {
                            _sketchLocationValue = SktchLocation;
                            validateUniqueSktchLocation(_sketchLocationValue);



                        }
                        else if (sketchLocTxt.Text.Length > 14)
                        {
                            MessageBox.Show("Sketch Location Length should be less than 14");
                        }
                        else
                        {
                            MessageBox.Show("Please enter valid Sketch Location");
                        }
                    }
                    else
                    {
                        MessageBox.Show("Sketch Location should not be blank");



                    }
                }
                else
                {
                    MessageBox.Show("Order Number should not be null. Please select order number from Dropdown.");
                }
            }
            catch (Exception ex)
            {
                logger.Error("PLC Save Pole Button Exception: " + ex.Message);
            }
        }

        //Function to validate Unique Sketch Location
        private void validateUniqueSktchLocation(string sktchLocation)
        {
            try
            {
                PLCBusyIndicator.IsBusy = true;
                string poleOrderNo = ((System.Collections.Generic.KeyValuePair<string, string>)(cboOrderNo.SelectedItem)).Value;
                if (poleOrderNo != null && poleOrderNo != "")
                {
                    string url = _poleDynamicServiceUrl + "/" + _plcDynamicLayerId;
                    Query _query = new Query();
                    _query.SpatialRelationship = SpatialRelationship.esriSpatialRelContains;
                    _query.ReturnGeometry = false;
                    // _query.Geometry = e.IdentifyResults[0].Feature.Geometry;
                    _query.OutFields.AddRange(new string[] { "*" });
                    _query.Where = "SKETCH_LOC=" + "'" + sktchLocation + "' AND " + "ORDER_NUMBER=" + "'" + poleOrderNo + "'";
                    QueryTask _circuitqueryTask = new QueryTask(url);
                    _circuitqueryTask.ExecuteCompleted += new EventHandler<QueryEventArgs>(_plcSktchqueryTask_ExecuteCompleted);
                    _circuitqueryTask.Failed += new EventHandler<TaskFailedEventArgs>(_wipqueryTask_Failed);
                    _circuitqueryTask.ExecuteAsync(_query);
                }
                else
                {
                    MessageBox.Show("Order No should not be null");
                }
            }
            catch (Exception e)
            {
                ConfigUtility.StatusBar.Text = "Exception:" + e.Message;
                logger.Error("PLC Validate Sketch Location Exception: " + e.Message);
            }


        }


        void _wipqueryTask_Failed(object sender, TaskFailedEventArgs e)
        {
            logger.Error("PLC Query Task to get pole inside wip failed: " + e.Error.Message);
            ConfigUtility.StatusBar.Text = "Query Task to get pole inside wip failed:" + e.Error.Message;
            PLCBusyIndicator.IsBusy = false;
        }

        void _plcSktchqueryTask_ExecuteCompleted(object sender, QueryEventArgs e)
        {
            try
            {
                if (e.FeatureSet.Features.Count > 0)
                {
                    List<string> existingSktchLocationList = new List<string>();
                    for (int i = 0; i < e.FeatureSet.Features.Count; i++)
                    {
                        if (e.FeatureSet.Features[i].Attributes["PLDBID"] != null)
                        {
                            if (e.FeatureSet.Features[i].Attributes["PLDBID"].ToString() != poleNoTxt.Text)
                            {
                                if (e.FeatureSet.Features[i].Attributes["SKETCH_LOC"] != null)
                                {
                                    existingSktchLocationList.Add(e.FeatureSet.Features[i].Attributes["SKETCH_LOC"].ToString());
                                }
                            }
                        }
                    }
                    if (existingSktchLocationList.Count > 0)
                    {
                        if (existingSktchLocationList.Contains(_sketchLocationValue))
                        {
                            PLCBusyIndicator.IsBusy = false;
                            MessageBox.Show("Please enter unique sketch location");

                        }
                        else
                        {
                            savePole(_sketchLocationValue);
                        }
                    }
                    else
                    {
                        savePole(_sketchLocationValue);
                    }
                }
                else
                {
                    savePole(_sketchLocationValue);
                }
            }
            catch (Exception ex)
            {
                //ConfigUtility.StatusBar.Text = "Exception:" + ex.Message;
                logger.Error("PLC Check Sketch Location Exception: " + ex.Message);
            }
        }
        // Save Pole in GIS
        private void savePole(string SktchLocation)
        {
            try
            {
                string currentUserlanID = WebContext.Current.User.Name.Replace("PGE\\", "").ToUpper();
                PLCBusyIndicator.IsBusy = true;
                sketchLocTxt.Text = SktchLocation;
                _sketchLocation = SktchLocation;

                _orderNumber = ((System.Collections.Generic.KeyValuePair<string, string>)(cboOrderNo.SelectedItem)).Value.ToString();
                _copyPoleNo = copyPoleNoTxt.Text;
                DateTime currentDate = DateTime.Now;

                _creationDate = currentDate.AddHours(-7).ToString();
                creationDateTxt.Text = currentDate.ToString();
                _description = DescTxt.Text;
                _elevation = ElevationTxt.Text;
                _lanID = currentUserlanID;
                lanIdTxt.Text = currentUserlanID;
                _latitude = latTxt.Text;
                _longitude = longTxt.Text;
                _matCode = matCodeTxt.Text;
                _sapEquipId = sapIdTxt.Text;
                _snowLoadDist = snowLoadTxt.Text;
                _poleNumber = poleNoTxt.Text;
                _notificationNo = notificaionNoTxt.Text;
                _isDelete = false;
                _isCopy = false;

                if (_objectIdSelected == null)    //Create Pole scenario
                {
                    _isSelect = false;

                    createPoleFeature();
                }
                else //Existing Pole scenario
                {
                    _plcWidgetObj.IsSelectPoleActive = true;
                    _plcWidgetObj.getElevationValue(_graphic);

                }
            }
            catch (Exception e)
            {

                logger.Error("PLC Save Pole Exception: " + e.Message);
            }

        }
        private void wip_identifyTask_Failed(object sender, TaskFailedEventArgs e)
        {
            logger.Error("PLC Wip identify task failed: " + e.Error.Message);
            ConfigUtility.StatusBar.Text = "Selection of Wip has failed. " + e.Error.Message;
        }

        //Function to validate and prefix LOC_ in SKetch location value
        private string getsketchLocation()
        {
            try
            {
                if (sketchLocTxt.Text.StartsWith("LOC_", StringComparison.OrdinalIgnoreCase) && sketchLocTxt.Text.Length > 4)
                {
                    string valueAfterLoc = sketchLocTxt.Text.Substring(4);
                    bool isValid = IsDigitsOnly(valueAfterLoc);
                    if (isValid == true)
                    {

                        return sketchLocTxt.Text.ToUpper();

                    }
                    else
                    {
                        return null;
                    }

                }
                else
                {
                    bool isValid = IsDigitsOnly(sketchLocTxt.Text);
                    if (isValid == true)
                    {
                        return "LOC_" + sketchLocTxt.Text;
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            catch (Exception ex)
            {

                logger.Error("PLC Query Task to get pole inside wip failed: " + ex.Message);
                return null;
            }
        }

        //Function to validate string contains number only
        bool IsDigitsOnly(string str)
        {
            foreach (char c in str)
            {
                if (c < '0' || c > '9')
                    return false;
            }

            return true;
        }

        //Function to activate Delete Pole
        public void DeleteSelectedPole()
        {
            _isDelete = true;
            createPoleFeature();


        }

        // Pole Popup OPen Ocalc button click function
        private void OpenOcalc_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (poleNoTxt.Text != null && poleNoTxt.Text != "")
                {
                    //DA# 200103 ME Q1 2020 - START
                    if (_plcWidgetObj.IsPLCV6OCalc == true)    //O-Calc V6
                    {
                        //check for WIP polygons at the selected Pole
                        try
                        {
                            _ocalcV6PMOrderNotificNoList = new Dictionary<string, string>();
                            var identifyParametersWIP = new IdentifyParameters();
                            identifyParametersWIP.Geometry = _selectedPoleGeom;
                            identifyParametersWIP.LayerIds.Add(0);
                            identifyParametersWIP.MapExtent = MapProperty.Extent;
                            identifyParametersWIP.LayerOption = LayerOption.all;
                            identifyParametersWIP.Height = (int)MapProperty.ActualHeight;
                            identifyParametersWIP.Width = (int)MapProperty.ActualWidth;
                            identifyParametersWIP.SpatialReference = MapProperty.SpatialReference;
                            identifyParametersWIP.Tolerance = 2;

                            var identifyTaskWIP = new IdentifyTask(_plcWIPServiceUrl);
                            identifyTaskWIP.Failed += new EventHandler<TaskFailedEventArgs>(OCalcWip_identifyTask_Failed);
                            identifyTaskWIP.ExecuteCompleted += new EventHandler<IdentifyEventArgs>(OCalcWip_identifyTask_ExecuteCompleted);

                            identifyTaskWIP.ExecuteAsync(identifyParametersWIP);
                        }
                        catch (Exception ex)
                        {

                            logger.Error("Identify on WIP Service Failed for O-Calc:" + ex.Message);
                        }
                    }
                    else   //O-Calc V5
                    {
                        long poleNo = Convert.ToInt64(poleNoTxt.Text);
                        HtmlPage.Window.Navigate(new Uri(("PLDB://" + poleNo)), "_blank");
                    }

                }
                else
                {
                    MessageBox.Show("PLDBID is Null");
                }
            }
            catch (Exception ex)
            {
                logger.Error("PLC open Ocalc Exception: " + ex.Message);
            }
        }

        //DA# 200103 ME Q1 2020 - START
        private void OCalcWip_identifyTask_ExecuteCompleted(object sender, IdentifyEventArgs e)
        {
            ConfigUtility.StatusBar.Text = string.Empty;
            try
            {
                if (e.IdentifyResults != null)
                {
                    if (e.IdentifyResults.Count > 0)
                    {
                        if (e.IdentifyResults[0].LayerName.Contains("WIP"))
                        {
                            for (int i = 0; i < e.IdentifyResults.Count; i++)
                            {
                                if (e.IdentifyResults[i].Feature.Attributes["PM Order Number"] != null)
                                {
                                    if (!(_ocalcV6PMOrderNotificNoList.Keys.Contains(Convert.ToString(e.IdentifyResults[i].Feature.Attributes["PM Order Number"]))))
                                    {
                                        if (e.IdentifyResults[i].Feature.Attributes["NOTIFICATION_NUMBER"] != null && Convert.ToString(e.IdentifyResults[i].Feature.Attributes["NOTIFICATION_NUMBER"]).ToUpper() != "NULL")
                                        {
                                            _ocalcV6PMOrderNotificNoList.Add(Convert.ToString(e.IdentifyResults[i].Feature.Attributes["PM Order Number"]), Convert.ToString(e.IdentifyResults[i].Feature.Attributes["NOTIFICATION_NUMBER"]));
                                        }
                                        else
                                        {
                                            _ocalcV6PMOrderNotificNoList.Add(Convert.ToString(e.IdentifyResults[i].Feature.Attributes["PM Order Number"]), "");
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                RedirectOCalcV6URI();
            }
            catch (Exception err)
            {
                logger.Error("O-Calc V6 WIP service issue" + err.Message);
            }
        }

        public void OCalcWip_identifyTask_Failed(object sender, TaskFailedEventArgs e)
        {
            logger.Error("Identify on WIP Service Failed for O-Calc: " + e.Error);
            ConfigUtility.StatusBar.Text = "Identify on WIP Service Failed for O-calc. " + e.Error.Message;
        }

        private void RedirectOCalcV6URI()
        {
            long poleNo = Convert.ToInt64(poleNoTxt.Text);
            if (_ocalcV6PMOrderNotificNoList.Count == 0)     //no WIP at selected Pole
            {
                HtmlPage.Window.Navigate(new Uri(("PLDB://LD/LaunchByPLDBID/" + poleNo + "?")), "_blank");
            }

            else if (_ocalcV6PMOrderNotificNoList.Count == 1)     //one WIP is present at selcted pole
            {
                string confirmationMsg = "You are about to open Line Design in O-Calc for\nPDLBID " + poleNo + " and PM Order " + Convert.ToString(_ocalcV6PMOrderNotificNoList.Keys.ToList()[0]) + ".\nDo you want to proceed?";
                MessageBoxResult messageResult = MessageBox.Show(confirmationMsg, "Confirmation", MessageBoxButton.OKCancel);
                if (messageResult == MessageBoxResult.OK)
                {
                    if (Convert.ToString(_ocalcV6PMOrderNotificNoList.ToList()[0].Value) != "")
                        HtmlPage.Window.Navigate(new Uri(("PLDB://LD/LaunchByPLDBID/" + poleNo + "?PMOrderNumber=" + Convert.ToString(_ocalcV6PMOrderNotificNoList.ToList()[0].Key) + "&NotificationNumber=" + Convert.ToString(_ocalcV6PMOrderNotificNoList.ToList()[0].Value))), "_blank");
                    else
                        HtmlPage.Window.Navigate(new Uri(("PLDB://LD/LaunchByPLDBID/" + poleNo + "?PMOrderNumber=" + Convert.ToString(_ocalcV6PMOrderNotificNoList.ToList()[0].Key))), "_blank");
                }
            }

            else if (_ocalcV6PMOrderNotificNoList.Count > 1)     //more than one WIPs are present at selcted pole
            {
                _plcWidgetObj.OpenPMOrderSelectionWindow(Convert.ToString(poleNoTxt.Text), _ocalcV6PMOrderNotificNoList);
            }
        }
        //DA# 200103 ME Q1 2020 - END

        // Pole Popup Copy button click function
        private void CopyButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (this.CopyButton.Content.ToString() == "Copy")
                {

                    _isCopy = true;
                    this.Close();

                    _plcWidgetObj.IsSelectPoleActive = true;
                    this.CopyButton.Content = "Cancel Copy";
                }
                else
                {
                    _isCopy = false;
                    _plcWidgetObj.IsSelectPoleActive = false;
                    this.CopyButton.Content = "Copy";
                    _copyPoleNo = null;
                    copyPoleNoTxt.Text = "";
                    _plcWidgetObj.IsNewPoleActive = true;

                }
            }
            catch (Exception ex)
            {
                logger.Error("PLC Copy Button Exception: " + ex.Message);
            }

        }
        // Pole Popup close button click function
        private void Close_click(object sender, System.ComponentModel.CancelEventArgs e)
        {
            try
            {
                Application.Current.RootVisual.SetValue(Control.IsEnabledProperty, true);
                if (!_isCopy)
                {
                    _objectIdSelected = null;
                    _copyPoleNo = null;
                    copyPoleNoTxt.Text = "";
                    GraphicsLayer layer = MapProperty.Layers[poleMarkerID] as GraphicsLayer;
                    if (layer != null)
                    {
                        if (layer.Graphics.Count > 0)
                        {
                            layer.Graphics.Clear();
                        }
                    }
                }
                else
                {
                    if (this.CopyButton.Content.ToString() == "Cancel Copy")
                    {
                        _isCopy = false;
                        _plcWidgetObj.IsSelectPoleActive = false;
                        this.CopyButton.Content = "Copy";
                        _copyPoleNo = null;
                        copyPoleNoTxt.Text = "";
                        _plcWidgetObj.IsNewPoleActive = true;
                        _objectIdSelected = null;
                        GraphicsLayer layer = MapProperty.Layers[poleMarkerID] as GraphicsLayer;
                        if (layer != null)
                        {
                            if (layer.Graphics.Count > 0)
                            {
                                layer.Graphics.Clear();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error("PLC Pole Popup close Exception: " + ex.Message);
            }

        }

        // Function to create feature in PLD_INFO feature class(Pole Feature)
        public void createPoleFeature()
        {

            try
            {
                _poleLayer = MapProperty.Layers[_poleLayerName] as FeatureLayer;

                if (_poleLayer != null)
                {
                    if (_objectIdSelected == null)    //Create Pole
                    {
                        _isNewPole = true;
                        _ownerLanId = WebContext.Current.User.Name.Replace("PGE\\", "").ToUpper();
                        _isSelect = false;
                        _graphic.Attributes["ORDER_NUMBER"] = _orderNumber;
                        _graphic.Attributes["SAPEQUIPID"] = _sapEquipId;
                        _graphic.Attributes["OVERALL_SF"] = Convert.ToDouble("0");
                        if (_notificationNo != "" && _notificationNo != null)
                        {
                            _graphic.Attributes["NOTIFICATION_NUMBER"] = _notificationNo;
                        }
                        else
                        {
                            _graphic.Attributes["NOTIFICATION_NUMBER"] = null;
                        }
                        if (_poleNumber.ToString() != "" && _poleNumber != null)
                        {
                            _graphic.Attributes["PLDBID"] = Convert.ToDouble(_poleNumber);
                        }
                        else
                        {
                            _graphic.Attributes["PLDBID"] = null;
                        }
                        if (_sketchLocation != "" && _sketchLocation != null)
                        {
                            _graphic.Attributes["SKETCH_LOC"] = _sketchLocation;
                        }
                        else
                        {
                            _graphic.Attributes["SKETCH_LOC"] = null;
                        }
                        if (_copyPoleNo != "" && _copyPoleNo != null)
                        {
                            _graphic.Attributes["COPY_PLD"] = Convert.ToDouble(_copyPoleNo);
                        }
                        else
                        {
                            _graphic.Attributes["COPY_PLD"] = null;
                        }
                        if (_longitude != "" && _longitude != null)
                        {
                            _graphic.Attributes["LONGITUDE"] = Convert.ToDouble(_longitude);
                        }
                        else
                        {
                            _graphic.Attributes["LONGITUDE"] = null;
                        }
                        if (_latitude != "" && _latitude != null)
                        {
                            _graphic.Attributes["LAT"] = Convert.ToDouble(_latitude);
                        }
                        else
                        {
                            _graphic.Attributes["LAT"] = null;
                        }
                        _graphic.Attributes["SNOW_LOAD_DIST"] = _snowLoadDist;
                        if (_matCode != "" && _matCode != null)
                        {
                            _graphic.Attributes["MAINTENANCECODE"] = _matCode;

                        }
                        else
                        {
                            _graphic.Attributes["MAINTENANCECODE"] = null;
                        }
                        _graphic.Attributes["ORDER_DESCRIPTION"] = _description;
                        _graphic.Attributes["LANID"] = _lanID;
                        if (_creationDate != "" && _creationDate != null)
                        {
                            _graphic.Attributes["CREATED_DATE"] = Convert.ToDateTime(_creationDate);
                        }
                        else
                        {
                            _graphic.Attributes["CREATED_DATE"] = null;
                        }
                        if (_elevation != "" && _elevation != null)
                        {
                            _graphic.Attributes["ELEVATION"] = Convert.ToDouble(_elevation);
                        }
                        else
                        {
                            _graphic.Attributes["ELEVATION"] = null;
                        }
                        _poleLayer.Graphics.Add(_graphic);
                    }
                    else     //Edit / Delete Pole
                    {

                        this.CopyButton.IsEnabled = false;

                        if (!_isDelete)
                        {
                            if (_isNewPole == false)
                            {
                                _isSelect = true;
                                _elevation = ElevationTxt.Text;
                                _snowLoadDist = snowLoadTxt.Text;

                                callPLDStartAnalysis();
                            }
                            else
                            {
                                updateFeatureExistingPole();
                            }


                        }
                        if (_isDelete)
                        {
                            var abc = _poleLayer.Where(g => g.Attributes["OBJECTID"].ToString() == _objectIdSelected.ToString());
                            _graphic =
                                _poleLayer.Graphics.Where(
                                    g => g.Attributes["OBJECTID"].ToString() == _objectIdSelected.ToString()).First();

                            if (_graphic.Attributes["PLDBID"] != null && _graphic.Attributes["ORDER_NUMBER"] != null)
                            {
                                callPoleRemovedFromOrder(_graphic.Attributes["PLDBID"].ToString(), _graphic.Attributes["ORDER_NUMBER"].ToString());
                            }
                            else
                            {
                                if (_graphic.Attributes["PLDBID"] == null)
                                {

                                    MessageBox.Show("PLDBID should not be null");
                                }
                                else if (_graphic.Attributes["ORDER_NUMBER"] == null)
                                {
                                    MessageBox.Show("ORDER_NUMBER should not be null");
                                }
                            }

                        }
                        else
                        {

                        }

                    }
                }
                if (_graphic == null)
                {
                    RefreshPoleLayer();
                    if (_isDelete)
                        ConfigUtility.StatusBar.Text = "Failed to delete Pole.";
                    PLCBusyIndicator.IsBusy = false;

                    ConfigUtility.StatusBar.Text = "Failed to save Pole.";
                }

                else if (_objectIdSelected == null)
                {
                    _poleLayer.EndSaveEdits += new EventHandler<EndEditEventArgs>(Polelayer_EndSaveEdits);
                    _poleLayer.SaveEditsFailed += new EventHandler<TaskFailedEventArgs>(LayerOnSaveEditsFailed);
                    if (_poleLayer.ValidateEdits) _poleLayer.SaveEdits();

                }
            }
            catch (Exception ex)
            {
                logger.Error("PLC Create Feature Exception: " + ex.Message);
            }

        }



        private void Polelayer_EndSaveEdits(object sender, EndEditEventArgs e)
        {
            try
            {
                if (_isDelete)
                {
                    RefreshPoleLayer();
                    if (_plcWidgetObj.IsDeletePoleActive)
                    {
                        ConfigUtility.StatusBar.Text = "Pole deleted successfully.";
                        _objectIdSelected = null;
                    }
                    else
                    {
                        this.Close();
                        ConfigUtility.StatusBar.Text = "";
                    }
                    GraphicsLayer layer = MapProperty.Layers[poleMarkerID] as GraphicsLayer;
                    if (layer != null)
                    {
                        if (layer.Graphics.Count > 0)
                        {
                            layer.Graphics.Clear();
                        }
                    }
                }
                else
                {

                    RefreshPoleLayer();
                    if (_isNewPole == true && _objectIdSelected == null)
                    {
                        getNewPoleObjId();

                        _isNewPole = false;
                        this.CopyButton.IsEnabled = false;
                    }


                    if (_isSelect == true)
                    {
                        PLCBusyIndicator.IsBusy = false;
                        _isSelect = false;
                        ConfigUtility.StatusBar.Text = "Pole saved successfully.";
                    }
                    if (this.CopyButton.Content.ToString() == "Cancel Copy")
                    {

                        this.CopyButton.Content = "Copy";

                    }

                }

                _poleLayer.EndSaveEdits -= new EventHandler<EndEditEventArgs>(Polelayer_EndSaveEdits);
                _poleLayer.SaveEditsFailed -= new EventHandler<TaskFailedEventArgs>(LayerOnSaveEditsFailed);
            }
            catch (Exception ex)
            {
                logger.Error("PLC Pole Save End Exception: " + ex.Message);
            }
        }

        private void LayerOnSaveEditsFailed(object sender, TaskFailedEventArgs taskFailedEventArgs)
        {
            RefreshPoleLayer();
            if (_isDelete)
            {
                PLCBusyIndicator.IsBusy = false;
                logger.Error("Failed to delete Pole: " + taskFailedEventArgs.Error);
                ConfigUtility.StatusBar.Text = "Delete Pole request to server has failed. " + taskFailedEventArgs.Error.Message;
            }
            else
            {
                PLCBusyIndicator.IsBusy = false;
                logger.Error("Failed to save Pole: " + taskFailedEventArgs.Error);
                ConfigUtility.StatusBar.Text = "Save Pole request to server has failed. " + taskFailedEventArgs.Error.Message;
            }
            _poleLayer.EndSaveEdits -= new EventHandler<EndEditEventArgs>(Polelayer_EndSaveEdits);
            _poleLayer.SaveEditsFailed -= new EventHandler<TaskFailedEventArgs>(LayerOnSaveEditsFailed);
        }

        //Update Pole Feature
        private void updateFeatureExistingPole()
        {
            try
            {
                ESRI.ArcGIS.Client.Geometry.Geometry selectedPoleGeo = _graphic.Geometry;
                this.CopyButton.IsEnabled = false;


                var abc = _poleLayer.Where(g => g.Attributes["OBJECTID"].ToString() == _objectIdSelected.ToString());
                _graphic =
                    _poleLayer.Graphics.Where(
                        g => g.Attributes["OBJECTID"].ToString() == _objectIdSelected.ToString()).First();



                if (!_isDelete)
                {
                    _isSelect = true;
                    _graphic.Attributes["ORDER_NUMBER"] = _orderNumber;
                    _graphic.Attributes["SAPEQUIPID"] = _sapEquipId;
                    if (_poleNumber.ToString() != "" && _poleNumber != null)
                    {
                        _graphic.Attributes["PLDBID"] = Convert.ToDouble(_poleNumber);

                    }
                    else
                    {
                        _graphic.Attributes["PLDBID"] = null;
                    }
                    if (_sketchLocation != "" && _sketchLocation != null)
                    {
                        _graphic.Attributes["SKETCH_LOC"] = _sketchLocation;
                    }
                    else
                    {
                        _graphic.Attributes["SKETCH_LOC"] = null;
                    }
                    if (_notificationNo != "" && _notificationNo != null)
                    {
                        _graphic.Attributes["NOTIFICATION_NUMBER"] = _notificationNo;
                    }
                    else
                    {
                        _graphic.Attributes["NOTIFICATION_NUMBER"] = null;
                    }
                    if (_copyPoleNo != "" && _copyPoleNo != null)
                    {
                        _graphic.Attributes["COPY_PLD"] = Convert.ToDouble(_copyPoleNo);
                    }
                    else
                    {
                        _graphic.Attributes["COPY_PLD"] = null;
                    }
                    if (_longitude != "" && _longitude != null)
                    {
                        _graphic.Attributes["LONGITUDE"] = Convert.ToDouble(_longitude);
                    }
                    else
                    {
                        _graphic.Attributes["LONGITUDE"] = null;
                    }
                    if (_latitude != "" && _latitude != null)
                    {
                        _graphic.Attributes["LAT"] = Convert.ToDouble(_latitude);
                    }
                    else
                    {
                        _graphic.Attributes["LAT"] = null;
                    }
                    _graphic.Attributes["SNOW_LOAD_DIST"] = _snowLoadDist;
                    if (_matCode != "" && _matCode != null)
                    {
                        _graphic.Attributes["MAINTENANCECODE"] = _matCode;

                    }
                    else
                    {
                        _graphic.Attributes["MAINTENANCECODE"] = null;
                    }
                    _graphic.Attributes["ORDER_DESCRIPTION"] = _description;
                    _graphic.Attributes["LANID"] = _lanID;
                    if (_creationDate != "" && _creationDate != null)
                    {
                        _graphic.Attributes["CREATED_DATE"] = Convert.ToDateTime(_creationDate);
                    }
                    else
                    {
                        _graphic.Attributes["CREATED_DATE"] = null;
                    }
                    if (_elevation != "" && _elevation != null)
                    {
                        _graphic.Attributes["ELEVATION"] = Convert.ToDouble(_elevation);
                    }
                    else
                    {
                        _graphic.Attributes["ELEVATION"] = null;
                    }
                    _graphic.Geometry = selectedPoleGeo;
                }
                if (_graphic == null)
                {
                    RefreshPoleLayer();
                    if (_isDelete)
                        ConfigUtility.StatusBar.Text = "Failed to delete Pole.";
                    PLCBusyIndicator.IsBusy = false;
                    //  else
                    ConfigUtility.StatusBar.Text = "Failed to save Pole.";
                }
                else
                {
                    _poleLayer.EndSaveEdits += new EventHandler<EndEditEventArgs>(Polelayer_EndSaveEdits);
                    _poleLayer.SaveEditsFailed += new EventHandler<TaskFailedEventArgs>(LayerOnSaveEditsFailed);
                    if (_poleLayer.ValidateEdits) _poleLayer.SaveEdits();

                }
            }
            catch (Exception ex)
            {
                logger.Error("PLC Update Pole Record: " + ex.Message);
            }
        }

        // Function to get new pole feature object id
        private void getNewPoleObjId()
        {
            string url = _poleDynamicServiceUrl + "/" + _plcDynamicLayerId;

            Query _query = new Query();
            _query.SpatialRelationship = SpatialRelationship.esriSpatialRelIntersects;
            _query.ReturnGeometry = false;
            _query.Geometry = _graphic.Geometry;
            _query.OutFields.AddRange(new string[] { "*" });

            QueryTask _circuitqueryTask = new QueryTask(url);
            _circuitqueryTask.ExecuteCompleted += new EventHandler<QueryEventArgs>(_plcNewPoleObjqueryTask_ExecuteCompleted);
            _circuitqueryTask.Failed += new EventHandler<TaskFailedEventArgs>(_plcqueryTask_Failed);
            _circuitqueryTask.ExecuteAsync(_query);
        }

        void _plcNewPoleObjqueryTask_ExecuteCompleted(object sender, QueryEventArgs e)
        {
            try
            {
                if (e.FeatureSet.Features.Count > 0)
                {
                    ConfigUtility.StatusBar.Text = "";
                    var feature = e.FeatureSet.Features[0];

                    _newPoleObjectId = feature.Attributes["OBJECTID"].ToString();
                    callPLDStartAnalysis();


                }
                else
                {
                    PLCBusyIndicator.IsBusy = false;
                    ConfigUtility.StatusBar.Text = "Pole Feature not found.";
                }
            }
            catch (Exception ex)
            {
                PLCBusyIndicator.IsBusy = false;
                ConfigUtility.StatusBar.Text = "Exception:" + ex.Message;
                logger.Error("PLC New Pole ObjectID query Exception: " + ex.Message);
            }
        }
        // Get Pole Object Id on the basis of PLDBID
        private void getPoleObjectId(string pldbid)
        {

            string url = _poleDynamicServiceUrl + "/" + _plcDynamicLayerId;

            Query _query = new Query();
            _query.SpatialRelationship = SpatialRelationship.esriSpatialRelIntersects;
            _query.ReturnGeometry = false;

            _query.OutFields.AddRange(new string[] { "*" });
            _query.Where = "PLDBID=" + Convert.ToInt32(pldbid);
            QueryTask _circuitqueryTask = new QueryTask(url);
            _circuitqueryTask.ExecuteCompleted += new EventHandler<QueryEventArgs>(_plcqueryTask_ExecuteCompleted);
            _circuitqueryTask.Failed += new EventHandler<TaskFailedEventArgs>(_plcqueryTask_Failed);
            _circuitqueryTask.ExecuteAsync(_query);
        }

        void _plcqueryTask_Failed(object sender, TaskFailedEventArgs e)
        {
            logger.Error("PLC Query Task to get pole object id failed: " + e.Error.Message);


        }

        void _plcqueryTask_ExecuteCompleted(object sender, QueryEventArgs e)
        {
            try
            {
                if (e.FeatureSet.Features.Count > 0)
                {
                    ConfigUtility.StatusBar.Text = "";
                    var feature = e.FeatureSet.Features[0];
                    _objectIdSelected = feature.Attributes["OBJECTID"].ToString();
                    updateFeatureExistingPole();

                }
                else
                {
                    PLCBusyIndicator.IsBusy = false;
                    ConfigUtility.StatusBar.Text = "Pole Feature not found.";
                }
            }
            catch (Exception ex)
            {
                logger.Error("PLC Query Task Exception: " + ex.Message);
            }
        }

        //Function to call PoleRemovedFromOrder endpoint to delete pole from PLD DB
        private void callPoleRemovedFromOrder(string polePldbid, string poleOrderNo)
        {
            try
            {
                var items = new List<KeyValuePair<string, string>>();


                Guid trackingID = Guid.NewGuid();


                var pgeHeader = new List<KeyValuePair<string, string>>();
                pgeHeader.Add(new KeyValuePair<string, string>("Source", "GIS"));
                pgeHeader.Add(new KeyValuePair<string, string>("TrackingID", trackingID.ToString()));
                pgeHeader.Add(new KeyValuePair<string, string>("Timestamp", DateTime.Now.ToString()));
                var pgeItems = new List<KeyValuePair<string, ulong>>();

                pgeItems.Add(new KeyValuePair<string, ulong>("PLDBID", Convert.ToUInt64(polePldbid)));
                pgeItems.Add(new KeyValuePair<string, ulong>("PGE_OrderNumber", Convert.ToUInt64(poleOrderNo)));

                string jsonPgeHeader = dictionaryToJson(pgeHeader);
                string jsonPgeDataBlock = deletePoleDataToJson(pgeItems);

                items.Add(new KeyValuePair<string, string>("Header", jsonPgeHeader));
                items.Add(new KeyValuePair<string, string>("RemovePoleData", jsonPgeDataBlock));
                string finaljson = dictionaryToJson(items);


                Uri uri = new Uri(_removePoleEIServiceUrl);
                WebClient cnt = new WebClient();
                cnt.UploadStringCompleted += new UploadStringCompletedEventHandler(c_PoleRemoveUploadStringCompleted);
                cnt.Headers["Content-type"] = "application/json";
                cnt.Encoding = Encoding.UTF8;
                cnt.Headers[HttpRequestHeader.Authorization] = "Basic " + _authorizationHeader;
                cnt.UploadStringAsync(uri, "POST", finaljson);
            }
            catch (Exception ex)
            {
                logger.Error("PLC Call PoleRemovedFrom Order Exception: " + ex.Message);
            }

        }
        // Response of PoleRemovedFromOrder Endpoint
        private void c_PoleRemoveUploadStringCompleted(object sender, UploadStringCompletedEventArgs e)
        {
            try
            {
                if (e.Result == "110")
                {
                    _poleLayer.Graphics.Remove(_graphic);
                    if (_graphic == null)
                    {
                        RefreshPoleLayer();
                        if (_isDelete)
                            ConfigUtility.StatusBar.Text = "Failed to delete Pole.";
                        PLCBusyIndicator.IsBusy = false;

                        ConfigUtility.StatusBar.Text = "Failed to save Pole.";
                    }
                    else if (_isDelete == true || _objectIdSelected == null)
                    {
                        _poleLayer.EndSaveEdits += new EventHandler<EndEditEventArgs>(Polelayer_EndSaveEdits);
                        _poleLayer.SaveEditsFailed += new EventHandler<TaskFailedEventArgs>(LayerOnSaveEditsFailed);
                        if (_poleLayer.ValidateEdits) _poleLayer.SaveEdits();

                    }
                }
                else
                {
                    StartAnalysisResponse result = JsonHelper.Deserialize<StartAnalysisResponse>(e.Result);
                    if (result.ErrorCode != null && result.StatusCode != null)
                    {

                        MessageBox.Show("Unable to Delete Pole in PLD." + Environment.NewLine + "Error Code: " + result.ErrorCode + Environment.NewLine + "Status Code: " + result.StatusCode);


                    }
                    else
                    {

                        MessageBox.Show("Unable to Delete Pole in PLD");

                    }

                }
            }
            catch (Exception ex)
            {
                logger.Error("PLC Delete pole EI service Exception: " + ex.Message);
            }
        }
        // Function to call Start Analysis EndPoint of EI Webservice and get PLDBID in return

        private void callPLDStartAnalysis()
        {

            try
            {
                Dictionary<string, string> plcAttrDictionary = new Dictionary<string, string>();
                var items = new List<KeyValuePair<string, string>>();

                var pgeItems = new List<KeyValuePair<string, string>>();
                PoleData poleDataObj = new PoleData();
                Dictionary<string, string> pge_DataDictionary = new Dictionary<string, string>();
                IList<string> pldFieldArray = _pgeDataFields.Split(',').ToList();
                if (_objectIdSelected == null && (_poleNumber == "" || _poleNumber == null))
                {

                    poleDataObj.PLDBID = 0;
                }
                else
                {
                    if (_poleNumber != "")
                    {
                        poleDataObj.PLDBID = Convert.ToUInt64(_poleNumber);
                    }

                }
                for (int i = 0; i < pldFieldArray.Count; i++)
                {
                    if (pldFieldArray[i] == "Latitude" && _latitude != "")
                    {
                        poleDataObj.Latitude = Convert.ToDouble(_latitude);

                    }
                    else if (pldFieldArray[i] == "Longitude" && _longitude != "")
                    {
                        poleDataObj.Longitude = Convert.ToDouble(_longitude);
                    }
                    else if (pldFieldArray[i] == "Elevation" && _elevation != "")
                    {
                        poleDataObj.Elevation = Convert.ToDouble(_elevation);
                    }
                    else if (pldFieldArray[i] == "PGE_SAPEQUIPID" && _sapEquipId != "")
                    {
                        poleDataObj.PGE_SAPEQUIPID = Convert.ToUInt64(_sapEquipId);

                    }
                    else if (pldFieldArray[i] == "PGE_OBJECTID")
                    {
                        if (_objectIdSelected == null && _newPoleObjectId != null && _newPoleObjectId != "" && (_poleNumber == "" || _poleNumber == null))
                        {
                            poleDataObj.PGE_OBJECTID = Convert.ToInt64(_newPoleObjectId);


                        }
                        else
                        {
                            poleDataObj.PGE_OBJECTID = Convert.ToInt64(_objectIdSelected);


                        }
                    }
                    else if (pldFieldArray[i] == "PGE_MatCode")
                    {
                        poleDataObj.PGE_MatCode = _matCode.ToString();

                    }
                    else if (pldFieldArray[i] == "PGE_OrderNumber" && _orderNumber != "")
                    {
                        poleDataObj.PGE_OrderNumber = Convert.ToUInt64(_orderNumber);

                    }
                    else if (pldFieldArray[i] == "PGE_SketchLocation")
                    {
                        poleDataObj.PGE_SketchLocation = _sketchLocation.ToString();

                    }
                    else if (pldFieldArray[i] == "PGE_CopiedPLDBID" && _copyPoleNo != "")
                    {
                        poleDataObj.PGE_CopiedPLDBID = Convert.ToUInt64(_copyPoleNo);

                    }
                    else if (pldFieldArray[i] == "PGE_LANID")
                    {
                        poleDataObj.PGE_LANID = _lanID.ToString();

                    }
                    else if (pldFieldArray[i] == "PGE_SnowLoadDistrict")
                    {
                        poleDataObj.PGE_SnowLoadDistrict = _snowLoadDist.ToString();
                    }
                    else if (pldFieldArray[i] == "PGE_Description")
                    {
                        poleDataObj.PGE_Description = _description.ToString();

                    }
                    else if (pldFieldArray[i] == "PGE_GLOBALID")
                    {
                        poleDataObj.PGE_GLOBALID = "00000000-0000-0000-0000-000000000000";
                    }
                    else if (pldFieldArray[i] == "PGE_NotificationNumber")
                    {
                        if (_notificationNo.ToString() != "Null" && _notificationNo != "")
                        {
                            poleDataObj.PGE_NotificationNumber = Convert.ToUInt64(_notificationNo);
                        }
                    }
                    else
                    {


                    }
                }
                string jsonPgeDataBlock = JsonHelperSerialiser.JsonSerialize(poleDataObj);


                Guid trackingID = Guid.NewGuid();


                var pgeHeader = new List<KeyValuePair<string, string>>();
                pgeHeader.Add(new KeyValuePair<string, string>("Source", "GIS"));
                pgeHeader.Add(new KeyValuePair<string, string>("TrackingID", trackingID.ToString()));
                pgeHeader.Add(new KeyValuePair<string, string>("Timestamp", DateTime.Now.ToString()));

                string jsonPgeHeader = dictionaryToJson(pgeHeader);
                items.Add(new KeyValuePair<string, string>("Header", jsonPgeHeader));
                items.Add(new KeyValuePair<string, string>("PoleData", jsonPgeDataBlock));
                string finaljson = dictionaryToJson(items);

                // Web Service call to EI Start Analysis
                Uri uri = new Uri(_saveEIServiceUrl);
                WebClient cnt = new WebClient();
                cnt.UploadStringCompleted += new UploadStringCompletedEventHandler(c_UploadStringCompleted);
                cnt.Headers["Content-type"] = "application/json";
                cnt.Encoding = Encoding.UTF8;
                cnt.Headers[HttpRequestHeader.Authorization] = "Basic " + _authorizationHeader;
                cnt.UploadStringAsync(uri, "POST", finaljson);

            }
            catch (Exception ex)
            {
                logger.Error("PLC Call StartAnalysis Exception: " + ex.Message);
            }

        }


        // function to convert dictionary to json
        string deletePoleDataToJson(List<KeyValuePair<string, ulong>> dict)
        {
            if (dict[0].Key == "Header")
            {
                var entries = dict.Select(d =>
                      string.Format("\"{0}\": {1}", d.Key, string.Join(",", d.Value)));
                return "{" + string.Join(",", entries) + "}";

            }
            else
            {
                var entries = dict.Select(d =>
                    string.Format("\"{0}\": {1}", d.Key, string.Join(",", d.Value)));
                return "{" + string.Join(",", entries) + "}";
            }
        }

        // function to convert dictionary to json
        string dictionaryToJson(List<KeyValuePair<string, string>> dict)
        {
            if (dict[0].Key == "Header")
            {
                var entries = dict.Select(d =>
                      string.Format("\"{0}\": {1}", d.Key, string.Join(",", d.Value)));
                return "{" + string.Join(",", entries) + "}";

            }
            else
            {
                var entries = dict.Select(d =>
                    string.Format("\"{0}\": \"{1}\"", d.Key, string.Join(",", d.Value)));
                return "{" + string.Join(",", entries) + "}";
            }
        }

        //Function to get response from Start Analysis Endpoint of EI webservice
        private void c_UploadStringCompleted(object sender, UploadStringCompletedEventArgs e)
        {
            StartAnalysisResponse result = null;
            try
            {

                result = JsonHelper.Deserialize<StartAnalysisResponse>(e.Result);


                ulong returnedPLDID = result.PLDBID;
                ConfigUtility.StatusBar.Text = "";

                if (returnedPLDID != null && returnedPLDID != 0)
                {
                    ConfigUtility.StatusBar.Text = "";

                    if (returnedPLDID.ToString() != _poleNumber.ToString())
                    {
                        if (_isSelect == false)
                        {
                            poleNoTxt.Text = returnedPLDID.ToString();
                            _poleNumber = returnedPLDID;
                            if (_newPoleObjectId != null && _newPoleObjectId != "")
                            {
                                _objectIdSelected = _newPoleObjectId;
                                _poleLayer.UpdateCompleted += new EventHandler(_poleLayer_UpdateCompleted);
                                _poleLayer.UpdateFailed += new EventHandler<ESRI.ArcGIS.Client.Tasks.TaskFailedEventArgs>(_poleLayer_UpdateFailed);
                                _poleLayer.Update();

                            }


                        }
                        else
                        {

                            PLCBusyIndicator.IsBusy = false;
                        }
                    }
                    else if (Convert.ToDouble(returnedPLDID) == Convert.ToDouble(_poleNumber))
                    {
                        if (_objectIdSelected != null)
                        {
                            updateFeatureExistingPole();
                        }
                        else
                        {

                            getPoleObjectId(returnedPLDID.ToString());
                        }
                    }
                }
                else
                {
                    if (result.ErrorCode != null && result.StatusCode != null)
                    {
                        if (_isSelect == false)
                        {
                            _objectIdSelected = _newPoleObjectId;
                            //MessageBox.Show("Unable to Save Pole in PLD." + Environment.NewLine + "Error Code: " + result.ErrorCode + Environment.NewLine + "Status Code: " + result.StatusCode);
                            MessageBox.Show("StartAnalysis Error:" + result.DeveloperMessage);
                            DeletePoleInException();

                            PLCBusyIndicator.IsBusy = false;
                        }
                        else
                        {
                            MessageBox.Show("StartAnalysis Error:" + result.DeveloperMessage);
                            PLCBusyIndicator.IsBusy = false;
                        }
                    }
                    else
                    {
                        _objectIdSelected = _newPoleObjectId;
                        MessageBox.Show("Unable to Save Pole in PLD");
                        DeletePoleInException();

                        PLCBusyIndicator.IsBusy = false;
                    }
                }

            }
            catch (Exception ex)
            {
                if (_isSelect == false)
                {
                    _objectIdSelected = _newPoleObjectId;
                    MessageBox.Show("StartAnalysis Error: PLDBID not returned from start analysis");
                    DeletePoleInException();

                    PLCBusyIndicator.IsBusy = false;
                }
                else
                {
                    MessageBox.Show("StartAnalysis Error: Unable to Save Pole in PLD");

                    PLCBusyIndicator.IsBusy = false;

                }
            }
        }


        void _poleLayer_UpdateCompleted(object sender, EventArgs e)
        {
            var _polePLCLayer = sender as FeatureLayer;
            _polePLCLayer.UpdateCompleted -= new EventHandler(_poleLayer_UpdateCompleted);
            _polePLCLayer.UpdateFailed -= new EventHandler<ESRI.ArcGIS.Client.Tasks.TaskFailedEventArgs>(_poleLayer_UpdateFailed);
            updateFeatureExistingPole();
        }

        void _poleLayer_UpdateFailed(object sender, ESRI.ArcGIS.Client.Tasks.TaskFailedEventArgs e)
        {
            logger.Error("Pole Feature Layer update failed!");
            var _polePLCLayer = sender as FeatureLayer;
            _polePLCLayer.UpdateCompleted -= new EventHandler(_poleLayer_UpdateCompleted);
            _polePLCLayer.UpdateFailed -= new EventHandler<ESRI.ArcGIS.Client.Tasks.TaskFailedEventArgs>(_poleLayer_UpdateFailed);
        }

        //Function to delete pole in case of exception occured
        private void DeletePoleInException()
        {
            try
            {
                _graphic = _poleLayer.Graphics.Where(g => g.Attributes["OBJECTID"].ToString() == _objectIdSelected.ToString()).First();
                _poleLayer.Graphics.Remove(_graphic);
                if (_graphic == null)
                {
                    RefreshPoleLayer();
                    if (_isDelete)
                        ConfigUtility.StatusBar.Text = "Failed to delete Pole.";
                    PLCBusyIndicator.IsBusy = false;
                    //  else
                    ConfigUtility.StatusBar.Text = "Failed to save Pole.";
                }

                _poleLayer.EndSaveEdits += new EventHandler<EndEditEventArgs>(Polelayer_EndSaveEdits);
                _poleLayer.SaveEditsFailed += new EventHandler<TaskFailedEventArgs>(LayerOnSaveEditsFailed);
                if (_poleLayer.ValidateEdits) _poleLayer.SaveEdits();
                GraphicsLayer poleGraphic = MapProperty.Layers[poleMarkerID] as GraphicsLayer;
                poleGraphic.Graphics.Clear();
                this.Close();
            }
            catch (Exception ex)
            {
                logger.Error("PLC Delete Pole Exception: " + ex.Message);
            }
        }

        //Function to refresh pole feature service
        private void RefreshPoleLayer()
        {
            try
            {
                if (_isDelete == true)
                {
                    GraphicsLayer poleGraphic = MapProperty.Layers[poleMarkerID] as GraphicsLayer;
                    poleGraphic.Graphics.Clear();
                }

                //refresh On hover info display
                FeatureLayer publicationFSLayer = MapProperty.Layers["Rollover_PLD_INFO"] as FeatureLayer;

                if (publicationFSLayer != null)
                {
                    publicationFSLayer.Update();
                }

                //refresh PLCINFO feature service
                FeatureLayer PLCFeatureLayer = MapProperty.Layers[PoleLayerName] as FeatureLayer;

                if (PLCFeatureLayer != null)
                {
                    PLCFeatureLayer.Update();
                }

                //refresh PLCINFO_Labels Map service
                ArcGISDynamicMapServiceLayer PLCLabelsLayer = MapProperty.Layers[_poleLabelsLayerName] as ArcGISDynamicMapServiceLayer;

                if (PLCFeatureLayer != null)
                {
                    PLCLabelsLayer.Refresh();
                }
            }
            catch (Exception ex)
            {
                logger.Error("PLC Refresh Pole Layer Exception: " + ex.Message);
            }

        }
        // On click event handler for copy PLDBID to clipboard button on Pole Popup
        private void CopyPLDBID_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string clipboardText = poleNoTxt.Text;

                if (clipboardText != null && clipboardText != "")
                {
                    Clipboard.SetText(clipboardText);
                }
            }
            catch (SecurityException se)
            {

                MessageBox.Show("You must give the application permission to access your clipboard.");
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
            }
        }

        private void CancelBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.Close();
                Application.Current.RootVisual.SetValue(Control.IsEnabledProperty, true);
                if (!_isCopy)
                {
                    _objectIdSelected = null;
                    _copyPoleNo = null;
                    copyPoleNoTxt.Text = "";
                    GraphicsLayer layer = MapProperty.Layers[poleMarkerID] as GraphicsLayer;
                    if (layer != null)
                    {
                        if (layer.Graphics.Count > 0)
                        {
                            layer.Graphics.Clear();
                        }
                    }
                }
                else
                {
                    if (this.CopyButton.Content.ToString() == "Cancel Copy")
                    {
                        _isCopy = false;
                        _plcWidgetObj.IsSelectPoleActive = false;
                        this.CopyButton.Content = "Copy";
                        _copyPoleNo = null;
                        copyPoleNoTxt.Text = "";
                        _plcWidgetObj.IsNewPoleActive = true;
                        _objectIdSelected = null;
                        GraphicsLayer layer = MapProperty.Layers[poleMarkerID] as GraphicsLayer;
                        if (layer != null)
                        {
                            if (layer.Graphics.Count > 0)
                            {
                                layer.Graphics.Clear();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error("PLC Pole Popup close Exception: " + ex.Message);
            }
        }

        private void cboOrderNo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                PLCBusyIndicator.IsBusy = true;
                if (cboOrderNo.SelectedItem != null)
                {
                    string selectedWipObjectId = ((System.Collections.Generic.KeyValuePair<string, string>)(cboOrderNo.SelectedItem)).Key.ToString();
                    string selectedWipOrderNo = ((System.Collections.Generic.KeyValuePair<string, string>)(cboOrderNo.SelectedItem)).Value.ToString();

                    if (_plcWidgetObj.WipOrderDescription.ContainsKey(selectedWipObjectId))
                    {
                        if (_plcWidgetObj.WipOrderDescription[selectedWipObjectId] != "Null")
                        {
                            DescTxt.Text = _plcWidgetObj.WipOrderDescription[selectedWipObjectId];
                        }
                        else
                        {
                            DescTxt.Text = "";
                        }
                    }
                    if (_plcWidgetObj.WipOrderNotificationNo.ContainsKey(selectedWipObjectId))
                    {
                        if (_plcWidgetObj.WipOrderNotificationNo[selectedWipObjectId] != "Null")
                        {
                            notificaionNoTxt.Text = _plcWidgetObj.WipOrderNotificationNo[selectedWipObjectId];
                        }
                        else
                        {
                            notificaionNoTxt.Text = "";
                        }
                    }
                    if (_plcWidgetObj != null && selectedWipOrderNo != "Null" && !_plcWidgetObj.IsMovePoleActive)
                    {
                        _plcWidgetObj.LoadPoleAttributes(selectedWipOrderNo);
                    }
                    if (_plcWidgetObj != null && _plcWidgetObj.IsSelectPoleActive)
                    {
                        _plcWidgetObj.openPolePopupForExistingPoleInWIP(selectedWipOrderNo, DescTxt.Text, notificaionNoTxt.Text);
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }




    }
}

