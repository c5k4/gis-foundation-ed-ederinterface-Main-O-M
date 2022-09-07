using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using PGE.Interface.Integration.DMS.Common;
using System.Diagnostics;
using System.IO;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.DataSourcesGDB;
using log4net;
using System.Data;
using Oracle.DataAccess.Client;
using System.Security.Principal;
using PGE.Common.Delivery.Diagnostics;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.Runtime.InteropServices;
using ESRI.ArcGIS.Geometry;
using PGE_DBPasswordManagement;

namespace PGE.Interface.Integration.DMS.Manager
{
    public class ExtractorManager
    {
        private LicenseInitializer _AOLicenseInitializer;
        private List<Process> ElectricProcessesRunning = new List<Process>();
        private int ElectricCircuitsToProcess = 0;
        private int SubstationCircuitsToProcess = 0;
        private ControlTable _controlTable;
        private string _batchID;
        private List<string> _circuitIDs;
        private List<string> _AlreadyProcessedCircuitIDs = new List<string>();
        private List<string> _substationIDs;
        private List<string> _AlreadyProcessedSubstationIDs = new List<string>();
        private static string dir = DateTime.Now.ToString("yyyyMMddHHmmss");
        Dictionary<string, List<string>> ProcessIDListByType = new Dictionary<string, List<string>>();
        Dictionary<string, List<string>> CircuitsToProcess = new Dictionary<string, List<string>>();
       
        private int maxProcessors = 0;
        

       
        //private IMMAppInitialize _MMAppInitialize;
        /// <summary>
        /// Logger to log error / debug/ user information
        /// </summary>
        private static Log4NetLogger _logger = new Log4NetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, "ED50.log4net.config");


