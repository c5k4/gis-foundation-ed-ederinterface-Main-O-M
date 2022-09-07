using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net;
using PGE.BatchApplication.GISPLDBAssetSynchProcess.Common;
using PGE.BatchApplication.GISPLDBAssetSynchProcess.DAL;
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


namespace PGE.BatchApplication.GISPLDBAssetSynchProcess
{
    public class GISPLDBAssetSyncTemplateService : IGISPLDBAssetSyncTemplate
    {
        //  List<Dictionary<string, string>>  asset = new TLDBInsertDictTemplate();
        private log4net.ILog Logger = null;
        List<Dictionary<string, string>> lstDictTemplateInsert = new List<Dictionary<string, string>>();
        List<Dictionary<string, string>> lstDictTemplateUpdate = new List<Dictionary<string, string>>();
        List<Dictionary<string, string>> lstDictTemplateDelete = new List<Dictionary<string, string>>();

        TLDBDictTemplate tldbCommonTemplate = new TLDBDictTemplate();

        //TLDBInsertDictTemplate tldbInsertTemplate = new TLDBInsertDictTemplate();
        //TLDBUpdateDictTemplate tldbUpdateTemplate = new TLDBUpdateDictTemplate();
        //TLDBDeleteDictTemplate tldbDeleteTemplate = new TLDBDeleteDictTemplate();

        public GISPLDBAssetSyncTemplateService()
        {
            InitializeLogger();
            //string Edit_Type = string.Empty;
            //CommonUtil cmnUtil = new CommonUtil();
            //DataTable ed06DataTable = cmnUtil.GetDataFromED06CSV(System.Configuration.ConfigurationManager.AppSettings["NAS_FILE_LOCATION_ED06"]);
            //DataTable ed07DataTable = cmnUtil.ReadCSVDataInTableandMap(System.Configuration.ConfigurationManager.AppSettings["NAS_FILE_LOCATION_ED07"], AppConstants.CNFG_ED07_FIELDMAPPINGSECTION);
            //DataTable ed06TempCopy = new DataTable();
            //ed06TempCopy = cmnUtil.UpdateSapEquipIDandCopyTable(ed06DataTable, ed07DataTable);
        }
        public DataTable ReadandUpdateEd06fromCSV(string Ed06Path, string Ed07path,string Ed07MappingSection)
        {
            CommonUtil cmnUtil = new CommonUtil();
            DataTable ed06DataTable = cmnUtil.GetDataFromED06CSV(Ed06Path);
            DataTable ed07DataTable = cmnUtil.ReadCSVDataInTableandMap(Ed07path, Ed07MappingSection);
            DataTable ed06TempCopy = new DataTable();
            ed06TempCopy = cmnUtil.UpdateSapEquipIDandCopyTable(ed06DataTable, ed07DataTable);
            return ed06TempCopy;
        }
        public void BulkInsertAfterSApEQUIPIDUpdate(DataTable Ed06FinalTable)
        {
            CommonUtil cmnUtil = new CommonUtil();
            if (Ed06FinalTable.Rows.Count > 0)
            {
                if (cmnUtil.BulkInsertintoED06(Ed06FinalTable))
                    Logger.Info("Data Inserted after Updating SAPEQUIPDID");
                else Logger.Info("Data Not Inserted after Updating SAPEQUIPDID");
            }
            else Logger.Info(" \t No proper Ed06 data to update");
        }
        public void RetreiveDataFromTableAndProcess()
        {
            DataTable ed06StagingTable = new DataTable();
            string query = string.Format("select * from {0} where INTERFACE_STATUS in ('NEW','ERROR')", AppConstants.DB_ED06_GISPLDB_INTERFACE);
            DBHelper objDB1 = new DBHelper(AppConstants.DB_Code, Logger);
            ed06StagingTable = objDB1.SelectPGEDataQuery(query, false);

            if (ed06StagingTable.Rows.Count > 0)
            {
                List<Dictionary<string, string>> tldbCommAsset = CategorizeAssetForPLDB(ed06StagingTable);
                ProcessAssetData(tldbCommAsset);
            }
            else
            {
                Logger.Info("No Data in Ed06table with New Or Error ");
                throw new Exception("No Data in Ed06table with New Or Error ...");

            }
        }
        
