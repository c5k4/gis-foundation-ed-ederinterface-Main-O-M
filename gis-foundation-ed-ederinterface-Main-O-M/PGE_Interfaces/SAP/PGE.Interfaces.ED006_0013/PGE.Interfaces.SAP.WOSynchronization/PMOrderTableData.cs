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
    /// Class handles the Search, Insert, Update functionalities of PM Orders
    /// </summary>
    public class PMOrderTableData : TableData
    {
        #region Private Variables

        /// <summary>
        /// Holds the XML Node Name and Field Index Mapping dictionary for the table in question i.e.<see cref="_table"/>
        /// </summary>
        private IDictionary<string, int> _nodeFieldIndexMapping;
        /// <summary>
        /// Holds the Field Index and Field Type Mapping dictionary for the table in question i.e.<see cref="_table"/>
        /// </summary>
        private IDictionary<int, esriFieldType> _fieldIndexFieldTypeMapping;

        /// <summary>
        /// Last Message Date and Time field index in the Table
        /// </summary>
        private int _lastMessageDateFieldIndex = -1;

        /// <summary>
        /// Last Message Date and Time to be inserted into the Database
        /// </summary>
        public DateTime LastMessageDateTime { get; set; }

        #endregion Private Variables

        #region Constructor

        /// <summary>
        /// Initializes the instance of <see cref="PMOrderTableData"/> class
        /// </summary>
        /// <param name="workspace">Workspace reference</param>
        /// <param name="tableOrFeatureClassName">Table or Feature Class Name to which this object deals with. Need to provided fully qualified name.</param>
        public PMOrderTableData(IWorkspace workspace, string tableOrFeatureClassName)
            : base(workspace, tableOrFeatureClassName)
        {
        }

        #endregion Constructor

        #region Public Methods

        /// <summary>
        /// Prepare string with JobNumbers concatenated
        /// </summary>
        /// <returns>Returns the concatenated JobNumbers. This is used in whereClause of filter</returns>
        public string GetAllJobNumbers()
        {
            try
            {
                //Prepare filter to get all not null JobNumbers
                IQueryFilter filter = new QueryFilterClass();
                filter.WhereClause = JobOrderNumberFieldName + " is not null or " + JobOrderNumberFieldName + "<>''";
                filter.SubFields = JobOrderNumberFieldName;
                int jobOrderFieldIndex = _table.FindField(JobOrderNumberFieldName);
                if (jobOrderFieldIndex == -1) return null;

                //Search for features with valid JobNumbers
                ICursor cursor = _table.Search(filter, true);
                IRow pmOrderRow = null;
                StringBuilder jobNumbers = new StringBuilder();
                object fieldValue;

                //Loop through each feature and prepare string with all JobNumbers
                while ((pmOrderRow = cursor.NextRow()) != null)
                {
                    fieldValue = pmOrderRow.get_Value(jobOrderFieldIndex);
                    if (fieldValue == null || fieldValue == DBNull.Value) continue;
                    jobNumbers.Append("'" + fieldValue.ToString() + "',");
                }
                if (jobNumbers.Length > 0) jobNumbers.Remove(jobNumbers.Length - 1, 1);
                return jobNumbers.ToString();
            }
            catch (Exception ex)
            {
                _logger.Error("Error getting job numbers.",ex);
                return null;
            }
        }

        /// <summary>
        /// Deletes the records satisfying this filter from the Table
        /// </summary>
        /// <param name="whereClause">WhereClause to delete the records</param>
        public void CleanRecords(string whereClause)
        {
            try
            {
                IQueryFilter filter = new QueryFilter();
                filter.WhereClause = whereClause;
                //Delete the records satisfying the whereclause
                _table.DeleteSearchedRows(filter);
            }
            catch (Exception ex)
            {
                _logger.Error("Error deleting records.",ex);
            }
        }

        /// <summary>
        /// Updates the Table with the IRowData2
        /// </summary>
        /// <param name="row">Data to be updated in the Table</param>
        /// <remarks>
        /// Call only to update the existing records.
        /// </remarks>
        public virtual void UpdateRow(IRowData2 row)
        {

            try
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
                    //Writes the values to IRow
                    WriteFieldValues(updateRow, fieldValues);
                    updateRow.Store();
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Error updating row.", ex);
            }
        }

        /// <summary>
        /// Inserts the IRowData2 into Table
        /// </summary>
        /// <param name="row">Data to be inserted in the Table</param>
        /// <remarks>
        /// Call only to insert the new records.
        /// </remarks>
        public virtual void InsertRow(IRowData2 row)
        {
            try
            {
                if (row == null) return;
                //Get the field value collection of the PM Order
                Dictionary<string, string> fieldValues = row.FieldValues;
                if (fieldValues.Count < 1) return;
                IRow newRow = _table.CreateRow();
                //Writes the values to IRow
                WriteFieldValues(newRow, fieldValues);

                newRow.Store();
            }
            catch (Exception ex)
            {
                _logger.Error("Error inserting row.",ex);
            }
        }

        /// <summary>
        /// Searches the Table for records satisfying the filter
        /// </summary>
        /// <param name="whereClause">WhereClause to search for satisfying records</param>
        /// <returns>Returns the list of IRow satisfying the filter</returns>
        public virtual List<IRow> SearchDBTable(string whereClause)
        {
            ICursor cursor = null;
            List<IRow> resultsSet = new List<IRow>();
            try
            {
                //Prepare Query filter and query on table
                IQueryFilter filter = new QueryFilterClass();
                filter.WhereClause = whereClause;
                cursor = _table.Search(filter, false);

                IRow resultRow = null;
                while ((resultRow = cursor.NextRow()) != null)
                {
                    resultsSet.Add(resultRow);
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Error searching table.",ex);
            }
            if (cursor != null) Marshal.ReleaseComObject(cursor);
            return resultsSet;
        }

        /// <summary>
        /// Release the resources
        /// </summary>
        public override void Dispose()
        {
            _nodeFieldIndexMapping = null;
            _fieldIndexFieldTypeMapping = null;
            base.Dispose();
            //Release all the resources used in this class
        }

        #endregion Public Methods

        #region Private Methods

        /// <summary>
        /// Writes the field values to IRow from dictionary.
        /// </summary>
        /// <param name="targetRow">TargetRow to write values</param>
        /// <param name="sourceFieldValues">Dictionary of field values</param>
        private void WriteFieldValues(IRow targetRow, Dictionary<string, string> sourceFieldValues)
        {
            int fieldIndex = -1;
            object fieldValue = null;
            //Prepare field name and index mapping
            if (_nodeFieldIndexMapping == null)
            {
                PrepareNodeFieldMappingMatrix();
                if (_nodeFieldIndexMapping == null)
                {
                    _logger.Debug("XML Node Name and Field Index Mapping is empty.");
                    return;
                }
            }

            foreach (string key in sourceFieldValues.Keys)
            {
                fieldIndex = _nodeFieldIndexMapping[key];
                if (fieldIndex == -1) continue;
                //Get the ESRI Field type and convert the value to proper type before inserting
                if (_fieldIndexFieldTypeMapping.ContainsKey(fieldIndex))
                {
                    fieldValue = Convert(sourceFieldValues[key], _fieldIndexFieldTypeMapping[fieldIndex]);
                    targetRow.set_Value(fieldIndex, fieldValue);
                }
                else
                {
                    targetRow.set_Value(fieldIndex, sourceFieldValues[key]);
                }

            }

            //Last Message Date & Time
            if (_lastMessageDateFieldIndex != -1) targetRow.set_Value(_lastMessageDateFieldIndex, LastMessageDateTime);
        }

        /// <summary>
        /// Prepares the XML Node and Field Index Mapping matrix
        /// </summary>
        private void PrepareNodeFieldMappingMatrix()
        {
            try
            {
                //Get XML Node names from Schema File
                IList<string> xmlNodeNames = GetFieldNamesFromXSD();
                //add the three common fields
                xmlNodeNames.Add(ResourceConstants.FieldNames.LastMessageDate);
                xmlNodeNames.Add(ResourceConstants.FieldNames.WIPFound);
                xmlNodeNames.Add(ResourceConstants.FieldNames.WIPMissing);

                //Get the user defined field mapping
                FieldMappingSection fieldMappingSection = Config.ReadConfigFromExeLocation().GetSection("FieldMappingSection") as FieldMappingSection;

                IDictionary<string, string> nodeFieldMapping = new Dictionary<string, string>();

                foreach (FieldMap fieldMap in fieldMappingSection.FieldMapping)
                {
                    nodeFieldMapping.Add(fieldMap.Key.ToUpper(), fieldMap.Value.ToUpper());
                }

                _nodeFieldIndexMapping = new Dictionary<string, int>();
                _fieldIndexFieldTypeMapping = new Dictionary<int, esriFieldType>();
                //Retrieves the corresponding Field Index for each XML Node / Field Name 
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
                    if (fieldIndex == -1)
                    {
                        _logger.Info(string.Format("{0} field is not found in {1}", fieldName, (_table as IDataset).Name));
                    }
                    else
                    {
                        //Get the ESRI Field type
                        esriFieldType fieldType = _table.Fields.get_Field(fieldIndex).Type;
                        if (fieldType != esriFieldType.esriFieldTypeString)
                        {
                            _fieldIndexFieldTypeMapping.Add(fieldIndex, fieldType);
                        }
                    }

                }

                _lastMessageDateFieldIndex = _nodeFieldIndexMapping[ResourceConstants.FieldNames.LastMessageDate.ToUpper()];
            }
            catch (Exception ex)
            {
                _logger.Error("Error creating data mapping.",ex);
            }
        }

        /// <summary>
        /// Gets the XML Node Names ( Field Names ) from the XSD Schema file
        /// </summary>
        /// <returns>Returns the XML Node Names ( Field Names ) retrieved from the XSD Schema file</returns>
        private IList<string> GetFieldNamesFromXSD()
        {
            IList<string> fieldNames = new List<string>();

            System.IO.Stream xsdStream = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream("PGE.Interfaces.SAP.WOSynchronization.PMOrderXSD.xsd");
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

        /// <summary>
        /// Converts the filed value to specific datatype
        /// </summary>
        /// <param name="fieldValue">Field Value</param>
        /// <param name="fieldType">ESRI Field type of the target data</param>
        /// <returns>Returns the Converted the Field Value in specific type.</returns>
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

        private Type GetFieldDataType(esriFieldType fieldType)
        {
            switch (fieldType)
            {
                case esriFieldType.esriFieldTypeString:
                    return null;
                case esriFieldType.esriFieldTypeInteger:
                    return typeof(Int32);
                case esriFieldType.esriFieldTypeSmallInteger:
                    return typeof(Int16);
                case esriFieldType.esriFieldTypeDate:
                    return typeof(DateTime);
                case esriFieldType.esriFieldTypeDouble:
                    return typeof(double);
                default:
                    return null;
            }
        }

        #endregion Private Methods
    }
}
