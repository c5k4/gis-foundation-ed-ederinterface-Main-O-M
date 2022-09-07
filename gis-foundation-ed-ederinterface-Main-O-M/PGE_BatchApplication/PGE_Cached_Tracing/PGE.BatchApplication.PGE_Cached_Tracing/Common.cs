using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ESRI.ArcGIS.esriSystem;
using Miner.Interop;
using System.Configuration;
using System.Text.RegularExpressions;
using PGE_DBPasswordManagement;


namespace PGE.BatchApplication.PGE_Cached_Tracing
{
    public static class Common
    {
        #region Public variables

        public static esriLicenseProductCode EsriLicense;
        public static mmLicensedProductCode ArcFMLicense;
        public static string EDSDEConnectionFile = "";
        public static string EDOracleConnection = "";
        public static string ElectricStitchPoint = "";
        public static string SUBOracleConnection = "";
        public static string SUBSDEConnectionFile = "";
        public static string SUBElectricStitchPoint = "";
        public static string SCHEMOracleConnection = "";
        public static string FromFeatureEIDFieldName = "FROM_FEATURE_EID";
        public static string CircuitIDFieldName = "FEEDERID";
        public static string ToFeatureEIDFieldName = "TO_FEATURE_EID";
        public static string MinBranchFieldName = "MIN_BRANCH";
        public static string MaxBranchFieldName = "MAX_BRANCH";
        public static string FeederFedByFieldName = "FEEDERFEDBY";
        public static string LevelFieldName = "TREELEVEL";
        public static string OrderFieldName = "ORDER_NUM";
        public static string ToFeatureTypeFieldName = "TO_FEATURE_TYPE";
        public static string ToFeatureFCIDFieldName = "TO_FEATURE_FCID";
        public static string ToFeatureOIDFieldName = "TO_FEATURE_OID";
        public static string ToFeatureSCHEMFCIDFieldName = "TO_FEATURE_SCHEM_FCID";
        public static string ToFeatureGlobalID = "TO_FEATURE_GLOBALID";
        public static string ToFeatureFeederInfo = "TO_FEATURE_FEEDERINFO";
        public static string SubstationGeomNetwork = "";
        public static string DistributionGeomNetwork = "";
        public static string UndergroundGeomNetwork = "EDGIS.UndergroundNetwork".ToUpper();
        public static int SubStitchToElecStitchSearchDistance = 5;
        public static int numProcessors = 1;
        public static int CircuitsPerRequest = 1;
        public static int PriUGFCID = 1021;
        public static int SecUGFCID = 1022;
        public static int DCCondFCID = 1020;
        public static int ConduitFCID = 1018;
        public static int SubsurfaceStructureFCID = 1017;
        public static int VaultSubtype = 3;
        public static string ConduitFeatureClass = "EDGIS.CONDUITSYSTEM";
        public static string ConduitPriUGRelTable = "EDGIS.CONDUITSYSTEM_PriUG".ToUpper();
        public static string ConduitPriUGRelUGFieldName = "UGObjectID".ToUpper();
        public static string ConduitPriUGRelConduitFieldName = "ULSObjectID".ToUpper();
        public static string ConduitSecUGRelTable = "EDGIS.CONDUITSYSTEM_SecUG".ToUpper();
        public static string ConduitSecUGRelUGFieldName = "UGObjectID".ToUpper();
        public static string ConduitSecUGRelConduitFieldName = "ULSObjectID".ToUpper();
        public static string ConduitDCCondRelTable = "EDGIS.CONDUITSYSTEM_DCCONDUCTOR".ToUpper();
        public static string ConduitDCCondRelUGFieldName = "UGObjectID".ToUpper();
        public static string ConduitDCCondRelConduitFieldName = "ULSObjectID".ToUpper();

