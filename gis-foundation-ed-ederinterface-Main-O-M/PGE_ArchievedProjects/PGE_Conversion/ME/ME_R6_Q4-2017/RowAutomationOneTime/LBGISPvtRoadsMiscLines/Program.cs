using System;
using System.Configuration;
using System.Reflection;
using System.IO;
using System.Threading; //.Tasks;

namespace LBGISPvtRoadsMiscLines
{

    public class ClassProcessDivision
    {
        private int _n;
        private int _divN;
        private ManualResetEvent _doneEvent;

        public int N { get { return _n; } }
        public int DivN { get { return _divN; } }

        // Constructor.  
        public ClassProcessDivision(int n, ManualResetEvent doneEvent)
        {
            _n = n;
            _doneEvent = doneEvent;
        }

        // Wrapper method for use with thread pool.  
        public void ThreadPoolCallback(System.Object threadContext)
        {
            int threadIndex = (int)threadContext;
            Console.WriteLine("Division thread {0} started...", threadIndex);
            _divN = ProcessDivision(_n);
            Console.WriteLine("Division thread {0} completed...", threadIndex);
            _doneEvent.Set();
        }

        public int ProcessDivision(int n)
        {
            string curr_loc = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string XY_TOLERANCE = ConfigurationManager.AppSettings["XY_TOLERANCE"];
            string GDB_FILE_LOCATION = curr_loc;
            string GDB_FILE_NAME = "GDB_MAIN";
            string LOG_FILE_PATH = (String.IsNullOrEmpty(ConfigurationManager.AppSettings["LOG_FILE_PATH"]) ? System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) : ConfigurationManager.AppSettings["LOG_FILE_PATH"]) + "\\Logfile_" + DateTime.Now.Ticks.ToString() + ".log"; // "C:\\Users\\t1gx\\Documents\\ArcGIS\\log1.txt";
            string GDB_FILE_PATH = GDB_FILE_LOCATION + "\\" + GDB_FILE_NAME + ".gdb";
            string GDB_DIV_FILE_PATH = GDB_FILE_LOCATION + "\\GDB_DIV_" + n.ToString() + ".gdb";
            string LOT_DIV_TEMP_FC_NAME = "DIV_" + n.ToString() + "_LOT_FC";
            string LOT_DIV_TEMP_FC = GDB_DIV_FILE_PATH + "\\" + LOT_DIV_TEMP_FC_NAME;
            string SYM_DIFF_ONLY_LOTS_FC_NAME = "SYM_DIFF_ONLY_LOTS_FC_" + n.ToString();
            string SYM_DIFF_ONLY_LOTS_FC = GDB_DIV_FILE_PATH + "\\" + SYM_DIFF_ONLY_LOTS_FC_NAME;

            System.Diagnostics.Process proc = new System.Diagnostics.Process();
            proc.StartInfo.FileName = curr_loc + "\\" + "ProcessLotsInDivisions.py";
            proc.StartInfo.UseShellExecute = true;
            string pyLogFile = curr_loc + "\\" + "Log_ProcessLotsInDivision" + n.ToString() + ".txt";
            proc.StartInfo.Arguments = n + " " + pyLogFile + " " + GDB_FILE_PATH + " " + GDB_FILE_LOCATION + " " + XY_TOLERANCE;
            proc.Start();
            proc.WaitForExit();

