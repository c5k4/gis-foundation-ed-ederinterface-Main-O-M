using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Data;
using PGE.BatchApplication.Version_report.Email;
using PGE.BatchApplication.Version_report.DBHelper;
using System.IO;
using System.Reflection;


namespace PGE.BatchApplication.Version_report.Email
{
     class Program
    {
       // public static Log4NetLogger _log = new Log4NetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, "pge.log4net.config");
        static void Main(string[] args)
        {
            createLogFile();
            WriteLine("START");
          
            GetData( );

        }

        public static void GetData( )
        {
            try
            {
                WriteLine("Collecting Data for Report...");
                bool check = cls_DBHelper_For_EDER.ExecuteStoredProcedureCommand("SDE.common_ancestor_exe", null);

                if (!check)
                    WriteLine("Error in Executing Procedure");

                string strquery = "SELECT state_id, lineage_name FROM sde.states WHERE state_id = (SELECT state_id FROM sde.versions WHERE name = 'DEFAULT' AND owner = 'SDE') ";

                DataTable DT = cls_DBHelper_For_EDER.GetDataTableByQuery(strquery);
                string q_outstanding_Sessions = "SELECT COUNT(1) AS DEPTH FROM SDE.STATE_LINEAGES WHERE LINEAGE_NAME IN (SELECT LINEAGE_NAME FROM SDE.STATES WHERE STATE_ID IN (SELECT STATE_ID FROM SDE.VERSIONS WHERE NAME = 'DEFAULT'))";
                DataTable DT_OS = cls_DBHelper_For_EDER.GetDataTableByQuery(q_outstanding_Sessions);
                string count_sessions = DT_OS.Rows[0]["DEPTH"].ToString();

                string q_outstanding_Versions = "SELECT COUNT(1) FROM SDE.STATES";
                DataTable DT1 = cls_DBHelper_For_EDER.GetDataTableByQuery(q_outstanding_Versions);
                string count_versions = DT1.Rows[0]["COUNT(1)"].ToString();

                string q_delta_Table = "select round(sum(a_d),2) as round from (select sum(num_rows)/1000000 a_d from dba_tables where " +
                "table_name in (select 'A'||registration_id from sde.table_registry) union all select sum(num_rows)/1000000 a_d from dba_tables where table_name in (select 'D'||registration_id from sde.table_registry))";

                DataTable DT2 = cls_DBHelper_For_EDER.GetDataTableByQuery(q_delta_Table);
                string count_versions1 = DT2.Rows[0]["round"].ToString();


                string q_state_Lineage_Tree = "select round(count(*)/1000000,2) from sde.state_lineages";

                DataTable DT3 = cls_DBHelper_For_EDER.GetDataTableByQuery(q_state_Lineage_Tree);
                string count_versions2 = DT3.Rows[0]["ROUND(COUNT(*)/1000000,2)"].ToString();

                //Table 2 input


                // string q_blocker_Version = "select SESSIONID, Current_Owner, session_Name, Created from (select v.OWNER|| '.'||v.name as SESSIONID, "+
                // "p.current_owner as Current_Owner, trim(p.session_name) as session_Name,p.create_date as Created  from "+
                // "process.mm_session p, sde.version_order v where to_number(regexp_replace(substr(v.name, 4), '[^0-9]+', '')) = p.session_id and length(v.OWNER)=4 order by v.ca_state_id asc, v.state_id asc) where rownum < 21 ";



                string html = "<div align='center'>";
                html += "<p font-size: 25pt><B><U>Electric Distribution Version Statistics Report</U></B></p>";
                html += "</div>";
                html += "<p font-size: 16pt ><B>General Guidlines:</B></p>";
                html += "<p font-size: 12pt >Please <a href='file:\\\\SFSHARE04-NAS2\\sfgispoc_data\\ApplicationDevelopment\\TCSDeliverables\\ESRI_Guidelines\\Enterprise_GIS_Performance_Runbook_V3.pdf'>click here </a> for detailed ESRI Version Management Guide Lines.</p>";
                html += "<p font-size: 16pt ><B>SUMMARY:</B></p>";
                html += "<table style='border: 1px solid #ccc;font-size: 9pt;font-family:arial'>";

                //Adding HeaderRow.
                string border_style = "style='width:120px;border: 1px solid #ccc'";
                html += "<tr>";
                html += "<th bgcolor='#add8e6' " + border_style + ">Item	</th>";
                html += "<th bgcolor='#add8e6' " + border_style + ">Healthy Value</th>";
                html += "<th  bgcolor='#add8e6' " + border_style + ">Alert Value</th>";
                html += "<th bgcolor='#add8e6' " + border_style + ">Max Value</th>";
                html += "<th bgcolor='#add8e6' " + border_style + " >Current Condition</th>";
                html += "<th  bgcolor='#add8e6' " + border_style + ">Action Item</th>";
                html += "</tr>";

                string subject_change = " ";
                string class_td = "";
                string class_td_1 = "";
                int flag = -1;
                string action = "-";
                string action_sessions = "-";
                string action_row2 = "-";
                string action_row3 = "-";

                //first row
                //Fix : 20-Nov-2020
                if (Convert.ToInt32(count_versions) > Convert.ToInt16(ConfigurationManager.AppSettings["MAX_VALUE_SESSION_VERSION"]))
                {
                    class_td = ConfigurationManager.AppSettings["MAX_COLOR_SESSION_VERSION"]; //red
                    action = "Orphaned Sessions and Versions should be deleted daily";
                    flag = 1;
                }
                else if (Convert.ToInt32(count_versions) >= Convert.ToInt16(ConfigurationManager.AppSettings["HEALTHY_VALUE_SESSION_VERSION"]) && Convert.ToInt32(count_versions) < Convert.ToInt16(ConfigurationManager.AppSettings["ALERT_VALUE_SESSION_VERSION"]))
                {
                    class_td = ConfigurationManager.AppSettings["HEALTHY_ALERT_COLOR_SESSION_VERSION"]; //orrange
                    flag = 0;
                }

                else if (Convert.ToInt32(count_versions) >= Convert.ToInt16(ConfigurationManager.AppSettings["ALERT_VALUE_SESSION_VERSION"]) && Convert.ToInt32(count_versions) < Convert.ToInt16(ConfigurationManager.AppSettings["MAX_VALUE_SESSION_VERSION"]))
                {
                    class_td = ConfigurationManager.AppSettings["ALERT_MAX_COLOR_SESSION_VERSION"]; //yellow
                    flag = 0;
                }
                else if (Convert.ToInt32(count_versions) < Convert.ToInt16(ConfigurationManager.AppSettings["HEALTHY_VALUE_SESSION_VERSION"]))
                {
                    class_td = ConfigurationManager.AppSettings["HEALTHY_COLOR_SESSION_VERSION"]; //green

                }

                //First(II) row
                if (Convert.ToInt32(count_sessions) > Convert.ToInt32(ConfigurationManager.AppSettings["MAX_VALUE_SESSION"]))
                {
                    class_td_1 = ConfigurationManager.AppSettings["MAX_COLOR_SESSION"]; //red
                    action_sessions = "Orphaned Sessions and Versions should be deleted daily";
                    flag = 1;
                }
                else if (Convert.ToInt32(count_sessions) >= Convert.ToInt32(ConfigurationManager.AppSettings["HEALTHY_VALUE_SESSION"]) && Convert.ToInt32(count_sessions) < Convert.ToInt32(ConfigurationManager.AppSettings["ALERT_VALUE_SESSION"]))
                {
                    class_td_1 = ConfigurationManager.AppSettings["HEALTHY_ALERT_COLOR_SESSION"]; //orrange
                    flag = 0;
                }

                else if (Convert.ToInt32(count_sessions) >= Convert.ToInt32(ConfigurationManager.AppSettings["ALERT_VALUE_SESSION"]) && Convert.ToInt32(count_sessions) < Convert.ToInt32(ConfigurationManager.AppSettings["MAX_VALUE_SESSION"]))
                {
                    class_td_1 = ConfigurationManager.AppSettings["ALERT_MAX_COLOR_SESSION"]; //yellow
                    flag = 0;
                }
                else if (Convert.ToInt32(count_sessions) < Convert.ToInt32(ConfigurationManager.AppSettings["HEALTHY_VALUE_SESSION"]))
                {
                    class_td_1 = ConfigurationManager.AppSettings["HEALTHY_COLOR_SESSION"]; //green

                }

                //second row

                string class_td1 = "";
                if (Convert.ToDouble(count_versions1) > Convert.ToDouble(ConfigurationManager.AppSettings["MAX_VALUE_DELTA_TABLE"]))
                {
                    class_td1 = ConfigurationManager.AppSettings["MAX_COLOR_DELTA_TABLE"];
                    action_row2 = "Top Blocking versions should be targeted for posting.";
                    flag = 1;
                }
                else if (Convert.ToDouble(count_versions1) >= Convert.ToDouble(ConfigurationManager.AppSettings["HEALTHY_VALUE_DELTA_TABLE"]) && Convert.ToDouble(count_versions1) < Convert.ToDouble(ConfigurationManager.AppSettings["ALERT_VALUE_DELTA_TABLE"]))
                {
                    class_td1 = ConfigurationManager.AppSettings["HEALTHY_ALERT_COLOR_DELTA_TABLE"];
                    flag = 0;
                }

                else if (Convert.ToDouble(count_versions1) >= Convert.ToDouble(ConfigurationManager.AppSettings["ALERT_VALUE_DELTA_TABLE"]) && Convert.ToDouble(count_versions1) < Convert.ToDouble(ConfigurationManager.AppSettings["MAX_VALUE_DELTA_TABLE"]))
                {
                    class_td1 = ConfigurationManager.AppSettings["ALERT_MAX_COLOR_DELTA_TABLE"];
                    flag = 0;
                }
                else if (Convert.ToDouble(count_versions1) < Convert.ToDouble(ConfigurationManager.AppSettings["HEALTHY_VALUE_DELTA_TABLE"]))
                {
                    class_td1 = ConfigurationManager.AppSettings["HEALTHY_COLOR_DELTA_TABLE"];
                }


                //third row
                string class_td2 = "";
                if (Convert.ToDouble(count_versions2) > Convert.ToDouble(ConfigurationManager.AppSettings["MAX_VALUE_STATE_LINEAGE"]))
                {
                    class_td2 = ConfigurationManager.AppSettings["MAX_COLOR_STATE_LINEAGE"];
                    action_row3 = "Reconciling and Posting older versions followed by compress may reduce this count.";
                    flag = 1;
                }
                else if (Convert.ToDouble(count_versions2) >= Convert.ToDouble(ConfigurationManager.AppSettings["HEALTHY_VALUE_STATE_LINEAGE"]) && Convert.ToDouble(count_versions2) < Convert.ToDouble(ConfigurationManager.AppSettings["ALERT_VALUE_STATE_LINEAGE"]))
                {
                    class_td2 = ConfigurationManager.AppSettings["HEALTHY_ALERT_COLOR_STATE_LINEAGE"];
                    flag = 0;

                }

                else if (Convert.ToDouble(count_versions2) >= Convert.ToInt32(ConfigurationManager.AppSettings["ALERT_VALUE_STATE_LINEAGE"]) && Convert.ToDouble(count_versions2) < Convert.ToDouble(ConfigurationManager.AppSettings["MAX_VALUE_STATE_LINEAGE"]))
                {
                    class_td2 = ConfigurationManager.AppSettings["ALERT_MAX_COLOR_STATE_LINEAGE"];
                    flag = 0;
                }
                else if (Convert.ToDouble(count_versions2) < Convert.ToDouble(ConfigurationManager.AppSettings["HEALTHY_VALUE_STATE_LINEAGE"]))
                {
                    class_td2 = ConfigurationManager.AppSettings["HEALTHY_COLOR_STATE_LINEAGE"];
                }

                if (flag == 1)
                {
                    subject_change = "Alert - ";

                }
                else if (flag == 0)
                {
                    subject_change = "Warning - ";
                }
                else
                {
                    subject_change = " ";

                }


                //Adding DataRow.
                html += "<tr>";
                html += "<td " + border_style + ">" + "GDB – State Count" + "</td>";
                html += "<td " + border_style + ">" + "<" + ConfigurationManager.AppSettings["HEALTHY_VALUE_SESSION_VERSION"] + "</td>";
                html += "<td " + border_style + ">" + ">" + ConfigurationManager.AppSettings["ALERT_VALUE_SESSION_VERSION"] + "</td>";
                html += "<td " + border_style + ">" + ">" + ConfigurationManager.AppSettings["MAX_VALUE_SESSION_VERSION"] + "</td>";
                html += "<td " + border_style + " " + class_td + ">" + count_versions + "</td>";
                html += "<td " + border_style + ">" + action + "</td>";

                html += "<tr>";
                html += "<td " + border_style + ">" + "GDB - Lineage Depth" + "</td>";
                html += "<td " + border_style + ">" + "<" + ConfigurationManager.AppSettings["HEALTHY_VALUE_SESSION"] + "</td>";
                html += "<td " + border_style + ">" + ">" + ConfigurationManager.AppSettings["ALERT_VALUE_SESSION"] + "</td>";
                html += "<td " + border_style + ">" + ">" + ConfigurationManager.AppSettings["MAX_VALUE_SESSION"] + "</td>";
                html += "<td " + border_style + " " + class_td_1 + ">" + count_sessions + "</td>";
                html += "<td " + border_style + ">" + action_sessions + "</td>";
                html += "</tr>";


                html += "</tr>";
                html += "<tr>";
                html += "<td " + border_style + ">" + "GDB Delta Table (A/D) Size" + "</td>";
                html += "<td " + border_style + ">" + "<" + ConfigurationManager.AppSettings["HEALTHY_VALUE_DELTA_TABLE"] + "M</td>";
                html += "<td " + border_style + ">" + ">" + ConfigurationManager.AppSettings["ALERT_VALUE_DELTA_TABLE"] + "M</td>";
                html += "<td " + border_style + ">" + ">" + ConfigurationManager.AppSettings["MAX_VALUE_DELTA_TABLE"] + "M</td>";
                html += "<td " + border_style + " " + class_td1 + ">" + count_versions1 + "M" + "</td>";
                html += "<td " + border_style + ">" + action_row2 + "</td>";


                html += "</tr>";
                html += "</tr>";
                html += "<tr>";
                html += "<td " + border_style + ">" + "GDB – State Lineage Tree" + "</td>";
                html += "<td " + border_style + ">" + "<" + ConfigurationManager.AppSettings["HEALTHY_VALUE_STATE_LINEAGE"] + "K</td>";
                html += "<td " + border_style + ">" + ">" + ConfigurationManager.AppSettings["ALERT_VALUE_STATE_LINEAGE"] + "M</td>";
                html += "<td " + border_style + ">" + ">" + ConfigurationManager.AppSettings["MAX_VALUE_STATE_LINEAGE"] + "M</td>";
                html += "<td " + border_style + " " + class_td2 + ">" + count_versions2 + "M" + "</td>";
                html += "<td " + border_style + ">" + action_row3 + "</td>";
                html += "</tr>";



                //Table end.
                html += "</table>";

                string o_Session_Version = "SELECT count(*) from  process.mm_session p where  length(p.CREATE_USER)=4";
                DataTable DT_Outstanding_SV = cls_DBHelper_For_EDER.GetDataTableByQuery(o_Session_Version);
                string o_Session = "select count(*) from sde.versions";
                DataTable DT_Outstanding_S = cls_DBHelper_For_EDER.GetDataTableByQuery(o_Session);

                //Table Start
                html += "<p font-size: 16pt ><b>Outstanding System versions+Sessions/Sessions:</b></p>";
                html += "<table style='border: 1px solid #ccc;font-size: 9pt;font-family:arial'>";
                string border_style_new = "style='width:400px;border: 1px solid #ccc'";
                //Adding HeaderRow.
                html += "<tr>";
                html += "<th bgcolor='#add8e6' " + border_style_new + ">Item	</th>";
                html += "<th bgcolor='#add8e6' " + border_style + ">Value</th>";
                html += "</tr>";

                html += "<tr>";
                html += "<td " + border_style_new + ">" + "GDB – Outstanding Versions(System versions+ Sessions)" + "</td>";
                html += "<td " + border_style + ">" + DT_Outstanding_SV.Rows[0]["COUNT(*)"].ToString() + "</td>";

                html += "<tr>";
                html += "<td " + border_style_new + ">" + "GDB – Outstanding Sessions" + "</td>";
                html += "<td " + border_style + ">" + DT_Outstanding_S.Rows[0]["COUNT(*)"].ToString() + "</td>";

                //Table end.
                html += "</table>";

                string q_Version = "select SESSIONID, Current_Owner, session_Name, Created from (  select v.OWNER|| '.'||v.name as SESSIONID," +
                    " p.current_owner as Current_Owner, trim(p.session_name) as session_Name,p.create_date as Created  from  process.mm_session p," +
                    " sde.version_orders v where to_number(regexp_replace(substr(v.name, 4), '[^0-9]+', '')) = p.session_id and length(v.OWNER)>3  " +
                    "order by v.ca_state_id asc, v.state_id asc) where rownum < 11";

                DataTable DT_Version_Occ = cls_DBHelper_For_EDER.GetDataTableByQuery(q_Version);
                foreach (DataRow row in DT_Version_Occ.Rows)
                {
                    try
                    {
                        string q_update_occ = "update SDE.PGE_BLOCKER_OCC set OCCURRENCE =" +
                            " (select OCCURRENCE from SDE.PGE_BLOCKER_OCC where session_id='" + row["SESSIONID"].ToString() + "')+1 " +
                            "where session_id='" + row["SESSIONID"].ToString()+"'";
                        int up_count = cls_DBHelper_For_EDER.UpdateQuery(q_update_occ);
                        if (up_count == 0)
                        {
                            string q_insert_occ = "insert into SDE.PGE_BLOCKER_OCC VALUES ('" + row["SESSIONID"].ToString() + "','"
                                + row["Current_Owner"].ToString() + "',to_date('"+Convert.ToDateTime(row["CREATED"].ToString()).ToString("MM/dd/yyyy hh:mm:ss tt") + 
                                "','MM/dd/yyyy HH:mi:ss AM'),'" + row["Session_Name"].ToString() + "',1)";
                             cls_DBHelper_For_EDER.ExecuteSpacialQuery(q_insert_occ);
                        }
                        //Delete Cleared version from occurance table
                       

                    }
                    catch (Exception ex)
                    {
                        WriteLine("Error while Insert/Update Occurence" + ex.Message);
                      //  throw;
                    }
                }
                //---------------Changed by YXA6 in Rearch Project--------------------------------------------------------------------
                //string q_delete_occ = "delete from SDE.pge_blocker_occ where session_id not in (select SESSIONID from (select v.OWNER|| '.'||v.name as SESSIONID," +
                //    " p.current_owner as Current_Owner, trim(p.session_name) as session_Name,p.create_date as Created  from process.mm_session p, sde.version_orders v" +
                //    " where to_number(regexp_replace(substr(v.name, 4), '[^0-9]+', '')) = p.session_id and length(v.OWNER)=4 order by v.ca_state_id asc, v.state_id asc ) " +
                //    "where rownum<11)";
                //cls_DBHelper_For_EDER.ExecuteSpacialQuery(q_delete_occ);



                //Table 2
                #region Find Blocker Version for 4 Character USERID
                //string q_blocker_Version = "select SESSION_ID, SESSION_OWNER, session_Name, CREATION_DATE,OCCURRENCE  from SDE.PGE_BLOCKER_OCC";
                string q_blocker_Version = "select SESSION_ID, SESSION_OWNER, session_Name, CREATION_DATE,OCCURRENCE  from SDE.PGE_BLOCKER_OCC where session_id in" +
                    " (select SESSIONID from (select v.OWNER|| '.'||v.name as SESSIONID," +
                   " p.current_owner as Current_Owner, trim(p.session_name) as session_Name,p.create_date as Created  from process.mm_session p, sde.version_orders v" +
                    " where to_number(regexp_replace(substr(v.name, 4), '[^0-9]+', '')) = p.session_id and length(v.OWNER)=4 order by v.ca_state_id asc, v.state_id asc ) " +
                    ") and rownum<11";

                // string Blocker_version = DT4.Rows[0][" "].ToString();

                html += "<p font-size: 16pt ><b>Top 10 blocker versions from Users:</b></p>";
                html += "<table  style='border: 1px solid #ccc;font-size: 9pt;font-family:arial'>";
                //Adding HeaderRow.
                string border_style1 = "style='width:120px;border: 1px solid #ccc'";
                html += "<tr>";
                html += "<th bgcolor='#add8e6' " + border_style1 + " > Blocker Versions </th>";
                html += "<th bgcolor='#add8e6' " + border_style1 + ">Session Owner </th>";
                html += "<th  bgcolor='#add8e6' " + border_style1 + " >Session Name</th>";
                html += "<th bgcolor='#add8e6' " + "style='width:180px;border: 1px solid #ccc'" + " >Creation Date</th>";
                html += "<th  bgcolor='#add8e6' " + border_style1 + ">OCCURRENCE</th>";
                html += "</tr>";


                DataTable DT4 = cls_DBHelper_For_EDER.GetDataTableByQuery(q_blocker_Version);
                foreach (DataRow row in DT4.Rows)
                {
                    html += "<tr>";
                    html += "<td " + border_style1 + ">" + row["SESSION_ID"].ToString() + "</td>";
                    html += "<td " + border_style1 + ">" + row["SESSION_OWNER"].ToString() + "</td>";
                    html += "<td " + border_style1 + ">" + row["session_Name"].ToString() + "</td>";
                    html += "<td " + border_style1 + ">" + row["CREATION_DATE"].ToString() + "</td>";
                    html += "<td " + border_style1 + ">" + row["OCCURRENCE"].ToString() + "</td>";

                    html += "</tr>";


                }


                //Table end
                html += "</table>";


                html += "<p>" + "<b> Note:</b> Blocking Versions may prevent effective compress of other versions resulting into poor application performance.</p>";
                     
                //html += "<p font-size: 16pt>" + "<b>Sessions reconciled in the past 1 day that resulted into Conflicts:</b>" + "</p>";
                #endregion region end -find Blocker version for  4 Character LANID User
                //Table 3 
                #region Find Blocker Version for Non-Users
                //string q_blocker_Version = "select SESSION_ID, SESSION_OWNER, session_Name, CREATION_DATE,OCCURRENCE  from SDE.PGE_BLOCKER_OCC";
                q_blocker_Version = "select SESSION_ID, SESSION_OWNER, session_Name, CREATION_DATE,OCCURRENCE  from SDE.PGE_BLOCKER_OCC where session_id in" +
                    " (select SESSIONID from (select v.OWNER|| '.'||v.name as SESSIONID," +
                   " p.current_owner as Current_Owner, trim(p.session_name) as session_Name,p.create_date as Created  from process.mm_session p, sde.version_orders v" +
                    " where to_number(regexp_replace(substr(v.name, 4), '[^0-9]+', '')) = p.session_id and length(v.OWNER)>4 order by v.ca_state_id asc, v.state_id asc ) " +
                    ")  and rownum<11";

                // string Blocker_version = DT4.Rows[0][" "].ToString();

                html += "<p font-size: 16pt ><b>Top 10 blocker versions from Non-Users:</b></p>";
                html += "<table  style='border: 1px solid #ccc;font-size: 9pt;font-family:arial'>";
                //Adding HeaderRow.
                 border_style1 = "style='width:120px;border: 1px solid #ccc'";
                html += "<tr>";
                html += "<th bgcolor='#add8e6' " + border_style1 + " > Blocker Versions </th>";
                html += "<th bgcolor='#add8e6' " + border_style1 + ">Session Owner </th>";
                html += "<th  bgcolor='#add8e6' " + border_style1 + " >Session Name</th>";
                html += "<th bgcolor='#add8e6' " + "style='width:180px;border: 1px solid #ccc'" + " >Creation Date</th>";
                html += "<th  bgcolor='#add8e6' " + border_style1 + ">OCCURRENCE</th>";
                html += "</tr>";


                DT4 = cls_DBHelper_For_EDER.GetDataTableByQuery(q_blocker_Version);
                foreach (DataRow row in DT4.Rows)
                {
                    html += "<tr>";
                    html += "<td " + border_style1 + ">" + row["SESSION_ID"].ToString() + "</td>";
                    html += "<td " + border_style1 + ">" + row["SESSION_OWNER"].ToString() + "</td>";
                    html += "<td " + border_style1 + ">" + row["session_Name"].ToString() + "</td>";
                    html += "<td " + border_style1 + ">" + row["CREATION_DATE"].ToString() + "</td>";
                    html += "<td " + border_style1 + ">" + row["OCCURRENCE"].ToString() + "</td>";

                    html += "</tr>";


                }


                //Table end
                html += "</table>";


                html += "<p>" + "<b> Note:</b> Blocking Versions may prevent effective compress of other versions resulting into poor application performance.</p>";

                html += "<p font-size: 16pt>" + "<b>Sessions reconciled in the past 1 day that resulted into Conflicts:</b>" + "</p>";
                #endregion region end -find Blocker version for  Non- User

                //Table 4

                            string q_sessions_Reconciled = " select SESSIONID, Current_Owner, session_Name, Created from (" +
                " Select g.version_name as SESSIONID, s.CURRENT_OWNER as Current_Owner,s.SESSION_NAME as session_Name,  s.CREATE_DATE  Created " +
                "from  (  select substr(REGEXP_SUBSTR(VERSION_NAME, '[^.]+', 1, 2), 4) as SESSION_NUM, version_name from SDE.GDBM_RECONCILE_HISTORY " +
                "where  SERVICE_NAME like '%EDER_RECONCILE%' and RECONCILE_START_DT > sysdate - 1 and RECONCILE_RESULT = 'Conflicts' " +
                "group by VERSION_NAME) g inner join (" +
                "select to_char(SESSION_ID) as SESSION_NUM, substr(SESSION_NAME, 1, 40) as SESSION_NAME, CREATE_USER, CURRENT_OWNER, CREATE_DATE, " +
                "substr(regexp_replace(DESCRIPTION, '[[:space:]]', ' '), 1, 80) as DESCRIPTION " +
                "from PROCESS.MM_SESSION where length(CREATE_USER)=4) s " +
                "on g.SESSION_NUM = s.SESSION_NUM order by g.SESSION_NUM )";

                html += "<table >";
                string border_style2 = "style='width:120px;border: 1px solid #ccc'";
                html += "<tr>";
                html += "<th bgcolor='#add8e6' " + border_style2 + ">Session	</th>";
                html += "<th bgcolor='#add8e6' " + border_style2 + ">Owner</th>";
                html += "<th bgcolor='#add8e6' " + border_style2 + ">Session Name</th>";
                html += "<th bgcolor='#add8e6' " + border_style2 + ">Creation Date</th>";

                html += "</tr>";


                DataTable DT5 = cls_DBHelper_For_EDER.GetDataTableByQuery(q_sessions_Reconciled);
                foreach (DataRow row in DT5.Rows)
                {
                    html += "<tr>";
                    html += "<td " + border_style2 + ">" + row["SESSIONID"].ToString() + "</td>";
                    html += "<td " + border_style2 + ">" + row["Current_Owner"].ToString() + "</td>";
                    html += "<td " + border_style2 + ">" + row["session_Name"].ToString() + "</td>";
                    html += "<td " + border_style2 + ">" + row["Created"].ToString() + "</td>";


                    html += "</tr>";


                }
                html += "</table>";

                //Table 4
                string sql_rowCnt = "SELECT count(*)  from  process.mm_session p where  p.create_date < sysdate-30  and length(p.CREATE_USER)=4 ";
                DataTable DT_rowcnt = cls_DBHelper_For_EDER.GetDataTableByQuery(sql_rowCnt);
                string row_cnt = DT_rowcnt.Rows[0][0].ToString();

                html += "<p>" + "<b>Note:</b> Conflicts should be resolved for effective reconcile and compress" + "</br>"
                    + "Open User Sessions older than 30 days:"+row_cnt+" </br>" + "</p>";
                html += "<p font-size: 16pt>" + "<b>Open User Sessions older than 30 days => oldest sessions details:</b>" + "</p>";

                string q_user_Sessions = "  select  SESSIONID,Current_owner,Session_name,create_date  from ( " +
                    "select p.CREATE_USER||'.SN_'||p.Session_ID AS SESSIONID, p.current_owner, " +
    "trim(p.session_name) as session_name, p.create_date " +
      "from  process.mm_session p where " +
               " p.create_date < sysdate-30  and length(p.CREATE_USER)=4 Order by p.create_date ASC) where rownum<11    ";

                DataTable DT6 = cls_DBHelper_For_EDER.GetDataTableByQuery(q_user_Sessions);
                html += "<table style='border: 1px solid #ccc;font-size: 9pt;font-family:arial'>";
                string border_style3 = "style='width:120px;border: 1px solid #ccc'";
                html += "<tr>";
                html += "<th bgcolor='#add8e6' " + border_style3 + ">Session	</th>";
                html += "<th bgcolor='#add8e6' " + border_style3 + " >Owner</th>";
                html += "<th bgcolor='#add8e6' " + border_style3 + ">Session Name</th>";
                html += "<th bgcolor='#add8e6' " + "style='width:180px;border: 1px solid #ccc'" + " >Creation Date</th>";

                html += "</tr>";


                //Preparing Occurence Table
                foreach (DataRow row in DT6.Rows)
                {
                    html += "<tr>";
                    html += "<td " + border_style3 + ">" + row["SESSIONID"].ToString() + "</td>";
                    html += "<td " + border_style3 + ">" + row["CURRENT_OWNER"].ToString() + "</td>";
                    html += "<td " + border_style3 + ">" + row["Session_Name"].ToString() + "</td>";
                    html += "<td " + border_style3 + ">" + row["CREATE_DATE"].ToString() + "</td>";


                    html += "</tr>";


                }

                //Table end
                html += "</table>";

                html += "<p><b>Note:</b> Older user sessions may block effective compress resulting into poor application performance.</p>";

                html += "<p font-size: 16pt><b>Orphan Versions(Versions without sessions):</b></p>";


                //Table 5

                string q_orphan_Versions = "  select Owner , creation_time  from ( " +
    "select owner||'.'||name  as owner ,creation_time from sde.versions " +
    "where OWNER not in ('EDGIS','RELEDITOR','GIS_I_WRITE','GIS_I','EDGIS_RO','IGPCITEDITOR') and " +
    "length(OWNER)=4 and name not in (select to_char('SN_'||SESSION_ID) from PROCESS.MM_SESSION) " +
    "order by owner||'.'||name)";
                DataTable DT7 = cls_DBHelper_For_EDER.GetDataTableByQuery(q_orphan_Versions);

                html += "<table style='border: 1px solid #ccc;font-size: 9pt;font-family:arial'>";
                string border_style4 = "style='width:120px;border: 1px solid #ccc'";
                html += "<tr>";
                html += "<th bgcolor='#add8e6'  " + border_style4 + " >Orphan Versions</th>";
                html += "<th bgcolor='#add8e6' " + border_style4 + ">Creation time</th>";
                html += "</tr>";


                foreach (DataRow row in DT7.Rows)
                {
                    html += "<tr>";
                    html += "<td " + border_style4 + ">" + row["Owner"].ToString() + "</td>";
                    html += "<td  " + "style='width:180px;border: 1px solid #ccc'" + ">" + row["creation_time"].ToString() + "</td>";



                    html += "</tr>";


                }
                html += "</table>";





                html += "<p>" + "<b>Note:</b> Orphan Versions should be deleted after business verification" + "</br>" + "</p>";
                html += "<p font-size: 16pt>" + "<b>Orphan Sessions(Sessions without versions):</b>" + "</p>";
                //Table 6


                string q_orphan_Sessions = "select Owner , creation_time  from ( " +
    "select current_owner||to_char('.SN_'||SESSION_ID) as owner ,create_date as creation_time " +
    "from PROCESS.MM_SESSION " +
    "where current_owner not in ('SDE', 'EDGIS', 'GIS_I', 'EDGIS_RO') and length(current_owner)=4 and " +
    "to_char('SN_'||SESSION_ID) not in (select name from sde.versions) " + "order by current_owner||to_char('.SN_'||SESSION_ID)) ";

                DataTable DT8 = cls_DBHelper_For_EDER.GetDataTableByQuery(q_orphan_Sessions);

                html += "<table style='border: 1px solid #ccc;font-size: 9pt;font-family:arial'>";
                string border_style5 = "style='width:120px;border: 1px solid #ccc'";
                html += "<tr>";
                html += "<th bgcolor='#add8e6'  " + border_style5 + " >Orphan Sessions</th>";
                html += "<th  bgcolor='#add8e6' " + "style='width:180px;border: 1px solid #ccc'" + " > Creation time </th>";
                html += "</tr>";






                foreach (DataRow row in DT8.Rows)
                {
                    html += "<tr>";
                    html += "<td " + border_style5 + ">" + row["Owner"].ToString() + "</td>";
                    html += "<td " + border_style5 + "> " + row["creation_time"].ToString() + "</td>";

                    html += "</tr>";


                }
                html += "</table>";





                html += "<p>" + "<b>Note:</b> Orphan Sessions should be deleted after business verification.</br>" + "<b>Note:</b> High aggregated  A table row count indicates bad state of version tree. It should be less than 1000000. Reconciling and Posting older versions followed by compress may reduce this count" + "</p>";
                
                WriteLine("Data Prepared");
                WriteLine("Sending Email");
                SendMail(html, subject_change);

            }
            catch (Exception e)
            {
                WriteLine("Error: " + e.Message);
            }
        }

       

