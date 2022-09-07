using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Telvent.Delivery.Framework;
using Miner.Interop;
using Telvent.Delivery.Diagnostics;
using System.Reflection;
using ESRI.ArcGIS.esriSystem;
using System.Diagnostics;
using Telvent.PGE.MapProduction.PreProcessing;

namespace Telvent.PGE.MapProduction.Processor
{
    /// <summary>
    /// Main Program that calls the Pre-Processor for Map Production
    /// </summary>
    public class Program
    {
        #region Private Fields

        /// <summary>
        /// Logs error/ custom information
        /// </summary>
        private static Log4NetLogger _logger = new Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType,"\\MapProduction1.0.log4net.config","MapProduction");

        #endregion Private Fields

        static int processesCompleted = 0;
        static int totalNumberOfProcesses = 0;

        #region Main Static Method

        /// <summary>
        /// Main Program gets executed when the application starts.
        /// </summary>
        /// <param name="args">Array of the input arguments</param>
        static void Main(string[] args)
        {
            Environment.ExitCode = (int)ExitCodes.Success;
            try
            {
                ShowUsage();
                ExportType exportType = ExportType.Difference;
                if (args.Length > 0)
                {
                    //Allows user to change the configuration settings
                    if (args[0] == "CONFIG")
                    {
                        UIMapProductionConfig uiMapProdConfig = new UIMapProductionConfig();
                        uiMapProdConfig.ShowDialog();
                        return;
                    }
                    else
                    {
                        //Get the export type from the argument
                        exportType = GetArgument<ExportType>(args, 0, "ExportType", ExportType.Difference);
                    }
                }

                //Initializes the ArcGIS & ArcFM license
                using (LicenseManager licenseManager = new LicenseManager())
                {
                    licenseManager.Initialize(esriLicenseProductCode.esriLicenseProductCodeAdvanced, mmLicensedProductCode.mmLPArcFM);
                    //licenseManager.Initialize(esriLicenseProductCode.esriLicenseProductCodeEngine, mmLicensedProductCode.mmLPEngine);
                    //Get the Number of export processes to be used
                    totalNumberOfProcesses = Convert.ToInt32(MapProductionConfigurationHandler.Settings["numberexporters"]);
                    using (BaseMapPreProcessor preProcessor = new PGEPreProcessor())
                    {
                        //Perform the Pre-Processing task and catch hold of the errors
                        //Perform the Pre-Processing task and catch hold of the errors
                        List<string> errorList = preProcessor.Process(exportType, totalNumberOfProcesses);
                        if (errorList.Count > 0)
                        {
                            if (errorList.Count == 1 && errorList[0] == preProcessor.SuccessMessage)
                            {
                                //Divide the entire process among the multiple processes
                                //int recordsProcessed = preProcessor.SetProcessInfo(totalNumberOfProcesses);
                                int recordsProcessed = preProcessor.RecordsProcessed;
                                //Set the actual number of export processes to be started
                                totalNumberOfProcesses = (recordsProcessed >= totalNumberOfProcesses) ? totalNumberOfProcesses : recordsProcessed;
                            }
                            else
                            {
                                foreach (string error in errorList)
                                {
                                    Console.WriteLine(error);
                                    _logger.Error(error);
                                    throw new Exception(string.Join("\n", errorList.ToArray()));
                                }
                            }
                        }
                    }

                    //Shuts down the license
                    licenseManager.Shutdown();

                }

                Console.WriteLine("Export Process Started.");
                _logger.Info("Export Process Started.");

                string exportApplicationDirectory = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
                if (MapProductionConfigurationHandler.GetSettingValue("RunExport").ToUpper() != "FALSE")
                {
                    //Start export operation for each Process
                    for (int i = 1; i <= totalNumberOfProcesses; i++)
                    {
                        //Provide Exporter.exe and start the process
                        ProcessStartInfo processInfo = new ProcessStartInfo(@"Telvent.PGE.MapProduction.Exporter.exe", i.ToString());
                        processInfo.CreateNoWindow = true;
                        processInfo.WorkingDirectory = exportApplicationDirectory;
                        processInfo.WindowStyle = ProcessWindowStyle.Normal;
                        Process exportProcess = new Process();
                        exportProcess.StartInfo = processInfo;
                        exportProcess.EnableRaisingEvents = true;
                        exportProcess.Exited += new EventHandler(exportProcess_Exited);
                        exportProcess.Start();
                    }

                    while (processesCompleted < totalNumberOfProcesses)
                    {
                        System.Threading.Thread.Sleep(15000);
                    }
                    Console.WriteLine("Export Process completed. Watch log file for details.");
                    _logger.Info("Export Process completed. Watch log file for details.");
                }
                else
                {
                    Console.WriteLine("Export PreProcess completed. Watch log file for details.");
                    _logger.Info("Export PreProcess completed. Watch log file for details.");
                    Console.WriteLine("Run the Exporter.Exe to generate PDFs and TIFFs.");
                }
            }
            catch (Exception ex)
            {
                Environment.ExitCode = (int)ExitCodes.Success;
                _logger.Error("Failed pre-processing" + Environment.NewLine + ex.Message, ex);
                Console.WriteLine("Failed pre-processing" + Environment.NewLine + ex.Message+Environment.NewLine+ex.StackTrace, ex);
            }
        }

        static void exportProcess_Exited(object sender, EventArgs e)
        {
            processesCompleted++;
            Console.WriteLine(string.Format("Export Process completed for Process ID : {0}", (sender as Process).StartInfo.Arguments));
        }

        #endregion Main Static Method

        #region Private Methods

        /// <summary>
        /// Returns the argument value at the specified postion after converting to required type 
        /// </summary>
        /// <typeparam name="T">Output parameter type</typeparam>
        /// <param name="args">Array of arguments</param>
        /// <param name="position">Position of the value in the <paramref name="args"/></param>
        /// <param name="name">Not currently in use</param>
        /// <param name="defaultValue">Default return value</param>
        /// <returns>Returns the argument value at the specified postion after converting to required type</returns>
        private static T GetArgument<T>(string[] args, int position, string name, T defaultValue)
        {
            T retVal = defaultValue;
            if (args.Length <= position)
            {
                return retVal;
            }
            else
            {
                if (args[position] is T)
                {
                    retVal = (T)Convert.ChangeType(args[position], typeof(T));
                    return retVal;
                }
                else if (typeof(T).IsEnum)
                {
                    retVal = (T)Enum.Parse(typeof(T), args[position]);
                }
            }
            return retVal;
        }

        /// <summary>
        /// Usage information logged to the output window
        /// </summary>
        private static void ShowUsage()
        {
            const string help =
                @"
Usage:
    MapProduction [Export Type] [Number of processes]
        [Export type]
            0 for bulk
            1 for difference (default)
        [Number of processes]
            The number of threads to run the export in.
            Maximum of 1 per core

    MapProduction CONFIG
        Launches the configuration dialog.            
";
            Console.WriteLine(help);
        }

        #endregion Private Methods

        public enum ExitCodes
        {
            Success,
            Failure,
            ChildFailure,
            InvalidArguments,
            LicenseFailure
        };
    }
}
