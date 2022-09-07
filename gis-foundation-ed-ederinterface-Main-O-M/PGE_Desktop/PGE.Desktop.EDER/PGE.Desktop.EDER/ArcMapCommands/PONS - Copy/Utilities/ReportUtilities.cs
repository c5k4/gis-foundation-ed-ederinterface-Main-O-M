using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.OracleClient;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Windows.Documents;
using System.Windows.Xps.Packaging;
//using System.Windows.Documents;
using ClosedXML.Excel;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.Reflection;
using Telvent.Delivery.Diagnostics;
using System.Text;

namespace Telvent.PGE.ED.Desktop.ArcMapCommands.PONS
{
    class ReportUtilities
    {
        private static UtilityFunctions utilityFunctions = new UtilityFunctions();
        private static readonly Log4NetLogger _logger = new Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "EDERDesktop.log4net.config");
        public ReportUtilities()
        {
          
        }

        public static bool SendMail(ref CustomerReport pReport)
        {
            try
            {
                if (string.IsNullOrEmpty(pReport.SubmitterID)) return false;
                if (!GetShutdownID(ref pReport)) return false;
                if (!SendtoPSL(pReport)) return false;
                MailMessage message = new MailMessage();
                message.To.Add(pReport.PSC_ID + "@pge.com");
                message.From = new MailAddress(pReport.SubmitterID + "@pge.com");
                message.Subject = "Planned Shutdown Request Confirmation";
                message.Body = pReport.SubmitterID + " submitted an outage for you on " + DateTime.Today.ToShortDateString() + ".  The PONS information for this outage is available on the Shutdown Letters website: " + utilityFunctions.ReadConfigurationValue("PSL_PORTAL_URL");
                message.Body += "\nPSC Information";
                message.Body += "\n----------------";
                message.Body += "\nShutdown ID Number: " + pReport.ReportID;
                message.Body += "\nSubmitter's LanID:  " + pReport.SubmitterID;
                message.Body += "\nPONS ID: " + pReport.SubmitterID;
                string sTemp = "\nOutage date: " + pReport.OutageDetails[0].Outage_Date_OFF + " from " + pReport.OutageDetails[0].OFF_Time + " to " /*+ ((pReport.OutageDetails[0].Outage_Date_OFF != pReport.OutageDetails[0].Outage_Date_ON) ? pReport.OutageDetails[0].Outage_Date_ON : string.Empty)*/ + pReport.OutageDetails[0].ON_Time;
                sTemp += (pReport.OutageDetails.Count > 1) ? "and\n" + pReport.OutageDetails[1].Outage_Date_OFF + " from " + pReport.OutageDetails[1].OFF_Time + " to " /*+ ((pReport.OutageDetails[1].Outage_Date_OFF != pReport.OutageDetails[1].Outage_Date_ON) ? pReport.OutageDetails[1].Outage_Date_ON : string.Empty)*/ + pReport.OutageDetails[1].ON_Time : string.Empty;
                sTemp += ", weather permitting.";
                message.Body += sTemp;
                message.Body += "\nAffected Area: " + pReport.Description;
                message.IsBodyHtml = false;
                if (pReport.CC_Me_Mail)
                    message.CC.Add(Environment.UserName + "@pge.com");
                SmtpClient client = new SmtpClient(utilityFunctions.ReadConfigurationValue("MAIL_SERVER"));//Convert.ToString(pDic_Config["MAIL_SMTPSERVER"]));
                client.Send(message);
                return true;
            }
            catch (Exception ex)
            {
                _logger.Error("Error Sending Mail "+ex.Message);
                return false;
            }
        }

        private static bool SendtoPSL(CustomerReport pReport)
        {
            try
            {
                using (SqlBulkCopy bulkCopy = new SqlBulkCopy(utilityFunctions.ReadConfigurationValue("PSL_DB_STRING"), SqlBulkCopyOptions.KeepIdentity & SqlBulkCopyOptions.KeepNulls))
                {
                    bulkCopy.DestinationTableName = utilityFunctions.ReadConfigurationValue("PSL_DB_STG_TBL");
                    try
                    {
                        // Write from the source to the destination.
                        bulkCopy.ColumnMappings.Add(new SqlBulkCopyColumnMapping("SubmitUser", "SubmitUser"));
                        bulkCopy.ColumnMappings.Add(new SqlBulkCopyColumnMapping("SubmitDT", "SubmitDT"));
                        bulkCopy.ColumnMappings.Add(new SqlBulkCopyColumnMapping("SubmitTM", "SubmitTM"));
                        bulkCopy.ColumnMappings.Add(new SqlBulkCopyColumnMapping("Vicinity", "Vicinity"));
                        bulkCopy.ColumnMappings.Add(new SqlBulkCopyColumnMapping("AddrLine1", "AddrLine1"));
                        bulkCopy.ColumnMappings.Add(new SqlBulkCopyColumnMapping("AddrLine2", "AddrLine2"));
                        bulkCopy.ColumnMappings.Add(new SqlBulkCopyColumnMapping("AddrLine3", "AddrLine3"));
                        bulkCopy.ColumnMappings.Add(new SqlBulkCopyColumnMapping("AddrLine4", "AddrLine4"));
                        bulkCopy.ColumnMappings.Add(new SqlBulkCopyColumnMapping("AddrLine5", "AddrLine5"));
                        bulkCopy.ColumnMappings.Add(new SqlBulkCopyColumnMapping("CGCTNum", "CGCTNum"));
                        bulkCopy.ColumnMappings.Add(new SqlBulkCopyColumnMapping("SSD", "SSD"));
                        bulkCopy.ColumnMappings.Add(new SqlBulkCopyColumnMapping("Phone", "Phone"));
                        bulkCopy.ColumnMappings.Add(new SqlBulkCopyColumnMapping("Work1DT", "Work1DT"));
                        bulkCopy.ColumnMappings.Add(new SqlBulkCopyColumnMapping("Work1TMStart", "Work1TMStart"));
                        bulkCopy.ColumnMappings.Add(new SqlBulkCopyColumnMapping("Work1TMEnd", "Work1TMEnd"));
                        bulkCopy.ColumnMappings.Add(new SqlBulkCopyColumnMapping("Work2DT", "Work2DT"));
                        bulkCopy.ColumnMappings.Add(new SqlBulkCopyColumnMapping("Work2TMStart", "Work2TMStart"));
                        bulkCopy.ColumnMappings.Add(new SqlBulkCopyColumnMapping("Work2TMEnd", "Work2TMEnd"));
                        bulkCopy.ColumnMappings.Add(new SqlBulkCopyColumnMapping("AccountID", "AccountID"));
                        bulkCopy.ColumnMappings.Add(new SqlBulkCopyColumnMapping("OutageID", "OutageID"));
                        bulkCopy.ColumnMappings.Add(new SqlBulkCopyColumnMapping("ContactID", "ContactID"));
                        bulkCopy.ColumnMappings.Add(new SqlBulkCopyColumnMapping("SPID", "SPID"));
                        bulkCopy.ColumnMappings.Add(new SqlBulkCopyColumnMapping("SPAddrLine1", "SPAddrLine1"));
                        bulkCopy.ColumnMappings.Add(new SqlBulkCopyColumnMapping("SPAddrLine2", "SPAddrLine2"));
                        bulkCopy.ColumnMappings.Add(new SqlBulkCopyColumnMapping("SPAddrLine3", "SPAddrLine3"));
                        bulkCopy.ColumnMappings.Add(new SqlBulkCopyColumnMapping("CustType", "CustType"));
                        bulkCopy.ColumnMappings.Add(new SqlBulkCopyColumnMapping("meter_number", "meter_number"));
                        bulkCopy.ColumnMappings.Add(new SqlBulkCopyColumnMapping("division", "division"));
                        bulkCopy.ColumnMappings.Add(new SqlBulkCopyColumnMapping("cn_fld1", "cn_fld1"));
                        bulkCopy.ColumnMappings.Add(new SqlBulkCopyColumnMapping("cn_fld2", "cn_fld2"));
                        bulkCopy.ColumnMappings.Add(new SqlBulkCopyColumnMapping("cn_fld3", "cn_fld3"));
                        
                        bulkCopy.WriteToServer(CustomerListtoDataTable(pReport));
                        CallPSLSP(pReport);
                    }
                    catch (SqlException sqlex)
                    {
                        //Console.WriteLine(sqlex.Message);
                        _logger.Error("Error Sending to PSL " + sqlex.Message);
                        return false;
                    }
                    catch (Exception ex)
                    {
                        //Console.WriteLine(ex.Message);
                        _logger.Error("Error Sending to PSL " + ex.Message);
                        return false;
                    }
                }

                //SqlConnection conn = new SqlConnection();
                //conn.ConnectionString = utilityFunctions.ReadConfigurationValue("PSL_DB_STRING");// "Server = ASCdev01;Database = BPM_ShutdownLtrs;User Id= CustInfo_Load;Password=40dc-a68f-5e74;";
                //SqlCommand pSQLCommand = new SqlCommand();
                //ArrayList pList_Insert = ComputeSQLInsert(pReport);
                //for (int iCount_Insert = 0; iCount_Insert < pList_Insert.Count; ++iCount_Insert)
                //{
                //    pSQLCommand.CommandText = Convert.ToString(pList_Insert[iCount_Insert]);
                //    pSQLCommand.Connection = conn;
                //    pSQLCommand.CommandType = CommandType.Text;
                //    pSQLCommand.Connection.Open();
                //    try
                //    {
                //        pSQLCommand.ExecuteNonQuery();                   
                        
                //    }
                //    catch (SqlException sqlex)
                //    {
                //        Console.WriteLine(sqlex.Message);
                //        return false;
                //    }
                //    catch (Exception ex)
                //    {
                //        Console.WriteLine(ex.Message);
                //        return false;
                //    }
                //    pSQLCommand.Connection.Close();
                //}

                return true;
            }
            catch (SqlException sqlex)
            {
                //Console.WriteLine(sqlex.Message);
                _logger.Error("Error Sending to PSL " + sqlex.Message);
                return false;
            }
            catch (Exception ex)
            {
                //Console.WriteLine(ex.Message);
                _logger.Error("Error Sending to PSL " + ex.Message);
                return false;
            }
        }

