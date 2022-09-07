using System;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.DataSourcesGDB;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geodatabase;
using Miner.Interop;
using Comm = PGE.Interfaces.CCBServicePointUpdater.Common;
namespace PGE.Interfaces.CCBServicePointUpdater.Common
{
    class Common
    {
        private static LicenseInitializer m_AOLicenseInitializer = new LicenseInitializer();
        /// <summary>
        /// Initialize the license required to read/edit ArcGIS components
        /// </summary>
        /// <returns></returns>
        public static void InitializeESRILicense()
        {
            try
            {

                Comm.LogManager.WriteLine("Checking out ESRI license...");

                //ESRI License Initializer generated code.
                m_AOLicenseInitializer.InitializeApplication(new esriLicenseProductCode[] { esriLicenseProductCode.esriLicenseProductCodeAdvanced }, new esriLicenseExtensionCode[] { });
            }
            catch (Exception e)
            {
                Comm.LogManager.WriteLine("Unable to checkout ESRI license");
                Comm.LogManager.WriteLine(e.ToString());
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
                Comm.LogManager.WriteLine("Checking out ArcFM license...");
                m_AOLicenseInitializer.GetArcFMLicense(Miner.Interop.mmLicensedProductCode.mmLPArcFM);
            }
            catch (Exception e)
            {
                Comm.LogManager.WriteLine("Unable to checkout ArcFM license");
                Comm.LogManager.WriteLine(e.ToString());
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
            m_AOLicenseInitializer.ShutdownApplication();
        }

        /// <summary>
        /// Opens a workspace given a direct connect string (i.e. sde:oracle11g:/;local=EDGISA1T)
        /// </summary>
        /// <param name="directConnectString"></param>
        /// <returns></returns>
        public static IWorkspace OpenWorkspace(string directConnectString, string user, string pass)
        {
            if (string.IsNullOrEmpty(user) || string.IsNullOrEmpty(pass))
            {
                Comm.LogManager.WriteLine("Unable to determine user name and password.  Please run Telvent.PGE.LoginEncryptor.exe");
                return null;
            }

            // Create and populate the property set to connect to the database.
            ESRI.ArcGIS.esriSystem.IPropertySet propertySet = new ESRI.ArcGIS.esriSystem.PropertySet();
            propertySet.SetProperty("SERVER", "");
            propertySet.SetProperty("INSTANCE", directConnectString);
            propertySet.SetProperty("DATABASE", "");
            propertySet.SetProperty("USER", user);
            propertySet.SetProperty("PASSWORD", pass);
            propertySet.SetProperty("VERSION", "sde.DEFAULT");

            SdeWorkspaceFactory wsFactory = new SdeWorkspaceFactory();
            return wsFactory.Open(propertySet, 0);
        }

        public static IWorkspace OpenWorkspaceFromSDEFile(string sdePath)
        {
            SdeWorkspaceFactory wsFactory = new SdeWorkspaceFactory();
            return wsFactory.OpenFromFile(sdePath, 0);
        }
    }
}
