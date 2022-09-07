using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ESRI.ArcGIS.Geodatabase;
using System.Data;
using System.Data.OleDb;
using System.Configuration;
using Miner.Interop.Process;
using System.IO;
using System.Data.OracleClient;
using Miner.Interop;
using ESRI.ArcGIS.Geometry;
using System.Runtime.InteropServices;
using ESRI.ArcGIS.DataSourcesGDB;
using PGE_DBPasswordManagement;

namespace PGE.BatchApplication.ConductorInTrench
{
    public class Bulk_Process
    { 
        public static DataTable _dtAllRule1Reocrds = null;
        public static bool _isLeftRecords = false;
        public static bool _isRunningProcessForChanges = false;

        /// <summary>
        /// This function will start the bulk update process
        /// </summary>
        /// <param name="argiStartCount"></param>
        /// <param name="argiEndCount"></param>
        /// <param name="argiRecordsPerRun"></param>
        /// <returns></returns>
        /// public static bool StartBulkProcess(int argiStartCount, int argiEndCount, int argiRecordsPerRun, string argStrwhereClause)
        public static bool StartBulkProcess(string argStrwhereClause)
        {
            IMMPxApplication pxApplication;
            IFeatureClass priUGFeatureClass = null;
            IFeatureClass fc_fifty_ScaleBoundary = null;
            IWorkspace workspace_edgis = null;
            DataTable copy_Success_PriUG = null;
            DataTable dtAll50ScaleReocrds = null;
            bool bOperaionSuccess = true;
            string strConnectionString_pgedata = null;
            string strConnectionString_igpciteditor = null;
            string strConnString_igpciteditor = null;
            string strconnStrToFetchedgisws = null;          
            string tableNameToStoreData = null;
            string queryToFetchdata = null;
            string queryToGetRule1Data = null;
            bool bCopyToDatabaseSuccess = false;
            string strTableNameToFetchData = string.Empty;        
            string OIDSNeeded_AlreadyCalculated = string.Empty;
            string strExclusion = string.Empty;
            string strIs50Scale = string.Empty;
            string strConduitType = string.Empty;
            try
            {
                bOperaionSuccess = MainClass.PreRequiste();

                if (!bOperaionSuccess)
                {
                    Common._log.Info("Error in fetching pre requisite values.");
                }

                bool bReadConfigValuesAndInitializesStaticVariables = MainClass.ReadConfigValues();

                if (Bulk_Process._isLeftRecords)
                {
                    strTableNameToFetchData = ConfigurationManager.AppSettings["tableforPriUGCondLeft"];
                    tableNameToStoreData = ConfigurationManager.AppSettings["tableforPriUGCondLeftFD"];
                }
                else if (Bulk_Process._isRunningProcessForChanges)
                {
                    strTableNameToFetchData = ConfigurationManager.AppSettings["tableForAllChangedPriUGCond"];
                    tableNameToStoreData = ConfigurationManager.AppSettings["tableForAllChangedPriUGCondFD"];
                }
                else
                {
                    tableNameToStoreData = ReadConfigurations.GetValue(ReadConfigurations.tableNameallPriUGCond_Rule1_FD);
                }

                if (!bReadConfigValuesAndInitializesStaticVariables)
                {
                    Common._log.Info("Error in reading config values.");
                }

                pxApplication = new PxApplicationClass();
                if (!bOperaionSuccess)
                {
                    Common._log.Info("Exiting the process.");
                    return false;
                }

                //Common._log.Info("Connection string used in process ." + ReadConfigurations.GetValue(ReadConfigurations.ConnString_EDWorkSpace));
               // m4jf edgisrearc 919 - get sde connection file using Password management tool
                strconnStrToFetchedgisws =ReadConfigurations.ConnString_EDWorkSpace;
                Common._log.Info("Connection string used in process to fetch EDGIS workspace." + strconnStrToFetchedgisws);
                workspace_edgis = MainClass.GetWorkspace(strconnStrToFetchedgisws);

                if (workspace_edgis == null)
                {
                    Common._log.Error("Work space not found for conn string : " + ReadConfigurations.ConnString_EDWorkSpace);
                    Common._log.Info("Exiting the process.");
                    return false;
                }

                Common._log.Info("Workspace checked out successfully.");
                priUGFeatureClass = ((IFeatureWorkspace)workspace_edgis).OpenFeatureClass(ReadConfigurations.FC_PRIUGCONDUCTOR);

                if (MainClass._processtype == "SCHEDULE")
                    fc_fifty_ScaleBoundary = ((IFeatureWorkspace)workspace_edgis).OpenFeatureClass(ReadConfigurations.FC_fifty_Scale_Boundary);

                strConnString_igpciteditor = ReadConfigurations.connString_igpciteditor;
                strConnectionString_igpciteditor = ReadConfigurations.ConnectionString_igpciteditor;

                strConnectionString_pgedata =ReadConfigurations.ConnectionString_pgedata;

                if (MainClass._processtype == "BULK")
                {
                    queryToFetchdata = "select * from " + ReadConfigurations.GetValue(ReadConfigurations.tableNameallPriUGCond_50Scale);
                    Common._log.Info("Query To Get all 50 scale records : " + queryToFetchdata);
                    dtAll50ScaleReocrds = GetConduitPriUGConductorData(strConnString_igpciteditor, queryToFetchdata);
                    Common._log.Info("50 scale records fetch success. Total records are:" + dtAll50ScaleReocrds.Rows.Count);

                    if (!Bulk_Process._isLeftRecords)
                    {
                        queryToFetchdata = ReadConfigurations.GetValue(ReadConfigurations.queryToGetRule1Records);
                        Common._log.Info("Query To Get all Rule1 records : " + queryToFetchdata);
                        _dtAllRule1Reocrds = GetConduitPriUGConductorData(strConnString_igpciteditor, queryToFetchdata);
                        Common._log.Info("All Rule 1 records fetch success. Total records are:" + _dtAllRule1Reocrds.Rows.Count);
                    }
                }

                bCopyToDatabaseSuccess = false;

                if (Bulk_Process._isLeftRecords || Bulk_Process._isRunningProcessForChanges)
                {
                    //queryToGetRule1Data = "select priug_objectid, globalid, circuitid,id from " + strTableNameToFetchData + " where ID>=" + iStart + " and ID<=" + iEnd; 
                    queryToGetRule1Data = "select * from " + strTableNameToFetchData;
                }
                else
                {
                    //queryToGetRule1Data = ReadConfigurations.GetValue(ReadConfigurations.queryToGetRule1Records) + " where ID>=" + iStart + " and ID<=" + iEnd;  
                    queryToGetRule1Data = ReadConfigurations.GetValue(ReadConfigurations.queryToGetRule1Records) + argStrwhereClause;
                }

                Common._log.Info("Query To Get Selected Rule1 Records : " + queryToGetRule1Data);
                //queries.Add(queryToGetRule1Data);

                copy_Success_PriUG = GetConduitPriUGConductorData(strConnString_igpciteditor, queryToGetRule1Data);
                copy_Success_PriUG.Columns.Add(ReadConfigurations.col_FILLEDDUCT, typeof(int));
                copy_Success_PriUG.Columns.Add(ReadConfigurations.col_Exclusion, typeof(string));
                copy_Success_PriUG.Columns.Add(ReadConfigurations.col_Is50Scale, typeof(string));
                copy_Success_PriUG.Columns.Add(ReadConfigurations.col_CONDUITTYPE, typeof(string));
                copy_Success_PriUG.Columns.Add(ReadConfigurations.col_OIDS_Needed, typeof(string));
                copy_Success_PriUG.Columns.Add(ReadConfigurations.col_OIDS_All, typeof(string));

                Common._log.Info("Total records to process : " + copy_Success_PriUG.Rows.Count);

                ProceccesRecords(ref copy_Success_PriUG,ref priUGFeatureClass, fc_fifty_ScaleBoundary, dtAll50ScaleReocrds, argStrwhereClause);
        
                copy_Success_PriUG.PrimaryKey = new DataColumn[] { copy_Success_PriUG.Columns[ReadConfigurations.col_OBJECTID] };
                Common._log.Info("Calculated filled duct for where clause " + argStrwhereClause + " and total records =" + copy_Success_PriUG.Rows.Count);

                Common._log.Info("Bulk copying the data for records to database table : " + tableNameToStoreData);

                if (copy_Success_PriUG.Rows.Count == 0)
                {
                    Common._log.Info("No data to copy in database for calculated filled duct for where clause " + argStrwhereClause);
                }
                else
                {
                    while (!bCopyToDatabaseSuccess)
                    {
                        if (MainClass._exception.Contains("Failure to access the DBMS server"))
                        {
                            Common._log.Info("Not Copying the data in database for calculated filled duct for where clause " + argStrwhereClause + " because of exception : Failure to access the DBMS server.");
                            MainClass._exception = "";
                            break;
                        }
                        else
                        {
                            bCopyToDatabaseSuccess = Common.BulkCopyDataFromDataTableToDatabaseTable(copy_Success_PriUG, tableNameToStoreData, strConnectionString_igpciteditor);

                            if (!bCopyToDatabaseSuccess)
                            {
                                Common._log.Info("Some issue occured in copying the data.");
                                System.Threading.Thread.Sleep(900000);
                            }
                            else
                            {
                                Common._log.Info("Copied the data in database for calculated filled duct for where clause " + argStrwhereClause);
                            }
                        }
                    }
                }

                // Storing all conductors captured as exception in datatable 
                Common._log.Info("Bulk copying the exceptions records to database table : " + MainClass._databaseTableName_Exceptions);
                Common.BulkCopyDataFromDataTableToDatabaseTable(MainClass._dtconductorExceptions, MainClass._databaseTableName_Exceptions, strConnectionString_pgedata);
                
                //MainClass.ExportResults(MainClass.dtconductorAllDetails);
                //MainClass.ExportResults(MainClass.dtconductorFILLEDDUCT);
                //MainClass.ExportResults(MainClass.dtconductorExceptions);
            }
            catch (Exception exp)
            {
                bOperaionSuccess = false;
                Common._log.Error("Exception : " + exp.Message +" |  Stack Trace | " + exp.StackTrace+" at "+exp.ToString());
            }
            finally
            {
                Common.CloseLicenseObject();
            }
            return bOperaionSuccess;
        }

