using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Configuration;
using Newtonsoft.Json;
using System.Web.UI;
using System.Data;
using System.Data.OracleClient;
using log4net;
using log4net.Config;


namespace PGE.Streetlight
{
    /// <summary>
    /// Summary description for MoveFile
    /// </summary>
    public class NewJob
    {
        private static string _destinationFolder = ConfigurationManager.AppSettings["DESTINATION_PATH"];
        private static string CONNECTION_STRING = ConfigurationManager.AppSettings["CONNECTION_STRING"];
        private static readonly ILog logger = LogManager.GetLogger(typeof(NewJob));
        private static string from_TaskID = string.Empty;
        private static string to_TaskID = string.Empty;
        private static string user_LANID = string.Empty;
       

        public NewJob()
        {
            XmlConfigurator.Configure();
        }

      

        public string ProcessJob(HttpContext context)
        {
            string strFileStatus = "";
             List<String> fileStatusArray = null;
            try
            {
                System.Collections.Specialized.NameValueCollection parameters = new System.Collections.Specialized.NameValueCollection();
                context.Response.ContentType = "text/plain";
                parameters = context.Request.Form;
                string _filePathArray = parameters["postData"];
                string jobDesc = parameters["jobDesc"];
                string comments = parameters["comments"];
                string txData = parameters["txData"];
                
                bool isDataInserted;
                bool createJobFlag = true;
                string strJobID = "";

                logger.Debug("Calling function to insert new job data in database.");
                isDataInserted = CreateJob(jobDesc, comments, txData, ref strJobID, context);

                if (isDataInserted == true)
                {
                    fileStatusArray = MoveFile(_filePathArray, strJobID);
                    if (fileStatusArray.Count > 0)
                    {
                        for (int i = 0; i < fileStatusArray.Count; i++)
                        {
                            if (fileStatusArray[i].Split(',')[1] == "0")
                            {
                                createJobFlag = false;
                                break;
                            }

                        }

                        if (createJobFlag == true)
                        {
                            //Add one more parameter in Create Job
                            //Done by Sharad Arya on 29/11/2016                          
                            fileStatusArray.Add(strJobID);
                            sendMail(strJobID, from_TaskID, to_TaskID, comments, user_LANID);
                        }
                        else
                        {   // del from database
                            DeleteRecordsFromDatabase(strJobID);
                        }
                    }
                }
                
            }
            catch (Exception ex)
            {
                string errorMsg = ConfigurationManager.AppSettings["MSG06"];
                if (fileStatusArray != null)
                {
                    fileStatusArray.Clear();
                }
                fileStatusArray.Add(errorMsg + ",0");
                logger.Error("Error in function ProcessJob:", ex);

            }
            strFileStatus = JsonConvert.SerializeObject(fileStatusArray);
            return strFileStatus;
           
        }

        private bool DeleteRecordsFromDatabase(string strJobID)
        {
            bool isDataDeleted = false;
            try
            {  List<string> strQuery = new List<string>();
                
            DataAccessObjects objDataAccess = new DataAccessObjects(CONNECTION_STRING);
            string query1 = "Delete sl_job where jobid = " + strJobID;

            string query2 = "delete from SL_TRANSACTION where REL_JOBID = TO_NUMBER(" + strJobID + ")";
            string query3 = "delete from SL_ASSIGNEDTASK where REL_JOBID = TO_NUMBER(" + strJobID + ")";
                strQuery.Add(query1);
                strQuery.Add(query2);
                strQuery.Add(query3);             
                isDataDeleted = objDataAccess.InsertData(strQuery);                      
            }
            catch (Exception ex)
            {
                logger.Error("Error in function DeleteRecordsFromDatabase:", ex);
                throw ex;             
            }
            return isDataDeleted;
        }

