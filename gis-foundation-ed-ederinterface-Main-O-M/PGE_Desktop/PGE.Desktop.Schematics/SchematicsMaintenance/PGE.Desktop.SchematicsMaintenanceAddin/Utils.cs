// Copyright 2013 ESRI
// 
// All rights reserved under the copyright laws of the United States
// and applicable international laws, treaties, and conventions.
// 
// You may freely redistribute and use this sample code, with or
// without modification, provided you include the original copyright
// notice and use restrictions.
// 
// See the use restrictions at http://resources.arcgis.com/en/help/arcobjects-net/conceptualhelp/index.html#/Copyright_information/00010000009s000000/
//

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.Schematic;
using ESRI.ArcGIS.SchematicUI;
using ESRI.ArcGIS.esriSystem;
using PGE.Desktop.SchematicsMaintenance.Core;

namespace PGE.Desktop.SchematicsMaintenance
{
    internal static class Utils
    {
        public const string cstJobFolderName = "CircuitMap_Edit";
        public const string cstSDE = "SDE";
        public const string cstPrefix_ChildDiagramTemplate = "";
        public const string cstSuffix_ChildDiagramTemplate = "_Edit";

        #region Private Fields

        private static Dictionary<int, string> m_dicEltIdTableName;
        private static string m_sGdbItemsTableName = "";
        public static ISchematicDiagramClass schDiagramClass = null;
        //(V3SF) EDGISREARC-375  : Stores Active Editing Session ID
        public static string SessionID = "";
        //(V3SF) EDGISREARC-813  : Schematic Customer Feedback
        public static string ObjectID = "";
        #endregion


        internal static SchematicExtension GetSchematicExtension()
        {
            IExtension extension = null;
            IExtensionManager extensionManager;

            extensionManager = (IExtensionManager) ArcMap.Application;

            extension = extensionManager.FindExtension("Esri Schematic Extension");
            if (extension != null)
                return extension as SchematicExtension;

            return null;
        }


        internal static string BuildString(IList<string> listString, string Quote = "")
        {
            var sb = new StringBuilder();

            for (int i = 0; i < listString.Count; i++)
            {
              
                if (i > 0)
                    sb.Append(",");
                sb.Append(String.Format("{0}{1}{2}", Quote, listString[i], Quote));
            }
            return sb.ToString();
        }

        internal static string BuildInClause(IList<string> listString, string fieldName, string Quote = "")
        {
            var sb = new StringBuilder();
            sb.AppendFormat("{0} IN (", fieldName);
            for (int i = 0; i < listString.Count; i++)
            {
                if (i > 0)
                {
                    //get around oracle 1000 entry IN statement limit
                    if (i%1000 == 0)
                    {
                        sb.AppendFormat(") OR {0} IN (", fieldName);
                    }
                    else
                    {
                        sb.Append(",");
                    }
                }

                sb.Append(String.Format("{0}{1}{2}", Quote, listString[i], Quote));
            }
            sb.Append(")");
            return sb.ToString();
        }

        internal static string BuildString(IList<int> listInt)
        {
            var sb = new StringBuilder();

            for (int i = 0; i < listInt.Count; i++)
            {
                if (i > 0)
                    sb.Append(",");
                sb.Append(String.Format("{0}", listInt[i].ToString()));
            }
            return sb.ToString();
        }

