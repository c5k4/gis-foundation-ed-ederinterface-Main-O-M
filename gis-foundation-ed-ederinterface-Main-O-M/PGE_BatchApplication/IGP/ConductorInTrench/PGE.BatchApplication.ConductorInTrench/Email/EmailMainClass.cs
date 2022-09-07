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

namespace PGE.BatchApplication.ConductorInTrench
{
    class EmailMainClass : IBuildEmail
    {
        private static readonly PGE.Common.Delivery.Diagnostics.Log4NetLogger _logger = new PGE.Common.Delivery.Diagnostics.Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "SettingsEmailNotification.log4net.config");
        //private bool _disposed = false;

        private const string DIV_KEY = "DIVISION";
        private const string DIST_KEY = "DISTRICT";
        private const string LO_KEY = "LOCALOFFICE";
        private const string REG_KEY = "REGION";

        private OracleConnection _settingsOracleConnection;

        public EmailMainClass(OracleConnection oraConn)
        {
            _settingsOracleConnection = oraConn;
        }

        //public List<Email> BuildEmails_orig()
        //{
        //    //added
        //    _settingsOracleConnection.Open();
        //    List<Email> emails = new List<Email>();
        //    Dictionary<Dictionary<string, string>, string> emailBodies = null;

        //    emailBodies = BuildEmailBodies();

        //    if (emailBodies.Count > 0)
        //    {
        //        foreach (KeyValuePair<Dictionary<string, string>, string> entry in emailBodies)
        //        {
        //            //recipeint addresses of local office
        //            StringBuilder sqlBuilder = new StringBuilder();
        //            sqlBuilder.Append("select ed.EMAILADDRESS from SETTINGSWORKINGASSIGNMENT s inner join EMAILDISTRIBUTIONLIST e on s.EMAILDISTRIBUTIONNAME = e.EMAILDISTRIBUTIONNAME ");
        //            sqlBuilder.Append("inner join EMAILDISTRIBUTIONLIST_DETAILS ed on e.EMAILDISTRIBUTIONNAME = ed.EMAILDISTRIBUTIONNAME where upper(s.LOCALOFFICEID) ='{0}' and upper(s.DISTRICT) ='{1}' and upper(s.DIVISION) ='{2}'");

        //            Email email = new Email();
        //            List<string> recepients = new List<string>();

        //            string loID = entry.Key[LO_KEY];
        //            string dist = entry.Key[DIST_KEY];
        //            string div = entry.Key[DIV_KEY];

        //            string sql = string.Format(sqlBuilder.ToString(), loID, dist, div);
        //            //using (OracleCommand settingsCmd = new OracleCommand(sql, _settingsOracleConnection))
        //            //{
        //            //    settingsCmd.CommandType = CommandType.Text;

        //            //    using (OracleDataReader settingsDataReader = settingsCmd.ExecuteReader())
        //            //    {
        //            //        if (settingsDataReader.HasRows)
        //            //        {

        //            //            while (settingsDataReader.Read())
        //            //            {
        //            //                recepients.Add(settingsDataReader["EMAILADDRESS"] == DBNull.Value ? string.Empty : settingsDataReader["EMAILADDRESS"] as string);
        //            //            }
        //            //        }
        //            //    }
        //            //}
        //            recepients.Add(entry.Value.Split('$')[0]);

        //            //remove empty entries
        //            recepients.RemoveAll(s => s.Trim() == string.Empty);
        //            if (recepients.Count > 0)
        //            {
        //                email.To = recepients;
                        
        //                email.From = ConfigurationManager.AppSettings["EmailSender"];
        //                email.Subject = ConfigurationManager.AppSettings["EmailSubject"];
        //                //email.Body = entry.Value;
        //                email.Body = entry.Value.Split('$')[1];
        //                email.LocalOffice = loID;
        //                email.District = dist;
        //                email.Division = div;
        //                emails.Add(email);
        //            }
        //            else // search for recipeint addresses of district and division
        //            {
        //                sqlBuilder = new StringBuilder();
        //                sqlBuilder.Append("select ed.EMAILADDRESS from SETTINGSWORKINGASSIGNMENT s inner join EMAILDISTRIBUTIONLIST e on s.EMAILDISTRIBUTIONNAME = e.EMAILDISTRIBUTIONNAME ");
        //                sqlBuilder.Append("inner join EMAILDISTRIBUTIONLIST_DETAILS ed on e.EMAILDISTRIBUTIONNAME = ed.EMAILDISTRIBUTIONNAME where upper(s.LOCALOFFICEID) is NULL and upper(s.DISTRICT) ='{0}' and upper(s.DIVISION) ='{1}'");