        public static bool ProceccesRecords(ref DataTable argdtPriUGConductors,ref IFeatureClass argpriUGFeatureClass, IFeatureClass argfiftyScaleBoundaryFC, DataTable argdtAll50ScaleReocrds, string argStrwhereClause)
        {
            bool bProcessSuccess = false;
            DataRow pRow = null;
            int oidPriUG = 0;
            string uniqueKey = null;
            int filledduct = 0;
            int filledduct_old = 0;
            int filledduct_AlreadyCalculated = 0;           
            string strTableNameToFetchData = string.Empty;
            int oidPriUGd = 0;
            bool bFilledductAlreadyCalculated = false;
            string allOIDs = null;
            string OIDSNeeded_AlreadyCalculated = string.Empty;
            string strExclusion = string.Empty;
            string strIs50Scale = string.Empty;
            string strConduitType = string.Empty;
            DataRow[] pRows = null;
            List<int> listintersectingLineFeatureOIDs_old = null;           
            try
            {
                listintersectingLineFeatureOIDs_old = new List<int>();

                for (int iCount = 0; iCount < argdtPriUGConductors.Rows.Count; iCount++)
                {
                    try
                    {
                        strExclusion = string.Empty;
                        strIs50Scale = string.Empty;
                        strConduitType = string.Empty;

                        pRow = argdtPriUGConductors.Rows[iCount];
                        uniqueKey = Convert.ToString(pRow[ReadConfigurations.col_GLOBALID]);
                        oidPriUGd = Convert.ToInt32(pRow[ReadConfigurations.col_OBJECTID]);
                        //if (oidPriUGd == 6776126 || oidPriUGd == 6797667 || oidPriUGd == 14027146)
                        //{                                 
                        //}                               

                        bFilledductAlreadyCalculated = Common.GetFilledductValueFromAlreadyProcessedConductors(argdtPriUGConductors, oidPriUGd, ref filledduct_AlreadyCalculated, ref OIDSNeeded_AlreadyCalculated);
                        filledduct = MainClass.ProcessPriUGCond_SpatialAnalysis_ForGivenQuery(argpriUGFeatureClass, argfiftyScaleBoundaryFC, "GLOBALID ='" + uniqueKey + "'", oidPriUG, MainClass._processtype, argdtAll50ScaleReocrds, true, ref strExclusion, ref strIs50Scale, ref strConduitType);

                        pRow[ReadConfigurations.col_CONDUITTYPE] = strConduitType;

                        //Check here if type is 1 or others , if type is not for spatial, then don't do any further processing
                        if (strConduitType == "0" || strConduitType == "2" || strConduitType == "3")
                        {
                            pRow[ReadConfigurations.col_Exclusion] = strExclusion;
                            pRow[ReadConfigurations.col_Is50Scale] = strIs50Scale;

                            if (filledduct >= 3)
                            {
                                filledduct_old = filledduct;
                                listintersectingLineFeatureOIDs_old.Clear();

                                listintersectingLineFeatureOIDs_old = MainClass._listintersectingLineFeatureOIDs.ToList<int>();

                                MainClass._searchDistance = MainClass._searchDistance * 2;
                                filledduct = MainClass.ProcessPriUGCond_SpatialAnalysis_ForGivenQuery(argpriUGFeatureClass, argfiftyScaleBoundaryFC, "GLOBALID ='" + uniqueKey + "'", oidPriUG, MainClass._processtype, argdtAll50ScaleReocrds, false, ref strExclusion, ref strIs50Scale, ref strConduitType);

                                if (filledduct == 5)
                                {
                                    //Check for next buffer ditance to see the value is 7 or more 
                                    //MainClass._searchDistance = MainClass._searchDistance + Convert.ToInt32(ReadConfigurations.GetValue(ReadConfigurations.searchDistance));
                                    //filledduct = MainClass.ProcessPriUGCond_SpatialAnalysis_ForGivenQuery(argpriUGFeatureClass, argfiftyScaleBoundaryFC, "GLOBALID ='" + uniqueKey + "'", oidPriUG, MainClass._processtype, argdtAll50ScaleReocrds, false, ref strExclusion, ref strIs50Scale, ref strConduitType);
                                }
                                
                                MainClass._searchDistance = Convert.ToInt32(ReadConfigurations.GetValue(ReadConfigurations.searchDistance));

                                //if FD=5 then for FD = 7  
                                if (filledduct_old == 3 && filledduct == filledduct_old)
                                {
                                    foreach (int OID in listintersectingLineFeatureOIDs_old)
                                    {
                                        if (!MainClass._listintersectingLineFeatureOIDs.Contains(OID))
                                        {
                                            MainClass._listintersectingLineFeatureOIDs.Add(OID);
                                        }                                    
                                    }                                
                                }
                                else if (filledduct_old == 4 && filledduct == filledduct_old)
                                {
                                    foreach (int OID in listintersectingLineFeatureOIDs_old)
                                    {
                                        if (!MainClass._listintersectingLineFeatureOIDs.Contains(OID))
                                        {
                                            MainClass._listintersectingLineFeatureOIDs.Add(OID);
                                        }
                                    }
                                }                                
                            }

                            if (!MainClass._listintersectingLineFeatureOIDs.Contains(oidPriUGd))
                            {
                                MainClass._listintersectingLineFeatureOIDs.Add(oidPriUGd);
                            }

                            if (!MainClass._listintersectingAllLineFeatureOIDs.Contains(oidPriUGd))
                            {
                                MainClass._listintersectingAllLineFeatureOIDs.Add(oidPriUGd);
                            }

                            allOIDs = Common.GetAllConductorInAcsenedingOrder(MainClass._listintersectingLineFeatureOIDs);

                            if (bFilledductAlreadyCalculated && filledduct_AlreadyCalculated < filledduct)
                            {
                                // Filledduct is already calculated and filledduct_AlreadyCalculated < filledduct
                                Common.SaveFilledductAndOIDsNeededForGivenConductors(argdtPriUGConductors, MainClass._listintersectingLineFeatureOIDs, filledduct, allOIDs);
                            }
                            else if (bFilledductAlreadyCalculated && filledduct_AlreadyCalculated >= filledduct)
                            {
                                // Filledduct is already calculated and filledduct_AlreadyCalculated >= filledduct
                                filledduct = filledduct_AlreadyCalculated;
                                allOIDs = OIDSNeeded_AlreadyCalculated;
                                Common.SaveFilledductAndOIDsNeededForGivenConductors(argdtPriUGConductors, MainClass._listintersectingLineFeatureOIDs, filledduct, allOIDs);
                            }
                            else if (!bFilledductAlreadyCalculated)
                            {
                                Common.SaveFilledductAndOIDsNeededForGivenConductors(argdtPriUGConductors, MainClass._listintersectingLineFeatureOIDs, filledduct, allOIDs);
                            }

                            allOIDs = Common.GetAllConductorInAcsenedingOrder(MainClass._listintersectingAllLineFeatureOIDs);
                            pRow[ReadConfigurations.col_OIDS_All] = allOIDs;
                        }
                        else
                        {
                            pRow[ReadConfigurations.col_FILLEDDUCT] = filledduct;
                            pRow[ReadConfigurations.col_Exclusion] = strExclusion;
                        }

                        pRow.AcceptChanges();

                        if (iCount % 500 == 0)
                            Common._log.Info("Features processed till yet : " + iCount);

                        if (MainClass._exception.Contains("Failure to access the DBMS server"))
                        {
                            Common._log.Error("Breaking the process in between for where clause " + argStrwhereClause + " because of exception : Failure to access the DBMS server for record : " + oidPriUGd);
                           //initialize feature class here again and let the process run 
                            System.Threading.Thread.Sleep(900000);
                            argpriUGFeatureClass = GetFeatureClass(ReadConfigurations.ConnString_EDWorkSpace,ReadConfigurations.FC_PRIUGCONDUCTOR);
                            if (argpriUGFeatureClass == null)
                            {
                                System.Threading.Thread.Sleep(900000);
                                argpriUGFeatureClass = GetFeatureClass(ReadConfigurations.ConnString_EDWorkSpace, ReadConfigurations.FC_PRIUGCONDUCTOR);

                                if (argpriUGFeatureClass == null)
                                {
                                    break;
                                }
                            }                            
                        }
                    }
                    catch (Exception exp)
                    {
                        Common._log.Error("Error occured for Pri UG OID :" + Convert.ToString(pRow[ReadConfigurations.col_OBJECTID]));
                        Common._log.Error("Exception : " + exp.Message +" |  Stack Trace | " + exp.StackTrace+" at "+exp.ToString());
                    }
                    finally
                    {
                        GC.Collect();
                        GC.WaitForPendingFinalizers();
                    }
                }

                //Setting Filledduct for all excluded conductors to 1
                pRows = argdtPriUGConductors.Select("Exclusion='"+ReadConfigurations.strExclusionBasedOnConductorCodes+"'");
                for (int iCount = 0; iCount < pRows.Length; iCount++)
                {
                    pRows[iCount][ReadConfigurations.col_FILLEDDUCT] = 1;                   
                }

                //Setting Filledduct for all not in service conductors to NULL
                pRows = argdtPriUGConductors.Select("Exclusion='"+ReadConfigurations.strExclusionBasedOnStatus+"'");
                // Changes for setting filledduct for conductors which are not In Service 
                int iFilledductNotInService = Convert.ToInt32(ConfigurationManager.AppSettings["FilledductValueForNotInService"]);
                for (int iCount = 0; iCount < pRows.Length; iCount++)
                {
                    pRows[iCount][ReadConfigurations.col_FILLEDDUCT] = iFilledductNotInService;
                }

                //Setting Filledduct for all short conductors to 1
                pRows = argdtPriUGConductors.Select("Exclusion='" + ReadConfigurations.strExclusionBasedOnLength + "'");
                for (int iCount = 0; iCount < pRows.Length; iCount++)
                {
                    pRows[iCount][ReadConfigurations.col_FILLEDDUCT] = 1;
                }

                argdtPriUGConductors.AcceptChanges();
                
                SetFilledductForExcludedBasedOnConductorCodes(ref argdtPriUGConductors, argpriUGFeatureClass);
                SetFilledductForSinglePhaseTwoPhaseConductors(ref argdtPriUGConductors, argpriUGFeatureClass);

                argdtPriUGConductors.AcceptChanges();
                bProcessSuccess = true;
            }
            catch (Exception exp)
            {                
                 Common._log.Error("Exception : " + exp.Message +" |  Stack Trace | " + exp.StackTrace+" at "+exp.ToString());
                 bProcessSuccess = false;
                 ErrorCodeException ece = new ErrorCodeException(exp);
                 Environment.ExitCode = ece.CodeNumber;
            }
            return bProcessSuccess;
        }