        internal static IMap GetGeographicalMap(bool WithSelectedFeature = false)
        {
            IMap pMap = null;
            var dicMap = new Dictionary<string, int>();
            IMaps pMaps = ArcMap.Document.Maps;
            int iMapSelected = -1;

            for (int i = 0; i <= pMaps.Count - 1; i++)
            {
                pMap = pMaps.get_Item(i);

                IEnumLayer enuLayer = pMap.get_Layers(null, true);

                enuLayer.Reset();
                ILayer pLayer = enuLayer.Next();

                ISchematicDiagramClassLayer schDiagLayer = null;
                ISchematicLayer schLayer = null;
                while (pLayer != null)
                {
                    schLayer = pLayer as ISchematicLayer;
                    if (schLayer != null)
                        break;

                    schDiagLayer = pLayer as ISchematicDiagramClassLayer;
                    if (schDiagLayer != null)
                        break;

                    pLayer = enuLayer.Next();
                }
                if (schLayer == null && schDiagLayer == null)
                {
                    if (WithSelectedFeature && pMap.SelectionCount > 0)
                        return pMap;

                    dicMap.Add(pMap.Name, i);
                    iMapSelected = i;
                }
            }

            if (WithSelectedFeature) return null;
            if (dicMap.Count == 0) return null;

            if (dicMap.Count > 1)
            {
                var loForm = new FrmSelectMap();
                loForm.InitList(dicMap);

                if (loForm.ShowDialog() == DialogResult.Cancel) return null;

                string sMapName = loForm.GetMapName();
                dicMap.TryGetValue(sMapName, out iMapSelected);
            }

            return pMaps.Item[iMapSelected];
        }

        internal static IMap GetSchematicMap(bool WithSelectedFeature = false)
        {
            IMap pMap = null;
            var dicMap = new Dictionary<string, int>();
            IMaps pMaps = ArcMap.Document.Maps;
            int iMapSelected = 0;
            for (int i = 0; i <= pMaps.Count - 1; i++)
            {
                pMap = pMaps.get_Item(i);

                IEnumLayer enuLayer = pMap.get_Layers(null, true);

                enuLayer.Reset();
                ILayer pLayer = enuLayer.Next();

                ISchematicDiagramClassLayer schDiagLayer = null;
                while (pLayer != null)
                {
                    schDiagLayer = pLayer as ISchematicDiagramClassLayer;
                    if (schDiagLayer != null)
                        break;

                    pLayer = enuLayer.Next();
                }
                if (schDiagLayer != null)
                {
                    if (WithSelectedFeature && pMap.SelectionCount > 0)
                        return pMap;

                    iMapSelected = i;
                    dicMap.Add(pMap.Name, i);
                }
            }

            if (WithSelectedFeature)
                return null;

            if (dicMap.Count == 0) return null;

            if (dicMap.Count > 1)
            {
                var loForm = new FrmSelectMap();
                loForm.InitList(dicMap);
                if (loForm.ShowDialog() == DialogResult.Cancel) return null;

                string sMapName = loForm.GetMapName();
                dicMap.TryGetValue(sMapName, out iMapSelected);
            }

            return pMaps.get_Item(iMapSelected);
        }

        internal static bool IsSchematicMap(IMap esrMap, bool WithSelection)
        {
            if (esrMap == null) return false;

            if (WithSelection && esrMap.SelectionCount == 0) return false;

            //if (esrMap.SelectionCount > 0)
            //{
                IEnumLayer enuLayer = esrMap.get_Layers(null, true);

                enuLayer.Reset();
                ILayer pLayer = enuLayer.Next();

                ISchematicDiagramClassLayer schDiagLayer = null;
                while (pLayer != null)
                {
                    schDiagLayer = pLayer as ISchematicDiagramClassLayer;
                    if (schDiagLayer != null)
                        return true;

                    pLayer = enuLayer.Next();
                }
            //}

            return false;
        }

        internal static void SelectFeaturesInMap(IMap esrMap, IFeatureLayer esrFeatLayer, List<int> listIds)
        {
            if (esrFeatLayer == null || esrFeatLayer.FeatureClass == null)
                return;
            
            var tabIds = new int[listIds.Count];
            listIds.CopyTo(tabIds);

            IFeatureCursor ipFCursor = esrFeatLayer.FeatureClass.GetFeatures(tabIds, false);
            if (ipFCursor == null) return;

            IFeature ipFeature = ipFCursor.NextFeature();
            while (ipFeature != null)
            {
                esrMap.SelectFeature(esrFeatLayer, ipFeature);
                ipFeature = ipFCursor.NextFeature();
            }
        }

