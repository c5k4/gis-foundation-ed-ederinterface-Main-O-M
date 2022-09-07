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
using System.Text.RegularExpressions;
using System.ComponentModel.DataAnnotations;
using System.Windows.Data;
using System.ComponentModel;
using System.IO;
using System.Xml.Linq;
using System.ServiceModel;
using System.Net;
using System.Text;
using System.Windows.Browser;
using ESRI.SilverlightViewer.Controls;
using System.Windows.Threading;

namespace ArcFMSilverlight
{
    public partial class AddDataControl : UserControl
    {
        #region Variables

        private Logger logger = LogManager.GetCurrentClassLogger();
        private Map _map;
        private string _uploadUrlSHP;
        private string _uploadUrlKML;
        private string _geoprocessingTaskUrlSHP;
        private string _geoprocessingTaskUrlKML;
        public List<TempLayer> TempLayerList;
        private static string KML = "kml";
        private static string KMZ = "kmz";
        private static string CSV = "csv";
        private static string ZIP = "zip";
        private Stream _fileData = null;
        private Geoprocessor _geometryUploaderGeoprocessingTask;
        private bool _isAsync;
        private string csv_latitude= null;
        private string csv_longitude= null;
        private int _GPServiceLimit;
        private string _serviceLayerLabel = null;
        private string _fileExtensionLower = null;
        private string _uploadFileHandler;
        private string _uploadedCSVURLNew;
        private string _geometryService;
        private int _uploadPartSizeMB;
        private MapTools _mapTools;
        #endregion Variables

        #region Constructor

        public AddDataControl(Map map, XElement addDataElement, MapTools tools)
        {
            try
            {
                InitializeComponent();
                _map = map;
                _mapTools = tools;

                lblWarning.Text = addDataElement.Element("Warning").Attribute("Message").Value.ToString();
                _geometryService = addDataElement.Element("GeometryService").Attribute("Url").Value.ToString();
                _GPServiceLimit = Convert.ToInt16(addDataElement.Element("GPServiceLimit").Attribute("Value").Value.ToString());
                _uploadPartSizeMB = Convert.ToInt32(addDataElement.Element("UploadPartSizeMB").Attribute("Value").Value.ToString());
                ReadConfig();
            }
            catch
            {
            }
        }

        #endregion Constructor

