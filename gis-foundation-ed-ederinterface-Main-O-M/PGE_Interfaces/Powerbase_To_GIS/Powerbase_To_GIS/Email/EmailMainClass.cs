using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Oracle.DataAccess.Client;
using System.Data;
using System.Configuration;
using System.Reflection;
using System.Collections;
using System.IO;

namespace Powerbase_To_GIS
{
    class EmailMainClass
    {      
        private static string GetMailContentFromDataTable(DataTable argdataTable)
        {
            StringBuilder sb = new StringBuilder();
            string mailContent = null;
            try
            {
                string[] columnNames = argdataTable.Columns.Cast<DataColumn>().
                                                  Select(column => column.ColumnName).
                                                  ToArray();
                sb.AppendLine(string.Join(",", columnNames));

                foreach (DataRow row in argdataTable.Rows)
                {
                    string[] fields = row.ItemArray.Select(field => field.ToString()).
                                                    ToArray();
                    sb.AppendLine(string.Join(",", fields));
                }

                mailContent = sb.ToString();
            }
            catch (Exception exp)
            {
                Common._log.Error("Exception : " + exp.Message +" |  Stack Trace | " + exp.StackTrace+" at "+exp.ToString());
            }
            return mailContent;
        }
        /// <summary>
        /// Initializes and send an email to users 
        /// </summary>
        /// <returns></returns>
        public static bool ComplieAndSendMail(string argStrConnstringpgedata)
        {
            bool bMailSentSuccess = false;
            DataTable dtRecords = null;
            string queryToGetDataToUpdate = null;
            try
            {
                queryToGetDataToUpdate = "SELECT " + ReadConfigurations.col_FEATURECLASS + "," + ReadConfigurations.col_GLOBALID + "," + ReadConfigurations.col_OPERATINGNUMBER + "," + ReadConfigurations.col_PB_RLID + "," + ReadConfigurations.col_ATTRIBUTENAME + "," + ReadConfigurations.col_ATTRIBUTEVALUE + ",'\"'||" + ReadConfigurations.col_COMMENTS + "||'\"' AS " + ReadConfigurations.col_COMMENTS + "," + ReadConfigurations.col_DATETIME + " FROM " + ReadConfigurations.GetValue(ReadConfigurations.TB_PB_GIS_REJECTED_RECORDS) + " WHERE " + ReadConfigurations.col_BATCHID + "=" + MainClass._ibatchID;
                dtRecords = DBHelper.GetDataTable(argStrConnstringpgedata, queryToGetDataToUpdate);

                string toemailID = ReadConfigurations.GetValue(ReadConfigurations.ToemailID);
                string fromAddress = ReadConfigurations.GetValue(ReadConfigurations.MAIL_FROM_ADDRESS);
                string fromDisplayName = ReadConfigurations.GetValue(ReadConfigurations.MAIL_FROM_DISPLAY_NAME);
                string subject = ReadConfigurations.GetValue(ReadConfigurations.SUBJECT);
                
                string bodyText = "Hello,<br /><br />" + "The attribute values ( in attached log file ) shared through the interface are not eligible to update in EDGIS.<br />"
                    + "These attributes are controlled by the coded value domain in EDGIS and these attributes values are not any one of the available domain code.<br />"
                    + "Please go through the attached log file for checking all the records.<br />"
                    + " <br /><br />Thank You, <br />EDGIS Support Team<br /><br />"
                    + "NOTE : This is a system generated message, Please don't reply directly to this email.";

                if (dtRecords.Rows.Count == 0)
                {
                    Common._log.Info("No data found for the current run to send mail. Query used to get data : " + queryToGetDataToUpdate + " , Record Count :" + dtRecords.Rows.Count + ".Not sending mail.");
                }
                else if (string.IsNullOrEmpty(toemailID) || string.IsNullOrEmpty(fromAddress))
                {
                    Common._log.Info("Either fromAddress : " + fromAddress + " or to email ID : " + toemailID + " is not provided Please check config file. Not sending mail.");
                }
                else
                {
                    Common._mailContent = EmailMainClass.GetMailContentFromDataTable(dtRecords);

                    bMailSentSuccess = EmailService.Send(mail =>
                    {
                        mail.From = fromAddress;
                        mail.FromDisplayName = fromDisplayName;
                        mail.To = toemailID;
                        mail.Subject = subject;
                        mail.BodyText = bodyText;
                        mail.IsHighPriority = false;
                    });
                    Common._log.Info("Email Successfully sent to: " + toemailID);
                    Common._log.Info("Email Content : " + Common._mailContent);
                }
            }
            catch (Exception exp)
            {
                bMailSentSuccess = false;
                Common._log.Error("Error in Sending Mail: ", exp);
                Common._log.Error("Exception : " + exp.Message + " |  Stack Trace | " + exp.StackTrace + " at " + exp.ToString());
                ErrorCodeException ece = new ErrorCodeException(exp);
                Environment.ExitCode = ece.CodeNumber;
            }
            return bMailSentSuccess;
        }
        /// <summary>
        /// Initializes and send an email to users 
        /// </summary>
        /// <returns></returns>
        public static bool ComplieAndSendMailForNoRecordsInStageTable()
        {
            bool bMailSentSuccess = false;
            try
            {
                string toemailID = ReadConfigurations.GetValue(ReadConfigurations.ToemailID);
                string fromAddress = ReadConfigurations.GetValue(ReadConfigurations.MAIL_FROM_ADDRESS);
                string fromDisplayName = ReadConfigurations.GetValue(ReadConfigurations.MAIL_FROM_DISPLAY_NAME);
                string subject = ReadConfigurations.GetValue(ReadConfigurations.SUBJECT);

                string bodyText = "Hello,<br /><br />" + "There is no record present in input table PGEDATA.PB_GIS_UPDATES for processing, Please connect with EI team for further check.<br />"
                    + "There might an issue in EI batch job.<br />"
                    + " <br /><br />Thank You, <br />EDGIS Support Team<br /><br />"
                    + "NOTE : This is a system generated message, Please don't reply directly to this email.";

                Common._mailContent = string.Empty;

                bMailSentSuccess = EmailService.Send(mail =>
                {
                    mail.From = fromAddress;
                    mail.FromDisplayName = fromDisplayName;
                    mail.To = toemailID;
                    mail.Subject = subject;
                    mail.BodyText = bodyText;
                    mail.IsHighPriority = true;
                });
                Common._log.Info("Email Successfully sent to: " + toemailID);
            }
            catch (Exception exp)
            {
                bMailSentSuccess = false;
                Common._log.Error("Error in Sending Mail: ", exp);
                Common._log.Error("Exception : " + exp.Message + " |  Stack Trace | " + exp.StackTrace + " at " + exp.ToString());
                ErrorCodeException ece = new ErrorCodeException(exp);
                Environment.ExitCode = ece.CodeNumber;
            }
            return bMailSentSuccess;
        }

