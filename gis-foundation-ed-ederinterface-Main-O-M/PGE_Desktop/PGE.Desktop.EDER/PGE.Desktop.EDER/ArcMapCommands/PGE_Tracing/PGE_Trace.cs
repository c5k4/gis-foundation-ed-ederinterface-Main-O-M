using System;
using System.Collections.Generic;
using System.Drawing;
using ESRI.ArcGIS.Geodatabase;
using Miner.Interop;
using ESRI.ArcGIS.EditorExt;
using ESRI.ArcGIS.NetworkAnalysis;
using Miner.FrameworkUI.Trace;
using Miner.Framework.Trace.Utilities;
using ESRI.ArcGIS.esriSystem;
using Miner.FrameworkUI;
using ESRI.ArcGIS.ArcMapUI;
using Miner.Framework;
using ESRI.ArcGIS.Carto;
using System.Collections;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.Geometry;
using PGE.Common.Delivery.Framework;
using PGE.Common.Delivery.Diagnostics;
using System.Reflection;
using Miner;

namespace PGE.Desktop.EDER.ArcMapCommands.PGE_Tracing
{
    public abstract class PGE_Trace : BaseElectricTraceTool
    {
        #region Private vars

        private static readonly Log4NetLogger _logger = new Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "EDERDesktop.log4net.config");
        private static PGE_TraceResults resultsDialog = null;
        private Dictionary<string, int> OperatingNumberFieldIndex = new Dictionary<string,int>();
        //This xy mapping will allow us to ensure that we only draw a directional indicator once for a given junction
        private Dictionary<double, List<double>> xyPairsAlreadyDrawn = new Dictionary<double, List<double>>();
        private Dictionary<IFeatureClass, List<IGraphicsLayer>> graphicsLayers = new Dictionary<IFeatureClass, List<IGraphicsLayer>>();
        private INetworkAnalysisExt networkAnalyst = null;
        private IMap currentMap = null;
        private TraceType traceType;
        private ArrayList protectiveClassIDs;

        #endregion

        #region Constructor

        public PGE_Trace(string category, string caption, string message, string tooltip, string name, IMMSearchStrategy searchStrategy, TraceType traceType)
            : base(searchStrategy)
        {
            //
            // TODO: Define values for the public properties
            //
            base.m_category = category; //localizable text
            base.m_caption = caption;  //localizable text
            base.m_message = message;  //localizable text 
            base.m_toolTip = tooltip;  //localizable text 
            base.m_name = name;   //unique id, non-localizable (e.g. "MyCategory_ArcMapCommand")
            this.traceType = traceType;
            try
            {
                //
                // TODO: change bitmap name if necessary
                //
                string bitmapResourceName = GetType().Name + ".bmp";
                base.m_bitmap = new Bitmap(GetType(), bitmapResourceName);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine(ex.Message, "Invalid Bitmap");
            }

            networkAnalyst = null;
            networkAnalyst = GetNetworkAnalyst();
            ClearResults();
            resultsDialog = null;
            OperatingNumberFieldIndex.Clear();
            xyPairsAlreadyDrawn.Clear();
            graphicsLayers.Clear();
            currentMap = null;
            protectiveClassIDs = null;
            refreshOnClick = true;
        }

        #endregion

        #region Overrides


        /// <summary>
        /// Should only enable if the currently selected network is one with the MMElectricTraceWeight weight
        /// </summary>
        public override bool Enabled
        {
            get
            {
                if (networkAnalyst == null)
                {
                    networkAnalyst = GetNetworkAnalyst();
                }

                if (networkAnalyst != null)
                {
                    if (networkAnalyst.CurrentNetwork == null) { return false; }

                    this.CurrentGeometricNetwork = networkAnalyst.CurrentNetwork;

                    INetSchema networkSchema = this.CurrentGeometricNetwork.Network as INetSchema;
                    INetWeight networkWeight = networkSchema.get_WeightByName("MMElectricTraceWeight");

                    if (networkWeight != null) { return true; }
                }
                return false;
            }
        }

        /// <summary>
        /// Occurs when the user clicks the command in ArcMap
        /// </summary>
        public override void OnClick()
        {
            this.CurrentFeatureSnap = null;
            DisplayStatusBarMessage(this.Res.GetString("Trace.Messages.ClickEdge"));
            if (base.m_cursor == null)
            {
                base.m_cursor = this.Res.GetCursor("Trace.FlagCursorCur");
            }
            base.OnClick();
        }


        /// <summary>
        /// Update the snapping behavior when the mouse moves
        /// </summary>
        /// <param name="button"></param>
        /// <param name="shift"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public override void OnMouseMove(int button, int shift, int x, int y)
        {
            if (refreshOnClick)
            {
                OnClick();
                refreshOnClick = false;
            }
            if (this.CurrentFeatureSnap == null)
            {
                this.CurrentFeatureSnap = new FeatureSnap(Document.ActiveView, this.CurrentGeometricNetwork as IFeatureClassContainer);
            }
            this.CurrentFeatureSnap.MouseMoveClicked(x, y);
        }

