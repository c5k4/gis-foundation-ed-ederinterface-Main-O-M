using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ESRI.ArcGIS.esriSystem;
using Miner.Interop;
using System.Configuration;
using System.Text.RegularExpressions;
using PGE_DBPasswordManagement;

namespace PGE.BatchApplication.PGE_Tx_SL_Connectivity_Report
{
    public static class Common
    {
        #region Public variables

        public static esriLicenseProductCode EsriLicense;
        public static mmLicensedProductCode ArcFMLicense;
        public static string SDEConnectionFile = "";
        public static int TransformerClassID = -1;
        public static int ServiceLocationClassID = -1;
        public static List<int> SecondaryClassIDs = new List<int>();
        public static string GeometricNetworkName = "";
        public static string OracleConnection = "";

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
                // m4jf edgisrearch 919 - Update EDER connection string 
                //OracleConnection = ConfigurationManager.AppSettings["OracleConnection"];
                OracleConnection = ConfigurationManager.AppSettings["EDER_ConnectionStr"];
               // SDEConnectionFile = ConfigurationManager.AppSettings["SDEConnectionFile"];
                SDEConnectionFile = ReadEncryption.GetSDEPath(ConfigurationManager.AppSettings["EDER_SDEConnection"].ToUpper());
                TransformerClassID = Int32.Parse(ConfigurationManager.AppSettings["TransformerClassID"]);
                ServiceLocationClassID = Int32.Parse(ConfigurationManager.AppSettings["ServiceLocationClassID"]);
                GeometricNetworkName = ConfigurationManager.AppSettings["GeometricNetwork"].ToUpper();
                string[] secondaryClassIDs = Regex.Split(ConfigurationManager.AppSettings["SecondaryClassIDs"], ";");
                foreach (string classID in secondaryClassIDs)
                {
                    SecondaryClassIDs.Add(Int32.Parse(classID));
                }
            }
            catch (Exception e)
            {
                throw new Exception("Invalid configuration.  Please check the PGE.BatchApplication.PGE_Tx_SL_Connectivity_Report.exe.config file" +
                                            " in the installation directory"); 
            }
        }

    }
}
