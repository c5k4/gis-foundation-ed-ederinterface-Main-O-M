using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Telvent.Delivery.Systems.Data;
using Telvent.Delivery.Systems.Data.Oracle;
using System.Data;
using Telvent.PGE.MapProduction.Processor;
using Telvent.Delivery.Diagnostics;
using System.Reflection;
using System.Diagnostics;

namespace Telvent.PGE.MapProduction.Export
{
    /// <summary>
    /// Executes the export operation by Process ID
    /// </summary>
    public class PGEExporter : IMPExporter
    {
        //private static Log4NetLogger _logger = new Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private static Log4NetLogger _logger = new Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "\\MapProduction1.0.log4net.config", "MapProduction");

        #region Private Fields

        private IDatabaseConnection _dbConnection = null;
        IMPDBDataManager _dbInteraction = null;
        private const string _successMessage = "Successfully processed maps";

        #endregion Private Fields

        #region Constructor

        /// <summary>
        /// Initialises new instance of <see cref="PGEExporter"/>
        /// </summary>
        public PGEExporter()
        { }

        #endregion Constructor

        #region Public Properties

        /// <summary>
        /// Connection the Oracle Database
        /// </summary>
        public IDatabaseConnection DatabaseConnection
        {
            get
            {
                if (_dbConnection == null)
                {
                    _dbConnection = new OracleDatabaseConnection(MapProductionConfigurationHandler.DatabaseServer, MapProductionConfigurationHandler.DataSource, MapProductionConfigurationHandler.UserName, MapProductionConfigurationHandler.Password);
                }
                return _dbConnection;
            }
        }

        /// <summary>
        /// Sucess message
        /// </summary>
        public string SuccessMessage
        {
            get { return _successMessage; }
        }

        /// <summary>
        /// Returns an instance of PGEMapLookUpTable
        /// </summary>
        public IMPMapLookUpTable MapLookUpTable
        {
            get { return new PGEMapLookUpTable(); }
        }
        
        /// <summary>
        /// New instance of DBDataManager object
        /// </summary>
        public IMPDBDataManager DatabaseManager
        {
            get
            {
                if (_dbInteraction == null)
                {
                    _dbInteraction = new DBDataManager(DatabaseConnection, MapLookUpTable);
                }
                return _dbInteraction;
            }
            protected set
            {
                _dbInteraction = value;
            }
        }

        #endregion Public Properties

        #region Public Methods

        /// <summary>
        /// Performs export operation for the the given Process ID
        /// </summary>
        /// <param name="processId">Process ID to perform the export operation</param>
        /// <returns>Returns the list of errors encountered, if any, while performing the operation</returns>
        public List<string> Export(int processId)
        {
            List<string> LegacyCoordinates = new List<string>();
            LegacyCoordinates.Add("NAD 1927 StatePlane California I FIPS 0401");
            LegacyCoordinates.Add("NAD 1927 StatePlane California II FIPS 0402");
            LegacyCoordinates.Add("NAD 1927 StatePlane California III FIPS 0403");
            LegacyCoordinates.Add("NAD 1927 StatePlane California IV FIPS 0404");
            LegacyCoordinates.Add("NAD 1927 StatePlane California V FIPS 0405");
            List<int> scaleValues = new List<int>();
            scaleValues.Add(25);
            scaleValues.Add(50);
            scaleValues.Add(100);
            scaleValues.Add(200);
            scaleValues.Add(250);
            scaleValues.Add(500);
            List<string> retVal = new List<string>();

            foreach (string legacyCoordinate in LegacyCoordinates)
            {
                foreach (int scale in scaleValues)
                {
                    //For each new scale / coordinate combination we need to tell the export process that it will
                    //need to reload its mxd
                    PGEMapProcess.LoadPDFMxd = true;
                    PGEMapProcess.LoadTiffMxd = true;


                    DataTable mapLut = DatabaseManager.GetMapLookUpTable("where " + MapLookUpTable.MapExportState + "=" + Convert.ToInt32(ExportState.ReadyToExport) + " and " + MapLookUpTable.ProcessingService + "=" + processId
                        + " and " + "LEGACYCOORDINATE = " + "'" + legacyCoordinate + "'" + " and " + MapLookUpTable.MapScale + "=" + scale);
                    int i = 0;
                    try
                    {
                        _logger.Debug("LegacyCoordinate: " + legacyCoordinate + ", Scale: " + scale + ", " + mapLut.Rows.Count + " maps found to process for processid :" + processId);
                        for (i = 0; i < mapLut.Rows.Count; i++)
                        {
                            //Micro Transaction makes hte App chatty. May be we should bulk update everything and then do the export and bulk update fro idle state.
                            mapLut.Rows[i][MapLookUpTable.MapExportState] = ExportState.Processing;
                            DatabaseManager.UpdateLookUpTable(mapLut);
                            //Do export here
                            List<string> errorList = new List<string>();
                            PGEExportData exportData = new PGEExportData(DatabaseManager, processId);
                            _logger.Debug("=======================================================================");
                            _logger.Debug("Exporting Map: " + (i + 1) + " of " + mapLut.Rows.Count);
                            _logger.Debug("=======================================================================");
                            errorList = exportData.ProcessRow(mapLut.Rows[i], MapLookUpTable);
                            if (errorList.Count > 0)
                            {
                                mapLut.Rows[i][MapLookUpTable.MapExportState] = ExportState.Failed;
                                DatabaseManager.UpdateLookUpTable(mapLut);
                                //Capture the errors
                                retVal.AddRange(errorList);
                            }
                            else
                            {
                                mapLut.Rows[i][MapLookUpTable.MapExportState] = ExportState.Idle;
                                DatabaseManager.UpdateLookUpTable(mapLut);
                                _logger.Error(string.Join(Environment.NewLine, errorList.ToArray()));
                            }
                            RestartDueToMemory(processId);
                        }
                    }
                    catch (Exception ex)
                    {
                        mapLut.Rows[i][MapLookUpTable.MapExportState] = ExportState.Failed;
                        DatabaseManager.UpdateLookUpTable(mapLut);
                        retVal.Add(ex.Message);
                        retVal.Add(ex.StackTrace);
                        _logger.Error("Failed processing", ex);
                    }
                }
            }
            return retVal;
        }

        #endregion Public Methods


        #region Private Methods

        /// <summary>
        /// Determines whether the memory threshold of the process has been reached.
        /// </summary>
        /// <returns></returns>
        private bool RestartDueToMemory(int processID)
        {
            Process currentProcess = System.Diagnostics.Process.GetCurrentProcess();
            long totalBytesOfMemoryUsed = currentProcess.PeakWorkingSet64;
            if (((totalBytesOfMemoryUsed / 1028) / 1000) > 800)
            {
                //Memory allocation is too high.  Need to restart this process.
                //Log("Memory Usage: " + ((totalBytesOfMemoryUsed / 1028) / 1000) + "MB");
                //Log("Memory usage is higher than configured value.  Restarting process");
                _logger.Debug("Memory threshold reached.  Restarting process");
                Process.GetCurrentProcess().Kill();

                return true;
            }
            else
            {
                //Log("Memory Usage: " + ((totalBytesOfMemoryUsed / 1028) / 1000) + "MB");
            }

            return false;
        }

        #endregion
    }
}
