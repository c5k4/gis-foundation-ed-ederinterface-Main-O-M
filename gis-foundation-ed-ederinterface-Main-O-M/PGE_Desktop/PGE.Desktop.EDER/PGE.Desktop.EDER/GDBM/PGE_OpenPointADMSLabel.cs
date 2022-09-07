using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Miner.Process.GeodatabaseManager.ActionHandlers;
using ESRI.ArcGIS.Geodatabase;
using Miner.Framework.Trace.Strategies;
using Miner.Geodatabase;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geometry;
using System.Runtime.InteropServices;
using Miner.Interop;
using ESRI.ArcGIS.NetworkAnalysis;
using System.Reflection;
using System.Xml;
using System.IO;
using ESRI.ArcGIS.GeoDatabaseDistributed;

using PGE.Common.Delivery.Systems.Configuration;
using PGE.Common.Delivery.Geodatabase.ChangeDetection;
using PGE.Common.Delivery.Framework.FeederManager;
using PGE.Common.Delivery.Framework;

namespace PGE.Desktop.EDER.GDBM
{

    /// <summary>
    /// This Action handler is responsible for updating OpenPoint Elbow features ADMSLabel that have been touched by
    /// feeder manager 2.0.  This will ensure that the sde.default version will always have the correct admslabel value.
    /// 
    /// </summary>
    public class PGE_OpenPointADMSLabel : PxActionHandler
    {
        /// <summary>
        /// Logger to log error/debug messages
        /// REMOVE THIS WHEN MOVING TO THE ACTION HANDLER - HANDLER USED this.log
        /// </summary>
        //private static readonly Log4NetLogger _logger = new Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "EDERDesktop.log4net.config");
        // private static IWorkspace _workspace = null;

        #region Class Variables
        private static List<int> _openPointOIDListFULL = new List<int>();
        private static readonly string _structureGUID = "STRUCTUREGUID";
        private static readonly string _globalID = "GlobalID";
        private static readonly string _deviceGroupType = "DEVICEGROUPTYPE";
        private static readonly string _subtypeCD = "SUBTYPECD";
        private static readonly string _electricDistNetworkJunctions = "EDGIS.ELECTRICDISTNETWORK_JUNCTIONS";
        private static readonly string _admsLabelField = "ADMSLABEL";

        internal static string[] _modelNames = {SchemaInfo.Electric.ClassModelNames.CapacitorBank,
                                               SchemaInfo.Electric.ClassModelNames.DynamicProtectiveDevice,
                                               SchemaInfo.Electric.ClassModelNames.Fuse,
                                               SchemaInfo.Electric.ClassModelNames.PGENetworkProtector,
                                               SchemaInfo.Electric.ClassModelNames.PGEOpenPoint,
                                               SchemaInfo.Electric.ClassModelNames.StepDown,
                                               SchemaInfo.Electric.ClassModelNames.PGESwitch,
                                               SchemaInfo.Electric.ClassModelNames.PGETransformer,
                                               SchemaInfo.Electric.ClassModelNames.VoltageRegulator};

        private bool _initializedXMLConfiguration = false;

        #endregion

        /// <summary>
        /// Constructor.
        /// </summary>
        public PGE_OpenPointADMSLabel()
        {
            this.Name = "PGE ADMSLabel Updater";
            this.Description = "Updates the Open Point ADMSLabel values for any features the may have been updated by feeder manager 2.0";
        }

        #region GDBM



        /// <summary>
        /// Function called by GDBM to execute handler.
        /// </summary>
        /// <param name="actionData"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        protected override bool PxSubExecute(Miner.Process.GeodatabaseManager.PxActionData actionData, Miner.Geodatabase.GeodatabaseManager.Serialization.GdbmParameter[] parameters)
        {
            if ((new CommonFunctions()).CheckUsage(actionData.Version, this.Log, ServiceConfiguration) == true)
            {
                return true;
            }
#if DEBUG
            System.Diagnostics.Debugger.Launch();
#endif
            MemoryUsage("PxSubExecute");

            GC.Collect();
            //   GC.WaitForPendingFinalizers();
            ModelNameFacade.ModelNameManager = Miner.Geodatabase.ModelNameManager.Instance;

            MemoryUsage("PxSubExecute 2");
            bool success = true;
            bool inEditOperation = false;
            DifferenceManager differenceManager = null;
            DifferenceDictionary updatedDictionary = null;
            DifferenceDictionary insertDictionary = null;
            DifferenceDictionary deleteDictionary = null;
            IWorkspace Editor = (IWorkspace)actionData.Version;
            IWorkspaceEdit wkspcEdit = (IWorkspaceEdit)actionData.Version;
            //  _workspace = Editor;
            this.Log.Info(ServiceConfiguration.Name, "PGE_OpenPointADMSLabel v 1.365 ");
            this.Log.Info(ServiceConfiguration.Name, "ADMS Label Update on Version: " + actionData.Version.VersionName);

            try
            {
                if (!wkspcEdit.IsBeingEdited())
                {
                    wkspcEdit.StartEditing(false);
                    this.Log.Debug(ServiceConfiguration.Name, "ADMS Label Starting editing of workspace");
                }
                else
                {
                    this.Log.Debug(ServiceConfiguration.Name, "ADMS Label Editing already in progress.");
                }

                if (wkspcEdit.IsBeingEdited())
                {
                    this.Log.Info(ServiceConfiguration.Name, "Determining features edited for ADMSLabel");

                    Dictionary<int, List<int>> editedOIDsByClassID = FeederManager2.GetEditedFeatures((IFeatureWorkspace)actionData.Version, actionData.VersionName);
                    Dictionary<int, List<int>> updatedOIDsByClassID = new Dictionary<int, List<int>>(editedOIDsByClassID);

                    //Updates aren't necessarily reported in the so we'll add them here.
                    this.Log.Info(ServiceConfiguration.Name, "Determining feature updates (ADMS)");
                    IVersionedWorkspace versionWS = actionData.Version as IVersionedWorkspace;

                    #region "Get differences"
                    differenceManager = new DifferenceManager(versionWS.DefaultVersion, actionData.Version);
                    differenceManager.Initialize();
                    differenceManager.LoadDifferences(false, esriDataChangeType.esriDataChangeTypeInsert, esriDataChangeType.esriDataChangeTypeUpdate, esriDataChangeType.esriDataChangeTypeDelete);

                    updatedDictionary = differenceManager.Updates;
                    insertDictionary = differenceManager.Inserts;
                    deleteDictionary = differenceManager.Deletes;

                    Dictionary<string, DiffTable>.Enumerator deletes = deleteDictionary.GetEnumerator();
                    while (deletes.MoveNext())
                    {
                        int currentObjectClassID = -1;
                        IEnumerable<IRow> updatedRows = differenceManager.GetDeletes(deletes.Current.Key, "");
                        IEnumerator<IRow> updatesRowsEnum = updatedRows.GetEnumerator();
                        //updatesRowsEnum.Reset();
                        while (updatesRowsEnum.MoveNext())
                        {
                            if (currentObjectClassID < 0)
                            {
                                currentObjectClassID = ((IObjectClass)updatesRowsEnum.Current.Table).ObjectClassID;
                            }
                            try
                            {

                                if (RowOfInsterest(updatesRowsEnum.Current))
                                {
                                    editedOIDsByClassID[currentObjectClassID].Add(updatesRowsEnum.Current.OID);
                                }
                            }
                            catch
                            {
                                editedOIDsByClassID.Add(currentObjectClassID, new List<int>());
                                if (RowOfInsterest(updatesRowsEnum.Current))
                                    editedOIDsByClassID[currentObjectClassID].Add(updatesRowsEnum.Current.OID);
                            }
                        }
                    }
                    this.Log.Info(ServiceConfiguration.Name, "Delete Count: " + editedOIDsByClassID.Count());
                    Dictionary<string, DiffTable>.Enumerator inserts = insertDictionary.GetEnumerator();
                    while (inserts.MoveNext())
                    {
                        int currentObjectClassID = -1;
                        IEnumerable<IRow> updatedRows = differenceManager.GetInserts(inserts.Current.Key, "");
                        IEnumerator<IRow> updatesRowsEnum = updatedRows.GetEnumerator();
                        //updatesRowsEnum.Reset();
                        while (updatesRowsEnum.MoveNext())
                        {
                            if (currentObjectClassID < 0)
                            {
                                currentObjectClassID = ((IObjectClass)updatesRowsEnum.Current.Table).ObjectClassID;
                            }
                            try
                            {
                                if (RowOfInsterest(updatesRowsEnum.Current))
                                    editedOIDsByClassID[currentObjectClassID].Add(updatesRowsEnum.Current.OID);
                            }
                            catch
                            {
                                editedOIDsByClassID.Add(currentObjectClassID, new List<int>());
                                if (RowOfInsterest(updatesRowsEnum.Current))
                                    editedOIDsByClassID[currentObjectClassID].Add(updatesRowsEnum.Current.OID);
                            }
                        }
                    }
                    this.Log.Info(ServiceConfiguration.Name, "Insert and Delete Count: " + editedOIDsByClassID.Count());

                    Dictionary<string, DiffTable>.Enumerator updates = updatedDictionary.GetEnumerator();
                    while (updates.MoveNext())
                    {
                        int currentObjectClassID = -1;
                        IEnumerable<IRow> updatedRows = differenceManager.GetUpdates(updates.Current.Key, "");
                        IEnumerator<IRow> updatesRowsEnum = updatedRows.GetEnumerator();
                        //updatesRowsEnum.Reset();
                        while (updatesRowsEnum.MoveNext())
                        {

                            if (currentObjectClassID < 0)
                            {
                                currentObjectClassID = ((IObjectClass)updatesRowsEnum.Current.Table).ObjectClassID;
                            }
                            try
                            {
                                if (RowOfInsterest(updatesRowsEnum.Current))
                                {
                                    updatedOIDsByClassID[currentObjectClassID].Add(updatesRowsEnum.Current.OID);
                                }

                            }
                            catch
                            {
                                updatedOIDsByClassID.Add(currentObjectClassID, new List<int>());
                                if (RowOfInsterest(updatesRowsEnum.Current))
                                    updatedOIDsByClassID[currentObjectClassID].Add(updatesRowsEnum.Current.OID);
                            }
                        }
                    }
                    this.Log.Info(ServiceConfiguration.Name, "I/D/U Count: " + editedOIDsByClassID.Count());
                    #endregion

                    //Now that we have our list of class IDs and object IDs that were edited by feeder manager 2.0 we need to update their adms label
                    //device IDs.

                    //ToDo
                    //1) Determine affected circuits from list above.  At the same time map ClassID-OID -> Current adms label device
                    //2) Perform a downstream trace of each circuit once, and put it in order like the cached trace functionality.
                    //3) Build current ClassID-OID -> adms label map
                    //4) Compare the two and determine what (if any) have changed and update those features alone.


                    if (editedOIDsByClassID.Count > 0 || updatedOIDsByClassID.Count > 0)
                    {
                        wkspcEdit.StartEditOperation();
                        inEditOperation = true;

                        //Get a list of circuits that may have been affected in this version.
                        List<IGeometricNetwork> networksAffected = new List<IGeometricNetwork>();
                        List<int> admsLabelDeviceClassIDs = new List<int>();
                        List<string> affectedCircuits = AffectedCircuits(editedOIDsByClassID, Editor, ref networksAffected, ref admsLabelDeviceClassIDs);

                        // Checking Updated circuits now.
                        if (updatedOIDsByClassID.Count > 0)
                        {
                            //   this.Log.Info(ServiceConfiguration.Name, "Staring Updated Circuits.");
                            List<string> updatedCircuits = AffectedCircuits(updatedOIDsByClassID, Editor, ref networksAffected, ref admsLabelDeviceClassIDs);
                            List<string> newList = LoadConfiguration(actionData.Version.VersionName, (IWorkspace)actionData.Version, updatedCircuits, affectedCircuits, ref updatedOIDsByClassID);
                            //   this.Log.Info(ServiceConfiguration.Name, "Staring Updated Circuits list u: " + string.Join(",", updatedCircuits.ToArray()));
                            //   this.Log.Info(ServiceConfiguration.Name, "Staring Updated Circuits list a: " + string.Join(",", affectedCircuits.ToArray()));

                            if (newList.Count() > 0)
                            {
                                if (newList[0].Contains("ERROR"))
                                {
                                    // Load updates by diff manager.
                                    affectedCircuits.AddRange(updatedCircuits);
                                    affectedCircuits = affectedCircuits.Distinct().ToList();
                                }
                                else
                                {
                                    affectedCircuits.AddRange(newList);
                                }
                            }
                            this.Log.Info(ServiceConfiguration.Name, "Complted Updated Circuits " + newList.Count() + " : " + updatedCircuits.Count());
                        }

                        this.Log.Info(ServiceConfiguration.Name, "Circuits affected in this version (ADMS):" + affectedCircuits.Count);

                        //Now we have our list of circuits and networks. We need to trace them and order them.
                        DateTime startDate = DateTime.Now;
                        bool bfinished = true;
                        this.Log.Info(ServiceConfiguration.Name, "Check Workspace.");
                        if (!Editor.Exists())
                        {
                            this.Log.Info(ServiceConfiguration.Name, "Workspace Reset.");
                            Editor = (IWorkspace)actionData.Version;
                        }
                        GC.KeepAlive(Editor);

                        foreach (string s in affectedCircuits)
                        {
                            bool bgood = LoopCircuitsBatch(Editor, s);
                            if (!bgood)
                                bfinished = false;
                        }

                        if (bfinished)
                            this.Log.Info(ServiceConfiguration.Name, "ADMSLabel Finished Successfully. M:" + GC.GetTotalMemory(false).ToString());
                        else
                            this.Log.Info(ServiceConfiguration.Name, "ADMSLabel Finished with issues.M:" + GC.GetTotalMemory(false).ToString());

                        this.Log.Debug(ServiceConfiguration.Name, "ADMS Label Finished loop:" + affectedCircuits.Count);

                        TimeSpan interval = DateTime.Now - startDate;
                        this.Log.Debug(ServiceConfiguration.Name, "ADMS Label ran for (seconds):" + interval.TotalSeconds);


                        try
                        {
                            this.Log.Info(ServiceConfiguration.Name, "ADMS Label stopping edit operation.");
                            wkspcEdit.StopEditOperation();
                        }
                        catch (System.Exception ex)
                        {
                            this.Log.Info(ServiceConfiguration.Name, "ERROR: StopEditOp " + ex.Message + " : " + ex.StackTrace.ToString());
                            throw ex;
                        }
                        inEditOperation = false;
                        success = true;
                    }
                    else
                    {
                        // B6CF-Bug update. No ADMS record(s) to update/review. 
                        success = true;
                    }

                    this.Log.Info(ServiceConfiguration.Name, "ADMS Label Returning  " + success);
                    return success;
                }
                else
                {
                    this.Log.Error(ServiceConfiguration.Name, "Error during ADMS Label processing: Version not being edited");
                }
            }
            catch (Exception ex)
            {
                success = false;
                this.Log.Error(ServiceConfiguration.Name, "Error during ADMS label processing: " + ex.Message + " StackTrace: " + ex.StackTrace.ToString());
                if (inEditOperation) { wkspcEdit.AbortEditOperation(); }
                wkspcEdit.StopEditing(false);
                wkspcEdit.StartEditing(false);

            }
            finally
            {
                if (success)
                {
                    this.Log.Info(ServiceConfiguration.Name, "ADMS Label Ended.");
                    wkspcEdit.StopEditing(true);
                    wkspcEdit.StartEditing(false);
                }
            }

            try
            {
                Miner.Geodatabase.GeodatabaseManager.Serialization.GdbmParameter[] pParamNew = new Miner.Geodatabase.GeodatabaseManager.Serialization.GdbmParameter[2];

                int iParamNew = 0;
                for (int iParam = 0; iParam < parameters.Length; ++iParam)
                {
                    if (parameters[iParam].Name.ToUpper() == "FromState".ToUpper() || parameters[iParam].Name.ToUpper() == "ToState".ToUpper())
                    {
                        pParamNew[iParamNew] = new Miner.Geodatabase.GeodatabaseManager.Serialization.GdbmParameter();
                        pParamNew[iParamNew].Name = parameters[iParam].Name;
                        pParamNew[iParamNew].Value = parameters[iParam].Value;
                        pParamNew[iParamNew].Type = parameters[iParam].Type;

                        ++iParamNew;
                    }
                }
                if (!success)
                {
                    Type transitionType = Type.GetType("Miner.Process.GeodatabaseManager.ActionHandlers.TransitionPxNodeState, Miner.Process, Version=10.8.0.0, Culture=neutral, PublicKeyToken=196beceb052ed5dc");
                    PxActionHandler transitionHandler = Activator.CreateInstance(transitionType, true) as PxActionHandler;
                    transitionHandler.ServiceConfiguration = this.ServiceConfiguration;
                    transitionHandler.ActionName = this.ActionName;
                    transitionHandler.ActionType = this.ActionType;
                    transitionHandler.Execute(actionData, pParamNew);
                }
            }
            catch (Exception ex)
            {
                this.Log.Error(ServiceConfiguration.Name, "Failed to transition the node for ADMSLabel errors " + ex.Message);
            }
            return success;
        }

