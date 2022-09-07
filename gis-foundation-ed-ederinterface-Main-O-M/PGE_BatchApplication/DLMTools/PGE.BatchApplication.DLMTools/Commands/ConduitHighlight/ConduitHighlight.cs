using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using ESRI.ArcGIS.ArcMapUI;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Framework;
using ESRI.ArcGIS.Geodatabase;
using Miner.Geodatabase;
using Miner.Interop;

namespace PGE.BatchApplication.DLMTools.Commands.ConduitHighlight
{
    public class ConduitHighlight
    {
        #region Private Varibales
        /// <summary>
        /// Currently running ArcMap instance
        /// </summary>
        private IApplication _application;

        private IMap _map;
        IMMModelNameManager _mnMgr;


        /// <summary>
        /// Logger to log error/debug messages
        /// </summary>
        //private static readonly Log4NetLogger _logger = new Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        
        #endregion Private Varibales

        #region Constructor
        /// <summary>
        /// Creates an instance of <see cref="PGEPrintMapCommand"/>
        /// </summary>
        public ConduitHighlight(IApplication app)
        {
            _application = app;
        }
        #endregion Constructor

        #region Overridden Class Methods

        public void Highlight()
        {
            try
            {
                IMxDocument mxDocument = _application.Document as IMxDocument;
                _map = mxDocument.FocusMap;
                _mnMgr = ModelNameManager.Instance;

                // Get the conduit feature class
                IList<IFeatureLayer> layers = FeatureLayersWithModelName(_map);
                //IFeatureLayer layer = layers[0] as IFeatureLayer;
                IWorkspace ws = ((IDataset) layers[0]).Workspace;
                IFeatureClass conduitFc = FeatureClassByModelName(ws, "ULS");
                IList<IFeatureLayer> conduitLayers = FeatureLayerByModelName("ULS", _map, false); // UfmHelper.GetFeatureLayer(_map, conduitFc);
                IFeatureLayer conduitLayer = conduitLayers[0];

                // Get the selected conduit
                ISelectionSet selectedConduit = ((IFeatureSelection) conduitLayer).SelectionSet;

                if (selectedConduit.Count == 0)
                {
                    // Reset def queries
                    ResetDefQuery(ws, "PGE_PRIMARYUGCONDUCTOR");
                    ResetDefQuery(ws, "PGE_SECONDARYUGCONDUCTOR");
                    ResetDefQuery(ws, "PGE_DCCONDUCTOR");
                    ResetDefQuery(ws, "PGE_NEUTRALCONDUCTOR");

                    ((IActiveView) _map).Refresh();
                }
                else if (selectedConduit.Count == 1)
                {
                    IEnumIDs conduitIds = selectedConduit.IDs;
                    int conduitId = conduitIds.Next();
                    IFeature conduit = conduitFc.GetFeature(conduitId);

                    // Get the related conductors
                    ISet primarys = GetRelatedObjects(conduit, "PGE_PRIMARYUGCONDUCTOR");
                    ISet secondarys = GetRelatedObjects(conduit, "PGE_SECONDARYUGCONDUCTOR");
                    ISet dcs = GetRelatedObjects(conduit, "PGE_DCCONDUCTOR");
                    ISet neutrals = GetRelatedObjects(conduit, "PGE_NEUTRALCONDUCTOR");

                    // Find the layers and filter
                    UpdateDefQuery(ws, primarys, "PGE_PRIMARYUGCONDUCTOR");
                    UpdateDefQuery(ws, secondarys, "PGE_SECONDARYUGCONDUCTOR");
                    UpdateDefQuery(ws, dcs, "PGE_DCCONDUCTOR");
                    UpdateDefQuery(ws, neutrals, "PGE_NEUTRALCONDUCTOR");
                    ((IActiveView) _map).Refresh();
                }
                else
                {
                    System.Windows.Forms.MessageBox.Show("More than 1 ConduitSystem selected");
                }
            }
            catch (Exception ex)
            {
                //_logger.Error(ex.Message, ex);
            }
        }

        private void ResetDefQuery(IWorkspace ws, string modelName)
        {
            // Get all the layers with that modelname
            IList<IFeatureLayer> layers = FeatureLayerByModelName(modelName, _map, true);
            
            // Clear the definition query on each one
            foreach (ILayer layer in layers)
            {
                IFeatureLayerDefinition layerDef = layer as IFeatureLayerDefinition;
                layerDef.DefinitionExpression = "";
            }
        }

