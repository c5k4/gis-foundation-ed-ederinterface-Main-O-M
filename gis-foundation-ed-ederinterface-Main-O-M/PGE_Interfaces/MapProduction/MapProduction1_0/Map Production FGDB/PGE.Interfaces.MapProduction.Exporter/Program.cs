using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using Oracle.DataAccess.Client;
using System.Linq;
using System.Text;
//using PGE.Common.Delivery.Diagnostics;
using System.Reflection;
//using PGE.Common.Delivery.Framework;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Carto;
//using Miner.Interop;
using System.Diagnostics;
//using PGE.Common.Delivery.Systems.Data;
//using PGE.Common.Delivery.Systems.Data.Oracle;
using System.Data;
//using PGE.Interfaces.MapProduction.Processor;
using System.Threading;
using System.Management;
using PGE.Interfaces.MapProduction.Export;
using System.Runtime.InteropServices; 

namespace PGE.Interfaces.MapProduction.Exporter
{
    public class Program
    {
        private static int _processTimeout = 0;
        private static LicenseInitializer m_AOLicenseInitializer = new LicenseInitializer();

        /// <summary>
        /// Main program to execute the export operation
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            Environment.ExitCode = 0;
            PGEExportHelper pExportHelper = null;
            MapProdMap pMap = null;
            PGEExportLogger pLogger = null;

            try
            {
                int start = -1;
                int end = -1;
                int processNumber = 0;
                string mapGeo = "";
                bool isChild = false;
                bool isExport = false;
                bool initialize = false;
                bool updateMXDNames = false;
                bool setupMapGeoPolygons = false;
                ExtractType pExtractType = ExtractType.ExtractTypeEDGIS;

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
                    else if (args[i] == "-o")
                    {
                        mapGeo = args[i + 1].ToString();
                    }
                    else if (args[i] == "-child")
                    {
                        isChild = true;
                    }
                    else if (args[i] == "-i")
                    {
                        initialize = true;
                    }
                    else if (args[i] == "-u")
                    {
                        updateMXDNames = true;
                    }
                    else if (args[i] == "-x")
                    {
                        isExport = true;
                    }
                    else if (args[i] == "-l")
                    {
                        pExtractType = ExtractType.ExtractTypeLandbase;
                    }
                    else if (args[i] == "-z")
                    {
                        setupMapGeoPolygons = true;
                    }
                }

                //Initialiase ArcGIS License 
                m_AOLicenseInitializer.InitializeApplication(
                    new esriLicenseProductCode[] { esriLicenseProductCode.esriLicenseProductCodeAdvanced },
                    new esriLicenseExtensionCode[] { });

                //*******************************************************************
                //Specific function to save the mapgeo shapes, must be run when changes are 
                //made to the edgis.maintenanceplat layer 
                if (setupMapGeoPolygons)
                {
                    processNumber = -1;
                    pLogger = new PGEExportLogger(processNumber);
                    pExportHelper = new PGEExportHelper(
                                    Convert.ToInt32(MapProductionConfigurationHandler.GetSettingValue("MaxAttempts")),
                                    pLogger);
                    pExportHelper.SetupMapGeoPolygons();
                    //m_AOLicenseInitializer.ShutdownApplication();
                    //return;
                }
                //*******************************************************************

                //Initialize flag - so setup the PGE_MAPNUMBERCOORDLUT mxdname field 
                else if (initialize)
                {
                    processNumber = -1;
                    pLogger = new PGEExportLogger(processNumber);
                    pLogger.Log("initialization starting...");
                    pLogger.Log("parent ADO connection opened successfully");
                    pExportHelper = new PGEExportHelper(
                                    Convert.ToInt32(MapProductionConfigurationHandler.GetSettingValue("MaxAttempts")),
                                    pLogger);
                    pExportHelper.InitializeMapProduction(updateMXDNames);
                    pExportHelper.InitializeExtract(pExtractType);
                    //return;
                }


                else if ((start != -1) &&
                    (end != -1) &&
                    (isExport == true))
                {
                    //parent export process 
                    if (pExtractType == ExtractType.ExtractTypeLandbase)
                        processNumber = 33;
                    else
                        processNumber = 34;
                    pLogger = new PGEExportLogger(processNumber);
                    pLogger.Log("parent export process starting...");
                    pLogger.Log("processNumber: " + processNumber.ToString());

                    //Setup the PGEExportHelper
                    pExportHelper = new PGEExportHelper(
                            Convert.ToInt32(MapProductionConfigurationHandler.GetSettingValue("MaxAttempts")),
                            pLogger);

                    //Start the controller (parent process)
                    if (pExtractType == ExtractType.ExtractTypeLandbase)
                        pExportHelper.ExtractLandbase(start, end);
                    else
                        pExportHelper.ProcessMaps(start, end);

                    pLogger.Log("parent export process exiting");
                }

