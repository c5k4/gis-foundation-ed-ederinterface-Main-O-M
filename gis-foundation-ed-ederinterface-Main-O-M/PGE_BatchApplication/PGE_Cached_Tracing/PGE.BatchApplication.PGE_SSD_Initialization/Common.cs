using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ESRI.ArcGIS.esriSystem;
using Miner.Interop;
using System.Configuration;
using System.Text.RegularExpressions;
using PGE_DBPasswordManagement;

namespace PGE.BatchApplication.PGE_SSD_Initialization
{
    public static class Common
    {
        #region Public variables

        public static esriLicenseProductCode EsriLicense;
        public static mmLicensedProductCode ArcFMLicense;
        public static string EDSDEConnectionFile = "";
        public static string EDOracleConnection = "";
        public static string ElectricStitchPoint = "";
        public static string DistributionGeomNetwork = "";
        public static int numProcessors = 1;
        public static int CircuitsPerRequest = 1;

        #endregion
        /// <summary>
        /// Reads the application settings into static vars
        /// </summary>
        public static void ReadAppSettings()
        {
            try
            {
                EsriLicense = (esriLicenseProductCode)Enum.Parse(typeof(esriLicenseProductCode), ConfigurationManager.AppSettings["EsriLicense"]);
            }
            catch (Exception e)
            {
                EsriLicense = (esriLicenseProductCode)Enum.Parse(typeof(esriLicenseProductCode), "esriLicenseProductCodeArcInfo");
            }
            try
            {
                ArcFMLicense = (mmLicensedProductCode)Enum.Parse(typeof(mmLicensedProductCode), ConfigurationManager.AppSettings["ArcFMLicense"]);
            }
            catch (Exception e)
            {
                ArcFMLicense = (mmLicensedProductCode)Enum.Parse(typeof(mmLicensedProductCode), "mmLPArcFM");
            }

            try
            {
                // m4jf edgisrearch 99- get connectionstring and sde file using password management tool
             //   EDOracleConnection = ConfigurationManager.AppSettings["EDOracleConnection"];
              //  EDSDEConnectionFile = ConfigurationManager.AppSettings["EDSDEConnectionFile"];

                EDOracleConnection = ConfigurationManager.AppSettings["EDER_ConnectionStr"];
                EDSDEConnectionFile = ReadEncryption.GetSDEPath(ConfigurationManager.AppSettings["EDER_SDEConnection"].ToUpper());



                ElectricStitchPoint = ConfigurationManager.AppSettings["ElectricStitchPoint"];
                DistributionGeomNetwork = ConfigurationManager.AppSettings["DistributionNetwork"].ToUpper();

                try
                {
                    numProcessors = Int32.Parse(ConfigurationManager.AppSettings["NumProcesses"].ToString());
                }
                catch { numProcessors = 1; }

                try
                {
                    CircuitsPerRequest = Int32.Parse(ConfigurationManager.AppSettings["CircuitsPerRequest"].ToString());
                }
                catch { numProcessors = 1; }

            }
            catch (Exception e)
            {
                throw new Exception("Invalid configuration.  Please check the PGE.BatchApplication.PGE_SSD_Initialization.exe.config file" +
                                            " in the installation directory. Message: " + e.Message);
            }
        }

    }
}
