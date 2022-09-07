using System;
using System.Collections.Generic;
using System.Net.Mime;
using System.Text;
using System.Threading;
using ESRI.ArcGIS.esriSystem;
using PGE.CircuitProtectionZone.Trace;
using PGE.Common;

namespace PGE.CircuitProtectionZone
{
    class Program
    {
        private static LicenseInitializer m_AOLicenseInitializer = new Tracing.LicenseInitializer();

        [STAThread()]
        static void Main(string[] args)
        {
            bool licenseCheckoutSuccess = false;

            try
            {
                if (args.Length > 0 && args[0] == "/?")
                {
                    WriteValidArgs();
                }
                else
                {

                    if (args.Length > 0 && args[0] == "-c")
                    {
                        isChild = true;
                    }
                    //Read application settings
                    Common.ReadAppSettings();

                    if (!string.IsNullOrEmpty(Common.EDSDEConnectionFile) && !string.IsNullOrEmpty(Common.SUBSDEConnectionFile))
                    {
                        if (!isChild) { Console.WriteLine("Checking out licenses"); }
                        _logger.Debug("Checking out licenses");
                        try
                        {
                            //ESRI License Initializer generated code.
                            licenseCheckoutSuccess = m_AOLicenseInitializer.InitializeApplication(new esriLicenseProductCode[] { Common.EsriLicense },
                                new esriLicenseExtensionCode[] { }) && m_AOLicenseInitializer.GetArcFMLicense(Common.ArcFMLicense);
                        }
                        catch (Exception e)
                        {
                            _logger.Error("Unable to check out licenses: Message: " + e.Message + ": StackTrace: " + e.StackTrace);
                            Environment.ExitCode = (int)ExitCodes.LicenseFailure;
                        }

                        if (licenseCheckoutSuccess)
                        {
                            if (isChild)
                            {
                                _logger.Debug("Licenses successfully checked out");
                                _logger.Debug("Beginning execution");
                                Tracing cache_Tracing = new Tracing();
                                cache_Tracing.ExecuteTraceCaching(Common.EDSDEConnectionFile, Common.SUBSDEConnectionFile, false);
                            }
                            else
                            {
                                _logger.Debug("Licenses successfully checked out");
                                Console.WriteLine("Licenses successfully checked out");
                                //Call our trace functionality which will trace all circuits and build the geometric connectivity
                                //relationship mapping between transformers and service locations
                                Console.WriteLine("Beginning execution");
                                _logger.Debug("Beginning execution");
                                Tracing cache_Tracing = new Tracing();
                                cache_Tracing.ExecuteTraceCaching(Common.EDSDEConnectionFile, Common.SUBSDEConnectionFile, true);
                                Console.WriteLine("Execution finished");
                            }
                        }

                        //ESRI License Initializer generated code.
                        //Do not make any call to ArcObjects after ShutDownApplication()
                        m_AOLicenseInitializer.ReleaseArcFMLicense();
                        m_AOLicenseInitializer.ShutdownApplication();
                        _logger.Debug("Execution finished");
                        Environment.Exit((int)ExitCodes.Success);
                    }
                    else
                    {
                        _logger.Error("Invalid configuration specified");
                        Console.WriteLine("Invalid configuration.  Please check the PGE_Cached_Tracing.exe.config file" +
                                            " in the installation directory");
                        Environment.ExitCode = (int)ExitCodes.InvalidConfiguration;
                    }
                }
            }
            catch (Exception e)
            {
                _logger.Error("Error during execution: Message: " + e.Message + ": StackTrace: " + e.StackTrace);
                Environment.ExitCode = (int)ExitCodes.Failure;
            }
        }

        private static void WriteValidArgs()
        {
            //Print out the arguments to the user
            Console.WriteLine("No Arguments are needed. All configuration is performed through the PGE_Tx_SL_Connectivity_Report.exe.config file" +
                " in the installation directory");
        }
    }

    public enum ExitCodes
    {
        Success,
        Failure,
        LicenseFailure,
        InvalidConfiguration
    };
}
