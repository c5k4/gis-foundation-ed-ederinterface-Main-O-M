using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ESRI.ArcGIS.Geodatabase;
using System.IO;
using PGE.Common.Delivery.Diagnostics;
using System.Xml;
using System.Xml.Linq;
using PGE.Interfaces.ED11_12.SessionManager;
using System.Runtime.InteropServices;
using ESRI.ArcGIS.Geometry;
using System.Diagnostics;
using System.Collections.Specialized;
using PGE.Desktop.EDER.AutoUpdaters;
using PGE.Common.Delivery.Framework;
using PGE.Desktop.EDER;
using ESRI.ArcGIS.esriSystem;
using Miner.Interop;
using PGE.Desktop.EDER.AutoUpdaters.Special;
using System.Globalization;
using PGE.Desktop.EDER.AutoUpdaters.LabelText;
using System.Xml.Serialization;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using System.Data;
using Oracle.DataAccess.Client;
using PGE.Desktop.EDER.AutoUpdaters.Special.RegionalAttributesAU;
using PGE.Desktop.EDER.AutoUpdaters.Special.PLC;
using System.Reflection;
namespace PGE.Interfaces.ED11_12
{
    public class PTTProcessor
    {
        /// <summary>
        /// Logs error/ custom information
        /// </summary>
        public static Log4NetLogger _log = new Log4NetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, "PGE.Interfaces.ED11_12.log4net.config");
        private const string SessionName = "PTTProcessing";
        private const string FCSupportStructure = "EDGIS.SupportStructure";
        private string FCSupportStructureStaging = Common.PTTStagingTable;
        private const string GlobalIDFieldName = "GLOBALID";
        private const string SAPEquipIDFieldName = "SAPEQUIPID";
        private const string ReplaceGuidFieldName = "REPLACEGUID";
        private const string StatusFieldName = "STATUS";
        //Fix : 22-Oct-20
        private const string PLDBIDIDFieldName = "PLDBID";
      
        public static PGE.Desktop.EDER.AutoUpdaters.Special.PLC.PGEUpdateElevationAU pldbidAU = new PGE.Desktop.EDER.AutoUpdaters.Special.PLC.PGEUpdateElevationAU();

        private List<object> featureClassInformation = new List<object>();
        private const string ED11ProcessingTable = "EDGIS.PGE_ED11_PROCESSING";
        private const string ED11ProcessesTable = "EDGIS.PGE_ED11_PROCESSES";
        // M4JF EDGISREARCH 415 ED 11 IMPROVEMENTS
        private const string ED11StagingTable = "PGEDATA.PGE_ED11_STAGING";
        private static bool NoRollup = false;
        private static string uname = Common.ED11UserName;

        private static OracleConnection _oraConnection = null;
        private static DataSet ED12TableDataset = null;


        #region Constructor


        // m4jf edgisrearch 415 - this function will not require inputFileDirectory and ArchivefileDirectory as file system is move to staging tables for ed11 interface

        //public PTTProcessor(IWorkspace defaultWorkspace, IWorkspace landbaseWorkspace, string inputFileDirectory, string archiveFileDirectory, string errorLoggingPath,
        //   bool forceUpdateTransactions, bool submitToGDBMPost, string childVersionName, string parentVersionName, int numberOfProcesses, int maxRunningProcesses,
        //    int processIdentifier, string configurationFile, bool prepProcessingTable, bool allowUnknownErrors, bool noRollup)
        public PTTProcessor(IWorkspace defaultWorkspace, IWorkspace landbaseWorkspace, string errorLoggingPath,
            bool forceUpdateTransactions, bool submitToGDBMPost, string childVersionName, string parentVersionName, int numberOfProcesses, int maxRunningProcesses,
            int processIdentifier, string configurationFile, bool prepProcessingTable, bool allowUnknownErrors, bool noRollup)
        {
            ChildVersionName = childVersionName;
            ParentVersionName = parentVersionName;
            SubmitToGDBMPost = submitToGDBMPost;
            ForceUpdateTransactions = forceUpdateTransactions;
            NumberOfChildProcesses = numberOfProcesses;
            ProcessIdentifier = processIdentifier;
            ConfigurationFile = configurationFile;
            PrepareProcessingTable = prepProcessingTable;
            MaxRunningProcesses = maxRunningProcesses;
            AllowUnknownErrors = allowUnknownErrors;
            NoRollup = noRollup;


            // m4jf-  commenting code for edgisrearch 415 - code not required as GIS will now pull data from SAP through EI
            //Check if file exists first
            //if (!Directory.Exists(inputFileDirectory)) { throw new Exception("PTTProcessor: Unable to find specified input xml directory"); }
            //InputDirectory = inputFileDirectory;

            ////Check if the archive directory exists
            //if (!Directory.Exists(archiveFileDirectory)) { throw new Exception("PTTProcessor: Unable to find the specified input xml archive directory"); }
            //ArchiveDirectory = archiveFileDirectory;

            //SortedDictionary<DateTime, string> inputFilesByDateTime = new SortedDictionary<DateTime, string>();
            //DirectoryInfo sourceinfo = new DirectoryInfo(InputDirectory);
            //FileInfo[] fileInfo = sourceinfo.GetFiles();
            //foreach (FileInfo fileInformation in fileInfo)
            //{
            //    if (fileInformation.Name.ToUpper().StartsWith(Common.PTTInputFileName.ToUpper()) && fileInformation.Name.ToUpper().EndsWith(".XML"))
            //    {
            //        try
            //        {
            //            string timeStamp = fileInformation.Name.Substring((Common.PTTInputFileName + "_").Length);
            //            timeStamp = timeStamp.ToUpper().Remove(".XML");
            //            timeStamp = timeStamp.Remove("_");
            //            int year = Int32.Parse(timeStamp.Substring(0, 4));
            //            int month = Int32.Parse(timeStamp.Substring(4, 2));
            //            int day = Int32.Parse(timeStamp.Substring(6, 2));
            //            int hour = Int32.Parse(timeStamp.Substring(8, 2));
            //            int minute = Int32.Parse(timeStamp.Substring(10, 2));
            //            int second = Int32.Parse(timeStamp.Substring(12, 2));
            //            DateTime dateTime = new DateTime(year, month, day, hour, minute, second);
            //            inputFilesByDateTime.Add(dateTime, System.IO.Path.Combine(InputDirectory, fileInformation.Name));
            //        }
            //        catch (Exception ex)
            //        {
            //            throw new Exception("Failed processing input file " + fileInformation.Name + ": Input file names must meet the format '" + Common.PTTInputFileName + "_yyyymmdd_hhMMss.xml'");
            //        }
            //    }
            //}

            //if (inputFilesByDateTime.Count < 1) { throw new Exception("No xml input files meeting the format '" + Common.PTTInputFileName + "_yyyymmdd_hhMMss.xml' in the input directory " + InputDirectory); }

            //InputFiles = new List<string>();
            //foreach (KeyValuePair<DateTime, string> kvp in inputFilesByDateTime)
            //{
            //    InputFiles.Add(kvp.Value);
            //}

            //Below code is commented for EDGIS Rearch Project -V1t8
            //ErrorLoggingPath = errorLoggingPath;
            //if (!Directory.Exists(ErrorLoggingPath)) { throw new Exception("PTTProcessor: Unable to find error logging location specified: " + errorLoggingPath); }

            if (defaultWorkspace == null) { throw new Exception("PTTProcessor: Invalid workspace specified"); }
            if (landbaseWorkspace == null) { throw new Exception("PTTProcessor: Invalid landbase workspace specified"); }
            DefaultWorkspace = defaultWorkspace;
            LandbaseWorkspace = landbaseWorkspace;
        }

        public PTTProcessor(IWorkspace defaultWorkspace, IWorkspace landbaseWorkspace, bool forceUpdateTransactions, string childVersionName, string parentVersionName,
            int processIdentifier, string configurationFile, bool allowUnknownErrors)
        {
            AllowUnknownErrors = allowUnknownErrors;
            ChildVersionName = childVersionName;
            ParentVersionName = parentVersionName;
            ForceUpdateTransactions = forceUpdateTransactions;
            ProcessIdentifier = processIdentifier;
            ConfigurationFile = configurationFile;

            if (defaultWorkspace == null) { throw new Exception("PTTProcessor: Invalid workspace specified"); }
            if (landbaseWorkspace == null) { throw new Exception("PTTProcessor: Invalid landbase workspace specified"); }
            DefaultWorkspace = defaultWorkspace;
            LandbaseWorkspace = landbaseWorkspace;
        }


        #endregion

        #region Private Properties

        private bool AllowUnknownErrors { get; set; }
        private bool SubmitToGDBMPost { get; set; }
        private bool ForceUpdateTransactions { get; set; }
        private string ArchiveDirectory { get; set; }
        private string InputDirectory { get; set; }
        private List<string> InputFiles { get; set; }
        private string ErrorLoggingPath { get; set; }
        private StreamWriter ErrorLoggingWriter { get; set; }
        private IWorkspace DefaultWorkspace { get; set; }
        private IWorkspace LandbaseWorkspace { get; set; }
        private MinerSession ParentSession { get; set; }
        private string ChildVersionName { get; set; }
        private string ParentVersionName { get; set; }
        private IVersion EditVersion { get; set; }
        private IVersion ParentVersion { get; set; }
        private List<PTTTransaction> PTTTransactions { get; set; }
        private List<Process> ChildProcesses { get; set; }
        private int NumberOfChildProcesses { get; set; }
        private int MaxRunningProcesses { get; set; }
        private int ProcessIdentifier { get; set; }
        private string ConfigurationFile { get; set; }
        private bool PrepareProcessingTable { get; set; }

        private IFeatureClass _supportStructureFC = null;
        private IFeatureClass SupportStructure
        {
            get
            {
                if (_supportStructureFC == null)
                {
                    _supportStructureFC = ((IFeatureWorkspace)EditVersion).OpenFeatureClass(FCSupportStructure);
                }
                return _supportStructureFC;
            }
        }

        private IFeatureClass _supportStructureStagingFC = null;
        private IFeatureClass SupportStructureStaging
        {
            get
            {
                if (_supportStructureStagingFC == null)
                {
                    _supportStructureStagingFC = ((IFeatureWorkspace)EditVersion).OpenFeatureClass(FCSupportStructureStaging);
                }
                return _supportStructureStagingFC;
            }
        }

        #endregion

        #region Public Methods

        private static IMMAutoUpdater auController = null;
        public void ProcessPTTData(bool IgnoreExistingSession)
        {
            int insertsProcessed = 0;
            int insertsFailed = 0;
            int updatesProcessed = 0;
            int updatesFailed = 0;
            int deletesProcessed = 0;
            int deletesFailed = 0;
            int replacesProcessed = 0;
            int replacesFailed = 0;
            bool success = true;
            try
            {
                SAPToGISMappings mapping = SAPToGISMappings.Instance;

                //_oraConnection = new OracleConnection("Data Source=EDGEM1D;User Id=EDGIS;Password=EDG_ZRz97*gem1d");
                //_oraConnection.Open();
                //long a = GetNextSeq();
                //Check for existing session and fail if one exists

                if (!IgnoreExistingSession)
                {
                    string currentSessionName = CurrentPTTSessionName(DefaultWorkspace);
                    if (!string.IsNullOrEmpty(currentSessionName))
                    {
                        Program.comment = "Current PTT session already exists: " + currentSessionName;
                        throw new Exception("Current PTT session already exists: " + currentSessionName);

                        
                    }
                }

                //Create a new Session
                CreateMinerSession();

                //Check our support structure feature classes
                if (SupportStructure == null) 
                {
                    Program.comment = "Unable to find: " + FCSupportStructure + " feature class";
                    throw new Exception("Unable to find: " + FCSupportStructure + " feature class");
                   
                }
                if (SupportStructureStaging == null) 
                {
                    Program.comment = "Unable to find: " + FCSupportStructureStaging + " feature class";
                    throw new Exception("Unable to find: " + FCSupportStructureStaging + " feature class");
                    
                }

                //Process our specified xml file to find out what updates we have to perform
                if (PrepareProcessingTable) { PTTTransactions = ProcessXml(); }

                //Need to kick off child processes
                ExecuteProcessingInChildren(NumberOfChildProcesses, ParentSession.Workspace);

                //Below code is commented for EDGIS Rearch Project to discrad the file system concept for GIS-SAP.- v1t8
                //Need to add generating the ED12 file from the EDGIS.PGE_ED11_PROCESSING table results column
                //   ProduceED12File(ref insertsProcessed, ref insertsFailed, ref updatesProcessed, ref updatesFailed, ref deletesProcessed, ref deletesFailed, ref replacesProcessed, ref replacesFailed);

                //Below code is added for EDGIS Rearch Project to insert data into staging table instead of genrating files to send SAP.- v1t8
                PopulateED12Staging(ref insertsProcessed, ref insertsFailed, ref updatesProcessed, ref updatesFailed, ref deletesProcessed, ref deletesFailed, ref replacesProcessed, ref replacesFailed);


                //Below code is commented for EDGIS Rearch Project to insert data into staging table instead of genrating files to send SAP.- v1t8
                //Finally, we want to archive the old input files.
                //       ArchiveOldFiles();


                if (SubmitToGDBMPost)
                {
                    //Finally, we can submit this PT&T session to GDBM for posting
                    ParentSession.SubmitSessionToGDBM();
                    Console.WriteLine("Session submitted to GDBM for posting");
                    _log.Info("Session submitted to GDBM for posting");
                }

                _log.Info(insertsProcessed + " insert transactions were processed with " + insertsFailed + " failures.");
                _log.Info(updatesProcessed + " update transactions were processed with " + updatesFailed + " failures.");
                _log.Info(deletesProcessed + " delete transactions were processed with " + deletesFailed + " failures.");
                _log.Info(replacesProcessed + " replace transactions were processed with " + replacesFailed + " failures.");

                Console.WriteLine(insertsProcessed + " insert transactions were processed with " + insertsFailed + " failures.");
                Console.WriteLine(updatesProcessed + " update transactions were processed with " + updatesFailed + " failures.");
                Console.WriteLine(deletesProcessed + " delete transactions were processed with " + deletesFailed + " failures.");
                Console.WriteLine(replacesProcessed + " replace transactions were processed with " + replacesFailed + " failures.");
               // m4jf edgisrearch 415 update remarks for interface execution summary table
                Program.comment = "ED11 Interface executed successfully ." + insertsProcessed + " insert transactions were processed with " + insertsFailed + " failures." + updatesProcessed + " update transactions were processed with " + updatesFailed + " failures." + deletesProcessed + " delete transactions were processed with " + deletesFailed + " failures.";
               

            }
            catch (Exception ex)
            {
                //Something failed.  Report the erorr and throw the exception
                _log.Error("Failed PTT processor " + ex.Message + "\r\nStacktrace: " + ex.StackTrace);
                Program.comment = "Failed PTT processor " + ex.Message + "\r\nStacktrace: " + ex.StackTrace;
                success = false;
                throw ex;
            }
            finally
            {
                #region Below code is not required as File system approch replaced with staging area and Layer 7 approch to send data to SAP -EDGIS Rearch Project 2021-v1t8
                //if (success)
                //{
                //    //create trigger file (empty .txt file)
                //    StreamWriter writer = File.CreateText(Common.TriggerFileName);
                //    writer.Close();

                //    //Let's go ahead and archive this file now. Just helps aid in debugging
                //    string ED12ArchiveFileName = "";
                //    try
                //    {
                //        string ED12ArchiveDirectory = System.IO.Path.Combine(Common.ResultsOutputLocation, "Archive\\");
                //        if (!Directory.Exists(ED12ArchiveDirectory)) { Directory.CreateDirectory(ED12ArchiveDirectory); }

                //        ED12ArchiveFileName = System.IO.Path.Combine(Common.ResultsOutputLocation, "Archive\\" + Common.PTTTransactionStatusFileName);
                //        File.Copy(Common.PTTTransactionStatusFile, ED12ArchiveFileName);
                //    }
                //    catch (Exception ex)
                //    {
                //        //If anything fails for achiving the ED12 file, we will just simply ignore for now
                //        _log.Debug("Failed to archive the ED12 file '" + ED12ArchiveFileName + "': " + ex.Message);
                //    }
                //}
                //else
                //{
                //    //Something failed, so let's delete the ED12 file.
                //    try
                //    {
                //        File.Delete(Common.PTTTransactionStatusFile);
                //    }
                //    catch { }
                //}
                #endregion
            }
        }

        public void ProcessPTTDataInVersion()
        {
            try
            {
                SAPToGISMappings mapping = SAPToGISMappings.Instance;

                //Create a new Session
                SetVersions(ParentVersionName, ChildVersionName);

                //Check our support structure feature classes
                if (SupportStructure == null) { throw new Exception("Unable to find: " + FCSupportStructure + " feature class"); }
                if (SupportStructureStaging == null) { throw new Exception("Unable to find: " + FCSupportStructureStaging + " feature class"); }

                //Start a new edit session
                ((IWorkspaceEdit)EditVersion).StartEditing(false);

                PTTTransactions = GetTransactions();

                //For performance sake, disable AUs
                Type type = Type.GetTypeFromProgID("mmGeoDatabase.MMAutoUpdater");
                auController = (IMMAutoUpdater)Activator.CreateInstance(type);
                mmAutoUpdaterMode prevAUMode = auController.AutoUpdaterMode;

                try
                {
                    Console.WriteLine("There are " + PTTTransactions.Count + " transactions to process");

                    //Cache our first set of poles for transaction processing
                    if (PTTTransactions.Count > 0) { CacheInformation(); }
                    int transactionsProcessed = 0;
                    int transactionsSinceLastSave = 0;
                    PopulateDistMapNoAU.AllowExecutionOutsideOfArcMap = true;
                    InheritRegionalAttributesAU.AllowExecutionOutsideOfArcMap(LandbaseWorkspace);
                    auController.AutoUpdaterMode = mmAutoUpdaterMode.mmAUMStandAlone;
                    foreach (PTTTransaction transaction in PTTTransactions)
                    {
                        try
                        {
                            bool transactionSuccess = false;

                            if (transaction.Type == TransactionType.Insert)
                            {
                                PopulateLocalOfficeAU.LookupLocalOfficeByModelName = false;
                            }
                            else if (transaction.Type == TransactionType.Update || transaction.Type == TransactionType.Replace || transaction.Type == TransactionType.Delete)
                            {
                                PopulateLocalOfficeAU.LookupLocalOfficeByModelName = true;
                            }

                            //Check if this transaction has malformed xml. If so, log the error and move on
                            if (transaction.TransactionPole.PoleMalformedXml)
                            {
                                LogTransactionError(transaction, TransactionErrorType.MalformedXml,
                            TransactionStatus.Failure, transaction.TransactionPole.PoleMalformedMessage, "");
                            }
                            else
                            {
                                CheckRequiredAttributes(transaction);

                                if (transaction.TransactionPole.MissingAttributesEncountered)
                                {
                                    LogTransactionError(transaction, TransactionErrorType.MissingMandatoryFieldValues,
                            TransactionStatus.Failure, "Missing field values for the fields " + transaction.TransactionPole.MissingAttributes, "");
                                }
                                else
                                {
                                    if (!((IWorkspaceEdit2)EditVersion).IsInEditOperation) { ((IWorkspaceEdit)EditVersion).StartEditOperation(); }
                                    transactionSuccess = ProcessTransaction(transaction);

                                    //If our transaction was successful, stop the edit operation.  If it failed, abort the edit operation.
                                    //This ensures that no transactions are partially executed
                                    if (transactionSuccess)
                                    {
                                        if (((IWorkspaceEdit2)EditVersion).IsInEditOperation) { ((IWorkspaceEdit)EditVersion).StopEditOperation(); }
                                    }
                                    else
                                    {
                                        if (((IWorkspaceEdit2)EditVersion).IsInEditOperation) { ((IWorkspaceEdit)EditVersion).AbortEditOperation(); }
                                    }
                                }
                            }

                            transactionsProcessed++;
                            transactionsSinceLastSave++;

                            //If this was the last cached transaction, then we need to build our next set of cached features for processing
                            if (LastTransactionCached == transaction)
                            {
                                try
                                {
                                    //Save any edits that have been completed for Oracle constaints
                                    if (((IWorkspaceEdit2)EditVersion).IsInEditOperation)
                                    {
                                        ((IWorkspaceEdit)EditVersion).StopEditOperation();
                                    }
                                    if (((IWorkspaceEdit)EditVersion).IsBeingEdited())
                                    {
                                        //Console.WriteLine("Stop editing and saving edits");
                                        ((IWorkspaceEdit)EditVersion).StopEditing(true);
                                        //Console.WriteLine("Starting editing");
                                        ((IWorkspaceEdit)EditVersion).StartEditing(false);
                                    }

                                    transactionsSinceLastSave = 0;
                                }
                                catch (Exception ex)
                                {
                                    throw new Exception("Failed to save edits: " + ex.Message);
                                }

                                CacheInformation();
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("Number of transactions processed before error: " + transactionsProcessed);
                            Console.WriteLine("Number of transactions since last save before error: " + transactionsSinceLastSave);
                            if (!AllowUnknownErrors) { throw new Exception("Failed to process transaction " + transaction.TransactionPole.SAPEquipID + ": " + ex.Message + ": StackTrace: " + ex.StackTrace); }
                            else
                            {
                                _log.Error(ex.ToString());
                                _log.Error("Unknown Error was encountered during batch operation ':" + ex.Message + "'" + transaction.ToString() + " " + transaction.OrderNumber + " " + transaction.TransactionPole.OriginalTransaction + " " + transaction.TransactionPole.SAPEquipID);
                                Console.WriteLine(ex.ToString());
                                Console.WriteLine("Unknown Error was encountered during batch operation ':" + ex.Message + "'" + transaction.ToString() + " " + transaction.OrderNumber + " " + transaction.TransactionPole.OriginalTransaction + " " + transaction.TransactionPole.SAPEquipID);
                                //We still need to log the error to ED12 if we are not throwing an exception.  We'll currently log this as an unknown error
                                LogTransactionError(transaction, TransactionErrorType.UnknownError, TransactionStatus.Failure,
                            "Unknown error encountered. Review ED11 error logs", "");
                            }
                        }
                        finally
                        {
                            if (((IWorkspaceEdit2)EditVersion).IsInEditOperation) { ((IWorkspaceEdit)EditVersion).AbortEditOperation(); }
                        }
                    }

                    Console.WriteLine("Completed processing transactions");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Failed to process PTT Transactions: " + ex.Message + "\r\nStacktrace: " + ex.StackTrace);
                    _log.Error("Failed to process PTT Transactions: " + ex.Message + "\r\nStacktrace: " + ex.StackTrace);
                    //Abort any editing that was done

                    if (((IWorkspaceEdit2)EditVersion).IsInEditOperation)
                    {
                        ((IWorkspaceEdit)EditVersion).AbortEditOperation();
                    }
                    if (((IWorkspaceEdit)EditVersion).IsBeingEdited())
                    {
                        ((IWorkspaceEdit)EditVersion).StopEditing(false);
                    }

                    throw ex;
                }

                //Save any edits that were completed
                if (((IWorkspaceEdit2)EditVersion).IsInEditOperation)
                {
                    ((IWorkspaceEdit)EditVersion).StopEditOperation();
                }
                if (((IWorkspaceEdit)EditVersion).IsBeingEdited())
                {
                    ((IWorkspaceEdit)EditVersion).StopEditing(true);
                }
            }
            catch (Exception ex)
            {
                //Something failed.  Report the erorr and throw the exception
                Console.WriteLine("Failed PTT processor " + ex.Message + "\r\nStacktrace: " + ex.StackTrace);
                throw ex;
            }
            finally
            {

            }

        }

        #endregion

        #region Private Methods

        private void StartChildProcess()
        {
            if (childVersions.Count < NumberOfChildProcesses && childProcessesRunning.Count < MaxRunningProcesses)
            {
                string AssemblyLocation = System.Reflection.Assembly.GetExecutingAssembly().Location;
                Process myProcess = null;
                string parentVerionName = ((IVersion)ParentSession.Workspace).VersionName;
                string childVersionName = parentVerionName.Substring(parentVerionName.LastIndexOf(".") + 1) + "_" + childVersions.Count;
                string arguments = "-c \"" + ConfigurationFile + "\" -pid " + childVersions.Count + " -v " + childVersionName + " -p " + parentVerionName;
                childVersions.Add(childVersionName);
                if (ForceUpdateTransactions) { arguments += " -ForceUpdateTransactions"; }
                if (AllowUnknownErrors) { arguments += " -UnknownErrors"; }

                AssemblyLocation = AssemblyLocation.Substring(0, AssemblyLocation.LastIndexOf("\\") + 1) + "PGE.Interfaces.ED11_12.exe";
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
                    Console.WriteLine("Error creating Process. " + ex.Message);
                    throw ex;
                }
            }
        }

