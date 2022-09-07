// ========================================================================
// Copyright © 2021 PGE 
// <history>
//  Update Circuit Color 
// YXA6 4/14/2021	Created
// JeeraID-> EDGISRearch-376
// </history>
// All rights reserved.
// ========================================================================
using System;
using System.Collections.Generic;
using System.Linq;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.NetworkAnalysis;
using Miner.Geodatabase;
using PGE.Common.Delivery.Framework.FeederManager;
using System.Runtime.InteropServices;
using ESRI.ArcGIS.Carto;
using Miner.Interop;
using PGE.Desktop.EDER.GDBM.CircuitProcessing;
using System.Data;
using Miner.Geodatabase.GeodatabaseManager.Logging;
using Miner.Geodatabase.GeodatabaseManager.Serialization;
using PGE_DBPasswordManagement;

namespace PGE.Desktop.EDER.GDBM
{
    public class PGE_CircuitColorAH
    {
        private INameLog Log;
        private GdbmReconcilePostService ServiceConfiguration;
        CommonFunctions cmnfuncts = new CommonFunctions();
        string strVersionName = string.Empty;
        string sSessionName = string.Empty;
        IWorkspaceEdit wkspcEdit = null;
        IMMAutoUpdater immAutoupdater =null;
       
        private const int BatchSize = 1000;
        public PGE_CircuitColorAH( INameLog iNameLog, GdbmReconcilePostService ServiceConfiguration)
        {
            // TODO: Complete member initialization
            this.Log = iNameLog;
            this.ServiceConfiguration = ServiceConfiguration;
            
        }
        
