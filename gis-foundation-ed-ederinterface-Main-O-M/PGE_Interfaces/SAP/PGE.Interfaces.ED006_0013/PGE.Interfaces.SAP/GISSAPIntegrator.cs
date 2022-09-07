using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Xml;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;//IVARA

using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.GeoDatabaseDistributed;
using EsriGeometry = ESRI.ArcGIS.Geometry;
using PGE.Interfaces.SAP.RowTransformers;

using Oracle.DataAccess.Client;
using PGE.Interfaces.Integration.Framework;
using PGE.Interfaces.Integration.Framework.Data;
using PGE.Common.Delivery.Geodatabase.ChangeDetection;
using PGE.Common.Delivery.Framework;
using PGE.Common.Delivery.Framework.FeederManager;
using PGE.Common.Delivery.Systems;
using PGE_DBPasswordManagement;
//V3SF - CD API (EDGISREARC-1452) - Added
using PGE.Common.ChangeDetectionAPI;

//(V3SF) - ED06 Improved Logging
using System.Data;
using System.Globalization;
using System.Threading;

namespace PGE.Interfaces.SAP
{
    /// <summary>
    /// The major class that transforms all edits in all classes into SAP format.
    /// </summary>
    public class GISSAPIntegrator : IGISIntegration<string>
    {
        #region Private Properties
        private IVersion _editVersion;
        private IVersion _targetVersion;
        private const string _configFileName = "GISSAP_AssetSynch_Config.xml";
        private const string _xsdFileName = "SAPIntegrationConfig.xsd";
        private const string _domainsFileName = "Domains.xml";
        private const string _domainsMappingName = "SAP";
        private const string _GUID = "GUID";
        private SystemMapper _systemMapper = null;

        private List<Exception> _errors;
        //private static ILog _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public static PGE.Common.Delivery.Diagnostics.Log4NetLogger _logger;//= new PGE.Common.Delivery.Diagnostics.Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "PGE.Interfaces.SAP.ED06.Batch.log4net.config");
        //private static ILog _logger = LogManager.GetLogger("PGE.Interfaces.SAP");

        Action<string> OtherLogMethod = null;
        private Configuration _config;



        #endregion

        #region Constructors

        /// <summary>
        /// Create an instance of the main class, With two valid versions to compare for changes.
        /// </summary>
        /// <param name="editVersion">The version where edits were made.</param>
        /// <param name="targetVersion">The version to compare to, usually parent version of editVersion.</param>
        /// <param name="alternateLogMethod"></param>
        public GISSAPIntegrator(IVersion editVersion, IVersion targetVersion, Action<string> alternateLogMethod)
        {
            try
            {

                Console.WriteLine("Constructor testing");

                Console.WriteLine("Constructor Called");
                _logger = new PGE.Common.Delivery.Diagnostics.Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "PGE.Interfaces.SAP.ED06.Batch.log4net.config");
                OtherLogMethod = alternateLogMethod;
                Console.WriteLine("otherlogmethod Called");
                _editVersion = editVersion;
                Console.WriteLine("edit version Called");
                _targetVersion = targetVersion;
                Console.WriteLine("_targetVersion Called");
                Console.WriteLine("Constructor end");
                try
                {
                    //  _dbHelper;
                    //DataHelper dbHelper = new DataHelper();
                    Console.WriteLine("Constructor end");
                }
                catch (Oracle.DataAccess.Client.OracleException oex) { GISSAPIntegrator._logger.Error(oex.GetType() + " || " + oex.Number + " || " + oex.Message + " || \n" + oex.StackTrace); if (GISSAPIntegrator.HandleOracleErrorCodes || GISSAPIntegrator.OracleErrorCodes.Contains(Convert.ToString(oex.ErrorCode))) { throw oex; } }
                catch(Exception ex)
                {
                    Console.WriteLine("dbhelper Constructor exception" + ex.Message);

                }
                // _logger = new PGE.Common.Delivery.Diagnostics.Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "PGE.Interfaces.SAP.ED06.Batch.log4net.config");
            }
            catch (Oracle.DataAccess.Client.OracleException oex) { GISSAPIntegrator._logger.Error(oex.GetType() + " || " + oex.Number + " || " + oex.Message + " || \n" + oex.StackTrace); if (GISSAPIntegrator.HandleOracleErrorCodes || GISSAPIntegrator.OracleErrorCodes.Contains(Convert.ToString(oex.ErrorCode))) { throw oex; } }
            catch (Exception ex)
            {
                Console.WriteLine("Constructor exception" + ex.Message);
            }

        }


        /// <summary>
        /// Create an instance of the main class, With two valid versions to compare for changes.
        /// </summary>
        /// <param name="editVersion">The version where edits were made.</param>
        /// <param name="targetVersion">The version to compare to, usually parent version of editVersion.</param>
        /// <param name="alternateLogMethod"></param>
        public GISSAPIntegrator(IVersion editVersion, IVersion targetVersion)
        {
            _editVersion = editVersion;
            _targetVersion = targetVersion;
        }

        #endregion

        /// <summary>
        /// Stores processing times for debugging and testing
        /// </summary>
        public Dictionary<string, Dictionary<int, double>> AssetOIDAndProcessTimeByClass = new Dictionary<string, Dictionary<int, double>>();
        private DataHelper dbHelper;

        public static string ReprocessTable = "EDGIS.PGE_GISSAP_REPROCESSASSETSYNC";
        public static string gisSapAssetSynchTableName = "EDGIS.PGE_GISSAP_AssetSynch";

        public static string remark = string.Empty;
        public static DateTime CDRunDate;
        public static DateTime lastRunDate;
        public static DateTime endDate;
        public static bool ResetSequence;
        public static string CDWhereClause;
        public static string GDBM_CD_Table = string.Empty;
        public static string Bkp_Schema_Credentials = string.Empty;
        public static bool dateUpdate = false;
        public static int RecordCount = 10000;
        public static int TimeDelay = 2000;
        public static int RetryCount = 1;
        public static bool HandleOracleErrorCodes = false;
        public static List<string> OracleErrorCodes = new List<string>();
        public static string EDER_Credential = string.Empty;
        public static StringBuilder _errorMessage = new StringBuilder();
        //#region public static member
        //public static Dictionary<string, Dictionary<string, IRowData>> AssetIDandRowDataByOutName 
        //    = new Dictionary<string, Dictionary<string, IRowData>>();
        //public static Dictionary<string, IRelationshipClass2> RelationshipClassByNameInEditVersion 
        //    = new Dictionary<string, IRelationshipClass2>();
        //public static Dictionary<string, IRelationshipClass2> RelationshipClassByNameInTargetVersion 
        //    = new Dictionary<string, IRelationshipClass2>();
        //public static List<string> DeviceGroupGlobalIDs = new List<string>();
        //#endregion

        //Added to handle Missing record issue EDGIS Rearch ED06 Inteface Improvement -v1t8
        public static List<int> nonSAPRecordList = null;

        #region Public Methods

        #region IGISIntegration Members

        /// <summary>
        /// Call Initialize and only if return value is true call Process
        /// </summary>
        /// <returns>True if successful, otherwise false</returns>
        public bool Initialize()
        {
            Log("Starting Initialize in GISSAPIntegrator at: " + DateTime.Now, MessageType.Debug);
            bool retVal = false;

            _errors = new List<Exception>();

            try
            {
                CircuitSourceRowTransformer.ProcessedOIDs.Clear();
                ControllerRowTransformer.ProcessedOIDs.Clear();

                if (LoadConfiguration())
                {
                    if (OnInitialize != null)
                    {
                        OnInitialize(this, new EventArgs());
                    }
                    retVal = true;
                    Console.WriteLine("LoadConfiguration Successful");
                }
            }
            catch (Oracle.DataAccess.Client.OracleException oex) { GISSAPIntegrator._logger.Error(oex.GetType() + " || " + oex.Number + " || " + oex.Message + " || \n" + oex.StackTrace); if (GISSAPIntegrator.HandleOracleErrorCodes || GISSAPIntegrator.OracleErrorCodes.Contains(Convert.ToString(oex.ErrorCode))) { throw oex; } }
            catch (Exception ex)
            {
                Log("Exception in" + MethodInfo.GetCurrentMethod().Name + ex.StackTrace, MessageType.Error);
            }
            return retVal;
        }

        private void LogMethodPlaceHolder(string message)
        {

        }

        /// <summary>
        /// Main method; call this after Initialize returns true.
        /// </summary>
        /// <returns></returns>
        public bool Process()
        {
            bool successfulRun = true;
            int retryCounter = 0;
            Log("Starting processing in GISSAPIntegrator at: " + DateTime.Now + " version " + _editVersion.VersionName, MessageType.Debug);
            Log("Starting processing version " + _editVersion.VersionName + " at: " + DateTime.Now, MessageType.Info);
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            DateTime date = default;
            string TableName = null;
            List<string> BatchIDs = default;
            CDBatchManager cDBatchManager = default;
            IWorkspace pWorkspace = default;
            try
            {
                TableName = "INTDATAARCH.CD_ED06_TEMP";
                BatchIDs = new List<string>();
                cDBatchManager = new CDBatchManager(Bkp_Schema_Credentials, TableName);

                if (ResetSequence)
                {
                    DataHelper dataHelper = new DataHelper(false);
                    dataHelper.ResetSequence();
                }

                //V3SF - CD API (EDGISREARC-1452) - Added [START]
                IList<string> TargetFCs = new List<string>();
                foreach (TrackedClass trackedCls in _systemMapper.TrackedClasses)
                {
                    if (!TargetFCs.Contains(trackedCls.QualifiedSourceClass))
                        TargetFCs.Add(trackedCls.QualifiedSourceClass);

                    if (trackedCls.RelatedClass != null)
                    {
                        if (!TargetFCs.Contains(trackedCls.RelatedClass.QualifiedSourceClass))
                            TargetFCs.Add(trackedCls.RelatedClass.QualifiedSourceClass);
                    }

                }

                //Process failed -------------------------------------------------------------------------------------------------------
                //Execute for All Batch IDS where were failed or not processed in previous run
                BatchIDs = cDBatchManager.GetUnProcessedBatchID(TableName);

                //(V3SF) - ED06 Improved Logging ( Fixed to re-process failed records along with previous batch - fix com exception on execution )
                if (BatchIDs.Count > 0) // (V3SF) In case of processing failed scenario process previous batch
                {
                    if (BatchIDs[0] != "1")
                    {
                        int num = Convert.ToInt32(BatchIDs[0]);
                        num -= 1;
                        BatchIDs.Insert(0, Convert.ToString(num));
                    }
                }

                // where record status <> 'P' for each FC
                foreach (string Batch in BatchIDs)
                {
                    retryF:
                    try
                    {
                        cDBatchManager.UpdateProcessFlag(TableName, Batch, "T");
                        ProcessCDChanges(cDBatchManager, TargetFCs, lastRunDate, CDRunDate, Batch, TableName);
                        cDBatchManager.UpdateProcessFlag(TableName, Batch, "P"); //Updating as FC Level
                    }
                    catch(Exception ex)
                    {
                        //(V3SF) - ED06 Improved Logging
                        if(retryCounter < RetryCount)
                        {
                            _logger.Info("Retry: " + retryCounter);
                            _logger.Error("Error Processing Batch " + Batch + " with error " + ex.Message + " of Type :: " + ex.GetType());
                            Thread.Sleep(TimeDelay);
                            pWorkspace = ArcSdeWorkspaceFromFile(EDER_Credential);
                            _editVersion = ((IVersionedWorkspace)pWorkspace).DefaultVersion;
                            _targetVersion = ((IVersionedWorkspace)pWorkspace).DefaultVersion;
                            retryCounter++;
                            goto retryF;
                        }
                        _logger.Fatal("Error Processing Batch " + Batch + " with error " + ex.Message + " of Type :: " + ex.GetType());
                        //_logger.Error("Error Processing Batch " + Batch + " with error " + ex.Message + " of Type :: " + ex.GetType() );
                       throw new Exception("Error Processing Batch " + Batch + " with error " + ex.Message + " at " + ex.StackTrace);
                    }
                }
                //Process failed -------------------------------------------------------------------------------------------------------

                //Use these lines for testing this line
                //TargetFCs.Clear();
                //TargetFCs.Add("EDGIS.SWITCH");
                date = cDBatchManager.GetDate(TableName);
                if (date != DateTime.MinValue)
                    if(!dateUpdate)
                        lastRunDate = date;
                try
                {
                    //Get Database Time in TimeZone -- select to_char(sysdate,'DD-MON-YY HH24:MM:SS') from dual;
                    if (endDate == DateTime.MinValue)
                        //(V3SF) - ED06 Improved Logging                        
                        //CDRunDate = DateTime.Now;
                        CDRunDate = GetCurrentTime(Bkp_Schema_Credentials);
                    else
                        CDRunDate = endDate;
                }
                catch
                {
                    //(V3SF) - ED06 Improved Logging                    
                    //CDRunDate = DateTime.Now;
                    CDRunDate = GetCurrentTime(Bkp_Schema_Credentials);
                }

                BatchIDs = cDBatchManager.PrepareTempTable(TableName, RecordCount, changeTypes.All, listType.FeatureClass, fromDate: lastRunDate, toDate: CDRunDate, whereClause: CDWhereClause, TargetFCs: TargetFCs,_gdbmTable: GDBM_CD_Table);

                foreach (string Batch in BatchIDs)
                {
                    retryP:
                    try
                    {
                        cDBatchManager.UpdateProcessFlag(TableName, Batch, "T");
                        ProcessCDChanges(cDBatchManager, TargetFCs, lastRunDate, CDRunDate, Batch, TableName);
                        cDBatchManager.UpdateProcessFlag(TableName, Batch, "P"); //Updating as FC Level
                    }
                    catch (Exception ex)
                    {
                        if (retryCounter < RetryCount)
                        {
                            _logger.Error("Error Processing Batch " + Batch + " with error " + ex.Message + " of Type :: " + ex.GetType());
                            Thread.Sleep(TimeDelay);
                            pWorkspace = ArcSdeWorkspaceFromFile(EDER_Credential);
                            _editVersion = ((IVersionedWorkspace)pWorkspace).DefaultVersion;
                            _targetVersion = ((IVersionedWorkspace)pWorkspace).DefaultVersion;
                            retryCounter++;
                            goto retryP;
                        }
                        //(V3SF) - ED06 Improved Logging
                        _logger.Fatal("Error Processing Batch " + Batch + " with error " + ex.Message + " of Type :: " + ex.GetType());
                        //_logger.Error("Error Processing Batch " + Batch + " with error " + ex.Message + " of Type :: " + ex.GetType());
                        throw new Exception("Error Processing Batch " + Batch + " with error " + ex.Message + " at " + ex.StackTrace);
                    }
                }

                #region Commented for Batch Approach
                //CDVersionManager cDVersionManager = new CDVersionManager(_editVersion as IWorkspace, GDBM_CD_Table, Bkp_Schema_Credentials);
                ////IDictionary<string, ChangedFeatures> cdVersionDifference = cDVersionManager.CDVersionDifference(changeTypes.All, listType.FeatureClass, fromDate: new DateTime(2020, 8, 1, 5, 10, 20), toDate: DateTime.Now, withReplacement: false, whereClause: "(USAGE IS NULL OR USAGE NOT LIKE 'NOED06')", TargetFCs: TargetFCs);
                //IDictionary<string, ChangedFeatures> cdVersionDifference = cDVersionManager.CDVersionDifference(changeTypes.All, listType.FeatureClass, fromDate: lastRunDate, toDate: CDRunDate, withReplacement: false, whereClause: CDWhereClause, TargetFCs: TargetFCs);
                ////V3SF - CD API (EDGISREARC-1452) - Added [END]
                //DifferenceManager_CDAPI diffMan = new DifferenceManager_CDAPI(_targetVersion, _editVersion, cdVersionDifference);
                //EsriGeometry.IGeometryBag childrenMaintenancePolygons = null;

                ////Below code commented for EDGIS-ReArch Change detection. Temp table will not used -v1t8
                ////Delete the temporary 
                ////using (AssetProcessor assetProcessor = new AssetProcessor())
                ////{
                ////    DeleteTempAssetRows(assetProcessor);
                ////}

                //foreach (TrackedClass trackedCls in _systemMapper.TrackedClasses)
                //{
                //    AssetOIDAndProcessTimeByClass[trackedCls.OutName + trackedCls.SourceClass] = new Dictionary<int, double>();
                //    //if (trackedCls.SourceClass != "Controller")
                //    //    continue;

                //    //v1t8 -Entry point 1
                //    //Below method is used for change detection approch 
                //    Process(diffMan, trackedCls, childrenMaintenancePolygons);

                //    //This method is used for reverse sysnc-no use in re arch change detection-v1t8
                //    // ProcessTrackedClassAssetIDandRowData(trackedCls, childrenMaintenancePolygons);

                //    /*
                //// time for each row
                //if (_logger.IsDebugEnabled)
                //{
                //    Dictionary<string, Dictionary<int, double>> assetOIDAndProcessingTimeByClass = this.AssetOIDAndProcessTimeByClass;
                //    foreach (KeyValuePair<string, Dictionary<int, double>> assetOIDTimeOneClass in assetOIDAndProcessingTimeByClass)
                //    {
                //        string assetOutNameSourceName = assetOIDTimeOneClass.Key;
                //        foreach (KeyValuePair<int, double> idTime in assetOIDTimeOneClass.Value)
                //        {
                //            string tmp = string.Empty;
                //            tmp = assetOutNameSourceName + " " + idTime.Key + " : " + idTime.Value;
                //            Log(tmp, MessageType.Debug);
                //            //logMethod(tmp);
                //        }
                //    }
                //}

                //// print out the processed data for debug
                //Dictionary<string, Dictionary<string, IRowData>>.KeyCollection keyColl = _systemMapper.AssetIDandRowDataByOutName.Keys;
                //string temp2 = "keys in processed data:\n";
                //foreach (KeyValuePair<string, Dictionary<string, IRowData>> pair in _systemMapper.AssetIDandRowDataByOutName)
                //{
                //    temp2 += pair.Key + ":";
                //    foreach (string key in pair.Value.Keys)
                //    {

                //        temp2 += key + ",";
                //    }
                //    temp2 += "\n";

                //}


                //Log(temp2, MessageType.Debug);
                ////logMethod(temp2);
                //*/

                //    Console.WriteLine(string.Format("Record Count for Class {0} :: {1} ", trackedCls.SourceClass, _systemMapper.AssetIDandRowDataByOutName.Count));
                //    Log(string.Format("Record Count for Class {0} :: {1} ", trackedCls.SourceClass, _systemMapper.AssetIDandRowDataByOutName.Count), MessageType.Info);


                //    if (_systemMapper.AssetIDandRowDataByOutName.Count > 0)
                //    {
                //        if (_systemMapper.AssetIDandRowDataByOutName.Values.Count == 0)
                //        {
                //            _systemMapper.AssetIDandRowDataByOutName.Clear();
                //            break;
                //        }

                //        Log(string.Format("Storing tracked class {0} to temporary asset table", trackedCls.SourceClass), MessageType.Info);
                //        //Store changes in temp table after every tracked class is processed
                //        using (AssetProcessor assetProcessor = new AssetProcessor())
                //        {
                //            assetProcessor.BeginTransaction();


                //            if (!StoreTempAssetRows(assetProcessor, false))
                //            {
                //                _systemMapper.AssetIDandRowDataByOutName.Clear();
                //                assetProcessor.Rollback();
                //                //successfulRun = false;
                //                Log("Errors found while storing data in stagging table for FC:: " + trackedCls.QualifiedSourceClass + " in " + MethodInfo.GetCurrentMethod().Name, MessageType.Error);
                //                //Log("Errors found while storing data in stagging table for Session: { 0}. Aborting Post Operation" + MethodInfo.GetCurrentMethod().Name, MessageType.Error);
                //                //throw new Exception(string.Format("Errors found while storing data in stagging table for Session: {0}. Aborting Post Operation", _editVersion.VersionName));
                //            }

                //            assetProcessor.Commit();
                //            //Clearing the dictionary because all the records has been stored into the temp table. Not sending the dictionary values across the action handlers to reduce memory load.
                //            _systemMapper.AssetIDandRowDataByOutName.Clear();
                //            Log("Test Records Added for ::"+ trackedCls.SourceClass, MessageType.Info);
                //        }
                //    }

                //    Log("Test 1 Records Added for ::" + trackedCls.SourceClass, MessageType.Info);
                //}
                #endregion Commented for Batch Approach

                Log("Test3 Records Processed ::", MessageType.Info);

                // write out failed fields/objects/classes
                Dictionary<string, Dictionary<int, List<string>>> objIdAndFailedFieldsBySourceName = _systemMapper.ObjectIDandFailedFieldsBySourceName;

                if (objIdAndFailedFieldsBySourceName.Count > 0 || _errors.Count > 0)
                {
                    //successfulRun = false;
                    Log("##########Errors in posting Version " + _editVersion.VersionName + "#############", MessageType.Error);
                    //logMethod(("##########Errors in posting Version " + _editVersion.VersionName + "#############"));
                }

                //Need to log  error description-v1t8
                if (objIdAndFailedFieldsBySourceName.Count > 0)
                {
                    StringBuilder failureDetails = new StringBuilder();
                    failureDetails.AppendLine("Version " + _editVersion.VersionName);
                    foreach (KeyValuePair<string, Dictionary<int, List<string>>> classVal in objIdAndFailedFieldsBySourceName)
                    {
                        //_logger.Debug("Failed rows and fields of class " + classVal.Key);
                        Log("Failed rows and fields of class " + classVal.Key, MessageType.Info);
                        failureDetails.AppendLine("Failed rows and fields of class " + classVal.Key);
                        foreach (KeyValuePair<int, List<string>> objIdVal in classVal.Value)
                        {
                            failureDetails.AppendLine("Row " + objIdVal.Key);
                            foreach (string failedField in objIdVal.Value)
                            {
                                failureDetails.AppendLine(failedField);
                            }
                        }
                    }

                    Log(failureDetails.ToString(), MessageType.Error);
                    remark = failureDetails.ToString();
                    //logMethod(failureDetails.ToString());
                }
                if (_errors.Count > 0)
                {
                    foreach (Exception e in _errors)
                    {
                        Log(e.Message + Environment.NewLine + e.StackTrace, MessageType.Error);
                        //_logger.Error(e.Message + Environment.NewLine + e.StackTrace);
                    }
                }
                stopWatch.Stop();
                //If the code enters here the post should be stopped. Because there were some data issues/configuration issues which were logged in the previous statement and the data cannot be sent to SAP - Ravi J
                if (objIdAndFailedFieldsBySourceName.Count > 0 || _errors.Count > 0)
                {
                    _errors.Clear();
                    objIdAndFailedFieldsBySourceName.Clear();
                    Log("######################################################################", MessageType.Error);
                    //logMethod("######################################################################");
                    Log("Total time within GISSAPIntegrator: " + stopWatch.Elapsed.ToString() + " for version " + _editVersion.VersionName, MessageType.Debug);

                    if (_systemMapper.AssetIDandRowDataByOutName != null)
                        _systemMapper.AssetIDandRowDataByOutName.Clear();
                    remark = ("Errors found while processing Records :: ") + remark;
                    //throw new Exception("Errors found while posting the session.Aborting Post Operation");
                }

                Log("Successfully processed all records in GISSAPIntegrator", MessageType.Info);

                Console.WriteLine("Successfully processed all records in GISSAPIntegrator");
                Log("Total time within GISSAPIntegrator: " + stopWatch.Elapsed.ToString() + " for version " + _editVersion.VersionName, MessageType.Debug);

                Log("Total processing time for version " + _editVersion.VersionName + ": " + stopWatch.Elapsed.ToString(), MessageType.Info);
            }
            catch (Exception ex)
            {
                successfulRun = false;
                remark = "Failed to processed all records in GISSAPIntegrator " + ex.Message;
                //(V3SF) - ED06 Improved Logging
                _logger.Error(remark);
                
            }
            //return _systemMapper.AssetIDandRowDataByOutName;
            return successfulRun;
        }

