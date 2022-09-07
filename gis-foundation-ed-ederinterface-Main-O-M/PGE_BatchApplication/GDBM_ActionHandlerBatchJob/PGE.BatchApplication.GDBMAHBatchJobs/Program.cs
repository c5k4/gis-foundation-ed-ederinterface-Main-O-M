// ========================================================================
// Copyright © 2021 PGE 
// <history>
// To execute SSD,Circuit Color,Jet Operating Number and DeviceGroupCircuitID Action Handler logic through batchjob
// YXA6 4/14/2021	Created
// JeeraID-> EDGISRearch-376
// </history>
// All rights reserved.
// ========================================================================
using System;
using Miner.Interop;
using ESRI.ArcGIS.esriSystem;
using System.Runtime.InteropServices;
using PGE.BatchApplication.GDBMAHBatchJobs.UtilityClasses;

namespace PGE.BatchApplication.GDBMAHBatchJobs
{
    class Program
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]

        private static extern bool TerminateProcess(IntPtr hProcess, uint uExitCode);
        private static Miner.Interop.IMMAppInitialize _appInitialize;
        private static LicenseInitializer m_AOLicenseInitializer = new LicenseInitializer();
        private static Common CommonFuntions = new Common();

        [Obsolete]
        static void Main(string[] args)
        {
            try
            {
                DateTime dtStart = DateTime.Now;
                //fdg
                string Scircuitid = string.Empty;
                string sParentVersionName = string.Empty;
                CommonFuntions.WriteLine_Info("Process is started at :");
                InitializeLicense();
               
                if (args.Length > 0)
                {
                    string[] splitarg = args[0].Split(',');
                    if (splitarg.Length > 0)
                    {
                        if (splitarg[0].ToString() == "-c")
                        {
                            MainClass.m_blIfChild = true;
                            Scircuitid = splitarg[1].ToString();
                            sParentVersionName = splitarg[2].ToString();
                        }
                        
                    }
                }
                if (!MainClass.m_blIfChild)
                {
                    CommonFuntions.WriteLine_Info("Parent Process Started");
                    if ((new GeoDBHelper()).CreateArcFMConnection())
                    {
                        CommonFuntions.WriteLine_Info("ArcFM Connected successfully :");
                    }
                    else
                    {
                        CommonFuntions.WriteLine_Info("ArcFM Connected successfully :");
                    }
                    if ((new MainClass()).ExecuteParentProcess())
                    {
                        CommonFuntions.WriteLine_Info("Parent Process Completed Successfully.");
                    }
                    


                }
                if (MainClass.m_blIfChild)
                {
                    if ((new ChildProcess()).ExecuteChildProcess(Scircuitid,sParentVersionName))
                    {
                        CommonFuntions.WriteLine_Info("Child Process Completed Successfully.");
                    }
                    else
                    {
                        CommonFuntions.WriteLine_Info("Child Process not Completed Successfully.");
                    }
                }
            }
            catch (Exception exp)
            {
                CommonFuntions.WriteLine_Error(exp.Message + " at " + exp.StackTrace);
                ErrorCodeException ece = new ErrorCodeException(exp);
                Environment.ExitCode = ece.CodeNumber;
            }
            finally
            {
                m_AOLicenseInitializer.ShutdownApplication();
              
            }
        }

       

        #region InitializeLicense
        private  static void InitializeLicense()
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
                    CommonFuntions.WriteLine_Error("This application could not initialize with the correct ArcFM license and will shutdown.");
                    return;
                }
                CommonFuntions.WriteLine_Info("License initialized successfully.");
            }
            catch (Exception ex)
            {
                CommonFuntions.WriteLine_Error("Exception occurred while intializing the ESRI/ArcFM license." + ex.Message.ToString());
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
                CommonFuntions.WriteLine_Error("Exception occurred while Checking out the ArcFM license." + ex.Message.ToString());
                throw ex;
            }
        }
        #endregion
    }
}
