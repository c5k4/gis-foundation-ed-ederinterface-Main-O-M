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
using System.ComponentModel;
using ESRI.ArcGIS.Client.Geometry;
using ESRI.ArcGIS.Client.Tasks;
using ESRI.ArcGIS.Client;
using System.Windows.Browser;
using System.Xml.Linq;
using Miner.Server.Client.Toolkit ;
using Miner.Server.Client.Tasks;
using System.Collections.ObjectModel;
using System.Windows.Printing;
using System.Windows.Media.Imaging;
using ESRI.ArcGIS.Client.Symbols;

namespace ArcFMSilverlight
{
    public partial class EnvironmentalInfo : UserControl,INotifyPropertyChanged 
    {
        public static EnvironmentalInfo environementInfo = null;
        public event PropertyChangedEventHandler PropertyChanged;
        private const int WgsWKID = 4326;
        private Map FocusMap = null;
        private string _wildfire;
        private List<int>  _lyrIds = new List<int>() ;
        private List<string>  _lyrnames = new List<string> ();
        private Dictionary<string, string> _pgewildfiredomain = new Dictionary<string, string>(); 
        private string _service;
        public List<TaskBase> CurrentlyExecutingTasks = new List<TaskBase>();
        private Geoprocessor profileGPTask = null;
        private string _elevationService = "";
        private string _faaurl = "";
        private int _restCallCount = 0;
        private string _jobid = "";

        private static readonly Size StandardLandscapePageSize = new Size(8.5, 11);
        private static readonly Size StandardLandscapePrintableAreaSize = new Size(7.5, 10);
        private const double DotsPerInch = 96;
        private const string PRINT_NAME = "WEBR GIS Attributes";
        private int pages;
        private WriteableBitmap printImage;
        private const string EnvironmentalGraphicsLayer = "__EnvReportGraphics";
        
        
        public EnvironmentalInfo()
        {
            InitializeComponent();
            this.DataContext = this;
            environementInfo = this;
            DisplayResults = false;
             

            if (Application.Current.Host.InitParams.ContainsKey("Config"))
            {
                string config = Application.Current.Host.InitParams["Config"];
                var attribute="";
                config = HttpUtility.HtmlDecode(config).Replace("***", ",");
                XElement elements = XElement.Parse(config);
                foreach (XElement element in elements.Elements())
                {
                    if (element.Name.LocalName == "EnvironmentalInformation")
                    {
                        if (element.HasElements)
                        {
                            foreach (XElement childelement in element.Elements())
                            {
                                if (childelement.Name.LocalName == "Service")
                                {
                                    attribute = childelement.Attribute("Name").Value;
                                    if (attribute != null)
                                    {
                                        _service = attribute;
                                    }  
                                }

                                if (childelement.Name.LocalName == "Elevation")
                                {
                                    attribute = childelement.Attribute("url").Value;
                                    if (attribute != null)
                                    {
                                        _elevationService = attribute;
                                    }
                                }
                                if (childelement.Name.LocalName == "FAAUrl")
                                {
                                    attribute = childelement.Attribute("url").Value;
                                    if (attribute != null)
                                    {
                                        _faaurl = attribute;
                                    }

                                    FAAUrl.NavigateUri = new Uri( _faaurl);
                                }
                                if (childelement.Name.LocalName == "Layers")
                                {
                                    attribute = childelement.Attribute("Names").Value;
                                    if (attribute != null)
                                    {
                                        foreach (string _str in attribute.Split(','))
                                        {
                                            _lyrnames.Add(_str);
                                        }
                                    }

                                    attribute = childelement.Attribute("LayerIds").Value;
                                    if (attribute != null)
                                    {
                                        foreach (string _str in attribute.Split(','))
                                        {
                                            _lyrIds.Add(Convert.ToInt16(_str));
                                        }
                                    }
                                }

                                

                                if (childelement.Name.LocalName == "Domain")
                                {
                                    var domain = "";
                                    var value = "";

                                    foreach (XElement domainelement in childelement.Elements())
                                    {
                                        if (domainelement.Name.LocalName == "Code")
                                        {
                                            domain = domainelement.Attribute("ID").Value;
                                            value = domainelement.Attribute("Value").Value;
                                            if (domain != null && value != null)
                                            {
                                                _pgewildfiredomain.Add(domain, value);   
                                            }
                                        }
                                    }
                                }
                            }                   

                        }

                    }
                }
            }
        }

