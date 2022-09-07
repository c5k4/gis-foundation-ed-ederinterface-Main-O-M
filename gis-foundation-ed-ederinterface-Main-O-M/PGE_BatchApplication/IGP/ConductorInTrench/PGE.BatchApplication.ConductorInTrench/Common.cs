using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Oracle.DataAccess.Client;
using System.Data;
using ESRI.ArcGIS.esriSystem;
using Miner.Interop;
using System.Configuration;
using System.IO;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using System.Data.OleDb;
using System.Collections;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using System.Runtime.InteropServices;
using PGE_DBPasswordManagement;

namespace PGE.BatchApplication.ConductorInTrench
{
    public static class Common
    {
        public static Log4NetLogger _log = new Log4NetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, ReadConfigurations.LOGCONFIG);
        private static AoInitializeClass _mAoInit = null;
        public static IMMAppInitialize _mmAppInit = null;
        public static mmLicenseStatus _arcFMLicenseStatus;

        public static DataTable _dtChangeDetection_PriUG = null;
        public static DataTable _dtmanualPriUG = null;
        public static IList<string> _manualGlobalExisting = new List<string>();

        public static string _mailContent = null;

        /// <summary>
        /// Initialize the license required to read/edit ArcGIS components
        /// </summary>
        /// <returns></returns>
        public static void InitializeESRILicense()
        {
            try
            {
                //Cache product codes by enum int so can be sorted without custom sorter
                List<int> m_requestedProducts = new List<int>();
                foreach (esriLicenseProductCode code in new esriLicenseProductCode[] { esriLicenseProductCode.esriLicenseProductCodeAdvanced })
                {
                    int requestCodeNum = Convert.ToInt32(code);
                    if (!m_requestedProducts.Contains(requestCodeNum))
                    {
                        m_requestedProducts.Add(requestCodeNum);
                    }
                }

                ESRI.ArcGIS.RuntimeManager.Bind(ESRI.ArcGIS.ProductCode.Desktop);

                _mAoInit = new AoInitializeClass();
                esriLicenseProductCode currentProduct = new esriLicenseProductCode();
                foreach (int prodNumber in m_requestedProducts)
                {
                    esriLicenseProductCode prod = (esriLicenseProductCode)Enum.ToObject(typeof(esriLicenseProductCode), prodNumber);
                    esriLicenseStatus status = _mAoInit.IsProductCodeAvailable(prod);
                    if (status == esriLicenseStatus.esriLicenseAvailable)
                    {
                        status = _mAoInit.Initialize(prod);
                        if (status == esriLicenseStatus.esriLicenseAlreadyInitialized ||
                            status == esriLicenseStatus.esriLicenseCheckedOut)
                        {
                            currentProduct = _mAoInit.InitializedProduct();
                        }
                    }
                }

                Common._log.Info("ESRI License check out successfully.");
            }
            catch (Exception exp)
            {
                Common._log.Error("ESRI License check out failed.");
                Common._log.Error("Exception : " + exp.Message +" |  Stack Trace | " + exp.StackTrace+" at "+exp.ToString());
                throw exp;
            }
        }

        /// <summary>
        /// Initializes the license necessary to read/edit ArcFM components
        /// </summary>
        /// <returns></returns>
        public static void InitializeArcFMLicense()
        {
            try
            {
                //Comm.LogManager.WriteLine("Checking out ArcFM license...");
                _mmAppInit = new MMAppInitialize();
                // check and see if the type of license is available to check out
                _arcFMLicenseStatus = _mmAppInit.IsProductCodeAvailable(Miner.Interop.mmLicensedProductCode.mmLPArcFM);
                if (_arcFMLicenseStatus == mmLicenseStatus.mmLicenseAvailable)
                {
                    // if the license is available, try to check it out.
                    _arcFMLicenseStatus = _mmAppInit.Initialize(Miner.Interop.mmLicensedProductCode.mmLPArcFM);

                    if (_arcFMLicenseStatus != mmLicenseStatus.mmLicenseCheckedOut)
                    {
                        // if the license cannot be checked out, an exception is raised
                        Common._log.Error("Arc FM License check out failed.");
                        throw new Exception("The ArcFM license requested could not be checked out");
                    }
                    Common._log.Info("Arc FM License check out successfully.");
                }

            }
            catch (Exception exp)
            {
                Common._log.Error("Arc FM License check out failed.");
                Common._log.Error("Exception : " + exp.Message +" |  Stack Trace | " + exp.StackTrace+" at "+exp.ToString());
                throw exp;
            }
        }

        /// <summary>
        /// Closes the license object. The user will not be able to read/edit ArcGIS
        /// or ArcFM components after a call to this method
        /// </summary>
        /// <returns></returns>
        public static void CloseLicenseObject()
        {
            //ESRI License Initializer generated code.
            //Do not make any call to ArcObjects after ShutDownApplication()            
            try
            {
                if (_mmAppInit != null)
                {
                    _mmAppInit.Shutdown();
                }

                if (_mAoInit != null)
                {
                    _mAoInit.Shutdown();
                }
            }
            catch (Exception exp)
            {
                Common._log.Error("Exception : " + exp.Message +" |  Stack Trace | " + exp.StackTrace+" at "+exp.ToString());
            }
        }

        public static bool BulkCopyDataFromDataTableToDatabaseTable(DataTable dt, string tableName, string argstrConnectionstring)
        {
            OracleConnection conOraEDER = null;
            bool bCopySuccess = false;
            try
            {
                if (dt.Rows.Count > 0)
                {
                    using (conOraEDER = OpenOracleDBConnection(argstrConnectionstring))
                    {
                        using (OracleBulkCopy bulkCopy = new OracleBulkCopy(conOraEDER))
                        {
                            bulkCopy.DestinationTableName = tableName;
                            bulkCopy.BulkCopyTimeout = 300;
                            foreach (DataColumn column in dt.Columns)
                            {
                                bulkCopy.ColumnMappings.Add(new OracleBulkCopyColumnMapping(column.ColumnName, column.ColumnName));
                            }
                            bulkCopy.WriteToServer(dt);
                            bCopySuccess = true;
                            Common._log.Info("Bulk copying data to " + tableName + " is successfull and total row count transferred :" + dt.Rows.Count);
                        }
                    }
                }
                else
                {
                    Common._log.Info("No record found for bulk copying data to " + tableName + ".");
                }
            }
            catch (Exception exp)
            {
                bCopySuccess = false;
                _log.Error("Exception encountered while BulkCopy the Data in Table " + tableName, exp);
                Common._log.Error("Exception : " + exp.Message +" |  Stack Trace | " + exp.StackTrace+" at "+exp.ToString());
                ErrorCodeException ece = new ErrorCodeException(exp);
                Environment.ExitCode = ece.CodeNumber;
            }
            finally
            {
                DBHelper.CloseConnection(conOraEDER);
            }
            return bCopySuccess;
        }

        public static bool BulkCopyDataFromDataTableToDatabaseTable_PGEDATA(DataTable dt, string tableName)
        {
            bool update_value = false;
            OracleConnection conOraEDER = null;
            try
            {
                using (conOraEDER = OpenOracleDBConnection_PGEDATA())
                {
                    using (OracleBulkCopy bulkCopy = new OracleBulkCopy(conOraEDER))
                    {
                        bulkCopy.DestinationTableName = tableName;
                        bulkCopy.BulkCopyTimeout = 300;
                        foreach (DataColumn column in dt.Columns)
                        {
                            bulkCopy.ColumnMappings.Add(new OracleBulkCopyColumnMapping(column.ColumnName, column.ColumnName));
                        }
                        bulkCopy.WriteToServer(dt);
                        update_value = true;

                        Common._log.Info("Bulk copying data to " + tableName + " is successfull.");
                    }
                }
            }
            catch (Exception exp)
            {
                _log.Error("Exception encountered while BulkCopy the Data in Table " + tableName, exp);
                Common._log.Error("Exception : " + exp.Message +" |  Stack Trace | " + exp.StackTrace+" at "+exp.ToString());
                Common._log.Error("Bulk copying data to " + tableName + " is failed.");
                ErrorCodeException ece = new ErrorCodeException(exp);
                Environment.ExitCode = ece.CodeNumber;
            }
            finally
            {
                DBHelper.CloseConnection(conOraEDER);
            }
            return update_value;
        }

        public static OracleConnection OpenOracleDBConnection()
        {
            OracleConnection objconOra = null;
            try
            {
                string[] oracleConnectionInfo = ConfigurationManager.AppSettings["EDER_ConnectionString"].Split(',');
                string oracleConnectionString = "Data Source=" + oracleConnectionInfo[1] + ";User Id=" + oracleConnectionInfo[2] + ";password=" + oracleConnectionInfo[3];
                objconOra = new OracleConnection();
                objconOra.ConnectionString = oracleConnectionString;
                objconOra.Open();
            }
            catch (Exception exp)
            {
                _log.Error("Exception encountered while Open the Oracle Connection", exp);
                Common._log.Error("Exception : " + exp.Message +" |  Stack Trace | " + exp.StackTrace+" at "+exp.ToString());
                ErrorCodeException ece = new ErrorCodeException(exp);
                Environment.ExitCode = ece.CodeNumber;
            }
            return objconOra;
        }