        //                sql = string.Format(sqlBuilder.ToString(), dist, div);
        //                using (OracleCommand settingsCmd = new OracleCommand(sql, _settingsOracleConnection))
        //                {
        //                    settingsCmd.CommandType = CommandType.Text;

        //                    using (OracleDataReader settingsDataReader = settingsCmd.ExecuteReader())
        //                    {
        //                        if (settingsDataReader.HasRows)
        //                        {

        //                            while (settingsDataReader.Read())
        //                            {
        //                                recepients.Add(settingsDataReader["EMAILADDRESS"] == DBNull.Value ? string.Empty : settingsDataReader["EMAILADDRESS"] as string);
        //                            }
        //                        }
        //                    }
        //                }

        //                //remove empty entries
        //                recepients.RemoveAll(s => s.Trim() == string.Empty);
        //                if (recepients.Count > 0)
        //                {
        //                    email.To = recepients;
        //                    email.From = ConfigurationManager.AppSettings["EmailSender"];
        //                    email.Subject = ConfigurationManager.AppSettings["EmailSubject"];
        //                    email.Body = entry.Value;
        //                    email.LocalOffice = loID;
        //                    email.District = dist;
        //                    email.Division = div;
        //                    emails.Add(email);
        //                }

        //            }
        //        }
        //    }

        //    return emails;

        //}

        public DataTable BuildEmails(string globalID, string DIVISION, DataTable table_PriUG)
        {
            //added
            if(_settingsOracleConnection.State == ConnectionState.Closed)
            {
                _settingsOracleConnection.Open();
            }            
            //List<string> recepients = new List<string>();
            string recepient = string.Empty;
            StringBuilder sqlBuilder = null;
            try
            {
                // search for recipeint addresses of district and division
                sqlBuilder = new StringBuilder();
                sqlBuilder.Append("select s.EMAILADDRESS from EMAILDISTRIBUTIONLIST_TEST s inner join SETTINGSWORKINGASSIGNMENT e on s.EMAILDISTRIBUTIONNAME = e.EMAILDISTRIBUTIONNAME ");
                sqlBuilder.Append("inner join GIS_DIVISIONS ed on e.DIVISION = upper(ed.DIV_NAME) where ed.DIV_# =" + DIVISION);

                string sql = string.Format(sqlBuilder.ToString());
                using (OracleCommand settingsCmd = new OracleCommand(sql, _settingsOracleConnection))
                {
                    settingsCmd.CommandType = CommandType.Text;

                    using (OracleDataReader settingsDataReader = settingsCmd.ExecuteReader())
                    {
                        if (settingsDataReader.HasRows)
                        {

                            while (settingsDataReader.Read())
                            {
                                //recepients.Add(settingsDataReader["EMAILADDRESS"] == DBNull.Value ? string.Empty : settingsDataReader["EMAILADDRESS"] as string);
                                recepient = Convert.ToString(settingsDataReader[ReadConfigurations.col_EMAILADDRESS]);
                                table_PriUG.Rows.Add(globalID, DIVISION, recepient);
                            }
                        }
                    }
                }

                //remove empty entries
                //recepients.RemoveAll(s => s.Trim() == string.Empty);
                //if (recepient.Count > 0)
                //{
                //    email.To = recepients;
                //    email.From = ConfigurationManager.AppSettings["EmailSender"];
                //    email.Subject = ConfigurationManager.AppSettings["EmailSubject"];               
                //    //email.District = dist;
                //    //email.Division = div;
                //    emails.Add(email);
                //}
      
            }
            catch (Exception exp)
            {
                Common._log.Error("Exception : " + exp.Message +" |  Stack Trace | " + exp.StackTrace+" at "+exp.ToString());
            }
            return table_PriUG;
                
        }

