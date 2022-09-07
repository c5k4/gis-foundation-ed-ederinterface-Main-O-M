using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.ADF;
using System.Runtime.InteropServices;
using System.Diagnostics;
using PGE.Common.ChangeDetectionAPI;

namespace PGE.Interfaces.SAP
{
    public class EnumerableCursor : IEnumerator<IRow>, IEnumerable<IRow>
    {
        ITable _table;
        IQueryFilter _queryFilter;

        List<EnumerableCursor> childEnumCursors = new List<EnumerableCursor>();
        IRow _currentRow = null;
        ICursor _cursor = null;
        int totalRowCount = 0;

        public EnumerableCursor(ITable table, IQueryFilter queryFilter)
        {
            _table = table;
            _queryFilter = queryFilter;
        }

        #region IEnumerator<IRow> Members

        public IRow Current
        {
            get { return _currentRow; }
        }

        public void CleanUpCursor()
        {
#if DEBUG
            //Debugger.Launch();
#endif
            //while (Marshal.ReleaseComObject(_queryFilter) > 0) { }
            if (_cursor != null) { while (Marshal.ReleaseComObject(_cursor) > 0) { } }

            //Release any resources for child cursors as well
            foreach (EnumerableCursor enumCursor in childEnumCursors)
            {
                if (enumCursor != null)
                {
                    enumCursor.CleanUpCursor();
                    enumCursor.Dispose();
                }
            }
            //while (Marshal.ReleaseComObject(_table) > 0) { }
        }

        #endregion

        #region IEnumerator Members

        object System.Collections.IEnumerator.Current
        {
            get { return _currentRow; }
        }

        public bool MoveNext()
        {
            if (_cursor == null) // initialize the cursor
            {
                totalRowCount = _table.RowCount(_queryFilter);
                _cursor = _table.Search(_queryFilter, false);
            }
            if (_currentRow != null) { while (Marshal.ReleaseComObject(_currentRow) > 0) { } }
            _currentRow = _cursor.NextRow();
            return _currentRow != null;
        }

        public int RowCount()
        {
            if (_cursor == null) // initialize the cursor
            {
                totalRowCount = _table.RowCount(_queryFilter);
                _cursor = _table.Search(_queryFilter, false);
            }
            return totalRowCount;
        }

        public void Reset()
        {
            _cursor = null;
        }

        #endregion

        #region IEnumerable<IRow> Members

        public IEnumerator<IRow> GetEnumerator()
        {
            //If a user is getting the enumerator then we need to create a new one
            EnumerableCursor childEnumCursor = new EnumerableCursor(_table, _queryFilter);
            childEnumCursors.Add(childEnumCursor);
            return childEnumCursor;
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            //If a user is getting the enumerator then we need to create a new one
            EnumerableCursor childEnumCursor = new EnumerableCursor(_table, _queryFilter);
            childEnumCursors.Add(childEnumCursor);
            return childEnumCursor;
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
        }

        #endregion
    }

    public class EnumerableCursor2 : IEnumerator<IRow>, IEnumerable<IRow>
    {
        ITable _table;
        ITable _targetTable = null;
        IQueryFilter _queryFilter;
        IQueryFilter _targetFilter;
        string OriginalWhereClause = "";
        List<int> OIDsToReturn = null;
        List<string> OIDWhereInClauses = new List<string>();
        int currentWhereClauseIndex = 0;
        IDictionary<string, ChangedFeatures> _cdVersionDifference;
        IWorkspaceEdit workspaceEdit;

        ComReleaser _comReleaser;
        IRow _currentRow = null;
        IRow _targetRow = null;
        ICursor _cursor = null;
        ICursor _TargetCursor = null;
        int totalRowCount = 0;

