using System;
using System.Net;
using System.Data;
using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Reflection;
using System.Net.Http;
using System.Globalization;

using Newtonsoft.Json.Linq;

namespace PGE.Interfaces.Integration.Gateway
{
    public class PullData
    {

        private DBHelper dBHelper = new DBHelper();

        #region public Variables

        public ED07Data dataBlock;
        #endregion 
        /// <summary>
        /// This method is used to create webrequest  
        /// </summary>
        /// <returns>DataTable</returns>
        public void CreateWebRequest(string interfaceName)
        {

            int batchCount = 01;
            string batchID = string.Empty;
            string newBatchId = string.Empty;
            string oldBatchID = string.Empty;
            int counter = 0;
            DateTime last_RunDate = default;
            DateTime today_RunDate = default;
            bool UpdateDate = false;
            // V3SF ED07 Delayed Retry Enhancement (16-June-2021)            
            int ED07Counter = 0;
            int ED07delayTime = ReadConfiguration.ED07RETRYDELAY;

            try
            {
                if (string.IsNullOrWhiteSpace(interfaceName) || (interfaceName == string.Empty))
                {
                    Common._log.Error("Passing Argument is not correct for Inbound Job " + interfaceName);
                    throw new Exception("Passing Argument is not correct for Inbound Job " + interfaceName);
                }

                Common._log.Info(" Get Layer 7 URL for interface " + interfaceName);
                //Get the layer 7 URL 
                string url = GetURL(interfaceName);
                Common._log.Info("Layer 7 URL for interface " + interfaceName + ":" + url);
                // V3SF ED07 Delayed Retry Enhancement (16-June-2021)  
                do
                {
                    // V3SF ED07 Delayed Retry Enhancement (16-June-2021) [START]
                    if (ED07Counter > 0)
                    {
                        Common._log.Info("Next Retry ("+ ED07Counter + ") for ED07 Interface in "+ ED07delayTime +" milliseconds");
                        System.Threading.Thread.Sleep(ED07delayTime); 
                    }

                    ED07Counter++;
                    // V3SF ED07 Delayed Retry Enhancement (16-June-2021)  [END]

                    // V3SF Date Enhancement (04/08/2021)
                    if (!DBHelper.GetINTEGRATION_LASTRUN_DATE(interfaceName, out last_RunDate))
                    {
                        throw new Exception("Last Run date for Interface :: " + interfaceName + " not found");
                    }
                    //url = url.Replace("'DATE'", "'" + DateTime.Now.ToString("yyyyMMdd") + "'");
                    url = url.Replace("'DATE'", "'" + last_RunDate.ToString("yyyyMMddHHmmss") + "'");
                    today_RunDate = DateTime.Now;
                    #region Commented code with the date given by sap for testing

                    // url = url.Replace("'DATE'", "'" + 20210203 + "'"); //ed07 20210405
                    //     url = url.Replace("'DATE'", "'" + 20210405 + "'"); //ED11                //                                                  
                    // url = url.Replace("'DATE'", "'" + 20210421 + "'"); //ED13
                    //  url = url.Replace("'DATE'", "'" + 20210513 + "'"); //ED13A
                    //    url = url.Replace("'DATE'", "'" + 20210420 + "'"); //ED14
                    // url = url.Replace("'DATE'", "'" + 20210420 + "'"); //ED15
                    //  url = url.Replace("'DATE'", "'" + 20210420 + "'"); //ED15summary

                    #endregion

                    // Create a new instance of PushDataInStagArea
                    PushDataInStagArea objStagArea = new PushDataInStagArea();

                    //Call the layer 7 URL in batch
                    for (int i = 0; i < batchCount; i++)
                    {
                        #region Commented Code
                        //BtachID- ED.0.07.20210506.01.BT.01 (16,2)

                        //Create WebRequest

                        //HttpWebRequest webRequest = (HttpWebRequest)HttpWebRequest.Create("https://api-d.cloud.pge.com/Electric/v1/ElectricDistribution/ED07Set?$filter=RequestDate eq '20210203' and BatchId eq ' '&$format=json");
                        //  Common._log.Info("Web Request Created");

                        //      webRequest.Method = "GET";
                        //     webRequest.ContentType = "application/json";
                        //(V3SF) Added Credentials
                        //     webRequest.Credentials = new NetworkCredential(ReadConfiguration.Credentials_serviceUserName, ReadConfiguration.Credentials_servicePassword);
                        //     webRequest.AutomaticDecompression = DecompressionMethods.GZip;

                        // HttpWebRequest webRequest = (HttpWebRequest)HttpWebRequest.Create("https://api-d.cloud.pge.com/Electric/v1/ElectricDistribution/ED07Set?$filter=RequestDate eq '20210506' and BatchId eq ' '&$format=json");

                        //HttpWebRequest webRequest = (HttpWebRequest)HttpWebRequest.Create(url);
                        //Common._log.Info("Web Request Created");

                        //webRequest.Method = ReadConfiguration.requestMethod;
                        //webRequest.ContentType = "application/json";
                        //webRequest.AutomaticDecompression = DecompressionMethods.GZip;
                        ////   httpWebRequest.Credentials = new NetworkCredential(System.Configuration.ConfigurationManager.AppSettings["serviceUserName"].ToString(), System.Configuration.ConfigurationManager.AppSettings["servicePassword"].ToString());

                        //HttpWebResponse response = null;

                        ////Get Response from web Request
                        //using (response = (HttpWebResponse)webRequest.GetResponse())

                        //// Get Response  stream from  Response
                        //using (Stream stream = response.GetResponseStream())
                        //using (StreamReader reader = new StreamReader(stream))
                        //{
                        //    var html = reader.ReadToEnd();
                        //    //Console.WriteLine(html);
                        //    Common._log.Info(html);
                        //    bool isContainError = html.Contains("error");
                        //    Common._log.Info("Error in Reponse:=  " + isContainError);
                        //    if (isContainError)


                        //        Common._log.Info("Web Response received");
                        //    // Deserialize HttpWebResponse                  

                        //    pRoot = JsonConvert.DeserializeObject<Root>(html);
                        #endregion

                        Common._log.Info(" Creating web request " + interfaceName + ":" + url);
                        //Create web request
                        DataTable result = CreateRequest(url);
                        //Testing - [Start]
                        //string filename = @"C:\Github\ED14 Interface\ED14Interface.json";// ED07_UnProcessed.json";
                        //StreamReader r = new StreamReader(filename);
                        //string jsonString = r.ReadToEnd();
                        //DataTable result = PullDataFromStagArea.ReadJsonString(jsonString);
                        //Testing - [END]
                        //To validate if  got the SAP data from layer 7
                        if (!(result != null && result.Rows.Count > 0))
                        {
                            Common._log.Info(" Could not find data from SAP :" + interfaceName + "Batch Count" + batchCount + "BatchID: " + batchID);
                            if (Program.comment != string.Empty)
                                Program.comment = Program.comment + " Could not find data from SAP :" + interfaceName + " Batch Count: " + batchCount + " BatchID: " + batchID;
                            break;
                        }
                        try
                        {
                            //To process SAP data in GIS
                            if (objStagArea.PopulateStagingTable(result, interfaceName))
                            {
                                UpdateDate = true;
                                Common._log.Info(" Successfully staged SAP data in GIS : " + interfaceName + "  Batch Count :" + batchCount + " BatchID : " + batchID);
                            }
                        }

                        catch (Exception ex)
                        {
                            Common._log.Error("Exception occure while processing the SAP data for BatchID :" + batchID + ex.Message);
                            throw ex;
                        }

                        #region Commentes for logic implemented for layer 7 URL filter agreed with SAP team 
                        // In 1st ODATA call, GIS will pass the below parameters:
                        //RequestDate: Date on which data is requested by GIS in YYYYMMDD format(20210203)
                        //BatchId: Blank
                        //MaxNumberOfRecord: Max agreed number of records in each call(500)

                        //In 2nd ODATA call GIS will pass the below parameters:
                        //                RequestDate: Same as above(20210203)
                        //BatchId: Batch id(ED.0.07.20210203.04.BT.01) of any record received in the previous call(1st call)
                        //MaxNumberOfRecord: Max agreed number of records in each call(500)

                        //In 3rd ODATA call GIS will pass the below parameters:
                        //                RequestDate: Same as above(20210203)
                        //BatchId: Batch id(ED.0.07.20210203.04.BT.02)  of any record received in the previous call(2nd call)
                        //MaxNumberOfRecord: Max agreed number of records in each call(500)

                        //In 4th ODATA call GIS will pass the below parameters:
                        //                RequestDate: Same as above(20210203)
                        //BatchId: Batch id(ED.0.07.20210203.04.BT.03)  of any record received in the previous call(3rd call)
                        //MaxNumberOfRecord: Max agreed number of records in each call(500)
                        #endregion

                        // logic implemented for layer 7 URL filter agreed with SAP team
                        //Check to validate batchID 
                        bool batchIDFlag = false;
                        foreach (DataRow dr in result.Rows)
                        {
                            foreach (DataColumn dt in result.Columns)
                            {

                                if (!batchIDFlag)
                                {
                                    if (dt.ColumnName == ReadConfiguration.sapBatchId)
                                    {
                                        newBatchId = dr[dt.ColumnName].ToString();
                                        oldBatchID = batchID;
                                        batchID = newBatchId;
                                        batchIDFlag = true;
                                        break;
                                    }
                                }

                            }
                        }
                        if (counter == 0)
                        {
                            if (batchID != string.Empty)
                            {
                                string id = batchID.Substring(17, 2);
                                batchCount = Convert.ToInt32(id);
                                counter++;
                            }
                        }
                        if (url.Contains("''"))
                            url = url.Replace("''", "'" + batchID + "'");
                        else
                            url = url.Replace(oldBatchID, batchID);

                        Common._log.Info(" old BatchID :" + oldBatchID + " ,New BatchID: " + batchID + "Total Batch Count :" + batchCount);
                    }

                    //Update Last run date only when data was recieved from SAP
                    if (UpdateDate)
                    {
                        if (!DBHelper.SetINTEGRATION_LASTRUN_DATE(interfaceName, today_RunDate))
                        {
                            throw new Exception("Unable to Update Last Run date");
                        }
                    }
                }
                while (ED07Counter < ReadConfiguration.ED07RETRYCOUNTLIMIT && !UpdateDate && interfaceName == "ED07");
                // V3SF ED07 Delayed Retry Enhancement (16-June-2021) 

            }
            catch (Exception ex)
            {
                Common._log.Error("Web Exception for Inbound Job " + interfaceName, ex);
                Common._log.Error(""); 
                
                string strmsg = "<table border=" + 1 + " cellpadding=" + 0 + " cellspacing=" + 0 + " width = " + 400 + "><tr ><td><b>Web Exception for Inbound Job </b></td> <td> " + interfaceName + " </td></tr>";
                strmsg += "<tr ><td><b>Batch ID </b></td> <td> " + batchID + " </td></tr>";
                strmsg += "</table>";

                   EmailHelper.SendMailToUser(strmsg, interfaceName);
                throw ex;
            }

        }

