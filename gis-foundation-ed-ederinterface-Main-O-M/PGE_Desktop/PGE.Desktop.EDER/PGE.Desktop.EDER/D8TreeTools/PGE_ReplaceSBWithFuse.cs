using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using Miner.ComCategories;
using Miner.Interop;
using Miner.Framework;
using ESRI.ArcGIS.Geodatabase;
using PGE.Common.Delivery.Framework;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Editor;
using ESRI.ArcGIS.EditorExt;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Display;
using PGE.Common.Delivery.Diagnostics;
using System.Reflection;
using PGE.Desktop.EDER.ValidationRules;
using PGE.Desktop.EDER.AutoUpdaters.Special;
using PGE.Desktop.EDER.AutoUpdaters;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.SystemUI;
using ESRI.ArcGIS.ArcMapUI;

namespace PGE.Desktop.EDER.D8TreeTools
{
    /// <summary>
    /// This tree view tool will create a new feature with the same attributes as the
    /// feature selected (based on configuration), unrelate all features from original feature
    /// and relate those features to the new feature.  It will then call the OOTB abandon and
    /// remove functionality on the selected feature.  This is the PG&E asset replacement tool
    /// </summary>
    [Guid("6CD7588E-D17F-452C-955F-205C6363A750")]
    [ClassInterface(ClassInterfaceType.None)]
    [ComponentCategory(ComCategory.D8SelectionTreeTool)]
    public class PGE_ReplaceSBWithFuse : IMMTreeViewTool
    {
        private static readonly Log4NetLogger _logger = new Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "EDERDesktop.log4net.config");
        private static IMMTreeTool OOTBAbandonAndRemove = null;
        private IEditor _editor = null;
        private IEngineEditor _enginEditor = null;
        private IInvalidArea _invalidArea = null;
        private IFeature _newFeature = null;
        private object _replacedFeatureGUIDValue = null;
        private IGeometry _replacedFeatureShape = null;        

        #region Com Reg Methods

        /// <summary>
        /// Registers the specified reg key.
        /// </summary>
        /// <param name="regKey">The reg key.</param>
        [ComRegisterFunction]
        private static void Register(string regKey)
        {
            Miner.ComCategories.D8SelectionTreeTool.Register(regKey);
        }
        /// <summary>
        /// Uns the register.
        /// </summary>
        /// <param name="regKey">The reg key.</param>
        [ComUnregisterFunction]
        private static void UnRegister(string regKey)
        {
            Miner.ComCategories.D8SelectionTreeTool.Unregister(regKey);
        }

        #endregion

        public PGE_ReplaceSBWithFuse()
        {
            _logger.Debug("Initializing PGE_ReplaceSBWithFuse");

            if (OOTBAbandonAndRemove == null)
            {
                string MMAbandonAndRemoveProgID = "MMAbandonTools.MMTVRemove";
                // We get the type using just the ProgID
                Type oType = Type.GetTypeFromProgID(MMAbandonAndRemoveProgID);
                if (oType != null)
                {
                    object obj = Activator.CreateInstance(oType);
                    OOTBAbandonAndRemove = obj as IMMTreeTool;
                }
            }

            if (_editor == null)
            {
                UID uID = new UID();
                uID.Value = "esriEditor.Editor";
                _editor = PGE.Common.Delivery.ArcFM.ApplicationFacade.Application.FindExtensionByCLSID(uID) as IEditor;
            }

            if (_enginEditor == null)
            {
                _enginEditor = new EngineEditorClass();
            }

            if (_invalidArea == null)
            {
                _invalidArea = new InvalidAreaClass();
            }
        }

        #region IMMTreeViewTool
        /// <summary>
        /// Use the same category as the OOTB abandon and remove tool
        /// </summary>
        public int Category
        {
            get { return OOTBAbandonAndRemove.Category; }
        }

