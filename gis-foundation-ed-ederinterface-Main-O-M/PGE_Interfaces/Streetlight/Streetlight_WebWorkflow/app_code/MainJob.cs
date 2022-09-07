using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Configuration;
using Newtonsoft.Json;
using System.Web.UI;
using System.Data;
using log4net;
using log4net.Config;

namespace PGE.Streetlight
{
    /// <summary>
    /// Summary description for MainJob
    /// </summary>
    public class MainJob
    {
        private static string CONNECTION_STRING = ConfigurationManager.AppSettings["CONNECTION_STRING"];
        private static readonly ILog logger = LogManager.GetLogger(typeof(MainJob));

        public MainJob()
        {
            XmlConfigurator.Configure();
        }

        public string getMainJobData(HttpContext context)
        {
            string strResponse = string.Empty;
            try
            {
                System.Collections.Specialized.NameValueCollection parameters = new System.Collections.Specialized.NameValueCollection();
                context.Response.ContentType = "text/plain";
                parameters = context.Request.Form;
                string jobId = parameters["jobID"];
                string taskId = parameters["taskID"];
                string callingFlag = parameters["callingFlag"];
                
                DataSet dsMainJobData = new DataSet();
                if (callingFlag == "histGrid")
                {
                    dsMainJobData = getColorData(jobId);
                    strResponse = JsonConvert.SerializeObject(dsMainJobData, Formatting.Indented);
                }
                else if (callingFlag == "WFSteps")
                {

                    dsMainJobData = getTransComments(jobId, taskId);
                    strResponse = JsonConvert.SerializeObject(dsMainJobData, Formatting.Indented);

                }
            }
            catch(Exception ex)
            {
                logger.Error("Error in function getMainJobData:", ex);
            }
           
            return strResponse;
        }
        public DataSet getTransComments(string jobId, string taskId)
        {
            DataSet dsMainJobData = new DataSet();
            logger.Info("Get Tranasction Comments");
            try
            {
                DataTable dtResponse = new DataTable();
                DataTable dtResponse2 = new DataTable();
                DataTable dtResponse3 = new DataTable();
                DataAccessObjects objDataAccess = new DataAccessObjects(CONNECTION_STRING);
                string query = string.Empty;
                query = "select * from (select t1.comments, t1.RESULT,t1.TXDATA from sl_transaction t1" +
      " where t1.REL_JOBID ='" + jobId + "' AND t1.TASKID ='" + taskId + "' ORDER BY TXDATE DESC) where rownum =1";


                dtResponse = objDataAccess.GetDataTableFromAdapter(query);

                string query2 = "select t2.REL_TASKID as AssignedTask, t2.TASKOWNERROLE from sl_assignedtask t2 where t2.REL_JOBID = '" + jobId + "' AND t2.REL_TASKID='" + taskId + "'";
                dtResponse2 = objDataAccess.GetDataTableFromAdapter(query2);

                string query3 = "select max(rel_taskid) as MAXTASKID from sl_assignedtask where rel_jobid='" + jobId + "'";
                dtResponse3 = objDataAccess.GetDataTableFromAdapter(query3);

                dsMainJobData.Tables.Add(dtResponse);
                dsMainJobData.Tables[0].TableName = "COMPLETEDTASK";
                dsMainJobData.Tables.Add(dtResponse2);
                dsMainJobData.Tables[1].TableName = "ACTIVETASK";
                dsMainJobData.Tables.Add(dtResponse3);
                dsMainJobData.Tables[2].TableName = "MAXASSIGNEDTASK";

            }
            catch(Exception ex)
            {
                logger.Error("Error in GetTransComments: ", ex);
                throw ex;
            }
            return dsMainJobData;

        }
        public DataSet getColorData(string jobId)
        {
            DataSet dsMainJobData = new DataSet();          
            logger.Info("Get color Data");
            try
            {
                DataTable dtResponse1 = new DataTable();
                DataTable dtResponse2 = new DataTable();
                DataAccessObjects objDataAccess = new DataAccessObjects(CONNECTION_STRING);
                string query1 = "Select tc.taskid,tr.result, tr.COMMENTS from sl_taskconfig  tc, sl_transaction  tr where tc.taskid = tr.taskid and tr.rel_jobid = " + jobId;
                dtResponse1 = objDataAccess.GetDataTableFromAdapter(query1);
                string query2 = "Select rel_taskid from sl_assignedtask where rel_jobid= " + jobId;
                dtResponse2 = objDataAccess.GetDataTableFromAdapter(query2);

                dsMainJobData.Tables.Add(dtResponse1);
                dsMainJobData.Tables[0].TableName = "COMPLETEDTASK";
                dsMainJobData.Tables.Add(dtResponse2);
                dsMainJobData.Tables[1].TableName = "ACTIVETASK";
            }
            catch(Exception ex)
            {
                logger.Error("Error in getColorData: ", ex);
                throw ex;
            }
            return dsMainJobData;               
        }
    }
}