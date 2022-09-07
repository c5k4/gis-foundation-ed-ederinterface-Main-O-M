using System;
using PGE.BatchApplication.AUConveyor.Processing;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geodatabase;
using PGE.Common.CommandFlags;
using PGE.BatchApplication.AUConveyor.Utilities;

namespace PGE.BatchApplication.AUConveyor
{
    class Program
    {
        private static LicenseInitializer m_AOLicenseInitializer = new AUConveyor.Utilities.LicenseInitializer();

        [STAThread()]
        static void Main(string[] args)
        {
            //Set up tracing (without file logging until we know the potential input file).
            LogManager.AddConsoleLogger();

            //Console Properties
            Console.Title = "AU Conveyor";
            Console.BackgroundColor = ConsoleColor.DarkBlue;
            AnimateLine("╔══════════════════════════════════════════════════════════╗");
            System.Threading.Thread.Sleep(200);
            AnimateLine("║                     AU Conveyor Tool                     ║");
            System.Threading.Thread.Sleep(200);
            AnimateLine("╚══════════════════════════════════════════════════════════╝");
            System.Threading.Thread.Sleep(200);
            Console.ResetColor();
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.Write("                               Created by Corey Blakeborough");
            Console.Write("\r");
            AnimateLine("Version " + System.Reflection.Assembly.GetEntryAssembly().GetName().Version);
            Console.ResetColor();
            Console.WriteLine();
            System.Threading.Thread.Sleep(500);

            #region Parameters/Flags
            ToolSettings.ResetSettings();

            bool showHelp = false;
            bool elevatedHelp = false;
            bool flagF = false, flagX = false, flagK = false;

            try
            {
                //Parse command line flags
                //Be sure to change ToolSettings.ToArguments() if this is modified.
                var flags = new FlagParser()
                {
                    { "h?", "help", false, false, "Show usage descriptions.", f => showHelp = f != null },
                    { "H", "helpAll", false, true, "Show usage descriptions with all flags visible.", f => {showHelp = f != null; elevatedHelp = f != null; } },
                    { "c", "connection", true, false, "The location of the database connection.", f => ToolSettings.Instance.DatabaseLocation = f },
                    { "v", "version", true, false, "The version in which to make edits.\nIf parallel processing is enabled, this will be the BASE version for each new process and any existing numerically-named child versions will be deleted.", f => ToolSettings.Instance.VersionName = f },
                    { "a", "auprogid", true, false, "(Optional) The ProgID of the autoupdater to run en masse.", f => ToolSettings.Instance.AuProgId = f },
                    { "b", "bypassIfFieldsEqual", false, false, "(Optional) If set and an AU is specified, the tool will run the AU but only call Store() on the base feature when a field (other than the shape) has changed.", f => ToolSettings.Instance.BypassAuStoreWhenFieldsEqual = f != null },
                    { "t", "table", true, false, "(Optional) The name of the object class to process.\nIf this is not configured and an AU is set to run, the tool will process every table where the AU is configured.", f => ToolSettings.Instance.TableName = f },
                    { "w", "whereClause", true, false, "(Optional) Custom SQL to use as the WHERE clause for GIS query filters.\nDo not include the word WHERE in the string.", f => ToolSettings.Instance.CustomWhereClause = f },
                    { "e", "processes", true, false, "(Optional) The number of processes to run simultaneously in different versions. Defaults to 1. Enables the -z flag when greater than 1.", f => {int i; if (Int32.TryParse(f, out i)) ToolSettings.Instance.NumProcesses = i; } },
                    { "p", "postVersion", false, true, "(Optional) If set, the tool will reconcile and post this version when processing is completed. Parallel process versions that successfully post will be deleted. Can currently only be set for single processes. This will be set automatically on child processes if parallel processing is enabled.", f => ToolSettings.Instance.PostVersion = f != null },
                    { "y", "doNotPostChildVersions", false, true, "(Optional) If set, the tool will not reconcile and post child versions.", f => ToolSettings.Instance.DoNotPostChildVersions = f != null },
                    { "g", "generateInputFilesOnly", false, false, "(Optional) If set and parallel processing is enabled, the processes will not start, but the tool will instead generate the input files and immediately exit.\nThe input files can then be used for other purposes (often to split the load between servers).", f => ToolSettings.Instance.GenerateInputFilesOnly = f != null },
                    { "z", "hiddenProcessing", false, false, "(Optional) If set, the tool will NOT show console windows for created processes. Only applicable if parallel processing is set.", f => ToolSettings.Instance.HiddenProcessing = f != null },
                    { "s", "seamless", false, false, "(Optional) If set, the tool will NOT prompt the user to continue (the log file can still be used to check output).", f => ToolSettings.Instance.SeamlessMode = f != null },
                    { "r", "saveRowInterval", true, false, "(Optional) The frequency of rows after which the tool will save its progress. Defaults to 1000.", f => {int i; if (Int32.TryParse(f, out i)) ToolSettings.Instance.SaveRowInterval = i; } },
                    { "f", "forceAll", false, false, "(Optional) If set, the tool will force all related objects to trigger an update (or recreate, if -n is enabled), even if the AU was not set, not found, or not configured.", f => { ToolSettings.Instance.UpdateRelatedIfAuNotFound = f != null; flagF = f != null; } },
                    { "x", "forceType", true, false, "(Optional) If set to an esriFeatureType value, the tool will force ONLY the related objects of this Esri type to trigger an update (or recreate, if -n is enabled), even if the AU was not set, not found, or not configured.\nExample: \"esriFTAnnotation\"", f => { ToolSettings.Instance.UpdateRelatedIfAuNotFound = f != null; ToolSettings.Instance.ForceType = (esriFeatureType)Enum.Parse(typeof(esriFeatureType), f, true); flagX = f != null; } },
                    { "k", "forceClass", true, false, "(Optional) If set to the name of an object class, the tool will force related objects of only that object class to trigger an update (or recreate, if -n is enabled), even if the AU was not set, not found, or not configured.\nThis flag can be set multiple times to add multiple configured classes.", f => { ToolSettings.Instance.UpdateRelatedIfAuNotFound |= f != null; ToolSettings.Instance.ForceClassNames.Add(f); flagK |= f != null; }  },
                    { "n", "recreateRelated", false, false, "(Optional) Uses the related features indicated from either -f, -x, or -k.\nIf set, the specified related objects will be deleted and recreated during processing so long as they are set up to automatically create upon related object creation.", f => ToolSettings.Instance.RecreateRelated = f != null },
                    { "o", "onlyRecreateWhenExists", false, false, "(Optional) Used when recreateRelated is set. Only recreates related on a feature if at least one of the specified related objects exist and is placed.", f => ToolSettings.Instance.RecreateOnlyWhenExists = f != null },
                    { "I", "inputFile", true, true, "(Optional) Indicates the location of an input file to use for OID determination.", f => ToolSettings.Instance.InputFile = f },
                    { "d", "inputFileDirectory", true, false, "(Optional) Overrides the default input file directory location. This allows for multiple parallel processing jobs in the same directory.", f => { if (f != null) ToolSettings.Instance.InputFileDirectory = f; } }
                };

                flags.Parse(args);

                // Enforce required fields.
                if (showHelp)
                {
                    ShowHelp(flags, elevatedHelp);
                    return;
                }

                if (string.IsNullOrEmpty(ToolSettings.Instance.DatabaseLocation))
                    throw new FlagException("Missing the connection flag.");
                if (string.IsNullOrEmpty(ToolSettings.Instance.VersionName))
                    throw new FlagException("Missing the version flag.");
                // One of these two HAS to be set: /a, /t
                if (string.IsNullOrEmpty(ToolSettings.Instance.AuProgId) && string.IsNullOrEmpty(ToolSettings.Instance.TableName))
                    throw new FlagException("Either the table or the AU ProgID must be set for the tool to run.");
                // If no AU ProgID is set, a force flag needs to be specified.
                if (string.IsNullOrEmpty(ToolSettings.Instance.AuProgId) && !flagF && !flagX && !flagK)
                    throw new FlagException("When omitting an AU ProgID, a force flag needs to be set.");
                // Cna't use /b without /a
                if (string.IsNullOrEmpty(ToolSettings.Instance.AuProgId) && ToolSettings.Instance.BypassAuStoreWhenFieldsEqual)
                    throw new FlagException("When specifying the bypass option, an AU ProgID needs to be set.");
                // Either NONE or ONE of these flags may be set: /f, /x, /k
                if (Convert.ToInt32(flagF) + Convert.ToInt32(flagX) + Convert.ToInt32(flagK) > 1)
                    throw new FlagException("Both force flags may not be set - please choose one or the other.");
                // Need a force flag with /n
                if (ToolSettings.Instance.RecreateRelated && !flagF && !flagX && !flagK)
                    throw new FlagException("A force flag needs to be set in order to recreate related objects.");
                // Can't use /w with /I
                if (!string.IsNullOrEmpty(ToolSettings.Instance.CustomWhereClause) && !string.IsNullOrEmpty(ToolSettings.Instance.InputFile))
                    throw new FlagException("You cannot specify a custom WHERE clause if an input file has been specified.");
                // Can't use /e with /I
                if (ToolSettings.Instance.NumProcesses > 1 && !string.IsNullOrEmpty(ToolSettings.Instance.InputFile))
                    throw new FlagException("You cannot specify multiple processes if an input file has been specified.");
                // Can't use /p > 1 with /e
                if (ToolSettings.Instance.NumProcesses > 1 && ToolSettings.Instance.PostVersion)
                    throw new FlagException("You cannot specify to post the version if parallel processing is enabled.");
                // Can't use /g without /e
                if (ToolSettings.Instance.NumProcesses <= 1 && ToolSettings.Instance.GenerateInputFilesOnly)
                    throw new FlagException("You cannot specify to generate only input files without specifying a number of processes.");
                // Can't use /z without /e
                if (ToolSettings.Instance.NumProcesses <= 1 && ToolSettings.Instance.HiddenProcessing)
                    throw new FlagException("You cannot specify hidden processing without parallel processes enabled.");
                // Make sure that there's at least one process.
                if (ToolSettings.Instance.NumProcesses <= 0)
                    throw new FlagException("At least one process is required.");
                // Make sure that the save interval is at least one.
                if (ToolSettings.Instance.SaveRowInterval <= 0)
                    throw new FlagException("The save row interval must be 1 or higher.");
            }
            catch (FlagException fe)
            {
                WriteError(fe.Message + (fe.InnerException != null ? Environment.NewLine + fe.InnerException.Message : ""), true);
                LogManager.WriteLine("For more information on parameters for this tool, use the \"help\" flag.");
                return;
            }
            catch (Exception ex)
            {
                WriteError(ex);
                return;
            }
            #endregion

            //Add version name to the title.
            Console.Title += " - " + ToolSettings.Instance.VersionName;

            //Set up file logging.
            LogManager.AddFileLogger(ToolSettings.Instance.InputFile);

            //Report start time.
            DateTime exeStart = DateTime.Now;
            LogManager.WriteLine("Process started: " + exeStart.ToLongDateString() + " " + exeStart.ToLongTimeString(), ConsoleColor.Cyan, null);
            LogManager.WriteLine();
            
            //Licenses
            LogManager.WriteLine("Checking out licenses...");

            //ESRI License Initializer generated code.
            bool licenseCheckoutSuccess = m_AOLicenseInitializer.InitializeApplication(new esriLicenseProductCode[] { esriLicenseProductCode.esriLicenseProductCodeAdvanced },
            new esriLicenseExtensionCode[] { });

            if (licenseCheckoutSuccess)
            {
                try
                {
                    m_AOLicenseInitializer.GetArcFMLicense(Miner.Interop.mmLicensedProductCode.mmLPArcFM);
                }
                catch (Exception e)
                {
                    WriteError("Unable to checkout ArcFM license", false);
                    WriteError(e);
                    licenseCheckoutSuccess = false;
                    return;
                }

                try
                {
                    Conveyor AUConveyor = new Conveyor();
                    if (AUConveyor.Enabled())
                    {
                        if (ToolSettings.Instance.NumProcesses > 1)
                        {
                            ConveyorSpawn spawner = new ConveyorSpawn(AUConveyor);
                            spawner.SpawnThreads();

                            
                            if (ToolSettings.Instance.GenerateInputFilesOnly)
                            {
                                LogManager.WriteLine("");
                                LogManager.WriteLine("Input files written to directory: \"" + ToolSettings.Instance.InputFileDirectory + "\"", ConsoleColor.Magenta, null);
                                LogManager.WriteLine("The tool will now exit due to the configured flags.");
                            }
                            else
                            {
                                //Start the threads.
                                spawner.StartThreads();
                            }
                        }
                        else
                        {
                           AUConveyor.Convey();
                        }
                    }
                    else
                    {
                        //Catchall for incompatibilities. Give them more specific errors when possible.
                        WriteError("This tool cannot be run with the specified parameters.", true);
                    }
                }
                catch (Exception ex)
                {
                    WriteError(ex);
                }
            }
            else
            {
                WriteError("Unable to check out Esri ArcInfo License", true);
            }

            try
            {
                m_AOLicenseInitializer.ReleaseArcFMLicense();
            }
            catch { }

            try
            {
                //Do not make any call to ArcObjects after ShutDownApplication()
                m_AOLicenseInitializer.ShutdownApplication();
            }
            catch { }


            //Report end time.
            DateTime exeEnd = DateTime.Now;
            LogManager.WriteLine();
            LogManager.WriteLine("Completed", ConsoleColor.Green, null);
            LogManager.WriteLine("Process started: " + exeStart.ToLongDateString() + " " + exeStart.ToLongTimeString(), ConsoleColor.Cyan, null);
            LogManager.WriteLine("Process ended: " + exeEnd.ToLongDateString() + " " + exeEnd.ToLongTimeString(), ConsoleColor.Cyan, null);
            LogManager.WriteLine("Process length: " + (exeEnd - exeStart).ToString(), ConsoleColor.Cyan, null);
            LogManager.WriteLine();
            Program.WaitIfNotSeamless();
        }