        /// <summary>
        /// This function will fetch data for given query
        /// </summary>
        /// <param name="strConnectionString"></param>
        /// <param name="strSqlQuery"></param>
        /// <returns></returns>
        public static DataTable GetConduitPriUGConductorData(string strConnectionString, string strSqlQuery)
        {
            DataTable dtConduitPriUGConductorData = null;
            try
            {
                dtConduitPriUGConductorData = new DataTable();
                dtConduitPriUGConductorData.Columns.Add(ReadConfigurations.col_OBJECTID, typeof(ulong));
                dtConduitPriUGConductorData.Columns.Add("ID", typeof(ulong));
               
                //Common._log.Info("Fetching connection string.");              
                //Common._log.Info("Connection string to fetch data : " + strConnectionString);
                //Common._log.Info("SQL query to fetch data : " + strConnectionString);

                using (OleDbConnection con = new OleDbConnection(strConnectionString))
                {
                    con.Open();
                    Common._log.Info("Connected To DataBase successfully.");
                    OleDbDataAdapter adp = new OleDbDataAdapter(strSqlQuery, con);
                    adp.Fill(dtConduitPriUGConductorData);                  
                }
            }
            catch (Exception exp)
            {
                Common._log.Error("Error while fetching data to update from database table." + exp.Message);
                Common._log.Error("Exception : " + exp.Message +" |  Stack Trace | " + exp.StackTrace+" at "+exp.ToString());
            }
            return dtConduitPriUGConductorData;
        }        

