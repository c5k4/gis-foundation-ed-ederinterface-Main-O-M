#region Sorted using

using ESRI.ArcGIS.esriSystem;
using Oracle.DataAccess.Client;
using System;
using System.Configuration;
using System.Data;
using System.IO;
using System.Windows.Forms;
using System.Text;
using System.Diagnostics;
using System.Threading;
using PGE_DBPasswordManagement;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.DataSourcesGDB;


#endregion

namespace PGE.Interfaces.LoadingDataInOracle
{
    public class LoadData
    {
        #region Private variables

        private static StreamWriter fileWriter = System.IO.File.AppendText(ConfigurationManager.AppSettings["Exception_FileName"]);
        private static Log4NetLogger _log = new Log4NetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, ConfigurationManager.AppSettings["LogConfigName"]);

        private static System.Windows.Forms.Timer myTimer = new System.Windows.Forms.Timer();
        private static LicenseInitializer m_AOLicenseInitializer = new LicenseInitializer();
        private static GDBMPostSession _gdbmPostSession = new GDBMPostSession();
        private static bool exitFlag = false;
        private static bool  successStatus;
        private static int dt = 0;
        private static int resultInt = 1;
        public static string  sSessionName=string.Empty;

        public  static string comment = default;
        private static  string startTime;
        private static StringBuilder remark = default;
        private static StringBuilder Argumnet = default;
        private static bool NoDataPresent = false;


        #endregion

        #region Private Methods

        /// <summary>
        ///  Run when the timer is raised.
        /// </summary>
        /// <param name="myObject"></param>
        /// <param name="myEventArgs"></param>
        private static void TimerEventProcessor(System.Object myObject, EventArgs myEventArgs)
        {
            dt += myTimer.Interval;             
           
            try
            {
                myTimer.Stop();
                if (!InitializeLicense())
                {
                    Console.WriteLine(m_AOLicenseInitializer.LicenseMessage());
                    Console.WriteLine("License not found");
                    _log.Info("License not found");
                    comment = "License not found";
                    ExecutionSummary(startTime, comment, Argument.Error);
                }
                else
                {
                    _log.Info("License Occupied");
                    Console.WriteLine("License Occupied");
                    _log.Info("Process Started");
                    Console.WriteLine("Process Started"); //Console.WriteLine("Process Started"); 

                    _log.Info("Checking the trigger file and then load the database.");

                    #region  code is commented for edgisrearch-374 (EDGIS Rearch project) by v1t8
                    // Check the trigger file and then load the database.

                    // code is commented for edgisrearch-374 (EDGIS Rearch project) by v1t8
                    //In new system iinterface will read data from staging table not from files .Reading files code is no more required 
                    //  string triggerFilePath = ConfigurationManager.AppSettings["TriggerFile_Path"];
                    //   FileInfo InBound_File_Dir_Path = new FileInfo(triggerFilePath);
                    //if (InBound_File_Dir_Path.Exists)
                    //{
                    //   _log.Info("Trigger file found.");
                    // Below check added for EDGIS ReArch improvement 
                    #endregion
                    IWorkspace _wSpace =null;
                    string sSessionName = string.Empty;
                    try
                    {
                        SdeWorkspaceFactory sdeWSpaceFactory = new SdeWorkspaceFactoryClass();

                        _wSpace = sdeWSpaceFactory.OpenFromFile(ReadEncryption.GetSDEPath(ConfigurationManager.AppSettings["EDER_ConnectionStr"].ToString()),0);
                        if (CheckVersionExistFromSessionName(_wSpace, out sSessionName) == true)
                        {
                            GDBMPostSession.SendMailForPriviousDaySession(sSessionName);
                            _log.Warn("Send mail as previous ED07 Session Already Exist.. ");
                        }
                    }
                    catch { }   
                    if (!CHECKSAPDATA())
                    {
                        _log.Info("No record present in staging table.");
                        exitFlag = true;
                        NoDataPresent = true;
                        return;   
                    }


                    bool LoadGIS = LoadGISData();
                    if (LoadGIS)
                    {

                        _log.Info("Process Start for GDBM session");
                        bool success = _gdbmPostSession.ProcessSession();

                        if (success)
                        {
                            if (GDBMPostSession._versionCreated && GDBMPostSession._sessionCreated)
                            {
                                _log.Info(" Process start to insert data into table");
                                successStatus = ProcessSAPData();
                                if (successStatus)
                                {
                                    _log.Info("Submit session to GDBM");
                                    if (_gdbmPostSession.SubmitSessionToGDBM())
                                    {
                                        
                                        comment = "ED07 process completed successfully";
                                        _log.Info("Process Completed");
                                    }
                                    else
                                    {

                                        throw new Exception("Error occurred, session not send to GDBM Post Queue");
                                    }
                                }
                                else
                                {
                                    throw new Exception("Error occurred, Data not processed successfully");
                                }

                            }

                            // Below code commented for edgisrearch-374 interface improvement for EDGIS ReArch project - v1t8
                            // WriteTable();
                            myTimer.Enabled = true;

                            // Stops the timer.
                            exitFlag = true;

                            Application.ExitThread();
                        }
                        else
                        {
                            _log.Info("Session or Version not created successfully");
                            if ((dt / 60000) >= Convert.ToInt32(ConfigurationManager.AppSettings["maxAppRunningTime"]))
                            {
                                exitFlag = true;
                                Application.ExitThread();
                            }
                            else
                            {
                                myTimer.Enabled = true; // Now wait for given time and come again for loading the data.
                            }




                            comment = "ED07 process not completed successfully";
                        }


                    }
                    else
                    {
                        throw new Exception("Load GIS Data not Executed successfully");

                    }

                }
                
            }
            catch (Exception ex)
            {
                _log.Error("Error while executing main" + ex.Message);
                _log.Info(ex.Message + "   " + ex.StackTrace);
                fileWriter.WriteLine(" Error while executing main. At " + DateTime.Now + ". " + ex);
                throw ex;
            }
            finally
            {
                fileWriter.Close();
            }
        }

