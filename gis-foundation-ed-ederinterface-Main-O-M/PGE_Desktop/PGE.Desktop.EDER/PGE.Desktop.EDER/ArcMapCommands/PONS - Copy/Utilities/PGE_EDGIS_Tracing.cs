﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Miner.Interop;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.EditorExt;
using ESRI.ArcGIS.NetworkAnalysis;
using System.Data;
using Telvent.Delivery.Diagnostics;
using System.Reflection;


namespace Telvent.PGE.ED.Desktop.ArcMapCommands.PONS.Utilities
{
    class PGE_EDGIS_Tracing
    {
        private static readonly Log4NetLogger _logger = new Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "EDERDesktop.log4net.config");
        public string RunTrace(IFeature startFeature, IFeature endFeature, bool ExcludeCustomers, string versionname)
        {
            string ServicePointIDs = string.Empty;
            try
            {
                UtilityFunctions objUtilityFunctions = new UtilityFunctions();
                string[] PointFeatureClasses = default(string[]),
                    LineFeatureClasses = default(string[]);

                PointFeatureClasses = objUtilityFunctions.ReadConfigurationValue("SearchBwDevicePointClasses").Split(';');
                LineFeatureClasses = objUtilityFunctions.ReadConfigurationValue("SearchBwDevicesLinearClasses").Split(';');

                esriElementType StartDeviceElementType = esriElementType.esriETNone;
                IDataset pDataSet = startFeature.Class as IDataset;
                if (pDataSet != null)
                    if (PointFeatureClasses.Contains(pDataSet.Name))
                        StartDeviceElementType = esriElementType.esriETJunction;
                    else if (LineFeatureClasses.Contains(pDataSet.Name))
                        StartDeviceElementType = esriElementType.esriETEdge;
                

                IUtilityNetworkAnalysisExt m_utilNetExt = objUtilityFunctions.GetArcMapExtensionByCLSID("esriEditorExt.UtilityNetworkAnalysisExt") as IUtilityNetworkAnalysisExt;
                INetworkAnalysisExt nax = default( INetworkAnalysisExt);
                IGeometricNetwork geomNet = default(IGeometricNetwork);

                if (PGEGlobal.WORKSPACE_MAP != null)
                {
                    // get the current network from the Utility Network Analysis extension
                    nax = m_utilNetExt as INetworkAnalysisExt;
                    geomNet = nax.CurrentNetwork;
                }
                else if(PGEGlobal.WORKSPACE_EDER != null)
                { 
                    IFeatureWorkspace pFeatWS = PGEGlobal.WORKSPACE_EDER as IFeatureWorkspace;

                    INetworkCollection pNWCollection = pFeatWS.OpenFeatureDataset(objUtilityFunctions.ReadConfigurationValue("ElectricDataset")) as INetworkCollection;

                    geomNet = pNWCollection.get_GeometricNetworkByName(objUtilityFunctions.ReadConfigurationValue("ElectricDistNetwork"));
                }

                //objNetClass = objFeatClass as INetworkClass;
                //objGeoNet = objNetClass.GeometricNetwork;
                INetElements objNetElems = geomNet.Network as INetElements;
                IDictionary<int, IList<int>> BarrierIds = default(IDictionary<int, IList<int>>);
                IDictionary<int, esriElementType> BarrierTypes = default(IDictionary<int, esriElementType>);
                IEnumNetEID edgeEIDs, juncEIDs = null;

                if (endFeature != null)
                {
                    BarrierIds = new Dictionary<int, IList<int>>();
                    BarrierTypes = new Dictionary<int, esriElementType>();

                    esriElementType EndDeviceElementType = esriElementType.esriETNone;
                    pDataSet = default(IDataset);
                    pDataSet = endFeature.Class as IDataset;
                    if (pDataSet != null)
                        if (PointFeatureClasses.Contains(pDataSet.Name))
                            EndDeviceElementType = esriElementType.esriETJunction;
                        else if (LineFeatureClasses.Contains(pDataSet.Name))
                            EndDeviceElementType = esriElementType.esriETEdge;

                    IFeatureClass pEndFeatureClass = endFeature.Class as IFeatureClass;
                    int objEndEID = objNetElems.GetEID(pEndFeatureClass.FeatureClassID, endFeature.OID, -1, EndDeviceElementType);
                    IList<int> BarrierOID = new List<int>();
                    BarrierOID.Add(endFeature.OID);
                    BarrierIds.Add(pEndFeatureClass.FeatureClassID, BarrierOID);

                    BarrierTypes.Add(pEndFeatureClass.FeatureClassID, EndDeviceElementType);
                }

                int objEID = objNetElems.GetEID((startFeature.Class as IFeatureClass).FeatureClassID, startFeature.OID, -1, StartDeviceElementType);
                ServicePointIDs = DownstreamTrace(geomNet, objEID, StartDeviceElementType, BarrierIds, BarrierTypes, ExcludeCustomers, versionname, out juncEIDs, out edgeEIDs);
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
            }

            return ServicePointIDs;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="inGeomNetwork"></param>
        /// <param name="startElementEID"></param>
        /// <param name="elementType"></param>
        /// <param name="BarrierIds">Dictionary with ClassID key and List of feature EIDs</param>
        /// <param name="BarrierTypes">Dictionary with ClassID and esriElementType</param>
        /// <param name="juncEIDs"></param>
        /// <param name="edgeEIDs"></param>
        public string DownstreamTrace(IGeometricNetwork inGeomNetwork, int startElementEID, esriElementType elementType,
            IDictionary<int, IList<int>> BarrierIds, IDictionary<int, esriElementType> BarrierTypes, bool ExcludeCustomers, string versionname, out IEnumNetEID juncEIDs,
            out IEnumNetEID edgeEIDs)
        {

            /*
             * UID uidUtilNet = new UIDClass();
            uidUtilNet.Value = "esriEditorExt.UtilityNetworkAnalysisExt";
            m_utilNetExt = ArcMap.Application.FindExtensionByCLSID(uidUtilNet) as IUtilityNetworkAnalysisExt;

            // get the current network from the Utility Network Analysis extension
            INetworkAnalysisExt nax = m_utilNetExt as INetworkAnalysisExt;
            IGeometricNetwork geomNet = nax.CurrentNetwork;

            // get the geometric network's logical network
            INetwork net = geomNet.Network;

             */

            ////////// create an instance of the Feeder Manager class
            ////////// that performs electric tracing functions
            string strServicePointIDs = string.Empty;
            juncEIDs = null; edgeEIDs = null;
            if (elementType == esriElementType.esriETNone || elementType == esriElementType.esriETTurn) return string.Empty;

            IMMNetworkAnalysisExtForFramework MMNetAnalExtForFramework = null;
            IMMElectricNetworkTracing elecNetTracing = null;

            IMMElectricTraceSettingsEx elecTraceSettingsEx = null;
            IMMElectricTraceSettings elecTraceSetttings = null;

            IMMNetworkFeatureID mmNetFeatureId = null;

            IList<IMMNetworkFeatureID> edgeFeatureIds = new List<IMMNetworkFeatureID>(),
                juncFeatureIds = new List<IMMNetworkFeatureID>();

            esriElementType eleType;
            try
            {
                //Initialize Object of Trace settings.
                elecTraceSetttings = new MMElectricTraceSettings();
                elecTraceSetttings.RespectConductorPhasing = true;
                elecTraceSetttings.RespectEnabledField = true;

                //Query Interface Trace Settings object to TraceSettingEx
                elecTraceSettingsEx = (IMMElectricTraceSettingsEx)elecTraceSetttings;
                elecTraceSettingsEx.UseFeederManagerCircuitSources = true;
                elecTraceSettingsEx.RespectESRIBarriers = true;

                //Initialize Network Analysis Extension Framework object to assign Barriers.
                MMNetAnalExtForFramework = new MMNetworkAnalysisExtForFrameworkClass();
                if (BarrierIds != null && BarrierTypes != null && BarrierIds.Count > 0)
                {
                    try
                    {
                        foreach (int classId in BarrierIds.Keys)
                        {
                            if (!BarrierTypes.ContainsKey(classId)) continue;

                            eleType = BarrierTypes[classId];
                            if (eleType != esriElementType.esriETJunction && eleType != esriElementType.esriETEdge) continue;
                            foreach (int oid in BarrierIds[classId])
                            {
                                try
                                {
                                    mmNetFeatureId = new MMNetworkFeatureIDClass();
                                    mmNetFeatureId.FCID = classId;
                                    mmNetFeatureId.OID = oid;
                                    mmNetFeatureId.SUBID = 0;
                                    if (eleType == esriElementType.esriETEdge) edgeFeatureIds.Add(mmNetFeatureId);
                                    else if (eleType == esriElementType.esriETJunction) juncFeatureIds.Add(mmNetFeatureId);
                                }
                                catch (Exception ex)
                                {
                                    //LogUtility.LogExcpetion(ex);
                                }
                            }
                        }
                        if (edgeFeatureIds.Count > 0) MMNetAnalExtForFramework.EdgeBarriers = edgeFeatureIds.ToArray<IMMNetworkFeatureID>();
                        if (juncFeatureIds.Count > 0) MMNetAnalExtForFramework.JunctionBarriers = juncFeatureIds.ToArray<IMMNetworkFeatureID>();
                    }
                    catch (Exception ex)
                    {
                        //LogUtility.LogExcpetion(ex);
                    }
                }
                //Initialize Tracing object to perform Tracing.
                elecNetTracing = new MMFeederTracerClass();
                juncEIDs = null;
                edgeEIDs = null;
                elecNetTracing.TraceDownstream(inGeomNetwork, MMNetAnalExtForFramework, elecTraceSettingsEx, startElementEID, elementType,
                    mmPhasesToTrace.mmPTT_Any, out juncEIDs, out edgeEIDs);


                IEIDHelper pEIDHelper = new EIDHelperClass();
                pEIDHelper.GeometricNetwork = inGeomNetwork;
                pEIDHelper.ReturnFeatures = true;
                pEIDHelper.ReturnGeometries = false;
                //pEIDHelper.AddField("OBJECTID");
                pEIDHelper.AddField("GLOBALID");
                IEnumEIDInfo pEnumEIDInfo = pEIDHelper.CreateEnumEIDInfo(juncEIDs);
                IEIDInfo pEIDInfo = pEnumEIDInfo.Next();
                IFeature pFeature = null;
                IFeatureClass m_parentFeatureClass = null;
                IFeatureWorkspace m_featureWorkspace = null;
                IRelationshipClass m_relationshipClass = null;
                IDataset pDataSet = null;
                UtilityFunctions objUtilityFunctions = new UtilityFunctions();
                string RelationshipClassName = objUtilityFunctions.ReadConfigurationValue("RC_ServiceLocation_ServicPoint");
                ISet pRelatedObjsSet = null;
                IRow m_RelatedRow = null;
                strServicePointIDs = string.Empty;
                while (pEIDInfo != null)
                {
                    pFeature = pEIDInfo.Feature;
                    
                    pDataSet = pFeature.Class as IDataset;
                    if (pDataSet.Name.Equals(objUtilityFunctions.ReadConfigurationValue("FC_ServiceLocation")))
                    {
                        if (m_parentFeatureClass == null)
                        {
                            m_parentFeatureClass = (IFeatureClass)pFeature.Class;

                            m_featureWorkspace = (IFeatureWorkspace)m_parentFeatureClass.FeatureDataset.Workspace;
                            if (m_featureWorkspace == null)
                                throw new Exception("Could not load selected feature workspace");

                            m_relationshipClass = m_featureWorkspace.OpenRelationshipClass(RelationshipClassName);
                            if (m_relationshipClass == null)
                                throw new Exception(string.Format("Could not load {0} relationship class.", RelationshipClassName));
                        }
                        //pRelatedObjsSet = null;
                        pRelatedObjsSet = m_relationshipClass.GetObjectsRelatedToObject(pFeature);
                        m_RelatedRow = default(IRow);
                        if (pRelatedObjsSet.Count > 0)
                        {
                            m_RelatedRow = (IRow)pRelatedObjsSet.Next();
                            while (m_RelatedRow != null)
                            {
                                strServicePointIDs += Convert.ToString(m_RelatedRow.get_Value(m_RelatedRow.Fields.FindField("SERVICEPOINTID"))) + ",";
                                m_RelatedRow = default(IRow);
                                m_RelatedRow = (IRow)pRelatedObjsSet.Next();
                            }
                        }
                    }
                    pEIDInfo = pEnumEIDInfo.Next();
                }
                strServicePointIDs = strServicePointIDs.TrimEnd(',');
                //return strServicePointIDs;
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
            }
            finally
            {
                #region Release COM Objects
                mmNetFeatureId = null;
                if (edgeFeatureIds != null) edgeFeatureIds.Clear();
                if (juncFeatureIds != null) juncFeatureIds.Clear();

                elecTraceSettingsEx = null;
                if (elecTraceSetttings != null)
                {
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(elecTraceSetttings);
                    elecTraceSetttings = null;
                }
                if (MMNetAnalExtForFramework != null)
                {
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(MMNetAnalExtForFramework);
                    MMNetAnalExtForFramework = null;
                }
                if (elecNetTracing != null)
                {
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(elecNetTracing);
                    elecNetTracing = null;
                }
                #endregion
            }
            return strServicePointIDs;
        }

    }
}