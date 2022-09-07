

using ESRI.ArcGIS.Geodatabase;
using Miner.Interop;
using System;
using ESRI.ArcGIS.Geometry;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using PGE.Common.Delivery.Diagnostics;
using System.Reflection;

namespace PGE.Common.Delivery.Framework
{

    /// <summary>
    /// Trace Facade is a set of functionalities that can be used to work with ArcFM Tracing logic. 
    /// Find Upstream edges, Upstream Junctions etc.
    /// </summary>
    public class TraceFacade
    {
        #region Private Properties

        /// <summary>
        /// Logger to log information
        /// </summary>
        private static readonly Log4NetLogger _logger = new Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "");
        
        #endregion

        #region Enums
        /// <summary>
        /// 
        /// </summary>
        public enum TraceType
        {
            /// <summary>
            /// 
            /// </summary>
            Upstream,
            /// <summary>
            /// 
            /// </summary>
            Downstream
        }
        #endregion

        #region Upstream

        /// <summary>
        /// ArcFM Trace upstream.
        /// </summary>
        /// <param name="startEID"></param>
        /// <param name="startElementType"></param>
        /// <param name="geomNetwork"></param>
        /// <param name="tracedJunctions"></param>
        /// <param name="tracedEdges"></param>
        public static void UpStreamTrace(int startEID, esriElementType startElementType, IGeometricNetwork geomNetwork, out IEnumNetEID tracedJunctions, out IEnumNetEID tracedEdges)
        {
            int[] barrierJuncs = new int[0];
            int[] barrierEdges = new int[0];

            // Call the trace.
            IMMElectricTraceSettingsEx mmElectricTraceSettings = new MMElectricTraceSettingsClass();
            IMMElectricNetworkTracing mmElectricTracing = new MMFeederTracerClass();
            IMMNetworkAnalysisExtForFramework networkAnalysisExtForFramework = new MMNetworkAnalysisExtForFrameworkClass();
            try
            {
                networkAnalysisExtForFramework.DrawComplex = true;
                networkAnalysisExtForFramework.SelectionSemantics = mmESRIAnalysisOptions.mmESRIAnalysisOnAllFeatures;

                // Perform trace and get a list of.
                mmElectricTracing.TraceUpstream(
                    // The geometric network (derived from the ServicePoint feature).
                    geomNetwork,
                    networkAnalysisExtForFramework,
                    // Setting regarding whether to respect phasing and Enabled status.
                    mmElectricTraceSettings,
                    // The EID of where the trace should start.
                    startEID,
                    // What kind of EID is startEID.
                    startElementType,
                    // What phases should be traced.
                    mmPhasesToTrace.mmPTT_Any,
                    out tracedJunctions,
                    out tracedEdges);
            }
            catch (Exception ex)
            {
                _logger.Error("Error executing tracing. Message: " + ex.Message + " StackTrace: " + ex.StackTrace);
                throw ex;
            }
            finally
            {
                if (mmElectricTracing != null) { while (Marshal.ReleaseComObject(mmElectricTracing) > 0) ; }
                if (networkAnalysisExtForFramework != null) { while (Marshal.ReleaseComObject(networkAnalysisExtForFramework) > 0) ; }
                if (mmElectricTraceSettings != null) { while (Marshal.ReleaseComObject(mmElectricTraceSettings) > 0) ; }
                GC.Collect();
            }
        }

