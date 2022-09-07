using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Configuration;
using System.Text.RegularExpressions;

using ESRI.ArcGIS.Geodatabase;

using Miner.Interop;
using Miner.Geodatabase.Edit;
using Miner.Interop.Process;
using Oracle.DataAccess.Client;
using System.Data.OleDb;
using PGE_DBPasswordManagement;

namespace PGE.BatchApplication.UpdateFeatures
{
    public class MainClass
    {
        private static IMMPxApplication _pxApplication;
        private static IMMSession _mmSession;
        //private static string _versionName;

        public static bool StartProcess()
        {
            bool bOperaionSuccess = true;
            DataTable TransformersToUpdate = null;    
            //List<DataTable> listTables = null;                    
            try
            {
                bOperaionSuccess = PreRequiste();

                _pxApplication = new PxApplicationClass();

                if (!bOperaionSuccess)
                {
                    Common._log.Info("Exiting the process.");
                    return false;
                }

                Common._log.Info("Connection string used in process ." + ReadConfigurations.GetValue(ReadConfigurations.EDWorkSpaceConnString));
               // M4JF EDGISREARC 919 - Updated below line of code - Get sde connection file using Passwordmanagement tool 
                IWorkspace workspace = GetWorkspace(ReadEncryption.GetSDEPath(ReadConfigurations.GetValue(ReadConfigurations.EDWorkSpaceConnString).ToUpper()));

                Common._log.Info("Workspace checked out successfully.");

                if (workspace == null)
                {
                    Common._log.Error("Work space not found for conn string : " + ReadConfigurations.GetValue(ReadConfigurations.EDWorkSpaceConnString));
                    Common._log.Info("Exiting the process.");
                    return false;
                }

                string strVersionName = ReadConfigurations.GetValue(ReadConfigurations.VersionNamePrefix);
                UpdateFeatureDataByChunk(workspace, strVersionName, TransformersToUpdate); 
                
            }
            catch (Exception exp)
            {
                bOperaionSuccess = false;
                Common._log.Info(exp.Message+"   "+exp.StackTrace);
            }
            finally
            {
                Common.CloseLicenseObject();

                //if (listTables != null)
                //{
                //    listTables.Clear();
                //    listTables = null;
                //}
            }
            return bOperaionSuccess;
        }

        public static bool UpdateFeatureDataByChunk(IWorkspace argWorkspace, string argVersionName, DataTable argDtFeaturesToUpdate)
        {
            bool bOperaionSuccess = true;
            try
            {
                //Not disabling autoupdaters ...                      
                try
                {
                    if (argWorkspace == null)
                    {
                        return false;
                    }

                    if (!bOperaionSuccess)
                    {
                        return false;
                    }

                    //Create Session code , if needed only comment/uncomment below line to create or not create session
                    argVersionName = CreateNewSession();


                    if (string.IsNullOrEmpty(argVersionName))
                    {
                        Common._log.Error("Version name after creating session is null.");
                    }
                    else
                    {
                        IVersion2 dataversion = CreateVersion(argWorkspace, argVersionName);
                        Common._log.Info("Created new version " + argVersionName);
                        if (dataversion == null)
                        {
                            return false;
                        }

                        IFeatureWorkspace featversionWorkspace = (IFeatureWorkspace)dataversion;
                        IFeatureClass featureClassToUpdate = featversionWorkspace.OpenFeatureClass(ReadConfigurations.GetValue(ReadConfigurations.TransformerClassName));

                        if (featureClassToUpdate == null)
                        {
                            Common._log.Error("Feature class not found.");
                            return false;
                        }

                        // m4jf edgisrearch 919 - updated connection string - get connection string using Password management tool

                        //string strConnectionString = ConfigurationManager.ConnectionStrings[ReadConfigurations.connStringreleditor].ConnectionString;


                        string strConnectionString = "Provider=MSDAORA;Persist Secutiry Info=True;PLSQLRSet=1;" + ReadEncryption.GetConnectionStr(ReadConfigurations.GetValue(ReadConfigurations.EDConnection_Str).ToUpper());


                        //string strSqlQueryAndPhase = ReadConfigurations.GetValue(ReadConfigurations.QueryToGetRecords);
                        //int phaseDesignationValue = 0;

                        //string strSqlQuery = strSqlQueryAndPhase.Split('@')[0];
                        string strSqlQuery = ReadConfigurations.GetValue(ReadConfigurations.QueryToGetRecords);
                        //phaseDesignationValue = Convert.ToInt16(strSqlQuery.Split('@')[1]);
                        DataTable TransformersToUpdate = GetDataToUpdate(strConnectionString, strSqlQuery);

                        DataTable TransformerUnitsToUpdate = GetDataToUpdate(strConnectionString, ReadConfigurations.GetValue(ReadConfigurations.QueryToGetTrUnitRecords));

                        UpdateFeatures(featversionWorkspace, featureClassToUpdate, TransformersToUpdate, TransformerUnitsToUpdate, argVersionName);
                    }
                }
                catch (Exception exp)
                {
                    Common._log.Error("Error while updaing SL features in chunk." + exp.Message);
                    Common._log.Info(exp.Message + "   " + exp.StackTrace);
                }
                finally
                {

                }
            }
            catch (Exception exp)
            {
                bOperaionSuccess = false;
                Common._log.Error("Error while updaing SL features in chunk." + exp.Message);
                Common._log.Info(exp.Message + "   " + exp.StackTrace);
            }
            return bOperaionSuccess;
        }

