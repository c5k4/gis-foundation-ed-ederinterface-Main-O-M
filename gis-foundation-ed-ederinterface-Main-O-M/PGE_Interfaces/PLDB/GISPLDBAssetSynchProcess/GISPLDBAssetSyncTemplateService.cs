using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net;
using GISPLDBAssetSynchProcess.Common;
using GISPLDBAssetSynchProcess.DAL;
using System.Reflection;
using System.Configuration;
using System.IO;
using System.Data;
using System.Collections.Specialized;
using Oracle.DataAccess.Client;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using System.Net;
using Newtonsoft.Json.Linq;
using System.Globalization;
using System.Diagnostics;
using System.Threading;

namespace GISPLDBAssetSynchProcess
{
    public class GISPLDBAssetSyncTemplateService : IGISPLDBAssetSyncTemplate
    {
        #region Private variables
        //  List<Dictionary<string, string>>  asset = new TLDBInsertDictTemplate();
        private log4net.ILog Logger = null;
        private static StringBuilder Argumnet = default;
        private static StringBuilder remark = default;
        List<Dictionary<string, string>> lstDictTemplateInsert = new List<Dictionary<string, string>>();
        List<Dictionary<string, string>> lstDictTemplateUpdate = new List<Dictionary<string, string>>();
        List<Dictionary<string, string>> lstDictTemplateDelete = new List<Dictionary<string, string>>();

        TLDBDictTemplate tldbCommonTemplate = new TLDBDictTemplate();

        //TLDBInsertDictTemplate tldbInsertTemplate = new TLDBInsertDictTemplate();
        //TLDBUpdateDictTemplate tldbUpdateTemplate = new TLDBUpdateDictTemplate();
        //TLDBDeleteDictTemplate tldbDeleteTemplate = new TLDBDeleteDictTemplate();
        #endregion
        public GISPLDBAssetSyncTemplateService()
        {
            InitializeLogger();

        }
        #region Commented Code 
        //public DataTable ReadandUpdateEd06fromCSV(string Ed06Path, string Ed07path, string Ed07MappingSection)
        //{
        //    DataTable ed06TempCopy = new DataTable();
        //    try
        //    {
        //        Logger.Info(" \t Reading Ed06 File Starts");
        //        CommonUtil cmnUtil = new CommonUtil(Logger);
        //        DataTable ed06DataTable = cmnUtil.GetDataFromED06CSV(Ed06Path);
        //        Logger.Info(" \t Reading Ed06 CSV FIle Ends");
        //        DataTable ed07DataTable = cmnUtil.ReadCSVDataInTableandMap(Ed07path, Ed07MappingSection);
        //        ed06TempCopy = cmnUtil.UpdateSapEquipIDandCopyTable(ed06DataTable, ed07DataTable);
        //    }
        //    catch (Exception ex)
        //    {
        //        Logger.Error("Method Name:-RemoveCurlybracesInGUID "+ex.Message.ToString());
        //    }
        //     return ed06TempCopy;

        //}
        #endregion
        /// <summary>
        ///  This function is used for insert records for Sap Equipment ID 
        /// </summary>
        /// <param name="Ed06FinalTable"></param>
        /// <param name="TableName"></param>
        /// <returns> Void</returns>
        public void BulkInsertAfterSApEQUIPIDUpdate(DataTable Ed06FinalTable,string TableName)
        {
            CommonUtil cmnUtil = new CommonUtil(Logger);
            try
            {
                if (Ed06FinalTable.Rows.Count > 0)
                {
                    if (cmnUtil.BulkInsertintoED06(Ed06FinalTable,TableName))
                        Logger.Info("Data Inserted In "+TableName+ "table ");
                    else Logger.Info("Data Not Inserted  In "+TableName+ " table");
                }
                else Logger.Info(" \t No proper " + TableName + " data to Insert");
            }
            catch (Exception exp)
            {

                if (string.IsNullOrEmpty(Program.Errorcomment))
                {
                    Program.Errorcomment = "Method Name:-BulkInsertAfterSApEQUIPIDUpdate " + exp.Message.ToString();

                }
                else
                {
                    Program.Errorcomment = Program.Errorcomment + Environment.NewLine + "Method Name:-BulkInsertAfterSApEQUIPIDUpdate " + exp.Message.ToString();
                }
                Logger.Error("Method Name:-BulkInsertAfterSApEQUIPIDUpdate " + exp.Message.ToString());
            }
        }
        //public DataTable FetchDataFromEd06forRetryCount()
        //{
        //    DataTable ed06Table = new DataTable();
        //    string query = string.Format("select * from {0} where INTERFACE_STATUS in ('NEW','ERROR')", AppConstants.DB_ED06_GISPLDB_INTERFACE);
        //    DBHelper objDB1 = new DBHelper(AppConstants.DB_Code, Logger);
        //    ed06Table = objDB1.SelectPGEDataQuery(query, false);

        //    return ed06Table;
        //}

