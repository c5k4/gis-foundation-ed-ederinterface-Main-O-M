using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using ESRI.ArcGIS.esriSystem;
using Miner.Interop;
using System.Configuration;
using System.IO;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using Oracle.DataAccess.Client;
using ESRI.ArcGIS.Geodatabase;
using System.Runtime.InteropServices;
using System.Collections;
using Miner.Geodatabase.Network;
using Miner.Geodatabase.FeederManager;
using Miner.Geodatabase;
using System.Data.OleDb;
using PGE.BatchApplication.IGPPhaseUpdate.Utility_Classes;
using PGE.BatchApplication.IGPPhaseUpdate.Processing_Logic_Classes;

namespace PGE.BatchApplication.IGPPhaseUpdate
{
    public class Common
    {

        public Log4NetLogger _log = new Log4NetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, ReadConfigurations.LOGCONFIG);

        //Feature update issue 
        public static bool _bwriteExtensivelog = false;

        public IWorkspace GetWorkspace(string argStrworkSpaceConnectionstring)
        {
            Type t = null;
            IWorkspaceFactory workspaceFactory = null;
            IWorkspace workspace = null;
            try
            {
                WriteLine_Info("Connection string used in process," + ReadConfigurations.GetValue(ReadConfigurations.SDEWorkSpaceConnString));
                t = Type.GetTypeFromProgID("esriDataSourcesGDB.SdeWorkspaceFactory");
                workspaceFactory = (IWorkspaceFactory)Activator.CreateInstance(t);
                workspace = workspaceFactory.OpenFromFile(argStrworkSpaceConnectionstring, 0);
                WriteLine_Info("Workspace checked out successfully.");

                if (workspace == null)
                {
                    WriteLine_Error("Workspace not found for conn string," + ReadConfigurations.GetValue(ReadConfigurations.SDEWorkSpaceConnString));
                    throw new Exception("Exiting the process.");
                }
            }
            catch (Exception exp)
            {
                WriteLine_Error("Error in getting SDE Workspace, function GetWorkspace," + exp.Message);
                WriteLine_Error(exp.Message + "   " + exp.StackTrace);

                ErrorCodeException ece = new ErrorCodeException(exp);
                Environment.ExitCode = ece.CodeNumber;
                throw exp;
            }
            return workspace;
        }


        public IFeature GetFeature(int iFieldValue, string sFieldName, IFeatureClass pQureyFeatureClass)
        {
            IFeature _ReturnFeature = null;
            try
            {
                IQueryFilter pQueryFilter = new QueryFilterClass();
                pQueryFilter.WhereClause = sFieldName + " = " + iFieldValue;
                IFeatureCursor pFeatureCursor = pQureyFeatureClass.Search(pQueryFilter, false);
                _ReturnFeature = pFeatureCursor.NextFeature();
                if (_ReturnFeature == null)
                {

                    WriteLine_Error("A requested feature is not retrieved,FieldName," + sFieldName + ",FieldValue," + iFieldValue.ToString() + ",Class," + pQureyFeatureClass.AliasName.ToString());
                }
                Marshal.ReleaseComObject(pFeatureCursor);

            }
            catch (Exception ex)
            {

                WriteLine_Error("A requested feature is not retrieved,FieldName," + sFieldName + ",FieldValue," + iFieldValue.ToString() + ",Class," + pQureyFeatureClass.AliasName.ToString());
                WriteLine_Error(ex.Message + " at " + ex.StackTrace);
            }
            return _ReturnFeature;
        }
        public IFeature GetFeature(string sFieldValue, string sFieldName, IFeatureClass pQueryFeatureClass)
        {
            IFeature _ReturnFeature = null;
            try
            {
                IQueryFilter pQueryFilter = new QueryFilterClass();
                pQueryFilter.WhereClause = sFieldName + " = '" + sFieldValue + "'";
                IFeatureCursor pFeatureCursor = pQueryFeatureClass.Search(pQueryFilter, false);
                _ReturnFeature = pFeatureCursor.NextFeature();
                if (_ReturnFeature == null)
                {
                    //WriteLine_Error("A requested feature is not retrieved,FieldName- " + sFieldName + " : FieldValue- " + sFieldValue.ToString() + " : Class :" + pQueryFeatureClass.AliasName.ToString());
                }
                Marshal.ReleaseComObject(pFeatureCursor);

            }
            catch (Exception ex)
            {
                WriteLine_Error("A requested feature is not retrieved,FieldName," + sFieldName + ",FieldValue," + sFieldValue.ToString() + ",Class," + pQueryFeatureClass.AliasName.ToString());
                WriteLine_Error(ex.Message + " at " + ex.StackTrace);
            }
            return _ReturnFeature;
        }


