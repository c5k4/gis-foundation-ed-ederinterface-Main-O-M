using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.GeoDatabaseDistributed;
using ESRI.ArcGIS.NetworkAnalysis;
using Miner.Geodatabase;
using Miner.Interop;
using Miner.Process.GeodatabaseManager.ActionHandlers;
using Telvent.Delivery.Diagnostics;
using Telvent.Delivery.Framework.FeederManager;
using Telvent.Delivery.Geodatabase.ChangeDetection;
using System.Runtime.InteropServices;

namespace Telvent.PGE.ED.Desktop.GDBM.CircuitProcessing
{
    public class PGE_CircuitColor : PxActionHandler
    {
        private const int BatchSize = 1000;

        public PGE_CircuitColor()
        {
            this.Name = "PGE Circuit Color and Feeder Type Updater";
            this.Description = "Updates the circuit colors for any features with the PGE_CircuitColor class and field model names assigned. This relies on the Circuit Source table as the source" + 
                " for the current circuit color. Also updates the Feeder Type field for feature classes with the PGE_FeederType class and field model names assigned based on ";
        }

        protected override bool PxSubExecute(Miner.Process.GeodatabaseManager.PxActionData actionData, Miner.Geodatabase.GeodatabaseManager.Serialization.GdbmParameter[] parameters)
        {
            return ExecuteActionHandler(actionData.Version);
        }

        public bool ExecuteActionHandler(IVersion version)
        {
            bool inEditOperation = false;
            IWorkspaceEdit wkspcEdit = (IWorkspaceEdit)version;

            Type auType = Type.GetTypeFromProgID("mmGeodatabase.MMAutoUpdater");
            object auObj = Activator.CreateInstance(auType);
            IMMAutoUpdater immAutoupdater = auObj as IMMAutoUpdater;
            mmAutoUpdaterMode currentAUMode = immAutoupdater.AutoUpdaterMode;
            immAutoupdater.AutoUpdaterMode = mmAutoUpdaterMode.mmAUMNoEvents;
            ISet totalObjectClasses = new Set();

            try
            {
#if DEBUG
                System.Diagnostics.Debugger.Launch();
#endif
                if (wkspcEdit.IsBeingEdited())
                {
                    Dictionary<int, List<int>> classIDToOIDMap = GetEditedOIDsByClassID(wkspcEdit as IWorkspace, version);
                    wkspcEdit.StartEditOperation();
                    inEditOperation = true;

                    Log.Info(ServiceConfiguration.Name, "Retrieving circuit source data");
                    Dictionary<string, CircuitSourceData> circuitDataMap = GetCircuitSourceData(wkspcEdit as IWorkspace);

                    IMMEnumObjectClass circuitColorObjectClasses = ModelNameManager.Instance.ObjectClassesFromModelNameWS(wkspcEdit as IWorkspace, SchemaInfo.Electric.ClassModelNames.PGECircuitColor);
                    IMMEnumObjectClass feederTypeObjectClasses = ModelNameManager.Instance.ObjectClassesFromModelNameWS(wkspcEdit as IWorkspace, SchemaInfo.Electric.ClassModelNames.PGEFeederType);
                    circuitColorObjectClasses.Reset();
                    feederTypeObjectClasses.Reset();

                    for (IObjectClass circuitColorOc = circuitColorObjectClasses.Next(); circuitColorOc != null; circuitColorOc = circuitColorObjectClasses.Next())
                    { totalObjectClasses.Add(circuitColorOc); }

                    for (IObjectClass feederTypeOc = feederTypeObjectClasses.Next(); feederTypeOc != null; feederTypeOc = feederTypeObjectClasses.Next())
                    { totalObjectClasses.Add(feederTypeOc); }

                    totalObjectClasses.Reset();
                    for (IObjectClass oc = (IObjectClass)totalObjectClasses.Next(); oc != null; oc = (IObjectClass)totalObjectClasses.Next())
                    {
                        if (!classIDToOIDMap.ContainsKey(oc.ObjectClassID)) continue;

                        try
                        {
                            string[] oids = classIDToOIDMap[oc.ObjectClassID].Distinct().Select(x => x.ToString()).ToArray();
                            Log.Info(ServiceConfiguration.Name, "There are " + oids.Length + " to be checked for circuit color on " + ((IDataset)oc).BrowseName);

                            //Build our where in clauses to quickly select large chunks of features for processing
                            List<string> whereInStringList = new List<string>();
                            for (int i = 0; i < oids.Length; i += BatchSize)
                            {
                                whereInStringList.Add(String.Join(",", oids, i,
                                    i + BatchSize <= oids.Length ? BatchSize : oids.Length - i));
                            }

                            PerformFieldUpdates(whereInStringList, oc, circuitDataMap);
                        }
                        catch
                        {
                            Log.Info(ServiceConfiguration.Name, "Failed to update " + ((IDataset)oc).BrowseName + ". Moving to next object class.");
                        }
                    }

                    wkspcEdit.StopEditOperation();
                    return true;
                }
            }
            catch (Exception ex)
            {
                Log.Error(ServiceConfiguration.Name, "Error executing PGE Circuit Color or Feeder Type Updater: " + ex.Message + " Stacktrace: " + ex.StackTrace);

                if (inEditOperation)
                {
                    wkspcEdit.AbortEditOperation();
                }

                if (totalObjectClasses != null && totalObjectClasses.Count > 0)
                {
                    totalObjectClasses.Reset();
                    IObjectClass objClass = null;
                    while ((objClass = totalObjectClasses.Next() as IObjectClass) != null)
                    {
                        while (Marshal.ReleaseComObject(objClass) > 0) { }
                    }
                }

                return false;
            }
            finally
            {
                immAutoupdater.AutoUpdaterMode = currentAUMode;
            }

            return false;
        }

