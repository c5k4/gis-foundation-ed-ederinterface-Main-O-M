using System;
using System.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ESRI.ArcGIS.esriSystem;
using System.Reflection;
using System.IO;
using ESRI.ArcGIS.Geodatabase;
using System.Collections;
using System.Runtime.InteropServices;
using System.Data;
using Oracle.DataAccess.Client;
using System.Diagnostics;


namespace PGE.Interfaces.StreetlightGIS_Job
{
    class Program
    {
        private static LicenseInitializer m_AOLicenseInitializer = new StreetlightGIS_Job.LicenseInitializer();
        private static string sPath = (String.IsNullOrEmpty(ConfigurationManager.AppSettings["LOGPATH"]) ? System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) : ConfigurationManager.AppSettings["LOGPATH"]) + "\\Logfile_" + DateTime.Now.Ticks.ToString() + ".log";
        private static OracleDataAccess _oda = new OracleDataAccess(SLConfig.strOracleConnString_Eder);
        private static OracleDataAccess _oda_Webr = new OracleDataAccess(SLConfig.strOracleConnString_Webr);
        private static StreamWriter pSWriter = File.CreateText(sPath);
        private static Hashtable hTable_LastProcessedData = new Hashtable();
        private static string strJobId = string.Empty;
        private static string strAssignedID = string.Empty;
        private static string strTxMessage = string.Empty;
        private static Hashtable hTableFieldMapping = null;
        private static string strReportPath = string.Empty;
        private static string strUnifiedGridFilePath = string.Empty;

        private static IFeatureClass pFeatClass_UnifiedGrid = null;
        private static IFeatureClass pFeatClass_Stl = null;
        private static IFeatureClass pFeatClass_FieldPoint = null;

        private static IWorkspace pMdbWorkspace = null;
        private static IFeatureWorkspace pFeatWS_Pgdb = null;
        private static IFeatureClass pFC_Pgdb = null;

        static void Main(string[] args)
        {
            pSWriter.Close();
            //ESRI License Initializer generated code.
            Utils.WriteLine(sPath, DateTime.Now.ToLongTimeString() + " -- Initializing Licence");
            //ESRI License Initializer generated code.
            m_AOLicenseInitializer.InitializeApplication(new esriLicenseProductCode[] { esriLicenseProductCode.esriLicenseProductCodeBasic, esriLicenseProductCode.esriLicenseProductCodeStandard, esriLicenseProductCode.esriLicenseProductCodeAdvanced },
            new esriLicenseExtensionCode[] { });


            getFeatureClassObjects();
            StartSLUpdateProcess();
            StartCopyFileProcess();
            StartFieldDataProcessing();
            ReleaseObjects();


            //ESRI License Initializer generated code.
            //Do not make any call to ArcObjects after ShutDownApplication()
            m_AOLicenseInitializer.ShutdownApplication();
        }

        private static void getFeatureClassObjects()
        {
            IWorkspace pDesDBWorkSpace = null;
            IFeatureWorkspace pFeatWS_Oracle = null;
            try
            {
                pDesDBWorkSpace = Utils.ArcSdeWorkspaceFromFile(SLConfig.strDestinationDB_SDE_FilePath);
                //pDesDBWorkSpace = Utils.ArcSdeWorkspaceFromFile(SLConfig.strDestinationDB_SDE_FilePath_edgis);
                pFeatWS_Oracle = (IFeatureWorkspace)pDesDBWorkSpace;
                pFeatClass_UnifiedGrid = pFeatWS_Oracle.OpenFeatureClass(SLConfig.strUnifiedGrid_FCName);
                pFeatClass_FieldPoint = pFeatWS_Oracle.OpenFeatureClass(SLConfig.strFieldPoint_FCName);
                pFeatClass_Stl = pFeatWS_Oracle.OpenFeatureClass(SLConfig.strStreetlightInv_FCName);
            }
            catch (Exception ex)
            {
                Utils.WriteLine(sPath, DateTime.Now.ToLongTimeString() + "  ----**ERROR(getFeatureClassObjects)**----" + ex.Message);
                throw ex;
            }
        }

        private static void StartFieldDataProcessing()
        {
            List<string> lstPgdbPaths = new List<string>();
            OracleParameter[] values = null;
            string query = "";
            DataTable _dtJobDetails;
            string strDirPath = string.Empty;
            bool isSuccess = false;
            try
            {
                Utils.WriteLine(sPath, DateTime.Now.ToLongTimeString() + " -----------------Process Started for Task 2--------------------");
                query = "SELECT * FROM " + SLConfig.TB_AssignedTask + " Where " + SLConfig.FLD_TaskID_AssignedTask + "= '" + SLConfig.Value_GISTaskNumber + "'";
                _dtJobDetails = _oda_Webr.GetDataTableFromAdapter(query, values, CommandType.Text);
                if (_dtJobDetails != null && _dtJobDetails.Rows.Count > 0)
                {
                    if (_dtJobDetails != null && _dtJobDetails.Rows.Count > 0)
                    {
                        for (int iCount = 0; iCount < _dtJobDetails.Rows.Count; iCount++)
                        {
                            strJobId = _dtJobDetails.Rows[iCount][SLConfig.FLD_JobID_AssignedTask].ToString();
                            strAssignedID = _dtJobDetails.Rows[iCount]["ASSIGNID"].ToString();
                            Utils.WriteLine(sPath, DateTime.Now.ToLongTimeString() + " Processing field Data for JOB ID #: " + strJobId);
                            lstPgdbPaths = GetJobPGDBPath(strJobId);
                            if (lstPgdbPaths.Count > 0 && strJobId != null)
                            {
                                Utils.WriteLine(sPath, DateTime.Now.ToLongTimeString() + " PGDB(s) to process are :" + lstPgdbPaths.Count);
                                isSuccess = GetLastProcessedIDs();
                                if (isSuccess)
                                {
                                    //Utils.WriteLine(sPath, DateTime.Now.ToLongTimeString() + "Step 3: Push pgdb(s) data in to Oracle");
                                    for (int iCnt = 0; iCnt < lstPgdbPaths.Count; iCnt++)
                                    {
                                        isSuccess = ReadPgdbAndPushInOracle(lstPgdbPaths[iCnt]);

                                    }
                                    //Utils.WriteLine(sPath, DateTime.Now.ToLongTimeString() + "Step 4: Duplicate reocords fetch and archival");
                                    isSuccess = DuplicateDataHandling();
                                    if (isSuccess)
                                    {
                                        //Utils.WriteLine(sPath, DateTime.Now.ToLongTimeString() + "Step 5: Execution of stored procedures");
                                        isSuccess = ExecuteStoredProcedures();
                                        GenerateReports();

                                        MakeDBEntries(true, SLConfig.Value_GISTaskNumber);

                                        SendEmailNotification(true, SLConfig.Value_GISTaskNumber);

                                    }
                                }
                            }
                            else
                            {
                                Utils.WriteLine(sPath, DateTime.Now.ToLongTimeString() + " No PGDB(s) available to process");
                            }
                        }
                    }
                }
                else
                {
                    Utils.WriteLine(sPath, DateTime.Now.ToLongTimeString() + " No JOB ID available to process");
                }
                Utils.WriteLine(sPath, DateTime.Now.ToLongTimeString() + " -----------------Process Completed for Task 2 ------------");
            }
            catch (Exception ex)
            {
                Utils.WriteLine(sPath, DateTime.Now.ToLongTimeString() + "  ----**ERROR(StartProcessing)**----" + ex.Message);
                if (hTable_LastProcessedData["LastObjectID"] != null)
                {
                    _oda.ExecuteQuery("DELETE FROM " + SLConfig.strFieldPoint_FCName + " where objectid > " + hTable_LastProcessedData["LastObjectID"], values, CommandType.Text);
                    Utils.WriteLine(sPath, DateTime.Now.ToLongTimeString() + "**Feature deleted form FieldPts**----");
                }
                MakeDBEntries(false, "2");
                SendEmailNotification(false, SLConfig.Value_GISTaskNumber);
                throw ex;
            }
        }

