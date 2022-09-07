using System;
using System.Collections.Generic;
using PGE.Common.Delivery.Systems.Data;
using PGE.Common.Delivery.Systems.Data.Oracle;
using System.Data;
using PGE.Common.Delivery.Diagnostics;
using System.Reflection;
using System.Diagnostics;
using System.IO;
using PGE_DBPasswordManagement;
using Microsoft.PowerBI.Api.Models;
using PGE.BatchApplication.PSPSMapProduction.PreProcessor;
using PGE.BatchApplication.PreProcessor;

namespace PGE.BatchApplication.PSPSMapProduction.Export
{
    /// <summary>
    /// Executes the export operation by Process ID
    /// </summary>
    public class PGEExporter : IMPExporter
    {
        private static Log4NetLogger _logger = new Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "\\PGE.BatchApplication.PSPSMapProduction1.0.log4net.config", "PSPSMapProduction");

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
                    _dbConnection = new OracleDatabaseConnection(ReadEncryption.GetConnectionStr(MapProductionConfigurationHandler.OracleConnectionSting));
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
        /// <param name="circuitId">circuit ID</param>
        /// <param name="circuitName">circuit name</param>
        /// <param name="mergePDF">only merge when build PDF for one circuit</param>
        /// <returns>Returns the list of errors encountered, if any, while performing the operation</returns>
        public List<string> Export(int processId, string circuitId, string circuitName, bool mergePDF)
        {

            List<string> retVal = new List<string>();

            if (!DatabaseConnection.IsOpen)
            {
                DatabaseConnection.Open();
            }

            //For each new scale / coordinate combination we need to tell the export process that it will
            //need to reload its mxd
            string whereClause = "where " + MapLookUpTable.ProcessingService + "=" + processId;
            if (circuitId != "")
            {
                whereClause += String.Format(" and (CIRCUITID = '{0}' or FEEDER_CIRCUITID = '{0}')", circuitId);
            }

            DataTable mapLut = DatabaseManager.GetMapLookUpTable(whereClause + " and " + MapLookUpTable.MapExportState + "=" + ExportState.ReadyToExport);

            int i = 0;
            string updateSql = "";

            try
            {
                string prevCircuitId = "";
                string prevCircuitName = "";
                string mergePdfsPy = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "MergePdfs.py");
                PGEMergePDFs pdfMerger = new PGEMergePDFs();
                PGEExportData exportData = new PGEExportData(DatabaseManager, processId);

                // update all rows with status of InProcessing
                for (i = 0; i < mapLut.Rows.Count; i++)
                {
                    mapLut.Rows[i][MapLookUpTable.MapExportState] = ExportState.Processing;
                }
                updateSql = "UPDATE " + DatabaseManager.MapLookUpTable.TableName + " set EXPORTSTATE=" + ExportState.Processing + " " + whereClause;
                DatabaseManager.UpdateLookUpTable(mapLut, updateSql);

                // we need to handle multiple circuit Ids in single process number
                for (i = 0; i < mapLut.Rows.Count; i++)
                {
                    string tmpCircuitId = mapLut.Rows[i][MapLookUpTable.CircuitId].ToString();
                    string tmpFeederCircuitId = mapLut.Rows[i][MapLookUpTable.FeederCircuitId].ToString();
                    string tmpCircuitName = mapLut.Rows[i][MapLookUpTable.CircuitName].ToString();
                    string tmpPSPSName = mapLut.Rows[i][MapLookUpTable.PSPSName].ToString();
                    string parentCircuitId = tmpCircuitId;
                    if (!String.IsNullOrEmpty(tmpFeederCircuitId))
                    {
                        parentCircuitId = tmpFeederCircuitId;
                        string feederCircuitName = FindFeederCircuitName(mapLut, tmpFeederCircuitId);
                        if (feederCircuitName != "")
                        {
                            tmpCircuitName = feederCircuitName;
                        }
                    }
                    List<string> childCircuitIds = FindChildCircuitIds(mapLut.Rows, parentCircuitId);

                    _logger.Debug("Circuit Id: " + tmpCircuitId + ", Circuit Name: " + tmpCircuitName + ", " + mapLut.Rows.Count + " maps found to process for processid :" + processId);

                    string tempPDFDirectory = GetTemporaryCircuitMapFolder(exportData.PDFLocation, tmpCircuitName);
                    string childCircuits = (childCircuitIds.Count > 0) ? String.Join(",", childCircuitIds.ToArray()) : "";
                    //Do export here
                    List<string> errorList = new List<string>();
                    _logger.Debug("=======================================================================");
                    _logger.Debug("Exporting Map: " + (i + 1) + " of " + mapLut.Rows.Count);
                    _logger.Debug("=======================================================================");
                    errorList = exportData.ProcessRow(mapLut.Rows[i], MapLookUpTable, tempPDFDirectory, childCircuits);
                    if (errorList.Count > 0)
                    {
                        updateSql = "UPDATE " + DatabaseManager.MapLookUpTable + " set EXPORTSTATE='" + ExportState.Failed + "' " + whereClause + " and MAPID=" + mapLut.Rows[i][MapLookUpTable.MapId].ToString();
                        DatabaseManager.UpdateLookUpTable(mapLut, updateSql);
                        //Capture the errors
                        retVal.AddRange(errorList);
                    }
                    else
                    {
                        updateSql = "UPDATE " + DatabaseManager.MapLookUpTable + " set EXPORTSTATE='" + ExportState.Idle + "' " + whereClause + " and MAPID=" + mapLut.Rows[i][MapLookUpTable.MapId].ToString();
                        DatabaseManager.UpdateLookUpTable(mapLut, updateSql);
                        _logger.Error(string.Join(Environment.NewLine, errorList.ToArray()));
                    }
                    RestartDueToMemory(processId);

                    prevCircuitId = tmpCircuitId;
                    prevCircuitName = tmpCircuitName;
                }
                // merge all pdf files 
                if (mergePDF)
                {
                    pdfMerger.MergePDF(mergePdfsPy, exportData.PDFLocation);
                }

            }
            catch (Exception ex)
            {
                updateSql = "UPDATE " + DatabaseManager.MapLookUpTable + " set EXPORTSTATE='" + ExportState.Failed + "' " + whereClause + " and MAPID=" + mapLut.Rows[i]["MAPID"].ToString();
                DatabaseManager.UpdateLookUpTable(mapLut, updateSql);
                retVal.Add(ex.Message);
                retVal.Add(ex.StackTrace);
                _logger.Error("Failed processing", ex);
            }
            finally
            {
                if (DatabaseConnection.IsOpen)
                {
                    DatabaseConnection.Close();
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
                _logger.Debug("Memory threshold reached.  Restarting process");
                //Process.GetCurrentProcess().Kill();

                return true;
            }
 
            return false;
        }