        internal static void SelectFromSchematic(IMap SchematicMap, IMap GeographicalMap, bool WithMasterId,
                                                 ref List<int> listMasterIds, ref ISchematicDiagramClass schDiagramClass)
        {
            GeographicalMap.ClearSelection();
            ((IActiveView) GeographicalMap).Extent = ((IActiveView) SchematicMap).Extent;
            ((IActiveView)GeographicalMap).Refresh();
            GeographicalMap.DelayEvents(true);
            int lastUCID = -1;
            List<int> listOid = null;

            var dicOidByFcid = new Dictionary<int, List<int>>();

            var enumFeature = (IEnumFeature) SchematicMap.FeatureSelection;
            ((IEnumFeatureSetup) enumFeature).AllFields = true;
            enumFeature.Reset();

            IFeature esrFeature = enumFeature.Next();

            ISchematicElementClass schElementClass = null;
            List<int> listIds = null;
            // get the list of all feature and associated feature
            while (esrFeature != null)
            {
                var schElement = esrFeature as ISchematicElement;

                if (schElement == null)
                {
                    esrFeature = enumFeature.Next();
                    continue;
                }

                if (WithMasterId)
                {
                    int iOIDField = esrFeature.Fields.FindField("DIAGRAMOBJECTID");
                    // get the diagramId
                    var iDiagramId = (int) esrFeature.get_Value(iOIDField);

                    if (!listMasterIds.Contains(iDiagramId)) // Add to the list if required
                        listMasterIds.Add(iDiagramId);

                    if (schDiagramClass == null)
                        schDiagramClass = schElement.SchematicDiagram.SchematicDiagramClass;
                }

                var schAssociation = schElement as ISchematicElementAssociation;
                int iID = schElement.OID;
                int iUCID = schAssociation.UCID;
                int iUOID = schAssociation.UOID;

                if (lastUCID != iUCID) // verify the diagram class id
                {
                    if (schElementClass != null)
                        SelectSecondaryAssociation(schElementClass, listIds, ref dicOidByFcid);

                    schElementClass = schElement.SchematicElementClass;
                    listIds = new List<int>();
                    // get the new list of OID for this DiagramClassId
                    lastUCID = iUCID;
                    if (!dicOidByFcid.TryGetValue(lastUCID, out listOid))
                    {
                        // not found, Add it to the list
                        listOid = new List<int>();
                        dicOidByFcid.Add(lastUCID, listOid);
                    }
                }

                if (!listOid.Contains(iUOID))
                    listOid.Add(iUOID);

                if (!listIds.Contains(iID))
                    listIds.Add(iID);

                esrFeature = enumFeature.Next();

                if (esrFeature == null && schElementClass != null)
                    SelectSecondaryAssociation(schElementClass, listIds, ref dicOidByFcid);
            }

            for (int i = 0; i < GeographicalMap.LayerCount; i++)
            {
                var esrFeatLayer = GeographicalMap.get_Layer(i) as IFeatureLayer;
                if (esrFeatLayer != null && esrFeatLayer.FeatureClass != null)
                {
                    IFeatureClass esrClass = esrFeatLayer.FeatureClass;

                    int iFeatClass = esrClass.ObjectClassID;

                    if (dicOidByFcid.TryGetValue(iFeatClass, out listOid))
                    {
                        if (listOid.Count == 0) continue;

                        SelectFeaturesInMap(GeographicalMap, esrFeatLayer, listOid);
                    }
                }
            }
            GeographicalMap.DelayEvents(false);
        }

