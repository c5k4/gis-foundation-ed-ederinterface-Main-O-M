using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ESRI.ArcGIS.EditorExt;
using ESRI.ArcGIS.Geodatabase;
using Miner.Interop;
using PGE.Common.Delivery.Framework;

namespace PGE.Desktop.EDER.ValidationRules.VoltageValidators
{
    /// <summary>
    /// Builds validatior objects.  Common tracing logic is stored here for reuse and to isolate it form the validator business logic.
    /// </summary>
    public class VoltageValidatorFactory : IMMCurrentStatus
    {

        /// <summary>
        /// Network Analysis Extension for tracing.
        /// </summary>
        private INetworkAnalysisExt naExt;

        /// <summary>
        /// Initializes a new instance of the <see cref="VoltageValidatorFactory"/> class.
        /// </summary>
        /// <param name="naExt">The na ext.</param>
        public VoltageValidatorFactory(INetworkAnalysisExt naExt) {
            this.naExt = naExt;
        }

        /// <summary>
        /// Creates s secondary voltage validator vby tracing upstream to the first transformer junction feature.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="XfrModelNames">The XFR model names.</param>
        /// <returns></returns>
        public SecVoltageValidator CreateSecondaryVoltageValidator(IFeature source, string[] XfrModelNames) {

            //Set up for the trace
            IGeometricNetwork geomNetwork = (source as INetworkFeature).GeometricNetwork; //naExt.CurrentNetwork;

            //; Find the source EID.
            int startEID = 0;

            if (source is ISimpleEdgeFeature)
            {
                startEID = (source as ISimpleEdgeFeature).EID;
            }
            else if (source is IComplexEdgeFeature)
            {
                startEID = GetFeatureEID(source);
            }
            else
            {
                return null;
            }

            // Execute the trace.
            int[] barrierJuncs = new int[0];
            int[] barrierEdges = new int[0];
            IMMFeedPath[] feedPaths;
            IMMTraceStopper[] traceStopperJuncs;
            IMMTraceStopper[] traceStopperEdges;
            //Call the trace
            IMMElectricTraceSettings mmElectricTraceSettings = new MMElectricTraceSettingsClass();
            IMMElectricTracing mmElectricTracing = new MMFeederTracerClass();

            mmElectricTracing.FindFeedPaths(
                geomNetwork, //The geometric network (derived from the ServicePoint feature)
                mmElectricTraceSettings, //Setting regarding whether to respect phasing and Enabled status

                this, //The class that implements IMMCurrentStatus

                startEID, //The EID of where the trace should start
                esriElementType.esriETEdge,  //What kind of EID is startEID
                SetOfPhases.abc, //What phases should be traced

                barrierJuncs, //What junctions should act as barriers 
                barrierEdges, //What edges should act as barriers

                out feedPaths, //The path(s) to the source features
                out traceStopperJuncs, //Junctions that stopped the trace
                out traceStopperEdges); //Edges that stopped the trace

            if (feedPaths.Length != 1) return null;

            IMMPathElement xfr = null;

            IFeatureClassContainer fcContainer = (IFeatureClassContainer)geomNetwork;
            IFeatureClass fc = null;

            // Find the first upstream xfr.
            foreach (IMMFeedPath path in feedPaths)
            {

                foreach (IMMPathElement element in path.PathElements)
                {
                    fc = fcContainer.get_ClassByID(element.FCID);

                    if (ModelNameFacade.ContainsClassModelName(fc, XfrModelNames)) {
                        xfr = element;
                        break;

                    }
                }
                
                if (xfr != null) break;
            }

            if (xfr == null || fc == null) return null;

            // Return the validator.
            SecVoltageValidator validator = new SecVoltageValidator(source, fc.GetFeature(xfr.OID));

            return validator;
        }