        private static void CallPSLSP(CustomerReport pReport)
        {
            SqlConnection sqlConnection1 = new SqlConnection(utilityFunctions.ReadConfigurationValue("PSL_DB_STRING"));
            SqlCommand cmd = new SqlCommand();

            cmd.CommandText = utilityFunctions.ReadConfigurationValue("PSL_SP_NAME");
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Connection = sqlConnection1;
            sqlConnection1.Open();
            try
            {                
                cmd.Parameters.Add("@iModeFlag", SqlDbType.TinyInt).Value = 2;
                cmd.Parameters.Add("@iOutageID", SqlDbType.Int).Value = pReport.ReportID;
                cmd.ExecuteNonQuery();                
            }
            catch (SqlException sqlex)
            {
                //Console.WriteLine(sqlex.Message);
                _logger.Error("Error Calling PSL SP " + sqlex.Message);
            }
            catch (Exception ex)
            {
                //Console.WriteLine(ex.Message);
                _logger.Error("Error Calling PSL PS " + ex.Message);
            }
            sqlConnection1.Close();
        }

        private static DataTable CustomerListtoDataTable(CustomerReport pReport)
        {
            try
            {
                DataTable pDT_Return = new DataTable("tblStaging");
                pDT_Return.Columns.Add("SubmitUser", typeof(string));
                pDT_Return.Columns.Add("SubmitDT", typeof(DateTime));
                pDT_Return.Columns.Add("SubmitTM", typeof(DateTime));
                pDT_Return.Columns.Add("Vicinity", typeof(string));
                pDT_Return.Columns.Add("AddrLine1", typeof(string));
                pDT_Return.Columns.Add("AddrLine2", typeof(string));
                pDT_Return.Columns.Add("AddrLine3", typeof(string));
                pDT_Return.Columns.Add("AddrLine4", typeof(string));
                pDT_Return.Columns.Add("AddrLine5", typeof(string));
                pDT_Return.Columns.Add("CGCTNum", typeof(string));
                pDT_Return.Columns.Add("SSD", typeof(string));
                pDT_Return.Columns.Add("Phone", typeof(string));
                pDT_Return.Columns.Add("Work1DT", typeof(DateTime));
                pDT_Return.Columns.Add("Work1TMStart", typeof(DateTime));
                pDT_Return.Columns.Add("Work1TMEnd", typeof(DateTime));
                pDT_Return.Columns.Add("Work2DT", typeof(DateTime));
                pDT_Return.Columns.Add("Work2TMStart", typeof(DateTime));
                pDT_Return.Columns.Add("Work2TMEnd", typeof(DateTime));
                pDT_Return.Columns.Add("AccountID", typeof(string));
                pDT_Return.Columns.Add("OutageID", typeof(Int32));
                pDT_Return.Columns.Add("ContactID", typeof(Int16));
                pDT_Return.Columns.Add("SPID", typeof(string));
                pDT_Return.Columns.Add("SPAddrLine1", typeof(string));
                pDT_Return.Columns.Add("SPAddrLine2", typeof(string));
                pDT_Return.Columns.Add("SPAddrLine3", typeof(string));
                pDT_Return.Columns.Add("CustType", typeof(string));
                pDT_Return.Columns.Add("meter_number", typeof(string));
                pDT_Return.Columns.Add("division", typeof(string));
                pDT_Return.Columns.Add("cn_fld1", typeof(string));
                pDT_Return.Columns.Add("cn_fld2", typeof(string));
                pDT_Return.Columns.Add("cn_fld3", typeof(string));

                foreach (Customer customer in pReport.AffectedCustomers)
                {
                    object[] obj = new object[] {pReport.SubmitterID.Substring(0,(pReport.SubmitterID.Length>5?5:pReport.SubmitterID.Length)),
                                             DateTime.Today,
                                             DateTime.Now,
                                             pReport.Description.Replace("'", "''"),
                                             customer.CustomerName.MailName1,
                                             customer.CustomerName.MailName2,
                                             customer.CustomerAddress.StreetNumber,
                                             customer.CustomerAddress.StreetName1 +" "+customer.CustomerAddress.StreetName2,
                                             customer.CustomerAddress.City+", "+customer.CustomerAddress.State+" "+customer.CustomerAddress.ZIPCode,
                                             customer.CGC12,
                                             customer.SSD,
                                             customer.PhoneNumber.AreaCode+customer.PhoneNumber.PhoneNumber,
                                             Convert.ToDateTime(pReport.OutageDetails[0].Outage_Date_OFF),
                                             Convert.ToDateTime(pReport.OutageDetails[0].OFF_Time),
                                             Convert.ToDateTime(pReport.OutageDetails[0].ON_Time),//DBNull.Value,
                                             DBNull.Value,//Convert.ToDateTime(pReport.OutageDetails[0].Outage_Date_ON),
                                             Convert.ToDateTime(pReport.OutageDetails[0].ON_Time),
                                             DBNull.Value,
                                             customer.CustomerAccountNumber,
                                             pReport.ReportID,// Convert.ToInt32(0),
                                             Convert.ToInt32(pReport.Contact_ID),//DBNull.Value,
                                             customer.ServicePointID,
                                             customer.MailAddress.StreetNumber,
                                             customer.MailAddress.StreetName1+" "+customer.MailAddress.StreetName2,
                                             customer.MailAddress.City+", "+customer.MailAddress.State+ " "+customer.MailAddress.ZIPCode,
                                             customer.CustomerType,
                                             customer.MeterNumber,
                                             customer.Division,
                                             DBNull.Value,
                                             DBNull.Value,
                                             DBNull.Value
                };
                    pDT_Return.Rows.Add(obj);
                }
                return pDT_Return;
            }
            catch (Exception ex)
            {
                _logger.Error("Error Getting Datatable from data " + ex.Message);
                return null;
            }
        }

