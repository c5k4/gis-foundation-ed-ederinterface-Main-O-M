//using Miner.Geodatabase.GeodatabaseManager;
//using Miner.Geodatabase.GeodatabaseManager.Serialization;
//using Miner.Process.GeodatabaseManager.ActionHandlers;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geodatabase;
using System;
using System.Configuration;
using System.Diagnostics;
//using Miner.Geodatabase.GeodatabaseManager.ActionHandlers;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using PGE_DBPasswordManagement;
//V3SF - CD API (EDGISREARC-1452) - Added
using PGE.BatchApplication.IntExecutionSummary;
using System.Globalization;
using System.Linq;
using System.Collections.Generic;

namespace PGE.Interfaces.SAP.ED06.Batch
{
    public class Program
    {
        #region private varriables
        private static  PGE.Common.Delivery.Diagnostics.Log4NetLogger Log;
        private static LicenseInitializer m_AOLicenseInitializer;
        private static bool successfulRun = false;
        public static string comment = default;
        //V3SF - CD API (EDGISREARC-1452) - Added [START]
        public static DateTime lastRunDate;
        public static DateTime CDRunDate;
        private static DateTime startTimeDT;
        //V3SF - CD API (EDGISREARC-1452) - Added [END]
        private static string startTime;
        private static StringBuilder remark = default;
        private static StringBuilder Argumnet = default;
        // M4JF edgisrearch 919 - get sde file using PGE_DBPasswordManagement
        private static string sdeConnection = ReadEncryption.GetSDEPath(ConfigurationManager.AppSettings["EDER_SDEConnection"].ToUpper());

        //private static Logger Log = new Logger();
        #endregion

