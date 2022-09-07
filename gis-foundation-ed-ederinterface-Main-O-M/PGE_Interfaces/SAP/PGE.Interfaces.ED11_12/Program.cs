using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PGE.Common.Delivery.Diagnostics;
using System.Reflection;
using PGE.Common.Delivery.Framework;
using ESRI.ArcGIS.esriSystem;
using Miner.Interop;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.DataSourcesGDB;
using System.IO;
using System.Diagnostics;
using System.Threading;

namespace PGE.Interfaces.ED11_12
{
   public class Program
    {
        /// <summary>
        /// Logs error/ custom information
        /// </summary>
        private static Log4NetLogger _log = new Log4NetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, "PGE.Interfaces.ED11_12.log4net.config");
        private static bool isParent = true;

        // m4jf edgisrearch 415 - variables for interface execution summary interface
        public static string comment = default;
        private static string startTime;
        private static StringBuilder remark = default;
        private static StringBuilder Arguments = default;

        [STAThread()]
        static void Main(string[] args)
        {
            Environment.ExitCode = (int)ExitCodes.Success;
            startTime = DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss");
            try
            {
                bool prepProcessingTable = true;
                bool ForceUpdateTransactions = false;
                bool SubmitToGDBMPost = false;
                string configurationFile = "";
                string pwd = "";
                string parentVersionName = "";
                string versionName = "";
                int numberOfChildren = 1;
                bool allowUnknownErrors = false;
                int maxRunningProcesses = 0;
                int PID = -1;
                bool NoRollup = false;
                bool IgnoreExistingSessionNotPosted = false;

                if (args.Length > 0 && args[0] == "/?")
                {
                    Console.WriteLine("-c: Specify the location of the configuration file to use");
                    Console.WriteLine("-pwd: Specify a new password for encryption in the configuration file");
                    Console.WriteLine("-ForceUpdateTransactions: Forces update transactions to use specified new value regardless of whether current GIS values match the specified old value");
                    Console.WriteLine("-n: Specify the number of child processes to execute. Default is 1");
                    Console.WriteLine("-N: Specify that maximum concurrent running processes. If n is specified as 24, but N is specified at 8 then a total of 24 child processes will execute, but only 8 will be running at any given time");
                    Console.WriteLine("-UnknownErrors: Allows transactions with unknown errors to be processed and sent in ED12 file");
                    Console.WriteLine("-IgnoreExistingSessionNotPosted: Continues the processing of any new ED11 input files even if there is a previous session not posted");
                    Console.WriteLine("-ForceUpdateTransactions: Update transactions will be applied regardless of whether the old value matches the current GIS value or not");
                    Console.WriteLine("Valid Argument combinations");
                    Console.WriteLine("-c,-pwd");
                    Console.WriteLine("-c, -n -p -ForceUpdateTransactions, -SubmitToGDBMPost");
                    Console.WriteLine("-c, -n -p -SubmitToGDBMPost");
                    Console.WriteLine("-c, -n -NoRollup");
                    Console.WriteLine("-c, -n -p -ForceUpdateTransactions");
                    Console.WriteLine("-c");

                    Environment.Exit((int)ExitCodes.Success);
                }
                else
                {
#if DEBUG
                    //Debugger.Launch();
#endif
                    //Process arguments
                    for (int i = 0; i < args.Length; i++)
                    {
                        if (args[i] == "-c")
                        {
                            configurationFile = args[i + 1];
                        }
                        else if (args[i] == "-ForceUpdateTransactions")
                        {
                            ForceUpdateTransactions = true;
                        }
                        else if (args[i] == "-SubmitToGDBMPost")
                        {
                            SubmitToGDBMPost = true;
                        }
                        else if (args[i] == "-pwd")
                        {
                            pwd = args[i + 1];
                        }
                        else if (args[i] == "-v")
                        {
                            versionName = args[i + 1];
                            isParent = false;
                        }
                        else if (args[i] == "-n")
                        {
                            numberOfChildren = Int32.Parse(args[i + 1]);
                        }
                        else if (args[i] == "-N")
                        {
                            maxRunningProcesses = Int32.Parse(args[i + 1]);
                        }
                        else if (args[i] == "-pid")
                        {
                            PID = Int32.Parse(args[i + 1]);
                        }
                        else if (args[i] == "-p")
                        {
                            parentVersionName = args[i + 1];
                        }
                        else if (args[i] == "-NoPrep")
                        {
                            prepProcessingTable = false;
                        }
                        else if (args[i] == "-UnknownErrors")
                        {
                            allowUnknownErrors = true;
                        }
                        else if (args[i] == "-NoRollup")
                        {
                            NoRollup = true;
                        }
                        else if (args[i] == "-IgnoreExistingSessionNotPosted")
                        {
                            IgnoreExistingSessionNotPosted = true;
                        }
                    }

                    if (maxRunningProcesses == 0) { maxRunningProcesses = numberOfChildren; }

                    if (string.IsNullOrEmpty(configurationFile) || !File.Exists(configurationFile)) { throw new Exception("Invalid Configuration file specified: " + configurationFile); }
                    
                    //Password reset 
                    if (pwd != string.Empty)
                    {
                        Common.ResetPassword(pwd, configurationFile);
                        Environment.Exit((int)ExitCodes.Success);
                    }

                    using (LicenseManager licenseManager = new LicenseManager())
                    {
                        //Check out licenses
                        licenseManager.Initialize(esriLicenseProductCode.esriLicenseProductCodeAdvanced, mmLicensedProductCode.mmLPArcFM);

//Open our workspace
                        Common.ReadAppSettings(configurationFile);

                        if (isParent)
                        {

                            //Below code commented for EDGIS Rearch Project-v1t8
                            //   string outputLoggingFolder = Common.ResultsOutputLocation;
                            string outputLoggingFolder = string.Empty;
                           //string PTTInputDirectory = Common.PTTXmlInputDirectory;
                            //string PTTArchiveDirectory = Common.PTTED11ArchiveDirectory;
                            //Below code commented for EDGIS Rearch Project-v1t8
                            //     string PTTED12ArchiveDirectory = Common.PTTED12ArchiveDirectory;


                           


                            _log.Info("Workspace specified: " + Common.SDEConnectionFile);
                            _log.Info("Landbase Workspace specified: " + Common.LandbaseConnectionFile);
                           // _log.Info("Session manager connection specified: " + Common.SessionManagerConnectionStringNoPassword);

                            //Below code commented for EDGIS Rearch Project-v1t8
                          //  _log.Info("Output results folder specified: " + outputLoggingFolder);
                            // m4jf edgisrearch 415 ed 11 improvements 
                            // GIS will now pull data from SAP through EI
                           //  _log.Info("PT&T input directory specified: " + PTTInputDirectory);
                           // _log.Info("PT&T archive directory specified: " + PTTArchiveDirectory + "\r\n\r\n");

                            if (allowUnknownErrors) { _log.Info("Unknown transaction errors will continue processing and be reported in ED12 files"); }
                            if (NoRollup) { _log.Info("NoRollup specified so child versions will not be posted to parent"); }
                            if (allowUnknownErrors) { Console.WriteLine("Unknown transaction errors will continue processing and be reported in ED12 files"); }
                            if (NoRollup) { Console.WriteLine("NoRollup specified so child versions will not be posted to parent"); }

                            Console.WriteLine("Workspace specified: " + Common.SDEConnectionFile);
                            Console.WriteLine("Landbase Workspace specified: " + Common.LandbaseConnectionFile);
                            //Console.WriteLine("Session manager connection specified: " + Common.SessionManagerConnectionStringNoPassword);




                            #region Below code commented for EDGIS Rearch Project-v1t8
                            //if (string.IsNullOrEmpty(PTTED12ArchiveDirectory) || !Directory.Exists(PTTED12ArchiveDirectory))


                            // m4jf edgisrearch 415 ed 11 improvements 
                            // GIS will now pull data from SAP through EI
                            //Console.WriteLine("PT&T ED11 input directory specified: " + PTTInputDirectory);
                            //Console.WriteLine("PT&T ED11 archive directory specified: " + PTTArchiveDirectory);
                            //Console.WriteLine("PT&T ED12 output directory specified: " + outputLoggingFolder);
                            //Console.WriteLine("PT&T ED12 archive directory specified: " + PTTED12ArchiveDirectory + "\r\n\r\n");
                            #endregion
                            if (allowUnknownErrors) { Console.WriteLine("Unknown transaction errors will continue processing and be reported in ED12 files"); }
                            if (NoRollup) { Console.WriteLine("NoRollup specified so child versions will not be posted to parent"); }

                            #region Commented code for edgisrearch 415
                            // m4jf edgisrearch 415 ED11 improvements 
                            // GIS will now pull data from SAP through EI , files will not be used for data transfer between GIS & SAP

                            //if (string.IsNullOrEmpty(PTTInputDirectory) || !Directory.Exists(PTTInputDirectory)) { throw new Exception("Invalid PTT input directory specified: " + PTTInputDirectory); }
                            //if (string.IsNullOrEmpty(PTTArchiveDirectory) || !Directory.Exists(PTTArchiveDirectory))

                            //{
                            //    try
                            //    {
                            //        //Attempt to create the directory


                            //        if (!string.IsNullOrEmpty(PTTArchiveDirectory))
                            //        {
                            //            Directory.CreateDirectory(PTTArchiveDirectory);
                            //        }
                            //    }
                            //    catch (Exception ex)
                            //    {
                            //        throw new Exception("Invalid ED11 archive directory specified: " + PTTArchiveDirectory + ": " + ex.Message);
                            //    }
                            //}
                            //                            if (string.IsNullOrEmpty(PTTED12ArchiveDirectory) || !Directory.Exists(PTTED12ArchiveDirectory))
                            //                            {
                            //                                try
                            //                                {
                            //                                    //Attempt to create the directory
                            //                                    if (!string.IsNullOrEmpty(PTTED12ArchiveDirectory))
                            //                                    {
                            //                                        Directory.CreateDirectory(PTTED12ArchiveDirectory);
                            //                                    }
                            //                                }
                            //                                catch (Exception ex) 
                            //                                {
                            //                                    throw new Exception("Invalid ED12 archive directory specified: " + PTTED12ArchiveDirectory + ": " + ex.Message);
                            //                                }


                            //                            }
                            //                            if (string.IsNullOrEmpty(outputLoggingFolder) || !Directory.Exists(outputLoggingFolder)) { throw new Exception("Invalid transaction error log folder specified: " + outputLoggingFolder); }
                            //#if DEBUG
                            //                            if (File.Exists(Common.PTTTransactionStatusFile)) { File.Delete(Common.PTTTransactionStatusFile); }
                            //#endif
                            //                            if (File.Exists(Common.PTTTransactionStatusFile)) { throw new Exception("An existing transaction status file already exists and must be processed first : " + Common.PTTTransactionStatusFile); }

                            #endregion Commented code for edgisrearch 415

                            _log.Info("Opening workspace: " + Common.SDEConnectionFile);
                            Console.WriteLine("Opening workspace: " + Common.SDEConnectionFile);
                            IWorkspaceFactory workspaceFactory = new SdeWorkspaceFactoryClass();
                            IWorkspace workspace = workspaceFactory.OpenFromFile(Common.SDEConnectionFile, 0);
                            IWorkspace landbaseWorkspace = workspaceFactory.OpenFromFile(Common.LandbaseConnectionFile, 0);
                            
                            //Execute our PTT processor
                            PTTProcessor pttProcessor = new PTTProcessor(workspace, landbaseWorkspace, outputLoggingFolder, ForceUpdateTransactions,
                                SubmitToGDBMPost, "", "", numberOfChildren, maxRunningProcesses, PID, configurationFile, prepProcessingTable, allowUnknownErrors, NoRollup);
                            pttProcessor.ProcessPTTData(IgnoreExistingSessionNotPosted);
                            ExecutionSummary(startTime, comment, Argument.Done);
                        }
                        else // IsChild
                        {
                            //This is a child process
                            _log.Info("Opening workspace: " + Common.SDEConnectionFile);
                            Console.WriteLine("Opening workspace: " + Common.SDEConnectionFile);
                            IWorkspaceFactory workspaceFactory = new SdeWorkspaceFactoryClass();
                            IWorkspace workspace = workspaceFactory.OpenFromFile(Common.SDEConnectionFile, 0);
                            IWorkspace landbaseWorkspace = workspaceFactory.OpenFromFile(Common.LandbaseConnectionFile, 0);

                            //Execute our PTT processor
                            PTTProcessor pttProcessor = new PTTProcessor(workspace, landbaseWorkspace, ForceUpdateTransactions,
                                versionName, parentVersionName, PID, configurationFile, allowUnknownErrors);
                            pttProcessor.ProcessPTTDataInVersion();
                        }

                        //Shuts down the license
                        licenseManager.Shutdown();
                    }
                }
            }
            catch (Exception ex)
            {
                //if (ex.Message.ToUpper().Contains("No xml input files meeting the format".ToUpper()))
                //{
                //    _log.Info("No transactions were processed: " + ex.Message);
                //    Environment.ExitCode = (int)ExitCodes.Success;
                //}
                //else
                //{
                    _log.Error("Failed to process due to an unexpected failure: " + ex.Message + " Stacktrace: " + ex.StackTrace);
                    Console.WriteLine("Failed to process due to an unexpected failure: " + ex.Message + " Stacktrace: " + ex.StackTrace);

                // m4jf edgisrearch 415 added code for Interface execution summary
                    comment = "Failed to process due to an unexpected failure: " + ex.Message + " Stacktrace: " + ex.StackTrace;
                   
                    Environment.ExitCode = (int)ExitCodes.Failure;
               // }
            }

            if (Environment.ExitCode == '1')
            {
                ExecutionSummary(startTime, comment, Argument.Error);
            }

#if DEBUG
            if (isParent)
            {
               
                Console.WriteLine("Press any key to exit");
                Console.ReadLine();
            }
#endif
            
            //Force an exit just in case something is holding on to the process
            Environment.Exit(Environment.ExitCode);
        }

