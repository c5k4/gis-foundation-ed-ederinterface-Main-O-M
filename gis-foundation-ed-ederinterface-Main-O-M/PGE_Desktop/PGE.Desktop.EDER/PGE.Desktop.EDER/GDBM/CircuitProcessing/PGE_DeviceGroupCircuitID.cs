using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Miner.Process.GeodatabaseManager.ActionHandlers;
using ESRI.ArcGIS.Geodatabase;
using Telvent.Delivery.Framework.FeederManager;
using Miner.Framework.Trace.Strategies;
using Miner.Geodatabase;
using ESRI.ArcGIS.esriSystem;
using System.Runtime.InteropServices;
using Miner.Interop;
using ESRI.ArcGIS.NetworkAnalysis;
using Telvent.Delivery.Geodatabase.ChangeDetection;
using ESRI.ArcGIS.GeoDatabaseDistributed;

namespace Telvent.PGE.ED.Desktop.GDBM
{
    /// <summary>
    /// This Action handler is responsible for updating source side device information for any features that have been touched by
    /// feeder manager 2.0.  This will ensure that the sde.default version will always have the correct source side device value.
    /// </summary>
    public class PGE_DeviceGroupCircuitID : PxActionHandler
    {
        public PGE_DeviceGroupCircuitID()
        {
            this.Name = "PGE DeviceGroup CircuitID Updater";
            this.Description = "Updates the device group circuit ID values for any features the may have been updated by feeder manager 2.0";
        }

