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
using PGE.BatchApplication.GISPLDBAssetSynchProcess.DAL;
using log4net.Repository.Hierarchy;
using System.Globalization;
using System.Text.RegularExpressions;
namespace PGE.BatchApplication.GISPLDBAssetSynchProcess.Common
{
    
    public class CommonUtil
    {
       
        private log4net.ILog Logger = null;
        public CommonUtil()
        {

            InitializeLogger();
        }


        public DataTable GetCSVDataInTable(string filePath)
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
            return ds.Tables[0];
        }
        public DataTable ReadCSVDataInTableandMap(string filePath,string SectionName)
        {
            List<String> filePaths = getTodaysFiles(filePath);
           // string sPath = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\Logfile_" + DateTime.Now.Ticks.ToString() + ".log";

            DataTable csVDataTable = new DataTable();
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
            
//            string firstFilepath = filePaths[0].ToString();

  
            return csVDataTable;
 
        }
        public DataTable UpdateSapEquipIDandCopyTable(DataTable ed06DataTable, DataTable ed07DataTable)
        {
            DataTable ed06TempCopy = new DataTable();
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
            return ed06TempCopy;
        }
        private string[] GetLatestUpdatedCSV(string filedirectory)
        {
            
            List<string> FileList = new List<string>();
            DateTime today = DateTime.Now.Date;
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
                if (fi.LastWriteTime.ToShortDateString().Equals(DateTime.Now.ToShortDateString()))
                    FileList.Add(fi.FullName);
                else Logger.Info("No Updated CSV files presented in Folder");
            }

