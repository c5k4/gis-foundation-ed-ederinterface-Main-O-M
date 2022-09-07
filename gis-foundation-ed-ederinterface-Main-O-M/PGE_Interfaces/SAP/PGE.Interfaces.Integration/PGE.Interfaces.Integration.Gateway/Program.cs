using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ESRI.ArcGIS.Geodatabase;
using Miner.Interop;
using ESRI.ArcGIS.esriSystem;
using System.Diagnostics;
using System.Threading;


namespace PGE.Interfaces.Integration.Gateway
{
    class Program
    {
        #region Private Variables
        private static Miner.Interop.IMMAppInitialize _appInitialize;
        private static LicenseInitializer m_AOLicenseInitializer;

        public static string comment = default;
        private static string startTime;
        private static StringBuilder remark = default;
        private static StringBuilder Argumnet = default;
        #endregion

        #region Public Variables
        #endregion 

        static void Main(string[] args)
        {
            DateTime dtStart = default;

            try
            {
                dtStart = DateTime.Now;
                startTime = dtStart.ToString("MM/dd/yyyy HH:mm:ss");
                m_AOLicenseInitializer = new LicenseInitializer();
                if (args == null || args.Length == 0)
                {
                    throw new Exception("No arguments passed");
                }

                if (args.Length != 2)
                {
                    throw new Exception("Incorrect number of arguments passed");
                }

                if (args == null || args.Length == 0)
                {
                    throw new Exception("No arguments passed");
                }

                if (args.Length != 2)
                {
                    throw new Exception("Incorrect number of arguments passed");
                }

                Common._log.Info("Process started.");
                InitializeLicense();

                if ((new MainClass()).StartProcess(args) == true)
                {

                    Common._log.Info("Process Completed Successfully.");
                    ExecutionSummary(startTime, comment, Argument.Done);
                }
                else
                {
                    Common._log.Info("Process not Completed Successfully.");
                    ExecutionSummary(startTime, comment, Argument.Error);
                }
            }
            catch (Exception exp)
            {
                Common._log.Error(exp.Message + " at " + exp.StackTrace);
                ErrorCodeException ece = new ErrorCodeException(exp);
                Environment.ExitCode = ece.CodeNumber;
                ExecutionSummary(startTime,  exp.Message + comment, Argument.Error);


            }
            finally
            {
                m_AOLicenseInitializer.ShutdownApplication();

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
                    Common._log.Error("This application could not initialize with the correct ArcFM license and will shutdown.");
                    return;
                }
                Common._log.Info("License initialized successfully.");
            }
            catch (Exception ex)
            {
                Common._log.Error("Exception occurred while intializing the ESRI/ArcFM license." + ex.Message.ToString());
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
                Common._log.Error("Exception occurred while Checking out the ArcFM license." + ex.Message.ToString());
                throw ex;
            }
        }
        #endregion


        /// <summary>
        /// This methos is to use for log the interface success or fail in interface summary table for EDGIS Rearch Project-V1t8
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="comment"></param>
        /// <param name="status"></param>
        private static void ExecutionSummary(string startTime, string comment, string status)
        {
            try
            {

                Argumnet = new StringBuilder();
                remark = new StringBuilder();
                //Setting arguments
                Argumnet.Append(MainClass.interfaceName + "_L7;");
                Argumnet.Append(MainClass.interfaceType + ";");
                Argumnet.Append(Argument.Integration);
                Argumnet.Append(startTime + ";");
                remark.Append(comment);
                Argumnet.Append(DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss") + ";");
                Argumnet.Append(status);
                Argumnet.Append(remark);

                //To execution for interface execution summary exe.This exe must need some input argument to run successffully. 
                ProcessStartInfo processStartInfo = new ProcessStartInfo(ReadConfiguration.intExecutionSummary, "\"" + Convert.ToString(Argumnet) + "\"");
                processStartInfo.UseShellExecute = false;
                processStartInfo.RedirectStandardOutput = true;
                processStartInfo.CreateNoWindow = true;

                Process proc = new Process();
                proc.StartInfo = processStartInfo;
                proc.EnableRaisingEvents = true;
                proc.Start();
                proc.BeginOutputReadLine();
                while (!proc.HasExited)
                {
                    Thread.Sleep(1000);
                }
            }
            catch (Exception ex)
            {
                Common._log.Error("Exception occures while executing the Interface Summary exe" + ex.Message);
                Common._log.Info(ex.Message + "   " + ex.StackTrace);
                Console.WriteLine("Exception occures while executing the Interface Summary exe", ex.StackTrace);
                //  fileWriter.WriteLine(" Error Occurred while executing the Interface Summary exe at " + DateTime.Now + " " + ex);
            }
        }

    }
}