        /// <summary>
        /// Displays the trace results on the map or as a selection, and it also will provide the new dialog for displaying
        /// the results in electrical connectivity order
        /// </summary>
        /// <param name="traceResults"></param>
        protected override void DisplayResults(IMMSearchResults traceResults)
        {
            if (resultsDialog != null)
            {
                resultsDialog.Close();
                resultsDialog.Dispose();
                resultsDialog = null;
            }

            if (resultsDialog == null) { resultsDialog = new PGE_TraceResults(this.CurrentGeometricNetwork, traceType); }

            xyPairsAlreadyDrawn.Clear();

            if (protectiveClassIDs == null) { protectiveClassIDs = PGE_Trace.GetProtectiveClassIDs(); }

            //Clear any current trace graphics
            currentMap = ((IMxDocument)PGE.Common.Delivery.ArcFM.ApplicationFacade.Application.Document).FocusMap;

            ICompositeGraphicsLayer compositeGraphicsLayer = currentMap.ActiveGraphicsLayer as ICompositeGraphicsLayer;
            try
            {
                List<string> toRemove = new List<string>();
                ICompositeLayer compLayer = compositeGraphicsLayer as ICompositeLayer;
                for (int i = 0; i < compLayer.Count; i++)
                {
                    if (compLayer.get_Layer(i).Name.Contains("PGETraceGraphics"))
                    {
                        toRemove.Add(compLayer.get_Layer(i).Name);
                    }
                }
                foreach (string removeLayer in toRemove)
                {
                    try
                    {
                        compositeGraphicsLayer.DeleteLayer(removeLayer);
                    }
                    catch (Exception e) { }
                }
            }
            catch { }

            graphicsLayers.Clear();


            IMMEidSearchResults results = traceResults as IMMEidSearchResults;

            if (networkAnalyst == null)
            {
                networkAnalyst = GetNetworkAnalyst();
            }

            Dictionary<int, IFeature> junctionEIDs = new Dictionary<int, IFeature>();
            Dictionary<int, IFeature> edgeEIDs = new Dictionary<int, IFeature>();

            IEIDHelper eidHelper = new EIDHelperClass();
            eidHelper.ReturnFeatures = true;
            eidHelper.ReturnGeometries = true;
            eidHelper.GeometricNetwork = this.CurrentGeometricNetwork;

            IEnumEIDInfo junctionEIDInfoEnum = eidHelper.CreateEnumEIDInfo(results.Junctions);
            IEIDInfo junctionEIDInfo = null;
            junctionEIDInfoEnum.Reset();
            //IEnumEIDInfo junctionEIDInfo = eidHelper.CreateEnumEIDInfo(junctions);
            while ((junctionEIDInfo = junctionEIDInfoEnum.Next()) != null)
            {
                junctionEIDs.Add(junctionEIDInfo.EID, junctionEIDInfo.Feature);
            }

            IEnumEIDInfo edgeEIDInfoEnum = eidHelper.CreateEnumEIDInfo(results.Edges);
            IEIDInfo edgeEIDInfo = null;
            edgeEIDInfoEnum.Reset();
            //IEnumEIDInfo junctionEIDInfo = eidHelper.CreateEnumEIDInfo(junctions);
            while ((edgeEIDInfo = edgeEIDInfoEnum.Next()) != null)
            {
                edgeEIDs.Add(edgeEIDInfo.EID, edgeEIDInfo.Feature);
            }



            //Draw or select the results based on current configuration
            if (networkAnalyst != null)
            {
                INetworkAnalysisExtResults networkAnalysisResults = networkAnalyst as INetworkAnalysisExtResults;
                networkAnalysisResults.ClearResults();

                if (networkAnalysisResults.ResultsAsSelection)
                {
                    networkAnalysisResults.CreateSelection(results.Junctions, results.Edges);

                    //Custom section for drawing features on map.  This section doesn't fully work as graphics layers don't seem to respect
                    //the associatedLayer visibility properties if they are in group layers. 
                    //Draw our line and junction results to the map
                }
                else
                {
                    //networkAnalysisResults.SetResults(results.Junctions, results.Edges);

                    //Custom section for drawing features on map.  This section doesn't fully work as graphics layers don't seem to respect
                    //the associatedLayer visibility properties if they are in group layers. 
                    //Draw our line and junction results to the map
                    foreach (KeyValuePair<int, IFeature> kvp in junctionEIDs)
                    {
                        AddLineOrJunctionGraphicToMap(kvp.Value);
                    }
                    foreach (KeyValuePair<int, IFeature> kvp in edgeEIDs)
                    {
                        AddLineOrJunctionGraphicToMap(kvp.Value);
                    }
                }
            }

            string traceTitle = "";
            if (traceType == TraceType.Downstream) { traceTitle = "Downstream Trace Results"; }
            else if (traceType == TraceType.DownstreamProtective) { traceTitle = "Downstream Protective Device Trace Results"; }
            else if (traceType == TraceType.Upstream) { traceTitle = "Upstream Trace Results"; }
            else if (traceType == TraceType.UpstreamProtective) { traceTitle = "Upstream Protective Device Trace Results"; }
            resultsDialog.SetTraceTitle(traceTitle);
            //Because ArcFM returns the results in an ordered fashion and not based on the order the nodes are encountered, 
            //we need to traverse the network again to re-order the results properly.
            List<int> orderedEdgeEIDList = new List<int>();
            List<int> orderedJunctionEIDList = new List<int>();

            int startJunctionEID = -1;
            if (edgeEIDs.ContainsKey(this.CurrentStartEID))
            {
                //We found our start edge so let's get the start junction.
                IEdgeFeature edgeFeature = edgeEIDs[this.CurrentStartEID] as IEdgeFeature;
                //If this is a protective device trace then only protective devices should be included
                if ((traceType == TraceType.Upstream || traceType == TraceType.Downstream))
                {
                    resultsDialog.AddTreeNode(GetDisplayString(edgeFeature as IFeature, this.CurrentStartEID.ToString()),
                        edgeFeature as IFeature);
                }
                orderedEdgeEIDList.Add(this.CurrentStartEID);

                ISimpleJunctionFeature ToJunctionFeature = edgeFeature.ToJunctionFeature as ISimpleJunctionFeature;
                if (junctionEIDs.ContainsKey(ToJunctionFeature.EID))
                {
                    startJunctionEID = ToJunctionFeature.EID;
                }
                else
                {
                    //Add our first directional arrow as well
                    AddArrowGraphicToMap(((IFeature)edgeEIDs[this.CurrentStartEID]), ((IFeature)ToJunctionFeature).Shape as IPoint);
                }

                ISimpleJunctionFeature FromJunctionFeature = edgeFeature.FromJunctionFeature as ISimpleJunctionFeature;
                if (junctionEIDs.ContainsKey(FromJunctionFeature.EID))
                {
                    startJunctionEID = FromJunctionFeature.EID;
                }
                else
                {
                    //Add our first directional arrow as well
                    AddArrowGraphicToMap(((IFeature)edgeEIDs[this.CurrentStartEID]), ((IFeature)FromJunctionFeature).Shape as IPoint);
                }
            }
            else
            {
                startJunctionEID = this.CurrentStartEID;
            }

            INetSchema networkSchema = this.CurrentGeometricNetwork.Network as INetSchema;
            INetWeight electricTraceWeight = networkSchema.get_WeightByName("MMELECTRICTRACEWEIGHT");

            ShowElectricConnectivityOrder(this.CurrentGeometricNetwork.Network, 0, startJunctionEID, ref resultsDialog,
                junctionEIDs, edgeEIDs, "", ref orderedJunctionEIDList, ref orderedEdgeEIDList, ref electricTraceWeight);

            ((IMxDocument)PGE.Common.Delivery.ArcFM.ApplicationFacade.Application.Document).ActiveView.Refresh();

            resultsDialog.UpdateTreeList();
            try
            {
                resultsDialog.Show(new ModelessDialog(PGE.Common.Delivery.ArcFM.ApplicationFacade.Application.hWnd));
            }
            catch (Exception e)
            {

            }
        }

