using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using PGE.Common.CommandFlags;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geodatabase;
using PGE.BatchApplication.RecycleDataChanges.Utilities;

namespace PGE.BatchApplication.RecycleDataChanges
{
    class Program
    {
        private static TextWriterTraceListener Logger = new TextWriterTraceListener("Output.log", "ChangeDetection");
        private static LicenseInitializer m_AOLicenseInitializer = new LicenseInitializer();

        static void Main(string[] args)
        {
            //Console Properties
            Console.Title = "Recycle Data Changes";
            Console.BackgroundColor = ConsoleColor.DarkGreen;

            WriteStatus("╔══════════════════════════════════════════════════════════╗");
            System.Threading.Thread.Sleep(200);
            WriteStatus("║                   Recycle Data Changes                   ║");
            System.Threading.Thread.Sleep(200);
            WriteStatus("╚══════════════════════════════════════════════════════════╝");
            System.Threading.Thread.Sleep(200);
            Console.ResetColor();
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.Write("Created by Corey Blakeborough");
            Console.ResetColor();
            Console.WriteLine();
            System.Threading.Thread.Sleep(500);
            Console.WriteLine();

            #region Parameters/Flags
            ToolSettings.ResetSettings();

            bool showHelp = false;

            try
            {
                //Parse command line flags
                //Be sure to change ToolSettings.ToArguments() if this is modified.
                var flags = new FlagParser()
                {
                    { "h?", "help", false, false, "Show usage descriptions.", f => showHelp = f != null },
                    { "c", "connection", true, false, "The location of the SDE database connection.", f => ToolSettings.Instance.DatabaseLocation = f },
                    { "s", "sourceVersion", true, false, "(Optional) The source version (the version containing the changes) to detect changes against. Specifying the source and target versions enables file exporting.", f => ToolSettings.Instance.VersionSourceName = f },
                    { "t", "targetVersion", true, false, "(Optional) The target version to detect changes against. Specifying the source and target versions enables file exporting.", f => ToolSettings.Instance.VersionTargetName = f },
                    { "l", "loadToVersion", true, false, "(Optional) The version to which differences will be loaded. Specifying this value enables file importing.", f => ToolSettings.Instance.VersionLoadName = f },
                    { "x", "xmlFileLocation", true, false, "(Optional) The XML file to use when exporting and/or importing. Required if importing only.", f => ToolSettings.Instance.XmlFileLocation = f },
                };

                flags.Parse(args);

                //Set the boolean values for import/export enabled.
                if (ToolSettings.Instance.VersionSourceName != null && ToolSettings.Instance.VersionTargetName != null)
                    ToolSettings.Instance.ExportFileEnabled = true;
                if (ToolSettings.Instance.VersionLoadName != null)
                    ToolSettings.Instance.ImportFileEnabled = true;

                if (showHelp)
                {
                    Console.WriteLine("Options:");
                    flags.WriteHelp(Console.Out, true);
                    return;
                }

                // Enforce required fields.
                if (string.IsNullOrEmpty(ToolSettings.Instance.DatabaseLocation))
                    throw new FlagException("Missing the connection flag.");
                // These have to be set together: /s, /t
                if (string.IsNullOrEmpty(ToolSettings.Instance.VersionSourceName) ^ string.IsNullOrEmpty(ToolSettings.Instance.VersionTargetName))
                    throw new FlagException("Both a source version and a target version must be specified.");
                // If only importing, the /x flag must be set.
                if (ToolSettings.Instance.ImportFileEnabled && !ToolSettings.Instance.ExportFileEnabled && string.IsNullOrEmpty(ToolSettings.Instance.XmlFileLocation))
                    throw new FlagException("When importing only, the XML file location must be specified.");
                // If not importing or exporting, nothing is being done...
                if (!ToolSettings.Instance.ImportFileEnabled && !ToolSettings.Instance.ExportFileEnabled)
                    throw new FlagException("No import/export flags found.");
            }
            catch (FlagException fe)
            {
                WriteError(new Exception(fe.Message + (fe.InnerException != null ? Environment.NewLine + fe.InnerException.Message : "")));
                WriteStatus("For more information on parameters for this tool, use the \"help\" flag.");
                return;
            }
            catch (Exception ex)
            {
                WriteError(ex);
                return;
            }
            #endregion

            //Report start time.
            DateTime exeStart = DateTime.Now;
            WriteStatus("Process started: " + exeStart.ToLongDateString() + " " + exeStart.ToLongTimeString());
            WriteStatus("");

            try
            {   
                //ESRI License Initializer generated code.
                bool licenseCheckoutSuccess = m_AOLicenseInitializer.InitializeApplication(new esriLicenseProductCode[] { esriLicenseProductCode.esriLicenseProductCodeAdvanced },
                new esriLicenseExtensionCode[] {  });

                if (licenseCheckoutSuccess)
                {
                    try
                    {
                        m_AOLicenseInitializer.GetArcFMLicense(Miner.Interop.mmLicensedProductCode.mmLPArcFM);
                    }
                    catch (Exception e)
                    {
                        WriteError(e);
                        licenseCheckoutSuccess = false;
                        return;
                    }
                    
                    WriteStatus("Running tool with the following settings:");
                    WriteStatus("  Database Location = " + ToolSettings.Instance.DatabaseLocation);
                    WriteStatus("  Using XML file = " + ToolSettings.Instance.XmlFileLocation);
                    if (ToolSettings.Instance.ExportFileEnabled)
                    {
                        WriteStatus("  Export Versions:");
                        WriteStatus("    Source  Version = " + ToolSettings.Instance.VersionSourceName);
                        WriteStatus("    Target  Version = " + ToolSettings.Instance.VersionTargetName);
                    }
                    if (ToolSettings.Instance.ImportFileEnabled)
                    {
                        WriteStatus("  Import Versions:");
                        WriteStatus("    Load to Version = " + ToolSettings.Instance.VersionLoadName);
                        WriteStatus("");
                    }

                    //Open SDE file and load workspace.
                    WriteStatus("Trying to initialize the workspaces...");
                    IWorkspace ws = IWorkspace_OpenFromFile(ToolSettings.Instance.DatabaseLocation);
                    if (ws == null)
                        throw new Exception("Could not initialize workspace.");

                    WriteStatus("  Workspace initialized.");
                    WriteStatus("");

                    //Run main program logic.
                    Recycler recycler = new Recycler(ws);
                    recycler.RecycleDifferences();
                    WriteStatus("");
                }
                else
                {
                    throw new Exception("Unable to check out Esri ArcInfo License");
                }
            }
            catch (Exception ex)
            {
                //Format this error as an ErrorCodeException and throw it the same way (becomes a general error).
                WriteError(ex);
            }
            finally
            {
                //ESRI License Initializer generated code.
                m_AOLicenseInitializer.ShutdownApplication();

                //Report end time.
                DateTime exeEnd = DateTime.Now;
                WriteStatus("");
                WriteStatus("Completed");
                WriteStatus("Process start time: " + exeStart.ToLongDateString() + " " + exeStart.ToLongTimeString());
                WriteStatus("Process end time: " + exeEnd.ToLongDateString() + " " + exeEnd.ToLongTimeString());
                WriteStatus("Process length: " + (exeEnd - exeStart).ToString());
                WriteStatus("");

#if DEBUG
                Console.Write("Press any key to exit.");
                Console.ReadKey(true);
#endif
            }
        }

