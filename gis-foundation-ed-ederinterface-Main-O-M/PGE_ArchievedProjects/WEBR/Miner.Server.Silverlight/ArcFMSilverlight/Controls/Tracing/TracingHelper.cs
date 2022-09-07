using System;
using System.Net;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using ESRI.ArcGIS.Client;
using ESRI.ArcGIS.Client.Geometry;
using ESRI.ArcGIS.Client.Tasks;
using System.Windows;
using ESRI.ArcGIS.Client.Symbols;
using System.Collections.Generic;
using ESRI.ArcGIS.Client.FeatureService;
using System.Text.RegularExpressions;
using System.Linq;
using System.Collections;

namespace ArcFMSilverlight.Controls.Tracing
{
    public class TracingHelper
    {
        public static string PGEElectricTracingURL = "";
        public static string PGESubstationTracingURL = "";
        public static string PGEElecCachedTracingTableLayerID = "";
        public static string PGESubCachedTracingTableLayerID = "";
        public static string LoadingInformationTracingTableURL = "";
        public static string LoadingInformationTracingTableLayerID = "";
        public static List<int> ClassIDsToDisplaySubtype = new List<int>();
        public static List<string> PGEElectricTracingURLs = new List<string>();
        public static List<string> PGESubstationTracingURLs = new List<string>();
        public static List<string> PGESchemTracingURLs = new List<string>();
        public List<TaskBase> CurrentlyExecutingTasks = new List<TaskBase>();
        public static Dictionary<string, int> PGEProtectiveDevicesByClassID = new Dictionary<string, int>();
        public static Dictionary<int, List<string>> PGETraceResultFieldsByClassID = new Dictionary<int, List<string>>();
        public static Dictionary<int, TraceRelRecordCheck> PGETraceResultRelatedCountsByClassID = new Dictionary<int, TraceRelRecordCheck>();
        public static List<int> DefaultVisibleClassIDs = new List<int>();
        public static Dictionary<int, int> PGESchemProtDeviceLookup = new Dictionary<int, int>();
        public static string FieldIdentifierName = "GLOBALID";
        public static string SchemFieldIdentifierName = "UGUID";
        public static string SchemIDFieldName = "ID";
        public static string ObjectIDFieldName = "OBJECTID";
        public static string OperatingNumberFieldName = "OPERATINGNUMBER";
        public static string StructureNumberFieldName = "STRUCTURENUMBER";
        public string CurrentResultsFeederID = "";
        public static string ConduitFCID = "-1";
        public static string SubsurfaceStructureFCID = "-1";
        public static string UndergroundNetworkURL = "";
        public static List<string> UndergroundSubsurfaceStructureSubtypes = new List<string>();
        public static string UndergroundNetworkLayerID = "";

        //ClassID - List of GlobalIDs
        public Dictionary<int, List<TraceItem>> CurrentFullTraceResultsByClassID = new Dictionary<int, List<TraceItem>>();
        //ClassID - Global ID
        public List<TraceItem> CurrentFullTraceResultsInOrder = new List<TraceItem>();
        public List<TraceItem> CurrentProtectiveDeviceTraceResultsInOrder = new List<TraceItem>();
        public bool CurrentResultsAreSchematics = false;
        public bool CurrentResultsAreUpstream = false;
        public bool CurrentResultsAreProtective = false;
        public bool CurrentResultsAreConduit = false;
        private List<int> ProtectiveDeviceClassIDs = new List<int>();
        private bool IsProtectiveDeviceTrace = false;
        private bool IsUndergroundNetworkTrace = false;
        private bool IsUpstreamTrace = true;
        private bool IsDownstreamTrace = false;
        private bool TraceAPhase = true;
        private bool TraceBPhase = true;
        private bool TraceCPhase = true;
        public bool TraceFeederFedFeeders = false;

        private GraphicsLayer pgeTraceGraphicsLayer;
        private Map FocusMap = null;
        private bool IsElectric = false;
        private bool IsSchematics = false;
        private bool IsSubstation = false;

        public string currentIdentifyUrl;
        public string currentTracingTableUrl;
        public string currentCachedTracingTableLayerId;
        public string currentFCIDFieldName;

        private static string PriUGConductorFCID = "1021";
        private static string PriOHConductorFCID = "1023";

        //Dictionary variable to hold layer name and alias name with class id -- currently used in case of trace result for circuit point
        public static Dictionary<string, string> PGETraceResultLayerAliasNames = new Dictionary<string, string>();

        private PGE_Tracing_Control TracingControl = null;

        //ME Q3 2019 Release - DA# 190501
        private string _ServiceLocation_FEATURE_FCID = "1001,1014";
        private string _Start_TO_FEATURE_FCID = "0";

        //ME Q4 2019 Release DA# 190806
        public static Dictionary<string, List<string>> PGETraceResultCustomDisplayField = new Dictionary<string, List<string>>();

        public TracingHelper()
        {

        }

        public TracingHelper(PGE_Tracing_Control tracingControl)
        {
            TracingControl = tracingControl;
        }

        public struct TraceRelRecordCheck
        {
            public string RelationshipName;
            public string FieldName;
            public Dictionary<int, string> FieldValues;
        }

        private static Object TraceItemsObjectLock = new Object();
        private static int CurrentTraceItemCount = 0;
        private static List<TraceItem> CurrentTraceItems = new List<TraceItem>();
        public static void GetTraceItemInformation(TraceItem item, bool isSchematics)
        {
            if (item.FieldInformationObtained) { return; }

            //Determine the URL for the current trace item.
            List<int> layerID = ConfigUtility.GetLayerIDFromClassID(TracingHelper.PGEElectricTracingURL, item.ClassID);

            if (layerID.Count < 1) { return; }

            List<string> fieldsToObtain = new List<string>();
            if (!PGETraceResultFieldsByClassID.ContainsKey(item.ClassID))
            {
                item.TracingItemInformation.Clear();
                return;
            }
            else
            {
                fieldsToObtain = PGETraceResultFieldsByClassID[item.ClassID];
            }

            //Query for our trace from the database table.
            QueryTask getTraceItemInformationQuery = new QueryTask(TracingHelper.PGEElectricTracingURL + "/" + layerID[0]);
            getTraceItemInformationQuery.ExecuteCompleted += new EventHandler<QueryEventArgs>(getTraceItemInformationQuery_ExecuteCompleted);
            getTraceItemInformationQuery.Failed += new EventHandler<TaskFailedEventArgs>(getTraceItemInformationQuery_Failed);
            Query query = new Query();

            query.OutFields.AddRange(fieldsToObtain);

            if (!fieldsToObtain.Contains("GLOBALID") && !isSchematics) { query.OutFields.Add("GLOBALID"); }
            else if (!fieldsToObtain.Contains("UGUID") && isSchematics) { query.OutFields.Add("UGUID"); }

            if (isSchematics) { query.Where = "UGUID = '" + item.GlobalID + "'"; }
            else { query.Where = "GLOBALID = '" + item.GlobalID + "'"; }

            lock (TraceItemsObjectLock)
            {
                CurrentTraceItemCount++;
                CurrentTraceItems.Add(item);
            }
            getTraceItemInformationQuery.ExecuteAsync(query);

            //Now we also need to get the relationship counts for any configured relationship checks.
            foreach (KeyValuePair<int, TraceRelRecordCheck> kvp in TracingHelper.PGETraceResultRelatedCountsByClassID)
            {
                if (kvp.Key != item.ClassID) { continue; }
                Relationship rel = ConfigUtility.GetRelationshipByName(TracingHelper.PGEElectricTracingURL, layerID[0], kvp.Value.RelationshipName);
                if (rel != null)
                {
                    //Query for our trace from the database table.
                    QueryTask getTraceItemRelInformationQuery = new QueryTask(TracingHelper.PGEElectricTracingURL + "/" + rel.RelatedTableId);
                    getTraceItemRelInformationQuery.ExecuteCompleted += new EventHandler<QueryEventArgs>(getTraceItemInformationQuery_ExecuteCompleted);
                    getTraceItemRelInformationQuery.Failed += new EventHandler<TaskFailedEventArgs>(getTraceItemInformationQuery_Failed);
                    Relationship relLookup = ConfigUtility.GetRelationshipByRelatedLayerID(TracingHelper.PGEElectricTracingURL, rel.RelatedTableId, layerID[0]);

                    Query relQuery = new Query();

                    relQuery.OutFields.Add(relLookup.KeyField);
                    relQuery.Where = relLookup.KeyField + " = '" + item.GlobalID + "'";

                    lock (TraceItemsObjectLock)
                    {
                        CurrentTraceItemCount++;
                    }
                    getTraceItemRelInformationQuery.ExecuteAsync(relQuery);
                }
            }
        }

        /// <summary>
        /// Notifies the user in the status update if obtaining the additional information for a TraceItem fails.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void getTraceItemInformationQuery_Failed(object sender, TaskFailedEventArgs e)
        {
            ConfigUtility.StatusBar.Text = "Failed obtaining additional information: " + e.Error;
            ConfigUtility.StatusBar.UpdateLayout();

            lock (TraceItemsObjectLock)
            {
                CurrentTraceItemCount--;
            }
        }

