using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using ESRI.ArcGIS;
using ESRI.ArcGIS.DataSourcesGDB;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using Oracle.DataAccess.Client;
using System.Text;
using System.Diagnostics;
using System.Collections.Generic;
using PGE_DBPasswordManagement;


namespace PGE.BatchApplication.VMS_GIS_STG
{
    class Program
    {
        private static LicenseInitializer m_AOLicenseInitializer = new LicenseInitializer();
        private static string sTicks = DateTime.Now.Ticks.ToString(),
            sPath = (String.IsNullOrEmpty(ConfigurationManager.AppSettings["LOGPATH"]) ? System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) : ConfigurationManager.AppSettings["LOGPATH"]) + "\\Logfile_" + sTicks + ".log";
        private static StreamWriter pSWriter = default(StreamWriter);
        private static string sSessionName = string.Empty, sPathData = string.Empty;
        //private static IMMAppInitialize arcFMAppInitialize = new MMAppInitializeClass();
        private static int iArg = -1, iSaveCount = Convert.ToInt32(ConfigurationManager.AppSettings["PERSAVECOUNT"]);
        private static string LBMAINTConnStr = string.Empty;
        private static IDictionary pDic_Field_STG = new Hashtable(),
            pDic_Field_VMS = new Hashtable();

        static void Main(string[] args)
        {
            IVersion pVersion = default(IVersion);
            IFeatureClass pFClass_VMAlerts = default(IFeatureClass);
            IVersionedWorkspace pVWorkspace = default(IVersionedWorkspace);
            try
            { 
                (pSWriter = File.CreateText(sPath = sPath.Replace(".log", (iArg == -1 ? ".log" : "_" + iArg.ToString() + ".log")))).Close();
                WriteLine("VMS Alerts Process started" + DateTime.Now.ToString());

                LBMAINTConnStr = ReadEncryption.GetConnectionStr(ConfigurationManager.AppSettings["LBMAINT_ConnectionStr"].ToUpper());
                SqlConnection sqlConn_Source = new SqlConnection(ConfigurationManager.AppSettings["SOURCE_CONN_KEY"] + ReadEncryption.GetPassword(ConfigurationManager.AppSettings["SOURCE_CONN"]));



                IntitializeLicense();
                pVWorkspace = (IVersionedWorkspace)ArcSdeWorkspaceFromFile(ReadEncryption.GetSDEPath(ConfigurationManager.AppSettings["LBMAINT_SDEConnection"].ToUpper()));
                if (pVWorkspace == null) throw new Exception("Error in Getting SDE Workspace,terminate the  process.");

                #region FindVersion
                try
                {
                    pVersion = pVWorkspace.FindVersion(ConfigurationManager.AppSettings["TARGET_VERSION"]);
                    string versionname = pVersion.VersionName.ToString();
                    if (ConfigurationManager.AppSettings["DELETEEXISTINGVERSION"] == "Y")
                    {
                        pVersion.Delete();
                        pVersion = null;
                        WriteLine("Existing version deleted successfully");
                    }
                }
                catch (Exception ex){ }
                if (pVersion ==null)
                {
                    pVersion = pVWorkspace.DefaultVersion.CreateVersion(ConfigurationManager.AppSettings["TARGET_VERSION"]);
                    WriteLine("New version created" + pVersion.VersionName);
                }
                #endregion

                //Below code added for EDGIS rearch Project to process VMS data via staging table and stored procedure
                if (ProcessVMSData(pVersion.VersionName))
                {
                    #region validate the feature count that feature class is populated correctly
                    pFClass_VMAlerts = ((IFeatureWorkspace)pVersion).OpenFeatureClass("LBGIS.VMSALERTS");
                    if (pFClass_VMAlerts != null)
                    {
                        int count = pFClass_VMAlerts.FeatureCount(null);
                        if (count == 0)
                        {
                            WriteLine("LBGIS.VMSALERTS featureclass is having null feature count");
                            string strMessage = ConfigurationManager.AppSettings["MAIL_BODY_FOR_VMSLAYERFAIL"];
                            SendMailtoOperationForFeatureCountError(strMessage);
                            throw new Exception("LBGIS.VMSALERTS featureclass is having null feature count");
                        }
                        else
                        {
                            WriteLine("LBGIS.VMSALERTS featureclass --feature count is --" + count.ToString());
                        }
                    }
                    else
                    {
                        WriteLine("LBGIS.VMSALERTS featureclass not found in LBGIS maintinence database.");
                        throw new Exception("LBGIS.VMSALERTS featureclass not found in LBGIS maintinence database");

                    }
                    #endregion

                    #region Find the version Difference
                    if (ConfigurationManager.AppSettings["RUNVD"].ToUpper() == "Y")
                    {
                        int iDelete, iUpdate, iinsert;
                        FindVersionDifferences(pVersion, ((IVersionedWorkspace)pVersion).DefaultVersion as IVersion, "LBGIS.VMSALERTS", out iDelete, out iinsert, out iUpdate);
                        WriteLine("Total Insert Count== ." + iinsert.ToString());
                        WriteLine("Total Update Count== ." + iUpdate.ToString());
                        WriteLine("Total Delete Count== ." + iDelete.ToString());
                    }
                    #endregion

                    //Reconcile and post the version;
                    if (ReconcilePostVersion(pVersion))
                    {
                        WriteLine("Successfully Reconcile and POST the version");
                        
                        if (ConfigurationManager.AppSettings["DELETEAFTERPOST"] == "Y")
                        {
                            #region Delete Version
                            WriteLine("Deleting version after post");
                            if (pVersion != null) Marshal.FinalReleaseComObject(pVersion);
                            if (pFClass_VMAlerts != null) Marshal.FinalReleaseComObject(pFClass_VMAlerts);
                            if (pVWorkspace != null) Marshal.FinalReleaseComObject(pVWorkspace);
                            pVersion = null;
                            pFClass_VMAlerts = null;
                            pVWorkspace = null;
                            GC.Collect();
                            
                            pVWorkspace = (IVersionedWorkspace)ArcSdeWorkspaceFromFile(ReadEncryption.GetSDEPath(ConfigurationManager.AppSettings["LBMAINT_SDEConnection"].ToUpper()));
                            try
                            {
                                pVersion = pVWorkspace.FindVersion(ConfigurationManager.AppSettings["TARGET_VERSION"]);
                                pVersion.Delete();
                                WriteLine("Version deleted!!");
                            }
                            catch(Exception ex) 
                            {
                                WriteLine("Version not deleted -- "  + ex.Message.ToString());

                            }
                            #endregion
                        }
                    }
                }
                WriteLine("Process Completes" + DateTime.Now.ToString()); ;
            }
            catch (Exception ex)
            {
                WriteLine("Main: Exception: " + ex.Message);
            }
            finally
            {
                m_AOLicenseInitializer.ShutdownApplication();
                if (pVersion != null) Marshal.FinalReleaseComObject(pVersion);
                if (pVWorkspace != null) Marshal.FinalReleaseComObject(pVWorkspace);
                GC.Collect();
            }
        }

