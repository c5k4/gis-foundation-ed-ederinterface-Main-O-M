using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Web.Helpers;
using System.Web.Http;
using System.Web.Http.Tracing;
using iTextSharp.text;
using iTextSharp.text.pdf;
using PGE.Interfaces.ProxyForSAPRWNotification.Services;
using PGE.Interfaces.ProxyForSAPRWNotification.ProxyForEI;
using Newtonsoft.Json;
using System.Web;


namespace PGE.Interfaces.ProxyForSAPRWNotification.Models
{
    internal class SaprwNotificationRepo : ISaprwNotificationDetail
    {
        private static readonly ITraceWriter Tracer = GlobalConfiguration.Configuration.Services.GetTraceWriter();
        public readonly DataHelper ObjDataHelper = new DataHelper();
        public static string DataCenter = "";
        public static string FunctionalLocation = "";
        public static string Mainworkcenter = "";
        public static string SapDivision = "";


        public Mapping ObjMapper = new Mapping();
        public static Common ObjCommon = new Common();

        public bool ProcessSaprwNotification(SaprwNotification item)
        {
            var processed = false;
            HttpRequestMessage request = new HttpRequestMessage();
            request.Method = HttpMethod.Post;

            try
            {
                if (item == null)
                    throw new ArgumentNullException();
                Tracer.Info(request, "ProcessSAPRWNotification Start Processing SAPRWNotification for '" + item.Notificationdetails.NotificationId + "'.", "");

                DataCenter = item.Notificationdetails.DataCenter;

                // Copy the Print map
                if (CopyAttachments(item, request))
                {
                    // Get the FLOC that need to send to SAP
                    FunctionalLocation = GetFunctionalLocation(item.Notificationdetails);

                    // Get the MainWorkCenter that need to send to SAP
                    Mainworkcenter = GetMainWorkCenter(item.Notificationdetails);

                    // Get the SAPDivision that need to send to SAP
                    SapDivision = GetSapDivision(item.Notificationdetails);

                    //Create the PDF for header section
                    Tracer.Info(request, "ProcessSAPRWNotification", "Create PDF file for Header");
                    CreatePdf(item);
                    Tracer.Info(request, "ProcessSAPRWNotification", "PDF file for Header has created");
                    processed = true;

                    // Insert Header, map location and Attachment records in Tables
                    Tracer.Info(request, "ProcessSAPRWNotification", "Inserting records for Notification");
                    InsertSaprwNotification(item, request);
                    Tracer.Info(request, "ProcessSAPRWNotification", "Inserting records for Notification has completed");

                    // Send the notification to EI
                    Tracer.Info(request, "ProcessSAPRWNotification", "Send records to EI");
                    SendNotificationDetailtoEi(item, request);
                    Tracer.Info(request, "ProcessSAPRWNotification", "Records has sent to EI");
                    Tracer.Info(request, "ProcessSAPRWNotification", "SAPRWNotification has processsed successfully");
                }

            }

            catch (Exception ex)
            {
                processed = false;
                Tracer.Error(request, "ProcessSAPRWNotification",
                    "error is present while SAPRWNotification was processed" + ex.Message);
            }
            return processed;
        }

        # region Copy Print Maps

