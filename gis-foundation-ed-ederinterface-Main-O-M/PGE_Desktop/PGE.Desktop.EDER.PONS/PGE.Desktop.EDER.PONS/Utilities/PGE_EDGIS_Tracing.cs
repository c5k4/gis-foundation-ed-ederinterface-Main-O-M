using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ESRI.ArcGIS.EditorExt;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.NetworkAnalysis;
using Miner.Interop;
using System.Data;
using System.Collections;
using PGE.Common.Delivery.Diagnostics;

namespace PGE.Desktop.EDER.ArcMapCommands.PONS.Utilities
{
    class PGE_EDGIS_Tracing
    {
        private static readonly Log4NetLogger _logger = new Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "EDERDesktop.log4net.config");
        public bool RunTrace(IFeature startFeature, IFeature endFeature, bool ExcludeCustomers, string versionname, out IList<string> TransformerCGCs,
            out IList<string> PriMeterCGCs, ref bool isOnSameNetwork)
        {
            //string ServicePointIDs = string.Empty;
            bool bReturn = false;

            TransformerCGCs = new List<string>();
            PriMeterCGCs = new List<string>();

            //string sCGCs = string.Empty;
            UtilityFunctions objUtilityFunctions = default(UtilityFunctions);
            string[] PointFeatureClasses = default(string[]),
                    LineFeatureClasses = default(string[]);

            esriElementType StartDeviceElementType = esriElementType.esriETNone,
                EndDeviceElementType = esriElementType.esriETNone; ;
            IDataset pDataSet = default(IDataset);
            IUtilityNetworkAnalysisExt m_utilNetExt = default(IUtilityNetworkAnalysisExt);
            INetworkAnalysisExt nax = default(INetworkAnalysisExt);
            IGeometricNetwork geomNet = default(IGeometricNetwork);
            IFeatureWorkspace pFeatWS = default(IFeatureWorkspace);
            INetworkCollection pNWCollection = default(INetworkCollection);
            INetElements objNetElems = default(INetElements);
            IDictionary<int, IList<int>> BarrierIds = default(IDictionary<int, IList<int>>);
            IDictionary<int, esriElementType> BarrierTypes = default(IDictionary<int, esriElementType>);
            //IEnumNetEID edgeEIDs, juncEIDs = default(IEnumNetEID);
            IFeatureClass pEndFeatureClass = default(IFeatureClass);
            //isOnSameNetwork = isOnSameNetwork || false;
            try
            {

                objUtilityFunctions = new UtilityFunctions();

                PointFeatureClasses = objUtilityFunctions.ReadConfigurationValue("SearchBwDevicePointClasses").Split(';');
                LineFeatureClasses = objUtilityFunctions.ReadConfigurationValue("SearchBwDevicesLinearClasses").Split(';');
                
                pDataSet = startFeature.Class as IDataset;

                if (pDataSet != null)
                {
                    if (PointFeatureClasses.Contains(pDataSet.Name) || pDataSet.Name.ToUpper() == objUtilityFunctions.ReadConfigurationValue("Search3_CircuitSource").ToUpper())
                        StartDeviceElementType = esriElementType.esriETJunction;
                    else if (LineFeatureClasses.Contains(pDataSet.Name))
                        StartDeviceElementType = esriElementType.esriETEdge;
                }
                else
                    return bReturn;

                if (StartDeviceElementType == esriElementType.esriETNone)
                    return bReturn;

                m_utilNetExt = objUtilityFunctions.GetArcMapExtensionByCLSID("esriEditorExt.UtilityNetworkAnalysisExt") as IUtilityNetworkAnalysisExt;
                nax = default(INetworkAnalysisExt);
                geomNet = default(IGeometricNetwork);
                pFeatWS = default(IFeatureWorkspace);

                //if (PGEGlobal.WORKSPACE_MAP != null)
                //{
                //    // get the current network from the Utility Network Analysis extension
                //    //nax = m_utilNetExt as INetworkAnalysisExt;

                //    //geomNet = nax.CurrentNetwork;

                //    pFeatWS = PGEGlobal.worksp
                //}
                //else if(PGEGlobal.WORKSPACE_EDER != null)
                //{ 
                //    IFeatureWorkspace pFeatWS = PGEGlobal.WORKSPACE_EDER as IFeatureWorkspace;

                //    //INetworkCollection pNWCollection = pFeatWS.OpenFeatureDataset(objUtilityFunctions.ReadConfigurationValue("ElectricDataset")) as INetworkCollection;

                //    //geomNet = pNWCollection.get_GeometricNetworkByName(objUtilityFunctions.ReadConfigurationValue("ElectricDistNetwork"));
                //}

                pFeatWS = (PGEGlobal.WORKSPACE_MAP != null ? PGEGlobal.WORKSPACE_MAP : PGEGlobal.WORKSPACE_EDER) as IFeatureWorkspace;
                if (pFeatWS == null)
                    return bReturn;

                pNWCollection = pFeatWS.OpenFeatureDataset(objUtilityFunctions.ReadConfigurationValue("ElectricDataset")) as INetworkCollection;
                if (pNWCollection == null)
                    return bReturn;

                geomNet = pNWCollection.get_GeometricNetworkByName(objUtilityFunctions.ReadConfigurationValue("ElectricDistNetwork"));
                if (geomNet == null)
                    return bReturn;

                //objNetClass = objFeatClass as INetworkClass;
                //objGeoNet = objNetClass.GeometricNetwork;
                objNetElems = geomNet.Network as INetElements;

                if (endFeature != null)
                {
                    BarrierIds = new Dictionary<int, IList<int>>();
                    BarrierTypes = new Dictionary<int, esriElementType>();

                    pEndFeatureClass = default(IFeatureClass);
                    pEndFeatureClass = endFeature.Class as IFeatureClass;

                    if (pEndFeatureClass != null)
                    {
                        pDataSet = default(IDataset);
                        pDataSet = pEndFeatureClass as IDataset;

                        if (pDataSet != null)
                        {
                            if (PointFeatureClasses.Contains(pDataSet.Name))
                                EndDeviceElementType = esriElementType.esriETJunction;
                            else if (LineFeatureClasses.Contains(pDataSet.Name))
                                EndDeviceElementType = esriElementType.esriETEdge;

                            if (EndDeviceElementType != esriElementType.esriETNone)
                            {
                                int objEndEID = objNetElems.GetEID(pEndFeatureClass.FeatureClassID, endFeature.OID, -1, EndDeviceElementType);
                                IList<int> BarrierOID = new List<int>();
                                BarrierOID.Add(endFeature.OID);
                                BarrierIds.Add(pEndFeatureClass.FeatureClassID, BarrierOID);
                                BarrierTypes.Add(pEndFeatureClass.FeatureClassID, EndDeviceElementType);
                            }
                        }
                    }
                }

                int objEID = objNetElems.GetEID((startFeature.Class as IFeatureClass).FeatureClassID, startFeature.OID, -1, StartDeviceElementType);
                int objEID1 = -1;
                if (endFeature != null)
                {
                    objEID1 = objNetElems.GetEID((endFeature.Class as IFeatureClass).FeatureClassID, endFeature.OID, -1, EndDeviceElementType);
                }
                
                //ServicePointIDs = DownstreamTrace(geomNet, objEID, StartDeviceElementType, BarrierIds, BarrierTypes, ExcludeCustomers, versionname, out juncEIDs, out edgeEIDs);
                if (DownstreamTrace(geomNet, objEID, StartDeviceElementType, BarrierIds, BarrierTypes, ExcludeCustomers, versionname,
                    out TransformerCGCs, out PriMeterCGCs, startFeature, endFeature, ref isOnSameNetwork, EndDeviceElementType, objEID1))
                    bReturn = true;
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
            }
            finally
            {

                //UtilityFunctions objUtilityFunctions = default(UtilityFunctions);
                //string[] PointFeatureClasses = default(string[]),
                //        LineFeatureClasses = default(string[]);

                //esriElementType StartDeviceElementType = esriElementType.esriETNone,
                //    EndDeviceElementType = esriElementType.esriETNone; ;
                //IDataset pDataSet = default(IDataset);
                //IUtilityNetworkAnalysisExt m_utilNetExt = default(IUtilityNetworkAnalysisExt);
                //INetworkAnalysisExt nax = default(INetworkAnalysisExt);
                //IGeometricNetwork geomNet = default(IGeometricNetwork);
                //IFeatureWorkspace pFeatWS = default(IFeatureWorkspace);
                //INetworkCollection pNWCollection = default(INetworkCollection);
                //INetElements objNetElems = default(INetElements);
                //IDictionary<int, IList<int>> BarrierIds = default(IDictionary<int, IList<int>>);
                //IDictionary<int, esriElementType> BarrierTypes = default(IDictionary<int, esriElementType>);
                //IEnumNetEID edgeEIDs, juncEIDs = default(IEnumNetEID);
                //IFeatureClass pEndFeatureClass = default(IFeatureClass);

            }

            //return ServicePointIDs;
            //return sCGCs;
            return bReturn;
        }