        private List<String> MoveFile(string _filePathArray, string strJobID)
        {
           string [] srcFilePath;                                     
           string srcFilename = string.Empty;
            string destFilePath = string.Empty;
            string strResponse = string.Empty;
            List<string> listFileStatus = new List<string>();
            try
            {
                logger.Info("Moving files.");
                srcFilePath = _filePathArray.Split(',');

                if (srcFilePath.Length > 0)
                {
                    if (!Directory.Exists(_destinationFolder))
                    {
                        Directory.CreateDirectory(_destinationFolder);
                    }

                    for (int i = 0; i < srcFilePath.Length; i++)
                    {
                        if (File.Exists(srcFilePath[i]))
                        {
                            srcFilename = Path.GetFileName(srcFilePath[i].ToString());                          
                            bool isFileLocked = IsFileLocked(srcFilePath[i]);
                            if (isFileLocked == false)
                            {
                                try
                                {
                                    Directory.CreateDirectory(_destinationFolder + strJobID);
                                    destFilePath = _destinationFolder + strJobID + " \\" + srcFilename;
                                    File.Move(srcFilePath[i], destFilePath);

                                    listFileStatus.Add(srcFilename + ",1");
                                }
                                catch (Exception ex)
                                {
                                    var fileExistError = ConfigurationManager.AppSettings["MSG04"];
                                    if (ex.Message.IndexOf(fileExistError) > (-1))
                                    {
                                        var fileExistErrMsg = ConfigurationManager.AppSettings["MSG05"];
                                        listFileStatus.Clear();
                                        listFileStatus.Add(fileExistErrMsg + ",0");
                                    }
                                    else
                                    {
                                        listFileStatus.Add(srcFilename + ",0");
                                    }
                                }
                            }
                            else
                            {
                                strResponse = ConfigurationManager.AppSettings["MSG03"];
                                listFileStatus.Clear();
                                listFileStatus.Add(strResponse + ",0");
                            }
                        }
                        else {
                            logger.Error("Error in function MoveFile: Either file does not exist or Access denied.");
                            strResponse = ConfigurationManager.AppSettings["MSG07"];
                            listFileStatus.Clear();
                            listFileStatus.Add(strResponse + ",0");
                            DeleteRecordsFromDatabase(strJobID);
                        }
                    }
                }
            }catch(Exception ex){
                logger.Error("Error in function MoveFile:", ex);
                throw ex;
            }
            return listFileStatus;
           
        }

        private bool IsFileLocked(string filePath)
        {

            FileStream stream = null;
              

            try
            {
                stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
            }
            catch (Exception ex)
            {
                //the file is unavailable because it is:
                //still being written to
                //or being processed by another thread
                //or does not exist (has already been processed)              
                return true;                          
            }
            finally
            {
                if (stream != null)
                    stream.Close();
            }

            //file is available
            return false;
        }