        public void PrintPages(StackPanel control)
        {
            PrintDocument pd = new PrintDocument();

            pd.BeginPrint += (s, args) =>
            {
                pages = 0;
                printImage = new WriteableBitmap(control, null);
            };

            pd.PrintPage += (s, args) =>
            {
                WriteableBitmap pictPart = printImage.Crop(0, (int)args.PrintableArea.Height * pages, (int)args.PrintableArea.Width, (int)args.PrintableArea.Height);

                Canvas cnv = new Canvas()
                {
                    Width = pictPart.PixelWidth,
                    Height = pictPart.PixelHeight,
                    Background = new ImageBrush() { ImageSource = pictPart, Stretch = Stretch.Fill }
                };

                args.PageVisual = cnv;

                pages++;
                if (control.ActualHeight > args.PrintableArea.Height * pages)
                    args.HasMorePages = true;
                else
                    args.HasMorePages = false;
            };

            pd.EndPrint += (s, args) =>
            {
                printImage = null;
            };

            pd.Print(PRINT_NAME);
        }

        private void PrintWindowCmd_Click(object sender, RoutedEventArgs e)
        {
          PrintPages(AllComponentsStackPanel);
        }

        

        private void AddEnvironmentalReportMarker(MapPoint mapPoint)
        {
            ESRI.ArcGIS.Client.Symbols.MarkerSymbol theSelectedMarkerSymbol = null;
            theSelectedMarkerSymbol = (ESRI.ArcGIS.Client.Symbols.MarkerSymbol)(LayoutRoot.Resources["esriDefaultMarker_98"]);

            GraphicsLayer graphicsLayer = FocusMap.Layers[EnvironmentalGraphicsLayer] as GraphicsLayer;
            if (graphicsLayer == null)
            {
                graphicsLayer = new GraphicsLayer();
                graphicsLayer.ID = EnvironmentalGraphicsLayer;
                FocusMap.Layers.Add(graphicsLayer);
            }
            graphicsLayer.ClearGraphics();

            Graphic graphic = new Graphic()
            {
                Geometry = mapPoint,
                Symbol = theSelectedMarkerSymbol
            };
            graphicsLayer.Graphics.Add(graphic);
        }

        private void RemoveEnvironmentalReportMarker()
        {
            GraphicsLayer graphicsLayer = FocusMap.Layers[EnvironmentalGraphicsLayer] as GraphicsLayer;
            if (graphicsLayer != null)
            {
                graphicsLayer.ClearGraphics();
            }
        }
        