        public void Execute(IMMTreeViewSelection pSelection)
        {
            _logger.Debug("Executing PGE_ReplaceSBWithFuse");

            //Set flag to indicate Enable Symbol Number AU 
            EvaluationEngine.Instance.EnableSymbolNumberAU = true;

            Dictionary<int, object> fieldMapping = null;
            Dictionary<IRelationshipClass, List<IObject>> relationshipMapping = null;
            try
            {
                ID8EnumListItem enumListItems = pSelection as ID8EnumListItem;
                enumListItems.Reset();

                ID8ListItem listItem = enumListItems.Next();

                if (listItem != null)
                {
                    if (listItem.ItemType == mmd8ItemType.mmd8itFeature || listItem.ItemType == mmd8ItemType.mmitRelationship)
                    {
                        ID8GeoAssoc geoAssoc = listItem as ID8GeoAssoc;
                        IObject SBObject = geoAssoc.AssociatedGeoRow as IObject;
                        IObjectClass objectClass = SBObject.Class;

                        //Get list of fields with modelname PGE_ASSETCOPY which will be 
                        //copied from the old object to the new object 
                        fieldMapping = GetValuesToMap(SBObject);

                        relationshipMapping = GetRelationshipMapping(SBObject);

                        _editor.StartOperation();

                        int installJobPrefixFldIdx = SBObject.Fields.FindField("installjobprefix");

                        //Give the install job prefix the default value for the field
                        string installJobPrefix = "";
                        if (!fieldMapping.ContainsKey(installJobPrefixFldIdx))
                        {
                            installJobPrefix = SBObject.Fields.get_Field(
                                installJobPrefixFldIdx).DefaultValue.ToString();
                        }
                       
                        string jobNumber = default(string);
                        int installJobYear = default(int);
                        DateTime installationdate = default(DateTime);

                        if (SBObject.get_Value(SBObject.Fields.FindField("installjobnumber")) != DBNull.Value)
                            jobNumber = Convert.ToString(SBObject.get_Value(SBObject.Fields.FindField("installjobnumber")));

                        if (SBObject.get_Value(SBObject.Fields.FindField("installjobyear")) != DBNull.Value)
                            installJobYear = Convert.ToInt16(SBObject.get_Value(SBObject.Fields.FindField("installjobyear")));

                        if (SBObject.get_Value(SBObject.Fields.FindField("installationdate")) != DBNull.Value)
                            installationdate = Convert.ToDateTime(SBObject.get_Value(SBObject.Fields.FindField("installationdate")));
                          
                        DeleteObject(SBObject);

                        //Set flag to indicate Disable Symbol Number AU 
                        EvaluationEngine.Instance.EnableSymbolNumberAU = false;
                       
                        objectClass =  ModelNameFacade.FeatureClassByModelName((SBObject.Class as IDataset).Workspace, SchemaInfo.Electric.ClassModelNames.PGEReplacedFuse);
                        PGE.Common.Delivery.ArcFM.DoNotOperateAutoUpdaters.Instance.AddAU(objectClass.ObjectClassID, typeof(PGEPopulateLastJobNumberAU));
                      

                        //Create our new feature
                        IObject newObject = CreateNewSBObject(fieldMapping, objectClass);

                        //Set all relationships back up with the new feature
                        SetRelationships(relationshipMapping, newObject);

                        int installjobNumFldIdxSwitch = newObject.Fields.FindField("installjobnumber");
                        int installJobPrefixFldIdxSwitch = newObject.Fields.FindField("installJobPrefix");
                        newObject.set_Value(installJobPrefixFldIdxSwitch, installJobPrefix);
                        newObject.set_Value(installjobNumFldIdxSwitch, jobNumber);
                        newObject.set_Value(newObject.Fields.FindField("subtypecd"), 1);
                        newObject.set_Value(newObject.Fields.FindField("installjobyear"), installJobYear);
                        newObject.set_Value(newObject.Fields.FindField("installationdate"), installationdate);

                        // ME Q4 19 Release DA#190804
                        //Setting Default value at time of Replace SB with Fuse
                        newObject.set_Value(newObject.Fields.FindField("INSTALLATIONTYPE"), "OH");

                        newObject.Store();
                        //******************************************************* 
                        PGE.Common.Delivery.ArcFM.DoNotOperateAutoUpdaters.Instance.RemoveAU(objectClass.ObjectClassID, typeof(PGEPopulateLastJobNumberAU));
                       
                        //Enable Undo/Redo button in ArcMap
                        IMxDocument mxDoc = _editor.Parent.Document as IMxDocument;
                        mxDoc.UpdateContents();

                        _editor.StopOperation("PGE_ReplaceSBWithFuse");

                        //Resfresh geography and graphics so anno will reflect changes 
                        mxDoc.ActiveView.PartialRefresh(
                            esriViewDrawPhase.esriViewGeography | esriViewDrawPhase.esriViewGraphics,
                            null, null);
                    }
                }

                _editor.Map.ClearSelection();
                if (_newFeature != null)
                {
                    //PGE IT change for November 15 2014 release. There was a problem 
                    //in the refresh selection logic

                    //Refresh the selection 
                    Hashtable hshOIds = new Hashtable();
                    hshOIds.Add(_newFeature.OID, 0);
                    ValidationEngine.Instance.Application = _editor.Parent;
                    ValidationEngine.Instance.AddToSelection(
                        ((IDataset)_newFeature.Class).Name.ToLower(), hshOIds);
                }
            }
            catch (Exception e)
            {
                System.Windows.Forms.MessageBox.Show("Failed executing PGE_ReplaceSBWithFuse.  Message: " + e.Message);

                _logger.Debug("Failed executing PGE_ReplaceSBWithFuse.  Message: " + e.Message + " StackTrace: " + e.StackTrace);

                if (((IWorkspaceEdit2)_editor.EditWorkspace).IsInEditOperation)
                {
                    _editor.AbortOperation();
                }

            }   
            finally
            {
                //Set flag to indicate Disable Symbol Number AU 
                EvaluationEngine.Instance.EnableSymbolNumberAU = false;
                //Cleanup
                if (fieldMapping != null)
                {
                    fieldMapping.Clear();
                }
                if (relationshipMapping != null)
                {
                    foreach (KeyValuePair<IRelationshipClass, List<IObject>> kvp in relationshipMapping)
                    {
                        foreach (IObject obj in kvp.Value)
                        {
                            while (Marshal.ReleaseComObject(obj) > 0) { }
                        }
                        while (Marshal.ReleaseComObject(kvp.Key) > 0) { }
                    }
                    relationshipMapping.Clear();
                }

                if (_newFeature != null)
                {
                    while (Marshal.ReleaseComObject(_newFeature) > 0) { }
                    _newFeature = null;
                }                
            }

        }


