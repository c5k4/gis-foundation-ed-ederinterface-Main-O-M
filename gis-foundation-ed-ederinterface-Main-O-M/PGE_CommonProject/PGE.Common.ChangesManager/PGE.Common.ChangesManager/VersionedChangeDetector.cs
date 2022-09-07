using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.GeoDatabaseDistributed;
using PGE.Common.ChangesManager.Utilities;
using System.Reflection;
using PGE.Common.ChangesManagerShared;
using PGE.Common.ChangesManagerShared.Interfaces;
using PGE.Common.ChangesManagerShared.Utilities;
using PGE.Common.Delivery.Diagnostics;
//V3SF - CD API (EDGISREARC-1452) - Added
using PGE.Common.ChangeDetectionAPI;

namespace PGE.Common.ChangesManager
{
    /// <summary>
    /// A supporting class that will load all of the changes between the source and target versions.
    /// </summary>
    public class VersionedChangeDetector : IChangeDetector
    {
        public string _versionTempName = "Daily_Change_Temp";

        public string VersionTempName
        {
            get
            {
                return _versionTempName;
            }
            set
            {
                _versionTempName = value;
            }
        }

        #region Private Variables
        private static readonly Log4NetLogger _logger = new Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "");

        private ChangeDictionary _updates;
        private ChangeDictionary _deletes;
        private ChangeDictionary _inserts;
        private IDictionary<string, ChangedFeatures> changedFeatures;
        public SDEWorkspaceConnection SDEWorkspace { get; private set; }

        public SchemaRestrictor SchemaRestrictor { get; set; }
        public bool DoNotRollVersions
        { get; set; }

        #endregion Private Variables

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="VersionedChangeDetector"/> class.
        /// </summary>
        /// <param name="workspaceConnectionFile">The workspace to detect changes against.</param>
        /// <param name="targetVersionName">The target version to detect changes against.</param>
        /// <param name="sourceVersionName">The source version (the version containing the changes) to detect changes against.</param>
        public VersionedChangeDetector(SDEWorkspaceConnection sdeWorkspaceConnection, ChangeDetectionVersionInfo versionInfo)
        {
            _logger.Debug(MethodBase.GetCurrentMethod().Name);
            _logger.Info(String.Format("Source GDB [ {0} ] Target Version [ {1} ] Source Version [ {2} ]", 
                sdeWorkspaceConnection.WorkspaceConnectionFile, versionInfo.VersionTarget, versionInfo.VersionSource));

            try
            {
                SDEWorkspace = sdeWorkspaceConnection;
                _parentVersionName = versionInfo.VersionSource;
                _childVersionName = versionInfo.VersionTarget;
                _versionTempName = versionInfo.VersionTemp;
                //V3SF - CD API (EDGISREARC-1452) - Added [START]
                _versionTableName = versionInfo.VersionTable;
                _versionTableConnStrName = versionInfo.VersionTableConnStr;
                _versionWhereClause = versionInfo.VersionWhereClause;
                //V3SF - CD API (EDGISREARC-1452) - Added [END]
                //TODO: Make this lazy initialization to move out of constructor
                //V3SF - CD API (EDGISREARC-1452) - Added - Runs if changedetection Table info not provided
                if (string.IsNullOrWhiteSpace(_versionTableName))
                    InitializeVersions();
            }
            catch (Exception ex)
            {
                _logger.Debug(ex.ToString());
                throw ex;
            }
        }

