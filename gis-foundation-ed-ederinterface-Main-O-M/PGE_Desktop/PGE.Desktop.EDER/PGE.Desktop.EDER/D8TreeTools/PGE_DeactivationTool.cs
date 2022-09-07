using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.Editor;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geodatabase;
using Miner.ComCategories;
using Miner.Interop;
using PGE.Common.Delivery.Diagnostics;
using PGE.Common.Delivery.Framework;

namespace PGE.Desktop.EDER.D8TreeTools
{
    [Guid("2A6276DF-9FD7-4C1E-BD92-CE7235515BCB")]
    [ClassInterface(ClassInterfaceType.None)]
    [ComponentCategory(ComCategory.D8SelectionTreeTool)]
    public class PGE_DeactivationTool : IMMTreeViewTool
    {
        private static readonly Log4NetLogger _logger = new Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "EDERDesktop.log4net.config");
        private static IMMTreeTool OOTBAbandon = null;
        private IEditor _editor = null;
        private IInvalidArea _invalidArea = null;
        public static List<IObject> parentObjectList;
        public static Dictionary<string, Dictionary<string, string>> parentObjectKayVal;

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


        #region Constructor
        public PGE_DeactivationTool()
        {
            if (OOTBAbandon == null)
            {
                string MMAbandonProgID = "MMAbandonTools.MMTVAbandon"; //"MMAbandonTools.MMTVRemove"; //
                Type oType = Type.GetTypeFromProgID(MMAbandonProgID);
                object obj = Activator.CreateInstance(oType);
                OOTBAbandon = obj as IMMTreeTool;
            }

            if (_editor == null)
            {
                UID uID = new UID();
                uID.Value = "esriEditor.Editor";
                _editor = PGE.Common.Delivery.ArcFM.ApplicationFacade.Application.FindExtensionByCLSID(uID) as IEditor;
            }

            if (_invalidArea == null)
            {
                _invalidArea = new InvalidAreaClass();
            }
        }
        #endregion


        #region IMMTreeViewTool

        public int Category
        {
            get { return OOTBAbandon.Category; }
        }

        public void Execute(IMMTreeViewSelection pSelection)
        {
            if (pSelection.Count > 1)
            {
                return;
            }

            ID8EnumListItem enumListItems = pSelection as ID8EnumListItem;
            enumListItems.Reset();
            ID8ListItem listItem = enumListItems.Next();

            if (listItem.ItemType == mmd8ItemType.mmd8itFeature)
            {
                ID8GeoAssoc geoAssoc = listItem as ID8GeoAssoc;
                if (geoAssoc.AssociatedGeoRow is IFeature)
                {
                    IFeature feature = geoAssoc.AssociatedGeoRow as IFeature;
                    var relatedObjects = feature.GetRelatedObjects(null, esriRelRole.esriRelRoleAny, SchemaInfo.Electric.ClassModelNames.PGEConduitSystem); //("Conduit System");
                    parentObjectList = null;
                    parentObjectList = relatedObjects.ToList();
                    IEnumRelationshipClass enumRelationshipClass = (feature.Class).get_RelationshipClasses(esriRelRole.esriRelRoleAny);
                    enumRelationshipClass.Reset();
                    PreserveSettingsForRelationship(enumRelationshipClass);
                    if (ModelNameFacade.ContainsAllFieldModelNames(feature.Class, SchemaInfo.General.FieldModelNames.DeactivateIndicator))
                    {
                        try
                        {
                            IWorkspaceEdit workspaceEdit = ((IDataset)feature.Class).Workspace as IWorkspaceEdit;

                            //Fix: Bug 14798 [Developer:- Bhaskar Singh] 
                            workspaceEdit.StartEditing(false);
                            workspaceEdit.StartEditOperation(); //Start edit session. If already started the avoid restarting this...
                            //_editor.StartEditing(((IDataset)feature.Class).Workspace);
                            //_editor.StartOperation();

                            int deactivatorFieldIndex = ModelNameFacade.FieldIndexFromModelName(feature.Class, SchemaInfo.General.FieldModelNames.DeactivateIndicator);
                            feature.set_Value(deactivatorFieldIndex, SchemaInfo.General.Domains.YesNoText.Yes);
                            int enabledFieldIndex = ModelNameFacade.FieldIndexFromModelName(feature.Class, SchemaInfo.General.FieldModelNames.Enabled);
                            feature.set_Value(enabledFieldIndex, SchemaInfo.General.Domains.EnabledDomain.False);
                            feature.Store();
                            //_editor.StopOperation("PGE - Deactivation Tool");

                            //Fix: Bug 14798 [Developer:- Bhaskar Singh]
                            //Stop the edit session
                            workspaceEdit.StopEditOperation();

                            _invalidArea.Display = _editor.Display;
                            //_editor.Map.ClearSelection();
                            _invalidArea.Invalidate((short)esriScreenCache.esriAllScreenCaches);

                            //_editor.Map.SelectByShape(feature.Shape, null, true);

                            IEnumLayer enumLayer = _editor.Map.get_Layers(CartoFacade.UIDFacade.FeatureLayers, true);
                            ILayer layer;

                            while ((layer = enumLayer.Next()) != null)//loop through each layer
                            {
                                if (layer.Name == feature.Class.AliasName)
                                {
                                    _editor.Map.SelectFeature(layer, feature);
                                    break;
                                }
                            }

                            _logger.Debug("PGE - Deactivation Tool. Message: " + feature.Class.AliasName + " -- OID: " + feature.OID + " deactivated.");

                        }
                        catch (Exception e)
                        {
                            _logger.Error("PGE - Deactivation Tool failed. Message: " + feature.Class.AliasName + " -- " + e.Message + " Stacktrace: " + e.StackTrace);
                        }
                    }
                    else
                    {
                        enumListItems.Reset();
                        OOTBAbandon.Execute(enumListItems, pSelection.Count);
                        _logger.Debug("PGE - Deactivation Tool. Message: " + feature.Class.AliasName + " -- OID: " + feature.OID + " deactivated.");
                    }
                }
            }
        }