        public static void  FindVersionDifferences(
           IVersion pEditVersion,
           IVersion pDefaultVersion,
           string tableName,
           out int iDelete,out int iInsert,out int iUpdate)
        {
            try
            {
                esriDifferenceType[] pDiffTypes = new esriDifferenceType[6];
                pDiffTypes[0] = esriDifferenceType.esriDifferenceTypeInsert;
                pDiffTypes[1] = esriDifferenceType.esriDifferenceTypeUpdateDelete;
                pDiffTypes[2] = esriDifferenceType.esriDifferenceTypeUpdateNoChange;
                pDiffTypes[3] = esriDifferenceType.esriDifferenceTypeUpdateUpdate;
                pDiffTypes[4] = esriDifferenceType.esriDifferenceTypeDeleteNoChange;
                pDiffTypes[5] = esriDifferenceType.esriDifferenceTypeDeleteUpdate;

                iDelete = 0;  iInsert = 0; iUpdate = 0;
                WriteLine("Entering FindVersionDifferences " + DateTime.Now.ToString());
                WriteLine("    Table: " + tableName);
                WriteLine("    Version: " + pEditVersion.VersionName);

                // Cast the child version to IFeatureWorkspace and open the table.
                Debug.Print("Processing featureclass: " + tableName);
                IFeatureWorkspace pChildFWS = (IFeatureWorkspace)pEditVersion;
                IFeatureWorkspace pDefaultFWS = (IFeatureWorkspace)pDefaultVersion;
                ITable pChildTable = pChildFWS.OpenTable(tableName);
                ITable pDefaulttable= pDefaultFWS.OpenTable(tableName);
                IDataset pChildDS = (IDataset)pChildTable;

                // Cast the common ancestor version to IFeatureWorkspace and open the table.
               
                // Cast to the IVersionedTable interface to create a difference cursor.
                IVersionedTable versionedTable = (IVersionedTable)pChildTable;

                for (int i = 0; i < pDiffTypes.Length; i++)
                {
                   WriteLine("Looking for differences of type: " + pDiffTypes[i].ToString());
                    IDifferenceCursor differenceCursor = versionedTable.Differences
                        (pDefaulttable, pDiffTypes[i], null);

                    // Step through the cursor, storing OId of each row
                    IRow differenceRow = null;
                    int objectID = -1;
                    
                    differenceCursor.Next(out objectID, out differenceRow);
                    List<int> VersionDiffOIDs = new List<int>();

                    while (objectID != -1)
                    {
                        if ((pDiffTypes[i] == esriDifferenceType.esriDifferenceTypeDeleteNoChange) ||
                            (pDiffTypes[i] == esriDifferenceType.esriDifferenceTypeDeleteUpdate))
                        {
                            iDelete = iDelete + 1;
                        }
                        else if ((pDiffTypes[i] == esriDifferenceType.esriDifferenceTypeUpdateNoChange) ||
                            (pDiffTypes[i] == esriDifferenceType.esriDifferenceTypeUpdateUpdate))
                        {
                            iUpdate = iUpdate + 1;
                        }
                        else if ((pDiffTypes[i] == esriDifferenceType.esriDifferenceTypeInsert))
                        {
                            iInsert = iInsert + 1;
                        }
                        else
                        {
                            WriteLine("New Difftype Found == " + pDiffTypes[i].ToString());
                        }

                        differenceCursor.Next(out objectID, out differenceRow);
                    }

                    }
            }
            catch (Exception ex)
            {
                WriteLine ("Entering FindVersionDifferences Error Handler: " + ex.Message);
                throw ex;
            }
            
        }