        private DataTable pDT_FFF_Info = new DataTable();
        private UtilityFunctions objUtilityFunctions = new UtilityFunctions();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="inGeomNetwork"></param>
        /// <param name="startElementEID"></param>
        /// <param name="StartelementType"></param>
        /// <param name="BarrierIds">Dictionary with ClassID key and List of feature EIDs</param>
        /// <param name="BarrierTypes">Dictionary with ClassID and esriElementType</param>
        /// <param name="juncEIDs"></param>
        /// <param name="edgeEIDs"></param>
        public bool DownstreamTrace(IGeometricNetwork inGeomNetwork, int startElementEID, esriElementType StartelementType,
            IDictionary<int, IList<int>> BarrierIds, IDictionary<int, esriElementType> BarrierTypes, bool ExcludeCustomers, string versionname,
            out IList<string> TransformerCGCs, out IList<string> PriMeterCGCs, IFeature startFeature, IFeature endFeature, ref bool isOnSameNetwork
            , esriElementType EndelementType, int endElementEID)
        {
            //isOnSameNetwork = false;

            #region Feeder Fed Feeder
            pDT_FFF_Info = new DataTable();
            if (pDT_FFF_Info.Rows.Count == 0 || pDT_FFF_Info.Select("FROM_CIRCUITID = '" + Convert.ToString(startFeature.get_Value(startFeature.Fields.FindField("CIRCUITID"))) + "'").Length == 0)
                pDT_FFF_Info = objUtilityFunctions.ExecuteQuery("SELECT FROM_CIRCUITID, FROM_LINE_FC_NAME, FROM_LINE_GLOBALID, TO_CIRCUITID, TO_POINT_FC_GUID FROM " +
                            objUtilityFunctions.ReadConfigurationValue("FEEDERFEDINFOTABLE") +
                            " WHERE FROM_CIRCUITID = '" + Convert.ToString(startFeature.get_Value(startFeature.Fields.FindField("CIRCUITID"))) + "'", objUtilityFunctions.ReadConnConfigValue("EDER_ConnectionString"));

            if (pDT_FFF_Info.Rows.Count > 0)
                pDT_FFF_Info.Columns.Add("OID", typeof(int));
            IDictionary<string, IFeatureClass> pDic_FClass = new Dictionary<string, IFeatureClass>(); 
            foreach (DataRow pDRow in pDT_FFF_Info.Rows)
            {
                if (!pDic_FClass.ContainsKey(Convert.ToString(pDRow["FROM_LINE_FC_NAME"])))
                    pDic_FClass.Add(Convert.ToString(pDRow["FROM_LINE_FC_NAME"]), ((IFeatureWorkspace)((IDataset)startFeature.Class).Workspace).OpenFeatureClass(Convert.ToString(pDRow["FROM_LINE_FC_NAME"])));
                pDRow["OID"] = pDic_FClass[Convert.ToString(pDRow["FROM_LINE_FC_NAME"])].Search(new QueryFilter() { WhereClause = "GLOBALID = '" + Convert.ToString(pDRow["FROM_LINE_GLOBALID"] + "'") }, false).NextFeature().OID;
            }

            #endregion



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
            //string strServicePointIDs = string.Empty;
            bool bReturn = false;

            TransformerCGCs = new List<string>();
            PriMeterCGCs = new List<string>();

            IList<string> TransformerCGCs1 = new List<string>();
            IList<string> PriMeterCGCs1 = new List<string>();

            IList<string> TransformerCGCs2 = new List<string>();
            IList<string> PriMeterCGCs2 = new List<string>();

            IEnumNetEID juncEIDs = default(IEnumNetEID), edgeEIDs = default(IEnumNetEID);
            IEnumNetEID juncEIDs1 = default(IEnumNetEID), edgeEIDs1 = default(IEnumNetEID);

            string sReturnCGCs = string.Empty;

            juncEIDs = null; edgeEIDs = null;
            if (StartelementType == esriElementType.esriETNone || StartelementType == esriElementType.esriETTurn) return bReturn;

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
                                catch (Exception)
                                {
                                    //LogUtility.LogExcpetion(ex);
                                }
                            }
                        }
                       // if (edgeFeatureIds.Count > 0) MMNetAnalExtForFramework.EdgeBarriers = edgeFeatureIds.ToArray<IMMNetworkFeatureID>();
                       // if (juncFeatureIds.Count > 0) MMNetAnalExtForFramework.JunctionBarriers = juncFeatureIds.ToArray<IMMNetworkFeatureID>();
                    }
                    catch (Exception)
                    {
                        //LogUtility.LogExcpetion(ex);
                    }
                }
                //Initialize Tracing object to perform Tracing.
                elecNetTracing = new MMFeederTracerClass();
                juncEIDs = null;
                edgeEIDs = null;

                elecNetTracing.TraceDownstream(inGeomNetwork, MMNetAnalExtForFramework, elecTraceSettingsEx, startElementEID, StartelementType,
                    mmPhasesToTrace.mmPTT_Any, out juncEIDs, out edgeEIDs);
                
                if (endFeature != null)
                    elecNetTracing.TraceDownstream(inGeomNetwork, MMNetAnalExtForFramework, elecTraceSettingsEx, endElementEID, EndelementType,
                    mmPhasesToTrace.mmPTT_Any, out juncEIDs1, out edgeEIDs1);

                IEIDHelper pEIDHelper = new EIDHelperClass();
                //UtilityFunctions objUtilityFunctions = new UtilityFunctions();
                pEIDHelper.GeometricNetwork = inGeomNetwork;
                pEIDHelper.ReturnFeatures = true;
                pEIDHelper.ReturnGeometries = false;
                //pEIDHelper.AddField("OBJECTID");
                //pEIDHelper.AddField("GLOBALID");
                pEIDHelper.AddField(objUtilityFunctions.ReadConfigurationValue("Field_CGC12"));

                IEnumEIDInfo pEnumEIDInfo = pEIDHelper.CreateEnumEIDInfo(juncEIDs);
                IEIDInfo pEIDInfo = pEnumEIDInfo.Next();
                IFeature pFeature = null;
                //IFeatureClass m_parentFeatureClass = null;
                //IFeatureWorkspace m_featureWorkspace = null;
                //IRelationshipClass m_relationshipClass = null;
                IDataset pDataSet = null;
                //string RelationshipClassName = objUtilityFunctions.ReadConfigurationValue("RC_ServiceLocation_ServicPoint");
                //ISet pRelatedObjsSet = null;
                //IRow m_RelatedRow = null;
                //strServicePointIDs = string.Empty;
                sReturnCGCs = string.Empty;
                int iCount_StartEndonSame = 0;
                while (pEIDInfo != null)
                {
                    pFeature = pEIDInfo.Feature;

                    pDataSet = pFeature.Class as IDataset;
                    //sReturnCGCs += Convert.ToString(pFeature.get_Value(pFeature.Fields.FindField(objUtilityFunctions.ReadConfigurationValue("Field_CGC12")))) + ",";
                    if (iCount_StartEndonSame < 1)
                    {
                        //try
                        //{
                        //    iCount_StartEndonSame += (((IDataset)startFeature.Class).Name == pDataSet.Name && startFeature.OID == pFeature.OID) ? 1 : 0;
                        //}
                        //catch { }
                        try
                        {
                            iCount_StartEndonSame += (((IDataset)endFeature.Class).Name == pDataSet.Name && endFeature.OID == pFeature.OID) ? 1 : 0;
                        }
                        catch { }
                    }
                    if (pDataSet.Name.Equals(objUtilityFunctions.ReadConfigurationValue("Transformer")))
                    {
                        TransformerCGCs1.Add(Convert.ToString(pFeature.get_Value(pFeature.Fields.FindField(objUtilityFunctions.ReadConfigurationValue("Field_CGC12")))));
                    }
                    else if (pDataSet.Name.Equals(objUtilityFunctions.ReadConfigurationValue("PrimaryMeter")))
                    {
                        PriMeterCGCs1.Add(Convert.ToString(pFeature.get_Value(pFeature.Fields.FindField(objUtilityFunctions.ReadConfigurationValue("Field_CGC12")))));
                    }

                    #region commented
                    //if (pDataSet.Name.Equals(objUtilityFunctions.ReadConfigurationValue("FC_ServiceLocation")))
                    //{
                    //    if (m_parentFeatureClass == null)
                    //    {
                    //        m_parentFeatureClass = (IFeatureClass)pFeature.Class;

                    //        m_featureWorkspace = (IFeatureWorkspace)m_parentFeatureClass.FeatureDataset.Workspace;
                    //        if (m_featureWorkspace == null)
                    //            throw new Exception("Could not load selected feature workspace");

                    //        m_relationshipClass = m_featureWorkspace.OpenRelationshipClass(RelationshipClassName);
                    //        if (m_relationshipClass == null)
                    //            throw new Exception(string.Format("Could not load {0} relationship class.", RelationshipClassName));
                    //    }
                    //    //pRelatedObjsSet = null;
                    //    pRelatedObjsSet = m_relationshipClass.GetObjectsRelatedToObject(pFeature);
                    //    m_RelatedRow = default(IRow);
                    //    if (pRelatedObjsSet.Count > 0)
                    //    {
                    //        m_RelatedRow = (IRow)pRelatedObjsSet.Next();
                    //        while (m_RelatedRow != null)
                    //        {
                    //            strServicePointIDs += Convert.ToString(m_RelatedRow.get_Value(m_RelatedRow.Fields.FindField("SERVICEPOINTID"))) + ",";
                    //            m_RelatedRow = default(IRow);
                    //            m_RelatedRow = (IRow)pRelatedObjsSet.Next();
                    //        }
                    //    }
                    //}
                    #endregion

                    pEIDInfo = pEnumEIDInfo.Next();
                }
                if (endFeature != null)
                {
                    pEnumEIDInfo = pEIDHelper.CreateEnumEIDInfo(juncEIDs1);
                    pEIDInfo = pEnumEIDInfo.Next();
                    pFeature = null;
                    //IFeatureClass m_parentFeatureClass = null;
                    //IFeatureWorkspace m_featureWorkspace = null;
                    //IRelationshipClass m_relationshipClass = null;
                    pDataSet = null;
                    //string RelationshipClassName = objUtilityFunctions.ReadConfigurationValue("RC_ServiceLocation_ServicPoint");
                    //ISet pRelatedObjsSet = null;
                    //IRow m_RelatedRow = null;
                    //strServicePointIDs = string.Empty;
                    sReturnCGCs = string.Empty;
                    //int iCount_StartEndonSame = 0;
                    while (pEIDInfo != null)
                    {
                        pFeature = pEIDInfo.Feature;

                        pDataSet = pFeature.Class as IDataset;

                        if (iCount_StartEndonSame < 1)
                        {
                            try
                            {
                                iCount_StartEndonSame += (((IDataset)startFeature.Class).Name == pDataSet.Name && startFeature.OID == pFeature.OID) ? 1 : 0;
                            }
                            catch { }
                            //try
                            //{
                            //    iCount_StartEndonSame += (((IDataset)endFeature.Class).Name == pDataSet.Name && endFeature.OID == pFeature.OID) ? 1 : 0;
                            //}
                            //catch { }
                        }

                        if (pDataSet.Name.Equals(objUtilityFunctions.ReadConfigurationValue("Transformer")))
                        {
                            TransformerCGCs2.Add(Convert.ToString(pFeature.get_Value(pFeature.Fields.FindField(objUtilityFunctions.ReadConfigurationValue("Field_CGC12")))));
                        }
                        else if (pDataSet.Name.Equals(objUtilityFunctions.ReadConfigurationValue("PrimaryMeter")))
                        {
                            PriMeterCGCs2.Add(Convert.ToString(pFeature.get_Value(pFeature.Fields.FindField(objUtilityFunctions.ReadConfigurationValue("Field_CGC12")))));
                        }

                        pEIDInfo = pEnumEIDInfo.Next();
                    }
                }

                isOnSameNetwork = isOnSameNetwork || iCount_StartEndonSame == 1;

                //if (endFeature == null) isOnSameNetwork = true;

                if (endFeature == null)
                {
                    TransformerCGCs = TransformerCGCs1;
                    PriMeterCGCs = PriMeterCGCs1;
                }
                else
                {
                    TransformerCGCs = TransformerCGCs1.Except(TransformerCGCs2).ToList();
                    PriMeterCGCs = PriMeterCGCs1.Except(PriMeterCGCs2).ToList();

                    if (!(TransformerCGCs.Count == TransformerCGCs1.Count && TransformerCGCs2.Count != 0) || TransformerCGCs.Count == 0)
                    {
                        TransformerCGCs = TransformerCGCs2.Except(TransformerCGCs1).ToList();
                    }

                    if (!(PriMeterCGCs.Count == PriMeterCGCs1.Count && PriMeterCGCs2.Count != 0) || PriMeterCGCs.Count == 0)
                    {
                        PriMeterCGCs = PriMeterCGCs2.Except(PriMeterCGCs1).ToList();
                    }
                }
                DataRow[] pDRow_Temp;// = new DataRow();
                ArrayList pList_ChildSources = new ArrayList();

                if (pDT_FFF_Info.Rows.Count > 0)
                {
                    pEnumEIDInfo = pEIDHelper.CreateEnumEIDInfo(edgeEIDs);
                    pEIDInfo = pEnumEIDInfo.Next();
                    while (pEIDInfo != null)
                    {
                        pFeature = pEIDInfo.Feature;
                        try
                        {
                            //if ((pDRow_Temp = pDT_FFF_Info.Select("FROM_LINE_FC_NAME = '" + ((IDataset)pFeature.Class).Name + "' AND FROM_LINE_GLOBALID = '" + Convert.ToString(((IFeatureClass)pFeature.Class).GetFeature(pFeature.OID).get_Value(pFeature.Fields.FindField("GLOBALID"))) + "'")).Length > 0)
                            if ((pDRow_Temp = pDT_FFF_Info.Select("FROM_LINE_FC_NAME = '" + ((IDataset)pFeature.Class).Name + "' AND OID = " + pFeature.OID)).Length > 0)
                                foreach (DataRow pDRow in pDRow_Temp)
                                    pList_ChildSources.Add(pDRow["TO_POINT_FC_GUID"]);

                            if (pList_ChildSources.Count == pDT_FFF_Info.Rows.Count) break;
                        }
                        catch { }

                        pEIDInfo = pEnumEIDInfo.Next();
                    }

                    if (endFeature != null)
                    {
                        pEnumEIDInfo = pEIDHelper.CreateEnumEIDInfo(edgeEIDs1);
                        pEIDInfo = pEnumEIDInfo.Next();
                        while (pEIDInfo != null)
                        {
                            pFeature = pEIDInfo.Feature;
                            try
                            {
                                //if ((pDRow_Temp = pDT_FFF_Info.Select("FROM_LINE_FC_NAME = '" + ((IDataset)pFeature.Class).Name + "' AND FROM_LINE_GLOBALID = '" + Convert.ToString(((IFeatureClass)pFeature.Class).GetFeature(pFeature.OID).get_Value(pFeature.Fields.FindField("GLOBALID"))) + "'")).Length > 0)
                                if ((pDRow_Temp = pDT_FFF_Info.Select("FROM_LINE_FC_NAME = '" + ((IDataset)pFeature.Class).Name + "' AND OID = " + pFeature.OID)).Length > 0)
                                    foreach (DataRow pDRow in pDRow_Temp)
                                        pList_ChildSources.Add(pDRow["TO_POINT_FC_GUID"]);

                                if (pList_ChildSources.Count == 0) break;
                            }
                            catch { }
                            pEIDInfo = pEnumEIDInfo.Next();
                        }
                    }
                }
                if (pList_ChildSources.Count > 0)
                {
                    string sGUIDs_CircuitSource = string.Empty;
                    foreach (object obj in pList_ChildSources)
                    {
                        sGUIDs_CircuitSource += "'" + obj.ToString() + "',";
                    }
                    sGUIDs_CircuitSource = sGUIDs_CircuitSource.Substring(0, sGUIDs_CircuitSource.Length - 1);

                    PGEFeatureClass pPFClass_CisuitSource = (new MapUtility()).GetFeatureClassByName(objUtilityFunctions.ReadConfigurationValue("Search3_CircuitSource"));
                    IFeatureCursor pFCursor = pPFClass_CisuitSource.FeatureClass.Search(new QueryFilterClass() { WhereClause = "GLOBALID IN (" + sGUIDs_CircuitSource + ")" }, false);

                    IFeature pFeat_CS = default(IFeature);
                    while ((pFeat_CS = pFCursor.NextFeature()) != null)
                    {
                        IList<string> tempTransCGCs = new List<string>(),
                           tempPriCGCs = new List<string>();
                        bool bStatus = false;
                        //bStatus = objTracing.RunTrace(startFeature, endFeature, this.checkBoxExclude.Checked, sVersionName, out tempTransCGCs, out tempPriCGCs);
                        bool isOnSameNetwork1 = false;
                        bStatus = RunTrace(pFeat_CS, null, !true, versionname, out tempTransCGCs, out tempPriCGCs, ref isOnSameNetwork1);

                        if (bStatus && tempTransCGCs.Count > 0)
                            ((List<string>)TransformerCGCs).AddRange(tempTransCGCs);

                        if (bStatus && tempPriCGCs.Count > 0)
                            ((List<string>)PriMeterCGCs).AddRange(tempPriCGCs);
                    }
                    pFCursor = null; pPFClass_CisuitSource = null;
                }
                //strServicePointIDs = strServicePointIDs.TrimEnd(',');
                //sReturnCGCs = sReturnCGCs.TrimEnd(',');
                //return strServicePointIDs;
                
                bReturn = true;

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
            // return strServicePointIDs;
            //return sReturnCGCs;
            return bReturn;
        }

    }
}