        public EnumerableCursor2(ITable sourceTable, IQueryFilter qf, List<int> oidsToReturn, ITable targetTable)
        {
            _table = sourceTable;
            _targetTable = targetTable;
            OIDsToReturn = oidsToReturn;
            OIDWhereInClauses = GetWhereInClauses(oidsToReturn);
            _comReleaser = new ComReleaser();
            _queryFilter = qf;
            _targetFilter = new QueryFilterClass();
            _targetFilter.SubFields = _queryFilter.SubFields;
            OriginalWhereClause = qf.WhereClause;

            IQueryFilterDefinition qfDef = _queryFilter as IQueryFilterDefinition;
            qfDef.PostfixClause = "ORDER BY OBJECTID";

            IQueryFilterDefinition qfDef2 = _targetFilter as IQueryFilterDefinition;
            qfDef2.PostfixClause = "ORDER BY OBJECTID";
        }

        public EnumerableCursor2(ITable sourceTable, IQueryFilter qf, List<int> oidsToReturn, ITable targetTable, IDictionary<string, ChangedFeatures> cdVersionDifference)
        {
            _table = sourceTable;
            _targetTable = targetTable;
            OIDsToReturn = oidsToReturn;
            OIDWhereInClauses = GetWhereInClauses(oidsToReturn);
            _comReleaser = new ComReleaser();
            _queryFilter = qf;
            _targetFilter = new QueryFilterClass();
            _targetFilter.SubFields = _queryFilter.SubFields;
            OriginalWhereClause = qf.WhereClause;

            _cdVersionDifference = cdVersionDifference;
            
            IQueryFilterDefinition qfDef = _queryFilter as IQueryFilterDefinition;
            qfDef.PostfixClause = "ORDER BY OBJECTID";

            IQueryFilterDefinition qfDef2 = _targetFilter as IQueryFilterDefinition;
            qfDef2.PostfixClause = "ORDER BY OBJECTID";
        }

        #region IEnumerator<IRow> Members

        public IRow Current
        {
            get { return _currentRow; }
        }

        public IRow TargtRow
        {
            get
            {
                if (_targetRow != null && _currentRow != null && _targetRow.OID == _currentRow.OID)
                {
                    return _targetRow;
                }
                else
                {
                    //Target row doesn't match the current row so 
                    return null;
                }
            }
            set { _targetRow = value; }
        }

        public void CleanUpCursor()
        {
            while (Marshal.ReleaseComObject(_cursor) > 0) { }
            if (_TargetCursor != null) { while (Marshal.ReleaseComObject(_TargetCursor) > 0) { } }
        }

        #endregion

        #region IEnumerator Members

        object System.Collections.IEnumerator.Current
        {
            get { return _currentRow; }
        }