        /// <summary>
        /// This method will apply the additional information for the correct TraceItem information. When applied,
        /// it will be visible to the user in the trace results window when the node is expanded.
        /// </summary>
        /// <param name="sender">QueryTask</param>
        /// <param name="e">QueryEventArgs</param>
        private static void getTraceItemInformationQuery_ExecuteCompleted(object sender, QueryEventArgs e)
        {
            lock (TraceItemsObjectLock)
            {
                bool relatedRecord = true;
                foreach (Field field in e.FeatureSet.Fields)
                {
                    if (field.Name.ToUpper() == TracingHelper.FieldIdentifierName || field.Name.ToUpper() == TracingHelper.SchemFieldIdentifierName)
                    {
                        relatedRecord = false;
                        break;
                    }
                }
                QueryTask qt = sender as QueryTask;
                if (relatedRecord)
                {
                    //This is a related table query so we are just going to count up the records found and then
                    //add the ficticious field as configured.
                    int recordCount = 0;
                    string fieldValue = "";

                    //The global ID and related table field name should have been passed in as the token.
                    string globalID = "";

                    foreach (Graphic graphic in e.FeatureSet.Features)
                    {
                        foreach (KeyValuePair<string, object> kvp in graphic.Attributes)
                        {
                            //There should have only been one field added the query and it would be the related field of type guid
                            globalID = kvp.Value.ToString();
                            break;
                        }
                    }

                    recordCount = e.FeatureSet.Features.Count;
                    TraceItem currentTraceItem = null;
                    foreach (TraceItem item in CurrentTraceItems)
                    {
                        if (item.GlobalID == globalID)
                        {
                            if (TracingHelper.PGETraceResultRelatedCountsByClassID.ContainsKey(item.ClassID))
                            {
                                TraceRelRecordCheck relRecordCheck = TracingHelper.PGETraceResultRelatedCountsByClassID[item.ClassID];
                                if (relRecordCheck.FieldValues.ContainsKey(recordCount))
                                {
                                    fieldValue = relRecordCheck.FieldValues[recordCount];
                                }
                                TraceItemInfo itemInfo = new TraceItemInfo(relRecordCheck.FieldName, fieldValue);
                                item.TracingItemInformation.Add(itemInfo);
                            }
                            break;
                        }
                    }
                }
                else
                {
                    TraceItem CurrentTraceItem = null;
                    foreach (Graphic result in e.FeatureSet.Features)
                    {
                        if (result.Attributes.ContainsKey(TracingHelper.FieldIdentifierName))
                        {
                            foreach (TraceItem item in CurrentTraceItems)
                            {
                                if (item.GlobalID == result.Attributes[TracingHelper.FieldIdentifierName].ToString())
                                {
                                    CurrentTraceItem = item;
                                    break;
                                }
                            }
                        }
                        else if (result.Attributes.ContainsKey(TracingHelper.SchemFieldIdentifierName))
                        {
                            foreach (TraceItem item in CurrentTraceItems)
                            {
                                string compareGUID = result.Attributes[TracingHelper.SchemFieldIdentifierName].ToString().ToUpper();
                                if (!compareGUID.Contains("{")) { compareGUID = "{" + compareGUID + "}"; }
                                if (item.GlobalID == compareGUID)
                                {
                                    CurrentTraceItem = item;
                                    break;
                                }
                            }
                        }
                        //Determine the URL for the current trace item.
                        List<int> layerID = ConfigUtility.GetLayerIDFromClassID(TracingHelper.PGEElectricTracingURL, CurrentTraceItem.ClassID);
                        if (layerID.Count < 1) { return; }

                        List<string> fieldsToObtain = new List<string>();
                        int currentTraceItemClassID = CurrentTraceItem.ClassID;
                        if (PGETraceResultFieldsByClassID.ContainsKey(currentTraceItemClassID))
                        {
                            fieldsToObtain = PGETraceResultFieldsByClassID[currentTraceItemClassID];
                        }

                        CurrentTraceItem.TracingItemInformation.Clear();

                        string subtypeFieldName = ConfigUtility.GetSubtypeFieldName(currentTraceItemClassID).ToUpper();
                        string subtypeCode = "";
                        if (result.Attributes.ContainsKey(subtypeFieldName))
                        {
                            subtypeCode = result.Attributes[subtypeFieldName].ToString();
                        }

                        foreach (string fieldName in fieldsToObtain)
                        {
                            string fieldNameAlias = fieldName;
                            fieldNameAlias = ConfigUtility.GetFieldNameAlias(TracingHelper.PGEElectricTracingURL, layerID[0], fieldName);
                            if (result.Attributes.ContainsKey(fieldName))
                            {
                                string domainDescription = "";
                                if (result.Attributes[fieldName] != null) { domainDescription = ConfigUtility.GetDomainDescription(TracingHelper.PGEElectricTracingURL, layerID[0], subtypeCode, fieldName, result.Attributes[fieldName].ToString()); }
                                CurrentTraceItem.TracingItemInformation.Add(new TraceItemInfo(fieldNameAlias, domainDescription));
                            }
                        }
                    }
                }

                CurrentTraceItemCount--;
                if (CurrentTraceItemCount < 1)
                {
                    int recordCount = 0;
                    string fieldValue = "";
                    //Finished processing so the rest of the objects in the rel trace items list are 0
                    foreach (TraceItem item in CurrentTraceItems)
                    {
                        if (TracingHelper.PGETraceResultRelatedCountsByClassID.ContainsKey(item.ClassID))
                        {
                            TraceRelRecordCheck relRecordCheck = TracingHelper.PGETraceResultRelatedCountsByClassID[item.ClassID];
                            bool fieldAlreadyAdded = false;
                            foreach (TraceItemInfo infoObject in item.TracingItemInformation)
                            {
                                if (infoObject.FieldName == relRecordCheck.FieldName)
                                {
                                    fieldAlreadyAdded = true;
                                    break;
                                }
                            }
                            if (fieldAlreadyAdded) { continue; }

                            if (relRecordCheck.FieldValues.ContainsKey(recordCount))
                            {
                                fieldValue = relRecordCheck.FieldValues[recordCount];
                            }
                            TraceItemInfo itemInfo = new TraceItemInfo(relRecordCheck.FieldName, fieldValue);

                            item.TracingItemInformation.Add(itemInfo);
                        }
                        item.FieldInformationObtained = true;
                    }
                    CurrentTraceItems.Clear();
                }
            }
        }

        /// <summary>
        /// Get the trace results from a specific point on the map where a user has clicked.  This method should only be called with a valid
        /// tracing control object as it will set the TraceResults when finished.
        /// </summary>
        /// <param name="focusMap">Current Map object</param>
        /// <param name="mapPoint">MapPoint where user has clicked</param>
        /// <param name="isUpstream">Is this an upstream trace?</param>
        /// <param name="traceAPhase">Trace A Phase?</param>
        /// <param name="traceBPhase">Trace B Phase?</param>
        /// <param name="traceCPhase">Trace C Phase?</param>
        public void GetTraceResults(Map focusMap, MapPoint mapPoint, bool isUpstream, bool isUndergroundNetworkTrace, bool traceAPhase, bool traceBPhase, bool traceCPhase)
        {
            StopCurrentAsyncTasks();

            TraceAPhase = traceAPhase;
            TraceBPhase = traceBPhase;
            TraceCPhase = traceCPhase;

            IsUpstreamTrace = isUpstream;
            IsDownstreamTrace = !isUpstream;

            IsProtectiveDeviceTrace = false;
            IsUndergroundNetworkTrace = isUndergroundNetworkTrace;
            ProtectiveDeviceClassIDs = new List<int>();
            CurrentFullTraceResultsInOrder.Clear();
            CurrentProtectiveDeviceTraceResultsInOrder.Clear();
            CurrentFullTraceResultsByClassID.Clear();
            FullTraceResultsByOrderNum.Clear();

            FocusMap = focusMap;

            if (LayerConfiguredForTracing())
            {
                GetTraceSettings();

                pgeTraceGraphicsLayer = focusMap.Layers["PGE_Trace_Graphics"] as GraphicsLayer;
                if (pgeTraceGraphicsLayer == null)
                {
                    pgeTraceGraphicsLayer = new GraphicsLayer();
                    pgeTraceGraphicsLayer.ID = "PGE_Trace_Graphics";
                    focusMap.Layers.Add(pgeTraceGraphicsLayer);
                }

                //Get the starting point
                GetStartingFeature(mapPoint);
            }
            else
            {
                TracingFinished();
            }
        }

        /// <summary>
        /// Get the trace results from a specific point on the map where a user has clicked.  This method should only be called with a valid
        /// tracing control object as it will set the TraceResults when finished.
        /// </summary>
        /// <param name="focusMap">Current Map object</param>
        /// <param name="mapPoint">MapPoint where user has clicked</param>
        /// <param name="protectiveDeviceClassIDs">List of class IDs that identify protective devices</param>
        /// <param name="isUpstream">Is this an upstream trace?</param>
        /// <param name="traceAPhase">Trace A Phase?</param>
        /// <param name="traceBPhase">Trace B Phase?</param>
        /// <param name="traceCPhase">Trace C Phase?</param>
        public void GetTraceResults(Map focusMap, MapPoint mapPoint, List<int> protectiveDeviceClassIDs, bool isUpstream, bool isUndergroundNetworkTrace,
            bool traceAPhase, bool traceBPhase, bool traceCPhase)
        {
            StopCurrentAsyncTasks();

            TraceAPhase = traceAPhase;
            TraceBPhase = traceBPhase;
            TraceCPhase = traceCPhase;

            IsUndergroundNetworkTrace = isUndergroundNetworkTrace;
            IsUpstreamTrace = isUpstream;
            IsDownstreamTrace = !isUpstream;

            CurrentFullTraceResultsInOrder.Clear();
            CurrentProtectiveDeviceTraceResultsInOrder.Clear();
            CurrentFullTraceResultsByClassID.Clear();
            FullTraceResultsByOrderNum.Clear();

            FocusMap = focusMap;

            if (LayerConfiguredForTracing())
            {
                GetTraceSettings();

                IsProtectiveDeviceTrace = true;
                if (IsSchematics)
                {
                    foreach (int classID in protectiveDeviceClassIDs)
                    {
                        if (PGESchemProtDeviceLookup.ContainsKey(classID))
                        {
                            ProtectiveDeviceClassIDs.Add(PGESchemProtDeviceLookup[classID]);
                        }
                    }
                }
                else
                {
                    ProtectiveDeviceClassIDs = protectiveDeviceClassIDs;
                }

                pgeTraceGraphicsLayer = focusMap.Layers["PGE_Trace_Graphics"] as GraphicsLayer;
                if (pgeTraceGraphicsLayer == null)
                {
                    pgeTraceGraphicsLayer = new GraphicsLayer();
                    pgeTraceGraphicsLayer.ID = "PGE_Trace_Graphics";
                    focusMap.Layers.Add(pgeTraceGraphicsLayer);
                }

                //Get the starting point
                GetStartingFeature(mapPoint);
            }
            else
            {
                TracingFinished();
            }
        }

