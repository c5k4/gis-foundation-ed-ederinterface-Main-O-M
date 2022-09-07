using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ESRI.ArcGIS.DataSourcesFile;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using PGE.BatchApplication.CircuitProtectionZone.Util;
using PGE.BatchApplication.CircuitProtectionZone.Common;
using Path = System.IO.Path;
using System.Runtime.InteropServices;
using PGE.BatchApplication.CircuitProtectionZone.Properties;
using System.Text;
using ESRI.ArcGIS.Geoprocessor;
using System.Diagnostics;
using ESRI.ArcGIS.NetworkAnalysis;

namespace PGE.BatchApplication.CircuitProtectionZone.Data
{
    public class ResultFeatureFactory
    {
        public ResultFeatureFactory()
        {
        }

        public double GetLength(string featureClassName, IEnumNetEID edges, INetwork network, ref FireTierAndIndexInformation FireInformation, bool isOverhead)
        {
            try
            {
                double length = 0;

                int userClassId;
                int userId;
                int userSubId;

                var netElements = (INetElements)network;

                IFeatureClass featureClass = Geodatabase.GetFeatureClass(featureClassName);
                GeometryBagClass primaryLineGeomBag = new GeometryBagClass();
                primaryLineGeomBag.SpatialReference = ((IGeoDataset)featureClass).SpatialReference;
                if (featureClass != null)
                {
                    List<int> featureIds = new List<int>();

                    edges.Reset();
                    int eid = edges.Next();

                    while (eid > 0)
                    {
                        try
                        {
                            netElements.QueryIDs(eid, esriElementType.esriETEdge, out userClassId, out userId, out userSubId);

                            if (userClassId == featureClass.FeatureClassID)
                            {
                                featureIds.Add(userId);
                            }
                        }
                        catch (Exception ex)
                        {
                            //Log the error and continue
                            Logger.Error(string.Format("Failed to determine feature with EID {0} for feature class {1} due to error {2}", eid, ((IDataset)featureClass).BrowseName, ex.Message), true);
                        }
                        eid = edges.Next();
                    }

                    if (featureIds.Count > 0)
                    {
                        List<string> whereInClauses = new List<string>();
                        whereInClauses = GetWhereInClauses(featureIds);
                        IQueryFilter qf = new QueryFilterClass();
                        qf.SubFields = featureClass.ShapeFieldName + "," + featureClass.OIDFieldName;
                        foreach (string whereInClause in whereInClauses)
                        {
                            qf.WhereClause = string.Format("{0} in ({1})", featureClass.OIDFieldName, whereInClause);
                            IFeatureCursor cursor = featureClass.Search(qf, false);

                            IFeature feature = null;
                            while ((feature = cursor.NextFeature()) != null)
                            {
                                IPolyline polyLine = feature.Shape as IPolyline;
                                if (polyLine != null)
                                {
                                    length += polyLine.Length;
                                }

                                primaryLineGeomBag.AddGeometry(feature.ShapeCopy);
                                while ((Marshal.ReleaseComObject(feature)) > 0) { }
                            }

                            if (cursor != null) { while (Marshal.ReleaseComObject(cursor) > 0) { } }
                        }
                    }
                }

                if (length > 0)
                {
                    length = length / 5280.0;
                }

                if (primaryLineGeomBag.Count > 0)
                {
                    GetTierLength(primaryLineGeomBag, ref FireInformation, length, isOverhead);
                    GetFireIndexLength(primaryLineGeomBag, ref FireInformation, isOverhead);
                }
                if (primaryLineGeomBag != null) { while (Marshal.ReleaseComObject(primaryLineGeomBag) > 0) { } }

                return length;
            }
            catch (Exception ex)
            {
                string error = string.Format("Failed to determine length. Error: {0}: StackTrace: {1}", ex.Message, ex.StackTrace);
                Logger.Error(error, true);
                throw new Exception(error);
            }
        }

        private void GetTierLength(IGeometry primaryLineGeometry, ref FireTierAndIndexInformation FireInformation, double totalLength, bool isOverhead)
        {
            try
            {
                double tierOneLength = totalLength;
                List<int> fireTierValues = new List<int>();
                fireTierValues.Add(2);
                fireTierValues.Add(3);

                foreach (int fireTierValue in fireTierValues)
                {
                    double length = 0.0;
                    IGeometry intersectingFireTierGeometry = GetIntersectingGridsFromGeometry(primaryLineGeometry, fireTierValue);

                    if (intersectingFireTierGeometry != null)
                    {
                        ITopologicalOperator topOp = intersectingFireTierGeometry as ITopologicalOperator;

                        IGeometry intersectionLine = null;
                        GeometryBagClass primaryLineGeomBag = primaryLineGeometry as GeometryBagClass;
                        primaryLineGeomBag.Reset();
                        IGeometry primaryLineGeom = null;
                        IGeometry primaryLineIntersectGeom = null;
                        int count = 0;
                        while ((primaryLineGeom = primaryLineGeomBag.Next()) != null)
                        {
                            if (primaryLineIntersectGeom == null) { primaryLineIntersectGeom = primaryLineGeom; }

                            ITopologicalOperator primaryLineTopOp = primaryLineGeom as ITopologicalOperator;

                            if ((count > 0 && (count % 10 == 0)) || (count == primaryLineGeomBag.Count - 1))
                            {
                                intersectionLine = topOp.Intersect(primaryLineIntersectGeom, esriGeometryDimension.esriGeometry1Dimension);

                                if (intersectionLine is IPolyline)
                                {
                                    length += ((IPolyline)intersectionLine).Length;
                                }
                                if (intersectionLine != null) { while (Marshal.ReleaseComObject(intersectionLine) > 0) { } }
                                if (primaryLineIntersectGeom != null) { while (Marshal.ReleaseComObject(primaryLineIntersectGeom) > 0) { } }
                                primaryLineIntersectGeom = primaryLineGeom;
                            }
                            else
                            {
                                primaryLineIntersectGeom = primaryLineTopOp.Union(primaryLineIntersectGeom);
                            }
                            count++;
                        }

                        if (intersectingFireTierGeometry != null) { while (Marshal.ReleaseComObject(intersectingFireTierGeometry) > 0) { } }

                        if (length > 0)
                        {
                            length = length / 5280.0;
                        }
                    }
                    tierOneLength -= length;
                    FireInformation.SetFireTierPrimaryMiles(isOverhead, fireTierValue, length);
                }

                //Anything that wasn't contained in a fire tier 2 or 3 is assumed fire tier 1
                FireInformation.SetFireTierPrimaryMiles(isOverhead, 1, tierOneLength);
            }
            catch (Exception ex)
            {
                string error = string.Format("Failed to determine tier length. Error: {0}: StackTrace: {1}", ex.Message, ex.StackTrace);
                Logger.Error(error, true);
                throw new Exception(error);
            }
        }