        /// <summary>
        /// Initialize License
        /// </summary>
        private static void IntitializeLicense()
        {
            try
            {
                m_AOLicenseInitializer.InitializeApplication(new esriLicenseProductCode[] { esriLicenseProductCode.esriLicenseProductCodeBasic, esriLicenseProductCode.esriLicenseProductCodeStandard, esriLicenseProductCode.esriLicenseProductCodeAdvanced },
                new esriLicenseExtensionCode[] { });

                if (RuntimeManager.ActiveRuntime == null)
                    RuntimeManager.BindLicense(ProductCode.Desktop);
                WriteLine("Licence initialized Successfully");
            }
            catch(Exception ex)
            {
                WriteLine("Exception occurred in initializing Licence.");
                throw ex;
            }
        }

       /// <summary>
      /// Reconcile and POST the Version
      /// </summary>
      /// <param name="pVersion"></param>
      /// <returns></returns>
        private static bool ReconcilePostVersion(IVersion pVersion)
        {
            try
            {
                if (ConfigurationManager.AppSettings["RECONCILEPOST"] == "Y")
                {
                    ((IWorkspaceEdit)pVersion).StartEditing(false);
                    ((IWorkspaceEdit)pVersion).StartEditOperation();
                    WriteLine("Reconciling the version");
                    ((IVersionEdit4)pVersion).Reconcile4(((IVersionedWorkspace)pVersion).DefaultVersion.VersionName, false, false, false, false);
                    ((IWorkspaceEdit)pVersion).StopEditOperation();
                    ((IWorkspaceEdit)pVersion).StartEditOperation();
                    WriteLine("Posting version");
                    try
                    {
                        ((IVersionEdit4)pVersion).Post(((IVersionedWorkspace)pVersion).DefaultVersion.VersionName);
                    }
                    catch (Exception ex1)
                    {
                        WriteLine("ReconcilePostVersion_Catch1: Exception: " + ex1.Message);
                        ((IVersionEdit4)pVersion).Reconcile4(((IVersionedWorkspace)pVersion).DefaultVersion.VersionName, false, false, false, false);
                        WriteLine("Reconciled again version");
                        ((IVersionEdit4)pVersion).Post(((IVersionedWorkspace)pVersion).DefaultVersion.VersionName);
                    }
                    WriteLine("Posting Completed");
                    ((IWorkspaceEdit)pVersion).StopEditOperation();
                    ((IWorkspaceEdit)pVersion).StopEditing(true);
                }
                return true;
                
            }
            catch (Exception ex)
            {
                WriteLine("ReconcilePostVersion: Exception: " + ex.Message);
                return false;
            }
            finally
            {
                if (((IWorkspaceEdit)pVersion).IsBeingEdited())
                    ((IWorkspaceEdit)pVersion).StopEditing(true);
            }
        }