        protected override bool PxSubExecute(Miner.Process.GeodatabaseManager.PxActionData actionData, Miner.Geodatabase.GeodatabaseManager.Serialization.GdbmParameter[] parameters)
        {
#if DEBUG
            System.Diagnostics.Debugger.Launch();
#endif
            bool success = false;
            bool inEditOperation = false;
            IWorkspace Editor = (IWorkspace)actionData.Version;
            IWorkspaceEdit wkspcEdit = (IWorkspaceEdit)actionData.Version;

            Type auType = Type.GetTypeFromProgID("mmGeodatabase.MMAutoUpdater");
            object auObj = Activator.CreateInstance(auType);
            IMMAutoUpdater immAutoupdater = auObj as IMMAutoUpdater;
            mmAutoUpdaterMode currentAUMode = immAutoupdater.AutoUpdaterMode;
            immAutoupdater.AutoUpdaterMode = mmAutoUpdaterMode.mmAUMNoEvents;

            IEnumFeatureClass deviceGroups = null;
            IFeatureClass deviceGroupFeatureClass = null;
            IFeatureClass featureClass = null;
            DifferenceManager differenceManager = null;
            DifferenceDictionary deletesDictionary = null;
            DifferenceDictionary insertsDictionary = null;
            DifferenceDictionary updatedDictionary = null;
            try
            {
                if (wkspcEdit.IsBeingEdited())
                {
                    this.Log.Info(ServiceConfiguration.Name, "Beginning device group circuit ID processing");
                    this.Log.Info(ServiceConfiguration.Name, "Determining edited features");
                    Dictionary<int, List<int>> editedOIDsByClassID = FeederManager2.GetEditedFeatures((IFeatureWorkspace)actionData.Version, actionData.VersionName);
                    IVersionedWorkspace versionWS = actionData.Version as IVersionedWorkspace;

                    differenceManager = new DifferenceManager(versionWS.DefaultVersion, actionData.Version);
                    differenceManager.Initialize();
                    differenceManager.LoadDifferences(false, esriDataChangeType.esriDataChangeTypeInsert, esriDataChangeType.esriDataChangeTypeUpdate, esriDataChangeType.esriDataChangeTypeDelete);
                    deletesDictionary = differenceManager.Deletes;
                    insertsDictionary = differenceManager.Inserts;
                    updatedDictionary = differenceManager.Updates;

                    List<string> deviceGroupGUIDsToProcess = new List<string>();
                    GetAffectedDeviceGroups(actionData.Version as IWorkspace, insertsDictionary, null, "Inserts", ref deviceGroupGUIDsToProcess);
                    GetAffectedDeviceGroups(actionData.Version as IWorkspace, updatedDictionary, null, "Updates", ref deviceGroupGUIDsToProcess);
                    GetAffectedDeviceGroups(actionData.Version as IWorkspace, null, editedOIDsByClassID, "Feeder Manager Edits", ref deviceGroupGUIDsToProcess);
                    GetAffectedDeviceGroups(differenceManager.CommonAncestorVersion as IWorkspace, deletesDictionary, null, "Deletes", ref deviceGroupGUIDsToProcess);
                    //Now that we have our list of class IDs and object IDs that were edited by feeder manager 2.0 we need to update their source side
                    //device IDs.

                    //Now that we have our feature classes that we care about we need to figure out the device groups affected
                    //and map the circuit IDs associated with them.
                    Dictionary<string, SortedDictionary<string, int>> deviceGroupToCircuitIDMap = GetDeviceGroupCircuitIDs(actionData.Version as IWorkspace, deviceGroupGUIDsToProcess);

                    deviceGroups = ModelNameManager.Instance.FeatureClassesFromModelNameWS(actionData.Version as IWorkspace, SchemaInfo.Electric.ClassModelNames.DeviceGroup);
                    deviceGroupFeatureClass = null;
                    deviceGroups.Reset();
                    deviceGroupFeatureClass = deviceGroups.Next();

                    //Now we can process through all of our discovered device groups and update the circuit ID if necessary
                    if (deviceGroupToCircuitIDMap.Count > 0)
                    {
                        wkspcEdit.StartEditOperation();
                        inEditOperation = true;
                        this.Log.Info(ServiceConfiguration.Name, "There were " + deviceGroupToCircuitIDMap.Keys.Count + " device groups found to process");

                        IField circuitIDField = ModelNameManager.Instance.FieldFromModelName(deviceGroupFeatureClass, SchemaInfo.Electric.FieldModelNames.CircuitID);
                        int circuitIDFieldIdx = deviceGroupFeatureClass.FindField(circuitIDField.Name);
                        int globalIDFieldIdx = deviceGroupFeatureClass.FindField("GLOBALID");

                        List<string> deviceGroupGuids = deviceGroupToCircuitIDMap.Keys.ToList();

                        //Build our where in clauses to quickly select large chunks of devices for processing.
                        List<string> whereInStringList = new List<string>();
                        StringBuilder whereInStringBuilder = new StringBuilder();
                        for (int j = 0; j < deviceGroupGuids.Count; j++)
                        {
                            if (j == deviceGroupGuids.Count - 1 || (j != 0 && (j % 999) == 0)) { whereInStringBuilder.Append("'" + deviceGroupGuids[j] + "'"); }
                            else { whereInStringBuilder.Append("'" + deviceGroupGuids[j] + "',"); }

                            if ((j % 999) == 0 && j != 0)
                            {
                                whereInStringList.Add(whereInStringBuilder.ToString());
                                whereInStringBuilder = new StringBuilder();
                            }
                        }

                        if (!string.IsNullOrEmpty(whereInStringBuilder.ToString()) && !whereInStringList.Contains(whereInStringBuilder.ToString()))
                        {
                            whereInStringList.Add(whereInStringBuilder.ToString());
                        }

                        //Process through all of our where clauses
                        foreach (string whereInClause in whereInStringList)
                        {
                            IQueryFilter qf = new QueryFilterClass();
                            IFeatureCursor deviceGroupFeatures = null;
                            IFeature deviceGroup = null;
                            try
                            {
                                qf.AddField(circuitIDField.Name);
                                qf.AddField("GLOBALID");
                                qf.WhereClause = "GLOBALID IN (" + whereInClause + ")";

                                deviceGroupFeatures = deviceGroupFeatureClass.Update(qf, false);
                                deviceGroup = null;
                                while ((deviceGroup = deviceGroupFeatures.NextFeature()) != null)
                                {
                                    string globalID = deviceGroup.get_Value(globalIDFieldIdx).ToString();

                                    SortedDictionary<string, int> circuitIDCounts = deviceGroupToCircuitIDMap[globalID];
                                    int highestCount = 0;
                                    string circuitIDToUse = "";
                                    //Determine the most encountered circuit ID for this guid
                                    foreach (KeyValuePair<string, int> kvp in circuitIDCounts)
                                    {
                                        if (highestCount < kvp.Value)
                                        {
                                            highestCount = kvp.Value;
                                            circuitIDToUse = kvp.Key;
                                        }
                                    }
                                    //Now we have the circuit ID that was encountered the most. We can assign it if needed.
                                    string currentCircuitID = deviceGroup.get_Value(circuitIDFieldIdx).ToString();
                                    if (currentCircuitID != circuitIDToUse)
                                    {
                                        this.Log.Info(ServiceConfiguration.Name, "Updating device group with GlobalID: " + globalID + ": Old Circuit ID: " + currentCircuitID + " New Circuit ID: " + circuitIDToUse);

                                        deviceGroup.set_Value(circuitIDFieldIdx, circuitIDToUse);
                                        deviceGroup.Store();
                                    }
                                    if (deviceGroup != null) { while (Marshal.ReleaseComObject(deviceGroup) > 0) { } }
                                }
                            }
                            finally
                            {
                                if (qf != null) { while (Marshal.ReleaseComObject(qf) > 0) { } }
                                if (deviceGroupFeatures != null) { while (Marshal.ReleaseComObject(deviceGroupFeatures) > 0) { } }
                                if (deviceGroup != null) { while (Marshal.ReleaseComObject(deviceGroup) > 0) { } }
                            }
                        }
                        
                        wkspcEdit.StopEditOperation();
                        inEditOperation = false;
                        success = true;
                    }
                    
                    return true;
                }
                else
                {
                    this.Log.Error(ServiceConfiguration.Name, "Error during device group circuit ID processing: Version not being edited");
                }
            }
            catch (Exception ex)
            {
                this.Log.Error(ServiceConfiguration.Name, "Error during device group circuit ID processing: " + ex.Message + " StackTrace: " + ex.StackTrace);
                if (inEditOperation) { wkspcEdit.AbortEditOperation(); }
                wkspcEdit.StopEditing(false);
                wkspcEdit.StartEditing(false);
            }
            finally
            {
                immAutoupdater.AutoUpdaterMode = currentAUMode;

                if (deviceGroups != null) { while (Marshal.ReleaseComObject(deviceGroups) > 0) { } }
                if (deviceGroupFeatureClass != null) { while (Marshal.ReleaseComObject(deviceGroupFeatureClass) > 0) { } }
                if (featureClass != null) { while (Marshal.ReleaseComObject(featureClass) > 0) { } }

                differenceManager = null;
                deletesDictionary = null;
                insertsDictionary = null;
                updatedDictionary = null;

                if (success)
                {
                    this.Log.Info(ServiceConfiguration.Name, "Saving edits");
                    wkspcEdit.StopEditing(true);
                    wkspcEdit.StartEditing(false);
                    this.Log.Info(ServiceConfiguration.Name, "Device group processing finished successfully");
                }
            }

            return false;
        }