        void versionEvents_OnRefreshVersion()
        {
            
        }

        private static bool refreshOnClick = false;
        private static bool canExecute = true;
        public override void OnMouseDown(int Button, int Shift, int X, int Y)
        {            
            canExecute = true;
            base.OnMouseDown(Button, Shift, X, Y);
        }

        /// <summary>
        /// On Mouse up is where the trace will start
        /// </summary>
        /// <param name="button">Any mouse buttons being presses</param>
        /// <param name="shift">Shift key being pressed</param>
        /// <param name="x">X position of mouse</param>
        /// <param name="y">Y position of mouse</param>
        public override void OnMouseUp(int button, int shift, int x, int y)
        {
            if (!canExecute) { return; }
            try
            {
                base.OnMouseUp(button, shift, x, y);

                this.CurrentSnappedFeature = this.CurrentFeatureSnap.SnappedEdgeFeature(this.XCoordinate, this.YCoordinate);
                esriElementType elementType;
                this.CurrentStartEID = GetEID(this.CurrentGeometricNetwork, Document.ActiveView.ScreenDisplay, this.CurrentSnappedFeature, this.XCoordinate, this.YCoordinate, out elementType);
            }
            catch (Exception ex)
            {

            }
            canExecute = false;
        }

        #endregion

        #region Trace Helpers

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
        /// Determines what phases to trace based on the currently selected ArcFM options
        /// </summary>
        /// <returns></returns>
        private mmPhasesToTrace FetchPhasesToTrace()
        {
            object defaultValue = "Any";
            object obj3 = MinerGetSetting(TraceProperties.ElectricSolversRelKey, TraceProperties.PhasesKey, defaultValue);
            if (obj3 != null)
            {
                switch (Convert.ToString(obj3))
                {
                    case "Any":
                        return mmPhasesToTrace.mmPTT_Any;

                    case "ATL_A":
                        return mmPhasesToTrace.mmPTT_AtLeastA;

                    case "ATL_B":
                        return mmPhasesToTrace.mmPTT_AtLeastB;

                    case "ATL_C":
                        return mmPhasesToTrace.mmPTT_AtLeastC;

                    case "ATL_AB":
                        return mmPhasesToTrace.mmPTT_AtLeastAB;

                    case "ATL_BC":
                        return mmPhasesToTrace.mmPTT_AtLeastBC;

                    case "ATL_AC":
                        return mmPhasesToTrace.mmPTT_AtLeastAC;

                    case "A":
                        return mmPhasesToTrace.mmPTT_A;

                    case "B":
                        return mmPhasesToTrace.mmPTT_B;

                    case "C":
                        return mmPhasesToTrace.mmPTT_C;

                    case "AB":
                        return mmPhasesToTrace.mmPTT_AB;

                    case "AC":
                        return mmPhasesToTrace.mmPTT_AC;

                    case "BC":
                        return mmPhasesToTrace.mmPTT_BC;

                    case "ABC":
                        return mmPhasesToTrace.mmPTT_ABC;
                }
            }
            return mmPhasesToTrace.mmPTT_Any;
        }

        /// <summary>
        /// Determines the EID for the feature at the selected x y location on screen
        /// </summary>
        /// <param name="geometricNetwork">Geometric network to get the EID from</param>
        /// <param name="screenDisplay">The screen display where the user clicked</param>
        /// <param name="feature">Feature to determine EID for (can be null)</param>
        /// <param name="x">X position</param>
        /// <param name="y">Y Position</param>
        /// <param name="elementType">Element type that the returned EID is</param>
        /// <returns></returns>
        private static int GetEID(IGeometricNetwork geometricNetwork, IScreenDisplay screenDisplay, IFeature feature, int x, int y, out esriElementType elementType)
        {
            elementType = esriElementType.esriETNone;
            int eID = -1;
            if (geometricNetwork == null)
            {
                return -1;
            }
            if (screenDisplay == null)
            {
                return -1;
            }
            if (feature == null)
            {
                return -1;
            }
            INetElements network = geometricNetwork.Network as INetElements;
            if (network == null)
            {
                return -1;
            }
            ISimpleJunctionFeature feature2 = feature as ISimpleJunctionFeature;
            if (feature2 != null)
            {
                eID = feature2.EID;
                elementType = esriElementType.esriETJunction;
                return eID;
            }
            elementType = esriElementType.esriETEdge;
            IComplexNetworkFeature feature3 = feature as IComplexNetworkFeature;
            if (feature3 == null)
            {
                return network.GetEID(feature.Class.ObjectClassID, feature.OID, -1, esriElementType.esriETEdge);
            }
            IPoint point = screenDisplay.DisplayTransformation.ToMapPoint(x, y);
            if (point == null)
            {
                return eID;
            }
            return feature3.FindEdgeEID(point);
        }

