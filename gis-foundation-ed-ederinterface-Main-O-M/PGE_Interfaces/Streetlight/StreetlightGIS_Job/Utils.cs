using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.DataSourcesGDB;
using System.Collections;
using System.Configuration;
using System.IO;
using System.Net.Mail;
using System.Data;
using System.Diagnostics;

namespace PGE.Interfaces.StreetlightGIS_Job
{
    class Utils
    {
        public static void WriteLine(string strLogPath, string sMsg)
        {
            StreamWriter pSWriter = File.AppendText(strLogPath);
            pSWriter.WriteLine(sMsg);
            Console.WriteLine(sMsg);
            pSWriter.Close();
        }

        public static IWorkspace AccessWorkspaceFromPropertySet(string database)
        {
            IPropertySet propertySet = new PropertySetClass();
            propertySet.SetProperty("DATABASE", database);
            IWorkspaceFactory workspaceFactory = new AccessWorkspaceFactoryClass();
            return workspaceFactory.Open(propertySet, 0);
        }

        public static IWorkspace ArcSdeWorkspaceFromFile(string connectionFile)
        {
            return ((IWorkspaceFactory)Activator.CreateInstance(Type.GetTypeFromProgID("esriDataSourcesGDB.SdeWorkspaceFactory"))).
                OpenFromFile(connectionFile, 0);
        }

