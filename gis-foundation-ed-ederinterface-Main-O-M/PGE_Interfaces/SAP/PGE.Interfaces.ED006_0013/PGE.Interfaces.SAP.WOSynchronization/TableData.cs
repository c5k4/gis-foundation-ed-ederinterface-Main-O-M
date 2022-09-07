using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Runtime.InteropServices;
using System.Xml;
using log4net;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.DataSourcesGDB;
using ESRI.ArcGIS.Geodatabase;

namespace PGE.Interfaces.SAP.WOSynchronization
{
    /// <summary>
    /// Class that allows deal with a SDE ITable and perfrom operation like Search and provided output in specific field order
    /// </summary>
    public class TableData : IDisposable
    {
        #region Protected & Private Variables

        /// <summary>
        /// Holds reference to the ITable instance
        /// </summary>
        protected ITable _table = null;
        /// <summary>
        /// Logger to log error / debug/ user information
        /// </summary>
        protected log4net.ILog _logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        /// <summary>
        /// Holds the Field Name and Field Index Mapping dictionary for the table in question i.e.<see cref="_table"/>
        /// </summary>
        private IDictionary<string, int> _fieldNameAndIndexMapping;
        private IList<string> _jobOrders = null;
        #endregion Private Variables

        #region Constructor

        /// <summary>
        /// Initializes the instance of <see cref="TableData"/> class.
        /// </summary>
        /// <param name="workspace">Workspace reference</param>
        /// <param name="tableOrFeatureClassName">Table or Feature Class Name to which this object deals with. Need to provided fully qualified name.</param>
        public TableData(IWorkspace workspace, string tableOrFeatureClassName)
        {
            _table = GetTable(workspace, tableOrFeatureClassName);
            if (_table == null)
            {
                throw new COMException("Could not retrieve a table with '" + tableOrFeatureClassName + "' name. Please ensure that database is valid and fully qualified table name is provided.");
            }
            TableName = tableOrFeatureClassName;
        }

        #endregion Constructor

        #region Public Properties

        /// <summary>
        /// <c>JobOrder</c> field name of ITable in question ie.e <see cref="_table"/>
        /// </summary>
        public virtual string JobOrderNumberFieldName
        {
            get;
            set;
        }

        /// <summary>
        /// Name of the table
        /// </summary>
        public string TableName
        {
            get;
            private set;
        }
        #endregion Public Properties

        #region Public Methods

        /// <summary>
        /// Disposes the resources
        /// </summary>
        public virtual void Dispose()
        {
            if (_table != null)
            {
                Marshal.ReleaseComObject(_table); _table = null;
            }
        }

        /// <summary>
        /// Determines whether a records exists with the given <paramref name="jobOrderNumber"/>
        /// </summary>
        /// <param name="jobOrderNumber">JobNumber to check</param>
        /// <returns>Returns true if at least a record exists with <paramref name="jobOrderNumber"/>; false, otherwise.</returns>
        /// <remarks>
        /// If <paramref name="jobOrderNumber"/> = Null or Empty, then returns false
        /// </remarks>
        public virtual bool RecordExists(string jobOrderNumber)
        {
            bool exists = false;
            if (string.IsNullOrEmpty(jobOrderNumber)) return exists;
            //IQueryFilter filter = new QueryFilterClass();
            //filter.WhereClause = JobOrderNumberFieldName + "='" + jobOrderNumber + "'";
            //exists = (_table.RowCount(filter) > 0);
            //return exists;
            return _jobOrders.Contains(jobOrderNumber);
        }

        /// <summary>
        /// Prepares the List for the given filter and in the order specified
        /// </summary>
        /// <param name="whereClause">WhereClause to look for records</param>
        /// <param name="fieldOrder">Order of fields to be set in <see cref="IRowData2"/> of list.</param>
        /// <returns>Returns the List for the given filter and in the order specified</returns>
        public virtual IList<IRowData2> SearchTable(string whereClause, string[] fieldOrder)
        {
            IList<IRowData2> resultDataRows = new List<IRowData2>();
            //Prepare Query filter and get the rows satisfying the filter
            IQueryFilter filter = new QueryFilterClass();
            filter.WhereClause = whereClause;
            ICursor cursor = _table.Search(filter, true);

            //Read the field order from config setting and prepare the required dictionary
            _fieldNameAndIndexMapping = PrepareFieldIndexDictionary(fieldOrder, _table);

            IRow row = null;
            IRowData2 resultDataRow = null;
            while ((row = cursor.NextRow()) != null)
            {
                //Prepare IRowData2 from IRow in the specified order
                resultDataRow = FillRowData(row);
                if (resultDataRow != null)
                {
                    resultDataRows.Add(resultDataRow);
                }
            }
            Marshal.ReleaseComObject(cursor);
            return resultDataRows;
        }

