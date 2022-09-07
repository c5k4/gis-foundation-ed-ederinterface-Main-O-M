using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ESRI.ArcGIS.esriSystem;

//using Miner.Interop;


namespace PGE.BatchApplication.ApplyGdbChanges
{
    public class LicenseHandler
    {

        private IAoInitialize m_AoInit;

        private bool m_hasShutDown = false;
        //private IMMAppInitialize _mmAppInit;


        /// <summary>
        /// Check out an ArcFM License by passing in an enumerated value of 
        /// license options to check out.  Gets an ArcFM license if one exists,
        /// attempts to check it out, and throws an exception if one cannot be checked out.
        /// </summary>
        /// <param name="LicenseToCheckOut">Enumerated value of the license to check out</param>
        //public void GetArcFMLicense(mmLicensedProductCode LicenseToCheckOut)
        //{
        //    _mmAppInit = new MMAppInitializeClass();
        //    // check and see if the type of license is available to check out
        //    mmLicenseStatus mmLS = _mmAppInit.IsProductCodeAvailable(LicenseToCheckOut);
        //    if (mmLS == mmLicenseStatus.mmLicenseAvailable)
        //    {
        //        // if the license is available, try to check it out.
        //        mmLicenseStatus arcFMLicenseStatus = _mmAppInit.Initialize(LicenseToCheckOut);

        //        if (arcFMLicenseStatus != mmLicenseStatus.mmLicenseCheckedOut)
        //        {
        //            // if the license cannot be checked out, an exception is raised
        //            throw new Exception("The ArcFM license requested could not be checked out");
        //        }
        //    }
        //}

        ///// <summary>
        ///// Shutdown the ArcFM Application Initializion object
        ///// Should be the last call to make on the object.
        ///// </summary>
        //public void ReleaseArcFMLicense()
        //{
        //    _mmAppInit.Shutdown();
        //}


        public void Bind()
        {
            if (!ESRI.ArcGIS.RuntimeManager.Bind(ESRI.ArcGIS.ProductCode.EngineOrDesktop))
            {
                throw new ApplicationException("Cannot bind to ArcGIS");
            }
        }
        public esriLicenseStatus CheckOut()
        {
            m_AoInit = new AoInitializeClass();
            esriLicenseStatus status = esriLicenseStatus.esriLicenseUnavailable;
            if (m_AoInit.IsProductCodeAvailable(esriLicenseProductCode.esriLicenseProductCodeStandard) == esriLicenseStatus.esriLicenseAvailable)
            {
                status = m_AoInit.Initialize(esriLicenseProductCode.esriLicenseProductCodeStandard);
            }
            if (m_AoInit.IsProductCodeAvailable(esriLicenseProductCode.esriLicenseProductCodeAdvanced) == esriLicenseStatus.esriLicenseAvailable)
            {
                status = m_AoInit.Initialize(esriLicenseProductCode.esriLicenseProductCodeAdvanced);
            }

            return status;
        }
        public void CheckIn()
        {
            try
            {
                if (m_hasShutDown)
                    return;
                m_AoInit.Shutdown();
                m_hasShutDown = true;
            }
            catch { }
        }
    }
}
