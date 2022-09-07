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
using ArcFM.Silverlight.PGE.CustomTools;
using ESRI.ArcGIS.Client.Symbols;
using System.Windows.Media.Imaging;
using ESRI.ArcGIS.Client.Tasks;
using Miner.Server.Client.Toolkit;
using System.Windows.Controls.Primitives;
using ESRI.ArcGIS.Client.Toolkit;

using NLog;
using Miner.Server.Client.Events;
using Miner.Server.Client.Toolkit.Events;
using System.Xml.Linq;
using ESRI.ArcGIS.Client.Geometry;
using System.Windows.Browser;
using System.Text;
using System.IO;
using ArcFMSilverlight.Controls.PLC;
using System.ComponentModel;

namespace ArcFMSilverlight
{
    public partial class PLCWidget : UserControl
    {
        private List<string> _plcLabelFieldNames = new List<string>();
        private string _plcLayerName = string.Empty;
        private string _wipLayerName = string.Empty;
        private string _plcMapServiceUrl = string.Empty;
        private string _plcFeatureServiceUrl = string.Empty;
        private static bool isNewPoleCurrentlyActive = false;
        private static bool isSelectPoleCurrentlyActive = false;
        private static bool isOpenOcalcCurrentlyActive = false;
        private static bool isMovePoleCurrentlyActive = false;
        private static bool isPLDStatusCurrentlyActive = false;
        private const string PinCursor = @"/Images/pin.png";
        private const string SelectionCursor = @"/ESRI.ArcGIS.Client.Toolkit;component/Images/NewSelection.png";
        private Logger logger = LogManager.GetCurrentClassLogger();
        private bool isDelete = false;
        private static string PoleGraphicID = "PoleGraphicID";
        private string _polesLayerName = string.Empty;
        private string _wip;
        private string _plcWIPServiceUrl = string.Empty;
        private string _plcDynamicServiceUrl = string.Empty;
        private string _plcLayerId = string.Empty;
        private string _elevationServiceUrl = string.Empty;
        private string _snowLoadServiceUrl = string.Empty;
        private string _snowLoadLayerId = string.Empty;
        private Graphic _newpoleGraphic = null;
        private Graphic _selectPoleGraphic = null;
        private Graphic _selectMovePoleGraphic = null;
        private string _pmOrderLayerId = string.Empty;
        private string _jobid = "";
        private static bool isDeletePoleCurrentlyActive = false;
        private const string DeleteCursor = @"/ESRI.ArcGIS.Client.Toolkit;component/Images/deleteFeature.png";
        private double poleLatitude;
        private double poleLongitude;
        Geoprocessor elevationGPTask = null;
        private string _geoServiceUrl = string.Empty;
        private string _pgeDataBlock = string.Empty;
        private string _startAnalysisEIServiceUrl = string.Empty;
        private string _removePoleFromOrderEIServiceUrl = string.Empty;
        private string _authorizationEIHeader = string.Empty;
        private IdentifyEventArgs _selectedPoleFeatureset = null;
        private MeasureControl _measureTool = null;
        public ArcFmMeasureAction _measureActionObj = null;
        private bool _clearAll;
        private List<SegmentLayer> _layers = new List<SegmentLayer>();
        private static bool isMeasureNewPoleActive = false;
        private static bool isLatLongNewPoleActive = false;
        PLCAttributeEditor PLCAttributeWindow = new PLCAttributeEditor();
        private Dictionary<string, string> _WipOrderDescription = null;
        private Dictionary<string, string> _wipOrderNotificationNo = null;
        NewPoleFromLATLONG PLCNewPoleLatLong = new NewPoleFromLATLONG();
        private string _plcLabelsLayerName = string.Empty;
        private string _getZOrderDataServiceUrl = string.Empty;
        private GetZOrderData _PLDPoleData = null;
        Dictionary<ulong, ZOrderDataAttr> _pldPoleDic = null;

        //DA# 200103 ME Q1 2020 - START
        public bool IsPLCV6OCalc;
        private int _ocalcV6IdentifyFlag = 0;
        private Dictionary<string, string> _ocalcV6PMOrderNotificNoList;
        private string _selectedPLDBIDOcalcV6;
        private OCalcWIPSelectionControl _ocalcWIPSelectionControl = new OCalcWIPSelectionControl();
        //DA# 200103 ME Q1 2020 - END

        public string GetZOrderDataServiceUrl
        {
            get { return _getZOrderDataServiceUrl; }
            set { _getZOrderDataServiceUrl = value; }
        }
        public string PLCLabelsLayerName
        {
            get { return _plcLabelsLayerName; }
            set { _plcLabelsLayerName = value; }
        }

        public Dictionary<string, string> WipOrderNotificationNo
        {
            get { return _wipOrderNotificationNo; }
            set { _wipOrderNotificationNo = value; }
        }
        public Dictionary<string, string> WipOrderDescription
        {
            get { return _WipOrderDescription; }
            set { _WipOrderDescription = value; }
        }
        public ArcFmMeasureAction MeasureActionObj
        {
            get { return _measureActionObj; }
            set { _measureActionObj = value; }
        }


        public MeasureControl MeasureTool
        {
            get { return _measureTool; }
            set { _measureTool = value; }
        }

        public List<string> PLCLabelFieldNames
        {
            get { return _plcLabelFieldNames; }
            set { _plcLabelFieldNames = value; }
        }

        public bool IsmeasureNewPoleActive
        {
            get { return isMeasureNewPoleActive; }
            set { isMeasureNewPoleActive = value; }
        }

        public string PMOrderLayerId
        {
            get { return _pmOrderLayerId; }
            set { _pmOrderLayerId = value; }
        }

        public string PLCLayerName
        {
            get { return _plcLayerName; }
            set { _plcLayerName = value; }
        }

        public string WIPLayerName
        {
            get { return _wipLayerName; }
            set { _wipLayerName = value; }
        }

        public string SnowLoadLayerId
        {
            get { return _snowLoadLayerId; }
            set { _snowLoadLayerId = value; }
        }


        public string GeoServiceUrl
        {
            get { return _geoServiceUrl; }
            set { _geoServiceUrl = value; }
        }

        public string SnowLoadServiceUrl
        {
            get { return _snowLoadServiceUrl; }
            set { _snowLoadServiceUrl = value; }
        }
        public string ElevationServiceUrl
        {
            get { return _elevationServiceUrl; }
            set { _elevationServiceUrl = value; }
        }

        public string PLCDynamicServiceUrl
        {
            get { return _plcDynamicServiceUrl; }
            set { _plcDynamicServiceUrl = value; }
        }

        public string WIPServiceUrl
        {
            get { return _plcWIPServiceUrl; }
            set { _plcWIPServiceUrl = value; }
        }

        public string PLCMapServiceUrl
        {
            get { return _plcMapServiceUrl; }
            set { _plcMapServiceUrl = value; }
        }

        public string PLCFeatureServiceUrl
        {
            get { return _plcFeatureServiceUrl; }
            set { _plcFeatureServiceUrl = value; }
        }

        public string PolesLayerName
        {
            get { return _polesLayerName; }
            set { _polesLayerName = value; }
        }

        public string PLCLayerId
        {
            get { return _plcLayerId; }
            set { _plcLayerId = value; }
        }

        public string PGEDataBlock
        {
            get { return _pgeDataBlock; }
            set { _pgeDataBlock = value; }
        }

        public string StartAnalysisEIServiceUrl
        {
            get { return _startAnalysisEIServiceUrl; }
            set { _startAnalysisEIServiceUrl = value; }
        }

        public string RemovePoleFromOrderEIServiceUrl
        {
            get { return _removePoleFromOrderEIServiceUrl; }
            set { _removePoleFromOrderEIServiceUrl = value; }
        }

        public string AuthorizationEIHeader
        {
            get { return _authorizationEIHeader; }
            set { _authorizationEIHeader = value; }
        }

        public PLCWidget()
        {
            InitializeComponent();
            EventAggregator.GetEvent<ControlActivatedEvent>().Subscribe(OnControlActivated);
            PLCAttributeWindow.PLCWidegetObj = this;
            PLCNewPoleLatLong.PLCWidegetObj = this;
        }


