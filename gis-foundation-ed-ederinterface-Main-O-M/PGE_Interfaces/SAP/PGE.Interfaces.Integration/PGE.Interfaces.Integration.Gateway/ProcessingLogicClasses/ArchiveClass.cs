using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace PGE.Interfaces.AutoCreateWIPPolygon.ProcessingLogicClasses
{
    class ArchiveClass
    {
        internal void StartArchiving()
        {
            try
            {
                Common._log.Info("Archiving process started");

                string sArchieveTable = ReadConfigurations.WIPINPUTTABLE;
                //Arciving of WIP_Input_Table
                Common._log.Info("Archiving of table  started");
                if (ArchiveTable(sArchieveTable, ReadConfigurations.WIP_STAGING_TABLE_FIELDS.FILECREATIONDT) == true)
                {
                    Common._log.Info("Archiving of table is successfully completed for table - " + sArchieveTable);
                }
                else
                {
                    Common._log.Error("Archiving of table failed for table - " + sArchieveTable);
                }
                sArchieveTable = ReadConfigurations.WIPFILEDETAILTABLE;
                if (ArchiveTable(sArchieveTable, ReadConfigurations.WIP_STAGING_TABLE_FIELDS.RECEIVEDDT) == true)
                {
                    Common._log.Info("Archiving of table is successfully completed for table - " + sArchieveTable);
                }
                else
                {
                    Common._log.Error("Archiving of table failed for table - " + sArchieveTable);
                }

                //Archiving of NAS Drive
                Common._log.Info("Archiving of Folder in NAS Drive is  started");
                string ArchiveFolderPath = ReadConfigurations.Output_PDF_Path.Replace("OUTBOUND", "Archive");
                if (System.IO.Directory.Exists(ArchiveFolderPath))
                {

                    (from f in new DirectoryInfo(ArchiveFolderPath).GetFiles()
                     where f.CreationTime < DateTime.Now.Subtract(TimeSpan.FromDays(Convert.ToDouble(ReadConfigurations.ArchivingDays)))
                     select f
                     ).ToList()
                     .ForEach(f => f.Delete());

                    Common._log.Info("Archiving of Folder in NAS Drive is  Compelted - FolderPath-" + ArchiveFolderPath);
                }
                ArchiveFolderPath = ReadConfigurations.XMLFolderPath + "\\Archieved\\SUCCESSED";
                if (System.IO.Directory.Exists(ArchiveFolderPath))
                {
                    
                    (from f in new DirectoryInfo(ArchiveFolderPath).GetFiles()
                     where f.CreationTime < DateTime.Now.Subtract(TimeSpan.FromDays(Convert.ToDouble(ReadConfigurations.ArchivingDays)))
                     select f
                     ).ToList()
                   .ForEach(f => f.Delete());

                    Common._log.Info("Archiving of Folder in NAS Drive is  completed- FolderPath-" + ReadConfigurations.XMLFolderPath + "\\Archieved\\SUCCESSED");
                }
                ArchiveFolderPath = ReadConfigurations.XMLFolderPath + "\\Archieved\\FAILED";
                if (System.IO.Directory.Exists(ArchiveFolderPath))
                {
                    (from f in new DirectoryInfo(ArchiveFolderPath).GetFiles()
                     where f.CreationTime < DateTime.Now.Subtract(TimeSpan.FromDays(Convert.ToDouble(ReadConfigurations.ArchivingDays)))
                     select f
                     ).ToList()
                   .ForEach(f => f.Delete());
                    Common._log.Info("Archiving of Folder in NAS Drive is  completed- FolderPath-" + ReadConfigurations.XMLFolderPath + "\\Archieved\\FAILED");
                }

               
           
                Common._log.Info("Archiving of Folder in NAS Drive is  Completed");
            }
            catch (Exception exp)
            {
                Common._log.Error(exp.Message + " at " + exp.StackTrace);
            }
        }
        /// <summary>
        /// Archive the data of the given table
        /// </summary>
        /// <param name="sTableDetails"></param>
        /// <returns></returns>
        private static bool ArchiveTable(string sArchieveTable,string sfield)
        {
            bool _retval = false;
            try
            {
                if (string.IsNullOrEmpty(sArchieveTable))
                {
                    Common._log.Error("Table details are not getting from config file,please check the config value to run the process.");
                    return false;
                }
                string sQuery = string.Empty;
                if(sArchieveTable ==ReadConfigurations.WIPINPUTTABLE)
                {
                    sQuery = "DELETE From  " + sArchieveTable + " WHERE  to_date(" + sfield + ",'YYYY-MM-DD') <= (sysdate -" + ReadConfigurations.ArchivingDays + ")";
                }
                else if (sArchieveTable == ReadConfigurations.WIPFILEDETAILTABLE)
                {
                    sQuery = "DELETE From  " + sArchieveTable + " WHERE  to_date(" + sfield + ",'dd-MON-YY') <= (sysdate -" + ReadConfigurations.ArchivingDays + ")";
                }
                 (new DBHelper()).UpdateQuery(sQuery);
                 _retval = true;
               
            }
            catch (Exception exp)
            {
                Common._log.Error(exp.Message + " at " + exp.StackTrace);
            }
            return _retval;
        }
    }
}
