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

//ENOS2EDGIS - Updating service locations for primary generation
namespace PGE.Interfaces.UpdateServiceLocationForPrimaryGen
{
    public class MainClass
    {
        private static string _versionName;

        public static bool StartProcess()
        {
            bool bOperaionSuccess = true;
            DataTable servLocsToUpdate = null;
            DataTable servLocsToUpdate_withLocalOffice = null;
            int iCountDataTables = 0;
            List<DataTable> listTables = null;
            int minValue;
            int maxValue ;
            int multiplier;
            int skipCount;
           
            try
            {
                bOperaionSuccess = PreRequiste();

                if (!bOperaionSuccess)
                {
                    Common._log.Info("Exiting the process.");
                    return false;
                }

                Common._log.Info("Connection string used in process ." + ReadConfigurations.GetValue(ReadConfigurations.EDWorkSpaceConnString));
               // m4jf edgisrearch 919
                IWorkspace workspace = GetWorkspace(ReadEncryption.GetSDEPath(ReadConfigurations.GetValue(ReadConfigurations.EDWorkSpaceConnString)).ToUpper());

                Common._log.Info("Workspace checked out successfully.");

                if (workspace == null)
                {
                    Common._log.Error("Work space not found for conn string : " + ReadConfigurations.GetValue(ReadConfigurations.EDWorkSpaceConnString));
                    Common._log.Info("Exiting the process.");
                    return false;
                }

                // Fetch all the service locations to be updated in a DataTable 
                //servLocsToUpdate = GetServiceLocationsToUpdate();

                string strSqlQuery = ReadConfigurations.GetValue(ReadConfigurations.QueryToGetRecordsFromSL);
                servLocsToUpdate = GetServiceLocationsToUpdate_New(strSqlQuery);

                strSqlQuery = ReadConfigurations.GetValue(ReadConfigurations.localOfficeQueryValue); 
                servLocsToUpdate_withLocalOffice = GetServiceLocationsToUpdate_New(strSqlQuery);

                listTables = new List<DataTable>();

                foreach (DataRow pRow in servLocsToUpdate_withLocalOffice.Rows)
                {
                    try
                    {
                        string localoffice = pRow.Field<string>("LOCALOFFICE");
                        decimal localofficeCount = pRow.Field<decimal>("COUNT");

                        if (string.IsNullOrEmpty(localoffice))
                        {

                            string query = "LOCALOFFICE IS NULL";
                            DataTable dt = servLocsToUpdate.Select(query).CopyToDataTable();

                            foreach (DataRow pInnerRow in dt.Rows)
                            {
                                string GUID = pInnerRow.Field<string>("GLOBALID");
                                query = "GLOBALID='" + GUID + "'";
                                //DataTable dt = servLocsToUpdate.Select(query).CopyToDataTable();
                                listTables.Add(servLocsToUpdate.Select(query).CopyToDataTable());
                            }      
                        }
                        else if (!string.IsNullOrEmpty(localoffice) && localofficeCount<=10)
                        {
                            string query = "LOCALOFFICE='" + localoffice + "'";
                            //DataTable dt = servLocsToUpdate.Select(query).CopyToDataTable();
                            listTables.Add(servLocsToUpdate.Select(query).CopyToDataTable());

                        }
                        else if (!string.IsNullOrEmpty(localoffice) && localofficeCount > 10)
                        {

                            string query = "LOCALOFFICE='" + localoffice + "'";
                            DataTable dtServiceLocations = servLocsToUpdate.Select(query).CopyToDataTable();

                            minValue = Convert.ToInt32(ReadConfigurations.GetValue(ReadConfigurations.minValue));
                            maxValue = Convert.ToInt32(ReadConfigurations.GetValue(ReadConfigurations.maxValue));
                            multiplier = Convert.ToInt32(ReadConfigurations.GetValue(ReadConfigurations.multiplier));

                            Common._log.Info(multiplier + " service locations will be updated per session.");

                            int dtCount = (dtServiceLocations.Rows.Count / multiplier) + 1;

                            Common._log.Info("Total " + dtCount + " sessions will be created to update all service locations.");

                            for (int iCount = 1; iCount <= dtCount; iCount++)
                            {
                                if (dtServiceLocations.AsEnumerable().Skip((iCount - 1) * multiplier).Take(multiplier).Count() > 0)
                                    listTables.Add(dtServiceLocations.AsEnumerable().Skip((iCount - 1) * multiplier).Take(multiplier).CopyToDataTable());

                                if (multiplier == 1)
                                    break;

                            }
                        }

                    }
                    catch (Exception exp)
                    {
                        Common._log.Error(exp.Message + "   " + exp.StackTrace);
                    }                
                }

                //DataTable dtss = listTables[79];

                Common._log.Info("Starting the process to update service locations.");

                iCountDataTables = Convert.ToInt32(ReadConfigurations.GetValue(ReadConfigurations.SessionNumberStartValue));

                foreach (DataTable dtServiceLocations in listTables)
                {
                    try
                    {                       
                        // Update n servicelocation at a time -- Keep value in config to control number of sessions to be created 
                        UpdateServiceLocationByChunk(workspace, "" + iCountDataTables, dtServiceLocations);
                        iCountDataTables++;
                    }
                    catch (Exception exp)
                    {
                        Common._log.Error(exp.Message + "   " + exp.StackTrace);
                    }
                }
            }
            catch (Exception exp)
            {
                bOperaionSuccess = false;
                Common._log.Info(exp.Message+"   "+exp.StackTrace);
            }
            finally
            {
                Common.CloseLicenseObject();

                if (listTables != null)
                {
                    listTables.Clear();
                    listTables = null;
                }
            }
            return bOperaionSuccess;
        }

