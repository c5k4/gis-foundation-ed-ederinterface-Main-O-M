using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ESRI.ArcGIS.Geodatabase;
using System.Reflection;
using PGE.Common.ChangesManagerShared.Utilities;
using System.Runtime.InteropServices;
using PGE.Common.ChangesManagerShared;
using PGE.Common.Delivery.Diagnostics;

namespace PGE.BatchApplication.ChangeSubscribers
{
    public class GridWriter
    {
        private static readonly Log4NetLogger _logger = new Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "ChangeDetectionDefault.log4net.config");

        private int _idxMapNumber;
        private int _idxChangeDate;
        private int _idxScale;
        private string _changeDate;


        public string OutputTable
        { get; private set; }

        /// <summary>
        /// The name of the field within the output table that indicates the map (grid) number.
        /// </summary>
        public string OutputFieldMapNumber
        { get; private set; }

        /// <summary>
        /// The name of the field within the output table that indicates the changed date.
        /// </summary>
        public string OutputFieldChangeDate
        { get; private set; }

        /// <summary>
        /// The name of the field within the output table that indicates the map scale.
        /// </summary>
        public string OutputFieldScale
        { get; private set; }

        private SDEWorkspaceConnection SDEWorkspace { get; set; }

        private ITable _gridTable = null;
        private ITable GridTable
        {
            get
            {
                _logger.Debug(MethodBase.GetCurrentMethod().Name);

                if (_gridTable == null)
                {
                    try
                    {
                        _gridTable = ((IFeatureWorkspace)SDEWorkspace.NonVersionedEditsWorkspace).OpenTable(OutputTable);
                    }
                    catch (Exception ex)
                    {
                        throw new ErrorCodeException(ErrorCode.OutputTable, "Unable to find the output table.", ex);
                    }
                }
                return _gridTable;
            }
        }


        public GridWriter(SDEWorkspaceConnection sdeWorkspaceConnection, string outputFieldChangeDate, string outputFieldScale, 
            string outputFieldMapNumber, string outputTable)
        {
            _logger.Debug(MethodBase.GetCurrentMethod().Name);
            _logger.Info(String.Format("Grid GDB [ {0} ] FieldChangeDate [ {1} ] FieldScale [ {2} ] FieldMapNum [ {3} ] OutputTable [ {4} ]",
                sdeWorkspaceConnection.WorkspaceConnectionFile, outputFieldChangeDate, outputFieldScale, 
                outputFieldMapNumber, outputTable));

            SDEWorkspace = sdeWorkspaceConnection;
            OutputFieldChangeDate = outputFieldChangeDate;
            OutputFieldScale = outputFieldScale;
            OutputFieldMapNumber = outputFieldMapNumber;
            OutputTable = outputTable;
        }

        public void Dispose()
        {
            _logger.Debug(MethodBase.GetCurrentMethod().Name);

            if (_gridTable != null) Marshal.FinalReleaseComObject(_gridTable);
            _gridTable = null;

            Marshal.ReleaseComObject(SDEWorkspace.Workspace);

            SDEWorkspace = null;
        }
        public void InitializeWorkspace()
        {
            _logger.Debug(MethodBase.GetCurrentMethod().Name);

            IWorkspace workspace = SDEWorkspace.Workspace;
            ITable table = GridTable;
        }

        private void ValidateGridTable()
        {
            _logger.Debug(MethodBase.GetCurrentMethod().Name);

            int idxMapNumber = GridTable.FindField(OutputFieldMapNumber);
            int idxChangeDate = GridTable.FindField(OutputFieldChangeDate);
            int idxScale = GridTable.FindField(OutputFieldScale);
            string changeDate = new System.Data.OracleClient.OracleDateTime(DateTime.Now).ToString();

            if (idxMapNumber < 0)
                throw new ErrorCodeException(ErrorCode.OutputTable, "Unable to find the map number field on the output table.");
            if (idxScale < 0)
                throw new ErrorCodeException(ErrorCode.OutputTable, "Unable to find the scale field on the output table.");
            if (idxChangeDate < 0)
                _logger.Error("Can't find the date field on the output table. Processing will continue without the date.");

        }