        private static List<string> GetJobPGDBPath(string strJobID)
        {
            List<string> lstPgdbPaths = new List<string>();
            string strDirPath = string.Empty;
            try
            {
                //strDirPath = "\\\\ffdistgis-nas01\\itgis-nas01\\Team Members\\MuditAgarwal\\Steetlight\\00ToBeProcessed\\CITY_OF_PARLIER_TT_FOR_UPLOAD_101816";// SLConfig.strJobfolderPath + "\\" + strJobId;
                //strDirPath = "\\\\ffdistgis-nas01\\itgis-nas01\\Team Members\\MuditAgarwal\\Steetlight\\00ToBeProcessed\\CITY_OF_PARLIER_TT_FOR_UPLOAD_101816";
                strDirPath = SLConfig.strJobfolderPath + "\\" + strJobId;
                if (Directory.Exists(strDirPath))
                {
                    foreach (string file in Directory.EnumerateFiles(strDirPath, "*.mdb"))
                    {
                        lstPgdbPaths.Add(file);
                    }
                }
                else
                {
                    Utils.WriteLine(sPath, DateTime.Now.ToLongTimeString() + " ----**Warning**----Path for Directory not available-------------");
                }
            }
            catch (Exception ex)
            {
                Utils.WriteLine(sPath, DateTime.Now.ToLongTimeString() + "----**ERROR(GetJobPGDBPaths)**----" + ex.Message);
                throw ex;
            }
            return lstPgdbPaths;
        }

        private static bool GetLastProcessedIDs()
        {
            DataTable dtResponse = new DataTable();
            OracleParameter[] values = null;
            string query = "";
            bool boolSuccess = false;
            try
            {
                CommandType cmdType = CommandType.Text;
                query = SLConfig.Query_LastDataProcessed;

                dtResponse = _oda.GetDataTableFromAdapter(query, values, cmdType);
                if (dtResponse != null && dtResponse.Rows.Count == 1)
                {
                    hTable_LastProcessedData["LastObjectID"] = dtResponse.Rows[0]["LastObjectID"].ToString();
                    hTable_LastProcessedData["LastGISID"] = dtResponse.Rows[0]["LastGISID"].ToString();
                }
                Utils.WriteLine(sPath, DateTime.Now.ToLongTimeString() + " Last OBJECTID Processeed is :" + dtResponse.Rows[0]["LastObjectID"].ToString());
                Utils.WriteLine(sPath, DateTime.Now.ToLongTimeString() + " Last GIS_ID Processeed is :" + dtResponse.Rows[0]["LastGISID"].ToString());
                boolSuccess = true;
            }
            catch (Exception ex)
            {
                Utils.WriteLine(sPath, DateTime.Now.ToLongTimeString() + "  ----**ERROR(GetLastProcessedIDs)**----" + ex.Message);
                throw ex;
            }
            return boolSuccess;
        }

        private static bool ReadPgdbAndPushInOracle(string strPgdbPath)
        {
            bool boolSuccess = false;
            IQueryFilter queryFilter = new QueryFilterClass();
            IFeatureCursor pFeatCur_Pgdb = null;
            IFeature pFeature_Pgdb;

            int intTotalFeatureToProcess = 0;
            int intFeatureCounter = 0;
            try
            {
                pMdbWorkspace = Utils.AccessWorkspaceFromPropertySet(strPgdbPath);
                pFeatWS_Pgdb = (IFeatureWorkspace)pMdbWorkspace;
                pFC_Pgdb = pFeatWS_Pgdb.OpenFeatureClass(SLConfig.strPGDB_FCName);
                if (hTableFieldMapping == null)
                {
                    hTableFieldMapping = Utils.GetFieldMapping(pFC_Pgdb, pFeatClass_FieldPoint, SLConfig.Value_GISTaskNumber);
                }
                if (hTableFieldMapping.Keys.Count > 1)
                {
                    queryFilter.SubFields = "*";
                    queryFilter.WhereClause = "";
                    pFeatCur_Pgdb = pFC_Pgdb.Search(queryFilter, false);
                    intTotalFeatureToProcess = pFC_Pgdb.FeatureCount(queryFilter);
                    Utils.WriteLine(sPath, DateTime.Now.ToLongTimeString() + " Feature count to insert in FLD PTS  : " + intTotalFeatureToProcess);
                    while ((pFeature_Pgdb = pFeatCur_Pgdb.NextFeature()) != null)
                    {
                        boolSuccess = CreateFeatureInDestinationDB(pFeature_Pgdb, pFeatClass_FieldPoint, hTableFieldMapping);
                        if (boolSuccess)
                        {
                            intFeatureCounter++;
                            //Utils.WriteLine(sPath, DateTime.Now.ToLongTimeString() + " ----Processed Feature : " + intFeatureCounter + " of " + intTotalFeatureToProcess);
                        }
                        if (boolSuccess == false)
                        {
                        }
                    }
                    boolSuccess = true;
                }
                else
                {
                    Utils.WriteLine(sPath, DateTime.Now.ToLongTimeString() + "----**Warning**----Mapping fields not available-------------");
                }
            }
            catch (Exception ex)
            {
                Utils.WriteLine(sPath, DateTime.Now.ToLongTimeString() + "----**ERROR(ReadPgdbAndPushInOracle)**----" + ex.Message);
                throw ex;
            }
            finally
            {
                if (pFeatCur_Pgdb != null) Marshal.FinalReleaseComObject(pFeatCur_Pgdb);
                GC.Collect();
            }
            return boolSuccess;
        }