        /// <summary>
        ///  This function is used for remove delete row for replace case 
        /// </summary>
        /// <param name="DT06InterfaceTable"></param>
        /// <returns> DataTable</returns>
        private DataTable RemoveDeleteRowsInReplace(DataTable DT06InterfaceTable)
        {
            
            List<DataRow> rowsToDelete = new List<DataRow>();
            DataRow DRTODelete = null;

            try
            {
                foreach (DataRow DR in DT06InterfaceTable.Rows)
                {
                    if (!string.IsNullOrEmpty(DR["PLDBID"].ToString()))
                    {
                        if ((DR["EDIT_TYPE"].ToString() == "D") && (DR["INTERFACE_STATUS"].ToString() == "NEW"))
                        {
                            var ReplaceCaseRows = from ReplaceCaseRow in DT06InterfaceTable.AsEnumerable()
                                                  where ReplaceCaseRow.Field<System.Decimal>("PLDBID") == (System.Decimal)DR["PLDBID"]
                                                  && ReplaceCaseRow.Field<string>("Parent_GUID") == DR["PGE_GLOBALID"].ToString()
                                                  && ReplaceCaseRow.Field<string>("EDIT_TYPE") == "I"
                                                  && ReplaceCaseRow.Field<string>("INTERFACE_STATUS") == "NEW"
                                                  select ReplaceCaseRow;
                            DRTODelete = ReplaceCaseRows.FirstOrDefault();
                        }
                    }

                    if (DRTODelete != null)
                    {
                        rowsToDelete.Add(DR);
                        DRTODelete = null;
                    }

                }
                foreach (DataRow row in rowsToDelete)
                {
                    UpdateNoProcessforDelete(row["PLDBID"].ToString(), ConfigurationManager.AppSettings["noupdates_Status"].ToString());
                    DT06InterfaceTable.Rows.Remove(row);
                    DT06InterfaceTable.AcceptChanges();

                }
            }
            catch (Exception exp)
            {
                if (string.IsNullOrEmpty(Program.Errorcomment))
                {
                    Program.Errorcomment = "Method Name:-RemoveDeleteRowsInReplace " + exp.Message.ToString();

                }
                else
                {
                    Program.Errorcomment = Program.Errorcomment + Environment.NewLine + "Method Name:-RemoveDeleteRowsInReplace " + exp.Message.ToString();
                }
                Logger.Error("Method Name:-RemoveDeleteRowsInReplace " + exp.Message.ToString());
            }

            return DT06InterfaceTable;

           // DT06InterfaceTable.AcceptChanges();
        }
        /// <summary>
        ///  This function is used for retreive data from Table And Process
        /// </summary>
        /// <returns> void</returns>
        public void RetreiveDataFromTableAndProcess()
        {
            int InsertRecordsLimit = 0;
            int UpdateRecordsLimit = 0;
            int DeleteRecordsLimit = 0;
            int configRetryCount=0;
            string strPLDBID = string.Empty;

            // Retrieve data from Interface table where the Status is New Or error
            DataTable ed06StagingTable = new DataTable();
            DataTable ed06StagingBatchTable = new DataTable();
           
            try
            {
                configRetryCount = Convert.ToInt16(ConfigurationManager.AppSettings["Retry_Count"].ToString());
                InsertRecordsLimit = Convert.ToInt16(ConfigurationManager.AppSettings["InsertBatchRecordLimit"].ToString());
                UpdateRecordsLimit = Convert.ToInt16(ConfigurationManager.AppSettings["UpdateBatchRecordLimit"].ToString());
                DeleteRecordsLimit = Convert.ToInt16(ConfigurationManager.AppSettings["DeleteBatchRecordLimit"].ToString());
                string query = string.Format("select * from {0} where INTERFACE_STATUS in ('NEW','ERROR') and  (RETRY_COUNT is null  or RETRY_COUNT<"+configRetryCount+" )", AppConstants.DB_ED06_GISPLDB_INTERFACE);
                DBHelper objDB1 = new DBHelper(AppConstants.DB_Code, Logger);
                ed06StagingTable = objDB1.SelectPGEDataQuery(query, false);
                DataTable Ed06StagingAfterRemmoving = RemoveDeleteRowsInReplace(ed06StagingTable);

                // Process records for Edit type "I"
                RetreiveDataFromTableAndProcessRecord(configRetryCount, InsertRecordsLimit, "I");
                // Process records for Edit type "U"
                RetreiveDataFromTableAndProcessRecord(configRetryCount, UpdateRecordsLimit, "U");
                // Process records for Edit type "D"
                RetreiveDataFromTableAndProcessRecord(configRetryCount, DeleteRecordsLimit, "D");
                #region Comment code 
                //if (ed06StagingTable.Rows.Count > 0)
                //{
                //    BatchCount = GetBatchCount(ed06StagingTable.Rows.Count, RecordsLimit);
                //    if (BatchCount > 1)
                //    {
                //        ed06StagingBatchTable.Clear();
                //        for (int i = 0; i < BatchCount; i++)
                //        {
                //            var rowsToDelete = new List<DataRow>();
                //            ed06StagingBatchTable = ed06StagingTable.Clone();
                //            foreach (DataRow DR in ed06StagingTable.Rows)
                //            {
                //                if (ed06StagingBatchTable.Rows.Count < RecordsLimit)
                //                {
                //                    strPLDBID = strPLDBID + DR["PLDBID"].ToString() + ",";
                //                    ed06StagingBatchTable.ImportRow(DR);
                //                    rowsToDelete.Add(DR);
                                    
                //                }
                //            }
                //            rowsToDelete.ForEach(x => ed06StagingTable.Rows.Remove(x));
                //            ed06StagingBatchTable.AcceptChanges();
                //            ed06StagingTable.AcceptChanges();
                //            if (strPLDBID.Length > 0)
                //            {
                //                strPLDBID = strPLDBID.Substring(0,strPLDBID.Length - 1);
                //            }
                //             List<Dictionary<string, string>> tldbCommAsset=new List<Dictionary<string,string>>();
                //             tldbCommAsset = CategorizeAssetForPLDB(ed06StagingBatchTable);
                //            //List<Dictionary<string, string>> tldbCommAsset = CategorizeAssetForPLDB(ed06StagingBatchTable);
                //            ProcessAssetData(tldbCommAsset, strPLDBID);
                //            strPLDBID = string.Empty;
                
                //        }
                       

                //    }
                //    else
                //    {
                //        ed06StagingBatchTable.Clear();
                //        ed06StagingBatchTable = ed06StagingTable;
                //        foreach (DataRow DR in ed06StagingBatchTable.Rows)
                //        {
                //            strPLDBID = strPLDBID + DR["PLDBID"].ToString() + ",";
                //        }
                //        if (strPLDBID.Length > 0)
                //        {
                //            strPLDBID = strPLDBID.Substring(0, strPLDBID.Length - 1);
                //        }
                //        List<Dictionary<string, string>> tldbCommAsset = CategorizeAssetForPLDB(ed06StagingBatchTable);
                //        ProcessAssetData(tldbCommAsset, strPLDBID);
                //    }
                    
                //}
                //else
                //{
                //    Logger.Info("No Data in Ed06table with New Or Error ");
                //    //  throw new Exception("No Data in Ed06table with New Or Error ...");

                //}
#endregion 

            }
            catch (Exception exp)
            {

                if (string.IsNullOrEmpty(Program.Errorcomment))
                {
                    Program.Errorcomment = "Method Name:-RetreiveDataFromTableAndProcess " + exp.Message.ToString();

                }
                else
                {
                    Program.Errorcomment = Program.Errorcomment + Environment.NewLine + "Method Name:-RetreiveDataFromTableAndProcess " + exp.Message.ToString();
                }
                Logger.Error("Method Name:-RetreiveDataFromTableAndProcess " + exp.Message.ToString());
            }
        }
        /// <summary>
        /// This function is used for Trim and compare status 
        /// </summary>
        /// <param name="ConfigRule"></param>
        /// <param name="RuletoCompare"></param>
        /// <returns> Boolean</returns>
        private Boolean TrimStatusandCompare(String ConfigRule, String RuletoCompare)
        {
            if (!String.IsNullOrEmpty(ConfigRule) && !String.IsNullOrEmpty(RuletoCompare))
            {
                string[] ConfigArray = ConfigRule.Split('_');
                string[] RuletoCompareArray = RuletoCompare.Split('_');
                return (ConfigArray[ConfigArray.Length - 2].ToUpper() == RuletoCompareArray[RuletoCompareArray.Length - 2].ToUpper());
            }
            else return false;
     
        }
        /// <summary>
        /// Process the dictionary data based on Insert / Update / Delete - Edit type.
        /// </summary>
        /// <param name="TLDBCommonAsset"></param>
        /// <param name="PLDBID"></param>
        /// <returns> void</returns>
        private void ProcessAssetData(List<Dictionary<string, string>> TLDBCommonAsset,string PLDBID)
        {
            Logger.Info("Processing Data Starts");
            string query = string.Empty;
            DataTable DTvalidateSupportStructTable = new DataTable();
            DataRow DR = null;
            try
            {
                lstDictTemplateInsert.Clear();
                lstDictTemplateUpdate.Clear();
                lstDictTemplateDelete.Clear();
                //DTvalidateSupportStructTable = GetSupportStructureRecordInBatach(PLDBID);
                if (TLDBCommonAsset.Count > 0)
                {
                    foreach (var keyvalue in TLDBCommonAsset)
                    {
                       
                        if (keyvalue["EDIT_TYPE"] == "I")
                        {

                            //DR = GetSupportStructureRecord(DTvalidateSupportStructTable, keyvalue["Pldbid"].ToString());

                            //if (DR != null)
                            //{
                                    Dictionary<string, string> dictFinalList = new Dictionary<string, string>();
                                    //string Final_Steps = ReadConflationRules(DR["HFTD"].ToString(), DR["SOURCEACCURACY"].ToString(), DR["LIDARConflatedDate"].ToString(),keyvalue["Pldbid"].ToString(), "Insert");
                                    //Check whether is it required for update. If not Update the status as DO_NOT_PROCESS
                                    string ruletoImplement ="PushAll_Insert";
                                    if (!TrimStatusandCompare(AppConstants.NOUPDATES_STATUS, ruletoImplement))
                                    {

                                        if (ConfigurationManager.AppSettings[ruletoImplement].ToString() != "")
                                        {
                                            var FinalFieldListToSend = new List<string>(ConfigurationManager.AppSettings[ruletoImplement].Split(new char[] { ';' }));
                                            foreach (string Fields in FinalFieldListToSend)
                                            {
                                                if (Fields.ToString() == AppConstants.CNFG_ED06_FIELD_PGE_GLOBALID)
                                                    keyvalue[Fields] = Regex.Replace(keyvalue[Fields], @"[\{\}]", "");
                                                dictFinalList.Add(Fields, keyvalue[Fields].ToString());
                                                //   JsonConvert.
                                            }
                                            lstDictTemplateInsert.Add(dictFinalList);
                                        }
                                        else Logger.Info("No Insert records to send to PLDB " + keyvalue["Pldbid"]);
                                    }
                                    else
                                    {
                                        //Check whether is it required for update. If not Update the status as DO_NOT_PROCESS
                                        Logger.Info("No Suppot Structure Records found for:" + keyvalue["Pldbid"]);
                                        UpdateNoProcess(keyvalue["Pldbid"], ConfigurationManager.AppSettings["noupdates_Status"].ToString());
                                    }

                                
                            }
                            //else
                            //{
                            //    Logger.Info("No Suppot Structure Records found for:" + keyvalue["Pldbid"]);
                            //    UpdateNoProcess(keyvalue["Pldbid"], ConfigurationManager.AppSettings["noupdates_Status"].ToString());

                            //}


                        //}
                        else if (keyvalue["EDIT_TYPE"] == "U")
                        {
                            //DR = GetSupportStructureRecord(DTvalidateSupportStructTable, keyvalue["Pldbid"].ToString());
                          
                            //if (DR!=null)
                            //{
                                
                                    Dictionary<string, string> dictFinalList = new Dictionary<string, string>();
                                    //string Final_Steps = ReadConflationRules(DR["HFTD"].ToString(), DR["SOURCEACCURACY"].ToString(), DR["LIDARConflatedDate"].ToString(),keyvalue["Pldbid"].ToString(), "Update");
                                    string ruletoImplement = "PushAll_Update";
                                    if (!TrimStatusandCompare(AppConstants.NOUPDATES_STATUS, ruletoImplement))
                                    {
                                        // string ruletoImplement = Final_Steps + "_" + "Update";
                                        if (ConfigurationManager.AppSettings[ruletoImplement].ToString() != "")
                                        {
                                            var FinalFieldListToSend = new List<string>(ConfigurationManager.AppSettings[ruletoImplement].Split(new char[] { ';' }));
                                            foreach (string Fields in FinalFieldListToSend)
                                            {
                                                dictFinalList.Add(Fields, keyvalue[Fields].ToString());
                                                //   JsonConvert.
                                            }
                                            lstDictTemplateUpdate.Add(dictFinalList);
                                        }
                                        else Logger.Info("No Updates to send to PLDB " + keyvalue["Pldbid"]);
                                    }
                                    else
                                    {
                                        Logger.Info("No Suppot Structure Records found for:" + keyvalue["Pldbid"]);
                                        UpdateNoProcess(keyvalue["Pldbid"], ConfigurationManager.AppSettings["noupdates_Status"].ToString());
                                    }

                                
                            //}
                            //else
                            //{
                            //    Logger.Info("No Suppot Structure Records found for:" + keyvalue["Pldbid"]);
                            //    UpdateNoProcess(keyvalue["Pldbid"], ConfigurationManager.AppSettings["noupdates_Status"].ToString());
                            //}


                        }
                        else
                        {
                            //DTvalidateSupportStructTable = objDB1.SelectQuery(query, false);
                            //if (DTvalidateSupportStructTable.Rows.Count > 0)
                            // {
                            //foreach (DataRow dr in DTvalidateSupportStructTable.Rows)
                            // {

                            Dictionary<string, string> dictFinalList = new Dictionary<string, string>();
                            //string Final_Steps = ReadConflationRules(dr["HFTD"].ToString(), dr["SOURCEACCURACY"].ToString(), dr["LIDARConflatedDate"].ToString(), "Delete");
                            string ruletoImplement = "PushAll_Delete";//Final_Steps + "_" + "Delete";
                            if (!TrimStatusandCompare(AppConstants.NOUPDATES_STATUS, ruletoImplement))
                            {
                                // string ruletoImplement = Final_Steps + "_" + "Delete";
                                var FinalFieldListToSend = new List<string>(ConfigurationManager.AppSettings[ruletoImplement].Split(new char[] { ';' }));
                                if (ConfigurationManager.AppSettings[ruletoImplement].ToString() != "")
                                {
                                    foreach (string Fields in FinalFieldListToSend)
                                    {
                                        if (keyvalue[Fields]!=null)
                                        dictFinalList.Add(Fields, keyvalue[Fields].ToString());
                                        //   JsonConvert.
                                    }
                                    lstDictTemplateDelete.Add(dictFinalList);
                                    //Check whether It's a Replace case.
                                    //foreach (var key in dictFinalList.Keys)
                                    //{
                                    //    if (!dictFinalList[key].Equals(d2[key]))
                                    //    {
                                    //        d3.Add(key, d2[key]);
                                    //    }
                                    //}

                                }
                                else Logger.Info("No Rows to delete for " + keyvalue["Pldbid"]);
                            }
                            else
                            {
                                Logger.Info("No Suppot Structure Records found for:" + keyvalue["Pldbid"]);
                                UpdateNoProcess(keyvalue["Pldbid"], ConfigurationManager.AppSettings["noupdates_Status"].ToString());
                            }

                       // }
                            //}
                            //else
                            //{
                            //    Logger.Info("No Suppot Structure Records found for:" + keyvalue["Pldbid"]);
                            //    UpdateNoProcess(keyvalue["Pldbid"], ConfigurationManager.AppSettings["noupdates_Status"].ToString());
                            //}

                        }

                    }
                    DR = null;
                }
                else
                {
                    Logger.Info("NO Data available to Insert / Update / Delete");
                }

                //if (lstDictTemplateDelete.Count > 0)
                //{
                //    List<Dictionary<string, string>> lstTempDeleteDict = new List<Dictionary<string, string>>();
                //    lstTempDeleteDict = CompareInsertandDeleteForReplaceCase(lstDictTemplateInsert, lstDictTemplateDelete);
                //    lstDictTemplateDelete.Clear();
                //    if (lstDictTemplateDelete.Count == 0) lstDictTemplateDelete = lstTempDeleteDict;
                //}

                Console.WriteLine("Data Process Ends");
                Logger.Info("Data Process Ends");
                Console.WriteLine("Preparing PLDB Details to send it to EI");
                Logger.Info("Preparing PLDB Details to send it to EI");
                //Process and Send Data individually for Insert, update and Delete

                if (lstDictTemplateInsert.Count > 0)
                    ProcessDictionaryandPost(lstDictTemplateInsert, "Insert");
                // PostListOfDataforInsert(lstDictTemplateInsert, "Insert");
              
                if (lstDictTemplateUpdate.Count > 0)
                    ProcessDictionaryandPost(lstDictTemplateUpdate, "Update");
           
                if (lstDictTemplateDelete.Count > 0)
                    ProcessDictionaryandPost(lstDictTemplateDelete, "Delete");
          
                Console.WriteLine("Ending the process of sending PLDB Details to EI");
                Logger.Info("Ending the process of sending PLDB Details to EI");
            }
            catch (Exception exp)
            {
                if (string.IsNullOrEmpty(Program.Errorcomment))
                {
                    Program.Errorcomment = "Method Name:-ProcessAssetData " + exp.Message.ToString();

                }
                else
                {
                    Program.Errorcomment = Program.Errorcomment + Environment.NewLine + "Method Name:-ReadEd06fromCSV " + exp.Message.ToString();
                }
                Logger.Error("Method Name:-ProcessAssetData " + exp.Message.ToString());
            }

        }
        #region Commented code 
        //private List<Dictionary<string, string>> CompareInsertandDeleteForReplaceCase(List<Dictionary<string, string>> InsertListDict, List<Dictionary<string, string>> DeleteListDict)
        //{
        //    if (InsertListDict.Count > 0 && DeleteListDict.Count > 0)
        //    {
        //        foreach (var InsDict in InsertListDict)
        //        {