        public static bool UpdateServiceLocationByChunk(IWorkspace argWorkspace, string argSessionName, DataTable argDtServiceLocations)
        {
            bool bOperaionSuccess = true;
            string strVersionName = null;            
            try
            {   
                //Not disabling autoupdaters ...
                //Code block to disable Auto Updaters
                Common._log.Info("Disabling the auto updaters.");
                Type type = Type.GetTypeFromProgID("mmGeoDatabase.MMAutoUpdater");
                IMMAutoUpdater auController = (IMMAutoUpdater)Activator.CreateInstance(type);
                mmAutoUpdaterMode prevAUMode = auController.AutoUpdaterMode;
                auController.AutoUpdaterMode = mmAutoUpdaterMode.mmAUMNoEvents;
                Common._log.Info("Auto updaters have been disabled.");    
                try
                {
                    if (argWorkspace == null)
                    {
                        return false;
                    }                   
                   
                    //bOperaionSuccess = CreateSession(argSessionName);

                    if (!bOperaionSuccess)
                    {
                        return false;
                    }

                    //VersionVsSession
                    _versionName = ReadConfigurations.GetValue(ReadConfigurations.VersionNamePrefix) + "_" + argSessionName;

                    IVersion2 dataversion = CreateVersion(argWorkspace, _versionName);

                    if (dataversion == null)
                    {
                        return false;
                    }

                    //_log.Info("Creating new version " + _versionName);
                    IFeatureWorkspace featversionWorkspace = (IFeatureWorkspace)dataversion;
                    IFeatureClass serviceLocation = featversionWorkspace.OpenFeatureClass(ReadConfigurations.GetValue(ReadConfigurations.ServiceLocationClassName));

                    if (serviceLocation == null)
                    {
                        Common._log.Error("Feature class for service location not found.");
                        return false;
                    }

                    // m4jf edgisrearch 919
                    string[] sdeconnectionParameters = ReadConfigurations.EDWorkSpaceConnString.Split('@');
                    // strVersionName = ReadConfigurations.GetValue(ReadConfigurations.SMUser) + "." + _versionName;
                    strVersionName = sdeconnectionParameters[0] + "." + _versionName;
                    UpdateServiceLocations(featversionWorkspace, serviceLocation, argDtServiceLocations, strVersionName.ToUpper());
                }
                catch (Exception exp)
                {
                    Common._log.Error("Error while updaing SL features in chunk."+exp.Message);
                    Common._log.Info(exp.Message + "   " + exp.StackTrace);
                }
                finally
                {
                    //Code to enable Auto Updaters
                    auController.AutoUpdaterMode = prevAUMode;
                    Common._log.Info("Auto updaters have been enabled again.");
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

        public static DataTable GetServiceLocationsToUpdate()
        {
            DataTable dtServiceLocation = null;
            string strSqlQuery = null;
            string strConnectionString = null;
            try
            {
                Common._log.Info("Fetching connection string.");

                // m4jf edgisrearch 919 
                //string[] con = ReadConfigurations.GetCommaSeparatedList(ReadConfigurations.EDConnection, new string[4]);
                //strConnectionString = Common.CreateOracleConnectionString(con[0], con[1], con[2], con[3]);
                strConnectionString = ReadEncryption.GetConnectionStr(ReadConfigurations.GetValue(ReadConfigurations.EDConnection).ToUpper());

                Common._log.Info("Connection string to fetch SL to update data : " + strConnectionString);
               
                strSqlQuery = ReadConfigurations.GetValue(ReadConfigurations.QueryToGetRecordsFromSL);

                using (OracleConnection oracleconn = new OracleConnection(strConnectionString))
                {
                    oracleconn.Open();
                    using (OracleCommand cmd = new OracleCommand(strSqlQuery, oracleconn))
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.CommandTimeout = 600;

                        using (OracleDataAdapter da = new OracleDataAdapter(cmd))
                        {
                            DataSet ds = new DataSet();
                            da.Fill(ds);
                            dtServiceLocation = ds.Tables[0];

                            Common._log.Info("SL to update fetched, Total SL to update : " + ds.Tables[0].Rows.Count);
                        }
                    }
                } 
            }
            catch (Exception exp)
            {
                Common._log.Error("Error while fetching service locations to update from database table."+exp.Message);
                Common._log.Info(exp.Message + "   " + exp.StackTrace);
            }
            return dtServiceLocation;
        }

        public static DataTable GetServiceLocationsToUpdate_New(string strSqlQuery)
        {
            DataTable dtServiceLocation = new DataTable();
            //string strSqlQuery = null;
            string strConnectionString = null;
            try
            {
                Common._log.Info("Fetching connection string.");

                //string[] con = ReadConfigurations.GetCommaSeparatedList(ReadConfigurations.EDConnection, new string[4]);
                //strConnectionString = Common.CreateOracleConnectionString(con[0], con[1], con[2], con[3]);
                // m4jf edgisrearch 919
                // strConnectionString = ConfigurationManager.ConnectionStrings[ReadConfigurations.connString].ConnectionString;
                string[] Connectionparameters = ConfigurationManager.AppSettings["EDER_ConnectionStr"].Split('@');
                string password = ReadEncryption.GetPassword(ConfigurationManager.AppSettings["EDER_ConnectionStr"].ToUpper());
                strConnectionString = string.Format("Provider=MSDAORA;Persist Secutiry Info=True;Data Source={0};PLSQLRSet=1;User Id={1};Password={2}", Connectionparameters[1], Connectionparameters[0],password);
                Common._log.Info("Connection string to fetch SL to update data : " + strConnectionString);                

                Common._log.Info("SQL query to fetch SL to update data : " + strConnectionString);

                using (OleDbConnection con = new OleDbConnection(strConnectionString))
                {
                    con.Open();
                    Common._log.Info("Connected To DataBase...");
                    //string sql = "Select * from PGEDATA.GEN_SUMMARY_STAGE ";
                    OleDbDataAdapter adp = new OleDbDataAdapter(strSqlQuery, con);
                    adp.Fill(dtServiceLocation);
                    Common._log.Info("SL to update fetched, Total SL to update : " + dtServiceLocation.Rows.Count);
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
                Common._log.Error("Error while fetching service locations to update from database table." + exp.Message);
                Common._log.Info(exp.Message + "   " + exp.StackTrace);
            }
            return dtServiceLocation;
        }

        private static bool CreateSession(string localCount)
        {
            IMMPxApplication _pxApplication=null;
            IMMSession _mmSession;
            bool bSessionCreated = true;
            try
            {
                _pxApplication = new PxApplicationClass();

                IMMPxLogin pxLogin = (IMMPxLogin)new PxLoginClass();

                _pxApplication.StandAlone = true;
                _pxApplication.Visible = false;

                //string smUser = ReadConfigurations.GetValue(ReadConfigurations.SMUser);
                //string smPwd = ReadConfigurations.GetValue(ReadConfigurations.SMUPWD) ;
                //string smDB = ReadConfigurations.GetValue(ReadConfigurations.SMDB);

                string[] userInst = ConfigurationManager.AppSettings["EDER_SDEConnection"].Split('@');
                string smUser = userInst[0].ToUpper();
                string smDB = userInst[1].ToUpper();

                string smPwd = ReadEncryption.GetPassword(smUser+"@"+smDB);
               

                string versionNamePrefix = ReadConfigurations.GetValue(ReadConfigurations.VersionNamePrefix);

                string sessionName = ReadConfigurations.GetValue(ReadConfigurations.SessionName);

                pxLogin.ConnectionString = "Provider=OraOLEDB.Oracle.1;User ID= " + smUser + ";Data Source= " + smDB + ";Password= " + smPwd;
          
                _pxApplication.Startup(pxLogin);

                IMMSessionManager sessionMgr = (IMMSessionManager)_pxApplication.FindPxExtensionByName("MMSessionManager");
                _mmSession = sessionMgr.CreateSession();
                
                //For this session - ISMMSession property must be true;

                int sID = _mmSession.get_ID();

                sessionName = sessionName + "_" + localCount;
                sessionName = Regex.Replace(sessionName, "/", "");
                _versionName = versionNamePrefix + sID;

                _mmSession.set_Name(_versionName);

                //DateTime dt = _mmSession.get_CreateDate();
                //string user = _mmSession.get_CreateUser();
                //string owner = _mmSession.get_Owner();

                //_mmSession.set_CreateDate(DateTime.Now);
                //_mmSession.set_CreateUser(smUser);
                _mmSession.set_Description("Session " + _versionName + " : Created to updae service location gencategory values for primary generations.");
                

                Common._log.Info("----------------------------------------------------------------------");
                Common._log.Info("Created Session: " + _versionName + ", Session ID: " + sID.ToString());
                Common._log.Info("----------------------------------------------------------------------");               
            }
            catch (Exception exp)
            {
                Common._log.Error("Error in creating session."+exp.Message);
                bSessionCreated = false;
                Common._log.Info(exp.Message + "   " + exp.StackTrace);
            }
            return bSessionCreated;
        }

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
                            Common._log.Info(slversionname + " already, deleting it.");
                            slversion.Delete();
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

        public static bool UpdateServiceLocations(IFeatureWorkspace argFeatversionWorkspace,IFeatureClass argServiceLocationFeatureClass, DataTable argDtServiceLocations,string argStrVersionName)
        {
            bool bOperationSuccess = false;
            IQueryFilter pQueryFilter = null;
            IFeatureCursor pCursor = null;
            IFeature pFeature = null;
            string guid = null;     
            int genCategory = 0;
            try
            {
                IWorkspaceEdit editSession = (IWorkspaceEdit)argFeatversionWorkspace;
                editSession.StartEditing(false);
                editSession.StartEditOperation();
              
                argServiceLocationFeatureClass = argFeatversionWorkspace.OpenFeatureClass(ReadConfigurations.GetValue(ReadConfigurations.ServiceLocationClassName));

                Common._log.Info("Edit operation started.");
                Common._log.Info("");
                Common._log.Info("");
                foreach (DataRow pRow in argDtServiceLocations.Rows)
                {
                    try
                    {
                        //guid = pRow.Field<string>(ReadConfigurations.GetValue(ReadConfigurations.Col_GLOBALID));

                        //guid = pRow.Field<string>(ReadConfigurations.GetValue(ReadConfigurations.Col_GLOBALID));
                        guid = Convert.ToString(pRow[ReadConfigurations.GetValue(ReadConfigurations.Col_GLOBALID)]);
                        Common._log.Info("GUID captured " + guid);
                        string genCategory_str = Convert.ToString(pRow[ReadConfigurations.GetValue(ReadConfigurations.Col_GENCATEGORY)]);

                        if (int.TryParse(genCategory_str, out genCategory))
                        {

                        }
                        else
                        {
                            continue;
                        }

                        Common._log.Info("Gencategory captured " + genCategory);
                        //int genCategory = pRow.Field<int>(ReadConfigurations.GetValue(ReadConfigurations.Col_GENCATEGORY));
                        string lableText = Convert.ToString(pRow[ReadConfigurations.GetValue(ReadConfigurations.Col_LABELTEXT)]);
                        //string lableText = pRow.Field<string>(ReadConfigurations.GetValue(ReadConfigurations.Col_LABELTEXT));
                        Common._log.Info("Labeltext " + lableText);

                        pQueryFilter = new QueryFilterClass();
                        pQueryFilter.WhereClause = ReadConfigurations.GetValue(ReadConfigurations.Col_GLOBALID) + "='" + guid + "'";

                        pCursor = argServiceLocationFeatureClass.Update(pQueryFilter, false);

                        pFeature = pCursor.NextFeature();

                        while (pFeature != null)
                        {
                            pFeature.set_Value(pFeature.Fields.FindField(ReadConfigurations.GetValue(ReadConfigurations.Col_GENCATEGORY)), genCategory);                                                        
                            pFeature.set_Value(pFeature.Fields.FindField(ReadConfigurations.GetValue(ReadConfigurations.Col_LABELTEXT)), lableText);
                            pFeature.set_Value(pFeature.Fields.FindField(ReadConfigurations.GetValue(ReadConfigurations.Col_VERSIONNAME)), argStrVersionName);
                            pFeature.Store();
                            pCursor.UpdateFeature(pFeature);

                            Common._log.Info("Updated SL with guid : " + guid + " updating values GENCATEGORY=" + genCategory);
                            //Common._log.Info("Updated SL with guid : " + guid + " updating values GENCATEGORY=" + genCategory + " , LABELTEXT='" + lableText + "'");

                            break;
                        }

                        Common._log.Info("Gencategory captured");

                       //string ver = Convert.ToString(pFeature.get_Value(pFeature.Fields.FindField("VERSIONNAME")));
                    }
                    catch (Exception exp)
                    {
                        Common._log.Error("Error while updaing SL feature with GUID : " + guid + " __" + exp.Message);
                        Common._log.Info(exp.Message + "   " + exp.StackTrace);
                    }
                    finally
                    {
                        if (pCursor != null)
                        {
                            System.Runtime.InteropServices.Marshal.FinalReleaseComObject(pCursor);
                        }
                        if (pQueryFilter != null)
                        {
                            System.Runtime.InteropServices.Marshal.FinalReleaseComObject(pQueryFilter);
                        }
                    }
                }

                editSession.StopEditOperation();
                editSession.StopEditing(true);

                Common._log.Info("");
                Common._log.Info("");
            }
            catch (Exception exp)
            {
                Common._log.Error("Error while updaing SL features." + exp.Message);
                Common._log.Info(exp.Message + "   " + exp.StackTrace);
            }
            return bOperationSuccess;
        }

    }
}
