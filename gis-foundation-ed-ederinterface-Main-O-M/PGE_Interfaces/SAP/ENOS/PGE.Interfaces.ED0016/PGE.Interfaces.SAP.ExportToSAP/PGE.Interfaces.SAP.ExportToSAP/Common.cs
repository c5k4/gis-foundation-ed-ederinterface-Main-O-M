using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Oracle.DataAccess.Client;
using System.Data;
using ESRI.ArcGIS.esriSystem;
using Miner.Interop;
using ESRI.ArcGIS.Geodatabase;
using System.Configuration;
using System.Reflection;
using Miner.Interop.Process;

namespace PGE.Interfaces.SAP.ExportToSAP
{
    public static class Common
    {
        public static IMMAppInitialize _mmAppInit;
        public static Log4NetLogger _log = new Log4NetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, ReadConfigurations.LOGCONFIG);
        private static AoInitializeClass m_AoInit = null;

    //    //private static LicenseInitializer m_AOLicenseInitializer = new LicenseInitializer();
    //    /// <summary>
    //    /// Initialize the license required to read/edit ArcGIS components
    //    /// </summary>
    //    /// <returns></returns>
        public static void InitializeESRILicense()
        {
            try
            {
                //Cache product codes by enum int so can be sorted without custom sorter
                List<int> m_requestedProducts = new List<int>();
                foreach (esriLicenseProductCode code in new esriLicenseProductCode[] { esriLicenseProductCode.esriLicenseProductCodeAdvanced })
                {
                    int requestCodeNum = Convert.ToInt32(code);
                    if (!m_requestedProducts.Contains(requestCodeNum))
                    {
                        m_requestedProducts.Add(requestCodeNum);
                    }
                }

                ESRI.ArcGIS.RuntimeManager.Bind(ESRI.ArcGIS.ProductCode.Desktop);

                m_AoInit = new AoInitializeClass();
                esriLicenseProductCode currentProduct = new esriLicenseProductCode();
                foreach (int prodNumber in m_requestedProducts)
                {
                    esriLicenseProductCode prod = (esriLicenseProductCode)Enum.ToObject(typeof(esriLicenseProductCode), prodNumber);
                    esriLicenseStatus status = m_AoInit.IsProductCodeAvailable(prod);
                    if (status == esriLicenseStatus.esriLicenseAvailable)
                    {
                        status = m_AoInit.Initialize(prod);
                        if (status == esriLicenseStatus.esriLicenseAlreadyInitialized ||
                            status == esriLicenseStatus.esriLicenseCheckedOut)
                        {
                            currentProduct = m_AoInit.InitializedProduct();
                        }
                    }
                }
            }
            catch (Exception Ex)
            {
                _log.Error("Unable to checkout ESRI license" + Ex.Message.ToString());
                throw Ex;

            }
        }

    //    /// <summary>
    //    /// Initializes the license necessary to read/edit ArcFM components
    //    /// </summary>
    //    /// <returns></returns>
        public static void InitializeArcFMLicense()
        {
            try
            {
                //Comm.LogManager.WriteLine("Checking out ArcFM license...");
                _mmAppInit = new MMAppInitialize();
                // check and see if the type of license is available to check out
                mmLicenseStatus mmLS = _mmAppInit.IsProductCodeAvailable(Miner.Interop.mmLicensedProductCode.mmLPArcFM);
                if (mmLS == mmLicenseStatus.mmLicenseAvailable)
                {
                    // if the license is available, try to check it out.
                    mmLicenseStatus arcFMLicenseStatus = _mmAppInit.Initialize(Miner.Interop.mmLicensedProductCode.mmLPArcFM);

                    if (arcFMLicenseStatus != mmLicenseStatus.mmLicenseCheckedOut)
                    {
                        // if the license cannot be checked out, an exception is raised
                        throw new Exception("The ArcFM license requested could not be checked out");
                    }
                }

            }
            catch (Exception Ex)
            {
                _log.Error("Unable to checkout ARCFM license" + Ex.Message.ToString());
                throw Ex;

            }
        }

    //    /// <summary>
    //    /// Closes the license object. The user will not be able to read/edit ArcGIS
    //    /// or ArcFM components after a call to this method
    //    /// </summary>
    //    /// <returns></returns>
        public static void ReleaseLicense()
        {
            //ESRI License Initializer generated code.
            //Do not make any call to ArcObjects after ShutDownApplication()            
            try
            {
                // Write if condition here 
                if (m_AoInit != null) m_AoInit.Shutdown();
                if (_mmAppInit != null) _mmAppInit.Shutdown();
            }
            catch (Exception exp)
            {
                _log.Error("Exception in function ReleaseLicense: " + exp.Message);
            }
        }

       