        private static bool CreateFeatureInDestinationDB(IFeature pSourceFeature, IFeatureClass pDestFClass, Hashtable FieldMappingTable)
        {
            bool isfeatureCreated = false;
            IFeature pNewFeature = null;
            try
            {
                if (pSourceFeature != null || pSourceFeature.Shape != null)
                {
                    pNewFeature = pDestFClass.CreateFeature();
                    pNewFeature.Shape = pSourceFeature.Shape;
                    foreach (DictionaryEntry entry in FieldMappingTable)
                    {
                        if (pSourceFeature.get_Value(Convert.ToInt16(entry.Key)) != null)
                        {
                            if (pSourceFeature.get_Value(Convert.ToInt16(entry.Key)).ToString().Trim() != string.Empty)
                            {
                                pNewFeature.set_Value(Convert.ToInt16(entry.Value), pSourceFeature.get_Value(Convert.ToInt16(entry.Key)));
                            }
                        }


                    }
                    pNewFeature.Store();
                    isfeatureCreated = true;
                }
            }
            catch (Exception ex)
            {
                Utils.WriteLine(sPath, DateTime.Now.ToLongTimeString() + "  ----**ERROR(CreateFeatureInDestinationDB)**----" + ex.Message);
                Utils.WriteLine(sPath, DateTime.Now.ToLongTimeString() + "  ----**ERROR(CreateFeatureInDestinationDB), Source Feature ID: " + pSourceFeature.OID);
                throw ex;
            }
            finally
            {
                //if (pDestFClass != null) Marshal.FinalReleaseComObject(pDestFClass);
                //if (pSourceFeature != null) Marshal.FinalReleaseComObject(pSourceFeature);
                //GC.Collect();
            }
            return isfeatureCreated;
        }

        private static bool CreateFeatureInDestinationDB(IFeature pSourceFeature, IFeatureClass pDestFClass, Hashtable FieldMappingTable, IFeatureClass pFeatClassUnifiedGrid, int intIndexMapNoGridFC, int intIndexMapNoSTL, int intindexScaleGridFC, out string strMapNo)
        {
            bool isfeatureCreated = false;
            IFeature pNewFeature = null;
            ISpatialFilter pSFilter = null;
            IFeatureCursor pFCursor1 = default(IFeatureCursor);
            IFeature pFeatUnifiedGrid = null;
            strMapNo = string.Empty;
            try
            {
                if (pSourceFeature != null || pSourceFeature.Shape != null)
                {
                    pNewFeature = pDestFClass.CreateFeature();
                    pNewFeature.Shape = pSourceFeature.Shape;
                    foreach (DictionaryEntry entry in FieldMappingTable)
                    {
                        if (pSourceFeature.get_Value(Convert.ToInt16(entry.Key)) != null)
                        {
                            if (pSourceFeature.get_Value(Convert.ToInt16(entry.Key)).ToString().Trim() != string.Empty)
                            {
                                pNewFeature.set_Value(Convert.ToInt16(entry.Value), pSourceFeature.get_Value(Convert.ToInt16(entry.Key)));
                            }
                        }
                    }

                    pSFilter = new SpatialFilterClass();
                    pSFilter.GeometryField = pFeatClassUnifiedGrid.ShapeFieldName;
                    pSFilter.SearchOrder = esriSearchOrder.esriSearchOrderSpatial;
                    pSFilter.SpatialRel = esriSpatialRelEnum.esriSpatialRelIntersects;
                    pSFilter.Geometry = pSourceFeature.ShapeCopy;
                    //using (ComReleaser comReleaser1 = new ComReleaser())
                    //{
                    pFCursor1 = pFeatClassUnifiedGrid.Search(pSFilter, true);
                    //comReleaser1.ManageLifetime(pFCursor1);
                    //WriteLine(DateTime.Now.ToLongTimeString() + " -- Intersecting MainPlat are : " + pFClass_MainPlat.FeatureCount(pSFilter));
                    while ((pFeatUnifiedGrid = pFCursor1.NextFeature()) != null)
                    {
                        if (pFeatUnifiedGrid.get_Value(intindexScaleGridFC).ToString() == "100")
                        {
                            strMapNo = pFeatUnifiedGrid.get_Value(intIndexMapNoGridFC).ToString();
                            //Utils.WriteToFile(strMapNo, strFilePathForUnifiedGrid);
                            break;
                        }
                    }
                    pNewFeature.set_Value(intIndexMapNoSTL, strMapNo);
                    pNewFeature.Store();
                    isfeatureCreated = true;
                }

            }
            catch (Exception ex)
            {
                Utils.WriteLine(sPath, DateTime.Now.ToLongTimeString() + "  ----**ERROR(CreateFeatureInDestinationDB)**----" + ex.Message);
                Utils.WriteLine(sPath, DateTime.Now.ToLongTimeString() + "  ----**ERROR(CreateFeatureInDestinationDB), Source Feature ID: " + pSourceFeature.OID);
                throw ex;
            }
            finally
            {
                if (pFCursor1 != null) Marshal.FinalReleaseComObject(pFCursor1);
                GC.Collect();
            }
            return isfeatureCreated;
        }

        private static bool DuplicateDataHandling()
        {
            DataTable dtResponse = new DataTable();
            OracleParameter[] values = null;
            string query = "";
            string strDuplicateCount = "";
            bool success = false;
            int intRowsAffected = 0;
            try
            {
                CommandType cmdType = CommandType.Text;
                //Select Count for duplicate Records
                query = string.Format(SLConfig.Query_DuplicateCount, hTable_LastProcessedData["LastObjectID"]);
                dtResponse = _oda.GetDataTableFromAdapter(query, values, cmdType);
                if (dtResponse != null && dtResponse.Rows.Count == 1)
                {
                    strDuplicateCount = dtResponse.Rows[0][0].ToString();
                    Utils.WriteLine(sPath, DateTime.Now.ToLongTimeString() + " Duplicate records found :" + strDuplicateCount);
                }
                if (Convert.ToInt64(strDuplicateCount) > 0)
                {
                    //Insert Duplicate Records in fieldDelete Table
                    query = string.Format(SLConfig.Query_DuplicateArchival, hTable_LastProcessedData["LastObjectID"]);
                    intRowsAffected = _oda.ExecuteQuery(query, values, cmdType);
                    Utils.WriteLine(sPath, DateTime.Now.ToLongTimeString() + " Duplicate records archived:" + intRowsAffected);

                    //Delete Duplicate Records
                    query = string.Format(SLConfig.Query_DeleteDuplicates, hTable_LastProcessedData["LastObjectID"]);
                    intRowsAffected = _oda.ExecuteQuery(query, values, cmdType);
                    Utils.WriteLine(sPath, DateTime.Now.ToLongTimeString() + " Duplicate records deleted from main table :" + intRowsAffected);
                }
                //Update Field Point Table
                object[] args = new object[] { SLConfig.strFieldPoint_FCName, hTable_LastProcessedData["LastGISID"] + "+ rownum", hTable_LastProcessedData["LastObjectID"] };
                query = string.Format(SLConfig.Query_UpdateFieldPoint, args);
                intRowsAffected = _oda.ExecuteQuery(query, values, cmdType);
                Utils.WriteLine(sPath, DateTime.Now.ToLongTimeString() + " GISIDs are updated for records :" + intRowsAffected);

                success = true;
            }
            catch (Exception ex)
            {
                Utils.WriteLine(sPath, DateTime.Now.ToLongTimeString() + "  ----**ERROR(DuplicateDataHandling)**----" + ex.Message);
                //_oda.ExecuteQuery("DELETE FROM " + SLConfig.strFieldPoint_FCName + " where objectid > " + hTable_LastProcessedData["LastObjectID"], values, CommandType.Text);
                //Utils.WriteLine(sPath, DateTime.Now.ToLongTimeString() + "**Feature deleted form FieldPts**----");
                throw ex;
            }
            return success;
        }