        public bool MoveNext()
        {
            if (_cursor == null) // initialize the cursor
            {
                foreach (string whereInClause in OIDWhereInClauses)
                {
                    _queryFilter.WhereClause = OriginalWhereClause;
                    if (!string.IsNullOrEmpty(_queryFilter.WhereClause)) { _queryFilter.WhereClause += " AND OBJECTID IN (" + whereInClause + ")"; }
                    else { _queryFilter.WhereClause += "OBJECTID IN (" + whereInClause + ")"; }
                    totalRowCount += _table.RowCount(_queryFilter);
                }

                currentWhereClauseIndex = 0;
                _queryFilter.WhereClause = OriginalWhereClause;
                if (!string.IsNullOrEmpty(_queryFilter.WhereClause)) { _queryFilter.WhereClause += " AND OBJECTID IN (" + OIDWhereInClauses[0] + ")"; }
                else { _queryFilter.WhereClause += "OBJECTID IN (" + OIDWhereInClauses[0] + ")"; }
                _targetFilter.WhereClause = "OBJECTID IN (" + OIDWhereInClauses[0] + ")";

                _cursor = _table.Search(_queryFilter, false);
                if (_targetTable != null)
                {
                    _TargetCursor = _targetTable.Search(_targetFilter, false);
                    _targetRow = _TargetCursor.NextRow();
                }
                _comReleaser.ManageLifetime(_cursor);
                if (_TargetCursor != null) { _comReleaser.ManageLifetime(_TargetCursor); }
            }



            if (_currentRow != null) { Marshal.ReleaseComObject(_currentRow); }
            _currentRow = _cursor.NextRow();

            //if(_currentRow == null)
            //{
            //    string FCName = string.Empty;
            //    if (_cdVersionDifference != null)
            //    {
            //        FCName = ((IDataset)_table).BrowseName;

            //        if (!string.IsNullOrWhiteSpace(FCName))
            //        {
            //            if (_cdVersionDifference.ContainsKey(FCName))
            //            {
            //                foreach (int oid in OIDsToReturn)
            //                {
            //                    if (_cdVersionDifference[FCName].Action.Delete.All(str => str.OID == oid))
            //                    {
            //                        if (workspaceEdit == null)
            //                            workspaceEdit = ((IWorkspaceEdit)(((IDataset)_table).Workspace));

            //                        if (!workspaceEdit.IsBeingEdited())
            //                        {
            //                            workspaceEdit.StartEditing(true);
            //                            workspaceEdit.StartEditOperation(); 
            //                        }

            //                        IRow row = (workspaceEdit as IFeatureWorkspace).OpenTable(FCName).CreateRow();
            //                        //IRow row = ((IFeatureWorkspace)(((IDataset)_table).Workspace)).OpenTable(FCName).CreateRow();

            //                        //IRow row = _table.CreateRow();

            //                        DeleteFeat deleteFeat = _cdVersionDifference[FCName].Action.Delete.First();
            //                        for (int i = 0; i < row.Fields.FieldCount; i++)
            //                        {
            //                            if(deleteFeat.fields_Old.ContainsKey(row.Fields.Field[i].Name))
            //                            {
            //                                row.Value[i] = deleteFeat.fields_Old[row.Fields.Field[i].Name];
            //                            }
            //                        }

            //                        _currentRow = row;
            //                        _cdVersionDifference[FCName].Action.Delete.Remove(deleteFeat);
            //                    }
            //                }
            //            }
            //        }
            //    }
            //}

            if (_currentRow == null)
            {
                //Check if there are more where clauses to process through first before returning a null row
                currentWhereClauseIndex++;
                if (currentWhereClauseIndex < OIDWhereInClauses.Count)
                {
                    _queryFilter.WhereClause = OriginalWhereClause;
                    if (!string.IsNullOrEmpty(_queryFilter.WhereClause)) { _queryFilter.WhereClause += " AND OBJECTID IN (" + OIDWhereInClauses[currentWhereClauseIndex] + ")"; }
                    else { _queryFilter.WhereClause += "OBJECTID IN (" + OIDWhereInClauses[currentWhereClauseIndex] + ")"; }
                    _targetFilter.WhereClause = "OBJECTID IN (" + OIDWhereInClauses[currentWhereClauseIndex] + ")";

                    while (Marshal.ReleaseComObject(_cursor) > 0) { }
                    if (_TargetCursor != null) { while (Marshal.ReleaseComObject(_TargetCursor) > 0) { } }

                    _cursor = _table.Search(_queryFilter, false);
                    if (_targetTable != null)
                    {
                        _TargetCursor = _targetTable.Search(_targetFilter, false);
                        _targetRow = _TargetCursor.NextRow();
                    }
                    _currentRow = _cursor.NextRow();
                }
            }

            if (_currentRow != null && _targetTable != null)
            {
                if (_targetRow == null || _targetRow.OID != _currentRow.OID)
                {
                    try
                    {
                        while ((_targetRow = _TargetCursor.NextRow()) != null)
                        {
                            if (_targetRow.OID == _currentRow.OID || _targetRow.OID > _currentRow.OID) { break; }
                        }
                    }
                    catch (Oracle.DataAccess.Client.OracleException oex) { GISSAPIntegrator._logger.Error(oex.GetType() + " || " + oex.Number + " || " + oex.Message + " || \n" + oex.StackTrace); if (GISSAPIntegrator.HandleOracleErrorCodes || GISSAPIntegrator.OracleErrorCodes.Contains(Convert.ToString(oex.ErrorCode))) { throw oex; } }
                    catch (Exception ex) { }
                }
            }
            return _currentRow != null;
        }

