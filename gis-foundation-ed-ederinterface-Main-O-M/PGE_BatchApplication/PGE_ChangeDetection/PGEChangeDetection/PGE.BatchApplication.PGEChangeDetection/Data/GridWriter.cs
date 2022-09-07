using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using ESRI.ArcGIS.Geodatabase;
using PGE.ChangeDetection.Exceptions;

namespace PGE.ChangeDetection.Data
{
    /// <summary>
    /// Contains the logic used to write grid numbers to the configured database table.
    /// </summary>
    public static class GridWriter
    {
        #region Public Static Members

        /// <summary>
        /// The name of the table used to store grid numbers for use by Map Production.
        /// </summary>
        public static string OutputTable;

        /// <summary>
        /// The name of the field within the output table that indicates the map (grid) number.
        /// </summary>
        public static string OutputFieldMapNumber;

        /// <summary>
        /// The name of the field within the output table that indicates the changed date.
        /// </summary>
        public static string OutputFieldChangeDate;

        /// <summary>
        /// The name of the field within the output table that indicates the map scale.
        /// </summary>
        public static string OutputFieldScale;

        #endregion

        #region Public Static Methods

        /// <summary>
        /// Writes the grid numbers to the configured to the database table.
        /// If the grid number already exists in the table, it will be ignored.
        /// </summary>
        /// <param name="ws">The workspace used to find the desired table.</param>
        /// <param name="gridsToWrite">A dictionary of grids (with correponding scales) needed to be written to the database.</param>
        public static void WriteGrids(IWorkspace ws, Dictionary<string, string> gridsToWrite)
        {
            Program.WriteStatus("", true);
            Program.WriteStatus("Writing grids to database table (" + OutputTable + "):", true);

            if (gridsToWrite.Count == 0)
            {
                Program.WriteStatus("  No grids found to write.", true);
                return;
            }

            ITable gridTable = null;
            try
            {
                gridTable = (ws as IFeatureWorkspace).OpenTable(OutputTable);
                if (gridTable == null)
                    throw new Exception("Table not found.");
            }
            catch (Exception ex)
            {
                throw new ErrorCodeExceptionLegacy(ErrorCodeLegacy.OutputTable, "Unable to find the output table.", ex);
            }
            int idxMapNumber = gridTable.FindField(OutputFieldMapNumber);
            int idxChangeDate = gridTable.FindField(OutputFieldChangeDate);
            int idxScale = gridTable.FindField(OutputFieldScale);
            string changeDate = new System.Data.OracleClient.OracleDateTime(DateTime.Now).ToString();

            if (idxMapNumber < 0)
                throw new ErrorCodeExceptionLegacy(ErrorCodeLegacy.OutputTable, "Unable to find the map number field on the output table.");
            if (idxScale < 0)
                throw new ErrorCodeExceptionLegacy(ErrorCodeLegacy.OutputTable, "Unable to find the scale field on the output table.");
            if (idxChangeDate < 0)
                Program.WriteWarning(new Exception("Can't find the date field on the output table. Processing will continue without the date."), true);

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
                    if (gridTable.RowCount(qf) > 0)
                    {
                        Program.WriteStatus("  The grid number '" + grid.Key + "' already exists in the output table and will be skipped.", false);
                        continue;
                    }
                }
                catch (Exception ex)
                {
                    Program.WriteWarning(new Exception("Issue checking for existing grids using grid '" + grid.Key + "': " + ex.Message, ex), false);
                    continue;
                }

                ICursor insertCursor = null;
                IRowBuffer newRow = null;
                try
                {
                    //Insert the grid into the table.
                    insertCursor = gridTable.Insert(false);
                    newRow = gridTable.CreateRowBuffer();
                    newRow.set_Value(idxMapNumber, grid.Key);
                    newRow.set_Value(idxScale, grid.Value);
                    if (idxChangeDate >= 0)
                        newRow.set_Value(idxChangeDate, changeDate);
                    insertCursor.InsertRow(newRow);
                }
                catch (Exception ex)
                {
                    Program.WriteWarning(new Exception("Issue inserting grid information using grid '" + grid.Key + "': " + ex.Message, ex), false);
                    continue;
                }
                finally
                {
                    if (newRow != null)
                        while (Marshal.ReleaseComObject(newRow) > 0) { }
                    if (insertCursor != null)
                        while (Marshal.ReleaseComObject(insertCursor) > 0) { }
                }
            }
            while (Marshal.ReleaseComObject(qf) > 0) { }

            Program.WriteStatus("  Successfully wrote newly changed grids to the database table.", true);
        }

        #endregion
    }
}
