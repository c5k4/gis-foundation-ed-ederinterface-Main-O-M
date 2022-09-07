using System;
using System.Diagnostics;
using ESRI.ArcGIS;
using Miner.Interop;

namespace PGE.BatchApplication.AUConveyor.Utilities
{
    /// <summary>
    /// Contains logic to initialize ArcGIS and ArcFM licenses.
    /// </summary>
    internal partial class LicenseInitializer
    {
        // ArcFM Application Initialization object located in Miner.Interop.System.DLL
        private IMMAppInitialize _mmAppInit;

        public LicenseInitializer()
        {
            ResolveBindingEvent += new EventHandler(BindingArcGISRuntime);
        }



        void BindingArcGISRuntime(object sender, EventArgs e)
        {
            ProductCode[] supportedRuntimes = new ProductCode[] { 
            ProductCode.Engine, ProductCode.Desktop };
            foreach (ProductCode c in supportedRuntimes)
            {
            if (RuntimeManager.Bind(c))
                return;
            }

            // Failed to bind, announce and force exit
            LogManager.WriteLine("ArcGIS runtime binding failed. Application will shut down.");
            System.Environment.Exit(0);
        }

        /// <summary>
        /// Check out an ArcFM License by passing in an enumerated value of 
        /// license options to check out.  Gets an ArcFM license if one exists,
        /// attempts to check it out, and throws an exception if one cannot be checked out.
        /// </summary>
        /// <param name="LicenseToCheckOut">Enumerated value of the license to check out</param>
        public void GetArcFMLicense(mmLicensedProductCode LicenseToCheckOut)
        {
            _mmAppInit = new MMAppInitializeClass();
            // check and see if the type of license is available to check out
            mmLicenseStatus mmLS = _mmAppInit.IsProductCodeAvailable(LicenseToCheckOut);
            if (mmLS == mmLicenseStatus.mmLicenseAvailable)
            {
                // if the license is available, try to check it out.
                mmLicenseStatus arcFMLicenseStatus = _mmAppInit.Initialize(LicenseToCheckOut);

                if (arcFMLicenseStatus != mmLicenseStatus.mmLicenseCheckedOut)
                {
                    // if the license cannot be checked out, an exception is raised
                    throw new Exception("The ArcFM license requested could not be checked out");
                }
            }
        }

        /// <summary>
        /// Shutdown the ArcFM Application Initializion object
        /// Should be the last call to make on the object.
        /// </summary>
        public void ReleaseArcFMLicense()
        {
            _mmAppInit.Shutdown();
        }
    }
}