        #region Program Main
        public static void Main(string[] args)
        {
            bool updateSummary = true;
            try
            {
                Console.WriteLine("Going to read log4net");
                Log = new PGE.Common.Delivery.Diagnostics.Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "PGE.Interfaces.SAP.ED06.Batch.log4net.config");
                Console.WriteLine("log4net found");
                m_AOLicenseInitializer = new LicenseInitializer();
                Console.WriteLine("LicenseInitializer created");
                Console.WriteLine(MethodBase.GetCurrentMethod().DeclaringType);//ED06_ChangeDetection.log4net.config
                //Console.WriteLine(Log.)
                //V3SF - CD API (EDGISREARC-1452) - Added
                startTimeDT = DateTime.Now;
                startTime = startTimeDT.ToString("MM/dd/yyyy HH:mm:ss");
                Console.WriteLine(startTime);

                Log.Info("\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n" +
                    "* ********-------------------------------------------------------------------------------------------------*********\n" +
                    "--ED06 Execution["+ startTime + "]\n" +
                    "* ********-------------------------------------------------------------------------------------------------*********\n");

                Log.Error("\n" +
                    "* ********-------------------------------------------------------------------------------------------------*********\n" +
                    "--ED06 Execution[" + startTime + "]\n" +
                    "* ********-------------------------------------------------------------------------------------------------*********\n");

                //ESRI License Initializer generated code.
                if (!m_AOLicenseInitializer.InitializeApplication(new esriLicenseProductCode[] { esriLicenseProductCode.esriLicenseProductCodeAdvanced },
                new esriLicenseExtensionCode[] { }))
                {
                    Console.WriteLine("License not found");
                    Log.Info(m_AOLicenseInitializer.LicenseMessage());
                    Log.Info("License not found");
                    comment = "License not found";
                    //V3SF - CD API (EDGISREARC-1452) - Commented                    
                    //ExecutionSummary(startTime, comment, Argument.Error);
                    //V3SF - CD API (EDGISREARC-1452) - Added
                    PGE.BatchApplication.IntExecutionSummary.ExecutionSummary.PopulateIntExecutionSummary(Argument.Interface, interface_type.Outbound, Argument.Integration, startTimeDT, DateTime.Now, status.E, comment);
                }
                else
                {
                    Console.WriteLine("License found");
                    Log.Info("License Occupied");
                    Log.Info("Process Started"); //Console.WriteLine("Process Started");
                    Console.WriteLine("entering to main process");
                    //V3SF - CD API (EDGISREARC-1452) - Added
                    lastRunDate = PGE.BatchApplication.IntExecutionSummary.ExecutionSummary.LastProcessExecutionDate<DateTime>(Argument.Interface);

                    try
                    {
                        updateSummary = Convert.ToBoolean(ConfigurationManager.AppSettings["updateINT_Summ"]);
                    }
                    catch { }

                    //lastRunDate = new DateTime(2021, 12, 15);
                    MainProcess();
                    Console.WriteLine(" main process done");
                    Log.Info("Process Completed");
                    //V3SF - CD API (EDGISREARC-1452) - Commented
                    //ExecutionSummary(startTime, comment, Argument.Done);
                    //V3SF - CD API (EDGISREARC-1452) - Added
                    if (updateSummary)
                        PGE.BatchApplication.IntExecutionSummary.ExecutionSummary.PopulateIntExecutionSummary(Argument.Interface, interface_type.Outbound, Argument.Integration, startTimeDT, DateTime.Now, status.D, comment, CDRunDate);
                    //Fix: 03-Nov-2020: Closing the console window after program completion.
                    Environment.Exit(0);
                }


            }
            catch (Exception ex)
            {
                Console.WriteLine("Main method " + MethodInfo.GetCurrentMethod().Name);
                Console.WriteLine(ex.Message,ex.StackTrace);
                Log.Info("Error Occurred : ", ex);
                //V3SF - CD API (EDGISREARC-1452) - Commented
                //ExecutionSummary(startTime, "ED06 Exception :" + ex.Message, Argument.Error);
                //V3SF - CD API (EDGISREARC-1452) - Added
                if (updateSummary)
                    PGE.BatchApplication.IntExecutionSummary.ExecutionSummary.PopulateIntExecutionSummary(Argument.Interface, interface_type.Outbound, Argument.Integration, startTimeDT, DateTime.Now, status.E, "ED06 Exception :" + ex.Message);

                ErrorCodeException ece = new ErrorCodeException(ex);
                Environment.ExitCode = ece.CodeNumber;
            }
        }
        #endregion