        private static bool ExecuteStoredProcedures()
        {
            bool isSuccess = false;
            int intCheck;
            try
            {
                OracleParameter[] prmIn_Out_Collection = new OracleParameter[1];
                prmIn_Out_Collection[0] = new OracleParameter("in_ID", Convert.ToInt32(hTable_LastProcessedData["LastGISID"]));
                prmIn_Out_Collection[0].Direction = ParameterDirection.Input;
                //Utils.WriteLine(sPath, DateTime.Now.ToLongTimeString() + "Field Compare Stored Procedure started");
                intCheck = _oda.ExecuteQuery(SLConfig.strOracleProc1Name, prmIn_Out_Collection, CommandType.StoredProcedure);
                //Utils.WriteLine(sPath, DateTime.Now.ToLongTimeString() + "Field Compare Stored Procedure Competed");
                 isSuccess = true;
            }
            catch (Exception ex)
            {
                Utils.WriteLine(sPath, DateTime.Now.ToLongTimeString() + "----**ERROR(ExecuteStoredProcedures)**----" + ex.Message);
                throw ex;
            }
            return isSuccess;
        }

        private static void SendEmailNotification(bool success, string strTaskID)
        {
            string strODAUserQuery = "SELECT * FROM " + SLConfig.TB_Roles + " WHERE ROLEID = " + SLConfig.strODARoleID;
            string strGISOpsUserQuery = "SELECT * FROM " + SLConfig.TB_Roles + " WHERE ROLEID = " + SLConfig.strGISOpsRoleID;
            DataTable dtLanIDs = new DataTable();
            OracleParameter[] values = null;
            List<string> lstODALanIds = new List<string>();
            List<string> lstGISOpsLanIds = new List<string>();
            string strLANIDs = string.Empty; string[] ArrstrLanIDs = null;
            try
            {

                //ODA LAN IDs from database 
                dtLanIDs = _oda_Webr.GetDataTableFromAdapter(strODAUserQuery, values, CommandType.Text);
                if (dtLanIDs.Rows.Count > 0)
                {
                    strLANIDs = dtLanIDs.Rows[0]["MEMBERS"].ToString();
                    if (strLANIDs != string.Empty)
                    {
                        ArrstrLanIDs = strLANIDs.Split(',');
                    }
                    for (int iCnt = 0; iCnt < ArrstrLanIDs.Length; iCnt++)
                    {
                        lstODALanIds.Add(ArrstrLanIDs[iCnt]);
                    }
                }
                //GIS Ops LAN IDs from database 
                dtLanIDs = _oda_Webr.GetDataTableFromAdapter(strGISOpsUserQuery, values, CommandType.Text);
                if (dtLanIDs.Rows.Count > 0)
                {
                    strLANIDs = dtLanIDs.Rows[0]["MEMBERS"].ToString();
                    if (strLANIDs != string.Empty)
                    {
                        ArrstrLanIDs = strLANIDs.Split(',');
                    }
                    for (int iCnt = 0; iCnt < ArrstrLanIDs.Length; iCnt++)
                    {
                        lstGISOpsLanIds.Add(ArrstrLanIDs[iCnt]);
                    }
                }

                clsEmailDetails objClsEmailDetails = new clsEmailDetails();
                objClsEmailDetails.StrJobId = strJobId;
                objClsEmailDetails.StrTaskId = strTaskID;
                if (success)
                {
                    if (strTaskID == SLConfig.Value_GISTaskNumber)
                    {
                        objClsEmailDetails.LstToLanIds = lstODALanIds;
                        objClsEmailDetails.LstCcLanIDs = lstGISOpsLanIds;
                    }
                    else
                    {
                        objClsEmailDetails.LstToLanIds = lstGISOpsLanIds;
                    }
                }
                else
                {
                    objClsEmailDetails.LstToLanIds = lstGISOpsLanIds;
                }
                Utils.SendMail(objClsEmailDetails, success, strTaskID);

            }
            catch (Exception ex)
            {
                Utils.WriteLine(sPath, DateTime.Now.ToLongTimeString() + "----**ERROR(SendEmailNotification)**----" + ex.Message);
                throw ex;
            }
        }

        private static void MakeDBEntries(bool success, string strTaskID)
        {
            OracleParameter[] values = null;
            string strDeleteQuery = "DELETE FROM " + SLConfig.TB_AssignedTask + " WHERE ASSIGNID = " + strAssignedID;
            string strInsertQueryAssignedTask = string.Empty;

            string strInsertQueryTransaction = "INSERT INTO " + SLConfig.TB_Transaction +
                                " values(" + Convert.ToInt64(strJobId) + ", '" + strTaskID + "','{0}','{1}', TO_DATE('" + DateTime.Now + "','MM/DD/YYYY HH:MI:SS am') " + ",'AUTO','{2}')";
            int intRowsAffected = -1;
            string strSuccessMsg = strTxMessage + "The files are in " + Environment.NewLine + strReportPath + Environment.NewLine + "for your inspection and approval.";
            string strLastprocessedMsg = string.Empty;
            string strUnifiedGridMsg = string.Empty;
            object[] args = null;
            try
            {
                if (hTable_LastProcessedData.Keys.Count > 0)
                {
                    strLastprocessedMsg = "*************************" + Environment.NewLine;
                    strLastprocessedMsg += "Last processed OBJECTID:" + hTable_LastProcessedData["LastObjectID"] + ",";
                    strLastprocessedMsg += "Last processed GISID:" + hTable_LastProcessedData["LastGISID"] + Environment.NewLine;
                    strLastprocessedMsg += "*************************";
                }
                if (success)
                {
                    if (strTaskID == SLConfig.Value_GISTaskNumber)
                    {
                        strInsertQueryAssignedTask = "INSERT INTO " + SLConfig.TB_AssignedTask +
                             " values ((select max(assignid)+1 from " + SLConfig.TB_AssignedTask + ")," +
                             Convert.ToInt64(strJobId) + ", '" + SLConfig.Value_ODAReviewTaskNumber + "'," + SLConfig.strODARoleID + ",TO_DATE('" + DateTime.Now + "','MM/DD/YYYY HH:MI:SS am')" + ")";
                        intRowsAffected = _oda_Webr.ExecuteQuery(strInsertQueryAssignedTask, values, CommandType.Text);
                        args = new object[] { "Y", "GIS Process Completed \r\n" + strSuccessMsg + Environment.NewLine + strLastprocessedMsg, null };
                    }
                    else if (strTaskID == SLConfig.Value_UpdateSLTableTaskNumber)
                    {
                        if (strUnifiedGridFilePath != string.Empty)
                        {
                            strUnifiedGridMsg = " Unified map grid file is located at :" + Environment.NewLine + strUnifiedGridFilePath;
                        }
                        args = new object[] { "Y", "Streetlight feature class updated" + strUnifiedGridMsg, null };
                    }
                    else if (strTaskID == SLConfig.Value_CopyFilesTaskNumber)
                    {
                        args = new object[] { "Y", "Files copied successfully", null };
                    }
                    strInsertQueryTransaction = string.Format(strInsertQueryTransaction, args);
                    intRowsAffected = _oda_Webr.ExecuteQuery(strInsertQueryTransaction, values, CommandType.Text);
                    intRowsAffected = _oda_Webr.ExecuteQuery(strDeleteQuery, values, CommandType.Text);
                }
                else
                {
                    if (strTaskID == SLConfig.Value_GISTaskNumber)
                    {
                        args = new object[] { "N", "Error in GIS process", strLastprocessedMsg };
                    }
                    else if (strTaskID == SLConfig.Value_UpdateSLTableTaskNumber)
                    {
                        args = new object[] { "N", "Error in Updating Streetlight feature class", null };
                    }
                    else if (strTaskID == SLConfig.Value_CopyFilesTaskNumber)
                    {
                        args = new object[] { "N", "Error in copying files", null };
                    }
                    strInsertQueryTransaction = string.Format(strInsertQueryTransaction, args);
                    intRowsAffected = _oda_Webr.ExecuteQuery(strInsertQueryTransaction, values, CommandType.Text);
                }
            }
            catch (Exception ex)
            {
                Utils.WriteLine(sPath, DateTime.Now.ToLongTimeString() + "  ----**ERROR(MakeDBEntries[Task # " + strTaskID + "])**----" + ex.Message);
                throw ex;
            }
        }

