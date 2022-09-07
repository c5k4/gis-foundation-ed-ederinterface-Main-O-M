using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows;
using System.Collections.ObjectModel;
using ESRI.ArcGIS.Client;
using ESRI.ArcGIS.Client.Printing;
using ESRI.ArcGIS.Client.Tasks;
using ESRI.ArcGIS.Client.Geometry;
using PGnE.Printing;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Net;
namespace ArcFMSilverlight.Controls.CADExport
{
    public class PngExport
    {
        //PrintServiceUrl
        #region private declarations
        private int flag;
        private double _printScale;
        private double _printPngScale;
        private string _fmePolygonCoordinates;
        private string _emailId;
        private string _outputFileName;
        private string _exportFormat;
        private string _butterflyFacilityId;
        private PrintTask _printTask = null;
        private int _printDpi = 300;
        private bool _printStandardMap = false;
        string _printFormat = "PNG32";
        private Map _map = null;
        private string _printTemplate = string.Empty;
        private Dictionary<string, string> _customTextElements = new Dictionary<string, string>();
        private Uri _printTaskUrl = null;
        private Uri _extractSendTaskUrl = null;
        private Envelope _printAreaExtent = null;
        private string _userEmail;
        private int _minimumAsyncSize;
        private IList<string> _currentJobs = new List<string>();
        private String[] _MapTypes = null;
        public bool DisablePopups { get; set; }
        System.Collections.Hastable outputRecords = new System.Collections.Hastable();
        System.Collections.Hastable InputMapTypes = new System.Collections.Hastable();
        string currentMapType = string.Empty;
        string strPrimaryURL = string.Empty;
        string strSecondaryURL = string.Empty;
        public string strDcviewURL = string.Empty;
        string strDuctURL = string.Empty;
        public string strButterflyURL = string.Empty;
        public string strStreetLightURL = string.Empty;
        string strLandbaseURL = string.Empty;
        string strSchematicsURL = string.Empty;
        public string lstButterflyVisibleLayers = string.Empty;
        public string lstLandBaseVisibleLayers = string.Empty;
        public string FMEPNGExportURL = string.Empty;
        ESRI.ArcGIS.Client.Symbols.TextSymbol _textSymbol=null;
        ESRI.ArcGIS.Client.GraphicsLayer graphicsLayer=null;
        ESRI.ArcGIS.Client.Geometry.Polygon _polygon=null;

        #endregion private declarations

        #region ctor

        /// <summary>
        /// Constructor.
        /// </summary>
        public PngExport(Uri printTaskUrl)
        {
            _printTaskUrl = printTaskUrl;
            //_extractSendTaskUrl = extractSendTaskUrl;
            //_userEmail = userEmail;
            //_minimumAsyncSize = minimumAsyncSize;
            CreatePrintTask();
        }

        #endregion ctor

        #region methods/properties

        /// <summary>
        /// Starts the printing job.
        /// Because of the lack of access through the api to Web_Map_as_JSON we have to create a dummy service
        /// on the server to make an Extract-and-Send service so that it can use the PrintTask.
        /// </summary>
        /// <param name="scale"></param>
        /// <param name="map"></param>
        /// <param name="printTemplate"></param>
        /// <param name="customTextElements"></param>
        /// <param name="printFormat"></param>
        /// <param name="printDpi"></param>
        /// <param name="printStandardMap"></param>
        public void PrintED50(string fmePolygonCoordinates,double scale,string outputFileName,string email,string selectedExportFormat,string facilityID, Map map, string printTemplate, Envelope printAreaExtent,
                          Dictionary<string, string> customTextElements = null,
                          string printFormat = "PNG32",
                          int printDpi = 300, bool printStandardMap = false,string[] MapTypes = null,ESRI.ArcGIS.Client.Symbols.TextSymbol textSymbol=null,ESRI.ArcGIS.Client.GraphicsLayer glgraphicsLayer=null,ESRI.ArcGIS.Client.Geometry.Polygon polygon=null)
        {
            if (printAreaExtent == null) throw new IndexOutOfRangeException("Invalid print extent.");
            _printScale = scale;
            _printPngScale = scale*12;
            flag = 0;
            _fmePolygonCoordinates=fmePolygonCoordinates;
            _outputFileName = outputFileName;
            _emailId = email;
            _exportFormat = selectedExportFormat;
            _butterflyFacilityId = facilityID;
            _map = map;
            _printTemplate = printTemplate;
            _printAreaExtent = printAreaExtent;
            if (customTextElements != null)
            {
                _customTextElements = customTextElements;
            }
            //_printFormat = printFormat;
            _printDpi = printDpi;
            _printStandardMap = printStandardMap;

            _MapTypes = MapTypes;
            _textSymbol=textSymbol;
            graphicsLayer=glgraphicsLayer;
            _polygon=polygon;
            
            //set for first time.
            currentMapType = _MapTypes[0].ToString();
            

            foreach (string s in _MapTypes)
            { InputMapTypes.Add(s,""); }

            foreach (string s in _MapTypes)
            { outputRecords.Add(s,""); }

            getLayerUrl();
            if (_map == null)
            {
                throw new IndexOutOfRangeException("Map is null (Print method)");
            }

            if (_printTaskUrl == null)
            {
                throw new IndexOutOfRangeException("Print Task URL is not set (Print method)");
            }

            ExportMap();
        }