        #region Main Process
        public static void MainProcess()
        {
            try
            {

                //getting WorkSpace

                // m4jf edgisrearch 919 get sde connection using PGE_DBPasswordmanagement
                //IWorkspace pWorkspace = ArcSdeWorkspaceFromFile(ConfigurationManager.AppSettings["SDE_Connection_File"].ToString());
                //sdeConnection = @"C:\Users\V3SF\AppData\Local\Temp\1\GIS_I_EDER_DV_5_18_2022_13_31_52_100.sde";
                IWorkspace pWorkspace = ArcSdeWorkspaceFromFile(sdeConnection);
                Console.WriteLine("workspace found");
                //Log.Info("WorkSpace found from SDE connection [" + ConfigurationManager.AppSettings["SDE_Connection_File"].ToString() + "]");
                // Edit Version WorkSpace

                //V3SF - CD API (EDGISREARC-1452) - Commented [START]
                //IVersion editVersion = ((IVersionedWorkspace)pWorkspace).FindVersion(System.Configuration.ConfigurationManager.AppSettings["Edit_Version"]);
                //Console.WriteLine("Edit version found");
                //IVersionedWorkspace versionedWorkspace = (IVersionedWorkspace)editVersion;
                //Console.WriteLine("Edit version workspace found");
                //// Target Version Workspace
                //bool createVersion = false;
                //IVersion targetVersion = null;
                //try
                //{
                //    targetVersion = versionedWorkspace.FindVersion(System.Configuration.ConfigurationManager.AppSettings["Target_Version"]);
                //    Console.WriteLine("target version workspace found");
                //}
                //catch (Exception ex)
                //{
                //    Console.WriteLine("target version workspace execption");
                //    createVersion = true;
                //    Log.Info("Change detection Version Not found [" + System.Configuration.ConfigurationManager.AppSettings["Target_Version"] + "]");
                //}
                //V3SF - CD API (EDGISREARC-1452) - Commented [END]

                //if Target version does not exist the create new one with same name
                //if (createVersion)
                //{
                //    Log.Info("Creating Change detection Version [" + System.Configuration.ConfigurationManager.AppSettings["Target_Version"] + "]");
                //    targetVersion = ((IVersionedWorkspace)pWorkspace).DefaultVersion.CreateVersion(System.Configuration.ConfigurationManager.AppSettings["Target_Version"]);
                //    targetVersion.Access = esriVersionAccess.esriVersionAccessPublic;

                //}

                //Calling Method to initiate Change detction

                //V3SF - CD API (EDGISREARC-1452) - Commented
                //Log.Info("Change Detection initialization started between versions - [" + editVersion.VersionName.ToString() + "] and [" + targetVersion.VersionName.ToString() + "] ");

                //V3SF - CD API (EDGISREARC-1452) - Added
                Log.Info("Change Detection initialization started ");

                Console.WriteLine("calling integrator consurtuctor");

                //V3SF - CD API (EDGISREARC-1452) - Commented
                //GISSAPIntegrator integrator = new GISSAPIntegrator(editVersion, targetVersion, null);

                //V3SF - CD API (EDGISREARC-1452) - Added
                GISSAPIntegrator integrator = new GISSAPIntegrator(((IVersionedWorkspace)pWorkspace).DefaultVersion, ((IVersionedWorkspace)pWorkspace).DefaultVersion, null);
                Console.WriteLine(" integrator consurtuctor end");
                bool initSucceessful = integrator.Initialize();
                if (initSucceessful == false)
                {
                    //V3SF - CD API (EDGISREARC-1452) - Commented [START]
                    //Log.Info("Failed to initialize change detection process using GISSAPIntegrator for versions [" + editVersion.VersionName.ToString() + "] and [" + targetVersion.VersionName.ToString() + "]. ");
                    //comment = "Failed to initialize change detection process using GISSAPIntegrator for versions [" + editVersion.VersionName.ToString() + "] and [" + targetVersion.VersionName.ToString() + "]. ";
                    //V3SF - CD API (EDGISREARC-1452) - Commented [END]

                    //V3SF - CD API (EDGISREARC-1452) - Added [START]
                    Log.Info("Failed to initialize change detection process using GISSAPIntegrator. ");
                    comment = "Failed to initialize change detection process using GISSAPIntegrator. ";
                    //V3SF - CD API (EDGISREARC-1452) - Added [END]
                }
                else
                {

                    Log.Info("Initialization successful and change detection process started using GISSAPInegrator.....");
                    //V3SF - CD API (EDGISREARC-1452) - Added [START]

                    GISSAPIntegrator.lastRunDate = lastRunDate;
                    GISSAPIntegrator.EDER_Credential = ConfigurationManager.AppSettings["EDER_SDEConnection"].ToUpper();

                    GISSAPIntegrator.GDBM_CD_Table = Convert.ToString(ConfigurationManager.AppSettings["GDBM_CD_Table"]);
                    try
                    {
                        GISSAPIntegrator.Bkp_Schema_Credentials = Convert.ToString(ConfigurationManager.AppSettings["Bkp_Schema_Credentials"]);
                    }
                    catch
                    {
                        GISSAPIntegrator.Bkp_Schema_Credentials = "";
                    }
                    try
                    {
                        GISSAPIntegrator.CDWhereClause = Convert.ToString(ConfigurationManager.AppSettings["CDWhereClause"]);
                    }
                    catch
                    {
                        GISSAPIntegrator.CDWhereClause = "";
                    }

                    try
                    {
                        GISSAPIntegrator.ResetSequence = Convert.ToBoolean(ConfigurationManager.AppSettings["ResetSequence"]);
                    }
                    catch
                    {
                        GISSAPIntegrator.ResetSequence = true;
                    }

                    try
                    {
                        string RetryCount = ConfigurationManager.AppSettings["RetryCount"];
                        if (!string.IsNullOrWhiteSpace(RetryCount))
                        {
                            GISSAPIntegrator.RetryCount = Convert.ToInt32(RetryCount);
                        }
                    }
                    catch { }

                    try
                    {
                        string TimeDelay = ConfigurationManager.AppSettings["TimeDelay"];
                        if (!string.IsNullOrWhiteSpace(TimeDelay))
                        {
                            GISSAPIntegrator.TimeDelay = Convert.ToInt32(TimeDelay);
                        }
                    }
                    catch { }

                    try
                    {
                        string startDate = ConfigurationManager.AppSettings["START_Date"];
                        if (!string.IsNullOrWhiteSpace(startDate))
                        {
                            GISSAPIntegrator.lastRunDate = DateTime.ParseExact(startDate, "yyyy-MM-dd HH:mm:ss", CultureInfo.CurrentCulture);
                            GISSAPIntegrator.dateUpdate = true;
                        }
                    }
                    catch { }

                    try
                    {
                        string endDate = ConfigurationManager.AppSettings["END_Date"];
                        if (!string.IsNullOrWhiteSpace(endDate))
                            GISSAPIntegrator.endDate = DateTime.ParseExact(endDate, "yyyy-MM-dd HH:mm:ss", CultureInfo.CurrentCulture);
                    }
                    catch { }

                    try
                    {
                        string RecordCount = ConfigurationManager.AppSettings["RecordCount"];
                        if (!string.IsNullOrWhiteSpace(RecordCount))
                        {
                            GISSAPIntegrator.RecordCount = Convert.ToInt32(RecordCount);
                        }
                    }
                    catch { }

                    try
                    {
                        GISSAPIntegrator.HandleOracleErrorCodes = Convert.ToBoolean(ConfigurationManager.AppSettings["HandleAllOracleErrorCodes"]);
                    }
                    catch
                    {
                        GISSAPIntegrator.HandleOracleErrorCodes = false;
                    }
                    try
                    {
                        GISSAPIntegrator.OracleErrorCodes = Convert.ToString(ConfigurationManager.AppSettings["OracleErrorCodes"]).Split(',').ToList();
                    }
                    catch
                    {
                        GISSAPIntegrator.OracleErrorCodes = new List<string>();
                    }


                    if (!string.IsNullOrWhiteSpace((GISSAPIntegrator.Bkp_Schema_Credentials)))
                        GISSAPIntegrator.Bkp_Schema_Credentials = ReadEncryption.GetConnectionStr(GISSAPIntegrator.Bkp_Schema_Credentials.ToUpper());
                    //V3SF - CD API (EDGISREARC-1452) - Added [END]
                    successfulRun = integrator.Process();
                    Log.Info("change detection process completed using GISSAPInegrator.....");
                    //Re-creating Target version
                    if (successfulRun)
                    {
                        //Marshal.ReleaseComObject(targetVersion);
                        if (pWorkspace != null) { Marshal.ReleaseComObject(pWorkspace); }
                        //V3SF - CD API (EDGISREARC-1452) - Commented [START]
                        //if (targetVersion != null) { Marshal.ReleaseComObject(targetVersion); }
                        //if (editVersion != null) { Marshal.ReleaseComObject(editVersion); }
                        //V3SF - CD API (EDGISREARC-1452) - Commented [END]
                        integrator = null;
                        //V3SF - CD API (EDGISREARC-1452) - Commented [START]
                        //Log.Info("Re-Creating change detection version.....");
                        //reCreateVersion(pWorkspace);
                        //V3SF - CD API (EDGISREARC-1452) - Commented [END]
                        comment = "All process completed sucessfully "+ GISSAPIntegrator.remark;
                        //V3SF - CD API (EDGISREARC-1452) - Added
                        CDRunDate = GISSAPIntegrator.CDRunDate;
                    }
                    else
                    {
                        comment = GISSAPIntegrator.remark;
                        throw new Exception(comment);
                    }

                    //Export CSV method call
                    //if (successfulRun)
                    //{
                    //    Log.Info("CSV export process started.....");

                    //    string[] args = System.Configuration.ConfigurationManager.AppSettings["CSVExportBatchArguments"].Split(',');
                    //    //Export CSV method call
                    //    PGE.Interfaces.SAP.Batch.Program exportCsvObj = new PGE.Interfaces.SAP.Batch.Program(args);

                    //    Log.Info("CSV export process completed.....");
                    //}
                }
            }
            catch (Exception ex)
            {
                Log.Info("Error Occurred : " + ex.ToString());
                Console.WriteLine("Error Occurred : " + ex.ToString());
                throw ex;
            }
            finally
            {
                if (m_AOLicenseInitializer != null) m_AOLicenseInitializer.ShutdownApplication();
                m_AOLicenseInitializer = null;
                GC.Collect();
            }

        }