        /// <summary>
        /// Given a feature returns a list of the first upstream edges in each path, 
        /// to find cases that have mulitple upstream lines (a,b,c) that merge into one abc line.
        /// </summary>
        /// <param name="sourceFeature">An object of type IFeature.</param>
        /// <returns>Returns a List of IFeatures.</returns>
        public static List<IFeature> GetFirstUpstreamEdgePerPath(IFeature sourceFeature)
        {
            List<IFeature> retList = new List<IFeature>();
            IGeometricNetwork geomNetwork = (sourceFeature as INetworkFeature).GeometricNetwork;
            IMMFeedPath[] feedPaths = GetFeedPaths(sourceFeature);
            if (feedPaths.Length == 0) return null;
            IFeature retVal = null;
            int startEID = TraceFacade.GetFeatureEID(sourceFeature);
            esriElementType startElementType = TraceFacade.GetElementType(sourceFeature);
            IEnumNetEID junctions = null;
            IEnumNetEID edges = null;
            UpStreamTrace(startEID, startElementType, geomNetwork, out junctions, out edges);
            List<int> upstreamEdgeEIDs = new List<int>();

            int edgeEID = -1;
            edges.Reset();
            while ((edgeEID = edges.Next()) > 0)
            {
                upstreamEdgeEIDs.Add(edgeEID);

            }

            foreach (IMMFeedPath path in feedPaths)
            {
                bool foundFirstEdgeInPath = false;
                int limit = 4;
                if (path.PathElements.Count() < 4)
                {
                    limit = path.PathElements.Count();
                }

                for (int i = 0; i < limit && !foundFirstEdgeInPath; i++)
                {
                    if (path.PathElements != null)
                    {
                        IMMPathElement pathElement = path.PathElements[i];
                        if (pathElement.ElementType == esriElementType.esriETEdge)
                        {

                            retVal = TraceFacade.GetFeatureFromPathElement(pathElement, geomNetwork);
                            if ((retVal.Class.ObjectClassID + "||" + retVal.OID) != (sourceFeature.Class.ObjectClassID + "||" + sourceFeature.OID))
                            {
                                if (upstreamEdgeEIDs.Contains(pathElement.EID))
                                {
                                    retList.Add(retVal);
                                    foundFirstEdgeInPath = true;
                                }
                            }


                        }

                    }

                }

            }
            return retList;
        }

        /// <summary>
        /// Given a feature that participates in the network will return the First Edge feature upstream on the given Feature.
        /// </summary>
        /// <param name="sourceFeature">Feature for which the first connected upstream edge should be found. Type of object passed should be IFeature</param>
        /// <returns>Return a IFeature object which is a Edge</returns>
        public static IFeature GetFirstUpstreamEdge(IFeature sourceFeature)
        {
            IFeature retVal = GetFirstFeatureOfType(sourceFeature, esriElementType.esriETEdge);
            return retVal;
        }

        /// <summary>
        /// Given a feature that participates in the network will return the First Junction feature upstream on the given Feature.
        /// </summary>
        /// <param name="sourceFeature">Feature for which the first connected upstream Junction should be found. Type of object passed should be IFeature</param>
        /// <returns>Return a IFeature object which is a Edge</returns>
        public static IFeature GetFirstUpstreamJunction(IFeature sourceFeature)
        {
            IFeature retVal = GetFirstFeatureOfType(sourceFeature, esriElementType.esriETJunction);
            return retVal;
        }


        /// <summary>
        /// Given a feature that participates in the network will return first Edge feature upstream on the given Feature.
        /// </summary>
        /// <param name="sourceFeature">Feature for which the first connected upstream edge should be found. Type of object passed should be IFeature</param>
        /// <returns>Return a List of IFeature object which is a Edge</returns>
        public static List<IFeature> GetFirstUpstreamEdges(IFeature sourceFeature)
        {
            return GetFirstConnectedFeatureOfType(sourceFeature, esriElementType.esriETEdge);
        }
        /// <summary>
        /// Given a feature that participates in the network will return first Junction feature upstream on the given Feature.
        /// </summary>
        /// <param name="sourceFeature">Feature for which the first connected upstream edge should be found. Type of object passed should be IFeature</param>
        /// <returns>Return a List of IFeature object which is a Junction</returns>
        public static List<IFeature> GetFirstUpstreamJunctions(IFeature sourceFeature)
        {
            return GetFirstConnectedFeatureOfType(sourceFeature, esriElementType.esriETJunction);
        }