        private static bool GenerateReports()
        {
            bool success = false;
            OracleParameter[] values = null;
            string strQuery = string.Empty;
            string strReportFolderPath = string.Empty;
            DataTable _DtableData;
            try
            {
                //shTable_LastProcessedData["LastObjectID"] = "636688";

                //Append Report
                strQuery = "SELECT * FROM " + SLConfig.AppendDataView + " WHERE to_number(gis_id) > " + hTable_LastProcessedData["LastGISID"];
                strReportPath = System.IO.Path.Combine(SLConfig.strJobfolderPath, strJobId);
                if (System.IO.Directory.Exists(strReportPath))
                {
                    strReportPath = System.IO.Path.Combine(strReportPath, "data_to_cdx");
                    if (!System.IO.Directory.Exists(strReportPath))
                    {
                        System.IO.Directory.CreateDirectory(strReportPath);
                    }
                }
                strReportFolderPath = strReportPath + "\\" + SLConfig.AppendReportName + ".txt";
                _DtableData = _oda.GetDataTableFromAdapter(strQuery, values, CommandType.Text);
                if (_DtableData != null && _DtableData.Rows.Count > 0)
                {
                    Utils.WriteToFile(_DtableData, strReportFolderPath, false, "\t");
                    strTxMessage = "********" + _DtableData.Rows.Count + " records written to " + SLConfig.AppendReportName + ".txt " + Environment.NewLine;
                    Utils.WriteLine(sPath, DateTime.Now.ToLongTimeString() + "  " + _DtableData.Rows.Count + " Records written to Append report " + SLConfig.AppendReportName + ".txt ");
                }
                else
                {
                    Utils.WriteToFile(null, strReportFolderPath, false, "\t");
                    Utils.WriteLine(sPath, DateTime.Now.ToLongTimeString() + " No data available for Append Report");
                }

                //Delete Report
                strQuery = "SELECT * FROM " + SLConfig.DeleteDataView + " WHERE to_number(gis_id) > " + hTable_LastProcessedData["LastGISID"];
                strReportFolderPath = strReportPath + "\\" + SLConfig.DeleteReportName + ".txt";
                _DtableData = _oda.GetDataTableFromAdapter(strQuery, values, CommandType.Text);
                if (_DtableData != null && _DtableData.Rows.Count > 0)
                {
                    Utils.WriteToFile(_DtableData, strReportFolderPath, false, "\t");
                    strTxMessage += "********" + _DtableData.Rows.Count + " records written to " + SLConfig.DeleteReportName + ".txt " + Environment.NewLine;
                    Utils.WriteLine(sPath, DateTime.Now.ToLongTimeString() + "  " + _DtableData.Rows.Count + " Records written to Delete report " + SLConfig.DeleteReportName + ".txt ");
                }
                else
                {
                    Utils.WriteToFile(null, strReportFolderPath, false, "\t");
                    Utils.WriteLine(sPath, DateTime.Now.ToLongTimeString() + " No data available for Delete Report");
                }

                //Update Report
                strQuery = "SELECT * FROM " + SLConfig.UpdateDataView + " WHERE to_number(gis_id) > " + hTable_LastProcessedData["LastGISID"];
                strReportFolderPath = strReportPath + "\\" + SLConfig.UpdateReportName + ".txt";
                _DtableData = _oda.GetDataTableFromAdapter(strQuery, values, CommandType.Text);
                if (_DtableData != null && _DtableData.Rows.Count > 0)
                {
                    Utils.WriteToFile(_DtableData, strReportFolderPath, false, "\t");
                    strTxMessage += "********" + _DtableData.Rows.Count + " records written to " + SLConfig.UpdateReportName + ".txt " + Environment.NewLine;
                    Utils.WriteLine(sPath, DateTime.Now.ToLongTimeString() + "  " + _DtableData.Rows.Count + " Records written to Update report " + SLConfig.UpdateReportName + ".txt ");
                }
                else
                {
                    Utils.WriteToFile(null, strReportFolderPath, false, "\t");
                    Utils.WriteLine(sPath, DateTime.Now.ToLongTimeString() + " No data available for Update Report");
                }

                //Other Report cdx_other
                strQuery = "SELECT * FROM " + SLConfig.OtherDataView + " WHERE to_number(gis_id) > " + hTable_LastProcessedData["LastGISID"];
                strReportFolderPath = strReportPath + "\\" + SLConfig.OtherReportName + ".txt";
                _DtableData = _oda.GetDataTableFromAdapter(strQuery, values, CommandType.Text);
                if (_DtableData != null && _DtableData.Rows.Count > 0)
                {
                    Utils.WriteToFile(_DtableData, strReportFolderPath, true, "\t");
                    strTxMessage += "********" + _DtableData.Rows.Count + " records written to " + SLConfig.OtherReportName + ".txt " + Environment.NewLine;
                    Utils.WriteLine(sPath, DateTime.Now.ToLongTimeString() + "  " + _DtableData.Rows.Count + " Records written to Other report " + SLConfig.OtherReportName + ".txt ");
                }
                else
                {
                    Utils.WriteToFile(null, strReportFolderPath, false, "\t");
                    Utils.WriteLine(sPath, DateTime.Now.ToLongTimeString() + " No data available for Other Report");
                }

                //Problem Report
                strQuery = "SELECT * FROM " + SLConfig.ProblemDataView + " WHERE to_number(gis_id) > " + hTable_LastProcessedData["LastGISID"];
                strReportFolderPath = strReportPath + "\\" + SLConfig.ProblemReportName + ".txt";
                _DtableData = _oda.GetDataTableFromAdapter(strQuery, values, CommandType.Text);
                if (_DtableData != null && _DtableData.Rows.Count > 0)
                {
                    Utils.WriteToFile(_DtableData, strReportFolderPath, false, "\t");
                    strTxMessage += "********" + _DtableData.Rows.Count + " records written to " + SLConfig.ProblemReportName + ".txt " + Environment.NewLine;
                    Utils.WriteLine(sPath, DateTime.Now.ToLongTimeString() + "  " + _DtableData.Rows.Count + " Records written to Problem report " + SLConfig.ProblemReportName + ".txt ");
                }
                else
                {
                    Utils.WriteToFile(null, strReportFolderPath, false, "\t");
                    Utils.WriteLine(sPath, DateTime.Now.ToLongTimeString() + " No data available for Problem Report");
                }

                //MC2 Report cdx_cc
                strQuery = "SELECT * FROM " + SLConfig.Mc2View + " WHERE to_number(gis_id) > " + hTable_LastProcessedData["LastGISID"];
                strReportFolderPath = strReportPath + "\\" + SLConfig.Mc2ReportName + ".txt";
                _DtableData = _oda.GetDataTableFromAdapter(strQuery, values, CommandType.Text);
                if (_DtableData != null && _DtableData.Rows.Count > 0)
                {
                    Utils.WriteToFile(_DtableData, strReportFolderPath, true, "\t");
                    strTxMessage += "********" + _DtableData.Rows.Count + " records written to " + SLConfig.Mc2ReportName + ".txt " + Environment.NewLine;
                    Utils.WriteLine(sPath, DateTime.Now.ToLongTimeString() + "  " + _DtableData.Rows.Count + " Records written to MC2 report " + SLConfig.Mc2ReportName + ".txt ");
                }
                else
                {
                    Utils.WriteToFile(null, strReportFolderPath, false, "\t");
                    Utils.WriteLine(sPath, DateTime.Now.ToLongTimeString() + " No data available for MC2 Report");
                }


                if (Directory.Exists(strReportPath))
                {
                    string strData_to_cdxPath = Path.Combine(Directory.GetParent(SLConfig.strJobfolderPath).FullName, "data_to_cdx");
                    foreach (var file in Directory.GetFiles(strReportPath))
                    {
                        File.Copy(file, Path.Combine(strData_to_cdxPath, Path.GetFileName(file)), true);
                        //Commenting below section for changing CC&B path from edgisbtcprd09 server to NAS --10/27/2017
                       // File.Copy(file, Path.Combine(SLConfig.data_to_cdx_path, Path.GetFileName(file)), true);
                    }
                    
                }
                Utils.WriteLine(sPath, DateTime.Now.ToLongTimeString() + " Report Files copied to data_to_cdx folder ");
                
                //Copy 6 txt files in ELVIS
                //string strDirPath = SLConfig.strJobfolderPath + "\\" + strJobId;
                //if (Directory.Exists(strDirPath))
                //{
                //    string path = Path.Combine(Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath), "CopyFiles.bat");
                //    FileInfo _fileinfo = new FileInfo(path);
                //    if (_fileinfo.Exists)
                //    {
                //        Utils.WriteLine(sPath, DateTime.Now.ToLongTimeString() + " Executing bat file for copy ");
                //        //ExecuteCommand(path + " "+strDirPath + " " + SLConfig.Value_GISTaskNumber, Directory.GetCurrentDirectory());
                //        //ExecuteCommandSync(path + " " + strDirPath + " " + SLConfig.Value_GISTaskNumber);
                //    }
                //    else
                //    {
                //        Utils.WriteLine(sPath, DateTime.Now.ToLongTimeString() + " Copy bat file is missing " + path);
                //    }
                //}

            }
            catch (Exception ex)
            {
                Utils.WriteLine(sPath, DateTime.Now.ToLongTimeString() + "  ----**ERROR(GenerateReports)**----" + ex.Message);
                throw ex;
            }
            return success;
        }

