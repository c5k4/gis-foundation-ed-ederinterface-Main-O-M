using ESRI.ArcGIS.Geodatabase;
using Miner.Interop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using PGE.Common.Delivery.ArcFM;
using PGE.Common.Delivery.Framework;

namespace PGE.Desktop.EDER
{
    public static class TracingHelper
    {
        #region Private Variables.
        /// <summary>
        /// Logger to log all the information, warning and errors.
        /// </summary>
        /// 
        private static readonly Log4NetLogger _logger = new Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "EDERDesktop.log4net.config");

        #endregion

        /// <summary>
        /// Trace down stream.
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
        /// Trace upstream.
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
        /// Given a feature returns a list of the first upstream edges in each path, to find cases that have mulitple upstream lines (a,b,c) that merge into one abc line .
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
        /// Given a feature will return the upstream feedpaths available for the feature. typically the array length should be 1. If there are more than 1 feed path that indicates a Looped scenario.
        /// </summary>
        /// <param name="sourceFeature">An object of type IFeature from which the upstream path should be determined.</param>
        /// <returns>An array of IMMFeedPath. The array length is typically 1. If the length is greater than 1 that indicates a loop scenario.</returns>
        public static IMMFeedPath[] GetFeedPaths(IFeature sourceFeature)
        {
            int[] barrierJuncs = new int[0];
            int[] barrierEdges = new int[0];
            IMMFeedPath[] feedPaths;
            IMMTraceStopper[] StopperJuncs;
            IMMTraceStopper[] StopperEdges;
            IMMElectricTraceSettings mmEleTraceSettings = new MMElectricTraceSettingsClass();
            IMMElectricTracing mmEleTracing = new MMFeederTracerClass();

            int startEID = TraceFacade.GetFeatureEID(sourceFeature);
            try
            {
                IGeometricNetwork geomNetwork = (sourceFeature as INetworkFeature).GeometricNetwork;

                esriElementType elementType = TraceFacade.GetElementType(sourceFeature);

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

    }
}
