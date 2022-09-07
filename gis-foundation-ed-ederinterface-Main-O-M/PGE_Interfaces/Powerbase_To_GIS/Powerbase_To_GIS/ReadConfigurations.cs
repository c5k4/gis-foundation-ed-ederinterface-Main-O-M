using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using PGE_DBPasswordManagement;

namespace Powerbase_To_GIS
{
    public class ReadConfigurations
    {

        public static string LOGCONFIG = System.Configuration.ConfigurationManager.AppSettings["LogConfigName"];

        public const string ConnString_EDWorkSpace = "ConnString_EDWorkSpace";
        public const string ConnectionString_pbuser = "ConnectionString_pbuser";
        public const string ConnectionString_pgedata = "ConnectionString_pgedata";

        public const string col_OBJECTID = "OBJECTID";
        public const string col_GLOBALID = "GLOBALID";
        public const string col_BATCHID = "BATCHID";
        public const string col_MAXBATCHID = "MAXBATCHID";
        public const string col_PARENTDEVICEGUID = "PARENTDEVICEGUID";

        public const string col_OPERATINGNUMBER = "OPERATINGNUMBER";
        public const string col_PB_RLID = "PB_RLID";

        public const string col_ATTRIBUTENAME = "ATTRIBUTENAME";
        public const string col_ATTRIBUTEVALUE = "ATTRIBUTEVALUE";
        public const string col_DATETIME = "DATETIME";
        public const string col_COMMENTS = "COMMENTS";

        public const string col_RECORD_STATUS = "RECORD_STATUS";
                      
        public const string col_LAST_MODIFIED = "LAST_MODIFIED";        
      
        public const string col_FEATURECLASS = "FEATURECLASS";

        public const string col_PBFeatureClassName = "PBFeatureClassName";
        public const string col_GISFeatureClassName = "GISFeatureClassName";

        public const string col_GLOBALIDColumnName = "GLOBALIDColumnName";

        public const string col_SUBTYPE = "SUBTYPE";

        public const string col_SESSION_NAME = "SESSION_NAME";
        public const string col_PROCESSED_ON = "PROCESSED_ON";
        public const string col_STATUS = "STATUS";

        public const string col_PBFieldName = "PBFieldName";
        public const string col_GISFieldName = "GISFieldName";
        public const string col_DomainName = "DomainName";

        public const string SessionNamePrefix = "SessionNamePrefix";

        public const string TB_PowerbaseStage = "tableName_PowerbaseStage";
        public const string TB_PowerbaseStage_Archive = "tableName_PowerbaseStage_Archive";
        public const string TB_PB_Session = "tableName_PB_Session";
        public const string TB_PB_Session_Archive = "tableName_PB_Session_Archive";
        public const string TB_PB_GIS_REJECTED_RECORDS = "tableName_PB_GIS_REJECTED_RECORDS";
        

        public const string TB_VoltageRegulatorUnitName = "EDGIS.VOLTAGEREGULATORUNIT";
        public const string FC_VoltageRegulatorName = "EDGIS.VOLTAGEREGULATORUNIT";

        public const string PB_Value_NO_UPDATE = "PB_Value_NO_UPDATE";
        public const string PB_Value_NOT_RELEVANT = "PB_Value_NOT_RELEVANT";

        public const string TB_SCADA = "EDGIS.SCADA";

        public const string TB_MM_SessionTableName = "PROCESS.MM_SESSION";
        public const string TB_MM_CurrentPxStateTableName = "PROCESS.MM_PX_CURRENT_STATE";
        public const string TB_MM_PxVersionTableName = "PROCESS.MM_PX_VERSIONS";
        public const string TB_MM_PxUserTableName = "PROCESS.mm_px_user";
        public const string TB_MM_PxStateTableName = "PROCESS.MM_PX_STATE";

        public const string TB_GDBM_POST_HISTORY = "SDE.GDBM_POST_HISTORY";
        public const string TB_GDBM_POST_QUEUE = "SDE.GDBM_POST_QUEUE";        

        public const string SESSION_STATUS_POSTED = "POSTED";
        public const string SESSION_STATUS_DELETED = "DELETED";
        public const string SESSION_STATUS_ERROR = "ERROR";

        public const string SESSION_STATUS_INPROGRESS = "INPROGRESS";
        public const string SESSION_STATUS_PENDING_QAQC = "PENDING_QAQC";
        public const string SESSION_STATUS_HOLD = "HOLD";
        public const string SESSION_STATUS_POST_QUEUE = "POST_QUEUE";
        public const string SESSION_STATUS_POST_ERROR = "POST_ERROR";
        public const string SESSION_STATUS_RECONCILE_ERROR = "RECONCILE_ERROR";
        public const string SESSION_STATUS_CONFLICT = "IN_CONFLICT";
        public const string SESSION_STATUS_DATA_PROCESSING = "DATA_PROCESSING";
        public const string SESSION_STATUS_QA_QC_ERROR = "QA_QC_ERROR";