                else if ((start != -1) &&
                    (end != -1) &&
                    (isExport == false))
                {
                    //parent mapping process 
                    processNumber = 35;
                    pLogger = new PGEExportLogger(processNumber);
                    pLogger.Log("parent mapping process starting...");
                    pLogger.Log("processNumber: " + processNumber.ToString());

                    //Setup the PGEExportHelper
                    pExportHelper = new PGEExportHelper(
                            Convert.ToInt32(MapProductionConfigurationHandler.GetSettingValue("MaxAttempts")),
                            pLogger);

                    //Spawn the child processes 
                    int processTimeout = getProcessTimeout();
                    pLogger.Log("processTimeout: " + processTimeout.ToString());
                    pLogger.Log("Parent process launching child processes: " +
                        start.ToString() + " to: " + end.ToString());
                    Hashtable hshProcesses = SpawnMappingProcesses(start, end, mapGeo, pLogger);
                    Hashtable hshProcessStartTimes = new Hashtable();
                    foreach (int procNum in hshProcesses.Keys)
                    {
                        hshProcessStartTimes.Add(procNum, DateTime.Now);
                    }

                    //Monitor each of the the child processes 
                    bool mapsToProcess = true;
                    bool restartProcess = false;
                    bool allExited = false;                     

                    while (!allExited)
                    {
                        //Determine if there are still maps to be processed
                        mapsToProcess = false;
                        pMap = pExportHelper.GetNextMapToProcess(processNumber, null, mapGeo, false);
                        if (pMap != null)
                        {
                            mapsToProcess = true;
                        }

                        DateTime pNow = DateTime.Now; 
                        Hashtable hshCurrentMapStartTimes = pExportHelper.GetStartTimeOfCurrentMaps(
                        start, end, pNow);
                        allExited = true;
                        for (int i = start; i <= end; i++)
                        {
                            pLogger.Log("parent process monitoring child process: " + i.ToString());
                            Process pProcess = (Process)hshProcesses[i];
                            DateTime processStartTime = (DateTime)hshProcessStartTimes[i];

                            //Check to see if all child processes have exited 
                            if (!pProcess.HasExited)
                                allExited = false;
                            else
                            {
                                pLogger.Log("child process: " + i.ToString() + " has exited");
                                if (pProcess.ExitCode != (int)ExitCodes.Success)
                                    Environment.ExitCode = (int)ExitCodes.ChildFailure;
                            }

                            //Find the last time when this process successfully exported 
                            //a map and compare it to the timeout - if timeout exceeded 
                            //then Kill process and restart the process
                            restartProcess = false;
                            TimeSpan ts = pNow - (DateTime)hshCurrentMapStartTimes[i];
                            if (ts.TotalMilliseconds > processTimeout)
                            {
                                pLogger.Log("Flagging process: " + i.ToString() + " for restart: " + 
                                    "totalmilliseconds: " + ts.TotalMilliseconds.ToString());
                                restartProcess = true;
                            }

                            //If we need to restart kill the process 
                            if (restartProcess)
                            {
                                if (!pProcess.HasExited)
                                {
                                    pLogger.Log("Parent killing child process: " + i.ToString());
                                    pExportHelper.UpdateMapStatus(
                                        i,
                                        "Map processing time exceeded the configured timeout interval");
                                    pProcess.Kill();
                                }
                            }

                            //If the process has exited, restart because there are still 
                            //maps left to process 
                            if ((pProcess.HasExited) && mapsToProcess)
                            {
                                //Still some maps to process, so lets spawn a new child 
                                Process pNewProcess = SpawnMappingProcess(i, true, false, ProcessWindowStyle.Hidden, true, mapGeo, pLogger);
                                allExited = false;
                                hshProcesses[i] = pNewProcess;
                                hshProcessStartTimes[i] = DateTime.Now;
                                processStartTime = DateTime.Now;
                                pLogger.Log("Child process exited with maps still left to process, restarted process: " + i.ToString());
                            }
                        }
                        //sleep for 60 seconds 
                        Thread.Sleep(60000); 
                    }

                    pLogger.Log("All child threads closed, so parent process will shut down");
                    Environment.ExitCode = 0;
                }
                else
                {
                    //This is a child process spawned by parent process 
                    //pLogger = new PGEExportLogger(processNumber); 
                    //pLogger.Log("child mapping process starting...");
                    //pLogger.Log("processNumber: " + processNumber.ToString());
                    
                    //Arguments check 
                    if (args == null || args.Length < 1)
                    {
                    }
                    else
                    {

                        //Get the process number
                        int.TryParse(args[0], out processNumber);

                        //ProcessNumber cannot be 0 - as this is the parent process 
                        if (processNumber == 0)
                            return;

                        ////Child process 
                        //if (isChild)
                        //{
                            IList<string> errors = null;

                            //Perform the export operation
                            PGEExporter exporter = new PGEExporter();
                            errors = exporter.Export(processNumber, mapGeo);
                        //}
                    }
                }