        /// <summary>
        /// Creates the primary voltage validator.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <returns></returns>
        public PriVoltageValidator CreatePrimaryVoltageValidator(IFeature source)
        {
            
            // int startEID = 0;

            if (source is ISimpleEdgeFeature)
            {
                // startEID = (source as ISimpleEdgeFeature).EID;
                return CreateEdgePrimaryVoltageValidator(source);
            }
            else if (source is IComplexEdgeFeature)
            {
                // startEID = GetFeatureEID(source);
                return CreateEdgePrimaryVoltageValidator(source);
            }
            else if (source is ISimpleJunctionFeature) 
            {
                // startEID = (source as ISimpleJunctionFeature).EID;
                return CreateJunctionPrimaryVoltageValidator(source);

            } else if (source is ComplexJunctionFeature) 
            {
                // startEID = GetFeatureEID(source);
                return CreateJunctionPrimaryVoltageValidator(source);
            }

            return null;
        }
        /// <summary>
        /// Creates the edge primary voltage validator.
        /// </summary>
        /// <param name="feature">The feature being validated.</param>
        /// <returns></returns>
        private PriVoltageValidator CreateEdgePrimaryVoltageValidator(IFeature feature)
        {

            List<IFeature> upstreamSourceLines = TraceFacade.GetFirstUpstreamEdgePerPath(feature);
            IFeature stepdown = null;
            IFeature upstreamJunction = TraceFacade.GetFirstUpstreamJunction(feature);
            if ((upstreamJunction != null) && (ModelNameFacade.ContainsClassModelName(upstreamJunction.Class, SchemaInfo.Electric.ClassModelNames.StepDown)))
            {
                stepdown = upstreamJunction;
            }
            PriVoltageValidator validator = new PriVoltageValidator(upstreamSourceLines, feature, stepdown);

            return validator;
        }

        /// <summary>
        /// Creates the junction primary voltage validator.
        /// </summary>
        /// <param name="feature">The feature being validated.</param>
        /// <returns></returns>
        private PriVoltageValidator CreateJunctionPrimaryVoltageValidator(IFeature feature)
        {
            List<IFeature> upstreamSourceLines = TraceFacade.GetFirstUpstreamEdgePerPath(feature);

            // Open devices usually don't trace, need to get the upstream connected line instead.
            if ((upstreamSourceLines == null) || (upstreamSourceLines.Count == 0))
            {
                if (IsOpen(feature))
                {
                    ISimpleJunctionFeature simpleJunction = feature as ISimpleJunctionFeature;
                    int[] conductorEdgeIndexs = ValidateClosedDevice.GetConductorsAtJunction(simpleJunction);
                    // Get the Features connecting the Switch.
                    for (int i = 0; i < conductorEdgeIndexs.Count(); i++)
                    {
                        IFeature source = simpleJunction.get_EdgeFeature(conductorEdgeIndexs[i]) as IFeature;
                        if (source != null)
                        {
                            upstreamSourceLines.Add(source);
                        }
                    }
                }


            }
            // If still no upstream lets try just getting the first one.
            if ((upstreamSourceLines == null) || (upstreamSourceLines.Count == 0))
            {
                IFeature source = TraceFacade.GetFirstUpstreamEdge(feature);
                if (source != null)
                {
                    upstreamSourceLines.Add(source);
                }
            }
            PriVoltageValidator validator = new PriVoltageValidator(upstreamSourceLines, feature, null);
            return validator;
        }


        /// <summary>
        /// Check the normal position fields and determine if this device is open.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private bool IsOpen(IFeature feat)
        {
            // Assign normal positions A,B,C to string array.
            string[] normalPostionModelNames = { SchemaInfo.Electric.FieldModelNames.NormalpositionA, SchemaInfo.Electric.FieldModelNames.NormalpositionB, SchemaInfo.Electric.FieldModelNames.NormalPositionC };
            bool isOpen = false;

            foreach (string normalPostionModelName in normalPostionModelNames)
            {
                int? normalstatus = (int?)feat.GetFieldValue(null, false, normalPostionModelName);
                // If value equal to 0 it's open.
                if (normalstatus == 0)
                {
                    isOpen = true;
                    break;
                }
            }
            return isOpen;
        }

        // Q1 -2021 QA/QC primary voltage rule change for ED-GIS Scripting project - Old Code.

        ///// <summary>
        ///// Creates the edge primary voltage validator.
        ///// </summary>
        ///// <param name="source">The source.</param>
        ///// <param name="startEID">The start EID.</param>
        ///// <returns></returns>
        // private PriVoltageValidator CreateEdgePrimaryVoltageValidator(IFeature source, int startEID)
        // {

        //    //Set up for the trace
        //    IGeometricNetwork geomNetwork = (source as INetworkFeature).GeometricNetwork; //naExt.CurrentNetwork;

        //    IMMFeedPath[] feedPaths;
        //    IMMTraceStopper[] traceStopperJuncs;
        //    IMMTraceStopper[] traceStopperEdges;
        //    //Call the trace
        //    new MMFeederTracerClass().FindFeedPaths(
        //        geomNetwork, //The geometric network (derived from the ServicePoint feature)
        //        new MMElectricTraceSettingsClass(), //Setting regarding whether to respect phasing and Enabled status

        //        this, //The class that implements IMMCurrentStatus

        //        startEID, //The EID of where the trace should start
        //        esriElementType.esriETEdge,  //What kind of EID is startEID
        //        SetOfPhases.abc, //What phases should be traced