        private Dictionary<string, SortedDictionary<string, int>> GetDeviceGroupCircuitIDs(IWorkspace workspaceToSearch, 
            List<string> DeviceGroupGuids)
        {
            List<string> deviceGroupGuidsFound = new List<string>();
            DeviceGroupGuids.Distinct();
            //Determine what feature classes are cared about
            Dictionary<string, SortedDictionary<string, int>> deviceGroupToCircuitIDMap = new Dictionary<string, SortedDictionary<string, int>>();
            this.Log.Info(ServiceConfiguration.Name, "Determining circuit IDs for device groups identified for processing");
            IEnumFeatureClass deviceGroups = null;
            IEnumFeatureClass deviceGroupChildren = null;
            IFeatureClass deviceGroupFeatureClass = null;
            try
            {
                List<string> whereInStringList = new List<string>();
                StringBuilder whereInStringBuilder = new StringBuilder();
                for (int j = 0; j < DeviceGroupGuids.Count; j++)
                {
                    if (j == DeviceGroupGuids.Count - 1 || (j != 0 && (j % 999) == 0)) { whereInStringBuilder.Append("'" + DeviceGroupGuids[j] + "'"); }
                    else { whereInStringBuilder.Append("'" + DeviceGroupGuids[j] + "',"); }

                    if ((j % 999) == 0 && j != 0)
                    {
                        whereInStringList.Add(whereInStringBuilder.ToString());
                        whereInStringBuilder = new StringBuilder();
                    }
                }

                if (!string.IsNullOrEmpty(whereInStringBuilder.ToString()) && !whereInStringList.Contains(whereInStringBuilder.ToString()))
                {
                    whereInStringList.Add(whereInStringBuilder.ToString());
                }

                deviceGroupChildren = ModelNameManager.Instance.FeatureClassesFromModelNameWS(workspaceToSearch as IWorkspace, SchemaInfo.Electric.ClassModelNames.DeviceGroupChild);
                deviceGroupChildren.Reset();

                IFeatureClass deviceGroupchild = null;
                while ((deviceGroupchild = deviceGroupChildren.Next()) != null)
                {
                    //Now query this device group child for the circuit IDs for associated device group guids
                    int deviceGroupGUIDFieldIdx = deviceGroupchild.FindField("STRUCTUREGUID");

                    foreach (string whereInClause in whereInStringList)
                    {
                        IQueryFilter qf = new QueryFilterClass();
                        qf.AddField(deviceGroupchild.OIDFieldName);
                        qf.AddField("STRUCTUREGUID");
                        IFeatureCursor featCursor = null;
                        IFeature feat = null;
                        try
                        {
                            qf.WhereClause = "STRUCTUREGUID IS NOT NULL AND STRUCTUREGUID in (" + whereInClause + ")";
                            featCursor = deviceGroupchild.Search(qf, false);
                            feat = null;
                            //Now process each feature discovered and build our device group to circuit ID map
                            while ((feat = featCursor.NextFeature()) != null)
                            {
                                string deviceGroupGUID = feat.get_Value(deviceGroupGUIDFieldIdx).ToString();
                                deviceGroupGuidsFound.Add(deviceGroupGUID);
                                string[] circuitIDs = FeederManager2.GetCircuitIDs(feat);
                                foreach (string circuitID in circuitIDs)
                                {
                                    try
                                    {
                                        deviceGroupToCircuitIDMap[deviceGroupGUID][circuitID] += 1;
                                    }
                                    catch
                                    {
                                        //Usually Faster to let it fail the first time it hits a new guid and add it here.
                                        if (!deviceGroupToCircuitIDMap.ContainsKey(deviceGroupGUID))
                                        {
                                            deviceGroupToCircuitIDMap.Add(deviceGroupGUID, new SortedDictionary<string, int>());
                                            deviceGroupToCircuitIDMap[deviceGroupGUID].Add(circuitID, 1);
                                        }
                                        else if (!deviceGroupToCircuitIDMap[deviceGroupGUID].ContainsKey(circuitID))
                                        {
                                            deviceGroupToCircuitIDMap[deviceGroupGUID].Add(circuitID, 1);
                                        }
                                    }
                                }
                                if (feat != null) { while (Marshal.ReleaseComObject(feat) > 0) { } }
                            }
                        }
                        finally
                        {
                            if (qf != null) { while (Marshal.ReleaseComObject(qf) > 0) { } }
                            if (featCursor != null) { while (Marshal.ReleaseComObject(featCursor) > 0) { } }
                            if (feat != null) { while (Marshal.ReleaseComObject(feat) > 0) { } }
                        }
                    }
                }

                deviceGroupGuidsFound.Distinct();
                foreach (string guidFound in deviceGroupGuidsFound)
                {
                    DeviceGroupGuids.Remove(guidFound);
                }

                //Bug#23068
                //Subhankar - GetAffectedDeviceGroups method on Inserted Device Group, collectes GUID twice for a newly added device group. 
                //            Hence "" circuit id gets asigned into circuit id field instead of actual circuit id. 
                //            Workaround - add 0 insted of increasing the value in each iteration.

                //Now our devicegroupguids have a list of all found items.  We need to add them to our list with a null circuitid
                foreach (string notFoundGuid in DeviceGroupGuids)
                {
                    try
                    {
                        deviceGroupToCircuitIDMap[notFoundGuid][""] += 0;
                    }
                    catch
                    {
                        //Usually Faster to let it fail the first time it hits a new guid and add it here.
                        if (!deviceGroupToCircuitIDMap.ContainsKey(notFoundGuid))
                        {
                            deviceGroupToCircuitIDMap.Add(notFoundGuid, new SortedDictionary<string, int>());
                            deviceGroupToCircuitIDMap[notFoundGuid].Add("", 0);
                        }
                        else if (!deviceGroupToCircuitIDMap[notFoundGuid].ContainsKey(""))
                        {
                            deviceGroupToCircuitIDMap[notFoundGuid].Add("", 0);
                        }
                    }
                }
            }
            finally
            {
                if (deviceGroups != null) { while (Marshal.ReleaseComObject(deviceGroups) > 0) { } }
                if (deviceGroupChildren != null) { while (Marshal.ReleaseComObject(deviceGroupChildren) > 0) { } }
                if (deviceGroupFeatureClass != null) { while (Marshal.ReleaseComObject(deviceGroupFeatureClass) > 0) { } }
            }
            return deviceGroupToCircuitIDMap;
        }