        public string Name
        {
            get {
                _logger.Info("PGE - Replace with Fuse");
                return "PGE - Replace with Fuse"; }
        }

        /// <summary>
        /// Set the priority to the same as the OOTB abandon and remove tool
        /// </summary>
        public int Priority
        {
            get { return OOTBAbandonAndRemove.Priority; }
        }

        /// <summary>
        /// Will return enabled if it contains the PGE_ReplaceSBWithFuse model name
        /// and if the out of the box abandon and remove would be enabled
        /// </summary>
        /// <param name="pSelection"></param>
        /// <returns></returns>
        public int get_Enabled(IMMTreeViewSelection pSelection)
        {
            int isEnabled = 0;

            _logger.Debug("Entering into get_Enabled of PGE_ReplaceSBWithFuse");
            try
            {
                ID8EnumListItem enumListItems = pSelection as ID8EnumListItem;
                enumListItems.Reset();
                ID8ListItem listItem = enumListItems.Next();
                if (listItem != null)
                {
                    if ((listItem.ItemType == mmd8ItemType.mmd8itFeature) || (listItem.ItemType == mmd8ItemType.mmitRelationship))
                    {
                        ID8GeoAssoc geoAssoc = listItem as ID8GeoAssoc;
                        if (geoAssoc != null && geoAssoc.AssociatedGeoRow is IObject)
                        {
                            IObject oldObject = geoAssoc.AssociatedGeoRow as IObject;
                            isEnabled = 1;
                            if (!ModelNameFacade.ContainsClassModelName(oldObject.Class, SchemaInfo.General.ClassModelNames.ReplaceSBWithFuseMN)) { isEnabled = 0; }
                            if (!ModelNameFacade.ContainsFieldModelName(oldObject.Class, SchemaInfo.General.FieldModelNames.ReplaceGUID)) { isEnabled = 0; }
                            try
                            {
                                if (Convert.ToDecimal(oldObject.get_Value(oldObject.Fields.FindField("SUBTYPECD"))) != 1) { isEnabled = 0; }
                            }
                            catch (Exception e)
                            { }
                        }
                    }
                }

                if (isEnabled == 1)
                {
                    if (_editor.EditState != esriEditState.esriStateEditing) { isEnabled = 0; }
                }

                //if (isEnabled == 1)
                //{
                //    isEnabled = OOTBAbandonAndRemove.get_Enabled(enumListItems, pSelection.Count);
                //}
            }
            catch (Exception ex)
            {
                _logger.Error("ERROR - ", ex);
                //enabled = -1;
            }

            if (isEnabled == 1)
            {
                return (int)(mmToolState.mmTSEnabled | mmToolState.mmTSVisible);
            }
            else
                return (int)(mmToolState.mmTSVisible);
            //return 0;
        }
        #endregion