        /// <summary>
        /// This function will start processing for capturing 50 scale data
        /// </summary>
        /// <returns></returns>
        public static bool StartProcessForCapturing50ScaleData()
        {
            IMMPxApplication _pxApplication_1;
            IFeatureClass priUGFeatureClass = null;
            IFeatureClass fc_fifty_ScaleBoundary = null;
            IWorkspace workspace_edgis = null;           
            bool bOperaionSuccess = true;
            string strConnectionString_igpciteditor = null;           
            bool bprocessSuccess = false;
            string tableNameToStoreData = null;
            string strconnStrToFetchedgisws = null;
            bool bCopyToDatabaseSuccess = false;
            try
            {
                bOperaionSuccess = MainClass.PreRequiste();

                if (!bOperaionSuccess)
                {
                    Common._log.Info("Error in fetching pre requisite values.");
                }

                bool bReadConfigValuesAndInitializesStaticVariables = MainClass.ReadConfigValues();

                if (!bReadConfigValuesAndInitializesStaticVariables)
                {
                    Common._log.Info("Error in reading config values.");
                }

                _pxApplication_1 = new PxApplicationClass();
                if (!bOperaionSuccess)
                {
                    Common._log.Info("Exiting the process.");
                    return false;
                }
                     
                strconnStrToFetchedgisws = ReadConfigurations.ConnString_EDWorkSpace;
                Common._log.Info("Connection string used in process ." + strconnStrToFetchedgisws);
                workspace_edgis = MainClass.GetWorkspace(strconnStrToFetchedgisws);

                if (workspace_edgis == null)
                {
                    Common._log.Error("Work space not found for conn string : " + ReadConfigurations.ConnString_EDWorkSpace);
                    Common._log.Info("Exiting the process.");
                    return false;
                }

                Common._log.Info("Workspace checked out successfully.");


                priUGFeatureClass = ((IFeatureWorkspace)workspace_edgis).OpenFeatureClass(ReadConfigurations.FC_PRIUGCONDUCTOR);
                fc_fifty_ScaleBoundary = ((IFeatureWorkspace)workspace_edgis).OpenFeatureClass(ReadConfigurations.FC_fifty_Scale_Boundary);

                DataTable dt50ScaleData = new Bulk_Process().Capture50ScaleData(priUGFeatureClass, fc_fifty_ScaleBoundary);

                strConnectionString_igpciteditor = ReadConfigurations.ConnectionString_igpciteditor;
                tableNameToStoreData = ReadConfigurations.GetValue(ReadConfigurations.tableNameallPriUGCond_50Scale);

                Common._log.Info("Bulk copying the data for 50 scale records to database table : " + tableNameToStoreData + " and count is " + dt50ScaleData.Rows.Count);

                while (!bCopyToDatabaseSuccess)
                {
                    bCopyToDatabaseSuccess = Common.BulkCopyDataFromDataTableToDatabaseTable(dt50ScaleData, tableNameToStoreData, strConnectionString_igpciteditor);
                }

                bprocessSuccess = true;
                Common._log.Info("Copied the data in database for 50 scale records.");             
            }
            catch (Exception exp)
            {
                Common._log.Error("Exception : " + exp.Message +" |  Stack Trace | " + exp.StackTrace+" at "+exp.ToString());
                bprocessSuccess = false;
            }
            return bprocessSuccess;
        }