        public void getEnvironmentalInformation(MapPoint mappoint, Map focusMap)
        {
            if (mappoint == null)
            {
                ConfigUtility.StatusBar.Text = "Right Click on Map Area";
                return;
            }

            ConfigUtility.StatusBar.Text  = "Identifying Electric Distribution Design Elements..";
            //Set Map
            FocusMap = focusMap;
            DisplayResults = false;
            // Set Empty 
            
            setEnvironmentEmpty();

            AddEnvironmentalReportMarker(mappoint);

            //Kill all previous task base

            StopCurrentAsyncTasks();
            _jobid = "";
            //Get the Lat Long Information
            GeometryService geometryService = new GeometryService(ConfigUtility.GeometryServiceURL);

            var graphic = new Graphic
            {
                Geometry = mappoint
            };

            var graphicList = new List<Graphic> { graphic };
            SpatialReference sp = new SpatialReference(WgsWKID);

            geometryService.Failed += new EventHandler<ESRI.ArcGIS.Client.Tasks.TaskFailedEventArgs>(geometryService_Failed); ;
            geometryService.ProjectCompleted  += new EventHandler<GraphicsEventArgs>(geometryService_ProjectCompleted);
            geometryService.ProjectAsync(graphicList, sp, mappoint);

            
            //Make the geometry service call

            //First we need to buffer our point click by using the geometry service.
            BufferParameters bufferParams = new BufferParameters();
            bufferParams.BufferSpatialReference = mappoint.SpatialReference;
            //Adjust the buffer point per the map scale.
            double bufferSize = FocusMap.Scale * .004;
            if (bufferSize > 10) { bufferSize = 10; }
            bufferParams.Distances.Add(bufferSize);
            bufferParams.Features.Add(graphic);

            GeometryService geomService = new GeometryService(ConfigUtility.GeometryServiceURL);
            geomService.BufferCompleted += new EventHandler<GraphicsEventArgs>(geomService_BufferCompleted);
            geomService.Failed += new EventHandler<ESRI.ArcGIS.Client.Tasks.TaskFailedEventArgs>(geomService_Failed);
            
            geomService.BufferAsync(bufferParams);

            ESRI.ArcGIS.Client.Geometry.Polyline _polyline = new ESRI.ArcGIS.Client.Geometry.Polyline();
            ESRI.ArcGIS.Client.Geometry.PointCollection _ptCollection = new ESRI.ArcGIS.Client.Geometry.PointCollection();

            CurrentlyExecutingTasks.Add(geomService);

                      

            //Add a default attribute to the newly created graphic
            List<Field> flds = new List<Field>();
            Field fld = new Field();
            fld.FieldName = "OBJECTID";
            fld.Type = Field.FieldType.OID;
            flds.Add(fld);
            Field fld1 = new Field();
            fld1.FieldName = "TYPE";
            fld1.Type = Field.FieldType.String;
            //flds.Add(fld1);
                      

            FeatureSet ft = new FeatureSet();
            ft.Fields = flds;
            graphic.Attributes.Add("OBJECTID", 1);
            //graphic.Attributes.Add("TYPE", "PROFILE");

            ft.Features.Add(graphic);
            ft.DisplayFieldName = "OBJECTID";
            //ft.Features.Add(lingraphic);

            profileGPTask = new Geoprocessor(_elevationService);

            profileGPTask.JobCompleted += new EventHandler<JobInfoEventArgs>(profileGPTask_JobCompleted);
            profileGPTask.StatusUpdated += new EventHandler<JobInfoEventArgs>(profileGPTask_StatusUpdated);
            profileGPTask.Failed += new EventHandler<ESRI.ArcGIS.Client.Tasks.TaskFailedEventArgs>(profileGPTask_Failed);

            //GPFeatureRecordSetLayer pointFS = new GPFeatureRecordSetLayer("Point", graphic.Geometry);
            List<GPParameter> gpParameters = new List<GPParameter>();
            gpParameters.Add(new GPFeatureRecordSetLayer("Point", ft));
            
                   
            profileGPTask.SubmitJobAsync(gpParameters);

            CurrentlyExecutingTasks.Add(profileGPTask);  
            
        }
        

        /// <summary>
        /// Status Updated event handler for profile Geoprocessor Task
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void profileGPTask_StatusUpdated(object sender, JobInfoEventArgs e)
        {
            _jobid = e.JobInfo.JobId;
            switch (e.JobInfo.JobStatus)
            {
                case esriJobStatus.esriJobSubmitted:
                    // Disable automatic status checking.
                    //geoprocessingTask.CancelJobStatusUpdates(e.JobInfo.JobId);
                    break;
                case esriJobStatus.esriJobSucceeded:
                    // Get the results.
                    //geoprocessingTask.GetResultDataAsync(e.JobInfo.JobId, "<parameter name>");
                    //GlobalVariables.mapBusyIndicator.IsBusy = false;
                    //MessageBox.Show("Job Succeeded");
                    break;
                case esriJobStatus.esriJobFailed:
                    //GlobalVariables.mapBusyIndicator.IsBusy = false;
                    //ConfigUtility.StatusBar.Text = "Failed to get Elevation..";
                    break;
                case esriJobStatus.esriJobTimedOut:
                    //GlobalVariables.mapBusyIndicator.IsBusy = false;
                    //MessageBox.Show("Job TimeOut");
                    //this.Close();
                    break;
            }
        }