        private void RelatedAbandonedRow(IFeature newFeature)
        {
            try
            {
                if (PGE_Asset_Replacement_AU.abandonedTable != null && PGE_Asset_Replacement_AU.newAbandonedRow != null)
                {
                    IEnumRelationshipClass relClasses = newFeature.Class.get_RelationshipClasses(esriRelRole.esriRelRoleDestination);
                    relClasses.Reset();
                    IRelationshipClass relClass = relClasses.Next();
                    while (relClass != null)
                    {
                        if (relClass.OriginClass == PGE_Asset_Replacement_AU.abandonedTable)
                        {
                            relClass.CreateRelationship(PGE_Asset_Replacement_AU.newAbandonedRow, newFeature);
                            return;
                        }
                        relClass = relClasses.Next();
                    }

                    relClasses = newFeature.Class.get_RelationshipClasses(esriRelRole.esriRelRoleOrigin);
                    relClasses.Reset();
                    relClass = relClasses.Next();
                    while (relClass != null)
                    {
                        if (relClass.DestinationClass == PGE_Asset_Replacement_AU.abandonedTable)
                        {
                            relClass.CreateRelationship(newFeature, PGE_Asset_Replacement_AU.newAbandonedRow);
                            return;
                        }
                        relClass = relClasses.Next();
                    }
                }
            }
            catch (Exception e)
            {
                _logger.Error("Error executing PGE_ReplaceSBWithFuse AU. Message: " + e.Message + " StackTrace: " + e.StackTrace);
            }
            finally
            {
                PGE_Asset_Replacement_AU.abandonedTable = null;
                PGE_Asset_Replacement_AU.newAbandonedRow = null;
            }
        }

        /// <summary>
        /// This method will take all relationships that are current assigned to the oldFeature, remove
        /// them and then apply them to the new feature.