       /// <summary>
       /// Initialize the ExtractorManager data structures
       /// </summary>
        public ExtractorManager()
        {
            _controlTable = new ControlTable(Configuration.CadopsConnection);
            _circuitIDs = new List<string>();
            _substationIDs = new List<string>();
        }
        /// <summary>
        /// The main entry point for the ExtractorManager
        /// </summary>
        /// <param name="msg">The message with input parameters</param>
        public int Process(ExtractorMessage msg)
        {
            int returncode = 0;
            DateTime startTime = DateTime.Now;
            dir = startTime.ToString("yyyyMMddHHmmss");
            // M4JF
            // IWorkspace EDWkspace = GetEDWorkSpace();
            IWorkspace EDWkspace = GetEDERWorkSpace();           
            IWorkspace SUBWkspace = GetSUBWorkSpace();
            CircuitFinder finder = new CircuitFinder(EDWkspace, SUBWkspace);

            string location = System.Reflection.Assembly.GetExecutingAssembly().Location;
            string statusLogDirectory = location.Substring(0, location.LastIndexOf("\\") + 1) + "StatusLogs";
            if (!Directory.Exists(statusLogDirectory)) { Directory.CreateDirectory(statusLogDirectory); }

            foreach (string file in Directory.GetFiles(statusLogDirectory))
            {
                try
                {
                    File.Delete(file);
                }
                catch (Exception e) { /* Don't care if we fail. This is solely to keep the directory under control. if it doens't get cleaned up this time, it will next*/}
            }

            _logger.Debug("Caching subtype mapping for child processes");

            // m4jf edgisrearch 388 - commented below lines as it was it was not referenced anywhere referenced 
            //Create a new subtypes so that it caches this to disk.  This saves several minutes for every process
            //Subtypes _EDSubtypes = new Subtypes(EDWkspace, false, true, "EDSubtypeMapping.txt", "EDFCIDMapping.txt", "EDDomainsmapping.txt");
            //Subtypes _SUBSubtypes = new Subtypes(SUBWkspace, false, true, "SUBSubtypeMapping.txt", "SUBFCIDMapping.txt", "SUBDomainsmapping.txt");
            _logger.Debug("Finished caching subtype mapping for child processes");

            msg.ProcessID = Guid.NewGuid().ToString();

            // #2
            maxProcessors = Configuration.getIntSetting("MaximumProcesses", -1);
            // #3
            _controlTable.InsertProcessIDNotification(maxProcessors, System.Environment.MachineName);

            // #4
            ProcessMessage(finder, msg, _controlTable);


            ExportProgress exportProgressForm = new ExportProgress();

            //monitor control/stage table for process finishing
            bool processFinished = false;
            bool processError = false;
            //int maxWaitTimeInSeconds = Configuration.getIntSetting("MaximumWaitTimeInSeconds", -1);
            //int checkInterval = Configuration.getIntSetting("CheckInterval", 30) * 1000;
            DateTime delay = DateTime.Now;
            while (!processFinished)
            {
                processFinished = true;

                try
                {
                    if (exportProgressForm == null) { exportProgressForm = new ExportProgress(); }
                    if (!exportProgressForm.Visible) { exportProgressForm.Show(); }
                    exportProgressForm.UpdateProgress(ProcessIDListByType, _controlTable, _batchID);
                    //Not the most graceful or desired way to have the form update, but this will not harm anything in this particular application
                    Application.DoEvents();
                }
                catch (Exception e) { }

                // #12
                StartNewProcessesIfNeeded(msg);

                //Ensure that any circuits that are assigned, the processes are in fact still running
                ReallocateCircuits();

                //Update total server progress
                UpdateServerProgress();

                if (processFinished)
                {
                    foreach (Process p in ElectricProcessesRunning)
                    {
                        if (!p.HasExited)
                        {
                            processFinished = false;
                            break;
                        }
                    }
                }

                //About to exit, last chance to check if there is any work this process still needs to do
                if (processFinished)
                {
                    if (_controlTable.CircuitsStillToProcess())
                    {
                        processFinished = false;
                    }
                }

                if (processFinished)
                {
                    //Last chance before we exit.  Let's verify that every circuit is either finished or errored
                    if (!_controlTable.AreProcessesFinished())
                    {
                        processFinished = false;
                    }
                }

                //We will run this check every 5 seconds
                Thread.Sleep(5000);
            }

            try
            {
                exportProgressForm.Close();
            }
            catch (Exception e) { }

            if (_controlTable.CanRunExport(System.Environment.MachineName))
            {
                //clean change tables if the export succeeded if the export is  change based
                if (msg.ExtractType == extracttype.Changes)
                {
                    if (processFinished && !processError)
                    {
                        finder.CleanChangeDetectionTables();
                    }
                }

                string exportErrors = CheckExport();
                //remove any duplicates before we export the data
                _controlTable.RemoveDuplicates();

                //Update the schematics xy data.  No longer necessary as we are directly connecting with the schematics database to determine
                //the xy and pathing information.
                //_controlTable.UpdateSchemXY();

                //check the data
                _logger.Debug("Beginning data validation");
                Stopwatch validateStopwatch = new Stopwatch();
                validateStopwatch.Start();
                string dataErrors = DataChecks.CheckData(Configuration.CadopsConnection);
                validateStopwatch.Stop();
                string time = validateStopwatch.Elapsed.ToString();
                _logger.Debug("Data validation took " + time + " to run.");
                ////write out file for the result            
                List<string> tableNames1 = Configuration.getCommaSeparatedList("ExportTables", new List<string>());
                bool exportSuccess = ExportToCSVfile(Configuration.CadopsConnection, tableNames1);
                DateTime endTime = DateTime.Now;

                //create the message with details about the export
                StringBuilder message = new StringBuilder();
                message.AppendLine(msg.ExtractType.ToString() + " Export started " + startTime.ToString());
                message.AppendLine("Circuits: " + _circuitIDs.Count + ", Substations: " + _substationIDs.Count);
                if (processFinished)
                {
                    message.AppendLine("Export finished " + endTime.ToString());
                }
                else
                {
                    message.AppendLine("Export did not finish in the allotted time.");
                }

                if (!string.IsNullOrEmpty(exportErrors))
                {
                    message.AppendLine("There were errors exporting.");
                    message.AppendLine(exportErrors);
                    returncode = (int)ReturnCode.DataError;
                }
                if (!string.IsNullOrEmpty(dataErrors))
                {
                    message.AppendLine("There were data errors in the export.");
                    message.AppendLine(dataErrors);
                    returncode = (int)ReturnCode.DataError;
                }
                //this needs to be last so the return code is 1
                if (processError)
                {
                    message.AppendLine("There was a process error.");
                    returncode = (int)ReturnCode.ProcessError;
                }
                _logger.Info(message.ToString());
                try
                {
                    File.WriteAllText(Configuration.Path + dir + "_log.txt", message.ToString());
                    if (!String.IsNullOrEmpty(Configuration.ArchivePath))
                    {
                        File.WriteAllText(Configuration.ArchivePath + dir + "_log.txt", message.ToString());
                    }
                }
                catch (Exception ex)
                {
                    _logger.Error("Error writing log file.", ex);
                }
                try
                {
                    string triggerfile = Configuration.Path + Configuration.TriggerFile;
                    System.IO.FileStream stream = File.Create(triggerfile);
                    stream.Dispose();
                }
                catch (Exception ex)
                {
                    _logger.Error("Error writing trigger file.", ex);
                }
            }
            //Console.ReadKey();
            return returncode;
        }