        /// <summary>
        /// (V3SF) - ED06 Improved Logging
        /// Get Current DateTime from Database
        /// </summary>
        /// <param name="bkp_Schema_Credentials"></param>
        /// <returns></returns>
        private DateTime GetCurrentTime(string bkp_Schema_Credentials)
        {
            DateTime date = default;
            try
            {
                
                using (OracleConnection con = new OracleConnection(bkp_Schema_Credentials))
                {
                    con.Open();
                    OracleCommand cmd = con.CreateCommand();
                    OracleDataReader oracleDataReader = default;
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = "select to_char(sysdate,'MM-DD-YYYY HH24:MI:SS') from dual";
                    oracleDataReader = cmd.ExecuteReader();

                    if (oracleDataReader.HasRows)
                    {
                        while (oracleDataReader.Read())
                        {
                            if (!DateTime.TryParseExact(Convert.ToString(oracleDataReader[0]), "MM-dd-yyyy HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture, DateTimeStyles.None, out date))
                                date = DateTime.Now;
                        }
                    }
                }
                Console.WriteLine("Reset Sequence Sucessful");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Reset Sequence failed with Error: " + ex.Message);
                _logger.Error("Exception in" + MethodInfo.GetCurrentMethod().Name + ex.StackTrace, ex);
                throw ex;
            }

            return date;

        }

        private void ProcessCDChanges(CDBatchManager cDBatchManager,IList<string> TargetFCs, DateTime lastRunDate, DateTime cDRunDate, string batch,string TableName)
        {
            CDTempVersionManager cDVersionManager = new CDTempVersionManager(_editVersion as IWorkspace, TableName, Bkp_Schema_Credentials);
            //IDictionary<string, ChangedFeatures> cdVersionDifference = cDVersionManager.CDVersionDifference(changeTypes.All, listType.FeatureClass, fromDate: new DateTime(2020, 8, 1, 5, 10, 20), toDate: DateTime.Now, withReplacement: false, whereClause: "(USAGE IS NULL OR USAGE NOT LIKE 'NOED06')", TargetFCs: TargetFCs);
            IDictionary<string, ChangedFeatures> cdVersionDifference = cDVersionManager.CDVersionDifference(batch, changeTypes.All, listType.FeatureClass, fromDate: lastRunDate, toDate: CDRunDate, withReplacement: false, whereClause: CDWhereClause, TargetFCs: TargetFCs);
            //V3SF - CD API (EDGISREARC-1452) - Added [END]
            DifferenceManager_CDAPI diffMan = new DifferenceManager_CDAPI(_targetVersion, _editVersion, cdVersionDifference);
            EsriGeometry.IGeometryBag childrenMaintenancePolygons = null;
            try
            {
                foreach (TrackedClass trackedCls in _systemMapper.TrackedClasses)
                {
                    AssetOIDAndProcessTimeByClass[trackedCls.OutName + trackedCls.SourceClass] = new Dictionary<int, double>();
                    //if (trackedCls.SourceClass != "Controller")
                    //    continue;

                    //v1t8 -Entry point 1
                    //Below method is used for change detection approch 
                    Process(diffMan, trackedCls, childrenMaintenancePolygons);

                    //This method is used for reverse sysnc-no use in re arch change detection-v1t8
                    // ProcessTrackedClassAssetIDandRowData(trackedCls, childrenMaintenancePolygons);

                    /*
                // time for each row
                if (_logger.IsDebugEnabled)
                {
                    Dictionary<string, Dictionary<int, double>> assetOIDAndProcessingTimeByClass = this.AssetOIDAndProcessTimeByClass;
                    foreach (KeyValuePair<string, Dictionary<int, double>> assetOIDTimeOneClass in assetOIDAndProcessingTimeByClass)
                    {
                        string assetOutNameSourceName = assetOIDTimeOneClass.Key;
                        foreach (KeyValuePair<int, double> idTime in assetOIDTimeOneClass.Value)
                        {
                            string tmp = string.Empty;
                            tmp = assetOutNameSourceName + " " + idTime.Key + " : " + idTime.Value;
                            Log(tmp, MessageType.Debug);
                            //logMethod(tmp);
                        }
                    }
                }

                // print out the processed data for debug
                Dictionary<string, Dictionary<string, IRowData>>.KeyCollection keyColl = _systemMapper.AssetIDandRowDataByOutName.Keys;
                string temp2 = "keys in processed data:\n";
                foreach (KeyValuePair<string, Dictionary<string, IRowData>> pair in _systemMapper.AssetIDandRowDataByOutName)
                {
                    temp2 += pair.Key + ":";
                    foreach (string key in pair.Value.Keys)
                    {

                        temp2 += key + ",";
                    }
                    temp2 += "\n";

                }


                Log(temp2, MessageType.Debug);
                //logMethod(temp2);
                */

                    Console.WriteLine(string.Format("Record Count for Class {0} :: {1}", trackedCls.SourceClass, _systemMapper.AssetIDandRowDataByOutName.Count));
                Log(string.Format("Record Count for Class {0} :: {1}", trackedCls.SourceClass, _systemMapper.AssetIDandRowDataByOutName.Count), MessageType.Info);


                    if (_systemMapper.AssetIDandRowDataByOutName.Count > 0)
                    {
                        if (_systemMapper.AssetIDandRowDataByOutName.Values.Count == 0)
                        {
                            _systemMapper.AssetIDandRowDataByOutName.Clear();
                            break;
                        }

                        Log(string.Format("Storing tracked class {0} to temporary asset table", trackedCls.SourceClass), MessageType.Info);
                        //Store changes in temp table after every tracked class is processed
                        using (AssetProcessor assetProcessor = new AssetProcessor())
                        {
                            assetProcessor.BeginTransaction();


                            if (!StoreTempAssetRows(assetProcessor, false))
                            {
                                _systemMapper.AssetIDandRowDataByOutName.Clear();
                                assetProcessor.Rollback();
                                //successfulRun = false;
                                Log("Errors found while storing data in stagging table for FC:: " + trackedCls.QualifiedSourceClass + " in " + MethodInfo.GetCurrentMethod().Name, MessageType.Error);
                                //Log("Errors found while storing data in stagging table for Session: { 0}. Aborting Post Operation" + MethodInfo.GetCurrentMethod().Name, MessageType.Error);
                                //throw new Exception(string.Format("Errors found while storing data in stagging table for Session: {0}. Aborting Post Operation", _editVersion.VersionName));
                            }

                            assetProcessor.Commit();
                            //Clearing the dictionary because all the records has been stored into the temp table. Not sending the dictionary values across the action handlers to reduce memory load.
                            _systemMapper.AssetIDandRowDataByOutName.Clear();
                            Log("Test Records Added for ::" + trackedCls.SourceClass, MessageType.Info);
                        }
                    }

                    Log("Test 1 Records Added for ::" + trackedCls.SourceClass, MessageType.Info);

                    //cDBatchManager.UpdateProcessFlag(TableName, batch, "P", trackedCls.QualifiedSourceClass.ToUpper());
                }
            }
            finally
            {
                #region Clean UP
                if (cdVersionDifference != null)
                {
                    foreach (string FCName in cdVersionDifference.Keys)
                    {

                        foreach (var ins in cdVersionDifference[FCName].Action.Insert)
                        {
                            ins.Dispose();
                        }
                        foreach (var ins in cdVersionDifference[FCName].Action.Update)
                        {
                            ins.Dispose();
                        }
                        foreach (var ins in cdVersionDifference[FCName].Action.Delete)
                        {
                            ins.Dispose();
                        }
                        ReleaseComReference(cdVersionDifference[FCName].FeatClass);
                    }

                    cdVersionDifference = null;
                }
                #endregion Clean UP
            }
        }

        private void ReleaseComReference(object obj)
        {
            try
            {
                if (obj != null)
                {
                    System.Runtime.InteropServices.Marshal.FinalReleaseComObject(obj);
                    obj = null;
                }
            }
            catch (Exception ex)
            {

            }
        }

        private bool DeleteTempAssetRows(AssetProcessor assetProcessor)
        {
            bool success = false;
            try
            {
                assetProcessor.DeleteTempData(_editVersion.VersionName);
                success = true;
            }
            catch (Exception ex)
            {
                Log(string.Format("Failed to delete temporary asset rows for Session: {0}. Error: {1}", _editVersion.VersionName, ex.Message), MessageType.Error);
                success = false;
            }
            return success;
        }

        /// <summary>
        /// /This method is used to stored data in temp table-v1t8
        /// </summary>
        /// <param name="assetProcessor"></param>
        /// <param name="deleteTempData"></param>
        /// <returns></returns>
        private bool StoreTempAssetRows(AssetProcessor assetProcessor, bool deleteTempData)
        {
            bool success = false;
            try
            {
                //v1t8 need to modify-entry point 2
                success = assetProcessor.ProcessTempData(_systemMapper.AssetIDandRowDataByOutName, _editVersion.VersionName, deleteTempData);
                //foreach (KeyValuePair<string, Dictionary<string, IRowData>> classAssetIDandData in _systemMapper.AssetIDandRowDataByOutName)
                //{
                //    string assetClass = classAssetIDandData.Key;
                //    foreach (KeyValuePair<string, IRowData> asset in classAssetIDandData.Value)
                //    {
                //        success = assetProcessor.ProcessTempData(asset, _editVersion.VersionName);

                //        if (!success) break;
                //    }

                //    if (!success) break;
                //}
            }
            catch (Oracle.DataAccess.Client.OracleException oex) { GISSAPIntegrator._logger.Error(oex.GetType() + " || " + oex.Number + " || " + oex.Message + " || \n" + oex.StackTrace); if (GISSAPIntegrator.HandleOracleErrorCodes || GISSAPIntegrator.OracleErrorCodes.Contains(Convert.ToString(oex.ErrorCode))) { throw oex; } }
            catch (Exception ex)
            {
                Log(string.Format("Store Temp asset rows for Session: {0}. Error: {1}", _editVersion.VersionName, ex.Message) + MethodInfo.GetCurrentMethod().Name, MessageType.Error);
                success = false;
            }

            return success;
        }
        /// <summary>
        /// This method is used for reverse synch
        /// </summary>
        /// <param name="trackedCls"></param>
        /// <param name="childrenMaintenancePolygons"></param>
        private void ProcessTrackedClassAssetIDandRowData(TrackedClass trackedCls, EsriGeometry.IGeometryBag childrenMaintenancePolygons)
        {
            ReverseSyncedClass reverseSyncedClass = _systemMapper.ReverseSyncedClasses.Find(rc => (rc.OutName == trackedCls.OutName) && (rc.SourceClass == trackedCls.SourceClass));

            if (trackedCls.SourceClass == "MaintenancePlat" && _systemMapper.AssetIDandRowDataByOutName.ContainsKey(trackedCls.OutName))
            {
                Dictionary<string, IRowData> assetIDandRowData = _systemMapper.AssetIDandRowDataByOutName[trackedCls.OutName];
                IEnumerable<KeyValuePair<string, IRowData>> inserts = assetIDandRowData.TakeWhile(asset => ((ISAPRowData)asset.Value).ActionType == ActionType.Insert);

                // create GeometryBag of insert MaintenancePolygon
                if (inserts.Count<KeyValuePair<string, IRowData>>() > 0)
                {
                    List<string> insertIDs = new List<string>();
                    foreach (KeyValuePair<string, IRowData> pair in inserts)
                    {
                        insertIDs.Add(pair.Key);
                    }

                    childrenMaintenancePolygons = BuildFilterGeometryBag(trackedCls.QualifiedSourceClass, trackedCls.Settings["AssetIDFieldName"], insertIDs);
                }
            }
            else if (trackedCls.SourceClass == "Controller" && trackedCls.OutName == "Controller")
            {
                //if (!_systemMapper.AssetIDandRowDataByOutName.ContainsKey(trackedCls.OutName))
                //    continue;

                if (_systemMapper.AssetIDandRowDataByOutName.ContainsKey(trackedCls.OutName))
                {

                    Dictionary<string, IRowData> assetIDandRowData = _systemMapper.AssetIDandRowDataByOutName[trackedCls.OutName];

                    //if (assetIDandRowData.Count == 0)
                    //    continue;

                    if (assetIDandRowData.Count > 0)
                    {
                        SettingDataHelper dataHelper = new SettingDataHelper();
                        try
                        {
                            // build dictionatory for fieldname to index for controller
                            Dictionary<string, int> fieldMap = getMapping(trackedCls.OutName, trackedCls.SourceClass, trackedCls.Subtypes);

                            int parentIDIndex = fieldMap["ParentID"];
                            foreach (KeyValuePair<string, IRowData> pair in assetIDandRowData)
                            {
                                if (pair.Value.FieldValues.ContainsKey(parentIDIndex) && pair.Value.FieldValues.ContainsKey(99))
                                {
                                    if (pair.Value.FieldValues[parentIDIndex] != null && pair.Value.FieldValues[parentIDIndex].Trim().Length > 0)
                                    {
                                        Log(string.Format("Settings: SubType:{0} Parent ID:{1} ControllerID:{2} Date:{3} Version:{4}", pair.Value.FieldValues[99], pair.Value.FieldValues[parentIDIndex], pair.Key, DateTime.Now, _editVersion.VersionName), MessageType.Debug);

                                        var settingsValues = dataHelper.GetDataForSAP(pair.Value.FieldValues[parentIDIndex], "PGEDATA_SM_SCADA_EAD");

                                        if (settingsValues.ContainsKey("SettingsRadioManufacturer"))
                                            pair.Value.FieldValues[fieldMap["SettingsRadioManufacturer"]] = settingsValues["SettingsRadioManufacturer"];

                                        if (settingsValues.ContainsKey("SettingsRadioModelNumber"))
                                            pair.Value.FieldValues[fieldMap["SettingsRadioModelNumber"]] = settingsValues["SettingsRadioModelNumber"];

                                        if (settingsValues.ContainsKey("SettingsRadioSerialNumber"))
                                            pair.Value.FieldValues[fieldMap["SettingsRadioSerialNumber"]] = settingsValues["SettingsRadioSerialNumber"];

                                        if (settingsValues.ContainsKey("SettingsNotes"))
                                            pair.Value.FieldValues[fieldMap["SettingsNotes"]] = settingsValues["SettingsNotes"];

                                        // replace the values that came from GIS with settings application values
                                        //if (settingsValues.ContainsKey("ControllerType"))
                                        //    pair.Value.FieldValues[fieldMap["ControllerType"]] = settingsValues["ControllerType"];

                                        if (settingsValues.ContainsKey("ControllerSerialNumber"))
                                            pair.Value.FieldValues[fieldMap["ControllerSerialNumber"]] = settingsValues["ControllerSerialNumber"];


                                        //Reverse Sync
                                        if (reverseSyncedClass != null)
                                        {
                                            var reverseSyncValues = dataHelper.GetDataForReverseSynch(pair.Value.FieldValues[parentIDIndex], reverseSyncedClass);
                                            if (reverseSyncValues.Count > 0)
                                            {
                                                foreach (ReverseSyncedField fld in reverseSyncedClass.ReverseSyncedFields)
                                                {
                                                    if (fieldMap.ContainsKey(fld.OutName))
                                                        pair.Value.FieldValues[fieldMap[fld.OutName]] = reverseSyncValues[fld.OutName];
                                                }
                                            }
                                        }

                                    }
                                    Log(string.Format("Settings: SubType:{0} Parent ID:{1} ControllerID:{2} Date:{3} Version:{4}", pair.Value.FieldValues[99], pair.Value.FieldValues[parentIDIndex], pair.Key, DateTime.Now, _editVersion.VersionName), MessageType.Debug);
                                    // remove the subtype field
                                    pair.Value.FieldValues.Remove(99);
                                }
                            }
                        }
                        finally
                        {
                            dataHelper.Dispose();
                        }

                    }
                }
            }
            else if (reverseSyncedClass != null) //Reverse Sync
            {
                if (_systemMapper.AssetIDandRowDataByOutName.Count > 0 && _systemMapper.AssetIDandRowDataByOutName.ContainsKey(trackedCls.OutName))
                {
                    // build dictionatory for fieldname to index for the class
                    Dictionary<string, int> fieldMap = getMapping(trackedCls.OutName, trackedCls.SourceClass, trackedCls.Subtypes);
                    if (fieldMap.ContainsKey(_GUID))
                    {
                        Dictionary<string, IRowData> assetIDandRowData = _systemMapper.AssetIDandRowDataByOutName[trackedCls.OutName];

                        SettingDataHelper dataHelper = new SettingDataHelper();
                        int guidIndex = fieldMap[_GUID];
                        foreach (KeyValuePair<string, IRowData> pair in assetIDandRowData)
                        {
                            var reverseSyncValues = dataHelper.GetDataForReverseSynch(pair.Value.FieldValues[guidIndex], reverseSyncedClass);
                            if (reverseSyncValues.Count > 0)
                            {
                                foreach (ReverseSyncedField fld in reverseSyncedClass.ReverseSyncedFields)
                                {
                                    if (fieldMap.ContainsKey(fld.OutName))
                                        pair.Value.FieldValues[fieldMap[fld.OutName]] = reverseSyncValues[fld.OutName];
                                }
                            }
                        }
                    }
                }

            }
        }



        //private string getSettingsTableName(string subTypeCD)
        //{
        //    string retVal = "";
        //    switch (subTypeCD)
        //    {
        //        case "1":
        //            retVal = "SM_Capacitor";
        //            break;
        //        case "2":
        //            retVal = "SM_Regulator";
        //            break;
        //        case "3":
        //            retVal = "SM_Recloser";
        //            break;
        //        case "4":
        //            retVal = "SM_Interrupter";
        //            break;
        //        case "5":
        //            retVal = "SM_Sectionalizer";
        //            break;
        //        case "6":
        //            retVal = "SM_Switch";
        //            break;
        //    }
        //    return retVal;
        //}

        private Dictionary<string, int> getMapping(string className, string sourceClass, string subType)
        {
            Dictionary<string, int> retVal = new Dictionary<string, int>();
            for (int x = 0; x < _systemMapper.TrackedClasses.Count; x++)
            {
                if (_systemMapper.TrackedClasses[x].OutName == className
                    && _systemMapper.TrackedClasses[x].SourceClass == sourceClass
                    && _systemMapper.TrackedClasses[x].Subtypes == subType)
                {
                    foreach (MappedField mf in _systemMapper.TrackedClasses[x].Fields)
                    {
                        retVal.Add(mf.OutName, mf.Sequence);
                    }
                }
            }
            return retVal;
        }
        /// <summary>
        /// Get a list of errors that occurred during processing
        /// </summary>
        /// <returns>A list of errors</returns>
        public List<Exception> ErrorMessages()
        {
            return _errors;
        }
        /// <summary>
        /// Event fired when the GISSAPIntegrator is initialized
        /// </summary>
        public event EventHandler OnInitialize;
        #endregion

        #endregion

        #region Private Methods

        #region Get ArcSDE Connection
        private static IWorkspace ArcSdeWorkspaceFromFile(String connection)
        {
            string connFile = ReadEncryption.GetSDEPath(connection.ToUpper());
            return ((IWorkspaceFactory)Activator.CreateInstance(Type.GetTypeFromProgID("esriDataSourcesGDB.SdeWorkspaceFactory"))).
                OpenFromFile(connFile, 0);
        }
        #endregion

        private enum MessageType
        {
            Debug,
            Info,
            Warn,
            Error,
            Fatal
        }

        private void Log(string message, MessageType messageType)
        {
            if (messageType == MessageType.Debug && _logger.IsDebugEnabled)
            {
                if (OtherLogMethod != null) { OtherLogMethod(message); }
                else { _logger.Debug(message); }
            }
            else if (messageType == MessageType.Info && _logger.IsInfoEnabled)
            {
                if (OtherLogMethod != null) { OtherLogMethod(message); }
                else { _logger.Info(message); }
            }
            else if (messageType == MessageType.Warn && _logger.IsWarnEnabled)
            {
                if (OtherLogMethod != null) { OtherLogMethod(message); }
                else { _logger.Warn(message); }
            }
            else if (messageType == MessageType.Error && _logger.IsErrorEnabled)
            {
                if (OtherLogMethod != null) { OtherLogMethod(message); }
                else { _logger.Error(message); }
            }
            else if (messageType == MessageType.Fatal && _logger.IsFatalEnabled)
            {
                if (OtherLogMethod != null) { OtherLogMethod(message); }
                else { _logger.Fatal(message); }
            }
        }

        private bool LoadConfiguration()
        {
            Log("Starting LoadConfiguration in GISSAPIntegrator at: " + DateTime.Now, MessageType.Info);
            bool retVal = false;
            string sapConfigFile = "";
            ExeConfigurationFileMap fileMap;

            //Get the Install folder from Registry
            //the install file location for all customizations will be stored in the Registry under Software\Miner and Miner\PGE under string subkey "Directory"
            // All the config files will be under the Config folder.
            // SystemRegistry sysRegistry = new SystemRegistry("PGESAP"); 
            //string PGEFolder = sysRegistry.Directory;
            //string installationFolder = sysRegistry.GetSetting<string>("SAPDirectory", PGEFolder);

            // string installationFolder = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            //string assemblyLocation = Assembly.GetExecutingAssembly().Location;
            //installationFolder = Path.GetDirectoryName(assemblyLocation);
            //string configPath = Path.Combine(installationFolder, "Config");

            try
            {
                //read config file
                //sapConfigFile = @"SAP Asset Synch\\PGE.Interfaces.SAP.dll.config";   
                // ME Q3 change : Config file to be read from installation directory not from system registry
                //  sapConfigFile = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\SAP Asset Synch\\PGE.Interfaces.SAP.dll.config"; //Path.Combine(installationFolder, "SAP Asset Synch\\PGE.Interfaces.SAP.dll.config");
                sapConfigFile = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\PGE.Interfaces.SAP.dll.config"; //Path.Combine(installationFolder, "SAP Asset Synch\\PGE.Interfaces.SAP.dll.config");
                Log("Config file path in PGE.Interfaces.SAP.dll =: " + sapConfigFile, MessageType.Info);
                Console.WriteLine("Config file path in PGE.Interfaces.SAP.dll =: " + sapConfigFile);
                fileMap = new ExeConfigurationFileMap(); //Path to your config file
                fileMap.ExeConfigFilename = sapConfigFile;
                _config = ConfigurationManager.OpenMappedExeConfiguration(fileMap, ConfigurationUserLevel.None);
            }
            catch (Oracle.DataAccess.Client.OracleException oex) { GISSAPIntegrator._logger.Error(oex.GetType() + " || " + oex.Number + " || " + oex.Message + " || \n" + oex.StackTrace); throw oex; }
            catch (Exception ex)
            {
                Log(ex.Message, MessageType.Error);
                string msg = "Failed to read " + sapConfigFile + ": " + ex.Message;
                throw new Exception(msg, ex);
            }

            OracleConnection conn = null;
            try
            {
                //first thing to check if the DB connection is correct although this is not needed until action hanlder is ready to persist.

                // m4jf edgisrearch 919
                //string connectionstring = _config.AppSettings.Settings["connectionString"].Value;
                string connval = _config.AppSettings.Settings["EDER_ConnectionStr"].Value.ToUpper();
                string connectionstring = ReadEncryption.GetConnectionStr(connval);

                //Log(string.Format("DB Connectionstring in {0} is :{1}", sapConfigFile, connectionstring), MessageType.Info);
                Log(string.Format("DB Connectionstring in {0} is :{1}", sapConfigFile, connval), MessageType.Info);

                conn = new OracleConnection(connectionstring);
                conn.Open();
            }
            catch (OracleException ex)
            {
                Log(ex.Message, MessageType.Error);
                string msg = "Failed to connect to persisting table DB:" + ex.Message;
                throw new Exception(msg, ex);
            }
            catch (Exception ex)
            {
                Log(ex.Message, MessageType.Error);
                throw ex;
            }
            finally
            {
                if (conn != null)
                    conn.Close();

            }
            // ME Q3 change : Config file to be read from installation directory not from system registry
            //string configPath = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\SAP Asset Synch\\Config";
            string configPath = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\Config";
            Log("Configuration path in GISSAPIntegrator =: " + configPath, MessageType.Info);
            try
            {
                string configFile = Path.Combine(configPath, _configFileName);
                Console.WriteLine("XML file path in =: " + configFile);
                XmlDocument document = new XmlDocument();
                document.Load(configFile);
                string configXml = document.OuterXml;

                string xsdFile = Path.Combine(configPath, _xsdFileName);
                if (XmlFacade.ValidateXML(configXml, xsdFile, false))
                {
                    _systemMapper = XmlFacade.DeserializeFromXml<SystemMapper>(document);


                    // this initialize all components
                    Log("Starting initialize systemMapper. Number of trackedcls : " + _systemMapper.TrackedClasses.Count, MessageType.Debug);


                    _systemMapper.Initialize();

                    // initialize DomainManager
                    string domainFile = Path.Combine(configPath, _domainsFileName);
                    PGE.Interfaces.Integration.Framework.Utilities.DomainManager.Instance.Initialize(domainFile, _domainsMappingName);

                    retVal = true;
                }
                else
                {
                    Log(string.Format("Failed to validate {0} against {1}", _configFileName, _xsdFileName), MessageType.Error);
                }
            }
            catch (Oracle.DataAccess.Client.OracleException oex) { GISSAPIntegrator._logger.Error(oex.GetType() + " || " + oex.Number + " || " + oex.Message + " || \n" + oex.StackTrace); if (GISSAPIntegrator.HandleOracleErrorCodes || GISSAPIntegrator.OracleErrorCodes.Contains(Convert.ToString(oex.ErrorCode))) { throw oex; } }
            catch (Exception e)
            {
                Log(e.Message, MessageType.Error);
            }


            return retVal;
        }

        public Dictionary<string, Dictionary<string, IRowData>> UpdateDataForReProcess(string objectClassName, IEnumerable<IRow> changedRows, TrackedClass trackedClass)
        {
            //_logger.Debug("objectClassName [ " + objectClassName + " ]");
            objectClassName = objectClassName.Substring(objectClassName.IndexOf(".") + 1);

            //TrackedClass trackedClass = _systemMapper.TrackedClasses.Find(tc => tc.SourceClass == objectClassName);

            if (trackedClass != null)
            {
                AssetOIDAndProcessTimeByClass[trackedClass.OutName + trackedClass.SourceClass] = new Dictionary<int, double>();
                trackedClass.ProcessedRowOIDs = new List<int>();
                string sourceName = trackedClass.QualifiedSourceClass;

                IFeatureWorkspace sourceFeatWorkspace = (IFeatureWorkspace)_editVersion;
                trackedClass.SourceTable = sourceFeatWorkspace.OpenTable(sourceName);

                IFeatureWorkspace targetFeatWorkspace = (IFeatureWorkspace)_targetVersion;
                trackedClass.TargetTable = targetFeatWorkspace.OpenTable(sourceName);

                ProcessRows(trackedClass, changedRows, ChangeType.Reprocess);

                ProcessTrackedClassAssetIDandRowData(trackedClass, null);
            }

            return _systemMapper.AssetIDandRowDataByOutName;
        }


        private EsriGeometry.IGeometryBag BuildFilterGeometryBag(string className, string assetIDFieldname, List<string> featureIDs)
        {
            EsriGeometry.IGeometryBag filterGeometry = null;
            IFeatureWorkspace sourceWorkspace = (IFeatureWorkspace)_editVersion;
            IFeatureClass maintenancePolygonFeatClass = sourceWorkspace.OpenFeatureClass(className);

            IGeoDataset geoDataset = (IGeoDataset)maintenancePolygonFeatClass;
            filterGeometry = new EsriGeometry.GeometryBagClass();
            filterGeometry.SpatialReference = geoDataset.SpatialReference;

            EsriGeometry.IGeometryCollection geomCollection = (EsriGeometry.IGeometryCollection)filterGeometry;

            IQueryFilter filter = new QueryFilterClass();
            string where = string.Empty;

            foreach (string assetID in featureIDs)
            {
                where = where + assetIDFieldname + " = '" + assetID + "' or ";
            }

            where = where.Substring(0, where.Length - 4);
            filter.WhereClause = where;

            object missing = Type.Missing;
            int mapNumberFldIndex = SAPEquipmentRowTransformer.GetFieldIndex(maintenancePolygonFeatClass, "MapNumber");
            int parentMapNumberFldIndex = SAPEquipmentRowTransformer.GetFieldIndex(maintenancePolygonFeatClass, "ParentMapNumber");

            IFeatureCursor featCur = maintenancePolygonFeatClass.Search(filter, false);
            IFeature feat = featCur.NextFeature();
            // if ParentMapNumber == MapNumber, it's not considered new child from Split, don't add to geometry bag?
            while (feat != null)
            {
                //object mapNumber = feat.get_Value(mapNumberFldIndex);
                //object parentMapNumber = feat.get_Value(parentMapNumberFldIndex);
                //if (mapNumber != null && mapNumber != DBNull.Value 
                //    && mapNumber != null && mapNumber != DBNull.Value 
                //    && object.Equals(mapNumber, parentMapNumber) == false)
                {
                    geomCollection.AddGeometry(feat.Shape, ref missing, ref missing);
                }

                feat = featCur.NextFeature();
            }

            // build spatialIndex for better performance
            EsriGeometry.ISpatialIndex spatialIndex = (EsriGeometry.ISpatialIndex)filterGeometry;
            spatialIndex.AllowIndexing = true;
            spatialIndex.Invalidate();

            Marshal.ReleaseComObject(featCur);

            return filterGeometry;
        }

        /// <summary>
        /// This method is used to find the version edits-v1t8
        /// </summary>
        /// <param name="diffMan"></param>
        /// <param name="trackedClass"></param>
        /// <param name="filterGeometry"></param>
        private void Process(DifferenceManager diffMan, TrackedClass trackedClass, EsriGeometry.IGeometry filterGeometry)
        {
            trackedClass.ProcessedRowOIDs = new List<int>();

            //string sourceName = trackedClass.SourceClass;
            string sourceName = trackedClass.QualifiedSourceClass;
            //DataHelper _dbHelper = new DataHelper();
            //  _dbHelper.GetRecordID("");
            bool hasChanges = diffMan.HasChanges(sourceName);

            if (hasChanges)
            {
                IFeatureWorkspace sourceFeatWorkspace = (IFeatureWorkspace)_editVersion;
                //  IFeatureClass featureClass = sourceFeatWorkspace.OpenFeatureClass(sourceName);
                trackedClass.SourceTable = sourceFeatWorkspace.OpenTable(sourceName);
                //V3SF - CD API (EDGISREARC-1452) - Commented
                //ITable featureClass = sourceFeatWorkspace.OpenTable(sourceName);

                //Code added to restrict ED07 User editing EDGIS ReArch Project-v1t8
                //int lastUserindex = featureClass.FindField("");
                //string lastUser= featureClass.Fields.FindField("")

                IFeatureWorkspace targetFeatWorkspace = (IFeatureWorkspace)_targetVersion;
                trackedClass.TargetTable = targetFeatWorkspace.OpenTable(sourceName);

                if (diffMan.HasChanges(sourceName, esriDataChangeType.esriDataChangeTypeInsert))
                {
                    Log("Processing inserts for " + sourceName, MessageType.Info);
                    ProcessInserts(diffMan, trackedClass);
                }

                if (diffMan.HasChanges(sourceName, esriDataChangeType.esriDataChangeTypeUpdate))
                {
                    Log("Processing updates for " + sourceName, MessageType.Info);
                    ProcessUpdates(diffMan, trackedClass);
                }

                if (diffMan.HasChanges(sourceName, esriDataChangeType.esriDataChangeTypeDelete))
                {
                    Log("Processing deletes for " + sourceName, MessageType.Info);
                    ProcessDeletes(diffMan, trackedClass);
                }
            }

            if (filterGeometry != null)
            {
                IWorkspace2 sourceWorkspace = (IWorkspace2)_editVersion;
                bool trackedClassIsFeatureClass = sourceWorkspace.get_NameExists(esriDatasetType.esriDTFeatureClass, sourceName);

                if (trackedClassIsFeatureClass)
                {
                    if (trackedClass.SourceTable == null)
                    {
                        IFeatureWorkspace sourceFeatWorkspace = (IFeatureWorkspace)_editVersion;
                        trackedClass.SourceTable = sourceFeatWorkspace.OpenTable(sourceName);
                    }

                    IEnumerable<IRow> filteredRows = Search(trackedClass, filterGeometry);
                    if (filteredRows.Count<IRow>() > 0)
                    {
                        if (trackedClass.TargetTable == null)
                        {
                            IFeatureWorkspace targetFeatWorkspace = (IFeatureWorkspace)_targetVersion;
                            trackedClass.TargetTable = targetFeatWorkspace.OpenTable(sourceName);
                        }

                        ProcessFiltered(trackedClass, filteredRows);
                    }
                }
            } //filterGeometry

            if (trackedClass.SourceTable != null)
            {
                int refCount = Marshal.ReleaseComObject(trackedClass.SourceTable);
                //while (refCount > 0)
                //{
                //    refCount = Marshal.ReleaseComObject(trackedClass.SourceTable);
                //}

                //refCount = Marshal.FinalReleaseComObject(trackedClass.SourceTable);
                trackedClass.SourceTable = null;
            }
            if (trackedClass.TargetTable != null)
            {
                int refCount = Marshal.ReleaseComObject(trackedClass.TargetTable);
                //while (refCount > 0)
                //{
                //    refCount = Marshal.ReleaseComObject(trackedClass.TargetTable);
                //}

                //refCount = Marshal.FinalReleaseComObject(trackedClass.SourceTable);
                trackedClass.TargetTable = null;
            }
        }

        /// <summary>
        /// V3SF - CD API (EDGISREARC-1452) - Commented
        /// This method is used to find the version edits-v1t8
        /// </summary>
        /// <param name="diffMan"></param>
        /// <param name="trackedClass"></param>
        /// <param name="filterGeometry"></param>
        private void Process(DifferenceManager_CDAPI diffMan, TrackedClass trackedClass, EsriGeometry.IGeometry filterGeometry)
        {
            trackedClass.ProcessedRowOIDs = new List<int>();

            //string sourceName = trackedClass.SourceClass;
            string sourceName = trackedClass.QualifiedSourceClass;
            //DataHelper _dbHelper = new DataHelper();
            //  _dbHelper.GetRecordID("");
            bool hasChanges = diffMan.HasChanges(sourceName.ToUpper());

            if (hasChanges)
            {
                IFeatureWorkspace sourceFeatWorkspace = (IFeatureWorkspace)_editVersion;
                //  IFeatureClass featureClass = sourceFeatWorkspace.OpenFeatureClass(sourceName);
                trackedClass.SourceTable = sourceFeatWorkspace.OpenTable(sourceName);
                //V3SF - CD API (EDGISREARC-1452) - Commented
                //ITable featureClass = sourceFeatWorkspace.OpenTable(sourceName);

                //Code added to restrict ED07 User editing EDGIS ReArch Project-v1t8
                //int lastUserindex = featureClass.FindField("");
                //string lastUser= featureClass.Fields.FindField("")

                IFeatureWorkspace targetFeatWorkspace = (IFeatureWorkspace)_targetVersion;
                trackedClass.TargetTable = targetFeatWorkspace.OpenTable(sourceName);

                if (diffMan.HasChanges(sourceName, esriDataChangeType.esriDataChangeTypeInsert))
                {
                    Log("Processing inserts for " + sourceName, MessageType.Info);
                    ProcessInserts(diffMan, trackedClass);
                }

                if (diffMan.HasChanges(sourceName, esriDataChangeType.esriDataChangeTypeUpdate))
                {
                    Log("Processing updates for " + sourceName, MessageType.Info);
                    ProcessUpdates(diffMan, trackedClass);
                }

                if (diffMan.HasChanges(sourceName, esriDataChangeType.esriDataChangeTypeDelete))
                {
                    Log("Processing deletes for " + sourceName, MessageType.Info);
                    ProcessDeletes(diffMan, trackedClass);
                }
            }

            if (filterGeometry != null)
            {
                IWorkspace2 sourceWorkspace = (IWorkspace2)_editVersion;
                bool trackedClassIsFeatureClass = sourceWorkspace.get_NameExists(esriDatasetType.esriDTFeatureClass, sourceName);

                if (trackedClassIsFeatureClass)
                {
                    if (trackedClass.SourceTable == null)
                    {
                        IFeatureWorkspace sourceFeatWorkspace = (IFeatureWorkspace)_editVersion;
                        trackedClass.SourceTable = sourceFeatWorkspace.OpenTable(sourceName);
                    }

                    IEnumerable<IRow> filteredRows = Search(trackedClass, filterGeometry);
                    if (filteredRows.Count<IRow>() > 0)
                    {
                        if (trackedClass.TargetTable == null)
                        {
                            IFeatureWorkspace targetFeatWorkspace = (IFeatureWorkspace)_targetVersion;
                            trackedClass.TargetTable = targetFeatWorkspace.OpenTable(sourceName);
                        }

                        ProcessFiltered(trackedClass, filteredRows);
                    }
                }
            } //filterGeometry

            if (trackedClass.SourceTable != null)
            {
                int refCount = Marshal.ReleaseComObject(trackedClass.SourceTable);
                //while (refCount > 0)
                //{
                //    refCount = Marshal.ReleaseComObject(trackedClass.SourceTable);
                //}

                //refCount = Marshal.FinalReleaseComObject(trackedClass.SourceTable);
                trackedClass.SourceTable = null;
            }
            if (trackedClass.TargetTable != null)
            {
                int refCount = Marshal.ReleaseComObject(trackedClass.TargetTable);
                //while (refCount > 0)
                //{
                //    refCount = Marshal.ReleaseComObject(trackedClass.TargetTable);
                //}

                //refCount = Marshal.FinalReleaseComObject(trackedClass.SourceTable);
                trackedClass.TargetTable = null;
            }
        }

        //IVARA
        private IEnumerable<IRow> GetFilteredRows(IEnumerable<IRow> changedRows, TrackedClass trackedClass)
        {
            List<IRow> filteredRows = new List<IRow>();
            string equalOp = "=";
            string notEqualOp = "!=";
            //List<IRow> rows = filteredRows as List<IRow>;

            if (!string.IsNullOrEmpty(trackedClass.SingleFieldWhereClauseCondition))
            {
                if (trackedClass.SingleFieldWhereClauseCondition.Contains(notEqualOp))
                {
                    string[] notEqualNameValue = Regex.Split(trackedClass.SingleFieldWhereClauseCondition, "!=");

                    foreach (IRow row in changedRows)
                    {
                        if (!string.IsNullOrEmpty(row.get_Value(row.Fields.FindField(notEqualNameValue[0].Trim())).ToString()))
                        {
                            if ((row.get_Value(row.Fields.FindField(notEqualNameValue[0].Trim())).ToString()) != notEqualNameValue[1].Trim())
                            {
                                filteredRows.Add(row);
                            }
                        }
                    }
                }
                else if (trackedClass.SingleFieldWhereClauseCondition.Contains(equalOp))
                {
                    string[] equalNameValue = Regex.Split(trackedClass.SingleFieldWhereClauseCondition, "=");
                    //string[] notEqualNameValue = Regex.Split(trackedClass.SingleFieldWhereClauseCondition, "!=");

                    foreach (IRow row in changedRows)
                    {
                        if (!string.IsNullOrEmpty(row.get_Value(row.Fields.FindField(equalNameValue[0].Trim())).ToString()))
                        {
                            if ((row.get_Value(row.Fields.FindField(equalNameValue[0].Trim())).ToString()) == equalNameValue[1].Trim())
                            {
                                filteredRows.Add(row);
                            }
                        }
                    }

                }


                //string[] equalNameValue = Regex.Split(trackedClass.SingleFieldWhereClauseCondition, "=");
                //string[] notEqualNameValue = Regex.Split(trackedClass.SingleFieldWhereClauseCondition, "!=");

                //if (equalNameValue.Length > 0)
                //    filteredRows = changedRows.Select(row => row.get_Value(row.Fields.FindField(equalNameValue[0].Trim())) == equalNameValue[1].Trim() ? row : null);
                //else if (notEqualNameValue.Length > 0)
                //    filteredRows = changedRows.Select(row => row.get_Value(row.Fields.FindField(notEqualNameValue[0].Trim())) != notEqualNameValue[1].Trim() ? row : null);
            }
            else
            {
                return changedRows;
            }

            return filteredRows as IEnumerable<IRow>;
        }

        private EnumerableCursor2 GetChangesRowsCursor(DifferenceManager diffMan, TrackedClass trackedClass, ITable sourceTable, ITable targetTable, esriDifferenceType differenceType)
        {
            List<string> subtypes = trackedClass.RowTransformer.Subtypes;
            string whereClause = "";

            if (subtypes.Count > 0)
            {
                foreach (string subtype in subtypes)
                {
                    if (!string.IsNullOrEmpty(whereClause)) { whereClause += "," + subtype; }
                    else { whereClause = subtype; }
                }
                whereClause = string.Format("SUBTYPECD IN ({0})", whereClause);
            }
            if (!string.IsNullOrEmpty(trackedClass.SingleFieldWhereClauseCondition))
            {
                if (string.IsNullOrEmpty(whereClause)) { whereClause = trackedClass.SingleFieldWhereClauseCondition; }
                else { whereClause += " AND " + trackedClass.SingleFieldWhereClauseCondition; }
            }
            else if (trackedClass.Subtypes == "3" && trackedClass.SourceClass == "SupportStructure")
            {
                whereClause += " AND (POLEUSE=4 OR POLEUSE=5) AND (SAPEQUIPID='9999' OR SAPEQUIPID is null) AND CUSTOMEROWNED='N' AND (STATUS = 5 OR STATUS=30)";
            }

            //Passing where clause in this method does not make sense it is not used inside GetChangedRowOIDs method. this method will return all the oid irrspective of SAP mapping.  
            List<int> rowOIDsToObtain = GetChangedRowOIDs(false, trackedClass.SourceTable, trackedClass.TargetTable, whereClause, differenceType);
            nonSAPRecordList = rowOIDsToObtain;
            IQueryFilter qf = new QueryFilterClass();
            qf.WhereClause = whereClause;

            EnumerableCursor2 cursor = new EnumerableCursor2(sourceTable, qf, rowOIDsToObtain, targetTable);
            return cursor;
        }

        /// <summary>
        /// V3SF - CD API (EDGISREARC-1452) - Commented
        /// </summary>
        /// <param name="diffMan"></param>
        /// <param name="trackedClass"></param>
        /// <param name="sourceTable"></param>
        /// <param name="targetTable"></param>
        /// <param name="differenceType"></param>
        /// <returns></returns>
        private EnumerableCursor2 GetChangesRowsCursor(DifferenceManager_CDAPI diffMan, TrackedClass trackedClass, ITable sourceTable, ITable targetTable, esriDifferenceType differenceType)
        {
            List<string> subtypes = trackedClass.RowTransformer.Subtypes;
            string whereClause = "";

            if (subtypes.Count > 0)
            {
                foreach (string subtype in subtypes)
                {
                    if (!string.IsNullOrEmpty(whereClause)) { whereClause += "," + subtype; }
                    else { whereClause = subtype; }
                }
                whereClause = string.Format("SUBTYPECD IN ({0})", whereClause);
            }
            if (!string.IsNullOrEmpty(trackedClass.SingleFieldWhereClauseCondition))
            {
                if (string.IsNullOrEmpty(whereClause)) { whereClause = trackedClass.SingleFieldWhereClauseCondition; }
                else { whereClause += " AND " + trackedClass.SingleFieldWhereClauseCondition; }
            }
            else if (trackedClass.Subtypes == "3" && trackedClass.SourceClass == "SupportStructure")
            {
                whereClause += " AND (POLEUSE=4 OR POLEUSE=5) AND (SAPEQUIPID='9999' OR SAPEQUIPID is null) AND CUSTOMEROWNED='N' AND (STATUS = 5 OR STATUS=30)";
            }

            //Passing where clause in this method does not make sense it is not used inside GetChangedRowOIDs method. this method will return all the oid irrspective of SAP mapping.  
            List<int> rowOIDsToObtain = diffMan.GetChangedRowOIDs(trackedClass.QualifiedSourceClass.ToUpper(), differenceType);

            nonSAPRecordList = rowOIDsToObtain;
            IQueryFilter qf = new QueryFilterClass();
            qf.WhereClause = whereClause;
            //Add log to print OID for a given Tracked Class with Action Type
            _logger.Info("Processing "+ differenceType + " of Tracked Class:: " + trackedClass.QualifiedSourceClass + " of SubType:: " + String.Join(", ", subtypes.ToArray()) + " and where Clause :: " + trackedClass.SingleFieldWhereClauseCondition);
            _logger.Info("OID(s) :: " + string.Join(", ", rowOIDsToObtain));
            EnumerableCursor2 cursor = new EnumerableCursor2(sourceTable, qf, rowOIDsToObtain, targetTable,diffMan._cdVersionDifference);
            return cursor;
        }

        private IList<DeleteFeat> GetChangesRowsCursor(DifferenceManager_CDAPI diffMan, TrackedClass trackedClass, esriDifferenceType differenceType)
        {
            IList<DeleteFeat> deleteFeats = new List<DeleteFeat>();
            List<int> OIDs = new List<int>();
            List<string> subtypes = trackedClass.RowTransformer.Subtypes;
            string whereClause = "";
            string[] splitString = default;

            foreach (DeleteFeat deleteFeat in diffMan._cdVersionDifference[trackedClass.QualifiedSourceClass.ToUpper()].Action.Delete)
            {
                if (subtypes.Count > 0)
                {
                    if(deleteFeat.fields_Old.ContainsKey("SUBTYPECD".ToUpper()))
                    {
                        if(subtypes.Contains(deleteFeat.fields_Old["SUBTYPECD".ToUpper()]))
                        {
                            if (!string.IsNullOrEmpty(trackedClass.SingleFieldWhereClauseCondition))
                            {
                                splitString = trackedClass.SingleFieldWhereClauseCondition.Split(' ');
                                if (splitString.Length == 3)
                                {
                                    if (deleteFeat.fields_Old.ContainsKey(splitString[0].ToUpper()))
                                    {
                                        if (splitString[1] != "!=")
                                        {
                                            if (deleteFeat.fields_Old[splitString[0].ToUpper()] == splitString[2])
                                            {
                                                deleteFeats.Add(deleteFeat);
                                            }
                                        }
                                        else
                                        {
                                            if (deleteFeat.fields_Old[splitString[0].ToUpper()] != splitString[2])
                                            {
                                                deleteFeats.Add(deleteFeat);
                                            }
                                        }
                                    }
                                }
                            }
                            else if (trackedClass.Subtypes == "3" && trackedClass.SourceClass == "SupportStructure")
                            {
                                whereClause += " AND (POLEUSE=4 OR POLEUSE=5) AND (SAPEQUIPID='9999' OR SAPEQUIPID is null) AND CUSTOMEROWNED='N' AND (STATUS = 5 OR STATUS=30)";
                                if (deleteFeat.fields_Old.ContainsKey("POLEUSE".ToUpper()) && deleteFeat.fields_Old.ContainsKey("SAPEQUIPID".ToUpper()) && deleteFeat.fields_Old.ContainsKey("STATUS".ToUpper()))
                                {
                                    if ((deleteFeat.fields_Old["POLEUSE".ToUpper()] == "4" || deleteFeat.fields_Old["POLEUSE".ToUpper()] == "5") &&
                                       (deleteFeat.fields_Old["SAPEQUIPID".ToUpper()] == "9999" || string.IsNullOrEmpty(deleteFeat.fields_Old["SAPEQUIPID".ToUpper()])) &&
                                       (deleteFeat.fields_Old["STATUS".ToUpper()] == "5" || deleteFeat.fields_Old["STATUS".ToUpper()] == "30"))
                                    {
                                        deleteFeats.Add(deleteFeat);
                                    }
                                }
                            }
                            else
                            {
                                deleteFeats.Add(deleteFeat);
                            }
                        }
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(trackedClass.SingleFieldWhereClauseCondition))
                    {
                        splitString = trackedClass.SingleFieldWhereClauseCondition.Split(' ');
                        if (splitString.Length == 3)
                        {
                            if (deleteFeat.fields_Old.ContainsKey(splitString[0].ToUpper()))
                            {
                                if (splitString[1] != "!=")
                                {
                                    if (deleteFeat.fields_Old[splitString[0].ToUpper()] == splitString[2])
                                    {
                                        deleteFeats.Add(deleteFeat);
                                    }
                                }
                                else
                                {
                                    if (deleteFeat.fields_Old[splitString[0].ToUpper()] != splitString[2])
                                    {
                                        deleteFeats.Add(deleteFeat);
                                    }
                                }
                            }
                        }
                    }
                    else if (trackedClass.Subtypes == "3" && trackedClass.SourceClass == "SupportStructure")
                    {
                        whereClause += " AND (POLEUSE=4 OR POLEUSE=5) AND (SAPEQUIPID='9999' OR SAPEQUIPID is null) AND CUSTOMEROWNED='N' AND (STATUS = 5 OR STATUS=30)";
                        if(deleteFeat.fields_Old.ContainsKey("POLEUSE".ToUpper()) && deleteFeat.fields_Old.ContainsKey("SAPEQUIPID".ToUpper()) && deleteFeat.fields_Old.ContainsKey("STATUS".ToUpper()))
                        {
                            if((deleteFeat.fields_Old["POLEUSE".ToUpper()] == "4" || deleteFeat.fields_Old["POLEUSE".ToUpper()] == "5") &&
                               (deleteFeat.fields_Old["SAPEQUIPID".ToUpper()] == "9999" || string.IsNullOrEmpty(deleteFeat.fields_Old["SAPEQUIPID".ToUpper()])) &&
                               (deleteFeat.fields_Old["STATUS".ToUpper()] == "5" || deleteFeat.fields_Old["STATUS".ToUpper()] == "30") )
                            {
                                deleteFeats.Add(deleteFeat);
                            }
                        }
                    }
                    else
                    {
                        deleteFeats.Add(deleteFeat);
                    }
                }
            }
            
            foreach(DeleteFeat delete in deleteFeats)
            {
                OIDs.Add(delete.OID);
                delete.Table = trackedClass.SourceTable;
                delete.ChangeFeatures = diffMan._cdVersionDifference;
            }

            //Add log to print OID for a given Tracked Class with Action Type
            _logger.Info("Processing " + differenceType + " of Tracked Class:: " + trackedClass.QualifiedSourceClass + " of SubType:: " + String.Join(", ", subtypes.ToArray()) + " and where Clause :: " + trackedClass.SingleFieldWhereClauseCondition);
            _logger.Info(" OID(s) :: " + string.Join(", ", OIDs));

            return deleteFeats;
        }

        public List<int> GetChangedRowOIDs( ITable sourceTable, string whereClause)
        {
            List<int> oidsToObtain = new List<int>();

            IRow row = null;
            IQueryFilter qf = new QueryFilterClass();
            qf.WhereClause = whereClause;

            ICursor cursor = sourceTable.Search(qf, false);
            while((row = cursor.NextRow())!=null)
            {
                if(!oidsToObtain.Contains(row.OID))
                    oidsToObtain.Add(row.OID);
            }

            if (cursor != null) Marshal.FinalReleaseComObject(cursor);
            if (row != null) Marshal.FinalReleaseComObject(row);

            oidsToObtain = oidsToObtain.Distinct().ToList();
            return oidsToObtain;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sourceTable"></param>
        /// <param name="targetTable"></param>
        /// <param name="whereClause"></param>
        /// <param name="differenceTypes"></param>
        /// <returns></returns>
        public List<int> GetChangedRowOIDs(bool GetFeederManager20Updates, ITable sourceTable, ITable targetTable, string whereClause, params esriDifferenceType[] differenceTypes)
        {
            List<int> oidsToObtain = new List<int>();
            IQueryFilter2 queryFilter = null;
            if (string.IsNullOrEmpty(whereClause) == false)
            {
                queryFilter = new QueryFilterClass();
                queryFilter.WhereClause = whereClause;
            }

            IVersionedTable sourceVersionedTable = (IVersionedTable)sourceTable;

            foreach (esriDifferenceType diffType in differenceTypes)
            {
                IQueryFilter qf = new QueryFilterClass();
                qf.AddField(sourceTable.OIDFieldName);
                IDifferenceCursor diffCur = sourceVersionedTable.Differences(targetTable, diffType, qf);
                try
                {
                    int oID;
                    IRow aRow;

                    diffCur.Next(out oID, out aRow);
                    while (oID != -1)
                    {
                        if (diffType == esriDifferenceType.esriDifferenceTypeDeleteNoChange
                            || diffType == esriDifferenceType.esriDifferenceTypeDeleteUpdate)
                        {
                            //aRow = targetTable.GetRow(oID);
                            oidsToObtain.Add(oID);
                        }
                        else
                        {
                            //aRow = sourceTable.GetRow(oID);
                            oidsToObtain.Add(oID);
                        }

                        //yield return aRow;
                        if (aRow != null) { while (Marshal.ReleaseComObject(aRow) > 0) { } }
                        diffCur.Next(out oID, out aRow);

                    }
                }
                finally
                {
                    while (Marshal.ReleaseComObject(diffCur) > 0) { }
                }

                //The following section will return any rows that feeder manager has identified as being "updated".  This supports
                //any functionality that needs to know when feeder manager specific information has changed at feeder manager 2.0
                if (GetFeederManager20Updates && diffType == esriDifferenceType.esriDifferenceTypeUpdateNoChange)
                {
                    IDataset ds = sourceTable as IDataset;
                    List<int> oids = FeederManager2.GetEditedFeatures((IFeatureWorkspace)ds.Workspace, ((IVersion)ds.Workspace).VersionName, (IObjectClass)sourceTable);
                    oidsToObtain.AddRange(oids);
                }

                oidsToObtain = oidsToObtain.Distinct().ToList();

            }

            return oidsToObtain;
        }

        private void ProcessInserts(DifferenceManager diffMan, TrackedClass trackedClass)
        {
            //IEnumerable<IRow> inserts = diffMan.GetInserts(trackedClass.SourceTable, trackedClass.TargetTable, subtypes);
            //inserts = GetFilteredRows(inserts, trackedClass); //IVARA
            _logger.Info("Started excuting " + MethodInfo.GetCurrentMethod().Name);
            //if (inserts != null)
            try
            {
                EnumerableCursor2 inserts = GetChangesRowsCursor(diffMan, trackedClass, trackedClass.SourceTable, null, esriDifferenceType.esriDifferenceTypeInsert);
                if (inserts.RowCount() > 0)
                {
                    _logger.Info("Processing  Non SAP record in staging table for feature class :" + trackedClass.SourceClass);
                    ProcessRows(trackedClass, inserts, ChangeType.Insert);
                    _logger.Info("Successfully processed SAP record in for feature class :" + trackedClass.SourceClass);
                }

                #region COMMENTED CODE - below else condtion to handle missing record. Non SAP edits will also insert in staging table with error description -v1t8 
                //Added below else condtion to handle missing record. Non SAP edits will also insert in staging table with error description -v1t8
                //else
                //{
                //    if (nonSAPRecordList != null && nonSAPRecordList.Count > 0)
                //    {
                //        int count = 0;
                //        _logger.Info("Processing  Non SAP record in staging table for feature class :" + trackedClass.SourceClass);
                //        for (int i = 0; i < nonSAPRecordList.Count; i++)
                //        {
                //            ITable table = trackedClass.SourceTable;
                //            IRow row = table.GetRow(nonSAPRecordList[i]);
                //            LogError(row, trackedClass.SourceClass + "OID :" + nonSAPRecordList[i] + " GIS Edit which is not an valid SAP Record", ChangeType.Insert);
                //            count++;

                //        }
                //        _logger.Info("Successfully Inserted Non SAP record in staging table  :" + trackedClass.SourceClass + "Count :" + count);
                //    }

                //    _logger.Info("Successfully Inserted Non SAP record in staging table for feature class :" + trackedClass.SourceClass);
                //}
                //
                #endregion
                _logger.Info("Sucessfully executed " + MethodInfo.GetCurrentMethod().Name);
            }
            catch (Oracle.DataAccess.Client.OracleException oex) { GISSAPIntegrator._logger.Error(oex.GetType() + " || " + oex.Number + " || " + oex.Message + " || \n" + oex.StackTrace); if (GISSAPIntegrator.HandleOracleErrorCodes || GISSAPIntegrator.OracleErrorCodes.Contains(Convert.ToString(oex.ErrorCode))) { throw oex; } }
            catch (Exception ex)
            {
                _logger.Error("Exception occure while processing Insert records " + MethodInfo.GetCurrentMethod().Name + ex.StackTrace, ex);
            }

        }

        /// <summary>
        /// V3SF - CD API (EDGISREARC-1452) - Added
        /// </summary>
        /// <param name="diffMan"></param>
        /// <param name="trackedClass"></param>
        private void ProcessInserts(DifferenceManager_CDAPI diffMan, TrackedClass trackedClass)
        {
            //IEnumerable<IRow> inserts = diffMan.GetInserts(trackedClass.SourceTable, trackedClass.TargetTable, subtypes);
            //inserts = GetFilteredRows(inserts, trackedClass); //IVARA
            _logger.Info("Started excuting " + MethodInfo.GetCurrentMethod().Name);
            //if (inserts != null)
            try
            {
                EnumerableCursor2 inserts = GetChangesRowsCursor(diffMan, trackedClass, trackedClass.SourceTable, null, esriDifferenceType.esriDifferenceTypeInsert);
                if (inserts.RowCount() > 0)
                {
                    _logger.Info("Processing  Non SAP record in staging table for feature class :" + trackedClass.SourceClass);
                    ProcessRows(trackedClass, inserts, ChangeType.Insert);
                    _logger.Info("Successfully processed SAP record in for feature class :" + trackedClass.SourceClass);
                }

                #region COMMENTED CODE - below else condtion to handle missing record. Non SAP edits will also insert in staging table with error description -v1t8 
                //Added below else condtion to handle missing record. Non SAP edits will also insert in staging table with error description -v1t8
                //else
                //{
                //    if (nonSAPRecordList != null && nonSAPRecordList.Count > 0)
                //    {
                //        int count = 0;
                //        _logger.Info("Processing  Non SAP record in staging table for feature class :" + trackedClass.SourceClass);
                //        for (int i = 0; i < nonSAPRecordList.Count; i++)
                //        {
                //            ITable table = trackedClass.SourceTable;
                //            IRow row = table.GetRow(nonSAPRecordList[i]);
                //            LogError(row, trackedClass.SourceClass + "OID :" + nonSAPRecordList[i] + " GIS Edit which is not an valid SAP Record", ChangeType.Insert);
                //            count++;

                //        }
                //        _logger.Info("Successfully Inserted Non SAP record in staging table  :" + trackedClass.SourceClass + "Count :" + count);
                //    }

                //    _logger.Info("Successfully Inserted Non SAP record in staging table for feature class :" + trackedClass.SourceClass);
                //}
                //
                #endregion
                _logger.Info("Sucessfully executed " + MethodInfo.GetCurrentMethod().Name);
            }
            catch (OracleException oex) { _logger.Error(oex.GetType() + " || " + oex.Number + " || " + oex.Message + " || \n" + oex.StackTrace); if (HandleOracleErrorCodes || OracleErrorCodes.Contains(Convert.ToString(oex.ErrorCode))) { throw oex; } } catch (Exception ex)
            {
                _logger.Error("Exception occure while processing Insert records " + MethodInfo.GetCurrentMethod().Name + ex.StackTrace, ex);
            }

        }

        private void ProcessUpdates(DifferenceManager diffMan, TrackedClass trackedClass)
        {
            //List<string> subtypes = trackedClass.RowTransformer.Subtypes;
            //IEnumerable<IRow> updates = diffMan.GetUpdates(false, trackedClass.SourceTable, trackedClass.TargetTable, subtypes);

            //updates = GetFilteredRows(updates, trackedClass); //IVARA
            _logger.Info("Started excuting " + MethodInfo.GetCurrentMethod().Name);

            try
            {
                EnumerableCursor2 updates = GetChangesRowsCursor(diffMan, trackedClass, trackedClass.SourceTable, trackedClass.TargetTable, esriDifferenceType.esriDifferenceTypeUpdateNoChange);
                if (updates.RowCount() > 0)
                {
                    _logger.Info("Processing SAP record  for feature class :" + trackedClass.SourceClass + "ActionType :" + ChangeType.Update.ToString());
                    ProcessRows(trackedClass, updates, ChangeType.Update);

                    _logger.Info("Successfully processed SAP record in for feature class :" + trackedClass.SourceClass);
                }
                #region COMMENTED CODE - below else condtion to handle missing record. Non SAP edits will also insert in staging table with error description -v1t8 
                //Added below else condtion to handle missing record. Non SAP edits will also insert in staging table with error description -v1t8 
                //else
                //{
                //    if (nonSAPRecordList != null && nonSAPRecordList.Count > 0)
                //    {
                //        int count = 0;
                //        _logger.Info("Processing  Non SAP record in staging table for feature class :" + trackedClass.SourceClass);
                //        for (int i = 0; i < nonSAPRecordList.Count; i++)
                //        {
                //            ITable table = trackedClass.SourceTable;
                //            IRow row = table.GetRow(nonSAPRecordList[i]);
                //            LogError(row, trackedClass.SourceClass + ", OID :" + nonSAPRecordList[i] + " GIS Edit which is not an valid SAP Record", ChangeType.Update);
                //            count++;

                //        }
                //        _logger.Info("Successfully Inserted Non SAP record in staging table  :" + trackedClass.SourceClass + "Count :" + count);
                //    }

                //    _logger.Info("Successfully Inserted Non SAP record in staging table for feature class :" + trackedClass.SourceClass);
                //}
                //
                #endregion
                _logger.Info("Sucessfully executed " + MethodInfo.GetCurrentMethod().Name);
            }
            catch (Oracle.DataAccess.Client.OracleException oex) { GISSAPIntegrator._logger.Error(oex.GetType() + " || " + oex.Number + " || " + oex.Message + " || \n" + oex.StackTrace); if (GISSAPIntegrator.HandleOracleErrorCodes || GISSAPIntegrator.OracleErrorCodes.Contains(Convert.ToString(oex.ErrorCode))) { throw oex; } }
            catch (Exception ex)
            {
                _logger.Error("Exception occure while processing update records " + MethodInfo.GetCurrentMethod().Name + ex.StackTrace, ex);
            }
        }

        private void ProcessUpdates(DifferenceManager_CDAPI diffMan, TrackedClass trackedClass)
        {
            //List<string> subtypes = trackedClass.RowTransformer.Subtypes;
            //IEnumerable<IRow> updates = diffMan.GetUpdates(false, trackedClass.SourceTable, trackedClass.TargetTable, subtypes);

            //updates = GetFilteredRows(updates, trackedClass); //IVARA
            _logger.Info("Started excuting " + MethodInfo.GetCurrentMethod().Name);

            try
            {
                EnumerableCursor2 updates = GetChangesRowsCursor(diffMan, trackedClass, trackedClass.SourceTable, trackedClass.TargetTable, esriDifferenceType.esriDifferenceTypeUpdateNoChange);
                if (updates.RowCount() > 0)
                {
                    _logger.Info("Processing SAP record  for feature class :" + trackedClass.SourceClass + "ActionType :" + ChangeType.Update.ToString());
                    ProcessRows(trackedClass, updates, ChangeType.Update,diffMan);

                    _logger.Info("Successfully processed SAP record in for feature class :" + trackedClass.SourceClass);
                }
                #region COMMENTED CODE - below else condtion to handle missing record. Non SAP edits will also insert in staging table with error description -v1t8 
                //Added below else condtion to handle missing record. Non SAP edits will also insert in staging table with error description -v1t8 
                //else
                //{
                //    if (nonSAPRecordList != null && nonSAPRecordList.Count > 0)
                //    {
                //        int count = 0;
                //        _logger.Info("Processing  Non SAP record in staging table for feature class :" + trackedClass.SourceClass);
                //        for (int i = 0; i < nonSAPRecordList.Count; i++)
                //        {
                //            ITable table = trackedClass.SourceTable;
                //            IRow row = table.GetRow(nonSAPRecordList[i]);
                //            LogError(row, trackedClass.SourceClass + ", OID :" + nonSAPRecordList[i] + " GIS Edit which is not an valid SAP Record", ChangeType.Update);
                //            count++;

                //        }
                //        _logger.Info("Successfully Inserted Non SAP record in staging table  :" + trackedClass.SourceClass + "Count :" + count);
                //    }

                //    _logger.Info("Successfully Inserted Non SAP record in staging table for feature class :" + trackedClass.SourceClass);
                //}
                //
                #endregion
                _logger.Info("Sucessfully executed " + MethodInfo.GetCurrentMethod().Name);
            }
            catch (Oracle.DataAccess.Client.OracleException oex) { GISSAPIntegrator._logger.Error(oex.GetType() + " || " + oex.Number + " || " + oex.Message + " || \n" + oex.StackTrace); if (GISSAPIntegrator.HandleOracleErrorCodes || GISSAPIntegrator.OracleErrorCodes.Contains(Convert.ToString(oex.ErrorCode))) { throw oex; } }
            catch (Exception ex)
            {
                _logger.Error("Exception occure while processing update records " + MethodInfo.GetCurrentMethod().Name + ex.StackTrace, ex);
            }
        }

        private void ProcessDeletes(DifferenceManager diffMan, TrackedClass trackedClass)
        {
            //List<string> subtypes = trackedClass.RowTransformer.Subtypes;
            //IEnumerable<IRow> deletes = diffMan.GetDeletes(trackedClass.SourceTable, trackedClass.TargetTable, subtypes);
            _logger.Info("Started excuting " + MethodInfo.GetCurrentMethod().Name);
            //deletes = GetFilteredRows(deletes, trackedClass); //IVARA
            try
            {

                EnumerableCursor2 deletes = GetChangesRowsCursor(diffMan, trackedClass, trackedClass.TargetTable, null, esriDifferenceType.esriDifferenceTypeDeleteNoChange);
                if (deletes.RowCount() > 0)
                {
                    _logger.Info("Processing SAP record  for feature class :" + trackedClass.SourceClass + "ActionType :" + ChangeType.Delete.ToString());
                    ProcessRows(trackedClass, deletes, ChangeType.Delete);
                    _logger.Info("Successfully processed SAP record in for feature class :" + trackedClass.SourceClass);
                }
                #region - COMMENTED CODE - below else condtion to handle missing record. Non SAP edits will also insert in staging table with error description -v1t8 
                //Added below else condtion to handle missing record. Non SAP edits will also insert in staging table with error description -v1t8 
                //else
                //{
                //    if (nonSAPRecordList != null && nonSAPRecordList.Count > 0)
                //    {
                //        int count = 0;
                //        _logger.Info("Processing  Non SAP record in staging table for feature class :" + trackedClass.SourceClass);
                //        for (int i = 0; i < nonSAPRecordList.Count; i++)
                //        {
                //            ITable table = trackedClass.SourceTable;
                //            IRow row = table.GetRow(nonSAPRecordList[i]);
                //            LogError(row, trackedClass.SourceClass + "OID :" + nonSAPRecordList[i] + " GIS Edit which is not an valid SAP Record", ChangeType.Delete);
                //            count++;

                //        }
                //        _logger.Info("Successfully Inserted Non SAP record in staging table  :" + trackedClass.SourceClass + "Count :" + count);
                //    }

                //    _logger.Info("Successfully Inserted Non SAP record in staging table for feature class :" + trackedClass.SourceClass);
                //}
                #endregion
                _logger.Info("Sucessfully executed " + MethodInfo.GetCurrentMethod().Name);
            }
            catch (Oracle.DataAccess.Client.OracleException oex) { GISSAPIntegrator._logger.Error(oex.GetType() + " || " + oex.Number + " || " + oex.Message + " || \n" + oex.StackTrace); if (GISSAPIntegrator.HandleOracleErrorCodes || GISSAPIntegrator.OracleErrorCodes.Contains(Convert.ToString(oex.ErrorCode))) { throw oex; } }
            catch (Exception ex)
            {
                _logger.Error("Exception occure while processing delete records " + MethodInfo.GetCurrentMethod().Name + ex.StackTrace, ex);
            }
        }

        private void ProcessDeletes(DifferenceManager_CDAPI diffMan, TrackedClass trackedClass)
        {
            _logger.Info("Started excuting " + MethodInfo.GetCurrentMethod().Name);
            try
            {
                //EnumerableCursor2 deletes = GetChangesRowsCursor(diffMan, trackedClass, trackedClass.TargetTable, null, esriDifferenceType.esriDifferenceTypeDeleteNoChange);
                string FCName = trackedClass.QualifiedSourceClass;
                if (!string.IsNullOrWhiteSpace(FCName))
                {
                    if (diffMan._cdVersionDifference.ContainsKey(FCName.ToUpper()))
                    {
                        IList<DeleteFeat> deleteFeats = new List<DeleteFeat>();
                        deleteFeats = GetChangesRowsCursor(diffMan, trackedClass, esriDifferenceType.esriDifferenceTypeDeleteNoChange);

                        //deleteFeats = diffMan._cdVersionDifference[FCName.ToUpper()].Action.Delete;
                        //if (deletes.RowCount() > 0)
                        if (deleteFeats.Count() > 0)
                        {
                            _logger.Info("Processing SAP record  for feature class :" + trackedClass.SourceClass + "ActionType :" + ChangeType.Delete.ToString());
                            ProcessRows(trackedClass, deleteFeats, ChangeType.Delete);
                            _logger.Info("Successfully processed SAP record in for feature class :" + trackedClass.SourceClass);
                        }
                    }
                }
                #region - COMMENTED CODE - below else condtion to handle missing record. Non SAP edits will also insert in staging table with error description -v1t8 
                //Added below else condtion to handle missing record. Non SAP edits will also insert in staging table with error description -v1t8 
                //else
                //{
                //    if (nonSAPRecordList != null && nonSAPRecordList.Count > 0)
                //    {
                //        int count = 0;
                //        _logger.Info("Processing  Non SAP record in staging table for feature class :" + trackedClass.SourceClass);
                //        for (int i = 0; i < nonSAPRecordList.Count; i++)
                //        {
                //            ITable table = trackedClass.SourceTable;
                //            IRow row = table.GetRow(nonSAPRecordList[i]);
                //            LogError(row, trackedClass.SourceClass + "OID :" + nonSAPRecordList[i] + " GIS Edit which is not an valid SAP Record", ChangeType.Delete);
                //            count++;

                //        }
                //        _logger.Info("Successfully Inserted Non SAP record in staging table  :" + trackedClass.SourceClass + "Count :" + count);
                //    }

                //    _logger.Info("Successfully Inserted Non SAP record in staging table for feature class :" + trackedClass.SourceClass);
                //}
                #endregion
                _logger.Info("Sucessfully executed " + MethodInfo.GetCurrentMethod().Name);
            }
            catch (Oracle.DataAccess.Client.OracleException oex) { GISSAPIntegrator._logger.Error(oex.GetType() + " || " + oex.Number + " || " + oex.Message + " || \n" + oex.StackTrace); if (GISSAPIntegrator.HandleOracleErrorCodes || GISSAPIntegrator.OracleErrorCodes.Contains(Convert.ToString(oex.ErrorCode))) { throw oex; } }
            catch (Exception ex)
            {
                _logger.Error("Exception occure while processing delete records " + MethodInfo.GetCurrentMethod().Name + ex.StackTrace, ex);
            }
        }

        private void ProcessFiltered(TrackedClass trackedClass, EsriGeometry.IGeometry filterGeometry)
        {
            IEnumerable<IRow> filteredRows = Search(trackedClass, filterGeometry);
            if (filteredRows.Count<IRow>() > 0)
            {
                ProcessRows(trackedClass, filteredRows, ChangeType.Update);
            }
        }

        private void ProcessFiltered(TrackedClass trackedClass, IEnumerable<IRow> filteredRows)
        {
            ProcessRows(trackedClass, filteredRows, ChangeType.Update);
        }

        private IEnumerable<IRow> Search(TrackedClass trackedClass, EsriGeometry.IGeometry filterGeometry)
        {
            // do spatial query against parameter filterGeometry, treat rows as update
            ISpatialFilter filter = new SpatialFilterClass();

            string where = string.Empty;
            List<string> subtypes = trackedClass.RowTransformer.Subtypes;
            if (subtypes.Count > 0)
            {
                foreach (string subtype in subtypes)
                {
                    where = where + "SubtypeCD = " + subtype + " OR ";
                }

                // remove the ending "spaceORspace"
                if (where.Length > 4)
                {
                    where = where.Substring(0, where.Length - 4);
                }
            }

            //IVARA - Subhankar
            //Check if there is any field value based filter condition
            if (!string.IsNullOrEmpty(trackedClass.SingleFieldWhereClauseCondition))
            {
                if (!string.IsNullOrEmpty(where))
                    where = where + " AND " + trackedClass.SingleFieldWhereClauseCondition;
                else
                    where = trackedClass.SingleFieldWhereClauseCondition;
            }
            //IVARA
            //MEQ3
            else if (trackedClass.Subtypes == "3" && trackedClass.SourceClass == "SupportStructure")
            {
                if (!string.IsNullOrEmpty(where))
                    where += " AND (POLEUSE=4 OR POLEUSE=5) AND (SAPEQUIPID='9999' OR SAPEQUIPID is null) AND CUSTOMEROWNED='N' AND (STATUS = 5 OR STATUS=30)";
                else
                    where = "(POLEUSE=4 OR POLEUSE=5) AND (SAPEQUIPID='9999' OR SAPEQUIPID is null) AND CUSTOMEROWNED='N' AND (STATUS = 5 OR STATUS=30)";
            }

            filter.WhereClause = where;

            filter.set_GeometryEx(filterGeometry, true);
            filter.SearchOrder = esriSearchOrder.esriSearchOrderSpatial;
            IFeatureClass sourceFeatClass = null;
            if (trackedClass.SourceTable is IFeatureClass)
            {
                sourceFeatClass = (IFeatureClass)trackedClass.SourceTable;
            }
            filter.GeometryField = sourceFeatClass.ShapeFieldName;
            //filter.SubFields = sourceFeatClass.ShapeFieldName;
            filter.SpatialRel = esriSpatialRelEnum.esriSpatialRelContains;
            IFeatureCursor featCur = sourceFeatClass.Search(filter, false);

            try
            {
                if (featCur != null)
                {
                    IRow row;
                    row = featCur.NextFeature();
                    while (row != null)
                    {
                        yield return row;

                        row = featCur.NextFeature();
                    }
                }
            }
            finally
            {
                Marshal.ReleaseComObject(featCur);
            }
        }

        /// <summary>
        /// Obtains a list of where in clauses for a list of guids
        /// </summary>
        /// <param name="guids"></param>
        /// <returns></returns>
        private static List<string> GetWhereInClauses(List<int> guids)
        {
            try
            {
                List<string> whereInClauses = new List<string>();
                StringBuilder builder = new StringBuilder();

                for (int i = 0; i < guids.Count; i++)
                {
                    if ((((i % 999) == 0) && i != 0) || (guids.Count == i + 1))
                    {
                        builder.Append(guids[i]);
                        whereInClauses.Add(builder.ToString());
                        builder = new StringBuilder();
                    }
                    else { builder.Append(guids[i] + ","); }
                }
                return whereInClauses;
            }
            catch (Oracle.DataAccess.Client.OracleException oex) { GISSAPIntegrator._logger.Error(oex.GetType() + " || " + oex.Number + " || " + oex.Message + " || \n" + oex.StackTrace);  throw oex; }
            catch (Exception ex)
            {
                throw new Exception("Failed to create guid where in clauses. Error: " + ex.Message);
            }
        }

        /// <summary>
        /// This method is used to process version edits -v1t8
        /// </summary>
        /// <param name="trackedClass"></param>
        /// <param name="changedRows"></param>
        /// <param name="changeType"></param>
        public void ProcessRows(TrackedClass trackedClass, IEnumerable<IRow> changedRows, ChangeType changeType)
        {
            Dictionary<int, double> assetOIDAndProcessTime = AssetOIDAndProcessTimeByClass[trackedClass.OutName + trackedClass.SourceClass];
            _logger.Info("Started excuting " + MethodInfo.GetCurrentMethod().Name);

            int totalRows = 0;
            //V3SF (28-Mar-2022) Added Additional variable to display Skipping Record in Staging
            string errMsg = string.Empty;
            //StringBuilder _errorMessage = new StringBuilder();
            if (changedRows is EnumerableCursor) { totalRows = ((EnumerableCursor)changedRows).RowCount(); }
            else if (changedRows is EnumerableCursor2) { totalRows = ((EnumerableCursor2)changedRows).RowCount(); }
            else { totalRows = changedRows.Count(); }

            int rowCounter = 0;
            if (changeType != ChangeType.Reprocess) { Log("Processing " + totalRows + " " + changeType + " rows of " + trackedClass.OutName + " " + trackedClass.SourceClass, MessageType.Debug); }

            /*
            Dictionary<int, IRow> associatedUpdateRows = new Dictionary<int, IRow>();
            foreach (IRow changedRow in changedRows)
            {
                associatedUpdateRows.Add(changedRow.OID, null);
            }

            IQueryFilter qf = new QueryFilterClass();
            List<string> whereInClauses = GetWhereInClauses(associatedUpdateRows.Keys.ToList());
            foreach (string whereInClause in whereInClauses)
            {

            }
            */

            foreach (IRow changedRow in changedRows)
            {
                EnumerableCursor2 enumCursor2 = changedRows as EnumerableCursor2;
                IRow sourceRow = null;
                IRow targetRow = null;

                bool success = true;
                try
                {
                    rowCounter++;
                    //Log("NO." + rowCounter + ": " + changedRow.OID, MessageType.Debug);
                    if (changeType != ChangeType.Reprocess) { Log("Processing " + rowCounter + " of " + totalRows + " for " + trackedClass.OutName + ": OID: " + changedRow.OID, MessageType.Info); }
                    //Console.WriteLine("Processing " + rowCounter + " of " + totalRows + " for " + trackedClass.OutName + ": OID: " + changedRow.OID, MessageType.Info);

                    Stopwatch stopWatch = new Stopwatch();
                    stopWatch.Start();                    
                    // check if the row has been processed as entering row
                    if (trackedClass.ProcessedRowOIDs.Contains(changedRow.OID))
                    {
                        if (changedRow != null) { while (Marshal.ReleaseComObject(changedRow) > 0) { } }
                        continue;
                    }
                    //V3SF (28-Mar-2022) Added Additional variable to display Skipping Record in Staging
                    else if (!trackedClass.RowTransformer.IsValid(changedRow,out errMsg)) //IVARA
                    {
                        LogError(changedRow, trackedClass.SourceClass + ", OID : " + changedRow.OID + " Not an Valid SAP record/CustomerOwned " + errMsg, changeType);
                        _logger.Info("Successfully Inserted GIS record in staging table with error :" + " OID : " + changedRow.OID + " " + trackedClass.SourceClass);
                        
                        continue;
                    }

                    switch (changeType)
                    {
                        case ChangeType.Insert:
                            sourceRow = changedRow;
                            break;
                        case ChangeType.Delete:
                            targetRow = changedRow;
                            break;
                        case ChangeType.Update:
                            sourceRow = changedRow;
                            /*Is this necessary? Updates don't need the row from SDE.Default*/
                            //int objID = changedRow.OID;
                            //ITable targetTable = trackedClass.TargetTable;
                            if (enumCursor2 != null) { targetRow = enumCursor2.TargtRow; }
                            else { targetRow = trackedClass.TargetTable.GetRow(changedRow.OID); }
                            //Catch all in case something fails above
                            if (targetRow == null) { targetRow = trackedClass.TargetTable.GetRow(changedRow.OID); }
                            break;
                        case ChangeType.Reprocess:
                            targetRow = changedRow;
                            sourceRow = changedRow;
                            break;
                        default:
                            break;
                    }

                    IRowTransformer<IRow> rowTransformer = trackedClass.RowTransformer;



                    //List<IRowData> rowDataList = rowTransformer.ProcessRow(sourceRow, targetRow, changeType);
                    //Adding the logic below to capture all the errors instead of processing and giving piece meal error for each issue found.
                    //The following lines of code will make the application to run even after failure and will process all the features and the Actionahandler should report all the issues found using the _error object.
                    //After reporting all the errors we should throw an exception to stop the post.
                    //It should mostly error out only for configuration exception here. Even then do not stop the process run through all the features so we can collect all the configuration exception an report it at the end.
                    //All the other data related exception is handled as part of ObjectIDOID Dictionary that is storing the issues as part of RowTransformer.Process method.
                    List<IRowData> rowDataList = null; int preRowCount = 0;
                    try
                    {
                        if (rowDataList != null)
                        { preRowCount = rowDataList.Count; }

                        
                        _errorMessage.Clear();

                        rowDataList = rowTransformer.ProcessRow(sourceRow, targetRow, changeType);
                        _logger.Info("Processed record from SAP row transformer " + " OID : " + changedRow.OID + trackedClass.SourceClass);

                        int latestRowCount = rowDataList.Count;
                        if (preRowCount == latestRowCount)
                        {
                            LogError(changedRow, trackedClass.SourceClass + ", OID : " + changedRow.OID + " Not an Valid SAP record/CustomerOwned "+ _errorMessage, changeType);
                            _logger.Info("Successfully Inserted GIS record in staging table with error :" + " OID : " + changedRow.OID + " " +  trackedClass.SourceClass);
                        }
                        //throw new Exception("my custom execption");
                    }
                    catch (Oracle.DataAccess.Client.OracleException oex) { GISSAPIntegrator._logger.Error(oex.GetType() + " || " + oex.Number + " || " + oex.Message + " || \n" + oex.StackTrace); if (GISSAPIntegrator.HandleOracleErrorCodes || GISSAPIntegrator.OracleErrorCodes.Contains(Convert.ToString(oex.ErrorCode))) { throw oex; } }
                    catch (Exception e)
                    {
                        //Need to handle  error description -v1t8
#if DEBUG   
                        Debugger.Launch();
#endif
                        //_logger.Debug(string.Format("Processing failed for object with OID {0} from featureclass {1} for change type {2}", changedRow.OID, ((IDataset)changedRow.Table).Name, changeType));
                        Log(string.Format("Failed to process ObjectID {0} with error {1}: {2}", changedRow.OID, e.Message, e.StackTrace), MessageType.Debug);
                        Console.WriteLine(string.Format("Failed to process ObjectID {0} with error {1}: {2}", changedRow.OID, e.Message, e.StackTrace));
                        //logMethod(string.Format("Failed to process NO.{0}: {1}", rowCounter, changedRow.OID));
                        _errors.Add(e);
                        success = false;

                        UpdateProcessedState(trackedClass, changedRow, changeType, Processed.ErrorDuringProcessing, e.Message);
                        //Below Code is added for EDGIS Rearch project by v1t8
                        //To log error in the error description field in staging table which will further utilize for GIS Error  dashboard 
                        LogError(changedRow, trackedClass.SourceClass + ", OID : " + changedRow.OID + e.Message, changeType);
                        continue;
                    }

                    try
                    {
                        if (changeType != ChangeType.Reprocess)
                        { Log(string.Format("{0} of rows processed by rowTransformer for change type {1}", rowDataList.Count, changeType), MessageType.Debug); }
                        else
                        {
                            bool errorsFound = false;
                            //Reprocess. Let's check for any errors
                            foreach (IRowData rowData in rowDataList)
                            {
                                if (!rowData.Valid)
                                {
                                    //string errorMessage = rowData.FeatureClassName + " with OID " + rowData.OID + " failed due to following issues:\r\n";
                                    //errorMessage += rowData.ErrorMessage;
                                    UpdateProcessedState(trackedClass, changedRow, changeType, Processed.ErrorDuringProcessing, rowData.ErrorMessage);
                                    errorsFound = true;
                                }
                            }
                            if (errorsFound)
                            {
                                success = false;
                                continue;
                            }
                        }
                    }
                    catch (Oracle.DataAccess.Client.OracleException oex) { GISSAPIntegrator._logger.Error(oex.GetType() + " || " + oex.Number + " || " + oex.Message + " || \n" + oex.StackTrace); throw oex; }
                    catch (Exception ex)
                    {
#if DEBUG
                        Debugger.Launch();
#endif
                        throw ex;
                    }

                    //logMethod(string.Format("{0} of rows processed by rowTransformer for change type {1}", rowDataList.Count, changeType));

                    try
                    {
                        if (rowDataList.Count > 0)
                        {
                            string outName = trackedClass.OutName;
                            Dictionary<string, IRowData> assetIDandRowData;
                            if (_systemMapper.AssetIDandRowDataByOutName.ContainsKey(outName))
                            {
                                assetIDandRowData = _systemMapper.AssetIDandRowDataByOutName[outName];
                            }
                            else
                            {
                                assetIDandRowData = new Dictionary<string, IRowData>();
                                _systemMapper.AssetIDandRowDataByOutName[outName] = assetIDandRowData;
                            }

                            // add the processed RowData result to cash
                            foreach (IRowData rowData in rowDataList)
                            {
                                // if data are all correct, this check is not needed but it's not rare to see one Stitch Point being related to more than one CircuitSource...
                                if (assetIDandRowData.ContainsKey(rowData.AssetID) == false)
                                {
                                    assetIDandRowData.Add(rowData.AssetID, rowData);
                                }
                            }

                            if (trackedClass.SourceClass == "Controller")
                            {
                                ControllerRowTransformer.ProcessedOIDs.Add(changedRow.OID);
                            }
                        }
                        else
                        {
                            success = false;
                        }

                        // cash so a row won't be process more than once
                        // a typical example is an inserted CapacitorBank that also falls in one new MaintenancePolygon
                        trackedClass.ProcessedRowOIDs.Add(changedRow.OID);
                        assetOIDAndProcessTime.Add(changedRow.OID, stopWatch.Elapsed.TotalSeconds);
                    }
                    catch (Oracle.DataAccess.Client.OracleException oex) { GISSAPIntegrator._logger.Error(oex.GetType() + " || " + oex.Number + " || " + oex.Message + " || \n" + oex.StackTrace); throw oex; }
                    catch (Exception ex)
                    {
                        throw ex;
                    }

                    _logger.Info("Sucessfully executed " + MethodInfo.GetCurrentMethod().Name);
                }
                catch (Oracle.DataAccess.Client.OracleException oex) { GISSAPIntegrator._logger.Error(oex.GetType() + " || " + oex.Number + " || " + oex.Message + " || \n" + oex.StackTrace); if (GISSAPIntegrator.HandleOracleErrorCodes || GISSAPIntegrator.OracleErrorCodes.Contains(Convert.ToString(oex.ErrorCode)) || changeType != ChangeType.Reprocess) { throw oex; } }
                catch (Exception ex)
                {
                    UpdateProcessedState(trackedClass, changedRow, changeType, Processed.ErrorDuringProcessing, ex.Message);
                    success = false;
                    _logger.Error("Exception occure while Proccessing rows" + MethodInfo.GetCurrentMethod().Name + ex.StackTrace, ex);
                    if (changeType != ChangeType.Reprocess) { throw ex; }
                }
                finally
                {
                    if (success) { UpdateProcessedState(trackedClass, changedRow, changeType, Processed.SuccessfullyProcessed, ""); }

                    if (changedRow != null) { while (Marshal.ReleaseComObject(changedRow) > 0) { } }
                    if (sourceRow != null) { while (Marshal.ReleaseComObject(sourceRow) > 0) { } }
                    if (targetRow != null) { while (Marshal.ReleaseComObject(targetRow) > 0) { } }
                    if (enumCursor2 != null) { enumCursor2.TargtRow = null; }
                }
            } //changedRow
        }

        public void ProcessRows(TrackedClass trackedClass, IEnumerable<IRow> changedRows, ChangeType changeType,DifferenceManager_CDAPI differenceManager_CDAPI)
        {
            Dictionary<int, double> assetOIDAndProcessTime = AssetOIDAndProcessTimeByClass[trackedClass.OutName + trackedClass.SourceClass];
            _logger.Info("Started excuting " + MethodInfo.GetCurrentMethod().Name);

            int totalRows = 0;
            //V3SF (28-Mar-2022) Added Additional variable to display Skipping Record in Staging
            string errMsg = string.Empty;
            //StringBuilder _errorMessage = new StringBuilder();
            if (changedRows is EnumerableCursor) { totalRows = ((EnumerableCursor)changedRows).RowCount(); }
            else if (changedRows is EnumerableCursor2) { totalRows = ((EnumerableCursor2)changedRows).RowCount(); }
            else { totalRows = changedRows.Count(); }

            int rowCounter = 0;
            if (changeType != ChangeType.Reprocess) { Log("Processing " + totalRows + " " + changeType + " rows of " + trackedClass.OutName + " " + trackedClass.SourceClass, MessageType.Debug); }
            UpdateFeat updateFeat = default;
            /*
            Dictionary<int, IRow> associatedUpdateRows = new Dictionary<int, IRow>();
            foreach (IRow changedRow in changedRows)
            {
                associatedUpdateRows.Add(changedRow.OID, null);
            }

            IQueryFilter qf = new QueryFilterClass();
            List<string> whereInClauses = GetWhereInClauses(associatedUpdateRows.Keys.ToList());
            foreach (string whereInClause in whereInClauses)
            {

            }
            */

            foreach (IRow changedRow in changedRows)
            {
                EnumerableCursor2 enumCursor2 = changedRows as EnumerableCursor2;
                IRow sourceRow = null;
                IRow targetRow = null;

                bool success = true;
                try
                {
                    rowCounter++;
                    //Log("NO." + rowCounter + ": " + changedRow.OID, MessageType.Debug);
                    if (changeType != ChangeType.Reprocess) { Log("Processing " + rowCounter + " of " + totalRows + " for " + trackedClass.OutName + ": OID: " + changedRow.OID, MessageType.Info); }
                    //Console.WriteLine("Processing " + rowCounter + " of " + totalRows + " for " + trackedClass.OutName + ": OID: " + changedRow.OID, MessageType.Info);

                    Stopwatch stopWatch = new Stopwatch();
                    stopWatch.Start();
                    // check if the row has been processed as entering row
                    if (trackedClass.ProcessedRowOIDs.Contains(changedRow.OID))
                    {
                        if (changedRow != null) { while (Marshal.ReleaseComObject(changedRow) > 0) { } }
                        continue;
                    }
                    //V3SF (28-Mar-2022) Added Additional variable to display Skipping Record in Staging
                    else if (!trackedClass.RowTransformer.IsValid(changedRow,out errMsg)) //IVARA
                    {
                        LogError(changedRow, trackedClass.SourceClass + ", OID : " + changedRow.OID + " Not an Valid SAP record/CustomerOwned " + errMsg, changeType);
                        _logger.Info("Successfully Inserted GIS record in staging table with error :" + " OID : " + changedRow.OID + " " + trackedClass.SourceClass);

                        continue;
                    }

                    switch (changeType)
                    {
                        case ChangeType.Insert:
                            sourceRow = changedRow;
                            break;
                        case ChangeType.Delete:
                            targetRow = changedRow;
                            break;
                        case ChangeType.Update:
                            sourceRow = changedRow;
                            /*Is this necessary? Updates don't need the row from SDE.Default*/
                            //int objID = changedRow.OID;
                            //ITable targetTable = trackedClass.TargetTable;
                            if (enumCursor2 != null) { targetRow = enumCursor2.TargtRow; }
                            else { targetRow = trackedClass.TargetTable.GetRow(changedRow.OID); }
                            //Catch all in case something fails above
                            if (targetRow == null) { targetRow = trackedClass.TargetTable.GetRow(changedRow.OID); }
                            break;
                        case ChangeType.Reprocess:
                            targetRow = changedRow;
                            sourceRow = changedRow;
                            break;
                        default:
                            break;
                    }

                    IRowTransformer<IRow> rowTransformer = trackedClass.RowTransformer;



                    //List<IRowData> rowDataList = rowTransformer.ProcessRow(sourceRow, targetRow, changeType);
                    //Adding the logic below to capture all the errors instead of processing and giving piece meal error for each issue found.
                    //The following lines of code will make the application to run even after failure and will process all the features and the Actionahandler should report all the issues found using the _error object.
                    //After reporting all the errors we should throw an exception to stop the post.
                    //It should mostly error out only for configuration exception here. Even then do not stop the process run through all the features so we can collect all the configuration exception an report it at the end.
                    //All the other data related exception is handled as part of ObjectIDOID Dictionary that is storing the issues as part of RowTransformer.Process method.
                    List<IRowData> rowDataList = null; int preRowCount = 0;
                    try
                    {
                        if (rowDataList != null)
                        { preRowCount = rowDataList.Count; }

                        _errorMessage.Clear();

                        if (changeType == ChangeType.Update)
                        {
                            string FCName = ((IDataset)(sourceRow.Table)).BrowseName.ToUpper();
                            if (differenceManager_CDAPI._cdVersionDifference.ContainsKey(FCName))
                            {
                                if(differenceManager_CDAPI._cdVersionDifference[FCName].Action.Update.Any(str => str.OID == sourceRow.OID ))
                                {
                                    updateFeat = differenceManager_CDAPI._cdVersionDifference[FCName].Action.Update.First(str => str.OID == sourceRow.OID);
                                    updateFeat.Table = sourceRow.Table;
                                    updateFeat.ChangeFeatures = differenceManager_CDAPI._cdVersionDifference;
                                }
                            }

                            if (updateFeat != null)
                                rowDataList = rowTransformer.ProcessRow(sourceRow, updateFeat, changeType);
                            else
                                continue;
                        }
                        else
                        {
                            rowDataList = rowTransformer.ProcessRow(sourceRow, targetRow, changeType);
                        }
                        _logger.Info("Processed record from SAP row transformer " + " OID : " + changedRow.OID + trackedClass.SourceClass);

                        int latestRowCount = rowDataList.Count;
                        if (preRowCount == latestRowCount)
                        {
                            LogError(changedRow, trackedClass.SourceClass + ", OID : " + changedRow.OID + " Not an Valid SAP record/CustomerOwned " + _errorMessage, changeType);
                            _logger.Info("Successfully Inserted GIS record in staging table with error :" + " OID : " + changedRow.OID + " " + trackedClass.SourceClass);
                        }
                        //throw new Exception("my custom execption");
                    }
                    catch (Oracle.DataAccess.Client.OracleException oex) { GISSAPIntegrator._logger.Error(oex.GetType() + " || " + oex.Number + " || " + oex.Message + " || \n" + oex.StackTrace); if (GISSAPIntegrator.HandleOracleErrorCodes || GISSAPIntegrator.OracleErrorCodes.Contains(Convert.ToString(oex.ErrorCode))) { throw oex; } }
                    catch (Exception e)
                    {
                        //Need to handle  error description -v1t8
#if DEBUG   
                        Debugger.Launch();
#endif
                        //_logger.Debug(string.Format("Processing failed for object with OID {0} from featureclass {1} for change type {2}", changedRow.OID, ((IDataset)changedRow.Table).Name, changeType));
                        Log(string.Format("Failed to process ObjectID {0} with error {1}: {2}", changedRow.OID, e.Message, e.StackTrace), MessageType.Debug);
                        Console.WriteLine(string.Format("Failed to process ObjectID {0} with error {1}: {2}", changedRow.OID, e.Message, e.StackTrace));
                        //logMethod(string.Format("Failed to process NO.{0}: {1}", rowCounter, changedRow.OID));
                        _errors.Add(e);
                        success = false;

                        UpdateProcessedState(trackedClass, changedRow, changeType, Processed.ErrorDuringProcessing, e.Message);
                        //Below Code is added for EDGIS Rearch project by v1t8
                        //To log error in the error description field in staging table which will further utilize for GIS Error  dashboard 
                        LogError(changedRow, trackedClass.SourceClass + ", OID : " + changedRow.OID + e.Message, changeType);
                        continue;
                    }

                    try
                    {
                        if (changeType != ChangeType.Reprocess)
                        { Log(string.Format("{0} of rows processed by rowTransformer for change type {1}", rowDataList.Count, changeType), MessageType.Debug); }
                        else
                        {
                            bool errorsFound = false;
                            //Reprocess. Let's check for any errors
                            foreach (IRowData rowData in rowDataList)
                            {
                                if (!rowData.Valid)
                                {
                                    //string errorMessage = rowData.FeatureClassName + " with OID " + rowData.OID + " failed due to following issues:\r\n";
                                    //errorMessage += rowData.ErrorMessage;
                                    UpdateProcessedState(trackedClass, changedRow, changeType, Processed.ErrorDuringProcessing, rowData.ErrorMessage);
                                    errorsFound = true;
                                }
                            }
                            if (errorsFound)
                            {
                                success = false;
                                continue;
                            }
                        }
                    }
                    catch (Oracle.DataAccess.Client.OracleException oex) { GISSAPIntegrator._logger.Error(oex.GetType() + " || " + oex.Number + " || " + oex.Message + " || \n" + oex.StackTrace); throw oex; }
                    catch (Exception ex)
                    {
#if DEBUG
                        Debugger.Launch();
#endif
                        throw ex;
                    }

                    //logMethod(string.Format("{0} of rows processed by rowTransformer for change type {1}", rowDataList.Count, changeType));

                    try
                    {
                        if (rowDataList.Count > 0)
                        {
                            string outName = trackedClass.OutName;
                            Dictionary<string, IRowData> assetIDandRowData;
                            if (_systemMapper.AssetIDandRowDataByOutName.ContainsKey(outName))
                            {
                                assetIDandRowData = _systemMapper.AssetIDandRowDataByOutName[outName];
                            }
                            else
                            {
                                assetIDandRowData = new Dictionary<string, IRowData>();
                                _systemMapper.AssetIDandRowDataByOutName[outName] = assetIDandRowData;
                            }

                            // add the processed RowData result to cash
                            foreach (IRowData rowData in rowDataList)
                            {
                                // if data are all correct, this check is not needed but it's not rare to see one Stitch Point being related to more than one CircuitSource...
                                if (assetIDandRowData.ContainsKey(rowData.AssetID) == false)
                                {
                                    assetIDandRowData.Add(rowData.AssetID, rowData);
                                }
                            }

                            if (trackedClass.SourceClass == "Controller")
                            {
                                ControllerRowTransformer.ProcessedOIDs.Add(changedRow.OID);
                            }
                        }
                        else
                        {
                            success = false;
                        }

                        // cash so a row won't be process more than once
                        // a typical example is an inserted CapacitorBank that also falls in one new MaintenancePolygon
                        trackedClass.ProcessedRowOIDs.Add(changedRow.OID);
                        assetOIDAndProcessTime.Add(changedRow.OID, stopWatch.Elapsed.TotalSeconds);
                    }
                    catch (Oracle.DataAccess.Client.OracleException oex) { GISSAPIntegrator._logger.Error(oex.GetType() + " || " + oex.Number + " || " + oex.Message + " || \n" + oex.StackTrace); throw oex; }
                    catch (Exception ex)
                    {
                        throw ex;
                    }

                    _logger.Info("Sucessfully executed " + MethodInfo.GetCurrentMethod().Name);
                }
                catch (Oracle.DataAccess.Client.OracleException oex) { GISSAPIntegrator._logger.Error(oex.GetType() + " || " + oex.Number + " || " + oex.Message + " || \n" + oex.StackTrace); if (GISSAPIntegrator.HandleOracleErrorCodes || GISSAPIntegrator.OracleErrorCodes.Contains(Convert.ToString(oex.ErrorCode)) || changeType != ChangeType.Reprocess) { throw oex; } }
                catch (Exception ex)
                {
                    UpdateProcessedState(trackedClass, changedRow, changeType, Processed.ErrorDuringProcessing, ex.Message);
                    success = false;
                    _logger.Error("Exception occure while Proccessing rows" + MethodInfo.GetCurrentMethod().Name + ex.StackTrace, ex);
                    if (changeType != ChangeType.Reprocess) { throw ex; }
                }
                finally
                {
                    if (success) { UpdateProcessedState(trackedClass, changedRow, changeType, Processed.SuccessfullyProcessed, ""); }

                    if (changedRow != null) { while (Marshal.ReleaseComObject(changedRow) > 0) { } }
                    if (sourceRow != null) { while (Marshal.ReleaseComObject(sourceRow) > 0) { } }
                    if (targetRow != null) { while (Marshal.ReleaseComObject(targetRow) > 0) { } }
                    if (enumCursor2 != null) { enumCursor2.TargtRow = null; }
                }
            } //changedRow
        }

        private void ProcessRows(TrackedClass trackedClass, IList<DeleteFeat> changedRows, ChangeType changeType)
        {
            Dictionary<int, double> assetOIDAndProcessTime = AssetOIDAndProcessTimeByClass[trackedClass.OutName + trackedClass.SourceClass];
            _logger.Info("Started excuting " + MethodInfo.GetCurrentMethod().Name);

            int totalRows = 0;
            string errMsg = string.Empty;
            //StringBuilder _errorMessage = new StringBuilder();
            totalRows = changedRows.Count;

            int rowCounter = 0;
            if (changeType != ChangeType.Reprocess) { Log("Processing " + totalRows + " " + changeType + " rows of " + trackedClass.OutName + " " + trackedClass.SourceClass, MessageType.Debug); }

            foreach (DeleteFeat changedRow in changedRows)
            {
                //EnumerableCursor2 enumCursor2 = changedRows as EnumerableCursor2;
                //IRow sourceRow = null;
                //IRow targetRow = null;

                bool success = true;
                try
                {
                    rowCounter++;
                    //Log("NO." + rowCounter + ": " + changedRow.OID, MessageType.Debug);
                    if (changeType != ChangeType.Reprocess) { Log("Processing " + rowCounter + " of " + totalRows + " for " + trackedClass.OutName + ": OID: " + changedRow.OID, MessageType.Info); }
                    //Console.WriteLine("Processing " + rowCounter + " of " + totalRows + " for " + trackedClass.OutName + ": OID: " + changedRow.OID, MessageType.Info);

                    Stopwatch stopWatch = new Stopwatch();
                    stopWatch.Start();
                    // check if the row has been processed as entering row
                    if (trackedClass.ProcessedRowOIDs.Contains(changedRow.OID))
                    {
                        if (changedRow != null)
                        {
                            //do nothing
                        }
                        continue;
                    }
                    else if (!trackedClass.RowTransformer.IsValid(changedRow,out errMsg)) //IVARA
                    {
                        LogError(changedRow, trackedClass.SourceClass + ", OID : " + changedRow.OID + " Not an Valid SAP record/CustomerOwned " + errMsg, changeType);
                        _logger.Info("Successfully Inserted GIS record in staging table with error :" + " OID : " + changedRow.OID + " " + trackedClass.SourceClass);
                        continue;
                    }

                    //switch (changeType)
                    //{
                    //    case ChangeType.Insert:
                    //        sourceRow = changedRow;
                    //        break;
                    //    case ChangeType.Delete:
                    //        targetRow = changedRow;
                    //        break;
                    //    case ChangeType.Update:
                    //        sourceRow = changedRow;
                    //        /*Is this necessary? Updates don't need the row from SDE.Default*/
                    //        //int objID = changedRow.OID;
                    //        //ITable targetTable = trackedClass.TargetTable;
                    //        if (enumCursor2 != null) { targetRow = enumCursor2.TargtRow; }
                    //        else { targetRow = trackedClass.TargetTable.GetRow(changedRow.OID); }
                    //        //Catch all in case something fails above
                    //        if (targetRow == null) { targetRow = trackedClass.TargetTable.GetRow(changedRow.OID); }
                    //        break;
                    //    case ChangeType.Reprocess:
                    //        targetRow = changedRow;
                    //        sourceRow = changedRow;
                    //        break;
                    //    default:
                    //        break;
                    //}

                    IRowTransformer<IRow> rowTransformer = trackedClass.RowTransformer;



                    //List<IRowData> rowDataList = rowTransformer.ProcessRow(sourceRow, targetRow, changeType);
                    //Adding the logic below to capture all the errors instead of processing and giving piece meal error for each issue found.
                    //The following lines of code will make the application to run even after failure and will process all the features and the Actionahandler should report all the issues found using the _error object.
                    //After reporting all the errors we should throw an exception to stop the post.
                    //It should mostly error out only for configuration exception here. Even then do not stop the process run through all the features so we can collect all the configuration exception an report it at the end.
                    //All the other data related exception is handled as part of ObjectIDOID Dictionary that is storing the issues as part of RowTransformer.Process method.
                    List<IRowData> rowDataList = null; int preRowCount = 0;
                    try
                    {
                        if (rowDataList != null)
                        { preRowCount = rowDataList.Count; }

                        _errorMessage = new StringBuilder();
                        
                        rowDataList = rowTransformer.ProcessRow(changedRow, changeType); //V3SF till here it is done
                        _logger.Info("Processed record from SAP row transformer " + " OID : " + changedRow.OID + trackedClass.SourceClass);

                        int latestRowCount = rowDataList.Count;
                        if (preRowCount == latestRowCount)
                        {
                            LogError(changedRow, trackedClass.SourceClass + ", OID : " + changedRow.OID + " Not an Valid SAP record/CustomerOwned " + _errorMessage, changeType);
                            _logger.Info("Successfully Inserted GIS record in staging table with error :" + " OID : " + changedRow.OID + " " + trackedClass.SourceClass);
                        }
                        //throw new Exception("my custom execption");
                    }
                    catch (Oracle.DataAccess.Client.OracleException oex) { GISSAPIntegrator._logger.Error(oex.GetType() + " || " + oex.Number + " || " + oex.Message + " || \n" + oex.StackTrace); if (GISSAPIntegrator.HandleOracleErrorCodes || GISSAPIntegrator.OracleErrorCodes.Contains(Convert.ToString(oex.ErrorCode))) { throw oex; } }
                    catch (Exception e)
                    {
                        //Need to handle  error description -v1t8
#if DEBUG   
                        Debugger.Launch();
#endif
                        //_logger.Debug(string.Format("Processing failed for object with OID {0} from featureclass {1} for change type {2}", changedRow.OID, ((IDataset)changedRow.Table).Name, changeType));
                        Log(string.Format("Failed to process ObjectID {0} with error {1}: {2}", changedRow.OID, e.Message, e.StackTrace), MessageType.Debug);
                        Console.WriteLine(string.Format("Failed to process ObjectID {0} with error {1}: {2}", changedRow.OID, e.Message, e.StackTrace));
                        //logMethod(string.Format("Failed to process NO.{0}: {1}", rowCounter, changedRow.OID));
                        _errors.Add(e);
                        success = false;

                        UpdateProcessedState(trackedClass, changedRow, changeType, Processed.ErrorDuringProcessing, e.Message);
                        //Below Code is added for EDGIS Rearch project by v1t8
                        //To log error in the error description field in staging table which will further utilize for GIS Error  dashboard 

                        LogError(changedRow, trackedClass.SourceClass + ", OID : " + changedRow.OID + e.Message, changeType);
                        continue;
                    }

                    try
                    {
                        if (changeType != ChangeType.Reprocess)
                        { Log(string.Format("{0} of rows processed by rowTransformer for change type {1}", rowDataList.Count, changeType), MessageType.Debug); }
                        else
                        {
                            bool errorsFound = false;
                            //Reprocess. Let's check for any errors
                            foreach (IRowData rowData in rowDataList)
                            {
                                if (!rowData.Valid)
                                {
                                    //string errorMessage = rowData.FeatureClassName + " with OID " + rowData.OID + " failed due to following issues:\r\n";
                                    //errorMessage += rowData.ErrorMessage;
                                    UpdateProcessedState(trackedClass, changedRow, changeType, Processed.ErrorDuringProcessing, rowData.ErrorMessage);
                                    errorsFound = true;
                                }
                            }
                            if (errorsFound)
                            {
                                success = false;
                                continue;
                            }
                        }
                    }
                    catch (Oracle.DataAccess.Client.OracleException oex) { GISSAPIntegrator._logger.Error(oex.GetType() + " || " + oex.Number + " || " + oex.Message + " || \n" + oex.StackTrace); throw oex; }
                    catch (Exception ex)
                    {
#if DEBUG
                        Debugger.Launch();
#endif
                        throw ex;
                    }

                    //logMethod(string.Format("{0} of rows processed by rowTransformer for change type {1}", rowDataList.Count, changeType));

                    try
                    {
                        if (rowDataList.Count > 0)
                        {
                            string outName = trackedClass.OutName;
                            Dictionary<string, IRowData> assetIDandRowData;
                            if (_systemMapper.AssetIDandRowDataByOutName.ContainsKey(outName))
                            {
                                assetIDandRowData = _systemMapper.AssetIDandRowDataByOutName[outName];
                            }
                            else
                            {
                                assetIDandRowData = new Dictionary<string, IRowData>();
                                _systemMapper.AssetIDandRowDataByOutName[outName] = assetIDandRowData;
                            }

                            // add the processed RowData result to cash
                            foreach (IRowData rowData in rowDataList)
                            {
                                // if data are all correct, this check is not needed but it's not rare to see one Stitch Point being related to more than one CircuitSource...
                                if (assetIDandRowData.ContainsKey(rowData.AssetID) == false)
                                {
                                    assetIDandRowData.Add(rowData.AssetID, rowData);
                                }
                            }

                            if (trackedClass.SourceClass == "Controller")
                            {
                                ControllerRowTransformer.ProcessedOIDs.Add(changedRow.OID);
                            }
                        }
                        else
                        {
                            success = false;
                        }

                        // cash so a row won't be process more than once
                        // a typical example is an inserted CapacitorBank that also falls in one new MaintenancePolygon
                        trackedClass.ProcessedRowOIDs.Add(changedRow.OID);
                        assetOIDAndProcessTime.Add(changedRow.OID, stopWatch.Elapsed.TotalSeconds);
                    }
                    catch (Oracle.DataAccess.Client.OracleException oex) { GISSAPIntegrator._logger.Error(oex.GetType() + " || " + oex.Number + " || " + oex.Message + " || \n" + oex.StackTrace); throw oex; }
                    catch (Exception ex)
                    {
                        throw ex;
                    }

                    _logger.Info("Sucessfully executed " + MethodInfo.GetCurrentMethod().Name);
                }
                catch (Oracle.DataAccess.Client.OracleException oex) { GISSAPIntegrator._logger.Error(oex.GetType() + " || " + oex.Number + " || " + oex.Message + " || \n" + oex.StackTrace); if (GISSAPIntegrator.HandleOracleErrorCodes || GISSAPIntegrator.OracleErrorCodes.Contains(Convert.ToString(oex.ErrorCode)) || changeType != ChangeType.Reprocess) { throw oex; } }
                catch (Exception ex)
                {
                    UpdateProcessedState(trackedClass, changedRow, changeType, Processed.ErrorDuringProcessing, ex.Message);
                    success = false;
                    _logger.Error("Exception occure while Proccessing rows" + MethodInfo.GetCurrentMethod().Name + ex.StackTrace, ex);
                    if (changeType != ChangeType.Reprocess) { throw ex; }
                }
                finally
                {
                    if (success)
                    {

                        UpdateProcessedState(trackedClass, changedRow, changeType, Processed.SuccessfullyProcessed, "");
                    }

                    //if (changedRow != null) { while (Marshal.ReleaseComObject(changedRow) > 0) { } }
                    //if (sourceRow != null) { while (Marshal.ReleaseComObject(sourceRow) > 0) { } }
                    //if (targetRow != null) { while (Marshal.ReleaseComObject(targetRow) > 0) { } }
                    //if (enumCursor2 != null) { enumCursor2.TargtRow = null; }
                }
            } //changedRow
        }

        private void UpdateProcessedState(TrackedClass trackedClass, IRow changedRow, ChangeType changeType, Processed succesResult, string error)
        {
            if (changeType == ChangeType.Reprocess)
            {
                //Change type is reprocess, so we want to update the reprocess table with this asset having been processed.  The error string has to have single quotes replaced by two single
                //quotes to support inserting to the database
                string globalIDField = GlobalIDFieldName(changedRow.Table);
                int globalIDx = changedRow.Fields.FindField(globalIDField);
                string globalID = changedRow.get_Value(globalIDx).ToString().ToUpper();
                string sql = string.Format("UPDATE " + ReprocessTable + " SET PROCESSED = {0}, ERROR = '{2}' WHERE ASSETID = '{1}'", (int)succesResult, globalID, error.Replace("'", "''"));
                ((IDataset)changedRow.Table).Workspace.ExecuteSQL(sql);
                ((IDataset)changedRow.Table).Workspace.ExecuteSQL("COMMIT");

            }


        }

        private void UpdateProcessedState(TrackedClass trackedClass, DeleteFeat changedRow, ChangeType changeType, Processed succesResult, string error)
        {
            if (changeType == ChangeType.Reprocess)
            {
                //Change type is reprocess, so we want to update the reprocess table with this asset having been processed.  The error string has to have single quotes replaced by two single
                //quotes to support inserting to the database
                //string globalIDField = GlobalIDFieldName(changedRow.Table);
                //int globalIDx = changedRow.Fields.FindField(globalIDField);
                //string globalID = changedRow.get_Value(globalIDx).ToString().ToUpper();
                string globalID = GlobalIDField(changedRow);
                string sql = string.Format("UPDATE " + ReprocessTable + " SET PROCESSED = {0}, ERROR = '{2}' WHERE ASSETID = '{1}'", (int)succesResult, globalID, error.Replace("'", "''"));
                ((IDataset)trackedClass.SourceTable).Workspace.ExecuteSQL(sql);
                ((IDataset)trackedClass.SourceTable).Workspace.ExecuteSQL("COMMIT");

            }


        }

        /// <summary>
        /// Method is used to log error in staging table if not valid SAP record
        /// </summary>
        /// <param name="changedRow"></param>
        /// <param name="error"></param>
        /// <param name="changeType"></param>
        private void LogError(IRow changedRow, string error, ChangeType changeType)
        {
            try
            {
                DataHelper _dbHelper = new DataHelper();
                //Change type is reprocess, so we want to update the reprocess table with this asset having been processed.  The error string has to have single quotes replaced by two single
                //quotes to support inserting to the database
                string globalIDField = GlobalIDFieldName(changedRow.Table);
                int globalIDx = changedRow.Fields.FindField(globalIDField);
                string globalID = changedRow.get_Value(globalIDx).ToString().ToUpper();
                string recordID = _dbHelper.GetRecordID("");

                _dbHelper.LogError(globalID, error, recordID, changeType.ToString().Substring(0, 1));
                _dbHelper.CloseConnection();
            }
            catch (Oracle.DataAccess.Client.OracleException oex) { GISSAPIntegrator._logger.Error(oex.GetType() + " || " + oex.Number + " || " + oex.Message + " || \n" + oex.StackTrace); throw oex; }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Method is used to log error in staging table if not valid SAP record
        /// </summary>
        /// <param name="changedRow"></param>
        /// <param name="error"></param>
        /// <param name="changeType"></param>
        private void LogError(DeleteFeat changedRow, string error, ChangeType changeType)
        {
            try
            {
                DataHelper _dbHelper = new DataHelper();
                //Change type is reprocess, so we want to update the reprocess table with this asset having been processed.  The error string has to have single quotes replaced by two single
                //quotes to support inserting to the database
                //string globalIDField = GlobalIDFieldName(changedRow.Table);
                //int globalIDx = changedRow.Fields.FindField(globalIDField);
                //string globalID = changedRow.get_Value(globalIDx).ToString().ToUpper();
                string globalID = GlobalIDField(changedRow);
                string recordID = _dbHelper.GetRecordID("");

                _dbHelper.LogError(globalID, error, recordID, changeType.ToString().Substring(0, 1));
                _dbHelper.CloseConnection();
            }
            catch (Oracle.DataAccess.Client.OracleException oex) { GISSAPIntegrator._logger.Error(oex.GetType() + " || " + oex.Number + " || " + oex.Message + " || \n" + oex.StackTrace); throw oex; }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void LogError(ITable table, string error, int OID, ChangeType changeType)
        {
            try
            {
                DataHelper _dbHelper = new DataHelper();
                //Change type is reprocess, so we want to update the reprocess table with this asset having been processed.  The error string has to have single quotes replaced by two single
                //quotes to support inserting to the database
                string globalIDField = GlobalIDFieldName(table);
                int globalIDx = table.FindField(globalIDField);
                //int globalIDx = changedRow.Fields.FindField(globalIDField);
                //  table.GetRow(OID)
                //  string globalID = changedRow.get_Value(globalIDx).ToString().ToUpper();
                //  string recordID = _dbHelper.GetRecordID("");

                //   dbHelper.LogError(globalID, error, recordID, changeType.ToString().Substring(0, 1));
            }
            catch (Oracle.DataAccess.Client.OracleException oex) { GISSAPIntegrator._logger.Error(oex.GetType() + " || " + oex.Number + " || " + oex.Message + " || \n" + oex.StackTrace); throw oex; }
            catch (Exception ex)
            {
                throw ex;
            }
        }



        private static string GlobalIDFieldName(ITable table)
        {
            for (int i = 0; i < table.Fields.FieldCount; i++)
            {
                IField field = table.Fields.get_Field(i);
                if (field.Type == esriFieldType.esriFieldTypeGlobalID)
                {
                    return field.Name;
                }
            }
            return "";
        }

        private static string GlobalIDField(DeleteFeat table)
        {
            Guid guidOutput = default;
            string inputString = default;
            foreach (string field in table.fields_Old.Keys)
            {
                inputString = table.fields_Old[field];

                if (Guid.TryParse(inputString, out guidOutput))
                    return table.fields_Old[field];
            }
            return "";
        }

        public Dictionary<string, Dictionary<string, IRowData>> UpdateSettingsData(string objectClassName, IEnumerable<IRow> changedRows)
        {
            return UpdateSettingsData(objectClassName, changedRows, LogMethodPlaceHolder);
        }

        /// <summary>
        /// UpdateSettingsData
        /// </summary>
        /// <param name="objectClassName"></param>
        /// <param name="changedRows"></param>
        /// <returns></returns>
        public Dictionary<string, Dictionary<string, IRowData>> UpdateSettingsData(string objectClassName, IEnumerable<IRow> changedRows, Action<string> logMethod)
        {
            objectClassName = objectClassName.Substring(objectClassName.IndexOf(".") + 1);

            List<TrackedClass> trackedClasses = _systemMapper.TrackedClasses.FindAll(tc => tc.SourceClass == objectClassName);

            //if (trackedClass != null)
            foreach (TrackedClass trackedClass in trackedClasses)
            {
                if (trackedClass != null)
                {
                    AssetOIDAndProcessTimeByClass[trackedClass.OutName + trackedClass.SourceClass] = new Dictionary<int, double>();
                    trackedClass.ProcessedRowOIDs = new List<int>();
                    string sourceName = trackedClass.QualifiedSourceClass;

                    IFeatureWorkspace sourceFeatWorkspace = (IFeatureWorkspace)_editVersion;
                    trackedClass.SourceTable = sourceFeatWorkspace.OpenTable(sourceName);

                    IFeatureWorkspace targetFeatWorkspace = (IFeatureWorkspace)_targetVersion;
                    trackedClass.TargetTable = targetFeatWorkspace.OpenTable(sourceName);

                    ProcessRows(trackedClass, changedRows, ChangeType.Update);

                    ProcessTrackedClassAssetIDandRowData(trackedClass, null);
                }
            }

            return _systemMapper.AssetIDandRowDataByOutName;
        }

        #endregion

        public enum Processed
        {
            NotProcessed = 2,
            SuccessfullyProcessed = 1,
            ErrorDuringProcessing = 3
        }



    }


    /// <summary>
    /// Enum for processed flag implemented in EDGIS ReArch change detection -v1t8
    /// </summary>
    public class ProcessFlag
    {
        // Processed flag in all interfaces should be updated properly :(P- GIS Processed/E-GIS Error/T-In Transition/F-Failed/D- Done)

        public static string GISProcessed { get; } = "P";
        public static string GISError { get; } = "E";
        public static string InTransition { get; } = "T";
        public static string Failed { get; } = "F";
        public static string Completed { get; } = "D";

    }
}