        public static Hashtable GetFieldMapping(IFeatureClass pSourceFeatureClass, IFeatureClass pDesFeatureClass, string strTask)
        {
            Hashtable fieldMappingHashTable = new Hashtable();
            string strMappingString = string.Empty;
            int intSourceFieldIndex = -1, intDesFieldIndex = -1;
            try
            {
                if (strTask == SLConfig.Value_GISTaskNumber)
                {
                    strMappingString = ConfigurationManager.AppSettings["FieldMappingTask_FieldProcss"];
                    string[] strArrFieldMapping = strMappingString.Split(';');
                    string[] strArrFieldMap;
                    for (int iCnt = 0; iCnt < strArrFieldMapping.Length; iCnt++)
                    {
                        strArrFieldMap = strArrFieldMapping[iCnt].Split(',');
                        intSourceFieldIndex = pSourceFeatureClass.FindField(strArrFieldMap[0]);
                        intDesFieldIndex = pDesFeatureClass.FindField(strArrFieldMap[1]);
                        if (strArrFieldMap[1] == "FAppliance4" || strArrFieldMap[0] == "FAppliance4")
                        {
                            string ss = "";
                        }
                        if (intSourceFieldIndex != -1 && intDesFieldIndex != -1)
                        {
                            fieldMappingHashTable.Add(intSourceFieldIndex, intDesFieldIndex);
                        }
                        else
                        {
                            string dsd = "";
                        }
                    }
                }
                else if (strTask == SLConfig.Value_UpdateSLTableTaskNumber)
                {
                    strMappingString = ConfigurationManager.AppSettings["FieldMappingTask_SLUpdate"];
                    string[] strArrFieldMapping = strMappingString.Split(';');
                    string[] strArrFieldMap;
                    for (int iCnt = 0; iCnt < strArrFieldMapping.Length; iCnt++)
                    {
                        strArrFieldMap = strArrFieldMapping[iCnt].Split(',');
                        intDesFieldIndex = pDesFeatureClass.FindField(strArrFieldMap[0]);
                        intSourceFieldIndex = pSourceFeatureClass.FindField(strArrFieldMap[1]);
                        if (strArrFieldMap[1] == "SPITEMHIST" || strArrFieldMap[1] == "SP_ITEM_HIST")
                        {
                            string ss = "";
                        }
                        if (intSourceFieldIndex != -1 && intDesFieldIndex != -1)
                        {
                            fieldMappingHashTable.Add(intSourceFieldIndex, intDesFieldIndex);
                        }
                        else
                        {
                            string dsd = "";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return fieldMappingHashTable;
        }

        public static bool SendMail(clsEmailDetails objEmailDetails, bool boolProcessSuccess, string strTaskID)
        {
            string strContent = string.Empty;
            try
            {
                if (objEmailDetails == null)
                    return false;
                MailMessage message = new MailMessage();
                message.IsBodyHtml = true;
                List<string> lstLanIds = objEmailDetails.LstToLanIds;
                if (lstLanIds != null && lstLanIds.Count > 0)
                {
                    for (int iCnt = 0; iCnt < lstLanIds.Count; iCnt++)
                    {
                        message.To.Add(lstLanIds[iCnt] + "@pge.com");
                    }
                }
                lstLanIds = objEmailDetails.LstCcLanIDs;
                if (lstLanIds != null && lstLanIds.Count > 0)
                {
                    for (int iCnt = 0; iCnt < lstLanIds.Count; iCnt++)
                    {
                        message.CC.Add(lstLanIds[iCnt] + "@pge.com");
                    }
                }
                message.From = new MailAddress(SLConfig.MailFrom);
                message.From = new MailAddress(SLConfig.MailFrom.Trim(), SLConfig.MailDisplayName);
                message.Subject = string.Format(SLConfig.MailSubject, objEmailDetails.StrJobId);

                if (boolProcessSuccess)
                {
                    if (strTaskID == SLConfig.Value_GISTaskNumber)
                    {
                        strContent = string.Format(SLConfig.MailBody_ToODA, objEmailDetails.StrJobId);
                    }
                    else if (strTaskID == SLConfig.Value_UpdateSLTableTaskNumber)
                    {
                        strContent = string.Format(SLConfig.MailBody_ToGIS, objEmailDetails.StrJobId, objEmailDetails.StrTaskId);
                    }
                    else if (strTaskID == SLConfig.Value_CopyFilesTaskNumber)
                    {
                        strContent = string.Format(SLConfig.MailBody_ToGIS, objEmailDetails.StrJobId, objEmailDetails.StrTaskId);
                    }
                }
                else
                {
                    strContent = string.Format(SLConfig.MailBody_ToGIS_Error, objEmailDetails.StrJobId, objEmailDetails.StrTaskId);
                }
                string htmlText = "<html><div style='font-size:16px; font-family:Calibri;'>" + strContent + "</div></html>";
                message.Body = htmlText;
                SmtpClient client = new SmtpClient(SLConfig.Mail_Server);//Convert.ToString(pDic_Config["MAIL_SMTPSERVER"]));
                client.Send(message);
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static string GetObjectIDFromTxData(string strTxData)
        {
            string strObjectID = "";
            string[] ArrStrData1 = null;
            string[] ArrStrData2 = null;
            try
            {
                if (strTxData != null && strTxData.Contains("Last processed OBJECTID") == true)
                {
                    ArrStrData1 = strTxData.Split(new string[] { "OBJECTID:" }, StringSplitOptions.None);
                    if (ArrStrData1.Length > 0)
                    {
                        ArrStrData2 = ArrStrData1[1].Split(',');
                        strObjectID = ArrStrData2[0].Trim();
                        //strTxData.Split(Environment.NewLine);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return strObjectID;
        }

        public static void WriteToFile(DataTable dataSource, string fileOutputPath, bool firstRowIsColumnHeader = false, string seperator = ";")
        {
            var sw = new StreamWriter(fileOutputPath, false);
            if (dataSource != null)
            {
                int icolcount = dataSource.Columns.Count;

                if (firstRowIsColumnHeader)
                {
                    for (int i = 0; i < icolcount; i++)
                    {
                        sw.Write("\"" + dataSource.Columns[i] + "\"");
                        if (i < icolcount - 1)
                            sw.Write(seperator);
                    }

                    sw.Write(sw.NewLine);
                }

                foreach (DataRow drow in dataSource.Rows)
                {
                    for (int i = 0; i < icolcount; i++)
                    {
                        if (!Convert.IsDBNull(drow[i]))
                            sw.Write(drow[i].ToString());
                        if (i < icolcount - 1)
                            sw.Write(seperator);
                    }
                    sw.Write(sw.NewLine);
                }
            }
            sw.Close();
        }

        public static void WriteToFile(List<string> lstMapNumbers, string fileOutputPath)
        {
            if (lstMapNumbers != null && lstMapNumbers.Count > 0)
            {
                var sw = new StreamWriter(fileOutputPath, false);
                for (int iCnt = 0; iCnt < lstMapNumbers.Count; iCnt++)
                {
                    sw.Write(lstMapNumbers[iCnt]);
                    sw.Write(sw.NewLine);
                }
                sw.Close();
            }
        }
    }
}

    public class clsEmailDetails
    {
        private List<string> lstToLanIds;

        public List<string> LstToLanIds
        {
            get { return lstToLanIds; }
            set { lstToLanIds = value; }
        }

        private List<string> lstCcLanIDs;

        public List<string> LstCcLanIDs
        {
            get { return lstCcLanIDs; }
            set { lstCcLanIDs = value; }
        }

        private string strJobId;

        public string StrJobId
        {
            get { return strJobId; }
            set { strJobId = value; }
        }
        private string strTaskId;

        public string StrTaskId
        {
            get { return strTaskId; }
            set { strTaskId = value; }
        }
        private string strEmailMessage;

        public string StrEmailMessage
        {
            get { return strEmailMessage; }
            set { strEmailMessage = value; }
        }

        private string strSubject;

        public string StrSubject
        {
            get { return strSubject; }
            set { strSubject = value; }
        }

    }
