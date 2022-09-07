using System;
using System.Collections.Generic;
using System.Text;
using ESRI.ArcGIS.esriSystem;

namespace PGE.BatchApplication.PLDBBatchProcess
{
    class Program
    {
        private enum ExitCodes
        {
            Success,
            Failure,
            ChildFailure,
            InvalidArguments,
            LicenseFailure
        };
        private static LicenseInitializer m_AOLicenseInitializer = new PGE.BatchApplication.PLDBBatchProcess.LicenseInitializer();
    
        [STAThread()]
        static void Main(string[] args)
        {
            //Assume success until a failure occurs 
            Environment.ExitCode = (int)ExitCodes.Success;
            try
            {
                //ESRI License Initializer generated code.
                m_AOLicenseInitializer.InitializeApplication(new esriLicenseProductCode[] { esriLicenseProductCode.esriLicenseProductCodeAdvanced },
                new esriLicenseExtensionCode[] { });

                string operation = string.Empty; 

                for (int i = 0; i < args.Length; i++)
                {
                    if (args[i] == "-o")
                    {
                        operation = args[i + 1].ToString();
                    }
                }

                 if (operation.ToUpper() == "SYNC_PLD_INFO")
                {
                    PLDInfoProcessor pProcessor = new PLDInfoProcessor();
                    pProcessor.SynchronizePLD_Info();
                }
                else
                {
                    throw new Exception("Error unknown operation - incorrect parameters!"); 
                }

                //Do not make any call to ArcObjects after ShutDownApplication()
                m_AOLicenseInitializer.ShutdownApplication();
            }
            catch(Exception ex)
            {
                Shared.WriteToLogfile(ex.Message);
                Environment.ExitCode = (int)ExitCodes.Failure;
            }
        }
    }
}
