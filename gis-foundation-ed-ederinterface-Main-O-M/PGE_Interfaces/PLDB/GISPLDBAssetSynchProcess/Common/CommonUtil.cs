using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Reflection;
using System.Configuration;
using System.Collections.Specialized;
using Oracle.DataAccess.Client;
using log4net;
using GISPLDBAssetSynchProcess.DAL;
using log4net.Repository.Hierarchy;
using System.Globalization;
using System.Text.RegularExpressions;
using Microsoft.VisualBasic.FileIO;
using Newtonsoft.Json;
namespace GISPLDBAssetSynchProcess.Common
{
    
    public class CommonUtil
    {
       
       private log4net.ILog Logger = null;
       public CommonUtil(log4net.ILog logger)
       {
           Logger = logger;
       }
        /// <summary>
        /// This function is used for get csv data in table 
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns> DataTable</returns>
        public DataTable GetCSVDataInTable(string filePath)
       {
            DataTable csVDataTable = new DataTable();
            try
            {
                List<String> filePaths = getTodaysFiles(filePath);
                string sPath = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\Logfile_" + DateTime.Now.Ticks.ToString() + ".log";
                string firstFilepath = filePaths[0].ToString();
                OleDbConnection conn = new System.Data.OleDb.OleDbConnection("Provider=Microsoft.Jet.OleDb.4.0; Data Source = " + Path.GetDirectoryName(firstFilepath) + "; Extended Properties = \"Text;HDR=YES;FMT=Delimited\"");
                conn.Open();
                string strQuery = "SELECT * FROM [" + Path.GetFileName(firstFilepath) + "]";
                OleDbDataAdapter adapter = new OleDbDataAdapter(strQuery, conn);
                DataSet ds = new System.Data.DataSet("CSV File");
                adapter.Fill(ds);
                csVDataTable = ds.Tables[0];

            }
            catch (Exception ex)
            {
                Logger.Error("Method Name:-GetCSVDataInTable " + ex.Message.ToString());
            }
            return csVDataTable;
        }
        /// <summary>
        /// This function is used for read csv data and map field value 
        /// </summary>
        /// <param name="filePath"></param>
        /// param name="SectionName"></param>
        /// <returns> DataTable</returns>
        public DataTable ReadCSVDataInTableandMap(string filePath,string SectionName)
        {
            DataTable csVDataTable = new DataTable();
            try
            {

                List<String> filePaths = getTodaysFiles(filePath);
                // string sPath = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\Logfile_" + DateTime.Now.Ticks.ToString() + ".log";


                var FieldMappingSection = ConfigurationManager.GetSection(SectionName) as NameValueCollection;
                foreach (string Fieldmap in FieldMappingSection)
                {
                    csVDataTable.Columns.Add(Fieldmap);
                }
                foreach (string firstFilepath in filePaths)
                {

                    using (StreamReader sr = new StreamReader(firstFilepath))
                    {
                        // FieldMappingSection fieldMappingSection = Config.ReadConfigFromExeLocation().GetSection("FieldMappingSection") as FieldMappingSection;
                        //  System.Configuration.ConfigurationManager.AppSettings["EDIT_TYPE"];
                        string line = string.Empty;
                        while ((line = sr.ReadLine()) != null)
                        {
                            string[] strRow = line.Split(',');
                            //   int strRowCount = 0;
                            DataRow dr = csVDataTable.NewRow();
                            if (csVDataTable.Columns.Count == strRow.Length)
                            {
                                for (int j = 0; j < csVDataTable.Columns.Count; j++)
                                {
                                    dr[csVDataTable.Columns[j].ColumnName] = strRow[j];

                                }

                                csVDataTable.Rows.Add(dr);
                            }


                        }

                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Method Name:-ReadCSVDataInTableandMap " + ex.Message.ToString());
            }
            return csVDataTable;
 
        }
        /// <summary>
        /// This function is used for update SapEquipID and copy table 
        /// </summary>
        /// <param name="ed06DataTable"></param>
        /// param name="ed07DataTable"></param>
        /// <returns> DataTable</returns>
        public DataTable UpdateSapEquipIDandCopyTable(DataTable ed06DataTable, DataTable ed07DataTable)
        {
            DataTable ed06TempCopy = new DataTable();
            try
            {
                if (ed06DataTable != null)
                {
                    ed06TempCopy = ed06DataTable.Copy();
                    var updateQuery = from r1 in ed06TempCopy.AsEnumerable()
                                      join r2 in ed07DataTable.AsEnumerable()
                                      on r1.Field<string>(AppConstants.CNFG_GUID) equals r2.Field<string>(AppConstants.CNFG_GUID)
                                      select new { r1, r2 };
                    foreach (var x in updateQuery)
                    {
                        x.r1.SetField(AppConstants.CNFG_SAP_EQUIP_ID, x.r2.Field<string>(AppConstants.CNFG_SAP_EQUIP_ID));
                    }
                    ed06TempCopy.AcceptChanges();
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Method Name:-ReadCSVDataInTableandMap " + ex.Message.ToString());
            }
            return ed06TempCopy;
        }
        /// <summary>
        /// This function is used for get latest csv file from file directory 
        /// </summary>
        /// <param name="filedirectory"></param>
        /// <returns> string</returns>
        private string[] GetLatestUpdatedCSV(string filedirectory)
        {
            List<string> FileList = new List<string>();
            DateTime today = DateTime.Now.Date;
            try
            {
                DirectoryInfo di = new DirectoryInfo(filedirectory);

                IEnumerable<FileInfo> fileList = di.GetFiles("*.*");

                //Create the query

                IEnumerable<FileInfo> fileQuery = from file in fileList
                                                  where (file.Extension.ToLower() == ".csv")
                                                  orderby file.LastWriteTime
                                                  select file;

                //&& file.LastWriteTime .ToString("dd/MM/yyyy") == today.ToString()

                foreach (System.IO.FileInfo fi in fileQuery)
                {
                    //fi.Attributes = FileAttributes.Normal;
                    if (fi.LastWriteTime.ToShortDateString().Equals(DateTime.Now.ToShortDateString()) && (fi.Name.ToUpper().Contains("SC")))
                        FileList.Add(fi.FullName);
                    // else Console.WriteLine("No Updated CSV files presented in Folder");
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Method Name:-GetLatestUpdatedCSV " + ex.Message.ToString());
            }
            return FileList.ToArray();
        }
        /// <summary>
        /// This function is used bulk  Insert records in ED006 interface table 
        /// </summary>
        /// <param name="DTable06"></param>
        /// <param name="TableName"></param>
        /// <returns> bool</returns>
        public bool BulkInsertintoED06(DataTable DTable06,string TableName)
        {
             bool IsBulkInsertintoED06=false;
            try
            {
                if ((DTable06 != null) && (DTable06.Rows.Count > 0))
                {
                    DBHelper objDB1 = new DBHelper(AppConstants.DB_Code, Logger);
                    objDB1.BulkInsert(DTable06, false,TableName);
                    IsBulkInsertintoED06= true;
                }
                else
                {
                    IsBulkInsertintoED06= false;
                }
            }
            catch (Exception exp)
            {

                if (string.IsNullOrEmpty(Program.Errorcomment))
                {
                    Program.Errorcomment = "Method Name:-BulkInsertintoED06 " + exp.Message.ToString();

                }
                else
                {
                    Program.Errorcomment = Program.Errorcomment + Environment.NewLine + "Method Name:-BulkInsertintoED06 " + exp.Message.ToString();
                }
                Logger.Error("Method Name:-BulkInsertintoED06 " + exp.Message.ToString());
            }
            return IsBulkInsertintoED06;
        }

        /// <summary>
        /// This function is used for get data from ED006 csv file 
        /// </summary>
        /// <returns> DataTable</returns>
        public DataTable GetDataFromED06CSV()
        {
            DataTable dt = new DataTable();
            try
            {
                dt = DefineEd06InterfaceTable();

                //DataTable dt = DefineEd06InterfaceTable();
                if (dt != null)
                {
                    //string path = ConfigurationManager.AppSettings["NAS_FILE_LOCATION_ED06"];
                   // string[] files = GetLatestUpdatedCSV();
                    dt = ValidateDataAndInsertInDataTable(dt);
                }
            }
            catch (Exception exp)
            {
                if (string.IsNullOrEmpty(Program.Errorcomment))
                {
                    Program.Errorcomment = "Method Name:-GetDataFromED06CSV " + exp.Message.ToString();

                }
                else
                {
                    Program.Errorcomment = Program.Errorcomment + Environment.NewLine + "Method Name:-GetDataFromED06CSV " + exp.Message.ToString();
                }
                Logger.Error("Method Name:-GetDataFromED06CSV " + exp.Message.ToString());
            }
            return dt;
        }

        /// <summary>
        /// This function is used for insert data in ED006 GIS interface table 
        /// </summary>
        /// <returns> void</returns>
        public void WriteTable()
        {
            DataTable dt = new DataTable();
            try
            {

                dt = DefineEd06InterfaceTable();
                if (dt != null)
                {

                    //string path = ConfigurationManager.AppSettings["NAS_FILE_LOCATION_ED06"];
                    string[] files = GetLatestUpdatedCSV(ConfigurationManager.AppSettings["NAS_FILE_LOCATION_ED06"].ToString());
                    dt = ValidateDataAndInsertInDataTable(dt);
                    if ((dt != null) && (dt.Rows.Count > 0))
                    {
                        DBHelper objDB1 = new DBHelper(AppConstants.DB_Code);
                        objDB1.BulkInsert(dt, false,"");
                    }
                    else Console.WriteLine("No Data Found in CSV"); ;
                }
            }
            catch (Exception exp)
            {
                Logger.Error("Method Name:-WriteTable " + exp.Message.ToString());
            }
                                   
        }
        /// <summary>
        /// This function is used for ValidateData ED006 data Insert records in DataTable 
        /// </summary>
        /// <param name="dt"></param>
        /// <returns> DataTable</returns>

        public DataTable ValidateDataAndInsertInDataTable(DataTable dt)
        {
            string strSapAattributecofig = string.Empty;
            string strSapAattribute = string.Empty;
            Dictionary<string, string> SapAttributeDict = new Dictionary<string, string>();
            DataTable dtJSONData = new DataTable();
            Boolean isNumeric;
            string strPLDBID = string.Empty;

            try
            {
                dt.BeginLoadData();
                // code for read data and create table 
                strSapAattributecofig = ConfigurationManager.AppSettings["SAPJSONAttributeSeq"].ToString();
                if (!string.IsNullOrEmpty(strSapAattributecofig))
                {
                    SapAttributeDict = strSapAattributecofig.Split(';').ToDictionary(item => item.Split('=')[0], item => item.Split('=')[1]);

                    dtJSONData = GetRecordFromPGE_GISSAP_ASSETSYNCH();
                    if (dtJSONData != null && dtJSONData.Rows.Count > 0)
                    {
                        foreach (DataRow drJsonData in dtJSONData.Rows)
                        {
                            strSapAattribute = drJsonData["SAPATTRIBUTES"].ToString();
                            if (!string.IsNullOrEmpty(strSapAattribute))
                            {
                                DataTable dtSapAattribute = new DataTable();
                                dtSapAattribute = ConvertJsonstringtoDataTable(strSapAattribute);
                                if (dtSapAattribute != null && dtSapAattribute.Rows.Count > 0)
                                {
                                    DataRow dr = dt.NewRow();
                                    for (int j = 0; j < dt.Columns.Count - 1; j++)
                                    {

                                        string ColType = dt.Columns[j].DataType.Name;
                                        switch (ColType)
                                        {
                                            case "String":

                                                if (dt.Columns[j].ColumnName.ToString() == "INTERFACE_STATUS")
                                                    dr[dt.Columns[j].ColumnName] = "NEW";
                                                else
                                                {
                                                    if (SapAttributeDict.ContainsKey(dt.Columns[j].ColumnName.ToString()))
                                                    {
                                                        DataRow[] selDRs = dtSapAattribute.Select("KeyField ='" + SapAttributeDict[dt.Columns[j].ColumnName.ToString()] + "'");
                                                        if (selDRs.Length > 0)
                                                        {
                                                            foreach (DataRow selDR in selDRs)
                                                            {
                                                                if (!string.IsNullOrEmpty(selDR["KeyValue"].ToString()))
                                                                    dr[dt.Columns[j].ColumnName] = selDR["KeyValue"].ToString();
                                                            }
                                                        }
                                                    }
                                                }
                                                break;

                                            case "DateTime":
                                                var cultureInfo = new CultureInfo("en-US");
                                                if (dt.Columns[j].ColumnName.ToString() == "CREATED_DATE")
                                                {

                                                    string result = DateTime.Parse(DateTime.Now.ToShortDateString(), CultureInfo.InvariantCulture).ToString("dd-MMM-yyyy");
                                                    dr[dt.Columns[j].ColumnName] = result;
                                                }

                                                break;
                                            //case "Int":
                                            //    if (dt.Columns[j].ColumnName.ToString() == "RETRY_COUNT")
                                            //        dr[dt.Columns[j].ColumnName] = 0;
                                            //    break;

                                            case "Int32":
                                                if (SapAttributeDict.ContainsKey(dt.Columns[j].ColumnName.ToString()))
                                                {
                                                    DataRow[] selDRs = dtSapAattribute.Select("KeyField ='" + SapAttributeDict[dt.Columns[j].ColumnName.ToString()] + "'");
                                                    if (selDRs.Length > 0)
                                                    {
                                                        foreach (DataRow selDR in selDRs)
                                                        {
                                                            if (selDR["KeyValue"].ToString() != "" && (isNumeric = selDR["KeyValue"].ToString().All(char.IsDigit)))
                                                                dr[dt.Columns[j].ColumnName] = Convert.ToInt32(selDR["KeyValue"].ToString());
                                                        }
                                                    }
                                                }
                                                break;

                                            case "Int64":
                                                if (SapAttributeDict.ContainsKey(dt.Columns[j].ColumnName.ToString()))
                                                {
                                                    DataRow[] selDRs = dtSapAattribute.Select("KeyField ='" + SapAttributeDict[dt.Columns[j].ColumnName.ToString()] + "'");
                                                    if (selDRs.Length > 0)
                                                    {
                                                        foreach (DataRow selDR in selDRs)
                                                        {
                                                            if (selDR["KeyValue"].ToString() != "" && (isNumeric = selDR["KeyValue"].ToString().All(char.IsDigit)))
                                                            {
                                                                dr[dt.Columns[j].ColumnName] = Convert.ToInt64(selDR["KeyValue"].ToString());
                                                                strPLDBID = selDR["KeyValue"].ToString();
                                                            }
                                                        }
                                                    }
                                                }
                                                break;

                                            case "Double":

                                                if (SapAttributeDict.ContainsKey(dt.Columns[j].ColumnName.ToString()))
                                                {
                                                    DataRow[] selDRs = dtSapAattribute.Select("KeyField ='" + SapAttributeDict[dt.Columns[j].ColumnName.ToString()] + "'");
                                                    if (selDRs.Length > 0)
                                                    {
                                                        foreach (DataRow selDR in selDRs)
                                                        {
                                                            if (selDR["KeyValue"].ToString() != "")
                                                                dr[dt.Columns[j].ColumnName] = Convert.ToDouble(selDR["KeyValue"].ToString());

                                                        }
                                                    }
                                                }
                                                break;

                                        }
                                    }

                                    if (!string.IsNullOrEmpty(strPLDBID))
                                    {
                                        dt.Rows.Add(dr);
                                        strPLDBID = string.Empty;
                                    }

                                }
                            }
                        }
                    }
                }

                dt.EndLoadData();
                // end code 
            }
            catch (Exception ex)
            {
                if (string.IsNullOrEmpty(Program.Errorcomment))
                {
                    Program.Errorcomment = "Method Name:-ValidateDataAndInsertInDataTable " + ex.Message.ToString();

                }
                else
                {
                    Program.Errorcomment = Program.Errorcomment + Environment.NewLine + "Method Name:-ValidateDataAndInsertInDataTable " + ex.Message.ToString();
                }
                Logger.Error("Method Name:-ValidateDataAndInsertInDataTable " + ex.Message.ToString());
            }
            return dt;
        }

        /// <summary>
        /// This function is used for update ststus 
        /// </summary>
        /// <param name="PLDBID"></param>
        /// <param name="status"></param>
        /// <returns> bool</returns>
        private bool updateInterfaceStatusSENT(string PLDBID, string status)
        {
            bool IsupdateInterfaceStatusSENT = false;
            try
            {
                if (!string.IsNullOrEmpty(PLDBID))
                {
                    DBHelper objDBhelper = new DBHelper(AppConstants.DB_Code);
                    string query = string.Format("update {1} set INTERFACE_STATUS= '{2}', LASTMODIFED_DATE = sysdate where PLDBID = '{0}'", PLDBID, AppConstants.DB_ED06_GISPLDB_INTERFACE, status);
                    objDBhelper.ExecuteScalarPGEData(query);
                    IsupdateInterfaceStatusSENT = true;
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Method Name:-updateInterfaceStatusSENT " + ex.Message.ToString());
            }
            return IsupdateInterfaceStatusSENT;
        }
        /// <summary>
        /// This function is used for Remove Curly braces In GUID
        /// </summary>
        /// <param name="PGE_GLOBALID"></param>
        /// <returns> string</returns>
        public string RemoveCurlybracesInGUID(string PGE_GLOBALID)
        {
            string strPGE_GLOBALID = string.Empty;
            try
            {

                strPGE_GLOBALID= Regex.Replace(PGE_GLOBALID, @"[\{\}]", "");
            }
            catch (Exception exp)
            {
                Logger.Info("Method Name:-RemoveCurlybracesInGUID " + exp.Message.ToString());
            }
            return strPGE_GLOBALID;
        }
        /// <summary>
        /// This function is used for cerate Ed006 Data table at run time 
        /// </summary>        
        /// <returns>DataTable </returns>
        public System.Data.DataTable DefineEd06InterfaceTable()
        {
            System.Data.DataTable GIS_TLDB_Table = new System.Data.DataTable("Ed06_GISPLDB_Interface");
            try
            {
               
                var FieldMappingSection = ConfigurationManager.GetSection(AppConstants.CNFG_ED06_FIELDMAPPINGSECTION) as NameValueCollection;
                if (FieldMappingSection.Count > 0)
                {

                    System.Data.DataColumn EDIT_TYPE = new DataColumn(FieldMappingSection[AppConstants.CNFG_ED06_FIELD_EDIT_TYPE], typeof(string));
                    EDIT_TYPE.AllowDBNull = true;
                    GIS_TLDB_Table.Columns.Add(EDIT_TYPE);

                    System.Data.DataColumn ID_TYPE = new DataColumn(FieldMappingSection[AppConstants.CNFG_ED06_FIELD_ID_TYPE], typeof(string));
                    ID_TYPE.AllowDBNull = true;
                    GIS_TLDB_Table.Columns.Add(ID_TYPE);  
   
                    System.Data.DataColumn PGE_SAPEquipID = new DataColumn(FieldMappingSection[AppConstants.CNFG_ED06_FIELD_PGE_SAPEquipID], typeof(string));
                    PGE_SAPEquipID.AllowDBNull = true;
                    GIS_TLDB_Table.Columns.Add(PGE_SAPEquipID);

                    System.Data.DataColumn PGE_OrderNumber = new DataColumn(FieldMappingSection[AppConstants.CNFG_ED06_FIELD_PGE_OrderNumber], typeof(string));
                    PGE_OrderNumber.AllowDBNull = true;
                    GIS_TLDB_Table.Columns.Add(PGE_OrderNumber);


                    System.Data.DataColumn Latitude = new DataColumn(FieldMappingSection[AppConstants.CNFG_ED06_FIELD_Latitude], typeof(Double));
                    Latitude.AllowDBNull = true;
                    GIS_TLDB_Table.Columns.Add(Latitude);

                    System.Data.DataColumn Longitude = new DataColumn(FieldMappingSection[AppConstants.CNFG_ED06_FIELD_Longitude], typeof(Double));
                    Longitude.AllowDBNull = true;
                    GIS_TLDB_Table.Columns.Add(Longitude);
                   
                    System.Data.DataColumn PGE_GLOBALID = new DataColumn(FieldMappingSection[AppConstants.CNFG_ED06_FIELD_PGE_GLOBALID], typeof(string));
                    PGE_GLOBALID.AllowDBNull = true;
                    GIS_TLDB_Table.Columns.Add(PGE_GLOBALID);

                    System.Data.DataColumn Parent_GUID = new DataColumn(FieldMappingSection[AppConstants.CNFG_ED06_FIELD_Parent_GUID], typeof(string));
                    Parent_GUID.AllowDBNull = true;
                    GIS_TLDB_Table.Columns.Add(Parent_GUID);
                    
                    System.Data.DataColumn PLDBID = new DataColumn(FieldMappingSection[AppConstants.CNFG_ED06_FIELD_PLDBID], typeof(Int64));
                    PLDBID.AllowDBNull = true;
                    GIS_TLDB_Table.Columns.Add(PLDBID);

                    System.Data.DataColumn Created_Date = new DataColumn(FieldMappingSection[AppConstants.CNFG_ED06_FIELD_CREATED_DATE], typeof(DateTime));
                    Created_Date.AllowDBNull = true;
                    GIS_TLDB_Table.Columns.Add(Created_Date);

                    System.Data.DataColumn LastModifed_Date = new DataColumn(FieldMappingSection[AppConstants.CNFG_ED06_FIELD_LastModifed_Date], typeof(DateTime));
                    LastModifed_Date.AllowDBNull = true;
                    GIS_TLDB_Table.Columns.Add(LastModifed_Date);

                    System.Data.DataColumn INTERFACE_STATUS = new DataColumn(FieldMappingSection[AppConstants.CNFG_ED06_FIELD_INTERFACE_STATUS], typeof(string));
                    INTERFACE_STATUS.AllowDBNull = true;
                    GIS_TLDB_Table.Columns.Add(INTERFACE_STATUS);

                    System.Data.DataColumn Retry_Count = new DataColumn(FieldMappingSection[AppConstants.CNFG_ED06_FIELD_Retry_Count], typeof(string));
                    INTERFACE_STATUS.AllowDBNull = true;
                    GIS_TLDB_Table.Columns.Add(Retry_Count);

                  
                }
            
              
            }
            catch (Exception ex)
            {
                if (string.IsNullOrEmpty(Program.Errorcomment))
                {
                    Program.Errorcomment = "Method Name: -DefineEd06InterfaceTable " + ex.Message.ToString();

                }
                else
                {
                    Program.Errorcomment = Program.Errorcomment + Environment.NewLine + "Method Name:-DefineEd06InterfaceTable " + ex.Message.ToString();
                }
                Logger.Error("Method Name:-DefineEd06InterfaceTable " + ex.Message.ToString());
                
                //  fileWriter.WriteLine(" Error Occurred at " + DateTime.Now + " " + ex);
            }
            return GIS_TLDB_Table;
        }
        #region Commented Code 
        //public string[] getTodaysFiles(String folderPath)
        //{
        //    List<String> todaysFiles = new List<String>();
        //    foreach (String file in Directory.GetFiles(folderPath))
        //    {
        //        DirectoryInfo di = new DirectoryInfo(file);
        //        if (di.LastWriteTime.ToShortDateString().Equals(DateTime.Now.ToShortDateString()))
        //           todaysFiles.Add(file);
        //    }
        //    string[] strArray = todaysFiles.ToArray();
        //    return strArray;
        //}
        #endregion
        /// <summary>
        /// This function is used for get all todys ED006 files 
        /// </summary>
        /// <param name="folderPath"></param>
        /// <returns> List</returns>
        public List<String> getTodaysFiles(String folderPath)
        {
                    
            List<String> todaysFiles = new List<String>();
            string getLatestRecordDate = string.Empty;
            try
            {
                var directory = new DirectoryInfo(folderPath);
                if (directory.GetFiles().Length > 0)
                {
                     var myFile = directory.GetFiles()
                    .OrderByDescending(f => f.LastWriteTime)
                    .First();
                    getLatestRecordDate=myFile.LastWriteTime.ToShortDateString();
                }
                
                foreach (String file in Directory.GetFiles(folderPath))
                {
                    DirectoryInfo di = new DirectoryInfo(file);
                    if (di.LastWriteTime.ToShortDateString().Equals(getLatestRecordDate))
                        todaysFiles.Add(file);
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Method Name:-getTodaysFiles " + ex.Message.ToString());
            }
            return todaysFiles;
        }
        /// <summary>
        /// This function is used for check Todays file 
        /// </summary>
        /// <returns> void</returns>
        public void CheckTodaysFile()
        {
            try
            {
                DateTime today = DateTime.Now.Date;
                var loc = new DirectoryInfo("C:\\");

                var fileList = loc.GetFiles().Where(x => x.CreationTime.ToString("dd/MM/yyyy") == today.ToString());
                foreach (FileInfo fileItem in fileList)
                {
                    //Process the file
                }
            }
            catch (Exception exp)
            {
                Logger.Error("Method Name:-CheckTodaysFile " + exp.Message.ToString());
            }
        }
        /// <summary>
        /// This function is used for fatching records from PGE_GISSAP_ASSETSYNCH table 
        /// </summary>
       /// <returns> DataTable</returns>
        public DataTable GetRecordFromPGE_GISSAP_ASSETSYNCH()
         {
             DataTable dtSAPATTRIBUTES = new DataTable();
             string strdate = string.Empty;
             
             try
             {

                 strdate = GetRecordsDate();
                 if (!string.IsNullOrEmpty(strdate))
                 {
                     string query = string.Format("select SAPATTRIBUTES  from {0} where (SAPATTRIBUTES is not null and (to_char(SAPATTRIBUTES) <>'N')) and Assettype='99' and DATEPROCESSED> '" + strdate + "'", AppConstants.DB_PGE_GISSAP_ASSETSYNCH);
                     DBHelper objDB1 = new DBHelper(AppConstants.DB_Code, Logger);
                     dtSAPATTRIBUTES = objDB1.SelectPGEDataQuery(query, false);
                 }
               
             }
             catch (Exception exp)
             {
                if (string.IsNullOrEmpty(Program.Errorcomment))
                {
                    Program.Errorcomment = "Method Name:-GetRecordFromPGE_GISSAP_ASSETSYNCH " + exp.Message.ToString();

                }
                else
                {
                    Program.Errorcomment = Program.Errorcomment + Environment.NewLine + "Method Name:-GetRecordFromPGE_GISSAP_ASSETSYNCH " + exp.Message.ToString();
                }
                Logger.Error("Method Name:-GetRecordFromPGE_GISSAP_ASSETSYNCH " + exp.Message.ToString());
             }

             return dtSAPATTRIBUTES;
         }
        /// <summary>
        /// This function is used for convert json string into datatable 
        /// </summary>
        /// <param name="jsontext"></param>
        /// <returns> string</returns>
        private DataTable ConvertJsonstringtoDataTable(string jsontext)
        {
            DataTable DTFromJsontext = new DataTable();
            try
            {
                DataSet dataSet = JsonConvert.DeserializeObject<DataSet>(jsontext);
                DTFromJsontext = dataSet.Tables["NAV_ED06_ST_KF_KV"];
            }
            catch (Exception exp)
            {
                if (string.IsNullOrEmpty(Program.Errorcomment))
                {
                    Program.Errorcomment = "Method Name:-ReadEd06fromCSV " + exp.Message.ToString();

                }
                else
                {
                    Program.Errorcomment = Program.Errorcomment + Environment.NewLine + "Method Name:-ReadEd06fromCSV " + exp.Message.ToString();
                }
                Logger.Error("Method Name:-ConvertJsonstringtoDataTable " + exp.Message.ToString());
            }
            return DTFromJsontext;
        }
        /// <summary>
        /// This function is used for get record date 
        /// </summary>
        /// <returns> string</returns>
        private string GetRecordsDate()
        {
          
            DataTable dt = new DataTable();
            string strRecordDate = string.Empty;
            string query = string.Empty;
            var cultureInfo = new CultureInfo("en-US");
            try
            {
                query = string.Format("select max(CREATED_DATE) as dateProcess  from {0}  order by CREATED_DATE DESC", AppConstants.DB_ED06_GISPLDB_INTERFACE);
                DBHelper objDB1 = new DBHelper(AppConstants.DB_Code, Logger);
                dt = objDB1.SelectPGEDataQuery(query, false);
                if (dt != null && dt.Rows.Count > 0)
                {
                    if (!string.IsNullOrEmpty(dt.Rows[0]["dateProcess"].ToString()))
                    { 
                        strRecordDate = DateTime.Parse(dt.Rows[0]["dateProcess"].ToString(), CultureInfo.InvariantCulture).ToString("dd-MMM-yyyy");
                    }
                    else
                    {
                        query = string.Format("select  max(DATEPROCESSED) as dateProcess  from {0} where (SAPATTRIBUTES is not null and (to_char(SAPATTRIBUTES) <>'N')) and Assettype='99'  order by DATEPROCESSED DESC", AppConstants.DB_PGE_GISSAP_ASSETSYNCH);
                        objDB1 = new DBHelper(AppConstants.DB_Code, Logger);
                        dt = objDB1.SelectPGEDataQuery(query, false);
                        if (dt != null && dt.Rows.Count > 0)
                        {

                            if (!string.IsNullOrEmpty(dt.Rows[0]["dateProcess"].ToString()))
                            {
                               
                                strRecordDate = DateTime.Parse(dt.Rows[0]["dateProcess"].ToString(), CultureInfo.InvariantCulture).AddDays(-1).ToString("dd-MMM-yyyy");
                            }
                        }
                    }
                }
               
            }
            catch (Exception exp)
            {
                if (string.IsNullOrEmpty(Program.Errorcomment))
                {
                    Program.Errorcomment = "Method Name:-ReadEd06fromCSV " + exp.Message.ToString();

                }
                else
                {
                    Program.Errorcomment = Program.Errorcomment + Environment.NewLine + "Method Name:-ReadEd06fromCSV " + exp.Message.ToString();
                }
                Logger.Error("Method Name:-GetRecordsDate " + exp.Message.ToString());
            }
            return strRecordDate;
        }
      
    }
}
