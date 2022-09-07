using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Diagnostics;
using System.ComponentModel;
using System.Configuration;
using System.Runtime.InteropServices;
using System.Data;
using Oracle.DataAccess.Client;

using PGE.Interfaces.TaxReportDataStaging;
using ESRI.ArcGIS.Geodatabase;
using System.Text.RegularExpressions;
using ESRI.ArcGIS.esriSystem;

namespace PGE.Interfaces.TaxReportData
{
    class Program
    {
        private static int _processCount;
        private static List<Process> _childProcessList;
        private static int _counter = 0;
        private static esriLicenseProductCode[] _prodCodes = { esriLicenseProductCode.esriLicenseProductCodeAdvanced };
        private static LicenseInitializer _licenceManager = new LicenseInitializer();

        private static bool _canSpin = true;
        private static bool _isOk = true;
        private static bool hadErrors = false;

        [MTAThread]
        static void Main(string[] args)
        {
            if (args.Count() > 0 && args[0] == "/?")
            {
                Console.WriteLine("-s: Execute spatial analysis for support structures");
                Console.WriteLine("-g: Execute spatial analysis for map grids");
                Environment.Exit(0);
            }
            if (args.Count() == 0) 
            {
                Console.WriteLine("Not enough arguments specified");
                Console.WriteLine("-s: Execute spatial analysis for support structures");
                Console.WriteLine("-g: Execute spatial analysis for map grids");
                Environment.Exit(1); 
            }

            Console.CancelKeyPress += new ConsoleCancelEventHandler(Console_CancelKeyPress);

            Console.WriteLine("Data processing start time: " + DateTime.Now.ToLocalTime());
            Common.WriteToLog("Data processing start time: " + DateTime.Now.ToLocalTime(), LoggingLevel.Info);

            _childProcessList = new List<Process>();
            if (CheckLicense(_licenceManager))
            {
                try
                {
                    string arg1 = "";
                    string arg2 = "";

                    if (args.Count() > 0)
                    {
                        arg1 = args[0];
                    }
                    if (args.Count() > 1)
                    {
                        arg2 = args[1];
                    }
                    

                    bool executeED08Structure = false;
                    bool executeED08MapGrids = false;

                    if (arg1 == "-s" || arg2 == "-s") { executeED08Structure = true; }
                    if (arg1 == "-g" || arg2 == "-g") { executeED08MapGrids = true; }

                    if (!executeED08MapGrids && !executeED08Structure)
                    {
                        Console.WriteLine("Invalid arguments specified");
                        Console.WriteLine("-s: Execute spatial analysis for support structures");
                        Console.WriteLine("-g: Execute spatial analysis for map grids");
                        Common.WriteToLog("Invalid arguments specified" + DateTime.Now.ToLocalTime(), LoggingLevel.Info);
                        Common.WriteToLog("-s: Execute spatial analysis for support structures" + DateTime.Now.ToLocalTime(), LoggingLevel.Info);
                        Common.WriteToLog("-g: Execute spatial analysis for map grids" + DateTime.Now.ToLocalTime(), LoggingLevel.Info);
                        Environment.Exit(1); 
                    }

                    if (executeED08Structure)
                    {
                        Console.WriteLine("Execute StoredProcedure");
                        //Clear taxreport data table
                        ExceuteStoredProcedure(ConfigurationManager.AppSettings["TruncateED08TempTableProc"]);
                    }
                    if (executeED08MapGrids)
                    {
                        //Clear CircuitToMapNum data table
                        ExceuteStoredProcedure(ConfigurationManager.AppSettings["TruncateCircuitToMapTableProc"]);
                    }

                    int numProcessesToUse = Int32.Parse(ConfigurationManager.AppSettings["NumberOfProcesses"]);

                    IWorkspace WorkSpace = Common.SetWorkspace();
                    IFeatureWorkspace _fWorkspace = WorkSpace as IFeatureWorkspace;

                    List<string> circuitIDList = null;
                    Dictionary<string, IGeometricNetwork> geomNetworks = null;
                    if (geomNetworks == null)
                    {
                        geomNetworks = PGE.Interfaces.TaxReportDataStaging.TaxReportDataStagingClass.GetNetworks(WorkSpace);
                    }
                    string desiredNetwork = ConfigurationManager.AppSettings["GeometricNetwork"].ToString();
                    foreach (KeyValuePair<string, IGeometricNetwork> kvp in geomNetworks)
                    {
                        if (kvp.Key.ToUpper() != null && kvp.Key.ToUpper() == desiredNetwork.ToUpper())
                        {
                            circuitIDList = PGE.Interfaces.TaxReportDataStaging.TaxReportDataStagingClass.GetFeederIDs(kvp.Value);
                            break;
                        }

                    }

                    double lastProgress = 0.0;
                    int circuitsPerProcess = -1;
                    try { circuitsPerProcess = Int32.Parse(ConfigurationManager.AppSettings["CircuitsPerProcess"]); }
                    catch { circuitsPerProcess = 4; }

                    Dictionary<Process, string> processesRunning = new Dictionary<Process, string>();
                    List<string> circuitIDsToProcess = circuitIDList.ToList();
                    while (circuitIDsToProcess.Count > 0)
                    {
                        string circuits = "";
                        for (int i = 0; i < circuitsPerProcess && i < circuitIDsToProcess.Count; i++)
                        {
                            if (i != 0) { circuits += ","; }
                            circuits += circuitIDsToProcess[i];
                        }
                        string[] circuitsToRemove = Regex.Split(circuits, ",");
                        foreach (string circuitToRemove in circuitsToRemove) { circuitIDsToProcess.Remove(circuitToRemove); }

                        string arguments = circuits;
                        if (executeED08Structure) { arguments += " -s"; }
                        if (executeED08MapGrids) { arguments += " -g"; }

                        ProcessStartInfo processStartInfo = new ProcessStartInfo("PGE.Interfaces.ProcessTaxData.exe", arguments);
                        processStartInfo.UseShellExecute = false;
                        processStartInfo.RedirectStandardOutput = true;
                        processStartInfo.CreateNoWindow = true;

                        Process proc = new Process();
                        processesRunning.Add(proc, circuits);
                        proc.StartInfo = processStartInfo;
                        proc.EnableRaisingEvents = true;
                        proc.OutputDataReceived += new DataReceivedEventHandler(proc_OutputDataReceived);
                        proc.Exited += new EventHandler(proc_Exited);
                        proc.Start();
                        proc.BeginOutputReadLine();

                        _childProcessList.Add(proc);

                        while (processesRunning.Count > numProcessesToUse - 1)
                        {
                            Thread.Sleep(1000);

                            foreach (KeyValuePair<Process, string> process in processesRunning)
                            {
                                if (process.Key.HasExited)
                                {
                                    processesRunning.Remove(process.Key);
                                    break;
                                }
                            }

                            double currentProgress = Math.Round(100 - ((((double)(circuitIDsToProcess.Count + (processesRunning.Count * circuitsPerProcess))) / ((double)circuitIDList.Count)) * 100.0), 1);
                            if (currentProgress != lastProgress)
                            {
                                lastProgress = currentProgress;
                                Console.WriteLine("Total Progress: " + currentProgress + "%");
                                Common.WriteToLog("Total Progress: " + currentProgress + "%" + DateTime.Now.ToLocalTime(), LoggingLevel.Info);
                            }
                        }
                    }

                    while (processesRunning.Count > 0)
                    {
                        Thread.Sleep(1000);

                        foreach (KeyValuePair<Process, string> process in processesRunning)
                        {
                            if (process.Key.HasExited)
                            {
                                processesRunning.Remove(process.Key);
                                break;
                            }
                        }

                        double currentProgress = Math.Round(100 - ((((double)(circuitIDsToProcess.Count + (processesRunning.Count * circuitsPerProcess))) / ((double)circuitIDList.Count)) * 100.0), 1);
                        if (currentProgress != lastProgress)
                        {
                            lastProgress = currentProgress;
                            Console.WriteLine("Total Progress: " + currentProgress + "%");
                            Common.WriteToLog("Total Progress: " + currentProgress + "%" + DateTime.Now.ToLocalTime(), LoggingLevel.Info);
                        }
                    }

                    /*
                    Console.WriteLine("To end the program, press 'CTRL+C' key...");
                    Console.WriteLine("");
                    Console.WriteLine("");

                    //Console.WriteLine("PGE.Interfaces.TaxReportData Processor Affinity: {0}", Process.GetCurrentProcess().ProcessorAffinity + " ...");

                    while (_canSpin)
                    {
                        Spin();
                    }
                    */
                    //Console.ReadLine();
                    Console.WriteLine("Total Progress: 100%");
                    Common.WriteToLog("Total Progress: 100%" + DateTime.Now.ToLocalTime(), LoggingLevel.Info);
                    if (executeED08MapGrids)
                    {
                        Common.WriteToLog("Executing Grid Completed Stored Procedure" +ConfigurationManager.AppSettings["ED08GridRunCompletedProc"] + DateTime.Now.ToLocalTime(), LoggingLevel.Info);
                        ExceuteStoredProcedure(ConfigurationManager.AppSettings["ED08GridRunCompletedProc"]);
                    }
                    if (executeED08Structure)
                    {
                        Common.WriteToLog("Executing Circuits Completed Stored Procedure" + ConfigurationManager.AppSettings["ED08RunCompletedProcedure"] + DateTime.Now.ToLocalTime(), LoggingLevel.Info);
                        ExceuteStoredProcedure(ConfigurationManager.AppSettings["ED08RunCompletedProcedure"]);
                    }

                    Console.WriteLine("Done!");
                    Common.WriteToLog("Done!" + DateTime.Now.ToLocalTime(), LoggingLevel.Info);
                    Environment.Exit(0);
                }
                catch (Exception ex)
                {
                    Common.WriteToLog("Error: [" + DateTime.Now.ToLocalTime() + "] PGE.Interfaces.TaxReportData -- Main() -- " + ex.Message, LoggingLevel.Error);
                    Console.WriteLine("Error Encountered check log file for problems.");
                    Environment.Exit(1);
                }
            }
            else
            {
                Common.WriteToLog("Error: [" + DateTime.Now.ToLocalTime() + "] PGE.Interfaces.TaxReportData -- Main() -- Unable to check out license.", LoggingLevel.Error);
                Console.WriteLine("Error Encountered check log file for problems.");
                Environment.Exit(1);
            }
        }