        private void FindVersions()
        {
            _logger.Debug(MethodBase.GetCurrentMethod().Name);

            IVersionedWorkspace vWorkspace = (IVersionedWorkspace)SDEWorkspace.Workspace;
            _logger.Info("Preparing versions:");
            try
            {
                //Find required versions (excluding the temp version, as it requires more logic).
                _logger.Info("  Finding parent version [ " + _parentVersionName + " ]");
                this.sourceVersion = (IVersion2)vWorkspace.FindVersion(_parentVersionName);
                _logger.Info("  Finding child version [ " + _childVersionName + " ]");
                this.targetVersion = (IVersion2)vWorkspace.FindVersion(_childVersionName);
                _logger.Info("  Child version created [ " + this.targetVersion.VersionInfo.Created + " ] modified [ " + 
                    this.targetVersion.VersionInfo.Modified + " ] ");
            }
            catch (Exception ex)
            {
                _logger.Error(ex.ToString());
                throw new ErrorCodeException(ErrorCode.VersionRetrieval, "Issue finding version: " + ex.Message, ex);
            }

            //Validate the versions to make sure that the parent/child relationship is correct.
            if (!AOHelper.VersionIsChildOf(this.targetVersion, this.sourceVersion))
            {
                string errorMsg = "The versions configured to run did not have a proper parent/child relationship.";
                if (AOHelper.VersionIsChildOf(this.targetVersion, this.sourceVersion))
                    errorMsg += " Check to ensure that the version names are not in the wrong order.";

                _logger.Error(errorMsg);
                throw new ErrorCodeException(ErrorCode.VersionRetrieval, errorMsg);
            }

            //Try to find the temp version.
            _logger.Info("  Looking for temporary version (" + VersionTempName + ")...");

            try 
            { 
                this.dailyTempVersion = (IVersion2)vWorkspace.FindVersion(VersionTempName); 
            }
            catch { }

        }
        /// <summary>
        /// Use the class-level variables to load the versions into the class.
        /// </summary>
        private void InitializeVersions()
        {
            _logger.Debug(MethodBase.GetCurrentMethod().Name);

            FindVersions();
            _logger.Info("  Versions found.");

            //Delete existing temp version if it exists.
            if (this.dailyTempVersion != null)
            {
                dailyTempVersion.Delete();
                _logger.Info("    Deleted old temporary version (" + VersionTempName + ").");
            }
            //Get a fresh temp version so it reflects today's data and set it to protected so GDBM doesn't try to reconcile it
            this.sourceVersion.CreateVersion(VersionTempName);
            _logger.Info("    Created new temporary version (" + VersionTempName + ").");
            dailyTempVersion = (IVersion2)((IVersionedWorkspace)SDEWorkspace.Workspace).FindVersion(VersionTempName);
            dailyTempVersion.Access = esriVersionAccess.esriVersionAccessPublic;

            //this.CommonAncestorVersion = ((IVersion2)sourceVersion).GetCommonAncestor(targetVersion);
        }

        #endregion


        public void Dispose()
        {
            while (Marshal.ReleaseComObject(this.CommonAncestorVersion) > 0) { }
            this.CommonAncestorVersion = null;
            this.targetVersion = null;
            this.sourceVersion = null;
        }
        
        #region Public Properties

        /// <summary>
        /// Dictionary mapping a list of features that have been deleted in the source version to the corresponding
        /// table name.
        /// </summary>
        public ChangeDictionary Deletes
        {
            get
            {
                return _deletes;
            }

        }

        /// <summary>
        /// Dictionary mapping a list of features that have been inserted in the source version to the corresponding
        /// table name.
        /// </summary>
        public ChangeDictionary Inserts
        {
            get
            {
                return _inserts;
            }
        }

        /// <summary>
        /// Dictionary mapping a list of features that have been updated in the source version to the corresponding
        /// table name.
        /// </summary>
        public ChangeDictionary Updates
        {
            get
            {
                return _updates;
            }
        }

        private string _parentVersionName;
        private string _childVersionName;

        //V3SF - CD API (EDGISREARC-1452) - Added [START]
        private string _versionTableName;
        private string _versionTableConnStrName;
        private string _versionWhereClause;
        //V3SF - CD API (EDGISREARC-1452) - Added [END]
        /// <summary>
        /// Gets the common ancestor version.
        /// </summary>
        public IVersion CommonAncestorVersion
        {
            get;
            private set;
        }


        /// <summary>
        /// The source version used for the difference evaluation.
        /// </summary>
        public IVersion sourceVersion
        {
            get;
            private set;
        }

        /// <summary>
        /// The target version used for the difference evaluation.
        /// </summary>
        public IVersion targetVersion
        {
            get;
            private set;
        }

        /// <summary>
        /// The target version used for the difference evaluation.
        /// </summary>
        public IVersion dailyTempVersion
        {
            get;
            private set;
        }


        #endregion

        #region Public Methods