        /// <summary>
        /// Obtains the electric trace settings for this trace
        /// </summary>
        /// <returns></returns>
        private IMMElectricTraceSettingsEx FetchElectricTraceSettings()
        {
            IMMElectricTraceSettingsEx ex = new MMElectricTraceSettingsClass();
            try
            {
                ex.UseFeederManagerProtectiveDevices = false;
                ex.RespectESRIBarriers = true;
                ex.RespectEnabledField = true;
                object defaultValue = 0;
                if (Convert.ToInt32(MinerGetSetting(TraceProperties.ElectricSolversRelKey, TraceProperties.UseFdrSourceKey, defaultValue)) == 1)
                {
                    ex.UseFeederManagerCircuitSources = true;
                }
                else
                {
                    ex.UseFeederManagerCircuitSources = false;
                }
                protectiveClassIDs = PGE_Trace.GetProtectiveClassIDs();
                if (protectiveClassIDs == null)
                {
                    return ex;
                }
                int[] numArray = new int[protectiveClassIDs.Count];
                for (int i = 0; i < protectiveClassIDs.Count; i++)
                {
                    numArray[i] = Convert.ToInt32(protectiveClassIDs[i]);
                }
                ex.ProtectiveDeviceClassIDs = numArray;
            }
            catch
            {
                ex.UseFeederManagerCircuitSources = true;
                ex.UseFeederManagerProtectiveDevices = false;
            }
            return ex;
        }

        /// <summary>
        /// Determines the protective device class IDs for a protective device as selected by the user
        /// </summary>
        /// <returns></returns>
        public static ArrayList GetProtectiveClassIDs()
        {
            ArrayList list = new ArrayList();
            object defaultValue = string.Empty;
            string str = MinerGetSetting(TraceProperties.ElectricSolversRelKey, TraceProperties.DeviceListKey, defaultValue).ToString();
            if (str.Length <= 0)
            {
                MMWorkspaceManagerClass class2 = new MMWorkspaceManagerClass();
                IPropertySet pPS = null;
                IWorkspace pWorkspace = class2.GetWorkspace(pPS, 2);
                IEnumFeatureClass class3 = (Activator.CreateInstance(Type.GetTypeFromProgID("mmGeoDatabase.MMModelNameManager")) as IMMModelNameManager).FeatureClassesFromModelNameWS(pWorkspace, TraceProperties.ProtectiveModelName);
                if (class3 == null)
                {
                    return null;
                }
                class3.Reset();
                for (IFeatureClass class4 = class3.Next(); class4 != null; class4 = class3.Next())
                {
                    list.Add(class4.ObjectClassID);
                }
                return list;
            }
            char[] separator = ":".ToCharArray();
            foreach (string str3 in str.Split(separator))
            {
                if (str3.Length > 0)
                {
                    list.Add(Convert.ToInt32(str3));
                }
            }
            return list;
        }

        #endregion

        #region Private Methods

        private static IVersion currentVersion = null;
        private void ClearResults()
        {
            try
            {
                if (resultsDialog != null)
                {
                    resultsDialog.Close();
                    resultsDialog.Dispose();
                    resultsDialog = null;
                }

                //Clear any current trace graphics
                if (currentMap == null) { currentMap = ((IMxDocument)PGE.Common.Delivery.ArcFM.ApplicationFacade.Application.Document).FocusMap; }

                ICompositeGraphicsLayer compositeGraphicsLayer = currentMap.ActiveGraphicsLayer as ICompositeGraphicsLayer;
                try
                {
                    List<string> toRemove = new List<string>();
                    ICompositeLayer compLayer = compositeGraphicsLayer as ICompositeLayer;
                    for (int i = 0; i < compLayer.Count; i++)
                    {
                        if (compLayer.get_Layer(i).Name.Contains("PGETraceGraphics"))
                        {
                            toRemove.Add(compLayer.get_Layer(i).Name);
                        }
                    }
                    foreach (string removeLayer in toRemove)
                    {
                        try
                        {
                            compositeGraphicsLayer.DeleteLayer(removeLayer);
                        }
                        catch (Exception e) { }
                    }
                }
                catch { }

                currentMap = null;
                graphicsLayers.Clear();
            }
            catch (Exception ex) { }
        }


        /// <summary>
        /// Provides a message to the user in the bottom left corner of ArcMap
        /// </summary>
        /// <param name="statusBarMessage"></param>
        private static void DisplayStatusBarMessage(string statusBarMessage)
        {
            IMMArcGISRuntimeEnvironment environment = new ArcGISRuntimeEnvironment();
            if (environment != null)
            {
                environment.SetStatusBarMessage(statusBarMessage);
            }
        }

        /// <summary>
        /// This method will perform a secondary trace to order the ArcFM results in electrical connectivity order
        /// </summary>
        /// <param name="networkDataset">INetwork for the network that was traced</param>
        /// <param name="startingJunctionEID">EID of the junction to start at</param>
        /// <param name="resultsDialog">Dialog to which the trace results will be added</param>
        /// <param name="junctionEIDs">List of the junction EIDs and their features that were returned by the ArcFM trace</param>
        /// <param name="edgeEIDs">List of the Edge EIDs and their features that were returned by the ArcFM trace</param>
        /// <param name="ParentNodeName">Name of the parent node from the trace results dialog tree</param>
        /// <param name="orderedEIDList">List of the EIDs as they are processed</param>
        private void ShowElectricConnectivityOrder(INetwork networkDataset, object junctionTraceWeight, int startingJunctionEID,
            ref PGE_TraceResults resultsDialog, Dictionary<int, IFeature> junctionEIDs, Dictionary<int, IFeature> edgeEIDs
            , string ParentNodeName, ref List<int> orderedJunctionEIDList, ref List<int> orderedEdgeEIDList, ref INetWeight MMElectricTraceWeight)
        {
            List<int> parentNodesProcessed = new List<int>();
            ProcessJunctionParent(startingJunctionEID, junctionTraceWeight, networkDataset, ref resultsDialog, ref junctionEIDs, ref edgeEIDs, ParentNodeName,
                ref parentNodesProcessed, ref orderedJunctionEIDList, ref orderedEdgeEIDList, ref MMElectricTraceWeight);
        }

