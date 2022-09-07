using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Data;
using System.Text.RegularExpressions;
using System.Web;
using Newtonsoft.Json;
using Oracle.DataAccess.Client;
using System.Reflection;
using PGE.BatchApplication.IGPPhaseUpdate;
using System.Configuration;
using PGE.BatchApplication.IGPPhaseUpdate.Utility_Classes;
using System.Collections;

namespace PGE.BatchApplication.IGPPhaseUpdate
{

    class ReadJSON
    {
        string sourcePath = string.Empty;
        string targetPath = string.Empty;
        string jsonFile = string.Empty;
        
        public Log4NetLogger logger = new Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "LoadData.config");
        DBHelper DBHelper = new DBHelper();
        public static Hashtable m_pHtable_MovedFiles = null;

        public void LoadJSONData(string sourceDirectory)
        {
            string sAllBatchIds = null;
            string sBatchId = null;
            int iRetSuccess = 0;
            int iProcessedFileCount = 0;
            string sWholeFilePath = null;
            string sSplitPath = null;
            string[] sArraySplit = null;
            try
            {
                int countFile = 0;
                m_pHtable_MovedFiles = new Hashtable();

                DirectoryInfo info = new DirectoryInfo(sourceDirectory);
                string[] fileEntries_All = info.GetFiles("*.json", SearchOption.AllDirectories)
                        .OrderBy(f => f.LastWriteTime)
                        .Select(f => f.FullName)
                        .ToArray();


                Console.WriteLine("Getting files from the directory");

                int fileCount = fileEntries_All.Length;
                if (fileCount > 0)
                {
                    Console.WriteLine("Processing started for JSON files");
                    for (int i = 0; i < fileEntries_All.Length; i++)
                    {
                        iRetSuccess = 0;
                        sWholeFilePath = null;
                        sSplitPath = null;
                        sArraySplit = null;

                        sWholeFilePath = fileEntries_All[i].ToString();
                        if ((sWholeFilePath.Contains(sourceDirectory + "\\BULK") == true) || (sWholeFilePath.Contains(sourceDirectory + "\\Bulk") == true) || (sWholeFilePath.Contains(sourceDirectory + "\\bulk") == true))
                        {
                            sArraySplit = sWholeFilePath.Split(new[] { "\\BULK\\" }, StringSplitOptions.None);
                            sSplitPath = sArraySplit[1].ToString();

                            if (sSplitPath.Contains("\\") == false)
                            {
                                iRetSuccess = JSONToDB(sWholeFilePath, "BULK", ref sBatchId);
                            }
                            else
                            {
                                continue;
                            }
                            
                        }
                        else if ((sWholeFilePath.Contains(sourceDirectory + "\\DELTA") == true) || (sWholeFilePath.Contains(sourceDirectory + "\\Delta") == true) || (sWholeFilePath.Contains(sourceDirectory + "\\delta") == true))
                        {
                            sArraySplit = sWholeFilePath.Split(new[] { "\\DELTA\\" }, StringSplitOptions.None);
                            sSplitPath = sArraySplit[1].ToString();

                            if (sSplitPath.Contains("\\") == false)
                            {
                                iRetSuccess = JSONToDB(sWholeFilePath, "DELTA", ref sBatchId);
                            }
                            else
                            {
                                continue;
                            }
                        }
                        else
                        {
                            continue;
                        }
                        if (iRetSuccess == -1)
                        {
                            throw new Exception("Process Terminated.");
                        }
                        else if (iRetSuccess == 0)
                        {
                            //delete entry from daphie tables
                            if (string.IsNullOrEmpty(sBatchId) == false)
                            {
                                DeleteRecordsFromDaphieTables("'" + sBatchId + "'");
                            }
                            logger.Error(sWholeFilePath + " file could not be processed.");
                            
                        }
                        else if (iRetSuccess == 1)
                        {
                            //delete entry from daphie tables,file already existing in processed folder
                            if (string.IsNullOrEmpty(sBatchId) == false)
                            {
                                DeleteRecordsFromDaphieTables("'" + sBatchId + "'");
                            }
                            logger.Error(sWholeFilePath + " file could not be processed. File is already present in Processed folder. FileName - >  " + sWholeFilePath);
                            (new Common()).SendMailToSupportTeam(ReadConfigurations.STATUS_DAPHIEFILE_DET.Status_LoadError, string.Empty, string.Empty, sWholeFilePath);
                        }
                        else if (iRetSuccess == 2)
                        {
                            sAllBatchIds = sAllBatchIds + "'" + sBatchId + "',";
                            iProcessedFileCount++;
                        }
                        countFile++;
                    }

                    if (string.IsNullOrEmpty(sAllBatchIds) == false)
                    {
                        sAllBatchIds = sAllBatchIds.Substring(0, sAllBatchIds.Length - 1);
                        if (ValidatingDB(sAllBatchIds) == false)
                        {
                            // move files in base folder again
                            string sTarget = null;
                            foreach (string sSource in m_pHtable_MovedFiles.Keys)
                            {
                                sTarget = m_pHtable_MovedFiles[sSource].ToString();

                                //move reverse
                                System.IO.File.Move(sTarget, sSource);
                            }

                            DeleteRecordsFromDaphieTables(sAllBatchIds);
                            logger.Error(sWholeFilePath + " file could not be processed due to validation failed error.");
                            (new Common()).SendMailToSupportTeam("FILEVALIDATE_ERROR", string.Empty, string.Empty, sWholeFilePath);
                        }
                        else
                        {
                            Console.WriteLine("Processing Completed for " + iProcessedFileCount.ToString() + " JSON files");
                        }
                    }
                }
                else
                {
                    Console.WriteLine("There are no files in the directory to proces");
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message + "   " + ex.StackTrace);
                ErrorCodeException ece = new ErrorCodeException(ex);
                Environment.ExitCode = ece.CodeNumber;
            }
        }

