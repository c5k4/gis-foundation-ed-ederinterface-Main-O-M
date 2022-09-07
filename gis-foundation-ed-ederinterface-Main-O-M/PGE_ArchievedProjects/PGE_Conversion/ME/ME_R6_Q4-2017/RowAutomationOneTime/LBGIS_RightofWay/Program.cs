using System;
using System.Configuration;
using System.IO;
using System.Reflection;
using ESRI.ArcGIS.Geodatabase;

namespace LBGISRightofWay
{
    class Program
    {
        #region private static variables
        //private static LicenseInitializer m_AOLicenseInitializer = new LicenseInitializer();
        //private static IWorkspace workspace = default(IWorkspace);
        private static StreamWriter pSWriter = default(StreamWriter);
        //private static IMMAppInitialize arcFMAppInitialize = new Miner.Interop.MMAppInitializeClass();

        private static string SDE_FILE_PATH = ConfigurationManager.AppSettings["SDE_FILE_PATH"]; //"Database Connections\\Connection to EDGISQ3Q@lbgis.sde";
        private static string LOTLINES_ONE_TIME_VERSION = ConfigurationManager.AppSettings["LOTLINES_ONE_TIME_VERSION"];
        private static string LOG_FILE_PATH = (String.IsNullOrEmpty(ConfigurationManager.AppSettings["LOG_FILE_PATH"]) ? System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) : ConfigurationManager.AppSettings["LOG_FILE_PATH"]) + "\\Logfile_" + DateTime.Now.Ticks.ToString() + ".log";
        private static string GDB_LOTLINE_DISSOLVED_FC_NAME = ConfigurationManager.AppSettings["GDB_LOTLINE_DISSOLVED_FC_NAME"];
        private static string curr_loc = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        private static string LOTLINES_GDB_FILE_NAME = "LOTLINES_ONE_TIME_GDB";
        private static string GDB_FILE_PATH = curr_loc + "\\" + LOTLINES_GDB_FILE_NAME + ".gdb";
        private static string GDB_LOTLINE_DISSOLVED_FC = ConfigurationManager.AppSettings["GDB_LOTLINE_DISSOLVED_FC"];