        //           foreach (var DelDict in DeleteListDict)
        //            {
        //                if(InsDict["PLDBID"].Equals(DelDict["PLDBID"]))
        //                {
        //                    DelDict.Clear();
        //                }

        //             }
        //        }

        //    }
        //    return DeleteListDict;
        //}
        #endregion
        /// <summary>
        /// This methos is to use for process ProcessDictionary records and Post data to grid search
        /// <param name="lstDictCommon"></param>
        /// <param name="ActionType"></param>
        ///<returns> void </returns>
        private void ProcessDictionaryandPost(List<Dictionary<string, string>> lstDictCommon, string ActionType)
        {
            string strProcessPldbid = string.Empty;
            string strErrorPldbid = string.Empty;
            string strSucessPldbid = string.Empty;
            string Response = string.Empty;
            string PLDBID = string.Empty;
            int RetryCount = 0;
            try
            {
                if (lstDictCommon.Count > 0)
                {
                    Console.WriteLine("Posting Data");
                    Logger.Info("Posting Data");
                    strProcessPldbid = string.Empty;
                    foreach (var Dict in lstDictCommon)
                    {
                        //string Response = postDataToPLDB(Dict, ActionType);
                        strProcessPldbid = strProcessPldbid + Dict["Pldbid"].ToString() + ",";
                       
                        //Logger.Info(Response);
                    }
                    if (strProcessPldbid.Length > 0)
                    {
                        strProcessPldbid = strProcessPldbid.Substring(0, strProcessPldbid.Length - 1);
                    }
                    Response = postData(lstDictCommon, ActionType, strProcessPldbid);
                   
                    if (Response.Length > 0)
                    {
                        //DataSet dataSet = JsonConvert.DeserializeObject<DataSet>(Response);
                         if (strProcessPldbid.Length > 0)
                         {
                             string[] PLDBArray = strProcessPldbid.Split(',');
                             if (PLDBArray != null)
                             {
                                 for (int i = 0; i < PLDBArray.Length; i++)
                                 {
                                     PLDBID = PLDBArray[i].ToString();
                                     if (PLDBID.Length > 0)
                                     {
                                         if (Response.Contains("PLDBID"))
                                         {
                                             if (Response.Contains(PLDBID))
                                             {


                                                 Logger.Info("Failed to send Request" + PLDBID);
                                                 // get count records 
                                                 RetryCount = Check_Retry_Count(PLDBID);
                                                 RetryCount = RetryCount + 1;
                                                 UpdateErrorRecord(PLDBID, ConfigurationManager.AppSettings["errorStatus"].ToString(), RetryCount);
                                                 // update queyry for Error Records 

                                             }
                                             else
                                             {
                                                 strSucessPldbid = strSucessPldbid + PLDBID + ",";
                                             }
                                         }
                                         else
                                         {
                                             Logger.Info("Failed to send Request" + PLDBID);
                                             // get count records 
                                             RetryCount = Check_Retry_Count(PLDBID);
                                             RetryCount = RetryCount + 1;
                                             UpdateErrorRecord(PLDBID, ConfigurationManager.AppSettings["errorStatus"].ToString(), RetryCount);
                                             // update queyry for Error Records 
                                         }
                                     }
                                     // get retry
                                 }
                                 if (strSucessPldbid.Length > 0)
                                 {
                                     strSucessPldbid = strSucessPldbid.Substring(0, strSucessPldbid.Length - 1);
                                     UpdateInterfaceStatusSENT(strSucessPldbid, ConfigurationManager.AppSettings["completedStatus"].ToString());
                                     // update query for Sucess records 

                                 }
                             }
                         }
                            
                    }
                }
            }
            catch (WebException ex)
            {

                Logger.Error("Method Name:-ProcessDictionaryandPost " + ex.Message.ToString());
                //return false;
            }
        }
        /// <summary>
        /// This methos is to use for update status as sent  in  PGEDATA.ED06_GISPLDB_INTERFACE
        /// <param name="PlDBID"></param>
        /// <param name="Status"></param>
        ///<returns> void </returns>