        /// <summary>
        /// Only want to do circuits that have had operatingnumber manipulation.
        /// </summary>
        /// <param name="row"></param>
        /// <returns></returns>
        private bool RowOfInsterest(IRow row)
        {
            MemoryUsage("RowOfInsterest");
            IObjectClass oc = row.Table as IObjectClass;
            bool result = false;
            if (ModelNameManager.Instance.ContainsClassModelName(oc, SchemaInfo.Electric.ClassModelNames.OperatingNumber)
                || ModelNameManager.Instance.ContainsClassModelName(oc, SchemaInfo.Electric.ClassModelNames.DeviceGroup))
            {
                result = true;
            }
            return result;
        }
        private List<string> AffectedCircuits(Dictionary<int, List<int>> editedOIDsByClassID, IWorkspace workspace,
          ref List<IGeometricNetwork> affectedNetworks, ref List<int> admsDeviceClassIDs)
        {
            MemoryUsage("AffectedCircuits");
            List<string> affectedCircuits = new List<string>();
            //Get list of feature classes with the PGE_admsDeviceID
            IFeatureWorkspace featWorkspace = workspace as IFeatureWorkspace;


            List<string> classNameList = new List<string>();
            //Get a list of feature class with the oper num or admslabel and then iterate over that networks feature classes for classes with the operating number/adms ID field.
            IEnumBSTR classNamesForADMS = ModelNameManager.Instance.ClassNamesFromModelNameWS(workspace, SchemaInfo.Electric.ClassModelNames.OperatingNumber);

            classNamesForADMS.Reset();
            string admsname = "";
            while (!string.IsNullOrEmpty((admsname = classNamesForADMS.Next())))
            {
                classNameList.Add(admsname);
            }
            classNamesForADMS = ModelNameManager.Instance.ClassNamesFromModelNameWS(workspace, SchemaInfo.Electric.ClassModelNames.DeviceGroup);

            classNamesForADMS.Reset();
            admsname = "";
            while (!string.IsNullOrEmpty((admsname = classNamesForADMS.Next())))
            {
                classNameList.Add(admsname);
            }

            List<string> networksProcessed = new List<string>();
            foreach (string admsDeviceClass in classNameList)
            {
                List<ITable> classesToProcess = new List<ITable>();
                IFeatureClass admsDevice = featWorkspace.OpenFeatureClass(admsDeviceClass);

                try
                {
                    string networkName = "";
                    if (admsDevice != null && admsDevice is INetworkClass)
                    {
                        admsDeviceClassIDs.Add(admsDevice.ObjectClassID);
                        INetworkClass networkClass = admsDevice as INetworkClass;
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
                                            this.Log.Warn(ServiceConfiguration.Name, "Unable to determine circuit ID for " + ((IDataset)featClass).BrowseName + " OID: " + feat.OID);
                                        }
                                        catch { }
                                    }

                                    while (Marshal.ReleaseComObject(feat) > 0) { }
                                }
                                if (cursor != null) { while (Marshal.ReleaseComObject(cursor) > 0) { } }
                                if (qf != null) { while (Marshal.ReleaseComObject(qf) > 0) { } }
                            }
                        }
                        if (featClass != null) { while (Marshal.ReleaseComObject(featClass) > 0) { } }
                    }
                }
                finally
                {
                    if (admsDevice != null) { while (Marshal.ReleaseComObject(admsDevice) > 0) { } }
                    foreach (ITable table in classesToProcess)
                    {
                        if (table != null) { while (Marshal.ReleaseComObject(table) > 0) { } }
                    }
                }
            }

            return affectedCircuits.Distinct().ToList();
        }

        private List<ITable> GetNetworkClasses(esriFeatureType featureType, IGeometricNetwork geomNetwork)
        {
            List<ITable> featureClasses = new List<ITable>();

            IEnumFeatureClass networkClasses = geomNetwork.get_ClassesByType(featureType);
            networkClasses.Reset();
            IFeatureClass netFeatClass = null;
            while ((netFeatClass = networkClasses.Next()) != null)
            {
                //IEnumBSTR fields = ModelNameManager.Instance.FieldNamesFromModelName(netFeatClass, SchemaInfo.Electric.FieldModelNames.admsLabelDeviceId);
                //string field = "";
                //if (!string.IsNullOrEmpty((field = fields.Next())))
                //{
                featureClasses.Add(netFeatClass as ITable);
                //}
            }
            return featureClasses;
        }


        #endregion

        #region Main
        /// <summary>
        /// Get features with operating number field or DeviceGroup features.
        /// </summary>
        /// <param name="workspace"></param>
        /// <returns></returns>
        private List<int> GetAdmsDeviceClassIDs(IWorkspace workspace)
        {
            MemoryUsage("GetAdmsDeviceClassIDs");
            IFeatureWorkspace featWorkspace = workspace as IFeatureWorkspace;
            List<int> result = new List<int>();

            List<string> classNameList = new List<string>();
            // Get a list of feature class with the adms device ID and then iterate over that networks feature classes for classes with the operating number or admslabel field.
            IEnumBSTR classNamesForADMS = ModelNameManager.Instance.ClassNamesFromModelNameWS(workspace, SchemaInfo.Electric.ClassModelNames.OperatingNumber);

            classNamesForADMS.Reset();
            string admsname = "";
            while (!string.IsNullOrEmpty((admsname = classNamesForADMS.Next())))
            {
                classNameList.Add(admsname);
            }

            // Add DeviceGroup Featureclass too.
            classNamesForADMS = ModelNameManager.Instance.ClassNamesFromModelNameWS(workspace, SchemaInfo.Electric.ClassModelNames.DeviceGroup);

            classNamesForADMS.Reset();
            admsname = "";
            while (!string.IsNullOrEmpty((admsname = classNamesForADMS.Next())))
            {
                classNameList.Add(admsname);
            }

            List<string> networksProcessed = new List<string>();
            foreach (string admsDeviceClass in classNameList)
            {
                List<ITable> classesToProcess = new List<ITable>();
                IFeatureClass admsDevice = featWorkspace.OpenFeatureClass(admsDeviceClass);

                try
                {
                    if ((admsDevice != null) && (admsDevice is INetworkClass))
                    {
                        result.Add(admsDevice.ObjectClassID);
                    }
                }
                catch
                {
                    // Ignore for now.
                }
            }

            return result;
        }

        /// <summary>
        /// Loads initial xml file
        /// </summary>
        private List<string> LoadConfiguration(string versionName, IWorkspace workspace, List<string> affectedCircuits,
            List<string> currentCircuits, ref Dictionary<int, List<int>> updatedOIDsByClassID)
        {
            List<string> result = new List<string>();
            if (!_initializedXMLConfiguration)
            {
                _initializedXMLConfiguration = true;
                //Read the config location from the Registry Entry.
                SystemRegistry sysRegistry = new SystemRegistry("PGE");
                string xmlConfigFilePath = sysRegistry.ConfigPath;
                //Prepare the xmlfile if config path exist.
                string xmlConfigXmlFilePath = System.IO.Path.Combine(xmlConfigFilePath, "PGE_OpenPointADMSLabel_Config.xml");

                if (!System.IO.File.Exists(xmlConfigXmlFilePath))
                {
                    string assemblyLoc = Assembly.GetExecutingAssembly().Location;
                    assemblyLoc = assemblyLoc.Replace(Assembly.GetExecutingAssembly().GetName().Name + ".dll", "");

                    xmlConfigXmlFilePath = System.IO.Path.Combine(assemblyLoc, "PGE_OpenPointADMSLabel_Config.xml");
                }
                this.Log.Debug(ServiceConfiguration.Name, "Config Path: " + xmlConfigXmlFilePath);

                try
                {
                    if (File.Exists(xmlConfigXmlFilePath))
                    {
                        // Remove circuits in the update list that are already in the insert/delete list.
                        List<string> updatedCircuits = new List<string>();
                        foreach (string c in affectedCircuits)
                        {
                            if (!currentCircuits.Contains(c))
                                updatedCircuits.Add(c);
                        }
                        this.Log.Info(ServiceConfiguration.Name, "LoadConfig Counts " + affectedCircuits.Count() + " : " + updatedCircuits.Count() + " : " + currentCircuits.Count());

                        if (updatedCircuits.Count() > 0)
                        {
                            this.Log.Info(ServiceConfiguration.Name, "LoadConfig have count. " + affectedCircuits.Count() + " : " + updatedCircuits.Count());

                            result = CheckUpdatedCircuits(versionName, xmlConfigXmlFilePath, updatedCircuits, workspace);
                        }

                        this.Log.Info(ServiceConfiguration.Name, "LoadConfig Final Cnt: " + result.Count());

                    }
                    return result;
                }
                catch (Exception ex)
                {
                    this.Log.Error(ServiceConfiguration.Name, "Warning loading ADMSLabel configuration: " + ex.Message + " : " + ex.StackTrace.ToString() + " : Path:" + xmlConfigFilePath);
                    result = new List<string>();
                    result.Add("ERROR With Config.");
                    return result;
                }
            }
            return result;
        }

        /// <summary>
        /// BATCH.
        /// Loop through each circuit to collect label.
        /// </summary>
        /// <param name="targetWorkspace"></param>
        /// <param name="networksAffected"></param>
        /// <param name="CircuitsAffected">List of circuits that have updates</param>
        private bool LoopCircuitsBatch(IWorkspace targetWorkspace, string circuitID)
        {
            MemoryUsage("LoopCircuitsBatch");
            bool bgood = false;
            List<bool> boolList = new List<bool>();
            try
            {
                IGeometricNetwork geometricNetwork = LabelHelpers.GetNetworks(targetWorkspace);
                if (geometricNetwork == null)
                    this.Log.Warn(ServiceConfiguration.Name, "Issue getting GeometricNetwork.");

                this.Log.Info(ServiceConfiguration.Name, "ADMSLabel reviewing CircuitID " + circuitID + ".");

                DateTime startDate = DateTime.Now;

                bgood = CollectElbows(circuitID, geometricNetwork, targetWorkspace);
                boolList.Add(bgood);

                if (!bgood)
                {
                    this.Log.Warn(ServiceConfiguration.Name, "CircuitID " + circuitID + " did not process properly.");
                }
                else
                {
                    this.Log.Info(ServiceConfiguration.Name, "CircuitID " + circuitID + " Finished Succefully.");
                }

                // Show time for Circuit run.
                TimeSpan interval = DateTime.Now - startDate;
                this.Log.Info(ServiceConfiguration.Name, "CircuitID " + circuitID + " Elapsed Time: " + interval.TotalSeconds);

                bgood = !boolList.Contains(false);

                LabelGroup.Init();
                //   GC.Collect();
                //   GC.WaitForPendingFinalizers();

                ModelNameFacade.ModelNameManager = Miner.Geodatabase.ModelNameManager.Instance;
                return bgood;
            }
            catch (System.Exception ex)
            {
                this.Log.Error(ServiceConfiguration.Name, "ADMSLabel LoopCircuit " + ex.Message + " Stack: " + ex.StackTrace.ToString());
                bgood = false;
                throw ex;
            }
        }

        /// <summary>
        /// Get all the openpoint elbows in this circuit.
        /// </summary>
        /// <param name="circuitID"></param>
        /// <param name="targetWorkspace"></param>
        /// <returns></returns>
        private bool CollectElbows(string circuitID, IGeometricNetwork network, IWorkspace targetWorkspace)
        {
            MemoryUsage("CollectElbows");
            bool result = false;
            try
            {
                // Initialize object to hold items.
                LabelGroup.Init();

                // We're going to do the update one each circuit.
                Dictionary<int, string> updateFeatures = new Dictionary<int, string>();
                //  this.Log.Info(ServiceConfiguration.Name, "CollectElbows Start.");
                List<int> openPntOIDs = ElbowsInCircuit(circuitID, network, targetWorkspace);
                // _openPointOIDListFULL.AddRange(openPntOIDs);
                //      this.Log.Info(ServiceConfiguration.Name, "CollectElbows OpenPoints " + openPntOIDs.Count() + " M:" + GC.GetTotalMemory(false).ToString());
                openPntOIDs.Clear();
                try
                {
                    //  result = FindLabels(circuitID, network, targetWorkspace);
                    result = true;
                }
                catch { }

                return result;
            }
            catch (System.Exception ex)
            {
                this.Log.Error(ServiceConfiguration.Name, "ADMSLabel CollectElbows " + ex.Message + " Stack: " + ex.StackTrace.ToString());
                throw ex;
            }
        }

        /// <summary>
        /// Optional third label - occures when 3 or more edges connect and a non-qualifing featurclass
        ///   and the trace flow is in the same direction as the ending label feature.
        ///     ex: T1234 TWD J4321 & [VIA Value]
        /// </summary>
        /// <param name="li"></param>
        /// <param name="direction"></param>
        /// <param name="geomNetwork"></param>
        /// <param name="chkEIDs"></param>
        /// <returns></returns>
        private SelItem CheckforViaLabel(LabelItems li, string direction, IGeometricNetwork geomNetwork, List<int> chkEIDs, IWorkspace workspace)
        {
            MemoryUsage("CheckforViaLabel");
            SelItem result = new SelItem(this.Log, this.ServiceConfiguration);
            IForwardStarGEN forwardStar = null;
            try
            {
                // this.Log.Debug(ServiceConfiguration.Name, "CheckforViaLabel started.");

                if (li.Full && (direction.Length > 0))
                {
                    //  this.Log.Debug(ServiceConfiguration.Name, "CheckforViaLabel I.");

                    List<int> junctionEIDs = new List<int>();
                    List<int> edgeEIDS = new List<int>();

                    // Tracing again to get shorts amount of data to scroll through.
                    TraceForExtraEdges(geomNetwork, li.NextItem.EID, ref junctionEIDs, ref edgeEIDS, ref li, direction);
                    //   this.Log.Debug(ServiceConfiguration.Name, "CheckforViaLabel II.");
                    INetWeight netWeight = GetNetworkWeight(geomNetwork);

                    forwardStar = geomNetwork.Network.CreateForwardStar(false, netWeight, netWeight, netWeight, null) as IForwardStarGEN;
                    //   this.Log.Debug(ServiceConfiguration.Name, "CheckforViaLabel III.");
                    int adjacentEdgesCount;
                    // DEBUG 
                    int maxcnt = 2;

                    if (li.Is50Scale)
                        maxcnt = 1;

                    int[] adjacentEdgeEIDS;
                    bool[] reverseOrientationEdge;
                    object[] weightValuesJunctions;
                    //   this.Log.Debug(ServiceConfiguration.Name, "CheckforViaLabel IV.");
                    for (int i = 0; i < junctionEIDs.Count; i++)
                    {
                        adjacentEdgesCount = -1;
                        forwardStar.FindAdjacent(0, junctionEIDs[i], out adjacentEdgesCount);

                        adjacentEdgeEIDS = new int[adjacentEdgesCount];
                        reverseOrientationEdge = new bool[adjacentEdgesCount];
                        weightValuesJunctions = new object[adjacentEdgesCount];

                        forwardStar.QueryAdjacentEdges(ref adjacentEdgeEIDS, ref reverseOrientationEdge, ref weightValuesJunctions);

                        // We only want branches that are primary conductors.
                        adjacentEdgeEIDS = OnlyPrimaries(adjacentEdgeEIDS, reverseOrientationEdge, geomNetwork);

                        // If only two edges this won't be a via.
                        if (adjacentEdgeEIDS.Length > 2)
                        {
                            List<SelItem> selList = new List<SelItem>();
                            for (int e = 0; e < adjacentEdgeEIDS.Length; e++)
                            {
                                if (!edgeEIDS.Contains(adjacentEdgeEIDS[e]))
                                {
                                    // An edge we didn't use in the intial flow.
                                    int count = TraversAnEdge2(workspace, geomNetwork, adjacentEdgeEIDS[e], li, ref selList, ref junctionEIDs);
                                    bool badd = true;

                                    foreach (SelItem s in selList)
                                    {
                                        if (li.ViaList.Count() == maxcnt)
                                            break;

                                        // Check if value is already in list.
                                        badd = true;
                                        foreach (SelItem v in li.ViaList)
                                        {
                                            if (v.Label == s.Label)
                                            {
                                                badd = false;
                                                break;
                                            }
                                        }

                                        // Just insuring we don't add the beginning ones again.
                                        if ((s.Label == li.NextItem.Label) || (s.Label == li.StartItem.Label))
                                            badd = false;

                                        if (badd)
                                            li.ViaList.Add(s);
                                    }

                                    // Only want # via/and features. 
                                    if (li.ViaList.Count() == maxcnt)
                                        break;
                                }
                            }

                        }
                        if ((result != null) && (result.FCName != null))
                            break;
                    }

                    adjacentEdgeEIDS = null;
                    reverseOrientationEdge = null;
                    weightValuesJunctions = null;
                    junctionEIDs.Clear();
                    edgeEIDS.Clear();

                }


                //     this.Log.Debug(ServiceConfiguration.Name, "CheckforViaLabel ended.");
                return result;
            }
            catch (System.Exception ex)
            {
                this.Log.Error(ServiceConfiguration.Name, "Error during ADMS Label processing: CheckForViaLabel " + ex.Message + " Stack: " + ex.StackTrace.ToString());
                return result;
            }
            finally
            {
                if (forwardStar != null) { while (Marshal.ReleaseComObject(forwardStar) > 0) ; }
                //   GC.Collect();
            }
        }



        /// <summary>
        /// Build ADMSlabel Value.
        /// </summary>
        /// <param name="circuitID"></param>
        /// <param name="network"></param>
        /// <param name="targetWorkspace"></param>
        /// <returns></returns>
        private bool FindLabels(string circuitID, IGeometricNetwork network, IWorkspace targetWorkspace)
        {
            bool result = true;
            try
            {
                // We're going to do the update one each circuit.
                Dictionary<int, string> updateFeatures = new Dictionary<int, string>();

                // Use less connected features for 50 scale.
                // Only checking circuit against first elbow, it may fall outside/inside 50 poly while others do not.
                bool is50scale = false;
                if (LabelGroup.Items.Count > 0)
                {
                    is50scale = LabelHelpers.IsFiftyScale(LabelGroup.Items[0].CenterItem.Feature.Shape, targetWorkspace);
                    //   this.Log.Info(ServiceConfiguration.Name, "Circuit is 50 scale: " + is50scale);
                }

                List<int> junctionEIDs;
                List<int> edgeEIDS;

                List<int> admsLabelDeviceClassIDs = GetAdmsDeviceClassIDs(targetWorkspace);

                string dir = string.Empty;
                SelItem si = null;

                // Loop our collection.
                foreach (LabelItems l in LabelGroup.Items)
                {
                    // Update requirements based on 6/30/2021.
                    // If Elbow does not have an OperatingNumber value or a valid relationship, put subtype of elbow in label.
                    // this.Log.Debug(ServiceConfiguration.Name, "Looping Features Center: " + l.CenterItem.OID);

                    string opnumber = FieldHelper.GetFieldValue(l.CenterItem.Feature, SchemaInfo.Electric.FieldModelNames.OperatingNumber, true);
                    if (opnumber.Length > 0)
                    {
                        //    this.Log.Debug(ServiceConfiguration.Name, "ConnectedFeature: " + DateTime.Now);
                        ConnnectedFeature_XX(l.CenterItem.Feature, l, network, ref admsLabelDeviceClassIDs, targetWorkspace);
                        List<int> chkEIDs = new List<int>();
                        // Ending label.
                        //    this.Log.Debug(ServiceConfiguration.Name, "GetNextInOrder: " + DateTime.Now);
                        SelItem siNext = GetNextInOrder(targetWorkspace, network, l, ref admsLabelDeviceClassIDs, ref dir, ref chkEIDs);
                        //    this.Log.Debug(ServiceConfiguration.Name, "GetNextInOrder done: " + DateTime.Now);
                        //    this.Log.Info(this.ServiceConfiguration.Name, "GNIO");
                        if (siNext.FCName != null)
                        {
                            //     this.Log.Info(this.ServiceConfiguration.Name, "Next Found: " + siNext.FCName + " " + siNext.OID);
                            l.NextItem = siNext;
                        }
                        else
                        {
                            //      this.Log.Info(this.ServiceConfiguration.Name, "Next Missing: " + l.CenterItem.OID);
                            l.Comments += "Missing Ending.";
                        }
                        if (l.CenterItem.BaseEdgeEID > 0)
                        {
                            junctionEIDs = new List<int>();
                            edgeEIDS = new List<int>();
                        }


                        // & part(s).
                        //    this.Log.Info(this.ServiceConfiguration.Name, "CV");
                        //    this.Log.Debug(ServiceConfiguration.Name, "CheckforViaLabel: " + DateTime.Now);
                        si = CheckforViaLabel(l, dir, network, chkEIDs, targetWorkspace);
                        // for debuging 
                        //  si = new SelItem(this.Log, this.ServiceConfiguration);
                        //    this.Log.Debug(ServiceConfiguration.Name, "CheckforViaLabel end: " + DateTime.Now);
                        if (si.FCName != null)
                        {
                            l.ViaItem = si;
                            //   theLabel += " ** " + l.ViaItem.Label;
                        }
                    }

                    updateFeatures.Add(l.CenterItem.OID, l.Label());

                }

                // Actual update of field.
                //     this.Log.Info(ServiceConfiguration.Name, "UL");
                if (UpdateOpenPoint(updateFeatures, circuitID, network, targetWorkspace))
                {
                    //      this.Log.Info(ServiceConfiguration.Name, "Updated Label " + updateFeatures.Count() + " on Circuit: " + circuitID);
                    result = true;


                }
                else
                {
                    //    this.Log.Info(ServiceConfiguration.Name, "Warning Unable to update Circuit: " + circuitID);
                    result = false;
                }

                updateFeatures.Clear();
                updateFeatures = new Dictionary<int, string>();
                junctionEIDs = null;
                edgeEIDS = null;
                admsLabelDeviceClassIDs = null;

                return result;
            }
            catch (System.Exception ex)
            {
                this.Log.Error(ServiceConfiguration.Name, "Error during ADMS Label processing: FindLabels " + ex.Message + " Stack: " + ex.StackTrace.ToString());
                throw ex;
            }
            finally
            {
                LabelGroup.Items.Clear();
                LabelGroup.Init();
                GC.Collect();
            }
        }

        /// <summary>
        /// Openpoint can not find it's related/parent feature by normal methods.
        /// Basicaly traces up and down and grabs a feature for the starting label.
        /// The ending label will be traced in the opposite direction from the one used here.
        /// </summary>
        /// <param name="sourceFeature"></param>
        /// <param name="li"></param>
        /// <param name="edgeFeatures"></param>
        /// <param name="geomNetwork"></param>
        /// <param name="admsLabelDeviceClassIDs"></param>
        /// <returns></returns>
        private SelItem GetNonStarter(IFeature sourceFeature, LabelItems li, out List<IFeature> edgeFeatures,
            IGeometricNetwork geomNetwork, List<int> admsLabelDeviceClassIDs, IWorkspace workspace)
        {
            List<IFeature> junctionFeatures;
            edgeFeatures = new List<IFeature>();

            //   this.Log.Debug(ServiceConfiguration.Name, "GetNonStarter started.");

            GetNextJunctions(sourceFeature, out junctionFeatures, out edgeFeatures);
            IFeature startEdge = null;
            IFeature startJunc = null;
            string dir = string.Empty;
            int eid = 0;
            SelItem startItem = new SelItem(this.Log, this.ServiceConfiguration);

            foreach (IFeature feature in edgeFeatures)
            {
                if (ModelNameFacade.ContainsClassModelName(feature.Class, SchemaInfo.Electric.ClassModelNames.PGEBusBar))
                {
                    // BusBars are a common conductor connectiong elbows and other devices, it gets a high priority.
                    startEdge = feature;
                    break;
                }
                else
                {
                    startEdge = feature;
                }
            }

            if ((startEdge != null) && (startEdge is INetworkFeature) && (startEdge.FeatureType == esriFeatureType.esriFTSimpleEdge))
            {
                ISimpleEdgeFeature edge = startEdge as ISimpleEdgeFeature;
                IEdgeFeature edgefeat = startEdge as IEdgeFeature;

                ISimpleJunctionFeature temp = edgefeat.FromJunctionFeature as ISimpleJunctionFeature;

                if (li.CenterItem.EID != edgefeat.FromJunctionEID)// (temp.EID != edgefeat.FromJunctionEID)
                {
                    // Getting From junction on connected conductor.
                    startJunc = edgefeat.FromJunctionFeature as IFeature;
                    eid = edgefeat.FromJunctionEID;
                    dir = "DOWN";
                }
                else if (li.CenterItem.EID != edgefeat.ToJunctionEID)// (temp.EID != edgefeat.ToJunctionEID)
                {
                    // Getting To junction on connected conductor.
                    startJunc = edgefeat.ToJunctionFeature as IFeature;
                    eid = edgefeat.ToJunctionEID;
                    dir = "UP";
                }
            }
            if (eid > 0)
            {
                List<int> ueid = new List<int>();
                List<int> deid = new List<int>();

                if ((ModelNameFacade.ContainsClassModelName(startEdge.Class, SchemaInfo.Electric.ClassModelNames.PGEBusBar)) && ((startJunc.Class as IDataset).Name.ToUpper() == _electricDistNetworkJunctions))
                    startItem = QueryDeviceGroupTransformers(startJunc, 5, sourceFeature, workspace);

                if (startItem.FCName == null)
                {
                    List<int> chkEIDs = new List<int>();
                    startItem = GetNextInOrder(workspace, geomNetwork, li, ref admsLabelDeviceClassIDs, ref dir, ref chkEIDs);
                }

            }
            //    this.Log.Debug(ServiceConfiguration.Name, "GetNonStarter ended.");
            return startItem;
        }

        #endregion

        #region Tracing/Connected
        /// <summary>
        ///  This traces up and down to find the 'ending' feature/label.
        /// </summary>
        /// <param name="network"></param>
        /// <param name="dir">direction' result was found in</param>
        /// <param name="li">collection of items</param>
        /// <param name="admsLabelDeviceClassIDs"></param>
        /// <returns></returns>
        private SelItem GetNextInOrder(IWorkspace workspace, IGeometricNetwork network, LabelItems li, ref List<int> admsLabelDeviceClassIDs, ref string dir, ref List<int> outEID)
        {
            MemoryUsage("GetNextInOrder");
            outEID = new List<int>();
            List<int> downEIDs = new List<int>();
            List<int> upEIDs = new List<int>();
            int downEID = 0;
            int upEID = 0;
            SelItem result = new SelItem(this.Log, this.ServiceConfiguration);
            SelItem si = new SelItem(this.Log, this.ServiceConfiguration);
            List<int> checkedJunctions = new List<int>();

            try
            {
                dir = string.Empty;
                //   this.Log.Debug(ServiceConfiguration.Name, "GetNextInOrder started.");
                if (GetPathEnd(network, li.CircuitID, ref admsLabelDeviceClassIDs, li, out downEIDs, out upEIDs))
                {
                    downEID = downEIDs[0];
                    upEID = upEIDs[0];


                    int maxcnt = downEIDs.Count;
                    if (maxcnt < upEIDs.Count)
                        maxcnt = upEIDs.Count;
                    // Want to limit the number of loops.
                    // Need all the eid's in the list for checking later for VIA/AND features, but 25 times in loop will get us our results here.
                    if (maxcnt > 25)
                        maxcnt = 25;

                    List<int> junctionEIDs = new List<int>();
                    List<int> edgeEIDS = new List<int>();



                    for (int i = 0; i < maxcnt; i++)
                    {
                        junctionEIDs = new List<int>();
                        edgeEIDS = new List<int>();
                        if (downEIDs.Count > i)
                            downEID = downEIDs[i];
                        else
                            downEID = -1;
                        if (upEIDs.Count > i)
                            upEID = upEIDs[i];
                        else
                            upEID = -1;
                        //    this.Log.Info(ServiceConfiguration.Name, "GetNextInOrder: Trace: " + downEID + " -- " + upEID);
                        dir = TraceForDIR2(network, downEID, upEID, ref junctionEIDs, ref edgeEIDS, li);

                        if ((dir == "UP") || (dir == "DOWN"))
                        {
                            //   this.Log.Info(ServiceConfiguration.Name, "Trace direction is " + dir);
                            foreach (int ii in junctionEIDs)
                            {
                                //        this.Log.Info(ServiceConfiguration.Name, "GetNextInOrder: " + ii + " -- " + i);
                                if (!checkedJunctions.Contains(ii))
                                {
                                    si = ValidFeature(ii, li, network, workspace);

                                    if (si.FCName != null)
                                    {
                                        result = si;
                                        result.EID = ii;

                                        // Need eids for edge - VIA/AND check.
                                        outEID = downEIDs;
                                        if (dir == "UP")
                                        {
                                            outEID = upEIDs;
                                            downEIDs.Clear();
                                        }
                                        else
                                            upEIDs.Clear();

                                        break;
                                    }
                                }
                            }
                            checkedJunctions.AddRange(junctionEIDs);
                        }
                        if (result.FCName != null)
                        {
                            //   this.Log.Debug(ServiceConfiguration.Name, "GetNextInOrder result " + result.FCName + " " + result.OID);
                            break;
                        }
                    }

                }
                checkedJunctions.Clear();
                //     this.Log.Debug(ServiceConfiguration.Name, "GetNextInOrder ended.");
                return result;
            }
            catch (System.Exception ex)
            {
                this.Log.Error(ServiceConfiguration.Name, "Error during ADMS Label processing: GetNextInOrder " + ex.Message + " Stack: " + ex.StackTrace.ToString());
                throw ex;
            }
        }

        /// <summary>
        /// This trace will return ONLY the edges/junctions between the start and ending label.
        /// The result is traveresed to find where a seperate junction splits off the 'main' path.
        /// </summary>
        /// <param name="geomNetwork"></param>
        /// <param name="theEID"></param>
        /// <param name="junctionEIDs"></param>
        /// <param name="edgeEIDS"></param>
        /// <param name="li"></param>
        /// <param name="dir"></param>
        /// <returns>true/false</returns>
        private bool TraceForExtraEdges(IGeometricNetwork geomNetwork, int theEID,
            ref List<int> junctionEIDs, ref List<int> edgeEIDS, ref LabelItems li, string dir)
        {
            string result = string.Empty;
            //    this.Log.Debug(ServiceConfiguration.Name, "TraceForExtraEdges started.");

            INetwork network = geomNetwork.Network;

            ITraceFlowSolverGEN traceFlowSolver = new TraceFlowSolverClass() as ITraceFlowSolverGEN;
            INetSolver netSolver = traceFlowSolver as INetSolver;

            try
            {

                netSolver.SourceNetwork = network;

                IEdgeFlag eFlag;
                IJunctionFlag jFlag;
                //    this.Log.Debug(ServiceConfiguration.Name, "TraceForExtraEdges GetFlags.");
                bool b = GetFlags(geomNetwork, li.CenterItem.Feature, out jFlag, out eFlag);

                IJunctionFlag[] junctionFlags = new IJunctionFlag[2];
                // Add the flag to the array.
                junctionFlags[0] = jFlag;

                // Set direction to trace.
                esriFlowMethod flow = esriFlowMethod.esriFMDownstream;

                if (dir == "UP")
                {
                    flow = esriFlowMethod.esriFMUpstream;
                }

                //    this.Log.Debug(ServiceConfiguration.Name, "TraceForExtraEdges GetFeatureFromEID.");
                // Add path end.
                IFeature pathfeat = LabelHelpers.GetFeaturefromEID(theEID, esriElementType.esriETJunction, geomNetwork);
                if (pathfeat == null)
                {
                    this.Log.Warn(ServiceConfiguration.Name, "Warning ADMSLabel TraceForExtraEdges edge: " + theEID + " unable to get feature.");
                    return false;
                }
                //   this.Log.Debug(ServiceConfiguration.Name, "TraceForExtraEdges GetFeatureNetFlag.");
                INetFlag netFlag = LabelHelpers.GetFeatureNetFlag(pathfeat, esriElementType.esriETJunction);
                IJunctionFlag junctionFlag = netFlag as IJunctionFlag;
                junctionFlags[1] = junctionFlag;

                //    this.Log.Debug(ServiceConfiguration.Name, "TraceForExtraEdges PutJunctions.");
                // Adding junctions to limit trace to what we care about and not whole circuit.
                traceFlowSolver.PutJunctionOrigins(ref junctionFlags);

                traceFlowSolver.TraceIndeterminateFlow = true;

                IEnumNetEID outJunctions;
                IEnumNetEID outEdges;

                object[] segments = new object[1];

                //    this.Log.Debug(ServiceConfiguration.Name, "TraceForExtraEdges FindPath.");
                traceFlowSolver.FindPath(flow, esriShortestPathObjFn.esriSPObjFnMinMax, out outJunctions, out outEdges, 1, ref segments);

                // Collect junctions.
                outJunctions.Reset();
                int junctionEID = -1;
                while ((junctionEID = outJunctions.Next()) > 0)
                {
                    if (!junctionEIDs.Contains(junctionEID))
                        junctionEIDs.Add(junctionEID);
                }

                // Collect edges.
                outEdges.Reset();
                junctionEID = -1;
                while ((junctionEID = outEdges.Next()) > 0)
                {
                    if (!edgeEIDS.Contains(junctionEID))
                        edgeEIDS.Add(junctionEID);
                }
                //    this.Log.Debug(ServiceConfiguration.Name, "TraceForExtraEdges ended.");
                return true;
            }
            catch (System.Exception ex)
            {
                this.Log.Error(ServiceConfiguration.Name, "TraceForExtraEdges Error Dir: " + dir + " CircuitID: " + li.CircuitID + " " + li.CenterItem.FCName + "." + li.CenterItem.OID + " Error Message: " + ex.Message + " " + ex.StackTrace.ToString());
                throw ex;
            }
            finally
            {
                if (traceFlowSolver != null) { while (Marshal.ReleaseComObject(traceFlowSolver) > 0) ; }
                if (netSolver != null) { while (Marshal.ReleaseComObject(netSolver) > 0) ; }

            }
        }


        /// <summary>
        /// Given that the elbow feature is tied directly to another feature with a single conductor busbar line.
        /// </summary>
        /// <param name="geomNetwork"></param>
        /// <param name="sourceFeature">Starting OpenPoint</param>
        private void ConnectedFeature(IFeature sourceFeature, LabelItems li, IGeometricNetwork geomNetwork, ref List<int> admsLabelDeviceClassIDs, IWorkspace workspace)
        {
            MemoryUsage("ConnectedFeature");

            List<IFeature> junctionFeatures;
            List<IFeature> edgeFeatures = new List<IFeature>();
            IFeature startingfeat = null;
            bool addToList = true;
            try
            {
                //   this.Log.Debug(ServiceConfiguration.Name, "ConnectedFeature started.");

                List<IFeature> tempfeats = new List<IFeature>();
                startingfeat = null;
                startingfeat = QueryForBusBarEnd(sourceFeature, ref li);

                if (startingfeat == null)
                {
                    // See if structurguid matches transformer.
                    startingfeat = QueryOther(sourceFeature, ref li, workspace);
                    //    this.Log.Info(ServiceConfiguration.Name, "ConnectedFeature qo.");
                }

                if (startingfeat != null)
                {
                    //     this.Log.Info(ServiceConfiguration.Name, "ConnectedFeature sf: " + startingfeat.Class.AliasName + " : " + startingfeat.OID);
                    li.StartItem.LoadFeature(startingfeat, FieldHelper.GetFieldValue(startingfeat, SchemaInfo.Electric.FieldModelNames.OperatingNumber, true));
                    return;
                }

                if ((startingfeat == null))
                {
                    //  this.Log.Debug(ServiceConfiguration.Name, "ConFeat queryOPII: " + sourceFeature.Class.AliasName + " : " + sourceFeature.OID);
                    startingfeat = QueryOpenPoint(sourceFeature, ref tempfeats, ref li, workspace);
                    if (startingfeat != null)
                    {
                        //  this.Log.Debug(ServiceConfiguration.Name, "ConFeat queryOP Return.");
                        return;
                    }
                }

                //if (startingfeat != null)
                //    this.Log.Debug(ServiceConfiguration.Name, "busbar end found ." + startingfeat.Class.AliasName + " : " + startingfeat.OID);

                if (li.CenterItem.CenterStructGUID != null)
                {

                    if (startingfeat == null)
                    {
                        if (startingfeat == null)
                        {
                            //    this.Log.Info(ServiceConfiguration.Name, "ConnectedFeature GUID: " + li.CenterItem.CenterStructGUID);
                            int x = GetNextJunctions(sourceFeature, out junctionFeatures, out edgeFeatures);
                            if (x == 1)
                            {
                                if (ModelNameFacade.ContainsFieldModelName(junctionFeatures[0].Class, SchemaInfo.Electric.FieldModelNames.OperatingNumber))
                                {
                                    startingfeat = junctionFeatures[0];
                                    if (li.StartItem.LoadFeature(startingfeat, FieldHelper.GetFieldValue(startingfeat, SchemaInfo.Electric.FieldModelNames.OperatingNumber, true)))
                                        return;
                                }
                            }

                            tempfeats = new List<IFeature>();
                            foreach (IFeature feature in junctionFeatures)
                            {
                                string sguid = FieldHelper.GetFieldValue(feature, _structureGUID);
                                if ((sguid.Length > 0) && (sguid == li.CenterItem.CenterStructGUID))
                                {
                                    //    this.Log.Debug(ServiceConfiguration.Name, "ConnectedFeature Found by GUID: " + feature.OID);
                                    // Every featureclass except jbox's fall into this area.
                                    startingfeat = feature;
                                    if (li.StartItem.LoadFeature(startingfeat, FieldHelper.GetFieldValue(feature, SchemaInfo.Electric.FieldModelNames.OperatingNumber, true)))
                                        break;
                                }
                                else if ((feature.Class as IDataset).Name.ToUpper() == _electricDistNetworkJunctions)
                                {
                                    tempfeats.Add(feature);
                                    //    this.Log.Debug(ServiceConfiguration.Name, "ConFeat qop xi: " + feature.Class.AliasName + " : " + feature.OID);
                                    startingfeat = QueryOpenPoint(sourceFeature, ref tempfeats, ref li, workspace);
                                    if (startingfeat != null)
                                    {
                                        addToList = false;
                                        //  this.Log.Debug(ServiceConfiguration.Name, "ConnectedFeature Found by QueryOpenPoint: " + startingfeat.Class.AliasName + " :: " + startingfeat.OID);
                                        break;
                                    }

                                    //   this.Log.Debug(ServiceConfiguration.Name, "ConnectedFeature Other");
                                    startingfeat = QueryOther(sourceFeature, ref li, workspace);
                                    if (startingfeat != null)
                                    {
                                        addToList = false;
                                        //   this.Log.Debug(ServiceConfiguration.Name, "ConnectedFeature Found by QueryOther: " + startingfeat.Class.AliasName + " :: " + startingfeat.OID);
                                        break;
                                    }

                                }
                                else if (ModelNameFacade.ContainsFieldModelName(feature.Class, SchemaInfo.Electric.FieldModelNames.OperatingNumber))
                                {
                                    startingfeat = feature;
                                    if (li.StartItem.LoadFeature(startingfeat, FieldHelper.GetFieldValue(startingfeat, SchemaInfo.Electric.FieldModelNames.OperatingNumber, true))) ;
                                    break;
                                }
                            }
                        }
                        //      this.Log.Debug(ServiceConfiguration.Name, "ConnectedFeature Step 2.");
                        // This is likely at a jbox.
                        if ((startingfeat == null) && (tempfeats.Count > 0))
                        {
                            //    this.Log.Debug(ServiceConfiguration.Name, "ConFeat queryOPX: " + sourceFeature.Class.AliasName + " : " + sourceFeature.OID);
                            startingfeat = QueryOpenPoint(sourceFeature, ref tempfeats, ref li, workspace);
                            addToList = false;
                        }

                        //  this.Log.Debug(ServiceConfiguration.Name, "ConnectedFeature Step 3.");
                        if (startingfeat == null)
                        {
                            //     this.Log.Debug(ServiceConfiguration.Name, "ConnectedFeature Other: " + sourceFeature.Class.AliasName);
                            // Directly query layers for structureguid.
                            startingfeat = QueryOther(sourceFeature, ref li, workspace);
                            addToList = false;
                        }
                    }
                    else
                    {
                        //   this.Log.Debug(ServiceConfiguration.Name, "ConnectedFeature No Structure GUID.");
                        li.Comments += " No StructureGUID ";
                    }
                }
                if (startingfeat == null)
                {
                    //     this.Log.Debug(ServiceConfiguration.Name, "ConnectedFeature QueryForBusBarEnd.");
                    // Directly query layers for structureguid.
                    startingfeat = QueryForBusBarEnd(sourceFeature, ref li);
                    //     this.Log.Debug(ServiceConfiguration.Name, "ConnectedFeature Found by BusBar: " + sourceFeature.OID);
                }
                //   this.Log.Debug(ServiceConfiguration.Name, "ConnectedFeature Step 4.");
                if ((startingfeat != null) && addToList)
                {
                    //      this.Log.Info(ServiceConfiguration.Name, "ConnectedFeature add " + startingfeat.Class.AliasName + " ObjectID: " + startingfeat.OID);

                    if (ModelNameFacade.ContainsClassModelName(startingfeat.Class, SchemaInfo.Electric.ClassModelNames.DeviceGroup))
                        li.StartItem.LoadFeature(startingfeat, FieldHelper.GetFieldValue(startingfeat, _admsLabelField, false));
                    else
                        li.StartItem.LoadFeature(startingfeat, FieldHelper.GetFieldValue(startingfeat, SchemaInfo.Electric.FieldModelNames.OperatingNumber, true));
                }

                //  this.Log.Debug(ServiceConfiguration.Name, "ConnectedFeature Step 5.");
                if (li.StartItem.FCName == null)
                {
                    //    this.Log.Info(ServiceConfiguration.Name, "ConnectedFeature simple searches did not produce feature.");
                    //li.Comments += " No Starting Location ";

                    //SelItem startItem = GetNonStarter(sourceFeature, li, out edgeFeatures, geomNetwork, admsLabelDeviceClassIDs, workspace);
                    //if (startItem.FCName != null)
                    //    li.StartItem = startItem;
                }

                //  this.Log.Info(ServiceConfiguration.Name, "ConnectedFeature ended.");
            }
            catch (System.Exception ex)
            {
                this.Log.Error(ServiceConfiguration.Name, "Error during ADMS Label processing: ConnectedFeature " + ex.Message + " Stack: " + ex.StackTrace.ToString());
                throw ex;
            }
        }

        private void ConnnectedFeature_XX(IFeature sourceFeature, LabelItems li, IGeometricNetwork geomNetwork, ref List<int> admsLabelDeviceClassIDs, IWorkspace workspace)
        {
            if (li.CenterItem.CenterStructGUID != null)
            {
                KeyValuePair<string, IFeature> kvp = UseOpenPoint(li.CenterItem.Feature, workspace);
                if (kvp.Value != null)
                {
                    li.StartItem.LoadFeature(kvp.Value, FieldHelper.GetFieldValue(kvp.Value, SchemaInfo.Electric.FieldModelNames.OperatingNumber, true));
                    return;
                }
            }

            List<IFeature> junctionFeatures = null;
            List<IFeature> edgeFeatures = null;
            IFeature startingfeat;

            int x = GetNextJunctions(sourceFeature, out junctionFeatures, out edgeFeatures);
            if (x == 1)
            {
                if (ModelNameFacade.ContainsFieldModelName(junctionFeatures[0].Class, SchemaInfo.Electric.FieldModelNames.OperatingNumber))
                {
                    startingfeat = junctionFeatures[0];
                    li.StartItem.LoadFeature(startingfeat, FieldHelper.GetFieldValue(startingfeat, SchemaInfo.Electric.FieldModelNames.OperatingNumber, true));
                    return;
                }
            }
        }

        /// <summary>
        /// Checks for starting open point.
        /// </summary>
        /// <param name="sourceFeature"></param>
        /// <param name="tempfeats"></param>
        /// <param name="li"></param>
        /// <returns></returns>
        private IFeature QueryOpenPoint(IFeature sourceFeature, ref List<IFeature> tempfeats, ref LabelItems li, IWorkspace workspace)
        {
            MemoryUsage("QueryOpenPoint");

            IFeature startingfeat = null;
            //     this.Log.Debug(ServiceConfiguration.Name, "QueryOpenPoint Start." + sourceFeature.Class.AliasName + " : " + sourceFeature.OID);
            try
            {
                KeyValuePair<string, IFeature> kvp = UseOpenPoint(sourceFeature, workspace);
                if (kvp.Value != null)
                {
                    //    this.Log.Debug(ServiceConfiguration.Name, "QueryOpenPoint Found.");
                    startingfeat = kvp.Value;

                    if (startingfeat != null)
                    {
                        if (tempfeats.Count() == 0)
                        {
                            //   this.Log.Debug(ServiceConfiguration.Name, "QueryOpenPoint adding temps.");

                            ISimpleJunctionFeature jfeat = sourceFeature as ISimpleJunctionFeature;
                            for (int i = 0; i < jfeat.EdgeFeatureCount; i++)
                            {
                                tempfeats.Add(jfeat.EdgeFeature[i].FromJunctionFeature as IFeature);
                                tempfeats.Add(jfeat.EdgeFeature[i].ToJunctionFeature as IFeature);
                            }

                            //   this.Log.Debug(ServiceConfiguration.Name, "QueryOpenPoint added temps. " + tempfeats.Count());

                        }
                        //  this.Log.Debug(ServiceConfiguration.Name, "QueryOpenPoint Feature: " + startingfeat.Class.AliasName + " : " + startingfeat.OID);
                        // It is possible to have two network junctions on one edge, so lets get the closest to the devicegroup feature.
                        // Use junction to store EID for later tracing.
                        IFeature closeFeature = GetFeatureByProx(kvp.Value, ref tempfeats, 3);

                        li.StartItem.LoadFeature(startingfeat, kvp.Key);
                        if (li.StartItem.EID == 0)
                        {
                            if (closeFeature != null)
                            {
                                //    this.Log.Debug(ServiceConfiguration.Name, "QueryOpenPoint Close: " + closeFeature.Class.AliasName + " : " + closeFeature.OID);

                                int eid = LabelHelpers.GetFeatureEID(closeFeature);
                                if (eid > 0)
                                {
                                    li.StartItem.EID = eid;
                                    li.StartItem.LoadFeature(closeFeature, kvp.Key);
                                }
                                else
                                {
                                    li.StartItem = new SelItem();
                                    startingfeat = null;
                                    //        this.Log.Debug(ServiceConfiguration.Name, "QueryOpenPoint EID null");
                                }
                            }
                            else
                            {
                                li.StartItem = new SelItem();
                                startingfeat = null;
                                //    this.Log.Debug(ServiceConfiguration.Name, "QueryOpenPoint Closefeat null.");
                            }
                        }
                    }
                }
                else
                {
                    li.Comments += " No Device Group ";
                }
                //    this.Log.Debug(ServiceConfiguration.Name, "QueryOpenPoint End.");
                return startingfeat;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// OpenPoint.StructureGUID didn't match another feature. 
        /// If a DistBusBar is connected to the openpoint, we take the junction on the apposing end.
        /// </summary>
        /// <param name="sourceFeature"></param>
        /// <param name="li"></param>
        /// <returns>Ifeature</returns>
        private IFeature QueryForBusBarEnd(IFeature sourceFeature, ref LabelItems li)
        {
            MemoryUsage("QueryForBusBarEnd");

            IFeature result = null;
            List<IFeature> edgeFeatures = new List<IFeature>();
            List<IFeature> junctionFeatures = new List<IFeature>();
            try
            {
                GetNextJunctions(sourceFeature, out junctionFeatures, out edgeFeatures, true);

                // If more then one busbar found, we can't determin direction.
                if (junctionFeatures.Count() >= 2)
                {
                    //     this.Log.Debug(ServiceConfiguration.Name, "QueryForBusBarEnd to many busbars connected: " + sourceFeature.Class.AliasName + " : " + sourceFeature.OID);
                    return null;
                }
                foreach (IFeature feature in junctionFeatures)
                {
                    //     this.Log.Debug(ServiceConfiguration.Name, "QueryForBusBarEnd: " + feature.Class.AliasName + " : " + feature.OID);
                    if (FeatureIsInList(feature))
                    {
                        //     this.Log.Debug(ServiceConfiguration.Name, "QueryForBusBarEnd result: " + feature.Class.AliasName + " : " + feature.OID);
                        result = feature;
                        break;
                    }
                }

                //if (result == null)
                //    this.Log.Warn(ServiceConfiguration.Name, "WARNING: QueryForBusBarEnd - OpenPoint is missing a DistBusBar OpenPoint.ObjectID: " + sourceFeature.OID);
                //else
                //    this.Log.Debug(ServiceConfiguration.Name, "QueryForBusBarEnd " + result.Class.AliasName + " ObjectID: " + result.OID);

                return result;
            }
            catch (System.Exception ex)
            {
                this.Log.Warn(ServiceConfiguration.Name, "WARNING: QueryForBusBarEnd " + ex.Message);
                return result;
            }
        }

        /// <summary>
        /// Check if openpoint.structureguid is equal to another features structureguid.
        /// Currently only using Transformers.
        /// </summary>
        /// <param name="sourceFeature"></param>
        /// <param name="li"></param>
        /// <returns></returns>
        private IFeature QueryOther(IFeature sourceFeature, ref LabelItems li, IWorkspace workspace)
        {
            MemoryUsage("QueryOther");

            IFeature startingfeat = null;
            //    this.Log.Debug(ServiceConfiguration.Name, "QueryOther started.");
            // Currently we're only using tranformer. 
            KeyValuePair<string, IFeature> kvp = UseOtherLayer(sourceFeature, "EDGIS.TRANSFORMER", workspace);
            if (kvp.Value != null)
            {
                //    this.Log.Debug(ServiceConfiguration.Name, "QueryOther Loading.");
                startingfeat = kvp.Value;
                li.StartItem.LoadFeature(startingfeat, kvp.Key);
                if (li.StartItem.EID == 0)
                {
                    li.StartItem.EID = LabelHelpers.GetFeatureEID(sourceFeature);
                }
            }
            else
            {
                li.Comments += " No Device Group ";
            }
            //     this.Log.Debug(ServiceConfiguration.Name, "QueryOther End.");
            return startingfeat;
        }

        /// <summary>
        /// Returns edge feature between to junction/points.
        /// </summary>
        /// <param name="featone">point feature</param>
        /// <param name="feattwo">point feature</param>
        /// <param name="edgeFeatures">edge points connect to</param>
        /// <returns></returns>
        private IFeature GetEdgeByEndPoints(IFeature featone, IFeature feattwo, List<IFeature> edgeFeatures)
        {
            MemoryUsage("GetEdgeByEndPoints");

            IFeature result = null;
            ISimpleJunctionFeature jOne = null;
            ISimpleJunctionFeature jTwo = null;
            if ((featone != null) && (featone is INetworkFeature) && (featone.FeatureType == esriFeatureType.esriFTSimpleJunction))
            {
                jOne = featone as ISimpleJunctionFeature;
            }
            if ((feattwo != null) && (feattwo is INetworkFeature) && (feattwo.FeatureType == esriFeatureType.esriFTSimpleJunction))
            {
                jTwo = featone as ISimpleJunctionFeature;
            }

            if ((jTwo != null) && (jOne != null))
            {
                for (int i = 0; i < jTwo.EdgeFeatureCount; i++)
                {
                    for (int x = 0; x < jOne.EdgeFeatureCount; x++)
                    {
                        if ((jOne.EdgeFeature[x] is ISimpleEdgeFeature) && (jTwo.EdgeFeature[i] is ISimpleEdgeFeature))
                        {

                            if ((jOne.EdgeFeature[x] as ISimpleEdgeFeature).EID == (jTwo.EdgeFeature[i] as ISimpleEdgeFeature).EID)
                            {
                                result = jOne.EdgeFeature[x] as IFeature;
                                break;
                            }
                        }
                    }
                    if (result != null)
                        break;
                }
            }
            return result;
        }


        /// <summary>
        /// Takes a junctionfeature and returns apposing junctions.
        /// </summary>
        /// <param name="sourceFeature">Point feature that participates in network.</param>
        /// <param name="junctionFeatures">Junctions connected to edgeFeatures.</param>
        /// <param name="edgeFeatures">Edges connected to source.</param>
        /// <param name="justBusBar">Only one junctions at the end of BusBar edges.</param>
        /// <returns></returns>
        private int GetNextJunctions(IFeature sourceFeature, out List<IFeature> junctionFeatures, out List<IFeature> edgeFeatures, bool justBusBar = false)
        {
            MemoryUsage("GetNextJunctions");

            int result = 0;
            junctionFeatures = new List<IFeature>();
            edgeFeatures = new List<IFeature>();

            if ((sourceFeature != null) && (sourceFeature is INetworkFeature) && (sourceFeature.FeatureType == esriFeatureType.esriFTSimpleJunction))
            {
                ISimpleJunctionFeature junctionFeature = sourceFeature as ISimpleJunctionFeature;
                IEdgeFeature edgeFeature = null;
                IFeature feature = null;

                IFeature workfeature = null;

                for (int i = 0; i < junctionFeature.EdgeFeatureCount; i++)
                {
                    edgeFeature = junctionFeature.EdgeFeature[i];
                    if (edgeFeature is ISimpleEdgeFeature)
                    {
                        workfeature = edgeFeature as IFeature;

                        edgeFeatures.Add(workfeature);

                        for (int x = 0; x < 2; x++)
                        {
                            // Could pull this for out to a seperate function if it gets to unweilding in here.
                            if (x == 0)
                                feature = edgeFeature.FromJunctionFeature as IFeature;
                            else
                                feature = edgeFeature.ToJunctionFeature as IFeature;

                            if (feature != null)
                            {
                                if (feature.OID != sourceFeature.OID)
                                {
                                    // Want busbar features first.
                                    if (ModelNameFacade.ContainsClassModelName(workfeature.Class, SchemaInfo.Electric.ClassModelNames.PGEBusBar))
                                        junctionFeatures.Insert(0, feature);
                                    else if (!justBusBar)
                                        junctionFeatures.Add(feature);

                                    result++;
                                }
                            }
                        }

                    }
                }

            }

            return result;

        }

        /// <summary>
        /// Selected edges that where not in base tracing/paths for label.
        ///   -- looking for VIA/& features.
        /// </summary>
        /// <param name="geomNetwork"></param>
        /// <param name="eid"></param>
        /// <param name="li"></param>
        /// <param name="sellist"></param>
        /// <param name="junctionEIDs"></param>
        /// <returns></returns>
        private int TraversAnEdge2(IWorkspace workspace, IGeometricNetwork geomNetwork, int eid, LabelItems li, ref List<SelItem> sellist, ref List<int> junctionEIDs)
        {
            MemoryUsage("TraversAnEdge2");

            List<SelItem> list = new List<SelItem>();
            try
            {
                //      this.Log.Debug(ServiceConfiguration.Name, "TraversAnEdge2 " + eid + " : " + li.CenterItem.OID + " ...");
                SelItem si = new SelItem(this.Log, this.ServiceConfiguration);
                IFeature feat = LabelHelpers.GetFeaturefromEID(eid, esriElementType.esriETEdge, geomNetwork);
                if (feat == null)
                {
                    this.Log.Warn(ServiceConfiguration.Name, "Warning ADMSLabel TraversAnEdge2 edge: " + eid + " unable to get feature.");
                    return sellist.Count();
                }

                IEdgeFeature edgeFeat = feat as IEdgeFeature;
                List<int> nextedges = new List<int>();


                // We want to stop after we have 2 valid features.
                if (sellist.Count() == 2)
                    return sellist.Count();

                if (!junctionEIDs.Contains(edgeFeat.FromJunctionEID)) // && chkEIDs.Contains(edgeFeat.FromJunctionEID))
                {
                    junctionEIDs.Add(edgeFeat.FromJunctionEID);
                    //    this.Log.Info(ServiceConfiguration.Name, "Travers edge From " + edgeFeat.FromJunctionEID);
                    si = ValidFeature(edgeFeat.FromJunctionEID, li, geomNetwork, workspace);
                    if (si.FCName != null)
                        sellist.Add(si);
                    else
                    {
                        nextedges = GetNextEdge(edgeFeat.FromJunctionFeature, eid);

                        // Way to keep traversing edges till we find a valid junction on this path.
                        foreach (int e in nextedges)
                        {
                            TraversAnEdge2(workspace, geomNetwork, e, li, ref sellist, ref junctionEIDs);
                        }
                    }
                }
                if (!junctionEIDs.Contains(edgeFeat.ToJunctionEID))
                {
                    junctionEIDs.Add(edgeFeat.ToJunctionEID);
                    si = new SelItem(this.Log, this.ServiceConfiguration);
                    //      this.Log.Info(ServiceConfiguration.Name, "Travers edge To " + edgeFeat.ToJunctionEID);
                    si = ValidFeature(edgeFeat.ToJunctionEID, li, geomNetwork, workspace);
                    if (si.FCName != null)
                        sellist.Add(si);
                    else
                    {
                        nextedges = GetNextEdge(edgeFeat.ToJunctionFeature, eid);
                        // Way to keep traversing edges till we find a valid junction on this path.
                        foreach (int e in nextedges)
                        {
                            TraversAnEdge2(workspace, geomNetwork, e, li, ref sellist, ref junctionEIDs);
                        }
                    }
                }

                return sellist.Count();
            }
            catch (System.Exception ex)
            {
                this.Log.Error(ServiceConfiguration.Name, "TraversANEdge2 Error: " + ex.Message + " " + ex.StackTrace.ToString());
                throw ex;
            }

        }


        /// <summary>
        /// Grabs opposing edge. 
        /// </summary>
        /// <param name="junction">Point edge is connected to.</param>
        /// <param name="edgeEID">EID of edge we don't want returned.</param>
        /// <returns>List of Edge(s) EID.</returns>
        private List<int> GetNextEdge(IJunctionFeature junction, int edgeEID)
        {
            MemoryUsage("GetNextEdge");

            List<int> result = new List<int>();
            ISimpleJunctionFeature jOne = null;
            ISimpleEdgeFeature eOne = null;
            try
            {
                if ((junction != null) && (junction is INetworkFeature))
                {
                    jOne = junction as ISimpleJunctionFeature;
                    for (int i = 0; i < jOne.EdgeFeatureCount; i++)
                    {
                        eOne = jOne.EdgeFeature[i] as ISimpleEdgeFeature;

                        if (eOne.EID != edgeEID)
                        {
                            result.Add(edgeEID);
                        }
                    }
                }

                return result;
            }
            catch (System.Exception ex)
            {
                this.Log.Error(ServiceConfiguration.Name, "GetNextEdge Error: " + ex.Message + " " + ex.StackTrace.ToString());
                throw ex;
            }
        }

        /// <summary>
        /// Returns MM network weight value.
        /// </summary>
        /// <param name="geomNetwork"></param>
        /// <returns></returns>
        private INetWeight GetNetworkWeight(IGeometricNetwork geomNetwork)
        {
            INetSchema networkSchema = geomNetwork.Network as INetSchema;
            INetWeight electricTraceWeight = networkSchema.get_WeightByName("MMELECTRICTRACEWEIGHT");
            return electricTraceWeight;
        }

        /// <summary>
        /// Builds tracing flags.
        /// </summary>
        /// <param name="geomNetwork"></param>
        /// <param name="sourceFeature"></param>
        /// <param name="JnetFlag"></param>
        /// <param name="EnetFlag"></param>
        /// <returns></returns>
        private bool GetFlags(IGeometricNetwork geomNetwork, IFeature sourceFeature, out IJunctionFlag JnetFlag, out IEdgeFlag EnetFlag)
        {
            MemoryUsage("GetFlags");

            bool result = false;
            string[] modelNames = { SchemaInfo.Electric.ClassModelNames.PGEPriOHConductor, SchemaInfo.Electric.ClassModelNames.PGEPriUGConductor };
            EnetFlag = null;
            JnetFlag = null;

            if ((sourceFeature != null) && (sourceFeature is INetworkFeature) && (sourceFeature.FeatureType == esriFeatureType.esriFTSimpleJunction))
            {
                ISimpleJunctionFeature junctionFeature = sourceFeature as ISimpleJunctionFeature;
                IEdgeFeature edgeFeature = null;
                IFeature feature = null;
                for (int i = 0; i < junctionFeature.EdgeFeatureCount; i++)
                {
                    edgeFeature = junctionFeature.EdgeFeature[i];
                    if (edgeFeature is ISimpleEdgeFeature)
                    {
                        ISimpleEdgeFeature simpleEdge = edgeFeature as ISimpleEdgeFeature;
                        feature = LabelHelpers.GetFeaturefromEID(simpleEdge.EID, esriElementType.esriETEdge, geomNetwork);

                        if (feature != null)
                        {
                            if (ModelNameFacade.ContainsClassModelName(feature.Class, modelNames))
                            {
                                break;
                            }
                        }
                        else
                        {
                            this.Log.Warn(ServiceConfiguration.Name, "Warning ADMSLabel GetFlags, issue tracing from OpenPoint.OID: " + sourceFeature.OID + " Edge.EID: " + simpleEdge.EID + " unable to get feature.");
                        }
                    }
                }
                if ((feature == null) && (edgeFeature != null))
                {
                    if (edgeFeature is ISimpleEdgeFeature)
                    {
                        ISimpleEdgeFeature simpleEdge = edgeFeature as ISimpleEdgeFeature;
                        feature = LabelHelpers.GetFeaturefromEID(simpleEdge.EID, esriElementType.esriETEdge, geomNetwork);
                        if (feature == null)
                        {
                            this.Log.Warn(ServiceConfiguration.Name, "Warning ADMSLabel GetFlags, issue tracing from OpenPoint.OID: " + sourceFeature.OID + " Edge.EID: " + simpleEdge.EID + " unable to get feature.");
                        }
                    }
                }

                if (feature != null)
                {
                    IFeature dfeat = edgeFeature.FromJunctionFeature as IFeature;
                    // != will set junction flag to openpoint.
                    if (dfeat.OID != sourceFeature.OID)
                    {
                        string temp = " :  " + dfeat.Class.AliasName + " : " + dfeat.OID;
                        dfeat = edgeFeature.ToJunctionFeature as IFeature;
                    }

                    INetFlag netFlag = LabelHelpers.GetFeatureNetFlag(dfeat, esriElementType.esriETJunction);

                    IJunctionFlag junctionFlag = netFlag as IJunctionFlag;
                    JnetFlag = junctionFlag;

                    netFlag = LabelHelpers.GetFeatureNetFlag(feature, esriElementType.esriETEdge);

                    IEdgeFlag edgeFlag = netFlag as IEdgeFlag;
                    edgeFlag.Position = 0.05F;
                    edgeFlag.TwoWay = false;
                    EnetFlag = edgeFlag;

                    result = true;
                }

            }

            return result;

        }


        /// <summary>
        /// Determine direction to trace and return eids for that are within the path.
        /// </summary>
        /// <param name="geomNetwork"></param>
        /// <param name="downEID">Eid for downstream trace flag.</param>
        /// <param name="upEID">Eid for upstream trace flag.</param>
        /// <param name="junctionEIDs">Junction eids in result.</param>
        /// <param name="edgeEIDS"></param>
        /// <param name="li">Contains eid that can not be traced over and the starting eid for each direction.</param>
        /// <returns></returns>
        private string TraceForDIR2(IGeometricNetwork geomNetwork, int downEID, int upEID,
            ref List<int> junctionEIDs, ref List<int> edgeEIDS, LabelItems li)
        {
            MemoryUsage("TraceForDIR2");

            string result = string.Empty;
            int junctionEID = 0;

            List<int> orderedJunctionEIDList = new List<int>();
            List<int> orderedEdgeEIDList = new List<int>();

            IEnumNetEID outJunctions;
            IEnumNetEID outEdges;
            INetSchema netSchema = geomNetwork.Network as INetSchema;
            INetWeight electricTraceWeight = netSchema.get_WeightByName("MMELECTRICTRACEWEIGHT");
            IMMElectricTraceSettingsEx mmElectricTraceSettings = new MMElectricTraceSettingsClass();
            mmElectricTraceSettings.UseFeederManagerCircuitSources = true;
            mmElectricTraceSettings.UseFeederManagerProtectiveDevices = false;

            IMMElectricNetworkTracing mmElectricTracing = new MMFeederTracerClass();

            IMMNetworkAnalysisExtForFramework networkAnalysisExtForFramework = new MMNetworkAnalysisExtForFrameworkClass();

            try
            {
                networkAnalysisExtForFramework.DrawComplex = true;
                networkAnalysisExtForFramework.SelectionSemantics = mmESRIAnalysisOptions.mmESRIAnalysisOnAllFeatures;
                #region Down Stream
                // Perform trace and get a list of.
                mmElectricTracing.TraceDownstream(
                    geomNetwork, networkAnalysisExtForFramework, mmElectricTraceSettings,
                    li.CenterItem.EID, esriElementType.esriETJunction, mmPhasesToTrace.mmPTT_Any, out outJunctions, out outEdges);

                outJunctions.Reset();
                junctionEID = -1;
                // Get downstream Junctions.
                while ((junctionEID = outJunctions.Next()) > 0)
                {
                    if (!junctionEIDs.Contains(junctionEID))
                        junctionEIDs.Add(junctionEID);
                }

                outEdges.Reset();
                junctionEID = -1;
                while ((junctionEID = outEdges.Next()) > 0)
                {
                    if (!edgeEIDS.Contains(junctionEID))
                        edgeEIDS.Add(junctionEID);
                }

                // Check if downstream eid is not starting feature.
                if ((junctionEIDs.Count() > 0) && (!junctionEIDs.Contains(li.StartItem.EID)))
                {
                    // Sort list to position.
                    ShowElectricConnectivityOrder(geomNetwork.Network, 0,
                        li.CenterItem.EID, junctionEIDs, edgeEIDS, "", ref orderedJunctionEIDList, ref orderedEdgeEIDList,
                        ref electricTraceWeight);
                    junctionEIDs = orderedJunctionEIDList;

                    return "DOWN";
                }
                else
                {
                    result += "DCROSSED";
                }

                #endregion

                #region UpStream

                outJunctions = null;
                outEdges = null;

                mmElectricTracing.TraceUpstream(
                   geomNetwork, networkAnalysisExtForFramework, mmElectricTraceSettings,
                   li.CenterItem.EID, esriElementType.esriETJunction, mmPhasesToTrace.mmPTT_Any, out outJunctions, out outEdges);

                outJunctions.Reset();
                junctionEIDs = new List<int>();
                junctionEID = -1;
                // Get downstream Junctions.
                while ((junctionEID = outJunctions.Next()) > 0)
                {
                    if (!junctionEIDs.Contains(junctionEID))
                        junctionEIDs.Add(junctionEID);
                }

                outEdges.Reset();
                edgeEIDS = new List<int>();
                junctionEID = -1;
                while ((junctionEID = outEdges.Next()) > 0)
                {
                    if (!edgeEIDS.Contains(junctionEID))
                        edgeEIDS.Add(junctionEID);
                }

                // Check if downstream eid is not starting feature.
                if ((junctionEIDs.Count() > 0) && (!junctionEIDs.Contains(li.StartItem.EID)))
                {
                    orderedJunctionEIDList = new List<int>();
                    orderedEdgeEIDList = new List<int>();

                    // Sort list to position.
                    ShowElectricConnectivityOrder(geomNetwork.Network, 0,
                        li.CenterItem.EID, junctionEIDs, edgeEIDS, "", ref orderedJunctionEIDList, ref orderedEdgeEIDList,
                        ref electricTraceWeight);
                    junctionEIDs = orderedJunctionEIDList;

                    return "UP";
                }
                else
                {
                    result += "UCROSSED";
                }

                #endregion
            }
            catch (Exception ex)
            {
                this.Log.Error("Error executing tracing. Message: " + ex.Message + " StackTrace: " + ex.StackTrace);
                throw ex;
            }
            finally
            {
                if (mmElectricTracing != null) { while (Marshal.ReleaseComObject(mmElectricTracing) > 0) ; }
                if (networkAnalysisExtForFramework != null) { while (Marshal.ReleaseComObject(networkAnalysisExtForFramework) > 0) ; }
                if (mmElectricTraceSettings != null) { while (Marshal.ReleaseComObject(mmElectricTraceSettings) > 0) ; }
                //   GC.Collect();
            }
            return result;
        }
        /// <summary>
        /// Takes starting location and traces to the end and start of the network location.
        /// </summary>
        /// <param name="geomNetwork"></param>
        /// <param name="circuitID"></param>
        /// <param name="admsLabelDeviceClassIDs"></param>
        /// <param name="li"></param>
        /// <param name="downeid">Junction eids downstream of starting location.</param>
        /// <param name="upeid">Junction eids upstream of starting location.</param>
        /// <returns></returns>
        private bool GetPathEnd(IGeometricNetwork geomNetwork, string circuitID, ref List<int> admsLabelDeviceClassIDs, LabelItems li, out List<int> downeid, out List<int> upeid)
        {
            MemoryUsage("GetPathEnd");

            bool result = false;
            bool endOfLine = false;

            downeid = new List<int>();
            upeid = new List<int>();

            string feederID = "";
            string message = string.Empty;
            IMMFeederExt feederExt = null;
            IMMFeederSpace feederSpace = null;
            IMMFeederSource feederSource = null;
            IMMElectricNetworkTracing downstreamTracer = null;
            IMMNetworkAnalysisExtForFramework networkAnalysisExtForFramework = null;
            IMMElectricTraceSettingsEx electricTraceSettings = null;

            Type type = Type.GetTypeFromProgID("mmFramework.MMFeederExt");
            //    this.Log.Debug(ServiceConfiguration.Name, "GetPathEnd started.");
            object obj = Activator.CreateInstance(type);

            try
            {
                //     this.Log.Debug(ServiceConfiguration.Name, "GetPathEnd I");
                feederExt = obj as IMMFeederExt;
                feederSpace = feederExt.get_FeederSpace(geomNetwork, false);
                if (feederSpace == null)
                {
                    throw new Exception("Unable to get feeder space information from geometric network");
                }

                INetwork network = geomNetwork.Network;
                INetSchema networkSchema = network as INetSchema;
                INetWeight electricTraceWeight = networkSchema.get_WeightByName("MMELECTRICTRACEWEIGHT");
                downstreamTracer = new MMFeederTracerClass();

                electricTraceSettings = new MMElectricTraceSettingsClass
                {
                    RespectESRIBarriers = true,
                    UseFeederManagerCircuitSources = true,
                    UseFeederManagerProtectiveDevices = false,
                    ProtectiveDeviceClassIDs = admsLabelDeviceClassIDs.ToArray()
                };

                networkAnalysisExtForFramework = new MMNetworkAnalysisExtForFrameworkClass
                {
                    DrawComplex = true,
                    SelectionSemantics = mmESRIAnalysisOptions.mmESRIAnalysisOnAllFeatures
                };

                //   this.Log.Debug(ServiceConfiguration.Name, "GetPathEnd Feeder Source.");
                // Phases to trace.
                mmPhasesToTrace phasesToTrace = mmPhasesToTrace.mmPTT_Any;
                feederSource = feederSpace.FeederSources.get_FeederSourceByFeederID(circuitID);
                if (feederSource != null)
                {
                    feederID = feederSource.FeederID.ToString();

                    DateTime startTime = DateTime.Now;
                    IEnumNetEID junctions = null;
                    IEnumNetEID edges = null;

                    electricTraceSettings.UseFeederManagerProtectiveDevices = true;
                    //    this.Log.Debug(ServiceConfiguration.Name, "GetPathEnd Down");
                    // Trace.
                    downstreamTracer.TraceDownstream(geomNetwork, networkAnalysisExtForFramework, electricTraceSettings, li.CenterItem.EID,
                                                        esriElementType.esriETJunction, phasesToTrace, out junctions, out edges);

                    junctions.Reset();
                    int junction = -1;
                    // Add our junction adms eids.
                    while ((junction = junctions.Next()) > 0)
                    {
                        if (!downeid.Contains(junction))
                            downeid.Add(junction);

                    }

                    if (edges.Count == 0)
                        endOfLine = true;

                    //      this.Log.Debug(ServiceConfiguration.Name, "GetPathEnd Up");
                    downstreamTracer.TraceUpstream(geomNetwork, networkAnalysisExtForFramework, electricTraceSettings, li.CenterItem.EID,
                                                        esriElementType.esriETJunction, phasesToTrace, out junctions, out edges);

                    junctions.Reset();
                    junction = -1;
                    // Add our junction adms eids.
                    while ((junction = junctions.Next()) > 0)
                    {
                        if (!upeid.Contains(junction))
                            upeid.Add(junction);
                    }
                    if (edges.Count == 0)
                        endOfLine = true;

                    if ((upeid.Count > 0) && (downeid.Count > 0))
                    {
                        result = true;
                    }
                    junctions = null;
                    edges = null;
                }

                if (endOfLine)
                {
                    //   this.Log.Debug(ServiceConfiguration.Name, "GetPathEnd found EOL");
                    return false;
                }
                //    this.Log.Debug(ServiceConfiguration.Name, "GetPathEnd Ending " + result);
                return result;
            }
            catch (Exception ex)
            {
                this.Log.Error(ServiceConfiguration.Name, "Error executing tracing. Message: " + ex.Message + " StackTrace: " + ex.StackTrace.ToString());
                throw ex;
            }
            finally
            {
                if (downstreamTracer != null) { while (Marshal.ReleaseComObject(downstreamTracer) > 0) ; }
                if (networkAnalysisExtForFramework != null) { while (Marshal.ReleaseComObject(networkAnalysisExtForFramework) > 0) ; }
                if (electricTraceSettings != null) { while (Marshal.ReleaseComObject(electricTraceSettings) > 0) ; }

                if (feederExt != null) { while (Marshal.ReleaseComObject(feederExt) > 0) ; }
                if (feederSpace != null) { while (Marshal.ReleaseComObject(feederSpace) > 0) ; }
                if (feederSource != null) { while (Marshal.ReleaseComObject(feederSource) > 0) ; }


                //     GC.Collect();
            }
        }

        #endregion

        #region Cloned from PGE_Trace

        /// <summary>
        /// This method will perform a secondary trace to order the ArcFM results in electrical connectivity order.
        /// </summary>
        /// <param name="networkDataset">INetwork for the network that was traced.</param>
        /// <param name="startingJunctionEID">EID of the junction to start at.</param>
        /// <param name="resultsDialog">Dialog to which the trace results will be added.</param>
        /// <param name="junctionEIDs">List of the junction EIDs and their features that were returned by the ArcFM trace.</param>
        /// <param name="edgeEIDs">List of the Edge EIDs and their features that were returned by the ArcFM trace.</param>
        /// <param name="ParentNodeName">Name of the parent node from the trace results dialog tree.</param>
        /// <param name="orderedEIDList">List of the EIDs as they are processed.</param>
        private void ShowElectricConnectivityOrder(INetwork networkDataset, object junctionTraceWeight,
            int startingJunctionEID,
            List<int> junctionEIDs, List<int> edgeEIDs, string ParentNodeName
            , ref List<int> orderedJunctionEIDList, ref List<int> orderedEdgeEIDList,
            ref INetWeight MMElectricTraceWeight)
        {
            MemoryUsage("ShowElectricConnectivityOrder");

            List<int> parentNodesProcessed = new List<int>();
            ProcessJunctionParent(startingJunctionEID, junctionTraceWeight, networkDataset, ref junctionEIDs, ref edgeEIDs, ref ParentNodeName,
                ref parentNodesProcessed, ref orderedJunctionEIDList, ref orderedEdgeEIDList, ref MMElectricTraceWeight);
        }

        /// <summary>
        /// This method will process a starting junction EID to determine the actual ordering of the ArcFM trace so that it can return an
        ///     order which is based on the electrical connectivity of the features, rather than sorting by feature classes.
        /// </summary>
        /// <param name="StartJunctionEID">EID of the junction to start at.</param>
        /// <param name="networkDataset">INetwork for the network that was traced.</param>
        /// <param name="resultsDialog">Dialog to which the trace results will be added.</param>
        /// <param name="junctionEIDs">List of the junction EIDs and their features that were returned by the ArcFM trace.</param>
        /// <param name="edgeEIDs">List of the Edge EIDs and their features that were returned by the ArcFM trace.</param>
        /// <param name="parentNodeName">Name of the parent node from the trace results dialog tree.</param>
        /// <param name="parentNodesProcessed">List of the nodes that have been processed already.</param>
        /// <param name="orderedEIDList">List of the EIDs as they are processed.</param>
        private void ProcessJunctionParent(int StartJunctionEID, object JunctionTraceWeight, INetwork networkDataset,
            ref List<int> junctionEIDs, ref List<int> edgeEIDs, ref string parentNodeName, ref List<int> parentNodesProcessed,
            ref List<int> orderedJunctionEIDList, ref List<int> orderedEdgeEIDList, ref INetWeight electricTraceWeight)
        {
            try
            {
                // If we haven't processed this parent yet add it to our list.  If we have, exit the method as we don't need
                //   to process it again.
                if (parentNodesProcessed.Contains(StartJunctionEID))
                {
                    return;
                }

                IForwardStar forwardStar = networkDataset.CreateForwardStar(false, electricTraceWeight, null, null, null);

                Stack<int> junctionQueue = new Stack<int>();

                junctionQueue.Push(StartJunctionEID);

                int junctionEID = -1;
                while (junctionQueue.Count > 0)
                {
                    junctionEID = junctionQueue.Pop();
                    string junctionDisplayName = "";
                    if (junctionEIDs.Contains(junctionEID))
                    {
                        junctionDisplayName = junctionEID.ToString();
                    }
                    if (junctionDisplayName != parentNodeName)
                    {
                        // If our original EID list returned by the ArcFM trace doesn't contain this junction eid then move on.  Also,
                        // If we have already added it to our ordered list then we have already processed this junction so we can move one.
                        if (!junctionEIDs.Contains(junctionEID) || orderedJunctionEIDList.Contains(junctionEID)) { continue; }

                        // Add this junction to our ordered EID list.
                        orderedJunctionEIDList.Add(junctionEID);
                    }


                    int edgeCount = 0;

                    // Find our adjacent edges to this junction EID.
                    forwardStar.FindAdjacent(0, junctionEID, out edgeCount);


                    if (junctionDisplayName == parentNodeName)
                    {
                        parentNodesProcessed.Add(junctionEID);
                    }

                    // Process all of the edges.
                    for (int i = 0; i < edgeCount; i++)
                    {
                        int adjacentEdgeEID = -1;
                        bool reverseOrientation = false;
                        object edgeWeightValue = null;
                        forwardStar.QueryAdjacentEdge(i, out adjacentEdgeEID, out reverseOrientation, out edgeWeightValue);

                        // If our original EID list returned by the ArcFM trace doesn't contain this edge eid then move on.  Also,
                        //    if we have already added it to our ordered list then we have already processed this edge so we can move one.
                        if (!edgeEIDs.Contains(adjacentEdgeEID) || orderedEdgeEIDList.Contains(adjacentEdgeEID)) { continue; }

                        // Add this edge to our odered EID list.
                        orderedEdgeEIDList.Add(adjacentEdgeEID);
                        // Find the adjacent junction for this edge and add it to our queue to process.
                        int adjacentJunctionEID = -1;
                        object junctionWeightValue = null;
                        forwardStar.QueryAdjacentJunction(i, out adjacentJunctionEID, out junctionWeightValue);
                        ProcessJunctionParent(adjacentJunctionEID, junctionWeightValue, networkDataset, ref junctionEIDs,
                            ref edgeEIDs, ref parentNodeName, ref parentNodesProcessed, ref orderedJunctionEIDList, ref orderedEdgeEIDList, ref electricTraceWeight);
                    }
                }
            }
            catch
            {

            }
        }

        #endregion
        #region "Helpers"

        /// <summary>
        /// Remove all line features/edges that are NOT PriOHConductors or PriUGConductors.
        /// </summary>
        /// <param name="adjacentEdgeEIDS"></param>
        /// <param name="reverseOrientationEdge"></param>
        /// <param name="geomNetwork"></param>
        /// <returns>List of primary conductors.</returns>
        private int[] OnlyPrimaries(int[] adjacentEdgeEIDS, bool[] reverseOrientationEdge, IGeometricNetwork geomNetwork)
        {
            MemoryUsage("OnlyPrimaries");

            List<int> result = new List<int>();
            IFeature feature;
            bool reverse = false;
            string[] modelNames = { SchemaInfo.Electric.ClassModelNames.PGEPriOHConductor, SchemaInfo.Electric.ClassModelNames.PGEPriUGConductor, SchemaInfo.Electric.ClassModelNames.PGEBusBar };


            for (int i = 0; i < adjacentEdgeEIDS.Length; i++)
            {
                reverse = reverseOrientationEdge[i];
                feature = LabelHelpers.GetFeaturefromEID(adjacentEdgeEIDS[i], esriElementType.esriETEdge, geomNetwork);
                if (feature != null)
                {
                    if (ModelNameFacade.ContainsClassModelName(feature.Class, modelNames))
                    {
                        //  If (reverse).
                        result.Add(adjacentEdgeEIDS[i]);
                    }
                }
                else
                {
                    this.Log.Warn(ServiceConfiguration.Name, "Warning ADMSLabel OnlyPrimaries edge: " + adjacentEdgeEIDS[i] + " unable to get feature.");
                }
            }

            return result.ToArray();
        }

        /// <summary>
        /// Is this a valid feature for a label part.
        /// </summary>
        /// <param name="eid">Junction EID we are testing for validity.</param>
        /// <param name="center"></param>
        /// <param name="geomNetwork"></param>
        /// <returns></returns>
        private SelItem ValidFeature(int eid, LabelItems center, IGeometricNetwork geomNetwork, IWorkspace workspace)
        {
            MemoryUsage("ValidFeature");

            //     this.Log.Debug(ServiceConfiguration.Name, "ValidFeature started.");
            SelItem result = new SelItem(this.Log, this.ServiceConfiguration);
            IFeature input = LabelHelpers.GetFeaturefromEID(eid, esriElementType.esriETJunction, geomNetwork);
            if (input != null)
            {

                string fcname = (input.Class as IDataset).Name.ToUpper();

                result.Index = 99;

                IFeatureClass deviceGroup = ModelNameFacade.FeatureClassByModelName(workspace, SchemaInfo.Electric.ClassModelNames.DeviceGroup);

                // Used to determine if next or start feature.
                string structguid = FieldHelper.GetFieldValue(input, _structureGUID);

                string lbl = string.Empty;
                KeyValuePair<string, IFeature> kvp = new KeyValuePair<string, IFeature>(string.Empty, null);
                //   this.Log.Debug(ServiceConfiguration.Name, "ValidFeature: " + fcname + " : " + input.OID);
                if (ModelNameFacade.ContainsClassModelName(input.Class, _modelNames))
                {

                    if (ModelNameFacade.ContainsClassModelName(input.Class, SchemaInfo.Electric.ClassModelNames.PGEOpenPoint))
                    {
                        kvp = UseOpenPoint(input, workspace);
                        lbl = kvp.Key;
                    }
                    else
                        lbl = FieldHelper.GetFieldValue(input, SchemaInfo.Electric.FieldModelNames.OperatingNumber, true);

                    if (lbl.Length > 0)
                    {
                        // Already have starting label, so we don't want it again.
                        if (structguid != center.CenterItem.CenterStructGUID)
                        {
                            // Store the actual feature. 
                            if (kvp.Key.Length > 0)
                                input = kvp.Value;

                            if (result.LoadFeature(input, lbl))
                            {
                                result.Index = 2;
                                result.Comments += " Guids DO NOT match";
                                //         this.Log.Debug(ServiceConfiguration.Name, "Validate Guids DO NOT Match.");
                            }
                        }

                    }
                }
                else
                {

                    if (fcname == _electricDistNetworkJunctions)
                    {
                        kvp = UseOpenPoint(input, workspace);
                        lbl = kvp.Key;

                        if (lbl.Length > 0)
                        {
                            if (result.LoadFeature(kvp.Value, lbl))
                            {
                                result.Index = 2;
                                result.Comments += " Standard OpenPoint";
                            }
                        }
                        else
                        {
                            // Lets try querying for devicegroup by location.
                            IGeometry geom = BufferPoint(input.Shape, 5);
                            kvp = QueryFeatNear(deviceGroup, geom, input, 2);
                            lbl = kvp.Key;
                            if (lbl.Length > 0)
                            {
                                if (result.LoadFeature(kvp.Value, lbl))
                                {
                                    result.Index = 2;
                                    result.Comments += " NEAR OpenPoint";
                                }
                            }
                        }
                    }
                }

                // Want eid of feature not devicegroup.
                if ((result.FCName != null) && ModelNameFacade.ContainsClassModelName(result.Feature.Class, SchemaInfo.Electric.ClassModelNames.DeviceGroup))
                {
                    result.EID = eid;
                }
            }
            else
            {
                this.Log.Warn(ServiceConfiguration.Name, "Warning ADMSLabel ValidFeature edge: " + eid + " unable to get feature.");
            }

            //    this.Log.Debug(ServiceConfiguration.Name, "ValidFeature ended.");
            return result;
        }

        /// <summary>
        /// Checks if openpoint belongs to a Devicegroup (JBox).
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private KeyValuePair<string, IFeature> UseOpenPoint(IFeature input, IWorkspace workspace)
        {
            MemoryUsage("UseOpenPoint");

            KeyValuePair<string, IFeature> result = new KeyValuePair<string, IFeature>(string.Empty, null);
            //  this.Log.Debug(ServiceConfiguration.Name, "UseOpenPoint Start.");
            SelItem item = new SelItem(this.Log, this.ServiceConfiguration);
            item.SetCenterStructGUID(input);

            if ((item.CenterStructGUID == null) || (item.CenterStructGUID.Length < 2))
            {
                //     this.Log.Debug(ServiceConfiguration.Name, "UseOpenPoint Guid is null");
            }
            else
            {
                //    this.Log.Debug(ServiceConfiguration.Name, "UseOpenPoint Guid: " + item.CenterStructGUID);
                string sourcevalue = FieldHelper.GetFieldValue(input, SchemaInfo.Electric.FieldModelNames.OperatingNumber, true);
                //   this.Log.Debug(ServiceConfiguration.Name, "UseOpenPoint Value: " + sourcevalue);
                result = UseOpenPoint(item, sourcevalue, workspace);
                //   if ((result.Key != null) || (result.Key.Length > 0))
                //       this.Log.Debug(ServiceConfiguration.Name, "UseOpenPoint Result: " + result.Key);
            }

            return result;
        }

        /// <summary>
        /// Returns DeviceGroup(JBox) based on OpenPoint.structutreguid = DeviceGroup.GLobalID.
        /// </summary>
        /// <param name="item"></param>
        /// <param name="sourcevalue"></param>
        /// <returns></returns>
        private KeyValuePair<string, IFeature> UseOpenPoint(SelItem item, string sourcevalue, IWorkspace workspace)
        {
            MemoryUsage("UseOpenPoint2");

            KeyValuePair<string, IFeature> result = new KeyValuePair<string, IFeature>(string.Empty, null);

            IFeatureClass deviceGroup = ModelNameFacade.FeatureClassByModelName(workspace, SchemaInfo.Electric.ClassModelNames.DeviceGroup);
            if (deviceGroup != null)
            {
                //  this.Log.Debug(ServiceConfiguration.Name, "UseOpenPoint DeviceGroup. " + item.CenterStructGUID);

                IFeature feature = LabelHelpers.GetFeature(deviceGroup, _globalID, item.CenterStructGUID);

                if (feature != null)
                {
                    //    this.Log.Debug(ServiceConfiguration.Name, "UseOpenPoint Found: " + feature.Class.AliasName + " : " + feature.OID);
                    string factype = FieldHelper.GetFieldValue(feature, _deviceGroupType);
                    string subtype = FieldHelper.GetFieldValue(feature, _subtypeCD);

                    // Padmount value = 36, subtype = 3.
                    // Subsurface value = 29, subtype = 2.
                    if (IsValidJBox(feature))
                    {
                        //  this.Log.Debug(ServiceConfiguration.Name, "UseOpenPoint Valid: " + feature.Class.AliasName + " : " + feature.OID);
                        string label = FieldHelper.GetFieldValue(feature, _admsLabelField, false);
                        //   this.Log.Debug(ServiceConfiguration.Name, "UseOpenPoint Label: " + label);
                        // Update - not using ADMSLabel from DeviceGroup any more, just need geometry for tracing now.
                        result = new KeyValuePair<string, IFeature>(label, feature);

                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Check to see if a feature is within a minimum distance of another feature.
        /// </summary>
        /// <param name="one"></param>
        /// <param name="two"></param>
        /// <param name="mindist"></param>
        /// <returns></returns>
        private bool IsNear(IFeature one, IFeature two, double mindist)
        {
            MemoryUsage("IsNear ");
            //     this.Log.Debug(ServiceConfiguration.Name, "Near: " + one.Class.AliasName + " " + one.OID);
            bool result = false;
            IProximityOperator proximitryOperator = one.Shape as IProximityOperator;
            double d = 9999;

            d = proximitryOperator.ReturnDistance(two.Shape);
            if (d < mindist)
            {
                result = true;
            }

            return result;
        }

        /// <summary>
        /// Check to see if DeviceGroup feature is a JBox.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private bool IsValidJBox(IFeature input)
        {
            MemoryUsage("IsValidJBox");

            bool result = false;

            if ((input != null) && (ModelNameFacade.ContainsClassModelName(input.Class, SchemaInfo.Electric.ClassModelNames.DeviceGroup)))
            {
                string factype = FieldHelper.GetFieldValue(input, _deviceGroupType);
                string subtype = FieldHelper.GetFieldValue(input, _subtypeCD);

                if (factype.Length > 0)
                {
                    // Padmount value = 36, subtype = 3.
                    // Subsurface value = 29, subtype = 2.
                    if (((subtype == "2") && (factype == "29")) || ((subtype == "3") && (factype == "36")))
                    {
                        result = true;
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Buffers Point.
        /// </summary>
        /// <param name="inputgeom"></param>
        /// <param name="dist"></param>
        /// <returns></returns>
        private IGeometry BufferPoint(IGeometry inputgeom, double dist)
        {
            //    MemoryUsage("BufferPoint");
            //    this.Log.Debug(ServiceConfiguration.Name, "Buffer");
            ITopologicalOperator topologicalOperator = inputgeom as ITopologicalOperator;
            IGeometry geometry = topologicalOperator.Buffer(dist);

            return geometry;
        }

        /// <summary>
        /// For malformed openpoints, checks to see if a devicegroup or transformer is near
        ///   the input is off a busbar  and   most likey a network junction point.
        /// Nearfeat is our original openpoint.
        /// </summary>
        /// <param name="input">Junction that shares an edge with nearFeat/OpenPoint.</param>
        /// <param name="dist"></param>
        /// <param name="nearFeature">Openpoint we're starting with.</param>
        /// <returns></returns>
        private SelItem QueryDeviceGroupTransformers(IFeature input, double dist, IFeature nearFeature, IWorkspace workspace)
        {
            //    MemoryUsage("QueryDeviceGroupTransformers");

            SelItem si = new SelItem(this.Log, this.ServiceConfiguration);
            //    this.Log.Debug(ServiceConfiguration.Name, "QueryDeviceGroupTransformers started.");
            IFeatureClass deviceGroup = ModelNameFacade.FeatureClassByModelName(workspace, SchemaInfo.Electric.ClassModelNames.DeviceGroup);

            IGeometry geometry = BufferPoint(input.Shape, dist);

            KeyValuePair<string, IFeature> kvp = QueryFeatNear(deviceGroup, geometry, nearFeature, 10);
            if (kvp.Value == null)
            {
                IFeatureClass transformer = ModelNameFacade.FeatureClassByModelName(workspace, SchemaInfo.Electric.ClassModelNames.PGETransformer);
                kvp = QueryFeatNear(transformer, geometry, nearFeature, 10);
            }

            if (kvp.Value != null)
            {
                if (!si.LoadFeature(kvp.Value, kvp.Key, input))
                    si = new SelItem(this.Log, this.ServiceConfiguration);
            }
            //     this.Log.Debug(ServiceConfiguration.Name, "QueryDeviceGroupTransformers ended.");
            return si;
        }


        /// <summary>
        /// Selects features and checks if there are near a parent feature.
        /// </summary>
        /// <param name="fc">Featureclass to select features from.</param>
        /// <param name="geom">Polygon to select features with.</param>
        /// <param name="nearFeat">Feature to check distance to selected feature(s).</param>
        /// <param name="dist">Distance selected features and nearFeat use to compare.</param>
        /// <returns></returns>
        private KeyValuePair<string, IFeature> QueryFeatNear(IFeatureClass featureClass, IGeometry geometry, IFeature nearFeat, double dist)
        {
            //     MemoryUsage("QueryFeatNear");
            //     this.Log.Debug(ServiceConfiguration.Name, "QueryFeatNear " + featureClass.AliasName + " " + nearFeat.OID);
            KeyValuePair<string, IFeature> result = new KeyValuePair<string, IFeature>(string.Empty, null);
            ISpatialFilter spatialFilter = new SpatialFilter
            {
                Geometry = geometry,
                GeometryField = featureClass.ShapeFieldName,
                SpatialRel = esriSpatialRelEnum.esriSpatialRelIntersects
            };

            IFeatureCursor featureCursor = featureClass.Search(spatialFilter, false);
            IFeature feature;
            string label = string.Empty;
            while ((feature = featureCursor.NextFeature()) != null)
            {
                if (!IsValidJBox(feature))
                    label = FieldHelper.GetFieldValue(feature, SchemaInfo.Electric.FieldModelNames.OperatingNumber, true); //  "OPERATINGNUMBER");
                else
                    label = FieldHelper.GetFieldValue(feature, _admsLabelField, false);

                if (label.Length > 0)
                {
                    if (IsNear(feature, nearFeat, dist))
                    {
                        result = new KeyValuePair<string, IFeature>(label, feature);
                        break;
                    }
                }
            }

            while (Marshal.ReleaseComObject(featureCursor) > 0) { }
            while (Marshal.ReleaseComObject(spatialFilter) > 0) { }

            return result;
        }


        /// <summary>
        /// Gets closest feature. 
        /// </summary>
        /// <param name="input">Base feature to check distance against.</param>
        /// <param name="list">Features measuring.</param>
        /// <param name="mindist">Minimum distance list can be to input to be returned.</param>
        /// <returns></returns>
        private IFeature GetFeatureByProx(IFeature input, ref List<IFeature> list, double mindist)
        {
            MemoryUsage("GetFeatureByProx");

            list = list.Distinct().ToList();

            IFeature result = null;
            IProximityOperator proximityOperator = input.Shape as IProximityOperator;
            double dist = 9999;
            double d = 9999;
            foreach (IFeature feature in list)
            {
                //  this.Log.Debug(ServiceConfiguration.Name, "Close: " + feature.Class.AliasName + " : " + feature.OID);

                int i = LabelHelpers.GetFeatureEID(feature);
                if (i != 0)
                {
                    d = proximityOperator.ReturnDistance(feature.Shape);
                    if ((d <= dist) && (d <= mindist))
                    {
                        result = feature;
                        dist = d;
                    }
                }
            }
            if (result == null)
            {
                if (mindist > 15)
                    return null;

                result = GetFeatureByProx(input, ref list, (mindist + 3));
            }

            return result;
        }


        /// <summary>
        /// Checks if a feature is in the approved list of featureclasses that can be selected/used.
        ///    - list is a static array _modelNames
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private bool FeatureIsInList(IFeature input)
        {
            bool result = false;

            if (ModelNameFacade.ContainsClassModelName(input.Class, _modelNames) ||
                (input.Class.AliasName.ToUpper() == _electricDistNetworkJunctions))
                result = true;

            return result;
        }

        /// <summary>
        /// Selects based on openpoint.structureguid == [other layer].structureguid.
        /// </summary>
        /// <param name="input"></param>
        /// <param name="layerName"></param>
        /// <returns></returns>
        private KeyValuePair<string, IFeature> UseOtherLayer(IFeature input, string layerName, IWorkspace workspace)
        {
            MemoryUsage("UseOtherLayer");


            KeyValuePair<string, IFeature> result = new KeyValuePair<string, IFeature>(string.Empty, null);
            //    this.Log.Debug(ServiceConfiguration.Name, "UseOtherLayer Started " + input.Class.AliasName);
            try
            {
                SelItem item = new SelItem(this.Log, this.ServiceConfiguration);
                if (item.SetCenterStructGUID(input))
                    result = UseOtherLayer(item, layerName, workspace);

                //     this.Log.Debug(ServiceConfiguration.Name, "UseOtherLayer end");
                return result;
            }
            catch (System.Exception ex)
            {
                //   this.Log.Debug(ServiceConfiguration.Name, "UseOtherLayer Exception: " + ex.Message + " stack: " + ex.StackTrace.ToString());
                return result;
            }
        }

        /// <summary>
        /// Selects based on openpoint.structureguid == [other layer].structureguid.
        /// </summary>
        /// <param name="item"></param>
        /// <param name="layerName"></param>
        /// <returns></returns>
        private KeyValuePair<string, IFeature> UseOtherLayer(SelItem item, string layerName, IWorkspace workspace)
        {
            MemoryUsage("UseOtherLayer2");

            KeyValuePair<string, IFeature> result = new KeyValuePair<string, IFeature>(string.Empty, null);
            IFeatureClass featureClass = LabelHelpers.GetFeatureClass(layerName, workspace);
            IFeature feature = LabelHelpers.GetFeature(featureClass, _structureGUID, item.CenterStructGUID);
            if (feature != null)
            {
                string lbl = FieldHelper.GetFieldValue(feature, SchemaInfo.Electric.FieldModelNames.OperatingNumber, true);
                if (lbl.Length > 0)
                {
                    result = new KeyValuePair<string, IFeature>(lbl, feature);
                }
            }

            return result;
        }

        private int _updateCount = 0;
        /// <summary>
        /// Selects all the DeadBreaks and LoadBreak Elbows in the OpenPoint FeatureClass in a circuit.
        ///    -- sets static collection with feature(s) = LabelGroup.
        /// </summary>
        /// <param name="circuitID">CircuitID you want to use.</param>
        /// <param name="geomNetwork"></param>
        /// <returns>List of OID's.</returns>
        private List<int> ElbowsInCircuit(string circuitID, IGeometricNetwork geomNetwork, IWorkspace workspace)
        {
            MemoryUsage("ElbowsInCircuit");
            _updateCount = 0;
            List<int> result = new List<int>();
            int oid = 0;
            IFeatureCursor cursor = null;
            IQueryFilter queryFilter = null;
            IFeatureClass openPoint = null;
            // Select dead break and load break elbows from openpoint in a single circuit.

            // OpenPoint.SubTypeCD =  1 = load break.
            // OpenPoint.SubTypeCD = 2 = dead break .
            try
            {
                ModelNameFacade.ModelNameManager = Miner.Geodatabase.ModelNameManager.Instance;
                //    this.Log.Debug(ServiceConfiguration.Name, "ElbowsInCircuit Model. 2 ");

                openPoint = ModelNameFacade.FeatureClassByModelName(workspace, SchemaInfo.Electric.ClassModelNames.PGEOpenPoint); // GetNetworkClass(esriFeatureType.esriFTSimpleJunction, geomNetwork, "EDGIS.OPENPOINT");
                                                                                                                                  //    this.Log.Debug(ServiceConfiguration.Name, "ElbowsInCircuit FC.");
                if (openPoint != null)
                {
                    try
                    {
                        queryFilter = new QueryFilterClass
                        {
                            WhereClause = "CircuitID = '" + circuitID + "' AND SubTypeCD in (1,2)"
                        };

                        cursor = openPoint.Search(queryFilter, false);
                        IFeature feat = null;
                        int cnt = 0;
                        int totalCnt = 0;
                        while ((feat = cursor.NextFeature()) != null)
                        {
                            //  this.Log.Debug(ServiceConfiguration.Name, "ElbowsInCircuit " + feat.Class.AliasName + " : " + feat.OID);
                            totalCnt++;
                            cnt++;
                            oid = feat.OID;
                            LabelItems li = new LabelItems(this.Log, this.ServiceConfiguration);
                            li.CenterItem.LoadFeature(feat, FieldHelper.GetFieldValue(feat, SchemaInfo.Electric.FieldModelNames.OperatingNumber, true));
                            li.CenterItem.SetCenterStructGUID(feat);
                            li.CircuitID = circuitID;

                            LabelGroup.Items.Add(li);
                            if (cnt == 50)
                            {
                                //    this.Log.Debug(ServiceConfiguration.Name, "ElbowsInCircuit 50s M:" + GC.GetTotalMemory(false).ToString());
                                FindLabels_xx(circuitID, geomNetwork, workspace);
                                //    this.Log.Info(ServiceConfiguration.Name, "Elbow Find End 50.");
                                cnt = 0;
                            }
                            result.Add(feat.OID);
                        }
                        // last group
                        //    this.Log.Info(ServiceConfiguration.Name, "Elbow Find Start");
                        FindLabels_xx(circuitID, geomNetwork, workspace);

                        this.Log.Info(ServiceConfiguration.Name, "CircuitID " + circuitID + " Feature Count: " + totalCnt);
                        this.Log.Info(ServiceConfiguration.Name, "CircuitID " + circuitID + " Feature updates: " + _updateCount);


                        if (cursor != null) { while (Marshal.ReleaseComObject(cursor) > 0) { } }
                        if (queryFilter != null) { while (Marshal.ReleaseComObject(queryFilter) > 0) { } }
                    }
                    catch (System.Exception ex)
                    {
                        this.Log.Error(ServiceConfiguration.Name, "Error loading ADMSLabel ElbowInCirc: " + ex.Message + " Stack: " + ex.StackTrace.ToString() + " : OID: " + oid);
                        throw ex;
                    }
                    finally
                    {
                        if (cursor != null) { while (Marshal.ReleaseComObject(cursor) > 0) { } }
                        if (queryFilter != null) { while (Marshal.ReleaseComObject(queryFilter) > 0) { } }
                    }
                }

                return result;
            }
            catch (System.Exception ex)
            {
                this.Log.Error(ServiceConfiguration.Name, "ADMSLabel ElbowsInCirc " + ex.Message + " Stack: " + ex.StackTrace.ToString());
                throw ex;
            }
            finally
            {
                if (cursor != null) { while (Marshal.ReleaseComObject(cursor) > 0) { } }
                if (queryFilter != null) { while (Marshal.ReleaseComObject(queryFilter) > 0) { } }
                if (openPoint != null) { while (Marshal.ReleaseComObject(openPoint) > 0) { } }
            }
        }

        #endregion

        #region "Get Ciruits"

        /// <summary>
        /// Get any edited circuit in this version we may want.
        /// </summary>
        /// <param name="versionName"></param>
        /// <param name="workSpace"></param>
        /// <returns></returns>
        //private int GetEditedCircuits(string versionName, IWorkspace workSpace)
        //{
        //    int result = 0;
        //    //  string circuitID = string.Empty;
        //    string layerName = string.Empty;


        //    // loop each record
        //    List<string> fulllist = SelectStageData(versionName, workSpace);
        //    foreach (string circuitID in fulllist)
        //    {
        //        if (!_circuitsEdited.ContainsKey(circuitID))
        //        {
        //            _circuitsEdited.Add(circuitID, CircuitStatus.NEW);
        //            result++;
        //        }
        //    }

        //    this.Log.Info(ServiceConfiguration.Name, "Selection GetEditCircuits Count: " + result);
        //    return result;
        //}

        /// <summary>
        /// Get circuits modified that have features with OperatingNumber.
        /// </summary>
        /// <param name="versionName"></param>
        /// <param name="workspace"></param>
        /// <returns></returns>
        private List<string> SelectStageData_RegisteredTable(string versionName, IWorkspace workspace)
        {
            List<string> result = new List<string>();
            IQueryFilter qf = new QueryFilterClass();
            ITable table = null;
            IFeatureWorkspace fws = (IFeatureWorkspace)workspace;
            ICursor cursor = null;

            try
            {
                table = fws.OpenTable("PGEDATA.PGE_GDBM_AH_INFO");
                if (table != null)
                {
                    List<string> list = GetAdmsDeviceClassNames(workspace);
                    string inclause = ArrayToCommaString(list, "','");

                    if (inclause.Length > 0)
                    {
                        // VERSIONNAME = 'GIS_I.SN_723637' AND UPPER(FEATURECLASSNAME) IN ('EDGIS.SWITCH','EDGIS.PADMOUNT')
                        //  qf.SubFields = "CIRCUITID";
                        qf.WhereClause = "VERSIONNAME = '" + versionName + "' AND UPPER(FEATURECLASSNAME) IN (" + inclause + ")";
                        cursor = table.Search(qf, false);
                        IRow row = cursor.NextRow();
                        int idx = table.FindField("CircuitID");

                        while (row != null)
                        {
                            object obj = row.get_Value(idx);
                            if (obj != null)
                                result.Add(obj.ToString());

                            row = cursor.NextRow();
                        }
                    }

                }

                return result;
            }
            catch (System.Exception ex)
            {
                this.Log.Error(ServiceConfiguration.Name, "ERROR SelectSageData Msg: " + ex.Message + "  Stack: " + ex.StackTrace.ToString());
                return result;
            }
            finally
            {
                if (cursor != null) { while (Marshal.ReleaseComObject(cursor) > 0) { } }
                if (qf != null) { while (Marshal.ReleaseComObject(qf) > 0) { } }
            }
        }

        /// <summary>
        /// Get circuits modified that have features with OperatingNumber.
        ///   For an unregistered table.
        /// </summary>
        /// <param name="versionName"></param>
        /// <param name="workspace"></param>
        /// <returns></returns>
        //private List<string> SelectStageData(string versionName, ESRI.ArcGIS.Geodatabase.IWorkspace workspace)
        //{
        //    List<string> result = new List<string>();

        //    try
        //    {
        //        List<string> list = GetAdmsDeviceClassNames(workspace);
        //        string inclause = ArrayToCommaString(list, "','");
        //        string where = "VERSIONNAME = '" + versionName + "' AND UPPER(FEATURECLASSNAME) IN (" + inclause + ")";
        //        string sql = "Select CIRCUITID FROM PGEDATA.PGE_GDBM_AH_INFO " +
        //                          "WHERE " + where;

        //    //    Telvent.PGE.ED.Desktop.GDBM.LabelHelpers common = new PGE.Desktop.EDER.GDBM.Common();
        //        ADODB.Recordset recSet = Telvent.PGE.ED.Desktop.GDBM.LabelHelpers.GetRecordset(sql, workspace);
        //        if (recSet != null)
        //        {
        //            if (recSet.RecordCount > 0)
        //            {
        //                recSet.MoveFirst();
        //                while (!recSet.EOF)
        //                {

        //                    string v = recSet.Fields[0].Value.ToString();

        //                    if (!String.IsNullOrEmpty(v))
        //                        result.Add(v);

        //                    recSet.MoveNext();

        //                }
        //                recSet.Close();
        //            }
        //        }
        //        this.Log.Debug(ServiceConfiguration.Name, "SelectStageData found " + result.Count());

        //        return result;
        //    }
        //    catch (System.Exception ex)
        //    {
        //        this.Log.Error(ServiceConfiguration.Name, "ERROR SelectSageData Msg: " + ex.Message + "  Stack: " + ex.StackTrace.ToString());
        //        return result;
        //    }
        //    finally
        //    {

        //    }
        //}

        /// <summary>
        /// Make comma seperated string from list.
        /// </summary>
        /// <param name="workspace"></param>
        /// <returns></returns>
        private string ArrayToCommaString(List<string> list, string seperator)
        {
            string result = string.Empty;

            if (list.Count() > 0)
            {
                if (seperator.Contains("'"))
                    result = "'" + string.Join(seperator, list.ToArray()) + "'";
                else
                    result = string.Join(seperator, list.ToArray());
            }


            return result;
        }

        /// <summary>
        /// Get list of featureclasses.
        /// </summary>
        /// <param name="workspace"></param>
        /// <returns></returns>
        private List<string> GetAdmsDeviceClassNames(IWorkspace workspace)
        {
            MemoryUsage("GetAdmsDeviceClassNames");

            IFeatureWorkspace featWorkspace = workspace as IFeatureWorkspace;
            List<int> result = new List<int>();

            List<string> classNameList = new List<string>();
            // Get a list of feature class with the adms device ID and then iterate over that networks feature classes for classes with the operating number or admslabel field.
            IEnumBSTR classNamesForADMS = ModelNameManager.Instance.ClassNamesFromModelNameWS(workspace, SchemaInfo.Electric.ClassModelNames.OperatingNumber);

            classNamesForADMS.Reset();
            string admsname = "";
            while (!string.IsNullOrEmpty((admsname = classNamesForADMS.Next())))
            {
                classNameList.Add(admsname.ToUpper());
            }

            // Add DeviceGroup Featureclass too.
            classNamesForADMS = ModelNameManager.Instance.ClassNamesFromModelNameWS(workspace, SchemaInfo.Electric.ClassModelNames.DeviceGroup);

            classNamesForADMS.Reset();
            admsname = "";
            while (!string.IsNullOrEmpty((admsname = classNamesForADMS.Next())))
            {
                classNameList.Add(admsname.ToUpper());
            }

            return classNameList;
        }

        #endregion

        #region "Update features"

        /// <summary>
        /// Mass update OpenPoint.ADMSLabel.
        /// </summary>
        /// <param name="dictionary">List of OpenPoint.ObjectIDs and associated ADMSLabel value.</param>
        /// <returns></returns>
        private bool UpdateOpenPoint(Dictionary<int, string> dictionary, string circuitid, IGeometricNetwork network, IWorkspace workspace)
        {
            MemoryUsage("UpdateOpenPoint");

            bool results = true;
            IFeatureCursor cursor = null;
            IWorkspaceEdit wkspcEdit = (IWorkspaceEdit)workspace;
            string step = "TOP";
            try
            {
                IFeatureClass openPoint = ModelNameFacade.FeatureClassByModelName(workspace, SchemaInfo.Electric.ClassModelNames.PGEOpenPoint);  // GetNetworkClass(esriFeatureType.esriFTSimpleJunction, network, "EDGIS.OPENPOINT");

                if (!wkspcEdit.IsBeingEdited())
                {
                    //  this.Log.Info(ServiceConfiguration.Name, "ADMS Label Update Starting Edit session in Update Func.");

                    wkspcEdit.StartEditing(false);
                    wkspcEdit.StartEditOperation();
                }

                if (wkspcEdit.IsBeingEdited())
                {
                    step = "Editing";
                    if (dictionary.Count() > 0)
                    {
                        int idone = 0;

                        IQueryFilter queryFilter = new QueryFilterClass();

                        // Only need deadbreak and load elbows.
                        string where = "SubtypeCD in (1,2) AND CircuitID = '" + circuitid + "'";
                        if (circuitid.Length <= 0)
                            where = "SubtypeCD in (1,2) AND CircuitID is NULL";

                        queryFilter.WhereClause = where;
                        queryFilter.AddField("ADMSLABEL");
                        //       this.Log.Debug(ServiceConfiguration.Name, "ADMS Label Feature Updated before cursor.");
                        cursor = openPoint.Update(queryFilter, false);
                        step = "After Cursor";
                        IFeature feature = null;
                        string value = string.Empty;

                        int idx = openPoint.FindField("ADMSLABEL");
                        step = "ADMS FIELD";
                        //     this.Log.Debug(ServiceConfiguration.Name, "ADMS Label Feature Updated Loop ");

                        while ((feature = cursor.NextFeature()) != null)
                        {
                            //     this.Log.Debug(ServiceConfiguration.Name, "ADMS Label Feature examine: " + feature.OID);

                            // There is no need to do more then in the dictionary.
                            if (dictionary.Count() <= idone)
                                break;

                            //  this.Log.Info(ServiceConfiguration.Name,"ADMS Label Feature Updated Loop " + feature.OID);

                            if (dictionary.ContainsKey(feature.OID))
                            {
                                //   this.Log.Debug(ServiceConfiguration.Name, "ADMS Label Feature Updated IN " + feature.OID);

                                var v = dictionary.Where(x => x.Key == feature.OID).ToDictionary(x => x.Key, x => x.Value).First();
                                step = "Loop " + v.Value;
                                // If value is same, no need to update the feature.
                                if (FieldHelper.GetFieldValue(feature, _admsLabelField, false) != v.Value)
                                {
                                    step = "Before " + v.Value;
                                    //   this.Log.Info(ServiceConfiguration.Name, "ADMS Label Feature Updated: " + idx + " : " + v.Value + " : " + feature.OID);

                                    feature.set_Value(idx, v.Value);
                                    step = "Editing SET " + v.Value;
                                    feature.Store();
                                    _updateCount++;
                                    step = "Editing STORE";
                                }
                                idone++;
                            }

                        }
                    }

                    //     this.Log.Debug(ServiceConfiguration.Name, "ADMS Label examined: " + dictionary.Count());
                    return results;
                }
                else
                {
                    this.Log.Warn(ServiceConfiguration.Name, "ADMS Label Updated not in an Edit session");
                    return results;
                }
            }
            catch (System.Exception ex)
            {
                this.Log.Error(ServiceConfiguration.Name, "Error Updating ADMSLabel : " + ex.Message + " Stack: " + ex.StackTrace.ToString() +
                    " STEP: " + step);
                throw ex;
            }
            finally
            {
                if (cursor != null) { while (Marshal.ReleaseComObject(cursor) > 0) { } }
            }
        }

        #endregion

        #region "Label Area"


        /// <summary>
        /// Class to hold collection related to the admslabel.
        /// </summary>
        private static class LabelGroup
        {
            /// <summary>
            /// Initialize class.
            /// </summary>
            internal static void Init()
            {

                Items = new List<LabelItems>();
            }

            /// <summary>
            /// Initialize list.
            /// </summary>
            internal static List<LabelItems> Items { get; set; }
        }

        /// <summary>
        /// Holds each individual object that makes up label.
        /// </summary>
        internal class LabelItems
        {
            private Miner.Geodatabase.GeodatabaseManager.Logging.INameLog Log;
            private Miner.Geodatabase.GeodatabaseManager.Serialization.GdbmReconcilePostService ServiceConfiguration;

            /// <summary>
            /// Constructor.
            /// </summary>
            internal LabelItems(Miner.Geodatabase.GeodatabaseManager.Logging.INameLog log, Miner.Geodatabase.GeodatabaseManager.Serialization.GdbmReconcilePostService serviceConfig)
            {
                this.Log = log;
                this.ServiceConfiguration = serviceConfig;
                this.CenterItem = new SelItem(this.Log, this.ServiceConfiguration);
                this.NextItem = new SelItem(this.Log, this.ServiceConfiguration);
                this.StartItem = new SelItem(this.Log, this.ServiceConfiguration);
                this.ViaItem = new SelItem(this.Log, this.ServiceConfiguration);

                this.ViaList = new List<SelItem>();

                this.Is50Scale = false;

                this.Comments = string.Empty;
            }

            internal LabelItems()
            {
                this.CenterItem = new SelItem();
                this.NextItem = new SelItem();
                this.StartItem = new SelItem();
                this.ViaItem = new SelItem();

                this.ViaList = new List<SelItem>();

                this.Is50Scale = false;

                this.Comments = string.Empty;
            }

            #region Properties

            /// <summary>
            /// Original point (OpenPoint/Elbow).
            /// </summary>
            internal SelItem CenterItem { get; set; }

            /// <summary>
            /// Starting labelitem.
            /// </summary>
            internal SelItem StartItem { get; set; }

            /// <summary>
            /// Second part of label.
            /// </summary>
            internal SelItem NextItem { get; set; }

            /// <summary>
            /// Viaitem is the ### TWD ### AND ViaItem[].
            /// </summary>
            internal SelItem ViaItem { get; set; }

            internal List<SelItem> ViaList { get; set; }
            /// <summary>
            /// Circuit items are related too.
            /// </summary>
            internal string CircuitID { get; set; }

            /// <summary>
            /// Check both start and next are populated.
            /// </summary>
            internal bool Full
            {
                get
                {
                    if ((this.StartItem.FCName != null) && (this.NextItem.Feature != null))
                        return true;
                    else
                        return false;
                }
            }

            /// <summary>
            /// Side notes.
            /// </summary>
            internal string Comments { get; set; }

            #endregion

            /// <summary>
            /// Is area 50 scale or not.
            /// </summary>
            internal bool Is50Scale { get; set; }

            /// <summary>
            /// Gets complete label.
            /// </summary>
            /// <returns>Adms label value.</returns>
            internal string Label()
            {
                string result = string.Empty;
                try
                {
                    string opnumber = FieldHelper.GetFieldValue(this.CenterItem.Feature, SchemaInfo.Electric.FieldModelNames.OperatingNumber, true);
                    if ((this.CenterItem.FCName != null) && (this.CenterItem.FCName.Length > 0))
                    {
                        // CenterItem is OpenPoint so shift it to StartItem unless the start is a transformer.
                        if ((!this.StartItem.FCName.Contains("TRANSFORMER")))
                            this.StartItem = this.CenterItem;

                    }

                    if ((this.StartItem.Label != null) && (this.StartItem.Label.Length > 0))
                    {
                        result += this.StartItem.Label;
                        List<string> sArray = new List<string>() { this.StartItem.Label };

                        if ((this.NextItem.Label != null) && (this.NextItem.Label.Length > 0))
                        {
                            result += " TWD " + this.NextItem.Label;
                            if ((this.ViaItem.Label != null) && (this.ViaItem.Label.Length > 0))
                            {
                                sArray.Add(this.NextItem.Label);
                                if (!sArray.Exists(x => x == this.ViaItem.Label))
                                {
                                    result += " & " + this.ViaItem.Label;
                                    sArray.Add(this.ViaItem.Label);
                                }
                            }

                            foreach (SelItem s in this.ViaList)
                            {
                                if ((s.Label != null) && (s.Label.Length > 0))
                                {
                                    // Field is only 50 chars, so we don't want a string longer then that (DeviceGroup.ADMSLabel could cause this).
                                    //  + 3 is for " & ".
                                    if ((result.Length + s.Label.Length + 3) < 50)
                                    {
                                        if (!sArray.Exists(x => x == s.Label))
                                        {
                                            result += " & " + s.Label;
                                            sArray.Add(s.Label);
                                        }

                                        // Only want one & if 50.
                                        if (this.Is50Scale)
                                            break;
                                    }
                                }
                            }
                        }
                        else
                        {
                            result += " TWD EOL";
                        }
                    }
                    else
                    {
                        string subtype = FieldHelper.GetFieldValue(this.CenterItem.Feature, _subtypeCD);
                        // 1 = Load Break, 2 = Dead Break
                        if (subtype != null)
                        {
                            if ("1" == subtype)
                            {
                                result = "LBSC";
                            }
                            else if ("2" == subtype)
                            {
                                result = "DBSC";
                            }
                        }

                    }
                    return result;
                }
                catch (System.Exception ex)
                {
                    this.Log.Error(this.ServiceConfiguration.Name, "Error: Label - " + ex.Message + " stack: " + ex.StackTrace.ToString() +
                        " Source: " + ex.Source);

                    throw ex;
                }
            }

            internal bool HasValue(string input)
            {
                bool result = false;
                if (input.Length > 0)
                {
                    if (input == this.StartItem.Label || input == this.NextItem.Label || this.CenterItem.Label == input)
                    {
                        result = true;
                        return result;
                    }

                    foreach (SelItem si in this.ViaList)
                    {
                        if (si.Label == input)
                        {
                            result = true;
                            break;
                        }
                    }
                }
                return result;
            }

        }

        /// <summary>
        /// Individual label item.
        /// </summary>
        internal class SelItem
        {
            #region private class variables
            private Miner.Geodatabase.GeodatabaseManager.Logging.INameLog Log;
            private Miner.Geodatabase.GeodatabaseManager.Serialization.GdbmReconcilePostService ServiceConfiguration;


            #endregion

            /// <summary>
            /// Construtcor.
            /// </summary>
            internal SelItem()
            {
                this.Feature = null;
                this.Label = string.Empty;
                this.FCName = string.Empty;
            }

            /// <summary>
            /// Construct.
            /// </summary>
            internal SelItem(Miner.Geodatabase.GeodatabaseManager.Logging.INameLog log, Miner.Geodatabase.GeodatabaseManager.Serialization.GdbmReconcilePostService serviceConfig)
            {
                this.Comments = string.Empty;
                this.FCName = null;
                this.Log = log;
                this.ServiceConfiguration = serviceConfig;
                this.Feature = null;
                this.Label = null;
            }

            /// <summary>
            /// Load a feature.
            /// </summary>
            /// <param name="input">feature</param>
            /// <param name="labelvalue">label value</param>
            /// <param name="posFeature">When input is not a network feature, pass feature that is part of network to store eid.</param>
            /// <returns>Did the feature load.</returns>
            internal bool LoadFeature(IFeature input, string labelvalue, IFeature posFeature)
            {
                bool result = false;
                try
                {
                    if (LoadFeature(input, labelvalue))
                    {
                        // Device group is not connected to network so we use the junction if we have it.

                        if ((this.FCName != null) && (ModelNameFacade.ContainsClassModelName(input.Class, SchemaInfo.Electric.ClassModelNames.DeviceGroup)))
                        {
                            if ((posFeature.Class as IDataset).Name.ToUpper() == _electricDistNetworkJunctions)
                            {

                                this.EID = LabelHelpers.GetFeatureEID(posFeature);
                                result = true;
                            }
                        }
                    }
                    return result;
                }
                catch (System.Exception ex)
                {
                    this.Log.Error(ServiceConfiguration.Name, "Error: SelItem.LoadFeature " + input.Class.AliasName + " ObjectID: " + input.OID + " message: " + ex.Message + " stack: " + ex.StackTrace.ToString());
                    return result;
                }
            }

            /// <summary>
            /// Load a feature.
            /// </summary>
            /// <param name="input">feature to load</param>
            /// <param name="labelvalue">label value</param>
            /// <returns>Did feature load.</returns>
            internal bool LoadFeature(IFeature input, string labelvalue)
            {

                // Device gropus with facility type (DEVICEGROUPTYPE).
                // Use ADMSLabel instead of OperatingNumber.
                //   DeviceGroup.subtypecd = 3  padmount ; DeviceGroup.DEVICEGROUPTYPE = 36.
                //   DeviceGroup.subtypecd = 2  subsurface ;DeviceGroup.DEVICEGROUPTYPE  = 26.
                //  Related by OpenPoint.StructureGUID = DeviceGroup.GlobalID.

                bool result = false;
                try
                {
                    this.Feature = input;
                    this.FCName = (input.Class as IDataset).Name.ToUpper();
                    this.OID = input.OID;

                    this.EID = LabelHelpers.GetFeatureEID(input);
                    if (labelvalue.Length > 0)
                    {
                        this.Label = labelvalue;
                        result = true;
                    }
                    else
                    {
                        // Empty field name is from centr feature.
                        result = true;
                    }
                    return result;
                }
                catch (System.Exception ex)
                {
                    this.FCName = null;
                    this.Feature = null;
                    this.Log.Error(ServiceConfiguration.Name, "Error: SelItem.LoadFeature(2) " + input.Class.AliasName + " ObjectID: " + input.OID + " message: " + ex.Message + " stack: " + ex.StackTrace.ToString());
                    return false;
                }

            }

            /// <summary>
            /// Structureguid of feature.
            /// </summary>
            /// <param name="input"></param>
            /// <returns></returns>
            internal bool SetCenterStructGUID(IFeature input)
            {
                bool result = false;

                string value = FieldHelper.GetFieldValue(input, _structureGUID);
                if (value.Length > 0)
                {
                    this.CenterStructGUID = value;
                    result = true;
                }
                return result;
            }

            #region Properties

            /// <summary>
            /// Feature class name.
            /// </summary>
            internal string FCName { get; set; }

            /// <summary>
            /// Objectid.
            /// </summary>
            internal int OID { get; set; }

            /// <summary>
            /// Network. 
            /// </summary>
            internal int EID { get; set; }

            /// <summary>
            /// Features label value.
            /// </summary>
            internal string Label { get; set; }

            /// <summary>
            /// OpenPoint structureguid.
            /// </summary>
            internal string CenterStructGUID { get; set; }

            /// <summary>
            /// Input feature.
            /// </summary>
            internal IFeature Feature { get; set; }

            /// <summary>
            /// Edge found between starting openpoint and starting label feature.
            /// </summary>
            internal int BaseEdgeEID { get; set; }

            /// <summary>
            /// 0 = start.
            /// 1 = center.
            /// 2 = end/next.
            /// </summary>
            internal int Index { get; set; }

            /// <summary>
            /// Side notes (more for debugging).
            /// </summary>
            internal string Comments { get; set; }

            #endregion

        }

        #endregion

        /// <summary>
        /// Enabled property.
        /// </summary>
        /// <param name="actionType"></param>
        /// <param name="gdbmAction"></param>
        /// <param name="versionType"></param>
        /// <returns></returns>
        public override bool Enabled(Miner.Geodatabase.GeodatabaseManager.ActionType actionType, Miner.Geodatabase.GeodatabaseManager.Actions gdbmAction, Miner.Geodatabase.GeodatabaseManager.PostVersionType versionType)
        {
            if ((actionType == Miner.Geodatabase.GeodatabaseManager.ActionType.Post) && (gdbmAction == Miner.Geodatabase.GeodatabaseManager.Actions.BeforePost))
            {
                return true;
            }

            return false;
        }

        private void MemoryUsage(string input)
        {
            //  System.Diagnostics.Process process = System.Diagnostics.Process.GetCurrentProcess();
            //  this.Log.Info(ServiceConfiguration.Name, "ADMS MEM: " + process.PrivateMemorySize64 + " : " + input);
        }

        #region "ADMS Updated Version Check"
        #region private class variables

        private List<QTableItem> _QTableItems = null;

        #endregion


        /// <summary>
        /// Checks if updated circuit needs to run through ADMSLabel.
        /// </summary>
        /// <param name="versionName"></param>
        /// <param name="file"></param>
        /// <param name="affectedCircuits"></param>
        /// <param name="workspace"></param>
        /// <returns>List of circuits that qualify.</returns>
        internal List<string> CheckUpdatedCircuits(string versionName, string file, List<string> affectedCircuits, IWorkspace workspace)
        {
            List<string> results = new List<string>();
            string sql;
            int cnt = 0;

            try
            {
                //    this.Log.Debug(ServiceConfiguration.Name, "ADMS Update Diff Config reading. ");
                ReadXMLFile(file, workspace);

                foreach (string c in affectedCircuits)
                {
                    this.Log.Debug(ServiceConfiguration.Name, "ADMS Update Circ: " + c);

                    cnt = 0;
                    foreach (QTableItem q in _QTableItems)
                    {
                        //   this.Log.Debug(ServiceConfiguration.Name, "  ADMS Update Q: " + q.imv_view_name);
                        sql = q.BuildSQL_Attributes(c, versionName);
                        //     this.Log.Debug(ServiceConfiguration.Name, "  ADMS Update ASQL: " + sql);
                        if (sql.Length > 0)
                            cnt = GetCount(sql, workspace);

                        // No need to check shape diff if we already found attr diffs.
                        if (cnt == 0)
                        {
                            sql = q.BuildSQL_Shape(c, versionName);
                            if (sql.Length > 0)
                                cnt = GetCount(sql, workspace);
                        }
                        if (cnt > 0)
                        {
                            results.Add(c);
                            break;
                        }
                    }
                }
                //   this.Log.Debug(ServiceConfiguration.Name, "ADMS Update results: " + results.Count());

                return results;
            }
            catch (System.Exception ex)
            {
                this.Log.Error(ServiceConfiguration.Name, "ADMS Update Check Warning: " + ex.Message + " : " + ex.StackTrace.ToString());
                throw ex;
            }
        }

        private ADODB.Recordset GetRecordset(string strQuery, IWorkspace workspace)
        {
            ADODB.Recordset rset = new ADODB.Recordset();
            ADODB.Connection adoConnection = new ADODB.ConnectionClass();
            try
            {
                rset.CursorLocation = ADODB.CursorLocationEnum.adUseClient;
                ESRI.ArcGIS.DataSourcesOleDB.IFDOToADOConnection fdoToadoConnection = new ESRI.ArcGIS.DataSourcesOleDB.FdoAdoConnectionClass();
                //    this.Log.Debug(ServiceConfiguration.Name, "ADMS Update RSET Conn.");
                adoConnection = fdoToadoConnection.CreateADOConnection(workspace) as ADODB.Connection;
                //   this.Log.Debug(ServiceConfiguration.Name, "ADMS Update RSET Conn After.");
                if (adoConnection != null)
                {
                    //   this.Log.Debug(ServiceConfiguration.Name, "ADMS Update RSET Open.");
                    rset.Open(strQuery, adoConnection, ADODB.CursorTypeEnum.adOpenForwardOnly, ADODB.LockTypeEnum.adLockReadOnly, 0);
                    //   this.Log.Debug(ServiceConfiguration.Name, "ADMS Update RSET Open After.");
                }

            }
            catch (System.Exception ex)
            {
                this.Log.Error(ServiceConfiguration.Name, "ADMS Update RSET Error: " + ex.Message + " : " + ex.StackTrace.ToString());
                rset = null;
            }
            finally
            {
                //if (_adoConnection.State == 1)
                //{
                //    _adoConnection.Close();
                //}
            }
            return rset;
        }

        /// <summary>
        /// Storage for checking Updated diff.
        /// </summary>
        private class QTableItem
        {
            /// <summary>
            /// Initializer.
            /// </summary>
            internal QTableItem()
            {
                fields = new List<string>();
            }
            internal string tableName { get; set; }
            internal int registration_id { get; set; }
            internal string imv_view_name { get; set; }
            internal List<string> fields { get; set; }

            /// <summary>
            /// Builds query for attribute diff check.
            /// </summary>
            /// <param name="circuitID"></param>
            /// <param name="sde_state_id"></param>
            /// <returns></returns>
            internal string BuildSQL_Attributes(string circuitID, string vname)
            {
                string result = string.Empty;
                string aLeader = "EDGIS.a" + registration_id;
                string bLeader = "EDGIS." + imv_view_name;
                string noFieldQry = "Select " + aLeader + ".objectid from " + aLeader + " " +
                        "where " + aLeader + ".VersionName  = '" + vname + "' and " + aLeader + ".circuitid = '" + circuitID + "'";

                if (fields.Count() > 0)
                {


                    string aflds = string.Empty;
                    string bflds = string.Empty;

                    foreach (string f in fields)
                    {
                        if (f.ToUpper() != "SHAPE")
                        {
                            if (aflds.Length <= 0)
                            {
                                aflds = aLeader + "." + f;
                                bflds = bLeader + "." + f;
                            }
                            else
                            {
                                aflds += " || " + aLeader + "." + f;
                                bflds += " || " + bLeader + "." + f;
                            }
                        }
                    }

                    if (aflds.Length > 0)
                        result = "Select " + aLeader + ".objectid from " + aLeader + ", " + bLeader +
                            " where  " + aLeader + ".VersionName  = '" + vname +
                            "' and  " + aLeader + ".circuitid = '" + circuitID + "' and " +
                            aLeader + ".objectid =  " + bLeader + ".objectid  and " +
                            "(" + aflds + " <> " + bflds + ")";
                    else
                        result = string.Empty;
                }
                else
                {
                    // Featureclasses without fields, this is when we only care if a feature is edited in this session and not about any fields or geometries.
                    result = noFieldQry;
                }

                return result;
            }

            /// <summary>
            /// Builds query for shape/geometry diff check.
            /// </summary>
            /// <param name="circuitID"></param>
            /// <param name="sde_state_id"></param>
            /// <returns></returns>
            internal string BuildSQL_Shape(string circuitID, string vname)
            {
                string result = string.Empty;
                string aLeader = "EDGIS.a" + registration_id;
                string bLeader = "EDGIS." + imv_view_name;

                result = "Select " + aLeader + ".objectid from " + aLeader + ", " + bLeader +
                        " where " + aLeader + ".VersionName  = '" + vname + "' and " + aLeader + ".circuitid = '" + circuitID + "' and " +
                        aLeader + ".ObjectID = " + bLeader + ".ObjectID and " +
                        "SDE.ST_EQUALS(" + bLeader + ".Shape, " + aLeader + ".shape) <> 1";

                return result;
            }
        }

        /// <summary>
        /// Read config file for features for Update diff.
        /// </summary>
        /// <param name="file"></param>
        /// <param name="workspace"></param>
        private void ReadXMLFile(string file, IWorkspace workspace)
        {
            _QTableItems = new List<QTableItem>();
            QTableItem q = new QTableItem();
            try
            {
                this.Log.Debug(ServiceConfiguration.Name, "ADMS Update XML.");
                using (XmlReader reader = XmlReader.Create(new StreamReader(file)))
                {
                    string fcname = string.Empty;
                    List<string> fields = new List<string>();
                    string fldname = string.Empty;

                    while (reader.Read())
                    {
                        switch (reader.NodeType)
                        {
                            case XmlNodeType.Element:
                                if (reader.Name.ToUpper() == "OBJECTCLASS")
                                {
                                    if (q.fields.Count() > 0)
                                        _QTableItems.Add(q);

                                    fields = new List<string>();
                                    fcname = reader.GetAttribute("Name").ToUpper();
                                    q = new QTableItem();
                                    q.tableName = fcname;
                                    KeyValuePair<int, string> kvp = Get_RegId_View(q.tableName, workspace);
                                    q.registration_id = kvp.Key;
                                    q.imv_view_name = kvp.Value;

                                }
                                else if (reader.Name.ToUpper() == "FIELD")
                                {
                                    fldname = reader.GetAttribute("Name").ToUpper();
                                    q.fields.Add(fldname);
                                }

                                break;
                        }
                    }
                    if (q.fields.Count() > 0)
                        _QTableItems.Add(q);

                }
            }
            catch (System.Exception ex)
            {
                this.Log.Error(ServiceConfiguration.Name, "ADMS Update XML Warning: " + ex.Message + " : " + ex.StackTrace.ToString());
                throw ex;
            }
        }

        /// <summary>
        /// Get count of records that match query.
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="workspace"></param>
        /// <returns></returns>
        private int GetCount(string sql, IWorkspace workspace)
        {
            int result = 0;
            try
            {
                ADODB.Recordset pRSet = GetRecordset(sql, workspace);

                if (pRSet.RecordCount > 0)
                {
                    pRSet.MoveFirst();

                    string value = pRSet.RecordCount.ToString(); // pRSet.Fields[0].Value.ToString();
                    if (!string.IsNullOrEmpty(value))
                    {
                        result = int.Parse(value);
                    }

                }

                pRSet.Close();

                return result;
            }
            catch (System.Exception ex)
            {
                this.Log.Error(ServiceConfiguration.Name, "ADMS Update Count Warning: " + ex.Message + " : " + ex.StackTrace.ToString());
                throw ex;
            }
        }

        /// <summary>
        /// Get regid from sde for table/featureclass.
        /// </summary>
        /// <param name="input"></param>
        /// <param name="workspace"></param>
        /// <returns></returns>
        private KeyValuePair<int, string> Get_RegId_View(string input, IWorkspace workspace)
        {
            KeyValuePair<int, string> result = new KeyValuePair<int, string>();

            string name = string.Empty;
            string owner = string.Empty;
            if (input.Contains("."))
            {
                string[] arr = input.Split(Convert.ToChar("."));
                owner = arr[0];
                name = arr[1];
            }
            else
            {
                owner = "EDGIS";
                name = input;
            }
            string sql = "select registration_id, imv_view_name from sde.table_registry where table_name = '" + name + "' and owner = '" + owner + "'";

            ADODB.Recordset pRSet = GetRecordset(sql, workspace);
            pRSet.MoveFirst();
            if (pRSet.RecordCount > 0)
            {
                int i = int.Parse(pRSet.Fields[0].Value.ToString());
                string s = pRSet.Fields[1].Value.ToString();
                result = new KeyValuePair<int, string>(i, s);
            }
            pRSet.Close();

            return result;
        }

        /// <summary>
        /// Get state id from sde for version.
        /// </summary>
        /// <param name="input"></param>
        /// <param name="workspace"></param>
        /// <returns></returns>
        private int Get_StateID(string input, IWorkspace workspace)
        {
            int result = -1;
            // select distinct state_id from sde.versions where NAME = 'SN_724040' and Owner = 'GIS_I';
            string name = string.Empty;
            string owner = string.Empty;
            if (input.Contains("."))
            {
                string[] arr = input.Split(Convert.ToChar("."));
                owner = arr[0];
                name = arr[1];

                string sql = "select distinct state_id from sde.versions where NAME = '" + name + "' and Owner = '" + owner + "'";

                ADODB.Recordset pRSet = GetRecordset(sql, workspace);
                pRSet.MoveFirst();
                if (pRSet.RecordCount > 0)
                {
                    int i = int.Parse(pRSet.Fields[0].Value.ToString());
                }
                pRSet.Close();

            }
            return result;
        }
        #endregion

        /// <summary>
        /// Build ADMSlabel Value.
        /// </summary>
        /// <param name="circuitID"></param>
        /// <param name="network"></param>
        /// <param name="targetWorkspace"></param>
        /// <returns></returns>
        private bool FindLabels_xx(string circuitID, IGeometricNetwork network, IWorkspace targetWorkspace)
        {
            bool result = true;
            try
            {
                // We're going to do the update one each circuit.
                Dictionary<int, string> updateFeatures = new Dictionary<int, string>();

                // Use less connected features for 50 scale.
                // Only checking circuit against first elbow, it may fall outside/inside 50 poly while others do not.
                bool is50scale = false;
                if (LabelGroup.Items.Count > 0)
                {
                    //  is50scale = LabelHelpers.IsFiftyScale(LabelGroup.Items[0].CenterItem.Feature.Shape, targetWorkspace);
                    //   this.Log.Info(ServiceConfiguration.Name, "Circuit is 50 scale: " + is50scale);
                }
                List<int> admsLabelDeviceClassIDs = GetAdmsDeviceClassIDs(targetWorkspace);
                Walking walk = new Walking(this.Log, ServiceConfiguration.Name);

                //   this.Log.Info(ServiceConfiguration.Name, "FL Top: " + admsLabelDeviceClassIDs.Count());
                // Loop our collection.
                foreach (LabelItems l in LabelGroup.Items)
                {
                    //     this.Log.Info(ServiceConfiguration.Name, "L: " + l.CenterItem.OID);
                    string opnumber = FieldHelper.GetFieldValue(l.CenterItem.Feature, SchemaInfo.Electric.FieldModelNames.OperatingNumber, true);
                    //      this.Log.Info(ServiceConfiguration.Name, "FL: OpnNum " + opnumber);

                    if (opnumber.Length <= 0)
                    {
                        //     this.Log.Info(ServiceConfiguration.Name, "FL: blank opnum: " + l.CenterItem.FCName + " : " + l.CenterItem.Feature.OID);

                        SelItem si = walk.ValidFeature(l.CenterItem.EID, l, network, targetWorkspace);
                        //     this.Log.Info(ServiceConfiguration.Name, "FL: blank opnum F");
                        if (!string.IsNullOrEmpty(si.FCName))
                        {
                            //    this.Log.Info(ServiceConfiguration.Name, "FL: blank opnum lbl: " + si.Label);
                            opnumber = si.Label;
                            l.CenterItem.Label = opnumber;

                            //     this.Log.Info(ServiceConfiguration.Name, "FL: Found Label: " + si.FCName + " : " + si.OID);
                        }
                    }

                    if (opnumber.Length <= 0)
                    {
                        // Operating number for openpoint is blank/null, try and use related feature.
                        KeyValuePair<string, IFeature> kvp = UseOtherLayer(l.CenterItem.Feature, "EDGIS.TRANSFORMER", targetWorkspace);
                        //     this.Log.Info(ServiceConfiguration.Name, "FL Find OPS");
                        if ((kvp.Value != null) && (!string.IsNullOrEmpty(kvp.Key)))
                        {
                            if (kvp.Key != l.CenterItem.Label)
                                l.StartItem.LoadFeature(kvp.Value, kvp.Key);
                        }

                        if ((l.StartItem != null) && (!string.IsNullOrEmpty(l.StartItem.FCName)))
                        {
                            if (l.StartItem.Label.Length > 0)
                            {
                                opnumber = l.StartItem.Label;
                                l.CenterItem.Label = opnumber;
                            }
                        }
                    }
                    if (opnumber.Length > 0)
                    {
                        this.Log.Info(ServiceConfiguration.Name, "FL Center OP: " + l.CenterItem.FCName + " : " + l.CenterItem.OID);
                        if ((l.StartItem != null) && (string.IsNullOrEmpty(l.StartItem.FCName)))
                            ConnectedFeature(l.CenterItem.Feature, l, network, ref admsLabelDeviceClassIDs, targetWorkspace);
                        //     this.Log.Info(ServiceConfiguration.Name, "FL Find OPSI");
                        if ((l.StartItem != null) && ((l.StartItem.FCName) != null) && (l.StartItem.FCName.Length > 0))
                        {
                            this.Log.Info(ServiceConfiguration.Name, "FL Start: " + l.StartItem.FCName + " : " + l.StartItem.OID);
                            LabelItems ll = walk.FFIndLabel(l, targetWorkspace);
                            if ((ll.NextItem != null) && (ll.NextItem.Feature != null))
                            {
                                //     this.Log.Info(ServiceConfiguration.Name, "FL Next: " + ll.NextItem.FCName + " : " + ll.NextItem.OID);
                                l.NextItem.LoadFeature(ll.NextItem.Feature, FieldHelper.GetFieldValue(ll.NextItem.Feature, SchemaInfo.Electric.FieldModelNames.OperatingNumber, true));
                                l.ViaList = ll.ViaList;
                                //      this.Log.Info(ServiceConfiguration.Name, "FL Upd: " + l.Label() + " : " + l.CenterItem.FCName + " : " + l.CenterItem.OID);
                                updateFeatures.Add(l.CenterItem.OID, l.Label());
                            }
                            else
                            {
                                //    this.Log.Info(ServiceConfiguration.Name, "FL Updt: " + l.Label() + " : " + l.CenterItem.FCName + " : " + l.CenterItem.OID);
                                updateFeatures.Add(l.CenterItem.OID, l.Label());
                            }
                        }
                        else
                        {
                            this.Log.Info(ServiceConfiguration.Name, "FL Missing Start: " + l.CenterItem.OID);
                        }
                    }
                    else
                        this.Log.Info(ServiceConfiguration.Name, "FL No Start found: " + l.CenterItem.OID);
                }

                if (UpdateOpenPoint(updateFeatures, circuitID, network, targetWorkspace))
                {
                    result = true;
                }
                else
                {
                    result = false;
                }

                updateFeatures.Clear();
                updateFeatures = new Dictionary<int, string>();


                return result;
            }
            catch (System.Exception ex)
            {
                this.Log.Error(ServiceConfiguration.Name, "Error during ADMS Label processing: FindLabels " + ex.Message + " Stack: " + ex.StackTrace.ToString());
                throw ex;
            }
            finally
            {
                LabelGroup.Items.Clear();
                LabelGroup.Init();
                //    GC.Collect();
            }
        }

        /// <summary>
        /// Used to navigate geomnetwork.
        /// </summary>
        internal class Walking
        {
            #region "Private Variables"
            private Miner.Geodatabase.GeodatabaseManager.Logging.INameLog _log = null;
            private string _name = string.Empty;
            #endregion

            /// <summary>
            /// Initializer.
            /// </summary>
            internal Walking() { }

            /// <summary>
            /// Initializer.
            /// </summary>
            /// <param name="log"></param>
            /// <param name="name"></param>
            internal Walking(Miner.Geodatabase.GeodatabaseManager.Logging.INameLog log, string name)
            {
                _log = log;
                _name = name;
            }

            /// <summary>
            /// Find parts of label.
            /// </summary>
            /// <param name="labelItems"></param>
            /// <param name="workspace"></param>
            /// <returns></returns>
            internal PGE_OpenPointADMSLabel.LabelItems FFIndLabel(PGE_OpenPointADMSLabel.LabelItems labelItems, IWorkspace workspace)
            {
                PGE_OpenPointADMSLabel.LabelItems result = labelItems;

                List<int> usedOIDS = new List<int>();
                List<int> usedEIDS = new List<int>();
                List<int> usedEdges = new List<int>();
                _log.Info(_name, "W FL Center: " + labelItems.CenterItem.FCName + " : " + labelItems.CenterItem.OID);
                usedOIDS.Add(labelItems.CenterItem.OID);
                if (labelItems.StartItem.FCName.Length > 0)
                {
                    usedOIDS.Add(labelItems.StartItem.OID);
                    _log.Info(_name, "W FL Start: " + labelItems.StartItem.FCName + " : " + labelItems.StartItem.OID);
                }

                IFeature feature = GetEndingJunction(labelItems.CenterItem.Feature, labelItems.CenterItem.Label, ref usedOIDS, ref usedEIDS, ref usedEdges);
                if (feature != null)
                {
                    string opNumber = FieldHelper.GetFieldValue(feature, SchemaInfo.Electric.FieldModelNames.OperatingNumber, true);
                    if (string.IsNullOrEmpty(opNumber))
                        opNumber = "UNKN";
                    //  _log.Info(_name, "W FL OpNum: " + opNumber);

                    result.NextItem.LoadFeature(feature, opNumber);

                    usedEIDS = usedEIDS.Distinct().ToList();
                    //   _log.Info(_name, "W FL Cnt: " + usedEIDS.Count() + " : " + usedEdges.Count());
                    if (usedEIDS.Count() > 0)
                    {
                        PGE_OpenPointADMSLabel.LabelItems li = GetViaLabel(result, usedEIDS, usedEdges, workspace);
                        if (li.ViaList.Count() > 0)
                        {
                            result.ViaList = li.ViaList;
                        }
                    }
                }
                return result;
            }


            /// <summary>
            /// Finds next part in label.
            /// </summary>
            /// <param name="sourceFeature"></param>
            /// <param name="usedOIDS"></param>
            /// <param name="usedEIDS"></param>
            /// <param name="usedEdges"></param>
            /// <returns></returns>
            private IFeature GetEndingJunction(IFeature sourceFeature, string alabel, ref List<int> usedOIDS, ref List<int> usedEIDS, ref List<int> usedEdges)
            {
                IFeature result = null;
                try
                {
                    //  _log.Info(_name, "W GEJ : " + sourceFeature.Class.AliasName + " : " + sourceFeature.OID);

                    if ((sourceFeature != null) && (sourceFeature is INetworkFeature) && (sourceFeature.FeatureType == esriFeatureType.esriFTSimpleJunction))
                    {
                        ISimpleJunctionFeature junctionFeature = sourceFeature as ISimpleJunctionFeature;
                        IEdgeFeature edgeFeature = null;

                        for (int i = 0; i < junctionFeature.EdgeFeatureCount; i++)
                        {
                            usedEIDS.Add(junctionFeature.EID);
                            edgeFeature = junctionFeature.EdgeFeature[i];
                            if ((edgeFeature != null) && (edgeFeature is ISimpleEdgeFeature))
                            {
                                ISimpleEdgeFeature temp = edgeFeature as ISimpleEdgeFeature;
                                //  IFeature tempx = edgeFeature as IFeature;
                                //  _log.Info(_name, "W GEJ SE: " + tempx.Class.AliasName + " : " + tempx.OID);
                                usedEdges.Add(temp.EID);
                            }
                            else if ((edgeFeature != null) && (edgeFeature is IComplexEdgeFeature))
                            {
                                IFeature temp = edgeFeature as IFeature;
                                // _log.Info(_name, "W GEJ CE: " + temp.Class.AliasName + " : " + temp.OID);
                                usedEdges.Add(GetFeatureEID(temp));
                            }
                            IFeature fromfeature = edgeFeature.FromJunctionFeature as IFeature;
                            IFeature tofeature = edgeFeature.ToJunctionFeature as IFeature;
                            _log.Info(_name, "W GEJ To:From: " + tofeature.Class.AliasName + " : " + tofeature.OID + " : " +
                                fromfeature.Class.AliasName + " : " + fromfeature.OID);

                            List<IFeature> featlist = new List<IFeature>();
                            if ((fromfeature != null) && (!usedOIDS.Contains(fromfeature.OID)))
                            {
                                usedEIDS.Add(edgeFeature.FromJunctionEID);
                                //     _log.Info(_name, "W GEJ Used Jun F: " + fromfeature.Class.AliasName + " : " + fromfeature.OID);
                                featlist.Add(fromfeature);
                            }
                            if ((tofeature != null) && (!usedOIDS.Contains(tofeature.OID)))
                            {
                                usedEIDS.Add(edgeFeature.ToJunctionEID);
                                //    _log.Info(_name, "W GEJ Used Jun T: " + tofeature.Class.AliasName + " : " + tofeature.OID);
                                featlist.Add(tofeature);
                            }

                            foreach (IFeature feature in featlist)
                            {
                                if (feature != null)
                                {
                                    if (!usedOIDS.Contains(feature.OID))
                                    {
                                        usedOIDS.Add(feature.OID);
                                        string opnum = string.Empty;
                                        if ((ModelNameFacade.ContainsClassModelName(feature.Class, SchemaInfo.Electric.FieldModelNames.OperatingNumber)))
                                        {
                                            opnum = FieldHelper.GetFieldValue(feature, SchemaInfo.Electric.FieldModelNames.OperatingNumber, true);
                                        }

                                        if ((!string.IsNullOrEmpty(opnum)) && (alabel != opnum))
                                            return feature;
                                        else
                                        {
                                            // Keep walking..

                                            return GetEndingJunction(feature, alabel, ref usedOIDS, ref usedEIDS, ref usedEdges);
                                        }
                                    }
                                }
                            }
                        }

                    }
                    else if ((sourceFeature.FeatureType != esriFeatureType.esriFTSimpleJunction))
                    {
                        _log.Info(_name, "W GEJ CompPoint: " + sourceFeature.Class.AliasName + " : " + sourceFeature.OID);
                    }
                    return result;
                }
                catch (System.Exception ex)
                {
                    _log.Info(_name, "W GEJ ERROR: " + ex.Message + " : " + ex.StackTrace.ToString());
                    return null;
                }
            }

            /// <summary>
            /// Get EID from Feature.
            /// </summary>
            /// <param name="feature"></param>
            /// <returns></returns>
            private int GetFeatureEID(IFeature feature)
            {
                IFeatureClass fc = feature.Class as IFeatureClass;
                INetElements netElements = (INetElements)(feature as INetworkFeature).GeometricNetwork.Network;

                IEnumNetEID eids = netElements.GetEIDs(fc.FeatureClassID, feature.OID, esriElementType.esriETEdge);

                if (eids.Count == 0) return 0;

                return eids.Next();
            }

            /// <summary>
            /// Get label values from branches.
            /// </summary>
            /// <param name="li"></param>
            /// <param name="chkEIDs"></param>
            /// <param name="chkEdges"></param>
            /// <param name="workspace"></param>
            /// <returns></returns>
            private PGE_OpenPointADMSLabel.LabelItems GetViaLabel(PGE_OpenPointADMSLabel.LabelItems li, List<int> chkEIDs, List<int> chkEdges, IWorkspace workspace)
            {
                PGE_OpenPointADMSLabel.SelItem si = new PGE_OpenPointADMSLabel.SelItem();
                PGE_OpenPointADMSLabel.LabelItems result = new PGE_OpenPointADMSLabel.LabelItems();
                IForwardStarGEN forwardStar = null;

                //    _log.Info(_name, "W Via: " + chkEIDs.Count() + " : " + chkEdges.Count());

                try
                {
                    IGeometricNetwork geometricNetwork = LabelHelpers.GetNetworks(workspace);

                    List<int> junctionEIDs = new List<int>();
                    List<int> edgeEIDS = new List<int>();

                    INetWeight netWeight = GetNetworkWeight(geometricNetwork);

                    forwardStar = geometricNetwork.Network.CreateForwardStar(false, null, null, null, null) as IForwardStarGEN;
                    int adjacentEdgesCount;
                    int maxcnt = 2;

                    if (li.Is50Scale)
                        maxcnt = 1;
                    // Just for debuging
                    //for (int i = 0; i < chkEdges.Count; i++)
                    //{
                    //    IFeature feature = LabelHelpers.GetFeaturefromEID(chkEdges[i], esriElementType.esriETEdge, geometricNetwork);
                    //    _log.Info(_name, "W Via Edge Look: " + feature.Class.AliasName + " : " + feature.OID);
                    //}

                    //for (int i = 0; i < chkEIDs.Count; i++)
                    //{
                    //    IFeature feature = LabelHelpers.GetFeaturefromEID(chkEIDs[i], esriElementType.esriETJunction, geometricNetwork);
                    //    _log.Info(_name, "W Via Junc Look: " + feature.Class.AliasName + " : " + feature.OID);
                    //}

                    int[] adjacentEdgeEIDS;
                    bool[] reverseOrientationEdge;
                    object[] weightValuesJunctions;
                    for (int i = 0; i < chkEIDs.Count; i++)
                    {
                        //IFeature feature = LabelHelpers.GetFeaturefromEID(chkEIDs[i], esriElementType.esriETJunction, geometricNetwork);
                        //_log.Info(_name, "W Via L F: " + feature.Class.AliasName + " : " + feature.OID);

                        adjacentEdgesCount = -1;
                        forwardStar.FindAdjacent(0, chkEIDs[i], out adjacentEdgesCount);

                        adjacentEdgeEIDS = new int[adjacentEdgesCount];
                        reverseOrientationEdge = new bool[adjacentEdgesCount];
                        weightValuesJunctions = new object[adjacentEdgesCount];

                        forwardStar.QueryAdjacentEdges(ref adjacentEdgeEIDS, ref reverseOrientationEdge, ref weightValuesJunctions);

                        // We only want branches that are primary conductors.
                        adjacentEdgeEIDS = OnlyPrimaries(adjacentEdgeEIDS, reverseOrientationEdge, geometricNetwork);
                        //  _log.Info(_name, "W Via adj: " + adjacentEdgeEIDS.Length);
                        // If only two edges this won't be a via.
                        if (adjacentEdgeEIDS.Length > 2)
                        {
                            //    _log.Info(_name, "W Via IL");
                            List<PGE_OpenPointADMSLabel.SelItem> selList = new List<PGE_OpenPointADMSLabel.SelItem>();
                            for (int e = 0; e < adjacentEdgeEIDS.Length; e++)
                            {
                                if (!chkEdges.Contains(adjacentEdgeEIDS[e]))
                                {
                                    //feature = LabelHelpers.GetFeaturefromEID(adjacentEdgeEIDS[e], esriElementType.esriETEdge, geometricNetwork);
                                    //_log.Info(_name, "W Via IL F: " + feature.Class.AliasName + " : " + feature.OID);

                                    // An edge we didn't use in the intial flow.
                                    //       _log.Info(_name, "W Via sel travs: " + selList.Count());
                                    // junctionEIDs = new List<int>();
                                    int count = TraversAnEdge2(workspace, geometricNetwork, adjacentEdgeEIDS[e], li, ref selList, ref junctionEIDs);
                                    //if (selList.Count() >= 0)
                                    //{
                                    //    _log.Info(_name, "W Via reloop." + count);
                                    ////    return GetViaLabel(li, new List<int>(count), chkEdges, workspace);
                                    //}
                                    bool badd = true;
                                    //       _log.Info(_name, "W Via sel: " + selList.Count());
                                    foreach (PGE_OpenPointADMSLabel.SelItem s in selList)
                                    {
                                        //           _log.Info(_name, "W Via sel loop: " + s.FCName + " : " + s.OID + "  cnt: " + li.ViaList.Count() + " : " + maxcnt);
                                        if (li.ViaList.Count() == maxcnt)
                                        {
                                            //     _log.Info(_name, "W Via sel loop Max Count.");
                                            //    break;
                                        }

                                        // Check if value is already in list.
                                        badd = true;
                                        foreach (PGE_OpenPointADMSLabel.SelItem v in li.ViaList)
                                        {
                                            if (v.Label == s.Label)
                                            {
                                                badd = false;
                                                //         _log.Info(_name, "W Via In Via already: " + s.Label);
                                                break;
                                            }
                                        }

                                        // Just insuring we don't add the beginning ones again.
                                        if ((s.Label == li.NextItem.Label) || (s.Label == li.StartItem.Label))
                                        {
                                            badd = false;
                                            //       _log.Info(_name, "W Via Matches start or next: " + s.Label);
                                        }

                                        if (badd)
                                        {
                                            li.ViaList.Add(s);
                                            //     _log.Info(_name, "W Via added: " + s.FCName + " : " + s.OID + " : " + s.Label);
                                        }
                                    }

                                    // Only want # via/and features. 
                                    if (li.ViaList.Count() == maxcnt)
                                    {
                                        //    _log.Info(_name, "W Via list count break: " + selList.Count());
                                        //   break;
                                    }
                                }
                                else
                                {
                                    //     _log.Info(_name, "W Via sel contains: " + selList.Count());
                                }
                            }

                        }
                        //if ((li != null) && (li..FCName != null))
                        //    break;
                    }

                    adjacentEdgeEIDS = null;
                    reverseOrientationEdge = null;
                    weightValuesJunctions = null;
                    junctionEIDs.Clear();
                    edgeEIDS.Clear();
                    result = li;

                    return result;
                }
                catch (System.Exception ex)
                {
                    return result;
                }
                finally
                {
                    if (forwardStar != null) { while (Marshal.ReleaseComObject(forwardStar) > 0) ; }
                    //   GC.Collect();
                }
            }


            /// <summary>
            /// Get extra parts of label.
            /// </summary>
            /// <param name="li"></param>
            /// <param name="chkEIDs"></param>
            /// <param name="chkEdges"></param>
            /// <param name="workspace"></param>
            /// <returns></returns>
            private PGE_OpenPointADMSLabel.LabelItems GetViaLabel2(PGE_OpenPointADMSLabel.LabelItems li, List<int> chkEIDs, List<int> chkEdges, IWorkspace workspace)
            {
                PGE_OpenPointADMSLabel.SelItem si = new PGE_OpenPointADMSLabel.SelItem();
                PGE_OpenPointADMSLabel.LabelItems result = new PGE_OpenPointADMSLabel.LabelItems();
                IForwardStarGEN forwardStar = null;

                _log.Info(_name, "W Via: " + chkEIDs.Count() + " : " + chkEdges.Count());

                try
                {
                    IGeometricNetwork geometricNetwork = LabelHelpers.GetNetworks(workspace);

                    List<int> junctionEIDs = new List<int>();
                    List<int> edgeEIDS = new List<int>();

                    INetWeight netWeight = GetNetworkWeight(geometricNetwork);

                    forwardStar = geometricNetwork.Network.CreateForwardStar(false, null, null, null, null) as IForwardStarGEN;
                    int adjacentEdgesCount;
                    int maxcnt = 2;

                    if (li.Is50Scale)
                        maxcnt = 1;
                    // Just for debuging
                    for (int i = 0; i < chkEdges.Count; i++)
                    {
                        IFeature feature = LabelHelpers.GetFeaturefromEID(chkEdges[i], esriElementType.esriETEdge, geometricNetwork);
                        _log.Info(_name, "W Via Edge Look: " + feature.Class.AliasName + " : " + feature.OID);
                    }

                    for (int i = 0; i < chkEIDs.Count; i++)
                    {
                        IFeature feature = LabelHelpers.GetFeaturefromEID(chkEIDs[i], esriElementType.esriETJunction, geometricNetwork);
                        _log.Info(_name, "W Via Junc Look: " + feature.Class.AliasName + " : " + feature.OID);
                    }

                    int[] adjacentEdgeEIDS;
                    bool[] reverseOrientationEdge;
                    object[] weightValuesJunctions;
                    for (int i = 0; i < chkEIDs.Count; i++)
                    {
                        IFeature feature = LabelHelpers.GetFeaturefromEID(chkEIDs[i], esriElementType.esriETJunction, geometricNetwork);
                        _log.Info(_name, "W Via L F: " + feature.Class.AliasName + " : " + feature.OID);

                        adjacentEdgesCount = -1;
                        forwardStar.FindAdjacent(0, chkEIDs[i], out adjacentEdgesCount);

                        adjacentEdgeEIDS = new int[adjacentEdgesCount];
                        reverseOrientationEdge = new bool[adjacentEdgesCount];
                        weightValuesJunctions = new object[adjacentEdgesCount];

                        forwardStar.QueryAdjacentEdges(ref adjacentEdgeEIDS, ref reverseOrientationEdge, ref weightValuesJunctions);

                        // We only want branches that are primary conductors.
                        adjacentEdgeEIDS = OnlyPrimaries(adjacentEdgeEIDS, reverseOrientationEdge, geometricNetwork);
                        _log.Info(_name, "W Via adj: " + adjacentEdgeEIDS.Length);
                        // If only two edges this won't be a via.
                        if (adjacentEdgeEIDS.Length >= 2)
                        {
                            List<PGE_OpenPointADMSLabel.SelItem> selList = new List<PGE_OpenPointADMSLabel.SelItem>();
                            for (int e = 0; e < adjacentEdgeEIDS.Length; e++)
                            {
                                feature = LabelHelpers.GetFeaturefromEID(adjacentEdgeEIDS[e], esriElementType.esriETEdge, geometricNetwork);
                                _log.Info(_name, "W Via IL F: " + feature.Class.AliasName + " : " + feature.OID);

                                _log.Info(_name, "W Via Edge Loop: " + adjacentEdgeEIDS[e]);
                                if (!chkEdges.Contains(adjacentEdgeEIDS[e]))
                                {
                                    // An edge we didn't use in the intial flow.
                                    _log.Info(_name, "W Via sel travs: " + selList.Count());
                                    // junctionEIDs = new List<int>();
                                    int count = TraversAnEdge2(workspace, geometricNetwork, adjacentEdgeEIDS[e], li, ref selList, ref junctionEIDs);
                                    if (count > 0)
                                    {
                                        _log.Info(_name, "W Via reloop." + count);
                                        return GetViaLabel(li, new List<int>(count), chkEdges, workspace);
                                    }
                                    bool badd = true;
                                    _log.Info(_name, "W Via sel: " + selList.Count());
                                    foreach (PGE_OpenPointADMSLabel.SelItem s in selList)
                                    {
                                        _log.Info(_name, "W Via sel loop: " + s.FCName + " : " + s.OID + "  cnt: " + li.ViaList.Count() + " : " + maxcnt);
                                        if (li.ViaList.Count() == maxcnt)
                                        {
                                            _log.Info(_name, "W Via sel loop Max Count.");
                                            //    break;
                                        }

                                        // Check if value is already in list.
                                        badd = true;
                                        foreach (PGE_OpenPointADMSLabel.SelItem v in li.ViaList)
                                        {
                                            if (v.Label == s.Label)
                                            {
                                                badd = false;
                                                _log.Info(_name, "W Via In Via already: " + s.Label);
                                                break;
                                            }
                                        }

                                        // Just insuring we don't add the beginning ones again.
                                        if ((s.Label == li.NextItem.Label) || (s.Label == li.StartItem.Label))
                                        {
                                            badd = false;
                                            _log.Info(_name, "W Via Matches start or next: " + s.Label);
                                        }

                                        if (badd)
                                        {
                                            li.ViaList.Add(s);
                                            _log.Info(_name, "W Via added: " + s.FCName + " : " + s.OID + " : " + s.Label);
                                        }
                                    }

                                    // Only want # via/and features. 
                                    if (li.ViaList.Count() == maxcnt)
                                    {
                                        _log.Info(_name, "W Via list count break: " + selList.Count());
                                        //   break;
                                    }
                                }
                                else
                                {
                                    _log.Info(_name, "W Via sel contains: " + selList.Count());
                                }
                            }

                        }
                        //if ((li != null) && (li..FCName != null))
                        //    break;
                    }

                    adjacentEdgeEIDS = null;
                    reverseOrientationEdge = null;
                    weightValuesJunctions = null;
                    junctionEIDs.Clear();
                    edgeEIDS.Clear();
                    result = li;

                    return result;
                }
                catch (System.Exception ex)
                {
                    return result;
                }
                finally
                {
                    if (forwardStar != null) { while (Marshal.ReleaseComObject(forwardStar) > 0) ; }
                    //   GC.Collect();
                }
            }

            /// <summary>
            /// Remove all line features/edges that are NOT PriOHConductors or PriUGConductors.
            /// </summary>
            /// <param name="adjacentEdgeEIDS"></param>
            /// <param name="reverseOrientationEdge"></param>
            /// <param name="geomNetwork"></param>
            /// <returns>List of primary conductors.</returns>
            private int[] OnlyPrimaries(int[] adjacentEdgeEIDS, bool[] reverseOrientationEdge, IGeometricNetwork geomNetwork)
            {
                List<int> result = new List<int>();
                IFeature feature;
                bool reverse = false;
                string[] modelNames = { SchemaInfo.Electric.ClassModelNames.PGEPriOHConductor, SchemaInfo.Electric.ClassModelNames.PGEPriUGConductor, SchemaInfo.Electric.ClassModelNames.PGEBusBar };

                for (int i = 0; i < adjacentEdgeEIDS.Length; i++)
                {
                    reverse = reverseOrientationEdge[i];
                    feature = LabelHelpers.GetFeaturefromEID(adjacentEdgeEIDS[i], esriElementType.esriETEdge, geomNetwork);
                    if (feature != null)
                    {
                        if (ModelNameFacade.ContainsClassModelName(feature.Class, modelNames))
                        {
                            //  If (reverse).
                            result.Add(adjacentEdgeEIDS[i]);
                        }
                    }
                    else { }
                }

                return result.ToArray();
            }

            /// <summary>
            /// Returns MM network weight value.
            /// </summary>
            /// <param name="geomNetwork"></param>
            /// <returns></returns>
            private INetWeight GetNetworkWeight(IGeometricNetwork geomNetwork)
            {
                INetSchema networkSchema = geomNetwork.Network as INetSchema;
                INetWeight electricTraceWeight = networkSchema.get_WeightByName("MMELECTRICTRACEWEIGHT");
                return electricTraceWeight;
            }

            /// <summary>
            /// Selected edges that where not in base tracing/paths for label.
            ///   -- looking for VIA/& features.
            /// </summary>
            /// <param name="geomNetwork"></param>
            /// <param name="eid"></param>
            /// <param name="li"></param>
            /// <param name="sellist"></param>
            /// <param name="junctionEIDs"></param>
            /// <returns></returns>
            private int TraversAnEdge2(IWorkspace workspace, IGeometricNetwork geomNetwork, int eid, PGE_OpenPointADMSLabel.LabelItems li, ref List<PGE_OpenPointADMSLabel.SelItem> sellist, ref List<int> junctionEIDs)
            {
                int result = 0;
                List<PGE_OpenPointADMSLabel.SelItem> list = new List<PGE_OpenPointADMSLabel.SelItem>();
                try
                {
                    PGE_OpenPointADMSLabel.SelItem si = new PGE_OpenPointADMSLabel.SelItem();
                    IFeature feat = LabelHelpers.GetFeaturefromEID(eid, esriElementType.esriETEdge, geomNetwork);
                    //         _log.Info(_name, "W TE look: " + feat.Class.AliasName + " : " + feat.OID);

                    if (feat == null)
                    {
                        //      _log.Info(_name, "W TE Null");
                        return result;
                    }

                    junctionEIDs = junctionEIDs.Distinct().ToList();

                    //for (int i = 0; i < junctionEIDs.Count(); i++)
                    //{
                    //    IFeature xfeat = LabelHelpers.GetFeaturefromEID(junctionEIDs[i], esriElementType.esriETJunction, geomNetwork);
                    //    _log.Info(_name, "W TE junk look: " + xfeat.Class.AliasName + " : " + xfeat.OID);
                    //}

                    IEdgeFeature edgeFeat = feat as IEdgeFeature;
                    List<int> nextedges = new List<int>();

                    // We want to stop after we have 2 valid features.
                    if (sellist.Count() == 2)
                    {
                        //     _log.Info(_name, "W TE Sel Count: " + sellist.Count());
                        return result;
                    }

                    //  IFeature tempFeat = null;

                    if (!junctionEIDs.Contains(edgeFeat.FromJunctionEID) && (li.StartItem.EID != edgeFeat.FromJunctionEID) && (li.NextItem.EID != edgeFeat.FromJunctionEID)) // && chkEIDs.Contains(edgeFeat.FromJunctionEID))
                    {
                        //tempFeat = LabelHelpers.GetFeaturefromEID(edgeFeat.FromJunctionEID, esriElementType.esriETJunction, geomNetwork);
                        //_log.Info(_name, "W TE JAddF: " + tempFeat.Class.AliasName + " : " + tempFeat.OID);
                        junctionEIDs.Add(edgeFeat.FromJunctionEID);
                        result = edgeFeat.FromJunctionEID;

                        si = ValidFeature(edgeFeat.FromJunctionEID, li, geomNetwork, workspace);
                        if (!string.IsNullOrEmpty(si.FCName))
                        {
                            //   _log.Info(_name, "W TE AddF: " + si.FCName + " : " + si.OID);
                            sellist.Add(si);
                        }
                        else
                        {
                            //  _log.Info(_name, "W TE NextEdge");
                            nextedges = GetNextEdge(edgeFeat.FromJunctionFeature, eid);
                            //  _log.Info(_name, "W TE NextEdge cnt: " + nextedges.Count());
                            // Way to keep traversing edges till we find a valid junction on this path.
                            foreach (int e in nextedges)
                            {
                                //IFeature xxFeat = LabelHelpers.GetFeaturefromEID(e, esriElementType.esriETEdge, geomNetwork);
                                //_log.Info(_name, "W TE F GEN " + xxFeat.Class.AliasName + " : " + xxFeat.OID);
                                result = TraversAnEdge2(workspace, geomNetwork, e, li, ref sellist, ref junctionEIDs);
                            }
                        }
                    }
                    else
                    {
                        //  _log.Info(_name, "W TE F InList or Start");
                    }
                    if (!junctionEIDs.Contains(edgeFeat.ToJunctionEID) && (li.StartItem.EID != edgeFeat.ToJunctionEID) && (li.NextItem.EID != edgeFeat.ToJunctionEID))
                    {
                        //tempFeat = LabelHelpers.GetFeaturefromEID(edgeFeat.ToJunctionEID, esriElementType.esriETJunction, geomNetwork);
                        //_log.Info(_name, "W TE JAddT: " + tempFeat.Class.AliasName + " : " + tempFeat.OID);

                        junctionEIDs.Add(edgeFeat.ToJunctionEID);
                        result = edgeFeat.ToJunctionEID;

                        // si = new PGE_OpenPointADMSLabel.SelItem();
                        //      this.Log.Info(ServiceConfiguration.Name, "Travers edge To " + edgeFeat.ToJunctionEID);
                        si = ValidFeature(edgeFeat.ToJunctionEID, li, geomNetwork, workspace);
                        if (!string.IsNullOrEmpty(si.FCName))
                        {
                            //   _log.Info(_name, "W TE AddT: " + si.FCName + " : " + si.OID + " : " + si.Label);
                            sellist.Add(si);
                        }
                        else
                        {
                            //   _log.Info(_name, "W TE NextEdgeT");
                            nextedges = GetNextEdge(edgeFeat.ToJunctionFeature, eid);
                            // Way to keep traversing edges till we find a valid junction on this path.
                            foreach (int e in nextedges)
                            {
                                //IFeature xxFeat = LabelHelpers.GetFeaturefromEID(e, esriElementType.esriETEdge, geomNetwork);
                                //_log.Info(_name, "W TE GENT " + xxFeat.Class.AliasName + " : " + xxFeat.OID);
                                result = TraversAnEdge2(workspace, geomNetwork, e, li, ref sellist, ref junctionEIDs);
                            }
                        }
                    }
                    else
                    {
                        //   _log.Info(_name, "W TE T InList or Start");
                    }

                    //  _log.Info(_name, "W TE Return: " + result);
                    return result;
                }
                catch (System.Exception ex)
                {
                    throw ex;
                }

            }


            /// <summary>
            /// Grabs opposing edge. 
            /// </summary>
            /// <param name="junction">Point edge is connected to.</param>
            /// <param name="edgeEID">EID of edge we don't want returned.</param>
            /// <returns>List of Edge(s) EID.</returns>
            private List<int> GetNextEdge(IJunctionFeature junction, int edgeEID)
            {
                List<int> result = new List<int>();
                ISimpleJunctionFeature jOne = null;
                ISimpleEdgeFeature eOne = null;

                IFeature feat = null; // junction as IFeature;

                //     _log.Info(_name, "W GetNE: " + edgeEID); // + " : " + feat.Class.AliasName + " : " + feat.OID);

                try
                {
                    if ((junction != null) && (junction is INetworkFeature))
                    {
                        //     _log.Info(_name, "W GetNE L: " + edgeEID); // + " : " + feat.Class.AliasName + " : " + feat.OID);

                        jOne = junction as ISimpleJunctionFeature;
                        for (int i = 0; i < jOne.EdgeFeatureCount; i++)
                        {
                            eOne = jOne.EdgeFeature[i] as ISimpleEdgeFeature;
                            //feat = eOne as IFeature;
                            //_log.Info(_name, "W GetNE LF: " + eOne.EID + " : " + feat.Class.AliasName + " : " + feat.OID);

                            if (eOne.EID != edgeEID)
                            {
                                result.Add(eOne.EID);
                            }
                        }
                    }

                    return result;
                }
                catch (System.Exception ex)
                {
                    throw ex;
                }
            }

            /// <summary>
            /// Is this a valid feature for a label part.
            /// </summary>
            /// <param name="eid">Junction EID we are testing for validity.</param>
            /// <param name="center"></param>
            /// <param name="geomNetwork"></param>
            /// <returns></returns>
            internal PGE_OpenPointADMSLabel.SelItem ValidFeature(int eid, PGE_OpenPointADMSLabel.LabelItems center, IGeometricNetwork geomNetwork, IWorkspace workspace)
            {
                //    _log.Info(_name, "W Valid eid." + eid);

                PGE_OpenPointADMSLabel.SelItem result = new PGE_OpenPointADMSLabel.SelItem();
                IFeature input = LabelHelpers.GetFeaturefromEID(eid, esriElementType.esriETJunction, geomNetwork);
                if (input != null)
                {
                    //     _log.Info(_name, "W Valid feat." + input.Class.AliasName + " : " + input.OID);
                    string lbl = string.Empty;

                    if (ModelNameFacade.ContainsClassModelName(input.Class, PGE_OpenPointADMSLabel._modelNames))
                    {

                        if (ModelNameFacade.ContainsFieldModelName(input.Class, SchemaInfo.Electric.ClassModelNames.OperatingNumber))
                        {
                            // If openpoint check for Transformer relationgship.
                            if (ModelNameFacade.ContainsClassModelName(input.Class, SchemaInfo.Electric.ClassModelNames.PGEOpenPoint))
                            {
                                //    _log.Info(_name, "W Valid is openpoint ." + input.OID);
                                IFeature trans = CheckRelationship(input, SchemaInfo.Electric.ClassModelNames.PGETransformer, workspace);
                                if (trans != null)
                                {
                                    input = trans;
                                    //         _log.Info(_name, "W Valid Picked Transformer." + trans.OID);
                                }
                            }

                            lbl = FieldHelper.GetFieldValue(input, SchemaInfo.Electric.FieldModelNames.OperatingNumber, true);
                            if (!string.IsNullOrEmpty(lbl) && !center.HasValue(lbl))
                            {
                                result.LoadFeature(input, lbl);
                                //        _log.Info(_name, "W Valid load: " + input.Class.AliasName + " : " + input.OID + " : " + lbl);
                            }
                            else
                            {
                                _log.Info(_name, "W INValid load: " + input.Class.AliasName + " : " + input.OID + " : " + lbl);
                            }
                        }
                    }

                }
                return result;
            }

            /// <summary>
            /// Check for another feature based on structureGUID and GlobalID.
            /// </summary>
            /// <param name="input"></param>
            /// <param name="layerName"></param>
            /// <param name="workspace"></param>
            /// <returns></returns>
            private IFeature CheckRelationship(IFeature input, string modelLayerName, IWorkspace workspace)
            {
                //   _log.Info(_name, "W Check.");
                IFeature result = null;
                IFeatureClass featureClass = ModelNameFacade.FeatureClassByModelName(workspace, modelLayerName);
                string guid = FieldHelper.GetFieldValue(input, _structureGUID, false);
                if (guid.Length > 0)
                {
                    //    _log.Info(_name, "W Check Guid: " + guid);

                    IFeature feature = LabelHelpers.GetFeature(featureClass, _structureGUID, guid);
                    if (feature != null)
                    {
                        //      _log.Info(_name, "W Check f: " + feature.OID);
                        string lbl = FieldHelper.GetFieldValue(feature, SchemaInfo.Electric.FieldModelNames.OperatingNumber, true);
                        if (lbl.Length > 0)
                        {
                            result = feature;
                        }
                    }
                }
                return result;
            }
        }
    }

    /// <summary>
    /// Collection of repeated functions.
    /// </summary>
    internal static class LabelHelpers
    {
        #region Class variables

        private static readonly string _pge50ScaleLayer = "EDGIS.PGE_50ScaleBoundary";

        #endregion


        /// <summary>
        /// Gets a feature.
        /// </summary>
        /// <param name="featureClass"></param>
        /// <param name="fieldName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        internal static IFeature GetFeature(IFeatureClass featureClass, string fieldName, string value)
        {
            IFeature result = null;
            IQueryFilter queryFilter = new QueryFilterClass
            {
                WhereClause = fieldName + " = '" + value + "'"
            };

            IFeatureCursor cursor = featureClass.Search(queryFilter, false);
            IFeature feature = null;
            while ((feature = cursor.NextFeature()) != null)
            {
                result = feature;
            }
            if (cursor != null) { while (Marshal.ReleaseComObject(cursor) > 0) { } }
            if (queryFilter != null) { while (Marshal.ReleaseComObject(queryFilter) > 0) { } }

            return result;
        }

        /// <summary>
        /// Gets a feature.
        /// </summary>
        /// <param name="featureClass"></param>
        /// <param name="oid"></param>
        /// <returns></returns>
        internal static IFeature GetFeature(IFeatureClass featureClass, int oid)
        {
            IFeature result = null;
            try
            {
                result = featureClass.GetFeature(oid);
                return result;
            }
            catch (System.Exception ex)
            {
                // Just return null for now, likely a deleted feature.
                //Log.Error(ServiceConfiguration.Name, "Error: LabelHelpers.GetFeature " + featureClass.AliasName + " Objectid: " + oid + " message: " + ex.Message + " stack: " + ex.StackTrace.ToString());
                return null;
            }
        }

        /// <summary>
        /// Gets a featureclass.
        /// </summary>
        /// <param name="featureClassName"></param>
        /// <param name="workspace"></param>
        /// <returns></returns>
        internal static IFeatureClass GetFeatureClass(string featureClassName, IWorkspace workspace)
        {
            IFeatureWorkspace featWorkspace = workspace as IFeatureWorkspace;
            IFeatureClass featClass = featWorkspace.OpenFeatureClass(featureClassName);
            if (featClass == null) { throw new Exception("Unable to find feature class " + featureClassName); }
            return featClass;
        }

        /// <summary>
        /// Given a feature gets the EID of the feature if it participates in a network.
        /// </summary>
        /// <param name="sourceFeature">The feature of type IFeature to determine the EID.</param>
        /// <returns>An Integer. the EID of the Network feature passed in. If the feature passed does not participate in geometric network will return 0.</returns>
        internal static int GetFeatureEID(IFeature sourceFeature)
        {
            try
            {
                int result = 0;
                if (sourceFeature is ISimpleEdgeFeature)
                {
                    result = (sourceFeature as ISimpleEdgeFeature).EID;
                }
                else if (sourceFeature is ISimpleJunctionFeature)
                {
                    ISimpleJunctionFeature junctionFeature = (ISimpleJunctionFeature)sourceFeature;
                    if (junctionFeature != null)
                        result = junctionFeature.EID;
                }
                // If it is Complex Edge return the EID of the First Edge feature.
                else if (sourceFeature is IComplexEdgeFeature)
                {
                    IFeatureClass featureClass = sourceFeature.Class as IFeatureClass;
                    INetElements netElements = (INetElements)(sourceFeature as INetworkFeature).GeometricNetwork.Network;

                    IEnumNetEID eids = netElements.GetEIDs(featureClass.FeatureClassID, sourceFeature.OID, esriElementType.esriETEdge);
                    if (eids.Count > 0)
                        result = eids.Next();
                }
                return result;
            }
            catch (System.Exception ex)
            {
                //  Log.Error(ServiceConfiguration.Name, "Error: LabelHelpers.GetFeatureEID " + sourceFeature.Class.AliasName + " ObjectID: " + sourceFeature.OID + " message: " + ex.Message + " stack: " + ex.StackTrace.ToString());
                return 0;
            }
        }


        /// <summary>
        /// Builds a network flag.
        /// </summary>
        /// <param name="sourceFeature">Feature flag is based on.</param>
        /// <param name="elemType">Type of flag to generate.</param>
        /// <returns>Network/trace flag.</returns>
        internal static INetFlag GetFeatureNetFlag(IFeature sourceFeature, esriElementType elemType)
        {
            INetFlag result = null;

            IJunctionFlag junflag = new JunctionFlagClass() as IJunctionFlag;
            IEdgeFlag edgFlag = new EdgeFlagClass() as IEdgeFlag;
            INetElements netElements = (INetElements)(sourceFeature as INetworkFeature).GeometricNetwork.Network;

            int userClassID = 0;
            int userID = 0;
            int userSubID = 0;

            netElements.QueryIDs(GetFeatureEID(sourceFeature), elemType, out userClassID, out userID, out userSubID);

            if (elemType == esriElementType.esriETEdge)
            {
                edgFlag.TwoWay = false;
                edgFlag.Position = .05F;

                result = edgFlag as INetFlag;
            }
            else
                result = junflag as INetFlag;

            result.UserClassID = userClassID;
            result.UserID = userID;
            result.UserSubID = userSubID;



            return result;
        }

        /// <summary>
        /// Get feature from it's EID.
        /// </summary>
        /// <param name="EID"></param>
        /// <param name="elementType"></param>
        /// <param name="geomNetwork"></param>
        /// <returns></returns>
        internal static IFeature GetFeaturefromEID(int EID, esriElementType elementType, IGeometricNetwork geomNetwork)
        {

            // Objectclass ID.
            int userClassID = 0;
            // ObjectID.
            int userID = 0;
            int userSubID = 0;
            try
            {
                INetElements netElements = geomNetwork.Network as INetElements;
                netElements.QueryIDs(EID, elementType, out userClassID, out userID, out userSubID);

                if ((userID <= 0) || (userClassID <= 0))
                    return null;

                IFeatureClassContainer container = (IFeatureClassContainer)geomNetwork;
                IFeatureClass featClass = container.get_ClassByID(userClassID);

                return LabelHelpers.GetFeature(featClass, userID); // featClass.GetFeature(userID);
            }
            catch (System.Exception ex)
            {
                //  Log.Error(ServiceConfiguration.Name, "Error: LabelHelpers.GetFeaturefromEID EID:" + EID + " GeomtryType: " + elementType + " message: " + ex.Message + " stack: " + ex.StackTrace.ToString());

                return null;
            }
        }

        /// <summary>
        /// Gets current geometricnetwork in a workspace.
        /// </summary>
        /// <param name="ws"></param>
        /// <returns></returns>
        internal static IGeometricNetwork GetNetworks(IWorkspace workspace)
        {
            IGeometricNetwork MMgeometricNetworks = null;
            // Get the circuit source objects first.
            IMMEnumObjectClass circuitSources = ModelNameManager.Instance.ObjectClassesFromModelNameWS(workspace, "CIRCUITSOURCE");
            circuitSources.Reset();
            IObjectClass circuitSource = circuitSources.Next();
            if (circuitSource != null)
            {
                IEnumRelationshipClass relationClasses = circuitSource.get_RelationshipClasses(esriRelRole.esriRelRoleDestination);
                relationClasses.Reset();
                IRelationshipClass relationClass = null;
                while ((relationClass = relationClasses.Next()) != null)
                {
                    if (relationClass.OriginClass is INetworkClass)
                    {
                        IGeometricNetwork geomNetwork = ((INetworkClass)relationClass.OriginClass).GeometricNetwork;
                        if ((geomNetwork.Network is INetSchema) && (((INetSchema)geomNetwork.Network).WeightByName["MMElectricTraceWeight"] != null))
                        {
                            MMgeometricNetworks = geomNetwork;
                            break;
                        }
                    }
                }
            }

            return MMgeometricNetworks;
        }

        /// <summary>
        /// Checks if geometry is in 50 scale polygon.
        /// </summary>
        /// <param name="input"></param>
        /// <param name="workspace"></param>
        /// <returns></returns>
        internal static bool IsFiftyScale(IGeometry input, IWorkspace workspace)
        {
            bool result = false;
            // Layer used to check EDGIS.PGE_50ScaleBoundary.
            IFeatureClass featureClass = GetFeatureClass(_pge50ScaleLayer, workspace);

            ISpatialFilter spatialFilter = new SpatialFilter
            {
                Geometry = input,
                GeometryField = featureClass.ShapeFieldName,
                SpatialRel = esriSpatialRelEnum.esriSpatialRelIntersects
            };

            IFeatureCursor featureCursor = featureClass.Search(spatialFilter, false);
            IFeature feat;
            while ((feat = featureCursor.NextFeature()) != null)
            {
                result = true;
                break;
            }

            while (Marshal.ReleaseComObject(featureCursor) > 0) { }
            while (Marshal.ReleaseComObject(spatialFilter) > 0) { }

            return result;
        }

        //internal static ADODB.Recordset GetRecordset(string strQuery, IWorkspace pworkspace)
        //{
        //    ADODB.Recordset pRSet = new ADODB.Recordset();
        //    ADODB.Connection _adoConnection = new ADODB.ConnectionClass();
        //    try
        //    {
        //        pRSet.CursorLocation = ADODB.CursorLocationEnum.adUseClient;
        //        ESRI.ArcGIS.DataSourcesOleDB.IFDOToADOConnection _fdoToadoConnection = new ESRI.ArcGIS.DataSourcesOleDB.FdoAdoConnectionClass();

        //        _adoConnection = _fdoToadoConnection.CreateADOConnection(pworkspace as IWorkspace) as ADODB.Connection;

        //        pRSet.Open(strQuery, _adoConnection, ADODB.CursorTypeEnum.adOpenKeyset, ADODB.LockTypeEnum.adLockBatchOptimistic, 0);

        //    }
        //    catch
        //    {
        //        pRSet = null;
        //    }
        //    finally
        //    {
        //        //if (_adoConnection.State == 1)
        //        //{
        //        //    _adoConnection.Close();
        //        //}
        //    }
        //    return pRSet;
        //}
    }

    /// <summary>
    /// Helps with field management.
    ///   - stores used found fields and indexes.
    ///   - has helper functions.
    /// </summary>
    internal static class FieldHelper
    {
        #region Class variables
        // Field index storage.
        private static Dictionary<string, int> FieldIndexes = new Dictionary<string, int>();

        #endregion

        /// <summary>
        /// Used to initialize fieldindexes from a configuration file (fields that do not have a modelname).
        ///    - Format is derived from PGE_OpenPointADMSLabel_Config.xml
        /// </summary>
        /// <param name="xmlpath">Full path with file name to xml document.</param>
        public static void Initialize(string xmlpath, IWorkspace workspace)
        {
            ReadXMLFile(xmlpath, workspace);
        }

        /// <summary>
        /// Store field indexes we visit.
        /// </summary>
        /// <param name="input">Feature field belongs to.</param>
        /// <param name="fieldName">Field you want or modelname.</param>
        /// <param name="isModelName">Is fieldname from FieldFromModelName.</param>
        /// <returns>index of field</returns>
        internal static int FieldIndex(IFeature input, string fieldName, bool isModelName = false)
        {
            int result = -1;
            // Using fully qualified name ex: EDGIS.DEVICEGROUP.ADMSLABEL   [schema].[feature class name].[field name].

            string featureClassName = (input.Class as IDataset).Name.ToUpper();
            string full = featureClassName + "." + fieldName.ToUpper();

            if (FieldIndexes.ContainsKey(full))
            {
                result = FieldIndexes[full];
            }
            else
            {
                // Field doesn't exist yet, so lets add it.
                int i = -1;
                if (!isModelName)
                {
                    i = input.Fields.FindField(fieldName);
                    FieldIndexes.Add(full, i);
                    result = i;
                }
                else
                {
                    IField field = ModelNameManager.Instance.FieldFromModelName(input.Class, fieldName);
                    if (field != null)
                    {
                        // Add model name with index so we don't have to grab field name again.
                        i = input.Fields.FindField(field.Name);
                        full = featureClassName + "." + field.Name.ToUpper();
                        if (!FieldIndexes.ContainsKey(full))
                            FieldIndexes.Add(full, i);
                        // Add actual field name, we may be sloppy in the future and use both.
                        full = featureClassName + "." + fieldName.ToUpper();
                        if (!FieldIndexes.ContainsKey(full))
                        {
                            FieldIndexes.Add(full, i);
                        }
                        result = i;
                    }

                }

            }
            return result;
        }


        /// <summary>
        /// Returns a fields value.
        /// </summary>
        /// <param name="input">Feature field should be on.</param>
        /// <param name="fieldName">String name or modelname of field (not alias).</param>
        /// <param name="isModelName">Is fieldname from FieldFromModelName.</param>
        /// <returns></returns>
        internal static string GetFieldValue(IFeature input, string fieldName, bool isModelName = false)
        {
            string result = string.Empty;
            // Search the indexes.
            int idx = FieldIndex(input, fieldName, isModelName);

            if (idx > 0)
            {
                object obj = input.get_Value(idx);
                if (obj != null)
                    result = obj.ToString().Trim();
            }

            return result;
        }

        /// <summary>
        /// Have an initial field list to load.
        ///    -- This allows the user to modify the field name in the database.
        ///    -- The Name attribute in the xml is what the code is searching for.
        ///    -- The Element value is what the database contains.
        /// </summary>
        /// <param name="file">xml file to read</param>
        /// <param name="workspace"></param>
        private static void ReadXMLFile(string file, IWorkspace workspace)
        {
            using (XmlReader reader = XmlReader.Create(new StreamReader(file)))
            {
                string fcname = string.Empty;
                List<string> fields = new List<string>();
                string fldname = string.Empty;

                while (reader.Read())
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:
                            if (reader.Name.ToUpper() == "OBJECTCLASS")
                            {
                                if (fields.Count() > 0)
                                    AddValueFromXML(fields, fcname, workspace);

                                fields = new List<string>();
                                fcname = reader.GetAttribute("Name").ToUpper();
                                fields = new List<string>();
                            }
                            else if (reader.Name.ToUpper() == "FIELD")
                            {
                                fldname = reader.GetAttribute("Name").ToUpper();
                                fields.Add(fldname);
                            }

                            break;
                    }
                }

                // Do last one. 
                if (fields.Count() > 0)
                    AddValueFromXML(fields, fcname, workspace);
            }
        }

        /// <summary>
        /// Adds the values from ReadXMLFile to local variable for use by tool.
        /// </summary>
        /// <param name="fields">List of fields to add.</param>
        /// <param name="fcname">Feature class fields are in.</param>
        /// <param name="workspace"></param>
        private static void AddValueFromXML(List<string> fields, string fcname, IWorkspace workspace)
        {
            IFeatureWorkspace featWorkspace = workspace as IFeatureWorkspace;
            IFeatureClass fc = featWorkspace.OpenFeatureClass(fcname);
            int idx;
            string full = fcname;

            foreach (string f in fields)
            {
                full = fcname.ToUpper() + "." + f.ToUpper();
                if (!(FieldIndexes.ContainsKey(full)))
                {
                    idx = fc.Fields.FindField(f);
                    if (idx > 0)
                    {
                        FieldIndexes.Add(full, idx);
                    }
                }
            }
        }
    }

}