        /// <summary>
        /// Failed event handler for profile Geoprocessor Task
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void profileGPTask_Failed(object sender, ESRI.ArcGIS.Client.Tasks.TaskFailedEventArgs e)
        {
            ConfigUtility.StatusBar.Text = "Failed to get Elevation..";
            
        }
        /// <summary>
        /// Job Completed event handler for Profile Geoprocessor Task
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void profileGPTask_JobCompleted(object sender, JobInfoEventArgs e)
        {
            try
            {
                profileGPTask.GetResultDataCompleted += new EventHandler<GPParameterEventArgs>(profileGPTask_GetResultDataCompleted);
                profileGPTask.GetResultDataAsync(e.JobInfo.JobId, "OutElevation_DTM");
                

            }
            catch (Exception ex)
            {
                //profileBusyIndicator.IsBusy = false;
                //client.WriteErrorAsync("Profile.xaml.cs : profileGPTask_JobCompleted", ex.Message, ex.StackTrace, RLPlan.Models.UserValues.UserName);
            }
        }

        /// <summary>
        /// GetResult data Completed event handler for Profile Geoprocessor Task
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void profileGPTask_GetResultDataCompleted(object sender, GPParameterEventArgs e)
        {
            try
            {
                _restCallCount++;

                GPString outputResults = e.Parameter as GPString;
                string elevationResults = outputResults.Value;
                
                if (elevationResults != "")
                {
                    Elevation = elevationResults + " feet";
                }
                
                
                if (_restCallCount == CurrentlyExecutingTasks.Count)
                {
                    DisplayResults = true;
                    ConfigUtility.StatusBar.Text = "";
                }
                                
            }
            catch (Exception ex)
            {
                RemoveEnvironmentalReportMarker();
            }
        }

        void geomService_Failed(object sender, ESRI.ArcGIS.Client.Tasks.TaskFailedEventArgs e)
        {
            
            ConfigUtility.StatusBar.Text = "Error in Buffering..";
            //throw new NotImplementedException();
        }

        void geomService_BufferCompleted(object sender, GraphicsEventArgs e)
        {
            //Identify the Land base Service 
            Graphic bufferResult = e.Results[0];
            IdentifyTask identifyTask = new IdentifyTask(_service);
            identifyTask.ExecuteCompleted += new EventHandler<IdentifyEventArgs>(identifyTask_ExecuteCompleted);
            identifyTask.Failed += new EventHandler<ESRI.ArcGIS.Client.Tasks.TaskFailedEventArgs>(identifyTask_Failed);

            IdentifyParameters identifyParams = new IdentifyParameters();
            identifyParams.LayerOption = LayerOption.all ;
            identifyParams.LayerIds.AddRange(_lyrIds) ; 
            identifyParams.MapExtent = FocusMap.Extent;
            identifyParams.Width = (int)FocusMap.ActualWidth;
            identifyParams.Height = (int)FocusMap.ActualHeight;
            identifyParams.Geometry = bufferResult.Geometry;
            //CurrentlyExecutingTasks.Add(identifyTask);
            identifyTask.ExecuteAsync(identifyParams);
            identifyParams.ReturnGeometry = false;
            CurrentlyExecutingTasks.Add(identifyTask);  
        }

        void identifyTask_Failed(object sender, ESRI.ArcGIS.Client.Tasks.TaskFailedEventArgs e)
        {
            ConfigUtility.StatusBar.Text = "Error in Identifying..";
            RemoveEnvironmentalReportMarker();
            //throw new NotImplementedException();
        }