        private static void StartCopyFileProcess()
        {
            string strQuery = string.Empty;
            OracleParameter[] values = null;
            DataTable _dtJobDetails;
            string strLastProcessedObjectID = string.Empty;
            string strObjectID = string.Empty;
            string TargetPath_Jobfolder = string.Empty;
            string SourcePath, TargetPath_Stlfolder = string.Empty;
            try
            {
                Utils.WriteLine(sPath, DateTime.Now.ToLongTimeString() + " -----------------Process Started for Task 5.2--------------------");
                CommandType cmdType = CommandType.Text;

                strQuery = "SELECT * FROM " + SLConfig.TB_AssignedTask + " Where " + SLConfig.FLD_TaskID_AssignedTask + "= '" + SLConfig.Value_CopyFilesTaskNumber + "'";
                _dtJobDetails = _oda_Webr.GetDataTableFromAdapter(strQuery, values, cmdType);
                if (_dtJobDetails != null && _dtJobDetails.Rows.Count > 0)
                {
                    for (int iCnt = 0; iCnt < _dtJobDetails.Rows.Count; iCnt++)
                    {
                        strJobId = _dtJobDetails.Rows[iCnt][SLConfig.FLD_JobID_AssignedTask].ToString();
                        strAssignedID = _dtJobDetails.Rows[iCnt]["ASSIGNID"].ToString();
                        Utils.WriteLine(sPath, DateTime.Now.ToLongTimeString() + " Processing Step 5.2 for JOB ID #: " + strJobId);
                        
                        TargetPath_Jobfolder = SLConfig.strJobfolderPath + "\\" + strJobId + "\\data_from_cdx";
                        if (!Directory.Exists(TargetPath_Jobfolder))
                        {
                            System.IO.Directory.CreateDirectory(TargetPath_Jobfolder);
                        }

                        #region Commenting below section for changing CC&B path from edgisbtcprd09 server to NAS --10/27/2017
                        /*TargetPath_Stlfolder = Path.Combine(Directory.GetParent(SLConfig.strJobfolderPath).FullName, "data_from_cdx"); 
                        
                        
                        SourcePath = SLConfig.data_from_cdx_path;
                        
                        string[] strArrFiles = Directory.GetFiles(SourcePath);
                        if (strArrFiles.Length > 0)
                        {
                            foreach (var file in Directory.GetFiles(SourcePath))
                            {
                                File.Copy(file, Path.Combine(TargetPath_Jobfolder, Path.GetFileName(file)), true);
                                File.Copy(file, Path.Combine(TargetPath_Stlfolder, Path.GetFileName(file)), true);
                            }
                            Utils.WriteLine(sPath, DateTime.Now.ToLongTimeString() + " File copied successfully ");
                            MakeDBEntries(true, SLConfig.Value_CopyFilesTaskNumber);
                        }*/
                        #endregion

                        SourcePath = Path.Combine(Directory.GetParent(SLConfig.strJobfolderPath).FullName, "data_from_cdx");
                        
                        string[] strArrFiles = Directory.GetFiles(SourcePath);
                        if (strArrFiles.Length > 0)
                        {
                            foreach (var file in Directory.GetFiles(SourcePath))
                            {
                                File.Copy(file, Path.Combine(TargetPath_Jobfolder, Path.GetFileName(file)), true);
                            }
                            Utils.WriteLine(sPath, DateTime.Now.ToLongTimeString() + " File copied successfully ");
                            MakeDBEntries(true, SLConfig.Value_CopyFilesTaskNumber);
                        }
                        else
                        {
                            MakeDBEntries(false, SLConfig.Value_CopyFilesTaskNumber);
                        }
                        //if (Directory.Exists(strDirTargetPath_Jobfolder))
                        //{
                        //    string strData_to_cdxPath = Path.Combine(Directory.GetParent(SLConfig.strJobfolderPath).FullName, "data_to_cdx");
                        //    //DirectoryInfo dirInfoTarget = new DirectoryInfo(strData_to_cdxPath);

                        //    foreach (var file in Directory.GetFiles(strDirPath))
                        //        File.Copy(file, Path.Combine(strData_to_cdxPath, Path.GetFileName(file)), true);


                        //    //string path = Path.Combine(Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath), "CopyFiles.bat");
                        //    //FileInfo _fileinfo = new FileInfo(path);
                        //    //if (_fileinfo.Exists)
                        //    //{
                        //    //    Utils.WriteLine(sPath, DateTime.Now.ToLongTimeString() + " Executing bat file for copy ");
                        //    //    //ExecuteCommand(path + " \"" + strDirPath + "\" " + SLConfig.Value_CopyFilesTaskNumber, Directory.GetCurrentDirectory());
                        //    //    ExecuteCommandSync(path + " " + strDirPath + " " + SLConfig.Value_CopyFilesTaskNumber);
                        //    //}
                        //    //else
                        //    //{
                        //    //    Utils.WriteLine(sPath, DateTime.Now.ToLongTimeString() + " Copy bat file is missing");
                        //    //}
                        //}
                        //Utils.ExecuteCommand(@"C:\Users\m4ab\Downloads\test\Test.bat");
                        
                    }
                }
                else
                {
                    Utils.WriteLine(sPath, DateTime.Now.ToLongTimeString() + " No JOB ID is available for files copy process");
                }
            }
            catch (Exception ex)
            {
                MakeDBEntries(false, SLConfig.Value_CopyFilesTaskNumber);
                Utils.WriteLine(sPath, DateTime.Now.ToLongTimeString() + "  ----**ERROR(StartSLUpdateProcess)**----" + ex.Message);
                throw ex;
            }
        }