        private void GetAffectedDeviceGroups(IWorkspace workspaceToSearch, DifferenceDictionary differenceDictionary, 
            Dictionary<int, List<int>> ClassIDToOIDMap, string UpdateType, ref List<string> deviceGroupGUIDsToProcess)
        {
            //Determine what feature classes are cared about
            this.Log.Info(ServiceConfiguration.Name, "Determining device groups that need to be processed for " + UpdateType);
            IEnumFeatureClass deviceGroups = null;
            IEnumFeatureClass deviceGroupChildren = null;
            IFeatureClass deviceGroupFeatureClass = null;
            try
            {
                Dictionary<string, List<int>> objectsToQuery = new Dictionary<string, List<int>>();
                Dictionary<string, IFeatureClass> validClasses = new Dictionary<string, IFeatureClass>();
                deviceGroups = ModelNameManager.Instance.FeatureClassesFromModelNameWS(workspaceToSearch as IWorkspace, SchemaInfo.Electric.ClassModelNames.DeviceGroup);
                deviceGroupFeatureClass = null;
                deviceGroups.Reset();
                deviceGroupFeatureClass = deviceGroups.Next();
                validClasses.Add(((IDataset)deviceGroupFeatureClass).BrowseName.ToUpper(), deviceGroupFeatureClass);

                //this.Log.Info(ServiceConfiguration.Name, "Determining device group child feature classes");
                deviceGroupChildren = ModelNameManager.Instance.FeatureClassesFromModelNameWS(workspaceToSearch as IWorkspace, SchemaInfo.Electric.ClassModelNames.DeviceGroupChild);
                deviceGroupChildren.Reset();
                
                IFeatureClass deviceGroupchild = null;
                while ((deviceGroupchild = deviceGroupChildren.Next()) != null)
                {
                    validClasses.Add(((IDataset)deviceGroupchild).BrowseName.ToUpper(), deviceGroupchild);
                }

                if (differenceDictionary != null)
                {
                    //Parse the difference dictionary and determine what OIDs we need to query for from the device groups and device
                    //group children
                    foreach (KeyValuePair<string, DiffTable> kvp in differenceDictionary)
                    {
                        string tableName = kvp.Value.TableName.ToUpper();
                        if (validClasses.ContainsKey(tableName))
                        {
                            DiffTable currentDiffTable = kvp.Value;
                            IEnumerator<DiffRow> diffRowEnumerator = currentDiffTable.GetEnumerator();
                            diffRowEnumerator.Reset();
                            while (diffRowEnumerator.MoveNext())
                            {
                                try
                                {
                                    objectsToQuery[tableName].Add(diffRowEnumerator.Current.OID);
                                }
                                catch
                                {
                                    objectsToQuery.Add(tableName, new List<int>());
                                    objectsToQuery[tableName].Add(diffRowEnumerator.Current.OID);
                                }
                            }
                        }
                    }
                }
                else
                {
                    //A dictionary of class ID to OIDs was passed in instead so we'll use that
                    if (ClassIDToOIDMap != null) 
                    {
                        objectsToQuery = new Dictionary<string, List<int>>();
                        foreach (KeyValuePair<string, IFeatureClass> kvp in validClasses)
                        {
                            if (ClassIDToOIDMap.ContainsKey(kvp.Value.ObjectClassID))
                            {
                                objectsToQuery.Add(kvp.Key, ClassIDToOIDMap[kvp.Value.ObjectClassID]);
                            }
                            else
                            {
                                objectsToQuery.Add(kvp.Key, new List<int>());
                            }
                        }
                    }
                    else { objectsToQuery = new Dictionary<string, List<int>>(); }
                }

                foreach (KeyValuePair<string, List<int>> kvp in objectsToQuery)
                {
                    //First build our where in clauses for the object ids that were shown as changed
                    IFeatureClass featClassToSearch = validClasses[kvp.Key];
                    List<int> objectIDs = kvp.Value;
                    List<string> whereInStringList = new List<string>();
                    StringBuilder whereInStringBuilder = new StringBuilder();
                    for (int j = 0; j < objectIDs.Count; j++)
                    {
                        if (j == objectIDs.Count - 1 || (j != 0 && (j % 999) == 0)) { whereInStringBuilder.Append(objectIDs[j]); }
                        else { whereInStringBuilder.Append(objectIDs[j] + ","); }

                        if ((j % 999) == 0 && j != 0)
                        {
                            whereInStringList.Add(whereInStringBuilder.ToString());
                            whereInStringBuilder = new StringBuilder();
                        }
                    }

                    if (!string.IsNullOrEmpty(whereInStringBuilder.ToString()) && !whereInStringList.Contains(whereInStringBuilder.ToString()))
                    {
                        whereInStringList.Add(whereInStringBuilder.ToString());
                    }

                    int deviceGroupGUIDFieldIdx = -1;
                    if (featClassToSearch.ObjectClassID == deviceGroupFeatureClass.ObjectClassID)
                    {
                        deviceGroupGUIDFieldIdx = featClassToSearch.FindField("GLOBALID");
                    }
                    else
                    {
                        deviceGroupGUIDFieldIdx = featClassToSearch.FindField("STRUCTUREGUID");
                    }

                    foreach (string whereInClause in whereInStringList)
                    {
                        IQueryFilter qf = new QueryFilterClass();
                        qf.AddField(featClassToSearch.OIDFieldName);
                        IFeatureCursor featCursor = null;
                        IFeature feat = null;
                        try
                        {
                            qf.WhereClause = featClassToSearch.OIDFieldName + " in (" + whereInClause + ")";
                            featCursor = featClassToSearch.Search(qf, false);
                            feat = null;
                            //Now process each feature discovered and build our device group to circuit ID map
                            while ((feat = featCursor.NextFeature()) != null)
                            {
                                object guidObj = feat.get_Value(deviceGroupGUIDFieldIdx);
                                if (guidObj != null)
                                {
                                    string deviceGroupGUID = feat.get_Value(deviceGroupGUIDFieldIdx).ToString();
                                    deviceGroupGUIDsToProcess.Add(deviceGroupGUID);
                                }

                                if (feat != null) { while (Marshal.ReleaseComObject(feat) > 0) { } }
                            }
                        }
                        finally
                        {
                            if (qf != null) { while (Marshal.ReleaseComObject(qf) > 0) { } }
                            if (featCursor != null) { while (Marshal.ReleaseComObject(featCursor) > 0) { } }
                            if (feat != null) { while (Marshal.ReleaseComObject(feat) > 0) { } }
                        }
                    }
                }
            }
            finally
            {
                if (deviceGroups != null) { while (Marshal.ReleaseComObject(deviceGroups) > 0) { } }
                if (deviceGroupChildren != null) { while (Marshal.ReleaseComObject(deviceGroupChildren) > 0) { } }
                if (deviceGroupFeatureClass != null) { while (Marshal.ReleaseComObject(deviceGroupFeatureClass) > 0) { } }
            }
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