        private bool LayerConfiguredForTracing()
        {
            int elecIndex = GetTracingUrl(PGEElectricTracingURLs);
            int schemIndex = GetTracingUrl(PGESchemTracingURLs);
            int subIndex = GetTracingUrl(PGESubstationTracingURLs);

            if (elecIndex < 0 && schemIndex < 0 && subIndex < 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        private void GetTraceSettings()
        {
            TraceType traceType = TraceType.Electric;
            int elecIndex = GetTracingUrl(PGEElectricTracingURLs);
            int schemIndex = GetTracingUrl(PGESchemTracingURLs);
            int subIndex = GetTracingUrl(PGESubstationTracingURLs);
            if (elecIndex >= 0) { traceType = TraceType.Electric; }
            else if (schemIndex >= 0) { traceType = TraceType.Schematics; }
            else if (subIndex >= 0) { traceType = TraceType.Substation; }

            if (traceType == TraceType.Electric)
            {
                IsSubstation = false;
                IsSchematics = false;
                IsElectric = true;

                currentTracingTableUrl = PGEElectricTracingURL;
                currentIdentifyUrl = PGEElectricTracingURLs[elecIndex];
                currentCachedTracingTableLayerId = PGEElecCachedTracingTableLayerID;
                currentFCIDFieldName = "TO_FEATURE_FCID";
            }
            else if (traceType == TraceType.Schematics)
            {
                IsSubstation = false;
                IsElectric = false;
                IsSchematics = true;

                currentTracingTableUrl = PGEElectricTracingURL;
                currentIdentifyUrl = PGESchemTracingURLs[schemIndex];
                currentCachedTracingTableLayerId = PGEElecCachedTracingTableLayerID;
                currentFCIDFieldName = "TO_FEATURE_SCHEM_FCID";
            }
            else
            {
                IsElectric = false;
                IsSchematics = false;
                IsSubstation = true;

                currentTracingTableUrl = PGESubstationTracingURL;
                currentIdentifyUrl = PGESubstationTracingURLs[subIndex];
                currentCachedTracingTableLayerId = PGESubCachedTracingTableLayerID;
                currentFCIDFieldName = "TO_FEATURE_FCID";
            }
        }

        /// <summary>
        /// Returnes the index of the list where the URL exists in the current map layers. 
        /// </summary>
        /// <param name="possibleURLs">List of URLs to search</param>
        /// <returns></returns>
        private int GetTracingUrl(List<string> possibleURLs)
        {
            foreach (Layer layer in FocusMap.Layers)
            {
                if (layer is ArcGISDynamicMapServiceLayer && layer.Visible)
                {
                    ArcGISDynamicMapServiceLayer dynamicLayer = layer as ArcGISDynamicMapServiceLayer;
                    for (int i = 0; i < possibleURLs.Count; i++)
                    {
                        string url = possibleURLs[i];
                        if (url.ToUpper() == dynamicLayer.Url.ToUpper())
                        {
                            return i;
                        }
                    }
                }
            }
            return -1;
        }

        private void GetStartingFeature(MapPoint mapPoint)
        {
            //First we need to buffer our point click by using the geometry service.
            BufferParameters bufferParams = new BufferParameters();
            bufferParams.BufferSpatialReference = mapPoint.SpatialReference;
            //Adjust the buffer point per the map scale.
            double bufferSize = FocusMap.Scale * .004;
            if (bufferSize > 10) { bufferSize = 10; }
            bufferParams.Distances.Add(bufferSize);
            Graphic graphic = new Graphic();
            graphic.Geometry = mapPoint;
            bufferParams.Features.Add(graphic);

            //Make the geometry service call
            GeometryService geomService = new GeometryService(ConfigUtility.GeometryServiceURL);
            geomService.BufferCompleted += new EventHandler<GraphicsEventArgs>(geomService_BufferCompleted);
            geomService.Failed += new EventHandler<TaskFailedEventArgs>(geomService_Failed);
            CurrentlyExecutingTasks.Add(geomService);
            geomService.BufferAsync(bufferParams);
        }

        void geomService_Failed(object sender, TaskFailedEventArgs e)
        {
            ConfigUtility.UpdateStatusBarText("Failed buffering starting point: " + e.Error.Message);
        }

        void geomService_BufferCompleted(object sender, GraphicsEventArgs e)
        {
            //Draw the resultant buffer graphic to the screen.  This can be removed later
            pgeTraceGraphicsLayer.Graphics.Clear();
            SimpleFillSymbol markerSymbol = new SimpleFillSymbol();
            SolidColorBrush brush = new System.Windows.Media.SolidColorBrush();
            brush.Opacity = 0.5;
            brush.Color = Colors.Red;
            markerSymbol.Fill = brush;
            Graphic bufferResult = e.Results[0];
            bufferResult.Symbol = markerSymbol;
            pgeTraceGraphicsLayer.Graphics.Add(bufferResult);


            IdentifyTask identifyTask = new IdentifyTask(currentIdentifyUrl);
            identifyTask.ExecuteCompleted += new EventHandler<IdentifyEventArgs>(identifyTask_ExecuteCompleted);
            identifyTask.Failed += new EventHandler<TaskFailedEventArgs>(identifyTask_Failed);

            IdentifyParameters identifyParams = new IdentifyParameters();
            identifyParams.LayerOption = LayerOption.visible;
            identifyParams.MapExtent = FocusMap.Extent;
            identifyParams.Width = (int)FocusMap.ActualWidth;
            identifyParams.Height = (int)FocusMap.ActualHeight;
            identifyParams.Geometry = bufferResult.Geometry;
            CurrentlyExecutingTasks.Add(identifyTask);
            identifyTask.ExecuteAsync(identifyParams);
        }

        void identifyTask_Failed(object sender, TaskFailedEventArgs e)
        {
            ConfigUtility.UpdateStatusBarText("Failed identifying starting feature: " + e.Error.Message);
        }

        void identifyTask_ExecuteCompleted(object sender, IdentifyEventArgs e)
        {
            try
            {
                Graphic startGraphic = null;
                IdentifyResult result = null;

                for (int i = 0; i < e.IdentifyResults.Count; i++)
                {
                    result = e.IdentifyResults[i];

                    //if (result.Feature.Geometry is MapPoint) { continue; }

                    if (!ConfigUtility.IsLayerVisible(currentIdentifyUrl, FocusMap, result.LayerId)) { continue; }

                    string layerName = result.LayerName;
                    bool isCircuit = false;

                    foreach (KeyValuePair<string, object> kvp in result.Feature.Attributes)
                    {
                        string fieldNameTest = kvp.Key.ToUpper().Replace(" ", "");
                        if (fieldNameTest == "CIRCUITID")
                        {
                            isCircuit = true;
                        }
                    }

                    if (isCircuit)
                    {
                        if (!(result.Feature.Geometry is MapPoint)) { startGraphic = result.Feature; }
                    }

                    //If our point graphic is not null then we should just start from the point graphic.
                    if (startGraphic != null) { break; }
                }

                //If we found a point feature then let's start from there.  Otherwise we can start from a line feature
                if (startGraphic != null)
                {
                    GetTraceResults(startGraphic, result.LayerId, IsSchematics);
                }
                else
                {
                    CurrentResultsFeederID = "";
                    TracingFinished();
                }
            }
            catch (Exception ex)
            {

            }
        }

        #region Trace for features matching FCID list

        private Action<Dictionary<int, List<TraceItem>>> CurrentTraceCallback = null;
        private Dictionary<int, List<TraceItem>> CurrentTraceCallbackResults = new Dictionary<int, List<TraceItem>>();
        private string CurrentTraceFCIDWhereClause = "";
        private string cachedTracingTableURLForCallback = "";

        /// <summary>
        /// Obtains trace results for the specified starting graphic and feederID.
        /// </summary>
        /// <param name="callback">Method to call back on when tracing has completed</param>
        /// <param name="focusMap">Focus Map current</param>
        /// <param name="startingGraphic">Graphic object to start from</param>
        /// <param name="feederID">Circuit to trace (will derive from Graphic if not specified)</param>
        /// <param name="FCIDsToInclude">List of feature class IDs to return in trace results</param>
        /// <param name="cachedTracingTableURL">Tracing URL to trace</param>
        public void GetTraceResults(Action<Dictionary<int, List<TraceItem>>> callback, Map focusMap, Graphic startingGraphic, string feederID,
            List<int> FCIDsToInclude, string cachedTracingTableURL)
        {
            string globalID = "";
            foreach (KeyValuePair<string, object> kvp in startingGraphic.Attributes)
            {
                string fieldNameTest = kvp.Key.ToUpper().Replace(" ", "");
                if (fieldNameTest == FieldIdentifierName)
                {
                    globalID = kvp.Value.ToString();
                }
            }

            if (String.IsNullOrEmpty(feederID))
            {
                if (startingGraphic.Attributes["FEEDERID"] != null)
                {
                    feederID = startingGraphic.Attributes["FEEDERID"].ToString();
                }
            }

            if (String.IsNullOrEmpty(feederID))
            {
                ConfigUtility.UpdateStatusBarText("Could not determine Feeder ID value");
            }

            if (String.IsNullOrEmpty(globalID))
            {
                ConfigUtility.UpdateStatusBarText("Could not get Global ID value");
            }
            else
            {
                ConfigUtility.UpdateStatusBarText("Obtaining Loading Information");
                GetTraceResults(callback, focusMap, globalID, feederID, FCIDsToInclude, cachedTracingTableURL);
            }
        }

        /// <summary>
        /// Trace a feeder from the starting globalID value.  If globalID is null, it will trace the entire circuit
        /// </summary>
        /// <param name="callback">Method to call when finished tracing</param>
        /// <param name="focusMap">Map with layer connections</param>
        /// <param name="globalID">Global ID to start trace from</param>
        /// <param name="feederID">Circuit to trace</param>
        /// <param name="FCIDsToInclude">List of feature class IDs to return in results</param>
        /// <param name="cachedTracingTableURL">Tracing table URL</param>
        public void GetTraceResults(Action<Dictionary<int, List<TraceItem>>> callback, Map focusMap, string globalID, string feederID,
            List<int> FCIDsToInclude, string cachedTracingTableURL)
        {
            FocusMap = focusMap;

            GetTraceSettings();

            CurrentTraceCallback = callback;
            cachedTracingTableURLForCallback = cachedTracingTableURL;
            ConfigUtility.UpdateStatusBarText("Obtaining Loading Information");

            string FCIDWhereClause = "";
            for (int i = 0; i < FCIDsToInclude.Count; i++)
            {
                if (i == FCIDsToInclude.Count - 1)
                {
                    FCIDWhereClause += FCIDsToInclude[i];
                }
                else
                {
                    FCIDWhereClause += FCIDsToInclude[i] + ",";
                }
            }

            CurrentTraceFCIDWhereClause = FCIDWhereClause;

            if (!ConfigUtility.ObjectClassIDMapping.ContainsKey(LoadingInformationTracingTableURL))
            {
                ConfigUtility.GetClassIDToLayerIDMapping((Success) =>
                {
                    //Query for our trace from the database table.
                    QueryTask getTraceResultsQuery = new QueryTask(cachedTracingTableURL);
                    getTraceResultsQuery.ExecuteCompleted += new EventHandler<QueryEventArgs>(getLoadingTraceResultsQuery_ExecuteCompleted);
                    getTraceResultsQuery.Failed += new EventHandler<TaskFailedEventArgs>(getLoadingTraceResultsQuery_Failed);
                    Query query = new Query();
                    query.OutFields.AddRange(new string[] { "ORDER_NUM", "MIN_BRANCH", "MAX_BRANCH", "TREELEVEL", "TO_FEATURE_FEEDERINFO", "FEEDERID", "FEEDERFEDBY" });

                    if (!string.IsNullOrEmpty(globalID))
                    {
                        query.Where = "TO_FEATURE_GLOBALID = '" + globalID + "' AND FEEDERID = '" + feederID + "'";
                    }
                    else
                    {
                        //Trace the entire feeder
                        query.Where = "FROM_FEATURE_EID = '-1' AND FEEDERID = '" + feederID + "'";
                    }

                    CurrentlyExecutingTasks.Add(getTraceResultsQuery);
                    getTraceResultsQuery.ExecuteAsync(query);
                }, currentIdentifyUrl, FocusMap, "Initializing Tracing", IsSchematics);
            }
            else
            {
                //Query for our trace from the database table.
                QueryTask getTraceResultsQuery = new QueryTask(cachedTracingTableURL);
                getTraceResultsQuery.ExecuteCompleted += new EventHandler<QueryEventArgs>(getLoadingTraceResultsQuery_ExecuteCompleted);
                getTraceResultsQuery.Failed += new EventHandler<TaskFailedEventArgs>(getLoadingTraceResultsQuery_Failed);
                Query query = new Query();
                query.OutFields.AddRange(new string[] { "ORDER_NUM", "MIN_BRANCH", "MAX_BRANCH", "TREELEVEL", "TO_FEATURE_FEEDERINFO", "FEEDERID", "FEEDERFEDBY" });

                if (!string.IsNullOrEmpty(globalID))
                {
                    query.Where = "TO_FEATURE_GLOBALID = '" + globalID + "' AND FEEDERID = '" + feederID + "'";
                }
                else
                {
                    //Trace the entire feeder
                    query.Where = "FROM_FEATURE_EID = '-1' AND FEEDERID = '" + feederID + "'";
                }

                CurrentlyExecutingTasks.Add(getTraceResultsQuery);
                getTraceResultsQuery.ExecuteAsync(query);
            }
        }

        void getLoadingTraceResultsQuery_Failed(object sender, TaskFailedEventArgs e)
        {
            ConfigUtility.UpdateStatusBarText("Failed tracing for loading information" + e.Error.Message);
        }

        void getLoadingTraceResultsQuery_ExecuteCompleted(object sender, QueryEventArgs e)
        {
            ++iTotalTraces;
            try
            {
                if (!isConnectedTrace)
                    CurrentTraceCallbackResults.Clear();

                if (e.FeatureSet.Count() > 0)
                {
                    //Now we have our starting row from the cached tracing table. This should be one single entry, but occasionally might be multiple.
                    //The Min_Branch branch value specifies the upstream branch to query for during an upstream trace.  
                    //The Min_Branch and Max_Branch values can be used to return all results for a downstream trace.  
                    //The TreeLevel specifies what level in the tree the queried feature lies in.  The To_Feature_FeederInfo 
                    //value can be used to determine the phase start value.  If it doesn't match what is currently selected for phases then just return

                    int minBranch = -1;
                    int maxBranch = -1;
                    int min_treeLevel = -1;
                    int min_order_num = -1;
                    int max_treeLevel = -1;
                    int max_order_num = -1;

                    int tempMinBranch = 0;
                    int tempMaxBranch = 0;
                    int tempTree = 0;
                    int tempOrder = 0;
                    foreach (Graphic graphic in e.FeatureSet.Features)
                    {
                        Int32.TryParse(graphic.Attributes["TREELEVEL"].ToString(), out tempTree);
                        Int32.TryParse(graphic.Attributes["ORDER_NUM"].ToString(), out tempOrder);
                        Int32.TryParse(graphic.Attributes["MIN_BRANCH"].ToString(), out tempMinBranch);
                        Int32.TryParse(graphic.Attributes["MAX_BRANCH"].ToString(), out tempMaxBranch);
                        if (min_order_num == -1 || tempOrder < min_order_num) { min_order_num = tempOrder; }
                        if (max_order_num == -1 || tempOrder > max_order_num) { max_order_num = tempOrder; }

                        if (min_treeLevel == -1 || tempTree < min_treeLevel) { min_treeLevel = tempTree; }
                        if (max_treeLevel == -1 || tempTree > max_treeLevel) { max_treeLevel = tempTree; }

                        if (minBranch == -1 || tempMinBranch < minBranch) { minBranch = tempMinBranch; }
                        if (maxBranch == -1 || tempMaxBranch > maxBranch) { maxBranch = tempMaxBranch; }
                    }

                    Graphic TraceStartGraphic = e.FeatureSet.Features[0];

                    if (TraceStartGraphic.Attributes["FEEDERID"] == null) { return; }

                    string feederID = TraceStartGraphic.Attributes["FEEDERID"].ToString();

                    if (minBranch < 0 || maxBranch < 0 || string.IsNullOrEmpty(feederID)) { return; }

                    //This feature contains at least one of the configured energized phases so we can query for upstream or downstream as appropriate
                    //Query for our trace from the database table.
                    QueryTask getFullTraceResultsQuery = new QueryTask(cachedTracingTableURLForCallback);
                    getFullTraceResultsQuery.ExecuteCompleted += new EventHandler<QueryEventArgs>(getLoadingFullTraceResultsQuery_ExecuteCompleted);
                    getFullTraceResultsQuery.Failed += new EventHandler<TaskFailedEventArgs>(getLoadingFullTraceResultsQuery_Failed);
                    Query query = new Query();
                    query.OutFields.AddRange(new string[] { "ORDER_NUM", "TO_FEATURE_FCID", "TO_FEATURE_SCHEM_FCID", "TO_FEATURE_FEEDERINFO", "TO_FEATURE_GLOBALID", "TO_CIRCUITID", "TO_LINE_GLOBALID", "FEEDERID" });
                    query.Where = "(FEEDERID = '" + feederID + "' OR FEEDERFEDBY = '" + feederID + "') AND MIN_BRANCH >= " + minBranch + " AND MAX_BRANCH <= " + maxBranch
                        + " AND TREELEVEL >= " + min_treeLevel + " AND ORDER_NUM <= " + max_order_num + " AND (" + "TO_FEATURE_FCID" + " in (" + CurrentTraceFCIDWhereClause + ") OR " + "TO_FEATURE_SCHEM_FCID" + " in (" + CurrentTraceFCIDWhereClause + ")" +
                        "OR (TO_FEATURE_FCID IN (" + PriUGConductorFCID + "," + PriOHConductorFCID + ") AND TO_CIRCUITID IS NOT NULL))";
                    CurrentlyExecutingTasks.Add(getFullTraceResultsQuery);
                    getFullTraceResultsQuery.ExecuteAsync(query);
                }
                else
                {
                    LoadingTracingFinished();
                }
            }
            catch (Exception ex)
            {
                ConfigUtility.UpdateStatusBarText("Failed tracing for loading information" + ex.Message);
            }
        }

        void getLoadingFullTraceResultsQuery_Failed(object sender, TaskFailedEventArgs e)
        {
            ConfigUtility.UpdateStatusBarText("Failed tracing for loading information" + e.Error.Message);
        }

        void getLoadingFullTraceResultsQuery_ExecuteCompleted(object sender, QueryEventArgs e)
        {
            try
            {
                if (e.FeatureSet.Features.Count > 0)
                {
                    foreach (Graphic graphic in e.FeatureSet.Features)
                    {
                        if (!String.IsNullOrEmpty(Convert.ToString(graphic.Attributes["TO_LINE_GLOBALID"])))
                            GetConnectedFeeder_Load(graphic);
                        int order_num = -1;
                        int fcid = -1;
                        Int32.TryParse(graphic.Attributes["ORDER_NUM"].ToString(), out order_num);
                        Int32.TryParse(graphic.Attributes["TO_FEATURE_FCID"].ToString(), out fcid);

                        if (graphic.Attributes["TO_FEATURE_GLOBALID"] == null) { continue; }

                        string globalID = graphic.Attributes["TO_FEATURE_GLOBALID"].ToString();

                        //Adjust globalID in case it doesn't begin and end in {} and also call upper.  This ensures it should match a string in results
                        if (!globalID.Contains("{")) { globalID = "{" + globalID; }
                        if (!globalID.Contains("}")) { globalID = globalID + "}"; }
                        globalID = globalID.ToUpper();

                        //Does this classID exist in the map? Decided I don't have to actually check for this.  The results are the results. Post processing is
                        //already done to remove items from the displayed results to the user if things don't exist in th emap.
                        //if (ConfigUtility.GetLayerIDFromClassID(currentIdentifyUrl, fcid).Count <= 0) { continue; }

                        //Decided it is faster to let it fail the first time and add it then, rather than check for the existence of the key every time
                        //if (!CurrentFullTraceResultsByClassID.ContainsKey(fcid))
                        //{
                        //    CurrentFullTraceResultsByClassID.Add(fcid, new List<TraceItem>());
                        //}

                        TraceItem newItem = new TraceItem(fcid, globalID, "", fcid + ":" + globalID);
                        try
                        {
                            CurrentTraceCallbackResults[fcid].Add(newItem);
                        }
                        catch (Exception ex)
                        {
                            //Faster to have it fail the first time for a given fcid rather than spend the extra overhead verifying the ky
                            //exists for every single result item
                            CurrentTraceCallbackResults.Add(fcid, new List<TraceItem>());
                            CurrentTraceCallbackResults[fcid].Add(newItem);
                        }
                    }
                }
                LoadingTracingFinished();
            }
            catch (Exception ex)
            {
                ConfigUtility.UpdateStatusBarText("Failed tracing for loading information" + ex.Message);
            }
        }

        private void LoadingTracingFinished()
        {
            ++iCompletedTraces;
            if (isConnectedTrace && iTotalTraces == 1) return;
            if (iCompletedTraces != iTotalTraces) return;

            CurrentTraceCallback(CurrentTraceCallbackResults);
        }

        #endregion

        bool isConnectedTrace = false;
        int iTotalTraces = 0, iCompletedTraces = 0;
        private void GetTraceResults(Graphic startingGraphic, int LayerID, bool isSchematics)
        {
            isConnectedTrace = false;
            iTotalTraces = 0; iCompletedTraces = 0;
            string globalID = "";

            string statusMessage = "";
            if (IsUpstreamTrace) { statusMessage = "Executing upstream trace..."; }
            else { statusMessage = "Executing downstream trace..."; }

            string feederID = "";
            foreach (KeyValuePair<string, object> kvp in startingGraphic.Attributes)
            {
                string fieldNameTest = kvp.Key.ToUpper().Replace(" ", "");
                if (fieldNameTest == "CIRCUITID")
                {
                    feederID = kvp.Value.ToString();
                    break;
                }
            }

            if (string.IsNullOrEmpty(feederID)) { return; }

            CurrentResultsFeederID = feederID;

            if (!isSchematics)
            {
                globalID = "";
                foreach (KeyValuePair<string, object> kvp in startingGraphic.Attributes)
                {
                    string fieldNameTest = kvp.Key.ToUpper().Replace(" ", "");
                    if (fieldNameTest == FieldIdentifierName)
                    {
                        globalID = kvp.Value.ToString();
                    }
                }

                if (!string.IsNullOrEmpty(globalID))
                {
                    if (!ConfigUtility.ObjectClassIDMapping.ContainsKey(currentIdentifyUrl))
                    {
                        ConfigUtility.GetClassIDToLayerIDMapping((Success) =>
                        {
                            //Query for our trace from the database table.
                            QueryTask getTraceResultsQuery = new QueryTask(currentTracingTableUrl + "/" + currentCachedTracingTableLayerId);
                            getTraceResultsQuery.ExecuteCompleted += new EventHandler<QueryEventArgs>(getTraceResultsQuery_ExecuteCompleted);
                            getTraceResultsQuery.Failed += new EventHandler<TaskFailedEventArgs>(getTraceResultsQuery_Failed);
                            Query query = new Query();
                            query.OutFields.AddRange(new string[] { "ORDER_NUM", "MIN_BRANCH", "MAX_BRANCH", "TREELEVEL", "TO_FEATURE_FEEDERINFO", "FEEDERID", "FEEDERFEDBY" });
                            query.Where = "TO_FEATURE_GLOBALID = '" + globalID + "' AND " + currentFCIDFieldName + " = " + ConfigUtility.GetClassIDFromLayerID(currentIdentifyUrl, LayerID)
                                + " AND FEEDERID = '" + feederID + "'";

                            ConfigUtility.StatusBar.Text = statusMessage;
                            ConfigUtility.StatusBar.UpdateLayout();
                            CurrentlyExecutingTasks.Add(getTraceResultsQuery);
                            getTraceResultsQuery.ExecuteAsync(query);
                        }, currentIdentifyUrl, FocusMap, "Initializing Tracing", IsSchematics);
                    }
                    else
                    {
                        //Query for our trace from the database table.
                        QueryTask getTraceResultsQuery = new QueryTask(currentTracingTableUrl + "/" + currentCachedTracingTableLayerId);
                        getTraceResultsQuery.ExecuteCompleted += new EventHandler<QueryEventArgs>(getTraceResultsQuery_ExecuteCompleted);
                        getTraceResultsQuery.Failed += new EventHandler<TaskFailedEventArgs>(getTraceResultsQuery_Failed);
                        Query query = new Query();
                        query.OutFields.AddRange(new string[] { "ORDER_NUM", "MIN_BRANCH", "MAX_BRANCH", "TREELEVEL", "TO_FEATURE_FEEDERINFO", "FEEDERID", "FEEDERFEDBY" });
                        query.Where = "TO_FEATURE_GLOBALID = '" + globalID + "' AND " + currentFCIDFieldName + " = " + ConfigUtility.GetClassIDFromLayerID(currentIdentifyUrl, LayerID)
                            + " AND FEEDERID = '" + feederID + "'";

                        ConfigUtility.StatusBar.Text = statusMessage;
                        ConfigUtility.StatusBar.UpdateLayout();
                        CurrentlyExecutingTasks.Add(getTraceResultsQuery);
                        getTraceResultsQuery.ExecuteAsync(query);
                    }
                }
            }
            else
            {
                //Since we are querying schematics we have to get the associated feature in the electric dataset that
                //this feature maps to and then we can perform the trace.  UCID is the feature class ID field and UGUID is the
                //guid field that will map back to the original feature.
                string featClassID = "";
                globalID = "";
                foreach (KeyValuePair<string, object> kvp in startingGraphic.Attributes)
                {
                    string fieldNameTest = kvp.Key.ToUpper().Replace(" ", "");
                    if (fieldNameTest == "UCID")
                    {
                        featClassID = kvp.Value.ToString();
                    }
                    else if (fieldNameTest == SchemFieldIdentifierName)
                    {
                        globalID = kvp.Value.ToString();
                    }
                }

                if (!string.IsNullOrEmpty(globalID))
                {
                    if (!ConfigUtility.ObjectClassIDMapping.ContainsKey(currentIdentifyUrl))
                    {
                        ConfigUtility.GetClassIDToLayerIDMapping((Success) =>
                        {
                            //Query for our trace from the database table.
                            QueryTask getTraceResultsQuery = new QueryTask(currentTracingTableUrl + "/" + currentCachedTracingTableLayerId);
                            getTraceResultsQuery.ExecuteCompleted += new EventHandler<QueryEventArgs>(getTraceResultsQuery_ExecuteCompleted);
                            getTraceResultsQuery.Failed += new EventHandler<TaskFailedEventArgs>(getTraceResultsQuery_Failed);
                            Query query = new Query();
                            query.OutFields.AddRange(new string[] { "ORDER_NUM", "MIN_BRANCH", "MAX_BRANCH", "TREELEVEL", "TO_FEATURE_FEEDERINFO", "FEEDERID", "FEEDERFEDBY" });
                            query.Where = "TO_FEATURE_GLOBALID = '" + globalID + "' AND " + currentFCIDFieldName + " = " + ConfigUtility.GetClassIDFromLayerID(currentIdentifyUrl, LayerID)
                                + " AND FEEDERID = '" + feederID + "'";

                            ConfigUtility.StatusBar.Text = statusMessage;
                            ConfigUtility.StatusBar.UpdateLayout();
                            CurrentlyExecutingTasks.Add(getTraceResultsQuery);
                            getTraceResultsQuery.ExecuteAsync(query);
                        }, currentIdentifyUrl, FocusMap, "Initializing Tracing", IsSchematics);
                    }
                    else
                    {
                        //Query for our trace from the database table.
                        QueryTask getTraceResultsQuery = new QueryTask(currentTracingTableUrl + "/" + currentCachedTracingTableLayerId);
                        getTraceResultsQuery.ExecuteCompleted += new EventHandler<QueryEventArgs>(getTraceResultsQuery_ExecuteCompleted);
                        getTraceResultsQuery.Failed += new EventHandler<TaskFailedEventArgs>(getTraceResultsQuery_Failed);
                        Query query = new Query();
                        query.OutFields.AddRange(new string[] { "ORDER_NUM", "MIN_BRANCH", "MAX_BRANCH", "TREELEVEL", "TO_FEATURE_FEEDERINFO", "FEEDERID", "FEEDERFEDBY" });
                        query.Where = "TO_FEATURE_GLOBALID = '" + globalID + "' AND " + currentFCIDFieldName + " = " + ConfigUtility.GetClassIDFromLayerID(currentIdentifyUrl, LayerID)
                            + " AND FEEDERID = '" + feederID + "'";

                        ConfigUtility.StatusBar.Text = statusMessage;
                        ConfigUtility.StatusBar.UpdateLayout();
                        CurrentlyExecutingTasks.Add(getTraceResultsQuery);
                        getTraceResultsQuery.ExecuteAsync(query);
                    }
                }
            }
        }

        void getTraceResultsQuery_Failed(object sender, TaskFailedEventArgs e)
        {
            letTracingFinish = true;
            ConfigUtility.UpdateStatusBarText("Failed to determine trace start: " + e.Error.Message);
        }

        Dictionary<int, List<TraceItem>> FullTraceResultsByOrderNum = new Dictionary<int, List<TraceItem>>();
        void getTraceResultsQuery_ExecuteCompleted(object sender, QueryEventArgs e)
        {
            ++iTotalTraces;
            letTracingFinish = true;
            try
            {
                if (e.FeatureSet.Count() <= 0)
                {
                    TracingFinished();
                    return;
                }

                if (!isConnectedTrace)
                    FullTraceResultsByOrderNum.Clear();

                Graphic graphic = e.FeatureSet.Features[0];
                //Now we have our starting row from the cached tracing table. This should be one single entry, but occasionally might be multiple.
                //The Min_Branch branch value specifies the upstream branch to query for during an upstream trace.  
                //The Min_Branch and Max_Branch values can be used to return all results for a downstream trace.  
                //The TreeLevel specifies what level in the tree the queried feature lies in.  The To_Feature_FeederInfo 
                //value can be used to determine the phase start value.  If it doesn't match what is currently selected for phases then just return

                int feederInfo = -1;
                int minBranch = -1;
                int maxBranch = -1;
                int min_treeLevel = -1;
                int min_order_num = -1;
                int max_treeLevel = -1;
                int max_order_num = -1;

                int tempMinBranch = 0;
                int tempMaxBranch = 0;
                int tempTree = 0;
                int tempOrder = 0;

                Int32.TryParse(graphic.Attributes["TREELEVEL"].ToString(), out tempTree);
                Int32.TryParse(graphic.Attributes["ORDER_NUM"].ToString(), out tempOrder);
                Int32.TryParse(graphic.Attributes["MIN_BRANCH"].ToString(), out tempMinBranch);
                Int32.TryParse(graphic.Attributes["MAX_BRANCH"].ToString(), out tempMaxBranch);
                if (min_order_num == -1 || tempOrder < min_order_num) { min_order_num = tempOrder; }
                if (max_order_num == -1 || tempOrder > max_order_num) { max_order_num = tempOrder; }

                if (min_treeLevel == -1 || tempTree < min_treeLevel) { min_treeLevel = tempTree; }
                if (max_treeLevel == -1 || tempTree > max_treeLevel) { max_treeLevel = tempTree; }

                if (minBranch == -1 || tempMinBranch < minBranch) { minBranch = tempMinBranch; }
                if (maxBranch == -1 || tempMaxBranch > maxBranch) { maxBranch = tempMaxBranch; }


                //Graphic graphic = e.FeatureSet.Features[0];

                /*
                Int32.TryParse(graphic.Attributes["TREELEVEL"].ToString(), out treeLevel);
                Int32.TryParse(graphic.Attributes["ORDER_NUM"].ToString(), out order_num);
                Int32.TryParse(graphic.Attributes["MIN_BRANCH"].ToString(), out minBranch);
                Int32.TryParse(graphic.Attributes["MAX_BRANCH"].ToString(), out maxBranch);
                */
                try
                {
                    Int32.TryParse(graphic.Attributes["TO_FEATURE_FEEDERINFO"].ToString(), out feederInfo);
                }
                catch (Exception ex)
                {
                    //this feeder info was null.  We will assume 7 and continue
                    feederInfo = 7;
                }

                if (graphic.Attributes["FEEDERID"] == null) { return; }

                string feederID = graphic.Attributes["FEEDERID"].ToString();
                string feederFedBy = "";

                if (graphic.Attributes["FEEDERFEDBY"] != null)
                {
                    feederFedBy = graphic.Attributes["FEEDERFEDBY"].ToString();
                }

                if (minBranch < 0 || maxBranch < 0 || string.IsNullOrEmpty(feederID)) { return; }

                int cPhase = feederInfo & 1;
                int bPhase = feederInfo & 2;
                int aPhase = feederInfo & 4;

                bool execute = true;
                if (TraceAPhase && TraceBPhase && TraceCPhase) { execute = true; }
                else if (!((aPhase == 4 && TraceAPhase) || (bPhase == 2 && TraceBPhase) || (cPhase == 1 && TraceCPhase))) { execute = false; }

                if (execute)
                {
                    string[] outFields = null;
                    string tracingTableURL = "";
                    string tracingTableLayerID = "";
                    if (IsUndergroundNetworkTrace)
                    {
                        outFields = new string[] { "ORDER_NUM", currentFCIDFieldName, "TO_FEATURE_GLOBALID" };
                        tracingTableURL = UndergroundNetworkURL;
                        tracingTableLayerID = UndergroundNetworkLayerID;
                    }
                    else
                    {
                        if (TraceFeederFedFeeders)
                        {
                            if (IsUpstreamTrace)
                            {
                                //
                                outFields = new string[] { "ORDER_NUM", currentFCIDFieldName, "TO_FEATURE_FEEDERINFO", "TO_FEATURE_GLOBALID", "MIN_BRANCH", "MAX_BRANCH", "TREELEVEL", "FROM_CIRCUITID", "FEEDERFEDBY", "FROM_LINE_GLOBALID", "FEEDERID", "FROM_POINT_FC_GUID" };
                            }
                            else
                            {
                                outFields = new string[] { "ORDER_NUM", currentFCIDFieldName, "TO_FEATURE_FEEDERINFO", "TO_FEATURE_GLOBALID", "MIN_BRANCH", "MAX_BRANCH", "TREELEVEL", "TO_CIRCUITID", "FEEDERFEDBY", "TO_LINE_GLOBALID", "FEEDERID", "TO_POINT_FC_GUID" };
                            }
                        }
                        else
                        {
                            outFields = new string[] { "ORDER_NUM", currentFCIDFieldName, "TO_FEATURE_FEEDERINFO", "TO_FEATURE_GLOBALID" };
                        }
                        tracingTableURL = currentTracingTableUrl;
                        tracingTableLayerID = currentCachedTracingTableLayerId;
                    }

                    //This feature contains at least one of the configured energized phases so we can query for upstream or downstream as appropriate
                    //Query for our trace from the database table.
                    QueryTask getFullTraceResultsQuery = new QueryTask(tracingTableURL + "/" + tracingTableLayerID);
                    getFullTraceResultsQuery.ExecuteCompleted += new EventHandler<QueryEventArgs>(getFullTraceResultsQuery_ExecuteCompleted);
                    getFullTraceResultsQuery.Failed += new EventHandler<TaskFailedEventArgs>(getFullTraceResultsQuery_Failed);
                    Query query = new Query();
                    query.OutFields.AddRange(outFields);

                    if (IsUpstreamTrace)
                    {
                        if (!string.IsNullOrEmpty(feederFedBy) && TraceFeederFedFeeders)
                        {
                            query.Where = "(FEEDERID = '" + feederFedBy + "' OR FEEDERFEDBY = '" + feederFedBy + "') AND MIN_BRANCH <= " + minBranch + " AND MAX_BRANCH >= " + maxBranch
                               + " AND TREELEVEL <= " + max_treeLevel + " AND ORDER_NUM >= " + min_order_num;
                        }
                        else
                        {
                            query.Where = "FEEDERID = '" + feederID + "' AND MIN_BRANCH <= " + minBranch + " AND MAX_BRANCH >= " + maxBranch
                                + " AND TREELEVEL <= " + max_treeLevel + " AND ORDER_NUM >= " + min_order_num;
                        }

                        if (IsUndergroundNetworkTrace)
                        {
                            //Only return subtype of vault
                            string subtypes = "";
                            foreach (string subtype in UndergroundSubsurfaceStructureSubtypes) { subtypes += subtype + ","; }
                            subtypes = subtypes.Substring(0, subtypes.Length - 1);
                            query.Where += " AND SUBTYPECD IN (" + subtypes + ")";
                        }
                    }
                    else
                    {
                        if (TraceFeederFedFeeders)
                        {
                            query.Where = "(FEEDERID = '" + feederID + "' OR FEEDERFEDBY = '" + feederID + "') AND MIN_BRANCH >= " + minBranch + " AND MAX_BRANCH <= " + maxBranch
                                + " AND TREELEVEL >= " + min_treeLevel + " AND ORDER_NUM <= " + max_order_num;
                        }
                        else
                        {
                            query.Where = "FEEDERID = '" + feederID + "' AND MIN_BRANCH >= " + minBranch + " AND MAX_BRANCH <= " + maxBranch
                                + " AND TREELEVEL >= " + min_treeLevel + " AND ORDER_NUM <= " + max_order_num;
                        }

                        if (IsUndergroundNetworkTrace)
                        {
                            //Only return subtype of vault
                            string subtypes = "";
                            foreach (string subtype in UndergroundSubsurfaceStructureSubtypes) { subtypes += subtype + ","; }
                            subtypes = subtypes.Substring(0, subtypes.Length - 1);
                            query.Where += " AND SUBTYPECD IN (" + subtypes + ")";
                        }
                    }
                    CurrentlyExecutingTasks.Add(getFullTraceResultsQuery);
                    getFullTraceResultsQuery.ExecuteAsync(query);
                }
                else
                {
                    TracingFinished();
                }
            }
            catch (Exception ex)
            {

            }
        }

        void getFullTraceResultsQuery_Failed(object sender, TaskFailedEventArgs e)
        {
            ConfigUtility.UpdateStatusBarText("Failed obtaining trace results: " + e.Error.Message);
        }

        ArrayList pList_TracedGUID = new ArrayList();
        bool letTracingFinish = true;
        void getFullTraceResultsQuery_ExecuteCompleted(object sender, QueryEventArgs e)
        {
            try
            {
                if (e.FeatureSet.Features.Count > 0)
                {
                    int subsurfaceStructureFCID = Int32.Parse(SubsurfaceStructureFCID);
                    foreach (Graphic graphic in e.FeatureSet.Features)
                    {
                        if (TraceFeederFedFeeders)
                        {
                            if (!String.IsNullOrEmpty(Convert.ToString(graphic.Attributes["FROM_LINE_GLOBALID"])) ||
                                !String.IsNullOrEmpty(Convert.ToString(graphic.Attributes["TO_LINE_GLOBALID"])))
                            {
                                GetConnectedFeeder(graphic);
                            }
                        }

                        int order_num = -1;
                        int feederInfo = -1;
                        int fcid = -1;
                        Int32.TryParse(graphic.Attributes["ORDER_NUM"].ToString(), out order_num);

                        //If the FCID field name is null then this is probably schematics trace and this is a substation result
                        if (graphic.Attributes[currentFCIDFieldName] == null) { continue; }
                        Int32.TryParse(graphic.Attributes[currentFCIDFieldName].ToString(), out fcid);

                        if (IsUndergroundNetworkTrace && fcid != subsurfaceStructureFCID) { continue; }

                        if (graphic.Attributes["TO_FEATURE_GLOBALID"] == null) { continue; }

                        string globalID = graphic.Attributes["TO_FEATURE_GLOBALID"].ToString();

                        //Adjust globalID in case it doesn't begin and end in {} and also call upper.  This ensures it should match a string in results
                        if (!globalID.Contains("{")) { globalID = "{" + globalID; }
                        if (!globalID.Contains("}")) { globalID = globalID + "}"; }
                        globalID = globalID.ToUpper();
                        if (pList_TracedGUID.Contains(globalID)) continue;
                        else pList_TracedGUID.Add(globalID);

                        bool execute = true;
                        if (TraceAPhase && TraceBPhase && TraceCPhase) { execute = true; }
                        else
                        {
                            try
                            {
                                if (graphic.Attributes["TO_FEATURE_FEEDERINFO"] == null) { feederInfo = 7; }
                                else { Int32.TryParse(graphic.Attributes["TO_FEATURE_FEEDERINFO"].ToString(), out feederInfo); }
                            }
                            catch (Exception ex)
                            {
                                //this feeder info was null.  We will assume 7 and continue
                                feederInfo = 7;
                            }

                            int cPhase = feederInfo & 1;
                            int bPhase = feederInfo & 2;
                            int aPhase = feederInfo & 4;

                            if (!((aPhase == 4 && TraceAPhase) || (bPhase == 2 && TraceBPhase) || (cPhase == 1 && TraceCPhase))) { execute = false; }
                        }

                        if (execute)
                        {
                            //Does this classID exist in the map? Decided I don't have to actually check for this.  The results are the results. Post processing is
                            //already done to remove items from the displayed results to the user if things don't exist in th emap.
                            //if (ConfigUtility.GetLayerIDFromClassID(currentIdentifyUrl, fcid).Count <= 0) { continue; }

                            //Decided it is faster to let it fail the first time and add it then, rather than check for the existence of the key every time
                            //if (!CurrentFullTraceResultsByClassID.ContainsKey(fcid))
                            //{
                            //    CurrentFullTraceResultsByClassID.Add(fcid, new List<TraceItem>());
                            //}

                            TraceItem newItem = new TraceItem(fcid, globalID, "", fcid + ":" + globalID);
                            try
                            {
                                CurrentFullTraceResultsByClassID[fcid].Add(newItem);
                            }
                            catch (Exception ex)
                            {
                                //Faster to have it fail the first time for a given fcid rather than spend the extra overhead verifying the ky
                                //exists for every single result item
                                CurrentFullTraceResultsByClassID.Add(fcid, new List<TraceItem>());
                                CurrentFullTraceResultsByClassID[fcid].Add(newItem);
                            }
                            try
                            {
                                FullTraceResultsByOrderNum.Add(order_num, new List<TraceItem>());
                                FullTraceResultsByOrderNum[order_num].Add(newItem);
                            }
                            catch (Exception ex)
                            {
                                FullTraceResultsByOrderNum[order_num].Add(newItem);
                            }
                        }
                    }
                }
                TracingFinished();
            }
            catch (Exception ex)
            {

            }
        }

        private void GetConnectedFeeder(Graphic graphic)
        {
            isConnectedTrace = true;
            letTracingFinish = false;
            QueryTask getTraceResultsQuery = new QueryTask(currentTracingTableUrl + "/" + currentCachedTracingTableLayerId);
            getTraceResultsQuery.ExecuteCompleted += new EventHandler<QueryEventArgs>(getTraceResultsQuery_ExecuteCompleted);
            getTraceResultsQuery.Failed += new EventHandler<TaskFailedEventArgs>(getTraceResultsQuery_Failed);
            Query query = new Query();
            query.OutFields.AddRange(new string[] { "ORDER_NUM", "MIN_BRANCH", "MAX_BRANCH", "TREELEVEL", "TO_FEATURE_FEEDERINFO", "FEEDERID", "FEEDERFEDBY" });
            query.Where = "TO_FEATURE_GLOBALID = '" + (IsUpstreamTrace ? Convert.ToString(graphic.Attributes["FROM_LINE_GLOBALID"]) : Convert.ToString(graphic.Attributes["TO_POINT_FC_GUID"])) + "' "
                + " AND FEEDERID = '" + (IsUpstreamTrace ? Convert.ToString(graphic.Attributes["FROM_CIRCUITID"]) : Convert.ToString(graphic.Attributes["TO_CIRCUITID"])) + "'";

            CurrentlyExecutingTasks.Add(getTraceResultsQuery);
            getTraceResultsQuery.ExecuteAsync(query);
        }

        private void GetConnectedFeeder_Load(Graphic graphic)
        {
            isConnectedTrace = true;
            QueryTask getTraceResultsQuery = new QueryTask(currentTracingTableUrl + "/" + currentCachedTracingTableLayerId);
            getTraceResultsQuery.ExecuteCompleted += new EventHandler<QueryEventArgs>(getLoadingTraceResultsQuery_ExecuteCompleted);
            getTraceResultsQuery.Failed += new EventHandler<TaskFailedEventArgs>(getLoadingTraceResultsQuery_Failed);
            Query query = new Query();
            query.OutFields.AddRange(new string[] { "ORDER_NUM", "MIN_BRANCH", "MAX_BRANCH", "TREELEVEL", "TO_FEATURE_FEEDERINFO", "FEEDERID", "FEEDERFEDBY" });
            query.Where = "TO_FEATURE_GLOBALID = '" + Convert.ToString(graphic.Attributes["TO_LINE_GLOBALID"]) + "' "
                + " AND FEEDERID = '" + Convert.ToString(graphic.Attributes["TO_CIRCUITID"]) + "'";

            CurrentlyExecutingTasks.Add(getTraceResultsQuery);
            getTraceResultsQuery.ExecuteAsync(query);
        }

        private void TracingFinished()
        {
            ++iCompletedTraces;
            if (TraceFeederFedFeeders)
            {
                if (isConnectedTrace && iTotalTraces == 1) return;
                if (!letTracingFinish) return;
                if (iCompletedTraces != iTotalTraces) return;
            }
            try
            {
                if (LayerConfiguredForTracing())
                {
                    //If our counter is at 0 then all queries have returned and we can complete our in order list to display for the user
                    List<int> order_Nums_In_Order = FullTraceResultsByOrderNum.Keys.ToList();
                    order_Nums_In_Order.Sort();
                    if (IsDownstreamTrace)
                    {
                        for (int i = order_Nums_In_Order.Count - 1; i >= 0; i--)
                        {
                            CurrentFullTraceResultsInOrder.AddRange(FullTraceResultsByOrderNum[order_Nums_In_Order[i]]);
                        }
                    }
                    else
                    {
                        foreach (int order_num in order_Nums_In_Order)
                        {
                            CurrentFullTraceResultsInOrder.AddRange(FullTraceResultsByOrderNum[order_num]);
                        }
                    }

                    CurrentResultsAreSchematics = IsSchematics;

                    //Adjust if this is a protective device search
                    if (IsProtectiveDeviceTrace)
                    {
                        if (IsUpstreamTrace) { CurrentResultsAreUpstream = true; }
                        else { CurrentResultsAreUpstream = false; }

                        CurrentResultsAreProtective = true;
                        for (int i = 0; i < CurrentFullTraceResultsInOrder.Count; i++)
                        {
                            TraceItem result = CurrentFullTraceResultsInOrder[i];
                            string[] items = Regex.Split(result.ToString(), ":");
                            if (ProtectiveDeviceClassIDs.Contains(Int32.Parse(items[0])))
                            {
                                CurrentProtectiveDeviceTraceResultsInOrder.Add(CurrentFullTraceResultsInOrder[i]);
                            }
                        }
                        ConfigUtility.StatusBar.Text = "";
                        ConfigUtility.StatusBar.UpdateLayout();
                        TracingControl.TraceResults = CurrentProtectiveDeviceTraceResultsInOrder;


                    }
                    else
                    {
                        CurrentResultsAreProtective = false;
                        if (IsUpstreamTrace) { CurrentResultsAreUpstream = true; }
                        else { CurrentResultsAreUpstream = false; }

                        ConfigUtility.StatusBar.Text = "";
                        ConfigUtility.StatusBar.UpdateLayout();
                        TracingControl.TraceResults = CurrentFullTraceResultsInOrder;
                    }
                }
                else
                {
                    ConfigUtility.StatusBar.Text = "";
                    ConfigUtility.StatusBar.UpdateLayout();
                    TracingControl.TraceResults = new List<TraceItem>();
                }
            }
            catch (Exception ex)
            {

            }
            finally
            {
                pList_TracedGUID.Clear();
            }
        }

        void queryTask_ExecuteCompleted(object sender, QueryEventArgs e)
        {
            try
            {

            }
            catch (Exception ex)
            {

            }
        }

        public void StopCurrentAsyncTasks()
        {
            foreach (TaskBase task in CurrentlyExecutingTasks)
            {
                if (task != null && task.IsBusy)
                {
                    task.CancelAsync();
                }
            }
            CurrentlyExecutingTasks.Clear();
        }

        //ME Q3 2019 Release - DA# 190501 --START
        public void GetLoadingTraceResults(Action<Dictionary<int, List<TraceItem>>> callback, Map focusMap, string globalID, string feederID,
           List<int> FCIDsToInclude, string cachedTracingTableURL, int classID)
        {
            FocusMap = focusMap;

            GetTraceSettings();

            CurrentTraceCallback = callback;
            cachedTracingTableURLForCallback = cachedTracingTableURL;
            ConfigUtility.UpdateStatusBarText("Obtaining Loading Information");
            _Start_TO_FEATURE_FCID = "0";

            if (!ConfigUtility.ObjectClassIDMapping.ContainsKey(LoadingInformationTracingTableURL))
            {
                ConfigUtility.GetClassIDToLayerIDMapping((Success) =>
                {
                    //Query for our trace from the database table.
                    QueryTask getTraceResultsQuery = new QueryTask(cachedTracingTableURL);
                    getTraceResultsQuery.ExecuteCompleted += new EventHandler<QueryEventArgs>(loadingStartTraceQuery_ExecuteCompleted);
                    getTraceResultsQuery.Failed += new EventHandler<TaskFailedEventArgs>(getLoadingTraceResultsQuery_Failed);
                    Query query = new Query();
                    query.OutFields.AddRange(new string[] { "ORDER_NUM", "MIN_BRANCH", "MAX_BRANCH", "TREELEVEL", "TO_FEATURE_FEEDERINFO", "TO_FEATURE_FCID", "TO_FEATURE_EID", "TO_FEATURE_OID", "FEEDERID", "FEEDERFEDBY", "TO_CIRCUITID" });

                    if (!string.IsNullOrEmpty(globalID) && classID != 0)
                    {
                        query.Where = "TO_FEATURE_GLOBALID = '" + globalID + "' AND FEEDERID = '" + feederID + "' AND TO_FEATURE_FCID =" + classID;
                        _Start_TO_FEATURE_FCID = Convert.ToString(classID);
                    }
                    else if (!string.IsNullOrEmpty(globalID))
                    {
                        query.Where = "TO_FEATURE_GLOBALID = '" + globalID + "' AND FEEDERID = '" + feederID + "'";
                    }
                    else
                    {
                        //Trace the entire feeder
                        query.Where = "FROM_FEATURE_EID = '-1' AND FEEDERID = '" + feederID + "'";
                    }

                    CurrentlyExecutingTasks.Add(getTraceResultsQuery);
                    getTraceResultsQuery.ExecuteAsync(query);
                }, currentIdentifyUrl, FocusMap, "Initializing Tracing", IsSchematics);
            }
            else
            {
                //Query for our trace from the database table.
                QueryTask getTraceResultsQuery = new QueryTask(cachedTracingTableURL);
                getTraceResultsQuery.ExecuteCompleted += new EventHandler<QueryEventArgs>(loadingStartTraceQuery_ExecuteCompleted);
                getTraceResultsQuery.Failed += new EventHandler<TaskFailedEventArgs>(getLoadingTraceResultsQuery_Failed);
                Query query = new Query();
                query.OutFields.AddRange(new string[] { "ORDER_NUM", "MIN_BRANCH", "MAX_BRANCH", "TREELEVEL", "TO_FEATURE_FEEDERINFO", "TO_FEATURE_FCID", "TO_FEATURE_EID", "TO_FEATURE_OID", "FEEDERID", "FEEDERFEDBY", "TO_CIRCUITID" });

                if (!string.IsNullOrEmpty(globalID) && classID != 0)
                {
                    query.Where = "TO_FEATURE_GLOBALID = '" + globalID + "' AND FEEDERID = '" + feederID + "' AND TO_FEATURE_FCID =" + classID;
                    _Start_TO_FEATURE_FCID = Convert.ToString(classID);
                }
                else if (!string.IsNullOrEmpty(globalID))
                {
                    query.Where = "TO_FEATURE_GLOBALID = '" + globalID + "' AND FEEDERID = '" + feederID + "'";
                }
                else
                {
                    //Trace the entire feeder
                    query.Where = "FROM_FEATURE_EID = '-1' AND FEEDERID = '" + feederID + "'";
                }

                CurrentlyExecutingTasks.Add(getTraceResultsQuery);
                getTraceResultsQuery.ExecuteAsync(query);
            }
        }

        private void loadingStartTraceQuery_ExecuteCompleted(object sender, QueryEventArgs e)
        {
            ++iTotalTraces;
            try
            {
                if (!isConnectedTrace)
                    CurrentTraceCallbackResults.Clear();

                if (e.FeatureSet.Count() > 0)
                {
                    //Now we have our starting row from the cached tracing table. This should be one single entry, but occasionally might be multiple.
                    //The Min_Branch branch value specifies the upstream branch to query for during an upstream trace.  
                    //The Min_Branch and Max_Branch values can be used to return all results for a downstream trace.  
                    //The TreeLevel specifies what level in the tree the queried feature lies in.  The To_Feature_FeederInfo 
                    //value can be used to determine the phase start value.  If it doesn't match what is currently selected for phases then just return

                    Graphic TraceStartGraphic = e.FeatureSet.Features[0];

                    int ORDER_NUM = Convert.ToInt32(TraceStartGraphic.Attributes["ORDER_NUM"]);
                    int MIN_BRANCH = Convert.ToInt32(TraceStartGraphic.Attributes["MIN_BRANCH"]);
                    int MAX_BRANCH = Convert.ToInt32(TraceStartGraphic.Attributes["MAX_BRANCH"]);
                    int TREELEVEL = Convert.ToInt32(TraceStartGraphic.Attributes["TREELEVEL"]);
                    int TO_FEATURE_FEEDERINFO = Convert.ToInt32(TraceStartGraphic.Attributes["TO_FEATURE_FEEDERINFO"]);
                    int TO_FEATURE_EID = Convert.ToInt32(TraceStartGraphic.Attributes["TO_FEATURE_EID"]);
                    int TO_FEATURE_OID = Convert.ToInt32(TraceStartGraphic.Attributes["TO_FEATURE_OID"]);
                    int TO_FEATURE_FCID = Convert.ToInt32(TraceStartGraphic.Attributes["TO_FEATURE_FCID"]);
                    string FEEDERID_tracing = Convert.ToString(TraceStartGraphic.Attributes["FEEDERID"]);
                    int TO_CIRCUITID = Convert.ToInt32(TraceStartGraphic.Attributes["TO_CIRCUITID"]);

                    //Query for our trace from the database table.
                    QueryTask getFullTraceResultsQuery = new QueryTask(cachedTracingTableURLForCallback);
                    getFullTraceResultsQuery.ExecuteCompleted += loadingFullTraceResultsQuery_ExecuteCompleted;
                    getFullTraceResultsQuery.Failed += getLoadingFullTraceResultsQuery_Failed;
                    string Tran_PriMtr_TO_FEATURE_FCID = _ServiceLocation_FEATURE_FCID;
                    string End_TO_FEATURE_FCID = "0";
                    ESRI.ArcGIS.Client.Tasks.Query query = new ESRI.ArcGIS.Client.Tasks.Query();
                    if (!string.IsNullOrEmpty(FEEDERID_tracing))
                    {
                        //query.OutFields.AddRange(new string[] { "TO_FEATURE_FCID,TO_FEATURE_GLOBALID,TO_CIRCUITID" });
                        query.OutFields.AddRange(new string[] { "ORDER_NUM", "TO_FEATURE_FCID", "TO_FEATURE_SCHEM_FCID", "TO_FEATURE_FEEDERINFO", "TO_FEATURE_GLOBALID", "TO_CIRCUITID", "TO_LINE_GLOBALID", "FEEDERID" });
                        query.Where = "(TO_FEATURE_FCID IN (" + Tran_PriMtr_TO_FEATURE_FCID + "," + _Start_TO_FEATURE_FCID + "," + End_TO_FEATURE_FCID + ")" +
                            " OR (TO_FEATURE_FCID IN (" + PriUGConductorFCID + "," + PriOHConductorFCID + ") AND TO_CIRCUITID IS NOT NULL)) " +
                            "AND MIN_BRANCH >= " + MIN_BRANCH + " AND MAX_BRANCH <= " + MAX_BRANCH
                                + " AND TREELEVEL >= " + TREELEVEL + " AND ORDER_NUM <= " + ORDER_NUM + " AND FEEDERID = '" + FEEDERID_tracing + "'";
                    }
                    getFullTraceResultsQuery.ExecuteAsync(query);

                    CurrentlyExecutingTasks.Add(getFullTraceResultsQuery);
                }
                else
                {
                    LoadingTracingFinished();
                }
            }
            catch (Exception ex)
            {
                ConfigUtility.UpdateStatusBarText("Failed tracing for loading information" + ex.Message);
            }
        }

        void loadingFullTraceResultsQuery_ExecuteCompleted(object sender, QueryEventArgs e)
        {
            try
            {
                if (e.FeatureSet.Features.Count > 0)
                {
                    foreach (Graphic graphic in e.FeatureSet.Features)
                    {
                        //if (!String.IsNullOrEmpty(Convert.ToString(graphic.Attributes["TO_LINE_GLOBALID"])))
                        //    GetConnectedFeeder_Load(graphic);
                        int order_num = -1;
                        int fcid = -1;
                        Int32.TryParse(graphic.Attributes["ORDER_NUM"].ToString(), out order_num);
                        Int32.TryParse(graphic.Attributes["TO_FEATURE_FCID"].ToString(), out fcid);

                        if (graphic.Attributes["TO_FEATURE_GLOBALID"] == null) { continue; }

                        string globalID = graphic.Attributes["TO_FEATURE_GLOBALID"].ToString();

                        //Adjust globalID in case it doesn't begin and end in {} and also call upper.  This ensures it should match a string in results
                        if (!globalID.Contains("{")) { globalID = "{" + globalID; }
                        if (!globalID.Contains("}")) { globalID = globalID + "}"; }
                        globalID = globalID.ToUpper();

                        //Does this classID exist in the map? Decided I don't have to actually check for this.  The results are the results. Post processing is
                        //already done to remove items from the displayed results to the user if things don't exist in th emap.
                        //if (ConfigUtility.GetLayerIDFromClassID(currentIdentifyUrl, fcid).Count <= 0) { continue; }

                        //Decided it is faster to let it fail the first time and add it then, rather than check for the existence of the key every time
                        //if (!CurrentFullTraceResultsByClassID.ContainsKey(fcid))
                        //{
                        //    CurrentFullTraceResultsByClassID.Add(fcid, new List<TraceItem>());
                        //}

                        TraceItem newItem = new TraceItem(fcid, globalID, "", fcid + ":" + globalID);
                        try
                        {
                            CurrentTraceCallbackResults[fcid].Add(newItem);
                        }
                        catch (Exception ex)
                        {
                            //Faster to have it fail the first time for a given fcid rather than spend the extra overhead verifying the ky
                            //exists for every single result item
                            CurrentTraceCallbackResults.Add(fcid, new List<TraceItem>());
                            CurrentTraceCallbackResults[fcid].Add(newItem);
                        }
                    }
                }
                LoadingTracingFinished();
            }
            catch (Exception ex)
            {
                ConfigUtility.UpdateStatusBarText("Failed tracing for loading information" + ex.Message);
            }
        }

        //ME Q3 2019 Release - DA# 190501 --END
    }

    public enum TraceType
    {
        Substation,
        Electric,
        Schematics
    }
}