        private void PerformFieldUpdates(List<string> whereInStringList, IObjectClass oc, IDictionary<string, CircuitSourceData> circuitDataMap)
        {
            IQueryFilter qf = new QueryFilterClass();
            qf.AddField(oc.OIDFieldName);

            int circuitColorIdx = -1;
            if (ModelNameManager.Instance.ContainsClassModelName(oc, SchemaInfo.Electric.ClassModelNames.PGECircuitColor))
            {
                IField circuitColorField = ModelNameManager.Instance.FieldFromModelName(oc, SchemaInfo.Electric.FieldModelNames.PGECircuitColor);
                if (circuitColorField != null)
                {
                    circuitColorIdx = oc.FindField(circuitColorField.Name);
                    qf.AddField(circuitColorField.Name);
                    Log.Info(ServiceConfiguration.Name, "Preparing to update circuit color on " + ((IDataset)oc).BrowseName);
                }
            }

            int feederTypeIdx = -1;
            if (ModelNameManager.Instance.ContainsClassModelName(oc, SchemaInfo.Electric.ClassModelNames.PGEFeederType))
            {
                IField feederTypeField = ModelNameManager.Instance.FieldFromModelName(oc, SchemaInfo.Electric.FieldModelNames.PGEFeederType);
                if (feederTypeField != null)
                {
                    feederTypeIdx = oc.FindField(feederTypeField.Name);
                    qf.AddField(feederTypeField.Name);
                    Log.Info(ServiceConfiguration.Name, "Preparing to update feeder type on " + ((IDataset)oc).BrowseName);
                }
            }

            ICursor cursor = null;
            IRow row = null;
            int colorCounter = 0, feederTypeCounter = 0;
            foreach (string whereInClause in whereInStringList)
            {
                qf.WhereClause = oc.OIDFieldName + " in (" + whereInClause + ")";
                cursor = ((ITable)oc).Update(qf, false);
                while ((row = cursor.NextRow()) != null)
                {
                    colorCounter = SetFieldOnFeature(circuitColorIdx, row, circuitDataMap, v => v.CircuitColor) ? colorCounter + 1 : colorCounter;
                    feederTypeCounter = SetFieldOnFeature(feederTypeIdx, row, circuitDataMap, v => v.FeederType) ? feederTypeCounter + 1 : feederTypeCounter;
                    if (row != null) { while (Marshal.ReleaseComObject(row) > 0) { } }
                }
                if (cursor != null) { while (Marshal.ReleaseComObject(cursor) > 0) { } }
            }

            Log.Info(ServiceConfiguration.Name, colorCounter + " had their circuit color updated for " + ((IDataset)oc).BrowseName);
            Log.Info(ServiceConfiguration.Name, feederTypeCounter + " had their feeder type updated for " + ((IDataset)oc).BrowseName);
        }