        private string GetTemporaryCircuitMapFolder(string pdfLocation, string circuitName)
        {
            //string tempPDFDirectory = System.IO.Path.GetTempPath();
            string tempPDFDirectory = System.IO.Path.Combine(pdfLocation, circuitName);
            try
            {
                //Check if temp directory exists.  If not create it.
                if (!Directory.Exists(tempPDFDirectory))
                {
                    Directory.CreateDirectory(tempPDFDirectory);
                }
            }
            catch (Exception e)
            {
                _logger.Error("Unable to create temporary directory: " + tempPDFDirectory);
                throw e;
            }
            return tempPDFDirectory;

        }

        /// <summary>
        /// Find the Circuit Name for the Feeder Circuit ID
        /// </summary>
        /// <param name="mapLut"></param>
        /// <param name="feederCircuitId"></param>
        /// <returns></returns>
        private string FindFeederCircuitName(DataTable mapLut, string feederCircuitId)
        {
            string circuitName = "";
            for (int i = 0; i < mapLut.Rows.Count; i++)
            {
                string tmpCircuitId = mapLut.Rows[i][MapLookUpTable.CircuitId].ToString();
                if (feederCircuitId.Equals(tmpCircuitId))
                {
                    circuitName = mapLut.Rows[i][MapLookUpTable.CircuitName].ToString();
                    break;
                }
            }
            return circuitName;
        }

        private List<string> FindChildCircuitIds(DataRowCollection dataRows, string circuitId)
        {
            List<string> childCircuits = new List<string>();
            foreach (DataRow row in dataRows)
            {
                if (row[MapLookUpTable.FeederCircuitId].ToString().Equals(circuitId))
                {
                    childCircuits.Add(row[MapLookUpTable.CircuitId].ToString());
                }
            }
            if (childCircuits.Count > 0)
            {
                childCircuits.Insert(0, circuitId);
            }
            return childCircuits;
        }
        #endregion
    }
}