        private bool UpdateInterfaceStatusSENT(string PLDBID, string status)
        {
            try
            {
                Logger.Info(" \t updateInterfaceStatus"+status+"Started for PLDBID:"+PLDBID);
                if (PLDBID == null || PLDBID.Trim() == string.Empty)
                {
                    throw new Exception("updateInterfaceStatus --  PLDBID can not be null .. ");
                }
                DBHelper objDBhelper = new DBHelper(AppConstants.DB_Code, Logger);
                // int updateTLDBID = Convert.ToInt32(PLDBID);
                string query = string.Format("update {1} set INTERFACE_STATUS= '{2}', LASTMODIFED_DATE = sysdate where PLDBID in ( {0}) and INTERFACE_STATUS in ('NEW','ERROR')", PLDBID, AppConstants.DB_ED06_GISPLDB_INTERFACE, status);
                objDBhelper.ExecuteScalarPGEData(query);
                Logger.Info(" \t updateInterfaceStatus" + status + "Ended for PLDBID:" + PLDBID);
                return true;
            }
            catch (Exception Ex)
            {
                Logger.Error(" \t  \t  Error In updateInterfaceStatusSENT Method", Ex);
                return false;
                //throw Ex;
            }
        }
        // <summary>
        /// This methos is to use for serialize PLDB Dict
        /// <param name="Dict"></param>
        /// ///<returns> string </returns>
        private string serializePLDBDict(Dictionary<string, string> Dict)
        {

            // JavaScriptSerializer js = new JavaScriptSerializer();
            string s = JsonConvert.SerializeObject(Dict);
            Logger.Info("GetAssetByETIntID - Serialization Completed.");
            return s;
        }
        /// <summary>
        /// This methos is to use for add object to JSON
        /// <param name="json"></param>
        /// <param name="objects"></param>
        ///<returns> string </returns>
        public string AddObjectsToJson<T>(string json, List<T> objects)
        {
            List<T> list = JsonConvert.DeserializeObject<List<T>>(json);
            list.AddRange(objects);
            return JsonConvert.SerializeObject(list);
        }
        /// <summary>
        /// This methos is to use for post data to Grid Search  
        /// <param name="dictListFinal"></param>
        /// <param name="ActionType"></param>
        /// <param name="PLDBID"></param>
        ///<returns> string </returns>
        // Post data to Grid Search 
        public string postData(List<Dictionary<string, string>> dictListFinal, string ActionType,string PLDBID)
        {
           
            var JsontosendUpdated = "";
            Logger.Info("Post Data Starts for PLDBID:" + PLDBID);
            string ResponseToSend = string.Empty;
            var responseText = "";
            string responseToSend = string.Empty;
            string urlToSend = string.Empty;
            try
            {
                if (ActionType == "Insert")
                    urlToSend = System.Configuration.ConfigurationManager.AppSettings["SetSAPEQUIPIDURL"];
                else if (ActionType == "Update")
                    urlToSend = System.Configuration.ConfigurationManager.AppSettings["UpdateLocationsURL"];
                else urlToSend = System.Configuration.ConfigurationManager.AppSettings["DecommissionPoleURL"];

                string responseFromServer = string.Empty;
               

                    var Jsontosend = JsonConvert.SerializeObject(dictListFinal, Formatting.Indented);
                    if (ActionType == "Insert")
                        JsontosendUpdated = Jsontosend.Replace("Pldbid", "Pldbid".ToUpper());
                    else if (ActionType == "Update")
                        JsontosendUpdated = Jsontosend.Replace("Pldbid", "PldbId");
                    else JsontosendUpdated = Jsontosend.Replace("Pldbid", "Pldbid".ToUpper());

                    //  string FinalJson= AddObjectsToJson(Jsontosend, 
                    //  JArray JsonArray = JArray.Parse(Jsontosend);
                    //  string Jsontosend = serializePLDBDict(dictSend);
                    var httpWebRequest = WebRequest.Create(String.Format(urlToSend));
                    httpWebRequest.Credentials = new NetworkCredential(System.Configuration.ConfigurationManager.AppSettings["serviceUserName"].ToString(), System.Configuration.ConfigurationManager.AppSettings["servicePassword"].ToString());
                    httpWebRequest.ContentType = "application/json; charset=utf-8";
                    httpWebRequest.Method = "POST";
                    using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                    {
                        // string json = "{ \"method\" : \"guru.test\", \"params\" : [ \"Guru\" ], \"id\" : 123 }";

                        // streamWriter.Write(Jsontosend);
                        streamWriter.Write(JsontosendUpdated);
                        streamWriter.Flush();
                    }
                    var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                    using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                    {
                        responseText = streamReader.ReadToEnd();
                        //Console.WriteLine(responseText);

                        //Now you have your response.
                        //or false depending on information in the response     
                    }
                    ResponseToSend = "Response Status Description from EI:" + ((HttpWebResponse)httpResponse).StatusDescription;
                    if (((HttpWebResponse)httpResponse).StatusDescription == "OK")
                    {
                        Logger.Info("Received Response Status Description from EI for " + PLDBID + "As" + ResponseToSend);
                        UpdateInterfaceStatusSENT(PLDBID, ConfigurationManager.AppSettings["completedStatus"].ToString());
                        ResponseToSend = string.Empty;

                    }
                   
                    Logger.Info("Post Data Ends" + PLDBID);
                
            }
            catch (WebException ex)
            {
                
                ResponseToSend = new StreamReader(ex.Response.GetResponseStream()).ReadToEnd().ToString();
               
                Logger.Error("Failed to send Request" +  ResponseToSend + ex.Message);
                return ResponseToSend;
            }
            return ResponseToSend;
        }
        #region Commented Code 
        //public string PostListOfDataforInsert(List<Dictionary<string, string>> dictList, string ActionType)
        //{
        //    var responseText = "";
        //    int RetryCount = 0;
        //    string responseToSend = string.Empty;
        //    foreach (var dictSend in dictList)
        //    {
        //        string urlToSend = string.Empty;
        //        try
        //        {
        //            RetryCount = Check_Retry_Count(dictSend["Pldbid"].ToString());
        //            if (ActionType == "Insert")
        //                urlToSend = System.Configuration.ConfigurationManager.AppSettings["SetSAPEQUIPIDURL"];
        //            else if (ActionType == "Update")
        //                urlToSend = System.Configuration.ConfigurationManager.AppSettings["UpdateLocationsURL"];
        //            else urlToSend = System.Configuration.ConfigurationManager.AppSettings["DecommissionPoleURL"];

        //            string responseFromServer = string.Empty;

        //            var Jsontosend = JsonConvert.SerializeObject(dictSend, Formatting.Indented);

        //            //  string FinalJson= AddObjectsToJson(Jsontosend, 
        //            //  JArray JsonArray = JArray.Parse(Jsontosend);
        //            //  string Jsontosend = serializePLDBDict(dictSend);
        //            var httpWebRequest = WebRequest.Create(String.Format(urlToSend));
        //            httpWebRequest.Credentials = new NetworkCredential(System.Configuration.ConfigurationManager.AppSettings["serviceUserName"].ToString(), System.Configuration.ConfigurationManager.AppSettings["servicePassword"].ToString());
        //            httpWebRequest.ContentType = "application/json; charset=utf-8";
        //            httpWebRequest.Method = "POST";
        //            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
        //            {
        //                // string json = "{ \"method\" : \"guru.test\", \"params\" : [ \"Guru\" ], \"id\" : 123 }";

        //                // streamWriter.Write(Jsontosend);
        //                UpdateInterfaceStatusSENT(dictSend["PLDBID"], ConfigurationManager.AppSettings["sentStatus"].ToString());
        //                streamWriter.Write(Jsontosend);
        //                streamWriter.Flush();
        //            }
        //            var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
        //            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
        //            {
        //                responseText = streamReader.ReadToEnd();
        //                //Console.WriteLine(responseText);

        //                //Now you have your response.
        //                //or false depending on information in the response     
        //            }
        //            Logger.Info("Web Request Sent to PLDB");
        //            UpdateInterfaceStatusSENT(dictSend["PLDBID"], ConfigurationManager.AppSettings["completedStatus"].ToString());

        //        }
        //        catch (WebException ex)
        //        {
        //            RetryCount = RetryCount + 1;
        //            responseToSend = new StreamReader(ex.Response.GetResponseStream()).ReadToEnd().ToString();
        //            UpdateErrorRecord(dictSend["PLDBID"], ConfigurationManager.AppSettings["errorStatus"].ToString(), RetryCount);
        //            //string Res = (HttpWebResponse)response).StatusDescription
        //            Logger.Info("Failed to send Request" + dictSend.ToString() + responseToSend + ex.Message);
        //            return responseToSend;
        //        }
        //    }
        //    return responseText;
        //}
        #endregion
        /// <summary>
        /// This methos is to use for update Retry count for Error records in  PGEDATA.ED06_GISPLDB_INTERFACE
        /// <param name="PlDBID"></param>
        /// <param name="Status"></param>
        /// <param name="ReTryCount"></param>
        ///<returns> void </returns>

