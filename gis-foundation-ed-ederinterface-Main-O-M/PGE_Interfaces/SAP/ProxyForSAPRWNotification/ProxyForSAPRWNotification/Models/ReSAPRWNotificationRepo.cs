using System;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.IO;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Tracing;
using PGE.Interfaces.ProxyForSAPRWNotification.Services;
using PGE.Interfaces.ProxyForSAPRWNotification.ProxyForEI;
using Newtonsoft.Json;

namespace PGE.Interfaces.ProxyForSAPRWNotification.Models
{
    
    public class ReSaprwNotificationRepo : IReSaprwNotification
    {
        private static readonly ITraceWriter Tracer = GlobalConfiguration.Configuration.Services.GetTraceWriter();
        public readonly DataHelper ObjDataHelper = new DataHelper();
        public Common ObjCommon = new Common();

        public bool Reprocess(string notificationid)
        {
            //var notification = notificationid;
            //if (Tracer != null)
            //    Tracer.Info(request, "Reprocess", notification);

            HttpRequestMessage request = new HttpRequestMessage();

            Tracer.Info(request, "Reprocessing '" + notificationid +"'.", "");
            //Get the Header Detail
            Tracer.Info(request, "Reprocess", "Get Notification Header Detail");
            DataTable dtnotificationdetail = ObjDataHelper.GetDataForSapRwnotification(notificationid);

            //Get the Map location
            Tracer.Info(request, "Reprocess", "Get Notification Location Details");
            DataTable dtnotificationlocation = ObjDataHelper.GetDataForSapRwnotificationLocation(notificationid);
            
            DataSet ds = new DataSet("ReProcessNortification");
            if (dtnotificationdetail != null)
            {
                ds.Tables.Add(dtnotificationdetail);
                if (dtnotificationdetail.Rows.Count < 1) return false;
                var lanId = dtnotificationdetail.Rows[0]["LANID"].ToString();
                var submitDate = dtnotificationdetail.Rows[0]["SUBMITDATE"].ToString();
                if (dtnotificationlocation == null) return false;
                if (dtnotificationlocation.Rows.Count < 1) return false;
                ds.Tables.Add(dtnotificationlocation);
                Tracer.Info(request, "Reprocess", "Calling EI Service");
                //Send Recirds to EI
                SendNotificationDetailtoEi(notificationid, lanId,submitDate, ds, request);
                Tracer.Info(request, "Reprocess", "End processing ReSapRWNotificationController");
                return true;
            }

            return false;
        }