        /// <summary>
        /// Get data from Source
        /// </summary>
        private static void  GetInsertUpdate()
        {
            string sLatestDate = string.Empty;
            SqlConnection sqlConn_Source = new SqlConnection(ConfigurationManager.AppSettings["SOURCE_CONN_KEY"] + ReadEncryption.GetPassword(ConfigurationManager.AppSettings["SOURCE_CONN"]));
            OracleConnection oraConn_Target = default(OracleConnection);
            // m4jf edgisrearch 919
            //oraConn_Target = new OracleConnection(ConfigurationManager.AppSettings["TARGET_CONN_KEY"]);
            oraConn_Target = new OracleConnection(LBMAINTConnStr);
            DataTable pDT_InsertUpdate = new DataTable();
            try
            {
                WriteLine("Starting getting updates");
                
                DataTable stagTable = GetStagingDataTable(ConfigurationManager.AppSettings["STG_TABLE"]);
                OracleConnection _oraConnection = new OracleConnection(LBMAINTConnStr);
                SqlCommand sQLcmd = new SqlCommand();
                sQLcmd.CommandType = System.Data.CommandType.Text;
                sQLcmd.Connection = sqlConn_Source;
                sQLcmd.CommandText = "SELECT DIVCODE,CIRCUITNAME,SOURCEDEV,ADDRESS,CITY,COUNTY,COMMENTS,DIRECTIONS,XSTREET,APN,CUSTNAME,CUSTPHONE,CUSTNAME2,CUSTPHONE2,ALERTCODES,ALERTDESCRIPTIONS,LATITUDE,LONGITUDE,REFRESHDATE,ALERTGROUP,RECORDSOURCE,IRECORDKEY,DTRECORDMODIFIED FROM " + ConfigurationManager.AppSettings["SOURCE_TABLE"]
                    + " WHERE LATITUDE IS NOT NULL AND LONGITUDE IS NOT NULL ";
                
                WriteLine("Query to fetch latest records : " + sQLcmd.CommandText);

                sQLcmd.Connection.Open();
                _oraConnection.Open();

                //truncate existing staging data
                string sQuery = "Delete from " + ConfigurationManager.AppSettings["STG_TABLE"].ToString();
                OracleCommand cmdExecuteQuery = null;
                cmdExecuteQuery = new OracleCommand(sQuery, _oraConnection);
                cmdExecuteQuery.CommandTimeout = 0;
                cmdExecuteQuery.ExecuteNonQuery();

                using (var reader = sQLcmd.ExecuteReader())
                using (OracleBulkCopy copy = new OracleBulkCopy(_oraConnection))
                {
                    copy.BulkCopyTimeout = 600;
                    copy.BatchSize = 5000;
                    copy.DestinationTableName = stagTable.TableName;
                    copy.WriteToServer(reader);
                    copy.Close();
                    copy.Dispose();                   
                 //   stagTable.Rows.Clear();
                    WriteLine("Sucessfully inserted rows in staging table");
                }
                _oraConnection.Close();
                sQLcmd.Connection.Close();

            }
            catch (Exception ex)
            {
                WriteLine("MigrateToStaging: Exception: " + ex.Message);
                //   return pDT_InsertUpdate;
                //return string.Empty;
               // return success ;
            }
            finally
            {
                if (oraConn_Target.State != ConnectionState.Closed)
                    oraConn_Target.Close();
                pDT_InsertUpdate.Clear();
                GC.Collect();
                //m4jf
               // iCountFile = File.ReadAllLines(sPathData).Count();
            }
        }