        private void ProcessAssetData(List<Dictionary<string, string>> TLDBCommonAsset)
        {
            Logger.Info("Processing Data Starts");
            string query = string.Empty;
            DataTable DTvalidateSupportStructTable = new DataTable();
            if (TLDBCommonAsset.Count > 0)
            {
                foreach (var keyvalue in TLDBCommonAsset)
                {
                    query = string.Format("select HFTD,SOURCEACCURACY,LIDARConflatedDate from {1} where PLDBID in ({0})", keyvalue["PLDBID"], AppConstants.DB_SUPPORT_STRUCTURE);
                    DBHelper objDB1 = new DBHelper(AppConstants.DB_Code, Logger);

                    if (keyvalue["EDIT_TYPE"] == "I")
                    {
                        
                        DTvalidateSupportStructTable = objDB1.SelectQuery(query, false);

                        if (DTvalidateSupportStructTable.Rows.Count > 0)
                        {
                            foreach (DataRow dr in DTvalidateSupportStructTable.Rows)
                            {

                                Dictionary<string, string> dictFinalList = new Dictionary<string, string>();
                                string Final_Steps = ReadConflationRules(dr["HFTD"].ToString(), dr["SOURCEACCURACY"].ToString(), dr["LIDARConflatedDate"].ToString(), "Insert");
                                string ruletoImplement = Final_Steps + "_" + "Insert";

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
                                else Logger.Info("No Insert records to send to PLDB " + keyvalue["PLDBID"]);
                                

                            }
                        }
                        else Logger.Info("No Rows to send with " + keyvalue["PLDBID"]);

                    }
                    else if (keyvalue["EDIT_TYPE"] == "U")
                    {

                        DTvalidateSupportStructTable = objDB1.SelectQuery(query, false);
                        if (DTvalidateSupportStructTable.Rows.Count > 0)
                        {
                            foreach (DataRow dr in DTvalidateSupportStructTable.Rows)
                            {
                                Dictionary<string, string> dictFinalList = new Dictionary<string, string>();
                                string Final_Steps = ReadConflationRules(dr["HFTD"].ToString(), dr["SOURCEACCURACY"].ToString(), dr["LIDARConflatedDate"].ToString(), "Update");
                                string ruletoImplement = Final_Steps + "_" + "Update";
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
                                else Logger.Info("No Updated to send to PLDB " + keyvalue["PLDBID"]);

                            }
                        }
                        else Logger.Info("No Rows to send with " + keyvalue["PLDBID"]);
                    }
                    else
                    {
                        DTvalidateSupportStructTable = objDB1.SelectQuery(query, false);
                        if (DTvalidateSupportStructTable.Rows.Count > 0)
                        {
                            foreach (DataRow dr in DTvalidateSupportStructTable.Rows)
                            {

                                Dictionary<string, string> dictFinalList = new Dictionary<string, string>();
                                string Final_Steps = ReadConflationRules(dr["HFTD"].ToString(), dr["SOURCEACCURACY"].ToString(), dr["LIDARConflatedDate"].ToString(),"Delete");
                                string ruletoImplement = Final_Steps + "_" + "Delete";
                                var FinalFieldListToSend = new List<string>(ConfigurationManager.AppSettings[ruletoImplement].Split(new char[] { ';' }));
                                if (ConfigurationManager.AppSettings[ruletoImplement].ToString() != "")
                                {
                                    foreach (string Fields in FinalFieldListToSend)
                                    {

                                        dictFinalList.Add(Fields, keyvalue[Fields].ToString());
                                        //   JsonConvert.
                                    }
                                    lstDictTemplateDelete.Add(dictFinalList);
                                }
                                else Logger.Info("No Rows to delete for " + keyvalue["PLDBID"]);
                            }
                        }

                    }

                }
            }
            else
            {
                Logger.Info("NO Data available to Insert");
            }
            Logger.Info("Data Process Ends");
            Logger.Info("Preparing PLDB Details to send it to EI");
            if (lstDictTemplateInsert.Count > 0)
              ProcessDictionaryandPost(lstDictTemplateInsert, "Insert");
               // PostListOfDataforInsert(lstDictTemplateInsert, "Insert");
            else Logger.Info("NO Data available to Insert");

            if(lstDictTemplateUpdate.Count > 0)         
                ProcessDictionaryandPost(lstDictTemplateUpdate, "Update");
            else Logger.Info("NO Data available to Update");

            if(lstDictTemplateDelete.Count >0)
                ProcessDictionaryandPost(lstDictTemplateDelete, "Delete");
            else Logger.Info("NO Data available to Delete");
            Logger.Info("Ending the process of sending PLDB Details to EI");

        }
        private void ProcessDictionaryandPost(List<Dictionary<string, string>> lstDictCommon, string ActionType)
        {
            try
            {
                if (lstDictCommon.Count > 0)
                {
                    foreach (var Dict in lstDictCommon)
                    {
                        //string Response = postDataToPLDB(Dict, ActionType);
                        string Response = postData(Dict, ActionType);
                        Logger.Info(Response);
                    }
                }
            }
            catch (WebException ex) 
            {
                Logger.Error(" \t  \t  Error In updateInterfaceStatusSENT Method" + ex.Message);
                //return false;
            }
        }