        /// <summary>
        /// This function will capture all 50 scale data
        /// </summary>
        /// <param name="argpriUGFeatureClass"></param>
        /// <param name="argfc_fifty_ScaleBoundary"></param>
        /// <returns></returns>
        private DataTable Capture50ScaleData(IFeatureClass argpriUGFeatureClass, IFeatureClass argfc_fifty_ScaleBoundary)
        {           
            ISpatialFilter spatialFilter = null;
            IFeatureCursor pfeatCursor = null;
            IFeature pFeatureOutPut = null;            
            IFeatureCursor pBoundaryCursor = null;
            IFeature pBoundary = null;
            DataTable dt50Scaledata = new DataTable();
            string globalid = null;
            try
            {
                dt50Scaledata.Columns.Add(ReadConfigurations.col_OBJECTID, typeof(ulong));
                dt50Scaledata.Columns.Add(ReadConfigurations.col_GLOBALID, typeof(String));               

                pBoundaryCursor = argfc_fifty_ScaleBoundary.Search(null, false);
                pBoundary = pBoundaryCursor.NextFeature();
                while (pBoundary != null)
                {
                    spatialFilter = new SpatialFilterClass();
                    spatialFilter.Geometry = pBoundary.Shape;
                    spatialFilter.SpatialRel = esriSpatialRelEnum.esriSpatialRelIntersects;
                    spatialFilter.SubFields = "GLOBALID,OBJECTID,SHAPE,STATUS";

                    pfeatCursor = argpriUGFeatureClass.Search(spatialFilter, false);

                    pFeatureOutPut = pfeatCursor.NextFeature();
                    while (pFeatureOutPut != null)
                    {
                        globalid = Convert.ToString(pFeatureOutPut.get_Value(pFeatureOutPut.Fields.FindField(ReadConfigurations.col_GLOBALID)));
                        dt50Scaledata.Rows.Add(pFeatureOutPut.OID,globalid);
                        pFeatureOutPut = pfeatCursor.NextFeature();
                    }
                 
                    pBoundary = pBoundaryCursor.NextFeature();
                }
            }
            catch (Exception exp)
            {
                Common._log.Error("Exception : " + exp.Message +" |  Stack Trace | " + exp.StackTrace+" at "+exp.ToString());
            }
            finally
            {
                if (pfeatCursor != null)
                {
                    Marshal.FinalReleaseComObject(pfeatCursor);
                }

                if (spatialFilter != null)
                {
                    Marshal.FinalReleaseComObject(spatialFilter);
                }

                if (pBoundaryCursor != null)
                {
                    Marshal.FinalReleaseComObject(pBoundaryCursor);
                }

                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
            return dt50Scaledata;
        }

        /// <summary>
        /// This function will start processing for capturing all possible changed primary UG conductors
        /// </summary>
        /// <param name="argStrTableForChangedPriUGCond"></param>
        /// <param name="argStrTableForAllChangedPriUGCond"></param>
        /// <returns></returns>
        public static bool StartProcessForCapturingAllpossibleCOnductorsForChangedConductors(string argStrTableForChangedPriUGCond, string argStrTableForAllChangedPriUGCond)
        {
            IMMPxApplication _pxApplication_1;
            IFeatureClass priUGFeatureClass = null;         
            IWorkspace workspace_edgis = null;           
            bool bOperaionSuccess = true;              
            string strConnString_igpciteditor = null;
            string strConnectionString_igpciteditor = null;         
            string queryToFetchdata = null;
            bool bprocessSuccess = false;
            string tableNameToStoreData = null;
            string strconnStrToFetchedgisws = null;
            bool bCopyToDatabaseSuccess = false;
            DataTable dtAllChangedConductors = null;
            try
            {
                bOperaionSuccess = MainClass.PreRequiste();

                if (!bOperaionSuccess)
                {
                    Common._log.Info("Error in fetching pre requisite values.");
                }

                bool bReadConfigValuesAndInitializesStaticVariables = MainClass.ReadConfigValues();

                if (!bReadConfigValuesAndInitializesStaticVariables)
                {
                    Common._log.Info("Error in reading config values.");
                }

                _pxApplication_1 = new PxApplicationClass();
                if (!bOperaionSuccess)
                {
                    Common._log.Info("Exiting the process.");
                    return false;
                }

                strconnStrToFetchedgisws = ReadConfigurations.ConnString_EDWorkSpace;
                Common._log.Info("Connection string used in process :" + strconnStrToFetchedgisws);
                workspace_edgis = MainClass.GetWorkspace(strconnStrToFetchedgisws);

                if (workspace_edgis == null)
                {
                    Common._log.Error("Work space not found for conn string : " + ReadConfigurations.ConnString_EDWorkSpace);
                    Common._log.Info("Exiting the process.");
                    return false;
                }

                Common._log.Info("Workspace checked out successfully.");

                priUGFeatureClass = ((IFeatureWorkspace)workspace_edgis).OpenFeatureClass(ReadConfigurations.FC_PRIUGCONDUCTOR);

                strConnString_igpciteditor = ReadConfigurations.connString_igpciteditor;

                queryToFetchdata = "select * from " + argStrTableForChangedPriUGCond;
                Common._log.Info("Query To Get changed conductors : " + queryToFetchdata);
                DataTable dtChangedConductors = GetConduitPriUGConductorData(strConnString_igpciteditor, queryToFetchdata);
                Common._log.Info("Changed conductors fetch success. Total records are:" + dtChangedConductors.Rows.Count);

                if (dtChangedConductors.Rows.Count > 0)
                {
                    dtAllChangedConductors = new Bulk_Process().CaptureAllChangedConductors(priUGFeatureClass, dtChangedConductors);

                    tableNameToStoreData = argStrTableForAllChangedPriUGCond;
                    strConnectionString_igpciteditor = ReadConfigurations.ConnectionString_igpciteditor;

                    Common._log.Info("Bulk copying the data for all possible conductors for changed conductors to database table : " + tableNameToStoreData + " and count is " + dtAllChangedConductors.Rows.Count);

                    while (!bCopyToDatabaseSuccess)
                    {
                        bCopyToDatabaseSuccess = Common.BulkCopyDataFromDataTableToDatabaseTable(dtAllChangedConductors, tableNameToStoreData, strConnectionString_igpciteditor);
                    }

                    bprocessSuccess = true;
                }

                Common._log.Info("Copied the data in database for all possible conductors for changed conductors.");
            }
            catch (Exception exp)
            {
                Common._log.Error("Exception : " + exp.Message +" |  Stack Trace | " + exp.StackTrace+" at "+exp.ToString());
                bprocessSuccess = false;
            }
            return bprocessSuccess;
        }

        /// <summary>
        /// This function will fetch all possible changed primary UG conductors
        /// </summary>
        /// <param name="argpriUGFeatureClass"></param>
        /// <param name="argchangedConductors"></param>
        /// <returns></returns>
        private DataTable CaptureAllChangedConductors(IFeatureClass argpriUGFeatureClass, DataTable argchangedConductors)
        {
            IQueryFilter pQf = null;
            IFeatureCursor pCursor = null;
            IFeature pFeature = null;
            IFeatureCursor pfeatCursor = null;
            ISpatialFilter spatialFilter = null;
            ITopologicalOperator topoOperator = null;
            IFeature pFeatureOutPut = null;
            DataTable dtAllChangedConductors = null;
            string globalid = null;
            string circuitID = null;
            DataRow[] pRows = null;
            try
            {
                dtAllChangedConductors = argchangedConductors.Clone();   
                foreach (DataRow pRow in argchangedConductors.Rows)
                {
                    try
                    {
                        globalid = Convert.ToString(pRow.Field<string>(ReadConfigurations.col_GLOBALID));

                        pQf = new QueryFilterClass();
                        pQf.WhereClause = "GLOBALID='"+globalid+"'";
                        pCursor = argpriUGFeatureClass.Search(pQf, false);

                        pFeature = pCursor.NextFeature();                      
                        if (pFeature != null)
                        {
                            spatialFilter = new SpatialFilterClass();
                            topoOperator = (ITopologicalOperator)pFeature.Shape;
                            spatialFilter.Geometry = topoOperator.Buffer(MainClass._searchDistance);
                            spatialFilter.GeometryField = argpriUGFeatureClass.ShapeFieldName;
                            spatialFilter.SpatialRel = esriSpatialRelEnum.esriSpatialRelIntersects;
                            spatialFilter.SubFields = ReadConfigurations.subFields;

                            pfeatCursor = argpriUGFeatureClass.Search(spatialFilter, false);

                            pFeatureOutPut = pfeatCursor.NextFeature();
                            while (pFeatureOutPut != null)
                            {
                                globalid = Convert.ToString(pFeatureOutPut.get_Value(pFeatureOutPut.Fields.FindField(ReadConfigurations.col_GLOBALID)));
                                circuitID = Convert.ToString(pFeatureOutPut.get_Value(pFeatureOutPut.Fields.FindField(ReadConfigurations.col_CIRCUITID)));

                                pRows = dtAllChangedConductors.Select("GLOBALID='"+globalid+"'");

                                if (pRows.Length == 0)
                                {
                                    dtAllChangedConductors.Rows.Add(pFeatureOutPut.OID, (dtAllChangedConductors.Rows.Count + 1), globalid, circuitID);
                                }
                                else
                                {
                                    Common._log.Info("Cable with OID : " + pFeatureOutPut.OID+" is already added in collection.");
                                }
  
                                pFeatureOutPut = pfeatCursor.NextFeature();
                            }
                        }
                    }
                    catch (Exception exp)
                    {
                        Common._log.Error("Exception : " + exp.Message +" |  Stack Trace | " + exp.StackTrace+" at "+exp.ToString());
                    }
                    finally
                    {
                        if (pCursor != null)
                        {
                            Marshal.FinalReleaseComObject(pCursor);
                        }

                        if (pQf != null)
                        {
                            Marshal.FinalReleaseComObject(pQf);
                        }

                        if (pfeatCursor != null)
                        {
                            Marshal.FinalReleaseComObject(pfeatCursor);
                        }

                        if (topoOperator != null)
                        {
                            Marshal.FinalReleaseComObject(topoOperator);
                        }

                        if (spatialFilter != null)
                        {
                            Marshal.FinalReleaseComObject(spatialFilter);
                        }

                        GC.Collect();
                        GC.WaitForPendingFinalizers();                    
                    }
                }              
            }
            catch (Exception exp)
            {
                Common._log.Error("Exception : " + exp.Message +" |  Stack Trace | " + exp.StackTrace+" at "+exp.ToString());
            }
            finally
            {
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
            return dtAllChangedConductors;
        }

        private static void SetFilledductForExcludedBasedOnConductorCodes(ref DataTable argdtAllConductors, IFeatureClass argpPriUGFeatureClass)
        {
            DataRow[] pRows = null;
            DataRow pRow = null;
            DataRow pRow_Required = null;
            int PriUGOID = 0;           
            string oids_all = null;
            int iOID_Split = 0;
            int iFilledduct_Final = 0;
            int iFilledduct = 0;
            string whereClause = null;
            bool bNearbyExcludedFound = false;
            bool bNearbyNotExcludedFound = false;
            try
            {
                //Setting Filledduct for all excluded conductors to 1
                pRows = argdtAllConductors.Select(ReadConfigurations.col_Exclusion+"='"+ReadConfigurations.strExclusionBasedOnConductorCodes+"'");
                for (int iCount = 0; iCount < pRows.Length; iCount++)
                {
                    pRow = pRows[iCount];
                    iFilledduct = 0;
                    iFilledduct_Final = 0;
                    bNearbyExcludedFound = false;
                    bNearbyNotExcludedFound = false;

                    // Take OID for this conductor , find conductors in buffer distance , take max(Filledduct) for conduit Status =(0/2/3) , +1 will give correct value
                    if (int.TryParse(Convert.ToString(pRow[ReadConfigurations.col_OBJECTID]), out PriUGOID))
                    {
                        if (!pRow.IsNull(ReadConfigurations.col_OIDS_All))
                        {
                            oids_all = Convert.ToString(pRow[ReadConfigurations.col_OIDS_All]);

                            foreach (string oid in oids_all.Split(','))
                            {
                                if (int.TryParse(oid, out iOID_Split))
                                {
                                    if (iOID_Split != PriUGOID)
                                    {
                                        whereClause = ReadConfigurations.col_OBJECTID + "='" + iOID_Split + "' AND " + ReadConfigurations.col_CONDUITTYPE + " IN ('0','2','3') AND ( " + ReadConfigurations.col_Exclusion + " IS NULL OR TRIM(" + ReadConfigurations.col_Exclusion + ")='')";

                                        if (argdtAllConductors.Select(whereClause).Length > 0)
                                        {
                                            pRow_Required = argdtAllConductors.Select(whereClause)[0];

                                            iFilledduct = Convert.ToInt32(pRow_Required[ReadConfigurations.col_FILLEDDUCT]);

                                            if (iFilledduct_Final < iFilledduct)
                                            {
                                                iFilledduct_Final = iFilledduct;
                                                bNearbyNotExcludedFound = true; 
                                            }
                                        }
                                        else
                                        {
                                            whereClause = ReadConfigurations.col_OBJECTID + "='" + iOID_Split + "' AND " + ReadConfigurations.col_CONDUITTYPE + " IN ('0','2','3') AND " + ReadConfigurations.col_Exclusion + "='"+ReadConfigurations.strExclusionBasedOnConductorCodes+"'";
                                            
                                            if (argdtAllConductors.Select(whereClause).Length > 0)
                                            {
                                                bNearbyExcludedFound = true;
                                            }
                                        }
                                    }
                                }
                            }

                            if (iFilledduct_Final == 0)
                            {
                                if (bNearbyExcludedFound)
                                {
                                    iFilledduct_Final = 2;
                                }
                                else
                                {
                                    iFilledduct_Final = 1;
                                }
                            }
                            else
                            {
                                if (bNearbyNotExcludedFound)
                                {
                                    iFilledduct_Final++;
                                }
                            }

                            pRow[ReadConfigurations.col_FILLEDDUCT] = iFilledduct_Final;
                        }
                        else
                        {
                            Common._log.Info("OID_All is null for Pri UG OID : " + PriUGOID);
                        }
                    }
                }
            }
            catch (Exception exp)
            {
                Common._log.Error("Exception : " + exp.Message +" |  Stack Trace | " + exp.StackTrace+" at "+exp.ToString());
            }
        }

        private static void SetFilledductForSinglePhaseTwoPhaseConductors(ref DataTable argdtAllConductors, IFeatureClass argpPriUGFeatureClass)
        {
            DataRow[] pRows = null;
            DataRow pRow = null;
            DataRow pRow_Required = null;
            int PriUGOID = 0;
            string oids_all = null;
            int iOID_Split = 0;
            int iFilledduct_Final = 0;
            int iFilledduct = 0;
            string whereClause = null;           
            string query = null;
            try
            {
                //Setting Filledduct for all single phase and two phase conductors
                query = ReadConfigurations.col_SUBTYPECD + " IN('1','2') AND (" + ReadConfigurations.col_Exclusion + "='' OR "+ ReadConfigurations.col_Exclusion+" IS NULL"+")";
                pRows = argdtAllConductors.Select(query);

                Common._log.Info("Row Count for query : " + query + " is " + pRows.Length);

                for (int iCount = 0; iCount < pRows.Length; iCount++)
                {
                    pRow = pRows[iCount];
                    iFilledduct = 0;
                    iFilledduct_Final = 0;                  

                    // Take OID for this conductor , find conductors in buffer distance , take max(Filledduct) for conduit Status =(0/2/3) , +1 will give correct value
                    if (int.TryParse(Convert.ToString(pRow[ReadConfigurations.col_OBJECTID]), out PriUGOID))
                    {
                        if (!pRow.IsNull(ReadConfigurations.col_OIDS_All))
                        {
                            oids_all = Convert.ToString(pRow[ReadConfigurations.col_OIDS_All]);

                            foreach (string oid in oids_all.Split(','))
                            {
                                if (int.TryParse(oid, out iOID_Split))
                                {
                                    if (iOID_Split != PriUGOID)
                                    {
                                        whereClause = ReadConfigurations.col_OBJECTID + "='" + iOID_Split + "' AND " + ReadConfigurations.col_CONDUITTYPE + " IN ('0','2','3') AND ( " + ReadConfigurations.col_Exclusion + " IS NULL OR TRIM(" + ReadConfigurations.col_Exclusion + ")='')";

                                        if (argdtAllConductors.Select(whereClause).Length > 0)
                                        {
                                            pRow_Required = argdtAllConductors.Select(whereClause)[0];

                                            iFilledduct = Convert.ToInt32(pRow_Required[ReadConfigurations.col_FILLEDDUCT]);

                                            if (iFilledduct_Final < iFilledduct)
                                            {
                                                iFilledduct_Final = iFilledduct;                                               
                                            }
                                        }                                      
                                    }
                                }
                            }

                            if (iFilledduct_Final != 0)
                            {
                                iFilledduct_Final++;
                                pRow[ReadConfigurations.col_FILLEDDUCT] = iFilledduct_Final;
                                Common._log.Info("Updating FD for single/Two Phase conductor : OID : " + PriUGOID + " is " + iFilledduct_Final);
                            }
                        }
                        else
                        {
                            Common._log.Info("OID_All is null for Pri UG OID : " + PriUGOID);
                        }
                    }
                }
            }
            catch (Exception exp)
            {
                Common._log.Error("Exception : " + exp.Message +" |  Stack Trace | " + exp.StackTrace+" at "+exp.ToString());
            }
        }


        /// <summary>
        /// This function will start processing for capturing 50 scale data
        /// </summary>
        /// <returns></returns>
        public static bool StartProcessForCapturingAllDataForCounties()
        {
            IMMPxApplication _pxApplication_1;
            IFeatureClass priUGFeatureClass = null;
            IFeatureClass fc_County = null;
            IWorkspace workspace_edgis = null;
            bool bOperaionSuccess = true;
            string strConnectionString_user = null;
            bool bprocessSuccess = false;
            string tableNameToStoreData = null;
            string strconnStrToFetchedgisws = null;
            bool bCopyToDatabaseSuccess = false;
            DataTable dtAllCountyData = null;
            try
            {
                bOperaionSuccess = MainClass.PreRequiste();

                if (!bOperaionSuccess)
                {
                    Common._log.Info("Error in fetching pre requisite values.");
                }

                bool bReadConfigValuesAndInitializesStaticVariables = MainClass.ReadConfigValues();

                if (!bReadConfigValuesAndInitializesStaticVariables)
                {
                    Common._log.Info("Error in reading config values.");
                }

                _pxApplication_1 = new PxApplicationClass();
                if (!bOperaionSuccess)
                {
                    Common._log.Info("Exiting the process.");
                    return false;
                }

                strconnStrToFetchedgisws =ReadConfigurations.ConnString_EDWorkSpace;
                Common._log.Info("Connection string used in process ." + strconnStrToFetchedgisws);
                workspace_edgis = MainClass.GetWorkspace(strconnStrToFetchedgisws);

                if (workspace_edgis == null)
                {
                    Common._log.Error("Work space not found for conn string : " + ReadConfigurations.ConnString_EDWorkSpace);
                    Common._log.Info("Exiting the process.");
                    return false;
                }

                Common._log.Info("Workspace checked out successfully.");


                priUGFeatureClass = ((IFeatureWorkspace)workspace_edgis).OpenFeatureClass(ReadConfigurations.FC_PRIUGCONDUCTOR);

                string strLBGISConnString = null;
                IWorkspace workspace_lbgis = null;


                strLBGISConnString = ConfigurationManager.AppSettings["ConnString_LBWorkSpace"];
                workspace_lbgis = MainClass.GetWorkspace(strLBGISConnString);

                fc_County = ((IFeatureWorkspace)workspace_lbgis).OpenFeatureClass("LBGIS.COUNTYUNCLIPPED");

                dtAllCountyData = new Bulk_Process().CaptureAllCountyData(priUGFeatureClass, fc_County);

                strConnectionString_user = ReadConfigurations.ConnectionString_igpciteditor;
                tableNameToStoreData = "cit_allPriUGCond_County";

                Common._log.Info("Bulk copying the data for 50 scale records to database table : " + tableNameToStoreData + " and count is " + dtAllCountyData.Rows.Count);

                while (!bCopyToDatabaseSuccess)
                {
                    bCopyToDatabaseSuccess = Common.BulkCopyDataFromDataTableToDatabaseTable(dtAllCountyData, tableNameToStoreData, strConnectionString_user);
                }

                bprocessSuccess = true;
                Common._log.Info("Copied the data in database for 50 scale records.");
            }
            catch (Exception exp)
            {
                Common._log.Error("Exception : " + exp.Message +" |  Stack Trace | " + exp.StackTrace+" at "+exp.ToString());
                bprocessSuccess = false;
            }
            return bprocessSuccess;
        }

        /// <summary>
        /// This function will capture all 50 scale data
        /// </summary>
        /// <param name="argpriUGFeatureClass"></param>
        /// <param name="argfc_County"></param>
        /// <returns></returns>
        private DataTable CaptureAllCountyData(IFeatureClass argpriUGFeatureClass, IFeatureClass argfc_County)
        {
            ISpatialFilter spatialFilter = null;
            IFeatureCursor pfeatCursor = null;
            IFeature pFeatureOutPut = null;
            IFeatureCursor pBoundaryCursor = null;
            IFeature pBoundary = null;
            DataTable dtAllCountyData = null;
            string globalid = null;
            string CountyID = null;
            string CountyName = null;
            string CountyFromConductor = null;
            int countyCount = 0;
            try
            {
                dtAllCountyData = new DataTable();
                dtAllCountyData.Columns.Add("County_ID", typeof(string));
                dtAllCountyData.Columns.Add("County_Name", typeof(string));
                dtAllCountyData.Columns.Add(ReadConfigurations.col_OBJECTID, typeof(ulong));
                dtAllCountyData.Columns.Add(ReadConfigurations.col_GLOBALID, typeof(String));
                dtAllCountyData.Columns.Add("CountyFromConductor", typeof(String));

                //IQueryFilter pQueryFilter =null;
                //IQueryFilterDefinition pQueryFilterDefinition =null;
                //pQueryFilterDefinition = pQueryFilter
                //pQueryFilterDefinition.PostFixClause = "ORDER BY FULLNAME"

                pBoundaryCursor = argfc_County.Search(null, false);
                pBoundary = pBoundaryCursor.NextFeature();
                while (pBoundary != null)
                {
                    countyCount++;

                    CountyID = Convert.ToString(pBoundary.OID);
                    CountyName = Convert.ToString(pBoundary.get_Value(pBoundary.Fields.FindField("CNTY_NAME")));

                    spatialFilter = new SpatialFilterClass();
                    spatialFilter.Geometry = pBoundary.Shape;
                    spatialFilter.SpatialRel = esriSpatialRelEnum.esriSpatialRelIntersects;
                    spatialFilter.SubFields = "GLOBALID,OBJECTID,SHAPE,STATUS,COUNTY";

                    pfeatCursor = argpriUGFeatureClass.Search(spatialFilter, false);

                    pFeatureOutPut = pfeatCursor.NextFeature();
                    while (pFeatureOutPut != null)
                    {
                        globalid = Convert.ToString(pFeatureOutPut.get_Value(pFeatureOutPut.Fields.FindField(ReadConfigurations.col_GLOBALID)));
                        CountyFromConductor = Convert.ToString(pFeatureOutPut.get_Value(pFeatureOutPut.Fields.FindField("COUNTY")));

                        if (dtAllCountyData.Select(ReadConfigurations.col_GLOBALID + "='" + globalid + "'").Length == 0)
                        {
                            dtAllCountyData.Rows.Add(CountyID, CountyName, pFeatureOutPut.OID, globalid, CountyFromConductor);
                        }
                        pFeatureOutPut = pfeatCursor.NextFeature();
                    }

                    pBoundary = pBoundaryCursor.NextFeature();
                }

                Common._log.Info("Total records : " +dtAllCountyData.Rows.Count);
            }
            catch (Exception exp)
            {
                Common._log.Error("Exception : " + exp.Message +" |  Stack Trace | " + exp.StackTrace+" at "+exp.ToString());
            }
            finally
            {
                if (pfeatCursor != null)
                {
                    Marshal.FinalReleaseComObject(pfeatCursor);
                }

                if (spatialFilter != null)
                {
                    Marshal.FinalReleaseComObject(spatialFilter);
                }

                if (pBoundaryCursor != null)
                {
                    Marshal.FinalReleaseComObject(pBoundaryCursor);
                }

                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
            return dtAllCountyData;
        }


        private static IFeatureClass GetFeatureClass(string argStrconnStrToFetchedgisws, string argStrFCName)
        {
            IWorkspace workspace = null;
            IFeatureClass pFeatureClass=null;
            try
            {
              
                Common._log.Info("Connection string used in process to fetch workspace." + argStrconnStrToFetchedgisws);
                workspace = MainClass.GetWorkspace(argStrconnStrToFetchedgisws);

                if (workspace == null)
                {
                    Common._log.Error("Work space not found for conn string : " + ReadConfigurations.GetValue(ReadConfigurations.ConnString_EDWorkSpace));
                    Common._log.Info("Exiting the process.");   
                    return null;
                }

                Common._log.Info("Workspace checked out successfully.");
                pFeatureClass = ((IFeatureWorkspace)workspace).OpenFeatureClass(argStrFCName);
            }
            catch (Exception exp)
            {
                Common._log.Error("Exception : " + exp.Message +" |  Stack Trace | " + exp.StackTrace+" at "+exp.ToString());
            }
            return pFeatureClass;
        }
    }
}
