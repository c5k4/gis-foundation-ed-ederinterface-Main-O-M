using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using ESRI.ArcGIS.Geodatabase;
using Comm = PGE.Interfaces.CCBServicePointUpdater.Common;
using PGE.Interfaces.CCBServicePointUpdater.SQL;
using System.Diagnostics;

namespace PGE.Interfaces.CCBServicePointUpdater
{
    /// <summary>
    /// has methods to insert/delete/update rows in the Servicepoint table so that the table is in sync with the most up-to-date CEDSA data
    /// </summary>
    class ServicePointModifier
    {
        private VersionedEditor vEditor;
        private ITable SERVICEPOINT;
        private int totalCount;

        private List<string> idBuffer;

        //This list is not necessarily filled with IDs that have all failed. All IDs that were in the same save operation as this ID will be stored in this list during execution
        private List<string> potentialFailedUniqueIDs;

        private static readonly int OPERATION_BUFFER_SIZE = 100;
        private Stopwatch s = new Stopwatch();

        public ServicePointModifier(VersionedEditor v)
        {
            vEditor = v;
            SERVICEPOINT = vEditor.OpenTable(SQLConstants.SERVICEPOINT_TABLE);
            potentialFailedUniqueIDs = new List<string>();
            GC.KeepAlive(s);
            idBuffer = new List<string>();
        }

        private void SetupOperationAndPerformOnSPTable(string operationString, Action<string> operationAction)
        {
            StringBuilder uniqueIDsToQuery = new StringBuilder();
            IRow row_ActionTbl;
            string uniqueID;
            ICursor opToBeDone_ActionTbl = null;

            opToBeDone_ActionTbl = SQLTableInterface.GetQueryResultsWhereClause(vEditor.getFeatureWorkspace(), SQLConstants.ALL_CHANGES_TABLE, SQLConstants.ALL_COLUMNS, SQLConstants.ACTION_COL, operationString);

            while ((row_ActionTbl = opToBeDone_ActionTbl.NextRow()) != null)
            {
                uniqueID = null;

                try
                {
                    uniqueID = GetValue(row_ActionTbl, SQLConstants.UNIQUEID_COL);
                    uniqueIDsToQuery.Append(uniqueID + ",");
                    totalCount++;

                    if (totalCount % OPERATION_BUFFER_SIZE == 0)
                    {
                        performOperationOnSPTable(uniqueIDsToQuery, operationAction);
                        uniqueIDsToQuery.Clear();
                    }
                }
                catch (Exception e)
                {
                    Comm.LogManager.WriteLine(e.ToString());
                }
            }

            performOperationOnSPTable(uniqueIDsToQuery, operationAction);
        }

        private void performOperationOnSPTable(StringBuilder uniqueIDsToQuery, Action<string> operationAction)
        {
            //Removes comma at the end of the string
            uniqueIDsToQuery.Remove(uniqueIDsToQuery.Length - 1, 1);
            operationAction(uniqueIDsToQuery.ToString());

            Comm.LogManager.WriteLine("Finished " + totalCount + " operations.");
            vEditor.SaveAndContinue();
        }

        /// <summary>
        /// Takes the 'I' (insert) rows from the already populated SP_ACTION table, copies the rows from the SERVICEPOINT_TAB
        /// table, and inserts them into the EDGIS.SERVICEPOINT table
        /// </summary>
        private void PerformInsertsOnSPTable(string uniqueIDsCSV)
        {
            IRow row_NewSPTbl = null;
            IRowBuffer rowBuff_InsertTbl = null;
            ICursor cursor_InsertTbl, rowsToInsert_NewSPTbl;

            s.Restart();

            try
            {
                cursor_InsertTbl = SERVICEPOINT.Insert(true);
                rowBuff_InsertTbl = SERVICEPOINT.CreateRowBuffer();

                rowsToInsert_NewSPTbl = SQLTableInterface.GetQueryResultsWhereInClause(vEditor.getFeatureWorkspace(), SQLConstants.NEW_SERVICEPOINT_DATA, SQLConstants.ALL_COLUMNS, SQLConstants.UNIQUEID_COL, uniqueIDsCSV);

                while ((row_NewSPTbl = rowsToInsert_NewSPTbl.NextRow()) != null)
                {
                    idBuffer.Add(GetValue(row_NewSPTbl, SQLConstants.UNIQUEID_COL));

                    try
                    {
                        VersionedEditor.WriteAllFieldsToNewRow(row_NewSPTbl, rowBuff_InsertTbl);
                        cursor_InsertTbl.InsertRow(rowBuff_InsertTbl);

                    }
                    catch (Exception e)
                    {
                        Comm.LogManager.WriteLine(e.ToString());
                        AbortOperationAndContinue();
                        cursor_InsertTbl = SERVICEPOINT.Insert(true);
                    }
                }
            }
            catch (Exception e)
            {
                Comm.LogManager.WriteLine(e.ToString());
            }
            Comm.LogManager.WriteLine(s.Elapsed.TotalMilliseconds.ToString());
            s.Restart();
        }