        public bool UpdateCircuitColor(IVersion pChildVersion)
        {
            bool inEditOperation = false;
            wkspcEdit = (IWorkspaceEdit)pChildVersion;
            
            //Turn Off AutoUpdator
            Type auType = Type.GetTypeFromProgID("mmGeodatabase.MMAutoUpdater");
            object auObj = Activator.CreateInstance(auType);
            immAutoupdater = auObj as IMMAutoUpdater;
            mmAutoUpdaterMode currentAUMode = immAutoupdater.AutoUpdaterMode;
            immAutoupdater.AutoUpdaterMode = mmAutoUpdaterMode.mmAUMNoEvents;
            ISet totalObjectClasses = new Set();
            bool success = false;

            try
            {
                sSessionName = cmnfuncts.GetSessionNamefromVersionName(pChildVersion);

                IEnumBSTR enumString = ModelNameManager.Instance.ClassNamesFromModelNameWS(pChildVersion as IWorkspace, SchemaInfo.Electric.ClassModelNames.CircuitSource);
                enumString.Reset();
                ITable circuitSourceTable = ((IFeatureWorkspace)pChildVersion).OpenTable(enumString.Next());
                if (circuitSourceTable == null) { throw new Exception("Unable to find Circuit Source table."); }
                string circuitSourceTableName = ((IDataset)circuitSourceTable).BrowseName;

                CommonFunctions.gConnectionstring = ReadEncryption.GetConnectionStr("EDGIS@EDER");
                strVersionName = pChildVersion.VersionName.ToString();
                string SsessionName = cmnfuncts.GetSessionNamefromVersionName(pChildVersion);
                //Start Editing
                if (!wkspcEdit.IsBeingEdited())
                {
                    wkspcEdit.StartEditing(true);
                }

                if (wkspcEdit.IsBeingEdited())
                {
                    wkspcEdit.StartEditOperation();
                    inEditOperation = true;

                    Log.Info(ServiceConfiguration.Name, "Retrieving circuit source data");
                    Dictionary<string, CircuitSourceData> circuitDataMap = GetCircuitSourceData(wkspcEdit as IWorkspace);
                    Dictionary<string, IObjectClass> OCList = new Dictionary<string, IObjectClass>();

                    
                    IMMEnumObjectClass circuitColorObjectClasses = ModelNameManager.Instance.ObjectClassesFromModelNameWS(wkspcEdit as IWorkspace, SchemaInfo.Electric.ClassModelNames.PGECircuitColor);
                    IMMEnumObjectClass feederTypeObjectClasses = ModelNameManager.Instance.ObjectClassesFromModelNameWS(wkspcEdit as IWorkspace, SchemaInfo.Electric.ClassModelNames.PGEFeederType);
                    circuitColorObjectClasses.Reset();
                    feederTypeObjectClasses.Reset();

                    for (IObjectClass circuitColorOc = circuitColorObjectClasses.Next(); circuitColorOc != null; circuitColorOc = circuitColorObjectClasses.Next())
                    {
                        if(!OCList.ContainsKey((circuitColorOc as IDataset).BrowseName.ToUpper().ToString()))  OCList.Add((circuitColorOc as IDataset).BrowseName.ToUpper().ToString(), circuitColorOc);
                        totalObjectClasses.Add(circuitColorOc); 
                    }

                    for (IObjectClass feederTypeOc = feederTypeObjectClasses.Next(); feederTypeOc != null; feederTypeOc = feederTypeObjectClasses.Next())
                    {
                        if (!OCList.ContainsKey((feederTypeOc as IDataset).BrowseName.ToUpper().ToString())) OCList.Add((feederTypeOc as IDataset).BrowseName.ToUpper().ToString(), feederTypeOc);

                        totalObjectClasses.Add(feederTypeOc); 
                    }

                    totalObjectClasses.Reset();
                    Dictionary<int, List<int>> classIDToOIDMap = GetEditedOIDsByClassID(pChildVersion,OCList);


                    for (IObjectClass oc = (IObjectClass)totalObjectClasses.Next(); oc != null; oc = (IObjectClass)totalObjectClasses.Next())
                    {
                        if (oc is IAnnoClass) continue;
                        if (!classIDToOIDMap.ContainsKey(oc.ObjectClassID)) continue;
                        try
                        {
                           
                            int[] oids = classIDToOIDMap[oc.ObjectClassID].Distinct().ToArray();
                            //this.Log.Info(ServiceConfiguration.Name.ToString(), "There are " + oids.Length + " to be checked for circuit color on " + ((IDataset)oc).BrowseName);

                            
                            Log.Info(ServiceConfiguration.Name.ToString(), "Perform Update on  " + ((IDataset)oc).BrowseName + " with total no of Update- " + oids.Length.ToString());
                            if(oids.Length>0)
                            {
                                PerformFieldUpdatesThroughOIDs(oids, oc, circuitDataMap);
                            }
                        }
                        catch(Exception ex)
                        {

                            this.Log.Error(ServiceConfiguration.Name.ToString(),"Failed to update " + ((IDataset)oc).BrowseName + ". Moving to next object class.");
                            this.Log.Error(ServiceConfiguration.Name.ToString(), ex.ToString());
                        }
                        finally
                        {
                            //if (classIDToOIDMap != null) classIDToOIDMap.Clear();
                            GC.Collect();
                        }
                    }

                    wkspcEdit.StopEditOperation();
                    success = true;
                }
            }
            catch (Exception ex)
            {
               this.Log.Error(ServiceConfiguration.Name.ToString(), "Error executing PGE Circuit Color or Feeder Type Updater: " + ex.Message + " Stacktrace: " + ex.StackTrace);

                if (inEditOperation)
                {
                    wkspcEdit.AbortEditOperation();
                    //if (wkspcEdit.IsBeingEdited())
                    //{
                    //    wkspcEdit.StopEditing(false);
                    //    wkspcEdit = null;

                    //}
                   
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

                success = false;
            }
            finally
            {
                immAutoupdater.AutoUpdaterMode = currentAUMode;
                //if (wkspcEdit != null)
                //{
                //    if (wkspcEdit.IsBeingEdited())
                //    {
                //        wkspcEdit.StopEditing(true);
                //        wkspcEdit = null;

                //    }
                //}
            }
            return success;
        }

        private void PerformFieldUpdatesThroughOIDs(int[] oids, IObjectClass oc, Dictionary<string, CircuitSourceData> circuitDataMap)
        {
            try
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
                        Log.Info(ServiceConfiguration.Name.ToString(), "Preparing to update circuit color on " + ((IDataset)oc).BrowseName);
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
                        Log.Info(ServiceConfiguration.Name.ToString(), "Preparing to update feeder type on " + ((IDataset)oc).BrowseName);
                    }
                }

                //S2NN: BugFix for CIRCUITNAME AU
                int substation = -1;
                if (ModelNameManager.Instance.ContainsClassModelName(oc, SchemaInfo.Electric.ClassModelNames.PGEPriOHConductor) || ModelNameManager.Instance.ContainsClassModelName(oc, SchemaInfo.Electric.ClassModelNames.PGEPriUGConductor))
                {
                    IField circuitNameField = ModelNameManager.Instance.FieldFromModelName(oc, SchemaInfo.Electric.FieldModelNames.PGECircuitName);
                    if (circuitNameField != null)
                    {
                        substation = oc.FindField(circuitNameField.Name);
                        qf.AddField(circuitNameField.Name);
                        Log.Info(ServiceConfiguration.Name.ToString(), "Preparing to update feeder type on " + ((IDataset)oc).BrowseName);
                    }
                }


