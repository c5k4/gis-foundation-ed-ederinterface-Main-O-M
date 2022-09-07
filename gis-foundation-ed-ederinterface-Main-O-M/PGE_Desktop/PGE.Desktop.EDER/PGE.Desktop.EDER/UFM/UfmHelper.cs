using System;
using System.Collections.Generic;
using ESRI.ArcGIS.ArcMapUI;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Framework;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using PGE.Common.Delivery.Framework;
using Miner.Interop;
using System.Runtime.InteropServices;
using PGE.Common.Delivery.Diagnostics;
using System.Reflection;
using PGE.Common.Delivery.Geodatabase;

namespace PGE.Desktop.EDER.UFM
{
    public static class UfmHelper
    {
        #region Member vars

        // For error handling
        private static readonly Log4NetLogger Logger = new Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "EDERDesktop.log4net.config");

        #endregion

        #region Public methods

        /// <summary>
        /// Returns the duct bank configuration for the supplied conduit feature class
        /// </summary>
        /// <param name="conduit"></param>
        /// <returns></returns>
        public static IMMDuctBankConfig GetDuctBankConfig(IFeature conduit)
        {
            IMMDuctBankConfig dbc = null;

            try
            {
                // Get the blob field index
                int dbcFieldIndex = ModelNameFacade.FieldIndexFromModelName(conduit.Class,
                    SchemaInfo.UFM.FieldModelNames.DuctBankConfig);
                if (dbcFieldIndex > 0)
                {
                    // Get the blob value
                    object value = conduit.Value[dbcFieldIndex];

                    // If its populated
                    if (value != null && !Convert.IsDBNull(value))
                    {
                        // Load it into the DuctBankConfig structure
                        dbc = new MMDuctBankConfigClass();
                        IMMPersistentListItem persist = (IMMPersistentListItem) dbc;
                        persist.LoadFromField(conduit, dbcFieldIndex);
                    }
                }
            }
            catch (Exception ex)
            {
                // Log and return null
                Logger.Error("Failed to load duct bank configuration: " + ex);
                dbc = null;
            }

            // Return the result
            return dbc;
        }

        /// <summary>
        /// Returns a feature cursor listing the ducts within the supplied duct bank feature
        /// </summary>
        /// <param name="ductBank"></param>
        /// <returns></returns>
        public static IFeatureCursor GetDucts(IFeature ductBank)
        {
            IFeatureCursor featureCursor = null;

            try
            {
                // Get the duct FC
                IWorkspace ws = ((IDataset) ductBank.Class).Workspace;
                IFeatureClass ductFc = ModelNameFacade.FeatureClassByModelName(ws,
                    SchemaInfo.UFM.ClassModelNames.UfmDuct);

                if (ductFc != null)
                {
                    // Create the spatial filter - we want all ducts within the duct bank
                    ISpatialFilter spatialFilter = new SpatialFilterClass();
                    spatialFilter.Geometry = ductBank.Shape;
                    spatialFilter.GeometryField = ductFc.ShapeFieldName;
                    spatialFilter.SpatialRel = esriSpatialRelEnum.esriSpatialRelContains;

                    // Execute the query.
                    featureCursor = ductFc.Search(spatialFilter, true);
                }
            }
            catch (Exception ex)
            {
                Logger.Warn("Failed to get ducts from duct bank: " + ex);
                throw;
            }

            // Return the result
            return featureCursor;
        }

        /// <summary>
        /// Gets the UFM floor feature for the vault floor the supplied duct bank feature is in.
        /// Null will be returned if the floor feature cannot be determined.
        /// </summary>
        /// <param name="ductBank"></param>
        /// <returns></returns>
        public static IFeature GetUfmFloor(IFeature ductBank)
        {
            // Get the wall feature that the 
            IFeature floor = null;

            try
            {
                // Get the facility ID
                object facId = GetFieldValue(ductBank, SchemaInfo.UFM.FieldModelNames.FacilityId);

                // If it had one...
                if (facId != null)
                {
                    // Get the feature class for the UFM floor
                    IWorkspace ws = ((IDataset) ductBank.Class).Workspace;
                    IFeatureClass fcFloor = ModelNameFacade.FeatureClassByModelName(ws,
                        SchemaInfo.UFM.ClassModelNames.UfmFloor);

                    // Build a query using the facility ID
                    IQueryFilter qf = new QueryFilterClass();
                    qf.WhereClause = "FACILITYID='" + facId + "'";

                    // Execute the query and return the first feature found
                    IFeatureCursor cursorFloor = fcFloor.Search(qf, false);
                    floor = cursorFloor.NextFeature();
                }
            }
            catch (Exception ex)
            {
                Logger.Warn("Failed to get floor for duct bank: " + ex);
                throw;
            }

            // Return the result
            return floor;
        }