        private static void SelectSecondaryAssociation(ISchematicElementClass schElementClass, List<int> listIds,
                                                       ref Dictionary<int, List<int>> dicOidByFcid)
        {
            var esrObjectClass = schElementClass as IObjectClass;
            int iFcId = esrObjectClass.ObjectClassID;
            IWorkspace ipWKS = ((IDataset) esrObjectClass).Workspace;

            string sEltIdTableName = GetTableName(ipWKS, iFcId);

            int iPosition = sEltIdTableName.IndexOf("E_");
            string sAssTableName = String.Format("{0}A{1}", sEltIdTableName.Substring(0, iPosition),
                                                 sEltIdTableName.Substring(iPosition + 1));

            string sSecFilterAss = BuildString(listIds);

            IQueryDef esrQuerySecondary = ((IFeatureWorkspace) ipWKS).CreateQueryDef();
            esrQuerySecondary.Tables = sAssTableName;
            esrQuerySecondary.SubFields = String.Format("UCID, UOID");
            esrQuerySecondary.WhereClause = String.Format("SCHEMATICID IN ({0})", sSecFilterAss);

            ICursor esrSecondaryCursor = esrQuerySecondary.Evaluate();

            if (esrSecondaryCursor != null)
            {
                int iFieldUCID = esrSecondaryCursor.FindField("UCID");
                int iFieldUOID = esrSecondaryCursor.FindField("UOID");
                IRow esrRow = esrSecondaryCursor.NextRow();

                while (esrRow != null)
                {
                    var iUCID = (int) esrRow.get_Value(iFieldUCID);
                    var iUOID = (int) esrRow.get_Value(iFieldUOID);

                    List<int> listOID;
                    if (!dicOidByFcid.TryGetValue(iUCID, out listOID))
                    {
                        listOID = new List<int>();
                        dicOidByFcid.Add(iUCID, listOID);
                    }

                    if (!listOID.Contains(iUOID))
                        listOID.Add(iUOID);

                    esrRow = esrSecondaryCursor.NextRow();
                }
            }
        }

        internal static string GetTableName(IWorkspace esrWorkspace, int iFcId)
        {
            if (m_dicEltIdTableName == null) m_dicEltIdTableName = new Dictionary<int, string>();
            if (m_dicEltIdTableName.ContainsKey(iFcId))
                return m_dicEltIdTableName[iFcId];

            if (m_sGdbItemsTableName == "")
            {
                if (esrWorkspace.Type == esriWorkspaceType.esriRemoteDatabaseWorkspace)
                {
                    var ipSqlSyntax = esrWorkspace as ISQLSyntax;
                    var esrDbConnInfo = esrWorkspace as IDatabaseConnectionInfo;

                    string sDbName = esrDbConnInfo.ConnectedDatabase;
                    m_sGdbItemsTableName = ipSqlSyntax.QualifyTableName(sDbName, cstSDE, "GDB_Items");
                }
                else
                    m_sGdbItemsTableName = "GDB_Items";
            }

            var esrFeatWork = esrWorkspace as IFeatureWorkspace;
            IQueryDef esrQueryGDBItems = esrFeatWork.CreateQueryDef();

            esrQueryGDBItems.Tables = m_sGdbItemsTableName;
            esrQueryGDBItems.SubFields = "PhysicalName";
            esrQueryGDBItems.WhereClause = String.Format("ObjectId = {0}", iFcId);

            ICursor esrCursor = esrQueryGDBItems.Evaluate();

            if (esrCursor != null)
            {
                int iFieldId = esrCursor.FindField("PhysicalName");
                IRow esrRow = esrCursor.NextRow();
                string sName = esrRow.get_Value(iFieldId).ToString();

                m_dicEltIdTableName.Add(iFcId, sName);
                return sName;
            }

            return "";
        }

        #region PGE Schematics Maintenance Utility methods

        internal static void SelectSchematicFeaturesByPolygon(IMap map, IPolygon polygon)
        {
            IEnumLayer layers = map.Layers[null, true];
            ILayer layer = layers.Next();
            while (layer != null)
            {
                if (layer is ISchematicDiagramClassLayer)
                {
                    var compositeLayer = layer as ICompositeLayer;
                    ISpatialFilter filter = new SpatialFilterClass();
                    filter.Geometry = polygon;
                    filter.SpatialRel = esriSpatialRelEnum.esriSpatialRelIntersects;
                    filter.SubFields = "*";
                    IFeatureSelection featureSelection;
                    for (int i = 0; i < compositeLayer.Count; i++)
                    {
                        if (compositeLayer.Layer[i] is IFeatureSelection)
                        {
                            featureSelection = compositeLayer.Layer[i] as IFeatureSelection;
                            try
                            {
                                featureSelection.SelectFeatures(filter, esriSelectionResultEnum.esriSelectionResultNew,
                                                                false);
                            }
                            catch
                            {
                            }
                        }
                    }
                }
                layer = layers.Next();
            }

            // map.SelectByShape(polygon, new SelectionEnvironmentClass() { CombinationMethod = esriSelectionResultEnum.esriSelectionResultNew, AreaSelectionMethod = esriSpatialRelEnum.esriSpatialRelIntersects }, false);
        }