        private void UpdateServerProgress()
        {
            int circuitsExported = 0;
            int totalFeaturesExported = 0;
            _controlTable.CircuitsByStatusAndServerName(System.Environment.MachineName, CircuitStatus.Finished, ref circuitsExported, ref totalFeaturesExported);
            _controlTable.CircuitsByStatusAndServerName(System.Environment.MachineName, CircuitStatus.Error, ref circuitsExported, ref totalFeaturesExported);
            _controlTable.UpdateOverallServerStatus(System.Environment.MachineName, circuitsExported, totalFeaturesExported);
        }

        /// <summary>
        /// This method will check to ensure that a processID that is assigned to this server is still running if there are any
        /// circuits that have that processID and they are marked as in progress
        /// </summary>
        private void ReallocateCircuits()
        {
            Dictionary<string, List<string>> processIDCircuitMap = _controlTable.CircuitsByStatusAndServerName(System.Environment.MachineName, CircuitStatus.InProgress);
            foreach (Process p in ElectricProcessesRunning)
            {
                if (!p.HasExited)
                {
                    string processID = p.Id.ToString();
                    if (processIDCircuitMap.ContainsKey(processID)) { processIDCircuitMap.Remove(processID); }
                }
            }

            //whatever is left is what is considered in progress, but there is no process to process it.
            foreach (KeyValuePair<string, List<string>> kvp in processIDCircuitMap)
            {
                foreach (string circuit in kvp.Value)
                {
                    _controlTable.UpdateCircuitsAsFinished(circuit, 0, CircuitStatus.Retry, "");
                }
            }
        }

        /// <summary>
        /// This method will start new processes if some have finished and can be reallocated
        /// </summary>
        private void StartNewProcessesIfNeeded(ExtractorMessage msg)
        {
            // #12
            if (!_controlTable.CircuitsStillToProcess()) { return; }

            bool startElectricProcess = false;

            foreach (Process p in ElectricProcessesRunning)
            {
                if (p.HasExited)
                {
                    startElectricProcess = true;
                    ElectricProcessesRunning.Remove(p);
                    break;
                }
            }

            if (startElectricProcess) { StartNewWorker(msg.ServerName); }

            if ((ElectricProcessesRunning.Count) < maxProcessors)
            {
                //start another electric thread
                StartNewWorker(msg.ServerName);
            }
        }

        /// <summary>
        /// Reads the EXPORTED table and makes sure every CircuitID is present. 
        /// </summary>
        /// <returns>Each Circuit that had an error should be appended to the message or any circuits that are missing</returns>
        private string CheckExport()
        {

            DataTable exportedTable = GetData(Configuration.CadopsConnection, "select EXPORT_ID,EXPORT_TYPE,STATUS from DMSSTAGING.exported");

            //get the exported circuits and substations as list      
            List<string> exportedCircuitIDs = GetExportedIDs(exportedTable, "C");

            //get the circuits and substations that are exported but have errors as list            
            List<string> exportedCircuitIDsWithError = GetExportedIDs(exportedTable, "C", 0);


            //get the missing circuits and substations           
            List<string> missingCircuitIDs = _circuitIDs.Except(exportedCircuitIDs).ToList<string>();


            StringBuilder sb = new StringBuilder();
            if (exportedCircuitIDsWithError.Count > 0)
            {
                sb.AppendLine("Circuit exported with errors:");
                string str = string.Join(",", exportedCircuitIDsWithError.ToArray<string>());
                sb.AppendLine(str);
            }

            if (missingCircuitIDs.Count > 0)
            {
                sb.AppendLine("Circuit not exported:");
                string str = string.Join(",", missingCircuitIDs.ToArray<string>());
                sb.AppendLine(str);
            }

            List<string> exportedSubstationIDs = GetExportedIDs(exportedTable, "S");
            List<string> exportedSubstationIDsWithError = GetExportedIDs(exportedTable, "S", 0);
            List<string> missingSubstationIDs = _substationIDs.Except(exportedSubstationIDs).ToList<string>();
            if (exportedSubstationIDsWithError.Count > 0)
            {
                sb.AppendLine("Substation exported with errors:");
                string str = string.Join(",", exportedSubstationIDsWithError.ToArray<string>());
                sb.AppendLine(str);
            }

            if (missingSubstationIDs.Count > 0)
            {
                sb.AppendLine("Substation not exported:");
                string str = string.Join(",", missingSubstationIDs.ToArray<string>());
                sb.AppendLine(str);
            }


            return sb.ToString();
        }