        public static OracleConnection OpenOracleDBConnection_PGEDATA()
        {
            OracleConnection objconOra = null;
            try
            {
                // string[] oracleConnectionInfo = ReadConfigurations.ConnectionString_pgedata.Split(',');
                //string oracleConnectionString = "Data Source=" + oracleConnectionInfo[1] + ";User Id=" + oracleConnectionInfo[2] + ";password=" + oracleConnectionInfo[3];
                string oracleConnectionString = ReadConfigurations.ConnectionString_pgedata;
                objconOra = new OracleConnection();
                objconOra.ConnectionString = oracleConnectionString;
                objconOra.Open();
            }
            catch (Exception exp)
            {
                _log.Error("Exception encountered while Open the Oracle Connection", exp);
                Common._log.Error("Exception : " + exp.Message +" |  Stack Trace | " + exp.StackTrace+" at "+exp.ToString());
                ErrorCodeException ece = new ErrorCodeException(exp);
                Environment.ExitCode = ece.CodeNumber;
            }
            return objconOra;
        }

        public static OracleConnection OpenOracleDBConnection(string argstrConnectionstring)
        {
            OracleConnection objconOra = null;
            try
            {
                string[] oracleConnectionInfo = argstrConnectionstring.Split(',');
                objconOra = new OracleConnection();
                if (oracleConnectionInfo.Length > 1)
                {
                    string oracleConnectionString = "Data Source=" + oracleConnectionInfo[1] + ";User Id=" + oracleConnectionInfo[2] + ";password=" + oracleConnectionInfo[3];
                    objconOra.ConnectionString = oracleConnectionString;
                }
                else
                {
                    objconOra.ConnectionString = argstrConnectionstring;
                }
                objconOra.Open();
            }
            catch (Exception exp)
            {
                _log.Error("Exception encountered while Open the Oracle Connection", exp);
                Common._log.Error("Exception : " + exp.Message +" |  Stack Trace | " + exp.StackTrace+" at "+exp.ToString());
                ErrorCodeException ece = new ErrorCodeException(exp);
                Environment.ExitCode = ece.CodeNumber;
            }
            return objconOra;
        }      