        void identifyTask_ExecuteCompleted(object sender, IdentifyEventArgs e)
        {
            
            string _layerName = "";
            _restCallCount++;

            foreach (IdentifyResult identifyResults in e.IdentifyResults)
            {

               //<Layers Names="Raptor Concentration Zone,PG&amp;E Wildland Fire Management Areas,Corrosion Areas,Electric Summer Temperature Zone,Snow Loading Zone,Fire Area Designation,CalFireAdjIndex,Electric Climate Zone,HistoricPeakWind,City,Districts,Division,County,InsulationAreas,PrimaryVoltageArea,SurgeProtectionDistricts,High Fire Threat District" LayerIds="4,6,7,8,9,10,11,12,13,19,20,21,23,14,15,16,17"></Layers>
                _layerName = identifyResults.LayerName;
               
                if(_layerName == _lyrnames[0])
                    RCZ  = "Yes";
                else if (_layerName == _lyrnames[1])
                {
                    WildLandFire = _pgewildfiredomain[identifyResults.Feature.Attributes["CATEGORY"].ToString()];
                }
                else if (_layerName == _lyrnames[2])
                    CorrosionArea = identifyResults.Feature.Attributes["CORROSION"].ToString();
                else if (_layerName == _lyrnames[3])
                    SummerTemperature = identifyResults.Feature.Attributes["ZONE"].ToString();
                else if (_layerName == _lyrnames[4])
                    SnowLoadingArea = identifyResults.Feature.Attributes["SNOWLOAD"].ToString();
                else if (_layerName == _lyrnames[5])
                    FireArea = _pgewildfiredomain[identifyResults.Feature.Attributes["SRA"].ToString()];
                else if (_layerName == _lyrnames[6])
                    FireIndexArea = identifyResults.Feature.Attributes["NAME1_NUM"].ToString();
                   
                 else if (_layerName == _lyrnames[6])
                    
                    FireIndexControlArea=identifyResults.Feature.Attributes["NAME2"].ToString();
                else if (_layerName == _lyrnames[7])
                    ClimateZoneCode = identifyResults.Feature.Attributes["CZ_CODE"].ToString();

                else if (_layerName == _lyrnames[8])
                    HistoricPeakWind = identifyResults.Feature.Attributes["DESCRIPTION"].ToString();  
                else if (_layerName == _lyrnames[9])
                    City = identifyResults.Feature.Attributes["CITY_NAME"].ToString();

                else if (_layerName == _lyrnames[10])

                    Districts = identifyResults.Feature.Attributes["DISTRICT"].ToString();
                else if (_layerName == _lyrnames[11])
                    Division = identifyResults.Feature.Attributes["DIVISION"].ToString();

                else if (_layerName == _lyrnames[12])
                    Country = identifyResults.Feature.Attributes["CNTY_NAME"].ToString();
              
                else if (_layerName == _lyrnames[13])

                    InsulationAreas = identifyResults.Feature.Attributes["CODE"].ToString();
                else if (_layerName == _lyrnames[14])

                    PrimaryVoltageArea = identifyResults.Feature.Attributes["CLASS"].ToString();
                else if (_layerName == _lyrnames[15])

                    SurgeProtectionDistricts = identifyResults.Feature.Attributes["DISTRICT_NUMBER"].ToString();
                else if (_layerName == _lyrnames[16] && identifyResults.Feature.Attributes["High Fire Threat District"] != null)   //INC000004493364

                    HighFireThreatDistrict = identifyResults.Feature.Attributes["High Fire Threat District"].ToString().Split('-')[0].Trim();
            }
            //ConfigUtility.StatusBar.Text = "";

            if (_restCallCount == CurrentlyExecutingTasks.Count )
            {
                DisplayResults = true;
                //ConfigUtility.StatusBar.Text = "Identifying Elevation..";
            }
            
        }