        public Map MapProperty
        {
            get { return (Map)GetValue(MapPropertyProperty); }
            set { SetValue(MapPropertyProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MapProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MapPropertyProperty =
            DependencyProperty.Register("MapProperty", typeof(Map), typeof(PLCWidget), null);

        //Function to disable PLC Tools when another tool is activated
        public void OnControlActivated(DependencyObject control)
        {
            if (control == this) return;
            IsNewPoleActive = false;
            IsDeletePoleActive = false;
            IsSelectPoleActive = false;
            IsOpenOcalcActive = false;
            IsMovePoleActive = false;
            isMeasureNewPoleActive = false;
            IsPLDStatusActive = false;

        }



        //<summary>
        // Gets the identifier for the <see cref="IsActive"/> dependency property.
        /// </summary>


        public static readonly DependencyProperty IsActiveNewPoleProperty = DependencyProperty.Register(
            "IsNewPoleActive",
            typeof(bool),
            typeof(PLCWidget),
            new PropertyMetadata(OnIsNewPoleActiveChanged));

        private static void OnIsNewPoleActiveChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {

            var control = (PLCWidget)d;
            isNewPoleCurrentlyActive = (bool)e.NewValue;
            if (isNewPoleCurrentlyActive)
            {
                EventAggregator.GetEvent<ControlActivatedEvent>().Publish(control);
                if (control.MapProperty == null) return;
                CursorSet.SetID(control.MapProperty, PinCursor);
            }
            else
            {
                string currentMapCursor = CursorSet.GetID(control.MapProperty);
                if (currentMapCursor.Contains("pin.png") == true)
                    CursorSet.SetID(control.MapProperty, "Arrow");
            }
        }


        public static readonly DependencyProperty IsOpenOcalcProperty = DependencyProperty.Register(
          "IsOpenOcalcActive",
          typeof(bool),
          typeof(PLCWidget),
          new PropertyMetadata(OnIsOpenOcalcActiveChanged));

        private static void OnIsOpenOcalcActiveChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {

            var control = (PLCWidget)d;
            isOpenOcalcCurrentlyActive = (bool)e.NewValue;
            if (isOpenOcalcCurrentlyActive)
            {
                EventAggregator.GetEvent<ControlActivatedEvent>().Publish(control);
                if (control.MapProperty == null) return;
                CursorSet.SetID(control.MapProperty, SelectionCursor);
            }
            else
            {
                string currentMapCursor = CursorSet.GetID(control.MapProperty);
                if (currentMapCursor.Contains("NewSelection.png") == true && isSelectPoleCurrentlyActive == false && isMovePoleCurrentlyActive == false)
                    CursorSet.SetID(control.MapProperty, "Arrow");
            }
        }

        public bool IsOpenOcalcActive
        {
            get
            {
                return (bool)GetValue(IsOpenOcalcProperty);
            }
            set
            {
                SetValue(IsOpenOcalcProperty, value);
                if (IsOpenOcalcActive)
                {
                    IsNewPoleActive = false;
                    IsDeletePoleActive = false;
                    IsSelectPoleActive = false;
                    IsMovePoleActive = false;
                    isMeasureNewPoleActive = false;
                    IsPLDStatusActive = false;
                    MeasureTool.IsActive = false;
                    if (_measureActionObj != null)
                    {
                        _measureActionObj.ClearLayers();
                    }

                    if (MapProperty != null)
                        MapProperty.MouseClick += new EventHandler<ESRI.ArcGIS.Client.Map.MouseEventArgs>(Map_MouseClickOpenOCalc);
                }
                else
                {
                    if (MapProperty != null)
                        MapProperty.MouseClick -= new EventHandler<ESRI.ArcGIS.Client.Map.MouseEventArgs>(Map_MouseClickOpenOCalc);
                }

                OpenOCalc.IsChecked = value;
            }
        }


        public static readonly DependencyProperty IsMovePoleProperty = DependencyProperty.Register(
          "IsMovePoleActive",
          typeof(bool),
          typeof(PLCWidget),
          new PropertyMetadata(OnIsMovePoleActiveChanged));

        private static void OnIsMovePoleActiveChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {

            var control = (PLCWidget)d;
            isMovePoleCurrentlyActive = (bool)e.NewValue;
            if (isMovePoleCurrentlyActive)
            {
                EventAggregator.GetEvent<ControlActivatedEvent>().Publish(control);
                if (control.MapProperty == null) return;
                CursorSet.SetID(control.MapProperty, SelectionCursor);
            }
            else
            {
                string currentMapCursor = CursorSet.GetID(control.MapProperty);
                if ((currentMapCursor.Contains("NewSelection.png") == true || currentMapCursor.Contains("Hand") == true) && isSelectPoleCurrentlyActive == false && isOpenOcalcCurrentlyActive == false && isPLDStatusCurrentlyActive == false)
                    CursorSet.SetID(control.MapProperty, "Arrow");
            }
        }

        public bool IsMovePoleActive
        {
            get
            {
                return (bool)GetValue(IsMovePoleProperty);
            }
            set
            {
                SetValue(IsMovePoleProperty, value);
                if (IsMovePoleActive)
                {
                    IsNewPoleActive = false;
                    IsDeletePoleActive = false;
                    IsSelectPoleActive = false;
                    IsOpenOcalcActive = false;
                    isMeasureNewPoleActive = false;
                    IsPLDStatusActive = false;
                    MeasureTool.IsActive = false;
                    if (_measureActionObj != null)
                    {
                        _measureActionObj.ClearLayers();
                    }

                    if (MapProperty != null)
                        MapProperty.MouseClick += new EventHandler<ESRI.ArcGIS.Client.Map.MouseEventArgs>(Map_MouseClickMovePole);
                }
                else
                {
                    if (MapProperty != null)
                    {
                        MapProperty.MouseClick -= new EventHandler<ESRI.ArcGIS.Client.Map.MouseEventArgs>(Map_MouseClickMovePole);
                        MapProperty.MouseClick -= new EventHandler<ESRI.ArcGIS.Client.Map.MouseEventArgs>(Map_MouseClickDropMovePole);



                    }
                }

                MovePole.IsChecked = value;
            }
        }

        public static readonly DependencyProperty IsActiveSelectPoleProperty = DependencyProperty.Register(
         "IsSelectPoleActive",
         typeof(bool),
         typeof(PLCWidget),
         new PropertyMetadata(OnIsSelectPoleActiveChanged));

        private static void OnIsSelectPoleActiveChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (PLCWidget)d;
            isSelectPoleCurrentlyActive = (bool)e.NewValue;
            if (isSelectPoleCurrentlyActive)
            {
                EventAggregator.GetEvent<ControlActivatedEvent>().Publish(control);
                if (control.MapProperty == null) return;
                CursorSet.SetID(control.MapProperty, "Hand");
            }
            else
            {
                string currentMapCursor = CursorSet.GetID(control.MapProperty);
                if (currentMapCursor.Contains("Hand") == true && isPLDStatusCurrentlyActive == false && isOpenOcalcCurrentlyActive == false && isMovePoleCurrentlyActive == false)
                    CursorSet.SetID(control.MapProperty, "Arrow");
            }
        }

        public bool IsSelectPoleActive
        {
            get
            {
                return (bool)GetValue(IsActiveSelectPoleProperty);
            }
            set
            {
                SetValue(IsActiveSelectPoleProperty, value);
                if (IsSelectPoleActive)
                {
                    IsOpenOcalcActive = false;
                    IsNewPoleActive = false;
                    IsDeletePoleActive = false;
                    IsMovePoleActive = false;
                    isMeasureNewPoleActive = false;
                    IsPLDStatusActive = false;
                    MeasureTool.IsActive = false;
                    if (_measureActionObj != null)
                    {
                        _measureActionObj.ClearLayers();
                    }

                    if (MapProperty != null)
                    {
                        MapProperty.MouseClick += new EventHandler<ESRI.ArcGIS.Client.Map.MouseEventArgs>(Map_MouseClickEditDelete);
                        FeatureLayer PLCFeatureLayer = MapProperty.Layers[PLCLayerName] as FeatureLayer;
                        if (PLCFeatureLayer.Visible == false)
                        {
                            PLCFeatureLayer.Visible = true;
                        }
                    }
                }
                else
                {
                    if (MapProperty != null)
                        MapProperty.MouseClick -= new EventHandler<ESRI.ArcGIS.Client.Map.MouseEventArgs>(Map_MouseClickEditDelete);
                }

                SelectPole.IsChecked = value;
            }
        }

        public static readonly DependencyProperty IsActiveDeletePoleProperty = DependencyProperty.Register(
          "IsDeletePoleActive",
          typeof(bool),
          typeof(PLCWidget),
          new PropertyMetadata(OnIsDeletePoleActiveChanged));

        private static void OnIsDeletePoleActiveChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (PLCWidget)d;
            isDeletePoleCurrentlyActive = (bool)e.NewValue;
            if (isDeletePoleCurrentlyActive)
            {
                EventAggregator.GetEvent<ControlActivatedEvent>().Publish(control);
                if (control.MapProperty == null) return;
                CursorSet.SetID(control.MapProperty, DeleteCursor);
            }
            else
            {
                string currentMapCursor = CursorSet.GetID(control.MapProperty);
                if (currentMapCursor.Contains("deleteFeature.png") == true)
                    CursorSet.SetID(control.MapProperty, "Arrow");
            }
        }

        //Mouse click event handler for move pole tool
        private void Map_MouseClickMovePole(object sender, Map.MouseEventArgs e)
        {
            ExecuteIdentifyTask(e.MapPoint, "MovePole");


        }

        //Mouse click event handler for Select/Delete Pole tool
        private void Map_MouseClickEditDelete(object sender, Map.MouseEventArgs e)
        {
            ExecuteIdentifyTask(e.MapPoint, "DeletePole");
        }

        public bool IsDeletePoleActive
        {
            get
            {
                return (bool)GetValue(IsActiveDeletePoleProperty);
            }
            set
            {
                SetValue(IsActiveDeletePoleProperty, value);
                if (IsDeletePoleActive)
                {
                    IsOpenOcalcActive = false;
                    IsNewPoleActive = false;
                    IsSelectPoleActive = false;
                    IsMovePoleActive = false;
                    isMeasureNewPoleActive = false;
                    IsPLDStatusActive = false;
                    MeasureTool.IsActive = false;
                    if (_measureActionObj != null)
                    {
                        _measureActionObj.ClearLayers();
                    }

                    if (MapProperty != null)
                        MapProperty.MouseClick += new EventHandler<ESRI.ArcGIS.Client.Map.MouseEventArgs>(Map_MouseClickEditDelete);
                }
                else
                {
                    if (MapProperty != null)
                        MapProperty.MouseClick -= new EventHandler<ESRI.ArcGIS.Client.Map.MouseEventArgs>(Map_MouseClickEditDelete);
                }

                DeletePole.IsChecked = value;
            }
        }

        // Get existing pole record when  a pole is selected on map
        private void ExecuteIdentifyTask(MapPoint mapPoint, string btnFlag)
        {
            try
            {
                if (MapProperty.Layers[_plcLayerName].Visible == true)
                {
                    ConfigUtility.StatusBar.Text = "";
                    var identifyParameters = new IdentifyParameters();
                    identifyParameters.Geometry = mapPoint;
                    identifyParameters.LayerIds.Add(0);
                    identifyParameters.MapExtent = MapProperty.Extent;
                    identifyParameters.LayerOption = LayerOption.visible;
                    identifyParameters.Height = (int)MapProperty.ActualHeight;
                    identifyParameters.Width = (int)MapProperty.ActualWidth;
                    identifyParameters.SpatialReference = MapProperty.SpatialReference;
                    identifyParameters.Tolerance = 15;

                    var url = _plcDynamicServiceUrl;
                    var identifyTask = new IdentifyTask(url);
                    if (btnFlag == "DeletePole")
                    {
                        identifyTask.Failed += new EventHandler<TaskFailedEventArgs>(pldIdentifyTask_Failed);
                        identifyTask.ExecuteCompleted += new EventHandler<IdentifyEventArgs>(pldIdentifyTask_ExecuteCompleted);
                    }
                    else if (btnFlag == "OpenOcalc")
                    {
                        identifyTask.Failed += new EventHandler<TaskFailedEventArgs>(ocalcIdentifyTask_Failed);
                        identifyTask.ExecuteCompleted += new EventHandler<IdentifyEventArgs>(ocalcIdentifyTask_ExecuteCompleted);
                        _ocalcV6IdentifyFlag = 0;
                        _ocalcV6PMOrderNotificNoList = new Dictionary<string, string>();
                        //DA# 200103 ME Q1 2020 - START
                        if (IsPLCV6OCalc == true)
                        {
                            //check for WIP polygons at the clicked location
                            try
                            {
                                var identifyParametersWIP = new IdentifyParameters();
                                identifyParametersWIP.Geometry = mapPoint;
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
                        //DA# 200103 ME Q1 2020 - END
                    }
                    else if (btnFlag == "MovePole")
                    {
                        identifyTask.Failed += new EventHandler<TaskFailedEventArgs>(ocalcIdentifyTask_Failed);
                        identifyTask.ExecuteCompleted += new EventHandler<IdentifyEventArgs>(movePoleIdentifyTask_ExecuteCompleted);
                    }


                    identifyTask.ExecuteAsync(identifyParameters);
                }
                else
                {
                    ConfigUtility.StatusBar.Text = "No Pole found at clicked location.";
                }
            }
            catch (Exception ex)
            {
                logger.Error("PLC Exception Indetify Task:" + ex.Message);
            }
        }


        private void movePoleIdentifyTask_ExecuteCompleted(object sender, IdentifyEventArgs e)
        {
            ConfigUtility.StatusBar.Text = string.Empty;
            try
            {
                if (e.IdentifyResults != null)
                {
                    if (e.IdentifyResults.Count > 0)
                    {
                        if (e.IdentifyResults[0].Feature.Attributes["SAPEQUIPID"] == null || Convert.ToString(e.IdentifyResults[0].Feature.Attributes["SAPEQUIPID"]) == "" || Convert.ToString(e.IdentifyResults[0].Feature.Attributes["SAPEQUIPID"]) == "Null" || Convert.ToString(e.IdentifyResults[0].Feature.Attributes["SAPEQUIPID"]) == "0")
                        {
                            ESRI.ArcGIS.Client.Geometry.Geometry selectedGeom = e.IdentifyResults[0].Feature.Geometry;
                            AddGraphic(selectedGeom);
                            _selectPoleGraphic = new Graphic()
                            {
                                Geometry = selectedGeom,
                            };
                            _selectMovePoleGraphic = _selectPoleGraphic;
                            _selectedPoleFeatureset = null;
                            var feature = e.IdentifyResults[0];
                            _selectedPoleFeatureset = e;

                            //check whether existing pole inside Wip or not 
                            checkPolePlacedInWIP(feature.Feature.Geometry, "MovePole");
                        }
                        else
                        {
                            MessageBox.Show("Pole with SAPEQUIPID can't be moved");
                        }
                    }
                    else
                        ConfigUtility.StatusBar.Text = "No Pole found at clicked location.";
                }
            }
            catch (Exception ex)
            {
                logger.Error("PLC Move Pole Exception Indetify Task Execute Complete:" + ex.Message);
            }
        }



        private void pldIdentifyTask_ExecuteCompleted(object sender, IdentifyEventArgs e)
        {
            ConfigUtility.StatusBar.Text = string.Empty;
            try
            {
                if (e.IdentifyResults != null)
                {
                    if (e.IdentifyResults.Count > 0)
                    {
                        ESRI.ArcGIS.Client.Geometry.Geometry selectedGeom = e.IdentifyResults[0].Feature.Geometry;
                        AddGraphic(selectedGeom);
                        _selectPoleGraphic = new Graphic()
                        {
                            Geometry = selectedGeom,
                        };


                        //Delete Pole scenario
                        if (isDelete)
                        {
                            if (e.IdentifyResults[0].Feature.Attributes["SAPEQUIPID"] == null || Convert.ToString(e.IdentifyResults[0].Feature.Attributes["SAPEQUIPID"]) == "0" || Convert.ToString(e.IdentifyResults[0].Feature.Attributes["SAPEQUIPID"]) == "Null" || Convert.ToString(e.IdentifyResults[0].Feature.Attributes["SAPEQUIPID"]) == "")
                            {
                                _selectedPoleFeatureset = null;
                                var feature = e.IdentifyResults[0];
                                _selectedPoleFeatureset = e;
                                //check whether existing pole inside Wip or not 
                                checkPolePlacedInWIP(feature.Feature.Geometry, "DeletePole");
                            }
                            else
                            {
                                MessageBox.Show("Pole with SAPEQUIPID can't be deleted");
                            }


                        }
                        // Copy Pole Scenario
                        else if (PLCAttributeWindow.IsCopy)
                        {
                            if (e.IdentifyResults[0].Feature.Attributes["PLDBID"] != null && e.IdentifyResults[0].Feature.Attributes["PLDBID"].ToString() != "Null")
                            {
                                PLCAttributeWindow.copyPoleNoTxt.Text = e.IdentifyResults[0].Feature.Attributes["PLDBID"].ToString();
                                PLCAttributeWindow.Show();
                                PLCAttributeWindow.CopyButton.IsEnabled = true;
                            }
                            else
                            {
                                MessageBox.Show("PLDB ID for selected pole not found");
                            }
                        }
                        //Existing Pole Scenario
                        else
                        {

                            _selectedPoleFeatureset = null;
                            var feature = e.IdentifyResults[0];
                            _selectedPoleFeatureset = e;
                            //check whether existing pole inside Wip or not 
                            checkPolePlacedInWIP(feature.Feature.Geometry, "ExistingPole");


                        }
                    }
                    else
                        ConfigUtility.StatusBar.Text = "No Pole found at clicked location.";
                }
            }
            catch (Exception err)
            {
                logger.Error("PLC Exception Update/Delete pole:" + err.Message);
            }

        }

        // Function for populating value in pole popup for existing pole inside wip and show pole popup
        public void openPolePopupForExistingPoleInWIP(string wipOrderNumber, string wipDescription, string wipNotificationNo)
        {
            try
            {
                var poleFeature = _selectedPoleFeatureset.IdentifyResults[0];
                SetPoleAttributeWinFields(PLCLayerName, MapProperty, _selectPoleGraphic, null, poleFeature.Feature.Attributes["OBJECTID"], null);



                if (wipOrderNumber == "Null")
                {
                    if (poleFeature.Feature.Attributes["MaintenanceCode"] != null && poleFeature.Feature.Attributes["MaintenanceCode"] != "Null")
                    {
                        PLCAttributeWindow.matCodeTxt.Text = poleFeature.Feature.Attributes["MaintenanceCode"].ToString();
                    }
                    else
                    {
                        PLCAttributeWindow.matCodeTxt.Text = "";
                    }
                }

                if (poleFeature.Feature.Attributes["LANID"] != null && poleFeature.Feature.Attributes["LANID"].ToString() != "Null")
                {
                    PLCAttributeWindow.lanIdTxt.Text = poleFeature.Feature.Attributes["LANID"].ToString();
                }
                else
                {
                    PLCAttributeWindow.lanIdTxt.Text = "";
                }
                if (poleFeature.Feature.Attributes["CREATED_DATE"] != null && poleFeature.Feature.Attributes["CREATED_DATE"].ToString() != "Null")
                {
                    PLCAttributeWindow.creationDateTxt.Text = poleFeature.Feature.Attributes["CREATED_DATE"].ToString();
                }
                else
                {
                    DateTime currentDate = DateTime.Now;
                    PLCAttributeWindow.creationDateTxt.Text = currentDate.ToString();
                }
                if (poleFeature.Feature.Attributes["LAT"] != null && poleFeature.Feature.Attributes["LAT"].ToString() != "Null")
                {
                    PLCAttributeWindow.latTxt.Text = poleFeature.Feature.Attributes["LAT"].ToString();
                }
                else
                {
                    PLCAttributeWindow.latTxt.Text = "";
                }
                if (poleFeature.Feature.Attributes["LONGITUDE"] != null && poleFeature.Feature.Attributes["LONGITUDE"].ToString() != "Null")
                {
                    PLCAttributeWindow.longTxt.Text = poleFeature.Feature.Attributes["LONGITUDE"].ToString();
                }
                else
                {
                    PLCAttributeWindow.longTxt.Text = "";
                }
                if (poleFeature.Feature.Attributes["PLDBID"] != null && poleFeature.Feature.Attributes["PLDBID"].ToString() != "Null")
                {
                    PLCAttributeWindow.poleNoTxt.Text = poleFeature.Feature.Attributes["PLDBID"].ToString();
                }
                else
                {
                    PLCAttributeWindow.poleNoTxt.Text = "";
                }
                if (poleFeature.Feature.Attributes["SAPEQUIPID"] != null && poleFeature.Feature.Attributes["SAPEQUIPID"].ToString() != "Null")
                {
                    PLCAttributeWindow.sapIdTxt.Text = poleFeature.Feature.Attributes["SAPEQUIPID"].ToString();
                }
                else
                {
                    PLCAttributeWindow.sapIdTxt.Text = "";
                }
                if (poleFeature.Feature.Attributes["SKETCH_LOC"] != null && poleFeature.Feature.Attributes["SKETCH_LOC"].ToString() != "Null")
                {
                    PLCAttributeWindow.sketchLocTxt.Text = poleFeature.Feature.Attributes["SKETCH_LOC"].ToString();
                }
                else
                {
                    PLCAttributeWindow.sketchLocTxt.Text = "";
                }
                if (poleFeature.Feature.Attributes["COPY_PLD"] != null && poleFeature.Feature.Attributes["COPY_PLD"].ToString() != "Null")
                {
                    PLCAttributeWindow.copyPoleNoTxt.Text = poleFeature.Feature.Attributes["COPY_PLD"].ToString();
                }
                else
                {
                    PLCAttributeWindow.copyPoleNoTxt.Text = "";
                }
                if (poleFeature.Feature.Attributes["SNOW_LOAD_DIST"] != null && poleFeature.Feature.Attributes["SNOW_LOAD_DIST"].ToString() != "Null")
                {
                    PLCAttributeWindow.snowLoadTxt.Text = poleFeature.Feature.Attributes["SNOW_LOAD_DIST"].ToString();
                }
                else
                {
                    PLCAttributeWindow.snowLoadTxt.Text = "";
                }
                if (poleFeature.Feature.Attributes["ELEVATION"] != null && poleFeature.Feature.Attributes["ELEVATION"].ToString() != "Null")
                {
                    PLCAttributeWindow.ElevationTxt.Text = poleFeature.Feature.Attributes["ELEVATION"].ToString();
                }
                else
                {
                    PLCAttributeWindow.ElevationTxt.Text = "";
                }

                PLCAttributeWindow.Show();
                PLCAttributeWindow.CopyButton.IsEnabled = false;
                PLCAttributeWindow.PLCBusyIndicator.IsBusy = false;
                PLCAttributeWindow.OKButton.IsEnabled = true;
                PLCAttributeWindow.sketchLocTxt.IsEnabled = true;
            }
            catch (Exception ex)
            {
                logger.Error("PLC Widget Exception:" + ex.Message);
            }
        }

        //Fow Existing pole outside wip
        private void openPolePopupForExistingPole()
        {
            try
            {
                var feature = _selectedPoleFeatureset.IdentifyResults[0];
                SetPoleAttributeWinFields(PLCLayerName, MapProperty, _selectPoleGraphic, null, feature.Feature.Attributes["OBJECTID"], null);

                if (feature.Feature.Attributes["ORDER_NUMBER"] != null && feature.Feature.Attributes["ORDER_NUMBER"].ToString() != "Null")
                {
                    Dictionary<string, string> featureset = new Dictionary<string, string>();
                    featureset.Add(feature.Feature.Attributes["OBJECTID"].ToString(), feature.Feature.Attributes["ORDER_NUMBER"].ToString());
                    PLCAttributeWindow.cboOrderNo.ItemsSource = featureset;
                    PLCAttributeWindow.cboOrderNo.SelectedIndex = 0;
                    PLCAttributeWindow.cboOrderNo.IsEnabled = true;
                    //PLCAttributeWindow.orderNoTxt.Text = feature.Feature.Attributes["ORDER_NUMBER"].ToString();
                }
                else
                {
                    // PLCAttributeWindow.orderNoTxt.Text = "";
                    Dictionary<string, string> featureset = new Dictionary<string, string>();
                    PLCAttributeWindow.cboOrderNo.ItemsSource = featureset;
                    PLCAttributeWindow.cboOrderNo.IsEnabled = false;

                }
                if (feature.Feature.Attributes["NOTIFICATION_NUMBER"] != null && feature.Feature.Attributes["NOTIFICATION_NUMBER"].ToString() != "Null")
                {
                    PLCAttributeWindow.notificaionNoTxt.Text = feature.Feature.Attributes["NOTIFICATION_NUMBER"].ToString();
                }
                else
                {
                    PLCAttributeWindow.notificaionNoTxt.Text = "";
                }
                if (feature.Feature.Attributes["MaintenanceCode"] != null && feature.Feature.Attributes["MaintenanceCode"] != "Null")
                {
                    PLCAttributeWindow.matCodeTxt.Text = feature.Feature.Attributes["MaintenanceCode"].ToString();
                }
                else
                {
                    PLCAttributeWindow.matCodeTxt.Text = "";
                }
                if (feature.Feature.Attributes["ORDER_DESCRIPTION"] != null && feature.Feature.Attributes["ORDER_DESCRIPTION"].ToString() != "Null")
                {
                    PLCAttributeWindow.DescTxt.Text = feature.Feature.Attributes["ORDER_DESCRIPTION"].ToString();
                }
                else
                {
                    PLCAttributeWindow.DescTxt.Text = "";
                }
                if (feature.Feature.Attributes["LANID"] != null && feature.Feature.Attributes["LANID"].ToString() != "Null")
                {
                    PLCAttributeWindow.lanIdTxt.Text = feature.Feature.Attributes["LANID"].ToString();
                }
                else
                {
                    PLCAttributeWindow.lanIdTxt.Text = "";
                }
                if (feature.Feature.Attributes["CREATED_DATE"] != null && feature.Feature.Attributes["CREATED_DATE"].ToString() != "Null")
                {
                    PLCAttributeWindow.creationDateTxt.Text = feature.Feature.Attributes["CREATED_DATE"].ToString();
                }
                else
                {
                    PLCAttributeWindow.creationDateTxt.Text = DateTime.Now.ToString();
                }
                if (feature.Feature.Attributes["LAT"] != null && feature.Feature.Attributes["LAT"].ToString() != "Null")
                {
                    PLCAttributeWindow.latTxt.Text = feature.Feature.Attributes["LAT"].ToString();
                }
                else
                {
                    PLCAttributeWindow.latTxt.Text = "";
                }
                if (feature.Feature.Attributes["LONGITUDE"] != null && feature.Feature.Attributes["LONGITUDE"].ToString() != "Null")
                {
                    PLCAttributeWindow.longTxt.Text = feature.Feature.Attributes["LONGITUDE"].ToString();
                }
                else
                {
                    PLCAttributeWindow.longTxt.Text = "";
                }
                if (feature.Feature.Attributes["PLDBID"] != null && feature.Feature.Attributes["PLDBID"].ToString() != "Null")
                {
                    PLCAttributeWindow.poleNoTxt.Text = feature.Feature.Attributes["PLDBID"].ToString();
                }
                else
                {
                    PLCAttributeWindow.poleNoTxt.Text = "";
                }
                if (feature.Feature.Attributes["SAPEQUIPID"] != null && feature.Feature.Attributes["SAPEQUIPID"].ToString() != "Null")
                {
                    PLCAttributeWindow.sapIdTxt.Text = feature.Feature.Attributes["SAPEQUIPID"].ToString();
                }
                else
                {
                    PLCAttributeWindow.sapIdTxt.Text = "";
                }
                if (feature.Feature.Attributes["SKETCH_LOC"] != null && feature.Feature.Attributes["SKETCH_LOC"].ToString() != "Null")
                {
                    PLCAttributeWindow.sketchLocTxt.Text = feature.Feature.Attributes["SKETCH_LOC"].ToString();
                }
                else
                {
                    PLCAttributeWindow.sketchLocTxt.Text = "";
                }
                if (feature.Feature.Attributes["COPY_PLD"] != null && feature.Feature.Attributes["COPY_PLD"].ToString() != "Null")
                {
                    PLCAttributeWindow.copyPoleNoTxt.Text = feature.Feature.Attributes["COPY_PLD"].ToString();
                }
                else
                {
                    PLCAttributeWindow.copyPoleNoTxt.Text = "";
                }
                if (feature.Feature.Attributes["SNOW_LOAD_DIST"] != null && feature.Feature.Attributes["SNOW_LOAD_DIST"].ToString() != "Null")
                {
                    PLCAttributeWindow.snowLoadTxt.Text = feature.Feature.Attributes["SNOW_LOAD_DIST"].ToString();
                }
                else
                {
                    PLCAttributeWindow.snowLoadTxt.Text = "";
                }
                if (feature.Feature.Attributes["ELEVATION"] != null && feature.Feature.Attributes["ELEVATION"].ToString() != "Null")
                {
                    PLCAttributeWindow.ElevationTxt.Text = feature.Feature.Attributes["ELEVATION"].ToString();
                }
                else
                {
                    PLCAttributeWindow.ElevationTxt.Text = "";
                }

                PLCAttributeWindow.Show();
                PLCAttributeWindow.CopyButton.IsEnabled = false;
                PLCAttributeWindow.PLCBusyIndicator.IsBusy = false;
                PLCAttributeWindow.OKButton.IsEnabled = false;
                PLCAttributeWindow.sketchLocTxt.IsEnabled = false;
            }
            catch (Exception e)
            {
                logger.Error("PLC Widget Exception:" + e.Message);
            }
        }

        private void pldIdentifyTask_Failed(object sender, TaskFailedEventArgs e)
        {
            logger.Error("PLC Identify Task failed: " + e.Error);
            ConfigUtility.StatusBar.Text = "Selection of Pole has failed. " + e.Error.Message;
        }

        //Function to activate/deactivate new pole tool
        private void Create_NewPole_Click(object sender, RoutedEventArgs e)
        {
            ConfigUtility.StatusBar.Text = string.Empty;
            PLCAttributeWindow.PLCBusyIndicator.IsBusy = true;
            PLCAttributeWindow.CopyButton.IsEnabled = true;
            PLCAttributeWindow.CopyButton.Content = "Copy";

            PLCAttributeWindow.IsCopy = false;
            if (NewPole.IsChecked == true)
            {
                IsNewPoleActive = true;
                isDelete = false;
                clearGraphic();
            }
            else
            {
                IsNewPoleActive = false;
            }
        }

        public bool IsNewPoleActive
        {
            get
            {
                return (bool)GetValue(IsActiveNewPoleProperty);
            }
            set
            {
                SetValue(IsActiveNewPoleProperty, value);
                if (IsNewPoleActive)
                {
                    IsSelectPoleActive = false;
                    IsDeletePoleActive = false;
                    IsOpenOcalcActive = false;
                    IsMovePoleActive = false;
                    isMeasureNewPoleActive = false;
                    IsPLDStatusActive = false;
                    MeasureTool.IsActive = false;
                    if (_measureActionObj != null)
                    {
                        _measureActionObj.ClearLayers();
                    }

                    if (MapProperty != null)
                    {
                        MapProperty.MouseClick += new EventHandler<ESRI.ArcGIS.Client.Map.MouseEventArgs>(Map_MouseClickCreatePole);
                        FeatureLayer PLCFeatureLayer = MapProperty.Layers[PLCLayerName] as FeatureLayer;
                        if (PLCFeatureLayer.Visible == false)
                        {
                            PLCFeatureLayer.Visible = true;
                        }
                    }
                }
                else
                {
                    if (MapProperty != null)
                        MapProperty.MouseClick -= new EventHandler<ESRI.ArcGIS.Client.Map.MouseEventArgs>(Map_MouseClickCreatePole);
                }

                NewPole.IsChecked = value;
            }
        }

        //Function called on map click event while creating new pole
        private void Map_MouseClickCreatePole(object sender, Map.MouseEventArgs e)
        {

            var geometry = e.MapPoint;

            createNewPole(geometry);

        }

        // Function to create new pole and get lat long values
        private void createNewPole(ESRI.ArcGIS.Client.Geometry.Geometry geometry)
        {
            try
            {
                PLCAttributeWindow.PLCBusyIndicator.IsBusy = true;
                PLCAttributeWindow.CopyButton.IsEnabled = true;
                PLCAttributeWindow.CopyButton.Content = "Copy";
                // Convert point X and Y to Lat and Long 
                var geometryService = new GeometryService(_geoServiceUrl);

                geometryService.Failed += GeometryServiceFailed;
                geometryService.ProjectCompleted += GeometryServiceProjectCompleted;

                var graphic = new Graphic { Geometry = geometry };
                var graphics = new List<Graphic> { graphic };

                geometryService.ProjectAsync(graphics, new SpatialReference(4326));

                //Check whether new pole is placed inside WIP
                checkPolePlacedInWIP(geometry, "NewPole");

                _newpoleGraphic = null;
                _newpoleGraphic = new Graphic()
                {
                    Geometry = geometry,
                };
            }
            catch (Exception ex)
            {
                logger.Error("PLC Exception Create New Pole" + ex.Message);
            }
        }

        private void GeometryServiceFailed(object sender, TaskFailedEventArgs e)
        {
            logger.Error("PLD Coordinates tool geometry task failed! " + e.Error.Message);
            throw new Exception("CoordinatesControl: ProjectionError: " + e.Error.Message);
        }

        // Response from geometry service for Lat and Long
        private void GeometryServiceProjectCompleted(object sender, GraphicsEventArgs e)
        {
            try
            {
                var wgsPoint = (MapPoint)e.Results[0].Geometry;

                poleLatitude = Math.Round(wgsPoint.Y, 8);
                poleLongitude = Math.Round(wgsPoint.X, 8);

                PLCAttributeWindow.latTxt.Text = poleLatitude.ToString();
                PLCAttributeWindow.longTxt.Text = poleLongitude.ToString();
            }
            catch (Exception ex)
            {
                logger.Error("PLC Geometry Service Exception:" + ex.Message);
            }

        }

        // Function to get Snow Load Dist value
        public void getSnowLoadDist(Graphic graphic)
        {
            try
            {
                var identifyParameters = new IdentifyParameters();
                identifyParameters.Geometry = graphic.Geometry;
                identifyParameters.LayerIds.Add(Convert.ToInt16(_snowLoadLayerId));
                identifyParameters.MapExtent = MapProperty.Extent;
                identifyParameters.LayerOption = LayerOption.all;
                identifyParameters.Height = (int)MapProperty.ActualHeight;
                identifyParameters.Width = (int)MapProperty.ActualWidth;
                identifyParameters.SpatialReference = MapProperty.SpatialReference;
                identifyParameters.Tolerance = 7;


                var identifyTask = new IdentifyTask(_snowLoadServiceUrl);
                identifyTask.Failed += new EventHandler<TaskFailedEventArgs>(snowload_identifyTask_Failed);
                identifyTask.ExecuteCompleted += new EventHandler<IdentifyEventArgs>(snowload_identifyTask_ExecuteCompleted);
                identifyTask.ExecuteAsync(identifyParameters);
            }
            catch (Exception ex)
            {
                logger.Error("PLC Snow Load Dist Exception:" + ex.Message);
            }
        }

        //Get snow load dist avlue and populate in pole popup
        private void snowload_identifyTask_ExecuteCompleted(object sender, IdentifyEventArgs e)
        {
            ConfigUtility.StatusBar.Text = string.Empty;
            try
            {
                if (e.IdentifyResults != null)
                {
                    if (e.IdentifyResults.Count > 0)
                    {
                        if (e.IdentifyResults[0].LayerName.Contains("Snow Loading Zone"))
                        {
                            PLCAttributeWindow.snowLoadTxt.Text = e.IdentifyResults[0].Feature.Attributes["SNOWLOAD"].ToString();
                        }
                    }
                    else
                    {
                        PLCAttributeWindow.snowLoadTxt.Text = "Light";
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error("PLC Snow Load Dist Exception:" + ex.Message);
            }
            finally
            {
                if (IsSelectPoleActive)
                {
                    PLCAttributeWindow.createPoleFeature();
                    IsSelectPoleActive = false;
                }
                PLCAttributeWindow.PLCBusyIndicator.IsBusy = false;
            }
        }

        void snowload_identifyTask_Failed(object sender, TaskFailedEventArgs e)
        {
            logger.Error("PLC Snow Load Identify Exception:" + e.Error);
            PLCAttributeWindow.PLCBusyIndicator.IsBusy = false;
            ConfigUtility.StatusBar.Text = "Snow Load Query Failed";

        }

        // Function to get Elevation value
        public void getElevationValue(Graphic graphic)
        {
            try
            {
                PLCAttributeWindow.PLCBusyIndicator.IsBusy = true;
                _selectPoleGraphic = graphic;
                //Add a default attribute to the newly created graphic
                List<Field> flds = new List<Field>();
                Field fld = new Field();
                fld.FieldName = "OBJECTID";
                fld.Type = Field.FieldType.OID;
                flds.Add(fld);
                Field fld1 = new Field();
                fld1.FieldName = "TYPE";
                fld1.Type = Field.FieldType.String;



                FeatureSet ft = new FeatureSet();
                ft.Fields = flds;
                if (!graphic.Attributes.ContainsKey("OBJECTID"))
                {
                    graphic.Attributes.Add("OBJECTID", 1);

                }


                ft.Features.Add(graphic);
                ft.DisplayFieldName = "OBJECTID";


                elevationGPTask = new Geoprocessor(_elevationServiceUrl);

                elevationGPTask.JobCompleted += new EventHandler<JobInfoEventArgs>(elevationGPTask_JobCompleted);
                elevationGPTask.StatusUpdated += new EventHandler<JobInfoEventArgs>(elevationGPTask_StatusUpdated);
                elevationGPTask.Failed += new EventHandler<ESRI.ArcGIS.Client.Tasks.TaskFailedEventArgs>(elevationGPTask_Failed);


                List<GPParameter> gpParameters = new List<GPParameter>();
                gpParameters.Add(new GPFeatureRecordSetLayer("Point", ft));


                elevationGPTask.SubmitJobAsync(gpParameters);
            }
            catch (Exception ex)
            {
                logger.Error("PLC Elevation Exception:" + ex.Message);
            }
        }


        void elevationGPTask_StatusUpdated(object sender, JobInfoEventArgs e)
        {
            _jobid = e.JobInfo.JobId;
            switch (e.JobInfo.JobStatus)
            {
                case esriJobStatus.esriJobSubmitted:

                    break;
                case esriJobStatus.esriJobSucceeded:
                    ConfigUtility.StatusBar.Text = "";

                    break;
                case esriJobStatus.esriJobFailed:

                    ConfigUtility.StatusBar.Text = "Failed to get Elevation..";
                    break;
                case esriJobStatus.esriJobTimedOut:

                    break;
            }
        }

        /// <summary>
        /// Failed event handler for Elevation Geoprocessor Task
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void elevationGPTask_Failed(object sender, ESRI.ArcGIS.Client.Tasks.TaskFailedEventArgs e)
        {
            try
            {
                logger.Error("PLC Elevation Service Failed:" + e.Error);
                ConfigUtility.StatusBar.Text = "Failed to get Elevation..";
                if (IsNewPoleActive || isMeasureNewPoleActive)
                {
                    if (isMeasureNewPoleActive == true)
                    {
                        CursorSet.SetID(MapProperty, "Arrow");
                    }
                    getSnowLoadDist(_newpoleGraphic);
                    isMeasureNewPoleActive = false;
                    MeasureTool.IsActive = false;
                    if (_measureActionObj != null)
                    {
                        _measureActionObj.ClearLayers();
                    }

                }
                else if (IsMovePoleActive)
                {
                    getSnowLoadDist(_selectPoleGraphic);
                    IsMovePoleActive = false;
                }
            }
            catch (Exception ex)
            {
                logger.Error("PLC Elevation Service Failed:" + ex.Message);
            }
        }
        /// <summary>
        /// Job Completed event handler for Elevation Geoprocessor Task
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void elevationGPTask_JobCompleted(object sender, JobInfoEventArgs e)
        {
            try
            {
                elevationGPTask.GetResultDataCompleted += new EventHandler<GPParameterEventArgs>(elevationGPTask_GetResultDataCompleted);
                elevationGPTask.GetResultDataAsync(e.JobInfo.JobId, "OutElevation_DTM");


            }
            catch (Exception ex)
            {
                logger.Error("PLC Elevation Service Failed:" + ex.Message);
            }
        }

        /// <summary>
        /// GetResult data Completed event handler for Elevation Geoprocessor Task
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void elevationGPTask_GetResultDataCompleted(object sender, GPParameterEventArgs e)
        {
            try
            {

                GPString outputResults = e.Parameter as GPString;
                string elevationResults = outputResults.Value;
                PLCAttributeWindow.ElevationTxt.Text = elevationResults;

            }
            catch (Exception ex)
            {
                logger.Error("PLC Elevation Service Failed:" + ex.Message);
            }
            finally
            {
                if (IsNewPoleActive || isMeasureNewPoleActive || isLatLongNewPoleActive)
                {
                    if (isMeasureNewPoleActive == true)
                    {
                        CursorSet.SetID(MapProperty, "Arrow");
                    }
                    getSnowLoadDist(_newpoleGraphic);
                    isMeasureNewPoleActive = false;
                    isLatLongNewPoleActive = false;
                    MeasureTool.IsActive = false;
                    if (_measureActionObj != null)
                    {
                        _measureActionObj.ClearLayers();
                    }

                }
                else if (IsMovePoleActive)
                {
                    getSnowLoadDist(_selectPoleGraphic);
                    IsMovePoleActive = false;
                }
                else if (IsSelectPoleActive)
                {
                    getSnowLoadDist(_selectPoleGraphic);


                }
            }
        }

        // FUnction to check whether new pole is placed inside WIP
        private void checkPolePlacedInWIP(ESRI.ArcGIS.Client.Geometry.Geometry geom, string poleType)
        {
            try
            {
                var identifyParameters = new IdentifyParameters();
                identifyParameters.Geometry = geom;
                identifyParameters.LayerIds.Add(0);
                identifyParameters.MapExtent = MapProperty.Extent;
                identifyParameters.LayerOption = LayerOption.visible;
                identifyParameters.Height = (int)MapProperty.ActualHeight;
                identifyParameters.Width = (int)MapProperty.ActualWidth;
                identifyParameters.SpatialReference = MapProperty.SpatialReference;
                identifyParameters.Tolerance = 2;


                var identifyTask = new IdentifyTask(_plcWIPServiceUrl);
                if (poleType == "NewPole")
                {
                    PLCAttributeWindow.sketchLocTxt.IsEnabled = true;
                    identifyTask.Failed += new EventHandler<TaskFailedEventArgs>(wip_identifyTask_Failed);
                    identifyTask.ExecuteCompleted += new EventHandler<IdentifyEventArgs>(wip_identifyTask_ExecuteCompleted);
                }
                else if (poleType == "ExistingPole")
                {
                    identifyTask.Failed += new EventHandler<TaskFailedEventArgs>(wipExistingPole_identifyTask_Failed);
                    identifyTask.ExecuteCompleted += new EventHandler<IdentifyEventArgs>(wipExistingPole_identifyTask_ExecuteCompleted);
                }
                else if (poleType == "MovePole")
                {
                    identifyTask.Failed += new EventHandler<TaskFailedEventArgs>(wipExistingPole_identifyTask_Failed);
                    identifyTask.ExecuteCompleted += new EventHandler<IdentifyEventArgs>(wipMovePole_identifyTask_ExecuteCompleted);
                }
                else if (poleType == "DropMovePole")
                {
                    identifyTask.Failed += new EventHandler<TaskFailedEventArgs>(wipExistingPole_identifyTask_Failed);
                    identifyTask.ExecuteCompleted += new EventHandler<IdentifyEventArgs>(wipDropMovePole_identifyTask_ExecuteCompleted);
                }
                else if (poleType == "DeletePole")
                {
                    identifyTask.Failed += new EventHandler<TaskFailedEventArgs>(wipExistingPole_identifyTask_Failed);
                    identifyTask.ExecuteCompleted += new EventHandler<IdentifyEventArgs>(wipDeletePole_identifyTask_ExecuteCompleted);
                }
                else if (poleType == "PLDStatus")
                {
                    identifyTask.Failed += new EventHandler<TaskFailedEventArgs>(wipExistingPole_identifyTask_Failed);
                    identifyTask.ExecuteCompleted += new EventHandler<IdentifyEventArgs>(wipPLDStatus_identifyTask_ExecuteCompleted);
                }
                identifyTask.ExecuteAsync(identifyParameters);
            }
            catch (Exception ex)
            {

                logger.Error("PLC Elevation Service Failed:" + ex.Message);
            }

        }

        //Delete Pole Tool
        private void wipDeletePole_identifyTask_ExecuteCompleted(object sender, IdentifyEventArgs e)
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
                            MessageBoxResult result = MessageBox.Show("Are you sure you want to delete following Pole?\n\nClick OK to confirm.", "Delete Pole", MessageBoxButton.OKCancel);
                            if (result == MessageBoxResult.OK)
                            {
                                SetPoleAttributeWinFields(PLCLayerName, MapProperty, null, null, _selectedPoleFeatureset.IdentifyResults[0].Feature.Attributes["OBJECTID"], null);
                                PLCAttributeWindow.DeleteSelectedPole();

                            }
                        }
                    }

                    else
                    {
                        MessageBox.Show("Please Select Pole Inside WIP");
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error("PLC Delete Pole Exception:" + ex.Message);
            }
        }

        private void wipMovePole_identifyTask_ExecuteCompleted(object sender, IdentifyEventArgs e)
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
                            Dictionary<string, string> featureset = new Dictionary<string, string>();
                            _WipOrderDescription = new Dictionary<string, string>();
                            _wipOrderNotificationNo = new Dictionary<string, string>();
                            var poleFeature = _selectedPoleFeatureset.IdentifyResults[0];

                            if (poleFeature.Feature != null)
                            {
                                _WipOrderDescription.Add(poleFeature.Feature.Attributes["OBJECTID"].ToString(), Convert.ToString(poleFeature.Feature.Attributes["ORDER_DESCRIPTION"]));
                                _wipOrderNotificationNo.Add(poleFeature.Feature.Attributes["OBJECTID"].ToString(), Convert.ToString(poleFeature.Feature.Attributes["NOTIFICATION_NUMBER"]));
                                featureset.Add(poleFeature.Feature.Attributes["OBJECTID"].ToString(), poleFeature.Feature.Attributes["ORDER_NUMBER"].ToString());
                            }

                            for (int i = 0; i < e.IdentifyResults.Count; i++)
                            {
                                if (e.IdentifyResults[i].Feature.Attributes["PM Order Number"] != null)
                                {
                                    if (!featureset.ContainsValue(Convert.ToString(e.IdentifyResults[i].Feature.Attributes["PM Order Number"])))
                                    {
                                        _WipOrderDescription.Add(e.IdentifyResults[i].Feature.Attributes["OBJECTID"].ToString(), Convert.ToString(e.IdentifyResults[i].Feature.Attributes["Description"]));
                                        _wipOrderNotificationNo.Add(e.IdentifyResults[i].Feature.Attributes["OBJECTID"].ToString(), Convert.ToString(e.IdentifyResults[i].Feature.Attributes["NOTIFICATION_NUMBER"]));
                                        featureset.Add(e.IdentifyResults[i].Feature.Attributes["OBJECTID"].ToString(), e.IdentifyResults[i].Feature.Attributes["PM Order Number"].ToString());
                                    }
                                }
                            }
                            PLCAttributeWindow.cboOrderNo.ItemsSource = featureset;
                            PLCAttributeWindow.cboOrderNo.SelectedIndex = 0;
                            PLCAttributeWindow.cboOrderNo.IsEnabled = false;
                            activatePoleEditor();

                        }
                    }

