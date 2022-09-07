using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ESRI.ArcGIS.esriSystem;

namespace PGE.BatchApplication.ChangeManager.Tests
{
    public class LicenseHandler
    {

        private IAoInitialize m_AoInit;
        
        private bool m_hasShutDown = false;

        public void Bind()
        {
            if (!ESRI.ArcGIS.RuntimeManager.Bind(ESRI.ArcGIS.ProductCode.Engine))
            {
                if (!ESRI.ArcGIS.RuntimeManager.Bind(ESRI.ArcGIS.ProductCode.Desktop))
                {
                    throw new ApplicationException("Cannot bind to ArcGIS");
                }
            }            
        }
        public esriLicenseStatus CheckOut()
        {
            m_AoInit = new AoInitializeClass();
            esriLicenseStatus status = esriLicenseStatus.esriLicenseUnavailable;
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
            catch {}
        }
    }
}