        void geometryService_ProjectCompleted(object sender, GraphicsEventArgs e)
        {
            //throw new NotImplementedException();
            var point = (MapPoint)e.Results[0].Geometry;
            Latitude = Math.Round(point.Y, 6) + " (" + ConvertDigreesMinutesSecons(point.Y) + " N)";
            Longitude = Math.Round(point.X, 6) + " (" + ConvertDigreesMinutesSecons(point.X) + " W)";
            _restCallCount++;

            if (_restCallCount > 1)
            {
                DisplayResults = true;
                ConfigUtility.StatusBar.Text = "Identifying Elevation..";
            }

        }

        private string ConvertDigreesMinutesSecons(double decimal_degrees)
        {
            // set decimal_degrees value here
            double digrees = Math.Abs(Math.Ceiling(decimal_degrees));
            double minutes = (decimal_degrees - Math.Floor(decimal_degrees)) * 60.0;
            double seconds = (minutes - Math.Floor(minutes)) * 60.0;
            double tenths = (seconds - Math.Floor(seconds)) * 10.0;
            // get rid of fractional part
            minutes = Math.Floor(minutes);
            seconds = Math.Floor(seconds);
            tenths = Math.Floor(tenths);
            return string.Format("{0}° ", digrees.ToString()) + string.Format("{0}' ", minutes.ToString()) + seconds.ToString() + "\"";
        }

        void geometryService_Failed(object sender, ESRI.ArcGIS.Client.Tasks.TaskFailedEventArgs e)
        {
            //throw new NotImplementedException();
        }

        private void setEnvironmentEmpty()
        {
            _restCallCount = 0;
            
            LatLong = " ";
             Latitude = " ";
            Longitude = " ";
            CorrosionArea = "Non-Corrosion";
            RCZ = "No";
            FireArea = "No Designation";
            WildLandFire = "No Designation";
            SummerTemperature = "No Designation";
            SnowLoadingArea = "Light";
            ClimateZoneCode = "No Designation";
            FireIndexArea = "No records";
            FireIndexControlArea = "No records";
            Elevation = "NAN";
            City = "No Data";
            Districts = "No Data";
            Division = "No Data";
            Country = "No Data";
            HistoricPeakWind = "No Data";
            InsulationAreas = "No Data";
            PrimaryVoltageArea = "No Data";

            SurgeProtectionDistricts = "No Data";
            HighFireThreatDistrict = "Tier 1";  //INC000004493364
        }



        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = this.PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #region Customer Properties
        private string _Latitude = "";
        public string Latitude
        {
            get { return _Latitude; }
            set
            {
                _Latitude = value;
                OnPropertyChanged("Latitude");
            }
        }

        private string _Longitude = "";
        public string Longitude
        {
            get { return _Longitude; }
            set
            {
                _Longitude = value;
                OnPropertyChanged("Longitude");
            }
        }
        private string _LatLong = "";
        public string LatLong
        {
            get { return _LatLong; }
            set
            {
                _LatLong = value;
                OnPropertyChanged("LatLong");
            }
        }

        private string  _Elevation = "NAN";
        public string  Elevation
        {
            get { return _Elevation; }
            set
            {
                _Elevation = value;
                OnPropertyChanged("Elevation");
            }
        }

        private string _CorrosionArea = "";
        public string CorrosionArea
        {
            get { return _CorrosionArea; }
            set
            {
                _CorrosionArea = value;
                OnPropertyChanged("CorrosionArea");
            }

        }
        
        private string _Rcz = "";
        public string RCZ
        {
            get { return _Rcz ; }
            set
            {
                _Rcz = value;
                OnPropertyChanged("RCZ");
            }
        }

        private string _FireArea = "";
        public string FireArea
        {
            get { return _FireArea; }
            set
            {
                _FireArea = value;
                OnPropertyChanged("FireArea");
            }
        }

        private string _WildLandFire = "";
        public string WildLandFire
        {
            get { return _WildLandFire; }
            set
            {
                _WildLandFire = value;
                OnPropertyChanged("WildLandFire");
            }
        }