        //public static DataTable GetServiceLocationsToUpdate()
        //{
        //    DataTable dtServiceLocation = null;
        //    string strSqlQuery = null;
        //    string strConnectionString = null;
        //    try
        //    {
        //        Common._log.Info("Fetching connection string.");

        //        string[] con = ReadConfigurations.GetCommaSeparatedList(ReadConfigurations.EDConnection, new string[4]);
        //        strConnectionString = Common.CreateOracleConnectionString(con[0], con[1], con[2], con[3]);

        //        Common._log.Info("Connection string to fetch records to update data : " + strConnectionString);
               
        //        strSqlQuery = ReadConfigurations.GetValue(ReadConfigurations.QueryToGetRecords);

        //        using (OracleConnection oracleconn = new OracleConnection(strConnectionString))
        //        {
        //            oracleconn.Open();
        //            using (OracleCommand cmd = new OracleCommand(strSqlQuery, oracleconn))
        //            {
        //                cmd.CommandType = CommandType.Text;
        //                cmd.CommandTimeout = 600;

        //                using (OracleDataAdapter da = new OracleDataAdapter(cmd))
        //                {
        //                    DataSet ds = new DataSet();
        //                    da.Fill(ds);
        //                    dtServiceLocation = ds.Tables[0];

        //                    Common._log.Info("Total records to update : " + ds.Tables[0].Rows.Count);
        //                }
        //            }
        //        } 
        //    }
        //    catch (Exception exp)
        //    {
        //        Common._log.Error("Error while fetching data to update from database table."+exp.Message);
        //        Common._log.Info(exp.Message + "   " + exp.StackTrace);
        //    }
        //    return dtServiceLocation;
        //}

        public static DataTable GetDataToUpdate(string strConnectionString ,string strSqlQuery)
        {
            DataTable dtServiceLocation = new DataTable();
            //string strSqlQuery = null;
            //string strConnectionString = null;
            try
            {
                Common._log.Info("Fetching connection string.");

                //string[] con = ReadConfigurations.GetCommaSeparatedList(ReadConfigurations.EDConnection, new string[4]);
                //strConnectionString = Common.CreateOracleConnectionString(con[0], con[1], con[2], con[3]);

                //strConnectionString = ConfigurationManager.ConnectionStrings[ReadConfigurations.connString].ConnectionString;
             
                Common._log.Info("Connection string to fetch data : " + strConnectionString);                

                Common._log.Info("SQL query to fetch data : " + strConnectionString);

                using (OleDbConnection con = new OleDbConnection(strConnectionString))
                {
                    con.Open();
                    Common._log.Info("Connected To DataBase...");
                    //string sql = "Select * from PGEDATA.GEN_SUMMARY_STAGE ";
                    OleDbDataAdapter adp = new OleDbDataAdapter(strSqlQuery, con);
                    adp.Fill(dtServiceLocation);
                    Common._log.Info("Total data to update : " + dtServiceLocation.Rows.Count);
                }

                //using (OracleConnection oracleconn = new OracleConnection(strConnectionString))
                //{
                //    oracleconn.Open();
                //    using (OracleCommand cmd = new OracleCommand(strSqlQuery, oracleconn))
                //    {
                //        cmd.CommandType = CommandType.Text;
                //        cmd.CommandTimeout = 600;

                //        using (OracleDataAdapter da = new OracleDataAdapter(cmd))
                //        {
                //            DataSet ds = new DataSet();
                //            da.Fill(ds);
                //            dtServiceLocation = ds.Tables[0];

                //            Common._log.Info("SL to update fetched, Total SL to update : " + ds.Tables[0].Rows.Count);
                //        }
                //    }
                //}
            }
            catch (Exception exp)
            {
                Common._log.Error("Error while fetching data to update from database table." + exp.Message);
                Common._log.Info(exp.Message + "   " + exp.StackTrace);
            }
            return dtServiceLocation;
        }