        private bool UpdateInterfaceStatusSENT(string PLDBID, string status)
        {
            try
            {
                Logger.Info(" \t updateInterfaceStatusSENT Start");
                if (PLDBID == null || PLDBID.Trim() == string.Empty)
                {
                    throw new Exception("updateInterfaceStatusSENT --  BatchID can not be null .. ");
                }
                DBHelper objDBhelper = new DBHelper(AppConstants.DB_Code, Logger);
               // int updateTLDBID = Convert.ToInt32(PLDBID);
                string query = string.Format("update {1} set INTERFACE_STATUS= '{2}', LASTMODIFED_DATE = sysdate where PLDBID = '{0}'", PLDBID, AppConstants.DB_ED06_GISPLDB_INTERFACE, status);
                objDBhelper.ExecuteScalarPGEData(query);
                Logger.Info(" \t updateInterfaceStatusSENT Complete ..");
                return true;
            }
            catch (Exception Ex)
            {
                Logger.Error(" \t  \t  Error In updateInterfaceStatusSENT Method", Ex);
                return false;
                //throw Ex;
            }
        }
        private string serializePLDBDict(Dictionary<string,string> Dict)
        {
            
           // JavaScriptSerializer js = new JavaScriptSerializer();
            string s = JsonConvert.SerializeObject(Dict);
            Logger.Info("GetAssetByETIntID - Serialization Completed.");
            return s;
        }
        public string AddObjectsToJson<T>(string json, List<T> objects)
        {
            List<T> list = JsonConvert.DeserializeObject<List<T>>(json);
            list.AddRange(objects);
            return JsonConvert.SerializeObject(list);
        }
        public string PostListOfDataforInsert(List<Dictionary<string, string>> dictList, string ActionType)
        {
            var responseText = "";
            string responseToSend = string.Empty;
            foreach (var dictSend in dictList)
            {
                string urlToSend = string.Empty;
                try
                {
                    if (ActionType == "Insert")
                        urlToSend = System.Configuration.ConfigurationManager.AppSettings["SetSAPEQUIPIDURL"];
                    else if (ActionType == "Update")
                        urlToSend = System.Configuration.ConfigurationManager.AppSettings["UpdateLocationsURL"];
                    else urlToSend = System.Configuration.ConfigurationManager.AppSettings["DecommissionPoleURL"];

                    string responseFromServer = string.Empty;

                    var Jsontosend = JsonConvert.SerializeObject(dictSend, Formatting.Indented);

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
                        streamWriter.Write(Jsontosend);
                        streamWriter.Flush();
                    }
                    var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                    using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                    {
                        responseText = streamReader.ReadToEnd();
                        Console.WriteLine(responseText);

                        //Now you have your response.
                        //or false depending on information in the response     
                    }
                    Logger.Info("Web Request Sent to PLDB");
                    UpdateInterfaceStatusSENT(dictSend["PLDBID"], ConfigurationManager.AppSettings["sentStatus"].ToString());

                }
                catch (WebException ex)
                {
                    responseToSend = new StreamReader(ex.Response.GetResponseStream()).ReadToEnd().ToString();
                    UpdateErrorRecord(dictSend["PLDBID"], ConfigurationManager.AppSettings["errorStatus"].ToString());
                    //string Res = (HttpWebResponse)response).StatusDescription
                    Logger.Info("Failed to send Request" + dictSend.ToString() + responseToSend + ex.Message);
                    return responseToSend;
                }
            }
            return responseText;
        }
        public string postData(Dictionary<string, string> dictSend, string ActionType)
        {
            string ResponseToSend = string.Empty;
            List<Dictionary<string, string>> dictListFinal = new  List<Dictionary<string, string>>();
            dictListFinal.Add(dictSend);
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
                streamWriter.Write(Jsontosend);
                streamWriter.Flush();
            }
            var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                responseText = streamReader.ReadToEnd();
                Console.WriteLine(responseText);