        #region Events
        private void TxtMapServiceUrl_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Key == Key.Enter)
                    AddServiceOnMap();
            }
            catch 
            {
                busyIndicator.IsBusy = false;
                MessageBox.Show("Error in adding Service on map. Please try again.", "Message", MessageBoxButton.OK);
            }
        }

        private void AddServiceBtn_Click(object sender, RoutedEventArgs e)
        {
            AddServiceOnMap();
        }

        private void AddFileBtn_Click(object sender, RoutedEventArgs e)
        {
            ConfigUtility.StatusBar.Text = "";
            HandleGeometryUpload();
        }

        private void btnBrowse_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                OpenFileDialog dlg = new OpenFileDialog();
                dlg.Multiselect = false;
                dlg.Filter = "Document Files|*.kml;*.kmz;*.zip;*.csv";
                
                bool? retval = dlg.ShowDialog();
                if (retval != null && retval == true)
                {
                    string ext = dlg.File.Extension.ToString().Split('.')[1].ToLower();
                    if (!(ext == KML || ext == KMZ || ext == CSV || ext == ZIP))
                    {
                        MessageBox.Show("File type should be KML, CSV or ZIP", "Message", MessageBoxButton.OK);
                        return;
                    }
                    _fileData = dlg.File.OpenRead();
                   
                    TxtUploadFileName.Text = dlg.File.Name.ToString();
                }                
            }

            catch (Exception ex)
            {
                busyIndicator.IsBusy = false;
                MessageBox.Show("Error in uploading file. Please try again", "Message", MessageBoxButton.OK);
            }
        }

        private void HandleGeoprocessingKML_JobCompleted(object sender, JobInfoEventArgs e) {

            try{
                if (e != null && e.JobInfo != null && e.JobInfo.JobId != null && e.JobInfo.JobStatus.ToString() == "esriJobSucceeded")
                {
                    ArcGISDynamicMapServiceLayer newKmlLayer = _geometryUploaderGeoprocessingTask.GetResultMapServiceLayer(e.JobInfo.JobId.ToString());
                    newKmlLayer.ImageFormat =ArcGISDynamicMapServiceLayer.RestImageFormat.PNG8;
                    newKmlLayer.Opacity = 0.8;
                    newKmlLayer.ID = _serviceLayerLabel;

                    //initialize layer after 2 sec
                    DispatcherTimer initializeDispatcherTimer = new DispatcherTimer();
                    initializeDispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, 2000); // Milliseconds
                    initializeDispatcherTimer.Tick += (s, ev) => InitializeKMLLayer(s, newKmlLayer, _serviceLayerLabel);
                    initializeDispatcherTimer.Start();
                    
                }
                else
                {
                    busyIndicator.IsBusy = false;
                    MessageBox.Show("Error in uploading file.", "Message", MessageBoxButton.OK);
                }
            }
            catch (Exception ex) {
                busyIndicator.IsBusy = false;
                MessageBox.Show("Error in uploading file.", "Message", MessageBoxButton.OK);
            }
            _geometryUploaderGeoprocessingTask = null;
        }

        private void InitializeKMLLayer(object sender, ArcGISDynamicMapServiceLayer kmlLayer, string label)
        {
            ((DispatcherTimer)sender).Stop();

            kmlLayer.Initialized += (s, ev) => KmlLayer_Initialized(s, _serviceLayerLabel);
            kmlLayer.InitializationFailed += (s, ev) => KmlLayer_InitializationFailed();
            kmlLayer.Initialize();
        }

        private void KmlLayer_InitializationFailed()
        {
            busyIndicator.IsBusy = false;
            MessageBox.Show("Unable to process KML File.", "Message", MessageBoxButton.OK);
        }

        private void KmlLayer_Initialized(object sender, string label)
        {
            try
            {
                ArcGISDynamicMapServiceLayer myKmlLayer = (ArcGISDynamicMapServiceLayer)sender;

                if (myKmlLayer.InitializationFailure == null)
                {
                    //get extent to zoom
                   List<Graphic> graphicList = new List< Graphic>();
                    Graphic graphic = new Graphic(){ Geometry = myKmlLayer.InitialExtent};
                    graphicList.Add(graphic);
                    var geometryService = new GeometryService(_geometryService);
                    geometryService.Failed += new EventHandler<ESRI.ArcGIS.Client.Tasks.TaskFailedEventArgs>(GeomteryService_Failed);
                    geometryService.ProjectCompleted += (s, e) => GeometryService_GetKMLExtent(e, myKmlLayer, label);
                    geometryService.ProjectAsync(graphicList, _map.SpatialReference);

                }
            }
            catch (Exception ex)
            {
                busyIndicator.IsBusy = false;
                MessageBox.Show("Error in uploading file. Please try again", "Message", MessageBoxButton.OK);
            }

        }

        private void GeometryService_GetKMLExtent(GraphicsEventArgs e, ArcGISDynamicMapServiceLayer kmlLayer, string label)
        {
            if (e != null && e.Results != null && e.Results.Count > 0)
            {
                _map.Layers.Add(kmlLayer);

                if (e.Results[0].Geometry.Extent != null)
                    _map.ZoomTo(e.Results[0].Geometry.Extent.Expand(1.25));

                busyIndicator.IsBusy = false;
                ConfigUtility.StatusBar.Text = "KML Uploaded Successfully.";
                TxtUploadFileLbl.Text = "";
                csv_latitude = null;
                csv_longitude = null;
                TempLayer tl = new TempLayer();
                tl.title = label + "-(Layer)";
                tl.layer = kmlLayer;
                TempLayerList.Add(tl);

                //update TOC - Toggle layers
                _mapTools.LayerControl.AddLayerToTOC(_mapTools.LayerControl, _map, kmlLayer.ID);
            }
            else
            {
                busyIndicator.IsBusy = false;
                MessageBox.Show("Error in uploading file. Please try again", "Message", MessageBoxButton.OK);
            }
        }
      
        //executed when gp service execution for shp file is complete
        private void HandleGeoprocessing_ExecuteCompleted(object sender, GPExecuteCompleteEventArgs e) {
            try
            {
                if (e != null && e.Results != null && e.Results.OutParameters != null && e.Results.OutParameters.Count > 0 && 
                    (e.Results.OutParameters[0] as GPFeatureRecordSetLayer) != null && (e.Results.OutParameters[0] as GPFeatureRecordSetLayer).FeatureSet != null &&
                (e.Results.OutParameters[0] as GPFeatureRecordSetLayer).FeatureSet.Features != null)
                {
                    GraphicsLayer graphicsLayer;
                    FeatureSet featureSet = (e.Results.OutParameters[0] as GPFeatureRecordSetLayer).FeatureSet;
                    int count = featureSet.Features.Count;
                    
                    //bool transferLimit = false;// featureSet.Features.response[0].value.exceededTransferLimit;
                    if (count > 0)
                    {
                        graphicsLayer = new GraphicsLayer() { ID = _serviceLayerLabel };
                        _map.Layers.Add(graphicsLayer);
                        Envelope featureExtent = GetGeometry(featureSet);
                        foreach (Graphic graphic in featureSet)
                        {
                            Symbol symbol;
                            if (graphic.Geometry is MapPoint)
                            {
                                symbol = new ESRI.ArcGIS.Client.Symbols.SimpleMarkerSymbol()
                                {
                                    Color = new SolidColorBrush(Color.FromArgb(128, 255, 0, 0)),
                                    Size = 20,
                                    Style = ESRI.ArcGIS.Client.Symbols.SimpleMarkerSymbol.SimpleMarkerStyle.Circle
                                };                  
                            }
                            else if (graphic.Geometry is Polyline)
                            {
                                symbol = new SimpleLineSymbol
                                {
                                    Color = new SolidColorBrush(Color.FromArgb(255, 255, 0, 0)),
                                    Width = 3,
                                    Style = SimpleLineSymbol.LineStyle.Solid
                                };
                            }
                            else if (graphic.Geometry is Polygon || graphic.Geometry is Envelope)
                            {
                                symbol = new SimpleFillSymbol
                                {
                                    Fill = new SolidColorBrush(Color.FromArgb(64, 255, 255, 0)),
                                    BorderBrush = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0)),
                                    BorderThickness = 2
                                };
                            }
                            else
                            {
                                throw new Exception("Invalid geometry type");
                            }
                            if (symbol != null)
                            {
                                //maptip
                                graphic.MapTip = CreateTipWindow(graphic.Attributes, _serviceLayerLabel);

                                graphic.Symbol = symbol;
                                graphicsLayer.Graphics.Add(graphic);
                            }
                        }
                        if (featureExtent != null)
                        {
                            _map.ZoomTo(featureExtent.Expand(1.25));
                        }
                        busyIndicator.IsBusy = false;
                        ConfigUtility.StatusBar.Text = "SHP Uploaded Successfully.";
                        _geometryUploaderGeoprocessingTask = null;
                        TxtUploadFileLbl.Text = "";
                        csv_latitude = null;
                        csv_longitude = null;
                        busyIndicator.IsBusy = false;
                        TempLayer tl = new TempLayer();
                        tl.title = _serviceLayerLabel + "-(Layer)";
                        tl.layer = graphicsLayer;
                        TempLayerList.Add(tl);

                        //update TOC - Toggle layers
                        _mapTools.LayerControl.AddLayerToTOC(_mapTools.LayerControl, _map, graphicsLayer.ID);
                    }
                    else if (count == 0)
                    {
                        busyIndicator.IsBusy = false;
                        MessageBox.Show("Record count for the uploaded file should be greater than 0 and less than or equal to " + _GPServiceLimit + ".", "Message", MessageBoxButton.OK);

                    }
                }
                else
                {
                    busyIndicator.IsBusy = false;
                     MessageBox.Show("No Data available in uploaded File.", "Message", MessageBoxButton.OK);
                }
            }
            catch(Exception ex)
            {
                busyIndicator.IsBusy = false;
                 MessageBox.Show("Error in uploading file.", "Message", MessageBoxButton.OK);
            }
        }
     
        //gp service execution for shp or kml file fails
        private void GeometryGeoprocessing_Failed(object sender, TaskFailedEventArgs e)  {
            busyIndicator.IsBusy = false;
            MessageBox.Show("Error in uploading file.", "Message", MessageBoxButton.OK);
        }

        #endregion Events

        #region private methods

        private void ReadConfig()
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
                        if (element.Name.LocalName == "SAPRWNotification")
                        {
                            if (element.HasElements)
                            {
                                foreach (XElement childelement in element.Elements())
                                {

                                    if (childelement.Name.LocalName == "UploadFileUrl")
                                    {
                                        attribute = childelement.Attribute("url").Value;
                                        if (attribute != null)
                                        {
                                            _uploadFileHandler = attribute;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                ClearAppResource("AddDataUrls");
                if (Application.Current.Host.InitParams.ContainsKey("TemplatesConfig"))
                {
                    string config = Application.Current.Host.InitParams["TemplatesConfig"];
                    config = HttpUtility.HtmlDecode(config).Replace("***", ",");
                    XElement xe = XElement.Parse(config).Element("AddDataUrls");

                    if (!xe.HasElements) return;
                    string baseURL = xe.Attribute("UrlPrefix").Value.ToString();
                    _uploadUrlSHP = baseURL + xe.Element("UploadUrlSHP").Attribute("Url").Value.ToString();
                    _uploadUrlKML = baseURL + xe.Element("UploadUrlKML").Attribute("Url").Value.ToString();
                    _geoprocessingTaskUrlSHP = baseURL + xe.Element("GeoprocessingTaskUrlSHP").Attribute("Url").Value.ToString();
                    _geoprocessingTaskUrlKML = baseURL + xe.Element("GeoprocessingTaskUrlKML").Attribute("Url").Value.ToString(); ;
                    _uploadedCSVURLNew = xe.Element("UploadedCSVURLNew").Attribute("Value").Value.ToString();
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
        }

        private void ClearAppResource(string resourceName)
        {
            if (Application.Current.Resources.Contains(resourceName))
                Application.Current.Resources.Remove(resourceName);
        }

        private void AddServiceOnMap()
        {
            ConfigUtility.StatusBar.Text = "";
            string label = TxtMapServiceLbl.Text.ToString().Trim();
            if (IsValidLabel(label))
            {
                //Check for Duplicate
                if (ChkDuplicate(TempLayerList, label) == true)
                {
                    MessageBox.Show("Duplicate Service Name");
                    return;
                }
                try
                {
                    if (label != null && label != "")
                    {
                        string serviceUrl = TxtMapServiceUrl.Text.ToString().Trim();
                        if (serviceUrl != null && serviceUrl != "")
                        {
                            if (!this.CheckDuplicateService(serviceUrl))
                            {
                                busyIndicator.IsBusy = true;
                                busyIndicator.BusyContent = "Adding Service.......";
                                if (serviceUrl.IndexOf("MapServer") > -1)
                                {
                                    ArcGISDynamicMapServiceLayer layer = new ArcGISDynamicMapServiceLayer { ID = label, Url = serviceUrl, DisableClientCaching = true, Opacity = 0.8, ImageFormat = ArcGISDynamicMapServiceLayer.RestImageFormat.PNG8 };
                                    layer.Initialized += new EventHandler<EventArgs>(Service_Initialized);
                                    layer.InitializationFailed += new EventHandler<EventArgs>(Service_InitializationFailure);
                                    layer.Initialize();
                                }

                                else if (serviceUrl.IndexOf("ImageServer") > -1)
                                {
                                    ArcGISImageServiceLayer imageServiceLayer = new ArcGISImageServiceLayer { Url = serviceUrl, ID = label, Opacity = 0.75 };

                                    imageServiceLayer.ImageFormat = ArcGISImageServiceLayer.ImageServiceImageFormat.PNG8;
                                    imageServiceLayer.NoData = 0;

                                    imageServiceLayer.Initialized += new EventHandler<EventArgs>(Service_Initialized);
                                    imageServiceLayer.Initialize();
                                }

                                else if (serviceUrl.IndexOf("FeatureServer") > -1)
                                {
                                    FeatureLayer featureLayer = new FeatureLayer();
                                    featureLayer.Url = serviceUrl;
                                    featureLayer.ID = label;

                                    featureLayer.Initialized += new EventHandler<EventArgs>(Service_Initialized);
                                    featureLayer.Initialize();
                                }
                                else
                                {
                                    busyIndicator.IsBusy = false;
                                    MessageBox.Show("Service type should be Map Service, Feature Service or Image Service.");
                                }
                            }
                        }
                        else
                        {
                            busyIndicator.IsBusy = false;
                            MessageBox.Show("Enter service URL");
                        }
                    }
                    else
                    {
                        busyIndicator.IsBusy = false;
                        MessageBox.Show("Enter service Name");
                    }
                }
                catch (Exception ex)
                {
                    busyIndicator.IsBusy = false;
                    MessageBox.Show("Failed to process REST URL");
                }
            }
        }
        private void Service_InitializationFailure(object sender, EventArgs e)
        {
            //to handle service failure
        }
        
        private void Service_Initialized(object sender, EventArgs e)
        {
            try
            {
                Layer layer = sender as Layer;
                if (layer.InitializationFailure == null)
                {
                    _map.Layers.Insert(_map.Layers.Count, layer);
                    logger.Info("Add Data - Layer Added: " + layer.ID);
                    TempLayer tl = new TempLayer();
                    tl.title = TxtMapServiceLbl.Text.ToString().Trim() + "-(Service)";
                    tl.layer = layer;
                    TempLayerList.Add(tl);
                    busyIndicator.IsBusy = false;
                    ConfigUtility.StatusBar.Text = "Service added : " + TxtMapServiceLbl.Text.ToString().Trim();
                    TxtMapServiceLbl.Text = "";
                    TxtMapServiceUrl.Text = "";

                    //update TOC - Toggle layers
                    _mapTools.LayerControl.AddLayerToTOC(_mapTools.LayerControl, _map, layer.ID);
                }
                else // case of invalid url
                {
                    busyIndicator.IsBusy = false;
                    MessageBox.Show("Failed to process REST URL");
                }
            }
            catch (Exception ex)
            {
                busyIndicator.IsBusy = false;
                MessageBox.Show("Failed to process REST URL");
            }
        }

        private bool IsValidLabel(string inputLabel)
        {
            Regex rxAllowed = new Regex("'^[0-9a-zA-Z-_ ]+$'", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);

            Match m = Regex.Match(inputLabel, @"'^[0-9a-zA-Z-_ ]+$'", RegexOptions.IgnoreCase);
            if (m.Success)
            {
                MessageBox.Show("Label '" + inputLabel + "' contains special characters." + "\r\n" + "Please avoid special characters in Label: '^[0-9a-zA-Z-_ ]+$'. ", "Message", MessageBoxButton.OK);
                return false;
            }
            else
                return true;
        }

        private bool ChkDuplicate(List<TempLayer> list, string inputLabel)
        {
            foreach (TempLayer tl in list)
            {
                if (tl.title.ToUpper().Substring(0,tl.title.ToUpper().LastIndexOf('-')) == inputLabel.ToUpper())
                    return true;
            }
            return false;
        }

        // This method checks duplicate service
        private bool CheckDuplicateService(string url)
        {
            if (_map != null)
            {
                foreach (Layer layer in _map.Layers) {
                    if (layer is ArcGISDynamicMapServiceLayer)
                    {
                        if ((layer as ArcGISDynamicMapServiceLayer).Url.ToString() == url)
                        {
                            busyIndicator.IsBusy = false;
                            MessageBox.Show("Duplicate Service URL", "Message", MessageBoxButton.OK);
                            return true;
                        }
                    }
                    else if (layer is ArcGISImageServiceLayer)
                    {
                        if ((layer as ArcGISImageServiceLayer).Url.ToString() == url)
                        {
                            busyIndicator.IsBusy = false;
                            MessageBox.Show("Duplicate Service URL", "Message", MessageBoxButton.OK);
                            return true;
                        }
                    }
                    else if (layer is FeatureLayer)
                    {
                        if ((layer as FeatureLayer).Url.ToString() == url)
                        {
                            busyIndicator.IsBusy = false;
                            MessageBox.Show("Duplicate Service URL", "Message", MessageBoxButton.OK);
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        // This method uploads CSV, shape file or KML file
        private void HandleGeometryUpload() {

            try {
                if (IsValidLabel(TxtUploadFileLbl.Text))
                {
                    if (TxtUploadFileName.Text == null || TxtUploadFileName.Text == "" || _fileData == null)
                    {
                        MessageBox.Show("Please upload file");
                        return;
                    }
                    if (TxtUploadFileName.Text.Split('.').Length < 1 || (TxtUploadFileName.Text.Split('.').Last().ToLower() != KML && TxtUploadFileName.Text.Split('.').Last().ToLower() != KMZ && TxtUploadFileName.Text.Split('.').Last().ToLower() != ZIP && TxtUploadFileName.Text.Split('.').Last().ToLower() != CSV))
                    {
                        MessageBox.Show("File type should be KML, CSV or ZIP", "Message", MessageBoxButton.OK);
                        return;
                    }
                     _serviceLayerLabel = TxtUploadFileLbl.Text;
                    if (_serviceLayerLabel == null || _serviceLayerLabel == "")
                    {
                        MessageBox.Show("Please enter Layer Name", "Message", MessageBoxButton.OK);
                        return;
                    }

                    //check for duplicate
                    if (TempLayerList.Count > 0)
                    {
                        if (ChkDuplicate(TempLayerList, _serviceLayerLabel) == true)
                        {
                            MessageBox.Show("Duplicate Layer Name");
                            return;
                        }
                    }

                    _fileExtensionLower = getExt(TxtUploadFileName.Text);
                    if (_fileExtensionLower == ZIP)    //case of shape file upload
                    {
                        if (_uploadUrlSHP != "")
                        {
                            //upload file
                            busyIndicator.IsBusy = true;
                            busyIndicator.BusyContent = "Validating Files .......";
                            upLoadFile(_uploadUrlSHP);
                        }
                         else {
                             busyIndicator.IsBusy = false;
                             MessageBox.Show("Error in uploading ZIP file", "Message", MessageBoxButton.OK);
                        }
                    }

                    else if ((_fileExtensionLower == KML) || (_fileExtensionLower == KMZ))
                    {
                        if (_uploadUrlKML != "")
                        {
                            busyIndicator.IsBusy = true;
                            busyIndicator.BusyContent = "Validating KML File .......";
                            validateKML();
                        }
                        else
                        {
                            busyIndicator.IsBusy = false;
                            MessageBox.Show("Error in uploading KML file", "Message", MessageBoxButton.OK);
                        }
                    }

                    else if (_fileExtensionLower == CSV)
                    {
                        busyIndicator.IsBusy = true;
                        busyIndicator.BusyContent = "Validating CSV File .......";
                        validateCSV();
                    }
                }
            }
           catch (Exception ex) {
               busyIndicator.IsBusy = false;
               MessageBox.Show("Error in uploading file.", "Message", MessageBoxButton.OK);
           }
        }

        //upload task for kml and shp files complete
        private void UploadTask_Complete(object sender, UploadEventArgs e)
        {
            try
            {
                if (e != null && e.Result != null)
                {
                    handleFileLoad(e.Result.Item);
                }
                else
                {
                    busyIndicator.IsBusy = false;
                    MessageBox.Show("Error in uploading file.", "Message", MessageBoxButton.OK);
                }
            }
            catch(Exception ex)
            {
                busyIndicator.IsBusy = false;
                MessageBox.Show("Error in uploading file.", "Message", MessageBoxButton.OK);
            }
        }

        //upload task for kml and shp files fail
        private void UploadTask_Failed(object sender, TaskFailedEventArgs e)
        {
            handleFileLoadError();
        }

        //on file upload failure
        private void handleFileLoadError() {
            busyIndicator.IsBusy = false;
            csv_latitude = null;
            csv_longitude = null;
            busyIndicator.IsBusy = false;
            MessageBox.Show("Error in uploading file." , "Message", MessageBoxButton.OK);
        }
     
        //after file is uploaded in gp server
        private void handleFileLoad(UploadItem item) {
            TxtUploadFileName.Text = "";
            _fileData = null;
            //busyIndicator.IsBusy = false;
            //send the gp request
            sendGeometryToEsriFormatRequest(item);
        }

        //get extension of browsed file
        private string getExt(string filename) {
            string ext = filename.Split('.').Last().ToString().ToLower();
            if (ext == filename) return "";
            return ext;
        }

        //call geoprocessing service
        private void sendGeometryToEsriFormatRequest(UploadItem item){
            try
            {
                List<GPParameter> requestParameters = new List<GPParameter>();
                if (((_fileExtensionLower == KML) || (_fileExtensionLower == KMZ)) && (_geoprocessingTaskUrlKML == null || _geoprocessingTaskUrlKML == "")){
                    MessageBox.Show("Invalid KML GP Service URL.", "Message", MessageBoxButton.OK);
                }
                if (_fileExtensionLower == ZIP && (_geoprocessingTaskUrlSHP == null || _geoprocessingTaskUrlSHP == ""))
                {
                    MessageBox.Show("Invalid SHP GP Service URL.", "Message", MessageBoxButton.OK);
                }
                busyIndicator.IsBusy = true;
                busyIndicator.BusyContent = "Rendering on Map.......";
                string uploadFilePath = item.ItemName;
                _fileExtensionLower = getExt(uploadFilePath);
                if ((_fileExtensionLower == KML) || (_fileExtensionLower == KMZ))
                {
                    requestParameters.Add(new GPItemID("kmlFile", item.ItemID));// "{ \"itemID\":\"" + item.ItemID + "\" } "));
                    
                    _geometryUploaderGeoprocessingTask = new Geoprocessor(_geoprocessingTaskUrlKML);
                    _geometryUploaderGeoprocessingTask.OutputSpatialReference = _map.SpatialReference;
                    _isAsync = true;
                }
                else if (_fileExtensionLower == ZIP)
                {
                    requestParameters.Add(new GPItemID("ipZipFile", item.ItemID));
                    _geometryUploaderGeoprocessingTask = new Geoprocessor(_geoprocessingTaskUrlSHP);
                    _geometryUploaderGeoprocessingTask.OutputSpatialReference = _map.SpatialReference;
                    _isAsync = false;
                }
                else
                {
                    busyIndicator.IsBusy = false;
                    return;
                }


                if (_isAsync && (_fileExtensionLower == KML || _fileExtensionLower == KMZ))
                {

                    _geometryUploaderGeoprocessingTask.JobCompleted += new EventHandler<JobInfoEventArgs>(HandleGeoprocessingKML_JobCompleted);
                    _geometryUploaderGeoprocessingTask.Failed += new EventHandler<ESRI.ArcGIS.Client.Tasks.TaskFailedEventArgs>(GeometryGeoprocessing_Failed);
                    _geometryUploaderGeoprocessingTask.SubmitJobAsync(requestParameters);

                }
                else if (_fileExtensionLower == ZIP)
                {
                    _geometryUploaderGeoprocessingTask.Failed += new EventHandler<ESRI.ArcGIS.Client.Tasks.TaskFailedEventArgs>(GeometryGeoprocessing_Failed);
                    _geometryUploaderGeoprocessingTask.ExecuteCompleted += new EventHandler<GPExecuteCompleteEventArgs>(HandleGeoprocessing_ExecuteCompleted);

                    _geometryUploaderGeoprocessingTask.ExecuteAsync(requestParameters);
                }

                //reset browse form
                TxtUploadFileName.Text = "";
                _fileData = null;
            }
            catch
            {
                busyIndicator.IsBusy = false;
                MessageBox.Show("Error in uploading file. Please try again", "Message", MessageBoxButton.OK);
            }
        }

        private void validateKML()
        {
            StreamReader reader = new StreamReader(_fileData);
            string data = reader.ReadToEnd();
            XElement parsedXml = XElement.Parse(data);
            bool isVaidKML = false;
            foreach (XElement element in parsedXml.Descendants())
            {
                if(element.Name != null & element.Name.LocalName != null){
                    if (element.Name.LocalName == "Placemark" || element.Name.LocalName == "Polygon" || element.Name.LocalName == "LineString" || element.Name.LocalName == "NetworkLink")
                    {
                        isVaidKML = true;
                        break;
                    }
                }
            }
            _fileData = new MemoryStream(Encoding.UTF8.GetBytes(data));  //refill _fileData
            if (isVaidKML)
            {
                //upload file
                busyIndicator.IsBusy = true;
                busyIndicator.BusyContent = "Uploading Files.......";
                upLoadFile(_uploadUrlKML);
                data = null;
            }
            else
            {
                busyIndicator.IsBusy = false;
                MessageBox.Show("The KML file should contain valid geometry.", "Message", MessageBoxButton.OK);
                TxtUploadFileName.Text = "";
                _fileData = null;
                data = null;
            }
        }

        private void validateCSV()
        {
            int newLineIndex = -1;
            int recordCount = 0;
            StreamReader reader = new StreamReader(_fileData);
            string data = reader.ReadToEnd();
            if (data.IndexOf("\r\n") > -1)
            {
                newLineIndex = data.IndexOf("\r\n");
                recordCount = data.Split(new string[] { "\r\n" }, StringSplitOptions.None).Count() - 1;
            }
            else if (data.IndexOf("\n") > -1)
            {
                newLineIndex = data.IndexOf("\n");
                recordCount = data.Split(new string[] { "\n" }, StringSplitOptions.None).Count() - 1;
            }

            if (recordCount > _GPServiceLimit)
            {
                busyIndicator.IsBusy = false;
                MessageBox.Show("The Maximum record count for uploaded File should be less than " + _GPServiceLimit.ToString() + ".", "Message", MessageBoxButton.OK);
                data = null;
            }
            else
            {
                string firstLine = data.Substring(0, newLineIndex).Trim();
                string[] header = firstLine.Split(',');
                readCoordinates(header, "LATITUDE", "LONGITUDE");
                readCoordinates(header, "LAT", "LON");
                readCoordinates(header, "Y", "X");
                readCoordinates(header, "LAT", "LONG");

                if (csv_latitude != null && csv_longitude != null && csv_latitude != "" && csv_longitude != "")
                {
                    //upload file
                    busyIndicator.IsBusy = true;
                    busyIndicator.BusyContent = "Uploading Files.......";
                    upLoadCSVFileOnServer(data);
                    data = null;
                }
                else
                {
                    busyIndicator.IsBusy = false;
                    MessageBox.Show("Input CSV should contain (Latitude,Longitude) or (Lat,Lon) or (Lat,Long) or (X,Y) attribute combination.", "Message", MessageBoxButton.OK);
                    TxtUploadFileName.Text = "";
                    _fileData = null;
                    csv_latitude = null;
                    csv_longitude = null;
                    data = null;
                }
            }
        }

        private void upLoadCSVFileOnServer(string data)
        {
            //Uploading the File to the server
            DateTime dt = System.DateTime.Now;
            string newFileName = WebContext.Current.User.Name.Replace("PGE\\", "").ToUpper() + "_" + dt.Day.ToString() + dt.Month.ToString() + dt.Year.ToString() + dt.Hour.ToString() + dt.Minute.ToString() + dt.Second.ToString() + "_" +TxtUploadFileName.Text;

            UriBuilder ub = new UriBuilder(_uploadFileHandler);
            ub.Query = "filename=" + newFileName + "&module=AddFileToServer";
            WebClient c = new WebClient();
            _fileData = new MemoryStream(Encoding.UTF8.GetBytes(data));
            c.OpenWriteCompleted += (sender, e) =>
            {
                try
                {
                    PushData(_fileData, e.Result);
                    e.Result.Close();
                    _fileData.Close();
                }
                catch (Exception ex)
                {
                    busyIndicator.IsBusy = false;
                    MessageBox.Show("Error in uploading file. Please try again", "Message", MessageBoxButton.OK);
                }
            };
            c.OpenWriteAsync(ub.Uri);
            c.WriteStreamClosed += (s, e) => c_WriteStreamClosed(newFileName, e);
        }

        private void PushData(Stream input, Stream output)
        {
            byte[] buffer = new byte[4096];
            int bytesRead;
            while ((bytesRead = input.Read(buffer, 0, buffer.Length)) != 0)
            {
                output.Write(buffer, 0, bytesRead);
            }
        }

        private void c_WriteStreamClosed(string newFileName, WriteStreamClosedEventArgs e)
        {
            try
            {
                if (e.Error != null)
                {
                    busyIndicator.IsBusy = false;
                    MessageBox.Show("Error in uploading file. Please try again", "Message", MessageBoxButton.OK);
                    return;
                }
                handleCSVFileLoad(newFileName);
            }
            catch (Exception ex)
            {
                busyIndicator.IsBusy = false;
                MessageBox.Show("Error in uploading file. Please try again", "Message", MessageBoxButton.OK);
            }
        }

        private void handleCSVFileLoad(string newFileName)
        {
            _fileExtensionLower = getExt(TxtUploadFileName.Text);
            ESRI.ArcGIS.Client.Toolkit.DataSources.CsvLayer myCsvLayer = new ESRI.ArcGIS.Client.Toolkit.DataSources.CsvLayer();

            // Set the Url of the CsvLayer to a public service. 
            // NOTE: you need to adjust the Url to a .csv file served up on your web server.
            myCsvLayer.Url = _uploadedCSVURLNew + newFileName;
            myCsvLayer.ID = TxtUploadFileLbl.Text.ToString().Trim();
            // Set the XFieldName and YFieldName Properties. This CSV file does not use standard X & Y field names.

            myCsvLayer.XFieldName = csv_longitude;
            myCsvLayer.YFieldName = csv_latitude;
            if (csv_latitude == "Y" && csv_longitude == "X")  // in case of X and Y
                myCsvLayer.SourceSpatialReference = _map.SpatialReference;
            else  // in case of latitude and longitude
                myCsvLayer.SourceSpatialReference = new SpatialReference(4326);

            myCsvLayer.Initialized += (s, e) => CsvLayer_Initialized(s, newFileName);
            myCsvLayer.InitializationFailed += new EventHandler<EventArgs>(CsvLayer_InitializationFailed);
            myCsvLayer.Initialize();
            TxtUploadFileName.Text = "";
            _fileData = null;
        }

        private void CsvLayer_InitializationFailed(object sender, EventArgs e)
        {
            busyIndicator.IsBusy = false;
            MessageBox.Show("Error in uploading CSV file. Please try again", "Message", MessageBoxButton.OK);
        }

        private void CsvLayer_Initialized(object sender, string fileNameOnServer)
        {
            try
            {
                // This function will execute as a result of the CsvLayer that was defined in code-behind being Initialized.

                // Get the CsvLayer.
                ESRI.ArcGIS.Client.Toolkit.DataSources.CsvLayer myCsvLayer = (ESRI.ArcGIS.Client.Toolkit.DataSources.CsvLayer)sender;

                if (myCsvLayer.InitializationFailure == null)
                {
                    // Get the GraphicCollection from the CsvLayer.
                    ESRI.ArcGIS.Client.GraphicCollection theGraphicCollection = myCsvLayer.Graphics;
                    var graphicList = new List<Graphic>();

                    if (theGraphicCollection != null && theGraphicCollection.Count > 0)
                    {
                        if (csv_latitude == "Y" && csv_longitude == "X")
                        {
                            GraphicsLayer graphicsLayer = new GraphicsLayer() { ID = TxtUploadFileLbl.Text.ToString().Trim() };
                            Envelope featureExtent = null;
                            foreach (ESRI.ArcGIS.Client.Graphic oneGraphic in theGraphicCollection)
                            {
                                Graphic newGraphic = new Graphic();
                                newGraphic.Geometry = oneGraphic.Geometry;
                                newGraphic.Symbol = new ESRI.ArcGIS.Client.Symbols.SimpleMarkerSymbol()
                                {
                                    Color = new SolidColorBrush(Color.FromArgb(128, 255, 0, 0)),
                                    Size = 20,
                                    Style = ESRI.ArcGIS.Client.Symbols.SimpleMarkerSymbol.SimpleMarkerStyle.Circle
                                };
                                if (featureExtent == null)
                                    featureExtent = oneGraphic.Geometry.Extent.Clone();
                                else
                                    featureExtent = featureExtent.Union(oneGraphic.Geometry.Extent);

                                //maptip
                                newGraphic.MapTip = CreateTipWindow(oneGraphic.Attributes, TxtUploadFileLbl.Text);

                                graphicsLayer.Graphics.Add(newGraphic);
                            }
                            _map.Layers.Add(graphicsLayer);

                            if (featureExtent != null)
                            {
                                _map.ZoomTo(featureExtent.Expand(1.25));
                            }
                            busyIndicator.IsBusy = false;
                            ConfigUtility.StatusBar.Text = "CSV Uploaded Successfully.";
                            TxtUploadFileLbl.Text = "";
                            csv_latitude = null;
                            csv_longitude = null;
                            TempLayer tl = new TempLayer();
                            tl.title = _serviceLayerLabel + "-(Layer)";
                            tl.layer = graphicsLayer;
                            TempLayerList.Add(tl);

                            //update TOC - Toggle layers
                            _mapTools.LayerControl.AddLayerToTOC(_mapTools.LayerControl, _map, graphicsLayer.ID);
                        }
                        else
                        {  // in case of latitude and longitude

                            //retain graphic attributes

                            List<IDictionary<string, object>> attributesList = new List<IDictionary<string, object>>();
                            foreach (Graphic oneGraphic in theGraphicCollection)
                            {
                                attributesList.Add(oneGraphic.Attributes);
                            }

                            var geometryService = new GeometryService(_geometryService);
                            geometryService.Failed += new EventHandler<ESRI.ArcGIS.Client.Tasks.TaskFailedEventArgs>(GeomteryService_Failed);
                            geometryService.ProjectCompleted += (s, e) => GeometryServiceNavigateProjectCompleted(fileNameOnServer, e, attributesList);
                            geometryService.ProjectAsync(theGraphicCollection, _map.SpatialReference);
                        }
                    }
                    else
                    {
                        busyIndicator.IsBusy = false;
                        MessageBox.Show("No Data available in uploaded File.", "Message", MessageBoxButton.OK);
                    }
                }
            }
            catch (Exception ex)
            {
                busyIndicator.IsBusy = false;
                MessageBox.Show("Error in uploading file. Please try again", "Message", MessageBoxButton.OK);
            }

        }

        private void GeometryServiceNavigateProjectCompleted(string fileNameOnServer, GraphicsEventArgs e, List<IDictionary<string, object>> attributesList)
        {
            try
            {
                if (e != null && e.Results != null && e.Results.Count > 0)
                {
                    GraphicsLayer graphicsLayer = new GraphicsLayer() { ID = TxtUploadFileLbl.Text.ToString().Trim() };
                    Envelope featureExtent = null;
                    for(var i = 0; i < e.Results.Count; i++)
                    {
                        e.Results[i].Symbol = new ESRI.ArcGIS.Client.Symbols.SimpleMarkerSymbol()
                        {
                            Color = new SolidColorBrush(Color.FromArgb(128, 255, 0, 0)),
                            Size = 20,
                            Style = ESRI.ArcGIS.Client.Symbols.SimpleMarkerSymbol.SimpleMarkerStyle.Circle
                        };
                        if (featureExtent == null)
                            featureExtent = e.Results[i].Geometry.Extent.Clone();
                        else
                            featureExtent = featureExtent.Union(e.Results[i].Geometry.Extent);
                        e.Results[i].MapTip = CreateTipWindow(attributesList[i], _serviceLayerLabel);
                        graphicsLayer.Graphics.Add(e.Results[i]);
                    }
                    _map.Layers.Add(graphicsLayer);

                    if (featureExtent != null)
                    {
                        _map.ZoomTo(featureExtent.Expand(1.25));
                    }
                    busyIndicator.IsBusy = false;
                    ConfigUtility.StatusBar.Text = "CSV Uploaded Successfully.";
                    TxtUploadFileLbl.Text = "";
                    csv_latitude = null;
                    csv_longitude = null;
                    TempLayer tl = new TempLayer();
                    tl.title = _serviceLayerLabel + "-(Layer)";
                    tl.layer = graphicsLayer;
                    TempLayerList.Add(tl);

                    //update TOC - Toggle layers
                    _mapTools.LayerControl.AddLayerToTOC(_mapTools.LayerControl, _map, graphicsLayer.ID);
                    
                }
                else
                {
                    busyIndicator.IsBusy = false;
                    MessageBox.Show("Error in uploading file. Please try again", "Message", MessageBoxButton.OK);
                }
            }
            catch (Exception ex)
            {
                busyIndicator.IsBusy = false;
                MessageBox.Show("Error in uploading file. Please try again", "Message", MessageBoxButton.OK);
            }
        }

        void GeomteryService_Failed(object sender, ESRI.ArcGIS.Client.Tasks.TaskFailedEventArgs e)
        {
            busyIndicator.IsBusy = false;
            MessageBox.Show("Error in uploading file. Geometry service failed.", "Message", MessageBoxButton.OK);
        }

        private void upLoadFile(string uploadtaskUrl)
        {
            double fileSize = Math.Round(Convert.ToDouble(Convert.ToDecimal(_fileData.Length) / 1000000), 3);
            if (fileSize > _uploadPartSizeMB)
            {
                MessageBox.Show("Error in uploading file. File size should not be greater than " + _uploadPartSizeMB.ToString() + " MB.", "Message", MessageBoxButton.OK);
                return;
            }
            UploadTask uploadTask = new UploadTask(uploadtaskUrl);
            uploadTask.PartSize = _uploadPartSizeMB * 1024 * 1024;
            uploadTask.UploadCompleted += UploadTask_Complete;
            uploadTask.Failed += UploadTask_Failed;
            uploadTask.UploadAsync(new UploadParameters() { FileStream = _fileData, FileName = TxtUploadFileName.Text });
            
        }

        private void readCoordinates(string[] arrayObject, string lat, string lon) {
            for (var i = 0; i < arrayObject.Count(); i++) {
                string upperCase = arrayObject[i].ToUpper();
                if (upperCase == lat.ToUpper()) {
                    csv_latitude = arrayObject[i];
                } 
                else if (upperCase == lon.ToUpper()) {
                    csv_longitude = arrayObject[i];
                }
            }           
        }

        private Envelope GetGeometry(FeatureSet featureSet)
        {
            Envelope env = null;
            foreach (Graphic feature in featureSet.Features)
            {
                if (feature.Geometry != null && feature.Geometry.Extent != null)
                {
                    if (env == null)
                        env = feature.Geometry.Extent.Clone();
                    else
                        env = env.Union(feature.Geometry.Extent);
                }
            }

            return env;
        }

        //Maptip code starts
        private PopupWindow CreateTipWindow(IDictionary<string, object> attributes, string header)
        {
            PopupWindow tipWindow = new PopupWindow() { ShowArrow = true, ShowCloseButton = false };
            tipWindow.Title = header.Trim();
            tipWindow.Content = CreateFeatureTipContent(attributes); 
            return tipWindow;
        }

        private FrameworkElement CreateFeatureTipContent(IDictionary<string, object> attributes)
        {
            StackPanel stackBox = new StackPanel() { Margin = new Thickness(4, 1, 4, 1), Orientation = Orientation.Vertical };

            foreach (string key in attributes.Keys)
            {
                TextBlock valueBlock = new TextBlock() { TextWrapping = TextWrapping.Wrap };
                if (attributes[key] != null)
                    valueBlock.Text = key + ": " + attributes[key].ToString();
                else
                    valueBlock.Text = key + ": ";
                stackBox.Children.Add(valueBlock);
            }

            return stackBox;
        }
        //Maptip code ends

        #endregion private methods

        #region public methods

        //to remove csv files from server on application startup
        public void RemoveFileFromServer()
        {
            try
            {
                UriBuilder ub = new UriBuilder(_uploadFileHandler);
                ub.Query = "LANID=" + WebContext.Current.User.Name.Replace("PGE\\", "").ToUpper() + "&module=DeleteFileFromServer";
                WebClient c = new WebClient();
                c.OpenWriteCompleted += (sender, e) =>
                {
                    e.Result.Close();
                };
                c.OpenWriteAsync(ub.Uri);
            }
            catch
            {
            }
        }

        #endregion public methods

    }
}
