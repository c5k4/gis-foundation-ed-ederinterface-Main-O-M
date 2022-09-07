using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using Miner.Framework.Trace.Utilities;
using Miner.Interop;
using PGE.BatchApplication.CircuitProtectionZone.Data;
using PGE.BatchApplication.CircuitProtectionZone.Properties;
using ESRI.ArcGIS.DataSourcesGDB;
using PGE.BatchApplication.CircuitProtectionZone.Util;
using System.Linq;
using System.Diagnostics;
using System.Threading;
using System.Text;
using ESRI.ArcGIS.NetworkAnalysis;
using ESRI.ArcGIS.esriSystem;
using Miner.Framework.Trace.Strategies;
using Miner.Framework.Search;
using PGE.BatchApplication.CircuitProtectionZone.Common;
using System.Text.RegularExpressions;

namespace PGE.BatchApplication.CircuitProtectionZone
{
    public class ProtectionZoneTrace
    {
        private readonly IWorkspace _workspace;
        private readonly IWorkspace _landbaseWorkspace;
        private readonly IWorkspace _substationWorkspace;
        private string GeometricNetworkName = "";
        private string SubstationGeometricNetworkName = "";
        private string ConnectionFile = "";
        private string LandbaseConnectionFile = "";
        private string SubstationConnectionFile = "";
        private string ReportFileLocation = "";
        private string ReportFileName = "";
        private TraceReportType ReportType = TraceReportType.ProtectionZone;

        public ProtectionZoneTrace(string connectionString, string landbaseConnectionFile, string substationConnectionFile, string reportType, string reportFileLocation, string reportFileName)
        {
            //Connect to the specified 
            Logger.Info(string.Format("Connecting to database '{0}'", connectionString), true);
            SdeWorkspaceFactory WSFactory = new SdeWorkspaceFactoryClass();
            this._workspace = WSFactory.OpenFromFile(connectionString, 0);
            this._landbaseWorkspace = WSFactory.OpenFromFile(landbaseConnectionFile, 0);
            this._substationWorkspace = WSFactory.OpenFromFile(substationConnectionFile, 0);
            Geodatabase.workspace = _workspace;
            Geodatabase.LandbaseWorkspace = _landbaseWorkspace;
            Geodatabase.SubstationWorkspace = _substationWorkspace;

#if DEBUG
            Geodatabase.ExecuteSqlWorkspace = WSFactory.OpenFromFile(@"C:\Users\k1f8\AppData\Roaming\ESRI\Desktop10.2\ArcCatalog\DQ1T_edgis.sde", 0);
#else
            Geodatabase.ExecuteSqlWorkspace = _workspace;
#endif

            GeometricNetworkName = Settings.Default.DistributionNetworkName;
            SubstationGeometricNetworkName = Settings.Default.SubstationNetworkName;
            ConnectionFile = connectionString;
            LandbaseConnectionFile = landbaseConnectionFile;
            SubstationConnectionFile = substationConnectionFile;
            ReportFileLocation = reportFileLocation;
            ReportFileName = reportFileName;

            if (reportType == TraceReportType.EnergizedZone.ToString())
            {
                ReportType = TraceReportType.EnergizedZone;
            }
        }

        public IFeatureClass FireTierFeatureClass { get; set; }

        public IFeatureClass ZoneFeatureClass { get; set; }
        public IObjectClass ZoneDetailsTable { get; set; }

        public IGeometricNetwork GeometricNetwork { get; set; }

        public IGeometricNetwork SubGeometricNetwork { get; set; }

        #region Parent Process Execution

        /// <summary>
        /// Updates progress within the console application.  If anything fails, it just catches and moves on.  This is for the case that UC4 fails updating the console
        /// window which has been seen in the past
        /// </summary>
        /// <param name="message"></param>
        /// <param name="currElementIndex"></param>
        /// <param name="totalElementCount"></param>
        private void ShowPercentProgress(string message, double currElementIndex, double totalElementCount)
        {
            try
            {
                if (currElementIndex < 0 || currElementIndex > totalElementCount)
                {
                    throw new InvalidOperationException("currElement out of range");
                }
                double percent = Math.Round((100.0 * ((double)currElementIndex)) / (double)totalElementCount, 2);
                Console.Write("\r{0}: {1}% complete", message, percent);
            }
            catch (Exception ex)
            {

            }
        }

        public void Execute(int numberOfChildProcesses)
        {
            IMMFeederExt feederExt = null;
            IMMFeederSpace feederSpace = null;
            IMMFeederSource feederSource = null;

            try
            {
                //Delete any buffers currently existing in our buffer table.

                if (ReportType == TraceReportType.ProtectionZone)
                {
                    Logger.Info(string.Format("Deleting all existing features in {0}", Settings.Default.ZoneFeatureClassName), true);
                    Geodatabase.ExecuteSqlWorkspace.ExecuteSQL(string.Format("TRUNCATE TABLE {0}", Settings.Default.ZoneFeatureClassName));
                    Logger.Info(string.Format("Deleting all existing features in {0}", Settings.Default.ZoneDetailsFeatureClassName), true);
                    Geodatabase.ExecuteSqlWorkspace.ExecuteSQL(string.Format("TRUNCATE TABLE {0}", Settings.Default.ZoneDetailsFeatureClassName));
                    Geodatabase.ExecuteSqlWorkspace.ExecuteSQL("COMMIT");
                }
                else if (ReportType == TraceReportType.EnergizedZone)
                {
                    Logger.Info(string.Format("Deleting all existing features in {0}", Settings.Default.ZoneEnergizedFeatureClassName), true);
                    Geodatabase.ExecuteSqlWorkspace.ExecuteSQL(string.Format("TRUNCATE TABLE {0}", Settings.Default.ZoneEnergizedFeatureClassName));
                    Logger.Info(string.Format("Deleting all existing features in {0}", Settings.Default.ZoneEnergizedDetailsFeatureClassName), true);
                    Geodatabase.ExecuteSqlWorkspace.ExecuteSQL(string.Format("TRUNCATE TABLE {0}", Settings.Default.ZoneEnergizedDetailsFeatureClassName));
                    Geodatabase.ExecuteSqlWorkspace.ExecuteSQL("COMMIT");
                }

                //Determine our geometric networks
                Dictionary<string, IGeometricNetwork> geomNetworks = new Dictionary<string, IGeometricNetwork>();
                GetNetworks(_workspace, ref geomNetworks);
                GetNetworks(_substationWorkspace, ref geomNetworks);
                foreach (KeyValuePair<string, IGeometricNetwork> kvp in geomNetworks)
                {
                    if (kvp.Key.ToUpper() == GeometricNetworkName.ToUpper()) { GeometricNetwork = kvp.Value; }
                    else if (kvp.Key.ToUpper() == SubstationGeometricNetworkName.ToUpper()) { SubGeometricNetwork = kvp.Value; }
                }
                if (GeometricNetwork == null)
                {
                    throw new Exception("Unable to find geometric network: " + GeometricNetworkName);
                }
                if (SubGeometricNetwork == null)
                {
                    throw new Exception("Unable to find subsation geometric network: " + GeometricNetworkName);
                }

                //Get our feeder space for the network
                feederExt = GetFeederSpace(feederExt, ref feederSpace);

                IMMEnumFeederSource feederSources = feederSpace.FeederSources;
                feederSources.Reset();

                feederSource = feederSources.Next();
                int counter = 0;
                List<object> feederIDs = new List<object>();
                Logger.Info("Clearing the EDGIS.PGE_CircuitProtZones_Process table", true);
                Geodatabase.ExecuteSqlWorkspace.ExecuteSQL("DELETE FROM EDGIS.PGE_CircuitProtZones_Process");
                Logger.Info("Preparing EDGIS.PGE_CircuitProtZones_Process for child processing", true);
                while ((feederSource = feederSources.Next()) != null)
                {
                    string sql = string.Format("INSERT INTO {0} VALUES({1},'{2}','{3}')", "EDGIS.PGE_CircuitProtZones_Process", counter, feederSource.FeederID, CircuitStatus.InProgress);
                    Geodatabase.ExecuteSqlWorkspace.ExecuteSQL(sql);
                    counter++;
                    if ((counter % numberOfChildProcesses) == 0) { counter = 0; }
                }
                Geodatabase.ExecuteSqlWorkspace.ExecuteSQL("COMMIT");
                //Execute our processing in child threads that this process will keep track of
                Logger.Info("Starting child processes", true);
                ExecuteProcessingInChildren(numberOfChildProcesses);

                Logger.Info("Circuits processed successfully", true);
            }
            catch (Exception ex)
            {
                Logger.Error("Error executing tracing. Message: " + ex.Message + " StackTrace: " + ex.StackTrace, true);
                throw ex;
            }
            finally
            {
                if (feederExt != null) { while (Marshal.ReleaseComObject(feederExt) > 0);}
                if (feederSpace != null) { while (Marshal.ReleaseComObject(feederSpace) > 0);}
                if (feederSource != null) { while (Marshal.ReleaseComObject(feederSource) > 0);}
            }
        }

