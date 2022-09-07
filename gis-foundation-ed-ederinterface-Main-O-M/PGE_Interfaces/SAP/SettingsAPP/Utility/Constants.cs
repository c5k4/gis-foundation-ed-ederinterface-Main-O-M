using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using PGE_DBPasswordManagement;
namespace Utility
{
    class Constants
    {
        public static string OMSProcess = "SMUtilityExport";
        public static string OmsDmsLogFile = string.Format(ConfigurationManager.AppSettings["OMS_LogFile"],DateTime.Now.ToString("MM-dd-yyyy"));
        public static string OmsDmsExportFileFolder = ConfigurationManager.AppSettings["OMS_ExportFileFolder"];
        public static string OmsDmsExportFileName = ConfigurationManager.AppSettings["OMS_ExportFileName"];
        public static string OmsDmsTriggerFile = OmsDmsExportFileFolder + ConfigurationManager.AppSettings["OMS_TriggerFileName"];

        // m4jf edgisrearch 919
        //public static string OmsDmsConnectionString = ConfigurationManager.ConnectionStrings["Settings"].ConnectionString;


        public static string OmsDmsConnectionString = ReadEncryption.GetConnectionStr(ConfigurationManager.ConnectionStrings["Settings_ConnectionStr"].ConnectionString.ToUpper());


        /*Changes for ENOS to SAP migration - ED51 changes ..*/
        //public static string OmsDmsEDERConnectionString = ConfigurationManager.ConnectionStrings["EDER"].ConnectionString;

        public static string OmsDmsEDERConnectionString = ReadEncryption.GetConnectionStr(ConfigurationManager.ConnectionStrings["EDER_ConnectionStr"].ConnectionString.ToUpper());
        public static string OmsDmsprimGenQuery = ConfigurationManager.AppSettings["OMS_PrimGenerationQuery"];
        public static string OmsDmsglobalIDFieldNameSettings = "GLOBAL_ID";
        public static string OmsDmsglobalIDFieldNameEDER = "GLOBALID";
        public static string OmsDmsDeviceGenerationName = "Generation";
        /*Changes for ENOS to SAP migration - ED51 changes ..*/

        /* Changes for Fuse Saver Start..*/
        public static string DeviceMappingKey = ConfigurationManager.AppSettings["DeviceMapping_Key"];
        public static string DeviceMappingValue = ConfigurationManager.AppSettings["DeviceMapping_Value"];
        /* Changes for Fuse Saver End..*/

        
        public static string TLMApplicationLogFile = string.Format(ConfigurationManager.AppSettings["TLM_ApplicationLogFile"], DateTime.Now.ToString("MM-dd-yyyy"));

        // m4jf edgisrearh 919
        // public static string TLMConnectionString = ConfigurationManager.ConnectionStrings["TLM"].ConnectionString;
        public static string TLMConnectionString = ReadEncryption.GetConnectionStr(ConfigurationManager.ConnectionStrings["TLM_ConnectionStr"].ConnectionString.ToUpper());

        //public static string TLMConnectionStringCD = ConfigurationManager.ConnectionStrings["TLM_Weekly"].ConnectionString;//TLM ENHANCEMENT 04/12/2019
        public static string TLMConnectionStringCD = ReadEncryption.GetConnectionStr(ConfigurationManager.ConnectionStrings["TLM_Weekly_ConnectionStr"].ConnectionString.ToUpper());//TLM ENHANCEMENT 04/12/2019



        public static string TLMArchiveFolder = ConfigurationManager.AppSettings["TLM_ArchiveFolder"];
        public static string TLMWorkingFolder = ConfigurationManager.AppSettings["TLM_WorkingFolder"];
        public static string TLMCCWSourceFile = ConfigurationManager.AppSettings["TLM_CCW_SourceFile"];
        public static string TMLCCWTriggerFile = ConfigurationManager.AppSettings["TLM_CCW_TriggerFile"];
        public static string TMLCCNBTriggerFile = ConfigurationManager.AppSettings["TLM_CCNB_TriggerFile"];
        public static string TLMSourceServicePointLoadFile = string.Concat(TLMWorkingFolder, ConfigurationManager.AppSettings["TLM_ServicePointLoadFile"]);
        public static string TLMSourceTransformerLoadFile = string.Concat(TLMWorkingFolder, ConfigurationManager.AppSettings["TLM_TransformerLoadFile"]);
        public static string TLMSourceServicePointGenerationFile = string.Concat(TLMWorkingFolder, ConfigurationManager.AppSettings["TLM_ServicePointGenerationFile"]);
        public static string TLMSourceTransformerGenerationFile = string.Concat(TLMWorkingFolder, ConfigurationManager.AppSettings["TLM_TransformerGenerationFile"]);
        public static string TLMSourceMeterFile = string.Concat(TLMWorkingFolder, ConfigurationManager.AppSettings["TLM_MeterFile"]);