                //Be sure to shut down resources 
                if (m_AOLicenseInitializer != null)
                    m_AOLicenseInitializer.ShutdownApplication();
                if (pExportHelper != null)
                    pExportHelper.Dispose();
            }
            catch (Exception ex)
            {
                if (pLogger != null)
                {
                    pLogger.Log("Entering error handler for Main: " + ex.Message);
                    pLogger.Log("Returning exit code of failure");
                }
                Environment.ExitCode = (int)ExitCodes.Failure;
            }
            //finally
            //{
            //    //Be sure to shut down resources 
            //    if (m_AOLicenseInitializer != null)
            //        m_AOLicenseInitializer.ShutdownApplication();
            //    if (pExportHelper != null)
            //        pExportHelper.Dispose();
            //}
        }

        /// <summary>
        /// Returns the configured time interval which is the maximum time 
        /// allowed for a map to be processed. If the interval is exceeded 
        /// the process is terminated 
        /// </summary>
        /// <returns></returns>
        private static int getProcessTimeout()
        {
            try
            {
                if (_processTimeout == 0)
                {
                    _processTimeout = int.TryParse(MapProductionConfigurationHandler.Settings["ProcessTimeout"], out _processTimeout) ? _processTimeout : 1200000;
                }
            }
            catch (Exception e) { _processTimeout = 1200000; }
            return _processTimeout;
        }

        /// <summary>
        /// Spawns the executables that perform the exporting of 
        /// the maps 
        /// </summary>
        /// <param name="processID"></param>
        /// <param name="isChild"></param>
        /// <param name="createWindow"></param>
        /// <param name="windowStyle"></param>
        /// <param name="redirectOutput"></param>
        /// <param name="legacyCoord"></param>
        /// <param name="pLogger"></param>
        /// <returns></returns>
        private static Process SpawnMappingProcess( 
            int processID, 
            bool isChild, 
            bool createWindow, 
            ProcessWindowStyle windowStyle, 
            bool redirectOutput, 
            string legacyCoord, 
            PGEExportLogger pLogger)
        {
            try
            {
                //_logger.Debug("Spawning child process: " + processID.ToString());
                pLogger.Log("Entering SpawnMappingProcess");
                pLogger.Log("   processID: " + processID.ToString());
                pLogger.Log("   isChild: " + isChild.ToString());
                pLogger.Log("   legacyCoord: " + legacyCoord);
                string arguments = processID.ToString() + " -o " + legacyCoord;

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
            catch (Exception ex)
            {
                //pLogger.Log("Entering Error Handler for SpawnMappingProcess: " + ex.Message);
                throw new Exception("Error spawning mapping process"); 
            }
        }

        /// <summary>
        /// Launches an individual executables that performs the exporting of 
        /// the maps 
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="legacyCoord"></param>
        /// <param name="pLogger"></param>
        /// <returns></returns>

        private static Hashtable SpawnMappingProcesses(int start, int end, string legacyCoord, PGEExportLogger pLogger)
        {
            try
            {
                pLogger.Log("Entering SpawnMappingProcesses");
                pLogger.Log("   start: " + start.ToString());
                pLogger.Log("   end: " + end.ToString());
                pLogger.Log("   legacyCoord: " + legacyCoord);

                Hashtable hshProcesses = new Hashtable();
                for (int i = start; i <= end; i++)
                {
                    hshProcesses.Add(i, SpawnMappingProcess(i, true, true, ProcessWindowStyle.Normal, false, legacyCoord, pLogger));
                }
                return hshProcesses;
            }
            catch (Exception ex)
            {
                pLogger.Log("Entering Error Handler for SpawnMappingProcesses: " + ex.Message);
                throw new Exception("Error spawning mapping processes");
            }
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
