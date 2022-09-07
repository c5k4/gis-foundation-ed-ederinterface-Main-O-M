// ========================================================================
// Copyright © 2021 PGE 
// <history>
// Update Source Side Device ID
// YXA6 4/14/2021	Created
// JeeraID-> EDGISRearch-376
// </history>
// All rights reserved.
// ========================================================================
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ESRI.ArcGIS.Geodatabase;
using PGE.Common.Delivery.Framework.FeederManager;
using Miner.Geodatabase;
using ESRI.ArcGIS.esriSystem;
using System.Runtime.InteropServices;
using Miner.Interop;
using ESRI.ArcGIS.NetworkAnalysis;
using System.Reflection;
using System.Xml;
using System.IO;
using PGE.Common.Delivery.Systems.Configuration;
using Miner.Geodatabase.GeodatabaseManager.Serialization;
using Miner.Geodatabase.GeodatabaseManager.Logging;
using PGE_DBPasswordManagement;
using System.Data;

namespace PGE.Desktop.EDER.GDBM
{
    /// <summary>
    /// updating source side device information for any features that have been touched by
    /// feeder manager 2.0.  This will ensure that the sde.default version will always have the correct source side device value.
    /// </summary>
    public class PGE_SourceSideDeviceAH
    {

        private INameLog Log;
        private GdbmReconcilePostService ServiceConfiguration;
        CommonFunctions cmnfuncts = new CommonFunctions();
        string strVersionName = string.Empty;
        string sSessionName = string.Empty;
        IWorkspaceEdit wkspcEdit = null;
        private const int BatchSize = 1000;
        public PGE_SourceSideDeviceAH(Miner.Geodatabase.GeodatabaseManager.Logging.INameLog iNameLog, GdbmReconcilePostService ServiceConfiguration)
        {
            // TODO: Complete member initialization
            this.Log = iNameLog;
            this.ServiceConfiguration = ServiceConfiguration;
            LoadSSDConfiguration();

        }
       