        //        new int[0], //What junctions should act as barriers 
        //        new int[0], //What edges should act as barriers

        //        out feedPaths, //The path(s) to the source features
        //        out traceStopperJuncs, //Junctions that stopped the trace
        //        out traceStopperEdges); //Edges that stopped the trace

        //    if (feedPaths.Length != 1) return null;

        //    IMMPathElement edge = null;
        //    IMMPathElement junction = null;

        //    var edgeIds = new HashSet<int>();

        //    foreach (IMMFeedPath path in feedPaths)
        //    {
        //        if (path.PathElements.Length < 3)
        //            continue;

        //        // Begin swap-if
        //        var featureA = junction = path.PathElements[1];
        //        var featureB = edge = path.PathElements[2];

        //        if (featureA.ElementType == esriElementType.esriETEdge)
        //        {
        //            edge = featureA;
        //            junction = featureB;
        //        }
        //        // End swap-if

        //        edgeIds.Add(edge.EID);

        //        if (edgeIds.Count > 1) throw new MultipleUpstreamFeaturesException();

        //    }

        //    if (junction == null || edge == null) return null;

        //    var fcContainer = (IFeatureClassContainer)geomNetwork;

        //    var junctionFeature = fcContainer.ClassByID[junction.FCID].GetFeature(junction.OID);
        //    var edgeFeature = fcContainer.ClassByID[edge.FCID].GetFeature(edge.OID);

        //    var traceFacadeEdgeFeature = TraceFacade.GetFirstUpstreamEdges(source).FirstOrDefault();
        //    //var traceFacadeEdgeFeature = TraceFacade.GetFirstUpstreamEdge(source);

        //    PriVoltageValidator validator = new PriVoltageValidator(traceFacadeEdgeFeature, source, junctionFeature);

        //    return validator;
        // }

        ///// <summary>
        ///// Creates the junction primary voltage validator.
        ///// </summary>
        ///// <param name="source">The source.</param>
        ///// <param name="startEID">The start EID.</param>
        ///// <returns></returns>
        // private PriVoltageValidator CreateJunctionPrimaryVoltageValidator(IFeature source, int startEID)
        // {
        //    IFeature upStreamEdge = GetUpstreamEdge(source, startEID);
        //    IFeature downStreamEdge = GetDownstreamEdge(source, startEID);

        //    PriVoltageValidator validator = new PriVoltageValidator(upStreamEdge, downStreamEdge, source); 


        //    return validator;
        //}


        ///// <summary>
        ///// Gets the upstream edge.
        ///// </summary>
        ///// <param name="source">The source.</param>
        ///// <param name="startEID">The start EID.</param>
        ///// <returns></returns>
        // private IFeature GetUpstreamEdge(IFeature source, int startEID) {



        //    //Set up for the trace
        //    IGeometricNetwork geomNetwork = (source as INetworkFeature).GeometricNetwork; //naExt.CurrentNetwork;

        //    int[] barrierJuncs = new int[0];
        //    int[] barrierEdges = new int[0];
        //    IMMFeedPath[] feedPaths;
        //    IMMTraceStopper[] traceStopperJuncs;
        //    IMMTraceStopper[] traceStopperEdges;
        //    //Call the trace
        //    IMMElectricTraceSettings mmElectricTraceSettings = new MMElectricTraceSettingsClass();
        //    IMMElectricTracing mmElectricTracing = new MMFeederTracerClass();
        //    esriElementType typeOfElement = esriElementType.esriETJunction;
        //    if ((source is ISimpleEdgeFeature) || (source is IComplexEdgeFeature))
        //    {
        //        typeOfElement = esriElementType.esriETEdge;
        //    }
        //    mmElectricTracing.FindFeedPaths(
        //        geomNetwork, //The geometric network (derived from the ServicePoint feature)
        //        mmElectricTraceSettings, //Setting regarding whether to respect phasing and Enabled status

        //        this, //The class that implements IMMCurrentStatus

        //        startEID, //The EID of where the trace should start
        //        typeOfElement,  //What kind of EID is startEID
        //        SetOfPhases.abc, //What phases should be traced

        //        barrierJuncs, //What junctions should act as barriers 
        //        barrierEdges, //What edges should act as barriers

        //        out feedPaths, //The path(s) to the source features
        //        out traceStopperJuncs, //Junctions that stopped the trace
        //        out traceStopperEdges); //Edges that stopped the trace

        //    if (feedPaths.Length != 1) return null;

        //    IMMPathElement edge = null;

        //    List<int> edgeIds = new List<int>();

        //    foreach (IMMFeedPath path in feedPaths)
        //    {
        //        edge = path.PathElements[1];