        public bool SetFieldOnFeature(int fieldIdx, IRow row, IDictionary<string, CircuitSourceData> circuitDataMap, Func<CircuitSourceData, string> GetFieldVal)
        {
            bool success = false;

            if (fieldIdx != -1)
            {
                string[] circuitIDs = FeederManager2.GetCircuitIDs(row);
                string feederIDValue = "<none>";
                string currentFieldVal = "";

                try { currentFieldVal = row.Value[fieldIdx].ToString(); }
                catch { }
                try
                {
                    string newFieldVal;
                    if (circuitIDs != null && circuitIDs.Length > 0)
                    {
                        feederIDValue = circuitIDs[0];
                        newFieldVal = GetFieldVal(circuitDataMap[feederIDValue]);
                    }
                    else newFieldVal = "";

                    if (currentFieldVal != newFieldVal)
                    {
                        row.Value[fieldIdx] = newFieldVal;
                        row.Store();
                        success = true;
                    }
                }
                catch
                {
                    this.Log.Warn(ServiceConfiguration.Name, "Unable to set circuit color for feeder: " + feederIDValue + " for OID " + row.OID);
                }
            }

            return success;
        }

        /// <summary>
        /// This method will return the list of circuit ID to circuit color from the CircuitSource table
        /// </summary>
        /// <param name="workspace">Workspace where the circuit source table resides</param>
        /// <returns></returns>
        private Dictionary<string, CircuitSourceData> GetCircuitSourceData(IWorkspace workspace)
        {
            this.Log.Info(ServiceConfiguration.Name, "Determining circuit colors for all circuit IDs from Circuit Source table");
            Dictionary<string, CircuitSourceData> circuitIDToColorMap = new Dictionary<string, CircuitSourceData>();
            //Now we need to determine if circuit source circuit color field was edited.  If so we need to trace that circuit and update those features as well.
            IEnumBSTR enumString = ModelNameManager.Instance.ClassNamesFromModelNameWS(workspace, SchemaInfo.Electric.ClassModelNames.CircuitSource);
            enumString.Reset();
            ITable circuitSourceTable = ((IFeatureWorkspace)workspace).OpenTable(enumString.Next());
            if (circuitSourceTable == null) { throw new Exception("Unable to find Circuit Source table."); }
            IQueryFilter qf = new QueryFilterClass();
            IRow circuitSourceRow = null;
            ICursor circuitSourceCursor = null;

            try
            {
                IField feederIDField = ModelNameManager.Instance.FieldFromModelName((IObjectClass)circuitSourceTable, SchemaInfo.Electric.FieldModelNames.FeederID);
                IField circuitColorField = ModelNameManager.Instance.FieldFromModelName((IObjectClass)circuitSourceTable, SchemaInfo.Electric.FieldModelNames.PGECircuitColor);
                IField feederTypeField = ModelNameManager.Instance.FieldFromModelName((IObjectClass)circuitSourceTable, SchemaInfo.Electric.FieldModelNames.PGEFeederType);

                if (feederIDField == null) { throw new Exception("Unable to find field with " + SchemaInfo.Electric.FieldModelNames.FeederID + " model name assigned on " + ((IDataset)circuitSourceTable).BrowseName); }
                if (circuitColorField == null) { throw new Exception("Unable to find field with " + SchemaInfo.Electric.FieldModelNames.PGECircuitColor + " model name assigned on " + ((IDataset)circuitSourceTable).BrowseName); }
                if (feederTypeField == null) { throw new Exception("Unable to find field with " + SchemaInfo.Electric.FieldModelNames.PGEFeederType + " model name assigned on " + ((IDataset)circuitSourceTable).BrowseName); }

                int feederIDFieldIdx = circuitSourceTable.FindField(feederIDField.Name);
                int circuitColorFieldIdx = circuitSourceTable.FindField(circuitColorField.Name);
                int feederTypeFieldIdx = circuitSourceTable.FindField(feederTypeField.Name);

                qf.AddField(feederIDField.Name);
                qf.AddField(circuitColorField.Name);
                qf.AddField(feederTypeField.Name);

                circuitSourceCursor = ((ITable)circuitSourceTable).Search(qf, false);
                while ((circuitSourceRow = circuitSourceCursor.NextRow()) != null)
                {
                    object feederIDObj = circuitSourceRow.Value[feederIDFieldIdx];
                    object circuitColorObj = circuitSourceRow.Value[circuitColorFieldIdx];
                    object feederTypeObj = circuitSourceRow.Value[feederTypeFieldIdx];

                    string feederID = "";
                    string circuitColor = "";
                    string feederType = "";
                    if (feederIDObj != null) { feederID = feederIDObj.ToString(); }
                    if (circuitColorObj != null) { circuitColor = circuitColorObj.ToString(); }
                    if (feederTypeObj != null) { feederType = feederTypeObj.ToString(); }

                    if (!circuitIDToColorMap.ContainsKey(feederID))
                    {
                        circuitIDToColorMap.Add(feederID, new CircuitSourceData(feederID, circuitColor, feederType));
                    }
                    if (circuitSourceRow != null) { while (Marshal.ReleaseComObject(circuitSourceRow) > 0) { } }
                }
            }
            finally
            {
                if (circuitSourceTable != null) { Marshal.ReleaseComObject(circuitSourceTable); }
                if (qf != null) { while (Marshal.ReleaseComObject(qf) > 0) { } }
                if (circuitSourceRow != null) { while (Marshal.ReleaseComObject(circuitSourceRow) > 0) { } }
                if (circuitSourceCursor != null) { while (Marshal.ReleaseComObject(circuitSourceCursor) > 0) { } }
            }
            return circuitIDToColorMap;
        }