        private static ArrayList ComputeSQLInsert(CustomerReport pReport)
        {
            ArrayList pList_Insert = new ArrayList();

            foreach (Customer customer in pReport.AffectedCustomers)
            {
                string sql = "INSERT INTO dbo.tblStaging"
                     + "(SubmitUser, SubmitDT, SubmitTM, Vicinity, AddrLine1, AddrLine2, AddrLine3, AddrLine4, AddrLine5, CGCTNum, SSD, Phone, Work1DT, Work1TMStart, Work1TMEnd, "
                     + "Work2DT, Work2TMStart, Work2TMEnd, AccountID, OutageID, ContactID, SPID, SPAddrLine1, SPAddrLine2, SPAddrLine3, CustType, meter_number, division, cn_fld1, "
                     + "cn_fld2, cn_fld3)"
                     + "VALUES"
                     + "(" + "'" + pReport.SubmitterID + "'"
                     + ", Cast('" + "06/11/2015 10:34:09 PM" + "' as datetime)" //+ ", CONVERT(datetime, '" + DateTime.Today.ToShortDateString() + "', 5)"
                     + ", Cast('" + "06/11/2015 10:34:09 PM" + "' as datetime)," //+ ", CONVERT([varchar](10), '" + DateTime.Now.ToShortTimeString() + "', 108),"
                     + "NULL, "
                     + "'" + customer.CustomerName.MailName1 + "', "//data.CustMail1
                     + "'" + customer.CustomerName.MailName2 + "', "//data.CustMail2
                     + "'" + customer.MailAddress.StreetNumber + " " + customer.MailAddress.StreetName1 + "',"//data.CustMailStNum + " " + data.CustMailStName1
                     + "'" + customer.MailAddress.StreetName2 + "',"//data.CustMailStName2);
                     + "'" + customer.MailAddress.City + " " + customer.MailAddress.State + " " + customer.MailAddress.ZIPCode + "',"//data.CustMailCity + " " + data.MailState + " " + data.CustMailZipCode
                     + "'" + customer.CGC12 + "',"//data.CGC12
                     + "'" + customer.SSD + "',"//data.TransSSD
                     + "'',"//data.CustPhone
                     + "Cast('06/11/2015 10:34:09 PM' as datetime),"//Sample Date//+ "CONVERT(datetime, '23-06-15 10:34:09 PM', 5),"//Sample Date
                     + "Cast('06/11/2015 10:34:09 PM' as datetime),"// /Sample Date//+ "CONVERT(datetime, '10:34:09 PM', 5),"// /Sample Date
                     + "Cast('06/11/2015 11:34:09 PM' as datetime),"// /Sample Date//+ "CONVERT(datetime, '11:34:09 PM', 5),"// /Sample Date
                     + "Cast('06/11/2015 10:34:09 PM' as datetime),"// /Sample Date//+ "CONVERT(datetime, '30-06-15 10:34:09 PM', 5),"// /Sample Date
                     + "Cast('06/11/2015 10:34:09 AM' as datetime),"// /Sample Date//+ "CONVERT(datetime, '10:34:09 AM', 5),"// /Sample Date
                     + "Cast('06/11/2015 01:34:09 PM' as datetime), "///Sample Date//+ "CONVERT(datetime, '01:34:09 PM', 5), "///Sample Date
                     + "'" + customer.CustomerAccountNumber + "',"//data.SerActID
                     + "'" + pReport.ReportID + "',"//done
                     + "'" + pReport.Contact_ID + "',"//done
                     + "'" + customer.ServicePointID + "',"//data.ServicepointId
                     + "'" + customer.MailAddress.StreetNumber + " " + customer.MailAddress.StreetName1 + "',"//data.SerStNumber + " " + data.SerStName1
                     + "'" + customer.MailAddress.StreetName2 + "',"//data.SerStName2
                     + "'" + customer.MailAddress.City + " " + customer.MailAddress.State + " " + customer.MailAddress.ZIPCode + "',"//data.SerCity + " " + data.SerState + " " + data.ServiceZip
                     + "'" + customer.CustomerType + "',"//data.CustType
                     + "'" + customer.MeterNumber + "', "//data.MeterNum
                     + (customer.Division.Length == 0 ? "0" : customer.Division) + ", "//data.SerDivision
                     + "NULL,"//dont know
                     + "NULL,"//dont know
                     + "NULL)";//dont know

                pList_Insert.Add(sql);
            }
            return pList_Insert;
        }
        
        private static bool GetShutdownID(ref CustomerReport pReport)
        {
            try
            {
                // SB : Replace GIS_DB_CONN_STR with following code
                OracleConnection OraConn = utilityFunctions.OpenConnection(utilityFunctions.ReadConfigurationValue("EDER_ConnectionString")); //pUtilities.OpenConnection("Data Source=EDGIST1D;User Id=gis_i;Password=gisTemp!;");
                OracleCommand OraCmd = default(OracleCommand);
                object oShutDownID = default(object);
                try
                {
                    OraCmd = new OracleCommand(utilityFunctions.ReadConfigurationValue("PONS_SHUTDOWNID_PCKG"), OraConn);//"PONS_TEST.GEN_SHUTDOWNID", OraConn);
                    OraCmd.CommandType = CommandType.StoredProcedure;

                    OracleParameter param = default(OracleParameter);
                    param = new OracleParameter();

                    param = OraCmd.Parameters.Add("p_shutdownId", OracleType.Number);
                    param.Direction = ParameterDirection.Output;

                    param = default(OracleParameter);
                    param = OraCmd.Parameters.Add("p_error", OracleType.VarChar, 100);
                    param.Direction = ParameterDirection.Output;

                    param = default(OracleParameter);
                    param = OraCmd.Parameters.Add("p_success", OracleType.Number, 100);
                    param.Direction = ParameterDirection.Output;

                    OraCmd.ExecuteNonQuery();
                    oShutDownID = OraCmd.Parameters["p_shutdownId"].Value;
                    return true;
                }
                catch (Exception ex) { return false; }
                finally { OraConn.Close(); pReport.ReportID = Convert.ToString(oShutDownID); }
            }
            catch (Exception ex)
            {
                _logger.Error("Error getting Shutdown ID " + ex.Message);
                return false;
            }
        }