        /// <summary>
        /// Determines the length of primary lines lying within each fire index
        /// </summary>
        /// <param name="primaryLineGeometry"></param>
        /// <param name="FireInformation"></param>
        /// <param name="totalLength"></param>
        /// <param name="isOverhead"></param>
        private void GetFireIndexLength(IGeometry primaryLineGeometry, ref FireTierAndIndexInformation FireInformation, bool isOverhead)
        {
            try
            {
                Dictionary<int, IGeometry> intersectingFireIndexGeometries = GetIntersectingFireIndexFromGeometry(primaryLineGeometry);

                foreach (KeyValuePair<int, IGeometry> kvp in intersectingFireIndexGeometries)
                {
#if DEBUG
                    if (intersectingFireIndexGeometries.Count > 2)
                    {
                        //Debugger.Launch();
                    }
#endif
                    double length = 0.0;

                    IGeometry intersectingFireIndexGeometry = kvp.Value;
                    if (intersectingFireIndexGeometry != null)
                    {
                        ITopologicalOperator topOp = intersectingFireIndexGeometry as ITopologicalOperator;

                        IGeometry intersectionLine = null;
                        GeometryBagClass primaryLineGeomBag = primaryLineGeometry as GeometryBagClass;
                        primaryLineGeomBag.Reset();
                        IGeometry primaryLineGeom = null;
                        IGeometry primaryLineIntersectGeom = null;
                        int count = 0;

                        //Strange behavior was ocurring when unioning a lot of primary lines together so it was broken out to only
                        //merge at max 10 lines at once
                        while ((primaryLineGeom = primaryLineGeomBag.Next()) != null)
                        {
                            if (primaryLineIntersectGeom == null) { primaryLineIntersectGeom = primaryLineGeom; }

                            ITopologicalOperator primaryLineTopOp = primaryLineGeom as ITopologicalOperator;

                            if ((count > 0 && (count % 10 == 0)) || (count == primaryLineGeomBag.Count - 1))
                            {
                                intersectionLine = topOp.Intersect(primaryLineIntersectGeom, esriGeometryDimension.esriGeometry1Dimension);

                                if (intersectionLine is IPolyline)
                                {
                                    length += ((IPolyline)intersectionLine).Length;
                                }
                                if (intersectionLine != null) { while (Marshal.ReleaseComObject(intersectionLine) > 0) { } }
                                if (primaryLineIntersectGeom != null) { while (Marshal.ReleaseComObject(primaryLineIntersectGeom) > 0) { } }
                                primaryLineIntersectGeom = primaryLineGeom;
                            }
                            else
                            {
                                primaryLineIntersectGeom = primaryLineTopOp.Union(primaryLineIntersectGeom);
                            }
                            count++;
                        }

                        if (intersectingFireIndexGeometry != null) { while (Marshal.ReleaseComObject(intersectingFireIndexGeometry) > 0) { } }

                        //Convert to miles
                        if (length > 0)
                        {
                            length = length / 5280.0;
                        }
                    }

                    //Length for this fire index has been calculated and can be saved
                    FireInformation.SetFireIndexPrimaryMiles(isOverhead, kvp.Key, length);
                }
            }
            catch (Exception ex)
            {
                string error = string.Format("Failed to determine fire index length. Error: {0}: StackTrace: {1}", ex.Message, ex.StackTrace);
                Logger.Error(error, true);
                throw new Exception(error);
            }
        }

        public Dictionary<int, IGeometry> GetIntersectingFireIndexFromGeometry(IGeometry queryGeometry)
        {
            try
            {
                Dictionary<int, IGeometry> resultGeometries = new Dictionary<int, IGeometry>();

                //Use spatial filter to detemine ALL intersected grids.
                IFeatureClass fireIndexFeatureClass = Geodatabase.GetFeatureClass(Settings.Default.FireIndexFeatureClassName);
                IQueryFilter qf = FilterUtil.CreateSpatialFilterFromGeometry(queryGeometry as IGeometry, fireIndexFeatureClass);
                qf.WhereClause = "";
                IFeatureCursor featCurs = fireIndexFeatureClass.Search(qf, false);
                int nameFieldIdx = fireIndexFeatureClass.FindField(Settings.Default.FireIndexNameField);

                //Due to differing grid sizes there will very likely be numerous results - loop through and retrieve all of them.
                //bool foundGrid = false;
                IFeature gridFeat = null;
                while ((gridFeat = featCurs.NextFeature()) != null)
                {
                    int name = -1;
                    try { name = Int32.Parse(gridFeat.get_Value(nameFieldIdx).ToString()); }
                    catch { name = -1; }

                    //If this name hasn't been added yet, let's do that now
                    if (!resultGeometries.ContainsKey(name)) { resultGeometries.Add(name, null); }

                    IGeometry newGeom = gridFeat.Shape;
                    newGeom.Project(queryGeometry.SpatialReference);
                    ITopologicalOperator topOp = newGeom as ITopologicalOperator;

                    if (resultGeometries[name] == null)
                    {
                        resultGeometries[name] = topOp.Union(newGeom);
                    }
                    else
                    {
                        resultGeometries[name] = topOp.Union(resultGeometries[name]);
                    }
                    if (gridFeat != null) { while (Marshal.ReleaseComObject(gridFeat) > 0) { } }
                }

                if (featCurs != null) { while (Marshal.ReleaseComObject(featCurs) > 0) { } }
                if (qf != null) { while (Marshal.ReleaseComObject(qf) > 0) { } }

                return resultGeometries;
            }
            catch (Exception ex)
            {
                string error = string.Format("Failed to determine intersecting grids. Error: {0}: StackTrace: {1}", ex.Message, ex.StackTrace);
                Logger.Error(error, true);
                throw new Exception(error);
            }
        }

