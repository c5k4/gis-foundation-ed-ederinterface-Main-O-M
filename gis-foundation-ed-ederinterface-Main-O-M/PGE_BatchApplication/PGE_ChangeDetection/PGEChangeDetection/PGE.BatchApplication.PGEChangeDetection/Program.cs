using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.GeoDatabaseDistributed;
using PGE.BatchApplication.ChangeDetection.ETL;
using PGE.Common.ChangesManager;
using PGE.BatchApplication.ChangeDetection;
using PGE.BatchApplication.ChangeDetection.Exceptions;
using PGE.Common.ChangesManagerShared;
using PGE.Common.ChangesManagerShared.Utilities;
using PGE.Common.ChangesManagerShared.Interfaces;
using Castle.Windsor.Configuration.Interpreters;
using Castle.Core;
using Castle.Windsor;

using System.Reflection;
using System.IO;
using PGE.Common.Delivery.Diagnostics;
using CommandFlags;
using PGE.Common.ChangeDetectionAPI;
using PGE.BatchApplication.IntExecutionSummary;

namespace PGE.BatchApplication.ChangeDetection
{
    /// <summary>
    /// Entry point for Change Detection execution.
    /// </summary>
    class Program
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool TerminateProcess(IntPtr hProcess, uint uExitCode);


        #region Private Static Members

        private static LicenseInitializer m_AOLicenseInitializer = new PGE.BatchApplication.ChangeDetection.LicenseInitializer();
        private static Log4NetLogger _logger;
//        private static Log4NetLogger _logger = new Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private static IWindsorContainer _container;

        public const string LogFileDirectory = "Change Detection Log Files";
        private const string logFileName = "ChangeDetection";
        //V3SF - CD API (EDGISREARC-1452) - Added [START]
        private static string Interface_Name = "Change Detection";
        public static DateTime toDate;
        //V3SF - CD API (EDGISREARC-1452) - Added [END]
        #endregion

        #region Main Execution Methods

        static void Main(string[] args)
        {
            SetLogger(args);
            _logger.Debug(MethodBase.GetCurrentMethod().Name);
            OutputVersion();
            Console.Title = "Change Detection";
            DateTime exeStart = DateTime.Now;
            //V3SF - CD API (EDGISREARC-1452) - Added
            status stat = status.E;
            try
            {
                //Report start time.
                _logger.Info("Process started: " + exeStart.ToLongDateString() + " " + exeStart.ToLongTimeString());
                _logger.Info("");

                //ESRI License Initializer generated code.
                if (!m_AOLicenseInitializer.InitializeApplication(new esriLicenseProductCode[] { esriLicenseProductCode.esriLicenseProductCodeAdvanced },
                new esriLicenseExtensionCode[] { }))
                {
                    _logger.Info(m_AOLicenseInitializer.LicenseMessage());
                    throw new ErrorCodeException(ErrorCode.LicenseCheckout, "This application could not initialize with the correct ArcGIS license and will shutdown.");
                }
                else
                {
                    ProcessArgs(args);
                }
                //V3SF - CD API (EDGISREARC-1452) - Added [START]
                stat = status.D;

                if(toDate == DateTime.MinValue)
                ExecutionSummary.PopulateIntExecutionSummary(Interface_Name, interface_type.Internal, "GIS", exeStart, DateTime.Now, stat, "Executed Successfully");
                else 
                ExecutionSummary.PopulateIntExecutionSummary(Interface_Name, interface_type.Internal, "GIS", exeStart, DateTime.Now, stat, "Executed Successfully", toDate);
                //V3SF - CD API (EDGISREARC-1452) - Added [END]
            }
            catch (ErrorCodeException ece)
            {
                WriteError(ece);
                //V3SF - CD API (EDGISREARC-1452) - Added
                ExecutionSummary.PopulateIntExecutionSummary(Interface_Name, interface_type.Internal, "GIS", exeStart, DateTime.Now, stat, "Error :: "+ece);
                Environment.ExitCode = ece.CodeNumber;
            }
            catch (Exception ex)
            {
                //Format this error as an ErrorCodeException and throw it the same way (becomes a general error).
                ErrorCodeException ece = new ErrorCodeException(ex);
                WriteError(ece);
                //V3SF - CD API (EDGISREARC-1452) - Added
                ExecutionSummary.PopulateIntExecutionSummary(Interface_Name, interface_type.Internal, "GIS", exeStart, DateTime.Now, stat, "Error :: " +ex.Message+" at "+ex.StackTrace+ " ErrorCodeException :: "+ ece);
                Environment.ExitCode = ece.CodeNumber;
            }
            finally
            { 
                WriteEndStatus(exeStart);
                if (m_AOLicenseInitializer != null) m_AOLicenseInitializer.ShutdownApplication();
                m_AOLicenseInitializer = null;
                _logger = null;
                if (_container != null) _container.Dispose();
                _container = null;
                GC.Collect();
                GC.WaitForPendingFinalizers();
                // Rolling Versions appears to occasionally lock the process, leaving it unable to exit
                // The exit code should have been set so as the last line in the program this should be safe to execute
                if (Debugger.IsAttached)
                {
                    Console.WriteLine("Press return to exit");
                    Console.ReadLine();
                }
                TerminateProcess(Process.GetCurrentProcess().Handle, Convert.ToUInt32(Environment.ExitCode));
            }
        }