        public static string CreatePDF(CustomerReport pReport)
        {
            try
            {
                string sPath = GetPath("pdf");
                Document doc = new Document(PageSize.A4.Rotate(), 25f, 25f, 10f, 25f);
                PdfWriter pdfWriter = PdfWriter.GetInstance(doc, new FileStream(sPath, FileMode.Create));
                PDFPageEvent pPageEvent = new PDFPageEvent(pReport);
                pdfWriter.PageEvent = pPageEvent;

                doc.Open();
                Font font_Arial8 = FontFactory.GetFont("arial", 8, Font.NORMAL);

                 //fontBold8 = FontFactory.GetFont("verdana", 9, Font.BOLD),
                    //fontNormal7_1 = FontFactory.GetFont("verdana", 8, Font.NORMAL),              
                //fontBold7 = FontFactory.GetFont("verdana", 8, Font.BOLD);

                PdfPTable ptable = new PdfPTable(7);
                float[] widths = new float[] { 25f, 100f, 150f, 85f, 50f, 50f, 50f };
                ptable.SetWidths(widths);
                ptable.WidthPercentage = 100;
                ptable.DefaultCell.Border = 0;
                ptable.DefaultCell.BorderColor = BaseColor.WHITE;

                #region Commented 
                //string sTemp = "Interruption date: ";

                //iTextSharp.text.Paragraph paragraph = new iTextSharp.text.Paragraph();//sTemp, fontBold11);
                //iTextSharp.text.Phrase phrase = new iTextSharp.text.Phrase(sTemp, fontBold8);

                //phrase.Font.SetStyle(Font.BOLD | Font.UNDERLINE);
                //doc.Add(phrase);

                //sTemp = string.Empty + pReport.OutageDetails[0].Outage_Date_OFF + " from " + pReport.OutageDetails[0].OFF_Time + " to " /*+ ((pReport.OutageDetails[0].Outage_Date_OFF != pReport.OutageDetails[0].Outage_Date_ON) ? pReport.OutageDetails[0].Outage_Date_ON : string.Empty)*/ + pReport.OutageDetails[0].ON_Time;
                //sTemp += (pReport.OutageDetails.Count > 1) ? " and\n                    " + pReport.OutageDetails[1].Outage_Date_OFF + " from " + pReport.OutageDetails[1].OFF_Time + " to " /*+ ((pReport.OutageDetails[1].Outage_Date_OFF != pReport.OutageDetails[1].Outage_Date_ON) ? pReport.OutageDetails[1].Outage_Date_ON : string.Empty)*/ + pReport.OutageDetails[1].ON_Time : string.Empty;
                //sTemp += ", weather permitting.";
                ////phrase.Add(new iTextSharp.text.Phrase(sTemp, fontNormal11));
                
                //doc.Add(new iTextSharp.text.Phrase(sTemp, fontNormal7_1));
                //doc.Add(paragraph);
                //phrase = new iTextSharp.text.Phrase("Interruption area: ", fontBold8);
                //phrase.Font.SetStyle(Font.BOLD | Font.UNDERLINE);
                //doc.Add(phrase);
                ////phrase.Add(new iTextSharp.text.Phrase(pReport.Description, fontNormal11));
                //doc.Add(new iTextSharp.text.Phrase(pReport.Description, fontNormal7_1));
                //paragraph = new iTextSharp.text.Paragraph();
                //doc.Add(paragraph);


                //fontBold8.SetStyle(Font.BOLD);
                //PdfPCell pcell = new PdfPCell(new Phrase("Customer Notification List", fontBold8));
                //pcell.Colspan = 6;
                //pcell.BorderColor = BaseColor.WHITE;
                //pcell.Border = 0;
                //pcell.HorizontalAlignment = 1;
                //ptable.DefaultCell.Border = 0;
                //ptable.DefaultCell.BorderColor = BaseColor.WHITE;
                //ptable.AddCell(pcell);
                //pcell = new PdfPCell(new Phrase(DateTime.Today.ToShortDateString(), fontNormal7_1));
                //pcell.Colspan = 1;
                //pcell.BorderColor = BaseColor.WHITE;
                //pcell.Border = 0;
                //pcell.HorizontalAlignment = 1;
                //ptable.DefaultCell.Border = 0;
                //ptable.DefaultCell.BorderColor = BaseColor.WHITE;
                //ptable.AddCell(pcell);

                //pcell = new PdfPCell(new Phrase(string.Empty, fontBold7));
                //pcell.BorderColor = BaseColor.WHITE;
                //pcell.Colspan = 7;
                //ptable.AddCell(pcell);

                //ptable.AddCell(new Phrase("Type", fontBold7));
                //ptable.AddCell(new Phrase("Customer Name", fontBold7));
                //ptable.AddCell(new Phrase("Address", fontBold7));
                //ptable.AddCell(new Phrase("City, State, ZIP", fontBold7));
                //ptable.AddCell(new Phrase("CGC/TNum", fontBold7));
                //ptable.AddCell(new Phrase("SSD", fontBold7));
                //ptable.AddCell(new Phrase("Phone", fontBold7));

                //pcell = new PdfPCell(new Phrase(string.Empty, fontNormal7_2));
                //pcell.Colspan = 7;
                //ptable.AddCell(pcell);
                #endregion

                foreach (Customer customer in pReport.AffectedCustomers.Where(c => !c.Excluded).OrderBy(c => c.SSD, new CustomComparer())
                                                                                .ThenBy(c => c.CGC12)
                                                                                .ThenBy(c => (c.CustomerAddress.StreetName1 + c.CustomerAddress.StreetName2))
                                                                                .ThenBy(c => c.CustomerAddress.StreetNumber)
                                                                                .ThenBy(c => c.ServicePointID))
                {
                    if (customer.Excluded) continue;                    
                    ptable.DefaultCell.Border = 0;
                    ptable.DefaultCell.BorderColor = BaseColor.WHITE;
                    ptable.AddCell(new Phrase(customer.CustomerType, font_Arial8));
                    ptable.AddCell(new Phrase(customer.CustomerName.MailName1 + (customer.CustomerName.MailName1.Length > 0 ? " " : string.Empty) + customer.CustomerName.MailName2, font_Arial8));
                    if (!string.IsNullOrEmpty(customer.MailAddress.City))
                    {
                        ptable.AddCell(new Phrase(customer.MailAddress.StreetNumber + (customer.MailAddress.StreetNumber.Length > 0 ? " " : string.Empty) + customer.MailAddress.StreetName1 + ((customer.MailAddress.StreetNumber + customer.MailAddress.StreetName1).Length > 0 ? (customer.MailAddress.StreetName2.Length > 0 ? ", " : string.Empty) : string.Empty + customer.MailAddress.StreetName2), font_Arial8));
                        ptable.AddCell(new Phrase(customer.MailAddress.City + (customer.MailAddress.City.Length > 0 ? ", " : string.Empty) + customer.MailAddress.State + ((customer.MailAddress.City + customer.MailAddress.State).Length > 0 ? " " : string.Empty) + customer.MailAddress.ZIPCode, font_Arial8));
                    }
                    else
                    {
                        ptable.AddCell(new Phrase(customer.CustomerAddress.StreetNumber + (customer.CustomerAddress.StreetNumber.Length > 0 ? " " : string.Empty) + customer.CustomerAddress.StreetName1 + ((customer.CustomerAddress.StreetNumber + customer.CustomerAddress.StreetName1).Length > 0 ? (customer.CustomerAddress.StreetName2.Length > 0 ? ", " : string.Empty) : string.Empty + customer.CustomerAddress.StreetName2), font_Arial8));
                        ptable.AddCell(new Phrase(customer.CustomerAddress.City + (customer.CustomerAddress.City.Length > 0 ? ", " : string.Empty) + customer.CustomerAddress.State + ((customer.CustomerAddress.City + customer.CustomerAddress.State).Length > 0 ? " " : string.Empty) + customer.CustomerAddress.ZIPCode, font_Arial8));
                    }

                    ptable.AddCell(new Phrase(customer.CGC12, font_Arial8));
                    ptable.AddCell(new Phrase(customer.SSD, font_Arial8));
                    ptable.AddCell(new Phrase(customer.PhoneNumber.AreaCode + (customer.PhoneNumber.AreaCode.Length > 0 ? " " : string.Empty) + customer.PhoneNumber.PhoneNumber, font_Arial8));
                }
                ptable.WidthPercentage = 100;

                doc.Add(new iTextSharp.text.Paragraph());
                doc.Add(new iTextSharp.text.Paragraph());

                doc.Add(ptable);
                doc.Close();

                return sPath;
            }
            catch (Exception ex)
            {
                _logger.Error("Error Creating PDF " + ex.Message);
                return string.Empty;
            }
        }

