using System;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Reflection;
using System.Text;
using Oracle.DataAccess.Client;
using PGE_DBPasswordManagement;


namespace PGE.BatchApplication.SupervisoryControlEmail
{
    class Program
    {
        private static string sPath = (String.IsNullOrEmpty(ConfigurationManager.AppSettings["LOG_PATH"]) ? System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) : ConfigurationManager.AppSettings["LOGPATH"]) + "\\Logfile_" + DateTime.Now.Ticks.ToString() + ".log";
        private static StreamWriter pSWriter = default(StreamWriter);
        private static DataTable pDTFinalServer = new DataTable();
        static void Main(string[] args)
        {
            (pSWriter = File.CreateText(sPath)).Close();
            WriteLine("Starting");
            OracleConnection pOConnection = new OracleConnection();            
            try
            {
                // m4jf edgisrearc 919 - updated connection string - get connection string using Passwordmanagement tool.
                // pOConnection = new OracleConnection(ConfigurationManager.AppSettings["CONNECTIONSTRING"]);
                pOConnection = new OracleConnection(ReadEncryption.GetConnectionStr(ConfigurationManager.AppSettings["EDER_ConnectionStr"].ToUpper()));
                pOConnection.Open();
                WriteLine("Oracle Connection Opened");
                
                if (SendAutomatedEmail(ExportDatatableToHtml(getDataTableFinal(pOConnection))))
                    WriteToServer(pOConnection, pDTFinalServer);
                WriteLine("Completed");
            }
            catch (Exception ex)
            {
                WriteLine("Main Exception: " + ex.Message);
            }
            finally
            {
                pOConnection.Close();
                WriteLine("Oracle Connection Closed");
            }
        }

        private static DataTable getDataTableFinal(OracleConnection pOConnection)
        {
            DataTable pDTFinal = new DataTable("FINAL"),
                pDTEmailTable = new DataTable("PGE_SVCEMAIL"),
                pDTSVCRecord = new DataTable("PGE_SVCRECORDS");            
            try
            {
                WriteLine("Getting Datatable for existing records");
                pDTEmailTable = getDataTable(pOConnection, File.ReadAllText("SVCEmail.sql"), "PGE_SVCEMAIL");


                var maxModifiedOn = Convert.ToDateTime(pDTEmailTable.AsEnumerable().Max(x => x["MODIFIEDON"]).ToString()).ToString("MM/dd/yyyy");

                WriteLine("Max modified date on:" + maxModifiedOn.ToString());

                WriteLine("Setting Default Version");
                Execute(pOConnection, "EXEC sde.version_util.set_current_version('SDE.DEFAULT')");
                WriteLine("Getting Datable for new records based on last emailed date");
                pDTSVCRecord = getDataTable(pOConnection, File.ReadAllText("SVCRecords.sql").Replace("DATEVARIABLE", maxModifiedOn), "PGE_SVCRECORDS");

                pDTFinal = pDTSVCRecord.Clone();

                WriteLine("Checking records eligibility for email functionality");
                foreach (DataRow pDRFinal in pDTSVCRecord.Rows)
                {
                    bool bAddRow = false;
                    DataRow[] pDEmailed = pDTEmailTable.Select("GLOBALID = '" + pDRFinal["GLOBALID"].ToString() + "'");
                    if (pDEmailed.Length == 0)
                        bAddRow = (Convert.ToString(pDRFinal["STATUS"]) == "5" && Convert.ToString(pDRFinal["SUPERVISORYCONTROL"]) == "Y");
                    else
                        bAddRow = (Convert.ToString(pDRFinal["STATUS"]) == Convert.ToString(pDEmailed[0]["STATUS"]) &&
                            !String.IsNullOrEmpty(Convert.ToString(pDRFinal["SUPERVISORYCONTROL"])) &&
                            Convert.ToString(pDRFinal["SUPERVISORYCONTROL"]) != Convert.ToString(pDEmailed[0]["SUPERVISORYCONTROL"]))
                            ||
                            (Convert.ToString(pDRFinal["STATUS"]) != Convert.ToString(pDEmailed[0]["STATUS"]) &&
                            Convert.ToString(pDRFinal["STATUS"]) != "0" &&
                            Convert.ToString(pDRFinal["SUPERVISORYCONTROL"]) == Convert.ToString(pDEmailed[0]["SUPERVISORYCONTROL"]));
                    if (bAddRow) pDTFinal.Rows.Add(pDRFinal.ItemArray);

                }
                WriteLine("Final records count " + pDTFinal.Rows.Count.ToString());
                pDTFinalServer = pDTFinal.Copy();
                pDTFinal.Columns["DEVICETYPE"].SetOrdinal(0);
                pDTFinal.Columns["SUBTYPE"].SetOrdinal(1);
                pDTFinal.Columns["OPERATINGNUMBER"].SetOrdinal(2);
                pDTFinal.Columns["CIRCUITID"].SetOrdinal(3);
                pDTFinal.Columns["CIRCUITNAME"].SetOrdinal(4);
                pDTFinal.Columns["DIVISION"].SetOrdinal(5);
                pDTFinal.Columns["LOCATION"].SetOrdinal(6);
                pDTFinal.Columns["SUPERVISORYCONTROL"].SetOrdinal(7);
                pDTFinal.Columns["STATUS"].SetOrdinal(8);
                pDTFinal.Columns["MODIFIEDON"].SetOrdinal(9);
                pDTFinal.Columns["GLOBALID"].SetOrdinal(10);
                pDTFinal.Columns["EMAILEDON"].SetOrdinal(11);
            }
            catch (Exception ex)
            {
                WriteLine("getDataTableFinal Exception:" + ex.Message);
            }
            finally
            {
                
            }
            return pDTFinal;
        }

