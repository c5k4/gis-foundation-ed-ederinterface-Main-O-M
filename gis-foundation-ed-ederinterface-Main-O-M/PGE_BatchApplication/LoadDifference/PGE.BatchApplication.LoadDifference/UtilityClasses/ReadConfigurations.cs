using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Reflection;
using ESRI.ArcGIS.Geodatabase;
using PGE_DBPasswordManagement;
//using System.Threading.Tasks;

namespace PGE.BatchApplication.LoadDifference
{
    public class ReadConfigurations
    {
        internal static string OracleConnString=ReadEncryption.GetConnectionStr(GetValue("OracleConnectionString"));
        internal static string TempConnectionString = GetValue("tempConnString"); 
        internal static string EDGMCConnectionString = ReadEncryption.GetSDEPath(GetValue("EDGMCConnectionString"));
        internal static string FINDFEATUREInINTDATAARACH = GetValue("FINDFEATURE_INTDATAARACH");
        internal static string FORRecovery = GetValue("FORRECORVERY");
        internal static string UpdateQuery = GetValue("UPDATEQUERY");
        internal static string InsertQuery = GetValue("INSERTQUERY");
        internal static string LOGCONFIG=GetValue("LogConfigName");
        internal static string VersionName = GetValue("VERSION_NAME");
        internal static string SDEEDERConnectionString =ReadEncryption.GetSDEPath(GetValue("SDEEDERWorkSpaceConnString"));
        internal static string SDEEDERSUBConnectionString = ReadEncryption.GetSDEPath(GetValue("SDEEDERSUBWorkSpaceConnString"));
        public static string pAHInfoTableName = "PGEDATA.PGE_GDBM_AH_INFO";
        internal static string T_RETENTION_PERIOD = GetValue("T_RETENTION_PERIOD");
        public static string EderConfigTable = "PGEDATA.PGE_EDERCONFIG";
        public static string GetValue(string key)
        {
            string setting = null;
            try
            {
                setting = System.Configuration.ConfigurationManager.AppSettings[key];
            } 
            catch  (Exception ex)
            {
                Program._log.Error("Key not present in Config File -- " + key) ;
                Program._log.Error(ex.Message + " at " + ex.StackTrace);
              
            }
            return setting;
        }

        public static string[] GetCommaSeparatedList(string Key, string[] Default)
        {
            string[] output = Default;
            string value = GetValue(Key);
            if (value != null)
            {
                try
                {
                    string[] temp = value.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    output = new string[temp.Length];
                    for (int i = 0; i < temp.Length; i++)
                    {
                        output[i] = temp[i].Trim();
                    }

                }
                catch (Exception ex)
                {
                    
                    Program._log.Error(ex.Message + " at " + ex.StackTrace);
                     
                }
            }
            return output;
        }

       
    }
}