        /// <summary>
        /// This method will process a starting junction EID to determine the actual ordering of the ArcFM trace so that it can return an
        /// order which is based on the electrical connectivity of the features, rather than sorting by feature classes.
        /// </summary>
        /// <param name="StartJunctionEID">EID of the junction to start at</param>
        /// <param name="networkDataset">INetwork for the network that was traced</param>
        /// <param name="resultsDialog">Dialog to which the trace results will be added</param>
        /// <param name="junctionEIDs">List of the junction EIDs and their features that were returned by the ArcFM trace</param>
        /// <param name="edgeEIDs">List of the Edge EIDs and their features that were returned by the ArcFM trace</param>
        /// <param name="parentNodeName">Name of the parent node from the trace results dialog tree</param>
        /// <param name="parentNodesProcessed">List of the nodes that have been processed already</param>
        /// <param name="orderedEIDList">List of the EIDs as they are processed</param>
        private void ProcessJunctionParent(int StartJunctionEID, object JunctionTraceWeight, INetwork networkDataset, ref PGE_TraceResults resultsDialog,
            ref Dictionary<int, IFeature> junctionEIDs, ref Dictionary<int, IFeature> edgeEIDs, string parentNodeName, ref List<int> parentNodesProcessed,
            ref List<int> orderedJunctionEIDList, ref List<int> orderedEdgeEIDList, ref INetWeight electricTraceWeight)
        {
            try
            {
                //If we haven't processed this parent yet add it to our list.  If we have, exit the method as we don't need
                //to process it again
                if (parentNodesProcessed.Contains(StartJunctionEID)) { return; }

                IForwardStar forwardStar = networkDataset.CreateForwardStar(false, electricTraceWeight, null, null, null);

                Stack<int> junctionQueue = new Stack<int>();

                junctionQueue.Push(StartJunctionEID);

                int junctionEID = -1;
                while (junctionQueue.Count > 0)
                {
                    junctionEID = junctionQueue.Pop();
                    string junctionDisplayName = "";
                    if (junctionEIDs.ContainsKey(junctionEID))
                    {
                        junctionDisplayName = GetDisplayString(junctionEIDs[junctionEID], junctionEID.ToString());
                    }
                    if (junctionDisplayName != parentNodeName)
                    {
                        //If our original EID list returned by the ArcFM trace doesn't contain this junction eid then move on.  Also,
                        //if we have already added it to our ordered list then we have already processed this junction so we can move one.
                        if (!junctionEIDs.ContainsKey(junctionEID) || orderedJunctionEIDList.Contains(junctionEID)) { continue; }

                        //Add this junction to our ordered EID list
                        orderedJunctionEIDList.Add(junctionEID);
                        resultsDialog.AddTreeNode(junctionDisplayName, junctionEIDs[junctionEID]);
                    }

                    if (traceType == TraceType.Downstream || traceType == TraceType.DownstreamProtective)
                    {
                        //If we are tracing downstream we need to first check if this junction feature is open or closed.  If open
                        //move to the next junction.  Don't want to get the connected edges for this junction.
                        if (!JunctionIsClosed(JunctionTraceWeight)) { continue; }
                    }

                    int edgeCount = 0;

                    //Find our adjacent edges to this junction EID
                    forwardStar.FindAdjacent(0, junctionEID, out edgeCount);

                    if (junctionDisplayName == parentNodeName)
                    {
                        parentNodesProcessed.Add(junctionEID);
                    }

                    //Process all of the edges
                    for (int i = 0; i < edgeCount; i++)
                    {
                        int adjacentEdgeEID = -1;
                        bool reverseOrientation = false;
                        object edgeWeightValue = null;
                        forwardStar.QueryAdjacentEdge(i, out adjacentEdgeEID, out reverseOrientation, out edgeWeightValue);

                        //If our original EID list returned by the ArcFM trace doesn't contain this edge eid then move on.  Also,
                        //if we have already added it to our ordered list then we have already processed this edge so we can move one.
                        if (!edgeEIDs.ContainsKey(adjacentEdgeEID) || orderedEdgeEIDList.Contains(adjacentEdgeEID)) { continue; }

                        //Add this edge to our odered EID list
                        orderedEdgeEIDList.Add(adjacentEdgeEID);

                        AddArrowGraphicToMap(edgeEIDs[adjacentEdgeEID], junctionEIDs[junctionEID].Shape as IPoint);

                        //We will only add to our results dialog if it isn't listed to exclude it
                        resultsDialog.AddTreeNode(GetDisplayString(edgeEIDs[adjacentEdgeEID], adjacentEdgeEID.ToString()), edgeEIDs[adjacentEdgeEID]);

                        //Find the adjacent junction for this edge and add it to our queue to process
                        int adjacentJunctionEID = -1;
                        object junctionWeightValue = null;
                        forwardStar.QueryAdjacentJunction(i, out adjacentJunctionEID, out junctionWeightValue);
                        ProcessJunctionParent(adjacentJunctionEID, junctionWeightValue, networkDataset, ref resultsDialog, ref junctionEIDs,
                            ref edgeEIDs, parentNodeName, ref parentNodesProcessed, ref orderedJunctionEIDList, ref orderedEdgeEIDList, ref electricTraceWeight);
                    }
                }
            }
            catch (Exception e)
            {

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

        /// <summary>
        /// Determines the display string for a feature in the 
        /// </summary>
        /// <param name="feat">Feature to determine display string for</param>
        /// <param name="defaultString">Default display string</param>
        /// <returns></returns>
        private string GetDisplayString(IFeature feat, string defaultString)
        {
            int operatingNumFieldIndex = -1;
            string displayName = "";
            try
            {
                if (OperatingNumberFieldIndex.ContainsKey(feat.Class.AliasName))
                {
                    operatingNumFieldIndex = OperatingNumberFieldIndex[feat.Class.AliasName];
                }
                else
                {
                    operatingNumFieldIndex = ModelNameFacade.FieldIndexFromModelName(feat.Class, SchemaInfo.Electric.FieldModelNames.OperatingNumber);
                    OperatingNumberFieldIndex.Add(feat.Class.AliasName, operatingNumFieldIndex);
                }
                if (operatingNumFieldIndex > -1)
                {
                    IRow row = feat.Table.GetRow(feat.OID);
                    string opNum = "" + row.get_Value(operatingNumFieldIndex);
                    if (!string.IsNullOrEmpty(opNum))
                    {
                        displayName = feat.Class.AliasName + " - Operating Number: " + opNum;
                        return displayName;
                    }
                    //while (Marshal.ReleaseComObject(row) > 0) { }
                }
                displayName = feat.Class.AliasName + " - Object ID: " + feat.OID;
            }
            catch (Exception e)
            {
                return defaultString;
            }
            return displayName;
        }

        /// <summary>
        /// Obtains the desired miner setting 
        /// </summary>
        /// <param name="relativeValue">Relative registry value in the ArcFM registry location</param>
        /// <param name="valueName">Name of the registry entry</param>
        /// <param name="defaultValue">Default value if the registry entry can't be found</param>
        /// <returns></returns>
        private static object MinerGetSetting(string relativeValue, string valueName, object defaultValue)
        {
            IMMRegistry registry = new MMRegistry();
            if (registry == null)
            {
                return null;
            }
            registry.OpenKey(mmHKEY.mmHKEY_CURRENT_USER, mmBaseKey.mmArcFM, relativeValue);
            return registry.Read(valueName, defaultValue);
        }

        /// <summary>
        /// Obtains the list of IGraphicsLayers for a given feature class from the current map
        /// </summary>
        /// <param name="featClass">Feature class to get associated IGraphicsLayer</param>
        /// <returns></returns>
        private List<IGraphicsLayer> GetGraphicLayers(IFeatureClass featClass)
        {
            if (graphicsLayers.ContainsKey(featClass))
            {
                return graphicsLayers[featClass];
            }
            else
            {
                if (currentMap == null) { currentMap = ((IMxDocument)PGE.Common.Delivery.ArcFM.ApplicationFacade.Application.Document).FocusMap; }

                List<IGraphicsLayer> tempGraphicsLayers = getGraphicsLayers();
                foreach (IGraphicsLayer graphLayer in tempGraphicsLayers)
                {
                    try
                    {
                        if (graphicsLayers.ContainsKey(((IFeatureLayer)graphLayer.AssociatedLayer).FeatureClass))
                        {
                            graphicsLayers[((IFeatureLayer)graphLayer.AssociatedLayer).FeatureClass].Add(graphLayer);
                        }
                        else
                        {
                            graphicsLayers.Add(((IFeatureLayer)graphLayer.AssociatedLayer).FeatureClass, new List<IGraphicsLayer>());
                            graphicsLayers[((IFeatureLayer)graphLayer.AssociatedLayer).FeatureClass].Add(graphLayer);
                        }
                    }
                    catch (Exception e) 
                    {
                    }
                }
                if (graphicsLayers.ContainsKey(featClass))
                {
                    return graphicsLayers[featClass];
                }
                else
                {
                    return null;
                }
            }
        }

        /*
        /// <summary>
        /// Parse through the specified layers 
        /// </summary>
        /// <param name="layers"></param>
        /// <param name="compositeGraphicsLayer"></param>
        private void ParseLayers(List<ILayer> layers, ICompositeGraphicsLayer compositeGraphicsLayer)
        {
            foreach (ILayer layer in layers)
            {
                if (layer is IGroupLayer)
                {                    
                    IGraphicsLayer testLayer = null;
                    try
                    {
                        testLayer = compositeGraphicsLayer.FindLayer(layer.Name + "_PGETraceGraphics");
                    }
                    catch { }
                    if (testLayer == null)
                    {
                        IGraphicsLayer graphLayer = compositeGraphicsLayer.AddLayer(layer.Name + "_PGETraceGraphics", null);
                        ICompositeGraphicsLayer newCompGraphicsLayer = graphLayer as ICompositeGraphicsLayer;
                        IGraphicsContainer graphContainer = graphLayer as IGraphicsContainer;
                        CompositeGraphicsLayerClass compGraphLayer;

                        newCompGraphicsLayer.AssociatedLayer = layer;
                        newCompGraphicsLayer.UseAssociatedLayerVisibility = true;
                        ((ILayer)newCompGraphicsLayer).Name = layer.Name + "_PGETraceGraphics";
                        ICompositeLayer groupLayer = layer as ICompositeLayer;
                        List<ILayer> groupsLayers = new List<ILayer>();
                        for (int i = 0; i < groupLayer.Count; i++)
                        {
                            groupsLayers.Add(groupLayer.get_Layer(i));
                        }
                        ParseLayers(groupsLayers, newCompGraphicsLayer);
                    }
                }
                else if (layer is IFeatureLayer)
                {
                    IGraphicsLayer graphicLayer = compositeGraphicsLayer.AddLayer(layer.Name + "_PGETraceGraphics", null);
                    graphicLayer.AssociatedLayer = layer;
                    graphicLayer.UseAssociatedLayerVisibility = true;
                    IFeatureLayer featLayer = layer as IFeatureLayer;
                    if (!graphicsLayers.ContainsKey(featLayer.FeatureClass))
                    {
                        graphicsLayers.Add(featLayer.FeatureClass, new List<IGraphicsLayer>());
                    }
                    graphicsLayers[featLayer.FeatureClass].Add(graphicLayer);
                }
            }
        }
        */

        /// <summary>
        /// Obtains a list of graphics layers for all IFeatureLayers in the map
        /// </summary>
        /// <returns></returns>
        private List<IGraphicsLayer> getGraphicsLayers()
        {
            ICompositeGraphicsLayer compositeGraphicsLayer = currentMap.ActiveGraphicsLayer as ICompositeGraphicsLayer;
            List<string> featLayerNames = new List<string>();
            List<IGraphicsLayer> featLayers = new List<IGraphicsLayer>();
            IMap focusMap = ((IMxDocument)PGE.Common.Delivery.ArcFM.ApplicationFacade.Application.Document).FocusMap;
            UID featureUID = new UIDClass();
            featureUID.Value = "{40A9E885-5533-11d0-98BE-00805F7CED21}";
            IEnumLayer featureLayers = focusMap.get_Layers(featureUID, true);
            featureLayers.Reset();
            IFeatureLayer featLayer = null;
            while ((featLayer = featureLayers.Next() as IFeatureLayer) != null)
            {
                try
                {
                    if (featLayer.FeatureClass == null) { continue; }
                    //Name based on a counter if there are multiple layers with the same name
                    int counter = 0;
                    while (featLayerNames.Contains(featLayer.Name + counter + "_PGETraceGraphics")) { counter++; }
                    featLayers.Add(compositeGraphicsLayer.AddLayer(featLayer.Name + counter + "_PGETraceGraphics", featLayer));
                    featLayerNames.Add(featLayer.Name + counter + "_PGETraceGraphics");     
                }
                catch (Exception e)
                {

                }
            }
            return featLayers;
        }

        /// <summary>
        /// Obtains a list of ILayers based on the specified UID value
        /// </summary>
        /// <param name="UID"></param>
        /// <returns></returns>
        public static List<ILayer> getFeatureLayers(string UID)
        {
            List<ILayer> featLayers = new List<ILayer>();
            IMap focusMap = ((IMxDocument)PGE.Common.Delivery.ArcFM.ApplicationFacade.Application.Document).FocusMap;
            UID featureUID = null;
            if (!string.IsNullOrEmpty(UID))
            {
                featureUID = new UIDClass();
                featureUID.Value = "{40A9E885-5533-11d0-98BE-00805F7CED21}";
            }
            IEnumLayer featureLayers = focusMap.get_Layers(featureUID, true);
            featureLayers.Reset();
            ILayer featLayer = null;
            while ((featLayer = featureLayers.Next() as ILayer) != null)
            {
                ILayer layer = featLayer as ILayer;
                featLayers.Add(featLayer);
            }
            return featLayers;
        }

        /// <summary>
        /// Adds a given element to the specified feature class's graphics layers
        /// </summary>
        /// <param name="element"></param>
        /// <param name="featClass"></param>
        private void AddElementToGraphicsContainers(IElement element, IFeatureClass featClass)
        {
            try
            {
                List<IGraphicsLayer> graphicLayers = GetGraphicLayers(featClass);
                foreach (IGraphicsLayer graphicLayer in graphicLayers)
                {
                    IGraphicsContainer container = graphicLayer as IGraphicsContainer;
                    container.AddElement(element, 0);
                }
            }
            catch (Exception ex)
            {

            }
        }

        /// <summary>
        /// This method will draw a directional arrow on the line segment passed in between each junction on the line in the direction
        /// of electrical flow.  The starting junction is assumed to be the upstream most junction.
        /// </summary>
        /// <param name="lineGeometry">Line geometry to have directional arrows drawn</param>
        /// <param name="startingJunction">Upstream most junction on the line.  This should be an endpoint of the passed in line geometry</param>
        private void AddLineOrJunctionGraphicToMap(IFeature Feature)
        {
            if ((traceType == TraceType.Downstream || traceType == TraceType.Upstream) || ((traceType == TraceType.DownstreamProtective || traceType == TraceType.UpstreamProtective)
               && protectiveClassIDs.Contains(Feature.Class.ObjectClassID)))
            {

                IGeometry geometry = Feature.Shape;
                if (currentMap == null) { currentMap = ((IMxDocument)PGE.Common.Delivery.ArcFM.ApplicationFacade.Application.Document).FocusMap; }

                IRgbColor redColor = new RgbColorClass();
                redColor.Blue = 0;
                redColor.Red = 255;
                redColor.Green = 255;

                //IGraphicsContainer graphicsContainer = GetPGEGraphicLayer() as IGraphicsContainer;
                //IGraphicsContainer graphicsContainer = (IGraphicsContainer)map;
                if ((geometry.GeometryType) == esriGeometryType.esriGeometryPoint)
                {
                    SimpleMarkerSymbol markerSymbol = new SimpleMarkerSymbol
                    {
                        Color = redColor,
                        Size = 5
                    };

                    //Add our new arrow marker at this point
                    IMarkerElement markerElement = new MarkerElementClass();
                    markerElement.Symbol = markerSymbol;
                    IElement pointElement = (IElement)markerElement;
                    pointElement.Geometry = geometry;
                    AddElementToGraphicsContainers(pointElement, Feature.Class as IFeatureClass);
                    //directionalArrowsGroup.AddElement(pointElement);
                }
                else if ((geometry.GeometryType) == esriGeometryType.esriGeometryPolyline)
                {
                    //  Line elements
                    ISimpleLineSymbol simpleLineSymbol = new SimpleLineSymbolClass();
                    simpleLineSymbol.Color = redColor;
                    simpleLineSymbol.Style = esriSimpleLineStyle.esriSLSSolid;
                    simpleLineSymbol.Width = 2.5;

                    ILineElement lineElement = new LineElementClass();
                    lineElement.Symbol = simpleLineSymbol;
                    IElement element = (IElement)lineElement;

                    if (!(element == null))
                    {
                        element.Geometry = geometry;
                        AddElementToGraphicsContainers(element, Feature.Class as IFeatureClass);
                    }
                }
            }
        }

        /// <summary>
        /// This method will draw a directional arrow on the line segment passed in between each junction on the line in the direction
        /// of electrical flow.  The starting junction is assumed to be the upstream most junction.
        /// </summary>
        /// <param name="lineGeometry">Line geometry to have directional arrows drawn</param>
        /// <param name="startingJunction">Upstream most junction on the line.  This should be an endpoint of the passed in line geometry</param>
        private void AddArrowGraphicToMap(IFeature lineFeature, IPoint startingJunction)
        {
            if ((traceType == TraceType.Downstream || traceType == TraceType.Upstream))
            {
                IGeometry lineGeometry = lineFeature.Shape;
                if (currentMap == null) { currentMap = ((IMxDocument)PGE.Common.Delivery.ArcFM.ApplicationFacade.Application.Document).FocusMap; }

                IRgbColor blackColor = new RgbColorClass();
                blackColor.Blue = 0;
                blackColor.Red = 0;
                blackColor.Green = 0;

                IRgbColor redColor = new RgbColorClass();
                redColor.Blue = 0;
                redColor.Red = 255;
                redColor.Green = 0;

                //IGraphicsContainer graphicsContainer = GetPGEGraphicLayer() as IGraphicsContainer;
                //IGraphicsContainer graphicsContainer = (IGraphicsContainer)map;
                if ((lineGeometry.GeometryType) == esriGeometryType.esriGeometryPolyline)
                {
                    //Define our arrow marker symbol
                    ArrowMarkerSymbol arrowMarker = new ArrowMarkerSymbolClass
                    {
                        Angle = 90,
                        Length = 9,
                        Width = 9,
                        Size = 4,
                        Color = blackColor,
                        XOffset = 0,
                        YOffset = 0,
                        Style = esriArrowMarkerStyle.esriAMSPlain
                    };

                    IPointCollection pointCollection = (IPointCollection)lineGeometry;

                    //We have to draw the arrow in the correct direction so we need to determine where the
                    //start junction lies in relation to how the point collection is organized
                    bool reverseDirection = false;
                    IPoint firstPoint = pointCollection.get_Point(0);
                    double xDifference = Math.Round(firstPoint.X - startingJunction.X, 3);
                    double yDifference = Math.Round(firstPoint.Y - startingJunction.Y, 3);
                    if (xDifference != 0 || yDifference != 0) { reverseDirection = true; }

                    for (int i = 0; i < pointCollection.PointCount; i++)
                    {
                        int index = i;
                        int nextIndex = i + 1;
                        if (reverseDirection)
                        {
                            index = pointCollection.PointCount - i - 1;
                            nextIndex = pointCollection.PointCount - i - 2;
                        }
                        IPoint comparisonPoint = null;
                        IPoint currentPoint = pointCollection.get_Point(index);

                        double xDiff = 0.0;
                        double yDiff = 0.0;

                        //Determine the angle between this point and the next point
                        if (i == pointCollection.PointCount - 1)
                        {
                            //No next index so skip this one.  Let the next edge pick it up
                            continue;
                        }
                        else
                        {
                            comparisonPoint = pointCollection.get_Point(nextIndex);
                            xDiff = comparisonPoint.X - currentPoint.X;
                            yDiff = comparisonPoint.Y - currentPoint.Y;
                        }

                        //Determine the center of this line segment to draw the arrow at that point
                        IPoint pointToDrawArrowAt = new PointClass();
                        for (int iDiv = 1; iDiv < 10; ++iDiv)
                        {
                            pointToDrawArrowAt.SpatialReference = currentPoint.SpatialReference;
                            pointToDrawArrowAt.X = iDiv * (currentPoint.X + comparisonPoint.X) / 10;
                            pointToDrawArrowAt.Y = iDiv * (currentPoint.Y + comparisonPoint.Y) / 10;

                            //Determine length of line segment
                            double xLength = Math.Abs(currentPoint.X - comparisonPoint.X);
                            double yLength = Math.Abs(currentPoint.Y - comparisonPoint.Y);
                            double length = Math.Sqrt((xLength * 10.0) + (yLength * 10.0));
                            if (length < 7.5)
                            {
                                continue;
                            }

                            //Check whether we have already added this junction directional arror.
                            if (xyPairsAlreadyDrawn.ContainsKey(pointToDrawArrowAt.X))
                            {
                                //The x value is contained.  Let's check the Y point.
                                if (xyPairsAlreadyDrawn[pointToDrawArrowAt.X].Contains(pointToDrawArrowAt.Y))
                                {
                                    continue;
                                }
                            }
                            else
                            {
                                xyPairsAlreadyDrawn.Add(pointToDrawArrowAt.X, new List<double>());
                            }

                            double angle = 0.0;
                            if (xDiff == 0.0 && yDiff >= 0) { angle = 90.0; }
                            else if (xDiff == 0.0 && yDiff < 0) { angle = 270.0; }
                            else if (xDiff < 0.0 && yDiff == 0.0) { angle = 180; }
                            else
                            {
                                angle = Math.Atan(Math.Abs(yDiff) / Math.Abs(xDiff)) * (180 / Math.PI);
                                //Add our offsets for different quadrants
                                if (yDiff >= 0 && xDiff < 0) { angle = 180.0 - angle; }
                                else if (xDiff < 0 && yDiff < 0) { angle = 180.0 + angle; }
                                else if (yDiff < 0 && xDiff >= 0) { angle = 0.0 - angle; }
                            }

                            if (traceType == TraceType.Upstream || traceType == TraceType.UpstreamProtective)
                            {
                                //Flip by 180 degrees so that it always shows direction of downstream
                                angle += 180;
                            }

                            xyPairsAlreadyDrawn[pointToDrawArrowAt.X].Add(pointToDrawArrowAt.Y);

                            //Add our new arrow marker at this point
                            IMarkerElement markerElement = new MarkerElementClass();
                            arrowMarker.Angle = angle;
                            markerElement.Symbol = arrowMarker;
                            IElement pointElement = (IElement)markerElement;
                            pointElement.Geometry = pointToDrawArrowAt;
                            AddElementToGraphicsContainers(pointElement, lineFeature.Class as IFeatureClass);
                            //directionalArrowsGroup.AddElement(pointElement);
                        }
                    }
                }
            }
        }

        #endregion

    }

    /// <summary>
    /// Enumeration to specify the type of a trace
    /// </summary>
    public enum TraceType
    {
        Downstream,
        Upstream,
        DownstreamProtective,
        UpstreamProtective
    }
}