        public const string SESSION_STATUS_USER_QUEUE = "USER_QUEUE";
        public const string SESSION_STATUS_UNKNOWN = "UNKNOWN";

        public const string CheckPreviousSessionStatus = "CheckPreviousSessionStatus";

        public const string PB_GIS_Mapping_ConfigFileName = "PB_GIS_Mapping_ConfigFileName";
        public const string value_Y = "Y";
        public const string value_N = "N";
        public const string value_UNIT = "Unit";
        public const string value_FAILED = "FAILED";
        public const string value_PROCESSED = "PROCESSED";
        public const string value_GUID_NOT_FOUND = "GUID_NOT_FOUND";

        public const string defaultVersionName = "SDE.DEFAULT";

        public const string whereClause_PowerbaseStage = "whereClause_PowerbaseStage";

        public const string SUBJECT = "SUBJECT";
        public const string MAIL_FROM_DISPLAY_NAME = "MAIL_FROM_DISPLAY_NAME";
        public const string MAIL_FROM_ADDRESS = "MAIL_FROM_ADDRESS";
        public const string ToemailID = "ToemailID";
        public const string clientHost = "clientHost";
        public const string SendMailDaysDifference = "SendMailDaysDifference";
        public const string SendMailDay = "SendMailDay";        
        public const string fileName = "fileName";
        public const string fileExtension = "fileExtension";
        public static string intExecutionSummary = ConfigurationManager.AppSettings["IntExecutionSummaryExePath"];
        public const string ToemailID_SessionNotPosted = "ToemailID_SessionNotPosted";
        public const string ccemailID_SessionNotPosted = "ccemailID_SessionNotPosted";
        public const string SUBJECT_SessionNotPosted = "SUBJECT_SessionNotPosted";

        /// <summary>
        /// This function returns the config value for a key 
        /// </summary>
        /// <param name="argStrKey"></param>
        /// <returns></returns>
        public static string GetValue(string argStrKey)
        {
            string setting = null;
            try
            {
                setting = System.Configuration.ConfigurationManager.AppSettings[argStrKey];

                if (argStrKey == ReadConfigurations.ConnectionString_pgedata)
                {
                    //string encryptedPassword = setting.Split(',')[2];
                    string decryptedPassword = PGE_DBPasswordManagement.ReadEncryption.GetPassword(setting);

                    //setting = setting.Replace(encryptedPassword, decryptedPassword);
                }
                else if (argStrKey == ReadConfigurations.ConnectionString_pbuser)
                {
                   // string encryptedPassword = setting.Split(',')[2];
                    string decryptedPassword = PGE_DBPasswordManagement.ReadEncryption.GetPassword(setting);

                    //setting = setting.Replace(encryptedPassword, decryptedPassword);
                }
            }
            catch (Exception exp)
            {
                Common._log.Error("Exception : " + exp.Message + " |  Stack Trace | " + exp.StackTrace + " at " + exp.ToString());               
                ErrorCodeException ece = new ErrorCodeException(exp);
                Environment.ExitCode = ece.CodeNumber;
            }
            return setting;
        }

        //public static string[] GetCommaSeparatedList(string Key, string[] Default)
        //{
        //    string[] output = Default;
        //    string value = GetValue(Key);
        //    if (value != null)
        //    {
        //        try
        //        {
        //            string[] temp = value.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
        //            output = new string[temp.Length];
        //            for (int i = 0; i < temp.Length; i++)
        //            {
        //                output[i] = temp[i].Trim();
        //            }

        //        }
        //        catch (Exception exp)
        //        {
        //            Common._log.Error("Exception : " + exp.Message + " |  Stack Trace | " + exp.StackTrace + " at " + exp.ToString());                   
        //            ErrorCodeException ece = new ErrorCodeException(exp);
        //            Environment.ExitCode = ece.CodeNumber;
        //        }
        //    }
        //    return output;
        //}
        public class Argument
        {
            /// <summary>
            /// This property to get Sucess status of process
            /// </summary>
            public static string Done { get; } = "D;";
            /// <summary>
            /// This property to get failure status of process
            /// </summary>
            public static string Error { get; } = "F;";
            /// <summary>
            /// This property to get interface name 
            /// </summary>
            public static string Interface { get; } = "Powerbase To GIS;";
            /// <summary>
            /// This property to get name of the system to integration
            /// </summary>
            public static string Integration { get; } = "POWERBASE;";
            /// <summary>
            /// This property to get type of interface
            /// </summary>
            public static string Type { get; } = "Outbound;";
        }

    }
}

