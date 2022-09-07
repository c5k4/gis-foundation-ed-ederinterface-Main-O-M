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

namespace Telvent.PGE.SAP.WOSynchronization
{
    public class TableCache : IDisposable
    {
        private IWorkspace _wspace = null;
        private ITable _table = null;
        private log4net.ILog _logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private IDictionary<string, int> _nodeFieldIndexMapping;
        private IDictionary<string, int> _fieldNameAndIndexMapping;
        public virtual string JobOrderNumberFieldName
        {
            get;
            set;
        }

        public TableCache()
        {
        }

        public TableCache(string connectionString)
        {
        }

        public TableCache(IWorkspace workspace)
        {
            _wspace = workspace;
        }

        public TableCache(IWorkspace workspace, string tableName)
        {
            _table = GetTable(workspace, tableName);
        }

        public virtual void LoadTable(string tableName)
        {
            _table = GetTable(_wspace, tableName);
        }

        public virtual IList<IRowData2> SearchTable(string whereClause,string[] fieldOrder)
        {
            IList<IRowData2> resultDataRows = new List<IRowData2>();
            //Prepare Query filter
            IQueryFilter filter = new QueryFilterClass();
            filter.WhereClause = whereClause;
            ICursor cursor = _table.Search(filter, true);

            //Read the field order from config setting
            _fieldNameAndIndexMapping = PrepareFieldIndexDictionary(fieldOrder, _table);

            IRow row = null;
            IRowData2 resultDataRow = null;
            while ((row = cursor.NextRow()) != null)
            {
                resultDataRow = FillRowData(row);
                if (resultDataRow != null)
                {
                    resultDataRows.Add(resultDataRow);
                }
            }
            Marshal.ReleaseComObject(cursor);
            return resultDataRows;
        }