        #endregion

        #region Get ArcSDE Connection
        private static IWorkspace ArcSdeWorkspaceFromFile(String connectionFile)
        {
            return ((IWorkspaceFactory)Activator.CreateInstance(Type.GetTypeFromProgID("esriDataSourcesGDB.SdeWorkspaceFactory"))).
                OpenFromFile(connectionFile, 0);
        }
        #endregion

        #region Re-Create Version
        public static void reCreateVersion(IWorkspace pWorkspaceT)
        {
            bool versionDeleted = true;
            IVersion2 versionToDelete = null;
            string versionName = System.Configuration.ConfigurationManager.AppSettings["Target_Version"];
            //getting WorkSpace
            // m4jf edgisrearch 919
            // IWorkspace pWorkspaceCon = ArcSdeWorkspaceFromFile(ConfigurationManager.AppSettings["SDE_Connection_File"].ToString());
            IWorkspace pWorkspaceCon = ArcSdeWorkspaceFromFile(sdeConnection);

            //Log.Info("WorkSpace found from SDE connection [" + ConfigurationManager.AppSettings["SDE_Connection_File"].ToString() + "]");

            try
            {
                Log.Info("Deleting Version [" + versionName + " ]");

                versionToDelete = ((IVersionedWorkspace)pWorkspaceCon).FindVersion(versionName) as IVersion2;
                versionToDelete.Delete();
            }
            catch (Exception ex)
            {
                Log.Info("Couldn't delete change detection version [" + versionToDelete.VersionName + " ] [ " + ex.ToString() + " ]");
                versionDeleted = false;
                bool reconcileResult = false;
                IVersion parentVersion = ((IVersionedWorkspace)pWorkspaceCon).FindVersion(System.Configuration.ConfigurationManager.AppSettings["Edit_Version"]);
                Log.Info("Reconciling Change detction version  with Default");
                while (reconcileResult == false)
                {
                    reconcileResult = Reconcile(versionToDelete, parentVersion);    //Reconcile(childVersion, parentVesion)
                    Log.Info("Change detction version " + versionToDelete.VersionName + " is Reconciled with Default version " + parentVersion.VersionName);

                }

            }

            if (versionDeleted)
            {
                try
                {
                    IVersion version = ((IVersionedWorkspace)pWorkspaceCon).DefaultVersion.CreateVersion(versionName);
                    version.Access = esriVersionAccess.esriVersionAccessPublic;
                    Log.Info("Change detection version Re-Created : [" + version.VersionName + " ]");
                    //GC.Collect();

                }
                catch (Exception ex)
                {
                    Log.Info("Couldn't re-create change detection version [" + versionName + " ] [ " + ex.ToString() + " ]");
                    comment = "Couldn't re-create change detection version [" + versionName + " ] [ " + ex.ToString() + " ]";

                }
            }
            if (pWorkspaceCon != null)
            {
                Marshal.ReleaseComObject(pWorkspaceCon);
            }
        }