        private static DataTable getDataTable(OracleConnection pOConnection, string sCommandText, string sTableName)
        {
            OracleCommand OracleCommand = new OracleCommand(sCommandText, pOConnection);

            OracleDataAdapter OracleDataAdapter = new OracleDataAdapter();
            OracleDataAdapter.SelectCommand = OracleCommand;
            DataSet dataSet = new DataSet();

            try
            {
                OracleDataAdapter.Fill(dataSet, sTableName);
            }
            catch (Exception ex)
            {
                WriteLine("getDataTable Exception: " + ex.Message);
                return null;
            }
            WriteLine("Table populated successfully with record count as " + dataSet.Tables[0].Rows.Count.ToString());
            return dataSet.Tables[0];
        }

        private static void Execute(OracleConnection pOConnection, string sCommandText)
        {
            OracleCommand objCmd = new OracleCommand();
            objCmd.Connection = pOConnection;
            objCmd.CommandText = "sde.version_util.set_current_version";
            objCmd.CommandType = CommandType.StoredProcedure;
            objCmd.Parameters.Add("VERSION_NAME", OracleDbType.Varchar2).Value = "SDE.DEFAULT";
            try
            {
               objCmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                WriteLine("Setting Default exception: " + ex.Message);
            }
        }

        private static string ExportDatatableToHtml(DataTable dt)
        {
            WriteLine("Creating HTML table for final data");
            try
            {
                if (dt.Rows.Count == 0) return string.Empty;
                dt.Columns.Remove("EMAILEDON");
                dt.Columns.Remove("GLOBALID");
                //dt.Columns.Remove("MODIFIEDON");
                dt.Columns["MODIFIEDON"].ColumnName = "LAST UPDATED ON";

                StringBuilder strHTMLBuilder = new StringBuilder();
                strHTMLBuilder.Append("Below is the list of devices with Supervisory Control, added to GIS system");
                strHTMLBuilder.Append("<br>");
                strHTMLBuilder.Append("<html >");
                strHTMLBuilder.Append("<head>");
                strHTMLBuilder.Append("</head>");
                strHTMLBuilder.Append("<body>");
                strHTMLBuilder.Append("<table border-collapse: collapse; cellborder='1px solid black' cellpadding='1' cellspacing='1' style='font-family:Ariel'>");

                strHTMLBuilder.Append("<tr style='background-color:#1E90FF' cellborder='1px solid black'>");
                foreach (DataColumn myColumn in dt.Columns)
                {
                    //if (myColumn.ColumnName == "EMAILEDON" || myColumn.ColumnName == "GLOBALID" || myColumn.ColumnName == "MODIFIEDON") continue;
                    strHTMLBuilder.Append("<td style=\"border-style:solid; border-width:thin;\">");
                    strHTMLBuilder.Append(myColumn.ColumnName);
                    strHTMLBuilder.Append("</td>");

                }
                strHTMLBuilder.Append("</tr>");

                int i = 0;
                foreach (DataRow myRow in dt.Rows)
                {
                    if (i % 2 == 0)
                        strHTMLBuilder.Append("<tr style='background-color:#FFFFFF' cellborder='1px solid black'>");
                    else
                        strHTMLBuilder.Append("<tr style='background-color:#87CEFA' cellborder='1px solid black'>");
                    foreach (DataColumn myColumn in dt.Columns)
                    {
                        //if (myColumn.ColumnName == "EMAILEDON" || myColumn.ColumnName == "GLOBALID" || myColumn.ColumnName == "MODIFIEDON") continue;
                        string sValue = Convert.ToString(myRow[myColumn.ColumnName]);
                        if (myColumn.ColumnName == "STATUS")
                            sValue = sValue.Replace("5", "In Service").Replace("30", "Idle").Replace("0", "Proposed Install");
                        if (myColumn.ColumnName == "SUPERVISORYCONTROL")
                            sValue = sValue.Replace("Y", "Yes").Replace("N", "No");

                        strHTMLBuilder.Append("<td style=\"border-style:solid; border-width:thin;\">");
                        strHTMLBuilder.Append(sValue);
                        strHTMLBuilder.Append("</td>");

                    }
                    strHTMLBuilder.Append("</tr>");
                    ++i;
                }

                //Close tags.  
                strHTMLBuilder.Append("</table>");
                strHTMLBuilder.Append("</body>");
                strHTMLBuilder.Append("</html>");

                string Htmltext = strHTMLBuilder.ToString();
                return Htmltext;
            }
            catch (Exception ex)
            {
                WriteLine("ExportDatatableToHtml Exception: " + ex.Message);
                return string.Empty;
            }
        }

