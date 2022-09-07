using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PGE.Common.Delivery.Systems.Data;
using PGE.Common.Delivery.Diagnostics;
using PGE.Common.Delivery.Systems.Configuration;
using System.Data;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.DataSourcesGDB;
using ESRI.ArcGIS.esriSystem;
using System.Runtime.InteropServices;
using System.Reflection;

namespace PGE.BatchApplication.PSPSMapProduction
{
    /// <summary>
    /// Base implementation  of Core Processor Interface. Provides ability to process the Map Production Tool set
    /// </summary>
    public abstract class BaseMapPreProcessor:IMPProcessor,IDisposable
    {
        private static Log4NetLogger _logger = new Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "\\PGE.BatchApplication.PSPSMapProduction1.0.log4net.config", "PSPSMapProduction");
        private IWorkspace _workspace = null;
        private const string _successMessage = "Successfully processed maps";
        private Dictionary<string, DataRow> mapLutDict = new Dictionary<string, DataRow>();

        /// <summary>
        /// Stores the current Date of pre-processing and the same is uploaded to the MapNumberCoordLut table
        /// </summary>
        private DateTime _currentDate;

        /// <summary>
        /// Constructor
        /// </summary>
        public BaseMapPreProcessor()
        {
            
        }

        #region IMPProcessor Implementaiton
        /// <summary>
        /// Executes the core logic to process data. Based on the ExportType specified 
        /// Gets the Datatable of MapLookUp Table using the DatabaseManager property and calls the ProcessBulk/ProcessDifference/ProcessMaps virtual methods by passing in the datatable 
        /// </summary>
        /// <param name="exportType">ExportType to use <see cref="ExportType"/>. If the Passed in ExportType is Difference and the ChangeDetector is not set will execute the Bulk logic</param>
        /// <param name="keyData">Key Data to filter the data processed</param>
        /// <returns></returns>
        public virtual List<string> Process(ExportType exportType, int totalNumberOfExporters = 0, params string[] keyFieldValue)
        {
            List<string> retVal = new List<string>();
            DataTable mapLut = null;
            try
            {
                if (keyFieldValue.Length > 1)
                {
                    mapLut = DatabaseManager.GetMapLookUpTable(keyFieldValue[1]);
                }
                else
                {
                    mapLut = DatabaseManager.GetMapLookUpTable();
                }

                _logger.Debug("Totla number of map records in staging table:" + mapLut.Rows.Count);
                //mapLut.AcceptChanges();
                if (MapLookUpTable.VerifyLookUpTable(mapLut))
                {
                    switch (exportType)
                    {
                        case ExportType.Bulk:
                            retVal = ProcessBulk(mapLut);
                            break;
                        case ExportType.Maps:
                            if (keyFieldValue == null || keyFieldValue.Length == 0)
                                throw new NullReferenceException("Map number is required for ExportType:Maps");
                            retVal = ProcessMaps(mapLut, keyFieldValue);
                            break;
                    }
                }
                else
                {
                    throw new NotSupportedException("Map Look Up table:" + MapLookUpTable.TableName + " does not have all the columns required.");
                }
            }
            catch (Exception ex)
            {
                retVal.Add(ex.Message + Environment.NewLine + ex.StackTrace);
            }
            if (retVal.Count == 0)
            {
                retVal.Add(SuccessMessage);
                //Mark the Exporter to Process here before saving to avoid hitting the database multiple times.
                //Saving the bulk of data to database with close to 40000 records takes close to 10 minutes.
                mapLut = DatabaseManager.GetMapLookUpTable("where " + MapLookUpTable.MapExportState + "=" + ExportState.ReadyToExport);
                RecordsProcessed = SetProcessInfo(mapLut, totalNumberOfExporters);
            }
            return retVal;
        }