        /// <summary>
        /// Given a feature and ElementType will return the first upstream element with elementtype defined by the returnElementType parameter of the given feature.
        /// </summary>
        /// <param name="sourceFeature">An object of type IFeature</param>
        /// <param name="returnElementType">An object of type esriElementType</param>
        /// <returns>Returns a IFeature and the type of feature is determined by the returnElementType parameter</returns>
        public static IFeature GetFirstFeatureOfType(IFeature sourceFeature, esriElementType returnElementType)
        {
            IGeometricNetwork geomNetwork = (sourceFeature as INetworkFeature).GeometricNetwork;
            IMMFeedPath[] feedPaths = GetFeedPaths(sourceFeature);
            if (feedPaths.Length == 0) return null;
            IFeature retVal = null;
            IFeatureClassContainer fcContainer = (IFeatureClassContainer)geomNetwork;
            foreach (IMMFeedPath path in feedPaths)
            {
                foreach (IMMPathElement pathElement in path.PathElements)
                {
                    if (pathElement.ElementType == returnElementType)
                    {
                        retVal=GetFeatureFromPathElement(pathElement, geomNetwork);
                        if ((retVal.Class.ObjectClassID + "||" + retVal.OID) != (sourceFeature.Class.ObjectClassID + "||" + sourceFeature.OID))
                            return retVal;
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Given a feature and ElementType will return the first upstream element with elementtype defined by the returnElementType parameter of the given feature.
        /// </summary>
        /// <param name="sourceFeature">An object of type IFeature</param>
        /// <param name="returnElementType">An object of type esriElementType</param>
        /// <returns>Returns a IFeature and the type of feature is determined by the returnElementType parameter</returns>
        public static List<IFeature> GetFirstConnectedFeatureOfType(IFeature sourceFeature, esriElementType returnElementType)
        {
            List<IFeature> retVal = new List<IFeature>();
            IGeometricNetwork geomNetwork = (sourceFeature as INetworkFeature).GeometricNetwork;
            IMMFeedPath[] feedPaths = GetFeedPaths(sourceFeature);
            List<int> tracedEIDs = new List<int>();
            foreach (IMMFeedPath path in feedPaths)
            {
                tracedEIDs.AddRange(TracedEIDs(path));
            }
            //int startElementEID = GetFeatureEID(sourceFeature);
            int startElementEID = GetStartEID(sourceFeature, tracedEIDs,TraceType.Upstream);
            if (feedPaths.Length == 0) return null;

            IForwardStarGEN forwardStar = (IForwardStarGEN)geomNetwork.Network.CreateForwardStar(false, null, null, null, null);
            IFeatureClassContainer fcContainer = (IFeatureClassContainer)geomNetwork;
            List<int> visitedElements = new List<int>();
            List<int> eidsAtPosition = new List<int>();
            foreach (IMMFeedPath path in feedPaths)
            {
                List<int> tracedElementEIDs = TracedEIDs(path);
                if (returnElementType == esriElementType.esriETJunction)
                {
                    eidsAtPosition = GetFirstConnectedJunction(startElementEID, ref forwardStar, tracedElementEIDs);
                }
                if (returnElementType == esriElementType.esriETEdge)
                {
                    eidsAtPosition = GetFirstConnectedEdge(startElementEID, ref forwardStar, tracedElementEIDs);
                }
            }
            IFeature returnedFeature = null;
            foreach (int eidAtPosition in eidsAtPosition)
            {
                returnedFeature = GetFeaturefromEID(eidAtPosition, returnElementType, geomNetwork);
                //For edges the source feature is also returned sometime
                if (returnedFeature != null && returnedFeature.OID != sourceFeature.OID)
                {
                    retVal.Add(returnedFeature);
                }
            }
            return retVal;
        }

        /// <summary>
        /// Given a Path Element of type IMMPathElement will fetch the IFeature associated to that PathElement.
        /// </summary>
        /// <param name="pathElement">An object of type IMMPathElement</param>
        /// <param name="geomNetwork">Geometric network that the PathElement is obtained from</param>
        /// <returns>IFeature that the IMMPathElement represents</returns>
        public static IFeature GetFeatureFromPathElement(IMMPathElement pathElement,IGeometricNetwork geomNetwork)
        {

            //IMMPathElement's OID and FCID properties are not populated while using FindFeedPaths so have to query using EID attribtue to get the OID and FCID from database.
            return GetFeaturefromEID(pathElement.EID, pathElement.ElementType,geomNetwork);   
 
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="EID"></param>
        /// <param name="elementType"></param>
        /// <param name="geomNetwork"></param>
        /// <returns></returns>
        public static IFeature GetFeaturefromEID(int EID, esriElementType elementType, IGeometricNetwork geomNetwork)
        {
            INetElements netElements = geomNetwork.Network as INetElements;
            //Objectclass ID
            int userClassID = 0;
            //ObjectID
            int userID = 0;
            int userSubID = 0;

            netElements.QueryIDs(EID, elementType, out userClassID, out userID, out userSubID);

            if (userID <= 0 || userClassID <= 0) return null;
            IFeatureClassContainer fcContainer = (IFeatureClassContainer)geomNetwork;
            IFeatureClass featClass = fcContainer.get_ClassByID(userClassID);

            return featClass.GetFeature(userID);
        }
        /// <summary>
        /// Given a feature will return the upstream feedpaths available for the feature. typically the array length should be 1. If there are more than 1 feed path that indicates a Looped scenario.
        /// </summary>
        /// <param name="sourceFeature">An object of type IFeature from which the upstream path should be determined</param>
        /// <returns>An array of IMMFeedPath. The array length is typically 1. If the length is greater than 1 that indicates a loop scenario</returns>
        public static IMMFeedPath[] GetFeedPaths(IFeature sourceFeature)
        {
            int[] barrierJuncs = new int[0];
            int[] barrierEdges = new int[0];
            IMMFeedPath[] feedPaths;
            IMMTraceStopper[] StopperJuncs;
            IMMTraceStopper[] StopperEdges;
            IMMElectricTraceSettings mmEleTraceSettings = new MMElectricTraceSettingsClass();
            IMMElectricTracing mmEleTracing = new MMFeederTracerClass();

            int startEID = GetFeatureEID(sourceFeature);
            try
            {
                IGeometricNetwork geomNetwork = (sourceFeature as INetworkFeature).GeometricNetwork;
            esriElementType elementType = GetElementType(sourceFeature);

            // Call the trace.
            mmEleTracing.FindFeedPaths(geomNetwork, mmEleTraceSettings, null, startEID, elementType, SetOfPhases.abc,
                                        barrierJuncs, barrierEdges, out feedPaths, out StopperJuncs, out StopperEdges);
            }
            catch (Exception ex)
            {
                _logger.Error("Error executing tracing. Message: " + ex.Message + " StackTrace: " + ex.StackTrace);
                throw ex;
            }
            finally
            {
                if (mmEleTracing != null) { while (Marshal.ReleaseComObject(mmEleTracing) > 0) ; }
                if (mmEleTraceSettings != null) { while (Marshal.ReleaseComObject(mmEleTraceSettings) > 0) ; }
                GC.Collect();
            }
            return feedPaths;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="feedPath"></param>
        /// <returns></returns>
        public static List<int> TracedEIDs(IMMFeedPath feedPath)
        {
            //return feedPath.PathElements.Select(pathElements => pathElements.EID).ToList();
            List<int> retVal = new List<int>();
            foreach (IMMPathElement pathElement in feedPath.PathElements)
            {
                retVal.Add(pathElement.EID);
            }
            return retVal;
        }
        #endregion

        #region Common

        #region public
        /// <summary>
        /// Given a feature gets the EID of the feature if it participates in a network
        /// </summary>
        /// <param name="sourceFeature">The feature of type IFeature to determine the EID</param>
        /// <returns>An Integer. the EID of the Network feature passed in. If the feature passed does not participate in geometric network will return 0</returns>
        public static int GetFeatureEID(IFeature sourceFeature)
        {
            int retVal = 0;
            if (sourceFeature is ISimpleEdgeFeature)
            {
                retVal = (sourceFeature as ISimpleEdgeFeature).EID;
            }
            else if (sourceFeature is ISimpleJunctionFeature)
            {
                ISimpleJunctionFeature junctionFeature = (ISimpleJunctionFeature)sourceFeature;
                if (junctionFeature != null)
                    retVal = junctionFeature.EID;
            }
                //If it is Complex Edge return the EID of the First Edge feature.
            else if (sourceFeature is IComplexEdgeFeature)
            {
                IFeatureClass fc = sourceFeature.Class as IFeatureClass;
                INetElements netElements = (INetElements)(sourceFeature as INetworkFeature).GeometricNetwork.Network;

                IEnumNetEID eids = netElements.GetEIDs(fc.FeatureClassID, sourceFeature.OID, esriElementType.esriETEdge);
                if (eids.Count > 0)
                    retVal = eids.Next();
            }
            return retVal;
        }

        /// <summary>
        /// Given a feature will return the esriElementType. If the feature does not participate in the Geomoetric Network will return esriElementType.esriETNone.
        /// For IPoint feature will return esriElementType.esriETJunction
        /// For Line features will return esriElementType.esriETEdge
        /// </summary>
        /// <param name="sourceFeature">Feature for which the ElementType should be determined.</param>
        /// <returns>Returns the ElementType for the feature passed in</returns>
        public static esriElementType GetElementType(IFeature sourceFeature)
        {
            if (sourceFeature is INetworkFeature)
            {
                if (sourceFeature is IJunctionFeature)
                {
                    return esriElementType.esriETJunction;
                }
                else if (sourceFeature is IEdgeFeature)
                {
                    return esriElementType.esriETEdge;
                }
                else
                {
                    return esriElementType.esriETNone;
                }
            }
            else
            {
                return esriElementType.esriETNone;
            }
        }

        /// <summary>
        /// Give a INetwork,esriElementType and a EID will get the OID of the feature that the combination represent.
        /// Will return 0 if a feature cannot be found.
        /// </summary>
        /// <param name="EID">EID of th Network Element to find the OID</param>
        /// <param name="geomNetwork">An object of type IGeometricNetwork that should be used to obtain the feature's OID from.</param>
        /// <param name="ElementType">esriElementType the EID represents</param>
        /// <returns>The ObjectID of the feature that is associated with the EID in the given network</returns>
        public static int GetOIDFromEID(int EID, IGeometricNetwork geomNetwork, ESRI.ArcGIS.Geodatabase.esriElementType ElementType)
        {
            return GetOIDFromEID(EID, geomNetwork.Network, ElementType);
        }
        /// <summary>
        /// Give a INetwork,esriElementType and a EID will get the OID of the feature that the combination represent.
        /// Will return 0 if a feature cannot be found.
        /// </summary>
        /// <param name="EID">EID of th Network Element to find the OID</param>
        /// <param name="network">An object of type INetwork that should be used to obtain the feature's OID from. IGeometricNetwork.Network or IStreetNetwork will provide the INetwork object</param>
        /// <param name="ElementType">esriElementType the EID represents</param>
        /// <returns>The ObjectID of the feature that is associated with the EID in the given network</returns>
        public static int GetOIDFromEID(int EID, INetwork network, ESRI.ArcGIS.Geodatabase.esriElementType ElementType)
        {
            INetElements netElements = network as INetElements;
            int userClassID = 0;
            int userID = 0;
            int userSubID = 0;
            netElements.QueryIDs(EID, ElementType, out userClassID, out userID, out userSubID);
            return userID;
        }

        #endregion

        #region Private
        private static int GetStartEID(IFeature feature, List<int> tracedEIDs, TraceType traceType)
        {
            esriElementType elementType = GetElementType(feature);
            switch (elementType)
            {
                //If it is Edge feature get the from and to junction and see which one is in the traced elements. 
                // For Downstream send the one that is not in the trace result and for upstream send the one in the trace result.
                //The one not in traced element is not in the direction of trace so send the one found in the traced elements.
                case esriElementType.esriETEdge:
                    IEdgeFeature edgeFeature = (IEdgeFeature)feature;
                    if (traceType == TraceType.Upstream)
                    {
                        if (tracedEIDs.Contains(edgeFeature.FromJunctionEID)) return edgeFeature.FromJunctionEID;
                        if (tracedEIDs.Contains(edgeFeature.ToJunctionEID)) return edgeFeature.ToJunctionEID;
                    }
                    else
                    {
                        if (!tracedEIDs.Contains(edgeFeature.FromJunctionEID)) return edgeFeature.FromJunctionEID;
                        if (!tracedEIDs.Contains(edgeFeature.ToJunctionEID)) return edgeFeature.ToJunctionEID;
                    }
                    return 0;
                case esriElementType.esriETJunction:
                default:
                    return GetFeatureEID(feature);

            }
        }

        private static List<int> GetFirstConnectedEdge(int startJunctionEID, ref IForwardStarGEN forwardStarGen, List<int> tracedEIDs)
        {
            List<int> retVal = new List<int>();
            // get connected edges
            int edgeCount;
            forwardStarGen.FindAdjacent(0, startJunctionEID, out edgeCount);

            //get adjacentEdges
            int[] adjacentEdgeEIDs = new int[edgeCount];
            bool[] reverseOrientationEdge = new bool[edgeCount];
            object[] weightValuesJunctions = new object[edgeCount];
            //for (int edgeIndex = 0; edgeIndex <= edgeCount; edgeIndex++)
            //{
            forwardStarGen.QueryAdjacentEdges(ref adjacentEdgeEIDs, ref reverseOrientationEdge, ref weightValuesJunctions);
            if (adjacentEdgeEIDs.Length == 0) return retVal;
            foreach (int adjacentEdgeEID in adjacentEdgeEIDs)
            {
                //Do not process if the Junction EID was not returned by ArcFM Trace
                if (!tracedEIDs.Contains(adjacentEdgeEID)) continue;
                retVal.Add(adjacentEdgeEID);
            }
            //}
            return retVal;
        }

        private static List<int> GetFirstConnectedJunction(int startJunctionEID, ref IForwardStarGEN forwardStarGen, List<int> tracedEIDs)
        {
            List<int> retVal = new List<int>();
            // get connected edges
            int edgeCount;
            forwardStarGen.FindAdjacent(0, startJunctionEID, out edgeCount);

            //get adjacentJunctions
            int[] adjacentJunctionEIDs = new int[edgeCount];
            object[] weightValuesJunctions = new object[edgeCount];
            forwardStarGen.QueryAdjacentJunctions(ref adjacentJunctionEIDs, ref weightValuesJunctions);
            if (adjacentJunctionEIDs.Length == 0) return retVal;
            foreach (int adjacentJunctionEID in adjacentJunctionEIDs)
            {
                //Do not process if the Junction EID was not returned by ArcFM Trace
                if (!tracedEIDs.Contains(adjacentJunctionEID)) continue;
                retVal.Add(adjacentJunctionEID);
            }
            return retVal;
        }
        #endregion

        #endregion

        #region DownStream
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sourceFeature"></param>
        /// <param name="ignoreModelName"></param>
        /// <returns></returns>
        public static List<IFeature> GetFirstDownstreamEdges(IFeature sourceFeature, string ignoreModelName)
        {
            return GetFirstDownstreamFeatureOfType(sourceFeature, esriElementType.esriETEdge, ignoreModelName);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sourceFeature"></param>
        /// <returns></returns>
        public static List<IFeature> GetFirstDownstreamEdges(IFeature sourceFeature)
        {
            return GetFirstDownstreamFeatureOfType(sourceFeature, esriElementType.esriETEdge);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sourceFeature"></param>
        /// <param name="ignoreModelName"></param>
        /// <returns></returns>
        public static List<IFeature> GetFirstDownstreamJunctions(IFeature sourceFeature, string ignoreModelName)
        {
            return GetFirstDownstreamFeatureOfType(sourceFeature, esriElementType.esriETJunction, ignoreModelName);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sourceFeature"></param>
        /// <returns></returns>
        public static List<IFeature> GetFirstDownstreamJunctions(IFeature sourceFeature)
        {
            return GetFirstDownstreamFeatureOfType(sourceFeature, esriElementType.esriETJunction);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sourceFeature"></param>
        /// <param name="returnElementType"></param>
        /// <param name="ignoreModelName"></param>
        /// <returns></returns>
        public static List<IFeature> GetFirstDownstreamFeatureOfType(IFeature sourceFeature, esriElementType returnElementType, string ignoreModelName = null)
        {
            List<IFeature> retVal = new List<IFeature>();
            IMMTracedElements tracedJunctions;
            IMMTracedElements tracedEdges;
            int startEID = GetFeatureEID(sourceFeature);

            //Set hte Geometric Network
            IGeometricNetwork geomNetwork = ((INetworkFeature)sourceFeature).GeometricNetwork;

            //Perform the Trace and get Downstream features
            DownStreamTrace(startEID, GetElementType(sourceFeature), geomNetwork, out tracedJunctions, out tracedEdges);
            List<int> tracedEIDs = TracedEIDs(tracedJunctions, tracedEdges);
            //Create ForwardStar
            IForwardStarGEN forwardStar = (IForwardStarGEN)geomNetwork.Network.CreateForwardStar(false, null, null, null, null);
            IFeatureClassContainer fcContainer = (IFeatureClassContainer)geomNetwork;
            List<int> visitedElements = new List<int>();
            List<int> eidsAtPosition = new List<int>();
            int startElementEID = GetStartEID(sourceFeature, tracedEIDs, TraceType.Downstream);
            if (returnElementType == esriElementType.esriETJunction)
            {
                eidsAtPosition = GetFirstConnectedJunction(startElementEID, ref forwardStar, tracedEIDs);
            }
            if (returnElementType == esriElementType.esriETEdge)
            {
                eidsAtPosition = GetFirstConnectedEdge(startElementEID, ref forwardStar, tracedEIDs);
            }
            IFeature returnedFeature = null;

            foreach (int eidAtPosition in eidsAtPosition)
            {
                returnedFeature = GetFeaturefromEID(eidAtPosition, returnElementType, geomNetwork);
                //Added by sanjeev 05 April 2013
                bool isIgnoreModelNameAvailable = (ignoreModelName != null);
                if (isIgnoreModelNameAvailable) isIgnoreModelNameAvailable = ModelNameFacade.ContainsClassModelName(returnedFeature.Class, ignoreModelName);
                //For edges the source feature is also returned sometime
                if (returnedFeature != null && returnedFeature.OID != sourceFeature.OID && isIgnoreModelNameAvailable == false) // && isIgnoreModelNameAvailable == false added by sanjeev 05 April 2013
                {
                    retVal.Add(returnedFeature);
                }
            }
            return retVal;
        }
        /// <summary>
        /// ArcFM Trace down stream.
        /// </summary>
        /// <param name="startEID"></param>
        /// <param name="startElementType"></param>
        /// <param name="geomNetwork"></param>
        /// <param name="tracedJunctions"></param>
        /// <param name="tracedEdges"></param>
        public static void DownStreamTrace(int startEID, esriElementType startElementType, IGeometricNetwork geomNetwork, out IMMTracedElements tracedJunctions, out IMMTracedElements tracedEdges)
        {

            int[] barrierJuncs = new int[0];
            int[] barrierEdges = new int[0];

            // Call the trace.
            IMMElectricTraceSettings mmElectricTraceSettings = new MMElectricTraceSettingsClass();
            IMMElectricTracing mmElectricTracing = new MMFeederTracerClass();

            try
            {
                // Perform trace and get a list of .
                mmElectricTracing.TraceDownstream(
                    // The geometric network (derived from the ServicePoint feature).
                    geomNetwork,
                    // Setting regarding whether to respect phasing and Enabled status.
                    mmElectricTraceSettings,
                    // The class that implements IMMCurrentStatus.
                    null,
                    // The EID of where the trace should start.
                    startEID,
                    // What kind of EID is startEID.
                    startElementType,
                    // What phases should be traced.
                    SetOfPhases.abc,
                    mmDirectionInfo.establishBySourceSearch,
                    0,
                    esriElementType.esriETNone,
                    // What junctions should act as barriers.
                    barrierJuncs,
                    // What edges should act as barriers.
                    barrierEdges,

                    false,
                    out tracedJunctions,
                    out tracedEdges);
            }
            catch (Exception ex)
            {
                _logger.Error("Error executing tracing. Message: " + ex.Message + " StackTrace: " + ex.StackTrace);
                throw ex;
            }
            finally
            {
                if (mmElectricTracing != null) { while (Marshal.ReleaseComObject(mmElectricTracing) > 0) ; }
                if (mmElectricTraceSettings != null) { while (Marshal.ReleaseComObject(mmElectricTraceSettings) > 0) ; }
                GC.Collect();
            }


        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="tracedJunctions"></param>
        /// <param name="tracedEdges"></param>
        /// <returns></returns>
        public static List<int> TracedEIDs(IMMTracedElements tracedJunctions, IMMTracedElements tracedEdges)
        {
            List<int> retVal = new List<int>();
            IMMTracedElement tracedElement = null;
            tracedEdges.Reset();
            while ((tracedElement = tracedEdges.Next()) != null)
            {
                retVal.Add(tracedElement.EID);
            }
            tracedJunctions.Reset();
            while ((tracedElement = tracedJunctions.Next()) != null)
            {
                retVal.Add(tracedElement.EID);
            }
            return retVal;
        }
        #endregion

        /// <summary>
        /// Checks that a specified bit position is "on" or "off".
        /// </summary>
        /// <param name="weight">Any integer value - FeederInfo or MMElectricTraceWeight</param>
        /// <param name="position">Bit position to check.</param>
        /// <returns></returns>
        public static bool IsBitSet(int weight, byte position)
        {
            return (weight >> position & 1) == 1;
        }
        /// <summary>
        /// Sets a specific bit in an integer.
        /// </summary>
        /// <param name="bitToSet">Bit position to set.</param>
        /// <param name="weight">Any integer value - FeederInfo or MMElectricTraceWeight</param>
        public static void SetBit(int bitToSet, ref int weight)
        {
            int x = (int)Math.Pow(2, bitToSet);
            weight = weight | x;
        }
        /// <summary>
        /// Unsets a specific bit in an integer.
        /// </summary>
        /// <param name="bitToUnset">Bit position to unset.</param>
        /// <param name="weight">Any integer value - FeederInfo or MMElectricTraceWeight</param>
        public static void UnSetBit(int bitToUnset, ref int weight)
        {
            int x = (int)Math.Pow(2, bitToUnset);
            weight = weight | x;
            weight = weight - x;
        }
    }
}