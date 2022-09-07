using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Oracle.DataAccess.Client;
using System.Data;
using System.Configuration;
using System.Reflection;
using Diagnostics = PGE.Common.Delivery.Diagnostics;

namespace PGE.Interfaces.SettingsEmailNotification
{
    class EmailByRecipient : IBuildEmail
    {
        private static readonly Diagnostics.Log4NetLogger _logger = new Diagnostics.Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "SettingsEmailNotification.log4net.config");
        private bool _disposed = false;
        
        private const string DIV_KEY = "DIVISION";
        private const string DIST_KEY = "DISTRICT";
        private const string LO_KEY = "LOCALOFFICE";
        private const string REG_KEY = "REGION";

        private OracleConnection _settingsOracleConnection;

        struct FeatureClassListByDivDistLO
        {
            public string Division;
            public string District;
            public string LocalOfficeID;
            public string Region;
            public List<string> FeatureClasses;
        }

        public EmailByRecipient(OracleConnection oraConn)
        {
            _settingsOracleConnection = oraConn;
        }

        #region commented old methods
        public List<Email> BuildEmails()
        {
            List<Email> emails = new List<Email>();
            Dictionary<string, string> emailBodies = null;

            emailBodies = BuildEmailBodies();


            if (emailBodies.Count > 0)
            {
                //StringBuilder sqlBuilder = new StringBuilder();
                //sqlBuilder.Append("select ed.EMAILADDRESS from EMAILDISTRIBUTIONLIST e inner join EMAILDISTRIBUTIONLIST_DETAILS ed ");
                //sqlBuilder.Append("on e.EMAILDISTRIBUTIONNAME = ed.EMAILDISTRIBUTIONNAME where e.EMAILDISTRIBUTIONNAME='{0}'");

                foreach (KeyValuePair<string, string> entry in emailBodies)
                {
                    Email email = new Email();
                    List<string> recepients = new List<string>();

                    string recipientName = entry.Key;

                    #region commented old method of picking recepients from table
                    //string sql = string.Format(sqlBuilder.ToString(), recipientName);
                    //using (OracleCommand settingsCmd = new OracleCommand(sql, _settingsOracleConnection))
                    //{
                    //    settingsCmd.CommandType = CommandType.Text;

                    //    using (OracleDataReader settingsDataReader = settingsCmd.ExecuteReader())
                    //    {
                    //        if (settingsDataReader.HasRows)
                    //        {

                    //            while (settingsDataReader.Read())
                    //            {
                    //                recepients.Add(settingsDataReader["EMAILADDRESS"] == DBNull.Value ? string.Empty : settingsDataReader["EMAILADDRESS"] as string);
                    //            }
                    //        }
                    //    }
                    //}
                    #endregion

                    string[] sRecepientNames = recipientName.Split(',');
                    for (int iCnt = 0; iCnt < sRecepientNames.Length; iCnt++)
                    {
                        recepients.Add(sRecepientNames[iCnt]);
                    }

                    //remove empty entries
                    recepients.RemoveAll(s => s.Trim() == string.Empty);
                    if (recepients.Count > 0)
                    {
                        email.To = recepients;
                        email.From = ConfigurationManager.AppSettings["EmailSender"];
                        email.Subject = ConfigurationManager.AppSettings["EmailSubject"];
                        email.Body = entry.Value;
                        emails.Add(email);
                    }
                }
            }

            return emails;
        }

