using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;


namespace GISPLDBAssetSynchProcess.DAL
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
            if (System.Configuration.ConfigurationManager.AppSettings["DB_Conn_String_EDGIS_PGEDATA"] == null)
                throw new Exception("Invalid DBCode");
            strConnString = System.Configuration.ConfigurationManager.AppSettings["DB_Conn_String_EDGIS_PGEDATA"].ToString();
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