        public IList<string> BuildEmailsRecepients_List(string DIVISION)
        {
            //added
            if (_settingsOracleConnection.State == ConnectionState.Closed)
            {
                _settingsOracleConnection.Open();
            }
            //List<string> recepients = new List<string>();
            string recepient = string.Empty;
            StringBuilder sqlBuilder = null;
            List<string> recepients = null;
            try
            {
                recepients = new List<string>();
                // search for recipeint addresses of district and division
                sqlBuilder = new StringBuilder();
                sqlBuilder.Append("select s.EMAILADDRESS from EMAILDISTRIBUTIONLIST_TEST s inner join SETTINGSWORKINGASSIGNMENT e on s.EMAILDISTRIBUTIONNAME = e.EMAILDISTRIBUTIONNAME ");
                sqlBuilder.Append("inner join GIS_DIVISIONS ed on e.DIVISION = upper(ed.DIV_NAME) where ed.DIV_# =" + DIVISION);

                string sql = string.Format(sqlBuilder.ToString());
                using (OracleCommand settingsCmd = new OracleCommand(sql, _settingsOracleConnection))
                {
                    settingsCmd.CommandType = CommandType.Text;

                    using (OracleDataReader settingsDataReader = settingsCmd.ExecuteReader())
                    {
                        if (settingsDataReader.HasRows)
                        {

                            while (settingsDataReader.Read())
                            {
                                //recepients.Add(settingsDataReader["EMAILADDRESS"] == DBNull.Value ? string.Empty : settingsDataReader["EMAILADDRESS"] as string);
                                recepient = Convert.ToString(settingsDataReader[ReadConfigurations.col_EMAILADDRESS]);
                                recepients.Add(recepient);
                            }
                        }
                    }
                }

            }
            catch (Exception exp)
            {
                Common._log.Error("Exception : " + exp.Message +" |  Stack Trace | " + exp.StackTrace+" at "+exp.ToString());
            }
            return recepients;

        }