        private void SendNotificationDetailtoEi(string notificationid, string lanId,string submitDate, DataSet item, HttpRequestMessage request)
        {
            if (request == null) throw new ArgumentNullException("request");

            string dataCenter = item.Tables[0].Rows[0]["DATACENTER"].ToString();
            try
            {
                //Map Object
                MapCorrection_PortClient objPortClient = new MapCorrection_PortClient();

                //Parameters Object 
                MapCorrectionRequestMessageType msg = new MapCorrectionRequestMessageType();

                // Header Object
                Tracer.Info(request, "SendNotificationDetailtoEI", "Create Map Data header");
                tMapDataHeader objMapDataHeader = CreateMapDataHeader(item.Tables[0]);
               // string jsonMapdataHeader = JsonConvert.SerializeObject(objMapDataHeader);
               // Tracer.Info(request, "SendNotificationDetailtoEI " + jsonMapdataHeader, "");

                //Detail Object
                Tracer.Info(request, "SendNotificationDetailtoEI", "Create Map Data Detail");
                tMapDataDetail[] objMapDataDetail = CreateMapDataDetail(item.Tables[1],dataCenter);
             //   string jsonMapDataDetail = JsonConvert.SerializeObject(objMapDataDetail);
              //  Tracer.Info(request, "SendNotificationDetailtoEI " + jsonMapDataDetail, "");

                tMapDataReq objMapDataReq = new tMapDataReq
                {
                    MapCorrectionDetail = objMapDataDetail,
                    MapCorrectionHeader = objMapDataHeader
                };

                HeaderType objHeaderType = new HeaderType
                {
                    Verb = HeaderTypeVerb.create,
                    Noun = "MapCorrection",
                    Revision = "1.0",
                    Source = "EDGIS",
                    MessageID = notificationid
                };

                MapCorrectionPayloadType objMapCorrPayLoad = new MapCorrectionPayloadType();
                MapCorrectionType objMapCorrType = new MapCorrectionType { createMapCorrectionRequest = objMapDataReq };
                objMapCorrPayLoad.MapCorrection = objMapCorrType;

                msg.Header = objHeaderType;
                msg.Payload = objMapCorrPayLoad;

                

                Tracer.Info(request, "SendNotificationDetailtoEI", "Send notification to EI");
                MapCorrectionResponseMessageType obj = objPortClient.CreateMapCorrection(msg);

                if (obj.Reply.Result == ReplyTypeResult.FAILED)
                {
                    Tracer.Error(request, "SendNotificationDetailtoEI", "Send notification to EI has failed");
                    string updateStatement = ConfigurationManager.AppSettings["Update_NotificationDetail_Fail"];
                    try
                    {
                        if (updateStatement != "")
                        {
                            updateStatement = updateStatement + notificationid + "'";
                            ObjDataHelper.UpdateData(updateStatement);
                        }
                    }

                    catch (Exception)
                    {
                        // ignored
                    }
                    HttpRequestMessage h = new HttpRequestMessage();

                    ObjCommon.SendEmailtoUser(h, notificationid, lanId, submitDate, "Sending Notification to EI has failed");
                }
                else
                {
                    Tracer.Info(request, "SendNotificationDetailtoEI", "Send notification to EI has Passed");
                    string updateStatement = ConfigurationManager.AppSettings["Update_NotificationDetail_Pass"];
                    try
                    {
                        if (updateStatement != "")
                        {
                            updateStatement = updateStatement + notificationid + "'";
                            ObjDataHelper.UpdateData(updateStatement);
                        }
                    }

                    catch (Exception)
                    {
                        // ignored
                    }
                }
            }

            catch (Exception ex)
            {
                Tracer.Error(request, "SendNotificationtoEI", ex.Message);
                string updateStatement = ConfigurationManager.AppSettings["Update_NotificationDetail_Fail"];
                try
                {
                    if (updateStatement != "")
                    {
                        updateStatement = updateStatement + notificationid + "'";
                        ObjDataHelper.UpdateData(updateStatement);
                    }
                }

                catch (Exception)
                {
                    // ignored
                }
                HttpRequestMessage h = new HttpRequestMessage();
                Tracer.Info(request, "SendNotificationtoEI", "Sending Email to user");
                ObjCommon.SendEmailtoUser(h, notificationid, lanId, submitDate, ex.Message);
            }
        }