        /// <summary>
        /// Opens an SDE connection file from a file name and loads it into an IWorkspace.
        /// </summary>
        /// <param name="sFile">The file path of the desired SDE connection file.</param>
        /// <returns>An IWorkspace object from the connection.</returns>
        public static IWorkspace IWorkspace_OpenFromFile(string sFile)
        {
            IWorkspaceFactory workspaceFactory = new ESRI.ArcGIS.DataSourcesGDB.SdeWorkspaceFactory();
            return workspaceFactory.OpenFromFile(sFile, 0);
        }

        /// <summary>
        /// Writes a message to the console and the log.
        /// </summary>
        /// <param name="status">The message/status to write.</param>
        public static void WriteStatus(string status)
        {
            Logger.WriteLine(status);
            Console.WriteLine(status);
            Logger.Flush();
        }

        /// <summary>
        /// Writes exception details to the console and the log.
        /// </summary>
        /// <param name="ex">The exception about which to write details.</param>
        public static void WriteError(Exception ex)
        {
            string tempString = "**** Error encountered  ****: ";
            Logger.WriteLine(tempString);
            Console.WriteLine(tempString);
            Logger.WriteLine(ex);
            Console.WriteLine(ex);
            Logger.Flush();
        }

        /// <summary>
        /// Writes exception details to the console and the log.
        /// </summary>
        /// <param name="ex">The exception about which to write details. These exceptions should be considered warnings that do not break execution.</param>
        public static void WriteWarning(Exception ex)
        {
            string tempString = "**** Error encountered  ****: ";
            Logger.WriteLine(tempString);
            Console.WriteLine(tempString);
            Logger.WriteLine(ex);
            Console.WriteLine(ex);
            Logger.Flush();
        }
    }
}