        //public static void ExecuteCommand(string command, string strWorkingDirPath)
        //{
        //    int exitCode;
        //    //ProcessStartInfo processInfo;
        //    Process process = new Process();

        //    process.StartInfo.FileName = command;
        //    //process.StartInfo.WorkingDirectory = @"C:\Users\m4ab\Downloads\test";
        //    process.StartInfo.WorkingDirectory = strWorkingDirPath;
        //    process.StartInfo.CreateNoWindow = true;
        //    process.StartInfo.UseShellExecute = false;
        //    // *** Redirect the output ***
        //    process.StartInfo.RedirectStandardError = true;
        //    process.StartInfo.RedirectStandardOutput = true;
        //    process.Start();
        //    process.WaitForExit();

        //    // *** Read the streams ***
        //    // Warning: This approach can lead to deadlocks, see Edit #2
        //    string output = process.StandardOutput.ReadToEnd();
        //    string error = process.StandardError.ReadToEnd();

        //    exitCode = process.ExitCode;

        //    Utils.WriteLine(sPath, DateTime.Now.ToLongTimeString() + " Bat output" + (String.IsNullOrEmpty(output) ? "(none)" : output));
        //    Utils.WriteLine(sPath, DateTime.Now.ToLongTimeString() + "Bat error" + (String.IsNullOrEmpty(error) ? "(none)" : error));
        //    //Console.WriteLine("output>>" + (String.IsNullOrEmpty(output) ? "(none)" : output));
        //    //Console.WriteLine("error>>" + (String.IsNullOrEmpty(error) ? "(none)" : error));
        //    //Console.WriteLine("ExitCode: " + exitCode.ToString(), "ExecuteCommand");
        //    process.Close();
        //}

        //public static string ExecuteCommandSync(object command)
        //{
        //    try
        //    {
        //        // create the ProcessStartInfo using "cmd" as the program to be run,
        //        // and "/c " as the parameters.
        //        // Incidentally, /c tells cmd that we want it to execute the command that follows,
        //        // and then exit.
        //        System.Diagnostics.ProcessStartInfo procStartInfo =
        //            new System.Diagnostics.ProcessStartInfo("cmd", "/c " + command);

        //        // The following commands are needed to redirect the standard output.
        //        // This means that it will be redirected to the Process.StandardOutput StreamReader.
        //        procStartInfo.RedirectStandardOutput = true;
        //        procStartInfo.UseShellExecute = false;
        //        // Do not create the black window.
        //        procStartInfo.CreateNoWindow = true;
        //        // Now we create a process, assign its ProcessStartInfo and start it
        //        System.Diagnostics.Process proc = new System.Diagnostics.Process();
        //        proc.StartInfo = procStartInfo;
        //        proc.Start();
        //        // Get the output into a string

        //        string result = proc.StandardOutput.ReadToEnd();
        //        Utils.WriteLine(sPath, DateTime.Now.ToLongTimeString() + " Bat Output: " + result);
        //        // Display the command output.
        //        return result;
        //    }
        //    catch (Exception objException)
        //    {
        //        Utils.WriteLine(sPath, DateTime.Now.ToLongTimeString() + "  ----**ERROR(ExecuteCommandSync)**----" + objException.Message);
        //        throw objException;
        //    }
        //}