        private void UpdateErrorRecord(string PLDBID, String Status, int ReTryCount)
        {
            try
            {
                Logger.Info(" \t updateErrorRecord Start");
                DBHelper objDBhelper = new DBHelper(AppConstants.DB_Code, Logger);
                //  int updateTLDBID = Convert.ToDouble(PLDBID);
                string query = string.Format("update {1} set INTERFACE_STATUS= '{2}', LASTMODIFED_DATE = sysdate, RETRY_COUNT= {3} where PLDBID = '{0}' and INTERFACE_STATUS in ('NEW','ERROR')", PLDBID, AppConstants.DB_ED06_GISPLDB_INTERFACE, Status, ReTryCount);
                objDBhelper.ExecuteScalarPGEData(query);
                Logger.Info("\t updateErrorRecord Complete ..");
            }
            catch (Exception Ex)
            {
                Logger.Error(" \t  \t Error in updateErrorRecord Method ", Ex);
                //Console.WriteLine(Ex.Message.ToString());
                //throw Ex;
            }
        }
        /// <summary>
        /// This methos is to use for update status for Edit type 'D'  records in  PGEDATA.ED06_GISPLDB_INTERFACE
        /// <param name="PlDBID"></param>
        /// <param name="Status"></param>
        ///<returns> void </returns>
        private void UpdateNoProcessforDelete(string PLDBID, String Status)
        {
            try
            {
                Logger.Info(" \t update_DO_NOT_PROCESS_Record Start for Delete Records this is for Replace Case for PLDBID:" + PLDBID);
                DBHelper objDBhelper = new DBHelper(AppConstants.DB_Code, Logger);
                //  int updateTLDBID = Convert.ToDouble(PLDBID);
                string query = string.Format("update {1} set INTERFACE_STATUS= '{2}', LASTMODIFED_DATE = sysdate where Edit_Type='D' AND INTERFACE_STATUS='NEW'  AND PLDBID = '{0}'", PLDBID, AppConstants.DB_ED06_GISPLDB_INTERFACE, Status);
                objDBhelper.ExecuteScalarPGEData(query);
                Logger.Info("\t update_DO_NOT_PROCESS_Record Complete ..");
            }
            catch (Exception Ex)
            {
                Logger.Error(" \t  \t Error in updateErrorRecord Method ", Ex);
                //Console.WriteLine(Ex.Message.ToString());
                //throw Ex;
            }
        }
        /// <summary>
        /// This methos is to use for update status 'Do Not Process' in  PGEDATA.ED06_GISPLDB_INTERFACE
        /// <param name="PlDBID"></param>
        /// <param name="Status"></param>
        ///<returns> void </returns>
        private void UpdateNoProcess(string PLDBID, String Status)
        {
            try
            {
                Logger.Info(" \t update_DO_NOT_PROCESS_Record Start or PLDBID:" + PLDBID);
                DBHelper objDBhelper = new DBHelper(AppConstants.DB_Code, Logger);
                //  int updateTLDBID = Convert.ToDouble(PLDBID);
                string query = string.Format("update {1} set INTERFACE_STATUS= '{2}', LASTMODIFED_DATE = sysdate where PLDBID = '{0}'", PLDBID, AppConstants.DB_ED06_GISPLDB_INTERFACE, Status);
                objDBhelper.ExecuteScalarPGEData(query);
                Logger.Info("\t update_DO_NOT_PROCESS_Record Complete ..");
            }
            catch (Exception Ex)
            {
                Logger.Error(" \t  \t Error in updateErrorRecord Method ", Ex);
                //Console.WriteLine(Ex.Message.ToString());
                //throw Ex;
            }
        }
        #region Commented Code 
        //private void UpdateSapEqupIDRecord(string GlobalID, string SapEquipID)
        //{

        //    try
        //    {
        //        DBHelper objDBhelper = new DBHelper(AppConstants.DB_Code, Logger);
        //        //  int updateTLDBID = Convert.ToDouble(PLDBID);
        //        string query = string.Format("update {1} set PGE_SAPEQUIPID= '{2}' where PGE_GLOBALID = '{0}' and INTERFACE_STATUS in ('NEW','ERROR') and EDIT_Type='I'", GlobalID, AppConstants.DB_ED06_GISPLDB_INTERFACE, SapEquipID);
        //        objDBhelper.ExecuteScalarPGEData(query);
        //        //if (RecordUpdate == 1)
        //        //{
        //        //    Logger.Info("\t PGE_SAPEQUIPID has been updated for global ID " + GlobalID);
        //        //}
        //    }
        //    catch (Exception exp)
        //    {
        //        Logger.Error("Method Name:-UpdateSapEqupIDRecord " + exp.Message.ToString());
        //        //Console.WriteLine(Ex.Message.ToString());
        //        //throw Ex;
        //    }
        //}
        #endregion
        /// <summary>
        /// This methos is to use for update SapEquipID
        /// </summary>
        ///<returns> void </returns>
        public void UpdateSapEquipID(string uspname)
        {

            string strSapEquipID = string.Empty;
            string strGlobalId = string.Empty;
            try
            {
                Logger.Info(" \t updateSAPEquipmentIDRecord Started");
                DBHelper objDBhelper = new DBHelper(AppConstants.DB_Code, Logger);
                objDBhelper.SPCallingWithoutParameter(uspname);
                Logger.Info(" \t update SAPEquipmentID Record completed");
                
            }
            catch (Exception ex)
            {
                if (string.IsNullOrEmpty(Program.Errorcomment))
                {
                    Program.Errorcomment = "Method Name:-UpdateSapEquipID " + ex.Message.ToString();

                }
                else
                {
                    Program.Errorcomment = Program.Errorcomment + Environment.NewLine + "Method Name:-UpdateSapEquipID " + ex.Message.ToString();
                }
                Logger.Error("Method Name:-UpdateSapEquipID " + ex.Message.ToString());
            }
        }
        #region Commented Code 
        //public void InsertRetryCount(DataTable DTEd06)
        //{

        //    foreach (DataRow row in DTEd06.Rows)
        //    {
        //        int Check_Count = Check_Retry_Count(row["PLDBID"].ToString());
        //        UpdateRetryCount(row["PLDBID"].ToString(), Check_Count);
        //    }
        //}
        //public void UpdateRetryCount(string PLDBID, int RetryCount)
        //{
        //    try
        //    {
        //        int Check_Count = Check_Retry_Count(PLDBID);
        //        if (Check_Count != int.Parse(ConfigurationManager.AppSettings["Retry_Count"].ToString()))
        //        {
        //            RetryCount++;
        //            Logger.Info(" \t update Retry Count Record Start for PLDBID:" + PLDBID);
        //            DBHelper objDBhelper = new DBHelper(AppConstants.DB_Code, Logger);
        //            //  int updateTLDBID = Convert.ToDouble(PLDBID);
        //            string query = string.Format("update {1} set RETRY_COUNT= '{2}' where PLDBID = '{0}'", PLDBID, AppConstants.DB_ED06_GISPLDB_INTERFACE, RetryCount);
        //            objDBhelper.ExecuteScalarPGEData(query);
        //            Logger.Info("\t update Retry Count Record ended for PLDBID .." + PLDBID);
        //        }

        //    }
        //    catch (Exception Ex)
        //    {
        //        Logger.Error(" \t  \t Error in updateErrorRecord Method ", Ex);
        //        //Console.WriteLine(Ex.Message.ToString());
        //        //throw Ex;
        //    }
        //}
        //private string ReadConflationRules(string HFTD, string SourceAccuracy, string LIDARConflatedDate, string PLDBID, string Method)
        //{

        //    Logger.Info("Reading Conflation Rules Starts for "+PLDBID);
        //    var connectionManagerDataSection = ConfigurationManager.GetSection(ConfigRuleReader.SectionName) as ConfigRuleReader;
        //    //  int ConvSourceAccuracy = (int) SourceAccuracy;
        //    string final_step = ConfigurationManager.AppSettings["CommonUpdateRule"].ToString();
        //    try
        //    {
        //        if (connectionManagerDataSection != null)
        //        {
        //            foreach (RuleElement element in connectionManagerDataSection.ConnectionManagerEndpoints)
        //            {
        //                if (element.HFTD.Contains(HFTD.ToString()))
        //                {
        //                    switch (HFTD)
        //                    {
        //                        case "0":
        //                        case " ":

        //                            // If HFTD is O then Pushall changes
        //                            if (element.Name == "NEITHER")
        //                                final_step = element.ruleDesc;
        //                            break;
        //                        case "2":
        //                        case "3":

        //                           // if (element.SourceAccuracy.Contains(SourceAccuracy.ToString()) && SourceAccuracy.All(char.IsDigit))

        //                            //If SourceAccuracy is 31 and 37 and HFTD is 2 or 3 then Do not update Lat and Long
        //                            if (element.SourceAccuracy.Contains(SourceAccuracy.ToString()) && !String.IsNullOrEmpty(SourceAccuracy))
        //                            //if (element.SourceAccuracy.Contains(SourceAccuracy.ToString()) && LIDARConflatedDate == element.LIDARConflatedDate)
        //                            {


        //                                if (element.Name == "BOTH")
        //                                {
        //                                    final_step = element.ruleDesc;
        //                                    return final_step;
        //                                }
        //                            }
        //                            //If SourceAccuracy is not equal to 31 and 37 and HFTD is 2 or 3 then Push All changes
        //                            else if (!(element.SourceAccuracy.Contains(SourceAccuracy.ToString())) || String.IsNullOrEmpty(SourceAccuracy))
        //                            {
        //                                if (!String.IsNullOrEmpty(LIDARConflatedDate) || LIDARConflatedDate != "")
        //                                {
        //                                    if (element.Name == "BOTHWITHDATE")
        //                                    {
        //                                        final_step = element.ruleDesc;
        //                                        return final_step;
        //                                    }
        //                                }
        //                                else
        //                                {
        //                                    //If SourceAccuracy is not equal to 31 and 37 , HFTD is 2 or 3 and Lidar Conflated date is null then No updates to be sent
        //                                    // waiting for the confirmation
        //                                    if (LIDARConflatedDate == "" || String.IsNullOrEmpty(LIDARConflatedDate))
        //                                    {
        //                                        if (element.Name == "BOTHWITHOUTDATE")
        //                                        {
        //                                            final_step = element.ruleDesc;
        //                                            return final_step;
        //                                        }
        //                                    }