        public static string TLMCCNBSourceFile = ConfigurationManager.AppSettings["TLM_CCNB_SourceFile"];
        public static bool TLMCCNBZipped = bool.Parse(ConfigurationManager.AppSettings["TLM_CCNB_Zipped"]);
        public static string TLMMonthlyLoadLogFile = string.Format(ConfigurationManager.AppSettings["TLM_MonthlyLoadLogFile"], DateTime.Now.ToString("MM-dd-yyyy"));

        public static DateTime TLMMonthlyLoadBatchDate = (string.IsNullOrEmpty(ConfigurationManager.AppSettings["TLM_MonthlyLoadBatchDate"]) ? new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1) : DateTime.Parse(ConfigurationManager.AppSettings["TLM_MonthlyLoadBatchDate"]));
        public static DateTime TLMCYMEMonthlyLoadBatchDate = (string.IsNullOrEmpty(ConfigurationManager.AppSettings["TLM_CYME_MonthlyLoadBatchDate"]) ? new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1) : DateTime.Parse(ConfigurationManager.AppSettings["TLM_CYME_MonthlyLoadBatchDate"]));
        //added  on  12/04/2019 for TLM ENHANCEMENT
        //public static DateTime TLMPopCDWBatchDate = (string.IsNullOrEmpty(ConfigurationManager.AppSettings["TLM_Pop_CDW_BatchDate"]) ? new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1) : DateTime.Parse(ConfigurationManager.AppSettings["TLM_Pop_CDW_BatchDate"]));
       // public static DateTime TLMMonthlyRunBatchDate = (string.IsNullOrEmpty(ConfigurationManager.AppSettings["TLM_Month_Run_BatchDate"]) ?  : DateTime.Parse(ConfigurationManager.AppSettings["TLM_Month_Run_BatchDate"]));
        //public static DateTime TLMProCCBBatchDate = (string.IsNullOrEmpty(ConfigurationManager.AppSettings["TLM_Process_CCB_BatchDate"]) ? new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1) : DateTime.Parse(ConfigurationManager.AppSettings["TLM_Process_CCB_BatchDate"]));
        public static string TLMPopCDWBatchDate = ConfigurationManager.AppSettings["TLM_MonthlyLoadBatchDate"];
        public static string TLMMonthlyRunBatchDate = ConfigurationManager.AppSettings["TLM_MonthlyLoadBatchDate"];
        public static string TLMPopCCBBatchDate = ConfigurationManager.AppSettings["TLM_MonthlyLoadBatchDate"];
        
        
        
        
        public static string TLMCCBRefresh = ConfigurationManager.AppSettings["TLM_Process_CCB_Refresh"];
        // changes 
        //m4jf edgisrearch 919
        //public static string LOADSEER_ConnectionString = ConfigurationManager.ConnectionStrings["LOADSEER"].ConnectionString;

        public static string LOADSEER_ConnectionString = ReadEncryption.GetConnectionStr(ConfigurationManager.ConnectionStrings["LOADSEER_ConnectionStr"].ConnectionString.ToUpper());

        public static string LOADSEER_ApplicationLogFile = string.Format(ConfigurationManager.AppSettings["LOADSEER_ApplicationLogFile"], DateTime.Now.ToString("MM-dd-yyyy"));

        // public static string ROBC_ConnectionString = ConfigurationManager.ConnectionStrings["ROBC"].ConnectionString;
        public static string ROBC_ConnectionString = ReadEncryption.GetConnectionStr(ConfigurationManager.ConnectionStrings["ROBC_ConnectionStr"].ConnectionString.ToUpper());
        public static string ROBC_ApplicationLogFile = string.Format(ConfigurationManager.AppSettings["ROBC_ApplicationLogFile"], DateTime.Now.ToString("MM-dd-yyyy"));
    }
}