        public static string CreateTXT(CustomerReport pReport)
        {
            try
            {
                string sPath = GetPath("txt");

                StreamWriter pSWriter = File.CreateText(sPath);
                string sTemp = "Interruption date: " + pReport.OutageDetails[0].Outage_Date_OFF + " from " + pReport.OutageDetails[0].OFF_Time + " to " /*+ ((pReport.OutageDetails[0].Outage_Date_OFF != pReport.OutageDetails[0].Outage_Date_ON) ? pReport.OutageDetails[0].Outage_Date_ON : string.Empty)*/ + pReport.OutageDetails[0].ON_Time;
                sTemp += (pReport.OutageDetails.Count > 1) ? " and\n                   " + pReport.OutageDetails[1].Outage_Date_OFF + " from " + pReport.OutageDetails[1].OFF_Time + " to " /*+ ((pReport.OutageDetails[1].Outage_Date_OFF != pReport.OutageDetails[1].Outage_Date_ON) ? pReport.OutageDetails[1].Outage_Date_ON : string.Empty)*/ + pReport.OutageDetails[1].ON_Time : string.Empty;
                sTemp += ", weather permitting."; 
                pSWriter.WriteLine(sTemp);
                pSWriter.WriteLine("Interruption area: " + pReport.Description);

                DataTable pDTable = new DataTable("CUSTOMERLIST");
                pDTable.Columns.Add("Type");
                pDTable.Columns.Add("Customer Name");
                pDTable.Columns.Add("Address");
                pDTable.Columns.Add("City, State, ZIP");
                pDTable.Columns.Add("CGC/TNUM");
                pDTable.Columns.Add("SSD");
                pDTable.Columns.Add("Phone");
                foreach (Customer customer in pReport.AffectedCustomers.Where(c => !c.Excluded).OrderBy(c => c.SSD, new CustomComparer())
                                                                                .ThenBy(c => c.CGC12)
                                                                                .ThenBy(c => (c.CustomerAddress.StreetName1 + c.CustomerAddress.StreetName2))
                                                                                .ThenBy(c => c.CustomerAddress.StreetNumber)
                                                                                .ThenBy(c => c.ServicePointID)
                                                                                .ThenBy(c => !string.IsNullOrEmpty(c.MailAddress.City)))
                {
                    if (customer.Excluded) continue;
                    DataRow pRow = pDTable.NewRow();
                    pRow[0] = customer.CustomerType;
                    pRow[1] = customer.CustomerName.MailName1 + (String.IsNullOrEmpty(customer.CustomerName.MailName1) ? string.Empty : " ") + customer.CustomerName.MailName2;
                    if (!string.IsNullOrEmpty(customer.MailAddress.City))
                    {
                        pRow[2] = customer.MailAddress.StreetNumber + (String.IsNullOrEmpty(customer.MailAddress.StreetNumber) ? string.Empty : " ") + customer.MailAddress.StreetName1 + (String.IsNullOrEmpty(customer.MailAddress.StreetNumber + customer.MailAddress.StreetName1) ? string.Empty : (String.IsNullOrEmpty(customer.MailAddress.StreetName2) ? string.Empty : ", ")) + customer.MailAddress.StreetName2;
                        pRow[3] = customer.MailAddress.City + (String.IsNullOrEmpty(customer.MailAddress.City) ? string.Empty : ", ") + customer.MailAddress.State + (String.IsNullOrEmpty(customer.MailAddress.City + customer.MailAddress.State) ? string.Empty : " ") + customer.MailAddress.ZIPCode;
                    }
                    else
                    {
                        pRow[2] = customer.CustomerAddress.StreetNumber + (String.IsNullOrEmpty(customer.CustomerAddress.StreetNumber) ? string.Empty : " ") + customer.CustomerAddress.StreetName1 + (String.IsNullOrEmpty(customer.CustomerAddress.StreetNumber + customer.CustomerAddress.StreetName1) ? string.Empty : (String.IsNullOrEmpty(customer.CustomerAddress.StreetName2) ? string.Empty : ", ")) + customer.CustomerAddress.StreetName2;
                        pRow[3] = customer.CustomerAddress.City + (String.IsNullOrEmpty(customer.CustomerAddress.City) ? string.Empty : ", ") + customer.CustomerAddress.State + (String.IsNullOrEmpty(customer.CustomerAddress.City + customer.CustomerAddress.State) ? string.Empty : " ") + customer.CustomerAddress.ZIPCode;
                    }
                   
                    pRow[4] = customer.CGC12;
                    pRow[5] = customer.SSD;
                    pRow[6] = customer.PhoneNumber.AreaCode + (String.IsNullOrEmpty(Convert.ToString(pRow[6])) ? string.Empty : " ") + customer.PhoneNumber.PhoneNumber;
                    pDTable.Rows.Add(pRow);
                }

                List<int> maximumLengthForColumns =
                    Enumerable.Range(0, pDTable.Columns.Count).
                    Select(col => pDTable.AsEnumerable().
                        Select(row => row[col]).OfType<string>().
                        Max(val => val.Length)).ToList();
                for (int iLength = 0; iLength < maximumLengthForColumns.Count; ++iLength)
                {
                    if (maximumLengthForColumns[iLength] == 0)
                        maximumLengthForColumns[iLength] += pDTable.Columns[iLength].ColumnName.Length;
                    maximumLengthForColumns[iLength] += 5;
                }


                int iTotalLenght = 0;
                pSWriter.WriteLine();
                for (int iLength = 0; iLength < maximumLengthForColumns.Count; ++iLength)
                    iTotalLenght += maximumLengthForColumns[iLength];

                string sHeaderTable = string.Empty;
                for (int i = 0; i < Convert.ToInt32(iTotalLenght / 2); ++i)
                {
                    sHeaderTable += " ";
                }
                sHeaderTable += "Customer Notification List";
                for (int i = 0; i < iTotalLenght - sHeaderTable.Length - 10; ++i)
                {
                    sHeaderTable += " ";
                }
                sHeaderTable += DateTime.Today.ToString("MM/dd/yyyy");
                int ilength = 0;
                pSWriter.WriteLine(sHeaderTable);
                pSWriter.WriteLine();
                string col_detail = string.Empty;
                for (int i = 0; i < pDTable.Columns.Count; ++i)
                {
                    col_detail += pDTable.Columns[i].ColumnName;
                    for (int j = 0; j < maximumLengthForColumns[i] - Convert.ToString(pDTable.Columns[i].ColumnName).Length; ++j)
                    {
                        col_detail += " ";
                    }
                    ++ilength;
                }

                pSWriter.WriteLine(col_detail);

                for (int i = 0; i < iTotalLenght; ++i)
                    pSWriter.Write("_");
                pSWriter.WriteLine();
                pSWriter.WriteLine();
                foreach (DataRow row in pDTable.Rows)
                {
                    string cust_detail = string.Empty;
                    for (int i = 0; i < pDTable.Columns.Count; ++i)
                    {
                        cust_detail += row[i];
                        for (int j = 0; j < maximumLengthForColumns[i] - Convert.ToString(row[i]).Length; ++j)
                        {
                            cust_detail += " ";
                        }
                    }
                    pSWriter.WriteLine(cust_detail);
                }
                pSWriter.Close();
                return sPath;
            }
            catch (Exception ex)
            {
                _logger.Error("Error creating Text " + ex.Message);
                return string.Empty;
            }
        }
        
