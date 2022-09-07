using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Oracle.DataAccess.Client;
using System.Data;
using ESRI.ArcGIS.esriSystem;
using Miner.Interop;


namespace DeletePrimaryGenerations
{
    public static class Common
    {
        public static Log4NetLogger _log = new Log4NetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public static OracleConnection GetDBConnection(string connectionString)
        {
            string[] oracleConnectionInfo = connectionString.Split(',');
            OracleConnection oraConn = new OracleConnection();
            string oracleConnectionString = "Data Source=(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST=" + oracleConnectionInfo[0] + ")(PORT=1521)))(CONNECT_DATA=(SERVER=DEDICATED)(SERVICE_NAME=" + oracleConnectionInfo[1] + "))); User Id=" + oracleConnectionInfo[2] + ";Password=" + oracleConnectionInfo[3] + ";Pooling=false";
            if ((oraConn != null) && (oraConn.State != ConnectionState.Open))
            {
                try
                {
                    //string oraConnectionString = string.Format("Data Source={0};User Id={1};Password={2};", oracleConnectionInfo[1], oracleConnectionInfo[2], oracleConnectionInfo[3]);
                    oraConn = new OracleConnection(oracleConnectionString);
                    _log.Info("Connecting Database [" + oracleConnectionInfo[1] + "]...");
                    oraConn.Open();
                    _log.Info("Database [" + oracleConnectionInfo[1] + "] connection successful...");
                }
                catch (Exception ex)
                {
                    _log.Error("Error: [" + DateTime.Now.ToLocalTime() + "] Connecting Database [" + oracleConnectionInfo[1] + "] -- " + ex.Message);
                    throw;
                }
            }

            return oraConn;
        }



        private static AoInitializeClass m_AoInit = null;

        //private static LicenseInitializer m_AOLicenseInitializer = new LicenseInitializer();
        /// <summary>
        /// Initialize the license required to read/edit ArcGIS components
        /// </summary>
        /// <returns></returns>
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
            catch (Exception e)
            {
                //Comm.LogManager.WriteLine("Unable to checkout ESRI license");
                //Comm.LogManager.WriteLine(e.ToString());
            }
        }

        /// <summary>
        /// Initializes the license necessary to read/edit ArcFM components
        /// </summary>
        /// <returns></returns>
        public static void InitializeArcFMLicense()
        {
            try
            {
                //Comm.LogManager.WriteLine("Checking out ArcFM license...");
                IMMAppInitialize _mmAppInit = new MMAppInitialize();
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
            catch (Exception e)
            {
                //Comm.LogManager.WriteLine("Unable to checkout ArcFM license");
                //Comm.LogManager.WriteLine(e.ToString());
            }
        }

        /// <summary>
        /// Closes the license object. The user will not be able to read/edit ArcGIS
        /// or ArcFM components after a call to this method
        /// </summary>
        /// <returns></returns>
        public static void CloseLicenseObject()
        {
            //ESRI License Initializer generated code.
            //Do not make any call to ArcObjects after ShutDownApplication()            
            try
            {
                // Write if condition here 
                m_AoInit.Shutdown();
            }
            catch (Exception exp)
            {
                // throw;
            }
        }
    }






}
