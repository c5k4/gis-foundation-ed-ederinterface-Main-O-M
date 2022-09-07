#region Organize and sorted using

using System;
using System.Collections; 
using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;

using ESRI.ArcGIS.Geodatabase;

using Miner.ComCategories;
using Miner.Interop;
using PGE.Common.Delivery.Diagnostics;
using PGE.Common.Delivery.Framework;
using ESRI.ArcGIS.Framework;
using PGE.Common.Delivery.ArcFM;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.ArcMapUI;
using System.Collections.Generic;

#endregion

namespace PGE.Desktop.EDER.LocatorTools.MlxRelationshipTool
{
    /// <summary>
    /// Class responsible for relating MLX number to PM Order number.
    /// <remarks>This class implement IMMTreeTool for right click command.</remarks>
    /// </summary>
    [Guid("CC69B2F9-1C13-4301-9ADB-52C43A33B53F")]
    [ProgId("PGE.Desktop.EDER.PGE_PMOrderNumberUI")]
    [ComponentCategory(ComCategory.MMLocatorContextMenu)]
    public class PGE_PMOrderNumberRelate : IMMTreeTool
    {
        #region Private Variable

        /// <summary>
        /// Member Variable for error logging.
        /// </summary>
        private static readonly Log4NetLogger _logger = new Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "EDERDesktop.log4net.config");

        #endregion

        #region Com Reg Methods

        /// <summary>
        /// Registers the specified reg key.
        /// </summary>
        /// <param name="regKey">The reg key.</param>
        [ComRegisterFunction]
        private static void Register(string regKey)
        {
            MMLocatorContextMenu.Register(regKey);
        }
        /// <summary>
        /// Uns the register.
        /// </summary>
        /// <param name="regKey">The reg key.</param>
        [ComUnregisterFunction]
        private static void UnRegister(string regKey)
        {
            MMLocatorContextMenu.Unregister(regKey);
        }

        #endregion

        #region IMMTreeTool implementation

        /// <summary>
        ///  Default action to be performed when the user double-clicks the TreeView item.  
        /// </summary>
        public bool AllowAsDefault
        {
            get { return true; }
        }

        /// <summary>
        ///  Bitmap associated with the context menu item. 
        /// </summary>
        public int Bitmap
        {
            get { return 0; }
        }

        /// <summary>
        /// A group in which to place the tool in the context menu. 
        /// </summary>
        public int Category
        {
            get { return 2000; }
        }