        public static string CreateEXL(CustomerReport pReport)
        {
            try
            {
                string sPath = GetPath("xlsx");

                XLWorkbook xlw = new XLWorkbook();

                IXLWorksheet xls = xlw.AddWorksheet("CustomerList");
                IXLCell xlc = xls.Cell(1, 1);
                xls.Range("A1:B1").Merge();
                xlc.Value = "Interruption date:";
                xlc.RichText.Underline = XLFontUnderlineValues.Single;
                xlc.RichText.Bold = true;
                xlc = xls.Cell(1, 3);
                //xls.Range("C1:" + /*((pReport.OutageDetails[0].Outage_Date_OFF != pReport.OutageDetails[0].Outage_Date_ON) ? "E1" : "I1")*/"I1").Merge();
                xls.Range("C1:" + (pReport.OutageDetails.Count > 1 ? "M1" : "I1")).Merge();
                string sTemp = "Interruption date: " + pReport.OutageDetails[0].Outage_Date_OFF + " from " + pReport.OutageDetails[0].OFF_Time + " to " /*+ ((pReport.OutageDetails[0].Outage_Date_OFF != pReport.OutageDetails[0].Outage_Date_ON) ? pReport.OutageDetails[0].Outage_Date_ON : string.Empty)*/ + pReport.OutageDetails[0].ON_Time;
                sTemp += (pReport.OutageDetails.Count > 1) ? " and " + pReport.OutageDetails[1].Outage_Date_OFF + " from " + pReport.OutageDetails[1].OFF_Time + " to " /*+ ((pReport.OutageDetails[1].Outage_Date_OFF != pReport.OutageDetails[1].Outage_Date_ON) ? pReport.OutageDetails[1].Outage_Date_ON : string.Empty)*/ + pReport.OutageDetails[1].ON_Time : string.Empty;
                sTemp += ", weather permitting."; 
                xlc.Value = sTemp;
                xls.Range("A2:B2").Merge();
                xlc = xls.Cell(2, 1);
                xlc.Value = "Interruption area:";
                xlc.RichText.Underline = XLFontUnderlineValues.Single;
                xlc.RichText.Bold = true;
                xlc = xls.Cell(2, 3);
                xls.Range("C2:E2").Merge();
                xlc.Value = pReport.Description;
                xls.Range("A4:F4").Merge();
                xlc = xls.Cell(4, 1);
                xlc.Value = "Customer Notification List";
                xls.Range("A4:F4").Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                xlc = xls.Cell(4, 7);
                xlc.Value = DateTime.Today.ToShortDateString();

                DataTable pDTable = new DataTable("CUSTOMERLIST");
                pDTable.Columns.Add("Type");
                pDTable.Columns.Add("Customer Name");
                pDTable.Columns.Add("Address");
                pDTable.Columns.Add("City, State, ZIP");
                pDTable.Columns.Add("CGC/TNUM");
                pDTable.Columns.Add("SSD");
                pDTable.Columns.Add("Phone");
                foreach (Customer customer in pReport.AffectedCustomers.Where(c => !c.Excluded).OrderBy(c => c.SSD, new CustomComparer())
                                                                                                .ThenBy(c => c.CGC12)
                                                                                                .ThenBy(c => (c.CustomerAddress.StreetName1 + c.CustomerAddress.StreetName2))
                                                                                                .ThenBy(c => c.CustomerAddress.StreetNumber)
                                                                                                .ThenBy(c => c.ServicePointID))
                {
                    if (customer.Excluded) continue;
                    DataRow pRow = pDTable.NewRow();
                    pRow[0] = customer.CustomerType;
                    pRow[1] = customer.CustomerName.MailName1 + (String.IsNullOrEmpty(customer.CustomerName.MailName1) ? string.Empty : " ") + customer.CustomerName.MailName2;
                    if (!string.IsNullOrEmpty(customer.MailAddress.City))
                    {
                        pRow[2] = customer.MailAddress.StreetNumber + (String.IsNullOrEmpty(customer.MailAddress.StreetNumber) ? string.Empty : " ") + customer.MailAddress.StreetName1 + (String.IsNullOrEmpty(customer.MailAddress.StreetNumber + customer.MailAddress.StreetName1) ? string.Empty : (String.IsNullOrEmpty(customer.MailAddress.StreetName2) ? string.Empty : ", ")) + customer.MailAddress.StreetName2;
                        pRow[3] = customer.MailAddress.City + (String.IsNullOrEmpty(customer.MailAddress.City) ? string.Empty : ", ") + customer.MailAddress.State + (String.IsNullOrEmpty(customer.MailAddress.City + customer.MailAddress.State) ? string.Empty : " ") + customer.MailAddress.ZIPCode;
                    }
                    else
                    {
                        pRow[2] = customer.CustomerAddress.StreetNumber + (String.IsNullOrEmpty(customer.CustomerAddress.StreetNumber) ? string.Empty : " ") + customer.CustomerAddress.StreetName1 + (String.IsNullOrEmpty(customer.CustomerAddress.StreetNumber + customer.CustomerAddress.StreetName1) ? string.Empty : (String.IsNullOrEmpty(customer.CustomerAddress.StreetName2) ? string.Empty : ", ")) + customer.CustomerAddress.StreetName2;
                        pRow[3] = customer.CustomerAddress.City + (String.IsNullOrEmpty(customer.CustomerAddress.City) ? string.Empty : ", ") + customer.CustomerAddress.State + (String.IsNullOrEmpty(customer.CustomerAddress.City + customer.CustomerAddress.State) ? string.Empty : " ") + customer.CustomerAddress.ZIPCode;
                    }

                    pRow[4] = customer.CGC12;
                    pRow[5] = customer.SSD;
                    pRow[6] = customer.PhoneNumber.AreaCode + (String.IsNullOrEmpty(customer.PhoneNumber.AreaCode) ? string.Empty : " ") + customer.PhoneNumber.PhoneNumber;
                    pDTable.Rows.Add(pRow);
                }

                xlc = xls.Cell(5, 1);
                xlc.InsertTable(pDTable);

                xlw.SaveAs(sPath);
                pDTable.Clear();

                return sPath;
            }
            catch (Exception ex)
            {
                _logger.Error("Error creating spreadsheet " + ex.Message);
                return string.Empty;
            }
        }

