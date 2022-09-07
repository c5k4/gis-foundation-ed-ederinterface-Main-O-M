using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using PGE_DBPasswordManagement;

namespace PGE.BatchApplication.GISPLDBAssetSynchProcess.DAL
{
    public class DBConfiguration
    {
        public DBConfiguration()
        {
            //
            // TODO: Add constructor logic here
            //
        }
        public static string GetPGEDataGetConnectionString()
        {
            string strConnString = string.Empty;
            // m4jf edgisrearch 919
           // if (System.Configuration.ConfigurationManager.AppSettings["DB_Conn_String_EDGIS_PGEDATA"] == null)
               // throw new Exception("Invalid DBCode");
           // strConnString = System.Configuration.ConfigurationManager.AppSettings["DB_Conn_String_EDGIS_PGEDATA"].ToString();

            if (System.Configuration.ConfigurationManager.AppSettings["EDER_ConnectionStr_PGEDATA"] == null)
                throw new Exception("Invalid DBCode");
            strConnString = "Persist Security Info=True;Connection Timeout=10;"+ReadEncryption.GetConnectionStr(System.Configuration.ConfigurationManager.AppSettings["EDER_ConnectionStr_PGEDATA"].ToString().ToUpper());
            return strConnString;
        }
        /// <summary>
        /// Get connection string from web.config
        /// </summary>
        /// <param name="strDBCode"></param>
        /// <returns></returns>
        public static string GetConnectionString(string strDBCode)
        {
            string strConnString = string.Empty;
            if (System.Configuration.ConfigurationManager.AppSettings["DB_Conn_String_" + strDBCode] == null)
                throw new Exception("Invalid DBCode");
            strConnString = System.Configuration.ConfigurationManager.AppSettings["DB_Conn_String_" + strDBCode].ToString();
            return strConnString;
        }
    }
}