        /// </summary>
        /// <param name="oldObject">Feature being replaced</param>
        private Dictionary<IRelationshipClass, List<IObject>> GetRelationshipMapping(IObject oldObject) //IFeature oldFeature
        {
            Dictionary<IRelationshipClass, List<IObject>> relationshipMapping = new Dictionary<IRelationshipClass, List<IObject>>();
            //IFeatureClass featClass = oldFeature.Class as IFeatureClass;
            IObjectClass objClass = oldObject.Class as IObjectClass;

            //Process relationships where this class is the destination
            IEnumRelationshipClass relClasses = objClass.get_RelationshipClasses(esriRelRole.esriRelRoleDestination);
            relClasses.Reset();
            IRelationshipClass relClass = relClasses.Next();
            while (relClass != null)
            {
                //Don't map annotation relationships
                if (relClass.OriginClass is IFeatureClass)
                {
                    IFeatureClass originClass = relClass.OriginClass as IFeatureClass;
                    if (originClass.FeatureType == esriFeatureType.esriFTAnnotation)
                    {
                        relClass = relClasses.Next();
                        continue;
                    }
                }
                if (!relationshipMapping.ContainsKey(relClass))
                {
                    relationshipMapping.Add(relClass, new List<IObject>());
                }
                ISet relatedFeatures = relClass.GetObjectsRelatedToObject(oldObject);
                relatedFeatures.Reset();
                for (int i = 0; i < relatedFeatures.Count; i++)
                {
                    IObject obj = relatedFeatures.Next() as IObject;
                    relationshipMapping[relClass].Add(obj);
                    relClass.DeleteRelationship(obj, oldObject);
                    //relClass.CreateRelationship(obj, newFeature);
                }
                 relClass = relClasses.Next();
            }


            //Process relationships where this class is the origin
            relClasses = objClass.get_RelationshipClasses(esriRelRole.esriRelRoleOrigin);
            relClasses.Reset();
            relClass = relClasses.Next();
            while (relClass != null)
            {
                //Don't map annotation relationships
                if (relClass.DestinationClass is IFeatureClass)
                {
                    IFeatureClass originClass = relClass.DestinationClass as IFeatureClass;

                    //Simon Change remove this (we want to include the anno) 
                    if (originClass.FeatureType == esriFeatureType.esriFTAnnotation)
                        System.Diagnostics.Debug.Print(originClass.AliasName);

                    //uncommented these lines again-- donot include annotation
                    if (originClass.FeatureType == esriFeatureType.esriFTAnnotation)
                    {
                        relClass = relClasses.Next();
                        continue;
                    }
                }
                if (!relationshipMapping.ContainsKey(relClass))
                {
                    relationshipMapping.Add(relClass, new List<IObject>());
                }
                ISet relatedFeatures = relClass.GetObjectsRelatedToObject(oldObject);
                relatedFeatures.Reset();
                for (int i = 0; i < relatedFeatures.Count; i++)
                {
                    IObject obj = relatedFeatures.Next() as IObject;
                    relationshipMapping[relClass].Add(obj);
                    relClass.DeleteRelationship(obj, oldObject);
                    System.Diagnostics.Debug.Print("Deleting rel to OId: " + obj.OID.ToString());
                    //relClass.CreateRelationship(obj, newFeature);
                }
                relClass = relClasses.Next();
            }


            return relationshipMapping;
        }

        //private Dictionary<string, Hashtable> GetAnnoMapping(IObject oldObject) //IFeature oldFeature
        //{
        //    Dictionary<string, List<int>> relationshipMapping = new Dictionary<string, new List<int>();

        //    List <int> pList = new List<int>(); 

        //    //IFeatureClass featClass = oldFeature.Class as IFeatureClass;
        //    IObjectClass objClass = oldObject.Class as IObjectClass;