        /// <summary>
        /// Initializes ArcGIS license
        /// </summary>
        /// <param name="licenceManager"></param>
        /// <returns></returns>
        private static bool CheckLicense(LicenseInitializer licenceManager)
        {
            bool isOk = false;

            try
            {
                isOk = licenceManager.InitializeApplication(_prodCodes);
                isOk = licenceManager.GetArcFMLicense(Miner.Interop.mmLicensedProductCode.mmLPArcFM);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Failed to initialize license : " + ex.Message);
                Common.WriteToLog("Failed to initialize license : " + ex.Message, LoggingLevel.Error);
            }

            return isOk;
        }

        /// <summary>
        /// Event handler that prints process messages in the console 
        /// </summary>
        /// <param name="sender">Sender object</param>
        /// <param name="e">DataReceivedEventArgs</param>
        static void proc_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (e.Data != null)
            {
                Console.WriteLine(e.Data);
                if (e.Data.ToLower().Contains("error:"))
                    _isOk = false;
            }
        }

        /// <summary>
        /// Event handler that fires when each process exits
        /// </summary>
        /// <param name="sender">Sender object</param>
        /// <param name="e">EventArgs</param>
        static void proc_Exited(object sender, EventArgs e)
        {
            try
            {
                _childProcessList.Remove(sender as Process);

                if ((sender as Process).ExitCode != 0)
                {
                    _isOk = false;
                    Console.WriteLine("Tax report data processing aborted due to error " + DateTime.Now.ToLocalTime());
                    Common.WriteToLog("Tax report data processing aborted due to error " + DateTime.Now.ToLocalTime(), LoggingLevel.Error);

                    // Cleanup
                    foreach (Process proc in _childProcessList)
                    {
                        try
                        {
                            //I believe killing the child processes when they are trying to enter data into the table
                            //may be causing index issues which result in no longer being able to create new rows
                            //in the table afterwards.  So we will no longer kill the child processes but rather let
                            //them finish their processes.
                            //proc.Kill();
                        }
                        catch (Exception ex)
                        {
                            Common.WriteToLog("Process kill exception " + ex.Message, LoggingLevel.Error);
                        }
                    }

                    using (var oraConn = Common.GetDBConnection())
                    {
                        OracleCommand oraComm = new OracleCommand();
                        oraComm.Connection = oraConn;
                        oraComm.CommandText = "update edgisbo.TAXREPORTSTAGINGSTATUS set STATUS = '" + Common.DataMigrationStatus.Failure + "'";
                        try
                        {
                            oraComm.ExecuteNonQuery();
                        }
                        catch { }
                    }

                    Environment.Exit(1);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Tax report data processing aborted due to error " + DateTime.Now.ToLocalTime());
                Common.WriteToLog("Tax report data processing aborted due to error " + DateTime.Now.ToLocalTime(), LoggingLevel.Error);
                Environment.Exit(1);
            }
        }

        

        /// <summary>
        /// Event handler which responds on CTRL+C key press
        /// </summary>
        /// <param name="sender">Sender object</param>
        /// <param name="e">ConsoleCancelEventArgs</param>
        static void Console_CancelKeyPress(object sender, ConsoleCancelEventArgs e)
        {
            _canSpin = false;
            _isOk = false;
            Console.WriteLine();
            Console.WriteLine("Shutting down...");
            Common.WriteToLog("Shutting down due to cancelation... " + DateTime.Now.ToLocalTime(), LoggingLevel.Error);
            
            // Cleanup here
            foreach (Process proc in _childProcessList)
            {
                try
                {
                    //I believe killing the child processes when they are trying to enter data into the table
                    //may be causing index issues which result in no longer being able to create new rows
                    //in the table afterwards.  So we will no longer kill the child processes but rather let
                    //them finish their processes.
                    //proc.Kill();
                }
                catch (Exception ex)
                {
                    Common.WriteToLog("Process kill exception " + ex.Message, LoggingLevel.Error);
                }
            }
        }
        
        /// <summary>
        /// Takes backup of existing data
        /// </summary>
        /// <returns>boolean that indicates success or failure</returns>
        static bool ExceuteStoredProcedure(string StoredProcedureName)
        {
            bool ok = false;

            using (var oraConn = Common.GetDBConnection())
            {
                string commandText = string.Empty;
                OracleCommand oraCmd = null;

                try
                {
                    //First truncate
                    Console.WriteLine("Executing stored procedure: " + StoredProcedureName);
                    Common.WriteToLog("Executing stored procedure: " + StoredProcedureName, LoggingLevel.Info);

                    commandText = StoredProcedureName;
                    oraCmd = new OracleCommand();
                    oraCmd.Connection = oraConn;
                    oraCmd.CommandType = CommandType.StoredProcedure;
                    oraCmd.CommandText = commandText;
                    oraCmd.ExecuteNonQuery();
                    ok = true;

                }
                catch (Exception ex)
                {
                    Common.WriteToLog("Error: [" + DateTime.Now.ToLocalTime() + "] Executing stored procedure: " + StoredProcedureName + ex.Message, LoggingLevel.Error);
                    Console.WriteLine("Error encountered executing procedure, check log");
                    throw ex;
                }
            }

            return ok;
        }

        /// <summary>
        /// Used for animated spinning cursor
        /// </summary>
        public static void Spin()
        {
            _counter++;
            Thread.Sleep(5);
            switch (_counter % 4)
            {
                case 0: Console.Write("/"); break;
                case 1: Console.Write("-"); break;
                case 2: Console.Write("\\"); break;
                case 3: Console.Write("|"); break;
            }

            try
            {
                Console.SetCursorPosition(Console.CursorLeft - 1, Console.CursorTop);
            }
            catch (Exception ex)
            { }
        }

    }


}