        /// <summary>
        /// Method to get VMS alerts data 
        /// </summary>
        /// <returns>DataTable</returns>
        //Below function singnature is changed for EDGIS rearch project added new signature for the GetInsertUpdate function. internal logics are used with modification mentioned in the comment line 
        // private static string /*DataTable*/ GetInsertUpdate(out string sLatestDate)
        private static void GetInsertUpdate_IncludeDate()
        {
            // below line is commented for EDGIS rearch project 
            // string sPathData = string.Empty;           
            //if (!String.IsNullOrEmpty(ConfigurationManager.AppSettings["DATAFILEPATH"]) || File.Exists(ConfigurationManager.AppSettings["DATAFILEPATH"]))
            //    return sPathData = ConfigurationManager.AppSettings["DATAFILEPATH"];

            string sLatestDate = string.Empty;
            SqlConnection sqlConn_Source = new SqlConnection(ConfigurationManager.AppSettings["SOURCE_CONN_KEY"] + ReadEncryption.GetPassword(ConfigurationManager.AppSettings["SOURCE_CONN"]));
            OracleConnection oraConn_Target = default(OracleConnection);
            // m4jf edgisrearch 919
            //oraConn_Target = new OracleConnection(ConfigurationManager.AppSettings["TARGET_CONN_KEY"]);
            oraConn_Target = new OracleConnection(LBMAINTConnStr);
            DataTable pDT_InsertUpdate = new DataTable();
            try
            {
                WriteLine("Starting getting updates");
                sLatestDate = GetLatestDate();
                if (string.IsNullOrEmpty(sLatestDate))
                {
                    oraConn_Target.Open();
                    OracleDataReader pOReader = (new OracleCommand() { Connection = oraConn_Target, CommandType = System.Data.CommandType.Text,
                        CommandText = "SELECT MAX(DTRECORDMODIFIED) FROM (SELECT MAX(V.DTRECORDMODIFIED) DTRECORDMODIFIED FROM " 
                        + ConfigurationManager.AppSettings["TARGET_FClass"] + " V UNION SELECT NVL(MAX(A.DTRECORDMODIFIED), TO_DATE('01/01/2000','MM/DD/YYYY')) DTRECORDMODIFIED FROM "
                        + ConfigurationManager.AppSettings["TARGET_FClass_ATABLE"] + " A)" }).ExecuteReader(CommandBehavior.SingleResult);
                    pOReader.Read();
                    sLatestDate = Convert.ToString(pOReader[0]);
                }

                 
                DataTable stagTable = GetStagingDataTable(ConfigurationManager.AppSettings["STG_TABLE"]);
                OracleConnection _oraConnection = new OracleConnection(LBMAINTConnStr);
                SqlCommand sQLcmd = new SqlCommand();
                sQLcmd.CommandType = System.Data.CommandType.Text;
                sQLcmd.Connection = sqlConn_Source;
                //cmd.CommandText = "SELECT * FROM " + ConfigurationManager.AppSettings["SOURCE_TABLE"];
                //cmd.CommandText = "SELECT DIVCODE,CIRCUITNAME,SOURCEDEV,ADDRESS,CITY,COUNTY,COMMENTS,DIRECTIONS,XSTREET,APN,CUSTNAME,CUSTPHONE,CUSTNAME2,CUSTPHONE2,ALERTCODES,ALERTDESCRIPTIONS,LATITUDE,LONGITUDE,CONVERT(VARCHAR(26),REFRESHDATE,100) REFRESHDATE,ALERTGROUP,RECORDSOURCE,IRECORDKEY, CONVERT(VARCHAR(26),DTRECORDMODIFIED,100) DTRECORDMODIFIED FROM " + ConfigurationManager.AppSettings["SOURCE_TABLE"] + " WHERE LATITUDE IS NOT NULL AND LONGITUDE IS NOT NULL AND DTRECORDMODIFIED < GETDATE()";
                sQLcmd.CommandText = "SELECT DIVCODE,CIRCUITNAME,SOURCEDEV,ADDRESS,CITY,COUNTY,COMMENTS,DIRECTIONS,XSTREET,APN,CUSTNAME,CUSTPHONE,CUSTNAME2,CUSTPHONE2,ALERTCODES,ALERTDESCRIPTIONS,LATITUDE,LONGITUDE,REFRESHDATE,ALERTGROUP,RECORDSOURCE,IRECORDKEY,DTRECORDMODIFIED FROM " + ConfigurationManager.AppSettings["SOURCE_TABLE"]
                    + " WHERE LATITUDE IS NOT NULL AND LONGITUDE IS NOT NULL AND DTRECORDMODIFIED < GETDATE()"
                    + " AND DTRECORDMODIFIED > CAST('" + sLatestDate + "' as datetime)"
                    ;
                WriteLine("Query to fetch latest records : " + sQLcmd.CommandText);

                sQLcmd.Connection.Open();
                _oraConnection.Open();
                using (var reader = sQLcmd.ExecuteReader())
                using (OracleBulkCopy copy = new OracleBulkCopy(_oraConnection))
                {
                    copy.BulkCopyTimeout = 600;
                    copy.BatchSize = 5000;
                    copy.DestinationTableName = stagTable.TableName;
                    copy.WriteToServer(reader);
                    copy.Close();
                    copy.Dispose();
                    //   stagTable.Rows.Clear();
                    WriteLine("Sucessfully inserted rows in staging table");
                }
                _oraConnection.Close();
                sQLcmd.Connection.Close();
            }
            catch (Exception ex)
            {
                WriteLine("MigrateToStaging: Exception: " + ex.Message);
                //   return pDT_InsertUpdate;
                //return string.Empty;
                // return success ;
            }
            finally
            {
                if (oraConn_Target.State != ConnectionState.Closed)
                    oraConn_Target.Close();
                pDT_InsertUpdate.Clear();
                GC.Collect();
                //m4jf
                // iCountFile = File.ReadAllLines(sPathData).Count();
            }
        }