        private static bool CopyAttachments(SaprwNotification item, HttpRequestMessage request)
        {
            var check = false;
            var filePathnamewithserver = "";
            var lstNotificationLocations = item.NotificationLocations;
            if (lstNotificationLocations != null)
            {
                var destFileNamepath = "";
                try
                {
                    NotificationLocation[] objNotificationLocations = lstNotificationLocations.ToArray();

                    //Processing all notificationLocation
                    foreach (NotificationLocation notificationLocation in objNotificationLocations)
                    {
                        Tracer.Info(request, "CopyAttachments", "Copying file for Map Location " + notificationLocation.MapLocationNum);
                        //checking MapJOBID is not null
                        if (!string.IsNullOrEmpty(notificationLocation.MapJobId))
                        {
                            // process for Print Maps
                            //get the Server fullpath based on the DataCenter
                            if (DataCenter != null)
                            {
                                if (DataCenter == "DC1")
                                {
                                    destFileNamepath = ConfigurationManager.AppSettings["DC1DestFileNamePath"];
                                    destFileNamepath = destFileNamepath + "\\" + notificationLocation.NotificationId;
                                    filePathnamewithserver = notificationLocation.MapJobId;
                                }
                                else if (DataCenter == "DC2")
                                {
                                    destFileNamepath = ConfigurationManager.AppSettings["DC2DestFileNamePath"];
                                    destFileNamepath = destFileNamepath + "\\" + notificationLocation.NotificationId;
                                    filePathnamewithserver = notificationLocation.MapJobId;
                                }
                            }

                            //Coping Print file to destination server
                            if (!CopyFileToServer(request, item, destFileNamepath, notificationLocation,
                                filePathnamewithserver))
                            {
                                return false;
                            }
                            check = true;
                        }

                    }
                }

                catch (Exception ex)
                {
                    //Common.WriteToLog(ex.Message, Common.LoggingLevel.Error);
                    Tracer.Error(request, "CopyAttachments", ex.Message);
                }
            }
            else
            {
                // Common.WriteToLog("Obj of NotificationLocation is null", Common.LoggingLevel.Warning);
                Tracer.Warn(request, "CopyAttachments", "Obj of NotificationLocation is null");
            }

            return check;
        }

        private static bool CopyFileToServer(HttpRequestMessage request, SaprwNotification item,
            string destFileNamepath, NotificationLocation notificationLocation, string filePathnamewithserver)
        {
            bool check = false;
            bool Isprintpdfpresent = false;
            string fullfilePathnamewithserver = "";
            // Get the Wait Time to check the Files
            var waitTime = Convert.ToInt32(ConfigurationManager.AppSettings["WaitTimeforFileCheck"]);
            DateTime startcheckingFile = DateTime.Now;
            var totalTime = 0;
            Tracer.Info(request, "CopyFileToServer", filePathnamewithserver);

            while (!Isprintpdfpresent && totalTime != waitTime)
            {
                
                if (Directory.Exists(filePathnamewithserver))
                {
                    
                    Tracer.Info(request, "CopyFileToServer", "checking Print map file is available..");
                    string[] PrintFileinfo = Directory.GetFiles(filePathnamewithserver, "*.pdf", SearchOption.TopDirectoryOnly);
                    if (PrintFileinfo != null)
                    {
                        int filecount = PrintFileinfo.Count();
                        if (filecount > 0)
                        {
                            Tracer.Info(request, "CopyFileToServer", "Print map file is available.");
                            fullfilePathnamewithserver = PrintFileinfo[0];
                            Isprintpdfpresent = true;
                            notificationLocation.MapJobId = fullfilePathnamewithserver;
                            break;

                        }
                    }
                }

                var processingTime = DateTime.Now;
                totalTime = (processingTime - startcheckingFile).Minutes;
                if (totalTime >= waitTime)
                     break;
            }

            //Checking print Map is available
            //while (!File.Exists(fullfilePathnamewithserver) && totalTime != waitTime)
            //{
            //    var processingTime = DateTime.Now;
            //    totalTime = (processingTime - startcheckingFile).Minutes;
            //    if (totalTime >= waitTime)
            //        break;
            //}
            if (File.Exists(fullfilePathnamewithserver))
            {
                Tracer.Info(request, "CopyFileToServer", "File " + fullfilePathnamewithserver + " is present at source");

                //Checking Dest Directoty is present
                bool exists = Directory.Exists(destFileNamepath);
                if (!exists)
                {
                    //Creating the Directory at Destination path
                    Tracer.Info(request, "CopyFileToServer", "Creating the Directory at Destination path");
                    Directory.CreateDirectory(destFileNamepath);
                }
                // Copy the Print Map 
                File.Copy(fullfilePathnamewithserver,
                    destFileNamepath + "\\" + notificationLocation.MapLocationNum.ToString() + "_Map.pdf", true);
                Tracer.Info(request, "CopyFileToServer", "File " + fullfilePathnamewithserver + " is copied at dest server");
                check =
                    File.Exists(destFileNamepath + "\\" + notificationLocation.MapLocationNum.ToString() + "_Map.pdf");
                if (check)
                    Tracer.Info(request, "CopyFileToServer", "Copied File is present at dest server ");
                else
                    Tracer.Error(request, "CopyFileToServer", " Copied File is not present at dest server ");
            }

            // If print map is not available after wait time then E-mail to support team to troubleshoot.
            else
            {
                Tracer.Error(request, "CopyFileToServer",
                    "File " + fullfilePathnamewithserver + " is not present at source ");
                ObjCommon.SendEmailtoUser(request, item.Notificationdetails.NotificationId,
                    item.Notificationdetails.Lanid,
                    item.Notificationdetails.SubmitDate.ToString(CultureInfo.InvariantCulture),
                    "File '" + filePathnamewithserver + "' is not present");
            }

            return check;
        }