        #endregion

        /// <summary>
        /// reconciles edit version with target version
        /// </summary>
        /// <param name="editVersion"></param>
        /// <param name="targetVersion"></param>
        /// <returns></returns>
        public static Boolean Reconcile(IVersion editVersion, IVersion targetVersion)
        {
            IMultiuserWorkspaceEdit muWorkspaceEdit = (IMultiuserWorkspaceEdit)editVersion;
            IWorkspaceEdit workspaceEdit = (IWorkspaceEdit2)editVersion;
            IVersionEdit4 versionEdit = (IVersionEdit4)workspaceEdit;

            if (muWorkspaceEdit.SupportsMultiuserEditSessionMode
                (esriMultiuserEditSessionMode.esriMESMVersioned))
            {
                //Reconcile with the target version. 
                bool conflicts = true;
                while (conflicts)
                {
                    try
                    {
                        muWorkspaceEdit.StartMultiuserEditing(esriMultiuserEditSessionMode.esriMESMVersioned);
                        conflicts = versionEdit.Reconcile4(targetVersion.VersionName, false, false, false, true);
                    }
                    catch (System.Runtime.InteropServices.COMException excom)
                    {
                        if (excom.ErrorCode == -2147217137) //ESRI.ArcGIS.Geodatabase.fdoError.FDO_E_RECONCILE_VERSION_NOT_AVAILABLE 
                        {
                            Log.Info("The target version  " + targetVersion.VersionName + "  is currently being reconciled against some other version.");
                        }
                        else if (excom.ErrorCode == -2147217139) //FDO_E_VERSION_BEING_EDITED 
                        {
                            Log.Info("Reconcile not allowed as the version is being edited by another application.");
                        }
                        else if (excom.ErrorCode == -2147217146)//FDO_E_VERSION_NOT_FOUND 
                        {
                            Log.Info("reconcile failed.The version " + targetVersion.VersionName + " could not be located");
                        }

                        if (workspaceEdit.IsBeingEdited())
                            workspaceEdit.StopEditing(false);
                        conflicts = true;
                    }
                }
                if (workspaceEdit.IsBeingEdited())
                    workspaceEdit.StopEditing(true);
                return true;
            }
            return false;
        }

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