        private List<Process> childProcessesRunning = new List<Process>();
        private bool ExecuteProcessingInChildren(int numProcesses)
        {
            bool success = true;
            try
            {
                ITable CPZTable = Geodatabase.GetTable("EDGIS.PGE_CircuitProtZones_Process");

                for (int i = 0; i < numProcesses; i++)
                {
                    string AssemblyLocation = System.Reflection.Assembly.GetExecutingAssembly().Location;
                    Process myProcess = null;
                    string arguments = string.Format("-c \"{0}\" -pid {1} -L \"{2}\" -S \"{3}\"", ConnectionFile, i, LandbaseConnectionFile, SubstationConnectionFile);

                    //Pass the report type to children processes
                    if (ReportType == TraceReportType.EnergizedZone) { arguments += " -reporttype EnergizedZone"; }

                    AssemblyLocation = AssemblyLocation.Substring(0, AssemblyLocation.LastIndexOf("\\") + 1) + "PGE.BatchApplication.CircuitProtectionZoneCL.exe";
                    ProcessStartInfo startInfo = new ProcessStartInfo(AssemblyLocation, arguments)
                    {
                        CreateNoWindow = true,
                        WindowStyle = ProcessWindowStyle.Hidden,
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                    };

                    try
                    {
                        //_logger.Debug("Spawning new child process.  Arguments: " + startInfo.Arguments);
                        myProcess = new Process();
                        myProcess.StartInfo = startInfo;
                        myProcess.EnableRaisingEvents = true;
                        myProcess.Exited += new EventHandler(Process_Exited);
                        myProcess.OutputDataReceived += new DataReceivedEventHandler(myProcess_OutputDataReceived);
                        myProcess.Start();
                        myProcess.BeginOutputReadLine();

                        //Place our processes in our list so we can monitor the progress of each
                        childProcessesRunning.Add(myProcess);
                    }
                    catch (Exception ex)//if exception occurred, does the control table need to be cleaned?
                    {
                        success = false;
                        Console.WriteLine("Error creating Process. " + ex.Message);
                        throw ex;
                    }
                }

                IQueryFilter qf = new QueryFilterClass();
                qf.WhereClause = "";

                double totalToProcess = CPZTable.RowCount(qf);
                double currentlyProcessed = 0;
                qf.WhereClause = string.Format("CircuitStatus = '{0}' OR CircuitStatus = '{1}'", CircuitStatus.Failure, CircuitStatus.Success);
                while (ChildrenStillProcessing())
                {
                    currentlyProcessed = CPZTable.RowCount(qf);
                    double percentComplete = (Math.Round((currentlyProcessed / totalToProcess) * 100.0, 2));
                    ShowPercentProgress(string.Format("Processing Circuits: {0} of {1} processed", currentlyProcessed, totalToProcess), currentlyProcessed, totalToProcess);
                    Thread.Sleep(3000);
                }

                currentlyProcessed = CPZTable.RowCount(qf);
                ShowPercentProgress(string.Format("Processing Circuits: {0} of {1} processed", currentlyProcessed, totalToProcess), currentlyProcessed, totalToProcess);
            }
            catch (Exception ex)
            {
                success = false;
                Console.WriteLine("Unexpected error encountered: " + ex.Message);
                throw ex;
            }

            Console.WriteLine("");

            return success;
        }

        /// <summary>
        /// Error types for PTT transaction failures
        /// </summary>
        [Serializable]
        public enum CircuitStatus
        {
            Success,
            Failure,
            InProgress
        }

        private bool ChildrenStillProcessing()
        {
            bool processesStillRunning = false;

            try
            {
                if (childProcessesRunning.Count > 0)
                {
                    processesStillRunning = true;
                }

                return processesStillRunning;
            }
            catch (Exception e)
            {
                KillChildProcesses();
                throw e;
            }
        }

        private void KillChildProcesses()
        {
            try
            {
                foreach (Process p in childProcessesRunning)
                {
                    if (!p.HasExited) { p.Kill(); }
                }
            }
            catch { }
        }

        private Dictionary<int, List<string>> ProcessOutputInformation = new Dictionary<int, List<string>>();
        private void myProcess_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            Process sendingProcess = sender as Process;
            //Common.WriteToLog("Process ID '" + sendingProcess.Id + "'" + e.Data, LoggingLevel.Info, isParent);
            if (!ProcessOutputInformation.ContainsKey(sendingProcess.Id)) { ProcessOutputInformation.Add(sendingProcess.Id, new List<string>()); }

            ProcessOutputInformation[sendingProcess.Id].Add(e.Data);
        }

        private static bool ProcessFailed = false;
        private System.Object childProcsRunningLock = new System.Object();
        private void Process_Exited(object sender, EventArgs e)
        {
            Process p = sender as Process;

            if (ProcessFailed) { return; }
            if (p.ExitCode != 0)
            {
                Console.WriteLine("Error in child process ID " + p.Id + ". Killing all child processes");
                ProcessFailed = true;
                lock (childProcsRunningLock)
                {
                    foreach (Process process in childProcessesRunning)
                    {
                        if (process != null && !process.HasExited)
                        {
                            process.Kill();
                        }
                    }
                }

                //Write out the logging information from this process
                if (ProcessOutputInformation.ContainsKey(p.Id))
                {
                    StringBuilder builder = new StringBuilder();
                    builder.AppendLine("***************************************************************");
                    builder.AppendLine("***************************************************************");
                    builder.AppendLine("***************************************************************");
                    builder.AppendLine("***************************************************************");
                    builder.AppendLine("Process ID " + p.Id + " failed during processing");
                    foreach (string message in ProcessOutputInformation[p.Id])
                    {
                        builder.AppendLine(message);
                    }
                    builder.AppendLine("***************************************************************");
                    builder.AppendLine("***************************************************************");
                    builder.AppendLine("***************************************************************");
                    builder.AppendLine("***************************************************************");
                    Logger.Error(builder.ToString(), true);
                    Console.WriteLine(builder.ToString());
                }
                Environment.ExitCode = 1;
#if DEBUG
                Console.WriteLine("Press any key to exit");
                Console.ReadLine();
#endif
                throw new Exception("Unexpected failure in child process");
            }
            else
            {
                //Write out the logging information from this process
                if (ProcessOutputInformation.ContainsKey(p.Id))
                {
                    StringBuilder builder = new StringBuilder();
                    builder.AppendLine("***************************************************************");
                    builder.AppendLine("***************************************************************");
                    builder.AppendLine("***************************************************************");
                    builder.AppendLine("***************************************************************");
                    builder.AppendLine("Process ID " + p.Id + " processed successfully");
                    foreach (string message in ProcessOutputInformation[p.Id])
                    {
                        builder.AppendLine(message);
                    }
                    builder.AppendLine("***************************************************************");
                    builder.AppendLine("***************************************************************");
                    builder.AppendLine("***************************************************************");
                    builder.AppendLine("***************************************************************");
                    Logger.Info(builder.ToString(), false);
                }
            }

            lock (childProcsRunningLock) { childProcessesRunning.Remove(p); }
        }

        #endregion

        private Dictionary<string, string> MultiVersionedViewNames = new Dictionary<string, string>();
        /// <summary>
        /// Gets the name of the Esri multi version view table for the provided feature class. 
        /// </summary>
        /// <param name="featureClass"></param>
        /// <returns></returns>
        private string GetMultiVersionViewName(ITable table)
        {
            string featureClassName = ((IDataset)table).BrowseName;
            featureClassName = ((IDataset)table).BrowseName.ToUpper();

            if (MultiVersionedViewNames.Count < 1)
            {
                //Build the list of all tables with their associated multi version view names
                IQueryFilter qf = new QueryFilterClass();
                qf.AddField("OWNER");
                qf.AddField("TABLE_NAME");
                qf.AddField("IMV_VIEW_NAME");

                ITable tableRegistry = ((IFeatureWorkspace)((IDataset)table).Workspace).OpenTable("SDE.TABLE_REGISTRY");
                int ownerIdx = tableRegistry.FindField("OWNER");
                int tableNameIdx = tableRegistry.FindField("TABLE_NAME");
                int viewNameIdx = tableRegistry.FindField("IMV_VIEW_NAME");
                ICursor rowCursor = tableRegistry.Search(qf, false);
                IRow row = null;
                while ((row = rowCursor.NextRow()) != null)
                {
                    string owner = row.get_Value(ownerIdx).ToString();
                    string tableName = row.get_Value(tableNameIdx).ToString();
                    string imvViewName = row.get_Value(viewNameIdx).ToString();

                    string fullTableName = owner + "." + tableName;
                    if (string.IsNullOrEmpty(imvViewName)) { imvViewName = fullTableName; }
                    else
                    {
                        imvViewName = owner + "." + imvViewName;
                    }

                    if (!MultiVersionedViewNames.ContainsKey(fullTableName)) { MultiVersionedViewNames.Add(fullTableName.ToUpper(), imvViewName.ToUpper()); }
                }
            }

            //Return the associated view name.  If a table isn't versioned, it will simply return the feature class name
            string viewName = "";
            if (MultiVersionedViewNames.ContainsKey(featureClassName))
            {
                viewName = MultiVersionedViewNames[featureClassName];
            }
            if (string.IsNullOrEmpty(viewName)) { viewName = featureClassName; }
            return viewName;
        }