            return FileList.ToArray();
        }
        public bool BulkInsertintoED06(DataTable DTable06)
        {
            if ((DTable06 != null) && (DTable06.Rows.Count > 0))
            {
                DBHelper objDB1 = new DBHelper(AppConstants.DB_Code);
                objDB1.BulkInsert(DTable06, false);
                return true;
            }
            else return false;
        }
        public DataTable GetDataFromED06CSV(string Path)
        {
            DataTable dt = DefineEd06InterfaceTable();
            try
            {
                //DataTable dt = DefineEd06InterfaceTable();
                 if (dt != null)
                {
                      //string path = ConfigurationManager.AppSettings["NAS_FILE_LOCATION_ED06"];
                    string[] files = GetLatestUpdatedCSV(Path);
                    dt = ValidateDataAndInsertInDataTable(dt, files);
                }

            }
            catch (Exception ex)
            {
                Logger.Info("Error in reading CSV" + ex);
  
            }
            return dt;
        }
        
        public void WriteTable()
        {
            try
            {
                DataTable dt = DefineEd06InterfaceTable();
                 if (dt != null)
                {
                   
                    //string path = ConfigurationManager.AppSettings["NAS_FILE_LOCATION_ED06"];
                    string[] files = GetLatestUpdatedCSV(ConfigurationManager.AppSettings["NAS_FILE_LOCATION_ED06"].ToString());
                    dt = ValidateDataAndInsertInDataTable(dt, files);
                    if ((dt != null) && (dt.Rows.Count > 0))
                    {
                        DBHelper objDB1 = new DBHelper(AppConstants.DB_Code);
                        objDB1.BulkInsert(dt, false);
                    }
                    else Logger.Info("No Data found to Insert ");
                }

            }
            catch (Exception ex)
            {
                Logger.Info("Error in reading CSV" + ex);
                // fileWriter.WriteLine(" Error Occurred at " + DateTime.Now + " " + ex);
               // return false;
            }
        }
        public DataTable ValidateDataAndInsertInDataTable(DataTable dt, string[] files)
        {
            dt.BeginLoadData();
            foreach (var item in files)
            {
                var reader = new StreamReader(item);
                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine();
                    string[] values = line.Split(',');
                    //HFRA-HFTD Project : As New Field HFRA is added in Support Structure and included in ED06 to send SAP
                    //hence total field count in changed from 37 to 38
                    if (!string.IsNullOrEmpty(line) && values.Length == 38)
                    {

                        DataRow dr = dt.NewRow();

                        for (int j = 0; j < dt.Columns.Count; j++)
                        {

                            string ColType = dt.Columns[j].DataType.Name;
                            switch (ColType)
                            {
                                case "String":
                                    if (dt.Columns[j].ColumnName.ToString() == "INTERFACE_STATUS")
                                        dr[dt.Columns[j].ColumnName] = "NEW";
                                    else
                                    {
                                        if (values[j] != "")
                                        {
                                            dr[dt.Columns[j].ColumnName] = values[j];
                                        }
                                    }
                                    break;

                                case "DateTime":
                                    var cultureInfo = new CultureInfo("en-US");
                                    if (values[j] != "")
                                    {
                                        string result = DateTime.ParseExact(values[j], "yyyyMMdd", CultureInfo.InvariantCulture).ToString("dd-MMM-yyyy");
                                        //  DateTime date = Convert.ToDateTime(result);
                                        dr[dt.Columns[j].ColumnName] = result;
                                    }
                                    break;
                                case "Int32":
                                    if (values[j] != "")
                                        dr[dt.Columns[j].ColumnName] = Convert.ToInt32(values[j]);
                                    break;

                                case "Int64":
                                    if (values[j] != "")
                                        dr[dt.Columns[j].ColumnName] = Convert.ToInt64(values[j]);
                                    break;
                                case "Double":
                                    if (values[j] != "")
                                        dr[dt.Columns[j].ColumnName] = Convert.ToDouble(values[j]);
                                    break;

                            }

                        }

                        dt.Rows.Add(dr);

                    }
                    else
                    {
                        Logger.Info("Couldn't read CSV");
                        //Logg.WriteLine(" Error Occurred at--- " + line);
                    }
                }

                reader.Close();
            }
            dt.EndLoadData();

            return dt;
        }
        static List<string> GetTLDBClasses(string nameSpace)
        {
            Assembly asm = Assembly.GetExecutingAssembly();

            List<string> namespacelist = new List<string>();
            List<string> classlist = new List<string>();

            foreach (Type type in asm.GetTypes())
            {
                if (type.Namespace == nameSpace)
                    namespacelist.Add(type.Name);
            }

            foreach (var classname in namespacelist)
            {
                if (classname.Contains("TLDB"))
                    classlist.Add(classname);
            }

            return classlist;
        }
        static IEnumerable<string> GetClasses(string nameSpace)
        {

            Assembly asm = Assembly.GetExecutingAssembly();
            return asm.GetTypes()
                .Where(type => type.Namespace == nameSpace && type.Name.Contains("TLDB"))
                .Select(type => type.Name);
        }
        private bool updateInterfaceStatusSENT(string PLDBID, string status)
        {
            try
            {
                Logger.Info(" \t updateInterfaceStatusSENT Start");
                if (PLDBID == null || PLDBID.Trim() == string.Empty)
                {
                    throw new Exception("updateInterfaceStatusSENT --  BatchID can not be null .. ");
                }
                DBHelper objDBhelper = new DBHelper(AppConstants.DB_Code, Logger);
                string query = string.Format("update {1} set INTERFACE_STATUS= '{2}', LASTMODIFED_DATE = sysdate where PLDBID = '{0}'", PLDBID, AppConstants.DB_ED06_GISPLDB_INTERFACE, status);
                objDBhelper.ExecuteScalarPGEData(query);
                Logger.Info(" \t updateInterfaceStatusSENT Complete ..");
                return true;
            }
            catch (Exception Ex)
            {
                Logger.Error(" \t  \t  Error In updateInterfaceStatusSENT Method", Ex);
                return false;
                //throw Ex;
            }
        }
        public string RemoveCurlybracesInGUID(string PGE_GLOBALID)
        {
            return Regex.Replace(PGE_GLOBALID, @"[\{\}]", "");
        }
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

                    System.Data.DataColumn LocDesc2 = new DataColumn(FieldMappingSection[AppConstants.CNFG_ED06_FIELD_LocDesc2], typeof(string));
                    LocDesc2.AllowDBNull = true;
                    GIS_TLDB_Table.Columns.Add(LocDesc2);

                    System.Data.DataColumn Barcode = new DataColumn(FieldMappingSection[AppConstants.CNFG_ED06_FIELD_Barcode], typeof(string));
                    Barcode.AllowDBNull = true;
                    GIS_TLDB_Table.Columns.Add(Barcode);

                    System.Data.DataColumn Class = new DataColumn(FieldMappingSection[AppConstants.CNFG_ED06_FIELD_Class], typeof(Int32));
                    Class.AllowDBNull = true;
                    GIS_TLDB_Table.Columns.Add(Class);

                    System.Data.DataColumn Material = new DataColumn(FieldMappingSection[AppConstants.CNFG_ED06_FIELD_Material], typeof(string));
                    Material.AllowDBNull = true;
                    GIS_TLDB_Table.Columns.Add(Material);

                    System.Data.DataColumn Species = new DataColumn(FieldMappingSection[AppConstants.CNFG_ED06_FIELD_Species], typeof(string));
                    Species.AllowDBNull = true;
                    GIS_TLDB_Table.Columns.Add(Species);

                    System.Data.DataColumn Height = new DataColumn(FieldMappingSection[AppConstants.CNFG_ED06_FIELD_Height], typeof(Int32));
                    Height.AllowDBNull = true;
                    GIS_TLDB_Table.Columns.Add(Height);

                    System.Data.DataColumn JPNumber = new DataColumn(FieldMappingSection[AppConstants.CNFG_ED06_FIELD_JPNumber], typeof(string));
                    JPNumber.AllowDBNull = true;
                    GIS_TLDB_Table.Columns.Add(JPNumber);

                    System.Data.DataColumn Customer_Owned = new DataColumn(FieldMappingSection[AppConstants.CNFG_ED06_FIELD_Customer_Owned], typeof(string));
                    Customer_Owned.AllowDBNull = true;
                    GIS_TLDB_Table.Columns.Add(Customer_Owned);

                    System.Data.DataColumn Original_Circumference = new DataColumn(FieldMappingSection[AppConstants.CNFG_ED06_FIELD_Original_Circumference], typeof(Int32));
                    Original_Circumference.AllowDBNull = true;
                    GIS_TLDB_Table.Columns.Add(Original_Circumference);

                    System.Data.DataColumn Original_Treatment_Type = new DataColumn(FieldMappingSection[AppConstants.CNFG_ED06_FIELD_Original_Treatment_Type], typeof(string));
                    Original_Treatment_Type.AllowDBNull = true;
                    GIS_TLDB_Table.Columns.Add(Original_Treatment_Type);

                    System.Data.DataColumn PGE_SAPEquipID = new DataColumn(FieldMappingSection[AppConstants.CNFG_ED06_FIELD_PGE_SAPEquipID], typeof(string));
                    PGE_SAPEquipID.AllowDBNull = true;
                    GIS_TLDB_Table.Columns.Add(PGE_SAPEquipID);

                    System.Data.DataColumn Installation_Date = new DataColumn(FieldMappingSection[AppConstants.CNFG_ED06_FIELD_Installation_Date], typeof(DateTime));
                    Installation_Date.AllowDBNull = true;
                    GIS_TLDB_Table.Columns.Add(Installation_Date);

                    System.Data.DataColumn PGE_OrderNumber = new DataColumn(FieldMappingSection[AppConstants.CNFG_ED06_FIELD_PGE_OrderNumber], typeof(string));
                    PGE_OrderNumber.AllowDBNull = true;
                    GIS_TLDB_Table.Columns.Add(PGE_OrderNumber);

                    System.Data.DataColumn Manufacturer = new DataColumn(FieldMappingSection[AppConstants.CNFG_ED06_FIELD_Manufacturer], typeof(string));
                    Manufacturer.AllowDBNull = true;
                    GIS_TLDB_Table.Columns.Add(Manufacturer);

                    System.Data.DataColumn Manufactured_Year = new DataColumn(FieldMappingSection[AppConstants.CNFG_ED06_FIELD_Manufactured_Year], typeof(Int32));
                    Manufactured_Year.AllowDBNull = true;
                    GIS_TLDB_Table.Columns.Add(Manufactured_Year);

                    System.Data.DataColumn Loc_Desc1 = new DataColumn(FieldMappingSection[AppConstants.CNFG_ED06_FIELD_Loc_Desc1], typeof(string));
                    Manufactured_Year.AllowDBNull = true;
                    GIS_TLDB_Table.Columns.Add(Loc_Desc1);

                    System.Data.DataColumn Local_Office_Id = new DataColumn(FieldMappingSection[AppConstants.CNFG_ED06_FIELD_Local_Office_Id], typeof(string));
                    Manufactured_Year.AllowDBNull = true;
                    GIS_TLDB_Table.Columns.Add(Local_Office_Id);

                    System.Data.DataColumn Latitude = new DataColumn(FieldMappingSection[AppConstants.CNFG_ED06_FIELD_Latitude], typeof(Double));
                    Latitude.AllowDBNull = true;
                    GIS_TLDB_Table.Columns.Add(Latitude);

                    System.Data.DataColumn Longitude = new DataColumn(FieldMappingSection[AppConstants.CNFG_ED06_FIELD_Longitude], typeof(Double));
                    Longitude.AllowDBNull = true;
                    GIS_TLDB_Table.Columns.Add(Longitude);

                    System.Data.DataColumn MapNumber = new DataColumn(FieldMappingSection[AppConstants.CNFG_ED06_FIELD_MapNumber], typeof(string));
                    MapNumber.AllowDBNull = true;
                    GIS_TLDB_Table.Columns.Add(MapNumber);

                    System.Data.DataColumn Circuit_MapNumber = new DataColumn(FieldMappingSection[AppConstants.CNFG_ED06_FIELD_Circuit_MapNumber], typeof(string));
                    Circuit_MapNumber.AllowDBNull = true;
                    GIS_TLDB_Table.Columns.Add(Circuit_MapNumber);

                    System.Data.DataColumn City = new DataColumn(FieldMappingSection[AppConstants.CNFG_ED06_FIELD_City], typeof(string));
                    City.AllowDBNull = true;
                    GIS_TLDB_Table.Columns.Add(City);

                    System.Data.DataColumn County = new DataColumn(FieldMappingSection[AppConstants.CNFG_ED06_FIELD_County], typeof(Int32));
                    County.AllowDBNull = true;
                    GIS_TLDB_Table.Columns.Add(County);

                    System.Data.DataColumn Zip = new DataColumn(FieldMappingSection[AppConstants.CNFG_ED06_FIELD_Zip], typeof(string));
                    Zip.AllowDBNull = true;
                    GIS_TLDB_Table.Columns.Add(Zip);

                    System.Data.DataColumn PGE_GLOBALID = new DataColumn(FieldMappingSection[AppConstants.CNFG_ED06_FIELD_PGE_GLOBALID], typeof(string));
                    PGE_GLOBALID.AllowDBNull = true;
                    GIS_TLDB_Table.Columns.Add(PGE_GLOBALID);

                    System.Data.DataColumn Parent_GUID = new DataColumn(FieldMappingSection[AppConstants.CNFG_ED06_FIELD_Parent_GUID], typeof(Int32));
                    Parent_GUID.AllowDBNull = true;
                    GIS_TLDB_Table.Columns.Add(Parent_GUID);

                    System.Data.DataColumn LastUser = new DataColumn(FieldMappingSection[AppConstants.CNFG_ED06_FIELD_LastUser], typeof(string));
                    LastUser.AllowDBNull = true;
                    GIS_TLDB_Table.Columns.Add(LastUser);

                    System.Data.DataColumn LastModifed_Date = new DataColumn(FieldMappingSection[AppConstants.CNFG_ED06_FIELD_LastModifed_Date], typeof(DateTime));
                    LastModifed_Date.AllowDBNull = true;
                    GIS_TLDB_Table.Columns.Add(LastModifed_Date);

                    System.Data.DataColumn GEMS_Map_Office = new DataColumn(FieldMappingSection[AppConstants.CNFG_ED06_FIELD_GEMS_Map_Office], typeof(string));
                    GEMS_Map_Office.AllowDBNull = true;
                    GIS_TLDB_Table.Columns.Add(GEMS_Map_Office);

                    System.Data.DataColumn SubtypeCD = new DataColumn(FieldMappingSection[AppConstants.CNFG_ED06_FIELD_SubtypeCD], typeof(Int32));
                    SubtypeCD.AllowDBNull = true;
                    GIS_TLDB_Table.Columns.Add(SubtypeCD);

                    System.Data.DataColumn PoleUse = new DataColumn(FieldMappingSection[AppConstants.CNFG_ED06_FIELD_PoleUse], typeof(Int32));
                    PoleUse.AllowDBNull = true;
                    GIS_TLDB_Table.Columns.Add(PoleUse);

                    System.Data.DataColumn PTTDIDC = new DataColumn(FieldMappingSection[AppConstants.CNFG_ED06_FIELD_PTTDIDC], typeof(string));
                    PTTDIDC.AllowDBNull = true;
                    GIS_TLDB_Table.Columns.Add(PTTDIDC);

                    System.Data.DataColumn PLDBID = new DataColumn(FieldMappingSection[AppConstants.CNFG_ED06_FIELD_PLDBID], typeof(Int64));
                    PLDBID.AllowDBNull = true;
                    GIS_TLDB_Table.Columns.Add(PLDBID);

                    System.Data.DataColumn Comments = new DataColumn(FieldMappingSection[AppConstants.CNFG_ED06_FIELD_Comments], typeof(string));
                    Comments.AllowDBNull = true;
                    GIS_TLDB_Table.Columns.Add(Comments);

                    System.Data.DataColumn HFTD = new DataColumn(FieldMappingSection[AppConstants.CNFG_ED06_FIELD_HFTD], typeof(Int32));
                    HFTD.AllowDBNull = true;
                    GIS_TLDB_Table.Columns.Add(HFTD);

                    System.Data.DataColumn INTERFACE_STATUS = new DataColumn(FieldMappingSection[AppConstants.CNFG_ED06_FIELD_INTERFACE_STATUS], typeof(string));
                    INTERFACE_STATUS.AllowDBNull = true;
                    GIS_TLDB_Table.Columns.Add(INTERFACE_STATUS);
                }
                else Logger.Info("Missing Ed06 Field Section Or vlaues ");
              
            }
            catch (Exception ex)
            {
                Logger.Info("Missing Ed06 Field Section Or vlaues"+ex.Message);
                //  fileWriter.WriteLine(" Error Occurred at " + DateTime.Now + " " + ex);
            }
            return GIS_TLDB_Table;
        }
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
        public List<String> getTodaysFiles(String folderPath)
        {
            List<String> todaysFiles = new List<String>();
            foreach (String file in Directory.GetFiles(folderPath))
            {
                DirectoryInfo di = new DirectoryInfo(file);
                if (di.LastWriteTime.ToShortDateString().Equals(DateTime.Now.ToShortDateString()))
                    todaysFiles.Add(file);
            }
            return todaysFiles;
        }
        public void CheckTodaysFile()
        {
            DateTime today = DateTime.Now.Date;
            var loc = new DirectoryInfo("C:\\");

            var fileList = loc.GetFiles().Where(x => x.CreationTime.ToString("dd/MM/yyyy") == today.ToString());
            foreach (FileInfo fileItem in fileList)
            {
                //Process the file
            }
        }
         private void InitializeLogger()
        {
            string executingAssemblyPath = string.Empty;
            try
            {
                //Get log file with complete path
                executingAssemblyPath = GetLogfilepath();

                if (executingAssemblyPath.Trim().Length == 0)
                {
                    executingAssemblyPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
                }
                string logPath = System.IO.Path.Combine(executingAssemblyPath, @"GISSAPAssetService_" + DateTime.Now.ToString("MM_dd_yyyy") + ".log");

                //log4net 
                log4net.GlobalContext.Properties["LogName"] = logPath;
                //string executingAssemblyPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);

                executingAssemblyPath = (System.IO.Path.Combine(executingAssemblyPath, "logging.xml"));

                StreamWriter wr = new StreamWriter(executingAssemblyPath);
                wr.Write(Getlog4netConfig());
                wr.Close();
                FileInfo fi = new FileInfo(executingAssemblyPath);
                log4net.Config.XmlConfigurator.Configure(fi);

                //Initialize the logging 
                Logger = log4net.LogManager.GetLogger(typeof(GISPLDBAssetSyncTemplateService));
                Logger.Info("Logger Initialized");
            }
            catch (Exception ex)
            {
                Logger.Info(ex.Message);
            }
        }
         //public class TLDBInsertDictTemplate : IEnumerable 
         //{
         //    public TLDBInsertDictTemplate() { }

         //    [DataMember]
         //    public List<Dictionary<string, string>> ASSET = new List<Dictionary<string, string>>();

         //    public IEnumerator GetEnumerator()
         //    {
         //        foreach (object o in ASSET)
         //        {
         //            if (o == null)
         //            {
         //                break;
         //            }
         //            yield return o;
         //        }
         //    }  

         //}
         //public class TLDBUpdateDictTemplate
         //{
         //    public TLDBUpdateDictTemplate() { }


         //    [DataMember]
         //    public List<Dictionary<string, string>> ASSET = new List<Dictionary<string, string>>();

         //}

         //public class TLDBDeleteDictTemplate
         //{
         //    public TLDBDeleteDictTemplate() { }


         //    [DataMember]
         //    public List<Dictionary<string, string>> ASSET = new List<Dictionary<string, string>>();

         //}
        private string GetLogfilepath()
        {
            try
            {
                string executingAssemblyPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
                //string logPath = (System.IO.Path.Combine(executingAssemblyPath, "C:\\Document\\Logs\\GIStoSAPLog.txt"));
                string path = System.Configuration.ConfigurationManager.AppSettings["LogFolder"];
                //string logPath = string.Format( path + "\\GIStoSAPLog_{0}.txt", DateTime.Now.ToShortDateString().Replace("/", "_"));
                return path;
            }
            catch (Exception ex)
            {
                Logger.Info(ex.Message);
            }
            return "";
        }
        /// <summary>
        /// Getlog4netConfig
        /// </summary>
        /// <returns></returns>
        /// private 
        /// 
        
        private string Getlog4netConfig()
        {

            string Config = @"<?xml version=""1.0"" encoding=""utf-8"" ?> 
                                    <configuration> 
                                      <log4net debug=""False""> 
                                        <!-- Define some output appenders --> 
                                        <appender name=""RollingLogFileAppender"" type=""log4net.Appender.RollingFileAppender""> 
                                          <file type=""log4net.Util.PatternString"" value=""%property{LogName}"" /> 
                                          <appendToFile value=""true"" /> 
                                          <rollingStyle value=""Composite"" />                               
                                         <datePattern value=""ddMMyyyy"" />
                                          <maxSizeRollBackups value=""30"" /> 
                                          <maximumFileSize value=""1MB"" />                                                                                    
                                        <layout type=""log4net.Layout.PatternLayout"">
                                           <conversionPattern value=""%date [%thread] %level %logger - %message%newline""/>
                                        </layout>
                                        </appender> 
                 
                                   <!-- Setup the root category, add the appenders and set the default level --> 
                                   <root> 
                                   <level value=""ALL""/> 
                                   <appender-ref ref=""RollingLogFileAppender"" /> 
                                   </root> 
                                   </log4net> 
                                    </configuration>";
            return Config;
        }

//        Extract Insert/Updates/ Delete from Ed06
//Get SApEQUIpID from Ed07
//Connect to DB and Filter Data based on Rules.
//Send Data to TLDB

        

    }
}