                Argumnet = new StringBuilder();
                remark = new StringBuilder();
                //Setting arguments
                Argumnet.Append(Argument.Interface);
                Argumnet.Append(Argument.Type);
                Argumnet.Append(Argument.Integration);
                Argumnet.Append(startTime + ";");
                remark.Append(comment);
                Argumnet.Append(DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss") + ";");
                Argumnet.Append(status);
                Argumnet.Append(remark);

                //To execution for interface execution summary exe.This exe must need some input argument to run successffully. 
                ProcessStartInfo processStartInfo = new ProcessStartInfo(ConfigurationManager.AppSettings["IntExecutionSummaryExePath"].ToString(), "\"" + Convert.ToString(Argumnet) + "\"");
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
                Log.Error("Exception occures while executing the Interface Summary exe" + ex.Message);
                Log.Info(ex.Message + "   " + ex.StackTrace);
                Console.WriteLine("Exception occures while executing the Interface Summary exe", ex.StackTrace);
            }
        }

    }

    /// <summary>
    /// This class is for interface Summary Execution
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
        public static string Interface { get; } = "ED06";
        /// <summary>
        /// This property to get name of the system to integration
        /// </summary>
        public static string Integration { get; } = "SAP;";
        /// <summary>
        /// This property to get type of interface
        /// </summary>
        public static string Type { get; } = "Outbound;";
    }
}