        public static string CreateLBL(CustomerReport pReport, int countinput)
        {
            try
            {
                string sPath = GetPath("xps");

                DataTable pDTable = new DataTable("CUSTOMERLIST");
                pDTable.Columns.Add("Name");
                pDTable.Columns.Add("Address");
                pDTable.Columns.Add("City, State, ZIP");
                for (int i = 0; i < countinput; ++i)
                {
                    DataRow pRow = pDTable.NewRow();
                    pRow[0] = string.Empty;
                    pRow[1] = string.Empty;
                    pRow[2] = string.Empty;
                    pDTable.Rows.Add(pRow);
                }
                foreach (Customer customer in pReport.AffectedCustomers)
                {
                    if (customer.Excluded) continue;
                    DataRow pRow = pDTable.NewRow();
                    string sNameSeperator = " ";
                    if (customer.CustomerName.MailName1.Contains(',') || customer.CustomerName.MailName2.Contains(',') || customer.CustomerName.MailName2.StartsWith("&"))
                        sNameSeperator = "^";
                    pRow[0] = customer.CustomerName.MailName1 + (String.IsNullOrEmpty(customer.CustomerName.MailName1) ? string.Empty : sNameSeperator) + customer.CustomerName.MailName2;
                    if (!string.IsNullOrEmpty(customer.MailAddress.City))
                    {
                        pRow[1] = customer.MailAddress.StreetNumber + (String.IsNullOrEmpty(customer.MailAddress.StreetNumber) ? string.Empty : " ") + customer.MailAddress.StreetName1 + (String.IsNullOrEmpty(customer.MailAddress.StreetNumber + customer.MailAddress.StreetName1) ? string.Empty : ", ") + customer.MailAddress.StreetName2;
                        pRow[2] = customer.MailAddress.City + (String.IsNullOrEmpty(customer.MailAddress.City) ? string.Empty : ", ") + customer.MailAddress.State + (String.IsNullOrEmpty(customer.MailAddress.City + customer.MailAddress.State) ? string.Empty : " ") + customer.MailAddress.ZIPCode;
                    }
                    else
                    {
                        pRow[1] = customer.CustomerAddress.StreetNumber + (String.IsNullOrEmpty(customer.CustomerAddress.StreetNumber) ? string.Empty : " ") + customer.CustomerAddress.StreetName1 + (String.IsNullOrEmpty(customer.CustomerAddress.StreetNumber + customer.CustomerAddress.StreetName1) ? string.Empty : ", ") + customer.CustomerAddress.StreetName2;
                        pRow[2] = customer.CustomerAddress.City + (String.IsNullOrEmpty(customer.CustomerAddress.City) ? string.Empty : ", ") + customer.CustomerAddress.State + (String.IsNullOrEmpty(customer.CustomerAddress.City + customer.CustomerAddress.State) ? string.Empty : " ") + customer.CustomerAddress.ZIPCode;
                    }
                    pDTable.Rows.Add(pRow);
                }


                XpsDocument xpsDoc_Avery = new XpsDocument(sPath, FileAccess.ReadWrite);
                XpsDocument.CreateXpsDocumentWriter(xpsDoc_Avery).
                    Write(((IDocumentPaginatorSource)(new Avery5160()).
                    CreateDocument(pDTable)).
                    DocumentPaginator);
                xpsDoc_Avery.Close();

                #region leave commented
                //SerializerProvider serializerProvider = new SerializerProvider();
                //// Locate the serializer that matches the fileName extension.
                //SerializerDescriptor selectedPlugIn = null;
                //foreach (SerializerDescriptor serializerDescriptor in
                //                serializerProvider.InstalledSerializers)
                //{
                //    if (serializerDescriptor.IsLoadable &&
                //         sPath.EndsWith(serializerDescriptor.DefaultFileExtension))
                //    {   // The plug-in serializer and fileName extensions match.
                //        selectedPlugIn = serializerDescriptor;
                //        break; // foreach
                //    }
                //}

                //// If a match for a plug-in serializer was found,
                //// use it to output and store the document.
                //if (selectedPlugIn != null)
                //{
                //    Stream package = File.Create(sPath);
                //    SerializerWriter serializerWriter =
                //        serializerProvider.CreateSerializerWriter(selectedPlugIn,
                //                                                  package);

                //    serializerWriter.Write(((IDocumentPaginatorSource)(new Avery5160()).CreateDocument(pDTable)).DocumentPaginator, null);
                //    package.Close();
                //}
                #endregion

                return sPath;
            }
            catch (Exception ex)
            {
                _logger.Error("Error creating labels " + ex.Message);
                return string.Empty;
            }
        }

        public static string GetPath(string sModule)
        {
            string sPath = string.Empty; ; try
            {
                sPath = (Directory.Exists(@"P:")) ? @"P:\" : Path.GetTempPath();

                string[] sFiles = Directory.GetFiles(sPath, "PONSReport_*" + sModule);
                foreach (string sFile in sFiles)
                {
                    try
                    {
                        File.Delete(sFile);
                    }
                    catch { }
                }
                sPath += "PONSReport_" + DateTime.Now.Ticks + "." + sModule;
            }
            catch (Exception ex)
            {
                _logger.Error("Error getting temp path " + ex.Message);
            }
            _logger.Info("Error temp file path " + sPath);
            return sPath;
        }