        internal static IPolygon CreateSIPPolygonFromGraphics(IGraphicsContainer graphicsLayer,
                                                              IFeatureClass sipFeatureClass,
                                                              IFeatureClass supFeatureClass)
        {
            graphicsLayer.Reset();
            IElement graphicElement = graphicsLayer.Next();
            IGeometryCollection polygonList = new PolygonClass();
            while (graphicElement != null)
            {
                if (graphicElement.Geometry.GeometryType == esriGeometryType.esriGeometryPolygon)
                {
                    var polygon = graphicElement.Geometry as IGeometryCollection;
                    polygonList.AddGeometryCollection(polygon);
                }
                graphicElement = graphicsLayer.Next();
            }

            if (polygonList.GeometryCount == 0)
            {
                return null;
            }

            return polygonList as IPolygon;
        }

        internal static void ClearGraphics(IMap map)
        {
            var defaultGraphicsLayer = map.ActiveGraphicsLayer as IGraphicsContainer;
            defaultGraphicsLayer.DeleteAllElements();
           
        }

        internal static bool GetSessionID(IFeatureClass supFeatureClass, IPolygon sipPolygon, out string sessionId, out string region)
        {
            sipPolygon.SpatialReference = ((IGeoDataset) supFeatureClass).SpatialReference;

            ISpatialFilter query = new SpatialFilterClass();
            query.Geometry = sipPolygon;
            
            query.SpatialRel = esriSpatialRelEnum.esriSpatialRelIntersects;

            IFeatureCursor cursor = supFeatureClass.Search(query, false);

           
            var supList = new List<string>();
            var supRegion = new Dictionary<string, string>();
            var bWarn = false;
            for (var feature = cursor.NextFeature(); feature != null; feature = cursor.NextFeature() )
            {
                object sessionObj = feature.Value[feature.Fields.FindField("SESSIONID")];
                object regionObj = feature.Value[feature.Fields.FindField("REGION")];
                if (sessionObj != null)
                {

                    if (!supList.Contains(Convert.ToString(sessionObj)))
                    {
                        supList.Add(Convert.ToString(sessionObj));
                        supRegion.Add(Convert.ToString(sessionObj),
                                      Convert.ToString(regionObj));
                    }
                    else
                    {
                        bWarn = true;
                    }
                }
                
            }
               
            

            Marshal.FinalReleaseComObject(cursor);

            if (bWarn)
            {

                DialogResult result =
                    MessageBox.Show(
                        "The current selection includes multiple Schematic Update Polygons with the same Session ID. Click OK to proceed, the polygons will be treated as a single Schematic Update Polygon.  Click Cancel to abort the Check Out operation.",
                        "", MessageBoxButtons.OKCancel);
                if (result == DialogResult.Cancel)
                {
                    sessionId = string.Empty;
                    region = string.Empty;
                    return true;
                }
            }

            string selectedSession = "";
            if (supList.Count == 0)
            {
                DialogResult result =
                    MessageBox.Show(
                        "A Schematic Update Polygon was not detected.  Click OK to proceed anyway.  Click Cancel to abort the Check Out operation.",
                        "", MessageBoxButtons.OKCancel);
                if (result == DialogResult.Cancel)
                {
                    sessionId = string.Empty;
                    region = string.Empty;
                    return true;
                }
               

                selectedSession = string.Empty;
               
            }
            else if (supList.Count > 1)
            {
                //ask the user to choose the SUP by session id

                var selectSup = new FrmSelectSUP();
                selectSup.SetSUPList(supList);
                DialogResult result = selectSup.ShowDialog();
                if (result == DialogResult.Cancel)
                {
                    sessionId = string.Empty;
                    region = string.Empty;
                    return true;
                }

                selectedSession = selectSup.GetSelectedSessionID();
            }
            else
            {
                selectedSession = supList[0];
            }

            sessionId = selectedSession;
            if (!string.IsNullOrEmpty(sessionId))
            {
                region = supRegion[sessionId];
            }
            else
            {
                region = string.Empty;
            }

            return false;
        }