        private bool CreateJob(string jobDesc, string comments, string txData, ref string strJobID, HttpContext context)
        {
            bool isInsertedJobTb;
            bool isInsertedTransAssignTb = false;
            string userLanId = string.Empty;
            try
            {
                logger.Info("Data insertion for new job.");
                Login login = new Login();
                userLanId = login.getLanID(context);
               
                DataTable dtResponse = new DataTable();
                DataAccessObjects objDataAccess1 = new DataAccessObjects(CONNECTION_STRING);
                string query1 = "insert into sl_job(jobid, jobdesc, jobstatus) values( (select NVL(max(jobid),0)+1 from sl_job), '" + jobDesc + "','INITIATED')";
                isInsertedJobTb = objDataAccess1.InsertData(query1);

                if (isInsertedJobTb == true)
                {                   
                    //Add code to get the JOb ID 
                    string strQueryJOBId = "select max(jobid) from sl_job";
                    DataTable db = objDataAccess1.GetDataTableFromAdapter(strQueryJOBId);
                    if (db != null && db.Rows.Count > 0)
                        strJobID = db.Rows[0][0].ToString();
                    string fromTaskID = "1";
                    string resultFlag = "Y";
                    bool isJobInitiated = true;
                    isInsertedTransAssignTb = TransAssignedInsertion(comments, userLanId, txData, resultFlag, strJobID, fromTaskID, isJobInitiated);
                    if (isInsertedTransAssignTb == false)
                    {                       
                        DataAccessObjects objDataAccess2 = new DataAccessObjects(CONNECTION_STRING);
                        string qrDelJob = "Delete sl_job where jobid = " + strJobID;
                        isInsertedJobTb = objDataAccess2.InsertData(qrDelJob);
                    }
                }
                logger.Info("Data insertion done.");
            }catch(Exception ex){
                logger.Error("Error in function CreateJob:", ex);
                throw ex;
            }
            return isInsertedTransAssignTb;
        }
        public bool TransAssignedInsertion(string comments, string userLanId, string txData, string resultFlag, string jobId, string fromTaskID, bool isJobInitiated)
        {
            bool isInsertedTransAssignTb = false;            
            string strToTaskID = "";           
            List<string> strQuery = new List<string>();
            DataTable dtResponse = new DataTable();
            try
            {
                logger.Info("Data Insertion for Job Transaction.");
                DataAccessObjects objDataAccess1 = new DataAccessObjects(CONNECTION_STRING);
                string query1 = "delete from SL_ASSIGNEDTASK where rel_jobid = TO_NUMBER(" + jobId + ") and REL_TASKID = '" + fromTaskID + "'";
                string query2 = "insert into sl_transaction (Rel_jobid, taskid,result,comments, lanid, txdata)" +
                                       " values(TO_NUMBER(" + jobId + "),'" + fromTaskID + "','" + resultFlag + "','" + comments + "','" + userLanId + "','" + txData + "')";
                strQuery.Add(query1);
                strQuery.Add(query2);
                logger.Info("Delete Query for SL_ASSIGNEDTASK:  "+query1);
                logger.Info("Insert Query for sl_transaction: " + query2);
                string query3 = string.Empty;              

               
                string qrGetToTaskID = "select totaskid from sl_taskflow where fromtaskid='" + fromTaskID + "' and taskresult ='" + resultFlag + "'";
                dtResponse = objDataAccess1.GetDataTableFromAdapter(qrGetToTaskID);

               
                for (int i = 0; i < dtResponse.Rows.Count; i++)
                {
                    strToTaskID = strToTaskID + dtResponse.Rows[i][0].ToString() + ",";                  
                }
                strToTaskID = strToTaskID.Remove(strToTaskID.Length - 1);

              
                if (float.Parse(fromTaskID) < 11)
                {

                    for (int i = 0; i < dtResponse.Rows.Count; i++)
                    {
                        query3 = "insert into SL_ASSIGNEDTASK (ASSIGNID, rel_jobid, REL_TASKID, TASKOWNERROLE)" +
                                    " values ((select NVL(max(ASSIGNID),0)+1 from sl_assignedtask),TO_NUMBER(" + jobId + "), '" +
                                    dtResponse.Rows[i][0].ToString() + "'," +
                                    " (select taskownerrole from sl_taskconfig where taskid = '" + dtResponse.Rows[i][0].ToString() + "'))";
                        strQuery.Add(query3);                       

                    }

                }
               
                DataAccessObjects objDataAccess2 = new DataAccessObjects(CONNECTION_STRING);
                isInsertedTransAssignTb = objDataAccess2.InsertData(strQuery);
              


                if (isInsertedTransAssignTb)
                {
                 
                    if (float.Parse(fromTaskID) == 11)
                    {
                        DateTime todayDate = new DateTime();
                        todayDate = DateTime.Now;

                        string qrUpdate = "Update SL_JOB set JOBSTATUS = 'COMPLETED', enddate = to_date('" + todayDate + "','mm/dd/yy hh:mi:ss AM') where JOBID = " + jobId;
                        DataAccessObjects objDataAccess3 = new DataAccessObjects(CONNECTION_STRING);
                        isInsertedTransAssignTb = objDataAccess3.InsertData(qrUpdate);
                      
                    }
                    if (isInsertedTransAssignTb)
                    {
                        if (isJobInitiated)
                        {
                            from_TaskID = fromTaskID;
                            to_TaskID = strToTaskID;
                            user_LANID = userLanId;
                        }
                        else
                        {
                            sendMail(jobId, fromTaskID, strToTaskID, comments, userLanId);
                        }
                    }
                       
                    
                }
                logger.Info("Data Insertion for Job Transaction done.");
            }
            catch (Exception ex) {
                isInsertedTransAssignTb = false;
                logger.Error("Error in function TransAssignedInsertion:", ex);
                throw ex;
               
            }
                

            return isInsertedTransAssignTb;
        }