        # endregion

        #region CreatePDF

        private void CreatePdf(SaprwNotification item)
        {

            var destFile = "";
            // checking the Data center and get the dest File Path
            if (item.Notificationdetails.DataCenter == "DC1")
            {
                destFile = ConfigurationManager.AppSettings["DC1DestFileNamePath"] +
                           item.Notificationdetails.NotificationId;
            }
            if (item.Notificationdetails.DataCenter == "DC2")
            {
                destFile = ConfigurationManager.AppSettings["DC2DestFileNamePath"] +
                           item.Notificationdetails.NotificationId;
            }
            var listNotificationDetail = new List<string[]>
            {
                new[]
                {
                    item.Notificationdetails.NotificationId, item.Notificationdetails.MapNumber,
                    item.Notificationdetails.CircuitId, item.Notificationdetails.ConstructionType,
                    item.Notificationdetails.CorrectionType,
                    item.Notificationdetails.Department, SapDivision,
                    item.Notificationdetails.Lanid, item.Notificationdetails.NumberOfCorrection.ToString(),
                    item.Notificationdetails.SubmitDate.ToString(CultureInfo.InvariantCulture),
                    item.Notificationdetails.SubStation
                }
            };

            var saprwHeader = ConvertListToDataTable(listNotificationDetail);
            // Creating the Header PDF
            ExportToPdf(saprwHeader, destFile, item);
        }

        private static DataTable ConvertListToDataTable(List<string[]> list)
        {
            // New table.
            DataTable table = new DataTable();
            // Get max columns.
            int columns = list.Select(array => array.Length).Concat(new[] { 0 }).Max();
            // Add columns.
            for (int i = 0; i < columns; i++)
            {
                table.Columns.Add();
            }

            // Add rows.
            foreach (object[] array in list)
            {
                table.Rows.Add(array);
            }
            return table;
        }

