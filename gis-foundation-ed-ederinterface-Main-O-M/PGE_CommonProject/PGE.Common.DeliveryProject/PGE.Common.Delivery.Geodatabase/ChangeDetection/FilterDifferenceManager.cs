using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Data;

using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.GeoDatabaseDistributed;

using PGE.Common.Delivery.Diagnostics;
using PGE.Common.Delivery.Framework;


namespace PGE.Common.Delivery.Geodatabase.ChangeDetection
{
    #region Enumerations

    /// <summary>
    /// Enumeration of Filter Result types.
    /// </summary>
    public enum FilterType
    {
        /// <summary>
        /// Edits will only be valid if the table they are on contains a modelname in the model name list
        /// </summary>
        IncludeModelNames,
        /// <summary>
        /// Edits will not be valid if they contain edits to tables with one of the model names
        /// </summary>
        ExcludeModelNames,
        /// <summary>
        /// Edits will only be valid if they are for the list of edited tables
        /// </summary>
        OnlyTables,
        /// <summary>
        /// Edits will not be valid if they are for the list of provided tables
        /// </summary>
        ExcludeTables,
        /// <summary>
        /// Edits will not be valid if they are for the IDatasetType Specified.
        /// http://resources.esri.com/help/9.3/ArcGISEngine/ArcObjects/esriGeoDatabase/esriDatasetType.htm
        /// </summary>
        ExcludeDatasetTypes,
        /// <summary>
        /// Edits will only be valid if they are for the IDatasetType specified.
        /// http://resources.esri.com/help/9.3/ArcGISEngine/ArcObjects/esriGeoDatabase/esriDatasetType.htm
        /// </summary>
        IncludeDatasetTypes,
        /// <summary>
        /// Edits will not be valid for the specified esriFeatureTypes(e.g esriFTAnnotation, esriFTRasterCatalogItem)
        /// http://resources.esri.com/help/9.3/ArcGISDesktop/ArcObjects/esriGeoDatabase/esriFeatureType.htm
        /// </summary>
        ExcludeFeatureTypes,
        /// <summary>
        /// Edits will not be valid for features, tests for IFeatureClass
        /// </summary>
        ExcludeAllFeatures,
        /// <summary>
        /// Edits will not be valid for the specified esriFeatureTypes(e.g esriFTAnnotation, esriFTRasterCatalogItem)
        /// http://resources.esri.com/help/9.3/ArcGISDesktop/ArcObjects/esriGeoDatabase/esriFeatureType.htm
        /// </summary>
        IncludeFeatureTypes,
        /// <summary>
        /// Edits will not be valid for anything, but features, tests for IFeatureClass
        /// </summary>
        IncludeOnlyFeatures,
        /// <summary>
        /// No filters applied: all results will be returned no filters applied
        /// </summary>
        None
    }

    #endregion

    /// <summary>
    /// Enables the use of Filters with the Difference Manager 2 base class.
    /// </summary>
    public class FilterDifferenceManager : DifferenceManager,IFilterDifferenceManager
    {
        #region Fields

        private List<esriDatasetType> _ExcludeDatasetTypes = new List<esriDatasetType>();
        private List<esriFeatureType> _ExcludeFeatureTypes = new List<esriFeatureType>();
        private List<string> _ExcludeClassModelNames = new List<string>();
        private List<string> _ExcludeFieldModelNames = new List<string>();
        private List<FilterType> _FilterList = new List<FilterType>();

