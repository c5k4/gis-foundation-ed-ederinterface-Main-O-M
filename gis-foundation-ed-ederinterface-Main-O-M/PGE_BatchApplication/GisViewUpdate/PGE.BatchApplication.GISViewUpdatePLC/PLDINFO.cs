using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Runtime.InteropServices;
using System.Xml;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.DataSourcesGDB;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using log4net;
using System.Reflection;

namespace PGE.BatchApplication.GISViewUpdatePLC
{
    

        public class PLDINFO : TableData
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

            private static readonly Log4NetLogger _logger = new Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "PLDInfo.log4net.config");

            #endregion Private Variables

            #region Constructor

            /// <summary>
            /// Initializes the instance of <see cref="PMOrderTableData"/> class
            /// </summary>
            /// <param name="workspace">Workspace reference</param>
            /// <param name="tableOrFeatureClassName">Table or Feature Class Name to which this object deals with. Need to provided fully qualified name.</param>
            public PLDINFO(IWorkspace workspace, string tableOrFeatureClassName)
                : base(workspace, tableOrFeatureClassName)
            {
            }

            #endregion Constructor

            #region Public Methods

          

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
                   // _table.DeleteField
                  //  _table.DeleteSearchedRows(filter);
                }
                catch (Exception ex)
                {
                    _logger.Error("Error deleting records.", ex);
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

                    //Get the PLDBID
                    string pldbID = fieldValues[ResourceConstants.XMLNodeNames.PLDBID];
                    //Get the Row with same jobOrder number
                    List<IFeature> resultSet = SearchDBTable(ResourceConstants.FieldNames.PLDBID + "='" + pldbID + "'");
                    for (int i = 0; i < resultSet.Count; i++)
                    {
                       
                        IFeature updateRow = resultSet[i];
                        //Writes the values to IRow
                        _logger.Info("Update Existing Record for PLDBID=:" + pldbID);
                        WriteFieldValues(updateRow, fieldValues);
                        updateRow.Store();
                        _logger.Info("Record Updated");
                    }
                }
                catch (Exception ex)
                {
                   // _logger.Info("Error Occured" + ex.Message);
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
                    IFeature newRow = _table.CreateFeature();
                    //Writes the values to IRow
                    _logger.Info("Inserted New Record");
                    WriteFieldValues(newRow, fieldValues);

                    newRow.Store();
                }
                catch (Exception ex)
                {
                    _logger.Error("Error inserting row.", ex);
                }
            }

            /// <summary>
            /// Searches the Table for records satisfying the filter
            /// </summary>
            /// <param name="whereClause">WhereClause to search for satisfying records</param>
            /// <returns>Returns the list of IRow satisfying the filter</returns>
            public virtual List<IFeature> SearchDBTable(string whereClause)
            {
                IFeatureCursor cursor = null;
                List<IFeature> resultsSet = new List<IFeature>();
                try
                {
                    //Prepare Query filter and query on table
                    IQueryFilter filter = new QueryFilterClass();
                    filter.WhereClause = whereClause;
                    cursor = _table.Search(filter, false);

                    IFeature resultRow = null;
                    while ((resultRow = cursor.NextFeature()) != null)
                    {
                        
                        resultsSet.Add(resultRow);
                    }
                }
                catch (Exception ex)
                {
                    _logger.Error("Error searching table.", ex);
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
            private void WriteFieldValues(IFeature targetRow, Dictionary<string, string> sourceFieldValues)
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

                try
                {
                    bool flag = false;
                    foreach (string key in sourceFieldValues.Keys)
                    {
                        _logger.Info("Key="  + key + ":= "+ sourceFieldValues[key]);
                        if (key==System.Configuration.ConfigurationManager.AppSettings["PGE_AltitudeChange"] ||   key == System.Configuration.ConfigurationManager.AppSettings["PGE_PossibleFAALookup"] || key == System.Configuration.ConfigurationManager.AppSettings["PercentRemainingStrength"] || key == System.Configuration.ConfigurationManager.AppSettings["PGE_PTTInspectionDate"] || key == System.Configuration.ConfigurationManager.AppSettings["AnalysisID"] || key == System.Configuration.ConfigurationManager.AppSettings["PGE_OBJECTID"])
                            continue;
                        fieldIndex = _nodeFieldIndexMapping[key.ToUpper()];
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
                       
                            if (System.Convert.ToDouble(sourceFieldValues["Latitude"]) != System.Convert.ToDouble(!(targetRow.get_Value(targetRow.Fields.FindField("LAT")) is DBNull)) || System.Convert.ToDouble(sourceFieldValues["Longitude"]) != System.Convert.ToDouble(!(targetRow.get_Value(targetRow.Fields.FindField("LONGITUDE"))is DBNull)))
                            {
                                if(System.Convert.ToDouble(sourceFieldValues["Latitude"])!=0.0 || System.Convert.ToDouble(sourceFieldValues["Longitude"]) !=0.0)
                                 flag = true;
                            }
                        

                       
                       
                    }

                    if (flag)
                        {
                            
                          
                            //Create Spatial Reference Factory  
                            ISpatialReferenceFactory srFactory = new SpatialReferenceEnvironmentClass();
                            ISpatialReference sr1;             //GCS to project from           
                            IGeographicCoordinateSystem gcs = srFactory.CreateGeographicCoordinateSystem(4326);
                            sr1 = gcs;             //Projected Coordinate System to project into   
                            IProjectedCoordinateSystem pcs = srFactory.CreateProjectedCoordinateSystem(102100);
                            ISpatialReference sr2;
                            sr2 = pcs;             //Point to project     
                            IPoint point = new PointClass() as IPoint;
                            
                            point.PutCoords(System.Convert.ToDouble(sourceFieldValues["Longitude"]), System.Convert.ToDouble(sourceFieldValues["Latitude"]));             //Geometry Interface to do actual project    
                            IGeometry geometry;
                            geometry = point;
                            geometry.SpatialReference = sr1;
                            geometry.Project(sr2);
                            point = geometry as IPoint;
                            double x; double y;
                            point.QueryCoords(out x, out y);

                            targetRow.set_Value(targetRow.Fields.FindField("Shape"), point);
                            flag = false;
                        }

                    
                }
                catch (Exception ex)
                {
                    _logger.Info("Key" + ex.Message);
                    _logger.Error("Error writing WriteFieldValues.", ex);
                }

                //Last Message Date & Time
              //  if (_lastMessageDateFieldIndex != -1) targetRow.set_Value(_lastMessageDateFieldIndex, LastMessageDateTime);
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

                  //  _lastMessageDateFieldIndex = _nodeFieldIndexMapping[ResourceConstants.FieldNames.LastMessageDate.ToUpper()];
                }
                catch (Exception ex)
                {
                    _logger.Error("Error creating data mapping.", ex);
                }
            }

            /// <summary>
            /// Gets the XML Node Names ( Field Names ) from the XSD Schema file
            /// </summary>
            /// <returns>Returns the XML Node Names ( Field Names ) retrieved from the XSD Schema file</returns>
            private IList<string> GetFieldNamesFromXSD()
            {
                IList<string> fieldNames = new List<string>();

                System.IO.Stream xsdStream = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream("PGE.BatchApplication.GISViewUpdatePLC.PLDInfoXSD.xsd");
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
