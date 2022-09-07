using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using PGE.Common.Delivery.ArcFM;
using PGE.Common.Delivery.Diagnostics;
using System.Reflection;
using ESRI.ArcGIS.Geodatabase;
using PGE.Common.Delivery.Framework;
using Miner.Interop;
using Miner.ComCategories;
using ESRI.ArcGIS.NetworkAnalysis;

namespace PGE.Desktop.EDER.AutoUpdaters.Special
{
    [Guid("afe808bc-76ec-4da1-8e15-f876024ad02d")]
    [ClassInterface(ClassInterfaceType.None)]
    [ProgId("PGE.Desktop.EDER.PGEInheritNominalVoltageAU")]
    [ComponentCategory(ComCategory.SpecialAutoUpdateStrategy)]
    public class PGEInheritNominalVoltageAU : BaseSpecialAU
    {
        #region Private Static readonly fields
        private static readonly Log4NetLogger _logger = new Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "EDERDesktop.log4net.config");
        private static readonly string _nomialVoltageFieldName = "NOMINALVOLTAGE";
        private static readonly string _ouputVoltageFieldName = "OUTPUTVOLTAGE";
        private const string StepDown = "PGE_STEPDOWN";
        private const string CircuitSource = "CircuitSource";
        private TraceType traceType;
        private Dictionary<string, int> OperatingNumberFieldIndex = new Dictionary<string, int>();
        #endregion

        #region Constructor / Desctructor

        /// <summary>
        /// Constructor, pass in name.
        /// </summary>
        /// 
        public PGEInheritNominalVoltageAU()
            : base("PGE Inherit Nominal Voltage AU") {}

        #endregion
        
        #region Base special AU Overrides
        /// <summary>
        /// Determines in which class the AU will be enabled
        /// </summary>
        /// <param name="objectClass"> Object's class. </param>
        /// <param name="eEvent">The edit event.</param>
        /// <returns> <c>true</c> if the AuoUpdater should be enabled; otherwise <c>false</c> </returns>
        protected override bool InternalEnabled(IObjectClass objectClass, mmEditEvent eEvent)
        {
            bool enabled = false;

            if (eEvent == mmEditEvent.mmEventFeatureCreate || eEvent == mmEditEvent.mmEventFeatureUpdate)
            {

                if (objectClass.Fields.FindField(_nomialVoltageFieldName) != -1)
                    enabled = true;
            }

            return enabled;
        }