        public static string DistributionTraceResultsTable = "EDGIS.PGE_Trace_Temp".ToUpper();
        public static string FeederFedFeederTraceResultsTable = "EDGIS.PGE_FeederFedNetwork_Trace".ToUpper();
        public static string UndergroundTraceResultsTable = "EDGIS.PGE_UndergroundNetwork_Temp".ToUpper();
        public static string SubstationTraceResultsTable = "EDGIS.PGE_Trace_Temp".ToUpper();
        public static List<string> EDBeforeTracingStoredProcedures = new List<string>();
        public static List<string> EDAfterTracingStoredProcedures = new List<string>();
        public static List<string> SUBBeforeTracingStoredProcedures = new List<string>();
        public static List<string> SUBAfterTracingStoredProcedures = new List<string>();

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
                // m4jf edgisrearch 919 - get sde connection file and connection string using Passwordmanagement tool
               // EDOracleConnection = ConfigurationManager.AppSettings["EDOracleConnection"];
                //EDSDEConnectionFile = ConfigurationManager.AppSettings["EDSDEConnectionFile"];

                EDOracleConnection = ConfigurationManager.AppSettings["EDER_ConnectionStr"];
                EDSDEConnectionFile = ReadEncryption.GetSDEPath(ConfigurationManager.AppSettings["EDER_SDEConnection"].ToUpper());



                ElectricStitchPoint = ConfigurationManager.AppSettings["ElectricStitchPoint"];
                DistributionGeomNetwork = ConfigurationManager.AppSettings["DistributionNetwork"].ToUpper();

                // m4jf edgisrearch 919 - get sde connection file and connection string using Passwordmanagement tool
               // SUBSDEConnectionFile = ConfigurationManager.AppSettings["SUBSDEConnectionFile"];
                SUBSDEConnectionFile = ReadEncryption.GetSDEPath(ConfigurationManager.AppSettings["EDERSUB_SDEConnection"].ToUpper());

                SUBElectricStitchPoint = ConfigurationManager.AppSettings["SubElectricStitchPoint"];
                // SUBOracleConnection = ConfigurationManager.AppSettings["SUBOracleConnection"];
                SUBOracleConnection = ConfigurationManager.AppSettings["EDERSUB_ConnectionStr"];


                SubstationGeomNetwork = ConfigurationManager.AppSettings["SubstationNetwork"].ToUpper();
                // m4jf edgisrearch 919 - get sde connection file and connection string using Passwordmanagement tool
                //SCHEMOracleConnection = ConfigurationManager.AppSettings["SCHEMOracleConnection"];
                SCHEMOracleConnection = ConfigurationManager.AppSettings["EDSCHM_ConnectionStr"];


                string[] AfterStoredProcs = Regex.Split(ConfigurationManager.AppSettings["EDAfterTracingStoredProcedures"].ToUpper(), ",");
                EDAfterTracingStoredProcedures = AfterStoredProcs.ToList();
                string[] BeforeStoredProcs = Regex.Split(ConfigurationManager.AppSettings["EDBeforeTracingStoredProcedures"].ToUpper(), ",");
                EDBeforeTracingStoredProcedures = BeforeStoredProcs.ToList();

                AfterStoredProcs = Regex.Split(ConfigurationManager.AppSettings["SUBAfterTracingStoredProcedures"].ToUpper(), ",");
                SUBAfterTracingStoredProcedures = AfterStoredProcs.ToList();

                BeforeStoredProcs = Regex.Split(ConfigurationManager.AppSettings["SUBBeforeTracingStoredProcedures"].ToUpper(), ",");
                SUBBeforeTracingStoredProcedures = BeforeStoredProcs.ToList();

                try
                {
                    SubStitchToElecStitchSearchDistance = Int32.Parse(ConfigurationManager.AppSettings["SubStitchToElecStitchSearchDistance"].ToUpper());
                }
                catch { SubStitchToElecStitchSearchDistance = 5; }

                try
                {
                    numProcessors = Int32.Parse(ConfigurationManager.AppSettings["NumProcesses"].ToString());
                }
                catch { numProcessors = 1; }

                try
                {
                    UndergroundGeomNetwork = ConfigurationManager.AppSettings["UndergroundGeomNetwork"].ToUpper();
                }
                catch { UndergroundGeomNetwork = "EDGIS.UndergroundNetwork".ToUpper(); }

                SubsurfaceStructureFCID = 1017;
                VaultSubtype = 3;

                try
                {
                    SubsurfaceStructureFCID = Int32.Parse(ConfigurationManager.AppSettings["SubsurfaceStructureFCID"].ToUpper());
                }
                catch { SubsurfaceStructureFCID = 1017; }

