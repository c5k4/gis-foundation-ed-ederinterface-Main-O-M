using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Configuration;
using System.Text.RegularExpressions;
using System.Runtime.InteropServices;

using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using System.Collections;
using Miner.Interop;
using Miner.Geodatabase.Edit;
using Miner.Interop.Process;
using Oracle.DataAccess.Client;
using System.Data.OleDb;
using Miner.Geodatabase;
using PGE_DBPasswordManagement;

namespace PGE.BatchApplication.ConductorInTrench
{
    public class MainClass
    {
        public static DataTable _dtConduitSubtypeDuctBankData = null;
        public static DataTable _dtconductorExceptions = null;
        public static DataTable _updated_PriUG = null;
        public static DataTable _dtToSaveFinalData = null;
        public static DataTable _dtCondcutorWithCodes = null;

        public static List<double> _list_LineFractions = null;
        public static List<int> _listintersectingLineFeatureOIDs = null;
        public static List<int> _listintersectingAllLineFeatureOIDs = null;
        public static List<string> _listConductorCodesToExclude = null;

        public static string _databaseTableName_Exceptions = null;
        public static string _databaseTableName_Merge = null;
        public static string _queryToGetConduitSystemDuctBankData = null;
        public static string _queryToGetChangeDetection = null;
        public static string _queryToGetWebrManualTable = null;
        public static string _processtype = null;
        public static string _exception = string.Empty;
        public static string _queryToGetChangedRecordsBasedOnCounty = string.Empty;

        public static double _intConductorLengthToCalculateParallelLengthIsGreaterThanCutOffLength = 0;
        public static double _searchDistance = 0;

        public static int _numberOfPointsToBeConsidered = 0;
        public static int _cutoffLengthConductorToExclude = 0;
        public static int _filledduct_Value = 0;      
        
        public static bool _bLogNeededWhileExecuting = false;  
        public static bool _bRunProcessFromQueryToGetRecords = false;
        public static bool _bRunProcessFromOIDsToGetRecords = false;
        public static bool _boverwriteFilledductValue = false;
        public static bool _bisRunningProcessForChanges = false;
      

        /// <summary>
        /// This function is the main entry point for this class and called from Program.cs to start the process
        /// </summary>
        /// <param name="argInputOIDs"></param>
        /// <returns></returns>
        public static bool StartProcess(string argProcess)
        {
            IMMPxApplication _pxApplication;                     
            bool bOperaionSuccess = true;
            string strConnString_igpciteditor = null;
            string strConnString_pgedata = null;           
            IWorkspace workspace_edgis = null;
            IFeatureClass priUGFeatureClass = null;
            IFeatureClass fc_fifty_ScaleBoundary = null;
            DataTable allRequiredConductors = null;
            int iCount_ToBeUpdated = 0;
            bool final_table_update = false;
            string strVersionName = null;
            string tableNameToSaveFinalData = null;
            DataRow pRow = null;
            DataTable Verdiff_PriUG = null;
            bool bUpdateStatus = false;          
            try
            {
                bOperaionSuccess = PreRequiste();
                _pxApplication = new PxApplicationClass();
                if (!bOperaionSuccess)
                {
                    Common._log.Info("Exiting the process.");
                    return false;
                }

                Common._log.Info("Connection string used in process ." + ReadConfigurations.ConnString_EDWorkSpace);
                string connection_string = ReadConfigurations.ConnString_EDWorkSpace;
                workspace_edgis = GetWorkspace(connection_string);

                Common._log.Info("Workspace checked out successfully.");

                if (workspace_edgis == null)
                {
                    Common._log.Error("Workspace could not be retreived for Connection : " + ReadConfigurations.GetValue(ReadConfigurations.ConnString_EDWorkSpace));
                    Common._log.Info("Exiting the Process.");
                    return false;
                }

                bool bReadConfigValuesAndInitializesStaticVariables = ReadConfigValues();

                if (!bReadConfigValuesAndInitializesStaticVariables)
                {
                    Common._log.Info("Error in retreiving configuration values.");
                }

                if (MainClass._processtype == ReadConfigurations.process_type_SCHEDULE)
                {
                    strConnString_pgedata =ReadConfigurations.ConnectionString_pgedata;
                    strConnString_igpciteditor = ReadConfigurations.ConnectionString_igpciteditor;
                    priUGFeatureClass = ((IFeatureWorkspace)workspace_edgis).OpenFeatureClass(ReadConfigurations.FC_PRIUGCONDUCTOR);
                    fc_fifty_ScaleBoundary = ((IFeatureWorkspace)workspace_edgis).OpenFeatureClass(ReadConfigurations.FC_fifty_Scale_Boundary);

                    if (argProcess == ReadConfigurations.process_DATAUPDATE)
                    {
                        if (MainClass._bisRunningProcessForChanges)
                        {
                            _queryToGetChangedRecordsBasedOnCounty = "select * from " + ConfigurationManager.AppSettings["tableNameToFindChanges"] + MainClass._queryToGetChangedRecordsBasedOnCounty;
                            Verdiff_PriUG = DBHelper.GetDataTable(strConnString_pgedata, _queryToGetChangedRecordsBasedOnCounty);

                            if (Verdiff_PriUG != null)
                            {
                                Common._log.Info("Primary UG Conductors found from query : " + _queryToGetChangedRecordsBasedOnCounty +" count is :"+ Verdiff_PriUG.Rows.Count);
                            }                          
                           
                            if (Verdiff_PriUG.Rows.Count >= 1)
                            {
                                //Calculate filledduct for changed conductors - Start
                                allRequiredConductors = GetAllPrimaryConductorsAfterBuffer_ForChangedConductors(workspace_edgis, priUGFeatureClass, Verdiff_PriUG, MainClass._bisRunningProcessForChanges);

                                if (allRequiredConductors != null)
                                {
                                    Common._log.Info("Primary UG Conductors found from buffer of all conductors in TABLE PGEDATA.CIT_PGE_CHANGED_PRIUGCONDUCTOR :" + allRequiredConductors.Rows.Count);
                                }

                                allRequiredConductors.Columns.Add(ReadConfigurations.col_FILLEDDUCT, typeof(int));
                                allRequiredConductors.Columns.Add(ReadConfigurations.col_Exclusion, typeof(string));
                                allRequiredConductors.Columns.Add(ReadConfigurations.col_Is50Scale, typeof(string));
                                allRequiredConductors.Columns.Add(ReadConfigurations.col_OIDS_Needed, typeof(string));
                                allRequiredConductors.Columns.Add(ReadConfigurations.col_OIDS_All, typeof(string));
                                allRequiredConductors.Columns.Add(ReadConfigurations.col_CONDUITTYPE, typeof(string));
                                //allRequiredConductors.Columns.Add(ReadConfigurations.col_STATUS, typeof(string));
                                //allRequiredConductors.Columns.Add(ReadConfigurations.col_PROCESSED_ON, typeof(DateTime));

                                Bulk_Process.ProceccesRecords(ref allRequiredConductors, ref priUGFeatureClass, fc_fifty_ScaleBoundary, null, string.Empty);

                                //Storing all primary UG conductors in database table 
                                Common._log.Info("Bulk copying the calculated records to database table : " + MainClass._databaseTableName_Exceptions);
                                Common.BulkCopyDataFromDataTableToDatabaseTable(MainClass._dtconductorExceptions, MainClass._databaseTableName_Exceptions, strConnString_pgedata);                                                                
                            }
                            else
                            {
                                allRequiredConductors = new DataTable();
                                Common._log.Info("Primary UG Conductors are not found from Version Difference, Table Name : PGEDATA.CIT_PGE_CHANGED_PRIUGCONDUCTOR");
                            }

                            Common._log.Info("Total Number of Primary UG Conductors found for Auto Updates : " + allRequiredConductors.Rows.Count);
                            
                            DataTable allRequiredConductors_new = Common.UpdatePriUGConductorDataForChangesInCommonTable(priUGFeatureClass,ref allRequiredConductors);

                            //Storing all changed primary UG conductors in database table 
                            string tableNameToCopyData = ConfigurationManager.AppSettings["tableNameToKeepAllChangedConductors"];
                            Common._log.Info("Bulk copying the caclulated records to database table : " + tableNameToCopyData);
                            Common.BulkCopyDataFromDataTableToDatabaseTable(allRequiredConductors_new, tableNameToCopyData, strConnString_pgedata); 
                        }
                        else
                        {
                            //Steps for daily process
                            //Find conductors which have been changed - auto changed 
                            //Find all required conductors to be changed after buffer of all conductors changed 
                            //Call procedure to calculate filledduct for rule2(Duct Bank)
                            //Calculate filledduct for all required conductors
                            //Find conductors which have been manually changed                                         
                            //Compare auto changed vs manual changed conductors
                            //Send mail if any conflict found
                            //Create version
                            //Make edits in version
                            //Post version - All failed conditions and resolution with business intervention required here
                            //Update status as per Post version Success or Failed

                            //Read all config values here 
                            tableNameToSaveFinalData = ReadConfigurations.GetValue(ReadConfigurations.tableNameToSaveFinalData);

                            //Find conductors which have been changed -Start
                            //Table is present in pgedata schema so using pgedata string in this function.   
                            //Common._log.Info("Connection string to fetch data : " + strConnString_pgedata);
                            //Verdiff_PriUG = Common.schedule_ChangeDetection(strConnString_pgedata, _queryToGetChangeDetection);
                            Verdiff_PriUG = DBHelper.GetDataTable(strConnString_pgedata, _queryToGetChangeDetection);
                            //Find conductors which have been changed -End

                            if (Verdiff_PriUG != null)
                            {
                                Common._log.Info("Primary UG Conductors found from TABLE PGEDATA.CIT_PGE_CHANGED_PRIUGCONDUCTOR :" + Verdiff_PriUG.Rows.Count);
                            }

                            //Delete versions which could not be deleted after execution of last process.                      
                            //Not needed because session will be submitted to gdbm post queue
                            //Common.DeleteVersionsUsingCITTable(workspace_edgis, strConnString_edgis);

                            // Checking the previous version has been posted or not                            
                            bool bPreviousSessionPosted = VersionOperations.CheckPreviousRunSessionPosted(strConnString_pgedata, ref strVersionName);

                            if (bPreviousSessionPosted)
                            {
                                if (!string.IsNullOrEmpty(strVersionName))
                                {
                                    Common.MakeChangesForCitVersionTable(true, strVersionName, "UPDATE");

                                    Common._log.Info("Data succesfully UPDATED AND SESSION posted. Session Name : " + strVersionName);
                                    //Update status in WEBR table CIT_MANUAL_UPDATE and CIT_PGE_AUTO_UPDATES here
                                    //UpdateStagingtableStatusAfterPostingVersion();

                                    //Changed Primary UG conductor table will be claered after posting the version successfully                                  
                                    string tablename = ConfigurationManager.AppSettings["tableNameChangeDetection"];
                                    string query = "delete from " + tablename;
                                    DBHelper.RunGivenQueryOnTable(tablename, query);
                                    Common._log.Info("Ran query :" + query + " on table : " + tablename);
                                }
                                else
                                {
                                    Common._log.Info("Not running further queries for version table and staging status table for previous session because previous session name could not be found from table.");
                                    string tablename = ConfigurationManager.AppSettings["tableNameChangeDetection"];
                                    string query = "delete from " + tablename;
                                    DBHelper.RunGivenQueryOnTable(tablename, query);
                                    Common._log.Info("Deleted all records from table : " + tablename);
                                }
                            }
                            else
                            {
                                Common._log.Info("Previous session, session name = (" + strVersionName + ") is not posted");

                                string tablename = ConfigurationManager.AppSettings["tableNameChangeDetection"];
                                string query = "delete from " + tablename;
                                DBHelper.RunGivenQueryOnTable(tablename, query);
                                Common._log.Info("Deleted all records from table : " + tablename);
                            }

                            fc_fifty_ScaleBoundary = ((IFeatureWorkspace)workspace_edgis).OpenFeatureClass(ReadConfigurations.FC_fifty_Scale_Boundary);
                            if (Verdiff_PriUG.Rows.Count >= 1)
                            {
                                //Calculate filledduct for changed conductors - Start

                                allRequiredConductors = GetAllPrimaryConductorsAfterBuffer(workspace_edgis, priUGFeatureClass, Verdiff_PriUG,false);

                                if (allRequiredConductors != null)
                                {
                                    Common._log.Info("Primary UG Conductors found from buffer of all conductors in TABLE PGEDATA.CIT_PGE_CHANGED_PRIUGCONDUCTOR :" + allRequiredConductors.Rows.Count);
                                }

                                allRequiredConductors.Columns.Add(ReadConfigurations.col_FILLEDDUCT, typeof(int));
                                allRequiredConductors.Columns.Add(ReadConfigurations.col_Exclusion, typeof(string));
                                allRequiredConductors.Columns.Add(ReadConfigurations.col_Is50Scale, typeof(string));
                                allRequiredConductors.Columns.Add(ReadConfigurations.col_OIDS_Needed, typeof(string));
                                allRequiredConductors.Columns.Add(ReadConfigurations.col_OIDS_All, typeof(string));
                                allRequiredConductors.Columns.Add(ReadConfigurations.col_CONDUITTYPE, typeof(string));
                                allRequiredConductors.Columns.Add(ReadConfigurations.col_STATUS, typeof(string));
                                allRequiredConductors.Columns.Add(ReadConfigurations.col_PROCESSED_ON, typeof(DateTime));

                                Bulk_Process.ProceccesRecords(ref allRequiredConductors, ref priUGFeatureClass, fc_fifty_ScaleBoundary, null, string.Empty);

                                //Storing all primary UG conductors caught as exceptions in database table 
                                Common._log.Info("Bulk copying the exceptions records to database table : " + MainClass._databaseTableName_Exceptions);
                                Common.BulkCopyDataFromDataTableToDatabaseTable(MainClass._dtconductorExceptions, MainClass._databaseTableName_Exceptions, strConnString_pgedata);
                                
                                for (int iCount = 0; iCount < allRequiredConductors.Rows.Count; iCount++)
                                {
                                    try
                                    {
                                        pRow = allRequiredConductors.Rows[iCount];
                                        pRow[ReadConfigurations.col_STATUS] = ReadConfigurations.val_New;
                                        pRow[ReadConfigurations.col_PROCESSED_ON] = DateTime.Today.Date;
                                    }
                                    catch (Exception exp)
                                    {
                                        Common._log.Error("Exception : " + exp.Message + " |  Stack Trace | " + exp.StackTrace+" at "+exp.ToString());
                                    }
                                }
                            }
                            else
                            {
                                allRequiredConductors = new DataTable();
                                Common._log.Info("Primary UG Conductors are not found from Version Difference, Table Name : PGEDATA.CIT_PGE_CHANGED_PRIUGCONDUCTOR");
                            }

                            Common._log.Info("Total Number of Primary UG Conductors found for Auto Updates : " + allRequiredConductors.Rows.Count);

                            //Step 2.Calculate filledduct for changed conductors - End

                            //fetching manually changed primary underground conductor *Start
                            _queryToGetWebrManualTable = _queryToGetWebrManualTable + " where status='New'";
                            Common._log.Info("Query to get manually changed record from WEBR : " + _queryToGetWebrManualTable);
                            Common._dtmanualPriUG = Common.GetManuallyChangedPrimaryUGConductors(_queryToGetWebrManualTable, priUGFeatureClass);
                            //fetching manually changed primary underground conductor *End

                            //comparison code auto and manual - Start                     
                            if (Common._dtmanualPriUG.Rows.Count >= 1)
                            {
                                Common._log.Info("All Primary UG Conductors found in table for Manual Updates(with all status) : " + Common._dtmanualPriUG.Rows.Count);
                                Common._log.Info("Running comparison between auto updated and manually updated conductors.");
                                iCount_ToBeUpdated = Common.comparisonCode(allRequiredConductors);
                            }
                            //comparison code auto and manual - End                   

                            //ACTION ITEM - Below part should come in sucessfull version post or add a status column 
                            //Step 4 Send mail if any conflict found-Start
                            if (Common._manualGlobalExisting.Count >= 1)
                            {
                                Common.Save_ConflictInformation(priUGFeatureClass, strConnString_igpciteditor, strConnString_pgedata);
                            }
                            //Step 4 Send mail if any conflict found - End

                            //Combining the webr and auto table into one final table and comparing the filledduct value with existing filledduct value
                            ComparetheCalculatedFilledductWithExistingFilledduct(priUGFeatureClass, allRequiredConductors, "AUTO");
                            ComparetheCalculatedFilledductWithExistingFilledduct(priUGFeatureClass, Common._dtmanualPriUG, "MANUAL");
                            // Take this below whole part to another function -End

                            Common._log.Info("Total number of features found for Auto changed conductors: " + allRequiredConductors.Rows.Count);
                            Common._log.Info("Total number of features found for WEBR changed conductors: " + Common._dtmanualPriUG.Select("STATUS = 'New'").Length);
                            Common._log.Info("Total number of features to be updated after merge operation : " + _dtToSaveFinalData.Rows.Count);

                            if (_dtToSaveFinalData.Rows.Count == 0)
                            {
                                Common._log.Info("After comparison between auto changed and manually changed conductors, no primary UG conductor found to be updated.");
                                Common._log.Info("No further processing is required. No version will be created.");

                                //Clearing all the tables here if there is nothing to update
                                //Changed primary UG conductor table will be truncated there 
                                string tablename = ConfigurationManager.AppSettings["tableNameChangeDetection"];
                                string query = "delete from " + tablename;
                                DBHelper.RunGivenQueryOnTable(tablename, query);
                            }

                            if (_dtToSaveFinalData.Rows.Count > 0)
                            {
                                final_table_update = Common.BulkCopyDataFromDataTableToDatabaseTable_PGEDATA(_dtToSaveFinalData, tableNameToSaveFinalData);
                                //This function will export results to log file 
                                ExportResults(_dtToSaveFinalData);
                            }

                            //If condition in case there is nothing to update *start
                            if (_dtToSaveFinalData.Rows.Count >= 1)
                            {
                                bUpdateStatus = DataUpdateAndVersionOperations(workspace_edgis,ref strVersionName);

                                if (bUpdateStatus)
                                {
                                    Common._log.Info("Updates done successfully in version : " + strVersionName);

                                    Common._log.Info("Submitting sersion : " + strVersionName+" to GDBM posting queue.");
                                    bool bSessionSubmitToGDBM = VersionOperations.SubmitSessionToGDBM(VersionOperations._mmSession, workspace_edgis);
                                    if (bSessionSubmitToGDBM)
                                    {
                                        Common._log.Info("Session " + strVersionName+" submitted successfully to GDBM.");
                                        //Update status in WEBR table CIT_MANUAL_UPDATE and CIT_PGE_AUTO_UPDATES here
                                        UpdateStagingtableStatusAfterPostingVersion();
                                    }
                                    else
                                    {
                                        Common._log.Info("There is some error while submitting the version to GDBM : " + strVersionName);
                                        // Update Status in table CIT_PGE_AUTO_UPDATES to Failed for all new records
                                        string tablename = ConfigurationManager.AppSettings["tableNameToSaveFinalData"];
                                        string query = "UPDATE " + tablename + " SET STATUS='Failed',PROCESSED_ON= systimestamp where STATUS='New'";
                                        DBHelper.RunGivenQueryOnTable(tablename, query);                                    
                                    }
                                }
                                else
                                {
                                    Common._log.Info("There is some error while saving the features in version : " + strVersionName);
                                    // Update Status in table CIT_PGE_AUTO_UPDATES to Failed for all new records
                                    string tablename = ConfigurationManager.AppSettings["tableNameToSaveFinalData"];
                                    string query = "UPDATE " + tablename + " SET STATUS='Failed',PROCESSED_ON= systimestamp where STATUS='New'";
                                    DBHelper.RunGivenQueryOnTable(tablename, query);
                                }
                            }
                            else
                            {
                                Common._log.Info("Nothing to update, exiting the process.");
                            }    
                        }                       
                    } 

                    //Below part should be called after Version POST - Wheather successful or failed 
                    //Writing the code for configuring the email sending duration (weekly)                 
                    Common.Check_and_SendMail(strConnString_pgedata, priUGFeatureClass);
                }
            }
            catch (Exception exp)
            {
                bOperaionSuccess = false;
                Common._log.Error("Exception : " + exp.Message + " |  Stack Trace | " + exp.StackTrace+" at "+exp.ToString());
                ErrorCodeException ece = new ErrorCodeException(exp);
                Environment.ExitCode = ece.CodeNumber;
            }
            finally
            {                
                Common.CloseLicenseObject();              
            }
            return bOperaionSuccess;
        }

