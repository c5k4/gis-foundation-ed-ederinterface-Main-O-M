using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ESRI.ArcGIS.esriSystem;
using Miner.Interop;
using System.Diagnostics;
using System.Runtime.InteropServices;
using PGE.BatchApplication.IGPPhaseUpdate.Processing_Logic_Classes;
using PGE.BatchApplication.IGPPhaseUpdate.Utility_Classes;

namespace PGE.BatchApplication.IGPPhaseUpdate
{
    class Program
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]

        private static extern bool TerminateProcess(IntPtr hProcess, uint uExitCode);

        private static Miner.Interop.IMMAppInitialize _appInitialize;
        private static LicenseInitializer m_AOLicenseInitializer = new LicenseInitializer();
        static void Main(string[] args)
        {
            Common CommonFunctions = new Common();
            bool bPOST = false;
            bool isChild = false;
            bool isChild_Batch2 = false;
            bool pProcess = false;
            bool blOnlyUserQueue = false;
            //string inputOIDs = "";
            string sBatchId = string.Empty;
            try
            {
                string Scircuitid = string.Empty;
                if (args.Length > 0)
                {
                    string[] splitarg = args[0].Split(',');
                    if (splitarg.Length > 0)
                    {
                        if (splitarg[0].ToString() == "-c")
                        {
                            isChild = true;
                            Scircuitid = splitarg[1].ToString();
                        }
                        else if (splitarg[0].ToString() == "-s")
                        {
                            isChild_Batch2 = true;
                            sBatchId = splitarg[1].ToString();
                        }
                    }
                }
                if (args.Length > 0 && args[0] == "-p")
                {
                    bPOST = true;
                }
                if (args.Length > 0 && args[0] == "-u")
                {
                    blOnlyUserQueue = true;
                }

                if (args.Length == 0)
                {
                    if (ReadConfigurations.Stage_To_Be_Completed == "POSTED")
                    {
                        bPOST = true;
                    }
                }
                InitializeLicense();
                DateTime dtStart = DateTime.Now;
                CommonFunctions.WriteLine_Info("Process is started at :" + DateTime.Now);


                //App crash issue resolution - Use below line to test only statistical report part 
                //new MainClass().ExtracttheReportinVersion("255292108", "88");

                //bool pProcess = MainClass_ProvideData.StartProcess(inputOIDs);
                if (isChild == true)
                {
                    //Start the process circuit wise
                    pProcess = (new MainClass()).StartProcess(isChild,Scircuitid);
                }
                else if (bPOST == true)
                {
                    //Start posting process
                    pProcess = (new Version_Management()).ReconcileAndPostAllPendingVersions();
                }
                else if (isChild_Batch2 == true)
                {
                    //Start the Batch2 process BatchID wise
                    pProcess = (new MainClass()).StartProcess_UpdateSession(isChild_Batch2, sBatchId);
                }
                else if (blOnlyUserQueue == true)
                {
                    //Start posting process
                    pProcess = (new MainClass()).StartProcess_UpdateSession(false,string.Empty);
                }
                else
                {
                    //Start the normal process
                    pProcess = (new MainClass()).StartProcess(false,string.Empty);
                }
                if (pProcess)
                {
                    CommonFunctions.WriteLine_Info("Process completed.");
                }
                else
                {
                    CommonFunctions.WriteLine_Info("Process not completed succesfully.");
                }

               
                CommonFunctions.WriteLine_Info("Total Time Taken :" + (DateTime.Now - dtStart));
                
            }
            catch (Exception exp)
            {
                CommonFunctions._log.Error(exp.Message + " at " + exp.StackTrace);

                ErrorCodeException ece = new ErrorCodeException(exp);
                Environment.ExitCode = ece.CodeNumber;
            }
            finally
            {
                m_AOLicenseInitializer.ShutdownApplication();
                TerminateProcess(Process.GetCurrentProcess().Handle, Convert.ToUInt32(Environment.ExitCode));
                //Process.GetCurrentProcess().Kill();
            }
        }
        #region InitializeLicense
        private static void InitializeLicense()
        {
            Common CommonFunctions = new Common();
            try
            {
                if (!m_AOLicenseInitializer.InitializeApplication(new esriLicenseProductCode[] { esriLicenseProductCode.esriLicenseProductCodeAdvanced, esriLicenseProductCode.esriLicenseProductCodeBasic, esriLicenseProductCode.esriLicenseProductCodeAdvanced },
                 new esriLicenseExtensionCode[] { }))
                {
                    System.Console.WriteLine(m_AOLicenseInitializer.LicenseMessage());
                    System.Console.WriteLine("This application could not initialize with the correct ArcGIS license and will shutdown.");
                    m_AOLicenseInitializer.ShutdownApplication();
                    return;
                }

                //Check out Telvent license
                if (CheckOutArcFMLicense(mmLicensedProductCode.mmLPArcFM) != mmLicenseStatus.mmLicenseCheckedOut)
                {
                    CommonFunctions._log.Error("This application could not initialize with the correct ArcFM license and will shutdown.");
                    return;
                }
            }
            catch (Exception ex)
            {
                CommonFunctions._log.Error("Exception occurred while intializing the ArcFM license." + ex.Message.ToString());
                throw ex;
            }
        }
        private static mmLicenseStatus CheckOutArcFMLicense(mmLicensedProductCode productCode)
        {
            Common CommonFunctions = new Common();
            try
            {
                if (_appInitialize == null) _appInitialize = new MMAppInitializeClass();

                var licenseStatus = _appInitialize.IsProductCodeAvailable(productCode);
                if (licenseStatus == mmLicenseStatus.mmLicenseAvailable)
                {
                    licenseStatus = _appInitialize.Initialize(productCode);
                }

                return licenseStatus;
            }
            catch (Exception ex)
            {
                CommonFunctions._log.Error("Exception occurred while Checking out the ArcFM license." + ex.Message.ToString());
                throw ex;
            }
        }
        #endregion

    }
}