        private string _SummerTemperature = "";
        public string SummerTemperature
        {
            get { return _SummerTemperature; }
            set
            {
                _SummerTemperature = value;
                OnPropertyChanged("SummerTemperature");
            }
        }
        private string _SnowLoadingArea = "";
        public string SnowLoadingArea
        {
            get { return _SnowLoadingArea; }
            set
            {
                _SnowLoadingArea = value;
                OnPropertyChanged("SnowLoadingArea");
            }
        }
        private string _ClimateZoneCode = "";
        public string ClimateZoneCode
        {
            get { return _ClimateZoneCode; }
            set
            {
                _ClimateZoneCode = value;
                OnPropertyChanged("ClimateZoneCode");
            }
        }
        private string _FireIndexArea = "";
        public string FireIndexArea
        {
            get { return _FireIndexArea; }
            set
            {
                _FireIndexArea = value;
                OnPropertyChanged("FireIndexArea");
            }
        }
        //

        private string _City = "";
        public string City
        {
            get { return _City; }
            set
            {
                _City = value;
                OnPropertyChanged("City");
            }

        }
        private string _Districts = "";
        public string Districts
        {
            get { return _Districts; }
            set
            {
                _Districts = value;
                OnPropertyChanged("Districts");
            }

        }
        private string _Division = "";
        public string Division
        {
            get { return _Division; }
            set
            {
                _Division = value;
                OnPropertyChanged("Division");
            }

        }
        private string _Country = "";
        public string Country
        {
            get { return _Country; }
            set
            {
                _Country = value;
                OnPropertyChanged("Country");
            }

        }
        private string _HistoricPeakWind  = "";
        public string HistoricPeakWind
        {
            get { return _HistoricPeakWind; }
            set
            {
                _HistoricPeakWind = value;
                OnPropertyChanged("HistoricPeakWind");
            }

        }
        private string _InsulationAreas = "";
        public string InsulationAreas
        {
            get { return _InsulationAreas; }
            set
            {
                _InsulationAreas = value;
                OnPropertyChanged("InsulationAreas");
            }

        }
        private string _PrimaryVoltageArea = "";
        public string PrimaryVoltageArea
        {
            get { return _PrimaryVoltageArea; }
            set
            {
                _PrimaryVoltageArea = value;
                OnPropertyChanged("PrimaryVoltageArea");
            }

        }
        private string _SurgeProtectionDistricts = "";
        public string SurgeProtectionDistricts
        {
            get { return _SurgeProtectionDistricts; }
            set
            {
                _SurgeProtectionDistricts = value;
                OnPropertyChanged("SurgeProtectionDistricts");
            }

        }
        private string _FireIndexControlArea = "";
        public string FireIndexControlArea
        {
            get { return _FireIndexControlArea; }
            set
            {
                _FireIndexControlArea = value;
                OnPropertyChanged("FireIndexControlArea");
            }

        }

        //INC000004493364
        private string _HighFireThreatDistrict = "";
        public string HighFireThreatDistrict
        {
            get { return _HighFireThreatDistrict; }
            set
            {
                _HighFireThreatDistrict = value;
                OnPropertyChanged("HighFireThreatDistrict");
            }

        }
        #endregion

        #region Dependency Properties

