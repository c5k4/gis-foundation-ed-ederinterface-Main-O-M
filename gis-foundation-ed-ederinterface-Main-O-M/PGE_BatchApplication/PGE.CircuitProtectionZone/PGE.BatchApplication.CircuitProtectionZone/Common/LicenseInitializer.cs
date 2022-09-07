using System;
using ESRI.ArcGIS;
using Miner.Interop;

namespace PGE.BatchApplication.CircuitProtectionZone.Common
{
    public partial class LicenseInitializer
    {
        // ArcFM Application Initialization object located in Miner.Interop.System.DLL
        private IMMAppInitialize _mmAppInit;

        public LicenseInitializer()
        {
            ResolveBindingEvent += new EventHandler(BindingArcGISRuntime);
        }

        void BindingArcGISRuntime(object sender, EventArgs e)
        {
            //
            // TODO: Modify ArcGIS runtime binding code as needed
            //
            if (!RuntimeManager.Bind(ProductCode.Desktop))
            {
                // Failed to bind, announce and force exit
                Console.WriteLine("Invalid ArcGIS runtime binding. Application will shut down.");
                System.Environment.Exit(0);
            }
        }

        /// <summary>
        /// Check out an ArcFM License by passing in an enumerated value of 
        /// license options to check out.  Gets an ArcFM license if one exists,
        /// attempts to check it out, and throws an exception if one cannot be checked out.
        /// </summary>
        /// <param name="LicenseToCheckOut">Enumerated value of the license to check out</param>
        public bool GetArcFMLicense(mmLicensedProductCode LicenseToCheckOut)
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
                    return false;
                }
                else
                {
                    return true;
                }
            }
            else
            {
                return false;
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