        /// <summary>
        /// Sets the Process integer on the Mapping table and makes it ready for being processed by the Exporters.
        /// All it does is take the number of processes and the number number of records to processed and divides the total number of records to be processed equally between the processes.
        /// </summary>
        /// <param name="noOfProcesses">Total number of Processes.</param>
        /// <returns>Total number of rows affected</returns>
        public virtual int SetProcessInfo(int noOfProcesses=1)
        {
            //If noOfProcesses sent is 0 default it to 1
            if (noOfProcesses == 0)
            {
                noOfProcesses = 1;
            }
            int retVal = 0;
            DataTable maplut = DatabaseManager.GetMapLookUpTable("where " + MapLookUpTable.MapExportState + "=" + ExportState.ReadyToExport);
            retVal = maplut.Rows.Count;
            if (maplut.Rows.Count > 0 && CircuitsProcessed > 0)
            {
                int quotient = 0;
                int numberPerProcess = Math.DivRem(CircuitsProcessed, noOfProcesses, out quotient);
                
                int cnt = 0;
                int processor = 1;
                if (numberPerProcess > 0)
                {                    
                    for (int i = 1; i <= noOfProcesses; i++)
                    {
                        for (int j = 1; j <= numberPerProcess; j++)
                        {
                            maplut.Rows[cnt][MapLookUpTable.ProcessingService] = processor;
                            processor=processor<noOfProcesses?processor+1:1;
                            cnt++;
                        }
                    }
                    
                }
                if (quotient > 0)
                {
                    for (int i = 1; i <= quotient; i++)
                    {
                        maplut.Rows[cnt][MapLookUpTable.ProcessingService] = processor;
                        processor = processor < noOfProcesses ? processor + 1 : 1;
                        cnt++;
                    }
                }
                DatabaseManager.UpdateLookUpTable(maplut);
            }
            return retVal;
        }

        /// <summary>
        /// Total Number of records processed by this processor
        /// </summary>
        public int RecordsProcessed
        {
            get;
            protected set;
        }
        /// <summary>
        /// Total Number of Circuit records processed by this processor
        /// </summary>
        public int CircuitsProcessed
        {
            get;
            protected set;
        }
        /// <summary>
        /// The Database Manager object to use to fetch the Look up table information. Abstract Property
        /// </summary>
        public abstract IMPDBDataManager DatabaseManager
        {
            get;
            protected set;
        }
        /// <summary>
        /// The Geodatabase Manager to use to fetch the Map Polygon feature information. Abstract Property
        /// </summary>
        public abstract IMPGDBDataManager GeodatabaseManager
        {
            get;
        }
        /// <summary>
        /// Database Connection to use to access Look Up Tables. Abstract Property
        /// </summary>
        public abstract IDatabaseConnection DatabaseConnection
        {
            get;
        }
        /// <summary>
        /// Workspace to use to get the Map Polygon featureclass. Virtual method. Calls the Protected Abstract method InitializeWorkspace
        /// </summary>
        public virtual IWorkspace Workspace
        {
            get
            {
                if (_workspace == null)
                {
                    _workspace=InitializeWorkspace();
                }
                return _workspace;
            }
        }
        /// <summary>
        /// Geodatabase Field Lookup to use to get data from Geodatabase. Abstract Property
        /// </summary>
        public abstract IMPGDBFieldLookUp GDBFieldLookUp
        {
            get;
        }
        /// <summary>
        /// Map Look up table to use for getting Lookup table information and data. Abstract Property
        /// </summary>
        public abstract IMPMapLookUpTable MapLookUpTable
        {
            get;
        }

        /// <summary>
        /// The SuccessMessage to be used if the Map Processing completes successfully.
        /// Returns "Successfully processed maps"
        /// </summary>
        public virtual string SuccessMessage
        {
            get
            {
                return _successMessage;
            }
        }
        #endregion

        #region Protected Methods

