using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
 using ESRI.ArcGIS.Geodatabase;
using PGE.BatchApplication.MapDocumentValidation.MapDocument;
using PGE.Common.CommandFlags;
using PGE.Common.Delivery.Diagnostics;
using PGE_DBPasswordManagement;

namespace PGE.BatchApplication.MapDocumentValidation
{
    internal static class Client
    {
        private const string MxdExt = ".mxd";
        private static readonly Log4NetLogger Logger = new Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "MapDocValidation.log4net.config");

        private static string _mapDoc1 = "", _mapDoc2 = "", _sdeFilePath = "", _operation = "";
        private static bool _showHelp;

        private static void Main(string[] args)
        {
            DateTime exeStart = DateTime.Now;

            try
            {
                ProcessArguments(args);
                Run();
            }
            catch (Exception ex)
            {
                Logger.Fatal("Error [ " + ex + " ]");
            }
            finally
            {
                Logger.Debug(MethodBase.GetCurrentMethod().Name);

                //Report end time.
                DateTime exeEnd = DateTime.Now;
                Logger.Info("");
                Logger.Info("Completed");
                Logger.Info("Process started: " + exeStart.ToLongDateString() + " " + exeStart.ToLongTimeString());
                Logger.Info("Process ended: " + exeEnd.ToLongDateString() + " " + exeEnd.ToLongTimeString());
                Logger.Info("Process length: " + (exeEnd - exeStart));


                Console.WriteLine("Complete. Press any key to exit.");
                Console.ReadKey(true);
            }
        }

        private static void Run()
        {
            Common.Common.InitializeEsriLicense();
            Common.Common.InitializeArcFmLicense();

            Logger.Info("Initializing PGE.BatchApplication.MapDocumentValidation");

            IWorkspace wksp = Common.Common.OpenWorkspaceFromSdeFile(_sdeFilePath);
            IList<ErrorMessage> errors;

            switch (_operation.ToLower())
            {
                case "validatemxds":
                    errors = DoMxdValidation(wksp);
                    MapDocumentValidator.WriteErrorsToFile(errors);
                    break;
                case "validatesds":
                    errors = DoStoredDisplaysValidation(wksp);
                    MapDocumentValidator.WriteErrorsToFile(errors);
                    break;
                case "exportallsds":
                    Logger.Info("Exporting stored displays to csv");
                    MapDocumentDataExtractor.WriteMapDocumentsToFile(
                        new MapDocumentValidator(wksp).ExportStoredDisplays(),
                        ConfigurationManager.AppSettings["ExportCSVWriteLocation"]);
                    break;
                case "exportsd":
                    Logger.Info("Exporting " + _mapDoc1 + " stored display to csv");
                    MapDocumentDataExtractor.WriteMapDocumentsToFile(
                        new MapDocumentValidator(wksp).ExportStoredDisplays(_mapDoc1),
                        ConfigurationManager.AppSettings["ExportCSVWriteLocation"]);
                    break;
                case "exportmxd":
                    Logger.Info("Exporting " + _mapDoc1 + " mxd to csv");
                    MapDocumentDataExtractor.WriteMapDocumentsToFile(
                            new MapDocumentValidator(wksp).ExportMxd(_mapDoc1),
                        ConfigurationManager.AppSettings["ExportCSVWriteLocation"]);
                    break;

            }


            Common.Common.CloseLicenseObject();
        }

