using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using PGE.Common.Delivery.Diagnostics;
using System.Reflection;
using PGE.Common.Delivery.Systems.Data;
using PGE.Common.Delivery.Systems.Data.Oracle;
using PGE_DBPasswordManagement;
using PGE.BatchApplication.PreProcessor;

namespace PGE.BatchApplication.PSPSMapProduction.Export
{
    /// <summary>
    /// Query Data from existing SDE feature class
    /// </summary>
    public class PGEInputData
    {
        int ProcessID = 1;
        #region Private Fields
        private static Log4NetLogger _logger = new Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "\\PGE.BatchApplication.PSPSMapProduction1.0.log4net.config", "PSPSMapProduction");
        private IMPGDBLayerManager _layerManager = null;
        /// <summary>
        /// SDE Connection File
        /// </summary>
        private string _sdeConnectionFile = null;
        /// <summary>
        /// Conductor Feature Class Name
        /// </summary>
        private string _conductorFeatureClassName = null;
        /// <summary>
        /// Map rows database connection
        /// </summary>
        private IDatabaseConnection _dbConnection = null;
        /// <summary>
        /// DB Data manager
        /// </summary>
        private IMPDBDataManager _dbInteraction = null;
        private Dictionary<string, string> _circuitFeederIds = null;
        private DataTable _feederLookupTable = null;

        /// <summary>
        /// PDF DIP retrieved from config file
        /// </summary>
        private int _pdfdpi = 0;

        #endregion Private Fields

        #region Constructor

        /// <summary>
        /// Initialises the new instance of <see cref="PGEExportData"/>
        /// </summary>
        /// <param name="dataManager"></param>
        public PGEInputData(int processID)
        {
            this.ProcessID = processID;
        }

        #endregion Constructor

        #region Public Methods

        /// <summary>
        /// Returns an instance of PGEMapLookUpTable
        /// </summary>
        public IMPMapLookUpTable MapLookUpTable
        {
            get { return new PGEMapLookUpTable(); }
        }

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

        /// <summary>
        /// New instance of DBDataManager object
        /// </summary>
        public Dictionary<string, string> CircuitFeederIds
        {
            get
            {
                if (_circuitFeederIds == null)
                {
                    BuildFeederCircuitDict(_feederLookupTable);
                }
                return _circuitFeederIds;
            }
            set
            {
                _circuitFeederIds = value;
            }
        }

        /// <summary>
        /// Search all map rows for Circuit Ids
        /// </summary>
        /// <param name="circuitIds">Circuit Ids</param>
        /// <returns>PSPS Map Segment rows</returns>
        public List<PSPSSegmentRecord> SearchPSPSMapRows(string circuitIds) 
        {
            _layerManager = new GDBLayerManager(EDERSDEConnectionFile, ConductorFeatureClassName);

            //if (!String.IsNullOrEmpty(circuitIds))
            //{
            //    // query Feeder by table
            //    if (_feederLookupTable == null)
            //    {
            //        _feederLookupTable = DatabaseManager.GetFeederLookUpTable();
            //    }
            //    List<string> childCircuitIds = FindChildCircuitIds(CircuitFeederIds, circuitIds);
            //    childCircuitIds.Insert(0, circuitIds);
            //    circuitIds = String.Join(",", childCircuitIds.ToArray());
            //}

            // extract all data
            List<PSPSSegmentRecord> pspsMapRows = _layerManager.GetLayerData("CIRCUITID,CIRCUITNAME,PSPS_SEGMENT", circuitIds);

            return pspsMapRows;
        }

        /// <summary>
        /// Delete map rows to database map table for given circuit ids
        /// </summary>
        /// <param name="circuitIds">Circuit Ids  rows</param>
        /// <returns>number rows deleted</returns>
        public int DeletePSPSMapRows(string circuitIds)
        {
            int numDeletedRows = 0;
            if (circuitIds != "")
            {
                string deleteCommand = String.Format("DELETE from {0} where CIRCUITID in ('{1}')", MapLookUpTable.TableName, String.Join("','", circuitIds.Split(',')));
      
                numDeletedRows = DatabaseManager.DatabaseConnection.ExecuteNonQuery(deleteCommand);

            }
            return numDeletedRows;
        }

