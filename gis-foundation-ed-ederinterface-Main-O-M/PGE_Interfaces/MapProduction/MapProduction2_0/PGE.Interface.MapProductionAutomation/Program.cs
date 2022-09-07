using System;
using System.Collections; 
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;
using Oracle.DataAccess.Client;
using System.Text;
using ESRI.ArcGIS.esriSystem;
using System.IO;
using PGE.Interfaces.MapProductionAutomation.MapProd;
using System.Reflection;
using System.Diagnostics;
using System.Threading;
using PGE.Common.Delivery.Diagnostics;
using PGE.Common.Delivery.Framework;
using Miner.Interop; 

namespace PGE.Interfaces.MapProductionAutomation
{
    class Program
    {
        //private static LicenseInitializer m_AOLicenseInitializer = new PGE.Interfaces.MapProductionAutomation.LicenseInitializer();
        //private static Log4NetLogger _logger = new Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "\\telvent.mapproduction.log4net.config", "MapProduction");
        //private static int _processTimeout = 0;
        public static string configurationFileName = "";


        [STAThread()]
        //arg1  -c "C:\Yasha\gis-foundation-ed-ederinterface\PGE_Interfaces\MapProduction\MapProduction2_0\PGE.Interface.MapProductionAutomation\bin\Debug\PGE.Interfaces.MapProductionAutomation.exe.config" -Gchild 1 DVEDGISBUDLX001 ED_M_AND_C 100 ED_M_and_C.mxd 0 BE129-I12
        //ar2 1 -c "C:\GIT_DP\gis-foundation-ed-ederinterface\PGE_Interfaces\MapProduction\MapProduction2_0\PGE.Interface.MapProductionAutomation\MapProductionAutomation.exe.config" -child
        static void Main(string[] args)
        {
            Environment.ExitCode = 0;
            MapProductionHelper pExportHelper = null;
            try
            {
                int start = -1;
                int end = -1;
                //string mapDivision = "";
                bool isChild = false;
                bool initialize = false;
                bool isGChild = false;
                int processNumber = 0;
                string pwd = "";
                string configurationFile1 = "";
                
                

                if (args.Length > 0 && args[0] == "/?")
                {
                    Console.WriteLine("-s: Specify the starting process count");
                    Console.WriteLine("-e: Specify the ending process count");
                    Console.WriteLine("-c: Specify the location of the configuration file to use");
                    Console.WriteLine("-pwd: Specify a new password for encryption in the configuration file");
                    Console.WriteLine("-i: Initialize map production tables.  Run before executing processes");
                    Console.WriteLine("Valid Argument combinations");
                    Console.WriteLine("-s,-e,-c");
                    Console.WriteLine("-c,-pwd");
                    Console.WriteLine("-i,-c");
                    Environment.Exit(0);
                }

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
                    else if (args[i] == "-c")
                    {
                        configurationFile1 = args[i + 1].ToString();
                    }
                    else if (args[i] == "-pwd")
                    {
                        pwd = args[i + 1].ToString();
                    }
                    else if (args[i] == "-child")
                    {
                        isChild = true;
                    }
                    else if (args[i] == "-Gchild")
                    {
                        isGChild = true;
                    }
                    else if (args[i] == "-i")
                    {
                        initialize = true;
                    }
                }
                 //configurationFile = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) +  configurationFileName ;
                //Password reset 
                if (pwd != string.Empty)
                {
                    Common.Common.ResetPassword(pwd, configurationFile1);
                    return;
                }

                //Initialize flag - so setup changedetection table 
                if (initialize)
                {
                    InitializeChangeDetection(configurationFile1);
                    return;
                }
                if(isGChild)
                {
                    //configurationFile = System.Reflection.Assembly.GetExecutingAssembly().Location + ".config";
                    MapProduction mapProduction = new MapProduction(Convert.ToInt32(args[3]), configurationFile1);
                    using (LicenseInitializer licenseManager = new LicenseInitializer())
                    {
                        //Initialiase ArcGIS License
                        MapProductionHelper.Log("Checking out Esri license", MapProductionHelper.LogType.Info, null);
                        bool licenceCheckoutSuccess = licenseManager.InitializeApplication(new esriLicenseProductCode[] { Common.Common.EsriLicense }, Common.Common.EsriExtensions.ToArray());

                        //if (licenceCheckoutSuccess)
                        //{
                        //    MapProductionHelper.Log("Checking out ArcFM License", MapProductionHelper.LogType.Info, null);
                        //    bool arcFMCheckoutSuccess = licenseManager.GetArcFMLicense(Common.Common.ArcFMLicense);

                        //    if (arcFMCheckoutSuccess)
                        //    {
                        //        //Perform the export operation 

                        //        //MapProductionHelper.Log("Beginning export of maps", MapProductionHelper.LogType.Info, null);
                        //        //mapProduction.NextMaptoProcess(configurationFile1, Convert.ToInt32(args[3]), args[4], args[5],
                        //        //    args[6],args[7], Convert.ToInt32(args[8]),args[9]);
                        //        //MapProductionHelper.Log("Completed export of maps", MapProductionHelper.LogType.Info, null);
                        //        return;
                        //    }
                        //}
                    }
                          
                }
                if (start != -1 && end != -1)
                {                    
                    //Open the ADO DB connection 
                    Common.Common.ReadAppSettings(configurationFile1);
                    pExportHelper = new MapProductionHelper(Common.Common.MaxTries, processNumber, true);
                    MapProductionHelper.Log("parent process starting...", MapProductionHelper.LogType.Info, null);
                    MapProductionHelper.Log("processNumber: " + processNumber.ToString(), MapProductionHelper.LogType.Info, null);
                    MapProductionHelper.Log("parent ADO.NET connection opened successfully", MapProductionHelper.LogType.Info, null);
                    MapProductionHelper.Log("process Timeout: " + Common.Common.ProcessTimeout.ToString(), MapProductionHelper.LogType.Info, null);
                    
                    //Clear out the temporary mapping directory 
                    MapProductionHelper.Log("clearing out the temp directory", MapProductionHelper.LogType.Info, null);
                    DeleteFilesInDirectory(pExportHelper); 

                    //Spawn the child processes 
                    MapProductionHelper.Log("Parent process launching child processes: " +
                        start.ToString() + " to: " + end.ToString(), MapProductionHelper.LogType.Info, null);
                    Hashtable hshProcesses = SpawnProcesses(start, end, configurationFile1);
                    Hashtable hshProcessStartTimes = new Hashtable();
                    foreach (int procNum in hshProcesses.Keys)
                    {
                        hshProcessStartTimes.Add(procNum, DateTime.Now);
                    }

                    //Monitor each of the the child processes 
                    bool mapsToProcess = true;
                    bool restartProcess = false;
                    bool allExited = false;
                    string curMapType = "";
                    string curMapMXD = "";

                    while (!allExited)
                    {
                        //Determine if there are still maps to be processed
                        mapsToProcess = false;
                        mapsToProcess = pExportHelper.MapsStillToProcess(configurationFile1);

                        allExited = true;
                        for (int i = start; i <= end; i++)
                        {
                            MapProductionHelper.Log("parent process monitoring child process: " + i.ToString(), MapProductionHelper.LogType.Info, null);
                            Process pProcess = (Process)hshProcesses[i];
                            DateTime processStartTime = (DateTime)hshProcessStartTimes[i];

                            //Check to see if all child processes have exited 
                            if (!pProcess.HasExited)
                                allExited = false;
                            else
                            {
                                MapProductionHelper.Log("child process: " + i.ToString() + " has exited", MapProductionHelper.LogType.Info, null);
                                if (pProcess.ExitCode != (int)Common.Common.ExitCodes.Success)
                                    Environment.ExitCode = (int)Common.Common.ExitCodes.ChildFailure;
                            }

                            //Find the last time when this process successfully exported 
                            //a map and compare it to the timeout - if timeout exceeded 
                            //then Kill process and restart the process
                            restartProcess = false;
                            DateTime lastSuccess = pExportHelper.GetTimeOfLastSucessfulMap(
                                    i, processStartTime);
                            int ts = DateTime.Now.Minute  - lastSuccess.Minute;
                            if (ts >30 )
                            {
                               // MapProductionHelper.Log("ts.TotalMilliseconds: " + ts.TotalMilliseconds.ToString(), MapProductionHelper.LogType.Info, null);
                                MapProductionHelper.Log("Common.Common.ProcessTimeout: " + Common.Common.ProcessTimeout.ToString(), MapProductionHelper.LogType.Info, null);
                                MapProductionHelper.Log("Successful maps - restart = true", MapProductionHelper.LogType.Info, null);
                                restartProcess = true;
                            }

                            //If we need to restart kill the process 
                            if (restartProcess)
                            {
                                if (!pProcess.HasExited)
                                {
                                    MapProductionHelper.Log("Parent killing child process: " + i.ToString(), MapProductionHelper.LogType.Info, null);
                                    pProcess.Kill();
                                }
                            }

                            //If the process has exited, restart because there are still 
                            //maps left to process 
                            if ((pProcess.HasExited) && mapsToProcess)
                            {
                                //Still some maps to process, so lets spawn a new child 
                                Process pNewProcess = SpawnProcess(i, true, false, ProcessWindowStyle.Hidden, true, configurationFile1);
                                allExited = false;
                                hshProcesses[i] = pNewProcess;
                                hshProcessStartTimes[i] = DateTime.Now;
                                processStartTime = DateTime.Now;
                                MapProductionHelper.Log("Child process exited with maps still left to process, restarted process: " + i.ToString(), MapProductionHelper.LogType.Info, null);
                            }
                        }
                        Thread.Sleep(10000);
                    }

                    MapProductionHelper.Log("All child threads closed, so parent process will shut down", MapProductionHelper.LogType.Info, null);
                    Environment.ExitCode = 0;
                }
                else
                {
                    //configurationFile = System.Reflection.Assembly.GetExecutingAssembly().Location + ".config";
                    //Child processing 
                    Common.Common.ReadAppSettings(configurationFile1);
                    if (args == null || args.Length < 1)
                        return;

                    //Get the process number                    
                    int.TryParse(args[0], out processNumber);
                    if (processNumber == 0)
                        return;

                    if (isChild)
                    {
                        using (LicenseInitializer licenseManager = new LicenseInitializer())
                        {
                            //Initialiase ArcGIS License
                            MapProductionHelper.Log("Checking out Esri license", MapProductionHelper.LogType.Info, null);
                            bool licenceCheckoutSuccess = licenseManager.InitializeApplication(new esriLicenseProductCode[] { Common.Common.EsriLicense }, Common.Common.EsriExtensions.ToArray());

                            if (licenceCheckoutSuccess)
                            {
                                MapProductionHelper.Log("Checking out ArcFM License", MapProductionHelper.LogType.Info, null);
                                bool arcFMCheckoutSuccess = licenseManager.GetArcFMLicense(Common.Common.ArcFMLicense);

                                if (arcFMCheckoutSuccess)
                                {
                                    //Perform the export operation 
                                     MapProduction  mapProduction = new MapProduction(processNumber, configurationFile1);
                                    MapProductionHelper.Log("Beginning export of maps", MapProductionHelper.LogType.Info, null);
                                    mapProduction.ExportMaps(processNumber, configurationFile1);
                                }
                                else
                                {
                                    MapProductionHelper.Log("Failed to checkout ArcFM license", MapProductionHelper.LogType.Info, null);
                                }
                            }
                            else
                            {
                                MapProductionHelper.Log("Failed to checkout Esri license", MapProductionHelper.LogType.Info, null);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Environment.ExitCode = (int)Common.Common.ExitCodes.Failure;
                if (pExportHelper != null)
                    MapProductionHelper.Log("", MapProductionHelper.LogType.Error, ex);
            }
            finally
            {

                if (pExportHelper != null)
                    pExportHelper.Disconnect(); 
            }
        }

        /// <summary>
        /// Removes all files and folders from the working directory 
        /// </summary>
        private static void DeleteFilesInDirectory(MapProductionHelper pExportHelper)
        {
            try
            {
                System.IO.DirectoryInfo tempFolder = new DirectoryInfo(Common.Common.TempFileDirectory);
                foreach (FileInfo file in tempFolder.GetFiles())
                {
                    file.Delete();
                }
                foreach (DirectoryInfo dir in tempFolder.GetDirectories())
                {
                    dir.Delete(true);
                }
            }
            catch (Exception ex)
            {
                MapProductionHelper.Log("Error in DeleteFilesInDirectory", MapProductionHelper.LogType.Error, ex); 
            }
        }

        /// <summary>
        /// Initialize change detection by creating the map (which are 
        /// flagged as having changes by change detection) records in the 
        /// PGE_CHANGEDETECTIONGRIDS table 
        /// </summary>
        public static void InitializeChangeDetection(string configurationFile)
        {
            MapProductionHelper pExportHelper = null; 

            try
            {
                Common.Common.ReadAppSettings(configurationFile);  
                pExportHelper = new MapProductionHelper(Common.Common.MaxTries, 0, true);
                MapProductionHelper.Log("Entering InitializeChangeDetection", MapProductionHelper.LogType.Info, null);                               
                pExportHelper.InsertChangeDetectionMaps(configurationFile);                
            }
            catch (Exception ex)
            {
                //Be sure to close connection
                if (pExportHelper != null)
                {
                    MapProductionHelper.Log("Error in InitializeChangeDetection", MapProductionHelper.LogType.Error, ex);
                }
            }
            finally
            {
                if (pExportHelper != null)
                    pExportHelper.Disconnect(); 
            }
        }

        private static Process SpawnProcess(
            int processID,
            bool isChild,
            bool createWindow,
            ProcessWindowStyle windowStyle,
            bool redirectOutput,
            string configurationFile)
        {
            //string arguments = processID.ToString() + " -o " + mapDivision;
            string arguments = processID.ToString();
            //Add the configuration file name to arguments
            arguments += " -c \"" + configurationFile + "\"";

            if (isChild)
            {
                arguments += " -child";
            }

            ProcessStartInfo startInfo = new ProcessStartInfo(System.Reflection.Assembly.GetExecutingAssembly().Location, arguments)
            {
                //Suggested by Rasu in EDER/Interface Code Review Meeting
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                Verb = "runas",
                CreateNoWindow=true,
                //CreateNoWindow = createWindow,
                WindowStyle = windowStyle,
                //UseShellExecute = false,
                //RedirectStandardOutput = redirectOutput
            };
            return Process.Start(startInfo);
        }

        private static Hashtable SpawnProcesses(int start, int end, string configurationFile)
        {
            Hashtable hshProcesses = new Hashtable(); 
            for (int i = start; i <= end; i++)
            {
                hshProcesses.Add(i, SpawnProcess(i, true, true, ProcessWindowStyle.Normal, false, configurationFile));
            }
            return hshProcesses;
        }

        //static void Main(string[] args)
        //{
        //    Environment.ExitCode = (int)Common.Common.ExitCodes.Success;

        //    AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);

        //    string errorLog = "";

        //    if (args[0] == "/?")
        //    {
        //        //Print out the arguments to the user
        //        Console.WriteLine("Valid Arguments:");
        //        Console.WriteLine("-c {SDE connection file}");
        //        Console.WriteLine("-M {Mxd Location}");
        //        Console.WriteLine("-E {Export Location}");
        //        Console.WriteLine("-TL {Temp Location}");
        //        Console.WriteLine("-T {Template Name}");
        //        Console.WriteLine("-S {Scale}");
        //        Console.WriteLine("-sd {Stored Display Name}");
        //        Console.WriteLine("-f {File Export Format}");
        //        Console.WriteLine("-O {Overwrite (will not overwrite existing PDFs if this is not specified}");
        //        Console.WriteLine("-A {Generate All Maps (will use change detection logic if not specified}");
        //        Console.WriteLine("-D {Specifies feature classes that define what it means for a map to have data (ex. EDGIS.PriOHConductor,EDGIS.PriUGConductor}");
        //        Console.WriteLine("");
        //        Console.WriteLine("Valid argument combinations");
        //        Console.WriteLine("Argument set 1: -c,-O,-A");
        //        Console.WriteLine("Argument set 2: -c,-M,-E,-TL,-T,-S,-sd,-f,-O,-D");
        //        Console.WriteLine("");
        //        Console.WriteLine("Notes:");
        //        Console.WriteLine("When using argument set 1, all configuration is defined in the PGE.Interfaces.MapProductionAutomation.exe.config file located in the installation directory");
        //    }
        //    else
        //    {
        //        Console.WriteLine("************************************************");
        //        Console.WriteLine("       PG&E Map production");
        //        Console.WriteLine("************************************************");


        //        try
        //        {

        //            //ESRI License Initializer generated code.
        //            Console.WriteLine("Checking out Esri license...");
        //            bool licenseCheckoutSuccess = m_AOLicenseInitializer.InitializeApplication(new esriLicenseProductCode[] { esriLicenseProductCode.esriLicenseProductCodeArcInfo },
        //            new esriLicenseExtensionCode[] { });

        //            if (!licenseCheckoutSuccess)
        //            {
        //                try
        //                {
        //                    m_AOLicenseInitializer.ReleaseArcFMLicense();
        //                }
        //                catch { }

        //                try
        //                {
        //                    //Do not make any call to ArcObjects after ShutDownApplication()
        //                    m_AOLicenseInitializer.ShutdownApplication();
        //                }
        //                catch { }

        //                //Retry after a minute
        //                Thread.Sleep(60000);
        //                m_AOLicenseInitializer = new PGE.Interfaces.MapProductionAutomation.LicenseInitializer();
        //                licenseCheckoutSuccess = m_AOLicenseInitializer.InitializeApplication(new esriLicenseProductCode[] { esriLicenseProductCode.esriLicenseProductCodeArcInfo },
        //            new esriLicenseExtensionCode[] { });

        //            }

        //            if (licenseCheckoutSuccess)
        //            {
        //                try
        //                {
        //                    Console.WriteLine("Checking out ArcFM license...");
        //                    if (!m_AOLicenseInitializer.GetArcFMLicense(Miner.Interop.mmLicensedProductCode.mmLPArcFM))
        //                    {
        //                        licenseCheckoutSuccess = false;
        //                    }
        //                }
        //                catch (Exception e)
        //                {
        //                    licenseCheckoutSuccess = false;
        //                }

        //                if (!licenseCheckoutSuccess)
        //                {
        //                    //retry after a minute
        //                    Thread.Sleep(60000);
        //                    try
        //                    {
        //                        Console.WriteLine("Checking out ArcFM license...");
        //                        if (!m_AOLicenseInitializer.GetArcFMLicense(Miner.Interop.mmLicensedProductCode.mmLPArcFM))
        //                        {
        //                            errorLog += "Could not check out ArcFM license\n";
        //                            licenseCheckoutSuccess = false;
        //                        }
        //                    }
        //                    catch (Exception e)
        //                    {
        //                        Console.WriteLine("Unable to checkout ArcFM license");
        //                        Console.WriteLine(e.Message);
        //                        errorLog += "Could not check out ArcFM License. Message: " + e.Message + "\n";
        //                        licenseCheckoutSuccess = false;
        //                    }
        //                }
        //            }
        //            else
        //            {
        //                errorLog += "Unable to checkout ArcInfo license\n";
        //            }

        //            try
        //            {
        //                if (licenseCheckoutSuccess)
        //                {
        //                    //Arg: Direct connect string
        //                    //Arg: Mxd Location
        //                    //Arg: Input file location
        //                    //Arg: Base Export location
        //                    //Arg: Map type name (i.e. CircuitMap)
        //                    //Arg: Scale of this execution (i.e. 100, or 500)
        //                    //Arg: Indication of whether to overwrite existing pdfs
        //                    //Arg: Indicate running map production on all map grids
        //                    string sdeConnectionFile = "";
        //                    string mxdLocation = "";
        //                    string inputFileLocation = "";
        //                    string exportLocation = "";
        //                    string tempLocation = "";
        //                    string templateName = "";
        //                    string scale = "";
        //                    string HasDataLayers = "";
        //                    bool overwrite = false;
        //                    bool AllMaps = false;
        //                    string SDName = "";
        //                    string Format = "";
        //                    string StartMapNumber = "";
        //                    string logFileName = "";
        //                    bool usingChangeDetection = false;
        //                    bool parentProcess = false;
        //                    int numProcesses = 0;

        //                    //Parse our arguments first
        //                    for (int i = 0; i < args.Length; i++)
        //                    {
        //                        if (args[i] == "-c")
        //                        {
        //                            sdeConnectionFile = args[i + 1];
        //                        }
        //                        else if (args[i] == "-M")
        //                        {
        //                            mxdLocation = args[i + 1];
        //                        }
        //                        else if (args[i] == "-I")
        //                        {
        //                            inputFileLocation = args[i + 1];
        //                        }
        //                        else if (args[i] == "-E")
        //                        {
        //                            exportLocation = args[i + 1];
        //                        }
        //                        else if (args[i] == "-TL")
        //                        {
        //                            tempLocation = args[i + 1];
        //                        }
        //                        else if (args[i] == "-T")
        //                        {
        //                            templateName = args[i + 1];
        //                        }
        //                        else if (args[i] == "-S")
        //                        {
        //                            scale = args[i + 1];
        //                        }
        //                        else if (args[i] == "-O")
        //                        {
        //                            overwrite = true;
        //                        }
        //                        else if (args[i] == "-A")
        //                        {
        //                            AllMaps = true;
        //                        }
        //                        else if (args[i] == "-D")
        //                        {
        //                            HasDataLayers = args[i + 1];
        //                        }
        //                        else if (args[i] == "-sd")
        //                        {
        //                            SDName = args[i + 1];
        //                        }
        //                        else if (args[i] == "-f")
        //                        {
        //                            Format = args[i + 1];
        //                        }
        //                        else if (args[i] == "-l")
        //                        {
        //                            logFileName = args[i + 1];
        //                        }
        //                        else if (args[i] == "-s")
        //                        {
        //                            StartMapNumber = args[i + 1];
        //                        }
        //                        else if (args[i] == "-cd")
        //                        {
        //                            usingChangeDetection = true;
        //                        }
        //                        else if (args[i] == "-p")
        //                        {
        //                            parentProcess = true;
        //                        }
        //                        else if (args[i] == "-n")
        //                        {
        //                            numProcesses = Int32.Parse(args[i + 1]);
        //                        }
        //                    } 

        //                    //If this is going to be a parent process that spawns all other processes
        //                    if (parentProcess && !String.IsNullOrEmpty(sdeConnectionFile) && !String.IsNullOrEmpty(mxdLocation)
        //                        && !String.IsNullOrEmpty(exportLocation) && !String.IsNullOrEmpty(templateName) && !String.IsNullOrEmpty(scale))
        //                    {
        //                        bool execute = true;
        //                        //Let's verify the locations specified above
        //                        if (!File.Exists(mxdLocation))
        //                        {
        //                            errorLog += "Could not find the specified Mxd\n";
        //                            Console.WriteLine("Could not find the specified Mxd");
        //                            execute = false;
        //                        }

        //                        if (execute)
        //                        {
        //                            MapProductionChildSpawn mapProduction = null;

        //                            try
        //                            {
        //                                mapProduction = new MapProductionChildSpawn(sdeConnectionFile, mxdLocation, exportLocation, tempLocation, templateName, scale, overwrite, HasDataLayers, SDName, Format, numProcesses);
        //                            }
        //                            catch (Exception e)
        //                            {
        //                                errorLog += "Error starting process: " + Process.GetCurrentProcess().Id + "\n";
        //                            }
        //                            try
        //                            {
        //                                mapProduction.Log("Printing PDFs");
        //                                mapProduction.SpawnChildren();
        //                            }
        //                            catch (Exception e)
        //                            {
        //                                mapProduction.Log("Error processing maps: Message: " + e.Message + " Stack Trace: " + e.StackTrace);
        //                                errorLog += "Error processing maps: Message: " + e.Message + " Stack Trace: " + e.StackTrace + "\n";
        //                            }
        //                        }
        //                    }
        //                    //Execute either the MapProduction or MapProductionSpawn classes depending on the arguments specified.
        //                    else if (!String.IsNullOrEmpty(sdeConnectionFile) && !String.IsNullOrEmpty(mxdLocation)
        //                        && !String.IsNullOrEmpty(exportLocation) && !String.IsNullOrEmpty(templateName) && !String.IsNullOrEmpty(scale))
        //                    {
        //                        Console.WriteLine("Printing maps into directory \"" + exportLocation + "\"");
        //                        Console.WriteLine("Using map template document \"" + mxdLocation + "\"");
        //                        Console.WriteLine("Using map list \"" + inputFileLocation + "\"");
        //                        Console.WriteLine();

        //                        bool execute = true;
        //                        //Let's verify the locations specified above
        //                        if (!File.Exists(mxdLocation))
        //                        {
        //                            errorLog += "Could not find the specified Mxd\n";
        //                            Console.WriteLine("Could not find the specified Mxd");
        //                            execute = false;
        //                        }

        //                        if (execute)
        //                        {
        //                            MapProduction mapProduction = null;

        //                            try
        //                            {
        //                                mapProduction = new MapProduction(sdeConnectionFile, mxdLocation, exportLocation, tempLocation, templateName, scale, overwrite, HasDataLayers, SDName, Format, logFileName);
        //                            }
        //                            catch (Exception e)
        //                            {
        //                                errorLog += "Error starting process: " + Process.GetCurrentProcess().Id + "\n";
        //                            }
        //                            try
        //                            {
        //                                mapProduction.Log("Printing PDFs");
        //                                mapProduction.PrintPDFs();
        //                            }
        //                            catch (Exception e)
        //                            {
        //                                mapProduction.Log("Error processing maps: Message: " + e.Message + " Stack Trace: " + e.StackTrace);
        //                                errorLog += "Error processing maps: Message: " + e.Message + " Stack Trace: " + e.StackTrace + "\n";
        //                            }
        //                        }
        //                    }
        //                    else if (!String.IsNullOrEmpty(sdeConnectionFile))
        //                    {
        //                        try
        //                        {
        //                            MapProductionSpawn spawnProcess = new MapProductionSpawn(sdeConnectionFile);
        //                            spawnProcess.SpawnProcesses(AllMaps, overwrite);
        //                        }
        //                        catch (Exception e)
        //                        {
        //                            Console.WriteLine("Error spawning processes.  Message: " + e.Message + " Stacktrace: " + e.StackTrace);
        //                            errorLog += "Error spawning processes.  Message: " + e.Message + " Stacktrace: " + e.StackTrace + "\n";
        //                        }
        //                    }
        //                    else
        //                    {
        //                        Console.WriteLine("Invalid Arguments specified. Please refer to documentation for valid arguments");
        //                        errorLog += "Invalid Arguments specified. Please refer to documentation for valid arguments\n";
        //                    }
        //                }
        //                else
        //                {
        //                    Environment.ExitCode = (int)Common.Common.ExitCodes.LicenseFailure;
        //                    Console.WriteLine("Could not check out an ArcEditor license");
        //                }
        //            }
        //            catch (Exception e)
        //            {
        //                errorLog += "Error during processing: Message: " + e.Message + " StackTrace: " + e.StackTrace + "\n";
        //                Console.WriteLine("Error in execution\n" + e.Message + "\n" + e.StackTrace);
        //                Environment.ExitCode = (int)Common.Common.ExitCodes.Failure;
        //            }

        //            try
        //            {
        //                m_AOLicenseInitializer.ReleaseArcFMLicense();
        //            }
        //            catch { }

        //            try
        //            {
        //                //Do not make any call to ArcObjects after ShutDownApplication()
        //                m_AOLicenseInitializer.ShutdownApplication();
        //            }
        //            catch { }
        //        }
        //        catch (Exception e)
        //        {
        //            Environment.ExitCode = (int)Common.Common.ExitCodes.Failure;
        //            errorLog += "Errors in processing: Message: " + e.Message + " StackTrace: " + e.StackTrace + "\n";
        //        }
        //    }

        //    //Write any errors to log file
        //    if (!String.IsNullOrEmpty(errorLog))
        //    {
        //        //If something fails creating the map production object we need a way to see the error.
        //        string path = Assembly.GetExecutingAssembly().Location;
        //        path = path.Substring(0, path.LastIndexOf("\\"));
        //        string logFileDirectory = System.IO.Path.Combine(path, Common.Common.LogFileDirectory);
        //        string day = DateTime.Today.Day.ToString();
        //        string month = DateTime.Today.Month.ToString();
        //        if (day.Length == 1) { day = "0" + day; }
        //        if (month.Length == 1) { month = "0" + month; }
        //        string year = DateTime.Today.Year.ToString();
        //        logFileDirectory = System.IO.Path.Combine(logFileDirectory, year + month + day);
        //        if (!Directory.Exists(logFileDirectory))
        //        {
        //            Directory.CreateDirectory(logFileDirectory);
        //        }

        //        int num = 0;
        //        bool createLogfile = true;
        //        StreamWriter writer = null;

        //        while (createLogfile)
        //        {
        //            try
        //            {
        //                writer = new StreamWriter(logFileDirectory + "\\" + "ProcessingErrors_Log" + num + ".txt", true);
        //                createLogfile = false;
        //            }
        //            catch { }
        //            num++;
        //        }

        //        writer.WriteLine(errorLog);
        //        writer.Flush();
        //        writer.Close();
        //    }
        //}

        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            try
            {
                Console.WriteLine("Unhandled Exception: Message: " + ((Exception)e.ExceptionObject).Message + " StackTrace: " +
                    ((Exception)e.ExceptionObject).StackTrace);
            }
            catch { }
            Environment.ExitCode = (int)Common.Common.ExitCodes.Failure;
        }
    }
}