        #endregion
        /// <summary>
        /// main method
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            string arguments = string.Empty;
            string pyLogFile = string.Empty;
            try
            {
                //create log file
                createLogFile();
                WriteLine("START");
                System.Diagnostics.Process proc = new System.Diagnostics.Process();

                //WriteLine("Initializing License");
                //m_AOLicenseInitializer.InitializeApplication(new esriLicenseProductCode[] { esriLicenseProductCode.esriLicenseProductCodeBasic, esriLicenseProductCode.esriLicenseProductCodeStandard, esriLicenseProductCode.esriLicenseProductCodeAdvanced },
                //    new esriLicenseExtensionCode[] { });
                //mmLicenseStatus licenseStatus = CheckOutLicenses(mmLicensedProductCode.mmLPArcFM);
                //if (RuntimeManager.ActiveRuntime == null)
                //    RuntimeManager.BindLicense(ProductCode.Desktop);
                //WriteLine("Getting workspace from SDE connection file. ");
                //workspace = ArcSdeWorkspaceFromFile(SDE_FILE_PATH);
               
                //IVersionedWorkspace pVersionedWorkspace = workspace as IVersionedWorkspace;
                //IVersion pDefaultVersion = pVersionedWorkspace.DefaultVersion;
                //IVersion pRowOlderVersion = null;
                //if (ConfigurationManager.AppSettings["CREATE_OPEN_VERSION"].ToString().ToUpper() == "TRUE")
                //{
                //    try
                //    {
                //        pRowOlderVersion = pVersionedWorkspace.FindVersion(LOTLINES_ONE_TIME_VERSION);
                //        WriteLine("Existing Version found: " + LOTLINES_ONE_TIME_VERSION);
                //        pRowOlderVersion.Delete();
                //        WriteLine("Version deleted ");
                //        VersionCreate(pVersionedWorkspace, pDefaultVersion, LOTLINES_ONE_TIME_VERSION);
                //        WriteLine("Version created: " + LOTLINES_ONE_TIME_VERSION + " as child of Default.");
                //    }
                //    catch (Exception ex)
                //    {
                //        if (ex.Message.Contains("Version not found"))
                //        {
                //            WriteLine("Assuming no previous ROW version exists, Creating a new version from default version");
                //            VersionCreate(pVersionedWorkspace, pDefaultVersion, LOTLINES_ONE_TIME_VERSION);
                //            WriteLine("Version created: " + LOTLINES_ONE_TIME_VERSION + " as child of Default.");
                //        }
                //    }
                //}
                //else
                //{
                //    WriteLine("Assuming SDE. " + LOTLINES_ONE_TIME_VERSION + " already exists.");
                //    WriteLine("Proceeding further");
                //}

                //if (ConfigurationManager.AppSettings["COPY_LOTLINE_FC_TO_GDB"].ToString().ToUpper() == "TRUE")
                //{
                //    WriteLine("Processing CopyLotLineFcToGdb.py");
                //    pyLogFile = curr_loc + "\\Log_CopyLotLineFcToGdb.txt";
                //    proc.StartInfo.FileName = curr_loc + "\\" + "CopyLotLineFcToGdb.py";
                //    proc.StartInfo.UseShellExecute = true;
                //    arguments = SDE_FILE_PATH + " " + curr_loc + " " + LOTLINES_GDB_FILE_NAME + " " + pyLogFile;
                //    proc.StartInfo.Arguments = arguments;
                //    proc.Start();
                //    proc.WaitForExit();
                //    WriteLine("Completed");
                //}
                //else
                //{
                //    WriteLine("Assuming lotline feature class has already been copied to " + GDB_FILE_PATH);
                //    WriteLine("Proceeding further");
                //}

                //if (ConfigurationManager.AppSettings["RUN_LOTLINE_DISSOLVE"].ToString().ToUpper() == "TRUE")
                //{
                //    WriteLine("Processing RunLotLineDissolve.py");
                //    pyLogFile = curr_loc + "\\Log_RunLotLineDissolve.txt";
                //    proc.StartInfo.FileName = curr_loc + "\\" + "RunLotLineDissolve.py";
                //    proc.StartInfo.UseShellExecute = true;
                //    arguments = SDE_FILE_PATH + " " + GDB_FILE_PATH + " " + pyLogFile + " " + LOTLINES_ONE_TIME_VERSION + " " + GDB_LOTLINE_DISSOLVED_FC;
                //    proc.StartInfo.Arguments = arguments;
                //    proc.Start();
                //    proc.WaitForExit();
                //    WriteLine("Completed");
                //}
                //else
                //{
                //    WriteLine("Assuming Lotline has already been dissolved and the dissolved feature class is saved at" + GDB_LOTLINE_DISSOLVED_FC);
                //    WriteLine("Proceeding further");
                //}

                if (ConfigurationManager.AppSettings["MOVE_LOTLINES_TO_VERSION"].ToString().ToUpper() == "TRUE")
                {
                    WriteLine("Processing MoveLotLinesToVersion.py");
                    pyLogFile = curr_loc + "\\Log_MoveLotLinesToVersion.txt";
                    proc.StartInfo.FileName = curr_loc + "\\" + "MoveLotLinesToVersion.py";
                    proc.StartInfo.UseShellExecute = true;
                    arguments = SDE_FILE_PATH + " " + GDB_FILE_PATH + " " + pyLogFile + " " + LOTLINES_ONE_TIME_VERSION + " " + GDB_LOTLINE_DISSOLVED_FC;
                    proc.StartInfo.Arguments = arguments;
                    proc.Start();
                    proc.WaitForExit();
                    WriteLine("Completed");

                }
                else
                {
                    WriteLine("MOVE_LOTLINES_TO_VERSION is set as false");
                    // WriteLine("Proceeding further");
                }
                WriteLine("Execution completed.");
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
        //public static Boolean Reconcileandpostchanges(IVersion EditVersion)
        //{
        //    IWorkspaceEdit pwEdit = null;
        //    pwEdit = EditVersion as IWorkspaceEdit;
        //    if (pwEdit.IsBeingEdited())
        //    {
        //        pwEdit.StopEditing(false);
        //    }
        //    pwEdit.StartEditing(true);
        //    try
        //    {
        //        // Reconcile and post version here
        //        IVersion pChildVersion = EditVersion as IVersion;
        //        IVersionEdit4 pVersionEdit = EditVersion as IVersionEdit4;
        //        // Acquire locks first, if lock not acquired, then wait in loop, try 10-15 times
        //        bool IsVersionPosted = false;
        //        int iRetryCount = 0;
        //        while (iRetryCount <= Convert.ToInt32(10))
        //        {
        //            if (!pVersionEdit.Reconcile4(pChildVersion.VersionInfo.Parent.VersionName, true, false, false, true))
        //            {
        //                if (pVersionEdit.CanPost())
        //                {
        //                    pVersionEdit.Post(pChildVersion.VersionInfo.Parent.VersionName);
        //                    IsVersionPosted = true;
        //                    break;
        //                }
        //            }
        //            else
        //            {
        //                iRetryCount++;
        //                WriteLine("Version reconcile retry count " + iRetryCount.ToString());

        //            }
        //        }

        //        if (iRetryCount > Convert.ToInt32(10))
        //        {
        //            WriteLine("Could not reconcile/post version - " + pChildVersion.VersionName);
        //            return false;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        if (pwEdit.IsBeingEdited())
        //        {
        //            pwEdit.StopEditing(false);
        //        }
        //        WriteLine("Could not reconcile/post version - " + ex.Message);
        //        return false;
        //    }
        //    finally
        //    {
        //        if (pwEdit != null)
        //        {
        //            if (pwEdit.IsBeingEdited())
        //            {
        //                pwEdit.StopEditing(true);
        //            }
        //        }
        //    }
        //    return true;
        //}

        private static IWorkspace OpenSDEWorkspacefromsdefile(string sdefilenamewithpath)
        {
            IWorkspace workspace = null;
            try
            {
                // Create an SDE workspace factory and open the workspace.
                Type factoryType = Type.GetTypeFromProgID("esriDataSourcesGDB.SdeWorkspaceFactory");
                IWorkspaceFactory workspaceFactory = (IWorkspaceFactory)Activator.CreateInstance(factoryType);
                workspace = workspaceFactory.OpenFromFile(sdefilenamewithpath, 0);
            }
            catch (Exception ex)
            {
                WriteLine(ex.Message);
            }
            return workspace;

        }
        public static void VersionCreate(IVersionedWorkspace versionedWorkspace, IVersion parentVersion, string newversionName)
        {

            IVersion version;
            version = parentVersion.CreateVersion(newversionName);
            //setting the versions access    
            version.Access = esriVersionAccess.esriVersionAccessPublic;
            //setting the versiones description
            version.Description = "For Lotline and Row Daily Update process";
        }
        /// <summary>
        /// create log file 
        /// </summary>
        public static void createLogFile()
        {
            (pSWriter = File.CreateText(LOG_FILE_PATH)).Close();
        }
        /// <summary>
        /// Write on console and log file
        /// </summary>
        /// <param name="sMsg"></param>
        private static void WriteLine(string sMsg)
        {
            sMsg = !String.IsNullOrEmpty(sMsg) ? DateTime.Now.ToLongTimeString() + " -- " + sMsg : sMsg;
            pSWriter = File.AppendText(LOG_FILE_PATH);
            pSWriter.WriteLine(sMsg);
            Console.WriteLine(sMsg);
            pSWriter.Close();
        }
        /// <summary>
        /// Initialize licence
        /// </summary>
        //private static void initializeLicence()
        //{
        //    //ESRI License Initializer generated code.
        //    WriteLine("Initializing Licence");
        //    m_AOLicenseInitializer.InitializeApplication(new esriLicenseProductCode[] { esriLicenseProductCode.esriLicenseProductCodeBasic, esriLicenseProductCode.esriLicenseProductCodeStandard, esriLicenseProductCode.esriLicenseProductCodeAdvanced },
        //    new esriLicenseExtensionCode[] { });
        //    mmLicenseStatus licenseStatus = CheckOutLicenses(mmLicensedProductCode.mmLPArcFM);
        //    if (RuntimeManager.ActiveRuntime == null)
        //        RuntimeManager.BindLicense(ProductCode.Desktop);

        //}
        /// <summary>
        /// get workspace from connection file
        /// </summary>
        /// <param name="connectionFile"></param>
        /// <returns></returns>
        private static IWorkspace ArcSdeWorkspaceFromFile(String connectionFile)
        {
            return ((IWorkspaceFactory)Activator.CreateInstance(Type.GetTypeFromProgID("esriDataSourcesGDB.SdeWorkspaceFactory"))).
                OpenFromFile(connectionFile, 0);
        }
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

    }

}
