using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

using ESRI.ArcGIS.esriSystem;
using Miner.Interop;

namespace PGEReGeoCodingProcess
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {

            if (ESRI.ArcGIS.RuntimeManager.ActiveRuntime == null)
            {
                ESRI.ArcGIS.RuntimeManager.Bind(ESRI.ArcGIS.ProductCode.EngineOrDesktop);
            }
            else
            {
                ESRI.ArcGIS.RuntimeManager.Bind(ESRI.ArcGIS.RuntimeManager.ActiveRuntime.Product);
            }
            //ARCFM License Initializer.
            LicenseCheckout licenseCheckout = new LicenseCheckout();

            try
            {
                //Checking out ESRI license
                licenseCheckout.GetESRILicense(esriLicenseProductCode.esriLicenseProductCodeAdvanced);

                //Checking out Telvent license
                licenseCheckout.GetArcFMLicense(mmLicensedProductCode.mmLPArcFM);
            }
            catch (Exception licExc)
            {
                MessageBox.Show(licExc.Message, "PGEReGeocodeProcess", MessageBoxButtons.OK, MessageBoxIcon.Error);
                licenseCheckout.Dispose();
                Application.Exit();
                return;
            }
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new FormReGeoCoding());
            licenseCheckout.Dispose();

            //Application.EnableVisualStyles();
            //Application.SetCompatibleTextRenderingDefault(false);
            //Application.Run(new FormReGeoCoding());
        }
    }
}