        private void CreateRequest(string url, string interfaceName)
        {
            //Root1 pRoot = null;
            try
            {
                //Create WebRequest
                // HttpWebRequest webRequest = (HttpWebRequest)HttpWebRequest.Create("https://api-d.cloud.pge.com/Electric/v1/ElectricDistribution/ED07Set?$filter=RequestDate eq '20210506' and BatchId eq ' '&$format=json");
                HttpWebRequest webRequest = (HttpWebRequest)HttpWebRequest.Create(url);
                Common._log.Info("Web Request Created");

                webRequest.Method = ReadConfiguration.requestMethod;
                webRequest.ContentType = "application/json";
                webRequest.AutomaticDecompression = DecompressionMethods.GZip;
                // webRequest.Credentials = new NetworkCredential(System.Configuration.ConfigurationManager.AppSettings["serviceUserName"].ToString(), System.Configuration.ConfigurationManager.AppSettings["servicePassword"].ToString());
                //Credential for Web Request
                if (!string.IsNullOrWhiteSpace(ReadConfiguration.Credentials_serviceUserName) && !string.IsNullOrWhiteSpace(ReadConfiguration.Credentials_servicePassword))
                    webRequest.Credentials = new NetworkCredential(ReadConfiguration.Credentials_serviceUserName, ReadConfiguration.Credentials_servicePassword);

                HttpWebResponse response = null;

                //Get Response from web Request
                using (response = (HttpWebResponse)webRequest.GetResponse())

                // Get Response  stream from  Response
                using (Stream stream = response.GetResponseStream())
                using (StreamReader reader = new StreamReader(stream))
                {
                    var html = reader.ReadToEnd();
                    //(V3SF) Added To test Response to DataTable
                    DataTable d = PullDataFromStagArea.ReadJsonString(html);
                    //Console.WriteLine(html);
                    Common._log.Info(html);
                    bool isContainError = html.Contains("error");
                    Common._log.Info("Error in Reponse:=  " + isContainError);
                    if (isContainError)

                        Common._log.Info("Web Response received");
                    // Deserialize HttpWebResponse                  

                    //For ED07
                    //  Root pRoot = JsonConvert.DeserializeObject<Root>(html);
                    if (interfaceName == InterfcaeName.ED11.ToString())
                    {
                        // For ED11
                        RootED11 ed11Root = JsonConvert.DeserializeObject<RootED11>(html);
                        List<ED11Result> ed11Result = ed11Root.D.ed11Results;
                    }
                    if (interfaceName == InterfcaeName.ED14.ToString())
                    {
                        //For ED14
                        RootED14 ed14Root = JsonConvert.DeserializeObject<RootED14>(html);
                        List<ED14Result> eD14Results = ed14Root.D.ed14Results;
                        // }
                        if (interfaceName == InterfcaeName.ED13.ToString())
                        {
                            ////For Ed13
                            RootED13 ed13Root = JsonConvert.DeserializeObject<RootED13>(html);
                            List<ED13Result> eD13Results = ed13Root.D.ed13Results;
                        }
                        if (interfaceName == InterfcaeName.ED13A.ToString())
                        {
                            ////For Ed13
                            RootED13A ed13Root = JsonConvert.DeserializeObject<RootED13A>(html);
                            List<ED13AResult> eD13Results = ed13Root.D.ed13AResults;
                        }

                        ////For Ed15
                        if (interfaceName == InterfcaeName.ED15INDV.ToString())
                        {
                            RootED15INDV rootED15INDV = JsonConvert.DeserializeObject<RootED15INDV>(html);
                            List<ED15INDVResult> eD15Indvs = rootED15INDV.D.ed15INDVResults;
                        }
                        if (interfaceName == InterfcaeName.ED15SUMM.ToString())
                        {
                            ////For Ed15 summary
                            RootED15Summary rootE15Summary = JsonConvert.DeserializeObject<RootED15Summary>(html);
                            List<ED15SummaryResult> eD15SummaryResults = rootE15Summary.D.ed15INDVResults;
                        }
                        ////DataTable dt = new DataTable();
                        //var properties = typeof(ED14Result).GetProperties();
                        //foreach (PropertyInfo info in typeof(ED14Result).GetProperties())
                        //{
                        //    dt.Columns.Add(info.Name);

                        //    //dt.Rows.Add(info.GetValue());
                        //}
                        //object[] array = eD14Results.ToArray();

                        //For ED11
                        //RootED11 ed11Root = JsonConvert.DeserializeObject<RootED11>(html);
                        //List<ED11Result> eD11Results = ed11Root.D.ed11Results;
                        //foreach (ED11Result item in eD14Results)
                        //{
                        //    PushDataInStagArea obj = new PushDataInStagArea();
                        //    obj.PopulateED11StagingTable(item, interfaceName);
                        //}

                        DataTable dt = new DataTable();
                        var properties = typeof(ED14Result).GetProperties();
                        foreach (PropertyInfo info in typeof(ED14Result).GetProperties())


                        //var properties = typeof(ED11Result).GetProperties();
                        //foreach (PropertyInfo info in typeof(ED11Result).GetProperties())
                        //{
                        //var properties = typeof(ED15SummaryResult).GetProperties();
                        //foreach (PropertyInfo info in typeof(ED15SummaryResult).GetProperties())
                        //var properties = typeof(ED15INDVResult).GetProperties();
                        //foreach (PropertyInfo info in typeof(ED15INDVResult).GetProperties())
                        //    var properties = typeof(ED13AResult).GetProperties();
                        //foreach (PropertyInfo info in typeof(ED13AResult).GetProperties())
                        {
                            dt.Columns.Add(info.Name);

                            //dt.Rows.Add(info.GetValue());
                        }
                        object[] array = eD14Results.ToArray();
                        //  object[] array = ed11Result.ToArray();
                        //  object[] array = eD15SummaryResults.ToArray();
                        // object[] array = eD15Indvs.ToArray();
                        // object[] array = eD13Results.ToArray();
                        foreach (object o in array)
                        {

                            DataRow dr = dt.NewRow();

                            foreach (PropertyInfo pi in properties)
                            {
                                dr[pi.Name] = pi.GetValue(o, null);
                            }
                            dt.Rows.Add(dr);

                        }
                        dt.Columns.Remove("__metadata");

                    }



                    // foreach (var entity in self)




                    Common._log.Info(" Web Response received:" + html);
                }
            }
            catch (Exception ex)
            {
                //V3SF
                Common._log.Error("Error :: (" + ex.Message + ") at " + ex.StackTrace);
            }
            //  return pRoot;

        }