        /// <summary>
        /// Add all map rows to database map table
        /// </summary>
        /// <param name="mapRows">Circuit map rows</param>
        /// <returns>status for adding process</returns>
        public int AddPSPSMapRows(List<PSPSSegmentRecord> mapRows)
        {
            // query Feeder by circuit ids
            // query Feeder by table
            if (_feederLookupTable == null)
            {
                _feederLookupTable = DatabaseManager.GetFeederLookUpTable();
            }


            DataTable dataTable = new DataTable(MapLookUpTable.TableName);
            DataRow dataRow = null;
            string currentDate = DateTime.Now.Date.ToString("dd-MMM-yyyy");

            // sort the list by CircuitName and PSPSName
            /* Commented for EGIS-1122 PSPS Segment application is not merging the PDFs automatically
            mapRows = mapRows.OrderBy(x => x.CircuitId)
                                    .ThenBy(x => x.PSPSSegmentName)
                                    .ToList();
            */
            mapRows = mapRows.OrderBy(x => x.PSPSSegmentName) // Changes made for EGIS-1122 PSPS Segment application is not merging the PDFs automatically
                                    .ThenBy(x => x.CircuitId)
                                    .ToList();

            // merge map rows
            for (int i = 0; i < mapRows.Count; i++)
            {
                PSPSSegmentRecord mapRow = mapRows[i];
                if (mapRow.PSPSSegmentName == "" && mapRow.MapType == 2)
                {
                    // find child circuits
                    List<string> childCircuitIds = FindChildCircuitIds(CircuitFeederIds, mapRow.CircuitId);
                    if (childCircuitIds.Count > 0)
                    {
                        MergePSPSRecord(mapRows, mapRow, childCircuitIds);
                    }
                }
            }

            // convert map rows to data table
            for (int i = 0; i < mapRows.Count; i++) 
            {
                PSPSSegmentRecord mapRow = mapRows[i];
                if (i == 0)
                {
                    foreach (string colName in MapLookUpTable.AllColumns) {
                        dataTable.Columns.Add(colName);
                    }
                }
                if (mapRow.deleted) continue;

                dataRow = dataTable.NewRow();

                foreach (string colName in MapLookUpTable.AllColumns)
                {
                    if (colName == MapLookUpTable.XMin) dataRow[colName] = mapRow.Extent.XMin;
                    else if (colName == MapLookUpTable.XMax) dataRow[colName] = mapRow.Extent.XMax;
                    else if (colName == MapLookUpTable.YMin) dataRow[colName] = mapRow.Extent.YMin;
                    else if (colName == MapLookUpTable.YMax) dataRow[colName] = mapRow.Extent.YMax;
                    else if (colName == MapLookUpTable.MapId) dataRow[colName] = DatabaseManager.GetNextSequence("EDGIS.PGE_PSPSMAPROWS_MAPID_SEQ");
                    else if (colName == MapLookUpTable.LayoutType) dataRow[colName] = mapRow.LayoutIndex;
                    else if (colName == MapLookUpTable.CircuitId) dataRow[colName] = mapRow.CircuitId;
                    else if (colName == MapLookUpTable.FeederCircuitId) dataRow[MapLookUpTable.FeederCircuitId] = mapRow.FeederCircuitId;
                    else if (colName == MapLookUpTable.ChildCircuitName) dataRow[MapLookUpTable.ChildCircuitName] = mapRow.ChildCircuitName;
                    else if (colName == MapLookUpTable.CircuitName) dataRow[colName] = mapRow.CircuitName;
                    else if (colName == MapLookUpTable.PSPSName) dataRow[colName] = mapRow.PSPSSegmentName;
                    else if (colName == MapLookUpTable.TotalSegmentLength) dataRow[colName] = mapRow.TotalMilage;
                    else if (colName == MapLookUpTable.MapExportState) dataRow[colName] = ExportState.ReadyToExport;
                    else if (colName == MapLookUpTable.ProcessingService) dataRow[colName] = -1;
                    else if (colName == MapLookUpTable.LastModifiedDate) dataRow[colName] = currentDate;
                }

                dataTable.Rows.Add(dataRow);
            }

            int numInsertedRows = DatabaseManager.AddLookUpRows(dataTable);

            return numInsertedRows;
        }

        #endregion Public Methods

        #region Protected Properties

