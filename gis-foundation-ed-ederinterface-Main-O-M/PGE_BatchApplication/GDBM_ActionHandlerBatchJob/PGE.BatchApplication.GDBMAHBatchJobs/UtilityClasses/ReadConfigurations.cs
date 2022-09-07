using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Reflection;
using ESRI.ArcGIS.Geodatabase;
using PGE_DBPasswordManagement;
//using System.Threading.Tasks;

namespace PGE.BatchApplication.GDBMAHBatchJobs
{
    public class ReadConfigurations
    {
        internal static string OracleConnString=ReadEncryption.GetConnectionStr(GetValue("OracleConnectionString"));
        internal static string LOGCONFIG=GetValue("LogConfigName");
        internal static string SDEConnectionString =ReadEncryption.GetSDEPath(GetValue("SDEEDERWorkSpaceConnString"));
        internal static string GDBMPriority = "10";
        public static string POST_QUEUE_STATE = GetValue("GDBM_POST_QUEUE_STATE");
        public static string sUserName= GetValue("USERNAME");
        public static string sPassword = GetValue("PASSWORD");
        public static string pAHInfoTableName = "PGEDATA.PGE_GDBM_AH_Info";
        public struct STATUS
        {
           public const string sNEW = "NEW";
            public const string sCompleted = "COMPLETED";
            public const string InProgress = "INPROGRESS";
            public const string ASSIGNED = "ASSIGNED";
            public const string CONFLICT = "CONFLICT";
            public const string POSTERROR = "POSTERROR";
            public const string POSTED = "POSTED";
            public const string PROCESSED = "PROCESSED";
            public const string PostingQueue = "POSTQUEUE";
            public const string DONE = "DONE";
        }
        public struct FIELDS
        {
            public const string GLOBALID = "GLOBALID";
            public const string CIRCUITID = "CIRCUITID";
            public const string FEATURECLASSNAME = "FEATURECLASSNAME";
            public const string VERSIONNAME = "VERSIONNAME";
            public const string STATUS = "STATUS";
            public const string ACTION = "ACTION";
            public const string PARENT_VERSIONNAME = "PARENT_VERSIONNAME";
            public const string FEATOID = "FEATOID";
        }
        public struct ErrorMessage
        {
        }

        public static string GetValue(string key)
        {
            string setting = null;
            try
            {
                setting = System.Configuration.ConfigurationManager.AppSettings[key];
            } 
            catch  (Exception ex)
            {
                Common._log.Error("Key not present in Config File -- " + key) ;
                Common._log.Error(ex.Message + " at " + ex.StackTrace);
              
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
                    
                    Common._log.Error(ex.Message + " at " + ex.StackTrace);
                     
                }
            }
            return output;
        }

       
    }
}
