using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Linq;
using System.Text;
using log4net;
using System.Reflection;
using Miner.Interop;
using Miner.Geodatabase;
using PGE.Common.Delivery.ArcFM;
using PGE.Common.Delivery.Diagnostics;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;

namespace PGE.Desktop.EDER.AutoUpdaters.Special
{
    public partial class SourceSideDevice : IMMCurrentStatus
    {
        //The Dictionary holds the collection of junction features that needs to be updated with SSD ID 
        //The int stores the FeatureClassID and the string stores a "|" delimited set of OIDs for the FCID feature class
        private Dictionary<int, string> m_FeatColl = new Dictionary<int, string>();
        private string[] OIDSeparator = new string[] { "|" };
        private int m_StartEID = -1;
        private int m_ProtectiveDeviceBit = (int)Math.Pow(2, 12);
        private IGeometricNetwork m_GeoNet = null;

        private enum TraceDirection { UpStream, DownStream, Unknown };
        private TraceDirection m_CurrentTraceDir = TraceDirection.Unknown;

        //private IFeatureClass m_SwitchFeatureClass = null;
        //private int m_SwitchTypeFldIndex = -1;


        #region Public Members

        /// <summary>
        /// Finds the first Source Side Device through an upstream trace
        /// Assumptions: the start element is a SourceSide Device and is a junction, tracing is not constrained by phases, and there no barriers
        /// </summary>
        /// <param name="geoNet">The Geometric Network that the StartElementID comes from</param>
        /// <param name="StartElementID">Element ID to Start Trace</param>
        /// <returns></returns>
        public IFeature UpstreamSourceSideDevice(IGeometricNetwork geoNet, int StartElementID)
        {
            IFeature SSDFeature = null;
            try
            {
                m_FeatColl.Clear();
                m_GeoNet = geoNet;
                m_StartEID = StartElementID;
                m_CurrentTraceDir = TraceDirection.UpStream;
                //do upstream
                DoUpStreamTrace();
                //get all feature class in list
                List<IFeature> SSDFeatLst = GetFeaturesFromClasses();
                _logger.Debug("ListCount :" + SSDFeatLst.Count);
                if (SSDFeatLst.Count > 0) SSDFeature = SSDFeatLst[0];
            }
            catch (Exception Ex)
            {
                _logger.Debug("Error: ", Ex);
            }
            finally
            {
                m_CurrentTraceDir = TraceDirection.Unknown;
            }
            return SSDFeature;
        }

        /// <summary>
        /// Finds all the devices between a SourceSide Device and the next downstream SourceSide Device.
        /// Assumptions : the start element is a SourceSide Device and is a junction, 
        /// tracing is not constrained by phases, and there no barriers.
        /// </summary>
        /// <param name="geoNet">The Geometric Network that the StartElementID comes from</param>
        /// <param name="StartElementID">Element ID to Start Trace</param>
        /// <returns></returns>
        public List<IFeature> DownstreamSourceSideDeviceZone(IObject sourceFeature, int startEID, IGeometricNetwork geoNet)
        {
            List<IFeature> InSSDZone = new List<IFeature>();
            List<IFeature> tempZone = new List<IFeature>();
            try
            {
                m_FeatColl.Clear();
                m_GeoNet = geoNet;
                m_StartEID = startEID;
                m_CurrentTraceDir = TraceDirection.DownStream;
                //do downstream
                DoDownStreamTrace();
                //get list of feature class
                tempZone = GetFeaturesFromClasses();
                _logger.Debug("ListCount :" + InSSDZone.Count);

                IFeature firstUpstreamJunc = null;// PGE.Common.Delivery.Framework.TraceFacade.GetFirstUpstreamJunction(sourceFeature as IFeature);
                string firstJuncKey = "1-";
                if (firstUpstreamJunc != null)
                {
                    firstJuncKey = firstUpstreamJunc.Class.ObjectClassID.ToString() + "-" + firstUpstreamJunc.OID.ToString();
                }

                foreach (IFeature zoneFeature in tempZone)
                {
                    string checkKey = zoneFeature.Class.ObjectClassID.ToString() + "-" + zoneFeature.OID.ToString();

                    //check to see if the feature is energized
                    FieldInstance feederID = zoneFeature.FieldInstanceFromModelName(SchemaInfo.Electric.FieldModelNames.FeederID);
                    if (feederID.Value != DBNull.Value)
                    {
                        if (checkKey != firstJuncKey)
                        {
                            InSSDZone.Add(zoneFeature);
                        }
                    }
                }

            }
            catch (Exception Ex)
            {
                _logger.Debug("Error: ", Ex);
            }
            finally
            {
                m_CurrentTraceDir = TraceDirection.Unknown;
            }

            //The first upstream junction is included in the trace.  Remove it.


            return InSSDZone;
        }