        #region Child Processes Execution
        List<string> childVersions = new List<string>();
        private List<Process> childProcessesRunning = new List<Process>();
        private bool ExecuteProcessingInChildren(int numProcesses, IWorkspace workspace)
        {
            bool success = true;
            try
            {
                ITable ED11Table = ((IFeatureWorkspace)workspace).OpenTable(ED11ProcessingTable);

                for (int i = 0; i < MaxRunningProcesses; i++)
                {
                    StartChildProcess();
                }

                IQueryFilter qf = new QueryFilterClass();
                qf.WhereClause = "";

                double totalToProcess = ED11Table.RowCount(qf);
                double currentlyProcessed = 0;
                qf.WhereClause = string.Format("TRANSACTIONSUCCESS = '{0}' OR TRANSACTIONSUCCESS = '{1}'", TransactionStatus.Failure, TransactionStatus.Success);

                while (ChildrenStillProcessing())
                {
                    currentlyProcessed = ShowProgress(ED11Table, qf.WhereClause, totalToProcess, currentlyProcessed);
                }

                currentlyProcessed = ED11Table.RowCount(qf);
                ShowPercentProgress(string.Format("Processing SAP Records: {0} of {1} processed", currentlyProcessed, totalToProcess), currentlyProcessed, totalToProcess);
            }
            catch (Exception ex)
            {
                success = false;

                // m4jf edgisrearch 415 update remarks for interface execution summary table
                Program.comment = "Unexpected error encountered: " + ex.Message;
                Console.WriteLine("Unexpected error encountered: " + ex.Message);
                _log.Error("Unexpected error encountered: " + ex.Message);
                throw ex;
            }

            Console.WriteLine("");
            //Now let's post the child versions.
            // bug in reconcile for v large versions means we gonna skip
            if (NoRollup)
            {
                _log.Info("No Rollup, exiting");
                return success;
            }

            _log.Info(string.Format("{0}: Cleaning up child versions", DateTime.Now));
            Console.WriteLine(string.Format("{0}: Cleaning up child versions", DateTime.Now));
            childVersions.Sort();
            foreach (string childVersion in childVersions)
            {
                IVersion versionToPost = null;
                string parentVersionName = ((IVersion)ParentSession.Workspace).VersionName;
                try
                {
                    _log.Info(string.Format("{2}: Posting version {0} to parent version {1}", childVersion, parentVersionName, DateTime.Now));
                    Console.WriteLine(string.Format("{2}: Posting version {0} to parent version {1}", childVersion, parentVersionName, DateTime.Now));
                    versionToPost = ((IVersionedWorkspace)workspace).FindVersion(childVersion);
                    IVersionEdit2 versionEdit2 = versionToPost as IVersionEdit2;
                    IVersionEdit versionEdit = versionToPost as IVersionEdit;
                    ((IWorkspaceEdit)versionEdit).StartEditing(false);
                    _log.Info(string.Format("Reconciling version {0} to parent version {1}", childVersion, parentVersionName, DateTime.Now));
                    versionEdit.Reconcile(parentVersionName);
                    _log.Info(string.Format("Posting version {0} to parent version {1}", childVersion, parentVersionName, DateTime.Now));
                    versionEdit.Post(parentVersionName);
                    ((IWorkspaceEdit)versionEdit).StopEditing(true);
                    _log.Info(string.Format("{0}: Session posted successfully", DateTime.Now));
                    Console.WriteLine(string.Format("{0}: Session posted successfully", DateTime.Now));
                    try
                    {
                        _log.Info(string.Format("{0}: Deleting associated version", DateTime.Now));
                        Console.WriteLine(string.Format("{0}: Deleting associated version", DateTime.Now));
                        versionToPost.Delete();
                        _log.Info(string.Format("{0}: Version deleted successfully", DateTime.Now));
                        Console.WriteLine(string.Format("{0}: Version deleted successfully", DateTime.Now));
                    }
                    catch (Exception ex)
                    {
                        _log.Info(string.Format("{0}: Failed to delete version: {1}", DateTime.Now, ex.Message));
                        Console.WriteLine(string.Format("{0}: Failed to delete version: {1}", DateTime.Now, ex.Message));
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception(string.Format("Failed to post version {0} to parent version {1}: {2}", childVersion, parentVersionName, ex.Message));
                }

                //Release version resources
                if (versionToPost != null) { while (Marshal.ReleaseComObject(versionToPost) > 0) { } }
            }

            return success;
        }

        private double ShowProgress(ITable ED11Table, string whereClause, double totalToProcess, double currentlyProcessed)
        {
            // Instantiating new QF because was leaking, increasing sleep time a tad, too
            IQueryFilter qf2 = new QueryFilterClass();
            qf2.WhereClause = whereClause;
            currentlyProcessed = ED11Table.RowCount(qf2);
            double percentComplete = (Math.Round((currentlyProcessed / totalToProcess) * 100.0, 2));
            ShowPercentProgress(string.Format("Processing ED11 Transactions: {0} of {1} processed", currentlyProcessed, totalToProcess), currentlyProcessed, totalToProcess);
            Thread.Sleep(10000);
            Marshal.FinalReleaseComObject(qf2);
            GC.Collect();
            GC.WaitForPendingFinalizers();
            return currentlyProcessed;
        }

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

        private bool ChildrenStillProcessing()
        {
            bool processesStillRunning = false;

            try
            {
                StartChildProcess();
                if (childProcessesRunning.Count > 0 || (childVersions.Count < NumberOfChildProcesses))
                {
                    processesStillRunning = true;
                }

                return processesStillRunning;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                //KillChildProcesses();
                throw e;
            }
        }

        private void KillChildProcesses()
        {
            try
            {
                foreach (Process p in childProcessesRunning)
                {
                    if (!p.HasExited)
                    {
                        //p.Kill(); 
                    }
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
                            //process.Kill();

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
                    _log.Error(builder.ToString());
                    Console.WriteLine(builder.ToString());
                }
                Environment.ExitCode = (int)PGE.Interfaces.ED11_12.Program.ExitCodes.Failure;
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
                    Console.WriteLine(builder.ToString());
                }
            }

            lock (childProcsRunningLock) { childProcessesRunning.Remove(p); }
        }
        #endregion

        private void ArchiveOldFiles()
        {
            try
            {
                foreach (string inputFile in InputFiles)
                {
                    string fileName = inputFile.Substring(inputFile.LastIndexOf("\\") + 1);
                    File.Copy(inputFile, System.IO.Path.Combine(ArchiveDirectory, fileName));
                    File.Delete(System.IO.Path.Combine(InputDirectory, fileName));
                }
            }
            catch (Exception ex)
            {
                _log.Error("Failed to archive input files: " + ex.Message);
                throw new Exception("Failed to archive input files: " + ex.Message);
            }
        }

        private void ArchiveED12Files(string ED12File)
        {
            try
            {
                string fileName = ED12File.Substring(ED12File.LastIndexOf("\\") + 1);
                File.Copy(ED12File, System.IO.Path.Combine(Common.PTTED12ArchiveDirectory, fileName));
            }
            catch (Exception ex)
            {
                _log.Error("Failed to archive ED12 output file: " + ex.Message);
                throw new Exception("Failed to archive ED12 output file: " + ex.Message);
            }
        }

        PTTTransaction LastTransactionCached = null;
        List<Guid> DeletedFeatures = new List<Guid>();
        List<CachedFeature> CachedFeatures = new List<CachedFeature>();
        //Dictionary<Guid, IFeature> AlreadyReplacedFeatures = new Dictionary<Guid, IFeature>();
        Dictionary<string, IFeature> AlreadyReplacedFeaturesByEquipID = new Dictionary<string, IFeature>();
        private void CacheInformation()
        {
            try
            {
                //Any currently cached features should be released
                foreach (CachedFeature cachedFeature in CachedFeatures)
                {
                    if (!DeletedFeatures.Contains(cachedFeature.guid))
                    {
                        //cachedFeature.PoleFeature.Store();
                    }

                    //Release information
                    while (Marshal.ReleaseComObject(cachedFeature.PoleFeature) > 0) { }

                    if (cachedFeature.RelatedObjects.Count > 0)
                    {
                        foreach (KeyValuePair<IRelationshipClass, List<IObject>> kvp in cachedFeature.RelatedObjects)
                        {
                            foreach (IObject row in kvp.Value)
                            {
                                while (Marshal.ReleaseComObject(row) > 0) { }
                            }
                        }
                    }
                }

                /*
                //Any currently cached features should be released
                foreach (KeyValuePair<Guid, IFeature> kvp in AlreadyReplacedFeatures)
                {
                    //Release information
                    while (Marshal.ReleaseComObject(kvp.Value) > 0) { }
                }

                //Any currently cached features should be released
                foreach (KeyValuePair<string, IFeature> kvp in AlreadyReplacedFeaturesByEquipID)
                {
                    //Release information
                    while (Marshal.ReleaseComObject(kvp.Value) > 0) { }
                }
                */

                //Clear our cache before we repopulate it
                CachedFeatures.Clear();
                DeletedFeatures.Clear();
                //AlreadyReplacedFeatures.Clear();
                //AlreadyReplacedFeaturesByEquipID.Clear();

                bool buildList = false;
                if (LastTransactionCached == null) { buildList = true; }

                //Iterate over our transactions and build our next list of guids
                List<string> guids = new List<string>();
                List<string> sapEquipIDs = new List<string>();
                foreach (PTTTransaction transaction in PTTTransactions)
                {
                    if (buildList)
                    {
                        if (transaction.Type == TransactionType.Delete || transaction.Type == TransactionType.Update)
                        {
                            guids.Add(transaction.TransactionPole.Guid.ToString("B").ToUpper());
                            sapEquipIDs.Add(transaction.TransactionPole.SAPEquipID);
                        }
                        else if (transaction.Type == TransactionType.Replace)
                        {
                            guids.Add(((PoleReplace)transaction.TransactionPole).PoleToDelete.Guid.ToString("B").ToUpper());
                            sapEquipIDs.Add(((PoleReplace)transaction.TransactionPole).PoleToDelete.SAPEquipID);
                        }
                        else if (transaction.Type == TransactionType.Insert)
                        {
                            sapEquipIDs.Add(transaction.TransactionPole.SAPEquipID);
                        }

                        if (sapEquipIDs.Count == 999 || guids.Count == 999)
                        {
                            //Distinct list first
                            guids = guids.Distinct().ToList();
                            sapEquipIDs = sapEquipIDs.Distinct().ToList();

                            //If the guids list is still 999 then go ahead and stop building our list of guids as this is the largest to fit in a sql query
                            if (sapEquipIDs.Count == 999 || guids.Count == 999)
                            {
                                //Limit reached. Mark our last cached transaction
                                LastTransactionCached = transaction;
                                break;
                            }
                        }
                    }

                    //If this transaction equals the last one we cached, then we want to build our next list of guids now.
                    if (LastTransactionCached == transaction) { buildList = true; }
                }

                //We have our next set of guids to caches
                List<string> whereInSAPIDClauses = GetWhereInClauses(sapEquipIDs);
                List<string> whereInClauses = GetWhereInClauses(guids);

                string whereInGuids = "";
                if (whereInClauses.Count > 0) { whereInGuids = whereInClauses[0]; }

                string whereInSAP = "";
                if (whereInSAPIDClauses.Count > 0) { whereInSAP = whereInSAPIDClauses[0]; }

                IQueryFilter qf = new QueryFilterClass();
                if (!string.IsNullOrEmpty(whereInGuids)) { qf.WhereClause = "GLOBALID IN (" + whereInGuids + ")"; }
                if (!string.IsNullOrEmpty(whereInSAP))
                {
                    if (!string.IsNullOrEmpty(qf.WhereClause)) { qf.WhereClause += " OR "; }
                    qf.WhereClause += SAPEquipIDFieldName + " in (" + whereInSAP + ")";
                }

                //Process the support structure table
                int globalIDIdx = FindFieldIndex(SupportStructure, GlobalIDFieldName);
                int sapEquipIDIdx = FindFieldIndex(SupportStructure, SAPEquipIDFieldName);
                IFeatureCursor featCursor = SupportStructure.Search(qf, false);
                IFeature feature = null;
                while ((feature = featCursor.NextFeature()) != null)
                {
                    Guid guid = new Guid(feature.get_Value(globalIDIdx).ToString());
                    object sapEquipIDObj = feature.get_Value(sapEquipIDIdx);
                    string sapEquipID = "";
                    if (sapEquipIDObj != null) { sapEquipID = sapEquipIDObj.ToString(); }

                    CachedFeatures.Add(new CachedFeature(feature, guid, sapEquipID));
                }

                while (Marshal.ReleaseComObject(featCursor) > 0) { }

                //Process the staging table now too
                globalIDIdx = FindFieldIndex(SupportStructureStaging, GlobalIDFieldName);
                featCursor = SupportStructureStaging.Search(qf, false);
                feature = null;
                while ((feature = featCursor.NextFeature()) != null)
                {
                    Guid guid = new Guid(feature.get_Value(globalIDIdx).ToString());
                    object sapEquipIDObj = feature.get_Value(sapEquipIDIdx);
                    string sapEquipID = "";
                    if (sapEquipIDObj != null) { sapEquipID = sapEquipIDObj.ToString(); }

                    CachedFeatures.Add(new CachedFeature(feature, guid, sapEquipID));
                }

                while (Marshal.ReleaseComObject(featCursor) > 0) { }



                if (!string.IsNullOrEmpty(whereInGuids))
                {
                    /*
                    qf = new QueryFilterClass();
                    qf.WhereClause = ReplaceGuidFieldName + " IN (" + whereInGuids + ")";

                    //Process the support structure table
                    int replaceGuidIdx = FindFieldIndex(SupportStructure, ReplaceGuidFieldName);
                    featCursor = SupportStructure.Search(qf, false);
                    feature = null;
                    while ((feature = featCursor.NextFeature()) != null)
                    {
                        Guid guid = new Guid(feature.get_Value(replaceGuidIdx).ToString());
                        if (!AlreadyReplacedFeatures.ContainsKey(guid)) { AlreadyReplacedFeatures.Add(guid, feature); }
                    }

                    while (Marshal.ReleaseComObject(featCursor) > 0) { }

                    //Process the staging table now too
                    replaceGuidIdx = FindFieldIndex(SupportStructureStaging, ReplaceGuidFieldName);
                    featCursor = SupportStructureStaging.Search(qf, false);
                    feature = null;
                    while ((feature = featCursor.NextFeature()) != null)
                    {
                        Guid guid = new Guid(feature.get_Value(replaceGuidIdx).ToString());
                        if (!AlreadyReplacedFeatures.ContainsKey(guid)) { AlreadyReplacedFeatures.Add(guid, feature); }
                    }

                    while (Marshal.ReleaseComObject(featCursor) > 0) { }
                    */
                }

                GetRelationshipMapping(null, true);
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to build feature cache: " + ex.Message);
            }
            finally
            {
            }
        }

        /// <summary>
        /// Obtains a list of where in clauses for a list of guids
        /// </summary>
        /// <param name="guids"></param>
        /// <returns></returns>
        private List<string> GetWhereInClauses(List<string> guids)
        {
            try
            {
                List<string> whereInClauses = new List<string>();
                StringBuilder builder = new StringBuilder();

                int counter = 0;
                foreach (string guid in guids)
                {
                    if (counter == 999)
                    {
                        builder.Append("'" + guid + "'");
                        whereInClauses.Add(builder.ToString());
                        builder = new StringBuilder();
                        counter = 0;
                    }
                    else
                    {
                        builder.Append("'" + guid + "',");
                        counter++;
                    }
                }

                if (builder.Length > 0)
                {
                    string whereInClause = builder.ToString();
                    whereInClause = whereInClause.Substring(0, whereInClause.LastIndexOf(","));
                    whereInClauses.Add(whereInClause);
                }
                return whereInClauses;
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to create guid where in clauses. Error: " + ex.Message);
            }
        }

        /// <summary>
        /// Updates progress within the console application.  If anything fails, it just catches and moves on.  This is for the case that UC4 fails updating the console
        /// window which has been seen in the past
        /// </summary>
        /// <param name="message"></param>
        /// <param name="currElementIndex"></param>
        /// <param name="totalElementCount"></param>
        private void ShowPercentProgress(string message, int currElementIndex, int totalElementCount)
        {
            try
            {
                if (currElementIndex < 0 || currElementIndex > totalElementCount)
                {
                    throw new InvalidOperationException("currElement out of range");
                }
                double percent = Math.Round((100.0 * ((double)currElementIndex)) / (double)totalElementCount, 2);
                Console.Write("\r{0}: {1}% complete", message, percent);
                if (currElementIndex == totalElementCount)
                {
                    Console.WriteLine(Environment.NewLine);
                }
            }
            catch (Exception ex)
            {

            }
        }


        /// <summary>
        /// Create (or get existing) ArcFM session manager session for PTT Processing
        /// </summary>
        private void CreateMinerSession()
        {
            MinerSession minerSession = new MinerSession(DefaultWorkspace, Common.SessionManagerConnectionString);
            minerSession.ReuseSession = false;
            minerSession.CreateMMSessionVersion(SessionName);
            ParentSession = minerSession;
            EditVersion = minerSession.Workspace as IVersion;
            ParentVersion = DefaultWorkspace as IVersion;
        }

        private string CurrentPTTSessionName(IWorkspace workspace)
        {
            try
            {
                string sessionName = "";
                ITable mmSessionTable = ((IFeatureWorkspace)workspace).OpenTable("PROCESS.MM_SESSION");
                int sessionNameIdx = mmSessionTable.FindField("SESSION_NAME");
                IQueryFilter qf = new QueryFilterClass();
                qf.SubFields = "SESSION_NAME";
                qf.WhereClause = "HIDDEN = 0 AND SESSION_NAME LIKE '%PTTProcessing%'";
                ICursor rowCursor = mmSessionTable.Search(qf, false);
                IRow row = null;
                while ((row = rowCursor.NextRow()) != null)
                {
                    sessionName = row.get_Value(sessionNameIdx).ToString();
                    while (Marshal.ReleaseComObject(row) > 0) { }
                }

                if (qf != null) { while (Marshal.ReleaseComObject(qf) > 0) { } }
                if (rowCursor != null) { while (Marshal.ReleaseComObject(rowCursor) > 0) { } }
                if (mmSessionTable != null) { while (Marshal.ReleaseComObject(mmSessionTable) > 0) { } }

                return sessionName;
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to check PROCESS.MM_SESSION table for existing sessions: " + ex.Message);
            }
        }

        private void SetVersions(string parentVersionName, string childVersionName)
        {
            try
            {
                Console.WriteLine(string.Format("Create version '{0}' from parent version '{1}'", childVersionName, parentVersionName));
                //Attempt to get and delete the current version if it exists
                try
                {
                    IVersion existingVersion = ((IVersionedWorkspace2)DefaultWorkspace).FindVersion(childVersionName);
                    existingVersion.Delete();
                }
                catch { }
                ParentVersion = ((IVersionedWorkspace2)DefaultWorkspace).FindVersion(parentVersionName);
                EditVersion = ((IVersion)ParentVersion).CreateVersion(childVersionName);
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Failed to create version '{0}' from parent version '{1}': {2}", childVersionName, parentVersionName, ex.Message));
            }
        }

        #region PTT Transaction Processing