        /// <summary>
        /// Dictionary to track the filter of Tables. If we put a table through the Filter, the result of that parsing is saved to avoid needless work.
        /// </summary>
        private Dictionary<string, bool> _FilterResultCache = new Dictionary<string, bool>();
        private List<esriDatasetType> _IncludeDatasetTypes = new List<esriDatasetType>();
        private List<esriFeatureType> _IncludeFeatureTypes = new List<esriFeatureType>();
        private List<string> _IncludeClassModelNames = new List<string>();
        private List<string> _IncludeFieldModelNames = new List<string>();
        private List<string> _TableNames = new List<string>();
        //private DifferenceDictionary _filteredUpdates=null;
        //Dictionary<DifferenceDictionaryWorkspace, DataSet> _updateDataset = null;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="FilterDifferenceManager"/> class.
        /// </summary>
        /// <param name="targetVersion">The target version.</param>
        /// <param name="sourceVersion">The source version.</param>        
        public FilterDifferenceManager(IVersion targetVersion, IVersion sourceVersion)
            : base(targetVersion, sourceVersion)
        {
            CommonInitializer();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FilterDifferenceManager"/> class.
        /// </summary>
        /// <param name="targetVersion">The target version.</param>
        /// <param name="sourceVersion">The source version.</param>
        /// <param name="filterList">List of Filters to apply.</param>        
        public FilterDifferenceManager(IVersion targetVersion, IVersion sourceVersion, List<FilterType> filterList)
            : this(targetVersion, sourceVersion)
        {
            if (filterList != null)
            {
                this.FilterList = filterList;
            }
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// List of ESRI.ArcGIS.Geodatabase.esriDatasetType to Exclude
        /// </summary>
        /// <value>The esriDatasetType.</value>
        public IEnumerable<ESRI.ArcGIS.Geodatabase.esriDatasetType> ExcludeDatasetTypes
        {
            get
            {
                foreach (ESRI.ArcGIS.Geodatabase.esriDatasetType datasetType in _ExcludeDatasetTypes)
                {
                    yield return datasetType;
                }
            }
            set
            {
                _ExcludeDatasetTypes.Clear();
                _ExcludeDatasetTypes.AddRange(value);
                ClearFilterCache();
            }
        }

        /// <summary>
        /// List of ESRI.ArcGIS.Geodatabase.esriFeatureType to Exclude
        /// </summary>
        /// <value>The esriFeatureType.</value>
        public IEnumerable<ESRI.ArcGIS.Geodatabase.esriFeatureType> ExcludeFeatureTypes
        {
            get
            {
                foreach (ESRI.ArcGIS.Geodatabase.esriFeatureType featureType in _ExcludeFeatureTypes)
                {
                    yield return featureType;
                }
            }
            set
            {
                _ExcludeFeatureTypes.Clear();
                _ExcludeFeatureTypes.AddRange(value);
                ClearFilterCache();
            }
        }

        /// <summary>
        /// List of ArcFM Class Model Names to Exclude
        /// </summary>
        /// <value>The class model names.</value>
        public IEnumerable<string> ExcludeClassModelNames
        {
            get
            {
                foreach (string modelName in _ExcludeClassModelNames)
                {
                    yield return modelName;
                }
            }
            set
            {
                _ExcludeClassModelNames.Clear();
                _ExcludeClassModelNames.AddRange(value);
                ClearFilterCache();
            }
        }

        /// <summary>
        /// List of ArcFM Field Model Names to Exclude
        /// </summary>
        /// <value>The class model names.</value>
        public IEnumerable<string> ExcludeFieldModelNames
        {
            get
            {
                foreach (string modelName in _ExcludeFieldModelNames)
                {
                    yield return modelName;
                }
            }
            set
            {
                _ExcludeFieldModelNames.Clear();
                _ExcludeFieldModelNames.AddRange(value);
                ClearFilterCache();
            }
        }

        /// <summary>
        /// Gets the type of the filter.
        /// </summary>
        /// <value>The type of the filter.</value>
        public IEnumerable<FilterType> FilterList
        {
            get
            {
                foreach (FilterType filterType in _FilterList)
                {
                    yield return filterType;
                }
            }
            set
            {
                _FilterList.Clear();
                _FilterList.AddRange(value);
                ClearFilterCache();
            }
        }

        /// <summary>
        /// List of ESRI.ArcGIS.Geodatabase.esriDatasetType to Include
        /// </summary>
        /// <value>The esriDatasetType.</value>
        public IEnumerable<ESRI.ArcGIS.Geodatabase.esriDatasetType> IncludeDatasetTypes
        {
            get
            {
                foreach (ESRI.ArcGIS.Geodatabase.esriDatasetType datasetType in _IncludeDatasetTypes)
                {
                    yield return datasetType;
                }
            }
            set
            {
                _IncludeDatasetTypes.Clear();
                _IncludeDatasetTypes.AddRange(value);
                ClearFilterCache();
            }
        }

        /// <summary>
        /// List of ESRI.ArcGIS.Geodatabase.esriFeatureType to Include
        /// </summary>
        /// <value>The esriFeatureType.</value>
        public IEnumerable<ESRI.ArcGIS.Geodatabase.esriFeatureType> IncludeFeatureTypes
        {
            get
            {
                foreach (ESRI.ArcGIS.Geodatabase.esriFeatureType featureType in _IncludeFeatureTypes)
                {
                    yield return featureType;
                }
            }
            set
            {
                _IncludeFeatureTypes.Clear();
                _IncludeFeatureTypes.AddRange(value);
                ClearFilterCache();
            }
        }

        /// <summary>
        /// List of ArcFM Class Model Names to Include
        /// </summary>
        /// <value>The class model names.</value>               
        public IEnumerable<string> IncludeClassModelNames
        {
            get
            {
                foreach (string modelName in _IncludeClassModelNames)
                {
                    yield return modelName;
                }
            }
            set
            {
                _IncludeClassModelNames.Clear();
                _IncludeClassModelNames.AddRange(value);
                ClearFilterCache();
            }
        }

        /// <summary>
        /// List of ArcFM Field Model Names to Include
        /// </summary>
        /// <value>The class model names.</value>               
        public IEnumerable<string> IncludeFieldModelNames
        {
            get
            {
                foreach (string modelName in _IncludeFieldModelNames)
                {
                    yield return modelName;
                }
            }
            set
            {
                _IncludeFieldModelNames.Clear();
                _IncludeFieldModelNames.AddRange(value);
                ClearFilterCache();
            }
        }
        /// <summary>
        /// Gets the list of Table Names.
        /// </summary>
        /// <value>The Table names.</value>
        public IEnumerable<string> TableNames
        {
            get
            {
                foreach (string tableName in _TableNames)
                {
                    yield return tableName;
                }
            }
            set
            {
                _TableNames.Clear();
                _TableNames.AddRange(value);
                ClearFilterCache();
            }
        }

        /// <summary>
        ///  Dictionary of the Tables and if they were filtered or not. Can be used to check and see if a specific table was skipped to drive further logic.
        /// </summary>
        public Dictionary<string, bool> WasFilteredResults
        {
            get
            {
                return _FilterResultCache;
            }
        }

        //public DifferenceDictionary FilteredUpdates
        //{
        //    get
        //    {
        //        return _filteredUpdates;
        //    }
        //}

        #endregion

        #region Public Methods
        /// <summary>
        /// 
        /// </summary>
        /// <param name="className"></param>
        /// <param name="filterFields"></param>
        /// <returns></returns>
        public IEnumerable<IRow> GetUpdatedRows(string className, params string[] filterFields)
        {
            IEnumerable<IRow> enumChangedRows = base.GetChangedRows(className, esriDataChangeType.esriDataChangeTypeUpdate);

            ITable targetTable = null;
            IFeatureWorkspace targetFeatWorkspace = (IFeatureWorkspace)TargetVersion;
            targetTable = targetFeatWorkspace.OpenTable(className);
            if (targetTable == null)
            {
                string message = string.Format("Table or FeatureClass {0} was not opened correctly", className);
                throw new ApplicationException(message);
            }

            IRow targetRow = null;
            foreach (IRow changedRow in enumChangedRows)
            {
                targetRow = targetTable.GetRow(changedRow.OID);

                if (WithEffectiveChanges(changedRow, targetRow, null))
                {
                    yield return changedRow;
                }
            }
        }

        /// <summary>
        /// Clears the List of Tables and if they should be filtered. This method should be called any time the filters are changed.
        /// </summary>
        public void ClearFilterCache()
        {
            if (_FilterResultCache != null)
            {
                _FilterResultCache.Clear();
            }
        }

        //public DataSet FilteredUpdatesDataSet(DifferenceDictionaryWorkspace workspaceType)
        //{
        //    if (workspaceType == DifferenceDictionaryWorkspace.Auto) workspaceType = DifferenceDictionaryWorkspace.Primary;
        //    if (_updateDataset != null && _updateDataset.ContainsKey(workspaceType))
        //    {
        //        return _updateDataset[workspaceType];
        //    }
        //    else
        //    {
        //        if (_updateDataset == null) _updateDataset = new Dictionary<DifferenceDictionaryWorkspace, DataSet>();
        //        FilterUpdateDataset();
        //        return _updateDataset[workspaceType];
        //    }
        //}
        #endregion

        #region Protected Methods

        /// <summary>
        /// Checks to see if the related table needs to be filtered out.
        /// </summary>
        /// <param name="table">the object to check</param>
        /// <returns><c>false</c> means no filtering needed. <c>true</c> means filter out this table.</returns>
        protected override bool CheckRelatedTable(ITable table)
        {
            return this.CheckIfFilterTable(table);
        }

        /// <summary>        
        /// Gets the difference for the specified versioned table.        
        /// </summary>
        /// <param name="dci">IDataChangesInfo for the versions to get the differences from</param>
        /// <param name="tableName">Name of the table</param>
        /// <param name="table">The ITable object</param>
        /// <param name="diffTypes">The types of changes to get</param>
        protected override void GetDifferences(IDataChangesInfo dci, string tableName, ITable table, params esriDataChangeType[] diffTypes)
        {
            if (!this.CheckIfFilterTable(table))
            {
                base.GetDifferences(dci, tableName, table, diffTypes);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="table"></param>
        /// <returns></returns>
        protected override IEnumerable<int> FieldIndicesToPopulate(ITable table)
        {
            IEnumerable<int> fieldIndices = new List<int>();
            //Get the FieldIndices of IncludeFieldModelNames
            Dictionary<string, List<int>> fieldIndecesWithModelNames = ModelNameFacade.FieldIndicesFromModelNames(table as IObjectClass, IncludeFieldModelNames.ToList());
            foreach (List<int> fieldIndex in fieldIndecesWithModelNames.Values)
            {
                fieldIndices=fieldIndices.Union(fieldIndex); 
            }
            //Get FieldIndices of ExcludeFieldModelName and remove them
            fieldIndecesWithModelNames = ModelNameFacade.FieldIndicesFromModelNames(table as IObjectClass, IncludeFieldModelNames.ToList());
            foreach (List<int> fieldIndex in fieldIndecesWithModelNames.Values)
            {
                fieldIndices=fieldIndices.Except(fieldIndex);
            }
            //Check if OID Field and GUID fields are missing and add them as the first two columns
            int oidFieldIndex=table.FindField(table.OIDFieldName);
            if (!fieldIndices.Contains(oidFieldIndex)) fieldIndices=fieldIndices.Concat(new int[] { oidFieldIndex }); 
            IClassEx classEx=(IClassEx)table;
            if(classEx.HasGlobalID)
            {
                int guidFieldIndex = table.FindField(classEx.GlobalIDFieldName);
                if (!fieldIndices.Contains(guidFieldIndex)) fieldIndices = fieldIndices.Concat(new int[] { guidFieldIndex });
            }
           return fieldIndices;
        }
        #endregion

        #region Private Methods
        private bool WithEffectiveChanges(IRow sourceRow, IRow targetRow, List<int> filterIndices)
        {
            bool effectivelyChanged = false;

            if (filterIndices.Count == 0)
            {
                effectivelyChanged = true;
            }
            else
            {
                object sourceVal = null;
                object targetVal = null;
                foreach (int fieldIndex in filterIndices)
                {
                    sourceVal = sourceRow.get_Value(fieldIndex);
                    targetVal = targetRow.get_Value(fieldIndex);
                    if (object.Equals(sourceVal, targetVal) == false)
                    {
                        effectivelyChanged = true;
                        break;
                    }
                }
            }

            return effectivelyChanged;
        }

        /// <summary>
        /// Table is compared to current set filters, returns true if filtered.
        /// </summary>
        /// <param name="tableToCheck">Table to check for filtering</param>
        /// <returns><c>true</c> means the table needs to be filtered <c>false</c> is default</returns>
        private bool CheckIfFilterTable(ITable tableToCheck)
        {
            bool outputTable = false;
            //string[] modelList;
            if (tableToCheck != null)
            {
                if (FilterList != null)
                {
                    string tableName = (tableToCheck as IDataset).Name;
                    if (_FilterResultCache.ContainsKey(tableName))
                    {
                        outputTable = _FilterResultCache[tableName];
                    }
                    else
                    {
                        foreach (FilterType filter in FilterList)
                        {
                            switch (filter)
                            {
                                case FilterType.None:
                                    outputTable = false;
                                    break;
                                case FilterType.ExcludeDatasetTypes:
                                    if (ExcludeDatasetTypes != null)
                                    {
                                        esriDatasetType testOut = (tableToCheck as IDataset).Type;
                                        if (ExcludeDatasetTypes.Contains(testOut))
                                        {
                                            outputTable = true;
                                        }
                                    }
                                    break;
                                case FilterType.IncludeDatasetTypes:
                                    if (IncludeDatasetTypes != null)
                                    {
                                        esriDatasetType testOut = (tableToCheck as IDataset).Type;
                                        if (!IncludeDatasetTypes.Contains(testOut))
                                        {
                                            outputTable = true;
                                        }
                                    }
                                    else
                                    {
                                        // set to only, but list is empty, so eveything should be filtered.
                                        outputTable = true;
                                    }
                                    break;
                                case FilterType.ExcludeFeatureTypes:
                                    if (ExcludeFeatureTypes != null && (tableToCheck is IFeatureClass))
                                    {
                                        esriFeatureType testOut = (tableToCheck as IFeatureClass).FeatureType;
                                        if (ExcludeFeatureTypes.Contains(testOut))
                                        {
                                            outputTable = true;
                                        }
                                    }
                                    break;
                                case FilterType.ExcludeAllFeatures:
                                    if (tableToCheck is IFeatureClass)
                                    {
                                        outputTable = true;
                                    }
                                    break;
                                case FilterType.IncludeOnlyFeatures:
                                    if (!(tableToCheck is IFeatureClass))
                                    {
                                        outputTable = true;
                                    }
                                    break;
                                case FilterType.IncludeFeatureTypes:
                                    if (IncludeFeatureTypes != null)
                                    {
                                        if (tableToCheck is IFeatureClass)
                                        {
                                            esriFeatureType testOut = (tableToCheck as IFeatureClass).FeatureType;
                                            if (!IncludeFeatureTypes.Contains(testOut))
                                            {
                                                outputTable = true;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        // set to only, but list is empty or is not a feature class, so eveything should be filtered.
                                        outputTable = true;
                                    }
                                    break;
                                case FilterType.ExcludeModelNames:
                                    if (ExcludeClassModelNames != null && ExcludeFieldModelNames!=null)
                                    {
                                        string[] modelList = new string[_ExcludeClassModelNames.Count];
                                        _ExcludeClassModelNames.CopyTo(modelList);
                                        if (ModelNameFacade.ContainsClassModelName(tableToCheck as IObjectClass, modelList) ||
                                           ModelNameFacade.ContainsFieldModelName(tableToCheck as IObjectClass, ExcludeFieldModelNames.ToArray()))
                                        {
                                            outputTable = true;
                                        }
                                    }
                                    break;
                                case FilterType.ExcludeTables:
                                    if (TableNames != null)
                                    {
                                        //http://msdn.microsoft.com/en-us/library/system.stringcomparer.aspx
                                        if (TableNames.Contains(tableName, StringComparer.InvariantCultureIgnoreCase))
                                        {
                                            outputTable = true;
                                        }
                                    }
                                    break;
                                case FilterType.IncludeModelNames:
                                    if (IncludeClassModelNames != null && IncludeClassModelNames != null)
                                    {
                                        string[] modelList = new string[_IncludeClassModelNames.Count];
                                        _IncludeClassModelNames.CopyTo(modelList);
                                        if ((!ModelNameFacade.ContainsClassModelName(tableToCheck as IObjectClass, modelList)) &&
                                           (!ModelNameFacade.ContainsFieldModelName(tableToCheck as IObjectClass, IncludeFieldModelNames.ToArray())))
                                        {
                                            outputTable = true;
                                        }
                                    }
                                    break;
                                case FilterType.OnlyTables:
                                    if (TableNames != null)
                                    {
                                        //http://msdn.microsoft.com/en-us/library/system.stringcomparer.aspx
                                        if (!TableNames.Contains(tableName, StringComparer.InvariantCultureIgnoreCase))
                                        {
                                            outputTable = true;
                                        }
                                    }
                                    else
                                    {
                                        outputTable = true;
                                    }
                                    break;
                                default:
                                    return false;
                            }
                        }
                        _FilterResultCache.Add(tableName, outputTable);
                    }
                }
            }
            else
            {
                throw new Exception("ITable provided was null, CheckIfFilterTable is not a valid operation");
            }
            return outputTable;
        }

        private void CommonInitializer()
        {
            if (_FilterResultCache == null)
            {
                _FilterResultCache = new Dictionary<string, bool>();
            }
            if (_FilterList == null)
            {
                _FilterList = new List<FilterType>();
            }
            if (_TableNames == null)
            {
                _TableNames = new List<string>();
            }
            if (_ExcludeClassModelNames == null)
            {
                _ExcludeClassModelNames = new List<string>();
            }
            if (_IncludeClassModelNames == null)
            {
                _IncludeClassModelNames = new List<string>();
            }
            if (_IncludeFeatureTypes == null)
            {
                _IncludeFeatureTypes = new List<ESRI.ArcGIS.Geodatabase.esriFeatureType>();
            }
            if (_ExcludeFeatureTypes == null)
            {
                _ExcludeFeatureTypes = new List<ESRI.ArcGIS.Geodatabase.esriFeatureType>();
            }
            if (_IncludeDatasetTypes == null)
            {
                _IncludeDatasetTypes = new List<ESRI.ArcGIS.Geodatabase.esriDatasetType>();
            }
            if (_ExcludeDatasetTypes == null)
            {
                _ExcludeDatasetTypes = new List<ESRI.ArcGIS.Geodatabase.esriDatasetType>();
            }
        }

        //private void FilterUpdateDataset()
        //{
        //    _updateDataset = new Dictionary<DifferenceDictionaryWorkspace, DataSet>();
        //    _filteredUpdates = new DifferenceDictionary(Updates);
        //    DataSet sourceDS = UpdatesDataSet(DifferenceDictionaryWorkspace.Primary).Copy();
        //    DataSet targetDS = UpdatesDataSet(DifferenceDictionaryWorkspace.Secondary).Copy();
        //    List<DataRow> rowsToDelete = new List<DataRow>();
        //    foreach (DataTable sourceDT in sourceDS.Tables)
        //    {
        //        DataTable targetDT = targetDS.Tables[sourceDT.TableName];
        //        foreach (DataRow dr in sourceDT.Rows)
        //        {
        //            int oidfieldVal = int.Parse((dr[sourceDT.PrimaryKey[0]]).ToString());
        //            DataRow[] targetDR = targetDT.Select(targetDT.PrimaryKey[0].ColumnName + "=" + oidfieldVal);
        //            if (AreEqual(dr, targetDR[0], sourceDT.Columns))
        //            {
        //                //Remove from Target DataTable
        //                targetDT.Rows.Remove(targetDR[0]);
        //                _filteredUpdates[sourceDT.TableName].Remove(oidfieldVal);
        //            }
        //        }
        //        foreach (DataRow dr in rowsToDelete) sourceDT.Rows.Remove(dr);
        //    }
        //    sourceDS.AcceptChanges();
        //    targetDS.AcceptChanges();
        //    _updateDataset.Add(DifferenceDictionaryWorkspace.Primary, sourceDS);
        //    _updateDataset.Add(DifferenceDictionaryWorkspace.Secondary, targetDS);
        //}

        private bool AreEqual(DataRow sourceRow, DataRow targetRow, DataColumnCollection columns)
        {
            foreach (DataColumn dc in columns)
            {
                if (!string.Equals(sourceRow[dc.ColumnName], targetRow[dc.ColumnName]))
                    return false;
            }
            return true;
        }

        #endregion
    }
}