        private static string GetLatestDate()
        {
            string sDate = string.Empty;
            WriteLine("GetLatestDate");
            try
            {
                if (Directory.Exists(ConfigurationManager.AppSettings["LOGPATH"]))
                    if (File.Exists(ConfigurationManager.AppSettings["LOGPATH"] + "LATESTDATE.TXT"))
                    {
                        string[] sAllLine = File.ReadAllLines(ConfigurationManager.AppSettings["LOGPATH"] + "LATESTDATE.TXT");
                        if (sAllLine.Length > 0)
                            sDate = sAllLine[0];
                    }
                WriteLine("Date from file: " + sDate);
                return sDate;
            }
            catch (Exception ex)
            {
                WriteLine("GetLatestDate: Exception: " + ex.Message);
                return string.Empty; ;
            }
            finally
            {
                //GC.Collect();
            }
        }


        private static void WriteLine(string sMsg)
        {
            sMsg = DateTime.Now.ToShortDateString() + "  " + DateTime.Now.ToLongTimeString() + " -- " + sMsg;
            pSWriter = File.AppendText(sPath);
            pSWriter.WriteLine(sMsg);
            //DrawProgressBar();
            Console.WriteLine(sMsg);
            pSWriter.Close();
        }

        //private static mmLicenseStatus CheckOutLicenses(mmLicensedProductCode productCode)
        //{
        //    mmLicenseStatus licenseStatus;
        //    licenseStatus = arcFMAppInitialize.IsProductCodeAvailable(productCode);
        //    if (licenseStatus == mmLicenseStatus.mmLicenseAvailable)
        //    {
        //        licenseStatus = arcFMAppInitialize.Initialize(productCode);
        //    }
        //    return licenseStatus;
        //}

