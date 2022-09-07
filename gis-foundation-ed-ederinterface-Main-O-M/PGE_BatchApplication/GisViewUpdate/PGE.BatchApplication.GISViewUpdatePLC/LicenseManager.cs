using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS;

namespace PGE.BatchApplication.GISViewUpdatePLC
{
    class LicenseManager
    {

                  #region Private Variables

            /// <summary>
            /// ArFM License initializer class
            /// </summary>
          //  private static IMMAppInitialize _ArcFMLicenseInializer;
            /// <summary>
            /// Logger to log error / debug/ user information
            /// </summary>
         //   private static log4net.ILog _logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

            #endregion Private Variables

            #region Public Methods

            /// <summary>
            /// Checks out ArcGIS and ArcFM licenses
            /// </summary>
            /// <returns>Returns true if the licenses are checked out successfully; false otherwise.</returns>
            public static bool ChecketOutLicenses()
            {
                bool checkedOut = CheckoutArcGISLicense();
                return checkedOut;
            }

            /// <summary>
            /// Checks-out ArcGIS license
            /// </summary>
            /// <returns>Returns true if the ArcGIS license is checked-out successfully; false otherwise</returns>
            private static bool CheckoutArcGISLicense()
            {
                bool checkedout = false;
                try
                {
                    //Binds the ArcGIS license to the application
                  // RuntimeManager
                    

                    RuntimeManager.BindLicense(ProductCode.Desktop);
                    checkedout = true;
                }
                catch (Exception ex)
                {
                    //_logger.Error(ex);
                }
                return checkedout;

            }

        

            /// <summary>
            /// Shutdowns the ArcGIS and ArcFM licenses
            /// </summary>
            //public static void Shutdown()
            //{
            //    if (_ArcFMLicenseInializer != null)
            //    {
            //        _ArcFMLicenseInializer.Shutdown();
            //        System.Runtime.InteropServices.Marshal.ReleaseComObject(_ArcFMLicenseInializer);
            //        _ArcFMLicenseInializer = null;
            //    }
            //    ESRI.ArcGIS.ADF.COMSupport.AOUninitialize.Shutdown();
            //}

            #endregion Public Methods

        
    }
}