        /// <summary>
        /// This function read all the configuration values from config file and initializes required static variables
        /// </summary>
        /// <returns></returns>
        public static bool ReadConfigValues()
        {
            bool bAllConfigReadSuccessfully = true;
            try
            {
                //CIT_Daily_Updates- code added for creating a table for exceptions
                _dtconductorExceptions = new DataTable();
                _dtconductorExceptions.Columns.Add(ReadConfigurations.col_OBJECTID, typeof(int));
                _dtconductorExceptions.Columns.Add(ReadConfigurations.col_COMMENTS, typeof(string));
                
                _updated_PriUG = new DataTable();
                _updated_PriUG.Columns.Add(ReadConfigurations.col_OBJECTID, typeof(int));
                _updated_PriUG.Columns.Add(ReadConfigurations.col_GLOBALID, typeof(string));               
                _updated_PriUG.Columns.Add(ReadConfigurations.col_FILLEDDUCT, typeof(int));
                _updated_PriUG.Columns.Add(ReadConfigurations.col_STATUS, typeof(string));
                _updated_PriUG.Columns.Add(ReadConfigurations.col_PROCESSED_ON, typeof(DateTime));
            
                _dtToSaveFinalData = new DataTable();
                _dtToSaveFinalData.Columns.Add(ReadConfigurations.col_OBJECTID, typeof(int));
                _dtToSaveFinalData.Columns.Add(ReadConfigurations.col_GLOBALID, typeof(string));
                _dtToSaveFinalData.Columns.Add(ReadConfigurations.col_CIRCUITID, typeof(string));
                _dtToSaveFinalData.Columns.Add(ReadConfigurations.col_LOCALOFFICEID, typeof(string));
                _dtToSaveFinalData.Columns.Add(ReadConfigurations.col_SUBTYPECD, typeof(int));
                _dtToSaveFinalData.Columns.Add(ReadConfigurations.col_FILLEDDUCT, typeof(int));
                _dtToSaveFinalData.Columns.Add(ReadConfigurations.col_Exclusion, typeof(string));
                _dtToSaveFinalData.Columns.Add(ReadConfigurations.col_Is50Scale, typeof(string));
                _dtToSaveFinalData.Columns.Add(ReadConfigurations.col_OIDS_Needed, typeof(string));
                _dtToSaveFinalData.Columns.Add(ReadConfigurations.col_OIDS_All, typeof(string));
                _dtToSaveFinalData.Columns.Add(ReadConfigurations.col_CONDUITTYPE, typeof(string));
                _dtToSaveFinalData.Columns.Add(ReadConfigurations.col_STATUS, typeof(string));
                _dtToSaveFinalData.Columns.Add(ReadConfigurations.col_REQUESTED_ON, typeof(DateTime));
                _dtToSaveFinalData.Columns.Add(ReadConfigurations.col_USER, typeof(string));
                _dtToSaveFinalData.Columns.Add(ReadConfigurations.col_PROCESSED_ON, typeof(DateTime));               
            
                _searchDistance = Convert.ToInt32(ReadConfigurations.GetValue(ReadConfigurations.searchDistance));

                Common._log.Info("Search(Buffer) distance = " + _searchDistance + " Feet");

                if (Convert.ToInt32(ReadConfigurations.GetValue(ReadConfigurations.linefractions)) == 5)
                {
                    _list_LineFractions = new List<double> { 0, 0.25, 0.5, 0.75, 1 };// lineFractions = 5
                }
                else if (Convert.ToInt32(ReadConfigurations.GetValue(ReadConfigurations.linefractions)) == 9)
                {
                    _list_LineFractions = new List<double> { 0, 0.125, 0.25, 0.375, 0.5, 0.625, 0.75, 0.875, 1 }; // lineFractions = 9
                }
                else if (Convert.ToInt32(ReadConfigurations.GetValue(ReadConfigurations.linefractions)) == 11)
                {
                    _list_LineFractions = new List<double> { 0, 0.1, 0.2, 0.3, 0.4, 0.5, 0.6, 0.7, 0.8, 0.9, 1 };// lineFractions = 11
                }

                if (Convert.ToInt32(ReadConfigurations.GetValue(ReadConfigurations.linefractions)) == 5)
                {
                    Common._log.Info("list_LineFractions = new List<double> { 0, 0.25, 0.5, 0.75, 1 };// lineFractions = 5");
                }
                else if (Convert.ToInt32(ReadConfigurations.GetValue(ReadConfigurations.linefractions)) == 9)
                {
                    Common._log.Info("list_LineFractions = new List<double> { 0, 0.125, 0.25, 0.375, 0.5, 0.625, 0.75, 0.875, 1 }; // lineFractions = 9");
                }
                else if (Convert.ToInt32(ReadConfigurations.GetValue(ReadConfigurations.linefractions)) == 11)
                {
                    Common._log.Info("list_LineFractions = new List<double> { 0, 0.1, 0.2, 0.3, 0.4, 0.5, 0.6, 0.7, 0.8, 0.9, 1 };// lineFractions = 11");
                }

                _numberOfPointsToBeConsidered = Convert.ToInt32(ReadConfigurations.GetValue(ReadConfigurations.numberOfPointsToBeConsidered));

                Common._log.Info("NumberOfPointsToBeConsidered = " + _numberOfPointsToBeConsidered);

                _cutoffLengthConductorToExclude = Convert.ToInt32(ReadConfigurations.GetValue(ReadConfigurations.cutoffLengthConductorToExclude));

                Common._log.Info("cutoffLengthConductorToExclude = " + _cutoffLengthConductorToExclude + " Feet");

                _bLogNeededWhileExecuting = Convert.ToBoolean(ReadConfigurations.GetValue(ReadConfigurations.logNeededWhileExecuting));

                Common._log.Info("logNeededWhileExecuting = " + _bLogNeededWhileExecuting);

                _boverwriteFilledductValue = Convert.ToBoolean(ReadConfigurations.GetValue(ReadConfigurations.overwriteFilledductValue));

                Common._log.Info("overwriteFilledductValue = " + _boverwriteFilledductValue);   

                _queryToGetConduitSystemDuctBankData = ReadConfigurations.GetValue(ReadConfigurations.queryToGetConduitSystemDuctBankData);

                Common._log.Info("Query To Get ConduitSystem DuctBank Data = " + _queryToGetConduitSystemDuctBankData);
             

                _queryToGetWebrManualTable = "select * from " + ReadConfigurations.GetValue(ReadConfigurations.tableNameForManualUpdates);
                Common._log.Info("Query To Get WEBR Data = " + _queryToGetWebrManualTable);
                
                _queryToGetChangeDetection = "select * from "+ReadConfigurations.GetValue(ReadConfigurations.tableNameChangeDetection);
                Common._log.Info("Query To Get Change Detection Data = " + _queryToGetChangeDetection); 
                
                _databaseTableName_Exceptions = ReadConfigurations.GetValue(ReadConfigurations.tableNameToSavePriUGException);
                Common._log.Info("Database tablename for exceptions to store data = " + _databaseTableName_Exceptions);
               
                _intConductorLengthToCalculateParallelLengthIsGreaterThanCutOffLength = Convert.ToInt32(ReadConfigurations.GetValue(ReadConfigurations.ConductorLengthToCalculateParallelLengthIsGreaterThanCutOffLength));
                Common._log.Info("Conductor Length To Calculate Parallel Length Less Than CutOff Length = " + _intConductorLengthToCalculateParallelLengthIsGreaterThanCutOffLength);
                                
                _listConductorCodesToExclude = new List<string>();                               

                _listintersectingLineFeatureOIDs = new List<int>();
                _listintersectingAllLineFeatureOIDs = new List<int>();

                _dtCondcutorWithCodes = new DataTable();
                _dtCondcutorWithCodes.Columns.Add(ReadConfigurations.col_OBJECTID);
                _dtCondcutorWithCodes.Columns.Add(ReadConfigurations.col_WEIGHT);
            }
            catch (Exception exp)
            {
                Common._log.Error("Exception : " + exp.Message + " |  Stack Trace | " + exp.StackTrace+" at "+exp.ToString());
                bAllConfigReadSuccessfully = false;
                ErrorCodeException ece = new ErrorCodeException(exp);
                Environment.ExitCode = ece.CodeNumber;
            }
            return bAllConfigReadSuccessfully;
        }      

        /// <summary>
        /// This function check for conductors and find filled duct for these conductors
        /// </summary>
        /// <param name="argWorkspace"></param>
        /// <param name="argInputOIDs"></param>
        /// <returns></returns>
        private static void ProcessPriUGCond_SpatialAnalysis(IFeatureClass argpriUGFeatureClass,IFeatureClass argfc_fifty_ScaleBoundary,int oid_priUG)
        {
            string strExclusion = string.Empty;
            string strIs50Scale = string.Empty;
            string strConduitType = string.Empty;
            try
            {
                if (!string.IsNullOrEmpty(oid_priUG + ""))
                {
                    ProcessPriUGCond_SpatialAnalysis_ForGivenQuery(argpriUGFeatureClass, argfc_fifty_ScaleBoundary, "OBJECTID IN (" + oid_priUG + ")", oid_priUG, MainClass._processtype, null, true, ref strExclusion, ref strIs50Scale, ref strConduitType);                 
                }
                //Call a function here to export the results 
                //ExportResults(dtconductorAllDetails);
                //ExportResults(dtconductorFILLEDDUCT);               
                //ExportResults(dtconductorExceptions);
                //Call a function here to send datatable to database , insert in table based on table name from config
                //Common.BulkCopyDataFromDataTableToDatabaseTable(dtconductorAllDetails, databaseTableName);
                //Common.BulkCopyDataFromDataTableToDatabaseTable(dtconductorFILLEDDUCT, databaseTableName_CondFilledDuct);
                //Common.BulkCopyDataFromDataTableToDatabaseTable(dtconductorExceptions, databaseTableName_Exceptions);                
            }
            catch (Exception exp)
            {
                Common._log.Error("Exception : " + exp.Message + " |  Stack Trace | " + exp.StackTrace+" at "+exp.ToString());
            }  
        }

