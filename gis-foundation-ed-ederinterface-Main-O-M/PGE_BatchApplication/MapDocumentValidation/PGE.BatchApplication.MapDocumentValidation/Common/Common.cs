using System;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.DataSourcesGDB;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geodatabase;
using Miner.Interop;

namespace PGE.BatchApplication.MapDocumentValidation.Common
{
    public static class Common
    {
        private static readonly LicenseInitializer MAoLicenseInitializer = new LicenseInitializer();

        /// <summary>
        ///     Initialize the license required to read/edit ArcGIS components
        /// </summary>
        /// <returns></returns>
        public static void InitializeEsriLicense()
        {
            try
            {
                Console.WriteLine("Checking out ESRI license...");

                //ESRI License Initializer generated code.
                MAoLicenseInitializer.InitializeApplication(
                    new[] {esriLicenseProductCode.esriLicenseProductCodeAdvanced}, new esriLicenseExtensionCode[] {});
            }
            catch (Exception e)
            {
                Console.WriteLine("Unable to checkout ESRI license");
                Console.WriteLine(e.Message);
            }
        }

        /// <summary>
        ///     Initializes the license necessary to read/edit ArcFM components
        /// </summary>
        /// <returns></returns>
        public static void InitializeArcFmLicense()
        {
            try
            {
                Console.WriteLine("Checking out ArcFM license...");
                MAoLicenseInitializer.GetArcFMLicense(mmLicensedProductCode.mmLPArcFM);
            }
            catch (Exception e)
            {
                Console.WriteLine("Unable to checkout ArcFM license");
                Console.WriteLine(e.Message);
            }
        }

        /// <summary>
        ///     Closes the license object. The user will not be able to read/edit ArcGIS
        ///     or ArcFM components after a call to this method
        /// </summary>
        /// <returns></returns>
        public static void CloseLicenseObject()
        {
            //ESRI License Initializer generated code.
            //Do not make any call to ArcObjects after ShutDownApplication()
            MAoLicenseInitializer.ShutdownApplication();
        }

        /// <summary>
        ///     Opens a workspace given a direct connect string (i.e. sde:oracle11g:/;local=EDGISA1T)
        /// </summary>
        /// <param name="directConnectString"></param>
        /// <param name="user">User name for connection</param>
        /// <param name="pass">Password for connection</param>
        /// <returns></returns>
        public static IWorkspace OpenWorkspace(string directConnectString, string user, string pass)
        {
            if (string.IsNullOrEmpty(user) || string.IsNullOrEmpty(pass))
            {
                Console.WriteLine(
                    "Unable to determine user name and password.  Please run Telvent.PGE.LoginEncryptor.exe");
                return null;
            }

            // Create and populate the property set to connect to the database.
            IPropertySet propertySet = new PropertySet();
            propertySet.SetProperty("SERVER", "");
            propertySet.SetProperty("INSTANCE", directConnectString);
            propertySet.SetProperty("DATABASE", "");
            propertySet.SetProperty("USER", user);
            propertySet.SetProperty("PASSWORD", pass);
            propertySet.SetProperty("VERSION", "sde.DEFAULT");

            SdeWorkspaceFactory wsFactory = new SdeWorkspaceFactory();
            return wsFactory.Open(propertySet, 0);
        }

        public static IWorkspace OpenWorkspaceFromSdeFile(string sdePath)
        {
            SdeWorkspaceFactory wsFactory = new SdeWorkspaceFactory();

            try
            {
                return wsFactory.OpenFromFile(sdePath, 0);
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        ///     Opens the specified stored display into the IMap
        /// </summary>
        /// <param name="requestedSd">Stored Display Name</param>
        /// <param name="workspace">Workspace containing the stored display</param>
        /// <param name="map">Map to load the stored display into</param>
        /// <returns></returns>
        public static bool OpenStoredDisplay(string requestedSd, IWorkspace workspace, IMap map)
        {
            IMMStoredDisplayManager sdManager = new MMStoredDisplayManager();
            sdManager.Workspace = workspace;

            IMMEnumStoredDisplayName sdNameEnum = sdManager.GetStoredDisplayNames(mmStoredDisplayType.mmSDTSystem);
            sdNameEnum.Reset();
            IMMStoredDisplayName sdName = sdNameEnum.Next();
            while (sdName != null)
            {
                if (sdName.Name.ToUpper() == requestedSd.ToUpper())
                {
                    break;
                }
                sdName = sdNameEnum.Next();
            }

            if (sdName != null)
            {
                IMMStoredDisplay sd = sdManager.GetUnopenedStoredDisplay(sdName);
                sd.Open(map);
                return true;
            }
            return false;
        }
    }
}