        /// <summary>
        /// Loads the JobNumbres into list for faster check the record existence
        /// </summary>
        public virtual void LoadCache()
        {
            try
            {
                _jobOrders = new List<string>();
                IQueryFilter filter = new QueryFilterClass();
                filter.WhereClause = JobOrderNumberFieldName + " is not null";
                ICursor cursor = _table.Search(filter, true);
                int jobNoIndex = _table.FindField(JobOrderNumberFieldName);

                IRow row = null;
                while ((row = cursor.NextRow()) != null)
                {
                    _jobOrders.Add(row.get_Value(jobNoIndex).ToString());
                }
                Marshal.ReleaseComObject(cursor);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion Public Methods

        #region Protected & Private Methods

        /// <summary>
        /// Opens the table from the Database
        /// </summary>
        /// <param name="wSpace">Reference to the database</param>
        /// <param name="tableName">Name of the table of FeatureClass</param>
        /// <returns>Returns the reference to Table/FeatureClass. Returns Null if Table / Feature Class does not exist.</returns>
        protected ITable GetTable(IWorkspace wSpace, string tableName)
        {
            if (wSpace == null) return null;
            if (string.IsNullOrEmpty(tableName)) return null;
            IWorkspace2 wSpace2 = wSpace as IWorkspace2;
            ITable table = null;
            IFeatureWorkspace featureWSpace = wSpace as IFeatureWorkspace;
            //Checks whether table exists with the given name
            if (wSpace2.get_NameExists(esriDatasetType.esriDTTable, tableName))
            {
                table = featureWSpace.OpenTable(tableName);
            }
            //Checks whether FeatureClass exists with the given name
            else if (wSpace2.get_NameExists(esriDatasetType.esriDTFeatureClass, tableName))
            {
                table = featureWSpace.OpenFeatureClass(tableName) as ITable;
            }
            if (table == null)
            {
                _logger.Debug(tableName + " does not exist in " + wSpace.PathName);
            }
            return table;
        }

        /// <summary>
        /// Prepares the Field Names and Field Index Mapping Dictionary
        /// </summary>
        /// <param name="fieldNames">Field Oder</param>
        /// <param name="table">Table reference to which the fields belong to</param>
        /// <returns>Returns the Field Names and Field Index Mapping Dictionary</returns>
        private IDictionary<string, int> PrepareFieldIndexDictionary(string[] fieldNames, ITable table)
        {
            if (table == null || fieldNames == null || fieldNames.Length < 1) return null;
            IDictionary<string, int> fieldIndexDictionary = new Dictionary<string, int>();
            int fieldIndex = -1;
            for (int i = 0; i < fieldNames.Length; i++)
            {
                fieldIndex = table.FindField(fieldNames[i]);
                if (fieldIndex == -1)
                {
                    _logger.Error(string.Format("The field ({0}) was not found in the table : {1}", fieldNames[i], (_table as IDataset).Name));
                }
                fieldIndexDictionary.Add(fieldNames[i], fieldIndex);
            }
            return fieldIndexDictionary;
        }

        /// <summary>
        /// Prepares IRowData2 from IRow in the specific order
        /// </summary>
        /// <param name="row">Instance of IRow</param>
        /// <returns>Returns the IRowDat2 filled with data from IRow</returns>
        private IRowData2 FillRowData(IRow row)
        {
            if (row == null) return null;
            IRowData2 rowData = new RowData2();
            Dictionary<string, string> fieldValues = new Dictionary<string, string>();
            int fieldIndex = -1;
            string fieldValue = string.Empty;
            object fieldObjValue = null;

            //Get the field values from IRow into IRowData2
            foreach (string fieldName in _fieldNameAndIndexMapping.Keys)
            {
                fieldIndex = _fieldNameAndIndexMapping[fieldName];
                if (fieldIndex == -1) fieldValue = string.Empty;
                else
                {
                    fieldObjValue = row.get_Value(fieldIndex);
                    if (fieldObjValue == null) fieldValue = string.Empty;
                    else fieldValue = fieldObjValue.ToString();
                }
                fieldValues.Add(fieldName, fieldValue);
            }

            //Attach field values to the IRowData2
            rowData.FieldValues = fieldValues;
            rowData.OID = row.OID;

            return rowData;

        }

        #endregion Protected & Private Methods
    }
}