        public static object GetFieldValue(IRow pRow, string modelName)
        {
            int fieldIndex = ModelNameFacade.FieldIndexFromModelName(((IObject) pRow).Class, modelName);
            object value = null;
            if (fieldIndex != -1)
            {
                value = pRow.Value[fieldIndex];
                if (Convert.IsDBNull(value))
                {
                    value = null;
                }
            }

            return value;
        }

        /// <summary>
        /// Returns an ISet of objects related to the supplied pRow of the class identified by the 
        /// relatedObjectModelName model name.
        /// </summary>
        /// <param name="pRow"></param>
        /// <param name="relatedObjectModelName"></param>
        /// <returns></returns>
        public static ISet GetRelatedObjects(IRow pRow, string relatedObjectModelName)
        {
            ISet relatedObjects = null;

            // Get the relationship class
            IRelationshipClass rc = ModelNameFacade.RelationshipClassFromModelName(((IObject) pRow).Class,
                esriRelRole.esriRelRoleAny, relatedObjectModelName);

            // If we found it, get the related objects
            if (rc != null)
            {
                relatedObjects = rc.GetObjectsRelatedToObject(pRow as IObject);
            }

            // Return the result
            return relatedObjects;
        }

        #endregion

        public static double GetAngle(IPoint p1, IPoint p2)
        {
            double x1 = p1.X;
            double y1 = p1.Y;
            
            double x2 = p2.X;
            double y2 = p2.Y;

            // Determine the angle
            double xDiff = x2 - x1;
            double yDiff = y2 - y1;
            double angle = Math.Atan2(yDiff, xDiff)*180.0/Math.PI;

            return angle;
        }

        public static IElement CloneAnnotationElement(IObject pObject, int refScale)
        {
            IElement clonedElement = null;

            try
            {
                // Get the existing annotation geometry
                IAnnotationFeature2 originalAnnoFeat = (IAnnotationFeature2) pObject;
                clonedElement = originalAnnoFeat.Annotation;

                // Change its scale
                IElementProperties3 elementProps = (IElementProperties3) clonedElement;
                elementProps.AutoTransform = true;
                elementProps.ReferenceScale = refScale;
            }
            catch (Exception ex)
            {
                // Log a warning
                Logger.Warn(ex.Message);
            }

            // Return the result
            return clonedElement;
        }

        public static IFeature GetParentFeature(IObject pObject)
        {
            IFeature feature = null;
            ISet relatedObjects = null;
            IRelationshipClass rc = ModelNameFacade.RelationshipClassFromModelName(pObject.Class,
                esriRelRole.esriRelRoleAny, SchemaInfo.UFM.ClassModelNames.Conduit);
            if (rc != null)
            {
                relatedObjects = rc.GetObjectsRelatedToObject(pObject);
            }

            if (relatedObjects != null)
            {
                relatedObjects.Reset();
                feature = relatedObjects.Next() as IFeature;
            }

            return feature;
        }

        public static bool IsArrow(IObject pObject)
        {
            bool isArrow = true;

            IElement annoElement = ((IAnnotationFeature2) pObject).Annotation;
            if (annoElement is IGroupElement)
            {
                isArrow = false;
            }

            return isArrow;
        }

        /// <summary>
        /// Returns the layer for the supplied feature class
        /// </summary>
        /// <param name="map"></param>
        /// <param name="featureClass"></param>
        /// <returns></returns>
        public static IFeatureLayer GetFeatureLayer(IMap map, IFeatureClass featureClass)
        {
            // Log entry
            string name = MethodBase.GetCurrentMethod().Name;
            Logger.Debug("Entered " + name);

            IFeatureLayer featLayer = null;
            IEnumLayer enumLayer = null;

            try
            {
                // Get all layers in the map
                UID uid = new UID {Value = "{40A9E885-5533-11d0-98BE-00805F7CED21}"};
                enumLayer = map.Layers[uid];

                // For each layer
                ILayer layer;
                while ((layer = enumLayer.Next()) != null)
                {
                    // Check to see if the layer is for our feature class
                    featLayer = layer as IFeatureLayer;
                    if (featLayer != null)
                    {
                        if (((IDataset) featLayer.FeatureClass).Name == ((IDataset) featureClass).Name)
                        {
                            // If it is, save it and bail out
                            featLayer = layer as IFeatureLayer;
                            break;
                        }
                    }
                }
            }
            finally
            {
                // Release object
                if (enumLayer != null)
                {
                    Marshal.ReleaseComObject(enumLayer);
                }
            }

            return featLayer;
        }

        /// <summary>
        /// Returns a workspace
        /// </summary>
        /// <returns></returns>
        public static IWorkspace GetWorkspace()
        {
            // Log entry
            string name = MethodBase.GetCurrentMethod().Name;
            Logger.Debug("Entered " + name);

            // Get and return the logged in workspace
            IMMLoginUtils utils = new MMLoginUtils();
            return utils.LoginWorkspace;
        }