        //private static bool CreateSession(string localCount)
        //{
        //    IMMPxApplication _pxApplication=null;
        //    IMMSession _mmSession;
        //    bool bSessionCreated = true;
        //    try
        //    {
        //        _pxApplication = new PxApplicationClass();

        //        IMMPxLogin pxLogin = (IMMPxLogin)new PxLoginClass();

        //        _pxApplication.StandAlone = true;
        //        _pxApplication.Visible = false;

        //        string smUser = ReadConfigurations.GetValue(ReadConfigurations.SMUser);
        //        string smPwd = ReadConfigurations.GetValue(ReadConfigurations.SMUPWD) ;
        //        string smDB = ReadConfigurations.GetValue(ReadConfigurations.SMDB);
        //        string versionNamePrefix = ReadConfigurations.GetValue(ReadConfigurations.VersionNamePrefix);

        //        string sessionName = ReadConfigurations.GetValue(ReadConfigurations.SessionName);

        //        pxLogin.ConnectionString = "Provider=OraOLEDB.Oracle.1;User ID= " + smUser + ";Data Source= " + smDB + ";Password= " + smPwd;
          
        //        _pxApplication.Startup(pxLogin);

        //        IMMSessionManager sessionMgr = (IMMSessionManager)_pxApplication.FindPxExtensionByName("MMSessionManager");
        //        _mmSession = sessionMgr.CreateSession();
                
        //        //For this session - ISMMSession property must be true;

        //        int sID = _mmSession.get_ID();

        //        sessionName = sessionName + "_" + localCount;
        //        sessionName = Regex.Replace(sessionName, "/", "");
        //        _versionName = versionNamePrefix + sID;

        //        _mmSession.set_Name(_versionName);

        //        //DateTime dt = _mmSession.get_CreateDate();
        //        //string user = _mmSession.get_CreateUser();
        //        //string owner = _mmSession.get_Owner();

        //        //_mmSession.set_CreateDate(DateTime.Now);
        //        //_mmSession.set_CreateUser(smUser);
        //        _mmSession.set_Description("Session " + _versionName + " : Created to updae service location gencategory values for primary generations.");
                

        //        Common._log.Info("----------------------------------------------------------------------");
        //        Common._log.Info("Created Session: " + _versionName + ", Session ID: " + sID.ToString());
        //        Common._log.Info("----------------------------------------------------------------------");               
        //    }
        //    catch (Exception exp)
        //    {
        //        Common._log.Error("Error in creating session."+exp.Message);
        //        bSessionCreated = false;
        //        Common._log.Info(exp.Message + "   " + exp.StackTrace);
        //    }
        //    return bSessionCreated;
        //}

        private static IVersion2 CreateVersion(IWorkspace argWorkspace, string slversionname)
        {
            IVersion2 slversion = null;
            try
            {
                IVersionedWorkspace vWorkspace = (IVersionedWorkspace)argWorkspace;
                {
                    IVersion sourceVersion = (IVersion2)vWorkspace.FindVersion(ReadConfigurations.GetValue(ReadConfigurations.DefaultVersionName));
                    try
                    {
                        Common._log.Info("Checking whether " + slversionname + " already exist or not");
                        slversion = (IVersion2)vWorkspace.FindVersion(slversionname);
                    }
                    catch (Exception exp)
                    {
                        // throw;
                    }

                    if (slversion != null)
                    {
                        try
                        {
                            Common._log.Info(slversionname + " already exist, stopping the process.");
                            return null;
                        }
                        catch (Exception exp)
                        {
                            //   throw;
                        }
                    }

                    Common._log.Info("Creating  " + slversionname + " version");
                    sourceVersion.CreateVersion(slversionname);

                    slversion = (IVersion2)vWorkspace.FindVersion(slversionname);
                    slversion.Access = esriVersionAccess.esriVersionAccessPublic;
                }
            }
            catch (Exception exp)
            {
                Common._log.Error("Error in creating version." + exp.Message);
                Common._log.Info(exp.Message + "   " + exp.StackTrace);
            }
            return slversion;
        }