        public static void SendMail(string strbodyText, string mail_subject)
        {
            try
            {
                // tstring strquery = "SELECT * FROM PGEDATA.APPLICATIONCONFIG where BATCH_NAME='ED15' ";

                //DataTable DT = cls_DBHelper_For_EDER.GetDaaTableByQuery(strquery);
                // if (DT != null)
                // {
                // if (DT.Rows.Count > 0)
                //{
                // DataRow[] Row_lanIDs = DT.Select("CONFIG_KEY='" + ConfigurationManager.AppSettings["LANIDS"] + "'");
                //foreach (DataRow rw in Row_lanIDs)
                // {
                //List<string> LanIDsList = ConfigurationManager.AppSettings["LANID"].ToString().Split(';').ToList();
                string lanID = string.Empty;
                string fromDisplayName = ConfigurationManager.AppSettings["MAIL_FROM_DISPLAY_NAME"];
                string fromAddress = ConfigurationManager.AppSettings["MAIL_FROM_ADDRESS"];
                string subject = mail_subject + "Electric Distribution Version Statistics Report-" + DateTime.Now.ToString("dd MMM yyyy");
                string bodyText = strbodyText;
                // string lanIDs = ConfigurationManager.AppSettings["LANIDS"];

                if (DateTime.Now.DayOfWeek == DayOfWeek.Friday)
                {
                    lanID = ConfigurationManager.AppSettings["LANID_FRIDAY"].ToString();
                }
                else
                {
                    lanID=ConfigurationManager.AppSettings["LANID"].ToString();

                }
                EmailService.Send(mail =>
                                {
                                    mail.From = fromAddress;
                                    mail.FromDisplayName = fromDisplayName;
                                    mail.To = lanID ;
                                    mail.Subject = subject;
                                    mail.BodyText = bodyText;

                                });
                
                
                WriteLine("Email Sent");

            }



            catch (Exception ex)
            {
                WriteLine("Error in Sending Email: " + ex.Message);
                //_log.Error("Error in Sending Mail: ", ex);
            }
        }

        private static string sPath = (String.IsNullOrEmpty(ConfigurationManager.AppSettings["LOG_PATH"]) ? System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) : ConfigurationManager.AppSettings["LOGPATH"]) + "\\Logfile_" + DateTime.Now.Ticks.ToString() + ".log";
        private static StreamWriter pSWriter = default(StreamWriter);
        /// <summary>
        /// create log file 
        /// </summary>
        public static void createLogFile()
        {
            (pSWriter = File.CreateText(sPath)).Close();
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
        
    