        /// <summary>
        /// Takes the 'U' (update) rows from the already populated SP_ACTION table, copies the rows from the SERVICEPOINT_TAB
        /// table, and updates them in the EDGIS.SERVICEPOINT table for each field that has been changed
        /// </summary>
        private void PerformUpdatesOnSPTable(string uniqueIDsCSV)
        {
            IQueryFilter bulkFilter;
            ICursor results_NewDataTbl, results_SPTbl;
            IRow rowToUpdate, sourceRow;
            
            Hashtable newDataRows = new Hashtable();
            try
            {
                bulkFilter = SQLTableInterface.GetQueryFilterWhereInClause(SQLConstants.ALL_COLUMNS, SQLConstants.UNIQUEID_COL, uniqueIDsCSV);
                results_NewDataTbl = SQLTableInterface.GetQueryResultsWhereInClause(vEditor.getFeatureWorkspace(), SQLConstants.NEW_SERVICEPOINT_DATA, SQLConstants.ALL_COLUMNS, SQLConstants.UNIQUEID_COL, uniqueIDsCSV);

                while ((sourceRow = results_NewDataTbl.NextRow()) != null)
                {
                    newDataRows.Add(GetValue(sourceRow, SQLConstants.UNIQUEID_COL), sourceRow);
                }

                results_SPTbl = SERVICEPOINT.Update(bulkFilter, true);

                while ((rowToUpdate = results_SPTbl.NextRow()) != null)
                {
                    try
                    {
                        string spUniqueID = GetValue(rowToUpdate, SQLConstants.UNIQUEID_COL);
                        idBuffer.Add(spUniqueID);

                        VersionedEditor.UpdateAllChangedFields(rowToUpdate, (IRow)newDataRows[GetValue(rowToUpdate, SQLConstants.UNIQUEID_COL)]);
                        results_SPTbl.UpdateRow(rowToUpdate);
                    }
                    catch (Exception e)
                    {
                        Comm.LogManager.WriteLine(e.ToString());
                        AbortOperationAndContinue();
                    }

                }
            }
            catch (Exception e)
            {
                Comm.LogManager.WriteLine(e.ToString());
            }
            Comm.LogManager.WriteLine(s.Elapsed.TotalMilliseconds.ToString());
            s.Restart();
        }

        /// <summary>
        /// Takes the 'D' (delete) rows from the already populated SP_ACTION table, copies the rows from the SERVICEPOINT_TAB
        /// table, and removes them from the EDGIS.SERVICEPOINT table
        /// </summary>
        private void PerformDeletesOnSPTable(string uniqueIDsCSV)
        {
            try
            {
                SERVICEPOINT.DeleteSearchedRows(SQLTableInterface.GetQueryFilterWhereInClause(SQLConstants.ALL_COLUMNS, SQLConstants.UNIQUEID_COL, uniqueIDsCSV));

            }
            catch (Exception e)
            {
                Comm.LogManager.WriteLine(e.ToString());
            }
            Comm.LogManager.WriteLine(s.Elapsed.TotalMilliseconds.ToString());
            s.Restart();
        }

        private void ResetCount()
        {
            totalCount = 0;
        }

        private string GetValue(IRowBuffer row, string col)
        {
            return (row.get_Value(row.Fields.FindField(col))).ToString();
        }

        private void AbortOperationAndContinue()
        {
            foreach (string uniqueID in idBuffer)
            {
                potentialFailedUniqueIDs.Add(uniqueID);
            }

            idBuffer = new List<string>();
            vEditor.AbortAndContinue();
        }

        private void checkFailedIDsList()
        {
            if (potentialFailedUniqueIDs.Count > 0)
            {
                Comm.LogManager.WriteLine("Failed IDs: " + string.Join(",", potentialFailedUniqueIDs.Select(id => id).ToArray()));
                potentialFailedUniqueIDs = new List<string>();
            }
        }

        public void MakeAllChangesToServicepointTable()
        {
            Action<string> opAction;
            vEditor.StartEditSession();
            s.Start();

            ResetCount();
            Comm.LogManager.WriteLine("Starting inserts.");
            opAction = PerformInsertsOnSPTable;
            SetupOperationAndPerformOnSPTable(SQLConstants.INSERT.ToString(), opAction);
            checkFailedIDsList();
            
            ResetCount();
            Comm.LogManager.WriteLine("Starting updates.");
            opAction = PerformUpdatesOnSPTable;
            SetupOperationAndPerformOnSPTable(SQLConstants.UPDATE.ToString(), opAction);
            checkFailedIDsList();
            
            ResetCount();
            Comm.LogManager.WriteLine("Starting deletes.");
            opAction = PerformDeletesOnSPTable;
            SetupOperationAndPerformOnSPTable(SQLConstants.DELETE.ToString(), opAction);
            
            vEditor.FinishEditSession(true);
        }
    }
}