        internal static void CommitSessionMetadata(IFeatureClass sipFeatureClass, IFeatureClass supFeatureClass,
                                                   IPolygon sipPolygon, string sessionID, string region)
        {
            //perform spatial query against SUP featureclass
            
            IWorkspaceEdit editWorkspace = ((IDataset) sipFeatureClass).Workspace as IWorkspaceEdit;

            //April 2019 release - Added MultiuserEditing to resolve database redefined state errors in checkin/checkout when multiple users are editing simultaneously
            ((IMultiuserWorkspaceEdit)editWorkspace).StartMultiuserEditing(esriMultiuserEditSessionMode.esriMESMNonVersioned);
            //editWorkspace.StartEditing(true);
            //editWorkspace.StartEditOperation();
            try
            {

                //System.Threading.Thread.Sleep(10000);
                //update sup feature status field
                long relatedSUP = -1;
                if (!string.IsNullOrEmpty(sessionID))
                {
                    IQueryFilter updateFilter = new QueryFilterClass();
                    updateFilter.SubFields = "*";
                    updateFilter.WhereClause = string.Format("{0} = '{1}'", "SessionID", sessionID);

                    IFeatureCursor updateCursor = supFeatureClass.Update(updateFilter, false);
                    IFeature updateFeature = updateCursor.NextFeature();
               
                    while (updateFeature != null)
                    {
                        relatedSUP = Convert.ToInt64(updateFeature.Value[updateFeature.Fields.FindField("OBJECTID")]);
                        //set status to 3 ('Diagram Under Review')
                        updateFeature.Value[updateFeature.Fields.FindField("STATUS")] = 3;
                        updateFeature.Store();

                        updateFeature = updateCursor.NextFeature();
                    }
                    Marshal.FinalReleaseComObject(updateCursor);
                }


                //Insert new SIP feature
                IFeature newSIP = sipFeatureClass.CreateFeature();

                newSIP.Shape = sipPolygon;
            
                if(!string.IsNullOrEmpty(sessionID))
                    newSIP.Value[newSIP.Fields.FindField("SESSIONID")] = sessionID;
                newSIP.Value[newSIP.Fields.FindField("RELATEDSUP")] = relatedSUP;
                newSIP.Value[newSIP.Fields.FindField("SCHEMATICEDITORID")] = Environment.UserName.Length > 8 ? Environment.UserName.Substring(0, 8) : Environment.UserName;
                newSIP.Value[newSIP.Fields.FindField("CREATIONDATE")] = DateTime.Now;
                if (newSIP.Fields.FindField("REGION") > -1)
                    newSIP.Value[newSIP.Fields.FindField("REGION")] = region;
              
                //TODO:  How are RELATEDSUP, GRIDCELLID AND REGION POPULATED?
                newSIP.Store();

                //editWorkspace.StopEditOperation();
                editWorkspace.StopEditing(true);

            }
            catch (Exception e)
            {
                Logger.Log.Error(e);
                try
                {
                    //editWorkspace.AbortEditOperation();
                    editWorkspace.StopEditing(false);
                }
                catch (Exception)
                {

                }
                throw;
            }
        }
        
        internal static IFeatureClass FindFeatureClass(string fcName)
        {
            
            try
            {
                IMaps maps = ArcMap.Document.Maps;

                for (int i = 0; i < maps.Count; i++)
                {
                    UID uid = new UIDClass();
                    uid.Value = "{40A9E885-5533-11d0-98BE-00805F7CED21}";
                    IEnumLayer layers = maps.Item[i].Layers[uid, true];
                    var layer = layers.Next() as IFeatureLayer;
                    while (layer != null)
                    {
                        var dataset = layer as IDataset;
                        if (dataset != null && dataset.BrowseName.ToUpper().Contains(fcName.ToUpper()))
                        {
                            return layer.FeatureClass;
                        }


                        layer = layers.Next() as IFeatureLayer;
                    }
                }
            }
            catch
            {
            }
            return null;
        }

