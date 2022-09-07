using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using ESRI.ArcGIS.esriSystem;
using Miner.Interop;
using System.Configuration;
using System.IO;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Carto;

namespace PGE.Interfaces.AutoCreateWIPPolygon
{
    public  class Common
    {
        
        public static Log4NetLogger _log = new Log4NetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, ReadConfigurations.LOGCONFIG);
       
        public  IWorkspace GetWorkspace(string argStrworkSpaceConnectionstring)
        {
            Type t = null;
            IWorkspaceFactory workspaceFactory = null;
            IWorkspace workspace = null;
            try
            {

                t = Type.GetTypeFromProgID("esriDataSourcesGDB.SdeWorkspaceFactory");
                workspaceFactory = (IWorkspaceFactory)Activator.CreateInstance(t);
                workspace = workspaceFactory.OpenFromFile(argStrworkSpaceConnectionstring, 0);
            }
            catch (Exception exp)
            {
                Common._log.Error("Error in getting SDE Workspace, function GetWorkspace: " + exp.Message);
                Common._log.Info(exp.Message + "   " + exp.StackTrace);
                throw exp;
            }
            return workspace;
        }

        public  void Convert_DataTable_To_CSV(DataTable dt, string sCSVFolderPath, string CSVNAME)
        {
            
            StringBuilder sb = new StringBuilder();

            string[] columnNames = dt.Columns.Cast<DataColumn>().
                                              Select(column => column.ColumnName).
                                              ToArray();
            sb.AppendLine(string.Join(",", columnNames));

            foreach (DataRow row in dt.Rows)
            {
                string[] fields = row.ItemArray.Select(field => field.ToString()).
                                                ToArray();
                sb.AppendLine(string.Join(",", fields));
            }

            File.WriteAllText(sCSVFolderPath + "\\" + CSVNAME + ".csv", sb.ToString());
        }

        public void UpdateStatusFlag(int sbatchid, string CurrentStatus)
        {
            String sQuery = null;

            try
            {
                sQuery = "UPDATE " + ReadConfigurations.WIPINPUTTABLE + " SET " +
                                   ReadConfigurations.WIP_STAGING_TABLE_FIELDS.PROCESS_STATUS + " = '" + CurrentStatus + "' WHERE " +
                                   ReadConfigurations.WIP_STAGING_TABLE_FIELDS.BATCHID + " = " + sbatchid + " and " + ReadConfigurations.WIP_STAGING_TABLE_FIELDS.PROCESS_STATUS +  " is null " ;
                (new DBHelper()).UpdateQuery(sQuery);

                Common._log.Info("Updated Status," + CurrentStatus + "," + DateTime.Now);
            }
            catch (Exception ex)
            {
                Common._log.Info (ex.Message + "   " + ex.StackTrace);
            }
        }

        public void Update_DETAIL_Table_StatusFlag(int sbatchid, string CurrentStatus,int total_wip,int success_records,int excep,int failed_Records)
        {
            String sQuery = null;

            try
            {
                
                sQuery = "UPDATE " + ReadConfigurations.WIPFILEDETAILTABLE + " SET " +
                                   ReadConfigurations.WIP_STAGING_TABLE_FIELDS.ZIP_STATUS + " = '" + CurrentStatus + "',"
                + ReadConfigurations.WIP_STAGING_TABLE_FIELDS.TOTAL_WIP_COUNT + " = " + total_wip + ","
                   + ReadConfigurations.WIP_STAGING_TABLE_FIELDS.FAILED_WIP_COUNT + " = " + excep + ","
                      + ReadConfigurations.WIP_STAGING_TABLE_FIELDS.SUCCESS_WIP_COUNT + " = " + success_records + ","
                         + ReadConfigurations.WIP_STAGING_TABLE_FIELDS.INCOMPLETE_WIP_COUNT + " = " + failed_Records 
                         +  " WHERE " + ReadConfigurations.WIP_STAGING_TABLE_FIELDS.BATCHID + " = " + sbatchid ;
                (new DBHelper()).UpdateQuery(sQuery);

                Common._log.Info("Updated Status," + CurrentStatus + "," + DateTime.Now);
            }
            catch (Exception ex)
            {
                Common._log.Info(ex.Message + "   " + ex.StackTrace);
            }
        }
        public void UpdateException(int sbatchid, string Error_Code,string Sequence)
        {
            String sQuery = null;
            string[] SError = Error_Code.Split(',');

            try
            {
                sQuery = "UPDATE " + ReadConfigurations.WIPINPUTTABLE + " SET "
                                 + ReadConfigurations.WIP_STAGING_TABLE_FIELDS.Exception_MSG + " = '" + SError[1] + "',"
                                 + ReadConfigurations.WIP_STAGING_TABLE_FIELDS.Error_Code + " = '" + SError[0] + "',"
                                 + ReadConfigurations.WIP_STAGING_TABLE_FIELDS.PROCESS_STATUS + " = '" + ReadConfigurations.STATUS.status_EXCEPTION + "',"
                                 + ReadConfigurations.WIP_STAGING_TABLE_FIELDS.PDF_STATUS  + " = '" + ReadConfigurations.STATUS.Status_PDF_FAILED + "' WHERE "
                                 + ReadConfigurations.WIP_STAGING_TABLE_FIELDS.BATCHID + " = " + sbatchid + " and " + ReadConfigurations.WIP_STAGING_TABLE_FIELDS.SEQUENCE + " = " + Sequence ;
                (new DBHelper()).UpdateQuery(sQuery);

                Common._log.Info("Updated Exception Message ," + SError[1] + "," + DateTime.Now);

            }
            catch (Exception ex)
            {
                Common._log.Info(ex.Message + "   " + ex.StackTrace);
            }
        }
      