        private static void ExportToPdf(DataTable dt, string destFile, SaprwNotification item)
        {
            HttpRequestMessage httprequestmsg = new HttpRequestMessage();
            if (!Directory.Exists(destFile))
            {
                Directory.CreateDirectory(destFile);
            }

            try
            {
                Document document = new Document(PageSize.A4);
                PdfWriter.GetInstance(document, new FileStream(destFile + "\\Header.pdf", FileMode.Create));
                document.Open();


                // Add Logo
                var addLogo = Image.GetInstance(AppDomain.CurrentDomain.BaseDirectory + "\\Logo.png");
                //addLogo.ScaleToFit(128, 37);
                addLogo.Alignment = Element.ALIGN_TOP;
                addLogo.SpacingAfter = 30f;

                document.Add(addLogo);

                //ADD Heading
                Paragraph heading = new Paragraph("Pacific Gas & Electric Company",
                    new Font(Font.FontFamily.HELVETICA, 30f, Font.BOLD))
                {
                    SpacingAfter = 18f


                };
                heading.Alignment = Element.ALIGN_JUSTIFIED_ALL;
                heading.SpacingBefore = 5f;

                document.Add(heading);


                Font headerFont = FontFactory.GetFont(FontFactory.TIMES_BOLD, 15, BaseColor.DARK_GRAY);
                Font headingFont = FontFactory.GetFont(FontFactory.COURIER_BOLD, 12, BaseColor.BLUE);
                Font rowFont = FontFactory.GetFont(FontFactory.COURIER, 10, BaseColor.BLACK);


                //ADD SAP RW NOtification header


                new PdfPTable(1);


                var table = new PdfPTable(6);
                var widths = new[] { 11f, 10f, 18f, 5f, 12f, 10f };

                table.SetWidths(widths);
                table.HorizontalAlignment = Element.ALIGN_LEFT;
                table.WidthPercentage = 100;
                table.SpacingBefore = 20f;
                table.SpacingAfter = 30f;
                var cell = new PdfPCell(new Phrase("SAP RW Notification Header", headerFont))
                {
                    Colspan = 6,
                    HorizontalAlignment = 0

                };


                //0=Left, 1=Centre, 2=Right
                table.AddCell(cell);


                //table.AddCell(new Phrase("MapNumber:", headingFont));
                //table.AddCell(new Phrase(dt.Rows[0][1].ToString(), rowFont));

                //table.AddCell(new Phrase("Circuit:", headingFont));
                //table.AddCell(new Phrase(dt.Rows[0][2].ToString(), rowFont));

                //table.AddCell(new Phrase("SubStation:", headingFont));
                //table.AddCell(new Phrase(dt.Rows[0][10].ToString(), rowFont));

                table.AddCell(new Phrase("LANID:", headingFont));
                table.AddCell(new Phrase(dt.Rows[0][7].ToString(), rowFont));

                table.AddCell(new Phrase("ConstructionType:", headingFont));
                table.AddCell(new Phrase(dt.Rows[0][3].ToString(), rowFont));

                table.AddCell(new Phrase("Division:", headingFont));
                table.AddCell(new Phrase(dt.Rows[0][6].ToString(), rowFont));

                table.AddCell(new Phrase("Date:", headingFont));
                table.AddCell(new Phrase(dt.Rows[0][9].ToString(), rowFont));

                table.AddCell(new Phrase("CorrectionType:", headingFont));
                table.AddCell(new Phrase(dt.Rows[0][4].ToString(), rowFont));

                table.AddCell(new Phrase("Department:", headingFont));
                table.AddCell(new Phrase(dt.Rows[0][5].ToString(), rowFont));



                document.Add(table);


                // Add Map Location Deatils

                var notificationlocationcount = item.NotificationLocations.Count();

                for (var i = 0; i < notificationlocationcount; i++)
                {
                    table = new PdfPTable(6);
                    widths = new[] { 15f, 15f, 10f, 10f, 12f, 10f };

                    table.SetWidths(widths);
                    table.HorizontalAlignment = Element.ALIGN_LEFT;
                    table.WidthPercentage = 100;
                    table.SpacingBefore = 20f;
                    table.SpacingAfter = 30f;
                    cell =
                        new PdfPCell(
                            new Phrase(
                                "SAP RW Notification " + item.NotificationLocations[i].MapLocationNum + " Detail",
                                headerFont))
                        {
                            Colspan = 6,
                            HorizontalAlignment = 0
                        };

                    //0=Left, 1=Centre, 2=Right
                    table.AddCell(cell);

                    table.AddCell(new Phrase("Location:", headingFont));
                    table.AddCell(new Phrase(item.NotificationLocations[i].MapLocation, rowFont));

                    cell = new PdfPCell(new Phrase("Comments:", headingFont))
                    {
                        HorizontalAlignment = 0
                    };
                    table.AddCell(cell);

                    cell = new PdfPCell(new Phrase(item.NotificationLocations[i].Comments, rowFont))
                    {
                        Colspan = 3,
                        HorizontalAlignment = 0
                    };
                    table.AddCell(cell);


                    table.AddCell(new Phrase("CorrectionType:", headingFont));
                    table.AddCell(new Phrase(item.NotificationLocations[i].MapCorrectionType, rowFont));

                    table.AddCell(new Phrase("Latitude:", headingFont));
                    table.AddCell(
                        new Phrase(item.NotificationLocations[i].Latitude.ToString(CultureInfo.InvariantCulture),
                            rowFont));

                    table.AddCell(new Phrase("Longitude:", headingFont));
                    table.AddCell(
                        new Phrase(item.NotificationLocations[i].Longitude.ToString(CultureInfo.InvariantCulture),
                            rowFont));

                    document.Add(table);
                }

                document.Close();
            }
            catch (Exception ex)
            {

                Tracer.Error(httprequestmsg, "ExporttoPDF", ex.Message);
            }

        }

