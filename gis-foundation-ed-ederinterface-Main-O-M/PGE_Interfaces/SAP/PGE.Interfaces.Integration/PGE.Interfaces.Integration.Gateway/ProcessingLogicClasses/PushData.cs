// ========================================================================
// Copyright © 2021 PGE.
// <history>
// Push Data to SAP
// TCS V3SF (EDGISREARC-373) 05/10/2021                             Created
// </history>
// All rights reserved.
// ========================================================================

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;

namespace PGE.Interfaces.Integration.Gateway
{
    class PushData
    {
        //private DBHelper dBHelper = new DBHelper();

        /// <summary>
        /// (V3SF) 
        /// (Outbound Jobs)
        /// Prepares Data for sharing from Staging Tables
        /// Creates WEB Request for Layer 7
        /// </summary>
        /// <param name="Interface_Name">Name of Interface like (ED08 or ED06)</param>
        public void CreateWebRequest(string Interface_Name)
        {
            #region Variables
            int batch_size = default;
            int recCount = default;
            int batchCount = default;
            int batRecCount = default;
            string columnNames = default;
            string staging_table = default;
            string database_name = default;
            string batchidField = default;
            string recordidField = default;
            string interface_number = default;
            string batchIDValue = default;
            string urlstr = default;
            string json = default;
            string urlToSend = default;
            string responseText = default;
            string TypeField = default;
            string addWhereClause = default;
            string startJsonTag = default;
            string jsonType = default;
            List<string> BatchIDList = default;
            Dictionary<string, string> ColumnMapping = default;
            Dictionary<string, List<string>> TypeTypeValue = default;
            DataTable stagingDataTable = default;
            WebRequest httpWebRequest = default;
            HttpWebResponse httpResponse = default;
            List<string> PostPassed = default;
            List<string> PostFailed = default;
            List<string> ResponsePassed = default;
            List<string> ResponseFailed = default;
            DateTime startDateTime = default;
            #endregion

            try
            {

                {
                    #region Commented
                    //staging_Table = DBHelper.GetStagingDataTable(ReadConfiguration.ed08StagingTableName);

                    //string test3 = "{\"NAV_ED06_ST_KF_KV\":[{\"KeyField\":\"ActionType\",\"KeyValue\":\"U\"},{\"KeyField\":\"IDType\",\"KeyValue\":\"99\"},{\"KeyField\":\"LocDesc2\",\"KeyValue\":\"\"},{\"KeyField\":\"Barcode\",\"KeyValue\":\"\"},{\"KeyField\":\"Class\",\"KeyValue\":\"\"},{\"KeyField\":\"Material\",\"KeyValue\":\"\"},{\"KeyField\":\"Species\",\"KeyValue\":\"\"},{\"KeyField\":\"Height\",\"KeyValue\":\"\"},{\"KeyField\":\"JPNumber\",\"KeyValue\":\"PT10945\"},{\"KeyField\":\"CustomerOwned\",\"KeyValue\":\"N\"},{\"KeyField\":\"OriginalCircumference\",\"KeyValue\":\"39\"},{\"KeyField\":\"OriginalTreatmentType\",\"KeyValue\":\"P\"},{\"KeyField\":\"SAPEquipID\",\"KeyValue\":\"102242631\"},{\"KeyField\":\"InstallationDate\",\"KeyValue\":\"19600101\"},{\"KeyField\":\"InstallJobNumber\",\"KeyValue\":\"999999\"},{\"KeyField\":\"Manufacturer\",\"KeyValue\":\"\"},{\"KeyField\":\"ManufacturedYear\",\"KeyValue\":\"1960\"},{\"KeyField\":\"LocDesc1\",\"KeyValue\":\"# 26 ALDERNEY AVE\"},{\"KeyField\":\"LocalOfficeID\",\"KeyValue\":\"\"},{\"KeyField\":\"y\",\"KeyValue\":\"37.9840883556\"},{\"KeyField\":\"x\",\"KeyValue\":\"-122.5682546432\"},{\"KeyField\":\"MapNumber\",\"KeyValue\":\"TT3105\"},{\"KeyField\":\"CircuitMapNumber\",\"KeyValue\":\"\"},{\"KeyField\":\"City\",\"KeyValue\":\"San Anselmo\"},{\"KeyField\":\"County\",\"KeyValue\":\"\"},{\"KeyField\":\"Zip\",\"KeyValue\":\"94960\"},{\"KeyField\":\"GUID\",\"KeyValue\":\"{9583B15C-EF54-4C39-9CE1-74F8A2833C95}\"},{\"KeyField\":\"ParentGUID\",\"KeyValue\":\"\"},{\"KeyField\":\"LastUser\",\"KeyValue\":\"M4JF\"},{\"KeyField\":\"LastModified\",\"KeyValue\":\"20210512\"},{\"KeyField\":\"GEMS Map office\",\"KeyValue\":\"\"},{\"KeyField\":\"SubtypeCD\",\"KeyValue\":\"8\"},{\"KeyField\":\"PoleUse\",\"KeyValue\":\"4\"},{\"KeyField\":\"PTTDIDC\",\"KeyValue\":\"\"},{\"KeyField\":\"PLDBID\",\"KeyValue\":\"7700443314\"},{\"KeyField\":\"Comments\",\"KeyValue\":\"\"},{\"KeyField\":\"HFTD\",\"KeyValue\":\"0\"}] }";
                    //DataSet dataSet = JsonConvert.DeserializeObject<DataSet>(test3);

                    //DataTable dataTable = dataSet.Tables[0];
                    //Console.WriteLine(dataTable.TableName);
                    #endregion

                    startDateTime = DateTime.Now;

                    #region Configration Values
                    #region INTERFACE_STAGINGTABLE_CONFIG 
                    Common._log.Info("Fetching values from " + ReadConfiguration.INTERFACE_STAGINGTABLE_CONFIG);
                    if (!DBHelper.GetINTERFACE_STAGINGTABLE_CONFIG(Interface_Name.ToUpper(), out staging_table, out database_name, out batch_size, out interface_number))
                    {
                        throw new Exception("Staging Table Config for " + Interface_Name.ToUpper() + " not found");
                    }

                    Common._log.Info("Staging Table :: " + staging_table);
                    Common._log.Info("Database Connection :: " + database_name);
                    Common._log.Info("Batch Size :: " + batch_size);
                    Common._log.Info("Interface Number :: " + interface_number);
                    #endregion

                    #region Type Field Check
                    Common._log.Info("Fetching Type values from " + ReadConfiguration.INTEGRATION_FIELD_MAPPING);
                    if (!DBHelper.GetTypeFieldValue(Interface_Name.ToUpper(), out TypeTypeValue, out TypeField))
                    {
                        throw new Exception("Unable to execute GetTypeFieldValue Function");
                    }

                    Common._log.Info("Fetched Type values from " + ReadConfiguration.INTEGRATION_FIELD_MAPPING);

                    if (!string.IsNullOrEmpty(TypeField))
                    {
                        Common._log.Info("Type Field :: " + TypeField);
                        Common._log.Info("Types Found :: " + TypeTypeValue.Count());

                        if (TypeTypeValue.Count() > 0)
                        {
                            foreach (string key in TypeTypeValue.Keys)
                            {
                                Common._log.Info("SAP Type :: " + key + " :: " + string.Join(", ", TypeTypeValue[key]));
                            }
                        }
                    }
                    else
                    {
                        Common._log.Info("No Type Field Found for Interface :: " + Interface_Name.ToUpper());
                    }
                    #endregion

                    #region INTEGRATION_FIELD_MAPPING
                    Common._log.Info("Fetching values from " + ReadConfiguration.INTEGRATION_FIELD_MAPPING);
                    if (!DBHelper.GetINTEGRATION_FIELD_MAPPING(Interface_Name.ToUpper(), out ColumnMapping, out columnNames))
                    {
                        throw new Exception("Field Mapping for " + Interface_Name.ToUpper() + " not found");
                    }

                    Common._log.Info("Fetched values from " + ReadConfiguration.INTEGRATION_FIELD_MAPPING);

                    if (ColumnMapping.Count() > 0)
                    {
                        Common._log.Info("Field Mapping for " + Interface_Name.ToUpper());
                        foreach (string key in ColumnMapping.Keys)
                        {
                            Common._log.Info("SAP :: " + key + " , GIS ::" + ColumnMapping[key]);
                        }
                    }
                    #endregion

                    #region Fetching Staging Table Field Names for Record ID and Batch ID Fields
                    if (ColumnMapping.ContainsKey(ReadConfiguration.SAPRecordIdField))
                    {
                        recordidField = ColumnMapping[ReadConfiguration.SAPRecordIdField];
                    }
                    else
                    {
                        throw new Exception("Field Mapping of " + ReadConfiguration.SAPRecordIdField + " for " + Interface_Name.ToUpper() + " not found");
                    }

                    if (ColumnMapping.ContainsKey(ReadConfiguration.SAPBatchIdField))
                    {
                        batchidField = ColumnMapping[ReadConfiguration.SAPBatchIdField];
                    }
                    else
                    {
                        throw new Exception("Field Mapping of " + ReadConfiguration.SAPBatchIdField + " for " + Interface_Name.ToUpper() + " not found");
                    }
                    #endregion
                    #endregion

                    #region Update BatchID and ProcessFlag

                    addWhereClause = default;

                    if (string.IsNullOrWhiteSpace(TypeField))
                    {
                        //Get Record Count of GIS Processed Record(s)
                        Common._log.Info("Fetching GIS Processed Record Count...");
                        recCount = DBHelper.GetRecordCount(database_name, staging_table, ReadConfiguration.GISErrorField, batchidField, ReadConfiguration.GISProcessFlagField, ProcessFlag.GISProcessed);
                        Common._log.Info("GIS Processed Record Count :: " + recCount);

                        if (recCount > 0)
                        {
                            //Calculate Batch Count by dividing GIS Processed Record Count with Batch Size for the Interface
                            Common._log.Info("Calculating GIS Processed Batch Count...");
                            batchCount = Convert.ToInt32(Math.Ceiling((double)(recCount / (double)batch_size)));
                            Common._log.Info("GIS Processed Batch Count Found :: " + batchCount);

                            //Limit Batch count to max 99 
                            if (batchCount > 99)
                            {
                                Common._log.Info("Limiting Batch Count to 99");
                                batchCount = 99;
                            }

                            //Updating Batch ID for GIS Processed Record(s)
                            for (int i = 1; i <= batchCount; i++)
                            {
                                //Calculate Batch ID Value
                                Common._log.Info("Calculating Batch ID Value...");
                                batchIDValue = PullDataFromStagArea.CreateBatchID(interface_number, batchCount, i);
                                Common._log.Info("Batch ID Value :: " + batchIDValue);

                                batRecCount = DBHelper.GetBatchRecordCount(database_name, staging_table, batchidField, batchIDValue);
                                if (batRecCount == 0)
                                {
                                    //Assign Batch ID and GIS Process Flag value as In Transition
                                    Common._log.Info("Updating Batch ID Value and Process Flag Value to In Transition...");
                                    if (DBHelper.UpdateBatchIDProcessFlag(database_name, staging_table, batchidField, ReadConfiguration.GISProcessFlagField, ProcessFlag.InTransition, batchIDValue, ReadConfiguration.GISErrorField, recordidField, batch_size, addWhereClause))
                                    {
                                        Common._log.Info(batchIDValue + " Updated in " + staging_table + " Table");
                                        batRecCount = DBHelper.GetBatchRecordCount(database_name, staging_table, batchidField, batchIDValue);
                                        Common._log.Info(batchIDValue + " Updated Count in " + staging_table + " Table :: " + batRecCount);
                                    }
                                    else
                                        Common._log.Error("Unable to update BatchId ( " + batchIDValue + ")");
                                }
                                else
                                {
                                    Common._log.Info(batchIDValue + " Already exists in staging table");
                                    Common._log.Error("Unable to update BatchId ( " + batchIDValue + ")");
                                }
                            }
                        }
                        else
                        {
                            Common._log.Info("No " + ProcessFlag.GISProcessed + "Records found for " + Interface_Name.ToUpper() + " Interface");
                        }
                    }
                    else if (!string.IsNullOrWhiteSpace(TypeField) && TypeTypeValue.Count > 0)
                    {
                        foreach (string type in TypeTypeValue.Keys)
                        {
                            //Reset Where Clause
                            addWhereClause = default;

                            Common._log.Info("Updating Update BatchID and ProcessFlag for Type :: " + type);

                            //Get Record Count of GIS Processed Record(s)
                            Common._log.Info("Fetching GIS Processed Record Count...");
                            recCount = DBHelper.GetRecordCount(database_name, staging_table, ReadConfiguration.GISErrorField, batchidField, ReadConfiguration.GISProcessFlagField, ProcessFlag.GISProcessed, TypeField, TypeTypeValue[type], out addWhereClause);
                            Common._log.Info("GIS Processed Record Count :: " + recCount);

                            if (recCount > 0)
                            {
                                //Calculate Batch Count by dividing GIS Processed Record Count with Batch Size for the Interface
                                Common._log.Info("Calculating GIS Processed Batch Count...");
                                batchCount = Convert.ToInt32(Math.Ceiling((double)(recCount / (double)batch_size)));
                                Common._log.Info("GIS Processed Batch Count Found :: " + batchCount);

                                //Limit Batch count to max 99 
                                if (batchCount > 99)
                                {
                                    Common._log.Info("Limiting Batch Count to 99");
                                    batchCount = 99;
                                }

                                //Updating Batch ID for GIS Processed Record(s)
                                for (int i = 1; i <= batchCount; i++)
                                {
                                    //Calculate Batch ID Value
                                    Common._log.Info("Calculating Batch ID Value...");
                                    batchIDValue = PullDataFromStagArea.CreateBatchID(interface_number, batchCount, i, type);
                                    Common._log.Info("Batch ID Value :: " + batchIDValue);

                                    batRecCount = DBHelper.GetBatchRecordCount(database_name, staging_table, batchidField, batchIDValue);
                                    if (batRecCount == 0)
                                    {
                                        //Assign Batch ID and GIS Process Flag value as In Transition
                                        Common._log.Info("Updating Batch ID Value and Process Flag Value to In Transition...");
                                        if (DBHelper.UpdateBatchIDProcessFlag(database_name, staging_table, batchidField, ReadConfiguration.GISProcessFlagField, ProcessFlag.InTransition, batchIDValue, ReadConfiguration.GISErrorField, recordidField, batch_size, addWhereClause))
                                        {
                                            Common._log.Info(batchIDValue + " Updated in " + staging_table + " Table");
                                            batRecCount = DBHelper.GetBatchRecordCount(database_name, staging_table, batchidField, batchIDValue);
                                            Common._log.Info(batchIDValue + " Updated Count in " + staging_table + " Table :: "+ batRecCount);
                                        }
                                        else
                                            Common._log.Error("Unable to update BatchId ( " + batchIDValue + ")");
                                    }
                                    else
                                    {
                                        Common._log.Info(batchIDValue + " Already exists in staging table");
                                        Common._log.Error("Unable to update BatchId ( " + batchIDValue + ")");
                                    }
                                }
                            }
                            else
                            {
                                Common._log.Info("No " + ProcessFlag.GISProcessed + "Records found for " + Interface_Name.ToUpper() + " Interface of Type " + type);
                            }

                        }
                    }
                    #endregion

                    #region Get BatchID List of unprocessed Records
                    //Get BatchID List of unprocessed Records
                    BatchIDList = new List<string>();
                    //V3SF (25 Nov 2021) - Update Re-process Records with T - In Transaction and F - Failed Status
                    if (!DBHelper.GetUnProcessedBatchIdList(database_name, staging_table, batchidField, ReadConfiguration.GISProcessFlagField, out BatchIDList))
                    {
                        throw new Exception("Unable to fetch unprocessed batchids for " + Interface_Name.ToUpper());
                    }

                    //Logs list of Batch IDs
                    if (BatchIDList.Count() > 0)
                    {
                        Common._log.Info("Batch ID's Found :: " + string.Join(", ", BatchIDList));
                    }

                    //For Execution Summary
                    PostPassed = new List<string>();
                    PostFailed = new List<string>();
                    ResponsePassed = new List<string>();
                    ResponseFailed = new List<string>();

                    #endregion


                    //Fetch Values to share with Layer 7 Gateway
                    foreach (string batchId in BatchIDList)
                    {
                        try
                        {
                            #region Prepare Posting Json
                            //Type value if different type of data is shared using the interface
                            jsonType = default;
                            stagingDataTable = new DataTable();

                            //Startting Tag/Key of Json String
                            startJsonTag = "NAV_" + Interface_Name.ToUpper();
                            Common._log.Info("Start Tag :: " + startJsonTag);

                            //Get Staging Table Values with given BatchID
                            Common._log.Info("Populating Data Table for Batch ID :: " + batchId);
                            stagingDataTable = DBHelper.GetStagingTableValues(database_name, staging_table, batchId, columnNames);
                            Common._log.Info(stagingDataTable.Rows.Count + " Records found for Batch ID :: " + batchId);

                            //Change Column names with one required by SAP
                            Common._log.Info("Changing Column names with one required by SAP for Batch ID :: " + batchId);
                            stagingDataTable = PullDataFromStagArea.SetColumnName(stagingDataTable, ColumnMapping);
                            Common._log.Info("Changed Column names with one required by SAP for Batch ID :: " + batchId);

                            //change null to " "
                            //Common._log.Info("Replacing Null Values with \" \" for Batch ID :: " + batchId);
                            //stagingDataTable = PullDataFromStagArea.NullValuetoSpace(stagingDataTable);
                            //Common._log.Info("Replaced Null Values with \" \" for Batch ID :: " + batchId);

                            //Change Json Column Value to DataTable Value
                            Common._log.Info("Building Json String for Batch ID :: " + batchId);
                            json = PullDataFromStagArea.SetJsonColumns(stagingDataTable, startJsonTag, recordidField, out jsonType);
                            Common._log.Info("Built Json String for Batch ID :: " + batchId);

                            //Serealize Datatable to jasn object
                            //json = JsonConvert.SerializeObject(stagingDataTable);

                            //Create string navtag with type for type fields
                            if (!string.IsNullOrWhiteSpace(jsonType))
                                json = "{\"" + startJsonTag + "_" + jsonType + "\":" + json + "}";
                            else
                                json = "{\"" + startJsonTag + "\":" + json + "}";

                            //string jsontest = "{  \"InterfaceName\": \"ED08\",  \"NAV_ED08\": [    {        \"RecordId\" : \"ED.0.08.20210405.111522.0000001\",        \"BatchId\" : \"ED.0.08.20210405.04.BT.01\",        \"Guid\" : \"{B909265F-13D5-44C8-957E-6642F3522D2B}\",        \"Equnr\" : \"103133799\",        \"Class\" : \"ED_POLE\",        \"Atnam\" : \"CIRCUIT_ID\",        \"Atwrt\" : \" \",        \"Clear\" : \"X\"     },     {        \"RecordId\" : \"ED.0.08.20210405.111522.0000002\",        \"BatchId\" : \"ED.0.08.20210405.04.BT.01\",        \"Guid\" : \"{C9A69C8A-CCA4-4154-B8CB-18C951433DEB}\",        \"Equnr\" : \"100013246\",        \"Class\" : \"ED_POLE\",        \"Atnam\" : \"CIRCUIT_ID\",        \"Atwrt\" : \"153132105\",        \"Clear\" : \" \"     }  ]}                ";

                            //Console.WriteLine(json);
                            Common._log.Info("Json for Batch ID ( " + batchId + " ) :: " + json);
                            #endregion

                            //Starting With Pushing data to SAP
                            #region Posting
                            try
                            {

                                #region Post Configration
                                //Prepare AppConfig Key for Interface URL
                                urlstr = "Url" + Interface_Name.ToUpper();

                                //Url for Post Request
                                urlToSend = ConfigurationManager.AppSettings[urlstr].ToString();
                                httpWebRequest = WebRequest.Create(String.Format(urlToSend));

                                Common._log.Info("Post Url for Interface ( " + Interface_Name.ToUpper() + " ) :: " + urlToSend);

                                //Credential for Web Request
                                if (!string.IsNullOrWhiteSpace(ReadConfiguration.Credentials_serviceUserName) && !string.IsNullOrWhiteSpace(ReadConfiguration.Credentials_servicePassword))
                                    httpWebRequest.Credentials = new NetworkCredential(ReadConfiguration.Credentials_serviceUserName, ReadConfiguration.Credentials_servicePassword);
                                //httpWebRequest.Headers[HttpRequestHeader.Authorization] =  ReadConfiguration.PostCredentials_serviceUserName;
                                httpWebRequest.ContentType = ReadConfiguration.PostContentType; //"application/json; charset=utf-8";
                                Common._log.Info("Post ContentType for Interface ( " + Interface_Name.ToUpper() + " ) :: " + ReadConfiguration.PostContentType);

                                httpWebRequest.Method = ReadConfiguration.PostMethod;//"POST";
                                Common._log.Info("Post Method for Interface ( " + Interface_Name.ToUpper() + " ) :: " + ReadConfiguration.PostMethod);

                                //Set response string as empty
                                responseText = string.Empty;
                                #endregion

                                #region Post Operation
                                Common._log.Info("Posting Batch ID " + batchId + " for ( " + Interface_Name.ToUpper() + " )");
                                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                                {
                                    //To test Json string
                                    //streamWriter.Write(jsontest);

                                    streamWriter.Write(json);
                                    streamWriter.Flush();
                                }
                                httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                                {
                                    responseText = streamReader.ReadToEnd();
                                    //Console.WriteLine(responseText);

                                    //Now you have your response.
                                    //or false depending on information in the response     
                                }
                                #endregion

                                if (!PostPassed.Contains(batchId))
                                    PostPassed.Add(batchId);
                            }
                            catch (WebException ex)
                            {
                                if (!PostFailed.Contains(batchId))
                                    PostFailed.Add(batchId);
                                Common._log.Error("Web Exception for Post Batch ID ( " + batchId + " ) for Outbound Job " + Interface_Name, ex);
                                using (WebResponse response = ex.Response)
                                {
                                    //var httpResponse = (HttpWebResponse)response;
                                    using (Stream data = response.GetResponseStream())
                                    {
                                        StreamReader sr = new StreamReader(data);
                                        throw new Exception(sr.ReadToEnd());
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                if (!PostFailed.Contains(batchId))
                                    PostFailed.Add(batchId);
                                Common._log.Error("Unable to Post Batch ID ( " + batchId + " ) for Outbound Job " + Interface_Name, ex);
                                throw ex;
                            }
                            #endregion

                            #region Processing Response
                            try
                            {
                                if (!string.IsNullOrWhiteSpace(responseText))
                                {
                                    //Update Staging Table for Status
                                    Common._log.Info("Processing Response for Batch ID :: " + batchId);
                                    PullDataFromStagArea.ProcessResponse(database_name, responseText, staging_table, ColumnMapping[ReadConfiguration.SAPRecordIdField]);

                                    if (!ResponsePassed.Contains(batchId))
                                        ResponsePassed.Add(batchId);
                                }
                            }
                            catch (Exception ex)
                            {
                                if (!ResponseFailed.Contains(batchId))
                                    ResponseFailed.Add(batchId);

                                Common._log.Error("Unable to Process Response of Batch ID ( " + batchId + " ) for Outbound Job " + Interface_Name, ex);
                                throw ex;
                            }
                            #endregion

                        }
                        catch (Exception ex)
                        {
                            Common._log.Error("Posting Failed for Batch ID ( " + batchId + " ) of Outbound Job " + Interface_Name, ex);
                        }
                    }
                }


                //Add Stats in Log
                Common._log.Info("Post Failed Count :: " + PostFailed.Count + "," + " Response Failed Count :: " + ResponseFailed.Count + "," + "Post Passed Count :: " + PostPassed.Count + "," + " Response Passed Count :: " + ResponsePassed.Count);
                Program.comment = Program.comment + "Post Failed Count :: " + PostFailed.Count + "," + " Response Failed Count :: " + ResponseFailed.Count + "," + "Post Passed Count :: " + PostPassed.Count + "," + " Response Passed Count :: " + ResponsePassed.Count;
                
                //Add Stats in Log
                if (PostFailed.Count > 0)
                    Common._log.Info("Post Failed Batch IDs :: " + string.Join(", ", PostFailed));
                if (ResponseFailed.Count > 0)
                    Common._log.Info("Response Failed Batch IDs :: " + string.Join(", ", ResponseFailed));
                if (PostPassed.Count > 0)
                    Common._log.Info("Post Passed Batch IDs :: " + string.Join(", ", PostPassed));
                if (ResponsePassed.Count > 0)
                    Common._log.Info("Response Passed Batch IDs :: " + string.Join(", ", ResponsePassed));

                if (PostFailed.Count > 0 || ResponseFailed.Count > 0)
                {
                    //Add Mail Code and add mail for incomming?
                    string strPostFailed = string.Join(", ", PostFailed);
                    string strResponseFailed = string.Join(", ", ResponseFailed);
                    string msg = "";
                    string stmsg = "";

                    stmsg = "<table border=" + 1 + " cellpadding=" + 0 + " cellspacing=" + 0 + " width = " + 400 + ">";
                    stmsg += "<tr><td> " + "<b> Web Exception for Outbound Job</b>" + " </td ><td> " + Interface_Name + " </td ></tr>";
                         if (strPostFailed.Length>0)
                    {
                        stmsg += "<tr><td>" + "<b>Post Failed Messages</b>" + "</td><td>" + strPostFailed + "</td></tr>";                       

                    }

                    if (strResponseFailed.Length > 0)
                    {
                        stmsg += "<tr><td>" + "<b>Response Failed Messages</b>" + "</td><td>" + strResponseFailed + "</td></tr>";

                    }

                    stmsg += "</table>";

                    EmailHelper.SendMailToUser(stmsg,Interface_Name);
                    
                 
                    //throw new Exception("Error in Posting Process");
                }

                if(!DBHelper.SetINTEGRATION_LASTRUN_DATE(Interface_Name,startDateTime))
                {
                    throw new Exception("Unable to Update Last Run date");
                }
                
            }
            catch (Exception ex)
            {
                Common._log.Error("Unable to POST for Outbound Job " + Interface_Name, ex);
                throw ex;
            }
        }
    }
}
