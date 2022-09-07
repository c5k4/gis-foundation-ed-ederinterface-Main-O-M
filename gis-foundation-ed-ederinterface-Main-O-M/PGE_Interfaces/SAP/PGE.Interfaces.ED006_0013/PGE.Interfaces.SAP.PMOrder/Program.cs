using PGE.Common.Delivery.Diagnostics;
using PGE.Interfaces.Integration.Framework.Utilities;
using PGE.Interfaces.SAP.WOSynchronization;
using System;
using System.Configuration;
using System.Threading;
using System.Reflection;
using System.Text;
using System.Diagnostics;

namespace Telvent.PGE.SAP.PMOrder
{
    /// <summary>
    /// Startup program for GIS Integration Component.
    /// </summary>
    public class Program
    {
        #region Private Variable
        /// <summary>
        /// Logger to log error / debug/ user information
        /// </summary>
        private static readonly Log4NetLogger _logger = new Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "SAPPMOrder.log4net.config");

        //Private Variable 
        private static string startTime;
        private static StringBuilder remark = default;
        private static StringBuilder argument = default;

        #endregion Private Variables

        #region public variable
        public static string comment = default;
        #endregion 

        #region Public Methods

        /// <summary>
        /// Executed on application startup
        /// </summary>
        /// <param name="args">Arguments passed. Pass full XML file name to process the file. If no arguments are passed, this methods processes all XML files present in the <c>PickupDirectory</c> folder</param>
        public static void Main(string[] args)
        {
            _logger.Info("Starting" + MethodBase.GetCurrentMethod().Name);
            //Load the config file for logging the details
            //log4net.Config.XmlConfigurator.ConfigureAndWatch(new FileInfo(Config.ReadConfigFromExeLocation().FilePath));
            //Below code added for EDGIS Rearch Project-v1t8
            startTime = DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss");
            //Object processes XML files
            PMOrderFileProcessor orderFileProcessor = new PMOrderFileProcessor();
            try
            {
                //Below code is added for EDGIS Rearch Project to read input data from staging table in place of file-v1t8
                if (args.Length == 0)
                {
                    // orderFileProcessor.ProcessFiles(xmlFiles); 
                    _logger.Info("Starting process");
                    orderFileProcessor.ProcessSAPData();
                    comment = "Process completed successfully";

                    _logger.Info("Calling IntExecutionSummary ");
                    ExecutionSummary(startTime, comment + PMOrderFileProcessor.remark , Argument.Done);
                }
                #region Below code is no longer needed for EDGIS Rearch Project to read input data from staging table in place of file.SAP data will be received via layer 7 and stored in staging area
                //Below Code commented for EDGIS Rearch Project to read input data from staging table in place of file.SAP data will be received via layer 7 and stored in staging area
                //File system conecpt is no more needed- v1t8
                //Get the Pickup directory of the XML Files
                // KeyValueConfigurationCollection settings = Config.ReadConfigFromExeLocation().AppSettings.Settings;
                //  string xmlPickUpPath = settings["PickupDirectory"].Value;
                //  string triggerFile = settings["TriggerFile"].Value;

                //Check for application is executed by Job Scheduler or commandline
                //for Job Scheduler args length is equal to 0.
                //    if (args.Length == 0)
                //{

                //    //Checks for PickupDirectory is exists
                //    if (Directory.Exists(xmlPickUpPath))
                //    {
                //        //make sure the trigger file exists. If it doesn't the XML files might be in the process of transferring
                //        if (File.Exists(xmlPickUpPath + triggerFile))
                //        {
                //            _logger.Info(xmlPickUpPath + " folder found.");
                //            //Get the XML files in the PickupDirectory and validate
                //            string[] xmlFiles = Directory.GetFiles(xmlPickUpPath, "*.xml");
                //            if (xmlFiles == null || xmlFiles.Length < 1)
                //            {
                //                _logger.Info("No XML file found for processing in '" + xmlPickUpPath + "'.");
                //                return;
                //            }

                //            //Validate the file names to check the existence of Date_Time at the end of the file
                //            if (IsFileNameValid(xmlFiles) == false)
                //            {
                //                _logger.Error("At least one of XML File names present in '" + xmlPickUpPath + "' folder is invalid. File Name format should be *yyyyMMddHHmmssxxx.xml");
                //                _logger.Error(string.Join(Environment.NewLine, xmlFiles));
                //                return;
                //            }

                //            //Sort the XML files by Date&Time present in the name at the end.
                //            //Date&Time format at the end is :yyyyMMdd_HHmmss Hence its length is 15 and including extension it is 19.
                //            //since we made the file name configurable lets just compare the entire file name. It is up to the user to make sure the naming convention will sort correctly
                //            Array.Sort<string>(xmlFiles, (x, y) => string.Compare(x, y, true));

                //            //Reverse the order to get the recent date file at the top
                //            Array.Reverse(xmlFiles);
                //            _logger.Info("Retrieved all XML files and sorted to have the latest XML file at the top.");

                //            //Process all XML files available in directory path                        
                //            orderFileProcessor.ProcessFiles(xmlFiles);

                //            //remove the trigger file to signify we have finished processing
                //            File.Delete(xmlPickUpPath + triggerFile);

                //            _logger.Info("Processing completed.");
                //        }
                //        else
                //        {
                //            _logger.Error("Trigger file: " + triggerFile + " does not exist. Terminating process");
                //        }
                //    }
                //    else
                //    {
                //        _logger.Error(xmlPickUpPath + " folder does not exist. Hence exits the process.");
                //    }
                //}
                //else
                //{
                //    string xmlFile = args[0];
                //    //Validate the XML file
                //    if (File.Exists(xmlFile))
                //    {
                //        Console.WriteLine("Started Processing....");
                //        orderFileProcessor.ProcessFiles(new string[] { xmlFile });
                //        Console.WriteLine("Processing completed. Please check the log file for full details.");
                //    }
                //    else
                //    {
                //        //Log the information to the console window
                //        _logger.Error("'" + xmlFile + "' XML file not found at the given path. Hence exits the process.");
                //        Console.WriteLine("'" + xmlFile + "' XML file not found at the given path. Hence exits the process.");
                //    }
                //}
                #endregion 
            }
            catch (Exception ex)
            {
                _logger.Error("Exception occures while executing the ED13 Interface :" + " " + ex.Message + MethodBase.GetCurrentMethod().Name);
                _logger.Error(ex.Message, ex);
                ExecutionSummary(startTime, ex.Message + comment + PMOrderFileProcessor.remark, Argument.Error);
            }
            _logger.Info("Completed Execution :" + MethodBase.GetCurrentMethod().Name);
        }

