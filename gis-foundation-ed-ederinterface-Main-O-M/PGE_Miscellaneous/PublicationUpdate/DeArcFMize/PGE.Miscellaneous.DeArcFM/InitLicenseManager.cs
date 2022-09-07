// ========================================================================
// Copyright © 2021 PGE.
// <history>
// InitLicenseManager Class for License related functions functions
// TCS V3SF (EDGISREARC-767) 04/20/2021               Created
// </history>
// All rights reserved.
// ========================================================================
using System;
using System.Collections.Generic;
using System.Text;
using ESRI.ArcGIS.esriSystem;
using Miner.Interop;
//using Telvent.Delivery.Diagnostics;
using System.Diagnostics;

namespace PGE.Miscellaneous.DeArcFM
{
    /// <summary>
    /// Represents a class too check out the ArcLicenses necessary to run applications outside of Miner and Miner and ESRI products for ArcGIS and ArcFM version prior to 10
    /// </summary>
    internal sealed class InitLicenseManager
    {
        private IAoInitialize _AoInitialize;
        private IMMAppInitialize _MmAppInit;

        private mmLicensedProductCode _MmLicense;
        private esriLicenseProductCode _EsriLicense;

        /// <summary>
        /// Initializes a new instance of the <see cref="LicenseManager"/> class.
        /// </summary>
        public InitLicenseManager()
        {

        }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="LicenseManager"/> class.
        /// </summary>
        /// <param name="esri">The esri.</param>
        /// <param name="arcfm">The arcfm.</param>
        public InitLicenseManager(esriLicenseProductCode esri, mmLicensedProductCode arcfm)
        {
            this.Initialize(esri, arcfm);
        }

        #region Public Members
        /// <summary>
        /// Checkout the given esri and mm product licenses.
        /// </summary>
        /// <param name="esri">The esri </param>
        /// <param name="arcfm">The arcfm </param>
        /// <returns>Boolean</returns>
        public bool Initialize(string esri, string arcfm)
        {
            return Initialize(ToESRI(esri), ToArcFM(arcfm));
        }
        
        /// <summary>
        /// Checkout the given esri and mm product licenses.
        /// </summary>
        /// <param name="esri">esriLicenseProductCode</param>
        /// <param name="arcfm">mmLicensedProductCode</param>
        /// <returns>
        /// True if both licenses are checked out; false otherwise
        /// </returns>
        public bool Initialize(esriLicenseProductCode esri, mmLicensedProductCode arcfm)
        {
            return (Initialize(esri) && Initialize(arcfm));
        }

        /// <summary>        
        /// Attempts to checkout a license for the specified esri product.
        /// </summary>
        /// <param name="prodCode">esriLicenseProductCode to check out.</param>
        /// <returns>True if successful; false otherwise.</returns>  
        public bool Initialize(esriLicenseProductCode prodCode)
        {
            try
            {
                MMEsriBindClass esriBindClass = new MMEsriBindClass();
                if (esriBindClass.AutoBind())
                {
                    // Create a new AoInitialize object
                    _AoInitialize = new AoInitializeClass();

                    if (_AoInitialize == null)
                    {
                        //EventLogger.Warn("Warning! The ArcGIS License failed to initialize the AoInitialize. License cannot be checked out.");
                        return false;
                    }

                    // Determine if the product is available
                    esriLicenseStatus licenseStatus = _AoInitialize.IsProductCodeAvailable(prodCode);
                    if (licenseStatus == esriLicenseStatus.esriLicenseAvailable)
                    {
                        licenseStatus = _AoInitialize.Initialize(prodCode);
                        if (licenseStatus != esriLicenseStatus.esriLicenseCheckedOut)
                        {
                            //EventLogger.Warn("Warning! The ArcGIS license cannot be checked out for " + prodCode + ".");
                        }
                        else
                        {
                            _EsriLicense = prodCode;
                            //EventLogger.Log("Checking out ArcGIS License: " + prodCode, EventLogEntryType.Information);
                            return true;
                        }
                    }
                    else
                    {
                        //Trace.WriteLine("Warning! The ArcGIS product " + prodCode + " is unavailable!");
                        //EventLogger.Warn("Warning! The ArcGIS product " + prodCode + " is unavailable!");
                    }
                }
            }
            catch (Exception Ex)
            {
                EventLog.WriteEntry("Error: ", Ex.Message);
            }

            return false;
        }