        Dictionary<string, int> uniqueGuidUpdates = new Dictionary<string, int>();
        /// <summary>
        /// Process a PTT transaction record
        /// </summary>
        /// <param name="transaction"></param>
        private bool ProcessTransaction(PTTTransaction transaction)
        {
            bool success = true;
            try
            {
                if (transaction.Type == TransactionType.Insert)
                {
                    //Insert a new pole
                    success = ProcessInsert(transaction, true, false);
                }
                else if (transaction.Type == TransactionType.Delete)
                {
                    //Delete an existing pole
                    success = ProcessDelete(transaction);
                }
                else if (transaction.Type == TransactionType.Replace)
                {
                    //Replace an existing pole
                    success = ProcessReplace(transaction);
                }
                else if (transaction.Type == TransactionType.Update)
                {
                    //Update an existing pole
                    success = ProcessUpdate(transaction);
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex.ToString());
                _log.Error(transaction.ToString() + " " + transaction.OrderNumber + " " + transaction.TransactionPole.OriginalTransaction + " " + transaction.TransactionPole.SAPEquipID);
                if (ex.Message.Contains("found for domain"))
                {
                    //Domain value wasn't found. Let's log and error;
                    LogTransactionError(transaction, TransactionErrorType.DomainValueMismatch, TransactionStatus.Failure,
                    ex.Message, "");
                    success = false;
                }
                else
                {
                    success = false;
                    // If we're running in backlog mode (NoRollup) let's not throw an exception but continue
                    if (AllowUnknownErrors)
                    {
                        _log.Error(ex.ToString());
                        _log.Error("Unknown Error was encountered during batch operation ':" + ex.Message + "'" + transaction.ToString() + " " + transaction.OrderNumber + " " + transaction.TransactionPole.OriginalTransaction + " " + transaction.TransactionPole.SAPEquipID);

                        //We still need to log the error to ED12 if we are not throwing an exception.  We'll currently log this as an unknown error
                        LogTransactionError(transaction, TransactionErrorType.UnknownError, TransactionStatus.Failure,
                    "Unknown error encountered. Review ED11 error logs", "");
                    }
                    else if (transaction.Type == TransactionType.Replace)
                    {
                        //throw new Exception(string.Format("Failed to process {0} transaction with Insert SAPEquipID {1}: Message {2}: StackTrace {3}",
                        //transaction.Type, ((PoleReplace)transaction.TransactionPole).PoleToInsert.SAPEquipID, ex.Message, ex.StackTrace));

                        // m4jf edgisrearch 
                        string error = string.Format("Failed to process {0} transaction with Insert SAPEquipID {1}: Message {2}",
                                transaction.Type, ((PoleReplace)transaction.TransactionPole).PoleToInsert.SAPEquipID, ex.Message);
                        _log.Error(error);
                        LogTransactionError(transaction, TransactionErrorType.UnknownError, TransactionStatus.Failure,
                         error, "");
                    }
                    else
                    {
                        // throw new Exception(string.Format("Failed to process {0} transaction with SAPEquipID {1}: Message {2}: StackTrace {3}",
                        //  transaction.Type, transaction.TransactionPole.SAPEquipID, ex.Message, ex.StackTrace));
                        string error = string.Format("Failed to process {0} transaction with SAPEquipID {1}: Message {2}",
                                transaction.Type, transaction.TransactionPole.SAPEquipID, ex.Message);
                        _log.Error(error);
                        LogTransactionError(transaction, TransactionErrorType.UnknownError, TransactionStatus.Failure,
                         error, "");
                    }
                    //else
                    //{
                    //    _log.Error(ex.ToString());
                    //    _log.Error("Unknown Error was encountered during batch operation ':"  + ex.Message + "'" + transaction.ToString() + " " + transaction.OrderNumber + " " + transaction.TransactionPole.OriginalTransaction + " " + transaction.TransactionPole.SAPEquipID);

                    //We still need to log the error to ED12 if we are not throwing an exception.  We'll currently log this as an unknown error
                    //    LogTransactionError(transaction, TransactionErrorType.UnknownError, TransactionStatus.Failure,
                    //"Unknown error encountered. Review ED11 error logs", "");
                    //}
                }
            }
            finally
            {
            }

            return success;
        }

        /// <summary>
        /// Sets the specified SAP attributes for the provided structure feature
        /// </summary>
        /// <param name="featureToSet"></param>
        /// <param name="ValuesToSet"></param>
        private void SetAttributes(ref IFeature featureToSet, Dictionary<string, string> ValuesToSet)
        {
            try
            {
                foreach (KeyValuePair<string, string> kvp in ValuesToSet)
                {
                    object fieldValue = DBNull.Value;
                   
                        string fieldName = SAPToGISMappings.Instance.GetGISMappedField(kvp.Key);


                        int fieldIndex = FindFieldIndex(featureToSet.Class as IFeatureClass, fieldName);
                        if (fieldIndex < 0)
                        {
                            Console.WriteLine("Unable to find field specified in PTT xml. SAP Field Name: " + kvp.Key + ". GIS Field Name: " + fieldName);
                            throw new Exception("Unable to find field specified in PTT xml. SAP Field Name: " + kvp.Key + ". GIS Field Name: " + fieldName);
                        }

                        fieldValue = GetDomainValue(featureToSet, fieldIndex, kvp.Value, true);

                        //Finally set our value
                        featureToSet.set_Value(fieldIndex, fieldValue);

                 

                }
            }
            finally
            {
            }
        }

        private FeatureClassInformation GetFeatureClassInformation(IFeatureClass featClass)
        {
            try
            {
                int indexOfInfo = featureClassInformation.IndexOf(featClass);
                FeatureClassInformation featClassInfo = null;
                if (indexOfInfo < 0)
                {
                    featClassInfo = new FeatureClassInformation(featClass);
                    featureClassInformation.Add(featClassInfo);
                }
                else
                {
                    featClassInfo = featureClassInformation[indexOfInfo] as FeatureClassInformation;
                }

                return featClassInfo;
            }
            finally
            {
            }
        }

        private int FindFieldIndex(IFeatureClass featClass, string fieldName)
        {
            try
            {
                FeatureClassInformation featClassInfo = GetFeatureClassInformation(featClass);
                return featClassInfo.GetFieldIndex(fieldName);
            }
            finally
            {
            }
        }


        /// <summary>
        /// Finds the specified pole and returns the subfields with it specified
        /// </summary>
        /// <param name="poleToFind"></param>
        /// <param name="subfields"></param>
        /// <returns></returns>
        private IFeature FindPole(Pole poleToFind, string subfields, bool checkStructureTable, bool checkStagingTable)
        {
            try
            {
                CachedFeature testFeature = new CachedFeature(null, poleToFind.Guid, poleToFind.SAPEquipID);
                int cachedFeatureIndex = CachedFeatures.IndexOf(testFeature);
                if (cachedFeatureIndex >= 0)
                {
                    //If this feature was already deleted, then we need to return null
                    if (DeletedFeatures.Contains(poleToFind.Guid)) { return null; }
                    else { return CachedFeatures[cachedFeatureIndex].PoleFeature; }
                }
                else
                {
                    //Feature wasn't in our cache, so we should return null

#if DEBUG
                    /*
                    IQueryFilter qf = new QueryFilterClass();
                    if (!(((Pole)poleToFind).Guid == Guid.Empty)) { qf.WhereClause = GlobalIDFieldName + " = '" + ((Pole)poleToFind).Guid.ToString("B").ToUpper() + "'"; }
                    else { qf.WhereClause = SAPEquipIDFieldName + " = '" + ((Pole)poleToFind).SAPEquipID + "'"; }
                    qf.SubFields = subfields;

                    return FindPoleByQF(qf, checkStructureTable, checkStructureTable);
                    */
#endif
                    return null;
                }
            }
            finally
            {
            }
        }

        private void RemovePoleFromCache(Pole poleToFind)
        {
            try
            {
                CachedFeature testFeature = new CachedFeature(null, poleToFind.Guid, poleToFind.SAPEquipID);
                int cachedFeatureIndex = CachedFeatures.IndexOf(testFeature);
                if (cachedFeatureIndex >= 0)
                {
                    //If this feature was already deleted, then we need to return null
                    CachedFeatures.Remove(testFeature);
                }
            }
            finally
            {
            }
        }

        /// <summary>
        /// Returns the structure if it is exists from the support structure table. If the support structure table doesn't contain the structure
        /// then it will return the structure from the staging table if it exists there.  If neither table contains the structure, then null is returned.
        /// </summary>
        /// <param name="poleToFind"></param>
        /// <returns></returns>
        private IFeature FindPoleByQF(IQueryFilter qf, bool checkStructureTable, bool checkStagingTable)
        {
            IFeature structure = null;
            if (checkStructureTable)
            {
                structure = GetStructure(SupportStructure, qf);
                if (structure != null) { return structure; }
            }

            if (checkStagingTable)
            {
                //Check the staging table next
                structure = GetStructure(SupportStructureStaging, qf);
            }
            return structure;
        }

        /// <summary>
        /// Searchs the feature class for the desired structure.
        /// </summary>
        /// <param name="poleToFind"></param>
        /// <param name="StructureTable"></param>
        /// <returns></returns>
        private IFeature GetStructure(IFeatureClass StructureTable, IQueryFilter qf)
        {
            //Search for the structure in the specified table
            //int featureCount = StructureTable.FeatureCount(qf);
            //if (featureCount > 0)
            //{
            IFeatureCursor featCursor = StructureTable.Search(qf, false);
            IFeature feature = null;

            try { feature = featCursor.NextFeature(); }
            catch { }

            while (Marshal.ReleaseComObject(featCursor) > 0) { }
            return feature;
            //}

            //return null;
        }

        private void CheckRequiredAttributes(PTTTransaction transaction)
        {
            try
            {
                if (transaction.Type == TransactionType.Insert || transaction.Type == TransactionType.Replace)
                {
                    List<string> missingAttributes = new List<string>();

                    PoleInsert insertPole = null;
                    if (transaction.Type == TransactionType.Insert) { insertPole = transaction.TransactionPole as PoleInsert; }
                    else { insertPole = ((PoleReplace)transaction.TransactionPole).PoleToInsert; }

                    if (string.IsNullOrEmpty(insertPole.SAPEquipID)) { insertPole.MissingAttributes = "EQUIPMENTNUMBER_I"; }
                    //if (!insertPole.FieldValues.ContainsKey("PoleClass_I") || string.IsNullOrEmpty(insertPole.FieldValues["PoleClass_I"])) { insertPole.MissingAttributes = "PoleClass_I"; }
                    //if (!insertPole.FieldValues.ContainsKey("Height_I") || string.IsNullOrEmpty(insertPole.FieldValues["Height_I"])) { insertPole.MissingAttributes = "Height_I"; }
                    //if (!insertPole.FieldValues.ContainsKey("Material_I") || string.IsNullOrEmpty(insertPole.FieldValues["Material_I"])) { insertPole.MissingAttributes = "Material_I"; }
                    //if (!insertPole.FieldValues.ContainsKey("PlantSection_I") || string.IsNullOrEmpty(insertPole.FieldValues["PlantSection_I"])) { insertPole.MissingAttributes = "PlantSection_I"; }
                    //if (!insertPole.FieldValues.ContainsKey("StartupDate_I") || string.IsNullOrEmpty(insertPole.FieldValues["StartupDate_I"])) { insertPole.MissingAttributes = "StartupDate_I"; }
                    if (string.IsNullOrEmpty(insertPole.Latitude.ToString()) || insertPole.Latitude == 0.0) { insertPole.MissingAttributes = "Latitude_I"; }
                    if (string.IsNullOrEmpty(insertPole.Longitude.ToString()) || insertPole.Longitude == 0.0) { insertPole.MissingAttributes = "Longitude_I"; }
                   
                    if (string.IsNullOrEmpty(insertPole.ProjectID)) { insertPole.MissingAttributes = "ProjectID_I"; }

                    //Fix 13th Jan 2021: Making the Pole Height Field & Plant Section (County) field Non- Mandatory as per the approval from Business [Mark Choiniere]
                    //to fix the ED11 interface issue as we are getting Null values from SAP system for some cases
                    //if (string.IsNullOrEmpty(insertPole.Height)) { insertPole.MissingAttributes = "Height_I"; }
                   // if (string.IsNullOrEmpty(insertPole.PoleClass)) { insertPole.MissingAttributes = "PoleClass_I"; }
                   // if (string.IsNullOrEmpty(insertPole.Material)) { insertPole.MissingAttributes = "Material_I"; }

                    //if (string.IsNullOrEmpty(insertPole.Height)) { insertPole.MissingAttributes = "Height"; }
                    if (string.IsNullOrEmpty(insertPole.PoleClass)) { insertPole.MissingAttributes = "PoleClass"; }
                    //if (string.IsNullOrEmpty(insertPole.Material)) { insertPole.MissingAttributes = "Material_I"; }

                    if (string.IsNullOrEmpty(insertPole.StartupDate)) { insertPole.MissingAttributes = "StartupDate_I"; }
                    //if (string.IsNullOrEmpty(insertPole.PlantSection)) { insertPole.MissingAttributes = "PlantSection_I"; }
                    if (string.IsNullOrEmpty(transaction.TransactionPole.MissingAttributes))
                    {
                        transaction.TransactionPole.MissingAttributes = insertPole.MissingAttributes;
                    }
                }
                if (transaction.Type == TransactionType.Delete || transaction.Type == TransactionType.Replace)
                {
                    List<string> missingAttributes = new List<string>();

                    PoleDelete deletePole = null;
                    if (transaction.Type == TransactionType.Delete) { deletePole = transaction.TransactionPole as PoleDelete; }
                    else if (transaction.Type == TransactionType.Replace) { deletePole = ((PoleReplace)transaction.TransactionPole).PoleToDelete; }

                    if (string.IsNullOrEmpty(deletePole.SAPEquipID)) { deletePole.MissingAttributes = "EQUIPMENTNUMBER_D"; }
                    if (string.IsNullOrEmpty(deletePole.ProjectID)) { deletePole.MissingAttributes = "ProjectID_D"; }

                    if (string.IsNullOrEmpty(transaction.TransactionPole.MissingAttributes))
                    {
                        transaction.TransactionPole.MissingAttributes = deletePole.MissingAttributes;
                    }
                }
                if (transaction.Type == TransactionType.Update)
                {
                    List<string> missingAttributes = new List<string>();

                    PoleUpdate updatePole = transaction.TransactionPole as PoleUpdate;
                    if (string.IsNullOrEmpty(updatePole.SAPEquipID)) { updatePole.MissingAttributes = "EQUIPMENTNUMBER_U"; }
                    if (string.IsNullOrEmpty(updatePole.ProjectID)) { updatePole.MissingAttributes = "ProjectID_U"; }

                    //If the old value is truly null in GIS then we need to allow for OldValue to be null or empty string
                    //if (string.IsNullOrEmpty(updatePole.OldFieldValue)) { missingAttributes.Add("OldValue"); }
                    if (string.IsNullOrEmpty(updatePole.NewFieldValue)) { updatePole.MissingAttributes = "NewValue_U"; }

                    if (string.IsNullOrEmpty(transaction.TransactionPole.MissingAttributes))
                    {
                        transaction.TransactionPole.MissingAttributes = updatePole.MissingAttributes;
                    }
                }
            }
            catch (Exception ex)
            {
                //Failed checking required attributes
                throw new Exception("Failed checking required attributes: " + ex.Message);
            }
        }


        /// <summary>
        /// Insert the new pole
        /// </summary>
        /// <param name="poleToInsert"></param>
        /// <returns></returns>
        private bool ProcessInsert(PTTTransaction transaction, bool insertInStagingTable, bool replaceOperation)
        {
            PoleInsert poleToInsert = transaction.TransactionPole as PoleInsert;
            bool success = true;
            IFeature newStructure = null;
            IFeatureClass supportStructureFC = SupportStructure;
            if (insertInStagingTable) { supportStructureFC = SupportStructureStaging; }

            string info = "";
            if (!(((Pole)poleToInsert).Guid == Guid.Empty)) { info = GlobalIDFieldName + ": " + ((Pole)poleToInsert).Guid.ToString("B").ToUpper(); }
            else { info = SAPEquipIDFieldName + ": " + ((Pole)poleToInsert).SAPEquipID; }

            IFeature existingStructure = null;

            //Only check the staging table if this is an insert operation. Both if it is a replace operation
            if (replaceOperation) { existingStructure = FindPole(poleToInsert, SupportStructure.OIDFieldName, true, true); }
            else { existingStructure = FindPole(poleToInsert, SupportStructure.OIDFieldName, false, true); }

            if (existingStructure != null)
            {
                //An existing staging feature already existing. Report an error
                int guidFieldIndex = FindFieldIndex(existingStructure.Class as IFeatureClass, GlobalIDFieldName);
                string guid = existingStructure.get_Value(guidFieldIndex).ToString();

                if (!replaceOperation)
                {
                    _log.Info("Unable to insert " + info + " due to already existing feature " + guid);
                    LogTransactionError(transaction, TransactionErrorType.DuplicateInsertFeature,
                        TransactionStatus.Failure, string.Format("Feature {0} has already been inserted", guid), "");
                    success = false;
                }
            }
            else
            {
                //We can now create our new structure and copy our attributes to it.
                newStructure = supportStructureFC.CreateFeature();

                //Bring new structures in with "In-Service" status to ensure ED06 processing
                int statusFieldIndex = FindFieldIndex(newStructure.Class as IFeatureClass, StatusFieldName);
                newStructure.set_Value(statusFieldIndex, 5);

                //Set our sap equipment ID field
                int sapEquipIDIdx = FindFieldIndex(newStructure.Class as IFeatureClass, SAPEquipIDFieldName);
                newStructure.set_Value(sapEquipIDIdx, poleToInsert.SAPEquipID);

                //Default Subtype of "Pole"
                IRowSubtypes rowSubtype = newStructure as IRowSubtypes;
                rowSubtype.SubtypeCode = 1;

                //Assign the passed in attributes
                SetAttributes(ref newStructure, poleToInsert.FieldValues);

                //Get our projected geometry and assign it to our new feature
                IPoint projectedPoint = ProjectLatLongPoint(poleToInsert.Latitude, poleToInsert.Longitude, ((IGeoDataset)supportStructureFC).SpatialReference);
                newStructure.Shape = projectedPoint as IGeometry;

                //Store our newly created feature
                SetInstallJobNumber(newStructure);
                newStructure.Store();

                int globalIdIdx = FindFieldIndex(newStructure.Class as IFeatureClass, GlobalIDFieldName);
                string guid = newStructure.get_Value(globalIdIdx).ToString();

                CachedFeatures.Add(new CachedFeature(newStructure, new Guid(guid), poleToInsert.SAPEquipID));
                if (!replaceOperation)
                {
                    _log.Info("Inserted new feature: " + info + " with GlobalID " + guid);
                    LogTransactionError(transaction, TransactionErrorType.Success, TransactionStatus.Success,
                        string.Format("Feature {0} has been inserted", guid), "");
                }
            }

            return success;
        }

