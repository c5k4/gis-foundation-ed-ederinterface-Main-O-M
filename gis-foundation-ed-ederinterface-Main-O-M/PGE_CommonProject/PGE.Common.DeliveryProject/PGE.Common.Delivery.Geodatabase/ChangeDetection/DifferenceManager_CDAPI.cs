using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Data;

using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.GeoDatabaseDistributed;
using ESRI.ArcGIS.esriSystem;

using PGE.Common.Delivery.Diagnostics;
using PGE.Common.Delivery.Systems;
using System.Text;
using PGE.Common.Delivery.Framework.FeederManager;
using PGE.Common.ChangeDetectionAPI;

namespace PGE.Common.Delivery.Geodatabase.ChangeDetection
{
    /// <summary>
    /// V3SF - CD API (EDGISREARC-1452) - Added
    /// A supporting class that will load all of the differences between the source and target versions.
    /// </summary>
    public class DifferenceManager_CDAPI : IDifferenceManager
    {
        DataSet _deleteDataset = null;
        Dictionary<DifferenceDictionaryWorkspace, DataSet> _updateDataset = null;
        DataSet _insertDataset = null;
        /// <summary>
        /// 
        /// </summary>
        protected IVersionDataChangesInit _versionDataChangesInit = null;
        private List<string> _modifiedClassNames;
        public IDictionary<string, ChangedFeatures> _cdVersionDifference;

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="DifferenceManager"/> class.
        /// </summary>
        /// <param name="targetVersion">The target version.</param>
        /// <param name="sourceVersion">The source version.</param>
        public DifferenceManager_CDAPI(IVersion targetVersion, IVersion sourceVersion, IDictionary<string, ChangedFeatures> cdVersionDifference)
        {
            this.TargetVersion = targetVersion;
            this.SourceVersion = sourceVersion;
            this.CommonAncestorVersion = ((IVersion2)SourceVersion).GetCommonAncestor(TargetVersion);
            _cdVersionDifference = cdVersionDifference;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the common ancestor version.
        /// </summary>
        public IVersion CommonAncestorVersion
        {
            get;
            private set;
        }

        /// <summary>
        /// Dictionary mapping a list of features that have been deleted in the source version to the corresponding
        /// table name.
        /// </summary>
        public DifferenceDictionary Deletes
        {
            get;
            private set;
        }

        /// <summary>
        /// Dictionary mapping a list of features that have been inserted in the source version to the corresponding
        /// table name.
        /// </summary>
        public DifferenceDictionary Inserts
        {
            get;
            private set;
        }

        /// <summary>
        /// The source version used for the difference evaluation.
        /// </summary>
        public IVersion SourceVersion
        {
            get;
            private set;
        }

        /// <summary>
        /// The target version used for the difference evaluation.
        /// </summary>
        public IVersion TargetVersion
        {
            get;
            private set;
        }

        /// <summary>
        /// Dictionary mapping a list of features that have been updated in the source version to the corresponding
        /// table name.
        /// </summary>
        public DifferenceDictionary Updates
        {
            get;
            private set;
        }

        #endregion

        #region Public Methods
        /// <summary>
        /// 
        /// </summary>
        public void Initialize()
        {
            if (_versionDataChangesInit == null)
            {
                IDataset sourceDataset = (IDataset)SourceVersion;
                IName sourceVersionName = sourceDataset.FullName;
                IWorkspaceName2 sourceWorkspaceName = (IWorkspaceName2)sourceVersionName;

                IDataset targetDataset = (IDataset)TargetVersion;
                IName targetVersionName = targetDataset.FullName;
                IWorkspaceName2 targetWorkspaceName = (IWorkspaceName2)targetVersionName;

                _versionDataChangesInit = new VersionDataChangesClass();
                _versionDataChangesInit.Init(sourceWorkspaceName, targetWorkspaceName);
            }
        }

        public List<int> GetChangedRowOIDs(string tableName, params esriDifferenceType[] esriDifference)
        {
            List<int> resut = new List<int>();

            foreach (esriDifferenceType differenceType in esriDifference)
            {
                if (differenceType == esriDifferenceType.esriDifferenceTypeInsert)
                {
                    for (int i = 0; i < _cdVersionDifference[tableName.ToUpper()].Action.Insert.Count; i++)
                    {
                        if(_cdVersionDifference[tableName.ToUpper()].Action.Insert[i].feature != null)
                        resut.Add(Convert.ToInt32(_cdVersionDifference[tableName.ToUpper()].Action.Insert[i].OID));
                    }
                }
                if (differenceType == esriDifferenceType.esriDifferenceTypeUpdateNoChange)
                {
                    for (int i = 0; i < _cdVersionDifference[tableName.ToUpper()].Action.Update.Count; i++)
                    {
                        if (_cdVersionDifference[tableName.ToUpper()].Action.Update[i].feature != null)
                            resut.Add(Convert.ToInt32(_cdVersionDifference[tableName.ToUpper()].Action.Update[i].OID));
                    }
                }
                if (differenceType == esriDifferenceType.esriDifferenceTypeDeleteNoChange)
                {
                    for (int i = 0; i < _cdVersionDifference[tableName.ToUpper()].Action.Delete.Count; i++)
                    {
                        resut.Add(Convert.ToInt32(_cdVersionDifference[tableName.ToUpper()].Action.Delete[i].OID));
                    }
                }
            }

            return resut;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="className"></param>
        /// <returns></returns>
        public bool HasChanges(string className)
        {
            bool changed = false;

            if (ModifiedClassNames.Contains(className.ToUpper()) == true)
            {
                changed = true;
            }

            return changed;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="className"></param>
        /// <param name="dataChangeType"></param>
        /// <returns></returns>
        public bool HasChanges(string className, esriDataChangeType dataChangeType)
        {
            bool changed = false;

            if (dataChangeType == esriDataChangeType.esriDataChangeTypeInsert)
            {
                if (_cdVersionDifference[className.ToUpper()].Action.Insert.Count > 0)
                    changed = true;
            }
            else if (dataChangeType == esriDataChangeType.esriDataChangeTypeUpdate)
            {
                if (_cdVersionDifference[className.ToUpper()].Action.Update.Count > 0)
                    changed = true;
            }
            else if (dataChangeType == esriDataChangeType.esriDataChangeTypeDelete)
            {
                if (_cdVersionDifference[className.ToUpper()].Action.Delete.Count > 0)
                    changed = true;
            }

            return changed;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sourceTable"></param>
        /// <param name="targetTable"></param>
        /// <param name="subtypes"></param>
        /// <returns></returns>
        public IEnumerable<IRow> GetInserts(ITable sourceTable, ITable targetTable, List<string> subtypes)
        {
            return GetChangedRows(false, sourceTable, targetTable, subtypes, esriDifferenceType.esriDifferenceTypeInsert);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sourceTable"></param>
        /// <param name="targetTable"></param>
        /// <param name="subtypes"></param>
        /// <returns></returns>
        public IEnumerable<IRow> GetDeletes(ITable sourceTable, ITable targetTable, List<string> subtypes)
        {
            return GetChangedRows(false, sourceTable, targetTable, subtypes, esriDifferenceType.esriDifferenceTypeDeleteNoChange);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sourceTable"></param>
        /// <param name="targetTable"></param>
        /// <param name="subtypes"></param>
        /// <returns></returns>
        public IEnumerable<IRow> GetUpdates(bool GetFeederManager20Updates, ITable sourceTable, ITable targetTable, List<string> subtypes)
        {
            return GetChangedRows(GetFeederManager20Updates, sourceTable, targetTable, subtypes, esriDifferenceType.esriDifferenceTypeUpdateNoChange);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sourceTable"></param>
        /// <param name="targetTable"></param>
        /// <param name="subtypes"></param>
        /// <param name="differenceTypes"></param>
        /// <returns></returns>
        public IEnumerable<IRow> GetChangedRows(bool GetFeederManager20Updates, ITable sourceTable, ITable targetTable, List<string> subtypes, params esriDifferenceType[] differenceTypes)
        {
            string where = string.Empty;
            if (subtypes.Count > 0)
            {
                foreach (string subtype in subtypes)
                {
                    where = where + "SubtypeCD = " + subtype + " OR ";
                }

                // remove the ending "spaceORspace"
                if (where.Length > 4)
                {
                    where = where.Substring(0, where.Length - 4);
                }
            }

            IEnumerable<IRow> changedRows = GetChangedRows(GetFeederManager20Updates, sourceTable, targetTable, where, differenceTypes);

            foreach (IRow changedRow in changedRows)
            {
                yield return changedRow;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sourceTable"></param>
        /// <param name="targetTable"></param>
        /// <param name="whereClause"></param>
        /// <param name="differenceTypes"></param>
        /// <returns></returns>
        public IEnumerable<IRow> GetChangedRows(bool GetFeederManager20Updates, ITable sourceTable, ITable targetTable, string whereClause, params esriDifferenceType[] differenceTypes)
        {
            IQueryFilter2 queryFilter = null;
            if (string.IsNullOrEmpty(whereClause) == false)
            {
                queryFilter = new QueryFilterClass();
                queryFilter.WhereClause = whereClause;
            }

            IVersionedTable sourceVersionedTable = (IVersionedTable)sourceTable;

            foreach (esriDifferenceType diffType in differenceTypes)
            {
                IQueryFilter qf = new QueryFilterClass();
                qf.AddField(sourceTable.OIDFieldName);
                IDifferenceCursor diffCur = sourceVersionedTable.Differences(targetTable, diffType, qf);
                List<int> oidsToObtain = new List<int>();
                try
                {
                    int oID;
                    IRow aRow;

                    diffCur.Next(out oID, out aRow);
                    while (oID != -1)
                    {
                        if (diffType == esriDifferenceType.esriDifferenceTypeDeleteNoChange
                            || diffType == esriDifferenceType.esriDifferenceTypeDeleteUpdate)
                        {
                            //aRow = targetTable.GetRow(oID);
                            oidsToObtain.Add(oID);
                        }
                        else
                        {
                            //aRow = sourceTable.GetRow(oID);
                            oidsToObtain.Add(oID);
                        }

                        //yield return aRow;
                        if (aRow != null) { while (Marshal.ReleaseComObject(aRow) > 0) { } }
                        diffCur.Next(out oID, out aRow);

                    }
                }
                finally
                {
                    while (Marshal.ReleaseComObject(diffCur) > 0) { }
                }

                //The following section will return any rows that feeder manager has identified as being "updated".  This supports
                //any functionality that needs to know when feeder manager specific information has changed at feeder manager 2.0
                if (GetFeederManager20Updates && diffType == esriDifferenceType.esriDifferenceTypeUpdateNoChange)
                {
                    IDataset ds = sourceTable as IDataset;
                    List<int> oids = FeederManager2.GetEditedFeatures((IFeatureWorkspace)ds.Workspace, ((IVersion)ds.Workspace).VersionName, (IObjectClass)sourceTable);
                    oidsToObtain.AddRange(oids);
                }

                oidsToObtain = oidsToObtain.Distinct().ToList();

                StringBuilder whereString = new StringBuilder();
                List<string> whereInClauses = new List<string>();
                for (int i = 0; i < oidsToObtain.Count; i++)
                {
                    if ((i != 0 && (i % 999) == 0) || oidsToObtain.Count == 1 || i == oidsToObtain.Count - 1)
                    {
                        whereString.Append(oidsToObtain[i]);
                        whereInClauses.Add(whereString.ToString());
                        whereString = new StringBuilder();
                    }
                    else
                    {
                        whereString.Append(oidsToObtain[i] + ",");
                    }
                }

                ITable tableToQuery = sourceTable;
                if (diffType == esriDifferenceType.esriDifferenceTypeDeleteNoChange
                            || diffType == esriDifferenceType.esriDifferenceTypeDeleteUpdate)
                {
                    tableToQuery = targetTable;
                }

                List<IRow> rowsToReturn = new List<IRow>();
                if (queryFilter == null) { queryFilter = new QueryFilterClass(); }
                string whereFilter = queryFilter.WhereClause;
                int oidFieldIndex = tableToQuery.FindField(tableToQuery.OIDFieldName);
                foreach (string where in whereInClauses)
                {
                    if (string.IsNullOrEmpty(whereFilter)) { queryFilter.WhereClause = tableToQuery.OIDFieldName + " in (" + where + ")"; }
                    else { queryFilter.WhereClause = "(" + whereFilter + ") AND (" + tableToQuery.OIDFieldName + " in (" + where + "))"; }

                    ICursor cursor = tableToQuery.Search(queryFilter, false);
                    IRow row = null;
                    while ((row = cursor.NextRow()) != null)
                    {
                        rowsToReturn.Add(row);
                    }
                }

                foreach (IRow row in rowsToReturn)
                {
                    yield return row;
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="className"></param>
        /// <param name="subtypes"></param>
        /// <returns></returns>
        public IEnumerable<IRow> GetInserts(string className, List<string> subtypes)
        {
            //bool hasChanges = HasChanges(className, esriDataChangeType.esriDataChangeTypeInsert);
            //if (hasChanges == false)
            //{
            //    return null;
            //}

            return GetChangedRows(className, subtypes, esriDifferenceType.esriDifferenceTypeInsert);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="className"></param>
        /// <param name="subtypes"></param>
        /// <returns></returns>
        public IEnumerable<IRow> GetDeletes(string className, List<string> subtypes)
        {
            //bool hasChanges = HasChanges(className, esriDataChangeType.esriDataChangeTypeDelete);
            //if (hasChanges == false)
            //{
            //    return null;
            //}

            //esriDifferenceType[] diffTypes =
            //    new esriDifferenceType[] {esriDifferenceType.esriDifferenceTypeDeleteNoChange
            //        , esriDifferenceType.esriDifferenceTypeDeleteUpdate};
            return GetChangedRows(className, subtypes, esriDifferenceType.esriDifferenceTypeDeleteNoChange);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="className"></param>
        /// <param name="subtypes"></param>
        /// <returns></returns>
        public IEnumerable<IRow> GetUpdates(string className, List<string> subtypes)
        {
            //bool hasChanges = HasChanges(className, esriDataChangeType.esriDataChangeTypeUpdate);
            //if (hasChanges == false)
            //{
            //    return null;
            //}

            //esriDifferenceType[] diffTypes =
            //    new esriDifferenceType[] {esriDifferenceType.esriDifferenceTypeUpdateDelete
            //        , esriDifferenceType.esriDifferenceTypeUpdateNoChange
            //        , esriDifferenceType.esriDifferenceTypeUpdateUpdate};

            return GetChangedRows(className, subtypes, esriDifferenceType.esriDifferenceTypeUpdateNoChange);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="className"></param>
        /// <param name="subtypes"></param>
        /// <param name="differenceTypes"></param>
        /// <returns></returns>
        public IEnumerable<IRow> GetChangedRows(string className, List<string> subtypes, params esriDifferenceType[] differenceTypes)
        {
            if (string.IsNullOrEmpty(className))
            {
                throw new ArgumentNullException(className);
            }

            IWorkspace2 sourceWorkspace = (IWorkspace2)SourceVersion;
            if (sourceWorkspace.get_NameExists(esriDatasetType.esriDTTable, className) == false
                && sourceWorkspace.get_NameExists(esriDatasetType.esriDTFeatureClass, className) == false)
            {
                throw new ArgumentException("Table or FeatureClass does not exist.", className);
            }

            string where = string.Empty;
            if (subtypes.Count > 0)
            {
                foreach (string subtype in subtypes)
                {
                    where = where + "SubtypeCD = " + subtype + " OR ";
                }

                // remove the ending "spaceORspace"
                if (where.Length > 4)
                {
                    where = where.Substring(0, where.Length - 4);
                }
            }

            IEnumerable<IRow> changedRows = GetChangedRows(className, where, differenceTypes);

            foreach (IRow changedRow in changedRows)
            {
                yield return changedRow;
            }

            //IQueryFilter2 queryFilter = null;
            //if (string.IsNullOrEmpty(where) == false)
            //{
            //    queryFilter = new QueryFilterClass();
            //    queryFilter.WhereClause = where;
            //}


            //IFeatureWorkspace sourceFeatWorkspace = (IFeatureWorkspace)SourceVersion;
            //ITable sourceTable = sourceFeatWorkspace.OpenTable(className);
            //IVersionedTable sourceVersionedTable = (IVersionedTable)sourceTable;

            //IFeatureWorkspace targetFeatWorkspace = (IFeatureWorkspace)TargetVersion;
            //ITable targetTable = targetFeatWorkspace.OpenTable(className);

            //foreach (esriDifferenceType diffType in differenceTypes)
            //{
            //    IDifferenceCursor diffCur = sourceVersionedTable.Differences(targetTable, diffType, queryFilter);

            //    int oID;
            //    IRow aRow;

            //    diffCur.Next(out oID, out aRow);
            //    while (oID != -1)
            //    {
            //        if (diffType == esriDifferenceType.esriDifferenceTypeDeleteNoChange
            //            || diffType == esriDifferenceType.esriDifferenceTypeDeleteUpdate)
            //        {
            //            aRow = targetTable.GetRow(oID);
            //        }

            //        yield return aRow;
            //        diffCur.Next(out oID, out aRow);
            //    }

            //    Marshal.ReleaseComObject(diffCur);
            //}
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="className"></param>
        /// <param name="whereClause"></param>
        /// <returns></returns>
        public IEnumerable<IRow> GetInserts(string className, string whereClause)
        {
            return GetChangedRows(className, whereClause, esriDifferenceType.esriDifferenceTypeInsert);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="className"></param>
        /// <param name="whereClause"></param>
        /// <returns></returns>
        public IEnumerable<IRow> GetDeletes(string className, string whereClause)
        {
            esriDifferenceType[] diffTypes =
                new esriDifferenceType[] {esriDifferenceType.esriDifferenceTypeDeleteNoChange
                    , esriDifferenceType.esriDifferenceTypeDeleteUpdate};
            return GetChangedRows(className, whereClause, diffTypes);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="className"></param>
        /// <param name="whereClause"></param>
        /// <returns></returns>
        public IEnumerable<IRow> GetUpdates(string className, string whereClause)
        {
            esriDifferenceType[] diffTypes =
                new esriDifferenceType[] {esriDifferenceType.esriDifferenceTypeUpdateDelete
                    , esriDifferenceType.esriDifferenceTypeUpdateNoChange
                    , esriDifferenceType.esriDifferenceTypeUpdateUpdate};

            return GetChangedRows(className, whereClause, diffTypes);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="className"></param>
        /// <param name="whereClause"></param>
        /// <param name="differenceTypes"></param>
        /// <returns></returns>
        public IEnumerable<IRow> GetChangedRows(string className, string whereClause, params esriDifferenceType[] differenceTypes)
        {
            if (string.IsNullOrEmpty(className))
            {
                throw new ArgumentNullException(className);
            }

            IWorkspace2 sourceWorkspace = (IWorkspace2)SourceVersion;
            if (sourceWorkspace.get_NameExists(esriDatasetType.esriDTTable, className) == false
                && sourceWorkspace.get_NameExists(esriDatasetType.esriDTFeatureClass, className) == false)
            {
                throw new ArgumentException("Table or FeatureClass does not exist.", className);
            }

            IQueryFilter2 queryFilter = null;
            if (string.IsNullOrEmpty(whereClause) == false)
            {
                queryFilter = new QueryFilterClass();
                queryFilter.WhereClause = whereClause;
            }


            IFeatureWorkspace sourceFeatWorkspace = (IFeatureWorkspace)SourceVersion;
            ITable sourceTable = sourceFeatWorkspace.OpenTable(className);
            IVersionedTable sourceVersionedTable = (IVersionedTable)sourceTable;

            IFeatureWorkspace targetFeatWorkspace = (IFeatureWorkspace)TargetVersion;
            ITable targetTable = targetFeatWorkspace.OpenTable(className);

            foreach (esriDifferenceType diffType in differenceTypes)
            {
                IDifferenceCursor diffCur = sourceVersionedTable.Differences(targetTable, diffType, queryFilter);

                try
                {
                    int oID;
                    IRow aRow;

                    diffCur.Next(out oID, out aRow);
                    while (oID != -1)
                    {
                        if (diffType == esriDifferenceType.esriDifferenceTypeDeleteNoChange
                            || diffType == esriDifferenceType.esriDifferenceTypeDeleteUpdate)
                        {
                            aRow = targetTable.GetRow(oID);
                        }

                        yield return aRow;
                        diffCur.Next(out oID, out aRow);
                    }
                }
                finally
                {
                    Marshal.ReleaseComObject(diffCur);
                }
            }
        }

        /// <summary>
        /// Call Initialize before this.
        /// Insert, Update rows are readonly rows, deleted rows are from the parent version, so don't modify.
        /// If it is needed to filter updated rows by comparing field values between source and target versions, 
        /// use FilterDifferenceManager.GetUpdatedRows.
        /// </summary>
        /// <param name="className">Table or FeatureClass name</param>
        /// <param name="dataChangeType"></param>
        /// <returns></returns>
        public IEnumerable<IRow> GetChangedRows(string className, esriDataChangeType dataChangeType)
        {
            if (string.IsNullOrEmpty(className))
            {
                throw new ArgumentNullException(className);
            }

            IWorkspace2 sourceWorkspace = (IWorkspace2)SourceVersion;
            if (sourceWorkspace.get_NameExists(esriDatasetType.esriDTTable, className) == false
                && sourceWorkspace.get_NameExists(esriDatasetType.esriDTFeatureClass, className) == false)
            {
                throw new ArgumentException("Table or FeatureClass does not exist.", className);
            }

            ITable targetTable = null;
            if (dataChangeType == esriDataChangeType.esriDataChangeTypeDelete)
            {
                IFeatureWorkspace targetFeatWorkspace = (IFeatureWorkspace)TargetVersion;
                targetTable = targetFeatWorkspace.OpenTable(className);
                if (targetTable == null)
                {
                    string message = string.Format("Table or FeatureClass {0} was not opened correctly", className);
                    throw new ApplicationException(message);
                }
            }

            IDataChanges dataChanges = (IDataChanges)_versionDataChangesInit;
            IDifferenceCursor diffCur = dataChanges.Extract(className, dataChangeType);

            int oID;
            IRow aRow;

            diffCur.Next(out oID, out aRow);
            while (oID != -1)
            {
                if (dataChangeType == esriDataChangeType.esriDataChangeTypeDelete)
                {
                    aRow = targetTable.GetRow(oID);
                }

                yield return aRow;
                diffCur.Next(out oID, out aRow);
            }

            Marshal.ReleaseComObject(diffCur);
        }

        /// <summary>
        /// Return new related objects for the given dictionary.
        /// </summary>
        /// <param name="inputDict">DifferenceDictionary 2 to run the logic against.</param>
        /// <returns>Initial Dictionary with the newly found Features </returns>
        public DifferenceDictionary AddObjectsRelatedToObjects(DifferenceDictionary inputDict)
        {
            try
            {
                //temporary collection of rows.
                DifferenceDictionary newRows = new DifferenceDictionary(inputDict.GetWorkspace(DifferenceDictionaryWorkspace.Auto));

                //Workign at the table level for initial test.
                foreach (ITable table in inputDict.KeysAsTable)
                {
                    newRows = Related_Object_Processing_For_Table(table, inputDict, newRows);
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(table);
                }

                //Add the new feature rows detected to the original list.
                foreach (string table in newRows.Keys)
                {
                    if (inputDict.ContainsKey(table))
                    {
                        foreach (DiffRow row in newRows[table])
                        {
                            //need to make sure the row is unique and not already in the original list.
                            if (!inputDict[table].Contains(row))
                            {
                                inputDict[table].Add(row);
                            }
                        }
                    }
                    else
                    {
                        //new table so add all of the new rows for the list.
                        inputDict.Add(table, newRows[table]);
                    }
                }
                newRows = null;
            }
            catch (Exception ex)
            {
                Logger.WriteLine(ex);
            }
            return inputDict;
        }

        /// <summary>
        /// Loads the differences into the Inserts, Updates and Deletes properties using the specified types.
        /// </summary>
        /// <param name="loadFeederManager20Updates">Defines whether to obtain feeder manager 2.0 "Updates" from the MM_Edited_Features table</param>
        /// <param name="differenceTypes">The difference types.</param>
        /// <returns>
        /// 	<c>true</c> if the differences are loaded; otherwise <c>false</c>.
        /// </returns>       
        public bool LoadDifferences(bool loadFeederManager20Updates, params esriDataChangeType[] differenceTypes)
        {
            try
            {
                IWorkspace targetWorkspace = (IWorkspace)this.TargetVersion;
                IWorkspace sourceWorkspace = (IWorkspace)this.SourceVersion;
                IWorkspace commonAncestorWorkspace = (IWorkspace)this.CommonAncestorVersion;

                // Initialize the dictionaries.
                this.Inserts = new DifferenceDictionary(sourceWorkspace, esriDataChangeType.esriDataChangeTypeInsert);
                this.Updates = new DifferenceDictionary(sourceWorkspace, targetWorkspace, esriDataChangeType.esriDataChangeTypeUpdate);
                this.Deletes = new DifferenceDictionary(targetWorkspace, esriDataChangeType.esriDataChangeTypeDelete);

                // Itterate through each of the modified classes in this version to get differences.
                //IVersionDataChangesInit vdci = new VersionDataChangesClass();
                //IWorkspaceName wsNameSource = (IWorkspaceName)((IDataset)this.SourceVersion).FullName;
                //IWorkspaceName wsNameTarget = (IWorkspaceName)((IDataset)this.TargetVersion).FullName;
                //vdci.Init(wsNameSource, wsNameTarget);
                //VersionDataChanges vdc = (VersionDataChanges)vdci;
                //IDataChanges dataChanges = (IDataChanges)vdci;
                if (_versionDataChangesInit == null) Initialize();
                VersionDataChanges vdc = (VersionDataChanges)_versionDataChangesInit;
                IDataChanges dataChanges = (IDataChanges)_versionDataChangesInit;
                IDataChangesInfo dci = (IDataChangesInfo)vdc;
                Logger.WriteLine(System.Diagnostics.TraceEventType.Information, "Initialized the Geodatabase cursors and about to make call to IDatachanges.GetModifiedClassesInfo, this can take several minutes for a hundred thousand changes.");
                Logger.Flush();
                //IEnumModifiedClassInfo enumMCI = dataChanges.GetModifiedClassesInfo();
                List<string> modifiedTableNames = ModifiedClassNames;
                Logger.WriteLine(System.Diagnostics.TraceEventType.Information, "IDatachanges.GetModifiedClassesInfo, has returned");
                Logger.Flush();

                List<IGeometricNetwork> editedGeometricNetworks = new List<IGeometricNetwork>();

                //enumMCI.Reset();
                //IModifiedClassInfo mci;
                //while ((mci = enumMCI.Next()) != null)
                //{
                //    string tableName = mci.ChildClassName;
                //    ITable diffTable = null;
                foreach (string tableName in modifiedTableNames)
                {
                    ITable diffTable = null;
                    try
                    {
                        diffTable = ((IFeatureWorkspace)this.SourceVersion).OpenTable(tableName);
                        Logger.WriteLine(System.Diagnostics.TraceEventType.Information, "Initializing differences for table : " + tableName);
                        Logger.Flush();
                    }
                    catch
                    {
                        Logger.WriteLine(TraceEventType.Warning, "The table " + tableName + " either could not be " +
                             "located in the current version or it is not a versioned table.");
                    }

                    // Get the differences between these tables.
                    if (diffTable != null)
                    {
                        if (loadFeederManager20Updates && diffTable is INetworkClass)
                        {
                            INetworkClass networkClass = diffTable as INetworkClass;
                            IGeometricNetwork editedNetwork = networkClass.GeometricNetwork;
                            if (editedNetwork != null && !editedGeometricNetworks.Contains(editedNetwork))
                            {
                                editedGeometricNetworks.Add(editedNetwork);
                            }
                        }

                        this.GetDifferences(dci, tableName, diffTable, differenceTypes);
                        System.Runtime.InteropServices.Marshal.ReleaseComObject(diffTable);
                    }
                }

                //Now need to process for the feeder manager 2.0 edits.
                if (loadFeederManager20Updates)
                {
                    try
                    {
                        foreach (IGeometricNetwork geomNetwork in editedGeometricNetworks)
                        {
                            int feederManager20UpdateCount = 0;
                            Logger.WriteLine(System.Diagnostics.TraceEventType.Information, "Obtaining feeder manager 2.0 updates : " + ((IDataset)geomNetwork).BrowseName);
                            Logger.Flush();
                            IFeatureClassContainer featClassContainer = geomNetwork as IFeatureClassContainer;
                            IFeatureClass featClass = null;
                            for (int i = 0; i < featClassContainer.ClassCount; i++)
                            {
                                featClass = featClassContainer.get_Class(i);
                                IDataset ds = featClass as IDataset;
                                List<int> oids = FeederManager2.GetEditedFeatures((IFeatureWorkspace)this.SourceVersion, this.SourceVersion.VersionName, (IObjectClass)featClass);
                                IFeatureCursor features = featClass.GetFeatures(oids.ToArray(), false);

                                string tableName = ds.BrowseName;
                                DifferenceDictionary diffDict = Updates;
                                DiffTable diffTable = null;
                                if (diffDict.ContainsKey(tableName))
                                {
                                    diffTable = diffDict[tableName];
                                }
                                else
                                {
                                    diffTable = new DiffTable((ITable)featClass);
                                    diffDict.Add(tableName, diffTable);
                                }

                                IFeature feature = null;
                                while ((feature = features.NextFeature()) != null)
                                {
                                    diffTable.AddUnique(new DiffRow(feature.OID, esriDataChangeType.esriDataChangeTypeUpdate));
                                    feederManager20UpdateCount++;
                                }
                            }
                            Logger.WriteLine(System.Diagnostics.TraceEventType.Information, "Feeder Manager 2.0 updated identified : " + feederManager20UpdateCount);
                            Logger.Flush();
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.WriteLine(System.Diagnostics.TraceEventType.Error, "Error obtaining Feeder Manager 2.0 updates : " + ex.Message + " StackTrace: " + ex.StackTrace);
                        Logger.Flush();
                    }
                }

                return true;
            }
            catch (Exception e)
            {
                Logger.WriteLine(e);
                Logger.Flush();
                EventLogger.Error(e);
            }

            return false;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="workspaceType"></param>
        /// <returns></returns>
        public DataSet UpdatesDataSet(DifferenceDictionaryWorkspace workspaceType)
        {
            if (_updateDataset != null && _updateDataset.ContainsKey(workspaceType))
            {
                return _updateDataset[workspaceType];
            }
            else
            {
                DataSet ds = GetDataSetFromDifferenceDictionary(Updates, workspaceType);
                _updateDataset.Add(workspaceType, ds);
                return ds;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public DataSet DeletesDataSet()
        {
            if (_deleteDataset == null)
            {
                _deleteDataset = GetDataSetFromDifferenceDictionary(Deletes, DifferenceDictionaryWorkspace.Primary);
            }
            return _deleteDataset;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public DataSet InsertsDataSet()
        {
            if (_insertDataset == null)
            {
                _insertDataset = GetDataSetFromDifferenceDictionary(Inserts, DifferenceDictionaryWorkspace.Primary);
            }
            return _insertDataset;
        }
        #endregion

        #region Protected Methods

        /// <summary>
        /// Adds the specified row information to the appropriate difference dictionary.
        /// </summary>
        /// <param name="table">The table.</param>
        protected void AddToDiffDictionary(DiffTable table)
        {
            // Determine the dictionary to add the row.
            foreach (DiffRow diffRow in table)
            {
                DifferenceDictionary dictionary = null;

                if (this.Inserts.DifferenceTypes.Contains(diffRow.DifferenceType))
                {
                    dictionary = this.Inserts;
                }
                else if (this.Updates.DifferenceTypes.Contains(diffRow.DifferenceType))
                {
                    dictionary = this.Updates;
                }
                else if (this.Deletes.DifferenceTypes.Contains(diffRow.DifferenceType))
                {
                    dictionary = this.Deletes;
                }

                // Add to the appropriate list.
                if (dictionary != null)
                {
                    if (!dictionary.ContainsKey(table.TableName))
                    {
                        dictionary.Add(table.TableName, new DiffTable(table.TableName, table.IsFeatureClass, table.RowType) { diffRow });
                    }
                    else
                    {
                        if (!dictionary[table.TableName].Contains(diffRow.OID))
                        {
                            dictionary[table.TableName].Add(diffRow);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Test to tell if a table has relationship classes at all
        /// </summary>
        /// <param name="table">The table to test</param>
        /// <returns><c>true</c> if relationship exists <c>false</c> if it does not</returns>
        protected bool HasRelationshipClass(ITable table)
        {
            bool hasRelationships = false;
            try
            {
                IEnumRelationshipClass enumRelationshipClass = ((table as IObjectClass)).get_RelationshipClasses(esriRelRole.esriRelRoleAny);
                if (enumRelationshipClass.Next() != null)
                {
                    hasRelationships = true; // not null, means relationships exist.
                    while (Marshal.ReleaseComObject(enumRelationshipClass) > 0) { }
                }
                else
                {
                    hasRelationships = false; // is null = 0 relationships
                }
            }
            catch (Exception ex)
            {
                hasRelationships = false; // If trying to get the relationshipclass reference fails, then it does not exist.
                Logger.WriteLine(System.Diagnostics.TraceEventType.Error, "Unable to get relationship for the requested table");
                if (table != null)
                {
                    if (table is IDataset)
                    {
                        Logger.WriteLine(System.Diagnostics.TraceEventType.Error, "Table: " + ((IDataset)table).Name);
                    }
                }
                Logger.WriteLine(ex);
                Logger.Flush();
                throw ex;
            }
            return hasRelationships;
        }

        /// <summary>
        /// Tests the ITable object to see if it an Attributed relathionship table.
        /// </summary>
        /// <param name="table">Table object to test.</param>
        /// <returns></returns>
        protected bool IsAttributeRelationshipClass(ITable table)
        {
            bool test = false;
            try
            {
                IFeatureWorkspace fw = (IFeatureWorkspace)(((IDataset)table).Workspace);
                IRelationshipClass tempRel = fw.OpenRelationshipClass((table as IDataset).Name);
                if (tempRel != null)
                {
                    test = true;
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(tempRel);
                }
            }
            catch
            {
            }
            return test;
        }

        /// <summary>
        /// Provides overload capability for additional logic to be applied once the related table is available.
        /// </summary>
        /// <param name="table">Object to check</param>
        /// <returns>Always returns <c>false</c></returns>
        protected virtual bool CheckRelatedTable(ITable table)
        {
            return false;
        }

        /// <summary>
        /// Creates a difference cursor between the specified versioned table and the difference table.
        /// </summary>
        /// <param name="dci">Table to examine for differences.</param>
        /// <param name="tableName">Name of the table used to determine differences.</param>
        /// <param name="sourceTable">ITable object of the table, used to initialize the DiffRow2.</param>
        /// <param name="diffTypes">The difference types.</param>
        protected virtual void GetDifferences(IDataChangesInfo dci, string tableName, ITable sourceTable, params esriDataChangeType[] diffTypes)
        {
            // Iterate through all of the difference types loading those changes.
            foreach (esriDataChangeType diffType in diffTypes)
            {
                DifferenceDictionary diffDict = null;
                if (Inserts.DifferenceTypes.Contains(diffType))
                {
                    diffDict = Inserts;
                }
                else if (Deletes.DifferenceTypes.Contains(diffType))
                {
                    diffDict = Deletes;
                }
                else if (Updates.DifferenceTypes.Contains(diffType))
                {
                    diffDict = Updates;
                }

                DiffTable diffTable = null;
                if (diffDict.ContainsKey(tableName))
                {
                    diffTable = diffDict[tableName];
                }
                else
                {
                    diffTable = new DiffTable(sourceTable);
                    diffDict.Add(tableName, diffTable);
                }

                IFIDSet listIDs = dci.get_ChangedIDs(tableName, diffType);
                listIDs.Reset();
                int oid;
                listIDs.Next(out oid);

                while (oid != -1)
                {
                    //this.AddToDiffDictionary(new DiffRow2(table, oid, diffType));
                    diffTable.Add(new DiffRow(oid, diffType));
                    listIDs.Next(out oid);
                }

                //this.AddToDiffDictionary(diffTable);
            }
        }

        /// <summary>
        /// Gets related objects for all OIDs belonging to the table in the dict Dictionary and then passes out the updated newRows dictionary for further processing.
        /// </summary>
        /// <param name="table">table to look for relationships on</param>
        /// <param name="dict">provides the list of DiffRows2 that need to have thier related objects found.</param>
        /// <param name="newRows">provides the unique output dictionary to add the new objects (if any found that are unique and new).</param>
        /// <returns>newRows dictionary with any new unique values found, that were not in the source dict or the newrows dict already.</returns>
        protected virtual DifferenceDictionary Related_Object_Processing_For_Table(ITable table, DifferenceDictionary dict, DifferenceDictionary newRows)
        {
            #region Standard Has Relationship logic.
            string tableName = ((IDataset)table).Name;
            Logger.WriteLine(System.Diagnostics.TraceEventType.Information, "Initializing related objects for table : " + tableName);
            Logger.Flush();
            if (HasRelationshipClass(table))
            {
                //Relationship was found, try to get other related rows.
                if (dict[tableName].Count > 0)
                {
                    // there are OIDs for this table.
                    IEnumRelationshipClass enumRelationshipClass = ((table) as IObjectClass).get_RelationshipClasses(esriRelRole.esriRelRoleAny);
                    IRelationshipClass rel = enumRelationshipClass.Next();
                    //Make sure there are Relationships, then loop through them all.
                    while (rel != null)
                    {
                        try
                        {
                            //make sure this relationship is even valid.
                            bool doNotProcess = true;
                            if (((IDataset)(rel.DestinationClass)).Name == tableName)
                            {
                                doNotProcess = CheckRelatedTable((ITable)rel.OriginClass);
                            }
                            else if (((IDataset)(rel.OriginClass)).Name == tableName)
                            {
                                doNotProcess = CheckRelatedTable((ITable)rel.DestinationClass);
                            }
                            if (!doNotProcess)
                            {
                                List<IRow> resultRows = new List<IRow>();
                                foreach (DiffRow row in dict[tableName])
                                {
                                    IRow Row = dict[tableName].GetIRow(dict.GetWorkspace(DifferenceDictionaryWorkspace.Auto), row);
                                    if (Row == null)
                                    {
                                        Logger.WriteLine(System.Diagnostics.TraceEventType.Error, "Error encountered while trying to get " + tableName + "   ,objectid, " + row.OID + " ,row was not found.");
                                    }
                                    else
                                    {
                                        ESRI.ArcGIS.esriSystem.ISet newRelSet = rel.GetObjectsRelatedToObject(Row as IObject);
                                        newRelSet.Reset();
                                        if (newRelSet != null)
                                        {
                                            //for each object make a new IRow and add it to the result set.
                                            object tempObject = newRelSet.Next();
                                            while (tempObject != null)
                                            {
                                                IObject newRow = tempObject as IObject;
                                                IRow testRow = newRow as IRow;
                                                if (testRow != null)
                                                {
                                                    if (!CheckRelatedTable(testRow.Table))
                                                    {
                                                        resultRows.Add(testRow);
                                                    }
                                                }
                                                tempObject = newRelSet.Next();
                                            }
                                        }
                                        //For memory reasons, processing each row's relationships one at a time.
                                        if (resultRows.Count > 0)
                                        {
                                            newRows = ProcessRelResultRows(resultRows, row, dict, newRows);
                                            foreach (IRow Row2 in resultRows)
                                            {
                                                Marshal.ReleaseComObject(Row2);
                                            }
                                            resultRows.Clear();
                                        }
                                        Marshal.ReleaseComObject(Row);
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Logger.WriteLine(System.Diagnostics.TraceEventType.Error, "Unable to get relationship for the requested table");
                            if (table != null)
                            {
                                if (table is IDataset)
                                {
                                    Logger.WriteLine(System.Diagnostics.TraceEventType.Error, "Table: " + ((IDataset)table).Name);
                                }
                            }
                            Logger.WriteLine(ex);
                            Logger.Flush();
                            throw ex;
                        }
                        rel = enumRelationshipClass.Next();
                    }
                    while (Marshal.ReleaseComObject(enumRelationshipClass) > 0) { }
                }
            }
            #endregion
            #region Is Attribute Relationship Logic
            else if (IsAttributeRelationshipClass(table))
            { //AttributedRelationship Relationships must be handled with seperate logic
                //that tests both the Origin and Destination for new rows to add.
                //Otherwise changes to these tables would not cause their corresponding objects to be added.

                //Relationship was found, try to get other related rows.
                if (dict[table].Count > 0)
                {  // there are OIDs for this table.
                    IFeatureWorkspace fw = (IFeatureWorkspace)(((IDataset)table).Workspace);
                    IRelationshipClass rel = fw.OpenRelationshipClass((table as IDataset).Name);
                    if (rel != null)
                    {
                        try
                        {
                            bool doNotProcessOrigin = CheckRelatedTable((ITable)rel.OriginClass);
                            bool doNotProcessDest = CheckRelatedTable((ITable)rel.DestinationClass);
                            if (!(doNotProcessDest && doNotProcessOrigin))
                            {
                                //either the Dest or Origin is valid, so we will try to process the rows.
                                List<IRow> resultRows = new List<IRow>();
                                foreach (DiffRow row in dict[table])
                                {
                                    IRow Row = dict[table].GetIRow(dict.GetWorkspace(DifferenceDictionaryWorkspace.Auto), row);
                                    if (Row == null)
                                    {
                                        Logger.WriteLine(System.Diagnostics.TraceEventType.Error, "Error encountered while trying to get " + tableName + "   ,objectid, " + row.OID + " ,row was not found.");
                                    }
                                    else
                                    {
                                        if (!doNotProcessOrigin)
                                        {
                                            resultRows.Add(dict.GetObjectFromRelationshipTableValue(rel, true, Row));
                                        }
                                        if (!doNotProcessDest)
                                        {
                                            resultRows.Add(dict.GetObjectFromRelationshipTableValue(rel, false, Row));
                                        }
                                        //For memory reasons, processing each row's relationships one at a time.
                                        if (resultRows.Count > 0)
                                        {
                                            newRows = ProcessRelResultRows(resultRows, row, dict, newRows);
                                            foreach (IRow Row2 in resultRows)
                                            {
                                                Marshal.ReleaseComObject(Row2);
                                            }
                                            resultRows.Clear();
                                        }
                                        Marshal.ReleaseComObject(Row);
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Logger.WriteLine(System.Diagnostics.TraceEventType.Error, "Unexpected failure while getting related objects for Attributed Relationship Table");
                            if (tableName != null)
                            {
                                Logger.WriteLine(System.Diagnostics.TraceEventType.Error, "Table that was being processed when error occured: " + tableName);
                            }
                            Logger.WriteLine(ex);
                        }
                    }
                    while (Marshal.ReleaseComObject(rel) > 0) { }
                }//end if count > 0
            }//end if IsAttributedRelationshipClass
            #endregion
            return newRows;
        }

        /// <summary>
        /// Populates the Field Indices to be populated from a table. 
        /// Difference Manager sends a list of all the fields. Can be overridden to get specific fields based on modelnames or field inclusion list
        /// </summary>
        /// <param name="table"></param>
        /// <returns></returns>
        protected virtual IEnumerable<int> FieldIndicesToPopulate(ITable table)
        {
            return Enumerable.Range(0, table.Fields.FieldCount);
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Short format and upper case feature or object class names.
        /// </summary>
        /// <returns></returns>
        public List<string> ModifiedClassNames
        {
            get
            {
                if (_modifiedClassNames == null)
                {
                    _modifiedClassNames = new List<string>();
                    foreach (string FCName in _cdVersionDifference.Keys)
                    {
                        _modifiedClassNames.Add(FCName.ToUpper());
                    }
                }

                return _modifiedClassNames;
            }
        }

        // short and upper case names
        private List<string> GetModifiedClassNames(IDataChanges dataChanges)
        {
            List<string> changedNames = new List<string>();
            // this seems very expensive
            IEnumModifiedClassInfo enumModifiedClassInfo = dataChanges.GetModifiedClassesInfo();
            enumModifiedClassInfo.Reset();
            IModifiedClassInfo modifiedClassInfo = enumModifiedClassInfo.Next();
            while (modifiedClassInfo != null)
            {
                //changedNames.Add(modifiedClassInfo.ParentClassName.ToUpper());
                changedNames.Add(modifiedClassInfo.ChildClassName.ToUpper());

                modifiedClassInfo = enumModifiedClassInfo.Next();
            }

            return changedNames;
        }

        private IFIDSet2 GetChangedFIDSet(string className, esriDataChangeType dataChangeType)
        {
            if (ModifiedClassNames.Contains(className.ToUpper()) == false)
            {
                return null;
            }

            IDataset sourceDataset = (IDataset)SourceVersion;
            IName sourceVersionName = sourceDataset.FullName;
            IWorkspaceName2 sourceWorkspaceName = (IWorkspaceName2)sourceVersionName;

            IDataset targetDataset = (IDataset)TargetVersion;
            IName targetVersionName = targetDataset.FullName;
            IWorkspaceName2 targetWorkspaceName = (IWorkspaceName2)targetVersionName;

            IVersionDataChangesInit versionDataChangesInit = new VersionDataChangesClass();
            versionDataChangesInit.Init(sourceWorkspaceName, targetWorkspaceName);

            IDataChangesInfo dataChangesInfo = (IDataChangesInfo)versionDataChangesInit;

            IFIDSet fIDSet = dataChangesInfo.get_ChangedIDs(className, dataChangeType);
            IFIDSet2 fIDSet2 = (IFIDSet2)fIDSet;

            return fIDSet2;
        }

        private List<int> GetChangedIDs(string className, esriDataChangeType dataChangeType)
        {
            List<int> fIDList = new List<int>();

            IFIDSet2 fIDSet2 = GetChangedFIDSet(className, dataChangeType);
            if (fIDSet2 == null)
            {
                return fIDList;
            }

            IEnumIDs fIDs = fIDSet2.IDs;
            int fID;
            fIDs.Reset();
            fID = fIDs.Next();
            while (fID != -1)
            {
                fIDList.Add(fID);
                fID = fIDs.Next();
            }
            //int[] fIDArray = fIDList.ToArray();

            return fIDList;
        }

        /// <summary>
        /// Produces a dictionary wiith new rows based on a list of IRows.
        /// </summary>
        /// <param name="resultRows">List of IRows to analyze and build off of</param>
        /// <param name="row">the source related object</param>
        /// <param name="dict">Original Dictionary to look in.</param>
        /// <param name="newRows">Dictionary the hold the unique results</param>
        /// <returns>newRows dictionary + the results from comparison</returns>
        private DifferenceDictionary ProcessRelResultRows(List<IRow> resultRows, DiffRow row, DifferenceDictionary dict, DifferenceDictionary newRows)
        {
            foreach (IRow tempRow in resultRows)
            {
                DiffRow diffRow = new DiffRow(tempRow.OID, row.DifferenceType);
                string tabName = ((IDataset)tempRow.Table).Name;
                if (dict.ContainsKey(tabName))
                {
                    if (!dict[tabName].Contains(diffRow))
                    {
                        if (newRows.ContainsKey(tabName))
                        {
                            if (!newRows[tabName].Contains(diffRow))
                            {
                                newRows[tabName].Add(diffRow);
                            }
                        }
                        else
                        {
                            newRows.Add(tabName, new DiffTable(tempRow.Table) { diffRow });
                        }
                    }
                }
                else
                {
                    if (newRows.ContainsKey(tabName))
                    {
                        if (!newRows[tabName].Contains(diffRow))
                        {
                            newRows[tabName].Add(diffRow);
                        }
                    }
                    else
                    {
                        newRows.Add(tabName, new DiffTable(tempRow.Table) { diffRow });
                    }
                }
            }
            return newRows;
        }

        /// <summary>
        /// Constructs a IN Whereclause from the Array of OIDS. IN Clause has a limit of  
        /// </summary>
        /// <param name="oids"></param>
        /// <param name="table"></param>
        /// <returns></returns>
        private string ConstructWhereClause(int[] oids, ITable table)
        {
            int extra = oids.Count() % 1000;
            int noOfParts = (oids.Count() - extra) / 1000;
            if (extra > 0) noOfParts++;
            int i = 0;
            //Split the OIDS by chunks of 100's to pass it through the IN CLAUSE. IN CLAUSE has certain limit for the number of discrete ids.
            // This might be slow need to investigate better option. Especially in ORACLE IN CLAUSE is really slow when the number of records in the table is less.
            IEnumerable<IEnumerable<int>> splitOids = from item in oids
                                                      group item by i++ % noOfParts into part
                                                      select part.AsEnumerable();
            string oidFieldName = table.OIDFieldName;
            string whereClause = string.Empty;
            bool prefixOR = false;
            foreach (IEnumerable<int> oidPart in splitOids)
            {
                //Do not add the OR CLAUSE the first time
                if (prefixOR) whereClause += " OR ";
                else prefixOR = true;
                whereClause += oidFieldName + " IN (" + TypeCastFacade.ToString<int>(oids) + ")";
            }
            return whereClause;
        }

        /// <summary>
        /// Gets a Dataset with the datatables for each table in the Diffdict.
        /// The tables will have the primary key column set to OID field.
        /// </summary>
        /// <param name="diffDict">DiffDict</param>
        /// <param name="workspaceType">DiffrenceDictionaryWorkspace to use</param>
        /// <returns></returns>
        private DataSet GetDataSetFromDifferenceDictionary(DifferenceDictionary diffDict, DifferenceDictionaryWorkspace workspaceType)
        {
            IRow row = null;
            ICursor cursor = null;
            ITable table = null;
            try
            {
                DataSet retVal = new DataSet();
                foreach (string tableName in diffDict.Keys)
                {
                    table = diffDict.AsTable(tableName, workspaceType);
                    DataTable dt = new DataTable(tableName);
                    IEnumerable<int> fieldsToPopulate = FieldIndicesToPopulate(table);
                    foreach (int fieldIndex in fieldsToPopulate)
                    {
                        dt.Columns.Add(table.Fields.get_Field(fieldIndex).Name);
                    }
                    string[] subfields = dt.Columns.Cast<DataColumn>().Select(x => x.ColumnName).ToArray();

                    int[] oids = diffDict[table].Select(diffrow => diffrow.OID).ToArray();
                    //object oidArray = (object)oids;
                    //cursor = table.GetRows(oidArray, false);
                    IQueryFilter qf = new QueryFilterClass();
                    qf.WhereClause = ConstructWhereClause(oids, table);
                    qf.SubFields = string.Join(",", subfields);
                    cursor = table.Search(qf, false);
                    while ((row = cursor.NextRow()) != null)
                    {
                        DataRow dr = dt.NewRow();
                        for (int i = 0; i < fieldsToPopulate.Count(); i++)
                        {
                            dr[i] = row.get_Value(fieldsToPopulate.ElementAt(i));
                        }
                        dt.Rows.Add(dr);
                        while (System.Runtime.InteropServices.Marshal.ReleaseComObject(row) > 0) { };
                    }
                    dt.PrimaryKey = new DataColumn[] { dt.Columns[table.OIDFieldName] };
                    retVal.Tables.Add(dt);
                    while (System.Runtime.InteropServices.Marshal.ReleaseComObject(table) > 0) { };
                }
                retVal.AcceptChanges();
                return retVal;
            }
            finally
            {
                while (System.Runtime.InteropServices.Marshal.ReleaseComObject(cursor) > 0) { };
            }
        }
        #endregion
    }
}