        //    public static DataTable GetDataTableWithSchema()
        //    {
        //        DataTable dtRecordsToUpdateInStageTable = null;
        //        try
        //        {
        //            dtRecordsToUpdateInStageTable = new DataTable();
        //            dtRecordsToUpdateInStageTable.Columns.Add(ReadConfigurations.GetValue(ReadConfigurations.DTCol_GlobalIDMain), typeof(String));
        //            dtRecordsToUpdateInStageTable.Columns.Add(ReadConfigurations.GetValue(ReadConfigurations.DTCol_GlobalIDStage), typeof(String));
        //            dtRecordsToUpdateInStageTable.Columns.Add(ReadConfigurations.GetValue(ReadConfigurations.DTCol_SPGuidStage), typeof(String));
        //            dtRecordsToUpdateInStageTable.Columns.Add(ReadConfigurations.GetValue(ReadConfigurations.DTCol_Action), typeof(String));
        //        }
        //        catch (Exception)
        //        {
        //            // throw;
        //        }
        //        return dtRecordsToUpdateInStageTable;
        //    }

        //    public static DataTable GetDataTableWithSchemaToUpdateServiceLocation()
        //    {
        //        DataTable dtUpdateServiceLocation = null;
        //        try
        //        {
        //            dtUpdateServiceLocation = new DataTable();
        //            dtUpdateServiceLocation.Columns.Add(ReadConfigurations.GetValue(ReadConfigurations.Col_Action), typeof(String));
        //            dtUpdateServiceLocation.Columns.Add(ReadConfigurations.GetValue(ReadConfigurations.Col_SERVICELOCATIONGUID), typeof(String));
        //            dtUpdateServiceLocation.Columns.Add(ReadConfigurations.GetValue(ReadConfigurations.Col_GENCATEGORY), typeof(int));
        //            dtUpdateServiceLocation.Columns.Add(ReadConfigurations.GetValue(ReadConfigurations.Col_LABELTEXT), typeof(String));
        //        }
        //        catch (Exception exp)
        //        {
        //            //throw;
        //        }
        //        return dtUpdateServiceLocation;
        //    }

        //    public static IWorkspace OpenSdeWorkspace(string sdeConnection)
        //    {
        //        ///Opening workspace and return feature work space. The connection is from the configuration file
        //        try
        //        {
        //            sdeConnection = ConfigurationManager.AppSettings["EDWorkSpaceConnString"];

        //            if (string.IsNullOrEmpty(sdeConnection))
        //            {
        //                _log.Info("Cannot read sde file path from configuration file");
        //                return null;
        //            }

        //            _log.Info("Try to connect to SDE: " + sdeConnection);
        //            Type t = Type.GetTypeFromProgID("esriDataSourcesGDB.SdeWorkspaceFactory");
        //            IWorkspaceFactory wsf = Activator.CreateInstance(t) as IWorkspaceFactory;
        //            IWorkspace featureWorkspace = wsf.OpenFromFile(sdeConnection, 0);
        //            _log.Info("Successfully connected to database");

        //            return featureWorkspace;
        //        }
        //        catch (Exception Ex)
        //        {
        //            _log.Error("Error Fail to connect to workspace: ", Ex);
        //            return null;
        //        }

        //    }
        //    public static bool CheckVersionExistFromSessionName(IWorkspace _wSpace, IMMPxApplication pxApplication)
        //    {

        //        bool SessionExist = false;

        //        IFeatureWorkspace fWSpace = (IFeatureWorkspace)_wSpace;
        //        IVersionedWorkspace vWSpace = (IVersionedWorkspace)fWSpace;
        //        try
        //        {
        //            int iSessionID = GetExistingSessionId(pxApplication);
        //            if (iSessionID != -1)
        //            {
        //                string sessionName = ConfigurationManager.AppSettings["VersionNamePrefix"] + iSessionID.ToString();
        //                IEnumVersionInfo vinfo = (IEnumVersionInfo)vWSpace.Versions;
        //                IVersionInfo pversioninfo = vinfo.Next();
        //                while (pversioninfo != null)
        //                {
        //                    string vname = pversioninfo.VersionName.ToString();

        //                    if (vname.Contains(sessionName))
        //                    {
        //                        SessionExist = true;
        //                        _log.Info("CCBToGIS Version Exist");
        //                        break;
        //                    }
        //                    pversioninfo = vinfo.Next();
        //                }
        //            }