        public static bool UpdatePriUGConductorDataInVersion(IFeatureClass PriUGClass, DataTable argdtAllDataToUpdate)
        {
            bool bProcessSuccess = true;
            IQueryFilter pqueryfilter = null;
            IFeatureCursor pfeatcursor = null;
            IFeature pfeature = null;
            string strCITUpdatedOn = null;
            string strUser = null;
            string sGLOBALID = null;
            string FillDuctValue = null;
            string strExclusion = null;
            string strStatusValue = null;
            string strOID = null;
            List<int> _lstChangedConductorsUpdated = null;
            string strLastUser = null;
            try
            {
                string oracleConnectionString = ReadConfigurations.ConnectionString_pgedata;
                string tblMergeTable = ConfigurationManager.AppSettings["tableNameToSaveFinalData"];
                string strQuery = "select * from " + tblMergeTable + " where " + ReadConfigurations.col_STATUS + "='New'";
                argdtAllDataToUpdate = DBHelper.GetDataTable(oracleConnectionString, strQuery);
                if (argdtAllDataToUpdate == null)
                {
                    Common._log.Error("Error while fetching the data table from which the data will be updated to Primary UG features.");
                    return false;
                }

                strLastUser = ConfigurationManager.AppSettings["SMUser"];

                foreach (DataRow row_PriUG in argdtAllDataToUpdate.Rows)
                {
                    try
                    {
                        sGLOBALID = Convert.ToString(row_PriUG[ReadConfigurations.col_GLOBALID]);
                        strOID = Convert.ToString(row_PriUG[ReadConfigurations.col_OBJECTID]);

                        if (row_PriUG.IsNull(ReadConfigurations.col_FILLEDDUCT))
                        {
                            FillDuctValue = null;
                        }
                        else
                        {
                            FillDuctValue = Convert.ToString(row_PriUG[ReadConfigurations.col_FILLEDDUCT]);
                        }

                        pqueryfilter = new QueryFilterClass();
                        pqueryfilter.WhereClause = ReadConfigurations.col_GLOBALID + " = '" + sGLOBALID + "'";
                        pfeatcursor = PriUGClass.Update(pqueryfilter, false);
                        pfeature = pfeatcursor.NextFeature();
                        if (pfeature != null)
                        {
                            try
                            {
                                strUser = Convert.ToString(row_PriUG[ReadConfigurations.col_USER]);
                                strCITUpdatedOn = Convert.ToString(pfeature.get_Value(pfeature.Fields.FindField(ReadConfigurations.col_CIT_UPDATEDON)));

                                if (strUser == ReadConfigurations.val_SYSTEM)
                                {
                                    if (!string.IsNullOrEmpty(strCITUpdatedOn))
                                    {
                                        Common._log.Info("PriUGConductor Feature GLOBALID: " + sGLOBALID + ", OBJECTID=" + strOID + " is already updated manually so not updating the filledduct value now.");
                                    }
                                    else
                                    {
                                        pfeature.set_Value(pfeature.Fields.FindField(ReadConfigurations.col_FILLEDDUCT), FillDuctValue);
                                        pfeature.set_Value(pfeature.Fields.FindField(ReadConfigurations.col_DATEMODIFIED), DateTime.Now);
                                        pfeature.set_Value(pfeature.Fields.FindField(ReadConfigurations.col_LASTUSER), strLastUser);
                                        pfeature.Store();

                                        // Must be called only for running process for changes at release 
                                        if (MainClass._bisRunningProcessForChanges)
                                        {
                                            if (_lstChangedConductorsUpdated == null)
                                            {
                                                _lstChangedConductorsUpdated = new List<int>();
                                            }

                                            _lstChangedConductorsUpdated.Add(pfeature.OID);
                                        }

                                        Common._log.Info("PriUGConductor Feature GLOBALID: " + sGLOBALID + ", OBJECTID=" + strOID + " , Filledduct=" + FillDuctValue + " This record is updated from auto updates");

                                        // Here Call a function to update all required conductors 
                                        if (!row_PriUG.IsNull(ReadConfigurations.col_OIDS_Needed))
                                        {
                                            strExclusion = Convert.ToString(row_PriUG[ReadConfigurations.col_Exclusion]);

                                            if (strExclusion == ReadConfigurations.strExclusionBasedOnStatus)
                                            {
                                                Common._log.Info("PriUGConductor Feature GLOBALID: " + sGLOBALID + ", OBJECTID=" + strOID + ", No need to update required records because of " + strExclusion);
                                            }
                                            else
                                            {
                                                UpdateAllRequiredConductors(row_PriUG, pfeature.OID, argdtAllDataToUpdate, PriUGClass, ref _lstChangedConductorsUpdated);
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    strStatusValue = Convert.ToString(pfeature.get_Value(pfeature.Fields.FindField(ReadConfigurations.col_STATUS)));

                                    if (strStatusValue != ReadConfigurations.val_StatusInService)
                                    {
                                        Common._log.Info("PiUGConductor Feature GLOBALID: " + sGLOBALID + ", OBJECTID=" + strOID + " , Filledduct=" + FillDuctValue + " This record will not be updated(manual upadates) because it's status =" + strStatusValue);
                                    }
                                    else
                                    {
                                        pfeature.set_Value(pfeature.Fields.FindField(ReadConfigurations.col_FILLEDDUCT), FillDuctValue);
                                        pfeature.set_Value(pfeature.Fields.FindField(ReadConfigurations.col_CIT_UPDATEDON), DateTime.Now);
                                        pfeature.set_Value(pfeature.Fields.FindField(ReadConfigurations.col_DATEMODIFIED), DateTime.Now);
                                        pfeature.set_Value(pfeature.Fields.FindField(ReadConfigurations.col_LASTUSER), strLastUser);
                                        pfeature.Store();

                                        Common._log.Info("PriUGConductor Feature GLOBALID: " + sGLOBALID + ", OBJECTID=" + strOID + " , Filledduct=" + FillDuctValue + " This record is updated from manual updates");
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                bProcessSuccess = false;
                                Common._log.Error(" PriUGConductor Feature not updated for GLOBALID: " + sGLOBALID + ", OBJECTID=" + strOID + ",  " + ex.Message + " |  Stack Trace | " + ex.StackTrace);
                            }
                        }
                    }
                    catch (Exception exp)
                    {
                        bProcessSuccess = false;
                        Common._log.Error("Exception : " + exp.Message +" |  Stack Trace | " + exp.StackTrace+" at "+exp.ToString());
                    }
                    finally
                    {
                        if (pfeatcursor != null)
                        {
                            Marshal.FinalReleaseComObject(pfeatcursor);
                            pfeatcursor = null;
                        }
                        if (pqueryfilter != null)
                        {
                            Marshal.FinalReleaseComObject(pqueryfilter);
                            pqueryfilter = null;
                        }
                    }
                }

                if (_lstChangedConductorsUpdated != null && _lstChangedConductorsUpdated.Count > 0)
                {
                    UpdateStageTableForChangedConductor(_lstChangedConductorsUpdated);
                }
            }
            catch (Exception exp)
            {
                Common._log.Error("Exception : " + exp.Message + " |  Stack Trace | " + exp.StackTrace + " at " + exp.ToString());
                bProcessSuccess = false;
                ErrorCodeException ece = new ErrorCodeException(exp);
                Environment.ExitCode = ece.CodeNumber;
            }
            finally
            {
                if (_lstChangedConductorsUpdated != null)
                {
                    _lstChangedConductorsUpdated.Clear();
                    _lstChangedConductorsUpdated = null;
                }
            }
            return bProcessSuccess;
        }

        public static DataTable UpdatePriUGConductorDataForChangesInCommonTable(IFeatureClass argFeatureClassPriUG, ref DataTable argdtAllDataToUpdate)
        {           
            string sGLOBALID = null;
            string FillDuctValue = null;     
            string strOID = null;
            string conduitType = null;
            DataTable dtChangedRecords = null;
            DataRow pRow = null;
            IFeature pFeature = null;
            try
            {
                dtChangedRecords = argdtAllDataToUpdate.Clone();

                foreach (DataRow row_PriUG in argdtAllDataToUpdate.Rows)
                {
                    try
                    {
                        sGLOBALID = Convert.ToString(row_PriUG[ReadConfigurations.col_GLOBALID]);
                        strOID = Convert.ToString(row_PriUG[ReadConfigurations.col_OBJECTID]);
                        FillDuctValue = Convert.ToString(row_PriUG[ReadConfigurations.col_FILLEDDUCT]);
                        //strCounty = Convert.ToString(row_PriUG[ReadConfigurations.col_COUNTY]);

                        dtChangedRecords.Rows.Add(row_PriUG.ItemArray);

                        // Here Call a function to update all required conductors 
                        if (!row_PriUG.IsNull(ReadConfigurations.col_OIDS_Needed))
                        {
                            string allOIDs = Convert.ToString(row_PriUG[ReadConfigurations.col_OIDS_Needed]);

                            string[] allOIDsArray = allOIDs.Split(',');
                            foreach (string givenOID in allOIDsArray)
                            {
                                // Check here that it is not same as given conductor
                                if (strOID != givenOID)
                                {
                                    //Check here that the same is not present in table for manual update or auto update , don't need to update here if already present in table
                                    DataRow[] pRows = argdtAllDataToUpdate.Select(ReadConfigurations.col_OBJECTID + "='" + givenOID + "'");
                                    if (pRows.Length == 0)
                                    {
                                        if (!string.IsNullOrEmpty(FillDuctValue))
                                        {
                                            pFeature = argFeatureClassPriUG.GetFeature(Convert.ToInt32(givenOID));
                                            //Add this conductor details in datatable , objectID , globalid and filledduct
                                            pRow = dtChangedRecords.Rows.Add();
                                            pRow[ReadConfigurations.col_OBJECTID] = givenOID;
                                            pRow[ReadConfigurations.col_FILLEDDUCT] = FillDuctValue;
                                            pRow[ReadConfigurations.col_GLOBALID] = Convert.ToString(pFeature.get_Value(pFeature.Fields.FindField(ReadConfigurations.col_GLOBALID)));
                                            pRow[ReadConfigurations.col_CIRCUITID] = Convert.ToString(pFeature.get_Value(pFeature.Fields.FindField(ReadConfigurations.col_CIRCUITID)));
                                            pRow[ReadConfigurations.col_COUNTY] = Convert.ToString(pFeature.get_Value(pFeature.Fields.FindField(ReadConfigurations.col_COUNTY)));
                                            pRow[ReadConfigurations.col_SUBTYPECD] = Convert.ToString(pFeature.get_Value(pFeature.Fields.FindField(ReadConfigurations.col_SUBTYPECD)));
                                            conduitType = MainClass.ProcessPriUGCond_GetTypeOfConduit(pFeature);
                                            pRow[ReadConfigurations.col_CONDUITTYPE] = conduitType;
                                        }
                                        else
                                        {
                                            if (MainClass._bLogNeededWhileExecuting)
                                                Common._log.Info("PriUGConductor Feature OBJECTID: " + givenOID + " , Filledduct is Null so not storing in database.");
                                        }
                                    }                                    
                                }                               
                            }                          
                        }                        
                    }
                    catch (Exception exp)
                    {                      
                        Common._log.Error("Exception : " + exp.Message +" |  Stack Trace | " + exp.StackTrace+" at "+exp.ToString());
                    }
                }
            }
            catch (Exception exp)
            {
                Common._log.Error("Exception : " + exp.Message + " |  Stack Trace | " + exp.StackTrace + " at " + exp.ToString());
            }
            return dtChangedRecords;
        }

        private static bool UpdateAllRequiredConductors(DataRow argpRow, int argiMainFeatureOID, DataTable argdtAllDataToUpdate, IFeatureClass argPriUGFeatureClass, ref List<int> _arglstChangedConductorsUpdated)
        {
            bool bProcessSuccess = false;
            IQueryFilter pqueryfilter = null;
            IFeatureCursor pfeatcursor = null;
            IFeature pfeature = null;
            string strCITUpdatedOn = null;
            string strStatusValue = null;
            string strLastUser = null;
            try
            {
                strLastUser = ConfigurationManager.AppSettings["SMUser"];

                string allOIDs = Convert.ToString(argpRow[ReadConfigurations.col_OIDS_Needed]);

                string[] allOIDsArray = allOIDs.Split(',');
                foreach (string givenOID in allOIDsArray)
                {
                    // Check here that it is not same as given conductor
                    if (argiMainFeatureOID + "" != givenOID)
                    {
                        //Check here that the same is not present in table for manual update or auto update , don't need to update here if already present in table
                        DataRow[] pRows = argdtAllDataToUpdate.Select(ReadConfigurations.col_OBJECTID + "='" + givenOID + "'");
                        if (pRows.Length == 0)
                        {
                            pqueryfilter = new QueryFilterClass();
                            pqueryfilter.WhereClause = ReadConfigurations.col_OBJECTID + "=" + givenOID + "";
                            pfeatcursor = argPriUGFeatureClass.Update(pqueryfilter, false);
                            pfeature = pfeatcursor.NextFeature();
                            if (pfeature != null)
                            {
                                try
                                {
                                    strCITUpdatedOn = Convert.ToString(pfeature.get_Value(pfeature.Fields.FindField(ReadConfigurations.col_CIT_UPDATEDON)));

                                    //Before update check CITUpdatedOn also   
                                    if (!string.IsNullOrEmpty(strCITUpdatedOn))
                                    {
                                        Common._log.Info("PriUGConductor Feature OBJECTID: " + givenOID + " is already updated manually so not updating the filledduct value now.");
                                    }
                                    else
                                    {
                                        string FillDuctValue = Convert.ToString(argpRow[ReadConfigurations.col_FILLEDDUCT]);
                                        if (!string.IsNullOrEmpty(FillDuctValue))
                                        {
                                            strStatusValue = Convert.ToString(pfeature.get_Value(pfeature.Fields.FindField(ReadConfigurations.col_STATUS)));

                                            if (strStatusValue != ReadConfigurations.val_StatusInService)
                                            {
                                                Common._log.Info("PiUGConductor Feature OBJECTID: " + givenOID + " , Filledduct=" + FillDuctValue + " This record will not be updated(auto upadates) because it's status =" + strStatusValue);
                                            }
                                            else
                                            {
                                                pfeature.set_Value(pfeature.Fields.FindField(ReadConfigurations.col_FILLEDDUCT), FillDuctValue);
                                                pfeature.set_Value(pfeature.Fields.FindField(ReadConfigurations.col_DATEMODIFIED), DateTime.Now);
                                                pfeature.set_Value(pfeature.Fields.FindField(ReadConfigurations.col_LASTUSER), strLastUser);
                                                pfeature.Store();

                                                // Must be called only for running process for changes at release
                                                if (MainClass._bisRunningProcessForChanges)
                                                {
                                                    _arglstChangedConductorsUpdated.Add(pfeature.OID);
                                                }

                                                if (MainClass._bLogNeededWhileExecuting)
                                                    Common._log.Info("PriUGConductor Feature OBJECTID: " + givenOID + " , Filledduct=" + FillDuctValue + " This record is updated from auto updates in required OID logic.");
                                            }
                                        }
                                        else
                                        {
                                            if (MainClass._bLogNeededWhileExecuting)
                                                Common._log.Info("PriUGConductor Feature OBJECTID: " + givenOID + " , Filledduct is Null so not storing in database.");
                                        }
                                    }
                                }
                                catch (Exception exp)
                                {
                                    bProcessSuccess = false;
                                    Common._log.Error(" PriUGConductor Feature not updated for OBJECTID: " + givenOID + exp.Message + " |  Stack Trace | " + exp.StackTrace+" at "+exp.ToString());
                                    Common._log.Error("Exception : " + exp.Message + " |  Stack Trace | " + exp.StackTrace + " at " + exp.ToString());
                                }
                            }
                        }
                        else
                        {
                            if (MainClass._bLogNeededWhileExecuting)
                                Common._log.Info("PriUGConductor Feature OBJECTID: " + givenOID + " , is present in current table to update so not updating from required OID update logic.");
                        }
                    }
                }
                bProcessSuccess = true;
            }
            catch (Exception exp)
            {
                bProcessSuccess = false;
                Common._log.Error("Exception : " + exp.Message +" |  Stack Trace | " + exp.StackTrace+" at "+exp.ToString());
            }
            finally
            {
                if (pfeatcursor != null)
                {
                    Marshal.FinalReleaseComObject(pfeatcursor);
                    pfeatcursor = null;
                }
                if (pqueryfilter != null)
                {
                    Marshal.FinalReleaseComObject(pqueryfilter);
                    pqueryfilter = null;
                }
            }
            return bProcessSuccess;
        }

        public static DataTable GetManuallyChangedPrimaryUGConductors(string strSqlQuery, IFeatureClass argPriUGFeatureClass)
        {
            int primaryUGOID = 0;
            string primaryUGGUID = string.Empty;
            string filledDuct = string.Empty;
            string status = string.Empty;
            string processedOn = string.Empty;
            DataTable dtManuallyChangedPriUG = null;
            int featureCount = 0;
            IQueryFilter pQueryFilter = null;
            IFeatureCursor pFeatureCursor = null;
            IFeature pFeature = null;
            int iCount = 0;
            try
            {
                // m4jf edgisrarch 919
               // string strConnString = ConfigurationManager.AppSettings["ConnectionString_wip"];
                string strConnString =  ReadEncryption.GetConnectionStr(ConfigurationManager.AppSettings["WIP_ConnectionStr_WEBR"].ToUpper());
                dtManuallyChangedPriUG = DBHelper.GetDataTable(strConnString, strSqlQuery);

                if (dtManuallyChangedPriUG != null)
                {
                    Common._log.Info("Total features got from table CIT_MANUAL_UPDATE : " + dtManuallyChangedPriUG.Rows.Count);
                }
                else
                {
                    Common._log.Info("Total features got from table CIT_MANUAL_UPDATE : 0");
                }

                pQueryFilter = new QueryFilterClass();
                pQueryFilter.WhereClause = ReadConfigurations.col_CIT_UPDATEDON + " IS NOT NULL";
                //featureCount = argPriUGFeatureClass.FeatureCount(pQueryFilter);              

                pFeatureCursor = argPriUGFeatureClass.Search(pQueryFilter, false);
                pFeature = pFeatureCursor.NextFeature();
                while (pFeature != null)
                {
                    try
                    {
                        primaryUGOID = pFeature.OID;
                        primaryUGGUID = Convert.ToString(pFeature.get_Value(pFeature.Fields.FindField(ReadConfigurations.col_GLOBALID)));
                        filledDuct = Convert.ToString(pFeature.get_Value(pFeature.Fields.FindField(ReadConfigurations.col_FILLEDDUCT)));
                        status = "Completed";
                        processedOn = Convert.ToString(pFeature.get_Value(pFeature.Fields.FindField(ReadConfigurations.col_CIT_UPDATEDON)));

                        if (!string.IsNullOrEmpty(filledDuct))
                        {
                            dtManuallyChangedPriUG.Rows.Add(primaryUGOID, primaryUGGUID, filledDuct, status, null, null, processedOn);
                            iCount++;
                        }
                        else
                        {
                            Common._log.Info("Filledduct value for OID : " + primaryUGOID + " is either Null or empty in Pri UG feature class : " + featureCount);
                        }

                    }
                    catch (Exception exp)
                    {
                        Common._log.Error("Exception : " + exp.Message +" |  Stack Trace | " + exp.StackTrace+" at "+exp.ToString());
                    }
                    pFeature = pFeatureCursor.NextFeature();
                }

                featureCount = iCount;
                Common._log.Info("Total features manually updated in Pri UG feature class : " + featureCount);
            }
            catch (Exception exp)
            {
                Common._log.Error("Exception : " + exp.Message + " |  Stack Trace | " + exp.StackTrace + " at " + exp.ToString());
                ErrorCodeException ece = new ErrorCodeException(exp);
                Environment.ExitCode = ece.CodeNumber;
            }
            finally
            {
                if (pFeatureCursor != null)
                {
                    Marshal.FinalReleaseComObject(pFeatureCursor);
                    pFeatureCursor = null;
                }

                if (pQueryFilter != null)
                {
                    Marshal.FinalReleaseComObject(pQueryFilter);
                    pQueryFilter = null;
                }
            }
            return dtManuallyChangedPriUG;
        }

        public static string checking_FilledDuctChange(string globalid, IFeatureClass pQueryFeatureclass)
        {
            string filledDuct_value = string.Empty;
            IQueryFilter pqueryfilter = null;
            IFeatureCursor pFeatCursor = null;
            IFeature pSearchFeature = null;
            try
            {
                pqueryfilter = new QueryFilterClass();
                pqueryfilter.WhereClause = ReadConfigurations.col_GLOBALID + "= '" + globalid + "'";

                pFeatCursor = pQueryFeatureclass.Search(pqueryfilter, false);
                pSearchFeature = pFeatCursor.NextFeature();
                if (pSearchFeature != null)
                {
                    filledDuct_value = Convert.ToString(pSearchFeature.get_Value(pSearchFeature.Fields.FindField(ReadConfigurations.col_FILLEDDUCT)));
                }
                else
                {
                    Common._log.Error("Feature could not be found in the " + pQueryFeatureclass.AliasName.ToString() + "  :  GLOBALID : " + globalid);
                }
            }
            catch (Exception exp)
            {
                Common._log.Error("Error while fetching data to update from database table." + exp.Message + " |Stack Trace | " + exp.StackTrace);
                Common._log.Error("Exception : " + exp.Message +" |  Stack Trace | " + exp.StackTrace+" at "+exp.ToString());
            }
            finally
            {
                if (pFeatCursor != null)
                {
                    Marshal.ReleaseComObject(pFeatCursor);
                    pFeatCursor = null;
                }
                if (pqueryfilter != null)
                {
                    Marshal.ReleaseComObject(pqueryfilter);
                    pqueryfilter = null;
                }
            }
            return filledDuct_value;
        }

        public static int comparisonCode(DataTable auto_update)
        {
            string manual_PriUG = string.Empty;
            string globalid_manual = string.Empty;
            int iCount_ToBeUpdated = 0;
            string entryInList = null;
            DataRow[] pRows = null;
            string globalid_auto = null;
            string filledductOldValue = string.Empty;
            string filledductNewValue = string.Empty;
            try
            {
                //ACTION ITEM - Comment below line if un commented
                //manual_Global_Existing.Clear();
                //From design document--
                //1.The process will never overwrite the manual updates, however if it will be found that a manually updated FILLEDDUCT value needs to be updated then a notification mail will be sent to the appropriate user group.
                //2.Mail will be sent to mailer group listing all the primary UG conductors identified by scheduled job for update but has been updated manually previously with different values.

                for (int count = 0; count < auto_update.Rows.Count; count++)
                {
                    try
                    {
                        globalid_auto = auto_update.Rows[count][ReadConfigurations.col_GLOBALID].ToString();
                        pRows = _dtmanualPriUG.Select(ReadConfigurations.col_GLOBALID + "='" + globalid_auto + "'", "requested_on desc");

                        if (pRows.Length > 0)
                        {
                            filledductOldValue = Convert.ToString(pRows[0][ReadConfigurations.col_FILLEDDUCT]);
                            filledductNewValue = Convert.ToString(auto_update.Rows[count][ReadConfigurations.col_FILLEDDUCT]);

                            if ((filledductNewValue == "0" && string.IsNullOrEmpty(filledductOldValue)) || (string.IsNullOrEmpty(filledductNewValue) && filledductOldValue == "0") || (string.IsNullOrEmpty(filledductNewValue) && string.IsNullOrEmpty(filledductOldValue)) || (filledductNewValue == "0" && filledductOldValue == "0"))
                            {
                                Common._log.Info("Not capturing the details in mail for GUID: " + globalid_auto + " because the filledduct value did not change.");
                            }
                            else if (filledductOldValue != filledductNewValue)
                            {
                                //"GLOBALID  |  OBJECTID  |  FILLEDDUCT VALUE MANUALLY UPDATED  |  NEW FILLEDDUCT VALUE CAPTURED BY PROGRAM"
                                entryInList = " " + globalid_auto + " | " + auto_update.Rows[count][ReadConfigurations.col_OBJECTID] + " | " + pRows[0][ReadConfigurations.col_FILLEDDUCT] + " | " + auto_update.Rows[count][ReadConfigurations.col_FILLEDDUCT];
                                if (!_manualGlobalExisting.Contains(entryInList))
                                {
                                    _manualGlobalExisting.Add(entryInList);
                                    Common._log.Info("Program will not update filledduct for objectid=" + auto_update.Rows[count][ReadConfigurations.col_OBJECTID] + " because this value is already updated manually through WEBR.");
                                }

                                auto_update.Rows[count][ReadConfigurations.col_STATUS] = "Conflict";
                                auto_update.AcceptChanges();
                            }
                            else
                            {
                                Common._log.Info("Not capturing the details in mail for GUID: " + globalid_auto + " because the filledduct value did not change.");
                            }
                        }
                        else
                        {
                            auto_update.Rows[count][ReadConfigurations.col_STATUS] = ReadConfigurations.val_New;
                            iCount_ToBeUpdated++;
                        }
                    }
                    catch (Exception exp)
                    {
                        Common._log.Error("Exception : " + exp.Message +" |  Stack Trace | " + exp.StackTrace+" at "+exp.ToString());
                    }
                }
            }
            catch (Exception exp)
            {
                Common._log.Error("Exception : " + exp.Message + " |  Stack Trace | " + exp.StackTrace + " at " + exp.ToString());
            }
            return iCount_ToBeUpdated;
        }
            
        public static void buffer_ChangeDetection_PriUG(int objectid, IFeatureClass featclass_PriUG, IWorkspace workspace, ref DataTable argdtConductors, bool agrbProcessRunningForChanges)
        {
            IQueryFilter pQueryFilter = null;
            int iFeatureCount = 0;
            IFeature pFeaturePriUG = null;
            IFeature pFeatureOutPut = null;
            ISpatialFilter spatialFilter = null;
            ITopologicalOperator topoOperator = null;
            IFeatureCursor spfeatureCursor = null;
            string strTypeOfConduit = null;
            try
            {
                Common._log.Info("The reference primary underground conductor is: " + objectid);
                
                pQueryFilter = new QueryFilterClass();
                pQueryFilter.WhereClause = "OBJECTID="+objectid;
                iFeatureCount = featclass_PriUG.FeatureCount(pQueryFilter);

                if (iFeatureCount == 0)
                {
                    Common._log.Info("No feature found in database from reference primary underground conductor OID: " + objectid);
                    return;
                }

                pFeaturePriUG = featclass_PriUG.GetFeature(objectid);

                try
                {
                    if (pFeaturePriUG != null)
                    {
                        AddFeatureToDataTable(pFeaturePriUG, argdtConductors, agrbProcessRunningForChanges);

                        //Check here the conduit Subtype                            
                        strTypeOfConduit = MainClass.GetTypeOfConduit(pFeaturePriUG, false, false);

                        // Only these conduit 
                        if (strTypeOfConduit == "0" || strTypeOfConduit == "2" || strTypeOfConduit == "3")
                        {
                            spatialFilter = new SpatialFilterClass();

                            spatialFilter.SubFields = ReadConfigurations.subFields;

                            topoOperator = (ITopologicalOperator)pFeaturePriUG.Shape;
                            //Keeping the biffer distance =12 Feet to capture all changed conductors
                            spatialFilter.Geometry = topoOperator.Buffer(MainClass._searchDistance * 2);
                            spatialFilter.GeometryField = featclass_PriUG.ShapeFieldName;
                            spatialFilter.SpatialRel = esriSpatialRelEnum.esriSpatialRelIntersects;

                            spfeatureCursor = featclass_PriUG.Search(spatialFilter, false);
                            pFeatureOutPut = spfeatureCursor.NextFeature();

                            while (pFeatureOutPut != null)
                            {
                                try
                                {
                                    Common._log.Info("The ObjectID of the primary conductor present in the buffer distance: " + pFeatureOutPut.OID);

                                    //Check here the conduit Subtype                            
                                    strTypeOfConduit = MainClass.GetTypeOfConduit(pFeatureOutPut, false, true);

                                    // Only these conduit 
                                    if (strTypeOfConduit == "0" || strTypeOfConduit == "2" || strTypeOfConduit == "3")
                                    {
                                        AddFeatureToDataTable(pFeatureOutPut, argdtConductors, agrbProcessRunningForChanges);
                                    }
                                }
                                catch (Exception exp)
                                {
                                    Common._log.Error("Error in Function buffer_ChangeDetection_PriUG()" + exp.Message + " |  Stack Trace | " + exp.StackTrace + " at " + exp.ToString());
                                }

                                pFeatureOutPut = spfeatureCursor.NextFeature();
                            }
                        }
                        else
                        {
                            if (MainClass._bLogNeededWhileExecuting)
                                Common._log.Info("The primary UG conductor conduit subtype is  " + strTypeOfConduit + " so not taking the nearby conductors after buffer.");
                        }
                    }
                    else
                    {
                        Common._log.Info("The primary underground conductor with OID : " + objectid + " not found in database");
                    }
                }
                catch (Exception exp)
                {
                    Common._log.Error("Error in Function buffer_ChangeDetection_PriUG()" + exp.Message + " |  Stack Trace | " + exp.StackTrace + " at " + exp.ToString());
                }
                finally
                {
                    if (pQueryFilter != null && Marshal.IsComObject(pQueryFilter))
                    {
                        Marshal.FinalReleaseComObject(pQueryFilter);
                        pQueryFilter = null;
                    }
                    
                    if (spfeatureCursor != null && Marshal.IsComObject(spfeatureCursor))
                    {
                        Marshal.FinalReleaseComObject(spfeatureCursor);
                        spfeatureCursor = null;
                    }

                    if (topoOperator != null && Marshal.IsComObject(topoOperator))
                    {
                        Marshal.FinalReleaseComObject(topoOperator);
                        topoOperator = null;
                    }

                    if (spatialFilter != null && Marshal.IsComObject(spatialFilter))
                    {
                        Marshal.FinalReleaseComObject(spatialFilter);
                        spatialFilter = null;
                    }
                }
            }
            catch (Exception exp)
            {
                Common._log.Error("Error in Function buffer_ChangeDetection_PriUG(). Exception: " + exp.Message + " |  Stack Trace | " + exp.StackTrace + " at " + exp.ToString());
            }
        }

        private static void AddFeatureToDataTable(IFeature argpFeature, DataTable argdtRecords, bool agrbProcessRunningForChanges)
        {
            string globalid = null;
            string circuitID = null;
            string localofficeID = null;
            string subtypeCD = null;
            string county = null;
            try
            {
                if (argdtRecords.Select("OBJECTID='" + argpFeature.OID + "'").Length == 0)
                {
                    globalid = Convert.ToString(argpFeature.get_Value(argpFeature.Fields.FindField(ReadConfigurations.col_GLOBALID)));
                    circuitID = Convert.ToString(argpFeature.get_Value(argpFeature.Fields.FindField(ReadConfigurations.col_CIRCUITID)));
                    localofficeID = Convert.ToString(argpFeature.get_Value(argpFeature.Fields.FindField(ReadConfigurations.col_LOCALOFFICEID)));
                    county = Convert.ToString(argpFeature.get_Value(argpFeature.Fields.FindField(ReadConfigurations.col_COUNTY)));
                    subtypeCD = Convert.ToString(argpFeature.get_Value(argpFeature.Fields.FindField(ReadConfigurations.col_SUBTYPECD)));

                    if (agrbProcessRunningForChanges)
                    {
                        argdtRecords.Rows.Add(argpFeature.OID, globalid, circuitID, county, subtypeCD);
                    }
                    else
                    {
                        argdtRecords.Rows.Add(argpFeature.OID, globalid, circuitID, localofficeID, subtypeCD);
                    }
                }
            }
            catch (Exception exp)
            {
                Common._log.Error("Exception : " + exp.Message +" |  Stack Trace | " + exp.StackTrace+" at "+exp.ToString());
            }
        }

        public static bool MakeChangesForCitVersionTable(bool blVersionPost, string sVersionName, string argStrOperation)
        {
            string sQuery = null;
            string sUpdateVal = null;
            try
            {
                //if ((blVersionPost == true) && (blVersionDelete == true))
                //{
                //    sUpdateVal = "1,1";
                //}
                //else if ((blVersionPost == true) && (blVersionDelete == false))
                //{
                //    sUpdateVal = "1,0";
                //}
                //else if ((blVersionPost == false) && (blVersionDelete == true))
                //{
                //    sUpdateVal = "0,1";
                //}
                //else if ((blVersionPost == false) && (blVersionDelete == false))
                //{
                //    sUpdateVal = "0,0";
                //}

                if (blVersionPost)
                {
                    sUpdateVal = "1";
                }
                else
                {
                    sUpdateVal = "0";
                }

                if (argStrOperation == "INSERT")
                {
                    sQuery = "INSERT INTO " + ConfigurationManager.AppSettings[ReadConfigurations.tableNameToSaveVersionData] + " (VERSION_NAME,POST_STATUS,PROCESSED_ON) VALUES " +
                            "('" + sVersionName + "'," + sUpdateVal + ",TO_DATE(sysdate,'" + ConfigurationManager.AppSettings["dateFormatToChange"] + "')" + ")";
                }
                else if (argStrOperation == "UPDATE")
                {
                    sQuery = "UPDATE " + ConfigurationManager.AppSettings[ReadConfigurations.tableNameToSaveVersionData] + " SET POST_STATUS=" + sUpdateVal + ",PROCESSED_ON=TO_DATE(sysdate, '" + ConfigurationManager.AppSettings["dateFormatToChange"] + "') WHERE VERSION_NAME='" + sVersionName + "'";
                }

                string strConnString = ReadConfigurations.ConnectionString_pgedata;
                int intUpdateResult = DBHelper.UpdateQuery(strConnString, sQuery);

                Common._log.Info("Ran query : " + sQuery + " and result count: " + intUpdateResult);

                return true;
            }
            catch (Exception exp)
            {
                Common._log.Error("Error in Updating the Custom Version Table at the time of Version Post and Delete. Exception : " + exp.Message + " |  Stack Trace | " + exp.StackTrace+" at "+exp.ToString());
                return false;
            }
        }

        public static void ReleaseStaticVariables()
        {
            try
            {
                if (MainClass._list_LineFractions != null)
                {
                    MainClass._list_LineFractions.Clear();
                    MainClass._list_LineFractions = null;
                }

                if (MainClass._dtConduitSubtypeDuctBankData != null)
                {
                    MainClass._dtConduitSubtypeDuctBankData.Clear();
                    MainClass._dtConduitSubtypeDuctBankData = null;
                }

                if (MainClass._dtconductorExceptions != null)
                {
                    MainClass._dtconductorExceptions.Clear();
                    MainClass._dtconductorExceptions = null;
                }

                if (MainClass._updated_PriUG != null)
                {
                    MainClass._updated_PriUG.Clear();
                    MainClass._updated_PriUG = null;
                }

                if (MainClass._listConductorCodesToExclude != null)
                {
                    MainClass._listConductorCodesToExclude.Clear();
                    MainClass._listConductorCodesToExclude = null;
                }

                if (MainClass._dtCondcutorWithCodes != null)
                {
                    MainClass._dtCondcutorWithCodes.Clear();
                    MainClass._dtCondcutorWithCodes = null;
                }

                MainClass._searchDistance = 0;
                MainClass._numberOfPointsToBeConsidered = 0;
                MainClass._cutoffLengthConductorToExclude = 0;
                MainClass._intConductorLengthToCalculateParallelLengthIsGreaterThanCutOffLength = 0;
                MainClass._filledduct_Value = 0;

                MainClass._databaseTableName_Exceptions = null;
                MainClass._databaseTableName_Merge = null;

                MainClass._queryToGetConduitSystemDuctBankData = null;
                MainClass._queryToGetChangeDetection = null;
                MainClass._queryToGetWebrManualTable = null;
                MainClass._processtype = null;
                MainClass._exception = null;
                MainClass._queryToGetChangedRecordsBasedOnCounty = null;

                MainClass._bLogNeededWhileExecuting = false;
                MainClass._bRunProcessFromQueryToGetRecords = false;
                MainClass._bRunProcessFromOIDsToGetRecords = false;
                MainClass._bisRunningProcessForChanges = false;

                #region Common

                if (_dtChangeDetection_PriUG != null)
                {
                    _dtChangeDetection_PriUG.Clear();
                    _dtChangeDetection_PriUG = null;
                }

                if (_dtmanualPriUG != null)
                {
                    _dtmanualPriUG.Clear();
                    _dtmanualPriUG = null;
                }

                if (_manualGlobalExisting != null)
                {
                    _manualGlobalExisting.Clear();
                    _manualGlobalExisting = null;
                }

                _mailContent = null;

                #endregion Common

                #region Bulk_process

                if (Bulk_Process._dtAllRule1Reocrds != null)
                {
                    Bulk_Process._dtAllRule1Reocrds.Clear();
                    Bulk_Process._dtAllRule1Reocrds = null;
                }

                Bulk_Process._isLeftRecords = false;
                Bulk_Process._isRunningProcessForChanges = false;

                #endregion Bulk_process
            }
            catch (Exception exp)
            {
                Common._log.Error("Exception : " + exp.Message +" |  Stack Trace | " + exp.StackTrace+" at "+exp.ToString());
                ErrorCodeException ece = new ErrorCodeException(exp);
                Environment.ExitCode = ece.CodeNumber;
            }
        }

        public static string GetConnectionStringToOpenSqlConnetion(string argStrConnectionString)
        {
            string connectionString = null;
            try
            {
                string[] oracleConnectionInfo = argStrConnectionString.Split(',');
                connectionString = "Data Source=" + oracleConnectionInfo[1] + ";User Id=" + oracleConnectionInfo[2] + ";password=" + oracleConnectionInfo[3];
            }
            catch (Exception exp)
            {
                Common._log.Error("Error in Functon GetConnectionStringToOpenSqlConnetion(). Exception : " + exp.Message + " |  Stack Trace | " + exp.StackTrace+" at "+exp.ToString());
                ErrorCodeException ece = new ErrorCodeException(exp);
                Environment.ExitCode = ece.CodeNumber;
            }
            return connectionString;
        }

        public static bool GetFilledductValueFromAlreadyProcessedConductors(DataTable argdtAllConductors, Int64 argintOIDPriUG, ref int argFilledductValue, ref string OIDSNeeded_AlreadyCalculated)
        {
            bool bValueFound = false;
            try
            {
                argFilledductValue = 0;
                DataRow[] pRows = argdtAllConductors.Select(ReadConfigurations.col_OBJECTID + "='" + argintOIDPriUG + "'");

                if (pRows.Length == 1)
                {
                    if (!pRows[0].IsNull(ReadConfigurations.col_FILLEDDUCT))
                    {
                        argFilledductValue = Convert.ToInt16(pRows[0][ReadConfigurations.col_FILLEDDUCT]);
                        OIDSNeeded_AlreadyCalculated = Convert.ToString(pRows[0][ReadConfigurations.col_OIDS_Needed]);
                        bValueFound = true;
                    }
                }
            }
            catch (Exception exp)
            {
                Common._log.Error("Exception : " + exp.Message +" |  Stack Trace | " + exp.StackTrace+" at "+exp.ToString());
                bValueFound = false;
            }
            return bValueFound;
        }

        public static string GetAllConductorInAcsenedingOrder(List<int> listintersectingLineFeatureOIDs)
        {
            string allConductors = null;
            try
            {
                listintersectingLineFeatureOIDs.Sort();
                foreach (int oid in listintersectingLineFeatureOIDs)
                {
                    allConductors += oid + ",";
                }

                allConductors = allConductors.Trim(',');
            }
            catch (Exception exp)
            {
                Common._log.Error("Exception : " + exp.Message +" |  Stack Trace | " + exp.StackTrace+" at "+exp.ToString());
            }
            return allConductors;
        }

        public static void SaveFilledductAndOIDsNeededForGivenConductors(DataTable argdtAllConductors, List<int> argListOIDs, int argintFilledDuct, string argStrAllOIDs)
        {
            int ioldFilledductValue = 0;
            DataRow[] pRows = null;
            string strWhereClause = null;
            try
            {
                foreach (int intOID in argListOIDs)
                {
                    strWhereClause = ReadConfigurations.col_OBJECTID + "='" + intOID + "'";
                    pRows = argdtAllConductors.Select(strWhereClause);

                    ioldFilledductValue = 0;
                    if (pRows.Length == 1)
                    {
                        if (!pRows[0].IsNull(ReadConfigurations.col_FILLEDDUCT))
                        {
                            ioldFilledductValue = Convert.ToInt16(pRows[0][ReadConfigurations.col_FILLEDDUCT]);
                        }

                        if (ioldFilledductValue < argintFilledDuct)
                        {
                            pRows[0][ReadConfigurations.col_FILLEDDUCT] = argintFilledDuct;
                            pRows[0][ReadConfigurations.col_OIDS_Needed] = argStrAllOIDs;
                        }
                    }
                    argdtAllConductors.AcceptChanges();
                }
            }
            catch (Exception exp)
            {
                Common._log.Error("Exception : " + exp.Message +" |  Stack Trace | " + exp.StackTrace+" at "+exp.ToString());
            }
        }

        public static void Save_ConflictInformation(IFeatureClass argpriUGFeatureClass, string argStrConnectionString, string argStrConnectionString_pgedata)
        {
            IFeature pFeaturePriUGCond = null;
            DataTable dtAllConflicts = null;
            DataTable dtCodesAndValues = null;
            DataTable dtCircuitSource = null;
            DataTable dtAllConflicts_FromDatabase = null;
            string priUG_value = null;
            string[] values_priUG = null;
            string objectID = string.Empty;
            string globalID = null;
            string citupdatedon = null;
            int filledduct_manual = 0;
            int filledduct_captured = 0;
            string district = null;
            string division = null;
            string localOfficeID = null;
            string circuitID = null;
            string subID = null;
            string subName = null;
            string strProcessedValue = null;
            string strQuery = null;           
            DataRow[] pRows = null;
            try
            {
                dtAllConflicts = new DataTable();
                dtAllConflicts.Columns.Add(ReadConfigurations.col_OBJECTID, typeof(string));
                dtAllConflicts.Columns.Add(ReadConfigurations.col_GLOBALID, typeof(string));
                dtAllConflicts.Columns.Add(ReadConfigurations.col_CIT_UPDATEDON, typeof(DateTime));
                dtAllConflicts.Columns.Add(ReadConfigurations.col_FILLEDDUCT_MANUAL, typeof(int));
                dtAllConflicts.Columns.Add(ReadConfigurations.col_FILLEDDUCT_CAPTURED, typeof(int));
                dtAllConflicts.Columns.Add(ReadConfigurations.col_DISTRICT, typeof(string));
                dtAllConflicts.Columns.Add(ReadConfigurations.col_DIVISION, typeof(string));
                dtAllConflicts.Columns.Add(ReadConfigurations.col_LOCALOFFICEID, typeof(string));
                dtAllConflicts.Columns.Add(ReadConfigurations.col_CIRCUITID, typeof(string));
                dtAllConflicts.Columns.Add(ReadConfigurations.col_SUBSTATIONID, typeof(string));
                dtAllConflicts.Columns.Add(ReadConfigurations.col_SUBSTATIONNAME, typeof(string));
                dtAllConflicts.Columns.Add(ReadConfigurations.col_PROCESSED, typeof(string));

                strQuery = "select UPPER(DOMAIN_NAME) as DOMAIN_NAME,UPPER(CODE) as CODE,DESCRIPTION from " + ReadConfigurations.TB_CODESDESCRIPTION;
                dtCodesAndValues = DBHelper.GetDataTable(argStrConnectionString, strQuery);

                strQuery = "select CIRCUITID,SUBSTATIONID,SUBSTATIONNAME from " + ReadConfigurations.TB_CIRCUITSOURCE;
                dtCircuitSource = DBHelper.GetDataTable(argStrConnectionString, strQuery);

                //Getting all new status conflict records 
                strQuery = "select * from " + ReadConfigurations.GetValue(ReadConfigurations.tableNameForConflictInformation) + " where processed is null or processed='N'";
                dtAllConflicts_FromDatabase = DBHelper.GetDataTable(argStrConnectionString_pgedata, strQuery);

                for (int count = 0; count < _manualGlobalExisting.Count; count++)
                {
                    try
                    {
                        priUG_value = Convert.ToString(_manualGlobalExisting[count]);
                        values_priUG = priUG_value.Split('|');

                        globalID = values_priUG[0].Trim();
                        objectID = values_priUG[1].Trim();
                        filledduct_manual = Convert.ToInt32(values_priUG[2].Trim());
                        filledduct_captured = Convert.ToInt32(values_priUG[3].Trim());

                        pFeaturePriUGCond = argpriUGFeatureClass.GetFeature(Convert.ToInt32(objectID));
                        division = Convert.ToString(pFeaturePriUGCond.get_Value(pFeaturePriUGCond.Fields.FindField(ReadConfigurations.col_DIVISION)));
                        division = GetValueFromCode(ReadConfigurations.col_DIVISION, division, dtCodesAndValues);

                        district = Convert.ToString(pFeaturePriUGCond.get_Value(pFeaturePriUGCond.Fields.FindField(ReadConfigurations.col_DISTRICT)));
                        district = GetValueFromCode(ReadConfigurations.col_DISTRICT, district, dtCodesAndValues);

                        localOfficeID = Convert.ToString(pFeaturePriUGCond.get_Value(pFeaturePriUGCond.Fields.FindField(ReadConfigurations.col_LOCALOFFICEID)));
                        localOfficeID = GetValueFromCode(ReadConfigurations.col_LOCALOFFICEID, localOfficeID, dtCodesAndValues);

                        circuitID = Convert.ToString(pFeaturePriUGCond.get_Value(pFeaturePriUGCond.Fields.FindField(ReadConfigurations.col_CIRCUITID)));

                        citupdatedon = Convert.ToString(pFeaturePriUGCond.get_Value(pFeaturePriUGCond.Fields.FindField(ReadConfigurations.col_CIT_UPDATEDON)));
                        if (string.IsNullOrEmpty(citupdatedon))
                        {
                            // That means this condutor is not updated yet and citUpdatedOn will be present in WEBR table or taoday's date                           
                            citupdatedon = DateTime.Now.ToString("dd-MMM-y").ToUpper();
                        }

                        subID = GetSubstationAttributes(ReadConfigurations.col_SUBSTATIONID, circuitID, dtCircuitSource);
                        subName = GetSubstationAttributes(ReadConfigurations.col_SUBSTATIONNAME, circuitID, dtCircuitSource);
                        strProcessedValue = "N";

                        //ACTIONITEM First checkin in database , if present then deleting it
                        pRows = dtAllConflicts_FromDatabase.Select("GLOBALID='" + globalID + "' AND processed='N'");

                        if (pRows.Length > 0)
                        {
                            Common._log.Info("Deleting the conductor with GLOBALID=" + globalID + " from conflict table because new entry will be added with same globalid.");
                            string strTableName = ReadConfigurations.GetValue(ReadConfigurations.tableNameForConflictInformation);
                            string query = "DELETE FROM " + strTableName + " where GLOBALID='" + globalID + "' AND (processed is null or processed='N')";
                            DBHelper.RunGivenQueryOnTable(strTableName, query);

                            pRows[0].Delete();
                        }

                        dtAllConflicts.Rows.Add(objectID, globalID, citupdatedon, filledduct_manual, filledduct_captured, district, division, localOfficeID, circuitID, subID, subName, strProcessedValue);
                    }
                    catch (Exception exp)
                    {
                        Common._log.Error("Exception : " + exp.Message + " |  Stack Trace | " + exp.StackTrace + " at " + exp.ToString());
                    }
                }

                Common.BulkCopyDataFromDataTableToDatabaseTable(dtAllConflicts, ReadConfigurations.GetValue(ReadConfigurations.tableNameForConflictInformation), argStrConnectionString_pgedata);
            }
            catch (Exception exp)
            {
                Common._log.Error("Exception : " + exp.Message + " |  Stack Trace | " + exp.StackTrace + " at " + exp.ToString());
                ErrorCodeException ece = new ErrorCodeException(exp);
                Environment.ExitCode = ece.CodeNumber;
            }
            finally
            {
                if (dtCodesAndValues != null)
                {
                    dtCodesAndValues.Clear();
                    dtCodesAndValues = null;
                }

                if (dtCircuitSource != null)
                {
                    dtCircuitSource.Clear();
                    dtCircuitSource = null;
                }           
            }
        }

        private static string GetValueFromCode(string argstrColumnName, string argStrCode, DataTable argdtAllCodesAndValues)
        {
            string strValue = null;
            string strDomainName = null;
            string strWhereClause = null;
            DataRow[] pRows = null;
            try
            {
                strDomainName = GetDomainName(argstrColumnName);
                strWhereClause = "DOMAIN_NAME='" + strDomainName + "' AND " + ReadConfigurations.col_CODE + "='" + argStrCode.ToUpper() + "'";
                pRows = argdtAllCodesAndValues.Select(strWhereClause);

                if (pRows.Length > 0)
                {
                    strValue = Convert.ToString(pRows[0][ReadConfigurations.col_DESCRIPTION]);
                }
            }
            catch (Exception exp)
            {
                Common._log.Error("Exception : " + exp.Message +" |  Stack Trace | " + exp.StackTrace+" at "+exp.ToString());
                ErrorCodeException ece = new ErrorCodeException(exp);
                Environment.ExitCode = ece.CodeNumber;
            }
            return strValue;
        }

        private static string GetDomainName(string argStrColumnName)
        {
            string strDomainName = null;
            string columnCodes = null;
            string[] columnCodesarray = null;
            try
            {
                columnCodes = ConfigurationManager.AppSettings["columnCodes"];
                columnCodesarray = columnCodes.Split(',');

                foreach (string strColumnCode in columnCodesarray)
                {
                    if (strColumnCode.Split('#')[0] == argStrColumnName)
                    {
                        strDomainName = strColumnCode.Split('#')[1];
                        break;
                    }
                }
            }
            catch (Exception exp)
            {
                Common._log.Error("Exception : " + exp.Message +" |  Stack Trace | " + exp.StackTrace+" at "+exp.ToString());
                ErrorCodeException ece = new ErrorCodeException(exp);
                Environment.ExitCode = ece.CodeNumber;
            }
            return strDomainName;
        }

        private static string GetSubstationAttributes(string argstrColumnName, string argCircuitID, DataTable argdtCircuitSource)
        {
            string strValue = null;
            string strWhereClause = null;
            DataRow[] pRows = null;
            try
            {
                strWhereClause = "CIRCUITID='" + argCircuitID + "'";
                pRows = argdtCircuitSource.Select(strWhereClause);

                if (pRows.Length > 0)
                {
                    strValue = Convert.ToString(pRows[0][argstrColumnName]);
                }
            }
            catch (Exception exp)
            {
                Common._log.Error("Exception : " + exp.Message +" |  Stack Trace | " + exp.StackTrace+" at "+exp.ToString());
            }
            return strValue;
        }

        public static void insert_ToDB_conflict_Information(string globalID, string objectID, int filled_manual, int filled_auto, string division, string recepient)
        {
            string strConnString = string.Empty;
            try
            {
                strConnString = ReadConfigurations.ConnectionString_pgedata;
                DBHelper.InsertQuery(strConnString, globalID, objectID, filled_manual, filled_auto, division, recepient);
            }
            catch (Exception exp)
            {
                _log.Error("Error storing data: ", exp);
                Common._log.Error("Exception : " + exp.Message +" |  Stack Trace | " + exp.StackTrace+" at "+exp.ToString());
                ErrorCodeException ece = new ErrorCodeException(exp);
                Environment.ExitCode = ece.CodeNumber;
            }
        }

        public static DataTable RunGivenQuerySendMail(string argTodaysDate)
        {
            string tableName = null;
            DataTable argdtAllDataCount = null;
            string strConnString = string.Empty;
            try
            {
                strConnString = ReadConfigurations.ConnectionString_pgedata;
                tableName = ReadConfigurations.GetValue(ReadConfigurations.tableNameForConflictInformation);
                var sql = "select * from " + tableName + " where CIT_UPDATEDON < '" + argTodaysDate + "' and processed = 'N'";
                argdtAllDataCount = DBHelper.GetDataTable(strConnString, sql);
            }
            catch (Exception exp)
            {
                _log.Error("Error storing data: ", exp);
                Common._log.Error("Exception : " + exp.Message +" |  Stack Trace | " + exp.StackTrace+" at "+exp.ToString());
                ErrorCodeException ece = new ErrorCodeException(exp);
                Environment.ExitCode = ece.CodeNumber;
            }
            return argdtAllDataCount;
        }

        public static void Check_and_SendMail(string argStrConnString_pgedata, IFeatureClass argPriugFeatureclass)
        {
            DataTable pdtConflicts = null;
            int intDayDifferenceFromConfig = 0;
            string sDate_Processed = string.Empty;
            string sPresent_Date = string.Empty;
            string strDayToSendMail = null;
            DateTime lastMailSentOn;
            double dblDaysDifference = 0;
            int intDaysDifference = 0;
            DateTime todaysDate;
            string dateTimeInRequiredFormat = null;
            string query = null;
            string whereClause = null;
            DataRow pRow = null;
            List<string> days = null;
            bool bSendMail = false;
            try
            {
                intDayDifferenceFromConfig = Convert.ToInt16(ConfigurationManager.AppSettings["SendMailDaysDifference"]);                              
                //ACTION ITEM - There should be single entry for a record in conflict table 
                strDayToSendMail = ConfigurationManager.AppSettings["SendMailDay"];
                days = strDayToSendMail.Split(',').ToList<string>();

                foreach (string day in days)
                {
                    if (DateTime.Now.DayOfWeek.ToString() == day)
                    {
                        bSendMail = true;
                        break;
                    }
                }

                if (bSendMail)
                {
                    todaysDate = DateTime.Now.Date;

                    query = "select " + ReadConfigurations.col_CIT_LASTMAILSENT + " from " + ReadConfigurations.TB_CIT_PGE_LASTMAILSENT;
                    pRow = DBHelper.GetSingleDataRowByQuery(argStrConnString_pgedata, query);

                    if (pRow != null)
                    {
                        lastMailSentOn = Convert.ToDateTime(pRow[ReadConfigurations.col_CIT_LASTMAILSENT]);
                    }
                    else
                    {
                        lastMailSentOn = DateTime.Today.AddDays(-1);
                    }

                    dblDaysDifference = (todaysDate - lastMailSentOn).TotalDays;

                    intDaysDifference = Convert.ToInt16(dblDaysDifference);

                    if (intDaysDifference >= intDayDifferenceFromConfig)
                    {
                        dateTimeInRequiredFormat = DateTime.Now.AddDays(-2).Date.ToString(ReadConfigurations.dateFormat_Database).ToUpper();
                        whereClause = " where CIT_UPDATEDON <= '" + dateTimeInRequiredFormat + "' and processed = 'N' ";
                        query = ConfigurationManager.AppSettings["columnOrder"] + whereClause + ConfigurationManager.AppSettings["OrderBy"];
                        pdtConflicts = DBHelper.GetDataTable(argStrConnString_pgedata, query);

                        if (pdtConflicts.Rows.Count > 0)
                        {
                            GetWEBRUrlForConductors(ref pdtConflicts, argPriugFeatureclass);
                       
                            if (Common.ComplieAndSendMail(pdtConflicts))
                            {
                                DBHelper.UpdateConflictInformationTable(whereClause, sPresent_Date);

                                string dateTimeNowInRequiredFormat = DateTime.Now.Date.ToString(ReadConfigurations.dateFormat_Database).ToUpper();

                                //Updating lastmailsent table also 
                                query = "UPDATE " + ReadConfigurations.TB_CIT_PGE_LASTMAILSENT + " SET " + ReadConfigurations.col_CIT_LASTMAILSENT + "='" + dateTimeNowInRequiredFormat + "'";
                                DBHelper.RunGivenQueryOnTable(ReadConfigurations.col_CIT_LASTMAILSENT, query);
                            }
                        }
                    }
                }
            }
            catch (Exception exp)
            {
                Common._log.Error("Exception : " + exp.Message +" |  Stack Trace | " + exp.StackTrace+" at "+exp.ToString());
                ErrorCodeException ece = new ErrorCodeException(exp);
                Environment.ExitCode = ece.CodeNumber;
            }
            finally
            {
                if (days != null)
                {
                    days.Clear();
                    days = null;
                }            
            }
        }

        public static bool ComplieAndSendMail(DataTable argdtConflictingPriUGConductors)
        {
            bool bMailSentSuccess = false;
            try
            {
                string toemailID = ConfigurationManager.AppSettings["ToemailID"]; ;
                string fromAddress = ConfigurationManager.AppSettings["MAIL_FROM_ADDRESS"];
                string fromDisplayName = ConfigurationManager.AppSettings["MAIL_FROM_DISPLAY_NAME"];
                string subject = ConfigurationManager.AppSettings["SUBJECT"];
                // ACTION NEEDED- Mail content will be given to business for verification
                string bodyText = "Hello, <br /><br />" + ConfigurationManager.AppSettings["mailContent"] + " <br /><br />Thank You, <br />EDGIS Support Team";
                Common._mailContent = EmailMainClass.GetMailContentFromDataTable(argdtConflictingPriUGConductors);

                bMailSentSuccess = EmailService.Send(mail =>
                {
                    mail.From = fromAddress;
                    mail.FromDisplayName = fromDisplayName;
                    mail.To = toemailID;
                    mail.Subject = subject;
                    mail.BodyText = bodyText;
                });
                _log.Info("Email Successfully sent to: " + toemailID);
            }
            catch (Exception exp)
            {
                bMailSentSuccess = false;
                _log.Error("Error in Sending Mail: ", exp);
                Common._log.Error("Exception : " + exp.Message +" |  Stack Trace | " + exp.StackTrace+" at "+exp.ToString());
                ErrorCodeException ece = new ErrorCodeException(exp);
                Environment.ExitCode = ece.CodeNumber;
            }
            return bMailSentSuccess;
        }

        private static void GetWEBRUrlForConductors(ref DataTable argdtConductors, IFeatureClass argPriugFeatureClass)
        {
            IFeature pFeature = null;
            string strWebrurl = null;
            int oid = 0;
            string strWebrAppurl = null;
            try
            {
                argdtConductors.Columns.Add(ReadConfigurations.col_WEBRURL, typeof(string));

                strWebrAppurl = ConfigurationManager.AppSettings["webrUrl"];

                for (int iCount = 0; iCount < argdtConductors.Rows.Count; iCount++)
                {
                    try
                    {
                        oid = Convert.ToInt32(argdtConductors.Rows[iCount][ReadConfigurations.col_OBJECTID]);

                        pFeature = argPriugFeatureClass.GetFeature(oid);

                        strWebrurl = GetWEBRUrl(pFeature, strWebrAppurl);
                        argdtConductors.Rows[iCount][ReadConfigurations.col_WEBRURL] = strWebrurl;
                    }
                    catch (Exception exp)
                    {
                        Common._log.Error("Exception : " + exp.Message +" |  Stack Trace | " + exp.StackTrace+" at "+exp.ToString());
                    }
                }
            }
            catch (Exception exp)
            {
                Common._log.Error("Exception : " + exp.Message +" |  Stack Trace | " + exp.StackTrace+" at "+exp.ToString());
                ErrorCodeException ece = new ErrorCodeException(exp);
                Environment.ExitCode = ece.CodeNumber;
            }
        }

        private static string GetWEBRUrl(IFeature argpriUGFeature, string argStrWebrAppurl)
        {
            string strUrl = null;
            try
            {
                IPoint pOutputPoint = new PointClass();
                ICurve pCurve = (ICurve)argpriUGFeature.Shape;
                pCurve.QueryPoint(esriSegmentExtension.esriNoExtension, 0.5, true, pOutputPoint);

                //Find Latitude and Longitude
                ISpatialReferenceFactory2 srFactory = new SpatialReferenceEnvironmentClass();
                int NAD27_Geograpic = (4326);
                ISpatialReference GeographicSR = srFactory.CreateSpatialReference(NAD27_Geograpic);

                IPoint geographicPoint = new ESRI.ArcGIS.Geometry.Point();
                geographicPoint.X = pOutputPoint.X;
                geographicPoint.Y = pOutputPoint.Y;
                geographicPoint.Z = pOutputPoint.Z;

                geographicPoint.SpatialReference = pOutputPoint.SpatialReference;

                geographicPoint.Project(GeographicSR);
                double dLat = Math.Round(geographicPoint.Y, 6);
                double dLong = Math.Round(geographicPoint.X, 6);
                strUrl = argStrWebrAppurl + "/?LAT=" + dLat + "&LONG=" + dLong;
                //http://wwwedgis/EDViewer/?LAT=37.5553&LONG=-122.3173
            }
            catch (Exception exp)
            {
                Common._log.Error("Exception : " + exp.Message +" |  Stack Trace | " + exp.StackTrace+" at "+exp.ToString());
                ErrorCodeException ece = new ErrorCodeException(exp);
                Environment.ExitCode = ece.CodeNumber;
            }
            return strUrl;
        }

        private static bool UpdateStageTableForChangedConductor(List<int> _lstChangedConductorsUpdated)
        {
            bool bUpdate = false;
            string strConncetionString = null;
            string strTableName = null;
            List<string> allQueries = new List<string>();
            try
            {
                strTableName = ConfigurationManager.AppSettings["tableNameToUpdateForChanges"];

                //Get pgedata conncetion string here 
                strConncetionString = ConfigurationManager.AppSettings["ConnectionString_pgedata"]; ;

                foreach (int OID in _lstChangedConductorsUpdated)
                {
                    string strUpdateQuery = "update " + strTableName + " SET UpdateFlag='Y' where OBJECTID=" + OID;
                    DBHelper.UpdateQuery(strConncetionString, strUpdateQuery);                    
                    //allQueries.Add(strUpdateQuery);
                }

                //DBHelper.UpdateMultipleQueries(strConncetionString, allQueries);

                bUpdate = true;
            }
            catch (Exception exp)
            {
                bUpdate = false;
                Common._log.Error("Exception : " + exp.Message +" |  Stack Trace | " + exp.StackTrace+" at "+exp.ToString());
            }
            finally
            {
                if (allQueries != null)
                {
                    allQueries.Clear();
                    allQueries = null;
                }
            }
            return bUpdate;
        }
    }
}
