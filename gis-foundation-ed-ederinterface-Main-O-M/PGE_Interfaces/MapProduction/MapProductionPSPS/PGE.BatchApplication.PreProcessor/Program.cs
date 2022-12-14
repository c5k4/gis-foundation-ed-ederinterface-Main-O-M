using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using PGE.Common.Delivery.Framework;
using Miner.Interop;
using PGE.Common.Delivery.Diagnostics;
using System.Reflection;
using ESRI.ArcGIS.esriSystem;
using System.Diagnostics;
//using PGE.BatchApplication.PreProcessing;
using PGE.BatchApplication.PSPSMapProduction.Export;
using PGE.BatchApplication.PreProcessing;

namespace PGE.BatchApplication.PSPSMapProduction.PreProcessor
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
        private static Log4NetLogger _logger = new Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "\\PGE.BatchApplication.PSPSMapProduction1.0.log4net.config", "PSPSMapProduction");

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
                ExportType exportType = ExportType.Bulk;
                string circuitIds = "";
                bool rebuildAllCircuitRecords = false;

                //Allows user to change the configuration settings
                if (args.Length > 0 && args[0] == "CONFIG")
                {
                    UIMapProductionConfig uiMapProdConfig = new UIMapProductionConfig();
                    uiMapProdConfig.ShowDialog();
                    return;
                }
                else
                {
                    for (int i = 0; i < args.Length; i++)
                    {
                        if (args[i] == "-c")
                        {
                            // build PSPS map rows for specific circuitids seperated by commas
                            circuitIds = args[i + 1];
                        }
                        else if (args[i] == "-a")
                        {
                            // rebuild all PSPS map rows
                            rebuildAllCircuitRecords = true;
                        }
                    }
                }
                //Initializes the ArcGIS & ArcFM license
                using (LicenseManager licenseManager = new LicenseManager())
                {
                    licenseManager.Initialize(esriLicenseProductCode.esriLicenseProductCodeAdvanced, mmLicensedProductCode.mmLPArcFM);

                    if (circuitIds != "")
                    {
                        _logger.Info("build the map lookup table and then export the maps-1");
                        BuildAndExport(circuitIds);
                    }
                    else if (rebuildAllCircuitRecords)
                    {
                        _logger.Info(" build the map lookup table and then export the maps-2");
                        BuildAndExport("");
                    }
                    else
                    {
                        //Get the export type from the argument
                        exportType = GetArgument<ExportType>(args, 0, "ExportType", ExportType.Bulk);

                        //licenseManager.Initialize(esriLicenseProductCode.esriLicenseProductCodeEngine, mmLicensedProductCode.mmLPEngine);
                        _logger.Info("Get the Number of export processes to be used");


                       totalNumberOfProcesses = Convert.ToInt32(MapProductionConfigurationHandler.Settings["numberexporters"]);
                        using (BaseMapPreProcessor preProcessor = new PGEPreProcessor())
                        {
                            //Rebuild the map rows if needed
                            if (args.Length > 1)
                            {
                                _logger.Info(" build the map lookup table and then export the maps-3");
                                BuildAndExport(args[1]);
                            }
                            _logger.Info("Perform the Pre-Processing task and catch hold of the errors");

                           List<string> errorList = preProcessor.Process(exportType, totalNumberOfProcesses, args);
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

                    }
                    //Shuts down the license
                    licenseManager.Shutdown();
                }
                
                if (circuitIds == "" && !rebuildAllCircuitRecords) 
                {
                    Console.WriteLine("Export Process Started - " + System.DateTime.Now);
                    _logger.Info("Export Process Started - " + System.DateTime.Now);

                    string exportApplicationDirectory = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
                    if (MapProductionConfigurationHandler.GetSettingValue("RunExport").ToUpper() != "FALSE")
                    {
                        //Start export operation for each Process
                        for (int i = 1; i <= totalNumberOfProcesses; i++)
                        {
                            //Provide Exporter.exe and start the process
                            ProcessStartInfo processInfo = new ProcessStartInfo(@"PGE.BatchApplication.PSPSMapProduction.Exporter.exe", i.ToString());
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
                        // run the merge PDF script to merge all PDF files based on circuit names
                        Console.WriteLine("Merge PDF files based on Circuit names Started.");
                        _logger.Info("Merge PDF files based on Circuit names Started.");
                        string mergePdfsPy = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "MergePdfs.py");
                        string pdfLocation = MapProductionConfigurationHandler.Settings["PDFLocation"];
                        PGEMergePDFs pdfMerger = new PGEMergePDFs();
                        pdfMerger.MergePDF(mergePdfsPy, pdfLocation);

                        Console.WriteLine("Export Process completed. Watch log file for details." + System.DateTime.Now);
                        _logger.Info("Export Process completed. - " + System.DateTime.Now);
                    }
                    else
                    {
                        Console.WriteLine("Export Process completed. Watch log file for details. - " + System.DateTime.Now);
                        _logger.Info("Export Process completed. - " + System.DateTime.Now);
                    }
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

        /// <summary>
        /// Build the map lookup table for a single circuit and then print the maps
        /// </summary>
        /// <param name="circuitIds">Circuit Ids separated by commas</param>
        static void BuildAndExport(string circuitIds)
        {
            // search the circuit and find all the map rows
            // open the map template

            PGEInputData inputDataManager = new PGEInputData(0);

            if (circuitIds != "")
            {
                int deletedRows = inputDataManager.DeletePSPSMapRows(circuitIds);
                _logger.Info(string.Format("Sucessfully delete {0} PSPS map rows.", deletedRows));
            }
            // extract all data
            List<PSPSSegmentRecord> pspsMapRows = inputDataManager.SearchPSPSMapRows(circuitIds);

            // write to the map table
            int numInsertedRows = inputDataManager.AddPSPSMapRows(pspsMapRows);

            _logger.Info(string.Format("Sucessfully build {0} PSPS map rows.", numInsertedRows));

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

        private void RebuildMapRows(string circuitIds)
        {
        }


        /// <summary>
        /// Usage information logged to the output window
        /// </summary>
        private static void ShowUsage()
        {
            const string help =
                @"
Usage:
    PSPSMapProduction [Export Type] [Number of processes]
        [Export type]
            0 for bulk (default)
            1 for difference
        [Number of processes]
            The number of threads to run the export in.
            Maximum of 1 per core

    PSPSMapProduction CONFIG
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
