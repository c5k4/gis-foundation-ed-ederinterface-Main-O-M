using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using PGE_DBPasswordManagement;

namespace PGE.Interfaces.StreetlightGIS_Job
{
    class SLConfig
    {
        public static string Query_LastDataProcessed = ConfigurationManager.AppSettings["Query_LastDataProcessed"];
        public static string Query_DuplicateCount = ConfigurationManager.AppSettings["Query_DuplicateCount"];
        public static string Query_DuplicateArchival = ConfigurationManager.AppSettings["Query_DuplicateArchival"];
        public static string Query_DeleteDuplicates = ConfigurationManager.AppSettings["Query_DeleteDuplicates"];
        public static string Query_UpdateFieldPoint = ConfigurationManager.AppSettings["Query_UpdateFieldPoint"];
       
        // M4JF EDGISREARCH 919
        
        //public static string strOracleConnString_Eder = ConfigurationManager.AppSettings["EDER_DBConnString"];
        //public static string strOracleConnString_Webr = ConfigurationManager.AppSettings["WEBR_DBConnString"];

        public static string strOracleConnString_Eder = "Persist Security Info=True;"+ReadEncryption.GetConnectionStr(ConfigurationManager.AppSettings["EDER_ConnectionStr"].ToUpper());
        public static string strOracleConnString_Webr = "Persist Security Info=True;"+ReadEncryption.GetConnectionStr(ConfigurationManager.AppSettings["EDAUX_ConnectionStr"].ToUpper());

        // M4JF EDGISREARCH 919
        // public static string strDestinationDB_SDE_FilePath = ConfigurationManager.AppSettings["DestinationDB_SDE_FilePath"];
        public static string strDestinationDB_SDE_FilePath = ReadEncryption.GetSDEPath(ConfigurationManager.AppSettings["EDER_SDEConnection"].ToUpper());

        public static string strFieldPoint_FCName = ConfigurationManager.AppSettings["FC_FieldPoint"].ToString();
        public static string strPGDB_FCName = ConfigurationManager.AppSettings["FC_PGDB_FeatureClassName"].ToString();
        public static string strStreetlightInv_FCName = ConfigurationManager.AppSettings["FC_StreetlightInv"].ToString();
        public static string strUnifiedGrid_FCName = ConfigurationManager.AppSettings["FC_UnifiedGrid"].ToString();

        public static string strFLDMapNoGrid = ConfigurationManager.AppSettings["FLD_MapNo_UnifiedGrid"];
        public static string strFLDMapNoStl = ConfigurationManager.AppSettings["FLD_MapNo_Stl"];
        public static string strFLDScaleGrid = ConfigurationManager.AppSettings["FLD_Scale_UnifiedGrid"];

        public static string TB_AssignedTask = ConfigurationManager.AppSettings["TB_AssignedTask"];
        public static string TB_Transaction = ConfigurationManager.AppSettings["TB_Transaction"];
        public static string FLD_JobID_AssignedTask = ConfigurationManager.AppSettings["FLD_JobID_AssignedTask"];
        public static string FLD_TaskID_AssignedTask = ConfigurationManager.AppSettings["FLD_TaskID_AssignedTask"];
        public static string FLD_TxData_Transaction = ConfigurationManager.AppSettings["FLD_TxData_Transaction"];
        public static string FLD_JobID_Transaction = ConfigurationManager.AppSettings["FLD_JobID_Transaction"];
        public static string FLD_TaskId_Transaction = ConfigurationManager.AppSettings["FLD_TaskId_Transaction"];
        public static string Value_GISTaskNumber = ConfigurationManager.AppSettings["Value_GISTaskNumber"];
        public static string Value_UpdateSLTableTaskNumber = ConfigurationManager.AppSettings["Value_UpdateSLTableTaskNumber"];
        public static string Value_ODAReviewTaskNumber = ConfigurationManager.AppSettings["Value_ODAReviewTaskNumber"];
        public static string Value_UploadFieldDataTaskNumber = ConfigurationManager.AppSettings["Value_UploadFieldDataTaskNumber"];
        public static string Value_CopyFilesTaskNumber = ConfigurationManager.AppSettings["Value_CopyFilesTaskNumber"];
        

        public static string strJobfolderPath = ConfigurationManager.AppSettings["JobFolderPath"];

        public static string strOracleProc1Name = ConfigurationManager.AppSettings["strFieldCompareProcName"];

        public static string strEmailSender = ConfigurationManager.AppSettings["EmailSender"];
        public static string Mail_Server = ConfigurationManager.AppSettings["MAIL_SERVER"];
        public static string MailFrom = ConfigurationManager.AppSettings["MailFrom"];
        public static string MailDisplayName = ConfigurationManager.AppSettings["MailDisplayName"];
        public static string MailSubject = ConfigurationManager.AppSettings["MailSubject"];
        public static string MailBody_ToODA = ConfigurationManager.AppSettings["MailBody_ToODA"];
        public static string MailBody_ToGIS = ConfigurationManager.AppSettings["MailBody_ToGIS"];
        public static string MailBody_ToGIS_Error = ConfigurationManager.AppSettings["MailBody_ToGIS_Error"];

        public static string strODARoleID = ConfigurationManager.AppSettings["ODARoleID"];
        public static string strGISOpsRoleID = ConfigurationManager.AppSettings["GISOpsRoleID"];
        public static string TB_Roles = ConfigurationManager.AppSettings["TB_Roles"];

        //ReportsView Names
        public static string AppendDataView = ConfigurationManager.AppSettings["AppendDataView"];
        public static string DeleteDataView = ConfigurationManager.AppSettings["DeleteDataView"];
        public static string UpdateDataView = ConfigurationManager.AppSettings["UpdateDataView"];
        public static string OtherDataView = ConfigurationManager.AppSettings["OtherDataView"];
        public static string ProblemDataView = ConfigurationManager.AppSettings["ProblemDataView"];
        public static string Mc2View = ConfigurationManager.AppSettings["Mc2View"];

        //Report File Names
        public static string AppendReportName = ConfigurationManager.AppSettings["AppendReportName"];
        public static string DeleteReportName = ConfigurationManager.AppSettings["DeleteReportName"];
        public static string UpdateReportName = ConfigurationManager.AppSettings["UpdateReportName"];
        public static string OtherReportName = ConfigurationManager.AppSettings["OtherReportName"];
        public static string ProblemReportName = ConfigurationManager.AppSettings["ProblemReportName"];
        public static string Mc2ReportName = ConfigurationManager.AppSettings["Mc2ReportName"];

        //Report Shared Path
        public static string ReportPath = ConfigurationManager.AppSettings["ReportPath"];

        //Commenting below section for changing CC&B path from edgisbtcprd09 server to NAS --10/27/2017
        //public static string data_to_cdx_path = ConfigurationManager.AppSettings["data_to_cdx_path"];
        //public static string data_from_cdx_path = ConfigurationManager.AppSettings["data_from_cdx_path"];
        
    }
}
 