        private static void StartSLUpdateProcess()
        {
            string strQuery = string.Empty;
            OracleParameter[] values = null;
            DataTable _dtJobDetails; DataTable _dtTxData;
            string strTxData;
            string strLastProcessedObjectID = string.Empty;
            string strObjectID = string.Empty;
            Hashtable pHashMapping = new Hashtable();
            try
            {
                Utils.WriteLine(sPath, DateTime.Now.ToLongTimeString() + " -----------------Process Started for Task 4.2--------------------");
                CommandType cmdType = CommandType.Text;
                strQuery = "SELECT * FROM " + SLConfig.TB_AssignedTask + " Where " + SLConfig.FLD_TaskID_AssignedTask + "= '" + SLConfig.Value_UpdateSLTableTaskNumber + "'";
                _dtJobDetails = _oda_Webr.GetDataTableFromAdapter(strQuery, values, cmdType);
                if (_dtJobDetails != null && _dtJobDetails.Rows.Count > 0)
                {
                    for (int iCnt = 0; iCnt < _dtJobDetails.Rows.Count; iCnt++)
                    {
                        strJobId = _dtJobDetails.Rows[iCnt][SLConfig.FLD_JobID_AssignedTask].ToString();
                        strAssignedID = _dtJobDetails.Rows[iCnt]["ASSIGNID"].ToString();
                        Utils.WriteLine(sPath, DateTime.Now.ToLongTimeString() + " Processing Step 4.2 for JOB ID #: " + strJobId + "-------------");
                        strQuery = "SELECT COMMENTS FROM " + SLConfig.TB_Transaction + " Where " + SLConfig.FLD_JobID_Transaction + "= " + strJobId + " AND " + SLConfig.FLD_TaskId_Transaction + " = '" + SLConfig.Value_GISTaskNumber + "' AND RESULT = 'Y'";
                        _dtTxData = _oda_Webr.GetDataTableFromAdapter(strQuery, values, cmdType);
                        if (_dtTxData != null && _dtTxData.Rows.Count > 0)
                        {
                            strTxData = _dtTxData.Rows[0][0].ToString();
                            strObjectID = Utils.GetObjectIDFromTxData(strTxData);

                            pHashMapping = Utils.GetFieldMapping(pFeatClass_FieldPoint, pFeatClass_Stl, SLConfig.Value_UpdateSLTableTaskNumber);

                            int indexMapNo_Grid = pFeatClass_UnifiedGrid.Fields.FindField(SLConfig.strFLDMapNoGrid);
                            int indexMapNo_Stl = pFeatClass_Stl.Fields.FindField(SLConfig.strFLDMapNoStl);
                            int indexScale_Grid = pFeatClass_UnifiedGrid.Fields.FindField(SLConfig.strFLDScaleGrid);

                            if (!FetchFldPointsTable(strObjectID, pFeatClass_FieldPoint, pFeatClass_Stl, pFeatClass_UnifiedGrid, pHashMapping, indexMapNo_Grid, indexMapNo_Stl, indexScale_Grid))
                            {
                                Utils.WriteLine(sPath, DateTime.Now.ToLongTimeString() + "**SL Feature class not updated for Task 4.2**");
                                MakeDBEntries(false, SLConfig.Value_UpdateSLTableTaskNumber);
                                SendEmailNotification(false, SLConfig.Value_UpdateSLTableTaskNumber);
                            }
                            else
                            {
                                MakeDBEntries(true, SLConfig.Value_UpdateSLTableTaskNumber);
                                SendEmailNotification(true, SLConfig.Value_UpdateSLTableTaskNumber);
                            }
                        }
                    }
                }
                else
                {
                    Utils.WriteLine(sPath, DateTime.Now.ToLongTimeString() + " No JOB ID is available for SL update process");
                }
                Utils.WriteLine(sPath, DateTime.Now.ToLongTimeString() + " -----------------Process completed for Task 4.2--------------------");
            }
            catch (Exception ex)
            {
                Utils.WriteLine(sPath, DateTime.Now.ToLongTimeString() + "  ----**ERROR(StartSLUpdateProcess)**----" + ex.Message);
                MakeDBEntries(false, SLConfig.Value_UpdateSLTableTaskNumber);
                SendEmailNotification(false, SLConfig.Value_UpdateSLTableTaskNumber);
                throw ex;
            }
            finally
            {
                GC.Collect();
            }
        }

        private static bool FetchFldPointsTable(string strLastObjectID, IFeatureClass pFC_FieldPoints, IFeatureClass pFC_Stl, IFeatureClass pFC_UnifiedGrid, Hashtable pHashMapping, int indexMapNo_Grid, int indexMapNo_Stl, int indexScale_Grid)
        {
            IQueryFilter queryFilter = new QueryFilterClass();
            IFeatureCursor pFeatCur = null;
            IFeature pFeatureFldPoint = null;
            int intTotalFeatureToProcess = -1;
            bool boolSuccess = false;
            string strMapNo = string.Empty;
            List<string> lstMapNo = new List<string>();
            try
            {
                queryFilter.SubFields = "*";
                queryFilter.WhereClause = "objectid > " + strLastObjectID + " and TRANSACTION <>'8'";
                pFeatCur = pFC_FieldPoints.Search(queryFilter, false);
                intTotalFeatureToProcess = pFC_FieldPoints.FeatureCount(queryFilter);
                Utils.WriteLine(sPath, DateTime.Now.ToLongTimeString() + " Feture count found to insert in SL Feature Class are : " + intTotalFeatureToProcess);

                if (strReportPath == string.Empty)
                {
                    strReportPath = System.IO.Path.Combine(SLConfig.strJobfolderPath, strJobId);
                }
                if (System.IO.Directory.Exists(strReportPath))
                {
                    //strReportPath = System.IO.Path.Combine(strReportPath, "UnifiedGrids");
                    strUnifiedGridFilePath = strReportPath + "\\UnifiedGrid.txt";
                }
                while ((pFeatureFldPoint = pFeatCur.NextFeature()) != null)
                {
                    CreateFeatureInDestinationDB(pFeatureFldPoint, pFC_Stl, pHashMapping, pFC_UnifiedGrid, indexMapNo_Grid, indexMapNo_Stl, indexScale_Grid, out strMapNo);
                    if (!lstMapNo.Any(str => str.Contains(strMapNo)))
                    {
                        lstMapNo.Add(strMapNo);
                    }
                }
                Utils.WriteToFile(lstMapNo, strUnifiedGridFilePath);
                boolSuccess = true;
            }
            catch (Exception ex)
            {
                Utils.WriteLine(sPath, DateTime.Now.ToLongTimeString() + "  ----**ERROR(FetchFldPointsTable)**----" + ex.Message);
                throw ex;
            }
            finally
            {
                if (pFeatureFldPoint != null) Marshal.FinalReleaseComObject(pFeatureFldPoint);
                if (pFeatCur != null) Marshal.FinalReleaseComObject(pFeatCur);
                GC.Collect();
            }
            return boolSuccess;
        }

        private static void ReleaseObjects()
        {
            try
            {
                _oda.ReleaseDataObjects();
                _oda_Webr.ReleaseDataObjects();
                if (pFeatClass_UnifiedGrid != null) Marshal.FinalReleaseComObject(pFeatClass_UnifiedGrid);
                if (pFeatClass_Stl != null) Marshal.FinalReleaseComObject(pFeatClass_Stl);
                if (pFeatClass_FieldPoint != null) Marshal.FinalReleaseComObject(pFeatClass_FieldPoint);
                if (pMdbWorkspace != null) Marshal.FinalReleaseComObject(pMdbWorkspace);
                if (pFeatWS_Pgdb != null) Marshal.FinalReleaseComObject(pFeatWS_Pgdb);
                GC.Collect();
            }
            catch (Exception ex)
            {
                Utils.WriteLine(sPath, DateTime.Now.ToLongTimeString() + "  ----**ERROR(ReleaseObjects)**----" + ex.Message);
                throw ex;
            }
        }
    }
}