        //    IEnumRelationshipClass relClasses = objClass.get_RelationshipClasses(esriRelRole.esriRelRoleDestination);
        //    relClasses.Reset();
        //    IRelationshipClass relClass = relClasses.Next();
        //    while (relClass != null)
        //    {
        //        //Don't map annotation relationships
        //        if (relClass.OriginClass is IFeatureClass)
        //        {
        //            IFeatureClass originClass = relClass.OriginClass as IFeatureClass;
        //            if (originClass.FeatureType == esriFeatureType.esriFTAnnotation)
        //            {
        //                relClass = relClasses.Next();
        //                continue;
        //            }
        //        }
        //        if (!relationshipMapping.ContainsKey(relClass))
        //        {
        //            relationshipMapping.Add(relClass, new List<IObject>());
        //        }
        //        ISet relatedFeatures = relClass.GetObjectsRelatedToObject(oldObject);
        //        relatedFeatures.Reset();
        //        for (int i = 0; i < relatedFeatures.Count; i++)
        //        {
        //            IObject obj = relatedFeatures.Next() as IObject;
        //            relationshipMapping[relClass].Add(obj);
        //            relClass.DeleteRelationship(obj, oldObject);
        //            //relClass.CreateRelationship(obj, newFeature);
        //        }
        //        relClass = relClasses.Next();
        //    }


        //    //Process relationships where this class is the origin
        //    relClasses = objClass.get_RelationshipClasses(esriRelRole.esriRelRoleOrigin);
        //    relClasses.Reset();
        //    relClass = relClasses.Next();
        //    while (relClass != null)
        //    {
        //        //Don't map annotation relationships
        //        if (relClass.DestinationClass is IFeatureClass)
        //        {
        //            IFeatureClass originClass = relClass.DestinationClass as IFeatureClass;
        //            if (originClass.FeatureType == esriFeatureType.esriFTAnnotation)
        //            {
        //                relClass = relClasses.Next();
        //                continue;




        //            }
        //        }
        //        if (!relationshipMapping.ContainsKey(relClass))
        //        {
        //            relationshipMapping.Add(relClass, new List<IObject>());
        //        }
        //        ISet relatedFeatures = relClass.GetObjectsRelatedToObject(oldObject);
        //        relatedFeatures.Reset();
        //        for (int i = 0; i < relatedFeatures.Count; i++)
        //        {
        //            IObject obj = relatedFeatures.Next() as IObject;
        //            relationshipMapping[relClass].Add(obj);
        //            relClass.DeleteRelationship(obj, oldObject);
        //            //relClass.CreateRelationship(obj, newFeature);
        //        }
        //        relClass = relClasses.Next();
        //    }


        //    return relationshipMapping;
        //}




        /// <summary>
        /// Relates all features that were related with the previous feature to the newly created feature
        /// </summary>
        /// <param name="relationshipMappingFuse"></param>
        /// <param name="switchObject"></param>
        private void SetRelationships(Dictionary<IRelationshipClass, List<IObject>> relationshipMappingFuse, IObject switchObject) //, IFeature newFeature
        {

            foreach (KeyValuePair<IRelationshipClass, List<IObject>> kvpFuse in relationshipMappingFuse)
            {
                foreach (IObject obj in kvpFuse.Value)
                {
                    if (obj != null)
                    {
                        IEnumRelationshipClass relClassesSwitch = switchObject.Class.get_RelationshipClasses(esriRelRole.esriRelRoleAny);
                        relClassesSwitch.Reset();
                        IRelationshipClass relClassSwitch = relClassesSwitch.Next();
                        while (relClassSwitch != null)
                        {
                            if (kvpFuse.Key.ForwardPathLabel == relClassSwitch.ForwardPathLabel || kvpFuse.Key.BackwardPathLabel == relClassSwitch.BackwardPathLabel)
                            {
                                relClassSwitch.CreateRelationship(switchObject, obj);
                            }
                            relClassSwitch = relClassesSwitch.Next();
                        }
                    }
                }
            }
        }