        private static string installJobNumber = "";
        /// <summary>
        /// Sets the install job number field value based on the configured parameter in the ED11_Config table
        /// </summary>
        /// <param name="Structure"></param>
        private void SetInstallJobNumber(IRow Structure)
        {
            try
            {
                if (string.IsNullOrEmpty(installJobNumber))
                {
                    installJobNumber = "FIF";

                    ITable ED11ConfigTable = ((IFeatureWorkspace)((IDataset)Structure.Table).Workspace).OpenTable("EDGIS.PGE_ED11_CONFIG");
                    int configurationValueIDX = ED11ConfigTable.Fields.FindField("CONFIGURATIONVALUE");
                    IQueryFilter qf = new QueryFilterClass();
                    qf.SubFields = "CONFIGURATIONVALUE";
                    qf.WhereClause = "CONFIGURATIONNAME = 'INSTALLJOBNUMBER'";
                    ICursor rowCursor = ED11ConfigTable.Search(qf, false);
                    IRow row = null;
                    while ((row = rowCursor.NextRow()) != null)
                    {
                        object installJobNumberObj = row.get_Value(configurationValueIDX);
                        if (installJobNumberObj != DBNull.Value) { installJobNumber = row.get_Value(configurationValueIDX).ToString(); }
                    }
                }

                int installjobNumFldIdx = FindFieldIndex(Structure.Table as IFeatureClass, "installjobnumber");

                if (installjobNumFldIdx > 0)
                {
                    Structure.set_Value(installjobNumFldIdx, installJobNumber);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to determine INSTALLJOBNUMBER configuration value from EDGIS.PGE_ED11_CONFIG table: " + ex.Message);
            }
        }

        /// <summary>
        /// Delete the existing pole
        /// </summary>
        /// <param name="poleToDelete"></param>
        private bool ProcessDelete(PTTTransaction transaction)
        {
            PoleDelete poleToDelete = transaction.TransactionPole as PoleDelete;
            bool success = true;
            ISubtypes tableSubtypes = SupportStructure as ISubtypes;
            IFeature existingStructure = FindPole(poleToDelete, "PTTDIDC," + tableSubtypes.SubtypeFieldName + ",SYMBOLNUMBER", true, true);

            if (existingStructure != null)
            {
                //Is this a "ghost" pole?
                //Subtype = 7 and Symbol Number = 96 or Null
                int symbolNumberIdx = FindFieldIndex(existingStructure.Class as IFeatureClass, "SYMBOLNUMBER");
                object symbolNumberObject = existingStructure.get_Value(symbolNumberIdx);
                IRowSubtypes rowSubtypes = existingStructure as IRowSubtypes;
                if (rowSubtypes.SubtypeCode == 7 && (symbolNumberObject == null || symbolNumberObject == DBNull.Value || Int32.Parse(symbolNumberObject.ToString()) == 96))
                {
                    //This is a ghost pole
                    _log.Info("Deleting structure " + poleToDelete.ToString());
                    existingStructure.Delete();
                    DeletedFeatures.Add(poleToDelete.Guid);
                    LogTransactionError(transaction, TransactionErrorType.Success, TransactionStatus.Success,
                        string.Format("Deleted Pole {0}", poleToDelete.Guid.ToString("B").ToUpper()), poleToDelete.Guid.ToString("B").ToUpper());
                    RemovePoleFromCache(poleToDelete);
                }
                else
                {
                    //This is not a ghost pole.  Let's search for a nearby ghost pole
                    ISpatialFilter sf = new SpatialFilterClass();
                    sf.Geometry = ((ITopologicalOperator)existingStructure.Shape).Buffer(Common.GhostPoleSearchBuffer);
                    sf.SpatialRel = esriSpatialRelEnum.esriSpatialRelContains;
                    sf.GeometryField = ((IFeatureClass)existingStructure.Table).ShapeFieldName;
                    sf.WhereClause = tableSubtypes.SubtypeFieldName + " = 7 AND (SYMBOLNUMBER IS NULL OR SYMBOLNUMBER = 96)";
                    sf.SubFields = SupportStructure.OIDFieldName;

                    IFeature nearbyStructure = GetStructure(SupportStructure, sf);
                    //Shouldn't need to check the staging table for a delete
                    //if (nearbyStructure == null)
                    //{
                    //Nothing in structure table.  Let's check the staging table
                    //    nearbyStructure = GetStructure(SupportStructureStaging, sf);
                    //}

                    if (nearbyStructure != null)
                    {
                        //Nearby ghost structure was found, let's go ahead and delete that one.
                        //Nearby ghost pole found. Let's delete it
                        int globalIDIdx = FindFieldIndex(nearbyStructure.Class as IFeatureClass, GlobalIDFieldName);
                        string guid = nearbyStructure.get_Value(globalIDIdx).ToString();
                        _log.Info("Deleting ghost structure nearby " + poleToDelete.ToString() + " with GUID " + guid);
                        nearbyStructure.Delete();
                        DeletedFeatures.Add(new Guid(guid));
                        LogTransactionError(transaction, TransactionErrorType.DeletedAlternativeGhostPole,
                            TransactionStatus.Failure, string.Format("Instead of {0} Deleted Alternative Pole {1}", poleToDelete.Guid.ToString("B").ToUpper(), guid.ToUpper()), guid.ToUpper());
                        success = true;
                        if (nearbyStructure != null) { while (Marshal.ReleaseComObject(nearbyStructure) > 0) { } }
                    }
                    else
                    {
                        //There is no nearby ghost structure.  Let's go ahead and set the PTTDIDC attribute
                        _log.Info("Setting PTTDIDC to Yes for structure " + poleToDelete.ToString());
                        int PTTIdcIdx = FindFieldIndex(existingStructure.Class as IFeatureClass, "PTTDIDC");
                        existingStructure.set_Value(PTTIdcIdx, "Y");
                        existingStructure.Store();
                        LogTransactionError(transaction, TransactionErrorType.Success, TransactionStatus.Success,
                            string.Format("Set PTTDIDC for Pole {0}", poleToDelete.Guid.ToString("B").ToUpper()), "");
                    }

                    if (sf != null) { while (Marshal.ReleaseComObject(sf) > 0) { } }
                }
            }
            else
            {
                //No associated feature found to delete
                if (poleToDelete.Guid == Guid.Empty)
                {
                    LogTransactionError(transaction, TransactionErrorType.DeleteMissingFeature, TransactionStatus.Failure,
                        string.Format("Pole GUID {0} Does Not Exist and SAPEquipID {1} Does Not Exist", poleToDelete.Guid.ToString("B").ToUpper(), poleToDelete.SAPEquipID.ToString()), "");
                }
                else
                {
                    LogTransactionError(transaction, TransactionErrorType.DeleteMissingFeature, TransactionStatus.Failure,
                        string.Format("Pole GUID {0} Does Not Exist", poleToDelete.Guid.ToString("B").ToUpper()), "");
                }
                success = false;
            }
            return success;
        }

        private static SymbolNumberAU symbolNumAU = new SymbolNumberAU();
        private static SupportStructureLabel supportStructureAU = new SupportStructureLabel();
       
        /// <summary>
        /// Update the existing pole
        /// </summary>
        /// <param name="poleToUpdate"></param>
        /// <returns></returns>
        private bool ProcessUpdate(PTTTransaction transaction)
        {
            PoleUpdate poleToUpdate = transaction.TransactionPole as PoleUpdate;
            bool success = true;
            //Find the existing pole to update
            IFeature existingStructure = FindPole(poleToUpdate, SupportStructure.OIDFieldName, true, true);

            if (existingStructure != null)
            {
                //Obtain the existing guid feature in case the transaction doesn't actually have it
                int globalIDIdx = FindFieldIndex(existingStructure.Class as IFeatureClass, GlobalIDFieldName);
                string guid = existingStructure.get_Value(globalIDIdx).ToString();
                Guid existingStructureGUID = new Guid(guid);

                string gisFieldName = SAPToGISMappings.Instance.GetGISMappedField(poleToUpdate.FieldName);
                int fieldIndex = FindFieldIndex(existingStructure.Class as IFeatureClass, gisFieldName);
                object currentValueObj = existingStructure.get_Value(fieldIndex);

                object oldValueToCheckObj = GetDomainValue(existingStructure, fieldIndex, poleToUpdate.OldFieldValue, false);

                string oldValueToCheck = "";
                if (oldValueToCheckObj != null) { oldValueToCheck = oldValueToCheckObj.ToString(); }
                string currentValue = "";
                if (currentValueObj != null) { currentValue = currentValueObj.ToString(); }

                if (poleToUpdate.FieldName.ToUpper() == "INSTALLATIONDATE" || poleToUpdate.FieldName.ToUpper() == "STARTUPDATE")
                {
                    //This is the installation date field.  SAP only holds the year, so we need to only check the year attribute
                    if (currentValueObj != DBNull.Value)
                    {
                        currentValue = ((DateTime)currentValueObj).Year.ToString();
                    }
                    if (oldValueToCheckObj != DBNull.Value)
                    {
                        try
                        {
                            oldValueToCheck = ((DateTime)oldValueToCheckObj).Year.ToString();
                        }
                        catch (Exception ex)
                        {
                            //It's possible that SAP sends us the integer for the year already.  So if something fails casting to the datetime value to get the
                            //year, then we will do nothing and the below comparison of old and new value will go ahead and throw it out
                        }
                    }
                }

                //Fix : 08-Apr-21 : allowing to update installation date if edgis has null and sap has some other default dates -- Added condition at last || (poleToUpdate.FieldName.ToUpper() == "INSTALLATIONDATE" &&!string.IsNullOrEmpty(oldValueToCheck.ToString()))
                //EGIS-1032 Fix : 17-Aug-21 allowing to update Barcode, install year, Pole class, Pole height and Pole material if edgis has null values
                if (ForceUpdateTransactions || ((oldValueToCheckObj == DBNull.Value && currentValueObj == DBNull.Value)
                    || (oldValueToCheck.ToString() == currentValue.ToString())) || (poleToUpdate.FieldName.ToUpper() == "INSTALLATIONDATE" && !string.IsNullOrEmpty(oldValueToCheck.ToString()))
                     || (poleToUpdate.FieldName.ToUpper() == "BARCODE" && (!string.IsNullOrEmpty(oldValueToCheck.ToString()) || currentValue.ToString() == "0"))
                     || (poleToUpdate.FieldName.ToUpper() == "CLASS" && !string.IsNullOrEmpty(oldValueToCheck.ToString()))
                     || (poleToUpdate.FieldName.ToUpper() == "MATERIAL" && !string.IsNullOrEmpty(oldValueToCheck.ToString()))
                     || (poleToUpdate.FieldName.ToUpper() == "HEIGHT" && (!string.IsNullOrEmpty(oldValueToCheck.ToString()) || currentValue.ToString() == "0"))
                     || (poleToUpdate.FieldName.ToUpper() == "INSTALLJOBYEAR" && !string.IsNullOrEmpty(oldValueToCheck.ToString())))
                {
                    //Values match.  //Let's go ahead and update the GIS attribute to the new attribute
                    object newValueToSet = GetDomainValue(existingStructure, fieldIndex, poleToUpdate.NewFieldValue, true);

                    _log.Info("Updating field " + poleToUpdate.FieldName + " for " + poleToUpdate.ToString() + " Old value: " + oldValueToCheckObj + " New value: " + newValueToSet);

                    existingStructure.set_Value(fieldIndex, newValueToSet);
                    symbolNumAU.Execute(existingStructure, mmAutoUpdaterMode.mmAUMArcMap, mmEditEvent.mmEventFeatureUpdate);
                    supportStructureAU.Execute(existingStructure, mmAutoUpdaterMode.mmAUMStandAlone, mmEditEvent.mmEventFeatureUpdate);
                    
                    //Fix : 22-Oct-20 : Checking Null PLDBID for existing pole. If PLDBID is null then populating it by calling PLDID AU
                    int pldbidIDIdx = FindFieldIndex(existingStructure.Class as IFeatureClass, PLDBIDIDFieldName);
                    // m4jf
                    Console.WriteLine("pldbidIDIdx index" + pldbidIDIdx + existingStructure.Class.AliasName.ToString());

                    int installJobIdx = FindFieldIndex(existingStructure.Class as IFeatureClass, "installjobnumber");
                    if (pldbidIDIdx != -1)
                    {
                        Console.WriteLine("Checking PLDBID for OID : " + existingStructure.OID.ToString());
                        _log.Info("Checking PLDBID for OID :  " + existingStructure.OID.ToString());
                        string pldbidValue = existingStructure.get_Value(pldbidIDIdx).ToString();
                        if (string.IsNullOrEmpty(pldbidValue))
                        {
                            Console.WriteLine("Null PLDBID found for OID : " + existingStructure.OID.ToString());
                            _log.Info("Null PLDBID found for OID : " + existingStructure.OID.ToString());
                            Console.WriteLine("Poulating PLDBID for OID : " + existingStructure.OID.ToString());
                            _log.Info("Populating PLDBID for OID : " + existingStructure.OID.ToString());
                            // PGEUpdateElevationAU.callByInterfaceED11 = true;
                            PGE.Desktop.EDER.AutoUpdaters.Special.PLC.PGEUpdateElevationAU.callByInterfaceED11 = true;
                            PGEUpdateElevationAU.PTTlandbaseWksp = LandbaseWorkspace;
                            PGEUpdateElevationAU.PTTSessionWksp = EditVersion as IWorkspace;

                            // m4jf - edgisrearch - 415 - Interface user name is sent to AU to fix populate pldbid value
                            PGEUpdateElevationAU.ED11interfaceUser = Common.ED11UserName;
                          
                            if (installJobIdx != -1)
                            {
                                PGEUpdateElevationAU.PTTJobNumber = existingStructure.get_Value(installJobIdx).ToString();
                            }
                            // Fix - M4JF - EDGISREARCH 415 - Autoupdator function is called directly to populate pldbid value
                            // pldbidAU.Execute(existingStructure, mmAutoUpdaterMode.mmAUMArcMap, mmEditEvent.mmEventFeatureCreate);
                            pldbidAU.UpdateFeature(existingStructure);
       
                            PGEUpdateElevationAU.callByInterfaceED11 = false;
                        }
                    }

                    existingStructure.Store();
                    LogTransactionError(transaction, TransactionErrorType.Success, TransactionStatus.Success,
                        string.Format("Pole GUID {0} had attribute {1} updated from {2} to {3}", existingStructureGUID.ToString("B").ToUpper(), poleToUpdate.FieldName,
                        poleToUpdate.OldFieldValue, poleToUpdate.NewFieldValue), "");
                }
                else
                {
                    //Current value doesn't match old value.  Let's report an error
                    _log.Info("Unable to update field " + poleToUpdate.FieldName + " for " + poleToUpdate.ToString() + " as the old values do not match");
                    LogTransactionError(transaction, TransactionErrorType.UpdateConflict, TransactionStatus.Failure,
                        string.Format("Pole GUID {0} has attribute conflict for {1} Old value vs New value", existingStructureGUID.ToString("B").ToUpper(), poleToUpdate.FieldName), "");
                    success = false;
                }
            }
            else
            {
                //No associated feature found to update
                _log.Info("Unable to update " + poleToUpdate.ToString() + ". Could not find GIS structure");
                if (poleToUpdate.Guid == Guid.Empty)
                {
                    LogTransactionError(transaction, TransactionErrorType.UpdateMissingFeature, TransactionStatus.Failure,
                        string.Format("Pole GUID {0} Does Not Exist and SAPEquipID {1} Does Not Exist", poleToUpdate.Guid.ToString("B").ToUpper(), poleToUpdate.SAPEquipID.ToString()), "");
                }
                else
                {
                    LogTransactionError(transaction, TransactionErrorType.UpdateMissingFeature, TransactionStatus.Failure,
                        string.Format("Pole GUID {0} Does Not Exist", poleToUpdate.Guid.ToString("B").ToUpper()), "");
                }
                success = false;
            }
            return success;
        }

        /// <summary>
        /// Replace the existing pole
        /// </summary>
        /// <param name="transaction"></param>
        /// <returns></returns>
        private bool ProcessReplace(PTTTransaction transaction)
        {
            PoleReplace poleToReplace = transaction.TransactionPole as PoleReplace;
            bool success = true;
            /*
             * This replace operation must be modified to use the same logic as the PGE - Asset Replace functionality.  After, the attributes that were passed in with the 
             * replace transaction should be utilized and updated for the new feature.
             */
            //Find the existing pole to delete
            IFeature existingStructure = FindPole(poleToReplace.PoleToDelete, SupportStructure.OIDFieldName, true, true);

            if (existingStructure != null)
            {


                int globalIDIdx = FindFieldIndex(existingStructure.Class as IFeatureClass, GlobalIDFieldName);
                Guid deletedFeatureGUID = new Guid(existingStructure.get_Value(globalIDIdx).ToString());

#if DEBUG
                //Debugger.Launch();
#endif

                //Get list of annotation features existing prior to replace
                List<IFeature> existingAnnotationFeatures = DetermineRelatedAnnotationsFeatures(existingStructure);

                IFeature newStructure = ReplaceAssetLogic(existingStructure, poleToReplace);

                //Get our projected geometry and assign it to our new feature.  For now, we will just use the original geometry during a replace
                //IPoint projectedPoint = ProjectLatLongPoint(poleToReplace.PoleToInsert.Latitude, poleToReplace.PoleToInsert.Longitude, ((IGeoDataset)newStructure.Class).SpatialReference);
                //newStructure.Shape = projectedPoint as IGeometry;

                string newFeatureGuid = newStructure.get_Value(globalIDIdx).ToString();

                //Update the replaceguid field on the new structure to the one we are deleting
                Guid replaceGuid = poleToReplace.PoleToDelete.Guid;
                int replaceGuidIdx = FindFieldIndex(newStructure.Class as IFeatureClass, ReplaceGuidFieldName);
                string deletedGuid = replaceGuid.ToString("B").ToUpper();
                newStructure.set_Value(replaceGuidIdx, deletedGuid);
                SetInstallJobNumber(newStructure);
                //Fix : 22-Oct-20 : Checking Null PLDBID for newly created pole. If PLDBID is null then populating it by calling PLDID AU
                int pldbidIDIdx = FindFieldIndex(newStructure.Class as IFeatureClass, PLDBIDIDFieldName);
                int installJobIdx = FindFieldIndex(newStructure.Class as IFeatureClass, "installjobnumber");
                if (pldbidIDIdx != -1)
                {
                    Console.WriteLine("Checking PLDBID for OID : " + newStructure.OID.ToString());
                    _log.Info("Checking PLDBID for OID :  " + newStructure.OID.ToString());
                    string pldbidValue = newStructure.get_Value(pldbidIDIdx).ToString();
                    if (string.IsNullOrEmpty(pldbidValue))
                    {
                        Console.WriteLine("Null PLDBID found for OID : " + newStructure.OID.ToString());
                        _log.Info("Null PLDBID found for OID : " + newStructure.OID.ToString());
                        Console.WriteLine("Poulating PLDBID for OID : " + newStructure.OID.ToString());
                        _log.Info("Populating PLDBID for OID : " + newStructure.OID.ToString());
                        PGEUpdateElevationAU.callByInterfaceED11 = true;
                        PGEUpdateElevationAU.PTTlandbaseWksp = LandbaseWorkspace;
                        PGEUpdateElevationAU.PTTSessionWksp = EditVersion as IWorkspace;
                        //m4jf - edgisrearch 415 - Interface user name is sent to AU to fix PLDBID AU 
                        PGEUpdateElevationAU.ED11interfaceUser = Common.ED11UserName;
                        if (installJobIdx != -1)
                        {
                            PGEUpdateElevationAU.PTTJobNumber = newStructure.get_Value(installJobIdx).ToString();
                        }
                        //pldbidAU.Execute(newStructure, mmAutoUpdaterMode.mmAUMArcMap, mmEditEvent.mmEventFeatureCreate);
                        //m4jf edgisrearch 415 - calling AU function directly to populate pldbid value
                        //pldbidAU.Execute(newStructure, mmAutoUpdaterMode.mmAUMArcMap, mmEditEvent.mmEventFeatureCreate);
                        pldbidAU.UpdateFeature(newStructure);
                        PGEUpdateElevationAU.callByInterfaceED11 = false;
                    }
                }

                newStructure.Store();

                //Determine the new annotation features and delete them as we don't want to create any new annotation features
                List<IFeature> newAnnotationFeatures = DetermineRelatedAnnotationsFeatures(newStructure);
                foreach (IFeature newAnno in newAnnotationFeatures)
                {
                    bool newAnnotation = true;
                    foreach (IFeature oldAnno in existingAnnotationFeatures)
                    {
                        if (oldAnno.Class == newAnno.Class && oldAnno.OID == newAnno.OID) { newAnnotation = false; }
                    }

                    if (newAnnotation) { newAnno.Delete(); }
                }

                //Notify the existing annotations that the related feature was updated to ensure that the annotation is updated accordingly.
                NotifyAnnotationOfChanges(newStructure, existingAnnotationFeatures);

                CachedFeatures.Add(new CachedFeature(newStructure, new Guid(newFeatureGuid), poleToReplace.PoleToInsert.SAPEquipID));

                int globalIDIdx2 = FindFieldIndex(newStructure.Class as IFeatureClass, GlobalIDFieldName);
                string newStructureGuid = newStructure.get_Value(globalIDIdx2).ToString();
                _log.Info("Deleted structure " + deletedFeatureGUID.ToString("B").ToUpper() + " and replaced with new structure " + newStructureGuid.ToUpper());
                LogTransactionError(transaction, TransactionErrorType.Success, TransactionStatus.Success,
                    "Deleted pole " + deletedFeatureGUID.ToString("B").ToUpper() + " and replaced with new pole " + newStructureGuid.ToUpper(), "");

                //Remove the deleted pole from our cache features list
                RemovePoleFromCache(poleToReplace.PoleToDelete);

                //Add this replaced feature to our cached list just in case there are two replace operations for the same pole
                //if (replaceGuid != Guid.Empty && !AlreadyReplacedFeatures.ContainsKey(replaceGuid)) { AlreadyReplacedFeatures.Add(replaceGuid, newStructure); }
                if (replaceGuid == Guid.Empty && !AlreadyReplacedFeaturesByEquipID.ContainsKey(poleToReplace.PoleToDelete.SAPEquipID)) { AlreadyReplacedFeaturesByEquipID.Add(poleToReplace.PoleToDelete.SAPEquipID, newStructure); }
                //else
                //{

                //}
            }
            else
            {
                //No existing structure was found matching the pole to delete.  Let's check if it has already been replaced
                IQueryFilter qf = new QueryFilterClass();
                qf.WhereClause = ReplaceGuidFieldName + " = '" + poleToReplace.PoleToDelete.Guid.ToString("B").ToUpper() + "'";
                qf.SubFields = SupportStructure.OIDFieldName + "," + GlobalIDFieldName;

                IFeature alreadyReplacedFeature = FindPoleByQF(qf, true, true);
                if (alreadyReplacedFeature == null && AlreadyReplacedFeaturesByEquipID.ContainsKey(poleToReplace.PoleToDelete.SAPEquipID)) { alreadyReplacedFeature = AlreadyReplacedFeaturesByEquipID[poleToReplace.PoleToDelete.SAPEquipID]; }

                if (poleToReplace.PoleToDelete.Guid == Guid.Empty || alreadyReplacedFeature == null)
                {
                    //No pole to replace was found and no existing replaced feature was found
                    if (poleToReplace.PoleToDelete.Guid == Guid.Empty)
                    {
                        LogTransactionError(transaction, TransactionErrorType.DeleteMissingFeature,
                            TransactionStatus.Failure, string.Format("Pole GUID {0} Does Not Exist and SAPEquipID {1} Does Not Exist", poleToReplace.PoleToDelete.Guid.ToString("B").ToUpper(), poleToReplace.PoleToDelete.SAPEquipID), "");
                        _log.Info("Structure with GUID " + poleToReplace.PoleToDelete.Guid.ToString("B").ToUpper() + " and SAPEquipID " + poleToReplace.PoleToDelete.SAPEquipID + " could not be found to replace");
                    }
                    else
                    {
                        LogTransactionError(transaction, TransactionErrorType.DeleteMissingFeature,
                            TransactionStatus.Failure, string.Format("Pole GUID {0} Does Not Exist", poleToReplace.PoleToDelete.Guid.ToString("B").ToUpper()), "");
                        _log.Info("Structure " + poleToReplace.PoleToDelete.Guid.ToString("B").ToUpper() + " could not be found to replace");
                    }
                    success = false;
                }
                else
                {
                    int globalIdIdx = FindFieldIndex(alreadyReplacedFeature.Class as IFeatureClass, GlobalIDFieldName);
                    string alreadyReplacedFeatureGuid = alreadyReplacedFeature.get_Value(globalIdIdx).ToString();

                    if (poleToReplace.PoleToDelete.Guid == Guid.Empty)
                    {
                        _log.Info("Structure with SAPEquipID " + poleToReplace.PoleToDelete.SAPEquipID.ToString() + " has already been replaced by Structure " + alreadyReplacedFeatureGuid.ToUpper());
                        LogTransactionError(transaction, TransactionErrorType.DuplicateReplace, TransactionStatus.Failure,
                            "Pole " + poleToReplace.PoleToDelete.SAPEquipID.ToString() + " has already been replaced by pole " + alreadyReplacedFeatureGuid.ToUpper(), "");
                    }
                    else
                    {
                        _log.Info("Structure " + poleToReplace.PoleToDelete.Guid.ToString("B").ToUpper() + " has already been replaced by Structure " + alreadyReplacedFeatureGuid.ToUpper());
                        LogTransactionError(transaction, TransactionErrorType.DuplicateReplace, TransactionStatus.Failure,
                            "Pole " + poleToReplace.PoleToDelete.Guid.ToString("B").ToUpper() + " has already been replaced by pole " + alreadyReplacedFeatureGuid.ToUpper(), "");
                    }
                }
            }

            return success;
        }

        private List<IFeature> DetermineRelatedAnnotationsFeatures(IFeature feature)
        {
            List<IFeature> originalAnnotationFeatures = new List<IFeature>();
            if (feature != null)
            {
                //Process relationships where this class is the destination
                FeatureClassInformation featClassInfo = GetFeatureClassInformation(feature.Class as IFeatureClass);

                if (featClassInfo != null)
                {
                    //IFeatureClass featClass = oldFeature.Class as IFeatureClass;
                    IObjectClass objClass = feature.Class as IObjectClass;

                    //Process relationships where this class is the origin
                    foreach (IRelationshipClass relClass in featClassInfo.OriginRelationshipClasses)
                    {
                        //Don't map annotation relationships
                        if (relClass.DestinationClass is IFeatureClass && ((IFeatureClass)relClass.DestinationClass).FeatureType == esriFeatureType.esriFTAnnotation)
                        {
                            ISet relatedFeatures = relClass.GetObjectsRelatedToObject(feature);
                            relatedFeatures.Reset();
                            for (int i = 0; i < relatedFeatures.Count; i++)
                            {
                                IFeature annoFeature = relatedFeatures.Next() as IFeature;
                                originalAnnotationFeatures.Add(annoFeature);
                            }
                        }
                    }
                }
            }

            return originalAnnotationFeatures;
        }

        private void NotifyAnnotationOfChanges(IFeature newFeature, List<IFeature> annotationFeatures)
        {
#if DEBUG
            //Debugger.Launch();
#endif
            if (newFeature != null)
            {
                //Process relationships where this class is the destination
                FeatureClassInformation featClassInfo = GetFeatureClassInformation(newFeature.Class as IFeatureClass);

                if (featClassInfo != null)
                {
                    //IFeatureClass featClass = oldFeature.Class as IFeatureClass;
                    IObjectClass objClass = newFeature.Class as IObjectClass;

                    //Process relationships where this class is the origin
                    foreach (IRelationshipClass relClass in featClassInfo.OriginRelationshipClasses)
                    {
                        //Don't map annotation relationships
                        if (relClass.DestinationClass is IFeatureClass && ((IFeatureClass)relClass.DestinationClass).FeatureType == esriFeatureType.esriFTAnnotation)
                        {
                            foreach (IFeature annoFeature in annotationFeatures)
                            {
                                if (annoFeature.Class == relClass.DestinationClass)
                                {
                                    //Correct annotation relationship class found
                                    IRelatedObjectEvents relObjectEvents = annoFeature as IRelatedObjectEvents;
                                    relObjectEvents.RelatedObjectChanged(relClass, newFeature);
                                }
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Replace logic code modified from the PGE Asset Replace Desktop functionality
        /// </summary>
        private IFeature ReplaceAssetLogic(IFeature oldObject, PoleReplace poleToReplace)
        {
            IFeature newFeature = null;

            //Set flag to indicate Enable Symbol Number AU 
            EvaluationEngine.Instance.EnableSymbolNumberAU = true;

            Dictionary<int, object> fieldMapping = null;
            Dictionary<IRelationshipClass, List<IObject>> relationshipMapping = null;
            try
            {
                IFeatureClass objectClass = oldObject.Class as IFeatureClass;

                //Get list of fields with modelname PGE_ASSETCOPY which will be 
                //copied from the old object to the new object 
                fieldMapping = GetValuesToMap(oldObject);

                //Update relationships from old feature to new feature
                relationshipMapping = GetRelationshipMapping(oldObject, false);

                int installJobPrefixFldIdx = FindFieldIndex(oldObject.Class as IFeatureClass, "installjobprefix");

                //Give the install job prefix the default value for the field
                string installJobPrefix = "";
                if (!fieldMapping.ContainsKey(installJobPrefixFldIdx))
                {
                    installJobPrefix = oldObject.Fields.get_Field(
                        installJobPrefixFldIdx).DefaultValue.ToString();
                }
                string jobNumberFromMemory = PGE.Desktop.EDER.ArcMapCommands.
                    PopulateLastJobNumberUC.jobNumber;

                //Set flag to indicate Disable Symbol Number AU 
                EvaluationEngine.Instance.EnableSymbolNumberAU = false;

                //Create our new feature
                IObject newObject = CreateNewObject(fieldMapping, objectClass, oldObject);
                IFeature newStructure = newObject as IFeature;

                //Moving this code here to ensure that any autoupdaters that fire after setting attributes can
                //do their job properly in case they only execute during insert operations and not update
                //Bring new structures in with "In-Service" status to ensure ED06 processing
                int statusFieldIdx = FindFieldIndex(newStructure.Class as IFeatureClass, StatusFieldName);
                newStructure.set_Value(statusFieldIdx, 5);
                //Set our sap equipment ID field
                int sapEquipIdx = FindFieldIndex(newStructure.Class as IFeatureClass, SAPEquipIDFieldName);
                newStructure.set_Value(sapEquipIdx, poleToReplace.PoleToInsert.SAPEquipID);
                //Default Subtype of "Pole"
                IRowSubtypes rowSubtype = newStructure as IRowSubtypes;
                rowSubtype.SubtypeCode = 1;

                //Assign the passed in attributes
                SetAttributes(ref newStructure, poleToReplace.PoleToInsert.FieldValues);

                //Set all relationships back up with the new feature
                SetRelationships(relationshipMapping, newObject);

                int globalIDIdx = FindFieldIndex(oldObject.Class as IFeatureClass, GlobalIDFieldName);
                Guid guid = new Guid(oldObject.get_Value(globalIDIdx).ToString());

                oldObject.Delete();
                DeletedFeatures.Add(guid);

                //*******************************************************
                //INC000004070566 
                //Trigger the store to update the anno and make sure it is 
                //in sync with the source feature e.g. may include job number 
                int installjobNumFldIdx = FindFieldIndex(newObject.Class as IFeatureClass, "installjobnumber");
                newObject.set_Value(installJobPrefixFldIdx, installJobPrefix);
                newObject.set_Value(installjobNumFldIdx, jobNumberFromMemory);
                newFeature = newObject as IFeature;

                return newFeature;
                //******************************************************* 
            }
            catch (Exception e)
            {
                Console.WriteLine("Failed to replace asset: " + e.Message);
                throw new Exception("Failed to replace asset: " + e.Message);

            }
            finally
            {
                //Set flag to indicate Disable Symbol Number AU 
                EvaluationEngine.Instance.EnableSymbolNumberAU = false;
                //Cleanup
                if (fieldMapping != null)
                {
                    fieldMapping.Clear();
                }
            }
        }


        /// <summary>
        /// Relates all features that were related with the previous feature to the newly created feature
        /// </summary>
        /// <param name="relationshipMapping"></param>
        /// <param name="newObject"></param>
        private void SetRelationships(Dictionary<IRelationshipClass, List<IObject>> relationshipMapping, IObject newObject) //, IFeature newFeature
        {
            FeatureClassInformation featClassInfo = GetFeatureClassInformation(newObject.Class as IFeatureClass);

            //First delete any new anno that has been created for the new object 
            //since we are going to use the old anno 
            foreach (IRelationshipClass pRelClass in featClassInfo.OriginRelationshipClasses)
            {
                if (pRelClass.DestinationClass is IFeatureClass && ((IFeatureClass)pRelClass.DestinationClass).FeatureType ==
                    esriFeatureType.esriFTAnnotation)
                {
                    ISet pSet = pRelClass.GetObjectsRelatedToObject(newObject);
                    IObject pRelObj = (IObject)pSet.Next();
                    while (pRelObj != null)
                    {
                        pRelObj.Delete();
                        pRelObj = (IObject)pSet.Next();
                    }
                }
            }


            foreach (KeyValuePair<IRelationshipClass, List<IObject>> kvp in relationshipMapping)
            {
                bool isOrigin = false;
                if (kvp.Key.OriginClass == newObject.Class)
                {
                    isOrigin = true;
                }

                if (isOrigin)
                {
                    foreach (IObject obj in kvp.Value)
                    {
                        if (obj != null)
                        {
                            kvp.Key.CreateRelationship(newObject, obj);
                        }
                    }
                }
                else
                {
                    foreach (IObject obj in kvp.Value)
                    {
                        if (obj != null)
                        {
                            kvp.Key.CreateRelationship(obj, newObject);
                        }
                    }
                }
            }
        }



        /// <summary>
        /// This method will take all relationships that are current assigned to the oldFeature, remove
        /// them and then apply them to the new feature.
        /// </summary>
        /// <param name="oldObject">Feature being replaced</param>
        private Dictionary<IRelationshipClass, List<IObject>> GetRelationshipMapping(IObject oldObject, bool rebuildCache) //IFeature oldFeature
        {
            Dictionary<IRelationshipClass, List<IObject>> relationshipMapping = null;

            if (rebuildCache)
            {
                //Process relationships where this class is the destination
                FeatureClassInformation featClassInfo = GetFeatureClassInformation(SupportStructure);

                ISet setOfObjectsToCache = new SetClass();
                foreach (CachedFeature cachedFeature in CachedFeatures)
                {
                    if (cachedFeature.PoleFeature.Class == SupportStructure)
                    {
                        foreach (PTTTransaction transaction in PTTTransactions)
                        {
                            if (transaction.Type == TransactionType.Replace)
                            {
                                PoleDelete deletePole = ((PoleReplace)transaction.TransactionPole).PoleToDelete;

                                if ((deletePole.SAPEquipID == cachedFeature.SapEquipID) || (deletePole.Guid == cachedFeature.guid))
                                {
                                    //Go ahead and cache this feature
                                    setOfObjectsToCache.Add(cachedFeature.PoleFeature);
                                }
                            }
                        }
                    }
                }

                if (setOfObjectsToCache.Count > 0)
                {
                    //Obtain all of the destionation relationships for this set of objects
                    foreach (IRelationshipClass relClass in featClassInfo.DestinationRelationshipClasses)
                    {
                        IRelClassEnumRowPairs rowPairs = relClass.GetObjectsMatchingObjectSet(setOfObjectsToCache);
                        rowPairs.Reset();
                        IRow sourceRow = null;
                        IRow targetRow = null;
                        rowPairs.Next(out sourceRow, out targetRow);

                        while (sourceRow != null)
                        {
                            //Find our associated cached row
                            foreach (CachedFeature cachedFeature in CachedFeatures)
                            {
                                if (cachedFeature.PoleFeature == sourceRow)
                                {
                                    //Found our matching cached feature
                                    if (!cachedFeature.RelatedObjects.ContainsKey(relClass)) { cachedFeature.RelatedObjects.Add(relClass, new List<IObject>()); }
                                    cachedFeature.RelatedObjects[relClass].Add(targetRow as IObject);
                                    break;
                                }
                            }
                            rowPairs.Next(out sourceRow, out targetRow);
                        }
                    }

                    //Obtain all of the origin relationships for this set of objects
                    foreach (IRelationshipClass relClass in featClassInfo.OriginRelationshipClasses)
                    {
                        IRelClassEnumRowPairs rowPairs = relClass.GetObjectsMatchingObjectSet(setOfObjectsToCache);
                        rowPairs.Reset();
                        IRow sourceRow = null;
                        IRow targetRow = null;
                        rowPairs.Next(out sourceRow, out targetRow);

                        while (sourceRow != null)
                        {
                            //Find our associated cached row
                            foreach (CachedFeature cachedFeature in CachedFeatures)
                            {
                                if (cachedFeature.PoleFeature == sourceRow)
                                {
                                    //Found our matching cached feature
                                    if (!cachedFeature.RelatedObjects.ContainsKey(relClass)) { cachedFeature.RelatedObjects.Add(relClass, new List<IObject>()); }
                                    cachedFeature.RelatedObjects[relClass].Add(targetRow as IObject);
                                    break;
                                }
                            }
                            rowPairs.Next(out sourceRow, out targetRow);
                        }
                    }
                }
            }
            if (oldObject != null)
            {
                //Process relationships where this class is the destination
                FeatureClassInformation featClassInfo = GetFeatureClassInformation(oldObject.Class as IFeatureClass);

                //First check if the related objects have already been cached.  No need to look for them if so
                foreach (CachedFeature cachedFeature in CachedFeatures)
                {
                    if (cachedFeature.PoleFeature == oldObject)
                    {
                        relationshipMapping = cachedFeature.RelatedObjects;
                    }
                }

                if (relationshipMapping == null)
                {
                    //Relationship mapping wasn't cached so we need to do some work
                    relationshipMapping = new Dictionary<IRelationshipClass, List<IObject>>();

                    //IFeatureClass featClass = oldFeature.Class as IFeatureClass;
                    IObjectClass objClass = oldObject.Class as IObjectClass;

                    //FeatureClassInformation featInfo = featureClassInformation[
                    foreach (IRelationshipClass relClass in featClassInfo.DestinationRelationshipClasses)
                    {
                        if (!relationshipMapping.ContainsKey(relClass))
                        {
                            relationshipMapping.Add(relClass, new List<IObject>());
                        }
                        ISet relatedFeatures = relClass.GetObjectsRelatedToObject(oldObject);
                        relatedFeatures.Reset();
                        for (int i = 0; i < relatedFeatures.Count; i++)
                        {
                            IObject obj = relatedFeatures.Next() as IObject;
                            relationshipMapping[relClass].Add(obj);
                            relClass.DeleteRelationship(obj, oldObject);
                            //relClass.CreateRelationship(obj, newFeature);
                        }
                    }


                    //Process relationships where this class is the origin
                    foreach (IRelationshipClass relClass in featClassInfo.OriginRelationshipClasses)
                    {
                        //Don't map annotation relationships
                        if (relClass.DestinationClass is IFeatureClass)
                        {
                            IFeatureClass originClass = relClass.DestinationClass as IFeatureClass;

                            //Simon Change remove this (we want to include the anno) 
                            if (originClass.FeatureType == esriFeatureType.esriFTAnnotation)
                                System.Diagnostics.Debug.Print(originClass.AliasName);
                            //if (originClass.FeatureType == esriFeatureType.esriFTAnnotation)
                            //{
                            //    relClass = relClasses.Next();
                            //    continue;
                            //}
                        }
                        if (!relationshipMapping.ContainsKey(relClass))
                        {
                            relationshipMapping.Add(relClass, new List<IObject>());
                        }
                        ISet relatedFeatures = relClass.GetObjectsRelatedToObject(oldObject);
                        relatedFeatures.Reset();
                        for (int i = 0; i < relatedFeatures.Count; i++)
                        {
                            IObject obj = relatedFeatures.Next() as IObject;
                            relationshipMapping[relClass].Add(obj);
                            relClass.DeleteRelationship(obj, oldObject);
                            System.Diagnostics.Debug.Print("Deleting rel to OId: " + obj.OID.ToString());
                            //relClass.CreateRelationship(obj, newFeature);
                        }
                    }
                }
            }
            return relationshipMapping;
        }


        /// <summary>
        /// This method will create the new feature based off of the configured values to map
        /// from the original feature
        /// </summary>
        /// <param name="origFeature"></param>
        private IFeature CreateNewObject(Dictionary<int, object> fieldMapping, IFeatureClass objectClass, IFeature replacedObject) //Dictionary<int, object> fieldMapping, IFeatureClass featClass, IFeature replacedFeature
        {
            IFeature feat = (objectClass as IFeatureClass).CreateFeature();

            foreach (KeyValuePair<int, object> kvp in fieldMapping)
            {
                if (kvp.Value != null)
                    feat.set_Value(kvp.Key, kvp.Value);
            }

            feat.Shape = ((IFeature)replacedObject).ShapeCopy;

            int guidIdx = FindFieldIndex(objectClass as IFeatureClass, GlobalIDFieldName);
            int ReplaceGUIDIndex = FindFieldIndex(feat.Class as IFeatureClass, ReplaceGuidFieldName);
            feat.set_Value(ReplaceGUIDIndex, replacedObject.get_Value(guidIdx));

            return feat;
        }


        /// <summary>
        /// Returns a list of all the fields that are configured for the asset replacement field copy
        /// </summary>
        /// <param name="origOject"></param>
        /// <returns></returns>
        private Dictionary<int, object> GetValuesToMap(IObject origOject) //IFeature origFeature
        {
            Dictionary<int, object> valuesToMap = new Dictionary<int, object>();
            List<int> fieldMapping = ModelNameFacade.FieldIndicesFromModelName(origOject.Class, SchemaInfo.General.FieldModelNames.AssetCopyFMN);
            foreach (int fieldIndex in fieldMapping)
            {
                valuesToMap.Add(fieldIndex, origOject.get_Value(fieldIndex));
            }
            return valuesToMap;
        }

        /// <summary>
        /// Projects the latitude / longitude into the desired spatial reference
        /// </summary>
        /// <param name="latitude"></param>
        /// <param name="longitude"></param>
        /// <param name="spatialRef"></param>
        /// <returns></returns>
        private IPoint ProjectLatLongPoint(double latitude, double longitude, ISpatialReference spatialRef)
        {
            //Create Spatial Reference Factory
            ISpatialReferenceFactory srFactory = new SpatialReferenceEnvironmentClass();
            ISpatialReference sr1;
            //GCS to project from            
            IGeographicCoordinateSystem gcs = srFactory.CreateGeographicCoordinateSystem(4326);
            sr1 = gcs;
            //Projected Coordinate System to project into             
            ISpatialReference sr2 = spatialRef;
            //Point to project             
            IPoint point = new PointClass() as IPoint;
            point.SpatialReference = sr1;
            point.PutCoords(longitude, latitude);
            point.SpatialReference = sr1;
            point.Project(sr2);

            return point;
        }


        /// <summary>
        /// Determine the SAP domain mapped value.  If no domain is assigned, then it simply returns the value
        /// </summary>
        /// <param name="row"></param>
        /// <param name="fieldIndex"></param>
        /// <param name="SAPFieldValue"></param>
        /// <param name="ErrorIfNotMatched">If false, returns the SAPFieldValue if not matched in domain</param>
        /// <returns></returns>
        private object GetDomainValue(IRow row, int fieldIndex, string SAPFieldValue, bool ErrorIfNotMatched)
        {
            string mappedValue = SAPFieldValue;
            //Determine our mapped field value
            IField field = row.Fields.get_Field(fieldIndex);
            IRowSubtypes rowSubtypes = row as IRowSubtypes;
            ISubtypes subtypes = row.Table as ISubtypes;
            IDomain fieldDomain = subtypes.get_Domain(rowSubtypes.SubtypeCode, field.Name);
            if (fieldDomain != null)
            {
                string domainName = fieldDomain.Name;
                mappedValue = SAPToGISMappings.Instance.GetGISDomainValue(domainName, SAPFieldValue, ErrorIfNotMatched);
            }

            try
            {
                if (string.IsNullOrEmpty(mappedValue)) { return DBNull.Value; }
                else if (field.Type == esriFieldType.esriFieldTypeDouble) { return Double.Parse(mappedValue); }
                else if (field.Type == esriFieldType.esriFieldTypeInteger) { return Int32.Parse(mappedValue); }
                else if (field.Type == esriFieldType.esriFieldTypeSingle) { return Single.Parse(mappedValue); }
                else if (field.Type == esriFieldType.esriFieldTypeSmallInteger) { return Int16.Parse(mappedValue); }
                else if (field.Type == esriFieldType.esriFieldTypeString) { return mappedValue; }
                else if (field.Type == esriFieldType.esriFieldTypeDate) { return DateTime.Parse(mappedValue); }
            }
            catch (Exception ex)
            {
                //Only throw an error if during parsing the mapped value if ErrorIfNotMatched is set
                if (ErrorIfNotMatched) { throw ex; }
            }

            return mappedValue;
        }

        /// <summary>
        /// Logs a PTT Transaction error to our error logging file
        /// </summary>
        /// <param name="SAPEquipID"></param>
        /// <param name="guid"></param>
        /// <param name="transactionType"></param>
        /// <param name="projectID"></param>
        /// <param name="errorType"></param>
        /// <param name="messageDescription"></param>
        private void LogTransactionError(Pole pole, TransactionType transactionType)
        {
            try
            {
                DateTime now = DateTime.Now;
                string second = now.Second.ToString();
                string minute = now.Minute.ToString();
                string hour = now.Hour.ToString();
                string month = now.Month.ToString();
                string day = now.Day.ToString();
                if (day.Length < 2) { day = "0" + day; }
                if (month.Length < 2) { month = "0" + month; }
                if (hour.Length < 2) { hour = "0" + hour; }
                if (minute.Length < 2) { minute = "0" + minute; }
                if (second.Length < 2) { second = "0" + second; }
                string errorTypeString = ((int)pole.ErrorType).ToString();
                if (errorTypeString.Length < 2) { errorTypeString = "0" + errorTypeString; }


                ErrorLoggingWriter.WriteLine("\t<PTTGISStatus>");
                ErrorLoggingWriter.WriteLine("\t\t<SAPEquipID>" + pole.SAPEquipID + "</SAPEquipID>");
                ErrorLoggingWriter.WriteLine("\t\t<GUID>" + pole.Guid.ToString("B").ToUpper() + "</GUID>");
                ErrorLoggingWriter.WriteLine("\t\t<TransactionType>" + transactionType.ToString() + "</TransactionType>");
                ErrorLoggingWriter.WriteLine("\t\t<TransactionStatus>" + pole.Status.ToString() + "</TransactionStatus>");
                ErrorLoggingWriter.WriteLine("\t\t<ProjectID>" + pole.ProjectID + "</ProjectID>");
                //ErrorLoggingWriter.WriteLine("\t\t<TransactionStatus>" + "Failure" + "</TransactionStatus>");
                ErrorLoggingWriter.WriteLine("\t\t<MessageCode>" + errorTypeString + "</MessageCode>");
                ErrorLoggingWriter.WriteLine("\t\t<MessageDescription>" + pole.ErrorType.ToString() + "</MessageDescription>");
                ErrorLoggingWriter.WriteLine("\t\t<MessageDetail>" + pole.MessageDetail.ToString() + "</MessageDetail>");
                ErrorLoggingWriter.WriteLine("\t\t<DateTimeProcessed>" + now.Year + month + day + "_" + hour + ":" + minute + ":" + second + "</DateTimeProcessed>");
                ErrorLoggingWriter.WriteLine("\t\t<DeletedGhostPole>" + pole.DeletedGhostPole.ToString() + "</DeletedGhostPole>");
                ErrorLoggingWriter.WriteLine("\t\t<PTTOriginalTransaction>\r\n" + pole.OriginalTransaction + "\t\t</PTTOriginalTransaction>");
                ErrorLoggingWriter.WriteLine("\t</PTTGISStatus>");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #region Methods for EDGIS Rearch Project by v1t8
        /// <summary>
        /// Method to get oracle connection
        /// </summary>
        private void GetOracleConnection()
        {
            _log.Info("Starting" + MethodBase.GetCurrentMethod().Name);
            try
            {
                _log.Info( "Creating oracle connection");
                _oraConnection = new OracleConnection(Common.OracleConnectionString);
                //  _oraConnection = new OracleConnection("Data Source=EDGEM1D;User Id=EDGIS;password=EDG_ZRz97*gem1d;");
                _log.Info("Opening oracle connection");
                _oraConnection.Open();
                _log.Info("Completed Execution :" + MethodBase.GetCurrentMethod().Name);
            }
            catch( Exception ex)
            {
                _log.Error("Exception occur while making oracle connection:" + MethodBase.GetCurrentMethod().Name);
                throw ex;
            }
        }

        /// <summary>
        /// This Method is used to pupulate ED12 Staging table for EDGIS Rearch Project -v1t8
        /// </summary>
        /// <param name="insertsProcessed"></param>
        /// <param name="insertsFailed"></param>
        /// <param name="updatesProcess"></param>
        /// <param name="updatesFailed"></param>
        /// <param name="deletesProcessed"></param>
        /// <param name="deletesFailed"></param>
        /// <param name="replacesProcessed"></param>
        /// <param name="replacesFailed"></param>
        private void PopulateED12Staging(ref int insertsProcessed, ref int insertsFailed, ref int updatesProcess, ref int updatesFailed, ref int deletesProcessed,
           ref int deletesFailed, ref int replacesProcessed, ref int replacesFailed)
        {
            List<PTTTransaction> transactions = new List<PTTTransaction>();
            _log.Info("Starting" + MethodBase.GetCurrentMethod().Name);
            try
            {
                ITable PTTTransactionsTable = ((IFeatureWorkspace)EditVersion).OpenTable(ED11ProcessingTable);
                _log.Info("Opened table:" + ED11ProcessingTable);

                IQueryFilter qf = new QueryFilterClass();
                IQueryFilterDefinition queryDef = qf as IQueryFilterDefinition;
                queryDef.PostfixClause = "ORDER BY ORDERNUMBER";
                _log.Info("Query Filter ORDER BY ORDERNUMBER");

                ICursor rowCursor = PTTTransactionsTable.Search(qf, false);
                _log.Info("Search in table:" + ED11ProcessingTable);
                IRow row = null;
                while ((row = rowCursor.NextRow()) != null)
                {
                    //Build our PTT Transaction from the binary values in the database
                    string binaryPartOne = row.get_Value(1).ToString();
                    string binaryPartTwo = row.get_Value(2).ToString();
                    string binaryPartThree = row.get_Value(3).ToString();
                    string binary = binaryPartOne + binaryPartTwo + binaryPartThree;
                    PTTTransaction transaction = (PTTTransaction)DeserializeObject(binary);
                    transactions.Add(transaction);
                }

                _log.Info("Getting oracle Connection");
                //Getting oracle Connection object 
                GetOracleConnection();
                _log.Info("Successfully connected with oracle Connection");
                //Get dataset from database for ED12
                //DataSet ED12TableDataset = GetED12Dataset(_oraConnection, "PGE_ED12_OUTBOUND_STAGING");
                ED12TableDataset = GetED12Dataset(_oraConnection, Common.ED12StagingTableName);

                foreach (PTTTransaction transaction in transactions)
                {
                    _log.Info("Inserting data into ED12 staging table");
                    //To log transction in Ed12 staging table -v1t8
                    LogTransactionErrorInStaging(transaction.TransactionPole, transaction.Type);
                    _log.Info("sucessfully transction logged into ED12 staging table");
                }
                //Loading data into ED12 Staging table
                _log.Info("Loading data into ED12 Staging table ");
                BulkLoadData(_oraConnection, ED12TableDataset);
                _log.Info("Sucessfully loaded data into ED12 Staging table");
                _log.Info("Completed Execution :" + MethodBase.GetCurrentMethod().Name);
                //throw new Exception("My new 'exception'");
                row = null;
                ED12TableDataset = null;
                _oraConnection.Close();


                //Get our successfully and failed counts
                _log.Info("Get our successfully and failed counts");
                qf = new QueryFilterClass();
                qf.WhereClause = string.Format("TRANSACTIONTYPE = '{0}'", TransactionType.Insert);
                insertsProcessed = PTTTransactionsTable.RowCount(qf);
                qf.WhereClause = string.Format("TRANSACTIONSUCCESS = '{0}' AND TRANSACTIONTYPE = '{1}'", TransactionStatus.Failure, TransactionType.Insert);
                insertsFailed = PTTTransactionsTable.RowCount(qf); 

                qf.WhereClause = string.Format("TRANSACTIONTYPE = '{0}'", TransactionType.Update);
                updatesProcess = PTTTransactionsTable.RowCount(qf);
                qf.WhereClause = string.Format("TRANSACTIONSUCCESS = '{0}' AND TRANSACTIONTYPE = '{1}'", TransactionStatus.Failure, TransactionType.Update);
                updatesFailed = PTTTransactionsTable.RowCount(qf);

                qf.WhereClause = string.Format("TRANSACTIONTYPE = '{0}'", TransactionType.Delete);
                deletesProcessed = PTTTransactionsTable.RowCount(qf);
                qf.WhereClause = string.Format("TRANSACTIONSUCCESS = '{0}' AND TRANSACTIONTYPE = '{1}'", TransactionStatus.Failure, TransactionType.Delete);
                deletesFailed = PTTTransactionsTable.RowCount(qf);

                qf.WhereClause = string.Format("TRANSACTIONTYPE = '{0}'", TransactionType.Replace);
                replacesProcessed = PTTTransactionsTable.RowCount(qf);
                qf.WhereClause = string.Format("TRANSACTIONSUCCESS = '{0}' AND TRANSACTIONTYPE = '{1}'", TransactionStatus.Failure, TransactionType.Replace);
                replacesFailed = PTTTransactionsTable.RowCount(qf);
                _log.Info("Completed Execution :" + MethodBase.GetCurrentMethod().Name);
            }
            catch(Exception ex)
            {
                _log.Error("Exception occur while making Poupulate ED12 Staging Table :" + MethodBase.GetCurrentMethod().Name);
                throw ex;
            }
        }
        /// <summary>
        /// Method is to log transctional error for ED 12 staging table -Added for EDGIS Rearch Project 2021 -v1t8
        /// </summary>
        /// <param name="pole"></param>
        /// <param name="transactionType"></param>
        private void LogTransactionErrorInStaging(Pole pole, TransactionType transactionType)
        {
            string recordId = string.Empty;
           // DataSet ED12TableDataset = null;
            _log.Info("Starting" + MethodBase.GetCurrentMethod().Name);
            try
            {
                DateTime now = DateTime.Now;
                string second = now.Second.ToString();
                string minute = now.Minute.ToString();
                string hour = now.Hour.ToString();
                string month = now.Month.ToString();
                string day = now.Day.ToString();
                if (day.Length < 2) { day = "0" + day; }
                if (month.Length < 2) { month = "0" + month; }
                if (hour.Length < 2) { hour = "0" + hour; }
                if (minute.Length < 2) { minute = "0" + minute; }
                if (second.Length < 2) { second = "0" + second; }
                string errorTypeString = ((int)pole.ErrorType).ToString();
                if (errorTypeString.Length < 2) { errorTypeString = "0" + errorTypeString; }

                //_log.Info("Getting oracle Connection");
                ////Getting oracle Connection object 
                ////GetOracleConnection();
                //_log.Info("Successfully connected with oracle Connection");
                ////Get dataset from database for ED12
                ////DataSet ED12TableDataset = GetED12Dataset(_oraConnection, "PGE_ED12_OUTBOUND_STAGING");
                //ED12TableDataset = GetED12Dataset(_oraConnection, Common.ED12StagingTableName);

                if (ED12TableDataset.Tables.Count < 1) return;
                _log.Info("Processing  start to fill data in ED12 Staging table ");
                //Create a new datarow  for ED12 Staging table 
                DataRow row = ED12TableDataset.Tables[Common.ED12StagingTable].NewRow();
                //Get the record ID
                recordId=GetRecordID();
                // Set SAPEquipID value in field
                row[Common.SAPEquipID] = pole.SAPEquipID;
                // Set GUID value in field
                row[Common.GUID] = pole.Guid;
                // Set TRANSACTIONTYPE value in field
                row[Common.TRANSACTIONTYPE] = transactionType;
                // Set TransactionStatus value in field
                row[Common.TransactionStatus] = pole.Status;
                // Set MessageCode value in field
                row[Common.MessageCode] = errorTypeString;
                // Set MesgDescription value in field
                row[Common.MesgDescription] = pole.ErrorType.ToString();
                // Set GUID value in field
                row[Common.messageDetails] = pole.MessageDetail.ToString();
                // Set dateTimeProcessed value in field
                row[Common.dateTimeProcessed] = now.Year + month + day + "_" + hour + ":" + minute + ":" + second;
                // Set deletedGhostPole value in field
                row[Common.deletedGhostPole] = pole.DeletedGhostPole;
                // Set ProjectId value in field
                row[Common.ProjectID] = pole.ProjectID;
                // Set RecordID value in field
                row[Common.RecordID] = recordId;
                // Set ProcessedFlag value in field
                row[Common.ProcessedFlag] = ProcessFlag.GISProcessed;
                // Set CreationDate  value in field
                row[Common.CreationDate] = DateTime.Now;

                #region  Commented Code for transction type as SAP does not required in payload
                //if (transactionType == TransactionType.Insert)
                //{
                //    _log.Info("Processing Insert Transaction for ED12 ");
                //    //Pole class object for insert transction
                //    PoleInsert insert = pole as PoleInsert;

                //    _log.Info("Processing SAP Mapped field value pair for Insert Transction ");
                //    // SAP Mapped field value pair for Insert Transction
                //    Dictionary<string, string> keyValue = insert.FieldValues;
                //    _log.Info("Looping SAP Mapped field value pair ");
                //    foreach (var kv in keyValue)
                //    {

                //        string FieldName = kv.Key;

                //        _log.Info("SAP Mapped field Name :" + FieldName + "Value :" + kv.Value);
                //        if (kv.Key.ToUpper() == "PoleClass".ToUpper())
                //        {
                //            //row[Common.PoleClass] = kv.Value;
                //        }
                //        if (kv.Key.ToUpper() == "Material".ToUpper())
                //        {
                //            row[Common.Material_I] = kv.Value;
                //        }
                //        if (kv.Key.ToUpper() == "Height".ToUpper())
                //        {
                //            row[Common.Height_I] = kv.Value;
                //        }
                //        if (kv.Key.ToUpper() == "County".ToUpper())
                //        {
                //            // row[Common.County_I] = kv.Value;
                //        }
                //        if (kv.Key.ToUpper() == "InstallationDate".ToUpper())
                //        {
                //            // row[Common.InstallationDate_I] = kv.Value;
                //        }
                //        if (kv.Key.ToUpper() == "LocalOfficeID".ToUpper())
                //        {
                //            //  row[Common.LocalOfficeID_I] = kv.Value;
                //        }
                //        if (kv.Key.ToUpper() == "Division".ToUpper())
                //        {
                //            row[Common.Division_I] = kv.Value;
                //        }
                //        if (kv.Key.ToUpper() == "District".ToUpper())
                //        {
                //            row[Common.District_I] = kv.Value;
                //        }
                //        if (kv.Key.ToUpper() == "LocDesc2".ToUpper())
                //        {
                //            row[Common.LocDesc2_I] = kv.Value;
                //        }

                //        if (kv.Key.ToUpper() == "City".ToUpper())
                //        {
                //            row[Common.City_I] = kv.Value;
                //        }
                //        if (kv.Key.ToUpper() == "Zip".ToUpper())
                //        {
                //            row[Common.Zip_I] = kv.Value;
                //        }
                //        if (kv.Key.ToUpper() == "Barcode".ToUpper())
                //        {
                //            row[Common.Material_I] = kv.Value;
                //        }
                //        if (kv.Key.ToUpper() == "GemsMapOffice".ToUpper())
                //        {
                //            row[Common.GemsMapOffice_I] = kv.Value;
                //        }
                //        if (kv.Key.ToUpper() == "JPNumber".ToUpper())
                //        {
                //            row[Common.JPNumber_I] = kv.Value;
                //        }
                //        if (kv.Key.ToUpper() == "DistMap".ToUpper())
                //        {
                //            row[Common.DistMap_I] = kv.Value;
                //        }
                //        if (kv.Key.ToUpper() == "PlantSection".ToUpper())
                //        {
                //            row[Common.PlantSection_I] = kv.Value;
                //        }
                //        if (kv.Key.ToUpper() == "StartupDate".ToUpper())
                //        {
                //            row[Common.StartupDate_I] = kv.Value;
                //        }
                //    }
                //    row[Common.EquipmentNumber_I] = insert.SAPEquipID;
                //    //row[Common.GUID_I] = insert.Guid;                                    

                //    row[Common.Latitude_I] = insert.Latitude;
                //    row[Common.Longitude_I] = insert.Longitude;
                //    row[Common.ProjectID_I] = insert.ProjectID;

                //    _log.Info("Field value assignment in new row is completed for Insert Transction");
                //}
                //if (transactionType == TransactionType.Delete)
                //{
                //    _log.Info("Processing Delete Transaction for ED12 staging");
                //    PoleDelete delete = pole as PoleDelete;

                //    // Set SAPEquipID value in field
                //    row[Common.SAPEquipID] = delete.SAPEquipID;
                //    // Set ProjectId value in field
                //    row[Common.ProjectID_D] = delete.ProjectID;
                //    // Set GUID value in field
                //    row[Common.GUID_D] = delete.Guid;
                //    string deletedGUID = delete.Guid.ToString("B").ToUpper();

                //    _log.Info("Field value assignment in new row is completed for Insert Transction");
                //}

                //if (transactionType == TransactionType.Replace)
                //{
                //    PoleReplace replace = pole as PoleReplace;
                //    string deletedGhost = replace.DeletedGhostPole;

                //    _log.Info("Processing SAP Mapped field value pair for Replace Transction ");
                //    _log.Info("Processing SAP Mapped field value pair for Replace -Insert Transction ");
                //    // SAP Mapped field value pair for Insert Transction
                //    Dictionary<string, string> keyValue = replace.PoleToInsert.FieldValues;
                //    _log.Info("Looping SAP Mapped field value pair ");
                //    foreach (var kv in keyValue)
                //    {

                //        string FieldName = kv.Key;

                //        _log.Info("SAP Mapped field Name :" + FieldName + "Value :" + kv.Value);
                //        if (kv.Key.ToUpper() == "PoleClass".ToUpper())
                //        {
                //            //row[Common.PoleClass] = kv.Value;
                //        }
                //        if (kv.Key.ToUpper() == "Material".ToUpper())
                //        {
                //            row[Common.Material_I] = kv.Value;
                //        }
                //        if (kv.Key.ToUpper() == "Height".ToUpper())
                //        {
                //            row[Common.Height_I] = kv.Value;
                //        }
                //        if (kv.Key.ToUpper() == "County".ToUpper())
                //        {
                //            // row[Common.County_I] = kv.Value;
                //        }
                //        if (kv.Key.ToUpper() == "InstallationDate".ToUpper())
                //        {
                //            // row[Common.InstallationDate_I] = kv.Value;
                //        }
                //        if (kv.Key.ToUpper() == "LocalOfficeID".ToUpper())
                //        {
                //            // row[Common.LocalOfficeID_I] = kv.Value;
                //        }
                //        if (kv.Key.ToUpper() == "Division".ToUpper())
                //        {
                //            row[Common.Division_I] = kv.Value;
                //        }
                //        if (kv.Key.ToUpper() == "District".ToUpper())
                //        {
                //            row[Common.District_I] = kv.Value;
                //        }
                //        if (kv.Key.ToUpper() == "LocDesc2".ToUpper())
                //        {
                //            row[Common.LocDesc2_I] = kv.Value;
                //        }

                //        if (kv.Key.ToUpper() == "City".ToUpper())
                //        {
                //            row[Common.City_I] = kv.Value;
                //        }
                //        if (kv.Key.ToUpper() == "Zip".ToUpper())
                //        {
                //            row[Common.Zip_I] = kv.Value;
                //        }
                //        if (kv.Key.ToUpper() == "Barcode".ToUpper())
                //        {
                //            row[Common.Material_I] = kv.Value;
                //        }
                //        if (kv.Key.ToUpper() == "GemsMapOffice".ToUpper())
                //        {
                //            row[Common.GemsMapOffice_I] = kv.Value;
                //        }
                //        if (kv.Key.ToUpper() == "JPNumber".ToUpper())
                //        {
                //            row[Common.JPNumber_I] = kv.Value;
                //        }
                //        if (kv.Key.ToUpper() == "DistMap".ToUpper())
                //        {
                //            row[Common.DistMap_I] = kv.Value;
                //        }
                //        //Need to check below 2 fields 
                //        if (kv.Key.ToUpper() == "PlantSection".ToUpper())
                //        {
                //            row[Common.PlantSection_I] = kv.Value;
                //        }
                //        if (kv.Key.ToUpper() == "StartupDate".ToUpper())
                //        {
                //            row[Common.StartupDate_I] = kv.Value;
                //        }
                //    }
                //    row[Common.EquipmentNumber_I] = replace.PoleToInsert.SAPEquipID;
                //    //row[Common.GUID_I] = insert.Guid;                                    

                //    row[Common.Latitude_I] = replace.PoleToInsert.Latitude;
                //    row[Common.Longitude_I] = replace.PoleToInsert.Longitude;
                //    row[Common.ProjectID_I] = replace.PoleToInsert.ProjectID;

                //    _log.Info("Field value assignment in new row is completed for replace-Insert Transction");
                //    //     row[Common.GUID_I] = replace.PoleToInsert.Guid;


                //    _log.Info("Processing Replace-Delete Transaction for ED12 staging");
                //    PoleDelete delete = pole as PoleDelete;

                //    // Set SAPEquipID value in field
                //    row[Common.SAPEquipID] = replace.PoleToDelete.SAPEquipID;
                //    // Set ProjectId value in field
                //    row[Common.ProjectID_D] = replace.PoleToDelete.ProjectID;
                //    // Set GUID value in field
                //    row[Common.GUID_D] = replace.PoleToDelete.Guid;
                //    string deletedGUID = delete.Guid.ToString("B").ToUpper();

                //    _log.Info("Field value assignment in new row is completed for Insert Transction");
                //    //row[Common.ProjectID_D] = replace.PoleToDelete.ProjectID;
                //    //row[Common.SAPEquipID] = replace.PoleToDelete.SAPEquipID;
                //    //row[Common.GUID_D] = replace.PoleToInsert.Guid.ToString("B").ToUpper(); ;
                //    //string deletedSapEquipId = replace.PoleToDelete.SAPEquipID;
                //    //string deletedProjectId = replace.PoleToDelete.ProjectID;
                //    //string deletedGUID = replace.PoleToDelete.Guid.ToString("B").ToUpper();
                //}
                //if (transactionType == TransactionType.Update)
                //{
                //    _log.Info("Processing Update Transaction for ED12 staging");
                //    PoleUpdate update = pole as PoleUpdate;
                //    // Set Attribute Name  in field
                //    row[Common.AttributeName_U] = update.FieldName;
                //    // Setold field Value in field
                //    row[Common.OldValue_U] = update.OldFieldValue;
                //    // Set new field value in field
                //    row[Common.NewValue_U] = update.NewFieldValue;
                //    // Set ProjectId value in field
                //    row[Common.ProjectID] = update.ProjectID;
                //    // Set SAPEquipID value in field
                //    row[Common.SAPEquipID] = update.SAPEquipID;

                //    _log.Info("Field value assignment in new row is completed for Update Transction");

                //}
                #endregion

                //Adding new row in Ed 12 staging table 
                ED12TableDataset.Tables[Common.ED12StagingTable].Rows.Add(row);
                _log.Info("Added new row in Ed 12 staging table ");

                ////Loading data into ED12 Staging table
                //_log.Info("Loading data into ED12 Staging table ");
                //BulkLoadData(_oraConnection, ED12TableDataset);
                //_log.Info("Sucessfully loaded data into ED12 Staging table");
                //_log.Info("Completed Execution :" + MethodBase.GetCurrentMethod().Name);
                ////throw new Exception("My new 'exception'");
                // row = null;
                // ED12TableDataset = null;
                //_oraConnection.Close();

            }
            catch (Exception ex)
            {
                _log.Info("Exception in Populating Ed12 Staging table:" + ex.StackTrace);
                //   LogError(Common.ED12StagingTable, ex.Message, recordId, pole.Guid.ToString());
                LogError(ED12TableDataset, ex.Message, recordId, pole.Guid.ToString());
                throw new Exception("Exception in Populating Ed12 Staging table:" + ex.StackTrace);
            }
            
        }

        /// <summary>
        /// This Method is used to get Dataset with ED12 Staging table 
        /// </summary>
        /// <param name="connection">OracleConnection</param>
        /// <param name="tableName">string</param>
        /// <returns>DataSet</returns>
        private static DataSet GetED12Dataset(OracleConnection connection, string tableName)
        {
            try
            {
                _log.Info("Starting" + MethodBase.GetCurrentMethod().Name);
                DataTable oleschema = connection.GetSchema("Tables");//This returns a table with a list of all the tables in the connected schema
                System.Data.Common.DbCommand GetTableCmd = connection.CreateCommand();
                System.Data.Common.DbDataAdapter ODA = null;

                ODA = new OracleDataAdapter() as System.Data.Common.DbDataAdapter;

                ODA.SelectCommand = GetTableCmd;
                DataSet set = new DataSet();
                foreach (DataRow row in oleschema.Rows)
                {
                    if (row["TABLE_NAME"].Equals(tableName))
                    {
                        DataTable DBTable = new DataTable();
                        _log.Info("");
                        GetTableCmd.CommandText = "SELECT * FROM " + row["OWNER"] + "." + row["TABLE_NAME"]; ;
                        ODA.FillSchema(DBTable, SchemaType.Source);//This pulls down the schema for the given table
                        DBTable.TableName = row["OWNER"] + "." + DBTable.TableName;
                        set.Tables.Add(DBTable);
                        DBTable = null;
                    }
                }
                _log.Info("Completed Execution :" + MethodBase.GetCurrentMethod().Name);
                return set;
            }
            catch (Exception ex)
            {
                _log.Info("Exception in Execution :" + MethodBase.GetCurrentMethod().Name);
                throw ex;
            }
            finally
            {
            }
            
        }

        /// <summary>
        /// This method is to get record ID 
        /// </summary>
        /// <returns></returns>
        private static string GetRecordID()
        {
            //Below is the foramt require for recordID ED06-374 integration improvement -v1t8
            //ED.0.12.20201228.110401.1234567 for ED07
            _log.Info("Starting" + MethodBase.GetCurrentMethod().Name);
            string recordID = string.Empty; string type = string.Empty;
            recordID = "ED" + "." + 0 + "." + "12" + "." + DateTime.Now.ToString("yyyyMMdd") + "." + DateTime.Now.ToString("hhmmss") + ".";
            recordID = recordID + GetNextSeq();
            _log.Info("Completed Execution :" + MethodBase.GetCurrentMethod().Name);
            return recordID;

        }
        /// <summary>
        /// This method is used to get next sequence for record id genration 
        /// </summary>
        /// <returns></returns>
        public static string GetNextSeq()
        {
            string nextVar = string.Empty;
            try
            {
                _log.Info("Starting" + MethodBase.GetCurrentMethod().Name);
                OracleCommand loCmd = _oraConnection.CreateCommand();
                loCmd.CommandType = CommandType.Text;
                loCmd.CommandText = "select LPAD(" + (Common.RecordSeq) + ".NEXTVAL,7,0) FROM dual";
                nextVar = loCmd.ExecuteScalar().ToString();
                loCmd.Dispose();
                _log.Info("Completed Execution :" + MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                _log.Error("Failed to GetNextSeq from database for recordId genration" + ex.Message);
                throw ex;
            }
            return nextVar;
        }
    

        /// <summary>
        /// This Method is used to log exception in ED12 Staging table 
        /// </summary>
        /// <param name="dataSet"></param>
        /// <param name="error"></param>
        /// <param name="recordID"></param>
        /// <param name="guid"></param>
        public static void LogError(DataSet dataSet , string error, string recordID, string guid)
        {
            try
            {
                if (dataSet.Tables.Count < 1) return;
                if(error.Contains("'"))
                {
                    error = error.Replace("'", "");
                }
                _log.Info("Starting" + MethodBase.GetCurrentMethod().Name);
                _log.Info("Error description is getting populated");
                //Create a new datarow  for ED12 Staging table 
                DataRow row = dataSet.Tables[Common.ED12StagingTable].NewRow();

                // Set GUID value in field
                row[Common.GUID] = guid;                
                row[Common.RecordID] = recordID;
                row[Common.ProcessedFlag] = ProcessFlag.GISError;
                row[Common.CreationDate] = DateTime.Now;
                row[Common.ErrorDescription] = error;
                DateTime DATEPROCESSED = DateTime.Now;
                //Adding new row in Ed 12 staging table 
                dataSet.Tables[Common.ED12StagingTable].Rows.Add(row);
                _log.Info("Added error row in ED12 staging table dataset ");

                //Loading data into ED12 Staging table
                _log.Info("Loading Error data into ED12 Staging table ");
                BulkLoadData(_oraConnection, dataSet);
                dataSet = null;
                _log.Info("Sucessfully loaded Error data into ED12 Staging table");
                _log.Info("Completed Execution :" + MethodBase.GetCurrentMethod().Name);

            }
            catch (Exception ex)
            {               
              _log.Error ("Exception while logging the exception in ED12staging table " +ex.Message + "   " + ex.StackTrace);                
               throw ex;
            }
            
        }
        #endregion 

        /// <summary>
        /// Logs a PTT Transaction error to our error logging file
        /// </summary>
        /// <param name="SAPEquipID"></param>
        /// <param name="guid"></param>
        /// <param name="transactionType"></param>
        /// <param name="projectID"></param>
        /// <param name="errorType"></param>
        /// <param name="messageDescription"></param>
        private void LogTransactionError(PTTTransaction transaction,
            TransactionErrorType errorType, TransactionStatus status, string messageDetail, string deletedGhostPole)
        {
            string processedFlag = default;
            try
            {
                if (string.IsNullOrEmpty(messageDetail))
                {
                    //Debugger.Launch();
                }
                transaction.TransactionPole.ErrorType = errorType;
                transaction.TransactionPole.Status = status;
                transaction.TransactionPole.MessageDetail = messageDetail;
                transaction.TransactionPole.DeletedGhostPole = deletedGhostPole;


                //Update our transactions table
                string binaryString = SerializeObject(transaction);
                int splitLength = binaryString.Length / 3;
                string binaryPartOne = binaryString.Substring(0, splitLength);
                string binaryPartTwo = binaryString.Substring(splitLength, splitLength);
                string binaryPartThree = binaryString.Substring(splitLength + splitLength);
                string processedDt = DateTime.Now.ToString(); 
                
                //OrderNumber, PTTTransaction, Process ID, Results (populated by child processes
                string sql = string.Format("UPDATE {0} SET PTTTRANSACTIONPART1 = '{1}', PTTTRANSACTIONPART2 = '{2}', PTTTRANSACTIONPART3 = '{3}', TRANSACTIONSUCCESS = '{4}', " +
                    "TRANSACTIONSTATUS = '{5}', TRANSACTIONMESSAGE = '{6}' WHERE ORDERNUMBER = {7}", ED11ProcessingTable,
                    binaryPartOne.Replace("'", "''"), binaryPartTwo.Replace("'", "''"), binaryPartThree.Replace("'", "''"), status, errorType, messageDetail, transaction.OrderNumber);


                // M4JF EDGISREARCH 415 ED11 IMPROVEMENTS 
                // UPDATE ED 11 STAGING TABLE ERROR DESCRIPTION FOR FAILURE RECORDS
                string updateErrQuery;
                if (transaction.TransactionPole.Status.ToString() == "Success")
                {
                    processedFlag = "D";
                    updateErrQuery = string.Format("Update {0} set  PROCESSEDFLAG = '{1}'  , PROCESSEDTIME = SYSDATE where RECORDID = '{2}'", ED11StagingTable, processedFlag, transaction.TransactionPole.RecordID);

                }
                else 
                {
                    processedFlag = "E";
                    updateErrQuery = string.Format("Update {0} set ERRORDESCRIPTION = '{1}' , PROCESSEDFLAG = '{2}'  , PROCESSEDTIME = SYSDATE where RECORDID = '{3}'", ED11StagingTable, messageDetail, processedFlag, transaction.TransactionPole.RecordID);

                }
                 //updateErrQuery = string.Format("Update {0} set ERRORDESCRIPTION = '{1}' , PROCESSEDFLAG = '{2}'  , PROCESSEDTIME = SYSDATE where RECORDID = '{3}'", ED11StagingTable , messageDetail, processedFlag , transaction.TransactionPole.RecordID );
                ((IWorkspace)EditVersion).ExecuteSQL(sql);
                ((IWorkspace)EditVersion).ExecuteSQL(updateErrQuery);
                ((IWorkspace)EditVersion).ExecuteSQL("commit");
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to log transaction: " + transaction.TransactionPole.SAPEquipID + ": " + ex.Message);
            }
        }

        #endregion

        #region XML Processing

        private List<PTTTransaction> ProcessXml()
        {

            string sqlQuery = default;
            DataTable dt = new DataTable();
            int orderNumber = 1;
            List<PTTTransaction> transactions = new List<PTTTransaction>();
            string OracleConnectionString = default;
            XmlDocument doc = new XmlDocument();
            // M4JF edgisrearch 415 - Data will be loaded from staging table directly .
            // foreach (string InputFile in InputFiles)
            
                // M4JF EDGISREARCH - 415 Below code is updated to get recrds from ed 11 staging table
                // Query to get records from staging table for processing
                sqlQuery = "Select RECORDID,TRANSACTIONTYPE,EQUIPMENTNUMBER_I,GUID_I,POLECLASS,POLETYPE, POLECNTYNAME,HEIGHT,STARTUPDATE_I,LATITUDE_I,LONGITUDE_I,PROJECTID_I,DIVISION,DISTRICT,LOCDESC2,CITY,ZIP,BARCODE,MAPNAME,MAPOFFICE,STARTUPDATE_I,EQUIPMENTNUMBER_D,GUID_D,PROJECTID_D,STARTUPDATE_D,EQUIPMENTNUMBER_U,GUID_U,ATTRIBUTENAME_U,OLDVALUE_U,NEWVALUE_U,PROJECTID_U from pgedata.pge_ed11_staging where processedflag='T' and errordescription is null";
               
                OracleConnectionString = Common.OracleConnectionString;                
                OracleConnection conn = new OracleConnection(OracleConnectionString);
                //Open the connection to the database
                conn.Open();
                OracleCommand cmd = new OracleCommand();
                cmd.Connection = conn;
                cmd.XmlCommandType = OracleXmlCommandType.Query;
                cmd.CommandText = sqlQuery;
                XmlReader reader = cmd.ExecuteXmlReader();
                // Load records to xml object .
                while (reader.Read())
                {
                    doc.Load(reader);
                }
                

                for (int i = 0; i < doc.DocumentElement.ChildNodes.Count; i++)
                {
                    XmlNode childNode1 = doc.DocumentElement.ChildNodes.Item(i);
                // m4jf edgisrearch - 415 Updated below code to read records from staging table .
                   // for (int j = 0; j < childNode1.ChildNodes.Count; j++)
                   // {
                            //XmlNode childNode = childNode1.ChildNodes.Item(i);
                        
                            XmlNode innerChildNode = childNode1.ChildNodes.Item(1);
                            PTTTransaction newTransaction = null;
                           // if (innerChildNode.Name == "PTTTransactionNewPole")
                            if (innerChildNode.InnerText == "N")
                            {
                                // m4jf edgisrearch 415 ed 11 improvements
                                // newTransaction = ProcessInsertElement(childNode);
                                newTransaction = ProcessInsertElement(childNode1);
                            }
                            //else if (innerChildNode.Name == "PTTTransactionDeletePole")
                            else if (innerChildNode.InnerText == "D")
                            {
                                // m4jf edgisrearch 415 ed 11 improvements
                                newTransaction = ProcessDeleteElement(childNode1);
                                //newTransaction = ProcessDeleteElement(innerChildNode);
                            }
                            // else if (innerChildNode.Name == "PTTTransactionUpdatePole")
                            else if (innerChildNode.InnerText == "U")
                            {
                                // m4jf edgisrearch 415 ed 11 improvements
                                // newTransaction = ProcessUpdateElement(childNode);
                                 newTransaction = ProcessUpdateElement(childNode1);
                            }

                    // m4jf edgisrearch 415
                    // else if (innerChildNode.Name == "PTTTransactionReplacePole")
                    
                            else if (innerChildNode.InnerText == "R")
                            {
                        // m4jf edgisrearch 415 ed 11 improvements - updating two Transactions for replace (Insert and Update)
                        // newTransaction = ProcessReplaceElement(childNode1);
                        
                                for(int k = 0; k < 2; k++)
                                {
                                    if(k == 0)
                                    {
                                         newTransaction = ProcessInsertElement(childNode1);
                                    }
                                    else
                                    {

                                        newTransaction = ProcessDeleteElement(childNode1);

                                    }
                                }
                               
                            }
                            if (newTransaction != null)
                            {
                                newTransaction.OrderNumber = orderNumber;
                                transactions.Add(newTransaction);
                                orderNumber++;
                            }
                      
                  //  }
                }

                //Clear resources for XML documents to ensure we don't run out of memory loading lots of data
                doc = null;
                GC.Collect();
                GC.WaitForPendingFinalizers();
            

            double currentlyProcessed = 0.0;
            double totalToProcess = transactions.Count;

            Console.WriteLine(string.Format("Truncating table {0}", ED11ProcessingTable));
            //((IWorkspace)EditVersion).ExecuteSQL(string.Format("TRUNCATE TABLE {0}", ED11ProcessingTable));
            //((IWorkspace)EditVersion).ExecuteSQL(string.Format("TRUNCATE TABLE {0}", "EDGIS.PGE_ED11_PROCESSES"));

            //M4JF EDGISREARCH 415 - updated truncate to delete statement as edgis table could not be truncated with new ED11 Interface user 
            ((IWorkspace)EditVersion).ExecuteSQL(string.Format("DELETE FROM {0}", ED11ProcessingTable));
            ((IWorkspace)EditVersion).ExecuteSQL(string.Format("DELETE FROM {0}", "EDGIS.PGE_ED11_PROCESSES"));
            ((IWorkspace)EditVersion).ExecuteSQL("COMMIT");

            Dictionary<string, List<string>> SAPEquipIDsDependency = new Dictionary<string, List<string>>();
            List<string> SAPEquipIDsWithDependencies = new List<string>();
            Console.WriteLine(string.Format("Populating the {0} table for processing", ED11ProcessingTable));
            XmlSerializer mySerializer = new XmlSerializer(typeof(PTTTransaction));
            // To write to a file, create a StreamWriter object.  
            foreach (PTTTransaction transaction in transactions)
            {
                try
                {
                    string sapEquipID1 = transaction.TransactionPole.SAPEquipID;
                    string sapEquipID2 = "";

                    if (transaction.TransactionPole is PoleReplace)
                    {
                        PoleReplace replacePole = transaction.TransactionPole as PoleReplace;
                        sapEquipID1 = replacePole.PoleToDelete.SAPEquipID;
                        sapEquipID2 = replacePole.PoleToInsert.SAPEquipID;
                    }

                    if (SAPEquipIDsDependency.ContainsKey(sapEquipID1))
                    {
                        if (!string.IsNullOrEmpty(sapEquipID2))
                        {
                            SAPEquipIDsWithDependencies.Add(sapEquipID2);
                            SAPEquipIDsDependency[sapEquipID1].Add(sapEquipID2);
                        }
                    }
                    else if (SAPEquipIDsDependency.ContainsKey(sapEquipID2) && !string.IsNullOrEmpty(sapEquipID2))
                    {
                        SAPEquipIDsWithDependencies.Add(sapEquipID1);
                        SAPEquipIDsDependency[sapEquipID2].Add(sapEquipID1);
                    }
                    else
                    {
                        //Verify that one doesn't exist in the list of another SAPEquipID list
                        string dependentSAPID = "";

                        if (SAPEquipIDsWithDependencies.Contains(sapEquipID1) || (!string.IsNullOrEmpty(sapEquipID2) && SAPEquipIDsWithDependencies.Contains(sapEquipID2)))
                        {
                            foreach (KeyValuePair<string, List<string>> kvp in SAPEquipIDsDependency)
                            {
                                if (kvp.Value.Contains(sapEquipID1) || (!string.IsNullOrEmpty(sapEquipID2) && kvp.Value.Contains(sapEquipID2)))
                                {
                                    //This is dependent on this sap equipment id
                                    dependentSAPID = kvp.Key;
                                    break;
                                }
                            }
                        }

                        if (!string.IsNullOrEmpty(dependentSAPID))
                        {
                            SAPEquipIDsDependency[dependentSAPID].Add(sapEquipID1);
                            SAPEquipIDsWithDependencies.Add(sapEquipID1);
                            if (!string.IsNullOrEmpty(sapEquipID2))
                            {
                                SAPEquipIDsDependency[dependentSAPID].Add(sapEquipID2);
                                SAPEquipIDsWithDependencies.Add(sapEquipID2);
                            }
                        }
                        else
                        {
                            SAPEquipIDsDependency.Add(sapEquipID1, new List<string>());
                            if (!string.IsNullOrEmpty(sapEquipID2))
                            {
                                SAPEquipIDsWithDependencies.Add(sapEquipID2);
                                SAPEquipIDsDependency[sapEquipID1].Add(sapEquipID2);
                            }
                        }
                    }

                    currentlyProcessed++;
                    if ((currentlyProcessed % 100) == 0) { ShowPercentProgress(string.Format("Determine SAPEquipID Dependencies: {0} of {1} processed", currentlyProcessed, totalToProcess), currentlyProcessed, totalToProcess); }

                }
                catch (Exception ex)
                {
                    Program.comment = "Failed to process XML: " + ex.Message;
                    throw new Exception("Failed to process XML: " + ex.Message);
                }
            }

            ShowPercentProgress(string.Format("Determine SAPEquipID Dependencies: {0} of {1} processed", currentlyProcessed, totalToProcess), currentlyProcessed, totalToProcess);
            Console.WriteLine("");

            currentlyProcessed = 0;
            totalToProcess = 0;
            int nextID = 0;
            Dictionary<int, List<string>> SAPEquipIDAssignments = new Dictionary<int, List<string>>();
            for (int i = 0; i < NumberOfChildProcesses; i++) { SAPEquipIDAssignments.Add(i, new List<string>()); }
            foreach (KeyValuePair<string, List<string>> kvp in SAPEquipIDsDependency)
            {
                if (nextID >= NumberOfChildProcesses) { nextID = 0; }
                SAPEquipIDAssignments[nextID].Add(kvp.Key);
                SAPEquipIDAssignments[nextID].AddRange(kvp.Value.Distinct().ToList());
                totalToProcess += kvp.Value.Distinct().ToList().Count + 1;
                nextID++;
            }

           // Console.WriteLine("Connecting to database: " + Common.OracleConnectionString);
            OracleConnection oracleConnection = new OracleConnection(Common.OracleConnectionString);
            //   OracleConnection oracleConnection = new OracleConnection("Data Source = EDGEM1D; User Id = EDGIS; password = EDG_ZRz97*gem1d");
            oracleConnection.Open();

            currentlyProcessed = 0;
            ShowPercentProgress(string.Format("Preparing EDGIS.PGE_ED11_PROCESSES table: {0} of {1} processed", currentlyProcessed, totalToProcess), currentlyProcessed, totalToProcess);
            DataSet ED11TableDataset = GetDataset(oracleConnection, "PGE_ED11_PROCESSING", "PGE_ED11_PROCESSES");
            //ED11TableDataset.Tables["EDGIS.PGE_ED11_PROCESSES"].BeginLoadData();
            foreach (KeyValuePair<int, List<string>> kvp in SAPEquipIDAssignments)
            {
                foreach (string sapEquipID in kvp.Value)
                {
                    DataRow newRow = ED11TableDataset.Tables["EDGIS.PGE_ED11_PROCESSES"].NewRow();
                    newRow["SAPEQUIPID"] = sapEquipID;
                    newRow["PROCESSID"] = kvp.Key;
                    ED11TableDataset.Tables["EDGIS.PGE_ED11_PROCESSES"].Rows.Add(newRow);

                    currentlyProcessed++;
                    if ((currentlyProcessed % 100) == 0) { ShowPercentProgress(string.Format("Preparing EDGIS.PGE_ED11_PROCESSES table: {0} of {1} processed", currentlyProcessed, totalToProcess), currentlyProcessed, totalToProcess); }
                    if ((currentlyProcessed % 10000) == 0)
                    {
                        //Bulk load data every 10,000 records
                        //ED11TableDataset.Tables["EDGIS.PGE_ED11_PROCESSES"].EndLoadData();
                        BulkLoadData(oracleConnection, ED11TableDataset);
                        ED11TableDataset.Tables["EDGIS.PGE_ED11_PROCESSES"].Rows.Clear();
                        //ED11TableDataset.Tables["EDGIS.PGE_ED11_PROCESSES"].BeginLoadData();
                        GC.Collect();
                        GC.WaitForPendingFinalizers();
                    }
                }
            }

            //ED11TableDataset.Tables["EDGIS.PGE_ED11_PROCESSES"].EndLoadData();
            BulkLoadData(oracleConnection, ED11TableDataset);
            ED11TableDataset.Tables["EDGIS.PGE_ED11_PROCESSES"].Rows.Clear();
            ShowPercentProgress(string.Format("Preparing EDGIS.PGE_ED11_PROCESSES table: {0} of {1} processed", currentlyProcessed, totalToProcess), currentlyProcessed, totalToProcess);
            GC.Collect();
            Console.WriteLine("");

            currentlyProcessed = 0;
            totalToProcess = transactions.Count;
            ShowPercentProgress(string.Format("Preparing EDGIS.PGE_ED11_PROCESSING table: {0} of {1} processed", currentlyProcessed, totalToProcess), currentlyProcessed, totalToProcess);
            //ED11TableDataset.Tables["EDGIS.PGE_ED11_PROCESSING"].BeginLoadData();
            foreach (PTTTransaction transaction in transactions)
            {
                try
                {
                    string binaryString = SerializeObject(transaction);
                    int splitLength = binaryString.Length / 3;
                    string binaryPartOne = binaryString.Substring(0, splitLength);
                    string binaryPartTwo = binaryString.Substring(splitLength, splitLength);
                    string binaryPartThree = binaryString.Substring(splitLength + splitLength);

                    string sapEquipID1 = transaction.TransactionPole.SAPEquipID;
                    string sapEquipID2 = "";

                    if (transaction.TransactionPole is PoleReplace)
                    {
                        PoleReplace replacePole = transaction.TransactionPole as PoleReplace;
                        sapEquipID1 = replacePole.PoleToDelete.SAPEquipID;
                        sapEquipID2 = replacePole.PoleToInsert.SAPEquipID;
                    }

                    int processID = 0;
                    //foreach (KeyValuePair<int, List<string>> kvp in SAPEquipIDAssignments)
                    //{
                    //    if (kvp.Value.Contains(sapEquipID1))
                    //    {
                    //        processID = kvp.Key;
                    //        break;
                    //    }
                    //}

                    //Add the new row to our in memory dataset
                    DataRow newRow = ED11TableDataset.Tables["EDGIS.PGE_ED11_PROCESSING"].NewRow();
                    newRow["ORDERNUMBER"] = transaction.OrderNumber;
                    newRow["PTTTRANSACTIONPART1"] = binaryPartOne.Replace("'", "''");
                    newRow["PTTTRANSACTIONPART2"] = binaryPartTwo.Replace("'", "''");
                    newRow["PTTTRANSACTIONPART3"] = binaryPartThree.Replace("'", "''");
                    newRow["PROCESSID"] = processID;
                    newRow["TRANSACTIONSUCCESS"] = TransactionStatus.InProgress;
                    newRow["TRANSACTIONSTATUS"] = TransactionErrorType.InProgress;
                    newRow["SAPEQUIPID1"] = sapEquipID1;
                    newRow["SAPEQUIPID2"] = sapEquipID2;
                    newRow["TRANSACTIONTYPE"] = transaction.Type;
                    newRow["TRANSACTIONMESSAGE"] = "";
                    ED11TableDataset.Tables["EDGIS.PGE_ED11_PROCESSING"].Rows.Add(newRow);

                    currentlyProcessed++;
                    if ((currentlyProcessed % 100) == 0) { ShowPercentProgress(string.Format("Preparing EDGIS.PGE_ED11_PROCESSING table: {0} of {1} processed", currentlyProcessed, totalToProcess), currentlyProcessed, totalToProcess); }
                    if ((currentlyProcessed % 10000) == 0)
                    {
                        //Bulk load data every 10,000 records
                        //ED11TableDataset.Tables["EDGIS.PGE_ED11_PROCESSING"].EndLoadData();
                        BulkLoadData(oracleConnection, ED11TableDataset);
                        ED11TableDataset.Tables["EDGIS.PGE_ED11_PROCESSING"].Rows.Clear();
                        //ED11TableDataset.Tables["EDGIS.PGE_ED11_PROCESSING"].BeginLoadData();
                        GC.Collect();
                        GC.WaitForPendingFinalizers();
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Failed to process XML: " + ex.Message);
                }
            }

            //ED11TableDataset.Tables["EDGIS.PGE_ED11_PROCESSING"].EndLoadData();
            BulkLoadData(oracleConnection, ED11TableDataset);
            GC.Collect();

            ShowPercentProgress(string.Format("Preparing EDGIS.PGE_ED11_PROCESSING table: {0} of {1} processed", currentlyProcessed, totalToProcess, ED11ProcessingTable), currentlyProcessed, totalToProcess);
            Console.WriteLine("");

            //Assign processes to transactions
            Console.WriteLine("Assign processes to transactions");
            string sql = string.Format("MERGE INTO {0} P USING (SELECT {2},{3} FROM {1}) P2 ON (P.{4} = P2.{3}) WHEN MATCHED THEN UPDATE SET P.{2} = P2.{2}",
                ED11ProcessingTable, ED11ProcessesTable, "PROCESSID", "SAPEQUIPID", "SAPEQUIPID1");
            ((IWorkspace)EditVersion).ExecuteSQL(sql);

            ((IWorkspace)EditVersion).ExecuteSQL("commit");

            return transactions;
        }
        #endregion
        public static void BulkLoadData(OracleConnection connection, DataSet set)
        {
            try
            {
                using (OracleBulkCopy copy = new OracleBulkCopy(connection))
                {
                    //copy.DestinationTableName = tableName;
                    copy.BulkCopyTimeout = 600;
                    //copy.BatchSize = BatchSize;
                    //copy.Insert(entities);
                    foreach (DataTable table in set.Tables)
                    {
                        //if (logBulkCopy) { Console.WriteLine(string.Format("Bulk copying {1} rows into table {0}", table.TableName, table.Rows.Count)); }
                        copy.DestinationTableName = table.TableName;
                        copy.WriteToServer(table);
                    }
                    copy.Close();
                    copy.Dispose();
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
            }
        }        

        public static DataSet GetDataset(OracleConnection connection, string tableName, string tableName2)
        {
            try
            {
                DataTable oleschema = connection.GetSchema("Tables");//This returns a table with a list of all the tables in the connected schema
                System.Data.Common.DbCommand GetTableCmd = connection.CreateCommand();
                System.Data.Common.DbDataAdapter ODA = null;

                ODA = new OracleDataAdapter() as System.Data.Common.DbDataAdapter;

                ODA.SelectCommand = GetTableCmd;
                DataSet set = new DataSet();
                foreach (DataRow row in oleschema.Rows)
                {
                    if (row["TABLE_NAME"].Equals(tableName) || row["TABLE_NAME"].Equals(tableName2))
                    {
                        DataTable DBTable = new DataTable();
                        GetTableCmd.CommandText = "SELECT * FROM " + row["OWNER"] + "." + row["TABLE_NAME"]; ;
                        ODA.FillSchema(DBTable, SchemaType.Source);//This pulls down the schema for the given table
                        DBTable.TableName = row["OWNER"] + "." + DBTable.TableName;
                        set.Tables.Add(DBTable);
                        DBTable = null;
                    }
                }

                return set;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
            }
            return null;
        }      


        /// <summary>
        /// Returns the transactions associated with this ProcessIdentifier from the database
        /// </summary>
        /// <returns></returns>
        private List<PTTTransaction> GetTransactions()
        {
            List<PTTTransaction> transactions = new List<PTTTransaction>();

            ITable PTTTransactionsTable = ((IFeatureWorkspace)EditVersion).OpenTable(ED11ProcessingTable);
            IQueryFilter qf = new QueryFilterClass();
            IQueryFilterDefinition queryDef = qf as IQueryFilterDefinition;
            queryDef.PostfixClause = "ORDER BY ORDERNUMBER";
            qf.WhereClause = "PROCESSID = " + ProcessIdentifier;

            ICursor rowCursor = PTTTransactionsTable.Search(qf, false);
            IRow row = null;
            while ((row = rowCursor.NextRow()) != null)
            {
                //Build our PTT Transaction from the binary values in the database
                string binaryPartOne = row.get_Value(1).ToString();
                string binaryPartTwo = row.get_Value(2).ToString();
                string binaryPartThree = row.get_Value(3).ToString();
                string binary = binaryPartOne + binaryPartTwo + binaryPartThree;
                PTTTransaction transaction = (PTTTransaction)DeserializeObject(binary);
                transactions.Add(transaction);
            }
            return transactions;
        }

        /// <summary>
        /// Produces the ED12 file from the results table
        /// </summary>
        /// <returns></returns>
        private void ProduceED12File(ref int insertsProcessed, ref int insertsFailed, ref int updatesProcess, ref int updatesFailed, ref int deletesProcessed,
            ref int deletesFailed, ref int replacesProcessed, ref int replacesFailed)
        {
            List<PTTTransaction> transactions = new List<PTTTransaction>();

            ITable PTTTransactionsTable = ((IFeatureWorkspace)EditVersion).OpenTable(ED11ProcessingTable);
            ITable ED12StagingTable = ((IFeatureWorkspace)EditVersion).OpenTable(Common.ED12StagingTable);
            IQueryFilter qf = new QueryFilterClass();
            IQueryFilterDefinition queryDef = qf as IQueryFilterDefinition;
            queryDef.PostfixClause = "ORDER BY ORDERNUMBER";

            ICursor rowCursor = PTTTransactionsTable.Search(qf, false);
            IRow row = null;
            while ((row = rowCursor.NextRow()) != null)
            {
                //Build our PTT Transaction from the binary values in the database
                string binaryPartOne = row.get_Value(1).ToString();
                string binaryPartTwo = row.get_Value(2).ToString();
                string binaryPartThree = row.get_Value(3).ToString();
                string binary = binaryPartOne + binaryPartTwo + binaryPartThree;
                PTTTransaction transaction = (PTTTransaction)DeserializeObject(binary);
                transactions.Add(transaction);
            }

            try { ErrorLoggingWriter = new StreamWriter(Common.PTTTransactionStatusFile); }
            catch (Exception ex) { throw new Exception("Failed to create transaction status file " + Common.PTTTransactionStatusFile + " EXC " + ex.Message); }

            ErrorLoggingWriter.WriteLine("<PTTGISStatuses>");
            foreach (PTTTransaction transaction in transactions)
            {
                LogTransactionError(transaction.TransactionPole, transaction.Type);
               // LogTransactionErrorInStaging(transaction.TransactionPole, transaction.Type);
            }

            //Finish the ED12 status file tags
            if (ErrorLoggingWriter != null)
            {
                ErrorLoggingWriter.WriteLine("</PTTGISStatuses>");
                ErrorLoggingWriter.Flush();
                ErrorLoggingWriter.Close();
                ErrorLoggingWriter = null;
            }

            //Archive our ED12 file for review later
        //    ArchiveED12Files(Common.PTTTransactionStatusFile);

            //Get our successfully and failed counts
            qf = new QueryFilterClass();
            qf.WhereClause = string.Format("TRANSACTIONTYPE = '{0}'", TransactionType.Insert);
            insertsProcessed = PTTTransactionsTable.RowCount(qf);
            qf.WhereClause = string.Format("TRANSACTIONSUCCESS = '{0}' AND TRANSACTIONTYPE = '{1}'", TransactionStatus.Failure, TransactionType.Insert);
            insertsFailed = PTTTransactionsTable.RowCount(qf);

            qf.WhereClause = string.Format("TRANSACTIONTYPE = '{0}'", TransactionType.Update);
            updatesProcess = PTTTransactionsTable.RowCount(qf);
            qf.WhereClause = string.Format("TRANSACTIONSUCCESS = '{0}' AND TRANSACTIONTYPE = '{1}'", TransactionStatus.Failure, TransactionType.Update);
            updatesFailed = PTTTransactionsTable.RowCount(qf);

            qf.WhereClause = string.Format("TRANSACTIONTYPE = '{0}'", TransactionType.Delete);
            deletesProcessed = PTTTransactionsTable.RowCount(qf);
            qf.WhereClause = string.Format("TRANSACTIONSUCCESS = '{0}' AND TRANSACTIONTYPE = '{1}'", TransactionStatus.Failure, TransactionType.Delete);
            deletesFailed = PTTTransactionsTable.RowCount(qf);

            qf.WhereClause = string.Format("TRANSACTIONTYPE = '{0}'", TransactionType.Replace);
            replacesProcessed = PTTTransactionsTable.RowCount(qf);
            qf.WhereClause = string.Format("TRANSACTIONSUCCESS = '{0}' AND TRANSACTIONTYPE = '{1}'", TransactionStatus.Failure, TransactionType.Replace);
            replacesFailed = PTTTransactionsTable.RowCount(qf);

        }

        public static string SerializeObject(object o)
        {
            if (!o.GetType().IsSerializable)
            {
                return null;
            }

            using (MemoryStream stream = new MemoryStream())
            {
                new BinaryFormatter().Serialize(stream, o);
                return Convert.ToBase64String(stream.ToArray());
            }
        }

        public static object DeserializeObject(string str)
        {
            byte[] bytes = Convert.FromBase64String(str);

            using (MemoryStream stream = new MemoryStream(bytes))
            {
                return new BinaryFormatter().Deserialize(stream);
            }
        }

        private PTTTransaction ProcessInsertElement(XmlNode element)
        {
            PTTTransaction insertTransaction = new PTTTransaction();
            insertTransaction.Type = TransactionType.Insert;

            PoleInsert newPole = new PoleInsert();
            newPole.OriginalTransaction += "\t\t\t<PTTTransactionNewPole>\r\n";
            newPole.FieldValues = new Dictionary<string, string>();
            for (int i = 0; i < element.ChildNodes.Count; i++)
            {
                XmlNode node = element.ChildNodes.Item(i);
                string AttributeName = node.Name;
                string value = node.InnerText;

                string encodedXmlValue = value.Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;").Replace("\"", "&quot;").Replace("'", "&apos;");
                newPole.OriginalTransaction += "\t\t\t\t<" + AttributeName + ">" + encodedXmlValue + "</" + AttributeName + ">\r\n";

                if (!((AttributeName.ToUpper() == "TRANSACTIONTYPE") || (AttributeName.ToUpper() == "OLDVALUE_U") || (AttributeName.ToUpper() == "NEWVALUE_U") || (AttributeName.ToUpper() == "ATTRIBUTENAME_U")))
                {

                    if (AttributeName.ToUpper() == "GUID_I") { ((Pole)newPole).SetGuid(value); }
                    else if (AttributeName.ToUpper() == "EQUIPMENTNUMBER_I") { ((Pole)newPole).SAPEquipID = value; }
                    else if (AttributeName.ToUpper() == "PROJECTID_I") { ((Pole)newPole).ProjectID = value; }
                    else if (AttributeName.ToUpper() == "LATITUDE_I") { newPole.SetLatitude(value); }
                    else if (AttributeName.ToUpper() == "LONGITUDE_I") { newPole.SetLongitude(value); }
                    // m4jf edgisrearch 415 ed11 improvements 
                    else if (AttributeName.ToUpper() == "RECORDID") { ((Pole)newPole).RecordID = value; }
                    else if (AttributeName.ToUpper() == "POLECLASS") { ((Pole)newPole).PoleClass = value; }
                    else if (AttributeName.ToUpper() == "HEIGHT") { ((Pole)newPole).Height = value; }
                   // else if (AttributeName.ToUpper() == "MATERIAL") { ((Pole)newPole).Material = value; }
                   // else if (AttributeName.ToUpper() == "PLANTSECTION_I") { ((Pole)newPole).PlantSection = value; }
                    else if (AttributeName.ToUpper() == "STARTUPDATE_I") { ((Pole)newPole).StartupDate = value; }


                    else { newPole.FieldValues.Add(AttributeName, value); }
                }
            }
            newPole.OriginalTransaction += "\t\t\t</PTTTransactionNewPole>\r\n";
            insertTransaction.TransactionPole = newPole;
            return insertTransaction;
        }

        private PTTTransaction ProcessReplaceElement(XmlNode element)
        {
            PTTTransaction replaceTransaction = new PTTTransaction();
            replaceTransaction.Type = TransactionType.Replace;

            PoleReplace replacePole = new PoleReplace();
            replacePole.OriginalTransaction += "\t\t\t<PTTTransactionReplacePole>\r\n";

            // M4JF EDGISREARCH 415 - Commented below line of code as there will field value entries in staging table for insert and delete records
            //for (int i = 0; i < element.ChildNodes.Count; i++)
            //{
            //    XmlNode childNode = element.ChildNodes.Item(i);
            //    if (childNode.Name == "PTTTransactionNewPole")
            //    {
                    PTTTransaction insertTransaction = ProcessInsertElement(element);
                    replacePole.PoleToInsert = insertTransaction.TransactionPole as PoleInsert;
                    replacePole.OriginalTransaction += replacePole.PoleToInsert.OriginalTransaction.Replace("\t\t\t", "\t\t\t\t");
               // }
                //else if (childNode.Name == "PTTTransactionDeletePole")
                //{
                    PTTTransaction deleteTransaction = ProcessDeleteElement(element);
                    replacePole.PoleToDelete = deleteTransaction.TransactionPole as PoleDelete;
                    replacePole.OriginalTransaction += replacePole.PoleToDelete.OriginalTransaction.Replace("\t\t\t", "\t\t\t\t");
               // }
           // }

            if (replacePole.PoleToInsert.PoleMalformedXml)
            {
                replacePole.PoleMalformedXml = true;
                replacePole.PoleMalformedMessage = replacePole.PoleToInsert.PoleMalformedMessage;
            }
            if (replacePole.PoleToDelete.PoleMalformedXml)
            {
                replacePole.PoleMalformedXml = true;
                replacePole.PoleMalformedMessage = replacePole.PoleToDelete.PoleMalformedMessage;
            }

            replacePole.OriginalTransaction += "\t\t\t</PTTTransactionReplacePole>\r\n";
            replaceTransaction.TransactionPole = replacePole;
            return replaceTransaction;
        }

        private PTTTransaction ProcessUpdateElement(XmlNode element)
        {
            PTTTransaction updateTransaction = new PTTTransaction();
            updateTransaction.Type = TransactionType.Update;

            PoleUpdate newPole = new PoleUpdate();
            newPole.OriginalTransaction += "\t\t\t<PTTTransactionUpdatePole>\r\n";
            for (int i = 0; i < element.ChildNodes.Count; i++)
            {
                XmlNode node = element.ChildNodes.Item(i);
                string AttributeName = node.Name;
                string value = node.InnerText;

                string encodedXmlValue = value.Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;").Replace("\"", "&quot;").Replace("'", "&apos;");
                newPole.OriginalTransaction += "\t\t\t\t<" + AttributeName + ">" + encodedXmlValue + "</" + AttributeName + ">\r\n";

                if (AttributeName.ToUpper() == "GUID_U") { ((Pole)newPole).SetGuid(value); }
                else if (AttributeName.ToUpper() == "EQUIPMENTNUMBER_U") { ((Pole)newPole).SAPEquipID = value; }
                else if (AttributeName.ToUpper() == "PROJECTID_U") { ((Pole)newPole).ProjectID = value; }
                else if (AttributeName.ToUpper() == "ATTRIBUTENAME_U") { newPole.FieldName = value; }
                else if (AttributeName.ToUpper() == "OLDVALUE_U") { newPole.OldFieldValue = value; }
                else if (AttributeName.ToUpper() == "NEWVALUE_U") { newPole.NewFieldValue = value; }
                // m4jf edgisrearch 415 ed11 improvements 
                else if (AttributeName.ToUpper() == "RECORDID") { ((Pole)newPole).RecordID = value; }
            }
            newPole.OriginalTransaction += "\t\t\t</PTTTransactionUpdatePole>\r\n";
            updateTransaction.TransactionPole = newPole;
            return updateTransaction;
        }

        private PTTTransaction ProcessDeleteElement(XmlNode element)
        {
            PTTTransaction deleteTransaction = new PTTTransaction();
            deleteTransaction.Type = TransactionType.Delete;

            PoleDelete newPole = new PoleDelete();
            newPole.OriginalTransaction += "\t\t\t<PTTTransactionDeletePole>\r\n";
            for (int i = 0; i < element.ChildNodes.Count; i++)
            {
                XmlNode node = element.ChildNodes.Item(i);
                string AttributeName = node.Name;        
                string value = node.InnerText;
                string encodedXmlValue = value.Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;").Replace("\"", "&quot;").Replace("'", "&apos;");
                newPole.OriginalTransaction += "\t\t\t\t<" + AttributeName + ">" + encodedXmlValue + "</" + AttributeName + ">\r\n";

                if (AttributeName.ToUpper() == "GUID_D") { ((Pole)newPole).SetGuid(value); }
                else if (AttributeName.ToUpper() == "EQUIPMENTNUMBER_D") { ((Pole)newPole).SAPEquipID = value; }
                else if (AttributeName.ToUpper() == "PROJECTID_D") { ((Pole)newPole).ProjectID = value; }
                // m4jf edgisrearch 415 ed11 improvements 
                else if (AttributeName.ToUpper() == "STARTUPDATE_D") { ((Pole)newPole).InstallationDate = value; }
                else if (AttributeName.ToUpper() == "RECORDID") { ((Pole)newPole).RecordID = value; }
            }
            newPole.OriginalTransaction += "\t\t\t</PTTTransactionDeletePole>\r\n";
            deleteTransaction.TransactionPole = newPole;
            return deleteTransaction;
        }

        #endregion

    }
    public class ProcessFlag
    {
        // Processed flag in all interfaces should be updated properly :(P- GIS Processed/E-GIS Error/T-In Transition/F-Failed/D- Done)

        public static string GISProcessed { get; } = "P";
        public static string GISError { get; } = "E";
        public static string InTransition { get; } = "T";
        public static string Failed { get; } = "F";
        public static string Completed { get; } = "D";

    }
    public class CachedFeature
    {
        public CachedFeature() { }
        public CachedFeature(IFeature feature, Guid guid)
        {
            PoleFeature = feature;
            this.guid = guid;
        }

        public CachedFeature(IFeature feature, string SAPEquipID)
        {
            PoleFeature = feature;
            this.SapEquipID = SAPEquipID;
        }

        public CachedFeature(IFeature feature, Guid guid, string SAPEquipID)
        {
            PoleFeature = feature;
            this.guid = guid;
            this.SapEquipID = SAPEquipID;
        }

        public Guid guid { get; set; }
        public string SapEquipID { get; set; }
        public IFeature PoleFeature { get; set; }
        public string FeatureClassName
        {
            get
            {
                if (PoleFeature != null)
                {
                    return ((IDataset)PoleFeature.Class).BrowseName;
                }
                return "";
            }
        }

        public override bool Equals(object obj)
        {
            if (obj is CachedFeature)
            {
                if (this.guid == ((CachedFeature)obj).guid)
                {
                    //guids match so this is the same feature
                    return true;
                }
                else if (this.SapEquipID == ((CachedFeature)obj).SapEquipID && (this.guid == Guid.Empty || ((CachedFeature)obj).guid == Guid.Empty))
                {
                    //Matched the SAP Equipment ID because one of the guids is empty and can't be matched.  Will be the case for insert features
                    return true;
                }
            }
            else if (obj is IFeature)
            {
                if (obj != null)
                {
                    if (obj == PoleFeature) { return true; }
                }
            }

            return false;
        }

        public Dictionary<IRelationshipClass, List<IObject>> RelatedObjects = new Dictionary<IRelationshipClass, List<IObject>>();

    }

    public class FeatureClassInformation
    {
        public FeatureClassInformation(IFeatureClass featureClass)
        {
            FeatureClassName = ((IDataset)featureClass).BrowseName;
            FieldIndices = new Dictionary<string, int>();

            for (int i = 0; i < featureClass.Fields.FieldCount; i++)
            {
                IField field = featureClass.Fields.get_Field(i);
                FieldIndices.Add(field.Name.ToUpper(), i);
            }

            DestinationRelationshipClasses = new List<IRelationshipClass>();
            OriginRelationshipClasses = new List<IRelationshipClass>();

            IEnumRelationshipClass destinationRelClasses = featureClass.get_RelationshipClasses(esriRelRole.esriRelRoleDestination);
            IRelationshipClass relClass = null;
            while ((relClass = destinationRelClasses.Next()) != null)
            {
                DestinationRelationshipClasses.Add(relClass);
            }

            IEnumRelationshipClass OriginRelClasses = featureClass.get_RelationshipClasses(esriRelRole.esriRelRoleOrigin);
            relClass = null;
            while ((relClass = OriginRelClasses.Next()) != null)
            {
                OriginRelationshipClasses.Add(relClass);
            }
        }

        public List<IRelationshipClass> DestinationRelationshipClasses { get; set; }
        public List<IRelationshipClass> OriginRelationshipClasses { get; set; }

        public string FeatureClassName { get; set; }

        private Dictionary<string, int> FieldIndices { get; set; }

        public int GetFieldIndex(string fieldName)
        {
            if (FieldIndices.ContainsKey(fieldName.ToUpper()))
            {
                return FieldIndices[fieldName.ToUpper()];
            }
            else { return -1; }
        }

        public override bool Equals(object obj)
        {
            if (obj is IDataset && ((IDataset)obj).BrowseName == FeatureClassName)
            {
                return true;
            }
            else if (obj is FeatureClassInformation && ((FeatureClassInformation)obj).FeatureClassName == FeatureClassName)
            {
                return true;
            }
            return false;
        }




    }

    /// <summary>    
    /// m4jf edgisrearch 415
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
        public static string Interface { get; } = "ED11;";
        /// <summary>
        /// This property to get name of the system to integration
        /// </summary>
        public static string Integration { get; } = "SAP;";
        /// <summary>
        /// This property to get type of interface
        /// </summary>
        public static string Type { get; } = "Inbound;";
    }


}