        //        }
        //        catch (Exception EX)
        //        {

        //            _log.Error("Exception encountered while checking the existing version", EX);

        //        }
        //        return SessionExist;

        //    }

        //    public static int GetExistingSessionId(IMMPxApplication pxApplication)
        //    {
        //        _log.Debug(MethodBase.GetCurrentMethod().Name);
        //        int sessionId = -1;
        //        try
        //        {
        //            string sapdailyinterfaceversionname = ReadConfigurations.GetValue(ReadConfigurations.SAPDailyInterfaceVersionName);
        //            string sql = "SELECT max(SESSION_ID) from PROCESS.MM_SESSION where hidden=0 and " +
        //                        " upper(Session_Name) like upper('%" + sapdailyinterfaceversionname + "%')";
        //            object recordsaffected = null;
        //            ADODB.Recordset recordset = pxApplication.Connection.Execute(sql, out recordsaffected);

        //            if (!recordset.EOF)
        //            {
        //                object sessionObj = recordset.GetRows();
        //                if (sessionObj != DBNull.Value)
        //                {
        //                    System.Array array = sessionObj as System.Array;
        //                    if (array.GetValue(0, 0) != DBNull.Value)
        //                    {
        //                        sessionId = Convert.ToInt32(array.GetValue(0, 0));
        //                        _log.Info("SESSION_ID is " + sessionId);
        //                    }
        //                }
        //            }
        //        }

        //        catch (Exception EX)
        //        {

        //            _log.Error("Exception encountered while getting the sessionid", EX);

        //        }

        //        return sessionId;
        //    }


        //    public static void SendMail(string strbodyText)
        //    {
        //        try
        //        {


        //            string lanID = string.Empty;
        //            string fromDisplayName = ConfigurationManager.AppSettings["MAIL_FROM_DISPLAY_NAME"];
        //            string fromAddress = ConfigurationManager.AppSettings["MAIL_FROM_ADDRESS"];
        //            string subject = ConfigurationManager.AppSettings["MAIL_SUBJECT"];
        //            string bodyText = strbodyText;
        //            string lanIDs = ConfigurationManager.AppSettings["LANIDS"];
        //            string[] LanIDsList = lanIDs.Split(';');

        //            for (int i = 0; i < LanIDsList.Length; i++)
        //            {
        //                lanID = LanIDsList[i];
        //                EmailService.Send(mail =>
        //                {
        //                    mail.From = fromAddress;
        //                    mail.FromDisplayName = fromDisplayName;
        //                    mail.To = lanID + ";";
        //                    mail.Subject = subject;
        //                    mail.BodyText = bodyText;

        //                });
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            _log.Error("Error in Sending Mail: ", ex);
        //        }
        //    }


        //}

        //public static class MyExtensions
        //{
        //    public static Dictionary<string, object> ToDictionary(this object myObj)
        //    {
        //        return myObj.GetType()
        //            .GetProperties()
        //            .Select(pi => new { Name = pi.Name, Value = pi.GetValue(myObj, null) })
        //            .Union(
        //                myObj.GetType()
        //                .GetFields()
        //                .Select(fi => new { Name = fi.Name, Value = fi.GetValue(myObj) })
        //             )
        //            .ToDictionary(ks => ks.Name, vs => vs.Value);
        //    }
    }

    /// <summary>
    /// This class is for interface Summary Execution
    /// </summary>
    public  class Argument
    {
        /// <summary>
        /// This property to get Sucess status of process
        /// </summary>
        public static string Done { get; } = "D;";
        /// <summary>
        /// This property to get failure status of process
        /// </summary>
        public static string Error { get; } = "F;";
        /// <summary>
        /// This property to get interface name 
        /// </summary>
        public static string Interface { get; } = "ED16;";
        /// <summary>
        /// This property to get name of the system to integration
        /// </summary>
        public static string Integration { get; } = "SAP;";
        /// <summary>
        /// This property to get type of interface
        /// </summary>
        public static string Type { get; } = "Outbound;";
    }

    /// <summary>
    /// Enum for processed flag implemented in EDGIS ReArch change detection -v1t8
    /// </summary>
    public class ProcessFlag
    {
        // Processed flag in all interfaces should be updated properly :(P- GIS Processed/E-GIS Error/T-In Transition/F-Failed/D- Done)

        public static string GISProcessed { get; } = "P";
        public static string GISError { get; } = "E";
        public static string InTransition { get; } = "T";
        public static string Failed { get; } = "F";
        public static string Completed { get; } = "D";

    }
}




