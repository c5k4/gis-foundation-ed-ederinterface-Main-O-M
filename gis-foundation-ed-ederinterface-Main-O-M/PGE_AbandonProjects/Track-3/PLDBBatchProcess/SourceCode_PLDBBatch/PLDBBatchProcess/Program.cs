using System;
using System.Collections.Generic;
using System.Text;
using ESRI.ArcGIS.esriSystem;

namespace PLDBBatchProcess
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
        private static LicenseInitializer m_AOLicenseInitializer = new PLDBBatchProcess.LicenseInitializer();
    
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

                if (operation.ToUpper() == "SYNC_PLDB")
                {
                    PLDBProcessor pProcessor = new PLDBProcessor();
                    pProcessor.SynchronizePLDB();
                }
                else if (operation.ToUpper() == "SYNC_PLD_INFO")
                {
                    PLDInfoProcessor pProcessor = new PLDInfoProcessor();
                    pProcessor.SynchronizePLD_Info();
                }

                //else if (operation.ToUpper() == "FIX_DUP_PLDBIDS")
                //{
                //    PLDBProcessor pProcessor = new PLDBProcessor();
                //    pProcessor.Decommission_FIX_DUPs();
                //}
                //else if (operation.ToUpper() == "FIX_DUP_PLDBIDS")
                //{
                //    PLDBProcessor pProcessor = new PLDBProcessor();
                //    pProcessor.FixDecommissions();
                //}
                else
                {
                    throw new Exception("Error unknown operation - incorrect parameters!"); 
                }

                //Do not make any call to ArcObjects after ShutDownApplication()
                m_AOLicenseInitializer.ShutdownApplication();
            }
            catch
            {
                Environment.ExitCode = (int)ExitCodes.Failure;
            }
        }
    }
}