        public void sendMail(string strJobID, string fromTaskID, string toTaskID, string comments, string userLanId)
        {
            DataTable dtToMembers = new DataTable();
            DataTable dtCCMembers = new DataTable();
            DataTable dtAdminMembers = new DataTable();
            string strToEmailID = string.Empty;
            string strCCEmailID = string.Empty;
            string strContent = string.Empty;

            try
            {
                logger.Info("Getting To & CC emailIDs.");
                DataAccessObjects objDataAccess = new DataAccessObjects(CONNECTION_STRING);
               
                string queryToMembers = "select distinct members from sl_role where roleid in (select taskownerrole from sl_taskconfig where taskid IN(" + toTaskID + "))";
                dtToMembers = objDataAccess.GetDataTableFromAdapter(queryToMembers);
                
                DataAccessObjects objDataAccess2 = new DataAccessObjects(CONNECTION_STRING);
               
                string queryFromMembers = "select distinct a.members,b.taskcoowner from sl_role a right join (select taskcoowner, taskownerrole from" +                                          " sl_taskconfig where taskid IN(" + fromTaskID + ") and taskcoowner is not null) b on(a.roleid = b.taskownerrole)";
                dtCCMembers = objDataAccess2.GetDataTableFromAdapter(queryFromMembers);


                DataAccessObjects objDataAccess3 = new DataAccessObjects(CONNECTION_STRING);
                string qrUserRoleId = "select members, roleid from sl_role where upper(Members) like '%" + userLanId.ToUpper() + "%'";
                dtAdminMembers = objDataAccess3.GetDataTableFromAdapter(qrUserRoleId);

                for (int i = 0; i < dtToMembers.Rows.Count; i++)
                {
                    for (int j = 0; j < dtToMembers.Rows[i][0].ToString().Split(',').Length; j++)
                    {
                        strToEmailID = strToEmailID + dtToMembers.Rows[i][0].ToString().Split(',')[j] + "@pge.com;";
                    }                                                 

                }

                for (int i = 0; i < dtCCMembers.Rows.Count; i++)
                {
                    for (int j = 0; j < dtCCMembers.Rows[i][0].ToString().Split(',').Length; j++)
                    {
                        strCCEmailID = strCCEmailID + dtCCMembers.Rows[i][0].ToString().Split(',')[j] + "@pge.com;";
                    }
                    if (fromTaskID == "11")
                    {
                        for (int j = 0; j < dtCCMembers.Rows[i][1].ToString().Split(',').Length; j++)
                        {
                            strCCEmailID = strCCEmailID + dtCCMembers.Rows[i][1].ToString().Split(',')[j] + "@pge.com;";
                        }
                    }
                  
                }

                for (int i = 0; i < dtAdminMembers.Rows.Count; i++)
                {
                    if (dtAdminMembers.Rows[i][1].ToString() == "7")
                    {
                        for (int j = 0; j < dtAdminMembers.Rows[i][0].ToString().Split(',').Length; j++)
                        {
                            strCCEmailID = strCCEmailID + dtAdminMembers.Rows[i][0].ToString().Split(',')[j] + "@pge.com;";
                        }
                    }
                }


                string strMailSubject = ConfigurationManager.AppSettings["MailSubject"];
                strMailSubject = String.Format(strMailSubject, strJobID);
                if (fromTaskID == "11")
                {
                 strContent = ConfigurationManager.AppSettings["MailBodyLastTask"]; 
                }else{
                  strContent = ConfigurationManager.AppSettings["MailBody"]; 
                }
              
                strContent = String.Format(strContent, strJobID, comments); // add comments
                string htmlText = "<html><div style='font-size:16px; font-family:Calibri;'>" + strContent + "</div></html>";
               
                logger.Info("Sending Mail.");
                EmailService.Send(mail =>
                {
                    mail.From = ConfigurationManager.AppSettings["MailFrom"];
                    mail.FromDisplayName = ConfigurationManager.AppSettings["MailDisplayName"];                   
                    mail.To = strToEmailID;
                    mail.CC = strCCEmailID;                  
                    mail.Subject = strMailSubject;
                    mail.BodyText = htmlText;
                });
                logger.Info("Mail sent.");
            }catch(Exception ex){
                logger.Error("Error in function sendMail:", ex);
                throw ex;
            }
        }
    }
}