        /// <summary>
        /// Return a list of all classID / OIDs that were marked as edited by feeder manager and those that would be affected by a circuit source table
        /// change
        /// </summary>
        /// <param name="workspace">Workspace being edited</param>
        /// <param name="currentVersion">Current version being edited</param>
        /// <returns></returns>
        private Dictionary<int, List<int>> GetEditedOIDsByClassID(IWorkspace workspace, IVersion currentVersion)
        {
            //Get those marked by feeder manager first
            this.Log.Info(ServiceConfiguration.Name, "Determining edited features");
            Dictionary<int, List<int>> editedOIDsByClassID = new Dictionary<int, List<int>>();
            Dictionary<string, IRow> UpdatedCircuits = new Dictionary<string, IRow>();

            this.Log.Info(ServiceConfiguration.Name, "Checking for updates to the circuit source table");
            //Now we need to determine if circuit source circuit color field was edited.  If so we need to trace that circuit and update those features as well.

            IEnumBSTR enumString = ModelNameManager.Instance.ClassNamesFromModelNameWS(workspace, SchemaInfo.Electric.ClassModelNames.CircuitSource);
            enumString.Reset();
            ITable circuitSourceTable = ((IFeatureWorkspace)workspace).OpenTable(enumString.Next());
            //IObjectClass circuitSourceTable = Miner.Geodatabase.ModelNameManager.Instance.TablesFromModelNameWS(workspace, SchemaInfo.Electric.ClassModelNames.CircuitSource);
            if (circuitSourceTable == null) { throw new Exception("Unable to find Circuit Source table."); }

            DifferenceManager differenceManager;
            DifferenceDictionary updatedDictionary;
            DifferenceDictionary insertedDictionary;

            try
            {
                IVersionedWorkspace versionWS = workspace as IVersionedWorkspace;
                differenceManager = new DifferenceManager(versionWS.DefaultVersion, currentVersion);
                differenceManager.Initialize();
                differenceManager.LoadDifferences(false,
                    new[] {esriDataChangeType.esriDataChangeTypeUpdate, esriDataChangeType.esriDataChangeTypeInsert});
                updatedDictionary = differenceManager.Updates;
                insertedDictionary = differenceManager.Inserts;

                if (insertedDictionary != null)
                {
                    //Parse the difference dictionary and determine what OIDs we need to query for from the device groups and device
                    //group children
                    foreach (KeyValuePair<string, DiffTable> kvp in insertedDictionary)
                    {
                        int currentClassID = -1;
                        DiffTable currentDiffTable = kvp.Value;
                        IEnumerator<DiffRow> diffRowEnumerator = currentDiffTable.GetEnumerator();
                        diffRowEnumerator.Reset();
                        while (diffRowEnumerator.MoveNext())
                        {
                            try
                            {
                                IRow diffRow = diffRowEnumerator.Current.GetIRow(workspace, kvp.Value);

                                if (currentClassID < 0)
                                {
                                    currentClassID = ((IObjectClass)diffRow.Table).ObjectClassID;
                                    if (diffRow != null) { while (Marshal.ReleaseComObject(diffRow) > 0);}

                                    if (currentClassID < 0)
                                    {
                                        this.Log.Info(ServiceConfiguration.Name, "Could not determine class ID for: " + kvp.Value.TableName +
                                                      ". Skipping and continuing on. This message is safe to ignore if being called for a relationship class.");
                                        continue;
                                    }
                                }
                                editedOIDsByClassID[currentClassID].Add(diffRowEnumerator.Current.OID);
                            }
                            catch
                            {
                                editedOIDsByClassID.Add(currentClassID, new List<int>());
                                editedOIDsByClassID[currentClassID].Add(diffRowEnumerator.Current.OID);
                            }
                        }
                    }
                }

                if (updatedDictionary != null)
                {
                    string circuitSourceTableName = ((IDataset)circuitSourceTable).BrowseName;
                    //Parse the difference dictionary and determine what OIDs we need to query for from the device groups and device
                    //group children
                    foreach (KeyValuePair<string, DiffTable> kvp in updatedDictionary)
                    {
                        int currentClassID = -1;
                        DiffTable currentDiffTable = kvp.Value;
                        IEnumerator<DiffRow> diffRowEnumerator = currentDiffTable.GetEnumerator();
                        diffRowEnumerator.Reset();
                        while (diffRowEnumerator.MoveNext())
                        {
                            if (kvp.Value.TableName == circuitSourceTableName)
                            {
                                IRow diffCircuitSourceRow = diffRowEnumerator.Current.GetIRow(workspace, kvp.Value);
                                string[] circuitIDs = FeederManager2.GetCircuitIDs(diffCircuitSourceRow);
                                if (circuitIDs.Length > 0)
                                {
                                    UpdatedCircuits.Add(circuitIDs[0], diffCircuitSourceRow);
                                }
                            }
                            else
                            {
                                try
                                {
                                    if (currentClassID < 0)
                                    {
                                        IRow diffRow = diffRowEnumerator.Current.GetIRow(workspace, kvp.Value);
                                        currentClassID = ((IObjectClass)diffRow.Table).ObjectClassID;
                                        if (diffRow != null) { while (Marshal.ReleaseComObject(diffRow) > 0);}
                                    }
                                    editedOIDsByClassID[currentClassID].Add(diffRowEnumerator.Current.OID);
                                }
                                catch
                                {
                                    if (currentClassID < 0)
                                    {
                                        this.Log.Info(ServiceConfiguration.Name, "Could not determine class ID for: " + kvp.Value.TableName +
                                                      ". Skipping and continuing on. This message is safe to ignore if being called for a relationship class.");
                                        continue;
                                    }
                                    editedOIDsByClassID.Add(currentClassID, new List<int>());
                                    editedOIDsByClassID[currentClassID].Add(diffRowEnumerator.Current.OID);
                                }
                            }

                        }
                    }
                }

                if (UpdatedCircuits.Count > 0)
                {
                    this.Log.Info(ServiceConfiguration.Name, "There were " + UpdatedCircuits.Count + " circuit source entries modified");
                }
                else
                {
                    this.Log.Info(ServiceConfiguration.Name, "No circuit source entries were modified");
                }

                //Now we need to perform a trace of the circuits that had their color updated to add their class ID / OIDs to our list of features to update.
                foreach (KeyValuePair<string, IRow> kvp in UpdatedCircuits)
                {
                    IGeometricNetwork geomNetwork = GetRelatedGeometricNetwork(workspace as IFeatureWorkspace, kvp.Value);
                    Dictionary<int, List<int>> tracedFeatures = TraceForClassIDsOIDsMap(geomNetwork, kvp.Key);

                    foreach (KeyValuePair<int, List<int>> kvp2 in tracedFeatures)
                    {
                        if (!editedOIDsByClassID.ContainsKey(kvp2.Key))
                        {
                            editedOIDsByClassID.Add(kvp2.Key, kvp2.Value);
                        }
                        else
                        {
                            editedOIDsByClassID[kvp2.Key].AddRange(kvp2.Value);
                        }
                    }
                    //if (geomNetwork != null) { while (Marshal.ReleaseComObject(geomNetwork) > 0) { } }
                }
            }
            finally
            {
                foreach (KeyValuePair<string, IRow> kvp in UpdatedCircuits)
                {
                    if (kvp.Value != null) { while (Marshal.ReleaseComObject(kvp.Value) > 0) { } }
                }

                if (circuitSourceTable != null) { Marshal.ReleaseComObject(circuitSourceTable); }

                differenceManager = null;
                updatedDictionary = null;
            }
            return editedOIDsByClassID;
        }

