using System;
using System.Configuration;
using System.Reflection;
using PGE.BatchApplication.ApplyArcFMProperties;
using ESRI.ArcGIS.esriSystem;
using Miner.Interop;
using PGE.Common.CommandFlags;
using PGE.Common.Delivery.Diagnostics;
using ArcFmWebConfigLimited.Core.Manager;
using ArcFmWebConfigLimited.Core.Output;

namespace ArcFmWebConfigLimited
{
    public static class Program
    {
        private static readonly LicenseInitializer MAoLicenseInitializer = new LicenseInitializer();
        private static readonly Log4NetLogger Logger = new Log4NetLogger("WebConfigMain", "webconfigmain");

        private static string _gdbConnection;

        public static string Mode = "";
        private static bool _showHelp;

        private static string GdbConnection
        {
            get { return _gdbConnection; }
            set
            {
                _gdbConnection = value;
                Logger.Info("Using connection " + _gdbConnection);
            }
        }

        private static void Main(string[] args)
        {
            DateTime exeStart = DateTime.Now;
            bool licensesCheckedOut = false;
            try
            {
                ProcessArguments(args);

                Mode = Mode.ToLower();
                if (Mode.Equals("writedbtocsv") || Mode.Equals("writedbtoxlsx") || Mode.Equals("writexlsxtodb") ||
                    Mode.Equals("writecsvtodb"))
                {
                    CheckOutLicenses();
                    licensesCheckedOut = true;
                    Run();
                }
                else if (Mode.Equals("writecsvtoxlsx") || Mode.Equals("writexlsxtocsv"))
                {
                    Run();
                }
            }
            catch (Exception ex)
            {
                Logger.Fatal("Error [ " + ex + " ]");
                Environment.ExitCode = 1;
            }
            finally
            {
                try
                {
                    //ESRI License Initializer generated code.
                    if (licensesCheckedOut)
                        CheckInLicenses();
                }
                catch (Exception e)
                {
                    Logger.Error(e.Message);
                    Logger.Error(e.StackTrace);
                }

                Logger.Debug(MethodBase.GetCurrentMethod().Name);

                //Report end time.
                DateTime exeEnd = DateTime.Now;
                Logger.Info("");
                Logger.Info("Completed");
                Logger.Info("Process started: " + exeStart.ToLongDateString() + " " + exeStart.ToLongTimeString());
                Logger.Info("Process ended: " + exeEnd.ToLongDateString() + " " + exeEnd.ToLongTimeString());
                Logger.Info("Process length: " + (exeEnd - exeStart));
            }
        }

        public static void CheckOutLicenses()
        {
            bool licenseCheckoutSuccess =
                MAoLicenseInitializer.InitializeApplication(
                    new[] {esriLicenseProductCode.esriLicenseProductCodeAdvanced},
                    new esriLicenseExtensionCode[] {});
            //ESRI License Initializer generated code.
            if (licenseCheckoutSuccess)
            {
                try
                {
                    MAoLicenseInitializer.GetArcFMLicense(mmLicensedProductCode.mmLPArcFM);
                }
                catch (Exception)
                {
                    Logger.Error("Unable to checkout ArcFM license");
                    throw;
                }
            }
            else
            {
                throw new Exception("can't get arcgis license");
            }
        }

        public static void CheckInLicenses()
        {
            MAoLicenseInitializer.ReleaseArcFMLicense();
            MAoLicenseInitializer.ShutdownApplication();
        }


        private static void Run()
        {
            Logger.Info(MethodBase.GetCurrentMethod().Name);

            ArcFmGeneralPropertyManager manager;

            string file = ConfigurationManager.AppSettings["FilePathAndBase"];

            Mode = Mode.ToLower();
            switch (Mode)
            {
                case "writedbtocsv":
                    GdbConnection = ConfigurationManager.AppSettings["ReadSDEConnection"];
                    manager = new ArcFmSimpleSettingManager(GdbConnection);
                    manager.GdbToCsv(file + ".csv");
                    break;
                case "writedbtoxlsx":
                    GdbConnection = ConfigurationManager.AppSettings["ReadSDEConnection"];
                    manager = new ArcFmSimpleSettingManager(GdbConnection);
                    manager.GdbToXlsx(file + ".xlsx");
                    break;
                case "writecsvtodb":
                    GdbConnection = ConfigurationManager.AppSettings["WriteSDEConnection"];
                    manager = new ArcFmSimpleSettingManager(GdbConnection);
                    manager.CsvToGdb(file + ".csv");
                    break;
                case "writexlsxtodb":
                    GdbConnection = ConfigurationManager.AppSettings["WriteSDEConnection"];
                    manager = new ArcFmSimpleSettingManager(GdbConnection);
                    manager.XlsxToGdb(file + ".xlsx");
                    break;
                case "writecsvtoxlsx":
                    SimpleSettingsXlsxConverter.ConvertCsvToXlsx(file + ".csv");
                    break;
                case "writexlsxtocsv":
                    SimpleSettingsXlsxConverter.ConvertXlsxToCsv(file + ".xlsx");
                    break;
                default:
                    Logger.Error("Invalid operation. No operations executed.");
                    Environment.ExitCode = 1;
                    break;
            }
        }

        private static void ProcessArguments(string[] args)
        {
            Logger.Info(MethodBase.GetCurrentMethod().Name);

            // Parse Flags
            var flags = new FlagParser
            {
                {"h", "help", false, false, "Show usage descriptions.", f => _showHelp = f != null},
                {"o", "operation", true, false, "The operation to perform", f => Mode = f},
            };
            flags.Parse(args);

            if (_showHelp)
            {
                Logger.Warn("Usage: \nArcFmWebConfigLimited.exe -o " +
                            "<WriteDbToCsv|WriteDbToXlsx|WriteCsvToDb|WriteXlsxToDb|WriteCsvToXlsx|WriteXlsxToCsv>" +
                            "\nUse one of the provided commands for the -o option" +
                            "\nArcFmWebConfigLimited.exe -h" +
                            "\nProvides help with command usage\n");

                Logger.Warn("Ex: " +
                            "\nArcFmWebConfigLimited.exe -o WriteDbToCsv");
                Logger.Warn(
                    "When dealing with indices, you must provide all subtypes and all fields of a feature class" +
                    " for the application to re-order fields of a feature class. I.E., you may delete all unnecessary rows but" +
                    " be sure to keep entire feature classes if re-ordering.\n");

                Logger.Warn(
                    "Changing the properties for the '-1' subtype code of an object class with subtypes will not apply " +
                    "changes to all subtypes of that feature class. You must set the property for each subtype to accomplish this.\n");

                Environment.Exit(0);
            }
        }
    }
}