        public static void RefreshView()
        {
            try
            {
                IMxDocument doc = GetCurrentMxDocument();
                if (doc != null)
                {
                    ((IActiveView)doc.FocusMap).Refresh();
                }
            }
            catch (Exception ex)
            {
                // Not the end of the world, log and move on
                Logger.Warn("Failed to refresh view: " + ex);
            }
        }

        public static IMxDocument GetCurrentMxDocument()
        {
            Type appRefType = Type.GetTypeFromProgID("esriFramework.AppRef");
            object appRefObj = Activator.CreateInstance(appRefType);
            IApplication arcMapApp = appRefObj as IApplication;
            if (arcMapApp != null)
            {
                return (IMxDocument)arcMapApp.Document;
            }

            return null;
        }

        public static IMxDocument GetEditor()
        {
            Type appRefType = Type.GetTypeFromProgID("esriFramework.AppRef");
            object appRefObj = Activator.CreateInstance(appRefType);
            IApplication arcMapApp = appRefObj as IApplication;
            if (arcMapApp != null)
            {
                return (IMxDocument)arcMapApp.Document;
            }

            return null;
        }

        /// <summary>
        /// Returns the OID of the ConduitSystem feature related to the supplied IObject
        /// </summary>
        /// <param name="pObject"></param>
        /// <returns></returns>
        public static int GetParentConduitOID(IObject pObject)
        {
            int oid = 0;

            try
            {
                // Get the value of the feature ID field which is the parent conduit
                IField conduitFeatureIdField = ModelNameFacade.FieldFromModelName(pObject.Class, SchemaInfo.UFM.FieldModelNames.FeatureId);
                object idValue = pObject.get_Value(pObject.Fields.FindFieldByAliasName(conduitFeatureIdField.AliasName));
                if (idValue != null)
                {
                    oid = (int)idValue;
                }
            }
            catch (Exception ex)
            {
                Logger.Warn("Failed to get parent conduit OID: " + ex.ToString());
            }

            return oid;
        }

        public static IFeature MatchFieldValueForFeatureList(IEnumerable<IFeature> featureList, IFeature featureToMatch,
            string fieldName)
        {
            foreach (IFeature ft in featureList)
            {
                object wallVal = ft.Value[ft.Fields.FindField(fieldName)];
                object ductVal = featureToMatch.Value[featureToMatch.Fields.FindField(fieldName)];

                if (wallVal.Equals(ductVal) && !wallVal.Equals(DBNull.Value))
                {
                    return ft;
                }
            }

            return null;
        }

        public static double GetShallowestAngle(double angle)
        {
            double absAngle = Math.Abs(angle);
            while (absAngle > 45) absAngle -= 90;

            // Fudge offset - if the angle is "just" on the -45 deg angle (within 2 degrees), then rotate the other way
            //if (absAngle > -45 && absAngle < -43) absAngle = absAngle + 90;
            return absAngle * (angle < 0 ? -1 : 1);
        }

        public static string EliminatePhase(IObject conduit, IObject conductor, string currentLabelText)
        {
            string newLabelText = currentLabelText;

            // If the feature is only related to the conduit once
            bool isSplit = IsSplit(conduit, conductor);

            // Remove its phasing from the text
            if (isSplit == true)
            {
                newLabelText = EliminatePhase(currentLabelText);
            }

            return newLabelText;
        }

        private static bool IsSplit(IObject conduit, IObject conductor)
        {
            bool split = false;

            try
            {
                // Get all the conduits related to the conductor
                ISet relatedConduits = UfmHelper.GetRelatedObjects(conductor as IRow, SchemaInfo.UFM.ClassModelNames.Conduit);
                IFeature relatedConduit = relatedConduits.Next() as IFeature;
                int relCount = 0;

                // Check each one
                while (relatedConduit != null)
                {
                    // Count how many are related to the conduit we care about
                    if (relatedConduit.OID == conduit.OID)
                    {
                        relCount++;

                        // If its more than 1, it must be split
                        if (relCount > 1)
                        {
                            split = true;
                            break;
                        }
                    }

                    relatedConduit = relatedConduits.Next() as IFeature;
                }
            }
            catch (Exception ex)
            {
                Logger.Warn("Failed to check if conductor was split: " + ex.ToString());
            }

            // Return the result
            return split;
        }