        //        if (!edgeIds.Contains(edge.EID))
        //        {
        //            edgeIds.Add(edge.EID);
        //        }

        //        if (edgeIds.Count > 1) throw new MultipleUpstreamFeaturesException();

        //    }

        //    if (edge == null) return null;

        //    IFeatureClassContainer fcContainer = (IFeatureClassContainer)geomNetwork;

        //    IFeatureClass edgeFC = fcContainer.get_ClassByID(edge.FCID);

        //    return edgeFC.GetFeature(edge.OID);
        // }

        /// <summary>
        /// Gets the downstream edge.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="startEID">The start EID.</param>
        /// <returns></returns>
        private IFeature GetDownstreamEdge(IFeature source, int startEID) {

            //Set up for the trace
            IGeometricNetwork geomNetwork = (source as INetworkFeature).GeometricNetwork; //naExt.CurrentNetwork;

            int[] barrierJuncs = new int[0];
            int[] barrierEdges = new int[0];

            IMMTracedElements tracedJunctions;
            IMMTracedElements tracedEdges;
            //Call the trace
            IMMElectricTraceSettings mmElectricTraceSettings = new MMElectricTraceSettingsClass();
            IMMElectricTracing mmElectricTracing = new MMFeederTracerClass();
            esriElementType typeOfElement = esriElementType.esriETJunction;
            if ((source is ISimpleEdgeFeature) || (source is IComplexEdgeFeature))
            {
                typeOfElement = esriElementType.esriETEdge;
            }
            mmElectricTracing.TraceDownstream(
                geomNetwork, //The geometric network (derived from the ServicePoint feature)
                mmElectricTraceSettings, //Setting regarding whether to respect phasing and Enabled status

                this, //The class that implements IMMCurrentStatus

                startEID, //The EID of where the trace should start
                typeOfElement,  //What kind of EID is startEID
                SetOfPhases.abc, //What phases should be traced
                mmDirectionInfo.establishBySourceSearch,
                0,
                esriElementType.esriETNone,
                barrierJuncs, //What junctions should act as barriers 
                barrierEdges, //What edges should act as barriers

                false,
                out tracedJunctions,
                out tracedEdges);

            IFeatureClassContainer fcContainer = (IFeatureClassContainer)geomNetwork;
            IFeatureClass fc = null;

            tracedEdges.Reset();

            IMMTracedElement edge = null;

            if ((edge = tracedEdges.Next()) != null)
            {
                fc = fcContainer.get_ClassByID(edge.FCID);

                return fc.GetFeature(edge.OID);
            }
            
            return null;
        }

        /// <summary>
        /// Returns the network id (EID) of the given feature
        /// </summary>
        /// <param name="feature"></param>
        /// <returns></returns>
        private int GetFeatureEID(IFeature feature)
        {
            IFeatureClass fc = feature.Class as IFeatureClass;
            INetElements netElements = (INetElements)(feature as INetworkFeature).GeometricNetwork.Network;

            IEnumNetEID eids = netElements.GetEIDs(fc.FeatureClassID, feature.OID, esriElementType.esriETEdge);

            if (eids.Count == 0) return 0;

            return eids.Next();
        }

        /// <summary>
        /// Returns the feature associated to the given eid, for the given feature class
        /// </summary>
        /// <param name="featureClass"></param>
        /// <param name="eid"></param>
        /// <returns></returns>
        private IFeature GetFeatureForEID(IFeatureClass featureClass, int eid)
        {
            int userClassID = 0;
            int userID = 0;
            int userSubID = 0;

            INetElements netElements = (INetElements)naExt.CurrentNetwork.Network;
            netElements.QueryIDs(eid, esriElementType.esriETEdge,
                        out userClassID, out userID, out userSubID);

            return featureClass.GetFeature(userID);
        }

        public void GetCurrentStatusAsWeightValue(int EID, int FCID, int OID, int SUBID, esriElementType ElementType, ref int weightVal)
        {
        }

    }

    /// <summary>
    /// An exception thrown is multiple source features are found when attempting to determine the next upstream feature.
    /// </summary>
    [Serializable]
    public class MultipleUpstreamFeaturesException : Exception
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="MultipleUpstreamFeaturesException"/> class.
        /// </summary>
        public MultipleUpstreamFeaturesException() 
            : this("Multiple upstream features were detected.  Use CreateVoltageValidators method instead.") 
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MultipleUpstreamFeaturesException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public MultipleUpstreamFeaturesException(string message) 
            : base(message) 
        { 
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MultipleUpstreamFeaturesException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="inner">The inner.</param>
        public MultipleUpstreamFeaturesException(string message, Exception inner)
            : base(message, inner) 
        {
        }


    }
}