        /// <summary>
        /// Returns the stitchpoint related to the provided circuit source row.  Should only be one and that is what is assumed here.
        /// </summary>
        /// <param name="circuitSourceRow">Circuit Source row object</param>
        /// <returns></returns>
        private IGeometricNetwork GetRelatedGeometricNetwork(IFeatureWorkspace featWorkspace, IRow circuitSourceRow)
        {
            IEnumRelationshipClass relClasses = ((IObjectClass)circuitSourceRow.Table).get_RelationshipClasses(esriRelRole.esriRelRoleDestination);
            relClasses.Reset();
            IRelationshipClass relClass = null;
            while ((relClass = relClasses.Next()) != null)
            {
                ISet relatedStitchPoint = relClass.GetObjectsRelatedToObject(circuitSourceRow as IObject);
                IObjectClass relatedClass = featWorkspace.OpenFeatureClass(((IDataset)relClass.OriginClass).BrowseName);
                if (relatedClass is INetworkClass)
                {
                    INetworkClass networkClass = relatedClass as INetworkClass;
                    return networkClass.GeometricNetwork;
                }
            }
            return null;
        }

        /// <summary>
        /// This trace method will return a list of all class ID and OIDs affected for a downstream trace of the given circuit
        /// </summary>
        /// <param name="geomNetwork">IGeometricNetwork associated with the circuit ID</param>
        /// <param name="circuitID">Circuit ID value to trace</param>
        /// <returns></returns>
        private Dictionary<int, List<int>> TraceForClassIDsOIDsMap(IGeometricNetwork geomNetwork, string circuitID)
        {
            Dictionary<int, List<int>> classIDToOIDMap = new Dictionary<int, List<int>>();
            string feederID = "";
            string message = string.Empty;
            IMMFeederExt feederExt = null;
            IMMFeederSpace feederSpace = null;
            IMMFeederSource feederSource = null;
            IMMElectricNetworkTracing downstreamTracer = null;
            IMMNetworkAnalysisExtForFramework networkAnalysisExtForFramework = null;
            IMMElectricTraceSettingsEx electricTraceSettings = null;

            Type type = Type.GetTypeFromProgID("mmFramework.MMFeederExt");
            object obj = Activator.CreateInstance(type);

            try
            {
                //Console.WriteLine("Initializing feeder space");
                //this.Log.Info(ServiceConfiguration.Name, "Initializing feeder space");

                feederExt = obj as IMMFeederExt;
                feederSpace = feederExt.get_FeederSpace(geomNetwork, false);
                if (feederSpace == null)
                {
                    throw new Exception("Unable to get feeder space information from geometric network");
                }

                //Console.WriteLine("Finsihed initializing feeder space");
                //this.Log.Info(ServiceConfiguration.Name, "Finished initializing feeder space");

                INetwork network = geomNetwork.Network;
                INetSchema networkSchema = network as INetSchema;
                INetWeight electricTraceWeight = networkSchema.get_WeightByName("MMELECTRICTRACEWEIGHT");
                downstreamTracer = new MMFeederTracerClass();

                electricTraceSettings = new MMElectricTraceSettingsClass();
                electricTraceSettings.RespectESRIBarriers = true;
                electricTraceSettings.UseFeederManagerCircuitSources = true;
                electricTraceSettings.UseFeederManagerProtectiveDevices = true;
                //electricTraceSettings.ProtectiveDeviceClassIDs = sourceSideDeviceClassIDs.ToArray();

                networkAnalysisExtForFramework = new MMNetworkAnalysisExtForFrameworkClass();
                networkAnalysisExtForFramework.DrawComplex = true;
                networkAnalysisExtForFramework.SelectionSemantics = mmESRIAnalysisOptions.mmESRIAnalysisOnAllFeatures;

                //Phases to trace.
                mmPhasesToTrace phasesToTrace = mmPhasesToTrace.mmPTT_Any;
                feederSource = feederSpace.FeederSources.get_FeederSourceByFeederID(circuitID);
                if (feederSource != null)
                {
                    feederID = feederSource.FeederID.ToString();

                    DateTime startTime = DateTime.Now;
                    IEnumNetEID junctions = null;
                    IEnumNetEID edges = null;

                    this.Log.Info(ServiceConfiguration.Name, "Tracing circuit: " + feederSource.FeederID);

                    electricTraceSettings.UseFeederManagerProtectiveDevices = true;
                    //Trace
                    downstreamTracer.TraceDownstream(geomNetwork, networkAnalysisExtForFramework, electricTraceSettings, feederSource.JunctionEID,
                                                        esriElementType.esriETJunction, phasesToTrace, out junctions, out edges);

                    //this.Log.Info(ServiceConfiguration.Name, "Finished Tracing circuit: " + feederSource.FeederID);

                    IEIDHelper eidHelper = new EIDHelperClass();
                    eidHelper.GeometricNetwork = geomNetwork;
                    eidHelper.ReturnFeatures = true;
                    eidHelper.ReturnGeometries = false;
                    IEnumEIDInfo junctionInfo = eidHelper.CreateEnumEIDInfo(junctions);
                    IEnumEIDInfo edgeInfo = eidHelper.CreateEnumEIDInfo(edges);
                    junctionInfo.Reset();
                    IEIDInfo eidInfo = null;
                    while ((eidInfo = junctionInfo.Next()) != null)
                    {
                        try
                        {
                            classIDToOIDMap[eidInfo.Feature.Class.ObjectClassID].Add(eidInfo.Feature.OID);
                        }
                        catch
                        {
                            classIDToOIDMap.Add(eidInfo.Feature.Class.ObjectClassID, new List<int>());
                            classIDToOIDMap[eidInfo.Feature.Class.ObjectClassID].Add(eidInfo.Feature.OID);
                        }
                    }
                    edgeInfo.Reset();
                    eidInfo = null;
                    while ((eidInfo = edgeInfo.Next()) != null)
                    {
                        try
                        {
                            classIDToOIDMap[eidInfo.Feature.Class.ObjectClassID].Add(eidInfo.Feature.OID);
                        }
                        catch
                        {
                            classIDToOIDMap.Add(eidInfo.Feature.Class.ObjectClassID, new List<int>());
                            classIDToOIDMap[eidInfo.Feature.Class.ObjectClassID].Add(eidInfo.Feature.OID);
                        }
                    }

                    this.Log.Info(ServiceConfiguration.Name, "Finished Processing Circuit ID: " + feederSource.FeederID);
                }
            }
            catch (Exception ex)
            {
                this.Log.Error(ServiceConfiguration.Name, "Error executing tracing. Message: " + ex.Message + " StackTrace: " + ex.StackTrace);
                throw ex;
            }
            finally
            {
                if (downstreamTracer != null) { while (Marshal.ReleaseComObject(downstreamTracer) > 0);}
                if (networkAnalysisExtForFramework != null) { while (Marshal.ReleaseComObject(networkAnalysisExtForFramework) > 0);}
                if (electricTraceSettings != null) { while (Marshal.ReleaseComObject(electricTraceSettings) > 0);}
                GC.Collect();
            }
            return classIDToOIDMap;
        }

        public override bool Enabled(Miner.Geodatabase.GeodatabaseManager.ActionType actionType, Miner.Geodatabase.GeodatabaseManager.Actions gdbmAction, Miner.Geodatabase.GeodatabaseManager.PostVersionType versionType)
        {
            if (actionType == Miner.Geodatabase.GeodatabaseManager.ActionType.Post && gdbmAction == Miner.Geodatabase.GeodatabaseManager.Actions.BeforePost)
            {
                return true;
            }

            return false;
        }
    }
}