        private static IWorkspace GetWorkspace(string argStrworkSpaceConnectionstring)
        {
            Type t = null;
            IWorkspaceFactory workspaceFactory = null;
            IWorkspace workspace = null;
            try
            {

                t = Type.GetTypeFromProgID("esriDataSourcesGDB.SdeWorkspaceFactory");
                workspaceFactory = (IWorkspaceFactory)Activator.CreateInstance(t);
                workspace = workspaceFactory.OpenFromFile(argStrworkSpaceConnectionstring, 0);
            }
            catch (Exception exp)
            {
                Common._log.Error("Error in getting SDE Workspace, function GetWorkspace: " + exp.Message);
                Common._log.Info(exp.Message + "   " + exp.StackTrace);
                // throw;
            }
            return workspace;
        }     

        public static bool PreRequiste()
        {
            bool ballPreRequisiteSuccessful = true;
            try
            {
                Common.InitializeESRILicense();
                Common.InitializeArcFMLicense();
            }
            catch (Exception exp)
            {
                Common._log.Error("Error in Initializing Arc GIS/ArcFM License, function PreRequiste " + exp.Message);
                Common._log.Info(exp.Message + "   " + exp.StackTrace);
                //throw;
            }
            return ballPreRequisiteSuccessful;
        }

        public static bool UpdateFeatures(IFeatureWorkspace argFeatversionWorkspace, IFeatureClass argInputFeatureClass, DataTable argdtInputData,DataTable TransformerUnitsToUpdate, string argStrVersionName)
        {
            bool bOperationSuccess = false;           
            IFeature pFeature = null;
            IRow pTrUnitRow = null;
            string OID = null;
            int oidfeature = 0;
            int phasedesignation = 0;
            int phasedesignationfromdb = 0;
            int countRecordsProcessed = 0;
            string varifiedStatusValue = ReadConfigurations.GetValue(ReadConfigurations.PhaseVerifiedValue);//Estimated/Defaulted
            try
            {
                IWorkspaceEdit editSession = (IWorkspaceEdit)argFeatversionWorkspace;
                editSession.StartEditing(false);
                editSession.StartEditOperation();
              
                argInputFeatureClass = argFeatversionWorkspace.OpenFeatureClass(ReadConfigurations.GetValue(ReadConfigurations.TransformerClassName));

                Common._log.Info("Edit operation started.");
                Common._log.Info("");
                Common._log.Info("Total transformer records to update : " + argdtInputData.Rows.Count);
                foreach (DataRow pRow in argdtInputData.Rows)
                {
                    try
                    {
                        if (int.TryParse(Convert.ToString(pRow["trphdsgtoupdate"]), out phasedesignation) && int.TryParse(Convert.ToString(pRow["TRANSOID"]), out oidfeature))
                        {
                            pFeature = argInputFeatureClass.GetFeature(oidfeature);

                            int.TryParse(Convert.ToString(pFeature.get_Value(pFeature.Fields.FindField("PHASEDESIGNATION"))), out phasedesignationfromdb);

                            if (phasedesignationfromdb != phasedesignation)
                            {
                                pFeature.set_Value(pFeature.Fields.FindField("PHASEDESIGNATION"), phasedesignation);
                                pFeature.set_Value(pFeature.Fields.FindField("phasingverifiedstatus"), varifiedStatusValue);
                                pFeature.Store();
                            }
                            countRecordsProcessed++;

                            if (countRecordsProcessed % 200 == 0)
                            {
                                Common._log.Info("Records processed till yet : " + countRecordsProcessed);
                            }
                        }
                    }
                    catch (Exception exp)
                    {
                        Common._log.Error("Error while updaing feature with OID : " + OID + " __" + exp.Message);
                        Common._log.Error(exp.Message + "   " + exp.StackTrace);
                    }
                    finally
                    {
                        GC.Collect();
                        GC.WaitForPendingFinalizers();
                    }
                }

                Common._log.Info("All transformer records updated");


                try
                {
                    //Updating Transform units here 

                    ITable pTable = argFeatversionWorkspace.OpenTable("EDGIS.TRANSFORMERUNIT");
                    countRecordsProcessed = 0;
                    Common._log.Info("Transformer unit updation started.");
                    Common._log.Info("");
                    Common._log.Info("Total Transformer unit records to update : " + TransformerUnitsToUpdate.Rows.Count);
                    foreach (DataRow pRow in TransformerUnitsToUpdate.Rows)
                    {
                        try
                        {                            

                            if (int.TryParse(Convert.ToString(pRow["trunitphdsgtoupdate"]), out phasedesignation) && int.TryParse(Convert.ToString(pRow["TRANSUNITOBJECTID"]), out oidfeature))
                            {
                                pTrUnitRow = pTable.GetRow(oidfeature);

                                int.TryParse(Convert.ToString(pTrUnitRow.get_Value(pTrUnitRow.Fields.FindField("PHASEDESIGNATION"))), out phasedesignationfromdb);

                                if (phasedesignationfromdb != phasedesignation)
                                {
                                    pTrUnitRow.set_Value(pTrUnitRow.Fields.FindField("PHASEDESIGNATION"), phasedesignation);
                                    pTrUnitRow.Store();
                                }
                                countRecordsProcessed++;

                                if (countRecordsProcessed % 200 == 0)
                                {
                                    Common._log.Info("Records processed till yet : " + countRecordsProcessed);
                                }
                            }
                        }
                        catch (Exception exp)
                        {
                            Common._log.Error("Error while updaing feature with OID : " + OID + " __" + exp.Message);
                            Common._log.Error(exp.Message + "   " + exp.StackTrace);
                        }
                        finally
                        {
                            GC.Collect();
                            GC.WaitForPendingFinalizers();
                        }
                    }

                    Common._log.Info("All transformer unit records updated");

                }
                catch (Exception exp)
                {
                     Common._log.Error(exp.Message + "   " + exp.StackTrace);
                }


                editSession.StopEditOperation();
                editSession.StopEditing(true);                
                Common._log.Info("");
            }
            catch (Exception exp)
            {
                Common._log.Error("Error while updaing features." + exp.Message);
                Common._log.Info(exp.Message + "   " + exp.StackTrace);
            }
            return bOperationSuccess;
        }