        /// <summary>
        /// This function check for conductors falling in business rule set 1 and find filled duct for these conductors (for specific conductors found by a query)
        /// </summary>
        /// <param name="argpriUGFeatureClass"></param>
        /// <param name="argInputquery"></param>
        public static int ProcessPriUGCond_SpatialAnalysis_ForGivenQuery(IFeatureClass argpriUGFeatureClass, IFeatureClass argfc_fifty_ScaleBoundary, string argInputquery, int argPriUGOID, string argStrProcessType, DataTable argdtAll50ScaleReocrds, bool argbCheckFor50Scale, ref string strExclusion, ref string strIs50Scale, ref string argStrtypeOfConduit)
        {
            IQueryFilter pQueryFilter = null;
            IFeatureCursor pFeatCursor = null;
            IFeature pFeaturePriUGCond = null;
            bool bIs50Scale = false;             
            int filledduct = 0;          
            bool bProcessIsBulk = false;          
            try
            {
                if (_listConductorCodesToExclude.Count == 0)
                {
                    // Get Conductor Codes 
                    string strConnectionString_pgedata = ReadConfigurations.ConnectionString_pgedata;
                    MainClass.CaptureAllConductorCodesToExclude(strConnectionString_pgedata);                
                }

                if (MainClass._processtype == "BULK")
                {
                    bProcessIsBulk = true;
                }

                pQueryFilter = new QueryFilterClass();
                pQueryFilter.WhereClause = argInputquery;
                pQueryFilter.SubFields = ReadConfigurations.subFields;
                pFeatCursor = argpriUGFeatureClass.Search(pQueryFilter, false);
                pFeaturePriUGCond = pFeatCursor.NextFeature();
               
                //Is this for testing purpose? - YES 
                //pFeaturePriUGCond = argpriUGFeatureClass.GetFeature(14114515); // Duct Bank - 14011258,14037690, Conduit - 6548322
                if (pFeaturePriUGCond != null)
                {
                    try
                    {
                        //Checking whether conductor is in 50 scale boundary or not 
                        if (argbCheckFor50Scale)
                        {                           
                            bIs50Scale = Check50Scale(argfc_fifty_ScaleBoundary, argdtAll50ScaleReocrds, pFeaturePriUGCond, bProcessIsBulk);

                            if (bIs50Scale)
                            {
                                strIs50Scale = "YES";
                            }
                        }

                        //Checking Conduit System type
                        argStrtypeOfConduit = GetTypeOfConduit(pFeaturePriUGCond, bProcessIsBulk,false);

                        //Getting the filledduct value
                        filledduct = GetFilledductValue(argpriUGFeatureClass, pFeaturePriUGCond, argStrtypeOfConduit, ref strExclusion);                       

                        if (bProcessIsBulk)
                        {
                            if (strExclusion == ReadConfigurations.strExclusionBasedOnConductorCodes || strExclusion == ReadConfigurations.strExclusionBasedOnStatus)
                            {
                                filledduct--;                               
                            }
                        }
                        else
                        {
                            if (strExclusion == ReadConfigurations.strExclusionBasedOnStatus)
                            {
                                filledduct = 0;
                            }

                            if (argStrtypeOfConduit == "0" || argStrtypeOfConduit == "2" || argStrtypeOfConduit == "3")
                            {
                                if (strExclusion == ReadConfigurations.strExclusionBasedOnConductorCodes)
                                {
                                    filledduct--;
                                }
                            }
                        }

                        if (_bLogNeededWhileExecuting)
                            Common._log.Info("For pri UG conductor OID " + pFeaturePriUGCond.OID + " 50 Scale : " + bIs50Scale + " Conduit Type :" + argStrtypeOfConduit + "Exclusion :" + strExclusion + " Filledduct=" + filledduct);

                        //code for updating table
                        if (!bProcessIsBulk)
                        {
                            PrcessForDailyUpdates(pFeaturePriUGCond, filledduct);
                        }
                    }
                    catch (Exception exp)
                    {
                        Common._log.Error("Exception : " + "Exception : " + exp.Message + " |  Stack Trace | " + exp.StackTrace+" at "+exp.ToString());
                        Common._log.Error("Query used : " + argInputquery);
                        MainClass._exception = exp.Message;
                    }                                    
                }
            }
            catch (Exception exp)
            {
                Common._log.Error("Exception : " + exp.Message + " |  Stack Trace | " + exp.StackTrace+" at "+exp.ToString());
                MainClass._exception = exp.Message;
            }
            finally
            {
                if (pFeatCursor != null && Marshal.IsComObject(pFeatCursor))
                {
                    Marshal.FinalReleaseComObject(pFeatCursor);
                    pFeatCursor = null;
                }              

                if (pQueryFilter != null && Marshal.IsComObject(pQueryFilter))
                {
                    Marshal.FinalReleaseComObject(pQueryFilter);
                    pQueryFilter = null;
                }

                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
            return filledduct;
        }

        /// <summary>
        /// This function provide a List of other primary UG conductors which take part in FilledDuct calculation with respect to given conductor
        /// </summary>
        /// <param name="argFeaturePriUGCond"></param>
        /// <param name="argcountAdjacentFeatures"></param>
        /// <returns></returns>
        private static void ProcessPriUGCond_ForSingleConductor(IFeature argFeaturePriUGCond, ref int argcountAdjacentFeatures,bool argbIsThreePhaseConductor,ref string argStrExclusion)
        {
            IFeatureClass priUGFeatureClass = null;
            IFeature pFeatureOutPut = null;
            ISpatialFilter spatialFilter = null;
            ITopologicalOperator topoOperator = null;
            IFeatureCursor featureCursor = null;
            StringBuilder sboutput = new StringBuilder();
            string output = "";
            string comment = string.Empty;            
            //bool IsConductorLengthToCalculateParallelLengthIsGreaterThanCutOffLength = false;
            bool bTwoConductorsAreSpatiallyRelated = false;
            int conductorWeight = 0;          
            try
            {
                argStrExclusion ="";

                //Checking the conductor length , if conductor length <cut off length (100 Feet) , Filledduct will be 1
                if (IsConductorLessThanCutOffLength(argFeaturePriUGCond, _cutoffLengthConductorToExclude))
                {
                    comment = "Calculating FILLEDDUCT for conductor with OID:" + argFeaturePriUGCond.OID + " and setting filledduct=1 because it's length is less than cut off length.";
                    if (_bLogNeededWhileExecuting) Common._log.Info(comment);
                    argStrExclusion = ReadConfigurations.strExclusionBasedOnLength;                    
                    return;
                }

                //Checking the conductor based on Exclusion Code List ,Filledduct will be 1
                if (IsConductorNeedsToBeExcludedBasedOnConductorCodes(argFeaturePriUGCond))
                {
                    comment = "Calculating FILLEDDUCT for conductor with OID:" + argFeaturePriUGCond.OID + " and found that it is excluded based on conductor codes.";
                    if (_bLogNeededWhileExecuting) Common._log.Info(comment);
                    argStrExclusion = ReadConfigurations.strExclusionBasedOnConductorCodes;                    
                    //return;
                }

                 //Checking the conductor based on status ,Filledduct will be 0/Null
                if (IsConductorNotInService(argFeaturePriUGCond))
                {
                    comment = "Calculating FILLEDDUCT for conductor with OID:" + argFeaturePriUGCond.OID + " and setting filledduct=0 because it is excluded based on status.";                    
                    if (_bLogNeededWhileExecuting)
                        Common._log.Info(comment);
                    argStrExclusion = ReadConfigurations.strExclusionBasedOnStatus;                    
                    //return;
                }              

                priUGFeatureClass = (IFeatureClass)argFeaturePriUGCond.Class;

                spatialFilter = new SpatialFilterClass();
                topoOperator = (ITopologicalOperator)argFeaturePriUGCond.Shape;
                spatialFilter.Geometry = topoOperator.Buffer(_searchDistance);
                spatialFilter.GeometryField = priUGFeatureClass.ShapeFieldName;
                spatialFilter.SpatialRel = esriSpatialRelEnum.esriSpatialRelIntersects;
                spatialFilter.SubFields = ReadConfigurations.subFields;

                featureCursor = priUGFeatureClass.Search(spatialFilter, false);
                pFeatureOutPut = featureCursor.NextFeature();

                while (pFeatureOutPut != null)
                {
                    if (pFeatureOutPut.OID != argFeaturePriUGCond.OID)
                    {
                        //IsConductorLengthToCalculateParallelLengthIsGreaterThanCutOffLength = false;                                                

                        //Check here if the conductor is less than 100 Feet , then don't consider
                        if (IsConductorLessThanCutOffLength(pFeatureOutPut, _cutoffLengthConductorToExclude))
                        {
                            comment = "Calculating FILLEDDUCT for conductor with OID:" + argFeaturePriUGCond.OID + " and Not considering conductor with OID :" + pFeatureOutPut.OID + " because it's length is less than cut off length.";
                            if (_bLogNeededWhileExecuting) Common._log.Info(comment);
                            //dtconductorExceptions.Rows.Add(pFeatureOutPut.OID, comment);
                            pFeatureOutPut = featureCursor.NextFeature();
                            continue;
                        }

                        //Check here if the main conductor is 3 phase and reference conductor is not 3 phase 
                        if (argbIsThreePhaseConductor && IsConductorNotThreePhase(pFeatureOutPut))
                        {
                            comment = "Calculating FILLEDDUCT for conductor with OID:" + argFeaturePriUGCond.OID + " and Not considering conductor with OID :" + pFeatureOutPut.OID + " because it's not a 3 phase conductor.";
                            if (_bLogNeededWhileExecuting) Common._log.Info(comment);
                            //dtconductorExceptions.Rows.Add(pFeatureOutPut.OID, comment);
                            pFeatureOutPut = featureCursor.NextFeature();
                            continue;
                        }

                        //Check here if the main conductor and reference conductor are spatially related
                        bTwoConductorsAreSpatiallyRelated = CheckForOverlap(argFeaturePriUGCond.OID,spatialFilter.Geometry, pFeatureOutPut.Shape, pFeatureOutPut.OID);

                        if (bTwoConductorsAreSpatiallyRelated)
                        {
                            //Check here if the reference conductor is not in service or excluded based on conductor codes 
                            if (IsConductorNeedsToBeExcludedBasedOnConductorCodes(pFeatureOutPut) || IsConductorNotInService(pFeatureOutPut))
                            {
                                comment = "Calculating FILLEDDUCT for conductor with OID:" + argFeaturePriUGCond.OID + " and Not considering conductor with OID :" + pFeatureOutPut.OID + " because it's excluded based on conductor codes or status.";
                                if (_bLogNeededWhileExecuting) Common._log.Info(comment);
                                conductorWeight = 0;
                            }
                            else
                            {
                                conductorWeight = 1;
                            }

                            if (IsConductorSubTypeNotForRuleOne(pFeatureOutPut))
                            {
                                comment = "Calculating FILLEDDUCT for conductor with OID:" + argFeaturePriUGCond.OID + " and Not considering conductor with OID :" + pFeatureOutPut.OID + " because it's conduit system subtype is not for Rule 1.";
                                if (_bLogNeededWhileExecuting) Common._log.Info(comment);                              
                                pFeatureOutPut = featureCursor.NextFeature();
                                continue;
                            }

                            argcountAdjacentFeatures++;

                            //Add these in a data table with weight and take final count based on weight = 1 only 
                            if (!MainClass._listintersectingAllLineFeatureOIDs.Contains(pFeatureOutPut.OID))
                                MainClass._listintersectingAllLineFeatureOIDs.Add(pFeatureOutPut.OID);

                            if (_dtCondcutorWithCodes.Select("OBJECTID=" + pFeatureOutPut.OID).Length == 0)
                                _dtCondcutorWithCodes.Rows.Add(pFeatureOutPut.OID, conductorWeight);
                        }                                             

                        //bTwoConductorsAreSpatiallyRelated = GetNearbyParallelPriUGConductor(priUGFeatureClass, argFeaturePriUGCond.OID, pFeatureOutPut, ref listntersectingLineFeatureOIDs, IsConductorLengthToCalculateParallelLengthIsGreaterThanCutOffLength);
                        //if (!bTwoConductorsAreSpatiallyRelated)
                        //{
                        //    // For given primary UG conductor - we checked that intersected pri UG conductor is not falling in rule(40%) now we need to check reverse means Is given primary UG conductor is occupied 40% or more by the intersected conductor.
                        //    // So taking the intersected conductor as reference now                            
                        //    IsConductorLengthToCalculateParallelLengthIsGreaterThanCutOffLength = false;

                        //    if (IsConductorLessThanCutOffLength(argFeaturePriUGCond, cutoffLengthConductorToExclude, ref IsConductorLengthToCalculateParallelLengthIsGreaterThanCutOffLength))
                        //    {                               
                        //        comment = "Calculating FILLEDDUCT for conductor with OID:" + argFeaturePriUGCond.OID + "Not considering conductor with OID :" + pFeatureOutPut.OID + " because in reverse calculation the length of main conductor is not enough.";
                        //        dtconductorExceptions.Rows.Add(pFeatureOutPut.OID, comment);
                        //        Common._log.Info(comment);
                        //        pFeatureOutPut = featureCursor.NextFeature();
                        //        continue;
                        //    }

                        //    bTwoConductorsAreSpatiallyRelated = GetNearbyParallelPriUGConductor_ForIntersectingConductor(priUGFeatureClass, pFeatureOutPut.OID, argFeaturePriUGCond, ref listntersectingLineFeatureOIDs, IsConductorLengthToCalculateParallelLengthIsGreaterThanCutOffLength);
                        //}

                    }
                    pFeatureOutPut = featureCursor.NextFeature();
                }                
            }
            catch (Exception exp)
            {
                Common._log.Error("Exception : " + exp.Message + " |  Stack Trace | " + exp.StackTrace + " Output conductor OIDs : " + output);
            }
            finally
            {
                if (featureCursor != null && Marshal.IsComObject(featureCursor))
                {
                    Marshal.FinalReleaseComObject(featureCursor);
                    featureCursor = null;
                }

                if (spatialFilter != null && Marshal.IsComObject(spatialFilter))
                {
                    Marshal.FinalReleaseComObject(spatialFilter);
                    spatialFilter = null;
                }

                if (topoOperator != null && Marshal.IsComObject(topoOperator))
                {
                    Marshal.FinalReleaseComObject(topoOperator);
                    topoOperator = null;
                }

                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
            
            return;
        }

        /// <summary>
        /// This function check the type of conduit
        /// </summary>
        /// <param name="argFeaturePriUGCond"></param>
        /// <returns></returns>
        public static string ProcessPriUGCond_GetTypeOfConduit(IFeature argFeaturePriUGCond)
        {
            IFeature pfeatConduit = null;
            string conduitSubType = null;
            List<string> listConduitSubType = null;
            try
            {
                IEnumRelationshipClass relClasses = ((IFeatureClass)argFeaturePriUGCond.Class).get_RelationshipClasses(esriRelRole.esriRelRoleDestination);
                IRelationshipClass rel = relClasses.Next();
                while (rel != null)
                {                    
                    if ( ((IDataset)rel.OriginClass).Name.ToUpper() == ReadConfigurations.FC_CONDUITSYSTEM)
                    {
                        ESRI.ArcGIS.esriSystem.ISet resultSet = rel.GetObjectsRelatedToObject(argFeaturePriUGCond);

                        // Case 1 Conductor having no related conduit system record
                        // Case 2 Conductor having single related conduit system record 
                        // Case 3 Conductor having multiple related conduit system record 
                        if (resultSet.Count == 0)
                        {
                            conduitSubType = "0";
                            break;
                        }
                        else if (resultSet.Count == 1)
                        {
                            pfeatConduit = (IFeature)resultSet.Next();
                            conduitSubType = Convert.ToString(pfeatConduit.get_Value(pfeatConduit.Fields.FindField(ReadConfigurations.col_SUBTYPECD)));
                            break;
                        }
                        else if (resultSet.Count > 1)
                        {
                            listConduitSubType = new List<string>();
                            pfeatConduit = (IFeature)resultSet.Next();

                            while (pfeatConduit != null)
                            {
                                // Check here if all the conduit subtypes are not same and return acoordingly
                                conduitSubType = Convert.ToString(pfeatConduit.get_Value(pfeatConduit.Fields.FindField(ReadConfigurations.col_SUBTYPECD)));
                                listConduitSubType.Add(conduitSubType);
                                pfeatConduit = (IFeature)resultSet.Next();
                            }

                            //Check here if all distinct then return else return exception
                            if (listConduitSubType.Distinct().Count() == 1)
                            {
                                if (listConduitSubType[0] == "1" || listConduitSubType[0] == "2" || listConduitSubType[0] == "3")
                                {
                                    conduitSubType = listConduitSubType[0];
                                }
                                else
                                {
                                    conduitSubType = "MULTI";
                                }
                            }
                            else
                            {
                                string result = listConduitSubType.FirstOrDefault(x => x == "1");
                                if (result != null)
                                {
                                    //found - At least one conduit system is subtype duct bank
                                    conduitSubType = "MIXED";
                                }
                                else
                                {
                                    //Not found - No record is having subtype duct bank
                                    conduitSubType = listConduitSubType[0];
                                }
                            }
                        }
                        break;
                    }
                    rel = relClasses.Next();
                }
            }
            catch (Exception exp)
            {
                Common._log.Error("Exception : " + exp.Message + " |  Stack Trace | " + exp.StackTrace + " , Feature OID : " + argFeaturePriUGCond.OID);
            }
            finally
            {
                if (listConduitSubType != null)
                {
                    listConduitSubType.Clear();
                    listConduitSubType = null;
                }

                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
            return conduitSubType;
        }       
       
        /// This function return the workspace for given connection string
        /// </summary>
        /// <param name="argStrworkSpaceConnectionstring"></param>
        /// <returns></returns>
        public static IWorkspace GetWorkspace(string argStrworkSpaceConnectionstring)
        {
            Type t = null;
            IWorkspaceFactory workspaceFactory = null;
            IWorkspace workspace = null;
            try
            {
                t = Type.GetTypeFromProgID("esriDataSourcesGDB.SdeWorkspaceFactory");
                workspaceFactory = (IWorkspaceFactory)Activator.CreateInstance(t);
                workspace = workspaceFactory.OpenFromFile(argStrworkSpaceConnectionstring, 0);
            }
            catch (Exception exp)
            {
                Common._log.Error("Error in getting SDE Workspace, function GetWorkspace: " + "Exception : " + exp.Message);
                Common._log.Error("Exception : " + exp.Message + " |  Stack Trace | " + exp.StackTrace+" at "+exp.ToString());
                ErrorCodeException ece = new ErrorCodeException(exp);
                Environment.ExitCode = ece.CodeNumber;
            }
            return workspace;
        }

        /// <summary>
        /// This function checks for all the pre requisites for running the process
        /// </summary>
        /// <returns></returns>
        public static bool PreRequiste()
        {
            bool ballPreRequisiteSuccessful = true;
            try
            {
                Common.InitializeESRILicense();
                Common.InitializeArcFMLicense();
            }
            catch (Exception exp)
            {
                Common._log.Error("Error in Initializing Arc GIS/ArcFM License, function PreRequiste " + "Exception : " + exp.Message);
                Common._log.Error("Exception : " + exp.Message + " |  Stack Trace | " + exp.StackTrace+" at "+exp.ToString());
                ErrorCodeException ece = new ErrorCodeException(exp);
                Environment.ExitCode = ece.CodeNumber;               
            }
            return ballPreRequisiteSuccessful;
        }
     
        /// <summary>
        /// This function provides the point on a line with respect to start point and given distance ratio
        /// </summary>
        /// <param name="argFeature"></param>
        /// <param name="argDistancealongcurve"></param>
        /// <returns></returns>
        private static IPoint GetPointFromLineAndDistanceAlongLine(IFeature argFeature, double argDistancealongcurve)
        {
            IPoint pOutputPoint = new PointClass();
            try
            {
                ICurve pCurve = (ICurve)argFeature.Shape;
                pCurve.QueryPoint(esriSegmentExtension.esriNoExtension, argDistancealongcurve, true, pOutputPoint);
            }
            catch (Exception exp)
            {
                Common._log.Error("Exception : " + exp.Message + " |  Stack Trace | " + exp.StackTrace+" at "+exp.ToString());
            }
            return pOutputPoint;
        }
       
        /// <summary>
        /// This function checks for all parallel conductors with respect to given Pri UG conductor based on business rules decided
        /// </summary>
        /// <param name="argPriUGCondFC"></param>
        /// <param name="argPriUGCondOID"></param>
        /// <param name="argIntersectingLineFeature"></param>
        /// <param name="arglistntersectingLineFeatureOIDs"></param>
        /// <param name="argIsConductorLengthToCalculateParallelLengthIsGreaterThanCutOffLength"></param>
        /// <returns></returns>
        private static bool GetNearbyParallelPriUGConductor(IFeatureClass argPriUGCondFC, int argPriUGCondOID, IFeature argIntersectingLineFeature, ref List<int> arglistntersectingLineFeatureOIDs, bool argIsConductorLengthToCalculateParallelLengthIsGreaterThanCutOffLength)
        {
            bool bConditionForTwoConductorsIsSuccess = false; 
            IPoint pPointFromLine = null;
            ISpatialFilter spatialFilter = null;
            ITopologicalOperator topoOperator = null;
            IFeatureCursor featureCursor = null;
            IFeature pFeatureOutPut = null;
            int countPointIntersectInLineBuffer = 0;
            string comment = string.Empty;
            int iCountIntersection = 0;
            try
            {
                foreach (double plineFraction in _list_LineFractions)
                {
                    try
                    {
                        pPointFromLine = GetPointFromLineAndDistanceAlongLine(argIntersectingLineFeature, plineFraction);
                        //double X = pPointFromLine.X;
                        //double Y = pPointFromLine.Y;
                        spatialFilter = new SpatialFilterClass();
                        topoOperator = (ITopologicalOperator)pPointFromLine;
                        spatialFilter.Geometry = topoOperator.Buffer(_searchDistance);
                        //spatialFilter.GeometryField = priUGFeatureClass.ShapeFieldName;
                        spatialFilter.SpatialRel = esriSpatialRelEnum.esriSpatialRelIntersects;
                        spatialFilter.SubFields = ReadConfigurations.subFields;

                        featureCursor = argPriUGCondFC.Search(spatialFilter, false);

                        pFeatureOutPut = featureCursor.NextFeature();
                        while (pFeatureOutPut != null)
                        {
                            if (pFeatureOutPut.OID == argPriUGCondOID)
                            {
                                countPointIntersectInLineBuffer++;
                                break;
                            }
                            pFeatureOutPut = featureCursor.NextFeature();
                        }

                        iCountIntersection++;

                        if (argIsConductorLengthToCalculateParallelLengthIsGreaterThanCutOffLength)
                        {
                            if (countPointIntersectInLineBuffer == _numberOfPointsToBeConsidered)
                                break;

                            // Not running the comparison further because the points left on line are not enough for desired results // Performacnce improvement for bulk process
                            if ((countPointIntersectInLineBuffer + (_list_LineFractions.Count - iCountIntersection)) < _numberOfPointsToBeConsidered)
                            {
                                Common._log.Info("Not running the comparison further because the points left on line OID:" + argIntersectingLineFeature.OID + " are not enough for desired results. Pri UG OID:" + argPriUGCondOID);
                                break;
                            }
                        }
                       
                    }
                    catch (Exception exp)
                    {
                        Common._log.Error("Exception : " + exp.Message +" |  Stack Trace | " + exp.StackTrace+" at "+exp.ToString());
                        bConditionForTwoConductorsIsSuccess = false;
                    }
                    finally
                    {
                        if (featureCursor != null && Marshal.IsComObject(featureCursor))
                        {
                            Marshal.FinalReleaseComObject(featureCursor);
                            featureCursor = null;
                        }

                        if (spatialFilter != null && Marshal.IsComObject(spatialFilter))
                        {
                            Marshal.FinalReleaseComObject(spatialFilter);
                            spatialFilter = null;
                        }

                        if (topoOperator != null && Marshal.IsComObject(topoOperator))
                        {
                            Marshal.FinalReleaseComObject(topoOperator);
                            topoOperator = null;
                        }

                        GC.Collect();
                        GC.WaitForPendingFinalizers();
                    }
                }

                if (countPointIntersectInLineBuffer >= _numberOfPointsToBeConsidered)
                {
                    // Check here , % length of conductor should be greater than cut off length
                    if (argIsConductorLengthToCalculateParallelLengthIsGreaterThanCutOffLength)
                    {
                        if (!arglistntersectingLineFeatureOIDs.Contains(argIntersectingLineFeature.OID))
                        { 
                            arglistntersectingLineFeatureOIDs.Add(argIntersectingLineFeature.OID);
                            bConditionForTwoConductorsIsSuccess = true;
                        }
                    }
                    else
                    {
                        if (IsConductorParallelLengthBasedOnNumberOfPointsContainedIsLessThanCutOffLength(argIntersectingLineFeature, countPointIntersectInLineBuffer, _cutoffLengthConductorToExclude))
                        {
                            // No need to add this conductor in calculation
                            //CIT_Daily_Updates
                            comment = "Calculating FILLEDDUCT for conductor with OID:" + argPriUGCondOID + "Not considering conductor with OID :" + argIntersectingLineFeature.OID + " because its Parallel Length Based On Number Of Points Contained Is Less Than the Cut Off Length.";
                            //_dtconductorExceptions.Rows.Add(argIntersectingLineFeature.OID, comment);
                            if (_bLogNeededWhileExecuting)
                                Common._log.Info("Calculating FILLEDDUCT for conductor with OID:"+ argPriUGCondOID + " and Not considering conductor with OID :" + argIntersectingLineFeature.OID + " because its Parallel Length Based On Number Of Points Contained Is Less Than the Cut Off Length.");
                        }
                        else
                        {
                            if (!arglistntersectingLineFeatureOIDs.Contains(argIntersectingLineFeature.OID))
                            {
                                arglistntersectingLineFeatureOIDs.Add(argIntersectingLineFeature.OID);
                                bConditionForTwoConductorsIsSuccess = true;
                            }
                        }
                    }                    
                }
            }
            catch (Exception exp)
            {
                Common._log.Error("Exception : " + exp.Message +" |  Stack Trace | " + exp.StackTrace+" at "+exp.ToString());
            }
            finally
            {
                if (featureCursor != null && Marshal.IsComObject(featureCursor))
                {
                    Marshal.FinalReleaseComObject(featureCursor);
                    featureCursor = null;
                }

                if (spatialFilter != null && Marshal.IsComObject(spatialFilter))
                {
                    Marshal.FinalReleaseComObject(spatialFilter);
                    spatialFilter = null;
                }

                if (topoOperator != null && Marshal.IsComObject(topoOperator))
                {
                    Marshal.FinalReleaseComObject(topoOperator);
                    topoOperator = null;
                }

                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
            return bConditionForTwoConductorsIsSuccess;
        }

        /// <summary>
        /// This function checks for parallel conductor with respect to given Pri UG conductor based on business rules (case when intersecting conductor becomes given conductor for reference)
        /// </summary>
        /// <param name="argPriUGCondFC"></param>
        /// <param name="argPriUGCondOID"></param>
        /// <param name="argIntersectingLineFeature"></param>
        /// <param name="arglistntersectingLineFeatureOIDs"></param>
        /// <param name="argIsConductorLengthToCalculateParallelLengthIsGreaterThanCutOffLength"></param>
        /// <returns></returns>
        private static bool GetNearbyParallelPriUGConductor_ForIntersectingConductor(IFeatureClass argPriUGCondFC, int argPriUGCondOID, IFeature argIntersectingLineFeature, ref List<int> arglistntersectingLineFeatureOIDs, bool argIsConductorLengthToCalculateParallelLengthIsGreaterThanCutOffLength)
        {
            bool bProcess = true;
            IPoint pPointFromLine = null;
            ISpatialFilter spatialFilter = null;
            ITopologicalOperator topoOperator = null;
            IFeatureCursor featureCursor = null;
            IFeature pFeatureOutPut = null;
            int countPointIntersectInLineBuffer = 0;
            string comment = string.Empty;
            int iCountIntersection = 0;
            try
            {
                //int OID = argIntersectingLineFeature.OID;
                foreach (double plineFraction in _list_LineFractions)
                {
                    try
                    {
                        pPointFromLine = GetPointFromLineAndDistanceAlongLine(argIntersectingLineFeature, plineFraction);
                        spatialFilter = new SpatialFilterClass();
                        topoOperator = (ITopologicalOperator)pPointFromLine;
                        spatialFilter.Geometry = topoOperator.Buffer(_searchDistance);
                        spatialFilter.SpatialRel = esriSpatialRelEnum.esriSpatialRelIntersects;
                        spatialFilter.SubFields = ReadConfigurations.subFields;

                        featureCursor = argPriUGCondFC.Search(spatialFilter, false);

                        pFeatureOutPut = featureCursor.NextFeature();
                        while (pFeatureOutPut != null)
                        {
                            if (pFeatureOutPut.OID == argPriUGCondOID)
                            {
                                countPointIntersectInLineBuffer++;
                                break;
                            }
                            pFeatureOutPut = featureCursor.NextFeature();
                        }

                        if (argIsConductorLengthToCalculateParallelLengthIsGreaterThanCutOffLength)
                        {
                            if (countPointIntersectInLineBuffer == _numberOfPointsToBeConsidered)
                                break;
                        }

                        iCountIntersection++;
                        // Not running the comparison further because the points left on line are not enough for desired results // Performacnce improvement for bulk process
                        if ((countPointIntersectInLineBuffer + (_list_LineFractions.Count - iCountIntersection)) < _numberOfPointsToBeConsidered)
                        {
                            Common._log.Info("Not running the comparison further because the points left on line,OID:" + argIntersectingLineFeature.OID + " are not enough for desired results. Pri UG OID:" + argPriUGCondOID);
                            break;
                        }
                    }
                    catch (Exception exp)
                    {
                        Common._log.Error("Exception : " + exp.Message +" |  Stack Trace | " + exp.StackTrace+" at "+exp.ToString());
                    }
                    finally
                    {
                        if (featureCursor != null && Marshal.IsComObject(featureCursor))
                        {
                            Marshal.FinalReleaseComObject(featureCursor);
                            featureCursor = null;
                        }

                        if (spatialFilter != null && Marshal.IsComObject(spatialFilter))
                        {
                            Marshal.FinalReleaseComObject(spatialFilter);
                            spatialFilter = null;
                        }

                        if (topoOperator != null && Marshal.IsComObject(topoOperator))
                        {
                            Marshal.FinalReleaseComObject(topoOperator);
                            topoOperator = null;
                        }

                        GC.Collect();
                        GC.WaitForPendingFinalizers();
                    }
                }

                if (countPointIntersectInLineBuffer >= _numberOfPointsToBeConsidered)
                {
                    // Check here , % length of conductor should be greater than cut off length
                    if (argIsConductorLengthToCalculateParallelLengthIsGreaterThanCutOffLength)
                    {
                        if (!arglistntersectingLineFeatureOIDs.Contains(argPriUGCondOID))
                            arglistntersectingLineFeatureOIDs.Add(argPriUGCondOID);
                    }
                    else
                    {
                        if (IsConductorParallelLengthBasedOnNumberOfPointsContainedIsLessThanCutOffLength(argIntersectingLineFeature, countPointIntersectInLineBuffer, _cutoffLengthConductorToExclude))
                        {
                            // No need to add this conductor in calculation
                            //CIT_Daily_Updates
                            comment = "Calculating FILLEDDUCT for conductor with OID:" + argPriUGCondOID + "Not considering conductor with OID :" + argIntersectingLineFeature.OID + " because its Parallel Length Based On Number Of Points Contained Is Less Than the Cut Off Length.";
                            //_dtconductorExceptions.Rows.Add(argIntersectingLineFeature.OID, comment);
                            if (_bLogNeededWhileExecuting)
                                Common._log.Info(comment);
                        }
                        else
                        {
                            if (!arglistntersectingLineFeatureOIDs.Contains(argPriUGCondOID))
                                arglistntersectingLineFeatureOIDs.Add(argPriUGCondOID);
                        }
                    }
                }
            }
            catch (Exception exp)
            {
                Common._log.Error("Exception : " + exp.Message +" |  Stack Trace | " + exp.StackTrace+" at "+exp.ToString());
            }
            finally
            {
                if (featureCursor != null && Marshal.IsComObject(featureCursor))
                {
                    Marshal.FinalReleaseComObject(featureCursor);
                    featureCursor = null;
                }

                if (spatialFilter != null && Marshal.IsComObject(spatialFilter))
                {
                    Marshal.FinalReleaseComObject(spatialFilter);
                    spatialFilter = null;
                }

                if (topoOperator != null && Marshal.IsComObject(topoOperator))
                {
                    Marshal.FinalReleaseComObject(topoOperator);
                    topoOperator = null;
                }

                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
            return bProcess;
        }

        private static bool GetNearbyParallelPriUGConductor_ForRelationBwTwoConductors(IFeatureClass argPriUGCondFC, int argPriUGCondOID, IFeature argIntersectingLineFeature, bool argIsConductorLengthToCalculateParallelLengthIsGreaterThanCutOffLength, double argSearchDistance)
        {
            bool bTwoConductorsAreSpatiallyRelated = false;
            IPoint pPointFromLine = null;
            ISpatialFilter spatialFilter = null;
            ITopologicalOperator topoOperator = null;
            IFeatureCursor featureCursor = null;
            IFeature pFeatureOutPut = null;
            int countPointIntersectInLineBuffer = 0;
            int iCountIntersection = 0;
            try
            {
                //int OID = argIntersectingLineFeature.OID;

                foreach (double plineFraction in _list_LineFractions)
                {
                    try
                    {
                        pPointFromLine = GetPointFromLineAndDistanceAlongLine(argIntersectingLineFeature, plineFraction);
                        //double X = pPointFromLine.X;
                        //double Y = pPointFromLine.Y;
                        spatialFilter = new SpatialFilterClass();
                        topoOperator = (ITopologicalOperator)pPointFromLine;
                        spatialFilter.Geometry = topoOperator.Buffer(argSearchDistance);
                        //spatialFilter.GeometryField = priUGFeatureClass.ShapeFieldName;
                        spatialFilter.SpatialRel = esriSpatialRelEnum.esriSpatialRelIntersects;
                        spatialFilter.SubFields = ReadConfigurations.subFields;

                        featureCursor = argPriUGCondFC.Search(spatialFilter, false);

                        pFeatureOutPut = featureCursor.NextFeature();
                        while (pFeatureOutPut != null)
                        {
                            if (pFeatureOutPut.OID == argPriUGCondOID)
                            {
                                countPointIntersectInLineBuffer++;
                                break;
                            }
                            pFeatureOutPut = featureCursor.NextFeature();
                        }

                        if (argIsConductorLengthToCalculateParallelLengthIsGreaterThanCutOffLength)
                        {
                            if (countPointIntersectInLineBuffer == _numberOfPointsToBeConsidered)
                                break;
                        }

                        iCountIntersection++;
                        // Not running the comparison further because the points left on line are not enough for desired results // Performacnce improvement for bulk process
                        if ((countPointIntersectInLineBuffer + (_list_LineFractions.Count - iCountIntersection)) < _numberOfPointsToBeConsidered)
                        {
                            Common._log.Info("Not running the comparison further because the points left on line,OID:" + argIntersectingLineFeature.OID + " are not enough for desired results. Pri UG OID:" + argPriUGCondOID);
                            break;
                        }

                    }
                    catch (Exception exp)
                    {
                        Common._log.Error("Exception : " + exp.Message +" |  Stack Trace | " + exp.StackTrace+" at "+exp.ToString());
                        bTwoConductorsAreSpatiallyRelated = false;
                    }
                    finally
                    {
                        if (featureCursor != null && Marshal.IsComObject(featureCursor))
                        {
                            Marshal.FinalReleaseComObject(featureCursor);
                            featureCursor = null;
                        }

                        if (spatialFilter != null && Marshal.IsComObject(spatialFilter))
                        {
                            Marshal.FinalReleaseComObject(spatialFilter);
                            spatialFilter = null;
                        }

                        if (topoOperator != null && Marshal.IsComObject(topoOperator))
                        {
                            Marshal.FinalReleaseComObject(topoOperator);
                            topoOperator = null;
                        }

                        GC.Collect();
                        GC.WaitForPendingFinalizers();
                    }
                }

                if (countPointIntersectInLineBuffer >= _numberOfPointsToBeConsidered)
                {
                    // Check here , % length of conductor should be greater than cut off length
                    if (argIsConductorLengthToCalculateParallelLengthIsGreaterThanCutOffLength)
                    {
                        bTwoConductorsAreSpatiallyRelated = true;
                    }
                    else
                    {
                        if (IsConductorParallelLengthBasedOnNumberOfPointsContainedIsLessThanCutOffLength(argIntersectingLineFeature, countPointIntersectInLineBuffer, _cutoffLengthConductorToExclude))
                        {
                            // No need to add this conductor in calculation
                            if (_bLogNeededWhileExecuting)
                                Common._log.Info("Conductor (OID:" + argIntersectingLineFeature.OID + ") Parallel Length Based On Number Of Points Contained IsLess Than Cut Off Length.");
                        }
                        else
                        {
                            bTwoConductorsAreSpatiallyRelated = true;
                        }
                    }
                }
            }
            catch (Exception exp)
            {
                Common._log.Error("Exception : " + exp.Message +" |  Stack Trace | " + exp.StackTrace+" at "+exp.ToString());
            }
            finally
            {
                if (featureCursor != null && Marshal.IsComObject(featureCursor))
                {
                    Marshal.FinalReleaseComObject(featureCursor);
                    featureCursor = null;
                }

                if (spatialFilter != null && Marshal.IsComObject(spatialFilter))
                {
                    Marshal.FinalReleaseComObject(spatialFilter);
                    spatialFilter = null;
                }

                if (topoOperator != null && Marshal.IsComObject(topoOperator))
                {
                    Marshal.FinalReleaseComObject(topoOperator);
                    topoOperator = null;
                }

                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
            return bTwoConductorsAreSpatiallyRelated;
        }        
      
        /// <summary>
        /// This function export results to log file
        /// </summary>
        /// <param name="argpDataTable"></param>
        /// <returns></returns>
        public static bool ExportResults(DataTable argpDataTable)
        {
            bool exportedSuccessfully = true;
            StringBuilder sb = new StringBuilder();
            StringBuilder sbin = new StringBuilder();
            string log = null;
            try
            {
                sb.AppendLine("");             
                foreach (DataColumn pColumn in argpDataTable.Columns)
                {
                    sb.Append(pColumn.ColumnName + "$");
                }
                sb.AppendLine("");  

                foreach (DataRow pRow in argpDataTable.Rows)
                {
                    sbin = new StringBuilder();
                    foreach (DataColumn pColumn in argpDataTable.Columns)
                    {
                        sbin.Append(Convert.ToString(pRow[pColumn.ColumnName]) + "$");
                    }
                    sb.AppendLine(sbin.ToString());
                }
                sb.AppendLine("");  

                Common._log.Info("");
                Common._log.Info("All Stats..");
                log = sb.ToString();
                Common._log.Info(log);              
                Common._log.Info("");
            }
            catch (Exception exp)
            {
                Common._log.Error("Exception : " + exp.Message + " |  Stack Trace | " + exp.StackTrace+" at "+exp.ToString());
            }
            return exportedSuccessfully;
        }

        /// <summary>
        /// This functions checkd whether given conductor is idle or not
        /// </summary>
        /// <param name="argpFeature"></param>
        /// <returns></returns>
        private static bool IsConductorNotInService(IFeature argpFeature)
        {
            string statusValue = null;
            bool isConductornotinservice = false;            
            //Status In Service code is 5
            try
            {
                statusValue = Convert.ToString(argpFeature.get_Value(argpFeature.Fields.FindField(ReadConfigurations.col_STATUS)));

                if (statusValue != ReadConfigurations.val_StatusInService)
                {
                    isConductornotinservice = true;     
               
                    if(_bLogNeededWhileExecuting)
                    Common._log.Info("Conductor with OID :" + argpFeature.OID + "is excluded because of status is not in service.");
                }
                else
                {
                    isConductornotinservice = false;
                }
            }
            catch (Exception exp)
            {
                Common._log.Error("Exception : " + exp.Message +" |  Stack Trace | " + exp.StackTrace+" at "+exp.ToString());
                isConductornotinservice = false;
            }
            return isConductornotinservice;
        }

        /// <summary>
        /// This function checks for whether given pri UG conductor is less than cut off length decided by business rules
        /// </summary>
        /// <param name="argpFeature"></param>
        /// <param name="argCutoffLength"></param>
        /// <param name="argIsConductorLengthToCalculateParallelLengthIsGreaterThanCutOffLength"></param>
        /// <returns></returns>
        private static bool IsConductorLessThanCutOffLength(IFeature argpFeature, double argCutoffLength)
        {
            double dblConductorLength = 0;
            bool isConductorLessThanCutOffLength = false;
            try
            {
                //IPoint pt= GetPointFromLineAndDistanceAlongLine(argpFeature, 0.5);
                //double ptX = pt.X;
                //double ptY = pt.Y;                

                dblConductorLength = ((ICurve)argpFeature.Shape).Length;
                
                if (dblConductorLength <= argCutoffLength)
                {
                    isConductorLessThanCutOffLength = true;
                }
                else
                {
                    isConductorLessThanCutOffLength = false;
                }

                //if (dblConductorLength >= _intConductorLengthToCalculateParallelLengthIsGreaterThanCutOffLength)
                //{
                //    argIsConductorLengthToCalculateParallelLengthIsGreaterThanCutOffLength = true;
                //}
                //else
                //{
                //    argIsConductorLengthToCalculateParallelLengthIsGreaterThanCutOffLength = false;
                //}
            }
            catch (Exception exp)
            {
                Common._log.Error("Exception : " + exp.Message +" |  Stack Trace | " + exp.StackTrace+" at "+exp.ToString());
                isConductorLessThanCutOffLength = false;
            }
            return isConductorLessThanCutOffLength;
        }

        /// <summary>
        /// This function checks for whether given pri UG conductor will be excluded based on conductor codes 
        /// </summary>
        /// <param name="argpFeature"></param>
        /// <param name="argCutoffLength"></param>
        /// <param name="argIsConductorLengthToCalculateParallelLengthIsGreaterThanCutOffLength"></param>
        /// <returns></returns>
        private static bool IsConductorNeedsToBeExcludedBasedOnConductorCodes(IFeature argpFeature)
        {
            string strConductorCode = "";           
            bool isConductorNeedsToBeExcluded = false;
            IRow pRowConductorInfo = null;          
            try
            {
                IEnumRelationshipClass relClasses = ((IFeatureClass)argpFeature.Class).get_RelationshipClasses(esriRelRole.esriRelRoleOrigin);
                IRelationshipClass rel = relClasses.Next();
                while (rel != null)
                {
                    string relName = ((IDataset)rel.DestinationClass).Name.ToUpper();

                    if (relName == ReadConfigurations.TB_PRIUGCONDUCTORINFO.ToUpper())
                    {
                        ESRI.ArcGIS.esriSystem.ISet resultSet = rel.GetObjectsRelatedToObject(argpFeature);

                        // Case 1 Conductor having no related conduit system record
                        // Case 2 Conductor having single related conduit system record 
                        // Case 3 Conductor having multiple related conduit system record 
                        if (resultSet.Count == 0)
                        {
                            //conduitSubType = "0";
                            break;
                        }
                        else if (resultSet.Count == 1)
                        {
                             pRowConductorInfo = (IRow)resultSet.Next();
                             strConductorCode = Convert.ToString(pRowConductorInfo.get_Value(pRowConductorInfo.Fields.FindField(ReadConfigurations.col_PGE_CONDUCTORCODE)));
                             if (!string.IsNullOrEmpty(strConductorCode) && _listConductorCodesToExclude.Contains(strConductorCode))
                             {
                                 isConductorNeedsToBeExcluded = true;
                             }
                            break;
                        }
                        else if (resultSet.Count > 1)
                        {                           
                            pRowConductorInfo = (IRow)resultSet.Next();
                            while (pRowConductorInfo != null)
                            {                                
                                strConductorCode = Convert.ToString(pRowConductorInfo.get_Value(pRowConductorInfo.Fields.FindField(ReadConfigurations.col_PGE_CONDUCTORCODE)));
                                if (!string.IsNullOrEmpty(strConductorCode) && _listConductorCodesToExclude.Contains(strConductorCode))
                                {
                                    isConductorNeedsToBeExcluded = true;
                                    break;
                                }
                                pRowConductorInfo = (IRow)resultSet.Next();
                            }                          
                        }
                        break;
                    }
                    rel = relClasses.Next();
                }

                if (_bLogNeededWhileExecuting && isConductorNeedsToBeExcluded )
                    Common._log.Info("Conductor with OID :" + argpFeature.OID + " is excluded because of conductor codes.");

            }
            catch (Exception exp)
            {
                Common._log.Error("Exception : " + exp.Message + " |  Stack Trace | " + exp.StackTrace+" at "+exp.ToString());
                isConductorNeedsToBeExcluded = false;
            }
            return isConductorNeedsToBeExcluded;
        }


        /// <summary>
        /// This function checks whether a conductor is 40% parallel to other conductor
        /// </summary>
        /// <param name="argpFeature"></param>
        /// <param name="argNumberOfPoints"></param>
        /// <param name="argCutoffLength"></param>
        /// <returns></returns>
        private static bool IsConductorParallelLengthBasedOnNumberOfPointsContainedIsLessThanCutOffLength(IFeature argpFeature, int argNumberOfPoints,double argCutoffLength)
        {
            bool isConductorParallelLengthBasedOnNumberOfPointsContainedIsLessThanCutOffLength = false;
            double dblConductorLength = 0;
            double dblConductorLength_inlogic = 0;
            try
            {
                dblConductorLength = ((ICurve)argpFeature.Shape).Length;

                switch (argNumberOfPoints)
                {
                    case 5:
                        {
                            dblConductorLength_inlogic = dblConductorLength * 0.4;
                            break;
                        }
                    case 6:
                        {
                            dblConductorLength_inlogic = dblConductorLength * 0.5;
                            break;
                        }
                    case 7:
                        {
                            dblConductorLength_inlogic = dblConductorLength * 0.6;
                            break;
                        }
                    case 8:
                        {
                            dblConductorLength_inlogic = dblConductorLength * 0.7;
                            break;
                        }
                    case 9:
                        {
                            dblConductorLength_inlogic = dblConductorLength * 0.8;
                            break;
                        }
                    case 10:
                        {
                            dblConductorLength_inlogic = dblConductorLength * 0.9;
                            break;

                        }
                    case 11:
                        {
                            dblConductorLength_inlogic = dblConductorLength * 1;
                            break;
                        }
                }

                if (dblConductorLength_inlogic <= argCutoffLength)
                {
                    isConductorParallelLengthBasedOnNumberOfPointsContainedIsLessThanCutOffLength = true;
                }
                else
                {
                    isConductorParallelLengthBasedOnNumberOfPointsContainedIsLessThanCutOffLength = false;
                }
            }
            catch (Exception exp)
            {
                Common._log.Error("Exception : " + exp.Message +" |  Stack Trace | " + exp.StackTrace+" at "+exp.ToString());
                isConductorParallelLengthBasedOnNumberOfPointsContainedIsLessThanCutOffLength = false;
            }
            return isConductorParallelLengthBasedOnNumberOfPointsContainedIsLessThanCutOffLength;
        }

        /// <summary>
        /// This function cheks whether conductor's conduit subtype is DuctBank
        /// </summary>
        /// <param name="argpFeature"></param>
        /// <returns></returns>
        private static bool IsConductorSubTypeNotForRuleOne(IFeature argpFeature)
        {
            string typeOfConduit = null;
            bool bIsConductorSubTypeNotForRuleOne = false;
            DataRow[] pRows = null;
            try
            {
                if (MainClass._processtype == "BULK")
                {
                    pRows = Bulk_Process._dtAllRule1Reocrds.Select(ReadConfigurations.col_OBJECTID+"=" + argpFeature.OID);
                    if (pRows != null && pRows.Length > 0)
                    {
                        typeOfConduit = "2";
                    }
                    else
                    {
                        typeOfConduit = ProcessPriUGCond_GetTypeOfConduit(argpFeature);
                    }
                }
                else if (MainClass._processtype == "SCHEDULE")
                {
                    typeOfConduit = ProcessPriUGCond_GetTypeOfConduit(argpFeature);
                }

                if (typeOfConduit == "1" || typeOfConduit == "MULTI" || typeOfConduit == "MIXED")
                {
                    bIsConductorSubTypeNotForRuleOne = true;
                }                
            }
            catch (Exception exp)
            {
                Common._log.Error("Exception : " + exp.Message +" |  Stack Trace | " + exp.StackTrace+" at "+exp.ToString());
                bIsConductorSubTypeNotForRuleOne = false;
            }
            return bIsConductorSubTypeNotForRuleOne;
        }

        /// <summary>
        /// This function cheks whether conductor's conduit subtype is DuctBank
        /// </summary>
        /// <param name="argpFeature"></param>
        /// <returns></returns>
        private static bool IsConductorSubTypeNotForRuleOne_RunTime(IFeature argpFeature)
        {
            string typeOfConduit = null;
            bool bIsConductorSubTypeNotForRuleOne = false;
            try
            {
                typeOfConduit = ProcessPriUGCond_GetTypeOfConduit(argpFeature);

                if (typeOfConduit == "1" || typeOfConduit == "MULTI" || typeOfConduit == "MIXED")
                {
                    bIsConductorSubTypeNotForRuleOne = true;
                }
            }
            catch (Exception exp)
            {
                Common._log.Error("Exception : " + exp.Message + " |  Stack Trace | " + exp.StackTrace+" at "+exp.ToString());
                bIsConductorSubTypeNotForRuleOne = false;
            }
            return bIsConductorSubTypeNotForRuleOne;
        }

        /// <summary>
        /// This function cheks whether conductor's conduit subtype is DuctBank
        /// </summary>
        /// <param name="argpFeature"></param>
        /// <returns></returns>
        private static bool IsConductorNotThreePhase(IFeature argpFeature)
        {
            string subTypeValue = null;
            bool isConductorNotThreePhase = false;            
            try
            {
                subTypeValue = Convert.ToString(argpFeature.get_Value(argpFeature.Fields.FindField(ReadConfigurations.col_SUBTYPECD)));

                if (subTypeValue == "3")
                {
                    isConductorNotThreePhase = false;
                }
                else
                {                   
                    isConductorNotThreePhase = true;
                }
            }
            catch (Exception exp)
            {
                Common._log.Error("Exception : " + exp.Message + " |  Stack Trace | " + exp.StackTrace+" at "+exp.ToString());
                isConductorNotThreePhase = false;
            }
            return isConductorNotThreePhase;
        }

        /// <summary>
        /// This function fetch all the data for conduit system subtype DuctBank
        /// </summary>
        /// <param name="strConnectionString"></param>
        /// <param name="strSqlQuery"></param>
        private static DataTable GetConduitSubtypeDuctBankData(string strConnectionString, string strSqlQuery)
        {
            DataTable _dtConduitSubtypeDuctBankData = null;
            try
            {
                _dtConduitSubtypeDuctBankData = new DataTable();
                //Common._log.Info("Connection string to fetch data : " + strConnectionString);
                Common._log.Info("SQL query to fetch data : " + strSqlQuery);
                _dtConduitSubtypeDuctBankData = DBHelper.GetDataTable(strConnectionString, strSqlQuery);
                           
            }
            catch (Exception exp)
            {
                Common._log.Error("Error while fetching data to update from database table." + "Exception : " + exp.Message);
                Common._log.Error("Exception : " + exp.Message + " |  Stack Trace | " + exp.StackTrace+" at "+exp.ToString());
            }
            return _dtConduitSubtypeDuctBankData;
        }

        /// <summary>
        /// This function return the filledduct value for given pri UG conductor (conduit system subtype Ductbank) from pre calculated duct bank value for all conductors
        /// </summary>
        /// <param name="argObjectID"></param>
        /// <returns></returns>
        private static int GetDuctFilledForPrimaryUnderGroundDuctBankType(int argObjectID)
        {
            int filledDucts = 0;
            try
            {
                DataRow[] pRows = MainClass._dtConduitSubtypeDuctBankData.Select(ReadConfigurations.col_OBJECTID+"="+ argObjectID);
                if (pRows.Length == 1)
                {
                    filledDucts = Convert.ToInt16(pRows[0][ReadConfigurations.col_FILLEDDUCT]);
                }
                else
                {
                    Common._log.Error("ObjectiD " + argObjectID + " is not present in table or multiple entries found in table.OID Count in table=" + pRows.Length);
                }  
            }
            catch (Exception exp)
            {               
                Common._log.Error("Exception : " + exp.Message +" |  Stack Trace | " + exp.StackTrace+" at "+exp.ToString());
                filledDucts = 1;
            }
            return filledDucts;
        }

        private static void GetSpatialRelationBetweenConductors_NonComplexScenarios(IFeatureClass argpriUGFeatureClass, IFeature pFeaturePriUGCond, string typeOfConduit, int countAdjacentFeatures, List<int> argListintersectingLineFeatureOIDs, ref int countConductorSpatiallyRelated, ref bool argbAllExcluded)
        {
            bool bConductorsAreSpatiallyRelated = false;
            IFeature Conductor1 = null;
            IFeature Conductor2 = null;
            string allParallelCables = string.Empty;
            Dictionary<int, List<int>> dictAllParallelCables = new Dictionary<int, List<int>>();
            Dictionary<int, List<int>> dictAllParallelCables_inner = null;
            List<int> oidstoCompare = new List<int>();
            List<int> oidstoCompare_inner = new List<int>();
            int OIDforFilledductMax = 0;
            try
            {
                OIDforFilledductMax = GetSpatialRelationBetweenMultipleConductors(argpriUGFeatureClass, argListintersectingLineFeatureOIDs, ref dictAllParallelCables);

                if (OIDforFilledductMax != 0)
                {
                    if (dictAllParallelCables.ContainsKey(OIDforFilledductMax))
                    {
                        foreach (int oidCond in dictAllParallelCables[OIDforFilledductMax])
                        {
                            if (!MainClass._listintersectingLineFeatureOIDs.Contains(oidCond))
                                MainClass._listintersectingLineFeatureOIDs.Add(oidCond);
                        }
                    }

                    countConductorSpatiallyRelated = dictAllParallelCables[OIDforFilledductMax].Count;

                    if (countConductorSpatiallyRelated >= 2)
                    {    
                        if (countConductorSpatiallyRelated == 2 || countConductorSpatiallyRelated == 3)
                        {
                            if (countConductorSpatiallyRelated == 2)
                            {
                                foreach (int OID in dictAllParallelCables[OIDforFilledductMax])
                                {
                                    oidstoCompare.Add(OID);
                                }
                            }
                            else if (countConductorSpatiallyRelated == 3)
                            {
                                foreach (int OID in dictAllParallelCables[OIDforFilledductMax])
                                {
                                    if (OID != OIDforFilledductMax)
                                    {
                                        oidstoCompare.Add(OID);
                                    }
                                }
                            }

                            Conductor1 = argpriUGFeatureClass.GetFeature(oidstoCompare[0]);
                            Conductor2 = argpriUGFeatureClass.GetFeature(oidstoCompare[1]);

                            bConductorsAreSpatiallyRelated = GetSpatialRelationBetweenTwoConductors(Conductor1, Conductor2);

                            if (!bConductorsAreSpatiallyRelated)
                            {
                                countConductorSpatiallyRelated--;
                            }
                        }
                        else if (countConductorSpatiallyRelated == 4)
                        {
                            foreach (int OID in dictAllParallelCables[OIDforFilledductMax])
                            {
                                if (OID != OIDforFilledductMax)
                                {
                                    oidstoCompare.Add(OID);
                                }
                            }

                            countConductorSpatiallyRelated--;

                            dictAllParallelCables_inner = new Dictionary<int, List<int>>();
                            //dictAllParallelCables.Clear();
                            OIDforFilledductMax = GetSpatialRelationBetweenMultipleConductors(argpriUGFeatureClass, oidstoCompare, ref dictAllParallelCables_inner);

                            if (OIDforFilledductMax != 0)
                            {
                                oidstoCompare_inner = new List<int>();
                                //dictAllParallelCables.
                                //Check here that all conductors are specially related to each other or not 
                                foreach (int OID in dictAllParallelCables_inner[OIDforFilledductMax])
                                {
                                    oidstoCompare_inner.Add(OID);
                                }

                                Conductor1 = argpriUGFeatureClass.GetFeature(oidstoCompare_inner[0]);
                                Conductor2 = argpriUGFeatureClass.GetFeature(oidstoCompare_inner[1]);

                                bConductorsAreSpatiallyRelated = GetSpatialRelationBetweenTwoConductors(Conductor1, Conductor2);

                                if (!bConductorsAreSpatiallyRelated)
                                {
                                    countConductorSpatiallyRelated--;
                                }
                            }
                            else
                            {
                                if(_bLogNeededWhileExecuting)
                                Common._log.Info("No further comparison required.");
                            }
                        }
                    }
                    //Commenting below to solve the issue for 3 cables IIll to each other , FD=3
                    //if (iCountforFilledductIncreaseMax < countConductorSpatiallyRelated)
                    //{
                    //    countConductorSpatiallyRelated = iCountforFilledductIncreaseMax;
                    //}
                }
                else
                {
                    argbAllExcluded = true;
                }

            }
            catch (Exception exp)
            {
                Common._log.Error("Exception : " + exp.Message + " |  Stack Trace | " + exp.StackTrace+" at "+exp.ToString());
            }
            finally
            {
                if (dictAllParallelCables != null)
                {
                    dictAllParallelCables.Clear();
                    dictAllParallelCables = null;
                }
                if (dictAllParallelCables_inner != null)
                {
                    dictAllParallelCables_inner.Clear();
                    dictAllParallelCables_inner = null;
                }
                if (oidstoCompare != null)
                {
                    oidstoCompare.Clear();
                    oidstoCompare = null;
                }
                if (oidstoCompare_inner != null)
                {
                    oidstoCompare_inner.Clear();
                    oidstoCompare_inner = null;
                }
            }
        }

        private static int GetSpatialRelationBetweenMultipleConductors(IFeatureClass argpriUGFeatureClass, List<int> argListintersectingLineFeatureOIDs, ref Dictionary<int, List<int>> argDictAllParallelCables)
        {
            bool bConductorsAreSpatiallyRelated = false;
            //int oidConductor1 = 0;
            //int oidConductor2 = 0;
            IFeature Conductor1 = null;
            IFeature Conductor2 = null;
            int countConductor = 0;
            string allParallelCables = string.Empty;
            List<int> lstParallelCables = null;
            //Dictionary<int, List<int>> dictAllParallelCables = new Dictionary<int, List<int>>();          
            bool bIsConductor1ThreePhase = false;
            bool bIsConductor2ThreePhase = false;
            int iCountforFilledductIncrease = 0;
            int iCountforFilledductIncreaseMax = 0;
            int OIDforFilledductMax = 0;
            bool bConductorOneNeedsToBeExcludedBasedOnConductorCodes = false;
            bool bConductorTwoNeedsToBeExcludedBasedOnConductorCodes = false;
            try
            {
                countConductor = argListintersectingLineFeatureOIDs.Count;

                for (int iCount = 0; iCount < countConductor; iCount++)
                {
                    lstParallelCables = new List<int>();

                    for (int iCountInner = 0; iCountInner < countConductor; iCountInner++)
                    {
                        if (iCountInner == iCount)
                            continue;

                        if(_bLogNeededWhileExecuting)
                        Common._log.Info(iCount + "," + iCountInner);

                        Conductor1 = argpriUGFeatureClass.GetFeature(argListintersectingLineFeatureOIDs[iCount]);
                        Conductor2 = argpriUGFeatureClass.GetFeature(argListintersectingLineFeatureOIDs[iCountInner]);

                        bIsConductor1ThreePhase = !IsConductorNotThreePhase(Conductor1);
                        bIsConductor2ThreePhase = !IsConductorNotThreePhase(Conductor2);

                        if (bIsConductor1ThreePhase && !bIsConductor2ThreePhase)
                        {
                            if (_bLogNeededWhileExecuting)
                            Common._log.Info(Conductor1.OID + "is 3 phase but " + Conductor2.OID + " is not a three phase conductor.");
                            continue;
                        }

                        bConductorsAreSpatiallyRelated = GetSpatialRelationBetweenTwoConductors(Conductor1, Conductor2);

                        if (bConductorsAreSpatiallyRelated)
                        {
                            bConductorOneNeedsToBeExcludedBasedOnConductorCodes = IsConductorNeedsToBeExcludedBasedOnConductorCodes(Conductor1);
                            bConductorTwoNeedsToBeExcludedBasedOnConductorCodes = IsConductorNeedsToBeExcludedBasedOnConductorCodes(Conductor2);

                            if (!bConductorOneNeedsToBeExcludedBasedOnConductorCodes && !bConductorTwoNeedsToBeExcludedBasedOnConductorCodes)
                            {
                                ////Check here for subtype concept as well;
                                //bool b1 = IsConductorSubTypeNotForRuleOne(Conductor1);
                                //bool b2 = IsConductorSubTypeNotForRuleOne(Conductor2);
                                //if (b1 || b2)
                                //{ 

                                //}

                                iCountforFilledductIncrease++;

                                if (_bLogNeededWhileExecuting)
                                Common._log.Info(Conductor1.OID + " and " + Conductor2.OID + " are specially related to each other.");

                                if (!lstParallelCables.Contains(Conductor1.OID))
                                    lstParallelCables.Add(Conductor1.OID);

                                if (!lstParallelCables.Contains(Conductor2.OID))
                                    lstParallelCables.Add(Conductor2.OID);

                                if (iCountforFilledductIncreaseMax < iCountforFilledductIncrease)
                                {
                                    iCountforFilledductIncreaseMax = iCountforFilledductIncrease;
                                    OIDforFilledductMax = Conductor1.OID;
                                }
                            }
                            else
                            {
                                //Check here for subtype concept as well;
                                //bool b1 = IsConductorSubTypeNotForRuleOne(Conductor1);
                                //bool b2 = IsConductorSubTypeNotForRuleOne(Conductor2);
                                //if (b1 || b2)
                                //{

                                //}

                                if (!bConductorOneNeedsToBeExcludedBasedOnConductorCodes && !lstParallelCables.Contains(Conductor1.OID))
                                    lstParallelCables.Add(Conductor1.OID);

                                if (!bConductorTwoNeedsToBeExcludedBasedOnConductorCodes && !lstParallelCables.Contains(Conductor2.OID))
                                    lstParallelCables.Add(Conductor2.OID);
                            }
                        }
                    }

                    if (!argDictAllParallelCables.ContainsKey(Conductor1.OID))
                    {
                        argDictAllParallelCables.Add(Conductor1.OID, new List<int>(lstParallelCables));
                    }

                    iCountforFilledductIncrease = 0;
                    lstParallelCables.Clear();
                }
            }
            catch (Exception exp)
            {
                Common._log.Error("Exception : " + exp.Message + " |  Stack Trace | " + exp.StackTrace+" at "+exp.ToString());
            }
            finally
            {
                if (lstParallelCables != null)
                {
                    lstParallelCables.Clear();
                    lstParallelCables = null;
                }            
            }
            return OIDforFilledductMax;
        }

        private static int GetSpatialRelationBetweenConductors_ComplexScenarios(IFeatureClass argpriUGFeatureClass, IFeature pFeaturePriUGCond, string typeOfConduit, int countAdjacentFeatures, List<int> argListintersectingLineFeatureOIDs, ref int countConductorSpatiallyRelated)
        {
            int iFilledDuctValue = 2;
            try
            {
                GetSpatialRelationBetweenConductors_ComplexScenarios_Generic(argpriUGFeatureClass, argListintersectingLineFeatureOIDs, ref iFilledDuctValue);
            }
            catch (Exception exp)
            {
                Common._log.Error("Exception : " + exp.Message +" |  Stack Trace | " + exp.StackTrace+" at "+exp.ToString());
            }
            finally
            {

            }
            return iFilledDuctValue;
        }

        private static int GetSpatialRelationBetweenConductors_ComplexScenarios_Generic(IFeatureClass argpriUGFeatureClass, List<int> argListintersectingLineFeatureOIDs, ref int iFilledDuctValue)
        {
            bool bConductorsAreSpatiallyRelated = false;
            IFeature Conductor1 = null;
            IFeature Conductor2 = null;
            int countConductor = 0;
            string allParallelCables = string.Empty;
            List<string> lstallParallelCables = new List<string>();
            List<int> lstCablesToCheck = new List<int>();
            int iConductorsAreSpatiallyRelated = 0;
            int iConductorsAreSpatiallyRelated_Final = 0;
            int mainConductorOID = 0;
            DataTable dtParallelCables = null;
            bool bConductorOneNeedsToBeExcludedBasedOnConductorCodes;
            bool bConductorTwoNeedsToBeExcludedBasedOnConductorCodes;
            try
            {
                dtParallelCables = new DataTable();
                dtParallelCables.Columns.Add("CableOID", typeof(int));
                dtParallelCables.Columns.Add("ParallelCableOID", typeof(int));

                countConductor = argListintersectingLineFeatureOIDs.Count;

                for (int iCount = 0; iCount < countConductor; iCount++)
                {
                    iConductorsAreSpatiallyRelated = 0;

                    for (int iCountInner = 0; iCountInner < countConductor; iCountInner++)
                    {
                        if (argListintersectingLineFeatureOIDs[iCount] != argListintersectingLineFeatureOIDs[iCountInner])
                        {

                            Conductor1 = argpriUGFeatureClass.GetFeature(argListintersectingLineFeatureOIDs[iCount]);
                            Conductor2 = argpriUGFeatureClass.GetFeature(argListintersectingLineFeatureOIDs[iCountInner]);

                            if (!IsConductorNotThreePhase(Conductor1) && IsConductorNotThreePhase(Conductor2))
                            {
                                if (_bLogNeededWhileExecuting)
                                Common._log.Info(Conductor1.OID + "is 3 phase but " + Conductor2.OID + " is not a three phase conductor.");
                                continue;
                            }

                            bConductorsAreSpatiallyRelated = GetSpatialRelationBetweenTwoConductors(Conductor1, Conductor2);

                            bConductorOneNeedsToBeExcludedBasedOnConductorCodes = IsConductorNeedsToBeExcludedBasedOnConductorCodes(Conductor1);
                            bConductorTwoNeedsToBeExcludedBasedOnConductorCodes = IsConductorNeedsToBeExcludedBasedOnConductorCodes(Conductor2);

                            bool b1 = IsConductorSubTypeNotForRuleOne(Conductor1);
                            bool b2 = IsConductorSubTypeNotForRuleOne(Conductor2);

                            if (bConductorsAreSpatiallyRelated)
                            {
                                if (_bLogNeededWhileExecuting)
                                Common._log.Info(Conductor1.OID + " and " + Conductor2.OID + " are spatially related to each other.");
                                iConductorsAreSpatiallyRelated++;
                                dtParallelCables.Rows.Add(Conductor1.OID, Conductor2.OID);
                            }
                        }
                    }
                    if (iConductorsAreSpatiallyRelated > iConductorsAreSpatiallyRelated_Final)
                    {
                        iConductorsAreSpatiallyRelated_Final = iConductorsAreSpatiallyRelated;
                        mainConductorOID = Conductor1.OID;
                        if (_bLogNeededWhileExecuting)
                        Common._log.Info(Conductor1.OID + " is the main conductor.");
                    }
                }

                if (mainConductorOID != 0)
                {
                    argListintersectingLineFeatureOIDs.Clear();
                    DataRow[] pRows = dtParallelCables.Select("CableOID=" + mainConductorOID);

                    //if (!argListintersectingLineFeatureOIDs.Contains(mainConductorOID))
                    //  argListintersectingLineFeatureOIDs.Add(mainConductorOID);
                    foreach (DataRow pRow in pRows)
                    {
                        if (!argListintersectingLineFeatureOIDs.Contains(pRow.Field<int>("ParallelCableOID")))
                            argListintersectingLineFeatureOIDs.Add(pRow.Field<int>("ParallelCableOID"));
                    }

                    iFilledDuctValue++;
                    mainConductorOID = GetSpatialRelationBetweenConductors_ComplexScenarios_Generic(argpriUGFeatureClass, argListintersectingLineFeatureOIDs, ref iFilledDuctValue);
                }
            }
            catch (Exception exp)
            {
                Common._log.Error("Function Name GetSpatialRelationBetweenConductors_ComplexScenarios_Generic. Exception: " + "Exception : " + exp.Message + " |  Stack Trace | " + exp.StackTrace+" at "+exp.ToString());
            }
            finally
            { 
            
            
            }
            return iFilledDuctValue;
        }

        /// <summary>
        /// This function checkd for spatial relationship between TWO given conductors ( further filledduct calculation for filledduct=3,4,5,6 or more )
        /// </summary>
        /// <param name="argpriUGFeatureClass"></param>
        /// <param name="argConductor1"></param>
        /// <param name="argConductor2"></param>
        /// <returns></returns>
        private static bool GetSpatialRelationBetweenTwoConductors(IFeature argConductor1, IFeature argConductor2)
        {
            bool bTwoConductorsAreSpatiallyRelated = false;
            ITopologicalOperator topoOperator = null;
            IGeometry geom = null;
            try
            {

                topoOperator = (ITopologicalOperator)argConductor1.Shape;
                geom = topoOperator.Buffer(_searchDistance * 2);

                bTwoConductorsAreSpatiallyRelated = CheckForOverlap(argConductor1.OID, geom, argConductor2.Shape, argConductor2.OID);

                // In this case passing search distance as double of serach distance, ie. if search distance is 6 Feet , passing 12 Feet here for correct calculations.
                //bTwoConductorsAreSpatiallyRelated = GetNearbyParallelPriUGConductor_ForRelationBwTwoConductors(argpriUGFeatureClass, argConductor1.OID, argConductor2, false,(searchDistance*2));
                //if (!bTwoConductorsAreSpatiallyRelated)
                //{
                //    bTwoConductorsAreSpatiallyRelated = GetNearbyParallelPriUGConductor_ForRelationBwTwoConductors(argpriUGFeatureClass, argConductor2.OID, argConductor1, false,(searchDistance * 2));
                //}
            }
            catch (Exception exp)
            {
                Common._log.Error("Function Name GetSpatialRelationBetweenTwoConductors. Exception: " + "Exception : " + exp.Message + " |  Stack Trace | " + exp.StackTrace+" at "+exp.ToString());
            }
            finally
            {
                if (topoOperator != null)
                {
                    Marshal.FinalReleaseComObject(topoOperator);
                    topoOperator = null;
                }
            }
            return bTwoConductorsAreSpatiallyRelated;
        }

        private static DataTable GetAllPrimaryConductorsAfterBuffer(IWorkspace argWorkspace, IFeatureClass argpriUGFeatureClass, DataTable argDTVerdiff_PriUG, bool agrbProcessRunningForChanges)
        {
            int oid_priUG = 0;   
            DataTable dtConductors = null;
            try
            {
                dtConductors = new DataTable();
                dtConductors.Columns.Add(ReadConfigurations.col_OBJECTID, typeof(string));
                dtConductors.Columns.Add(ReadConfigurations.col_GLOBALID, typeof(string));
                dtConductors.Columns.Add(ReadConfigurations.col_CIRCUITID, typeof(string));
                dtConductors.Columns.Add(ReadConfigurations.col_LOCALOFFICEID, typeof(string));
                dtConductors.Columns.Add(ReadConfigurations.col_SUBTYPECD, typeof(string));                           

                for (int count = 0; count < argDTVerdiff_PriUG.Rows.Count; count++)
                {
                    try
                    {
                        oid_priUG = Convert.ToInt32(argDTVerdiff_PriUG.Rows[count][ReadConfigurations.col_OBJECTID]);
                        Common.buffer_ChangeDetection_PriUG(oid_priUG, argpriUGFeatureClass, argWorkspace, ref dtConductors, agrbProcessRunningForChanges);
  
                        if (count % 1000 == 1)
                        {
                            Common._log.Info("Findng the nearby conductors in given buffer.Conductors processed till yet :" + count);
                        }
                    }
                    catch (Exception exp)
                    {
                        Common._log.Error("Function Name GetAllPrimaryConductorsAfterBuffer. Exception: " + "Exception : " + exp.Message + " |  Stack Trace | " + exp.StackTrace+" at "+exp.ToString());
                    }
                }               
            }
            catch (Exception exp)
            {
                Common._log.Error("Exception : " + exp.Message + " |  Stack Trace | " + exp.StackTrace+" at "+exp.ToString());
                ErrorCodeException ece = new ErrorCodeException(exp);
                Environment.ExitCode = ece.CodeNumber;
            }          
            return dtConductors;
        }

        private static DataTable GetAllPrimaryConductorsAfterBuffer_ForChangedConductors(IWorkspace argWorkspace, IFeatureClass argpriUGFeatureClass, DataTable argDTVerdiff_PriUG, bool agrbProcessRunningForChanges)
        {
            int oid_priUG = 0;
            DataTable dtConductors = null;
            try
            {
                dtConductors = new DataTable();
                dtConductors.Columns.Add(ReadConfigurations.col_OBJECTID, typeof(string));
                dtConductors.Columns.Add(ReadConfigurations.col_GLOBALID, typeof(string));
                dtConductors.Columns.Add(ReadConfigurations.col_CIRCUITID, typeof(string));
                dtConductors.Columns.Add(ReadConfigurations.col_COUNTY, typeof(string));
                dtConductors.Columns.Add(ReadConfigurations.col_SUBTYPECD, typeof(string));

                for (int count = 0; count < argDTVerdiff_PriUG.Rows.Count; count++)
                {
                    try
                    {
                        oid_priUG = Convert.ToInt32(argDTVerdiff_PriUG.Rows[count][ReadConfigurations.col_OBJECTID]);
                        Common.buffer_ChangeDetection_PriUG(oid_priUG, argpriUGFeatureClass, argWorkspace, ref dtConductors, agrbProcessRunningForChanges);

                        if (count % 1000 == 1)
                        {
                            Common._log.Info("Findng the nearby conductors in given buffer.Conductors processed till yet :" + count);
                        }
                    }
                    catch (Exception exp)
                    {
                        Common._log.Error("Function Name GetAllPrimaryConductorsAfterBuffer. Exception: " + "Exception : " + exp.Message + " |  Stack Trace | " + exp.StackTrace+" at "+exp.ToString());
                    }
                }
            }
            catch (Exception exp)
            {
                Common._log.Error("Exception : " + exp.Message + " |  Stack Trace | " + exp.StackTrace+" at "+exp.ToString());
            }
            finally
            {

            }
            return dtConductors;
        }
        
        private static void ComparetheCalculatedFilledductWithExistingFilledduct(IFeatureClass argpriUGFeatureClass, DataTable argChangedPriUG,string argMode)
        {
            DataRow[] pRows_Updated_PriUG = null;
            string sObjectId = null;
            string globalID =null;
            string filledduct=null;            
            DataRow drRow = null;
            DataRow drAddedRow = null;
            string filledduct_existing = null;           
            try
            {
                if (argChangedPriUG!=null && argChangedPriUG.Rows.Count > 0)
                {
                    pRows_Updated_PriUG = argChangedPriUG.Select(ReadConfigurations.col_STATUS + " = 'New'");
                    for (int i = 0; i < pRows_Updated_PriUG.Length; i++)
                    {
                        try
                        {
                            if (argMode == "MANUAL")
                            {
                                sObjectId = pRows_Updated_PriUG[i][ReadConfigurations.col_OBJECTID].ToString();
                                globalID = pRows_Updated_PriUG[i][ReadConfigurations.col_GLOBALID].ToString();
                                filledduct = pRows_Updated_PriUG[i][ReadConfigurations.col_FILLEDDUCT].ToString();
                            }
                            else if (argMode == "AUTO")
                            {
                                sObjectId = pRows_Updated_PriUG[i][ReadConfigurations.col_OBJECTID].ToString();
                                globalID = pRows_Updated_PriUG[i][ReadConfigurations.col_GLOBALID].ToString();
                                filledduct = pRows_Updated_PriUG[i][ReadConfigurations.col_FILLEDDUCT].ToString();
                            }

                            // If we need to overwrite the filledduct values then giving a default value so that whatever we calculate will be stored
                            if (MainClass._boverwriteFilledductValue)
                            {
                                filledduct_existing = "100";
                            }
                            else
                            {
                                filledduct_existing = Common.checking_FilledDuctChange(globalID, argpriUGFeatureClass);
                            }

                            if ((string.IsNullOrEmpty(filledduct) && string.IsNullOrEmpty(filledduct_existing)) || (filledduct == "0" && filledduct_existing == "0"))
                            {
                                Common._log.Info("The calculated filleduct value for Primary UG Conductor: Object Id: " + sObjectId + " is matching with existing value, FilledDuct Value: " + filledduct);
                            }
                            else if (filledduct_existing != filledduct)
                            {
                                //Changing below code and keeping seperate for manual and auto 
                                if (argMode == "MANUAL")
                                {
                                    //pRows_Updated_PriUG[i]["PROCESSED_ON"] = DateTime.Now.ToString("MM/dd/yyyy");
                                    drRow = pRows_Updated_PriUG[i];
                                    drAddedRow = _dtToSaveFinalData.Rows.Add();
                                    drAddedRow[ReadConfigurations.col_OBJECTID] = drRow[ReadConfigurations.col_OBJECTID];
                                    drAddedRow[ReadConfigurations.col_GLOBALID] = drRow[ReadConfigurations.col_GLOBALID];
                                    drAddedRow[ReadConfigurations.col_FILLEDDUCT] = drRow[ReadConfigurations.col_FILLEDDUCT];
                                    drAddedRow[ReadConfigurations.col_STATUS] = drRow[ReadConfigurations.col_STATUS];
                                    drAddedRow[ReadConfigurations.col_REQUESTED_ON] = drRow[ReadConfigurations.col_REQUESTED_ON];
                                    drAddedRow[ReadConfigurations.col_USER] = drRow[ReadConfigurations.col_REQUESTED_BY];
                                }
                                else if (argMode == "AUTO")
                                {
                                    drRow = pRows_Updated_PriUG[i];
                                    drAddedRow = _dtToSaveFinalData.Rows.Add();
                                    drAddedRow[ReadConfigurations.col_OBJECTID] = drRow[ReadConfigurations.col_OBJECTID];
                                    drAddedRow[ReadConfigurations.col_GLOBALID] = drRow[ReadConfigurations.col_GLOBALID];
                                    drAddedRow[ReadConfigurations.col_CIRCUITID] = drRow[ReadConfigurations.col_CIRCUITID];
                                    drAddedRow[ReadConfigurations.col_LOCALOFFICEID] = drRow[ReadConfigurations.col_LOCALOFFICEID];
                                    drAddedRow[ReadConfigurations.col_SUBTYPECD] = drRow[ReadConfigurations.col_SUBTYPECD];
                                    drAddedRow[ReadConfigurations.col_FILLEDDUCT] = drRow[ReadConfigurations.col_FILLEDDUCT];
                                    drAddedRow[ReadConfigurations.col_Exclusion] = drRow[ReadConfigurations.col_Exclusion];
                                    drAddedRow[ReadConfigurations.col_Is50Scale] = drRow[ReadConfigurations.col_Is50Scale];
                                    drAddedRow[ReadConfigurations.col_OIDS_Needed] = drRow[ReadConfigurations.col_OIDS_Needed];
                                    drAddedRow[ReadConfigurations.col_OIDS_All] = drRow[ReadConfigurations.col_OIDS_All];
                                    drAddedRow[ReadConfigurations.col_CONDUITTYPE] = drRow[ReadConfigurations.col_CONDUITTYPE];
                                    drAddedRow[ReadConfigurations.col_STATUS] = drRow[ReadConfigurations.col_STATUS];
                                    drAddedRow[ReadConfigurations.col_REQUESTED_ON] = drRow[ReadConfigurations.col_PROCESSED_ON];
                                    drAddedRow[ReadConfigurations.col_USER] = ReadConfigurations.val_SYSTEM;
                                }

                                Common._log.Info("Primary UG Conductor: Object Id: " + sObjectId + ", FilledDuct Value to be updated : " + filledduct);
                            }
                            else
                            {
                                Common._log.Info("The calculated filleduct value for Primary UG Conductor: Object Id: " + sObjectId + " is matching with existing value, FilledDuct Value: " + filledduct);
                            }
                        }
                        catch (Exception exp)
                        {
                            Common._log.Error("Function Name ComparetheCalculatedFilledductWithExistingFilledduct. Exception: " + "Exception : " + exp.Message + " |  Stack Trace | " + exp.StackTrace + " at " + exp.ToString());
                        }
                    }
                }
                else
                {
                    Common._log.Info("No rows found in table.");
                }
            }
            catch (Exception exp)
            {
                Common._log.Error("Function Name ComparetheCalculatedFilledductWithExistingFilledduct. Exception: " + "Exception : " + exp.Message +" |  Stack Trace | " + exp.StackTrace+" at "+exp.ToString());
            }           
        }

        private static bool UpdateStagingtableStatusAfterPostingVersion()
        {
            bool bProcessSuccess = false;
            DataRow[] pRows_Updated_PriUG = null;
            string strConnString = null;
            string strQuery=null;
            try
            {
                Common._log.Info("Updating status in stage tables after version Post-Start.");
                // m4jf edgisrearch 919
               // strConnString = ConfigurationManager.AppSettings["ConnectionString_wip"];
                strConnString = ReadEncryption.GetConnectionStr(ConfigurationManager.AppSettings["WIP_ConnectionStr_WEBR"].ToUpper());
                strQuery = "SELECT * FROM " + ConfigurationManager.AppSettings["tableNameForManualUpdates"] + " WHERE STATUS='New'";
                //Getting the data here to update status for manual updates
                Common._dtmanualPriUG = DBHelper.GetDataTable(strConnString, strQuery);

                if (Common._dtmanualPriUG !=null && Common._dtmanualPriUG.Rows.Count >= 1)
                {                    
                    try
                    {
                        pRows_Updated_PriUG = Common._dtmanualPriUG.Select("STATUS = 'New'");
                        DBHelper.UpdateQuery(strConnString, pRows_Updated_PriUG, ConfigurationManager.AppSettings["tableNameForManualUpdates"]);
                    }
                    catch (Exception exp)
                    {
                        Common._log.Error("Exception : " + exp.Message + " |  Stack Trace | " + exp.StackTrace+" at "+exp.ToString());
                    }
                }

                //Getting the data here to update status for auto updates
                strConnString = ReadConfigurations.ConnectionString_pgedata ;
                strQuery = "select * from " + ConfigurationManager.AppSettings["tableNameToSaveFinalData"] + " WHERE STATUS='New'";
                MainClass._dtToSaveFinalData = DBHelper.GetDataTable(strConnString, strQuery);

                if (MainClass._dtToSaveFinalData.Rows.Count >= 1)
                {                   
                    try
                    {
                        pRows_Updated_PriUG = MainClass._dtToSaveFinalData.Select(ReadConfigurations.col_STATUS+ " = 'New'");
                        DBHelper.UpdateQuery(strConnString, pRows_Updated_PriUG);
                    }
                    catch (Exception exp)
                    {
                        Common._log.Error("Exception : " + exp.Message + " |  Stack Trace | " + exp.StackTrace+" at "+exp.ToString());
                    }
                }

                Common._log.Info("Updating status in stage tables after version post-End.");
                bProcessSuccess = true;
            }
            catch (Exception exp)
            {
                Common._log.Error("Function Name UpdateStagingtableStatusAfterPostingVersion. " + "Exception : " + exp.Message +" |  Stack Trace | " + exp.StackTrace+" at "+exp.ToString());
            }
            return bProcessSuccess;
        }

        /// <summary>
        /// This function will check whether the conductor lies in 50 scale or not
        /// </summary>
        /// <param name="argfc_fifty_ScaleBoundary"></param>
        /// <param name="argdtAll50ScaleReocrds"></param>
        /// <param name="argpFeaturePriUGCond"></param>
        private static bool Check50Scale(IFeatureClass argfc_fifty_ScaleBoundary,DataTable argdtAll50ScaleReocrds,IFeature argpFeaturePriUGCond,bool argIsProcessBulk)
        {
            ISpatialFilter spatialFilter = null;
            IFeatureCursor pfeatCursor = null;
            IFeature pFeatureOutPut = null;
            bool bIs50Scale = false;
            DataRow [] pRows = null;
            try
            {
                if (argIsProcessBulk)
                {

                    pRows = argdtAll50ScaleReocrds.Select(ReadConfigurations.col_OBJECTID+"=" + argpFeaturePriUGCond.OID);

                    if (pRows.Length > 0)
                    {
                        bIs50Scale = true;
                    }
                    else
                    {
                        bIs50Scale = false;
                    }

                    if (bIs50Scale)
                    {
                        _searchDistance = Convert.ToInt32(ReadConfigurations.GetValue(ReadConfigurations.searchDistance_50));                      
                    }
                    else
                    {
                        _searchDistance = Convert.ToInt32(ReadConfigurations.GetValue(ReadConfigurations.searchDistance));
                    }
                }
                else
                {
                    try
                    {
                        spatialFilter = new SpatialFilterClass();
                        spatialFilter.Geometry = argpFeaturePriUGCond.Shape;
                        spatialFilter.SpatialRel = esriSpatialRelEnum.esriSpatialRelIntersects;
                        pfeatCursor = argfc_fifty_ScaleBoundary.Search(spatialFilter, false);
                        pFeatureOutPut = pfeatCursor.NextFeature();
                        while (pFeatureOutPut != null)
                        {
                            bIs50Scale = true;
                            break;
                        }

                        if (bIs50Scale)
                        {
                            _searchDistance = Convert.ToInt32(ReadConfigurations.GetValue(ReadConfigurations.searchDistance_50));                         
                        }
                        else
                        {
                            _searchDistance = Convert.ToInt32(ReadConfigurations.GetValue(ReadConfigurations.searchDistance));
                        }                       
                    }
                    catch (Exception exp)
                    {
                        Common._log.Error("Exception : " + exp.Message + " |  Stack Trace | " + exp.StackTrace+" at "+exp.ToString());
                        MainClass._exception = exp.Message;
                    }
                    finally
                    {
                        if (pfeatCursor != null)
                        {
                            Marshal.FinalReleaseComObject(pfeatCursor);
                            pfeatCursor = null;
                        }

                        if (spatialFilter != null)
                        {
                            Marshal.FinalReleaseComObject(spatialFilter);
                            spatialFilter = null;
                        }
                    }
                }                
            }
            catch (Exception exp)
            {
                Common._log.Error("Exception : " + exp.Message + " |  Stack Trace | " + exp.StackTrace+" at "+exp.ToString());
                MainClass._exception = exp.Message;
            }
            return bIs50Scale;
        }

        /// <summary>
        /// This function will check type of conduit for primary UG conductor
        /// </summary>
        /// <param name="argpFeaturePriUGCond"></param>
        /// <param name="argIsProcessBulk"></param>
        /// <returns></returns>
        public static string GetTypeOfConduit(IFeature argpFeaturePriUGCond,bool argIsProcessBulk,bool argbConductorInBuffers)
        {
            string typeOfConduit = null;             
            try
            {
                //Call a fuction here to check Conduit System Type
                if (argIsProcessBulk)
                {
                    // In case of bulk updates , type of conduit is 2 
                    if (Bulk_Process._isLeftRecords || Bulk_Process._isRunningProcessForChanges)
                    {
                        typeOfConduit = ProcessPriUGCond_GetTypeOfConduit(argpFeaturePriUGCond);
                    }
                    else
                    {
                        typeOfConduit = "2";
                    }
                }
                else if(MainClass._bisRunningProcessForChanges)
                {
                    if (argbConductorInBuffers)
                    {
                        if (Bulk_Process._dtAllRule1Reocrds == null)
                        {
                            string queryToFetchdata = ReadConfigurations.GetValue(ReadConfigurations.queryToGetRule1Records);
                            string strConnString_pgedata = ConfigurationManager.AppSettings["ConnectionString_pgedata"];
                            Bulk_Process._dtAllRule1Reocrds = DBHelper.GetDataTable(strConnString_pgedata, queryToFetchdata);
                            Common._log.Info("All Rule 1 records fetch success. Total records are:" + Bulk_Process._dtAllRule1Reocrds.Rows.Count);

                            if (Bulk_Process._dtAllRule1Reocrds.Select("OBJECTID='" + argpFeaturePriUGCond.OID + "'").Length > 0)
                            {
                                typeOfConduit = "2";
                            }
                            else
                            {
                                typeOfConduit = string.Empty;
                            }  
                        }
                        else
                        {
                            if (Bulk_Process._dtAllRule1Reocrds.Select("OBJECTID='" + argpFeaturePriUGCond.OID + "'").Length > 0)
                            {
                                typeOfConduit = "2";
                            }
                            else
                            {
                                typeOfConduit = string.Empty;
                            }                        
                        }
                    }
                    else
                    {
                        typeOfConduit = "2";
                    }
                } 
                else 
                {
                    //if (argbConductorInBuffers)
                    //{
                    //    //Check if length is less than 100 Feet , no need to check conduit subtype
                    //    if (IsConductorLessThanCutOffLength(argpFeaturePriUGCond, MainClass._cutoffLengthConductorToExclude))
                    //    {
                    //        typeOfConduit = string.Empty;
                    //    }
                    //    else
                    //    {
                    //        typeOfConduit = ProcessPriUGCond_GetTypeOfConduit(argpFeaturePriUGCond);
                    //    }
                    //}
                    //else
                    //{
                    //    typeOfConduit = ProcessPriUGCond_GetTypeOfConduit(argpFeaturePriUGCond);
                    //}

                    typeOfConduit = ProcessPriUGCond_GetTypeOfConduit(argpFeaturePriUGCond);
                }                
            }
            catch (Exception exp)
            {
                Common._log.Error("Exception : " + exp.Message + " |  Stack Trace | " + exp.StackTrace+" at "+exp.ToString());
            }
            return typeOfConduit;
        }

        /// <summary>
        /// This function will calculate filledduct for given primary UG conductor
        /// </summary>
        /// <returns></returns>
        private static int GetFilledductValue(IFeatureClass argpriUGFeatureClass, IFeature argpFeaturePriUGCond, string argStrtypeOfConduit, ref string argStrExclusion)
        {
            int filledduct = 0;
            int countAdjacentFeatures = 0;
            string comments = null;                       
            bool bIsThreePhaseConductor = false;
            int countConductorSpatiallyRelated = 0;
            List<int> oldListintersectingLineFeatureOIDs = null;
            try
            {
                _listintersectingLineFeatureOIDs.Clear();
                _listintersectingAllLineFeatureOIDs.Clear();
                _dtCondcutorWithCodes.Clear();

                //This function returns the cables under consideration for single PriUG conductor
                if (argStrtypeOfConduit == "0" || argStrtypeOfConduit == "2" || argStrtypeOfConduit == "3")
                {
                    bIsThreePhaseConductor = !IsConductorNotThreePhase(argpFeaturePriUGCond);

                    // Conductor doesn't have a Conduit system, Conductor has single conduit with sutype CIC or Conduit // Business rule set 1                     
                    ProcessPriUGCond_ForSingleConductor(argpFeaturePriUGCond, ref countAdjacentFeatures, bIsThreePhaseConductor, ref argStrExclusion);
                    if (_listintersectingAllLineFeatureOIDs.Count == 0)
                    {
                        //This is the case of DUCTFILLED=1                       
                        filledduct = 1;
                    }
                    else if (_listintersectingAllLineFeatureOIDs.Count == 1)
                    {
                        filledduct = _dtCondcutorWithCodes.Select(ReadConfigurations.col_WEIGHT + "=1").Length + 1;

                        //This is the case of DUCTFILLED=2   
                        _listintersectingLineFeatureOIDs.Clear();
                        foreach (int value in _listintersectingAllLineFeatureOIDs)
                        {
                            _listintersectingLineFeatureOIDs.Add(value);
                        }
                    }
                    else if (_listintersectingAllLineFeatureOIDs.Count >= 2)
                    {
                        bool argbAllExcluded = false;

                        //This is the case of DUCTFILLED=3 or more or less (based on spatial analysis for other conductors) , where we need to analyze the other conductors found
                        GetSpatialRelationBetweenConductors_NonComplexScenarios(argpriUGFeatureClass, argpFeaturePriUGCond, argStrtypeOfConduit, countAdjacentFeatures, _listintersectingAllLineFeatureOIDs, ref countConductorSpatiallyRelated, ref argbAllExcluded);
                        //This function will return count for pairs that are spatially related with each other  

                        if (countConductorSpatiallyRelated == 0)
                        {
                            //No pair spatially related , This is the case of DUCTFILLED=2
                            //Check if we can put ObjectID of  spatially related conductors here in datatable , however storing the objectid in log.                                 
                            if (argbAllExcluded)
                            {
                                filledduct = 1;
                            }
                            else
                            {
                                filledduct = 2;
                            }
                        }
                        else if (countConductorSpatiallyRelated == 1)
                        {
                            //No pair spatially related , This is the case of DUCTFILLED=2
                            //Check if we can put ObjectID of  spatially related conductors here in datatable , however storing the objectid in log.
                            filledduct = 2;
                        }
                        else if (countConductorSpatiallyRelated == 2)
                        {
                            //Just one pair spatially related , This is the case of DUCTFILLED=3
                            //Check if we can put ObjectID of  spatially related conductors here in datatable , however storing the objectid in log.                               
                            filledduct = 3;
                        }
                        else if (countConductorSpatiallyRelated > 2)
                        {
                            // This is the case for complex scenarios and we need to drill down further to calculate filled Duct
                            // listintersectingLineFeatureOIDs.Count = 2 cases will be covered earlier so starting with count 3
                            if (_listintersectingLineFeatureOIDs.Count == 3)
                            {
                                if (countConductorSpatiallyRelated == 3)
                                {
                                    // This is the case when 3 intersecting cable found and on further analysis, total 3 pairs found spatially related that means all cables are spatially related , so filledduct will be 4 , exp OID=6550233
                                    //dtconductorFILLEDDUCT.Rows.Add(pFeaturePriUGCond.OID, typeOfConduit, 4);
                                    filledduct = 4;
                                    //Check if we can put ObjectID of  spatially related conductors here in datatable , however storing the objectid in log.
                                    if (_bLogNeededWhileExecuting)
                                    {
                                        //Common._log.Info("$" + argpFeaturePriUGCond.OID + "$" + argStrtypeOfConduit + "$" + countAdjacentFeatures + "$" + listintersectingLineFeatureOIDs.Count + "$" + 4 + "$" + listintersectingLineFeatureOIDs[0] + "$" + searchDistance);
                                        Common._log.Info("This is the case when 3 intersecting cable found and on further analysis, total 3 pairs found spatially related that means all cables are spatially related , so filledduct will be 4,OID=" + argpFeaturePriUGCond.OID);
                                    }
                                }
                                else
                                {
                                    //There is only one condition left and that is  countConductorSpatiallyRelated=2 for this case , hence setting filledduct=3                                                                              
                                    filledduct = 3;
                                    //Check if we can put ObjectID of  spatially related conductors here in datatable , however storing the objectid in log.
                                    if (_bLogNeededWhileExecuting)
                                    {
                                        //Common._log.Info("$" + argpFeaturePriUGCond.OID + "$" + argStrtypeOfConduit + "$" + countAdjacentFeatures + "$" + listintersectingLineFeatureOIDs.Count + "$" + 3 + "$" + listintersectingLineFeatureOIDs[0] + "$" + searchDistance);
                                        //Common._log.Info("There is only one condition left and that is  countConductorSpatiallyRelated=2 for this case , hence setting filledduct=3,OID=" + argpFeaturePriUGCond.OID);
                                    }
                                }
                            }
                            else
                            {
                                oldListintersectingLineFeatureOIDs = new List<int>(MainClass._listintersectingLineFeatureOIDs);

                                if (_bLogNeededWhileExecuting)
                                {
                                    Common._log.Info("These are the complex scenarios,listintersectingLineFeatureOIDs.Count=" + _listintersectingLineFeatureOIDs.Count + ",countConductorSpatiallyRelated=" + countConductorSpatiallyRelated + ",OID=" + argpFeaturePriUGCond.OID);
                                }

                                countConductorSpatiallyRelated = 0;
                                countConductorSpatiallyRelated = GetSpatialRelationBetweenConductors_ComplexScenarios(argpriUGFeatureClass, argpFeaturePriUGCond, argStrtypeOfConduit, countAdjacentFeatures, _listintersectingLineFeatureOIDs, ref countConductorSpatiallyRelated);

                                //Setting the filledduct value returned by the function
                                filledduct = countConductorSpatiallyRelated;
                                //Common._log.Info("$" + argpFeaturePriUGCond.OID + "$" + argStrtypeOfConduit + "$" + countAdjacentFeatures + "$" + listintersectingLineFeatureOIDs.Count + "$" + filledduct + "$" + listintersectingLineFeatureOIDs[0] + "$" + searchDistance);

                                MainClass._listintersectingLineFeatureOIDs = new List<int>(oldListintersectingLineFeatureOIDs);
                            }
                        }
                        else
                        {
                            //countConductorSpatiallyRelated = GetSpatialRelationBetweenConductors_ComplexScenarios(argpriUGFeatureClass, argpFeaturePriUGCond, argStrtypeOfConduit, countAdjacentFeatures, _listintersectingLineFeatureOIDs, ref countConductorSpatiallyRelated);
                            //filledduct = countConductorSpatiallyRelated;
                        }
                        //else if (countConductorSpatiallyRelated == 2)
                        //{
                        //    //Just one pair spatially related , This is the case of DUCTFILLED=3
                        //    //Check if we can put ObjectID of  spatially related conductors here in datatable , however storing the objectid in log.
                        //    filledduct = 3;
                        //}
                        //else if (countConductorSpatiallyRelated > 1)
                        //{      
                        //    // This is the case for complex scenarios and we need to drill down further to calculate filled Duct
                        //    // listintersectingLineFeatureOIDs.Count = 2 cases will be covered earlier so starting with count 3
                        //    if (_listintersectingLineFeatureOIDs.Count == 3)
                        //    {
                        //        if (countConductorSpatiallyRelated == 3)
                        //        {
                        //            // This is the case when 3 intersecting cable found and on further analysis, total 3 pairs found spatially related that means all cables are spatially related , so filledduct will be 4 , exp OID=6550233
                        //            //dtconductorFILLEDDUCT.Rows.Add(pFeaturePriUGCond.OID, typeOfConduit, 4);
                        //            filledduct = 4;
                        //            //Check if we can put ObjectID of  spatially related conductors here in datatable , however storing the objectid in log.
                        //            if (bLogNeededWhileExecuting)
                        //            {
                        //                //Common._log.Info("$" + argpFeaturePriUGCond.OID + "$" + argStrtypeOfConduit + "$" + countAdjacentFeatures + "$" + listintersectingLineFeatureOIDs.Count + "$" + 4 + "$" + listintersectingLineFeatureOIDs[0] + "$" + searchDistance);
                        //                Common._log.Info("This is the case when 3 intersecting cable found and on further analysis, total 3 pairs found spatially related that means all cables are spatially related , so filledduct will be 4,OID=" + argpFeaturePriUGCond.OID);
                        //            }
                        //        }
                        //        else
                        //        {
                        //            //There is only one condition left and that is  countConductorSpatiallyRelated=2 for this case , hence setting filledduct=3                                                                              
                        //            filledduct = 3;
                        //            //Check if we can put ObjectID of  spatially related conductors here in datatable , however storing the objectid in log.
                        //            if (bLogNeededWhileExecuting)
                        //            {
                        //                //Common._log.Info("$" + argpFeaturePriUGCond.OID + "$" + argStrtypeOfConduit + "$" + countAdjacentFeatures + "$" + listintersectingLineFeatureOIDs.Count + "$" + 3 + "$" + listintersectingLineFeatureOIDs[0] + "$" + searchDistance);
                        //                //Common._log.Info("There is only one condition left and that is  countConductorSpatiallyRelated=2 for this case , hence setting filledduct=3,OID=" + argpFeaturePriUGCond.OID);
                        //            }
                        //        }
                        //    }
                        //    else
                        //    {
                        //        //Need to think for this condition  
                        //        Common._log.Info("These are the complex scenarios,listintersectingLineFeatureOIDs.Count=" + _listintersectingLineFeatureOIDs.Count + ",countConductorSpatiallyRelated=" + countConductorSpatiallyRelated + ",OID=" + argpFeaturePriUGCond.OID);
                        //        countConductorSpatiallyRelated = 0;
                        //        countConductorSpatiallyRelated = GetSpatialRelationBetweenConductors_ComplexScenarios(argpriUGFeatureClass, argpFeaturePriUGCond, argStrtypeOfConduit, countAdjacentFeatures, _listintersectingLineFeatureOIDs, ref countConductorSpatiallyRelated);

                        //        //Setting the filledduct value returned by the function
                        //        filledduct = countConductorSpatiallyRelated;
                        //        //Common._log.Info("$" + argpFeaturePriUGCond.OID + "$" + argStrtypeOfConduit + "$" + countAdjacentFeatures + "$" + listintersectingLineFeatureOIDs.Count + "$" + filledduct + "$" + listintersectingLineFeatureOIDs[0] + "$" + searchDistance);
                        //    }
                        //}
                    }
                }
                else if (argStrtypeOfConduit == "1")
                {
                    if (Bulk_Process._isRunningProcessForChanges)
                    {
                        Common._log.Info("The process is running for changes so not calculating filledduct for feature : " + argpFeaturePriUGCond.OID);
                    }
                    else if (MainClass._processtype == "SCHEDULE")
                    {
                        filledduct = GetFilledDuctValueForDuctBank(argpFeaturePriUGCond, ref argStrExclusion);
                    }
                    else
                    {
                        if (Bulk_Process._isLeftRecords)
                        {
                            filledduct = GetFilledDuctValueForDuctBank(argpFeaturePriUGCond, ref argStrExclusion);
                        }
                        else
                        {
                            // Conduit system subtype is Duct Bank
                            filledduct = GetDuctFilledForPrimaryUnderGroundDuctBankType(argpFeaturePriUGCond.OID);
                            if (filledduct == 0)
                            {
                                Common._log.Error("Filledduct value coming 0 for OID =" + argpFeaturePriUGCond.OID);
                            }
                        }
                    }
                }
                else if (argStrtypeOfConduit == "MIXED")
                {
                    // exceptions 
                    filledduct = 0;
                    comments = "Conductor having related records of conduit with different subtypes.";
                    //CIT_Daily_Updates
                    _dtconductorExceptions.Rows.Add(argpFeaturePriUGCond.OID, comments);
                    Common._log.Info("Conductor OID : " + argpFeaturePriUGCond.OID + " is " + comments);
                }
                else if (argStrtypeOfConduit == "MULTI")
                {
                    // exceptions 
                    filledduct = 0;
                    comments = "Conductor having multiple related records of conduit with subtype CIC/Conduit.";
                    //CIT_Daily_Updates
                    //_dtconductorExceptions.Rows.Add(argpFeaturePriUGCond.OID, comments);
                    Common._log.Info("Conductor OID : " + argpFeaturePriUGCond.OID + " is " + comments);
                }
                else if (string.IsNullOrEmpty(argStrtypeOfConduit))
                {
                    filledduct = 0;
                    comments = "Could not determine conduit subtype for conductor.";
                    _dtconductorExceptions.Rows.Add(argpFeaturePriUGCond.OID, comments);
                    Common._log.Info("Conductor OID : " + argpFeaturePriUGCond.OID + " is " + comments);
                }
                else
                {
                    filledduct = 0;
                    comments = "All other exceptions.";
                    _dtconductorExceptions.Rows.Add(argpFeaturePriUGCond.OID, comments);
                    Common._log.Info("Conductor OID : " + argpFeaturePriUGCond.OID + " is " + comments);
                }
            }
            catch (Exception exp)
            {
                Common._log.Error("Exception : " + exp.Message + " |  Stack Trace | " + exp.StackTrace+" at "+exp.ToString());
                filledduct = 0;
                MainClass._exception = exp.Message;
            }
            return filledduct;
        }

        /// <summary>
        /// This function will perform required processing and insert data in datatable for daily process
        /// </summary>
        private static void PrcessForDailyUpdates(IFeature argpFeaturePriUGCond,int argifilledduct)
        {
            try
            {
                string PriUG_GlobalId = Convert.ToString(argpFeaturePriUGCond.get_Value(argpFeaturePriUGCond.Fields.FindField(ReadConfigurations.col_GLOBALID)));                
                string status = ReadConfigurations.val_New;
                DateTime processed_on = DateTime.Today.Date;
                _updated_PriUG.Rows.Add(argpFeaturePriUGCond.OID, PriUG_GlobalId,argifilledduct, status, processed_on);
            }
            catch (Exception exp)
            {
                Common._log.Error("Exception : " + exp.Message + " |  Stack Trace | " + exp.StackTrace+" at "+exp.ToString());
            }
        }

        private static bool DataUpdateAndVersionOperations(IWorkspace workspace_edgis,ref string argStrVersionName)
        {
            bool bOperationSuccess = false;
            IVersion2 PriUG_SuccessDailyVersion;
            string TempVesionName = null;
            bool checkPost_Status=false;   
            //Code block to disable Auto Updaters // If we will not disable pFeature.Store() will throw internittent exceptions
            Type type = Type.GetTypeFromProgID("mmGeoDatabase.MMAutoUpdater");
            IMMAutoUpdater auController = (IMMAutoUpdater)Activator.CreateInstance(type);
            mmAutoUpdaterMode prevAUMode = auController.AutoUpdaterMode;
            auController.AutoUpdaterMode = mmAutoUpdaterMode.mmAUMNoEvents;
            try
            {
                PriUG_SuccessDailyVersion = VersionOperations.CreateVersion(workspace_edgis);

                if (PriUG_SuccessDailyVersion == null)
                {
                    Common._log.Error("Version Creation Failed.");
                    return false;
                }

                TempVesionName = PriUG_SuccessDailyVersion.VersionName;
                argStrVersionName = TempVesionName;
                Common._log.Info("Version Created Successfully. Version Name : " + PriUG_SuccessDailyVersion.VersionName.ToUpper());
                //5.Create version - End

                //6.Make edits in version - Start
                IFeatureWorkspace featversionWorkspace = (IFeatureWorkspace)PriUG_SuccessDailyVersion;
                IWorkspaceEdit editSession = (IWorkspaceEdit)PriUG_SuccessDailyVersion;
                if (Editor.EditState == EditState.StateNotEditing)
                {
                    Editor.StartEditing((IWorkspace)editSession);
                    Editor.StartOperation();
                }

                IFeatureClass FC_PriUG = featversionWorkspace.OpenFeatureClass(ReadConfigurations.FC_PRIUGCONDUCTOR);
                bool priUGUpdateIsSuccessFull = Common.UpdatePriUGConductorDataInVersion(FC_PriUG, _dtToSaveFinalData);

                if (priUGUpdateIsSuccessFull == false)
                {
                    Common._log.Error("Unable to update changed primary UG conductors.");
                }

                if (Editor.EditState == EditState.StateEditing)
                {
                    Editor.StopOperation("");
                    Editor.StopEditing(true);
                }

                Common._log.Info("Editing in version " + TempVesionName + " Completed.");
                //Make edits in version - End
              
                bOperationSuccess = priUGUpdateIsSuccessFull;
            }
            catch (Exception exp)
            {
                bOperationSuccess = false;
               Common._log.Error("Exception : " + exp.Message + " |  Stack Trace | " + exp.StackTrace+" at "+exp.ToString());
            }
            finally
            {
                auController.AutoUpdaterMode = prevAUMode;
                //Insert into CIT_VERSION_STATUS table 
                Common.MakeChangesForCitVersionTable(checkPost_Status,TempVesionName,"INSERT");
            }
            return bOperationSuccess;
        }

        private static string GetVersionToPost(string argStrConnString)
        {
            string versionName = null;
            string query = null;
            try
            {
                query = ReadConfigurations.GetValue( ReadConfigurations.queryToGetVersionName);
                DataRow pRow = DBHelper.GetSingleDataRowByQuery(argStrConnString, query);
                versionName = Convert.ToString(pRow[ReadConfigurations.col_VERSION_NAME]);
            }
            catch (Exception exp)
            {
                Common._log.Error("Exception : " + exp.Message + " |  Stack Trace | " + exp.StackTrace+" at "+exp.ToString());
            }
            return versionName;
        }

        //private static bool VersionPostOperations(IWorkspace workspace_edgis, string strConnString_edgis, string argStrVersionName)
        //{
        //    bool bOperationSuccess = false;
        //    IVersion2 PriUG_SuccessDailyVersion;           
        //    bool checkPost_Status = false;
        //    bool bVersionDelete = false;            
        //    try
        //    {
        //        PriUG_SuccessDailyVersion = VersionOperations.GetRequiredVersion(workspace_edgis, argStrVersionName);

        //        if (PriUG_SuccessDailyVersion == null)
        //        {
        //            Common._log.Info("Version " + argStrVersionName + " not found in geo database.");
        //        }
        //        else
        //        {
        //            //Reconcile & Post //Post version - Start    
        //            checkPost_Status = VersionOperations.ReconcileAndPostVersion(PriUG_SuccessDailyVersion);
        //            //7.Post version - End

        //            Marshal.ReleaseComObject(PriUG_SuccessDailyVersion); PriUG_SuccessDailyVersion = null;
        //            bVersionDelete = VersionOperations.DeleteVersion(argStrVersionName, strConnString_edgis);

        //            bOperationSuccess = true;
        //        }
        //    }
        //    catch (Exception exp)
        //    {
        //        bOperationSuccess = false;
        //        Common._log.Error("Exception : " + exp.Message + " |  Stack Trace | " + exp.StackTrace+" at "+exp.ToString());
        //    }
        //    finally
        //    {                
        //        //Common.MakeChangesForCitVersionTable(checkPost_Status, bVersionDelete, argStrVersionName,"UPDATE");
        //    }
        //    return bOperationSuccess;
        //}


        /// <summary>
        /// This function calculates and return filledduct value for Duct Bank case
        /// </summary>
        /// <param name="argFeaturePriUGCond"></param>
        /// <returns></returns>
        private static int GetFilledDuctValueForDuctBank(IFeature argFeaturePriUGCond,ref string argStrExclusion)
        {
            IEnumRelationshipClass relClassesConduit = null;
            IRelationshipClass relConduit = null;
            ESRI.ArcGIS.esriSystem.ISet resultSetConduits = null;
            IFeature pfeatConduit = null;
            IEnumRelationshipClass relClassesPriUG = null;
            IRelationshipClass relPriUG = null;
            ESRI.ArcGIS.esriSystem.ISet resultSetPriUGs = null;
            IFeature pfeatPriUG = null;
            string strStatusConduit = null;
            string strStatusPriUG = null;
            List<int> lstPriUGOIDs = new List<int>();
            int filledductValue = 0;
            string strPhaseParentConductor = string.Empty;
            string strPhaseChildConductor = string.Empty;
            bool bConductorIsThreePhase = false;
            string comment = null;
            try
            {
                //Checking the conductor based on status ,Filledduct will be 0/Null
                if (IsConductorNotInService(argFeaturePriUGCond))
                {
                    comment = "Calculating FILLEDDUCT for conductor with OID:" + argFeaturePriUGCond.OID + " and setting filledduct=0 because it is excluded based on status.";
                    if (_bLogNeededWhileExecuting)
                        Common._log.Info(comment);
                    filledductValue = 0;
                    argStrExclusion = ReadConfigurations.strExclusionBasedOnStatus;
                    return filledductValue;
                }    

                strPhaseParentConductor = Convert.ToString(argFeaturePriUGCond.get_Value(argFeaturePriUGCond.Fields.FindField(ReadConfigurations.col_PHASEDESIGNATION)));
                if (strPhaseParentConductor == "7")
                    bConductorIsThreePhase = true;

                relClassesConduit = ((IFeatureClass)argFeaturePriUGCond.Class).get_RelationshipClasses(esriRelRole.esriRelRoleDestination);
                relConduit = relClassesConduit.Next();
                while (relConduit != null)
                {
                    if ( ((IDataset)relConduit.OriginClass).Name.ToUpper() == ReadConfigurations.FC_CONDUITSYSTEM)
                    {  
                        resultSetConduits = relConduit.GetObjectsRelatedToObject(argFeaturePriUGCond);
                        pfeatConduit = (IFeature)resultSetConduits.Next();                        

                        while (pfeatConduit != null)
                        {
                            //Get Count of releated Pri UG records with status 5 (InService) 
                            strStatusConduit = Convert.ToString(pfeatConduit.get_Value(pfeatConduit.Fields.FindField(ReadConfigurations.col_STATUS)));
                            relClassesPriUG = ((IFeatureClass)pfeatConduit.Class).get_RelationshipClasses(esriRelRole.esriRelRoleOrigin);

                            relPriUG = relClassesPriUG.Next();
                            while (relPriUG != null)
                            {
                                if (((IDataset)relPriUG.DestinationClass).Name.ToUpper() == ReadConfigurations.FC_PRIUGCONDUCTOR)
                                {
                                    lstPriUGOIDs.Clear();

                                    resultSetPriUGs = relPriUG.GetObjectsRelatedToObject(pfeatConduit);

                                    pfeatPriUG = (IFeature)resultSetPriUGs.Next();

                                    while (pfeatPriUG != null)
                                    {
                                        strStatusPriUG = Convert.ToString(pfeatPriUG.get_Value(pfeatPriUG.Fields.FindField(ReadConfigurations.col_STATUS)));
                                        if (strStatusPriUG == ReadConfigurations.val_StatusInService)
                                        {
                                            if (bConductorIsThreePhase)
                                            {
                                                strPhaseChildConductor = Convert.ToString(pfeatPriUG.get_Value(pfeatPriUG.Fields.FindField(ReadConfigurations.col_PHASEDESIGNATION)));
                                                if (strPhaseChildConductor == "7")
                                                {
                                                    if (!lstPriUGOIDs.Contains(pfeatPriUG.OID))
                                                        lstPriUGOIDs.Add(pfeatPriUG.OID);
                                                }
                                                else
                                                {                                                    
                                                    Common._log.Info("Excluding Pri UG OID : " + pfeatPriUG.OID+ "(Not a 3 phase) for calculating filledduct for Pri UG OID : "+argFeaturePriUGCond.OID+" (3 phase)");
                                                }
                                            }
                                            else
                                            {
                                                if (!lstPriUGOIDs.Contains(pfeatPriUG.OID))
                                                    lstPriUGOIDs.Add(pfeatPriUG.OID);
                                            }
                                        }

                                        pfeatPriUG = (IFeature)resultSetPriUGs.Next();
                                    }

                                    if (filledductValue < lstPriUGOIDs.Count)
                                    {
                                        filledductValue = lstPriUGOIDs.Count;
                                    }
                                    break;
                                }

                                relPriUG = relClassesPriUG.Next();
                            }

                            pfeatConduit = (IFeature)resultSetConduits.Next();
                        }                       
                    }
                    relConduit = relClassesConduit.Next();
                }
            }
            catch (Exception exp)
            {
                Common._log.Error("Exception : " + exp.Message + " |  Stack Trace | " + exp.StackTrace + " , Feature OID : " + argFeaturePriUGCond.OID);
            }
            finally
            {
                GC.Collect();
                GC.WaitForPendingFinalizers();

                if (lstPriUGOIDs != null)
                {
                    lstPriUGOIDs.Clear();
                    lstPriUGOIDs = null;
                }
            }
            return filledductValue;
        }

        //private static bool AreTwoConductorsSpatialRelaetd(IFeature argFeaturePriUGCondReference, IGeometry pRefGeom, int iOID)
        //{
        //    string comment = "";
        //    bool bTwoConductorsAreSpatiallyRelated = false;
        //    try
        //    {
        //        ITopologicalOperator diffGeometry = pRefGeom as ITopologicalOperator;
        //        IPolyline pDiffLine = diffGeometry.Difference(pInputGeom) as IPolyline;
        //        double pLength = (pRefGeom as IPolyline).Length - pDiffLine.Length;

        //        if (pLength >= cutoffLengthConductorToExclude)
        //        {
        //            comment = "Primary UG with OID :" + iOID + " Total Length : " + (pRefGeom as IPolyline).Length + " Parrellel Length : " + pLength;
        //            Common._log.Info(comment);
        //            bTwoConductorsAreSpatiallyRelated = true;
        //        }
        //    }
        //    catch (Exception exp)
        //    {
        //        bTwoConductorsAreSpatiallyRelated = false;
        //        Common._log.Error("Error in getting SDE Workspace, function GetWorkspace: " + exp.Message);
        //        Common._log.Info(exp.Message + "   " + exp.StackTrace);
        //    }
        //    return bTwoConductorsAreSpatiallyRelated;
        //}

        private static bool CheckForOverlap( int iOID1,IGeometry pInputGeom, IGeometry pRefGeom, int iOID2)
        {
            string comment = "";
            bool bTwoConductorsAreSpatiallyRelated = false;
            ITopologicalOperator diffGeometry = null;
            IPolyline pDiffLine = null;
            double lineLength = 0;
            double pLength = 0;
            try
            {
                //For testing //if (iOID1 == 6639671 && iOID2 == 6641377)
                //{ 
                //}               

                diffGeometry = pRefGeom as ITopologicalOperator;
                pDiffLine = diffGeometry.Difference(pInputGeom) as IPolyline;
                lineLength = (pRefGeom as IPolyline).Length;
                pLength = lineLength - pDiffLine.Length;

                if (pLength >= _cutoffLengthConductorToExclude)
                {
                    comment = "For Conductor :" + iOID1 + " PrimaryUG Conductor with OID :" + iOID2 + " has Total Length : " + (pRefGeom as IPolyline).Length + " and Parrellel Length : " + pLength;
                    if (_bLogNeededWhileExecuting) Common._log.Info(comment);
                    bTwoConductorsAreSpatiallyRelated = true;
                }
            }
            catch (Exception exp)
            {
                bTwoConductorsAreSpatiallyRelated = false;
                Common._log.Error("Error in getting SDE Workspace, function GetWorkspace: " + exp.Message);
                Common._log.Error(exp.Message + "   " + exp.StackTrace);
            }
            finally
            {
                if (diffGeometry != null && Marshal.IsComObject(diffGeometry))
                {
                    Marshal.FinalReleaseComObject(diffGeometry);
                    diffGeometry = null;
                }
            }
            return bTwoConductorsAreSpatiallyRelated;
        }

        private static bool CaptureAllConductorCodesToExclude(string argstrConncetion)
        {
            bool bprocess = false;
            DataTable dtConductorCodes = null;
            string strCode = null;
            try
            {   
                string strQuery = "select PGE_CONDUCTORCODE from "+ReadConfigurations.GetValue(ReadConfigurations.tableNameForConductorCodes)+" where exclude='Y'";
                Common._log.Info("Query to get excluded conductor codes : " + strQuery);
                
                dtConductorCodes = DBHelper.GetDataTable(argstrConncetion, strQuery);
             
                foreach (DataRow pRow in dtConductorCodes.Rows)
                {
                    strCode = Convert.ToString(pRow[ReadConfigurations.col_PGE_CONDUCTORCODE]);

                    if (!string.IsNullOrEmpty(strCode) && !MainClass._listConductorCodesToExclude.Contains(strCode))
                    {
                        MainClass._listConductorCodesToExclude.Add(strCode);
                    }
                }
                Common._log.Info("Total excluded conductor codes count : " + MainClass._listConductorCodesToExclude.Count);              
            }
            catch (Exception exp)
            {
                bprocess = false;
                Common._log.Error(exp.Message + " at  " + exp.StackTrace);
            }
            finally
            {
                if (dtConductorCodes != null)
                {
                    dtConductorCodes.Clear();
                    dtConductorCodes = null;
                }
            }
            return bprocess;
        }
    }
}