        private static bool CheckVersionExistFromSessionName(IWorkspace _wSpace, out string sSessionName)
        {
            sSessionName = string.Empty;
            bool SessionExist = false;

            IFeatureWorkspace fWSpace = (IFeatureWorkspace)_wSpace;
            IVersionedWorkspace vWSpace = (IVersionedWorkspace)fWSpace;
            try
            {
                int iSessionID = GetExistingSessionId(out sSessionName);
                if (iSessionID != -1)
                {
                    string sPrifix = ConfigurationManager.AppSettings["SessionName"].ToString() ;
                    IEnumVersionInfo vinfo = (IEnumVersionInfo)vWSpace.Versions;
                    IVersionInfo pversioninfo = vinfo.Next();
                    while (pversioninfo != null)
                    {
                        string vname = pversioninfo.VersionName.ToString();

                        if (vname.Contains(sPrifix))
                        {
                            SessionExist = true;
                            _log.Info("ED07 Version Exist");
                            break;
                        }
                        else 
                        {
                        }
                        pversioninfo = vinfo.Next();
                    }
                }

            }
            catch (Exception EX)
            {

                _log.Error("Unhandled exception encountered while checking the existing version", EX);

            }
            return SessionExist;

        }

        private static int GetExistingSessionId(out string sSessionName)
        {
            sSessionName = string.Empty;
            int sessionId = -1;
            try
            {
                string sessionName = ConfigurationManager.AppSettings["SessionName"];
                string sql = "select  session_id, session_name from process.mm_session where session_id = (SELECT max(SESSION_ID) from process.mm_session where hidden=0 and " +
                            " upper(Session_Name) like upper('%" + sessionName + "%'))";
                DataTable DT = new DataTable();
                #region FInd SESSION Name
                if (string.IsNullOrEmpty(GDBMPostSession._oracleConnectionString))
                {
                    GDBMPostSession._oracleConnectionString = ReadEncryption.GetConnectionStr(ConfigurationManager.AppSettings["EDER_ConnectionStr"]);
                }
                using (OracleConnection connection = new OracleConnection(GDBMPostSession._oracleConnectionString))
                {
                    _log.Info("Using Connection : " + ConfigurationManager.AppSettings["EDER_ConnectionStr"]);
                    connection.Open();
                    Oracle.DataAccess.Client.OracleCommand oracleCommand = new Oracle.DataAccess.Client.OracleCommand();
                    oracleCommand.Connection = connection;

                    //Code added to read sp name from config file for EDGIS ReArch
                    // oracleCommand.CommandText = "EDGIS.LOAD_GIS_GUID";
                    oracleCommand.CommandText = sql;
                    oracleCommand.CommandType = CommandType.Text;

                    OracleDataAdapter daOracle = new OracleDataAdapter();
                    daOracle.SelectCommand = oracleCommand;
                    daOracle.Fill(DT);

                    if (DT != null)
                    {
                        if (DT.Rows.Count > 0)
                        {
                            sessionId = Convert.ToInt32(DT.Rows[0].ItemArray[0].ToString());
                            sSessionName = DT.Rows[0].ItemArray[1].ToString();
                        }
                    }

                    #endregion
                }
            }

            catch (Exception EX)
            {

                _log.Error("Unhandled exception encountered while getting the sessionid", EX);

            }

            return sessionId;
        }