        public int JSONToDB(string subdirectoryDAPHIE, string LoadType, ref string sBatchId)
        {
            int iRetSuccess = 0;
            sBatchId = string.Empty;
            try
            {
                bool bDeserialize = false;
                DataTable dataTableFromJSON = new DataTable();
                string transformerTable = ReadConfigurations.DAPHIETransformerTableName;
                string conductorTable = ReadConfigurations.DAPHIEConductorTableName;
                string metersTable = ReadConfigurations.DAPHIEMeterTableName;

                int iSerialNumber = -1;

                using (StreamReader streamReaderJSON = new StreamReader(subdirectoryDAPHIE))
                {
                    bDeserialize = false;
                    string fileName = subdirectoryDAPHIE.Substring(subdirectoryDAPHIE.LastIndexOf('\\') + 1);
                    jsonFile = streamReaderJSON.ReadToEnd();
                    DataSet JSONDataset = null;
                    try
                    {
                        JSONDataset = new DataSet();
                        JSONDataset = JsonConvert.DeserializeObject<DataSet>(jsonFile);                         
                    }
                    catch (Exception ex)
                    {
                        //Send Mail to DAPHIE
                        (new Common()).SendMailToDAPHIE(ReadConfigurations.STATUS_DAPHIEFILE_DET.status_INVALIDJSON, string.Empty,fileName);
                        targetPath = System.Configuration.ConfigurationManager.AppSettings["DAPHIE_FolderPath"].ToString();
                        targetPath = (targetPath).Substring(0, targetPath.Length - 6) + "\\PROCESSED\\FAILED";
                        if (System.IO.Directory.Exists(targetPath) == false)
                        { System.IO.Directory.CreateDirectory(targetPath); }
                        string destFile = System.IO.Path.Combine(targetPath, fileName);
                        streamReaderJSON.Close();
                        System.IO.File.Move(subdirectoryDAPHIE, destFile);
                        throw ex;
                    }
                    File.GetCreationTime(subdirectoryDAPHIE);
                    iSerialNumber = DBHelper.GetSequence(ReadConfigurations.IGPSequence);
                    if (iSerialNumber <=0)
                    {
                        logger.Error("Application has been failed to get the sequence.");
                        (new Common()).SendMailToSupportTeam("GETSEQUENCE_ERROR", string.Empty, string.Empty, string.Empty);
                        iRetSuccess = -1;
                        throw new Exception("Process is Terminated.");
                    }
                    iSerialNumber = iSerialNumber + 1;

                    sBatchId = Convert.ToString(iSerialNumber);

                    //EGIS:975 : Check the existing request for same curcuit id. If row exists then status and current status will be updated as below to re-process same request again 
                    if (ConfigurationManager.AppSettings["REPROCESS_EXISTING_REQ_JSON_CIRCUITID"].ToString().ToUpper() == ("TRUE").ToUpper())
                    {
                        string sameJSONandCircuitIDRowSql = "select * from " + ReadConfigurations.DAPHIEFileDetTableName + " WHERE circuitid='" + JSONDataset.Tables[0].Rows[0][0] + "'";
                        DataTable rsDT = DBHelper.GetDataTable(sameJSONandCircuitIDRowSql);
                        if (rsDT.Rows.Count > 0)
                        {
                            string updateExistingRow = "update " + ReadConfigurations.DAPHIEFileDetTableName + " set status ='DELETED' , current_status='Re-Processing in batchid " + sBatchId + "' WHERE circuitid='" + JSONDataset.Tables[0].Rows[0][0] + "'";
                            DBHelper.UpdateQuery(updateExistingRow);
                            logger.Error("Existing request for this Circuit id is updated for Re-Processing in batchid : " + sBatchId + " in Table  " + ReadConfigurations.DAPHIEFileDetTableName);
                        }
                    }

                    string strInsertFileDetQuery = "Insert into " + ReadConfigurations.DAPHIEFileDetTableName + " (FILENAME, RECEIVEDDT, PROCESSINGDT, CIRCUITID, SERIAL_NO,STATUS,LOADTYPE,BATCHID) values ('" + fileName + "',"
                        + "to_date('" + File.GetCreationTime(subdirectoryDAPHIE).ToString("MM/dd/yyyy HH:mm:ss") + "','MM/dd/yyyy HH24:mi:ss')," + "to_date('" + DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss") + "','MM/dd/yyyy HH24:mi:ss')," + "'" + JSONDataset.Tables[0].Rows[0][0] + "'," + iSerialNumber + ",'" + ReadConfigurations.STATUS_DAPHIEFILE_DET.Status_Loaded + "','" + LoadType + "','" + sBatchId + "')";

                    if (DBHelper.UpdateQuery(strInsertFileDetQuery) == -1)
                    {
                        logger.Error("Please see the log file as application is failed to insert the record in Table  " + ReadConfigurations.DAPHIEFileDetTableName );
                        (new Common()).SendMailToSupportTeam("FILEINSERT_ERROR", string.Empty, string.Empty, string.Empty);
                        throw new Exception("Record not updated on " + ReadConfigurations.DAPHIEFileDetTableName);
                    }
                    bDeserialize = true;
                    if (bDeserialize == true)
                    {
                        logger.Info("Serializing JSON file");
                        for (int i = 0; i < JSONDataset.Tables.Count; i++)
                        {

                            if (JSONDataset.Tables[i].TableName.ToUpper() == "SERVICELOCATION")
                            {
                                dataTableFromJSON = JSONDataset.Tables[i];
                                ValidatingJSON(dataTableFromJSON);
                                AddColumnInDataTable(dataTableFromJSON, iSerialNumber.ToString(), "SERIAL_NO");
                                AddColumnInDataTable(dataTableFromJSON, sBatchId, "BATCHID");
                                DBHelper.BulkCopyDataFromDataTableJSON(dataTableFromJSON, metersTable);
                                logger.Info(metersTable + " has been table updated");
                                dataTableFromJSON.Clear();
                                dataTableFromJSON = null;
                            }

                            else if (JSONDataset.Tables[i].TableName.ToUpper() == "TRANSFORMER")
                            {
                                dataTableFromJSON = JSONDataset.Tables[i];
                                ValidatingJSON(dataTableFromJSON);
                                AddColumnInDataTable(dataTableFromJSON, iSerialNumber.ToString(), "SERIAL_NO");
                                AddColumnInDataTable(dataTableFromJSON, sBatchId, "BATCHID");
                                DBHelper.BulkCopyDataFromDataTableJSON(dataTableFromJSON, transformerTable);
                                logger.Info(transformerTable + " has been table updated");
                                dataTableFromJSON.Clear();
                                dataTableFromJSON = null;
                            }

                            else if (JSONDataset.Tables[i].TableName.ToUpper() == "PRIMARYCONDUCTOR")
                            {
                                dataTableFromJSON = JSONDataset.Tables[i];
                                ValidatingJSON(dataTableFromJSON);
                                AddColumnInDataTable(dataTableFromJSON, iSerialNumber.ToString(), "SERIAL_NO");
                                AddColumnInDataTable(dataTableFromJSON, sBatchId, "BATCHID");
                                DBHelper.BulkCopyDataFromDataTableJSON(dataTableFromJSON, conductorTable);
                                logger.Info(conductorTable + " has been table updated");
                                dataTableFromJSON.Clear();
                                dataTableFromJSON = null;
                            }
                        }
                        iRetSuccess = 1;

                        streamReaderJSON.Close();
                        targetPath = System.Configuration.ConfigurationManager.AppSettings["DAPHIE_FolderPath"].ToString();
                        targetPath = (targetPath).Substring(0, targetPath.Length - 6) + "PROCESSED\\SUCCESS";
                        if (System.IO.Directory.Exists(targetPath) == false)
                        { System.IO.Directory.CreateDirectory(targetPath); }
                        string DestFile = System.IO.Path.Combine(targetPath, fileName);
                        if (System.IO.File.Exists(targetPath + "\\" + fileName) == false)
                        {
                            System.IO.File.Move(subdirectoryDAPHIE, DestFile);
                            iRetSuccess = 2;

                            //in case need to move the old folder again
                            if (m_pHtable_MovedFiles.Contains(subdirectoryDAPHIE) == false)
                            {
                                m_pHtable_MovedFiles.Add(subdirectoryDAPHIE, DestFile);
                            }
                        }
                    }
                }

                return iRetSuccess;
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message + "   " + ex.StackTrace);
                ErrorCodeException ece = new ErrorCodeException(ex);
                Environment.ExitCode = ece.CodeNumber;
                return iRetSuccess;
            }
        }


        public bool ValidatingDB(string sBATCHID)
        {
            bool retval = false;
            try
            {
               
                DBHelper.runValidations(ReadConfigurations.CheckGUIDSP, sBATCHID);
                DBHelper.runValidations(ReadConfigurations.ChecksubtypeSP, sBATCHID);
                retval = true;
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message + "   " + ex.StackTrace);               
            }
            return retval;
        }


     
        public void ValidatingJSON(DataTable JSON_datatble)
        {
            string phaseValue = string.Empty;
            string globalIDValue = string.Empty;
            try
            {
                string[] columnNames = (from dc in JSON_datatble.Columns.Cast<DataColumn>()
                                        select dc.ColumnName).ToArray();
          
                foreach (string s in columnNames)
                {
                    foreach (DataRow row in JSON_datatble.Rows)
                    {
                        try
                        {
                            if(s.Contains("CONFIDENCE") || s.Contains("LEVEL") ||  s.Contains("GIS") || s.Contains("READY"))
                            {
                        
                                 if (row.IsNull(s))
                                     { continue; }
                            }

                            else if (s.Contains("PHASE") || s.Contains("PREDICTION"))
                            {

                                if (row[s].ToString() == "10")
                                {
                                    row["ERROR_CODE"] = ReadConfigurations.IGPPHID_04_Error;
                                    string error_description = System.Configuration.ConfigurationManager.AppSettings[ReadConfigurations.IGPPHID_04_Error].ToString();
                                    row["ERROR_DESCRIPTION"] = error_description;

                                }

                            }
                            else 
                            {
                                if (row[s].Equals("NaN"))
                                {
                                    row["ERROR_CODE"] = ReadConfigurations.IGPPHID_01_Error;
                                    string error_description = System.Configuration.ConfigurationManager.AppSettings[ReadConfigurations.IGPPHID_01_Error].ToString();
                                    row["ERROR_DESCRIPTION"] = error_description;
                                }

                                if (row.IsNull(s))
                                {
                                    row["ERROR_CODE"] = ReadConfigurations.IGPPHID_01_Error;
                                    string error_description = System.Configuration.ConfigurationManager.AppSettings[ReadConfigurations.IGPPHID_01_Error].ToString();
                                   row["ERROR_DESCRIPTION"] = error_description;
                                }                           
                            }
                        }
                        catch (Exception ex)
                        {
                            logger.Error(ex.Message + "   " + ex.StackTrace);                     
                        
                        }
                    }
                }

                #region AddColumnsToTable

                DataColumnCollection columnsJSON = JSON_datatble.Columns;
                if (!columnsJSON.Contains("ERROR_CODE"))
                {
                    JSON_datatble.Columns.Add("ERROR_CODE", typeof(String));
                }

                if (!columnsJSON.Contains("ERROR_DESCRIPTION"))
                {
                    JSON_datatble.Columns.Add("ERROR_DESCRIPTION", typeof(String));
                }

                if (!columnsJSON.Contains("DATE_TIME"))
                {
                    JSON_datatble.Columns.Add("DATE_TIME", typeof(String));
                    foreach (DataRow row in JSON_datatble.Rows)

                    { row["DATE_TIME"] = DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss tt"); }

                }
                #endregion AddColumnsToTable
          
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message + "   " + ex.StackTrace);
            }
        }


        public void AddColumnInDataTable(DataTable dt,string value,string sfieldname)
        {
            try
            {
                DataColumnCollection columns = dt.Columns;
                if (!columns.Contains(sfieldname))
                {
                    DataColumn column = new DataColumn();
                    column.ColumnName = sfieldname;
                    column.AutoIncrement = true;
                    column.AutoIncrementSeed = 0;
                    column.AutoIncrementStep = 1;
                    dt.Columns.Add(column);

                    foreach (DataRow row in dt.Rows)
                    {
                        row.SetField(column, value);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message + "   " + ex.StackTrace);
            }

        }

        private bool DeleteRecordsFromDaphieTables(string sBatchId)
        {
            string sQuery = null;
            try
            {
                sQuery = "DELETE FROM " + ReadConfigurations.DAPHIEFileDetTableName + " WHERE " + ReadConfigurations.DAPHIETablesFields.BATCH_NUMBER + " IN (" + sBatchId + ")";
                DBHelper.UpdateQuery(sQuery);

                sQuery = "DELETE FROM " + ReadConfigurations.DAPHIETransformerTableName + " WHERE " + ReadConfigurations.DAPHIETablesFields.BATCH_NUMBER + " IN (" + sBatchId + ")";
                DBHelper.UpdateQuery(sQuery);

                sQuery = "DELETE FROM " + ReadConfigurations.DAPHIEConductorTableName + " WHERE " + ReadConfigurations.DAPHIETablesFields.BATCH_NUMBER + " IN (" + sBatchId + ")";
                DBHelper.UpdateQuery(sQuery);

                sQuery = "DELETE FROM " + ReadConfigurations.DAPHIEMeterTableName + " WHERE " + ReadConfigurations.DAPHIETablesFields.BATCH_NUMBER + " IN (" + sBatchId + ")";
                DBHelper.UpdateQuery(sQuery);

                return true;
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message + "   " + ex.StackTrace);
                return false;
            }
        }

 
    }
}