        private static string CreateNewSession()
        {
            string versionName = string.Empty;

            try
            {
                IMMPxLogin pxLogin = (IMMPxLogin)new PxLoginClass();

                _pxApplication.StandAlone = true;
                _pxApplication.Visible = false;

                // m4jf edgisrearch 919 - commented below lines - Get connection string using Password management tool
                //string smUser = ConfigurationManager.AppSettings["SMUser"];
                //string smPwd = ConfigurationManager.AppSettings["SMUPWD"];
                //string smDB = ConfigurationManager.AppSettings["SMDB"];
                string versionNamePrefix = "SN_";

                string sessionName = ConfigurationManager.AppSettings["SessionName"];

                // m4jd edgisrearch 9191 - updated connection string - get connection string using Password Management tool
                //pxLogin.ConnectionString = "Provider=OraOLEDB.Oracle.1;User ID= " + smUser + ";Data Source= " + smDB + ";Password= " + smPwd;

                pxLogin.ConnectionString = "Provider=OraOLEDB.Oracle.1;"+ReadEncryption.GetConnectionStr(ReadConfigurations.GetValue(ReadConfigurations.EDConnectionString).ToUpper());


                //pxLogin.PromptConnection(

                _pxApplication.Startup(pxLogin);

                IMMSessionManager sessionMgr = (IMMSessionManager)_pxApplication.FindPxExtensionByName("MMSessionManager");
                _mmSession = sessionMgr.CreateSession();

                int sID = _mmSession.get_ID();

                //sessionName = sessionName + +sID;  //session name will be SN_<NodeId>
                //sessionName = Regex.Replace(sessionName, "/", "");
                versionName = versionNamePrefix + sID; //session name will be SN_<NodeId>
                sessionName = versionName;

                _mmSession.set_Name(sessionName);
                Common._log.Info("----------------------------------------------------------------------");
                Common._log.Info("Created Session: " + sessionName + ", Session ID: " + sID.ToString());

                Common._log.Info("----------------------------------------------------------------------");
            }
            catch (Exception exp)
            {
                Common._log.Info(exp.Message+" at "+exp.StackTrace);
            }
            return versionName;
        }

    }
}
