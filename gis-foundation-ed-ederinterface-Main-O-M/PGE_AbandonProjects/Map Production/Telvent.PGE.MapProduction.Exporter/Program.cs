using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Telvent.Delivery.Diagnostics;
using System.Reflection;
using Telvent.Delivery.Framework;
using ESRI.ArcGIS.esriSystem;
using Miner.Interop;
using System.Diagnostics;
using Telvent.Delivery.Systems.Data;
using Telvent.Delivery.Systems.Data.Oracle;
using System.Data;
using Telvent.PGE.MapProduction.Processor;
using System.Threading;
using System.Management;

namespace Telvent.PGE.MapProduction.Exporter
{
    public class Program
    {
        //private static Log4NetLogger _logger = new Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private static Log4NetLogger _logger = new Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "\\MapProduction1.0.log4net.config", "MapProduction");
        private static Process childProcess = null;
        private static int _processTimeout = 0;
        /// <summary>
        /// Main program to execute the export operation
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            Environment.ExitCode = 0;
            try
            {
                int start = -1;
                int end = -1;
                for (int i = 0; i < args.Length; i++)
                {
                    if (args[i] == "-s")
                    {
                        start = Int32.Parse(args[i + 1]);
                    }
                    else if (args[i] == "-e")
                    {
                        end = Int32.Parse(args[i + 1]);
                    }
                }

                if (start != -1 && end != -1)
                {
                    List<Process> processes = SpawnProcesses(start, end);
                    bool continueRunning = true;
                    while (continueRunning)
                    {
                        bool allExited = true;

                        foreach (Process p in processes)
                        {
                            if (!p.HasExited)
                            {
                                allExited = false;
                            }
                            else
                            {
                                if (p.ExitCode != (int)ExitCodes.Success)
                                {
                                    Environment.ExitCode = (int)ExitCodes.ChildFailure;
                                }
                            }
                        }

                        if (allExited) { continueRunning = false; }

                        Thread.Sleep(5000);
                    }
                }
                else
                {

                    bool isChild = false;
                    if (args == null || args.Length < 1)
                    {
                        _logger.Error("Process ID argument is missing in the Exporter application.");
                        return;
                    }

                    if (args.Length > 1)
                    {
                        isChild = true;
                    }

                    //Get the process number
                    int processNumber = 0;
                    int.TryParse(args[0], out processNumber);
                    _logger.Debug("Process Number:" + processNumber);
                    if (processNumber == 0)
                    {
                        _logger.Error(string.Format("Process ID argument '{0}' is in valid.", args[0]));
                        return;
                    }

                    if (!isChild)
                    {
                        string exportApplicationDirectory = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);

                        try
                        {
                            _logger.Debug("Resetting failed maps to ready to export");

                            IDatabaseConnection _dbConnection = null;
                            _dbConnection = new OracleDatabaseConnection(MapProductionConfigurationHandler.DatabaseServer, MapProductionConfigurationHandler.DataSource, MapProductionConfigurationHandler.UserName, MapProductionConfigurationHandler.Password);

                            if (!_dbConnection.IsOpen)
                            {
                                _dbConnection.Open();
                            }

                            IMPMapLookUpTable MapLookUpTable = new PGEMapLookUpTable();
                            IMPDBDataManager _dbInteraction = new DBDataManager(_dbConnection, MapLookUpTable);

                            //Let's set any failed maps from previous runs to ready to export so they can be attempted again
                            DataTable mapLut2 = _dbInteraction.GetMapLookUpTable("where (" + MapLookUpTable.MapExportState + "=" + Convert.ToInt32(ExportState.Processing) + " or " + MapLookUpTable.MapExportState + "=" + Convert.ToInt32(ExportState.Failed) + ")" + " and " + MapLookUpTable.ProcessingService + "=" + processNumber);
                            if (mapLut2 != null && mapLut2.Rows != null && mapLut2.Rows.Count > 0)
                            {
                                for (int i = 0; i < mapLut2.Rows.Count; i++)
                                {
                                    mapLut2.Rows[i][MapLookUpTable.MapExportState] = ExportState.ReadyToExport;
                                    _dbInteraction.UpdateLookUpTable(mapLut2);
                                }
                            }
                            _logger.Debug("Failed maps successfully set to ready to export");

                            if (_dbConnection.IsOpen)
                            {
                                _dbConnection.Close();
                            }
                        }
                        catch (Exception e)
                        {
                            //Something failed, most likely connecting to the database.  Let's continue processing
                            Console.WriteLine("Failed setting failed maps to ready to export. Message: " + e.Message + " StackTrace: " + e.StackTrace);
                        }

                        //Spawn our child and wait for the process to exit
                        int lastCount = -1;
                        Stopwatch stopWatch = new Stopwatch();
                        stopWatch.Start();
                        childProcess = SpawnProcess(processNumber, true, false, ProcessWindowStyle.Hidden, true);
                        _logger.Debug("Started child process with process ID: " + childProcess.Id);
                        Console.WriteLine("Started child process with process ID: " + childProcess.Id);
                        bool mapsToProcess = true;
                        while (mapsToProcess)
                        {
                            try
                            {
                                mapsToProcess = false;

                                IDatabaseConnection _dbConnection = null;
                                _dbConnection = new OracleDatabaseConnection(MapProductionConfigurationHandler.DatabaseServer, MapProductionConfigurationHandler.DataSource, MapProductionConfigurationHandler.UserName, MapProductionConfigurationHandler.Password);

                                if (!_dbConnection.IsOpen)
                                {
                                    _dbConnection.Open();
                                }

                                IMPMapLookUpTable MapLookUpTable = new PGEMapLookUpTable();
                                IMPDBDataManager _dbInteraction = new DBDataManager(_dbConnection, MapLookUpTable);
                                DataTable mapLut = _dbInteraction.GetMapLookUpTable("where " + MapLookUpTable.MapExportState + "=" + Convert.ToInt32(ExportState.ReadyToExport) + " and " + MapLookUpTable.ProcessingService + "=" + processNumber);
                                if (mapLut != null && mapLut.Rows != null && mapLut.Rows.Count > 0)
                                {
                                    mapsToProcess = true;

                                    if (lastCount == -1) { lastCount = mapLut.Rows.Count; }
                                    else if (lastCount == mapLut.Rows.Count)
                                    {
                                        if (stopWatch.ElapsedMilliseconds > getProcessTimeout())
                                        {
                                            //Seems like the child process is idle and hasn't produced a map in 40 minutes.  Let's kill the process
                                            //and all processes it may have started (i.e. Python.exe)
                                            childProcess.Kill();
                                            stopWatch = new Stopwatch();
                                            stopWatch.Start();
                                        }
                                    }
                                    else if (lastCount > mapLut.Rows.Count)
                                    {
                                        //Maps have processed so let's restart our stopwatch
                                        lastCount = mapLut.Rows.Count;
                                        stopWatch = new Stopwatch();
                                        stopWatch.Start();
                                    }

                                    if (childProcess.HasExited)
                                    {
                                        //Still some maps to process.  Let's clean the table of any rows that need to be set to ready to export again
                                        //and Let's start our process

                                        DataTable mapLut2 = _dbInteraction.GetMapLookUpTable("where (" + MapLookUpTable.MapExportState + "=" + Convert.ToInt32(ExportState.Processing) + " or " + MapLookUpTable.MapExportState + "=" + Convert.ToInt32(ExportState.Processing) + ")" + " and " + MapLookUpTable.ProcessingService + "=" + processNumber);
                                        if (mapLut2 != null && mapLut2.Rows != null && mapLut2.Rows.Count > 0)
                                        {
                                            for (int i = 0; i < mapLut2.Rows.Count; i++)
                                            {
                                                mapLut2.Rows[i][MapLookUpTable.MapExportState] = ExportState.ReadyToExport;
                                                _dbInteraction.UpdateLookUpTable(mapLut2);
                                            }
                                        }
                                        childProcess = SpawnProcess(processNumber, true, false, ProcessWindowStyle.Hidden, true);
                                        _logger.Debug("Child process exited with maps still left to process.  Restarting with process ID: " + childProcess.Id);
                                        Console.WriteLine("Child process exited with maps still left to process.  Restarting with process ID: " + childProcess.Id);
                                    }
                                }

                                if (_dbConnection.IsOpen)
                                {
                                    _dbConnection.Close();
                                }
                            }
                            catch (Exception e)
                            {
                                //Something failed, most likely connecting to the database.  Let's continue processing
                                Console.WriteLine("Failed checking on progress. Message: " + e.Message + " StackTrace: " + e.StackTrace);
                                mapsToProcess = true;
                            }
                            Thread.Sleep(60000);
                        }
                    }
                    else
                    {

                        IList<string> errors = null;
                        using (LicenseManager licenseManager = new LicenseManager())
                        {
                            //Initialiase ArcGIS License
                            licenseManager.Initialize(ESRI.ArcGIS.esriSystem.esriLicenseProductCode.esriLicenseProductCodeAdvanced, mmLicensedProductCode.mmLPArcFM);
                            //licenseManager.Initialize(esriLicenseProductCode.esriLicenseProductCodeEngine, mmLicensedProductCode.mmLPEngine);
                            _logger.Debug("Starting the Exporter");
                            //Perform the export operatiion
                            IMPExporter exporter = new Export.PGEExporter();
                            errors = exporter.Export(processNumber);
                            //_logger.Debug(errors.ToString());  
                            //Shutdown the licese
                            licenseManager.Shutdown();
                        }


                        //Log the Success / Failure message
                        if (errors == null || errors.Count < 1)
                        {
                            _logger.Info(string.Format("Process ID : {0} sucessfully completed the export operation.", processNumber));
                        }
                        else
                        {
                            StringBuilder errorList = new StringBuilder();
                            _logger.Error(string.Format("Errors encountered while executing the Export operation for Process ID: {0}", processNumber));
                            _logger.Error("--------------------------------------------");
                            _logger.Error(string.Join(System.Environment.NewLine, errors.ToArray()));
                            _logger.Error("--------------------------------------------");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Environment.ExitCode = (int)ExitCodes.Failure;
                _logger.Error(ex.Message, ex); 
            }
        }
        /*
        /// <summary>
        /// Kill a process, and all of its children.
        /// </summary>
        /// <param name="pid">Process ID.</param>
        private static void KillProcessAndChildren(int pid)
        {
            ManagementObjectSearcher searcher = new ManagementObjectSearcher("Select * From Win32_Process Where ParentProcessID=" + pid);
            ManagementObjectCollection moc = searcher.Get();
            foreach (ManagementObject mo in moc)
            {
                KillProcessAndChildren(Convert.ToInt32(mo["ProcessID"]));
            }
            try
            {
                Process proc = Process.GetProcessById(pid);
                proc.Kill();
            }
            catch (ArgumentException)
            {
                // Process already exited.
            }
        }
        */

        private static int getProcessTimeout()
        {
            try
            {
                if (_processTimeout == 0)
                {
                    _processTimeout = int.TryParse(MapProductionConfigurationHandler.Settings["PROCESS_TIMEOUT"], out _processTimeout) ? _processTimeout : 2400000;
                }
            }
            catch (Exception e) { _processTimeout = 2400000; }
            return _processTimeout;
        }

        private static Process SpawnProcess(int processID, bool isChild, bool createWindow, ProcessWindowStyle windowStyle, bool redirectOutput)
        {
            string arguments = processID.ToString();

            if (isChild)
            {
                arguments += " -child";
            }

            ProcessStartInfo startInfo = new ProcessStartInfo(System.Reflection.Assembly.GetExecutingAssembly().Location, arguments)
            {
                CreateNoWindow = createWindow,
                WindowStyle = windowStyle,
                UseShellExecute = false,
                RedirectStandardOutput = redirectOutput
            };
            return Process.Start(startInfo);
        }

        private static List<Process> SpawnProcesses(int start, int end)
        {
            List<Process> processes = new List<Process>();
            for (int i = start; i <= end; i++)
            {
                processes.Add(SpawnProcess(i, false, true, ProcessWindowStyle.Normal, false));
            }
            return processes;
        }

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