        /// <summary>
        /// This method will create the new feature based off of the configured values to map
        /// from the original feature
        /// </summary>
        /// <param name="origFeature"></param>
        private IObject CreateNewSBObject(Dictionary<int, object> fieldMapping, IObjectClass objectClass) //Dictionary<int, object> fieldMapping, IFeatureClass featClass, IFeature replacedFeature
        {
            
            IObject newObject = null;

            if (objectClass is IFeatureClass)
            {
                IFeature feat = (objectClass as IFeatureClass).CreateFeature();

                foreach (KeyValuePair<int, object> kvp in fieldMapping)
                {
                    if (kvp.Value != null)
                        feat.set_Value(kvp.Key, kvp.Value);
                }

                feat.Shape = _replacedFeatureShape;

                int ReplaceGUIDIndex = ModelNameFacade.FieldIndexFromModelName(feat.Class, SchemaInfo.General.FieldModelNames.ReplaceGUID);
                feat.set_Value(ReplaceGUIDIndex, _replacedFeatureGUIDValue);

                feat.Store();

                newObject = _newFeature = feat;
            }
            else
            {
                IRow newRow = (objectClass as ITable).CreateRow();
                foreach (KeyValuePair<int, object> kvp in fieldMapping)
                {
                    if (kvp.Value != null)
                        newRow.set_Value(kvp.Key, kvp.Value);
                }

                int ReplaceGUIDIndex = ModelNameFacade.FieldIndexFromModelName(objectClass, SchemaInfo.General.FieldModelNames.ReplaceGUID);
                newRow.set_Value(ReplaceGUIDIndex, _replacedFeatureGUIDValue);

                newRow.Store();

                newObject = newRow as IObject;
            }
            ////_newFeatureList.Add(feat);

            return newObject; //feat
        }

        /// <summary>
        /// This method delets a feature
        /// </summary>
        /// <param name="origFeature"></param>
        private void DeleteObject(IObject objectToDelete) //IFeature feature
        {
            IFeatureEdit featureEdit = null;
            ISet deleteSet = null;

            int GUIDIndex = objectToDelete.Fields.FindField(SchemaInfo.Electric.GlobalID);
            _replacedFeatureGUIDValue = objectToDelete.get_Value(GUIDIndex);


            if (objectToDelete is IFeature)
            {
                _replacedFeatureShape = (objectToDelete as IFeature).ShapeCopy;

                deleteSet = new ESRI.ArcGIS.esriSystem.SetClass();
                deleteSet.Add(objectToDelete);
                deleteSet.Reset();

                featureEdit = deleteSet.Next() as IFeatureEdit;
                featureEdit.DeleteSet(deleteSet);

                _invalidArea.Add(objectToDelete);
            }
            else
            {
                objectToDelete.Delete();
            }

        }

        /// <summary>
        /// Obtains the network analysis extension
        /// </summary>
        /// <returns></returns>
        private INetworkAnalysisExt GetNetworkAnalyst()
        {
            UID uid = new UIDClass();
            uid.Value = "esriEditorExt.UtilityNetworkAnalysisExt";
            INetworkAnalysisExt networkAnalyst = PGE.Common.Delivery.ArcFM.ApplicationFacade.Application.FindExtensionByCLSID(uid) as INetworkAnalysisExt;
            return networkAnalyst;
        }

        /// <summary>
        /// Returns a list of all the fields that are configured for the asset replacement field copy
        /// </summary>
        /// <param name="SBOject"></param>
        /// <returns></returns>
        private Dictionary<int, object> GetValuesToMap(IObject SBOject) //IFeature origFeature
        {
            IFeatureClass fuseFC = ModelNameFacade.FeatureClassByModelName((SBOject.Class as IDataset).Workspace, SchemaInfo.Electric.ClassModelNames.PGEReplacedFuse);
            Dictionary<int, object> valuesToMap = new Dictionary<int, object>();
            List<int> fieldMapping = ModelNameFacade.FieldIndicesFromModelName(SBOject.Class, SchemaInfo.General.FieldModelNames.FuseSBAssetCopyFMN);
            foreach (int fieldIndex in fieldMapping)
            {
                try
                {
                    valuesToMap.Add(fuseFC.FindField(SBOject.Fields.get_Field(fieldIndex).Name), SBOject.get_Value(fieldIndex));
                }
                catch (Exception ex)
                {
                    //add to log
                }
            }
            return valuesToMap;
        }
    }
}