        public static DataSet PSCNameList()
        {
            try
            {
                SqlConnection conn = new SqlConnection();
                conn.ConnectionString = utilityFunctions.ReadConfigurationValue("PSL_DB_STRING");// "Server = ASCdev01;Database = BPM_ShutdownLtrs;User Id= CustInfo_Load;Password=40dc-a68f-5e74;";
                string query = "select FName, LName, LANID, ContactID, Division from tblContacts where type = " + "'PSC'" + " and Active = 1 order by Division, FName, LName";
                SqlDataAdapter da = new SqlDataAdapter(query, conn);
                conn.Open();
                DataSet ds = new DataSet();
                da.Fill(ds, "tblContacts");
                conn.Close();
                return ds;
            }
            catch (Exception ex)
            {
                _logger.Error("Error getting PSC Name List " + ex.Message);
                return null;
            }
        }
    }

    public class CustomComparer : IComparer<string>
    {
        public int Compare(string x, string y)
        {
            string s1 = x as string;
            if (s1 == null)
            {
                return 0;
            }
            string s2 = y as string;
            if (s2 == null)
            {
                return 0;
            }
 
            int len1 = s1.Length;
            int len2 = s2.Length;
            int marker1 = 0;
            int marker2 = 0;
 
            // Walk through two the strings with two markers.
            while (marker1 < len1 && marker2 < len2)
            {
                char ch1 = s1[marker1];
                char ch2 = s2[marker2];
 
                // Some buffers we can build up characters in for each chunk.
                char[] space1 = new char[len1];
                int loc1 = 0;
                char[] space2 = new char[len2];
                int loc2 = 0;
 
                // Walk through all following characters that are digits or
                // characters in BOTH strings starting at the appropriate marker.
                // Collect char arrays.
                do
                {
                    space1[loc1++] = ch1;
                    marker1++;
 
                    if (marker1 < len1)
                    {
                        ch1 = s1[marker1];
                    }
                    else
                    {
                        break;
                    }
                } while (char.IsDigit(ch1) == char.IsDigit(space1[0]));
 
                do
                {
                    space2[loc2++] = ch2;
                    marker2++;
 
                    if (marker2 < len2)
                    {
                        ch2 = s2[marker2];
                    }
                    else
                    {
                        break;
                    }
                } while (char.IsDigit(ch2) == char.IsDigit(space2[0]));
 
                // If we have collected numbers, compare them numerically.
                // Otherwise, if we have strings, compare them alphabetically.
                string str1 = new string(space1);
                string str2 = new string(space2);
 
                int result;
 
                if (char.IsDigit(space1[0]) && char.IsDigit(space2[0]))
                {
                    int thisNumericChunk = int.Parse(str1);
                    int thatNumericChunk = int.Parse(str2);
                    result = thisNumericChunk.CompareTo(thatNumericChunk);
                }
                else
                {
                    result = str1.CompareTo(str2);
                }
 
                if (result != 0)
                {
                    return result;
                }
            }
            return len1 - len2;
        }   

        //public int Compare(string x, string y)
        //{
        //    double xNum, yNum;
        //    bool xIsNum = Double.TryParse(x, out xNum);
        //    bool yIsNum = Double.TryParse(y, out yNum);

        //    // compare numbers
        //    if (xIsNum && yIsNum)
        //    {
        //        return xNum.CompareTo(yNum);
        //    }

        //    // compare num to string
        //    if (xIsNum)
        //    {
        //        return 1;
        //    }

        //    // compare num to string
        //    if (yIsNum)
        //    {
        //        return -1;
        //    }

        //    // compare as strings
        //    return x.CompareTo(y);
        //}
    }

    public class PDFPageEvent : PdfPageEventHelper
    {
        CustomerReport pReport = new CustomerReport();

        public PDFPageEvent(CustomerReport _Report)
        {
            pReport = _Report;
        }
        public override void OnStartPage(PdfWriter writer, Document doc)
        {
            if (!String.IsNullOrEmpty(pReport.ReportID))
            {
                Header header = new Header("H1", string.Empty);
                header.Append("ShutDown Reference ID: " + pReport.ReportID);
                doc.Add(header);
            }
            //FontFactory.RegisterDirectories();
            Font font_Arial11Bold = FontFactory.GetFont("arial", 11, Font.BOLD),
            font_Arial11Normal = FontFactory.GetFont("arial", 11, Font.NORMAL),
            //fontNormal7_2 = FontFactory.GetFont("arial", 8, Font.NORMAL),
            font_Arial8Normal = FontFactory.GetFont("arial", 8, Font.NORMAL);

            string sTemp = "Interruption date: ";

            iTextSharp.text.Paragraph paragraph = new iTextSharp.text.Paragraph();//sTemp, fontBold11);
            iTextSharp.text.Phrase phrase = new iTextSharp.text.Phrase(sTemp, font_Arial11Bold);

            phrase.Font.SetStyle(Font.BOLD | Font.UNDERLINE);
            doc.Add(phrase);

            sTemp = string.Empty + pReport.OutageDetails[0].Outage_Date_OFF + " from " + pReport.OutageDetails[0].OFF_Time + " to " /*+ ((pReport.OutageDetails[0].Outage_Date_OFF != pReport.OutageDetails[0].Outage_Date_ON) ? pReport.OutageDetails[0].Outage_Date_ON : string.Empty)*/ + pReport.OutageDetails[0].ON_Time;
            sTemp += (pReport.OutageDetails.Count > 1) ? " and\n                                   " + pReport.OutageDetails[1].Outage_Date_OFF + " from " + pReport.OutageDetails[1].OFF_Time + " to " /*+ ((pReport.OutageDetails[1].Outage_Date_OFF != pReport.OutageDetails[1].Outage_Date_ON) ? pReport.OutageDetails[1].Outage_Date_ON : string.Empty)*/ + pReport.OutageDetails[1].ON_Time : string.Empty;
            sTemp += ", weather permitting.";
            //phrase.Add(new iTextSharp.text.Phrase(sTemp, fontNormal11));

            doc.Add(new iTextSharp.text.Phrase(sTemp, font_Arial11Normal));
            doc.Add(paragraph);
            phrase = new iTextSharp.text.Phrase("Interruption area: ", font_Arial11Bold);
            phrase.Font.SetStyle(Font.BOLD | Font.UNDERLINE);
            doc.Add(phrase);
            //phrase.Add(new iTextSharp.text.Phrase(pReport.Description, fontNormal11));
            doc.Add(new iTextSharp.text.Phrase(pReport.Description, font_Arial11Normal));
            paragraph = new iTextSharp.text.Paragraph();
            doc.Add(paragraph);

            PdfPTable ptable = new PdfPTable(7);
            float[] widths = new float[] { 25f, 100f, 150f, 85f, 50f, 50f, 50f };
            ptable.SetWidths(widths);
            ptable.WidthPercentage = 100;
            ptable.DefaultCell.Border = 0;
            ptable.DefaultCell.BorderColor = BaseColor.WHITE;
            font_Arial11Bold.SetStyle(Font.BOLD);
            PdfPCell pcell = new PdfPCell(new Phrase("Customer Notification List", font_Arial11Bold));
            pcell.Colspan = 6;
            pcell.BorderColor = BaseColor.WHITE;
            pcell.Border = 0;
            pcell.HorizontalAlignment = 1;
            ptable.DefaultCell.Border = 0;
            ptable.DefaultCell.BorderColor = BaseColor.WHITE;
            ptable.AddCell(pcell);
            pcell = new PdfPCell(new Phrase(DateTime.Today.ToShortDateString(), font_Arial11Normal));
            pcell.Colspan = 1;
            pcell.BorderColor = BaseColor.WHITE;
            pcell.Border = 0;
            pcell.HorizontalAlignment = 1;
            ptable.DefaultCell.Border = 0;
            ptable.DefaultCell.BorderColor = BaseColor.WHITE;
            ptable.AddCell(pcell);

            pcell = new PdfPCell(new Phrase(string.Empty, font_Arial8Normal));
            pcell.BorderColor = BaseColor.WHITE;
            pcell.Colspan = 7;
            ptable.AddCell(pcell);

            ptable.AddCell(new Phrase("Type", font_Arial8Normal));
            ptable.AddCell(new Phrase("Customer Name", font_Arial8Normal));
            ptable.AddCell(new Phrase("Address", font_Arial8Normal));
            ptable.AddCell(new Phrase("City, State, ZIP", font_Arial8Normal));
            ptable.AddCell(new Phrase("CGC/TNum", font_Arial8Normal));
            ptable.AddCell(new Phrase("SSD", font_Arial8Normal));
            ptable.AddCell(new Phrase("Phone", font_Arial8Normal));

            pcell = new PdfPCell(new Phrase(string.Empty, font_Arial8Normal));
            pcell.Colspan = 7;
            ptable.AddCell(pcell);
            doc.Add(ptable);
        }

        // This is the contentbyte object of the writer
        PdfContentByte cb;
        // we will put the final number of pages in a template
        PdfTemplate template;
        // this is the BaseFont we are going to use for the header / footer
        BaseFont bf = null;
        // This keeps track of the creation time
        DateTime PrintTime = DateTime.Now;
        #region Properties
        private string _Title;
        public string Title
        {
            get { return _Title; }
            set { _Title = value; }
        }

        private string _HeaderLeft;
        public string HeaderLeft
        {
            get { return _HeaderLeft; }
            set { _HeaderLeft = value; }
        }
        private string _HeaderRight;
        public string HeaderRight
        {
            get { return _HeaderRight; }
            set { _HeaderRight = value; }
        }
        private Font _HeaderFont;
        public Font HeaderFont
        {
            get { return _HeaderFont; }
            set { _HeaderFont = value; }
        }
        private Font _FooterFont;
        public Font FooterFont
        {
            get { return _FooterFont; }
            set { _FooterFont = value; }
        }
        #endregion

        public override void OnOpenDocument(PdfWriter writer, Document document)
        {
            try
            {
                PrintTime = DateTime.Now;

                //bf = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);
                Font font_Arial10 = FontFactory.GetFont("arial", 10, Font.NORMAL);
                bf = font_Arial10.GetCalculatedBaseFont(false);
                cb = writer.DirectContent;
                template = cb.CreateTemplate(50, 50);
            }
            catch (DocumentException de)
            {
            }
            catch (System.IO.IOException ioe)
            {
            }
        }

        public override void OnEndPage(PdfWriter writer, Document document)
        {
            Font font_Arial10 = FontFactory.GetFont("arial", 10, Font.NORMAL);
            bf = font_Arial10.GetCalculatedBaseFont(false);
            base.OnEndPage(writer, document);
            int pageN = writer.PageNumber;
            String text = "Page " + pageN + " of ";
            float len = bf.GetWidthPoint(text, 10);
            Rectangle pageSize = document.PageSize;
            cb.SetRGBColorFill(0, 0, 0);
            cb.BeginText();
            cb.SetFontAndSize(bf, 10);
            cb.SetTextMatrix(pageSize.GetLeft(355), pageSize.GetBottom(15));
            cb.ShowText(text);
            cb.EndText();
            cb.AddTemplate(template, pageSize.GetLeft(355) + len, pageSize.GetBottom(15));

            //cb.BeginText();
            //cb.SetFontAndSize(bf, 8);
            //cb.ShowTextAligned(PdfContentByte.ALIGN_RIGHT,
            //    "Printed On " + PrintTime.ToString(),
            //    pageSize.GetRight(40),
            //    pageSize.GetBottom(30), 0);
            //cb.EndText();
        }
        public override void OnCloseDocument(PdfWriter writer, Document document)
        {
            base.OnCloseDocument(writer, document);
            template.BeginText();
            template.SetFontAndSize(bf, 8);
            template.SetTextMatrix(0, 0);
            template.ShowText("" + (writer.PageNumber - 1));
            template.EndText();
        }
    }
}