        public IGeometry GetIntersectingGridsFromGeometry(IGeometry queryGeometry, int tier)
        {
            try
            {
                IGeometry resultGeometry = null;

                //Use spatial filter to detemine ALL intersected grids.
                IFeatureClass fireTierFeatureClass = Geodatabase.GetFeatureClass(Settings.Default.FireTierFeatureClassName);
                IQueryFilter qf = FilterUtil.CreateSpatialFilterFromGeometry(queryGeometry as IGeometry, fireTierFeatureClass);
                qf.WhereClause = string.Format("{1} LIKE '%Tier {0}%'", tier, Settings.Default.FireTierFieldName);
                IFeatureCursor featCurs = fireTierFeatureClass.Search(qf, false);

                //Due to differing grid sizes there will very likely be numerous results - loop through and retrieve all of them.
                //bool foundGrid = false;
                IFeature gridFeat = null;
                while ((gridFeat = featCurs.NextFeature()) != null)
                {
                    IGeometry newGeom = gridFeat.Shape;
                    newGeom.Project(queryGeometry.SpatialReference);
                    ITopologicalOperator topOp = newGeom as ITopologicalOperator;

                    if (resultGeometry == null)
                    {
                        resultGeometry = topOp.Union(newGeom);
                    }
                    else
                    {
                        resultGeometry = topOp.Union(resultGeometry);
                    }
                    if (gridFeat != null) { while (Marshal.ReleaseComObject(gridFeat) > 0) { } }
                }

                if (featCurs != null) { while (Marshal.ReleaseComObject(featCurs) > 0) { } }
                if (qf != null) { while (Marshal.ReleaseComObject(qf) > 0) { } }

                return resultGeometry;
            }
            catch (Exception ex)
            {
                string error = string.Format("Failed to determine intersecting grids. Error: {0}: StackTrace: {1}", ex.Message, ex.StackTrace);
                Logger.Error(error, true);
                throw new Exception(error);
            }
        }

        /// <summary>
        /// Obtains a list of where in clauses for a list of guids
        /// </summary>
        /// <param name="guids"></param>
        /// <returns></returns>
        private List<string> GetWhereInClauses(List<string> guids)
        {
            try
            {
                List<string> whereInClauses = new List<string>();
                StringBuilder builder = new StringBuilder();

                int counter = 0;
                foreach (string guid in guids)
                {
                    if (counter == 999)
                    {
                        builder.Append("'" + guid + "'");
                        whereInClauses.Add(builder.ToString());
                        builder = new StringBuilder();
                        counter = 0;
                    }
                    else
                    {
                        builder.Append("'" + guid + "',");
                        counter++;
                    }
                }

                if (builder.Length > 0)
                {
                    string whereInClause = builder.ToString();
                    whereInClause = whereInClause.Substring(0, whereInClause.LastIndexOf(","));
                    whereInClauses.Add(whereInClause);
                }
                return whereInClauses;
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to create guid where in clauses. Error: " + ex.Message);
            }
        }

        /// <summary>
        /// Obtains a list of where in clauses for a list of guids
        /// </summary>
        /// <param name="guids"></param>
        /// <returns></returns>
        private List<string> GetWhereInClauses(List<int> guids)
        {
            try
            {
                List<string> whereInClauses = new List<string>();
                StringBuilder builder = new StringBuilder();

                int counter = 0;
                foreach (int guid in guids)
                {
                    if (counter == 999)
                    {
                        builder.Append(guid);
                        whereInClauses.Add(builder.ToString());
                        builder = new StringBuilder();
                        counter = 0;
                    }
                    else
                    {
                        builder.Append(guid + ",");
                        counter++;
                    }
                }

                if (builder.Length > 0)
                {
                    string whereInClause = builder.ToString();
                    whereInClause = whereInClause.Substring(0, whereInClause.LastIndexOf(","));
                    whereInClauses.Add(whereInClause);
                }
                return whereInClauses;
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to create guid where in clauses. Error: " + ex.Message);
            }
        }

        public string GetFireTier(IFeatureClass featureClass, IPolygon polygon, string fieldName)
        {
            string returnValue = string.Empty;
            List<string> returnValues = new List<string>();

            if (featureClass != null)
            {
                IQueryFilter qf = FilterUtil.CreateSpatialFilterFromGeometry(polygon, featureClass);
                IFeatureCursor featureCursor = featureClass.Search(qf, false);
                IFeature feature = featureCursor.NextFeature();
                while (feature != null)
                {
                    returnValues.Add(EsriFieldsUtil.GetFieldValue(feature, fieldName).ToString());
                    while (Marshal.ReleaseComObject(feature) > 0) { }
                    feature = featureCursor.NextFeature();
                }
                if (qf != null) { while (Marshal.ReleaseComObject(qf) > 0) { } }
                if (featureCursor != null) { while (Marshal.ReleaseComObject(featureCursor) > 0) { } }
            }

            if (returnValues.Any() == true)
            {
                returnValue = returnValues.Contains("T3") == true
                    ? "T3"
                    : (returnValues.Contains("T2") == true ? "T2" : returnValues[0]);
            }

            return returnValue;
        }

        public string Create(IFeatureClass featureClass, ResultFeature resultFeature)
        {
            string featureClassNameString = ((IDataset)featureClass).FullName.NameString;
            int fileExtIndex = featureClassNameString.LastIndexOf(".", StringComparison.Ordinal) + 1;
            string fileExt = featureClassNameString.Substring(fileExtIndex, featureClassNameString.Length - fileExtIndex);

            try
            {
                if (featureClass != null)
                {
                    IFeatureBuffer featureBuffer = featureClass.CreateFeatureBuffer();

                    featureBuffer.Shape = resultFeature.Polygon;

                    SetFeatureClassFields(featureClass, resultFeature, ref featureBuffer);

                    IFeatureCursor featureCursor = featureClass.Insert(true);
                    object zoneFeatureID = featureCursor.InsertFeature(featureBuffer);
                    featureCursor.Flush();
                    while (Marshal.ReleaseComObject(featureCursor) > 0) { }

                    IFeature zoneFeature = featureClass.GetFeature(Int32.Parse(zoneFeatureID.ToString()));
                    return zoneFeature.get_Value(featureClass.Fields.FindField("GLOBALID")).ToString();
                }
            }
            catch (Exception ex)
            {
#if DEBUG
                //Debugger.Launch();
#endif
                string error = string.Format("An error occurred during in ResultFeatureFactory.Create. Error {0}: StackTrace: {1}", ex.Message, ex.StackTrace);
                Logger.Error(error, true);
                throw new Exception(error);
            }
            return "";
        }