        public static bool ComplieAndSendMailForSessionInProgress(string argStrSessionName, string argStrSessionStatus)
        {
            bool bMailSentSuccess = false;
            try
            {
                string toemailID = ReadConfigurations.GetValue(ReadConfigurations.ToemailID_SessionNotPosted);
                string ccemailID = ReadConfigurations.GetValue(ReadConfigurations.ccemailID_SessionNotPosted);
                string fromAddress = ReadConfigurations.GetValue(ReadConfigurations.MAIL_FROM_ADDRESS);
                string fromDisplayName = ReadConfigurations.GetValue(ReadConfigurations.MAIL_FROM_DISPLAY_NAME);
                string subject = ReadConfigurations.GetValue(ReadConfigurations.SUBJECT_SessionNotPosted);

                string bodyText = "Hello,<br /><br />" + "The previous sesion [" + argStrSessionName + "] created by the batch job has not been posted yet. Session Status : [" + argStrSessionStatus + "]. Please take corrective action based on session status. Exiting the process.<br />"                    
                    + " <br /><br />Thank You, <br />EDGIS Support Team<br /><br />"
                    + "NOTE : This is a system generated message, Please don't reply directly to this email.";

                Common._mailContent = string.Empty;

                bMailSentSuccess = EmailService.Send(mail =>
                {
                    mail.From = fromAddress;
                    mail.FromDisplayName = fromDisplayName;
                    mail.To = toemailID;
                    mail.CC = ccemailID;
                    mail.Subject = subject;
                    mail.BodyText = bodyText;
                    mail.IsHighPriority = true;
                });
                Common._log.Info("Email Successfully sent to: " + toemailID);
            }
            catch (Exception exp)
            {
                bMailSentSuccess = false;
                Common._log.Error("Error in Sending Mail: ", exp);
                Common._log.Error("Exception : " + exp.Message + " |  Stack Trace | " + exp.StackTrace + " at " + exp.ToString());
                ErrorCodeException ece = new ErrorCodeException(exp);
                Environment.ExitCode = ece.CodeNumber;
            }
            return bMailSentSuccess;
        }
    }
}
