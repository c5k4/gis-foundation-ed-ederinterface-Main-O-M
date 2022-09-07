using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Runtime.InteropServices;

using PGE.Interfaces.TaxReportDataStaging;

using ESRI.ArcGIS;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geodatabase;
using System.Text.RegularExpressions;
using System.Reflection;
using System.Configuration;

namespace PGE.Interfaces.ProcessTaxData
{
    class Program
    {
        
        private static esriLicenseProductCode[] _prodCodes = { esriLicenseProductCode.esriLicenseProductCodeAdvanced };
        private static LicenseInitializer _licenceManager = new LicenseInitializer();
        //private static string _featureClassName = string.Empty;

        [MTAThread]
        static void Main(string[] args)
        {
            try
            {
                try
                {
                    List<string> IDs = new List<string>();
                    IDs = Regex.Split(args[0], ",").ToList();
                    List<string> circuitIDs = new List<string>();
                    foreach (string id in IDs)
                    {
                        circuitIDs.Add(id);
                    }

                    try { Int32.Parse(ConfigurationManager.AppSettings["OracleTimeout"]); }
                    catch { Common.OracleTimeout = 360; }

                    //_featureClassName = args[0];

                    IWorkspace ws = null;
                    //Console.WriteLine("Current Processor Affinity: {0}", Process.GetCurrentProcess().ProcessorAffinity + " for " + _featureClassName + " ...");
                    //Console.WriteLine("Checking license to process " + args[0] + " ...");
                    if (CheckLicense(_licenceManager))
                    {
                        //Console.WriteLine("License... OK");
                        try
                        {
                            bool executeED08Structure = false;
                            bool executeED08MapGrids = false;

                            string arg1 = "";
                            string arg2 = "";

                            if (args.Count() > 1)
                            {
                                arg1 = args[1];
                            }
                            if (args.Count() > 2)
                            {
                                arg2 = args[2];
                            }

                            if (arg1 == "-s" || arg2 == "-s") { executeED08Structure = true; }
                            if (arg1 == "-g" || arg2 == "-g") { executeED08MapGrids = true; }

                            if (!executeED08MapGrids && !executeED08Structure)
                            {
                                Console.WriteLine("Invalid arguments specified");
                                Console.WriteLine("-s: Execute spatial analysis for support structures");
                                Console.WriteLine("-g: Execute spatial analysis for map grids");
                                Common.WriteToLog("Invalid arguments specified" + DateTime.Now.ToLocalTime(), LoggingLevel.Info);
                                Common.WriteToLog("-s: Execute spatial analysis for support structures" + DateTime.Now.ToLocalTime(), LoggingLevel.Info);
                                Common.WriteToLog("-g: Execute spatial analysis for map grids" + DateTime.Now.ToLocalTime(), LoggingLevel.Info);
                                Environment.Exit(1);
                            }

                            ws = Common.SetWorkspace();
                            TaxReportDataStagingClass tc = new TaxReportDataStagingClass();
                            tc.WorkSpace = ws;
                            tc.Process(circuitIDs, args[0], executeED08Structure, executeED08MapGrids);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("Error: [" + DateTime.Now.ToLocalTime() + "] in PGE.Interfaces.ProcessTaxData for Circuit IDs: " + args[0] + ".", LoggingLevel.Error);
                            Common.WriteToLog("Error: [" + DateTime.Now.ToLocalTime() + "] PGE.Interfaces.ProcessTaxData -- Inside tc.Process for Circuit IDs: " + args[0] + ".", LoggingLevel.Error);
                            throw ex;
                        }
                        finally
                        {
                            if (ws != null) { while (Marshal.ReleaseComObject(ws) > 0);}
                            ws = null;
                        }

                    }
                    else
                    {
                        Console.WriteLine("Error: [" + DateTime.Now.ToLocalTime() + "] in PGE.Interfaces.ProcessTaxData for Circuit IDs: " + args[0] + ". Unable to check out license.", LoggingLevel.Error);
                        Common.WriteToLog("Error: [" + DateTime.Now.ToLocalTime() + "] PGE.Interfaces.ProcessTaxData -- Inside tc.Process for Circuit IDs: " + args[0] + ". Unable to check out license.", LoggingLevel.Error);
                        Environment.Exit(1);
                    }

                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: [" + DateTime.Now.ToLocalTime() + "] in PGE.Interfaces.ProcessTaxData for Circuit IDs: " + args[0] + ".");
                    Common.WriteToLog("Error: [" + DateTime.Now.ToLocalTime() + "] in PGE.Interfaces.ProcessTaxData for Circuit IDs: " + args[0] + ".", LoggingLevel.Error);
                    Environment.Exit(1);
                }

                Environment.Exit(0);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: PGE.Interfaces.ProcessTaxData -> " + ex.Message);
                Common.WriteToLog("Error: PGE.Interfaces.ProcessTaxData -> " + ex.Message + DateTime.Now.ToLocalTime(), LoggingLevel.Info);
                Environment.Exit(1);
            }
            finally
            {
                _licenceManager.ShutdownApplication();
            }
        }

        /// <summary>
        /// Initializes ArcGIS license
        /// </summary>
        /// <param name="licenceManager"></param>
        /// <returns></returns>
        private static bool CheckLicense(LicenseInitializer licenceManager)
        {
            bool isOk = false;

            try
            {
                isOk = licenceManager.InitializeApplication(_prodCodes);
                isOk = licenceManager.GetArcFMLicense(Miner.Interop.mmLicensedProductCode.mmLPArcFM);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Failed to initialize license : " + ex.Message);
                Common.WriteToLog("Failed to initialize license : " + ex.Message, LoggingLevel.Error);
            }

            return isOk;
        }
    }
}