        private static bool CHECKSAPDATA()
        {
            DataTable DT = new DataTable();
            bool checkdata = false;
            try
            {
                string sQuery = ConfigurationManager.AppSettings["CHECK_SAP_DATA"].ToString();
                if (string.IsNullOrEmpty(GDBMPostSession._oracleConnectionString))
                {
                    GDBMPostSession._oracleConnectionString = ReadEncryption.GetConnectionStr(ConfigurationManager.AppSettings["EDER_ConnectionStr"]);
                }
                using (OracleConnection connection = new OracleConnection(GDBMPostSession._oracleConnectionString))
                {
                    _log.Info("Using Connection : " + ConfigurationManager.AppSettings["EDER_ConnectionStr"]);
                    connection.Open();
                    Oracle.DataAccess.Client.OracleCommand oracleCommand = new Oracle.DataAccess.Client.OracleCommand();
                    oracleCommand.Connection = connection;

                    //Code added to read sp name from config file for EDGIS ReArch
                    // oracleCommand.CommandText = "EDGIS.LOAD_GIS_GUID";
                    oracleCommand.CommandText =sQuery;
                    oracleCommand.CommandType = CommandType.Text;
                    
                    OracleDataAdapter  daOracle = new OracleDataAdapter();
                    daOracle.SelectCommand = oracleCommand;
                    daOracle.Fill(DT);

                    if (DT != null)
                    {
                        if (DT.Rows.Count > 0)
                        {
                            if (Convert.ToInt32(DT.Rows[0].ItemArray[0].ToString()) > 0)
                            {
                                checkdata = true;
                            }
                        }
                    }       
                        

                    //checkdata = false;
                    resultInt = 0;
                    return checkdata;

                }
            }
            catch (Exception ex)
            {
                _log.Error("Exception occures while processing the SAP data" + ex.Message);
                _log.Info(ex.Message + "   " + ex.StackTrace);
                Console.WriteLine("Exception occures while writing the tablee", ex.StackTrace);
                //  fileWriter.WriteLine(" Error Occurred at " + DateTime.Now + " " + ex);
                
            }
            return checkdata;
        }