        internal IRelationshipClass GetRelationshipClass(string OriginClassName, IFeatureClass pDestinationClass)
        {
            IRelationshipClass pRelClass = null;
            try
            {
                if (pDestinationClass != null)
                {
                    IEnumRelationshipClass enumrelClass = pDestinationClass.get_RelationshipClasses(ESRI.ArcGIS.Geodatabase.esriRelRole.esriRelRoleAny);
                    pRelClass = enumrelClass.Next();
                    while (pRelClass != null)
                    {
                        if ((pRelClass.OriginClass.AliasName.ToString().ToUpper() == OriginClassName.ToUpper()))
                        {

                            break;
                        }
                        pRelClass = enumrelClass.Next();
                    }
                }
                else
                {
                   _log.Debug(" Queried FeatureClass " + pDestinationClass.AliasName.ToString() + " is not present in the data, Please Verify and Try again later.");
                }

            }
            catch (Exception ex)
            {
                Common._log.Info(ex.Message + "   " + ex.StackTrace);
            }
            return pRelClass;
        }
        public IFeatureIndex CreateFeatureIndex(IFeatureClass pFeatureClass)
        {
            IFeatureIndex fi = new FeatureIndexClass();
            try
            {

                fi.FeatureClass = pFeatureClass;
                //create the index
                fi.Index(null, ((IGeoDataset)pFeatureClass).Extent);
            }
            catch (Exception ex)
            {
                Common._log.Error(ex.Message + " at " + ex.StackTrace);
            }
            return fi;
        }
        public void UpdateStatusFlag(int sbatchid, string CurrentStatus, string sequence)
        {
            String sQuery = null;

            try
            {
                sQuery = "UPDATE " + ReadConfigurations.WIPINPUTTABLE + " SET " +
                                    ReadConfigurations.WIP_STAGING_TABLE_FIELDS.PROCESS_STATUS + " = '" + CurrentStatus + "' WHERE " +
                                   ReadConfigurations.WIP_STAGING_TABLE_FIELDS.BATCHID + " = " + sbatchid + " and "  
                                   + ReadConfigurations.WIP_STAGING_TABLE_FIELDS.SEQUENCE + " = " + sequence ;
                (new DBHelper()).UpdateQuery(sQuery);
            }
            catch (Exception ex)
            {
                Common._log.Info(ex.Message + "   " + ex.StackTrace);
            }
        }
        internal void UpdateStatusFlag(int batchid, string CurrentStatus, string PDF_STATUS, string sequence)
        {
            String sQuery = null;

            try
            {
                sQuery = "UPDATE " + ReadConfigurations.WIPINPUTTABLE + " SET " 
                                    + ReadConfigurations.WIP_STAGING_TABLE_FIELDS.PROCESS_STATUS + " = '" + CurrentStatus + "',"
                                    + ReadConfigurations.WIP_STAGING_TABLE_FIELDS.PDF_STATUS + " = '" + PDF_STATUS + "' WHERE " +
                                    ReadConfigurations.WIP_STAGING_TABLE_FIELDS.BATCHID + " = " + batchid + " and " + ReadConfigurations.WIP_STAGING_TABLE_FIELDS.SEQUENCE + " = " + sequence;
                (new DBHelper()).UpdateQuery(sQuery);
            }
            catch (Exception ex)
            {
                Common._log.Info(ex.Message + "   " + ex.StackTrace);
            }
        }