        /// <summary>        
        /// Attempts to checkout a license for the specified Miner and Miner product.
        /// </summary>
        /// <param name="prodCode">mmLicenseProductCode to check out.</param>
        /// <returns>True if successfull; false otherwise.</returns>    
        public bool Initialize(mmLicensedProductCode prodCode)
        {
            _MmAppInit = new MMAppInitializeClass();

            if (_MmAppInit == null)
            {
                //EventLogger.Warn("Warning! Unable to initialize ArcFM.  No licenses can be checked out.");
                return false;
            }

            // Determine if the product license is available or is already checked out
            mmLicenseStatus mmlicenseStatus = _MmAppInit.IsProductCodeAvailable(prodCode);
            if (mmlicenseStatus == mmLicenseStatus.mmLicenseCheckedOut)
            {
                return true;
            }

            if (mmlicenseStatus == mmLicenseStatus.mmLicenseAvailable)
            {
                mmlicenseStatus = _MmAppInit.Initialize(prodCode);
                if (mmlicenseStatus != mmLicenseStatus.mmLicenseCheckedOut)
                {
                    //Trace.WriteLine("Warning! A license cannot be checked out for M&M product " + prodCode);
                    //EventLogger.Warn("Warning! A license cannot be checked out for M&M product " + prodCode); 
                    return false;
                }

                _MmLicense = prodCode;
                //EventLogger.Log("Checking out ArcFM License: " + prodCode, EventLogEntryType.Information);

                return true;
            }

            //EventLogger.Warn("Warning! No license is available for M&M product " + prodCode); 

            return false;
        }

        /// <summary>
        /// Checks in all ArcGIS and ArcFM licenses that have been checked out.
        /// </summary>
        public void Shutdown()
        {
            Dispose();
        }
        #endregion

        #region Static Members
        /// <summary>
        /// Converts the string version of the arc fm product code into the enumeration.
        /// </summary>
        /// <param name="arcfm">The string version of the mmLicenseProductCode either with or without the mmLP.</param>
        /// <returns>mmLicensedProductCode converted; mmLPArcFM otherwise.</returns>
        public static mmLicensedProductCode ToArcFM(string arcfm)
        {
            foreach (string prod in Enum.GetNames(typeof(mmLicensedProductCode)))
            {
                if (string.Compare(prod, arcfm, true) == 0)
                    return (mmLicensedProductCode)Enum.Parse(typeof(mmLicensedProductCode), prod);

                string value = prod.Replace("mmLP", "");
                if (string.Compare(value, arcfm, true) == 0)
                    return (mmLicensedProductCode)Enum.Parse(typeof(mmLicensedProductCode), prod);
            }

            return mmLicensedProductCode.mmLPArcFM;
        }

        /// <summary>
        /// Converts the string version of the esri product code into the enumeration.
        /// </summary>
        /// <param name="esri">The string version of the esriLicenseProductCode either with or without the esriLicenseProductCode.</param>
        /// <returns>esriLicenseProductCode converted; esriLicenseProductCodeArcInfo otherwise.</returns>
        public static esriLicenseProductCode ToESRI(string esri)
        {
            foreach (string prod in Enum.GetNames(typeof(esriLicenseProductCode)))
            {
                if (string.Compare(prod, esri, true) == 0)
                    return (esriLicenseProductCode)Enum.Parse(typeof(esriLicenseProductCode), prod);

                string value = prod.Replace("esriLicenseProductCode", "");
                if (string.Compare(value, esri, true) == 0)
                    return (esriLicenseProductCode)Enum.Parse(typeof(esriLicenseProductCode), prod);
            }

            return esriLicenseProductCode.esriLicenseProductCodeAdvanced;
        }
        #endregion

        #region IDisposable Members

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            try
            {
                if (_MmAppInit != null) _MmAppInit.Shutdown();
                if (_AoInitialize != null) _AoInitialize.Shutdown();

                //if ((int)_MmLicense != 0) EventLogger.Log("Checking in ArcFM License: " + _MmLicense, EventLogEntryType.Information); 
                //if ((int)_EsriLicense != 0) EventLogger.Log("Checking in ArcGIS License: " + _EsriLicense, EventLogEntryType.Information);

                // Release COM objects and shut down the AoInitilaize object
                //AOUninitialize.Shutdown();
            }
            catch
            {

            }
        }

        #endregion
    }
}
