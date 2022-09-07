using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Configuration;
using System.Runtime.InteropServices;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.DataSourcesGDB;
using ESRI.ArcGIS.Geometry;
using System.Net;
using System.IO;
using Newtonsoft.Json;
using log4net;
using System.Reflection;
using PGE_DBPasswordManagement;

namespace PGE.BatchApplication.GISViewUpdatePLC
{
    class Program
    {
        #region Private Variable
        /// <summary>
        /// Logger to log error / debug/ user information
        /// </summary>
        private static readonly Log4NetLogger _logger = new Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "PLDInfo.log4net.config");
       
        #endregion Private Variables

        Dictionary<string, string> dicPLDBID;
        PLDINFO _pldinfo;
        static void Main(string[] args)
        {
            try
            {
               
                string startDate;
                string enddate;
                if (args.Length > 0)
                {
                    startDate = args[0];
                    enddate = args[1];
                   _logger.Info(" startDate:= " + startDate + " " + " EndDate:= " + enddate);
                }
                else
                {
                    // Format Date in required Format yyyy-MM-ddTHH:mm:ss
                    enddate = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss");
                    DateTime d1 = DateTime.Now;
                    d1 = d1.AddDays(-1);
                    startDate = d1.ToString("yyyy-MM-ddTHH:mm:ss");

                }
                   

                Program P1 = new Program();
                List<IRowData2> lstobj = null;
                IRowData2 obj;

                List<PGEDataBlock> result;

               

                //Create WebRequest
                HttpWebRequest webRequest = (HttpWebRequest)HttpWebRequest.Create(String.Format(System.Configuration.ConfigurationManager.AppSettings["ViewUpdateURL"], startDate, enddate));
                _logger.Info("Web Request Created");
                _logger.Error("Test");
                //webRequest.Proxy = GlobalProxySelection.GetEmptyWebProxy();
                webRequest.Method = System.Configuration.ConfigurationManager.AppSettings["RequestMethod"];
                webRequest.ContentType = "application/json";
                // webRequest.UserAgent = "Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 5.1)";
  

                // Add Headers to WebRequest
                webRequest.Headers[HttpRequestHeader.Authorization] = System.Configuration.ConfigurationManager.AppSettings["AuthorizationKey"];
                webRequest.Headers["TimeStamp"] = System.Configuration.ConfigurationManager.AppSettings["HeaderTimeStamp"];
                webRequest.Headers["Source"] = System.Configuration.ConfigurationManager.AppSettings["HeaderSource"];
                webRequest.Headers["TrackingID"] = Guid.NewGuid().ToString();
                _logger.Info("Web Request Headers Added");
    
                webRequest.AutomaticDecompression = DecompressionMethods.GZip;
                // ="";
                _logger.Info("Web Request Submitted.");

                //Get HttpWebReponse 
                using (HttpWebResponse response = (HttpWebResponse)webRequest.GetResponse())
                using (Stream stream = response.GetResponseStream())
                using (StreamReader reader = new StreamReader(stream))
                {
                    var html = reader.ReadToEnd();
                    Console.WriteLine(html);
                    bool isContainError=html.Contains("error");
                    Console.WriteLine("Error in Reponse:=  "+isContainError);
                    if(isContainError)
                    return;
                    _logger.Info("Web Response received");
                    // Deserialize HttpWebResponse
                    result = JsonConvert.DeserializeObject<List<PGEDataBlock>>(html);
                    _logger.Info(" Web Response received:" + html);
                    
                }
                if (result == null)
                {
                    _logger.Info("No Records returned to be Processed from Service for time stamp" + startDate + " " + enddate);
                    return;
                }
                    
                _logger.Info("Records Count: "+ result.Count);
                Console.WriteLine("Result Count" + result.Count);

                lstobj = new List<IRowData2>();
                for (int count = 0; count < result.Count; count++)
                {
                    obj = new RowData2();
                    obj.FieldValues = result[count].PGE_DataBlock;
                    obj.FieldValues["PLDBID"] = result[count].PLDBID;
                    obj.FieldValues["PLD_STATUS"] = result[count].Status;
                    //obj.FieldValues["STATUSCODE"] = result[count].StatusCode;
                   
                    lstobj.Add(obj);

                }
                //obj.FieldValues();
                if (lstobj.Count > 0)
                {
                    P1.OutPutData(lstobj);
                }
                else
                {
                    _logger.Info("No Records returned to be Processed from Service.");
                    Console.WriteLine("No Records returned to be Processed from Service for time stamp" + startDate + " " + enddate);
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Error Main : ",ex);
            }
        }
        
        public void OutPutData(List<IRowData2> pldInfoData)
        {
            try
            {
                if (pldInfoData == null) return;

                //Checkout License
                if (LicenseManager.ChecketOutLicenses() == false)
                {
                    _logger.Info("ArcGIS License check-out failed. Hence exits the process.");
                    return;
                }

                _logger.Info("ArcGIS Licenses checked out.");
                //Open the Database using the credentials provided in configuration settings
                IWorkspace wSpace = GetSDEWorkSpace();
                if (wSpace == null)
                {
                    _logger.Debug("Could not connect to the SDE Database using the credentials provided in config file.");
                    return;
                }
                //Get the PLDInfo table names from config file
                string pldinfoTableName = Config.ReadConfigFromExeLocation().AppSettings.Settings["PLDInfo"].Value;
                _logger.Debug("'PLDInfo' config parameter read.");


                //Create an instance to process PLDInfo
                _pldinfo = new PLDINFO(wSpace, pldinfoTableName);
                _pldinfo.PLDBFieldName = System.Configuration.ConfigurationManager.AppSettings["PLDBFieldName"];


                //Create Dictonary for checking PLDBID in Feature class
    
                //Load the cache
                Dictionary<string, string> dicPLDBID = _pldinfo.LoadCache();

           

                //Start editing the workspace
                IWorkspaceEdit wSpaceEdit = wSpace as IWorkspaceEdit;
                wSpaceEdit.StartEditing(false);

                // _logger.Info(string.Format("Started inserting/updating PMOrders (Count={0})...", pldInfo.Count));

                IRowData2 pldInfo;
                Dictionary<string, string> fieldValues;
                string pldbID;
                bool recordExists;
                // bool wipPolygonFound;
                for (int i = 0; i < pldInfoData.Count; i++)
                {
                   
                   
                   
                    pldInfo = pldInfoData[i];
                    fieldValues = pldInfo.FieldValues;

                    ///This check is already taken care
                    if (fieldValues == null || !fieldValues.ContainsKey(ResourceConstants.XMLNodeNames.PLDBID))
                    {
                        _logger.Debug(ResourceConstants.FieldNames.PLDBID + " node is missing the XML file. Hence could not process this PLDINFO");
                        continue;
                    }

                 
                    //Get the PLDBID of the PLDINFO
                    pldbID = fieldValues["PLDBID"].ToString();

                    //Check to handle null and 0 value for PLDBID, Latitide and Longitude fields
                    if (Convert.ToUInt64(fieldValues["PLDBID"]) == 0 || fieldValues["PLDBID"] == null)
                    {
                        _logger.Info("Record PLDBID is null or blank or empty:= " + pldbID + " Record Count:= " + i);
                        continue;
                    }
                    if (Convert.ToDouble(fieldValues["Latitude"]) == 0.0 || fieldValues["Latitude"] == null)
                    {
                        _logger.Info("Record where latitude is null or blank or empty for PLDBID:= " + pldbID + " Record Count:= " + i);
                        continue;
                    }


                    if (Convert.ToDouble(fieldValues["Longitude"]) == 0.0 || fieldValues["Longitude"] == null)
                    {
                        _logger.Info("Record where Longitude is null or blank or empty for PLDBID:= " + pldbID + " Record Count:= " + i);
                        continue;
                    }



                    //Check whether a record already exists with this pldb id
                    //recordExists = _pldinfo.RecordExists(pldbID);

                    recordExists = dicPLDBID.ContainsValue(pldbID);
                    _logger.Info("Record to be Processed for PLDBID:= " + pldbID + " Record Count:= " + i);
                    //Re-attach the field value collection to the PLDINFO
                    pldInfo.FieldValues = fieldValues;

                    //Insert PLDInfo or update the existing the one
                    if (recordExists)
                    {
                        _logger.Info("Update Record for PLDBID" + pldbID);
                        _pldinfo.UpdateRow(pldInfo);
                    }
                    else
                    {
                        _logger.Info("Inser Record for PLDBID" + pldbID);
                       
                        _pldinfo.InsertRow(pldInfo);
                        dicPLDBID.Add("PLDBID" + dicPLDBID.Count + 1, pldbID);
                    }
                }

                _logger.Info("Completed inserting/updating PLDInfo...");



                //Save and Stop editing
                wSpaceEdit.StopEditing(true);

                _logger.Info("Saved the edits to the database.");

                //Release the workspace reference
                Marshal.ReleaseComObject(wSpace);
            }
            catch (Exception ex)
            {
                _logger.Error("Error occured" + ex.Message);
            }
            finally
            {
             
  
                _logger.Info("ArcGIS & ArcFM Licenses checked-in if any checked-out already.");
                //Release the resources
                if (_pldinfo != null)
                {
                    _pldinfo.Dispose(); _pldinfo = null;
                }
    
            }
        }

        private IWorkspace GetSDEWorkSpace()
        {
            try
            {
                KeyValueConfigurationCollection appSettings = Config.ReadConfigFromExeLocation().AppSettings.Settings;
                IPropertySet propertySet = new PropertySetClass();
                // m4jf edgisrearch 919 - get password using Password Mangement tool
                string[] UserInst = appSettings["WIP_SDEConnection"].Value.Split('@');
                //propertySet.SetProperty("instance", appSettings["DBConnec"].Value);
                //propertySet.SetProperty("User", appSettings["UserName"].Value);
                //propertySet.SetProperty("Password", appSettings["Password"].Value);
                string password = ReadEncryption.GetPassword(appSettings["WIP_SDEConnection"].Value.ToUpper());
                propertySet.SetProperty("instance", "sde:oracle11g:" + UserInst[1]);
                propertySet.SetProperty("User", UserInst[0]);
                propertySet.SetProperty("Password", password);

                propertySet.SetProperty("version", appSettings["PLDCloudVersion"].Value);

                IWorkspaceFactory workspaceFactory = new SdeWorkspaceFactory();
                IWorkspace wspace = workspaceFactory.Open(propertySet, 0);
                System.Runtime.InteropServices.Marshal.ReleaseComObject(workspaceFactory);
                return wspace;
            }
            catch (Exception ex)
            {
                _logger.Error("Error getting workspace.", ex);
                return null;
            }

        }
    }
}