        //                                }
        //                            }
        //                            //else if (String.IsNullOrEmpty(SourceAccuracy))
        //                            //{
        //                            //    if (String.IsNullOrEmpty(LIDARConflatedDate))
        //                            //    {
        //                            //        if (element.Name == "BOTHWITHOUTDATE")
        //                            //            final_step = element.ruleDesc;
        //                            //    }
        //                            //    else
        //                            //        if (element.Name == "BOTHWITHDATE")
        //                            //            final_step = element.ruleDesc;

        //                            //}

        //                         // else if(
        //                            break;
        //                    }
        //                    //final_step = element.ruleDesc;

        //                }

        //            }

        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Logger.Info("Error Reading Rules" + ex);
        //    }

        //    Logger.Info("Reading Conflation Rules Ends "+PLDBID);
        //    return final_step;

        //}
        #endregion
        /// <summary>
        /// This methos is to use for check Retry Count
        /// </summary>
        /// <param name="PlDBID"></param>
        ///<returns> int </returns>
        public int Check_Retry_Count(string PlDBID)
        {

            int retry_Count_from_DB = 0;
            DataTable ed06Retry = new DataTable();
            try
            {
                string query = string.Format("select RETRY_COUNT from {1} where PLDBID = '{0}' and INTERFACE_STATUS in ('NEW','ERROR')", PlDBID, AppConstants.DB_ED06_GISPLDB_INTERFACE);
                DBHelper objDB1 = new DBHelper(AppConstants.DB_Code, Logger);
                ed06Retry = objDB1.SelectPGEDataQuery(query, false);
                if (ed06Retry != null && ed06Retry.Rows.Count > 0)
                {
                    string Retry_Count = ed06Retry.Rows[0][0].ToString();
                    if (!string.IsNullOrEmpty(Retry_Count))
                     retry_Count_from_DB = Convert.ToInt16(Retry_Count);
                }
               
               
            }
            catch (Exception exp)
            {
                Logger.Error("Method Name:-Check_Retry_Count " + exp.Message.ToString());
            }
            return retry_Count_from_DB;
        }
        /// <summary>
        /// This methos is to use for Categorize Asset For PLDB
        /// </summary>
        /// <param name="DTEd06Table"></param>
        ///<returns> List </returns>
        private List<Dictionary<string, string>> CategorizeAssetForPLDB(DataTable DTEd06Table)
        {
            Logger.Info("Categorizing the Assets According to Type Starts");
            tldbCommonTemplate.ASSET.Clear(); 
            try
            {
                NameValueCollection FieldList = new NameValueCollection();
                //      FieldList = ConfigurationManager.GetSection("Insert_FieldList") as NameValueCollection;

                foreach (DataRow dr in DTEd06Table.Rows)
                {
                    if (!string.IsNullOrEmpty(dr["PLDBID"].ToString()))
                    {
                        Dictionary<string, string> dictAsset = new Dictionary<string, string>();
                        if ((dr["Edit_TYPE"].ToString() == "I") && (!string.IsNullOrEmpty(dr["PGE_SAPEQUIPID"].ToString())))
                        {
                            //Logger.Info(" \t Edit Type of Insert Found");
                            FieldList = ConfigurationManager.GetSection("Insert_FieldList") as NameValueCollection;

                            //   var result = FieldList.Keys  Select(m => m.ToUpper());
                            foreach (DataColumn dc in dr.Table.Columns)
                            {
                                string columnKey = System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(dc.ColumnName.ToLower());
                                if (FieldList.AllKeys.Contains(columnKey))
                                {
                                    if (!dictAsset.ContainsKey(dc.ColumnName))
                                        dictAsset.Add(FieldList.Get(columnKey).ToString(), dr[dc.ColumnName].ToString());
                                }

                            }
                            // tldbCommonTemplate.InsertASSET.Add(dictAsset);
                            tldbCommonTemplate.ASSET.Add(dictAsset);
                            // tldbInsertTemplate.ASSET.Add(dictAsset);
                        }
                        else if (dr["Edit_TYPE"].ToString() == "U")
                        {
                           // Logger.Info(" \t Edit Type of Update Found");
                            FieldList = ConfigurationManager.GetSection("Update_FieldList") as NameValueCollection;
                            foreach (DataColumn dc in dr.Table.Columns)
                            {

                                string columnKey = System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(dc.ColumnName.ToLower());
                                if (FieldList.AllKeys.Contains(columnKey))
                                {
                                    if (!dictAsset.ContainsKey(dc.ColumnName))
                                        dictAsset.Add(FieldList.Get(columnKey).ToString(), dr[dc.ColumnName].ToString());
                                }
                            }

                            tldbCommonTemplate.ASSET.Add(dictAsset);
                        }
                        else if (dr["Edit_TYPE"].ToString() == "D")
                        {

                            FieldList = ConfigurationManager.GetSection("Delete_FieldList") as NameValueCollection;
                            foreach (DataColumn dc in dr.Table.Columns)
                            {
                                string columnKey = System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(dc.ColumnName.ToLower());
                                if (FieldList.AllKeys.Contains(columnKey))
                                {
                                    if (!dictAsset.ContainsKey(dc.ColumnName))
                                        dictAsset.Add(FieldList.Get(columnKey).ToString(), dr[dc.ColumnName].ToString());
                                }

                            }
                            tldbCommonTemplate.ASSET.Add(dictAsset);
                        }
                        else
                        {
                            Logger.Info(" \t Wrong Edit Type Found in " + dr["PLDBID"].ToString());
                        }



                    }

                }

                //   lstDictTemplateInsert.Add(dictAsset);
                Logger.Info("Categorizing the Assets According to Type Ends");
            }
            catch (Exception exp)
            {
                if (string.IsNullOrEmpty(Program.Errorcomment))
                {
                    Program.Errorcomment = "Method Name:-CategorizeAssetForPLDB " + exp.Message.ToString();

                }
                else
                {
                    Program.Errorcomment = Program.Errorcomment + Environment.NewLine + "Method Name:-CategorizeAssetForPLDB " + exp.Message.ToString();
                }
                Logger.Error("Method Name:-CategorizeAssetForPLDB " + exp.Message.ToString());
            }
            return tldbCommonTemplate.ASSET;
        }
        /// <summary>
        /// This methos is to use for get batch count 
        /// </summary>
        /// <param name="RecordsCount"></param>
        /// <param name="RecordLimit"></param>
        ///<returns> int </returns>
        private int GetBatchCount(int RecordsCount,int RecordLimit)
        {
            int BatchCount = 0;
            int remainder = 0;
            try
            {

                if (RecordsCount <= RecordLimit)
                {
                    BatchCount = 1;
                }
                else
                {
                    BatchCount = Math.DivRem(RecordsCount, RecordLimit, out remainder);
                    if (remainder != 0)
                    {
                        BatchCount = BatchCount + 1;
                    }
                }

            }
            catch (Exception exp)
            {
                if (string.IsNullOrEmpty(Program.Errorcomment))
                {
                    Program.Errorcomment = "Method Name:-GetBatchCount " + exp.Message.ToString();

                }
                else
                {
                    Program.Errorcomment = Program.Errorcomment + Environment.NewLine + "Method Name:-GetBatchCount " + exp.Message.ToString();
                }
                Logger.Error("Method Name:-GetBatchCount " + exp.Message.ToString());
            }
            return BatchCount;
        }
        /// <summary>
        /// This methos is to use for covert Json string into Datatable
        /// </summary>
        /// <param name="jsontext"></param>
        ///<returns> DataTable </returns>
        private DataTable ConvertJsonstringtoDataTable(string jsontext)
        {
            DataTable DTFromJsontext = new DataTable();
            try
            {
                DataSet dataSet = JsonConvert.DeserializeObject<DataSet>(jsontext);
                DTFromJsontext = dataSet.Tables["NAV_ED06_ST_KF_KV"];
            }
            catch (Exception exp)
            {
                Logger.Error("Method Name:-ConvertJsonstringtoDataTable " + exp.Message.ToString());
            }
            return DTFromJsontext;
        }
        #region Commented Code 
        //private DataTable GetSupportStructureRecordInBatach(string strPldbid)
        //{

        //    DataTable DTvalidateSupportStructTable = new DataTable();
        //    string strquery = string.Empty;
        //    try
        //    {

        //        strquery = "select PLDBID,HFTD,SOURCEACCURACY,LIDARConflatedDate from " + AppConstants.DB_SUPPORT_STRUCTURE + " where PLDBID in(" + strPldbid + ")";
        //        DBHelper objDB1 = new DBHelper(AppConstants.DB_Code, Logger);
        //        DTvalidateSupportStructTable = objDB1.SelectQuery(strquery, false);

        //    }
        //    catch (Exception exp)
        //    {
        //        Logger.Error("Method Name:-GetSupportStructureRecordInBatach " + exp.Message.ToString());
        //    }
        //    return DTvalidateSupportStructTable;
        //}
        //private DataRow GetSupportStructureRecord(DataTable DTvalidateSupportStructTable,string strPldbid)
        //{
        //    DataRow DR = null;
        //    try
        //    {
        //        if (DTvalidateSupportStructTable != null && DTvalidateSupportStructTable.Rows.Count > 0)
        //        {
        //            if (!string.IsNullOrEmpty(strPldbid))
        //            {
        //                var ReplaceCaseRows = from ReplaceCaseRow in DTvalidateSupportStructTable.AsEnumerable()
        //                                      where ReplaceCaseRow.Field<System.Decimal>("PLDBID") == Convert.ToDecimal(strPldbid)
        //                                      select ReplaceCaseRow;
        //                DR = ReplaceCaseRows.FirstOrDefault();
        //            }
        //        }