        /// <summary>
        /// Exports the map to the specified format.
        /// </summary>
        private void ExportMap()
        {
            //int lastDPI = _printDpi;

            PrintParameters printParameters = InitializePrintParameters();
            if (printParameters == null)
            {
                throw new Exception("Failed to initialize PrintParameters");
            }
            _printTask.Url = _printTaskUrl.ToString();
            
            _printTask.SubmitJobAsync(printParameters);
            //_printDpi = lastDPI;
        }

        private bool LayoutIsLarge(string layoutTemplate)
        {
            MatchCollection sizeMatch = Regex.Matches(layoutTemplate.Replace("-", "."), @"[\d\.]+");
            foreach (Match match in sizeMatch)
            {
                if (Convert.ToDouble(match.Value) >= _minimumAsyncSize)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Initializes PrintParameters class.
        /// </summary>
        /// <returns></returns>
        private PrintParameters InitializePrintParameters()
        {

                PrintParameters printParameters = null;                
                LayoutOptions layoutOptions = new LayoutOptions();
                layoutOptions.LegendOptions = null;

                IEnumerable<Layer> layers = GetLayers();
                
                //Envelope extent = new Envelope(_map.Extent.XMin,_map.Extent.YMin,_map.Extent.XMax,_map.Extent.YMax);
                Envelope extent = _printAreaExtent;


                if (currentMapType == "BUTTERFLY")
                {
                   
                    
                    printParameters = new PrintParameters(_map)
                    {
                        MapOptions = new MapOptions(extent),
                        LayoutOptions = layoutOptions
                    };

                    printParameters.MapOptions.Scale = 50;
                }
                else
                {
                    printParameters = new PrintParameters(layers, extent)
                    {
                        MapOptions = new MapOptions(extent),
                        LayoutOptions = layoutOptions
                    };

                    printParameters.MapOptions.Scale = _printPngScale;
                }
                
            
            printParameters.ExportOptions = new ExportOptions() { Dpi = _printDpi };
            printParameters.LayoutTemplate = _printTemplate;
            printParameters.Format = _printFormat;
           
           

            if ((_customTextElements != null) && (_customTextElements.Count > 0))
            {
                printParameters.LayoutOptions = new LayoutOptions();
                printParameters.LayoutOptions.CustomTextElements = new Dictionary<string, string>();
                foreach (KeyValuePair<string, string> kvp in _customTextElements)
                {
                    printParameters.LayoutOptions.CustomTextElements[kvp.Key] = kvp.Value;

                }
            }
            else
            {
                // This has seemingly stopped working at 10.2.1 in both Desktop and Server
                // Running Server > Export Web Map GP with various values in the LayoutOptions.LegendOptions json tag causes
                // the GP tool to execute indefinitely
                printParameters.LayoutOptions = new LayoutOptions();
                printParameters.LayoutOptions.LegendOptions = new LegendOptions() { LegendLayers = null };
            }

            return printParameters;
        }

        private IEnumerable<Layer> GetLayers()
        {
            ArcGISDynamicMapServiceLayer oppLayer = new ArcGISDynamicMapServiceLayer();

            //primary.Url = "http://edgisappqa01:6080/arcgis/rest/services/Data/Primary_50/MapServer";
            //primary.Visible = true;

            ArcGISDynamicMapServiceLayer landbaseLayer = new ArcGISDynamicMapServiceLayer();
            landbaseLayer.Url = strLandbaseURL;
            landbaseLayer.Visible = true;
            int[] myVisibleLayers = lstLandBaseVisibleLayers.Split(',').Select(s => Convert.ToInt32(s)).ToArray();
            landbaseLayer.VisibleLayers = myVisibleLayers;
                       

            if (currentMapType == "PRIMARY")
            {
                oppLayer.Url = strPrimaryURL;
                oppLayer.Visible = true;                               
            }
            else if (currentMapType == "SECONDARY")
            {
                oppLayer.Url = strSecondaryURL;
                oppLayer.Visible = true;                               
            }
            else if (currentMapType == "DUCTVIEW")
            {
                oppLayer.Url = strDuctURL;
                oppLayer.Visible = true;
            }
            else if (currentMapType == "DCVIEW")
            {
                oppLayer.Url = strDcviewURL;
                oppLayer.Visible = true;
            }
            else if (currentMapType == "BUTTERFLY")
            {
                oppLayer.Url = strButterflyURL;
                oppLayer.Visible = true;
                //Need to set visible layers of UFM here
                myVisibleLayers = null;
                myVisibleLayers = lstButterflyVisibleLayers.Split(',').Select(s => Convert.ToInt32(s)).ToArray();
                oppLayer.VisibleLayers = myVisibleLayers;
               
            }
            else if (currentMapType == "STREETLIGHT")
            {
                oppLayer.Url = strStreetLightURL;
                oppLayer.Visible = true;
            }
            else if (currentMapType == "SCHEMATICS")
            {
                oppLayer.Url = strSchematicsURL;
                oppLayer.Visible = true;
                
                //Need to set visible layers of Schematics here
            }

            List<Layer> lstLyr = new List<Layer>();  
           
            lstLyr.Add(landbaseLayer);
            lstLyr.Add(oppLayer);
            IEnumerable<Layer> lyrEnum = lstLyr;
            return lyrEnum;

        }

       

        private void getLayerUrl()
        {
            if (Application.Current.Host.InitParams.ContainsKey("Config"))
            {
                string config = Application.Current.Host.InitParams["Config"];
                //var attribute = "";
                config = System.Windows.Browser.HttpUtility.HtmlDecode(config).Replace("***", ",");
                XElement elements = XElement.Parse(config);
                foreach (XElement element in elements.Elements())
                {
                    if (element.Name.LocalName == "Layers")
                    {
                        if (element.HasElements)
                        {
                            var i = 0;

                            foreach (XElement childelement in element.Elements())
                            {
                                if (childelement.Attribute("MapServiceName").Value.ToString() == "Elec Dist 50 Scale Primary View")
                                {
                                    strPrimaryURL = childelement.Attribute("Url").Value.ToString();
                                }
                                else if (childelement.Attribute("MapServiceName").Value.ToString() == "Elec Dist 50 Scale Secondary View")
                                {
                                    strSecondaryURL = childelement.Attribute("Url").Value.ToString();
                                }
                                else if (childelement.Attribute("MapServiceName").Value.ToString() == "Elec Dist 50 Scale Duct View")
                                {
                                    strDuctURL = childelement.Attribute("Url").Value.ToString();
                                }
                                else if (childelement.Attribute("MapServiceName").Value.ToString() == "Schematics")
                                {
                                    strSchematicsURL = childelement.Attribute("Url").Value.ToString();
                                }
                                else if (childelement.Attribute("MapServiceName").Value.ToString() == "Commonlandbase")
                                {
                                    strLandbaseURL = childelement.Attribute("Url").Value.ToString();
                                }
                                
                            }
                        }
                    }
                }
            }
        }



        /// <summary>
        /// Initializes the asynchronous print task.
        /// </summary>
        private void CreatePrintTask()
        {
            if (string.IsNullOrEmpty(_printTaskUrl.AbsolutePath) == true)
            {
                throw new IndexOutOfRangeException("Print Service has an invalid url: " + _printTaskUrl.AbsolutePath);
            }
            _printTask = new PrintTask(_printTaskUrl.AbsoluteUri);
            if (Application.Current.Resources.Contains("UpdateDelay"))
            {
                _printTask.UpdateDelay = (Int32)Application.Current.Resources["UpdateDelay"];
            }

            _printTask.DisableClientCaching = true;
            _printTask.JobCompleted += PrintTask_JobCompleted;
            _printTask.StatusUpdated += PrintTask_StatusUpdated;
            _printTask.GetServiceInfoCompleted += PrintTask_GetServiceInfoCompleted;
        }

        /// <summary>
        /// Gets the list of configured templates.
        /// </summary>
        public void GetLayoutsAsync()
        {
            _printTask.GetServiceInfoAsync();
        }

        /// <summary>
        /// This event happens when a print job is done.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void PrintTask_JobCompleted(object sender, PrintJobEventArgs e)
        {
            //if (_currentJobs.Contains(e.PrintJobInfo.JobId))
            //{
            //    _currentJobs.Remove(e.PrintJobInfo.JobId);
            //}

            //// This is the case of the ExtractSend, see top of this file
            //if (e.PrintJobInfo.JobStatus == esriJobStatus.esriJobSucceeded && e.PrintResult == null) return;

            //if (e.PrintJobInfo.JobStatus == esriJobStatus.esriJobCancelled)
            //{
            //    OnPrintJobCancelled(EventArgs.Empty);
            //    return;
            //}

            //if (e.Error != null)
            //{
            //    //throw new Exception(e.Error.Message);

            //    //Do not throw exception. Alert the user but continue without crashing.
            //    MessageBox.Show("An error occured on the print server while creating your Ad Hoc map."
            //                    + Environment.NewLine
            //                    + Environment.NewLine
            //                    +
            //                    "Please try printing your map again. If the problem persists, please contact the application administrator."
            //                    + Environment.NewLine
            //                    + Environment.NewLine
            //                    + "Error message returned from the server: " + Environment.NewLine + e.Error.Message,
            //        "Print server error encountered while creating Ad Hoc Map",
            //        MessageBoxButton.OK);

            //    OnPrintJobCompleted(EventArgs.Empty);
            //}
            //else
            //{
            //    string s = "Your PDF Map has been created and will now open in a new browser tab.";
            //    MessageBox.Show(s, "PDF Map Successfully Created", MessageBoxButton.OK);

            //    OnPrintJobCompleted(EventArgs.Empty);
            //    System.Windows.Browser.HtmlPage.Window.Navigate(e.PrintResult.Url, "_blank");
            //}
        }

        /// <summary>
        /// This event updates the caller about the status of a print job.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void PrintTask_StatusUpdated(object sender, JobInfoEventArgs e)
        {
            if (e.JobInfo.JobStatus == esriJobStatus.esriJobSubmitted)
            {
                //find number o
                if (InputMapTypes.Count > 0)
                {
                    outputRecords[currentMapType] = e.JobInfo.JobId;
                    InputMapTypes.Remove(currentMapType);
                    if (InputMapTypes.Count > 0)
                    {
                        currentMapType = InputMapTypes.Keys.ElementAt(0).ToString();
                        ExportMap();
                    }
                }
                if (InputMapTypes.Count == 0 && flag==0)
                {
                   // MessageBox.Show(outputRecords.Count.ToString());

                    //FME format
                    //PRIMARY;SECONDARY;DUCT;DC;BUTTERFLY;STREETLIGHT
                    //Send request to FME with JOBID's
                    flag = 1;
                    
                    WebClient wc = new WebClient();
                    wc.UploadStringCompleted += new UploadStringCompletedEventHandler(UploadStringCompleted);
                    wc.Headers["Content-type"] = "application/x-www-form-urlencoded";
                    // For some reason, "+" doesn't work in place of spaces for FME Server but %20 does

                    Dictionary<string, string> restParameters = new Dictionary<string, string>();
                    restParameters.Add("opt_showresult", "false");
                    restParameters.Add("opt_servicemode", "async");
                    restParameters.Add("scale", _printScale.ToString());
                    restParameters.Add("output_file_name", _outputFileName);
                    restParameters.Add("MAPTYPE", "ED");
                    restParameters.Add("EMAIL", _emailId);
                   
                    restParameters.Add("FACILITYID", _butterflyFacilityId);

                    restParameters.Add("MAPTYPES", string.Join(";", outputRecords.Keys));
                   // restParameters.Add("INPUT_URL",_printTaskUrl+"/");
                    if(outputRecords.ContainsKey("PRIMARY"))
                    restParameters.Add("Pry_JOBID",outputRecords["PRIMARY"].ToString());  
                    else restParameters.Add("Pry_JOBID","-1" );

                    if(outputRecords.ContainsKey("SECONDARY"))
                    restParameters.Add("Sec_JOBID",outputRecords["SECONDARY"].ToString());  
                    else restParameters.Add("Sec_JOBID","-1" );

                    if(outputRecords.ContainsKey("DCVIEW"))
                    restParameters.Add("DC_JOBID",outputRecords["DCVIEW"].ToString());  
                    else restParameters.Add("DC_JOBID","-1" );

                    if(outputRecords.ContainsKey("DUCTVIEW"))
                    restParameters.Add("DUCT_JOBID",outputRecords["DUCTVIEW"].ToString());  
                    else restParameters.Add("DUCT_JOBID","-1" );

                    if(outputRecords.ContainsKey("BUTTERFLY"))
                    restParameters.Add("BUTTERFLY_JOBID",outputRecords["BUTTERFLY"].ToString());  
                    else restParameters.Add("BUTTERFLY_JOBID","-1" );

                    if(outputRecords.ContainsKey("STREETLIGHT"))
                    restParameters.Add("STREETLIGHT_JOBID",outputRecords["STREETLIGHT"].ToString());  
                    else restParameters.Add("STREETLIGHT_JOBID","-1" );
                   
                   
                    if (_exportFormat == "DWG")
                    {
                        restParameters.Add("REQUESTTYPE", "DWG");
                    }
                    else if (_exportFormat == "PNG")
                    {
                        restParameters.Add("REQUESTTYPE", "PNG");
                    }
                    else
                    {
                        restParameters.Add("REQUESTTYPE", "PNG;DWG");
                    }

                    string data = "?" + restParameters.ToQueryString() + "&_GEODBInSearchFeature_GEODATABASE_SDE=" + _fmePolygonCoordinates.Replace(" ", "%20");
                   // restParameters.Add("Destination File Copy Directory", @"C:\Program Files\FMEServer\DefaultResults");

                    wc.UploadStringAsync(new Uri(FMEPNGExportURL), "POST", "?" + restParameters.ToQueryString() + "&_GEODBInSearchFeature_GEODATABASE_SDE=" + _fmePolygonCoordinates.Replace(" ", "%20"));
                }
                    
            }
        }

        void UploadStringCompleted(object sender, UploadStringCompletedEventArgs e)
        {
            try
            {
                MapPoint mp;
                if (_polygon != null)
                {
                     mp = _polygon.Extent.GetCenter();
                }
                else
                {
                    mp = _map.Extent.GetCenter();
                }
                if (e.Result.Contains("SUCCEEDED"))
                {
                    Graphic graphicText = new Graphic()
                    {
                        Geometry = mp,
                        Symbol = _textSymbol,
                    };

                    graphicsLayer.Graphics.Add(graphicText);
                    _polygon = null;
                }

            }
            catch (Exception)
            {
                MessageBox.Show("An error occurred communicating with the ETL Server\r\n[ " + FMEPNGExportURL + "]");
            }
        }

        /// <summary>
        /// This event returns the information about the the type of service and configured print templates.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PrintTask_GetServiceInfoCompleted(object sender, ServiceInfoEventArgs e)
        {
            //if (e.ServiceInfo == null)
            //{
            //    OnPrintTemplates(new PrintTemplatesEventArgs(new List<string>()));
            //}

            //if (e.ServiceInfo.IsServiceAsynchronous == false)
            //{
            //    throw new InvalidOperationException("Print Service is not asynchronous: " + _printTaskUrl.AbsolutePath);
            //}

            //OnPrintTemplates(new PrintTemplatesEventArgs(e.ServiceInfo.LayoutTemplates));
        }

        /// <summary>
        /// Cancels a print job with the given job id.
        /// </summary>
        /// <param name="jobId"></param>
        public void CancelPrint(string jobId)
        {
            if (string.IsNullOrEmpty(jobId) == true) return;
            if (_printTask == null) return;

            _printTask.CancelJobAsync(jobId);
        }

        #endregion methods/properties

        #region events

        /// <summary>
        /// Event that fires when a job is submitted.
        /// </summary>
        public event EventHandler<PrintJobSubmittedEventArgs> PrintJobSubmitted;
        protected virtual void OnPrintJobSubmitted(PrintJobSubmittedEventArgs e)
        {
            EventHandler<PrintJobSubmittedEventArgs> handler = PrintJobSubmitted;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        /// <summary>
        /// Event that fires when a job is being cancelled.
        /// </summary>
        public event EventHandler<EventArgs> PrintJobCancelling;
        protected virtual void OnPrintJobCancelling(EventArgs e)
        {
            EventHandler<EventArgs> handler = PrintJobCancelling;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        /// <summary>
        /// Event that fires when a job is cancelled.
        /// </summary>
        public event EventHandler<EventArgs> PrintJobCancelled;
        protected virtual void OnPrintJobCancelled(EventArgs e)
        {
            EventHandler<EventArgs> handler = PrintJobCancelled;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        /// <summary>
        /// Event that fires when a job is completed.
        /// </summary>
        public event EventHandler<EventArgs> PrintJobCompleted;
        protected virtual void OnPrintJobCompleted(EventArgs e)
        {
            EventHandler<EventArgs> handler = PrintJobCompleted;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        /// <summary>
        /// Event that fires when a list of configured templates is available.
        /// </summary>
        public event EventHandler<PrintTemplatesEventArgs> PrintTemplates;
        protected virtual void OnPrintTemplates(PrintTemplatesEventArgs e)
        {
            EventHandler<PrintTemplatesEventArgs> handler = PrintTemplates;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        #endregion events


    }

    

    

}