        #endregion

        #region IMMCurrentStatus Implementation
        /// <summary>
        /// Get current status of a feature 
        /// </summary>
        /// <param name="EID">Feature EID</param>
        /// <param name="FCID">Feature Class</param>
        /// <param name="OID">Object ID</param>
        /// <param name="SUBID">?</param>
        /// <param name="ElementType">esri feature type</param>
        /// <param name="weightVal">trace weight vlaue</param>
        public void GetCurrentStatusAsWeightValue(int EID, int FCID, int OID, int SUBID, esriElementType ElementType, ref int weightVal)
        {
            _logger.Debug("EID: " + EID + " FCID: " + FCID + " OID: " + OID + " SUBID: " + SUBID + " Trace weight: " + weightVal);
            if (EID == m_StartEID) return;
            //Get the FCIS, OID from EID
            INetElements netElements = m_GeoNet.Network as INetElements;
            netElements.QueryIDs(EID, ElementType, out FCID, out OID, out SUBID);
            //if ((m_CurrentTraceDir == TraceDirection.UpStream) & (m_FeatColl.Count > 0)) return;

            if (ElementType == esriElementType.esriETEdge)
            {
                //Stop the trace if edge assigned with PGE_TRACEEDGEBARRIER model name fond
                if (_barrierEdgeClassIDs.Contains(FCID))
                {
                    //Set the 12 bitgate field (represents FdrMgrNonTraceble) to 1. Hence this edge behaves as de-energized
                    SetTraceStopBit(12, ref weightVal);
                }
            }

            if (m_CurrentTraceDir == TraceDirection.DownStream)
            {
                if (_upstreamEdges.Contains(EID))
                {
                    //Set the 12 bitgate field (represents FdrMgrNonTraceble) to 1. Hence this edge behaves as de-energized
                    SetTraceStopBit(12, ref weightVal);
                    return;
                }
                else if (_upstreamJuncs.Contains(EID))
                {
                    StopTrace(ref weightVal); return;
                }
                if (_featureClassIDs.ContainsKey(FCID)) //if true
                {
                    Add2UpdateColl(FCID, OID);
                }
                if (IsJnASSD(ref FCID, ref OID, EID, weightVal, ElementType))//if true
                {
                    StopTrace(ref weightVal);
                }
            }
        }
        #endregion

        #region Private Members
        /// <summary>
        /// Returns the network id (EID) of the given feature
        /// </summary>
        /// <param name="feature">esri feature</param>
        /// <returns>esri element type</returns>
        private int GetFeatureEID(IFeature feature, esriElementType elementType)
        {
            IFeatureClass fc = feature.Class as IFeatureClass;
            INetElements netElements = (INetElements)(feature as INetworkFeature).GeometricNetwork.Network;
            //get EID of a feature
            IEnumNetEID eids = netElements.GetEIDs(fc.FeatureClassID, feature.OID, elementType);
            //if true
            if (eids.Count == 0) return 0;
            _logger.Debug("EID :" + eids.Count);
            return eids.Next();
        }


        /// <summary>
        /// Performs a downstream trace.
        /// </summary>
        private void DoDownStreamTrace()
        {
            int[] barrierJuncs = _upstreamJuncs.ToArray();
            int[] barrierEdges = _upstreamEdges.ToArray();

            IMMTracedElements tracedJunctions;
            IMMTracedElements tracedEdges;
            IMMElectricTraceSettings mmEleTraceSettings = new MMElectricTraceSettingsClass();
            IMMElectricTracingEx mmEleTracing = new MMFeederTracerClass();
            //electric down stream trace
            mmEleTracing.TraceDownstream(m_GeoNet, mmEleTraceSettings, this,
                                           m_StartEID, esriElementType.esriETJunction, SetOfPhases.abc, mmDirectionInfo.dontCare, 0,
                                           esriElementType.esriETNone, barrierJuncs, barrierEdges, false, out tracedJunctions, out tracedEdges);

        }