        public string GetFieldValue(IFeature InputFeature, string sFieldName)
        {
            string sFieldValue = string.Empty;
            try
            {

                int intfldindex = InputFeature.Fields.FindField(sFieldName);

                if (intfldindex != -1)
                {
                    IDomain pDomain = ((IField)InputFeature.Fields.get_Field(intfldindex)).Domain;
                    //object objST = pfild.Domain
                    if (pDomain != null)
                    {
                        sFieldValue = InputFeature.get_Value(intfldindex).ToString();
                        if (pDomain is ICodedValueDomain)
                        {
                            ICodedValueDomain pCVDomain = (ICodedValueDomain)pDomain;

                            for (int x = 0; x <= pCVDomain.CodeCount - 1; x++)
                            {
                                string sDomainName = pCVDomain.get_Value(x).ToString();
                                if (sFieldValue.ToUpper().Trim() == sDomainName.ToUpper().Trim())
                                {
                                    sFieldValue = pCVDomain.get_Name(x).ToString();

                                    break;
                                }
                            }
                        }
                    }
                    else
                    {
                        sFieldValue = InputFeature.get_Value(intfldindex).ToString();
                    }

                }
            }
            catch (Exception ex)
            {
                WriteLine_Error(ex.Message + " at " + ex.StackTrace);
                throw ex;
            }
            return sFieldValue;
        }
        private void InsertValueInDeEnergizeList(IFeature pFeature, ref DataTable DeEnergizeList, string sCircuitID)
        {
            try
            {

                DataRow DR = DeEnergizeList.NewRow();
                // string ss = pFeature.get_Value(pFeature.Fields.FindField(ReadConfigurations.GUIDFIELDNAME)).ToString();
                DR["OBJECTID"] = pFeature.OID;
                DR["GLOBALID"] = pFeature.get_Value(pFeature.Fields.FindField(ReadConfigurations.GUIDFIELDNAME)).ToString();
                DR["CLASS_NAME"] = ((IDataset)pFeature.Class).BrowseName;
                DR["VERSION_NAME"] = ((pFeature.Class as IFeatureClass).FeatureDataset.Workspace as IVersion).VersionName;
                DR["CIRCUITID"] = sCircuitID;
                DR["BATCHID"] = MainClass.Batch_Number;
                DR["SERIAL_NO"] = MainClass.sSerial_No;
                if (ContainDataRowInDataTable(pFeature.OID.ToString(), DeEnergizeList, "OBJECTID") == false)
                {
                    DeEnergizeList.Rows.Add(DR);
                }
            }
            catch (Exception ex)
            {
                WriteLine_Error("Requested Record is not inserted in the De-Energize List");
                WriteLine_Error(ex.Message + " at " + ex.StackTrace);
            }
        }
        internal void InsertRecordInDatatable_UnProcessed(string sRec_Type, string ObjectClassName, int Feature_OID, string PhaseValue, string sProcessed,
            string sFeederId, string sOldPhase, string sErrorMsg, string sFeatGuid, string sSubType, string sReviewBy, string sParentGuid, string sParentClass)
        {
            try
            {
                if (ContainDataRowInDataTable(sFeatGuid, MainClass.g_dtAllDetails, "FEATURE_GUID") == false)
                {
                    if (string.IsNullOrEmpty(PhaseValue))
                    {
                        int i = 0;
                    }
                    DataRow DR = MainClass.g_dtAllDetails.NewRow();

                    DR["REC_TYPE"] = sRec_Type;
                    DR["NAME"] = ObjectClassName;
                    DR["FEATURE_OID"] = Feature_OID;
                    DR["VALUE"] = PhaseValue;
                    DR["PROCESSED"] = sProcessed;
                    DR["CIRCUITID"] = sFeederId;
                    DR["OLD_PHASE"] = sOldPhase;
                    DR["ERROR_MSG"] = sErrorMsg;
                    DR["FEATURE_GUID"] = sFeatGuid;
                    if(!string.IsNullOrEmpty(sSubType))  DR["SUBTYPE"] = sSubType;
                    DR["REVIEWBY"] = sReviewBy;
                    DR["BATCHID"] = MainClass.Batch_Number;
                    DR["SERIAL_NO"] = MainClass.sSerial_No;
                    DR["PARENT_GUID"] = sParentGuid;
                    DR["PARENT_CLASS"] = sParentClass;

                    MainClass.g_dtAllDetails.Rows.Add(DR);
                }
                else
                {
                    DataRow[] drd = MainClass.g_dtAllDetails.Select("FEATURE_GUID = '" + sFeatGuid.ToString() + "'");

                    if (drd != null)
                    {
                        if (drd.Length > 0)
                        {
                            //for (int i = 0; i < drd.Length; i++)
                            //{
                            DataRow DR = drd[0];

                            DR["REC_TYPE"] = sRec_Type;
                            DR["NAME"] = ObjectClassName;
                            DR["FEATURE_OID"] = Feature_OID;
                            DR["VALUE"] = PhaseValue;
                            DR["PROCESSED"] = sProcessed;
                            DR["CIRCUITID"] = sFeederId;
                            DR["OLD_PHASE"] = sOldPhase;
                            DR["ERROR_MSG"] = sErrorMsg;
                            DR["FEATURE_GUID"] = sFeatGuid;
                            DR["SUBTYPE"] = sSubType;
                            DR["REVIEWBY"] = sReviewBy;
                            DR["BATCHID"] = MainClass.Batch_Number;
                            DR["SERIAL_NO"] = MainClass.sSerial_No;
                            DR["PARENT_GUID"] = sParentGuid;
                            DR["PARENT_CLASS"] = sParentClass;
                            //}
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                WriteLine_Error("Requested Record is not inserted in the table," + sRec_Type + "," + ObjectClassName + "," + Feature_OID + "," + PhaseValue + "," + sFeederId);
                WriteLine_Error(ex.Message + " at " + ex.StackTrace);
            }

        }


        private bool ContainDataRowInDataTable(string sOID, DataTable DT, string sfield)
        {
            bool _retval = true;
            try
            {
                string valueToSearch = sOID;
                string columnName = sfield;
                int count = (int)DT.Compute(string.Format("count({0})", columnName),
                        string.Format("{0} like '{1}'", columnName, valueToSearch));
                if (count == 0)
                {
                    _retval = false;
                }
            }
            catch (Exception ex)
            {
                WriteLine_Error(ex.Message + " at " + ex.StackTrace);
            }

            return _retval;
        }

        internal void InsertRecordInDeviceDatatable(string sRec_Type, string ObjectClassName, int Feature_OID, string PhaseValue, string sProcessed, string sFeederId,
            string NP_A, string NP_B, string NP_C, string NP, string sOldPhase, string sFeatGuid, string sSubType, string sReviewBy)
        {
            try
            {
                DataRow DR = MainClass.g_dtAllDetailsForDevice.NewRow();

                DR["REC_TYPE"] = sRec_Type;
                DR["NAME"] = ObjectClassName;
                DR["FEATURE_OID"] = Feature_OID;
                DR["VALUE"] = PhaseValue;
                DR["PROCESSED"] = sProcessed;
                DR["CIRCUITID"] = sFeederId;
                DR["NORMAL_POSITION_A"] = NP_A;
                DR["NORMAL_POSITION_B"] = NP_B;
                DR["NORMAL_POSITION_C"] = NP_C;
                DR["NORMAL_POSITION"] = NP;
                DR["OLD_PHASE"] = sOldPhase;
                DR["FEATURE_GUID"] = sFeatGuid;
                DR["SUBTYPE"] = sSubType;
                DR["REVIEWBY"] = sReviewBy;
                DR["BATCHID"] = MainClass.Batch_Number;
                DR["SERIAL_NO"] = MainClass.sSerial_No;
                if (ContainDataRowInDataTable(Feature_OID.ToString(), MainClass.g_dtAllDetailsForDevice, "FEATURE_OID") == false)
                {
                    MainClass.g_dtAllDetailsForDevice.Rows.Add(DR);
                }

            }
            catch (Exception ex)
            {
                WriteLine_Error("Requested Record is not inserted in the table," + sRec_Type + "," + ObjectClassName + "," + Feature_OID + "," + PhaseValue + "," + sFeederId);
                WriteLine_Error(ex.Message + " at " + ex.StackTrace);
            }
        }

        //internal  bool DeleteRowFromDataTable(string sType, string sName, int iOid, string sValue,string sProcessed, string TRANS_GUID)
        internal bool DeleteRowFromDataTable(string sType, string sName, int iOid, string sValue, string TRANS_GUID)
        {
            string sQuery = null;
            try
            {

                sQuery = "REC_TYPE = '" + sType + "' AND NAME = '" + sName + "' AND FEATURE_OID = '" + iOid.ToString() + "' AND VALUE = '" + sValue + "'";

                DataRow[] pRows = MainClass.g_dtAllDetails.Select(sQuery);

                for (int i = pRows.Length - 1; i >= 0; i--)
                {
                    DataRow dr = pRows[i];

                    dr.Delete();
                }
                return true;
            }
            catch (Exception ex)
            {
                WriteLine_Error(ex.Message + " at " + ex.StackTrace);
                return false;
            }
        }

        internal bool DeleteRowsFor3PhaseTransformer(string sName, string TRANS_GUID)
        {
            string sQuery = null;
            try
            {
                sQuery = "PARENT_GUID = '" + TRANS_GUID + "' AND PARENT_CLASS = '" + sName + "' AND PROCESSED = 'N'";
                DataRow[] pRows = MainClass.g_dtAllDetails.Select(sQuery);

                for (int i = pRows.Length - 1; i >= 0; i--)
                {
                    DataRow dr = pRows[i];

                    dr.Delete();
                }
                return true;
            }
            catch (Exception ex)
            {
                WriteLine_Error(ex.Message + " at " + ex.StackTrace);
                return false;
            }
        }

        internal bool DeleteAllRows_ForTransformer(string TRANS_GUID)
        {
            string sQuery = null;
            try
            {

                sQuery = "INPUT_TRANSFORMER = '" + TRANS_GUID + "' AND PROCESSED = '" + "N" + "'";
                DataRow[] pRows = MainClass.g_dtAllDetails.Select(sQuery);

                for (int i = pRows.Length - 1; i >= 0; i--)
                {
                    DataRow dr = pRows[i];

                    dr.Delete();
                }
                return true;
            }
            catch (Exception ex)
            {
                WriteLine_Error(ex.Message + " at " + ex.StackTrace);
                return false;
            }
        }


        internal bool StoreFeaturesFromTable(IWorkspace pWorkspace, string sCircuitId)
        {
            Hashtable DeviceList = new Hashtable();
            string sQuery = null;
            DataTable pDataTable = new DataTable();
            string sRecType = null;
            string sFeatClsName = null;
            int iOid = 0;
            int iUpdatedFeatureCount = 0;
            string sPhaseValue = null;
            string sFeederID = string.Empty;
            string sGuidFeat = null;
            IFeatureClass pFeatClass = null;
            Hashtable pHT_All_FeatCls = new Hashtable();
            Hashtable pHT_All_Tables = new Hashtable();
            IFeature pFeat = null;
            DBHelper DBHelperClass = new DBHelper();
            //Start Editing
            IWorkspaceEdit pWorkSpaceEdit = null;
            bool blFeatUpdated = false;

            try
            {

                //Feature update issue - Update this from a value from config 
                _bwriteExtensivelog = true;

                pWorkSpaceEdit = (IWorkspaceEdit)pWorkspace;
                pWorkSpaceEdit.StartEditing(true);
                pWorkSpaceEdit.StartEditOperation();

                sQuery = "SELECT * FROM  " + ReadConfigurations.UnprocessedTableName + " where Processed in ('N','T') AND CIRCUITID = '" + sCircuitId + "' and BATCHID = '" + MainClass.Batch_Number + "'";
                pDataTable = DBHelperClass.GetDataTable(sQuery);

                if ((pDataTable == null) || (pDataTable.Rows.Count == 0))
                {
                    return true; 
                }
                TraceFeeder(pWorkspace as IVersion, sCircuitId);


                //App crash issue resolution - Adding below line
                pWorkSpaceEdit.StopEditOperation();
                WriteLine_Info("Stop Edit Operation completed");

                pWorkSpaceEdit.StopEditing(true);
                WriteLine_Info("Stop Editing completed");

                //App crash issue resolution - Wait for 5 seconds
                System.Threading.Thread.Sleep(5000);

                //Feature update issue 
                pWorkSpaceEdit = (IWorkspaceEdit)pWorkspace;
                pWorkSpaceEdit.StartEditing(true);
                WriteLine_Info("Start Editing started again");
                pWorkSpaceEdit.StartEditOperation();
                WriteLine_Info("Start Edit Operation started again");

                for (int i = 0; i < pDataTable.Rows.Count; ++i)
                {
                    Console.WriteLine(i);
                    sRecType = null;
                    sFeatClsName = null;
                    iOid = 0;
                    sPhaseValue = null;
                    pFeatClass = null;
                    pFeat = null;
                    sGuidFeat = null;
                    blFeatUpdated = false;

                    iOid = Convert.ToInt32(pDataTable.Rows[i]["FEATURE_OID"].ToString());
                    sRecType = pDataTable.Rows[i]["REC_TYPE"].ToString();
                    sFeatClsName = pDataTable.Rows[i]["NAME"].ToString();
                    sPhaseValue = pDataTable.Rows[i]["VALUE"].ToString();
                    sPhaseValue = GetPhaseDomainValue(sPhaseValue);
                    sFeederID = pDataTable.Rows[i]["CIRCUITID"].ToString();
                    sGuidFeat = pDataTable.Rows[i]["FEATURE_GUID"].ToString();
                    //To store feature classes
                    if (string.IsNullOrEmpty(sPhaseValue)) { continue; }
                    if (sRecType == ReadConfigurations.ObjectClassType.FeatureClassType)
                    {
                        if (pHT_All_FeatCls.Contains(sFeatClsName) == false)
                        {
                            pFeatClass = ((IFeatureWorkspace)pWorkspace).OpenFeatureClass(sFeatClsName);
                            pHT_All_FeatCls.Add(sFeatClsName, pFeatClass);
                        }
                        else
                        {
                            pFeatClass = (IFeatureClass)pHT_All_FeatCls[sFeatClsName];
                        }

                        try
                        {
                            pFeat = pFeatClass.GetFeature(iOid);
                        }
                        catch
                        {
                            string sError = "No record found in database for this ID ";
                            WriteLine_Error(sError + "," + sFeatClsName + ",OID," + iOid.ToString());
                            //update query 

                            continue;
                        }

                        if (pFeat == null)
                        {

                            string sError = "No record found in database for this ID";
                            WriteLine_Error(sError + " : " + sFeatClsName + ", OID : " + iOid.ToString());
                            //update query 
                            //InsertRecordInDatatable_UnProcessed(ReadConfigurations.ObjectClassType.FeatureClassType, ReadConfigurations.Devices.TransformerClassName, Convert.ToInt32(sOID_Transformer), DAPHIE_PHASE, "E", sFeederID, sPhase_DTR_Mv, sError);

                            continue;
                        }



                        if (pFeat.Fields.FindField(ReadConfigurations.PhasingVerifiedStatusFldName) != -1)
                        {
                            if ((((IDataset)pFeatClass).BrowseName.Trim().ToUpper() == ReadConfigurations.Devices.SwitchClassName.Trim().ToUpper()) || (((IDataset)pFeatClass).BrowseName.Trim().ToUpper() == ReadConfigurations.Devices.DPDClassName.Trim().ToUpper()) ||
                                         (((IDataset)pFeatClass).BrowseName.Trim().ToUpper() == ReadConfigurations.Devices.OPENPOINTClassName.Trim().ToUpper()) || (((IDataset)pFeatClass).BrowseName.Trim().ToUpper() == ReadConfigurations.Devices.FUSEClassName.Trim().ToUpper()))
                            {
                                if (pFeat.get_Value(pFeat.Fields.FindField(ReadConfigurations.PhaseDesignationFieldName)).ToString() != sPhaseValue)
                                {
                                    try
                                    {
                                        //Feature update issue 
                                        if (_bwriteExtensivelog)
                                            WriteLine_Info("Start updating the feature : " + ((IDataset)pFeatClass).BrowseName.Trim().ToUpper() + " : " + pFeat.OID + " PhasingVerifiedStatus=AutoUpdated and Phase=" + sPhaseValue);

                                        string oldPhasingstats = pFeat.get_Value(pFeat.Fields.FindField(ReadConfigurations.PhasingVerifiedStatusFldName)).ToString();
                                        pFeat.set_Value(pFeat.Fields.FindField(ReadConfigurations.PhasingVerifiedStatusFldName), "AutoUpdated");
                                        pFeat.set_Value(pFeat.Fields.FindField(ReadConfigurations.PhaseDesignationFieldName), sPhaseValue);
                                        pFeat.Store();
                                        UpdateNormalPosition(pFeat, pFeat.get_Value(pFeat.Fields.FindField(ReadConfigurations.PhaseDesignationFieldName)).ToString());
                                        blFeatUpdated = true;

                                        //string strUpateQuery = "Update " + ReadConfigurations.UnprocessedTableName + " set Processed='P' where FEATURE_GUID = '" + sGuidFeat + "' AND CIRCUITID = '" + sCircuitId + "' and BATCHID='" + MainClass.Batch_Number + "'";
                                        string strUpateQuery = "Update " + ReadConfigurations.UnprocessedTableName + " set Processed='T' where FEATURE_GUID = '" + sGuidFeat + "' AND CIRCUITID = '" + sCircuitId + "' and BATCHID='" + MainClass.Batch_Number + "'";
                                        DBHelperClass.UpdateQuery(strUpateQuery);

                                        //Feature update issue 
                                        if (_bwriteExtensivelog)
                                            WriteLine_Info("Feature update completed: " + ((IDataset)pFeatClass).BrowseName.Trim().ToUpper() + " : " + pFeat.OID);

                                    }//App crash issue resolution  - adding varibale to catch actual exception                                   
                                    catch (Exception ex)
                                    {
                                        string sError = "Exception Occurred while storing the  feature";
                                        WriteLine_Error(sError + pFeat.OID + ",Class," + ((IDataset)pFeat.Class).BrowseName + ",FeederID," + sCircuitId);
                                        WriteLine_Error(ex.Message + " at " + ex.StackTrace);
                                        string strUpateQuery = "Update " + ReadConfigurations.UnprocessedTableName + " set Processed='E' ,ERROR_MSG='" + sError + "' where FEATURE_GUID = '" + sGuidFeat + "' AND CIRCUITID = '" + sCircuitId + "' and BATCHID='" + MainClass.Batch_Number + "'";
                                        DBHelperClass.UpdateQuery(strUpateQuery);
                                    }
                                }
                                else
                                {
                                    //Feature update issue 
                                    if (_bwriteExtensivelog)
                                        WriteLine_Info("Start updating the feature : " + ((IDataset)pFeatClass).BrowseName.Trim().ToUpper() + " : " + pFeat.OID);

                                    if (UpdateNormalPosition(pFeat, pFeat.get_Value(pFeat.Fields.FindField(ReadConfigurations.PhaseDesignationFieldName)).ToString()) == true)
                                    {
                                        string strUpateQuery = "Update " + ReadConfigurations.UnprocessedTableName + " set Processed='P' where FEATURE_GUID = '" + sGuidFeat + "' AND CIRCUITID = '" + sCircuitId + "' and BATCHID='" + MainClass.Batch_Number + "'";
                                        DBHelperClass.UpdateQuery(strUpateQuery);
                                    }
                                    else
                                    {
                                        string strUpateQuery = "Update " + ReadConfigurations.UnprocessedTableName + " set Processed='I' where FEATURE_GUID = '" + sGuidFeat + "' AND CIRCUITID = '" + sCircuitId + "' and BATCHID='" + MainClass.Batch_Number + "'";
                                        DBHelperClass.UpdateQuery(strUpateQuery);
                                    }

                                    //Feature update issue 
                                    if (_bwriteExtensivelog)
                                        WriteLine_Info("Feature update completed: " + ((IDataset)pFeatClass).BrowseName.Trim().ToUpper() + " : " + pFeat.OID);
                                }

                            }
                            else
                            {
                                if (pFeat.get_Value(pFeat.Fields.FindField(ReadConfigurations.PhaseDesignationFieldName)).ToString() != sPhaseValue)
                                {
                                    try
                                    {
                                        //Feature update issue 
                                        if (_bwriteExtensivelog)
                                            WriteLine_Info("Start updating the feature : " + ((IDataset)pFeatClass).BrowseName.Trim().ToUpper() + " : " + pFeat.OID + " PhasingVerifiedStatus=AutoUpdated and Phase=" + sPhaseValue);

                                        string oldPhasingstats = pFeat.get_Value(pFeat.Fields.FindField(ReadConfigurations.PhasingVerifiedStatusFldName)).ToString();
                                        pFeat.set_Value(pFeat.Fields.FindField(ReadConfigurations.PhasingVerifiedStatusFldName), "AutoUpdated");
                                        pFeat.set_Value(pFeat.Fields.FindField(ReadConfigurations.PhaseDesignationFieldName), sPhaseValue);
                                        pFeat.Store();
                                        blFeatUpdated = true;

                                        string strUpateQuery = "Update " + ReadConfigurations.UnprocessedTableName + " set Processed='T' where FEATURE_GUID = '" + sGuidFeat + "' AND CIRCUITID = '" + sCircuitId + "' and BATCHID='" + MainClass.Batch_Number + "'";
                                        DBHelperClass.UpdateQuery(strUpateQuery);

                                        //Feature update issue 
                                        if (_bwriteExtensivelog)
                                            WriteLine_Info("Feature update completed: " + ((IDataset)pFeatClass).BrowseName.Trim().ToUpper() + " : " + pFeat.OID);

                                    }//App crash issue resolution  - adding varibale to catch actual exception
                                    catch (Exception ex)
                                    {
                                        string sError = "Exception Occurred while storing the  feature";
                                        WriteLine_Error(sError + pFeat.OID + ",Class," + ((IDataset)pFeat.Class).BrowseName + ",FeederID," + sCircuitId);
                                        WriteLine_Error(ex.Message + " at " + ex.StackTrace);
                                        string strUpateQuery = "Update " + ReadConfigurations.UnprocessedTableName + " set Processed='E' ,ERROR_MSG='" + sError + "' where FEATURE_GUID = '" + sGuidFeat + "' AND CIRCUITID = '" + sCircuitId + "' and BATCHID='" + MainClass.Batch_Number + "'";
                                        DBHelperClass.UpdateQuery(strUpateQuery);
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (pFeat.get_Value(pFeat.Fields.FindField(ReadConfigurations.PhaseDesignationFieldName)).ToString() != sPhaseValue)
                            {
                                try
                                {
                                    //Feature update issue 
                                    if (_bwriteExtensivelog)
                                        WriteLine_Info("Start updating the feature : " + ((IDataset)pFeatClass).BrowseName.Trim().ToUpper() + " : " + pFeat.OID + " PhasingVerifiedStatus=AutoUpdated and Phase=" + sPhaseValue);

                                    pFeat.set_Value(pFeat.Fields.FindField(ReadConfigurations.PhaseDesignationFieldName), sPhaseValue);
                                    pFeat.Store();
                                    blFeatUpdated = true;

                                    string strUpateQuery = "Update " + ReadConfigurations.UnprocessedTableName + " set Processed='T' where FEATURE_GUID = '" + sGuidFeat + "'AND CIRCUITID = '" + sCircuitId + "' and BATCHID='" + MainClass.Batch_Number + "'";
                                    DBHelperClass.UpdateQuery(strUpateQuery);

                                    //Feature update issue 
                                    if (_bwriteExtensivelog)
                                        WriteLine_Info("Feature update completed: " + ((IDataset)pFeatClass).BrowseName.Trim().ToUpper() + " : " + pFeat.OID);

                                }//App crash issue resolution  - adding varibale to catch actual exception
                                catch (Exception ex)
                                {
                                    string sError = "Exception Occurred while storing the feature";
                                    WriteLine_Error(sError + pFeat.OID + ",Class," + ((IDataset)pFeat.Class).BrowseName + ",FeederID," + sCircuitId);
                                    WriteLine_Error(ex.Message + " at " + ex.StackTrace);
                                    string strUpateQuery = "Update " + ReadConfigurations.UnprocessedTableName + " set Processed='E' ,ERROR_MSG='" + sError + "' where FEATURE_GUID = '" + sGuidFeat + "'AND CIRCUITID = '" + sCircuitId + "' and BATCHID='" + MainClass.Batch_Number + "'";
                                }
                            }
                            if ((((IDataset)pFeatClass).BrowseName.Trim().ToUpper() == ReadConfigurations.Devices.SwitchClassName.Trim().ToUpper()) || (((IDataset)pFeatClass).BrowseName.Trim().ToUpper() == ReadConfigurations.Devices.DPDClassName.Trim().ToUpper()) ||
                                        (((IDataset)pFeatClass).BrowseName.Trim().ToUpper() == ReadConfigurations.Devices.OPENPOINTClassName.Trim().ToUpper()) || (((IDataset)pFeatClass).BrowseName.Trim().ToUpper() == ReadConfigurations.Devices.FUSEClassName.Trim().ToUpper()))
                            {

                                //Feature update issue 
                                if (_bwriteExtensivelog)
                                    WriteLine_Info("Start updating the feature : " + ((IDataset)pFeatClass).BrowseName.Trim().ToUpper() + " : " + pFeat.OID);

                                if (UpdateNormalPosition(pFeat, pFeat.get_Value(pFeat.Fields.FindField(ReadConfigurations.PhaseDesignationFieldName)).ToString()) == true)
                                {

                                }

                                //Feature update issue 
                                if (_bwriteExtensivelog)
                                    WriteLine_Info("Feature update completed: " + ((IDataset)pFeatClass).BrowseName.Trim().ToUpper() + " : " + pFeat.OID);
                            }
                        }
                    }

                    if (blFeatUpdated == true)
                    {
                        iUpdatedFeatureCount++;

                        //after 1000 features, save edits and then start editing again
                        if (iUpdatedFeatureCount % 1000 == 0)
                        {
                            if (pWorkSpaceEdit != null)
                            {
                                if (pWorkSpaceEdit.IsBeingEdited() == true)
                                {
                                    try
                                    {
                                        pWorkSpaceEdit.StopEditOperation();
                                    }//App crash issue resolution  - adding varibale to catch actual exception
                                    catch (Exception ex)
                                    {
                                        WriteLine_Error(ex.Message + " at " + ex.StackTrace);
                                    }
                                    pWorkSpaceEdit.StopEditing(true);
                                    string strUpateQuery = "Update " + ReadConfigurations.UnprocessedTableName + " set Processed='P' where Processed = 'T' AND CIRCUITID = '" + sCircuitId + "' and BATCHID='" + MainClass.Batch_Number + "'";
                                    DBHelperClass.UpdateQuery(strUpateQuery);

                                    pWorkSpaceEdit.StartEditing(true);
                                    pWorkSpaceEdit.StartEditOperation();
                                }
                            }
                        }

                    }

                }
                try
                {
                    if (pWorkSpaceEdit != null)
                    {
                        if (pWorkSpaceEdit.IsBeingEdited() == true)
                        {
                            try
                            {
                                pWorkSpaceEdit.StopEditOperation();
                            }//App crash issue resolution  - adding varibale to catch actual exception
                            catch (Exception ex)
                            {
                                WriteLine_Error(ex.Message + " at " + ex.StackTrace);
                            }
                            TraceFeeder(pWorkspace as IVersion, sCircuitId);
                            pWorkSpaceEdit.StopEditing(true);
                            string strUpateQuery = "Update " + ReadConfigurations.UnprocessedTableName + " set Processed='P' where Processed = 'T' AND CIRCUITID = '" + sCircuitId + "' and BATCHID='" + MainClass.Batch_Number + "'";
                            DBHelperClass.UpdateQuery(strUpateQuery);
                        }
                    }
                }
                //App crash issue resolution  - adding varibale to catch actual exception
                catch (Exception ex)
                {
                    WriteLine_Error(ex.Message + " at " + ex.StackTrace);
                }

                return true;
            }
            catch (Exception ex)
            {
                WriteLine_Error(ex.Message + " at " + ex.StackTrace);
                ErrorCodeException ece = new ErrorCodeException(ex);
                Environment.ExitCode = ece.CodeNumber;
                return false;
            }
            finally
            {
                if (pWorkSpaceEdit != null)
                {
                    if (pWorkSpaceEdit.IsBeingEdited() == true)
                    {
                        pWorkSpaceEdit.StopEditOperation();
                        pWorkSpaceEdit.StopEditing(true);
                    }
                }
            }
        }

        private bool UpdateNormalPosition(IFeature objFeature, string FeaturePhaseDesignation)
        {
            bool retval = false;
            try
            {

                int idx_A = objFeature.Fields.FindField(ReadConfigurations.NORMALPOSITION_A_FieldName);
                int idx_B = objFeature.Fields.FindField(ReadConfigurations.NORMALPOSITION_B_FieldName);
                int idx_C = objFeature.Fields.FindField(ReadConfigurations.NORMALPOSITION_C_FieldName);
                int idx_NormalPosition = objFeature.Fields.FindField(ReadConfigurations.NORMALPOSITION_FieldName);
                switch (FeaturePhaseDesignation)
                {

                    case "7":
                        //For ABC Normal Position either close or open for all.

                        if ((objFeature.get_Value(idx_A).ToString() == ReadConfigurations.NormalPosition.Open) || (objFeature.get_Value(idx_B).ToString() == ReadConfigurations.NormalPosition.Open) || (objFeature.get_Value(idx_C).ToString() == ReadConfigurations.NormalPosition.Open))
                        {
                            if (UpdateNPValue(objFeature, idx_A, idx_B, idx_C, idx_NormalPosition, ReadConfigurations.NormalPosition.Open, ReadConfigurations.NormalPosition.Open, ReadConfigurations.NormalPosition.Open, ReadConfigurations.NormalPosition.Open) == true)
                            {
                                retval = true;
                            }
                        }
                        else if ((objFeature.get_Value(idx_A).ToString() == ReadConfigurations.NormalPosition.Close) || (objFeature.get_Value(idx_B).ToString() == ReadConfigurations.NormalPosition.Close) || (objFeature.get_Value(idx_C).ToString() == ReadConfigurations.NormalPosition.Close))
                        {
                            if (UpdateNPValue(objFeature, idx_A, idx_B, idx_C, idx_NormalPosition, ReadConfigurations.NormalPosition.Close, ReadConfigurations.NormalPosition.Close, ReadConfigurations.NormalPosition.Close, ReadConfigurations.NormalPosition.Close) == true)
                            {
                                retval = true;
                            }
                        }
                        break;
                    case "6":
                        //For AB Normal Position A and B either close or open and C is Not Applicable.
                        if ((objFeature.get_Value(idx_A).ToString() == ReadConfigurations.NormalPosition.Open) || (objFeature.get_Value(idx_B).ToString() == ReadConfigurations.NormalPosition.Open) || (objFeature.get_Value(idx_C).ToString() == ReadConfigurations.NormalPosition.Open))
                        {
                            if (UpdateNPValue(objFeature, idx_A, idx_B, idx_C, idx_NormalPosition, ReadConfigurations.NormalPosition.Open, ReadConfigurations.NormalPosition.Open, ReadConfigurations.NormalPosition.NotApplicable, ReadConfigurations.NormalPosition.NotApplicable) == true)
                            {
                                retval = true;
                            }

                        }
                        else if ((objFeature.get_Value(idx_A).ToString() == ReadConfigurations.NormalPosition.Close) || (objFeature.get_Value(idx_B).ToString() == ReadConfigurations.NormalPosition.Close) || (objFeature.get_Value(idx_C).ToString() == ReadConfigurations.NormalPosition.Close))
                        {
                            if (UpdateNPValue(objFeature, idx_A, idx_B, idx_C, idx_NormalPosition, ReadConfigurations.NormalPosition.Close, ReadConfigurations.NormalPosition.Close, ReadConfigurations.NormalPosition.NotApplicable, ReadConfigurations.NormalPosition.NotApplicable) == true)
                            {
                                retval = true;
                            }

                        }

                        break;
                    case "5":
                        //For AC Normal Position A and C either close or open and B is Not Applicable.
                        if ((objFeature.get_Value(idx_A).ToString() == ReadConfigurations.NormalPosition.Open) || (objFeature.get_Value(idx_B).ToString() == ReadConfigurations.NormalPosition.Open) || (objFeature.get_Value(idx_C).ToString() == ReadConfigurations.NormalPosition.Open))
                        {
                            if (UpdateNPValue(objFeature, idx_A, idx_B, idx_C, idx_NormalPosition, ReadConfigurations.NormalPosition.Open, ReadConfigurations.NormalPosition.NotApplicable, ReadConfigurations.NormalPosition.Open, ReadConfigurations.NormalPosition.NotApplicable) == true)
                            {
                                retval = true;
                            }
                        }
                        else if ((objFeature.get_Value(idx_A).ToString() == ReadConfigurations.NormalPosition.Close) || (objFeature.get_Value(idx_B).ToString() == ReadConfigurations.NormalPosition.Close) || (objFeature.get_Value(idx_C).ToString() == ReadConfigurations.NormalPosition.Close))
                        {
                            if (UpdateNPValue(objFeature, idx_A, idx_B, idx_C, idx_NormalPosition, ReadConfigurations.NormalPosition.Close, ReadConfigurations.NormalPosition.NotApplicable, ReadConfigurations.NormalPosition.Close, ReadConfigurations.NormalPosition.NotApplicable) == true)
                            {
                                retval = true;
                            }
                        }
                        break;
                    case "4":
                        //For A Normal Position A is either close or open and B/C is Not Applicable.
                        if ((objFeature.get_Value(idx_A).ToString() == ReadConfigurations.NormalPosition.Open) || (objFeature.get_Value(idx_B).ToString() == ReadConfigurations.NormalPosition.Open) || (objFeature.get_Value(idx_C).ToString() == ReadConfigurations.NormalPosition.Open))
                        {
                            if (UpdateNPValue(objFeature, idx_A, idx_B, idx_C, idx_NormalPosition, ReadConfigurations.NormalPosition.Open, ReadConfigurations.NormalPosition.NotApplicable, ReadConfigurations.NormalPosition.NotApplicable, ReadConfigurations.NormalPosition.NotApplicable) == true)
                            {
                                retval = true;
                            }
                        }
                        else if ((objFeature.get_Value(idx_A).ToString() == ReadConfigurations.NormalPosition.Close) || (objFeature.get_Value(idx_B).ToString() == ReadConfigurations.NormalPosition.Close) || (objFeature.get_Value(idx_C).ToString() == ReadConfigurations.NormalPosition.Close))
                        {
                            if (UpdateNPValue(objFeature, idx_A, idx_B, idx_C, idx_NormalPosition, ReadConfigurations.NormalPosition.Close, ReadConfigurations.NormalPosition.NotApplicable, ReadConfigurations.NormalPosition.NotApplicable, ReadConfigurations.NormalPosition.NotApplicable) == true)
                            {
                                retval = true;
                            }
                        }
                        break;
                    case "3":
                        //For BC Normal Position B and C either close or open and A is Not Applicable.
                        if ((objFeature.get_Value(idx_A).ToString() == ReadConfigurations.NormalPosition.Open) || (objFeature.get_Value(idx_B).ToString() == ReadConfigurations.NormalPosition.Open) || (objFeature.get_Value(idx_C).ToString() == ReadConfigurations.NormalPosition.Open))
                        {

                            if (UpdateNPValue(objFeature, idx_A, idx_B, idx_C, idx_NormalPosition, ReadConfigurations.NormalPosition.NotApplicable, ReadConfigurations.NormalPosition.Open, ReadConfigurations.NormalPosition.Open, ReadConfigurations.NormalPosition.NotApplicable) == true)
                            {
                                retval = true;
                            }

                        }
                        else if ((objFeature.get_Value(idx_A).ToString() == ReadConfigurations.NormalPosition.Close) || (objFeature.get_Value(idx_B).ToString() == ReadConfigurations.NormalPosition.Close) || (objFeature.get_Value(idx_C).ToString() == ReadConfigurations.NormalPosition.Close))
                        {

                            if (UpdateNPValue(objFeature, idx_A, idx_B, idx_C, idx_NormalPosition, ReadConfigurations.NormalPosition.NotApplicable, ReadConfigurations.NormalPosition.Close, ReadConfigurations.NormalPosition.Close, ReadConfigurations.NormalPosition.NotApplicable) == true)
                            {
                                retval = true;
                            }

                        }
                        break;
                    case "2":
                        //For B Normal Position B is either close or open and A/C is Not Applicable.
                        if ((objFeature.get_Value(idx_A).ToString() == ReadConfigurations.NormalPosition.Open) || (objFeature.get_Value(idx_B).ToString() == ReadConfigurations.NormalPosition.Open) || (objFeature.get_Value(idx_C).ToString() == ReadConfigurations.NormalPosition.Open))
                        {
                            if (UpdateNPValue(objFeature, idx_A, idx_B, idx_C, idx_NormalPosition, ReadConfigurations.NormalPosition.NotApplicable, ReadConfigurations.NormalPosition.Open, ReadConfigurations.NormalPosition.NotApplicable, ReadConfigurations.NormalPosition.NotApplicable) == true)
                            {
                                retval = true;
                            }

                        }
                        else if ((objFeature.get_Value(idx_A).ToString() == ReadConfigurations.NormalPosition.Close) || (objFeature.get_Value(idx_B).ToString() == ReadConfigurations.NormalPosition.Close) || (objFeature.get_Value(idx_C).ToString() == ReadConfigurations.NormalPosition.Close))
                        {
                            if (UpdateNPValue(objFeature, idx_A, idx_B, idx_C, idx_NormalPosition, ReadConfigurations.NormalPosition.NotApplicable, ReadConfigurations.NormalPosition.Close, ReadConfigurations.NormalPosition.NotApplicable, ReadConfigurations.NormalPosition.NotApplicable) == true)
                            {
                                retval = true;
                            }

                        }
                        break;
                    case "1":
                        //For C Normal Position C is either close or open and A/B is Not Applicable.
                        if ((objFeature.get_Value(idx_A).ToString() == ReadConfigurations.NormalPosition.Open) || (objFeature.get_Value(idx_B).ToString() == ReadConfigurations.NormalPosition.Open) || (objFeature.get_Value(idx_C).ToString() == ReadConfigurations.NormalPosition.Open))
                        {
                            if (UpdateNPValue(objFeature, idx_A, idx_B, idx_C, idx_NormalPosition, ReadConfigurations.NormalPosition.NotApplicable, ReadConfigurations.NormalPosition.NotApplicable, ReadConfigurations.NormalPosition.Open, ReadConfigurations.NormalPosition.NotApplicable) == true)
                            {
                                retval = true;
                            }

                        }
                        else if ((objFeature.get_Value(idx_A).ToString() == ReadConfigurations.NormalPosition.Close) || (objFeature.get_Value(idx_B).ToString() == ReadConfigurations.NormalPosition.Close) || (objFeature.get_Value(idx_C).ToString() == ReadConfigurations.NormalPosition.Close))
                        {
                            if (UpdateNPValue(objFeature, idx_A, idx_B, idx_C, idx_NormalPosition, ReadConfigurations.NormalPosition.NotApplicable, ReadConfigurations.NormalPosition.NotApplicable, ReadConfigurations.NormalPosition.Close, ReadConfigurations.NormalPosition.NotApplicable) == true)
                            {
                                retval = true;
                            }

                        }
                        break;

                }


            }
            catch (Exception ex)
            {
                WriteLine_Error(ex.Message + " at " + ex.StackTrace);
            }

            return retval;
        }

        private bool UpdateNPValue(IFeature objFeature, int idx_A, int idx_B, int idx_C, int idx_NormalPosition, string Position_A, string Position_B, string Position_C, string Position_NP)
        {
            bool bupdate = false;
            try
            {

                // string ss= objFeature.get_Value(idx_A).ToString();
                if (objFeature.get_Value(idx_A).ToString() != Position_A)
                {
                    objFeature.set_Value(idx_A, Position_A);
                    bupdate = true;
                }
                if (objFeature.get_Value(idx_B).ToString() != Position_B)
                {
                    objFeature.set_Value(idx_B, Position_B);
                    bupdate = true;
                }
                if (objFeature.get_Value(idx_C).ToString() != Position_C)
                {
                    objFeature.set_Value(idx_C, Position_C);
                    bupdate = true;
                }

                if (bupdate == true)
                {
                    objFeature.Store();
                }
            }
            catch (Exception ex)
            {
                WriteLine_Error(ex.Message + " at " + ex.StackTrace);
            }
            return bupdate;

        }

        internal string GetOIDFromGUID(string sGUID, string sClass_ViewName)
        {
            DBHelper DBHelperClass = new DBHelper();
            string _retval = null;
            DataTable pDT = new DataTable();
            try
            {
                string strquery = "select OBJECTID from " + sClass_ViewName + " where GLOBALID  = '" + sGUID + "'";
                DataTable dt = DBHelperClass.GetDataTableByQuery(strquery);
                if ((dt != null) && (dt.Rows.Count > 0))
                {
                    _retval = dt.Rows[0].ItemArray[0].ToString();

                }
            }
            catch (Exception ex)
            {
                _retval = string.Empty;
                WriteLine_Error(ex.Message + " at " + ex.StackTrace);
            }
            return _retval;
        }

        internal void CheckDeEnerzige(IFeature pFeature, ref DataTable DeEnergizeList, string sCircuitID)
        {
            try
            {

                IFeederInfo<FeatureKey> feederInfo = GetFeederInfo(pFeature as IRow);
                string EnergizedPhase = feederInfo.EnergizedPhases.ToString();
                if (EnergizedPhase == "None")
                {
                    InsertValueInDeEnergizeList(pFeature, ref DeEnergizeList, sCircuitID);
                }
                else { }
            }
            catch (Exception ex)
            {
                WriteLine_Error(ex.Message + " at " + ex.StackTrace);
            }
        }



        /// <summary>
        /// Returns the Feeder information for a provided row object
        /// </summary>
        /// <param name="row">IRow to obtain feeder information for</param>
        /// <returns></returns>
        public IFeederInfo<FeatureKey> GetFeederInfo(IRow row)
        {
            Dictionary<IWorkspace, FeederInfoProvider> feederInfoProviders = new Dictionary<IWorkspace, FeederInfoProvider>();

            IFeederInfo<FeatureKey> feederInfo = null;
            try
            {
                ITable pRowTable = row.Table as ITable;
                IDataset ds = pRowTable as IDataset;
                if (ds != null && ds.Workspace != null)
                {
                    if (!feederInfoProviders.ContainsKey(ds.Workspace))
                    {
                        IConnectionProperties connectionProps = new ConnectionProperties(ds.Workspace);
                        feederInfoProviders.Add(ds.Workspace, new FeederInfoProvider(connectionProps));
                    }

                    if (feederInfoProviders.ContainsKey(ds.Workspace))
                    {
                        FeatureKey featKey = new FeatureKey(((IObjectClass)pRowTable).ObjectClassID, row.OID);
                        feederInfo = feederInfoProviders[ds.Workspace].GetFeederInfo(featKey);
                    }
                }
            }
            catch (Exception ex)
            {
                /*Failed to obtain via feeder info.*/
                //_logger.Error("Failed to determine Feeder information: " + ex.Message + "\nStackTrace:" + ex.StackTrace);
                WriteLine_Error("Failed to determine Feeder information: " + ex.Message + "\nStackTrace:" + ex.StackTrace);
            }

            return feederInfo;
        }
        /// <summary>
        /// This will trace all feeders specified in the input file against the version created for this execution
        /// </summary>
        /// <param name="version"></param>
        public void TraceFeeder(IVersion version, string sCircuitID)
        {
            IMMFeederSpace feederSpace = null;
            IMMEnumFeederSource feederSources = null;
            IMMFeederTracer feederTracer = new MMFeederTracer();
            try
            {
                if (ConfigurationManager.AppSettings["TRACE_FEEDER_REQUIRED"].ToString().ToUpper() == ("false").ToUpper())
                {
                    return;
                }
                WriteLine_Info("Tracing specified feeders for CircuitID " + sCircuitID + " at," + DateTime.Now);

                IWorkspace versionWorkspace = version as IWorkspace;
                IGeometricNetwork geomNetwork = GetNetwork(versionWorkspace, "EDGIS.ElectricDistNetwork") as IGeometricNetwork;
                IWorkspaceEdit wsEdit = version as IWorkspaceEdit;
                // if (wsEdit.IsBeingEdited()) { wsEdit.StartEditing(false); }

                Type type = Type.GetTypeFromProgID("mmFramework.MMFeederExt");
                object obj = Activator.CreateInstance(type);
                IMMFeederExt feederExt = obj as IMMFeederExt;
                feederSpace = feederExt.get_FeederSpace(geomNetwork, false);
                feederSources = feederSpace.FeederSources;
                feederTracer = new MMFeederTracer();
                feederSources.Reset();
                //int feederCount = 1;

                string FeederID = sCircuitID;

                IMMFeederSource feederSource = feederSources.get_FeederSourceByFeederID(FeederID);
                if (feederSource != null)
                {

                    WriteLine_Info("Tracing --- FeederID: " + feederSource.FeederID + " FeatureClassID: " + feederSource.FeatureClassID + " at," + DateTime.Now);

                    //Now we can trace this feeder
                    feederTracer.TraceFeeder(geomNetwork, feederSource);
                    WriteLine_Info("Finished tracing feeder " + feederSource.FeederID + " at," + DateTime.Now);
                }
                if (feederSource != null) { while (Marshal.ReleaseComObject(feederSource) > 0) { } }


            }
            catch (Exception ex)
            {
                WriteLine_Error(ex.Message + " at " + ex.StackTrace);
            }
            finally
            {
                if (feederSpace != null) { while (Marshal.ReleaseComObject(feederSpace) > 0);}
                if (feederSources != null) { while (Marshal.ReleaseComObject(feederSources) > 0);}
                if (feederTracer != null) { while (Marshal.ReleaseComObject(feederTracer) > 0);}
            }
        }
        /// <summary>
        /// This method will return the INetwork with the specified network name from the specified workspace
        /// </summary>
        /// <param name="networkName"></param>
        /// <returns></returns>
        public IGeometricNetwork GetNetwork(IWorkspace ws, string networkName)
        {
            IGeometricNetwork network = null;
            try
            {
                IEnumDataset enumDataset = ws.get_Datasets(esriDatasetType.esriDTFeatureDataset);
                enumDataset.Reset();
                IDataset dsName = enumDataset.Next();
                while (dsName != null)
                {
                    IFeatureDataset featDataset = dsName as IFeatureDataset;
                    if (featDataset != null)
                    {
                        IEnumDataset geomDatasets = featDataset.Subsets;
                        geomDatasets.Reset();
                        IDataset geomDataset = geomDatasets.Next();
                        while (geomDataset != null)
                        {
                            if (geomDataset.BrowseName.ToUpper() == networkName.ToUpper())
                            {
                                network = geomDataset as IGeometricNetwork;
                                break;
                            }
                            geomDataset = geomDatasets.Next();
                        }
                    }

                    if (network != null) { break; }
                    dsName = enumDataset.Next();
                }

                if (network != null)
                {
                    return network;
                }
                else
                {
                    throw new Exception("Could not find the specified geometric network in the specified database");
                }
            }
            catch (Exception ex)
            {
                WriteLine_Error(ex.Message + " at " + ex.StackTrace);
                return network;
            }
        }

        internal bool StoreFeatures_RelatedUnits(IWorkspace pWorkspace, string sCircuitId)
        {
            string sQuery = null;
            DataTable pDataTable = new DataTable();
            string sRecType = null;
            string sFeatClsName = null;
            int iOid = 0;
            int iUpdatedFeatureCount = 0;
            string sPhaseValue = null;
            string sGuidFeat = null;
            ITable pTable = null;
            Hashtable pHT_All_Tables = new Hashtable();
            IRow pRow = null;
            IWorkspaceEdit pWorkSpaceEdit = null;
            DBHelper DBHelperClass = new DBHelper();
            bool blFeatUpdated = false;
            try
            {
                //Start Editing
                pWorkSpaceEdit = (IWorkspaceEdit)pWorkspace;
                pWorkSpaceEdit.StartEditing(true);
                pWorkSpaceEdit.StartEditOperation();

                sQuery = "SELECT * FROM  " + ReadConfigurations.UnprocessedTableName + " WHERE PROCESSED in ('R','S') AND CIRCUITID = '" + sCircuitId + "' and BATCHID='" + MainClass.Batch_Number + "'";
                pDataTable = DBHelperClass.GetDataTable(sQuery);

                if ((pDataTable == null) || (pDataTable.Rows.Count == 0))
                {
                    return true;
                }

                for (int i = 0; i < pDataTable.Rows.Count; ++i)
                {
                    Console.WriteLine(i);
                    sRecType = null;
                    sFeatClsName = null;
                    iOid = 0;
                    sPhaseValue = null;
                    pRow = null;
                    sGuidFeat = null;
                    blFeatUpdated = false;

                    iOid = Convert.ToInt32(pDataTable.Rows[i]["FEATURE_OID"].ToString());
                    sRecType = pDataTable.Rows[i]["REC_TYPE"].ToString();
                    sFeatClsName = pDataTable.Rows[i]["NAME"].ToString();
                    sPhaseValue = pDataTable.Rows[i]["VALUE"].ToString();
                    sPhaseValue = GetPhaseDomainValue(sPhaseValue);
                    sGuidFeat = pDataTable.Rows[i]["FEATURE_GUID"].ToString();

                    if (sRecType == ReadConfigurations.ObjectClassType.TableType)
                    {
                        if (pHT_All_Tables.Contains(sFeatClsName) == false)
                        {
                            pTable = ((IFeatureWorkspace)pWorkspace).OpenTable(sFeatClsName);
                            pHT_All_Tables.Add(sFeatClsName, pTable);
                        }
                        else
                        {
                            pTable = (ITable)pHT_All_Tables[sFeatClsName];
                        }

                        try
                        {
                            pRow = pTable.GetRow(iOid);
                        }
                        catch
                        {
                            string sError = "Record is not found in table : " + sFeatClsName + ", OID : " + iOid.ToString();
                            WriteLine_Error(sError);
                            //update query 
                            //InsertRecordInDatatable_UnProcessed(ReadConfigurations.ObjectClassType.FeatureClassType, ReadConfigurations.Devices.TransformerClassName, Convert.ToInt32(sOID_Transformer), DAPHIE_PHASE, "E", sFeederID, sPhase_DTR_Mv, sError);

                            continue;
                        }

                        if (pRow == null)
                        {
                            string sError = "Record is not found in table : " + sFeatClsName + ", OID : " + iOid.ToString();
                            WriteLine_Error(sError);
                            //update query 
                            //InsertRecordInDatatable_UnProcessed(ReadConfigurations.ObjectClassType.FeatureClassType, ReadConfigurations.Devices.TransformerClassName, Convert.ToInt32(sOID_Transformer), DAPHIE_PHASE, "E", sFeederID, sPhase_DTR_Mv, sError);

                            continue;
                        }
                        try
                        {
                            pRow.set_Value(pRow.Fields.FindField(ReadConfigurations.PhaseDesignationFieldName), sPhaseValue);
                            pRow.Store();
                            blFeatUpdated = true;

                            string strUpateQuery = "Update " + ReadConfigurations.UnprocessedTableName + " set Processed='S' where FEATURE_GUID = '" + sGuidFeat + "' AND CIRCUITID = '" + sCircuitId + "' and BATCHID='" + MainClass.Batch_Number + "'";
                            DBHelperClass.UpdateQuery(strUpateQuery);
                        }

                        catch (Exception ex)
                        {
                            string sError = "Exception Occurred while storing the feature.Exception :-  " + ex.Message.ToString();
                            WriteLine_Error(sError + pRow.OID + ",Class," + ((IDataset)pRow.Table).BrowseName + ",FeederID," + sCircuitId);
                            string strUpateQuery = "Update " + ReadConfigurations.UnprocessedTableName + " set Processed='E' ,ERROR_MSG='" + sError + "' where FEATURE_GUID = '" + sGuidFeat + "' AND CIRCUITID = '" + sCircuitId + "' and BATCHID='" + MainClass.Batch_Number + "'";
                            DBHelperClass.UpdateQuery(strUpateQuery);
                        }

                    }

                    if (blFeatUpdated == true)
                    {
                        iUpdatedFeatureCount++;

                        //after 1000 features, save edits and then start editing again
                        if (iUpdatedFeatureCount % 1000 == 0)
                        {
                            if (pWorkSpaceEdit != null)
                            {
                                if (pWorkSpaceEdit.IsBeingEdited() == true)
                                {
                                    try
                                    {
                                        pWorkSpaceEdit.StopEditOperation();
                                    }
                                    catch { }
                                    pWorkSpaceEdit.StopEditing(true);
                                    string strUpateQuery = "Update " + ReadConfigurations.UnprocessedTableName + " set Processed='P' where Processed = 'S' AND CIRCUITID = '" + sCircuitId + "' and BATCHID='" + MainClass.Batch_Number + "'";
                                    DBHelperClass.UpdateQuery(strUpateQuery);

                                    pWorkSpaceEdit.StartEditing(true);
                                    pWorkSpaceEdit.StartEditOperation();
                                }
                            }
                        }

                    }

                }

                if (pWorkSpaceEdit != null)
                {
                    if (pWorkSpaceEdit.IsBeingEdited() == true)
                    {
                        try
                        {
                            pWorkSpaceEdit.StopEditOperation();
                        }
                        catch { }
                        pWorkSpaceEdit.StopEditing(true);
                        string strUpateQuery = "Update " + ReadConfigurations.UnprocessedTableName + " set Processed='P' where Processed = 'S' AND CIRCUITID = '" + sCircuitId + "' and BATCHID='" + MainClass.Batch_Number + "'";
                        DBHelperClass.UpdateQuery(strUpateQuery);
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                WriteLine_Error(ex.Message + " at " + ex.StackTrace);
                ErrorCodeException ece = new ErrorCodeException(ex);
                Environment.ExitCode = ece.CodeNumber;
                return false;
            }
            finally
            {
                if (pWorkSpaceEdit != null)
                {
                    if (pWorkSpaceEdit.IsBeingEdited() == true)
                    {
                        pWorkSpaceEdit.StopEditOperation();
                        pWorkSpaceEdit.StopEditing(true);
                    }
                }
            }
        }

        internal string ReturnViewNameFromClassName(string ClassName)
        {
            string _retval = string.Empty;
            try
            {
                string[] ViewName = ClassName.Split('.');
                if (ViewName.Length == 1)
                {
                    _retval = "ZZ_MV_" + ClassName;
                }
                else if (ViewName.Length == 2)
                {
                    _retval = ViewName[0] + ".ZZ_MV_" + ViewName[1];
                }

            }
            catch (Exception ex)
            {
                WriteLine_Error(ex.Message + " at " + ex.StackTrace);
            }
            return _retval;
        }



        public string GetPhaseDomainValue(string PhaseValue)
        {
            try
            {
                if (string.IsNullOrEmpty(PhaseValue)) return string.Empty;
                switch (PhaseValue)
                {
                    case "ABC":
                        return "7";
                    case "AB":
                        return "6";
                    case "AC":
                        return "5";
                    case "A":
                        return "4";
                    case "BC":
                        return "3";
                    case "B":
                        return "2";
                    case "C":
                        return "1";
                    case "1":
                        return "C";
                    case "2":
                        return "B";
                    case "3":
                        return "BC";
                    case "4":
                        return "A";
                    case "5":
                        return "AC";
                    case "6":
                        return "AB";
                    case "7":
                        return "ABC";
                }
                return string.Empty;
            }
            catch (Exception ex)
            {
                WriteLine_Error(ex.Message + " at " + ex.StackTrace);
                return string.Empty;
            }
        }

        internal string GetGUIDFromOID(string sOID, string sClass_ViewName, ref string sSubType)
        {
            string _retval = null;
            string strquery = null;
            DataTable pDT = new DataTable();
            try
            {
                sSubType = null;
                if (sClass_ViewName == ReadConfigurations.Conductors_ViewName.TransformerLeadViewName)
                {
                    strquery = "select globalid from " + sClass_ViewName + " where objectid  = '" + sOID + "'";
                }
                else
                {
                    strquery = "select globalid,SUBTYPECD from " + sClass_ViewName + " where objectid  = '" + sOID + "'";
                }
                DBHelper DBHelperClass = new DBHelper();
                DataTable dt = DBHelperClass.GetDataTableByQuery(strquery);
                if ((dt != null) && (dt.Rows.Count > 0))
                {
                    _retval = dt.Rows[0].ItemArray[0].ToString();

                    if (sClass_ViewName != ReadConfigurations.Conductors_ViewName.TransformerLeadViewName)
                    {
                        sSubType = dt.Rows[0].ItemArray[1].ToString();
                    }

                }
            }
            catch (Exception ex)
            {
                _retval = string.Empty;
                WriteLine_Error(ex.Message + " at " + ex.StackTrace);
            }
            return _retval;
        }

        internal void UpdateRelatedRecordsFromStoredProcedure(string sProcedureName)
        {
            DBHelper DBHelperClass = new DBHelper();
            try
            {
                if (DBHelperClass.ExecuteStoredProcedureCommand(sProcedureName, null) == false)
                {
                    WriteLine_Error(" Error in updating the Conductor Info records through Stored Procedure");
                }
            }
            catch (Exception ex)
            {

                WriteLine_Error(" Error in updating the Conductor Info records through Stored Procedure," + ex.Message + " at " + ex.StackTrace);
            }
        }





        internal void UpdateUnProcessedPhaseFromStoredProcedure(string sProcedureName, string sCircuitId, string sUnprocessedTableName)
        {
            var pListParams = new List<OracleParameter>();
            string sCircuitIdsIncldQuotes = null;
            try
            {
                sCircuitIdsIncldQuotes = "'" + sCircuitId + "'";

                OracleParameter pParam = new OracleParameter("sCircuitIds", OracleDbType.Varchar2);
                pParam.Direction = ParameterDirection.Input;
                pParam.Size = 1000;
                pParam.Value = sCircuitIdsIncldQuotes;

                pListParams.Add(pParam);

                OracleParameter pParam1 = new OracleParameter("sUnprocessedTableName", OracleDbType.Varchar2);
                pParam1.Direction = ParameterDirection.Input;
                pParam1.Size = 100;
                pParam1.Value = sUnprocessedTableName;

                pListParams.Add(pParam1);

                OracleParameter pParam2 = new OracleParameter("sBatchNumber", OracleDbType.Varchar2);
                pParam2.Direction = ParameterDirection.Input;
                pParam2.Size = 100;
                pParam2.Value = MainClass.Batch_Number;

                pListParams.Add(pParam2);

                OracleParameter pParam3 = new OracleParameter("iSerialNo", OracleDbType.Int32);
                pParam3.Direction = ParameterDirection.Input;
                //pParam3.Size = 100;
                pParam3.Value = Convert.ToInt32(MainClass.sSerial_No);

                pListParams.Add(pParam3);


                DBHelper DBHelperClass = new DBHelper();
                if (DBHelperClass.ExecuteStoredProcedureCommand_TRUnit(sProcedureName, pListParams) == false)
                {
                    throw new Exception("Error in Updating Unprocessed phase records through Stored Procedure," + sProcedureName);
                }
            }
            catch (Exception ex)
            {
                WriteLine_Error("Error in Updating Unprocessed phase records through Stored Procedure," + sProcedureName + "," + ex.Message + " at " + ex.StackTrace);
                throw ex;
            }
        }

        internal void UpdateOpenPointPhaseFromStoredProcedure(string sProcedureName, string sUnprocessedTableName, string sFeederNetworkTrace_Table)
        {
            var pListParams = new List<OracleParameter>();
            try
            {
                OracleParameter pParam1 = new OracleParameter("sUnprocessedTableName", OracleDbType.Varchar2);
                pParam1.Direction = ParameterDirection.Input;
                pParam1.Size = 100;
                pParam1.Value = sUnprocessedTableName;

                pListParams.Add(pParam1);

                OracleParameter pParam2 = new OracleParameter("sBatchNumber", OracleDbType.Varchar2);
                pParam2.Direction = ParameterDirection.Input;
                pParam2.Size = 100;
                pParam2.Value = MainClass.Batch_Number;

                pListParams.Add(pParam2);

                OracleParameter pParam3 = new OracleParameter("sFeederNetworkTrace_Table", OracleDbType.Varchar2);
                pParam3.Direction = ParameterDirection.Input;
                pParam3.Size = 100;
                pParam3.Value = sFeederNetworkTrace_Table;

                pListParams.Add(pParam3);

                DBHelper DBHelperClass = new DBHelper();
                if (DBHelperClass.ExecuteStoredProcedureCommand_TRUnit(sProcedureName, pListParams) == false)
                {
                    throw new Exception("Error in updating Open Point records through Stored Procedure," + sProcedureName);
                }
            }
            catch (Exception ex)
            {
                WriteLine_Error("Error in updating Open Point records through Stored Procedure," + sProcedureName + "," + ex.Message + " at " + ex.StackTrace);
                throw ex;
            }
        }

        internal string GetDataFromDatatable(string strquery, string sFieldName, DataTable InputdataTable)
        {
            string _retval = string.Empty;
            try
            {
                DataRow[] drd = InputdataTable.Select(strquery);

                if (drd != null)
                {
                    if (drd.Length > 0)
                    {
                        for (int i = 0; i < drd.Length; i++)
                        {
                            DataRow dr = drd[0];
                            _retval = dr[sFieldName].ToString();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                WriteLine_Error("Exception occurred while getting the data from datatable from query : - " + strquery + "   Exception -- " + ex.Message.ToString());
            }
            return _retval;
        }


        internal string getFCISFromFeatureclassName(string sClassName)
        {
            string _retval = null;
            DataTable pDT = new DataTable();
            try
            {
                string strquery = "select OBJECTID from " + ReadConfigurations.GDBITEMSTableName + " where PHYSICALNAME  = '" + sClassName + "'";
                DataTable dt = (new DBHelper()).GetDataTableByQuery(strquery);
                if ((dt != null) && (dt.Rows.Count > 0))
                {
                    _retval = dt.Rows[0].ItemArray[0].ToString();

                }
            }
            catch (Exception ex)
            {
                _retval = string.Empty;
                WriteLine_Error(ex.Message + " at " + ex.StackTrace);
            }
            return _retval;
        }
        #region Update Datatable for  Unprocessed Record
        public void UpdateAttribute(string sExistingStatus, string sExistingPhase, string sClassName, int iFeatOid, string sPhaseToUp_2,
          string TRANS_GUID, string sObjectClassType, string sFeatGuid, string sSubType, string sReviewBy, string ParentGUID, string parentClass)
        {
            try
            {
                if (string.IsNullOrEmpty(sSubType))
                {
                    int i = 0;
                }
                if (!string.IsNullOrEmpty(sExistingStatus))
                {
                    if ((sExistingStatus.ToUpper() != ReadConfigurations.PhasingStatus.FieldVerified) && (sExistingStatus.ToUpper() != ReadConfigurations.PhasingStatus.OfficeVerified))
                    {
                        InsertRecordInDatatable_UnProcessed(sObjectClassType, sClassName, iFeatOid, sPhaseToUp_2, "N", TRANS_GUID, sExistingPhase, string.Empty, sFeatGuid, sSubType, sReviewBy, ParentGUID, parentClass);
                    }
                    else
                    {
                        //Warning
                        if (sExistingStatus.ToUpper() == ReadConfigurations.PhasingStatus.FieldVerified)
                        {
                            //string sWarningMsg = "Feature is having <FIELDVERIFIED> or <OFFICEVERIFIED> status.";
                            //string sWarningMsg = "Feature is having status as Field Verified.";

                            string sWarningMsg = ReadConfigurations.ERROR_MESSAGES.ERROR_CODE_07.Split(':')[0];
                            sReviewBy = ReadConfigurations.ERROR_MESSAGES.ERROR_CODE_07.Split(':')[1];

                            WriteLine_Warn(sWarningMsg + " Feature Class = " + sClassName + " and ObjectId = " + iFeatOid.ToString());
                            InsertRecordInDatatable_UnProcessed(sObjectClassType, sClassName, iFeatOid, sPhaseToUp_2, "V", TRANS_GUID, sExistingPhase, sWarningMsg, sFeatGuid, sSubType, sReviewBy, ParentGUID, parentClass);
                        }
                        else if (sExistingStatus.ToUpper() == ReadConfigurations.PhasingStatus.OfficeVerified)
                        {
                            //string sWarningMsg = "Feature is having <FIELDVERIFIED> or <OFFICEVERIFIED> status.";
                            //string sWarningMsg = "Feature is having status as Office Verified.";

                            string sWarningMsg = ReadConfigurations.ERROR_MESSAGES.ERROR_CODE_08.Split(':')[0];
                            sReviewBy = ReadConfigurations.ERROR_MESSAGES.ERROR_CODE_08.Split(':')[1];

                            WriteLine_Warn(sWarningMsg + " Feature Class = " + sClassName + " and ObjectId = " + iFeatOid.ToString());
                            InsertRecordInDatatable_UnProcessed(sObjectClassType, sClassName, iFeatOid, sPhaseToUp_2, "V", TRANS_GUID, sExistingPhase, sWarningMsg, sFeatGuid, sSubType, sReviewBy, ParentGUID, parentClass);
                        }


                    }
                }
                else
                {
                    InsertRecordInDatatable_UnProcessed(sObjectClassType, sClassName, iFeatOid, sPhaseToUp_2, "N", TRANS_GUID, sExistingPhase, string.Empty, sFeatGuid, sSubType, sReviewBy, ParentGUID, parentClass);
                }
            }
            catch (Exception ex)
            {
                WriteLine_Error("Unhandled exception encountered while updating the  attribute in output table," + ex.Message.ToString() + " at " + ex.StackTrace);
            }
        }

        public void UpdateErrorAttribute(string sErrorMsg, string sExistingPhase, string sClassName, int iFeatOid, string sPhaseToUp_2, string TRANS_GUID,
            string sObjectClassType, string sFeatGuid, string sSubType, string sReviewBy, string ParentGUID, string parentClass)
        {

            try
            {
                if (string.IsNullOrEmpty(sSubType))
                {
                    int i = 0;
                }
                InsertRecordInDatatable_UnProcessed(sObjectClassType, sClassName, iFeatOid, sPhaseToUp_2, "E", TRANS_GUID, sExistingPhase, sErrorMsg, sFeatGuid, sSubType, sReviewBy, ParentGUID, parentClass);
            }
            catch (Exception ex)
            {
                WriteLine_Error("Unhandled exception encountered while updating the error attribute in output table," + ex.Message.ToString() + " at " + ex.StackTrace);
            }
        }

        #endregion

        internal void InsertRecordInDatatable_QAQC(string ObjectClassName, int Feature_OID, string sGuid, string sError, string sCircuitID)
        {
            try
            {
                if (ContainDataRowInDataTable(sGuid, MainClass.g_DT_QAQC, "FEATURE_GUID") == false)
                {

                    DataRow DR = MainClass.g_DT_QAQC.NewRow();

                    DR["NAME"] = ObjectClassName;
                    DR["FEATURE_OID"] = Feature_OID.ToString();
                    DR["FEATURE_GUID"] = sGuid;
                    DR["ERROR"] = sError;
                    DR["CIRCUITID"] = sCircuitID;
                    DR["BATCHID"] = MainClass.Batch_Number;
                    DR["COMMENTS"] = "NA";
                    DR["DE_ENERGIZE_TYPE"] = "";
                    DR["VERSION_NAME"] = "";
                    MainClass.g_DT_QAQC.Rows.Add(DR);
                }
                else
                {
                    DataRow[] drd = MainClass.g_DT_QAQC.Select("FEATURE_GUID = '" + sGuid.ToString() + "'");

                    if (drd != null)
                    {
                        if (drd.Length > 0)
                        {
                            //for (int i = 0; i < drd.Length; i++)
                            //{
                            DataRow DR = drd[0];
                            DR["NAME"] = ObjectClassName;
                            DR["FEATURE_OID"] = Feature_OID.ToString();
                            DR["FEATURE_GUID"] = sGuid;
                            DR["ERROR"] = sError;
                            DR["CIRCUITID"] = sCircuitID;
                            DR["BATCHID"] = MainClass.Batch_Number;
                            DR["COMMENTS"] = "NA";
                            DR["DE_ENERGIZE_TYPE"] = "";
                            DR["VERSION_NAME"] = "";
                        }
                    }


                }

            }
            catch (Exception ex)
            {
                WriteLine_Error("QAQC Record is not inserted in the table," + ObjectClassName + "," + Feature_OID + "," + sError);
                WriteLine_Error(ex.Message + " at " + ex.StackTrace);
            }

        }



        public void ExportToExcel(DataTable dt, string strReportpath, string stableName)
        {
            try
            {
                if (dt.Rows.Count == 0)
                {
                    return;
                }
                System.Data.OleDb.OleDbConnection MyConnection;
                System.Data.OleDb.OleDbCommand myCommand = new System.Data.OleDb.OleDbCommand();
                string sql = null;
                MyConnection = new System.Data.OleDb.OleDbConnection("provider=Microsoft.Jet.OLEDB.4.0;Data Source='" + strReportpath + "';Extended Properties=Excel 8.0;");
                MyConnection.Open();

                myCommand.Connection = MyConnection;
                //Create a Table;;

                int i = 0;
                //making table query
                string strTableQ = "CREATE TABLE [" + stableName + "](";
                int j = 0;
                for (j = 0; j <= dt.Columns.Count - 1; j++)
                {
                    DataColumn dCol = null;
                    dCol = dt.Columns[j];
                    strTableQ += " [" + dCol.ColumnName + "] varchar(255) , ";
                }
                strTableQ = strTableQ.Substring(0, strTableQ.Length - 2);
                strTableQ += ")";
                OleDbCommand cmd = new OleDbCommand(strTableQ, MyConnection);
                cmd.ExecuteNonQuery();

                string strCoumnvalue = "";
                strCoumnvalue += " (";
                //making insert query
                for (int l = 0; l <= dt.Columns.Count - 1; l++)
                {
                    DataColumn dCol = null;
                    dCol = dt.Columns[l];
                    strCoumnvalue += dCol.ColumnName + ", ";

                }
                strCoumnvalue = strCoumnvalue.Substring(0, strCoumnvalue.Length - 2);
                strCoumnvalue += ")";
                string strvalues = "";
                strvalues += " ( ";
                foreach (DataRow dr in dt.Rows)
                {
                    strvalues = "";
                    strvalues += " ( ";
                    for (int k = 0; k < dt.Columns.Count; k++)
                    {

                        strvalues += "'" + dr[k].ToString() + "',";


                    }
                    strvalues = strvalues.Substring(0, strvalues.Length - 1);
                    strvalues += ")";

                    sql = "Insert into " + stableName + " values" + strvalues + "";
                    myCommand.CommandText = sql;
                    try
                    {
                        myCommand.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        WriteLine_Error("Exception occurred while inserting the row ." + ex.Message + " at " + ex.StackTrace);
                    }
                }

                MyConnection.Close();

            }
            catch (Exception ex)
            {
                WriteLine_Error("Exception occurred while exporting the excel.");
                WriteLine_Error(ex.Message + " at " + ex.StackTrace);
            }
        }


        public void ExportDatagridviewToCsv(DataTable pDT_Exceptions, string strReportpath)
        {
            StreamWriter pSWriter = default(StreamWriter);
            string sClassName = null;
            string sOid = null;
            string sError = null;
            try
            {
                pSWriter = File.AppendText(strReportpath);

                for (int i = 0; i < pDT_Exceptions.Rows.Count; ++i)
                {
                    sClassName = null;
                    sOid = null;
                    sError = null;

                    sClassName = pDT_Exceptions.Rows[i]["NAME"].ToString();
                    sOid = pDT_Exceptions.Rows[i]["FEATURE_OID"].ToString();
                    sError = pDT_Exceptions.Rows[i]["ERROR"].ToString();

                    pSWriter.WriteLine(sClassName + "," + sOid + "," + sError);
                }
                pSWriter.Close();
            }
            catch (Exception ex)
            {
                WriteLine_Error("Error in ExportDatagridviewToCsv," + ex.Message + " at " + ex.StackTrace);
            }
        }

        public Boolean CheckIfVersionNotExists(string sSession_Name)
        {
            IFeatureWorkspace fWSpace = null;
            IVersionedWorkspace vWSpace = null;
            IVersion pVersion = null;

            try
            {
                fWSpace = (IFeatureWorkspace)MainClass.m_SDEDefaultworkspace;
                vWSpace = (IVersionedWorkspace)fWSpace;

                pVersion = vWSpace.FindVersion(sSession_Name);
                if (pVersion == null)
                {
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                WriteLine_Error(ex.Message + " at " + ex.StackTrace);
                return true;
            }

        }

        internal bool CheckIfUnprocessedCountInLimit()
        {
            string sQuery = null;
            DataTable pDataTable = new DataTable();
            int iTotalCount = 0;
            int iLimit = 0;
            int iPercent = 0;
            int iLimitAfterIncrement = 0;
            try
            {

                DBHelper DBHelperClass = new DBHelper();
                sQuery = "SELECT COUNT(1) FROM  " + ReadConfigurations.UnprocessedTableName + " WHERE PROCESSED IN ('N','P','R','T','S') AND " + ReadConfigurations.DAPHIETablesFields.BATCH_NUMBER + " IN (SELECT " +
                    ReadConfigurations.DAPHIETablesFields.BATCH_NUMBER + " FROM " +
                    ReadConfigurations.DAPHIEFileDetTableName + " WHERE " + ReadConfigurations.DAPHIETablesFields.Current_StatusField + " IS NOT NULL)";

                pDataTable = DBHelperClass.GetDataTable(sQuery);

                if ((pDataTable == null) || (pDataTable.Rows.Count == 0))
                {
                    return true;
                }


                iTotalCount = Convert.ToInt32(pDataTable.Rows[0][0]);

                //Find the limit number
                iLimit = Convert.ToInt32(ConfigurationManager.AppSettings["MAX_EDITED_FEATURES_ALLOWED"]);
                iPercent = Convert.ToInt32(ConfigurationManager.AppSettings["PERCENTAGE_ALLOWED"]);

                iLimitAfterIncrement = iLimit + (iPercent * iLimit / 100);

                if (iTotalCount > iLimitAfterIncrement)
                {
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                WriteLine_Error(ex.Message + " at " + ex.StackTrace);
                return false;
            }
        }


        public string CheckNormalPosition(string sOID, string sClass_ViewName)
        {
            string _retval = "OPEN";
            try
            {
                string strquery = " select " + ReadConfigurations.NORMALPOSITION_FieldName + "," + ReadConfigurations.NORMALPOSITION_A_FieldName + "," +
                ReadConfigurations.NORMALPOSITION_B_FieldName + "," + ReadConfigurations.NORMALPOSITION_C_FieldName + "," + ReadConfigurations.PhaseDesignationFieldName + "," +
                ReadConfigurations.PhasingVerifiedStatusFldName + " from " + sClass_ViewName + " where " + ReadConfigurations.OBJECTIDFIeldName + "= " + sOID;
                DataTable dt = (new DBHelper()).GetDataTableByQuery(strquery);
                if (dt != null && dt.Rows.Count > 0)
                {
                    string Val_NormalPosition = dt.Rows[0].ItemArray[0].ToString();
                    string Val_A = dt.Rows[0].ItemArray[1].ToString();
                    string Val_B = dt.Rows[0].ItemArray[2].ToString();
                    string Val_C = dt.Rows[0].ItemArray[3].ToString();
                    string sOldPhase = dt.Rows[0].ItemArray[4].ToString();
                    string sExistingStatus = dt.Rows[0].ItemArray[5].ToString();

                    //For ABC Normal Position either close or open for all.

                    if ((Val_A == ReadConfigurations.NormalPosition.Close) || (Val_B == ReadConfigurations.NormalPosition.Close) || (Val_C == ReadConfigurations.NormalPosition.Close))
                    {
                        _retval = "CLOSE";

                    }

                }


            }
            catch (Exception ex)
            {
                WriteLine_Error("Unhandled exception encountered while Checking the normal position of the feature," + ex.Message.ToString() + " at " + ex.StackTrace);
            }
            return _retval;
        }

        #region SendMail


        public void SendMail(string sFileName, string subject, string bodyText, string lanIDs)
        {
            try
            {

                WriteLine_Info("Sending email to " + lanIDs);
                string fromDisplayName = ConfigurationManager.AppSettings["MAIL_FROM_DISPLAY_NAME"];
                string fromAddress = ConfigurationManager.AppSettings["MAIL_FROM_ADDRESS"];


                EmailService.Send(mail =>
                {
                    mail.From = fromAddress;
                    mail.FromDisplayName = fromDisplayName;
                    mail.To = lanIDs;
                    mail.Subject = subject;
                    mail.BodyText = bodyText;
                    if (!string.IsNullOrEmpty(sFileName))
                    {
                        string[] sfile = sFileName.Split(',');
                        if (sfile.Length == 1)
                        {
                            mail.Attachments.Add(sfile[0]);
                        }
                        else if (sfile.Length == 2)
                        {
                            mail.Attachments.Add(sfile[0]);
                            mail.Attachments.Add(sfile[1]);
                        }
                        else
                        {
                            mail.Attachments.Add(sFileName);
                        }
                    }

                });
            }

            catch (Exception ex)
            {
                WriteLine_Error(ex.Message);
            }
        }
        /// <summary>
        /// send mail to daphie if session will cross the de-energize feature threshold limit  or JSON file is invalid  
        /// </summary>
        /// <param name="MailReason"></param>
        /// <param name="sSessionName"></param>
        internal void SendMailToDAPHIE(string MailReason, string scircuitID, string sJSONFilePath)
        {
            try
            {
                string bodyText = string.Empty;
                string subject = string.Empty;
                subject = ConfigurationManager.AppSettings["MAIL_SUBJECT_INVALIDJSON"];
                if (MailReason == ReadConfigurations.STATUS_DAPHIEFILE_DET.Status_DELETED)
                {
                    int PMCount = FindPrimaryFeatureCount();
                    string sFilename = GetFileName();
                    subject = subject + " due to high de-energization cases";
                    bodyText = "Hello Team,  <br /><br />"
                      + "ARAD phase prediction file has been rejected by the EDGIS system due to high number of de-energization cases. <br /> "
                      + "Filename - > " + sFilename + "  <br /> "
                      + "No Of Primary Denergization:  " + Convert.ToString(PMCount) + "  <br />  <br /> "
                      + "Please send the correct file again through ESFT.<br />  <br /> "
                      + "Thank you,  <br />"
                      + " IGP Phase Update Process  <br /><br />"

                      + " [THIS IS AN AUTOMATED MESSAGE - PLEASE DO NOT REPLY DIRECTLY TO THIS EMAIL] ";
                }
                else if (MailReason == ReadConfigurations.STATUS_DAPHIEFILE_DET.status_INVALIDJSON)
                {

                    bodyText = "Hello Team,  <br />"
                        + " Failed to load ARAD phase prediction JSON file in EDGIS system due to format errors.<br /> "
                        + " Filename - > " + sJSONFilePath + "  <br />  <br /> "
                        + " Please send the correct file again through ESFT.<br />  <br /> "
                        + "Thank you,  <br />"
                        + " IGP Phase Update Process  <br /><br />"

                        + " [THIS IS AN AUTOMATED MESSAGE - PLEASE DO NOT REPLY DIRECTLY TO THIS EMAIL] ";
                }
                string lanIDs = ConfigurationManager.AppSettings["MAIL_TO_ADDRESS_DAPHIE"];
                SendMail("", subject, bodyText, lanIDs);
            }
            catch (Exception ex)
            {
                WriteLine_Error("Unhandled exception occurred while sending the mail to DAPHIE  ," + ex.Message.ToString() + " at " + ex.StackTrace);
            }
        }

        private string GetFileName()
        {
            string retval = string.Empty;
            try
            {
                string strquery = "select filename from " + ReadConfigurations.DAPHIEFileDetTableName + "  where batchid='" + MainClass.Batch_Number + "'";
                DataTable DT = (new DBHelper()).GetDataTable(strquery);
                if ((DT != null) && (DT.Rows.Count > 0))
                {
                    if (DT != null)
                    {
                        retval = DT.Rows[0].ItemArray[0].ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                WriteLine_Error("Unhandled exception occurred while fetching the Primary Feature De-Energize Count  ," + ex.Message.ToString() + " at " + ex.StackTrace);
            }
            return retval;
        }
        /// <summary>
        /// For any process error send mail to support team
        /// </summary>
        /// <param name="MailReason"></param>
        /// <param name="sCircuitID"></param>
        /// <param name="Batch_Number"></param>
        /// <param name="sJSONFilePath"></param>
        internal void SendMailToSupportTeam(string MailReason, string sCircuitID, string Batch_Number, string sJSONFilePath)
        {
            try
            {
                string subject = ConfigurationManager.AppSettings["MAIL_SUBJECT_PROCESSERROR"];
                string bodyText = string.Empty;

                if (MailReason == ReadConfigurations.STATUS_DAPHIEFILE_DET.Status_CalculationError)
                {
                    bodyText = "Hello Team,  <br />"
                       + " IGP session has been terminated as process is having calculation error, for more details please see the log file. <br /><br /> "
                       + " CircuitID : " + sCircuitID + " <br />"
                       + " BatchID : " + MainClass.Batch_Number.ToString() + " <br /><br />"

                       + "Thank you,  <br />"
                       + " IGP Phase Update Process  <br /><br />"

                       + " [THIS IS AN AUTOMATED MESSAGE - PLEASE DO NOT REPLY DIRECTLY TO THIS EMAIL] ";
                }
                else if (MailReason == ReadConfigurations.STATUS_DAPHIEFILE_DET.Status_UpdateError)
                {

                    bodyText = "Hello Team,  <br />"
                        + " IGP session has been terminated as process is having updates error, for more details please see the log file. <br /><br /> "
                        + " CircuitID : " + sCircuitID + " <br />"
                        + " BatchID : " + MainClass.Batch_Number.ToString() + " <br /><br />"

                        + "Thank you,  <br />"
                        + " IGP Phase Update Process  <br /><br />"

                        + " [THIS IS AN AUTOMATED MESSAGE - PLEASE DO NOT REPLY DIRECTLY TO THIS EMAIL] ";
                }
                else if (MailReason == ReadConfigurations.STATUS_DAPHIEFILE_DET.Status_ValidateError)
                {
                    bodyText = "Hello Team,  <br />"
                       + " IGP session has been terminated as process is having validation error, for more details please see the log file. <br /><br /> "
                       + " CircuitID : " + sCircuitID + " <br />"
                       + " BatchID : " + MainClass.Batch_Number.ToString() + " <br /><br />"

                       + "Thank you,  <br />"
                       + " IGP Phase Update Process  <br /><br />"

                       + " [THIS IS AN AUTOMATED MESSAGE - PLEASE DO NOT REPLY DIRECTLY TO THIS EMAIL] ";
                }
                else if (MailReason == ReadConfigurations.STATUS_DAPHIEFILE_DET.Status_POSTERROR)
                {
                    bodyText = "Hello Team,  <br />"
                     + " IGP session has been terminated as process is having Post error, for more details please see the log file. <br /><br /> "
                     + " CircuitID : " + sCircuitID + " <br />"
                     + " BatchID : " + MainClass.Batch_Number.ToString() + " <br /><br />"

                     + "Thank you,  <br />"
                     + " IGP Phase Update Process  <br /><br />"

                     + " [THIS IS AN AUTOMATED MESSAGE - PLEASE DO NOT REPLY DIRECTLY TO THIS EMAIL] ";
                }
                else if (MailReason == ReadConfigurations.STATUS_DAPHIEFILE_DET.Status_LoadError)
                {
                    bodyText = "Hello Team,  <br />"
                       + " IGP phase update process has been terminated due to below error, for more details please see the log file. <br /><br /> "
                       + " Error: Input JSON file could not be processed as file is already present in Processed/Success folder.Filename - > " + sJSONFilePath + "  <br />  <br /> "

                       + "Thank you,  <br />"
                       + " IGP Phase Update Process  <br /><br />"

                       + " [THIS IS AN AUTOMATED MESSAGE - PLEASE DO NOT REPLY DIRECTLY TO THIS EMAIL] ";
                }
                else if (MailReason == "GETSEQUENCE_ERROR")
                {
                    bodyText = "Hello Team,  <br />"
                       + " IGP phase update process has been terminated due to below error, for more details please see the log file. <br /><br /> "
                       + " Error: Application has been failed to getting a database sequence  <br />  <br /> "

                       + "Thank you,  <br />"
                       + " IGP Phase Update Process  <br /><br />"

                       + " [THIS IS AN AUTOMATED MESSAGE - PLEASE DO NOT REPLY DIRECTLY TO THIS EMAIL] ";
                }
                else if (MailReason == "FILEVALIDATE_ERROR")
                {
                    bodyText = "Hello Team,  <br />"
                       + " IGP phase update process has been terminated due to below error, for more details please see the log file. <br /><br /> "
                       + " Error: Application has been failed to validate the file through stored procedure <br />  <br /> "

                       + "Thank you,  <br />"
                       + " IGP Phase Update Process  <br /><br />"

                       + " [THIS IS AN AUTOMATED MESSAGE - PLEASE DO NOT REPLY DIRECTLY TO THIS EMAIL] ";
                }
                else if (MailReason == "FILEINSERT_ERROR")
                {
                    bodyText = "Hello Team,  <br />"
                        + " IGP phase update process has been terminated due to below error, for more details please see the log file. <br /><br /> "
                        + " Error: Please see the log file as application is failed to insert the record in Table  " + ReadConfigurations.DAPHIEFileDetTableName + " <br />  <br /> "

                        + "Thank you,  <br />"
                        + " IGP Phase Update Process  <br /><br />"

                        + " [THIS IS AN AUTOMATED MESSAGE - PLEASE DO NOT REPLY DIRECTLY TO THIS EMAIL] ";
                }
                else
                {
                    bodyText = "Hello Team,  <br />"
                    + " IGP session has been terminated as process is having " + MailReason.ToLower().ToString() + " error, for more details please see the log file. <br /><br /> "
                    + " CircuitID : " + sCircuitID + " <br />"
                    + " BatchID : " + MainClass.Batch_Number.ToString() + " <br /><br />"

                    + "Thank you,  <br />"
                    + " IGP Phase Update Process  <br /><br />"

                    + " [THIS IS AN AUTOMATED MESSAGE - PLEASE DO NOT REPLY DIRECTLY TO THIS EMAIL] ";
                }
                string lanIDs = ConfigurationManager.AppSettings["MAIL_TO_ADDRESS_EDGISSupportTeam"];
                SendMail("", subject, bodyText, lanIDs);
            }
            catch (Exception ex)
            {
                WriteLine_Error("Unhandled exception occurred while sending the mail to DAPHIE  ," + ex.Message.ToString() + " at " + ex.StackTrace);
            }
        }

        /// <summary>
        /// send mail to mapper if session has been deleted by user or session is having de-energize features
        /// </summary>
        /// <param name="MailReason"></param>
        /// <param name="MailAttachment"></param>
        /// <param name="SessionID"></param>
        /// <param name="sCircuitID"></param>
        internal void SendMailToMapper(string MailReason, string MailAttachment, int SessionID, string sCircuitID)
        {
            try
            {

                string bodyText = string.Empty;
                string subject = string.Empty;
                string sSessionName = ConfigurationManager.AppSettings["SessionName"] + "_" + SessionID.ToString();
                if (MailReason == ReadConfigurations.STATUS_DAPHIEFILE_DET.Status_CONFLICT)
                {
                    //subject = ConfigurationManager.AppSettings["MAIL_SUBJECT_MAPPER"] + "#SN_" + SessionID + " has Conflict";
                    //EGIS-923 : CHANGE IN subject LINE AND MAIL BODY
                    subject = ConfigurationManager.AppSettings["MAIL_SUBJECT_MAPPER"] + " " + sCircuitID + " has Conflict";
                    bodyText = "Hello Team,  <br />"
                        + " IGP Phase ID update session has CONFLICT. Please resolve the errors and send the session back to GDBM Queue for Posting. <br /><br /> "
                        + " Session ID : " + "SN_" + SessionID + " <br />"
                        + " Session Name : " + sSessionName + " <br />"
                        + " CircuitID : " + sCircuitID + " <br /><br />"

                        + "Thank you,  <br />"
                        + " IGP Phase Update Process  <br /><br />"

                        + " [THIS IS AN AUTOMATED MESSAGE - PLEASE DO NOT REPLY DIRECTLY TO THIS EMAIL] ";

                }
                else if (MailReason == ReadConfigurations.STATUS_DAPHIEFILE_DET.Status_RECONCILEERROR)
                {
                    subject = ConfigurationManager.AppSettings["MAIL_SUBJECT_MAPPER"] + " " + sCircuitID + " has Reconcile Error";
                    bodyText = "Hello Team,  <br />"
                        + " IGP Phase ID update session has RECONCILE ERROR. Please resolve the errors and send the session back to GDBM Queue for Posting. <br /><br /> "
                        + " Session ID : " + "SN_" + SessionID + " <br />"
                        + " Session Name : " + sSessionName + " <br />"
                        + " CircuitID : " + sCircuitID + " <br /><br />"

                        + "Thank you,  <br />"
                        + " IGP Phase Update Process  <br /><br />"

                        + " [THIS IS AN AUTOMATED MESSAGE - PLEASE DO NOT REPLY DIRECTLY TO THIS EMAIL] ";

                }
                else if (MailReason == ReadConfigurations.STATUS_DAPHIEFILE_DET.Status_POSTERROR)
                {
                    subject = ConfigurationManager.AppSettings["MAIL_SUBJECT_MAPPER"] + " " + sCircuitID + " has Post Error";
                    bodyText = "Hello Team,  <br />"
                        + " IGP Phase ID update session has POST ERROR. Please resolve the errors and send the session back to GDBM Queue for Posting. <br /><br /> "
                        + " Session ID : " + "SN_" + SessionID + " <br />"
                        + " Session Name : " + sSessionName + " <br />"
                        + " CircuitID : " + sCircuitID + " <br /><br />"

                        + "Thank you,  <br />"
                        + " IGP Phase Update Process  <br /><br />"

                        + " [THIS IS AN AUTOMATED MESSAGE - PLEASE DO NOT REPLY DIRECTLY TO THIS EMAIL] ";

                }
                else if (MailReason == ReadConfigurations.STATUS_DAPHIEFILE_DET.Status_USERQUEUE)
                {
                    int PMCount = FindPrimaryFeatureCount();
                    int SeCount = FindSecondaryFeatureCount();

                    subject = ConfigurationManager.AppSettings["MAIL_SUBJECT_MAPPER"] + " " + sCircuitID + " has QAQC errors";
                    bodyText = "Hello Team,  <br />"
                        + " IGP Phase ID update session is not posted due to de-energization or QA/QC errors in the network. Please resolve the errors and assign the session back to " + ReadConfigurations.ApplicationUser + " user for further action.. <br /><br /> "
                        + "Please refer enclosed QA/QC logs for more details.<br /><br />"
                        + " Session ID : " + "SN_" + SessionID + " <br />"
                        + " Session Name : " + sSessionName + " <br />"
                        + " CircuitID : " + sCircuitID + " <br /><br />"
                        + "No Of Primary Denergization:  " + Convert.ToString(PMCount) + " <br />"
                        + "No Of Secondary Denergization:  " + Convert.ToString(SeCount) + " <br />"
                        + "No Of Phase Mismatches:  " + Convert.ToString(ReadConfigurations.DMSRuleErrorCount) + " <br /><br />"

                        + "Thank you,  <br />"
                        + " IGP Phase Update Process  <br /><br />"

                        + " [THIS IS AN AUTOMATED MESSAGE - PLEASE DO NOT REPLY DIRECTLY TO THIS EMAIL] ";

                }
                string lanIDs = ConfigurationManager.AppSettings["MAIL_TO_ADDRESS_MAPPER"];
                SendMail(MailAttachment, subject, bodyText, lanIDs);

            }
            catch (Exception ex)
            {
                WriteLine_Error("Unhandled exception occurred while sending the mail to DAPHIE  ," + ex.Message.ToString() + " at " + ex.StackTrace);
            }
        }

        /// <summary>
        /// send mail to mapper if session has been deleted by user or session is having de-energize features
        /// </summary>
        /// <param name="MailReason"></param>
        /// <param name="MailAttachment"></param>
        /// <param name="SessionID"></param>
        /// <param name="sCircuitID"></param>
        internal void SendMailToMapper_Reminder(string MailReason, string MailAttachment, int SessionID, string sCircuitID)
        {
            try
            {

                string bodyText = string.Empty;
                string subject = string.Empty;
                string sSessionName = ConfigurationManager.AppSettings["SessionName"] + "_" + SessionID.ToString();
                if (MailReason == ReadConfigurations.STATUS_DAPHIEFILE_DET.Status_CONFLICT)
                {
                    subject = ConfigurationManager.AppSettings["MAIL_SUBJECT_MAPPER_REMINDER"] + " " + sCircuitID + " has Conflict";
                    bodyText = "Hello Team,  <br />"
                        + " This is a gentle reminder mail for IGP Phase ID update session which has conflict. Please resolve the conflict and assign the session back to " + ReadConfigurations.ApplicationUser + " user for further action. <br /><br /> "
                        + " Session ID : " + "SN_" + SessionID + " <br />"
                        + " Session Name : " + sSessionName + " <br />"
                        + " CircuitID : " + sCircuitID + " <br /><br />"

                        + "Thank you,  <br />"
                        + " IGP Phase Update Process  <br /><br />"

                        + " [THIS IS AN AUTOMATED REMINDER MAIL - PLEASE DO NOT REPLY DIRECTLY TO THIS EMAIL] ";

                }
                else if (MailReason == ReadConfigurations.STATUS_DAPHIEFILE_DET.Status_USERQUEUE)
                {
                    int PMCount = FindPrimaryFeatureCount();
                    int SeCount = FindSecondaryFeatureCount();
                    int PhaseMismatchCount = (new DMSRULECLASS()).FindPhaseMismatch(sCircuitID);
                    subject = ConfigurationManager.AppSettings["MAIL_SUBJECT_MAPPER_REMINDER"] + " " + sCircuitID + " has QAQC errors";
                    bodyText = "Hello Team,  <br />"
                        + " This is a gentle reminder mail for IGP Phase ID update session which is not posted due to de-energization or QA/QC errors in the network. Please resolve the errors and assign the session back to " + ReadConfigurations.ApplicationUser + " user for further action. <br /><br /> "
                        + " Session ID : " + "SN_" + SessionID + " <br />"
                        + " Session Name : " + sSessionName + " <br />"
                        + " CircuitID : " + sCircuitID + " <br /><br />"
                        + "No Of Primary Denergization:  " + Convert.ToString(PMCount) + " <br />"
                        + "No Of Secondary Denergization:  " + Convert.ToString(SeCount) + " <br />"
                        + "No Of Phase Mismatches:  " + Convert.ToString(PhaseMismatchCount) + " <br /><br />"
                        + "Thank you,  <br />"
                        + " IGP Phase Update Process  <br /><br />"

                        + " [THIS IS AN AUTOMATED REMINDER MAIL - PLEASE DO NOT REPLY DIRECTLY TO THIS EMAIL] ";

                }
                string lanIDs = ConfigurationManager.AppSettings["MAIL_TO_ADDRESS_MAPPER"];
                SendMail(MailAttachment, subject, bodyText, lanIDs);

            }
            catch (Exception ex)
            {
                WriteLine_Error("Unhandled exception occurred while sending the mail to DAPHIE  ," + ex.Message.ToString() + " at " + ex.StackTrace);
            }
        }

        private int FindPrimaryFeatureCount()
        {
            int retcount = -1;
            try
            {
                string strquery = "select count(name) from " + ReadConfigurations.QAQCTableName + "  where batchid='" +
                MainClass.Batch_Number + "' and upper(name) not in ('EDGIS.DELIVERYPOINT','EDGIS.SECOHCONDUCTOR','EDGIS.SECUGCONDUCTOR','EDGIS.SERVICELOCATION','EDGIS.STREETLIGHT','EDGIS.TRANSFORMERLEAD')";
                DataTable DT = (new DBHelper()).GetDataTable(strquery);
                if ((DT != null) && (DT.Rows.Count > 0))
                {
                    retcount = Convert.ToInt32(DT.Rows[0].ItemArray[0].ToString());
                }
            }
            catch (Exception ex)
            {
                WriteLine_Error("Unhandled exception occurred while fetching the Primary Feature De-Energize Count  ," + ex.Message.ToString() + " at " + ex.StackTrace);
            }
            return retcount;
        }

        private int FindSecondaryFeatureCount()
        {
            int retcount = -1;
            try
            {
                string strquery = "select count(name) from " + ReadConfigurations.QAQCTableName + "  where batchid='" +
                MainClass.Batch_Number + "' and upper(name) in ('EDGIS.DELIVERYPOINT','EDGIS.SECOHCONDUCTOR','EDGIS.SECUGCONDUCTOR','EDGIS.SERVICELOCATION','EDGIS.STREETLIGHT','EDGIS.TRANSFORMERLEAD')";
                DataTable DT = (new DBHelper()).GetDataTable(strquery);
                if ((DT != null) && (DT.Rows.Count > 0))
                {
                    retcount = Convert.ToInt32(DT.Rows[0].ItemArray[0].ToString());
                }
            }
            catch (Exception ex)
            {
                WriteLine_Error("Unhandled exception occurred while fetching the Primary Feature De-Energize Count  ," + ex.Message.ToString() + " at " + ex.StackTrace);
            }
            return retcount;
        }
        #endregion

        public int FindErrorCircuitIdCount()
        {
            int iRetCount = -1;
            try
            {
                string strquery = "SELECT COUNT(*) FROM " + ReadConfigurations.DAPHIEFileDetTableName + " WHERE " +
                        ReadConfigurations.DAPHIETablesFields.Current_StatusField + " IS NOT NULL AND " +
                        ReadConfigurations.DAPHIETablesFields.StatusField + " IN ('CALCULATION_ERROR','UPDATE_ERROR','LOAD_ERROR','VALIDATE_ERROR')";
                DataTable DT = (new DBHelper()).GetDataTable(strquery);
                if ((DT != null) && (DT.Rows.Count > 0))
                {
                    iRetCount = Convert.ToInt32(DT.Rows[0].ItemArray[0].ToString());
                }
            }
            catch (Exception ex)
            {
                WriteLine_Error("Unhandled exception occurred while FindErrorCircuitIdCount  ," + ex.Message.ToString() + " at " + ex.StackTrace);
            }
            return iRetCount;
        }

        public int FindErrorCircuitIdCount_UpdateSession()
        {
            int iRetCount = -1;
            try
            {
                string strquery = "SELECT COUNT(*) FROM " + ReadConfigurations.DAPHIEFileDetTableName + " WHERE " + ReadConfigurations.DAPHIETablesFields.StatusField +
                    " IN (" + ReadConfigurations.Status_To_Be_Skipped + ")";
                DataTable DT = (new DBHelper()).GetDataTable(strquery);
                if ((DT != null) && (DT.Rows.Count > 0))
                {
                    iRetCount = Convert.ToInt32(DT.Rows[0].ItemArray[0].ToString());
                }
            }
            catch (Exception ex)
            {
                WriteLine_Error("Unhandled exception occurred while FindErrorCircuitIdCount_UpdateSession  ," + ex.Message.ToString() + " at " + ex.StackTrace);
            }
            return iRetCount;
        }

        #region Update log
        public void WriteLine_Info(string sMsg)
        {
            try
            {
                if (MainClass.m_blIfChild == false)
                {
                    _log.Info(sMsg);
                    return;
                }
                StreamWriter pSWriter = default(StreamWriter);
                pSWriter = File.AppendText(MainClass.m_sLogFileName);
                pSWriter.WriteLine("INFO," + sMsg);
                //DrawProgressBar();
                Console.WriteLine(sMsg);
                pSWriter.Close();
            }
            catch (Exception ex)
            {
                //WriteLine_Error("Error in Inserting Transformer Unit records through Stored Procedure," + ex.Message + " at " + ex.StackTrace);
            }
        }

        public void WriteLine_Warn(string sMsg)
        {
            try
            {
                if (MainClass.m_blIfChild == false)
                {
                    _log.Warn(sMsg);
                    return;
                }

                StreamWriter pSWriter = default(StreamWriter);
                pSWriter = File.AppendText(MainClass.m_sLogFileName);
                pSWriter.WriteLine("WARNING," + sMsg);
                //DrawProgressBar();
                Console.WriteLine(sMsg);
                pSWriter.Close();
            }
            catch
            {
                //WriteLine_Error("Error in Inserting Transformer Unit records through Stored Procedure," + ex.Message + " at " + ex.StackTrace);
            }
        }

        public void WriteLine_Error(string sMsg)
        {
            try
            {
                if (MainClass.m_blIfChild == false)
                {
                    _log.Error(sMsg);
                    return;
                }
                StreamWriter pSWriter = default(StreamWriter);
                pSWriter = File.AppendText(MainClass.m_sLogFileName);
                pSWriter.WriteLine("ERROR," + sMsg);
                //DrawProgressBar();
                Console.WriteLine(sMsg);
                pSWriter.Close();
            }
            catch (Exception ex)
            {
                //WriteLine_Error("Error in Inserting Transformer Unit records through Stored Procedure," + ex.Message + " at " + ex.StackTrace);
            }
        }

        # endregion



        internal void UpdateParentInformationForBusbar(string sCircuitID)
        {
            try
            {
                string strUpdateQry = (new DBQueries()).GetBusbarwithGetParentFeature(sCircuitID);

                DataTable BusBarTable = (new DBHelper()).GetDataTableByQuery(strUpdateQry);
                if ((BusBarTable != null) && (BusBarTable.Rows.Count > 0))
                {
                    foreach (DataRow dr in BusBarTable.Rows)
                    {
                        string strSourceFeatureID = dr["To_Feature_GlobalID"].ToString();
                        string updateQuery = "Update " + ReadConfigurations.UnprocessedTableName + " set PARENT_GUID='" + strSourceFeatureID + "' where "
                            + " Feature_OID= '" + dr["BUSBAR_OID"].ToString() + "' and batchid='" + MainClass.Batch_Number + "'";
                        (new DBHelper()).UpdateQuery(updateQuery);

                    }
                }

            }
            catch (Exception ex)
            {
                WriteLine_Error("Exception Occurred while updating the parent information for Busbar Feature," + ex.Message + " at " + ex.StackTrace);
                throw ex;
            }
        }
    }


}
