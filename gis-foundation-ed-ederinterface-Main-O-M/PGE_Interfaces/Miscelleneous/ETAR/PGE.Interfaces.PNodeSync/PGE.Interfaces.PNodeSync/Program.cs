using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.IO;
using System.Data;
using PGE.Interface.PNodeSync.OracleClasses;
using PGE.Interface.PNodeSync.PNodeSyncProcess;

// Configure log4net using the .config file
[assembly: log4net.Config.XmlConfigurator(Watch = true)]


namespace PGE.Interface.PNodeSync
{
    /// <summary>
    /// Feb-2019, ETAR Project - Moving FNM data from MDR to GIS
    /// This is a Console (EXE) application, that when executed will read the data from
    /// source and write the data to target.
    /// In this case SOURCE data is MDR and the TARGET data is GIS.
    /// The connection properties are stored in CONFIG file.
    /// 
    /// The process reads FNM data from MDR and write to FNM table in GIS.
    /// -The process is desinged to run nighly as UC4 job, and keep the GIS FNM table in Sync with FNM data in MDR.
    /// FNM: Full Network Mode
    /// MDR: Market Data Repository
    /// </summary>
    internal class Program
    {
        #region Private variables
        // Create a logger for use in this class
        private static readonly log4net.ILog _logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private static bool _exceptionWasThrown = false;
        #endregion

        static void Main(string[] args)
        {
            DateTime startTime = DateTime.Now;
            LoadDataToGISFNM loadFNMData = null;
            try
            {
                _logger.Info("Process started: " + startTime.ToLongDateString() + " " + startTime.ToLongTimeString());
                if (!InitializeProcess.InitializeVariables())
                {
                    throw new Exception("Failed intialize process.");
                }

                //Call the class to load data to GIS FNM table.
                loadFNMData = new LoadDataToGISFNM();
                loadFNMData.ExecuteLoad();

            }
            catch (Exception ex)
            {
                _logger.Error(ex.ToString());
                _exceptionWasThrown = true;
            }
            finally
            {
                if (loadFNMData != null)
                    loadFNMData.Close();
                LogEndProcess(startTime);
                if (Debugger.IsAttached)
                {
                    Console.WriteLine("Press a key to end program");
                    Console.ReadKey();
                }

                else if (_exceptionWasThrown || InitializeProcess.ErrorOccured)
                {
                    // This is used by the Batch file (when used) to signal concurrent process termination
                    System.Environment.Exit(1);
                }
            }
        }

        /// <summary>
        /// Log start/end/execution times to log file.
        /// </summary>
        private static void LogEndProcess(DateTime exeStart)
        {
            DateTime exeEnd = DateTime.Now;
            _logger.Info("");
            _logger.Info("Completed");
            _logger.Info("Process start time: " + exeStart.ToLongDateString() + " " + exeStart.ToLongTimeString());
            _logger.Info("Process end time: " + exeEnd.ToLongDateString() + " " + exeEnd.ToLongTimeString());
            _logger.Info("Process length: " + (exeEnd - exeStart).ToString());
        }


    }

}