        #region This methods are not in use after EDGIS Rearch project as file system concept is descarded - April 2021 :v1t8
        /// <summary>
        /// Copy files from Inbound file directory to Archive File Directory.
        /// </summary>
        private static void CopyFile()
        {
            try
            {
                DirectoryInfo sourceinfo = new DirectoryInfo(ConfigurationManager.AppSettings["InBound_Dir_Path"]);
                DirectoryInfo target = new DirectoryInfo(ConfigurationManager.AppSettings["Archive_File_Location"]);
                if (!Directory.Exists(target.FullName))
                {
                    Directory.CreateDirectory(target.FullName);
                }
                //string inBoundPath = ;
                string outBoundPath = ConfigurationManager.AppSettings["InBound_Dir_Path"];
                FileInfo[] info = sourceinfo.GetFiles();
                for (int i = 0; i < info.Length; i++)
                {
                    if (info[i].Extension.ToUpper() == ".TXT" || info[i].Extension.ToUpper() == ".CSV")
                    {
                        File.Copy(sourceinfo + "\\" + info[i].Name, target + "\\" + string.Format("{0:yyyy-MM-dd_hh-mm-ss-tt}", DateTime.Now) + info[i].Name);
                        TryToDelete(sourceinfo + "\\" + info[i].Name);
                    }
                }
                Console.WriteLine("All csv and trigger file archived successfully.");
            }
            catch (Exception ex)
            {
                _log.Error("Error while executing main." + ex.Message);
                _log.Info(ex.Message + "   " + ex.StackTrace);
                Console.WriteLine("Error while executing main.", ex.StackTrace);
                fileWriter.WriteLine(" Error while executing main. At " + DateTime.Now + ". " + ex);
            }
        }

        /// <summary>
        /// Try to delete file. OtherWise write an error log using exception handler.
        /// </summary>
        static bool TryToDelete(string fileName)
        {
            try
            {
                // Try to delete the file.
                File.Delete(fileName);
                return true;
            }
            catch (IOException ex)
            {
                _log.Error("Error while executing main." + ex.Message);
                _log.Info(ex.Message + "   " + ex.StackTrace);
                // We could not delete the file.
                fileWriter.WriteLine(" Error while executing main. At " + DateTime.Now + ". " + ex);
                return false;
            }
        }

