using System;
using System.Collections.Generic;
using System.Text;
using ESRI.ArcGIS.esriSystem;
using System.Threading;
using System.Reflection;

namespace PGE.BatchApplication.PGE_Tx_SL_Connectivity_Report
{
    class Program
    {
        private static LicenseInitializer m_AOLicenseInitializer = new PGE.BatchApplication.PGE_Tx_SL_Connectivity_Report.LicenseInitializer();
        private static readonly PGE.Common.Delivery.Diagnostics.Log4NetLogger _logger = new PGE.Common.Delivery.Diagnostics.Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType);

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
                    //Read application settings
                    Common.ReadAppSettings();

                    if (!string.IsNullOrEmpty(Common.SDEConnectionFile))
                    {
                        Console.WriteLine("Checking out licenses");
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
                            _logger.Debug("Licenses successfully checked out");
                            Console.WriteLine("Licenses successfully checked out");
                            //Call our trace functionality which will trace all circuits and build the geometric connectivity
                            //relationship mapping between transformers and service locations
                            Console.WriteLine("Beginning execution");
                            _logger.Debug("Beginning execution");
                            Tx_SL_RelMapping tx_SL_Mapping = new Tx_SL_RelMapping();
                            tx_SL_Mapping.Execute_Tx_SL_Mapping(Common.SDEConnectionFile);
                            Console.WriteLine("Execution finished");
                            _logger.Debug("Execution finished");
                        }

                        //ESRI License Initializer generated code.
                        //Do not make any call to ArcObjects after ShutDownApplication()
                        m_AOLicenseInitializer.ReleaseArcFMLicense();
                        m_AOLicenseInitializer.ShutdownApplication();
                    }
                    else
                    {
                        _logger.Error("Invalid configuration specified");
                        Console.WriteLine("Invalid configuration.  Please check the PGE.BatchApplication.PGE_Tx_SL_Connectivity_Report.exe.config file" +
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
            Console.WriteLine("No Arguments are needed. All configuration is performed through the PGE.BatchApplication.PGE_Tx_SL_Connectivity_Report.exe.config file" +
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