        /// <summary>
        /// Shows the flags and descriptions for this program.
        /// </summary>
        /// <param name="p">The FlagParser object that contains the flags to parse.</param>
        /// <param name="elevated">Whether or not the user has specified to get help for all commands.</param>
        private static void ShowHelp(FlagParser p, bool elevated)
        {
            Console.WriteLine();
            Console.WriteLine("Options:");
            p.WriteHelp(Console.Out, elevated);
        }

        /// <summary>
        /// Writes each character to the line at one time. Used for the titles only.
        /// </summary>
        /// <param name="message"></param>
        static void AnimateLine(string message)
        {
            string line = "";
            foreach (char c in message)
            {
                line += c.ToString();
                Console.Write("\r" + line);
                System.Threading.Thread.Sleep(5);
            }
            Console.WriteLine();
        }

        /// <summary>
        /// Log/Console method that allows a warning message to be written in a consistent manner.
        /// </summary>
        /// <param name="message">The warning message to write.</param>
        public static void WriteWarning(string message)
        {
            ProcessBitgate.ThisExitBitgate.WarningsEncountered = true;

            LogManager.WriteLine();
            LogManager.WriteLine("**Warning:  " + message, ConsoleColor.Yellow, null);
        }

        /// <summary>
        /// Log/Console method that allows an error message to be written in a consistent manner.
        /// </summary>
        /// <param name="message">The error message to write.</param>
        /// <param name="wait">If the program is not in seamless mode, whether or not to wait after this message for user input.</param>
        public static void WriteError(string message, bool wait)
        {
            ProcessBitgate.ThisExitBitgate.ErrorsEncountered = true;

            LogManager.WriteLine();
            LogManager.WriteLine("**Error: " + message, ConsoleColor.Red, null);

            System.Threading.Thread.Sleep(1000);

            if (wait)
                WaitIfNotSeamless();
        }