                    else
                    {
                        MessageBox.Show("Please Select Pole Inside WIP");
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error("PLC Move Pole Exception:" + ex.Message);
            }
        }

        private void wipDropMovePole_identifyTask_ExecuteCompleted(object sender, IdentifyEventArgs e)
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
                            Dictionary<string, string> featureset = new Dictionary<string, string>();
                            Dictionary<string, string> orderNoList = new Dictionary<string, string>();
                            _WipOrderDescription = new Dictionary<string, string>();
                            _wipOrderNotificationNo = new Dictionary<string, string>();

                            for (int i = 0; i < e.IdentifyResults.Count; i++)
                            {
                                if (e.IdentifyResults[i].Feature.Attributes["PM Order Number"] != null)
                                {
                                    if (!orderNoList.ContainsValue(Convert.ToString(e.IdentifyResults[i].Feature.Attributes["PM Order Number"])))
                                    {
                                        orderNoList.Add(e.IdentifyResults[i].Feature.Attributes["OBJECTID"].ToString(), e.IdentifyResults[i].Feature.Attributes["PM Order Number"].ToString());
                                    }
                                }
                            }

                            var poleFeature = _selectedPoleFeatureset.IdentifyResults[0];

                            if (poleFeature.Feature != null)
                            {
                                _WipOrderDescription.Add(poleFeature.Feature.Attributes["OBJECTID"].ToString(), Convert.ToString(poleFeature.Feature.Attributes["ORDER_DESCRIPTION"]));
                                _wipOrderNotificationNo.Add(poleFeature.Feature.Attributes["OBJECTID"].ToString(), Convert.ToString(poleFeature.Feature.Attributes["NOTIFICATION_NUMBER"]));
                                featureset.Add(poleFeature.Feature.Attributes["OBJECTID"].ToString(), poleFeature.Feature.Attributes["ORDER_NUMBER"].ToString());
                            }