        #endregion Public Methods

        #region Private Methods

        /// <summary>
        /// Validates the XML file name to chech whether they end with Date_Time format.- This Method is no more needed for EDGIs Rearch Project. As input will come via layer 7 and stored in staging table by v1t8
        /// </summary>
        /// <param name="xmlFiles">Array of XML files to validate</param>
        /// <returns>True if all the file names are valid. False, otherwise.</returns>
        private static bool IsFileNameValid(string[] xmlFiles)
        {
            bool isvalid = true;
            if (xmlFiles == null || xmlFiles.Length < 1) return isvalid;
            //Prepare regular expression : yyyyMMdd_HHmmss
            string regExpressionDate = AppConfiguration.getStringSetting("file_regex", @"(20)\d\d(0[1-9]|1[012])(0[1-9]|[12][0-9]|3[01])_(0[0-9]|1[0-9]|2[0-3])([0-5][0-9])([0-5][0-9]).[xX][mM][lL]$");
            System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex(regExpressionDate);

            foreach (string file in xmlFiles)
            {
                if (regex.IsMatch(file) == false) { isvalid = false; break; }
            }
            return isvalid;
        }


        /// <summary>
        /// This methos is to use for log the interface success or fail in interface summary table for EDGIS Rearch Project-V1t8
        /// </summary>
        /// <param name="startTime">string </param>
        /// <param name="comment">string</param>
        /// <param name="status">comment</param>
        private static void ExecutionSummary(string startTime, string comment, string status)
        {
            try
            {
                _logger.Info("Starting" + MethodBase.GetCurrentMethod().Name);
                argument = new StringBuilder();
                remark = new StringBuilder();
                //Setting arguments
                argument.Append(Argument.Interface);
                argument.Append(Argument.Type);
                argument.Append(Argument.Integration);
                argument.Append(startTime + ";");
                remark.Append(comment);
                argument.Append(DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss") + ";");
                argument.Append(status);
                argument.Append(remark);

                //To execution for interface execution summary exe.This exe must need some input argument to run successffully. 
                ProcessStartInfo processStartInfo = new ProcessStartInfo(ConfigurationManager.AppSettings["IntExecutionSummaryExePath"], "\"" + Convert.ToString(argument) + "\"");
                processStartInfo.UseShellExecute = false;
                processStartInfo.RedirectStandardOutput = true;
                processStartInfo.CreateNoWindow = true;

                Process proc = new Process();
                proc.StartInfo = processStartInfo;
                proc.EnableRaisingEvents = true;
                proc.Start();
                proc.BeginOutputReadLine();
                while (!proc.HasExited)
                {
                    Thread.Sleep(1000);
                }
            }
            catch (Exception ex)
            {
                _logger.Error ("Exception occures while executing the Interface Summary exe" + ex.Message + MethodBase.GetCurrentMethod().Name);
                _logger.Error(ex.Message + "   " + ex.StackTrace);
                Console.WriteLine("Exception occures while executing the Interface Summary exe", ex.StackTrace);                
            }
            _logger.Info("Completed Execution :" + MethodBase.GetCurrentMethod().Name);
        }

        #endregion Private Methods
    }

    /// <summary>
    /// This class is for interface Summary Execution -Added for EDGIS rearch project-v1t8
    /// </summary>
    public class Argument
    {
        /// <summary>
        /// This property to get Sucess status of process
        /// </summary>
        public static string Done { get; } = "D;";
        /// <summary>
        /// This property to get failure status of process
        /// </summary>
        public static string Error { get; } = "F;";
        /// <summary>
        /// This property to get interface name 
        /// </summary>
        public static string Interface { get; } = "ED13;";
        /// <summary>
        /// This property to get name of the system to integration
        /// </summary>
        public static string Integration { get; } = "SAP;";
        /// <summary>
        /// This property to get type of interface
        /// </summary>
        public static string Type { get; } = "Inbound;";
    }
}