        private List<string> GetExportedIDs(DataTable table, string exportType)
        {
            return GetExportedIDs(table, exportType, -1);
        }

       
        private List<string> GetExportedIDs(DataTable exportedTable, string exportType, int status)
        {
            string whereClause = string.Format("EXPORT_TYPE='{0}'", exportType);
            if (status != -1)
            {
                whereClause += string.Format(" and STATUS={0}", status);
            }

            DataRow[] results = exportedTable.Select(whereClause);

            if (results.Length > 0)
            {
                return results.Select(row => row.Field<string>("EXPORT_ID")).ToList<string>();
            }

            return new List<string>();
        }
        public bool GetLicenses()
        {

            //Get esri license 
            _AOLicenseInitializer = new LicenseInitializer();
            if (_AOLicenseInitializer.InitializeApplication(new esriLicenseProductCode[] { esriLicenseProductCode.esriLicenseProductCodeAdvanced }, new esriLicenseExtensionCode[] { }) == false)
            {
                return false;
            }


            return true;
        }

        public void ReleaseLicenses()
        {
            if (_AOLicenseInitializer != null)
            {
                _AOLicenseInitializer.ShutdownApplication();
            }

        }

        private void ExportTableToCSV(DataTable dt)
        {
            StringBuilder sb = new StringBuilder();

            string[] columnNames = dt.Columns.Cast<DataColumn>().
                                              Select(column => column.ColumnName).
                                              ToArray();
            sb.AppendLine(string.Join(",", columnNames));

            foreach (DataRow row in dt.Rows)
            {
                string[] fields = row.ItemArray.Select(field => field.ToString()).
                                                ToArray();
                sb.AppendLine(string.Join(",", fields));
            }

            File.WriteAllText(dt.TableName + ".csv", sb.ToString());
        }

