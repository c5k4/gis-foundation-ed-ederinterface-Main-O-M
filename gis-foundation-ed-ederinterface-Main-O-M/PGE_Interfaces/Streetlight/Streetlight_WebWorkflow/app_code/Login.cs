/**************************************************************************************************************
Project            :ETGIS Viewer Application
Module             :Login Module 
File Name          :Login.cs
File Description   :This file is used for  getting the user authorization using active directory management
Author             :Tata Consultancy Services
Date of Creation   :25/06/2014
Company            :TCS
Version            :1.0.0
**************************************************************************************************************/
/*Update Log
Date Modified    Changed By        Description

**************************************************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using log4net;
using log4net.Config;
using System.Configuration;
using System.Collections;
using System.Data;
using System.Web.Script.Serialization;
using System.Data.OracleClient;
using Newtonsoft.Json;

namespace PGE.Streetlight
{
    /// <summary>
    /// Summary description for Login
    /// </summary>
    public class Login
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(Login));
        private static string CONNECTION_STRING = ConfigurationManager.AppSettings["CONNECTION_STRING"];
        DataTable dtError = new DataTable();

        public Login()
        {
            XmlConfigurator.Configure();
        }

        
        public string getLanID(HttpContext context)
        {
            string strUserName = "";
            try
            {
                logger.Info("Get LanID");
                string strFullUserName = string.Empty;
                System.Security.Principal.IPrincipal userPrincipal = context.User;
                System.Security.Principal.WindowsIdentity userIdentity = userPrincipal.Identity as System.Security.Principal.WindowsIdentity;
                strFullUserName = userIdentity.Name;
                strFullUserName = strFullUserName.ToLower();
                
                strUserName = strFullUserName.Split('\\')[1];
            }
            catch (Exception ex)
            {
                logger.Error("Error in function getLanID:", ex);
                throw ex;
            }
            return strUserName;

        }
        /// <summary>
        /// Validates and get logged in user info
        /// </summary>
        /// <param name="context">HttpContext context</param>
        /// <returns>User details as serialised string</returns>
        public string ValidateAndGetLoginInfo(HttpContext context)
        {
            string strResponse = string.Empty;
            string strUserName = string.Empty;
            string roleID = string.Empty;
            DataTable dtRole;
            DataSet dsJobData = new DataSet();
            DataTable dtDashboard = new DataTable();
            DataTable dtJobHistory = new DataTable();
            logger.Info("Appliation Launched");
            System.Web.Script.Serialization.JavaScriptSerializer ser = new System.Web.Script.Serialization.JavaScriptSerializer();
           
            try
            {
                logger.Info("Get Assigned Job and History as per user details");
                strUserName = getLanID(context);
                logger.Info("For User:" + strUserName);
                dtRole = UserAuthentication(strUserName);
                if (dtRole.Rows.Count > 0)
                {
                    foreach (DataRow row in dtRole.Rows)
                    {
                        roleID = roleID+ "'" + row[0].ToString() +"',";
                    }
                    roleID = roleID.Remove(roleID.Length - 1);                 
                    logger.Info("Get Assigned Task for :"+ strUserName);
                    dtDashboard = getDashboardTable(roleID);
                    if (dtDashboard.Rows.Count > 0)
                    {
                      
                        dsJobData.Tables.Add(dtRole);
                        dsJobData.Tables[0].TableName = "UserRole";

                        System.Data.DataColumn lanIDColumn = new System.Data.DataColumn("LANID", typeof(System.String));
                        lanIDColumn.DefaultValue = strUserName;
                        dtDashboard.Columns.Add(lanIDColumn);
                        logger.Info("Get Job History ");
                        dtJobHistory = getJobHistory();
                        

                        dsJobData.Tables.Add(dtDashboard);
                        dsJobData.Tables[1].TableName = "Dashboard";
                        dsJobData.Tables.Add(dtJobHistory);
                        dsJobData.Tables[2].TableName = "JobHistory";
                        
                    }
                    else
                    {
                        string strMessage = ConfigurationManager.AppSettings["MSG01"];
                        logger.Info("No Jobs : " + strMessage);
                        dtDashboard = dtRole;
                        System.Data.DataColumn jobCount = new System.Data.DataColumn("JOBCOUNT", typeof(System.String));
                        jobCount.DefaultValue = "0";
                        dtDashboard.Columns.Add(jobCount);
                        System.Data.DataColumn lanIDColumn = new System.Data.DataColumn("LANID", typeof(System.String));
                        lanIDColumn.DefaultValue = strUserName;
                        dtDashboard.Columns.Add(lanIDColumn);
                        logger.Info("Get Job History ");
                        dtJobHistory = getJobHistory();
                       

                        dsJobData.Tables.Add(dtDashboard);
                        dsJobData.Tables[0].TableName = "Dashboard";
                        dsJobData.Tables.Add(dtJobHistory);
                        dsJobData.Tables[1].TableName = "JobHistory";
                        System.Data.DataColumn roleId = new System.Data.DataColumn("ROLEID", typeof(System.String));
                        roleId.DefaultValue = dtRole.Rows[0][0].ToString();
                        dtDashboard.Columns.Add(roleId);

                    }                               
                    
                }
                else
                {
                    string strMessage = ConfigurationManager.AppSettings["MSG02"];
                    logger.Info("Invalid user : " + strMessage);
                    dtDashboard = getErrorTable(strMessage);
                    dsJobData.Tables.Add(dtDashboard);
                    dsJobData.Tables[0].TableName = "Dashboard";
                }

            }
            catch (Exception ex)
            {
                string strADResponse = System.Configuration.ConfigurationManager.AppSettings["MSG08"] + "." + strUserName;
                logger.Error(strADResponse, ex);
                dtDashboard = getErrorTable(strADResponse);
                dsJobData.Tables.Add(dtDashboard);
                dsJobData.Tables[0].TableName = "Dashboard";
            }
          
            strResponse = JsonConvert.SerializeObject(dsJobData, Formatting.Indented);
            return strResponse;
        }

        private DataTable UserAuthentication(string userLanId)
        {
            DataTable dtResponse = new DataTable();
            try
            {
                logger.Info("Get User Role of : " + userLanId);
                DataAccessObjects objDataAccess = new DataAccessObjects(CONNECTION_STRING);
                string query = "select ROLEID as ROLE, ROLENAME from SL_ROLE where upper(Members) like '%" + userLanId.ToUpper() + "%'";              
                dtResponse = objDataAccess.GetDataTableFromAdapter(query);
            }
            catch(Exception ex)
            {
                logger.Error("Error in Function UserAuthentication ", ex);
                throw ex;;
            }
            return dtResponse;
        }

        private DataTable getDashboardTable(string strRoleID)
        {
            DataTable dtResponse = new DataTable();
            try
            {
                DataAccessObjects objDataAccess = new DataAccessObjects(CONNECTION_STRING);
                string query = string.Empty;
                logger.Info("For RoleID : " + strRoleID);

                if (strRoleID == "'7'")
                {
                    query = "select row_number() OVER (ORDER BY a.Rel_jobid) as ID, a.*,b.jobdesc from (select a.*,b.lanid as SUBMITTER,to_char(b.txdate, 'MM-DD-YY HH12:MI:SS AM') as txdate  from " +
                           "(select a.*,b.roledesc,b.rolename from (select a.*,b.TASKNAME, b.TASKDESC from " +
                           "(select distinct t1.rel_taskid,t1.rel_jobid,taskownerrole as ROLEID from sl_assignedtask t1)a left join " +
                           "sl_taskconfig b on (a.rel_taskid = b.taskid))a left join sl_role b on(a.ROLEID = b.roleid))a left join " +
                           "(select * from sl_transaction where taskid = 1 and lanid is not null) b on(a.rel_jobid = b.rel_jobid)) a left join sl_job b on(a.rel_jobid = b.jobid)";
                }
                else
                {

                    query = "select row_number() OVER (ORDER BY a.Rel_jobid) as ID, a.*,b.jobdesc from (select a.*,b.lanid as SUBMITTER,to_char(b.txdate, 'MM-DD-YY HH12:MI:SS AM') as txdate  from " +
                        "(select a.*,b.roledesc,b.rolename from (select a.*,b.TASKNAME, b.TASKDESC from " +
                        "(select distinct t1.rel_taskid,t1.rel_jobid,taskownerrole as ROLEID from sl_assignedtask t1 where taskownerrole in (" + strRoleID + "))a left join " +
                        "sl_taskconfig b on (a.rel_taskid = b.taskid))a left join sl_role b on(a.ROLEID = b.roleid))a left join " +
                        "(select * from sl_transaction where taskid = 1 and lanid is not null) b on(a.rel_jobid = b.rel_jobid)) a left join sl_job b on(a.rel_jobid = b.jobid)";
                }
               
                dtResponse = objDataAccess.GetDataTableFromAdapter(query);
            }
            catch(Exception ex)
            {
                logger.Error("Error in Function GetDashBoardTable : ", ex);
                throw ex;
            }
            return dtResponse;
        }

        private DataTable getJobHistory()
        {
            DataTable dtResponse = new DataTable();            
            try
            {
                DataAccessObjects objDataAccess = new DataAccessObjects(CONNECTION_STRING);

                string query = "select row_number() OVER (ORDER BY t1.jobid) as ID, t1.jobid as rel_jobid, t1.jobdesc, t1.jobstatus, to_char(t1.startdate, 'MM-DD-YY HH12:MI:SS AM') as startdate, to_char( t1.enddate, 'MM-DD-YY HH12:MI:SS AM') as enddate, t2.LANID as SUBMITTER from sl_job t1, sl_transaction t2 where t1.jobid = t2.rel_jobid and t2.taskid = '1' and t2.result ='Y'";

                dtResponse = objDataAccess.GetDataTableFromAdapter(query);
            }
            catch(Exception ex)
            {
                logger.Error("Error in getJobHistory: ", ex);
                throw ex;
            }
            return dtResponse;
        }

        private DataTable getErrorTable(string errMsg)
        {
            logger.Info("Print Error Message");
            DataTable errTable = new DataTable();
            try
            {
                errTable.Columns.Add("ErrorMsg");
                DataRow errMsgRow = errTable.NewRow();
                errMsgRow["ErrorMsg"] = errMsg;
                errTable.Rows.Add(errMsgRow);
            }
            catch(Exception ex)
            {
                logger.Error("Error in function getErrorTable:", ex);
                throw ex;
            }
            return errTable;
        }

        

    }
}
