using System;
using ESRI.ArcGIS.esriSystem;
using Miner.Interop;

namespace PGEElecCleanup
{
    /// <summary>
    /// Objects used to help checkout a particular ESRI or ArcFM License
    /// </summary>
    class LicenseCheckout : IDisposable
    {
        private IAoInitialize _aoInit;
        private IMMAppInitialize _mmAppInit;
        private bool blnESRIlicense = false;
        private bool blnMMlicense = false;

        private bool disposed = false; // Track whether Dispose has been called.

        public LicenseCheckout()
        {
            _aoInit = new AoInitializeClass();
            _mmAppInit = new MMAppInitializeClass();
        }

        /// <summary>
        /// The following code will check out an ESRI License based on a
        /// license enumeration value passed into the function
        /// If a license cannot be checked out, the function will throw an error
        /// </summary>
        /// <param name="LicenseToCheckOut">Enumerated value of an ESRI license to check out</param>
        public void GetESRILicense(esriLicenseProductCode LicenseToCheckOut)
        {
            // check and see if the type of license is available to check out
            esriLicenseStatus ESRILicense = (esriLicenseStatus)_aoInit.IsProductCodeAvailable(LicenseToCheckOut);
            if (ESRILicense == esriLicenseStatus.esriLicenseAvailable)
            {
                // if the license is available, try to check it out.
                ESRILicense = _aoInit.Initialize(LicenseToCheckOut);
                if (ESRILicense == esriLicenseStatus.esriLicenseCheckedOut)
                {
                    blnESRIlicense = true;
                    return;
                }
            }

            throw new Exception("The ESRI license requested could not be checked out");
        }

        /// <summary>
        /// Check out an ArcFM License by passing in an enumerated value of 
        /// license options to check out.  Gets an ArcFM license if one exists,
        /// attempts to check it out, and throws an exception if one cannot be checked out.
        /// </summary>
        /// <param name="LicenseToCheckOut">Enumerated value of the license to check out</param>
        public void GetArcFMLicense(mmLicensedProductCode LicenseToCheckOut)
        {
            // check and see if the type of license is available to check out
            mmLicenseStatus mmLS = _mmAppInit.IsProductCodeAvailable(LicenseToCheckOut);
            if (mmLS == mmLicenseStatus.mmLicenseAvailable)
            {
                // if the license is available, try to check it out.
                mmLicenseStatus arcFMLicenseStatus = _mmAppInit.Initialize(LicenseToCheckOut);

                if (arcFMLicenseStatus == mmLicenseStatus.mmLicenseCheckedOut)
                {
                    blnMMlicense = true;
                    return;
                }
            }

            // if the license cannot be checked out, an exception is raised
            throw new Exception("The ArcFM license requested could not be checked out");
        }

        #region Private Methods

        private void Dispose(bool disposing)
        {
            // Check to see if Dispose has already been called.
            if (!this.disposed)
            {
                // If disposing equals true, dispose all managed and unmanaged resources.

                if (blnESRIlicense) _aoInit.Shutdown();
                if (blnMMlicense) _mmAppInit.Shutdown();

                if (_aoInit != null)
                {
                    _aoInit = null;
                }
                if (_mmAppInit != null)
                {
                    _mmAppInit = null;
                }
            }
            disposed = true;
        }

        ~LicenseCheckout()
        {
            Dispose(false);
        }

        #endregion Private Methods

        #region IDisposable Members

        public void Dispose()
        {
            Dispose(true);
            //Prevent finalization code for this object from executing a second time.
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}