        /// <summary>
        /// Write the table in database.
        /// </summary>
        /// <param name="table"></param>
        /// <returns></returns>
        private static bool WriteTable()
        {
            try
            {
                DataTable dt = DefineWorkOrderStructureTable();
                int objectIdValue = 0;
                bool objectIdRead = false;
                if (dt != null)
                {
                    using (OracleConnection connection = new OracleConnection(GDBMPostSession._oracleConnectionString))
                    {
                        #region Truncate table is not required for EDGIS Rearch project. It should be done by backup schema job-v1t8
                        _log.Info("EDGIS.TRUNCATE_SAP_TABLES Success!!");
                        Console.WriteLine("EDGIS.TRUNCATE_SAP_TABLES Success!!");
                        Oracle.DataAccess.Client.OracleCommand oracleCommand = new Oracle.DataAccess.Client.OracleCommand();
                        oracleCommand.Connection = connection;
                        // oracleCommand.CommandText = "EDGIS.TRUNCATE_SAP_TABLES";
                        //Code added to read sp name from config file for EDGIS ReArch
                        oracleCommand.CommandText = GDBMPostSession._spTruncateTable;
                        oracleCommand.CommandType = CommandType.StoredProcedure;
                        connection.Open();
                        oracleCommand.ExecuteNonQuery();

                        #endregion
                        _log.Info("Using Connection : " + ConfigurationManager.AppSettings["EDER_ConnectionStr"]);
                        Oracle.DataAccess.Client.OracleCommand cmdSQL = new Oracle.DataAccess.Client.OracleCommand("select objectid from edgis.sap_to_gis where rowid in(select max(rowid) from edgis.sap_to_gis) ", connection);
                        Oracle.DataAccess.Client.OracleDataReader dataReader = cmdSQL.ExecuteReader();
                        if (dataReader.FieldCount > 0)
                        {
                            while (dataReader.Read())
                            {
                                objectIdValue = Convert.ToInt32(dataReader[0]);
                                objectIdRead = true;
                            }
                        }
                        connection.Close();
                    }
                    dt.BeginLoadData();
                    string path = ConfigurationManager.AppSettings["InBound_Dir_Path"];
                    string[] files = Directory.GetFiles(path, @"*.csv", SearchOption.TopDirectoryOnly);
                    if (objectIdRead)
                    {
                        _log.Error(" Error Occurred at while reading the objectID value. Operation abort");
                        fileWriter.WriteLine(" Error Occurred at while reading the objectID value. Operation abort");
                        return false;
                    }
                    foreach (var item in files)
                    {
                        var reader = new StreamReader(item);
                        while (!reader.EndOfStream)
                        {
                            string line = reader.ReadLine();
                            string[] values = line.Split(',');
                            if (!string.IsNullOrEmpty(line) && values.Length == 4)
                            {
                                objectIdValue++;
                                DataRow newRow = dt.NewRow();
                                newRow["OBJECTID"] = objectIdValue;
                                newRow["SAP_EQUIPMENT_ID"] = values[0];
                                newRow["EQUIPMENT_NAME"] = values[1];
                                newRow["SAP_EQUIPMENT_TYPE"] = values[2];
                                newRow["GUID"] = values[3];
                                dt.Rows.Add(newRow);
                            }
                            else
                            {
                                fileWriter.WriteLine(" Error Occurred at--- " + line);
                            }
                        }

                        reader.Close();
                    }
                    dt.EndLoadData();
                    if ((dt != null) && (dt.Rows.Count > 0))
                    {
                        using (Oracle.DataAccess.Client.OracleConnection connection = new Oracle.DataAccess.Client.OracleConnection(ConfigurationManager.AppSettings["OracleConnectionString"]))
                        {
                            _log.Info("Oracle connection open");
                            connection.Open();
                            using (OracleBulkCopy bulkCopy = new OracleBulkCopy(connection))
                            {
                                bulkCopy.BulkCopyOptions = OracleBulkCopyOptions.UseInternalTransaction;
                                bulkCopy.BulkCopyTimeout = 60000;
                                //bulkCopy.DestinationTableName = "edgis.sap_to_gis";
                                //Code added to read table name from config file for EDGIS ReArch
                                bulkCopy.DestinationTableName = GDBMPostSession._ed07StagingTable;
                                bulkCopy.WriteToServer(dt);
                                _log.Info("EDGIS.sap_to_gis Data Inserted Success!!");
                                Console.WriteLine("EDGIS.sap_to_gis Data Inserted Success!!");
                            }
                            _log.Info("Oracle connection closed");
                            connection.Close();
                        }
                    }

                }
                _log.Info("Write Operation completed");
                Console.WriteLine("Write Operation completed");
                using (Oracle.DataAccess.Client.OracleConnection connection = new Oracle.DataAccess.Client.OracleConnection(ConfigurationManager.AppSettings["OracleConnectionString"]))
                {
                    connection.Open();
                    Console.WriteLine("Calling EDGIS.LOAD_GIS_GUID");
                    Oracle.DataAccess.Client.OracleCommand oracleCommand = new Oracle.DataAccess.Client.OracleCommand();
                    oracleCommand.Connection = connection;
                    //Code added to read sp name from config file for EDGIS ReArch
                    // oracleCommand.CommandText = "EDGIS.LOAD_GIS_GUID";
                    oracleCommand.CommandText = GDBMPostSession._spLoadData;
                    oracleCommand.CommandType = CommandType.StoredProcedure;
                    oracleCommand.ExecuteNonQuery();
                    _log.Info("EDGIS.LOAD_GIS_GUID Completed!!");
                    Console.WriteLine("EDGIS.LOAD_GIS_GUID Completed!!");

                    _log.Info("Calling EDGIS.INSERT_SAP_INTEGRATED_RESULT");
                    Console.WriteLine("Calling EDGIS.INSERT_SAP_INTEGRATED_RESULT");
                    //Code added to read sp name from config file for EDGIS ReArch
                    //  oracleCommand.CommandText = "EDGIS.INSERT_SAP_INTEGRATED_RESULT"; SPIntegratedResult
                    oracleCommand.CommandText = GDBMPostSession._spSAPTOGIS;
                    oracleCommand.CommandType = CommandType.StoredProcedure;
                    //Below line added to pass version name as a parameter in stored procedure-ReArch Improvement 
                    oracleCommand.Parameters.Add("", OracleDbType.Varchar2).Value = GDBMPostSession._versionName;

                    oracleCommand.ExecuteNonQuery();
                    connection.Close();
                    _log.Info("EDGIS.INSERT_SAP_INTEGRATED_RESULT Completed!!");
                    Console.WriteLine("EDGIS.INSERT_SAP_INTEGRATED_RESULT Completed!!");
                    resultInt = 0;
                    CopyFile();
                }
                return true;
            }
            catch (Exception ex)
            {
                _log.Error("Exception occures while writing the table" + ex.Message);
                _log.Info(ex.Message + "   " + ex.StackTrace);
                Console.WriteLine("Exception occures while writing the tablee", ex.StackTrace);
                fileWriter.WriteLine(" Error Occurred at " + DateTime.Now + " " + ex);
                return false;
            }
        }
        /// <summary>
        /// Define Work Order Structure Table for DataTable.
        /// </summary>
        /// <returns></returns>
        public static System.Data.DataTable DefineWorkOrderStructureTable()
        {
            System.Data.DataTable sap_to_gisStructureTable = new System.Data.DataTable("sap_to_gis");
            try
            {

                System.Data.DataColumn globalIDCol = new DataColumn("OBJECTID", System.Type.GetType("System.Int32"));
                globalIDCol.AllowDBNull = false;
                sap_to_gisStructureTable.Columns.Add(globalIDCol);

                System.Data.DataColumn SAP_EQUIPMENT_ID = new DataColumn("SAP_EQUIPMENT_ID", System.Type.GetType("System.String"));
                SAP_EQUIPMENT_ID.AllowDBNull = true;
                sap_to_gisStructureTable.Columns.Add(SAP_EQUIPMENT_ID);

                System.Data.DataColumn EQUIPMENT_NAME = new DataColumn("EQUIPMENT_NAME", System.Type.GetType("System.String"));
                EQUIPMENT_NAME.AllowDBNull = true;
                sap_to_gisStructureTable.Columns.Add(EQUIPMENT_NAME);

                System.Data.DataColumn SAP_EQUIPMENT_TYPE = new DataColumn("SAP_EQUIPMENT_TYPE", System.Type.GetType("System.String"));
                SAP_EQUIPMENT_TYPE.AllowDBNull = true;
                sap_to_gisStructureTable.Columns.Add(SAP_EQUIPMENT_TYPE);

                System.Data.DataColumn GUID = new DataColumn("GUID", System.Type.GetType("System.String"));
                GUID.AllowDBNull = true;
                sap_to_gisStructureTable.Columns.Add(GUID);
            }
            catch (Exception ex)
            {
                fileWriter.WriteLine(" Error Occurred at " + DateTime.Now + " " + ex);
            }
            return sap_to_gisStructureTable;
        }