        public int RowCount()
        {
            string FCName = string.Empty;

            if (_cursor == null && OIDWhereInClauses.Count>0) // initialize the cursor
            {
                foreach (string whereInClause in OIDWhereInClauses)
                {
                    _queryFilter.WhereClause = OriginalWhereClause;
                    if (!string.IsNullOrEmpty(_queryFilter.WhereClause)) { _queryFilter.WhereClause += " AND OBJECTID IN (" + whereInClause + ")"; }
                    else { _queryFilter.WhereClause += "OBJECTID IN (" + whereInClause + ")"; }
                    totalRowCount += _table.RowCount(_queryFilter);

                    if (totalRowCount == 0)
                    {
                        if (_cdVersionDifference != null)
                        {
                            FCName = ((IDataset)_table).BrowseName;

                            if(!string.IsNullOrWhiteSpace(FCName))
                            {
                                if(_cdVersionDifference.ContainsKey(FCName.ToUpper()))
                                {
                                    foreach (int oid in OIDsToReturn)
                                    {
                                        if (_cdVersionDifference[FCName.ToUpper()].Action.Delete.All(str => str.OID == oid))
                                        {
                                            totalRowCount += 1;
                                        }
                                    }
                                    
                                }
                            }
                        }
                    }
                }

                currentWhereClauseIndex = 0;
                _queryFilter.WhereClause = OriginalWhereClause;
                if (!string.IsNullOrEmpty(_queryFilter.WhereClause)) { _queryFilter.WhereClause += " AND OBJECTID IN (" + OIDWhereInClauses[0] + ")"; }
                else { _queryFilter.WhereClause += "OBJECTID IN (" + OIDWhereInClauses[0] + ")"; }
                _targetFilter.WhereClause = "OBJECTID IN (" + OIDWhereInClauses[0] + ")";

                _cursor = _table.Search(_queryFilter, false);
                if (_targetTable != null)
                {
                    _TargetCursor = _targetTable.Search(_targetFilter, false);
                    _targetRow = _TargetCursor.NextRow();
                }
                _comReleaser.ManageLifetime(_cursor);
                if (_TargetCursor != null) { _comReleaser.ManageLifetime(_TargetCursor); }
            }
            return totalRowCount;
        }

        public void Reset()
        {
            _cursor = null;
        }

        #endregion

        #region IEnumerable<IRow> Members

        public IEnumerator<IRow> GetEnumerator()
        {
            //If a user is getting the enumerator then we need to create a new one
            /*
            IQueryFilter qf = new QueryFilterClass();
            qf.WhereClause = OriginalWhereClause;
            qf.SubFields = _queryFilter.SubFields;
            return new EnumerableCursor2(_table, qf, OIDsToReturn, _targetTable);
            */
            return this;
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            /*
            //If a user is getting the enumerator then we need to create a new one
            IQueryFilter qf = new QueryFilterClass();
            qf.WhereClause = OriginalWhereClause;
            qf.SubFields = _queryFilter.SubFields;
            return new EnumerableCursor2(_table, qf, OIDsToReturn, _targetTable);
            */
            return this;
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            _comReleaser.Dispose();
        }

        /// <summary>
        /// Obtains a list of where in clauses for a list of guids
        /// </summary>
        /// <param name="guids"></param>
        /// <returns></returns>
        private static List<string> GetWhereInClauses(List<int> guids)
        {
            try
            {
                List<string> whereInClauses = new List<string>();
                StringBuilder builder = new StringBuilder();

                for (int i = 0; i < guids.Count; i++)
                {
                    if ((((i % 999) == 0) && i != 0) || (guids.Count == i + 1))
                    {
                        builder.Append(guids[i]);
                        whereInClauses.Add(builder.ToString());
                        builder = new StringBuilder();
                    }
                    else { builder.Append(guids[i] + ","); }
                }
                return whereInClauses;
            }
            catch (Oracle.DataAccess.Client.OracleException oex) { GISSAPIntegrator._logger.Error(oex.GetType() + " || " + oex.Number + " || " + oex.Message + " || \n" + oex.StackTrace); throw oex; }
            catch (Exception ex)
            {
                throw new Exception("Failed to create guid where in clauses. Error: " + ex.Message);
            }
        }

        #endregion
    }
}
