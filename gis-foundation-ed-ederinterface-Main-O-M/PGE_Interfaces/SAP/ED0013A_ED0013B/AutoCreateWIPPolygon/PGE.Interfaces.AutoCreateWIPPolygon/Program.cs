using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Miner.Interop;
using ESRI.ArcGIS.esriSystem;

namespace PGE.Interfaces.AutoCreateWIPPolygon
{
    class Program
    {
         
        private static Miner.Interop.IMMAppInitialize _appInitialize;
        private static LicenseInitializer m_AOLicenseInitializer = new LicenseInitializer();
        static void Main(string[] args)
        {
            try
            {
                DateTime dtStart = DateTime.Now;
              

                Common._log.Info("Process started.");
                InitializeLicense();
                if ((new MainClass()).StartProcess() == true)
                {
                    Common._log.Info("Process Completed Successfully.");
                }
                else
                {
                    Common._log.Info("Process not Completed Successfully.");
                }
            }
            catch (Exception exp)
            {
                Common._log.Error(exp.Message + " at " + exp.StackTrace);
                ErrorCodeException ece = new ErrorCodeException(exp);
                Environment.ExitCode = ece.CodeNumber;
            }
            finally
            {
                m_AOLicenseInitializer.ShutdownApplication();
              
            }
        }

       

        #region InitializeLicense
        private  static void InitializeLicense()
        {

            Common CommonFunctions = new Common();
            try
            {
                if (!m_AOLicenseInitializer.InitializeApplication(new esriLicenseProductCode[] { esriLicenseProductCode.esriLicenseProductCodeAdvanced, esriLicenseProductCode.esriLicenseProductCodeBasic, esriLicenseProductCode.esriLicenseProductCodeAdvanced },
                 new esriLicenseExtensionCode[] { }))
                {
                    System.Console.WriteLine(m_AOLicenseInitializer.LicenseMessage());
                    System.Console.WriteLine("This application could not initialize with the correct ArcGIS license and will shutdown.");
                    m_AOLicenseInitializer.ShutdownApplication();
                    return;
                }
                
                //Check out Telvent license
                if (CheckOutArcFMLicense(mmLicensedProductCode.mmLPArcFM) != mmLicenseStatus.mmLicenseCheckedOut)
                {
                    Common._log.Error("This application could not initialize with the correct ArcFM license and will shutdown.");
                    return;
                }
                Common._log.Info("License initialized successfully.");
            }
            catch (Exception ex)
            {
                Common._log.Error("Exception occurred while intializing the ESRI/ArcFM license." + ex.Message.ToString());
                throw ex;
            }
        }
        private static mmLicenseStatus CheckOutArcFMLicense(mmLicensedProductCode productCode)
        {
            Common CommonFunctions = new Common();
            try
            {
                if (_appInitialize == null) _appInitialize = new MMAppInitializeClass();

                var licenseStatus = _appInitialize.IsProductCodeAvailable(productCode);
                if (licenseStatus == mmLicenseStatus.mmLicenseAvailable)
                {
                    licenseStatus = _appInitialize.Initialize(productCode);
                }

                return licenseStatus;
            }
            catch (Exception ex)
            {
                Common._log.Error("Exception occurred while Checking out the ArcFM license." + ex.Message.ToString());
                throw ex;
            }
        }
        #endregion
    }
}