        public bool updateSSD(IVersion pVersion)
        {
            CommonFunctions.gConnectionstring = ReadEncryption.GetConnectionStr("EDGIS@EDER");
            bool success = false;
            bool inEditOperation = false;
            IWorkspace Editor = (IWorkspace)pVersion;
            wkspcEdit = (IWorkspaceEdit)pVersion;
            string selectQuery = string.Empty;
            try
            {
              
                strVersionName = pVersion.VersionName.ToString();
                sSessionName = cmnfuncts.GetSessionNamefromVersionName(pVersion);

                wkspcEdit.StartEditing(true);
                Log.Info(ServiceConfiguration.Name.ToString(), "Getting Records from Info table");
                //CDVersionManager pobj = new PGE.Common.ChangeDetectionAPI.CDVersionManager(pVersion as IWorkspace,"  + SchemaInfo.General.pAHInfoTableName + ");
                selectQuery = "select s.Feat_OID,s.Feat_classname, g.Objectid from " + SchemaInfo.General.pAHInfoTableName + " s inner join SDE.GDB_ITEMS g "
                 + " on s.feat_classname = g.PHYSICALNAME  where((s.versionname ='" + pVersion.VersionName.ToString()
                    + "' and s.Action in ('I')) and g.type in ( '{70737809-852C-4A03-9E22-2CECEA5B9BFA}','{CD06BC3B-789D-4C51-AAFA-A467912B8965}'))";

                DataTable dt = (new DBHelper()).GetDataTable(selectQuery);


                Dictionary<int, List<int>> editedOIDsByClassID = new Dictionary<int, List<int>>();
                int currentObjectClassID = -1;

                Log.Info(ServiceConfiguration.Name.ToString(), "Process Insert Records");
                
                #region  Execute insert feature
                if (dt.Rows.Count > 0)
                {
                    //Parse the difference dictionary and determine what OIDs we need to query for from the device groups and device
                    //group children
                    foreach (DataRow DR in dt.Rows)
                    {
                        bool obtainedClassID = false;
                        int currentClassID = -1;
                        string tableName = DR[1].ToString();

                        try
                        {
                            if (currentClassID < 0)
                            {
                                if (!obtainedClassID)
                                {

                                    currentClassID = Convert.ToInt32(DR[2].ToString());
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
                #region For Insert Records
                
                // Log.Info(ServiceConfiguration.Name.ToString(), "Total Update Count For  => " + skey.ToString() + " === " + DifferenceList[skey.ToString()].Action.Update.Count);
                selectQuery = "select s.Feat_OID,s.Feat_classname, G.Objectid from " + SchemaInfo.General.pAHInfoTableName + " s inner join SDE.GDB_ITEMS G "
                 + " on s.feat_classname = g.PHYSICALNAME  where((s.versionname ='" + pVersion.VersionName.ToString()
                    + "' and s.Action in ('U')) and g.type in ( '{70737809-852C-4A03-9E22-2CECEA5B9BFA}','{CD06BC3B-789D-4C51-AAFA-A467912B8965}'))";
                dt = (new DBHelper()).GetDataTable(selectQuery);
                ////For update Records 
                Log.Info(ServiceConfiguration.Name.ToString(), "Process Update Records");
                if (dt.Rows.Count > 0)
                {

                    //Parse the difference dictionary and determine what OIDs we need to query for from the device groups and device
                    //group children
                    foreach (DataRow DR in dt.Rows)
                    {

                        currentObjectClassID = -1;

                        // changefeat.

                        if (currentObjectClassID < 0)
                        {
                            currentObjectClassID = Convert.ToInt32(DR[2].ToString());
                            if (currentObjectClassID < 0)
                            {
                                Log.Info(ServiceConfiguration.Name, "Could not determine class ID for: " + DR[1].ToString() + ". Skipping and continuing on. This message is safe to ignore if being called for a relationship class.");
                                continue;
                            }
                        }
                        try
                        {
                            editedOIDsByClassID[currentObjectClassID].Add(Convert.ToInt32(DR[0].ToString()));
                        }
                        catch
                        {
                            editedOIDsByClassID.Add(currentObjectClassID, new List<int>());
                            editedOIDsByClassID[currentObjectClassID].Add(Convert.ToInt32(DR[0].ToString()));
                        }


                    }

                }

                #endregion
                if (dt != null) { dt.Rows.Clear(); dt = null; }
                //Now that we have our list of class IDs and object IDs that were edited by feeder manager 2.0 we need to update their source side
                //device IDs.
                //ToDo
                //1) Determine affected circuits from list above.  At the same time map ClassID-OID -> Current Source Side device
                //2) Perform a downstream trace of each circuit once, and put it in order like the cached trace functionality.
                //3) Build current ClassID-OID -> Source side device map
                //4) Compare the two and determine what (if any) have changed and update those features alone.

                if (editedOIDsByClassID.Count > 0)
                {
                    wkspcEdit.StartEditOperation();
                    inEditOperation = true;
                    Log.Info(ServiceConfiguration.Name.ToString(), "Processing for source side device information");

                    //Get a list of circuits that may have been affected in this version
                    Log.Info(ServiceConfiguration.Name.ToString(), "Determining circuits affected in this version");
                    List<IGeometricNetwork> networksAffected = new List<IGeometricNetwork>();
                    List<int> sourceSideDeviceClassIDs = new List<int>();
                    List<string> affectedCircuits = AffectedCircuits(editedOIDsByClassID, Editor, ref networksAffected, ref sourceSideDeviceClassIDs);
                    Log.Info(ServiceConfiguration.Name.ToString(), "Circuits affected in this version:" + affectedCircuits.Count);

                    //Now we have our list of circuits and networks. We need to trace them and order them.
                    TraceNetworks(Editor, networksAffected, affectedCircuits, ref sourceSideDeviceClassIDs, ref editedOIDsByClassID);
                    wkspcEdit.StopEditOperation();
                    inEditOperation = false;
                    success = true;
                }

                return true;

            }
            catch (Exception ex)
            {
                this.Log.Error(ServiceConfiguration.Name.ToString(), "Error during source side device processing: " + ex.Message + " StackTrace: " + ex.StackTrace);
                if (wkspcEdit != null)
                {
                    if (inEditOperation) { wkspcEdit.AbortEditOperation(); }
                    if (wkspcEdit.IsBeingEdited())
                    {
                        wkspcEdit.StopEditing(false);

                    }
                    wkspcEdit = null;
                }
                throw ex;

            }
            finally
            {
                if (wkspcEdit != null)
                {
                    if (success)
                    {
                        if (wkspcEdit.IsBeingEdited())
                        {
                            wkspcEdit.StopEditing(true);
                        }
                    }
                    else
                    {
                        if (wkspcEdit.IsBeingEdited())
                        {
                            wkspcEdit.StopEditing(false);
                        }
                    }
                    wkspcEdit = null;
                }

            }
        }

        private List<string> AffectedCircuits(Dictionary<int, List<int>> editedOIDsByClassID, IWorkspace workspace,
              ref List<IGeometricNetwork> affectedNetworks, ref List<int> sourceSideDeviceClassIDs)
        {
            List<string> affectedCircuits = new List<string>();
            //Get list of feature classes with the PGE_SourceSideDeviceID
            IFeatureWorkspace featWorkspace = workspace as IFeatureWorkspace;
            try
            {
                //Get a list of feature class with the source side device ID and then iterate over that networks feature classes for classes with the source side device ID field
                IEnumBSTR classNamesForSSD = ModelNameManager.Instance.ClassNamesFromModelNameWS(workspace, SchemaInfo.Electric.ClassModelNames.SourceSideDevice);
                classNamesForSSD.Reset();
                string sourceSideDeviceClass = "";
                List<string> networksProcessed = new List<string>();
                while (!string.IsNullOrEmpty((sourceSideDeviceClass = classNamesForSSD.Next())))
                {
                    List<ITable> classesToProcess = new List<ITable>();
                    IFeatureClass sourceSideDevice = featWorkspace.OpenFeatureClass(sourceSideDeviceClass);

                    try
                    {
                        string networkName = "";
                        if (sourceSideDevice != null && sourceSideDevice is INetworkClass)
                        {
                            sourceSideDeviceClassIDs.Add(sourceSideDevice.ObjectClassID);
                            INetworkClass networkClass = sourceSideDevice as INetworkClass;
                            networkName = ((IDataset)networkClass.GeometricNetwork).BrowseName;
                            if (networksProcessed.Contains(networkName)) { continue; }

                            classesToProcess.AddRange(GetNetworkClasses(esriFeatureType.esriFTSimpleJunction, networkClass.GeometricNetwork));
                            classesToProcess.AddRange(GetNetworkClasses(esriFeatureType.esriFTSimpleEdge, networkClass.GeometricNetwork));
                            classesToProcess.AddRange(GetNetworkClasses(esriFeatureType.esriFTComplexEdge, networkClass.GeometricNetwork));
                            classesToProcess.AddRange(GetNetworkClasses(esriFeatureType.esriFTComplexJunction, networkClass.GeometricNetwork));

                            networksProcessed.Add(networkName);
                            affectedNetworks.Add(networkClass.GeometricNetwork);
                        }


                        foreach (ITable featClass in classesToProcess)
                        {
                            int objectClassID = ((IObjectClass)featClass).ObjectClassID;
                            if (featClass != null && editedOIDsByClassID.ContainsKey(objectClassID))
                            {
                                List<int> oids = editedOIDsByClassID[objectClassID];
                                StringBuilder whereInClauseBuilder = new StringBuilder();
                                List<string> whereInClauses = new List<string>();
                                for (int i = 0; i < oids.Count; i++)
                                {
                                    if ((i != 0 && (i % 999) == 0) || (i == oids.Count - 1))
                                    {
                                        whereInClauseBuilder.Append(oids[i]);
                                        whereInClauses.Add(whereInClauseBuilder.ToString());
                                        whereInClauseBuilder = new StringBuilder();
                                    }
                                    else
                                    {
                                        whereInClauseBuilder.Append(oids[i] + ",");
                                    }
                                }

                                foreach (string whereInClause in whereInClauses)
                                {

                                    IQueryFilter qf = new QueryFilterClass();
                                    qf.WhereClause = featClass.OIDFieldName + " in (" + whereInClause + ")";
                                    qf.SubFields = featClass.OIDFieldName;

                                    ICursor cursor = featClass.Search(qf, false);
                                    IRow feat = null;
                                    while ((feat = cursor.NextRow()) != null)
                                    {
                                        string[] circuitIDs = FeederManager2.GetCircuitIDs(feat);
                                        if (circuitIDs != null)
                                        {
                                            foreach (string circuitID in circuitIDs)
                                            {
                                                if (!string.IsNullOrEmpty(circuitID))
                                                {
                                                    affectedCircuits.Add(circuitID);
                                                }
                                            }
                                        }
                                        else
                                        {
                                            try
                                            {
                                                this.Log.Warn("Unable to determine circuit ID for " + ((IDataset)featClass).BrowseName + " OID: " + feat.OID);
                                            }
                                            catch { }
                                        }

                                    }
                                    if (cursor != null) { Marshal.FinalReleaseComObject(cursor); cursor = null; }
                                    //if (qf != null) { while (Marshal.ReleaseComObject(qf) > 0) { } }
                                }
                            }
                            //if (featClass != null) { while (Marshal.ReleaseComObject(featClass) > 0) { } }
                        }
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    finally
                    {
                        if (sourceSideDevice != null) { Marshal.FinalReleaseComObject(sourceSideDevice); sourceSideDevice = null; }
                        if (classesToProcess != null) { classesToProcess.Clear(); }
                    }
                }

                if (classNamesForSSD != null) { Marshal.FinalReleaseComObject(classNamesForSSD); classNamesForSSD = null; }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            
            return affectedCircuits.Distinct().ToList();
        }

        private List<ITable> GetNetworkClasses(esriFeatureType featureType, IGeometricNetwork geomNetwork)
        {
            List<ITable> featureClasses = new List<ITable>();
            try
            {
                IEnumFeatureClass networkClasses = geomNetwork.get_ClassesByType(featureType);
                networkClasses.Reset();
                IFeatureClass netFeatClass = null;
                while ((netFeatClass = networkClasses.Next()) != null)
                {
                    //IEnumBSTR fields = ModelNameManager.Instance.FieldNamesFromModelName(netFeatClass, SchemaInfo.Electric.FieldModelNames.SourceSideDeviceId);
                    //string field = "";
                    //if (!string.IsNullOrEmpty((field = fields.Next())))
                    //{
                    featureClasses.Add(netFeatClass as ITable);
                    //}
                }
                if (networkClasses != null) { Marshal.FinalReleaseComObject(networkClasses); networkClasses = null; }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return featureClasses;
        }

        private void TraceNetworks(IWorkspace editWorkspace, List<IGeometricNetwork> networksAffected,
            List<string> CircuitsAffected, ref List<int> sourceSideDeviceClassIDs, ref Dictionary<int, List<int>> oidsByClassID)
        {
            IMMFeederExt feederExt = null;
            try
            {
                Type type = Type.GetTypeFromProgID("mmFramework.MMFeederExt");
                object obj = Activator.CreateInstance(type);
                feederExt = obj as IMMFeederExt;
                foreach (IGeometricNetwork network in networksAffected)
                {
                    Dictionary<int, List<int>> junctionSSDMapping = null;
                    Dictionary<int, List<int>> edgeSSDMapping = null;


                    //Trace all of the circuits to get the mapping of junctions and edges to their associated source side devices
                    //This returns an EID mapping.
                    foreach (string circuitID in CircuitsAffected)
                    {
                        try
                        {
                            junctionSSDMapping = new Dictionary<int, List<int>>();
                            edgeSSDMapping = new Dictionary<int, List<int>>();
                            Log.Info(ServiceConfiguration.Name.ToString(), "Processing circuit: " + circuitID + " of " + ((IDataset)network).BrowseName);
                            TraceForSSDs(network, circuitID, sourceSideDeviceClassIDs,
                                ref junctionSSDMapping, ref edgeSSDMapping, feederExt);
                            GC.Collect();
                            Log.Info(ServiceConfiguration.Name.ToString(), "Checking for SSDs that need updates");
                         }
                        catch (Exception ex)
                        {
                            Log.Info(ServiceConfiguration.Name.ToString(),"Failed for CircuitID - - " + circuitID  +  "   Exception ---" + ex.ToString());
                        }


                    }
                    if (junctionSSDMapping != null)
                    {
                        ApplySourceSideDeviceIDs(network, junctionSSDMapping, edgeSSDMapping, ref oidsByClassID);
                    }
                    if (junctionSSDMapping != null) { junctionSSDMapping.Clear(); junctionSSDMapping = null; }
                    if (edgeSSDMapping != null) { edgeSSDMapping.Clear(); edgeSSDMapping = null; }
                   

                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (feederExt != null) { Marshal.FinalReleaseComObject(feederExt); feederExt = null; }
            }

        }

        private void ApplySourceSideDeviceIDs(IGeometricNetwork geomNetwork, Dictionary<int, List<int>> junctionSSDMapping,
            Dictionary<int, List<int>> edgeSSDMapping, ref Dictionary<int, List<int>> oidsByClassID)
        {
           
            Type auType = Type.GetTypeFromProgID("mmGeodatabase.MMAutoUpdater");
            object auObj = Activator.CreateInstance(auType);
            IMMAutoUpdater immAutoupdater = auObj as IMMAutoUpdater;
            mmAutoUpdaterMode currentAUMode = immAutoupdater.AutoUpdaterMode;
            immAutoupdater.AutoUpdaterMode = mmAutoUpdaterMode.mmAUMNoEvents;

            IEnumNetEIDBuilder SSDEidBuilder = new EnumNetEIDArrayClass();
            IEIDHelper eidHelper = new ESRI.ArcGIS.NetworkAnalysis.EIDHelperClass();
            IEnumEIDInfo SSDInfoEnum = null;
            IEnumNetEIDBuilder junctionEidBuilder = new EnumNetEIDArrayClass();
            IEnumEIDInfo JunctionEIDInfo = null;
           // Dictionary<string, List<IEIDInfo>> ClassNameToJunctionEIDMapping = new Dictionary<string, List<IEIDInfo>>();

            try
            {
                //First build the list of source side device EID to operating number mappings
                Log.Info(ServiceConfiguration.Name.ToString(),"Retrieving Source Side Device Information");
                SSDEidBuilder.ElementType = esriElementType.esriETJunction;
                SSDEidBuilder.Network = geomNetwork.Network;

                List<int> distinctSSDList = new List<int>();
                distinctSSDList.AddRange(junctionSSDMapping.Keys);
                distinctSSDList = distinctSSDList.Distinct().ToList();

                Log.Info(ServiceConfiguration.Name.ToString(),"There are " + distinctSSDList.Count + " SSDs to gather information for");

                foreach (int SSDEid in distinctSSDList)
                {
                    SSDEidBuilder.Add(SSDEid);
                }

                eidHelper.GeometricNetwork = geomNetwork;
                eidHelper.ReturnGeometries = false;
                eidHelper.ReturnFeatures = true;
                eidHelper.AddField("OPERATINGNUMBER");
                eidHelper.AddField("GLOBALID");
                SSDInfoEnum = eidHelper.CreateEnumEIDInfo((IEnumNetEID)SSDEidBuilder);
                Log.Info(ServiceConfiguration.Name.ToString(),"Obtained potential SSD lists");
                Log.Info(ServiceConfiguration.Name.ToString(),"Determining which SSDs are Protective and Auto protective");
                List<SSDInformation> SSDInfoList = new List<SSDInformation>();
                Dictionary<int, SSDInformation> SSDInformationByEID = new Dictionary<int, SSDInformation>();
                Dictionary<IObjectClass, List<int>> SSDFeatClassByOIDs = new Dictionary<IObjectClass, List<int>>();
                #region getSSDInfo
                SSDInfoEnum.Reset();
                IEIDInfo SSDInfo = null;
                SSDInfo = SSDInfoEnum.Next();
                while (SSDInfo != null)
                {
                    try
                    {
                        if (SSDInfo.EID >= 0)
                        {
                            SSDInformation newSSDInformation = new SSDInformation();
                            int OperatingNumberIdx = GetFieldIxFromModelName(SSDInfo.Feature.Class, SchemaInfo.Electric.FieldModelNames.OperatingNumber);
                            int GlobalIDIdx = SSDInfo.Feature.Fields.FindField("GLOBALID");
                            newSSDInformation.OperatingNumber = SSDInfo.Feature.get_Value(OperatingNumberIdx);
                            newSSDInformation.GlobalID = SSDInfo.Feature.get_Value(GlobalIDIdx);

                            newSSDInformation.EID = SSDInfo.EID;
                            newSSDInformation.ObjectClassID = SSDInfo.Feature.Class.ObjectClassID;
                            newSSDInformation.ObjectID = SSDInfo.Feature.OID;
                            try
                            {
                                SSDFeatClassByOIDs[SSDInfo.Feature.Class].Add(SSDInfo.Feature.OID);
                            }
                            catch
                            {
                                SSDFeatClassByOIDs.Add(SSDInfo.Feature.Class, new List<int>());
                                SSDFeatClassByOIDs[SSDInfo.Feature.Class].Add(SSDInfo.Feature.OID);
                            }
                            SSDInfoList.Add(newSSDInformation);

                        }
                        Marshal.ReleaseComObject(SSDInfo) ; SSDInfo = null;
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    try
                    {
                        SSDInfo = SSDInfoEnum.Next();
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                   
                }
                #endregion
                if (SSDInfoEnum != null) { Marshal.FinalReleaseComObject(SSDInfoEnum); SSDInfoEnum = null; }

                DetermineSSDInformation(ref SSDInfoList, SSDFeatClassByOIDs);

                foreach (SSDInformation ssdInfo in SSDInfoList)
                {
                    SSDInformationByEID.Add(ssdInfo.EID, ssdInfo);
                }

                Log.Info(ServiceConfiguration.Name.ToString(),"Finished determining Protective and Auto Protective SSDs");

                if (SSDEidBuilder != null) { while (Marshal.ReleaseComObject(SSDEidBuilder) > 0) { } }

                Log.Info(ServiceConfiguration.Name.ToString(),"Retrieving traced junctions to check source side device information");
                //Next build our full list of EIDInfo objects for our junctions                
                junctionEidBuilder.ElementType = esriElementType.esriETJunction;
                junctionEidBuilder.Network = geomNetwork.Network;
                foreach (KeyValuePair<int, List<int>> kvp in junctionSSDMapping)
                {
                    foreach (int junctionEID in kvp.Value)
                    {
                        junctionEidBuilder.Add(junctionEID);
                    }
                }

                eidHelper.ClearFields();
                eidHelper.AddField("SOURCESIDEDEVICEID");
                eidHelper.AddField("PROTECTIVESSD");
                eidHelper.AddField("AUTOPROTECTIVESSD");
                eidHelper.AddField("SSDGUID");
                JunctionEIDInfo = eidHelper.CreateEnumEIDInfo((IEnumNetEID)junctionEidBuilder);

                Dictionary<IFeatureClass, List<int>> SSDOIDsToUpdateByFeatureClass = new Dictionary<IFeatureClass, List<int>>();
                Dictionary<string, SSDToUpdate> SSDsToUpdateByFeatureClass = new Dictionary<string, SSDToUpdate>();
                List<int> reportedMissingFMN = new List<int>();
                Log.Info(ServiceConfiguration.Name.ToString(),"Collecting list of SSDs that require updating");

                #region Now create our dictionary of class names to junction EID info mapping                
                IEIDInfo junctionInfo = null;
                while ((junctionInfo = JunctionEIDInfo.Next()) != null)
                {
                    if (junctionInfo.Feature == null) { continue; }
                    //Commented by YXA6 to Resolve SSSD update ERROR
                    //Don't add the object if our feeder manager edited features doesn't include it.
                    // if (!oidsByClassID.ContainsKey(junctionInfo.Feature.Class.ObjectClassID)) { continue; }
                    //Don't add the object if our feeder manager edited features doesn't include this OID
                    //if (!oidsByClassID[junctionInfo.Feature.Class.ObjectClassID].Contains(junctionInfo.Feature.OID)) { continue; }
                    //Don't add the object if this class doens't have the desired source side device, protectiveSSD, and AutoProtectiveSSD fields.
                    bool containsFieldModelName = ContainsSSDFieldModelName(junctionInfo);
                    if (!containsFieldModelName) { continue; }

                    int SSDGuidIdx = GetFieldIxFromModelName(junctionInfo.Feature.Class, SchemaInfo.Electric.FieldModelNames.SSDGUID);
                    int sourceSideDeviceIdx = GetFieldIxFromModelName(junctionInfo.Feature.Class, SchemaInfo.Electric.FieldModelNames.SourceSideDeviceId);
                    int protectiveSSDIdx = GetFieldIxFromModelName(junctionInfo.Feature.Class, SchemaInfo.Electric.FieldModelNames.ProtectiveSSD);
                    int autoProtectiveSSDIdx = GetFieldIxFromModelName(junctionInfo.Feature.Class, SchemaInfo.Electric.FieldModelNames.AutoProtectiveSSD);

                    object currentSSDOpNum = junctionInfo.Feature.get_Value(sourceSideDeviceIdx);
                    object currentProtectiveSSDOpNum = junctionInfo.Feature.get_Value(protectiveSSDIdx);
                    object currentAutoProtectiveSSDOpNum = junctionInfo.Feature.get_Value(autoProtectiveSSDIdx);
                    object proposedNewSSD = null;
                    object proposedNewSSDGuid = null;
                    object proposedNewProtectiveSSD = null;
                    object proposedNewAutoProtectiveSSD = null;
                    bool SSDAlreadyCorrect = false;
                    bool ProtectiveSSDAlreadyCorrect = false;
                    bool AutoProtectiveSSDAlreadyCorrect = false;

                    GetFeatureSSDValues(SSDInformationByEID, junctionSSDMapping, junctionInfo, currentSSDOpNum, currentProtectiveSSDOpNum,
                        currentAutoProtectiveSSDOpNum, ref proposedNewSSD, ref proposedNewProtectiveSSD, ref proposedNewAutoProtectiveSSD,
                        ref SSDAlreadyCorrect, ref ProtectiveSSDAlreadyCorrect, ref AutoProtectiveSSDAlreadyCorrect, ref proposedNewSSDGuid);

                    //If all values are already correct, then we don't need to do anything more for this one
                    if (SSDAlreadyCorrect && ProtectiveSSDAlreadyCorrect && AutoProtectiveSSDAlreadyCorrect) { continue; }

                    SSDToUpdate ssdToUpdate = new SSDToUpdate();

                    //If the current SSD was not correct then let's set it
                    if (!SSDAlreadyCorrect && proposedNewSSD != null)
                    {
                        ssdToUpdate.updateSSD = true;
                        ssdToUpdate.newSSD = proposedNewSSD;
                        ssdToUpdate.newSSDGUID = proposedNewSSDGuid;
                        ssdToUpdate.currentSSD = currentSSDOpNum;
                    }
                    if (!ProtectiveSSDAlreadyCorrect && proposedNewProtectiveSSD != null)
                    {
                        ssdToUpdate.updateProtectiveSSD = true;
                        ssdToUpdate.newProtectiveSSD = proposedNewProtectiveSSD;
                        ssdToUpdate.currentProtectiveSSD = currentSSDOpNum;
                    }
                    if (!AutoProtectiveSSDAlreadyCorrect && proposedNewAutoProtectiveSSD != null)
                    {
                        ssdToUpdate.updateAutoProtectiveSSD = true;
                        ssdToUpdate.newAutoProtectiveSSD = proposedNewAutoProtectiveSSD;
                        ssdToUpdate.currentAutoProtectiveSSD = currentSSDOpNum;
                    }

                    if (ssdToUpdate.updateAutoProtectiveSSD || ssdToUpdate.updateProtectiveSSD || ssdToUpdate.updateSSD)
                    {
                        try
                        {
                            SSDOIDsToUpdateByFeatureClass[(IFeatureClass)junctionInfo.Feature.Class].Add(junctionInfo.Feature.OID);
                        }
                        catch
                        {
                            SSDOIDsToUpdateByFeatureClass.Add((IFeatureClass)junctionInfo.Feature.Class, new List<int>());
                            SSDOIDsToUpdateByFeatureClass[(IFeatureClass)junctionInfo.Feature.Class].Add(junctionInfo.Feature.OID);
                        }
                        try
                        {
                            SSDsToUpdateByFeatureClass.Add(junctionInfo.Feature.Class.ObjectClassID + "," + junctionInfo.Feature.OID, ssdToUpdate);
                        }
                        catch { /*if a key has already been added we can ignore here*/ }
                    }

                    Marshal.FinalReleaseComObject(junctionInfo);
                }

                SSDInformationByEID.Clear();
                SSDInformationByEID = null;
                if (JunctionEIDInfo != null) { Marshal.FinalReleaseComObject(JunctionEIDInfo); JunctionEIDInfo = null; }
                if (SSDInfo != null) { Marshal.ReleaseComObject(SSDInfo); }

                Log.Info(ServiceConfiguration.Name.ToString(),"Finished Collecting list of SSDs that require updating");
                #endregion
                foreach (KeyValuePair<IFeatureClass, List<int>> kvp in SSDOIDsToUpdateByFeatureClass)
                {
                    List<int> distinctOIDs = kvp.Value.Distinct().ToList();
                    string className = ((IDataset)kvp.Key).BrowseName;
                    Log.Info(ServiceConfiguration.Name.ToString(),"There are " + distinctOIDs.Count + " " + className + " to update SSD information for");

                    StringBuilder whereInClauseBuilder = new StringBuilder();
                    List<string> whereInClauses = new List<string>();
                    for (int i = 0; i < distinctOIDs.Count; i++)
                    {
                        if ((i != 0 && (i % 999) == 0) || (i == distinctOIDs.Count - 1))
                        {
                            whereInClauseBuilder.Append(distinctOIDs[i]);
                            whereInClauses.Add(whereInClauseBuilder.ToString());
                            whereInClauseBuilder = new StringBuilder();
                        }
                        else
                        {
                            whereInClauseBuilder.Append(distinctOIDs[i] + ",");
                        }
                    }

                    IField SSDGuidField = ModelNameManager.Instance.FieldFromModelName(kvp.Key, SchemaInfo.Electric.FieldModelNames.SSDGUID);
                    IField sourceSideDeviceField = ModelNameManager.Instance.FieldFromModelName(kvp.Key, SchemaInfo.Electric.FieldModelNames.SourceSideDeviceId);
                    IField protectiveSSDField = ModelNameManager.Instance.FieldFromModelName(kvp.Key, SchemaInfo.Electric.FieldModelNames.ProtectiveSSD);
                    IField autoProtectiveSSDField = ModelNameManager.Instance.FieldFromModelName(kvp.Key, SchemaInfo.Electric.FieldModelNames.AutoProtectiveSSD);

                    int SSDGuidIDx = GetFieldIxFromModelName(kvp.Key, SchemaInfo.Electric.FieldModelNames.SSDGUID);
                    int sourceSideDeviceIdx = GetFieldIxFromModelName(kvp.Key, SchemaInfo.Electric.FieldModelNames.SourceSideDeviceId);
                    int protectiveSSDIdx = GetFieldIxFromModelName(kvp.Key, SchemaInfo.Electric.FieldModelNames.ProtectiveSSD);
                    int autoProtectiveSSDIdx = GetFieldIxFromModelName(kvp.Key, SchemaInfo.Electric.FieldModelNames.AutoProtectiveSSD);

                    IQueryFilter qf = new QueryFilterClass();
                    qf.AddField(SSDGuidField.Name);
                    qf.AddField(sourceSideDeviceField.Name);
                    qf.AddField(protectiveSSDField.Name);
                    qf.AddField(autoProtectiveSSDField.Name);
                    qf.AddField(kvp.Key.OIDFieldName);
                    int updatedCount = 0;
                    if (wkspcEdit != null)
                    {
                        if (!(wkspcEdit.IsBeingEdited()))
                        {
                            wkspcEdit.StartEditing(true);
                            wkspcEdit.StartEditOperation();

                        }
                    }
                    foreach (string whereInClause in whereInClauses)
                    {
                        qf.WhereClause = kvp.Key.OIDFieldName + " in (" + whereInClause + ")";

                        IFeatureCursor cursor = kvp.Key.Update(qf, false);
                        IFeature feature = null;

                        while ((feature = cursor.NextFeature()) != null)
                        {

                            SSDToUpdate ssdToUpdate = SSDsToUpdateByFeatureClass[kvp.Key.ObjectClassID + "," + feature.OID];
                            if (ssdToUpdate.updateSSD)
                            {
                                Log.Info(ServiceConfiguration.Name.ToString(),className + ": Updating:" + feature.OID + " Old SSD:'" + ssdToUpdate.currentSSD + "' New SSD:'" + ssdToUpdate.newSSD + "'");
                                feature.set_Value(sourceSideDeviceIdx, ssdToUpdate.newSSD.ToString());
                                Log.Info(ServiceConfiguration.Name.ToString(),className + ": Updating:" + feature.OID + " New SSD GUID:'" + ssdToUpdate.newSSDGUID + "'");
                                feature.set_Value(SSDGuidIDx, ssdToUpdate.newSSDGUID.ToString());
                                feature.Store();
                                //Add Data in GDBM staging table

                                cmnfuncts.UpdateData(feature as IObject, strVersionName, sSessionName, "U", string.Empty);
                            }
                            if (ssdToUpdate.updateProtectiveSSD)
                            {
                                Log.Info(ServiceConfiguration.Name.ToString(),className + ": Updating:" + feature.OID + " Old SSD:'" + ssdToUpdate.currentProtectiveSSD + "' New SSD:'" + ssdToUpdate.newProtectiveSSD + "'");
                                feature.set_Value(protectiveSSDIdx, ssdToUpdate.newProtectiveSSD.ToString());
                                feature.Store();
                                //Add Data in GDBM staging table

                                cmnfuncts.UpdateData(feature as IObject, strVersionName, sSessionName, "U", string.Empty);

                            }
                            if (ssdToUpdate.updateAutoProtectiveSSD)
                            {
                                Log.Info(ServiceConfiguration.Name.ToString(),className + ": Updating:" + feature.OID + " Old SSD:'" + ssdToUpdate.currentAutoProtectiveSSD + "' New SSD:'" + ssdToUpdate.newAutoProtectiveSSD + "'");
                                feature.set_Value(autoProtectiveSSDIdx, ssdToUpdate.newAutoProtectiveSSD.ToString());
                                feature.Store();
                                //Add Data in GDBM staging table

                                cmnfuncts.UpdateData(feature as IObject, strVersionName, sSessionName, "U", string.Empty);
                            }

                            updatedCount++;
                        }
                        Marshal.ReleaseComObject(cursor);

                        Log.Info(ServiceConfiguration.Name.ToString(),updatedCount + " have been Suceesfully updated so far for " + className);
                    }

                    try
                    {
                        if (wkspcEdit != null)
                        {
                            if (wkspcEdit.IsBeingEdited())
                            {

                                wkspcEdit.StopEditOperation();
                                wkspcEdit.StopEditing(true);

                            }
                        }
                    }
                    catch { }
                }
            }
            catch (Exception ex)
            {
                if (wkspcEdit != null)
                {
                    if (!(wkspcEdit.IsBeingEdited()))
                    {
                        wkspcEdit.StopEditing(false);
                    }
                }
                throw ex;
            }
            finally
            {
                immAutoupdater.AutoUpdaterMode = currentAUMode;

                if (SSDEidBuilder != null) { Marshal.ReleaseComObject(SSDEidBuilder); }
                
                if (junctionEidBuilder != null) { Marshal.ReleaseComObject(junctionEidBuilder); }
                if (JunctionEIDInfo != null) { Marshal.ReleaseComObject(JunctionEIDInfo); }
                if (eidHelper != null) { Marshal.ReleaseComObject(eidHelper); }

            }
        }

        private void DetermineSSDInformation(ref List<SSDInformation> SSDInformationList, Dictionary<IObjectClass, List<int>> FeatClassToSSDOID)
        {
            try
            {
                foreach (KeyValuePair<IObjectClass, List<int>> kvp in FeatClassToSSDOID)
                {
                    List<int> SSDs = new List<int>();
                    if (SSDClassIDToWhereClause.ContainsKey(((IObjectClass)kvp.Key).ObjectClassID))
                    {
                        string SSDWhereClause = SSDClassIDToWhereClause[((IObjectClass)kvp.Key).ObjectClassID];
                        Log.Info(ServiceConfiguration.Name.ToString(),"Querying for SSDs for " + ((IDataset)kvp.Key).BrowseName + " using: " + SSDWhereClause);
                        SSDs = GetOIDsSatisfiedByQuery(kvp.Key as ITable, kvp.Value, SSDWhereClause);
                    }

                    List<int> ProtectiveSSDs = new List<int>();
                    if (ProtectiveSSDClassIDToWhereClause.ContainsKey(((IObjectClass)kvp.Key).ObjectClassID))
                    {
                        string protectiveWhereClause = ProtectiveSSDClassIDToWhereClause[((IObjectClass)kvp.Key).ObjectClassID];
                        Log.Info(ServiceConfiguration.Name.ToString(),"Querying for Protective SSDs for " + ((IDataset)kvp.Key).BrowseName + " using: " + protectiveWhereClause);
                        ProtectiveSSDs = GetOIDsSatisfiedByQuery(kvp.Key as ITable, kvp.Value, protectiveWhereClause);
                    }

                    List<int> AutoProtectiveSSDs = new List<int>();
                    if (AutoProtectiveSSDClassIDToWhereClause.ContainsKey(((IObjectClass)kvp.Key).ObjectClassID))
                    {
                        string autoProtectiveWhereClause = AutoProtectiveSSDClassIDToWhereClause[((IObjectClass)kvp.Key).ObjectClassID];
                        Log.Info(ServiceConfiguration.Name.ToString(),"Querying for Auto Protective SSDs for " + ((IDataset)kvp.Key).BrowseName + " using: " + autoProtectiveWhereClause);
                        AutoProtectiveSSDs = GetOIDsSatisfiedByQuery(kvp.Key as ITable, kvp.Value, autoProtectiveWhereClause);
                    }

                    foreach (SSDInformation SSDInfo in SSDInformationList)
                    {
                        if (SSDInfo.ObjectClassID == kvp.Key.ObjectClassID)
                        {
                            if (SSDs.Contains(SSDInfo.ObjectID))
                            {
                                SSDInfo.IsSSD = true;
                            }
                            if (ProtectiveSSDs.Contains(SSDInfo.ObjectID))
                            {
                                SSDInfo.IsProtectiveSSD = true;
                            }
                            if (AutoProtectiveSSDs.Contains(SSDInfo.ObjectID))
                            {
                                SSDInfo.IsAutoProtectiveSSD = true;
                            }
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// This method will determine all three of the source side devices for a given feature
        /// </summary>
        /// <param name="junctionSSDMapping">Junction SSD Mapping dictionary</param>
        /// <param name="feature">Feature to determine SSDs for</param>
        /// <param name="SSD">Source Side Device</param>
        /// <param name="protectiveSSD">Protective Source Side Device</param>
        /// <param name="autoProtectiveSSD">Auto Protective Source Side Device</param>
        private void GetFeatureSSDValues(Dictionary<int, SSDInformation> SSDOperatingNumbersByEID,
            Dictionary<int, List<int>> junctionSSDMapping, IEIDInfo feature, object currentSSDOpNum, object currentProtectiveSSDOpNum,
            object currentAutoProtectiveSSDOpNum, ref object SSDOpNum, ref object protectiveSSDOpNum, ref object autoProtectiveSSDOpNum,
            ref bool SSDAlreadyCorrect, ref bool ProtectiveSSDAlreadyCorrect, ref bool AutoProtectiveSSDAlreadyCorrect, ref object SSDGUID)
        {
            int currentSSDEid = -1;
            SSDInformation CurrentSSDInformation = new SSDInformation();
            SSDInformation CurrentProtectiveSSDInformation = new SSDInformation();
            SSDInformation CurrentAutoProtectiveSSDInformation = new SSDInformation();
            Dictionary<int, SSDInformation> proposedSSDInformation = null;
            try
            {
                //Determine the proposed new SSD
                proposedSSDInformation = GetSSDs(feature.EID, SSDOperatingNumbersByEID, junctionSSDMapping);
                foreach (KeyValuePair<int, SSDInformation> ssd in proposedSSDInformation)
                {
                    if (ssd.Value.IsSSD)
                    {
                        if (!string.IsNullOrEmpty(ssd.Value.OperatingNumber.ToString()) && ssd.Value.OperatingNumber.Equals(currentSSDOpNum))
                        {
                            currentSSDEid = ssd.Key;
                            CurrentSSDInformation = ssd.Value;
                            SSDOpNum = CurrentSSDInformation.OperatingNumber;
                            SSDAlreadyCorrect = true;
                            break;
                        }
                        else
                        {
                            currentSSDEid = ssd.Key;
                            CurrentSSDInformation = ssd.Value;
                            SSDOpNum = CurrentSSDInformation.OperatingNumber;
                            SSDGUID = CurrentSSDInformation.GlobalID;
                        }
                    }
                }


                //Determine the protective SSD. This should be determined based on what the current (if correct) or new source side device is.
                if (CurrentSSDInformation.IsProtectiveSSD)
                {
                    //Our current SSD is also our current protective SSD
                    CurrentProtectiveSSDInformation = CurrentSSDInformation;
                }
                else
                {
                    //Our current SSD is not a protective SSD so we need to go further upstream
                    CurrentProtectiveSSDInformation = GetProtectiveSSD(currentSSDEid, SSDOperatingNumbersByEID, junctionSSDMapping);
                }

                //Check whether the current protective SSD is the same
                if (CurrentProtectiveSSDInformation.OperatingNumber != null && !string.IsNullOrEmpty(CurrentProtectiveSSDInformation.OperatingNumber.ToString()))
                {
                    if (CurrentProtectiveSSDInformation.OperatingNumber.Equals(currentProtectiveSSDOpNum))
                    {
                        ProtectiveSSDAlreadyCorrect = true;
                    }
                    else
                    {
                        ProtectiveSSDAlreadyCorrect = false;
                    }
                    protectiveSSDOpNum = CurrentProtectiveSSDInformation.OperatingNumber;
                }

                //Determine the Auto protective SSD. This should be determined based on what the current (if correct) or new protective 
                //source side device is.
                if (CurrentProtectiveSSDInformation.IsAutoProtectiveSSD)
                {
                    //Our current SSD is also our current protective SSD
                    CurrentAutoProtectiveSSDInformation = CurrentProtectiveSSDInformation;
                }
                else
                {
                    //Our current SSD is not a protective SSD so we need to go further upstream
                    CurrentAutoProtectiveSSDInformation = GetAutoProtectiveSSD(currentSSDEid, SSDOperatingNumbersByEID, junctionSSDMapping);
                }

                //Check whether the current auto protective SSD is the same
                if (CurrentAutoProtectiveSSDInformation.OperatingNumber != null && !string.IsNullOrEmpty(CurrentAutoProtectiveSSDInformation.OperatingNumber.ToString()))
                {
                    if (CurrentAutoProtectiveSSDInformation.OperatingNumber.Equals(currentAutoProtectiveSSDOpNum))
                    {
                        AutoProtectiveSSDAlreadyCorrect = true;
                    }
                    else
                    {
                        AutoProtectiveSSDAlreadyCorrect = false;
                    }
                    autoProtectiveSSDOpNum = CurrentAutoProtectiveSSDInformation.OperatingNumber;
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally 
            {
                if (proposedSSDInformation != null) { proposedSSDInformation.Clear(); proposedSSDInformation = null; }
            }
        }

        ///// <summary>
        /// Returns the first encountered Protective device upstream of the given junction EID
        /// </summary>
        /// <param name="EID">Junction EID for start</param>
        /// <param name="SSDInformationByEID">SSD Information Objects sorted by EID</param>
        /// <param name="junctionSSDMapping">Junction SSD map</param>
        /// <returns></returns>
        private SSDInformation GetProtectiveSSD(int EID, Dictionary<int, SSDInformation> SSDInformationByEID, Dictionary<int, List<int>> junctionSSDMapping)
        {
            SSDInformation protectiveSSD = new SSDInformation();
            protectiveSSD.ObjectClassID = -1;
            protectiveSSD.OperatingNumber = null;
            try
            {
                Dictionary<int, SSDInformation> proposedProtectiveDevices = GetSSDs(EID, SSDInformationByEID, junctionSSDMapping);
                foreach (KeyValuePair<int, SSDInformation> ssd in proposedProtectiveDevices)
                {
                    if (ssd.Value.IsProtectiveSSD)
                    {
                        protectiveSSD = ssd.Value;
                        break;
                    }
                    else { return GetProtectiveSSD(ssd.Key, SSDInformationByEID, junctionSSDMapping); }
                }
            }
            catch(Exception ex) 
            { throw ex; }

            return protectiveSSD;
        }

        /// <summary>
        /// Returns the first encountered Auto Protective device upstream of the given junction EID
        /// </summary>
        /// <param name="EID">Junction EID for start</param>
        /// <param name="SSDInformationByEID">SSD Information Objects sorted by EID</param>
        /// <param name="junctionSSDMapping">Junction SSD map</param>
        /// <returns></returns>
        private SSDInformation GetAutoProtectiveSSD(int EID, Dictionary<int, SSDInformation> SSDInformationByEID, Dictionary<int, List<int>> junctionSSDMapping)
        {
            SSDInformation autoProtectiveSSD = new SSDInformation();
            autoProtectiveSSD.ObjectClassID = -1;
            autoProtectiveSSD.OperatingNumber = null;
            try
            {
                Dictionary<int, SSDInformation> proposedAutoProtectiveDevices = GetSSDs(EID, SSDInformationByEID, junctionSSDMapping);
                foreach (KeyValuePair<int, SSDInformation> ssd in proposedAutoProtectiveDevices)
                {
                    if (ssd.Value.IsAutoProtectiveSSD)
                    {
                        autoProtectiveSSD = ssd.Value;
                        break;
                    }
                    else { return GetAutoProtectiveSSD(ssd.Key, SSDInformationByEID, junctionSSDMapping); }
                }
            }
            catch (Exception ex)
            { throw ex; }

            return autoProtectiveSSD;
        }

        bool initializedSSDConfiguration = false;
        Dictionary<int, string> SSDClassIDToWhereClause = new Dictionary<int, string>();
        Dictionary<int, string> ProtectiveSSDClassIDToWhereClause = new Dictionary<int, string>();
        Dictionary<int, string> AutoProtectiveSSDClassIDToWhereClause = new Dictionary<int, string>();
        private void LoadSSDConfiguration()
        {
            try
            {
                if (!initializedSSDConfiguration)
                {
                    initializedSSDConfiguration = true;
                    //Read the config location from the Registry Entry
                    SystemRegistry sysRegistry = new SystemRegistry("PGE");
                    string SSDConfigFilePath = sysRegistry.ConfigPath;
                    //prepare the xmlfile if config path exist
                    string SSDConfigXmlFilePath = System.IO.Path.Combine(SSDConfigFilePath, "PGESSDConfig.xml");

                    if (!System.IO.File.Exists(SSDConfigXmlFilePath))
                    {
                        string assemblyLoc = Assembly.GetExecutingAssembly().Location;
                        SSDConfigXmlFilePath = System.IO.Path.Combine(assemblyLoc, "PGESSDConfig.xml");
                    }

                    try
                    {
                        bool processingSSDs = false;
                        bool processingProtectiveSSDs = false;
                        bool processingAutoProtectiveSSDs = false;
                        int currentClassID = -1;
                        using (XmlReader reader = XmlReader.Create(new StreamReader(SSDConfigXmlFilePath)))
                        {
                            while (reader.Read())
                            {
                                switch (reader.NodeType)
                                {
                                    case XmlNodeType.Element:
                                        if (reader.Name.ToUpper() == "PROTECTIVESSDS")
                                        {
                                            processingAutoProtectiveSSDs = false;
                                            processingProtectiveSSDs = true;
                                            processingSSDs = false;
                                        }
                                        else if (reader.Name.ToUpper() == "AUTOPROTECTIVESSDS")
                                        {
                                            processingAutoProtectiveSSDs = true;
                                            processingProtectiveSSDs = false;
                                            processingSSDs = false;
                                        }
                                        else if (reader.Name.ToUpper() == "SSDS")
                                        {
                                            processingAutoProtectiveSSDs = false;
                                            processingProtectiveSSDs = false;
                                            processingSSDs = true;
                                        }
                                        else if (reader.Name.ToUpper() == "CLASS")
                                        {
                                            currentClassID = Int32.Parse(reader.GetAttribute("ClassID"));
                                            string whereClause = reader.GetAttribute("WhereClause");
                                            if (processingProtectiveSSDs)
                                            {
                                                ProtectiveSSDClassIDToWhereClause.Add(currentClassID, whereClause);
                                            }
                                            else if (processingAutoProtectiveSSDs)
                                            {
                                                AutoProtectiveSSDClassIDToWhereClause.Add(currentClassID, whereClause);
                                            }
                                            else if (processingSSDs)
                                            {
                                                SSDClassIDToWhereClause.Add(currentClassID, whereClause);
                                            }
                                        }
                                        break;
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.Info(ServiceConfiguration.Name.ToString(),"Error loading SSD configuration: " + ex.Message);
                    }
                }
            }
            catch (Exception ex)
            { throw ex; }
        }

        /// <summary>
        /// Checks if the given SSDInformation object is a protective SSD
        /// </summary>
        /// <param name="table">Table to query against if a where clause is needed</param>
        /// <param name="SSDInformation">SSDInformation object to check</param>
        /// <returns></returns>
        private List<int> GetOIDsSatisfiedByQuery(ITable table, List<int> toQueryIDs, string SSDWhereClause)
        {
            toQueryIDs = toQueryIDs.Distinct().ToList();

            List<int> IDsSatisfiedByQuery = new List<int>();
            StringBuilder whereInClauseBuilder = new StringBuilder();
            List<string> whereInClauses = new List<string>();
            try
            {
                for (int i = 0; i < toQueryIDs.Count; i++)
                {
                    if ((i != 0 && (i % 999) == 0) || (i == toQueryIDs.Count - 1))
                    {
                        whereInClauseBuilder.Append(toQueryIDs[i]);
                        whereInClauses.Add(whereInClauseBuilder.ToString());
                        whereInClauseBuilder = new StringBuilder();
                    }
                    else
                    {
                        whereInClauseBuilder.Append(toQueryIDs[i] + ",");
                    }
                }

                foreach (string whereInClause in whereInClauses)
                {
                    IQueryFilter qf = new QueryFilterClass();

                    qf.AddField(table.OIDFieldName);
                    qf.WhereClause = table.OIDFieldName + " in (" + whereInClause + ")";
                    if (!string.IsNullOrEmpty(SSDWhereClause))
                    {
                        qf.WhereClause += " AND (" + SSDWhereClause + ")";
                    }

                    ICursor foundFeatures = table.Search(qf, false);
                    IRow row = null;
                    while ((row = foundFeatures.NextRow()) != null)
                    {
                        IDsSatisfiedByQuery.Add(row.OID);
                       
                    }
                    Marshal.FinalReleaseComObject(foundFeatures);
                }
            }
            catch (Exception ex)
            { throw ex; }
            return IDsSatisfiedByQuery;
        }

        /// <summary>
        /// Checks if the given SSDInformation object is a protective SSD
        /// </summary>
        /// <param name="table">Table to query against if a where clause is needed</param>
        /// <param name="SSDInformation">SSDInformation object to check</param>
        /// <returns></returns>
        private bool IsProtectiveSSD(ITable table, SSDInformation SSDInformation)
        {
            bool isSSD = false;
            try
            {
                if (ProtectiveSSDClassIDToWhereClause.ContainsKey(SSDInformation.ObjectClassID))
                {
                    IQueryFilter qf = new QueryFilterClass();
                    string classWhereClause = ProtectiveSSDClassIDToWhereClause[SSDInformation.ObjectClassID];
                    qf.WhereClause = table.OIDFieldName + " = " + SSDInformation.ObjectID;
                    if (!string.IsNullOrEmpty(classWhereClause))
                    {
                        qf.WhereClause += " AND (" + classWhereClause + ")";
                    }
                    int rowCount = table.RowCount(qf);
                    if (rowCount > 0) { isSSD = true; }
                }
            }
            catch (Exception ex)
            { throw ex; }
            return isSSD;
        }

        /// <summary>
        /// Checks if the given SSDInformation object is an auto protective SSD
        /// </summary>
        /// <param name="table">Table to query against if a where clause is needed</param>
        /// <param name="SSDInformation">SSDInformation object to check</param>
        /// <returns></returns>
        private bool IsAutoProtectiveSSD(ITable table, SSDInformation SSDInformation)
        {
            bool isSSD = false;
            try
            { 
                if (AutoProtectiveSSDClassIDToWhereClause.ContainsKey(SSDInformation.ObjectClassID))
                {
                    IQueryFilter qf = new QueryFilterClass();
                    string classWhereClause = AutoProtectiveSSDClassIDToWhereClause[SSDInformation.ObjectClassID];
                    qf.WhereClause = table.OIDFieldName + " = " + SSDInformation.ObjectID;
                    if (!string.IsNullOrEmpty(classWhereClause))
                    {
                        qf.WhereClause += " AND (" + classWhereClause + ")";
                    }
                    int rowCount = table.RowCount(qf);
                    if (rowCount > 0) { isSSD = true; }
                }

            }
            catch (Exception ex)
            { throw ex; }
            return isSSD;
        }

        private Dictionary<int, SSDInformation> GetSSDs(int EID, Dictionary<int, SSDInformation> SSDOperatingNumbersByEID, Dictionary<int, List<int>> junctionSSDMapping)
        {
            Dictionary<int, SSDInformation> currentSSDs = new Dictionary<int, SSDInformation>();
            try
            {
                foreach (KeyValuePair<int, List<int>> SSDToJunctionsKVP in junctionSSDMapping)
                {
                    if (SSDToJunctionsKVP.Value.Contains(EID))
                    {
                        if (!SSDOperatingNumbersByEID.ContainsKey(SSDToJunctionsKVP.Key)) { continue; }

                        SSDInformation thisSSD = SSDOperatingNumbersByEID[SSDToJunctionsKVP.Key];
                        if (thisSSD.OperatingNumber == null) { continue; }
                        else
                        {
                            currentSSDs.Add(SSDToJunctionsKVP.Key, thisSSD);
                        }
                    }
                }
            }
            catch (Exception ex)
            { throw ex; }
            return currentSSDs;
        }

        private List<int> reportedMissingFMN = new List<int>();
        private bool ContainsSSDFieldModelName(IEIDInfo FeatureInfo)
        {
            bool sdvalue = false;
            try
            {
                if (GetFieldIxFromModelName(FeatureInfo.Feature.Class, SchemaInfo.Electric.FieldModelNames.SourceSideDeviceId) < 0)
                {
                    if (!reportedMissingFMN.Contains(FeatureInfo.Feature.Class.ObjectClassID))
                    {
                        reportedMissingFMN.Add(FeatureInfo.Feature.Class.ObjectClassID);
                        Log.Info(ServiceConfiguration.Name.ToString(), ((IDataset)FeatureInfo.Feature.Class).BrowseName + " does not have the required " +
                        SchemaInfo.Electric.FieldModelNames.SourceSideDeviceId + " field model name assigned and will be skipped for processing.");
                    }
                    sdvalue =false;
                    return false;
                }
                if (GetFieldIxFromModelName(FeatureInfo.Feature.Class, SchemaInfo.Electric.FieldModelNames.ProtectiveSSD) < 0)
                {
                    if (!reportedMissingFMN.Contains(FeatureInfo.Feature.Class.ObjectClassID))
                    {
                        reportedMissingFMN.Add(FeatureInfo.Feature.Class.ObjectClassID);
                        Log.Info(ServiceConfiguration.Name.ToString(), ((IDataset)FeatureInfo.Feature.Class).BrowseName + " does not have the required " +
                       SchemaInfo.Electric.FieldModelNames.ProtectiveSSD + " field model name assigned and will be skipped for processing.");
                    }
                    sdvalue = false;
                    return false;

                }
                if (GetFieldIxFromModelName(FeatureInfo.Feature.Class, SchemaInfo.Electric.FieldModelNames.AutoProtectiveSSD) < 0)
                {
                    if (!reportedMissingFMN.Contains(FeatureInfo.Feature.Class.ObjectClassID))
                    {
                        reportedMissingFMN.Add(FeatureInfo.Feature.Class.ObjectClassID);
                        Log.Info(ServiceConfiguration.Name.ToString(), ((IDataset)FeatureInfo.Feature.Class).BrowseName + " does not have the required " +
                       SchemaInfo.Electric.FieldModelNames.AutoProtectiveSSD + " field model name assigned and will be skipped for processing.");
                    }
                    sdvalue = false;

                    return false;
                }
                sdvalue = true;

                return true;
            }
            catch(Exception ex)
            {
                Log.Error(ex.ToString());
            }

            return sdvalue;
            
        }

        Dictionary<int, Dictionary<string, int>> FieldIndexMap = new Dictionary<int, Dictionary<string, int>>();

        private int GetFieldIxFromModelName(IObjectClass table, string fieldModelName)
        {
            IField field = ModelNameManager.Instance.FieldFromModelName(table, fieldModelName);
            if (field != null)
            {
                return GetFieldIdx(table, field.Name);
            }
            else
            {
                return -1;
            }
        }

        private int GetFieldIdx(IObjectClass table, string fieldName)
        {
            if (table == null) { return -1; }

            try
            {
                return FieldIndexMap[table.ObjectClassID][fieldName];
            }
            catch
            {
                if (!FieldIndexMap.ContainsKey(table.ObjectClassID))
                {
                    FieldIndexMap.Add(table.ObjectClassID, new Dictionary<string, int>());
                }
                int fieldIndex = table.FindField(fieldName);
                FieldIndexMap[table.ObjectClassID].Add(fieldName, fieldIndex);
                return fieldIndex;
            }
        }

        private void TraceForSSDs(IGeometricNetwork geomNetwork, string circuitID, List<int> sourceSideDeviceClassIDs,
            ref Dictionary<int, List<int>> junctionSSDList, ref Dictionary<int, List<int>> edgeSSDList,IMMFeederExt feederExt)
        {
            string feederID = "";
            string message = string.Empty;
           
            IMMFeederSpace feederSpace = null;
            IMMFeederSource feederSource = null;
            IMMElectricNetworkTracing downstreamTracer = null;
            IMMNetworkAnalysisExtForFramework networkAnalysisExtForFramework = null;
            IMMElectricTraceSettingsEx electricTraceSettings = null;

            
            try
            {
                feederSpace = feederExt.get_FeederSpace(geomNetwork, false);
                if (feederSpace == null)
                {
                    throw new Exception("Unable to get feeder space information from geometric network");
                }

                INetwork network = geomNetwork.Network;
                INetSchema networkSchema = network as INetSchema;
                INetWeight electricTraceWeight = networkSchema.get_WeightByName("MMELECTRICTRACEWEIGHT");
                downstreamTracer = new MMFeederTracerClass();

                electricTraceSettings = new MMElectricTraceSettingsClass();
                electricTraceSettings.RespectESRIBarriers = true;
                electricTraceSettings.UseFeederManagerCircuitSources = true;
                electricTraceSettings.UseFeederManagerProtectiveDevices = false;
                electricTraceSettings.ProtectiveDeviceClassIDs = sourceSideDeviceClassIDs.ToArray();

                networkAnalysisExtForFramework = new MMNetworkAnalysisExtForFramework();
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

                    Log.Info(ServiceConfiguration.Name.ToString(),"Tracing circuit: " + feederSource.FeederID);

                    //Get our protective devices first
                    downstreamTracer.FindDownstreamProtectiveDevices(geomNetwork, networkAnalysisExtForFramework, electricTraceSettings, feederSource.JunctionEID,
                                                        esriElementType.esriETJunction, phasesToTrace, out junctions, out edges);

                    junctions.Reset();
                    int junction = -1;
                    List<int> sourceSideDeviceEIDs = new List<int>();
                    //Add our junction SSD eids
                    while ((junction = junctions.Next()) > 0)
                    {
                        sourceSideDeviceEIDs.Add(junction);
                    }

                    electricTraceSettings.UseFeederManagerProtectiveDevices = true;
                    //Trace
                    downstreamTracer.TraceDownstream(geomNetwork, networkAnalysisExtForFramework, electricTraceSettings, feederSource.JunctionEID,
                                                        esriElementType.esriETJunction, phasesToTrace, out junctions, out edges);

                    //this.Log.Info( "Finished Tracing circuit: " + feederSource.FeederID);

                    List<int> edgesList = new List<int>();
                    List<int> junctionsList = new List<int>();
                    junctions.Reset();
                    edges.Reset();
                    junction = -1;
                    while ((junction = junctions.Next()) > 0)
                    {
                        junctionsList.Add(junction);
                    }

                    int edge = -1;
                    while ((edge = edges.Next()) > 0)
                    {
                        edgesList.Add(edge);
                    }
                    if (junctions != null) { Marshal.FinalReleaseComObject(junctions); junctions = null; }
                    if (edges != null) { Marshal.FinalReleaseComObject(edges); edges = null; }

                    List<int> visitedJunctions = new List<int>();
                    List<int> visitedEdges = new List<int>();

                    DetermineSourceSideDevices(ref visitedJunctions, ref visitedEdges, network, feederSource.JunctionEID,
                        ref edgesList, ref junctionsList, ref electricTraceWeight, -1, ref junctionSSDList, ref edgeSSDList, ref sourceSideDeviceEIDs);

                   Log.Info(ServiceConfiguration.Name.ToString(), "Finished Processing Circuit ID: " + feederSource.FeederID);
                }
            }
            catch (Exception ex)
            {
               this.Log.Error(ServiceConfiguration.Name.ToString(), "Error executing tracing. Message: " + ex.Message + " StackTrace: " + ex.StackTrace);
                throw ex;
            }
            finally
            {
                if (downstreamTracer != null) { Marshal.FinalReleaseComObject(downstreamTracer); }
                if (networkAnalysisExtForFramework != null) { Marshal.FinalReleaseComObject(networkAnalysisExtForFramework) ; }
                if (electricTraceSettings != null) { Marshal.FinalReleaseComObject(electricTraceSettings);  }
                if (feederSpace != null) { Marshal.FinalReleaseComObject(feederSpace); feederSpace = null; }
                if (feederSource != null) { Marshal.FinalReleaseComObject(feederSource); feederSource = null; }
               
                GC.Collect();
            }
        }

        private void DetermineSourceSideDevices(ref List<int> visitedJunctions, ref List<int> visitedEdges, INetwork networkDataset,
            int startingEID, ref List<int> edgeEIDs, ref List<int> junctionEIDs, ref INetWeight electricTraceWeight, int sourceSideDeviceEID,
            ref Dictionary<int, List<int>> junctionSSDList, ref Dictionary<int, List<int>> edgeSSDList, ref List<int> sourceSideDeviceEIDs)
        {
            if (visitedJunctions.Contains(startingEID)) { return; }
            visitedJunctions.Add(startingEID);

            AddSSDToList(ref junctionSSDList, sourceSideDeviceEID, startingEID);

            //Is this starting eid a source side device?
            if (sourceSideDeviceEIDs.Contains(startingEID)) { sourceSideDeviceEID = startingEID; }

            IForwardStar forwardStar = networkDataset.CreateForwardStar(false, electricTraceWeight, null, null, null);

            try
            {
                int edgeCount = 0;
                forwardStar.FindAdjacent(0, startingEID, out edgeCount);

                for (int i = 0; i < edgeCount; i++)
                {
                    int adjacentEdgeEID = -1;
                    bool reverseOrientation = false;
                    object edgeWeightValue = null;
                    forwardStar.QueryAdjacentEdge(i, out adjacentEdgeEID, out reverseOrientation, out edgeWeightValue);

                    if (edgeEIDs.Contains(adjacentEdgeEID) && !visitedEdges.Contains(adjacentEdgeEID))
                    {
                        visitedEdges.Add(adjacentEdgeEID);

                        //Find the adjacent junction for this edge and add a new row
                        int adjacentJunctionEID = -1;
                        object junctionWeightValue = null;
                        forwardStar.QueryAdjacentJunction(i, out adjacentJunctionEID, out junctionWeightValue);

                        if (junctionEIDs.Contains(adjacentJunctionEID))
                        {
                            if (JunctionIsClosed(junctionWeightValue))
                            {
                                DetermineSourceSideDevices(ref visitedJunctions, ref visitedEdges, networkDataset, adjacentJunctionEID,
                                    ref edgeEIDs, ref junctionEIDs, ref electricTraceWeight, sourceSideDeviceEID, ref junctionSSDList, ref edgeSSDList, ref sourceSideDeviceEIDs);
                            }
                            else
                            {
                                AddSSDToList(ref junctionSSDList, sourceSideDeviceEID, adjacentJunctionEID);
                            }
                            AddSSDToList(ref edgeSSDList, sourceSideDeviceEID, adjacentEdgeEID);
                        }
                        else
                        {
                            AddSSDToList(ref edgeSSDList, sourceSideDeviceEID, adjacentEdgeEID);
                        }
                    }
                }
                return;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (forwardStar != null) { while (Marshal.ReleaseComObject(forwardStar) > 0) ; }
            }
        }

        private void AddSSDToList(ref Dictionary<int, List<int>> SSDList, int junctionSSDEID, int otherEID)
        {
            try
            {
                SSDList[junctionSSDEID].Add(otherEID);
            }
            catch
            {
                SSDList.Add(junctionSSDEID, new List<int>());
                SSDList[junctionSSDEID].Add(otherEID);
            }
        }

        private bool JunctionIsClosed(object electricTraceWeightObject)
        {
            int electricTraceWeight = -1;
            Int32.TryParse(electricTraceWeightObject.ToString(), out electricTraceWeight);

            //If the trace weight is null or less than 0 assume we can trace through
            if (electricTraceWeight < 0) { return true; }

            //Determine the operational phases.  Values of 0 indicate the device is operational on a phase
            bool cPhase = (electricTraceWeight & 32) != 32;
            bool bPhase = (electricTraceWeight & 16) != 16;
            bool aPhase = (electricTraceWeight & 8) != 8;

            //If all phases are showing as not operational (i.e. Null phase designation), we will go ahead and trace through the device
            //as this appears to be how ArcFM handles this scenario as well.
            if ((!aPhase) && (!bPhase) && (!cPhase)) { return true; }

            //Determine open states.  Values of 0 indicate the device is closed on a phase
            bool cPhaseOpen = (electricTraceWeight & 1073741824) != 1073741824;
            bool bPhaseOpen = (electricTraceWeight & 536870912) != 536870912;
            bool aPhaseOpen = (electricTraceWeight & 268435456) != 268435456;

            bool traceThrough = false;
            if (aPhase && aPhaseOpen) { traceThrough = true; }
            else if (bPhase && bPhaseOpen) { traceThrough = true; }
            else if (cPhase && cPhaseOpen) { traceThrough = true; }

            return traceThrough;
        }

       

        private class SSDInformation
        {
            public int ObjectClassID;
            public int ObjectID;
            public int EID;
            public object OperatingNumber;
            public object GlobalID;
            public bool IsSSD;
            public bool IsAutoProtectiveSSD;
            public bool IsProtectiveSSD;
        }

        struct SSDToUpdate
        {
            public bool updateSSD;
            public bool updateProtectiveSSD;
            public bool updateAutoProtectiveSSD;
            public object currentSSD;
            public object currentProtectiveSSD;
            public object currentAutoProtectiveSSD;
            public object newSSD;
            public object newSSDGUID;
            public object newProtectiveSSD;
            public object newAutoProtectiveSSD;
        }
    }
}