        /// <summary>
        /// Method to create webrequest and get SAP data via layer 7- v1t8
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        private DataTable CreateRequest(string url)
        {
            //variable to return
            DataTable result = null;
            try
            {
                // HttpWebRequest webRequest = (HttpWebRequest)HttpWebRequest.Create("https://api-d.cloud.pge.com/Electric/v1/ElectricDistribution/ED07Set?$filter=RequestDate eq '20210506' and BatchId eq ' '&$format=json");
                //Create WebRequest
                HttpWebRequest webRequest = (HttpWebRequest)HttpWebRequest.Create(url);
                Common._log.Info("Web Request Created URL:" + url);

                //Set request method
                webRequest.Method = ReadConfiguration.requestMethod;
                Common._log.Info("Web Request method:" + ReadConfiguration.requestMethod);
                webRequest.ContentType = "application/json";
                webRequest.AutomaticDecompression = DecompressionMethods.GZip;

                //Layer 7 authentication               
                if (!string.IsNullOrWhiteSpace(ReadConfiguration.Credentials_serviceUserName) && !string.IsNullOrWhiteSpace(ReadConfiguration.Credentials_servicePassword))
                    webRequest.Credentials = new NetworkCredential(ReadConfiguration.Credentials_serviceUserName, ReadConfiguration.Credentials_servicePassword);
                //  webRequest.Credentials = new NetworkCredential(System.Configuration.ConfigurationManager.AppSettings["serviceUserName"].ToString(), System.Configuration.ConfigurationManager.AppSettings["servicePassword"].ToString());
                //Common._log.Info("layer 7 Credentials:" + "USERNAME :-" + ReadConfiguration.Credentials_serviceUserName + "PASSWORD :-" + ReadConfiguration.Credentials_servicePassword);
                HttpWebResponse response = null;

                //Get Response from web Request
                using (response = (HttpWebResponse)webRequest.GetResponse())

                // Get Response  stream from  Response
                using (Stream stream = response.GetResponseStream())
                using (StreamReader reader = new StreamReader(stream))
                {
                    var html = reader.ReadToEnd();
                    Common._log.Info("Web response received" + html);

                    bool isContainError = html.Contains("error");
                    Common._log.Info("Error in Reponse:=  " + isContainError);
                    if (isContainError)

                        Common._log.Info("Web Response received");

                    Common._log.Info("Converting Web response  to datatable for further use");
                    //Converting web response to datatable for further use
                    result = PullDataFromStagArea.ReadJsonString(html);
                    Common._log.Info("Web response sucessfully converted  into Datatable");

                }



            }
            catch (Exception ex)
            {
                //V3SF Exception Handling
                Common._log.Error("Error :: (" + ex.Message + ") at " + ex.StackTrace);
                throw ex;
            }
            return result;

        }
        private string GetURL(string interfaceName)
        {
            string url = string.Empty;

            string RequestFilter = " eq 'DATE' and BatchId eq '' &$format=json";
            // string RequestFilter = "";


            if (interfaceName.ToUpper() == InterfcaeName.ED07.ToString())
            {
                string urlED07 = ReadConfiguration.urlED07;
                url = urlED07 + RequestFilter;
            }
            if (interfaceName.ToUpper() == InterfcaeName.ED11.ToString())
            {
                string urlED11 = ReadConfiguration.urlED11;
                url = urlED11 + RequestFilter;
            }
            if (interfaceName.ToUpper() == InterfcaeName.ED13.ToString())
            {
                string urlED13 = ReadConfiguration.urlED13;
                url = urlED13 + RequestFilter;
            }
            if (interfaceName.ToUpper() == InterfcaeName.ED13A.ToString())
            {
                string urlED13 = ReadConfiguration.urlED13A;
                url = urlED13 + RequestFilter;
            }
            if (interfaceName.ToUpper() == InterfcaeName.ED15INDV.ToString())
            {
                string urlED15 = ReadConfiguration.urlED15Indv; ;
                url = urlED15 + RequestFilter;
            }
            if (interfaceName.ToUpper() == InterfcaeName.ED15SUMM.ToString())
            {
                string urlED15 = ReadConfiguration.urlED15Summary;
                url = urlED15 + RequestFilter;
            }
            if (interfaceName.ToUpper() == InterfcaeName.ED14.ToString())
            {
                string urlED14 = ReadConfiguration.urlED14;
                url = urlED14 + RequestFilter;
            }

            return url;
        }