        private static IWorkspace ArcSdeWorkspaceFromFile(String connectionFile)
        {
            return ((IWorkspaceFactory)Activator.CreateInstance(Type.GetTypeFromProgID("esriDataSourcesGDB.SdeWorkspaceFactory"))).
                OpenFromFile(connectionFile, 0);
        }

        #region Method foor EDGIS ReArch-v1t8

       
        /// <summary>
        /// Method to processs VMS data 
        /// </summary>
        /// <param name="versionName"></param>
        /// <returns>bool</returns>
        private static bool ProcessVMSData(string versionName)
        {
            bool success = false;

            try
            {
                //To get VMS data from VMS database and insert the same into oracle staging table 
                if (ConfigurationManager.AppSettings["INCLUDE_DATE"].ToUpper() == "Y")
                {
                    GetInsertUpdate_IncludeDate();
                }
                else
                {
                    GetInsertUpdate();
                }
                // m4jf edgisrearch 919
                //using (OracleConnection connection = new OracleConnection(ConfigurationManager.AppSettings["TARGET_CONN_KEY"]))
                if (StageDataNull())
                {
                    string strMessage = ConfigurationManager.AppSettings["MAIL_BODY_FOR_STGFAIL"];
                    SendMailtoOperationForFeatureCountError(strMessage);
                    throw new Exception("Terminate the process as Staging table is having null data");
                }


                using (OracleConnection connection = new OracleConnection(LBMAINTConnStr))
                {
                    connection.Open();
                    Console.WriteLine("Calling LBGIS.VMSAlertFeature stored procedure");
                    Oracle.DataAccess.Client.OracleCommand oracleCommand = new Oracle.DataAccess.Client.OracleCommand();
                    oracleCommand.Connection = connection;

                    //Code added to read sp name from config file for EDGIS ReArch
                    // oracleCommand.CommandText = "EDGIS.INSERT_SAP_INTEGRATED_RESULT"; SPIntegratedResult
                    oracleCommand.CommandText = ConfigurationManager.AppSettings["VMSStoredProcedure"];
                    oracleCommand.CommandType = CommandType.StoredProcedure;
                    //Below line added to pass version name as a parameter in stored procedure-ReArch Improvement 
                    oracleCommand.Parameters.Add("", OracleDbType.Varchar2).Value = versionName;
                    oracleCommand.ExecuteNonQuery();
                    connection.Close();
                    //_log.Info("EDGIS.INSERT_SAP_INTEGRATED_RESULT Completed!!");
                    Console.WriteLine(" stored procedureLBGIS.VMSAlertFeature Completed!!");

                    WriteLine(" Stored procedure LBGIS.VMSAlertFeature Completed!!");
                    success = true;
                    //resultInt = 0;
                    return success;
                }
            }   
            catch (Exception ex)
            {                
                WriteLine(" Exception while executing stored procedure");
                Console.WriteLine("Exception while executing stored procedure", ex.StackTrace);               
                throw ex;
            }
        } 

        private static bool StageDataNull()
        {
            bool CheckNulldata = false;
            try
            {
                string squery= "select count(*) from " + ConfigurationManager.AppSettings["STG_TABLE"].ToString();
                DataTable dt = GetDataTableByQuery(squery);
                if (dt != null)
                {
                    if (dt.Rows.Count > 0)
                    {
                        if (Convert.ToInt32(dt.Rows[0].ItemArray[0].ToString()) == 0)
                        {
                            CheckNulldata = true;
                            WriteLine(ConfigurationManager.AppSettings["STG_TABLE"].ToString() + " -- is having no data ");
                            
                        }
                        else
                        {
                            WriteLine(ConfigurationManager.AppSettings["STG_TABLE"].ToString() + " -- is having total no of records- " + dt.Rows[0].ItemArray[0].ToString());

                        }
                    }
                }
                
            }
            catch(Exception ex) 
            {
                throw ex;
            }

            return CheckNulldata;
        }