        private void UpdateDefQuery(IWorkspace ws, ISet features, string modelName)
        {
            // Build the definition query
            string defQuery = "";
            features.Reset();
            object featureObj = features.Next();

            // OR any features together
            while (featureObj != null)
            {
                IFeature feature = featureObj as IFeature;
                string id = feature.OID.ToString();
                if (defQuery != "")
                {
                    defQuery += " OR ";
                }
                defQuery += GetOidString(feature.Class) + "=" + id;
                featureObj = features.Next();
            }

            // If nothing, we want to turn them all off - use a bogus ID
            if (defQuery == string.Empty)
            {
                defQuery = "1=2";
            }

            // Get all the layers with that modelname
            IList<IFeatureLayer> layers = FeatureLayerByModelName(modelName, _map, true);

            // Clear the definition query on each one
            foreach (ILayer layer in layers)
            {
                IFeatureLayerDefinition layerDef = layer as IFeatureLayerDefinition;
                layerDef.DefinitionExpression = defQuery;
            }
        }

        #endregion Overridden Class Methods

        #region Support methods

        private String GetOidString(IObjectClass oc)
        {
            return oc.OIDFieldName;
        }

        public virtual List<IFeatureLayer> FeatureLayersWithModelName(IMap map)
        {
            List<IFeatureLayer> items = new List<IFeatureLayer>();
            try
            {
                if (map == null)
                    return items;

                if (map.LayerCount == 0) return null;

                UID uid = new UIDClass();
                uid.Value = "{40A9E885-5533-11d0-98BE-00805F7CED21}";
                IEnumLayer enumerator = map.get_Layers(uid, true);
                ILayer layer;

                while ((layer = enumerator.Next()) != null)
                {
                    if (layer is IFeatureLayer && layer.Valid)
                    {
                        IFeatureLayer layerFilter = ((IFeatureLayer)layer);
                        items.Add(layerFilter);
                    }
                }
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
            }
            return items;
        }

        private ISet GetRelatedObjects(IRow pRow, string relatedObjectModelName)
        {
            ISet relatedObjects = null;

            // Get the relationship class
            IRelationshipClass rc = RelationshipClassFromModelName(((IObject)pRow).Class,
                esriRelRole.esriRelRoleAny, relatedObjectModelName);

            // If we found it, get the related objects
            if (rc != null)
            {
                relatedObjects = rc.GetObjectsRelatedToObject(pRow as IObject);
            }

            // Return the result
            return relatedObjects;
        }

        public IFeatureClass FeatureClassByModelName(IWorkspace ws, string modelName)
        {
            try
            {
                IEnumFeatureClass pEnumFC = ModelNameManager.Instance.FeatureClassesFromModelNameWS(ws, modelName);
                pEnumFC.Reset();
                return pEnumFC.Next();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        public List<IFeatureLayer> FeatureLayerByModelName(string featureClassModelName, IMap map, bool all)
        {
            List<IFeatureLayer> flList = new List<IFeatureLayer>();
            try
            {
                UID uid = new UIDClass();
                uid.Value = "{40A9E885-5533-11d0-98BE-00805F7CED21}";
                IEnumLayer enumLayer = map.get_Layers(uid, true);
                enumLayer.Reset();
                IFeatureLayer featLayer = null;
                while ((featLayer = (IFeatureLayer)enumLayer.Next()) != null)
                {
                    if (featLayer.FeatureClass == null) continue;
                    if (ModelNameManager.Instance.ContainsClassModelName(featLayer.FeatureClass, featureClassModelName))
                    {
                        flList.Add(featLayer);
                        if (!all)
                        {
                            return flList;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
            return flList;
        }
        
        public IRelationshipClass RelationshipClassFromModelName(IObjectClass oclass, esriRelRole relRole, string modelNames)
        {
            var enumClasses = oclass.RelationshipClasses[relRole];
            try
            {
                enumClasses.Reset();

                IRelationshipClass relClass;
                while ((relClass = enumClasses.Next()) != null)
                {
                    switch (relRole)
                    {
                        case esriRelRole.esriRelRoleAny:
                            if (_mnMgr.ContainsClassModelName(relClass.DestinationClass, modelNames) || _mnMgr.ContainsClassModelName(relClass.OriginClass, modelNames))
                                return relClass;

                            break;
                        case esriRelRole.esriRelRoleDestination:
                            if (_mnMgr.ContainsClassModelName(relClass.OriginClass, modelNames))
                                return relClass;

                            break;
                        case esriRelRole.esriRelRoleOrigin:
                            if (_mnMgr.ContainsClassModelName(relClass.DestinationClass, modelNames))
                                return relClass;

                            break;
                    }
                }
            }
            finally
            {
                while (Marshal.ReleaseComObject(enumClasses) > 0) { }
            }

            throw new NullReferenceException("Unable to obtain the object class with the model name: " + modelNames + " via the relationship to the " + oclass.AliasName + " class with a role of " + relRole + ".");
        }

        #endregion
    }
}
