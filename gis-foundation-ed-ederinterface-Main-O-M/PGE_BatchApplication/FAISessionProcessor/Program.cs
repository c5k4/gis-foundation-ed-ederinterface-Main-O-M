using System;
using ESRI.ArcGIS.esriSystem;
using Miner.Interop;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.DataSourcesGDB;
using System.IO;
using System.Diagnostics;
using PGE.Common.Delivery.Diagnostics;
using PGE.Common.Delivery.Framework;

namespace PGE.BatchApplication.FAISessionProcessor
{
    class Program
    {
        /// <summary>
        /// Logs error/ custom information
        /// </summary>
        private static Log4NetLogger _log = new Log4NetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, "PGE.BatchApplication.FAISessionProcessor.log4net.config");
        private static bool isParent = true;
        [STAThread()]
        static void Main(string[] args)
        {
            Environment.ExitCode = (int)ExitCodes.Success;
            Stopwatch SW = new Stopwatch();
            SW.Start();
            try
            {
                string configurationFile = "";
                string pwd = "";


                if (args.Length > 0 && args[0] == "/?")
                {
                    Console.WriteLine("-c: Specify the location of the configuration file to use");
                    Console.WriteLine("-pwd: Specify a new password for encryption in the configuration file");
                    Console.WriteLine("Valid Argument combinations");
                    Console.WriteLine("-c,-pwd");
                    Console.WriteLine("-c");

                    Environment.Exit((int)ExitCodes.Success);
                }
                else
                {
                    //Process arguments
                    for (int i = 0; i < args.Length; i++)
                    {
                        if (args[i] == "-c")
                        {
                            configurationFile = args[i + 1];
                        }
                        else if (args[i] == "-pwd")
                        {
                            pwd = args[i + 1];
                        }
                    }

                    if (string.IsNullOrEmpty(configurationFile) || !File.Exists(configurationFile)) { throw new Exception("Invalid Configuration file specified: " + configurationFile); }

                    //Password reset 
                    if (pwd != string.Empty)
                    {
                        Common.ResetPassword(pwd, configurationFile);
                        Environment.Exit((int)ExitCodes.Success);
                    }

                    using (LicenseManager licenseManager = new LicenseManager())
                    {
                        //Check out licenses
                        licenseManager.Initialize(esriLicenseProductCode.esriLicenseProductCodeAdvanced, mmLicensedProductCode.mmLPArcFM);

                        //Open our workspace
                        Common.ReadAppSettings(configurationFile);

                        Logger.Info("GIS Workspace specified: " + Common.SDEConnectionFile);
                        Logger.Info("Landbase Workspace specified: " + Common.LandbaseConnectionFile);
                        Logger.Info("FAI FGDB Input Location specified: " + Common.InputFGDBLocation);
                        Logger.Info("FAI FGDB Archive Location specified: " + Common.ArchiveFGDBLocation);
                        Logger.Info("Session manager connection specified: " + Common.SessionManagerConnectionStringNoPassword);

                        Logger.Info("FAI Inserted Feature Classes clause: " + Common.AddedFeaturesFCWhereClause);
                        Logger.Info("FAI Updated Feature Classes clause: " + Common.UpdatedFeaturesFCWhereClause);
                        Logger.Info("FAI Deleted Feature Classes clause: " + Common.DeletedFeaturesFCWhereClause);

                        Logger.Info("FAI Inserted Tables clause: " + Common.AddedFeaturesTablesWhereClause);
                        Logger.Info("FAI Updated Tables clause: " + Common.UpdatedFeaturesTablesWhereClause);
                        Logger.Info("FAI Deleted Tables clause: " + Common.DeletedFeaturesTablesWhereClause);

                        if (string.IsNullOrEmpty(Common.SDEConnectionFile) || !File.Exists(Common.SDEConnectionFile) || !Common.SDEConnectionFile.ToUpper().EndsWith(".SDE")) { throw new Exception("Invalid GIS SDE connection file: " + Common.SDEConnectionFile); }
                        if (string.IsNullOrEmpty(Common.LandbaseConnectionFile) || !File.Exists(Common.LandbaseConnectionFile) || !Common.LandbaseConnectionFile.ToUpper().EndsWith(".SDE")) { throw new Exception("Invalid Landbase SDE connection file: " + Common.SDEConnectionFile); }
                        if (string.IsNullOrEmpty(Common.InputFGDBLocation) || !Directory.Exists(Common.InputFGDBLocation)) { throw new Exception("Input FGDB location does not exist: " + Common.InputFGDBLocation); }
                        if (string.IsNullOrEmpty(Common.ArchiveFGDBLocation) || !Directory.Exists(Common.ArchiveFGDBLocation)) { throw new Exception("Archive FGDB location does not exist: " + Common.ArchiveFGDBLocation); }

                        IWorkspaceFactory workspaceFactory = new SdeWorkspaceFactoryClass();
                        Logger.Info("Opening GIS Workspace: " + Common.SDEConnectionFile);
                        IWorkspace workspace = workspaceFactory.OpenFromFile(Common.SDEConnectionFile, 0);

                        Logger.Info("Opening Landbase Workspace: " + Common.SDEConnectionFile);
                        IWorkspace landbaseWorkspace = workspaceFactory.OpenFromFile(Common.LandbaseConnectionFile, 0);


                        //Execute our FAI processor
                        FAIProcessor FAIProcessor = new FAIProcessor(workspace, landbaseWorkspace);
                        FAIProcessor.ProcessFAIData();

                        //Shuts down the license
                        licenseManager.Shutdown();
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Failed to process due to an unexpected failure: " + ex.Message + " Stacktrace: " + ex.StackTrace);
                Console.WriteLine("Failed to process due to an unexpected failure: " + ex.Message + " Stacktrace: " + ex.StackTrace);
                Environment.ExitCode = (int)ExitCodes.Failure;
            }
            finally
            {
                SW.Stop();
                string timeToExecute = SW.Elapsed.Hours + ":" + SW.Elapsed.Minutes + ":" + SW.Elapsed.Seconds;
                Console.WriteLine("Execution time: " + timeToExecute);
            }

#if DEBUG
            if (isParent)
            {
                Console.WriteLine("Press any key to exit");
                Console.ReadLine();
            }
#endif

            //Force an exit just in case something is holding on to the process
            Environment.Exit(Environment.ExitCode);
        }

        public enum ExitCodes
        {
            Success,
            Failure
        };
    }
}
