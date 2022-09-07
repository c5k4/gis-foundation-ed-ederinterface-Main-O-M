using System;
using System.Runtime.InteropServices;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.DataSourcesGDB;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geodatabase;
using Miner.Interop;
using PGE.Desktop.EDER.Tests.Common;

namespace PGE.Desktop.Tests.Common
{
    class Common
    {
        private static LicenseInitializer _mAoLicenseInitializer = new LicenseInitializer();

        public static void InitializeLicenses()
        {
            _mAoLicenseInitializer = new LicenseInitializer();
            InitializeESRILicense();
            InitializeArcFMLicense();
        }

        /// <summary>
        /// Initialize the license required to read/edit ArcGIS components
        /// </summary>
        /// <returns></returns>
        public static void InitializeESRILicense()
        {
            try
            {

                Console.WriteLine("Checking out ESRI license...");

                //ESRI License Initializer generated code.
                _mAoLicenseInitializer.InitializeApplication(new esriLicenseProductCode[] { esriLicenseProductCode.esriLicenseProductCodeAdvanced }, new esriLicenseExtensionCode[] { });
            }
            catch (Exception e)
            {
                Console.WriteLine("Unable to checkout ESRI license");
                Console.WriteLine(e.Message);
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
                Console.WriteLine("Checking out ArcFM license...");
                _mAoLicenseInitializer.GetArcFMLicense(Miner.Interop.mmLicensedProductCode.mmLPArcFM);
            }
            catch (Exception e)
            {
                Console.WriteLine("Unable to checkout ArcFM license");
                Console.WriteLine(e.Message);
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
            _mAoLicenseInitializer.ShutdownApplication();
            _mAoLicenseInitializer = null;
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
                Console.WriteLine("Unable to determine user name and password.  Please run PGE.Desktop.LoginEncryptor.exe");
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

            try
            {
                IWorkspace wk = wsFactory.OpenFromFile(sdePath, 0);
                Marshal.FinalReleaseComObject(wsFactory);
                return wk;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        /// <summary>
        /// Opens the specified stored display into the IMap
        /// </summary>
        /// <param name="requestedSD">Stored Display Name</param>
        /// <param name="workspace">Workspace containing the stored display</param>
        /// <param name="map">Map to load the stored display into</param>
        /// <returns></returns>
        public static bool OpenStoredDisplay(string requestedSD, IWorkspace workspace, IMap map)
        {
            IMMStoredDisplayManager sdManager = new MMStoredDisplayManager();
            sdManager.Workspace = workspace;

            IMMEnumStoredDisplayName sdNameEnum = sdManager.GetStoredDisplayNames(mmStoredDisplayType.mmSDTSystem);
            sdNameEnum.Reset();
            IMMStoredDisplayName sdName = sdNameEnum.Next();
            while (sdName != null)
            {
                if (sdName.Name.ToUpper() == requestedSD.ToUpper())
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
            else
            {
                return false;
            }
        }
    }
}