        private static IList<ErrorMessage> DoMxdValidation(IWorkspace wksp)
        {
            IList<ErrorMessage> errors = null;
            MapDocumentValidator validator = new MapDocumentValidator(wksp);

            if (_mapDoc1.Length == 0 && _mapDoc2.Length == 0)
            {
                Logger.Info("No map document arguments parsed, validating all mxds in folder.");
                
                errors = validator.CompareAllMxdsBetweenFolders(ConfigurationManager.AppSettings["ControlMxdFolder"],
                    ConfigurationManager.AppSettings["CompareMxdFolder"],
                    Directory.GetFiles(ConfigurationManager.AppSettings["ControlMxdFolder"]).Select(Path.GetFileName));
            }
            else
            {
                try
                {
                    Logger.Info("Validating " + _mapDoc1 + " against " + _mapDoc2 +
                                " using local TNS file with saved TNS link in mxd.");

                    errors = validator.CompareTwoMxds(_mapDoc1, _mapDoc2);
                }
                catch (Exception e)
                {
                    Logger.Error(e.Message);
                }
            }

            

            return errors;
        }

        private static IList<ErrorMessage> DoStoredDisplaysValidation(IWorkspace wksp)
        {
            IList<ErrorMessage> errors = null;

            try
            {
                string dbName = (string) wksp.ConnectionProperties.GetProperty("INSTANCE");
                Logger.Info("Starting " + dbName);


                MapDocumentValidator validator = new MapDocumentValidator(wksp);

                if (_mapDoc1.Length == 0 && _mapDoc2.Length == 0)
                {
                    Logger.Info("No map document arguments parsed, validating all stored displays in db.");
                    errors = validator.ValidateAllStoredDisplaysForDatabase();
                }
                else
                {
                    Logger.Info("Validating " + _mapDoc1 + " against " + _mapDoc2 + " in db.");
                    errors = validator.CompareTwoStoredDisplays(_mapDoc1, _mapDoc2);
                }

                Logger.Info("Done with " + dbName);
            }
            catch (Exception e)
            {
                Logger.Error(e.Message);
                Logger.Error("Issue with SDE file: " + _sdeFilePath);
            }

            return errors;
        }

        /// <summary>
        ///     Processes arguments passed in via command line and stores them in class variables that were defined earlier
        /// </summary>
        /// <param name="args">The args array passed into the main method of this application</param>
        private static void ProcessArguments(string[] args)
        {
            // M4JF EDGISREARCH 919
            _sdeFilePath = ReadEncryption.GetSDEPath(ConfigurationManager.AppSettings["EDER_SDEConnection"].ToUpper());
            //Logger.Debug(MethodBase.GetCurrentMethod().Name);

            // Parse Flags
            var flags = new FlagParser
            {
                {"h", "help", false, false, "Show usage descriptions.", f => _showHelp = f != null},
                {"a", "sd1", true, false, "The first map document", f => _mapDoc1 = f},
                {"b", "sd2", true, false, "The second map document", f => _mapDoc2 = f},
                {"o", "operation", true, false, "The operation to perform", f => _operation = f}
            };
            flags.Parse(args);

            // Enforce required fields.
            if (_showHelp || String.IsNullOrEmpty(_sdeFilePath) || _operation == "")
            {
                Logger.Warn(
                    "Usage: " +
                    "\nMapDocumentValidation.exe -o \"<Operation to perform>\" -a \"<First document name>\" -b \"<Second document name>\"" +
                    "\nUsing a given SDE file, opens the specified map document. If operation=validatemxds, it compares layers to determine if "+
                    "they are set up in the same way (symbology, font, size, etc.) in mxds. If operation=validatesds, "+
                    "it does the same with stored displays. If operation=export, it exports the current settings of the map documents." +
                    "\nMapDocumentValidation.exe -o \"<Operation>\" -a \"<First document path>\" -b \"<Second document path\"\n" +
                    "Same as above usage but with mxds" +
                    "\nMapDocumentValidation.exe -h" +
                    "\nProvides help with command usage" +
                    "\nMapDocumentValidation.exe" +
                    "\nValidates all stored displays in the database against each other\n\n");

                Logger.Warn("Ex: " +
                            "\nMapDocumentValidation.exe " +
                            "-o export -a \"ED Master\" -b \"Circuit Map View\"");

                Environment.Exit(0);
            }
        }
    }
}