        public static void OutputVersion()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            FileVersionInfo fileVersionInfo = FileVersionInfo.GetVersionInfo(assembly.Location);
            string version = fileVersionInfo.ProductVersion;            
            _logger.Info("Version [ " + version + " ]");
        }

        protected static void SetLogger(string[] args)
        {
            string configFile = "";
            var flags = new FlagParser()
                {
                    { "c", "config", true, false, "The config file to load", f => configFile = f },
                };
            flags.Parse(args);

            if (!String.IsNullOrEmpty(configFile))
            {
                string log4netFile = "ChangeDetection.log4net." + configFile;
                _logger = new Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, log4netFile);
            }

            if (_logger == null)
            {
                _logger = new Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "ChangeDetectionDefault.log4net.config");
            }

        }

        public static void ProcessArgs(string[] args)
        {
            _logger.Debug(MethodBase.GetCurrentMethod().Name + " [ " + string.Join(",", args) + " ]");

            string mode = "";
            string configFile = "";
            // Parse Flags
            var flags = new FlagParser()
                {
                    { "o", "operation", true, false, "The operation to perform", f => mode = f },
                    { "c", "config", true, false, "The config file to load", f => configFile = f },
                };
            flags.Parse(args);

            if (String.IsNullOrEmpty(configFile))
            {
                ShowHelp();
            }
            else
            {
                LoadConfig(configFile);

                //V3SF - CD API (EDGISREARC-1452) - Added [START]
                if(configFile.Contains(".config"))
                    Interface_Name = "CD_" + configFile.Split('.')[0].ToUpper();
                //V3SF - CD API (EDGISREARC-1452) - Added [END]

                switch (mode.ToUpper())
                {
                    case "KILLTEMPVERSIONS":
                        KillTempVersions();
                        break;
                    case "ROLLOVEREXTRACTGDB":
                        RolloverExtractGDB();
                        break;
                    case "CREATEINITIALMDB":
                        CreateInitialExtractGDB();
                        break;
                    case "CREATEINITIALVERSION":
                        CreateInitialVersions();
                        break;
                    case "RECREATEINITIALVERSION":
                        RecreateInitialVersion();
                        break;
                    case "OUTPUTVERSIONDIFFERENCES":
                        //V3SF - CD API - Commented
                        //OutputVersionDifferences();
                        //V3SF - CD API - Added
                        OutputVersionDifferencesCDAPI();
                        break;
                    case "RUNETL":
                        RunETL();
                        break;
                    case "RUNCDANDETL":
                        RunChangeDetection();
                        RunETL();
                        break;
                    default:
                        //Added this condition because for ADMS RUNETL should run first. 
                        if (configFile.ToUpper() == "ADMS_CD.config".ToUpper())
                        {
                            RunETL();
                            RunChangeDetection();
                        }
                        else
                        {
                            RunChangeDetection();
                            RunETL();
                        }
                        break;                        
                }
            }
        }

        protected static void ShowHelp()
        {
            _logger.Debug(MethodBase.GetCurrentMethod().Name);

            Console.WriteLine("PGEChangeDetection Usage:");
            Console.WriteLine("");
            Console.WriteLine("PGEChangeDetection -o rollOverExtractGDB -c <configfilename>");
            Console.WriteLine("PGEChangeDetection -o createInitialMDB -c <configfilename>");
            Console.WriteLine("PGEChangeDetection -o createInitialVersion -c <configfilename>");
            Console.WriteLine("PGEChangeDetection -o recreateInitialVersion -c <configfilename>");
            Console.WriteLine("PGEChangeDetection -o runETL -c <configfilename>");
            Console.WriteLine("PGEChangeDetection -o runCDAndETL -c <configfilename>");
            Console.WriteLine("PGEChangeDetection -o killTempVersions -c <configfilename>");
            Console.WriteLine("PGEChangeDetection -c <configfilename>");
            Console.WriteLine("");
        }

        protected static void LoadConfig(string configFile)
        {
            _logger.Debug(MethodBase.GetCurrentMethod().Name);
            _logger.Debug("Loading config file from [ " + configFile + " ]");
            Console.WriteLine("Loading config file from [ " + configFile + " ]");

            if (!File.Exists(configFile))
            {
                throw new ArgumentException("Config file doesn't exist! [ " + configFile + " ]");
            }
            _container = new WindsorContainer(new XmlInterpreter(filename: configFile));
 
        }

        public static void RunETL()
        {
            _logger.Debug(MethodBase.GetCurrentMethod().Name);

            try
            {
                IExtractTransformLoad[] extractTransformLoads = _container.ResolveAll<IExtractTransformLoad>();
                foreach (IExtractTransformLoad extractTransformLoad in extractTransformLoads)
                {
                    extractTransformLoad.Transfer();
                    extractTransformLoad.Dispose();
                }
            }
            catch (Castle.MicroKernel.ComponentNotFoundException)
            {
                _logger.Debug("Not running ETL (not set in configuration)");
            }

        }

        public static void RolloverExtractGDB()
        {
            _logger.Debug(MethodBase.GetCurrentMethod().Name);

            IExtractGDBManager extractGdbManager = _container.Resolve<IExtractGDBManager>();
            extractGdbManager.RolloverGDBs();

        }

        public static void CreateInitialExtractGDB()
        {
            _logger.Debug(MethodBase.GetCurrentMethod().Name);
            WriteStatus("Creating the initial base extract geodatabase...", true);

            IExtractGDBManager extractGdbManager = _container.Resolve<IExtractGDBManager>();
            extractGdbManager.ExportToBaseGDB();

        }

        public static SDEWorkspaceConnection GetActiveWorkspaceConnection()
        {
            SDEWorkspaceConnection sdeWorkspaceConnection = null;
            try
            {
                // This is not what should be done but we'd have to refactor the VersionedChangeDetector to 
                // move the Initialization out of the constructor. 
                sdeWorkspaceConnection =
                    _container.Resolve("pge.connections.landbaseSDEWorkspaceConnection") as SDEWorkspaceConnection;
                return sdeWorkspaceConnection;
            }
            catch
            {
            }
            try
            {
                // This is not what should be done but we'd have to refactor the VersionedChangeDetector to 
                // move the Initialization out of the constructor. 
                sdeWorkspaceConnection =
                    _container.Resolve("pge.connections.ederSubSDEWorkspaceConnection") as SDEWorkspaceConnection;
                return sdeWorkspaceConnection;
            }
            catch
            {
            }
            sdeWorkspaceConnection =
                _container.Resolve("pge.connections.ederSDEWorkspaceConnection") as SDEWorkspaceConnection;

            return sdeWorkspaceConnection;
        }

        public static IEnumerable<SDEWorkspaceConnection> GetActiveWorkspaceConnections()
        {
            SDEWorkspaceConnection sdeWorkspaceConnection = null;
            try
            {
                // This is not what should be done but we'd have to refactor the VersionedChangeDetector to 
                // move the Initialization out of the constructor. 
                sdeWorkspaceConnection =
                    _container.Resolve("pge.connections.landbaseSDEWorkspaceConnection") as SDEWorkspaceConnection;
            }
            catch
            {
            }
            if (sdeWorkspaceConnection != null)
            {
                yield return sdeWorkspaceConnection;
            }
            try
            {
                // This is not what should be done but we'd have to refactor the VersionedChangeDetector to 
                // move the Initialization out of the constructor. 
                sdeWorkspaceConnection =
                    _container.Resolve("pge.connections.ederSubSDEWorkspaceConnection") as SDEWorkspaceConnection;
            }
            catch
            {
            }
            if (sdeWorkspaceConnection != null)
            {
                yield return sdeWorkspaceConnection;
            }
            sdeWorkspaceConnection =
                _container.Resolve("pge.connections.ederSDEWorkspaceConnection") as SDEWorkspaceConnection;

            yield return sdeWorkspaceConnection;
        }


        public static void CreateInitialVersions()
        {
            _logger.Debug(MethodBase.GetCurrentMethod().Name);
            WriteStatus("Creating the version...", true);

            ChangeDetectionVersionInfo versionInfo = _container.Resolve<ChangeDetectionVersionInfo>();
            string versionName = versionInfo.VersionLow;

            foreach (SDEWorkspaceConnection sdeWorkspaceConnection in GetActiveWorkspaceConnections())
            {
                WriteStatus("Using GDB [" + sdeWorkspaceConnection.WorkspaceConnectionFile + " ]", true);
                try
                {
                    IVersion version = ((IVersionedWorkspace)sdeWorkspaceConnection.Workspace).DefaultVersion.CreateVersion(versionName);
                    version.Access = esriVersionAccess.esriVersionAccessPublic;
                    WriteStatus("Created the version [" + versionInfo.VersionLow + " ]", true);
                }
                catch (Exception exception)
                {
                    _logger.Error(exception.ToString());
                }
            }
            
        }

        public static void KillTempVersions()
        {
            _logger.Debug(MethodBase.GetCurrentMethod().Name);
            WriteStatus("Killing any outstanding versions...", true);

            string versionName = "";
            foreach (SDEWorkspaceConnection sdeWorkspaceConnection in GetActiveWorkspaceConnections())
            {
                WriteStatus("Using GDB [" + sdeWorkspaceConnection.WorkspaceConnectionFile + " ]", true);
                try
                {
                    WriteStatus("Trying to Delete Version [" + versionName + " ]", true);
                    ChangeDetectionVersionInfo versionInfo = _container.Resolve<ChangeDetectionVersionInfo>();
                    versionName = versionInfo.VersionTemp;
                    IVersion2 versionToDelete =
                        ((IVersionedWorkspace) sdeWorkspaceConnection.Workspace).FindVersion(versionName) as IVersion2;
                    versionToDelete.Delete();
                    WriteStatus("Deleted Version [" + versionName + " ]", true);
                }
                catch (Exception ex)
                {
                    WriteStatus("Couldn't delete Version [" + versionName + " ] [ " + ex.ToString() + " ]", true);
                }

                versionName = sdeWorkspaceConnection.NonVersionedEditsVersionName;
                try
                {
                    if (!String.IsNullOrEmpty(versionName))
                    {
                        WriteStatus("Trying to Delete Version [" + versionName + " ]", true);
                        IVersion2 versionToDelete =
                            ((IVersionedWorkspace) sdeWorkspaceConnection.Workspace).FindVersion(versionName) as
                                IVersion2;
                        versionToDelete.Delete();
                        WriteStatus("Deleted Version [" + versionName + " ]", true);
                    }
                }
                catch (Exception ex)
                {
                    WriteStatus("Couldn't delete Version [" + versionName + " ] [ " + ex.ToString() + " ]", true);
                }
            }
        }

        /// <summary>
        /// V3SF - CD API (EDGISREARC-1452) - Added
        /// CD API Changes
        /// </summary>
        public static void OutputVersionDifferencesCDAPI()
        {
            _logger.Debug(MethodBase.GetCurrentMethod().Name);
            ChangeDetectionVersionInfo versionInfo = _container.Resolve<ChangeDetectionVersionInfo>();
            WriteStatus("Getting Version Differences for [ " + versionInfo.VersionLow + " ] ", true);

            foreach (SDEWorkspaceConnection sdeWorkspaceConnection in GetActiveWorkspaceConnections())
            {
                WriteStatus("Using GDB [" + sdeWorkspaceConnection.WorkspaceConnectionFile + " ]", true);

                CDVersionManager cDVersionManager = new CDVersionManager(sdeWorkspaceConnection.Workspace, versionInfo.VersionTable,versionInfo.VersionTableConnStr);
                DateTime fromDate =  ExecutionSummary.LastProcessExecutionDate<DateTime>(versionInfo.VersionLow);
                DateTime toDate = DateTime.Now;
                IDictionary<string,ChangedFeatures> changedFeatures= cDVersionManager.CDVersionDifference(changeTypes.All, listType.None, fromDate: fromDate, toDate: toDate, withReplacement: false);
                string transactionType = "update";
                foreach (string FCName in changedFeatures.Keys)
                {
                    for(int i=0;i < changedFeatures[FCName].Action.Insert.Count;i++)
                    {
                        transactionType = "insert";
                        File.AppendAllText(@"c:\temp\" + transactionType + "_guids.csv", Convert.ToString(changedFeatures[FCName].Action.Insert[i].GUID) + "," + FCName.ToUpper() + "\r\n");
                    }
                    for (int i = 0; i < changedFeatures[FCName].Action.Update.Count; i++)
                    {
                        transactionType = "update";
                        File.AppendAllText(@"c:\temp\" + transactionType + "_guids.csv", Convert.ToString(changedFeatures[FCName].Action.Update[i].GUID) + "," + FCName.ToUpper() + "\r\n");
                    }
                    for (int i = 0; i < changedFeatures[FCName].Action.Delete.Count; i++)
                    {
                        transactionType = "delete";
                        File.AppendAllText(@"c:\temp\" + transactionType + "_guids.csv", Convert.ToString(changedFeatures[FCName].Action.Delete[i].GUID) + "," + FCName.ToUpper() + "\r\n");
                    }
                }
                
            }
        }

        // Needs to be refactored
        public static void OutputVersionDifferences()
        {
            _logger.Debug(MethodBase.GetCurrentMethod().Name);
            ChangeDetectionVersionInfo versionInfo = _container.Resolve<ChangeDetectionVersionInfo>();
            string versionName = versionInfo.VersionLow;
            WriteStatus("Getting Version Differences for [ " + versionName + " ] ", true);

            foreach (SDEWorkspaceConnection sdeWorkspaceConnection in GetActiveWorkspaceConnections())
            {
                WriteStatus("Using GDB [" + sdeWorkspaceConnection.WorkspaceConnectionFile + " ]", true);
                IVersion2 versionToCompare = null;
                try
                {
                    WriteStatus("Trying to Find Version [" + versionName + " ]", true);
                    versionToCompare =
                        ((IVersionedWorkspace)sdeWorkspaceConnection.Workspace).FindVersion(versionName) as IVersion2;
                }
                catch (Exception ex)
                {
                    WriteStatus("Couldn't find Version [" + versionName + " ] [ " + ex.ToString() + " ]", true);
                    continue;
                }

                esriDataChangeType[] differenceTypes = new esriDataChangeType[] { esriDataChangeType.esriDataChangeTypeInsert, esriDataChangeType.esriDataChangeTypeUpdate,
                esriDataChangeType.esriDataChangeTypeDelete };
                IVersionDataChangesInit vdci = null;
                IEnumModifiedClassInfo enumMCI = null;
                IVersion sourceVersion = ((IVersionedWorkspace)versionToCompare).DefaultVersion as IVersion;
                IVersion targetVersion = versionToCompare as IVersion;
                IWorkspace targetWorkspace = targetVersion as IWorkspace;
                IWorkspace sourceWorkspace = sourceVersion as IWorkspace;
                vdci = new ESRI.ArcGIS.GeoDatabaseDistributed.VersionDataChangesClass();
                IWorkspaceName wsNameSource = (IWorkspaceName)((IDataset)sourceVersion).FullName;
                IWorkspaceName wsNameTarget = (IWorkspaceName)((IDataset)targetVersion).FullName;
                vdci.Init(wsNameSource, wsNameTarget);

                VersionDataChanges vdc = (VersionDataChanges)vdci;
                IDataChanges dataChanges = (IDataChanges)vdci;
                IDataChangesInfo dci = (IDataChangesInfo)vdc;
                Console.WriteLine("Initialized the Geodatabase cursors and about to make call to IDatachanges.GetModifiedClassesInfo, this can take several minutes for a hundred thousand changes.");
                enumMCI = dataChanges.GetModifiedClassesInfo();
                Console.WriteLine("Returned!");
                enumMCI.Reset();
                IModifiedClassInfo mci;
                while ((mci = enumMCI.Next()) != null)
                {
                    string tableName = mci.ChildClassName;
                    Console.WriteLine("table [ " + tableName + " ]");

                    ITable ChangeTable = null;
                    ITable OriginalTable = null;

                    try
                    {
                        ChangeTable = ((IFeatureWorkspace)sourceVersion).OpenTable(tableName); // sde.default
                        OriginalTable = ((IFeatureWorkspace)targetVersion).OpenTable(tableName); // cd
                        Console.WriteLine("Initializing changes for table : " + tableName);
                    }
                    catch
                    {
                        Console.WriteLine("The table " + tableName + " either could not be " +
                                "located in the current version or it is not a versioned table.");
                    }
                    // Get the changes between the tables.
                    if (ChangeTable != null)
                    {

                        foreach (esriDataChangeType diffType in differenceTypes)
                        {
                            Console.WriteLine(Enum.GetName(typeof(esriDataChangeType), diffType));
                            IList<int> oids = new List<int>();
                            IFIDSet listIDs = dci.get_ChangedIDs(tableName, diffType);
                            listIDs.Reset();

                            int oid;
                            listIDs.Next(out oid);
                            while (oid != -1)
                            {
                                Console.WriteLine("Change [ " + ((IDataset)ChangeTable).Name + " ] Diff [ " + Enum.GetName(typeof(esriDataChangeType), diffType) + " ] OID [ " + oid.ToString() + " ]");
                                oids.Add(oid);
                                listIDs.Next(out oid);
                            }
                            // Need to reference original table for Delete GUIDs, otherwise ChangeTable
                            Console.WriteLine("oids count [ " + oids.Count + " ]");
                            if (oids.Count == 0) continue;
                            ITable deltaTable = diffType == esriDataChangeType.esriDataChangeTypeDelete ? OriginalTable : ChangeTable;
                            ICursor differenceRowsToAdd = deltaTable.GetRows(oids.ToArray(), false);
                            IRow row = null;
                            int globalIdFieldPosition = AOHelper.GlobalIDFieldPosition(deltaTable);
                            if (globalIdFieldPosition < 0)
                            {
                                Console.WriteLine("No GUID field");
                                continue;
                            }
                            string objectClassName = ((IDataset)deltaTable).Name;
                            string transactionType = "update";
                            if (diffType == esriDataChangeType.esriDataChangeTypeDelete)
                            {
                                transactionType = "delete";
                            }
                            else if (diffType == esriDataChangeType.esriDataChangeTypeInsert)
                            {
                                transactionType = "insert";
                            }
                            
                            while ((row = differenceRowsToAdd.NextRow()) != null)
                            {
                                object val = row.get_Value(globalIdFieldPosition);
                                if (val != DBNull.Value)
                                {
                                    File.AppendAllText(@"c:\temp\" + transactionType + "_guids.csv", Convert.ToString(val) + "," + objectClassName.ToUpper() + "\r\n");
                                }
                            }
                        }
                    }
                }
            }
        }

        public static void RecreateInitialVersion()
        {
            _logger.Debug(MethodBase.GetCurrentMethod().Name);
            WriteStatus("Recreating the version...", true);

            ChangeDetectionVersionInfo versionInfo = _container.Resolve<ChangeDetectionVersionInfo>();
            string versionName = versionInfo.VersionLow;
            foreach (SDEWorkspaceConnection sdeWorkspaceConnection in GetActiveWorkspaceConnections())
            {
                WriteStatus("Using GDB [" + sdeWorkspaceConnection.WorkspaceConnectionFile + " ]", true);
                try
                {
                    WriteStatus("Trying to Delete Version [" + versionName + " ]", true);
                    IVersion2 versionToDelete =
                        ((IVersionedWorkspace) sdeWorkspaceConnection.Workspace).FindVersion(versionName) as IVersion2;
                    versionToDelete.Delete();
                }
                catch (Exception ex)
                {
                    WriteStatus("Couldn't delete Version [" + versionName + " ] [ " + ex.ToString() + " ]", true);
                }

                IVersion version =
                    ((IVersionedWorkspace) sdeWorkspaceConnection.Workspace).DefaultVersion.CreateVersion(versionName);
                version.Access = esriVersionAccess.esriVersionAccessPublic;
                WriteStatus("Created the version [" + versionInfo.VersionLow + " ]", true);
            }
        }

        public static void RunChangeDetection()
        {
            _logger.Debug(MethodBase.GetCurrentMethod().Name);
            DateTime fromDate = default;
            DateTime date = default;
            string TableName = null;
            List<string> BatchIDs = default;
            CDBatchManager cDBatchManager = default;
            try
            {
                //V3SF - CD API (EDGISREARC-1452) - Added
                toDate = DateTime.Now;
                

                ChangeManager changeManager = _container.Resolve<ChangeManager>();

                //V3SF - CD API (EDGISREARC-1452) - Added
                ChangeDetectionVersionInfo versionInfo = default;
                try
                {
                    versionInfo = _container.Resolve<ChangeDetectionVersionInfo>();
                    fromDate = ExecutionSummary.LastProcessExecutionDate<DateTime>(Interface_Name);
                }
                catch { } 

                if (versionInfo != null)
                {
                    if (string.IsNullOrWhiteSpace(versionInfo.VersionTable))
                    {
                        changeManager.ProcessChanges();
                    }
                    else
                    {
                        //V3SF - CD API (EDGISREARC-1452) - Added
                        if (versionInfo.RecordCount != -1)
                        {
                            //Call Procedure
                            //Pass Query and other parameters like Record Count
                            //Foreach loop of Batch ID
                            //TableName = "INTDATAARCH."+InterfaceName
                            //Call ProcessChangesCDSUB(toDate,fromDate,TableName,ProcessID)
                            //fromDate = new DateTime(2022, 1, 17);
                            //toDate = new DateTime(2022, 2, 11);
                            

                            TableName = "INTDATAARCH." + Interface_Name + "_TEMP";
                            
                            BatchIDs = new List<string>();
                            cDBatchManager = new CDBatchManager(versionInfo.VersionTableConnStr, TableName);
                            
                            //Execute for All Batch IDS where were failed or not processed in previous run
                            BatchIDs = cDBatchManager.GetUnProcessedBatchID(TableName);
                            
                            if (BatchIDs.Count>0)
                            {
                                date = cDBatchManager.GetLasttoDate(TableName);
                                if (date != DateTime.MinValue)
                                    toDate = date;

                                foreach (string Batch in BatchIDs)
                                {
                                    try
                                    {
                                        cDBatchManager.UpdateProcessFlag(TableName, Batch, "T");
                                        //changeManager = _container.Resolve<ChangeManager>();
                                        changeManager.ProcessChangesCDBatch(toDate, fromDate, TableName, Batch);
                                        //changeManager.Dispose();
                                        //changeManager = null;
                                        cDBatchManager.UpdateProcessFlag(TableName, Batch, "P");
                                    }
                                    catch (Exception ex)
                                    {
                                        cDBatchManager.UpdateProcessFlag(TableName, Batch, "F");
                                        throw new Exception(Batch + "BatchID failed with error " + ex.Message + " at " + ex.StackTrace);
                                    }
                                }
                            }

                            //Get Max Date from Temp Table
                            date = cDBatchManager.GetDate(TableName);

                            if (date != DateTime.MinValue)
                                fromDate = date;

                            if (versionInfo.fromDateTime != DateTime.MinValue)
                                fromDate = versionInfo.fromDateTime;
                            if (versionInfo.toDateTime != DateTime.MinValue)
                                toDate = versionInfo.toDateTime;
                            else
                                toDate = DateTime.Now;

                            
                                
                            BatchIDs = cDBatchManager.PrepareTempTable(TableName, versionInfo.RecordCount, changeTypes.All, listType.None, fromDate: fromDate, toDate: toDate, whereClause: versionInfo.VersionWhereClause, _gdbmTable: versionInfo.VersionTable);

                            foreach (string Batch in BatchIDs)
                            {
                                try
                                {
                                    _logger.Info("Starting Batch :" + Batch);
                                    _logger.Info(LOGGINGDiagno.PRINTMemory());
                                    cDBatchManager.UpdateProcessFlag(TableName, Batch, "T");
                                    changeManager.ProcessChangesCDBatch(toDate, fromDate, TableName, Batch);
                                    cDBatchManager.UpdateProcessFlag(TableName, Batch, "P");
                                    _logger.Info("Ending Batch :" + Batch);
                                    _logger.Info(LOGGINGDiagno.PRINTMemory());
                                }
                                catch (Exception ex)
                                {
                                    cDBatchManager.UpdateProcessFlag(TableName, Batch, "F");
                                    throw new Exception(Batch + "BatchID failed with error " + ex.Message + " at " + ex.StackTrace);
                                }
                            }

                        }
                        else
                        {
                            changeManager.ProcessChangesCD(toDate, fromDate);
                        }

                    }
                }
                else
                {
                    changeManager.ProcessChanges();
                }
                
                changeManager.DetachAll();
                changeManager.Dispose();
                changeManager = null;
            }
            catch (Castle.MicroKernel.ComponentNotFoundException)
            {
                _logger.Debug("Not running Change Detection (not set in configuration)");
            }
        }


        public static void WriteEndStatus(DateTime exeStart)
        {
            //Report end time.
            DateTime exeEnd = DateTime.Now;
            _logger.Info("");
            _logger.Info("Completed");
            _logger.Info("Process start time: " + exeStart.ToLongDateString() + " " + exeStart.ToLongTimeString());
            _logger.Info("Process end time: " + exeEnd.ToLongDateString() + " " + exeEnd.ToLongTimeString());
            _logger.Info("Process length: " + (exeEnd - exeStart).ToString());
            _logger.Info("");

#if DEBUG
            //Console.Write("Press any key to exit.");
            //Console.ReadKey();
#endif

        }


        /// <summary>
        /// Print the usage details of the application (arguments to pass in).
        /// Prints to the log and console.
        /// </summary>
        static void PrintUsage()
        {
            _logger.Info("Usage for the PGEChangeDetection application");
            _logger.Info("             written by PGE");
            _logger.Info("Usage: PGEChangeDetection.exe [-o createInitial]");

            Console.Write("Press any key to continue.");
            Console.ReadKey(true);
        }

        #endregion

        #region Public Static Helper Methods

        /// <summary>
        /// Opens an SDE connection file from a file name and loads it into an IWorkspace.
        /// </summary>
        /// <param name="sFile">The file path of the desired SDE connection file.</param>
        /// <returns>An IWorkspace object from the connection.</returns>
        public static ESRI.ArcGIS.Geodatabase.IWorkspace IWorkspace_OpenFromFile(string sFile) 
        { 
            IWorkspaceFactory workspaceFactory = new  ESRI.ArcGIS.DataSourcesGDB.SdeWorkspaceFactory(); 
            return workspaceFactory.OpenFromFile(sFile, 0); 
        }

        /// <summary>
        /// Writes a message to the console and the log.
        /// </summary>
        /// <param name="status">The message/status to write.</param>
        /// <param name="writeToConsole">The message/status to write.</param>
        public static void WriteStatus(string status, bool writeToConsole)
        {
            status = DateTime.Now.ToString() + ": " + status;

            _logger.Info(status);
        }

        /// <summary>
        /// Writes exception details to the console and the log.
        /// </summary>
        /// <param name="ece">The exception about which to write details. All errors should be wrapped in the <see cref="ErrorCodeException"/> class.</param>
        public static void WriteError(ErrorCodeException ece)
        {
            string tempString = DateTime.Now.ToString() + ": " + "**** Error encountered  ****: ";
            tempString += "[" + ece.CodeNumber + "] ";
            _logger.Error(tempString);
            _logger.Error(ece.ToString());
        }

        /// <summary>
        /// Writes exception details to the console and the log.
        /// </summary>
        /// <param name="ex">The exception about which to write details. These exceptions should be considered warnings that do not break execution.</param>
        public static void WriteWarning(Exception ex)
        {
            string tempString = DateTime.Now.ToString() + ": " + "**** Error encountered  ****: ";
            _logger.Warn(tempString);
            _logger.Warn(ex.ToString());
        }

        #endregion

    }
}