        #endregion

        /// <summary>
        /// Keep track the timer event handler and call the load data code.
        /// </summary>
        /// <param name="args"></param>
        public static int Main(string[] args)
        {
            try
            {
                startTime = DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss");
                // Adds the event and the event handler for the method that will process the timer event to the timer.
                myTimer.Tick += new EventHandler(TimerEventProcessor);
                // Sets the timer interval.               
                myTimer.Interval = Convert.ToInt32(ConfigurationManager.AppSettings["CheckInterval"].ToString());
                myTimer.Start();

                // Runs the timer, and raises the event. 
                while (exitFlag == false)
                {
                    // Processes all the events in the queue.
                    Application.DoEvents();
                }
                if (NoDataPresent == false)
                {
                    ExecutionSummary(startTime, comment, Argument.Done);
                }
                return resultInt;
            }
            catch (Exception ex)
            {
                 GDBMPostSession.SendMailtoOperationForError();
                _log.Error("Error while executing main." + ex.Message);
                _log.Info(ex.Message + "   " + ex.StackTrace);
                fileWriter.WriteLine(" Error while executing main. At " + DateTime.Now + ". " + ex);
                ExecutionSummary(startTime, "ED07 Exception :" + ex.Message, Argument.Error);
                return resultInt;
            }
            finally
            {
                fileWriter.Close();
            }
        }

        
        /// <summary>
        /// This Method is used to process SAP data and to update SAP equipment id,lastuser and date modified in respective feature classes.
        /// </summary>
        /// <returns>bool</returns>
        private static bool ProcessSAPData()
        {
            bool success = false;

            try
            {
                using (OracleConnection connection = new OracleConnection(GDBMPostSession._oracleConnectionString))
                {
                    _log.Info("Using Connection : " + ConfigurationManager.AppSettings["EDER_ConnectionStr"]);
                    connection.Open();
                    Oracle.DataAccess.Client.OracleCommand oracleCommand = new Oracle.DataAccess.Client.OracleCommand();
                    oracleCommand.Connection = connection;

                   
                    _log.Info("Calling EDGIS.INSERT_SAP_INTEGRATED_RESULT");
                    Console.WriteLine("Calling EDGIS.INSERT_SAP_INTEGRATED_RESULT");

                    //Code added to read sp name from config file for EDGIS ReArch
                    //  oracleCommand.CommandText = "EDGIS.INSERT_SAP_INTEGRATED_RESULT"; SPIntegratedResult
                    oracleCommand.CommandText = GDBMPostSession._spSAPTOGIS;
                    oracleCommand.CommandType = CommandType.StoredProcedure;

                    //Below line added to pass version name as a parameter in stored procedure-ReArch Improvement 
                    oracleCommand.Parameters.Add("", OracleDbType.Varchar2).Value = GDBMPostSession._versionName;

                    oracleCommand.ExecuteNonQuery();
                    connection.Close();
                    _log.Info("EDGIS.INSERT_SAP_INTEGRATED_RESULT Completed!!");
                    Console.WriteLine("EDGIS.INSERT_SAP_INTEGRATED_RESULT Completed!!");
                    success = true;
                    resultInt = 0;
                    return success;

                }
            }
            catch (Exception ex)
            {
                _log.Error("Exception occures while processing the SAP data" + ex.Message);
                _log.Info(ex.Message + "   " + ex.StackTrace);
                Console.WriteLine("Exception occures while writing the tablee", ex.StackTrace);
              //  fileWriter.WriteLine(" Error Occurred at " + DateTime.Now + " " + ex);
                throw ex;
            }
           
        }
        private static bool LoadGISData()
        {
            bool success = false;

            try
            {
                if (string.IsNullOrEmpty(GDBMPostSession._oracleConnectionString))
                {
                    GDBMPostSession._oracleConnectionString = ReadEncryption.GetConnectionStr(ConfigurationManager.AppSettings["EDER_ConnectionStr"]);
                }
                GDBMPostSession._spLoadData = ConfigurationManager.AppSettings["SPLoadGISData"];

                using (OracleConnection connection = new OracleConnection(GDBMPostSession._oracleConnectionString))
                {
                    _log.Info("Using Connection : " + ConfigurationManager.AppSettings["EDER_ConnectionStr"]);
                    connection.Open();
                    Console.WriteLine("Calling EDGIS.LOAD_GIS_GUID");
                    Oracle.DataAccess.Client.OracleCommand oracleCommand = new Oracle.DataAccess.Client.OracleCommand();
                    oracleCommand.Connection = connection;

                    //Code added to read sp name from config file for EDGIS ReArch
                    // oracleCommand.CommandText = "EDGIS.LOAD_GIS_GUID";
                    oracleCommand.CommandText = GDBMPostSession._spLoadData;
                    oracleCommand.CommandType = CommandType.StoredProcedure;

                    oracleCommand.ExecuteNonQuery();
                    _log.Info("EDGIS.LOAD_GIS_GUID Completed!!");
                    Console.WriteLine("EDGIS.LOAD_GIS_GUID Completed!!");

                    
                    success = true;
                    resultInt = 0;
                    return success;

                }
            }
            catch (Exception ex)
            {
                _log.Error("Exception occures while processing the SAP data" + ex.Message);
                _log.Info(ex.Message + "   " + ex.StackTrace);
                Console.WriteLine("Exception occures while writing the tablee", ex.StackTrace);
                //  fileWriter.WriteLine(" Error Occurred at " + DateTime.Now + " " + ex);
                throw ex;
            }

        }
        /// <summary>
        /// This methos is to use for log the interface success or fail in interface summary table for EDGIS Rearch Project-V1t8
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="comment"></param>
        /// <param name="status"></param>
        private static void ExecutionSummary(string startTime,string comment,string status)
        {
            try
            {
                
               Argumnet = new StringBuilder();
                 remark = new StringBuilder();
                //Setting arguments
                Argumnet.Append(Argument.Interface);
                Argumnet.Append(Argument.Type);
                Argumnet.Append(Argument.Integration);
                Argumnet.Append(startTime  + ";");
                remark.Append(comment) ;
                Argumnet.Append(DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss") + ";");
                Argumnet.Append(status);
                Argumnet.Append(remark);

                //To execution for interface execution summary exe.This exe must need some input argument to run successffully. 
                ProcessStartInfo processStartInfo = new ProcessStartInfo(GDBMPostSession.intExecutionSummary, "\"" + Convert.ToString(Argumnet) + "\"");
                processStartInfo.UseShellExecute = false;
                processStartInfo.RedirectStandardOutput = true;
                processStartInfo.CreateNoWindow = true;

                Process proc = new Process();
                proc.StartInfo = processStartInfo;
                proc.EnableRaisingEvents = true;
                proc.Start();
                proc.BeginOutputReadLine();
                while (!proc.HasExited)
                {
                    Thread.Sleep(1000);
                }
            }
            catch (Exception ex)
            {
                _log.Error("Exception occures while executing the Interface Summary exe" + ex.Message);
                _log.Info(ex.Message + "   " + ex.StackTrace);
                Console.WriteLine("Exception occures while executing the Interface Summary exe", ex.StackTrace);
              //  fileWriter.WriteLine(" Error Occurred while executing the Interface Summary exe at " + DateTime.Now + " " + ex);
            }
        }

        /// <summary>
        /// This Method is used to Initialize ArcFM and ArcGIS license- EDGIS Rearch project by v1t8
        /// </summary>
        /// <returns>bool</returns>
        private static bool InitializeLicense()
        {
            try
            {
                if (!m_AOLicenseInitializer.InitializeApplication(new esriLicenseProductCode[] { esriLicenseProductCode.esriLicenseProductCodeAdvanced, esriLicenseProductCode.esriLicenseProductCodeBasic, esriLicenseProductCode.esriLicenseProductCodeAdvanced },
                 new esriLicenseExtensionCode[] { }))
                {
                    Console.WriteLine(m_AOLicenseInitializer.LicenseMessage());
                    _log.Info("This application could not initialize with the correct ArcGIS license and will shutdown.");
                    Console.WriteLine("This application could not initialize with the correct ArcGIS license and will shutdown.");
                    m_AOLicenseInitializer.ShutdownApplication();
                    fileWriter.WriteLine(" This application could not initialize with the correct ArcGIS license and will shutdown. " + DateTime.Now);
                    return false;
                }

                ////Check out Telvent license
                //if (m_AOLicenseInitializer.CheckOutArcFMLicense (mmLicensedProductCode.mmLPArcFM) != mmLicenseStatus.mmLicenseCheckedOut)
                if (m_AOLicenseInitializer.CheckOutArcFMLicense(Miner.Interop.mmLicensedProductCode.mmLPArcFM) != Miner.Interop.mmLicenseStatus.mmLicenseCheckedOut)
                {
                    _log.Info("This application could not initialize with the correct ArcFM license and will shutdown..");
                    Console.WriteLine("This application could not initialize with the correct ArcFM license and will shutdown.");
                    fileWriter.WriteLine(" This application could not initialize with the correct ArcFM license and will shutdown. " + DateTime.Now);
                    return false;
                }
                _log.Info("License initialized successfully.");
                Console.WriteLine("License initialized successfully.");
                return true;
            }
            catch (Exception ex)
            {
                _log.Error("Exception occurred while intializing the ESRI/ArcFM license." + ex.Message);
                _log.Info(ex.Message + "   " + ex.StackTrace);
                Console.WriteLine("Exception occurred while intializing the ESRI/ArcFM license." + ex.Message.ToString());
                comment = "Exception occurred while intializing the ESRI/ArcFM license." + ex.Message;
             //   fileWriter.WriteLine("Exception occurred while intializing the ESRI/ArcFM license " + DateTime.Now);
                return false;
             
            }
        }
        #endregion
    }
}