                ICursor cursor = null;
                IRow row = null;
                int colorCounter = 0, feederTypeCounter = 0, substationCounter = 0;
                int cnt = 0;
                cursor = (oc as ITable).GetRows(oids,false);
                row = cursor.NextRow();
                while (row != null)
                {
                    bool ccupdate = false, ccname = false, ccFtype = false;
                    cnt = cnt + 1;
                    //if (circuitColorIdx != -1) colorCounter = SetFieldOnFeature(circuitColorIdx, row, circuitDataMap, v => v.CircuitColor) ? colorCounter + 1 : colorCounter;
                    if (circuitColorIdx != -1 && SetFieldOnFeature(circuitColorIdx, row, circuitDataMap, v => v.CircuitColor))
                    {
                        colorCounter = colorCounter + 1;
                        ccupdate = true;
                    }

                    if (feederTypeIdx != -1 && SetFieldOnFeature(feederTypeIdx, row, circuitDataMap, v => v.FeederType))
                    {
                        feederTypeCounter = feederTypeCounter + 1;
                        ccFtype = true;
                    }
                    if (substation != -1 && SetFieldOnFeature(substation, row, circuitDataMap, v => v.Substation))
                    {
                        substationCounter = substationCounter + 1;
                        ccname = true;
                    }


                    //if (cnt == 500)
                    //{
                    //    cursor.Flush();
                    //    cnt = 0;
                    //}
                    if ((ccupdate) || (ccFtype) || (ccname))
                    {
                        cmnfuncts.UpdateData(row as IObject, strVersionName, sSessionName, "U", string.Empty);
                    }

                    row = cursor.NextRow();
                }
               // cursor.Flush();
                if (cursor != null) { Marshal.FinalReleaseComObject(cursor); cursor = null; }
                
