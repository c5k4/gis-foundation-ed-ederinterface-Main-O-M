using System;
using System.Collections.Generic;
using System.Data;
using Oracle.DataAccess.Client;
using Oracle.DataAccess.Types;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using ESRI.ArcGIS.DataManagementTools;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.Geoprocessing;
using ESRI.ArcGIS.Geoprocessor;
using PGE.Common.ChangesManagerShared;
using PGE.Common.ChangesManagerShared.Utilities;
using PGE.Common.Delivery.Diagnostics;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace PGE.BatchApplication.ChangeDetection.ETL
{
    public class RowTransferrer
    {
        private static readonly Log4NetLogger _logger = new Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "ChangeDetectionDefault.log4net.config");

        public bool UseGPTool { get; set; }

        public bool DisableSchemaTest { get; set; }
        public bool InsertOnly { get; set; }

        public SDEWorkspaceConnection SourceSdeWorkspaceConnection { get; set; }
        public SDEWorkspaceConnection DestSdeWorkspaceConnection { get; set; }

        public OleDbWorkspaceConnection SourceOleDbWorkspaceConnection { get; set; }

        private OleDbWorkspaceConnection _destOleDbWorkspaceConnection = null;
        public OleDbWorkspaceConnection DestOleDbWorkspaceConnection
        {
            get
            {
                return _destOleDbWorkspaceConnection;
            }
            set
            {
                _destOleDbWorkspaceConnection = value;
            }
        }

        public AdoOracleConnection SourceAdoOracleConnection { get; set; }
        public AdoOracleConnection DestAdoOracleConnection { get; set; }

        public bool UseInsertRowsByChunk { get; set; }

        private string _sourceTableName;
        private string _destTableName;
        private string _checkdestTable = "WEBR.PGE_TRANSFORMER_STAGING";
        private int _keyFieldIndex;
        private bool _keyFieldTypeIsNumeric;

        private ITable _sourceTable = null;
        private ITable _destTable = null;

        private IDictionary<string, int> _sourceCachedFields = new Dictionary<string, int>();
        private IDictionary<string, int> _destCachedFields = new Dictionary<string, int>();

        public IList<string> AttributeFilterList { get; set; }
        public IDictionary<string, string> AttributeMapping { get; set; }
        public string KeyAttribute { get; set; }
        public string IgnoreField { get; set; }
        public IDictionary<string, string> DefaultDestAttributes { get; set; }
        public bool TruncateDestTableBeforeWriting { get; set; }

        public string RelatedTableName { get; set; }
        public string RelatedTableKey { get; set; }
        public string RelatedTableField { get; set; }

        private int _maxSourceObjectID;

        private bool UseAttributeFilter
        {
            get
            {
                return AttributeFilterList != null;
            }
        }

        public bool IgnoreExistingCheck { get; set; }
        public bool UpdateIfRowExists { get; set; }
        public bool DeleteSourceRowsAfterTransfer { get; set; }

        private IList<object> _primaryKeys = new List<object>(); // object because the type might differ
        public bool Synchronize { get; set; }

        public RowTransferrer(string sourceTableName, string destTableName)
        {
            _sourceTableName = sourceTableName;
            _destTableName = destTableName;
            DeleteSourceRowsAfterTransfer = true;
            UseGPTool = true;
        }

        // Need an interface for this!
        private string SourceWorkspaceConnectionFile()
        {
            if (SourceOleDbWorkspaceConnection != null)
            {
                return SourceOleDbWorkspaceConnection.WorkspaceConnectionFile;
            }
            else if (SourceSdeWorkspaceConnection != null)
            {
                return SourceSdeWorkspaceConnection.WorkspaceConnectionFile;
            }
            else
            {
                return SourceAdoOracleConnection.ConnectionStringEncrypted;
            }
        }
        private string DestinationWorkspaceConnectionFile()
        {
            if (DestOleDbWorkspaceConnection != null)
            {
                return DestOleDbWorkspaceConnection.WorkspaceConnectionFile;
            }
            else if (DestSdeWorkspaceConnection != null)
            {
                return DestSdeWorkspaceConnection.WorkspaceConnectionFile;
            }
            else
            {
                return DestAdoOracleConnection.ConnectionStringEncrypted;
            }
        }

        // Need an interface
        private IWorkspace SourceWorkspace
        {
            get
            {
                if (SourceOleDbWorkspaceConnection != null)
                {
                    return SourceOleDbWorkspaceConnection.Workspace;
                }
                else
                {
                    return SourceSdeWorkspaceConnection.Workspace;
                }
            }
        }
        private IWorkspace DestinationWorkspace
        {
            get
            {
                if (DestOleDbWorkspaceConnection != null)
                {
                    return DestOleDbWorkspaceConnection.Workspace;
                }
                else if (DestSdeWorkspaceConnection != null)
                {
                    return DestSdeWorkspaceConnection.Workspace;
                }
                else
                {
                    return null;
                }
            }
        }

        private void TruncateDestTableAdo()
        {
            _logger.Debug(MethodBase.GetCurrentMethod().Name);

            OracleCommand deleteCommand = new OracleCommand("delete from " + _destTableName, DestAdoOracleConnection.OracleConnection);
            _logger.Debug(deleteCommand.CommandText);
            int rowsDeleted = deleteCommand.ExecuteNonQuery();
            _logger.Debug("Deleted [ " + rowsDeleted + " ] rows");
        }

        public void Transfer()
        {

            string SourceStr = SourceWorkspaceConnectionFile();
            int index = SourceStr.LastIndexOf("Password=");
            if (index > 0)
                SourceStr = SourceStr.Substring(0, index);
            string destStr = DestinationWorkspaceConnectionFile();
            index = destStr.LastIndexOf("Password=");
            if (index > 0)
                destStr = destStr.Substring(0, index);
            _logger.Debug(MethodBase.GetCurrentMethod().Name);
            _logger.Info("Copying from Workspace [ " + SourceStr + " ] to [ " + destStr + " ]");
            _logger.Info("Copying from table [ " + _sourceTableName + " ] to [ " + _destTableName + " ]");

            //_logger.Debug(MethodBase.GetCurrentMethod().Name);
            //_logger.Info("Copying from Workspace [ " + SourceWorkspaceConnectionFile() + " ] to [ " + DestinationWorkspaceConnectionFile() + " ]");
            //_logger.Info("Copying from table [ " + _sourceTableName + " ] to [ " + _destTableName + " ]");

            //LogParametersToDebug(); 

            OpenTables();

            if (SourceAdoOracleConnection != null)
            {
                if (TruncateDestTableBeforeWriting)
                {
                    TruncateDestTableAdo();
                }
                if (UseInsertRowsByChunk)
                {
                    InsertRowsDataAdapterChunk();
                }
                else
                {
                    CopyRowsDataAdapter();
                }
            }
            else if (DestOleDbWorkspaceConnection != null || DestAdoOracleConnection != null)
            {
                CopyRows();
            }
            else
            {
                CopyRowsBuffer();
            }

            if (DeleteSourceRowsAfterTransfer)
            {
                if (DestAdoOracleConnection != null)
                {
                    TruncateSourceTableAdo();
                }
                else
                {
                    TruncateSourceTable();
                }
            }

        }

        // NB only works to a certain size and no triggers e.g. exc = {"ORA-26086: direct path does not support triggers"}
        private void InsertRowsBulk()
        {
            _logger.Debug(MethodBase.GetCurrentMethod().Name);
            _logger.Debug("Copying from [ " + _sourceTableName + " ] to [ " + _destTableName + " ]");

            OracleCommand sourceCommand = new OracleCommand("select * from " + _sourceTableName, SourceAdoOracleConnection.OracleConnection);
            OracleDataAdapter sourceDataAdapter = new OracleDataAdapter(sourceCommand);
            DataSet sourceDataSet = new DataSet();
            sourceDataAdapter.Fill(sourceDataSet, _sourceTableName);
            _logger.Debug("Loaded [ " + sourceDataSet.Tables[0].Rows.Count + " ] Rows Source Data from [ " + _sourceTableName + " ]");

            try
            {
                using (OracleBulkCopy bulkCopy = new OracleBulkCopy(DestAdoOracleConnection.OracleConnection))
                {
                    bulkCopy.DestinationTableName = _destTableName;
                    DataTable sourceTable = sourceDataSet.Tables[_sourceTableName];
                    bulkCopy.WriteToServer(sourceTable);
                }
            }
            catch (Exception exc)
            {
                throw;
            }
        }

        private void InsertRowsDataAdapterChunk()
        {
            _logger.Debug(MethodBase.GetCurrentMethod().Name);

            OracleCommand sourceCommand = new OracleCommand("select max(rownum) from  " + _sourceTableName, SourceAdoOracleConnection.OracleConnection);
            object scalarObject = sourceCommand.ExecuteScalar();
            if (!(scalarObject is DBNull))
            {
                int maxRows = Convert.ToInt32(scalarObject);
                int chunkSize = 500000;
                int i = 1;

                // Loop and call 
                while (i <= maxRows)
                {
                    InsertRowsDataAdapter(i, i + (chunkSize - 1) > maxRows ? maxRows : i + (chunkSize - 1));
                    i += chunkSize;
                }
            }
        }

        private void InsertRowsDataAdapter(int rowsLow, int rowsHigh)
        {
            _logger.Debug(MethodBase.GetCurrentMethod().Name);
            _logger.Debug("Copying from [ " + rowsLow + " ] to [ " + rowsHigh + " ]");

            //OracleCommand sourceCommand = new OracleCommand("select * from (" + 
            //    "select c.*, rownum R from " + _sourceTableName + " c) where R >= " + rowsLow + " and R <= " + rowsHigh, SourceAdoOracleConnection.OracleConnection);
            OracleCommand sourceCommand = new OracleCommand("select * from (" +
                "select c.*, rownum R from " + _sourceTableName + " c) where R >= " + rowsLow + " and R <= " + rowsHigh, SourceAdoOracleConnection.OracleConnection);
            OracleCommand destCommand = new OracleCommand("select * from " + _destTableName + " where rownum = 1", DestAdoOracleConnection.OracleConnection);

            OracleDataAdapter sourceDataAdapter = new OracleDataAdapter(sourceCommand);
            DataSet sourceDataSet = new DataSet();
            sourceDataAdapter.Fill(sourceDataSet, _destTableName);
            //            sourceDataAdapter.Fill(sourceDataSet, rowsLow, rowsHigh - rowsLow, _destTableName);
            _logger.Debug("Loaded [ " + sourceDataSet.Tables[0].Rows.Count + " ] Rows Source Data from [ " + _sourceTableName + " ]");
            DataTable sourceTable = sourceDataSet.Tables[_destTableName];
            OracleDataAdapter destDataAdapter = new OracleDataAdapter(destCommand);
            DataSet destDataSet = new DataSet();
            destDataAdapter.Fill(destDataSet, _destTableName);
            DataTable destTable = destDataSet.Tables[_destTableName];
            sourceTable.Columns.Remove("R");
            _logger.Debug("Loaded [ " + destDataSet.Tables[0].Rows.Count + " ] Rows Dest Data from [ " + _destTableName + " ]");
            OracleCommandBuilder builder = new OracleCommandBuilder(destDataAdapter);
            destDataAdapter.InsertCommand = builder.GetInsertCommand();
            //destDataAdapter.UpdateCommand = builder.GetUpdateCommand();
            //destDataAdapter.DeleteCommand = builder.GetDeleteCommand();
            destDataAdapter.InsertCommand.Transaction = DestAdoOracleConnection.DbTransaction;
            //destDataAdapter.UpdateCommand.Transaction = DestAdoOracleConnection.DbTransaction;
            //destDataAdapter.DeleteCommand.Transaction = DestAdoOracleConnection.DbTransaction;


            // Better than using Merge because it correctly sets the appropriate RowState for use by dataAdapter.Update
            using (DataTableReader changeReader = new DataTableReader(sourceDataSet.Tables[0]))
            {
                destDataSet.Tables[0].Load(changeReader, LoadOption.Upsert);
            }


            _logger.Debug("Merged Dest Data from [ " + _destTableName + " ]");
            _logger.Debug("Added [ " + destTable.AsEnumerable().Where(r => r.RowState == DataRowState.Added).ToList().Count + " ]");

            int rowCount = destDataAdapter.Update(destTable);
            _logger.Debug("Inserted [ " + rowCount + " ] rows");

        }

        private void CopyRowsDataAdapter()
        {
            _logger.Debug(MethodBase.GetCurrentMethod().Name);
            _logger.Debug("Copying from [ " + _sourceTableName + " ] to [ " + _destTableName + " ]");

            OracleCommand sourceCommand = new OracleCommand("select * from " + _sourceTableName, SourceAdoOracleConnection.OracleConnection);
            string destSql = "select * from " + _destTableName;
            // InsertOnly is for big tables, everything will go over as in insert. Pulling down one row to initialize dataset
            destSql += InsertOnly ? " where rownum < 2" : "";
            OracleCommand destCommand = new OracleCommand(destSql, DestAdoOracleConnection.OracleConnection);

            OracleDataAdapter sourceDataAdapter = new OracleDataAdapter(sourceCommand);
            DataSet sourceDataSet = new DataSet();
            sourceDataAdapter.Fill(sourceDataSet, _destTableName);
            _logger.Debug("Loaded [ " + sourceDataSet.Tables[0].Rows.Count + " ] Rows Source Data from [ " + _sourceTableName + " ]");
            OracleDataAdapter destDataAdapter = new OracleDataAdapter(destCommand);
            DataSet destDataSet = new DataSet();
            destDataAdapter.Fill(destDataSet, _destTableName);
            _logger.Debug("Loaded [ " + destDataSet.Tables[0].Rows.Count + " ] Rows Dest Data from [ " + _destTableName + " ]");
            OracleCommandBuilder builder = new OracleCommandBuilder(destDataAdapter);
            destDataAdapter.InsertCommand = builder.GetInsertCommand();
            destDataAdapter.UpdateCommand = builder.GetUpdateCommand();
            destDataAdapter.DeleteCommand = builder.GetDeleteCommand();
            destDataAdapter.InsertCommand.Transaction = DestAdoOracleConnection.DbTransaction;
            destDataAdapter.UpdateCommand.Transaction = DestAdoOracleConnection.DbTransaction;
            destDataAdapter.DeleteCommand.Transaction = DestAdoOracleConnection.DbTransaction;

            DataTable sourceTable = sourceDataSet.Tables[_destTableName];
            DataTable destTable = destDataSet.Tables[_destTableName];

            if (!String.IsNullOrEmpty(KeyAttribute))
            {
                DataColumn[] sourcePrimaryKeys = new DataColumn[1];
                sourcePrimaryKeys[0] = sourceTable.Columns[KeyAttribute];
                sourceTable.PrimaryKey = sourcePrimaryKeys;
                DataColumn[] destPrimaryKeys = new DataColumn[1];
                destPrimaryKeys[0] = destTable.Columns[KeyAttribute];
                destTable.PrimaryKey = destPrimaryKeys;
            }

            // Better than using Merge because it correctly sets the appropriate RowState for use by dataAdapter.Update
            using (DataTableReader changeReader = new DataTableReader(sourceDataSet.Tables[0]))
            {
                destDataSet.Tables[0].Load(changeReader, LoadOption.Upsert);
            }

            if (Synchronize)
            {
                SetTableDeletesDataAdapter(sourceTable, destTable);
            }

            _logger.Debug("Merged Dest Data from [ " + _destTableName + " ]");
            _logger.Debug("Added [ " + destTable.AsEnumerable().Where(r => r.RowState == DataRowState.Added).ToList().Count + " ]");
            _logger.Debug("Modified [ " + destTable.AsEnumerable().Where(r => r.RowState == DataRowState.Modified).ToList().Count + " ]");
            _logger.Debug("Deleted [ " + destTable.AsEnumerable().Where(r => r.RowState == DataRowState.Deleted).ToList().Count + " ]");
            _logger.Debug("Unchanged [ " + destTable.AsEnumerable().Where(r => r.RowState == DataRowState.Unchanged).ToList().Count + " ]");

            int rowCount = destDataAdapter.Update(destTable);
            _logger.Debug("Inserted/Updated/Deleted [ " + rowCount + " ] rows");

        }


        private void SetTableDeletesDataAdapter(DataTable sourceTable, DataTable destTable)
        {
            _logger.Debug(MethodBase.GetCurrentMethod().Name);
            if (sourceTable.PrimaryKey == null || destTable.PrimaryKey == null)
            {
                _logger.Warn("Primary Keys need to exist in source & dest tables");
                return;
            }

            for (int i = 0; i < destTable.Rows.Count - 1; i++)
            {
                object primaryKey = destTable.Rows[i][destTable.PrimaryKey[0]];
                if (sourceTable.Rows.Find(primaryKey) == null)
                {
                    destTable.Rows[i].Delete();
                }
            }
        }

        private void DeleteExtraneousDestRows()
        {
            _logger.Debug(MethodBase.GetCurrentMethod().Name);

            OracleDataAdapter dataAdapter = new OracleDataAdapter("select * from " + _destTableName, DestAdoOracleConnection.OracleConnection);

            DataSet dataSet = new DataSet();
            dataAdapter.Fill(dataSet, _destTableName);
            DataTable table = dataSet.Tables[_destTableName];

            foreach (DataRow row in table.Rows)
            {
                if (!_primaryKeys.Contains(row[KeyAttribute]))
                {
                    if (DestAdoOracleConnection.OracleConnection.State != ConnectionState.Open)
                    {
                        DestAdoOracleConnection.OracleConnection.Open();
                    }

                    _logger.Debug("Deleting [ " + row[KeyAttribute] + " ]");
                    new OracleCommand("DELETE FROM " + _destTableName + " WHERE " + KeyAttribute + " = " +
                                          GetKeyAttributeSqlValue(row[KeyAttribute]), DestAdoOracleConnection.OracleConnection).ExecuteNonQuery();

                }
            }
        }

        private void EnsureFieldsAreEquivalent()
        {
            _logger.Debug(MethodBase.GetCurrentMethod().Name);

            if (_sourceCachedFields.Count == 0 || _destCachedFields.Count == 0)
            {
                throw new ApplicationException("Fields not cached");
            }
            if (_sourceCachedFields.Count != _destCachedFields.Count)
            {
                throw new ApplicationException("Different # of fields");
            }

            foreach (KeyValuePair<string, int> cachedField in _sourceCachedFields)
            {
                if (!_destCachedFields.ContainsKey(cachedField.Key))
                {
                    throw new ApplicationException("Field [ " + cachedField.Key + " ] does not exist in Dest table");
                }
                // TODO: could check field type/length
            }
        }

        private void EnsureFieldsAreCached(ITable table, IDictionary<string, int> fields)
        {
            _logger.Debug(MethodBase.GetCurrentMethod().Name);

            if (table == null) return;

            for (int i = 0; i < table.Fields.FieldCount; i++)
            {
                if (table.Fields.get_Field(i).Type != esriFieldType.esriFieldTypeOID &&
                    (IgnoreField == null || (!String.IsNullOrEmpty(IgnoreField) && table.Fields.get_Field(i).Name != IgnoreField)))
                {

                    fields.Add(table.Fields.get_Field(i).Name, i);

                }
                else if (_destTableName == _checkdestTable && (IgnoreField == null || (!String.IsNullOrEmpty(IgnoreField) && table.Fields.get_Field(i).Name != IgnoreField)) && i == 0)
                {
                    fields.Add(table.Fields.get_Field(i).Name, i);
                }

            }
        }


        private void LogParametersToDebug()
        {
            _logger.Info("Transferring from source connection [ " + SourceWorkspace.PathName + " ]");
            if (DestAdoOracleConnection != null)
            {
                _logger.Info("Transferring to dest connection [ " + DestAdoOracleConnection.ConnectionStringEncrypted + " ]");
            }
            else
            {
                _logger.Info("Transferring to dest connection [ " + DestinationWorkspace.PathName + " ]");
            }
            _logger.Info("Table to copy from [ " + _sourceTableName + " ] to [ " + _destTableName + " ]");
            if (UseAttributeFilter == true)
            {
                _logger.Info("Using Attribute Filter" + string.Join(",", AttributeFilterList.ToArray()));
            }
            else
            {
                _logger.Info("Copying All Attributes");
            }

        }

        private ITable OpenSourceTable()
        {
            _logger.Debug(MethodBase.GetCurrentMethod().Name);

            if (SourceOleDbWorkspaceConnection != null)
            {
                return ((IFeatureWorkspace)SourceOleDbWorkspaceConnection.Workspace).OpenTable(_sourceTableName);
            }
            else if (SourceSdeWorkspaceConnection.Workspace is IVersionedWorkspace)
            {
                return ((IFeatureWorkspace)SourceSdeWorkspaceConnection.NonVersionedEditsWorkspace).OpenTable(_sourceTableName);
            }
            else
            {
                return ((IFeatureWorkspace)SourceSdeWorkspaceConnection.Workspace).OpenTable(_sourceTableName);
            }
        }
        private ITable OpenDestinationTable()
        {
            _logger.Debug(MethodBase.GetCurrentMethod().Name);

            if (DestOleDbWorkspaceConnection != null)
            {
                return ((IFeatureWorkspace)DestOleDbWorkspaceConnection.Workspace).OpenTable(_destTableName);
            }
            else if (DestSdeWorkspaceConnection != null && DestSdeWorkspaceConnection.Workspace is IVersionedWorkspace)
            {
                return ((IFeatureWorkspace)DestSdeWorkspaceConnection.NonVersionedEditsWorkspace).OpenTable(_destTableName);
            }
            else if (DestSdeWorkspaceConnection != null && DestSdeWorkspaceConnection.Workspace != null)
            {
                return ((IFeatureWorkspace)DestSdeWorkspaceConnection.Workspace).OpenTable(_destTableName);
            }
            else
            {
                return null;
            }
        }

        private void OpenTables()
        {
            _logger.Debug(MethodBase.GetCurrentMethod().Name);

            if (SourceAdoOracleConnection != null && DestAdoOracleConnection != null) return;

            _sourceTable = OpenSourceTable();
            _destTable = OpenDestinationTable();

            EnsureFieldsAreCached(_sourceTable, _sourceCachedFields);
            EnsureFieldsAreCached(_destTable, _destCachedFields);
            //            EnsureFieldsAreEquivalent();
        }

        public void Dispose()
        {
            _logger.Debug(MethodBase.GetCurrentMethod().Name);

            _destCachedFields.Clear();
            _sourceCachedFields.Clear();
            _destCachedFields = null;
            _sourceCachedFields = null;
            if (_sourceTable != null) Marshal.FinalReleaseComObject(_sourceTable);
            if (_destTable != null) Marshal.FinalReleaseComObject(_destTable);
            _sourceTable = null;
            _destTable = null;
            this.SourceSdeWorkspaceConnection = null;
            this.DestSdeWorkspaceConnection = null;
        }

        public void CopyRows()
        {
            _logger.Debug(MethodBase.GetCurrentMethod().Name);

            IQueryFilter qf = null;
            ICursor sourceCursor = null;
            IRow sourceRow = null;
            int rowCount = 0;

            try
            {
                qf = new QueryFilterClass();
                qf.WhereClause = "1=1";

                if (TruncateDestTableBeforeWriting == true)
                {
                    TruncateDestTable();
                }

                if (KeyAttribute == null)
                {
                    CopyByGPTool();
                }
                else
                {
                    sourceCursor = _sourceTable.Search(qf, false);
                    CacheKeyAttributeField(_sourceTable.Fields);

                    while ((sourceRow = sourceCursor.NextRow()) != null)
                    {
                        if (sourceRow.HasOID && _maxSourceObjectID < sourceRow.OID) _maxSourceObjectID = sourceRow.OID;

                        if (!String.IsNullOrEmpty(KeyAttribute) && Synchronize)
                        {
                            object primaryKey = GetSanitizedObjectValue(sourceRow.get_Value(_keyFieldIndex));
                            if (!_primaryKeys.Contains(primaryKey))
                            {
                                _primaryKeys.Add(primaryKey);
                            }
                        }
                        bool destRowExists;
                        if (KeyAttribute == null || (destRowExists = DestRowExists(sourceRow)) == false)
                        {
                            if (DestAdoOracleConnection != null)
                            {
                                InsertRowAdoOracle(sourceRow);
                            }
                            else
                            {
                                InsertRowOracle(sourceRow);
                            }
                            if (++rowCount % 1000 == 0)
                            {
                                _logger.Debug("Transferred [ " + rowCount + " ] rows");
                            }
                            //_logger.Debug("Read [ " + ++rowCount + " ] rows");
                        }
                        else if (destRowExists && UpdateIfRowExists)
                        {
                            if (DestAdoOracleConnection != null)
                            {
                                UpdateRowAdoOracle(sourceRow);
                            }
                            else
                            {
                                UpdateRowOracle(sourceRow);
                            }
                        }

                    }
                    _logger.Info("Transferred [ " + rowCount + " ] rows from table [ " + ((IDataset)_sourceTable).Name + " ]");
                }
            }
            catch (Exception exception)
            {
                _logger.Error(exception.ToString());

                throw exception;
            }
            finally
            {
                if (qf != null) Marshal.FinalReleaseComObject(qf);
                if (sourceCursor != null) Marshal.FinalReleaseComObject(sourceCursor);
                if (sourceRow != null) Marshal.FinalReleaseComObject(sourceRow);
            }
        }

        // THis is necessary because of Oracle NUMBER(38) coming out as double. let's manhandle to long
        private object GetSanitizedObjectValue(object value)
        {
            if (_keyFieldTypeIsNumeric)
            {
                return Convert.ToInt64(value);
            }
            return value;
        }

        private void CacheKeyAttributeField(IFields fields)
        {
            if (String.IsNullOrEmpty(KeyAttribute)) return;

            _logger.Debug(MethodBase.GetCurrentMethod().Name);

            _keyFieldIndex = fields.FindField(KeyAttribute);

            switch (fields.get_Field(_keyFieldIndex).Type)
            {
                case esriFieldType.esriFieldTypeDouble:
                case esriFieldType.esriFieldTypeInteger:
                case esriFieldType.esriFieldTypeOID:
                case esriFieldType.esriFieldTypeSingle:
                case esriFieldType.esriFieldTypeSmallInteger:
                    _keyFieldTypeIsNumeric = true;
                    break;
                default:
                    _keyFieldTypeIsNumeric = false;
                    break;
            }
        }

        private string GetKeyAttributeSqlValue(object keyAttributeValue)
        {
            if (_keyFieldTypeIsNumeric)
            {
                return Convert.ToString(keyAttributeValue);
            }

            return "'" + Convert.ToString(keyAttributeValue) + "'";
        }

        private bool DestRowExistsAdoOracle(IRow sourceRow)
        {
            //_logger.Debug(MethodBase.GetCurrentMethod().Name);

            object sourceValue = GetSanitizedObjectValue(sourceRow.get_Value(_sourceCachedFields[KeyAttribute]));
            string whereClause = KeyAttribute + " =" + GetKeyAttributeSqlValue(sourceValue);

            string existsSql = "select count(*) from dual where exists (select * from " + _destTableName + " where " +
                         whereClause + ")";
            OracleCommand existsCommand = new OracleCommand(existsSql, DestAdoOracleConnection.OracleConnection);
            existsCommand.Transaction = DestAdoOracleConnection.DbTransaction;
            decimal numRows = Convert.ToDecimal(existsCommand.ExecuteScalar());

            return (numRows > 0);
        }

        private bool DestRowExists(IRow sourceRow)
        {
            //_logger.Debug(MethodBase.GetCurrentMethod().Name);

            bool rowExists;

            if (DestAdoOracleConnection != null)
            {
                rowExists = DestRowExistsAdoOracle(sourceRow);
            }
            else
            {
                IRow existingRow = null;
                try
                {
                    existingRow = GetExistingRow(sourceRow);
                    rowExists = (existingRow != null);
                }
                catch (Exception)
                {

                    throw;
                }
                finally
                {
                    if (existingRow != null) Marshal.FinalReleaseComObject(existingRow);
                }
            }

            return rowExists;
        }

        public void CopyByGPTool()
        {
            _logger.Debug(MethodBase.GetCurrentMethod().Name);

            if (_destTableName == _checkdestTable)
            {
                ProcessStartInfo start = default;
                string sourceConn = string.Empty;
                string destConn = string.Empty;
                string pythonexe = string.Empty;
                string pythonfile = string.Empty;
                string arguments = string.Empty;
                string standardError = string.Empty;
                List<string> argumentsList = new List<string>();
                List<string> pythonexeList = new List<string>();

                sourceConn = getConnectionDetail(((IDataset)_sourceTable).Workspace);
                destConn = getConnectionDetail(((IDataset)_destTable).Workspace);

                #region Python Path
                pythonexeList.Add(@"D:\python27\ArcGIS10.8\python.exe");
                pythonexeList.Add(@"c:\python27\arcgisx6410.1\python.exe");
                pythonexeList.Add(@"d:\arcgis10.0\python.exe");
                pythonexeList.Add(@"c:\arcgis10.0\python.exe");
                pythonexeList.Add(@"d:\python26\arcgis10.0\python.exe");
                pythonexeList.Add(@"c:\python26\arcgis10.0\python.exe");
                pythonexeList.Add(@"d:\python27\arcgis10.1\python.exe");
                pythonexeList.Add(@"c:\python27\arcgis10.1\python.exe");
                pythonexeList.Add(@"d:\python27\arcgis10.2\python.exe");
                pythonexeList.Add(@"c:\python27\arcgis10.2\python.exe");
                pythonexeList.Add(@"c:\python27\arcgis10.8\python.exe");
                #endregion

                foreach (string str in pythonexeList)
                {
                    if (File.Exists(str))
                    {
                        pythonexe = str;
                    }
                }

                if (string.IsNullOrWhiteSpace(pythonexe))
                    throw new Exception("Valid Python Path not found");

                //Python File Name
                string dir = Assembly.GetExecutingAssembly().Location.Replace("PGE.BatchApplication.ChangeDetection.ETL.dll","");
                pythonfile = System.IO.Path.Combine(dir, "Python Files","append.py");
                pythonfile = "\"" + pythonfile + "\"";
                argumentsList.Add(sourceConn);
                argumentsList.Add(_sourceTableName);
                argumentsList.Add(destConn);
                argumentsList.Add(_destTableName);
                if (DisableSchemaTest)
                    argumentsList.Add("NO_TEST");
                else
                    argumentsList.Add("TEST");
                arguments = string.Join(" ",argumentsList.ToArray());

                start = new ProcessStartInfo();
                start.FileName = pythonexe;
                start.Arguments = string.Format("{0} {1}", pythonfile, arguments);
                start.UseShellExecute = false;
                start.RedirectStandardOutput = true;
                start.RedirectStandardError = true;

                using (Process process = Process.Start(start))
                {
                    using (StreamReader reader = process.StandardOutput)
                    {
                        string result = reader.ReadToEnd();
                        Console.Write(result);
                    }
                    standardError = process.StandardError.ReadToEnd();
                }

                if(!string.IsNullOrWhiteSpace(standardError))
                {
                    throw new Exception("GP Tool Copy Failed with Error:" + standardError);
                }
            }
            else
            {
                ESRI.ArcGIS.DataManagementTools.Append append = new Append();
                append.inputs = _sourceTable;
                append.target = _destTable;
                if (DisableSchemaTest)
                {
                    append.schema_type = "NO_TEST";
                    // Odd -- if you don't set this then no attribute values are copied -- https://geonet.esri.com/thread/21322
                    append.field_mapping = GetFieldMapping(_sourceTable, _destTable);
                }
                else
                {
                    append.schema_type = "TEST";
                }

                ESRI.ArcGIS.Geoprocessor.Geoprocessor geoprocessor = new Geoprocessor();
                geoprocessor.Execute(append, null);


                //            IGeoProcessor2 gp = new GeoProcessorClass();
                //            gp.OverwriteOutput = true;
                //            IVariantArray parameters = new VarArrayClass();
                //            //string sourceTablePath = SourceSdeWorkspaceConnection.WorkspaceConnectionFile + "\\" + _sourceTableName;
                //            //string destTablePath = DestSdeWorkspaceConnection.WorkspaceConnectionFile + "\\" + _destTableName;
                //            parameters.Add(_sourceTable);
                //            parameters.Add(_destTable);
                //            if (DisableSchemaTest)
                //            {
                //                parameters.Add("NO_TEST");
                //                parameters.Add("");
                //            }
                //            else
                //            {
                //                parameters.Add("TEST");
                //                parameters.Add("#");
                //            }

                ////            parameters.Add("#");
                //            try
                //            {
                //                gp.Execute("Append_management", parameters, null);
                //            }
                //            catch (COMException exception)
                //            {
                //                if (exception.ErrorCode != -2147467259 && exception.ErrorCode != -2147417851)
                //                {
                //                    throw;
                //                }
                //            }
            }
            
         }

        private string getConnectionDetail(IWorkspace _workspace)
        {
            IPropertySet propertySet = default;
            IFeatureWorkspace featWorkspace = default;
            string dbUser = string.Empty;
            string dbInstance = string.Empty;
            try
            {
                propertySet = new PropertySet();
                featWorkspace = _workspace as IFeatureWorkspace;

                //Get User and Instance details from IPropertySet of Workspace
                propertySet = _workspace.ConnectionProperties;
                dbUser = Convert.ToString(propertySet.GetProperty("USER")).ToUpper();
                dbInstance = Convert.ToString(propertySet.GetProperty("INSTANCE")).Split(':').Last().ToUpper();
                if (dbInstance.Contains('$'))
                    dbInstance = dbInstance.Split('$').Last();

                if (string.IsNullOrWhiteSpace(dbUser) || string.IsNullOrWhiteSpace(dbUser))
                    throw new Exception("Invalid connection found"); 
            }
            catch(Exception ex)
            {
                throw ex;
            }
            return dbUser + "@" + dbInstance;
        }

        private IGPFieldMapping GetFieldMapping(ITable sourceTable, ITable destTable)
        {
            _logger.Debug(MethodBase.GetCurrentMethod().Name);

            // Create the GPUtilites object
            IGPUtilities gputilities = new GPUtilitiesClass();

            IArray inTables = new ArrayClass();
            inTables.Add(sourceTable);
            inTables.Add(destTable);

            IGPFieldMapping fieldmapping = new GPFieldMappingClass();
            fieldmapping.Initialize(inTables, null);

            return fieldmapping;
        }

        public void CopyByGPTool2()
        {
            _logger.Debug(MethodBase.GetCurrentMethod().Name);

            Geoprocessor GP = new Geoprocessor();

            Append appendTool = new Append();
            appendTool.inputs = _sourceTable;
            appendTool.target = _destTable;
            appendTool.schema_type = "NO_TEST";
            IGeoProcessorResult pResult = (IGeoProcessorResult)GP.Execute(appendTool, null);
            //for (int i = 0; i < pResult.MessageCount; i++)
            //{
            //    _logger.Info(pResult.GetMessage(i));
            //}
        }

        public void CopyRowsBuffer()
        {
            _logger.Debug(MethodBase.GetCurrentMethod().Name);

            IQueryFilter qf = null;
            ICursor sourceCursor = null;
            IRow sourceRow = null;
            int rowCount = 0;
            IRowBuffer tableBuffer = null;
            ICursor destCursor = null;
            IRow destRow = null;

            try
            {
                qf = new QueryFilterClass();
                qf.WhereClause = "1=1";

                if (TruncateDestTableBeforeWriting == true)
                {
                    TruncateDestTable();
                }

                if (KeyAttribute == null && UseGPTool )//&& _destTableName != _checkdestTable)
                {
                    CopyByGPTool();
                }
                else
                {
                    sourceCursor = _sourceTable.Search(qf, false);
                    while ((sourceRow = sourceCursor.NextRow()) != null)
                    {
                        if (sourceRow.HasOID && _maxSourceObjectID < sourceRow.OID) _maxSourceObjectID = sourceRow.OID;

                        #region not required right now, we are handeling this by using AH INFO Table insted of Posted Session History
                        //if(_sourceTableName == "EDGIS.PGE_EDERPostedSession")
                        //{
                        //    string FIELD_REGION = "REGION";
                        //    string FIELD_DISTRICT = "DISTRICT";
                        //    string FIELD_DIVISION = "DIVISION";
                        //    string FIELD_LOCALOFFICE = "LOCALOFFICE";
                        //    string FIELD_FEATUREGLOBALID = "FEATUREGLOBALID";
                        //    if(string.IsNullOrWhiteSpace(Convert.ToString(sourceRow.Value[_sourceTable.FindField(FIELD_REGION)]))
                        //        && string.IsNullOrWhiteSpace(Convert.ToString(sourceRow.Value[_sourceTable.FindField(FIELD_DISTRICT)]))
                        //        && string.IsNullOrWhiteSpace(Convert.ToString(sourceRow.Value[_sourceTable.FindField(FIELD_DIVISION)]))
                        //        && string.IsNullOrWhiteSpace(Convert.ToString(sourceRow.Value[_sourceTable.FindField(FIELD_LOCALOFFICE)]))
                        //        && string.IsNullOrWhiteSpace(Convert.ToString(sourceRow.Value[_sourceTable.FindField(FIELD_FEATUREGLOBALID)]))
                        //        )
                        //    {
                        //        continue;
                        //    }
                        //}
                        #endregion

                        if (KeyAttribute == null || !RowExistsInDestTable(sourceRow))
                        {
                            if (tableBuffer == null)
                            {
                                tableBuffer = _destTable.CreateRowBuffer();
                            }
                            if (destCursor == null)
                            {
                                destCursor = _destTable.Insert(true);
                            }
                            destRow = tableBuffer as IRow;
                            if (destRow is IFeature)
                            {
                                ((IFeature)destRow).Shape = ((IFeature)sourceRow).ShapeCopy;
                            }

                            CopyFields(sourceRow, destRow);
                            HandleRelatedTableInfos(sourceRow, destRow);
                            HandleDefaultAttributes(destRow);
                            IRowBuffer tableRowBuffer = tableBuffer as IRowBuffer;
                            object newOID = destCursor.InsertRow(tableRowBuffer);
                            //_logger.Debug("New Row [ " + newOID.ToString() + " ]");

                            if (++rowCount % 1000 == 0)
                            {
                                _logger.Debug("Transferred [ " + rowCount + " ] rows");
                            }
                            //_logger.Debug("Read [ " + ++rowCount + " ] rows");
                        }
                    }
                    _logger.Info("Transferred [ " + rowCount + " ] rows from table [ " + ((IDataset)_sourceTable).Name +
                                 " ]");
                }
            }
            catch (Exception exception)
            {
                _logger.Error(exception.ToString());

                throw exception;
            }
            finally
            {
                if (destCursor != null) Marshal.FinalReleaseComObject(destCursor);
                if (tableBuffer != null) Marshal.FinalReleaseComObject(tableBuffer);
                if (qf != null) Marshal.FinalReleaseComObject(qf);
                if (sourceCursor != null) Marshal.FinalReleaseComObject(sourceCursor);
                if (sourceRow != null) Marshal.FinalReleaseComObject(sourceRow);
                if (destRow != null) Marshal.FinalReleaseComObject(destRow);
            }
        }

        private void CopyRow(IRow sourceRow, IRow destRow)
        {
            if (destRow is IFeature)
            {
                ((IFeature)destRow).Shape = ((IFeature)sourceRow).ShapeCopy;
            }

            CopyFields(sourceRow, destRow);
            HandleRelatedTableInfos(sourceRow, destRow);
            HandleDefaultAttributes(destRow);
        }

        private void InsertRow(IRow sourceRow)
        {
            string oid = sourceRow.HasOID ? sourceRow.OID.ToString() : "None";
            _logger.Debug(MethodBase.GetCurrentMethod().Name + " sourceOID [ " + oid + " ]");

            IRow newRow = null;

            try
            {
                //                ((IWorkspaceEdit)((IDataset)_destTable).Workspace).StartEditOperation();
                newRow = _destTable.CreateRow();
                if (newRow is IFeature)
                {
                    ((IFeature) newRow).Shape = ((IFeature) sourceRow).ShapeCopy;
                }
                CopyFields(sourceRow, newRow);
                HandleRelatedTableInfos(sourceRow, newRow);
                HandleDefaultAttributes(newRow);

                newRow.Store();
                //             ((IWorkspaceEdit)((IDataset)_destTable).Workspace).StopEditOperation();

            }
            catch (Exception exception)
            {
                _logger.Error(exception.ToString());
                throw exception;
            }
            finally
            {
                if (newRow != null)     Marshal.FinalReleaseComObject(newRow);
            }
        }

        private string GetSQLInsertString(IRow sourceRow, bool includeGlobalID)
        {
            string insertString = " (";
            string insertFields = "";
            string insertValues = "";
            int i = 0;

            foreach (var sourceCachedField in _sourceCachedFields)
            {
                if (!includeGlobalID && i++ == 0) continue;
                insertFields += sourceCachedField.Key + ",";
            }

            i = 0;
            foreach (var sourceCachedField in _sourceCachedFields)
            {
                if (!includeGlobalID && i++ == 0) continue;
                insertValues += GetFieldValue(sourceRow, sourceCachedField.Key, sourceCachedField.Value) + ",";
            }

            if (insertFields.EndsWith(","))
            {
                insertFields = insertFields.Substring(0, insertFields.Length - 1);
            }
            if (insertValues.EndsWith(","))
            {
                insertValues = insertValues.Substring(0, insertValues.Length - 1);
            }

            insertString += insertFields + ") VALUES (" + insertValues + ")";

            return insertString;
        }

        private string GetOracleDateStringFromField(IRow row, int fieldIndex)
        {
            string oracleDateString = "NULL";

            object oracleDateValue = row.get_Value(fieldIndex);

            if (oracleDateValue != null && oracleDateValue != DBNull.Value)
            {
                DateTime dateTime = Convert.ToDateTime(oracleDateValue);
                oracleDateString = dateTime.GetOracleDateString();
            }

            return oracleDateString;
        }

        private string GetFieldValue(IRow row, string fieldName, int fieldIndex)
        {
            string fieldValue = "";

            if (row.Fields.get_Field(fieldIndex).Type == esriFieldType.esriFieldTypeDate)
            {
                fieldValue = GetOracleDateStringFromField(row, fieldIndex);
            }
            else
            {
                fieldValue = row.GetValueAsString(fieldName).GetDatabaseStringValue();
            }

            return fieldValue;
        }

        private string GetSQLUpdateString(IRow sourceRow)
        {
            string updateString = "SET ";

            foreach (var sourceCachedField in _sourceCachedFields)
            {
                if (sourceCachedField.Key == KeyAttribute) continue;

                updateString += sourceCachedField.Key + "=" + GetFieldValue(sourceRow, sourceCachedField.Key, sourceCachedField.Value) + ",";
            }

            if (updateString.EndsWith(","))
            {
                updateString = updateString.Substring(0, updateString.Length - 1);
            }

            updateString += " WHERE " + KeyAttribute + "=" +
                            sourceRow.GetValueAsString(KeyAttribute).GetDatabaseStringValue();

            return updateString;
        }

        private void UpdateRowOracle(IRow sourceRow)
        {
            string keyId = sourceRow.GetValueAsString(KeyAttribute);
            _logger.Debug("Updating " + KeyAttribute + " [ " + keyId + " ]");

            try
            {
                string updateString = "UPDATE " + _destTableName + " " + GetSQLUpdateString(sourceRow);
                ((IDataset)_destTable).Workspace.ExecuteSQL(updateString);
            }
            catch (Exception exception)
            {
                _logger.Error(exception.ToString());
                throw exception;
            }
        }

        private void UpdateRowAdoOracle(IRow sourceRow)
        {
            string keyId = sourceRow.GetValueAsString(KeyAttribute);
            _logger.Debug("Updating " + KeyAttribute + " [ " + keyId + " ]");

            try
            {
                string updateStringSql = "UPDATE " + _destTableName + " " + GetSQLUpdateString(sourceRow);
                OracleCommand updateCommand = new OracleCommand(updateStringSql, DestAdoOracleConnection.OracleConnection);
                updateCommand.Transaction = DestAdoOracleConnection.DbTransaction;
                updateCommand.ExecuteNonQuery();
            }
            catch (Exception exception)
            {
                _logger.Error(exception.ToString());
                throw;
            }
        }


        private void InsertRowOracle(IRow sourceRow)
        {
            string keyId = sourceRow.GetValueAsString(KeyAttribute);
            _logger.Debug("Inserting " + KeyAttribute + " [ " + keyId + " ]");

            IRow newRow = null;

            try
            {
                string insertString = "INSERT INTO " + _destTableName + " " + GetSQLInsertString(sourceRow, true);
                ((IDataset)_destTable).Workspace.ExecuteSQL(insertString);
            }
            catch (Exception exception)
            {
                _logger.Error(exception.ToString());
                throw;
            }
            finally
            {
                if (newRow != null) Marshal.FinalReleaseComObject(newRow);
            }
        }

        private void InsertRowAdoOracle(IRow sourceRow)
        {
            string keyId = sourceRow.GetValueAsString(KeyAttribute);
            _logger.Debug("Inserting " + KeyAttribute + " [ " + keyId + " ]");

            try
            {
                string insertStringSql = "INSERT INTO " + _destTableName + " " + GetSQLInsertString(sourceRow, true);
                OracleCommand insertCommand = new OracleCommand(insertStringSql, DestAdoOracleConnection.OracleConnection);
                insertCommand.Transaction = DestAdoOracleConnection.DbTransaction;
                insertCommand.ExecuteNonQuery();
            }
            catch (Exception exception)
            {
                _logger.Error(exception.ToString());
                throw;
            }
        }

        private void HandleDefaultAttributes(IRow newRow)
        {
            //            _logger.Debug(MethodBase.GetCurrentMethod().Name);

            if (DefaultDestAttributes != null)
            {
                foreach (KeyValuePair<string, string> defaultDestAttribute in DefaultDestAttributes)
                {
                    if (!String.IsNullOrEmpty(defaultDestAttribute.Value))
                    {
                        newRow.set_Value(_destCachedFields[defaultDestAttribute.Key], defaultDestAttribute.Value);
                    }
                    else
                    {
                        if (newRow.Fields.get_Field(_destCachedFields[defaultDestAttribute.Key]).Type ==
                            esriFieldType.esriFieldTypeDate)
                        {
                            newRow.set_Value(_destCachedFields[defaultDestAttribute.Key], DateTime.Now);
                        }
                    }
                }
            }
        }

        // This stuff was all thrown in at the 11th hour
        private void HandleRelatedTableInfos(IRow sourceRow, IRow destRow)
        {
            //            _logger.Debug(MethodBase.GetCurrentMethod().Name);

            if (!string.IsNullOrEmpty(RelatedTableName) & !string.IsNullOrEmpty(KeyAttribute))
            {
                HandleRelatedTableInfo(sourceRow, destRow, RelatedTableName , RelatedTableKey, RelatedTableField);
            }

        }

        private void HandleRelatedTableInfo(IRow sourceRow, IRow destRow, string relatedTableName, string relatedTableKey, string fieldName)
        {
            _logger.Debug(MethodBase.GetCurrentMethod().Name);
            _logger.Debug("Related Table [ " + relatedTableName + " ] key [ " + relatedTableKey + " ] fieldName [ " + fieldName + " ] attributeKey [ " + KeyAttribute + " ]");

            ITable relatedTable = ((IFeatureWorkspace) DestSdeWorkspaceConnection.NonVersionedEditsWorkspace).OpenTable(relatedTableName);

            IQueryFilter qf = null;
            ICursor cursor = null;
            IRow relatedRow = null;

            try
            {
                qf = new QueryFilterClass();
                object sourceValue = GetSanitizedObjectValue(sourceRow.get_Value(sourceRow.Fields.FindField(KeyAttribute)));
                qf.WhereClause = relatedTableKey + " = " + GetKeyAttributeSqlValue(sourceValue);
                cursor = relatedTable.Search(qf, false);
                relatedRow = cursor.NextRow();
                if (relatedRow != null)
                {
                    string fieldValue = relatedRow.GetValueAsString(fieldName);
                    destRow.set_Value(_destCachedFields[GetDestAttributeName(fieldName)], fieldValue);
                }
            }
            catch (Exception exception)
            {
                _logger.Error(exception.ToString());
                throw exception;
            }
            finally
            {
                if (qf != null) Marshal.FinalReleaseComObject(qf);
                if (cursor != null) Marshal.FinalReleaseComObject(cursor);
                if (relatedRow != null) Marshal.FinalReleaseComObject(relatedRow);
                if (relatedTable != null) Marshal.FinalReleaseComObject(relatedTable);
            }
        }

        private void CopyFields(IRow sourceRow, IRow destRow)
        {
            //            _logger.Debug(MethodBase.GetCurrentMethod().Name);

            if (UseAttributeFilter)
            {
                // Only copy the fields spec'd in config
                foreach (string attributeName in AttributeFilterList)
                {
                    try
                    {
                        CopyField(sourceRow, _sourceCachedFields[attributeName], destRow, _destCachedFields[GetDestAttributeName(attributeName)]);
                    }
                    catch (Exception exception)
                    {
                        _logger.Error("Error copying field [ " + attributeName + " ]");

                        throw exception;
                    }
                }
            }
            else
            {
                foreach (KeyValuePair<string, int> sourceCachedField in _sourceCachedFields)
                {
                    try
                    {
                        CopyField(sourceRow, sourceCachedField.Value, destRow, _destCachedFields[GetDestAttributeName(sourceCachedField.Key)]);
                    }
                    catch (Exception exception)
                    {
                        _logger.Error("Error copying field [ " + sourceCachedField.Key + " ]");

                        throw exception;
                    }
                }
            }
        }

        private void CopyField(IRow sourceRow, int sourceFieldIndex, IRow destRow, int destFieldIndex)
        {
            //_logger.Debug(MethodBase.GetCurrentMethod().Name);

            if (sourceRow.Fields.get_Field(sourceFieldIndex).Name == "SHAPE.LEN") return;

            object sourceFieldValue = sourceRow.get_Value(sourceFieldIndex);
            if (sourceFieldValue == null || sourceFieldValue is DBNull)
            {
                destRow.set_Value(destFieldIndex, DBNull.Value);
                return;
            }

            switch (sourceRow.Fields.get_Field(sourceFieldIndex).Type)
            {
                // Ignore
                case esriFieldType.esriFieldTypeBlob:
                case esriFieldType.esriFieldTypeGlobalID:
                case esriFieldType.esriFieldTypeOID:
                case esriFieldType.esriFieldTypeRaster:
                case esriFieldType.esriFieldTypeXML:
                case esriFieldType.esriFieldTypeGeometry:
                    break;
                // Copy
                case esriFieldType.esriFieldTypeDouble:
                    destRow.set_Value(destFieldIndex, Convert.ToDouble(sourceFieldValue));
                    break;
                case esriFieldType.esriFieldTypeDate:
                case esriFieldType.esriFieldTypeInteger:
                case esriFieldType.esriFieldTypeSingle:
                case esriFieldType.esriFieldTypeSmallInteger:
                case esriFieldType.esriFieldTypeGUID:
                case esriFieldType.esriFieldTypeString:
                    if (_destTableName != _checkdestTable)
                    {
                        destRow.set_Value(destFieldIndex, sourceFieldValue);
                    }
                    else if (_destTableName == _checkdestTable && destFieldIndex != 0)
                    {
                        destRow.set_Value(destFieldIndex, sourceFieldValue);
                    }
                    break;
                default:
                    break;
            }

        }

        private string GetDestAttributeName(string sourceAttributeName)
        {
            if (AttributeMapping != null)
            {
                if (AttributeMapping.ContainsKey(sourceAttributeName))
                {
                    return AttributeMapping[sourceAttributeName];
                }
            }

            return sourceAttributeName;
        }

        private IRow GetRowFromDestTable(IRow row)
        {
            //_logger.Debug(MethodBase.GetCurrentMethod().Name);

            if (KeyAttribute == null) return null;

            IRow existingRow = null;
            IQueryFilter qf = null;
            ICursor destCursor = null;

            try
            {
                object sourceValue = GetSanitizedObjectValue(row.get_Value(_sourceCachedFields[KeyAttribute]));
                string whereClause = GetDestAttributeName(KeyAttribute) + " =" + GetKeyAttributeSqlValue(sourceValue);

                qf = new QueryFilterClass();
                qf.WhereClause = whereClause;

                destCursor = _destTable.Search(qf, false);
                existingRow = destCursor.NextRow();

            }
            catch (Exception exception)
            {
                _logger.Error(exception.ToString());

                throw exception;
            }
            finally
            {
                if (qf != null) Marshal.FinalReleaseComObject(qf);
                if (destCursor != null) Marshal.FinalReleaseComObject(destCursor);
            }

            return existingRow;
        }

        private IRow GetExistingRow(IRow row)
        {
            //_logger.Debug(MethodBase.GetCurrentMethod().Name);

            if (KeyAttribute == null) return null;

            IRow existingRow = null;
            IQueryFilter qf = null;
            ICursor destCursor = null;

            try
            {
                object sourceValue = GetSanitizedObjectValue(row.get_Value(_sourceCachedFields[KeyAttribute]));
                string whereClause = GetDestAttributeName(KeyAttribute) + " =" + GetKeyAttributeSqlValue(sourceValue);

                qf = new QueryFilterClass();
                qf.WhereClause = whereClause;

                destCursor = _destTable.Search(qf, false);
                existingRow = destCursor.NextRow();
            }
            catch (Exception exception)
            {
                _logger.Error(exception.ToString());

                throw exception;
            }
            finally
            {
                if (qf != null) Marshal.FinalReleaseComObject(qf);
                if (destCursor != null) Marshal.FinalReleaseComObject(destCursor);
            }

            return existingRow;
        }

        private bool RowExistsInDestTable(IRow row)
        {
            //_logger.Debug(MethodBase.GetCurrentMethod().Name);

            IRow existingRow = null;
            bool rowExists = false;

            try
            {
                existingRow = GetExistingRow(row);
                rowExists = existingRow != null;
            }
            catch (Exception exception)
            {
                _logger.Error(exception.ToString());

                throw exception;
            }
            finally
            {
                if (existingRow != null) Marshal.FinalReleaseComObject(existingRow);
            }

            return rowExists;
        }

        public void TruncateSourceTableAdo()
        {
            _logger.Debug(MethodBase.GetCurrentMethod().Name);

            OracleCommand truncateCommand = new OracleCommand("delete from " + _sourceTableName, SourceAdoOracleConnection.OracleConnection);
            truncateCommand.ExecuteNonQuery();
        }

        public void TruncateSourceTable()
        {
            _logger.Debug(MethodBase.GetCurrentMethod().Name);

            IQueryFilter qf = null;

            try
            {
                AOHelper.EnsureInEditSession(((IDataset)_sourceTable).Workspace);
                qf = new QueryFilterClass();
                if (_sourceTable.HasOID)
                {
                    _logger.Debug("Deleting < [ " + _maxSourceObjectID + " ]");
                    //if (_sourceTableName != "EDGIS.PGE_EDERPostedSession")
                        qf.WhereClause = _sourceTable.OIDFieldName + " <= " + _maxSourceObjectID;
                    #region not required right now, we are handeling this by using AH INFO Table insted of Posted Session History
                    //else
                    //{
                    //    string FIELD_REGION = "REGION";
                    //    string FIELD_DISTRICT = "DISTRICT";
                    //    string FIELD_DIVISION = "DIVISION";
                    //    string FIELD_LOCALOFFICE = "LOCALOFFICE";
                    //    string FIELD_FEATUREGLOBALID = "FEATUREGLOBALID";
                    //    qf.WhereClause = _sourceTable.OIDFieldName + " <= " + _maxSourceObjectID
                    //        + " AND " + FIELD_REGION + " is not null"
                    //        + " AND " + FIELD_DISTRICT + " is not null"
                    //        + " AND " + FIELD_DIVISION + " is not null"
                    //        + " AND " + FIELD_LOCALOFFICE + " is not null"
                    //        + " AND " + FIELD_FEATUREGLOBALID + " is not null";
                    //}
                    #endregion
                }
                else
                {
                    _logger.Debug("Deleting all rows in source table [ " + ((IDataset)_sourceTable).Name + " ]");
                }
                _sourceTable.DeleteSearchedRows(qf);
            }
            catch (Exception exception)
            {
                _logger.Error(exception.ToString());

                throw exception;
            }
            finally
            {
                if (qf != null) Marshal.FinalReleaseComObject(qf);
            }
        }

        public void TruncateDestTable()
        {
            _logger.Debug(MethodBase.GetCurrentMethod().Name);

            IQueryFilter qf = null;

            try
            {
                if (((IDataset) _destTable).Workspace is IVersionedWorkspace)
                {
                    AOHelper.EnsureInEditSession(((IDataset)_destTable).Workspace);
                }

                string sql = "DELETE FROM " + ((IDataset)_destTable).Name;
                _logger.Debug(sql);
                ((IDataset)_destTable).Workspace.ExecuteSQL(sql);
            }
            catch (Exception exception)
            {
                _logger.Error(exception.ToString());

                throw exception;
            }
            finally
            {
                if (qf != null) Marshal.FinalReleaseComObject(qf);
                _logger.Debug("Exiting " + MethodBase.GetCurrentMethod().Name);
            }
        }

    }
}