        private void PreserveSettingsForRelationship(IEnumRelationshipClass enumRelationshipClass)
        {
            try
            {
                parentObjectKayVal = null;
                IRelationshipClass relationship = null;
                parentObjectKayVal = new Dictionary<string, Dictionary<string, string>>();
                while ((relationship = enumRelationshipClass.Next()) != null)
                {
                    if (relationship.BackwardPathLabel == SchemaInfo.Electric.Rel_BackwardPathLabel)// "Conduit System")
                    {
                        foreach (IObject parentObject in parentObjectList)
                        {
                            ITable relObj = relationship as ITable;
                            if (relObj != null)
                            {
                                int row = relObj.FindField(SchemaInfo.Electric.UlsObjectId);// "ULSOBJECTID");
                                int uLS_POS = relObj.FindField(SchemaInfo.Electric.UlsPosition);//"ULS_POSITION");
                                int phase = relObj.FindField(SchemaInfo.Electric.Phasedesignation);//"PHASEDESIGNATION");
                                string whereClause = string.Format("{0}='{1}'", SchemaInfo.Electric.UlsObjectId, parentObject.OID);
                                IQueryFilter filter = new QueryFilterClass();
                                filter.WhereClause = whereClause;
                                ICursor featureCursor = relObj.Search(filter, false);
                                IRow irow;
                                while ((irow = featureCursor.NextRow()) != null)
                                {
                                    Dictionary<string, string> valuePairs = new Dictionary<string, string>();
                                    if (!parentObjectKayVal.ContainsKey(irow.get_Value(row).ToString()))
                                    {
                                        valuePairs.Add(irow.get_Value(uLS_POS).ToString(), irow.get_Value(phase).ToString());
                                        parentObjectKayVal.Add(irow.get_Value(row).ToString(), valuePairs);
                                    }
                                }
                                if (featureCursor != null)
                                {
                                    while (Marshal.ReleaseComObject(featureCursor) > 0) { }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                _logger.Error("Error while running Preserve Relationship: " + e.Message);
            }
        }

        public string Name
        {

            get {
                _logger.Info("PGE - Deactivation Tool");
                return "PGE - Deactivation Tool"; }
        }

        public int Priority
        {
            get { return OOTBAbandon.Priority; }
        }

        public int get_Enabled(IMMTreeViewSelection pSelection)
        {
            bool isEnabled = false;
            int iEnable = (int)(mmToolState.mmTSVisible);
            try
            {
                ID8EnumListItem enumListItems = pSelection as ID8EnumListItem;
                enumListItems.Reset();
                ID8ListItem listItem = enumListItems.Next();

                while ((listItem != null))
                {
                    if (listItem.ItemType == mmd8ItemType.mmd8itFeature)
                    {
                        ID8GeoAssoc geoAssoc = listItem as ID8GeoAssoc;
                        if (geoAssoc.AssociatedGeoRow is IFeature)
                        {
                            IFeature feature = geoAssoc.AssociatedGeoRow as IFeature;
                            isEnabled = true;
                            if (!ModelNameFacade.ContainsClassModelName(feature.Class, SchemaInfo.General.ClassModelNames.Deactivate)) isEnabled = false;
                        }
                    }

                    if (!isEnabled) break;

                    listItem = enumListItems.Next();
                }

                if (isEnabled)
                {
                    if (_editor.EditState != esriEditState.esriStateEditing) isEnabled = false;
                }

                if (isEnabled)
                {
                    isEnabled = Convert.ToBoolean(OOTBAbandon.get_Enabled(enumListItems, pSelection.Count));
                }

                if (isEnabled)
                {
                    iEnable=(int)(mmToolState.mmTSEnabled | mmToolState.mmTSVisible);
                }
                else
                    iEnable=(int)(mmToolState.mmTSVisible);
            }
            catch (Exception ex)
            {
                _logger.Error("ERROR - ", ex);
                // enabled = - 1;
            }
            return iEnable;
        }

        #endregion
    }
}