        public static string EliminateConductorInsulation(IObject crossSection, string currentLabelText)
        {            
            // Get a list of domain values (not descriptions)
            IList<string> domainVals = new List<string>();
            IWorkspaceDomains wsDomain = (crossSection.Class as IDataset).Workspace as IWorkspaceDomains;            
            IDomain domain = wsDomain.DomainByName["Conductor Insulation - UG"];
            if (domain is ICodedValueDomain)
            {
                ICodedValueDomain codedDomain = (ICodedValueDomain)domain;
                for (int i = 0; i < codedDomain.CodeCount; i++)
                {
                    domainVals.Add(codedDomain.get_Value(i).ToString());
                }
            }

            // Remove any reference to any of them
            string newLabelText = currentLabelText;
            foreach (string insulationType in domainVals)
            {
                // Colon should only be used to separate duct number and phase - this will only match on phase
                newLabelText = newLabelText.Replace(" " + insulationType + " ", " ");
                newLabelText = newLabelText.Replace(" " + insulationType + "T", "T");
                newLabelText = newLabelText.Replace(" " + insulationType + "Q", "Q");
                newLabelText = newLabelText.Replace(" " + insulationType + "\r", "\r");
                newLabelText = newLabelText.Replace(" " + insulationType + "T\r", "T\r");
                newLabelText = newLabelText.Replace(" " + insulationType + "Q\r", "Q\r");
                if (newLabelText.EndsWith(" " + insulationType) == true)
                {
                    newLabelText = newLabelText.Substring(0, newLabelText.Length - (insulationType.Length + 1));
                }
            }

            // Return the result
            return newLabelText;
        }

        public static string EliminatePhase(string currentLabelText)
        {
            string newLabelText = currentLabelText;

            // Colon should only be used to separate duct number and phase - this will only match on phase
            newLabelText = newLabelText.Replace("ABC: ", string.Empty);
            newLabelText = newLabelText.Replace("AB: ", string.Empty);
            newLabelText = newLabelText.Replace("AC: ", string.Empty);
            newLabelText = newLabelText.Replace("BC: ", string.Empty);
            newLabelText = newLabelText.Replace("A: ", string.Empty);
            newLabelText = newLabelText.Replace("B: ", string.Empty);
            newLabelText = newLabelText.Replace("C: ", string.Empty);

            // Return the result
            return newLabelText;
        }

        public static IDictionary<string, IList<int>> GetSplitDucts(IFeature conduit)
        {
            //IDictionary<string, string> cables = new Dictionary<string, string>();
            IDictionary<int, string> conductors = new Dictionary<int, string>();
            IDictionary<string, IList<int>> splitDucts = new Dictionary<string, IList<int>>();
            IMMDuctBankConfig dbc = UfmHelper.GetDuctBankConfig(conduit);

            ID8List dbcList = (ID8List)dbc;
            dbcList.Reset();
            ID8List ductDefObjs = (ID8List)dbcList.Next(false);
            ID8List ductDefinitionObj;

            for (ductDefObjs.Reset(), ductDefinitionObj = (ID8List)ductDefObjs.Next(); ductDefinitionObj != null;
                ductDefinitionObj = (ID8List)ductDefObjs.Next())
            {
                if (ductDefinitionObj is IMMDuctDefinition)
                {
                    IMMDuctDefinition ductDef = (IMMDuctDefinition)ductDefinitionObj;
                    ID8ListItem ductDefId8Item;

                    for (ductDefinitionObj.Reset(), ductDefId8Item = ductDefinitionObj.Next(); ductDefId8Item != null;
                        ductDefId8Item = ductDefinitionObj.Next())
                    {
                        if (ductDefId8Item is IMMDuctPhase)
                        {
                            IMMDuctPhase ductPhase = (IMMDuctPhase)ductDefId8Item;

                            if (conductors.ContainsKey(ductPhase.cableID) == true)
                            {
                                IList<int> splitConductorsInDuct;
                                if (splitDucts.ContainsKey(ductDef.ductID) == true)
                                {
                                    if (splitDucts[ductDef.ductID].Contains(ductPhase.cableID) == false)
                                    {
                                        splitDucts[ductDef.ductID].Add(ductPhase.cableID);
                                    }
                                }
                                else
                                {
                                    splitConductorsInDuct = new List<int>();
                                    splitConductorsInDuct.Add(ductPhase.cableID);
                                    splitDucts.Add(ductDef.ductID, splitConductorsInDuct);
                                }

                                string ductNoConductorWasFirstFoundIn = conductors[ductPhase.cableID];
                                if (splitDucts.ContainsKey(ductNoConductorWasFirstFoundIn) == false)
                                {
                                    splitConductorsInDuct = new List<int>();
                                    splitConductorsInDuct.Add(ductPhase.cableID);
                                    splitDucts.Add(ductNoConductorWasFirstFoundIn, splitConductorsInDuct);
                                }
                            }
                            else
                            {
                                // What if it isn't split but has neutrals
                                conductors.Add(ductPhase.cableID, ductDef.ductID);
                            }
                        }
                    }
                }
            }

            return splitDucts;
        }



    }
}
