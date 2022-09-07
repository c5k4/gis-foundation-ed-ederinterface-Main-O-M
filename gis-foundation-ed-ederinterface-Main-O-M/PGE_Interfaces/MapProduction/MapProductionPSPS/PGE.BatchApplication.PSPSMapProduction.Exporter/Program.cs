using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PGE.Common.Delivery.Diagnostics;
using System.Reflection;
using PGE.Common.Delivery.Framework;
using ESRI.ArcGIS.esriSystem;
using Miner.Interop;
using System.Diagnostics;
using PGE.Common.Delivery.Systems.Data;
using PGE.Common.Delivery.Systems.Data.Oracle;
using System.Data;
using PGE.BatchApplication.PSPSMapProduction.PreProcessor;
using System.Threading;
using System.Management;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geodatabase;
using PGE.BatchApplication.PSPSMapProduction.Export;
using ESRI.ArcGIS.Geometry;

namespace PGE.BatchApplication.PSPSMapProduction.Exporter
{
    public class Program
    {
        private static Log4NetLogger _logger = new Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "\\PGE.BatchApplication.PSPSMapProduction1.0.log4net.config", "PSPSMapProduction");
        private static Process childProcess = null;
        private static int _processTimeout = 0;
        /// <summary>
        /// Main program to execute the export operation
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            Environment.ExitCode = 0;
            try
            {
                int start = -1;
                int end = -1;

                if (args.Length == 1)
                {
                    //Get the process number
                    int processNumber = 0;
                    if (args.Length > 0)
                    {
                        int.TryParse(args[0], out processNumber);
                    }
                    _logger.Debug("Process Number:" + processNumber);

                    IList<string> errors = null;
                    using (LicenseManager licenseManager = new LicenseManager())
                    {
                        //Initialiase ArcGIS License
                        licenseManager.Initialize(ESRI.ArcGIS.esriSystem.esriLicenseProductCode.esriLicenseProductCodeAdvanced, mmLicensedProductCode.mmLPArcFM);
                        //licenseManager.Initialize(esriLicenseProductCode.esriLicenseProductCodeEngine, mmLicensedProductCode.mmLPEngine);
                        _logger.Debug("Starting the Exporter");
                        //Perform the export operatiion
                        IMPExporter exporter = new Export.PGEExporter();
                        errors = exporter.Export(processNumber, "", "", false);
                        //_logger.Debug(errors.ToString());  
                        //Shutdown the licese
                        licenseManager.Shutdown();
                    }


                    //Log the Success / Failure message
                    if (errors == null || errors.Count < 1)
                    {
                        _logger.Info(string.Format("Process ID : {0} sucessfully completed the export operation.", processNumber));
                    }
                    else
                    {
                        StringBuilder errorList = new StringBuilder();
                        _logger.Error(string.Format("Errors encountered while executing the Export operation for Process ID: {0}", processNumber));
                        _logger.Error("--------------------------------------------");
                        _logger.Error(string.Join(System.Environment.NewLine, errors.ToArray()));
                        _logger.Error("--------------------------------------------");
                    }
                }
            }
            catch (Exception ex)
            {
                Environment.ExitCode = (int)ExitCodes.Failure;
                _logger.Error(ex.Message, ex); 
            }
        }


        private static int getProcessTimeout()
        {
            try
            {
                if (_processTimeout == 0)
                {
                    _processTimeout = int.TryParse(MapProductionConfigurationHandler.Settings["PROCESS_TIMEOUT"], out _processTimeout) ? _processTimeout : 2400000;
                }
            }
            catch (Exception e) { _processTimeout = 2400000; }
            return _processTimeout;
        }

        private static Process SpawnProcess(int processID, bool isChild, bool createWindow, ProcessWindowStyle windowStyle, bool redirectOutput)
        {
            string arguments = processID.ToString();

            if (isChild)
            {
                arguments += " -child";
            }

            ProcessStartInfo startInfo = new ProcessStartInfo(System.Reflection.Assembly.GetExecutingAssembly().Location, arguments)
            {
                CreateNoWindow = createWindow,
                WindowStyle = windowStyle,
                UseShellExecute = false,
                RedirectStandardOutput = redirectOutput
            };
            return Process.Start(startInfo);
        }

        private static List<Process> SpawnProcesses(int start, int end)
        {
            List<Process> processes = new List<Process>();
            for (int i = start; i <= end; i++)
            {
                processes.Add(SpawnProcess(i, false, true, ProcessWindowStyle.Normal, false));
            }
            return processes;
        }

        public enum ExitCodes
        {
            Success,
            Failure,
            ChildFailure,
            InvalidArguments,
            LicenseFailure
        };
    }
}