        private void InitializeFields()
        {
            _idxMapNumber = GridTable.FindField(OutputFieldMapNumber);
            _idxChangeDate = GridTable.FindField(OutputFieldChangeDate);
            _idxScale = GridTable.FindField(OutputFieldScale);
            _changeDate = new System.Data.OracleClient.OracleDateTime(DateTime.Now).ToString();
        }

        /// <summary>
        /// Writes the grid numbers to the configured to the database table.
        /// If the grid number already exists in the table, it will be ignored.
        /// </summary>
        /// <param name="ws">The workspace used to find the desired table.</param>
        /// <param name="gridsToWrite">A dictionary of grids (with correponding scales) needed to be written to the database.</param>
        public void WriteGrids(Dictionary<string, string> gridsToWrite)
        {
            _logger.Debug(MethodBase.GetCurrentMethod().Name);
            _logger.Info("Writing grids to database table (" + OutputTable + "):");

            if (gridsToWrite.Count == 0)
            {
                _logger.Info("  No grids found to write.");
                return;
            }
            AOHelper.EnsureInEditSession(SDEWorkspace.NonVersionedEditsWorkspace);
            ValidateGridTable();
            InitializeFields();

            string whereClauseWrapper = OutputFieldMapNumber + "='{0}' AND " + OutputFieldChangeDate + " IS NOT NULL";
            IQueryFilter qf = new QueryFilter();
            foreach (KeyValuePair<string, string> grid in gridsToWrite)
            {
                if (string.IsNullOrEmpty(grid.Key) || string.IsNullOrEmpty(grid.Value))
                    continue;

                try
                {
                    //Check to make sure the grid doesn't already exist.
                    qf.WhereClause = string.Format(whereClauseWrapper, grid.Key);
                    if (GridTable.RowCount(qf) > 0)
                    {
                        _logger.Debug("  The grid number '" + grid.Key + "' already exists in the output table and will be skipped.");
                        continue;
                    }
                }
                catch (Exception ex)
                {
                    _logger.Error("Issue checking for existing grids using grid '" + grid.Key + "': " + ex.Message, ex);
                    continue;
                }
                WriteGrid(grid.Key, grid.Value);
                
            }
            while (Marshal.ReleaseComObject(qf) > 0) { }
            if (_gridTable != null) Marshal.FinalReleaseComObject(_gridTable);

            _gridTable = null;
            qf = null;
            AOHelper.EnsureCloseEditSession(SDEWorkspace.NonVersionedEditsWorkspace);
            SDEWorkspace.DeleteTempVersion();

            _logger.Info("  Successfully wrote newly changed grids to the database table.");
        }

        private void WriteGrid(string gridKey, string gridValue)
        {
            _logger.Debug(MethodBase.GetCurrentMethod().Name);

            ICursor insertCursor = null;
            IRowBuffer newRow = null;
            try
            {
                //Insert the grid into the table.
                insertCursor = GridTable.Insert(false);
                newRow = GridTable.CreateRowBuffer();
                newRow.set_Value(_idxMapNumber, gridKey);
                newRow.set_Value(_idxScale, gridValue);
                if (_idxChangeDate >= 0)
                    newRow.set_Value(_idxChangeDate, _changeDate);
                insertCursor.InsertRow(newRow);
                _logger.Debug("Inserted Row for GridKey [ " + gridKey + " ] GridValue [ " + gridValue + " ]");
            }
            catch (Exception ex)
            {
                _logger.Error("Issue inserting grid information using grid '" + gridKey + "': " + ex.Message);
                throw;
            }
            finally
            {
                if (newRow != null)
                    while (Marshal.ReleaseComObject(newRow) > 0) { }
                if (insertCursor != null)
                    while (Marshal.ReleaseComObject(insertCursor) > 0) { }
            }
        }
    }
}
