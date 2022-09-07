using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using PGE.Common.CommandFlags;
using System.Threading;
using PGE.Common.Delivery.Diagnostics;

namespace TestFMEServer
{
    class Program
    {
        private static readonly Log4NetLogger _logger = new Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "ED46.log4net.config");

        private static string _email;
        private static string _restServiceURL;
        private static bool _isSync = false;
        private static bool _showHelp = false;

        static void Main(string[] args)
        {
            try
            {
                ProcessArguments(args);

                IList<RestArguments> restArgumentsList = GetRestArgumentsList();

                Program program = new Program();
                program.Run(restArgumentsList);
            }
            catch (Exception exception)
            {
                _logger.Error(exception.ToString());
            }
            finally
            {
#if DEBUG
                Console.WriteLine("Press any key to exit.");
                Console.ReadKey(true);
#endif
            }

        }

        public static IList<RestArguments> GetRestArgumentsList()
        {
            IList<RestArguments> restArgumentsList = new List<RestArguments>();

            string[] lines = System.IO.File.ReadAllLines(@"exports.csv");
            foreach (string line in lines)
            {
                string[] values = line.Split(',');
                string coords = values[0];
                string jobName = values[1];
                string scale = "100";
                string email = _email;
                // New format
                if (values.Length > 2)
                {
                    email = values[0].Trim(' ');
                    coords = values[1].Trim(' ');
                    jobName = values[2].Trim(' ');
                    scale = values[3].Trim(' ');
                }
                else
                {
                    coords = values[0].Trim(' ');
                    jobName = values[1].Trim(' ');                    
                }

                RestArguments restArguments = new RestArguments { Coords = coords, JobName = jobName, Email = email, Scale = scale};
                restArgumentsList.Add(restArguments);
            }

            return restArgumentsList;
        }
        public static void ProcessArguments(string[] args)
        {
            _logger.Debug(MethodBase.GetCurrentMethod().Name);

            // -m async -e p1pc@pge.com -r http://WSGO496902:8080/fmedatadownload/Data/gis_to_cad.fmw
            var flags = new FlagParser()
                {
                    { "h?", "help", false, false, "Show usage descriptions.", f => _showHelp = f != null },
                    { "m", "mode", true, false, "Mode [sync | async]", f => _isSync = f.ToUpper() == "SYNC" },
                    { "e", "email", true, false, "Email Address to send Jobs to", f => _email = f },
                    { "r", "restService", true, false, "FME Rest Service URL", f => _restServiceURL = f },
                };
            flags.Parse(args);

            // Default
            _logger.Debug("Using Sync Mode [ " + _isSync.ToString() + " ]");

        }


        public void Run(IList<RestArguments> restArgumentsList )
        {
            if (_isSync == false)
            {
                foreach (RestArguments restArguments in restArgumentsList)
                {
                    CallFMEAsync(restArguments);
                }
            }
            else
            {
                CreateFMESyncTasks(restArgumentsList);
                
            }
        }

        private void CreateFMESyncTasks(IList<RestArguments> restArgumentsList)
        {
            Task[] tasks = new Task[restArgumentsList.Count];
            int i = 0;
            //TODO: chunk threads by some limit e.g. 50
            foreach (RestArguments restArguments in restArgumentsList)
            {
                tasks[i++] = Task.Factory.StartNew(() =>
                {
                    CallFMESync(restArguments);
                });
            }
            while (tasks.Any(t => !t.IsCompleted))
            {
                Thread.Yield();
            } //spin wait
            
        }

        static private void CallFMESync(RestArguments restArguments)
        {
            _logger.Debug("CaLLFMESync Thread [ " + Thread.CurrentThread.ManagedThreadId.ToString() + " ]");

            Dictionary<string, string> restParameters = GetPostFormParameters(restArguments);
            string postURL = _restServiceURL;
            WebClient wc = new WebClient();
            wc.Headers["Content-type"] = "application/x-www-form-urlencoded";
            string result = wc.UploadString(new Uri(postURL), "POST", "?" + restParameters.ToQueryString());
            _logger.Debug("Thread [ " + Thread.CurrentThread.ManagedThreadId.ToString() + " ] [ " + result + " ]");

            if (!result.Contains("success"))
            {
                _logger.Error("Thread [ " + Thread.CurrentThread.ManagedThreadId.ToString() + " ] failure");                
                System.Environment.Exit(1);
            }
            _logger.Debug("Finished waiting on Thread [ " + Thread.CurrentThread.ManagedThreadId.ToString() + " ]");
        }

        static private Dictionary<string, string> GetPostFormParameters(RestArguments restArguments)
        {
            string fmePolygonCoordinates = restArguments.Coords;
            string fileName = DateTime.Now.ToString("yyyyMMdd_HHmmss");

            Dictionary<string, string> restParameters = new Dictionary<string, string>();
            //restParameters.Add("_GEODBInSearchFeature_GEODATABASE_SDE", fmePolygonCoordinates);
            restParameters.Add("scale", restArguments.Scale);
            restParameters.Add("opt_showresult", "false");
            restParameters.Add("opt_responseformat", "json");
            if (_isSync == true)
            {
                restParameters.Add("opt_servicemode", "sync");                
            }
            else
            {
                restParameters.Add("opt_servicemode", "async");
                restParameters.Add("opt_requesteremail", restArguments.Email);                
            }
            restParameters.Add("output_file_name", restArguments.JobName);

            return restParameters;
        }

        public void CallFMEAsync(RestArguments restArguments)
        {
            Dictionary<string, string> restParameters = GetPostFormParameters(restArguments);
//            string postURL =  "http://WSGO496902:8080/fmedatadownload/Data/gis_to_cad.fmw";
            string postURL = _restServiceURL;
            WebClient wc = new WebClient();
            wc.UploadStringCompleted += new UploadStringCompletedEventHandler(UploadStringCompleted);
            wc.Headers["Content-type"] = "application/x-www-form-urlencoded";
            wc.UploadStringAsync(new Uri(postURL), "POST", "?" + restParameters.ToQueryString() + "&_GEODBInSearchFeature_GEODATABASE_SDE=" + restArguments.Coords.Replace(" ", "%20"));            
        }

        private void UploadStringCompleted(object sender, UploadStringCompletedEventArgs e)
        {
            if (e.Result.Contains("SUCCEEDED"))
            {
                _logger.Debug("Requested");
            }
            else
            {
                _logger.Error(e.Result);
            }
        }
    }

    class RestArguments
    {
        public string Coords;
        public string JobName;
        public string Email;
        public string Scale;
    }
    static class Extensions
    {
        public static string ToQueryString(this Dictionary<string, string> source)
        {
            return String.Join("&", source.Select(kvp => String.Format("{0}={1}", HttpUtility.UrlEncode(kvp.Key), HttpUtility.UrlEncode(kvp.Value))).ToArray());
        }
    }

}