        public enum ExitCodes
        {
            Success,
            Failure
        };



        /// <summary>
        /// This methos is to use for log the interface success or fail in interface summary table for EDGIS Rearch Project-V1t8
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="comment"></param>
        /// <param name="status"></param>
        private static void ExecutionSummary(string startTime, string comment, string status)
        {
            try
            {

                Arguments = new StringBuilder();
                remark = new StringBuilder();
                //Setting arguments
                Arguments.Append(Argument.Interface);
                Arguments.Append(Argument.Type);
                Arguments.Append(Argument.Integration);
                Arguments.Append(startTime + ";");
                remark.Append(comment);
                Arguments.Append(DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss") + ";");
                Arguments.Append(status);
                Arguments.Append(remark);

                //To execution for interface execution summary exe.This exe must need some input argument to run successffully. 
                ProcessStartInfo processStartInfo = new ProcessStartInfo(Common.intExecutionSummary, "\"" + Convert.ToString(Arguments) + "\"");
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
                _log.Error("Exception occures while executing the Interface Summary exe" + ex.Message);
                _log.Info(ex.Message + "   " + ex.StackTrace);
                Console.WriteLine("Exception occures while executing the Interface Summary exe", ex.StackTrace);
                //  fileWriter.WriteLine(" Error Occurred while executing the Interface Summary exe at " + DateTime.Now + " " + ex);
            }
        }
       
    }
}
