using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows;

using ESRI.ArcGIS.Client;
using ESRI.ArcGIS.Client.Printing;
using ESRI.ArcGIS.Client.Tasks;
using ESRI.ArcGIS.Client.Geometry;
namespace PGnE.Printing
{
    public class HiResPrint
    {
        #region private declarations

        private double _printScale;
        private PrintTask _printTask = null;
        private int _printDpi = 300;
        private bool _printStandardMap = false;
        private Utilites.PrintFormat _printFormat = Utilites.PrintFormat.PDF;
        private Map _map = null;
        private string _printTemplate = string.Empty;
        private Dictionary<string, string> _customTextElements = new Dictionary<string, string>();
        private Uri _printTaskUrl = null;
        private Uri _extractSendTaskUrl = null;
        private Envelope _printAreaExtent = null;
        private string _userEmail;
        private int _minimumAsyncSize;
        private IList<string> _currentJobs = new List<string>();
       
        public bool DisablePopups { get; set; }

        #endregion private declarations
        
        #region ctor

        /// <summary>
        /// Constructor.
        /// </summary>
        public HiResPrint(Uri printTaskUrl, Uri extractSendTaskUrl, string userEmail, int minimumAsyncSize)
        {
            _printTaskUrl = printTaskUrl;
            _extractSendTaskUrl = extractSendTaskUrl;
            _userEmail = userEmail;
            _minimumAsyncSize = minimumAsyncSize;
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
        public void Print(double scale, Map map, string printTemplate, Envelope printAreaExtent,
                          Dictionary<string, string> customTextElements = null,
                          Utilites.PrintFormat printFormat = Utilites.PrintFormat.PDF,
                          int printDpi = 300, bool printStandardMap = false)
        {
            if (printAreaExtent == null) throw new IndexOutOfRangeException("Invalid print extent.");
            _printScale = scale;
            _map = map;
            _printTemplate = printTemplate;
            _printAreaExtent = printAreaExtent;
            if (customTextElements != null)
            {
                _customTextElements = customTextElements;
            }
            _printFormat = printFormat;
            _printDpi = printDpi;
            _printStandardMap = printStandardMap;            
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
            if (LayoutIsLarge(printParameters.LayoutTemplate))
            {
                printParameters.Format = _userEmail;
                //_printDpi = 50;
                _printTask.Url = _extractSendTaskUrl.ToString();
            }
            else
            {
                _printTask.Url = _printTaskUrl.ToString();
            }
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
            
            if (_printStandardMap == false)
            {
                printParameters = new PrintParameters(_map)
                {
                    //MapOptions = new MapOptions(_map.Extent),
                    MapOptions = new MapOptions(_printAreaExtent),
                };
            }
            else
            {
                Map newMap = new Map();
                newMap.Extent = _map.Extent;

                LayoutOptions layoutOptions = new LayoutOptions();
                layoutOptions.LegendOptions = null;
                //layoutOptions.CustomTextElements

                printParameters = new PrintParameters(newMap)
                {
                    MapOptions = new MapOptions(newMap.Extent),
                    LayoutOptions = layoutOptions
                };
            }

            printParameters.ExportOptions = new ExportOptions() { Dpi = _printDpi };
            printParameters.LayoutTemplate = _printTemplate;
            printParameters.Format = Enum.GetName(typeof(Utilites.PrintFormat), _printFormat);
            printParameters.MapOptions.Scale = _printScale;

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

            if (printParameters.LayoutTemplate.ToString().Contains("CMCS") == true)
                printParameters.LayoutOptions.LegendOptions = new LegendOptions() { LegendLayers = null };   //INC000004403856 - For CMCS (Otherwise it runs indefinitely)

            return printParameters;
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
            if (_currentJobs.Contains(e.PrintJobInfo.JobId))
            {
                _currentJobs.Remove(e.PrintJobInfo.JobId);
            }

            // This is the case of the ExtractSend, see top of this file
            if (e.PrintJobInfo.JobStatus == esriJobStatus.esriJobSucceeded && e.PrintResult == null) return;

            if (e.PrintJobInfo.JobStatus == esriJobStatus.esriJobCancelled)
            {
                OnPrintJobCancelled(EventArgs.Empty);
                return;
            }

            if (e.Error != null)
            {
                //throw new Exception(e.Error.Message);

                //Do not throw exception. Alert the user but continue without crashing.
                string mapType = "";
                if(_printTemplate.Contains("CMCS") == true){
                    mapType = "CMCS";
                }
                else{
                    mapType = "Ad Hoc";
                }
                MessageBox.Show("An error occured on the print server while creating your " + mapType + " map."
                                + Environment.NewLine
                                + Environment.NewLine
                                +
                                "Please try printing your map again. If the problem persists, please contact the application administrator."
                                + Environment.NewLine
                                + Environment.NewLine
                                + "Error message returned from the server: " + Environment.NewLine + e.Error.Message,
                    "Print server error encountered while creating " + mapType + " Map",
                    MessageBoxButton.OK);

                OnPrintJobCompleted(EventArgs.Empty);
            }
            else
            {   
                    string s = "Your PDF Map has been created and will now open in a new browser tab.";
                    MessageBox.Show(s, "PDF Map Successfully Created", MessageBoxButton.OK);

                    OnPrintJobCompleted(EventArgs.Empty);
                    System.Windows.Browser.HtmlPage.Window.Navigate(e.PrintResult.Url, "_blank");
               
            }
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
                if (!_currentJobs.Contains(e.JobInfo.JobId) &&
                _printTask.Url == _extractSendTaskUrl.ToString())
                {
                    _currentJobs.Add(e.JobInfo.JobId);
                    // Msgbox to user reminding them
                    if (!DisablePopups)
                    {
                        string mapType = "";
                        if (_printTemplate.Contains("CMCS") == true)
                        {
                            mapType = "CMCS";
                        }
                        else
                        {
                            mapType = "Ad Hoc";
                        }
                        MessageBox.Show("You have selected a large size " + mapType + " map."
                                        + Environment.NewLine
                                        +
                                        "Your extract will be emailed to you when ready."
                                        + Environment.NewLine
                                        +
                                        "You do not need to keep this window open."
                                        + Environment.NewLine,
                            "Large " + mapType + " Map Print Job",
                            MessageBoxButton.OK);
                    }
                    else
                    {
                        OnPrintJobSubmitted(new PrintJobSubmittedEventArgs(e.JobInfo.JobId));
                    }
                    OnPrintJobCompleted(EventArgs.Empty);
                }
                else
                {
                    OnPrintJobSubmitted(new PrintJobSubmittedEventArgs(e.JobInfo.JobId));
                }
            }
            else if (e.JobInfo.JobStatus == esriJobStatus.esriJobCancelling)
            {
                OnPrintJobCancelling(EventArgs.Empty);
            }
        }

        /// <summary>
        /// This event returns the information about the the type of service and configured print templates.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PrintTask_GetServiceInfoCompleted(object sender, ServiceInfoEventArgs e)
        {
            try
            {
                if (e.ServiceInfo == null)
                {
                    OnPrintTemplates(new PrintTemplatesEventArgs(new List<string>()));
                }

                if (e.ServiceInfo.IsServiceAsynchronous == false)
                {
                    throw new InvalidOperationException("Print Service is not asynchronous: " + _printTaskUrl.AbsolutePath);
                }

                OnPrintTemplates(new PrintTemplatesEventArgs(e.ServiceInfo.LayoutTemplates));
            }
            catch (Exception ex)
            {
            }
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
