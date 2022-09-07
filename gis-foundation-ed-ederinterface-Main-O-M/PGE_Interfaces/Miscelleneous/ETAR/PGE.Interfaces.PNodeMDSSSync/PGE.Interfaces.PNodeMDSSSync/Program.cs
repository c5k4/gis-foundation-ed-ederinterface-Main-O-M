using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.IO;
using System.Data;
using PGE.Interface.PNodeMDSSSync.OracleClasses;
using PGE.Interface.PNodeMDSSSync.PNodeSyncProcess;

// Configure log4net using the .config file
[assembly: log4net.Config.XmlConfigurator(Watch = true)]

namespace PGE.Interface.PNodeMDSSSync
{
    /// <summary>
    /// June-2019, ETAR Project 
    /// -Read data from MDSS and importing to GIS
    /// -Moving PNode-Sublap data from GIS, to those tables, which are then exposed in Datamart, where MDSS can read them, and ingest the changes.
    /// This is a Console (EXE) application, that when executed will read the data from
    /// source and write the data to target.
    /// In this case SOURCE data is GIS and MDSS and the TARGET data is GIS.
    /// --Data is read from MDSS, and then compared with data in GIS, the changes are written to table, and then exposed in Datamart.
    /// The connection properties are stored in CONFIG file.
    /// 
    /// The process reads MDSS data, and copied the data to GIS, then updates the data based on changes in GIS.
    /// -The process is desinged to run nighly as UC4 job, and keeps the GIS data in-Sync with MDSS.
    /// GIS: Geographic Information Systems.
    /// MDSS: ?
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
            LoadDataToGIS_MDSSTbls loadMDSSData = null;
            try
            {
                _logger.Info("Process started: " + startTime.ToLongDateString() + " " + startTime.ToLongTimeString());
                if (!InitializeProcess.InitializeVariables())
                {
                    throw new Exception("Failed intialize process.");
                }
                //Call GIS functionality
                if (InterectPNodeCls.GISMain() == 1)
                    throw new Exception("GIS functionality failed. Please check log file.");
                //return;//for TESTING.

                //Call the class to load data to GIS MDSS table.
                loadMDSSData = new LoadDataToGIS_MDSSTbls();
                loadMDSSData.ExecuteLoad();

            }
            catch (Exception ex)
            {
                _logger.Error(ex.ToString());
                _exceptionWasThrown = true;
            }
            finally
            {
                if (loadMDSSData != null)
                    loadMDSSData.Close();
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