        //    }
        //    catch (Exception exp)
        //    {
        //        Logger.Error("Method Name:-GetSupportStructureRecord " + exp.Message.ToString());
        //    }
        //    return DR;
        //}
        #endregion
        /// <summary>
        /// This methos is to use for Initialize Logger 
        /// </summary>
        ///<returns> Void </returns>
        public void InitializeLogger()
        {
            string executingAssemblyPath = string.Empty;
            try
            {
                //Get log file with complete path
                executingAssemblyPath = GetLogfilepath();

                if (executingAssemblyPath.Trim().Length == 0)
                {
                    executingAssemblyPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
                }
                string logPath = System.IO.Path.Combine(executingAssemblyPath, @"GISPLDBAssetSyncProcess_" + DateTime.Now.ToString("MM_dd_yyyy") + ".log");

                //log4net 
                log4net.GlobalContext.Properties["LogName"] = logPath;
                //string executingAssemblyPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);

                executingAssemblyPath = (System.IO.Path.Combine(executingAssemblyPath, "logging.xml"));

                StreamWriter wr = new StreamWriter(executingAssemblyPath);
                wr.Write(Getlog4netConfig());
                wr.Close();
                FileInfo fi = new FileInfo(executingAssemblyPath);
                log4net.Config.XmlConfigurator.Configure(fi);

                //Initialize the logging 
                Logger = log4net.LogManager.GetLogger(typeof(Program));
                Logger.Info("Logger Initialized");
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message);
            }
        }
        /// <summary>
        /// This methos is to use for get log file location 
        /// </summary>
        ///<returns> String </returns>
        private string GetLogfilepath()
        {
            try
            {
                string executingAssemblyPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
                //string logPath = (System.IO.Path.Combine(executingAssemblyPath, "C:\\Document\\Logs\\GIStoSAPLog.txt"));
                string path = System.Configuration.ConfigurationManager.AppSettings["LogFolder"];
                //string logPath = string.Format( path + "\\GIStoSAPLog_{0}.txt", DateTime.Now.ToShortDateString().Replace("/", "_"));
                return path;
            }
            catch (Exception ex)
            {
                Logger.Error("Error in creating log file path" + ex.Message);
            }
            return "";
        }
        /// <summary>
        /// This methos is to use for read records from ED006 csv file 
        /// </summary>
        ///<returns> Datatable </returns>
        public DataTable ReadEd06fromCSV()
        {
            DataTable ed06DataTable = new DataTable();
            try
            {
                Logger.Info(" \t Reading Ed06 data from GeoMart backup Schema Starts");
                CommonUtil cmnUtil = new CommonUtil(Logger);
                ed06DataTable = cmnUtil.GetDataFromED06CSV();
                Logger.Info(" \t Reading Ed06 data from GeoMart backup Schema Starts Ends");
                //DataTable ed07DataTable = cmnUtil.ReadCSVDataInTableandMap(Ed07path, Ed07MappingSection);
                //ed06TempCopy = cmnUtil.UpdateSapEquipIDandCopyTable(ed06DataTable, ed07DataTable);
            }
            catch (Exception ex)
            {
                if(string.IsNullOrEmpty(Program.Errorcomment))
                {
                    Program.Errorcomment = "Method Name:-ReadEd06fromCSV " + ex.Message.ToString();

                }
                else
                {
                    Program.Errorcomment = Program.Errorcomment+Environment.NewLine+ "Method Name:-ReadEd06fromCSV " + ex.Message.ToString();
                }
                Logger.Error("Method Name:-ReadEd06fromCSV " + ex.Message.ToString());
            }
            return ed06DataTable;

        }
        /// <summary>
        /// This methos is to use for read records from ED007 csv file 
        /// </summary>
        /// <param name="Ed07path"></param>
        /// <param name="Ed07MappingSection"></param>
        ///<returns> Datatable </returns>
        public DataTable ReadEd07fromCSV(string Ed07path, string Ed07MappingSection)
        {
            DataTable ed07TempCopy = new DataTable();
            try
            {
                Logger.Info(" \t Reading Ed07 File Starts");
                CommonUtil cmnUtil = new CommonUtil(Logger);
                ed07TempCopy = cmnUtil.ReadCSVDataInTableandMap(Ed07path, Ed07MappingSection);
                Logger.Info(" \t Reading Ed07 CSV FIle Ends");
                
            }
            catch (Exception ex)
            {
                Logger.Error("Method Name:-ReadEd07fromCSV " + ex.Message.ToString()); ;
            }
            return ed07TempCopy;

        }
        /// <summary>
        /// This methos is to use for delete records  from  PGEDATA.ED06_GISPLDB_INTERFACE table 
        /// </summary>
        ///<returns> Void </returns>
        public void DeleteRecordfromED06_GISPLDB_INTERFACETable()
        {
            string NoDays = string.Empty;

            try
            {
                NoDays = ConfigurationManager.AppSettings["DeleteRecordDays"].ToString();
                DBHelper objDBhelper = new DBHelper(AppConstants.DB_Code, Logger);
                //  int updateTLDBID = Convert.ToDouble(PLDBID);
                string query = string.Format("DELETE FROM  PGEData.ED06_GISPLDB_INTERFACE where trunc(LASTMODIFED_DATE) < trunc(SYSDATE) -" + NoDays);
                objDBhelper.ExecuteScalarPGEData(query);
                Logger.Info(" \t " + NoDays + " days old records deleted successfully from ED06_GISPLDB_INTERFACE ");
                
            }
            catch (Exception exp)
            {
                Logger.Error("Method Name:-DeleteRecordfromED06_GISPLDB_INTERFACETable " + exp.Message.ToString());
                //Console.WriteLine(Ex.Message.ToString());
                //throw Ex;
            }
        }
        /// <summary>
        /// This methos is to use for get insert,Update and Delete records from  PGEDATA.ED06_GISPLDB_INTERFACE table 
        /// and break records into based on batch limit size.
        /// </summary>
        /// <param name="configRetryCount"></param>
        /// <param name="RecordsLimit"></param>
        /// <param name="Edittype"></param>
        ///<returns> Void </returns>
        private void RetreiveDataFromTableAndProcessRecord( int configRetryCount, int RecordsLimit,string Edittype)
        {
            int BatchCount = 0;
            string strPLDBID = string.Empty;
            DataTable ed06StagingTable = new DataTable();
            DataTable ed06StagingBatchTable = new DataTable();
            try
            {
                string query = string.Format("select * from {0} where INTERFACE_STATUS in ('NEW','ERROR') and  (RETRY_COUNT is null  or RETRY_COUNT<" + configRetryCount + " ) and EDIT_Type='" + Edittype + "'", AppConstants.DB_ED06_GISPLDB_INTERFACE);
                DBHelper objDB1 = new DBHelper(AppConstants.DB_Code, Logger);
                ed06StagingTable = objDB1.SelectPGEDataQuery(query, false);
                if (ed06StagingTable !=null && ed06StagingTable.Rows.Count > 0)
                {
                    BatchCount = GetBatchCount(ed06StagingTable.Rows.Count, RecordsLimit);
                    if (BatchCount > 1)
                    {
                        ed06StagingBatchTable.Clear();
                        for (int i = 0; i < BatchCount; i++)
                        {
                            var rowsToDelete = new List<DataRow>();
                            ed06StagingBatchTable = ed06StagingTable.Clone();
                            foreach (DataRow DR in ed06StagingTable.Rows)
                            {
                                if (ed06StagingBatchTable.Rows.Count < RecordsLimit)
                                {
                                    strPLDBID = strPLDBID + DR["PLDBID"].ToString() + ",";
                                    ed06StagingBatchTable.ImportRow(DR);
                                    rowsToDelete.Add(DR);

                                }
                            }
                            rowsToDelete.ForEach(x => ed06StagingTable.Rows.Remove(x));
                            ed06StagingBatchTable.AcceptChanges();
                            ed06StagingTable.AcceptChanges();
                            if (strPLDBID.Length > 0)
                            {
                                strPLDBID = strPLDBID.Substring(0, strPLDBID.Length - 1);
                            }
                            List<Dictionary<string, string>> tldbCommAsset = new List<Dictionary<string, string>>();
                            tldbCommAsset = CategorizeAssetForPLDB(ed06StagingBatchTable);
                            //List<Dictionary<string, string>> tldbCommAsset = CategorizeAssetForPLDB(ed06StagingBatchTable);
                            ProcessAssetData(tldbCommAsset, strPLDBID);
                            strPLDBID = string.Empty;

                        }


                    }
                    else
                    {
                        ed06StagingBatchTable.Clear();
                        ed06StagingBatchTable = ed06StagingTable;
                        foreach (DataRow DR in ed06StagingBatchTable.Rows)
                        {
                            strPLDBID = strPLDBID + DR["PLDBID"].ToString() + ",";
                        }
                        if (strPLDBID.Length > 0)
                        {
                            strPLDBID = strPLDBID.Substring(0, strPLDBID.Length - 1);
                        }
                        List<Dictionary<string, string>> tldbCommAsset = CategorizeAssetForPLDB(ed06StagingBatchTable);
                        ProcessAssetData(tldbCommAsset, strPLDBID);
                    }

                }
                else
                {
                    Logger.Info("No Data in Ed06table with New Or Error ");
                    //  throw new Exception("No Data in Ed06table with New Or Error ...");

                }
            }
            catch (Exception exp)
            {
                if (string.IsNullOrEmpty(Program.Errorcomment))
                {
                    Program.Errorcomment = "Method Name:-RetreiveInsertDataFromTableAndProcess " + exp.Message.ToString();

                }
                else
                {
                    Program.Errorcomment = Program.Errorcomment + Environment.NewLine + "Method Name:-RetreiveInsertDataFromTableAndProcess " + exp.Message.ToString();
                }
                Logger.Error("Method Name:-RetreiveInsertDataFromTableAndProcess " + exp.Message.ToString());
            }
        }
        /// <summary>
        /// This methos is to use for get Completed  records count from PGEDATA.ED06_GISPLDB_INTERFACE table 
        /// </summary>
        ///<returns> int </returns>
        public int GetCompletedRecordcount()
        {
            DataTable dtCompleted = new DataTable();
            string strdate = string.Empty;
            var cultureInfo = new CultureInfo("en-US");
            int RecordCount = 0;

            try
            {
                string strCurrentDate = DateTime.Parse(DateTime.Now.ToShortDateString(), CultureInfo.InvariantCulture).ToString("dd-MMM-yyyy");
                int configRetryCount = Convert.ToInt16(ConfigurationManager.AppSettings["Retry_Count"].ToString());
                if (!string.IsNullOrEmpty(strCurrentDate))
                {
                    string query = string.Format("select count(1)  as recordCount  from   {0} where INTERFACE_STATUS ='COMPLETED' and  (RETRY_COUNT is null  or RETRY_COUNT<" + configRetryCount + " ) and to_char(to_date(LASTMODIFED_DATE))=to_char(to_date('" + strCurrentDate + "'))", AppConstants.DB_ED06_GISPLDB_INTERFACE);
                    DBHelper objDB1 = new DBHelper(AppConstants.DB_Code, Logger);
                    dtCompleted = objDB1.SelectPGEDataQuery(query, false);
                    if (dtCompleted != null && dtCompleted.Rows.Count > 0)
                    {
                        RecordCount = Convert.ToInt32(dtCompleted.Rows[0]["recordCount"].ToString());
                    }
                }

            }
            catch (Exception exp)
            {
                if (string.IsNullOrEmpty(Program.Errorcomment))
                {
                    Program.Errorcomment = "Method Name:-GetCompletedRecord " + exp.Message.ToString();

                }
                else
                {
                    Program.Errorcomment = Program.Errorcomment + Environment.NewLine + "Method Name:-GetCompletedRecordcount " + exp.Message.ToString();
                }
                Logger.Error("Method Name:-GetCompletedRecordcount " + exp.Message.ToString());
            }

            return RecordCount;
        }
        /// <summary>
        /// This methos is to use for get Do Not Process records count from PGEDATA.ED06_GISPLDB_INTERFACE table 
        /// </summary>
        ///<returns> int </returns>
        public int GetDoNotProcesscount()
        {
            DataTable dtCompleted = new DataTable();
            string strdate = string.Empty;
            var cultureInfo = new CultureInfo("en-US");
            int RecordCount = 0;

            try
            {
                string strCurrentDate = DateTime.Parse(DateTime.Now.ToShortDateString(), CultureInfo.InvariantCulture).ToString("dd-MMM-yyyy");
                int configRetryCount = Convert.ToInt16(ConfigurationManager.AppSettings["Retry_Count"].ToString());
                if (!string.IsNullOrEmpty(strCurrentDate))
                {
                    string query = string.Format("select count(1)  as recordCount  from   {0} where INTERFACE_STATUS ='DO_NOT_PROCESS' and  (RETRY_COUNT is null  or RETRY_COUNT<" + configRetryCount + " ) and to_char(to_date(LASTMODIFED_DATE))=to_char(to_date('" + strCurrentDate + "'))", AppConstants.DB_ED06_GISPLDB_INTERFACE);
                    DBHelper objDB1 = new DBHelper(AppConstants.DB_Code, Logger);
                    dtCompleted = objDB1.SelectPGEDataQuery(query, false);
                    if (dtCompleted != null && dtCompleted.Rows.Count > 0)
                    {
                        RecordCount = Convert.ToInt32(dtCompleted.Rows[0]["recordCount"].ToString());
                    }
                }

            }
            catch (Exception exp)
            {
                if (string.IsNullOrEmpty(Program.Errorcomment))
                {
                    Program.Errorcomment = "Method Name:-GetCompletedRecord " + exp.Message.ToString();

                }
                else
                {
                    Program.Errorcomment = Program.Errorcomment + Environment.NewLine + "Method Name:-GetCompletedRecordcount " + exp.Message.ToString();
                }
                Logger.Error("Method Name:-GetCompletedRecordcount " + exp.Message.ToString());
            }

            return RecordCount;
        }
        /// <summary>
        /// This methos is to use for get error records count from PGEDATA.ED06_GISPLDB_INTERFACE table 
        /// </summary>
        ///<returns> int </returns>