                try
                {
                    VaultSubtype = Int32.Parse(ConfigurationManager.AppSettings["VaultSubtype"].ToUpper());
                }
                catch { VaultSubtype = 3; }

                try
                {
                    ConduitPriUGRelTable = ConfigurationManager.AppSettings["ConduitPriUGRelTable"].ToUpper();
                }
                catch { ConduitPriUGRelTable = "EDGIS.CONDUITSYSTEM_PriUG".ToUpper(); }

                try
                {
                    ConduitFeatureClass = ConfigurationManager.AppSettings["ConduitFeatureClass"].ToUpper();
                }
                catch { ConduitFeatureClass = "EDGIS.CONDUITSYSTEM".ToUpper(); }

                try
                {
                    ConduitSecUGRelTable = ConfigurationManager.AppSettings["ConduitSecUGRelTable"].ToUpper();
                }
                catch { ConduitSecUGRelTable = "EDGIS.CONDUITSYSTEM_SecUG".ToUpper(); }

                try
                {
                    ConduitDCCondRelTable = ConfigurationManager.AppSettings["ConduitDCCondRelTable"].ToUpper();
                }
                catch { ConduitDCCondRelTable = "EDGIS.CONDUITSYSTEM_DCCONDUCTOR".ToUpper(); }
                
                try
                {
                    ConduitPriUGRelUGFieldName = ConfigurationManager.AppSettings["ConduitPriUGRelUGFieldName"].ToUpper();
                }
                catch { ConduitPriUGRelUGFieldName = "UGObjectID".ToUpper(); }
                try
                {
                    ConduitPriUGRelConduitFieldName = ConfigurationManager.AppSettings["ConduitPriUGRelConduitFieldName"].ToUpper();
                }
                catch { ConduitPriUGRelConduitFieldName = "ULSObjectID".ToUpper(); }

                try
                {
                    ConduitSecUGRelUGFieldName = ConfigurationManager.AppSettings["ConduitSecUGRelUGFieldName"].ToUpper();
                }
                catch { ConduitSecUGRelUGFieldName = "UGObjectID".ToUpper(); }
                try
                {
                    ConduitSecUGRelConduitFieldName = ConfigurationManager.AppSettings["ConduitSecUGRelConduitFieldName"].ToUpper();
                }
                catch { ConduitSecUGRelConduitFieldName = "ULSObjectID".ToUpper(); }

                try
                {
                    ConduitDCCondRelUGFieldName = ConfigurationManager.AppSettings["ConduitDCCondRelUGFieldName"].ToUpper();
                }
                catch { ConduitDCCondRelUGFieldName = "UGObjectID".ToUpper(); }
                try
                {
                    ConduitDCCondRelConduitFieldName = ConfigurationManager.AppSettings["ConduitDCCondRelConduitFieldName"].ToUpper();
                }
                catch { ConduitDCCondRelConduitFieldName = "ULSObjectID".ToUpper(); }

                try
                {
                    ConduitFCID = Int32.Parse(ConfigurationManager.AppSettings["ConduitFCID"].ToUpper());
                }
                catch { ConduitFCID = 1018; }

                try
                {
                    PriUGFCID = Int32.Parse(ConfigurationManager.AppSettings["PriUGFCID"].ToUpper());
                }
                catch { PriUGFCID = 1021; }

                try
                {
                    SecUGFCID = Int32.Parse(ConfigurationManager.AppSettings["SecUGFCID"].ToUpper());
                }
                catch { SecUGFCID = 1022; }

                try
                {
                    DCCondFCID = Int32.Parse(ConfigurationManager.AppSettings["DCCondFCID"].ToUpper());
                }
                catch { DCCondFCID = 1020; }

                try
                {
                    CircuitsPerRequest = Int32.Parse(ConfigurationManager.AppSettings["CircuitsPerRequest"].ToString());
                }
                catch { numProcessors = 1; }

            }
            catch (Exception e)
            {
                throw new Exception("Invalid configuration.  Please check the PGE.BatchApplication.PGE_Cached_Tracing.exe.config file" +
                                            " in the installation directory. Message: " + e.Message);
            }
        }

    }
}