            return n;
        }
    }

    class Program
    {
        #region private static variables
        private static string sPath = (String.IsNullOrEmpty(ConfigurationManager.AppSettings["LOG_FILE_PATH"]) ? System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) : ConfigurationManager.AppSettings["LOG_FILE_PATH"]) + "\\Logfile_" + DateTime.Now.Ticks.ToString() + ".log"; // "C:\\Users\\t1gx\\Documents\\ArcGIS\\log1.txt";
        private static StreamWriter pSWriter = default(StreamWriter);
        //private static LicenseInitializer m_AOLicenseInitializer = new LBGISPvtRoadsMiscLines.LicenseInitializer();
        //private static IWorkspace workspace = default(IWorkspace);
        //private static IMMAppInitialize arcFMAppInitialize = new MMAppInitializeClass();
        private static string LOG_FILE_PATH = (String.IsNullOrEmpty(ConfigurationManager.AppSettings["LOG_FILE_PATH"]) ? System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) : ConfigurationManager.AppSettings["LOG_FILE_PATH"]) + "\\Logfile_" + DateTime.Now.Ticks.ToString() + ".log";
        #endregion
        /// <summary>
        /// main method
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {

            string curr_loc = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string pyLogFile = string.Empty;
            #region configurable values
            string SDE_FILE_PATH = ConfigurationManager.AppSettings["SDE_FILE_PATH"];
            string ONE_TIME_VERSION_NAME = ConfigurationManager.AppSettings["ONE_TIME_VERSION_NAME"];
            string XY_TOLERANCE = ConfigurationManager.AppSettings["XY_TOLERANCE"];
            string GDB_FILE_LOCATION = curr_loc;
            string GDB_FILE_NAME = "GDB_MAIN";
            // string LOG_FILE_PATH = (String.IsNullOrEmpty(ConfigurationManager.AppSettings["LOG_FILE_PATH"]) ? System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) : ConfigurationManager.AppSettings["LOG_FILE_PATH"]) + "\\Logfile_" + DateTime.Now.Ticks.ToString() + ".log";
            string GDB_FILE_PATH = GDB_FILE_LOCATION + "\\" + GDB_FILE_NAME + ".gdb";
            #endregion
            string arguments = string.Empty;
            try
            {
                //create log file
                createLogFile();
                WriteLine("START");
                System.Diagnostics.Process proc = new System.Diagnostics.Process();

                if (ConfigurationManager.AppSettings["COPY_FEATURE_CLASSES_TO_GDB"].ToString().ToUpper() == "TRUE")
                {
                    WriteLine("Processing CopyFeatureClassesToGDB.py");
                    pyLogFile = curr_loc + "\\Log_CopyFeatureClassesToGDB.txt";
                    proc.StartInfo.FileName = curr_loc + "\\" + "CopyFeatureClassesToGDB.py";
                    proc.StartInfo.UseShellExecute = true;
                    arguments = SDE_FILE_PATH + " " + GDB_FILE_LOCATION + " " + GDB_FILE_NAME + " " + pyLogFile;
                    proc.StartInfo.Arguments = arguments;
                    proc.Start();
                    proc.WaitForExit();
                    WriteLine("Completed");
                }
                else
                {
                    WriteLine("Assuming feature classes have already been copied to GDB_MAIN");
                    WriteLine("Proceeding further");
                }
                const int noOfDivisions = 19;
                int i = 1;
                if (ConfigurationManager.AppSettings["CREATE_SUBSETS_FROM_DIVISIONS"].ToString().ToUpper() == "TRUE")
                {
                    while (i <= noOfDivisions)
                    {
                        WriteLine("Processing CreateSubsetBasedOnDivisions " + i);
                        proc.StartInfo.FileName = curr_loc + "\\" + "CreateSubsetBasedOnDivisions.py";
                        proc.StartInfo.UseShellExecute = true;
                        pyLogFile = curr_loc + "\\" + "Log_CreateSubsetBasedOnDivision" + i.ToString() + ".txt";
                        proc.StartInfo.Arguments = i + " " + pyLogFile + " " + GDB_FILE_PATH + " " + GDB_FILE_LOCATION;
                        proc.Start();
                        proc.WaitForExit();
                        WriteLine("Completed Division " + i);
                        i = i + 1;
                    }
                }
                else
                {
                    WriteLine("Assuming subsets have already been created from divisions");
                    WriteLine("19 gdbs should exist in the executing folder having naming convention like GDB_DIV_<Divisionnumber>");
                    WriteLine("Proceeding further");
                }

                if (ConfigurationManager.AppSettings["PROCESS_ALL_DIVISIONS"].ToString().ToUpper() == "TRUE")
                {
                    WriteLine("Thread pooling for all 19 divisions");
                    WriteLine("One event is used for each Division object.");
                    ManualResetEvent[] doneEvents = new ManualResetEvent[noOfDivisions];
                    ClassProcessDivision[] clsProcessDiv = new ClassProcessDivision[noOfDivisions];
                    WriteLine("Launching " + noOfDivisions + " tasks...");
                    for (int i3 = 0; i3 < noOfDivisions; i3++)
                    {
                        doneEvents[i3] = new ManualResetEvent(false);
                        ClassProcessDivision f = new ClassProcessDivision(i3 + 1, doneEvents[i3]);
                        clsProcessDiv[i3] = f;
                        ThreadPool.QueueUserWorkItem(f.ThreadPoolCallback, i3);
                    }

                    // Wait for all threads in pool to calculate.  
                    WaitHandle.WaitAll(doneEvents);
                    WriteLine("All divisions are complete.");

                    // Display the results.  
                    for (int i2 = 0; i2 < noOfDivisions; i2++)
                    {
                        ClassProcessDivision f = clsProcessDiv[i2];
                        WriteLine("Division(" + f.N + ") = " + f.DivN);
                    }
                }
                else
                {
                    WriteLine("Assuming All threads for 19 divisions have processed successfully");
                    WriteLine("Proceeding further");
                }

                if (ConfigurationManager.AppSettings["APPEND_TO_MAIN_GDB"].ToString().ToUpper() == "TRUE")
                {
                    int i1 = 1;
                    while (i1 <= noOfDivisions)
                    {
                        WriteLine("Processing AppendNewToGdbFcs " + i1);
                        proc.StartInfo.FileName = curr_loc + "\\" + "AppendNewToGdbFcs.py";
                        proc.StartInfo.UseShellExecute = true;
                        pyLogFile = curr_loc + "\\" + "Log_AppendToGdbDivision" + i1.ToString() + ".txt";
                        proc.StartInfo.Arguments = i1 + " " + pyLogFile + " " + GDB_FILE_LOCATION;
                        proc.Start();
                        proc.WaitForExit();
                        WriteLine("Completed Division " + i1);
                        i1 = i1 + 1;
                    }
                }
                else
                {
                    WriteLine("Assuming individual division results have been appended to GDB_MAIN");
                    WriteLine("");
                    WriteLine("Proceeding further");
                }

                if (ConfigurationManager.AppSettings["PROCESS_LOTS_OUTSIDE_DIVISIONS"].ToString().ToUpper() == "TRUE")
                {
                    WriteLine("Processing ProcessLotsOutsideDivisions.py");
                    proc.StartInfo.FileName = curr_loc + "\\ProcessLotsOutsideDivisions.py";
                    proc.StartInfo.UseShellExecute = true;
                    pyLogFile = curr_loc + "\\Log_ProcessLotsOutsideDivisions.txt";
                    arguments = pyLogFile + " " + GDB_FILE_PATH + " " + XY_TOLERANCE;
                    proc.StartInfo.Arguments = arguments;
                    proc.Start();
                    proc.WaitForExit();
                    WriteLine("Completed");
                }
                else
                {
                    WriteLine("Assuming lots outside divisions have been processed to GDB_MAIN");
                    WriteLine("Proceeding further");
                }

                if (ConfigurationManager.AppSettings["FINAL_PROCESSING"].ToString().ToUpper() == "TRUE")
                {
                    WriteLine("Processing FinalProcessing.py");
                    proc.StartInfo.FileName = curr_loc + "\\FinalProcess.py";
                    proc.StartInfo.UseShellExecute = true;
                    pyLogFile = curr_loc + "\\Log_FinalProcess.txt";
                    arguments = GDB_FILE_PATH + " " + pyLogFile;
                    proc.StartInfo.Arguments = arguments;
                    proc.Start();
                    proc.WaitForExit();
                    WriteLine("Completed");
                }
                else
                {
                    WriteLine("Assuming a final processing has been done on GDB_MAIN");
                    WriteLine("Proceeding further");
                }


                //if (ConfigurationManager.AppSettings["CREATE_OPEN_VERSION"].ToString().ToUpper() == "TRUE")
                //{
                //    WriteLine("Initializing License");
                //    m_AOLicenseInitializer.InitializeApplication(new esriLicenseProductCode[] { esriLicenseProductCode.esriLicenseProductCodeBasic, esriLicenseProductCode.esriLicenseProductCodeStandard, esriLicenseProductCode.esriLicenseProductCodeAdvanced },
                //        new esriLicenseExtensionCode[] { });
                //    mmLicenseStatus licenseStatus = CheckOutLicenses(mmLicensedProductCode.mmLPArcFM);
                //    if (RuntimeManager.ActiveRuntime == null)
                //        RuntimeManager.BindLicense(ProductCode.Desktop);
                //    WriteLine("Getting workspace from SDE connection file. ");
                //    workspace = ArcSdeWorkspaceFromFile(SDE_FILE_PATH);
                //    IVersionedWorkspace pVersionedWorkspace = workspace as IVersionedWorkspace;
                //    IVersion pDefaultVersion = pVersionedWorkspace.DefaultVersion;
                //    IVersion pPvtMiscVersion = null;
                //    try
                //    {
                //        pPvtMiscVersion = pVersionedWorkspace.FindVersion(ONE_TIME_VERSION_NAME);
                //        WriteLine("Existing Version found: " + ONE_TIME_VERSION_NAME);
                //        pPvtMiscVersion.Delete();
                //        WriteLine("Version deleted ");
                //        VersionCreate(pVersionedWorkspace, pDefaultVersion, ONE_TIME_VERSION_NAME);
                //        WriteLine("Version created: " + ONE_TIME_VERSION_NAME + " as child of Default.");
                //    }
                //    catch (Exception ex)
                //    {
                //        if (ex.Message.Contains("Version not found"))
                //        {
                //            WriteLine("Assuming no previous ROW version exists, Creating a new version from default version");
                //            VersionCreate(pVersionedWorkspace, pDefaultVersion, ONE_TIME_VERSION_NAME);
                //            WriteLine("Version created: " + ONE_TIME_VERSION_NAME + " as child of Default.");
                //        }
                //    }
                //}
                //else
                //{
                //    WriteLine("Assuming SDE. " + ONE_TIME_VERSION_NAME + " already exists.");
                //    WriteLine("Proceeding further");
                //}

                if (ConfigurationManager.AppSettings["MOVE_TO_SDE_VERSION"].ToString().ToUpper() == "TRUE")
                {
                    WriteLine("Processing MoveToSDE.py");
                    proc.StartInfo.FileName = curr_loc + "\\MoveToSDE.py";
                    pyLogFile = curr_loc + "\\Log_MoveToSDE.txt";
                    proc.StartInfo.UseShellExecute = true;
                    arguments = SDE_FILE_PATH + " " + GDB_FILE_PATH + " " + pyLogFile + " " + ONE_TIME_VERSION_NAME;
                    proc.StartInfo.Arguments = arguments;
                    proc.Start();
                    proc.WaitForExit();
                    WriteLine("Completed");
                }
                else
                {
                    WriteLine("MOVE_TO_SDE_VERSION is set as false");
                    WriteLine("");
                }

                WriteLine("Process completed");
                return;
            }
            catch (Exception ex)
            {
                WriteLine("ERROR " + ex.Message);
            }
            finally
            {
                WriteLine("");
                WriteLine("Finally");
                WriteLine("END");
            }
        }
        //private static IWorkspace OpenSDEWorkspacefromsdefile(string sdefilenamewithpath)
        //{
        //    IWorkspace workspace = null;
        //    try
        //    {
        //        // Create an SDE workspace factory and open the workspace.
        //        Type factoryType = Type.GetTypeFromProgID("esriDataSourcesGDB.SdeWorkspaceFactory");
        //        IWorkspaceFactory workspaceFactory = (IWorkspaceFactory)Activator.CreateInstance(factoryType);
        //        workspace = workspaceFactory.OpenFromFile(sdefilenamewithpath, 0);
        //    }
        //    catch (Exception ex)
        //    {
        //        WriteLine(ex.Message);
        //    }
        //    return workspace;

        //}
        //public static void VersionCreate(IVersionedWorkspace versionedWorkspace, IVersion parentVersion, string newversionName)
        //{

        //    IVersion version;
        //    version = parentVersion.CreateVersion(newversionName);
        //    //setting the versions access    
        //    version.Access = esriVersionAccess.esriVersionAccessPublic;
        //    //setting the versiones description
        //    version.Description = "For Lotline and Row Daily Update process";
        //}
        //private static IWorkspace ArcSdeWorkspaceFromFile(String connectionFile)
        //{
        //    return ((IWorkspaceFactory)Activator.CreateInstance(Type.GetTypeFromProgID("esriDataSourcesGDB.SdeWorkspaceFactory"))).
        //        OpenFromFile(connectionFile, 0);
        //}
        /// <summary>
        /// ArcFM Licence Initializer
        /// </summary>
        /// <param name="productCode">Code to checkout</param>
        /// <returns>Licence Status</returns>
        //private static mmLicenseStatus CheckOutLicenses(mmLicensedProductCode productCode)
        //{
        //    mmLicenseStatus licenseStatus;
        //    licenseStatus = arcFMAppInitialize.IsProductCodeAvailable(productCode);
        //    if (licenseStatus == mmLicenseStatus.mmLicenseAvailable)
        //    {
        //        licenseStatus = arcFMAppInitialize.Initialize(productCode);
        //    }
        //    return licenseStatus;
        //}

        public static void createLogFile()
        {
            (pSWriter = File.CreateText(sPath)).Close();
        }
        /// <summary>
        /// Write on console and log file
        /// </summary>
        /// <param name="sMsg"></param>
        public static void WriteLine(string sMsg)
        {
            sMsg = !String.IsNullOrEmpty(sMsg) ? DateTime.Now.ToLongTimeString() + " -- " + sMsg : sMsg;
            pSWriter = File.AppendText(sPath);
            pSWriter.WriteLine(sMsg);
            Console.WriteLine(sMsg);
            pSWriter.Close();
        }
    }
}