        private tMapDataHeader CreateMapDataHeader(DataTable dtnotificationdetail)
        {
            tMapDataHeader objMapDataHeader = new tMapDataHeader
            {
                Division = dtnotificationdetail.Rows[0]["DIVISION"].ToString(),
                FunctionalLocation = dtnotificationdetail.Rows[0]["FUNCTIONAL_LOCATION"].ToString(),
                MainWorkCenter = dtnotificationdetail.Rows[0]["MAINWORKCENTER"].ToString(),
                NumberOfCorrection = dtnotificationdetail.Rows[0]["NUMBEROFCORRECTION"].ToString(),
                Department = dtnotificationdetail.Rows[0]["DEPARTMENT"].ToString(),
                Feeder = dtnotificationdetail.Rows[0]["CIRCUITID"].ToString(),
                MapNumber = dtnotificationdetail.Rows[0]["MAPNUMBER"].ToString(),
                Substation = dtnotificationdetail.Rows[0]["SUBSTATION"].ToString(),
                TransactionDateTime = Convert.ToDateTime(dtnotificationdetail.Rows[0]["SUBMITDATE"]).ToString("MM/dd/yyyy"),
                UniqueNotificationID = dtnotificationdetail.Rows[0]["NOTIFICATION_ID"].ToString(),
                UserID = dtnotificationdetail.Rows[0]["LANID"].ToString()
            };



            // If Construction type is OverHead
            if (dtnotificationdetail.Rows[0]["CONSTRUCTION_TYPE"].ToString() == "OH")
                objMapDataHeader.ServiceDeliveryMethod = tServiceDeliveryMethod.OverHead;
            // If Construction type is UnderGround
            else if (dtnotificationdetail.Rows[0]["CONSTRUCTION_TYPE"].ToString() == "UG")
                objMapDataHeader.ServiceDeliveryMethod = tServiceDeliveryMethod.UnderGround;

            // If correction type is electric distribution
            if (dtnotificationdetail.Rows[0]["CORRECTION_TYPE"].ToString() == "ED")
                objMapDataHeader.CorrectionCategory = tCorrectionCategory.ED;
            // If correction type is landbase
            else if (dtnotificationdetail.Rows[0]["CORRECTION_TYPE"].ToString() == "LAND")
                objMapDataHeader.CorrectionCategory = tCorrectionCategory.LAND;

            return objMapDataHeader;
        }

        private DataTable GetAllFileNameforMapLocation(DataRow row)
        {
            
            var maplocationNum = row["MAP_LOCATION_NUM"].ToString();
            var notificationid = row["NOTIFICATION_ID"].ToString();
            DataTable dtAttachment = ObjDataHelper.GetDataForSapRwnotificationAttachment(notificationid, maplocationNum);
            if (dtAttachment == null) throw new ArgumentNullException("dtAttachment");
            return dtAttachment;
        }

