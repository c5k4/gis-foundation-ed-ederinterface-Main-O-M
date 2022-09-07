using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;
using System.Net;
using System.Configuration;
using System.Data.OleDb;
using System.Data;
using log4net;
using GISPLDBAssetSynchProcess.Common;
using GISPLDBAssetSynchProcess.DAL;
using log4net;
using System.Threading;
using System.Diagnostics;

namespace GISPLDBAssetSynchProcess
{
    class Program
    {
        #region Private variables

        // private log4net.ILog Logger = null;
        //    private static readonly Log4NetLogger _logger = new Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "GISPLDB.log4net.config");
        public static string comment = string.Empty;
        public static string Errorcomment = string.Empty;
        private static string startTime;
        private static int TotalProcessRecordCount = 0;
        private static int TotalErrorRecordCount = 0;
        private static int TotalDonotProcessCount = 0;
        private static int TotalCompltedRecordCount = 0;
        private static string usp_Name = string.Empty;
             
        #endregion
        static void Main(string[] args)
        {
            GISPLDBAssetSyncTemplateService GISTempService = new GISPLDBAssetSyncTemplateService();
            try
            {
                usp_Name = System.Configuration.ConfigurationManager.AppSettings["USP_UpdateSapEquipID"];
                AppConstants.DB_PGE_GISSAP_ASSETSYNCH= System.Configuration.ConfigurationManager.AppSettings["Table_ED006"];
                AppConstants.DB_SAP_TO_GIS = System.Configuration.ConfigurationManager.AppSettings["Table_ED007"];
                AppConstants.DB_ED06_GISPLDB_INTERFACE = System.Configuration.ConfigurationManager.AppSettings["Table_Ed006__GISPLDB_INTERFACE"];
                startTime = DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss");
                // Get Error Records 
                TotalProcessRecordCount = GISTempService.GetErrorRecordcount();
                 // Read Data from CSV file 
                Console.WriteLine("Batch Process Starts");
                Console.WriteLine("Reading  ED006 Data Started");
                // Geeting records from PGE_GISSAP_ASSETSYNCH  for sending to PLDB 
                DataTable DTUpdateEd06 = GISTempService.ReadEd06fromCSV();
                Console.WriteLine("Reading  ED006 Data Completed");
                if (DTUpdateEd06 != null && DTUpdateEd06.Rows.Count > 0)
                {
                    TotalProcessRecordCount = TotalProcessRecordCount + DTUpdateEd06.Rows.Count;
                    // Insert Records in  PGEDATA.ED06_GISPLDB_INTERFACE table 
                    GISTempService.BulkInsertAfterSApEQUIPIDUpdate(DTUpdateEd06, AppConstants.DB_ED06_GISPLDB_INTERFACE);

                }
                // Calling function for update SAP Equipment ID for edit type 'C' records 
                GISTempService.UpdateSapEquipID(usp_Name);
                #region Comment code 
                //Console.WriteLine("Reading  ED007 Data Started");
                //DataTable DTUpdateEd07 = GISTempService.ReadEd07fromCSV(System.Configuration.ConfigurationManager.AppSettings["NAS_FILE_LOCATION_ED07"], AppConstants.CNFG_ED07_FIELDMAPPINGSECTION);
                //Console.WriteLine("Reading  ED007 Data Completed");
                //if (DTUpdateEd07 != null && DTUpdateEd07.Rows.Count > 0)
                //{
                //    GISTempService.BulkInsertAfterSApEQUIPIDUpdate(DTUpdateEd07, AppConstants.DB_ED07_GISPLDB_INTERFACE);

                //}
                #endregion
                Console.WriteLine("Retereiving Data to Send");
                GISTempService.RetreiveDataFromTableAndProcess();
                // GISTempService.DeleteRecordfromED06_GISPLDB_INTERFACETable();
                //Get Total error records 
                TotalErrorRecordCount = GISTempService.GetErrorRecordcount();
                //Get Total Do Not Process  records 
                TotalDonotProcessCount = GISTempService.GetDoNotProcesscount();
                //Get Total Completed records 
                TotalCompltedRecordCount = GISTempService.GetCompletedRecordcount();

                Console.WriteLine("Batch Process Ends");


                comment = "Process done sucessfully"+Environment.NewLine+"Total Records are prccessed:-"+ TotalProcessRecordCount+Environment.NewLine+ "Total Records are Completed:-" + TotalCompltedRecordCount+Environment.NewLine+ "Total Records are Error:-"+TotalErrorRecordCount+Environment.NewLine + "Total Records are Do Not Process:-"+TotalDonotProcessCount;
                
            }
            catch(Exception exp)
            {

                if (string.IsNullOrEmpty(Program.Errorcomment))
                {
                    Program.Errorcomment = "Method Name:-ReadEd06fromCSV " + exp.Message.ToString();

                }
                else
                {
                    Program.Errorcomment = Program.Errorcomment + Environment.NewLine + "Method Name:-ReadEd06fromCSV " + exp.Message.ToString();
                }
                GISTempService.ExecutionSummary(startTime, Errorcomment, Argument.Error);
            }
            finally
            {
                if(string.IsNullOrEmpty(Errorcomment))
                {
                    GISTempService.ExecutionSummary(startTime, comment, Argument.Done);
                }
                else
                {
                    GISTempService.ExecutionSummary(startTime, Errorcomment, Argument.Error);
                }
            }

            
          
        }

       

    }
}