        /// <summary>
        /// Log/Console method that allows an error message to be written in a consistent manner.
        /// </summary>
        /// <param name="message">The error to write.</param>
        public static void WriteError(Exception ex)
        {
            ProcessBitgate.ThisExitBitgate.ErrorsEncountered = true;

            LogManager.WriteLine();
            string[] errorComponents = ex.ToString().Split(new string[] { "   at" }, StringSplitOptions.RemoveEmptyEntries);

            //A little extra logic to make the messsage component of any errors easier to read.
            bool firstMessage = true;
            foreach (string s in errorComponents)
            {
                if (firstMessage)
                {
                    LogManager.Write("**Error: " + s, ConsoleColor.Red, null);
                    firstMessage = false;
                }
                else
                {
                    LogManager.Write("  at " + s, ConsoleColor.DarkGray, null);
                }
            }
            LogManager.WriteLine();

            System.Threading.Thread.Sleep(1000);

            WaitIfNotSeamless();
        }

        /// <summary>
        /// Indicates a point in execution where a user will be prompted to press a key to continue,
        /// unless the program is running in seamless mode.
        /// </summary>
        public static void WaitIfNotSeamless()
        {
            if (!ToolSettings.Instance.SeamlessMode)
            {
                Console.BackgroundColor = ConsoleColor.DarkBlue;
                Console.Write("► Press any key to continue.");
                Console.ReadKey();
                Console.ResetColor();
                Console.WriteLine();
            }
        }
    }
}