        private IRowData2 FillRowData(IRow row)
        {
            if (row == null) return null;
            IRowData2 rowData = new RowData2();
            Dictionary<string, string> fieldValues = new Dictionary<string, string>();
            int fieldIndex = -1;
            string fieldValue = string.Empty;
            object fieldObjValue = null;
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

        private IDictionary<string, int> PrepareFieldIndexDictionary(string[] fieldNames, ITable table)
        {
            if (table == null || fieldNames == null || fieldNames.Length < 1) return null;
            IDictionary<string, int> fieldIndexDictionary = new Dictionary<string, int>();
            for (int i = 0; i < fieldNames.Length; i++)
            {
                fieldIndexDictionary.Add(fieldNames[i], table.FindField(fieldNames[i]));
            }
            return fieldIndexDictionary;
        }

        public virtual void UpdateRow(IRowData2 row)
        {
            if (row == null) return;
            //Get the field value collection of the PM Order
            Dictionary<string, string> fieldValues = row.FieldValues;
            if (fieldValues.Count < 1) return;

            //Get the JobORDERNumber
            string jobOrderNumber = fieldValues[ResourceConstants.XMLNodeNames.JOBORDERNODENAME];
            //Get the Row with same jobOrder number
            List<IRow> resultSet = SearchDBTable(ResourceConstants.FieldNames.JOBORDER + "='" + jobOrderNumber + "'");
            for (int i = 0; i < resultSet.Count; i++)
            {
                IRow updateRow = resultSet[i];
                WriteFieldValues(updateRow, fieldValues);
                updateRow.Store();
            }
        }

        public virtual void InsertRow(IRowData2 row)
        {
            if (row == null) return;
            //Get the field value collection of the PM Order
            Dictionary<string, string> fieldValues = row.FieldValues;
            if (fieldValues.Count < 1) return;
            IRow newRow = _table.CreateRow();
            WriteFieldValues(newRow, fieldValues);
            newRow.Store();
        }

        private void WriteFieldValues(IRow targetRow, Dictionary<string, string> sourceFieldValues)
        {
            int fieldIndex = -1;
            esriFieldType fieldType;
            object fieldValue = null;

            foreach (string key in sourceFieldValues.Keys)
            {
                fieldIndex = XMLNodeFieldIndexMapping[key];
                if (fieldIndex == -1)
                {
                    _logger.Debug("No mapping field for XML Node: " + key); continue;
                }
                fieldType = _table.Fields.get_Field(fieldIndex).Type;
                fieldValue = Convert(sourceFieldValues[key], fieldType);
                targetRow.set_Value(fieldIndex, fieldValue);
            }
        }

        public virtual List<IRow> SearchDBTable(string whereClause)
        {
            List<IRow> resultsSet = new List<IRow>();
            //Prepare Query filter
            IQueryFilter filter = new QueryFilterClass();
            filter.WhereClause = whereClause;
            ICursor cursor = _table.Search(filter, false);

            IRow resultRow = null;
            while ((resultRow = cursor.NextRow()) != null)
            {
                resultsSet.Add(resultRow);
            }
            Marshal.ReleaseComObject(cursor);
            return resultsSet;
        }

        public virtual void Dispose()
        {
            _nodeFieldIndexMapping = null;
            if (_table != null)
            {
                Marshal.ReleaseComObject(_table); _table = null;
            }
            if (_wspace != null)
            {
                Marshal.ReleaseComObject(_wspace); _wspace = null;
            }
        }

        protected IDictionary<string, int> XMLNodeFieldIndexMapping
        {
            get
            {
                if (_nodeFieldIndexMapping == null) { PrepareFieldMappingMatrix(); }
                return _nodeFieldIndexMapping;
            }
        }

        public virtual bool RecordExists(string jobOrderNumber)
        {
            bool exists = false;
            if (string.IsNullOrEmpty(jobOrderNumber)) return exists;
            IQueryFilter filter = new QueryFilterClass();
            filter.WhereClause = JobOrderNumberFieldName + "='" + jobOrderNumber + "'";
            exists = (_table.RowCount(filter) > 0);
            return exists;
        }

        public void CleanRecords(string whereClause)
        {
            IQueryFilter filter = new QueryFilter();
            filter.WhereClause = whereClause;
            //Delete the records satisfying the whereclause
            _table.DeleteSearchedRows(filter);
        }

        #region Private Methods

        protected ITable GetTable(IWorkspace wSpace, string tableName)
        {
            if (wSpace == null) return null;
            if (string.IsNullOrEmpty(tableName)) return null;
            IWorkspace2 wSpace2 = wSpace as IWorkspace2;
            ITable table = null;
            IFeatureWorkspace featureWSpace = wSpace as IFeatureWorkspace;

            if (wSpace2.get_NameExists(esriDatasetType.esriDTTable, tableName))
            {
                table = featureWSpace.OpenTable(tableName);
            }
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

        private object Convert(string fieldValue, esriFieldType fieldType)
        {
            if (string.IsNullOrEmpty(fieldValue)) return DBNull.Value;

            switch (fieldType)
            {
                case esriFieldType.esriFieldTypeString:
                    return fieldValue;
                case esriFieldType.esriFieldTypeInteger:
                    return System.Convert.ToInt32(fieldValue);
                case esriFieldType.esriFieldTypeSmallInteger:
                    return System.Convert.ToInt16(fieldValue);
                case esriFieldType.esriFieldTypeDate:
                    return System.Convert.ToDateTime(fieldValue);
                case esriFieldType.esriFieldTypeDouble:
                    return System.Convert.ToDouble(fieldValue);
                default:
                    return DBNull.Value;
            }
        }

        private void PrepareFieldMappingMatrix()
        {
            IList<string> xmlNodeNames = GetFieldNamesFromXSD();
            //add the three common fields
            xmlNodeNames.Add(ResourceConstants.FieldNames.LastMessageDate);
            xmlNodeNames.Add(ResourceConstants.FieldNames.WIPFound);
            xmlNodeNames.Add(ResourceConstants.FieldNames.WIPMissing);

            //Get the user defined field mapping
            Configuration config = System.Configuration.ConfigurationManager.OpenExeConfiguration(ResourceConstants.ConfigFileLocation);
            ConfigurationSection configsection = config.GetSection("FieldMapping");
            
            NameValueCollection fieldMapping = ConfigurationManager.GetSection("FieldMapping") as NameValueCollection;
            IDictionary<string, string> nodeFieldMapping = new Dictionary<string, string>();

            foreach (string nodeName in fieldMapping.Keys)
            {
                nodeFieldMapping.Add(nodeName.ToUpper(), fieldMapping[nodeName].ToUpper());
            }

            _nodeFieldIndexMapping = new Dictionary<string, int>();

            for (int i = 0; i < xmlNodeNames.Count; i++)
            {
                //Get the corresponding field index from SDE database
                string fieldName = xmlNodeNames[i].ToUpper();
                if (nodeFieldMapping.ContainsKey(fieldName))
                {
                    fieldName = nodeFieldMapping[fieldName];
                }
                //Get the Field Index
                int fieldIndex = _table.Fields.FindField(fieldName);
                _nodeFieldIndexMapping.Add(xmlNodeNames[i].ToUpper(), fieldIndex);
            }

        }

        private IList<string> GetFieldNamesFromXSD()
        {
            IList<string> fieldNames = new List<string>();

            System.IO.Stream xsdStream = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream("Telvent.PGE.SAP.WOSynchronization.PMOrderXSD.xsd");
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(xsdStream);

            XmlNamespaceManager nsmanager = new XmlNamespaceManager(xmlDoc.NameTable);
            nsmanager.AddNamespace("xs", "http://www.w3.org/2001/XMLSchema");

            XmlNodeList columnNodeList = xmlDoc.DocumentElement.SelectNodes("//xs:element[@name='Table']//xs:element", nsmanager);

            for (int i = 0; i < columnNodeList.Count; i++)
            {
                fieldNames.Add(columnNodeList[i].Attributes["name"].Value);
            }

            return fieldNames;
        }

        public string GetAllJobNumbers()
        {
            IQueryFilter filter = new QueryFilterClass();
            filter.WhereClause = JobOrderNumberFieldName + " is not null or " + JobOrderNumberFieldName + "<>''";
            filter.SubFields = JobOrderNumberFieldName;
            int jobOrderFieldIndex = _table.FindField(JobOrderNumberFieldName);
            if (jobOrderFieldIndex == -1) return null;
            ICursor cursor = _table.Search(filter, true);
            IRow pmOrderRow = null;
            StringBuilder jobNumbers = new StringBuilder();
            object fieldValue;
            while ((pmOrderRow = cursor.NextRow()) != null)
            {
                fieldValue = pmOrderRow.get_Value(jobOrderFieldIndex);
                if (fieldValue == null || fieldValue == DBNull.Value) continue;
                jobNumbers.Append("'"+fieldValue.ToString() + "',");
            }
            if (jobNumbers.Length > 0) jobNumbers.Remove(jobNumbers.Length - 1, 1);
            return jobNumbers.ToString();
        }

        #endregion Private Methods
    }
}