        /// <summary>
        /// Determines whether actually this AU should be run, based on the AU Mode.
        /// </summary>
        /// <param name="eAUMode"> The auto updater mode. </param>
        /// <returns> <c>true</c> if the AuoUpdater should be executed; otherwise <c>false</c> </returns>
        protected override bool CanExecute(mmAutoUpdaterMode eAUMode)
        {
            if (eAUMode == mmAutoUpdaterMode.mmAUMArcMap)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        protected override void InternalExecute(IObject obj, mmAutoUpdaterMode eAUMode, mmEditEvent eEvent)
        {
            // check if event is create or update
            if (eEvent == mmEditEvent.mmEventFeatureCreate || eEvent == mmEditEvent.mmEventFeatureUpdate)
            {
                // if event is update then check whether shape is changed or not.
                // if shape is changed then inherit voltage from connected source line.
                // else do nothing.
                if (eEvent == mmEditEvent.mmEventFeatureUpdate)
                {
                    IFeatureChanges objectFeatureChanges = obj as IFeatureChanges;
                    if (objectFeatureChanges.ShapeChanged)
                    {
                        // inherit operating voltage
                        UpdateVoltage(obj);
                    }
                }
                else if (eEvent == mmEditEvent.mmEventFeatureCreate) // Feature create event
                {
                    // inherit operating voltage
                    UpdateVoltage(obj);
                }
            }
        }

        #endregion

        private IFeature GetFirstFeatureOfType(IFeature sourceFeature, esriElementType returnElementType)
        {
            IGeometricNetwork geomNetwork = (sourceFeature as INetworkFeature).GeometricNetwork;
            IMMFeedPath[] feedPaths = TraceFacade.GetFeedPaths(sourceFeature);
            if (feedPaths.Length == 0) return null;
            IFeature retVal = null;
            IFeatureClassContainer fcContainer = (IFeatureClassContainer)geomNetwork;
            foreach (IMMFeedPath path in feedPaths)
            {
                foreach (IMMPathElement pathElement in path.PathElements)
                {
                    if (pathElement.ElementType == returnElementType)
                    {
                        retVal = TraceFacade.GetFeatureFromPathElement(pathElement, geomNetwork);
                        if (retVal.Class.AliasName == "Step Down" || retVal.Class.AliasName == "Electric Stitch Point")
                            return retVal;
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// This method will update the voltage of a feature object.
        /// </summary>
        /// <param name="obj">IObject (Feature)</param>
        private void UpdateVoltage(IObject obj)
        {
            //Checking whether the obj geometry is point type or line type
            Boolean isPointFeature = ((obj as IFeature).Shape.GeometryType == ESRI.ArcGIS.Geometry.esriGeometryType.esriGeometryPoint);
            //if obj is Point
            if (isPointFeature)
            {
                //UpdatePoint(obj);
            }
            else
            {
                UpdateLine(obj);
            }

        }

        /// <summary>
        /// This method will update the operating voltage of a line feature
        /// </summary>
        /// <param name="obj">Line object</param>
        private void UpdateLine(IObject obj, bool callStore = false)
        {            
            
            int incommingNVIndex = obj.Class.Fields.FindField(_nomialVoltageFieldName);
            bool bolStepDownfeature = false;
            IRow circuitRow = CheckforStepDownFeature(obj as IFeature, out bolStepDownfeature);

            if (circuitRow == null && !(obj.get_Value(obj.Fields.FindField("CIRCUITID")) is DBNull) && !bolStepDownfeature)
            {
                //No feature found. Nothing to do
                return;
            }
            else if (circuitRow != null && !bolStepDownfeature)
            {
                //Update the Nominal Voltage of Circuit Source
                obj.set_Value(incommingNVIndex, circuitRow.get_Value(circuitRow.Fields.FindField(_nomialVoltageFieldName)));
                return;
            } 

            IFeature retVal = GetFirstFeatureOfType(obj as IFeature, esriElementType.esriETJunction);
            if(retVal != null )
            {
                if (retVal.Class.AliasName == "Step Down")
                {
                    obj.set_Value(incommingNVIndex, retVal.get_Value(retVal.Fields.FindField(_ouputVoltageFieldName)));
                }
                else if (retVal.Class.AliasName == "Electric Stitch Point")
                {
                    circuitRow = CheckforStepDownFeature(retVal as IFeature, out bolStepDownfeature );
                    obj.set_Value(incommingNVIndex, circuitRow.get_Value(circuitRow.Fields.FindField(_nomialVoltageFieldName)));
                }
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
             Dictionary<int, IFeature> junctionEIDs, Dictionary<int, IFeature> edgeEIDs
            , string ParentNodeName, ref List<int> orderedJunctionEIDList, ref List<int> orderedEdgeEIDList, ref INetWeight MMElectricTraceWeight)
        {
            List<int> parentNodesProcessed = new List<int>();
            ProcessJunctionParent(startingJunctionEID, junctionTraceWeight, networkDataset,  ref junctionEIDs, ref edgeEIDs, ParentNodeName,
                ref parentNodesProcessed, ref orderedJunctionEIDList, ref orderedEdgeEIDList, ref MMElectricTraceWeight);
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
        private void ProcessJunctionParent(int StartJunctionEID, object JunctionTraceWeight, INetwork networkDataset, 
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
                        //resultsDialog.AddTreeNode(junctionDisplayName, junctionEIDs[junctionEID]);
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

                        //AddArrowGraphicToMap(edgeEIDs[adjacentEdgeEID], junctionEIDs[junctionEID].Shape as IPoint);

                        //We will only add to our results dialog if it isn't listed to exclude it
                        //resultsDialog.AddTreeNode(GetDisplayString(edgeEIDs[adjacentEdgeEID], adjacentEdgeEID.ToString()), edgeEIDs[adjacentEdgeEID]);

                        //Find the adjacent junction for this edge and add it to our queue to process
                        int adjacentJunctionEID = -1;
                        object junctionWeightValue = null;
                        forwardStar.QueryAdjacentJunction(i, out adjacentJunctionEID, out junctionWeightValue);
                        ProcessJunctionParent(adjacentJunctionEID, junctionWeightValue, networkDataset, ref junctionEIDs,
                            ref edgeEIDs, parentNodeName, ref parentNodesProcessed, ref orderedJunctionEIDList, ref orderedEdgeEIDList, ref electricTraceWeight);
                    }
                }
            }
            catch (Exception e)
            {

            }
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

        private IRow CheckforStepDownFeature(IFeature obj, out bool bolStepDown)
        {
            ICursor cursor = null;
            IRow retVal = null;
            IRow pRow = null;
            bolStepDown = false;
            IObjectClass _queryTable = null;

            try
            {
                IQueryFilter qf = new QueryFilterClass();
                qf.WhereClause = "CIRCUITID='" + obj.get_Value(obj.Fields.FindField("CIRCUITID")) + "'";
                IWorkspace wkspc = ((IDataset)obj.Class).Workspace;
                if (wkspc is IFeatureWorkspace)
                {
                    _queryTable = ModelNameFacade.ObjectClassByModelName(wkspc, StepDown);
                }

                cursor = ((ITable)_queryTable).Search(qf, false);
                pRow = cursor.NextRow();
                                
                if (pRow != null)
                   bolStepDown = true;
                
                if (cursor != null)                    
                    while (Marshal.ReleaseComObject(cursor) > 0) { };
                    
                _queryTable = ModelNameFacade.ObjectClassByModelName(wkspc, CircuitSource);
                cursor = ((ITable)_queryTable).Search(qf, false);
                retVal = cursor.NextRow();             
               
                //release the cursor
            }
            catch (Exception ex)
            {
                _logger.Debug(ex.Message, ex);
            }
            finally
            {
                if (cursor != null)
                {
                    while (Marshal.ReleaseComObject(cursor) > 0) { };
                }
            }

            return retVal;
            
        }
    }
}