        /// <summary>
        /// Builds email bodies
        /// </summary>
        /// <returns>dictionary of email bodies having dictionary of div-dist-localoffice as key </returns>
        private Dictionary<Dictionary<string, string>, string> BuildEmailBodies()
        {

            Dictionary<Dictionary<string, string>, string> emailBodies = new Dictionary<Dictionary<string, string>, string>();
            HashSet<Dictionary<string, string>> DivDistLocaOffice = new HashSet<Dictionary<string, string>>();
            //List<string> localOffices = new List<string>();
            //List<string> featureClasses = new List<string>();
            List<string> regions = new List<string>();

            //localOffice IDs
            using (OracleCommand cmd = new OracleCommand("select distinct upper(DIVISION) DIVISION, upper(DISTRICT) DISTRICT, upper(LOCALOFFICEID) LOCALOFFICEID, upper(REGION) REGION from FEATURES_TO_NOTIFY where REGION IS NOT NULL", _settingsOracleConnection))
            {
                cmd.CommandType = CommandType.Text;

                using (OracleDataReader dataReader = cmd.ExecuteReader())
                {
                    while (dataReader.Read())
                    {
                        Dictionary<string, string> dic = new Dictionary<string, string>();
                        dic.Add(DIV_KEY, dataReader["DIVISION"] == DBNull.Value ? string.Empty : dataReader["DIVISION"].ToString());
                        dic.Add(DIST_KEY, dataReader["DISTRICT"] == DBNull.Value ? string.Empty : dataReader["DISTRICT"].ToString());
                        dic.Add(LO_KEY, dataReader["LOCALOFFICEID"] == DBNull.Value ? string.Empty : dataReader["LOCALOFFICEID"].ToString());
                        dic.Add(REG_KEY, dataReader["REGION"] == DBNull.Value ? string.Empty : dataReader["REGION"].ToString());
                        DivDistLocaOffice.Add(dic);
                        //localOffices.Add(dataReader["LOCALOFFICEID"] as string);
                    }
                }
            }

            //Feature classes
            //using (OracleCommand cmd = new OracleCommand("select distinct upper(FEATURE_CLASS_NAME) as FEATURE_CLASS_NAME, upper(REGION) REGION from FEATURES_TO_NOTIFY where REGION is not NULL and FEATURE_CLASS_NAME is not NULL", _settingsOracleConnection))
            //{
            //    cmd.CommandType = CommandType.Text;

            //    using (OracleDataReader dataReader = cmd.ExecuteReader())
            //    {
            //        while (dataReader.Read())
            //        {
            //            featureClasses.Add(dataReader["FEATURE_CLASS_NAME"] as string);
            //        }
            //    }
            //}

            using (OracleCommand cmd = new OracleCommand("select distinct upper(REGION) REGION from FEATURES_TO_NOTIFY where REGION is not NULL and FEATURE_CLASS_NAME is not NULL", _settingsOracleConnection))
            {
                cmd.CommandType = CommandType.Text;

                using (OracleDataReader dataReader = cmd.ExecuteReader())
                {
                    while (dataReader.Read())
                    {
                        //featureClasses.Add(dataReader["FEATURE_CLASS_NAME"] as string);
                        regions.Add(dataReader["REGION"] as string);
                    }
                }
            }


            //Build email bodies
            foreach (Dictionary<string, string> dic in DivDistLocaOffice) // (string localOfficeID in localOffices)
            {
                string msgHeader = string.Empty;
                string loID = dic[LO_KEY];
                string dist = dic[DIST_KEY];
                string div = dic[DIV_KEY];
                string reg = dic[REG_KEY];
                try
                {
                    msgHeader = ConfigurationManager.AppSettings["MessageHeader"];

                    if (!string.IsNullOrEmpty(msgHeader.Trim()))
                    {
                        msgHeader = string.Format("<div>{0}</div>", string.Format(msgHeader));
                    }
                    else
                    {
                        msgHeader = "<div>Following features have been added in settings database...<hr></div>";
                    }

                    msgHeader += string.Format("<div><hr><h5>Local Office ID: {0} &nbsp;&nbsp;&nbsp; District: {1} &nbsp;&nbsp;&nbsp; Division: {2} </h5><hr></div>", loID, dist, div);
                }
                catch (Exception ex)
                {
                    _logger.Error("BuildEmailBodies() - " + ex.Message);
                    throw ex;
                }

                StringBuilder tempString = new StringBuilder();
                //foreach (string featureClass in featureClasses)
                foreach (string region in regions)
                {
                    //using (OracleCommand cmd = new OracleCommand(string.Format("select OBJECTID, GLOBAL_ID, RECIPIENT from FEATURES_TO_NOTIFY where upper(LOCALOFFICEID)='{0}' and upper(DISTRICT)='{1}' and upper(DIVISION)='{2}' and upper(FEATURE_CLASS_NAME) = '{3}'",
                        //loID.ToUpper(), dist.ToUpper(), div.ToUpper(), featureClass.ToUpper()), _settingsOracleConnection))

                    OracleCommand pOCmd = new OracleCommand(string.Format("select distributionlist from edsett.EMAILADDRESSESLIST_ORIGINAL where upper(REGION)='{0}'", reg.ToUpper()), _settingsOracleConnection);
                    OracleDataReader pORd = pOCmd.ExecuteReader();

                   // ArrayList pAList = new ArrayList();

                    while (pORd.Read())
                    {//   pAList.Add(pORd["distributionlist"].ToString());

                        using (OracleCommand cmd = new OracleCommand(string.Format("select OBJECTID, GLOBAL_ID, RECIPIENT, upper(FEATURE_CLASS_NAME) as FEATURE_CLASS_NAME from FEATURES_TO_NOTIFY where upper(REGION)='{0}' AND RECIPIENT LIKE '%{1}%'",
                           reg.ToUpper(), pORd["distributionlist"].ToString()), _settingsOracleConnection))
                        {
                            cmd.CommandType = CommandType.Text;

                            using (OracleDataReader dataReader = cmd.ExecuteReader())
                            {
                                if (dataReader.HasRows)
                                {
                                    while (dataReader.Read())
                                    {
                                        tempString.AppendLine("<div><p>");
                                        tempString.AppendLine(string.Format("<u>Feature Class: {0}</u>", dataReader["FEATURE_CLASS_NAME"].ToString().Replace("EDGIS.", "")));
                                        tempString.AppendLine("<br/>");


                                        string objectID = dataReader["OBJECTID"] == DBNull.Value ? string.Empty : dataReader["OBJECTID"].ToString();
                                        string globalID = dataReader["GLOBAL_ID"] == DBNull.Value ? string.Empty : dataReader["GLOBAL_ID"].ToString();
                                        tempString.AppendLine(string.Format("Object ID: {0} &nbsp;&nbsp;&nbsp;Global ID: {1}", objectID, globalID));
                                        tempString.AppendLine("<br/>");
                                    }
                                    tempString.AppendLine("</p></div>");
                                    //tempString.AppendLine("");
                                }
                            }
                        }
                        if (!string.IsNullOrEmpty(tempString.ToString()))
                        {
                            string emailbody = string.Format("{0}{1}", msgHeader, tempString.ToString());
                            emailBodies.Add(dic, pORd["distributionlist"].ToString()+"$"+emailbody);
                        }
                    }
                }
                //if (!string.IsNullOrEmpty(tempString.ToString()))
                //{
                //    string emailbody = string.Format("{0}{1}", msgHeader, tempString.ToString());
                //    emailBodies.Add(dic, emailbody);
                //}
            }

            return emailBodies;
        }