        public void jsonToStaggingTable(Root root)
        {
            PushDataInStagArea objStagArea = new PushDataInStagArea();
            List<Result> result = root.D.Results;
            List<Result> errorInResponse = new List<Result>();
            int count = root.D.Results.Count;
            if (count < 1) return;

            try
            {
                foreach (Result row in result)
                {

                    #region required validation 
                    ////Check to handle null and 0 value for BatchId, SAP_EQUIPMENT_ID,EQUIPMENT_NAME,SAP_EQUIPMENT_TYPE,  and Longitude fields
                    if (string.IsNullOrEmpty(row.BatchId))
                    {
                        Common._log.Info("Web Request Created");

                        Common._log.Info("Record BatchId is null or blank or empty for  " + row.RecordId);
                        Common._log.Info("Skipping this record");
                        errorInResponse.Add(row);
                        continue;
                    }
                    //if (Convert.ToInt32(row.RecordId) == 0 || string.IsNullOrEmpty(row.RecordId))
                    if (string.IsNullOrEmpty(row.RecordId))
                    {
                        Common._log.Info("Web Request Created");

                        Common._log.Info("Record RecordId is null or blank or empty for  " + row.RelatedRecordID);
                        Common._log.Info("Skipping this record");
                        errorInResponse.Add(row);
                        continue;
                    }
                    if (Convert.ToUInt64(row.SapEquipId) == 0 || string.IsNullOrEmpty(row.SapEquipId))
                        if (string.IsNullOrEmpty(row.SapEquipId))
                        {
                            Common._log.Info("Web Request Created");

                            Common._log.Info("Record SapEquipId is null or blank or empty for  " + row.RecordId);
                            Common._log.Info("Skipping this record");
                            errorInResponse.Add(row);
                            continue;
                        }
                    if (string.IsNullOrEmpty(row.EquipName))
                    {
                        Common._log.Info("Web Request Created");

                        Common._log.Info("Record EquipName is null or blank or empty for  " + row.RecordId);
                        Common._log.Info("Skipping this record");
                        errorInResponse.Add(row);
                        continue;
                    }
                    if (string.IsNullOrEmpty(row.EquipType))
                    {
                        Common._log.Info("Web Request Created");

                        Common._log.Info("Record EquipType is null or blank or empty for  " + row.RecordId);
                        Common._log.Info("Skipping this record");
                        errorInResponse.Add(row);
                        continue;
                    }
                    if (string.IsNullOrEmpty(row.GisGuid))
                    {

                        Common._log.Info("Record GUID is null or blank or empty for  " + row.RecordId);
                        Common._log.Info("Skipping this record");
                        errorInResponse.Add(row);
                        continue;
                    }

                    #endregion
                    DBHelper.GetOracleConnection();
                    //     objStagArea.PopulateED07StagingTable(row);

                }
            }
            catch (Exception ex)
            {
                //V3SF
                Common._log.Error("Error :: (" + ex.Message + ") at " + ex.StackTrace);
            }
        }





    }
}