         private DataTable GetData(string connectionString, string sql)
        {
            DataTable table = new DataTable();
            try
            {

                using (OracleConnection connection = new OracleConnection(connectionString))
                {
                    connection.Open();

                    OracleDataAdapter adapter = new OracleDataAdapter(
                     sql, connection);

                    adapter.FillSchema(table, SchemaType.Source);
                    adapter.Fill(table);

                    adapter.Dispose();
                    connection.Close();
                    connection.Dispose();
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Failed to getdata:" + ex.Message);
            }
            return table;
        }

        
        public static bool ExportToCSVfile(string connectionString, List<string> tableNames)
        {
            if (tableNames.Count == 0)
            {
                _logger.Error("No tables are configured for export.");
                return false;
            }
            // Connects to the database, and makes the select command.

            string schema = "dmsstaging";
            int bufferSize = Configuration.getIntSetting("BufferSizeInBytes", -1);
            string path = Configuration.Path;

            try
            {

                using (OracleConnection connection = new OracleConnection(connectionString))
                {
                    connection.Open();

                    foreach (string tableName in tableNames)
                    {

                        // Creates the CSV file as a stream, using the default encoding.
                        string fileName = path + dir + "_" + tableName + ".csv";

                        using (StreamWriter sw = new StreamWriter(fileName, false, Encoding.Default, bufferSize))
                        {
                            OracleCommand command = new OracleCommand(
                            string.Format("select * from {0}", schema + "." + tableName), connection);

                            // Creates a SqlDataReader instance to read data from the table.
                            OracleDataReader dr = command.ExecuteReader();

                            // Retrives the schema of the table.
                            DataTable dtSchema = dr.GetSchemaTable();

                            StringBuilder sb = new StringBuilder();

                            string[] columnNames = dtSchema.AsEnumerable()
                                .Select(row => row.Field<string>("ColumnName")).ToArray();

                            sb.AppendLine(string.Join(",", columnNames));

                            sw.Write(sb);
                            int numOfColumns = columnNames.Length;

                            //quote any fields that have csv tokens
                            char[] csvTokens = new[] { '\"', '^', '\n', '\r' };

                            while (dr.Read())
                            {
                                sb.Length = 0;


                                object[] aRowArray = new object[numOfColumns];
                                dr.GetValues(aRowArray);
                                string[] fields = aRowArray.Select(field => field.ToString()).
                                                                        ToArray();



                                for (int i = 0; i < fields.Length; i++)
                                {
                                    string field = fields[i];
                                    if (field.IndexOfAny(csvTokens) >= 0)
                                    {
                                        fields[i] = "\"" + field.Replace("\"", "\"\"") + "\"";
                                    }
                                }

                                sb.AppendLine(string.Join("^", fields));

                                sw.Write(sb);

                            }

                            dr.Close();
                            dr.Dispose();
                            dtSchema.Dispose();
                            command.Dispose();
                        }
                        //if an archive location is set copy the files there
                        if (!String.IsNullOrEmpty(Configuration.ArchivePath))
                        {
                            File.Copy(fileName, Configuration.ArchivePath + dir + "_" + tableName + ".csv");
                        }
                    }
                    connection.Close();
                    connection.Dispose();
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Export to csv failed:", ex);
                return false;
            }
            return true;
        }
        // m4jf
        private IWorkspace GetEDERWorkSpace()
        {
            try
            {
                // M4JF EDGISREARCH 388
                IPropertySet propertySet = new PropertySetClass();
                // string[] commaSepConnectionList = Configuration.getCommaSeparatedList("EDConnection", new string[3]);
                // m4jf edgisrearch 919
                string[] userInst = System.Configuration.ConfigurationManager.AppSettings["EDER_ConnectionStr_dmsstaging"].Split('@');
                string password = ReadEncryption.GetPassword(System.Configuration.ConfigurationManager.AppSettings["EDER_ConnectionStr_dmsstaging"].ToUpper());


                //string dbconn = "sde:oracle11g:" + commaSepConnectionList[0];
                //_logger.Debug("Using dbconn: " + dbconn);
                //propertySet.SetProperty("instance", dbconn);
                //propertySet.SetProperty("User", commaSepConnectionList[1]);
                //propertySet.SetProperty("Password", commaSepConnectionList[2]);
                string dbconn = "sde:oracle11g:" + userInst[1];
                _logger.Debug("Using dbconn: " + dbconn);
                propertySet.SetProperty("instance", dbconn);
                propertySet.SetProperty("User", userInst[0]);
                propertySet.SetProperty("Password", password);
                propertySet.SetProperty("version", "SDE.DEFAULT");

                IWorkspaceFactory workspaceFactory = new SdeWorkspaceFactory();
                IWorkspace wspace = workspaceFactory.Open(propertySet, 0);
                System.Runtime.InteropServices.Marshal.ReleaseComObject(workspaceFactory);
                return wspace;
            }
            catch (Exception ex)
            {
                _logger.Error("Error getting ED SDE workspace.", ex);
                return null;
            }

        }

        /// <summary>
        /// Retrieves the Database workspace by reading the database credentials from the config file.
        /// </summary>
        /// <returns>Returns the reference to the Database</returns>
        private IWorkspace GetEDWorkSpace()
        {
            try
            {
                // M4JF EDGISREARCH 388
                IPropertySet propertySet = new PropertySetClass();
                // string[] commaSepConnectionList = Configuration.getCommaSeparatedList("EDConnection", new string[3]);
                // m4jf edgisrearch 919
                string[] userInst = System.Configuration.ConfigurationManager.AppSettings["EDGMC_ConnectionStr"].Split('@');
                string password = ReadEncryption.GetPassword(System.Configuration.ConfigurationManager.AppSettings["EDGMC_ConnectionStr"].ToUpper());


                //string dbconn = "sde:oracle11g:" + commaSepConnectionList[0];
                //_logger.Debug("Using dbconn: " + dbconn);
                //propertySet.SetProperty("instance", dbconn);
                //propertySet.SetProperty("User", commaSepConnectionList[1]);
                //propertySet.SetProperty("Password", commaSepConnectionList[2]);
                string dbconn = "sde:oracle11g:" + userInst[1];
                _logger.Debug("Using dbconn: " + dbconn);
                propertySet.SetProperty("instance", dbconn);
                propertySet.SetProperty("User", userInst[0]);
                propertySet.SetProperty("Password", password);
                propertySet.SetProperty("version", "SDE.DEFAULT");

                IWorkspaceFactory workspaceFactory = new SdeWorkspaceFactory();
                IWorkspace wspace = workspaceFactory.Open(propertySet, 0);
                System.Runtime.InteropServices.Marshal.ReleaseComObject(workspaceFactory);
                return wspace;
            }
            catch (Exception ex)
            {
                _logger.Error("Error getting ED SDE workspace.", ex);
                return null;
            }

        }

        /// <summary>
        /// Retrieves the Database workspace by reading the database credentials from the config file.
        /// </summary>
        /// <returns>Returns the reference to the Database</returns>
        private IWorkspace GetSUBWorkSpace()
        {
            try
            {
                // M4JF EDGISREARCH 388
                IPropertySet propertySet = new PropertySetClass();
                //string[] commaSepConnectionList = Configuration.getCommaSeparatedList("SUBConnection", new string[3]);
                // M4JF EDGISREARCH 919
                string[] userInst = System.Configuration.ConfigurationManager.AppSettings["EDSUBGMC_ConnectionStr"].Split('@');
                //string dbconn = "sde:oracle11g:" + commaSepConnectionList[0];
                //_logger.Debug("Using dbconn: " + dbconn);
                //propertySet.SetProperty("instance", dbconn);
                //propertySet.SetProperty("User", commaSepConnectionList[1]);
                //propertySet.SetProperty("Password", commaSepConnectionList[2]);
                string password = ReadEncryption.GetPassword(System.Configuration.ConfigurationManager.AppSettings["EDSUBGMC_ConnectionStr"].ToUpper());
                string dbconn = "sde:oracle11g:" + userInst[1];
                _logger.Debug("Using dbconn: " + dbconn);
                propertySet.SetProperty("instance", dbconn);
                propertySet.SetProperty("User", userInst[0]);
                propertySet.SetProperty("Password", password);
                propertySet.SetProperty("version", "SDE.DEFAULT");

                IWorkspaceFactory workspaceFactory = new SdeWorkspaceFactory();
                IWorkspace wspace = workspaceFactory.Open(propertySet, 0);
                System.Runtime.InteropServices.Marshal.ReleaseComObject(workspaceFactory);
                return wspace;
            }
            catch (Exception ex)
            {
                _logger.Error("Error getting SUB SDE workspace.", ex);
                return null;
            }

        }

        private Process CreateWorker(ExtractorMessage msg)
        {
            // #14
            string AssemblyLocation = System.Reflection.Assembly.GetExecutingAssembly().Location;
            Process myProcess = null;
            string arguments = "";
            //myProcess.StartInfo.FileName = "PGE.Interface.Integration.DMS.Extractor.exe";
            //myProcess.StartInfo.UseShellExecute = false;
            //myProcess.StartInfo.CreateNoWindow = true;
            //myProcess.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            arguments += "-T " + ((int)msg.ExtractType);
            arguments += " -P " + msg.ProcessID;
            arguments += " -N \"" + msg.ServerName + "\"";

            AssemblyLocation = AssemblyLocation.Substring(0, AssemblyLocation.LastIndexOf("\\") + 1) + "PGE.Interface.Integration.DMS.Extractor.exe";
            ProcessStartInfo startInfo = new ProcessStartInfo(AssemblyLocation, arguments)
            {
                CreateNoWindow = true,
                WindowStyle = ProcessWindowStyle.Hidden,
                UseShellExecute = false,
            };

            try
            {
                //_logger.Debug("Spawning new child process.  Arguments: " + startInfo.Arguments);
                myProcess = System.Diagnostics.Process.Start(startInfo);

                //Place our processes in our list so we can monitor the progress of each
                ElectricProcessesRunning.Add(myProcess);
            }
            catch (Exception ex)//if exception occurred, does the control table need to be cleaned?
            {
                _logger.Error("Error creating Batch Process.", ex);
            }

            return myProcess;
        }

        private void ProcessMessage(CircuitFinder finder, ExtractorMessage msg, ControlTable controlTable)
        {
            // #5
            //process substations
            ProcessMessage(finder, msg, true);
            
            //process circuits
            ProcessMessage(finder, msg, false);

            Dictionary<int, int> subStitchToElecStitchMap = BuildSubtationToDistributionMap();

            controlTable.InsertCircuits(_substationIDs, _circuitIDs, subStitchToElecStitchMap, ref _batchID);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="SUBFeatWS"></param>
        /// <param name="EDFeatWS"></param>
        /// <returns></returns>
        private Dictionary<int, int> BuildSubtationToDistributionMap()
        {
            Dictionary<int, int> SubStitchToElecStitchMap = new Dictionary<int, int>();
            IFeatureWorkspace SUBFeatWS = null;
            IFeatureWorkspace EDFeatWS = null;
            IFeatureClass subElectricStitchPoint = null;
            IFeatureClass electricStitchPoint = null;
            IQueryFilter qf = new QueryFilter();
            IFeatureCursor SUBFeatCursor = null;
            IFeatureCursor relatedElecFeatures = null;
            IFeature elecFeature = null;
            ISpatialFilter spatialFilter = null;
            ITopologicalOperator topologicalOperator = null;
            try
            {
                SUBFeatWS = GetSUBWorkSpace() as IFeatureWorkspace;
                EDFeatWS = GetEDWorkSpace() as IFeatureWorkspace;
                subElectricStitchPoint = SUBFeatWS.OpenFeatureClass("EDGIS.SUBELECTRICSTITCHPOINT");
                electricStitchPoint = EDFeatWS.OpenFeatureClass("EDGIS.ELECTRICSTITCHPOINT");
                int searchDistance = 1;

                //Now query for all features such that the foreign key is not null
                qf = new QueryFilter();
                qf.SubFields = subElectricStitchPoint.OIDFieldName + "," + subElectricStitchPoint.ShapeFieldName;
                SUBFeatCursor = subElectricStitchPoint.Search(qf, true);
                List<string> elecStitchPointGUIDs = new List<string>();
                IFeature substationRow = null;
                while ((substationRow = SUBFeatCursor.NextFeature()) != null)
                {
                    string substationRowOID = substationRow.OID.ToString();
                    string distributionOID = "";

                    spatialFilter = new SpatialFilterClass();
                    topologicalOperator = substationRow.ShapeCopy as ITopologicalOperator;
                    spatialFilter.Geometry = topologicalOperator.Buffer(searchDistance);
                    spatialFilter.GeometryField = electricStitchPoint.ShapeFieldName;
                    spatialFilter.SubFields = electricStitchPoint.OIDFieldName;
                    spatialFilter.SpatialRel = esriSpatialRelEnum.esriSpatialRelContains;
                    relatedElecFeatures = electricStitchPoint.Search(spatialFilter, false);
                    int numRelated = 0;
                    while ((elecFeature = relatedElecFeatures.NextFeature()) != null)
                    {
                        try
                        {
                            numRelated++;
                            SubStitchToElecStitchMap.Add(substationRow.OID, elecFeature.OID);
                        }
                        catch (Exception e)
                        {
                            //If something fails here we can continue as the FeederSource will pick up anything
                            //that we may miss here, even if relationships are invalid.
                            _logger.Warn("Error building relationship for SubStitchPoint: " + substationRowOID +
                                " ElecStitchPoint: " + distributionOID + " Message: " + e.Message);
                        }

                        if (elecFeature != null) { while (Marshal.ReleaseComObject(elecFeature) > 0) { } }
                    }

                    if (relatedElecFeatures != null) { while (Marshal.ReleaseComObject(relatedElecFeatures) > 0) { } }
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Error determining substation stitch point to electric stitch point relationships.", ex);
                throw ex;
            }
            finally
            {
                if (qf != null) { while (Marshal.ReleaseComObject(qf) > 0) { } }
                if (spatialFilter != null) { while (Marshal.ReleaseComObject(spatialFilter) > 0) { } }
                if (topologicalOperator != null) { while (Marshal.ReleaseComObject(topologicalOperator) > 0) { } }
                if (elecFeature != null) { while (Marshal.ReleaseComObject(elecFeature) > 0) { } }
                if (relatedElecFeatures != null) { while (Marshal.ReleaseComObject(relatedElecFeatures) > 0) { } }
                if (SUBFeatCursor != null) { while (Marshal.ReleaseComObject(SUBFeatCursor) > 0) { } }
                if (subElectricStitchPoint != null) { while (Marshal.ReleaseComObject(subElectricStitchPoint) > 0) { } }
                if (electricStitchPoint != null) { while (Marshal.ReleaseComObject(electricStitchPoint) > 0) { } }
                if (SUBFeatWS != null) { while (Marshal.ReleaseComObject(SUBFeatWS) > 0) { } }
                if (EDFeatWS != null) { while (Marshal.ReleaseComObject(EDFeatWS) > 0) { } }
            }

            return SubStitchToElecStitchMap;
        }

        private void DetermineSubStitchToElecStitchRelationships()
        {
            IWorkspace EDWorkspace = null;
            IWorkspace SUBWorkspace = null;
            try
            {
                EDWorkspace = GetEDWorkSpace();
                SUBWorkspace = GetSUBWorkSpace();


            }
            catch
            {

            }
            finally
            {

            }
        }

        private void ProcessMessage(CircuitFinder finder, ExtractorMessage msg, bool isSubStation)
        {
            try
            {
              
                List<string> IDs;
                List<string> processIDs = new List<string>();
                //int minEntities;
                if (isSubStation)
                {
                    // #6
                    IDs = finder.FindSubstations(msg, ref SubstationCircuitsToProcess);
                    _logger.Debug("Found " + IDs.Count + " Substations");
                    _substationIDs = IDs;
                    //minEntities = Configuration.getIntSetting("MinimumSubstations", -1);
                }
                else//circuit
                {
                    // #9
                    IDs = finder.FindCircuits(msg, ref ElectricCircuitsToProcess);
                    ElectricCircuitsToProcess = IDs.Count;
                    _logger.Debug("Found " + IDs.Count + " Circuits");
                    _circuitIDs = IDs;
                    //minEntities = Configuration.getIntSetting("MinimumCircuits", -1);
                }
                int totals = IDs.Count;

                if (totals > 0)
                {
                    // m4jf edgisrearch 388 - Updating batch size of export 

                   // batchSize = totals / 10;
                    //determine the # of process to run            
                    maxProcessors = Configuration.getIntSetting("MaximumProcesses", -1);
                    /*Commenting out for now as I try to expand the application to be able to run on multiple servers
                     * 
                    int numProcesses = Int32.Parse(Math.Floor(maxProcessors / 2.0).ToString());
                    //if (totals < maxProcesses * minEntities)
                    //{
                    //    numProcesses = (int)Math.Ceiling(((double)totals) / minEntities);
                    //}

                    //loop through IDs to tell each process which ID to take care
                    int numEntityPerProcess = Int32.Parse(Math.Ceiling((double.Parse(totals.ToString())) / (double.Parse(numProcesses.ToString()))).ToString());
                    if (numEntityPerProcess > CircuitsPerProcess) { numEntityPerProcess = CircuitsPerProcess; }
                    
                    
                    for (int i = 0; i < numProcesses; i++)
                    {
                        int startIndex = i * numEntityPerProcess;
                        int endIndex = endIndex = (i + 1) * numEntityPerProcess - 1;

                        //If our startindex or is greater than the total circuits identified we are finished.
                        if (startIndex >= totals) { break; }

                        //If our end index is greater than what is left, then we will simply set our end index to the last index of the array.
                        if (endIndex >= totals) { endIndex = totals - 1; }

                        ExtractorMessage msg1 = new ExtractorMessage();
                        msg1.ExtractType = extracttype.Batch;
                        msg1.ServerName = msg.ServerName;
                        msg1.ProcessID = Guid.NewGuid().ToString();
                        if (isSubStation)
                        {
                            msg1.Substations.AddRange(IDs.GetRange(startIndex, endIndex - startIndex + 1));
                            LastSubstationEndIndex = endIndex;
                        }
                        else
                        {
                            msg1.IncludeCircuits.AddRange(IDs.GetRange(startIndex, endIndex - startIndex + 1));
                            LastElectricEndIndex = endIndex;
                        }
                        //start each process
                        CreateWorker(msg1, isSubStation, ref processIDs);
                        Console.WriteLine("process {0}: {1}-{2}", (i + 1), startIndex, endIndex);

                    }
                    */
                    if (isSubStation) { ProcessIDListByType.Add("SUBSTATION", processIDs); }
                    else { ProcessIDListByType.Add("ELECTRIC", processIDs); }

                }
                Console.WriteLine(IDs.Count);

            }
            catch (Exception ex)
            {
                _logger.Error("Error calculating data to export.", ex);
            }


        }

        private Process StartNewWorker(string serverName)
        {
            // #13
            ExtractorMessage msg1 = new ExtractorMessage();
            msg1.ExtractType = extracttype.Batch;
            msg1.ServerName = serverName;
            msg1.ProcessID = Guid.NewGuid().ToString();

            //start each process
            return CreateWorker(msg1);
        }

#if(DEBUG)
        /// <summary>
        /// Used for testing purposes
        /// </summary>
        static void Main(string[] args)
        {

            ExtractorManager em = new ExtractorManager();
            ExtractorMessage msg1 = new ExtractorMessage();
            List<string> processIDs = new List<string>();
            msg1.IncludeCircuits.Add("252971106");
            //msg1.IncludCircuits.Add("254071106");
            //msg1.IncludCircuits.Add("253411102");
            //ExtractorMessage msg2 = new ExtractorMessage();
            //msg2.IncludCircuits.Add("253701101");
            //msg2.IncludCircuits.Add("254291102");
            //msg2.IncludCircuits.Add("258101101");
            //ExtractorMessage msg3 = new ExtractorMessage();
            //msg3.IncludCircuits.Add("253701110");
            //msg3.IncludCircuits.Add("252931102");
            //msg3.IncludCircuits.Add("253372108");
            em.CreateWorker(msg1);
            //em.CreateWorker(msg2);
            //em.CreateWorker(msg3);
            while (true)
            {
                Thread.Sleep(1000);
            }
        }
#endif
    }
}