        public void BuildEmailBodies2(out DataTable division)
        {
            division = null;
            try
            {
                division = new DataTable();
                division.Columns.Add("DIV_#", typeof(string));
                division.Columns.Add("DIV_NAME", typeof(string));
                using (OracleCommand cmd = new OracleCommand("select * from GIS_DIVISIONS", _settingsOracleConnection))
                {
                    cmd.CommandType = CommandType.Text;

                    using (OracleDataReader dataReader = cmd.ExecuteReader())
                    {
                        while (dataReader.Read())
                        {
                            division.Rows.Add(dataReader["DIV_#"] == DBNull.Value ? string.Empty : dataReader["DIV_#"].ToString(), dataReader["DIV_NAME"] == DBNull.Value ? string.Empty : dataReader["DIV_NAME"].ToString());

                        }
                    }
                }
            }
            catch (Exception ee)
            {
            }
           
        }

        public DataTable checkTableStatus(string globalID)
        {
            //added
            DataTable table_check = new DataTable();
            table_check.Columns.Add("EMAILADDRESS", typeof(string));
            if (_settingsOracleConnection.State == ConnectionState.Closed)
            {
                _settingsOracleConnection.Open();
            }
            //List<string> recepients = new List<string>();
            string recepient = string.Empty;
            StringBuilder sqlBuilder = null;
            try
            {
                // search for recipeint addresses of district and division
                sqlBuilder = new StringBuilder();
                sqlBuilder.Append("select s.EMAILADDRESS from EMAILDISTRIBUTIONLIST_TEST s inner join SETTINGSWORKINGASSIGNMENT e on s.EMAILDISTRIBUTIONNAME = e.EMAILDISTRIBUTIONNAME ");
                sqlBuilder.Append("inner join GIS_DIVISIONS ed on e.DIVISION = upper(ed.DIV_NAME)");

                string sql = string.Format(sqlBuilder.ToString());
                using (OracleCommand settingsCmd = new OracleCommand(sql, _settingsOracleConnection))
                {
                    settingsCmd.CommandType = CommandType.Text;

                    using (OracleDataReader settingsDataReader = settingsCmd.ExecuteReader())
                    {
                        if (settingsDataReader.HasRows)
                        {

                            while (settingsDataReader.Read())
                            {
                                //recepients.Add(settingsDataReader["EMAILADDRESS"] == DBNull.Value ? string.Empty : settingsDataReader["EMAILADDRESS"] as string);
                                recepient = Convert.ToString(settingsDataReader[ReadConfigurations.col_EMAILADDRESS]);
                                table_check.Rows.Add(recepient);
                            }
                        }
                    }
                }
            }
            catch (Exception exp)
            {
                Common._log.Error("Exception : " + exp.Message +" |  Stack Trace | " + exp.StackTrace+" at "+exp.ToString());
            }
            return table_check;

        }

        public static string GetMailContentFromDataTable(DataTable argdataTable)
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

    }
}
