using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Data.SqlClient; 

//using ESRI.ArcGIS.ArcMapUI;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.DataSourcesRaster;
//using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Framework;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.NetworkAnalysis; 

namespace PoleLoadExtract
{ 
    class PoleLoad
    {
        Hashtable _hshAllAttributes = null;
        Hashtable _hshAllDomains = null;
        IFeatureClass m_SnowLoadingFC = null;
        IFeatureClass m_PopDensityFC = null;
        IFeatureClass m_ClimateZoneFC = null;
        IFeatureClass m_CorrosionAreaFC = null;
        IFeatureClass m_RCZ_FC = null;
        IFeatureClass m_SurgeProtFC = null;
        IFeatureClass m_InsulationFC = null;
        IFeatureClass m_UWF_FC = null;
        IFeatureClass m_PeakWindFC = null;


        IFeatureClass m_SupportStructureFC = null;
        IFeatureClass m_PriOHConductorFC = null;
        IFeatureClass m_SecOHConductorFC = null;
        IFeatureClass m_NeutralConductorFC = null; 
        IFeatureClass m_CapBankFC = null;
        IFeatureClass m_TransformerFC = null;
        IFeatureClass m_AnchorGuyFC = null;
        IFeatureClass m_VoltageRegulatorFC = null;
        IFeatureClass m_StreetlightFC = null;
        IFeatureClass m_OpenPointFC = null; 
        IFeatureClass m_FuseFC = null;
        IFeatureClass m_SwitchFC = null;
        IFeatureClass m_DPDFC = null;
        IFeatureClass m_PrimaryRiserFC = null;
        IFeatureClass m_SecondaryRiserFC = null;
        IFeatureClass m_AntennaFC = null;
        IFeatureClass m_PVFC = null;
        IFeatureClass m_StepDownFC = null;

        private SqlConnection  _pConn = null;
        private ISpatialReference m_pSR_WGS84; 
        
        ITable m_PriOHConductorInfoTbl = null;
        ITable m_SecOHConductorInfoTbl = null;
        ITable m_TransformerUnitTbl = null;
        ITable m_StepDownUnitTbl = null;
        ITable m_VoltageRegUnitTbl = null;
        ITable m_JointOwnerTbl = null;

        IRelationshipClass m_PriOHConductorInfoRC = null;
        IRelationshipClass m_SecOHConductorInfoRC = null;
        IRelationshipClass m_TransformerRC = null;
        IRelationshipClass m_JointUseAttachmentRC = null;
        IRelationshipClass m_VoltageRegulatorRC = null; 
        IRelationshipClass m_TransformerUnitRC = null;
        IRelationshipClass m_VoltageRegUnitRC = null;
        IRelationshipClass m_StepDownUnitRC = null;
        IRelationshipClass m_AnchorGuyRC = null;
        IRelationshipClass m_CapBankRC = null;
        IRelationshipClass m_DPDRC = null;
        IRelationshipClass m_StreetlightRC = null;
        IRelationshipClass m_JointOwnerRC = null;
        IRelationshipClass m_FuseRC = null;
        IRelationshipClass m_SwitchRC = null;
        IRelationshipClass m_OpenPointRC = null;
        IRelationshipClass m_PrimaryRiserRC = null;
        IRelationshipClass m_SecondaryRiserRC = null;
        IRelationshipClass m_AntennaRC = null;
        IRelationshipClass m_PVRC = null;
        IRelationshipClass m_StepDownRC = null;       

        public PoleLoad()
        {

        }

        //public void Main()
        //{
        //    try
        //    {
        //        //Load up the workspaces 
        //        Shared.LoadConfigurationSettings();
        //        Shared.InitializeLogfile();
        //        Shared.InitializeSpanCSVfile();
        //        Shared.InitializeFullDataCSVfile();
        //        Shared.InitializeStaticCSVfile();
        //        Shared.WriteToLogfile("Entering Main");
        //        Shared.WriteToLogfile("connecting to workspaces");
        //        Shared.LoadWorkspaces();
        //        Shared.WriteToLogfile("======================================================");

        //        //int poleOId = 30044026;
        //        //ProcessPole(poleOId); 
        //        InitializeData();
        //        ProcessPoles(); 

        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception("Error encountered in Main");
        //    }
        //}

        private double GetHeightAtPoint(IPoint pPoint, IIdentify pIdentify)
        {
            try
            {

                double rasterValue = -1;

                //Get RasterIdentifyObject on that point 
                IArray pIDArray = pIdentify.Identify(pPoint);
                if (pIDArray != null)
                {
                    IRasterIdentifyObj pRIDObj = (IRasterIdentifyObj)pIDArray.get_Element(0);
                    rasterValue = Convert.ToDouble(pRIDObj.Name);
                }

                return Math.Round(rasterValue, 1);
            }
            catch (Exception ex)
            {
                Shared.WriteToLogfile("Error in " + MethodBase.GetCurrentMethod() +
                    " details: " + ex.Message);
                throw new Exception("Error returning the height at point");
            }
        }

        private List<ILayer> getLayersFromMap(IMap map, string featureClassName)
        {
            List<ILayer> layers = new List<ILayer>();

            //Get all of the IFeatureLayers
            //string shortFCName = GetShortDatasetName(featureClassName).ToUpper();
            ESRI.ArcGIS.esriSystem.UID uid = new ESRI.ArcGIS.esriSystem.UIDClass();
            uid.Value = "{40A9E885-5533-11D0-98BE-00805F7CED21}";
            IEnumLayer enumLayer = map.Layers;
            enumLayer.Reset();
            ILayer layer = enumLayer.Next();
            while (layer != null)
            {

                if (layer.Name.ToLower() == featureClassName.ToLower())
                {
                    layers.Add(layer);
                }

                layer = enumLayer.Next();
            }
            return layers;
        }


        /// <summary>
        /// CalculatePoleSpans 
        /// Fille in span attributes for each pole 
        /// 
        /// </summary>
        //private void CalculatePoleSpans()
        //{

        //    try
        //    {

        //        //Open up the poles featureclass 
        //        Hashtable hshAllAttributes = new Hashtable();
        //        IFeatureWorkspace pFileGDB_FWS = (IFeatureWorkspace)Shared.GetWorkspaceByName("output"); 
        //        IFeatureWorkspace pFWS = (IFeatureWorkspace)Shared.GetWorkspaceByName("electric");
        //        IFeatureClass pSupportStructureFC = pFWS.OpenFeatureClass("edgis.supportstructure");
        //        IFeatureClass pPriOHConductorFC = pFWS.OpenFeatureClass("edgis.priohconductor");
        //        CacheAttributesForOC(pPriOHConductorFC, ref hshAllAttributes); 
        //        ITable pPriOHConductorInfoTbl = pFWS.OpenTable("edgis.priohconductorinfo");
        //        CacheAttributesForOC((IObjectClass)pPriOHConductorInfoTbl, ref hshAllAttributes);
        //        IRelationshipClass pPriOHConductorInfoRC = pFWS.OpenRelationshipClass("EDGIS.PriOHConductor_PriOHConductorInfo");
        //        IFeatureClass pSecOHConductorFC = pFWS.OpenFeatureClass("edgis.secohconductor");
        //        CacheAttributesForOC(pSecOHConductorFC, ref hshAllAttributes);
        //        ITable pSecOHConductorInfoTbl = pFWS.OpenTable("edgis.secohconductorinfo");
        //        CacheAttributesForOC((IObjectClass)pSecOHConductorInfoTbl, ref hshAllAttributes);              
        //        IRelationshipClass pSecOHConductorInfoRC = pFWS.OpenRelationshipClass("EDGIS.SecOHConductor_SecOHConductorInfo");
        //        IFeatureClass pTransformerFC = pFWS.OpenFeatureClass("edgis.transformer");
        //        CacheAttributesForOC(pTransformerFC, ref hshAllAttributes);              
        //        IRelationshipClass pTransformerRC = pFWS.OpenRelationshipClass("EDGIS.SupportStruct_Transformer");
        //        ITable pTransformerUnitTbl = pFWS.OpenTable("edgis.transformerunit");
        //        CacheAttributesForOC((IObjectClass)pTransformerUnitTbl, ref hshAllAttributes);
        //        IRelationshipClass pTransformerUnitRC = pFWS.OpenRelationshipClass("EDGIS.Transformer_TransformerUnit");
        //        IFeatureClass pAnchorGuyFC = pFWS.OpenFeatureClass("edgis.anchorguy");
        //        CacheAttributesForOC((IObjectClass)pAnchorGuyFC, ref hshAllAttributes);
        //        IRelationshipClass pAnchorGuyRC = pFWS.OpenRelationshipClass("EDGIS.SupportStruct_AnchorGuy");
        //        IFeatureClass pCapBankFC = pFWS.OpenFeatureClass("edgis.capacitorbank");
        //        CacheAttributesForOC((IObjectClass)pCapBankFC, ref hshAllAttributes);
        //        IRelationshipClass pCapBankRC = pFWS.OpenRelationshipClass("EDGIS.SupportStruct_CapacitorBank");
        //        IFeatureClass pDPDFC = pFWS.OpenFeatureClass("edgis.dynamicprotectivedevice");
        //        CacheAttributesForOC((IObjectClass)pDPDFC, ref hshAllAttributes);
        //        IRelationshipClass pDPDRC = pFWS.OpenRelationshipClass("EDGIS.SupportStruct_DynProtDev");
        //        ITable pJointOwnerTbl = pFWS.OpenTable("edgis.jointowner");
        //        CacheAttributesForOC((IObjectClass)pJointOwnerTbl, ref hshAllAttributes);
        //        IRelationshipClass pJointOwnerRC = pFWS.OpenRelationshipClass("EDGIS.SupportStructure_JointOwner");
        //        IFeatureClass pStreetlightFC = pFWS.OpenFeatureClass("edgis.streetlight");
        //        CacheAttributesForOC((IObjectClass)pStreetlightFC, ref hshAllAttributes);
        //        IRelationshipClass pStreetlightRC = pFWS.OpenRelationshipClass("EDGIS.SupportStruct_Streetlight");
        //        IFeatureClass pFuseFC = pFWS.OpenFeatureClass("edgis.Fuse");
        //        CacheAttributesForOC((IObjectClass)pFuseFC, ref hshAllAttributes);
        //        IRelationshipClass pFuseRC = pFWS.OpenRelationshipClass("EDGIS.SupportStruct_Fuse");
        //        IFeatureClass pSwitchFC = pFWS.OpenFeatureClass("edgis.Switch");
        //        CacheAttributesForOC((IObjectClass)pSwitchFC, ref hshAllAttributes);
        //        IRelationshipClass pSwitchRC = pFWS.OpenRelationshipClass("EDGIS.SupportStruct_Switch");
        //        IFeatureClass pVoltageRegulatorFC = pFWS.OpenFeatureClass("edgis.VoltageRegulator");
        //        CacheAttributesForOC((IObjectClass)pVoltageRegulatorFC, ref hshAllAttributes);
        //        IRelationshipClass pVoltageRegulatorRC = pFWS.OpenRelationshipClass("EDGIS.SupportStruct_VoltReg");


        //        IPolygon pClipPolygon = Shared.GetClipPolygon();
        //        IRelationalOperator pRO_MotherLoad = (IRelationalOperator)pClipPolygon;
        //        IQueryFilter pQF = new QueryFilterClass();

        //        IFeatureCursor pFCursor = null;
        //        IFeature pSupportStructFeature = null; 


        //        //Get the raster layer in the map 
        //        IMxDocument pMxDoc = (IMxDocument)m_app.Document;
        //        List<ILayer> pLayers = getLayersFromMap(pMxDoc.FocusMap, "Heights");
        //        IRasterLayer pRasterLayer = (IRasterLayer)pLayers[0];
        //        IIdentify pIdentify = (IIdentify)pRasterLayer;
        //        int poleGUIDFldIdx = pSupportStructureFC.Fields.FindField("GLOBALID");
        //        string fromPoleGUID = string.Empty;
        //        string toPoleGUID = string.Empty;
        //        //string replaceGUID = string.Empty;


        //        //Step 1. Get the O/H conductors within configureable tolerance 
        //        //of each pole and put them in a bucket 
        //        IMap pMap = ((IMxDocument)m_app.Document).FocusMap;
        //        IActiveView pAV = (IActiveView)pMap;                
        //        List<Conductor> pConductorList = new List<Conductor>();
        //        ITopologicalOperator pTopo = null;
        //        ITopologicalOperator kTopo = null;
        //        IPolygon pSearchPoly = null;
        //        IPolygon pExtentPoly = null; 
        //        IRelationalOperator pRO = null;
        //        IProximityOperator pPO = null;
        //        IFeature pAdjacentPoleFeature = null; 
        //        Span pSpan = null;
        //        int segCount = -1;
        //        int spanCount = 0;
        //        int poleCount = 0;
        //        double fromPoleRL = -1;
        //        double toPoleRL = -1;
        //        Hashtable hshSpans = new Hashtable();
        //        List<Span> pSpanList = new List<Span>();
        //        WriteSpanCSVHeader(null);
        //        WriteFullDataCSVHeader(null);


        //        Hashtable hsh715ALsPlusMLs = Get715ALsListPlusMLs(pFileGDB_FWS, "SupportStructure_ALL_2", pClipPolygon);

        //        //Create index on SupportStructure feature class
        //        //IFeatureIndex ifxidx = CreateFeatureIndex(pSupportStructureFC);

        //        string[] commaSeparatedkeyList = GetArrayOfCommaSeparatedKeys(
        //            hsh715ALsPlusMLs, 1000, false);
        //        for (int j = 0; j < commaSeparatedkeyList.Length; j++)
        //        {

        //            pQF.WhereClause = "objectid" + " IN(" + commaSeparatedkeyList[j] + ")";
        //            pFCursor = pSupportStructureFC.Search(pQF, false);
        //            pSupportStructFeature = pFCursor.NextFeature();

        //            while (pSupportStructFeature != null)
        //            {

        //                //If the pole is within mother load or the pole is in the list of 
        //                //poles with 715 AL then process the pole 
        //                poleCount++;
        //                //Imran - Code to Capture the pole elevation
        //                IPoint ptPole = (IPoint)pSupportStructFeature.ShapeCopy;
        //                Double poleElevation = GetHeightAtPoint(ptPole, pIdentify);

        //                Debug.Print("pole Height is " + poleElevation + " for :" + pSupportStructFeature.OID);

        //                                        Debug.Print("========================================");
        //                    Debug.Print("Analyzing pole: " + pSupportStructFeature.OID.ToString());

        //                    fromPoleGUID = pSupportStructFeature.get_Value(poleGUIDFldIdx).ToString();
        //                    toPoleGUID = string.Empty;
        //                    spanCount = 0;
        //                    pSpanList.Clear();

        //                    pTopo = (ITopologicalOperator)pSupportStructFeature.ShapeCopy;
        //                    pSearchPoly = (IPolygon)pTopo.Buffer(Shared.PoleBuffer);
        //                    pTopo = (ITopologicalOperator)pSupportStructFeature.ShapeCopy;
        //                    pExtentPoly = (IPolygon)pTopo.Buffer(300);
        //                    pAV.Extent = pExtentPoly.Envelope;
        //                    pRO = (IRelationalOperator)pSearchPoly;
        //                    pPO = (IProximityOperator)pSupportStructFeature.ShapeCopy;

        //                    pConductorList.Clear();

        //                    //Get intersecting primary conductors 
        //                    GetNearbyConductors(
        //                        pPriOHConductorFC,
        //                        (Hashtable)hshAllAttributes["PRIOHCONDUCTOR"],
        //                        pPriOHConductorInfoRC,
        //                        pPriOHConductorInfoTbl,
        //                        (Hashtable)hshAllAttributes["PRIOHCONDUCTORINFO"],
        //                        pSearchPoly,
        //                        pPO,
        //                        ref pConductorList);

        //                    //Get intersecting secondary conductors
        //                    GetNearbyConductors(
        //                        pSecOHConductorFC,
        //                        (Hashtable)hshAllAttributes["SECOHCONDUCTOR"],
        //                        pSecOHConductorInfoRC,
        //                        pSecOHConductorInfoTbl,
        //                        (Hashtable)hshAllAttributes["SECOHCONDUCTORINFO"],
        //                        pSearchPoly,
        //                        pPO,
        //                        ref pConductorList);

        //                    Debug.Print("Found: " + pConductorList.Count.ToString() + " nearby conductors");
        //                    foreach (Conductor pConductor in pConductorList)
        //                    {
        //                        ISegmentCollection pSegColl = (ISegmentCollection)pConductor.Shape;
        //                        segCount = pSegColl.SegmentCount;
        //                        Debug.Print("Seg Count: " + segCount.ToString());

        //                        for (int i = 0; i < pSegColl.SegmentCount; i++)
        //                        {
        //                            ISegment pSeg = pSegColl.get_Segment(i);
        //                            Debug.Print("Seg Length: " + pSeg.Length);
        //                            if(pSeg.Length <3)
        //                            {
        //                            continue;
        //                            }
        //                            if (pRO.Contains(pSeg.FromPoint) ||
        //                            pRO.Contains(pSeg.ToPoint))
        //                            {
        //                                ILine pLine = new LineClass();
        //                                pLine.SpatialReference = pConductor.Shape.SpatialReference;

        //                                //pLine.FromPoint is always the current pole 
        //                                if (pRO.Contains(pSeg.FromPoint))
        //                                    pLine.PutCoords(pSeg.FromPoint, pSeg.ToPoint);
        //                                else if (pRO.Contains(pSeg.ToPoint))
        //                                    pLine.PutCoords(pSeg.ToPoint, pSeg.FromPoint);
        //                                else
        //                                {
        //                                    //Case where there is no vertex where the 
        //                                    //pole is located 
        //                                    Debug.Print("No vertex found - have to find closest point on line and use this");
        //                                }

        //                                //Extent out the line to a projected point 
        //                                IConstructPoint contructionPoint = new PointClass();
        //                                contructionPoint.ConstructAlong(
        //                                    (ICurve)pLine, esriSegmentExtension.esriExtendAtTo, 2000, false);

        //                                //Construct the line for the buffer 
        //                                ILine pLineForBuffer = new LineClass();
        //                                pLineForBuffer.SpatialReference = pConductor.Shape.SpatialReference;
        //                                pLineForBuffer.PutCoords(pLine.FromPoint, (IPoint)contructionPoint);
        //                                IPolyline pPolylineForBuffer = GetPolylineFromLine(pLineForBuffer,
        //                                    pConductor.Shape.SpatialReference);

        //                                //Buffer the line to create the corridor
        //                                kTopo = (ITopologicalOperator)pPolylineForBuffer;
        //                                IPolygon pBufferPoly = (IPolygon)kTopo.Buffer(Shared.PoleBuffer);
        //                                //IGeometry pGeomPoly = kTopo.Buffer(Shared.PoleBuffer);

        //                                //Search for adjacent poles within the ray buffer and 
        //                                //find the closest one in each direction and this will be 
        //                                //the adjacent pole 

        //                                pAdjacentPoleFeature = FindAdjacentPole(
        //                                    (IPoint)pSupportStructFeature.ShapeCopy,
        //                                    pBufferPoly,
        //                                    pSupportStructureFC,
        //                                    pSupportStructFeature.OID,
        //                                    pConductor.CircId//,
        //                                    //ifxidx,
        //                                   // pGeomPoly
        //                                   );


        //                                if (pAdjacentPoleFeature != null)
        //                                {
        //                                    spanCount++;

        //                                    toPoleGUID = pAdjacentPoleFeature.get_Value(poleGUIDFldIdx).ToString();
        //                                    fromPoleRL = GetHeightAtPoint(pLine.FromPoint, pIdentify);
        //                                    toPoleRL = GetHeightAtPoint(pLine.ToPoint, pIdentify);
        //                                    pSpan = new Span(
        //                                        spanCount,
        //                                        GetSpanAngleDegrees(pLine.Angle),
        //                                        pPO.ReturnDistance(pAdjacentPoleFeature.ShapeCopy),
        //                                        pConductor.FCName.ToUpper(),
        //                                        pConductor.Subtype,
        //                                        pConductor.GUID,
        //                                        fromPoleGUID,
        //                                        fromPoleRL,
        //                                        toPoleGUID,
        //                                        toPoleRL,
        //                                        pConductor.CircId,
        //                                        poleElevation);
        //                                    if (!SpanIsDuplicate(pSpan, pSpanList))
        //                                        pSpanList.Add(pSpan);

        //                                }
        //                                else
        //                                {
        //                                    //Check if there is a SL at the other end of the line 
        //                                    //If this is suptype 3 or subtype 5 this is a service span
        //                                    if (((pConductor.Subtype == "Service Overhead Conductor") /*|| (pConductor.Subtype == "Streetlight Overhead Conductor") 
        //                                        || (pConductor.Subtype == "Pseudo Service")*/) && (pConductor.FCName.ToUpper() == "SECOHCONDUCTOR")) //AND SECONARY 
        //                                    {
        //                                        //This is a service span probably going to the servicelocation
        //                                        if (segCount == 1)
        //                                        {
        //                                            spanCount++;
        //                                            IPoint pSL = pLine.ToPoint;
        //                                            pSL.SpatialReference = pConductor.Shape.SpatialReference;
        //                                            fromPoleRL = GetHeightAtPoint(pLine.FromPoint, pIdentify);
        //                                            toPoleGUID = string.Empty;
        //                                            toPoleRL = GetHeightAtPoint(pLine.ToPoint, pIdentify);

        //                                            pSpan = new Span(
        //                                                spanCount,
        //                                                GetSpanAngleDegrees(pLine.Angle),
        //                                                pPO.ReturnDistance(pSL),
        //                                                pConductor.FCName.ToUpper(),
        //                                                pConductor.Subtype,
        //                                                pConductor.GUID,
        //                                                fromPoleGUID,
        //                                                fromPoleRL,
        //                                                toPoleGUID,
        //                                                toPoleRL,
        //                                                pConductor.CircId,
        //                                                poleElevation);
        //                                            if (!SpanIsDuplicate(pSpan, pSpanList))
        //                                                pSpanList.Add(pSpan);

        //                                        }
        //                                        else
        //                                        {
        //                                            //This should not happen 
        //                                            //if the conductor has a vertex there should be a pole there                                             
        //                                            Shared.WriteToLogfile("Investigate secondary service for: " + fromPoleGUID + " conductor : " + pConductor.FCName + ":" + pConductor.GUID + " missing pole?");
        //                                        }
        //                                    }
        //                                    else
        //                                    {
        //                                        if (pSeg.Length > Shared.PoleBuffer)
        //                                        {
        //                                            Shared.WriteToLogfile("Investigate pole: " + fromPoleGUID + " conductor : " +
        //                                                pConductor.FCName + ":" + pConductor.GUID +
        //                                                " Seg length: " + pSeg.Length.ToString() + " missing pole?");
        //                                        }
        //                                    }
        //                                }
        //                            }
        //                        }
        //                    }


        //                    if (poleCount % 100 == 0)
        //                    {
        //                        Shared.WriteToLogfile("Processed " + poleCount.ToString() + " poles");
        //                        Debug.Print("Processed " + poleCount.ToString() + " poles");
        //                    }



        //                    //Write the conductor / conductorinfo data
        //                    WriteConductorDeviceInfoToCSV(
        //                        pConductorList,
        //                        pSupportStructFeature,
        //                        fromPoleGUID);

        //                    //Write the SpanList to csv 
        //                    WriteSpanListToCSV(pSpanList);

        //                //} //end of if statement for inside Mother Lode or 715 AL 
        //                pSupportStructFeature = pFCursor.NextFeature();
        //            }
        //            Marshal.FinalReleaseComObject(pFCursor);
        //        }


        //        //Save the Span Information to the SupportStructures 
        //        //SavePoleInformation(hshSpans); 

        //    }
        //    catch (Exception ex)
        //    {
        //        Shared.WriteToLogfile("Entering error handler for " + "CalculatePoleSpans" + ex.Message); 
        //        throw new Exception("Error encountered in CalculatePoleSpans: " + 
        //        ex.Message);
        //    }
        //}

        //private void ExtractForAlasdair()
        //{

        //    try
        //    {
        //        //Open up the poles featureclass 
        //        //Hashtable hshAllAttributes = new Hashtable();                                               
        //        //IFeatureWorkspace pFWS = (IFeatureWorkspace)Shared.GetWorkspaceByName("electric"); 
        //        //IFeatureClass pSupportStructureFC = pFWS.OpenFeatureClass("edgis.supportstructure");

        //        //Get the raster layer in the map 
        //        IMxDocument pMxDoc = (IMxDocument)m_app.Document;
        //        List<ILayer> pLayers = getLayersFromMap(pMxDoc.FocusMap, "LBGIS.ca_dem_30m");
        //        IRasterLayer pRasterLayer = (IRasterLayer)pLayers[0];
        //        IIdentify pIdentify = (IIdentify)pRasterLayer;
        //        int fldIdx = -1;

        //        Shared.InitializeStaticCSVfile(); 

        //        IWorkspaceFactory workspaceFactory = new ESRI.ArcGIS.DataSourcesFile.ShapefileWorkspaceFactoryClass();
        //        IFeatureWorkspace featureWorkspace = (IFeatureWorkspace)workspaceFactory.OpenFromFile("C:\\Simon\\Data\\500PoleShapefile\\AOI_v3_20161025", 0);
        //        IFeatureClass pPoleShapefileFC = featureWorkspace.OpenFeatureClass("Pole_Loading_Pilot_AOI_20161025.shp");

        //        //Loop through the pPoleShapefileFC and write out the attributes 
        //        IFeatureCursor pFCursor = pPoleShapefileFC.Search(null, false);
        //        IFeature pFeature = pFCursor.NextFeature();


        //        while (pFeature != null)
        //        {

        //            fldIdx = pFeature.Fields.FindField("BARCODE");
        //            string barcode = pFeature.get_Value(fldIdx).ToString(); 

        //            fldIdx = pFeature.Fields.FindField("SAPEQUIPID");
        //            string sapId = pFeature.get_Value(fldIdx).ToString();

        //            double poleHeight = GetHeightAtPoint((IPoint)pFeature.ShapeCopy, pIdentify);
        //            int poleheightInt = Convert.ToInt32(Math.Round(poleHeight, 0)); 

        //            Shared.WriteStaticDataToCSV(sapId + "," + barcode  + "," + poleheightInt.ToString());

        //            pFeature = pFCursor.NextFeature();
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        Shared.WriteToLogfile("Entering error handler for " + "CalculatePoleSpans" + ex.Message);
        //        throw new Exception("Error encountered in CalculatePoleSpans: " +
        //        ex.Message);
        //    }
        //}

        //private void CalculatePoleSpans(int poleOID)
        //{

        //    try
        //    {

        //        //Open up the poles featureclass 
        //        Hashtable hshAllAttributes = new Hashtable();
        //        IFeatureWorkspace pFileGDB_FWS = (IFeatureWorkspace)Shared.GetWorkspaceByName("output");
        //        IFeatureWorkspace pFWS = (IFeatureWorkspace)Shared.GetWorkspaceByName("electric");
        //        IFeatureClass pSupportStructureFC = pFWS.OpenFeatureClass("edgis.supportstructure");
        //        IFeatureClass pPriOHConductorFC = pFWS.OpenFeatureClass("edgis.priohconductor");
        //        CacheAttributesForOC(pPriOHConductorFC, ref hshAllAttributes);
        //        ITable pPriOHConductorInfoTbl = pFWS.OpenTable("edgis.priohconductorinfo");
        //        CacheAttributesForOC((IObjectClass)pPriOHConductorInfoTbl, ref hshAllAttributes);
        //        IRelationshipClass pPriOHConductorInfoRC = pFWS.OpenRelationshipClass("EDGIS.PriOHConductor_PriOHConductorInfo");
        //        IFeatureClass pSecOHConductorFC = pFWS.OpenFeatureClass("edgis.secohconductor");
        //        CacheAttributesForOC(pSecOHConductorFC, ref hshAllAttributes);
        //        ITable pSecOHConductorInfoTbl = pFWS.OpenTable("edgis.secohconductorinfo");
        //        CacheAttributesForOC((IObjectClass)pSecOHConductorInfoTbl, ref hshAllAttributes);
        //        IRelationshipClass pSecOHConductorInfoRC = pFWS.OpenRelationshipClass("EDGIS.SecOHConductor_SecOHConductorInfo");
        //        IFeatureClass pTransformerFC = pFWS.OpenFeatureClass("edgis.transformer");
        //        CacheAttributesForOC(pTransformerFC, ref hshAllAttributes);
        //        IRelationshipClass pTransformerRC = pFWS.OpenRelationshipClass("EDGIS.SupportStruct_Transformer");
        //        ITable pTransformerUnitTbl = pFWS.OpenTable("edgis.transformerunit");
        //        CacheAttributesForOC((IObjectClass)pTransformerUnitTbl, ref hshAllAttributes);
        //        IRelationshipClass pTransformerUnitRC = pFWS.OpenRelationshipClass("EDGIS.Transformer_TransformerUnit");
        //        IFeatureClass pAnchorGuyFC = pFWS.OpenFeatureClass("edgis.anchorguy");
        //        CacheAttributesForOC((IObjectClass)pAnchorGuyFC, ref hshAllAttributes);
        //        IRelationshipClass pAnchorGuyRC = pFWS.OpenRelationshipClass("EDGIS.SupportStruct_AnchorGuy");
        //        IFeatureClass pCapBankFC = pFWS.OpenFeatureClass("edgis.capacitorbank");
        //        CacheAttributesForOC((IObjectClass)pCapBankFC, ref hshAllAttributes);
        //        IRelationshipClass pCapBankRC = pFWS.OpenRelationshipClass("EDGIS.SupportStruct_CapacitorBank");
        //        IFeatureClass pDPDFC = pFWS.OpenFeatureClass("edgis.dynamicprotectivedevice");
        //        CacheAttributesForOC((IObjectClass)pDPDFC, ref hshAllAttributes);
        //        IRelationshipClass pDPDRC = pFWS.OpenRelationshipClass("EDGIS.SupportStruct_DynProtDev");
        //        ITable pJointOwnerTbl = pFWS.OpenTable("edgis.jointowner");
        //        CacheAttributesForOC((IObjectClass)pJointOwnerTbl, ref hshAllAttributes);
        //        IRelationshipClass pJointOwnerRC = pFWS.OpenRelationshipClass("EDGIS.SupportStructure_JointOwner");
        //        IFeatureClass pStreetlightFC = pFWS.OpenFeatureClass("edgis.streetlight");
        //        CacheAttributesForOC((IObjectClass)pStreetlightFC, ref hshAllAttributes);
        //        IRelationshipClass pStreetlightRC = pFWS.OpenRelationshipClass("EDGIS.SupportStruct_Streetlight");
        //        IFeatureClass pFuseFC = pFWS.OpenFeatureClass("edgis.Fuse");
        //        CacheAttributesForOC((IObjectClass)pFuseFC, ref hshAllAttributes);
        //        IRelationshipClass pFuseRC = pFWS.OpenRelationshipClass("EDGIS.SupportStruct_Fuse");
        //        IFeatureClass pSwitchFC = pFWS.OpenFeatureClass("edgis.Switch");
        //        CacheAttributesForOC((IObjectClass)pSwitchFC, ref hshAllAttributes);
        //        IRelationshipClass pSwitchRC = pFWS.OpenRelationshipClass("EDGIS.SupportStruct_Switch");
        //        IFeatureClass pVoltageRegulatorFC = pFWS.OpenFeatureClass("edgis.VoltageRegulator");
        //        CacheAttributesForOC((IObjectClass)pVoltageRegulatorFC, ref hshAllAttributes);
        //        IRelationshipClass pVoltageRegulatorRC = pFWS.OpenRelationshipClass("EDGIS.SupportStruct_VoltReg");


        //        IPolygon pClipPolygon = Shared.GetClipPolygon();
        //        IRelationalOperator pRO_MotherLoad = (IRelationalOperator)pClipPolygon;
        //        IQueryFilter pQF = new QueryFilterClass();

        //        IFeatureCursor pFCursor = null;
        //        IFeature pSupportStructFeature = null;


        //        //Get the raster layer in the map 
        //        IMxDocument pMxDoc = (IMxDocument)m_app.Document;
        //        List<ILayer> pLayers = getLayersFromMap(pMxDoc.FocusMap, "Heights");
        //        IRasterLayer pRasterLayer = (IRasterLayer)pLayers[0];
        //        IIdentify pIdentify = (IIdentify)pRasterLayer;
        //        int poleGUIDFldIdx = pSupportStructureFC.Fields.FindField("GLOBALID");
        //        string fromPoleGUID = string.Empty;
        //        string toPoleGUID = string.Empty;
        //        //string replaceGUID = string.Empty;


        //        //Step 1. Get the O/H conductors within configureable tolerance 
        //        //of each pole and put them in a bucket 
        //        IMap pMap = ((IMxDocument)m_app.Document).FocusMap;
        //        IActiveView pAV = (IActiveView)pMap;
        //        List<Conductor> pConductorList = new List<Conductor>();
        //        ITopologicalOperator pTopo = null;
        //        ITopologicalOperator kTopo = null;
        //        IPolygon pSearchPoly = null;
        //        IPolygon pExtentPoly = null;
        //        IRelationalOperator pRO = null;
        //        IProximityOperator pPO = null;
        //        IFeature pAdjacentPoleFeature = null;
        //        Span pSpan = null;
        //        int segCount = -1;
        //        int spanCount = 0;
        //        int poleCount = 0;
        //        double fromPoleRL = -1;
        //        double toPoleRL = -1;
        //        Hashtable hshSpans = new Hashtable();
        //        List<Span> pSpanList = new List<Span>();
        //        WriteSpanCSVHeader(null);
        //        WriteFullDataCSVHeader(null);


        //        Hashtable hsh715ALsPlusMLs = Get715ALsListPlusMLs(pFileGDB_FWS, "SupportStructure_ALL_2", pClipPolygon);

        //        //Create index on SupportStructure feature class
        //        //IFeatureIndex ifxidx = CreateFeatureIndex(pSupportStructureFC);



        //        pQF.WhereClause = "objectid" + " IN(" + commaSeparatedkeyList[j] + ")";
        //        pFCursor = pSupportStructureFC.Search(pQF, false);
        //        pSupportStructFeature = pFCursor.NextFeature();

        //        while (pSupportStructFeature != null)
        //        {

        //            //If the pole is within mother load or the pole is in the list of 
        //            //poles with 715 AL then process the pole 
        //            poleCount++;
        //            //Imran - Code to Capture the pole elevation
        //            IPoint ptPole = (IPoint)pSupportStructFeature.ShapeCopy;
        //            Double poleElevation = GetHeightAtPoint(ptPole, pIdentify);

        //            Debug.Print("pole Height is " + poleElevation + " for :" + pSupportStructFeature.OID);

        //            Debug.Print("========================================");
        //            Debug.Print("Analyzing pole: " + pSupportStructFeature.OID.ToString());

        //            fromPoleGUID = pSupportStructFeature.get_Value(poleGUIDFldIdx).ToString();
        //            toPoleGUID = string.Empty;
        //            spanCount = 0;
        //            pSpanList.Clear();

        //            pTopo = (ITopologicalOperator)pSupportStructFeature.ShapeCopy;
        //            pSearchPoly = (IPolygon)pTopo.Buffer(Shared.PoleBuffer);
        //            pTopo = (ITopologicalOperator)pSupportStructFeature.ShapeCopy;
        //            pExtentPoly = (IPolygon)pTopo.Buffer(300);
        //            pAV.Extent = pExtentPoly.Envelope;
        //            pRO = (IRelationalOperator)pSearchPoly;
        //            pPO = (IProximityOperator)pSupportStructFeature.ShapeCopy;

        //            pConductorList.Clear();

        //            //Get intersecting primary conductors 
        //            GetNearbyConductors(
        //                pPriOHConductorFC,
        //                (Hashtable)hshAllAttributes["PRIOHCONDUCTOR"],
        //                pPriOHConductorInfoRC,
        //                pPriOHConductorInfoTbl,
        //                (Hashtable)hshAllAttributes["PRIOHCONDUCTORINFO"],
        //                pSearchPoly,
        //                pPO,
        //                ref pConductorList);

        //            //Get intersecting secondary conductors
        //            GetNearbyConductors(
        //                pSecOHConductorFC,
        //                (Hashtable)hshAllAttributes["SECOHCONDUCTOR"],
        //                pSecOHConductorInfoRC,
        //                pSecOHConductorInfoTbl,
        //                (Hashtable)hshAllAttributes["SECOHCONDUCTORINFO"],
        //                pSearchPoly,
        //                pPO,
        //                ref pConductorList);

        //            Debug.Print("Found: " + pConductorList.Count.ToString() + " nearby conductors");
        //            foreach (Conductor pConductor in pConductorList)
        //            {
        //                ISegmentCollection pSegColl = (ISegmentCollection)pConductor.Shape;
        //                segCount = pSegColl.SegmentCount;
        //                Debug.Print("Seg Count: " + segCount.ToString());

        //                for (int i = 0; i < pSegColl.SegmentCount; i++)
        //                {
        //                    ISegment pSeg = pSegColl.get_Segment(i);
        //                    Debug.Print("Seg Length: " + pSeg.Length);
        //                    if (pSeg.Length < 3)
        //                    {
        //                        continue;
        //                    }
        //                    if (pRO.Contains(pSeg.FromPoint) ||
        //                    pRO.Contains(pSeg.ToPoint))
        //                    {
        //                        ILine pLine = new LineClass();
        //                        pLine.SpatialReference = pConductor.Shape.SpatialReference;

        //                        //pLine.FromPoint is always the current pole 
        //                        if (pRO.Contains(pSeg.FromPoint))
        //                            pLine.PutCoords(pSeg.FromPoint, pSeg.ToPoint);
        //                        else if (pRO.Contains(pSeg.ToPoint))
        //                            pLine.PutCoords(pSeg.ToPoint, pSeg.FromPoint);
        //                        else
        //                        {
        //                            //Case where there is no vertex where the 
        //                            //pole is located 
        //                            Debug.Print("No vertex found - have to find closest point on line and use this");
        //                        }

        //                        //Extent out the line to a projected point 
        //                        IConstructPoint contructionPoint = new PointClass();
        //                        contructionPoint.ConstructAlong(
        //                            (ICurve)pLine, esriSegmentExtension.esriExtendAtTo, 2000, false);

        //                        //Construct the line for the buffer 
        //                        ILine pLineForBuffer = new LineClass();
        //                        pLineForBuffer.SpatialReference = pConductor.Shape.SpatialReference;
        //                        pLineForBuffer.PutCoords(pLine.FromPoint, (IPoint)contructionPoint);
        //                        IPolyline pPolylineForBuffer = GetPolylineFromLine(pLineForBuffer,
        //                            pConductor.Shape.SpatialReference);

        //                        //Buffer the line to create the corridor
        //                        kTopo = (ITopologicalOperator)pPolylineForBuffer;
        //                        IPolygon pBufferPoly = (IPolygon)kTopo.Buffer(Shared.PoleBuffer);
        //                        //IGeometry pGeomPoly = kTopo.Buffer(Shared.PoleBuffer);

        //                        //Search for adjacent poles within the ray buffer and 
        //                        //find the closest one in each direction and this will be 
        //                        //the adjacent pole 

        //                        pAdjacentPoleFeature = FindAdjacentPole(
        //                            (IPoint)pSupportStructFeature.ShapeCopy,
        //                            pBufferPoly,
        //                            pSupportStructureFC,
        //                            pSupportStructFeature.OID,
        //                            pConductor.CircId//,
        //                                             //ifxidx,
        //                                             // pGeomPoly
        //                           );


        //                        if (pAdjacentPoleFeature != null)
        //                        {
        //                            spanCount++;

        //                            toPoleGUID = pAdjacentPoleFeature.get_Value(poleGUIDFldIdx).ToString();
        //                            fromPoleRL = GetHeightAtPoint(pLine.FromPoint, pIdentify);
        //                            toPoleRL = GetHeightAtPoint(pLine.ToPoint, pIdentify);
        //                            pSpan = new Span(
        //                                spanCount,
        //                                GetSpanAngleDegrees(pLine.Angle),
        //                                pPO.ReturnDistance(pAdjacentPoleFeature.ShapeCopy),
        //                                pConductor.FCName.ToUpper(),
        //                                pConductor.Subtype,
        //                                pConductor.GUID,
        //                                fromPoleGUID,
        //                                fromPoleRL,
        //                                toPoleGUID,
        //                                toPoleRL,
        //                                pConductor.CircId,
        //                                poleElevation);
        //                            if (!SpanIsDuplicate(pSpan, pSpanList))
        //                                pSpanList.Add(pSpan);

        //                        }
        //                        else
        //                        {
        //                            //Check if there is a SL at the other end of the line 
        //                            //If this is suptype 3 or subtype 5 this is a service span
        //                            if (((pConductor.Subtype == "Service Overhead Conductor") /*|| (pConductor.Subtype == "Streetlight Overhead Conductor") 
        //                                        || (pConductor.Subtype == "Pseudo Service")*/) && (pConductor.FCName.ToUpper() == "SECOHCONDUCTOR")) //AND SECONARY 
        //                            {
        //                                //This is a service span probably going to the servicelocation
        //                                if (segCount == 1)
        //                                {
        //                                    spanCount++;
        //                                    IPoint pSL = pLine.ToPoint;
        //                                    pSL.SpatialReference = pConductor.Shape.SpatialReference;
        //                                    fromPoleRL = GetHeightAtPoint(pLine.FromPoint, pIdentify);
        //                                    toPoleGUID = string.Empty;
        //                                    toPoleRL = GetHeightAtPoint(pLine.ToPoint, pIdentify);

        //                                    pSpan = new Span(
        //                                        spanCount,
        //                                        GetSpanAngleDegrees(pLine.Angle),
        //                                        pPO.ReturnDistance(pSL),
        //                                        pConductor.FCName.ToUpper(),
        //                                        pConductor.Subtype,
        //                                        pConductor.GUID,
        //                                        fromPoleGUID,
        //                                        fromPoleRL,
        //                                        toPoleGUID,
        //                                        toPoleRL,
        //                                        pConductor.CircId,
        //                                        poleElevation);
        //                                    if (!SpanIsDuplicate(pSpan, pSpanList))
        //                                        pSpanList.Add(pSpan);

        //                                }
        //                                else
        //                                {
        //                                    //This should not happen 
        //                                    //if the conductor has a vertex there should be a pole there                                             
        //                                    Shared.WriteToLogfile("Investigate secondary service for: " + fromPoleGUID + " conductor : " + pConductor.FCName + ":" + pConductor.GUID + " missing pole?");
        //                                }
        //                            }
        //                            else
        //                            {
        //                                if (pSeg.Length > Shared.PoleBuffer)
        //                                {
        //                                    Shared.WriteToLogfile("Investigate pole: " + fromPoleGUID + " conductor : " +
        //                                        pConductor.FCName + ":" + pConductor.GUID +
        //                                        " Seg length: " + pSeg.Length.ToString() + " missing pole?");
        //                                }
        //                            }
        //                        }
        //                    }
        //                }
        //            }


        //            if (poleCount % 100 == 0)
        //            {
        //                Shared.WriteToLogfile("Processed " + poleCount.ToString() + " poles");
        //                Debug.Print("Processed " + poleCount.ToString() + " poles");
        //            }



        //            //Write the conductor / conductorinfo data
        //            WriteConductorDeviceInfoToCSV(
        //                pConductorList,
        //                pSupportStructFeature,
        //                fromPoleGUID);

        //            //Write the SpanList to csv 
        //            WriteSpanListToCSV(pSpanList);





        //            //Save the Span Information to the SupportStructures 
        //            //SavePoleInformation(hshSpans); 

        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Shared.WriteToLogfile("Entering error handler for " + "CalculatePoleSpans" + ex.Message);
        //        throw new Exception("Error encountered in CalculatePoleSpans: " +
        //        ex.Message);
        //    }
        //}
            
        /// <summary>
        /// Buffers the passed geometry by the passed buffer distance to 
        /// return an IPolygon 
        /// </summary>
        private static IPolygon GetBufferPolygon(IGeometry pGeometry, double bufferDist)
        {
            try
            {

                ITopologicalOperator pTopo = (ITopologicalOperator)pGeometry;
                IPolygon pSearchPoly = (IPolygon)pTopo.Buffer(bufferDist);

                ITopologicalOperator2 pTopo2 = (ITopologicalOperator2)pSearchPoly;
                pTopo2.IsKnownSimple_2 = false;
                pTopo2.Simplify();
                return pSearchPoly;

            }
            catch (Exception ex)
            {
                Shared.WriteToLogfile("Error in " + MethodBase.GetCurrentMethod() +
                    " details: " + ex.Message);
                throw new Exception("Error generating buffer polygon");
            }
        }
        /// <summary>
        /// Writes out the header row for the CSV containing 
        /// static attributes of supportstructure derived from 
        /// the GIS 
        /// </summary>
        private void WriteStaticDataCSVHeader()
        {
            try
            {
                Shared.WriteToLogfile("Entering " + MethodBase.GetCurrentMethod());

                //Write the header 
                //TODO *** change this it needs to be the alias not the 
                //field name 

                StringBuilder sb = new StringBuilder();
                foreach (string fieldName in ((Hashtable)_hshAllAttributes["SUPPORTSTRUCTURE"]).Keys)
                {
                    int fldIdx = m_SupportStructureFC.Fields.FindField(fieldName);
                    string fieldAlias = m_SupportStructureFC.Fields.
                        Field[fldIdx].AliasName;
                    if (sb.ToString() == string.Empty)
                        sb.Append(fieldAlias);
                    else
                    {
                        sb.Append(",");
                        sb.Append(fieldAlias);
                    }
                }

                sb.Append(",");
                sb.Append("SnowLoading");
                sb.Append(",");
                sb.Append("Density");
                sb.Append(",");
                sb.Append("ClimateZone");
                sb.Append(",");
                sb.Append("CorrosionZone");
                sb.Append(",");
                sb.Append("RaptorConcentrationZone");


                Shared.WriteStaticDataToCSV(sb.ToString());
            }
            catch (Exception ex)
            {
                Shared.WriteToLogfile("Error in " + MethodBase.GetCurrentMethod() +
                    " details: " + ex.Message);
                throw new Exception("Error generating static data csv header");
            }
        }

        private void WriteStaticData(IFeature pRefPoleFeature)
        {
            try
            {
                Shared.WriteToLogfile("Entering " + MethodBase.GetCurrentMethod());
                //Open up the poles featureclass 
                Hashtable hshAllAttributes = new Hashtable();
                //IFeatureWorkspace pFileGDB_FWS = (IFeatureWorkspace)Shared.GetWorkspaceByName("output");
                //IFeatureWorkspace pFWS = (IFeatureWorkspace)Shared.GetWorkspaceByName("electric");                
                //IFeatureClass m_SupportStructureFC = pFWS.OpenFeatureClass("edgis.supportstructure");
                //IPolygon pClipPolygon = Shared.GetClipPolygon();
                //IRelationalOperator pRO_MotherLoad = (IRelationalOperator)pClipPolygon;
                //IQueryFilter pQF = new QueryFilterClass();
                //IFeatureCursor pFCursor = null;
                //IFeature pSupportStructFeature = null;

                //Get the raster layer in the map 
                //int globalIdDFldIdx = m_SupportStructureFC.Fields.FindField("GLOBALID");
                //int sapEqpIdDFldIdx = m_SupportStructureFC.Fields.FindField("SAPEQUIPID");
                object theValueObj = null;
                string theDomainDescription = string.Empty;
                string theValue = string.Empty;


                Hashtable hshDomain = null;

                //Hashtable hshAllDomains = new Hashtable(); 
                //for (int i = 0; i < m_SupportStructureFC.Fields.FieldCount; i++)
                //{
                //    if (m_SupportStructureFC.Fields.get_Field(i).Domain != null)
                //    {
                //        if (m_SupportStructureFC.Fields.get_Field(i).Domain is ICodedValueDomain)
                //        {
                //            hshDomain = new Hashtable();
                //            ICodedValueDomain pCodedValueDomain = (ICodedValueDomain)m_SupportStructureFC.Fields.get_Field(i).Domain;
                //            int codeCount = pCodedValueDomain.CodeCount;
                //            //Loop through the list of values and print their names
                //            for (int k = 0; k < codeCount; k++)
                //            {
                //                if (!hshDomain.ContainsKey(pCodedValueDomain.get_Value(k)))
                //                    hshDomain.Add(pCodedValueDomain.get_Value(k), pCodedValueDomain.get_Name(k));
                //            }
                //            //Add the domain using the field name as the key 
                //            hshAllDomains.Add(m_SupportStructureFC.Fields.get_Field(i).Name, hshDomain); 
                //        }
                //    }
                //}

                string guid = string.Empty;
                string sapEquipId = string.Empty;
                int poleCount = 0;

                //Hashtable hshFieldsToInclude = new Hashtable();
                //hshFieldsToInclude.Add("SAPEQUIPID", m_SupportStructureFC.Fields.get_Field(m_SupportStructureFC.Fields.FindField("SAPEQUIPID")).AliasName);
                //hshFieldsToInclude.Add("POLETYPE", m_SupportStructureFC.Fields.get_Field(m_SupportStructureFC.Fields.FindField("POLETYPE")).AliasName);
                //hshFieldsToInclude.Add("MATERIAL", m_SupportStructureFC.Fields.get_Field(m_SupportStructureFC.Fields.FindField("MATERIAL")).AliasName);
                //hshFieldsToInclude.Add("HEIGHT", m_SupportStructureFC.Fields.get_Field(m_SupportStructureFC.Fields.FindField("HEIGHT")).AliasName);
                //hshFieldsToInclude.Add("CLASS", m_SupportStructureFC.Fields.get_Field(m_SupportStructureFC.Fields.FindField("CLASS")).AliasName);
                //hshFieldsToInclude.Add("SPECIES", m_SupportStructureFC.Fields.get_Field(m_SupportStructureFC.Fields.FindField("SPECIES")).AliasName);
                //hshFieldsToInclude.Add("FOREIGNATTACHIDC", m_SupportStructureFC.Fields.get_Field(m_SupportStructureFC.Fields.FindField("FOREIGNATTACHIDC")).AliasName);
                //hshFieldsToInclude.Add("JOINTCOUNT", m_SupportStructureFC.Fields.get_Field(m_SupportStructureFC.Fields.FindField("JOINTCOUNT")).AliasName);
                //hshFieldsToInclude.Add("INSTALLJOBYEAR", m_SupportStructureFC.Fields.get_Field(m_SupportStructureFC.Fields.FindField("INSTALLJOBYEAR")).AliasName);
                //hshFieldsToInclude.Add("INSTALLATIONDATE", m_SupportStructureFC.Fields.get_Field(m_SupportStructureFC.Fields.FindField("INSTALLATIONDATE")).AliasName);
                //hshFieldsToInclude.Add("POLECOUNT", m_SupportStructureFC.Fields.get_Field(m_SupportStructureFC.Fields.FindField("POLECOUNT")).AliasName);
                //hshFieldsToInclude.Add("SUBTYPECD", m_SupportStructureFC.Fields.get_Field(m_SupportStructureFC.Fields.FindField("SUBTYPECD")).AliasName);
                //hshFieldsToInclude.Add("STATUS", m_SupportStructureFC.Fields.get_Field(m_SupportStructureFC.Fields.FindField("STATUS")).AliasName);
                //hshFieldsToInclude.Add("POLEUSE", m_SupportStructureFC.Fields.get_Field(m_SupportStructureFC.Fields.FindField("POLEUSE")).AliasName);
                //hshFieldsToInclude.Add("ORIGINALCIRCUMFERENCE", m_SupportStructureFC.Fields.get_Field(m_SupportStructureFC.Fields.FindField("ORIGINALCIRCUMFERENCE")).AliasName);
                //hshFieldsToInclude.Add("EXISTINGREINFORCEMENT", m_SupportStructureFC.Fields.get_Field(m_SupportStructureFC.Fields.FindField("EXISTINGREINFORCEMENT")).AliasName);
                //hshFieldsToInclude.Add("POLETOPEXTIDC", m_SupportStructureFC.Fields.get_Field(m_SupportStructureFC.Fields.FindField("POLETOPEXTIDC")).AliasName);
                //hshFieldsToInclude.Add("MANUFACTUREDYEAR", m_SupportStructureFC.Fields.get_Field(m_SupportStructureFC.Fields.FindField("MANUFACTUREDYEAR")).AliasName);
                //hshFieldsToInclude.Add("GLOBALID", m_SupportStructureFC.Fields.get_Field(m_SupportStructureFC.Fields.FindField("GLOBALID")).AliasName);
                //hshFieldsToInclude.Add("CUSTOMEROWNED", m_SupportStructureFC.Fields.get_Field(m_SupportStructureFC.Fields.FindField("CUSTOMEROWNED")).AliasName);
                //hshFieldsToInclude.Add("BARCODE", m_SupportStructureFC.Fields.get_Field(m_SupportStructureFC.Fields.FindField("BARCODE")).AliasName);
                //hshFieldsToInclude.Add("CEDSAID", m_SupportStructureFC.Fields.get_Field(m_SupportStructureFC.Fields.FindField("CEDSAID")).AliasName);
                //hshFieldsToInclude.Add("COMMENTS", m_SupportStructureFC.Fields.get_Field(m_SupportStructureFC.Fields.FindField("COMMENTS")).AliasName);
                //hshFieldsToInclude.Add("MAXVOLTAGELEVEL", m_SupportStructureFC.Fields.get_Field(m_SupportStructureFC.Fields.FindField("MAXVOLTAGELEVEL")).AliasName);
                //hshFieldsToInclude.Add("DISTMAP", m_SupportStructureFC.Fields.get_Field(m_SupportStructureFC.Fields.FindField("DISTMAP")).AliasName);
                //hshFieldsToInclude.Add("GPSLATITUDE", m_SupportStructureFC.Fields.get_Field(m_SupportStructureFC.Fields.FindField("GPSLATITUDE")).AliasName);
                //hshFieldsToInclude.Add("GPSLONGITUDE", m_SupportStructureFC.Fields.get_Field(m_SupportStructureFC.Fields.FindField("GPSLONGITUDE")).AliasName);
                //hshFieldsToInclude.Add("LOCDESC1", m_SupportStructureFC.Fields.get_Field(m_SupportStructureFC.Fields.FindField("LOCDESC1")).AliasName);
                //hshFieldsToInclude.Add("FUNCTIONALLOCATIONID", m_SupportStructureFC.Fields.get_Field(m_SupportStructureFC.Fields.FindField("FUNCTIONALLOCATIONID")).AliasName);
                //hshFieldsToInclude.Add("POLENUMBER", m_SupportStructureFC.Fields.get_Field(m_SupportStructureFC.Fields.FindField("POLENUMBER")).AliasName);
                //hshFieldsToInclude.Add("REPLACEGUID", m_SupportStructureFC.Fields.get_Field(m_SupportStructureFC.Fields.FindField("REPLACEGUID")).AliasName);

                //Hashtable hshFieldIndexes = new Hashtable();
                //foreach (string fld in hshFieldsToInclude.Keys)
                //{
                //    hshFieldIndexes.Add(fld, m_SupportStructureFC.Fields.FindField(fld)); 
                //}

                //Write the header 
                StringBuilder sb = new StringBuilder();
                //foreach (string alias in ((Hashtable)_hshAllAttributes["SUPPORTSTRUCTURE"]).Keys)
                //{
                //    if (sb.ToString() == string.Empty)
                //        sb.Append(alias); 
                //    else
                //    {
                //        sb.Append(",");
                //        sb.Append(alias); 
                //    }
                //}

                //sb.Append(",");
                //sb.Append("SnowLoading");
                //sb.Append(",");
                //sb.Append("Density");
                //sb.Append(",");
                //sb.Append("ClimateZone");
                //sb.Append(",");
                //sb.Append("CorrosionZone");
                //sb.Append(",");
                //sb.Append("RaptorConcentrationZone");


                //Shared.WriteStaticDataToCSV(sb.ToString());
                string datasetName = ((IDataset)m_SupportStructureFC).Name.ToUpper();
                //datasetName = GetShortDatasetName(datasetName); 

                if (pRefPoleFeature != null)
                {

                    //If the pole is within mother load or the pole is in the list of 
                    //poles with 715 AL then process the pole 
                    //poleCount++;
                    sb.Clear();

                    foreach (string field in ((Hashtable)_hshAllAttributes["SUPPORTSTRUCTURE"]).Keys)
                    {

                        if (sb.ToString() != string.Empty)
                        {
                            sb.Append(",");
                        }

                        int fldIdx = (int)((Hashtable)_hshAllAttributes["SUPPORTSTRUCTURE"])[field];
                        if (pRefPoleFeature.get_Value(fldIdx) != DBNull.Value)
                        {
                            theValue = GetFieldValue(pRefPoleFeature, fldIdx);
                            sb.Append(theValue);
                        }
                        else
                        {
                            sb.Append("<NULL>");
                        }

                        //if (_hshAllDomains.ContainsKey(datasetName + "." + field))  
                        //{
                        //    int fldIdx = (int)((Hashtable)_hshAllAttributes["SUPPORTSTRUCTURE"])[field];
                        //    theValueObj = pRefPoleFeature.get_Value(fldIdx);
                        //    hshDomain = (Hashtable)_hshAllDomains[datasetName + "." + field];

                        //    if (hshDomain.ContainsKey(theValueObj)) 
                        //    {
                        //        theDomainDescription = hshDomain[theValueObj].ToString();
                        //        sb.Append(theDomainDescription);                                    
                        //    }
                        //    else
                        //    {
                        //        //Just write the value 
                        //        if (pRefPoleFeature.get_Value(fldIdx) != DBNull.Value)
                        //            sb.Append(theValueObj.ToString());
                        //        else
                        //            sb.Append("<NULL>"); 
                        //    }                                
                        //}
                        //else 
                        //{
                        //int fldIdx = (int)((Hashtable)_hshAllAttributes["SUPPORTSTRUCTURE"])[field]; 
                        //if (pRefPoleFeature.get_Value(fldIdx) != DBNull.Value)
                        //    {
                        //        theValue = pRefPoleFeature.get_Value(fldIdx).ToString();
                        //        sb.Append(theValue);
                        //    }
                        //    else
                        //    {
                        //        sb.Append("<NULL>");
                        //    }
                        //}
                    }

                    //Logic for writting SnowLoading and Urban/Rural data in STATIS_DATA.csv
                    //Logic inplemented by Tina

                    //Snow Loading 
                    sb.Append(",");
                    sb.Append(GetPointInPolygonLandbaseValue(
                                (IPoint)pRefPoleFeature.ShapeCopy,
                                m_SnowLoadingFC,
                                "SNOWLOAD",
                                "Light"));

                    //Urban Rural Indicator (Density) 
                    sb.Append(",");
                    sb.Append(GetPointInPolygonLandbaseValue(
                                (IPoint)pRefPoleFeature.ShapeCopy,
                                m_PopDensityFC,
                                "DENSITY",
                                "MEDIUM (60-1000 per sqmi)"));

                    //Climate zone 
                    sb.Append(",");
                    sb.Append(GetPointInPolygonLandbaseValue(
                                (IPoint)pRefPoleFeature.ShapeCopy,
                                m_ClimateZoneFC,
                                "CZ_CODE",
                                "Outside Climate Zone"));

                    //Corrosion zone 
                    sb.Append(",");
                    sb.Append(GetPointInPolygonLandbaseValue(
                                (IPoint)pRefPoleFeature.ShapeCopy,
                                m_CorrosionAreaFC,
                                "CORROSION",
                                "Outside Corrosion Area"));

                    //Raptor Concentration Zone
                    sb.Append(",");
                    sb.Append(GetPointInPolygonLandbaseValue(
                                (IPoint)pRefPoleFeature.ShapeCopy,
                                m_RCZ_FC,
                                "ZONE",
                                "Outside Raptor Concentration Zone"));

                    //Write the SpanList to csv 
                    sb.Append(",");
                    Shared.WriteStaticDataToCSV(sb.ToString());

                    //} //end of if statement for inside Mother Lode or 715 AL 
                    //pSupportStructFeature = pFCursor.NextFeature();
                }


            }
            catch (Exception ex)
            {
                Shared.WriteToLogfile("Error in " + MethodBase.GetCurrentMethod() +
                    " details: " + ex.Message);
                throw ex;
            }
        }

        //private void WriteStaticDataToDB_old(IFeature pRefPoleFeature)
        //{
        //    try
        //    {
        //        Shared.WriteToLogfile("Entering " + MethodBase.GetCurrentMethod());
        //        string landbaseValue = string.Empty; 
        //        Hashtable hshPoleFields = (Hashtable)_hshAllAttributes["SUPPORTSTRUCTURE"]; 

        //        if (_pConn == null)
        //        {
        //            //Get output DB connection 
        //            _pConn = new SqlConnection(
        //                Shared.OutputConnectString);
        //        }
        //        if (_pConn.State != System.Data.ConnectionState.Open)
        //            _pConn.Open();

        //        //String to hold the SQL insert statement 
        //        StringBuilder sqlStatement = new StringBuilder();
        //        StringBuilder fieldList = new StringBuilder();
        //        StringBuilder valuesList = new StringBuilder(); 
        //        SqlCommand pCmd = null;
        //        int fldIdx = -1; 

        //        if (pRefPoleFeature != null)
        //        {
        //            //Calculate the fields and values 
        //            foreach (string fldName in hshPoleFields.Keys)
        //            {
        //                fldIdx = Convert.ToInt32(hshPoleFields[fldName]); 
        //                if (fieldList.ToString() != string.Empty)
        //                {
        //                    fieldList.Append(",");
        //                    valuesList.Append(",");
        //                }
        //                fieldList.Append(fldName);

        //                if (pRefPoleFeature.get_Value(fldIdx) != DBNull.Value)
        //                    valuesList.Append("'" + GetFieldValue(pRefPoleFeature,
        //                        fldIdx) + "'");
        //                else
        //                    valuesList.Append("'" + "<NULL>" + "'");

        //            }

        //            //Landbase fields 
        //            fieldList.Append(",");
        //            fieldList.Append("SNOWLOADING");
        //            fieldList.Append(",");
        //            fieldList.Append("DENSITY");
        //            fieldList.Append(",");
        //            fieldList.Append("CLIMATEZONE");
        //            fieldList.Append(",");
        //            fieldList.Append("CORROSIONZONE");
        //            fieldList.Append(",");
        //            fieldList.Append("RAPTORCONCZONE");

        //            //Landbase values 
        //            //Snow Loading 
        //            valuesList.Append(",");
        //            landbaseValue = GetPointInPolygonLandbaseValue(
        //                        (IPoint)pRefPoleFeature.ShapeCopy,
        //                        m_SnowLoadingFC,
        //                        "SNOWLOAD",
        //                        "Light"); 
        //            valuesList.Append("'" + landbaseValue + "'");

        //            //Urban Rural Indicator (Density) 
        //            valuesList.Append(",");
        //            landbaseValue = GetPointInPolygonLandbaseValue(
        //                        (IPoint)pRefPoleFeature.ShapeCopy,
        //                        m_PopDensityFC,
        //                        "DENSITY",
        //                        "MEDIUM (60-1000 per sqmi)");
        //            valuesList.Append("'" + landbaseValue + "'");

        //            //Climate zone 
        //            valuesList.Append(",");
        //            landbaseValue = GetPointInPolygonLandbaseValue(
        //                        (IPoint)pRefPoleFeature.ShapeCopy,
        //                        m_ClimateZoneFC,
        //                        "CZ_CODE",
        //                        "Outside Climate Zone"); 
        //            valuesList.Append("'" + landbaseValue + "'");

        //            //Corrosion zone 
        //            valuesList.Append(",");
        //            landbaseValue = GetPointInPolygonLandbaseValue(
        //                        (IPoint)pRefPoleFeature.ShapeCopy,
        //                        m_CorrosionAreaFC,
        //                        "CORROSION",
        //                        "Outside Corrosion Area");
        //            valuesList.Append("'" + landbaseValue + "'");

        //            //Raptor Concentration Zone
        //            valuesList.Append(",");
        //            landbaseValue = GetPointInPolygonLandbaseValue(
        //                        (IPoint)pRefPoleFeature.ShapeCopy,
        //                        m_RCZ_FC,
        //                        "ZONE",
        //                        "Outside Raptor Concentration Zone");
        //            valuesList.Append("'" + landbaseValue + "'");

        //            //Write the insert statement 
        //            sqlStatement.Clear();
        //            sqlStatement.Append("INSERT INTO " + PoleLoadConstants.STATIC_DATA_TBL + " ");
        //            sqlStatement.Append(  "(");
        //            sqlStatement.Append(      fieldList.ToString());
        //            sqlStatement.Append(  ") ");
        //            sqlStatement.Append("VALUES ");
        //            sqlStatement.Append(  "(");
        //            sqlStatement.Append(      valuesList.ToString());
        //            sqlStatement.Append(  ")");
        //        }

        //        //Execute the command against the database 
        //        pCmd = _pConn.CreateCommand();
        //        pCmd.Connection = _pConn;
        //        pCmd.CommandText = sqlStatement.ToString();
        //        int recordsUpdated = pCmd.ExecuteNonQuery();

        //        if (recordsUpdated == 0)
        //            throw new Exception("SQL insert failed for static data"); 

        //    }
        //    catch (Exception ex)
        //    {
        //        Shared.WriteToLogfile("Error in " + MethodBase.GetCurrentMethod() +
        //            " details: " + ex.Message);
        //        throw ex;
        //    }
        //}
        /// <summary>
        /// Use the new format 
        /// </summary>
        /// <param name="pRefPoleFeature"></param>
        private void WritePoleToDB( 
            IFeature pRefPoleFeature, 
            double refPoleRL, 
            string snowLoadingZone, 
            string popDensity)
        {
            try
            {
                Shared.WriteToLogfile("Entering " + MethodBase.GetCurrentMethod());
                string landbaseValue = string.Empty;
                Hashtable hshPoleFields = (Hashtable)_hshAllAttributes["SUPPORTSTRUCTURE"];

                if (_pConn == null)
                {
                    //Get output DB connection 
                    _pConn = new SqlConnection(
                        Shared.OutputConnectString);
                }
                if (_pConn.State != System.Data.ConnectionState.Open)
                    _pConn.Open();

                //String to hold the SQL insert statement 
                string sqlStatement = string.Empty;
                StringBuilder fieldList = new StringBuilder();
                StringBuilder valuesList = new StringBuilder();
                
                SqlCommand pCmd = null;
                double latitude = 0;
                double longitude = 0;
                string zoneValue = string.Empty; 
                int jointCount = 0;
                string jointCountString = string.Empty; 
                bool jointOwned = false;  
                int recordsUpdated = 0; 
                int fldIdx = -1;

                if (pRefPoleFeature != null)
                {
                    //POLE fields to include 
                    fieldList.Clear();
                    valuesList.Clear(); 
                    fieldList.Append("POLEGUID");
                    fieldList.Append(",");
                    fieldList.Append("SAPEQUIPID");
                    fieldList.Append(",");
                    fieldList.Append("LATITUDE");
                    fieldList.Append(",");
                    fieldList.Append("LONGITUDE");
                    fieldList.Append(",");
                    fieldList.Append("ELEVATION");
                    fieldList.Append(",");
                    fieldList.Append("HEIGHT");
                    fieldList.Append(",");
                    fieldList.Append("CLASS");
                    fieldList.Append(",");
                    fieldList.Append("SPECIES");
                    fieldList.Append(",");
                    fieldList.Append("POP_DENSITY");
                    fieldList.Append(",");
                    fieldList.Append("SNOW_LOADING_ZONE");
                    fieldList.Append(",");
                    fieldList.Append("CLIMATE_ZONE");
                    fieldList.Append(",");
                    fieldList.Append("CORROSION_ZONE");
                    fieldList.Append(",");
                    fieldList.Append("RCZ_ZONE");
                    fieldList.Append(",");
                    fieldList.Append("SURGE_PROT_ZONE");
                    fieldList.Append(",");
                    fieldList.Append("INSULATION_ZONE");
                    fieldList.Append(",");
                    fieldList.Append("UWF_ZONE");
                    fieldList.Append(",");
                    fieldList.Append("PEAK_WIND_ZONE");
                    fieldList.Append(",");
                    fieldList.Append("SUBTYPE");
                    fieldList.Append(",");
                    fieldList.Append("POLETYPE");
                    fieldList.Append(",");
                    fieldList.Append("POLEUSE");
                    fieldList.Append(",");
                    fieldList.Append("JOINT_OWNED");

                    //POLEGUID 
                    fldIdx = Convert.ToInt32(hshPoleFields["GLOBALID"]);
                    valuesList.Append("'" + GetFieldValue(pRefPoleFeature, 
                        fldIdx) + "'");
                    valuesList.Append(",");

                    //SAPEQUIPID 
                    fldIdx = Convert.ToInt32(hshPoleFields["SAPEQUIPID"]); 
                    if (pRefPoleFeature.get_Value(fldIdx) != DBNull.Value)
                        valuesList.Append("'" + GetFieldValue(pRefPoleFeature,
                            fldIdx) + "'");
                    else
                        valuesList.Append("'" + "<NULL>" + "'");
                    valuesList.Append(",");

                    //Default lat and long 
                    latitude = 0;
                    longitude = 0;

                    //LATITUDE                     
                    //fldIdx = Convert.ToInt32(hshPoleFields["GPSLATITUDE"]);
                    //if (pRefPoleFeature.get_Value(fldIdx) != DBNull.Value)
                    //    latitude = Convert.ToDouble(pRefPoleFeature.get_Value(fldIdx));

                    ////LONGITUDE                      
                    //fldIdx = Convert.ToInt32(hshPoleFields["GPSLONGITUDE"]);
                    //if (pRefPoleFeature.get_Value(fldIdx) != DBNull.Value)
                    //    longitude = Convert.ToDouble(pRefPoleFeature.get_Value(fldIdx));

                    //if ((latitude == 0) || (longitude == 0))
                    //{
                    //    PopulateLatLongForPole(
                    //        (IPoint)pRefPoleFeature.ShapeCopy, ref latitude, ref longitude);
                    //}

                    PopulateLatLongForPole(
                            (IPoint)pRefPoleFeature.ShapeCopy, ref latitude, ref longitude);

                    //LATITUDE 
                    valuesList.Append("'" + latitude.ToString() + "'");
                    valuesList.Append(",");

                    //LONGITUDE  
                    valuesList.Append("'" + longitude.ToString() + "'");
                    valuesList.Append(",");

                    //ELEVATION 
                    valuesList.Append("'" + refPoleRL.ToString() + "'");
                    valuesList.Append(",");

                    //HEIGHT                      
                    fldIdx = Convert.ToInt32(hshPoleFields["HEIGHT"]);
                    if (pRefPoleFeature.get_Value(fldIdx) != DBNull.Value)
                        valuesList.Append("'" + GetFieldValue(pRefPoleFeature,
                            fldIdx) + "'");
                    else
                        valuesList.Append("'" + "<NULL>" + "'");
                    valuesList.Append(",");

                    //CLASS                       
                    fldIdx = Convert.ToInt32(hshPoleFields["CLASS"]);
                    if (pRefPoleFeature.get_Value(fldIdx) != DBNull.Value)
                        valuesList.Append("'" + GetFieldValue(pRefPoleFeature,
                            fldIdx) + "'");
                    else
                        valuesList.Append("'" + "<NULL>" + "'");
                    valuesList.Append(","); 
                    
                    //SPECIES                        
                    fldIdx = Convert.ToInt32(hshPoleFields["SPECIES"]);
                    if (pRefPoleFeature.get_Value(fldIdx) != DBNull.Value)
                        valuesList.Append("'" + GetFieldValue(pRefPoleFeature,
                            fldIdx) + "'");
                    else
                        valuesList.Append("'" + "<NULL>" + "'");
                    valuesList.Append(",");

                    //POP_DENSITY 
                    valuesList.Append("'" + popDensity + "'");
                    valuesList.Append(",");

                    //SNOW_LOADING_ZONE
                    valuesList.Append("'" + snowLoadingZone + "'");
                    valuesList.Append(",");

                    //CLIMATE_ZONE
                    zoneValue = GetPointInPolygonLandbaseValue(
                                (IPoint)pRefPoleFeature.ShapeCopy,
                                m_ClimateZoneFC,
                                "CZ_CODE",
                                "Outside Climate Zone"); 
                    valuesList.Append("'" + zoneValue + "'");
                    valuesList.Append(",");

                    //CORROSION_ZONE 
                    zoneValue = GetPointInPolygonLandbaseValue(
                                (IPoint)pRefPoleFeature.ShapeCopy,
                                m_CorrosionAreaFC,
                                "CORROSION",
                                "Outside Corrosion Area"); 
                    valuesList.Append("'" + zoneValue + "'");
                    valuesList.Append(",");

                    //RCZ_ZONE
                    zoneValue = GetPointInPolygonLandbaseValue(
                                (IPoint)pRefPoleFeature.ShapeCopy,
                                m_RCZ_FC,
                                "ZONE",
                                "Outside Raptor Concentration Zone"); 
                    valuesList.Append("'" + zoneValue + "'");
                    valuesList.Append(",");

                    //SURGE_PROT_ZONE
                    zoneValue = GetPointInPolygonLandbaseValue(
                                (IPoint)pRefPoleFeature.ShapeCopy,
                                m_SurgeProtFC,
                                "DISTRICT_DEF",
                                "Outside Surge Protection Zone"); 
                    valuesList.Append("'" + zoneValue + "'");
                    valuesList.Append(",");

                    //INSULATION_ZONE
                    zoneValue = GetPointInPolygonLandbaseValue(
                                (IPoint)pRefPoleFeature.ShapeCopy,
                                m_InsulationFC,
                                "CODE",
                                "Outside Insulation Zone"); 
                    valuesList.Append("'" + zoneValue + "'");
                    valuesList.Append(",");

                    //UWF_ZONE
                    zoneValue = GetPointInPolygonLandbaseValue(
                                (IPoint)pRefPoleFeature.ShapeCopy,
                                m_UWF_FC,
                                "CATEGORY",
                                "Outside UWF Zones"); 
                    valuesList.Append("'" + zoneValue + "'");
                    valuesList.Append(",");

                    //PEAK_WIND_ZONE
                    zoneValue = GetPointInPolygonLandbaseValue(
                                (IPoint)pRefPoleFeature.ShapeCopy,
                                m_PeakWindFC,
                                "DESCRIPTION",
                                "Outside Peak Wind Zones"); 
                    valuesList.Append("'" + zoneValue + "'");
                    valuesList.Append(",");

                    //SUBTYPE                        
                    fldIdx = Convert.ToInt32(hshPoleFields["SUBTYPECD"]);
                    if (pRefPoleFeature.get_Value(fldIdx) != DBNull.Value)
                        valuesList.Append("'" + GetFieldValue(pRefPoleFeature,
                            fldIdx) + "'");
                    else
                        valuesList.Append("'" + "<NULL>" + "'");
                    valuesList.Append(",");

                    //POLETYPE                        
                    fldIdx = Convert.ToInt32(hshPoleFields["POLETYPE"]);
                    if (pRefPoleFeature.get_Value(fldIdx) != DBNull.Value)
                        valuesList.Append("'" + GetFieldValue(pRefPoleFeature,
                            fldIdx) + "'");
                    else
                        valuesList.Append("'" + "<NULL>" + "'");
                    valuesList.Append(",");

                    //POLEUSE                        
                    fldIdx = Convert.ToInt32(hshPoleFields["POLEUSE"]);
                    if (pRefPoleFeature.get_Value(fldIdx) != DBNull.Value)
                        valuesList.Append("'" + GetFieldValue(pRefPoleFeature,
                            fldIdx) + "'");
                    else
                        valuesList.Append("'" + "<NULL>" + "'");
                    valuesList.Append(",");
                    
                    //JOINT_OWNED  
                    //Changed on 07/19 from direction from Alasdair / Robert to accommodate 
                    //the Contact Pole scenario 
                    jointCount = 1;
                    jointOwned = false;
                    fldIdx = Convert.ToInt32(hshPoleFields["JOINTCOUNT"]);
                    if (pRefPoleFeature.get_Value(fldIdx) != DBNull.Value)
                    {
                        jointCountString = GetFieldValue(pRefPoleFeature, fldIdx);
                        jointCountString = jointCountString.Substring(0, 1);
                        Int32.TryParse(jointCountString, out jointCount);
                    }
                    ISet pJointUseAttachSet = m_JointUseAttachmentRC.GetObjectsRelatedToObject((IObject)pRefPoleFeature);
                    pJointUseAttachSet.Reset();
                    //If jointcount greater than 1 or there is at least 1 joint use attachment then 
                    //JOINT_OWNED = TRUE (otherwise FALSE)
                    if ((pJointUseAttachSet.Count > 0) || (jointCount > 1))
                        jointOwned = true;
                    Marshal.FinalReleaseComObject(pJointUseAttachSet);
                    valuesList.Append("'" + jointOwned.ToString().ToUpper() + "'");
                                        
                    //Write the insert statement 
                    sqlStatement = BuildSQLInsertStatement(PoleLoadConstants.POLES_TBL, 
                        fieldList.ToString(), valuesList.ToString()); 
                }

                //Execute the command against the database 
                if (sqlStatement != string.Empty)
                {
                    pCmd = _pConn.CreateCommand();
                    pCmd.Connection = _pConn;
                    pCmd.CommandText = sqlStatement;
                    recordsUpdated = pCmd.ExecuteNonQuery();
                }
                if (recordsUpdated == 0)
                    throw new Exception("SQL insert failed for static data");

            }
            catch (Exception ex)
            {
                Shared.WriteToLogfile("Error in " + MethodBase.GetCurrentMethod() +
                    " details: " + ex.Message);
                throw ex;
            }
        }

        //private void WriteSpanListToDB_old(List<Span> pSpanList)
        //{
        //    try
        //    {
        //        StringBuilder fieldList = new StringBuilder();
        //        fieldList.Clear();
        //        fieldList.Append("FROM_POLE_GUID");
        //        fieldList.Append(",");
        //        fieldList.Append("FROM_POLE_RL");
        //        fieldList.Append(",");
        //        fieldList.Append("TO_POLE_GUID");
        //        fieldList.Append(",");
        //        fieldList.Append("TO_POLE_RL");
        //        fieldList.Append(",");
        //        fieldList.Append("CONDUCTOR_GUID");
        //        fieldList.Append(",");
        //        fieldList.Append("CONDUCTOR_SPAN_LENGTH");
        //        fieldList.Append(",");
        //        fieldList.Append("CONDUCTOR_CIRCUIT");
        //        fieldList.Append(",");
        //        fieldList.Append("CONDUCTOR_CLASS");
        //        fieldList.Append(",");
        //        fieldList.Append("CONDUCTOR_SUBTYPE");
        //        fieldList.Append(",");
        //        fieldList.Append("CONDUCTOR_ANGLE");

        //        if (_pConn == null)
        //        {
        //            //Get output DB connection 
        //            _pConn = new SqlConnection(
        //                Shared.OutputConnectString);
        //        }
        //        if (_pConn.State != System.Data.ConnectionState.Open)
        //            _pConn.Open();

        //        //String to hold the SQL insert statement 
        //        StringBuilder sqlStatement = new StringBuilder();
        //        StringBuilder valuesList = new StringBuilder();
        //        SqlCommand pCmd = _pConn.CreateCommand();

        //        foreach (Span pSpan in pSpanList)
        //        {
        //            sqlStatement.Clear();
        //            sqlStatement.Append("INSERT INTO " + PoleLoadConstants.SPAN_ANGLE_TBL + " ");
        //            sqlStatement.Append(" (");
        //            sqlStatement.Append(fieldList.ToString());
        //            sqlStatement.Append(" ) ");
        //            sqlStatement.Append("VALUES");
        //            sqlStatement.Append(" (");
        //            sqlStatement.Append("'" + pSpan.FromPoleGUID + "'" + ",");
        //            sqlStatement.Append("'" + pSpan.FromPoleRL + "'" + ",");
        //            sqlStatement.Append("'" + pSpan.ToPoleGUID + "'" + ",");
        //            sqlStatement.Append("'" + pSpan.ToPoleRL + "'" + ",");
        //            sqlStatement.Append("'" + pSpan.ConductorGUID + "'" + ",");
        //            sqlStatement.Append("'" + pSpan.Length + "'" + ",");
        //            sqlStatement.Append("'" + pSpan.CircuitId + "'" + ",");
        //            sqlStatement.Append("'" + pSpan.ConductorClass + "'" + ",");
        //            sqlStatement.Append("'" + pSpan.ConductorSubtype + "'" + ",");
        //            sqlStatement.Append("'" + pSpan.SpanAngle + "'");
        //            sqlStatement.Append(" )");

        //            //Execute the command against the database 
        //            pCmd.CommandText = sqlStatement.ToString();
        //            int recordsUpdated = pCmd.ExecuteNonQuery();
        //            if (recordsUpdated == 0)
        //                throw new Exception("SQL insert failed for static data");

        //        }                
        //    }

        //    catch (Exception ex)
        //    {
        //        Shared.WriteToLogfile("Error in " + MethodBase.GetCurrentMethod() +
        //            " details: " + ex.Message);
        //        throw ex;
        //    }
        //}
        /// <summary>
        /// New format 
        /// </summary>
        /// <param name="pSpanList"></param>
        private void WriteSpanListToDB( 
            List<Span> pSpanList, 
            List<Conductor> pAllConductors, 
            string snowLoadingZone,
            string popDensity) 
        {
            try
            {
                StringBuilder fieldList = new StringBuilder();
                fieldList.Clear();
                fieldList.Append("POLEGUID");
                fieldList.Append(",");
                fieldList.Append("TARGETGUID");
                fieldList.Append(",");
                fieldList.Append("CONDUCTORGUID");
                fieldList.Append(",");
                fieldList.Append("CONDUCTORCLASS");
                fieldList.Append(",");
                fieldList.Append("CONDUCTORSUBTYPE");
                fieldList.Append(",");
                fieldList.Append("CONDUCTORUSE");
                fieldList.Append(",");
                fieldList.Append("CONDUCTORCIRCUIT");
                fieldList.Append(",");
                fieldList.Append("[KEY]");
                fieldList.Append(",");
                fieldList.Append("COUNT");
                fieldList.Append(",");
                fieldList.Append("FRAMING");
                fieldList.Append(",");
                fieldList.Append("LENGTH");
                fieldList.Append(",");
                fieldList.Append("ANGLE");
                fieldList.Append(",");
                fieldList.Append("RISE");
                fieldList.Append(",");
                fieldList.Append("CONDUCTORCODE");

                if (_pConn == null)
                {
                    //Get output DB connection 
                    _pConn = new SqlConnection(
                        Shared.OutputConnectString);
                }
                if (_pConn.State != System.Data.ConnectionState.Open)
                    _pConn.Open();

                //String to hold the SQL insert statement 
                StringBuilder valuesList = new StringBuilder();
                SqlCommand pCmd = _pConn.CreateCommand();
                Conductor pSpanConductor = null;
                string conductorKey = string.Empty;
                double riseFall = 0; 

                foreach (Span pSpan in pSpanList)
                {

                    //Find the conductor using the GUID / CLASS 
                    pSpanConductor = null;
                    foreach (Conductor pConductor in pAllConductors)
                    {
                        if (pConductor.GUID == pSpan.ConductorGUID)
                        {
                            pSpanConductor = pConductor;
                            break;
                        }
                    }
                                                            
                    //Loop through each of the conductorinfos 
                    if (pSpanConductor != null)
                    {
                        //Loop through the conductorinfos 
                        foreach (ConductorInfo pConInfo in pSpanConductor.ConductorInfos)
                        {
                            valuesList.Clear();
                            conductorKey = GetConductorKey( 
                                pSpanConductor.FCName, 
                                pSpanConductor.Subtype, 
                                pConInfo.ConductorCode, 
                                pConInfo.ConductorType,  
                                pConInfo.Material, 
                                pConInfo.ConductorSize, 
                                snowLoadingZone, 
                                popDensity); 
                            riseFall = Math.Round(pSpan.ToPoleRL - pSpan.FromPoleRL, 1);

                            //Build the values list                             
                            valuesList.Append("'" + pSpan.FromPoleGUID + "'" + ","); //POLEGUID
                            valuesList.Append("'" + pSpan.ToPoleGUID + "'" + ","); //TARGETGUID
                            valuesList.Append("'" + pSpan.ConductorGUID + "'" + ","); //CONDUCTORGUID 
                            valuesList.Append("'" + pSpanConductor.FCName + "'" + ","); //CONDUCTORCLASS 
                            valuesList.Append("'" + pSpanConductor.Subtype + "'" + ","); //CONDUCTORSUBTYPE
                            valuesList.Append("'" + pConInfo.ConductorUse + "'" + ","); //CONDUCTORUSE
                            valuesList.Append("'" + pSpan.CircuitId + "'" + ","); //CONDUCTORCIRCUIT
                            valuesList.Append("'" + conductorKey + "'" + ","); //KEY
                            valuesList.Append("'" + pConInfo.ConductorCount + "'" + ","); //COUNT 
                            valuesList.Append("'" + pSpanConductor.ConstructionType + "'" + ","); //FRAMING
                            valuesList.Append("'" + Math.Round(pSpan.Length, 1) + "'" + ","); //LENGTH
                            valuesList.Append("'" + Math.Round(pSpan.SpanAngle, 1) + "'" + ","); //ANGLE 
                            valuesList.Append("'" + riseFall + "'" + ","); //RISE 
                            valuesList.Append("'" + pConInfo.ConductorCode + "'"); //CONDUCTORCODE

                            //Execute the command against the database 
                            pCmd.CommandText = BuildSQLInsertStatement(
                                PoleLoadConstants.SPANS_TBL, fieldList.ToString(), valuesList.ToString());
                            int recordsUpdated = pCmd.ExecuteNonQuery();
                            if (recordsUpdated == 0)
                                throw new Exception("SQL insert failed for span data");
                        }

                        //Some conductors do not have conductorinfo records 
                        if (pSpanConductor.ConductorInfos.Count == 0)
                        {
                            //What do we do? Have to confirm this with Alasdair 
                            valuesList.Clear();
                            conductorKey = GetConductorKey(
                                pSpanConductor.FCName,
                                pSpanConductor.Subtype, 
                                -1,
                                "Unknown",
                                "Unknown",
                                "Unknown",
                                snowLoadingZone, 
                                popDensity);
                            riseFall = Math.Round(pSpan.ToPoleRL - pSpan.FromPoleRL, 1);

                            //determine the conductoruse 
                            string conductorUse = "Unknown";
                            switch (pSpanConductor.Subtype.ToUpper())
                            {
                                //Primary scenarios 
                                case "SINGLE PHASE PRIMARY OVERHEAD":
                                    conductorUse = "Primary";
                                    break;
                                case "TWO PHASE PRIMARY OVERHEAD":
                                    conductorUse = "Primary";
                                    break;
                                case "THREE PHASE PRIMARY OVERHEAD":
                                    conductorUse = "Primary";
                                    break;
                                //Secondary Scenarios 
                                case "SINGLE PHASE SECONDARY OVERHEAD":
                                    conductorUse = "Secondary";
                                    break;
                                case "THREE PHASE SECONDARY OVERHEAD":
                                    conductorUse = "Secondary";
                                    break;
                                case "SERVICE OVERHEAD CONDUCTOR":
                                    conductorUse = "Service";
                                    break;
                                case "STREETLIGHT OVERHEAD CONDUCTOR":
                                    conductorUse = "Streetlight";
                                    break;
                                case "PSEUDO SERVICE":
                                    conductorUse = "Service";
                                    break; 

                                //NeutralConductor scenario 
                                case "OVERHEAD":
                                    conductorUse = "Neutral";
                                    break;
                                default:
                                    //Leave it to Unknown 
                                    break;
                            }


                            //Build the values list                             
                            valuesList.Append("'" + pSpan.FromPoleGUID + "'" + ","); //POLEGUID
                            valuesList.Append("'" + pSpan.ToPoleGUID + "'" + ","); //TARGETGUID
                            valuesList.Append("'" + pSpan.ConductorGUID + "'" + ","); //CONDUCTORGUID 
                            valuesList.Append("'" + pSpanConductor.FCName + "'" + ","); //CONDUCTORCLASS 
                            valuesList.Append("'" + pSpanConductor.Subtype + "'" + ","); //CONDUCTORSUBTYPE
                            valuesList.Append("'" + conductorUse + "'" + ","); //CONDUCTORUSE
                            valuesList.Append("'" + pSpan.CircuitId + "'" + ","); //CONDUCTORCIRCUIT
                            valuesList.Append("'" + conductorKey + "'" + ","); //KEY
                            valuesList.Append("'" + "0" + "'" + ","); //COUNT 
                            valuesList.Append("'" + pSpanConductor.ConstructionType + "'" + ","); //FRAMING
                            valuesList.Append("'" + Math.Round(pSpan.Length, 1) + "'" + ","); //LENGTH
                            valuesList.Append("'" + Math.Round(pSpan.SpanAngle, 1) + "'" + ","); //ANGLE 
                            valuesList.Append("'" + riseFall + "'" + ","); //RISE 
                            valuesList.Append("'" + "-1" + "'"); //CONDUCTORCODE 

                            //Execute the command against the database 
                            pCmd.CommandText = BuildSQLInsertStatement(
                                PoleLoadConstants.SPANS_TBL, fieldList.ToString(), valuesList.ToString());
                            int recordsUpdated = pCmd.ExecuteNonQuery();
                            if (recordsUpdated == 0)
                                throw new Exception("SQL insert failed for span data");

                        }
                    }
                    else
                    {
                        Shared.WriteToLogfile("Error cannot find underlying conductor!");
                        throw new Exception("Unable to find underlying conductor"); 
                    }
                }
            }

            catch (Exception ex)
            {
                Shared.WriteToLogfile("Error in " + MethodBase.GetCurrentMethod() +
                    " details: " + ex.Message);
                throw ex;
            }
        }

        private string GetConductorKey( 
            string fcName, 
            string subtype, 
            int conductorCode, 
            string conductorType,  
            string material, 
            string size,  
            string snowLoadingZone,
            string popDensity)
        {
            try
            {
                //FC Name 
                StringBuilder conKeyBuilder = new StringBuilder();

                switch (fcName)
                {
                    case "PRIOHCONDUCTOR":
                        conKeyBuilder.Append("PRIOH");
                        break;
                    case "SECOHCONDUCTOR":
                        conKeyBuilder.Append("SECOH");
                        break;
                    case "NEUTRALCONDUCTOR":
                        //conKeyBuilder.Append("NEUOH_");
                        conKeyBuilder.Append("PRIOH");
                        break;
                    default:
                        throw new Exception("Unhandled Conductor Type");
                }

                //Loading Key 
                switch (snowLoadingZone.ToUpper())
                {
                    //Primary scenarios 
                    case "HEAVY":
                        conKeyBuilder.Append("_H");
                        break;
                    case "INTERMEDIATE":
                        conKeyBuilder.Append("_I");
                        break;
                    case "LIGHT":
                        if (popDensity.Contains("HIGH"))
                            conKeyBuilder.Append("_S");
                        else 
                            conKeyBuilder.Append("_L");
                        break;
                    default:
                        conKeyBuilder.Append("_L");
                        break;
                }

                //Subtype 
                switch (subtype.ToUpper())
                {
                    //Primary scenarios 
                    case "SINGLE PHASE PRIMARY OVERHEAD":
                        //conKeyBuilder.Append("1P_");
                        //Do nothing 
                        break;
                    case "TWO PHASE PRIMARY OVERHEAD":
                        //conKeyBuilder.Append("2P_");
                        //Do nothing 
                        break;
                    case "THREE PHASE PRIMARY OVERHEAD":
                        //conKeyBuilder.Append("3P_");
                        //Do nothing 
                        break;

                    //Secondary Scenarios 
                    case "SINGLE PHASE SECONDARY OVERHEAD":
                        conKeyBuilder.Append("_SEC");
                        break;
                    case "THREE PHASE SECONDARY OVERHEAD":
                        conKeyBuilder.Append("_SEC");
                        break;
                    case "SERVICE OVERHEAD CONDUCTOR":
                        conKeyBuilder.Append("_SRV"); 
                        break;
                    case "STREETLIGHT OVERHEAD CONDUCTOR":
                        conKeyBuilder.Append("_SRV");
                        break;
                    case "PSEUDO SERVICE":
                        conKeyBuilder.Append("_PSRV");
                        break;

                    //NeutralConductor scenario 
                    case "OVERHEAD":
                        //Do nothing since all subtypes are overhead 
                        break; 

                    //Don't know what this is 
                    default:
                        conKeyBuilder.Append("_UNKNOWN");
                        break; 
                }


                //Change SecOH material mapping - Brian Nugent email 5/11/2017
                string plcSecOHMaterial = string.Empty;
                string plcSecOHSize = size.ToString();

                if (fcName == "SECOHCONDUCTOR")
                {
                    switch (material)
                    {
                        case "AL":
                            plcSecOHMaterial = "AL";
                            break;
                        case "AWAC":
                            plcSecOHMaterial = "AL";
                            break;
                        case "Aluminum Conductor Steel Supported":
                            plcSecOHMaterial = "ACSS";
                            break;
                        case "ACSR":
                            plcSecOHMaterial = "ACSR";
                            break;
                        case "CU":
                            plcSecOHMaterial = "CU";
                            break;
                        case "Copper Weld":
                            plcSecOHMaterial = "CUWD";
                            break;
                        case "Hard Drawn Copper":
                            plcSecOHMaterial = "CU";
                            break;
                        case "None":
                            plcSecOHMaterial = "UNKNOWN";
                            break;
                        case "NONE":
                            plcSecOHMaterial = "UNKNOWN";
                            break;
                        case "Soft Drawn Copper":
                            plcSecOHMaterial = "CU";
                            break;
                        case "Stranded Copper":
                            plcSecOHMaterial = "CU";
                            break;
                        case "Steel":
                            plcSecOHMaterial = "STEEL";
                            break;
                        case "Unknown":
                            plcSecOHMaterial = "UNKNOWN";
                            break;
                        case "":
                            plcSecOHMaterial = "UNKNOWN";
                            break;
                        default:
                            Debug.Print("have an issue here!");
                            break;
                    }

                    if (conductorType == "Aluminum Wire Aerial Cable")
                        plcSecOHMaterial = "AWAC";
                    if (size == "None" || size == "Unknown" || size == "UNKNOWN"  || size == "")
                        plcSecOHSize = "UNKNOWN";
                }

                if (material == "Unknown")
                    material = material.ToUpper();

                //Material, Size
                if (subtype.ToUpper() != "PSEUDO SERVICE")
                {
                    //If there is a valid conductor code 
                    if (conductorCode != -1)
                    {
                        if (Shared.ConductorCodes().ContainsKey(conductorCode))
                        {
                            //Found a valid conductor code so use the 
                            //conductorcode to derive material / size 
                            ConductorCode pConCode = (ConductorCode)Shared.
                                ConductorCodes()[conductorCode];
                            material = pConCode.Material;
                            size = pConCode.Size;
                        }
                    }

                    if (fcName == "SECOHCONDUCTOR")
                    {
                        conKeyBuilder.Append("_" + plcSecOHMaterial);
                        conKeyBuilder.Append("_" + plcSecOHSize);
                    }
                    else
                    { 
                        conKeyBuilder.Append("_" + material);
                        conKeyBuilder.Append("_" + size);
                    }
                }


                //Quadraplex / triplex logic 
                if ((fcName == "SECOHCONDUCTOR") && (subtype.ToUpper() != "PSEUDO SERVICE"))
                {
                    //QPX 
                    if ((material == "AL" || material == "AWAC" || material == "ACSR") && 
                        (conductorType == "Quadraplex Aerial Cable" || conductorType == "Quadraplex 600V") && 
                        (size == "1/0" || size == "4/0"))
                        conKeyBuilder.Append("_QPX");
                    //TPX 
                    if ((material == "AL" || material == "AWAC" || material == "ACSR") &&
                        (conductorType == "Triple Aerial Cable" || conductorType == "Triplex") &&
                        (size == "1/0" || size == "4/0"))
                        conKeyBuilder.Append("_TPX");


                    //QPX 
                    if ((material == "CU" || material == "Stranded Copper") &&
                        (conductorType == "Quadraplex Aerial Cable" || conductorType == "Quadraplex 600V") &&
                        (size == "4" || size == "6"))
                        conKeyBuilder.Append("_QPX");
                    //TPX 
                    if ((material == "CU" || material == "Stranded Copper") &&
                        (conductorType == "Triple Aerial Cable" || conductorType == "Triplex") &&
                        (size == "4" || size == "6"))
                        conKeyBuilder.Append("_TPX");


                    //QPX 
                    if ((material == "None" || material == "UNKNOWN") &&
                        (conductorType == "Quadraplex Aerial Cable" || conductorType == "Quadraplex 600V") &&
                        (size == "1/0" || size == "4/0" || size == "4" || size == "6"))
                        conKeyBuilder.Append("_QPX");
                    //TPX 
                    if ((material == "None" || material == "UNKNOWN") &&
                        (conductorType == "Triple Aerial Cable" || conductorType == "Triplex") &&
                        (size == "1/0" || size == "4/0" || size == "4" || size == "6"))
                        conKeyBuilder.Append("_TPX"); 
                }

                string conductorKey = conKeyBuilder.ToString();
                ReplaceHyphenInKey(ref conductorKey); 
                return conductorKey; 
            }
            catch (Exception ex)
            {
                Shared.WriteToLogfile("Error in " + MethodBase.GetCurrentMethod() +
                    " details: " + ex.Message);
                throw ex;
            }
        }

        /// <summary>
        /// Keys are not allowed to have a hyphen so replace 
        /// the hyphen with a space 
        /// </summary>
        /// <returns></returns>
        private void ReplaceHyphenInKey(ref string theKey)
        {
            try
            {
                if (theKey.Contains("-"))
                    theKey = theKey.Replace("-"," ");
                if (theKey.Contains("Unknown"))
                    theKey = theKey.Replace("Unknown", "UNKNOWN");
            }
            catch (Exception ex)
            {
                Shared.WriteToLogfile("Error in " + MethodBase.GetCurrentMethod() +
                    " details: " + ex.Message);
                throw ex;
            }
        }

        /// <summary>
        /// Returns the value of the landbase feature at the passed 
        /// point, from the passed field or the defaultValue if there 
        /// is no intersecting polygon at the passed point
        /// </summary>
        /// <param name="pRefPolePoint"></param>
        /// <param name="pLandbaseFC"></param>
        /// <param name="fieldName"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        private string GetPointInPolygonLandbaseValue(
            IPoint pRefPolePoint,
            IFeatureClass pLandbaseFC,
            string fieldName,
            string defaultValue)
        {
            try
            {
                string landbaseValue = string.Empty;
                ISpatialFilter pSF = new SpatialFilterClass();
                pSF.Geometry = pRefPolePoint;
                pSF.SpatialRel = esriSpatialRelEnum.esriSpatialRelIntersects;
                IFeatureCursor pFCursor = pLandbaseFC.Search(pSF, false);
                IFeature pTargetFeature = pFCursor.NextFeature();
                Marshal.FinalReleaseComObject(pFCursor);
                int fldIdx = pLandbaseFC.Fields.FindField(fieldName);
                if (fldIdx == -1)
                    throw new Exception("Incorrect field name for landbase featureclass");

                if (pTargetFeature != null)
                {
                    if (pTargetFeature.get_Value(fldIdx) != DBNull.Value)
                    {
                        landbaseValue = pTargetFeature.get_Value(fldIdx).ToString();
                    }
                    else
                    {
                        landbaseValue = "<NULL>";
                    }
                }
                else
                {
                    landbaseValue = defaultValue;
                }

                return landbaseValue;
            }
            catch (Exception ex)
            {
                Shared.WriteToLogfile("Error in " + MethodBase.GetCurrentMethod() +
                    " details: " + ex.Message);
                throw new Exception("Error encountered in GetPointInPolygonLandbaseValue: " + ex.Message);
            }
        }

        //private void WriteOutLandbaseData()
        //{

        //    try
        //    {
        //        //Open up the poles featureclass 
        //        Hashtable hshAllAttributes = new Hashtable();
        //        IFeatureWorkspace pFileGDB_FWS = (IFeatureWorkspace)Shared.GetWorkspaceByName("output");
        //        IFeatureWorkspace pFWS = (IFeatureWorkspace)Shared.GetWorkspaceByName("electric");

        //        IFeatureClass pSupportStructureFC = pFileGDB_FWS.OpenFeatureClass("Urb_Corr_Clim_Snow_RCZ");
        //        IQueryFilter pQF = new QueryFilterClass();
        //        IFeatureCursor pFCursor = null;
        //        IFeature pSupportStructFeature = null;
        //        Hashtable hshAllDomains = new Hashtable();

        //        Hashtable hshDomain = new Hashtable();
        //        hshDomain.Add(1, "Urban");
        //        hshDomain.Add(0, "Rural");
        //        hshAllDomains.Add("URBAN_IDC", hshDomain); 

        //        string guid = string.Empty;
        //        string sapEquipId = string.Empty;
        //        int poleCount = 0;

        //        Hashtable hshFieldsToInclude = new Hashtable();
        //        hshFieldsToInclude.Add("SAPEQUIPID", "SAPEQUIPID");
        //        hshFieldsToInclude.Add("GLOBALID_OLD", "GlobalId");
        //        hshFieldsToInclude.Add("URBAN_IDC", "Urban Rural Indicator");
        //        hshFieldsToInclude.Add("CZ_CODE", "Climate Zone Code");
        //        hshFieldsToInclude.Add("CORROSION", "Corrosion Zone");
        //        hshFieldsToInclude.Add("SNOWLOAD", "Snow Loading Zone");
        //        hshFieldsToInclude.Add("ZONE", "Raptor Concentration Zone");

        //        Hashtable hshFieldIndexes = new Hashtable();
        //        foreach (string fld in hshFieldsToInclude.Keys)
        //        {
        //            hshFieldIndexes.Add(fld, pSupportStructureFC.Fields.FindField(fld));
        //        }


        //        //Write the header 
        //        StringBuilder sb = new StringBuilder();
        //        foreach (string alias in hshFieldsToInclude.Values)
        //        {
        //            if (sb.ToString() == string.Empty)
        //                sb.Append(alias);
        //            else
        //            {
        //                sb.Append(",");
        //                sb.Append(alias);
        //            }
        //        }
        //        Shared.WriteStaticDataToCSV(sb.ToString());


        //        string theValue = string.Empty;
        //        object theValueObj = null;
        //        string theDomainDescription = string.Empty;
        //        pFCursor = pSupportStructureFC.Search(null, false);
        //        pSupportStructFeature = pFCursor.NextFeature();

        //        while (pSupportStructFeature != null)
        //        {

        //            //If the pole is within mother load or the pole is in the list of 
        //            //poles with 715 AL then process the pole 
        //            poleCount++;                    
        //            sb.Clear();                     

        //            foreach (string field in hshFieldsToInclude.Keys)
        //            {


        //                if (sb.ToString() != string.Empty)
        //                {
        //                    sb.Append(",");
        //                }


        //                if (hshAllDomains.ContainsKey(field))
        //                {
        //                    int fldIdx = (int)hshFieldIndexes[field];
        //                    theValueObj = pSupportStructFeature.get_Value(fldIdx);
        //                    hshDomain = (Hashtable)hshAllDomains[field];
        //                    int theVal = Convert.ToInt32(theValueObj);

        //                    if (theVal == 0)
        //                         sb.Append("Rural");
        //                    else 
        //                        sb.Append("Urban");


        //                }
        //                else
        //                {
        //                    if (pSupportStructFeature.get_Value((int)hshFieldIndexes[field]) != DBNull.Value)
        //                    {
        //                        theValue = pSupportStructFeature.get_Value((int)hshFieldIndexes[field]).ToString();
        //                        sb.Append(theValue);
        //                    }
        //                    else
        //                    {
        //                        sb.Append("<NULL>");
        //                    }
        //                }
        //            }


        //            //Write the SpanList to csv 
        //            Shared.WriteStaticDataToCSV(sb.ToString());

        //            //} //end of if statement for inside Mother Lode or 715 AL 
        //            pSupportStructFeature = pFCursor.NextFeature();
        //        }
        //        Marshal.FinalReleaseComObject(pFCursor);


        //    }
        //    catch (Exception ex)
        //    {
        //        Shared.WriteToLogfile("Entering error handler for " + "WriteOutStaticData" + ex.Message);
        //        throw new Exception("Error encountered in CalculatePoleSpans: " +
        //        ex.Message);
        //    }
        //}



        /// <summary>
        /// Returns a lit of comma separated keys to allow use in a SQL 
        /// IN() clause 
        /// </summary>
        /// <param name="hshKeys"></param>
        /// <param name="batchSize"></param>
        /// <param name="addApostrophe"></param>
        /// <returns></returns>
        //private string[] GetArrayOfCommaSeparatedKeys(Hashtable hshKeys, int batchSize, bool addApostrophe)
        //{
        //    try
        //    {
        //        Hashtable hshCommaSeparatedKeys = new Hashtable();
        //        int counter = 0;
        //        StringBuilder batchLine = new StringBuilder();

        //        foreach (object key in hshKeys.Keys)
        //        {
        //            if (counter == 0)
        //            {
        //                if (addApostrophe)
        //                    batchLine.Append("'" + key.ToString() + "'");
        //                else
        //                    batchLine.Append(key.ToString());
        //            }
        //            else
        //            {
        //                if (addApostrophe)
        //                    batchLine.Append("," + "'" + key.ToString() + "'");
        //                else
        //                    batchLine.Append("," + key.ToString());
        //            }

        //            counter++;
        //            if (counter == batchSize)
        //            {
        //                hshCommaSeparatedKeys.Add(batchLine.ToString(), 0);
        //                batchLine = new StringBuilder();
        //                counter = 0;
        //            }
        //        }

        //        //Add what is left over 
        //        if (batchLine.ToString().Length != 0)
        //            hshCommaSeparatedKeys.Add(batchLine.ToString(), 0);

        //        //Convert this to an array 
        //        counter = 0;
        //        string[] commaSepKeys = new string[hshCommaSeparatedKeys.Count];
        //        foreach (string line in hshCommaSeparatedKeys.Keys)
        //        {
        //            commaSepKeys[counter] = line;
        //            counter++;
        //        }

        //        //return array 
        //        return commaSepKeys;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception("Error returning array of comma separated keys");
        //    }
        //}


        //private Hashtable Get715ALsListPlusMLs(IFeatureWorkspace pFWS, string fcName, IPolygon pClipPolygon)
        //{
        //    Hashtable hsh715AlsPlusMLs = new Hashtable(); 

        //    try
        //    {
        //        IFeatureClass pFC = pFWS.OpenFeatureClass(fcName);
        //        IQueryFilter pQF = new QueryFilterClass();
        //        pQF.WhereClause = "IS715ALIDC = 1";
        //        int fldIdx = pFC.Fields.FindField("OBJECTID_OLD"); 
        //        int totalPoles = pFC.FeatureCount(pQF);
        //        int oid = -1; 
        //        IFeatureCursor pFCursor = pFC.Search(pQF, false);
        //        IFeature pSupportStructFeature = pFCursor.NextFeature();

        //        while (pSupportStructFeature != null)
        //        {
        //            if (pSupportStructFeature.get_Value(fldIdx) != DBNull.Value)
        //            {
        //                oid = Convert.ToInt32(pSupportStructFeature.get_Value(fldIdx));
        //                if (!hsh715AlsPlusMLs.ContainsKey(oid))
        //                    hsh715AlsPlusMLs.Add(oid, 0); 
        //            }

        //            pSupportStructFeature = pFCursor.NextFeature();
        //        }
        //        Marshal.FinalReleaseComObject(pFCursor);

        //        //Now look for the mother lodes 
        //        ISpatialFilter pSF = new SpatialFilterClass();
        //        pSF.Geometry = pClipPolygon;
        //        pSF.SpatialRel = esriSpatialRelEnum.esriSpatialRelIntersects;
        //        pFCursor = pFC.Search(pSF, false);
        //        pSupportStructFeature = pFCursor.NextFeature();
        //        while (pSupportStructFeature != null)
        //        {
        //            if (pSupportStructFeature.get_Value(fldIdx) != DBNull.Value)
        //            {
        //                oid = Convert.ToInt32(pSupportStructFeature.get_Value(fldIdx));
        //                if (!hsh715AlsPlusMLs.ContainsKey(oid))
        //                    hsh715AlsPlusMLs.Add(oid, 0);
        //            }

        //            pSupportStructFeature = pFCursor.NextFeature();
        //        }
        //        Marshal.FinalReleaseComObject(pFCursor);
        //        return hsh715AlsPlusMLs; 
        //    }
        //    catch (Exception ex)
        //    {
        //        Shared.WriteToLogfile("Entering error handler for " + "Get715ALsList" + ex.Message);
        //        throw new Exception("Error encountered in CalculatePoleSpans: " +
        //        ex.Message);
        //    }
        //}


        private double GetSpanAngleFromAdjacentPoles(IPoint pRefPole, IPoint pAdjacentPole)
        {
            try
            {
                ILine pLine = new LineClass();
                pLine.SpatialReference = pRefPole.SpatialReference;

                //pLine.FromPoint is always the current pole 
                pLine.PutCoords(pRefPole, pAdjacentPole);

                double spanAngleDegrees = GetSpanAngleDegrees(pLine.Angle);
                return spanAngleDegrees;
            }
            catch (Exception ex)
            {
                Shared.WriteToLogfile("Error in " + MethodBase.GetCurrentMethod() +
                    " details: " + ex.Message);
                throw new Exception("Error calculating span a angle");
            }
        }

        private double GetSpanLength(IPoint pRefPole, IPoint pAdjacentPole)
        {
            try
            {
                ILine pLine = new LineClass();
                pLine.SpatialReference = pRefPole.SpatialReference;

                //pLine.FromPoint is always the current pole 
                pLine.PutCoords(pRefPole, pAdjacentPole);
                return pLine.Length;
            }
            catch (Exception ex)
            {
                Shared.WriteToLogfile("Error in " + MethodBase.GetCurrentMethod() +
                    " details: " + ex.Message);
                throw new Exception("Error calculating span length");
            }
        }

        private IPolyline GetSpanPolyline(IPoint pRefPole, IPoint pAdjacentPole)
        {
            try
            {
                IPolyline pPolyline = null;
                ILine pLine = new LineClass();
                pLine.SpatialReference = pRefPole.SpatialReference;

                //pLine.FromPoint is always the current pole 
                pLine.PutCoords(pRefPole, pAdjacentPole);
                pPolyline = GetPolylineFromLine(pLine, pRefPole.SpatialReference);
                return pPolyline;
            }
            catch (Exception ex)
            {
                Shared.WriteToLogfile("Error in " + MethodBase.GetCurrentMethod() +
                    " details: " + ex.Message);
                throw new Exception("Error calculating span length");
            }
        }


        private double GetSpanAngleDegrees(double spanAngleRadians)
        {
            try
            {
                double spanAngleDegrees = (180 * spanAngleRadians) / Math.PI;
                //Debug.Print("And initially is: " + spanAngleDegrees.ToString());                
                //Debug.Print("Returning: " + spanAngleDegrees.ToString());
                return spanAngleDegrees;
            }
            catch (Exception ex)
            {
                Shared.WriteToLogfile("Error in " + MethodBase.GetCurrentMethod() +
                    " details: " + ex.Message);
                throw new Exception("Error encountered in GetSpanAngle: " +
                ex.Message);
            }
        }

        //private bool SpanIsDuplicate(Span pRefSpan, List<Span> pSpanList)
        //{
        //    try
        //    {
        //        bool spanIsDuplicate = false;
        //        if (pRefSpan.ToPoleGUID == "")
        //        {
        //            foreach (Span pSpan in pSpanList)
        //            {
        //                if ((pSpan.Length == pRefSpan.Length) &&
        //                    (pSpan.SpanAngle == pRefSpan.SpanAngle))
        //                {
        //                    spanIsDuplicate = true;
        //                    break;
        //                }
        //            }
        //        }
        //        else
        //        {
        //            foreach (Span pSpan in pSpanList)
        //            {
        //                if ((pSpan.ToPoleGUID == pRefSpan.ToPoleGUID) && (pSpan.ConductorGUID == pRefSpan.ConductorGUID))
        //                {
        //                    spanIsDuplicate = true;
        //                    break; 
        //                }
        //            }
        //        }
        //        return spanIsDuplicate; 
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception("Error encountered in SpanIsDuplicate: " +
        //        ex.Message);
        //    }
        //}    

        //private void GetPoleFieldIndexes(IFeatureClass pSupportStructureFC)
        //{
        //    try
        //    {
        //        _hshPoleFieldIndexes = new Hashtable();
        //        _hshPoleFieldIndexes.Add("Span1Angle", pSupportStructureFC.Fields.FindField("Span1Angle"));
        //        _hshPoleFieldIndexes.Add("Span2Angle", pSupportStructureFC.Fields.FindField("Span2Angle"));
        //        _hshPoleFieldIndexes.Add("Span3Angle", pSupportStructureFC.Fields.FindField("Span3Angle"));
        //        _hshPoleFieldIndexes.Add("Span4Angle", pSupportStructureFC.Fields.FindField("Span4Angle"));

        //        _hshPoleFieldIndexes.Add("Span1Length", pSupportStructureFC.Fields.FindField("Span1Length"));
        //        _hshPoleFieldIndexes.Add("Span2Length", pSupportStructureFC.Fields.FindField("Span2Length"));
        //        _hshPoleFieldIndexes.Add("Span3Length", pSupportStructureFC.Fields.FindField("Span3Length"));
        //        _hshPoleFieldIndexes.Add("Span4Length", pSupportStructureFC.Fields.FindField("Span4Length"));

        //        _hshPoleFieldIndexes.Add("Span1ConductorCode", pSupportStructureFC.Fields.FindField("Span1ConductorCode"));
        //        _hshPoleFieldIndexes.Add("Span2ConductorCode", pSupportStructureFC.Fields.FindField("Span2ConductorCode"));
        //        _hshPoleFieldIndexes.Add("Span3ConductorCode", pSupportStructureFC.Fields.FindField("Span3ConductorCode"));
        //        _hshPoleFieldIndexes.Add("Span4ConductorCode", pSupportStructureFC.Fields.FindField("Span4ConductorCode"));

        //        _hshPoleFieldIndexes.Add("Span1PriSecIdc", pSupportStructureFC.Fields.FindField("Span1PriSecIdc"));
        //        _hshPoleFieldIndexes.Add("Span2PriSecIdc", pSupportStructureFC.Fields.FindField("Span2PriSecIdc"));
        //        _hshPoleFieldIndexes.Add("Span3PriSecIdc", pSupportStructureFC.Fields.FindField("Span3PriSecIdc"));
        //        _hshPoleFieldIndexes.Add("Span4PriSecIdc", pSupportStructureFC.Fields.FindField("Span4PriSecIdc"));

        //        _hshPoleFieldIndexes.Add("PriSpanCount", pSupportStructureFC.Fields.FindField("PriSpanCount"));
        //        _hshPoleFieldIndexes.Add("SecSpanCount", pSupportStructureFC.Fields.FindField("SecSpanCount"));
        //        _hshPoleFieldIndexes.Add("PriDiffAngle", pSupportStructureFC.Fields.FindField("PriDiffAngle"));
        //        _hshPoleFieldIndexes.Add("SecDiffAngle", pSupportStructureFC.Fields.FindField("SecDiffAngle"));

        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception("Error encountered in GetPoleFieldIndexes: " +
        //        ex.Message);
        //    }
        //}

        //private void GetSpanFieldIndexes(IFeatureClass pSpanFC)
        //{
        //    try
        //    {
        //        _hshSpanFieldIndexes = new Hashtable();
        //        _hshSpanFieldIndexes.Add("POLE_GUID", pSpanFC.Fields.FindField("POLE_GUID"));
        //        _hshSpanFieldIndexes.Add("CONDUCTOR_CODE", pSpanFC.Fields.FindField("CONDUCTOR_CODE"));
        //        _hshSpanFieldIndexes.Add("MATERIAL", pSpanFC.Fields.FindField("MATERIAL"));
        //        _hshSpanFieldIndexes.Add("CONDUCTORSIZE", pSpanFC.Fields.FindField("CONDUCTORSIZE"));
        //        _hshSpanFieldIndexes.Add("FROM_RL", pSpanFC.Fields.FindField("FROM_RL"));
        //        _hshSpanFieldIndexes.Add("TO_RL", pSpanFC.Fields.FindField("TO_RL"));
        //        _hshSpanFieldIndexes.Add("ANGLE", pSpanFC.Fields.FindField("ANGLE"));
        //        _hshSpanFieldIndexes.Add("LENGTH", pSpanFC.Fields.FindField("LENGTH"));
        //        _hshSpanFieldIndexes.Add("PRIMARYIDC", pSpanFC.Fields.FindField("PRIMARYIDC"));
        //        _hshSpanFieldIndexes.Add("CONDUCTOROID", pSpanFC.Fields.FindField("CONDUCTOROID"));
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception("Error encountered in GetPoleFieldIndexes: " +
        //        ex.Message);
        //    }
        //}

        private void WriteSpanCSVHeader()
        {
            try
            {
                Shared.WriteToLogfile("Entering WriteSpanCSVHeader");
                //Imran  - Added Pole elevation
                Shared.WriteToSpanCSV("FROM_POLE_GUID,FROM_POLE_RL,TO_POLE_GUID,TO_POLE_RL,CONDUCTOR_GUID,CONDUCTOR_SPAN_LENGTH,CONDUCTOR_CIRCUIT,CONDUCTOR_CLASS,CONDUCTOR_SUBTYPE,CONDUCTOR_ANGLE");
            }
            catch (Exception ex)
            {
                Shared.WriteToLogfile("Error in " + MethodBase.GetCurrentMethod() +
                    " details: " + ex.Message);
                throw new Exception("Error writing span CSV header");
            }
        }

        private void WriteFullDataCSVHeader()
        {
            try
            {
                Shared.WriteToLogfile("Entering WriteFullDataCSVHeader"); 
                //Change by Tina to replace ATTRIBUTENAME to ATTRIBUTE_NM and ATTRIBUTEVALUE to ATTRIBUTE_VAL
                Shared.WriteToFullDataCSV("POLEGUID,PARENTGUID,DEVICEGUID,DEVICETYPE,ATTRIBUTE_NM,ATTRIBUTE_VAL");
            }
            catch (Exception ex)
            {
                Shared.WriteToLogfile("Error in " + MethodBase.GetCurrentMethod() +
                    " details: " + ex.Message);
                throw new Exception("Error encountered in WriteSpanCSVHeader");
            }
        }

        private void WriteSpanListToCSV(List<Span> pSpanList)
        {
            try
            {
                Shared.WriteToLogfile("Entering " + MethodBase.GetCurrentMethod());
                int iPriCnt = 0;
                string sPoleGUID = string.Empty;
                foreach (Span pSpan in pSpanList)
                {
                    //Debug.Print("Span: length: " + pSpan.Length + " to GUID: " + pSpan.ToPoleGUID);
                    sPoleGUID = pSpan.FromPoleGUID;
                    if (pSpan.ConductorClass == "PRIOHCONDUCTOR")
                        iPriCnt++;
                }

                //if (iPriCnt == 0 && sPoleGUID != string.Empty)
                //    Shared.WriteMissingPohDataToCSV(sPoleGUID+",No PRIOHCONDUCTOR found for this Pole");

                foreach (Span pSpan in pSpanList)
                {
                    //Exclude secondary service and pseudo service 
                    //Changed by Tina Add domain value of subtype
                    //if (pSpan.ConductorClass.ToUpper() == "SECOHCONDUCTOR" &&
                    //    ((pSpan.ConductorSubtype == "Service Overhead Conductor")
                    //    /*|| (pSpan.ConductorSubtype == "Streetlight Overhead Conductor") 
                    //    || (pSpan.ConductorSubtype == "Pseudo Service")*/))
                    //{
                    //    Debug.Print("Ignore secondary services and pseudo services"); 
                    //}
                    //else
                    //{
                    Shared.WriteToSpanCSV(
                        pSpan.FromPoleGUID + "," +
                        pSpan.FromPoleRL + "," +
                        pSpan.ToPoleGUID + "," +
                        pSpan.ToPoleRL + "," +
                        pSpan.ConductorGUID + "," +
                        pSpan.Length + "," +
                        pSpan.CircuitId + "," +
                        pSpan.ConductorClass + "," +
                        pSpan.ConductorSubtype + "," +
                        pSpan.SpanAngle);
                    //}
                }
            }
            catch (Exception ex)
            {
                Shared.WriteToLogfile("Error in " + MethodBase.GetCurrentMethod() +
                    " details: " + ex.Message);
                throw ex;
            }
        }

        private void WriteRelatedDevicesToCSV(
            Pole pRefPole,
            List<Span> pSpanList,
            List<Conductor> pNearbyConductors,
            List<RelatedDevice> pAllRelatedDevices)
        {
            try
            {
                Shared.WriteToLogfile("Entering " + MethodBase.GetCurrentMethod());

                //First the conductor information 
                StringBuilder sb = new StringBuilder();
                Conductor pSpanConductor = null;

                foreach (Span pSpan in pSpanList)
                {
                    //Find the conductor using the GUID / CLASS 
                    pSpanConductor = null;
                    foreach (Conductor pConductor in pNearbyConductors)
                    {
                        if (pConductor.GUID == pSpan.ConductorGUID)
                        {
                            pSpanConductor = pConductor;
                            break;
                        }
                    }

                    if (pSpanConductor != null)
                    {
                        //POLEGUID,PARENTGUID,DEVICEGUID,DEVICETYPE,ATTRIBUTENAME,ATTRIBUTEVALUE
                        if (pSpanConductor.FCName.ToUpper() == "PRIOHCONDUCTOR")
                        {
                            for (int i = 1; i <= 4; i++)
                            {
                                sb.Append(pRefPole.GUID);
                                sb.Append(",");
                                sb.Append(string.Empty);
                                sb.Append(",");
                                sb.Append(pSpanConductor.GUID);
                                sb.Append(",");
                                sb.Append(pSpanConductor.FCName.ToUpper());

                                if (i == 1)
                                {
                                    sb.Append(",");
                                    sb.Append("CIRCUITID");
                                    sb.Append(",");
                                    sb.Append(pSpanConductor.CircId);
                                }
                                else if (i == 2)
                                {
                                    sb.Append(",");
                                    sb.Append("CONSTRUCTIONTYPE");
                                    sb.Append(",");
                                    sb.Append(pSpanConductor.ConstructionType);
                                }
                                else if (i == 3)
                                {
                                    sb.Append(",");
                                    sb.Append("OPERATINGVOLTAGE");
                                    sb.Append(",");
                                    sb.Append(pSpanConductor.OperatingVoltage);
                                }
                                else if (i == 4)
                                {
                                    sb.Append(",");
                                    sb.Append("SPACING");
                                    sb.Append(",");
                                    sb.Append(pSpanConductor.Spacing);
                                }
                                Shared.WriteToFullDataCSV(sb.ToString());
                                sb.Clear();

                            }
                        }//Changed by Tina : Write .ToUpper() in below statement
                        else if (pSpanConductor.FCName.ToUpper() == "SECOHCONDUCTOR")
                        {
                            for (int i = 1; i <= 3; i++)
                            {
                                sb.Append(pRefPole.GUID);
                                sb.Append(",");
                                sb.Append(string.Empty);
                                sb.Append(",");
                                sb.Append(pSpanConductor.GUID);
                                sb.Append(",");
                                sb.Append(pSpanConductor.FCName.ToUpper());

                                if (i == 1)
                                {
                                    sb.Append(",");
                                    sb.Append("CIRCUITID");
                                    sb.Append(",");
                                    sb.Append(pSpanConductor.CircId);
                                }
                                else if (i == 2)
                                {
                                    sb.Append(",");
                                    sb.Append("OPERATINGVOLTAGE");
                                    sb.Append(",");
                                    sb.Append(pSpanConductor.OperatingVoltage);
                                }
                                else if (i == 3)//Added by Tina
                                {
                                    sb.Append(",");
                                    sb.Append("SUBTYPECD");
                                    sb.Append(",");
                                    sb.Append(pSpanConductor.Subtype);
                                }
                                Shared.WriteToFullDataCSV(sb.ToString());
                                sb.Clear();

                            }
                        }

                        //Now the ConductorInfo               
                        foreach (ConductorInfo pConductorInfo in pSpanConductor.ConductorInfos)
                        {
                            //POLEGUID,PARENTGUID,DEVICEGUID,DEVICETYPE,ATTRIBUTENAME,ATTRIBUTEVALUE
                            for (int i = 1; i <= 6; i++)
                            {
                                sb.Append(pRefPole.GUID);
                                sb.Append(",");
                                sb.Append(pSpanConductor.GUID);
                                sb.Append(",");
                                sb.Append(pConductorInfo.GUID);
                                sb.Append(",");
                                sb.Append(pConductorInfo.TableName.ToUpper());

                                if (i == 1)
                                {
                                    sb.Append(",");
                                    sb.Append("CONDUCTORSIZE");
                                    sb.Append(",");
                                    sb.Append(pConductorInfo.ConductorSize);
                                }
                                else if (i == 2)
                                {
                                    sb.Append(",");
                                    sb.Append("CONDUCTORUSE");
                                    sb.Append(",");
                                    sb.Append(pConductorInfo.ConductorUse);
                                }
                                else if (i == 3)
                                {
                                    sb.Append(",");
                                    sb.Append("CONDUCTORCOUNT");
                                    sb.Append(",");
                                    sb.Append(pConductorInfo.ConductorCount);
                                }
                                else if (i == 4)//Added by Tina
                                {
                                    sb.Append(",");
                                    sb.Append("CONDUCTORTYPE");
                                    sb.Append(",");
                                    sb.Append(pConductorInfo.ConductorType);
                                }
                                else if (i == 5)
                                {
                                    sb.Append(",");
                                    sb.Append("MATERIAL");
                                    sb.Append(",");
                                    sb.Append(pConductorInfo.Material);
                                }
                                else if (i == 6)
                                {
                                    sb.Append(",");
                                    sb.Append("PHASE");
                                    sb.Append(",");
                                    sb.Append(pConductorInfo.Phase);
                                }

                                //Write the conductor info 
                                Shared.WriteToFullDataCSV(sb.ToString());
                                sb.Clear();
                            }
                        }
                    }
                }


                //Now write the related device information 
                Hashtable hshAttributes = null;
                IObjectClass pDeviceClass = null;

                foreach (RelatedDevice pRelDevice in pAllRelatedDevices)
                {
                    //Get the device row
                    IRow pDeviceRow = null;
                    hshAttributes = (Hashtable)_hshAllAttributes[pRelDevice.DeviceType.ToUpper()];

                    switch (pRelDevice.DeviceType.ToUpper())
                    {
                        case "ANCHORGUY":
                            pDeviceRow = m_AnchorGuyFC.GetFeature(pRelDevice.OId);
                            break;
                        case "CAPACITORBANK":
                            pDeviceRow = m_CapBankFC.GetFeature(pRelDevice.OId);
                            break;
                        case "DYNAMICPROTECTIVEDEVICE":
                            pDeviceRow = m_DPDFC.GetFeature(pRelDevice.OId);
                            break;
                        case "STREETLIGHT":
                            pDeviceRow = m_StreetlightFC.GetFeature(pRelDevice.OId);
                            break;
                        case "TRANSFORMER":
                            pDeviceRow = m_TransformerFC.GetFeature(pRelDevice.OId);
                            break;
                        case "FUSE":
                            pDeviceRow = m_FuseFC.GetFeature(pRelDevice.OId);
                            break;
                        case "JOINTOWNER":
                            pDeviceRow = m_JointOwnerTbl.GetRow(pRelDevice.OId);
                            break;
                        case "SWITCH":
                            pDeviceRow = m_SwitchFC.GetFeature(pRelDevice.OId);
                            break;
                        case "VOLTAGEREGULATOR":
                            pDeviceRow = m_VoltageRegulatorFC.GetFeature(pRelDevice.OId);
                            break;
                        case "PRIMARYRISER":
                            pDeviceRow = m_PrimaryRiserFC.GetFeature(pRelDevice.OId);
                            break;
                        case "SECONDARYRISER":
                            pDeviceRow = m_SecondaryRiserFC.GetFeature(pRelDevice.OId);
                            break;
                        case "ANTENNA":
                            pDeviceRow = m_AntennaFC.GetFeature(pRelDevice.OId);
                            break;
                        case "PHOTOVOLTAICCELL":
                            pDeviceRow = m_PVFC.GetFeature(pRelDevice.OId);
                            break;
                        case "STEPDOWN":
                            pDeviceRow = m_StepDownFC.GetFeature(pRelDevice.OId);
                            break;


                    }


                    if (pDeviceRow != null)
                    {
                        sb.Clear();
                        string deviceGUID = pDeviceRow.get_Value((int)hshAttributes["GLOBALID"]).ToString();
                        foreach (string fieldname in hshAttributes.Keys)
                        {
                            //POLEGUID,PARENTGUID,DEVICEGUID,DEVICETYPE,ATTRIBUTENAME,ATTRIBUTEVALUE 
                            if (fieldname != "GLOBALID")
                            {
                                sb.Append(pRefPole.GUID);
                                sb.Append(",");
                                sb.Append("");
                                sb.Append(",");
                                sb.Append(deviceGUID);
                                sb.Append(",");
                                sb.Append(pRelDevice.DeviceType.ToUpper());
                                sb.Append(",");
                                sb.Append(fieldname.ToUpper());
                                sb.Append(",");
                                sb.Append(GetFieldValue(pDeviceRow, (int)hshAttributes[fieldname]));
                                Shared.WriteToFullDataCSV(sb.ToString());
                                sb.Clear();
                            }
                        }

                        //In the case of a transformer we have to also get the 
                        //transformerunits 
                        if (pRelDevice.DeviceType.ToUpper() == "TRANSFORMER")
                        {
                            ISet pTxUnits = m_TransformerUnitRC.GetObjectsRelatedToObject(
                                (IObject)pDeviceRow);
                            pTxUnits.Reset();
                            IRow pTxUnitRow = (IRow)pTxUnits.Next();
                            while (pTxUnitRow != null)
                            {
                                hshAttributes = (Hashtable)_hshAllAttributes["TRANSFORMERUNIT"];
                                sb.Clear();
                                string txUnitGUID = pTxUnitRow.get_Value((int)hshAttributes["GLOBALID"]).ToString();

                                foreach (string fieldname in hshAttributes.Keys)
                                {
                                    //POLEGUID,PARENTGUID,DEVICEGUID,DEVICETYPE,ATTRIBUTENAME,ATTRIBUTEVALUE 
                                    if (fieldname != "GLOBALID")
                                    {
                                        sb.Append(pRefPole.GUID);
                                        sb.Append(",");
                                        sb.Append(deviceGUID);
                                        sb.Append(",");
                                        sb.Append(txUnitGUID);
                                        sb.Append(",");
                                        sb.Append("TRANSFORMERUNIT");
                                        sb.Append(",");
                                        sb.Append(fieldname.ToUpper());
                                        sb.Append(",");
                                        sb.Append(GetFieldValue(pTxUnitRow, (int)hshAttributes[fieldname]));
                                        Shared.WriteToFullDataCSV(sb.ToString());
                                        sb.Clear();
                                    }
                                }
                                pTxUnitRow = (IRow)pTxUnits.Next();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Shared.WriteToLogfile("Error in " + MethodBase.GetCurrentMethod() +
                    " details: " + ex.Message);
                throw ex;
            }
        }

        //private void WriteRelatedDevicesToDB(
        //    Pole pRefPole,
        //    List<Span> pSpanList,
        //    List<Conductor> pNearbyConductors,
        //    List<RelatedDevice> pAllRelatedDevices)
        //{
        //    try
        //    {
        //        Shared.WriteToLogfile("Entering " + MethodBase.GetCurrentMethod());

        //        if (_pConn == null)
        //        {
        //            //Get output DB connection 
        //            _pConn = new SqlConnection( 
        //                Shared.OutputConnectString);
        //        }
        //        if (_pConn.State != System.Data.ConnectionState.Open)
        //            _pConn.Open();

        //        //Setup the command 
        //        SqlCommand pCmd = _pConn.CreateCommand(); 

        //        //String to hold the SQL insert statement 
        //        StringBuilder sqlStatement = new StringBuilder();
        //        StringBuilder valuesList = new StringBuilder();
        //        StringBuilder fieldList = new StringBuilder();

        //        //POLEGUID,PARENTGUID,DEVICEGUID,DEVICETYPE,ATTRIBUTENAME,ATTRIBUTEVALUE
        //        fieldList.Clear();
        //        fieldList.Append("POLEGUID");
        //        fieldList.Append(",");
        //        fieldList.Append("PARENTGUID");
        //        fieldList.Append(",");
        //        fieldList.Append("DEVICEGUID");
        //        fieldList.Append(",");
        //        fieldList.Append("DEVICETYPE");
        //        fieldList.Append(",");
        //        fieldList.Append("ATTRIBUTE_NM");
        //        fieldList.Append(",");
        //        fieldList.Append("ATTRIBUTE_VAL");

        //        //First the conductor information 
        //        //StringBuilder sb = new StringBuilder();
        //        Conductor pSpanConductor = null;

        //        foreach (Span pSpan in pSpanList)
        //        {
        //            //Find the conductor using the GUID / CLASS 
        //            pSpanConductor = null;
        //            foreach (Conductor pConductor in pNearbyConductors)
        //            {
        //                if (pConductor.GUID == pSpan.ConductorGUID)
        //                {
        //                    pSpanConductor = pConductor;
        //                    break;
        //                }
        //            }

        //            if (pSpanConductor != null)
        //            {
        //                //POLEGUID,PARENTGUID,DEVICEGUID,DEVICETYPE,ATTRIBUTENAME,ATTRIBUTEVALUE
        //                if (pSpanConductor.FCName == "PRIOHCONDUCTOR")
        //                {
        //                    for (int i = 1; i <= 4; i++)
        //                    {
        //                        valuesList.Clear();
        //                        valuesList.Append("'" + pRefPole.GUID + "'");
        //                        valuesList.Append(",");
        //                        valuesList.Append("''");
        //                        valuesList.Append(",");
        //                        valuesList.Append("'" + pSpanConductor.GUID + "'");
        //                        valuesList.Append(",");
        //                        valuesList.Append("'" + pSpanConductor.FCName + "'");

        //                        if (i == 1)
        //                        {
        //                            valuesList.Append(",");
        //                            valuesList.Append("'" + "CIRCUITID" + "'");
        //                            valuesList.Append(",");
        //                            valuesList.Append("'" + pSpanConductor.CircId + "'");
        //                        }
        //                        else if (i == 2)
        //                        {
        //                            valuesList.Append(",");
        //                            valuesList.Append("'" + "CONSTRUCTIONTYPE" + "'");
        //                            valuesList.Append(",");
        //                            valuesList.Append("'" + pSpanConductor.ConstructionType + "'");
        //                        }
        //                        else if (i == 3)
        //                        {
        //                            valuesList.Append(",");
        //                            valuesList.Append("'" + "OPERATINGVOLTAGE" + "'");
        //                            valuesList.Append(",");
        //                            valuesList.Append("'" + pSpanConductor.OperatingVoltage + "'");
        //                        }
        //                        else if (i == 4)
        //                        {
        //                            valuesList.Append(",");
        //                            valuesList.Append("'" + "SPACING" + "'");
        //                            valuesList.Append(",");
        //                            valuesList.Append("'" + pSpanConductor.Spacing + "'");
        //                        }

        //                        //Write the conductor record to the database 
        //                        //Execute the command against the database 
        //                        pCmd.CommandText = BuildSQLInsertStatement(
        //                                    PoleLoadConstants.FULL_DATA_TBL,
        //                                    fieldList.ToString(),
        //                                    valuesList.ToString());
        //                        int recordsUpdated = pCmd.ExecuteNonQuery();
        //                        if (recordsUpdated == 0)
        //                            throw new Exception("SQL insert failed for static data");

        //                }
        //                }//Changed by Tina : Write .ToUpper() in below statement
        //                else if (pSpanConductor.FCName.ToUpper() == "SECOHCONDUCTOR")
        //                {
        //                    for (int i = 1; i <= 3; i++)
        //                    {
        //                        valuesList.Clear();

        //                        valuesList.Append("'" + pRefPole.GUID + "'");
        //                        valuesList.Append(",");
        //                        valuesList.Append("''");
        //                        valuesList.Append(",");
        //                        valuesList.Append("'" + pSpanConductor.GUID + "'");
        //                        valuesList.Append(",");
        //                        valuesList.Append("'" + pSpanConductor.FCName + "'");

        //                        if (i == 1)
        //                        {
        //                            valuesList.Append(",");
        //                            valuesList.Append("'" + "CIRCUITID" + "'");
        //                            valuesList.Append(",");
        //                            valuesList.Append("'" + pSpanConductor.CircId + "'");
        //                        }
        //                        else if (i == 2)
        //                        {
        //                            valuesList.Append(",");
        //                            valuesList.Append("'" + "OPERATINGVOLTAGE" + "'");
        //                            valuesList.Append(",");
        //                            valuesList.Append("'" + pSpanConductor.OperatingVoltage + "'");
        //                        }
        //                        else if (i == 3)//Added by Tina
        //                        {
        //                            valuesList.Append(",");
        //                            valuesList.Append("'" + "SUBTYPECD" + "'");
        //                            valuesList.Append(",");
        //                            valuesList.Append("'" + pSpanConductor.Subtype + "'");
        //                        }


        //                        //Write the conductor record to the database
        //                        //Execute the command against the database 
        //                        pCmd.CommandText = BuildSQLInsertStatement(
        //                                    PoleLoadConstants.FULL_DATA_TBL,
        //                                    fieldList.ToString(),
        //                                    valuesList.ToString());
        //                        int recordsUpdated = pCmd.ExecuteNonQuery();
        //                        if (recordsUpdated == 0)
        //                            throw new Exception("SQL insert failed for static data");

        //                    }
        //                }

        //                //Now the ConductorInfo               
        //                foreach (ConductorInfo pConductorInfo in pSpanConductor.ConductorInfos)
        //                {
        //                    //POLEGUID,PARENTGUID,DEVICEGUID,DEVICETYPE,ATTRIBUTENAME,ATTRIBUTEVALUE
        //                    for (int i = 1; i <= 6; i++)
        //                    {
        //                        valuesList.Clear();

        //                        valuesList.Append("'" + pRefPole.GUID + "'");
        //                        valuesList.Append(",");
        //                        valuesList.Append("'" + pSpanConductor.GUID + "'");
        //                        valuesList.Append(",");
        //                        valuesList.Append("'" + pConductorInfo.GUID + "'");
        //                        valuesList.Append(",");
        //                        valuesList.Append("'" + pConductorInfo.TableName.ToUpper() + "'");

        //                        if (i == 1)
        //                        {
        //                            valuesList.Append(",");
        //                            valuesList.Append("'" + "CONDUCTORSIZE" + "'");
        //                            valuesList.Append(",");
        //                            valuesList.Append("'" + pConductorInfo.ConductorSize + "'");
        //                        }
        //                        else if (i == 2)
        //                        {
        //                            valuesList.Append(",");
        //                            valuesList.Append("'" + "CONDUCTORUSE" + "'");
        //                            valuesList.Append(",");
        //                            valuesList.Append("'" + pConductorInfo.ConductorUse + "'");
        //                        }
        //                        else if (i == 3)
        //                        {
        //                            valuesList.Append(",");
        //                            valuesList.Append("'" + "CONDUCTORCOUNT" + "'");
        //                            valuesList.Append(",");
        //                            valuesList.Append("'" + pConductorInfo.ConductorCount + "'");
        //                        }
        //                        else if (i == 4)//Added by Tina
        //                        {
        //                            valuesList.Append(",");
        //                            valuesList.Append("'" + "CONDUCTORTYPE" + "'");
        //                            valuesList.Append(",");
        //                            valuesList.Append("'" + pConductorInfo.ConductorType + "'");
        //                        }
        //                        else if (i == 5)
        //                        {
        //                            valuesList.Append(",");
        //                            valuesList.Append("'" + "MATERIAL" + "'");
        //                            valuesList.Append(",");
        //                            valuesList.Append("'" + pConductorInfo.Material + "'");
        //                        }
        //                        else if (i == 6)
        //                        {
        //                            valuesList.Append(",");
        //                            valuesList.Append("'" + "PHASE" + "'");
        //                            valuesList.Append(",");
        //                            valuesList.Append("'" + pConductorInfo.Phase + "'");
        //                        }

        //                        //Write the conductor info to DB 
        //                        //Execute the command against the database 
        //                        pCmd.CommandText = BuildSQLInsertStatement(
        //                                    PoleLoadConstants.FULL_DATA_TBL,
        //                                    fieldList.ToString(),
        //                                    valuesList.ToString());
        //                        int recordsUpdated = pCmd.ExecuteNonQuery();
        //                        if (recordsUpdated == 0)
        //                            throw new Exception("SQL insert failed for static data"); 
        //                    }
        //                }
        //            }
        //        }


        //        //Now write the related device information 
        //        Hashtable hshAttributes = null;

        //        foreach (RelatedDevice pRelDevice in pAllRelatedDevices)
        //        {
        //            //Get the device row
        //            IRow pDeviceRow = null;
        //            hshAttributes = (Hashtable)_hshAllAttributes[pRelDevice.DeviceType.ToUpper()];

        //            switch (pRelDevice.DeviceType.ToUpper())
        //            {
        //                case "ANCHORGUY":
        //                    pDeviceRow = m_AnchorGuyFC.GetFeature(pRelDevice.OId);
        //                    break;
        //                case "CAPACITORBANK":
        //                    pDeviceRow = m_CapBankFC.GetFeature(pRelDevice.OId);
        //                    break;
        //                case "DYNAMICPROTECTIVEDEVICE":
        //                    pDeviceRow = m_DPDFC.GetFeature(pRelDevice.OId);
        //                    break;
        //                case "STREETLIGHT":
        //                    pDeviceRow = m_StreetlightFC.GetFeature(pRelDevice.OId);
        //                    break;
        //                case "TRANSFORMER":
        //                    pDeviceRow = m_TransformerFC.GetFeature(pRelDevice.OId);
        //                    break;
        //                case "FUSE":
        //                    pDeviceRow = m_FuseFC.GetFeature(pRelDevice.OId);
        //                    break;
        //                case "JOINTOWNER":
        //                    pDeviceRow = m_JointOwnerTbl.GetRow(pRelDevice.OId);
        //                    break;
        //                case "SWITCH":
        //                    pDeviceRow = m_SwitchFC.GetFeature(pRelDevice.OId);
        //                    break;
        //                case "VOLTAGEREGULATOR":
        //                    pDeviceRow = m_VoltageRegulatorFC.GetFeature(pRelDevice.OId);
        //                    break;
        //            }


        //            if (pDeviceRow != null)
        //            {
        //                string deviceGUID = pDeviceRow.get_Value((int)hshAttributes["GLOBALID"]).ToString();
        //                foreach (string fieldname in hshAttributes.Keys)
        //                {
        //                    //POLEGUID,PARENTGUID,DEVICEGUID,DEVICETYPE,ATTRIBUTENAME,ATTRIBUTEVALUE 
        //                    if (fieldname != "GLOBALID")
        //                    {
        //                        valuesList.Clear();
        //                        valuesList.Append("'" + pRefPole.GUID + "'");
        //                        valuesList.Append(",");
        //                        valuesList.Append("''");
        //                        valuesList.Append(",");
        //                        valuesList.Append("'" + deviceGUID + "'");
        //                        valuesList.Append(",");
        //                        valuesList.Append("'" + pRelDevice.DeviceType.ToUpper() + "'");
        //                        valuesList.Append(",");
        //                        valuesList.Append("'" + fieldname.ToUpper() + "'");
        //                        valuesList.Append(",");
        //                        valuesList.Append("'" + GetFieldValue(pDeviceRow, (int)hshAttributes[fieldname]) + "'");

        //                        //Execute the command against the database 
        //                        pCmd.CommandText = BuildSQLInsertStatement(
        //                                    PoleLoadConstants.FULL_DATA_TBL,
        //                                    fieldList.ToString(),
        //                                    valuesList.ToString());
        //                        int recordsUpdated = pCmd.ExecuteNonQuery();
        //                        if (recordsUpdated == 0)
        //                            throw new Exception("SQL insert failed for static data");
        //                    }
        //                }

        //                //In the case of a transformer we have to also get the 
        //                //transformerunits 
        //                if (pRelDevice.DeviceType.ToUpper() == "TRANSFORMER")
        //                {
        //                    ISet pTxUnits = m_TransformerUnitRC.GetObjectsRelatedToObject(
        //                        (IObject)pDeviceRow);
        //                    pTxUnits.Reset();
        //                    IRow pTxUnitRow = (IRow)pTxUnits.Next();
        //                    while (pTxUnitRow != null)
        //                    {
        //                        hshAttributes = (Hashtable)_hshAllAttributes["TRANSFORMERUNIT"];                                
        //                        string txUnitGUID = pTxUnitRow.get_Value((int)hshAttributes["GLOBALID"]).ToString();

        //                        foreach (string fieldname in hshAttributes.Keys)
        //                        {
        //                            //POLEGUID,PARENTGUID,DEVICEGUID,DEVICETYPE,ATTRIBUTENAME,ATTRIBUTEVALUE 
        //                            if (fieldname != "GLOBALID")
        //                            {
        //                                valuesList.Clear(); 
        //                                valuesList.Append("'" + pRefPole.GUID + "'");
        //                                valuesList.Append(",");
        //                                valuesList.Append("'" + deviceGUID + "'");
        //                                valuesList.Append(",");
        //                                valuesList.Append("'" + txUnitGUID + "'");
        //                                valuesList.Append(",");
        //                                valuesList.Append("'" + "TRANSFORMERUNIT" + "'");
        //                                valuesList.Append(",");
        //                                valuesList.Append("'" + fieldname.ToUpper() + "'");
        //                                valuesList.Append(",");
        //                                valuesList.Append("'" + GetFieldValue(pTxUnitRow, (int)hshAttributes[fieldname]) + "'");

        //                                //Execute the command against the database 
        //                                pCmd.CommandText = BuildSQLInsertStatement( 
        //                                    PoleLoadConstants.FULL_DATA_TBL, 
        //                                    fieldList.ToString(), 
        //                                    valuesList.ToString()); 
        //                                int recordsUpdated = pCmd.ExecuteNonQuery();
        //                                if (recordsUpdated == 0)
        //                                    throw new Exception("SQL insert failed for static data");

        //                            }
        //                        }
        //                        pTxUnitRow = (IRow)pTxUnits.Next();
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Shared.WriteToLogfile("Error in " + MethodBase.GetCurrentMethod() +
        //            " details: " + ex.Message);
        //        throw ex;
        //    }
        //}

        private void WriteEquipmentToDB(
                Pole pRefPole,
                List<RelatedDevice> pAllRelatedDevices)
        {
            try
            {
                Shared.WriteToLogfile("Entering " + MethodBase.GetCurrentMethod());

                if (_pConn == null)
                {
                    //Get output DB connection 
                    _pConn = new SqlConnection(
                        Shared.OutputConnectString);
                }
                if (_pConn.State != System.Data.ConnectionState.Open)
                    _pConn.Open();

                //Setup the command 
                SqlCommand pCmd = _pConn.CreateCommand();
                ISet pSet = null;
                IRow pUnitRow = null; 
                int fldIdx = -1;
                int unitCount = 0; 
                //string unitCount = "0";
                string subtype = string.Empty;
                string direction = string.Empty;
                string angle = string.Empty;
                bool hasUnits = false; 

                //String to hold the SQL insert statement 
                StringBuilder sqlStatement = new StringBuilder();
                StringBuilder valuesList = new StringBuilder();
                StringBuilder fieldList = new StringBuilder();

                //POLEGUID,KEY,COUNT,ANGLE,TYPE,SUBTYPE
                fieldList.Clear();
                fieldList.Append("POLEGUID");
                fieldList.Append(",");
                fieldList.Append("[KEY]");
                fieldList.Append(",");
                fieldList.Append("COUNT");
                fieldList.Append(",");
                fieldList.Append("ANGLE");
                fieldList.Append(",");
                fieldList.Append("TYPE");
                fieldList.Append(",");
                fieldList.Append("SUBTYPE");

                //Write the related device information 
                Hashtable hshAttributes = null;

                //For all related devices that do not have unit info 
                foreach (RelatedDevice pRelDevice in pAllRelatedDevices)
                {
                    //Get the device row
                    hasUnits = false; 
                    IRow pDeviceRow = null;
                    hshAttributes = (Hashtable)_hshAllAttributes[pRelDevice.DeviceType.ToUpper()];

                    switch (pRelDevice.DeviceType)
                    {
                        //Process the devices with units in a separate section                        
                        case "TRANSFORMER":
                            hasUnits = true;
                            hshAttributes = (Hashtable)_hshAllAttributes["TRANSFORMERUNIT"];
                            valuesList.Clear(); 
                            pDeviceRow = m_TransformerFC.GetFeature(pRelDevice.OId);
                            pSet = m_TransformerUnitRC.GetObjectsRelatedToObject((IObject)pDeviceRow);
                            pSet.Reset();
                            pUnitRow = (IRow)pSet.Next(); 
                            while (pUnitRow != null)
                            {
                                //POLEGUID
                                unitCount++; 
                                valuesList.Clear();
                                valuesList.Append("'" + pRefPole.GUID + "'");
                                valuesList.Append(",");

                                //KEY 
                                valuesList.Append("'" + GetEquipmentKey(pUnitRow, pRelDevice.DeviceType) + "'");
                                valuesList.Append(",");

                                //COUNT
                                //unitCount = "1";
                                valuesList.Append("'" + "1" + "'");
                                valuesList.Append(",");

                                //ANGLE - dunno 
                                //angle = "UNKNOWN";
                                valuesList.Append("''");
                                valuesList.Append(",");

                                //TYPE 
                                valuesList.Append("'" + pRelDevice.DeviceType + "'");
                                valuesList.Append(",");

                                //SUBTYPE 
                                subtype = "UNKNOWN";
                                fldIdx = (int)hshAttributes["SUBTYPECD"]; 
                                if (pUnitRow.get_Value((int)hshAttributes["SUBTYPECD"]) != DBNull.Value)
                                    subtype = GetFieldValue(pUnitRow, fldIdx);
                                valuesList.Append("'" + subtype + "'");

                                //Execute the command against the database 
                                pCmd.CommandText = BuildSQLInsertStatement(
                                            PoleLoadConstants.EQUIPMENT_TBL,
                                            fieldList.ToString(),
                                            valuesList.ToString());
                                int recordsUpdated = pCmd.ExecuteNonQuery();
                                if (recordsUpdated == 0)
                                    throw new Exception("SQL insert failed for equipment record");

                                pUnitRow = (IRow)pSet.Next();
                            }                                                         
                            break;
                        case "VOLTAGEREGULATOR":
                            hasUnits = true;
                            hshAttributes = (Hashtable)_hshAllAttributes["VOLTAGEREGULATORUNIT"];
                            valuesList.Clear();
                            pDeviceRow = m_VoltageRegulatorFC.GetFeature(pRelDevice.OId);
                            pSet = m_VoltageRegUnitRC.GetObjectsRelatedToObject((IObject)pDeviceRow);
                            pSet.Reset();
                            pUnitRow = (IRow)pSet.Next();
                            while (pUnitRow != null)
                            {
                                //POLEGUID
                                unitCount++; 
                                valuesList.Clear();
                                valuesList.Append("'" + pRefPole.GUID + "'");
                                valuesList.Append(",");

                                //KEY 
                                valuesList.Append("'" + GetEquipmentKey(pUnitRow, pRelDevice.DeviceType) + "'");
                                valuesList.Append(",");

                                //COUNT
                                //unitCount = "1";
                                valuesList.Append("'" + "1" + "'");
                                valuesList.Append(",");

                                //ANGLE - dunno 
                                //angle = "UNKNOWN";
                                valuesList.Append("''");
                                valuesList.Append(",");

                                //TYPE 
                                valuesList.Append("'" + pRelDevice.DeviceType + "'");
                                valuesList.Append(",");

                                //SUBTYPE 
                                subtype = "UNKNOWN";
                                fldIdx = (int)hshAttributes["SUBTYPECD"];
                                if (pUnitRow.get_Value((int)hshAttributes["SUBTYPECD"]) != DBNull.Value)
                                    subtype = GetFieldValue(pUnitRow, fldIdx);
                                valuesList.Append("'" + subtype + "'");

                                //Execute the command against the database 
                                pCmd.CommandText = BuildSQLInsertStatement(
                                            PoleLoadConstants.EQUIPMENT_TBL,
                                            fieldList.ToString(),
                                            valuesList.ToString());
                                int recordsUpdated = pCmd.ExecuteNonQuery();
                                if (recordsUpdated == 0)
                                    throw new Exception("SQL insert failed for equipment record");


                                pUnitRow = (IRow)pSet.Next();
                            }
                            break;
                        case "STEPDOWN":
                            hasUnits = true;
                            hshAttributes = (Hashtable)_hshAllAttributes["STEPDOWNUNIT"];
                            valuesList.Clear();
                            pDeviceRow = m_StepDownFC.GetFeature(pRelDevice.OId);
                            pSet = m_StepDownUnitRC.GetObjectsRelatedToObject((IObject)pDeviceRow);
                            pSet.Reset();
                            pUnitRow = (IRow)pSet.Next();
                            while (pUnitRow != null)
                            {
                                //POLEGUID 
                                unitCount++; 
                                valuesList.Clear();
                                valuesList.Append("'" + pRefPole.GUID + "'");
                                valuesList.Append(",");

                                //KEY 
                                valuesList.Append("'" + GetEquipmentKey(pUnitRow, pRelDevice.DeviceType) + "'");
                                valuesList.Append(",");

                                //COUNT
                                //unitCount = "1";
                                valuesList.Append("'" + "1" + "'");
                                valuesList.Append(",");

                                //ANGLE - dunno 
                                //angle = "UNKNOWN";
                                valuesList.Append("''");
                                valuesList.Append(",");

                                //TYPE 
                                valuesList.Append("'" + pRelDevice.DeviceType + "'");
                                valuesList.Append(",");

                                //SUBTYPE 
                                subtype = "UNKNOWN";
                                fldIdx = (int)hshAttributes["SUBTYPECD"];
                                if (pUnitRow.get_Value((int)hshAttributes["SUBTYPECD"]) != DBNull.Value)
                                    subtype = GetFieldValue(pUnitRow, fldIdx);
                                valuesList.Append("'" + subtype + "'");

                                //Execute the command against the database 
                                pCmd.CommandText = BuildSQLInsertStatement(
                                            PoleLoadConstants.EQUIPMENT_TBL,
                                            fieldList.ToString(),
                                            valuesList.ToString());
                                int recordsUpdated = pCmd.ExecuteNonQuery();
                                if (recordsUpdated == 0)
                                    throw new Exception("SQL insert failed for equipment record");
                                
                                pUnitRow = (IRow)pSet.Next();
                            }
                            break;
                        case "ANCHORGUY":
                            pDeviceRow = m_AnchorGuyFC.GetFeature(pRelDevice.OId);
                            break;
                        case "CAPACITORBANK":
                            pDeviceRow = m_CapBankFC.GetFeature(pRelDevice.OId);
                            break;
                        case "DYNAMICPROTECTIVEDEVICE":
                            pDeviceRow = m_DPDFC.GetFeature(pRelDevice.OId);
                            break;
                        case "STREETLIGHT":
                            pDeviceRow = m_StreetlightFC.GetFeature(pRelDevice.OId);
                            break;
                        case "OPENPOINT":
                            pDeviceRow = m_OpenPointFC.GetFeature(pRelDevice.OId);
                            break;                        
                        case "FUSE":
                            pDeviceRow = m_FuseFC.GetFeature(pRelDevice.OId);
                            break;
                        case "JOINTOWNER":
                            pDeviceRow = m_JointOwnerTbl.GetRow(pRelDevice.OId);
                            break;
                        case "SWITCH":
                            pDeviceRow = m_SwitchFC.GetFeature(pRelDevice.OId);
                            break;
                        case "PRIMARYRISER":
                            pDeviceRow = m_PrimaryRiserFC.GetFeature(pRelDevice.OId);
                            break;
                        case "SECONDARYRISER":
                            pDeviceRow = m_SecondaryRiserFC.GetFeature(pRelDevice.OId);
                            break;
                        case "ANTENNA":
                            pDeviceRow = m_AntennaFC.GetFeature(pRelDevice.OId);
                            break;
                        case "PHOTOVOLTAICCELL":
                            pDeviceRow = m_PVFC.GetFeature(pRelDevice.OId);
                            break;
                        default:
                            Debug.Print("Equipment type: " + pRelDevice.DeviceType + " is not handled");
                            break;
                    }
                    
                    if ((pDeviceRow != null) && (hasUnits == false))
                    {
                        //POLEGUID 
                        //KEY 
                        //COUNT 
                        //ANGLE 
                        //TYPE 
                        //SUBTYPE 

                        //POLEGUID 
                        valuesList.Clear();
                        valuesList.Append("'" + pRefPole.GUID + "'");
                        valuesList.Append(",");

                        //KEY 
                        valuesList.Append("'" + GetEquipmentKey(pDeviceRow, pRelDevice.DeviceType) + "'");
                        valuesList.Append(",");

                        //COUNT
                        //unitCount = "NOT APPLICABLE"; 
                        valuesList.Append("'" + "1" + "'");
                        valuesList.Append(",");

                        //ANGLE - dunno 
                        angle = "UNKNOWN"; 
                        if (pRelDevice.DeviceType == "ANCHORGUY")
                        {
                            //Logic here to convert direction to an angle 
                            fldIdx = Convert.ToInt32(((Hashtable)_hshAllAttributes 
                                ["ANCHORGUY"])["DIRECTION"]);
                            angle = ConvertDirectionToAngle( 
                                GetFieldValue(pDeviceRow, fldIdx));
                            valuesList.Append("'" + angle + "'");
                        }
                        else
                        {
                            valuesList.Append("''");
                        }
                        valuesList.Append(",");

                        //TYPE 
                        valuesList.Append("'" + pRelDevice.DeviceType + "'");
                        valuesList.Append(",");

                        //SUBTYPE  
                        subtype = "UNKNOWN";
                        fldIdx = (int)hshAttributes["SUBTYPECD"];
                        if (pDeviceRow.get_Value((int)hshAttributes["SUBTYPECD"]) != DBNull.Value)
                            subtype = GetFieldValue(pDeviceRow, fldIdx);
                        valuesList.Append("'" + subtype + "'");

                        //Execute the command against the database 
                        pCmd.CommandText = BuildSQLInsertStatement(
                                    PoleLoadConstants.EQUIPMENT_TBL,
                                    fieldList.ToString(),
                                    valuesList.ToString());
                        int recordsUpdated = pCmd.ExecuteNonQuery();
                        if (recordsUpdated == 0)
                            throw new Exception("SQL insert failed for equipment record");
                    }
                    
                    if ((hasUnits == true) && (unitCount == 0))
                    {
                        Shared.WriteToLogfile("Data error with: " + 
                            pRelDevice.DeviceType + ":" + 
                            pRelDevice.OId.ToString() + " has no related units");
                    } 
                }
            }
            catch (Exception ex)
            {
                Shared.WriteToLogfile("Error in " + MethodBase.GetCurrentMethod() +
                    " details: " + ex.Message);
                throw ex;
            }
        }

        /// <summary>
        /// Converts the heading to an angle where 0 degrees 
        /// represents East and 90 degrees in North etc. 
        /// </summary>
        /// <returns></returns>
        private string ConvertDirectionToAngle(string direction)
        {
            try
            {
                string angle = "UNKNOWN"; 
                switch (direction.ToUpper())
                {
                    case "NORTH":
                        angle = "90"; 
                        break;
                    case "SOUTH":
                        angle = "-90";
                        break;
                    case "EAST":
                        angle = "0"; 
                        break;
                    case "WEST":
                        angle = "180";
                        break;
                    case "NW":
                        angle = "135";
                        break;
                    case "SW":
                        angle = "-135";
                        break;
                    case "NE":
                        angle = "45";
                        break;
                    case "SE":
                        angle = "-45";
                        break;
                    default: 
                        angle = "UNKNOWN";
                        break; 
                }
                return angle; 
            }
            catch (Exception ex)
            {
                Shared.WriteToLogfile("Error in " + MethodBase.GetCurrentMethod() +
                    " details: " + ex.Message);
                throw ex;
            }
        }
        /// <summary>
        /// Returns equipment key for each type of equipment 
        /// </summary>
        /// <returns></returns>
        private string GetEquipmentKey(IRow pEquipRow, string equipType)
        {
            try
            {
                string kva = string.Empty;
                string transformerType = string.Empty;
                StringBuilder key = new StringBuilder(); 
                Hashtable hshAttributes = null;
                string numPhases = string.Empty;
                string opVoltage = string.Empty;
                string suspension = string.Empty; 
                string switchType = string.Empty; 
                string totalKVAR = string.Empty;
                string ratedAmps = string.Empty;
                string direction = string.Empty;
                string anchorSize = string.Empty; 
                string subtype = string.Empty;

                switch (equipType)
                {
                    case "TRANSFORMER":

                        //Equipment Row is now the transformerunit row
                        hshAttributes = (Hashtable)_hshAllAttributes["TRANSFORMERUNIT"];
                        kva = "UNKNOWN";
                        transformerType = "UNKNOWN";

                        if (pEquipRow.get_Value((int)hshAttributes["RATEDKVA"]) != DBNull.Value)
                            kva = GetFieldValue(pEquipRow, (int)hshAttributes["RATEDKVA"]);

                        if (pEquipRow.get_Value((int)hshAttributes["TRANSFORMERTYPE"]) != DBNull.Value)
                            transformerType = pEquipRow.get_Value((int)hshAttributes["TRANSFORMERTYPE"]).ToString();

                        key.Append(
                            "TX" + "_" +
                            "TYPE" + transformerType + "_" +
                            "KVA" + kva);
                        break;
                    case "VOLTAGEREGULATOR":

                        hshAttributes = (Hashtable)_hshAllAttributes["VOLTAGEREGULATORUNIT"];

                        kva = "UNKNOWN";
                        if (pEquipRow.get_Value((int)hshAttributes["RATEDKVA"]) != DBNull.Value)
                            kva = GetFieldValue(pEquipRow, (int)hshAttributes["RATEDKVA"]);
                        ratedAmps = "UNKNOWN";
                        if (pEquipRow.get_Value((int)hshAttributes["RATEDAMPS"]) != DBNull.Value)
                            ratedAmps = pEquipRow.get_Value((int)hshAttributes["RATEDAMPS"]).ToString();
                        key.Append(
                            "VOLTREG" + "_" +
                            "RATEDAMPS" + ratedAmps + "_" +
                            "KVA" + kva);
                        break;

                    case "STEPDOWN":

                        hshAttributes = (Hashtable)_hshAllAttributes["STEPDOWNUNIT"];
                        kva = "UNKNOWN";
                        if (pEquipRow.get_Value((int)hshAttributes["RATEDKVA"]) != DBNull.Value)
                            kva = GetFieldValue(pEquipRow, (int)hshAttributes["RATEDKVA"]);
                        //ratedAmps = "UNKNOWN";
                        //if (pEquipRow.get_Value((int)hshAttributes["RATEDAMPS"]) != DBNull.Value)
                        //    ratedAmps = pEquipRow.get_Value((int)hshAttributes["RATEDAMPS"]).ToString();
                        key.Append(
                            "STEPDOWN" + "_" +
                             "KVA" + kva);
                        break;

                    case "STREETLIGHT":

                        hshAttributes = (Hashtable)_hshAllAttributes["STREETLIGHT"];
                        suspension = "UNKNOWN";
                        if (pEquipRow.get_Value((int)hshAttributes["SUSPENSION"]) != DBNull.Value)
                            suspension = GetFieldValue(pEquipRow, (int)hshAttributes["SUSPENSION"]);
                        key.Append(
                            "SL" + "_" + suspension);
                        break;

                    case "SWITCH":

                        hshAttributes = (Hashtable)_hshAllAttributes["SWITCH"];
                        switchType = "UNKNOWN";
                        if (pEquipRow.get_Value((int)hshAttributes["SWITCHTYPE"]) != DBNull.Value)
                            switchType = GetFieldValue(pEquipRow, (int)hshAttributes["SWITCHTYPE"]);
                        key.Append("SWITCH" + "_" + switchType);
                        break;
                    case "FUSE":

                        hshAttributes = (Hashtable)_hshAllAttributes["FUSE"];
                        numPhases = "UNKNOWN";
                        if (pEquipRow.get_Value((int)hshAttributes["NUMBEROFPHASES"]) != DBNull.Value)
                            numPhases = pEquipRow.get_Value((int)hshAttributes["NUMBEROFPHASES"]).ToString();
                        opVoltage = "UNKNOWN";
                        if (pEquipRow.get_Value((int)hshAttributes["OPERATINGVOLTAGE"]) != DBNull.Value)
                            opVoltage = GetFieldValue(pEquipRow, (int)hshAttributes["OPERATINGVOLTAGE"]);
                        key.Append(
                            "FUSE" + "_" +
                            "PHASES" + numPhases + "_" +
                            "OPVOLT" + opVoltage);
                        break;

                    case "OPENPOINT":

                        hshAttributes = (Hashtable)_hshAllAttributes["OPENPOINT"];
                        opVoltage = "UNKNOWN";
                        if (pEquipRow.get_Value((int)hshAttributes["OPERATINGVOLTAGE"]) != DBNull.Value)
                            opVoltage = GetFieldValue(pEquipRow, (int)hshAttributes["OPERATINGVOLTAGE"]);
                        key.Append(
                            "OP" + "_" +
                            "OPVOLT" + opVoltage);
                        break;

                    case "DYNAMICPROTECTIVEDEVICE":

                        hshAttributes = (Hashtable)_hshAllAttributes["DYNAMICPROTECTIVEDEVICE"];
                        subtype = "UNKNOWN";
                        if (pEquipRow.get_Value((int)hshAttributes["SUBTYPECD"]) != DBNull.Value)
                            subtype = GetFieldValue(pEquipRow, (int)hshAttributes["SUBTYPECD"]);
                        key.Append("DPD" + "_" + subtype.ToUpper()); 

                        break;
                    case "ANCHORGUY":

                        hshAttributes = (Hashtable)_hshAllAttributes["ANCHORGUY"];
                        direction = "UNKNOWN";
                        if (pEquipRow.get_Value((int)hshAttributes["DIRECTION"]) != DBNull.Value)
                            direction = GetFieldValue(pEquipRow, (int)hshAttributes["DIRECTION"]);
                        anchorSize = "UNKNOWN";
                        if (pEquipRow.get_Value((int)hshAttributes["ANCHORSIZE"]) != DBNull.Value)
                            anchorSize = GetFieldValue(pEquipRow, (int)hshAttributes["ANCHORSIZE"]);

                        key.Append(
                            "ANCHORGUY" + "_" +
                            "SIZE" + anchorSize + "_" + 
                            "DIR" + direction); 

                        break;
                    case "CAPACITORBANK":

                        hshAttributes = (Hashtable)_hshAllAttributes["CAPACITORBANK"];
                        //numPhases = "UNKNOWN";
                        //if (pEquipRow.get_Value((int)hshAttributes["NUMBEROFPHASES"]) != DBNull.Value)
                        //    numPhases = pEquipRow.get_Value((int)hshAttributes["NUMBEROFPHASES"]).ToString();
                        totalKVAR = "UNKNOWN";
                        if (pEquipRow.get_Value((int)hshAttributes["TOTALKVAR"]) != DBNull.Value)
                            totalKVAR = GetFieldValue(pEquipRow, (int)hshAttributes["TOTALKVAR"]);
                        key.Append(
                            "CAPBANK" + "_" +
                            "TOTALKVAR" + totalKVAR);
                        break;
                    default:
                        key.Append(equipType);
                        break; 

                    //Not accounted for individually: 
                    //  SecondaryRiser 
                    //  PrimaryRiser 
                    //  Antenna 
                    //  PhotovoltaicCell

                }

                string equipmentKey = key.ToString();
                ReplaceHyphenInKey(ref equipmentKey);
                return equipmentKey;            
            }
            catch (Exception ex)
            {
                Shared.WriteToLogfile("Error in " + MethodBase.GetCurrentMethod() +
                    " details: " + ex.Message);
                throw ex;
            }
        }

        /// <summary>
        /// Builds a SQL Insert Statement 
        /// </summary>
        /// <param name="targetTable"></param>
        /// <param name="fieldList"></param>
        /// <param name="valuesList"></param>
        /// <returns></returns>
        private string BuildSQLInsertStatement( 
            string targetTable, 
            string fieldList, 
            string valuesList)
        {
            try
            {
                //Write the insert statement 
                StringBuilder sqlStatement = new StringBuilder(); 
                sqlStatement.Clear();
                sqlStatement.Append("INSERT INTO " + targetTable + " ");
                sqlStatement.Append("(");
                sqlStatement.Append(fieldList);
                sqlStatement.Append(") ");
                sqlStatement.Append("VALUES ");
                sqlStatement.Append("(");
                sqlStatement.Append(valuesList);
                sqlStatement.Append(")");

                return sqlStatement.ToString(); 
            }
            catch (Exception ex)
            {
                Shared.WriteToLogfile("Error in " + MethodBase.GetCurrentMethod() +
                    " details: " + ex.Message);
                throw ex;
            }
        }

        /// <summary>
        /// Returns a hashtable of existing Pole SapIds so as 
        /// to avoid processing the same pole multiple times 
        /// </summary>
        /// <returns></returns>
        public Hashtable GetExistingPoleSapIds()
        {
            try
            {
                Shared.WriteToLogfile("Entering " + MethodBase.GetCurrentMethod());

                if (_pConn == null)
                {
                    //Get output DB connection 
                    _pConn = new SqlConnection(
                        Shared.OutputConnectString);
                }
                if (_pConn.State != System.Data.ConnectionState.Open)
                    _pConn.Open();

                //Setup the command 
                SqlCommand pCmd = _pConn.CreateCommand();

                //Run an INSERT INTO SELECT statement for the error 
                Hashtable hshExistingPoleSapIds = new Hashtable();
                string sql = "SELECT" + " " + "SAPEQUIPID" + " " + 
                    "FROM" + " " +
                    PoleLoadConstants.POLES_TBL;
                pCmd.CommandText = sql;
                Int32 sapId = -1;
                string sapIdString = string.Empty; 
                SqlDataReader reader = pCmd.ExecuteReader();
                while (reader.Read())
                {
                    sapIdString = reader.GetString(0);
                    if (Int32.TryParse(sapIdString, out sapId))
                    {
                        if (!hshExistingPoleSapIds.ContainsKey(sapId))
                            hshExistingPoleSapIds.Add(sapId, 0);
                    }
                }
                if (reader != null)
                    reader.Close();

                //Return the hashtable 
                return hshExistingPoleSapIds;
            }
            catch (Exception ex)
            {
                Shared.WriteToLogfile("Entering error handler for " + MethodBase.GetCurrentMethod() + " error: " +
                    ex.Message);
                throw new Exception("Error returning existing Pole SapEquipIds");
            }
        }

        private void CacheAttributesForOC(IObjectClass pOC)
        {
            Hashtable hshAttributes = new Hashtable();

            try
            {

                IDataset pDS = (IDataset)pOC;
                string datasetName = GetShortDatasetName(pDS.Name.ToUpper());

                switch (datasetName)
                {
                    case "ANCHORGUY":
                        if (pOC.Fields.FindField("GLOBALID") != -1)
                            hshAttributes.Add("GLOBALID", pOC.Fields.FindField("GLOBALID"));
                        if (pOC.Fields.FindField("DIRECTION") != -1)
                            hshAttributes.Add("DIRECTION", pOC.Fields.FindField("DIRECTION"));
                        if (pOC.Fields.FindField("ANCHORSIZE") != -1)
                            hshAttributes.Add("ANCHORSIZE", pOC.Fields.FindField("ANCHORSIZE"));
                        if (pOC.Fields.FindField("SUBTYPECD") != -1)
                            hshAttributes.Add("SUBTYPECD", pOC.Fields.FindField("SUBTYPECD"));
                        break;
                    case "CAPACITORBANK":
                        if (pOC.Fields.FindField("GLOBALID") != -1)
                            hshAttributes.Add("GLOBALID", pOC.Fields.FindField("GLOBALID"));
                        if (pOC.Fields.FindField("TOTALKVAR") != -1)
                            hshAttributes.Add("TOTALKVAR", pOC.Fields.FindField("TOTALKVAR"));
                        if (pOC.Fields.FindField("NUMBEROFPHASES") != -1)
                            hshAttributes.Add("NUMBEROFPHASES", pOC.Fields.FindField("NUMBEROFPHASES"));
                        if (pOC.Fields.FindField("UNITCOUNT") != -1)
                            hshAttributes.Add("UNITCOUNT", pOC.Fields.FindField("UNITCOUNT"));
                        if (pOC.Fields.FindField("SUBTYPECD") != -1)
                            hshAttributes.Add("SUBTYPECD", pOC.Fields.FindField("SUBTYPECD"));
                        if (pOC.Fields.FindField("CIRCUITID") != -1)
                            hshAttributes.Add("CIRCUITID", pOC.Fields.FindField("CIRCUITID"));
                        break;
                    case "FUSE":
                        if (pOC.Fields.FindField("GLOBALID") != -1)
                            hshAttributes.Add("GLOBALID", pOC.Fields.FindField("GLOBALID"));
                        if (pOC.Fields.FindField("LINKRATING") != -1)
                            hshAttributes.Add("LINKRATING", pOC.Fields.FindField("LINKRATING"));
                        if (pOC.Fields.FindField("LINKTYPE") != -1)
                            hshAttributes.Add("LINKTYPE", pOC.Fields.FindField("LINKTYPE"));
                        if (pOC.Fields.FindField("NUMBEROFPHASES") != -1)
                            hshAttributes.Add("NUMBEROFPHASES", pOC.Fields.FindField("NUMBEROFPHASES"));
                        if (pOC.Fields.FindField("OPERATINGVOLTAGE") != -1)
                            hshAttributes.Add("OPERATINGVOLTAGE", pOC.Fields.FindField("OPERATINGVOLTAGE"));
                        if (pOC.Fields.FindField("SUBTYPECD") != -1)
                            hshAttributes.Add("SUBTYPECD", pOC.Fields.FindField("SUBTYPECD"));
                        if (pOC.Fields.FindField("CIRCUITID") != -1)
                            hshAttributes.Add("CIRCUITID", pOC.Fields.FindField("CIRCUITID"));
                        break;
                    case "JOINTOWNER":
                        if (pOC.Fields.FindField("GLOBALID") != -1)
                            hshAttributes.Add("GLOBALID", pOC.Fields.FindField("GLOBALID"));
                        if (pOC.Fields.FindField("JONAME") != -1)
                            hshAttributes.Add("JONAME", pOC.Fields.FindField("JONAME"));
                        if (pOC.Fields.FindField("SUBTYPECD") != -1)
                            hshAttributes.Add("SUBTYPECD", pOC.Fields.FindField("SUBTYPECD"));
                        if (pOC.Fields.FindField("CIRCUITID") != -1)
                            hshAttributes.Add("CIRCUITID", pOC.Fields.FindField("CIRCUITID"));
                        break;
                    case "DYNAMICPROTECTIVEDEVICE":
                        if (pOC.Fields.FindField("GLOBALID") != -1)
                            hshAttributes.Add("GLOBALID", pOC.Fields.FindField("GLOBALID"));
                        if (pOC.Fields.FindField("NUMBEROFPHASES") != -1)
                            hshAttributes.Add("NUMBEROFPHASES", pOC.Fields.FindField("NUMBEROFPHASES"));
                        if (pOC.Fields.FindField("OPERATINGVOLTAGE") != -1)
                            hshAttributes.Add("OPERATINGVOLTAGE", pOC.Fields.FindField("OPERATINGVOLTAGE"));
                        if (pOC.Fields.FindField("SUBTYPECD") != -1)
                            hshAttributes.Add("SUBTYPECD", pOC.Fields.FindField("SUBTYPECD"));
                        if (pOC.Fields.FindField("CIRCUITID") != -1)
                            hshAttributes.Add("CIRCUITID", pOC.Fields.FindField("CIRCUITID"));
                        break;
                    case "STREETLIGHT":
                        if (pOC.Fields.FindField("GLOBALID") != -1)
                            hshAttributes.Add("GLOBALID", pOC.Fields.FindField("GLOBALID"));
                        if (pOC.Fields.FindField("ELEVATION") != -1)
                            hshAttributes.Add("ELEVATION", pOC.Fields.FindField("ELEVATION"));
                        if (pOC.Fields.FindField("OPERATINGVOLTAGE") != -1)
                            hshAttributes.Add("OPERATINGVOLTAGE", pOC.Fields.FindField("OPERATINGVOLTAGE"));
                        if (pOC.Fields.FindField("SUSPENSION") != -1)
                            hshAttributes.Add("SUSPENSION", pOC.Fields.FindField("SUSPENSION"));
                        if (pOC.Fields.FindField("SUBTYPECD") != -1)
                            hshAttributes.Add("SUBTYPECD", pOC.Fields.FindField("SUBTYPECD"));
                        if (pOC.Fields.FindField("CIRCUITID") != -1)
                            hshAttributes.Add("CIRCUITID", pOC.Fields.FindField("CIRCUITID"));
                        break;
                    case "OPENPOINT":
                        if (pOC.Fields.FindField("GLOBALID") != -1)
                            hshAttributes.Add("GLOBALID", pOC.Fields.FindField("GLOBALID"));
                        if (pOC.Fields.FindField("OPERATINGVOLTAGE") != -1)
                            hshAttributes.Add("OPERATINGVOLTAGE", pOC.Fields.FindField("OPERATINGVOLTAGE"));
                        if (pOC.Fields.FindField("SUBTYPECD") != -1)
                            hshAttributes.Add("SUBTYPECD", pOC.Fields.FindField("SUBTYPECD"));
                        if (pOC.Fields.FindField("CIRCUITID") != -1)
                            hshAttributes.Add("CIRCUITID", pOC.Fields.FindField("CIRCUITID"));
                        break;
                    case "SWITCH":
                        if (pOC.Fields.FindField("GLOBALID") != -1)
                            hshAttributes.Add("GLOBALID", pOC.Fields.FindField("GLOBALID"));
                        if (pOC.Fields.FindField("NUMBEROFPHASES") != -1)
                            hshAttributes.Add("NUMBEROFPHASES", pOC.Fields.FindField("NUMBEROFPHASES"));
                        if (pOC.Fields.FindField("OPERATINGVOLTAGE") != -1)
                            hshAttributes.Add("OPERATINGVOLTAGE", pOC.Fields.FindField("OPERATINGVOLTAGE"));
                        if (pOC.Fields.FindField("SWITCHTYPE") != -1)
                            hshAttributes.Add("SWITCHTYPE", pOC.Fields.FindField("SWITCHTYPE"));
                        if (pOC.Fields.FindField("SUBTYPECD") != -1)
                            hshAttributes.Add("SUBTYPECD", pOC.Fields.FindField("SUBTYPECD"));
                        if (pOC.Fields.FindField("CIRCUITID") != -1)
                            hshAttributes.Add("CIRCUITID", pOC.Fields.FindField("CIRCUITID"));
                        break;
                    case "TRANSFORMER":
                        if (pOC.Fields.FindField("GLOBALID") != -1)
                            hshAttributes.Add("GLOBALID", pOC.Fields.FindField("GLOBALID"));
                        if (pOC.Fields.FindField("UNITCOUNT") != -1)
                            hshAttributes.Add("UNITCOUNT", pOC.Fields.FindField("UNITCOUNT"));
                        if (pOC.Fields.FindField("SUBTYPECD") != -1)
                            hshAttributes.Add("SUBTYPECD", pOC.Fields.FindField("SUBTYPECD"));
                        if (pOC.Fields.FindField("CIRCUITID") != -1)
                            hshAttributes.Add("CIRCUITID", pOC.Fields.FindField("CIRCUITID"));
                        break;
                    case "TRANSFORMERUNIT":
                        if (pOC.Fields.FindField("GLOBALID") != -1)
                            hshAttributes.Add("GLOBALID", pOC.Fields.FindField("GLOBALID"));
                        if (pOC.Fields.FindField("RATEDKVA") != -1)
                            hshAttributes.Add("RATEDKVA", pOC.Fields.FindField("RATEDKVA"));
                        if (pOC.Fields.FindField("PHASEDESIGNATION") != -1)
                            hshAttributes.Add("PHASEDESIGNATION", pOC.Fields.FindField("PHASEDESIGNATION"));
                        if (pOC.Fields.FindField("TRANSFORMERTYPE") != -1)
                            hshAttributes.Add("TRANSFORMERTYPE", pOC.Fields.FindField("TRANSFORMERTYPE"));
                        if (pOC.Fields.FindField("SUBTYPECD") != -1)
                            hshAttributes.Add("SUBTYPECD", pOC.Fields.FindField("SUBTYPECD"));
                        break;
                    case "VOLTAGEREGULATOR":
                        if (pOC.Fields.FindField("GLOBALID") != -1)
                            hshAttributes.Add("GLOBALID", pOC.Fields.FindField("GLOBALID"));
                        if (pOC.Fields.FindField("NUMBEROFPHASES") != -1)
                            hshAttributes.Add("NUMBEROFPHASES", pOC.Fields.FindField("NUMBEROFPHASES"));
                        if (pOC.Fields.FindField("UNITCOUNT") != -1)
                            hshAttributes.Add("UNITCOUNT", pOC.Fields.FindField("UNITCOUNT"));
                        if (pOC.Fields.FindField("OPERATINGVOLTAGE") != -1)
                            hshAttributes.Add("OPERATINGVOLTAGE", pOC.Fields.FindField("OPERATINGVOLTAGE"));
                        if (pOC.Fields.FindField("RATEDAMPS") != -1)
                            hshAttributes.Add("RATEDAMPS", pOC.Fields.FindField("RATEDAMPS"));
                        if (pOC.Fields.FindField("CIRCUITID") != -1)
                            hshAttributes.Add("CIRCUITID", pOC.Fields.FindField("CIRCUITID"));
                        break;
                    case "VOLTAGEREGULATORUNIT":
                        if (pOC.Fields.FindField("GLOBALID") != -1)
                            hshAttributes.Add("GLOBALID", pOC.Fields.FindField("GLOBALID"));
                        if (pOC.Fields.FindField("RATEDKVA") != -1)
                            hshAttributes.Add("RATEDKVA", pOC.Fields.FindField("RATEDKVA"));
                        if (pOC.Fields.FindField("RATEDAMPS") != -1)
                            hshAttributes.Add("RATEDAMPS", pOC.Fields.FindField("RATEDAMPS"));
                        if (pOC.Fields.FindField("SUBTYPECD") != -1)
                            hshAttributes.Add("SUBTYPECD", pOC.Fields.FindField("SUBTYPECD"));
                        break;
                    case "STEPDOWN":
                        if (pOC.Fields.FindField("GLOBALID") != -1)
                            hshAttributes.Add("GLOBALID", pOC.Fields.FindField("GLOBALID"));
                        if (pOC.Fields.FindField("CIRCUITID") != -1)
                            hshAttributes.Add("CIRCUITID", pOC.Fields.FindField("CIRCUITID"));
                        break;
                    case "STEPDOWNUNIT":
                        if (pOC.Fields.FindField("GLOBALID") != -1)
                            hshAttributes.Add("GLOBALID", pOC.Fields.FindField("GLOBALID"));
                        if (pOC.Fields.FindField("RATEDKVA") != -1)
                            hshAttributes.Add("RATEDKVA", pOC.Fields.FindField("RATEDKVA"));
                        if (pOC.Fields.FindField("RATEDAMPS") != -1)
                            hshAttributes.Add("RATEDAMPS", pOC.Fields.FindField("RATEDAMPS"));
                        if (pOC.Fields.FindField("SUBTYPECD") != -1)
                            hshAttributes.Add("SUBTYPECD", pOC.Fields.FindField("SUBTYPECD"));
                        break;
                    case "PRIMARYRISER":
                        if (pOC.Fields.FindField("GLOBALID") != -1)
                            hshAttributes.Add("GLOBALID", pOC.Fields.FindField("GLOBALID"));
                        if (pOC.Fields.FindField("SUBTYPECD") != -1)
                            hshAttributes.Add("SUBTYPECD", pOC.Fields.FindField("SUBTYPECD"));
                        if (pOC.Fields.FindField("CIRCUITID") != -1)
                            hshAttributes.Add("CIRCUITID", pOC.Fields.FindField("CIRCUITID"));
                        break;
                    case "SECONDARYRISER":
                        if (pOC.Fields.FindField("GLOBALID") != -1)
                            hshAttributes.Add("GLOBALID", pOC.Fields.FindField("GLOBALID"));
                        if (pOC.Fields.FindField("SUBTYPECD") != -1)
                            hshAttributes.Add("SUBTYPECD", pOC.Fields.FindField("SUBTYPECD"));
                        if (pOC.Fields.FindField("CIRCUITID") != -1)
                            hshAttributes.Add("CIRCUITID", pOC.Fields.FindField("CIRCUITID"));
                        break;
                    case "ANTENNA":
                        if (pOC.Fields.FindField("GLOBALID") != -1)
                            hshAttributes.Add("GLOBALID", pOC.Fields.FindField("GLOBALID"));
                        if (pOC.Fields.FindField("SUBTYPECD") != -1)
                            hshAttributes.Add("SUBTYPECD", pOC.Fields.FindField("SUBTYPECD"));
                        if (pOC.Fields.FindField("CIRCUITID") != -1)
                            hshAttributes.Add("CIRCUITID", pOC.Fields.FindField("CIRCUITID"));
                        break;
                    case "PHOTOVOLTAICCELL":
                        if (pOC.Fields.FindField("GLOBALID") != -1)
                            hshAttributes.Add("GLOBALID", pOC.Fields.FindField("GLOBALID"));
                        if (pOC.Fields.FindField("SUBTYPECD") != -1)
                            hshAttributes.Add("SUBTYPECD", pOC.Fields.FindField("SUBTYPECD"));
                        if (pOC.Fields.FindField("CIRCUITID") != -1)
                            hshAttributes.Add("CIRCUITID", pOC.Fields.FindField("CIRCUITID"));
                        break;
                    case "PRIOHCONDUCTOR":
                        if (pOC.Fields.FindField("GLOBALID") != -1)
                            hshAttributes.Add("GLOBALID", pOC.Fields.FindField("GLOBALID"));
                        if (pOC.Fields.FindField("SUBTYPECD") != -1)
                            hshAttributes.Add("SUBTYPECD", pOC.Fields.FindField("SUBTYPECD"));
                        if (pOC.Fields.FindField("CIRCUITID") != -1)
                            hshAttributes.Add("CIRCUITID", pOC.Fields.FindField("CIRCUITID"));
                        if (pOC.Fields.FindField("CONSTRUCTIONTYPE") != -1)
                            hshAttributes.Add("CONSTRUCTIONTYPE", pOC.Fields.FindField("CONSTRUCTIONTYPE"));
                        if (pOC.Fields.FindField("OPERATINGVOLTAGE") != -1)
                            hshAttributes.Add("OPERATINGVOLTAGE", pOC.Fields.FindField("OPERATINGVOLTAGE"));
                        if (pOC.Fields.FindField("SPACING") != -1)
                            hshAttributes.Add("SPACING", pOC.Fields.FindField("SPACING"));
                        break;
                    case "SECOHCONDUCTOR":
                        if (pOC.Fields.FindField("GLOBALID") != -1)
                            hshAttributes.Add("GLOBALID", pOC.Fields.FindField("GLOBALID"));
                        if (pOC.Fields.FindField("SUBTYPECD") != -1)
                            hshAttributes.Add("SUBTYPECD", pOC.Fields.FindField("SUBTYPECD"));
                        if (pOC.Fields.FindField("CIRCUITID") != -1)
                            hshAttributes.Add("CIRCUITID", pOC.Fields.FindField("CIRCUITID"));
                        if (pOC.Fields.FindField("OPERATINGVOLTAGE") != -1)
                            hshAttributes.Add("OPERATINGVOLTAGE", pOC.Fields.FindField("OPERATINGVOLTAGE"));
                        break;
                    case "NEUTRALCONDUCTOR":
                        if (pOC.Fields.FindField("GLOBALID") != -1)
                            hshAttributes.Add("GLOBALID", pOC.Fields.FindField("GLOBALID"));
                        if (pOC.Fields.FindField("SUBTYPECD") != -1)
                            hshAttributes.Add("SUBTYPECD", pOC.Fields.FindField("SUBTYPECD"));
                        if (pOC.Fields.FindField("CONDUCTORCOUNT") != -1)
                            hshAttributes.Add("CONDUCTORCOUNT", pOC.Fields.FindField("CONDUCTORCOUNT"));
                        if (pOC.Fields.FindField("CONDUCTORSIZE") != -1)
                            hshAttributes.Add("CONDUCTORSIZE", pOC.Fields.FindField("CONDUCTORSIZE"));
                        if (pOC.Fields.FindField("CONDUCTORUSE") != -1)
                            hshAttributes.Add("CONDUCTORUSE", pOC.Fields.FindField("CONDUCTORUSE"));
                        if (pOC.Fields.FindField("MATERIAL") != -1)
                            hshAttributes.Add("MATERIAL", pOC.Fields.FindField("MATERIAL"));
                        break;
                    case "PRIOHCONDUCTORINFO":
                        if (pOC.Fields.FindField("GLOBALID") != -1)
                            hshAttributes.Add("GLOBALID", pOC.Fields.FindField("GLOBALID"));
                        if (pOC.Fields.FindField("PGE_CONDUCTORCODE") != -1)
                            hshAttributes.Add("PGE_CONDUCTORCODE", pOC.Fields.FindField("PGE_CONDUCTORCODE"));
                        if (pOC.Fields.FindField("CONDUCTORCOUNT") != -1)
                            hshAttributes.Add("CONDUCTORCOUNT", pOC.Fields.FindField("CONDUCTORCOUNT"));
                        if (pOC.Fields.FindField("CONDUCTORSIZE") != -1)
                            hshAttributes.Add("CONDUCTORSIZE", pOC.Fields.FindField("CONDUCTORSIZE"));
                        if (pOC.Fields.FindField("CONDUCTORUSE") != -1)
                            hshAttributes.Add("CONDUCTORUSE", pOC.Fields.FindField("CONDUCTORUSE"));
                        if (pOC.Fields.FindField("CONDUCTORTYPE") != -1)
                            hshAttributes.Add("CONDUCTORTYPE", pOC.Fields.FindField("CONDUCTORTYPE"));
                        if (pOC.Fields.FindField("MATERIAL") != -1)
                            hshAttributes.Add("MATERIAL", pOC.Fields.FindField("MATERIAL"));
                        if (pOC.Fields.FindField("PHASEDESIGNATION") != -1)
                            hshAttributes.Add("PHASEDESIGNATION", pOC.Fields.FindField("PHASEDESIGNATION"));
                        break;
                    case "SECOHCONDUCTORINFO":
                        if (pOC.Fields.FindField("GLOBALID") != -1)
                            hshAttributes.Add("GLOBALID", pOC.Fields.FindField("GLOBALID"));
                        if (pOC.Fields.FindField("CONDUCTORCOUNT") != -1)
                            hshAttributes.Add("CONDUCTORCOUNT", pOC.Fields.FindField("CONDUCTORCOUNT"));
                        if (pOC.Fields.FindField("CONDUCTORSIZE") != -1)
                            hshAttributes.Add("CONDUCTORSIZE", pOC.Fields.FindField("CONDUCTORSIZE"));
                        if (pOC.Fields.FindField("CONDUCTORUSE") != -1)
                            hshAttributes.Add("CONDUCTORUSE", pOC.Fields.FindField("CONDUCTORUSE"));
                        //CONDUCTORTYPE add by Tina
                        if (pOC.Fields.FindField("CONDUCTORTYPE") != -1)
                            hshAttributes.Add("CONDUCTORTYPE", pOC.Fields.FindField("CONDUCTORTYPE"));
                        if (pOC.Fields.FindField("MATERIAL") != -1)
                            hshAttributes.Add("MATERIAL", pOC.Fields.FindField("MATERIAL"));
                        if (pOC.Fields.FindField("PHASEDESIGNATION") != -1)
                            hshAttributes.Add("PHASEDESIGNATION", pOC.Fields.FindField("PHASEDESIGNATION"));
                        break;
                    case "SUPPORTSTRUCTURE":
                        if (pOC.Fields.FindField("SAPEQUIPID") != -1)
                            hshAttributes.Add("SAPEQUIPID", pOC.Fields.FindField("SAPEQUIPID"));
                        if (pOC.Fields.FindField("POLETYPE") != -1)
                            hshAttributes.Add("POLETYPE", pOC.Fields.FindField("POLETYPE"));
                        if (pOC.Fields.FindField("MATERIAL") != -1)
                            hshAttributes.Add("MATERIAL", pOC.Fields.FindField("MATERIAL"));
                        if (pOC.Fields.FindField("HEIGHT") != -1)
                            hshAttributes.Add("HEIGHT", pOC.Fields.FindField("HEIGHT"));
                        if (pOC.Fields.FindField("CLASS") != -1)
                            hshAttributes.Add("CLASS", pOC.Fields.FindField("CLASS"));
                        if (pOC.Fields.FindField("SPECIES") != -1)
                            hshAttributes.Add("SPECIES", pOC.Fields.FindField("SPECIES"));
                        if (pOC.Fields.FindField("FOREIGNATTACHIDC") != -1)
                            hshAttributes.Add("FOREIGNATTACHIDC", pOC.Fields.FindField("FOREIGNATTACHIDC"));
                        if (pOC.Fields.FindField("JOINTCOUNT") != -1)
                            hshAttributes.Add("JOINTCOUNT", pOC.Fields.FindField("JOINTCOUNT"));
                        if (pOC.Fields.FindField("INSTALLJOBYEAR") != -1)
                            hshAttributes.Add("INSTALLJOBYEAR", pOC.Fields.FindField("INSTALLJOBYEAR"));
                        if (pOC.Fields.FindField("INSTALLATIONDATE") != -1)
                            hshAttributes.Add("INSTALLATIONDATE", pOC.Fields.FindField("INSTALLATIONDATE"));
                        if (pOC.Fields.FindField("POLECOUNT") != -1)
                            hshAttributes.Add("POLECOUNT", pOC.Fields.FindField("POLECOUNT"));
                        if (pOC.Fields.FindField("SUBTYPECD") != -1)
                            hshAttributes.Add("SUBTYPECD", pOC.Fields.FindField("SUBTYPECD"));
                        if (pOC.Fields.FindField("STATUS") != -1)
                            hshAttributes.Add("STATUS", pOC.Fields.FindField("STATUS"));
                        if (pOC.Fields.FindField("POLEUSE") != -1)
                            hshAttributes.Add("POLEUSE", pOC.Fields.FindField("POLEUSE"));
                        if (pOC.Fields.FindField("ORIGINALCIRCUMFERENCE") != -1)
                            hshAttributes.Add("ORIGINALCIRCUMFERENCE", pOC.Fields.FindField("ORIGINALCIRCUMFERENCE"));
                        if (pOC.Fields.FindField("EXISTINGREINFORCEMENT") != -1)
                            hshAttributes.Add("EXISTINGREINFORCEMENT", pOC.Fields.FindField("EXISTINGREINFORCEMENT"));
                        if (pOC.Fields.FindField("POLETOPEXTIDC") != -1)
                            hshAttributes.Add("POLETOPEXTIDC", pOC.Fields.FindField("POLETOPEXTIDC"));
                        if (pOC.Fields.FindField("MANUFACTUREDYEAR") != -1)
                            hshAttributes.Add("MANUFACTUREDYEAR", pOC.Fields.FindField("MANUFACTUREDYEAR"));
                        if (pOC.Fields.FindField("GLOBALID") != -1)
                            hshAttributes.Add("GLOBALID", pOC.Fields.FindField("GLOBALID"));
                        if (pOC.Fields.FindField("CUSTOMEROWNED") != -1)
                            hshAttributes.Add("CUSTOMEROWNED", pOC.Fields.FindField("CUSTOMEROWNED"));
                        if (pOC.Fields.FindField("BARCODE") != -1)
                            hshAttributes.Add("BARCODE", pOC.Fields.FindField("BARCODE"));
                        if (pOC.Fields.FindField("CEDSAID") != -1)
                            hshAttributes.Add("CEDSAID", pOC.Fields.FindField("CEDSAID"));
                        if (pOC.Fields.FindField("COMMENTS") != -1)
                            hshAttributes.Add("COMMENTS", pOC.Fields.FindField("COMMENTS"));
                        if (pOC.Fields.FindField("MAXVOLTAGELEVEL") != -1)
                            hshAttributes.Add("MAXVOLTAGELEVEL", pOC.Fields.FindField("MAXVOLTAGELEVEL"));
                        if (pOC.Fields.FindField("DISTMAP") != -1)
                            hshAttributes.Add("DISTMAP", pOC.Fields.FindField("DISTMAP"));
                        if (pOC.Fields.FindField("GPSLATITUDE") != -1)
                            hshAttributes.Add("GPSLATITUDE", pOC.Fields.FindField("GPSLATITUDE"));
                        if (pOC.Fields.FindField("GPSLONGITUDE") != -1)
                            hshAttributes.Add("GPSLONGITUDE", pOC.Fields.FindField("GPSLONGITUDE"));
                        if (pOC.Fields.FindField("LOCDESC1") != -1)
                            hshAttributes.Add("LOCDESC1", pOC.Fields.FindField("LOCDESC1"));
                        if (pOC.Fields.FindField("FUNCTIONALLOCATIONID") != -1)
                            hshAttributes.Add("FUNCTIONALLOCATIONID", pOC.Fields.FindField("FUNCTIONALLOCATIONID"));
                        if (pOC.Fields.FindField("POLENUMBER") != -1)
                            hshAttributes.Add("POLENUMBER", pOC.Fields.FindField("POLENUMBER"));
                        if (pOC.Fields.FindField("REPLACEGUID") != -1)
                            hshAttributes.Add("REPLACEGUID", pOC.Fields.FindField("REPLACEGUID"));
                        break;
                }

                //Add the hashtable of fields 
                _hshAllAttributes.Add(datasetName, hshAttributes);
            }
            catch (Exception ex)
            {
                Shared.WriteToLogfile("Error in " + MethodBase.GetCurrentMethod() +
                    " details: " + ex.Message);
                throw new Exception("Error returning attribute list for related device FC");
            }
        }

        //private void WriteRelatedDeviceToCSV( 
        //    IRelationshipClass pDeviceRC, 
        //    IObjectClass pDeviceOC, 
        //    Hashtable hshAttributes, 
        //    IFeature pSupportStructure, 
        //    string poleGUID, 
        //    string parentGUID)
        //{
        //    try
        //    {
        //        StringBuilder sb = new StringBuilder();
        //        IDataset pDS = (IDataset)pDeviceOC;
        //        string deviceType = GetShortDatasetName(pDS.Name).ToUpper(); 
        //        ISet pDeviceSet = pDeviceRC.GetObjectsRelatedToObject(pSupportStructure);
        //        pDeviceSet.Reset();

        //        IRow pDeviceRow = (IRow)pDeviceSet.Next();
        //        while (pDeviceRow != null)
        //        {
        //            sb.Clear(); 

        //            string deviceGUID = pDeviceRow.get_Value((int)hshAttributes["GLOBALID"]).ToString(); 
        //            foreach (string fieldname in hshAttributes.Keys)
        //            {
        //                //POLEGUID,PARENTGUID,DEVICEGUID,DEVICETYPE,ATTRIBUTENAME,ATTRIBUTEVALUE 
        //                if (fieldname != "GLOBALID")
        //                {
        //                    sb.Append(poleGUID);
        //                    sb.Append(",");
        //                    sb.Append(parentGUID);
        //                    sb.Append(",");
        //                    sb.Append(deviceGUID);
        //                    sb.Append(",");
        //                    sb.Append(deviceType);
        //                    sb.Append(",");
        //                    sb.Append(fieldname.ToUpper());
        //                    sb.Append(",");
        //                    sb.Append(GetFieldValue(pDeviceRow, (int)hshAttributes[fieldname]));
        //                    Shared.WriteToFullDataCSV(sb.ToString());
        //                    sb.Clear();

        //                }
        //            }                    
        //            pDeviceRow = (IRow)pDeviceSet.Next();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Shared.WriteToLogfile("Entering error handler for " + "WriteRelatedDeviceToCSV" + ex.Message);
        //        throw new Exception("Error encountered in WriteRelatedDeviceToCSV: " +
        //        ex.Message);
        //    }
        //}

        //private void WriteTransformerDeviceInfoToCSV(
        //    IRelationshipClass pDeviceRC,
        //    IObjectClass pDeviceOC,
        //    IRelationshipClass pUnitInfoRC,
        //    IObjectClass pUnitInfoOC,
        //    Hashtable hshDeviceAttributes,
        //    Hashtable hshUnitInfoAttributes,
        //    IFeature pSupportStructure,
        //    string poleGUID)
        //{
        //    try
        //    {
        //        StringBuilder sb = new StringBuilder();
        //        IDataset pDS = (IDataset)pDeviceOC;
        //        string deviceType = GetShortDatasetName(pDS.Name).ToUpper();
        //        pDS = (IDataset)pUnitInfoOC;
        //        string unitInfoDeviceType = GetShortDatasetName(pDS.Name).ToUpper();

        //        //ISet pAllDevicesSet = new SetClass(); 
        //        ISet pDeviceSet = pDeviceRC.GetObjectsRelatedToObject(pSupportStructure);
        //        pDeviceSet.Reset();

        //        IRow pDeviceRow = (IRow)pDeviceSet.Next();
        //        while (pDeviceRow != null)
        //        {
        //            sb.Clear(); 

        //            string deviceGUID = pDeviceRow.get_Value((int)hshDeviceAttributes["GLOBALID"]).ToString();
        //            foreach (string fieldname in hshDeviceAttributes.Keys)
        //            {
        //                //POLEGUID,PARENTGUID,DEVICEGUID,DEVICETYPE,ATTRIBUTENAME,ATTRIBUTEVALUE 
        //                if (fieldname != "GLOBALID")
        //                {
        //                    sb.Append(poleGUID);
        //                    sb.Append(",");
        //                    sb.Append(string.Empty);
        //                    sb.Append(",");
        //                    sb.Append(deviceGUID);
        //                    sb.Append(",");
        //                    sb.Append(deviceType);
        //                    sb.Append(",");
        //                    sb.Append(fieldname.ToUpper());
        //                    sb.Append(",");
        //                    sb.Append(GetFieldValue(pDeviceRow, (int)hshDeviceAttributes[fieldname]));
        //                    Shared.WriteToFullDataCSV(sb.ToString());
        //                }
        //            }                    
        //            pDeviceRow = (IRow)pDeviceSet.Next();
        //        }

        //        //Now process the second tier 
        //        pDeviceSet.Reset();
        //        pDeviceRow = (IRow)pDeviceSet.Next();
        //        while (pDeviceRow != null)
        //        {
        //            string parentGUID = pDeviceRow.get_Value((int)hshDeviceAttributes["GLOBALID"]).ToString();
        //            ISet pSecondTierSet = pUnitInfoRC.GetObjectsRelatedToObject((IObject)pDeviceRow);
        //            pSecondTierSet.Reset();
        //            IRow pUnitInfoRow = (IRow)pSecondTierSet.Next();

        //            while (pUnitInfoRow != null)
        //            {
        //                string unitInfoGUID = pUnitInfoRow.get_Value((int)hshUnitInfoAttributes["GLOBALID"]).ToString();

        //                sb.Clear(); 

        //                foreach (string fieldname in hshUnitInfoAttributes.Keys)
        //                {
        //                    //POLEGUID,PARENTGUID,DEVICEGUID,DEVICETYPE,ATTRIBUTENAME,ATTRIBUTEVALUE
        //                    if (fieldname != "GLOBALID")
        //                    {
        //                        sb.Append(poleGUID);
        //                        sb.Append(",");
        //                        sb.Append(parentGUID);
        //                        sb.Append(",");
        //                        sb.Append(unitInfoGUID);
        //                        sb.Append(",");
        //                        sb.Append(unitInfoDeviceType);
        //                        sb.Append(",");
        //                        sb.Append(fieldname.ToUpper());
        //                        sb.Append(",");
        //                        sb.Append(GetFieldValue(pUnitInfoRow, (int)hshUnitInfoAttributes[fieldname]));
        //                        Shared.WriteToFullDataCSV(sb.ToString());
        //                        sb.Clear(); 

        //                    }
        //                }                        
        //                pUnitInfoRow = (IRow)pSecondTierSet.Next();
        //            }
        //            Marshal.FinalReleaseComObject(pSecondTierSet); 


        //            //Go to next device 
        //            pDeviceRow = (IRow)pDeviceSet.Next(); 
        //        }
        //        Marshal.FinalReleaseComObject(pDeviceSet); 
        //    }
        //    catch (Exception ex)
        //    {
        //        Shared.WriteToLogfile("Entering error handler for " + "Write2TierRelatedDeviceToCSV" + ex.Message);
        //        throw new Exception("Error encountered in Write2TierRelatedDeviceToCSV: " +
        //        ex.Message);
        //    }
        //}

        //private void WriteConductorDeviceInfoToCSV(
        //    List<Conductor> pConductorList, 
        //    IFeature pSupportStructure,
        //    string poleGUID)
        //{
        //    try
        //    {
        //        StringBuilder sb = new StringBuilder();                
        //        foreach (Conductor pConductor in pConductorList)
        //        { 
        //            //POLEGUID,PARENTGUID,DEVICEGUID,DEVICETYPE,ATTRIBUTENAME,ATTRIBUTEVALUE
        //            if (pConductor.FCName.ToUpper() == "PRIOHCONDUCTOR")
        //            {
        //                for (int i = 1; i <= 4; i++) 
        //                {
        //                    sb.Append(poleGUID); 
        //                    sb.Append(",");
        //                    sb.Append(string.Empty); 
        //                    sb.Append(",");
        //                    sb.Append(pConductor.GUID);
        //                    sb.Append(",");
        //                    sb.Append(pConductor.FCName.ToUpper());

        //                    if (i == 1) 
        //                    {
        //                        sb.Append(",");
        //                        sb.Append("CIRCUITID");
        //                        sb.Append(",");
        //                        sb.Append(pConductor.CircId);
        //                    }
        //                    else if (i == 2) 
        //                    {
        //                        sb.Append(",");
        //                        sb.Append("CONSTRUCTIONTYPE");
        //                        sb.Append(",");
        //                        sb.Append(pConductor.ConstructionType);
        //                    }
        //                    else if (i == 3) 
        //                    {
        //                        sb.Append(",");
        //                        sb.Append("OPERATINGVOLTAGE");
        //                        sb.Append(",");
        //                        sb.Append(pConductor.OperatingVoltage);
        //                    }
        //                    else if (i == 4)
        //                    {
        //                        sb.Append(",");
        //                        sb.Append("SPACING");
        //                        sb.Append(",");
        //                        sb.Append(pConductor.Spacing);
        //                    }
        //                    Shared.WriteToFullDataCSV(sb.ToString());
        //                    sb.Clear(); 

        //                }
        //            }//Changed by Tina : Write .ToUpper() in below statement
        //            else if (pConductor.FCName.ToUpper() == "SECOHCONDUCTOR") 
        //            {
        //                for (int i = 1; i <= 3; i++)
        //                {
        //                    sb.Append(poleGUID);
        //                    sb.Append(",");
        //                    sb.Append(string.Empty);
        //                    sb.Append(",");
        //                    sb.Append(pConductor.GUID);
        //                    sb.Append(",");
        //                    sb.Append(pConductor.FCName.ToUpper());

        //                    if (i == 1)
        //                    {
        //                        sb.Append(",");
        //                        sb.Append("CIRCUITID");
        //                        sb.Append(",");
        //                        sb.Append(pConductor.CircId);
        //                    }
        //                    else if (i == 2)
        //                    {
        //                        sb.Append(",");
        //                        sb.Append("OPERATINGVOLTAGE");
        //                        sb.Append(",");
        //                        sb.Append(pConductor.OperatingVoltage);
        //                    }
        //                    else if (i == 3)//Added by Tina
        //                    {
        //                        sb.Append(",");
        //                        sb.Append("SUBTYPECD");
        //                        sb.Append(",");
        //                        sb.Append(pConductor.Subtype);
        //                    }
        //                    Shared.WriteToFullDataCSV(sb.ToString());
        //                    sb.Clear(); 

        //                }
        //            }

        //            //Now the ConductorInfo               
        //            foreach (ConductorInfo pConductorInfo in pConductor.ConductorInfos) 
        //            {
        //                //POLEGUID,PARENTGUID,DEVICEGUID,DEVICETYPE,ATTRIBUTENAME,ATTRIBUTEVALUE
        //                for (int i = 1; i <= 6; i++)
        //                {
        //                    sb.Append(poleGUID);
        //                    sb.Append(",");
        //                    sb.Append(pConductor.GUID);
        //                    sb.Append(",");
        //                    sb.Append(pConductorInfo.GUID);
        //                    sb.Append(",");
        //                    sb.Append(pConductorInfo.TableName.ToUpper());

        //                    if (i == 1)
        //                    {
        //                        sb.Append(",");
        //                        sb.Append("CONDUCTORSIZE");
        //                        sb.Append(",");
        //                        sb.Append(pConductorInfo.ConductorSize);
        //                    }
        //                    else if (i == 2)
        //                    {
        //                        sb.Append(",");
        //                        sb.Append("CONDUCTORUSE");
        //                        sb.Append(",");
        //                        sb.Append(pConductorInfo.ConductorUse);
        //                    }
        //                    else if (i == 3)
        //                    {
        //                        sb.Append(",");
        //                        sb.Append("CONDUCTORCOUNT");
        //                        sb.Append(",");
        //                        sb.Append(pConductorInfo.ConductorCount);
        //                    }
        //                    else if (i == 4)//Added by Tina
        //                    {
        //                        sb.Append(",");
        //                        sb.Append("CONDUCTORTYPE");
        //                        sb.Append(",");
        //                        sb.Append(pConductorInfo.ConductorType);
        //                    }
        //                    else if (i == 5)
        //                    {
        //                        sb.Append(",");
        //                        sb.Append("MATERIAL");
        //                        sb.Append(",");
        //                        sb.Append(pConductorInfo.Material);
        //                    }
        //                    else if (i == 6)
        //                    {
        //                        sb.Append(",");
        //                        sb.Append("PHASE");
        //                        sb.Append(",");
        //                        sb.Append(pConductorInfo.Phase);
        //                    }

        //                    Shared.WriteToFullDataCSV(sb.ToString());
        //                    sb.Clear();

        //                }
        //            }                    
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Shared.WriteToLogfile("Entering error handler for " + "Write2TierRelatedDeviceToCSV" + ex.Message);
        //        throw new Exception("Error encountered in Write2TierRelatedDeviceToCSV: " +
        //        ex.Message);
        //    }
        //}

        private string GetFieldValue(IRow pRow, int fieldIdx)
        {
            try
            {
                string fieldValue = string.Empty;
                ISubtypes subtypes = (ISubtypes)pRow.Table;
                IDomain pDomain = null;

                pDomain = pRow.Fields.get_Field(fieldIdx).Domain;
                if ((subtypes.HasSubtype) && (pDomain == null))
                {
                    int subtypeCode = Convert.ToInt32(pRow.get_Value(subtypes.SubtypeFieldIndex));
                    pDomain = subtypes.get_Domain(subtypeCode, pRow.Fields.get_Field(fieldIdx).Name);

                    if (fieldIdx == subtypes.SubtypeFieldIndex)
                    {
                        //Give the subtype name not the subtype code 
                        if (pRow.get_Value(fieldIdx) != DBNull.Value)
                        {
                            int subTypeCode = Convert.ToInt32(pRow.get_Value(fieldIdx));
                            IRowSubtypes rowSubtypes = (IRowSubtypes)pRow;
                            fieldValue = subtypes.get_SubtypeName(subTypeCode);
                        }
                        else
                            fieldValue = "<NULL>";

                        return fieldValue;
                    }
                }

                if (pDomain == null)
                {
                    if (pRow.get_Value(fieldIdx) == DBNull.Value)
                    {
                        fieldValue = "<NULL>";
                    }
                    else
                    {
                        fieldValue = pRow.get_Value(fieldIdx).ToString();
                    }
                }
                else
                {
                    //Get the domain description 
                    object theVal = pRow.get_Value(fieldIdx);
                    //IDataset pDS = (IDataset)pRow.Table;
                    //string datasetName = GetShortDatasetName(pDS.Name).ToUpper(); 

                    //Hashtable hshDomain = (Hashtable)_hshAllDomains[
                    //    datasetName + "." + 
                    //    pRow.Fields.get_Field(fieldIdx).Name.ToUpper()];
                    //if (hshDomain.ContainsKey(theVal))
                    //    fieldValue = hshDomain[theVal].ToString(); 
                    //else
                    //    fieldValue = pRow.get_Value(fieldIdx).ToString(); 

                    if (pDomain is ICodedValueDomain)
                    {
                        ICodedValueDomain pCodedValueDomain = (ICodedValueDomain)pDomain;
                        int codeCount = pCodedValueDomain.CodeCount;
                        //Loop through the list of values and print their names
                        for (int i = 0; i < codeCount; i++)
                        {
                            //Console.WriteLine("{0} : {1)", pCodedValueDomain.get_Value(i).ToString(), pCodedValueDomain.get_Name(i));
                            if (pCodedValueDomain.get_Value(i).ToString() == theVal.ToString())
                            {
                                fieldValue = pCodedValueDomain.get_Name(i);
                                break;
                            }
                        }
                    }
                    else
                    {
                        fieldValue = theVal.ToString();
                    }
                }
                return fieldValue;
            }
            catch (Exception ex)
            {
                Shared.WriteToLogfile("Error in " + MethodBase.GetCurrentMethod() +
                    " details: " + ex.Message);
                throw new Exception("Error returning field value: " + ex.Message);
            }
        }

        private string GetDomainDescriptionFromDomainCode(IField pField, object theVal)
        {
            try
            {
                ICodedValueDomain pCodedValueDomain = (ICodedValueDomain)pField.Domain;
                int codeCount = pCodedValueDomain.CodeCount;
                string domainDescrip = string.Empty;
                //Loop through the list of values and print their names
                for (int i = 0; i < codeCount; i++)
                {

                    if (pCodedValueDomain.get_Value(i).ToString() == theVal.ToString())
                    {
                        domainDescrip = pCodedValueDomain.get_Name(i).ToString();
                        break;
                    }
                }
                return domainDescrip;
            }
            catch (Exception ex)
            {
                Shared.WriteToLogfile("Error in " + MethodBase.GetCurrentMethod() +
                    " details: " + ex.Message);
                throw new Exception("Error returning domain description: " + ex.Message);
            }

        }
        private string GetShortDatasetName(string datasetName)
        {
            try
            {
                string shortDatasetName = "";
                int posOfLastPeriod = -1;

                posOfLastPeriod = datasetName.LastIndexOf(".");
                if (posOfLastPeriod != -1)
                {
                    shortDatasetName = datasetName.Substring(
                        posOfLastPeriod + 1, (datasetName.Length - posOfLastPeriod) - 1);
                }
                else
                {
                    shortDatasetName = datasetName;
                }

                return shortDatasetName;
            }
            catch (Exception ex)
            {
                Shared.WriteToLogfile("Error in " + MethodBase.GetCurrentMethod() +
                    " details: " + ex.Message);
                throw new Exception("Error returning the shortened dataset name");
            }
        }

        private int GetFieldIndexForUpdate(Hashtable hshFieldIndexes, string param, int index)
        {
            try
            {
                string key = "Span" + index.ToString() + param;
                int fldIdx = (int)hshFieldIndexes[key];
                return fldIdx;
            }
            catch (Exception ex)
            {
                Shared.WriteToLogfile("Error in " + MethodBase.GetCurrentMethod() +
                    " details: " + ex.Message);
                throw new Exception("Error encountered in GetFieldIndexForUpdate");
            }
        }

        private IPolyline GetPolylineFromLine(ILine pLine, ISpatialReference pSR)
        {
            try
            {
                object obj = Type.Missing;
                ISegmentCollection segCollection = new PolylineClass() as ISegmentCollection;
                segCollection.AddSegment((ISegment)pLine, ref obj, ref obj);

                //Set the spatial reference on the new polyline

                //The spatial reference is not transfered automatically from the segments

                IGeometry geom = segCollection as IGeometry;

                geom.SpatialReference = pSR;

                //Can now be used with ITopologicalOperator methods
                return (IPolyline)geom;
            }
            catch (Exception ex)
            {
                Shared.WriteToLogfile("Error in " + MethodBase.GetCurrentMethod() +
                    " details: " + ex.Message);
                throw new Exception("Error encountered in CalculatePoleSpans: " +
                ex.Message);
            }
        }

        //private static IFeatureIndex CreateFeatureIndex(IFeatureClass pFeatureClass)
        //{
        //    IFeatureIndex fi = new FeatureIndexClass();
        //    try
        //    {

        //        fi.FeatureClass = pFeatureClass;
        //        //create the index
        //        fi.Index(null, ((IGeoDataset)pFeatureClass).Extent);
        //    }
        //    catch (Exception ex)
        //    {
        //        fi = null;
        //    }
        //    return fi;
        //}

        /* private static void FindNearest(IGeometry g, IFeatureClass fc, IFeatureIndex fi, out int objectId, out double distanceLength, string where = "")
         {
             IQueryFilter qfilter = null;
             if (!string.IsNullOrEmpty(where))
             {
                 qfilter = new QueryFilterClass();
                 qfilter.WhereClause = where;
                 IFeatureCursor fcursor = fc.Search(qfilter, true);
                 fi.FeatureCursor = fcursor;
             }
             int fid;
             double distance;
             ((IIndexQuery)fi).NearestFeature(g, out fid, out distance);

             objectId = fid;
             distanceLength = distance;

         }*/


        //private IFeature FindAdjacentPole(
        //    IPoint pExistingPolePoint,
        //    IPolygon pRayPolygon,
        //    IFeatureClass pSupportStructureFC,
        //    int oidOfExistingPole,
        //    string circuitid//,
        //                    //IFeatureIndex fi,
        //                    //IGeometry pGp
        //    )
        //{
        //    ITopologicalOperator kTopoOH = null;
        //    IPolygon pSearchPolyOH = null;
        //    //string sFound = "NO";
        //    //int fid = 0;
        //    // double distance = 0.0;

        //    try
        //    {
        //        //code to replace nearest pole search with feature index logic to increase the performance
        //        // IFeatureIndex ifxidx = CreateFeatureIndex(pSupportStructureFC);

        //        /*  ((IIndexQuery)fi).NearestFeature(pGp, out fid, out distance);

        //          IFeature pAdjacentPole = null;
        //          IFeature pAdjacentPoleNull = null;

        //          if(distance > 0)
        //              pAdjacentPole = pSupportStructureFC.GetFeature(fid);


        //          pAdjacentPole = pSupportStructureFC.GetFeature(fid);
        //         */

        //        IProximityOperator pPO = (IProximityOperator)pExistingPolePoint;
        //        ISpatialFilter pSF = new SpatialFilterClass();
        //        pSF.Geometry = pRayPolygon;
        //        pSF.SpatialRel = esriSpatialRelEnum.esriSpatialRelIntersects;
        //        IFeatureCursor pFCursor = pSupportStructureFC.Search(pSF, false);
        //        IFeature pPoleFeature = pFCursor.NextFeature();
        //        double distAway = 0;
        //        double closestDistAway = 5000;
        //        IFeature pAdjacentPole = null;
        //        IFeature pAdjacentPoleNull = null;


        //        while (pPoleFeature != null)
        //        {
        //            if (pPoleFeature.OID != oidOfExistingPole)
        //            {
        //                distAway = pPO.ReturnDistance(pPoleFeature.ShapeCopy);
        //                if (distAway < closestDistAway)
        //                {
        //                    pAdjacentPole = pPoleFeature;
        //                    closestDistAway = distAway;
        //                }
        //            }
        //            pPoleFeature = pFCursor.NextFeature();
        //        }


        //        //if ((pAdjacentPole != null) && distance <= 5000)
        //        if (pAdjacentPole != null)
        //        {
        //            Debug.Print("found adjacent pole at distance: " + closestDistAway.ToString());
        //            //Debug.Print("found adjacent pole at distance: " + distance.ToString());  


        //            /*     //Circuit match logic
        //               //Search in PRIOHCONDUCTOR Feature class
        //               IFeatureWorkspace pFileGDB_FWS_POH = (IFeatureWorkspace)Shared.GetWorkspaceByName("output");
        //               IFeatureWorkspace pFWS_POH = (IFeatureWorkspace)Shared.GetWorkspaceByName("electric");
        //               IFeatureClass pPriOHConductor = pFWS_POH.OpenFeatureClass("edgis.priohconductor");

        //               ISpatialFilter pSF_POH = new SpatialFilterClass();
        //               kTopoOH = (ITopologicalOperator)pAdjacentPole.ShapeCopy;
        //               pSearchPolyOH = (IPolygon)kTopoOH.Buffer(Shared.PoleBuffer);
        //               pSF_POH.Geometry = pSearchPolyOH;
        //               pSF_POH.SpatialRel = esriSpatialRelEnum.esriSpatialRelIntersects;
        //               pSF_POH.GeometryField = "SHAPE";
        //               IFeatureCursor pFCursorPOH = pPriOHConductor.Search(pSF_POH, false);

        //               IFeature pPOHFeature = pFCursorPOH.NextFeature();

        //               while (pPOHFeature != null)
        //               {
        //                   if (circuitid == pPOHFeature.get_Value(pPOHFeature.Fields.FindField("CIRCUITID")).ToString())
        //                       sFound = "YES";
        //                   pPOHFeature = pFCursorPOH.NextFeature();
        //               }
        //               Marshal.FinalReleaseComObject(pFCursorPOH); 

        //               //Search in SECOHCONDUCTOR Feature class
        //               if (sFound == "NO")
        //               {
        //                   IFeatureWorkspace pFileGDB_FWS_SOH = (IFeatureWorkspace)Shared.GetWorkspaceByName("output");
        //                   IFeatureWorkspace pFWS_SOH = (IFeatureWorkspace)Shared.GetWorkspaceByName("electric");
        //                   IFeatureClass pSecOHConductor = pFWS_SOH.OpenFeatureClass("edgis.secohconductor");

        //                   ISpatialFilter pSF_SOH = new SpatialFilterClass();
        //                   kTopoOH = (ITopologicalOperator)pAdjacentPole.ShapeCopy;
        //                   pSearchPolyOH = (IPolygon)kTopoOH.Buffer(Shared.PoleBuffer);
        //                   pSF_POH.Geometry = pSearchPolyOH;
        //                   pSF_POH.SpatialRel = esriSpatialRelEnum.esriSpatialRelIntersects;
        //                   pSF_POH.GeometryField = "SHAPE";
        //                   IFeatureCursor pFCursorSOH = pSecOHConductor.Search(pSF_POH, false);

        //                   IFeature pSOHFeature = pFCursorSOH.NextFeature();

        //                   while (pSOHFeature != null)
        //                   {
        //                       if (circuitid == pSOHFeature.get_Value(pSOHFeature.Fields.FindField("CIRCUITID")).ToString())
        //                           sFound = "YES";
        //                       pSOHFeature = pFCursorSOH.NextFeature();
        //                   }
        //                   Marshal.FinalReleaseComObject(pFCursorSOH); 
                  

        //               } */

        //        }

        //        /* if (sFound == "NO")
        //         {
        //             Marshal.FinalReleaseComObject(pFCursor);
        //             return pAdjacentPoleNull;
        //         }
        //         else
        //         {*/
        //        Marshal.FinalReleaseComObject(pFCursor);
        //        return pAdjacentPole;
        //        //  }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception("Error encountered in FindAdjacentPole: " +
        //        ex.Message);
        //    }
        //}

        private List<RelatedDevice> GetRelatedDevices(
            IFeature pPoleFeature,
            IRelationshipClass pRelatedDeviceRC,
            ref List<RelatedDevice> pRelatedDevices)
        {
            try
            {
                //Shared.WriteToLogfile("Entering " + MethodBase.GetCurrentMethod());
                ISet pSet = null;
                IRow pDeviceRow = null;
                string relDeviceFCName = string.Empty;
                IPoint pDevicePoint = null;
                //int fldIdx = -1;
                string circId = string.Empty;
                //Hashtable hshAttributes = null;  
                
                pSet = pRelatedDeviceRC.GetObjectsRelatedToObject((IObject)pPoleFeature);
                pDeviceRow = (IRow)pSet.Next();
                while (pDeviceRow != null)
                {
                    pDevicePoint = null;
                    circId = string.Empty; 
                    relDeviceFCName = GetShortDatasetName(((IDataset)pDeviceRow.Table).Name.ToUpper());
                    if (pDeviceRow is IFeature)
                        pDevicePoint = (IPoint)((IFeature)pDeviceRow).ShapeCopy;

                    //hshAttributes = (Hashtable)_hshAllAttributes[relDeviceFCName];
                    //if (hshAttributes.ContainsKey("CIRCUITID"))
                    //{
                    //    fldIdx = (int)((Hashtable)_hshAllAttributes[relDeviceFCName])["CIRCUITID"];
                    //    if (pDeviceRow.get_Value(fldIdx) != DBNull.Value)
                    //        circId = pDeviceRow.get_Value(fldIdx).ToString();
                    //}
                    RelatedDevice pRelDev = new RelatedDevice(
                                                relDeviceFCName,
                                                pDevicePoint,
                                                pDeviceRow.OID);
                    pRelatedDevices.Add(pRelDev);
                    pDeviceRow = (IRow)pSet.Next();
                }

                return null;

            }
            catch (Exception ex)
            {
                Shared.WriteToLogfile("Error in " + MethodBase.GetCurrentMethod() + 
                    " details: " + ex.Message);
                throw ex;
            }
        }

        private void InitializeData()
        {
            try
            {
                Shared.WriteToLogfile("Entering " + MethodBase.GetCurrentMethod());

                //Initially load up all the required data                 
                IFeatureWorkspace pElectricFWS = (IFeatureWorkspace)Shared.GetWorkspaceByName("electric");
                IFeatureWorkspace pLocalFWS = (IFeatureWorkspace)Shared.GetWorkspaceByName("output");

                if (_hshAllAttributes == null)
                    _hshAllAttributes = new Hashtable();
                //if (_hshAllDomains == null)
                //    _hshAllDomains = new Hashtable();

                //Landbase Featureclasses

                m_SnowLoadingFC = pLocalFWS.OpenFeatureClass("ElecSnowLoadingArea");
                m_PopDensityFC = pLocalFWS.OpenFeatureClass("PopDens_2010");
                m_ClimateZoneFC = pLocalFWS.OpenFeatureClass("ElecClimateZone");
                m_CorrosionAreaFC = pLocalFWS.OpenFeatureClass("ElecCorrosionArea");
                m_RCZ_FC = pLocalFWS.OpenFeatureClass("PGE_RCZ");
                m_SurgeProtFC = pLocalFWS.OpenFeatureClass("SurgeProtectionDistricts");
                m_InsulationFC = pLocalFWS.OpenFeatureClass("InsulationAreas");
                m_UWF_FC = pLocalFWS.OpenFeatureClass("PGE_UWF");
                m_PeakWindFC = pLocalFWS.OpenFeatureClass("HistoricPeakWind");

                //Device Featureclasses

                m_SupportStructureFC = pElectricFWS.OpenFeatureClass("edgis.supportstructure");
                CacheAttributesForOC(m_SupportStructureFC);

                m_PriOHConductorFC = pElectricFWS.OpenFeatureClass("edgis.priohconductor");
                CacheAttributesForOC(m_PriOHConductorFC);

                m_PriOHConductorInfoTbl = pElectricFWS.OpenTable("edgis.priohconductorinfo");
                CacheAttributesForOC((IObjectClass)m_PriOHConductorInfoTbl);

                m_SecOHConductorFC = pElectricFWS.OpenFeatureClass("edgis.secohconductor");
                CacheAttributesForOC(m_SecOHConductorFC);

                m_SecOHConductorInfoTbl = pElectricFWS.OpenTable("edgis.secohconductorinfo");
                CacheAttributesForOC((IObjectClass)m_SecOHConductorInfoTbl);

                m_NeutralConductorFC = pElectricFWS.OpenFeatureClass("edgis.neutralconductor");
                CacheAttributesForOC(m_NeutralConductorFC);

                m_TransformerFC = pElectricFWS.OpenFeatureClass("edgis.transformer");
                CacheAttributesForOC(m_TransformerFC);

                m_TransformerUnitTbl = pElectricFWS.OpenTable("edgis.transformerunit");
                CacheAttributesForOC((IObjectClass)m_TransformerUnitTbl); 

                m_AnchorGuyFC = pElectricFWS.OpenFeatureClass("edgis.anchorguy");
                CacheAttributesForOC((IObjectClass)m_AnchorGuyFC);

                m_CapBankFC = pElectricFWS.OpenFeatureClass("edgis.capacitorbank");
                CacheAttributesForOC((IObjectClass)m_CapBankFC);

                m_DPDFC = pElectricFWS.OpenFeatureClass("edgis.dynamicprotectivedevice");
                CacheAttributesForOC((IObjectClass)m_DPDFC);

                m_JointOwnerTbl = pElectricFWS.OpenTable("edgis.jointowner");
                CacheAttributesForOC((IObjectClass)m_JointOwnerTbl);

                m_StreetlightFC = pElectricFWS.OpenFeatureClass("edgis.streetlight");
                CacheAttributesForOC((IObjectClass)m_StreetlightFC);

                m_OpenPointFC = pElectricFWS.OpenFeatureClass("edgis.openpoint");
                CacheAttributesForOC((IObjectClass)m_OpenPointFC);

                m_FuseFC = pElectricFWS.OpenFeatureClass("edgis.Fuse");
                CacheAttributesForOC((IObjectClass)m_FuseFC);

                m_SwitchFC = pElectricFWS.OpenFeatureClass("edgis.Switch");
                CacheAttributesForOC((IObjectClass)m_SwitchFC);

                m_VoltageRegulatorFC = pElectricFWS.OpenFeatureClass("edgis.VoltageRegulator");
                CacheAttributesForOC((IObjectClass)m_VoltageRegulatorFC);

                m_VoltageRegUnitTbl = pElectricFWS.OpenTable("edgis.VoltageRegulatorUnit");
                CacheAttributesForOC((IObjectClass)m_VoltageRegUnitTbl);

                m_PrimaryRiserFC = pElectricFWS.OpenFeatureClass("edgis.PrimaryRiser");
                CacheAttributesForOC((IObjectClass)m_PrimaryRiserFC);

                m_SecondaryRiserFC = pElectricFWS.OpenFeatureClass("edgis.SecondaryRiser");
                CacheAttributesForOC((IObjectClass)m_SecondaryRiserFC);

                m_AntennaFC = pElectricFWS.OpenFeatureClass("edgis.Antenna");
                CacheAttributesForOC((IObjectClass)m_AntennaFC);

                m_PVFC = pElectricFWS.OpenFeatureClass("edgis.PhotoVoltaicCell");
                CacheAttributesForOC((IObjectClass)m_PVFC);

                m_StepDownFC = pElectricFWS.OpenFeatureClass("edgis.StepDown");
                CacheAttributesForOC((IObjectClass)m_StepDownFC);

                m_StepDownUnitTbl= pElectricFWS.OpenTable("edgis.StepDownUnit");
                CacheAttributesForOC((IObjectClass)m_StepDownUnitTbl);
                
                //Relationshipclasses 
                m_PriOHConductorInfoRC = pElectricFWS.OpenRelationshipClass("EDGIS.PriOHConductor_PriOHConductorInfo");
                m_SecOHConductorInfoRC = pElectricFWS.OpenRelationshipClass("EDGIS.SecOHConductor_SecOHConductorInfo");
                m_TransformerRC = pElectricFWS.OpenRelationshipClass("EDGIS.SupportStruct_Transformer");
                m_JointUseAttachmentRC = pElectricFWS.OpenRelationshipClass("EDGIS.SupportStructure_JointUseAttach");
                m_TransformerUnitRC = pElectricFWS.OpenRelationshipClass("EDGIS.Transformer_TransformerUnit");
                m_VoltageRegUnitRC = pElectricFWS.OpenRelationshipClass("EDGIS.VoltageReg_VoltageRegUnit");
                m_StepDownUnitRC = pElectricFWS.OpenRelationshipClass("EDGIS.StepDown_StepDownUnit");
                m_AnchorGuyRC = pElectricFWS.OpenRelationshipClass("EDGIS.SupportStruct_AnchorGuy");
                m_CapBankRC = pElectricFWS.OpenRelationshipClass("EDGIS.SupportStruct_CapacitorBank");
                m_DPDRC = pElectricFWS.OpenRelationshipClass("EDGIS.SupportStruct_DynProtDev");
                m_JointOwnerRC = pElectricFWS.OpenRelationshipClass("EDGIS.SupportStructure_JointOwner");
                m_StreetlightRC = pElectricFWS.OpenRelationshipClass("EDGIS.SupportStruct_Streetlight");
                m_FuseRC = pElectricFWS.OpenRelationshipClass("EDGIS.SupportStruct_Fuse");
                m_SwitchRC = pElectricFWS.OpenRelationshipClass("EDGIS.SupportStruct_Switch");
                m_VoltageRegulatorRC = pElectricFWS.OpenRelationshipClass("EDGIS.SupportStruct_VoltReg");
                m_OpenPointRC = pElectricFWS.OpenRelationshipClass("EDGIS.SupportStruct_OpenPoint");                
                m_PrimaryRiserRC = pElectricFWS.OpenRelationshipClass("EDGIS.SupportStruct_PriRiser");
                m_SecondaryRiserRC = pElectricFWS.OpenRelationshipClass("EDGIS.SupportStruct_SecRiser");
                m_AntennaRC = pElectricFWS.OpenRelationshipClass("EDGIS.SupportStruct_Antenna");
                m_PVRC = pElectricFWS.OpenRelationshipClass("EDGIS.SupportStructure_PhotoVoltaicCell");
                m_StepDownRC = pElectricFWS.OpenRelationshipClass("EDGIS.SupportStruct_StepDown"); 

            }
            catch (Exception ex)
            {
                Shared.WriteToLogfile("Error in " + MethodBase.GetCurrentMethod() +
                    " details: " + ex.Message);
                throw new Exception("Error during data initialization");
            }
        }
        /// <summary>
        /// Adds all the domains to memory for performance benefit 
        /// </summary>
        /// <param name="pOC"></param>
        /// <param name="hshAllDomains"></param>
        //private void CacheDomains(IObjectClass pOC)
        //{
        //    try
        //    {
        //        IDataset pDS = (IDataset)pOC;
        //        string datasetName = GetShortDatasetName(pDS.Name).ToUpper();

        //        for (int i = 0; i < pOC.Fields.FieldCount; i++)
        //        {
        //            if (pOC.Fields.get_Field(i).Domain != null)
        //            {
        //                if (pOC.Fields.get_Field(i).Domain is ICodedValueDomain)
        //                {
        //                    if (((Hashtable)_hshAllAttributes[datasetName]).ContainsKey(pOC.Fields.get_Field(i).Name.ToUpper()))
        //                    {
        //                        //Hashtable hshTemp = (Hashtable)_hshAllAttributes[datasetName];
        //                        Hashtable hshDomain = new Hashtable();
        //                        ICodedValueDomain pCodedValueDomain = (ICodedValueDomain)pOC.Fields.get_Field(i).Domain;
        //                        int codeCount = pCodedValueDomain.CodeCount;
        //                        //Loop through the list of values and print their names
        //                        for (int k = 0; k < codeCount; k++)
        //                        {
        //                            if (!hshDomain.ContainsKey(pCodedValueDomain.get_Value(k)))
        //                                hshDomain.Add(pCodedValueDomain.get_Value(k), pCodedValueDomain.get_Name(k));
        //                        }
        //                        //Add the domain using the field name as the key 
        //                        _hshAllDomains.Add(datasetName + "." + pOC.Fields.get_Field(i).Name.ToUpper(), hshDomain);
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception("Error caching domains");
        //    }
        //}


        /// <summary>
        /// Determines the subset of poles to be processed and calls 
        /// ProcessPole routine to process each one 
        /// </summary>
        public void ProcessPoles( 
            string lastDigitsOfOId, 
            string processSuffix)
        {
            try
            {
                //Load up the workspaces 
                Shared.LoadConfigurationSettings(processSuffix);
                Shared.InitializeLogfile();
                Shared.WriteToLogfile("Entering " + MethodBase.GetCurrentMethod());

                Shared.InitializeSpanCSVfile();
                Shared.InitializeFullDataCSVfile();
                Shared.InitializeStaticCSVfile();
                Shared.WriteToLogfile("Connecting to workspaces");
                Shared.LoadWorkspaces();
                Shared.WriteToLogfile("======================================================");

                //Initialize data                 
                InitializeData();


                //Open the map document  
                Shared.WriteToLogfile("Opening map document"); 
                string mapDocPath = Shared.MapDocumentPath; 
                IMapDocument pMapDoc = new MapDocumentClass();                
                pMapDoc.Open(mapDocPath);

                //Get the DEM layer for determining pole heights 
                List<ILayer> pLayers = getLayersFromMap(pMapDoc.Map[0], "Heights");
                IRasterLayer pRasterLayer = (IRasterLayer)pLayers[0];
                IIdentify pIdentify = (IIdentify)pRasterLayer;

                //Get the list of poles to be processed
                Hashtable hshPoleOIDs = null;
                if (Shared.UseList)
                {
                    hshPoleOIDs = GetPolesToProcess(lastDigitsOfOId, Shared.PolesList());
                }
                else
                {
                    hshPoleOIDs = GetPolesToProcess(lastDigitsOfOId);
                }
                if (hshPoleOIDs == null)
                {
                    Shared.WriteToLogfile("No poles found to process"); 
                    return;
                } 
                GC.Collect();

                Shared.WriteToLogfile("Found: " + hshPoleOIDs.Count.ToString() + " poles to process");
                int counter = 0;
                foreach (int oid in hshPoleOIDs.Keys)
                {
                    counter++;
                    
                    try
                    {
                        Shared.WriteToLogfile(
                            "Processing pole: " + counter.ToString() + " of: " + hshPoleOIDs.Count.ToString() +
                            ", OID: " + oid.ToString());
                        ProcessPole(oid, pIdentify, false);
                        GC.Collect();                        
                    }
                    catch
                    {
                        //Log the error and proceeed 
                        Shared.WriteToLogfile("Error processing pole with OId: " + oid.ToString());
                    }                    
                    Debug.Print("Finished: " + counter.ToString() + " poles");
                }
            }
            catch (Exception ex)
            {
                Shared.WriteToLogfile("Error in " + MethodBase.GetCurrentMethod() +
                    " details: " + ex.Message);
                throw ex; 
            }
        }

        public void GeneratePoleCategories(
            string lastDigitsOfOId, 
            string processSuffix, 
            string fileName)
        {
            try
            {
                //Load up the workspaces 
                Shared.LoadConfigurationSettings(processSuffix);
                Shared.InitializeLogfile();
                Shared.WriteToLogfile("Entering " + MethodBase.GetCurrentMethod());
                Shared.WriteToLogfile("Connecting to workspaces");
                Shared.LoadWorkspaces();
                Shared.WriteToLogfile("======================================================");

                //Initialize data                 
                InitializeData();
                string largeConductorWhereClause = string.Empty; 

                //Category 1 
                //Pole class >= 5 
                //JointOwned or Contact Pole 
                //MultipleCircuits 
                //ProcessCategory1("CATEGORY1");

                //Category 2 
                //Pole class >= 5  
                //Long conductor (> 250 feet) 
                //Large Conductor (715, 397, 1/0, 3/0, 4/0) 
                largeConductorWhereClause =
                    "CONDUCTORUSE" + " <> " + "'Service'" + " And " +
                    "([KEY]" + " LIKE '%397%'" +
                    " OR " +
                    "[KEY]" + " LIKE '%715%'" +
                    " OR " +
                    "[KEY]" + " LIKE '%1/0%'" +
                    " OR " +
                    "[KEY]" + " LIKE '%3/0%'" +
                    " OR " +
                    "[KEY]" + " LIKE '%4/0%'" + ")";
                ProcessCategory2("CATEGORY2", 250, largeConductorWhereClause);

                //Category 3 
                //Pole class= 4  
                //JointOwned or Contact Pole 
                //MultipleCircuits
                //ProcessCategory3("CATEGORY3");

                //Category 4 
                //Pole class = 4  
                //JointOwned or Contact Pole 
                //Long conductor (> 350 feet) 
                //Large Conductor (715 or 397) 
                largeConductorWhereClause =
                    "CONDUCTORUSE" + " <> " + "'Service'" + " And " +
                    "([KEY]" + " LIKE '%397%'" +
                    " OR " +
                    "[KEY]" + " LIKE '%715%')";
                ProcessCategory4("CATEGORY4", 350, largeConductorWhereClause);

            }
            catch (Exception ex)
            {
                Shared.WriteToLogfile("Error in " + MethodBase.GetCurrentMethod() +
                    " details: " + ex.Message);
                throw ex;
            }
        }

        private void ProcessCategory1(string poleCategoryField)
        {
            try
            {
                //Pole class >= 5 
                //JointOwned or Contact Pole 
                //DoubleCircuits 

                string whereClause = string.Empty;
                //vales are: 0,00,1,2,3,4,5,6,7,H1,H2,H3,H4,H5,Other 
                whereClause = "[CLASS] IN('5','6','7')";
                whereClause += " And ";
                whereClause += "JOINT_OWNED = 'TRUE'";
                List<string> pGUIDs = GetPolesSatisfyingWhereClause(whereClause);
                int counter = 0; 

                foreach (string poleGUID in pGUIDs)
                {
                    if (HasMultipleCircuits(poleGUID, 12))
                    {
                        Shared.WriteToLogfile("Pole " + poleGUID + " is " + poleCategoryField);
                        UpdatePoleCategory(poleGUID, poleCategoryField);
                    }
                    counter++; 
                    if (counter % 500 == 0)
                    {
                        Debug.Print("finished: " + counter.ToString() + "!");
                    }
                }

                Shared.WriteToLogfile("Finished " + poleCategoryField + " found: " + counter.ToString());
            }
            catch (Exception ex)
            {
                Shared.WriteToLogfile("Error in " + MethodBase.GetCurrentMethod() +
                    " details: " + ex.Message);
                throw ex;
            }
        }

        private void ProcessCategory2(string poleCategoryField, int conductorLengthThreshold, string largeConductorWhereClause)
        {
            try
            {
                //Pole class >= 5 
                //Large Conductor (> 250 feet)  
                //Long Conductor (397 or 715)  

                string whereClause = string.Empty;
                //vales are: 0,00,1,2,3,4,5,6,7,H1,H2,H3,H4,H5,Other 
                whereClause = "[CLASS] IN('5','6','7')";
                List<string> pGUIDs = GetPolesSatisfyingWhereClause(whereClause);
                int counter = 0;

                foreach (string poleGUID in pGUIDs)
                {
                    if (HasLargeConductor(poleGUID, largeConductorWhereClause))
                    {
                        //Found a large conductor 
                        if (HasLongConductor(poleGUID, conductorLengthThreshold))
                        {
                            //Found a long conductor 
                            Shared.WriteToLogfile("Pole " + poleGUID + " is " + poleCategoryField);
                            UpdatePoleCategory(poleGUID, poleCategoryField);
                        }
                    }
                    counter++;
                    if (counter % 500 == 0)
                    {
                        Debug.Print("finished: " + counter.ToString() + "!");
                    }
                }

                Shared.WriteToLogfile("Finished " + poleCategoryField + " found: " + counter.ToString());
            }
            catch (Exception ex)
            {
                Shared.WriteToLogfile("Error in " + MethodBase.GetCurrentMethod() +
                    " details: " + ex.Message);
                throw ex;
            }
        }

        private void ProcessCategory3(string poleCategoryField)
        {
            try
            {
                //Pole class = 4 
                //JointOwned or Contact Pole 
                //DoubleCircuits 

                string whereClause = string.Empty;
                //vales are: 0,00,1,2,3,4,5,6,7,H1,H2,H3,H4,H5,Other 
                whereClause = "[CLASS] = '4'";
                whereClause += " And ";
                whereClause += "JOINT_OWNED = 'TRUE'";
                List<string> pGUIDs = GetPolesSatisfyingWhereClause(whereClause);
                int counter = 0;

                foreach (string poleGUID in pGUIDs)
                {
                    if (HasMultipleCircuits(poleGUID, 12))
                    {
                        Shared.WriteToLogfile("Pole " + poleGUID + " is " + poleCategoryField);
                        UpdatePoleCategory(poleGUID, poleCategoryField);
                    }
                    counter++;
                    if (counter % 500 == 0)
                    {
                        Debug.Print("finished: " + counter.ToString() + "!");
                    }
                }

                Shared.WriteToLogfile("Finished " + poleCategoryField + " found: " + counter.ToString());
            }
            catch (Exception ex)
            {
                Shared.WriteToLogfile("Error in " + MethodBase.GetCurrentMethod() +
                    " details: " + ex.Message);
                throw ex;
            }
        }

        private void ProcessCategory4(string poleCategoryField, int conductorLengthThreshold, string largeConductorWhereClause)
        {
            try
            {
                //Pole class  = 4  
                //Large Conductor (> 250 feet)  
                //Long Conductor (397 or 715)  

                string whereClause = string.Empty;
                whereClause = "[CLASS] = '4'";
                whereClause += " And ";
                whereClause += "JOINT_OWNED = 'TRUE'";
                List<string> pGUIDs = GetPolesSatisfyingWhereClause(whereClause);
                int counter = 0;

                foreach (string poleGUID in pGUIDs)
                {
                    if (HasLargeConductor(poleGUID, largeConductorWhereClause))
                    {
                        //Found a large conductor 
                        if (HasLongConductor(poleGUID, conductorLengthThreshold))
                        {
                            //Found a long conductor 
                            Shared.WriteToLogfile("Pole " + poleGUID + " is " + poleCategoryField);
                            UpdatePoleCategory(poleGUID, poleCategoryField);
                        }
                    }
                    counter++;
                    if (counter % 500 == 0)
                    {
                        Debug.Print("finished: " + counter.ToString() + "!");
                    }
                }

                Shared.WriteToLogfile("Finished " + poleCategoryField + " found: " + counter.ToString());
            }
            catch (Exception ex)
            {
                Shared.WriteToLogfile("Error in " + MethodBase.GetCurrentMethod() +
                    " details: " + ex.Message);
                throw ex;
            }
        }



        private void UpdatePoleCategory(
            string poleGUID,
            string poleCategoryField)
        {
            try
            {
                Shared.WriteToLogfile("Entering " + MethodBase.GetCurrentMethod());
                if (_pConn == null)
                {
                    //Get output DB connection 
                    _pConn = new SqlConnection(
                        Shared.OutputConnectString);
                }
                if (_pConn.State != System.Data.ConnectionState.Open)
                    _pConn.Open();

                //String to hold the SQL insert statement 
                string sqlStatement = string.Empty;
                SqlCommand pCmd = null;

                //Write the insert statement 
                sqlStatement =
                    "UPDATE [PGE_Combined].[dbo].[POLES_BU] SET " +
                        poleCategoryField + " = " + 1 + " " +
                    "WHERE" + " " +
                        "POLEGUID = '" + poleGUID + "'" + ";";

                //Execute the command against the database 
                if (sqlStatement != string.Empty)
                {
                    //Shared.WriteToLogfile(sqlStatement);

                    //Shared.WriteToLogfile(sqlStatement);
                    pCmd = _pConn.CreateCommand();
                    pCmd.Connection = _pConn;
                    pCmd.CommandText = sqlStatement;
                    int recordsUpdated = pCmd.ExecuteNonQuery();
                    if (recordsUpdated == 0)
                        Shared.WriteToLogfile("SQL update failed for pole: " + poleGUID);
                }
            }
            catch (Exception ex)
            {
                Shared.WriteToLogfile("Error in UpdatePoleCategory for : " + poleGUID);
            }
        }

        private bool HasMultipleCircuits(string poleGUID, int maxConductorCount)
        {
            try
            {
                //Shared.WriteToLogfile("Entering " + MethodBase.GetCurrentMethod());

                Hashtable hshCircs = new Hashtable();
                if (_pConn == null)
                {
                    //Get output DB connection 
                    _pConn = new SqlConnection(
                        Shared.OutputConnectString);
                }
                if (_pConn.State != System.Data.ConnectionState.Open)
                    _pConn.Open();

                //Setup the command 
                SqlCommand pCmd = _pConn.CreateCommand();

                //Run an INSERT INTO SELECT statement for the error 
                //Hashtable hshExistingPoleSapIds = new Hashtable();
                string sql = "SELECT " + " " + "[COUNT]" + " " +
                    "FROM" + " " +
                    PoleLoadConstants.SPANS_TBL + " " +
                    "WHERE" + " " + 
                    "POLEGUID" + " = " + "'" + poleGUID + "'" + " And " + 
                    "CONDUCTORUSE" + " <> " + "'Service'" + ";";

                int conductCount = 0;
                int conductCountTotal = 0;
                string conCountString = string.Empty;  
                pCmd.CommandText = sql;
                SqlDataReader reader = pCmd.ExecuteReader();

                if (!reader.HasRows)
                {
                    reader.Close();
                    return false;
                }

                string circ = string.Empty;

                while (reader.Read())
                {
                    conCountString = string.Empty;
                    if (!reader.IsDBNull(0))
                        conCountString = reader.GetString(0).Trim(); 

                    
                    if (conCountString != string.Empty)
                    {
                        if (Int32.TryParse(conCountString, out conductCount))
                            conductCountTotal += conductCount;
                    }
                }
                if (reader != null)
                    reader.Close();


                //Return the hashtable 
                if (conductCountTotal >= maxConductorCount)
                    return true; 
                else 
                    return false;
            }
            catch (Exception ex)
            {
                Shared.WriteToLogfile("Entering error handler for " + MethodBase.GetCurrentMethod() + " error: " +
                    ex.Message);
                throw new Exception("Error returning existing Pole SapEquipIds");
            }
        }


        private bool HasLongConductor(string poleGUID, int conductorLengthThreshold)
        {
            try
            {
                //Shared.WriteToLogfile("Entering " + MethodBase.GetCurrentMethod());

                bool hasLongCon = false;

                Hashtable hshCircs = new Hashtable();
                if (_pConn == null)
                {
                    //Get output DB connection 
                    _pConn = new SqlConnection(
                        Shared.OutputConnectString);
                }
                if (_pConn.State != System.Data.ConnectionState.Open)
                    _pConn.Open();

                //Setup the command 
                SqlCommand pCmd = _pConn.CreateCommand();

                //Run an INSERT INTO SELECT statement for the error 
                //Hashtable hshExistingPoleSapIds = new Hashtable();
                string sql = "SELECT " + " " + "[POLEGUID]" + " " +
                    "FROM" + " " +
                    PoleLoadConstants.SPANS_TBL + " " +
                    "WHERE" + " " +
                    "POLEGUID" + " = " + "'" + poleGUID + "'" + " And " + 
                    "CONDUCTORUSE" + " <> " + "'Service'" + " And " +
                    "CAST(LENGTH AS DECIMAL(18,1)) > " + conductorLengthThreshold.ToString() + ";";

                string conCountString = string.Empty;
                pCmd.CommandText = sql;
                SqlDataReader reader = pCmd.ExecuteReader();

                if (reader.HasRows)
                    hasLongCon = true;
                if (reader != null)
                    reader.Close();


                //Return the boolean 
                return hasLongCon;
            }
            catch (Exception ex)
            {
                Shared.WriteToLogfile("Entering error handler for " + MethodBase.GetCurrentMethod() + " error: " +
                    ex.Message);
                throw new Exception("Error returning existing Pole SapEquipIds");
            }
        }




        private bool HasLargeConductor(string poleGUID, string largeConductorWhereClause)
        {
            try
            {
                //Shared.WriteToLogfile("Entering " + MethodBase.GetCurrentMethod());

                bool hasLargeCon = false;

                Hashtable hshCircs = new Hashtable();
                if (_pConn == null)
                {
                    //Get output DB connection 
                    _pConn = new SqlConnection(
                        Shared.OutputConnectString);
                }
                if (_pConn.State != System.Data.ConnectionState.Open)
                    _pConn.Open();

                //Setup the command 
                SqlCommand pCmd = _pConn.CreateCommand();

                //Run an INSERT INTO SELECT statement for the error 
                //Hashtable hshExistingPoleSapIds = new Hashtable();
                string sql = "SELECT " + " " + "[POLEGUID]" + " " +
                    "FROM" + " " +
                    PoleLoadConstants.SPANS_TBL + " " +
                    "WHERE" + " " +
                    "POLEGUID" + " = " + "'" + poleGUID + "'" + 
                    " And " + 
                    largeConductorWhereClause + ";"; 

                int conductCount = 0;
                int conductCountTotal = 0;
                string conCountString = string.Empty;
                pCmd.CommandText = sql;
                SqlDataReader reader = pCmd.ExecuteReader();

                if (reader.HasRows)
                    hasLargeCon = true;
                if (reader != null)
                    reader.Close();


                //Return the boolean 
                return hasLargeCon;
            }
            catch (Exception ex)
            {
                Shared.WriteToLogfile("Entering error handler for " + MethodBase.GetCurrentMethod() + " error: " +
                    ex.Message);
                throw new Exception("Error returning existing Pole SapEquipIds");
            }
        }


        private List<string> GetPolesSatisfyingWhereClause(string whereClause)
        {
            try
            {
                Shared.WriteToLogfile("Entering " + MethodBase.GetCurrentMethod());

                Hashtable hshGUIDs = new Hashtable();
                List<string> pGUIDs = new List<string>(); 
                if (_pConn == null)
                {
                    //Get output DB connection 
                    _pConn = new SqlConnection(
                        Shared.OutputConnectString);
                }
                if (_pConn.State != System.Data.ConnectionState.Open)
                    _pConn.Open();

                //Setup the command 
                SqlCommand pCmd = _pConn.CreateCommand();

                //Run an INSERT INTO SELECT statement for the error 
                //Hashtable hshExistingPoleSapIds = new Hashtable();
                string sql = "SELECT" + " " + "POLEGUID" + " " +
                    "FROM" + " " +
                    PoleLoadConstants.POLES_TBL + " " +
                    "WHERE" + " " + whereClause + ";";

                pCmd.CommandText = sql;
                SqlDataReader reader = pCmd.ExecuteReader();

                if (!reader.HasRows)
                {
                    reader.Close();
                    return pGUIDs;
                }

                string guid = string.Empty; 

                while (reader.Read())
                {
                    guid = string.Empty;
                    if (!reader.IsDBNull(0))
                        guid = reader.GetString(0);

                    if (guid != string.Empty)
                    {
                        if (!hshGUIDs.ContainsKey(guid))
                        {
                            hshGUIDs.Add(guid, 0);
                            pGUIDs.Add(guid);
                        }
                    }
                }
                if (reader != null)
                    reader.Close();


                //Return the hashtable 
                return pGUIDs;
            }
            catch (Exception ex)
            {
                Shared.WriteToLogfile("Entering error handler for " + MethodBase.GetCurrentMethod() + " error: " +
                    ex.Message);
                throw new Exception("Error returning existing Pole SapEquipIds");
            }
        }

        public Hashtable LoadConductorCodes(XmlNode pTopNode)
        {
            try
            {
                Hashtable hshConductorCodes = new Hashtable();
                int codeInt = 0;
                string material = string.Empty;
                string size = string.Empty;
                XmlNode pSubNode = null;
                XmlNode pXMLNode = null; 

                for (int i = 0; i < pTopNode.ChildNodes.Count; i++)
                {
                    pXMLNode = pTopNode.ChildNodes.Item(i);
                    if (pXMLNode != null)
                    {
                        codeInt = 0;
                        material = string.Empty;
                        size = string.Empty;
                        for (int j = 0; j < pXMLNode.ChildNodes.Count; j++)
                        {
                            pSubNode = pXMLNode.ChildNodes.Item(j);
                            if (pSubNode.LocalName == "Code")
                                codeInt = Convert.ToInt32(pSubNode.InnerText);
                            if (pSubNode.LocalName == "Material")
                                material = pSubNode.InnerText;
                            if (pSubNode.LocalName == "ConductorSize")
                                size = pSubNode.InnerText;
                        }

                        ConductorCode pConCode = new ConductorCode(codeInt, size, material);
                        hshConductorCodes.Add(codeInt, pConCode); 
                    }
                }

                return hshConductorCodes;
            }
            catch
            {
                throw new Exception("Error loading conductor codes");
            }            
        }

        private Hashtable GetPolesToProcess( 
            string lastDigitsOfOId)
        {
            try
            {
                Shared.WriteToLogfile("Entering " + MethodBase.GetCurrentMethod());

                //Get a hashtable of digits to process for this thread
                Hashtable hshDigits = new Hashtable();
                Hashtable hshOIds = new Hashtable(); 
                string lastDigit = string.Empty;
                int fldIdx = -1;
                string[] digits = lastDigitsOfOId.Split(',');
                for (int i = 0; i < digits.Length; i++)
                {
                    if (!hshDigits.ContainsKey(digits[i]))
                        hshDigits.Add(digits[i], 0);
                }

                //Loop through the poles and find the ones with the 
                //matching last digits 
                ISpatialFilter pSF = null; 
                IPolygon pClipPolygon = Shared.GetClipPolygon();
                if (pClipPolygon != null)
                {
                    pSF = new SpatialFilterClass();
                    pSF.Geometry = pClipPolygon;
                    pSF.SpatialRel = esriSpatialRelEnum.esriSpatialRelIntersects;
                    pSF.WhereClause = Shared.PoleFilter;
                }

                //Get the existing sapequipids from the output database so as 
                //not to process the same pole twice 
                Hashtable hshExistingSapIds = GetExistingPoleSapIds();
                Shared.WriteToLogfile("Found: " + hshExistingSapIds.Count.ToString() + " existing poles");

                fldIdx = m_SupportStructureFC.Fields.FindField("SAPEQUIPID");
                int sapId = -1;
                int counter = 0; 
                
                IFeatureCursor pFCursor = m_SupportStructureFC.Search(pSF, false);
                IFeature pFeature = pFCursor.NextFeature();
                while (pFeature != null)
                {
                    counter++;
                    //SyncPoleTemp(pFeature); 

                    lastDigit = pFeature.OID.ToString().Substring(
                        pFeature.OID.ToString().Length - 1, 1);

                    //Get the SapEquipId
                    sapId = -1;
                    if (pFeature.get_Value(fldIdx) != DBNull.Value)
                        Int32.TryParse(pFeature.get_Value(fldIdx).ToString(), out sapId);

                    if (!hshExistingSapIds.ContainsKey(sapId))
                    {
                        if (hshDigits.ContainsKey(lastDigit))
                        {
                            //If not already processed 
                            if (!hshOIds.ContainsKey(pFeature.OID))
                                hshOIds.Add(pFeature.OID, 0);
                        }
                    }
                    pFeature = pFCursor.NextFeature();
                }
                Marshal.FinalReleaseComObject(pFCursor);
                hshExistingSapIds.Clear();

                //*** TODO - comment this line after debugging
                //these are test case poles 

                //hshSapIds.Clear();
                ////hshSapIds.Add(100914183, 0);

                //if (!hshSapIds.ContainsKey(101991505))
                //    hshSapIds.Add(101991505, 0);
                //if (!hshSapIds.ContainsKey(101438788))
                //    hshSapIds.Add(101438788, 0);
                //if (!hshSapIds.ContainsKey(101440074))
                //    hshSapIds.Add(101440074, 0);
                //if (!hshSapIds.ContainsKey(101440510))
                //    hshSapIds.Add(101440510, 0);
                //if (!hshSapIds.ContainsKey(101440062))
                //    hshSapIds.Add(101440062, 0);
                //if (!hshSapIds.ContainsKey(101440074))
                //    hshSapIds.Add(101440074, 0);
                //if (!hshSapIds.ContainsKey(101440199))
                //    hshSapIds.Add(101440199, 0);
                //if (!hshSapIds.ContainsKey(103157736))
                //    hshSapIds.Add(103157736, 0);
                //if (!hshSapIds.ContainsKey(101771074))
                //    hshSapIds.Add(101771074, 0);
                //if (!hshSapIds.ContainsKey(103264784))
                //    hshSapIds.Add(103264784, 0);
                //if (!hshSapIds.ContainsKey(100599046))
                //    hshSapIds.Add(100599046, 0);
                //if (!hshSapIds.ContainsKey(101440398))
                //    hshSapIds.Add(101440398, 0);
                //if (!hshSapIds.ContainsKey(101282869))
                //    hshSapIds.Add(101282869, 0);
                //if (!hshSapIds.ContainsKey(101615419))
                //    hshSapIds.Add(101615419, 0);
                //if (!hshSapIds.ContainsKey(102123531))
                //    hshSapIds.Add(102123531, 0);
                //if (!hshSapIds.ContainsKey(101440230))
                //    hshSapIds.Add(101440230, 0);
                //if (!hshSapIds.ContainsKey(101444553))
                //    hshSapIds.Add(101444553, 0);
                //if (!hshSapIds.ContainsKey(101439110))
                //    hshSapIds.Add(101439110, 0);
                //if (!hshSapIds.ContainsKey(101440258))
                //    hshSapIds.Add(101440258, 0);
                //if (!hshSapIds.ContainsKey(101439984))
                //    hshSapIds.Add(101439984, 0);
                //if (!hshSapIds.ContainsKey(101192661))
                //    hshSapIds.Add(101192661, 0);
                //if (!hshSapIds.ContainsKey(101620906))
                //    hshSapIds.Add(101620906, 0);
                //if (!hshSapIds.ContainsKey(101440061))
                //    hshSapIds.Add(101440061, 0);
                //if (!hshSapIds.ContainsKey(100599046))
                //    hshSapIds.Add(100599046, 0);
                
                return hshOIds; 

            }
            catch (Exception ex)
            {
                Shared.WriteToLogfile("Error in " + MethodBase.GetCurrentMethod() +
                    " details: " + ex.Message);
                throw ex;
            }
        }

        //private Hashtable SyncPoles(
        //    string lastDigitsOfOId)
        //{
        //    try
        //    {
        //        Shared.WriteToLogfile("Entering " + "SyncPoles");

        //        //Get a hashtable of digits to process for this thread
        //        Hashtable hshDigits = new Hashtable();
        //        Hashtable hshOIds = new Hashtable();
        //        string lastDigit = string.Empty;
        //        int fldIdx = -1;
        //        string[] digits = lastDigitsOfOId.Split(',');
        //        for (int i = 0; i < digits.Length; i++)
        //        {
        //            if (!hshDigits.ContainsKey(digits[i]))
        //                hshDigits.Add(digits[i], 0);
        //        }

        //        //Loop through the poles and find the ones with the 
        //        //matching last digits 
        //        ISpatialFilter pSF = null;
        //        IPolygon pClipPolygon = Shared.GetClipPolygon();
        //        if (pClipPolygon != null)
        //        {
        //            pSF = new SpatialFilterClass();
        //            pSF.Geometry = pClipPolygon;
        //            pSF.SpatialRel = esriSpatialRelEnum.esriSpatialRelIntersects;
        //            pSF.WhereClause = Shared.PoleFilter;
        //        }

        //        //Get the existing sapequipids from the output database so as 
        //        //not to process the same pole twice 
        //        //Hashtable hshExistingSapIds = GetExistingPoleSapIds();
        //        //Shared.WriteToLogfile("Found: " + hshExistingSapIds.Count.ToString() + " existing poles");

        //        fldIdx = m_SupportStructureFC.Fields.FindField("SAPEQUIPID");
        //        int sapId = -1;
        //        int counter = 0;

        //        IFeatureCursor pFCursor = m_SupportStructureFC.Search(pSF, false);
        //        IFeature pFeature = pFCursor.NextFeature();
        //        while (pFeature != null)
        //        {
        //            counter++;
        //            //SyncPoleTemp(pFeature); 

        //            lastDigit = pFeature.OID.ToString().Substring(
        //                pFeature.OID.ToString().Length - 1, 1);

        //            //Get the SapEquipId
        //            sapId = -1;
        //            if (pFeature.get_Value(fldIdx) != DBNull.Value)
        //                Int32.TryParse(pFeature.get_Value(fldIdx).ToString(), out sapId);

        //            if (hshDigits.ContainsKey(lastDigit))
        //            {
        //                SyncPole(pFeature);
        //            }
        //            pFeature = pFCursor.NextFeature();
        //        }
        //        Marshal.FinalReleaseComObject(pFCursor);
        //        //hshExistingSapIds.Clear();

        //        //*** TODO - comment this line after debugging
        //        //these are test case poles 

        //        //hshSapIds.Clear();
        //        ////hshSapIds.Add(100914183, 0);

        //        //if (!hshSapIds.ContainsKey(101991505))
        //        //    hshSapIds.Add(101991505, 0);
        //        //if (!hshSapIds.ContainsKey(101438788))
        //        //    hshSapIds.Add(101438788, 0);
        //        //if (!hshSapIds.ContainsKey(101440074))
        //        //    hshSapIds.Add(101440074, 0);
        //        //if (!hshSapIds.ContainsKey(101440510))
        //        //    hshSapIds.Add(101440510, 0);
        //        //if (!hshSapIds.ContainsKey(101440062))
        //        //    hshSapIds.Add(101440062, 0);
        //        //if (!hshSapIds.ContainsKey(101440074))
        //        //    hshSapIds.Add(101440074, 0);
        //        //if (!hshSapIds.ContainsKey(101440199))
        //        //    hshSapIds.Add(101440199, 0);
        //        //if (!hshSapIds.ContainsKey(103157736))
        //        //    hshSapIds.Add(103157736, 0);
        //        //if (!hshSapIds.ContainsKey(101771074))
        //        //    hshSapIds.Add(101771074, 0);
        //        //if (!hshSapIds.ContainsKey(103264784))
        //        //    hshSapIds.Add(103264784, 0);
        //        //if (!hshSapIds.ContainsKey(100599046))
        //        //    hshSapIds.Add(100599046, 0);
        //        //if (!hshSapIds.ContainsKey(101440398))
        //        //    hshSapIds.Add(101440398, 0);
        //        //if (!hshSapIds.ContainsKey(101282869))
        //        //    hshSapIds.Add(101282869, 0);
        //        //if (!hshSapIds.ContainsKey(101615419))
        //        //    hshSapIds.Add(101615419, 0);
        //        //if (!hshSapIds.ContainsKey(102123531))
        //        //    hshSapIds.Add(102123531, 0);
        //        //if (!hshSapIds.ContainsKey(101440230))
        //        //    hshSapIds.Add(101440230, 0);
        //        //if (!hshSapIds.ContainsKey(101444553))
        //        //    hshSapIds.Add(101444553, 0);
        //        //if (!hshSapIds.ContainsKey(101439110))
        //        //    hshSapIds.Add(101439110, 0);
        //        //if (!hshSapIds.ContainsKey(101440258))
        //        //    hshSapIds.Add(101440258, 0);
        //        //if (!hshSapIds.ContainsKey(101439984))
        //        //    hshSapIds.Add(101439984, 0);
        //        //if (!hshSapIds.ContainsKey(101192661))
        //        //    hshSapIds.Add(101192661, 0);
        //        //if (!hshSapIds.ContainsKey(101620906))
        //        //    hshSapIds.Add(101620906, 0);
        //        //if (!hshSapIds.ContainsKey(101440061))
        //        //    hshSapIds.Add(101440061, 0);
        //        //if (!hshSapIds.ContainsKey(100599046))
        //        //    hshSapIds.Add(100599046, 0);

        //        return hshOIds;

        //    }
        //    catch (Exception ex)
        //    {
        //        Shared.WriteToLogfile("Error in " + MethodBase.GetCurrentMethod() +
        //            " details: " + ex.Message);
        //        throw ex;
        //    }
        //}

        //private void SyncPoles( 
        //    string lastDigitsOfID, 
        //    string fileName)
        //{
        //    try
        //    {
        //        Shared.WriteToLogfile("Entering " + "SyncPoles");

        //        int counter = 0;
        //        string line;
        //        string dateTimeStr = string.Empty;
        //        int year;
        //        int month;
        //        int day;
        //        int hour;
        //        int minute; 
        //        int second;
        //        int poleHeight = 0;
        //        int poleClass = 0;
        //        string poleSpecies = string.Empty;
        //        string poleSapEquipId = string.Empty;
        //        int poleOriginalCirc = 0;
        //        int poleEffectCirc = 0;
        //        int poleRemainingStrength = 0;
        //        bool jointOwned = false;

        //        DirectoryInfo dirInfo = new DirectoryInfo("C:\\Simon\\Data\\SAP");
        //        FileInfo[] finfos = dirInfo.GetFiles();


        //        Shared.WriteToLogfile("Processing file: " + fileName);
        //        System.IO.StreamReader file =
        //            new System.IO.StreamReader(fileName);
        //        while ((line = file.ReadLine()) != null)
        //        {
        //            Console.WriteLine(line);

        //            //Reset variables 
        //            bool poleExists = false; 
        //            poleHeight = 0;
        //            poleClass = 0;
        //            string theValue = string.Empty; 
        //            poleSpecies = string.Empty;
        //            poleSapEquipId = string.Empty;
        //            poleOriginalCirc = 0;
        //            poleEffectCirc = 0;
        //            Nullable<DateTime> pLastInspectDate = null;
        //            Nullable<DateTime> pCurrInspectDate = null;
        //            poleRemainingStrength = 0;
        //            jointOwned = false;
        //            year = 0; 

        //            //Ignore the header line 
        //            if (counter != 0)
        //            {
        //                string[] fieldVals = line.Split('\t');

        //                if (fieldVals.Length == 8)
        //                {
        //                    //1.SAPEQUIPID 
        //                    if (fieldVals[0].Trim().Length > 0)
        //                        poleSapEquipId = fieldVals[0].Trim(); 

        //                    //Find the SapEquipId in the Poles table
        //                    poleExists = PoleExists(poleSapEquipId, ref pCurrInspectDate);                         

        //                    //2.HEIGHT_SAP 
        //                    if (fieldVals[1].Trim().Length > 0)
        //                        Int32.TryParse(fieldVals[1].Trim(), out poleHeight);

        //                    //3.CLASS_SAP 
        //                    if (fieldVals[2].Trim().Length > 0)
        //                        Int32.TryParse(fieldVals[2].Trim(), out poleClass);

        //                    //4.SPECIES_SAP 
        //                    if (fieldVals[3].Trim().Length > 0)
        //                        poleSpecies = fieldVals[3].Trim();

        //                    //5.ORIGINALCIRCUMFERENCE 
        //                    if (fieldVals[4].Trim().Length > 0)
        //                    {
        //                        theValue = fieldVals[4].Trim();
        //                        if (theValue.EndsWith(".00"))
        //                            theValue = theValue.Substring(0, theValue.Length - 3);
        //                        Int32.TryParse(theValue, out poleOriginalCirc);
        //                    }

        //                    //6.EFFECTIVECIRCUMFERENCE 
        //                    if (fieldVals[5].Trim().Length > 0)
        //                    {
        //                        theValue = fieldVals[5].Trim();
        //                        if (theValue.EndsWith(".00"))
        //                            theValue = theValue.Substring(0, theValue.Length - 3);
        //                        Int32.TryParse(theValue, out poleEffectCirc);
        //                    }

        //                    //7.LASTINSPECTIONDATETIME 
        //                    if (fieldVals[6].Trim().Length > 0)
        //                    {
        //                        dateTimeStr = fieldVals[6].Trim();
        //                        if (dateTimeStr != "0")
        //                        {
        //                            year = Convert.ToInt32(dateTimeStr.Substring(0, 4));
        //                            month = Convert.ToInt32(dateTimeStr.Substring(4, 2));
        //                            day = Convert.ToInt32(dateTimeStr.Substring(6, 2));
        //                            hour = Convert.ToInt32(dateTimeStr.Substring(8, 2));
        //                            minute = Convert.ToInt32(dateTimeStr.Substring(10, 2));
        //                            second = Convert.ToInt32(dateTimeStr.Substring(12, 2));
        //                            pLastInspectDate = new DateTime(year, month, day, hour, minute, second);
        //                        } 
        //                    }

        //                    //8.REMAININGSTRENGTH 
        //                    poleRemainingStrength = 100; 
        //                    if ((year < 2014) && (year != 0))
        //                    {
        //                        //Ratio of effectcirc / originalcirc 
        //                        if ((poleEffectCirc != 0) && (poleOriginalCirc != 0))
        //                        {
        //                            if (poleEffectCirc != poleOriginalCirc)
        //                                Debug.Print("Not equal"); 

        //                            int theRemStr = (int)(((double)poleEffectCirc / (double)poleOriginalCirc) * 100);
        //                            if ((theRemStr > 0) && (theRemStr < 100))
        //                                poleRemainingStrength = theRemStr;
        //                        }
        //                    }
        //                    else if (year >= 2014)
        //                    {
        //                        //Take it from the text file 
        //                        if (fieldVals[7].Trim().Length > 0)
        //                        {
        //                            theValue = fieldVals[7].Trim();
        //                            if (theValue.EndsWith(".00"))
        //                                theValue = theValue.Substring(0, theValue.Length - 3);
        //                            Int32.TryParse(fieldVals[5].Trim(), out poleRemainingStrength);
        //                            if (poleRemainingStrength == 0)
        //                                poleRemainingStrength = 100;
        //                            else
        //                                Debug.Print("Not zero"); 
        //                        }
        //                    }

        //                    if (poleRemainingStrength == 0)
        //                        Debug.Print("Possible problem");
        //                    else if (poleRemainingStrength > 100)
        //                        Debug.Print("Possible problem");

        //                    //9.JOINT_OWNED 
        //                    jointOwned = IsJointOwned(poleSapEquipId);

        //                    //Sync the pole 
        //                    if (poleExists)
        //                        SyncPole(
        //                            poleSapEquipId,
        //                            poleHeight.ToString(),
        //                            poleClass.ToString(),
        //                            poleSpecies,
        //                            poleOriginalCirc,
        //                            poleEffectCirc,
        //                            poleRemainingStrength,
        //                            jointOwned,
        //                            pLastInspectDate);
        //                    else
        //                        Debug.Print("pole does not exist");

        //                }
        //                else
        //                {
        //                    Shared.WriteToLogfile("Field sequencing error with line: " + line);
        //                }
        //            }

        //            counter++;
        //            Shared.WriteToLogfile("Finished line: " + counter.ToString());
        //        }

        //        Shared.WriteToLogfile("Finished entire file");
        //        file.Close();
        //    }
        //    catch (Exception ex)
        //    {

        //    }
        //}

        public void UpdateRemainingStrength(
            string folder)
        {
            try
            {
                Shared.LoadConfigurationSettings("SYNC");
                Shared.InitializeLogfile();
                Shared.WriteToLogfile("Entering " + "SyncPoles2");

                int counter = 0;
                int updateCount = 0;
                string line;
                string dateTimeStr = string.Empty;
                int year;
                int month;
                int day;
                int hour;
                int minute;
                int second;
                int poleHeight = 0;
                int poleClass = 0;
                string poleSpecies = string.Empty;
                string poleSapEquipId = string.Empty;
                double poleOriginalCirc = 0;
                double poleEffectCirc = 0;
                double poleRemainingStrength = 0;
                bool jointOwned = false;
                Nullable<DateTime> pLastInspectDate = null;
                string filename = string.Empty;

                DirectoryInfo dirInfo = new DirectoryInfo(folder);
                FileInfo[] finfos = dirInfo.GetFiles();
                
                for (int i = 0; i < finfos.Length; i++)
                {

                    filename = finfos[i].FullName;
                    counter = 0;
                    updateCount = 0;
                    System.IO.StreamReader file =
                        new System.IO.StreamReader(filename);

                    while ((line = file.ReadLine()) != null)
                    {

                        //Reset variables 
                        bool processPole = false;
                        string theValue = string.Empty;
                        poleSapEquipId = string.Empty;
                        poleOriginalCirc = 0;
                        poleEffectCirc = 0;
                        pLastInspectDate = null;

                        //Ignore the header line 
                        if (counter != 0)
                        {
                            string[] fieldVals = line.Split('\t');
                            year = 0;

                            if (fieldVals.Length == 8)
                            {

                                //SAPEQUIPID  
                                if (fieldVals[0].Trim().Length > 0)
                                {
                                    poleSapEquipId = fieldVals[0].Trim();
                                }

                                //if (poleSapEquipId == "103220832")
                                //    Debug.Print("this is the one");

                                //ORIGINALCIRCUMFERENCE 
                                if (fieldVals[4].Trim().Length > 0)
                                {
                                    theValue = fieldVals[4].Trim();
                                    if (!double.TryParse(theValue, out poleOriginalCirc))
                                        Debug.Print("Error");

                                    //if (!theValue.EndsWith(".00"))
                                    //    processPole = true;
                                }

                                //EFFECTIVECIRCUMFERENCE 
                                if (fieldVals[5].Trim().Length > 0)
                                {
                                    theValue = fieldVals[5].Trim();
                                    if (!double.TryParse(theValue, out poleEffectCirc))
                                        Debug.Print("Error");

                                    //if (!theValue.EndsWith(".00"))
                                    //    processPole = true;
                                }

                                //LASTINSPECTIONDATETIME 
                                if (fieldVals[6].Trim().Length > 0)
                                {
                                    dateTimeStr = fieldVals[6].Trim();
                                    if (dateTimeStr != "0")
                                    {
                                        year = Convert.ToInt32(dateTimeStr.Substring(0, 4));
                                        month = Convert.ToInt32(dateTimeStr.Substring(4, 2));
                                        day = Convert.ToInt32(dateTimeStr.Substring(6, 2));
                                        hour = Convert.ToInt32(dateTimeStr.Substring(8, 2));
                                        minute = Convert.ToInt32(dateTimeStr.Substring(10, 2));
                                        second = Convert.ToInt32(dateTimeStr.Substring(12, 2));
                                        pLastInspectDate = new DateTime(year, month, day, hour, minute, second);
                                    }
                                }

                                ////REMAININGSTRENGTH 
                                if (fieldVals[7].Trim().Length > 0)
                                {
                                    theValue = fieldVals[7].Trim();
                                    //if (theValue.EndsWith(".00"))
                                    //    theValue = theValue.Substring(0, theValue.Length - 3);
                                }

                                if (year < 2013)
                                {
                                    //Take it from the text file 
                                    if ((poleEffectCirc <= poleOriginalCirc) && 
                                        (poleOriginalCirc != 0) &&
                                        (poleEffectCirc != 0))
                                    {
                                        processPole = true; 
                                        double theCub = Math.Pow((poleEffectCirc / poleOriginalCirc), 3);
                                        poleRemainingStrength = Math.Round(theCub,2)*100;

                                        if (poleRemainingStrength != 100)
                                            Debug.Print("not 100");                                                                                 
                                    }
                                }

                                //Sync the pole 
                                if (processPole)
                                {
                                    updateCount++;
                                    SyncPole(
                                        poleSapEquipId,
                                        poleRemainingStrength.ToString());
                                }
                            }
                            else
                            {
                                Shared.WriteToLogfile("Field sequencing error with line: " + line);
                            }
                        }

                        counter++;
                        //Shared.WriteToLogfile("Finished line: " + counter.ToString());
                    }

                    Shared.WriteToLogfile("Finished entire file: " + filename);
                    file.Close();
                }
            }
            catch (Exception ex)
            {
                Debug.Print("Error");

            }
        }

        public void SyncPoles2(
            string logfileSuffix,
            string folder)
        {
            try
            {
                Shared.LoadConfigurationSettings(logfileSuffix);
                Shared.InitializeLogfile();
                Shared.WriteToLogfile("Entering " + "SyncPoles2");

                int counter = 0;
                int updateCount = 0;
                string line;
                string dateTimeStr = string.Empty;
                int year;
                int month;
                int day;
                int hour;
                int minute;
                int second;
                int poleHeight = 0;
                int poleClass = 0;
                string poleSpecies = string.Empty;
                string poleSapEquipId = string.Empty;
                double poleOriginalCirc = 0;
                double poleEffectCirc = 0;
                int poleRemainingStrengthInt = 0;
                double poleRemainingStrengthDbl = 0;
                bool jointOwned = false;
                Nullable<DateTime> pLastInspectDate = null;
                string filename = string.Empty;

                DirectoryInfo dirInfo = new DirectoryInfo(folder);
                FileInfo[] finfos = dirInfo.GetFiles();

                for (int i = 0; i < finfos.Length; i++)
                {

                    filename = finfos[i].FullName;
                    counter = 0;
                    updateCount = 0;
                    System.IO.StreamReader file =
                        new System.IO.StreamReader(filename);

                    while ((line = file.ReadLine()) != null)
                    {

                        //Reset variables 
                        bool processPole = false;
                        string theValue = string.Empty;
                        poleSapEquipId = string.Empty;
                        poleHeight = 0;
                        poleClass = 0;
                        poleSpecies = string.Empty;
                        poleOriginalCirc = 0;
                        poleEffectCirc = 0;
                        pLastInspectDate = null;
                        poleRemainingStrengthInt = 0;
                        poleRemainingStrengthDbl = 0;
                        year = 0;

                        //Ignore the header line 
                        if (counter != 0)
                        {
                            string[] fieldVals = line.Split('\t');
                            

                            if (fieldVals.Length == 8)
                            {
                                //SAPEQUIPID  
                                if (fieldVals[0].Trim().Length > 0)
                                {
                                    poleSapEquipId = fieldVals[0].Trim();
                                }

                                if (poleSapEquipId == "101856541")
                                    Debug.Print("this is the one");

                                //2.HEIGHT_SAP 
                                if (fieldVals[1].Trim().Length > 0)
                                    Int32.TryParse(fieldVals[1].Trim(), out poleHeight);

                                //3.CLASS_SAP 
                                if (fieldVals[2].Trim().Length > 0)
                                    Int32.TryParse(fieldVals[2].Trim(), out poleClass);

                                //4.SPECIES_SAP 
                                if (fieldVals[3].Trim().Length > 0)
                                    poleSpecies = fieldVals[3].Trim();


                                //ORIGINALCIRCUMFERENCE 
                                if (fieldVals[4].Trim().Length > 0)
                                {
                                    theValue = fieldVals[4].Trim();
                                    if (!double.TryParse(theValue, out poleOriginalCirc))
                                        Debug.Print("Error");

                                    //if (!theValue.EndsWith(".00"))
                                    //    processPole = true;
                                }

                                //EFFECTIVECIRCUMFERENCE 
                                if (fieldVals[5].Trim().Length > 0)
                                {
                                    theValue = fieldVals[5].Trim();
                                    if (!double.TryParse(theValue, out poleEffectCirc))
                                        Debug.Print("Error");

                                    //if (!theValue.EndsWith(".00"))
                                    //    processPole = true;
                                }

                                //LASTINSPECTIONDATETIME 
                                if (fieldVals[6].Trim().Length > 0)
                                {
                                    dateTimeStr = fieldVals[6].Trim();
                                    if (dateTimeStr != "0")
                                    {
                                        year = Convert.ToInt32(dateTimeStr.Substring(0, 4));
                                        month = Convert.ToInt32(dateTimeStr.Substring(4, 2));
                                        day = Convert.ToInt32(dateTimeStr.Substring(6, 2));
                                        hour = Convert.ToInt32(dateTimeStr.Substring(8, 2));
                                        minute = Convert.ToInt32(dateTimeStr.Substring(10, 2));
                                        second = Convert.ToInt32(dateTimeStr.Substring(12, 2));
                                        pLastInspectDate = new DateTime(year, month, day, hour, minute, second);
                                    }
                                }

                                ////REMAININGSTRENGTH 
                                if (fieldVals[7].Trim().Length > 0)
                                {
                                    theValue = fieldVals[7].Trim();
                                    if (!Int32.TryParse(theValue, out poleRemainingStrengthInt))
                                        Debug.Print("Error");
                                }

                                if (year < 2013)
                                {
                                    //Take it from the text file 
                                    if ((poleEffectCirc <= poleOriginalCirc) &&
                                        (poleOriginalCirc != 0) &&
                                        (poleEffectCirc != 0))
                                    {
                                        //processPole = true;
                                        double theCub = Math.Pow((poleEffectCirc / poleOriginalCirc), 3);
                                        poleRemainingStrengthDbl = Math.Round(theCub, 2) * 100;

                                        if (!Int32.TryParse(poleRemainingStrengthDbl.ToString(), out poleRemainingStrengthInt))
                                            Debug.WriteLine("There is a problem"); 

                                        if (poleRemainingStrengthInt != 100)
                                            Debug.Print("not 100");
                                    }
                                }

                                if (poleRemainingStrengthInt == 0)
                                    poleRemainingStrengthInt = 100;
                                if (poleRemainingStrengthInt > 100)
                                    poleRemainingStrengthInt = 100;

                                //Sync the pole 
                                updateCount++;
                                SyncPole(
                                    poleSapEquipId,
                                    poleHeight.ToString(),
                                    poleClass.ToString(),
                                    poleSpecies,
                                    poleOriginalCirc.ToString(),
                                    poleEffectCirc.ToString(),
                                    poleRemainingStrengthInt.ToString(),
                                    pLastInspectDate);
                            }
                            else
                            {
                                Shared.WriteToLogfile("Field sequencing error with line: " + line);
                            }
                        }

                        counter++;
                        //Shared.WriteToLogfile("Finished line: " + counter.ToString());
                    }

                    Shared.WriteToLogfile("Finished entire file: " + filename);
                    file.Close();
                }
            }
            catch (Exception ex)
            {
                Debug.Print("Error");

            }
        }

        public void PopulatePLC_InfoFeatureclass(string logfileSuffix)
        {
            SqlDataReader reader = null;
            IFeatureClassLoad featureClassLoad = null;
            int recCount = 0; 

            try
            {
                //Get a handle on the output workspace                
                Shared.LoadConfigurationSettings(logfileSuffix);
                Shared.InitializeLogfile();
                Shared.WriteToLogfile("Entering " + "PopulatePLC_InfoFeatureclass");

                //Step 1. Populate a list of SAPEQUIPIDs already loaded 
                //into the featureclass 
                Shared.LoadWorkspaces(); 
                IFeatureWorkspace pFWS = (IFeatureWorkspace)Shared.GetWorkspaceByName("output");
                IWorkspaceEdit pWSE = (IWorkspaceEdit)pFWS;
                Hashtable hshExistingSapEquipIDs = GetExistingPLDSapEquipIds(pFWS);

                //Step 2. Query PLDB for all Poles and join with Analysis 
                //table (PoleAnalysis Query)  
                reader = GetPoleAnalysisQuery();
                
                //First delete all existing rows if necessary 
                IFeatureClass pDestFC = pFWS.OpenFeatureClass("PLD_Info");
                
                //Get the input and output spatial references 
                int geoType = (int)ESRI.ArcGIS.Geometry.esriSRGeoCSType.esriSRGeoCS_WGS1984;
                ISpatialReferenceFactory pSRF = new SpatialReferenceEnvironmentClass();
                ISpatialReference pWGS84_SR = pSRF.CreateGeographicCoordinateSystem(geoType);
                ISpatialReference pUTM10_SR = ((IGeoDataset)pDestFC).SpatialReference;

                //Step 3. Loop through the records in the PoleAnalysis query 
                //and where they have not been already added populate the 
                //PLC_Info featureclass 

                //Get the field indexes
                int PLDBIDfldIdx = pDestFC.Fields.FindField("PLDBID"); 
                int sapEquipIdFldIdx = pDestFC.Fields.FindField("SAPEQUIPID");
                int pldbStatusFldIdx = pDestFC.Fields.FindField("PLD_STATUS");
                int latFldIdx = pDestFC.Fields.FindField("LAT");
                int longFldIdx = pDestFC.Fields.FindField("LONGITUDE");
                int elevationFldIdx = pDestFC.Fields.FindField("ELEVATION");
                int horizSFFldIdx = pDestFC.Fields.FindField("HORIZONTAL_SF");
                int overallSFFldIdx = pDestFC.Fields.FindField("OVERALL_SF");
                int bendingSFFldIdx = pDestFC.Fields.FindField("BENDING_SF");
                int verticalSFFldIdx = pDestFC.Fields.FindField("VERTICAL_SF");
                int classFldIdx = pDestFC.Fields.FindField("CLASS");
                int lengthInInchesFldIdx = pDestFC.Fields.FindField("LENGTHININCHES");
                int speciesFldIdx = pDestFC.Fields.FindField("SPECIES");
                int globalIdFldIdx = pDestFC.Fields.FindField("GLOBALID_PLD");
                int lanIdFldIdx = pDestFC.Fields.FindField("LANID");
                int snowLoadFldIdx = pDestFC.Fields.FindField("SNOW_LOAD_DIST");
                int oderDescriptionFldIdx = pDestFC.Fields.FindField("ORDER_DESCRIPTION");

                int newFeatureCount = 0;
                int editCount = 0;
                int sapEquipId = 0;
                string sapEquipIdString = string.Empty; 
                IPoint pPoint = null;
                double latitude = 0;
                double longitude = 0;

                pWSE.StartEditing(false);
                pWSE.StartEditOperation();

                //If this is not an existing sapEquipId then add the 
                //feature to the featureclass 

                // Cast the feature class to the IFeatureClassLoad interface.
                featureClassLoad = (IFeatureClassLoad)pDestFC;

                // Enable load-only mode on the feature class (much faster) 
                featureClassLoad.LoadOnlyMode = true;

                //Create an insert featurecusor                     
                IFeatureCursor pInsertCursor = pDestFC.Insert(true);
                IFeatureBuffer pInsertFB = pDestFC.CreateFeatureBuffer();

                while (reader.Read())
                {
                    //Loop through the features to clip   
                    recCount++;

                    //Check we do not already have this feature 
                    sapEquipIdString = reader.GetString(6).Trim();
                    if (Int32.TryParse(sapEquipIdString, out sapEquipId))
                    {
                        if (!hshExistingSapEquipIDs.ContainsKey(sapEquipId))
                        {
                            //Set all the attributes 

                            //Get the shape and project 
                            latitude = Convert.ToDouble(reader.GetString(11).Trim());
                            longitude = Convert.ToDouble(reader.GetString(12).Trim());

                            //SAPEQUIPID 
                            pInsertFB.set_Value(sapEquipIdFldIdx, sapEquipIdString);

                            //PLDBIDID 
                            pInsertFB.set_Value(PLDBIDfldIdx, reader.GetInt64(0));

                            //PLDB_STATUS
                            pInsertFB.set_Value(pldbStatusFldIdx, "BaseLine");

                            //LAT
                            pInsertFB.set_Value(latFldIdx, latitude);

                            //LONGITUDE
                            pInsertFB.set_Value(longFldIdx, longitude);

                            //ELEVATION
                            pInsertFB.set_Value(elevationFldIdx, reader.GetString(13));

                            //HORIZONTAL_SF
                            pInsertFB.set_Value(horizSFFldIdx, reader.GetValue(2));

                            //OVERALL_SF
                            pInsertFB.set_Value(overallSFFldIdx, reader.GetValue(3));

                            //BENDING_SF
                            pInsertFB.set_Value(bendingSFFldIdx, reader.GetValue(4));

                            //VERTICAL_SF
                            pInsertFB.set_Value(verticalSFFldIdx, reader.GetValue(5));

                            //CLASS 
                            pInsertFB.set_Value(classFldIdx, reader.GetString(9));

                            //LENGTHININCHES 
                            pInsertFB.set_Value(lengthInInchesFldIdx, reader.GetValue(1));

                            //SPECIES 
                            pInsertFB.set_Value(speciesFldIdx, reader.GetValue(10));

                            //GLOBALID_PLD 
                            pInsertFB.set_Value(globalIdFldIdx, reader.GetValue(7));

                            //LANID 
                            pInsertFB.set_Value(lanIdFldIdx, "Unset");

                            //SNOW_LOAD_DIST 
                            pInsertFB.set_Value(snowLoadFldIdx, reader.GetValue(8));

                            //ORDER_DESCRIPTION 
                            pInsertFB.set_Value(oderDescriptionFldIdx, sapEquipId.ToString());

                            //Set the shape 
                            pPoint = new PointClass();
                            pPoint.PutCoords(longitude, latitude);
                            pPoint.SpatialReference = pWGS84_SR;
                            pPoint.Project(pUTM10_SR);
                            pInsertFB.Shape = pPoint;

                            //Insert the new feature 
                            newFeatureCount++;
                            editCount++;
                            try
                            {
                                pInsertCursor.InsertFeature(pInsertFB);
                            }
                            catch (Exception ex)
                            {
                                //Write("Failed to insert featurebuffer on: " +
                                //    pDestDS.Name + " oid: " + pSourceFeature.OID.ToString() +
                                //    " error: " + ex.Message);
                                throw new Exception("Unable to insert feature");
                            }

                            //Flush every record 
                            if (newFeatureCount == 5000)
                            {
                                Shared.WriteToLogfile("Written: " + editCount.ToString() + " Poles");
                                try
                                { pInsertCursor.Flush(); }
                                catch
                                { throw new Exception("Unable to flush feature buffer"); }
                                newFeatureCount = 0;
                            }
                        }
                    }
                }

                if (reader != null)
                    reader.Close();

                //Flush if necessary 
                if (newFeatureCount != 0)
                    pInsertCursor.Flush();

                //Stop edit operation 
                pWSE.StopEditOperation();
                pWSE.StopEditing(true);

                //Release the cursor and buffer 
                Marshal.FinalReleaseComObject(pInsertCursor);
                Marshal.FinalReleaseComObject(pInsertFB);                               
                
            }
            catch (Exception ex)
            {
                if (reader != null)
                    reader.Close();
                MessageBox.Show("Error in PopulatePLC_InfoFeatureclass: " + ex.Message);
            }
        }

        private SqlDataReader GetPoleAnalysisQuery()
        {
            try
            {
                //Horizontal_SF is mapped to PoleStrengthFactor
                //overall_SF is mapped to PoleFactorOfSafety
                //VERTICAL_SF is mapped to VerticalFactorOfSafety

                if (_pConn == null)
                {
                    //Get output DB connection 
                    _pConn = new SqlConnection(
                        Shared.OutputConnectString);
                }
                if (_pConn.State != System.Data.ConnectionState.Open)
                    _pConn.Open();

                //Setup the command 
                SqlCommand pCmd = _pConn.CreateCommand();

                //Run an INSERT INTO SELECT statement for the error 
                string sql =
                    "SELECT PGE_Combined.dbo.OCalcProAnalysis.PLDBID, " +
                    "PGE_Combined.dbo.OCalcProAnalysis.LenghtInInches, " +
                    "PGE_Combined.dbo.OCalcProAnalysis.PoleStrengthFactor, " +
                    "PGE_Combined.dbo.OCalcProAnalysis.PoleFactorOfSafety, " +
                    "PGE_Combined.dbo.OCalcProAnalysis.BendingFactorOfSafety, " +
                    "PGE_Combined.dbo.OCalcProAnalysis.VerticalFactorOfSafety, " +
                    "PGE_Combined.dbo.Poles.SAPEQUIPID, " +
                    "PGE_Combined.dbo.Poles.POLEGUID, " +
                    "PGE_Combined.dbo.Poles.SNOW_LOADING_ZONE, " +
                    "PGE_Combined.dbo.Poles.CLASS, " +
                    "PGE_Combined.dbo.Poles.SPECIES, " +
                    "PGE_Combined.dbo.Poles.LATITUDE, " +
                    "PGE_Combined.dbo.Poles.LONGITUDE, " +
                    "PGE_Combined.dbo.Poles.ELEVATION " +
                    "FROM " +
                    "PGE_Combined.dbo.OCalcProAnalysis " +
                    "INNER JOIN " +
                    "PGE_Combined.dbo.Poles " +
                    "ON PGE_Combined.dbo.OCalcProAnalysis.PGE_GLOBALID = PGE_Combined.dbo.Poles.POLEGUID;";

                pCmd.CommandText = sql;
                string sapIdString = string.Empty;
                SqlDataReader reader = pCmd.ExecuteReader();
                return reader; 
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error in GetPoleAnalysisQuery: " + ex.Message);
                throw new Exception("Error returning Pole Analysis query");
            }
        }
        private Hashtable GetExistingPLDSapEquipIds(IFeatureWorkspace pFWS)
        {
            try
            {
                Hashtable hshExistingSapEquipIds = new Hashtable();                
                IFeatureClass pPLD_InfoFC = pFWS.OpenFeatureClass("PLD_Info");
                //((ITable)pPLD_InfoFC).DeleteSearchedRows(null);
                IFeatureCursor pFCursor = pPLD_InfoFC.Search(null, false);
                int fldIdx = pPLD_InfoFC.Fields.FindField("sapequipid");
                Int32 sapEquipid = -1;
                string sapEquipIdString = string.Empty; 

                IFeature pFeature = pFCursor.NextFeature(); 
                while (pFeature != null)
                {
                    sapEquipIdString = pFeature.get_Value(fldIdx).ToString().Trim();
                    if (Int32.TryParse(sapEquipIdString, out sapEquipid))
                    {
                        if (!hshExistingSapEquipIds.ContainsKey(sapEquipid))
                            hshExistingSapEquipIds.Add(sapEquipid, null);
                    }
                    else
                    {
                        Debug.Print("equipid problem");
                    }
                    pFeature = pFCursor.NextFeature();
                }
                Marshal.FinalReleaseComObject(pFCursor); 
                return hshExistingSapEquipIds; 
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error in GetExistingPLDSapEquipIds: " + ex.Message);
                throw new Exception("Error returning existing list of PLD features");
            }
        }


        public bool PoleExists(string sapEquipId, ref Nullable<DateTime> pLastInspectionDate)
        {
            try
            {
                Shared.WriteToLogfile("Entering " + MethodBase.GetCurrentMethod());

                if (_pConn == null)
                {
                    //Get output DB connection 
                    _pConn = new SqlConnection(
                        Shared.OutputConnectString);
                }
                if (_pConn.State != System.Data.ConnectionState.Open)
                    _pConn.Open();

                //Setup the command 
                SqlCommand pCmd = _pConn.CreateCommand();

                //Run an INSERT INTO SELECT statement for the error 
                //Hashtable hshExistingPoleSapIds = new Hashtable();
                string sql = "SELECT" + " " + "LASTINSPECTIONDATETIME" + " " +
                    "FROM" + " " +
                    PoleLoadConstants.POLES_TBL + " " + 
                    "WHERE" + " SAPEQUIPID = '" + sapEquipId + "';";

                pCmd.CommandText = sql;
                Int32 sapId = -1;
                string sapIdString = string.Empty;
                SqlDataReader reader = pCmd.ExecuteReader();

                if (!reader.HasRows)
                {
                    reader.Close(); 
                    return false;
                }

                //DateTime pInspDate = new DateTime(1900, 1, 1);

                while (reader.Read())
                {
                        if (!reader.IsDBNull(0))
                            pLastInspectionDate = reader.GetDateTime(0);

                }
                if (reader != null)
                    reader.Close();


                //Return the hashtable 
                return true;
            }
            catch (Exception ex)
            {
                Shared.WriteToLogfile("Entering error handler for " + MethodBase.GetCurrentMethod() + " error: " +
                    ex.Message);
                throw new Exception("Error returning existing Pole SapEquipIds");
            }
        }


        /// <summary>
        /// Return a hashtable of OIDs to be processed 
        /// </summary>
        /// <param name="lastDigitsOfOId"></param>
        /// <param name="hshSapIds"></param>
        /// <returns></returns>
        private Hashtable GetPolesToProcess(
            string lastDigitsOfOId, 
            Hashtable hshSapIds)
        {
            try
            {
                Shared.WriteToLogfile("Entering " + MethodBase.GetCurrentMethod());
                string[] commaSeparatedkeyList = GetArrayOfCommaSeparatedKeys( 
                    hshSapIds, 500, true);
                
                Hashtable hshDigits = new Hashtable();
                Hashtable hshOIDs = new Hashtable();
                string lastDigit = string.Empty;
                string[] digits = lastDigitsOfOId.Split(',');
                for (int i = 0; i < digits.Length; i++)
                {
                    if (!hshDigits.ContainsKey(digits[i]))
                        hshDigits.Add(digits[i], 0);
                }

                string sql = "";
                IQueryFilter pQF = new QueryFilterClass(); 
                
                for (int i = 0; i < commaSeparatedkeyList.Length; i++)
                {
                    sql = "sapequipid" + " IN(" + commaSeparatedkeyList[i] + ")";
                    pQF.WhereClause = sql; 
                    IFeatureCursor pFCursor = m_SupportStructureFC.Search(pQF, false);
                    IFeature pFeature = pFCursor.NextFeature();
                    while (pFeature != null)
                    {
                        lastDigit = pFeature.OID.ToString().Substring(
                            pFeature.OID.ToString().Length - 1, 1);
                        if (hshDigits.ContainsKey(lastDigit))
                        {
                            //If not already processed 
                            if (!hshOIDs.ContainsKey(pFeature.OID))
                                hshOIDs.Add(pFeature.OID, 0);
                            else
                                Debug.Print("Already have this one");                             

                        }
                        pFeature = pFCursor.NextFeature();
                    }
                    Marshal.FinalReleaseComObject(pFCursor);
                }

                return hshOIDs;
            }
            catch (Exception ex)
            {
                Shared.WriteToLogfile("Error in " + MethodBase.GetCurrentMethod() +
                    " details: " + ex.Message);
                throw ex;
            }
        }

        /// <summary>
        /// Returns a lit of comma separated keys to allow use in a SQL 
        /// IN() clause 
        /// </summary>
        /// <param name="hshKeys"></param>
        /// <param name="batchSize"></param>
        /// <param name="addApostrophe"></param>
        /// <returns></returns>
        private string[] GetArrayOfCommaSeparatedKeys(Hashtable hshKeys, int batchSize, bool addApostrophe)
        {
            try
            {
                Hashtable hshCommaSeparatedKeys = new Hashtable();
                int counter = 0;
                StringBuilder batchLine = new StringBuilder();

                foreach (object key in hshKeys.Keys)
                {
                    if (counter == 0)
                    {
                        if (addApostrophe)
                            batchLine.Append("'" + key.ToString() + "'");
                        else
                            batchLine.Append(key.ToString());
                    }
                    else
                    {
                        if (addApostrophe)
                            batchLine.Append("," + "'" + key.ToString() + "'");
                        else
                            batchLine.Append("," + key.ToString());
                    }

                    counter++;
                    if (counter == batchSize)
                    {
                        hshCommaSeparatedKeys.Add(batchLine.ToString(), 0);
                        batchLine = new StringBuilder();
                        counter = 0;
                    }
                }

                //Add what is left over 
                if (batchLine.ToString().Length != 0)
                    hshCommaSeparatedKeys.Add(batchLine.ToString(), 0);

                //Convert this to an array 
                counter = 0;
                string[] commaSepKeys = new string[hshCommaSeparatedKeys.Count];
                foreach (string line in hshCommaSeparatedKeys.Keys)
                {
                    commaSepKeys[counter] = line;
                    counter++;
                }

                //return array 
                return commaSepKeys;
            }
            catch (Exception ex)
            {
                Shared.WriteToLogfile("Error in " + MethodBase.GetCurrentMethod() +
                    " details: " + ex.Message);
                throw new Exception("Error returning comma separate list of keys");
            }
        }

        /// <summary>
        /// Performs Pole Loading Track 3 extraction for the passed pole 
        /// Including calculation of spans for the pole, determining devices 
        /// that sit on the pole etc. and outputting them to CSV or SQL Server 
        /// </summary>
        private void ProcessPole(
            int poleOId,
            IIdentify pIdentify, 
            bool displayForm)
        {
            try
            {
                Shared.WriteToLogfile("Entering " + MethodBase.GetCurrentMethod());
                Shared.WriteToLogfile(" OId: " + poleOId.ToString());

                //=====================================================================
                //Logic Followed 

                //Step 1 Get AllOverheadConductors  
                // get all overhead conductors that could be connected to adjacent poles 
                // use a search tolerance of maxpolelength feet 

                //Step 2 Get AllPoles
                // get all poles that may be adjacent and connected to the ref pole through 
                // a span, use a search tolerance of maxpolelength feet

                //Setp 3 Get AllConnectedDevices 
                // get all network devices that are supposed to be on the ref pole, include the 
                // following devices: 
                //      Switch 
                //      Fuse 
                //      Transformer 
                //      VoltageRegulator 
                //      Streetlight 
                //      OpenPoint 
                //      CapacitorBank 
                //      DynamicProtectiveDevice 
                //      FaultIndicator 
                //      PrimaryMeter 
                //      PrimaryRiser 
                //      SmartMeterNetworkDevice 
                //      StepDown 

                //Step 4 Get AllNearbyConductors 
                // Get nearby conductors which are all poles that are within the 
                // AllOverheadConductorsList that are within searchtol feet of the ref pole OR 
                // that is connected to a device from the  AllConnectedDevices list 

                //Step 5 delineate the span list for the ref pole AllSpans  
                //For each of the AllNearbyConductors list 
                // a)find the point on the conductor closest to the pole 
                // if this point is not an end point it means the trace will have to go in both 
                // directions 
                // b)trace along each conductor and with each segment that is extended 
                // look for an adjacent pole 
                // c)if nothing is found use a routine to find the connected edges until you 
                // find an adjacent pole (trace through any switches whether open or closed) if 
                // you reach a point that is already in the segmentcollection then you can stop 
                // tracing this branch, if the topoint is cincident with another device that is on 
                // the reference pole then you can stop tracing this branch 
                // d) as you trace along you will build up a polyline, this will form the reference 
                // polyline for the span and should be saved with the span for debugging purposes  
                // e) once you find an adjacent pole stop and at this point you have the span 
                // f) if you never reach an adjacent pole then flag this in the log file 

                //Step 6 write the span information and the static pole information etc 

                //done 
                //===================================================================== 

                //Create a circle polygon with the maxpolelength radius 
                //around the geometry of the reference pole (pMaxSpanPolygon)
                //IQueryFilter pQF = new QueryFilterClass();
                //pQF.WhereClause = "SAPEQUIPID = '" + sapEquipId.ToString() + "'";
                //IFeatureCursor pFCursor = m_SupportStructureFC.Search(pQF, false);
                //IFeature pRefPoleFeature = pFCursor.NextFeature();
                //Marshal.FinalReleaseComObject(pFCursor);
                IFeature pRefPoleFeature = m_SupportStructureFC.GetFeature(poleOId);
                if (pRefPoleFeature == null)
                {
                    //Exit out as the referebce pole was not found 
                    Shared.WriteToLogfile("Error: Unable to find ref pole with OId: " + poleOId.ToString());
                    return;
                }
                if (pRefPoleFeature.Shape == null)
                {
                    //Exit out as the reference pole must have a valid shape  
                    Shared.WriteToLogfile("Error: Unable to process ref pole with OId: " + poleOId.ToString() + 
                        " because the shape is null");
                    return;
                }
                Shared.WriteToLogfile("ObjectId: " + pRefPoleFeature.OID.ToString());
                
                //Return the reference pole 
                int fldIdx = m_SupportStructureFC.Fields.FindField("globalid");
                string poleGlobalId = pRefPoleFeature.get_Value(fldIdx).ToString();
                Pole pRefPole = new Pole(poleGlobalId, (IPoint)pRefPoleFeature.ShapeCopy);
                double refPoleRL = GetHeightAtPoint(pRefPole.Shape, pIdentify);

                //Populate the Snow Loading Zone, POP Density
                string snowLoadingZone = GetPointInPolygonLandbaseValue(
                                (IPoint)pRefPoleFeature.ShapeCopy,
                                m_SnowLoadingFC,
                                "SNOWLOAD",
                                "Light");
                string popDensity = GetPointInPolygonLandbaseValue(
                            (IPoint)pRefPoleFeature.ShapeCopy,
                            m_PopDensityFC,
                            "DENSITY",
                            "MEDIUM (60-1000 per sqmi)");

                //Get the max span polygon based on the population density 
                //as in denser areas we can use a smaller buffer 
                int maxSpanLength = GetMaxSpanLength(popDensity);
                IPolygon pMaxSpanPolygon = GetBufferPolygon(
                    (IPoint)pRefPoleFeature.ShapeCopy, maxSpanLength);

                //Get all the conductors in the vicinity 
                List<Conductor> pAllConductors = new List<Conductor>();
                GetAllOverheadConductors(ref pAllConductors, pMaxSpanPolygon);
                Shared.WriteToLogfile("found: " + pAllConductors.Count.ToString() + " overhead conductors in vicinity");

                //Get all the poles in the vicinity  
                List<Pole> pAllPoles = new List<Pole>();
                GetAllPoles(ref pAllPoles, pMaxSpanPolygon);
                Shared.WriteToLogfile("found: " + pAllPoles.Count.ToString() + " poles in vicinity");

                //Get all the devices related to the pole 
                List<RelatedDevice> pAllRelatedDevices = new List<RelatedDevice>();
                GetAllRelatedDevices(pRefPoleFeature, ref pAllRelatedDevices);
                Shared.WriteToLogfile("found: " + pAllRelatedDevices.Count.ToString() + " related devices");

                //Create a circle polygon with the tolerance radius 
                //around the geometry of the reference pole (pPoleTolPolygon)
                IPolygon pPoleTolPolygon = GetBufferPolygon(
                    (IPoint)pRefPoleFeature.ShapeCopy, Shared.PoleBuffer);
                List<Conductor> pNearbyConductors = new List<Conductor>();
                GetNearbyConductors(
                    pRefPole,
                    pPoleTolPolygon, 
                    pAllPoles, 
                    pAllConductors,
                    pAllRelatedDevices,
                    ref pNearbyConductors);
                Shared.WriteToLogfile("found: " + pNearbyConductors.Count.ToString() + " nearby conductors");

                //IMap pmap = ((IMxDocument)m_app.Document).FocusMap;
                //IActiveView pAV = (IActiveView)pmap;
                //ESRI.ArcGIS.Carto.IGraphicsContainer pGC = (ESRI.ArcGIS.Carto.IGraphicsContainer)pmap;
                //pGC.DeleteAllElements(); 
                //foreach (Conductor pTempConductor in pNearbyConductors)
                //{
                //    GraphicUtils.DrawPolyline(pmap, pTempConductor.Shape, GraphicUtils.GetRGBColour(0, 0, 0), "");

                //    pAV.Refresh(); 
                //    MessageBox.Show("here is one");
                //}

                //Delineate the spans by tracing along the nearby conductors
                //pGC.DeleteAllElements();

                List<Span> pSpans = new List<Span>();
                DelineateSpanList(
                    pAllConductors,
                    pAllPoles,
                    pIdentify,
                    pRefPole, 
                    refPoleRL, 
                    pAllRelatedDevices,
                    pNearbyConductors,
                    ref pSpans);
                Shared.WriteToLogfile("found: " + pSpans.Count.ToString() + " spans");

                //foreach (Span pSpan in pSpans)
                //{
                //    if (IsConincident(pSpan.SpanPolyline.FromPoint, pSpan.SpanPolyline.ToPoint))
                //    {
                //        Debug.Print("found one"); 
                //    }
                //}

                //1.Write out pole static attribution
                WritePoleToDB(pRefPoleFeature, refPoleRL, snowLoadingZone, popDensity);

                //2. Write out span angle information 
                WriteSpanListToDB(pSpans, pAllConductors, snowLoadingZone, popDensity);

                //3. Write out related conductor / device information 
                WriteEquipmentToDB(
                    pRefPole,
                    pAllRelatedDevices);

                //Can bring up the form to display the pole 
                //if (displayForm)
                //{
                //    frmDisplayPole frm = new frmDisplayPole(
                //        sapEquipId.ToString(),
                //        pSpans,
                //        pNearbyConductors,
                //        pAllRelatedDevices,
                //        m_app);
                //    frm.Show();
                //}
            }
            catch (Exception ex)
            {
                Shared.WriteToLogfile("Error in " + MethodBase.GetCurrentMethod() +
                    " details: " + ex.Message);
                throw new Exception("Error processing pole: " + poleOId.ToString());
            }
        }

        private void SyncPole(
            IFeature pRefPoleFeature)
        {
            try
            {
                Hashtable hshPoleFields = (Hashtable)_hshAllAttributes["SUPPORTSTRUCTURE"];
                //IFeature pRefPoleFeature = m_SupportStructureFC.GetFeature(poleOId);
                int poleOId = pRefPoleFeature.OID; 
                if (pRefPoleFeature == null)
                {
                    //Exit out as the referebce pole was not found 
                    Shared.WriteToLogfile("Error: Unable to find ref pole with OId: " + poleOId.ToString());
                    return;
                }
                if (pRefPoleFeature.Shape == null)
                {
                    //Exit out as the reference pole must have a valid shape  
                    Shared.WriteToLogfile("Error: Unable to process ref pole with OId: " + poleOId.ToString() +
                        " because the shape is null");
                    return;
                }
                //Shared.WriteToLogfile("ObjectId: " + pRefPoleFeature.OID.ToString());

                if (_pConn == null)
                {
                    //Get output DB connection 
                    _pConn = new SqlConnection(
                        Shared.OutputConnectString);
                }
                if (_pConn.State != System.Data.ConnectionState.Open)
                    _pConn.Open();

                //String to hold the SQL insert statement 
                string sqlStatement = string.Empty;
                string zoneValue = string.Empty;
                int jointCount = 0;
                string jointCountString = string.Empty;
                string sapEquipId = string.Empty;
                bool jointOwned = false;
                int fldIdx = -1;
                SqlCommand pCmd = null;
                
                if (pRefPoleFeature != null)
                {
                    fldIdx = Convert.ToInt32(hshPoleFields["SAPEQUIPID"]);
                    if (pRefPoleFeature.get_Value(fldIdx) != DBNull.Value)
                    {
                        sapEquipId = GetFieldValue(pRefPoleFeature, fldIdx);
                    }

                    //JOINT_OWNED  
                    //Changed on 07/19 from direction from Alasdair / Robert to accommodate 
                    //the Contact Pole scenario 
                    jointCount = 1;
                    jointOwned = false;
                    fldIdx = Convert.ToInt32(hshPoleFields["JOINTCOUNT"]);
                    if (pRefPoleFeature.get_Value(fldIdx) != DBNull.Value)
                    {
                        jointCountString = GetFieldValue(pRefPoleFeature, fldIdx);
                        jointCountString = jointCountString.Substring(0, 1);
                        Int32.TryParse(jointCountString, out jointCount);
                    }
                    ISet pJointUseAttachSet = m_JointUseAttachmentRC.GetObjectsRelatedToObject((IObject)pRefPoleFeature);
                    pJointUseAttachSet.Reset();
                    //If jointcount greater than 1 or there is at least 1 joint use attachment then 
                    //JOINT_OWNED = TRUE (otherwise FALSE)
                    if ((pJointUseAttachSet.Count > 0) || (jointCount > 1))
                        jointOwned = true;
                    Marshal.FinalReleaseComObject(pJointUseAttachSet);
                    //valuesList.Append("'" + jointOwned.ToString().ToUpper() + "'");

                    //Write the insert statement 
                    sqlStatement = "UPDATE [PGE_Combined].[dbo].[POLES] SET JOINT_OWNED = '" + jointOwned.ToString().ToUpper() + "' WHERE SAPEQUIPID = '" + sapEquipId + "'" + ";";
                }

                //Execute the command against the database 
                if (sqlStatement != string.Empty)
                {
                    Shared.WriteToLogfile(sqlStatement);

                    //Shared.WriteToLogfile(sqlStatement);
                    pCmd = _pConn.CreateCommand();
                    pCmd.Connection = _pConn;
                    pCmd.CommandText = sqlStatement;
                    int recordsUpdated = pCmd.ExecuteNonQuery();
                    if (recordsUpdated == 0)
                        Shared.WriteToLogfile("SQL insert failed for pole: " + sapEquipId);
                }

            }
            catch (Exception ex)
            {
                Shared.WriteToLogfile("Error in " + MethodBase.GetCurrentMethod() +
                    " details: " + ex.Message);
                throw new Exception("Error processing pole: " + ex.Message);
            }
        }

        private void SyncPole(
            string sapEquipId,
            string poleHeight,
            string poleClass,
            string poleSpecies,
            string poleOriginalCircumference,
            string poleEffectiveCircumference,
            string poleRemainingStrength,
            Nullable<DateTime> poleInspectionDate)
        {
            try
            {

                if (_pConn == null)
                {
                    //Get output DB connection 
                    _pConn = new SqlConnection(
                        Shared.OutputConnectString);
                }
                if (_pConn.State != System.Data.ConnectionState.Open)
                    _pConn.Open();

                //String to hold the SQL insert statement 
                string sqlStatement = string.Empty;
                string inspectDateString = "NULL";
                if (poleInspectionDate != null)
                    inspectDateString = "'" + poleInspectionDate.ToString() + "'";

                SqlCommand pCmd = null;

                //Write the insert statement 
                sqlStatement =
                    "UPDATE [PGE_Combined].[dbo].[POLES] SET " +
                        "HEIGHT_SAP = '" + poleHeight + "', " +
                        "CLASS_SAP = '" + poleClass + "', " +
                        "SPECIES_SAP = '" + poleSpecies + "', " +
                        "ORIGINALCIRCUMFERENCE = " + poleOriginalCircumference + ", " +
                        "EFFECTIVECIRCUMFERENCE = " + poleEffectiveCircumference + ", " +
                        "LASTINSPECTIONDATETIME = " + inspectDateString + ", " +
                        "REMAININGSTRENGTH = " + poleRemainingStrength + " " +
                    "WHERE" + " " +
                        "SAPEQUIPID = '" + sapEquipId + "'" + ";";


                //Execute the command against the database 
                if (sqlStatement != string.Empty)
                {
                    //Shared.WriteToLogfile(sqlStatement);

                    //Shared.WriteToLogfile(sqlStatement);
                    pCmd = _pConn.CreateCommand();
                    pCmd.Connection = _pConn;
                    pCmd.CommandText = sqlStatement;
                    int recordsUpdated = pCmd.ExecuteNonQuery();
                    if (recordsUpdated == 0)
                        Shared.WriteToLogfile("SQL update failed for pole: " + sapEquipId);
                }
            }
            catch (Exception ex)
            {
                Shared.WriteToLogfile("Error in SyncPole for : " + sapEquipId);
            }
        }

        private void SyncPole(
            string sapEquipId,
            string poleRemainingStrength)
        {
            try
            {

                if (_pConn == null)
                {
                    //Get output DB connection 
                    _pConn = new SqlConnection(
                        Shared.OutputConnectString);
                }
                if (_pConn.State != System.Data.ConnectionState.Open)
                    _pConn.Open();

                //String to hold the SQL insert statement 
                string sqlStatement = string.Empty;

                SqlCommand pCmd = null;

                //Write the insert statement 
                sqlStatement =
                    "UPDATE [PGE_Combined].[dbo].[POLES] SET " +
                        "REMAININGSTRENGTH = " + poleRemainingStrength + " " +
                    "WHERE" + " " +
                        "SAPEQUIPID = '" + sapEquipId + "'" + ";";

                //Execute the command against the database 
                if (sqlStatement != string.Empty)
                {
                    //Shared.WriteToLogfile(sqlStatement);
                    pCmd = _pConn.CreateCommand();
                    pCmd.Connection = _pConn;
                    pCmd.CommandText = sqlStatement;
                    int recordsUpdated = pCmd.ExecuteNonQuery();
                    if (recordsUpdated == 0)
                        Shared.WriteToLogfile("SQL update failed for pole: " + sapEquipId);
                }
            }
            catch (Exception ex)
            {
                Shared.WriteToLogfile("Error in SyncPole for : " + sapEquipId);
            }
        }


        private void SyncPole2(
            string sapEquipId, 
            int poleRemainingStrength)
        {
            try
            {

                if (_pConn == null)
                {
                    //Get output DB connection 
                    _pConn = new SqlConnection(
                        Shared.OutputConnectString);
                }
                if (_pConn.State != System.Data.ConnectionState.Open)
                    _pConn.Open();

                //if (poleRemainingStrength > 100)
                //{
                //    Shared.WriteToLogfile("Remaining strength for pole: " + sapEquipId +
                //            " exceeds 100% [" + poleRemainingStrength.ToString() + "]" + 
                //            " so it is being reset to 100");
                //    poleRemainingStrength = 100;
                //}

                //String to hold the SQL insert statement 
                string sqlStatement = string.Empty;
                SqlCommand pCmd = null;

                //Write the insert statement 
                sqlStatement =
                    "UPDATE [PGE_Combined].[dbo].[POLES] SET " +
                        "REMAININGSTRENGTH = " + poleRemainingStrength + " " +
                    "WHERE" + " " +
                        "SAPEQUIPID = '" + sapEquipId + "'" + ";";

                //Execute the command against the database 
                if (sqlStatement != string.Empty)
                {

                    //Shared.WriteToLogfile(sqlStatement);
                    pCmd = _pConn.CreateCommand();
                    pCmd.Connection = _pConn;
                    pCmd.CommandText = sqlStatement;
                    int recordsUpdated = pCmd.ExecuteNonQuery();
                    if (recordsUpdated == 1)
                        Shared.WriteToLogfile("SQL update made for pole: " + sapEquipId + 
                            " remaining strength set to: " + poleRemainingStrength.ToString());
                }
            }
            catch (Exception ex)
            {
                Shared.WriteToLogfile("Error in SyncPole for : " + sapEquipId);
            }
        }



        private bool IsJointOwned(
            string sapEquipId)
        {
            try
            {
                bool jointOwned = false; 
                IQueryFilter pQF = new QueryFilterClass();
                pQF.WhereClause = "SAPEQUIPID = " + "'" + sapEquipId + "'";
                IFeatureCursor pFCursor = m_SupportStructureFC.Search(pQF, false);
                IFeature pRefPoleFeature = pFCursor.NextFeature();
                Marshal.FinalReleaseComObject(pFCursor); 
                
                Hashtable hshPoleFields = (Hashtable)_hshAllAttributes["SUPPORTSTRUCTURE"]; 
                if (pRefPoleFeature == null)
                {
                    //Exit out as the referebce pole was not found 
                    Shared.WriteToLogfile("Error: Unable to find Joint_Owned pole with EuipID: " + sapEquipId.ToString());
                    return jointOwned;
                }
                int poleOId = pRefPoleFeature.OID; 

                //if (pRefPoleFeature.Shape == null)
                //{
                //    //Exit out as the reference pole must have a valid shape  
                //    Shared.WriteToLogfile("Error: Unable to process ref pole with OId: " + poleOId.ToString() +
                //        " because the shape is null");
                //    return jointOwned;
                //}
                //Shared.WriteToLogfile("ObjectId: " + pRefPoleFeature.OID.ToString());

                //if (_pConn == null)
                //{
                //    //Get output DB connection 
                //    _pConn = new SqlConnection(
                //        Shared.OutputConnectString);
                //}
                //if (_pConn.State != System.Data.ConnectionState.Open)
                //    _pConn.Open();

                //String to hold the SQL insert statement 
                string sqlStatement = string.Empty;
                string zoneValue = string.Empty;
                int jointCount = 0;
                string jointCountString = string.Empty;
                int fldIdx = -1;
                SqlCommand pCmd = null;

                if (pRefPoleFeature != null)
                {
                    //fldIdx = Convert.ToInt32(hshPoleFields["SAPEQUIPID"]);
                    //if (pRefPoleFeature.get_Value(fldIdx) != DBNull.Value)
                    //{
                    //    sapEquipId = GetFieldValue(pRefPoleFeature, fldIdx);
                    //}

                    //JOINT_OWNED  
                    //Changed on 07/19 from direction from Alasdair / Robert to accommodate 
                    //the Contact Pole scenario 
                    jointCount = 1;
                    jointOwned = false;
                    fldIdx = Convert.ToInt32(hshPoleFields["JOINTCOUNT"]);
                    if (pRefPoleFeature.get_Value(fldIdx) != DBNull.Value)
                    {
                        jointCountString = GetFieldValue(pRefPoleFeature, fldIdx);
                        jointCountString = jointCountString.Substring(0, 1);
                        Int32.TryParse(jointCountString, out jointCount);
                    }
                    ISet pJointUseAttachSet = m_JointUseAttachmentRC.GetObjectsRelatedToObject((IObject)pRefPoleFeature);
                    pJointUseAttachSet.Reset();
                    //If jointcount greater than 1 or there is at least 1 joint use attachment then 
                    //JOINT_OWNED = TRUE (otherwise FALSE)
                    if ((pJointUseAttachSet.Count > 0) || (jointCount > 1))
                        jointOwned = true;
                    Marshal.FinalReleaseComObject(pJointUseAttachSet);
                    //valuesList.Append("'" + jointOwned.ToString().ToUpper() + "'");

                    //Write the insert statement 
                    //sqlStatement = "UPDATE [PGE_Combined].[dbo].[POLES] SET JOINT_OWNED = '" + jointOwned.ToString().ToUpper() + "' WHERE SAPEQUIPID = '" + sapEquipId + "'" + ";";
                }

                //Execute the command against the database 
                //if (sqlStatement != string.Empty)
                //{
                //    Shared.WriteToLogfile(sqlStatement);

                //    //Shared.WriteToLogfile(sqlStatement);
                //    pCmd = _pConn.CreateCommand();
                //    pCmd.Connection = _pConn;
                //    pCmd.CommandText = sqlStatement;
                //    int recordsUpdated = pCmd.ExecuteNonQuery();
                //    if (recordsUpdated == 0)
                //        Shared.WriteToLogfile("SQL insert failed for pole: " + sapEquipId);
                //}

                return jointOwned; 

            }
            catch (Exception ex)
            {
                Shared.WriteToLogfile("Error in " + MethodBase.GetCurrentMethod() +
                    " details: " + ex.Message);
                throw new Exception("Error processing pole: " + ex.Message);
            }
        }



        private int GetMaxSpanLength(string popDensity)
        {            
            try
            {
                int maxSpanLength = 0; 
                int defaultMaxSpanLength = Shared.MaxSpanLengthMediumDensity; 
                switch (popDensity)
                {
                    case "HIGH (>1000 per sqmi)":
                        maxSpanLength = Shared.MaxSpanLengthHighDensity; 
                        break;
                    case "MEDIUM (60-1000 per sqmi)":
                        maxSpanLength = Shared.MaxSpanLengthMediumDensity;
                        break;
                    case "LOW (<60 per sqmi)":
                        maxSpanLength = Shared.MaxSpanLengthLowDensity;
                        break;
                    default:
                        maxSpanLength = defaultMaxSpanLength;
                        break;
                }
                return maxSpanLength; 
            }
            catch (Exception ex)
            {
                Shared.WriteToLogfile("Error in " + MethodBase.GetCurrentMethod() +
                    " details: " + ex.Message);
                throw new Exception("Error returning max span length");
            }
        }

        /// <summary>
        /// Trace along and find all of the spans by 
        /// looking for the adjacent poles
        /// </summary>
        /// <param name="pAllConductors"></param>
        /// <param name="pAllPoles"></param>
        /// <param name="pRefPole"></param>
        /// <param name="pAllRelatedDevices"></param>
        /// <param name="pNearbyConductors"></param>
        /// <param name="pMaxSpanRefPoleBuffer"></param>
        /// <param name="pSpans"></param>
        private void DelineateSpanList(
            List<Conductor> pAllConductors,
            List<Pole> pAllPoles,
            IIdentify pIndentify,
            Pole pRefPole, 
            double refPoleRL, 
            List<RelatedDevice> pAllRelatedDevices,
            List<Conductor> pNearbyConductors,
            ref List<Span> pSpans)
        {
            try
            {
                //Loop through each of the nearby conductors 
                //keep getting the adjacent conductors until 
                //we reach an adjacent pole or an end point 
                //IMap pmap = ((IMxDocument)m_app.Document).FocusMap;
                //IActiveView pAV = (IActiveView)pmap;
                //ESRI.ArcGIS.Carto.IGraphicsContainer pGC = (ESRI.ArcGIS.Carto.IGraphicsContainer)pmap;
                //pGC.DeleteAllElements();
                int conductorCount = pNearbyConductors.Count;
                int currentConductor = 0;
                int callCounter = 0; 
                
                foreach (Conductor pConductor in pNearbyConductors)
                {
                    currentConductor++;
                    int spanCountb4 = pSpans.Count;
                    callCounter = 0; 
                    Shared.WriteToLogfile("Processing nearby conductor: " + pConductor.FCName + pConductor.OID.ToString());
                    //if (pConductor.GUID == "{4268B02F-DCAA-431B-B265-9F57FA89A3F7}")
                    //    Debug.Print("this one"); 

                    TrackSpans(
                        pAllConductors,
                        pAllPoles,
                        pIndentify,
                        pRefPole,
                        refPoleRL,
                        pConductor.Shape.FromPoint,
                        pConductor,
                        null,
                        ref pSpans, 
                        ref callCounter);
                    
                    if ((pSpans.Count - spanCountb4) == 0)
                        Shared.WriteToLogfile("Did not find any spans nearby: " + pConductor.FCName + pConductor.OID.ToString());
                    else if ((pSpans.Count - spanCountb4) > 1)
                        Shared.WriteToLogfile("Found > 1 spans nearby: " + pConductor.FCName + pConductor.OID.ToString());
                    else if ((pSpans.Count - spanCountb4) == 1)
                        Shared.WriteToLogfile("Found 1 span nearby: " + pConductor.FCName + pConductor.OID.ToString());
                    
                    //pGC.DeleteAllElements();
                    //GraphicUtils.DrawPolyline(pmap, pConductor.Shape, GraphicUtils.GetRGBColour(255, 0, 0), "");
                    //pAV.Refresh();
                    //MessageBox.Show("here is the nearby conductor");

                    //foreach (Span pSpan in pSpans)
                    //{
                    //    //Construct the line for the buffer 
                    //    GraphicUtils.DrawPolyline(pmap, pSpan.SpanPolyline, GraphicUtils.GetRGBColour(0, 255, 0), "");
                    //}

                    //pAV.Refresh();
                    //MessageBox.Show("here are the spans count: " + pSpans.Count.ToString());
                }

                //At this point we have to remove the duplicate spans the 
                //rule being that you cannot have 2 spans between the same 
                //2 poles that share an identical segment  
                RemoveSpansWithDuplicateSegments(ref pSpans);

                //Remove psuedo service spans connected to a transformer 
                //which is related to a pole which is NOT the ref pole
                RemovePseudoServSpansRelatedToOtherPole(
                    pAllRelatedDevices,
                    pAllConductors,
                    ref pSpans);

                //Remove spans where there is a related device at the ToPoint 
                //this scenario is represented with Pole 101440510 
                RemoveSpanWhereRelatedEquipmentAtToPoint( 
                    pAllRelatedDevices, 
                    pAllConductors,
                    pAllPoles,
                    ref pSpans);

            }
            catch (Exception ex)
            {
                Shared.WriteToLogfile("Error in " + MethodBase.GetCurrentMethod() +
                    " details: " + ex.Message);
                throw new Exception("Error delineating the spans");
            }
        }

        /// <summary>
        /// If there is a Span which has a piece of related equipment 
        /// e.g. A Riser at the ToPoint, then the Span must be removed. 
        /// Also if there is a short Span where the device at the topoint 
        /// that is not related to reference pole but there is no other 
        /// poles in the vicinity (then it should be related to the ref 
        /// pole but there is a data error) then likewise the Span will 
        /// be removed 
        /// </summary>
        /// <param name="pSpans"></param>
        private void RemoveSpanWhereRelatedEquipmentAtToPoint(
            List<RelatedDevice> pAllRelatedDevices,
            List<Conductor> pAllConductors,
            List<Pole> pAllPoles,
            ref List<Span> pSpans)
        {
            try
            {
                Hashtable hshSpansToRemove = new Hashtable();

                //1. Related device at the to point remove the span 
                foreach (Span pSpan in pSpans)
                {
                    //Debug.Print(pSpan.ConductorGUID);
                    //if (pSpan.ConductorGUID == "{8977D24A-BD3B-4193-AFE0-227367699B90}")
                    //    Debug.Print("this is it");

                    //Look for a related device at the ToPoint 
                    foreach (RelatedDevice pRelDevice in pAllRelatedDevices)
                    {
                        //Look for a related device at ToPoint 
                        if (IsConincident(pRelDevice.Shape, pSpan.SpanPolyline.ToPoint))
                        {
                            if (!hshSpansToRemove.ContainsKey(pSpan.SpanGUID))
                                hshSpansToRemove.Add(pSpan.SpanGUID, 0);
                        }
                    }                    
                }               

                
                //2. Remove the span where the following conditions are met 
                //   a) No Pole at the ToPoint of the Span 
                //   b) Span length < 50 feet 
                //   c) Device at the ToPoint either 
                //          -> has no relationship to another pole 
                //          -> ref pole is the closest pole 
                foreach (Span pSpan in pSpans)
                {
                    //Span length less than 50 feet and no ToPole 
                    if ((pSpan.Length < 50) && 
                        (pSpan.ToPoleGUID == string.Empty))
                    {

                        //Check if the reference pole is the closest 
                        //pole to the ToPoint 
                        double distAway = 0;
                        Pole pClosestPole = GetClosestPoleFromPoint(
                            pSpan.SpanPolyline.ToPoint,
                            pAllPoles,
                            ref distAway);                        
                        if (pClosestPole.GUID == pSpan.FromPoleGUID)
                        { 
                            //Ref pole is closest 

                            //Look for the device at the ToPoint
                            string poleGUID = string.Empty;                          
                            INetworkClass pNDS = (INetworkClass)m_TransformerFC;
                            IGeometricNetwork pGN = pNDS.GeometricNetwork;
                            int eid = pGN.get_JunctionElement(pSpan.SpanPolyline.ToPoint);

                            if (eid != -1)
                            {
                                IEIDHelper eidHelper = new EIDHelperClass();
                                INetElements netElements = pGN.Network as INetElements;
                                int userClassID = 0;
                                int userID = 0;
                                int userSubID = 0;
                                IFeatureClassContainer fcContainer = (IFeatureClassContainer)pGN;
                                netElements.QueryIDs(eid, esriElementType.esriETJunction,
                                    out userClassID, out userID, out userSubID);
                                if (userID > 0)
                                {
                                    IFeatureClass featClass = fcContainer.get_ClassByID(userClassID);
                                    IFeature pFeature = featClass.GetFeature(userID);
                                    IDataset pDS = (IDataset)featClass;

                                    //Look for the structureGUID - must have a structureguid field  
                                    int fldidx = featClass.Fields.FindField("structureguid");
                                    if (fldidx != -1)
                                    {
                                        if (pFeature.get_Value(fldidx) != DBNull.Value)
                                            poleGUID = pFeature.get_Value(fldidx).ToString();

                                        //Check if the pole is linked to another pole in the vicinity
                                        bool isLinkedToAnotherPole = false;
                                        foreach (Pole pPole in pAllPoles)
                                        {
                                            if ((pPole.GUID == poleGUID) &&
                                                (poleGUID != pSpan.FromPoleGUID))
                                            {
                                                isLinkedToAnotherPole = true;
                                                break;
                                            }
                                        }

                                        if (isLinkedToAnotherPole == false)
                                        {
                                            //Remove this span because the device at the end of the 
                                            //span should be related to the reference pole 
                                            if (!hshSpansToRemove.ContainsKey(pSpan.SpanGUID))
                                                hshSpansToRemove.Add(pSpan.SpanGUID, 0);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                
                //Remove the spans 
                foreach (string spanGUID in hshSpansToRemove.Keys)
                {
                    //Remove the item                    
                    pSpans.RemoveAll(item => item.SpanGUID == spanGUID);
                }

                //Debug.Print("Spans to be removed: " + hshSpansToRemove.Count.ToString());
                //Debug.Print("span count before: " + pSpans.Count.ToString());
                //Debug.Print("span count after: " + pSpans.Count.ToString());
            }
            catch (Exception ex)
            {
                Shared.WriteToLogfile("Error in " + MethodBase.GetCurrentMethod() +
                    " details: " + ex.Message);
                throw new Exception("Error in routine RemoveSpansWithDuplicateSegments");
            }
        }

        /// <summary>
        /// we have to remove the duplicate spans the rule being 
        /// that you cannot have 2 spans between the same 
        /// 2 points that share an identical trunk segment (so keep 
        /// the one with the longest trunk) 
        /// </summary>
        /// <param name="pSpans"></param>
        private void RemoveSpansWithDuplicateSegments(ref List<Span> pSpans)
        {
            try
            {
                string spanToRemove = "";
                Hashtable hshSpansToRemove = new Hashtable();

                foreach (Span pSpan in pSpans)
                {
                    foreach (Span pSp in pSpans)
                    {
                        //make sure it is not the actual same span 
                        if (pSpan.SpanGUID != pSp.SpanGUID)
                        {
                            //If they have the same ToPoint 
                            if (IsConincident(
                                pSpan.SpanPolyline.ToPoint,
                                pSp.SpanPolyline.ToPoint))
                            {
                                //They must not share any segment 
                                //in the trunk so check this 
                                if (PolylinesHaveSameSegment(pSpan.SpanTrunk, pSp.SpanTrunk))
                                {
                                    //must remove the one that is the shortest 
                                    if (pSpan.SpanTrunk.Length < pSp.SpanTrunk.Length)
                                        spanToRemove = pSpan.SpanGUID;                                         
                                    else
                                        spanToRemove = pSp.SpanGUID;

                                    if (!hshSpansToRemove.ContainsKey(spanToRemove))
                                        hshSpansToRemove.Add(spanToRemove, "");
                                }
                            }
                        }
                    }
                }

                foreach (string spanGUID in hshSpansToRemove.Keys)
                {
                    //Remove the item                    
                    pSpans.RemoveAll(item => item.SpanGUID == spanGUID);
                }
            }
            catch (Exception ex)
            {
                Shared.WriteToLogfile("Error in " + MethodBase.GetCurrentMethod() +
                    " details: " + ex.Message);
                throw new Exception("Error in routine RemoveSpansWithDuplicateSegments");
            }
        }

        /// <summary>
        /// If a span is a PseudoService connected to a transformer 
        /// which is not related to the current reference pole then 
        /// we must remove this Span from the list  
        /// </summary>
        /// <param name="pSpans"></param>
        private void RemovePseudoServSpansRelatedToOtherPole(
            List<RelatedDevice> pAllRelatedDevices, 
            List<Conductor> pAllConductors, 
            ref List<Span> pSpans)
        {
            try
            {
                Hashtable hshSpansToRemove = new Hashtable();
                bool foundRefPoleTransformer = false; 

                foreach (Span pSpan in pSpans)
                {
                    if (pSpan.ConductorSubtype == "Pseudo Service")
                    {
                        //Find that Conductor
                        Conductor pTargetConductor = null;
                        foreach (Conductor pConductor in pAllConductors)
                        {
                            if (pSpan.ConductorGUID == pConductor.GUID)
                            {
                                pTargetConductor = pConductor;
                                break;
                            }
                        }

                        //Check that it has a related transformer at one end 
                        if (pTargetConductor != null)
                        {
                            IPolyline pPseudoSPolyline = pTargetConductor.Shape;

                            //Look for a related transformer at the To or From point
                            foundRefPoleTransformer = false; 
                            foreach (RelatedDevice pRelDevice in pAllRelatedDevices)
                            {
                                if (pRelDevice.DeviceType == "TRANSFORMER")
                                {
                                    //Look for a related transformer at each end 
                                    if ((IsConincident(pRelDevice.Shape, pPseudoSPolyline.FromPoint)) ||
                                        (IsConincident(pRelDevice.Shape, pPseudoSPolyline.ToPoint)))
                                    {
                                        foundRefPoleTransformer = true;
                                        break; 
                                    }
                                }
                            }
                            
                            //If no transformer related to ref pole then must remove span 
                            if (!foundRefPoleTransformer)
                            {
                                if (!hshSpansToRemove.ContainsKey(pSpan.SpanGUID))
                                    hshSpansToRemove.Add(pSpan.SpanGUID, 0); 
                            } 
                        }
                    }                        
                }

                //Debug.Print("Spans to be removed: " + hshSpansToRemove.Count.ToString());
                //Debug.Print("span count before: " + pSpans.Count.ToString());
                foreach (string spanGUID in hshSpansToRemove.Keys)
                {
                    //Remove the item                    
                    pSpans.RemoveAll(item => item.SpanGUID == spanGUID);                    
                }
                //Debug.Print("span count after: " + pSpans.Count.ToString());
            }
            catch (Exception ex)
            {
                Shared.WriteToLogfile("Error in " + MethodBase.GetCurrentMethod() +
                    " details: " + ex.Message);
                throw new Exception("Error in routine RemoveSpansWithDuplicateSegments");
            }
        }

        /// <summary>
        /// Sometimes the GPSLatitude and GPSLongitude is not populated in the 
        /// GIS. So in these cases derive the lat / long from the geometry. 
        /// </summary>
        /// <param name="pPolePoint"></param>
        /// <param name="latValue"></param>
        /// <param name="longValue"></param>
        public void PopulateLatLongForPole(IPoint pPolePoint, ref double latValue, ref double longValue)
        {
            try
            {
                //First open the source featureclass and do a spatial search 
                //for all plats within the clip
                if (m_pSR_WGS84 == null)
                {
                    int geoType = (int)ESRI.ArcGIS.Geometry.esriSRGeoCSType.esriSRGeoCS_WGS1984;
                    ISpatialReferenceFactory pSRF = new SpatialReferenceEnvironmentClass();
                    m_pSR_WGS84 = pSRF.CreateGeographicCoordinateSystem(geoType);
                }

                //Clone the ref pole point 
                IPoint pPolePointClone = (IPoint)CloneShape(pPolePoint, pPolePoint.SpatialReference);
                pPolePointClone.Project(m_pSR_WGS84);

                longValue = pPolePointClone.X;
                latValue = pPolePointClone.Y;                

            }
            catch (Exception ex)
            {
                Shared.WriteToLogfile("Error in " + MethodBase.GetCurrentMethod() +
                    " details: " + ex.Message);
                throw new Exception("Error in routine PopulateLatLongForPole");
            }
        }

        private bool PolylinesHaveSameSegment(IPolyline pPolyline1, IPolyline pPolyline2)
        {
            try
            {
                bool sameSeg = false;
                ISegmentCollection pSegColl1 = (ISegmentCollection)pPolyline1;
                ISegmentCollection pSegColl2 = (ISegmentCollection)pPolyline2;

                IRelationalOperator pRO = (IRelationalOperator)pPolyline1;
                if (!pRO.Disjoint(pPolyline2))
                {
                    for (int i = 0; i < pSegColl1.SegmentCount; i++)
                    {
                        for (int j = 0; j < pSegColl2.SegmentCount; j++)
                        {
                            if ((IsConincident(pSegColl1.Segment[i].FromPoint, pSegColl2.Segment[j].FromPoint)) &&
                               (IsConincident(pSegColl1.Segment[i].ToPoint, pSegColl2.Segment[j].ToPoint)))
                            {
                                sameSeg = true;
                                break;
                            }
                        }
                        if (sameSeg)
                            break;
                    }
                }

                return sameSeg;
            }
            catch (Exception ex)
            {
                Shared.WriteToLogfile("Error in " + MethodBase.GetCurrentMethod() +
                    " details: " + ex.Message);
                throw new Exception("Error in routine PolylinesHaveSameSegment");
            }
        }

        private IPoint GetPolePointByGUID(List<Pole> pAllPoles, string guid)
        {
            try
            {
                foreach (Pole pPole in pAllPoles)
                {
                    if (pPole.GUID == guid)
                    {
                        return (IPoint)CloneShape((IGeometry)pPole.Shape, pPole.Shape.SpatialReference);
                    }
                }

                return null;
            }
            catch (Exception ex)
            {
                Shared.WriteToLogfile("Error in " + MethodBase.GetCurrentMethod() +
                    " details: " + ex.Message);
                throw new Exception("Error returning pole point");
            }
        }
        /// <summary>
        /// Looks for a span using the passed conductor
        /// </summary>
        /// <param name="pAllConductors"></param>
        /// <param name="pAllPoles"></param>
        /// <param name="pRefPole"></param>
        /// <param name="pStartPoint"></param>
        /// <param name="pEndPoint"></param>
        /// <param name="pCurrentConductor"></param>
        /// <param name="pSpanTrunk"></param>
        private void TrackSpans(
            List<Conductor> pAllConductors,
            List<Pole> pAllPoles,
            IIdentify pIdentify,
            Pole pRefPole,
            double refPoleRL, 
            IPoint pStartPoint,
            Conductor pCurrentConductor,
            IPolyline pSpanTrunk,
            ref List<Span> pSpans, 
            ref int callCounter)
        {
            try
            {
                //Avoid getting into a continuous loop situation with 
                //recursive function 
                callCounter++;
                if (callCounter > 100)
                    return;
                if (pSpanTrunk != null)
                {
                    if (pSpanTrunk.Length > 1500)
                        return;
                }

                //Join the current conductor polyline to the pSpanTrunk 
                IPolyline pSpanTrunkExtended = JoinPolylineToTrunk(pSpanTrunk, pCurrentConductor.Shape);

                //IMap pmap = ((IMxDocument)m_app.Document).FocusMap;
                //IActiveView pAV = (IActiveView)pmap;
                //ESRI.ArcGIS.Carto.IGraphicsContainer pGC = (ESRI.ArcGIS.Carto.IGraphicsContainer)pmap;
                //pGC.DeleteAllElements();
                //GraphicUtils.DrawPolyline(pmap, pSpanTrunkExtended, GraphicUtils.GetRGBColour(255, 0, 0), "");
                //pAV.Refresh();
                //MessageBox.Show("Here is the extended trunk");

                //If the ToPoint AND FromPoint of the pCurrentConductor is already 
                //in the pSpanTrunk this is telling us that we are looping back onto
                //the same spot, in this case we need to exit (otherwise endless loop 
                //condition) 
                bool isLoopBack = false;
                if (pSpanTrunk != null)
                {
                    bool foundFrom = false;
                    bool foundTo = false;

                    IPointCollection pPointColl = (IPointCollection)pSpanTrunk;
                    for (int i = 0; i < pPointColl.PointCount; i++)
                    {
                        if (IsConincident(pCurrentConductor.Shape.ToPoint, pPointColl.Point[i]))
                            foundTo = true;
                        if (IsConincident(pCurrentConductor.Shape.FromPoint, pPointColl.Point[i]))
                            foundFrom = true;
                        if (foundFrom && foundTo)
                        {
                            isLoopBack = true;
                            break;
                        }
                    }
                    //Where we have a loop back simply exit out 
                    if (isLoopBack)
                        return;
                }

                //Update the condutorinfo in pAllConductors if necessary 
                foreach (Conductor pCon in pAllConductors)
                {
                    if (pCon.GUID == pCurrentConductor.GUID)
                    {
                        if (pCon.ConductorInfos == null)
                        {
                            pCon.ConductorInfos = GetConductorInfoForConductor(
                            pCurrentConductor.FCName,
                            pCurrentConductor.OID);
                            break; 
                        }
                    }
                }

                //Check for a pole within tolerance of the pCurrentConductor
                double distFromPoleToConductor = 0;
                Pole pAdjacentPole = GetAdjacentPole( 
                    pAllPoles, 
                    pAllConductors, 
                    pCurrentConductor, 
                    pRefPole, 
                    ref distFromPoleToConductor);

                if (pAdjacentPole != null)
                {
                    //We have found an adjacent pole 

                    //Check the value of the distFromPoleToConductor and if this 
                    //value is > 12 foot then look to see if there is another 
                    //connected conductor that will get us closer to the adjacent 
                    //pole 
                    if (distFromPoleToConductor > 10)
                    {
                        Debug.Print("more than 10 foot to the adjacent pole: " +
                            distFromPoleToConductor.ToString());

                        //Check to see if there is a connected conductor which 
                        //will bring the conductor in tighter to this adjacent pole 
                        //if so then add it into the trunk before creating the span 
                        IPoint pNewStartPoint = GetOtherEndOfConductor(pStartPoint, pCurrentConductor.Shape);
                        Conductor pCloserConductor = LookForConductorCloserToAdjacentPole( 
                            pAllConductors,
                            pCurrentConductor,
                            pAdjacentPole,                              
                            distFromPoleToConductor,
                            pNewStartPoint);
                        if (pCloserConductor != null)
                            pSpanTrunkExtended = JoinPolylineToTrunk(pSpanTrunkExtended, pCloserConductor.Shape);
                    }

                    //We have found the adjacent pole so create the span created 
                    //between the pRefPole and the pAdjacentPole 
                    IPolyline pSpanPolyline = GetSpanPolyline(pRefPole.Shape, pAdjacentPole.Shape);
                    Span pSpan = new Span(
                        Guid.NewGuid().ToString(),
                        pSpanPolyline,
                        (IPolyline)CloneShape(pSpanTrunkExtended, pRefPole.Shape.SpatialReference),
                        GetSpanAngleFromAdjacentPoles(pRefPole.Shape, pAdjacentPole.Shape),
                        pSpanPolyline.Length,
                        pCurrentConductor.FCName.ToUpper(),
                        pCurrentConductor.Subtype,
                        pCurrentConductor.GUID,
                        pRefPole.GUID,
                        refPoleRL,
                        pAdjacentPole.GUID,
                        GetHeightAtPoint(pAdjacentPole.Shape, pIdentify),
                        pCurrentConductor.CircId);
                    pSpans.Add(pSpan);

                }
                else
                {
                    //We did not find an adjacent pole 

                    //continue tracking and looking for an adjacent pole 
                    IPoint pNewStartPoint = GetOtherEndOfConductor(pStartPoint, pCurrentConductor.Shape);
                    List<Conductor> pConnectedConductors = GetConnectedConductors(
                        pNewStartPoint, pCurrentConductor, pAllConductors);

                    //It may reach a dead end here for example if it reaches 
                    //a house so lets call this a span (but it will be a slack span) 
                    //and the ToPole will be null 
                    if (pConnectedConductors.Count == 0)
                    {
                        //Create a span here but the ToPole will be null 
                        IPolyline pSpanPolyline = GetSpanPolyline(pRefPole.Shape, pNewStartPoint);
                        Span pSpan = new Span(
                            Guid.NewGuid().ToString(),
                            pSpanPolyline,
                            (IPolyline)CloneShape(pSpanTrunkExtended, pRefPole.Shape.SpatialReference),
                            GetSpanAngleFromAdjacentPoles(pRefPole.Shape, pNewStartPoint),
                            pSpanPolyline.Length,
                            pCurrentConductor.FCName.ToUpper(),
                            pCurrentConductor.Subtype,
                            pCurrentConductor.GUID,
                            pRefPole.GUID,
                            refPoleRL,
                            "", //Since there is no adjacent pole (dead end) 
                            GetHeightAtPoint(pNewStartPoint, pIdentify),
                            pCurrentConductor.CircId);
                        pSpans.Add(pSpan);
                    }

                    // Loop through the connected conductors 
                    foreach (Conductor pConductor in pConnectedConductors)
                    {
                        //Recursive call to track spans 
                        TrackSpans(
                            pAllConductors,
                            pAllPoles,
                            pIdentify,
                            pRefPole, 
                            refPoleRL,
                            pSpanTrunkExtended.ToPoint,
                            pConductor,
                            pSpanTrunkExtended,
                            ref pSpans, 
                            ref callCounter);
                    }
                }
            }
            catch (Exception ex)
            {
                Shared.WriteToLogfile("Error in " + MethodBase.GetCurrentMethod() +
                    " details: " + ex.Message);
                throw new Exception("Error tracking recursive spans");
            }
        }

        private Conductor LookForConductorCloserToAdjacentPole( 
            List<Conductor> pAllConductors, 
            Conductor pCurrentConductor, 
            Pole pAdjacentPole, 
            double currentDistFromPoleToConductor, 
            IPoint pNewStartPoint 
            )
        {
            try
            {
                List<Conductor> pConnectedConductors = GetConnectedConductors(
                    pNewStartPoint, pCurrentConductor, pAllConductors);
                double distFromPoleToConductor; 
                double closestDist = 10000;
                Conductor pClosestConductor = null;               

                foreach (Conductor pConnectedConductor in pConnectedConductors)
                {
                    //Check if there is one that is much closer to the adjacent 
                    //pole 
                    IProximityOperator pProxOp = (IProximityOperator)pConnectedConductor.Shape;
                    distFromPoleToConductor = pProxOp.ReturnDistance(pAdjacentPole.Shape);
                    if (distFromPoleToConductor < closestDist)
                    {
                        pClosestConductor = pConnectedConductor; 
                        closestDist = distFromPoleToConductor;
                    }
                }

                Conductor pCloserConductor = null; 
                if (closestDist < currentDistFromPoleToConductor)
                    pCloserConductor = pClosestConductor;

                return pCloserConductor; 
            }
            catch (Exception ex)
            {
                Shared.WriteToLogfile("Error in " + MethodBase.GetCurrentMethod() +
                    " details: " + ex.Message);
                throw new Exception("Error tracking recursive spans");
            }
        }

        private IPolyline JoinPolylineToTrunk(IPolyline pTrunkPolyline, IPolyline pNewConductor)
        {
            try
            {
                //Debug.Print("b4: " + ((ISegmentCollection)pTrunkPolyline).SegmentCount.ToString()); 
                IPolyline pJoinedPolyline;
                if (pTrunkPolyline == null)
                {
                    //Just clone the pNewConductor
                    IClone pClone = (IClone)pNewConductor;
                    pJoinedPolyline = (IPolyline)pClone.Clone();
                    pJoinedPolyline.SpatialReference = pNewConductor.SpatialReference;
                    return pJoinedPolyline;
                }
                else
                {
                    //Clone the pNewConductor and the pTrunkPolyline 
                    IClone pClone = (IClone)pNewConductor;
                    IPolyline pNewConductorCopy = (IPolyline)pClone.Clone();
                    pNewConductorCopy.SpatialReference = pNewConductor.SpatialReference;
                    pClone = (IClone)pTrunkPolyline;
                    IPolyline pTrunkPolylineCopy = (IPolyline)pClone.Clone();
                    pTrunkPolylineCopy.SpatialReference = pTrunkPolyline.SpatialReference;

                    //Reverse the orientation if it does not connect up 
                    if (!IsConincident(pTrunkPolylineCopy.ToPoint, pNewConductorCopy.FromPoint))
                        pNewConductorCopy.ReverseOrientation();

                    ISegmentCollection pSegColl = (ISegmentCollection)pTrunkPolylineCopy;
                    pSegColl.AddSegmentCollection((ISegmentCollection)pNewConductorCopy);

                    //Simplify the shape 
                    ITopologicalOperator2 pTopo = (ITopologicalOperator2)pTrunkPolylineCopy;
                    pTopo.IsKnownSimple_2 = false;
                    pTopo.Simplify();
                    //Debug.Print("after: " + ((ISegmentCollection)pTrunkPolylineCopy).SegmentCount.ToString());
                    return pTrunkPolylineCopy;
                }
            }
            catch (Exception ex)
            {
                Shared.WriteToLogfile("Error in " + MethodBase.GetCurrentMethod() +
                    " details: " + ex.Message);
                throw new Exception("Error joining polylines");
            }
        }

        private IPoint GetOtherEndOfConductor(IPoint pOneEndPoint, IPolyline pConductorPolyline)
        {
            try
            {
                IClone pClone = null;
                if (IsConincident(pOneEndPoint, pConductorPolyline.FromPoint))
                    pClone = (IClone)pConductorPolyline.ToPoint;
                else
                    pClone = (IClone)pConductorPolyline.FromPoint;

                //Debug.Print(IsConincident(pOneEndPoint, pConductorPolyline.FromPoint).ToString()); 

                IPoint pOtherEndPoint = (IPoint)pClone.Clone();
                pOtherEndPoint.SpatialReference =
                        pConductorPolyline.SpatialReference;
                return pOtherEndPoint;
            }
            catch (Exception ex)
            {
                Shared.WriteToLogfile("Error in " + MethodBase.GetCurrentMethod() +
                    " details: " + ex.Message);
                throw new Exception("Error finding other end of conductor");
            }
        }

        /// <summary>
        /// Simply find the end of the conductor that is closest to the  
        /// reference pole
        /// </summary>
        /// <param name="pOneEndPoint"></param>
        /// <param name="pConductorPolyline"></param>
        /// <returns></returns>
        //private IPoint GetConductorStartPoint(Pole pRefPole, Conductor pConductor)
        //{
        //    try
        //    {
        //        IProximityOperator pProxOp = (IProximityOperator)pRefPole.Shape;
        //        double distToFromPoint = pProxOp.ReturnDistance(pConductor.Shape.FromPoint);
        //        double distToToPoint = pProxOp.ReturnDistance(pConductor.Shape.ToPoint);

        //        //Clone the shape to be safe 
        //        IPoint pClosestEnd = null;
        //        IClone pClone = null;
        //        if (distToFromPoint < distToToPoint)
        //            pClone = (IClone)pConductor.Shape.FromPoint;
        //        else
        //            pClone = (IClone)pConductor.Shape.ToPoint;

        //        pClosestEnd = (IPoint)pClone.Clone();
        //        pClosestEnd.SpatialReference = pRefPole.Shape.SpatialReference;
        //        return pClosestEnd;
        //    }
        //    catch (Exception ex)
        //    {
        //        Shared.WriteToLogfile("Error in " + MethodBase.GetCurrentMethod() +
        //            " details: " + ex.Message);
        //        throw new Exception("Error finding other end of conductor");
        //    }
        //}

        /// <summary>
        /// Look for connected conductors based on coincidence with the 
        /// connection point. Include logic to make sure that primary connects 
        /// to primary and secondary connects to secondary 
        /// </summary>
        /// <param name="pEndPoint"></param>
        /// <param name="pRefConductor"></param>
        /// <param name="pAllConductors"></param>
        /// <returns></returns>
        private List<Conductor> GetConnectedConductors(
            IPoint pEndPoint,
            Conductor pRefConductor,
            List<Conductor> pAllConductors)
        {
            try
            {
                List<Conductor> pConnectedConductors = new List<Conductor>();
                foreach (Conductor pConductor in pAllConductors)
                {
                    if (pRefConductor.FCName == pConductor.FCName)
                    {
                        if (!(pConductor.OID == pRefConductor.OID))
                        {
                            if ((IsConincident(pEndPoint, pConductor.Shape.FromPoint)) ||
                                (IsConincident(pEndPoint, pConductor.Shape.ToPoint)))
                            {

                                //Must be connected 
                                IClone pClone = (IClone)pConductor.Shape;
                                IPolyline pPolylineClone = (IPolyline)pClone.Clone();
                                pPolylineClone.SpatialReference =
                                    pConductor.Shape.SpatialReference;

                                //Copy the conductor 
                                Conductor pConnectedConductor = new Conductor(
                                    pConductor.OID,
                                    pConductor.Subtype,
                                    pConductor.FCName,
                                    pConductor.GUID,
                                    pConductor.CircId, 
                                    pConductor.OperatingVoltage,
                                    pConductor.ConstructionType,
                                    pConductor.Spacing,
                                    pPolylineClone,
                                    null);
                                pConnectedConductors.Add(pConnectedConductor);
                            }
                        }
                    }
                }
                return pConnectedConductors;
            }
            catch (Exception ex)
            {
                Shared.WriteToLogfile("Error in " + MethodBase.GetCurrentMethod() +
                    " details: " + ex.Message);
                throw new Exception("Error returning the connected conductors");
            }
        }


        private Pole GetAdjacentPole(
            List<Pole> pAllPoles,
            List<Conductor> pAllConductors,
            Conductor pCurrentConductor,
            Pole pRefPole,
            ref double distFromPoleToConductor)
        {
            try
            {
                Pole pAdjacentPole = null;
                Pole pPossibleAdjacentPole = null;
                IProximityOperator pPO = (IProximityOperator)pRefPole.Shape;

                //New buffer logic to be added here 
                //1. If SecondaryConductor then buffer by 
                //the SecondaryConductorBuffer 
                //2. If primary get the distance of the conductor 
                //to the refPole add the PrimaryBufferOffset and 
                //buffer by that amount (not the PoleBuffer) 
                double distToCurrentPole = 0; 
                double adjacentPoleSearchBuffer = GetAdjacentPoleSearchBuffer(
                    pCurrentConductor,
                    pRefPole);  
                IPolygon pConductorBuffer = GetBufferPolygon(
                    pCurrentConductor.Shape, adjacentPoleSearchBuffer);

                    //Loop through all the poles to and find any that 
                    //lie within the conductorbuffer
                    List<Pole> pAdjacentPoles = new List<Pole>();
                IRelationalOperator pRelOp = (IRelationalOperator)pConductorBuffer;
                foreach (Pole pCurrentPole in pAllPoles)
                {
                    if ((pRelOp.Contains(pCurrentPole.Shape)) &&
                        (pCurrentPole.GUID != pRefPole.GUID))
                    {

                        distToCurrentPole = pPO.ReturnDistance(pCurrentPole.Shape);
                        if (distToCurrentPole == 0)
                        {
                            Shared.WriteToLogfile("Data error - conincident poles for ref pole: " + pRefPole.GUID);
                        }
                        else
                        {
                            pPossibleAdjacentPole = new Pole(pCurrentPole.GUID, pCurrentPole.Shape);
                            pAdjacentPoles.Add(pPossibleAdjacentPole);
                        }

                        ////If we are processing a primary conductor we have 
                        ////to discount the ToPole if it does NOT have a 
                        ////PriOHConductor within x many feet 
                        ////Requirement from Alasdair added Jan 24 2017  
                        //includeAdjacentPole = true; 
                        //if (pCurrentConductor.FCName == "PRIOHCONDUCTOR")
                        //{
                        //    if (!HasConductorWithinBuffer(pRefPole, pAllConductors))
                        //        includeAdjacentPole = false; 
                        //}

                        ////We have found an adjacent pole so 
                        ////add it to a list (because there maybe a few of them)
                        //if (includeAdjacentPole)
                        //{
                        //    pPossibleAdjacentPole = new Pole(pCurrentPole.GUID, pCurrentPole.Shape);
                        //    pAdjacentPoles.Add(pPossibleAdjacentPole);
                        //}
                    }
                }

                if (pAdjacentPoles.Count == 1)
                {
                    //Just get the first one 
                    foreach (Pole pPossAdjacent in pAdjacentPoles)
                        pAdjacentPole = new Pole(pPossAdjacent.GUID, pPossAdjacent.Shape);

                }
                else if (pAdjacentPoles.Count > 1)
                {
                    //Find the closest one to the reference pole 
                    double closestDist = 10000;
                    double distToVertex = 0;

                    foreach (Pole pPossAdjacent in pAdjacentPoles)
                    {
                        distToVertex = pPO.ReturnDistance(pPossAdjacent.Shape);
                        if (distToVertex < closestDist)
                        {
                            closestDist = distToVertex;
                            pAdjacentPole = new Pole(pPossAdjacent.GUID, pPossAdjacent.Shape);
                        }
                    } 

                }
                else if (pAdjacentPoles.Count == 0)
                {
                    //No adjacent pole found for this conductor so this will 
                    //go to the next conductor 
                }

                //Get the distance of the point to the conductor 
                if (pAdjacentPole != null)
                {
                    IProximityOperator pProxOp = (IProximityOperator)pCurrentConductor.Shape;
                    distFromPoleToConductor = pProxOp.ReturnDistance(pAdjacentPole.Shape);
                }
                else
                    distFromPoleToConductor = 0;

                //If it finds more than one adjacent pole then 
                //return the one that is closest to the ref pole 
                return pAdjacentPole;
            }
            catch (Exception ex)
            {
                Shared.WriteToLogfile("Error in " + MethodBase.GetCurrentMethod() +
                    " details: " + ex.Message);
                throw new Exception("Error returning adjacent pole");
            }
        }

        /// <summary>
        /// If we are processing a primary conductor we have 
        /// to discount the ToPole if it does NOT have a 
        /// PriOHConductor within x many feet (parameterized buffer)
        /// </summary>
        /// <returns></returns>
        private bool HasConductorWithinBuffer( 
            Pole pRefPole, 
            List<Conductor> pAllConductors)
        {
            try
            {
                IPolygon pPoleBuffer = GetBufferPolygon(
                    (IPoint)pRefPole.Shape, Shared.PolePrimaryConductorBuffer);
                IRelationalOperator pRelOp = (IRelationalOperator)pPoleBuffer; 
                
                bool hasConductorWithinBuffer = false; 
                foreach (Conductor pConductor in pAllConductors)
                {
                    if (pConductor.FCName == "PRIOHCONDUCTOR")
                    {
                        if (!pRelOp.Disjoint(pConductor.Shape))
                        {
                            hasConductorWithinBuffer = true;
                            break;
                        }
                    }
                }

                if (!hasConductorWithinBuffer)
                {
                    Shared.WriteToLogfile("Discounting ToPole on basis that no primary within: " +
                        Shared.PolePrimaryConductorBuffer.ToString()); 
                }

                return hasConductorWithinBuffer; 
            }
            catch (Exception ex)
            {
                Shared.WriteToLogfile("Error in " + MethodBase.GetCurrentMethod() +
                    " details: " + ex.Message);
                throw new Exception("Error returning adjacent pole");
            }
        }

        /// <summary>
        ///New buffer logic to be added here 
        ///1. If SecondaryConductor then buffer by 
        ///the SecondaryConductorBuffer 
        ///2. If primary get the distance of the conductor 
        ///to the refPole add the PrimaryBufferOffset and 
        ///buffer by that amount (not the PoleBuffer) 
        /// </summary>
        /// <param name="pCurrentConductor"></param>
        /// <param name="pRefPole"></param>
        /// <returns></returns>
        private double GetAdjacentPoleSearchBuffer( 
            Conductor pCurrentConductor,
            Pole pRefPole)
        {
            try
            {
                double adjacentPoleBuffer = Shared.PoleBuffer;                
                if (pCurrentConductor.FCName == "SECOHCONDUCTOR")
                {
                    adjacentPoleBuffer = Shared.SecondaryConductorBuffer;
                }
                else
                {
                    //PrimaryConductor Logic for Dynamic Buffer 
                    //Get the distance between the current conductor 
                    //and the reference pole 
                    IProximityOperator pProxOp = (IProximityOperator)pRefPole.Shape;
                    double distFromPoleToConductor = pProxOp.ReturnDistance( 
                        pCurrentConductor.Shape);
                    adjacentPoleBuffer = distFromPoleToConductor +
                        Shared.PrimaryBufferIncrement;
                    if (adjacentPoleBuffer > Shared.PoleBuffer)
                    {
                        Shared.WriteToLogfile("Conductor is: " + distFromPoleToConductor +
                            " from the reference pole"); 
                        adjacentPoleBuffer = Shared.PoleBuffer;
                    }
                }
                return adjacentPoleBuffer;
            }
            catch (Exception ex)
            {
                Shared.WriteToLogfile("Error in " + MethodBase.GetCurrentMethod() +
                    " details: " + ex.Message);
                throw new Exception("Error returning search buffer distance");
            }
        }

        private void GetAllOverheadConductors(
            ref List<Conductor> pConductorList,
            IPolygon pMaxSpanRefPoleBuffer)
        {
            try
            {
                Shared.WriteToLogfile("Entering " + MethodBase.GetCurrentMethod());

                //Primary Overheads  
                GetConductorsInPolygon(
                    m_PriOHConductorFC,
                    (Hashtable)_hshAllAttributes["PRIOHCONDUCTOR"],
                    pMaxSpanRefPoleBuffer,
                    ref pConductorList);

                //Secondary Overheads  
                GetConductorsInPolygon(
                    m_SecOHConductorFC,
                    (Hashtable)_hshAllAttributes["SECOHCONDUCTOR"],
                    pMaxSpanRefPoleBuffer,
                    ref pConductorList);

                //Neutral Conductors
                GetConductorsInPolygon(
                    m_NeutralConductorFC,
                    (Hashtable)_hshAllAttributes["NEUTRALCONDUCTOR"],
                    pMaxSpanRefPoleBuffer,
                    ref pConductorList);
            }
            catch (Exception ex)
            {
                Shared.WriteToLogfile("Error in " + MethodBase.GetCurrentMethod() +
                    " details: " + ex.Message);
                throw new Exception("Error returning all overhead conductors");
            }
        }

        private void GetAllPoles(
                ref List<Pole> pPoleList,
                IPolygon pMaxSpanRefPoleBuffer)
        {
            try
            {
                Shared.WriteToLogfile("Entering " + MethodBase.GetCurrentMethod());

                //Get the buffer distance of max pole length
                ISpatialFilter pSF = new SpatialFilterClass();
                pSF.WhereClause = "status = 5";
                pSF.Geometry = pMaxSpanRefPoleBuffer;
                pSF.SpatialRel = esriSpatialRelEnum.esriSpatialRelIntersects;
                IFeatureCursor pFCursor = m_SupportStructureFC.Search(pSF, false);
                IFeature pPoleFeature = pFCursor.NextFeature();

                int fldIdx = -1;
                string poleGlobalId = string.Empty;
                string material = string.Empty;
                string phase = string.Empty;
                string conductorCode = string.Empty;
                string conductorType = string.Empty;
                int isubType = -1;
                string subType = string.Empty;

                while (pPoleFeature != null)
                {



                    poleGlobalId = string.Empty;
                    //isubType = -1;
                    //operatingVoltage = string.Empty;
                    //constructionType = string.Empty;
                    //spacing = 0;
                    //subType = string.Empty;

                    //CIRCUITID 
                    //fldIdx = (int)hshConductorAttributes["CIRCUITID"];
                    //if (pConductorFeature.get_Value(fldIdx) != DBNull.Value)
                    //    circId = pConductorFeature.get_Value(fldIdx).ToString();

                    //CONSTRUCTIONTYPE 
                    //fldIdx = (int)hshConductorAttributes["CONSTRUCTIONTYPE"];
                    //if (pConductorFeature.get_Value(fldIdx) != DBNull.Value)
                    //    GetDomainDescriptionFromDomainCode(
                    //        pConductorFC.Fields.Field[fldIdx], pConductorFeature.get_Value(fldIdx));

                    ////OPERATINGVOLTAGE 
                    //fldIdx = (int)hshConductorAttributes["OPERATINGVOLTAGE"];
                    //if (pConductorFeature.get_Value(fldIdx) != DBNull.Value)
                    //    operatingVoltage = pConductorFeature.get_Value(fldIdx).ToString();

                    ////SPACING
                    //if (hshConductorAttributes.ContainsKey("SPACING"))
                    //{
                    //    fldIdx = (int)hshConductorAttributes["SPACING"];
                    //    if (pConductorFeature.get_Value(fldIdx) != DBNull.Value)
                    //        spacing = Convert.ToInt32(pConductorFeature.get_Value(fldIdx));
                    //}

                    //GLOBALID 
                    //fldIdx = (int)hshAttributes["GLOBALID"];
                    fldIdx = pPoleFeature.Fields.FindField("GLOBALID");
                    poleGlobalId = pPoleFeature.get_Value(fldIdx).ToString();

                    //SUBTYPECD                   
                    //fldIdx = (int)hshAttributes["SUBTYPECD"];
                    //if (pPoleFeature.get_Value(fldIdx) != DBNull.Value)
                    //{
                    //    isubType = Convert.ToInt32(pPoleFeature.get_Value(fldIdx).ToString());

                    //    //Written by Tina to fetch domain value
                    //    ISubtypes subtypes = (ISubtypes)((IFeatureClass)pPoleFeature.Class);
                    //    IRowSubtypes rowSubtypes = (IRowSubtypes)pPoleFeature;
                    //    subType = subtypes.get_SubtypeName(isubType);

                    //}

                    Pole pPole = new Pole(poleGlobalId, (IPoint)pPoleFeature.ShapeCopy);
                    pPoleList.Add(pPole);
                    pPoleFeature = pFCursor.NextFeature();
                }
                Marshal.FinalReleaseComObject(pFCursor);
            }
            catch (Exception ex)
            {
                Shared.WriteToLogfile("Error in " + MethodBase.GetCurrentMethod() +
                    " details: " + ex.Message);
                throw ex;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="pPoleList"></param>
        /// <param name="pMaxSpanRefPoleBuffer"></param>
        private void GetAllRelatedDevices(
                IFeature pPoleFeature,
                ref List<RelatedDevice> pRelatedDevices)
        {
            try
            {
                Shared.WriteToLogfile("Entering " + MethodBase.GetCurrentMethod());

                //      AnchorGuy 
                //      Switch 
                //      Fuse 
                //      Transformer 
                //      VoltageRegulator 
                //      Streetlight 
                //      OpenPoint 
                //      CapacitorBank 
                //      DynamicProtectiveDevice 
                //      PrimaryRiser
                //      SecondaryRiser  
                //      Antenna 
                //      PhotoVoltaicCell  
                //      StepDown 

                //JointOwner
                //GetRelatedDevices(pPoleFeature, m_JointOwnerRC, ref pRelatedDevices);
                //AnchorGuy
                GetRelatedDevices(pPoleFeature, m_AnchorGuyRC, ref pRelatedDevices);
                //Switch
                GetRelatedDevices(pPoleFeature, m_SwitchRC, ref pRelatedDevices);
                //Fuse
                GetRelatedDevices(pPoleFeature, m_FuseRC, ref pRelatedDevices);
                //Transformer
                GetRelatedDevices(pPoleFeature, m_TransformerRC, ref pRelatedDevices);
                //VoltageRegulator
                GetRelatedDevices(pPoleFeature, m_VoltageRegulatorRC, ref pRelatedDevices);
                //Streetlight 
                GetRelatedDevices(pPoleFeature, m_StreetlightRC, ref pRelatedDevices);
                //OpenPoint 
                GetRelatedDevices(pPoleFeature, m_OpenPointRC, ref pRelatedDevices);
                //CapacitorBank
                GetRelatedDevices(pPoleFeature, m_CapBankRC, ref pRelatedDevices);
                //DPD 
                GetRelatedDevices(pPoleFeature, m_DPDRC, ref pRelatedDevices);
                //PrimaryRiser 
                GetRelatedDevices(pPoleFeature, m_PrimaryRiserRC, ref pRelatedDevices);
                //SecondaryRiser 
                GetRelatedDevices(pPoleFeature, m_SecondaryRiserRC, ref pRelatedDevices);
                //Antenna  
                GetRelatedDevices(pPoleFeature, m_AntennaRC, ref pRelatedDevices);
                //PhotoVoltaicCell  
                GetRelatedDevices(pPoleFeature, m_PVRC, ref pRelatedDevices);
                //StepDown 
                GetRelatedDevices(pPoleFeature, m_StepDownRC, ref pRelatedDevices);
            }
            catch (Exception ex)
            {
                Shared.WriteToLogfile("Error in " + MethodBase.GetCurrentMethod() +
                    " details: " + ex.Message);
                throw ex;
            }
        }
        /// <summary>
        /// Finds the conductors that are within the max pole length 
        /// buffer that are also within tolerance of the pole point 
        /// </summary>
        /// <param name="pAllConductorList"></param>
        /// <param name="pPoleTolBuffer"></param>
        private void GetNearbyConductors(
            Pole pRefPole,
            IPolygon pPoleTolBuffer,
            List<Pole> pAllPoles, 
            List<Conductor> pAllConductors,
            List<RelatedDevice> pAllRelatedDevices,
            ref List<Conductor> pNearbyConductorList)
        {
            try
            {
                //Here we need to ensure that the start point where we 
                //would like to begin tracing for the conductor is the acutal 
                //from point of the IPolyline, if we derived this Conductor 
                //from the connected device then the start point should always 
                //be where the connected device is 
                //If we derived this Conductor from a buffer of the ref pole 
                //then we should start the conductor at 
                //  a) the end point closest to the ref pole is within tolerance 
                //  b) the vertex of Conductor closest to the ref pole that is 
                //  within tolerance of the ref pole (this means splitting the 
                //  Conductor into two parts 
                //  c) where no end point or vertex within tolerance then we 
                //  take the closest point on the Conductor to the ref Pole 
                //  (again this means splitting the Conductor into 2 parts) 

                //1. Get everything connected to the related devices 
                Hashtable hshNearbyConductors = new Hashtable();
                string conductorKey = string.Empty;
                bool lookforConductorsAtPoint = false;
                bool isAtFromPoint = false;
                bool isAtToPoint = false;
                bool isBetweenDeviceAndRefPole = false; 
                Hashtable hshCircIds = new Hashtable(); 

                foreach (RelatedDevice pRelDevice in pAllRelatedDevices)
                {
                    lookforConductorsAtPoint = false;
                    switch (pRelDevice.DeviceType)
                    {
                        //list of all featureclasses within geometric network related 
                        //to the pole 
                        case "SWITCH":
                            lookforConductorsAtPoint = true;
                            break;
                        case "TRANSFORMER":
                            lookforConductorsAtPoint = true;
                            break;
                        case "FUSE":
                            lookforConductorsAtPoint = true;
                            break;
                        case "DYNAMICPROTECTIVEDEVICE":
                            lookforConductorsAtPoint = true;
                            break;
                        case "VOLTAGEREGULATOR":
                            lookforConductorsAtPoint = true;
                            break;
                        case "CAPACITORBANK":
                            lookforConductorsAtPoint = true;
                            break;
                        case "FAULTINDICATOR":
                            lookforConductorsAtPoint = true;
                            break;
                        case "OPENPOINT":
                            lookforConductorsAtPoint = true;
                            break;
                        case "PRIMARYMETER":
                            lookforConductorsAtPoint = true;
                            break;
                        case "PRIMARYRISER":
                            lookforConductorsAtPoint = true;
                            break;
                        case "SECONDARYRISER":
                            lookforConductorsAtPoint = true;
                            break;
                        case "ANTENNA":
                            lookforConductorsAtPoint = true;
                            break;
                        case "PHOTOVOLTAICCELL":
                            lookforConductorsAtPoint = true;
                            break;
                        case "STEPDOWN":
                            lookforConductorsAtPoint = true;
                            break;
                        case "STREETLIGHT":
                            lookforConductorsAtPoint = true;
                            break;
                        default:
                            lookforConductorsAtPoint = false;
                            break;
                    }

                    //Check if coincident                     
                    if (lookforConductorsAtPoint)
                    {
                        foreach (Conductor pConductor in pAllConductors)
                        {
                            //There should be 2 in most cases 
                            isAtFromPoint = IsConincident(pConductor.Shape.FromPoint, pRelDevice.Shape);                             
                            isAtToPoint = IsConincident(pConductor.Shape.ToPoint, pRelDevice.Shape);
                            
                            if (isAtFromPoint || isAtToPoint)
                            {
                                //Check if conductor runs between device and ref pole 
                                isBetweenDeviceAndRefPole = false; 
                                if (isAtFromPoint && IsConincident(pConductor.Shape.ToPoint, pRefPole.Shape))
                                    isBetweenDeviceAndRefPole = true;
                                if (isAtToPoint && IsConincident(pConductor.Shape.FromPoint, pRefPole.Shape))
                                    isBetweenDeviceAndRefPole = true;

                                //Do not add if it is already in the list 
                                conductorKey = pConductor.FCName + pConductor.OID;
                                if (!hshNearbyConductors.ContainsKey(conductorKey))
                                {
                                    IClone pClone = (IClone)pConductor.Shape;
                                    IPolyline pPolylineClone = (IPolyline)pClone.Clone();
                                    pPolylineClone.SpatialReference =
                                        pConductor.Shape.SpatialReference;

                                    //Make the start point of the polyline at the 
                                    //actual device (simplifies things) 
                                    if (!isAtFromPoint)
                                    {
                                        pPolylineClone.ReverseOrientation();
                                        isAtFromPoint = IsConincident(pPolylineClone.FromPoint, pRelDevice.Shape);
                                        //Debug.Print(isAtFromPoint.ToString()); 
                                    }

                                    //Cannot be between ref pole and device 
                                    if (!isBetweenDeviceAndRefPole)
                                    {
                                        //Copy the conductor 
                                        Conductor pNearbyConductor = CopyConductor(pConductor, pPolylineClone);
                                        pNearbyConductorList.Add(pNearbyConductor);
                                        hshNearbyConductors.Add(conductorKey, "");
                                    }
                                    else
                                        Debug.Print("found one"); 
                                }
                            }
                        }
                    }
                }

                //2. Get everything within tolerance of the ref pole  
                IProximityOperator pProxOp = (IProximityOperator)pRefPole.Shape;
                IRelationalOperator pRelOp = (IRelationalOperator)pPoleTolBuffer;
                double closestPoleDist = 0; 
                double distToFrom = 0;
                double distToTo = 0;
                double distToVertex = 0;
                double closestDist = 0;
                IPoint pClosestVertex = null;
                object obj = Type.Missing;

                //Is the pole a primary pole (must have primary conductor within 
                //certain paramterized distance) 
                //bool isPrimaryPole = HasConductorWithinBuffer(pRefPole, pAllConductors);
                //bool includeConductor = true; 
                
                foreach (Conductor pConductor in pAllConductors)
                {
                    //Exclude primary conductors if this is not a primary pole  
                    //includeConductor = true;
                    //if ((pConductor.FCName == "PRIOHCONDUCTOR") && (isPrimaryPole == false))
                    //    includeConductor = false;  

                    //Check if it is within the ray buffer from the pole  
                    //if ((includeConductor == true) && (!pRelOp.Disjoint(pConductor.Shape)))
                    if (!pRelOp.Disjoint(pConductor.Shape))
                    {
                        //Do not add if it is already in the list of nearby conductors 
                        conductorKey = pConductor.FCName + pConductor.OID;
                        if (!hshNearbyConductors.ContainsKey(conductorKey))
                        {
                            //Check this is not a 'go around' of the reference 
                            //pole 
                            if (!IsGoAround(pConductor.Shape, pRefPole.Shape, conductorKey))
                            {
                                //Clone to be safe 
                                IClone pClone = (IClone)pConductor.Shape;
                                IPolyline pPolylineClone = (IPolyline)pClone.Clone();
                                pPolylineClone.SpatialReference =
                                    pConductor.Shape.SpatialReference;

                                //(a) Look for an endpoint within tolerance of the pole 
                                distToFrom = pProxOp.ReturnDistance(pPolylineClone.FromPoint);
                                distToTo = pProxOp.ReturnDistance(pPolylineClone.ToPoint);
                                if ((distToFrom < Shared.PoleBuffer) ||
                                (distToTo < Shared.PoleBuffer))
                                {
                                    //This is scenario (a) (endpoint within tolerance)

                                    //ReverseOrientation if necessary 
                                    if (!(distToFrom < distToTo))
                                        pPolylineClone.ReverseOrientation();

                                    //Check to make sure that there is no other 
                                    //pole other than the ref pole that is closer 
                                    //to this conductor startpoint - if so then do NOT add 
                                    //this as a nearby conductor (otherwise this conductor 
                                    //is on the other side of a closer pole) 
                                    Pole pClosestPole = GetClosestPoleFromPoint(
                                        pPolylineClone.FromPoint,
                                        pAllPoles,
                                        ref closestPoleDist);
                                    if (pClosestPole.GUID == pRefPole.GUID)
                                    {
                                        //Copy the conductor
                                        Conductor pNearbyConductor = CopyConductor(pConductor, pPolylineClone);
                                        pNearbyConductorList.Add(pNearbyConductor);
                                        hshNearbyConductors.Add(conductorKey, "");
                                    }
                                    else
                                    {
                                        Debug.Print("found another closer pole at the conductor end point!"); 
                                    }
                                }
                                else
                                {
                                    //Look for scenario (b) (vertex within tolerance)
                                    //find the closest vertex to the refPole and split 
                                    //the Conductor into 2 parts 
                                    closestDist = 10000;
                                    IPointCollection pPointColl = (IPointCollection)pPolylineClone;
                                    for (int i = 0; i < pPointColl.PointCount; i++)
                                    {
                                        distToVertex = pProxOp.ReturnDistance(pPointColl.Point[i]);
                                        if (distToVertex < closestDist)
                                        {
                                            closestDist = distToVertex;
                                            pClosestVertex = (IPoint)CloneShape(
                                                pPointColl.Point[i], pRefPole.Shape.SpatialReference);
                                        }
                                    }
                                    Shared.WriteToLogfile(pConductor.FCName + pConductor.OID + ": " +  
                                        "found vertex (scenario b) at dist: " + closestDist.ToString());

                                    if (closestDist < Shared.PoleBuffer)
                                    {
                                        //Check the closest pole to the split point 
                                        //is the reference pole 
                                        Pole pClosestPole = GetClosestPoleFromPoint(
                                                    pClosestVertex,
                                                    pAllPoles,
                                                    ref closestPoleDist);
                                        if (pClosestPole.GUID == pRefPole.GUID)
                                        {
                                            //Split the polyline at the closest vertex and add both Conductors 
                                            //to the NearbyConductor list 
                                            List<IPolyline> pPolylines = SplitPolylineIntoTwo(
                                                pPolylineClone,
                                                pClosestVertex,
                                                pRefPole.Shape.SpatialReference);

                                            foreach (IPolyline pPolyline in pPolylines)
                                            {
                                                //Copy the conductors
                                                Conductor pNearbyConductor = CopyConductor(pConductor, pPolyline);
                                                pNearbyConductorList.Add(pNearbyConductor);
                                            }
                                            hshNearbyConductors.Add(conductorKey, "");

                                        }
                                        else
                                        {
                                            Debug.Print("found another closer pole at that vertex!");
                                        }                                        
                                    }
                                    else //no vertex within ray buffer 
                                    {
                                        //Look for scenario (c) where we have to actually crack 
                                        //a vertex into the polyline, since none of the existing 
                                        //vertices are within tolerance 
                                        //Debug.Print("Scenario c or");

                                        //1.Find the closest point on the line to the point 
                                        //IPolyline.QueryPointAndDistance 
                                        IPoint pSplitPoint = new PointClass();
                                        double distAlongCurve = 0;
                                        double distFromCurve = 0;
                                        bool rightSide = false;

                                        //IMap pmap = ((IMxDocument)m_app.Document).FocusMap;
                                        //IActiveView pAV = (IActiveView)pmap;
                                        //ESRI.ArcGIS.Carto.IGraphicsContainer pGC = (ESRI.ArcGIS.Carto.IGraphicsContainer)pmap;
                                        //pGC.DeleteAllElements();

                                        //GraphicUtils.DrawPolyline(pmap, pPolylineClone, GraphicUtils.GetRGBColour(0, 0, 255), "");
                                        //pAV.Refresh();
                                        //MessageBox.Show("here is the conductor");

                                        pPolylineClone.QueryPointAndDistance(esriSegmentExtension.esriNoExtension,
                                            pRefPole.Shape, false, pSplitPoint, ref distAlongCurve, ref distFromCurve,
                                            ref rightSide);

                                        //pGC.DeleteAllElements();
                                        //GraphicUtils.DrawPoint(pmap, pSplitPoint, GraphicUtils.GetRGBColour(0, 0, 255), esriSimpleMarkerStyle.esriSMSX);
                                        //pAV.Refresh();
                                        //MessageBox.Show("here is the split point");

                                        //2.Split the conductor at that point using IPolycurve.SplitAtPoint 
                                        bool splitHappened = false;
                                        int newPartIdx = 0;
                                        int newSegIdx = 0;
                                        //Debug.Print("Vertice count prior to split = " + ((IPointCollection)pPolylineClone).PointCount.ToString()); 

                                        pPolylineClone.SplitAtDistance(
                                            distAlongCurve, false, false, out splitHappened, out newPartIdx,
                                            out newSegIdx);

                                        //Debug.Print("Vertice count after split = " + ((IPointCollection)pPolylineClone).PointCount.ToString());

                                        if (!splitHappened)
                                        {
                                            //Unable to split so ignore this conductor, this 
                                            //is very rare occurrence (1 in 100,000 poles) 
                                            Shared.WriteToLogfile("Unable to split conductor: " + 
                                                conductorKey + " at closest point to reference pole " + 
                                                "so the conductor is being ignored");
                                        }
                                        else
                                        {

                                            //Check the closest pole to the split point 
                                            //is the reference pole 
                                            Pole pClosestPole = GetClosestPoleFromPoint(
                                                        pSplitPoint,
                                                        pAllPoles,
                                                        ref closestPoleDist);
                                            if (pClosestPole.GUID == pRefPole.GUID)
                                            {
                                                //Split the polyline at the closest vertex and add both Conductors 
                                                //to the NearbyConductor list 
                                                List<IPolyline> pPolylines = SplitPolylineIntoTwo(
                                                pPolylineClone,
                                                pSplitPoint,
                                                pRefPole.Shape.SpatialReference);
                                                foreach (IPolyline pPolyline in pPolylines)
                                                {
                                                    //Copy the conductors
                                                    //Debug.Print(IsConincident(pPolyline.ToPoint, pSplitPoint).ToString());
                                                    //Debug.Print(IsConincident(pPolyline.FromPoint, pSplitPoint).ToString());
                                                    //pGC.DeleteAllElements();
                                                    //GraphicUtils.DrawPolyline(pmap, pPolyline, GraphicUtils.GetRGBColour(0, 0, 255), "");
                                                    //pAV.Refresh();
                                                    //MessageBox.Show("here is the conductor");


                                                    Conductor pNearbyConductor = CopyConductor(pConductor, pPolyline);
                                                    pNearbyConductorList.Add(pNearbyConductor);
                                                }
                                                hshNearbyConductors.Add(conductorKey, "");
                                            }
                                            else
                                            {
                                                Debug.Print("found another closer pole at that split point!");
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Shared.WriteToLogfile("Error in " + MethodBase.GetCurrentMethod() +
                    " details: " + ex.Message);
                throw new Exception("Error returning nearby conductors");
            }
        }
        /// <summary>
        /// Finds the closest pole to the passed point 
        /// </summary>
        /// <returns></returns>
        private Pole GetClosestPoleFromPoint(IPoint pPoint, List<Pole> pAllPoles, ref double dist)
        {
            try
            {

                Pole pClosestPole = null; 
                double distToPoint;
                double closestDist = 5000; 
                IProximityOperator pProxOp = (IProximityOperator)pPoint; 
                foreach (Pole pCurrentPole in pAllPoles)
                {
                    distToPoint = pProxOp.ReturnDistance(pCurrentPole.Shape);
                    if (distToPoint < closestDist)
                    {
                        closestDist = distToPoint;
                        pClosestPole = pCurrentPole;
                        dist = closestDist;  
                    }
                }
                return pClosestPole; 
            }
            catch (Exception ex)
            {
                Shared.WriteToLogfile("Error in " + MethodBase.GetCurrentMethod() +
                    " details: " + ex.Message);
                throw new Exception("Error returning nearby conductors");
            }
        }

        /// <summary>
        /// Copies the conductor, and uses the passed IPolyline
        /// </summary>
        /// <param name="pConductor"></param>
        /// <param name="pPolyline"></param>
        /// <returns></returns>
        private Conductor CopyConductor(Conductor pConductor, IPolyline pPolyline)
        {
            try
            {
                Conductor pNewConductor = new Conductor(
                                            pConductor.OID,
                                            pConductor.Subtype,
                                            pConductor.FCName,
                                            pConductor.GUID,
                                            pConductor.CircId,
                                            pConductor.OperatingVoltage,
                                            pConductor.ConstructionType,
                                            pConductor.Spacing,
                                            pPolyline,
                                            GetConductorInfoForConductor(pConductor.FCName, pConductor.OID));
                return pNewConductor; 
            }
            catch (Exception ex)
            {
                Shared.WriteToLogfile("Error in " + MethodBase.GetCurrentMethod() +
                    " details: " + ex.Message);
                throw new Exception("Error returning all related devices");
            }
        }
        /// <summary>
        /// Uses a specific set of criteria to determine whether a conductor 
        /// is a 'go around' - meaning that the conductor is not actually 
        /// supported by the particular pole 
        /// </summary>
        /// <param name="pConductorPolyline"></param>
        /// <param name="pRefPolePoint"></param>
        /// <returns></returns>
        private bool IsGoAround( 
            IPolyline pConductorPolyline, 
            IPoint pRefPolePoint, 
            string conductorKey)
        {
            try
            {
                bool isGoAround = false;

                //Step 1. Check the conductor has > GoAroundMinimumSegmentCount 
                //        segments (otherwise this is NOT a go around)
                Int32 goAroundMinSegCount = Shared.GoAroundPointCount - 1; 
                ISegmentCollection pSegColl = (ISegmentCollection)pConductorPolyline;
                if (pSegColl.SegmentCount < goAroundMinSegCount)
                    return isGoAround;

                //Step 2. Find the closest point to reference pole and check that 
                //        this point is not closer than GoAroundTolerance 
                //        (otherwise this is NOT a go around)
                IProximityOperator pProxOp = (IProximityOperator)pRefPolePoint; 
                Hashtable hshPointProximity = new Hashtable();
                double distToRefPole = 0;
                double closestDistToRefPole = 500;
                IPointCollection pPointColl = (IPointCollection)pConductorPolyline; 
                for (int i = 0; i < pPointColl.PointCount; i++)
                {
                    distToRefPole = pProxOp.ReturnDistance(pPointColl.Point[i]);
                    if (distToRefPole < closestDistToRefPole)
                        closestDistToRefPole = distToRefPole;

                    //Create an ordered list of proximity of each vertex to the 
                    //reference pole 
                    hshPointProximity.Add(i, distToRefPole); 
                }

                //Closest dist of vertex to pole must be < ray buffer 
                //and > GoAroundTolerance 
                if (closestDistToRefPole < Shared.GoAroundTolerance)
                    return isGoAround;
                if (closestDistToRefPole > Shared.PoleBuffer)
                    return isGoAround; 
                
                //Step 3. Loop through the segments and make sure there are 
                //        at least GoAroundPointCount adjacent points along the 
                //        conductor that are within GoAroundProxPercentFromClosestPoint 
                //        feet of the closest point 
                Shared.WriteToLogfile("Closest vertex from ref pole is at: " + closestDistToRefPole.ToString());                
                double onePlueGoAroundProxPercent = (double)1 + ((double)(Shared.GoAroundProxPercent / (double)100));
                double closestPolePlusProxPercent = closestDistToRefPole * onePlueGoAroundProxPercent;
                Shared.WriteToLogfile("Closest vertex plus GoAroundProxPercentFromClosestPoint is: " +
                    closestPolePlusProxPercent.ToString());
                int adjacentPointCountWithinTolerance = 0;
                int maxAdjacentPointsWithinTol = 0;                 
            
                for (int i = 0; i < pPointColl.PointCount; i++)
                {
                    distToRefPole = (double)hshPointProximity[i];
                    if (distToRefPole < closestPolePlusProxPercent)
                    {
                        adjacentPointCountWithinTolerance++;
                        if (adjacentPointCountWithinTolerance >
                            maxAdjacentPointsWithinTol)
                        {
                            maxAdjacentPointsWithinTol = adjacentPointCountWithinTolerance; 
                        }
                    }
                    else
                        adjacentPointCountWithinTolerance = 0;                                         
                }

                Shared.WriteToLogfile("Found: " + maxAdjacentPointsWithinTol.ToString() +
                        "adjacent vertexes on conductor in a zone of less than: " +
                        closestDistToRefPole.ToString() + " plus: " + Shared.GoAroundProxPercent.ToString() +
                        " %" + "from the reference pole!");
                if (maxAdjacentPointsWithinTol >= Shared.GoAroundPointCount)
                {
                    //We have found GoAroundPointCount adjacent points in 
                    //the 'Goldilocks zone' so this is a go around
                    isGoAround = true;
                }
                
                if (isGoAround)
                    Shared.WriteToLogfile("Conductor: " + conductorKey + " was found to contain a go around"); 
                return isGoAround; 
            }
            catch (Exception ex)
            {
                Shared.WriteToLogfile("Error in " + MethodBase.GetCurrentMethod() +
                    " details: " + ex.Message);
                throw new Exception("Error finding go around");
            }
        }

        private List<ConductorInfo> GetConductorInfoForConductor(
            string conductorFCName,
            int conductorOId)
        {
            try
            {
                List<ConductorInfo> pConductorInfos = new List<ConductorInfo>();
                IFeature pConductorFeature = null;
                string conInfoGlobalid = string.Empty;
                string conductorSize = string.Empty;
                string conductorUse = string.Empty;
                string conductorInfoTblName = string.Empty;
                int conductorCount = 0;
                int conductorCode = 0; 
                string material = string.Empty;
                string phase = string.Empty;
                string conductorType = string.Empty;
                Hashtable hshConductorInfoAttributes = null;
                Hashtable hshConductorAttributes = null; 
                ISet pSet = null;
                int fldIdx = -1;
                bool loadRelatedConductorInfo = true; 

                switch (conductorFCName)
                {
                    case "PRIOHCONDUCTOR":                        
                        conductorInfoTblName = "PRIOHCONDUCTORINFO";
                        hshConductorInfoAttributes = (Hashtable)_hshAllAttributes[conductorInfoTblName];
                        pConductorFeature = m_PriOHConductorFC.GetFeature(conductorOId);
                        pSet = m_PriOHConductorInfoRC.GetObjectsRelatedToObject(pConductorFeature);
                        break;
                    case "SECOHCONDUCTOR":
                        conductorInfoTblName = "SECOHCONDUCTORINFO";
                        hshConductorInfoAttributes = (Hashtable)_hshAllAttributes[conductorInfoTblName];
                        pConductorFeature = m_SecOHConductorFC.GetFeature(conductorOId);
                        pSet = m_SecOHConductorInfoRC.GetObjectsRelatedToObject(pConductorFeature);
                        break;
                    case "NEUTRALCONDUCTOR":
                        loadRelatedConductorInfo = false;
                        break;
                    default:
                        throw new Exception("Unhandled Conductor Featureclass");
                }

                if (loadRelatedConductorInfo)
                {
                    //Load the conductor info records 
                    pConductorInfos.Clear();
                    pSet.Reset();
                    IRow pConductorInfoRow = (IRow)pSet.Next();
                    while (pConductorInfoRow != null)
                    {
                        conductorCode = -1;
                        conductorSize = string.Empty;
                        conductorUse = string.Empty;
                        conductorCount = 0;
                        material = string.Empty;
                        phase = string.Empty;
                        conductorType = string.Empty;

                        //GLOBALID
                        fldIdx = (int)hshConductorInfoAttributes["GLOBALID"];
                        conInfoGlobalid = pConductorInfoRow.get_Value(fldIdx).ToString();

                        //PGE_CONDUCTORCODE 
                        if (hshConductorInfoAttributes.ContainsKey("PGE_CONDUCTORCODE"))
                        {
                            fldIdx = (int)hshConductorInfoAttributes["PGE_CONDUCTORCODE"];
                            if (pConductorInfoRow.get_Value(fldIdx) != DBNull.Value)
                                conductorCode = Convert.ToInt32(pConductorInfoRow.get_Value(fldIdx));
                        }
                        
                        //CONDUCTORSIZE 
                        fldIdx = (int)hshConductorInfoAttributes["CONDUCTORSIZE"];
                        conductorSize = GetFieldValue(pConductorInfoRow, fldIdx);
                        if ((conductorSize.Trim() == string.Empty) || (conductorSize == "<NULL>"))
                            conductorSize = "Unknown";

                        //CONDUCTORUSE 
                        fldIdx = (int)hshConductorInfoAttributes["CONDUCTORUSE"];
                        conductorUse = GetFieldValue(pConductorInfoRow, fldIdx);
                        if ((conductorUse.Trim() == string.Empty) || (conductorUse == "<NULL>"))
                            conductorUse = "Unknown";

                        //CONDUCTORCOUNT
                        fldIdx = (int)hshConductorInfoAttributes["CONDUCTORCOUNT"];
                        if (pConductorInfoRow.get_Value(fldIdx) != DBNull.Value)
                            conductorCount = Convert.ToInt32(pConductorInfoRow.get_Value(fldIdx));

                        //CONDUCTORTYPE Added by Tina
                        fldIdx = (int)hshConductorInfoAttributes["CONDUCTORTYPE"];
                        if (pConductorInfoRow.get_Value(fldIdx) != DBNull.Value)
                            conductorType = GetDomainDescriptionFromDomainCode(
                            pConductorInfoRow.Fields.get_Field(fldIdx),
                            pConductorInfoRow.get_Value(fldIdx));

                        //MATERIAL 
                        fldIdx = (int)hshConductorInfoAttributes["MATERIAL"];
                        material = GetFieldValue(pConductorInfoRow, fldIdx);
                        if ((material.Trim() == string.Empty) || (material == "<NULL>"))
                            material = "Unknown";

                        //PHASEDESIGNATION 
                        fldIdx = (int)hshConductorInfoAttributes["PHASEDESIGNATION"];
                        if (pConductorInfoRow.get_Value(fldIdx) != DBNull.Value)
                            phase = GetDomainDescriptionFromDomainCode(
                            pConductorInfoRow.Fields.get_Field(fldIdx),
                            pConductorInfoRow.get_Value(fldIdx));

                        ConductorInfo pConInfo = new ConductorInfo(
                            pConductorInfoRow.OID,
                            conductorInfoTblName,
                            conInfoGlobalid, 
                            conductorCode,
                            conductorSize,
                            conductorUse,
                            conductorCount,
                            conductorType,
                            material,
                            phase);

                        //Add the conductorinfo 
                        pConductorInfos.Add(pConInfo);

                        pConductorInfoRow = (IRow)pSet.Next();
                    }
                    Marshal.FinalReleaseComObject(pSet);
                }
                else
                {
                    //NeutralConductor situation 
                    //Get the conductorinfo information from the 
                    //Conductor record itself
                    pConductorFeature = m_NeutralConductorFC.GetFeature(conductorOId);
                    hshConductorAttributes = (Hashtable)_hshAllAttributes[conductorFCName];
                    conductorSize = string.Empty;
                    conductorUse = string.Empty;
                    conductorCount = 0;
                    material = string.Empty;
                    phase = string.Empty;
                    conductorType = string.Empty;
                    conductorCode = -1;                      

                    //GLOBALID
                    conInfoGlobalid = string.Empty; //empty since there is no conductorinfo 

                    //CONDUCTORSIZE 
                    fldIdx = (int)hshConductorAttributes["CONDUCTORSIZE"];
                    conductorSize = GetFieldValue(pConductorFeature, fldIdx);
                    if ((conductorSize.Trim() == string.Empty) || (conductorSize == "<NULL>"))
                        conductorSize = "Unknown";

                    //CONDUCTORUSE 
                    fldIdx = (int)hshConductorAttributes["CONDUCTORUSE"];
                    conductorUse = GetFieldValue(pConductorFeature, fldIdx);
                    if ((conductorUse.Trim() == string.Empty) || (conductorUse == "<NULL>"))
                        conductorUse = "Unknown";

                    //CONDUCTORCOUNT
                    fldIdx = (int)hshConductorAttributes["CONDUCTORCOUNT"];
                    if (pConductorFeature.get_Value(fldIdx) != DBNull.Value)
                        conductorCount = Convert.ToInt32(pConductorFeature.get_Value(fldIdx));

                    //CONDUCTORTYPE
                    conductorType = "Unknown"; //field does not exist for NeutralConductor 

                    //MATERIAL 
                    fldIdx = (int)hshConductorAttributes["MATERIAL"];
                    material = GetFieldValue(pConductorFeature, fldIdx);
                    if ((material.Trim() == string.Empty) || (material == "<NULL>"))
                        material = "Unknown";

                    //PHASEDESIGNATION 
                    phase = "";

                    ConductorInfo pConInfo = new ConductorInfo(
                        0,
                        conductorFCName,
                        conInfoGlobalid, 
                        conductorCode, 
                        conductorSize,
                        conductorUse,
                        conductorCount,
                        conductorType,
                        material,
                        phase);

                    //Add the conductorinfo 
                    pConductorInfos.Add(pConInfo);
                }

                return pConductorInfos;
            }
            catch (Exception ex)
            {
                Shared.WriteToLogfile("Error in " + MethodBase.GetCurrentMethod() +
                    " details: " + ex.Message);
                throw new Exception("Error returning conductorinfo information for conductor");
            }
        }

        /// <summary>
        /// Split the passed IPolyline into 2 IPolylines at the 
        /// passed split point, and reverses the orientation if 
        /// necessary so that both the new IPolylines start at the 
        /// split point 
        /// </summary>
        /// <param name="pPolylineToSplit"></param>
        /// <param name="pSplitPoint"></param>
        /// <param name="pSR"></param>
        /// <returns></returns>
        private List<IPolyline> SplitPolylineIntoTwo(
            IPolyline pPolylineToSplit,
            IPoint pSplitPoint,
            ISpatialReference pSR)
        {
            try
            {
                //Split the polyline at the closest vertex and add both Conductors 
                //to the NearbyConductor list
                List<IPolyline> pPolylines = new List<IPolyline>();
                bool reachedSplitPoint = false;
                IClone pClone = null;

                ISegmentCollection pSegColl1 = new PolylineClass();
                IPolyline pPolyline1 = (IPolyline)pSegColl1;
                pPolyline1.SpatialReference = pSR;

                ISegmentCollection pSegColl2 = new PolylineClass();
                IPolyline pPolyline2 = (IPolyline)pSegColl2;
                pPolyline2.SpatialReference = pSR;

                ISegmentCollection pSegCollOrig = (ISegmentCollection)(pPolylineToSplit);
                reachedSplitPoint = false;
                for (int j = 0; j < pSegCollOrig.SegmentCount; j++)
                {
                    if (IsConincident(pSegCollOrig.Segment[j].FromPoint, pSplitPoint))
                        reachedSplitPoint = true;

                    if (!reachedSplitPoint)
                    {
                        pClone = (IClone)pSegCollOrig.Segment[j];
                        pSegColl1.AddSegment((ISegment)pClone.Clone());
                    }
                    else
                    {
                        pClone = (IClone)pSegCollOrig.Segment[j];
                        pSegColl2.AddSegment((ISegment)pClone.Clone());
                    }
                }

                //Simplify both 
                ITopologicalOperator2 pTopo = (ITopologicalOperator2)pPolyline1;
                pTopo.IsKnownSimple_2 = false;
                pTopo.Simplify();
                pTopo = (ITopologicalOperator2)pPolyline2;
                pTopo.IsKnownSimple_2 = false;
                pTopo.Simplify();

                //Reverse orientation if necessary 
                if (!IsConincident(pPolyline1.FromPoint, pSplitPoint))
                {
                    //This condition should be true 
                    //Debug.Print(IsConincident(pPolyline1.ToPoint, pSplitPoint).ToString());
                    pPolyline1.ReverseOrientation();
                }
                if (!IsConincident(pPolyline2.FromPoint, pSplitPoint))
                {
                    //This condition should be true 
                    //Debug.Print(IsConincident(pPolyline1.ToPoint, pSplitPoint).ToString());
                    pPolyline2.ReverseOrientation();
                }

                //Add the two polylines to a list and return the list 
                pPolylines.Add(pPolyline1);
                pPolylines.Add(pPolyline2);
                return pPolylines;
            }
            catch (Exception ex)
            {
                Shared.WriteToLogfile("Error in " + MethodBase.GetCurrentMethod() +
                    " details: " + ex.Message);
                throw new Exception("Error splitting polyline");

            }
        }

        /// <summary>
        /// Clones the passed geometry 
        /// </summary>
        /// <param name="pGeometryToClone"></param>
        /// <param name="pSR"></param>
        /// <returns></returns>
        private IGeometry CloneShape(IGeometry pGeometryToClone, ISpatialReference pSR)
        {
            try
            {
                IClone pClone = (IClone)pGeometryToClone;
                IGeometry pGeom = (IGeometry)pClone.Clone();
                pGeom.SpatialReference = pSR;
                return pGeom;
            }
            catch (Exception ex)
            {
                Shared.WriteToLogfile("Error in " + MethodBase.GetCurrentMethod() +
                    " details: " + ex.Message);
                throw new Exception("Error cloning shape");
            }
        }

        private bool IsConincident(IPoint pPoint1, IPoint pPoint2)
        {
            try
            {
                bool isPointConincident = false;
                if ((pPoint1.X == pPoint2.X) &&
                    ((pPoint1.Y == pPoint2.Y)))
                {
                    isPointConincident = true;
                }
                else
                {
                    //Quick check based on delta in X and Y coord 
                    double deltaX = Math.Abs(pPoint1.X - pPoint2.X);
                    double deltaY = Math.Abs(pPoint1.Y - pPoint2.Y);

                    if ((deltaX > 1) || (deltaY > 1))
                    {
                        isPointConincident = false;
                    }
                    else
                    {
                        //Check if less that coincidence tolerance e.g. 
                        //0.001 feet 
                        double dist = GetSpanLength(pPoint1, pPoint2);
                        if (dist < PoleLoadConstants.COINCIDENCE_THRESHOLD)
                            isPointConincident = true;
                    }
                }
                return isPointConincident;
            }
            catch (Exception ex)
            {
                Shared.WriteToLogfile("Error in " + MethodBase.GetCurrentMethod() +
                    " details: " + ex.Message);
                throw new Exception("Error determining if points are coincident");
            }
        }

        /// <summary>
        /// Returns the conductors within the searchpolygon
        /// </summary>
        /// <param name="pConductorFC"></param>
        /// <param name="hshConductorAttributes"></param>
        /// <param name="pSearchPolygon"></param>
        /// <param name="pConductorList"></param>
        private void GetConductorsInPolygon(
            IFeatureClass pConductorFC,
            Hashtable hshConductorAttributes,
            IPolygon pSearchPolygon,
            ref List<Conductor> pConductorList)
        {
            try
            {
                //Shared.WriteToLogfile("Entering " + MethodBase.GetCurrentMethod());
                IDataset pDS = (IDataset)pConductorFC;
                string conductorFCName = GetShortDatasetName(pDS.Name).ToUpper();
                ISpatialFilter pSF = new SpatialFilterClass();
                //screen out ones that are not in service 
                pSF.WhereClause = "status = 5";
                //Neutral Conductor should just include overhead 
                if (conductorFCName == "NEUTRALCONDUCTOR")
                    pSF.WhereClause += " And subtypecd = 1"; //SubtypeCD for overhead 
                pSF.Geometry = pSearchPolygon;
                pSF.SpatialRel = esriSpatialRelEnum.esriSpatialRelIntersects;
                IFeatureCursor pFCursor = pConductorFC.Search(pSF, false);
                IFeature pConductorFeature = pFCursor.NextFeature();

                string circId = string.Empty;
                string operatingVoltage = string.Empty;
                string constructionType = string.Empty;
                int spacing = 0;
                string conductorGlobalid = string.Empty;
                string conInfoGlobalid = string.Empty;
                string conductorSize = string.Empty;
                string conductorUse = string.Empty;
                int fldIdx = -1;
                string material = string.Empty;
                string phase = string.Empty;
                string conductorCode = string.Empty;
                string conductorType = string.Empty;
                int isubType = -1;
                string subType = string.Empty;

                while (pConductorFeature != null)
                {
                    circId = string.Empty;
                    conductorGlobalid = string.Empty;
                    isubType = -1;
                    operatingVoltage = string.Empty;
                    constructionType = string.Empty;
                    spacing = 0;
                    subType = string.Empty;

                    //CIRCUITID                    
                    if (hshConductorAttributes.ContainsKey("CIRCUITID")) //Does not exist for NeutralConductor 
                    {
                        fldIdx = (int)hshConductorAttributes["CIRCUITID"];
                        if (pConductorFeature.get_Value(fldIdx) != DBNull.Value)
                            circId = pConductorFeature.get_Value(fldIdx).ToString();
                    }

                    //CONSTRUCTIONTYPE 
                    constructionType = PoleLoadConstants.CONSTRUCTION_TYPE_DEFAULT; 
                    if (hshConductorAttributes.ContainsKey("CONSTRUCTIONTYPE"))
                    {
                        fldIdx = (int)hshConductorAttributes["CONSTRUCTIONTYPE"];
                        if (pConductorFeature.get_Value(fldIdx) != DBNull.Value)
                            constructionType = GetFieldValue(pConductorFeature, fldIdx);
                    }

                    //OPERATINGVOLTAGE 
                    if (hshConductorAttributes.ContainsKey("CONSTRUCTIONTYPE"))
                    {
                        fldIdx = (int)hshConductorAttributes["OPERATINGVOLTAGE"];
                        if (pConductorFeature.get_Value(fldIdx) != DBNull.Value)
                        {
                            operatingVoltage = GetFieldValue(pConductorFeature, fldIdx);
                        }
                    }
                    
                    //SPACING
                    if (hshConductorAttributes.ContainsKey("SPACING"))
                    {
                        fldIdx = (int)hshConductorAttributes["SPACING"];
                        if (pConductorFeature.get_Value(fldIdx) != DBNull.Value)
                            spacing = Convert.ToInt32(pConductorFeature.get_Value(fldIdx));
                    }

                    //GLOBALID 
                    fldIdx = (int)hshConductorAttributes["GLOBALID"];
                    conductorGlobalid = pConductorFeature.get_Value(fldIdx).ToString();

                    //SUBTYPECD                   
                    fldIdx = (int)hshConductorAttributes["SUBTYPECD"];
                    if (pConductorFeature.get_Value(fldIdx) != DBNull.Value)
                    {
                        isubType = Convert.ToInt32(pConductorFeature.get_Value(fldIdx).ToString());

                        //Written by Tina to fetch domain value
                        ISubtypes subtypes = (ISubtypes)((IFeatureClass)pConductorFeature.Class);
                        IRowSubtypes rowSubtypes = (IRowSubtypes)pConductorFeature;
                        subType = subtypes.get_SubtypeName(isubType);

                    }

                    Conductor pConductor = new Conductor(
                        pConductorFeature.OID,
                        subType,
                        conductorFCName,
                        conductorGlobalid,
                        circId, 
                        operatingVoltage,
                        constructionType,
                        spacing,
                        (IPolyline)pConductorFeature.ShapeCopy,
                        null);

                    pConductorList.Add(pConductor);
                    pConductorFeature = pFCursor.NextFeature();
                }

                Marshal.FinalReleaseComObject(pFCursor);
            }
            catch (Exception ex)
            {
                Shared.WriteToLogfile("Error in " + MethodBase.GetCurrentMethod() +
                    " details: " + ex.Message);
                throw ex; 
            }
        }


        //internal void ForwardStarTraceDownStream(Int32 JunctionEid, IForwardStarGEN pStar, ref Dictionary<Int32, string> dictForwardTraceQAJunctions,
        //                                                                                    ref Dictionary<Int32, string> dictForwardTraceQAEdges,
        //                                                                                        Dictionary<Int32, string> arcFMTraceResultsJunctions,
        //                                                                                        Dictionary<Int32, string> arcFMTraceResultsEdges,
        //                                                                                        Int32 parentJuncEid,
        //                                                                                        out TraceResults forwardTraceResults
        //                                                                                         )
        //{
        //    forwardTraceResults = new TraceResults();
        //    TraceResults childJunction = new TraceResults();
        //    try
        //    {
        //        string strFeatInfo = null;
        //        int intAdgCount = 0;
        //        //only process if the result is part of ArcFM down stream trace.
        //        if (!arcFMTraceResultsJunctions.ContainsKey(JunctionEid)) return; //If not in ArcFM trace results, do not continue with this junction.
        //        if (dictForwardTraceQAJunctions.ContainsKey(JunctionEid)) return; //If junction already processed, return.

        //        arcFMTraceResultsJunctions.TryGetValue(JunctionEid, out strFeatInfo);
        //        dictForwardTraceQAJunctions.Add(JunctionEid, strFeatInfo);
        //        string[] aryValues = strFeatInfo.Split('/');
        //        string fcName = aryValues[0];
        //        string oid = aryValues[1];
        //        string fcid = aryValues[2];
        //        string subid = aryValues[3];
        //        forwardTraceResults = new TraceResults(fcName, oid, fcid, subid, JunctionEid.ToString(), esriElementType.esriETJunction, parentJuncEid.ToString());

        //        //Determine if the passed junction is a barrier. Don't trace further if it is a barrier.
        //        if (IsSwitchOpen(fcid, oid)) return;

        //        //Use for forward star interface to find adjacent elements.
        //        pStar.FindAdjacent(0, JunctionEid, out intAdgCount);
        //        Int32 adjacentEdgeEid = -1;
        //        bool blnRO = false;
        //        object objAEWV = null;
        //        Int32 adjacentJunctionEid = 0;
        //        int intCount2 = 0;
        //        bool blnAddToparentEdge = false;

        //        //Sort Edges, so that Edges can be processed in Consistent manner.
        //        Dictionary<int, Int32> dictElementIDs = new Dictionary<int, Int32>();
        //        for (int i = 0; i < intAdgCount; i++)
        //        {
        //            //Get adjacent Edge.
        //            pStar.FindAdjacent(i, JunctionEid, out intCount2);
        //            pStar.QueryAdjacentEdge(i, out adjacentEdgeEid, out blnRO, out objAEWV);
        //            dictElementIDs.Add(i, adjacentEdgeEid);
        //        }

        //        //Sort Element-Ids, this makes moving to branches consistent, during tracing.
        //        SortEdgeElementIds(ref dictElementIDs);

        //        TraceResults edgeResult = new TraceResults();
        //        foreach (KeyValuePair<int, Int32> pair in dictElementIDs) //for (int i = 0; i < intAdgCount; i++)
        //        {
        //            blnAddToparentEdge = false;
        //            try
        //            {
        //                //Get adjacent Edge.
        //                pStar.FindAdjacent(pair.Key, JunctionEid, out intCount2);
        //                pStar.QueryAdjacentEdge(pair.Key, out adjacentEdgeEid, out blnRO, out objAEWV);
        //            }
        //            catch (Exception exAdj) { _logger.Error(exAdj); }

        //            //only process if the result is part of ArcFM down stream trace.
        //            if (arcFMTraceResultsEdges.ContainsKey(adjacentEdgeEid))
        //            {
        //                if (!dictForwardTraceQAEdges.ContainsKey(adjacentEdgeEid))
        //                {
        //                    //add  adjacent edge to results.
        //                    strFeatInfo = null;
        //                    arcFMTraceResultsEdges.TryGetValue(adjacentEdgeEid, out strFeatInfo);
        //                    dictForwardTraceQAEdges.Add(adjacentEdgeEid, strFeatInfo);//Add to memory.
        //                    aryValues = strFeatInfo.Split('/');
        //                    fcName = aryValues[0];
        //                    oid = aryValues[1];
        //                    fcid = aryValues[2];
        //                    subid = aryValues[3];
        //                    edgeResult = new TraceResults(fcName, oid, fcid, subid, adjacentEdgeEid.ToString(), esriElementType.esriETEdge, JunctionEid.ToString());
        //                    forwardTraceResults.ChilderenFeatures.Add(edgeResult);
        //                    //this provides edges's From-Junciton information.
        //                    forwardTraceResults.ConnectedElement.Add(edgeResult);
        //                    blnAddToparentEdge = true;
        //                }
        //            }
        //            pStar.FindAdjacent(pair.Key, JunctionEid, out intCount2);
        //            try
        //            {
        //                //Get adjacent Junction.
        //                pStar.QueryAdjacentJunction(pair.Key, out adjacentJunctionEid, out objAEWV);
        //            }
        //            catch (Exception exAdj) { _logger.Error(exAdj); }

        //            //only process if the result is part of ArcFM down stream trace.
        //            if (arcFMTraceResultsJunctions.ContainsKey(adjacentJunctionEid))
        //            {
        //                //Check if this junction has been processed
        //                if (!dictForwardTraceQAJunctions.ContainsKey(adjacentJunctionEid))
        //                {
        //                    //add adjacent junction to results.
        //                    childJunction = new TraceResults();
        //                    //for each adjacnet juntion process it to determine what are the connected elements.
        //                    ForwardStarTraceDownStream(adjacentJunctionEid, pStar, ref dictForwardTraceQAJunctions, ref dictForwardTraceQAEdges, arcFMTraceResultsJunctions,
        //                                                                                                                arcFMTraceResultsEdges, JunctionEid, out childJunction);
        //                    forwardTraceResults.ChilderenFeatures.Add(childJunction);
        //                    if (blnAddToparentEdge)
        //                    {
        //                        //this provides Edges's to Junctions information.
        //                        edgeResult.ConnectedElement.Add(childJunction);
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.Error(ex);
        //    }
        //}



        /// <summary>
        /// Returns the conductors within the searchpolygon 
        /// </summary>
        /// <param name="fcName"></param>
        /// <param name="workspaceName"></param>
        /// <param name="pSearchPolygon"></param>
        //private void GetNearbyConductors( 
        //    IFeatureClass pConductorFC, 
        //    Hashtable hshConductorAttributes, 
        //    IRelationshipClass pConductorInfoRC, 
        //    ITable pConductorInfoTbl, 
        //    Hashtable hshConductorInfoAttributes,
        //    IPolygon pSearchPolygon, 
        //    IProximityOperator pP,
        //    ref List<Conductor> pConductorList)
        //{
        //    try
        //    {
        //        List<ConductorInfo> pConductorInfos = new List<ConductorInfo>();
        //        IDataset pDS = (IDataset)pConductorFC;
        //        string conductorFCName = GetShortDatasetName(pDS.Name);
        //        pDS = (IDataset)pConductorInfoTbl;
        //        string conductorInfoTblName = GetShortDatasetName(pDS.Name);
        //        ISpatialFilter pSF = new SpatialFilterClass();
        //        //*** should screen out ones that are not in service 
        //        //pSF.WhereClause = "status = 5"; 
        //        pSF.Geometry = pSearchPolygon;
        //        pSF.SpatialRel = esriSpatialRelEnum.esriSpatialRelIntersects;
        //        IFeatureCursor pFCursor = pConductorFC.Search(pSF, false);
        //        IFeature pConductorFeature = pFCursor.NextFeature();

        //        //code to check distance between codunctor's to and from point to Pole
        //     /*   double distFrm = 0;
        //        double distTo = 0;

        //        IPoint pFrmPt = ((IPolyline)(pConductorFeature.ShapeCopy)).FromPoint;
        //        IPoint pToPt = ((IPolyline)(pConductorFeature.ShapeCopy)).ToPoint;

        //        pFrmPt.
        //        distFrm = pP.ReturnDistance(((IFeature)pFrmPt).ShapeCopy);
        //        distTo = pP.ReturnDistance(((IFeature)pToPt).ShapeCopy);


        //        if (distFrm <= 12 || distTo <= 12)
        //        {*/
        //            string circId = string.Empty;
        //            string operatingVoltage = string.Empty;
        //            string constructionType = string.Empty;
        //            int spacing = 0;
        //            string conductorGlobalid = string.Empty;
        //            string conInfoGlobalid = string.Empty;
        //            string conductorSize = string.Empty;
        //            string conductorUse = string.Empty;
        //            int conductorCount = 0;
        //            int fldIdx = -1;
        //            string material = string.Empty;
        //            string phase = string.Empty;
        //            string conductorCode = string.Empty;
        //            string conductorType = string.Empty;
        //            int isubType = -1;
        //            string subType = string.Empty;

        //            while (pConductorFeature != null)
        //            {

        //                pConductorInfos.Clear();
        //                ISet pSet = pConductorInfoRC.GetObjectsRelatedToObject(pConductorFeature); 


        //                //***this will not bring the ones with more than 1 conductorinfo 
        //                pSet.Reset();
        //                if (pSet.Count > 1)
        //                    Debug.Print("more than one conductorinfo");
        //                IRow pConductorInfoRow = (IRow)pSet.Next();
        //                if (pConductorInfoRow != null)
        //                {
        //                    conductorSize = string.Empty;
        //                    conductorUse = string.Empty;
        //                    conductorCount = 0;
        //                    material = string.Empty;
        //                    phase = string.Empty;
        //                    conductorType = string.Empty;

        //                    //GLOBALID
        //                    fldIdx = (int)hshConductorInfoAttributes["GLOBALID"];
        //                    conInfoGlobalid = pConductorInfoRow.get_Value(fldIdx).ToString();

        //                    //CONDUCTORSIZE 
        //                    fldIdx = (int)hshConductorInfoAttributes["CONDUCTORSIZE"];
        //                    conductorSize = GetDomainDescriptionFromDomainCode(
        //                        pConductorInfoRow.Fields.get_Field(fldIdx),
        //                        pConductorInfoRow.get_Value(fldIdx));

        //                    //CONDUCTORUSE 
        //                    fldIdx = (int)hshConductorInfoAttributes["CONDUCTORUSE"];
        //                    conductorUse = GetDomainDescriptionFromDomainCode(
        //                        pConductorInfoRow.Fields.get_Field(fldIdx),
        //                        pConductorInfoRow.get_Value(fldIdx));

        //                    //CONDUCTORCOUNT
        //                    fldIdx = (int)hshConductorInfoAttributes["CONDUCTORCOUNT"];
        //                    if (pConductorInfoRow.get_Value(fldIdx) != DBNull.Value)
        //                        conductorCount = Convert.ToInt32(pConductorInfoRow.get_Value(fldIdx));

        //                    //CONDUCTORTYPE Added by Tina
        //                    fldIdx = (int)hshConductorInfoAttributes["CONDUCTORTYPE"];
        //                    if (pConductorInfoRow.get_Value(fldIdx) != DBNull.Value)
        //                        conductorType = pConductorInfoRow.get_Value(fldIdx).ToString();

        //                    //MATERIAL 
        //                    fldIdx = (int)hshConductorInfoAttributes["MATERIAL"];
        //                    material = GetDomainDescriptionFromDomainCode(
        //                        pConductorInfoRow.Fields.get_Field(fldIdx),
        //                        pConductorInfoRow.get_Value(fldIdx));

        //                    //PHASEDESIGNATION 
        //                    fldIdx = (int)hshConductorInfoAttributes["PHASEDESIGNATION"];
        //                    phase = GetDomainDescriptionFromDomainCode(
        //                        pConductorInfoRow.Fields.get_Field(fldIdx),
        //                        pConductorInfoRow.get_Value(fldIdx));

        //                    ConductorInfo pConInfo = new ConductorInfo(
        //                        pConductorInfoRow.OID,
        //                        conductorInfoTblName,
        //                        conInfoGlobalid,
        //                        conductorSize,
        //                        conductorUse,
        //                        conductorCount,
        //                        conductorType,
        //                        material,
        //                        phase);

        //                    //Add the conductorinfo 
        //                    pConductorInfos.Add(pConInfo);

        //                }
        //                Marshal.FinalReleaseComObject(pSet);

        //                circId = string.Empty;
        //                conductorGlobalid = string.Empty;
        //                isubType = -1;
        //                operatingVoltage = string.Empty;
        //                constructionType = string.Empty;
        //                spacing = 0;
        //                subType = string.Empty;

        //                //CIRCUITID 
        //                fldIdx = (int)hshConductorAttributes["CIRCUITID"];
        //                if (pConductorFeature.get_Value(fldIdx) != DBNull.Value)
        //                    circId = pConductorFeature.get_Value(fldIdx).ToString();

        //                //CONSTRUCTIONTYPE
        //                if (hshConductorAttributes.ContainsKey("CONSTRUCTIONTYPE"))
        //                {
        //                    String strConstType = null;
        //                    fldIdx = (int)hshConductorAttributes["CONSTRUCTIONTYPE"];
        //                    if (pConductorFeature.get_Value(fldIdx) != DBNull.Value)
        //                        strConstType = pConductorFeature.get_Value(fldIdx).ToString();


        //                    //Written by Tina to fetch domain value
        //                    IDomain objST;
        //                    IField pfild;

        //                    pfild = pConductorFeature.Fields.get_Field(fldIdx);
        //                    objST = pfild.Domain;

        //                    ICodedValueDomain pCVDomain = (ICodedValueDomain)objST;
        //                    for (int x = 0; x < pCVDomain.CodeCount; x++)
        //                    {
        //                        if (strConstType == pCVDomain.get_Value(x).ToString())
        //                        {
        //                            constructionType = pCVDomain.get_Name(x).ToString();
        //                            break;
        //                        }
        //                    }

        //                }

        //                //OPERATINGVOLTAGE 
        //                fldIdx = (int)hshConductorAttributes["OPERATINGVOLTAGE"];
        //                if (pConductorFeature.get_Value(fldIdx) != DBNull.Value)
        //                    operatingVoltage = pConductorFeature.get_Value(fldIdx).ToString();

        //                //SPACING
        //                if (hshConductorAttributes.ContainsKey("SPACING"))
        //                {
        //                    fldIdx = (int)hshConductorAttributes["SPACING"];
        //                    if (pConductorFeature.get_Value(fldIdx) != DBNull.Value)
        //                        spacing = Convert.ToInt32(pConductorFeature.get_Value(fldIdx));
        //                }

        //                //GLOBALID 
        //                fldIdx = (int)hshConductorAttributes["GLOBALID"];
        //                conductorGlobalid = pConductorFeature.get_Value(fldIdx).ToString();

        //                //SUBTYPECD
        //                fldIdx = (int)hshConductorAttributes["SUBTYPECD"];
        //                if (pConductorFeature.get_Value(fldIdx) != DBNull.Value)
        //                {
        //                    isubType = Convert.ToInt32(pConductorFeature.get_Value(fldIdx).ToString());

        //                    //Written by Tina to fetch domain value
        //                    ISubtypes subtypes = (ISubtypes)((IFeatureClass)pConductorFeature.Class);
        //                    IRowSubtypes rowSubtypes = (IRowSubtypes)pConductorFeature;
        //                    subType = subtypes.get_SubtypeName(isubType);

        //                }

        //                Conductor pConductor = new Conductor(
        //                    pConductorFeature.OID,
        //                    subType,
        //                    conductorFCName,
        //                    conductorGlobalid,
        //                    circId,
        //                    operatingVoltage,
        //                    constructionType,
        //                    spacing,
        //                    (IPolyline)pConductorFeature.ShapeCopy,
        //                    pConductorInfos);
        //                pConductorList.Add(pConductor);
        //                pConductorFeature = pFCursor.NextFeature();
        //            }
        //       // }
        //        Marshal.FinalReleaseComObject(pFCursor);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception("Error encountered in PopulateNearbyConductors: " +
        //        ex.Message);
        //    }
        //}





        /// <summary>
        /// Returns the conductors within the searchpolygon 
        /// </summary>
        /// <param name="fcName"></param>
        /// <param name="workspaceName"></param>
        /// <param name="pSearchPolygon"></param>
        //private List<Pole> GetNearbyPoles(
        //    IFeatureClass pPoleFC,
        //    IPolygon pSearchPolygon)
        //{
        //    try
        //    {
        //        List<Pole> pPoleList = new List<Pole>();
        //        ISpatialFilter pSF = new SpatialFilterClass();
        //        pSF.Geometry = pSearchPolygon;
        //        pSF.SpatialRel = esriSpatialRelEnum.esriSpatialRelIntersects;
        //        IFeatureCursor pFCursor = pPoleFC.Search(pSF, false);
        //        IFeature pPoleFeature = pFCursor.NextFeature();

        //        string globalid = string.Empty;
        //        int fldIdx = -1;
        //        fldIdx = pPoleFC.Fields.FindField("globalid");

        //        while (pPoleFeature != null)
        //        {
        //            globalid = pPoleFeature.get_Value(fldIdx).ToString();
        //            Pole pPole = new Pole(globalid, (IPoint)pPoleFeature.ShapeCopy);
        //            pPoleList.Add(pPole);
        //            pPoleFeature = pFCursor.NextFeature();
        //        }
        //        Marshal.FinalReleaseComObject(pFCursor);

        //        return pPoleList;
        //    }
        //    catch (Exception ex)
        //    {
        //        Shared.WriteToLogfile("Error in " + MethodBase.GetCurrentMethod() +
        //            " details: " + ex.Message);
        //        throw new Exception("Error finding nearby poles");
        //    }
        //} 
    }

    public class Conductor
    {
        //Property storage variables 
        private int _oid;
        private string _fcName;
        private string _guid;
        private string _circId;
        private string _operatingVoltage;
        private string _constructionType;
        private int _spacing;
        private string _subtype; //changed by Tina
        private IPolyline _shape;
        List<ConductorInfo> _pConductorInfos;

        public Conductor(
            int oid,
            string subtype, //changed by Tina
            string fcName,
            string guid,
            string circId, 
            string operatingVoltage,
            string constructionType,
            int spacing,
            IPolyline polyline,
            List<ConductorInfo> pConductorInfos)
        {
            _oid = oid;
            _subtype = subtype;
            _fcName = fcName;
            _guid = guid;
            _circId = circId;
            _operatingVoltage = operatingVoltage;
            _constructionType = constructionType.ToUpper();
            _spacing = spacing;
            _shape = polyline;
            _pConductorInfos = pConductorInfos;
        }

        public int OID
        {
            get { return _oid; }
        }

        public string Subtype
        {
            get { return _subtype; }
        }

        public string FCName
        {
            get { return _fcName; }
        }

        public string CircId
        {
            get { return _circId; }
        }

        public string ConstructionType
        {
            get { return _constructionType; }
        }

        public string OperatingVoltage
        {
            get { return _operatingVoltage; }
        }

        public int Spacing
        {
            get { return _spacing; }
        }

        public string GUID
        {
            get { return _guid; }
        }

        public IPolyline Shape
        {
            get { return _shape; }
        }

        public List<ConductorInfo> ConductorInfos
        {
            get { return _pConductorInfos; }
            set { _pConductorInfos =  value; }
        }
    }

    public class Pole
    {
        //Property storage variables 
        private string _guid;
        private IPoint _shape;

        public Pole(
            string guid,
            IPoint pPoint)
        {
            _guid = guid;
            _shape = pPoint;
        }

        public string GUID
        {
            get { return _guid; }
        }

        public IPoint Shape
        {
            get { return _shape; }
        }
    }

    public class RelatedDevice
    {
        //Property storage variables 
        private string _deviceType;
        private int _OId;
        //private string _circId; 
        private IPoint _shape;

        public RelatedDevice(
            string deviceType,
            IPoint pPoint,
            int OId)
        {
            _deviceType = deviceType.ToUpper();
            _shape = pPoint;
            _OId = OId;
            //_circId = circId; 
        }

        public string DeviceType
        {
            get { return _deviceType; }
        }

        public IPoint Shape
        {
            get { return _shape; }
        }

        public int OId
        {
            get { return _OId; }
        }

        //public string CircuitId
        //{
        //    get { return _circId; }
        //}
    }

    public class ConductorInfo
    {
        //Property storage variables 
        private int _oid;
        private string _guid;
        private string _tableName;
        private int _conductorCode;
        private string _conductorSize;
        private string _conductorUse;
        private int _conductorCount;
        private string _conductorType;//added by Tina
        private string _material;
        private string _phase;

        public ConductorInfo(
            int oid,
            string tableName,
            string guid, 
            int conductorCode, 
            string conductorSize,
            string conductorUse,
            int conductorCount,
            string conductorType,//added by Tina
            string material,
            string phase)
        {
            _oid = oid;
            _tableName = tableName;
            _guid = guid;
            _conductorCode = conductorCode; 
            _conductorSize = conductorSize;
            _conductorUse = conductorUse;
            _conductorCount = conductorCount;
            _conductorType = conductorType;//added by Tina
            _material = material;
            _phase = phase;
        }

        public int OID
        {
            get { return _oid; }
        }

        public string GUID
        {
            get { return _guid; }
        }

        public int ConductorCode
        {
            get { return _conductorCode; }
        }

        public string TableName
        {
            get { return _tableName; }
        }

        public string ConductorSize
        {
            get { return _conductorSize; }
        }

        public int ConductorCount
        {
            get { return _conductorCount; }
        }

        //Added by Tina
        public string ConductorType
        {
            get { return _conductorType; }
        }

        public string ConductorUse
        {
            get { return _conductorUse; }
        }

        public string Material
        {
            get { return _material; }
        }

        public string Phase
        {
            get { return _phase; }
        }
    }

    public class ConductorCode
    {
        //Property storage variables 
        private int _code;
        private string _size;
        private string _material;

        public ConductorCode(
            int code,
            string size,
            string material)
        {
            _material = material;
            _size = size;
            _code = code;
        }

        public string Material
        {
            get { return _material; }
        }
        public string Size
        {
            get { return _size; }
        }
        public int Code
        {
            get { return _code; }
        }
    }


    public class Span
    {

        //Property storage variables 
        private string _spanGUID;
        private double _spanAngle;
        private double _length;
        private string _conductorClass;
        private string _conductorSubtype; //changed by Tina
        private string _conductorGUID;
        private string _fromPoleGUID;
        private double _fromPoleRL;
        private string _toPoleGUID;
        private double _toPoleRL;
        private string _circId;
        private IPolyline _spanPolyline;
        private IPolyline _spanTrunk;

        //private Double _poleElevation; //Addded by Imran

        //public Span(
        //    int index,
        //    double spanAngle, 
        //    double length, 
        //    string conductorClass,
        //    string conductorSubtype,//changed by Tina
        //    string conductorGUID, 
        //    string fromPoleGUID, 
        //    double fromPoleRL, 
        //    string toPoleGUID, 
        //    double toPoleRL, 
        //    string circId,
        //    Double poleElevation//Added by Imran
        //    )
        //{
        //    _index = index;
        //    _spanAngle = spanAngle; 
        //    _length = length;
        //    _conductorClass = conductorClass;
        //    _conductorSubtype = conductorSubtype;
        //    _conductorGUID = conductorGUID; 
        //    _fromPoleGUID = fromPoleGUID; 
        //    _fromPoleRL = fromPoleRL; 
        //    _toPoleGUID = toPoleGUID;
        //    _toPoleRL = toPoleRL;
        //    _circId = circId;
        //    _poleElevation = poleElevation;//Added by Imran
        //}

        public Span(
            string spanGUID,
            IPolyline spanPolyline,
            IPolyline spanTrunk,
            double spanAngle,
            double length,
            string conductorClass,
            string conductorSubtype,//changed by Tina
            string conductorGUID,
            string fromPoleGUID,
            double fromPoleRL,
            string toPoleGUID,
            double toPoleRL,
            string circId
            )
        {
            _spanGUID = spanGUID;
            _spanPolyline = spanPolyline;
            _spanTrunk = spanTrunk;
            _spanAngle = spanAngle;
            _length = length;
            _conductorClass = conductorClass;
            _conductorSubtype = conductorSubtype;
            _conductorGUID = conductorGUID;
            _fromPoleGUID = fromPoleGUID;
            _fromPoleRL = fromPoleRL;
            _toPoleGUID = toPoleGUID;
            _toPoleRL = toPoleRL;
            _circId = circId;
        }

        public string SpanGUID
        {
            get { return _spanGUID; }
        }

        public IPolyline SpanPolyline
        {
            get { return _spanPolyline; }
        }

        public IPolyline SpanTrunk
        {
            get { return _spanTrunk; }
        }

        public double SpanAngle
        {
            get { return _spanAngle; }
        }

        public double Length
        {
            get { return _length; }
        }

        public string ConductorClass
        {
            get { return _conductorClass; }
        }

        public string ConductorSubtype//changed by Tina
        {
            get { return _conductorSubtype; }
        }

        public string ConductorGUID
        {
            get { return _conductorGUID; }
        }

        public string FromPoleGUID
        {
            get { return _fromPoleGUID; }
        }

        public double FromPoleRL
        {
            get { return _fromPoleRL; }
        }

        public string ToPoleGUID
        {
            get { return _toPoleGUID; }
        }

        public double ToPoleRL
        {
            get { return _toPoleRL; }
        }

        public string CircuitId
        {
            get { return _circId; }
        }

        //public Double PoleElevation
        //{
        //    get { return _poleElevation; }
        //}
    }
}