        internal void SendMailToSAP(string MailReason,  string sXMLFilePath)
        {
            try
            {
                string bodyText = string.Empty;
                string subject = string.Empty;
                subject = ConfigurationManager.AppSettings["MAIL_SUBJECT_INVALIDXML"];
               
                    bodyText = "Hello Team,  <br />"
                        + " Failed to load XML file in WIP Database due to format errors.<br /> "
                        + " Filename - > " + sXMLFilePath + "  <br />  <br /> "
                        + " Please send the correct file again through ESFT.<br />  <br /> "
                        + "Thank you,  <br />"
                        + "Auto WIP Create Process  <br /><br />"

                        + " [THIS IS AN AUTOMATED MESSAGE - PLEASE DO NOT REPLY DIRECTLY TO THIS EMAIL] ";
                
                string lanIDs = ConfigurationManager.AppSettings["MAIL_TO_ADDRESS_SAP"];
                SendMail("", subject, bodyText, lanIDs);
            }
            catch (Exception ex)
            {
                _log.Error("Unhandled exception occurred while sending the mail to SAP  ," + ex.Message.ToString() + " at " + ex.StackTrace);
            }
        }
        public void SendMail(string sFileName, string subject, string bodyText, string lanIDs)
        {
            try
            {

               _log.Info("Sending email to " + lanIDs);
                string fromDisplayName = ConfigurationManager.AppSettings["MAIL_FROM_DISPLAY_NAME"];
                string fromAddress = ConfigurationManager.AppSettings["MAIL_FROM_ADDRESS"];


                EmailService.Send(mail =>
                {
                    mail.From = fromAddress;
                    mail.FromDisplayName = fromDisplayName;
                    mail.To = lanIDs;
                    mail.Subject = subject;
                    mail.BodyText = bodyText;
                    if (!string.IsNullOrEmpty(sFileName))
                    {
                        mail.Attachments.Add(sFileName);
                    }

                });
            }

            catch (Exception ex)
            {
                _log.Error(ex.Message);
            }
        }
        /// <summary>
        /// For any process error send mail to support team
        /// </summary>
        /// <param name="MailReason"></param>
        /// <param name="sCircuitID"></param>
        /// <param name="Batch_Number"></param>
        /// <param name="sXMLFilePath"></param>
        internal void SendMailToSupportTeam(string MailReason, string Batch_Number, string sXMLFilePath)
        {
            try
            {
                string subject = ConfigurationManager.AppSettings["MAIL_SUBJECT_PROCESSERROR"];
                string bodyText = string.Empty;

                if (MailReason == ReadConfigurations.STATUS.Status_CREATEDEXP)
                {
                    bodyText = "Hello Team,  <br />"
                       + " Auto WIP Creation Process has been terminated as process is having feature creation error, for more details please see the log file. <br /><br /> "
                      
                       + " BatchID : " + Batch_Number.ToString() + " <br /><br />"

                       + "Thank you,  <br />"
                       + " Auto WIP Create Process <br /><br />"

                       + " [THIS IS AN AUTOMATED MESSAGE - PLEASE DO NOT REPLY DIRECTLY TO THIS EMAIL] ";
                }
                else if (MailReason == ReadConfigurations.STATUS.Status_PDFEXP)
                {

                    bodyText = "Hello Team,  <br />"
                        + " Auto WIP Creation Process has been terminated as process is having error while generating the PDF, for more details please see the log file. <br /><br /> "                       
                        + " BatchID : " + Batch_Number.ToString() + " <br /><br />"
                        + "Thank you,  <br />"
                        + " Auto WIP Create Process <br /><br />"

                        + " [THIS IS AN AUTOMATED MESSAGE - PLEASE DO NOT REPLY DIRECTLY TO THIS EMAIL] ";
                }               
                else if (MailReason == "GETSEQUENCE_ERROR")
                {
                    bodyText = "Hello Team,  <br />"
                       + " Auto WIP Create Processhas been terminated due to below error, for more details please see the log file. <br /><br /> "
                       + " Error: Application has been failed to getting a database sequence  <br />  <br /> "

                       + "Thank you,  <br />"
                       + " Auto WIP Create Process <br /><br />"

                       + " [THIS IS AN AUTOMATED MESSAGE - PLEASE DO NOT REPLY DIRECTLY TO THIS EMAIL] ";
                }
               
                else
                {
                    bodyText = "Hello Team,  <br />"
                    + " Auto WIP Creation Process has been terminated as process is having " + MailReason.ToLower().ToString() + " error, for more details please see the log file. <br /><br /> "
                    
                    + " BatchID : " + Batch_Number.ToString() + " <br /><br />"

                    + "Thank you,  <br />"
                    + " Auto WIP Create Process <br /><br />"

                    + " [THIS IS AN AUTOMATED MESSAGE - PLEASE DO NOT REPLY DIRECTLY TO THIS EMAIL] ";
                }
                string lanIDs = ConfigurationManager.AppSettings["MAIL_TO_ADDRESS_EDGISSupportTeam"];
                SendMail("", subject, bodyText, lanIDs);
            }
            catch (Exception ex)
            {
                _log.Error("Unhandled exception occurred while sending the mail to DAPHIE  ," + ex.Message.ToString() + " at " + ex.StackTrace);
            }
        }

        internal IFeatureClass GetFeatureClass(IFeatureWorkspace pFeatureWorkspace, string sClassName)
        {
            IFeatureClass returnFeatureclass = null;
            try
            {
                returnFeatureclass = pFeatureWorkspace.OpenFeatureClass(sClassName);
            }
            catch (Exception ex)
            {
                _log.Error("Unhandled exception occurred while getting the Featureclass-  ," + sClassName + ", Exception Message :-," + ex.Message.ToString() + " at " + ex.StackTrace);
                throw ex;
            }
            return returnFeatureclass;
        }
    }
}