        private static string getHtml(DataTable pDTFinal)
        {
            try
            {
                if (pDTFinal.Rows.Count == 0)
                    return string.Empty;

                string htmlTableStart = "<table style=\"border-collapse:collapse; text-align:center;\" >";
                string htmlTableEnd = "</table>";
                string htmlHeaderRowStart = "<tr style =\"background-color:#6FA1D2; color:#ffffff;\">";
                string htmlHeaderRowEnd = "</tr>";
                string htmlTrStart = "<tr style =\"color:#555555;\">";
                string htmlTrEnd = "</tr>";
                string htmlTdStart = "<td style=\" border-color:#5c87b2; border-style:solid; border-width:thin; padding: 5px;\">";
                string htmlTdEnd = "</td>";
                
                string messageBody = "<font>The following are the records: </font><br><br>";
                messageBody += htmlTableStart;
                messageBody += htmlHeaderRowStart;
                messageBody += htmlTdStart + "Column1 " + htmlTdEnd;
                messageBody += htmlHeaderRowEnd;

                foreach (DataRow Row in pDTFinal.Rows)
                {
                    messageBody = messageBody + htmlTrStart;
                    messageBody = messageBody + htmlTdStart + Row["fieldName"] + htmlTdEnd;
                    messageBody = messageBody + htmlTrEnd;
                }
                messageBody = messageBody + htmlTableEnd;


                return messageBody;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        private static bool SendAutomatedEmail(string htmlString)
        {
            bool retval = false;
            WriteLine("Sending email");
            try
            {
                retval = !string.IsNullOrEmpty(htmlString);
                string mailServer = ConfigurationManager.AppSettings["SMTPSERVER"];

                MailMessage message = new MailMessage();
                message.From = new MailAddress(ConfigurationManager.AppSettings["MAILFROM"]);
                foreach (string to in ConfigurationManager.AppSettings["MAILTO"].Split(';'))
                    message.To.Add(to);
                foreach (string cc in ConfigurationManager.AppSettings["MAILCC"].Split(';'))
                    message.To.Add(cc);
                message.IsBodyHtml = true;

                message.Body = retval ? htmlString : "There is no list to be shared today";

                message.Subject = ConfigurationManager.AppSettings["MAILSUBJECT"];

                SmtpClient client = new SmtpClient(mailServer);
                //var AuthenticationDetails = new NetworkCredential("user@domain.com", "password");
                //client.Credentials = AuthenticationDetails;
                client.Send(message);
                WriteLine("Email Sent");
                return retval;
            }
            catch (Exception ex)
            {
                WriteLine("SenAutomatedEmail Exception: " + ex.Message);
                return retval;
            }

        }

        private static void WriteToServer(OracleConnection pOConnection, DataTable dataTable)
        {
            WriteLine("Writing all final data to databse");
            try
            {
                using (OracleBulkCopy bulkCopy = new OracleBulkCopy(pOConnection))
                {
                    bulkCopy.BulkCopyOptions = OracleBulkCopyOptions.UseInternalTransaction;                    
                    bulkCopy.DestinationTableName = "PGE_SVCEMAIL";
                    bulkCopy.WriteToServer(dataTable);
                    bulkCopy.Close();
                }
                WriteLine("Writing datatable to oracle");
            }
            catch (Exception ex)
            {
                WriteLine("WriteToServer Exception: " + ex.Message);
                throw;
            }
        }

        /// <summary>
        /// Write on console and log file
        /// </summary>
        /// <param name="sMsg"></param>
        public static void WriteLine(string sMsg)
        {
            sMsg = !String.IsNullOrEmpty(sMsg) ? DateTime.Now.ToLongTimeString() + " -- " + sMsg : sMsg;
            pSWriter = File.AppendText(sPath);
            pSWriter.WriteLine(sMsg);
            Console.WriteLine(sMsg);
            pSWriter.Close();
        }
    }
}