                            for (int i = 0; i < e.IdentifyResults.Count; i++)
                            {
                                if (e.IdentifyResults[i].Feature.Attributes["PM Order Number"] != null)
                                {
                                    if (!featureset.ContainsValue(Convert.ToString(e.IdentifyResults[i].Feature.Attributes["PM Order Number"])))
                                    {
                                        _WipOrderDescription.Add(e.IdentifyResults[i].Feature.Attributes["OBJECTID"].ToString(), Convert.ToString(e.IdentifyResults[i].Feature.Attributes["Description"]));
                                        _wipOrderNotificationNo.Add(e.IdentifyResults[i].Feature.Attributes["OBJECTID"].ToString(), Convert.ToString(e.IdentifyResults[i].Feature.Attributes["NOTIFICATION_NUMBER"]));
                                        featureset.Add(e.IdentifyResults[i].Feature.Attributes["OBJECTID"].ToString(), e.IdentifyResults[i].Feature.Attributes["PM Order Number"].ToString());

                                    }
                                }
                            }


                            if (orderNoList.ContainsValue(Convert.ToString(_selectedPoleFeatureset.IdentifyResults[0].Feature.Attributes["ORDER_NUMBER"])))
                            {
                                PLCAttributeWindow.cboOrderNo.ItemsSource = featureset;
                                PLCAttributeWindow.cboOrderNo.SelectedIndex = 0;
                                PLCAttributeWindow.cboOrderNo.IsEnabled = false;
                                _selectPoleGraphic = _selectMovePoleGraphic;
                                showPoleAttributes(_selectMovePoleGraphic);
                                ESRI.ArcGIS.Client.Geometry.Geometry movePoleNewGeometry = _selectMovePoleGraphic.Geometry;

                                _selectMovePoleGraphic.Selected = false;
                                _selectMovePoleGraphic = null;

                                if (MapProperty != null)
                                    MapProperty.MouseClick -= new EventHandler<ESRI.ArcGIS.Client.Map.MouseEventArgs>(Map_MouseClickDropMovePole);

                                string currentMapCursor = CursorSet.GetID(MapProperty);
                                if (currentMapCursor.Contains("Hand") == true)
                                    CursorSet.SetID(MapProperty, "NewSelection.png");

                            }
                            else
                            {
                                MessageBox.Show("Please Move Pole Inside Same WIP");
                                PLCAttributeWindow.cboOrderNo.ItemsSource = null;
                                PLCAttributeWindow.cboOrderNo.IsEnabled = true;
                            }

                        }
                    }

                    else
                    {
                        MessageBox.Show("Please Move Pole Inside Same WIP");
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error("PLC Move Pole Drop Exception:" + ex.Message);
            }
        }
        //Function for move pole tool to changes mouse cursor and register move pole drop event
        private void activatePoleEditor()
        {

            try
            {
                string currentMapCursor = CursorSet.GetID(MapProperty);
                if (currentMapCursor.Contains("NewSelection.png") == true)
                    CursorSet.SetID(MapProperty, "Hand");
                if (MapProperty != null)
                    MapProperty.MouseClick -= new EventHandler<ESRI.ArcGIS.Client.Map.MouseEventArgs>(Map_MouseClickMovePole);

                MapProperty.MouseClick += new EventHandler<ESRI.ArcGIS.Client.Map.MouseEventArgs>(Map_MouseClickDropMovePole);
            }
            catch (Exception ex)
            {
                logger.Error("PLC Move Pole Exception:" + ex.Message);
            }
        }

        private void Map_MouseClickDropMovePole(object sender, Map.MouseEventArgs e)
        {
            e.Handled = true;
            if (_selectMovePoleGraphic != null)
            {
                AddGraphic(e.MapPoint);
                _selectMovePoleGraphic.Geometry = e.MapPoint;
                checkPolePlacedInWIP(e.MapPoint, "DropMovePole");
            }

        }


        private void showPoleAttributes(Graphic movePoleGraphic)
        {
            try
            {
                var poleFeature = _selectedPoleFeatureset.IdentifyResults[0];
                string selectedWipOrderNo = ((System.Collections.Generic.KeyValuePair<string, string>)(PLCAttributeWindow.cboOrderNo.SelectedItem)).Value.ToString();
                if (selectedWipOrderNo == poleFeature.Feature.Attributes["ORDER_NUMBER"].ToString())
                {
                    var geometryService = new GeometryService(_geoServiceUrl);

                    geometryService.Failed += GeometryServiceFailed;
                    geometryService.ProjectCompleted += GeometryServiceProjectCompleted;

                    var graphic = new Graphic { Geometry = movePoleGraphic.Geometry };
                    var graphics = new List<Graphic> { graphic };

                    geometryService.ProjectAsync(graphics, new SpatialReference(4326));


                    SetPoleAttributeWinFields(PLCLayerName, MapProperty, _selectPoleGraphic, null, poleFeature.Feature.Attributes["OBJECTID"], null);

                    // get updated elevation value
                    getElevationValue(movePoleGraphic);

                    if (poleFeature.Feature.Attributes["MaintenanceCode"] != null && poleFeature.Feature.Attributes["MaintenanceCode"] != "Null")
                    {
                        PLCAttributeWindow.matCodeTxt.Text = poleFeature.Feature.Attributes["MaintenanceCode"].ToString();
                    }
                    else
                    {
                        PLCAttributeWindow.matCodeTxt.Text = "";
                    }

                    if (poleFeature.Feature.Attributes["LANID"] != null && poleFeature.Feature.Attributes["LANID"].ToString() != "Null")
                    {
                        PLCAttributeWindow.lanIdTxt.Text = poleFeature.Feature.Attributes["LANID"].ToString();
                    }
                    else
                    {
                        PLCAttributeWindow.lanIdTxt.Text = "";
                    }
                    if (poleFeature.Feature.Attributes["CREATED_DATE"] != null && poleFeature.Feature.Attributes["CREATED_DATE"].ToString() != "Null")
                    {
                        PLCAttributeWindow.creationDateTxt.Text = poleFeature.Feature.Attributes["CREATED_DATE"].ToString();
                    }
                    else
                    {
                        PLCAttributeWindow.creationDateTxt.Text = DateTime.Now.ToString();
                    }

                    if (poleFeature.Feature.Attributes["PLDBID"] != null && poleFeature.Feature.Attributes["PLDBID"].ToString() != "Null")
                    {
                        PLCAttributeWindow.poleNoTxt.Text = poleFeature.Feature.Attributes["PLDBID"].ToString();
                    }
                    else
                    {
                        PLCAttributeWindow.poleNoTxt.Text = "";
                    }
                    if (poleFeature.Feature.Attributes["SAPEQUIPID"] != null && poleFeature.Feature.Attributes["SAPEQUIPID"].ToString() != "Null")
                    {
                        PLCAttributeWindow.sapIdTxt.Text = poleFeature.Feature.Attributes["SAPEQUIPID"].ToString();
                    }
                    else
                    {
                        PLCAttributeWindow.sapIdTxt.Text = "";
                    }
                    if (poleFeature.Feature.Attributes["SKETCH_LOC"] != null && poleFeature.Feature.Attributes["SKETCH_LOC"].ToString() != "Null")
                    {
                        PLCAttributeWindow.sketchLocTxt.Text = poleFeature.Feature.Attributes["SKETCH_LOC"].ToString();
                    }
                    else
                    {
                        PLCAttributeWindow.sketchLocTxt.Text = "";
                    }
                    if (poleFeature.Feature.Attributes["COPY_PLD"] != null && poleFeature.Feature.Attributes["COPY_PLD"].ToString() != "Null")
                    {
                        PLCAttributeWindow.copyPoleNoTxt.Text = poleFeature.Feature.Attributes["COPY_PLD"].ToString();
                    }
                    else
                    {
                        PLCAttributeWindow.copyPoleNoTxt.Text = "";
                    }

                    PLCAttributeWindow.Show();
                    PLCAttributeWindow.CopyButton.IsEnabled = false;
                    PLCAttributeWindow.PLCBusyIndicator.IsBusy = true;
                    PLCAttributeWindow.OKButton.IsEnabled = true;
                    PLCAttributeWindow.sketchLocTxt.IsEnabled = true;
                }
                else
                {
                    MessageBox.Show("Please move pole inside same WIP");
                }
            }
            catch (Exception ex)
            {
                logger.Error("PLC Move Pole Show Attributes Exception:" + ex.Message);
            }

        }

        private void wipExistingPole_identifyTask_ExecuteCompleted(object sender, IdentifyEventArgs e)
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
                            Dictionary<string, string> featureset = new Dictionary<string, string>();
                            _WipOrderDescription = new Dictionary<string, string>();
                            _wipOrderNotificationNo = new Dictionary<string, string>();
                            var poleFeature = _selectedPoleFeatureset.IdentifyResults[0];

                            //if (poleFeature.Feature!=null)
                            //{
                            //    _WipOrderDescription.Add(poleFeature.Feature.Attributes["OBJECTID"].ToString(), Convert.ToString(poleFeature.Feature.Attributes["ORDER_DESCRIPTION"]));
                            //    _wipOrderNotificationNo.Add(poleFeature.Feature.Attributes["OBJECTID"].ToString(), Convert.ToString(poleFeature.Feature.Attributes["NOTIFICATION_NUMBER"]));                                  
                            //    featureset.Add(poleFeature.Feature.Attributes["OBJECTID"].ToString(), poleFeature.Feature.Attributes["ORDER_NUMBER"].ToString());
                            //}

                            for (int i = 0; i < e.IdentifyResults.Count; i++)
                            {
                                if (e.IdentifyResults[i].Feature.Attributes["PM Order Number"] != null)
                                {
                                    if (!featureset.ContainsValue(Convert.ToString(e.IdentifyResults[i].Feature.Attributes["PM Order Number"])))
                                    {
                                        _WipOrderDescription.Add(e.IdentifyResults[i].Feature.Attributes["OBJECTID"].ToString(), Convert.ToString(e.IdentifyResults[i].Feature.Attributes["Description"]));
                                        _wipOrderNotificationNo.Add(e.IdentifyResults[i].Feature.Attributes["OBJECTID"].ToString(), Convert.ToString(e.IdentifyResults[i].Feature.Attributes["NOTIFICATION_NUMBER"]));
                                        featureset.Add(e.IdentifyResults[i].Feature.Attributes["OBJECTID"].ToString(), e.IdentifyResults[i].Feature.Attributes["PM Order Number"].ToString());
                                    }
                                }
                            }
                            PLCAttributeWindow.cboOrderNo.ItemsSource = featureset;
                            PLCAttributeWindow.cboOrderNo.SelectedIndex = 0;
                            PLCAttributeWindow.cboOrderNo.IsEnabled = true;

                        }
                    }
                    else
                    {
                        openPolePopupForExistingPole();
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error("PLC Existing Pole Wip Exception:" + ex.Message);
            }
        }

        private void wipExistingPole_identifyTask_Failed(object sender, TaskFailedEventArgs e)
        {
            logger.Error("Wip Existing Pole Identify failed: " + e.Error);
            ConfigUtility.StatusBar.Text = "Selection of Wip has failed. " + e.Error.Message;
        }

        private void wip_identifyTask_ExecuteCompleted(object sender, IdentifyEventArgs e)
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

                            if (PLCNewPoleLatLong != null)
                            {
                                if (PLCNewPoleLatLong.IsNPLatLongActive == true)
                                {
                                    PLCNewPoleLatLong.Close();
                                    PLCNewPoleLatLong.IsNPLatLongActive = false;
                                    PLCNewPoleLatLong.latitideTxt.Text = "";
                                    PLCNewPoleLatLong.longitudeTxt.Text = "";
                                }

                            }

                            Dictionary<string, string> featureset = new Dictionary<string, string>();
                            _WipOrderDescription = new Dictionary<string, string>();
                            _wipOrderNotificationNo = new Dictionary<string, string>();
                            for (int i = 0; i < e.IdentifyResults.Count; i++)
                            {
                                if (e.IdentifyResults[i].Feature.Attributes["PM Order Number"] != null)
                                {
                                    _WipOrderDescription.Add(e.IdentifyResults[i].Feature.Attributes["OBJECTID"].ToString(), Convert.ToString(e.IdentifyResults[i].Feature.Attributes["Description"]));
                                    _wipOrderNotificationNo.Add(e.IdentifyResults[i].Feature.Attributes["OBJECTID"].ToString(), Convert.ToString(e.IdentifyResults[i].Feature.Attributes["NOTIFICATION_NUMBER"]));
                                    featureset.Add(e.IdentifyResults[i].Feature.Attributes["OBJECTID"].ToString(), e.IdentifyResults[i].Feature.Attributes["PM Order Number"].ToString());
                                }
                            }
                            PLCAttributeWindow.cboOrderNo.ItemsSource = featureset;
                            PLCAttributeWindow.cboOrderNo.SelectedIndex = 0;
                            PLCAttributeWindow.cboOrderNo.IsEnabled = true;
                            AddGraphic(_newpoleGraphic.Geometry);
                            SetPoleAttributeWinFields(PLCLayerName, MapProperty, _newpoleGraphic, null, null, null);


                        }
                    }
                    else
                    {
                        MessageBox.Show("Please create pole inside WIP");
                        if (isMeasureNewPoleActive)
                        {


                            _measureActionObj.ClearLayers();
                        }

                    }
                }
            }
            catch (Exception err)
            {
                logger.Error("PLC New Pole Exception:" + err.Message);
            }

        }

        private void wip_identifyTask_Failed(object sender, TaskFailedEventArgs e)
        {
            logger.Error("PLC Selection of Wip has failed: " + e.Error);
            ConfigUtility.StatusBar.Text = "Selection of Wip has failed. " + e.Error.Message;
        }

        // Function to get Pole attributes like MAT code
        public void LoadPoleAttributes(string wipOrderNo)
        {
            try
            {
                string url = _plcWIPServiceUrl + "/" + PMOrderLayerId;

                Query _query = new Query();
                _query.SpatialRelationship = SpatialRelationship.esriSpatialRelIntersects;
                _query.ReturnGeometry = false;

                _query.OutFields.AddRange(new string[] { "*" });
                _query.Where = "INSTALLJOBNUMBER=" + wipOrderNo;
                QueryTask _circuitqueryTask = new QueryTask(url);
                _circuitqueryTask.ExecuteCompleted += new EventHandler<QueryEventArgs>(_wipqueryTask_ExecuteCompleted);
                _circuitqueryTask.Failed += new EventHandler<TaskFailedEventArgs>(_wipqueryTask_Failed);
                _circuitqueryTask.ExecuteAsync(_query);
            }
            catch (Exception e)
            {
                logger.Error("PLC LoadPoleAttributes Exception: " + e.Message);
            }
        }

        void _wipqueryTask_Failed(object sender, TaskFailedEventArgs e)
        {
            logger.Error("PLC Wip Query Task has failed: " + e.Error);

        }

        void _wipqueryTask_ExecuteCompleted(object sender, QueryEventArgs e)
        {
            try
            {
                string lanId = WebContext.Current.User.Name.Replace("PGE\\", "").ToLower().Replace("admin", "");
                if (e.FeatureSet.Features.Count > 0)
                {
                    var feature = e.FeatureSet.Features[0];

                    PLCAttributeWindow.matCodeTxt.Text = feature.Attributes["MAT"].ToString();

                    PLCAttributeWindow.lanIdTxt.Text = lanId;
                    PLCAttributeWindow.creationDateTxt.Text = DateTime.Now.ToString();

                    PLCAttributeWindow.PLCBusyIndicator.IsBusy = false;
                    if (IsNewPoleActive || IsmeasureNewPoleActive || isLatLongNewPoleActive)
                    {

                        PLCAttributeWindow.poleNoTxt.Text = "";
                        PLCAttributeWindow.copyPoleNoTxt.Text = "";
                        PLCAttributeWindow.sketchLocTxt.Text = "";
                        PLCAttributeWindow.sapIdTxt.Text = "";
                        PLCAttributeWindow.snowLoadTxt.Text = "";
                        PLCAttributeWindow.ElevationTxt.Text = "";
                        getElevationValue(_newpoleGraphic);
                    }


                    PLCAttributeWindow.OKButton.IsEnabled = true;

                    PLCAttributeWindow.Show();

                }
                else
                {
                    PLCAttributeWindow.lanIdTxt.Text = lanId;
                    PLCAttributeWindow.creationDateTxt.Text = DateTime.Now.ToString();
                    PLCAttributeWindow.PLCBusyIndicator.IsBusy = false;
                    if (IsNewPoleActive || IsmeasureNewPoleActive || isLatLongNewPoleActive)
                    {

                        PLCAttributeWindow.poleNoTxt.Text = "";
                        PLCAttributeWindow.copyPoleNoTxt.Text = "";
                        PLCAttributeWindow.sketchLocTxt.Text = "";
                        PLCAttributeWindow.matCodeTxt.Text = "";
                        PLCAttributeWindow.ElevationTxt.Text = "";
                        PLCAttributeWindow.snowLoadTxt.Text = "";
                        PLCAttributeWindow.sapIdTxt.Text = "";
                        getElevationValue(_newpoleGraphic);
                    }

                    PLCAttributeWindow.OKButton.IsEnabled = true;

                    PLCAttributeWindow.Show();
                }
            }
            catch (Exception ex)
            {
                logger.Error("PLC Load Pole Attributes Exception: " + ex.Message);
            }
        }

        //Function to add graphic on map for pole
        private void AddGraphic(ESRI.ArcGIS.Client.Geometry.Geometry geom)
        {
            try
            {
                GraphicsLayer layer = MapProperty.Layers[PoleGraphicID] as GraphicsLayer;
                if (layer == null)
                {
                    layer = CreatePoleGraphicLayer();
                    MapProperty.Layers.Add(layer);
                }
                layer.Visible = true;
                if (!PLCAttributeWindow.IsCopy)
                {
                    layer.Graphics.Clear();

                    layer.Graphics.Add(new Graphic
                    {
                        Symbol = new SimpleMarkerSymbol
                        {
                            Color = new SolidColorBrush(Color.FromArgb(255, 0, 255, 255)),
                            Size = 18,
                            Style = SimpleMarkerSymbol.SimpleMarkerStyle.Circle,

                        },
                        Geometry = geom
                    });
                }
            }
            catch (Exception ex)
            {
                logger.Error("PLC Add Pole Graphic Exception: " + ex.Message);
            }
        }

        //Function to create graphic layer
        private GraphicsLayer CreatePoleGraphicLayer()
        {
            GraphicsLayer lyr = new GraphicsLayer { ID = PoleGraphicID };
            return lyr;
        }

        //Function to set config attributes to PLCAttributrEditor
        private void SetPoleAttributeWinFields(string polesLayerName, Map mapProperty, Graphic graphic, object orderNo, object objectIdSelected, object notesSelected)
        {
            try
            {
                PLCAttributeWindow.PoleLayerName = polesLayerName;
                PLCAttributeWindow.MapProperty = MapProperty;
                if (graphic != null)
                {
                    PLCAttributeWindow.Graphic = graphic;
                }
                if (orderNo != null && orderNo != "")
                {
                    PLCAttributeWindow.WipOrderNumber = orderNo.ToString();
                }
                PLCAttributeWindow.ObjectIdSelected = objectIdSelected;

                PLCAttributeWindow.PoleFeatureServiceUrl = _plcFeatureServiceUrl;
                PLCAttributeWindow.PoleDynamicServiceUrl = _plcDynamicServiceUrl;
                PLCAttributeWindow.PLCDynamicLayerId = _plcLayerId;
                PLCAttributeWindow.PGEDataFields = _pgeDataBlock;
                PLCAttributeWindow.AuthorizationHeader = _authorizationEIHeader;
                PLCAttributeWindow.SaveEIServiceUrl = _startAnalysisEIServiceUrl;
                PLCAttributeWindow.RemovePoleEIServiceUrl = _removePoleFromOrderEIServiceUrl;
                PLCAttributeWindow.PLCWIPServiceUrl = _plcWIPServiceUrl;
                PLCAttributeWindow.PoleLabelsLayerName = _plcLabelsLayerName;
            }
            catch (Exception ex)
            {
                logger.Error("PLC SetPoleAttributeWinFields Exception: " + ex.Message);
            }
        }

        //Function to activate nad deactivate select pole tool
        private void Select_Pole_Click(object sender, RoutedEventArgs e)
        {
            ConfigUtility.StatusBar.Text = string.Empty;
            if (SelectPole.IsChecked == true)
            {
                IsSelectPoleActive = true;
                isDelete = false;
                clearGraphic();
            }
            else
            {
                IsSelectPoleActive = false;
            }
        }
        //Function to activate nad deactivate delete pole tool
        private void Delete_Pole_Click(object sender, RoutedEventArgs e)
        {
            ConfigUtility.StatusBar.Text = string.Empty;
            if (DeletePole.IsChecked == true)
            {
                IsDeletePoleActive = true;
                isDelete = true;
                clearGraphic();
            }
            else
            {
                IsDeletePoleActive = false;
            }
        }

        //Function to activate and deactivate Open Ocalc tool
        private void OpenOclac_Click(object sender, RoutedEventArgs e)
        {

            ConfigUtility.StatusBar.Text = string.Empty;
            if (OpenOCalc.IsChecked == true)
            {
                IsOpenOcalcActive = true;
                isDelete = false;
                clearGraphic();
            }
            else
            {
                IsOpenOcalcActive = false;
                clearGraphic();
            }
        }


        private void Map_MouseClickOpenOCalc(object sender, Map.MouseEventArgs e)
        {
            ExecuteIdentifyTask(e.MapPoint, "OpenOcalc");
        }


        private void ocalcIdentifyTask_ExecuteCompleted(object sender, IdentifyEventArgs e)
        {
            try
            {
                if (e.IdentifyResults.Count > 0)
                {
                    ConfigUtility.StatusBar.Text = "";
                    ESRI.ArcGIS.Client.Geometry.Geometry selectedGeom = e.IdentifyResults[0].Feature.Geometry;
                    string pldbId = e.IdentifyResults[0].Feature.Attributes["PLDBID"].ToString();
                    if (pldbId != null && pldbId != "Null")
                    {
                        AddGraphic(selectedGeom);

                        //DA# 200103 ME Q1 2020 
                        if (IsPLCV6OCalc == true)
                        {
                            _ocalcV6IdentifyFlag++;
                            _selectedPLDBIDOcalcV6 = pldbId;
                            if (_ocalcV6IdentifyFlag == 2)
                                RedirectOCalcV6URI();
                        }
                        else
                        {
                            HtmlPage.Window.Navigate(new Uri(("PLDB://" + pldbId)), "_blank");
                        }
                    }
                    else
                    {
                        MessageBox.Show("PLDB ID not found for selected pole");

                    }

                }
                else
                {
                    ConfigUtility.StatusBar.Text = "No Pole found at clicked location.";
                }

            }
            catch (Exception ex)
            {
                logger.Error("PLC Open Ocalc Exception: " + ex.Message);
            }
        }

        private void ocalcIdentifyTask_Failed(object sender, TaskFailedEventArgs e)
        {
            logger.Error("PLC Open Ocalc Identify Task Failed: " + e.Error);
            ConfigUtility.StatusBar.Text = "Selection of Pole has failed. " + e.Error.Message;
        }

        //function to clear graphic from map.
        private void clearGraphic()
        {
            GraphicsLayer layer = MapProperty.Layers[PoleGraphicID] as GraphicsLayer;
            if (layer != null)
            {
                if (layer.Graphics.Count > 0)
                {
                    layer.Graphics.Clear();
                }
            }
        }

        private void NewPoleLATLONG_Click(object sender, RoutedEventArgs e)
        {
            clearGraphic();
            FeatureLayer PLCFeatureLayer = MapProperty.Layers[PLCLayerName] as FeatureLayer;
            if (PLCFeatureLayer != null)
            {
                if (PLCFeatureLayer.Visible == false)
                {
                    PLCFeatureLayer.Visible = true;
                }
            }
            ConfigUtility.StatusBar.Text = string.Empty;
            PLCNewPoleLatLong.GeometryServiceUrl = _geoServiceUrl;
            PLCNewPoleLatLong.MapProperty = MapProperty;
            PLCNewPoleLatLong.PoleGraphicLayerName = PoleGraphicID;
            IsNewPoleActive = false;
            IsDeletePoleActive = false;
            IsSelectPoleActive = false;
            IsOpenOcalcActive = false;
            IsPLDStatusActive = false;
            PLCNewPoleLatLong.IsNPLatLongActive = true;
            isLatLongNewPoleActive = true;
            PLCNewPoleLatLong.Show();

        }

        public void getPoleGraphicFromNewPoleLatLong(Graphic gotGraphic)
        {
            IsNewPoleActive = false;
            IsDeletePoleActive = false;
            IsSelectPoleActive = false;
            IsOpenOcalcActive = false;
            IsMovePoleActive = false;
            IsPLDStatusActive = false;
            AddGraphic(gotGraphic.Geometry);
            createNewPole(gotGraphic.Geometry);

        }


        private void Move_Pole_Click(object sender, RoutedEventArgs e)
        {
            ConfigUtility.StatusBar.Text = string.Empty;
            if (MovePole.IsChecked == true)
            {
                IsMovePoleActive = true;
                isDelete = false;
                clearGraphic();
            }
            else
            {
                IsMovePoleActive = false;
                clearGraphic();
            }
        }

        private void NewPoleMeasure_Click(object sender, RoutedEventArgs e)
        {
            IsNewPoleActive = false;
            IsDeletePoleActive = false;
            IsSelectPoleActive = false;
            IsOpenOcalcActive = false;
            IsMovePoleActive = false;
            IsPLDStatusActive = false;
            MeasureTool.StatusBar = ConfigUtility.StatusBar;
            MeasureTool.IsActive = true;
            clearGraphic();
            isMeasureNewPoleActive = true;

            FeatureLayer PLCFeatureLayer = MapProperty.Layers[PLCLayerName] as FeatureLayer;
            if (PLCFeatureLayer != null)
            {
                if (PLCFeatureLayer.Visible == false)
                {
                    PLCFeatureLayer.Visible = true;
                }
            }


        }

        public void createPoleAfterMeasure(object sender, MouseButtonEventArgs e)
        {


            createNewPole(MapProperty.ScreenToMap(e.GetPosition(MapProperty)));
        }
        private void MapProperty_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (_clearAll == true)
                {
                    _layers.Add(new SegmentLayer());
                    MapProperty.Layers.Add(_layers.Last().Layer);
                    _clearAll = false;


                }
                else if (!_layers.Any())
                {
                    _layers.Add(new SegmentLayer());
                    MapProperty.Layers.Add(_layers.Last().Layer);
                }
                Point pt = e.GetPosition(null);




                if (Math.Abs(pt.X - _layers.Last().LastClick.X) < 2 && Math.Abs(pt.Y - _layers.Last().LastClick.Y) < 2)
                {


                    createNewPole(MapProperty.ScreenToMap(e.GetPosition(MapProperty)));
                    MapProperty.MouseLeftButtonDown -= new MouseButtonEventHandler(MapProperty_MouseLeftButtonDown);
                }
            }
            catch (Exception ex)
            {
                logger.Error("PLC Measure Create Pole Exception: " + ex.Message);
            }
        }

        private void pldbSearchBtn_Click(object sender, RoutedEventArgs e)
        {
            startPoleSearch();
        }

        // Pole Search Function
        private void startPoleSearch()
        {
            try
            {
                string pldbIdEntered = pldbIDTxt.Text;
                IsNewPoleActive = false;
                IsSelectPoleActive = false;
                IsOpenOcalcActive = false;
                IsDeletePoleActive = false;
                isMeasureNewPoleActive = false;
                IsPLDStatusActive = false;


                if (pldbIdEntered != "" && pldbIdEntered != null)
                {
                    bool isValidPldbId = validatePldbId(pldbIdEntered);
                    if (isValidPldbId)
                    {
                        executePoleSearch(pldbIdEntered);
                    }
                    else
                    {
                        MessageBox.Show("PLDBID should contains number only");
                    }
                }
                else
                {
                    MessageBox.Show("Please Enter PLDBID");
                }
            }
            catch (Exception ex)
            {
                logger.Error("PLC Search Pole Exception: " + ex.Message);
            }
        }

        // Validate Entered PLDBID
        private bool validatePldbId(string pldbid)
        {
            foreach (char c in pldbid)
            {
                if (c < '0' || c > '9')
                    return false;
            }

            return true;
        }


        private void executePoleSearch(string pldbid)
        {
            try
            {
                FeatureLayer PLCFeatureLayer = MapProperty.Layers[PLCLayerName] as FeatureLayer;
                if (PLCFeatureLayer.Visible == false)
                {
                    PLCFeatureLayer.Visible = true;
                }
                string url = _plcDynamicServiceUrl + "/" + _plcLayerId;

                Query _query = new Query();
                _query.SpatialRelationship = SpatialRelationship.esriSpatialRelIntersects;
                _query.ReturnGeometry = true;

                _query.OutFields.AddRange(new string[] { "*" });
                _query.Where = "PLDBID=" + Convert.ToDouble(pldbid);
                QueryTask _circuitqueryTask = new QueryTask(url);
                _circuitqueryTask.ExecuteCompleted += new EventHandler<QueryEventArgs>(_poleSearchqueryTask_ExecuteCompleted);
                _circuitqueryTask.Failed += new EventHandler<TaskFailedEventArgs>(_wipqueryTask_Failed);
                _circuitqueryTask.ExecuteAsync(_query);
            }
            catch (Exception ex)
            {
                logger.Error("PLC Search Pole Query Task Exception: " + ex.Message);
            }
        }

        void _poleSearchqueryTask_ExecuteCompleted(object sender, QueryEventArgs e)
        {
            try
            {
                if (e.FeatureSet.Features.Count > 0)
                {
                    var feature = e.FeatureSet.Features[0];
                    Envelope envelope = new Envelope();

                    envelope.XMax = e.FeatureSet.Features[0].Geometry.Extent.XMax + 200;
                    envelope.XMin = e.FeatureSet.Features[0].Geometry.Extent.XMin - 200;
                    envelope.YMax = e.FeatureSet.Features[0].Geometry.Extent.YMax + 200;
                    envelope.YMin = e.FeatureSet.Features[0].Geometry.Extent.YMin - 200;

                    MapProperty.ZoomTo(envelope);
                    AddGraphic(feature.Geometry);
                    //Fow Existing pole outside wip

                    SetPoleAttributeWinFields(PLCLayerName, MapProperty, _selectPoleGraphic, null, feature.Attributes["OBJECTID"], null);


                    if (feature.Attributes["ORDER_NUMBER"] != null && feature.Attributes["ORDER_NUMBER"].ToString() != "Null")
                    {
                        Dictionary<string, string> featureset = new Dictionary<string, string>();
                        featureset.Add(feature.Attributes["OBJECTID"].ToString(), feature.Attributes["ORDER_NUMBER"].ToString());
                        PLCAttributeWindow.cboOrderNo.ItemsSource = featureset;
                        PLCAttributeWindow.cboOrderNo.SelectedIndex = 0;
                        PLCAttributeWindow.cboOrderNo.IsEnabled = false;

                    }
                    else
                    {

                        Dictionary<string, string> featureset = new Dictionary<string, string>();
                        PLCAttributeWindow.cboOrderNo.ItemsSource = featureset;
                        PLCAttributeWindow.cboOrderNo.IsEnabled = false;

                    }
                    if (feature.Attributes["NOTIFICATION_NUMBER"] != null && feature.Attributes["NOTIFICATION_NUMBER"].ToString() != "Null")
                    {
                        PLCAttributeWindow.notificaionNoTxt.Text = feature.Attributes["NOTIFICATION_NUMBER"].ToString();
                    }
                    else
                    {
                        PLCAttributeWindow.notificaionNoTxt.Text = "";
                    }
                    if (feature.Attributes["MAINTENANCECODE"] != null && feature.Attributes["MAINTENANCECODE"] != "Null")
                    {
                        PLCAttributeWindow.matCodeTxt.Text = feature.Attributes["MAINTENANCECODE"].ToString();
                    }
                    else
                    {
                        PLCAttributeWindow.matCodeTxt.Text = "";
                    }
                    if (feature.Attributes["ORDER_DESCRIPTION"] != null && feature.Attributes["ORDER_DESCRIPTION"].ToString() != "Null")
                    {
                        PLCAttributeWindow.DescTxt.Text = feature.Attributes["ORDER_DESCRIPTION"].ToString();
                    }
                    else
                    {
                        PLCAttributeWindow.DescTxt.Text = "";
                    }
                    if (feature.Attributes["LANID"] != null && feature.Attributes["LANID"].ToString() != "Null")
                    {
                        PLCAttributeWindow.lanIdTxt.Text = feature.Attributes["LANID"].ToString();
                    }
                    else
                    {
                        PLCAttributeWindow.lanIdTxt.Text = "";
                    }
                    if (feature.Attributes["CREATED_DATE"] != null && feature.Attributes["CREATED_DATE"].ToString() != "Null")
                    {
                        PLCAttributeWindow.creationDateTxt.Text = feature.Attributes["CREATED_DATE"].ToString();
                    }
                    else
                    {
                        PLCAttributeWindow.creationDateTxt.Text = "";
                    }
                    if (feature.Attributes["LAT"] != null && feature.Attributes["LAT"].ToString() != "Null")
                    {
                        PLCAttributeWindow.latTxt.Text = feature.Attributes["LAT"].ToString();
                    }
                    else
                    {
                        PLCAttributeWindow.latTxt.Text = "";
                    }
                    if (feature.Attributes["LONGITUDE"] != null && feature.Attributes["LONGITUDE"].ToString() != "Null")
                    {
                        PLCAttributeWindow.longTxt.Text = feature.Attributes["LONGITUDE"].ToString();
                    }
                    else
                    {
                        PLCAttributeWindow.longTxt.Text = "";
                    }
                    if (feature.Attributes["PLDBID"] != null && feature.Attributes["PLDBID"].ToString() != "Null")
                    {
                        PLCAttributeWindow.poleNoTxt.Text = feature.Attributes["PLDBID"].ToString();
                    }
                    else
                    {
                        PLCAttributeWindow.poleNoTxt.Text = "";
                    }
                    if (feature.Attributes["SAPEQUIPID"] != null && feature.Attributes["SAPEQUIPID"].ToString() != "Null")
                    {
                        PLCAttributeWindow.sapIdTxt.Text = feature.Attributes["SAPEQUIPID"].ToString();
                    }
                    else
                    {
                        PLCAttributeWindow.sapIdTxt.Text = "";
                    }
                    if (feature.Attributes["SKETCH_LOC"] != null && feature.Attributes["SKETCH_LOC"].ToString() != "Null")
                    {
                        PLCAttributeWindow.sketchLocTxt.Text = feature.Attributes["SKETCH_LOC"].ToString();
                    }
                    else
                    {
                        PLCAttributeWindow.sketchLocTxt.Text = "";
                    }
                    if (feature.Attributes["COPY_PLD"] != null && feature.Attributes["COPY_PLD"].ToString() != "Null")
                    {
                        PLCAttributeWindow.copyPoleNoTxt.Text = feature.Attributes["COPY_PLD"].ToString();
                    }
                    else
                    {
                        PLCAttributeWindow.copyPoleNoTxt.Text = "";
                    }
                    if (feature.Attributes["SNOW_LOAD_DIST"] != null && feature.Attributes["SNOW_LOAD_DIST"].ToString() != "Null")
                    {
                        PLCAttributeWindow.snowLoadTxt.Text = feature.Attributes["SNOW_LOAD_DIST"].ToString();
                    }
                    else
                    {
                        PLCAttributeWindow.snowLoadTxt.Text = "";
                    }
                    if (feature.Attributes["ELEVATION"] != null && feature.Attributes["ELEVATION"].ToString() != "Null")
                    {
                        PLCAttributeWindow.ElevationTxt.Text = feature.Attributes["ELEVATION"].ToString();
                    }
                    else
                    {
                        PLCAttributeWindow.ElevationTxt.Text = "";
                    }

                    //DA# 200103 ME Q1 2020 - START
                    PLCAttributeWindow.SelectedPoleGeom = feature.Geometry;
                    //DA# 200103 ME Q1 2020 - END

                    PLCAttributeWindow.Show();
                    PLCAttributeWindow.CopyButton.IsEnabled = false;
                    PLCAttributeWindow.PLCBusyIndicator.IsBusy = false;
                    PLCAttributeWindow.OKButton.IsEnabled = false;
                    PLCAttributeWindow.sketchLocTxt.IsEnabled = false;

                }
                else
                {
                    MessageBox.Show("No Pole Found");
                }
            }
            catch (Exception ex)
            {
                logger.Error("PLC Pole Search Result Exception: " + ex.Message);
            }
        }

        //Event handler for check of PLCINFO toggle layer visibility checkbox
        private void PLCINFOLayerVisibleToggleButton_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                if (ConfigUtility.StatusBar != null)
                    ConfigUtility.StatusBar.Text = string.Empty;
                if (MapProperty != null)
                {
                    Layer PLCLayer = MapProperty.Layers[_plcLayerName];
                    if (PLCLayer != null)
                    {
                        PLCLayer.Visible = true;

                    }

                }
            }
            catch (Exception ex)
            {
                logger.Error("PLC PLCINFO ON/OFF Exception: " + ex.Message);
            }
        }

        //Event handler for uncheck of PLCINFO toggle layer visibility checkbox
        private void PLCINFOLayerVisibleToggleButton_Unchecked(object sender, RoutedEventArgs e)
        {
            try
            {
                if (ConfigUtility.StatusBar != null)
                    ConfigUtility.StatusBar.Text = string.Empty;
                if (MapProperty != null)
                {
                    Layer PLCLayer = MapProperty.Layers[_plcLayerName];
                    if (PLCLayer != null)
                    {
                        PLCLayer.Visible = false;

                    }

                }
            }
            catch (Exception ex)
            {
                logger.Error("PLC PLCINFO Layer ON/OFF Exception: " + ex.Message);
            }
        }

        // Activate PLDBID Search on Enter Key Press
        private void pldbIDTxt_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key.Equals(Key.Enter))
            {
                startPoleSearch();
            }
        }


        // Show PLD Status Tool Code Starts

        private void Show_PLD_Status_Click(object sender, RoutedEventArgs e)
        {
            ConfigUtility.StatusBar.Text = string.Empty;
            if (ShowPLDStatus.IsChecked == true)
            {
                IsPLDStatusActive = true;
                isDelete = false;
                clearGraphic();
            }
            else
            {
                clearGraphic();
                IsPLDStatusActive = false;
            }
        }

        public static readonly DependencyProperty IsActivePLDStatusProperty = DependencyProperty.Register(
         "IsPLDStatusActive",
         typeof(bool),
         typeof(PLCWidget),
         new PropertyMetadata(OnIsPLDStatusActiveChanged));

        private static void OnIsPLDStatusActiveChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (PLCWidget)d;
            isPLDStatusCurrentlyActive = (bool)e.NewValue;
            if (isPLDStatusCurrentlyActive)
            {
                EventAggregator.GetEvent<ControlActivatedEvent>().Publish(control);
                if (control.MapProperty == null) return;
                CursorSet.SetID(control.MapProperty, "Hand");
            }
            else
            {
                string currentMapCursor = CursorSet.GetID(control.MapProperty);
                if (currentMapCursor.Contains("Hand") == true && isSelectPoleCurrentlyActive == false && isOpenOcalcCurrentlyActive == false && isMovePoleCurrentlyActive == false)
                    CursorSet.SetID(control.MapProperty, "Arrow");
            }
        }

        public bool IsPLDStatusActive
        {
            get
            {
                return (bool)GetValue(IsActivePLDStatusProperty);
            }
            set
            {
                SetValue(IsActivePLDStatusProperty, value);
                if (IsPLDStatusActive)
                {
                    IsOpenOcalcActive = false;
                    IsSelectPoleActive = false;
                    IsNewPoleActive = false;
                    IsDeletePoleActive = false;
                    IsMovePoleActive = false;
                    isMeasureNewPoleActive = false;
                    MeasureTool.IsActive = false;
                    if (_measureActionObj != null)
                    {
                        _measureActionObj.ClearLayers();
                    }

                    if (MapProperty != null)
                    {
                        MapProperty.MouseClick += new EventHandler<ESRI.ArcGIS.Client.Map.MouseEventArgs>(Map_MouseClickPLDStatus);
                        FeatureLayer PLCFeatureLayer = MapProperty.Layers[PLCLayerName] as FeatureLayer;
                        if (PLCFeatureLayer.Visible == false)
                        {
                            PLCFeatureLayer.Visible = true;
                        }
                    }
                }
                else
                {
                    if (MapProperty != null)
                        MapProperty.MouseClick -= new EventHandler<ESRI.ArcGIS.Client.Map.MouseEventArgs>(Map_MouseClickPLDStatus);
                }

                ShowPLDStatus.IsChecked = value;
            }
        }

        private void Map_MouseClickPLDStatus(object sender, Map.MouseEventArgs e)
        {
            checkPolePlacedInWIP(e.MapPoint, "PLDStatus");
        }

        private void wipPLDStatus_identifyTask_ExecuteCompleted(object sender, IdentifyEventArgs e)
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
                            callGetZOrderData(Convert.ToString(e.IdentifyResults[0].Feature.Attributes["PM Order Number"]), e.IdentifyResults[0].Feature.Geometry);
                            
                        }

                    }
                    else
                    {
                        MessageBox.Show("Please select a WIP to display Poles PLD Status");
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error("PLD Status Tool: Exception in query task " + ex.Message);
            }
        }

        private void callGetZOrderData(string wipOrderNumber, ESRI.ArcGIS.Client.Geometry.Geometry wipGeometry)
        {
            string queryStringSuffix = "?orderNumber=" + Convert.ToString(wipOrderNumber);// "35012096";

            string prefixUrl = GetPrefixUrl();
            UriBuilder uriBuilder = new UriBuilder(prefixUrl + _getZOrderDataServiceUrl + queryStringSuffix);
            WebClient client = new WebClient();

            client.DownloadStringCompleted += (s, args) =>
            {
                try
                {
                    GetZOrderData result = null;
                    _PLDPoleData = null;
                    result = JsonHelper.Deserialize<GetZOrderData>(args.Result);
                    if (result != null && result.Count > 0)
                    {
                        _PLDPoleData = result;
                        getPLCINFOPolesInsideWIP(wipGeometry);
                    }
                    else
                    {
                        MessageBox.Show("No Poles present in PLDB for Order Number: " + wipOrderNumber);
                    }
                }
                catch (Exception)
                {
                    
                }
            };
            client.DownloadStringAsync(uriBuilder.Uri);


        
        }

        private void getPLCINFOPolesInsideWIP(ESRI.ArcGIS.Client.Geometry.Geometry wipGeometry)
        {
            string url = _plcDynamicServiceUrl + "/" + _plcLayerId;
            Query _query = new Query();
            _query.SpatialRelationship = SpatialRelationship.esriSpatialRelContains;
            _query.ReturnGeometry = true;
            _query.OutFields.AddRange(new string[] { "PLDBID" });
            _query.Geometry = wipGeometry;

            QueryTask _circuitqueryTask = new QueryTask(url);
            _circuitqueryTask.ExecuteCompleted += new EventHandler<QueryEventArgs>(_PLCQueryTask_ExecuteCompleted);
            _circuitqueryTask.Failed += new EventHandler<TaskFailedEventArgs>(_PLCQueryTask_Failed);
            _circuitqueryTask.ExecuteAsync(_query);
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
       
        void _PLCQueryTask_ExecuteCompleted(object sender, QueryEventArgs e)
        {
            try
            {
                if (e.FeatureSet.Features.Count > 0)
                {
                    _pldPoleDic = new Dictionary<ulong, ZOrderDataAttr>();
                    for (int j = 0; j < e.FeatureSet.Features.Count; j++)
                    {

                        for (int i = 0; i < _PLDPoleData.Count; i++)
                        {
                            if (Convert.ToUInt64(e.FeatureSet.Features[j].Attributes["PLDBID"]) == _PLDPoleData.SAPInfos[i].PLDBID)
                            {
                                _pldPoleDic.Add(_PLDPoleData.SAPInfos[i].PLDBID, _PLDPoleData.SAPInfos[i]);
                            }
                        }
                    }
                    CreatePoleStatusLabel(e.FeatureSet);
                }
                else
                {
                    MessageBox.Show("No Pole found inside WIP");
                }
            }
            catch (Exception ex)
            {
                logger.Error("PLD Status Tool: Exception in query task " + ex.Message);
            }
        }

        void _PLCQueryTask_Failed(object sender, TaskFailedEventArgs e)
        {
            logger.Error("PLC PLD Status Query Task has failed: " + e.Error);

        }

        void CreatePoleStatusLabel(FeatureSet poleFeatureset)
        {
            try
            {
                GraphicsLayer layer = MapProperty.Layers[PoleGraphicID] as GraphicsLayer;
                if (layer == null)
                {
                    layer = CreatePoleGraphicLayer();
                    MapProperty.Layers.Add(layer);
                }
                layer.Visible = true;
                layer.Graphics.Clear();

                for (int i = 0; i < poleFeatureset.Features.Count; i++)
                {
                    
                    string labelText = String.Empty;
                    if (_pldPoleDic.ContainsKey(Convert.ToUInt64(poleFeatureset.Features[i].Attributes["PLDBID"])))
                    {

                        labelText += _pldPoleDic[Convert.ToUInt64(poleFeatureset.Features[i].Attributes["PLDBID"])].PLDBID;
                        labelText += Environment.NewLine;
                        labelText += _pldPoleDic[Convert.ToUInt64(poleFeatureset.Features[i].Attributes["PLDBID"])].PGE_SketchLocation;
                        labelText += Environment.NewLine;
                        labelText += _pldPoleDic[Convert.ToUInt64(poleFeatureset.Features[i].Attributes["PLDBID"])].PGE_Status;

                        //Size _textSize = GetTextSize(labelText, new System.Windows.Media.FontFamily("Arial"), 25);
                        TextSymbol _textSymbol = new TextSymbol()
                        {
                            FontFamily = new System.Windows.Media.FontFamily("Arial"),
                            Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Red),
                            FontSize = 15,
                            Text = labelText,
                            OffsetX = 10,
                            OffsetY = -10
                        };

                        Graphic graphicText = new Graphic()
                        {
                            Geometry = poleFeatureset.Features[i].Geometry,
                            Symbol = _textSymbol,
                        };



                        layer.Graphics.Add(graphicText);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error("PLD Status Tool: Issue in Creating Graphics " + ex.Message);
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

                _ocalcV6IdentifyFlag++;
                if (_ocalcV6IdentifyFlag == 2)
                    RedirectOCalcV6URI();
            }
            catch (Exception err)
            {
                logger.Error("O-Calc V6 WIP service issue" + err.Message);
            }
        }

        private void OCalcWip_identifyTask_Failed(object sender, TaskFailedEventArgs e)
        {
            logger.Error("Identify on WIP Service Failed for O-Calc: " + e.Error);
            ConfigUtility.StatusBar.Text = "Identify on WIP Service Failed for O-calc. " + e.Error.Message;
        }

        private void RedirectOCalcV6URI()
        {
            if (_ocalcV6PMOrderNotificNoList.Count == 0)     //no WIP at selected Pole
            {
                HtmlPage.Window.Navigate(new Uri(("PLDB://LD/LaunchByPLDBID/" + _selectedPLDBIDOcalcV6 + "?")), "_blank");
            }

            else if (_ocalcV6PMOrderNotificNoList.Count == 1)     //one WIP is present at selcted pole
            {
                string confirmationMsg = "You are about to open Line Design in O-Calc for\nPDLBID " + _selectedPLDBIDOcalcV6 + " and PM Order " + Convert.ToString(_ocalcV6PMOrderNotificNoList.Keys.ToList()[0]) + ".\nDo you want to proceed?";
                MessageBoxResult messageResult = MessageBox.Show(confirmationMsg, "Confirmation", MessageBoxButton.OKCancel);
                if (messageResult == MessageBoxResult.OK)
                {
                    if(Convert.ToString(_ocalcV6PMOrderNotificNoList.ToList()[0].Value) != "")
                        HtmlPage.Window.Navigate(new Uri(("PLDB://LD/LaunchByPLDBID/" + _selectedPLDBIDOcalcV6 + "?PMOrderNumber=" + Convert.ToString(_ocalcV6PMOrderNotificNoList.ToList()[0].Key) + "&NotificationNumber=" + Convert.ToString(_ocalcV6PMOrderNotificNoList.ToList()[0].Value))), "_blank");
                    else
                        HtmlPage.Window.Navigate(new Uri(("PLDB://LD/LaunchByPLDBID/" + _selectedPLDBIDOcalcV6 + "?PMOrderNumber=" + Convert.ToString(_ocalcV6PMOrderNotificNoList.ToList()[0].Key))), "_blank");
                }
            }

            else if (_ocalcV6PMOrderNotificNoList.Count > 1)     //more than one WIPs are present at selcted pole
            {
                OpenPMOrderSelectionWindow(_selectedPLDBIDOcalcV6, _ocalcV6PMOrderNotificNoList);
            }
        }

        public void OpenPMOrderSelectionWindow(string selectedPLDBIDOcalcV6, Dictionary<string, string> ocalcV6PMOrderNotificNoList)
        {
             _ocalcWIPSelectionControl.SetWIPSelectionData(_selectedPLDBIDOcalcV6, _ocalcV6PMOrderNotificNoList);
             _ocalcWIPSelectionControl.Show();
        }
        //DA# 200103 ME Q1 2020 - END

    }
}