        public void CalculateCustomers(int PID)
        {
            ITable servicePointTable = ((IFeatureWorkspace)Geodatabase.workspace).OpenTable("EDGIS.ServicePoint");
            string servicePointTableView = GetMultiVersionViewName(servicePointTable);
            if (PID < 0)
            {
                Logger.Info("Calculating customers", true);
                if (ReportType == TraceReportType.EnergizedZone)
                {
                    //Count of all customers
                    string sql = "MERGE INTO EDGIS.PGE_CircuitEnergizedZones T USING (SELECT Z.GLOBALID,COUNT(*) SPCOUNT FROM EDGIS.PGE_CircuitEnergizedZones Z JOIN (select D.CPZGLOBALID,SL.globalid from EDGIS.PGE_CircuitEnergizedDetails D join " + servicePointTableView + " SL on D.featureglobalid = SL.servicelocationguid) SERVICEPOINTS ON Z.GLOBALID = SERVICEPOINTS.CPZGLOBALID GROUP BY Z.GLOBALID) S ON (T.GLOBALID = S.GLOBALID) WHEN MATCHED THEN UPDATE SET T.CUSTOMERSINZONE = S.SPCOUNT";
                    Logger.Info("Calculating customer counts", true);
                    Logger.Debug(sql, false);
                    Geodatabase.ExecuteSqlWorkspace.ExecuteSQL(sql);

                    //Essential customers
                    sql = "MERGE INTO EDGIS.PGE_CircuitEnergizedZones T USING (SELECT Z.GLOBALID,COUNT(*) SPCOUNT FROM EDGIS.PGE_CircuitEnergizedZones Z JOIN (select D.CPZGLOBALID,SL.globalid from EDGIS.PGE_CircuitEnergizedDetails D join " + servicePointTableView + " SL on D.featureglobalid = SL.servicelocationguid AND SL.ESSENTIALCUSTOMERIDC = 'Y') SERVICEPOINTS ON Z.GLOBALID = SERVICEPOINTS.CPZGLOBALID GROUP BY Z.GLOBALID) S ON (T.GLOBALID = S.GLOBALID) WHEN MATCHED THEN UPDATE SET T.CUSTOMERSESSENTIAL = S.SPCOUNT";
                    Logger.Info("Calculating essential customer counts", true);
                    Logger.Debug(sql, false);
                    Geodatabase.ExecuteSqlWorkspace.ExecuteSQL(sql);

                    //Sensitive customers
                    sql = "MERGE INTO EDGIS.PGE_CircuitEnergizedZones T USING (SELECT Z.GLOBALID,COUNT(*) SPCOUNT FROM EDGIS.PGE_CircuitEnergizedZones Z JOIN (select D.CPZGLOBALID,SL.globalid from EDGIS.PGE_CircuitEnergizedDetails D join " + servicePointTableView + " SL on D.featureglobalid = SL.servicelocationguid AND SL.SENSITIVECUSTOMERIDC = 'Y') SERVICEPOINTS ON Z.GLOBALID = SERVICEPOINTS.CPZGLOBALID GROUP BY Z.GLOBALID) S ON (T.GLOBALID = S.GLOBALID) WHEN MATCHED THEN UPDATE SET T.CUSTOMERSSENSITIVE = S.SPCOUNT";
                    Logger.Info("Calculating sensitive customer counts", true);
                    Logger.Debug(sql, false);
                    Geodatabase.ExecuteSqlWorkspace.ExecuteSQL(sql);

                    //LifeSupport customers
                    sql = "MERGE INTO EDGIS.PGE_CircuitEnergizedZones T USING (SELECT Z.GLOBALID,COUNT(*) SPCOUNT FROM EDGIS.PGE_CircuitEnergizedZones Z JOIN (select D.CPZGLOBALID,SL.globalid from EDGIS.PGE_CircuitEnergizedDetails D join " + servicePointTableView + " SL on D.featureglobalid = SL.servicelocationguid AND SL.LIFESUPPORTIDC = 'Y') SERVICEPOINTS ON Z.GLOBALID = SERVICEPOINTS.CPZGLOBALID GROUP BY Z.GLOBALID) S ON (T.GLOBALID = S.GLOBALID) WHEN MATCHED THEN UPDATE SET T.CUSTOMERSLIFESUPPORT = S.SPCOUNT";
                    Logger.Info("Calculating life support customer counts", true);
                    Logger.Debug(sql, false);
                    Geodatabase.ExecuteSqlWorkspace.ExecuteSQL(sql);

                    //DOM customers
                    sql = "MERGE INTO EDGIS.PGE_CircuitEnergizedZones T USING (SELECT Z.GLOBALID,COUNT(*) SPCOUNT FROM EDGIS.PGE_CircuitEnergizedZones Z JOIN (select D.CPZGLOBALID,SL.globalid from EDGIS.PGE_CircuitEnergizedDetails D join " + servicePointTableView + " SL on D.featureglobalid = SL.servicelocationguid AND SL.CUSTOMERTYPE = 'DOM') SERVICEPOINTS ON Z.GLOBALID = SERVICEPOINTS.CPZGLOBALID GROUP BY Z.GLOBALID) S ON (T.GLOBALID = S.GLOBALID) WHEN MATCHED THEN UPDATE SET T.CUSTOMERSDOM = S.SPCOUNT";
                    Logger.Info("Calculating DOM customer counts", true);
                    Logger.Debug(sql, false);
                    Geodatabase.ExecuteSqlWorkspace.ExecuteSQL(sql);

                    //COM customers
                    sql = "MERGE INTO EDGIS.PGE_CircuitEnergizedZones T USING (SELECT Z.GLOBALID,COUNT(*) SPCOUNT FROM EDGIS.PGE_CircuitEnergizedZones Z JOIN (select D.CPZGLOBALID,SL.globalid from EDGIS.PGE_CircuitEnergizedDetails D join " + servicePointTableView + " SL on D.featureglobalid = SL.servicelocationguid AND SL.CUSTOMERTYPE = 'COM') SERVICEPOINTS ON Z.GLOBALID = SERVICEPOINTS.CPZGLOBALID GROUP BY Z.GLOBALID) S ON (T.GLOBALID = S.GLOBALID) WHEN MATCHED THEN UPDATE SET T.CUSTOMERSCOM = S.SPCOUNT";
                    Logger.Info("Calculating COM customer counts", true);
                    Logger.Debug(sql, false);
                    Geodatabase.ExecuteSqlWorkspace.ExecuteSQL(sql);

                    //AGR customers
                    sql = "MERGE INTO EDGIS.PGE_CircuitEnergizedZones T USING (SELECT Z.GLOBALID,COUNT(*) SPCOUNT FROM EDGIS.PGE_CircuitEnergizedZones Z JOIN (select D.CPZGLOBALID,SL.globalid from EDGIS.PGE_CircuitEnergizedDetails D join " + servicePointTableView + " SL on D.featureglobalid = SL.servicelocationguid AND SL.CUSTOMERTYPE = 'AGR') SERVICEPOINTS ON Z.GLOBALID = SERVICEPOINTS.CPZGLOBALID GROUP BY Z.GLOBALID) S ON (T.GLOBALID = S.GLOBALID) WHEN MATCHED THEN UPDATE SET T.CUSTOMERSAGR = S.SPCOUNT";
                    Logger.Info("Calculating AGR customer counts", true);
                    Logger.Debug(sql, false);
                    Geodatabase.ExecuteSqlWorkspace.ExecuteSQL(sql);

                    //IND customers
                    sql = "MERGE INTO EDGIS.PGE_CircuitEnergizedZones T USING (SELECT Z.GLOBALID,COUNT(*) SPCOUNT FROM EDGIS.PGE_CircuitEnergizedZones Z JOIN (select D.CPZGLOBALID,SL.globalid from EDGIS.PGE_CircuitEnergizedDetails D join " + servicePointTableView + " SL on D.featureglobalid = SL.servicelocationguid AND SL.CUSTOMERTYPE = 'IND') SERVICEPOINTS ON Z.GLOBALID = SERVICEPOINTS.CPZGLOBALID GROUP BY Z.GLOBALID) S ON (T.GLOBALID = S.GLOBALID) WHEN MATCHED THEN UPDATE SET T.CUSTOMERSIND = S.SPCOUNT";
                    Logger.Info("Calculating IND customer counts", true);
                    Logger.Debug(sql, false);
                    Geodatabase.ExecuteSqlWorkspace.ExecuteSQL(sql);

                    //OTH customers
                    sql = "MERGE INTO EDGIS.PGE_CircuitEnergizedZones T USING (SELECT Z.GLOBALID,COUNT(*) SPCOUNT FROM EDGIS.PGE_CircuitEnergizedZones Z JOIN (select D.CPZGLOBALID,SL.globalid from EDGIS.PGE_CircuitEnergizedDetails D join " + servicePointTableView + " SL on D.featureglobalid = SL.servicelocationguid AND SL.CUSTOMERTYPE = 'OTH') SERVICEPOINTS ON Z.GLOBALID = SERVICEPOINTS.CPZGLOBALID GROUP BY Z.GLOBALID) S ON (T.GLOBALID = S.GLOBALID) WHEN MATCHED THEN UPDATE SET T.CUSTOMERSOTH = S.SPCOUNT";
                    Logger.Info("Calculating OTH customer counts", true);
                    Logger.Debug(sql, false);
                    Geodatabase.ExecuteSqlWorkspace.ExecuteSQL(sql);
                }
                else if (ReportType == TraceReportType.ProtectionZone)
                {
                    //Count of all customers
                    string sql = "MERGE INTO EDGIS.pge_circuitprotectionzones T USING (SELECT Z.GLOBALID,COUNT(*) SPCOUNT FROM EDGIS.pge_circuitprotectionzones Z JOIN (select D.CPZGLOBALID,SL.globalid from EDGIS.pge_circuitprotectiondetails D join " + servicePointTableView + " SL on D.featureglobalid = SL.servicelocationguid) SERVICEPOINTS ON Z.GLOBALID = SERVICEPOINTS.CPZGLOBALID GROUP BY Z.GLOBALID) S ON (T.GLOBALID = S.GLOBALID) WHEN MATCHED THEN UPDATE SET T.CUSTOMERSINZONE = S.SPCOUNT";
                    Logger.Info("Calculating customer counts", true);
                    Logger.Debug(sql, false);
                    Geodatabase.ExecuteSqlWorkspace.ExecuteSQL(sql);

                    //Essential customers
                    sql = "MERGE INTO EDGIS.pge_circuitprotectionzones T USING (SELECT Z.GLOBALID,COUNT(*) SPCOUNT FROM EDGIS.pge_circuitprotectionzones Z JOIN (select D.CPZGLOBALID,SL.globalid from EDGIS.pge_circuitprotectiondetails D join " + servicePointTableView + " SL on D.featureglobalid = SL.servicelocationguid AND SL.ESSENTIALCUSTOMERIDC = 'Y') SERVICEPOINTS ON Z.GLOBALID = SERVICEPOINTS.CPZGLOBALID GROUP BY Z.GLOBALID) S ON (T.GLOBALID = S.GLOBALID) WHEN MATCHED THEN UPDATE SET T.CUSTOMERSESSENTIAL = S.SPCOUNT";
                    Logger.Info("Calculating essential customer counts", true);
                    Logger.Debug(sql, false);
                    Geodatabase.ExecuteSqlWorkspace.ExecuteSQL(sql);

                    //Sensitive customers
                    sql = "MERGE INTO EDGIS.pge_circuitprotectionzones T USING (SELECT Z.GLOBALID,COUNT(*) SPCOUNT FROM EDGIS.pge_circuitprotectionzones Z JOIN (select D.CPZGLOBALID,SL.globalid from EDGIS.pge_circuitprotectiondetails D join " + servicePointTableView + " SL on D.featureglobalid = SL.servicelocationguid AND SL.SENSITIVECUSTOMERIDC = 'Y') SERVICEPOINTS ON Z.GLOBALID = SERVICEPOINTS.CPZGLOBALID GROUP BY Z.GLOBALID) S ON (T.GLOBALID = S.GLOBALID) WHEN MATCHED THEN UPDATE SET T.CUSTOMERSSENSITIVE = S.SPCOUNT";
                    Logger.Info("Calculating sensitive customer counts", true);
                    Logger.Debug(sql, false);
                    Geodatabase.ExecuteSqlWorkspace.ExecuteSQL(sql);

                    //LifeSupport customers
                    sql = "MERGE INTO EDGIS.pge_circuitprotectionzones T USING (SELECT Z.GLOBALID,COUNT(*) SPCOUNT FROM EDGIS.pge_circuitprotectionzones Z JOIN (select D.CPZGLOBALID,SL.globalid from EDGIS.pge_circuitprotectiondetails D join " + servicePointTableView + " SL on D.featureglobalid = SL.servicelocationguid AND SL.LIFESUPPORTIDC = 'Y') SERVICEPOINTS ON Z.GLOBALID = SERVICEPOINTS.CPZGLOBALID GROUP BY Z.GLOBALID) S ON (T.GLOBALID = S.GLOBALID) WHEN MATCHED THEN UPDATE SET T.CUSTOMERSLIFESUPPORT = S.SPCOUNT";
                    Logger.Info("Calculating life support customer counts", true);
                    Logger.Debug(sql, false);
                    Geodatabase.ExecuteSqlWorkspace.ExecuteSQL(sql);

                    //DOM customers
                    sql = "MERGE INTO EDGIS.pge_circuitprotectionzones T USING (SELECT Z.GLOBALID,COUNT(*) SPCOUNT FROM EDGIS.pge_circuitprotectionzones Z JOIN (select D.CPZGLOBALID,SL.globalid from EDGIS.pge_circuitprotectiondetails D join " + servicePointTableView + " SL on D.featureglobalid = SL.servicelocationguid AND SL.CUSTOMERTYPE = 'DOM') SERVICEPOINTS ON Z.GLOBALID = SERVICEPOINTS.CPZGLOBALID GROUP BY Z.GLOBALID) S ON (T.GLOBALID = S.GLOBALID) WHEN MATCHED THEN UPDATE SET T.CUSTOMERSDOM = S.SPCOUNT";
                    Logger.Info("Calculating DOM customer counts", true);
                    Logger.Debug(sql, false);
                    Geodatabase.ExecuteSqlWorkspace.ExecuteSQL(sql);

                    //COM customers
                    sql = "MERGE INTO EDGIS.pge_circuitprotectionzones T USING (SELECT Z.GLOBALID,COUNT(*) SPCOUNT FROM EDGIS.pge_circuitprotectionzones Z JOIN (select D.CPZGLOBALID,SL.globalid from EDGIS.pge_circuitprotectiondetails D join " + servicePointTableView + " SL on D.featureglobalid = SL.servicelocationguid AND SL.CUSTOMERTYPE = 'COM') SERVICEPOINTS ON Z.GLOBALID = SERVICEPOINTS.CPZGLOBALID GROUP BY Z.GLOBALID) S ON (T.GLOBALID = S.GLOBALID) WHEN MATCHED THEN UPDATE SET T.CUSTOMERSCOM = S.SPCOUNT";
                    Logger.Info("Calculating COM customer counts", true);
                    Logger.Debug(sql, false);
                    Geodatabase.ExecuteSqlWorkspace.ExecuteSQL(sql);

                    //AGR customers
                    sql = "MERGE INTO EDGIS.pge_circuitprotectionzones T USING (SELECT Z.GLOBALID,COUNT(*) SPCOUNT FROM EDGIS.pge_circuitprotectionzones Z JOIN (select D.CPZGLOBALID,SL.globalid from EDGIS.pge_circuitprotectiondetails D join " + servicePointTableView + " SL on D.featureglobalid = SL.servicelocationguid AND SL.CUSTOMERTYPE = 'AGR') SERVICEPOINTS ON Z.GLOBALID = SERVICEPOINTS.CPZGLOBALID GROUP BY Z.GLOBALID) S ON (T.GLOBALID = S.GLOBALID) WHEN MATCHED THEN UPDATE SET T.CUSTOMERSAGR = S.SPCOUNT";
                    Logger.Info("Calculating AGR customer counts", true);
                    Logger.Debug(sql, false);
                    Geodatabase.ExecuteSqlWorkspace.ExecuteSQL(sql);

                    //IND customers
                    sql = "MERGE INTO EDGIS.pge_circuitprotectionzones T USING (SELECT Z.GLOBALID,COUNT(*) SPCOUNT FROM EDGIS.pge_circuitprotectionzones Z JOIN (select D.CPZGLOBALID,SL.globalid from EDGIS.pge_circuitprotectiondetails D join " + servicePointTableView + " SL on D.featureglobalid = SL.servicelocationguid AND SL.CUSTOMERTYPE = 'IND') SERVICEPOINTS ON Z.GLOBALID = SERVICEPOINTS.CPZGLOBALID GROUP BY Z.GLOBALID) S ON (T.GLOBALID = S.GLOBALID) WHEN MATCHED THEN UPDATE SET T.CUSTOMERSIND = S.SPCOUNT";
                    Logger.Info("Calculating IND customer counts", true);
                    Logger.Debug(sql, false);
                    Geodatabase.ExecuteSqlWorkspace.ExecuteSQL(sql);

                    //OTH customers
                    sql = "MERGE INTO EDGIS.pge_circuitprotectionzones T USING (SELECT Z.GLOBALID,COUNT(*) SPCOUNT FROM EDGIS.pge_circuitprotectionzones Z JOIN (select D.CPZGLOBALID,SL.globalid from EDGIS.pge_circuitprotectiondetails D join " + servicePointTableView + " SL on D.featureglobalid = SL.servicelocationguid AND SL.CUSTOMERTYPE = 'OTH') SERVICEPOINTS ON Z.GLOBALID = SERVICEPOINTS.CPZGLOBALID GROUP BY Z.GLOBALID) S ON (T.GLOBALID = S.GLOBALID) WHEN MATCHED THEN UPDATE SET T.CUSTOMERSOTH = S.SPCOUNT";
                    Logger.Info("Calculating OTH customer counts", true);
                    Logger.Debug(sql, false);
                    Geodatabase.ExecuteSqlWorkspace.ExecuteSQL(sql);
                }
            }
        }

