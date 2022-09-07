using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using Oracle.DataAccess.Client;
using System.Data;
using ESRI.ArcGIS.esriSystem;
using Miner.Interop;

namespace PGE.BatchApplication.UpdateFeatures
{
    public static class Common
    {
        public static Log4NetLogger _log = new Log4NetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, ReadConfigurations.LOGCONFIG);      
        private static AoInitializeClass m_AoInit = null;
        private static IMMAppInitialize _mmAppInit = null;

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

                Common._log.Info("ESRI License check out successfull.");
            }
            catch (Exception exp)
            {
                Common._log.Error("ESRI License check out failed.");
                Common._log.Info(exp.Message + "   " + exp.StackTrace);
                throw exp;
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
                        Common._log.Error("Arc FM License check out failed.");
                        throw new Exception("The ArcFM license requested could not be checked out");
                    }
                    Common._log.Info("Arc FM License check out successfull.");
                }

            }
            catch (Exception exp)
            {
                Common._log.Error("Arc FM License check out failed.");
                Common._log.Info(exp.Message + "   " + exp.StackTrace);
                throw exp;
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
                if (_mmAppInit != null)
                {
                    _mmAppInit.Shutdown();                
                }

                if (m_AoInit != null)
                {
                    m_AoInit.Shutdown();
                }
            }
            catch (Exception exp)
            {
                // throw;
            }
        }

        public static string CreateOracleConnectionString(string server, string sid, string user, string pass)
        {
            string connection = string.Empty;
            try
            {
                connection = String.Format("Data Source=(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST={0})(PORT=1521)))(CONNECT_DATA=(SERVER=DEDICATED)(SID={1})));", server, sid);
                connection += String.Format("User Id={0};Password={1};", user, pass);
            }
            catch (Exception exp)
            {
                Common._log.Error("Error in creating oracle conn string." + exp.Message);
                Common._log.Info(exp.Message + "   " + exp.StackTrace);
            }
            return connection;
        }
    }
}