        /// <summary>
        /// Performs a Upstream trace.
        /// Depends on class level IGeometricNetwork and StartEID
        /// </summary>
        private void DoUpStreamTrace()
        {
            int[] barrierJuncs = new int[0];
            int[] barrierEdges = new int[0];
            IMMFeedPath[] feedPaths;
            IMMTraceStopper[] StopperJuncs;
            IMMTraceStopper[] StopperEdges;

            IMMElectricTraceSettings mmEleTraceSettings = new MMElectricTraceSettingsClass();
            IMMElectricTracing mmEleTracing = new MMFeederTracerClass();

            //Call the trace
            mmEleTracing.FindFeedPaths(m_GeoNet, mmEleTraceSettings, this, m_StartEID, esriElementType.esriETJunction, SetOfPhases.abc,
                                        barrierJuncs, barrierEdges, out feedPaths, out StopperJuncs, out StopperEdges);

            // go over the feedPaths looking for the first device of interest
            if (feedPaths.Length == 0) return;

            IMMPathElement junction = null;
            IMMPathElement nearestSSD = null;
            int hops2ClosestSSD = int.MaxValue;
            int FCID = 0;
            int objectID = 0;
            foreach (IMMFeedPath path in feedPaths)
            {
                for (int hop = 0; hop < path.PathElements.Length; hop++)
                {
                    junction = path.PathElements[hop];
                    if (junction.EID == m_StartEID)
                        continue;
                    if (junction.ElementType == esriElementType.esriETJunction)
                    {
                        _upstreamJuncs.Add(junction.EID);
                        if (IsJnASSD(ref FCID, ref objectID, junction.EID, junction.Weight, esriElementType.esriETJunction))
                        {
                            if (hop <= hops2ClosestSSD)
                            {
                                nearestSSD = junction;
                                hops2ClosestSSD = hop;
                            }
                            break;
                        }
                    }
                    else if (junction.ElementType == esriElementType.esriETEdge)
                    {
                        _upstreamEdges.Add(junction.EID);
                    }
                }
            }

            if (nearestSSD != null)
            {
                if (_featureClassIDs.ContainsKey(nearestSSD.FCID))
                    Add2UpdateColl(nearestSSD.FCID, nearestSSD.OID);
            }
        }
        /// <summary>
        /// Adds the feature class ID and OID of the feature to the collection
        /// of features that require SSD updating
        /// </summary>
        /// <param name="FCID"></param>
        /// <param name="OID"></param>
        private void Add2UpdateColl(int FCID, int OID)
        {
            try
            {
                string CurrValue = string.Empty;
                if (m_FeatColl.TryGetValue(FCID, out CurrValue))
                {
                    m_FeatColl[FCID] = CurrValue + OIDSeparator[0] + OID.ToString();
                }
                else
                {
                    m_FeatColl.Add(FCID, OID.ToString());
                }
            }
            catch (Exception Ex)
            {
                _logger.Debug(Ex.Message);
                throw new Exception(Ex.Message, Ex);
            }
        }
        /// <summary>
        /// Stop trace by setting the bits 28,29,30 in trace weight
        /// </summary>
        /// <param name="TraceWeight"></param>
        private void StopTrace(ref int TraceWeight)
        {
            SetTraceStopBit(12, ref TraceWeight);
            SetTraceStopBit(28, ref TraceWeight);
            SetTraceStopBit(29, ref TraceWeight);
            SetTraceStopBit(30, ref TraceWeight);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="BitNumber">bitnumber integer</param>
        /// <param name="WeightValue">trace weight value</param>
        private void SetTraceStopBit(int BitNumber, ref int WeightValue)
        {
            int x = (int)Math.Pow(2, BitNumber);
            WeightValue = WeightValue | x;
        }

        /// <summary>
        /// Assign userID and userClassID
        /// </summary>
        /// <param name="elementFCID">UserClassID</param>
        /// <param name="elementOID">UserID</param>
        /// <param name="elementEID">Network Element ID</param>
        private void AssignElementInfo(ref int elementFCID, ref int elementOID, int elementEID)
        {
            INetElements netElements = m_GeoNet.Network as INetElements;
            int userClassID = 0;
            int userID = 0;
            int userSubID = 0;
            netElements.QueryIDs(elementEID, esriElementType.esriETJunction, out userClassID, out userID, out userSubID);
            elementOID = userID;
            elementFCID = userClassID;
        }

        /// <summary>
        /// Returns the set of features that requires updating for SSD ID
        /// Uses class level Dictionary m_UpdateFeatColl
        /// </summary>
        /// <returns></returns>
        private List<IFeature> GetFeaturesFromClasses()
        {
            List<IFeature> UpdateFeatLst = new List<IFeature>();
            try
            {
                IGeoDatabaseBridge pGDBBridge = new GeoDatabaseHelperClass();
                //Get the features from each feature class from their OIDs
                foreach (KeyValuePair<int, string> kvp in m_FeatColl)
                {
                    string SetOfOIDs = kvp.Value;
                    string[] OIDArray = SetOfOIDs.Split(OIDSeparator, StringSplitOptions.RemoveEmptyEntries);
                    int[] intOIDs = Array.ConvertAll(OIDArray, int.Parse);
                    IFeature Feat2Add;
                    if (m_CurrentTraceDir == TraceDirection.DownStream)
                    {
                        IFeatureCursor pFeatCur = pGDBBridge.GetFeatures(_featureClassIDs[kvp.Key], ref intOIDs, false);
                        while ((Feat2Add = pFeatCur.NextFeature()) != null)
                        {
                            UpdateFeatLst.Add(Feat2Add);
                        }

                        while (Marshal.ReleaseComObject(pFeatCur) > 0) { }
                        pFeatCur = null;
                    }
                    else if (m_CurrentTraceDir == TraceDirection.UpStream)
                    {
                        IFeatureClass pSSDFC = FCFromFCID(kvp.Key);
                        UpdateFeatLst.Add(pSSDFC.GetFeature(intOIDs[0]));
                    }
                }
            }
            catch (Exception Ex)
            {
                _logger.Debug(Ex.Message);
                //throw new Exception(Ex.Message, Ex);
            }
            return UpdateFeatLst;
        }

        /// <summary>
        /// Checks if a junction element is a SSD from Element ID
        /// If the FCID and OID is not set by the trace these values are set.
        /// </summary>
        /// <param name="FCId">Feature class ID of the feature that element represents</param>
        /// <param name="ObjID">Object ID of the feature that element represents</param>
        /// <param name="EleID">Element ID of the Junction element</param>
        /// <param name="WtValue">Trace weight of the element</param>
        /// <param name="EleType">Type netwrok element. Should be esriETJunction</param>
        /// <returns></returns>
        private bool IsJnASSD(ref int FCID, ref int objectID, int elementID, int networkWeightValue, esriElementType elementType)
        {
            bool IsSSD = false;
            if (elementType != esriElementType.esriETJunction) return IsSSD;        //Not a Junction return false

            if ((networkWeightValue & m_ProtectiveDeviceBit) > 0)              //Check if the Junc. is one of Fuse, DPD or Switch
            {
                if (m_CurrentTraceDir == TraceDirection.UpStream)
                {
                    AssignElementInfo(ref FCID, ref objectID, elementID); //Upstream trace results in zeros for FCID and OID, so set it by 'ref'
                }

                if (_SSDFeatureClassIDs.Contains(FCID) == false)
                {
                    return IsSSD;  //Device is does not have the SourceSideDevice model name, return false.
                }
                else
                {
                    if (FCID != m_SwitchFeatureClass.FeatureClassID)
                    {
                        IsSSD = true;   //Not a Switch, could be a Fuse or DPD. All Fuses and DPDs are SSD, so return true
                    }
                    else
                    {
                        if ((m_SwitchFeatureClass != null) && (m_SwitchTypeFldIndex != -1))
                        {
                            IFeature SwitchFeature = m_SwitchFeatureClass.GetFeature(objectID);
                            string SwitchTypeValue = SwitchFeature.get_Value(m_SwitchTypeFldIndex).ToString();
                            //SwitchTypes 7,8,99 are not SSDs.
                            if ((SwitchTypeValue == "7") | (SwitchTypeValue == "8") | (SwitchTypeValue == "99"))    //Replace hard-code for check-values?
                            {
                                IsSSD = false;
                            }
                            else
                            {
                                IsSSD = true;
                            }
                        }
                    }
                }
            }
            else
            {
                IsSSD = false;          //Junc is not a Fuse, DPD or Switch, so not a SSD hence return false;
            }
            return IsSSD;
        }

        /// <summary>
        /// Returns the feature class given the Feature class ID of a junction feature
        /// </summary>
        /// <param name="FCId">feature class id</param>
        /// <returns></returns>
        private IFeatureClass FCFromFCID(int FCId)
        {
            IFeatureClass pFC = null;
            try
            {
                IEnumFeatureClass pENumJnFCs = m_GeoNet.get_ClassesByType(esriFeatureType.esriFTSimpleJunction);
                pENumJnFCs.Reset();
                pFC = pENumJnFCs.Next();
                while (pFC != null)//if feature class not null
                {
                    if (pFC.FeatureClassID == FCId) break;
                    pFC = pENumJnFCs.Next();
                }
            }
            catch (Exception Ex)
            {
                _logger.Debug(Ex.Message);
                throw new Exception(Ex.Message, Ex);
            }
            return pFC;
        }

        #endregion
    }
}