        public void ExecuteTracing(int PID)
        {
            IMMFeederExt feederExt = null;
            IMMFeederSpace feederSpace = null;
            IMMFeederSource feederSource = null;

            try
            {
                if (PID < 0)
                {
                    if (ReportType == TraceReportType.ProtectionZone)
                    {
                        Logger.Info(string.Format("Deleting all existing features in {0}", Settings.Default.ZoneFeatureClassName), true);
                        Geodatabase.ExecuteSqlWorkspace.ExecuteSQL(string.Format("TRUNCATE TABLE {0}", Settings.Default.ZoneFeatureClassName));
                        Logger.Info(string.Format("Deleting all existing features in {0}", Settings.Default.ZoneDetailsFeatureClassName), true);
                        Geodatabase.ExecuteSqlWorkspace.ExecuteSQL(string.Format("TRUNCATE TABLE {0}", Settings.Default.ZoneDetailsFeatureClassName));
                        Geodatabase.ExecuteSqlWorkspace.ExecuteSQL("COMMIT");
                    }
                    else if (ReportType == TraceReportType.EnergizedZone)
                    {
                        Logger.Info(string.Format("Deleting all existing features in {0}", Settings.Default.ZoneEnergizedFeatureClassName), true);
                        Geodatabase.ExecuteSqlWorkspace.ExecuteSQL(string.Format("TRUNCATE TABLE {0}", Settings.Default.ZoneEnergizedFeatureClassName));
                        Logger.Info(string.Format("Deleting all existing features in {0}", Settings.Default.ZoneEnergizedDetailsFeatureClassName), true);
                        Geodatabase.ExecuteSqlWorkspace.ExecuteSQL(string.Format("TRUNCATE TABLE {0}", Settings.Default.ZoneEnergizedDetailsFeatureClassName));
                        Geodatabase.ExecuteSqlWorkspace.ExecuteSQL("COMMIT");
                    }
                }

                //Determine our geometric networks
                Dictionary<string, IGeometricNetwork> geomNetworks = new Dictionary<string, IGeometricNetwork>();
                GetNetworks(_workspace, ref geomNetworks);
                GetNetworks(_substationWorkspace, ref geomNetworks);
                foreach (KeyValuePair<string, IGeometricNetwork> kvp in geomNetworks)
                {
                    if (kvp.Key.ToUpper() == GeometricNetworkName.ToUpper()) { GeometricNetwork = kvp.Value; }
                    else if (kvp.Key.ToUpper() == SubstationGeometricNetworkName.ToUpper()) { SubGeometricNetwork = kvp.Value; }
                }
                if (GeometricNetwork == null)
                {
                    throw new Exception("Unable to find distribution geometric network: " + GeometricNetworkName);
                }
                if (SubGeometricNetwork == null)
                {
                    throw new Exception("Unable to find subsation geometric network: " + GeometricNetworkName);
                }

                //Get our feeder space for the network
                feederExt = GetFeederSpace(feederExt, ref feederSpace);

                IMMEnumFeederSource feederSources = feederSpace.FeederSources;
                feederSources.Reset();

                List<string> circuitsToProcess = new List<string>();
                if (PID >= 0)
                {
                    Logger.Info("Querying EDGIS.PGE_CircuitProtZones_Process for circuits to process", true);
                    ITable CPZTable = Geodatabase.GetTable("EDGIS.PGE_CircuitProtZones_Process");
                    IQueryFilter qf = new QueryFilterClass();
                    qf.WhereClause = string.Format("{0} = {1}", "PROCESSID", PID);
                    ICursor rowCursor = CPZTable.Search(qf, false);
                    IRow row = null;
                    int circuitIDIdx = CPZTable.Fields.FindField("CIRCUITID");
                    while ((row = rowCursor.NextRow()) != null)
                    {
                        string circuitID = row.get_Value(circuitIDIdx).ToString();
                        circuitsToProcess.Add(circuitID);
                    }
                }

                List<object> feederIDs = new List<object>();
#if DEBUG
                //feederIDs.Add("254072112");

                
                feederSource = feederSources.Next();
                while ((feederSource = feederSources.Next()) != null)
                {
                    if (PID < 0 || circuitsToProcess.Contains(feederSource.FeederID.ToString())) { feederIDs.Add(feederSource.FeederID); }
                }
                feederIDs = feederIDs.Distinct().ToList();
                feederIDs.Sort();
                
#else
                feederSource = feederSources.Next();
                while ((feederSource = feederSources.Next()) != null)
                {
                    if (PID < 0 || circuitsToProcess.Contains(feederSource.FeederID.ToString())) { feederIDs.Add(feederSource.FeederID); }
                }
                feederIDs = feederIDs.Distinct().ToList();
                feederIDs.Sort();
#endif

                Logger.Info(string.Format("There were {0} circuits found to process", feederIDs.Count), true);

                int feederSourceCount = feederIDs.Count;
                int feedersProcessed = 0;
                //ShowPercentProgress(string.Format("{0} of {1} processed", feedersProcessed, feederSourceCount), feedersProcessed, feederSourceCount);
                foreach (object feederID in feederIDs)
                {
                    feederSource = feederSources.get_FeederSourceByFeederID(feederID);
                    if (feederSource.JunctionEID > 0)
                    {
                        Logger.Debug("Processing Feeder Name: " + feederSource.FeederID, false);
                        Trace(feederSource);
                        feedersProcessed++;
                        string sql = string.Format("UPDATE {0} SET CIRCUITSTATUS = '{1}' WHERE CIRCUITID = '{2}'", "EDGIS.PGE_CircuitProtZones_Process", CircuitStatus.Success, feederID.ToString());
                        Geodatabase.ExecuteSqlWorkspace.ExecuteSQL(sql);
                        Geodatabase.ExecuteSqlWorkspace.ExecuteSQL("COMMIT");
                        //ShowPercentProgress(string.Format("{0} of {1} processed", feedersProcessed, feederSourceCount), feedersProcessed, feederSourceCount);
                    }
                    else
                    {
                        Logger.Debug("Feeder Name: " + feederSource.FeederID + " is not part of the logical network.", false);
                    }
                    if (feederSource != null) { Marshal.ReleaseComObject(feederSource); }
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Error executing tracing. Message: " + ex.Message + " StackTrace: " + ex.StackTrace, true);
                throw ex;
            }
            finally
            {
                if (feederExt != null) { while (Marshal.ReleaseComObject(feederExt) > 0);}
                if (feederSpace != null) { while (Marshal.ReleaseComObject(feederSpace) > 0);}
                if (feederSource != null) { while (Marshal.ReleaseComObject(feederSource) > 0);}
            }
        }

        /// <summary>
        /// This method will return the collection of netwotrks from the specified workspace
        /// </summary>
        /// <param name="networkName"></param>
        /// <returns></returns>
        private static void GetNetworks(IWorkspace ws, ref Dictionary<string, IGeometricNetwork> geomNetworks)
        {
            if (geomNetworks == null) { geomNetworks = new Dictionary<string, IGeometricNetwork>(); }
            IEnumDataset enumDataset = ws.get_Datasets(esriDatasetType.esriDTFeatureDataset);
            enumDataset.Reset();

            IDataset dsName = enumDataset.Next();
            while (dsName != null)
            {
                IFeatureDataset featureDataset = dsName as IFeatureDataset;
                if (featureDataset != null)
                {
                    IEnumDataset geomDatasets = featureDataset.Subsets;
                    geomDatasets.Reset();
                    IDataset ds = null;
                    while ((ds = geomDatasets.Next()) != null)
                    {
                        if (ds is IGeometricNetwork)
                        {
                            geomNetworks.Add(ds.BrowseName.ToUpper(), ds as IGeometricNetwork);
                        }
                    }
                }
                dsName = enumDataset.Next();
            }
        }

        private void Trace(IMMFeederSource feederSource)
        {
            Stopwatch SW_CustomerCount = new Stopwatch();
            Stopwatch SW_TotalTrace = new Stopwatch();
            SW_TotalTrace.Start();

            IMMElectricNetworkTracing downstreamTracer = new MMFeederTracerClass();
            IMMNetworkAnalysisExtForFramework networkAnalysisExtForFramework = new MMNetworkAnalysisExtForFrameworkClass();
            IMMElectricTraceSettingsEx electricTraceSettings = new MMElectricTraceSettingsClass();

            try
            {
                ResultFeatureFactory resultFeatureFactory = new ResultFeatureFactory();

                electricTraceSettings.RespectESRIBarriers = true;
                electricTraceSettings.UseFeederManagerCircuitSources = true;
                electricTraceSettings.UseFeederManagerProtectiveDevices = true;

                //JM Only using DPS's for EnergizedZone trace
                //Set the object class ids for protective devices
                electricTraceSettings.ProtectiveDeviceClassIDs = GetDistributionProtectiveDeviceClassIds();

                //Set up the network analysis
                networkAnalysisExtForFramework.DrawComplex = false;
                networkAnalysisExtForFramework.SelectionSemantics = mmESRIAnalysisOptions.mmESRIAnalysisOnAllFeatures;

                IEnumNetEID junctions = null;
                IEnumNetEID edges = null;

                //Get the network id for the source feature
                string feederId = feederSource.FeederID.ToString();
                string substationName = GetSubstationName(feederSource);

                List<int> junctionClassIDsForDetails = new List<int>();
                List<int> lineClassIDsForDetails = new List<int>();

                if (!string.IsNullOrEmpty(Settings.Default.PointDetailsToInclude))
                {
                    string[] junctionClasses = Regex.Split(Settings.Default.PointDetailsToInclude, ",");
                    foreach (string junction in junctionClasses)
                    {
                        IFeatureClass featClass = Geodatabase.GetFeatureClass(junction);
                        junctionClassIDsForDetails.Add(featClass.ObjectClassID);
                    }
                }

                if (!string.IsNullOrEmpty(Settings.Default.LineDetailsToInclude))
                {
                    string[] edgeClasses = Regex.Split(Settings.Default.LineDetailsToInclude, ",");
                    foreach (string edge in edgeClasses)
                    {
                        IFeatureClass featClass = Geodatabase.GetFeatureClass(edge);
                        lineClassIDsForDetails.Add(featClass.ObjectClassID);
                    }
                }

                IFeatureClass featureClass = Geodatabase.GetFeatureClass(Settings.Default.ElectricStitchPointFeatureClassName);
                NetworkEntityFactory networkEntityFactory = new NetworkEntityFactory();
                NetworkEntity startNetworkEntity = null;
                IFeature sourceFeature = null;

                if (featureClass.FeatureClassID == feederSource.FeatureClassID)
                {
                    sourceFeature = featureClass.GetFeature(feederSource.OID);

                    if (sourceFeature != null)
                    {
                        //JM Create a barrier feature to set attributes and then create the starting network entity
                        BarrierFeatureFactory barrierFeatureFactory = new BarrierFeatureFactory();
                        BarrierFeature barrierFeature = barrierFeatureFactory.Create(sourceFeature);

                        startNetworkEntity = networkEntityFactory.Create(barrierFeature,
                            GeometricNetwork.Network);
                    }
                }

                downstreamTracer.FindDownstreamProtectiveDevices(GeometricNetwork, networkAnalysisExtForFramework,
                    electricTraceSettings, startNetworkEntity.Eid, esriElementType.esriETJunction, mmPhasesToTrace.mmPTT_Any,
                    out junctions, out edges);

                junctions.Reset();
                edges.Reset();

                //W want to get the related substation network stitch point so that we can trace upstream to the SubInterruptingDevice
                Guid subinterruptingDeviceGuid = GetSubInterruptingDeviceGuid(sourceFeature, startNetworkEntity);

                List<NetworkEntity> zoneStarts = new List<NetworkEntity>();
                //JM Get the zone starts depending on type of report (ENERGIZED - determine status of device)
                zoneStarts = GetZoneStarts(startNetworkEntity, junctions, ReportType);

                //start the zone count at 0
                int zone = 0;

                Logger.Debug("Feeder " + feederSource.FeederID + " zones: " + zoneStarts.Count, false);

                IFeatureClass fuseFeatureClass = Geodatabase.GetFeatureClass(Settings.Default.FuseFeatureClassName);
                IFeatureClass DPDFeatureClas = Geodatabase.GetFeatureClass(Settings.Default.DynamicProtectiveDeviceFeatureClassName);
                int DPDSubtypeFieldIdx = DPDFeatureClas.FindField("SUBTYPECD");
                foreach (var networkEntity in zoneStarts)
                {
                    //Determine downstream barriers to set.  Can't set any barriers upstream, or the trace will stop before it ever even gets to this start device
                    List<IMMNetworkFeatureID> DPDJunctionBarriers = new List<IMMNetworkFeatureID>();
                    List<IMMNetworkFeatureID> FuseJunctionBarriers = new List<IMMNetworkFeatureID>();

                    //JM Get the zone starts depending on type of report (ENERGIZED - determine status of device)
                    string listOfValues = string.Empty;
                    try
                    {
                        downstreamTracer.FindDownstreamProtectiveDevices(GeometricNetwork,
                            networkAnalysisExtForFramework,
                            electricTraceSettings, networkEntity.Eid, esriElementType.esriETJunction,
                            mmPhasesToTrace.mmPTT_Any,
                            out junctions, out edges);
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(
                            string.Format(
                                "Failed protective device downstream trace for object class ID {0} with GlobalID {1}. Error: {2} StackTrace: {3}",
                                networkEntity.ObjectClassId, networkEntity.GlobalId, ex.Message, ex.StackTrace));
                    }
                    junctions.Reset();

                    string[] listOfProtectiveDevices = GetDownstreamProtectiveDevices(junctions);
                    listOfValues = string.Join(",", listOfProtectiveDevices);

                    #region Protection Zone Trace

                    //Only set barrier features if this is a protection zone trace
                    if (ReportType == TraceReportType.ProtectionZone)
                    {
                        //Determine downstream barriers to set.  Can't set any barriers upstream, or the trace will stop before it ever even gets to this start device
                        networkAnalysisExtForFramework.JunctionBarriers = null;
                        
                        junctions.Reset();
                        edges.Reset();

                        //Query our junctions for all of the fuses and DPD (subtype 3 for recloser). Those will be the barriers
                        IEIDHelper eidHelper = new EIDHelperClass();
                        eidHelper.ReturnFeatures = true;
                        eidHelper.ReturnGeometries = false;
                        eidHelper.AddField("OBJECTID");
                        eidHelper.AddField("SUBTYPECD");
                        eidHelper.GeometricNetwork = GeometricNetwork;

                        DPDJunctionBarriers = new List<IMMNetworkFeatureID>();
                        FuseJunctionBarriers = new List<IMMNetworkFeatureID>();
                        IEnumEIDInfo junctionEIDInfoEnum = eidHelper.CreateEnumEIDInfo(junctions);
                        IEIDInfo junctionEIDInfo = null;
                        junctionEIDInfoEnum.Reset();
                        //IEnumEIDInfo junctionEIDInfo = eidHelper.CreateEnumEIDInfo(junctions);
                        while ((junctionEIDInfo = junctionEIDInfoEnum.Next()) != null)
                        {
                            if (junctionEIDInfo.EID == networkEntity.Eid)
                            {
                                continue;
                            }
                            int classID = ((IObjectClass)junctionEIDInfo.Feature.Table).ObjectClassID;
                            if (classID == DPDFeatureClas.ObjectClassID)
                            {
                                IRowSubtypes rowSubtype = junctionEIDInfo.Feature as IRowSubtypes;
                                if (rowSubtype.SubtypeCode == 3)
                                {
                                    //This is a recloser DPD
                                    IMMNetworkFeatureID networkFeatureId = new MMNetworkFeatureIDClass
                                    {
                                        FCID = DPDFeatureClas.FeatureClassID,
                                        OID = junctionEIDInfo.Feature.OID
                                    };
                                    DPDJunctionBarriers.Add(networkFeatureId);
                                }
                            }
                            else if (classID == fuseFeatureClass.ObjectClassID)
                            {
                                //This is a FUSE
                                IMMNetworkFeatureID networkFeatureId = new MMNetworkFeatureIDClass
                                {
                                    FCID = fuseFeatureClass.FeatureClassID,
                                    OID = junctionEIDInfo.Feature.OID
                                };
                                FuseJunctionBarriers.Add(networkFeatureId);
                            }
                        }
                    }

                    #endregion

                    //Trace this feature twice. Once for TAPS, once for NOTAPS
                    //JM
                    int numTraces = 2;

                    if (ReportType == TraceReportType.EnergizedZone)
                    {
                        numTraces = 1;
                    }

                    for (int i = 0; i < numTraces; i++)
                    {
                        if (networkEntity.Eid > 0)
                        {
                            //Get trace barriers but exclude the current start DPD
                            List<IMMNetworkFeatureID> junctionBarriers = DPDJunctionBarriers;

                            //Add the fuse junction barriers if this is for NOTAPS
                            if (i > 0) { junctionBarriers.AddRange(FuseJunctionBarriers); }

                            //Set our junction barriers
                            networkAnalysisExtForFramework.JunctionBarriers = junctionBarriers.ToArray();

                            //Do an initial trace that includes the *ALL* secondary conductors so that we can get the service locations for counts
                            networkAnalysisExtForFramework.DisabledFeatureLayerFCIDs = null;
                            downstreamTracer.TraceDownstream(GeometricNetwork, networkAnalysisExtForFramework,
                                electricTraceSettings, networkEntity.Eid, esriElementType.esriETJunction, mmPhasesToTrace.mmPTT_Any,
                                out junctions, out edges);

                            junctions.Reset();
                            edges.Reset();

                            //Report all of the features that are contained within this zone
                            IEIDHelper eidHelper = new EIDHelperClass();
                            eidHelper.AddField("GLOBALID");
                            eidHelper.GeometricNetwork = GeometricNetwork;
                            eidHelper.ReturnGeometries = false;
                            eidHelper.ReturnFeatures = true;
                            IEnumEIDInfo junctionInfo = null;
                            if (junctionClassIDsForDetails.Count > 0) { junctionInfo = eidHelper.CreateEnumEIDInfo(junctions); }
                            IEnumEIDInfo edgeInfo = null;
                            if (lineClassIDsForDetails.Count > 0) { edgeInfo = eidHelper.CreateEnumEIDInfo(edges); }

                            int lifeSupportCustomerCount = 0;
                            int essentialCustomerCount = 0;
                            int sensitiveCustomerCount = 0;
                            int DOMCustomerCount = 0;
                            int AGRCustomerCount = 0;
                            int INDCustomerCount = 0;
                            int COMCustomerCount = 0;
                            int OTHCustomerCount = 0;
                            int customerCount = 0;

                            SW_CustomerCount.Start();
                            //customerCount = resultFeatureFactory.GetCustomerCount(junctions,
                            //    GeometricNetwork.Network, out lifeSupportCustomerCount, out sensitiveCustomerCount, out essentialCustomerCount,
                            //    out DOMCustomerCount, out AGRCustomerCount, out INDCustomerCount, out COMCustomerCount, out OTHCustomerCount);
                            SW_CustomerCount.Stop();

                            //Exclude certain layers from the results.  Currently this is only the secondary OH and UG feature classes
                            networkAnalysisExtForFramework.DisabledFeatureLayerFCIDs = GetDisabledLayerFcids();

                            //Now trace downstream and exclude the secondary conductors
                            downstreamTracer.TraceDownstream(GeometricNetwork, networkAnalysisExtForFramework,
                                electricTraceSettings, networkEntity.Eid, esriElementType.esriETJunction, mmPhasesToTrace.mmPTT_Any,
                                out junctions, out edges);

                            junctions.Reset();
                            edges.Reset();

                            if (edges.Count > 0)
                            {
                                if (i == 0) { zone++; }

                                FireTierAndIndexInformation fireInformation = new FireTierAndIndexInformation();
                                resultFeatureFactory.GetLength(Settings.Default.PrimaryOverheadFeatureClassName, edges, GeometricNetwork.Network,
                                    ref fireInformation, true);
                                resultFeatureFactory.GetLength(Settings.Default.PrimaryUndergroundFeatureClassName, edges, GeometricNetwork.Network,
                                    ref fireInformation, false);

                                //Create the trace buffer polygon
                                IMMTraceBuffer mmTraceBuffer = new TraceBuffer();
                                edges.Reset();
                                IPolygon polygon = mmTraceBuffer.CreateBufferPolygon(GeometricNetwork, edges, new EnumNetEIDArrayClass(), false);

                                FireTierFeatureClass = Geodatabase.GetFeatureClass(Settings.Default.FireTierFeatureClassName);
                                string fireTier =
                                    resultFeatureFactory.GetFireTier(FireTierFeatureClass, polygon, Settings.Default.FireTierFieldName);

                                //Create a new result feature
                                ResultFeature resultFeature = new ResultFeature
                                {
                                    Zone = zone,
                                    CircuitId = feederSource.FeederID.ToString(),
                                    CircuitName = substationName,
                                    PrimaryOhMiles = fireInformation.GetPrimaryMiles(true),
                                    PrimaryUgMiles = fireInformation.GetPrimaryMiles(false),
                                    T1PrimaryOhMiles = fireInformation.OHMilesPerFireTier[1],
                                    T2PrimaryOhMiles = fireInformation.OHMilesPerFireTier[2],
                                    T3PrimaryOhMiles = fireInformation.OHMilesPerFireTier[3],
                                    T1PrimaryUgMiles = fireInformation.UGMilesPerFireTier[1],
                                    T2PrimaryUgMiles = fireInformation.UGMilesPerFireTier[2],
                                    T3PrimaryUgMiles = fireInformation.UGMilesPerFireTier[3],
                                    FireIndex = fireInformation.FireIndex,
                                    DeviceGlobalId = networkEntity.GlobalId,
                                    DeviceOperatingNumber = networkEntity.OperatingNumber,
                                    SubInterruptingDeviceGuid = subinterruptingDeviceGuid,
                                    Status = networkEntity.Status.ToString(),
                                    Position = networkEntity.NormalPosition.ToString(),
                                    ZoneType = "TAPS",
                                    Polygon = polygon,
                                    FireTier = fireTier,
                                    CustomersInZone = customerCount,
                                    CustomersLifeSupportInZone = lifeSupportCustomerCount,
                                    CustomersSensitiveInZone = sensitiveCustomerCount,
                                    CustomersEssentialInZone = essentialCustomerCount,
                                    ListOfDpds = listOfValues,
                                    CustomersDOMDInZone = DOMCustomerCount,
                                    CustomersAGRInZone = AGRCustomerCount,
                                    CustomersINDInZone = INDCustomerCount,
                                    CustomersOTHInZone = OTHCustomerCount,
                                    CustomersCOMInZone = COMCustomerCount
                                };

                                resultFeature.SetIndexInformation(fireInformation);

                                if (i > 0) { resultFeature.ZoneType = "NOTAPS"; }

                                //Save device type
                                resultFeature.DeviceType = zone == 1 ? "CB" : "LR";

                                string zoneFeatureGuid = "";
                                //JM
                                //Save result feature
                                if (ReportType == TraceReportType.EnergizedZone)
                                {
                                    //Write result to csv
                                    ZoneFeatureClass =
                                        Geodatabase.GetFeatureClass(Settings.Default.ZoneEnergizedFeatureClassName);
                                    zoneFeatureGuid = resultFeatureFactory.Create(ZoneFeatureClass, resultFeature);
                                    //Now report on the details for this zone
                                    resultFeatureFactory.CacheDetailFeatures(junctionClassIDsForDetails, lineClassIDsForDetails, zoneFeatureGuid, junctionInfo, edgeInfo);
                                    //resultFeatureFactory.WriteFeatureToCsv(resultFeature, ReportFileLocation,
                                    //    ReportFileName);
                                }
                                else
                                {
                                    ZoneFeatureClass =
                                        Geodatabase.GetFeatureClass(Settings.Default.ZoneFeatureClassName);
                                    zoneFeatureGuid = resultFeatureFactory.Create(ZoneFeatureClass, resultFeature);
                                    //Now report on the details for this zone
                                    resultFeatureFactory.CacheDetailFeatures(junctionClassIDsForDetails, lineClassIDsForDetails, zoneFeatureGuid, junctionInfo, edgeInfo);
                                }
                            }
                        }
                    }
                }

                ZoneDetailsTable = null;
                if (ReportType == TraceReportType.EnergizedZone) { ZoneDetailsTable = Geodatabase.GetObjectClass(Settings.Default.ZoneEnergizedDetailsFeatureClassName); }
                else if (ReportType == TraceReportType.ProtectionZone) { ZoneDetailsTable = Geodatabase.GetObjectClass(Settings.Default.ZoneDetailsFeatureClassName); }
                resultFeatureFactory.Create(ZoneDetailsTable as ITable);
            }
            catch (Exception ex)
            {
#if DEBUG
                //Debugger.Launch();
                //Debugger.Break();
#endif
                string error = string.Format("An error occurred while tracing Circuit {0}: Error {1}: StackTrace {2}", feederSource.FeederID.ToString(), ex.Message, ex.StackTrace);
                Logger.Error(error, true);
                throw new Exception(error);
            }
            finally
            {
                if (downstreamTracer != null) { while (Marshal.ReleaseComObject(downstreamTracer) > 0);}
                if (networkAnalysisExtForFramework != null) { while (Marshal.ReleaseComObject(networkAnalysisExtForFramework) > 0);}
                if (electricTraceSettings != null) { while (Marshal.ReleaseComObject(electricTraceSettings) > 0);}
            }

            SW_TotalTrace.Stop();

            string CountTime = SW_CustomerCount.Elapsed.ToString();
            string totalTime = SW_TotalTrace.Elapsed.ToString();
        }

        private string[] GetDownstreamProtectiveDevices(IEnumNetEID junctions)
        {
            List<string> returnString = new List<string>();

            BarrierFeatureFactory barrierFeatureFactory = new BarrierFeatureFactory();
            List<BarrierFeature> barrierFeatures = barrierFeatureFactory.Create(junctions, GeometricNetwork.Network, Settings.Default.DynamicProtectiveDeviceFeatureClassName);

            foreach (var barrierFeature in barrierFeatures)
            {
                if (ReportType == TraceReportType.ProtectionZone)
                {
                    if (barrierFeature.IsRecloser == true)
                    {
                        if (barrierFeature.Status == 5)
                        {
                            returnString.Add(barrierFeature.OperatingNumber);
                        }
                    }
                }
                else if (ReportType == TraceReportType.EnergizedZone)
                {
                    if (barrierFeature.Status == 5 && !barrierFeature.CustomerOwned)
                    {
                        returnString.Add(barrierFeature.OperatingNumber);
                    }
                }
            }

            return returnString.ToArray();
        }

        private Guid GetSubInterruptingDeviceGuid(IFeature sourceFeature, NetworkEntity ElectricStitchPoint)
        {

            Guid subInterruptingDeviceGuid = Guid.Empty;

            /*
            //Obtain the related substation stitch point
            IFeatureClass SubElecStitchPointFeatClass = Geodatabase.GetFeatureClass(Settings.Default.SubElectricStitchPointTableName);
            int subGlobalIdIdx = SubElecStitchPointFeatClass.FindField("GLOBALID");
            IQueryFilter qf = new QueryFilterClass();
            qf.WhereClause = string.Format("GLOBALID = '{0}'", ElectricStitchPoint.GlobalId.ToString("B").ToUpper());
            IFeatureCursor subStitchCursor = SubElecStitchPointFeatClass.Search(qf, false);
            IFeature subElectricStitch = subStitchCursor.NextFeature();
            if (subElectricStitch == null)
            {
                //Try a spatial query instead
                ISpatialFilter sf = new SpatialFilterClass();
                sf.Geometry = ((ITopologicalOperator)sourceFeature.ShapeCopy).Buffer(double.Parse(Settings.Default.SubStitchToElecStitchBuffer));
                sf.GeometryField = "SHAPE";
                sf.SearchOrder = esriSearchOrder.esriSearchOrderSpatial;
                subStitchCursor = SubElecStitchPointFeatClass.Search(sf, false);
                subElectricStitch = subStitchCursor.NextFeature();
            }

            if (subElectricStitch != null)
            {
                ISimpleJunctionFeature subElectricStitchJunction = subElectricStitch as ISimpleJunctionFeature;

                //Now that we have the related stitch point we can obtain the upstream sub interrupting device
                IPropertySet searchParameters = this.FetchTraceProperties(subElectricStitchJunction.EID, subElectricStitch, SubGeometricNetwork);
                IMMSearchConfiguration iMMSearchConfiguration = new SearchConfiguration();
                iMMSearchConfiguration.SearchParameters = searchParameters;
                ElectricNextUpstreamProtectiveDevice electricNextUpstreamProtectiveDevice = new ElectricNextUpstreamProtectiveDevice();
                electricNextUpstreamProtectiveDevice.TraceCurrentStatus = null;
                IMMEidSearchResults iMMEidSearchResults = electricNextUpstreamProtectiveDevice.Find(iMMSearchConfiguration, null) as IMMEidSearchResults;
                iMMEidSearchResults.Junctions.Reset();

                //Query our junctions for all of the fuses and DPD (subtype 3 for recloser). Those will be the barriers
                IEIDHelper eidHelper = new EIDHelperClass();
                eidHelper.ReturnFeatures = true;
                eidHelper.ReturnGeometries = false;
                eidHelper.AddField("GLOBALID");
                eidHelper.ReturnGeometries = false;
                eidHelper.ReturnFeatures = true;
                eidHelper.GeometricNetwork = GeometricNetwork;
                IEnumEIDInfo junctionInformation = eidHelper.CreateEnumEIDInfo(iMMEidSearchResults.Junctions);

                junctionInformation.Reset();
                IEIDInfo junctionInfo = null;
                while ((junctionInfo = junctionInformation.Next()) != null)
                {
                    int globalIDIdx = junctionInfo.Feature.Class.Fields.FindField("GLOBALID");
                    string globalID = junctionInfo.Feature.get_Value(globalIDIdx).ToString();
                    subInterruptingDeviceGuid = new Guid(globalID);
                }
            }
            */

            //ElectricUpstreamProtectiveDeviceTraceResults electricUpstreamProtectiveDeviceTraceResults = new ElectricUpstreamProtectiveDeviceTraceResults();

            IMMElectricNetworkTracing downstreamTracer = new MMFeederTracerClass();
            IMMNetworkAnalysisExtForFramework networkAnalysisExtForFramework = new MMNetworkAnalysisExtForFrameworkClass();
            IMMElectricTraceSettingsEx electricTraceSettings = new MMElectricTraceSettingsClass();

            try
            {
                IFeatureClass SubElecStitchPointFeatClass = Geodatabase.GetFeatureClass(Settings.Default.SubElectricStitchPointTableName);
                int subGlobalIdIdx = SubElecStitchPointFeatClass.FindField("GLOBALID");
                IQueryFilter qf = new QueryFilterClass();
                qf.WhereClause = string.Format("GLOBALID = '{0}'", ElectricStitchPoint.GlobalId.ToString("B").ToUpper());
                IFeatureCursor subStitchCursor = SubElecStitchPointFeatClass.Search(qf, false);
                IFeature subElectricStitch = subStitchCursor.NextFeature();
                if (subElectricStitch == null)
                {
                    //Try a spatial query instead
                    ISpatialFilter sf = new SpatialFilterClass();
                    sf.Geometry = ((ITopologicalOperator)sourceFeature.ShapeCopy).Buffer(double.Parse(Settings.Default.SubStitchToElecStitchBuffer));
                    sf.GeometryField = "SHAPE";
                    sf.SearchOrder = esriSearchOrder.esriSearchOrderSpatial;
                    sf.SpatialRel = esriSpatialRelEnum.esriSpatialRelContains;
                    subStitchCursor = SubElecStitchPointFeatClass.Search(sf, false);
                    subElectricStitch = subStitchCursor.NextFeature();
                }

                if (subElectricStitch != null)
                {
                    ISimpleJunctionFeature subElectricStitchJunction = subElectricStitch as ISimpleJunctionFeature;


                    electricTraceSettings.RespectESRIBarriers = true;
                    electricTraceSettings.UseFeederManagerCircuitSources = true;
                    electricTraceSettings.UseFeederManagerProtectiveDevices = true;

                    //Set the object class ids for protective devices
                    electricTraceSettings.ProtectiveDeviceClassIDs = GetSubstationProtectiveDeviceClassId();

                    //Set up the network analysis
                    networkAnalysisExtForFramework.DrawComplex = false;
                    networkAnalysisExtForFramework.SelectionSemantics = mmESRIAnalysisOptions.mmESRIAnalysisOnAllFeatures;

                    IEnumNetEID junctions = null;
                    IEnumNetEID edges = null;

                    downstreamTracer.TraceUpstream(SubGeometricNetwork, networkAnalysisExtForFramework,
                        electricTraceSettings, subElectricStitchJunction.EID, esriElementType.esriETJunction, mmPhasesToTrace.mmPTT_Any,
                        out junctions, out edges);

                    junctions.Reset();
                    edges.Reset();

                    //Query our junctions for all of the fuses and DPD (subtype 3 for recloser). Those will be the barriers
                    IEIDHelper eidHelper = new EIDHelperClass();
                    eidHelper.ReturnFeatures = true;
                    eidHelper.ReturnGeometries = false;
                    eidHelper.AddField("GLOBALID");
                    eidHelper.ReturnGeometries = false;
                    eidHelper.ReturnFeatures = true;
                    eidHelper.GeometricNetwork = SubGeometricNetwork;
                    IEnumEIDInfo junctionInformation = eidHelper.CreateEnumEIDInfo(junctions);

                    junctionInformation.Reset();
                    IEIDInfo junctionInfo = null;
                    while ((junctionInfo = junctionInformation.Next()) != null)
                    {
                        string featureClassName = ((IDataset)junctionInfo.Feature.Class).BrowseName;
                        if (featureClassName.ToUpper() == Settings.Default.SubInterruptingDeviceTableName.ToUpper())
                        {
                            int globalIDIdx = junctionInfo.Feature.Class.Fields.FindField("GLOBALID");
                            string globalID = junctionInfo.Feature.get_Value(globalIDIdx).ToString();
                            subInterruptingDeviceGuid = new Guid(globalID);
                        }
                    }

                }
            }
            catch (Exception ex)
            {
#if DEBUG
                //Debugger.Launch();
                //Debugger.Break();
#endif
                string error = string.Format("Failed to determine the related substation stitch point during processing. Message: {0} StackTrace: {1}", ex.Message, ex.StackTrace);
                Logger.Error(error, true);
                throw new Exception(error);
            }
            finally
            {
                if (downstreamTracer != null) { while (Marshal.ReleaseComObject(downstreamTracer) > 0);}
                if (networkAnalysisExtForFramework != null) { while (Marshal.ReleaseComObject(networkAnalysisExtForFramework) > 0);}
                if (electricTraceSettings != null) { while (Marshal.ReleaseComObject(electricTraceSettings) > 0);}
            }

            return subInterruptingDeviceGuid;
        }

        private IPropertySet FetchTraceProperties(int startEID, IFeature startFeature, IGeometricNetwork geometricNetwork)
        {
            IMMElectricTraceSettingsEx value = this.FetchElectricTraceSettings();
            IPropertySet propertySet = new PropertySetClass();
            propertySet.SetProperty(TraceProperties.GeometricNetwork, geometricNetwork);
            propertySet.SetProperty(TraceProperties.TraceSettings, value);
            propertySet.SetProperty(TraceProperties.StartEid, startEID);
            propertySet.SetProperty(TraceProperties.StartElementType, 2);
            propertySet.SetProperty(TraceProperties.PhasesToTrace, mmPhasesToTrace.mmPTT_Any);
            propertySet.SetProperty(TraceProperties.TraceEnds, null);
            propertySet.SetProperty(TraceProperties.IncludeEdges, false);
            propertySet.SetProperty(TraceProperties.IncludeJunctions, true);
            propertySet.SetProperty(TraceProperties.DrawGraphics, false);
            propertySet.SetProperty(TraceProperties.DrawColor, null);
            propertySet.SetProperty(TraceProperties.DrawComplex, true);
            return propertySet;
        }

        private IMMElectricTraceSettingsEx FetchElectricTraceSettings()
        {
            IMMElectricTraceSettingsEx iMMElectricTraceSettingsEx = new MMElectricTraceSettingsClass();
            try
            {
                iMMElectricTraceSettingsEx.UseFeederManagerProtectiveDevices = false;
                iMMElectricTraceSettingsEx.ProtectiveDeviceClassIDs = GetSubstationProtectiveDeviceClassId();
                iMMElectricTraceSettingsEx.RespectESRIBarriers = true;
                iMMElectricTraceSettingsEx.RespectEnabledField = false;
                return iMMElectricTraceSettingsEx;
            }
            catch
            {
                iMMElectricTraceSettingsEx.UseFeederManagerProtectiveDevices = false;
                iMMElectricTraceSettingsEx.RespectESRIBarriers = true;
                iMMElectricTraceSettingsEx.RespectEnabledField = true;
                return iMMElectricTraceSettingsEx;
            }
        }

        private string GetSubstationName(IMMFeederSource feederSource)
        {
            string feederName = feederSource.FeederName;
            string substationName = feederName;

            ITable table = Geodatabase.GetTable(Settings.Default.CircuitSourceTableName);

            if (table != null)
            {
                //Get the substation name
                string whereClause = FilterUtil.CreateSimpleWhereClause(table,
                    Settings.Default.SubstationIdFieldName,
                    feederSource.SubstationID);
                if (string.IsNullOrEmpty(whereClause) == false)
                {
                    IQueryFilter queryFilter = FilterUtil.CreateQueryFilter(table, whereClause);

                    ICursor cursor = table.Search(queryFilter, false);
                    IRow row = cursor.NextRow();
                    if (row != null)
                    {
                        object substationValue = EsriFieldsUtil.GetFieldValue((IObject)row,
                            Settings.Default.SubstationNameFieldName);
                        if (substationValue != null)
                        {
                            substationName = substationValue.ToString() + " " + feederName;
                        }
                        while (Marshal.ReleaseComObject(row) > 0) { }
                    }
                    if (queryFilter != null) { while (Marshal.ReleaseComObject(queryFilter) > 0) { } }
                    if (cursor != null) { while (Marshal.ReleaseComObject(cursor) > 0) { } }
                }
            }

            return substationName;
        }

        //JM
        private List<NetworkEntity> GetZoneStarts(NetworkEntity startZoneNetworkEntity, IEnumNetEID junctions, TraceReportType reportType = TraceReportType.ProtectionZone)
        {
            BarrierFeatureFactory barrierFeatureFactory = new BarrierFeatureFactory();
            List<BarrierFeature> barrierFeatures = barrierFeatureFactory.Create(junctions, GeometricNetwork.Network, Settings.Default.DynamicProtectiveDeviceFeatureClassName);

            NetworkEntityFactory networkEntityFactory = new NetworkEntityFactory();

            List<NetworkEntity> networkEntities = new List<NetworkEntity>();
            networkEntities.Add(startZoneNetworkEntity);

            foreach (var barrierFeature in barrierFeatures)
            {
                NetworkEntity networkEntity = null;

                if (reportType == TraceReportType.ProtectionZone)
                {
                    if (barrierFeature.IsRecloser == true)
                    {
                        networkEntity = networkEntityFactory.Create(barrierFeature,
                            GeometricNetwork.Network);
                    }
                }
                else if (reportType == TraceReportType.EnergizedZone)
                {
                    if (barrierFeature.Status == 5 && !barrierFeature.CustomerOwned)
                    {
                        networkEntity = networkEntityFactory.Create(barrierFeature,
                            GeometricNetwork.Network);
                    }
                }

                if (networkEntity != null)
                {
                    if (networkEntity.Eid != startZoneNetworkEntity.Eid)
                    {
                        networkEntities.Add(networkEntity);
                    }
                }
            }

            return networkEntities;
        }

        private List<IMMNetworkFeatureID> GetTraceBarriers(string circuitId, string featureClassName, string barrierWhereClause, NetworkEntity networkEntity)
        {
            List<IMMNetworkFeatureID> networkFeatureIds = new List<IMMNetworkFeatureID>();

            IFeatureClass featureClass = Geodatabase.GetFeatureClass(featureClassName);
            if (featureClass != null)
            {
                string whereClause = FilterUtil.CreateSimpleWhereClause(featureClass,
                    Settings.Default.CircuitIdFieldName, circuitId);

                if (string.IsNullOrEmpty(whereClause) == false)
                {
                    if (string.IsNullOrEmpty(barrierWhereClause) == false)
                    {
                        whereClause = whereClause + " AND " + barrierWhereClause;
                    }
                }

                GetFeatureBarriers(ref networkFeatureIds, featureClass, whereClause, networkEntity);
            }

            return networkFeatureIds;
        }

        private void GetFeatureBarriers(ref List<IMMNetworkFeatureID> networkFeatureIds, IFeatureClass featureClass, string whereClause, NetworkEntity networkEntity)
        {
            bool checkForExclusion = false;
            int excludeOid = 0;
            int excludeFcid = 0;

            if (networkEntity != null)
            {
                checkForExclusion = true;
                excludeFcid = networkEntity.ObjectClassId;
                excludeOid = networkEntity.ObjectId;
            }

            IQueryFilter queryFilter = FilterUtil.CreateQueryFilter(featureClass, whereClause);
            IFeatureCursor featureCursor = featureClass.Search(queryFilter, false);
            IFeature feature = featureCursor.NextFeature();
            while (feature != null)
            {
                IMMNetworkFeatureID networkFeatureId = null;

                if (checkForExclusion == true)
                {
                    if ((featureClass.FeatureClassID == excludeFcid && feature.OID == excludeOid) == false)
                    {
                        networkFeatureId = new MMNetworkFeatureIDClass
                        {
                            FCID = featureClass.FeatureClassID,
                            OID = feature.OID
                        };
                    }
                }
                else
                {
                    networkFeatureId = new MMNetworkFeatureIDClass
                    {
                        FCID = featureClass.FeatureClassID,
                        OID = feature.OID
                    };
                }

                if (networkFeatureId != null)
                {
                    networkFeatureIds.Add(networkFeatureId);
                }

                while (Marshal.ReleaseComObject(feature) > 0) { }
                feature = featureCursor.NextFeature();

            }
            if (queryFilter != null) { while (Marshal.ReleaseComObject(queryFilter) > 0) { } }
            if (featureCursor != null) { while (Marshal.ReleaseComObject(featureCursor) > 0) { } }
        }

        private Int32[] GetDisabledLayerFcids()
        {
            List<Int32> fcidList = new List<Int32>();

            fcidList.Add(GetObjectClassId("EDGIS.SecUGConductor"));
            fcidList.Add(GetObjectClassId("EDGIS.SecOHConductor"));

            return fcidList.ToArray();
        }

        private Int32[] GetSubstationProtectiveDeviceClassId()
        {
            List<Int32> objectClassIds = new List<Int32>();

            int objectClassId = GetObjectClassId(Settings.Default.DynamicProtectiveDeviceFeatureClassName);
            objectClassIds.Add(objectClassId);
            objectClassId = GetObjectClassId(Settings.Default.SwitchFeatureClassName);
            objectClassIds.Add(objectClassId);
            objectClassId = GetObjectClassId(Settings.Default.FuseFeatureClassName);
            objectClassIds.Add(objectClassId);

            return objectClassIds.ToArray();
        }

        private Int32[] GetDistributionProtectiveDeviceClassIds()
        {
            List<Int32> objectClassIds = new List<Int32>();

            int objectClassId = GetObjectClassId(Settings.Default.DynamicProtectiveDeviceFeatureClassName);
            objectClassIds.Add(objectClassId);
            objectClassId = GetObjectClassId(Settings.Default.SwitchFeatureClassName);
            objectClassIds.Add(objectClassId);
            objectClassId = GetObjectClassId(Settings.Default.FuseFeatureClassName);
            objectClassIds.Add(objectClassId);

            return objectClassIds.ToArray();
        }
        
        private IMMFeederExt GetFeederSpace(IMMFeederExt feederExt, ref IMMFeederSpace feederSpace)
        {
            Type type = Type.GetTypeFromProgID("mmFramework.MMFeederExt");
            object obj = Activator.CreateInstance(type);

            feederExt = obj as IMMFeederExt;
            feederSpace = feederExt.get_FeederSpace(GeometricNetwork, false);
            if (feederSpace == null)
            {
                throw new Exception("Unable to get feeder space information from geometric network");
            }
            return feederExt;
        }

        private int GetObjectClassId(string featureClassName)
        {
            IFeatureClass featClass = Geodatabase.GetFeatureClass(featureClassName);
            if (featClass != null)
            {
                return featClass.ObjectClassID;
            }

            return 0;
        }
    }

    internal enum TraceReportType
    {
        ProtectionZone,
        EnergizedZone
    }
}