        #region LookUp Table updaters
        ///// <summary>
        /// Process every record from the Geodtabase and inserts/updates records in the Datatable also sets the ExportState to ReadyToExport for all the records that were modified or inserted.
        /// </summary>
        /// <param name="mapLut">The Map Look Up Table data</param>
        /// <returns></returns>
        protected virtual List<string> ProcessBulk(DataTable mapLut)
        {
            List<string> retVal = new List<string>();
            _logger.Debug("Procesisng the data with IGDBData");
            retVal = ProcessData(mapLut);
            _logger.Debug("Completed processing data");
            return retVal;
        }
        /// <summary>
        /// Process record from the Geodtabase and inserts/updates records in the Datatable also sets the ExportState to ReadyToExport for all the records that were modified or inserted.
        /// The Records processed are filtered using the keyFieldValue Parameter
        /// </summary>
        /// <param name="mapLut">The Map Look Up Table data</param>
        protected virtual List<string> ProcessMaps(DataTable mapLut, string[] keyFieldValue)
        {
            List<string> retVal = new List<string>();
            if (mapLut != null)
            {
                //DataTable dbDataTable = DatabaseManager.GetMapLookUpTable(keyFieldValue);
                retVal = ProcessData(mapLut);
            }
            return retVal;
        }

        /// <summary>
        /// Given the DataTable and the IMPGDBData will process every record in the gdbDataList and will insert a record into the DataTable if the record is not found or update the record and set the ExportState to ReadyToExport in the DtaTable.
        /// </summary>
        /// <param name="mapLut">Datatable that holds information from the MapLookUpTable. If there are not DataRow in the table for a given gdbData a new record is created</param>
        /// <param name="gdbDataList">List of IMPGDBData to mark as modified or to insert</param>
        /// <returns></returns>
        protected virtual List<string> ProcessData(DataTable mapLut)
        {
            //return ProcessDataByCaching(mapLut, gdbDataList); 
            _logger.Debug("In Process Data");
            Console.WriteLine("Starting update of rows:" + System.DateTime.Now);
            List<string> retVal = new List<string>();
            List<string> circuitIds = new List<string>();

            try
            {
                //Set the current Date as LastModifiedDate for the MapNumberCoordLut records that will be updated
                _currentDate = DateTime.Now.Date;

                foreach (DataRow dbData in mapLut.Rows)
                {
                    if (circuitIds.IndexOf(dbData[MapLookUpTable.CircuitId].ToString()) < 0)
                    {
                        if (!String.IsNullOrEmpty(dbData[MapLookUpTable.FeederCircuitId].ToString()))
                        {
                            if (circuitIds.IndexOf(dbData[MapLookUpTable.FeederCircuitId].ToString()) < 0)
                            {
                                circuitIds.Add(dbData[MapLookUpTable.FeederCircuitId].ToString());
                            }
                        }
                        else if (circuitIds.IndexOf(dbData[MapLookUpTable.CircuitId].ToString()) < 0)
                        {
                            circuitIds.Add(dbData[MapLookUpTable.CircuitId].ToString());
                        }
                    }
                    dbData.BeginEdit();
                    //_logger.Debug("Updating datarow for mapnumber:" + gdbData.MapNumber);
                    UpdateDataRow(dbData);
                    dbData.EndEdit();
                }
                DatabaseManager.UpdateLookUpTable(mapLut);
                CircuitsProcessed = circuitIds.Count;
            }
            catch (Exception ex)
            {
                retVal.Add(ex.Message);
                retVal.Add(ex.StackTrace);
            }
            Console.WriteLine("Finished update of rows:" + System.DateTime.Now);
            return retVal;
        }
        /// <summary>
        /// Given the DataTable and the IMPGDBData will process every record in the gdbDataList and will insert a record into the DataTable if the record is not found or update the record and set the ExportState to ReadyToExport in the DtaTable.
        /// The difference between this method and ProcessData method is the DataRows are cached into a dictionary before being processed in this method to increase the performance of processing these records.
        /// </summary>
        /// <param name="mapLut">Datatable that holds information from the MapLookUpTable. If there are not DataRow in the table for a given gdbData a new record is created</param>
        /// <param name="gdbDataList">List of IMPGDBData to mark as modified or to insert</param>
        /// <returns></returns>
        protected virtual List<string> ProcessDataByCaching(DataTable mapLut, List<IMPGDBData> gdbDataList)
        {
            List<string> retVal = new List<string>();
            //Cache Data into Dictionary first
            //Datatable.Select takes a long time and we cannot call AcceptChanges before updating the table.
            foreach (DataRow dr in mapLut.Rows)
            {
                mapLutDict.Add(GetDatatableWhereClause(dr), dr);
            }
            try
            {
                //Set the current Date as LastModifiedDate for the MapNumberCoordLut records that will be updated
                _currentDate = DateTime.Now.Date;

                DataRow dr = null;
                string whereClause=string.Empty;
                foreach (IMPGDBData gdbData in gdbDataList)
                {
                    if (gdbData.IsValid)
                    {
                        //drArray = mapLut.Select(GetDatatableWhereClause(gdbData));
                        //if (drArray != null && drArray.Count() > 0)
                        //{
                        whereClause=GetDatatableWhereClause(gdbData);
                        if(mapLutDict.ContainsKey(whereClause))
                        {
                            //foreach (DataRow dr in drArray)
                            //{
                            dr = mapLutDict[whereClause];
                                dr.BeginEdit();
                                UpdateDataRow(dr, gdbData);
                                dr.EndEdit();
                            //}
                        }
                        else
                        {
                            dr = mapLut.NewRow();
                            dr.BeginEdit();
                            UpdateDataRow(dr, gdbData);
                            mapLut.Rows.Add(dr);
                            dr.EndEdit();
                            //string currValue = dr[0, DataRowVersion.Current].ToString();
                            //currValue = currValue;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                retVal.Add(ex.Message);
                retVal.Add(ex.StackTrace);
            }
            return retVal;
        }

        /// <summary>
        /// Sets the Process integer on the Mapping table and makes it ready for being processed by the Exporters.
        /// All it does is take the number of processes and the number number of records to processed and divides the total number of records to be processed equally between the processes.
        /// </summary>
        /// <param name="maplut">Datatable that should be processed to set the service number that should process</param>
        /// <param name="noOfProcesses">Total number of Processes.</param>
        /// <returns>Total number of rows affected</returns>
        protected virtual int SetProcessInfo(DataTable maplut, int noOfProcesses = 1)
        {
            //If noOfProcesses sent is 0 default it to 1
            _logger.Debug("Setting ServiceToProcess");
            Console.WriteLine("Organizing services to process :" + System.DateTime.Now);
            if (noOfProcesses == 0)
            {
                noOfProcesses = 1;
            }
            int retVal = 0;

            retVal = CircuitsProcessed;
            List<string> curcuitIds = new List<string>();

            if (maplut.Rows.Count > 0)
            {
                int quotient = 0;
                int numberPerProcess = Math.DivRem(CircuitsProcessed, noOfProcesses, out quotient) + 1;

                int cnt = 0;
                int processor = 1;
                string prevCircuitId = "";
                if (numberPerProcess > 0)
                {
                    prevCircuitId = "";
                    for (int i = 0; i < maplut.Rows.Count; i++)
                    {
                        string currCircuitId = maplut.Rows[i][MapLookUpTable.FeederCircuitId].ToString();
                        if (String.IsNullOrEmpty(currCircuitId))
                        {
                            currCircuitId = maplut.Rows[i][MapLookUpTable.CircuitId].ToString();
                        }
                        if (prevCircuitId != "" && !prevCircuitId.Equals(currCircuitId))
                        {
                            processor = processor < noOfProcesses ? processor + 1 : 1;
                        }
                        maplut.Rows[i].BeginEdit();
                        maplut.Rows[i][MapLookUpTable.ProcessingService] = processor;
                        maplut.Rows[i].EndEdit();

                        prevCircuitId = currCircuitId;
                    }
                }

                DatabaseManager.UpdateLookUpTable(maplut);
            }
            Console.WriteLine("Finished organizing:" + System.DateTime.Now);
            return retVal;
            
        }
        #endregion

        /// <summary>
        /// Gets the WhereClause to use for the given IMPGDBData on the DataTable.
        /// uses MapLUTGDBFieldLookUp property to get the MapLookUpTable whereclause from the IMPGDBData
        /// </summary>
        /// <param name="gdbData">IMPGDBData</param>
        /// <returns></returns>
        protected virtual string GetDatatableWhereClause(IMPGDBData gdbData)
        {
            string retVal = string.Empty;
            retVal = MapLookUpTable.MapId + "=" + gdbData.MapId.ToString();
            string s = string.Empty;
            foreach(string keyField in MapLookUpTable.KeyFields)
            {
                //s= s + " and "+ keyField + "='" + gdbData.KeyFieldValues[MapLUTGDBFieldLookUp[keyField]] + "'";
            }
            retVal = retVal + s;
            return retVal;
        }

        protected virtual string GetDatatableWhereClause(DataRow row)
        {
            string retVal = string.Empty;
            retVal = ""; // MapLookUpTable.MapNumber + "='" + row[MapLookUpTable.MapNumber] + "'";
            string s = string.Empty;
            foreach (string keyField in MapLookUpTable.KeyFields)
            {
                s = s + " and " + keyField + "='" + row[keyField]+ "'";
            }
            retVal = retVal + s;
            return retVal;
        }
        /// <summary>
        /// Given a DataRow will update the DataRow with values from IMPGDBData
        /// </summary>
        /// <param name="dr">Datarow from teh IMPMapLookUpTable</param>
        /// <param name="gdbData">IMPGDBData</param>
        protected virtual void UpdateDataRow(DataRow dr)
        {
            dr[MapLookUpTable.MapExportState] = ExportState.ReadyToExport;
            dr[MapLookUpTable.ProcessingService] = -1;
            dr[MapLookUpTable.LastModifiedDate] = _currentDate;
        }

        /// <summary>
        /// Given a DataRow will update the DataRow with values from IMPGDBData
        /// </summary>
        /// <param name="dr">Datarow from teh IMPMapLookUpTable</param>
        /// <param name="gdbData">IMPGDBData</param>
        protected virtual void UpdateDataRow(DataRow dr, IMPGDBData gdbData)
        {
            dr[MapLookUpTable.XMin] = gdbData.XMin;
            dr[MapLookUpTable.XMax] = gdbData.XMax;
            dr[MapLookUpTable.YMin] = gdbData.YMin;
            dr[MapLookUpTable.YMax] = gdbData.YMax;

            dr[MapLookUpTable.MapExportState] = ExportState.ReadyToExport;
            dr[MapLookUpTable.ProcessingService] = -1;
            dr[MapLookUpTable.LastModifiedDate] = _currentDate;
        }

        /// <summary>
        /// Abstract method to initialize the IWorkspace to look for Map Polygon featureclass.
        /// </summary>
        /// <returns></returns>
        protected abstract IWorkspace InitializeWorkspace();
        #endregion

        #region IDsposable Implementation
        /// <summary>
        /// Disposes all the persisted objects
        /// </summary>
        public void Dispose()
        {
            try
            {
                if (_workspace != null)
                {
                    Marshal.ReleaseComObject(_workspace);
                    _workspace = null;
                }
                if (DatabaseConnection != null)
                {
                    if (DatabaseConnection.IsOpen) DatabaseConnection.Close();
                    DatabaseConnection.Dispose();
                }
                if (DatabaseManager != null)
                {
                    DatabaseManager = null;
                }
            }
            catch { }
        }
        #endregion

    }
}