        #endregion

        #region Insert Notification Records

        private void InsertSaprwNotification(SaprwNotification item, HttpRequestMessage request)
        {
            // Insering the Details and Locations
            InsertNotificationDetail(request, item.Notificationdetails);
            InsertNotificationLocations(item.NotificationLocations, request);
        }

        private void InsertNotificationDetail(HttpRequestMessage request, NotificationDetail objNotificationDetail)
        {
            if (objNotificationDetail != null)
            {


                Tracer.Info(request, "InsertNotificationDetail", "Inserting records in NotificationDetail Table");
                string insertSql = ConfigurationManager.AppSettings["InsertSQL_NotificationDetail"];

                try
                {
                    if (insertSql == "")
                    {
                        Tracer.Error(request, "InsertNotificationDetail", "SQL Statement is blank");
                    }
                    else
                    {
                        insertSql = insertSql + "'" + objNotificationDetail.NotificationId + "','" + objNotificationDetail.MapNumber + "' ,'" + objNotificationDetail.SubStation + "' ,'" + objNotificationDetail.CircuitId + "' ,TO_DATE('" + objNotificationDetail.SubmitDate.ToShortDateString() + "', 'MM/DD/YYYY'),'" + SapDivision + "' ,'" + objNotificationDetail.Department + "' ,'" + objNotificationDetail.Lanid + "' ,'" + objNotificationDetail.ConstructionType + "' ,'" + objNotificationDetail.CorrectionType + "' ," + objNotificationDetail.Status + " ,'" + FunctionalLocation + "' ,'" + Mainworkcenter + "' ," + objNotificationDetail.NumberOfCorrection + " ,'" + objNotificationDetail.DataCenter + "')";

                        ObjDataHelper.InsertData(insertSql);
                        Tracer.Info(request, "InsertNotificationDetail", "Inserting records in NotificationDetail Table has completed");
                    }
                }
                catch (Exception ex)
                {
                    Tracer.Error(request, "InsertNotificationDetail", "no Record has inserted for NotificationDetail" + ex.Message);
                }
            }
            else
            {

                Tracer.Warn(request, "InsertNotificationDetail", "Obj of NotificationDetail is null");
            }

        }

        private void InsertNotificationLocations(List<NotificationLocation> lstNotificationLocations,
            HttpRequestMessage request)
        {
            if (lstNotificationLocations != null)
            {
                Tracer.Info(request, "InsertNotificationLocations", "Inserting records in NotificationLocation Table");
                NotificationLocation[] objNotificationLocations = lstNotificationLocations.ToArray();

                //Process all NotificationLocation
                foreach (NotificationLocation objNotificationLocation in objNotificationLocations)
                {
                    string insertSql = ConfigurationManager.AppSettings["InsertSQL_NotificationLocation"];
                    try
                    {
                        if (insertSql == "")
                        {
                            Tracer.Error(request, "InsertNotificationLocations", "SQL Statement is blank");
                        }
                        else
                        {
                            if (objNotificationLocation.Comments == null)
                            {
                                objNotificationLocation.Comments = "";
                            }
                            insertSql = insertSql + "'" + objNotificationLocation.NotificationId + "'," + objNotificationLocation.MapLocationNum + " ,'" + objNotificationLocation.MapLocation.Replace("'", "''") + "' ,'" + objNotificationLocation.Comments.Replace("'", "''") + "' ," + objNotificationLocation.Latitude + "," + objNotificationLocation.Longitude + " ,'" + objNotificationLocation.MapJobId + "' ,'" + objNotificationLocation.MapCorrectionType + "')";
                            ObjDataHelper.InsertData(insertSql);
                            Tracer.Info(request, "InsertNotificationLocations", "Inserting records in NotificationLocation Table has completed");
                        }

                        //Get all the Attachment fileNames
                        if (objNotificationLocation.FileName != null)
                        {
                            string[] fileNames = objNotificationLocation.FileName;
                            foreach (string filePathName in fileNames)
                            {
                                string insertAttachmentSql = ConfigurationManager.AppSettings["InsertSQL_Attachment"];
                                if (insertAttachmentSql == "")
                                {
                                    Tracer.Info(request, "InsertNotificationLocations",
                                        "InsertSQL_Attachment Statement is blank");
                                }
                                else
                                {
                                    insertAttachmentSql = insertAttachmentSql + "'" +
                                                          objNotificationLocation.NotificationId +
                                                          "'," + objNotificationLocation.MapLocationNum + " ,'" +
                                                          filePathName.Replace("'", "''") + "')";
                                    ObjDataHelper.InsertData(insertAttachmentSql);
                                    Tracer.Info(request, "InsertNotificationLocations",
                                        "Inserting records in Attachment Table has completed");

                                }
                            }
                        }

                    }
                    catch (Exception ex)
                    {
                        Tracer.Error(request, "InsertNotificationLocations",
                             "no Record has inserted for NotificationLocation" + ex.Message);
                    }

                }
            }
            else
            {
                Tracer.Info(request, "InsertNotificationLocations", "Obj of NotificationLocation is null");
            }

        }


