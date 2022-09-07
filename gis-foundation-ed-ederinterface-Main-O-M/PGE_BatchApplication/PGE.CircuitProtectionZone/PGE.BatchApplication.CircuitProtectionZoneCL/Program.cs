using System;
using PGE.BatchApplication.CircuitProtectionZone;
using PGE.BatchApplication.CircuitProtectionZone.Common;
using ESRI.ArcGIS.esriSystem;
using Miner.Interop;
using System.Text.RegularExpressions;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;

namespace PGE.BatchApplication.CircuitProtectionZoneCL
{
    class Program
    {
        private static LicenseInitializer m_AOLicenseInitializer = new LicenseInitializer();

        [STAThread()]
        static void Main(string[] args)
        {
            AppDomain.CurrentDomain.UnhandledException += UhandledExceptionHandler;

            try
            {
#if DEBUG
                Debugger.Launch();
#endif
                Args clArgs = Args.Create(args);

                clArgs.Validate();

                bool licenseCheckoutSuccess = false;
                try
                {
                    //ESRI License Initializer generated code.
                    licenseCheckoutSuccess = m_AOLicenseInitializer.InitializeApplication(new esriLicenseProductCode[] { esriLicenseProductCode.esriLicenseProductCodeAdvanced },
                        new esriLicenseExtensionCode[] { }) && m_AOLicenseInitializer.GetArcFMLicense(mmLicensedProductCode.mmLPArcFM);
                }
                catch (Exception e)
                {
                    Logger.Error("Unable to check out licenses: Message: " + e.Message + ": StackTrace: " + e.StackTrace, true);
                    Environment.ExitCode = (int)ExitCodes.LicenseFailure;
                }

                if (licenseCheckoutSuccess)
                {
                    try
                    {
                        int processID = -1;
                        if (!string.IsNullOrEmpty(clArgs.ProcessID)) { processID = Int32.Parse(clArgs.ProcessID); }
                        int numChildProcesses = 0;
                        if (!string.IsNullOrEmpty(clArgs.NumProcesses)) { numChildProcesses = Int32.Parse(clArgs.NumProcesses); }

                        var trace = new ProtectionZoneTrace(clArgs.ConnectionString, clArgs.LandbaseConnectionFile,
                            clArgs.SubstationConnectionFile, clArgs.ReportType, clArgs.ReportFileLocation, clArgs.ReportFileName);

                        if (numChildProcesses > 1)
                        {
                            trace.Execute(numChildProcesses);
                        }
                        else
                        {
                            trace.ExecuteTracing(processID);
                        }

                        //Post processing to calculate customers for performance reasons
                        trace.CalculateCustomers(processID);

                        m_AOLicenseInitializer.ReleaseArcFMLicense();
                        m_AOLicenseInitializer.ShutdownApplication();

                        Environment.Exit((int)ExitCodes.Success);
                    }
                    catch (Exception e)
                    {
                        Logger.Error("cpz failed", e, true);
                        Environment.ExitCode = (int)ExitCodes.Failure;
                    }
                }
            }
            catch (ArgsException ex)
            {
                ShowUsage(ex);
            }
            catch (Exception ex)
            {
                Logger.Error("Unexpected error during execution", ex, true);
                Console.WriteLine();
                Console.WriteLine("Exception occured during run. Check log for details. ");
                Console.WriteLine("Exception Message: " + ex.Message);
            }
        }

        public enum ExitCodes
        {
            Success,
            Failure,
            LicenseFailure,
            InvalidConfiguration
        };

        private static void ShowUsage(ArgsException ex)
        {
            Console.WriteLine();
            Console.WriteLine(ex.Message);
            Console.WriteLine();
            Console.WriteLine("PGE.BatchApplication.CircuitProtectionZoneCL -c <connection file> -o <database owner> -n <network name> -loglevel [Error|Info|Debug] -logdir <logdir> -logname <logname>");
            Console.WriteLine("\t-c : Required - Location of the connection file.");
            Console.WriteLine("\t-o : Optional - Database owner.");
            Console.WriteLine("\t-n : Required - Network name.");
            Console.WriteLine("\t-f : Optional - Location of the fire tier shapefile. (Optional)");
            Console.WriteLine("\t-r : Optional - Location of the result shapefile. (Optional");
            Console.WriteLine("\t-loglevel : Optional - Either Error, Info, or Debug (Info is default)");
            Console.WriteLine("\t-logname : Optional - Log file name (CpzCommandLine.log is default)");
            Console.WriteLine("\t-logdir : Optional - Log file location (current dir of exe is default)");
            Console.ReadLine();
        }

        private static void UhandledExceptionHandler(object sender, UnhandledExceptionEventArgs e)
        {
            if (e.IsTerminating == true)
            {
                Console.WriteLine("Unhandled Fatal Exception: " + e.ExceptionObject);
            }
            else
            {
                Console.WriteLine("Unhandled Exception: " + e.ExceptionObject);
            }
        }
    }
}