        public bool DisplayResults
        {
            get { return (bool)GetValue(DisplayResultsProperty); }
            set { SetValue(DisplayResultsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for DisplayResults.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DisplayResultsProperty =
            DependencyProperty.Register("DisplayResults", typeof(bool), typeof(EnvironmentalInfo), null);


        #endregion


        public void StopCurrentAsyncTasks()
        {
            foreach (TaskBase task in CurrentlyExecutingTasks)
            {
                if (task != null && task.IsBusy)
                {
                    task.CancelAsync();
                }

                if (task != null && task is Geoprocessor)
                {
                    ((Geoprocessor)task).CancelJobAsync(_jobid);
                    ((Geoprocessor)task).CancelJobStatusUpdates(_jobid);
                }
            }


            CurrentlyExecutingTasks.Clear();
        }

        private void CloseWindowCmd_Click(object sender, RoutedEventArgs e)
        {
            RemoveEnvironmentalReportMarker();
            ConfigUtility.StatusBar.Text  = "";
            _restCallCount = 0;
            DisplayResults = false;
            StopCurrentAsyncTasks();
        }

        private void CopyWindowCmd_Click(object sender, RoutedEventArgs e)
        {
            String _strCopyClicp = "";
            _strCopyClicp = "Environmental Report\n\n";
            _strCopyClicp = _strCopyClicp + "Latitude:" + Latitude + "\n";
            _strCopyClicp = _strCopyClicp + "Longitude:" + Longitude + "\n";
            _strCopyClicp = _strCopyClicp + "Elevation:" + Elevation + "\n";
            _strCopyClicp = _strCopyClicp + "City:" + City + "\n";
            _strCopyClicp = _strCopyClicp + "District:" + Districts + "\n";
            _strCopyClicp = _strCopyClicp + "Division:" + Division + "\n";
            _strCopyClicp = _strCopyClicp + "Country:" + Country + "\n";
            _strCopyClicp = _strCopyClicp + "Corrosion Area:" + CorrosionArea + "\n";
            _strCopyClicp = _strCopyClicp + "Inside Raptor Concentration Zone (RCZ)?:" + RCZ + "\n";
            _strCopyClicp = _strCopyClicp + "Fire Area Designation:" + FireArea + "\n";
            _strCopyClicp = _strCopyClicp + "PGE Wildland Fire Management Area:" + WildLandFire + "\n";
            _strCopyClicp = _strCopyClicp + "Historical Peak Wind:" + HistoricPeakWind + "\n";
            _strCopyClicp = _strCopyClicp + "Summer Temperature District:" + SummerTemperature + "\n";
            _strCopyClicp = _strCopyClicp + "Snow Loading Area:" + SnowLoadingArea + "\n";
            _strCopyClicp = _strCopyClicp + "Climate Zone Code:" + ClimateZoneCode + "\n";
            _strCopyClicp = _strCopyClicp + "Fire Index Area:" + FireIndexArea + "\n";
            _strCopyClicp = _strCopyClicp + "Fire Index Control Centers:" + FireIndexArea + "\n";
            _strCopyClicp = _strCopyClicp + "High Fire Threat District:" + HighFireThreatDistrict + "\n"; //INC000004493364
            _strCopyClicp = _strCopyClicp + "Surge Protection Districts:" + SurgeProtectionDistricts + "\n";
            _strCopyClicp = _strCopyClicp + "Primary Voltage Area:" + PrimaryVoltageArea + "\n";
            _strCopyClicp = _strCopyClicp + "Insulation Areas:" + InsulationAreas ;
         
         

            Clipboard.SetText(_strCopyClicp);
        }
    }

    public static class WriteableBitMapExtension
    {
        private const int SizeOfArgb = 4;
        public static WriteableBitmap Crop(this WriteableBitmap bmp, int x, int y, int width, int height)
        {
            var srcWidth = bmp.PixelWidth;
            var srcHeight = bmp.PixelHeight;

            // If the rectangle is completly out of the bitmap
            if (x > srcWidth || y > srcHeight)
            {
                return new WriteableBitmap(0, 0);
            }

            // Clamp to boundaries
            if (x < 0) x = 0;
            if (x + width > srcWidth) width = srcWidth - x;
            if (y < 0) y = 0;
            if (y + height > srcHeight) height = srcHeight - y;

            // Copy the pixels line by line using fast BlockCopy
            var result = new WriteableBitmap(width, height);
            for (var line = 0; line < height; line++)
            {
                var srcOff = ((y + line) * srcWidth + x) * SizeOfArgb;
                var dstOff = line * width * SizeOfArgb;
                Buffer.BlockCopy(bmp.Pixels, srcOff, result.Pixels, dstOff, width * SizeOfArgb);
            }
            return result;
        }
    }
}