        private tMapDataDetail[] CreateMapDataDetail(DataTable dtnotificationlocation, string dataCenter)
        {
            var sectionCount = dtnotificationlocation.Rows.Count;
            //Section object

            tMapDataDetail[] objMapDataDetail = new tMapDataDetail[sectionCount];
            var destFileNamepath = "";


            try
            {

                //tMapDataDetail[] objMapDataDetail = new tMapDataDetail()[sectionCount];
                for (var i = 0; i < sectionCount; i++)
                {
                    switch (dataCenter)
                    {
                        case "DC1":
                            destFileNamepath = ConfigurationManager.AppSettings["DC1DestFileNamePath"];
                            destFileNamepath = destFileNamepath + "\\" + dtnotificationlocation.Rows[i]["NOTIFICATION_ID"];
                            break;
                        case "DC2":
                            destFileNamepath = ConfigurationManager.AppSettings["DC2DestFileNamePath"];
                            destFileNamepath = destFileNamepath + "\\" + dtnotificationlocation.Rows[i]["NOTIFICATION_ID"];
                            break;
                    }
                    //Attachment Object

                   // var fileNames = GetAllFileNameforMapLocation(destFileNamepath, dtnotificationlocation.Rows[i],i);
                   // if (fileNames == null) throw new ArgumentNullException("dtnotificationlocation");
                   // //count of filename
                   // int filenamecount = fileNames.Length;
                   //// var objAttach = i == 0 ? new tAttachment[filenamecount + 2] : new tAttachment[filenamecount + 1];

                   // // int filenamecount = -1;
                   // tAttachment[] objAttach = new tAttachment[filenamecount];


                   // for (var j = 0; j < filenamecount; j++)
                   // {
                   //     objAttach[j] = new tAttachment();
                   //     var filename = fileNames[i];
                   //     //filename = destFileNamepath + item.Notificationdetails.Notification_id + "\\" + item.NotificationLocations[i].Map_Location_Num.ToString() + "_" + FilePathnamewithserver.Substring(FilePathnamewithserver.LastIndexOf("\\") + 1);
                   //     objAttach[j].Name = filename.Substring(filename.LastIndexOf("\\", StringComparison.Ordinal) + 1);
                   //     objAttach[j].Data = File.ReadAllBytes(filename);


                   // }

                    DataTable dtAttachment = GetAllFileNameforMapLocation(dtnotificationlocation.Rows[i]);
                    //count of filename
                    int filenamecount = dtAttachment.Rows.Count;
                    var objAttach = i == 0 ? new tAttachment[filenamecount + 2] : new tAttachment[filenamecount + 1];

                    // int filenamecount = -1;
                    // objAttach = new tAttachment[1];


                    string filename;
                    for (int j = 0; j < filenamecount; j++)
                    {
                        objAttach[j] = new tAttachment();
                        filename = destFileNamepath + "\\" + dtAttachment.Rows[j]["FILENAMES"].ToString();
                        //filename = destFileNamepath + item.Notificationdetails.Notification_id + "\\" + item.NotificationLocations[i].Map_Location_Num.ToString() + "_" + FilePathnamewithserver.Substring(FilePathnamewithserver.LastIndexOf("\\") + 1);
                        if (File.Exists(filename))
                        {
                            objAttach[j].Name =
                                filename.Substring(filename.LastIndexOf("\\", StringComparison.Ordinal) + 1);
                            objAttach[j].Data = File.ReadAllBytes(filename);
                        }


                    }

                    objAttach[filenamecount] = new tAttachment();
                    filename = destFileNamepath + "\\" + dtnotificationlocation.Rows[i]["MAP_LOCATION_NUM"] + "_Map.pdf";
                    if (File.Exists(filename))
                    {
                        if (filename.Length > filename.LastIndexOf("\\", StringComparison.Ordinal) + 1)
                            objAttach[filenamecount].Name =
                                filename.Substring(filename.LastIndexOf("\\", StringComparison.Ordinal) + 1);
                        objAttach[filenamecount].Data = File.ReadAllBytes(filename);
                    }


                    if (i == 0)
                    {

                        objAttach[filenamecount + 1] = new tAttachment();
                        filename = destFileNamepath + "\\" + "Header.PDF";
                        if (File.Exists(filename))
                        {
                            if (filename.Length > filename.LastIndexOf("\\", StringComparison.Ordinal) + 1)
                                objAttach[filenamecount + 1].Name =
                                    filename.Substring(filename.LastIndexOf("\\", StringComparison.Ordinal) + 1);
                            objAttach[filenamecount + 1].Data = File.ReadAllBytes(filename);
                        }

                    }

                    objMapDataDetail[i] = new tMapDataDetail
                    {
                        Comments = dtnotificationlocation.Rows[i]["COMMENTS"].ToString(),
                        CorrectionType = dtnotificationlocation.Rows[i]["MAP_CORRECTION_TYPE"].ToString(),
                        Latitude = dtnotificationlocation.Rows[i]["LATITUDE"].ToString(),
                        LocationDescription = dtnotificationlocation.Rows[i]["MAP_LOCATION"].ToString(),
                        LocationID = dtnotificationlocation.Rows[i]["MAP_LOCATION_NUM"].ToString(),
                        Longitude = dtnotificationlocation.Rows[i]["LONGITUDE"].ToString(),
                        Attachment = objAttach
                    };
                    //objMapDataDetail[i].CorrectionCategory = tCorrectionCategory.ED;
                    //objMapDataDetail[i].FunctionalLocation = item.NotificationLocations[i].Map_Location_Num.ToString();
                    //objMapDataDetail[i].UpdatedAttachmentList = filenames;
                }
            }

            catch (Exception ex)
            {
                // ignored
            }
            return objMapDataDetail;
        }
    }
}