        private Dictionary<string, Dictionary<Guid, List<Guid>>> GuidToCPZ = new Dictionary<string, Dictionary<Guid, List<Guid>>>();
        public void CacheDetailFeatures(List<int> ClassIDsToIncludeForJunctions, List<int> ClassIDsToIncludeForEdges, string CPZGlobalID, IEnumEIDInfo junctionInfoEnum, IEnumEIDInfo edgeInfoEnum)
        {
            try
            {
                if (junctionInfoEnum != null)
                {
                    IEIDInfo junctionInfo = null;
                    while ((junctionInfo = junctionInfoEnum.Next()) != null)
                    {
                        if (ClassIDsToIncludeForJunctions.Contains(junctionInfo.Feature.Class.ObjectClassID))
                        {
                            string tableName = ((IDataset)junctionInfo.Feature.Table).BrowseName;
                            if (!GuidToCPZ.ContainsKey(tableName)) { GuidToCPZ.Add(tableName, new Dictionary<Guid, List<Guid>>()); }

                            Guid featureGUID = new Guid(junctionInfo.Feature.get_Value(junctionInfo.Feature.Fields.FindField("GLOBALID")).ToString());
                            if (!GuidToCPZ[tableName].ContainsKey(featureGUID)) { GuidToCPZ[tableName].Add(featureGUID, new List<Guid>()); }
                            GuidToCPZ[tableName][featureGUID].Add(new Guid(CPZGlobalID));
                        }
                    }
                }

                if (edgeInfoEnum != null)
                {
                    IEIDInfo edgeInfo = null;
                    while ((edgeInfo = edgeInfoEnum.Next()) != null)
                    {
                        if (ClassIDsToIncludeForEdges.Contains(edgeInfo.Feature.Class.ObjectClassID))
                        {
                            string tableName = ((IDataset)edgeInfo.Feature.Table).BrowseName;
                            if (!GuidToCPZ.ContainsKey(tableName)) { GuidToCPZ.Add(tableName, new Dictionary<Guid, List<Guid>>()); }

                            Guid featureGUID = new Guid(edgeInfo.Feature.get_Value(edgeInfo.Feature.Fields.FindField("GLOBALID")).ToString());
                            if (!GuidToCPZ[tableName].ContainsKey(featureGUID)) { GuidToCPZ[tableName].Add(featureGUID, new List<Guid>()); }
                            GuidToCPZ[tableName][featureGUID].Add(new Guid(CPZGlobalID));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
#if DEBUG
                //Debugger.Launch();
#endif
                string error = string.Format("An error occurred during in ResultFeatureFactory.Create. Error {0}: StackTrace: {1}", ex.Message, ex.StackTrace);
                Logger.Error(error, true);
                throw new Exception(error);
            }
        }

        internal void Create(ITable table)
        {
            string featureClassNameString = ((IDataset)table).FullName.NameString;

            try
            {
                if (table != null)
                {
                    ICursor featureCursor = table.Insert(true);

                    foreach (KeyValuePair<string, Dictionary<Guid, List<Guid>>> kvp in GuidToCPZ)
                    {
                        string tableName = kvp.Key;
                        foreach (KeyValuePair<Guid, List<Guid>> CPZList in kvp.Value)
                        {
                            Guid featureGuid = CPZList.Key;
                            foreach (Guid CPZ in CPZList.Value)
                            {
                                IRowBuffer rowBuffer = table.CreateRowBuffer();

                                EsriFieldsUtil.SetField((IObject)rowBuffer, "TABLENAME", tableName);
                                EsriFieldsUtil.SetField((IObject)rowBuffer, "CPZGLOBALID", CPZ.ToString("B").ToUpper());
                                EsriFieldsUtil.SetField((IObject)rowBuffer, "FEATUREGLOBALID", featureGuid.ToString("B").ToUpper());
                                featureCursor.InsertRow(rowBuffer);
                            }
                        }
                    }

                    featureCursor.Flush();
                    while (Marshal.ReleaseComObject(featureCursor) > 0) { }
                    GuidToCPZ.Clear();
                }
            }
            catch (Exception ex)
            {
#if DEBUG
                //Debugger.Launch();
#endif
                string error = string.Format("An error occurred during in ResultFeatureFactory.Create. Error {0}: StackTrace: {1}", ex.Message, ex.StackTrace);
                Logger.Error(error, true);
                throw new Exception(error);
            }
        }

        private static void SetFeatureClassFields(IFeatureClass featureClass, ResultFeature resultFeature,
            ref IFeatureBuffer featureBuffer)
        {
            try
            {
                if (EsriFieldsUtil.IsFieldSupported(featureClass, "CIRCUITID"))
                {
                    EsriFieldsUtil.SetField((IObject)featureBuffer, "CIRCUITID",
                        resultFeature.CircuitId);
                }

                if (EsriFieldsUtil.IsFieldSupported(featureClass, "CIRCUITNAME"))
                {
                    EsriFieldsUtil.SetField((IObject)featureBuffer, "CIRCUITNAME",
                        resultFeature.CircuitName);
                }

                if (EsriFieldsUtil.IsFieldSupported(featureClass, "ZONE"))
                {
                    EsriFieldsUtil.SetField((IObject)featureBuffer, "ZONE",
                        resultFeature.Zone);
                }

                if (EsriFieldsUtil.IsFieldSupported(featureClass, "ZONETYPE"))
                {
                    EsriFieldsUtil.SetField((IObject)featureBuffer, "ZONETYPE",
                        resultFeature.ZoneType);
                }

                if (EsriFieldsUtil.IsFieldSupported(featureClass, "UGPRIMARYMILES"))
                {
                    EsriFieldsUtil.SetField((IObject)featureBuffer, "UGPRIMARYMILES",
                        resultFeature.PrimaryUgMiles);
                }
                if (EsriFieldsUtil.IsFieldSupported(featureClass, "T1UGPRIMARYMILES"))
                {
                    EsriFieldsUtil.SetField((IObject)featureBuffer, "T1UGPRIMARYMILES",
                        resultFeature.T1PrimaryUgMiles);
                }
                if (EsriFieldsUtil.IsFieldSupported(featureClass, "T2UGPRIMARYMILES"))
                {
                    EsriFieldsUtil.SetField((IObject)featureBuffer, "T2UGPRIMARYMILES",
                        resultFeature.T2PrimaryUgMiles);
                }
                if (EsriFieldsUtil.IsFieldSupported(featureClass, "T3UGPRIMARYMILES"))
                {
                    EsriFieldsUtil.SetField((IObject)featureBuffer, "T3UGPRIMARYMILES",
                        resultFeature.T3PrimaryUgMiles);
                }

                if (EsriFieldsUtil.IsFieldSupported(featureClass, "OHPRIMARYMILES"))
                {
                    EsriFieldsUtil.SetField((IObject)featureBuffer, "OHPRIMARYMILES",
                        resultFeature.PrimaryOhMiles);
                }
                if (EsriFieldsUtil.IsFieldSupported(featureClass, "T1OHPRIMARYMILES"))
                {
                    EsriFieldsUtil.SetField((IObject)featureBuffer, "T1OHPRIMARYMILES",
                        resultFeature.T1PrimaryOhMiles);
                }
                if (EsriFieldsUtil.IsFieldSupported(featureClass, "T2OHPRIMARYMILES"))
                {
                    EsriFieldsUtil.SetField((IObject)featureBuffer, "T2OHPRIMARYMILES",
                        resultFeature.T2PrimaryOhMiles);
                }
                if (EsriFieldsUtil.IsFieldSupported(featureClass, "T3OHPRIMARYMILES"))
                {
                    EsriFieldsUtil.SetField((IObject)featureBuffer, "T3OHPRIMARYMILES",
                        resultFeature.T3PrimaryOhMiles);
                }

                if (EsriFieldsUtil.IsFieldSupported(featureClass, "PROTECTIVEDEVICEGUID"))
                {
                    string globalId = string.Format("{{{0}}}", resultFeature.DeviceGlobalId);

                    EsriFieldsUtil.SetField((IObject)featureBuffer, "PROTECTIVEDEVICEGUID",
                        globalId);
                }

                if (EsriFieldsUtil.IsFieldSupported(featureClass, "DEVICEOPERATINGNUMBER"))
                {
                    EsriFieldsUtil.SetField((IObject)featureBuffer, "DEVICEOPERATINGNUMBER",
                        resultFeature.DeviceOperatingNumber);
                }

                if (EsriFieldsUtil.IsFieldSupported(featureClass, "GLOBALID"))
                {
                    string globalId = string.Format("{{{0}}}", Guid.NewGuid());
                    EsriFieldsUtil.SetField((IObject)featureBuffer, "GLOBALID",
                        globalId);
                }

                if (EsriFieldsUtil.IsFieldSupported(featureClass, "FIRETIER"))
                {
                    EsriFieldsUtil.SetField((IObject)featureBuffer, "FIRETIER",
                        resultFeature.FireTier);
                }

                if (EsriFieldsUtil.IsFieldSupported(featureClass, "CUSTOMERSINZONE"))
                {
                    EsriFieldsUtil.SetField((IObject)featureBuffer, "CUSTOMERSINZONE",
                        resultFeature.CustomersInZone);
                }

                if (EsriFieldsUtil.IsFieldSupported(featureClass, "ListOfDPDs"))
                {
                    EsriFieldsUtil.SetField((IObject)featureBuffer, "ListOfDPDs",
                           resultFeature.ListOfDpds);
                }
                if (EsriFieldsUtil.IsFieldSupported(featureClass, "CustomersEssential"))
                {
                    EsriFieldsUtil.SetField((IObject)featureBuffer, "CustomersEssential",
                           resultFeature.CustomersEssentialInZone);
                }
                if (EsriFieldsUtil.IsFieldSupported(featureClass, "CustomersSensitive"))
                {
                    EsriFieldsUtil.SetField((IObject)featureBuffer, "CustomersSensitive",
                           resultFeature.CustomersSensitiveInZone);
                }
                if (EsriFieldsUtil.IsFieldSupported(featureClass, "CustomersLifeSupport"))
                {
                    EsriFieldsUtil.SetField((IObject)featureBuffer, "CustomersLifeSupport",
                           resultFeature.CustomersLifeSupportInZone);
                }
                if (EsriFieldsUtil.IsFieldSupported(featureClass, "CustomersDOM"))
                {
                    EsriFieldsUtil.SetField((IObject)featureBuffer, "CustomersDOM",
                           resultFeature.CustomersDOMDInZone);
                }
                if (EsriFieldsUtil.IsFieldSupported(featureClass, "CustomersAGR"))
                {
                    EsriFieldsUtil.SetField((IObject)featureBuffer, "CustomersAGR",
                           resultFeature.CustomersAGRInZone);
                }
                if (EsriFieldsUtil.IsFieldSupported(featureClass, "CustomersIND"))
                {
                    EsriFieldsUtil.SetField((IObject)featureBuffer, "CustomersIND",
                           resultFeature.CustomersINDInZone);
                }
                if (EsriFieldsUtil.IsFieldSupported(featureClass, "CustomersCOM"))
                {
                    EsriFieldsUtil.SetField((IObject)featureBuffer, "CustomersCOM",
                           resultFeature.CustomersCOMInZone);
                }
                if (EsriFieldsUtil.IsFieldSupported(featureClass, "CustomersOTH"))
                {
                    EsriFieldsUtil.SetField((IObject)featureBuffer, "CustomersOTH",
                           resultFeature.CustomersOTHInZone);
                }

                if (EsriFieldsUtil.IsFieldSupported(featureClass, "FIREINDEX1"))
                {
                    EsriFieldsUtil.SetField((IObject)featureBuffer, "FIREINDEX1",
                           resultFeature.FireIndex1);
                }
                if (EsriFieldsUtil.IsFieldSupported(featureClass, "FIREINDEX2"))
                {
                    EsriFieldsUtil.SetField((IObject)featureBuffer, "FIREINDEX2",
                           resultFeature.FireIndex2);
                }
                if (EsriFieldsUtil.IsFieldSupported(featureClass, "FIREINDEX3"))
                {
                    EsriFieldsUtil.SetField((IObject)featureBuffer, "FIREINDEX3",
                           resultFeature.FireIndex3);
                }

                if (EsriFieldsUtil.IsFieldSupported(featureClass, "FI1OHPRIMARYMILES"))
                {
                    EsriFieldsUtil.SetField((IObject)featureBuffer, "FI1OHPRIMARYMILES",
                           resultFeature.FireIndex1OHMiles);
                }
                if (EsriFieldsUtil.IsFieldSupported(featureClass, "FI2OHPRIMARYMILES"))
                {
                    EsriFieldsUtil.SetField((IObject)featureBuffer, "FI2OHPRIMARYMILES",
                           resultFeature.FireIndex2OHMiles);
                }
                if (EsriFieldsUtil.IsFieldSupported(featureClass, "FI3OHPRIMARYMILES"))
                {
                    EsriFieldsUtil.SetField((IObject)featureBuffer, "FI3OHPRIMARYMILES",
                           resultFeature.FireIndex3OHMiles);
                }

                if (EsriFieldsUtil.IsFieldSupported(featureClass, "FI1UGPRIMARYMILES"))
                {
                    EsriFieldsUtil.SetField((IObject)featureBuffer, "FI1UGPRIMARYMILES",
                           resultFeature.FireIndex1UGMiles);
                }
                if (EsriFieldsUtil.IsFieldSupported(featureClass, "FI2UGPRIMARYMILES"))
                {
                    EsriFieldsUtil.SetField((IObject)featureBuffer, "FI2UGPRIMARYMILES",
                           resultFeature.FireIndex2UGMiles);
                }
                if (EsriFieldsUtil.IsFieldSupported(featureClass, "FI3UGPRIMARYMILES"))
                {
                    EsriFieldsUtil.SetField((IObject)featureBuffer, "FI3UGPRIMARYMILES",
                           resultFeature.FireIndex3UGMiles);
                }
                if (EsriFieldsUtil.IsFieldSupported(featureClass, "SUBInterruptingDeviceGuid"))
                {
                    EsriFieldsUtil.SetField((IObject)featureBuffer, "SUBInterruptingDeviceGuid",
                           resultFeature.SubInterruptingDeviceGuid.ToString("B").ToUpper());
                }
                if (EsriFieldsUtil.IsFieldSupported(featureClass, "DeviceType"))
                {
                    EsriFieldsUtil.SetField((IObject)featureBuffer, "DeviceType",
                           resultFeature.DeviceType);
                }
                if (EsriFieldsUtil.IsFieldSupported(featureClass, "Status"))
                {
                    EsriFieldsUtil.SetField((IObject)featureBuffer, "Status",
                           resultFeature.Status);
                }
            }
            catch (Exception ex)
            {
                Logger.Error("An error occurred in ResultFeatureFactory.SetFeatureFields: ", ex, true);
            }
        }

        private static void DeleteExistingFeature(IFeatureClass featureClass, ResultFeature resultFeature)
        {
            var whereClause = GetDeleteExistingWhereClause(resultFeature, featureClass);

            IQueryFilter queryFilter = FilterUtil.CreateQueryFilter(featureClass, whereClause);

            IFeatureCursor selectFeatureCursor = featureClass.Search(queryFilter, false);
            IFeature existingFeature = selectFeatureCursor.NextFeature();
            while (existingFeature != null)
            {
                existingFeature.Delete();
                while (Marshal.ReleaseComObject(existingFeature) > 0) { }
                existingFeature = selectFeatureCursor.NextFeature();
            }
            if (queryFilter != null) { while (Marshal.ReleaseComObject(queryFilter) > 0) { } }
            if (selectFeatureCursor != null) { while (Marshal.ReleaseComObject(selectFeatureCursor) > 0) { } }
        }

        private static string GetDeleteExistingWhereClause(ResultFeature resultFeature, IFeatureClass featureClass)
        {
            string whereClause = FilterUtil.CreateSimpleWhereClause(featureClass,
                "ZONE", resultFeature.Zone);
            whereClause = whereClause + " AND " + FilterUtil.CreateSimpleWhereClause(featureClass,
                "ZONETYPE", resultFeature.ZoneType);
            whereClause = whereClause + " AND " + FilterUtil.CreateSimpleWhereClause(featureClass,
                "CIRCUITID", resultFeature.CircuitId);

            return whereClause;
        }

        public void GetResultFeatureFields(ref IFields fields)
        {
            IFieldsEdit fieldsEdit = (IFieldsEdit)fields; // Explicit Cast

            AddField(ref fieldsEdit, "CId", esriFieldType.esriFieldTypeString, 9);
            AddField(ref fieldsEdit, "CName", esriFieldType.esriFieldTypeString, 50);
            AddField(ref fieldsEdit, "GlobalId", esriFieldType.esriFieldTypeString, 50);
            AddField(ref fieldsEdit, "Zone", esriFieldType.esriFieldTypeInteger, null);
            AddField(ref fieldsEdit, "DGlobalId", esriFieldType.esriFieldTypeString, 50);
            AddField(ref fieldsEdit, "DOPNUM", esriFieldType.esriFieldTypeString, 50);
            AddField(ref fieldsEdit, "DType", esriFieldType.esriFieldTypeString, 50);
            AddField(ref fieldsEdit, "OPMTap", esriFieldType.esriFieldTypeDouble, 9);
            AddField(ref fieldsEdit, "OPMNoTap", esriFieldType.esriFieldTypeDouble, 9);
            AddField(ref fieldsEdit, "UPMTap", esriFieldType.esriFieldTypeDouble, 9);
            AddField(ref fieldsEdit, "UPMNoTap", esriFieldType.esriFieldTypeDouble, 9);
            AddField(ref fieldsEdit, "ZoneType", esriFieldType.esriFieldTypeString, 50);
            AddField(ref fieldsEdit, "FTTap", esriFieldType.esriFieldTypeString, 50);
            AddField(ref fieldsEdit, "FTNoTap", esriFieldType.esriFieldTypeString, 50);

            fields = (IFields)fieldsEdit; // Explicit Cast
        }

        private static void AddField(ref IFieldsEdit fieldsEdit, string fieldName, esriFieldType fieldType, int? fieldLength)
        {
            IField field = new FieldClass(); // create a user defined text field

            try
            {
                IFieldEdit fieldEdit = (IFieldEdit)field; // Explicit Cast

                // setup field properties
                fieldEdit.Name_2 = fieldName;
                fieldEdit.Type_2 = fieldType;
                fieldEdit.IsNullable_2 = true;
                fieldEdit.AliasName_2 = fieldName;
                fieldEdit.Editable_2 = true;
                if (fieldLength != null)
                {
                    fieldEdit.Length_2 = (int)fieldLength;
                }

                fieldsEdit.AddField(field);
            }
            catch (Exception)
            {
                Logger.Error("An error occurred adding field: " + fieldName, true);
            }
        }

        public void WriteFeatureToCsv(ResultFeature resultFeature, string reportFileLocation, string reportFileName)
        {
            string csvFile = null;

            if (Directory.Exists(reportFileLocation) == false)
            {
                Directory.CreateDirectory(@reportFileLocation);
            }

            if (Directory.Exists(@reportFileLocation) == true)
            {
                csvFile = string.Format("{0}\\{1}.csv", reportFileLocation, reportFileName);
            }

            if (string.IsNullOrEmpty(csvFile) == true)
            {
                return;
            }

            bool appendToFile = (File.Exists(csvFile) == true);

            try
            {
                using (var writer = new CsvWriter.CsvFileWriter(csvFile, appendToFile))
                {
                    var row = new CsvWriter.CsvRow();

                    if (appendToFile == false)
                    {
                        row.Add("CircuitId");
                        row.Add("CircuitName");
                        row.Add("DeviceType");
                        row.Add("DeviceGuid");
                        row.Add("DeviceOperatingNumber");
                        row.Add("Status");
                        row.Add("Position");
                        row.Add("Dpds");
                        row.Add("Zone");
                        row.Add("PrimaryOhMiles");
                        row.Add("NumCustomers");
                        row.Add("NumCustomersEssential");
                        row.Add("NumCustomersSensitive");
                        row.Add("NumCustomersLifeSupport");
                        row.Add("T1PrimaryOhMiles");
                        row.Add("T2PrimaryOhMiles");
                        row.Add("T3PrimaryOhMiles");
                        row.Add("FireIndex");
                        row.Add("FireTier");

                        writer.WriteRow(row);
                    }

                    row = new CsvWriter.CsvRow();

                    row.Add(resultFeature.CircuitId);
                    row.Add(resultFeature.CircuitName);
                    row.Add(resultFeature.DeviceType);
                    row.Add(resultFeature.DeviceGlobalId.ToString());
                    row.Add(resultFeature.DeviceOperatingNumber);
                    row.Add(resultFeature.Status);
                    row.Add(resultFeature.Position);
                    row.Add(resultFeature.ListOfDpds);
                    row.Add(resultFeature.Zone.ToString());
                    row.Add(resultFeature.PrimaryOhMiles.ToString());
                    row.Add(resultFeature.CustomersInZone.ToString());
                    row.Add(resultFeature.CustomersEssentialInZone.ToString());
                    row.Add(resultFeature.CustomersSensitiveInZone.ToString());
                    row.Add(resultFeature.CustomersLifeSupportInZone.ToString());
                    row.Add(resultFeature.T1PrimaryOhMiles.ToString());
                    row.Add(resultFeature.T2PrimaryOhMiles.ToString());
                    row.Add(resultFeature.T3PrimaryOhMiles.ToString());
                    row.Add(resultFeature.FireIndex.ToString());
                    row.Add(resultFeature.FireTier);

                    writer.WriteRow(row);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        
        public int GetCustomerCount(IEnumNetEID junctions, INetwork network, out int lifeSupportCustomerCount, out int sensitiveCustomerCount, out int essentialCustomerCount,
            out int DOMCustomerCount, out int AGRCustomerCount, out int INDCustomerCount, out int COMCustomerCount, out int OTHCustomerCount)
        {
            int customerCount = 0;
            essentialCustomerCount = 0;
            sensitiveCustomerCount = 0;
            lifeSupportCustomerCount = 0;
            DOMCustomerCount = 0;
            AGRCustomerCount = 0;
            INDCustomerCount = 0;
            COMCustomerCount = 0;
            OTHCustomerCount = 0;

            try
            {
                int userClassId;
                int userId;
                int userSubId;

                var netElements = (INetElements)network;

                IFeatureClass serviceLocationFeatClass =
                    Geodatabase.GetFeatureClass(Settings.Default.ServiceLocationFeatureClassName);
                ITable servicePointTable = Geodatabase.GetTable(Settings.Default.ServicePointTableName);

                List<int> featureIds = new List<int>();

                junctions.Reset();
                int eid = junctions.Next();

                while (eid > 0)
                {
                    netElements.QueryIDs(eid, esriElementType.esriETJunction, out userClassId, out userId, out userSubId);

                    if (userClassId == serviceLocationFeatClass.FeatureClassID)
                    {
                        featureIds.Add(userId);
                    }
                    eid = junctions.Next();
                }

                if (featureIds.Count > 0)
                {
                    //Prepare our list of service location global IDs that were included in this trace
                    List<string> globalIDs = new List<string>();
                    IQueryFilter SLQf = new QueryFilterClass();
                    List<string> SLWhereInClauses = new List<string>();
                    SLWhereInClauses = GetWhereInClauses(featureIds);
                    SLQf.SubFields = "GLOBALID";
                    int globalIDIdx = serviceLocationFeatClass.FindField("GLOBALID");
                    foreach (string whereInClause in SLWhereInClauses)
                    {
                        SLQf.WhereClause = string.Format("{0} in ({1})", serviceLocationFeatClass.OIDFieldName,
                            whereInClause);
                        IFeatureCursor cursor = serviceLocationFeatClass.Search(SLQf, false);
                        IFeature serviceLocation = null;
                        while ((serviceLocation = cursor.NextFeature()) != null)
                        {
                            globalIDs.Add(serviceLocation.get_Value(globalIDIdx).ToString());
                            while ((Marshal.ReleaseComObject(serviceLocation)) > 0) { }
                        }

                        if (cursor != null)
                        {
                            while (Marshal.ReleaseComObject(cursor) > 0) { }
                        }
                    }
                    if (SLQf != null)
                    {
                        while (Marshal.ReleaseComObject(SLQf) > 0) { }
                    }

                    int essentialCustomerIdx = servicePointTable.FindField(Settings.Default.EssentialCustomerFieldName);
                    int sensitiveCustomerIdx = servicePointTable.FindField(Settings.Default.SensitiveCustomerFieldName);
                    int lifeSupportCustomerIdx = servicePointTable.FindField(Settings.Default.LifeSupportCustomerFieldName);
                    int customerTypeIdx = servicePointTable.FindField(Settings.Default.CustomerTypeFieldName);

                    //Now we want to query for all service points associated with this list of global IDs                    
                    List<string> whereInClauses = GetWhereInClauses(globalIDs);
                    foreach (string whereInClause in whereInClauses)
                    {
                        IQueryFilter qf = new QueryFilterClass();
                        string initialWhereClause = string.Format("{0} in ({1})", Settings.Default.ServicePointForiegnKeyFieldName,
                            whereInClause);
                        
                        qf.SubFields = string.Format("{0},{1},{2},{3}", Settings.Default.EssentialCustomerFieldName, Settings.Default.SensitiveCustomerFieldName,
                            Settings.Default.LifeSupportCustomerFieldName, Settings.Default.CustomerTypeFieldName);

                        qf.WhereClause = initialWhereClause;

                        /*
                        customerCount += servicePointTable.RowCount(qf);

                        qf.WhereClause = initialWhereClause + string.Format(" AND {0} = '{1}'", Settings.Default.EssentialCustomerFieldName, "Y");
                        essentialCustomerCount += servicePointTable.RowCount(qf);

                        qf.WhereClause = initialWhereClause + string.Format(" AND {0} = '{1}'", Settings.Default.SensitiveCustomerFieldName, "Y");
                        sensitiveCustomerCount += servicePointTable.RowCount(qf);

                        qf.WhereClause = initialWhereClause + string.Format(" AND {0} = '{1}'", Settings.Default.LifeSupportCustomerFieldName, "Y");
                        lifeSupportCustomerCount += servicePointTable.RowCount(qf);

                        qf.WhereClause = initialWhereClause + string.Format(" AND {0} = '{1}'", Settings.Default.CustomerTypeFieldName, "DOM");
                        DOMCustomerCount += servicePointTable.RowCount(qf);

                        qf.WhereClause = initialWhereClause + string.Format(" AND {0} = '{1}'", Settings.Default.CustomerTypeFieldName, "AGR");
                        AGRCustomerCount += servicePointTable.RowCount(qf);

                        qf.WhereClause = initialWhereClause + string.Format(" AND {0} = '{1}'", Settings.Default.CustomerTypeFieldName, "IND");
                        INDCustomerCount += servicePointTable.RowCount(qf);

                        qf.WhereClause = initialWhereClause + string.Format(" AND {0} = '{1}'", Settings.Default.CustomerTypeFieldName, "COM");
                        COMCustomerCount += servicePointTable.RowCount(qf);

                        qf.WhereClause = initialWhereClause + string.Format(" AND {0} = '{1}'", Settings.Default.CustomerTypeFieldName, "OTH");
                        OTHCustomerCount += servicePointTable.RowCount(qf);
                        */
                        
                        ICursor servicePointCursor = servicePointTable.Search(qf, false);
                        IRow row = null;
                        while ((row = servicePointCursor.NextRow()) != null)
                        {
                            customerCount++;
                            object essentialCustomer = row.get_Value(essentialCustomerIdx);
                            object sensitiveCustomer = row.get_Value(sensitiveCustomerIdx);
                            object lifeSupportCustomer = row.get_Value(lifeSupportCustomerIdx);
                            object customerType = row.get_Value(customerTypeIdx);

                            if (essentialCustomer != DBNull.Value && essentialCustomer.ToString() == "Y") { essentialCustomerCount++; }
                            if (sensitiveCustomer != DBNull.Value && sensitiveCustomer.ToString() == "Y") { sensitiveCustomerCount++; }
                            if (lifeSupportCustomer != DBNull.Value && lifeSupportCustomer.ToString() == "Y") { lifeSupportCustomerCount++; }

                            if (customerType != DBNull.Value && customerType.ToString() == "DOM") { DOMCustomerCount++; }
                            else if (customerType != DBNull.Value && customerType.ToString() == "AGR") { AGRCustomerCount++; }
                            else if (customerType != DBNull.Value && customerType.ToString() == "IND") { INDCustomerCount++; }
                            else if (customerType != DBNull.Value && customerType.ToString() == "COM") { COMCustomerCount++; }
                            else if (customerType != DBNull.Value && customerType.ToString() == "OTH") { OTHCustomerCount++; }

                            while (Marshal.ReleaseComObject(row) > 0) { }
                        }

                        while (Marshal.ReleaseComObject(servicePointCursor) > 0) { }
                        while (Marshal.ReleaseComObject(qf) > 0) { }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to determine customer counts: " + ex.Message + " Stacktrace: " +
                                    ex.StackTrace);
            }
            return customerCount;
        }
    }

    public class FireTierAndIndexInformation
    {
        public int FireIndex = -1;
        public string FireTier = "Tier 1";

        public SortedDictionary<int, double> OHMilesPerFireTier = new SortedDictionary<int, double>();
        public SortedDictionary<int, double> UGMilesPerFireTier = new SortedDictionary<int, double>();

        public SortedDictionary<int, double> OHMilesPerFireIndex = new SortedDictionary<int, double>();
        public SortedDictionary<int, double> UGMilesPerFireIndex = new SortedDictionary<int, double>();
        public SortedDictionary<int, double> AllMilesPerFireIndex = new SortedDictionary<int, double>();

        public FireTierAndIndexInformation()
        {
            OHMilesPerFireTier.Add(1, 0.0);
            OHMilesPerFireTier.Add(2, 0.0);
            OHMilesPerFireTier.Add(3, 0.0);

            UGMilesPerFireTier.Add(1, 0.0);
            UGMilesPerFireTier.Add(2, 0.0);
            UGMilesPerFireTier.Add(3, 0.0);
        }

        public void SetFireTierPrimaryMiles(bool isOverhead, int tier, double length)
        {
            if (isOverhead)
            {
                OHMilesPerFireTier[tier] = length;
            }
            else
            {
                UGMilesPerFireTier[tier] = length;
            }
        }

        public void SetFireIndexPrimaryMiles(bool isOverhead, int FireIndex, double length)
        {
            if (isOverhead)
            {
                OHMilesPerFireIndex.Add(FireIndex, length);
            }
            else
            {
                UGMilesPerFireIndex.Add(FireIndex, length);
            }

            if (!AllMilesPerFireIndex.ContainsKey(FireIndex)) { AllMilesPerFireIndex.Add(FireIndex, length); }
            else { AllMilesPerFireIndex[FireIndex] += length; }

            FireIndex = -1;
            double maxLength = 0.0;
            foreach (KeyValuePair<int, double> kvp in AllMilesPerFireIndex)
            {
                if (kvp.Value > maxLength)
                {
                    FireIndex = kvp.Key;
                    maxLength = kvp.Value;
                }
            }
        }

        public double GetPrimaryMiles(bool isOverhead)
        {
            double length = 0.0;
            if (isOverhead)
            {
                foreach (KeyValuePair<int, double> kvp in OHMilesPerFireTier)
                {
                    length += kvp.Value;
                }
            }
            else
            {
                foreach (KeyValuePair<int, double> kvp in UGMilesPerFireTier)
                {
                    length += kvp.Value;
                }
            }
            return length;
        }

    }
}