        public int GetErrorRecordcount()
        {
            int RecordCount = 0;
            DataTable dtCompleted = new DataTable();
            
            try
            {
                
               int configRetryCount = Convert.ToInt16(ConfigurationManager.AppSettings["Retry_Count"].ToString());               
               string query = string.Format("select count(1)  as recordCount  from   {0} where INTERFACE_STATUS ='ERROR' and  (RETRY_COUNT is null  or RETRY_COUNT<" + configRetryCount + " ) ", AppConstants.DB_ED06_GISPLDB_INTERFACE);
               DBHelper objDB1 = new DBHelper(AppConstants.DB_Code, Logger);
               dtCompleted = objDB1.SelectPGEDataQuery(query, false);
               if(dtCompleted!=null && dtCompleted.Rows.Count>0)
                {
                    RecordCount = Convert.ToInt32(dtCompleted.Rows[0]["recordCount"].ToString());
                }
                

            }
            catch (Exception exp)
            {
                if (string.IsNullOrEmpty(Program.Errorcomment))
                {
                    Program.Errorcomment = "Method Name:-GetErrorRecordcount " + exp.Message.ToString();

                }
                else
                {
                    Program.Errorcomment = Program.Errorcomment + Environment.NewLine + "Method Name:-GetErrorRecordcount " + exp.Message.ToString();
                }
                Logger.Error("Method Name:-GetErrorRecordcount " + exp.Message.ToString());
            }

            return RecordCount;
        }
        /// <summary>
        /// This methos is to use for log the interface success or fail in interface summary table for EDGIS Rearch Project-V1t8
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="comment"></param>
        /// <param name="status"></param>
        public  void ExecutionSummary(string startTime, string comment, string status)
        {
            try
            {

                Argumnet = new StringBuilder();
                remark = new StringBuilder();
                //Setting arguments
                Argumnet.Append(Argument.Interface);
                Argumnet.Append(Argument.Type);
                Argumnet.Append(Argument.Integration);
                Argumnet.Append(startTime + ";");
                remark.Append(comment);
                Argumnet.Append(DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss") + ";");
                Argumnet.Append(status);
                Argumnet.Append(remark);
                AppConstants.intExecutionSummary = ConfigurationManager.AppSettings["IntExecutionSummaryExePath"];
                //To execution for interface execution summary exe.This exe must need some input argument to run successffully. 
                ProcessStartInfo processStartInfo = new ProcessStartInfo(AppConstants.intExecutionSummary, "\"" + Convert.ToString(Argumnet) + "\"");
                processStartInfo.UseShellExecute = false;
                processStartInfo.RedirectStandardOutput = true;
                processStartInfo.CreateNoWindow = true;

                Process proc = new Process();
                proc.StartInfo = processStartInfo;
                proc.EnableRaisingEvents = true;
                proc.Start();
                proc.BeginOutputReadLine();
                while (!proc.HasExited)
                {
                    Thread.Sleep(1000);
                }
            }
            catch (Exception ex)
            {

                Logger.Error("Method Name:-ExecutionSummary " + ex.Message.ToString());

            }
        }
        /// <summary>
        /// Getlog4netConfig
        /// </summary>
        /// <returns></returns>
        private string Getlog4netConfig()
        {

            string Config = @"<?xml version=""1.0"" encoding=""utf-8"" ?> 
                                    <configuration> 
                                      <log4net debug=""False""> 
                                        <!-- Define some output appenders --> 
                                        <appender name=""RollingLogFileAppender"" type=""log4net.Appender.RollingFileAppender""> 
                                          <file type=""log4net.Util.PatternString"" value=""%property{LogName}"" /> 
                                          <appendToFile value=""true"" /> 
                                          <rollingStyle value=""Composite"" />                               
                                         <datePattern value=""ddMMyyyy"" />
                                          <maxSizeRollBackups value=""30"" /> 
                                          <maximumFileSize value=""1MB"" />                                                                                    
                                        <layout type=""log4net.Layout.PatternLayout"">
                                           <conversionPattern value=""%date [%thread] %level %logger - %message%newline""/>
                                        </layout>
                                        </appender> 
                 
                                   <!-- Setup the root category, add the appenders and set the default level --> 
                                   <root> 
                                   <level value=""ALL""/> 
                                   <appender-ref ref=""RollingLogFileAppender"" /> 
                                   </root> 
                                   </log4net> 
                                    </configuration>";
            return Config;
        }
    }


}
