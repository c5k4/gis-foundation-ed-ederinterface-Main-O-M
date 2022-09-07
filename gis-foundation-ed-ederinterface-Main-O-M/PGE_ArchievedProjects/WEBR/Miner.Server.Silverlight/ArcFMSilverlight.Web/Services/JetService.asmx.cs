using System;
using System.Collections.Generic;
using System.Data.OracleClient;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.Script.Services;
using System.Web.Script.Serialization;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using System.Net;
using System.IO;

namespace ArcFMSilverlight.Web.Services
{
    /// <summary>
    /// Summary description for JetService. Cheap & cheerful Web Service in place of a more fully blown REST Service
    /// Using System.Data.OracleClient and non-TNS generics to minimize the impact to the web server deployment.
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
//    [ScriptService]
    public class JetService : System.Web.Services.WebService
    {
        private string _usersJson = null;

        private string GetConnectionString()
        {
            return System.Configuration.ConfigurationManager.AppSettings["JetTns"];
        }

        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        [WebMethod]
        public void GenerateOperatingNumbers(int equipmentId, bool isCustomer, int numOperatingNumbers)
        {
            string resultJson = "";
            string isActiveString = isCustomer ? "'1'" : "'0'";
            HttpContext.Current.Response.ContentType = "application/json";
            try
            {

                string connectionString = GetConnectionString();
                using (OracleConnection connection = new OracleConnection())
                {
                    connection.ConnectionString = connectionString;
                    connection.Open();
                    Console.WriteLine("State: {0}", connection.State);
                    Console.WriteLine("ConnectionString: {0}",
                        connection.ConnectionString);

                    OracleCommand command = connection.CreateCommand();
                    string sql = "select WEBR.GENERATE_OPERATING_NUMBER(" + equipmentId + "," + isActiveString + "," +
                                 numOperatingNumbers + ") from dual";

                    command.CommandText = sql;

                    OracleDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        string operatingNumbersCsv = (string) reader[0];
                        resultJson = new JavaScriptSerializer().Serialize(operatingNumbersCsv.Split(',').ToList());
                    }
                }

                HttpContext.Current.Response.Write(resultJson);
            }
            catch (Exception exception)
            {
                resultJson = "{\"Error\":\"" + exception.ToString() + "\"}";
                HttpContext.Current.Response.Write(resultJson);
                HttpContext.Current.Response.End();

                throw;
            }
            finally
            {
                HttpContext.Current.Response.End();
            }
        }


        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        [WebMethod]
        public void GenerateCgc12()
        {
            string resultJson = "";
            HttpContext.Current.Response.ContentType = "application/json";
            try
            {

                string connectionString = GetConnectionString();
                using (OracleConnection connection = new OracleConnection())
                {
                    connection.ConnectionString = connectionString;
                    connection.Open();
                    Console.WriteLine("State: {0}", connection.State);
                    Console.WriteLine("ConnectionString: {0}",
                        connection.ConnectionString);

                    OracleCommand command = connection.CreateCommand();
                    string sql = "select WEBR.Generate_Cgc_Number() from dual";

                    command.CommandText = sql;

                    OracleDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        string cgc12 = (string)reader[0];
                        resultJson = new JavaScriptSerializer().Serialize(new { cgc12 = cgc12});
                    }
                }

                HttpContext.Current.Response.Write(resultJson);
            }
            catch (Exception exception)
            {
                resultJson = "{\"Error\":\"" + exception.ToString() + "\"}";
                HttpContext.Current.Response.Write(resultJson);
                HttpContext.Current.Response.End();

                throw;
            }
            finally
            {
                HttpContext.Current.Response.End();
            }
        }

        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        [WebMethod]
        public void GetUsers()
        {
            if (_usersJson == null)
            {
                SortedDictionary<string,string> users = new SortedDictionary<string, string>();
                using (var context = new PrincipalContext(ContextType.Domain, "utility.pge.com"))
                {
                    string jetUserGroup = System.Configuration.ConfigurationManager.AppSettings["JetUserGroup"];
                    GroupPrincipal group = GroupPrincipal.FindByIdentity(context, jetUserGroup);

                    // if found....
                    if (group != null)
                    {
                        // iterate over members
                        foreach (Principal p in group.GetMembers())
                        {
                            UserPrincipal theUser = p as UserPrincipal;

                            if (theUser != null)
                            {
                                users.Add(theUser.SamAccountName.ToUpper(), theUser.GivenName + " " + theUser.Surname);
                                Console.WriteLine(theUser.SamAccountName);
                            }
                        }
                    }
                }
                _usersJson = new JavaScriptSerializer().Serialize(users);
            }
            HttpContext.Current.Response.ContentType = "application/json";
            HttpContext.Current.Response.Write(_usersJson);
            HttpContext.Current.Response.End();
        }
        /*******PLC Changes Start*********************/
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        [WebMethod]
        public void CallGetZOrderDataService(string orderNumber)
        {
            string getZOrderServiceUrl = System.Configuration.ConfigurationManager.AppSettings["GetZOrderDataUrl"];
            string getZOrderServiceAuthorization = System.Configuration.ConfigurationManager.AppSettings["GetZOrderDataAuthorization"];

            HttpWebRequest webRequest = (HttpWebRequest)HttpWebRequest.Create(getZOrderServiceUrl+"/"+ orderNumber);
            webRequest.Method = "GET";
            webRequest.ContentType = "application/json";

            // Add Headers to WebRequest
            webRequest.Headers[HttpRequestHeader.Authorization] = "Basic " + getZOrderServiceAuthorization;
            webRequest.Headers["Source"] = "GIS";
            webRequest.Headers["TrackingID"] = Guid.NewGuid().ToString();

            webRequest.AutomaticDecompression = DecompressionMethods.GZip;         

            //Get HttpWebReponse 
            using (HttpWebResponse response = (HttpWebResponse)webRequest.GetResponse())
            using (Stream stream = response.GetResponseStream())
            using (StreamReader reader = new StreamReader(stream))
            {
                var html = reader.ReadToEnd();
                HttpContext.Current.Response.ContentType = "application/json";
                HttpContext.Current.Response.Write(html);
                HttpContext.Current.Response.End();                

            }
        }

        /*******PLC Changes End*********************/
    }
}