                //Now you have your response.
                //or false depending on information in the response     
            }
            ResponseToSend = "Reponse Status Descritiion from EI:" + ((HttpWebResponse)httpResponse).StatusDescription;
            Logger.Info("Web Request Sent to PLDB");
            UpdateInterfaceStatusSENT(dictSend["PLDBID"], ConfigurationManager.AppSettings["sentStatus"].ToString()); 
            
            
            }
             catch (WebException ex)
             {
                 ResponseToSend = new StreamReader(ex.Response.GetResponseStream()).ReadToEnd().ToString();
                 UpdateErrorRecord(dictSend["PLDBID"], ConfigurationManager.AppSettings["errorStatus"].ToString());
                 //string Res = (HttpWebResponse)response).StatusDescription
                 Logger.Info("Failed to send Request" + dictSend.ToString() + responseToSend + ex.Message);
                 return ResponseToSend;
             }
            return ResponseToSend;
        }
        public string postDataToPLDB(Dictionary<string, string> dictSend, string ActionType)
        {
            lstDictTemplateInsert.Add(dictSend);
            string responseFromServer = string.Empty;
            string urlToSend = string.Empty;
            string responseToSend = string.Empty;
           // var Jsontosend = JsonConvert.SerializeObject(dictSend, Formatting.Indented);
            var Jsontosend = JsonConvert.SerializeObject(lstDictTemplateInsert, Formatting.Indented);
            try
            {
                if (ActionType == "Insert")
                    urlToSend = System.Configuration.ConfigurationManager.AppSettings["SetSAPEQUIPIDURL"];
                else if (ActionType == "Update")
                    urlToSend = System.Configuration.ConfigurationManager.AppSettings["UpdateLocationsURL"];
                else urlToSend = System.Configuration.ConfigurationManager.AppSettings["DecommissionPoleURL"];

                Logger.Info("Creating " + ActionType.ToUpper() + "WebRequest to send details to EI" + dictSend.ToString());
                HttpWebRequest webRequest = (HttpWebRequest)HttpWebRequest.Create(String.Format(urlToSend));
                webRequest.Method = ConfigurationManager.AppSettings["RequestMethod"];
                byte[] byteArray = Encoding.UTF8.GetBytes(Jsontosend);
                webRequest.ContentType = "application/json";
                webRequest.Credentials = new NetworkCredential(System.Configuration.ConfigurationManager.AppSettings["serviceUserName"].ToString(), System.Configuration.ConfigurationManager.AppSettings["servicePassword"].ToString());

                webRequest.ContentLength = byteArray.Length;
                Stream dataStream = webRequest.GetRequestStream();
                dataStream.Write(byteArray, 0, byteArray.Length);
                dataStream.Close();
                Logger.Info("Fetching Response from EI");
                WebResponse response = webRequest.GetResponse();
                Console.WriteLine(((HttpWebResponse)response).StatusDescription);
                responseToSend ="Reponse Status Descritiion from EI:"+ ((HttpWebResponse)response).StatusDescription;
                dataStream = response.GetResponseStream();

                StreamReader reader = new StreamReader(dataStream);
                // Read the content.
                responseFromServer = reader.ReadToEnd();
                Logger.Info("EI Response: " + responseFromServer);
                Console.WriteLine(responseFromServer);
                reader.Close();
                dataStream.Close();
                response.Close();
                // Clean up the streams.
                Logger.Info("Web Request Sent to PLDB");
                UpdateInterfaceStatusSENT(dictSend["PLDBID"], ConfigurationManager.AppSettings["sentStatus"].ToString());
            }
            catch (WebException ex)
            {
                responseToSend = new StreamReader(ex.Response.GetResponseStream()).ReadToEnd().ToString();
                UpdateErrorRecord(dictSend["PLDBID"], ConfigurationManager.AppSettings["errorStatus"].ToString());
                //string Res = (HttpWebResponse)response).StatusDescription
                Logger.Info("Failed to send Request" + dictSend.ToString() + responseToSend + ex.Message);
                return responseToSend;
            }
            //webRequest.Proxy = GlobalProxySelection.GetEmptyWebProxy();
            return responseToSend;
        }
        private void UpdateErrorRecord(string PLDBID,String Status)
        {
            try
            {
                Logger.Info(" \t updateErrorRecord Start");
                DBHelper objDBhelper = new DBHelper(AppConstants.DB_Code, Logger);
              //  int updateTLDBID = Convert.ToDouble(PLDBID);
                string query = string.Format("update {1} set INTERFACE_STATUS= '{2}', LASTMODIFED_DATE = sysdate where PLDBID = '{0}'", PLDBID, AppConstants.DB_ED06_GISPLDB_INTERFACE, Status);
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

        //private string serializeTLDBDict(Dictionary<string, string t)
        //{

        //    var json = new JavaScriptSerializer().Serialize(yourDictionary.ToDictionary(item => item.Key.ToString(), item => item.Value.ToString()));
        //    JavaScriptSerializer js = new JavaScriptSerializer();
        //    string s = js.Serialize(t);
        //    Logger.Info("GetAssetByETIntID - Serialization Completed.");
        //    return s;
        //}
        private string ReadConflationRules(string HFTD, string SourceAccuracy, string LIDARConflatedDate,string Method)
        {
            Logger.Info("Reading Conflation Rules Starts");
            var connectionManagerDataSection = ConfigurationManager.GetSection(ConfigRuleReader.SectionName) as ConfigRuleReader;
            //  int ConvSourceAccuracy = (int) SourceAccuracy;
            string final_step = ConfigurationManager.AppSettings["CommonUpdateRule"].ToString();
            try
            {
                if (connectionManagerDataSection != null)
                {
                    foreach (RuleElement element in connectionManagerDataSection.ConnectionManagerEndpoints)
                    {
                        if (element.HFTD.Contains(HFTD.ToString()))
                        {
                            switch (HFTD)
                            {
                                case "0":
                                    final_step = element.ruleDesc;
                                    break;
                                case "2":
                                case "3":
                                    if (element.SourceAccuracy.Contains(SourceAccuracy.ToString()))
                                   //if (element.SourceAccuracy.Contains(SourceAccuracy.ToString()) && LIDARConflatedDate == element.LIDARConflatedDate)
                                    {
                                        //if (SourceAccuracy ==  || SourceAccuracy == 31)
                                        if (element.Name == "BOTH")
                                            final_step = element.ruleDesc;
                                    }
                                    else if (!(element.SourceAccuracy.Contains(SourceAccuracy.ToString())))
                                    {
                                        if (LIDARConflatedDate != element.LIDARConflatedDate || LIDARConflatedDate != null)
                                        {
                                            if (element.Name == "BOTHWITHDATE")
                                                final_step = element.ruleDesc;
                                        }
                                        else
                                        {
                                            // waiting for the confirmation
                                            if (LIDARConflatedDate == "" || LIDARConflatedDate == null)
                                            {
                                                if (element.Name == "BOTHWITHOUTDATE")
                                                    final_step = element.ruleDesc;
                                           }

                                        }
                                    }
                                     // else if(
                                    break;
                            }
                            //final_step = element.ruleDesc;
                           
                        }

                    }

                }
            }
            catch (Exception ex)
            {
                Logger.Info("Error Reading Rules" + ex);
            }

            Logger.Info("Reading Conflation Rules Ends");
            return final_step;

        }
        private List<Dictionary<string, string>> CategorizeAssetForPLDB(DataTable DTEd06Table)
        {
            Logger.Info("Categorizing the Assets According to Type Starts");
            NameValueCollection FieldList = new NameValueCollection();
            //      FieldList = ConfigurationManager.GetSection("Insert_FieldList") as NameValueCollection;

            foreach (DataRow dr in DTEd06Table.Rows)
            {
                Dictionary<string, string> dictAsset = new Dictionary<string, string>();
                if (dr["Edit_TYPE"].ToString() == "I")
                {
                    Logger.Info(" \t Edit Type of Insert Found");
                    FieldList = ConfigurationManager.GetSection("Insert_FieldList") as NameValueCollection;

                 //   var result = FieldList.Keys  Select(m => m.ToUpper());
                    foreach (DataColumn dc in dr.Table.Columns)
                    {
                        string columnKey= System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(dc.ColumnName.ToLower());
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
                    Logger.Info(" \t Edit Type of Update Found");
                    FieldList = ConfigurationManager.GetSection("Update_FieldList") as NameValueCollection;
                    foreach (DataColumn dc in dr.Table.Columns)
                    {

                        //if (FieldList.AllKeys.Contains(dc.ColumnName))
                        //    dictAsset.Add(dc.ColumnName, dr[dc.ColumnName].ToString());
                        string columnKey = System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(dc.ColumnName.ToLower());
                        if (FieldList.AllKeys.Contains(columnKey))
                        {
                            if (!dictAsset.ContainsKey(dc.ColumnName))
                                dictAsset.Add(FieldList.Get(columnKey).ToString(), dr[dc.ColumnName].ToString());
                        }
                    }
                    //  lstDictTemplateUpdate.Add(dictAsset);
                    //tldbUpdateTemplate.ASSET.Add(dictAsset);
                    //  tldbCommonTemplate.UpdateASSET.Add(dictAsset);
                    tldbCommonTemplate.ASSET.Add(dictAsset);
                }
                else
                {
                    if (dr["Edit_TYPE"].ToString() == "D")
                    {
                        Logger.Info(" \t Edit Type of Delete Found");
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
                        //lstDictTemplateDelete.Add(dictAsset);
                        //  tldbCommonTemplate.DeleteASSET.Add(dictAsset);
                        tldbCommonTemplate.ASSET.Add(dictAsset);
                    }
                    else Logger.Info(" \t Wrong Edit Type Found");
                }

            }
            //   lstDictTemplateInsert.Add(dictAsset);
            Logger.Info("Categorizing the Assets According to Type Ends");
            return tldbCommonTemplate.ASSET;
        }

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
                Logger.Info(ex.Message);
            }
        }
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
                Logger.Info("Error in creating log file path" + ex.Message);
            }
            return "";
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