        /// <summary>
        /// Builds email bodies
        /// </summary>
        /// <returns>dictionary of email bodies having dictionary of div-dist-localoffice as key </returns>
        private Dictionary<string, string> BuildEmailBodies()
        {
            Dictionary<string, string> emailBodies = new Dictionary<string, string>();

            List<string> liRecipients = new List<string>();

            #region Prepare recipient list
            using (OracleCommand cmdRecipient = new OracleCommand("select RECIPIENT from FEATURES_TO_NOTIFY where REGION IS NOT NULL and RECIPIENT IS NOT NULL", _settingsOracleConnection))
            {
                cmdRecipient.CommandType = CommandType.Text;

                using (OracleDataReader dataReaderRecipient = cmdRecipient.ExecuteReader())
                {
                    while (dataReaderRecipient.Read())
                    {
                        string sRecipient = Convert.ToString(dataReaderRecipient["RECIPIENT"]);
                        string[] sRecepients = sRecipient.Split(',');

                        for (int iCnt = 0; iCnt < sRecepients.Length; iCnt++)
                        {
                            if (!liRecipients.Contains(sRecepients[iCnt].Trim()))
                                liRecipients.Add(sRecepients[iCnt].Trim());
                        }
                    }
                }
            }
            #endregion

            ////using (OracleCommand cmdRL = new OracleCommand("select distinct RECIPIENT from FEATURES_TO_NOTIFY where LOCALOFFICEID is not NULL and RECIPIENT is not NULL", _settingsOracleConnection))
            //using (OracleCommand cmdRL = new OracleCommand("select distinct RECIPIENT from FEATURES_TO_NOTIFY where REGION is not NULL and RECIPIENT is not NULL", _settingsOracleConnection))
            //{
            //    cmdRL.CommandType = CommandType.Text;
            //    string recipient = string.Empty;

            //    using (OracleDataReader dataReaderRL = cmdRL.ExecuteReader())
            //    {

                    //while (dataReaderRL.Read())
                    //{
            foreach (string recipient in liRecipients)
            {
                List<Dictionary<string, string>> divDistLOs = new List<Dictionary<string, string>>();
                List<FeatureClassListByDivDistLO> fcsByDivDistLOList = new List<FeatureClassListByDivDistLO>();

                //localOffice IDs
                //using (OracleCommand cmdDivDistLO = new OracleCommand("select distinct DIVISION, DISTRICT, REGION, LOCALOFFICEID from FEATURES_TO_NOTIFY where LOCALOFFICEID is not NULL and RECIPIENT ='" + recipient + "'", _settingsOracleConnection))
                using (OracleCommand cmdDivDistLO = new OracleCommand("select distinct DIVISION, DISTRICT, REGION, LOCALOFFICEID from FEATURES_TO_NOTIFY where REGION is not NULL and RECIPIENT Like ('%" + recipient + "%')", _settingsOracleConnection))
                {
                    cmdDivDistLO.CommandType = CommandType.Text;

                    using (OracleDataReader dataReaderDivDistLO = cmdDivDistLO.ExecuteReader())
                    {
                        while (dataReaderDivDistLO.Read())
                        {
                            List<string> featureClasses = new List<string>();
                            string loID = dataReaderDivDistLO["LOCALOFFICEID"] == DBNull.Value ? string.Empty : dataReaderDivDistLO["LOCALOFFICEID"].ToString();
                            string dist = dataReaderDivDistLO["DISTRICT"] == DBNull.Value ? string.Empty : dataReaderDivDistLO["DISTRICT"].ToString();
                            string div = dataReaderDivDistLO["DIVISION"] == DBNull.Value ? string.Empty : dataReaderDivDistLO["DIVISION"].ToString();
                            string region = dataReaderDivDistLO["REGION"] == DBNull.Value ? string.Empty : dataReaderDivDistLO["REGION"].ToString();

                            string sqlFC = string.Format("select distinct upper(FEATURE_CLASS_NAME) as FEATURE_CLASS_NAME from FEATURES_TO_NOTIFY where LOCALOFFICEID ='{0}' and DISTRICT ='{1}' and DIVISION ='{2}' and REGION = '{3}' and RECIPIENT LIKE ('%{4}%') and FEATURE_CLASS_NAME is not NULL", loID, dist, div, region, recipient);
                            //Feature classes
                            using (OracleCommand cmdFC = new OracleCommand(sqlFC, _settingsOracleConnection))
                            {
                                cmdFC.CommandType = CommandType.Text;

                                using (OracleDataReader dataReaderFC = cmdFC.ExecuteReader())
                                {
                                    while (dataReaderFC.Read())
                                    {
                                        featureClasses.Add(dataReaderFC["FEATURE_CLASS_NAME"] as string);
                                    }
                                }
                            }

                            if (featureClasses.Count > 0)
                            {
                                FeatureClassListByDivDistLO fcsByDivDistLO = new FeatureClassListByDivDistLO();
                                fcsByDivDistLO.Division = div;
                                fcsByDivDistLO.District = dist;
                                fcsByDivDistLO.LocalOfficeID = loID;
                                fcsByDivDistLO.Region = region;
                                fcsByDivDistLO.FeatureClasses = featureClasses;

                                fcsByDivDistLOList.Add(fcsByDivDistLO);
                            }
                        }
                    }
                }

                //Build email bodies
                string msgHeader = string.Empty;
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
                }
                catch (Exception ex)
                {
                    _logger.Error("BuildEmailBodies() - " + ex.Message);
                    throw ex;
                }

                StringBuilder tempString = new StringBuilder();
                foreach (FeatureClassListByDivDistLO fcsByDivDistLO in fcsByDivDistLOList) // (string localOfficeID in localOffices)
                {

                    string msgSubHeader = string.Format("<div><hr><h5><u>Local Office ID: {0} &nbsp;&nbsp;&nbsp; District: {1} &nbsp;&nbsp;&nbsp; Division: {2} &nbsp;&nbsp;&nbsp; REGION: {3} </u></h5></div>", fcsByDivDistLO.LocalOfficeID, fcsByDivDistLO.District, fcsByDivDistLO.Division, fcsByDivDistLO.Region);

                    tempString.AppendLine(msgSubHeader);
                    foreach (string featureClass in fcsByDivDistLO.FeatureClasses)
                    {
                        using (OracleCommand cmd = new OracleCommand(string.Format("select OBJECTID, GLOBAL_ID, RECIPIENT, ATTRIBUTEINFO from FEATURES_TO_NOTIFY where upper(LOCALOFFICEID)='{0}' and upper(DISTRICT)='{1}' and upper(DIVISION)='{2}' and upper(REGION) = '{3}' and upper(FEATURE_CLASS_NAME) = '{4}'",
                            fcsByDivDistLO.LocalOfficeID.ToUpper(), fcsByDivDistLO.District.ToUpper(), fcsByDivDistLO.Division.ToUpper(), fcsByDivDistLO.Region.ToUpper(), featureClass.ToUpper()), _settingsOracleConnection))
                        {
                            cmd.CommandType = CommandType.Text;

                            using (OracleDataReader dataReader = cmd.ExecuteReader())
                            {
                                if (dataReader.HasRows)
                                {
                                    tempString.AppendLine("<div><p>");
                                    tempString.AppendLine(string.Format("<u>Feature Class: {0}</u>", featureClass.ToUpper().Replace("EDGIS.", "")));
                                    tempString.AppendLine("<br/>");
                                    while (dataReader.Read())
                                    {
                                        string attributeInfo = dataReader["ATTRIBUTEINFO"] == DBNull.Value ? string.Empty : dataReader["ATTRIBUTEINFO"].ToString();
                                        tempString.AppendLine(attributeInfo);
                                        tempString.AppendLine("<br/><br/>");
                                        //string objectID = dataReader["OBJECTID"] == DBNull.Value ? string.Empty : dataReader["OBJECTID"].ToString();
                                        //string globalID = dataReader["GLOBAL_ID"] == DBNull.Value ? string.Empty : dataReader["GLOBAL_ID"].ToString();
                                        //tempString.AppendLine(string.Format("Object ID: {0} &nbsp;&nbsp;&nbsp;Global ID: {1}", objectID, globalID));
                                        //tempString.AppendLine("<br/>");
                                    }
                                    tempString.AppendLine("</p></div>");
                                }
                            }
                        }
                    }

                    //tempString.AppendLine("<hr>");

                }

                if (!string.IsNullOrEmpty(tempString.ToString()))
                {
                    string emailbody = string.Format("{0}{1}", msgHeader, tempString.ToString());
                    emailBodies.Add(recipient, emailbody);
                }
                fcsByDivDistLOList = null;
                //}
                //}
                //}
            }
            return emailBodies;
        }
        #endregion

        #region Shashwat's Code : Commented
        //public List<Email> BuildEmails()
        //{
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
        //private Dictionary<Dictionary<string, string>, string> BuildEmailBodies()
        //{

        //    Dictionary<Dictionary<string, string>, string> emailBodies = new Dictionary<Dictionary<string, string>, string>();
        //    HashSet<Dictionary<string, string>> DivDistLocaOffice = new HashSet<Dictionary<string, string>>();
        //    //List<string> localOffices = new List<string>();
        //    //List<string> featureClasses = new List<string>();
        //    List<string> regions = new List<string>();

        //    //localOffice IDs
        //    using (OracleCommand cmd = new OracleCommand("select distinct upper(DIVISION) DIVISION, upper(DISTRICT) DISTRICT, upper(LOCALOFFICEID) LOCALOFFICEID, upper(REGION) REGION from FEATURES_TO_NOTIFY where REGION IS NOT NULL", _settingsOracleConnection))
        //    {
        //        cmd.CommandType = CommandType.Text;

        //        using (OracleDataReader dataReader = cmd.ExecuteReader())
        //        {
        //            while (dataReader.Read())
        //            {
        //                Dictionary<string, string> dic = new Dictionary<string, string>();
        //                dic.Add(DIV_KEY, dataReader["DIVISION"] == DBNull.Value ? string.Empty : dataReader["DIVISION"].ToString());
        //                dic.Add(DIST_KEY, dataReader["DISTRICT"] == DBNull.Value ? string.Empty : dataReader["DISTRICT"].ToString());
        //                dic.Add(LO_KEY, dataReader["LOCALOFFICEID"] == DBNull.Value ? string.Empty : dataReader["LOCALOFFICEID"].ToString());
        //                dic.Add(REG_KEY, dataReader["REGION"] == DBNull.Value ? string.Empty : dataReader["REGION"].ToString());
        //                DivDistLocaOffice.Add(dic);
        //                //localOffices.Add(dataReader["LOCALOFFICEID"] as string);
        //            }
        //        }
        //    }

        //    //Feature classes
        //    //using (OracleCommand cmd = new OracleCommand("select distinct upper(FEATURE_CLASS_NAME) as FEATURE_CLASS_NAME, upper(REGION) REGION from FEATURES_TO_NOTIFY where REGION is not NULL and FEATURE_CLASS_NAME is not NULL", _settingsOracleConnection))
        //    //{
        //    //    cmd.CommandType = CommandType.Text;

        //    //    using (OracleDataReader dataReader = cmd.ExecuteReader())
        //    //    {
        //    //        while (dataReader.Read())
        //    //        {
        //    //            featureClasses.Add(dataReader["FEATURE_CLASS_NAME"] as string);
        //    //        }
        //    //    }
        //    //}

        //    using (OracleCommand cmd = new OracleCommand("select distinct upper(REGION) REGION from FEATURES_TO_NOTIFY where REGION is not NULL and FEATURE_CLASS_NAME is not NULL", _settingsOracleConnection))
        //    {
        //        cmd.CommandType = CommandType.Text;

        //        using (OracleDataReader dataReader = cmd.ExecuteReader())
        //        {
        //            while (dataReader.Read())
        //            {
        //                //featureClasses.Add(dataReader["FEATURE_CLASS_NAME"] as string);
        //                regions.Add(dataReader["REGION"] as string);
        //            }
        //        }
        //    }


        //    //Build email bodies
        //    foreach (Dictionary<string, string> dic in DivDistLocaOffice) // (string localOfficeID in localOffices)
        //    {
        //        string msgHeader = string.Empty;
        //        string loID = dic[LO_KEY];
        //        string dist = dic[DIST_KEY];
        //        string div = dic[DIV_KEY];
        //        string reg = dic[REG_KEY];
        //        try
        //        {
        //            msgHeader = ConfigurationManager.AppSettings["MessageHeader"];

        //            if (!string.IsNullOrEmpty(msgHeader.Trim()))
        //            {
        //                msgHeader = string.Format("<div>{0}</div>", string.Format(msgHeader));
        //            }
        //            else
        //            {
        //                msgHeader = "<div>Following features have been added in settings database...<hr></div>";
        //            }

        //            msgHeader += string.Format("<div><hr><h5>Local Office ID: {0} &nbsp;&nbsp;&nbsp; District: {1} &nbsp;&nbsp;&nbsp; Division: {2} </h5><hr></div>", loID, dist, div);
        //        }
        //        catch (Exception ex)
        //        {
        //            _logger.Error("BuildEmailBodies() - " + ex.Message);
        //            throw ex;
        //        }

        //        StringBuilder tempString = new StringBuilder();
        //        //foreach (string featureClass in featureClasses)
        //        foreach (string region in regions)
        //        {
        //            //using (OracleCommand cmd = new OracleCommand(string.Format("select OBJECTID, GLOBAL_ID, RECIPIENT from FEATURES_TO_NOTIFY where upper(LOCALOFFICEID)='{0}' and upper(DISTRICT)='{1}' and upper(DIVISION)='{2}' and upper(FEATURE_CLASS_NAME) = '{3}'",
        //            //loID.ToUpper(), dist.ToUpper(), div.ToUpper(), featureClass.ToUpper()), _settingsOracleConnection))

        //            OracleCommand pOCmd = new OracleCommand(string.Format("select distributionlist from edsett.EMAILADDRESSESLIST_ORIGINAL where upper(REGION)='{0}'", reg.ToUpper()), _settingsOracleConnection);
        //            OracleDataReader pORd = pOCmd.ExecuteReader();

        //            // ArrayList pAList = new ArrayList();

        //            while (pORd.Read())
        //            {//   pAList.Add(pORd["distributionlist"].ToString());

        //                using (OracleCommand cmd = new OracleCommand(string.Format("select OBJECTID, GLOBAL_ID, RECIPIENT, upper(FEATURE_CLASS_NAME) as FEATURE_CLASS_NAME from FEATURES_TO_NOTIFY where upper(REGION)='{0}' AND RECIPIENT LIKE '%{1}%'",
        //                    reg.ToUpper(), pORd["distributionlist"].ToString()), _settingsOracleConnection))
        //                {
        //                    cmd.CommandType = CommandType.Text;

        //                    using (OracleDataReader dataReader = cmd.ExecuteReader())
        //                    {
        //                        if (dataReader.HasRows)
        //                        {
        //                            while (dataReader.Read())
        //                            {
        //                                tempString.AppendLine("<div><p>");
        //                                tempString.AppendLine(string.Format("<u>Feature Class: {0}</u>", dataReader["FEATURE_CLASS_NAME"].ToString().Replace("EDGIS.", "")));
        //                                tempString.AppendLine("<br/>");


        //                                string objectID = dataReader["OBJECTID"] == DBNull.Value ? string.Empty : dataReader["OBJECTID"].ToString();
        //                                string globalID = dataReader["GLOBAL_ID"] == DBNull.Value ? string.Empty : dataReader["GLOBAL_ID"].ToString();
        //                                tempString.AppendLine(string.Format("Object ID: {0} &nbsp;&nbsp;&nbsp;Global ID: {1}", objectID, globalID));
        //                                tempString.AppendLine("<br/>");
        //                            }
        //                            tempString.AppendLine("</p></div>");
        //                            //tempString.AppendLine("");
        //                        }
        //                    }
        //                }
        //                if (!string.IsNullOrEmpty(tempString.ToString()))
        //                {
        //                    string emailbody = string.Format("{0}{1}", msgHeader, tempString.ToString());
        //                    emailBodies.Add(dic, pORd["distributionlist"].ToString() + "$" + emailbody);
        //                }
        //            }
        //        }
        //        //if (!string.IsNullOrEmpty(tempString.ToString()))
        //        //{
        //        //    string emailbody = string.Format("{0}{1}", msgHeader, tempString.ToString());
        //        //    emailBodies.Add(dic, emailbody);
        //        //}
        //    }

        //    return emailBodies;
        //}
        #endregion
        //private Dictionary<string, string> BuildEmailBodies()
        //{
        //    Dictionary<string, string> emailBodies = new Dictionary<string, string>();
        //    List<string> liRecipients = new List<string>();

        //    //Prepare recipient list
        //    using (OracleCommand cmdRecipient = new OracleCommand("select RECIPIENT from FEATURES_TO_NOTIFY where REGION IS NOT NULL", _settingsOracleConnection))
        //    {
        //        cmdRecipient.CommandType = CommandType.Text;

        //        using (OracleDataReader dataReaderRecipient = cmdRecipient.ExecuteReader())
        //        {
        //            while (dataReaderRecipient.Read())
        //            {
        //                string sRecipient = Convert.ToString(dataReaderRecipient["RECIPIENT"]);
        //                string[] sRecepients = sRecipient.Split(',');

        //                for (int iCnt = 0; iCnt < sRecepients.Length; iCnt++)
        //                {
        //                    if (!liRecipients.Contains(sRecepients[iCnt]))
        //                        liRecipients.Add(sRecepients[iCnt]);
        //                }
        //            }
        //        }
        //    }

        //    foreach (string recipient in liRecipients)
        //    {
        //        string sSQL = "select distinct upper(DIVISION) DIVISION, upper(DISTRICT) DISTRICT, upper(LOCALOFFICEID) LOCALOFFICEID, upper(REGION) REGION from FEATURES_TO_NOTIFY where REGION IS NOT NULL";
        //        using (OracleCommand cmd = new OracleCommand("select distinct upper(DIVISION) DIVISION, upper(DISTRICT) DISTRICT, upper(LOCALOFFICEID) LOCALOFFICEID, upper(REGION) REGION from FEATURES_TO_NOTIFY where REGION IS NOT NULL", _settingsOracleConnection))
        //        {
        //            cmd.CommandType = CommandType.Text;

        //            using (OracleDataReader dataReader = cmd.ExecuteReader())
        //            {




        //            }
        //        }
        //    }

        //    return emailBodies;
        //}

        #region Backup
        //public List<Email> BuildEmails()
        //{
        //    List<Email> emails = new List<Email>();
        //    Dictionary<string, string> emailBodies = null;

        //    emailBodies = BuildEmailBodies();


        //    if (emailBodies.Count > 0)
        //    {
        //        //StringBuilder sqlBuilder = new StringBuilder();
        //        //sqlBuilder.Append("select ed.EMAILADDRESS from EMAILDISTRIBUTIONLIST e inner join EMAILDISTRIBUTIONLIST_DETAILS ed ");
        //        //sqlBuilder.Append("on e.EMAILDISTRIBUTIONNAME = ed.EMAILDISTRIBUTIONNAME where e.EMAILDISTRIBUTIONNAME='{0}'");

        //        foreach (KeyValuePair<string, string> entry in emailBodies)
        //        {
        //            Email email = new Email();
        //            List<string> recepients = new List<string>();

        //            string recipientName = entry.Key;

        //            #region commented old method of picking recepients from table
        //            //string sql = string.Format(sqlBuilder.ToString(), recipientName);
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
        //            #endregion

        //            string[] sRecepientNames = recipientName.Split(',');
        //            for (int iCnt = 0; iCnt < sRecepientNames.Length; iCnt++)
        //            {
        //                recepients.Add(sRecepientNames[iCnt]);
        //            }

        //            //remove empty entries
        //            recepients.RemoveAll(s => s.Trim() == string.Empty);
        //            if (recepients.Count > 0)
        //            {
        //                email.To = recepients;
        //                email.From = ConfigurationManager.AppSettings["EmailSender"];
        //                email.Subject = ConfigurationManager.AppSettings["EmailSubject"];
        //                email.Body = entry.Value;
        //                emails.Add(email);
        //            }
        //        }
        //    }

        //    return emails;
        //}

        ///// <summary>
        ///// Builds email bodies
        ///// </summary>
        ///// <returns>dictionary of email bodies having dictionary of div-dist-localoffice as key </returns>
        //private Dictionary<string, string> BuildEmailBodies()
        //{
        //    Dictionary<string, string> emailBodies = new Dictionary<string, string>();

        //    List<string> liRecipients = new List<string>();

        //    ////using (OracleCommand cmdRL = new OracleCommand("select distinct RECIPIENT from FEATURES_TO_NOTIFY where LOCALOFFICEID is not NULL and RECIPIENT is not NULL", _settingsOracleConnection))
        //    using (OracleCommand cmdRL = new OracleCommand("select distinct RECIPIENT from FEATURES_TO_NOTIFY where REGION is not NULL and RECIPIENT is not NULL", _settingsOracleConnection))
        //    {
        //        cmdRL.CommandType = CommandType.Text;
        //        string recipient = string.Empty;

        //        using (OracleDataReader dataReaderRL = cmdRL.ExecuteReader())
        //        {

        //    while (dataReaderRL.Read())
        //    {
        //        List<Dictionary<string, string>> divDistLOs = new List<Dictionary<string, string>>();
        //        recipient = dataReaderRL["RECIPIENT"].ToString();

        //        List<FeatureClassListByDivDistLO> fcsByDivDistLOList = new List<FeatureClassListByDivDistLO>();

        //        //localOffice IDs
        //        //using (OracleCommand cmdDivDistLO = new OracleCommand("select distinct DIVISION, DISTRICT, REGION, LOCALOFFICEID from FEATURES_TO_NOTIFY where LOCALOFFICEID is not NULL and RECIPIENT ='" + recipient + "'", _settingsOracleConnection))
        //        using (OracleCommand cmdDivDistLO = new OracleCommand("select distinct DIVISION, DISTRICT, REGION, LOCALOFFICEID from FEATURES_TO_NOTIFY where REGION is not NULL and RECIPIENT ='" + recipient + "'", _settingsOracleConnection))
        //        {
        //            cmdDivDistLO.CommandType = CommandType.Text;

        //            using (OracleDataReader dataReaderDivDistLO = cmdDivDistLO.ExecuteReader())
        //            {
        //                while (dataReaderDivDistLO.Read())
        //                {
        //                    List<string> featureClasses = new List<string>();
        //                    string loID = dataReaderDivDistLO["LOCALOFFICEID"] == DBNull.Value ? string.Empty : dataReaderDivDistLO["LOCALOFFICEID"].ToString();
        //                    string dist = dataReaderDivDistLO["DISTRICT"] == DBNull.Value ? string.Empty : dataReaderDivDistLO["DISTRICT"].ToString();
        //                    string div = dataReaderDivDistLO["DIVISION"] == DBNull.Value ? string.Empty : dataReaderDivDistLO["DIVISION"].ToString();
        //                    string region = dataReaderDivDistLO["REGION"] == DBNull.Value ? string.Empty : dataReaderDivDistLO["REGION"].ToString();

        //                    string sqlFC = string.Format("select distinct upper(FEATURE_CLASS_NAME) as FEATURE_CLASS_NAME from FEATURES_TO_NOTIFY where LOCALOFFICEID ='{0}' and DISTRICT ='{1}' and DIVISION ='{2}' and REGION = '{3}' and RECIPIENT ='{4}' and FEATURE_CLASS_NAME is not NULL", loID, dist, div, region, recipient);
        //                    //Feature classes
        //                    using (OracleCommand cmdFC = new OracleCommand(sqlFC, _settingsOracleConnection))
        //                    {
        //                        cmdFC.CommandType = CommandType.Text;

        //                        using (OracleDataReader dataReaderFC = cmdFC.ExecuteReader())
        //                        {
        //                            while (dataReaderFC.Read())
        //                            {
        //                                featureClasses.Add(dataReaderFC["FEATURE_CLASS_NAME"] as string);
        //                            }
        //                        }
        //                    }

        //                    if (featureClasses.Count > 0)
        //                    {
        //                        FeatureClassListByDivDistLO fcsByDivDistLO = new FeatureClassListByDivDistLO();
        //                        fcsByDivDistLO.Division = div;
        //                        fcsByDivDistLO.District = dist;
        //                        fcsByDivDistLO.LocalOfficeID = loID;
        //                        fcsByDivDistLO.Region = region;
        //                        fcsByDivDistLO.FeatureClasses = featureClasses;

        //                        fcsByDivDistLOList.Add(fcsByDivDistLO);
        //                    }
        //                }
        //            }
        //        }

        //        //Build email bodies
        //        string msgHeader = string.Empty;
        //        try
        //        {
        //            msgHeader = ConfigurationManager.AppSettings["MessageHeader"];

        //            if (!string.IsNullOrEmpty(msgHeader.Trim()))
        //            {
        //                msgHeader = string.Format("<div>{0}</div>", string.Format(msgHeader));
        //            }
        //            else
        //            {
        //                msgHeader = "<div>Following features have been added in settings database...<hr></div>";
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            _logger.Error("BuildEmailBodies() - " + ex.Message);
        //            throw ex;
        //        }

        //        StringBuilder tempString = new StringBuilder();
        //        foreach (FeatureClassListByDivDistLO fcsByDivDistLO in fcsByDivDistLOList) // (string localOfficeID in localOffices)
        //        {

        //            string msgSubHeader = string.Format("<div><hr><h5><u>Local Office ID: {0} &nbsp;&nbsp;&nbsp; District: {1} &nbsp;&nbsp;&nbsp; Division: {2} &nbsp;&nbsp;&nbsp; REGION: {3} </u></h5></div>", fcsByDivDistLO.LocalOfficeID, fcsByDivDistLO.District, fcsByDivDistLO.Division, fcsByDivDistLO.Region);

        //            tempString.AppendLine(msgSubHeader);
        //            foreach (string featureClass in fcsByDivDistLO.FeatureClasses)
        //            {
        //                using (OracleCommand cmd = new OracleCommand(string.Format("select OBJECTID, GLOBAL_ID, RECIPIENT, ATTRIBUTEINFO from FEATURES_TO_NOTIFY where upper(LOCALOFFICEID)='{0}' and upper(DISTRICT)='{1}' and upper(DIVISION)='{2}' and upper(REGION) = '{3}' and upper(FEATURE_CLASS_NAME) = '{4}'",
        //                    fcsByDivDistLO.LocalOfficeID.ToUpper(), fcsByDivDistLO.District.ToUpper(), fcsByDivDistLO.Division.ToUpper(), fcsByDivDistLO.Region.ToUpper(), featureClass.ToUpper()), _settingsOracleConnection))
        //                {
        //                    cmd.CommandType = CommandType.Text;

        //                    using (OracleDataReader dataReader = cmd.ExecuteReader())
        //                    {
        //                        if (dataReader.HasRows)
        //                        {
        //                            tempString.AppendLine("<div><p>");
        //                            tempString.AppendLine(string.Format("<u>Feature Class: {0}</u>", featureClass.ToUpper().Replace("EDGIS.", "")));
        //                            tempString.AppendLine("<br/>");
        //                            while (dataReader.Read())
        //                            {
        //                                string attributeInfo = dataReader["ATTRIBUTEINFO"] == DBNull.Value ? string.Empty : dataReader["ATTRIBUTEINFO"].ToString();
        //                                tempString.AppendLine(attributeInfo);
        //                                tempString.AppendLine("<br/><br/>");
        //                                //string objectID = dataReader["OBJECTID"] == DBNull.Value ? string.Empty : dataReader["OBJECTID"].ToString();
        //                                //string globalID = dataReader["GLOBAL_ID"] == DBNull.Value ? string.Empty : dataReader["GLOBAL_ID"].ToString();
        //                                //tempString.AppendLine(string.Format("Object ID: {0} &nbsp;&nbsp;&nbsp;Global ID: {1}", objectID, globalID));
        //                                //tempString.AppendLine("<br/>");
        //                            }
        //                            tempString.AppendLine("</p></div>");
        //                        }
        //                    }
        //                }
        //            }

        //            //tempString.AppendLine("<hr>");

        //        }

        //        if (!string.IsNullOrEmpty(tempString.ToString()))
        //        {
        //            string emailbody = string.Format("{0}{1}", msgHeader, tempString.ToString());
        //            emailBodies.Add(recipient, emailbody);
        //        }
        //        fcsByDivDistLOList = null;
        //        }
        //        }
        //        }
        //    }
        //    return emailBodies;
        //}
        #endregion
    }
}
