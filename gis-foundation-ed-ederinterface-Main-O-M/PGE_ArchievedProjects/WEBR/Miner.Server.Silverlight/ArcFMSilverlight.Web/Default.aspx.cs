using System;
using System.IO;
using System.Text;
using System.Web.UI;
using System.Xml;
using System.DirectoryServices.AccountManagement;
using System.DirectoryServices;
using System.Xml.Linq;
using System.Collections.Generic;
using NLog;


namespace ArcFMSilverlight.Web
{
    public partial class Default : Page
    {
        private PrincipalContext m_PrincipalContext = null;     //INC000004403856-CMCS

        private string pageConfigText = string.Empty;
        private List<string> ADGroupsChecked;
        /// <summary>
        /// ip control.
        /// </summary>
        protected System.Web.UI.HtmlControls.HtmlGenericControl ip;

        protected void Page_Load(object sender, EventArgs e)
        {
            var builder = new StringBuilder();

            // load the client-side page templates
            var info = new DirectoryInfo(Server.MapPath(@"ClientBin\Templates"));
            if (info.Exists)
            {
                int templateCounter = 0;
                FileInfo[] files = info.GetFiles("*.xap");
                foreach (FileInfo file in files)
                {
                    builder.Append("Template");
                    builder.Append(templateCounter);
                    builder.Append("=");
                    builder.Append(@"Templates\");
                    builder.Append(file.Name);
                    builder.Append(",");
                    templateCounter++;
                }
            }

            info = new DirectoryInfo(Server.MapPath(@"ClientBin\ExportToExcel"));
            if (info.Exists)
            {
                int templateCounter = 0;
                FileInfo[] files = info.GetFiles("*.dll");
                foreach (FileInfo file in files)
                {
                    builder.Append("ExportFormats");
                    builder.Append(templateCounter);
                    builder.Append("=");
                    builder.Append(@"ExportToExcel\");
                    builder.Append(file.Name);
                    builder.Append(",");
                    templateCounter++;
                }
            }

            string fileName = Server.MapPath("ClientBin/UserConfiguration/Page.config");
            if (File.Exists(fileName))
            {
                EncodeAndAppendString(builder, fileName, "Config");
            }

            string tempConfigFileName = Server.MapPath("ClientBin/Templates/Templates.config");
            if (File.Exists(tempConfigFileName))
            {
                EncodeAndAppendString(builder, tempConfigFileName, "TemplatesConfig");
            }

            builder.Append("," + "CurrentUserFullName" + "=" + GetUserFullName());  //INC000004403856-CMCS

            try
            {
                string hostname = Convert.ToString(System.Net.Dns.GetHostEntry(this.Page.Request.UserHostAddress).HostName).ToUpper();
                builder.Append("," + "CurrentUserMachineName" + "=" + hostname.Substring(0, hostname.IndexOf(".")));// - Remove .utility.pge.com from hostname  //DA# 190506 - WEBR Access Management 

            }
            catch
            {
            }
            //builder.Append("," + "GisDataDate" + "=" + getDataDate());// added for banner date

            ip.Attributes.Add("value", builder.ToString());


            //WEBR Access Management - //ME Q2 2019 Release
            if (pageConfigText != string.Empty)
            {
                try
                {
                    XElement pageConfigElement = XElement.Parse(pageConfigText);
                    string groupFilter = string.Empty;
                    if (pageConfigElement.HasElements)
                    {
                        foreach (XElement element in pageConfigElement.Elements("WebrAccess"))
                        {
                            groupFilter = element.Attribute("GroupFilter").Value.ToString().Replace("***", ",");
                            break;
                        }
                    }

                    if (groupFilter != string.Empty)
                    {
                        IList<string> groupFilterArr = new List<string>(groupFilter.Split(','));
                        if (!groupFilterArr.Contains("Everyone"))
                        {
                            string strPGEUsrsGrp = System.Environment.UserDomainName;
                            PrincipalContext objPrincipalContext = GetPrincipalContext(strPGEUsrsGrp);

                            UserPrincipal _userPrincipal = UserPrincipal.FindByIdentity(objPrincipalContext, IdentityType.SamAccountName, System.Web.HttpContext.Current.User.Identity.Name.ToString());//Environment.UserName.ToString());
                            bool openWebr = false;
                            foreach (string group in groupFilterArr)
                            {
                                ADGroupsChecked = new List<string>();
                                openWebr = ADGroupAccess(group, objPrincipalContext, _userPrincipal);
                                if (openWebr)
                                    break;
                            }

                            if (!openWebr)
                                Response.Redirect("WebrLocked.html");
                        }
                    }

                }
                catch (Exception ex)
                {
                }
            }

        }

        //check the WEBR access in AD group and sub AD groups
        private bool ADGroupAccess(string ADGroupName, PrincipalContext objPrincipalContext, UserPrincipal _userPrincipal)
        {
            try
            {
                string groupName = ADGroupName.Trim();
                if (ADGroupName.ToUpper().IndexOf("PGE\\") == 0)
                {
                    groupName = ADGroupName.Substring(4);
                }

                //verify if the same AD group has been checked before
                if (ADGroupsChecked.Contains(groupName))
                    return false;
                else
                    ADGroupsChecked.Add(groupName);

                if (_userPrincipal.IsMemberOf(objPrincipalContext, IdentityType.Name, groupName))
                {
                    return true;
                }
                else
                {
                    GroupPrincipal objGroup = GroupPrincipal.FindByIdentity(objPrincipalContext, IdentityType.SamAccountName, ADGroupName);
                    if (objGroup != null)
                    {
                        bool openWebr = false;
                        foreach (Principal member in objGroup.GetMembers())
                        {
                            if (member.StructuralObjectClass != null && member.StructuralObjectClass.ToUpper() == "GROUP")
                            {
                                openWebr = ADGroupAccess(member.Name.ToString(), objPrincipalContext, _userPrincipal);
                                if (openWebr)
                                    break;
                            }
                        }
                        return openWebr;
                    }
                    else
                        return false;
                }
            }
            catch
            {
                return false;
            }

        }

        private void EncodeAndAppendString(StringBuilder builder, string fileName, string keyName)
        {
            using (StreamReader stream = File.OpenText(fileName))
            {
                string text = stream.ReadToEnd().Replace(",", "***").Replace("\n", "").Replace("\t", "").Replace("\r", "");
                if (keyName == "Config")
                {
                    pageConfigText = text;
                }
                text = Server.HtmlEncode(text);
                builder.Append("," + keyName + "=" + text);
            }
        }

        //INC000004403856 CMCS---START
        private string GetUserFullName()
        {

            string strPGEUsrsGrp = System.Environment.UserDomainName;
            PrincipalContext objPrincipalContext = GetPrincipalContext(strPGEUsrsGrp);
            string strUserName = "";
            if (objPrincipalContext != null)
            {
                UserPrincipal _userPrincipal = GetUser(objPrincipalContext, System.Web.HttpContext.Current.User.Identity.Name.ToString());//Environment.UserName.ToString());
                if (_userPrincipal != null)
                {
                    strUserName += _userPrincipal.GivenName;
                    if (_userPrincipal.MiddleName != null)
                        strUserName += " " + _userPrincipal.MiddleName;
                    if (_userPrincipal.Surname != null)
                        strUserName += " " + _userPrincipal.Surname;
                    strUserName = strUserName.Trim();
                }
            }
            return strUserName;
        }



        ///
        //private string getDataDate()
        //{
        //    Data_Utility.GisData obj = new Data_Utility.GisData();
        //    return obj.getDataDate();
        //}



        /// <summary>
        /// Gets the principal context on specified OU
        /// </summary>
        /// <param name="sUserName">The username to get the principal context</param>        
        /// <returns>Returns the PrincipalContext object</returns>
        public PrincipalContext GetPrincipalContext(string sDomainName)
        {
            if (m_PrincipalContext == null)
            {

                m_PrincipalContext = new PrincipalContext(ContextType.Domain, sDomainName);
            }
            return m_PrincipalContext;
        }

        public UserPrincipal GetUser(PrincipalContext objPrincipalContext, string sUserName)
        {
            try
            {
                if (objPrincipalContext != null)
                {
                    UserPrincipal oUserPrincipal = UserPrincipal.FindByIdentity(objPrincipalContext, IdentityType.SamAccountName, sUserName);
                    return oUserPrincipal;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return null;
        }
        //INC000004403856 - CMCS---END
    }
}