        public static void SendMailtoOperationForFeatureCountError( string strMessage)
        {
            try
            {


                String strToList = String.Empty;
                String strFromList = String.Empty;
                String strSubject = String.Empty;


                strToList = ConfigurationManager.AppSettings["TO_LANID"];
                strFromList = ConfigurationManager.AppSettings["FROM_LANID"];
                strSubject = ConfigurationManager.AppSettings["MAIL_SUBJECT"];

                // strHost = (codeRow.get_Value(codeRow.Fields.FindField("HOST"))).ToString();

                string[] LanIDsList = strToList.Split(';');
                //send email

                //for (int i = 0; i < LanIDsList.Length; i++)
                //{
                //    string lanID = LanIDsList[i];
                EmailService.Send(mail =>
                {
                    mail.From = strFromList;
                    mail.FromDisplayName = strFromList;
                    mail.To = strToList;
                   
                    mail.Subject = strSubject;
                    
                    mail.BodyText = strMessage;

                });
                //}

            }
            catch (Exception ex)
            {

                WriteLine("Exception occurred while sending the mail for failed Job--" + ex.Message.ToString());
            }

        }

        public static DataTable GetDataTableByQuery(string strQuery)
        {
            //Open Connection
          
            DataSet dsData = new DataSet();
            DataTable DT = new DataTable();
            OracleCommand cmdExecuteSQL = null;
            OracleDataAdapter daOracle = null;
            try
            {
                OracleConnection _oraConnection = new OracleConnection(LBMAINTConnStr);
                _oraConnection.Open();
                cmdExecuteSQL = new OracleCommand();
                cmdExecuteSQL.CommandType = CommandType.Text;
                cmdExecuteSQL.CommandTimeout = 0;
                cmdExecuteSQL.CommandText = strQuery;
                cmdExecuteSQL.Connection = _oraConnection;

                daOracle = new OracleDataAdapter();
                daOracle.SelectCommand = cmdExecuteSQL;
                daOracle.Fill(DT);
                _oraConnection.Close();

            }
            catch (Exception ex)
            {
                WriteLine("Exception encountered while getting the data from query with querystring " + strQuery + " Exception -- " + ex.Message.ToString());
                throw ex;
            }
            finally
            {
                cmdExecuteSQL.Dispose();
                daOracle.Dispose();
                

            }
            return DT;
        }
        /// <summary>
        /// To get staging table schema -v1t8
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns>DataTable</returns>
        public static DataTable GetStagingDataTable(string tableName)
        {
            DataTable DBTable = default;
            try
            {
                // m4jf edgisrearch 919 
                //OracleConnection _oraConnection = new OracleConnection(ConfigurationManager.AppSettings["TARGET_CONN_KEY"]);
                OracleConnection _oraConnection = new OracleConnection(LBMAINTConnStr);

                System.Data.Common.DbCommand GetTableCmd = _oraConnection.CreateCommand();
                System.Data.Common.DbDataAdapter ODA = null;
                DBTable = new DataTable();
                ODA = new OracleDataAdapter() as System.Data.Common.DbDataAdapter;
                ODA.SelectCommand = GetTableCmd;
                GetTableCmd.CommandText = "SELECT * FROM " + tableName;
                ODA.FillSchema(DBTable, SchemaType.Source);//This pulls down the schema for the given table
                DBTable.TableName = tableName;
                ODA.Dispose();
                _oraConnection.Dispose();
                return DBTable;
            }
            catch (Exception ex)
            {
                WriteLine("Exception in GetStagingDataTable() " + ex.Message);
                throw ex;
            }

        }

        #endregion 
    }
}