                Log.Info(ServiceConfiguration.Name.ToString(), colorCounter + " had their circuit color updated for " + ((IDataset)oc).BrowseName);
                Log.Info(ServiceConfiguration.Name.ToString(), feederTypeCounter + " had their feeder type updated for " + ((IDataset)oc).BrowseName);
                Log.Info(ServiceConfiguration.Name.ToString(), substationCounter + " had their Circuit Name updated for " + ((IDataset)oc).BrowseName);//S2NN: BugFix for CIRCUITNAME AU

            }
            catch (Exception ex)
            {
                throw ex;
            }
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
                    Log.Info(ServiceConfiguration.Name.ToString(), "Preparing to update circuit color on " + ((IDataset)oc).BrowseName);
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
                    Log.Info(ServiceConfiguration.Name.ToString(), "Preparing to update feeder type on " + ((IDataset)oc).BrowseName);
                }
            }

            //S2NN: BugFix for CIRCUITNAME AU
            int substation = -1;
            if (ModelNameManager.Instance.ContainsClassModelName(oc, SchemaInfo.Electric.ClassModelNames.PGEPriOHConductor) || ModelNameManager.Instance.ContainsClassModelName(oc, SchemaInfo.Electric.ClassModelNames.PGEPriUGConductor))
            {
                IField circuitNameField = ModelNameManager.Instance.FieldFromModelName(oc, SchemaInfo.Electric.FieldModelNames.PGECircuitName);
                if (circuitNameField != null)
                {
                    substation = oc.FindField(circuitNameField.Name);
                    qf.AddField(circuitNameField.Name);
                    Log.Info(ServiceConfiguration.Name.ToString(), "Preparing to update feeder type on " + ((IDataset)oc).BrowseName);
                }
            }


            ICursor cursor = null;
            IRow row = null;
            int colorCounter = 0, feederTypeCounter = 0, substationCounter = 0;
            foreach (string whereInClause in whereInStringList)
            {
                if (string.IsNullOrEmpty(whereInClause)) continue;
                qf.WhereClause = oc.OIDFieldName + " in (" + whereInClause + ")";
                cursor = ((ITable)oc).Update(qf, false);
                int cnt = 0;
                while ((row = cursor.NextRow()) != null)
                {
                    cnt = cnt + 1;
                    if(circuitColorIdx!=-1) colorCounter = SetFieldOnFeature(circuitColorIdx, row, circuitDataMap, v => v.CircuitColor) ? colorCounter + 1 : colorCounter;

                    if (feederTypeIdx !=-1) feederTypeCounter = SetFieldOnFeature(feederTypeIdx, row, circuitDataMap, v => v.FeederType) ? feederTypeCounter + 1 : feederTypeCounter;
                    //S2NN: BugFix for CIRCUITNAME AU
                    if (ModelNameManager.Instance.ContainsClassModelName(oc, SchemaInfo.Electric.ClassModelNames.PGEPriOHConductor) ||
                        ModelNameManager.Instance.ContainsClassModelName(oc, SchemaInfo.Electric.ClassModelNames.PGEPriUGConductor))
                        substationCounter = SetFieldOnFeature(substation, row, circuitDataMap, v => v.Substation) ? substationCounter + 1 : substationCounter;
                    if (cnt==500)
                    {
                        cursor.Flush();
                        cnt = 0;
                    }
                   // if (row != null) { while (Marshal.ReleaseComObject(row) > 0) { } }
                }
                cursor.Flush();
                //if (cursor != null) { while (Marshal.ReleaseComObject(cursor) > 0) { } }
                if (cursor != null) { Marshal.FinalReleaseComObject(cursor); cursor = null; }
            }

            Log.Info(ServiceConfiguration.Name.ToString(), colorCounter + " had their circuit color updated for " + ((IDataset)oc).BrowseName);
            Log.Info(ServiceConfiguration.Name.ToString(), feederTypeCounter + " had their feeder type updated for " + ((IDataset)oc).BrowseName);
            Log.Info(ServiceConfiguration.Name.ToString(), substationCounter + " had their Circuit Name updated for " + ((IDataset)oc).BrowseName);//S2NN: BugFix for CIRCUITNAME AU
        }

        public bool SetFieldOnFeature(int fieldIdx, IRow row, IDictionary<string, CircuitSourceData> circuitDataMap, Func<CircuitSourceData, string> GetFieldVal)
        {
            


            bool success = false;
            
            if (fieldIdx != -1)
            {
                string[] circuitIDs = null;

                IAnnotationFeature annoFeat = row as IAnnotationFeature;
                //if (annoFeat != null)
                //{
                //    //int featureID = annoFeat.LinkedFeatureID;
                //    IEnumerable<IRelationshipClass> relClasses = (annoFeat as IFeature).GetRelationships(SchemaInfo.Electric.ClassModelNames.PGEFeederType)
                //        .Where(rc => (rc.OriginClass is IFeatureClass));

                //    foreach (IRelationshipClass rc in relClasses)
                //    {
                //        ESRI.ArcGIS.esriSystem.ISet relSet = rc.GetObjectsRelatedToObject(row as IObject);
                //        IFeature feat = null;
                //        while ((feat = relSet.Next() as IFeature) != null)
                //        {
                //            circuitIDs = FeederManager2.GetCircuitIDs(feat);
                //            break;
                //        }

                //        break;
                //    }
                //}
                //else
                if (row == null) return false;

                   IField circuitIDField = ModelNameManager.Instance.FieldFromModelName(row.Table as IObjectClass, "FEEDERID");
                    if (circuitIDField == null) { circuitIDField = ModelNameManager.Instance.FieldFromModelName(row.Table as IObjectClass, "PGE_CIRCUITID"); }
                if (circuitIDField == null) return false;
                    int circuitIDFieldIdx = row.Table.FindField(circuitIDField.Name);
                if (circuitIDFieldIdx != -1)
                {
                    circuitIDs = new string[1];
                    circuitIDs[0] = row.get_Value(circuitIDFieldIdx).ToString();
                }
                    //circuitIDs = FeederManager2.GetCircuitIDs(row);
                

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

                    if (!string.IsNullOrEmpty(newFieldVal) &&  currentFieldVal != newFieldVal)
                    {

                        row.Value[fieldIdx] = newFieldVal;
                        //wkspcEdit.StartEditOperation();
                        row.Store();

                        //Add Data in GDBM staging table
                        ///wkspcEdit.StopEditOperation();
                        Log.Info(ServiceConfiguration.Name.ToString(), "FeederID Value-> "  + feederIDValue + " is updated from old value -> " + currentFieldVal + " to new value - > " + newFieldVal + "  :  for OID " + row.OID);
                        success = true;
                    }
                   

                }
                catch(Exception ex)
                {
                   this.Log.Error(ServiceConfiguration.Name.ToString(), "Unable to set circuit color for feeder: " + feederIDValue + " for OID " + row.OID);
                   this.Log.Error(ServiceConfiguration.Name.ToString(), ex.Message.ToString());

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
            //this.Log.Info(ServiceConfiguration.Name.ToString(), "Determining circuit colors for all circuit IDs from Circuit Source table");
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
                int substationFieldIdx = circuitSourceTable.FindField("SUBSTATIONNAME"); //S2NN: BugFix for CIRCUITNAME AU

                qf.AddField(feederIDField.Name);
                qf.AddField(circuitColorField.Name);
                qf.AddField(feederTypeField.Name);
                qf.AddField("SUBSTATIONNAME"); //S2NN: BugFix for CIRCUITNAME AU

                circuitSourceCursor = ((ITable)circuitSourceTable).Search(qf, false);
                while ((circuitSourceRow = circuitSourceCursor.NextRow()) != null)
                {
                    object feederIDObj = circuitSourceRow.Value[feederIDFieldIdx];
                    object circuitColorObj = circuitSourceRow.Value[circuitColorFieldIdx];
                    object feederTypeObj = circuitSourceRow.Value[feederTypeFieldIdx];
                    object substationObj = circuitSourceRow.Value[substationFieldIdx];//S2NN: BugFix for CIRCUITNAME AU

                    string feederID = "";
                    string circuitColor = "";
                    string feederType = "";
                    string substation = "";//S2NN: BugFix for CIRCUITNAME AU
                    if (feederIDObj != null) { feederID = feederIDObj.ToString(); }
                    if (circuitColorObj != null) { circuitColor = circuitColorObj.ToString(); }
                    if (feederTypeObj != null) { feederType = feederTypeObj.ToString(); }
                    if (substationObj != null && feederID.Length > 4) { substation = substationObj.ToString() + " " + feederID.Substring(feederID.Length - 4); }//S2NN: BugFix for CIRCUITNAME AU

                    if (!circuitIDToColorMap.ContainsKey(feederID))
                    {
                        circuitIDToColorMap.Add(feederID, new CircuitSourceData(feederID, circuitColor, feederType, substation));
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
        private Dictionary<int, List<int>> GetEditedOIDsByClassID(IVersion currentVersion, Dictionary<string, IObjectClass> OCList)
        {
            //Get those marked by feeder manager first
            Log.Info(ServiceConfiguration.Name.ToString(),"Determining edited features");
            Dictionary<int, List<int>> editedOIDsByClassID = new Dictionary<int, List<int>>();
            Dictionary<string, IRow> UpdatedCircuits = new Dictionary<string, IRow>();

            Log.Info(ServiceConfiguration.Name.ToString(), "Checking for updates to the circuit source table");
            //Now we need to determine if circuit source circuit color field was edited.  If so we need to trace that circuit and update those features as well.
            IEnumBSTR enumString = null;
            ITable circuitSourceTable = null;

            try
            {
                enumString = ModelNameManager.Instance.ClassNamesFromModelNameWS(currentVersion as IWorkspace, SchemaInfo.Electric.ClassModelNames.CircuitSource);
                enumString.Reset();
                 circuitSourceTable = ((IFeatureWorkspace)currentVersion).OpenTable(enumString.Next());
                //IObjectClass circuitSourceTable = Miner.Geodatabase.ModelNameManager.Instance.TablesFromModelNameWS(workspace, SchemaInfo.Electric.ClassModelNames.CircuitSource);


                if (circuitSourceTable == null) { throw new Exception("Unable to find Circuit Source table."); }
                
                IVersionedWorkspace versionWS = currentVersion as IVersionedWorkspace;
                string circuitSourceTableName = ((IDataset)circuitSourceTable).BrowseName;
                int indx = circuitSourceTable.Fields.FindField("CIRCUITID");

                string selectQuery = "select  Feat_OID,Feat_classname from "  + SchemaInfo.General.pAHInfoTableName + " Where versionname ='" + currentVersion.VersionName.ToString() + "' and Action in ('I') ";
                DataTable dt = (new DBHelper()).GetDataTable(selectQuery);
               
                #region  Execute insert feature
                if (dt.Rows.Count > 0)
                {
                    //Parse the difference dictionary and determine what OIDs we need to query for from the device groups and device
                    //group children
                    foreach (DataRow DR in dt.Rows)
                    {
                        bool obtainedClassID = false;
                        int currentClassID = -1;
                        string tableName  = DR[1].ToString();
                       
                        try
                        {
                            if (currentClassID < 0)
                            {
                                if (!obtainedClassID)
                                {
                                    IObjectClass oc= null;
                                    OCList.TryGetValue(DR[1].ToString().ToUpper(), out oc);
                                    if (oc == null) continue;
                                    currentClassID =oc.ObjectClassID;
                                    obtainedClassID = true;
                                }

                                if (currentClassID < 0)
                                {
                                   // Log.Info(ServiceConfiguration.Name, "Could not determine class ID for: " + DR[1].ToString() + ". Skipping and continuing on. This message is safe to ignore if being called for a relationship class.");
                                    continue;
                                }
                            }
                            editedOIDsByClassID[currentClassID].Add(Convert.ToInt32(DR[0].ToString()));
                        }
                        catch
                        {
                            editedOIDsByClassID.Add(currentClassID, new List<int>());
                            editedOIDsByClassID[currentClassID].Add(Convert.ToInt32(DR[0].ToString()));
                        }
                        obtainedClassID = true;
                        
                    }
                }
                #endregion
                 selectQuery = "select   Feat_OID,Feat_classname from " + SchemaInfo.General.pAHInfoTableName + " Where versionname ='" + currentVersion.VersionName.ToString() + "' and Action in ('U') ";
                 dt = (new DBHelper()).GetDataTable(selectQuery);
                
                #region execute Update Feature
                if (dt.Rows.Count>0)
                {
                   
                    //Parse the difference dictionary and determine what OIDs we need to query for from the device groups and device
                    //group children
                    foreach (DataRow DR in dt.Rows)
                    {
                        bool obtainedClassID = false;                
                        int currentClassID = -1;
                        if (DR[1].ToString().ToUpper() == circuitSourceTableName)
                        {
                            IRow diffCircuitSourceRow = circuitSourceTable.GetRow(Convert.ToInt32(DR[0].ToString()));
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
                                    if (!obtainedClassID)
                                    {

                                        IObjectClass oc = null;
                                        OCList.TryGetValue(DR[1].ToString().ToUpper(), out oc);
                                        if (oc == null) continue;
                                        currentClassID = oc.ObjectClassID;
                                        obtainedClassID = true;
                                    }
                                }
                                editedOIDsByClassID[currentClassID].Add(Convert.ToInt32(DR[0].ToString()));
                            }
                            catch
                            {
                                if (currentClassID < 0)
                                {
                                    Log.Info(ServiceConfiguration.Name, "Could not determine class ID for: " + DR[1].ToString().ToUpper() + ". Skipping and continuing on. This message is safe to ignore if being called for a relationship class.");
                                    continue;
                                }
                                editedOIDsByClassID.Add(currentClassID, new List<int>());
                                editedOIDsByClassID[currentClassID].Add(Convert.ToInt32(DR[0].ToString()));
                            }
                        }
                            obtainedClassID = true;
                        
                    }
                }


                #endregion
                if (UpdatedCircuits.Count > 0)
                {
                   Log.Info(ServiceConfiguration.Name.ToString(), "There were " + UpdatedCircuits.Count + " circuit source entries modified");
                }
                else
                {
                    Log.Info(ServiceConfiguration.Name.ToString(), "No circuit source entries were modified");
                }

                //Now we need to perform a trace of the circuits that had their color updated to add their class ID / OIDs to our list of features to update.
                foreach (KeyValuePair<string, IRow> kvp in UpdatedCircuits)
                {
                    IGeometricNetwork geomNetwork = GetRelatedGeometricNetwork(currentVersion as IFeatureWorkspace, kvp.Value);
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
                if (UpdatedCircuits.Count>0) UpdatedCircuits.Clear();

              //  if (circuitSourceTable != null) { Marshal.ReleaseComObject(circuitSourceTable); }

            }
            return editedOIDsByClassID;
        }
        ///// <summary>
        ///// Return a list of all classID / OIDs that were marked as edited by feeder manager and those that would be affected by a circuit source table
        ///// change
        ///// </summary>
        ///// <param name="workspace">Workspace being edited</param>
        ///// <param name="currentVersion">Current version being edited</param>
        ///// <returns></returns>
        //private Dictionary<int, List<int>> GetEditedOIDsByClassID( IVersion currentVersion,IObjectClass oc, string circuitSourceTableName )
        //{
        //    //Get those marked by feeder manager first
        //    Dictionary<int, List<int>> editedOIDsByClassID = new Dictionary<int, List<int>>();
        //    Dictionary<string, IRow> UpdatedCircuits = new Dictionary<string, IRow>();
        //    Log.Info(ServiceConfiguration.Name.ToString(), "Determining edited features for ObjectClass--" + oc.AliasName.ToString());

        //    //Now we need to determine if circuit source circuit color field was edited.  If so we need to trace that circuit and update those features as well.


        //    IDictionary<string, ChangedFeatures> DifferenceList = null;


        //    try
        //    {
        //        IVersionedWorkspace versionWS = currentVersion as IVersionedWorkspace;

        //        CDVersionManager pobj = new PGE.Common.ChangeDetectionAPI.CDVersionManager(currentVersion as IWorkspace, "  + SchemaInfo.General.pAHInfoTableName + ");
        //        IList<string> clist = new List<string>();
        //        clist.Add(((IDataset)oc).BrowseName.ToString());
        //         DifferenceList = pobj.CDVersionDifference(changeTypes.All, listType.FeatureClass,clist
        //           , whereClause: SchemaInfo.General.FIELDS.VERSIONNAME + "='" + currentVersion.VersionName + "'");
        //        Log.Info(ServiceConfiguration.Name.ToString(), "Checking for updates to the circuit source table");
        //        foreach (object skey in DifferenceList.Keys)
        //        {

        //            Log.Info(ServiceConfiguration.Name.ToString(), "Total Insert Count For  => " + skey.ToString() + " === " + DifferenceList[skey.ToString()].Action.Insert.Count);


        //            for (int i = 0; i < DifferenceList[skey.ToString()].Action.Insert.Count; i++)

        //            {

        //                IRow prow = DifferenceList[skey.ToString()].Action.Insert[i].feature as IRow;

        //                bool obtainedClassID = false;
        //                int currentClassID = -1;

        //                try
        //                {
        //                    if (currentClassID < 0)
        //                    {
        //                        if (!obtainedClassID)
        //                        {

        //                            currentClassID = ((IObjectClass)prow.Table).ObjectClassID;
        //                            editedOIDsByClassID[currentClassID].Add(prow.OID);
        //                            obtainedClassID = true;
        //                            if (currentClassID < 0)
        //                            {
        //                                Log.Info(ServiceConfiguration.Name.ToString(), "Could not determine class ID for: " + ((IDataset)prow.Table).BrowseName.ToString() +
        //                                                  ". Skipping and continuing on. This message is safe to ignore if being called for a relationship class.");
        //                                continue;
        //                            }


        //                        }


        //                    }

        //                }
        //                catch
        //                {
        //                    editedOIDsByClassID.Add(currentClassID, new List<int>());
        //                    editedOIDsByClassID[currentClassID].Add(prow.OID);
        //                }
        //                obtainedClassID = true;
        //            }
        //        }

        //        foreach (object skey in DifferenceList.Keys)
        //        {
        //            Log.Info(ServiceConfiguration.Name.ToString(), "Total Update Count For  => " + skey.ToString() + " === " + DifferenceList[skey.ToString()].Action.Update.Count);

        //            for (int i = 0; i < DifferenceList[skey.ToString()].Action.Update.Count; i++)

        //            {

        //                IRow prow = DifferenceList[skey.ToString()].Action.Update[i].feature as IRow;


        //                //Parse the difference dictionary and determine what OIDs we need to query for from the device groups and device
        //                //group children
        //                if (skey.ToString() == circuitSourceTableName)
        //                {

        //                    UpdatedCircuits.Add(prow.get_Value(prow.Fields.FindField("CircuitID")).ToString(), prow);

        //                }
        //                else
        //                {
        //                    bool obtainedClassID = false;
        //                    int currentClassID = -1;
        //                    try
        //                    {
        //                        if (currentClassID < 0)
        //                        {

        //                            currentClassID = ((IObjectClass)prow.Table).ObjectClassID;

        //                            obtainedClassID = true;

        //                        }
        //                        try
        //                        {
        //                            editedOIDsByClassID[currentClassID].Add(prow.OID);
        //                        }
        //                        catch
        //                        {
        //                            editedOIDsByClassID.Add(currentClassID, new List<int>());
        //                            editedOIDsByClassID[currentClassID].Add(prow.OID);
        //                        }
        //                    }
        //                    catch (Exception ex)
        //                    {
        //                        if (currentClassID < 0)
        //                        {
        //                            this.Log.Error("Could not determine class ID for: " + ((IDataset)prow.Table).BrowseName.ToString() +
        //                                           ". Skipping and continuing on. This message is safe to ignore if being called for a relationship class.");
        //                            continue;
        //                        }
        //                        editedOIDsByClassID.Add(currentClassID, new List<int>());
        //                        editedOIDsByClassID[currentClassID].Add(prow.OID);
        //                    }

        //                    obtainedClassID = true;
        //                }
        //            }
        //        }


        //        if (UpdatedCircuits.Count > 0)
        //        {
        //            Log.Info(ServiceConfiguration.Name.ToString(), "There were " + UpdatedCircuits.Count + " circuit source entries modified");
        //        }
        //        else
        //        {
        //            Log.Info(ServiceConfiguration.Name.ToString(), "No circuit source entries were modified");

        //        }

        //        //Now we need to perform a trace of the circuits that had their color updated to add their class ID / OIDs to our list of features to update.
        //        foreach (KeyValuePair<string, IRow> kvp in UpdatedCircuits)
        //        {
        //            IGeometricNetwork geomNetwork = GetRelatedGeometricNetwork(currentVersion as IFeatureWorkspace, kvp.Value);
        //            Dictionary<int, List<int>> tracedFeatures = TraceForClassIDsOIDsMap(geomNetwork, kvp.Key);

        //            foreach (KeyValuePair<int, List<int>> kvp2 in tracedFeatures)
        //            {
        //                if (!editedOIDsByClassID.ContainsKey(kvp2.Key))
        //                {
        //                    editedOIDsByClassID.Add(kvp2.Key, kvp2.Value);
        //                }
        //                else
        //                {
        //                    editedOIDsByClassID[kvp2.Key].AddRange(kvp2.Value);
        //                }
        //            }
        //            //if (geomNetwork != null) { while (Marshal.ReleaseComObject(geomNetwork) > 0) { } }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Log.Info(ServiceConfiguration.Name.ToString(), "Exception occurred while getting the edited OIDs - "  + ex.ToString() );
        //        throw ex;

        //    }
        //    finally
        //    {
        //      //  if (circuitSourceTable != null) { Marshal.ReleaseComObject(circuitSourceTable); }
        //        if (DifferenceList != null)
        //        {
        //            DifferenceList.Clear();
        //        }
        //        if(UpdatedCircuits!=null)
        //        {
        //            UpdatedCircuits.Clear();
        //        }

        //    }
        //    return editedOIDsByClassID;
        //}

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
                //this.Log.Info(ServiceConfiguration.Name.ToString(), "Initializing feeder space");

                feederExt = obj as IMMFeederExt;
                feederSpace = feederExt.get_FeederSpace(geomNetwork, false);
                if (feederSpace == null)
                {
                    throw new Exception("Unable to get feeder space information from geometric network");
                }

                //Console.WriteLine("Finsihed initializing feeder space");
                //this.Log.Info(ServiceConfiguration.Name.ToString(), "Finished initializing feeder space");

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

                    //this.Log.Info(ServiceConfiguration.Name.ToString(), "Tracing circuit: " + feederSource.FeederID);

                    electricTraceSettings.UseFeederManagerProtectiveDevices = true;
                    //Trace
                    downstreamTracer.TraceDownstream(geomNetwork, networkAnalysisExtForFramework, electricTraceSettings, feederSource.JunctionEID,
                                                        esriElementType.esriETJunction, phasesToTrace, out junctions, out edges);

                    //this.Log.Info(ServiceConfiguration.Name.ToString(), "Finished Tracing circuit: " + feederSource.FeederID);

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

                    //this.Log.Info(ServiceConfiguration.Name.ToString(), "Finished Processing Circuit ID: " + feederSource.FeederID);
                }
            }
            catch (Exception ex)
            {
               this.Log.Error(ServiceConfiguration.Name.ToString(), "Error executing tracing. Message: " + ex.Message + " StackTrace: " + ex.StackTrace);
                throw ex;
            }
            finally
            {
                if (downstreamTracer != null) { while (Marshal.ReleaseComObject(downstreamTracer) > 0) ; }
                if (networkAnalysisExtForFramework != null) { while (Marshal.ReleaseComObject(networkAnalysisExtForFramework) > 0) ; }
                if (electricTraceSettings != null) { while (Marshal.ReleaseComObject(electricTraceSettings) > 0) ; }
                GC.Collect();
            }
            return classIDToOIDMap;
        }

    }

    }