        private string GetFunctionalLocation(NotificationDetail item)
        {
            // Get the FLOC
            string functionalLocation = ObjMapper.Data[item.Division].Floc;
            item.FunctionalLocation = functionalLocation;

            return functionalLocation;
        }

        private string GetMainWorkCenter(NotificationDetail item)
        {
            // Get the MainWorkCenter
            string mainworkcenter = ObjMapper.Data[item.Division].MainWorkCenter;
            item.MainWorkCenter = mainworkcenter;

            return mainworkcenter;
        }

        private string GetSapDivision(NotificationDetail item)
        {
            // Get the SAP Division
            string sapDivision = ObjMapper.Data[item.Division].SapDivision;
            item.Division = sapDivision;
            return sapDivision;
        }

        #endregion

        #region Sending Notifaction to EI

        private void SendNotificationDetailtoEi(SaprwNotification item, HttpRequestMessage request)
        {
            if (request == null) throw new ArgumentNullException("request");

            try
            {

                //Map Object

                MapCorrection_PortClient objPortClient = new MapCorrection_PortClient();

                //Parameters Object 

                MapCorrectionRequestMessageType msg = new MapCorrectionRequestMessageType();


                // Header Object
                Tracer.Info(request, "SendNotificationDetailtoEI", "Create Map Data header");
                tMapDataHeader objMapDataHeader = MapDataHeader(item);
                //   string jsonMapdataHeader = JsonConvert.SerializeObject(objMapDataHeader);
                //   Tracer.Info(request, "SendNotificationDetailtoEI " + jsonMapdataHeader, "");


                //Detail Object
                Tracer.Info(request, "SendNotificationDetailtoEI", "Create Map Data Detail");
                tMapDataDetail[] objMapDataDetail = MapDataDetail(item);
                // string jsonMapDataDetail = JsonConvert.SerializeObject(objMapDataDetail);
                // Tracer.Info(request, "SendNotificationDetailtoEI " + jsonMapDataDetail, "");

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
                    MessageID = item.Notificationdetails.NotificationId
                };

                MapCorrectionPayloadType objMapCorrPayLoad = new MapCorrectionPayloadType();


                MapCorrectionType objMapCorrType = new MapCorrectionType { createMapCorrectionRequest = objMapDataReq };
                objMapCorrPayLoad.MapCorrection = objMapCorrType;

                msg.Header = objHeaderType;
                msg.Payload = objMapCorrPayLoad;


                //mp.CreatedMapCorrectionCompleted += new EventHandler<CreatedMapCorrectionCompletedEventArgs>(mp_CreatedMapCorrectionCompleted);



                Tracer.Info(request, "SendNotificationDetailtoEi", "Sending records to EI");
                MapCorrectionResponseMessageType obj = objPortClient.CreateMapCorrection(msg);


                if (obj.Reply.Result == ReplyTypeResult.FAILED)
                {
                    string updateStatement = ConfigurationManager.AppSettings["Update_NotificationDetail_Fail"];
                    try
                    {
                        if (updateStatement != "")
                        {
                            updateStatement = updateStatement + item.Notificationdetails.NotificationId + "'";
                            ObjDataHelper.UpdateData(updateStatement);
                        }
                    }

                    catch (Exception)
                    {
                        // ignored
                    }
                    HttpRequestMessage h = new HttpRequestMessage();
                    ObjCommon.SendEmailtoUser(h, item.Notificationdetails.NotificationId, item.Notificationdetails.Lanid,
                        item.Notificationdetails.SubmitDate.ToString(CultureInfo.InvariantCulture),
                        "Sending Notification to EI has failed");
                }
                else
                {
                    Tracer.Info(request, "SendNotificationDetailtoEi", "records to EI has send successfully");
                    string updateStatement = ConfigurationManager.AppSettings["Update_NotificationDetail_Pass"];
                    try
                    {
                        if (updateStatement != "")
                        {
                            updateStatement = updateStatement + item.Notificationdetails.NotificationId + "'";
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
                
                Tracer.Error(request, "SendNotificationtoEI", ex);
                string updateStatement = ConfigurationManager.AppSettings["Update_NotificationDetail_Fail"];
                try
                {
                    if (updateStatement != "")
                    {
                        updateStatement = updateStatement + item.Notificationdetails.NotificationId + "'";
                        ObjDataHelper.UpdateData(updateStatement);
                    }
                }

                catch (Exception)
                {
                    // ignored
                }
                HttpRequestMessage h = new HttpRequestMessage();
                if (ex.GetType().FullName == "System.IO.FileNotFoundException")
                {
                    ObjCommon.SendEmailtoUser(h, item.Notificationdetails.NotificationId, item.Notificationdetails.Lanid,
                    item.Notificationdetails.SubmitDate.ToString(CultureInfo.InvariantCulture), "File " + ex.Message + " is not present.");
                }
                else 
                    ObjCommon.SendEmailtoUser(h, item.Notificationdetails.NotificationId, item.Notificationdetails.Lanid,
                    item.Notificationdetails.SubmitDate.ToString(CultureInfo.InvariantCulture), ex.Message);
            }
        }

        private static tMapDataDetail[] MapDataDetail(SaprwNotification item)
        {
            int sectionCount = item.NotificationLocations.Count();
            //Section object

            tMapDataDetail[] objMapDataDetail = new tMapDataDetail[sectionCount];
            string destFileNamepath = "";
            bool check = false;


            try
            {

                //tMapDataDetail[] objMapDataDetail = new tMapDataDetail()[sectionCount];
                for (int i = 0; i < sectionCount; i++)
                {
                    // Check the DataCenter to get the destFileName Path
                    if (DataCenter == "DC1")
                    {
                        destFileNamepath = ConfigurationManager.AppSettings["DC1DestFileNamePath"];
                        destFileNamepath = destFileNamepath + "\\" + item.NotificationLocations[i].NotificationId;
                    }
                    else if (DataCenter == "DC2")
                    {
                        destFileNamepath = ConfigurationManager.AppSettings["DC2DestFileNamePath"];
                        destFileNamepath = destFileNamepath + "\\" + item.NotificationLocations[i].NotificationId;
                    }

                    //Attachment Object
                    //count of filename
                    int filenamecount = 0;
                    if (item.NotificationLocations[i].FileName != null)
                    {
                        filenamecount = item.NotificationLocations[i].FileName.Count();
                    }
                    var objAttach = i == 0 ? new tAttachment[filenamecount + 2] : new tAttachment[filenamecount + 1];

                    string filename;
                    for (int j = 0; j < filenamecount; j++)
                    {
                        objAttach[j] = new tAttachment();
                        filename = destFileNamepath + "\\" + item.NotificationLocations[i].FileName[j];
                        if (!File.Exists(filename))
                            throw new FileNotFoundException(filename);
                        
                            objAttach[j].Name =
                                filename.Substring(filename.LastIndexOf("\\", StringComparison.Ordinal) + 1);
                            objAttach[j].Data = File.ReadAllBytes(filename);
                        


                    }

                    objAttach[filenamecount] = new tAttachment();
                    filename = destFileNamepath + "\\" + item.NotificationLocations[i].MapLocationNum.ToString() +
                               "_Map.pdf";
                    check =  File.Exists(filename);
                    if (check == false)
                        throw new FileNotFoundException(filename);
                    
                        if (filename.Length > filename.LastIndexOf("\\", StringComparison.Ordinal) + 1)
                            objAttach[filenamecount].Name =
                                filename.Substring(filename.LastIndexOf("\\", StringComparison.Ordinal) + 1);
                        objAttach[filenamecount].Data = File.ReadAllBytes(filename);
                    


                    if (i == 0)
                    {

                        objAttach[filenamecount + 1] = new tAttachment();
                        filename = destFileNamepath + "\\" + "Header.PDF";
                        check = File.Exists(filename);
                        if (check == false)
                            throw new FileNotFoundException(filename);
                        
                            if (filename.Length > filename.LastIndexOf("\\", StringComparison.Ordinal) + 1)
                                objAttach[filenamecount + 1].Name =
                                    filename.Substring(filename.LastIndexOf("\\", StringComparison.Ordinal) + 1);
                            objAttach[filenamecount + 1].Data = File.ReadAllBytes(filename);
                        

                    }

                    objMapDataDetail[i] = new tMapDataDetail
                    {
                        Comments = item.NotificationLocations[i].Comments,
                        CorrectionType = item.NotificationLocations[i].MapCorrectionType,
                        Latitude = item.NotificationLocations[i].Latitude.ToString(CultureInfo.InvariantCulture),
                        LocationDescription = item.NotificationLocations[i].MapLocation,
                        LocationID = item.NotificationLocations[i].MapLocationNum.ToString(),
                        Longitude = item.NotificationLocations[i].Longitude.ToString(CultureInfo.InvariantCulture),
                        Attachment = objAttach
                    };

                }
            }

            catch (FileNotFoundException ex)
            {
                throw ex;
            }
            return objMapDataDetail;
        }

        private static tMapDataHeader MapDataHeader(SaprwNotification item)
        {
            tMapDataHeader objMapDataHeader = new tMapDataHeader
            {
                Division = item.Notificationdetails.Division,
                FunctionalLocation = item.Notificationdetails.FunctionalLocation,
                MainWorkCenter = item.Notificationdetails.MainWorkCenter,
                NumberOfCorrection = item.Notificationdetails.NumberOfCorrection.ToString()
            };


            // If correction type is electric distribution
            if (item.Notificationdetails.CorrectionType == "ED")
                objMapDataHeader.CorrectionCategory = tCorrectionCategory.ED;
            // If correction type is landbase
            else if (item.Notificationdetails.CorrectionType == "LAND")
                objMapDataHeader.CorrectionCategory = tCorrectionCategory.LAND;

            objMapDataHeader.Department = item.Notificationdetails.Department;
            objMapDataHeader.Feeder = item.Notificationdetails.CircuitId;
            //objMapDataHeader.FunctionalLocation = 
            objMapDataHeader.MapNumber = item.Notificationdetails.MapNumber;
            objMapDataHeader.OperationsArea = item.Notificationdetails.Division;

            // If Construction type is OverHead
            if (item.Notificationdetails.ConstructionType == "OH")
                objMapDataHeader.ServiceDeliveryMethod = tServiceDeliveryMethod.OverHead;
            // If Construction type is UnderGround
            else if (item.Notificationdetails.ConstructionType == "UG")
                objMapDataHeader.ServiceDeliveryMethod = tServiceDeliveryMethod.UnderGround;

            objMapDataHeader.Substation = item.Notificationdetails.SubStation;
            objMapDataHeader.TransactionDateTime = item.Notificationdetails.SubmitDate.ToString("MM/dd/yyyy");
            objMapDataHeader.UniqueNotificationID = item.Notificationdetails.NotificationId;
            objMapDataHeader.UserID = item.Notificationdetails.Lanid;


            return objMapDataHeader;
        }

        #endregion

    }
}