        /// <summary>
        /// Initiate the process of relating MLX number and the PM Order number.
        /// </summary>
        /// <param name="pEnumItems">Currently clicked item by user.</param>
        /// <param name="lItemCount">Count of currently selected items.</param>
        public void Execute(ID8EnumListItem pEnumItems, int lItemCount)
        {
            try
            {
                pEnumItems.Reset();
                PGE_PMOrderNumberRelateUI objectPGE_PMOrderNumberRelateUI = new PGE_PMOrderNumberRelateUI();
                objectPGE_PMOrderNumberRelateUI.ShowDialog();
                if (objectPGE_PMOrderNumberRelateUI.PMOrderNumber != string.Empty)
                {
                    RelateMLXNumbertoPMNumber(pEnumItems, objectPGE_PMOrderNumberRelateUI.PMOrderNumber, lItemCount);
                    objectPGE_PMOrderNumberRelateUI.PMOrderNumber = string.Empty;
                    // _cancel = objectPGE_PMOrderNumberRelateUI.SearchCancel;
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Error : " + ex);
            }
        }

        /// <summary>
        ///  Name which will appear in the context menu. 
        /// </summary>
        public string Name
        {
            get { return "Related Order Number"; }
        }

        /// <summary>
        /// Display order of the tool in its category. 
        /// </summary>
        public int Priority
        {
            get { return 1; }
        }

        /// <summary>
        /// Shortcut key to use the menu item.
        /// </summary>
        public mmShortCutKey ShortCut
        {
            get
            {
                return mmShortCutKey.mmShortcutNone;
            }
        }

        /// <summary>
        /// Evaluate the selected item and enable or disable the tool in the context menu.  
        /// </summary>
        /// <param name="pEnumItems"> The selected items in the tree.</param>
        /// <param name="lItemCount"> Number of selected items in the tree.</param>
        /// <returns></returns>
        public int get_Enabled(ID8EnumListItem pEnumItems, int lItemCount)
        {
            try
            {
                //skip other checks unless exactly one feature is selected
                if (lItemCount != 1)
                    return (int)mmToolState.mmTSNone;

                //only enable if selected feature is point feature
                pEnumItems.Reset();
                ID8GeoAssoc geoAssoc = pEnumItems.Next() as ID8GeoAssoc;
                if (geoAssoc == null)
                    return (int)mmToolState.mmTSNone;

                ITable displayTable = geoAssoc.AssociatedGeoRow.Table as ITable;

                IMMModelNameManager modelNameManager = ModelNameManager.Instance;
                if (!modelNameManager.ContainsClassModelName((displayTable as IObjectClass), SchemaInfo.Electric.ClassModelNames.CustomerAgreementObject))
                {
                    return (int)(mmToolState.mmTSVisible);
                }
                else
                {
                    IWorkspace ws = GetWorkspace();
                    IWorkspaceEdit workspaceEdit = ws as IWorkspaceEdit;
                    if (workspaceEdit.IsBeingEdited())
                    {
                        return (int)(mmToolState.mmTSEnabled | mmToolState.mmTSVisible);
                    }
                    else
                    {
                        return (int)(mmToolState.mmTSVisible);
                    }
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.ToString());
                _logger.Error("Error : " + ex);
                return (int)mmToolState.mmTSNone;
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Relate the MLX number to PMOrder number.
        /// </summary>
        /// <param name="EnumItems">The selected items in the tree.</param>
        /// <param name="PMOrderNumber">PM Order number entered by the user.</param>
        /// <param name="lItemCount"> Number of selected items in the tree.<</param>
        private void RelateMLXNumbertoPMNumber(ID8EnumListItem EnumItems, string PMOrderNumber, int lItemCount)
        {
            IWorkspaceEdit workspaceEdit = null;
            bool hasEditOperation = false;

            try
            {
                IObjectClass featureObject = null;
                IRelationship relationship = null;
                IEnumRelationshipClass enumRelationshipClass = null;
                Hashtable hshRelOIds = new Hashtable();
                IWorkspace ws = (IWorkspace)Miner.Geodatabase.Edit.Editor.EditWorkspace;
                ID8ListItem selectedItem = EnumItems.Next();
                ID8GeoAssoc geoAssoc = (ID8GeoAssoc)selectedItem;
                IRow originRow = geoAssoc.AssociatedGeoRow;
                featureObject = geoAssoc.AssociatedGeoRow.Table as IObjectClass;
                workspaceEdit = ws as IWorkspaceEdit;
                int relCreateCount = 0;
                bool relExists = false;

                //Reworked this function because of the following Remedy Incidents: 
                //INC000004090415, INC000004020260
                //Determine the agreement type as this will dictate which objects 
                //can be related 
                //MLX                       - relate to all relatable objects 
                //TEMPORARY FACILITIES      - relate to all relatable objects 
                //SPECIAL FACILITIES        - relate to all relatable objects
                //RISER POLE AGREEMENT      - relate to SupportStructure only  
                //APPLICANT WARRANTY        - relate to CustomerAgreementNumber, 
                //Padmount Structure, PriUGConductor, SecUGConductor, SubSurfaceStructure 

                //Should create a field modelname for this
                string agreementType = "MLX";
                if (originRow.get_Value(originRow.Fields.FindField("AGREEMENTTYPE")) != DBNull.Value)
                    agreementType = originRow.get_Value(originRow.Fields.FindField("AGREEMENTTYPE")).ToString();

                //Start edit session. If already started the avoid restarting this...
                //Start edit operation 
                workspaceEdit.StartEditOperation();
                hasEditOperation = true;

                IEnumFeatureClass pEnumFC = GetCustAgreementRelatedTables(ws, agreementType);
                IFeatureClass featureClass = null;
                ICursor cursor = null;
                string fieldName = string.Empty;
                IRelationshipClass relationshipClass = null;
                if (featureObject != null)
                {
                    enumRelationshipClass = featureObject.get_RelationshipClasses(esriRelRole.esriRelRoleOrigin);
                }

                // iterate each feature class 
                while ((featureClass = pEnumFC.Next()) != null)
                {
                    IObjectClass objectClass = (IObjectClass)featureClass;
                    if (enumRelationshipClass == null) { return; }
                    enumRelationshipClass.Reset();
                    string CurrentObjectId = string.Empty;
                    // Check destination class and current object class has same ObjectClassID's So that we can create right relation between them.                    
                    while ((relationshipClass = enumRelationshipClass.Next()) != null)
                    {
                        if (relationshipClass.DestinationClass.ObjectClassID == objectClass.ObjectClassID)
                        {
                            break;
                        }
                    }

                    if (relationshipClass != null)
                    {
                        Debug.Print("Processing Rel class: " + ((IDataset)relationshipClass).Name);
                        //Determine the existing related object OIds 
                        hshRelOIds.Clear();
                        ESRI.ArcGIS.esriSystem.ISet pSet = relationshipClass.GetObjectsRelatedToObject((IObject)originRow);
                        pSet.Reset();
                        object pObj = pSet.Next();
                        while (pObj != null)
                        {
                            Debug.Print(((IObject)pObj).OID.ToString());
                            if (!hshRelOIds.ContainsKey(((IObject)pObj).OID))
                                hshRelOIds.Add(((IObject)pObj).OID, 0);

                            pObj = pSet.Next();
                        }
                        Marshal.ReleaseComObject(pSet);
                    }

                    //Search each featureclass using the PM Number 
                    fieldName = string.Empty;
                    if (ModelNameFacade.FieldFromModelName(objectClass, SchemaInfo.Electric.FieldModelNames.JobNumber) != null)
                    {
                        fieldName = ModelNameFacade.FieldFromModelName(objectClass, SchemaInfo.Electric.FieldModelNames.JobNumber).Name;
                    }
                    if (fieldName == string.Empty)
                    {
                        continue;
                    }
                    IQueryFilter queryFilter = new QueryFilterClass
                    {
                        WhereClause = string.Format("UPPER({0}) = UPPER('{1}')", fieldName, PMOrderNumber)
                    };
                    cursor = (ICursor)featureClass.Search(queryFilter, false);
                    IRow destinationRow = null;
                    while ((destinationRow = cursor.NextRow()) != null && relationshipClass != null)
                    {
                        //Check for an existing relationship 
                        Debug.Print("found destination OId: " + destinationRow.OID.ToString() + " with matching PM number");
                        relExists = hshRelOIds.ContainsKey(destinationRow.OID);

                        if (!relExists)
                        {
                            //Create the relationship and increment the relationship count 
                            relationship = relationshipClass.CreateRelationship(originRow as IObject, destinationRow as IObject);
                            relCreateCount++;
                        }
                        else
                            Debug.Print("relationship already exists");

                    }
                    if (cursor != null)
                    {
                        Marshal.ReleaseComObject(cursor);
                        cursor = null;
                    }
                    if (destinationRow != null)
                    {
                        Marshal.ReleaseComObject(destinationRow);
                        destinationRow = null;
                    }
                    if (destinationRow != null)
                    {
                        Marshal.ReleaseComObject(destinationRow);
                        destinationRow = null;
                    }
                    fieldName = string.Empty;
                }
                if (featureClass != null)
                {
                    Marshal.ReleaseComObject(featureClass);
                    featureClass = null;
                }
                //Stop the edit session
                workspaceEdit.StopEditOperation();

                //Provide appropriate user feedback 
                string userMsg = "";
                if (relCreateCount > 0)
                    userMsg = relCreateCount.ToString() + " relationships created successfully!";
                else
                    userMsg = "No additional relationships created!";
                MessageBox.Show(
                    userMsg, "PGE Customer Agreement Relate",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                //Abort the edit operation if necessary 
                if (hasEditOperation)
                    workspaceEdit.AbortEditOperation();
                _logger.Error("Error in RelateMLXNumbertoPMNumber: " + ex.Message);
            }
        }

        /// <summary>
        /// Return customer agreement number of related field.
        /// </summary>
        /// <param name="featureClass">Feature class for the record.</param>
        /// <param name="destinationRow">Row which has existing guid</param>
        /// <returns>Customer Agreement number.</returns>
        private string GetAgreementNumber(IFeatureClass featureClass, IRow destinationRow, out string objectId)
        {
            try
            {
                //Get Destination relationship class
                IEnumRelationshipClass enumRelationshipClass = featureClass.get_RelationshipClasses(esriRelRole.esriRelRoleDestination);
                IRelationshipClass relationshipClass = null;
                while ((relationshipClass = enumRelationshipClass.Next()) != null)
                {
                    if (relationshipClass.OriginClass.AliasName == "Customer Agreement")
                    {
                        break; // We got the desire class. so we can proceed for next.
                    }
                }
                //Get the row from ISet the retrieve the related customer agreement number.
                ESRI.ArcGIS.esriSystem.ISet set = relationshipClass.GetObjectsRelatedToObject(destinationRow as IObject);
                set.Reset();
                IRow rowRecord = set.Next() as IRow;
                if (rowRecord != null)
                {
                    objectId = rowRecord.get_Value(rowRecord.Fields.FindField("OBJECTID")).ToString();
                    return rowRecord.get_Value(rowRecord.Fields.FindField("AGREEMENTNUM")).ToString();
                }
                else
                {
                    objectId = string.Empty;
                    return string.Empty;
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Error : " + ex);
                objectId = string.Empty;
                return string.Empty;
            }
        }


        /// <summary>
        /// This method will return current selected workspace and mlx number related table.
        /// </summary>
        /// <returns></returns>
        private IWorkspace GetWorkspace()
        {
            IWorkspace workSpace = null;
            try
            {
                var mapLayers = GetAllLayersFromMap();
                if (mapLayers.Count > 0)
                {
                    workSpace = ((IDataset)mapLayers[0]).Workspace;
                }
                return workSpace;
            }
            catch (Exception ex)
            {
                _logger.Error("Error : " + ex);
                return workSpace;
            }
        }

        /// <summary>
        /// Return all the selected layers from the map.
        /// </summary>
        /// <returns></returns>
        private List<ILayer> GetAllLayersFromMap()
        {
            var mapLayers = new List<ILayer>();
            IMxDocument mxDoc = PGE.Common.Delivery.ArcFM.ApplicationFacade.Application.Document as IMxDocument;
            CreateLayerList(null, mxDoc.FocusMap, mxDoc.FocusMap.LayerCount, mapLayers);
            return mapLayers;
        }

        /// <summary>
        /// Create the layer list from current map.
        /// </summary>
        /// <param name="compositeLayer"></param>
        /// <param name="map"></param>
        /// <param name="mapCount"></param>
        /// <param name="mapLayers"></param>
        private void CreateLayerList(ICompositeLayer compositeLayer, IMap map, int mapCount, List<ILayer> mapLayers)
        {
            try
            {
                for (int i = 0; i < mapCount; i++)
                {
                    ILayer layer;
                    if (map != null && compositeLayer == null)
                        layer = map.Layer[i];
                    else if (compositeLayer != null)
                        layer = compositeLayer.Layer[i];
                    else
                        return;
                    ICompositeLayer comLayer;
                    if (((comLayer = layer as ICompositeLayer) != null) && ((layer as IGroupLayer) != null))
                        CreateLayerList(comLayer, null, comLayer.Count, mapLayers);
                    else
                        mapLayers.Add(layer);
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Error : " + ex);
            }
        }

        /// <summary>
        /// Get the Feature Table from current workspace and there class model name.
        /// </summary>
        /// <param name="workspace">Currently selected workspace.</param>
        /// <param name="classModelName">Class model name of which feature table related.</param>
        /// <returns>Enum of related feature classes.</returns>
        //private IEnumFeatureClass GetFeatureTable(IWorkspace workspace, string classModelName)
        //{
        //    try
        //    {
        //        IMMModelNameManager mnm = ModelNameManager.Instance;
        //        IEnumFeatureClass enumTable = mnm.FeatureClassesFromModelNameWS(workspace, classModelName);
        //        enumTable.Reset();
        //        return enumTable;
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.Error("Error : " + ex);
        //        return null;
        //    }
        //}

        /// <summary>
        /// Get the Feature Table from current workspace and there class model name.
        /// </summary>
        /// <param name="workspace">Currently selected workspace.</param>
        /// <param name="classModelName">Class model name of which feature table related.</param>
        /// <returns>Enum of related feature classes.</returns>
        private IEnumFeatureClass GetCustAgreementRelatedTables( 
            IWorkspace workspace, 
            string customerAgreementType)
        {
            try
            {
                string customerAgreementModelName = string.Empty;
                switch (customerAgreementType)
                {
                    case "MLX":
                        {
                            customerAgreementModelName = SchemaInfo.Electric.ClassModelNames.MLXRelateField; 
                            break; 
                        }
                    case "AW":
                        {
                            customerAgreementModelName = SchemaInfo.Electric.ClassModelNames.AWRelateField;
                            break; 
                        }
                    case "TF":
                        {
                            customerAgreementModelName = SchemaInfo.Electric.ClassModelNames.TFRelateField;
                            break;
                        }
                    case "SF":
                        {
                            customerAgreementModelName = SchemaInfo.Electric.ClassModelNames.SFRelateField;
                            break;
                        }
                    case "RPA":
                        {
                            customerAgreementModelName = SchemaInfo.Electric.ClassModelNames.RPARelateField;
                            break;
                        }
                    default:
                        {
                            break;
                        }
                }

                IMMModelNameManager mnm = ModelNameManager.Instance;
                IEnumFeatureClass enumTable = mnm.FeatureClassesFromModelNameWS(workspace, customerAgreementModelName);
                enumTable.Reset();
                return enumTable;
            }
            catch (Exception ex)
            {
                _logger.Error("Error : " + ex);
                return null;
            }
        }

        #endregion
    }

    #region Internal class from Model Name Manager

    /// <summary>
    /// Class for accessing model name manager instance.
    /// </summary>
    internal static class ModelNameManager
    {
        private static IMMModelNameManager _manager;

        /// <summary>
        /// Get the IMMModelNameManager instance from program id.
        /// </summary>
        [System.ComponentModel.Browsable(false)]
        public static IMMModelNameManager Instance
        {
            get
            {
                if (_manager != null)
                {
                    return _manager;
                }
                Type modelNameManagerType = Type.GetTypeFromProgID("mmGeodatabase.mmModelNameManager");
                return (IMMModelNameManager)Activator.CreateInstance(modelNameManagerType);
            }
        }
    }

    #endregion
}