        /// <summary>
        /// Return new related objects for the given dictionary.
        /// </summary>
        /// <param name="inputDict">ChangesDictionary 2 to run the logic against.</param>
        /// <returns>Initial Dictionary with the newly found Features </returns>
        public ChangeDictionary AddObjectsRelatedToObjects(ChangeDictionary inputDict)
        {
            _logger.Debug(MethodBase.GetCurrentMethod().Name);

            try
            {
                //temporary collection of rows.
                ChangeDictionary newRows = new ChangeDictionary(inputDict.GetWorkspace(ChangeWorkspace.Auto));

                //Workign at the table level for initial test.
                foreach (ITable table in inputDict.KeysAsTable)
                {
                    newRows = Related_Object_Processing_For_Table(table, inputDict, newRows);
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(table);
                }

                //Add the new feature rows detected to the isOriginal list.
                foreach (string table in newRows.Keys)
                {
                    if (inputDict.ContainsKey(table))
                    {
                        foreach (ChangeRow row in newRows[table])
                        {
                            //need to make sure the row is unique and not already in the isOriginal list.
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
                _logger.Error(ex.ToString());
            }
            return inputDict;
        }

        public void Cleanup()
        {
            _logger.Debug(MethodBase.GetCurrentMethod().Name);

            if (!DoNotRollVersions)
            {
                RollVersions();
            }
        }

        public void Read()
        {

            LoadDifferences(
                new esriDataChangeType[] { esriDataChangeType.esriDataChangeTypeInsert, esriDataChangeType.esriDataChangeTypeUpdate,
                    esriDataChangeType.esriDataChangeTypeDelete }
                );

        }

        /// <summary>
        /// Loads the changes into the Inserts, Updates and Deletes properties using the specified types.
        /// </summary>
        /// <param name="differenceTypes">The difference types.</param>
        /// <returns>
        /// 	<c>true</c> if the changes are loaded; otherwise <c>false</c>.
        /// </returns>       
        public void LoadDifferences(params esriDataChangeType[] differenceTypes)
        {
            _logger.Debug(MethodBase.GetCurrentMethod().Name);

            IVersionDataChangesInit vdci = null;
            IEnumModifiedClassInfo enumMCI = null;

            try
            {
                IWorkspace targetWorkspace = (IWorkspace)this.targetVersion;
                IWorkspace sourceWorkspace = (IWorkspace)this.sourceVersion;

                // Initialize the dictionaries.
                _inserts = new ChangeDictionary(sourceWorkspace, esriDataChangeType.esriDataChangeTypeInsert);
                _updates = new ChangeDictionary(sourceWorkspace, targetWorkspace, esriDataChangeType.esriDataChangeTypeUpdate);
                _deletes = new ChangeDictionary(targetWorkspace, esriDataChangeType.esriDataChangeTypeDelete);

                // loop through each of the modified classes in this version to get differences.
                vdci = new ESRI.ArcGIS.GeoDatabaseDistributed.VersionDataChangesClass();
                IWorkspaceName wsNameSource = (IWorkspaceName)((IDataset)this.sourceVersion).FullName;
                IWorkspaceName wsNameTarget = (IWorkspaceName)((IDataset)this.targetVersion).FullName;
                vdci.Init(wsNameSource, wsNameTarget);
                VersionDataChanges vdc = (VersionDataChanges)vdci;
                IDataChanges dataChanges = (IDataChanges)vdci;
                IDataChangesInfo dci = (IDataChangesInfo)vdc;
                _logger.Info("Initialized the Geodatabase cursors and about to make call to IDatachanges.GetModifiedClassesInfo, this can take several minutes for a hundred thousand changes.");
                enumMCI = dataChanges.GetModifiedClassesInfo();
                _logger.Info("IDatachanges.GetModifiedClassesInfo, has returned");
                enumMCI.Reset();
                IModifiedClassInfo mci;
                while ((mci = enumMCI.Next()) != null)
                {
                    string tableName = mci.ChildClassName;
                    if (SchemaRestrictor != null && !SchemaRestrictor.FeatureClassIsIncluded(tableName))
                    {
                        _logger.Info("Ignoring changes (Schema Restriction) for table : " + tableName);
                        continue;
                    }
                    ITable ChangeTable = null;

                    try
                    {
                        ChangeTable = ((IFeatureWorkspace)this.sourceVersion).OpenTable(tableName);
                        _logger.Info("Initializing changes for table : " + tableName);
                    }
                    catch
                    {
                        _logger.Error("The table " + tableName + " either could not be " +
                             "located in the current version or it is not a versioned table.");
                    }

                    // Get the changes between the tables.
                    if (ChangeTable != null)
                    {
                        this.GetDifferences(dci, tableName, ChangeTable, differenceTypes);
                        while (System.Runtime.InteropServices.Marshal.ReleaseComObject(ChangeTable) > 0) { }
                    }
                }

            }
            finally
            {
                if (vdci != null)   while (Marshal.ReleaseComObject(vdci) > 0) { }
                if (enumMCI != null) while (Marshal.ReleaseComObject(vdci) > 0) { }
                vdci = null;
                enumMCI = null;
            }

        }

        /// <summary>
        /// V3SF - CD API (EDGISREARC-1452) - Added
        /// Use CD API to calculate Version Difference
        /// </summary>
        /// <param name="toDate"></param>
        public void ReadCD(DateTime toDate, DateTime fromDate)
        {
            string tableName = string.Empty;
            int oid = -1;
            ChangeTable ChangeTable;
            esriDataChangeType esriDataChangeType = default;
            IWorkspace sourceWorkspace = null;

            try
            {
                if (this.sourceVersion != null)
                    sourceWorkspace = (IWorkspace)this.sourceVersion;
                else
                    sourceWorkspace = SDEWorkspace.Workspace;
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message + " " + ex.StackTrace);
            }

            CDVersionManager cDVersionManager = new CDVersionManager(sourceWorkspace, _versionTableName, _versionTableConnStrName);
            //toDate = DateTime.Now; //Create public global Variable 
            string whereclause = _versionWhereClause;
            changedFeatures = new Dictionary<string, ChangedFeatures>();
            changedFeatures = cDVersionManager.CDVersionDifference(changeTypes.All, listType.None, fromDate: fromDate, toDate: toDate, withReplacement: false, whereClause: whereclause);

            var newDictionary = changedFeatures.ToDictionary(entry => entry.Key,
                                               entry => (ChangedFeatures)entry.Value.Clone());

            _inserts = new ChangeDictionary(sourceWorkspace, esriDataChangeType.esriDataChangeTypeInsert);
            _updates = new ChangeDictionary(sourceWorkspace, sourceWorkspace, esriDataChangeType.esriDataChangeTypeUpdate);
            _deletes = new ChangeDictionary(sourceWorkspace, esriDataChangeType.esriDataChangeTypeDelete);

            _inserts.changedFeatures = newDictionary;
            _updates.changedFeatures = newDictionary;
            _deletes.changedFeatures = newDictionary;

            foreach (string FCName in changedFeatures.Keys)
            {
                tableName = FCName.Split('.').Last();
                if (SchemaRestrictor != null && !SchemaRestrictor.FeatureClassIsIncluded(tableName))
                {
                    _logger.Info("Ignoring changes (Schema Restriction) for table : " + tableName);
                    continue;
                }
                
                if(changedFeatures[FCName].FeatClass == null)
                {
                    _logger.Info("Featureclass not found in current workspace  : " + tableName);
                    continue;
                }

                if(changedFeatures[FCName].Action.Insert.Count>0)
                {
                    ChangeTable = new ChangeTable(FCName, true);
                    esriDataChangeType = esriDataChangeType.esriDataChangeTypeInsert;

                    for (int i =0;i< changedFeatures[FCName].Action.Insert.Count;i++)
                    {
                        oid = Convert.ToInt32(changedFeatures[FCName].Action.Insert[i].OID);

                        if (changedFeatures[FCName].Action.Insert[i].feature == null)
                            continue;

                        _logger.Debug("Change [ " + FCName + " ] Diff [ " + Enum.GetName(typeof(esriDataChangeType), esriDataChangeType) + " ] OID [ " + oid.ToString() + " ]");
                        ChangeTable.Add(new ChangeRow(oid, esriDataChangeType));
                    }
                    
                    this.AddToDiffDictionary(ChangeTable);
                }

                if (changedFeatures[FCName].Action.Update.Count > 0)
                {
                    ChangeTable = new ChangeTable(FCName, true);
                    esriDataChangeType = esriDataChangeType.esriDataChangeTypeUpdate;

                    for (int i = 0; i < changedFeatures[FCName].Action.Update.Count; i++)
                    {
                        oid = Convert.ToInt32(changedFeatures[FCName].Action.Update[i].OID);

                        if (changedFeatures[FCName].Action.Update[i].feature == null)
                            continue;
                        
                        _logger.Debug("Change [ " + FCName + " ] Diff [ " + Enum.GetName(typeof(esriDataChangeType), esriDataChangeType) + " ] OID [ " + oid.ToString() + " ]");
                        ChangeTable.Add(new ChangeRow(oid, esriDataChangeType));
                    }

                    this.AddToDiffDictionary(ChangeTable);
                }

                #region Commented till Deleted xml changes are implemented
                if (changedFeatures[FCName].Action.Delete.Count > 0)
                {
                    ChangeTable = new ChangeTable(FCName, true);
                    esriDataChangeType = esriDataChangeType.esriDataChangeTypeDelete;

                    for (int i = 0; i < changedFeatures[FCName].Action.Delete.Count; i++)
                    {
                        oid = Convert.ToInt32(changedFeatures[FCName].Action.Delete[i].OID);

                        _logger.Debug("Change [ " + FCName + " ] Diff [ " + Enum.GetName(typeof(esriDataChangeType), esriDataChangeType) + " ] OID [ " + oid.ToString() + " ]");
                        ChangeTable.Add(new ChangeRow(oid, esriDataChangeType));
                    }

                    this.AddToDiffDictionary(ChangeTable);
                }
                #endregion
            }
        }

        /// <summary>
        /// V3SF - CD API (EDGISREARC-1452) - Added
        /// Use CD API to calculate Version Difference
        /// </summary>
        /// <param name="toDate"></param>
        public void ReadCDBatch(DateTime toDate, DateTime fromDate, string TableName, string BatchID)
        {
            string tableName = string.Empty;
            int oid = -1;
            ChangeTable ChangeTable;
            esriDataChangeType esriDataChangeType = default;
            IWorkspace sourceWorkspace = null;

            try
            {
                if (this.sourceVersion != null)
                    sourceWorkspace = (IWorkspace)this.sourceVersion;
                else
                    sourceWorkspace = SDEWorkspace.Workspace;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + " " + ex.StackTrace);
            }

            CDTempVersionManager cDVersionManager = new CDTempVersionManager(sourceWorkspace, TableName, _versionTableConnStrName);
            //toDate = DateTime.Now; //Create public global Variable 
            string whereclause = _versionWhereClause;
            changedFeatures = new Dictionary<string, ChangedFeatures>();
            changedFeatures = cDVersionManager.CDVersionDifference(BatchID,changeTypes.All, listType.None, fromDate: fromDate, toDate: toDate, withReplacement: false, whereClause: whereclause);

            var newDictionary = changedFeatures.ToDictionary(entry => entry.Key,
                                               entry => (ChangedFeatures)entry.Value.Clone());

            _inserts = new ChangeDictionary(sourceWorkspace, esriDataChangeType.esriDataChangeTypeInsert);
            _updates = new ChangeDictionary(sourceWorkspace, sourceWorkspace, esriDataChangeType.esriDataChangeTypeUpdate);
            _deletes = new ChangeDictionary(sourceWorkspace, esriDataChangeType.esriDataChangeTypeDelete);

            _inserts.changedFeatures = newDictionary;
            _updates.changedFeatures = newDictionary;
            _deletes.changedFeatures = newDictionary;

            foreach (string FCName in changedFeatures.Keys)
            {
                tableName = FCName.Split('.').Last();
                if (SchemaRestrictor != null && !SchemaRestrictor.FeatureClassIsIncluded(tableName))
                {
                    _logger.Info("Ignoring changes (Schema Restriction) for table : " + tableName);
                    continue;
                }

                if (changedFeatures[FCName].FeatClass == null)
                {
                    _logger.Info("Featureclass not found in current workspace  : " + tableName);
                    continue;
                }

                if (changedFeatures[FCName].Action.Insert.Count > 0)
                {
                    ChangeTable = new ChangeTable(FCName, true);
                    esriDataChangeType = esriDataChangeType.esriDataChangeTypeInsert;

                    for (int i = 0; i < changedFeatures[FCName].Action.Insert.Count; i++)
                    {
                        oid = Convert.ToInt32(changedFeatures[FCName].Action.Insert[i].OID);

                        if (changedFeatures[FCName].Action.Insert[i].feature == null)
                            continue;

                        _logger.Debug("Change [ " + FCName + " ] Diff [ " + Enum.GetName(typeof(esriDataChangeType), esriDataChangeType) + " ] OID [ " + oid.ToString() + " ]");
                        ChangeTable.Add(new ChangeRow(oid, esriDataChangeType));
                    }

                    this.AddToDiffDictionary(ChangeTable);
                }

                if (changedFeatures[FCName].Action.Update.Count > 0)
                {
                    ChangeTable = new ChangeTable(FCName, true);
                    esriDataChangeType = esriDataChangeType.esriDataChangeTypeUpdate;

                    for (int i = 0; i < changedFeatures[FCName].Action.Update.Count; i++)
                    {
                        oid = Convert.ToInt32(changedFeatures[FCName].Action.Update[i].OID);

                        if (changedFeatures[FCName].Action.Update[i].feature == null)
                            continue;

                        _logger.Debug("Change [ " + FCName + " ] Diff [ " + Enum.GetName(typeof(esriDataChangeType), esriDataChangeType) + " ] OID [ " + oid.ToString() + " ]");
                        ChangeTable.Add(new ChangeRow(oid, esriDataChangeType));
                    }

                    this.AddToDiffDictionary(ChangeTable);
                }

                #region Commented till Deleted xml changes are implemented
                if (changedFeatures[FCName].Action.Delete.Count > 0)
                {
                    ChangeTable = new ChangeTable(FCName, true);
                    esriDataChangeType = esriDataChangeType.esriDataChangeTypeDelete;

                    for (int i = 0; i < changedFeatures[FCName].Action.Delete.Count; i++)
                    {
                        oid = Convert.ToInt32(changedFeatures[FCName].Action.Delete[i].OID);

                        _logger.Debug("Change [ " + FCName + " ] Diff [ " + Enum.GetName(typeof(esriDataChangeType), esriDataChangeType) + " ] OID [ " + oid.ToString() + " ]");
                        ChangeTable.Add(new ChangeRow(oid, esriDataChangeType));
                    }

                    this.AddToDiffDictionary(ChangeTable);
                }
                #endregion
            }
        }
        #endregion

        #region Protected Methods

        /// <summary>
        /// Adds the specified row information to the appropriate difference dictionary.
        /// </summary>
        /// <param name="table">The table.</param>
        protected void AddToDiffDictionary(ChangeTable table)
        {
            _logger.Debug(MethodBase.GetCurrentMethod().Name);

            // Determine the dictionary to add the row.
            foreach (ChangeRow ChangeRow in table)
            {
                ChangeDictionary dictionary = null;

                if (this.Inserts.DifferenceTypes.Contains(ChangeRow.DifferenceType))
                {
                    dictionary = this.Inserts;
                }
                else if (this.Updates.DifferenceTypes.Contains(ChangeRow.DifferenceType))
                {
                    dictionary = this.Updates;
                }
                else if (this.Deletes.DifferenceTypes.Contains(ChangeRow.DifferenceType))
                {
                    dictionary = this.Deletes;
                }

                // Add to the appropriate list.
                if (dictionary != null)
                {
                    if (!dictionary.ContainsKey(table.TableName))
                    {
                        dictionary.Add(table.TableName, new ChangeTable(table.TableName, table.IsFC, table.RowType) { ChangeRow });
                    }
                    else
                    {
                        if (!dictionary[table.TableName].Contains(ChangeRow.OID))
                        {
                            dictionary[table.TableName].Add(ChangeRow);
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
            _logger.Debug(MethodBase.GetCurrentMethod().Name);

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
                _logger.Error("Unable to get relationship for the requested table");
                if (table != null)
                {
                    if (table is IDataset)
                    {
                        _logger.Error("Table: " + ((IDataset)table).Name);
                    }
                }
                _logger.Error(ex.ToString());
                throw ex;
            }
            return hasRelationships;
        }

        private void KillConnections()
        {
            Marshal.FinalReleaseComObject(sourceVersion);
            this.sourceVersion = null;
            Marshal.FinalReleaseComObject(targetVersion);
            this.targetVersion = null;
            if (this.CommonAncestorVersion != null)     Marshal.FinalReleaseComObject(this.CommonAncestorVersion);
            this.CommonAncestorVersion = null;
            Marshal.FinalReleaseComObject(this.dailyTempVersion);
            this.dailyTempVersion = null;
            SDEWorkspace.Disconnect();
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }


        /// <summary>
        /// Intended to be run after all processing is complete. Rolls the versions forward by deleting the child version
        /// and renaming the temporary version to that of the child version.
        /// </summary>
        public void RollVersions()
        {
            _logger.Debug(MethodBase.GetCurrentMethod().Name);
            if (DoNotRollVersions)
            {
                _logger.Info("Not going to be Rolling Versions for [ " + this.targetVersion.VersionName + " ]");
            }

            _logger.Info("Rolling Versions for [ " + this.targetVersion.VersionName + " ]");

            try
            {
                IVersion2 versDailyChange = this.targetVersion as IVersion2;
                string versionNameToReplace = this.targetVersion.VersionName;

                KillConnections();

                //Delete daily change version.
                _logger.Info("Deleting version (" + versionNameToReplace + ").");
                // This should reconnect
                IVersionedWorkspace vWorkspace = (IVersionedWorkspace)SDEWorkspace.Workspace;
                versDailyChange = (IVersion2)vWorkspace.FindVersion(versionNameToReplace);
                versDailyChange.Delete();
                versDailyChange = null;

                //Rename the temporary version.
                IVersion2 versDailyChangeTemp = (IVersion2)vWorkspace.FindVersion(VersionTempName);
                versDailyChangeTemp.VersionName = versionNameToReplace;
                _logger.Info("  Renamed version " + VersionTempName + " to " + versionNameToReplace + ".");
            }
            catch (Exception ex)
            {
                throw new ErrorCodeException(ErrorCode.VersionReset, "Issue rolling versions forward: " + ex.Message, ex);
            }
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
                IRelationshipClass tempRelClass = fw.OpenRelationshipClass((table as IDataset).Name);
                if (tempRelClass != null)
                {
                    test = true;
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(tempRelClass);
                }
            }
            catch
            {
            }
            return test;
        }

        /// <summary>
        /// Provides overload access for customization.
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
        /// <param name="table">ITable object of the table, used to initialize the ChangeRow2.</param>
        /// <param name="diffTypes">The difference types.</param>
        protected virtual void GetDifferences(IDataChangesInfo dci, string tableName, ITable table, params esriDataChangeType[] diffTypes)
        {
            _logger.Debug(MethodBase.GetCurrentMethod().Name);

            // Iterate through all of the difference types loading those changes.
            foreach (esriDataChangeType diffType in diffTypes)
            {
                IFIDSet listIDs = dci.get_ChangedIDs(tableName, diffType);
                listIDs.Reset();
                int oid;
                listIDs.Next(out oid);
                ChangeTable ChangeTable = new ChangeTable(((IDataset)table).Name, true);
                while (oid != -1)
                {
                    _logger.Debug("Change [ " + ((IDataset)table).Name + " ] Diff [ " + Enum.GetName(typeof(esriDataChangeType), diffType) + " ] OID [ " + oid.ToString() + " ]");
                    ChangeTable.Add(new ChangeRow(oid, diffType));                                                
                    listIDs.Next(out oid);
                }
                this.AddToDiffDictionary(ChangeTable);
            }
        }

        /// <summary>
        /// Gets related objects for all OIDs belonging to the table in the dict Dictionary and then passes out the updated newRows dictionary for further processing.
        /// </summary>
        /// <param name="table">table to look for relationships on</param>
        /// <param name="dict">provides the list of ChangeRows2 that need to have thier related objects found.</param>
        /// <param name="newRows">provides the unique output dictionary to add the new objects (if any found that are unique and new).</param>
        /// <returns>newRows dictionary with any new unique values found, that were not in the source dict or the newrows dict already.</returns>
        protected virtual ChangeDictionary Related_Object_Processing_For_Table(ITable table, ChangeDictionary dict, ChangeDictionary newRows)
        {
            #region Standard Has Relationship logic.
            string tableName = ((IDataset)table).Name;
            _logger.Debug("Initializing related objects for table : " + tableName);

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
                                foreach (ChangeRow row in dict[tableName])
                                {
                                    IRow Row = dict[tableName].GetIRow(dict.GetWorkspace(ChangeWorkspace.Auto), row);
                                    if (Row == null)
                                    {
                                        _logger.Error("Error encountered while trying to get " + tableName + "   ,objectid, " + row.OID + " ,row was not found.");
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
                            _logger.Error("Unable to get relationship for the requested table");
                            if (table != null)
                            {
                                if (table is IDataset)
                                {
                                    _logger.Error("Table: " + ((IDataset)table).Name);
                                }
                            }
                            _logger.Error(ex.ToString());
                
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
                //that tests both the isOrigin and Destination for new rows to add.
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
                            bool doNotProcessisOrigin = CheckRelatedTable((ITable)rel.OriginClass);
                            bool doNotProcessDest = CheckRelatedTable((ITable)rel.DestinationClass);
                            if (!(doNotProcessDest && doNotProcessisOrigin))
                            {
                                //either the Dest or isOrigin is valid, so we will try to process the rows.
                                List<IRow> resultRows = new List<IRow>();
                                foreach (ChangeRow row in dict[table])
                                {
                                    IRow Row = dict[table].GetIRow(dict.GetWorkspace(ChangeWorkspace.Auto), row);
                                    if (Row == null)
                                    {
                                        _logger.Error("Error encountered while trying to get " + tableName + "   ,objectid, " + row.OID + " ,row was not found.");
                                    }
                                    else
                                    {
                                        if (!doNotProcessisOrigin)
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
                            _logger.Error("Unexpected failure while getting related objects for Attributed Relationship Table");
                            if (tableName != null)
                            {
                                _logger.Error("Table that was being processed when error occured: " + tableName);
                            }
                            _logger.Error(ex.ToString());
                        }
                    }
                    while (Marshal.ReleaseComObject(rel) > 0) { }
                }//end if count > 0
            }//end if IsAttributedRelationshipClass
            #endregion
            return newRows;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Produces a dictionary wiith new rows based on a list of IRows.
        /// </summary>
        /// <param name="resultRows">List of IRows to analyze and build off of</param>
        /// <param name="row">the source related object</param>
        /// <param name="dict">isOriginal Dictionary to look in.</param>
        /// <param name="newRows">Dictionary the hold the unique results</param>
        /// <returns>newRows dictionary + the results from comparison</returns>
        private ChangeDictionary ProcessRelResultRows(List<IRow> resultRows, ChangeRow row, ChangeDictionary dict, ChangeDictionary newRows)
        {
            foreach (IRow tempRow in resultRows)
            {
                ChangeRow ChangeRow = new ChangeRow(tempRow.OID, row.DifferenceType);
                string tabName = ((IDataset)tempRow.Table).Name;
                if (dict.ContainsKey(tabName))
                {
                    if (!dict[tabName].Contains(ChangeRow))
                    {
                        if (newRows.ContainsKey(tabName))
                        {
                            if (!newRows[tabName].Contains(ChangeRow))
                            {
                                newRows[tabName].Add(ChangeRow);
                            }
                        }
                        else
                        {
                            newRows.Add(tabName, new ChangeTable(((IDataset)tempRow.Table).Name, true) { ChangeRow });
                        }
                    }
                }
                else
                {
                    if (newRows.ContainsKey(tabName))
                    {
                        if (!newRows[tabName].Contains(ChangeRow))
                        {
                            newRows[tabName].Add(ChangeRow);
                        }
                    }
                    else
                    {
                        newRows.Add(tabName, new ChangeTable(((IDataset)tempRow.Table).Name, true) { ChangeRow });
                    }
                }
            }
            return newRows;
        }

        #endregion
    }

}
