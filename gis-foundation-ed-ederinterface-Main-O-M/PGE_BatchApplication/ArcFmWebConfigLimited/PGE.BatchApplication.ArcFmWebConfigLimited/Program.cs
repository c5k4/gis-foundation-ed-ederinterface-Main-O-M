using System;
using System.Configuration;
using System.Reflection;
using PGE.BatchApplication.ArcFmWebConfigLimited;
using ArcFmWebConfigLimited.Core.Enumerator;
using ArcFmWebConfigLimited.Core.Manager;
using ArcFmWebConfigLimited.Core.Output;
using CommandFlags;
using ESRI.ArcGIS.esriSystem;
using Miner.Interop;
using Telvent.Delivery.Diagnostics;

namespace ArcFmWebConfigLimited
{
    public static class Program
    {
        private static readonly LicenseInitializer MAoLicenseInitializer = new LicenseInitializer();
        private static readonly Log4NetLogger Logger = new Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "WebConfigLimited.log4net.config");

        private static string _elecGdbConnection;
        private static string _subGdbConnection;

        public static string Mode = "";
        private static bool _showHelp;

        private static string ElecGdbConnection
        {
            get { return _elecGdbConnection; }
            set
            {
                _elecGdbConnection = value;
                Logger.Info("Using connection " + _elecGdbConnection);
            }
        }

        private static string SubGdbConnection
        {
            get { return _subGdbConnection; }
            set
            {
                _subGdbConnection = value;
                Logger.Info("Using connection " + _subGdbConnection);
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
                if (Mode.Contains("db"))
                {
                    CheckOutLicenses();
                    licensesCheckedOut = true;
                }

                Run();
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

            string elecFile = ConfigurationManager.AppSettings["ElecFilePathAndBase"];
            string subFile = ConfigurationManager.AppSettings["SubFilePathAndBase"];

            Mode = Mode.ToLower();
            switch (Mode)
            {
                case "writedbtocsv":
                    ElecGdbConnection = ConfigurationManager.AppSettings["ElecReadSDEConnection"];
                    manager = new ArcFmSimpleSettingManager(ElecGdbConnection);
                    manager.GdbToCsv(elecFile + ".csv");
                    PropertyEnumerator.ResetCompiledObjectClasses();

                    SubGdbConnection = ConfigurationManager.AppSettings["SubReadSDEConnection"];
                    manager = new ArcFmSimpleSettingManager(SubGdbConnection);
                    manager.GdbToCsv(subFile + ".csv");
                    break;
                case "writedbtoxlsx":
                    ElecGdbConnection = ConfigurationManager.AppSettings["ElecReadSDEConnection"];
                    manager = new ArcFmSimpleSettingManager(ElecGdbConnection);
                    manager.GdbToXlsx(elecFile + ".xlsx");
                    PropertyEnumerator.ResetCompiledObjectClasses();
                    SubGdbConnection = ConfigurationManager.AppSettings["SubReadSDEConnection"];
                    manager = new ArcFmSimpleSettingManager(SubGdbConnection);
                    manager.GdbToXlsx(subFile + ".xlsx");
                    break;
                case "writecsvtodb":
                    ElecGdbConnection = ConfigurationManager.AppSettings["ElecWriteSDEConnection"];
                    manager = new ArcFmSimpleSettingManager(ElecGdbConnection);
                    manager.CsvToGdb(elecFile + ".csv");
                    PropertyEnumerator.ResetCompiledObjectClasses();
                    SubGdbConnection = ConfigurationManager.AppSettings["SubWriteSDEConnection"];
                    manager = new ArcFmSimpleSettingManager(SubGdbConnection);
                    manager.CsvToGdb(subFile + ".csv");
                    break;
                case "writexlsxtodb":
                    ElecGdbConnection = ConfigurationManager.AppSettings["ElecWriteSDEConnection"];
                    manager = new ArcFmSimpleSettingManager(ElecGdbConnection);
                    manager.XlsxToGdb(elecFile + ".xlsx");
                    PropertyEnumerator.ResetCompiledObjectClasses();
                    SubGdbConnection = ConfigurationManager.AppSettings["SubWriteSDEConnection"];
                    manager = new ArcFmSimpleSettingManager(SubGdbConnection);
                    manager.XlsxToGdb(elecFile + ".xlsx");
                    break;
                case "writecsvtoxlsx":
                    SimpleSettingsXlsxConverter.ConvertCsvToXlsx(elecFile + ".csv");
                    SimpleSettingsXlsxConverter.ConvertCsvToXlsx(subFile + ".csv");
                    break;
                case "writexlsxtocsv":
                    SimpleSettingsXlsxConverter.ConvertXlsxToCsv(elecFile + ".xlsx");
                    SimpleSettingsXlsxConverter.ConvertXlsxToCsv(subFile + ".xlsx");
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