        /// <summary>
        /// SDE Connection File for EDER using sde
        /// </summary>
        protected string EDERSDEConnectionFile
        {
            get
            {
                if (_sdeConnectionFile == null)
                {
                    _sdeConnectionFile = MapProductionConfigurationHandler.Settings["EDERSDEConnectionFile"];
                }
                return _sdeConnectionFile;
            }
        }

        /// <summary>
        /// Feature class name to search for circuit segments
        /// </summary>
        protected string ConductorFeatureClassName
        {
            get
            {
                if (_conductorFeatureClassName == null)
                {
                    _conductorFeatureClassName = MapProductionConfigurationHandler.Settings["ConductorFeatureClassName"];
                }
                return _conductorFeatureClassName;
            }
        }     

        protected int PDFDPI
        {
            get
            {
                if (_pdfdpi == 0)
                {
                    _pdfdpi = int.TryParse(MapProductionConfigurationHandler.Settings["PDFDPI"], out _pdfdpi) ? _pdfdpi : 300;
                }
                return _pdfdpi;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="circuitId"></param>
        /// <returns></returns>
        private void BuildFeederCircuitDict(DataTable fcTable)
        {
            if (_circuitFeederIds == null)
            {
                _circuitFeederIds = new Dictionary<string, string>();

                foreach (DataRow drow in fcTable.Rows)
                {
                    string currCircuitId = drow["CIRCUITID"].ToString();
                    string feederId = drow["FEEDER_CIRCUIT"].ToString();
                    while (feederId.Length < 9)
                    {
                        feederId = "0" + feederId;
                    }
                    while (currCircuitId.Length < 9)
                    {
                        currCircuitId = "0" + currCircuitId;
                    }
                    if (!_circuitFeederIds.ContainsKey(currCircuitId))
                    {
                        _circuitFeederIds.Add(currCircuitId, feederId);
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="circuitId"></param>
        /// <returns></returns>
        private string FindFeederCircuitId(DataTable fcTable, string circuitId)
        {
            if (_circuitFeederIds == null)
            {
                _circuitFeederIds = new Dictionary<string, string>();

                foreach (DataRow drow in fcTable.Rows)
                {
                    string currCircuitId = drow["CIRCUITID"].ToString();
                    string feederId = drow["FEEDER_CIRCUIT"].ToString();
                    while (feederId.Length < 9)
                    {
                        feederId = "0" + feederId;
                    }
                    while (currCircuitId.Length < 9)
                    {
                        currCircuitId = "0" + currCircuitId;
                    }
                    if (!_circuitFeederIds.ContainsKey(currCircuitId))
                    {
                        _circuitFeederIds.Add(currCircuitId, feederId);
                    }
                }
            }
            if (_circuitFeederIds.ContainsKey(circuitId))
            {
                return _circuitFeederIds[circuitId];
            }
            else
            {
                return "";
            }
        }

        /// <summary>
        /// Find all child circuits for a given Circuit Id
        /// </summary>
        /// <param name="feederCircuitDict">FeederFed by mapping</param>
        /// <param name="circuitIds">Given feedfed by circuit Id</param>
        /// <returns></returns>
        private List<string> FindChildCircuitIds(Dictionary<string, string> feederCircuitDict, string circuitId)
        {
            List<string> childCircuits = new List<string>();
            foreach (string key in feederCircuitDict.Keys)
            {
                if (feederCircuitDict[key].Equals(circuitId) && !key.Equals(circuitId))
                {
                    childCircuits.Add(key);
                }
            }

            return childCircuits;
        }

        private void MergePSPSRecord(List<PSPSSegmentRecord> mapRows, PSPSSegmentRecord mapRow, List<string> childCircuitIds)
        {
            for (int i = 0; i < mapRows.Count; i++)
            {
                if (childCircuitIds.Contains(mapRows[i].CircuitId))
                {
                    if (mapRows[i].MapType == 2)
                    {
                        mapRow.Extent.Union(mapRows[i].Extent);
                        mapRow.TotalMilage += mapRows[i].TotalMilage;
                        mapRows[i].deleted = true;
                    }
                    else
                    {
                        mapRows[i].FeederCircuitId = mapRow.CircuitId;
                        mapRows[i].ChildCircuitName = mapRows[i].CircuitName;
                        mapRows[i].CircuitName = mapRow.CircuitName;
                    }
                }
            }
        }

        #endregion Protected Properties

    }
}