        internal static void ApplyCircuitIDQueryDefToLayers(IMap focusMap,string circuitIdField, string circuitId)
        {
            var layers = focusMap.Layers[null, true];

            for (var layer = layers.Next(); layer != null; layer = layers.Next() )
            {
                var fields = layer as ILayerFields;
                if (fields != null)
                {
                    

                    var definition = layer as IFeatureLayerDefinition;

                    if (fields.FindField(circuitIdField) > -1 && definition != null)
                    {
                        string defQuery = definition.DefinitionExpression;

                        if (string.IsNullOrEmpty(defQuery) || string.IsNullOrEmpty(defQuery.Trim()))
                        {
                            definition.DefinitionExpression = string.Format("{0} = '{1}'",
                                                                            circuitIdField, circuitId);
                        }
                        else
                        {
                            defQuery = RemoveCircuitIDQueryFromString(defQuery, circuitIdField);
                            if (string.IsNullOrEmpty(defQuery) || string.IsNullOrEmpty(defQuery.Trim()))
                            {
                                definition.DefinitionExpression = string.Format("{0} = '{1}'",
                                                                                circuitIdField, circuitId);
                            }
                            else
                            {
                                definition.DefinitionExpression = string.Format("{0} AND {1} = '{2}'",
                                                                                defQuery, circuitIdField, circuitId);    
                            }
                            
                        }

                    }
                }
            }
        }

        internal static string RemoveCircuitIDQueryFromString(string defQuery, string circuitIdField)
        {
            if (Regex.IsMatch(defQuery, string.Format(@"\sAND\s*{0}\s*=\s*'.+'", circuitIdField), RegexOptions.IgnoreCase))
            {
                return Regex.Replace(defQuery, string.Format(@"\sAND\s*{0}\s*=\s*'.+'", circuitIdField), "",
                              RegexOptions.IgnoreCase);
            }
            if (Regex.IsMatch(defQuery, string.Format(@"\s*{0}\s*=\s*'.+'", circuitIdField),
                                   RegexOptions.IgnoreCase))
            {
                return Regex.Replace(defQuery, string.Format(@"\s*{0}\s*=\s*'.+'", circuitIdField), "",
                              RegexOptions.IgnoreCase);
            }
            return defQuery;
        }


        internal static void RemoveCircuitIDQueryDefFromLayers(IMap focusMap, string circuitIdField)
        {
            var layers = focusMap.Layers[null, true];

            for (var layer = layers.Next(); layer != null; layer = layers.Next())
            {
                var fields = layer as ILayerFields;
                if (fields != null)
                {


                    var definition = layer as IFeatureLayerDefinition;

                    if (fields.FindField(circuitIdField) > -1 && definition != null)
                    {
                        string defQuery = definition.DefinitionExpression;

                        if (!string.IsNullOrEmpty(defQuery) && !string.IsNullOrEmpty(defQuery.Trim()))
                        {

                            definition.DefinitionExpression =
                                RemoveCircuitIDQueryFromString(definition.DefinitionExpression, circuitIdField);
                        }

                    }
                }
            }
        }


        #endregion
    }

    public class MyEnumObject : IEnumObject
    {
        private readonly IEnumFeature m_enumFeature;

        public MyEnumObject(IEnumFeature pEnumFeature)
        {
            if (pEnumFeature != null)
            {
                pEnumFeature.Reset();
                ((IEnumFeatureSetup) pEnumFeature).AllFields = true;
            }

            m_enumFeature = pEnumFeature;
        }

        public IObject Next()
        {
            if (m_enumFeature != null)
                return m_enumFeature.Next();
            else return null;
        }

        public void Reset()
        {
            if (m_enumFeature != null) m_enumFeature.Reset();
        }


        internal static void InitGraphicsLayer()
        {
            IMap map = Utils.GetSchematicMap(false);
        }
    }
}