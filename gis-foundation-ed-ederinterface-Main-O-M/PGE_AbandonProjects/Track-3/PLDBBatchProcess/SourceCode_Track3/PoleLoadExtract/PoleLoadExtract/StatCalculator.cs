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

using ESRI.ArcGIS.ArcMapUI;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.DataSourcesRaster; 
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Framework;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;


namespace PoleLoadingStats
{
    public enum ExtractDatasetType
    {
        etTable = 0,
        etFeatureclass = 1,
        etAnnoFeatureclass = 2,
        etJoinFeatureclass = 3,
        etRelationshipclass = 4
    }

    class StatCalculator
    {
              
        IApplication m_app = null;

        Hashtable _hshAllAttributes = null;
        Hashtable _hshAllDomains = null;
        IFeatureClass m_SnowLoadingFC = null;
        IFeatureClass m_PopDensityFC = null;
        IFeatureClass m_ClimateZoneFC = null;
        IFeatureClass m_CorrosionAreaFC = null; 
        IFeatureClass m_RCZ_FC = null;

        IFeatureClass m_SupportStructureFC = null;
        IFeatureClass m_PriOHConductorFC = null;
        IFeatureClass m_SecOHConductorFC = null;
        IFeatureClass m_CapBankFC = null;
        IFeatureClass m_TransformerFC = null;
        IFeatureClass m_AnchorGuyFC = null;
        IFeatureClass m_VoltageRegulatorFC = null;
        IFeatureClass m_StreetlightFC = null;
        IFeatureClass m_FuseFC = null;
        IFeatureClass m_SwitchFC = null;
        IFeatureClass m_DPDFC = null;

        ITable m_PriOHConductorInfoTbl = null;
        ITable m_SecOHConductorInfoTbl = null;
        ITable m_TransformerUnitTbl = null;
        ITable m_JointOwnerTbl = null;

        IRelationshipClass m_PriOHConductorInfoRC = null; 
        IRelationshipClass m_SecOHConductorInfoRC = null;         
        IRelationshipClass m_TransformerRC = null;        
        IRelationshipClass m_TransformerUnitRC = null;        
        IRelationshipClass m_AnchorGuyRC = null;        
        IRelationshipClass m_CapBankRC = null;        
        IRelationshipClass m_DPDRC = null;
        IRelationshipClass m_StreetlightRC = null;        
        IRelationshipClass m_JointOwnerRC = null; 
        IRelationshipClass m_FuseRC = null;        
        IRelationshipClass m_SwitchRC = null;
        IRelationshipClass m_OpenPointRC = null;
        IRelationshipClass m_PrimaryRiserRC = null;
        IRelationshipClass m_PrimaryMeterRC = null;
        IRelationshipClass m_StepDownRC = null;
        IRelationshipClass m_FaultIndicatorRC = null;
        IRelationshipClass m_VoltageRegulatorRC = null;

        public StatCalculator(IApplication pApp)
        {
            m_app = pApp; 
        }

        public void Main()
        {
            try
            {

                //Load up the workspaces 
                Shared.LoadConfigurationSettings();
                Shared.InitializeLogfile();
                Shared.InitializeSpanCSVfile();
                Shared.InitializeFullDataCSVfile();
                Shared.InitializeStaticCSVfile();
                Shared.WriteToLogfile("Entering Main");
                Shared.WriteToLogfile("connecting to workspaces");
                Shared.LoadWorkspaces();
                Shared.WriteToLogfile("======================================================");

                //int poleOId = 30044026;
                //ProcessPole(poleOId); 
                InitializeData();
                ProcessPoles(); 



                //Have a config file that has a connections section 
                //  source connection 
                //  dest connection 
                //ExtractPoles();

                // Calculate Pole Spans
                //CalculatePoleSpans();


                //WriteToLogfile("ExportPoles completed");

                //WriteOutStaticData(); 
                //WriteOutLandbaseData();
                 
                                

                //Calculate wind zone info 
                //IPoint pPoint = null; 
                //int pHeight = GetHeightAtPoint(pPoint); 


                //Calculate fire zone attributes                 

                //Calculate angles for each of the spans 

                //Populate attribute to indicate presence of joint owners 


                //Fire zone
                //CreateLocalOfficePolygonLayer(); 

            }
            catch (Exception ex)
            {
                throw new Exception("Error encountered in Main"); 
            }
        }

        private double GetHeightAtPoint(IPoint pPoint, IIdentify pIdentify)
        {
            try
            {                
                
                int rasterValue = -1; 

                //Get RasterIdentifyObject on that point 
                IArray pIDArray = pIdentify.Identify(pPoint);
                if (pIDArray != null)
                {
                    IRasterIdentifyObj pRIDObj = (IRasterIdentifyObj)pIDArray.get_Element(0);
                    rasterValue = Convert.ToInt32(pRIDObj.Name); 
                }

                return rasterValue; 
            }
            catch (Exception ex)
            {
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
        //                                IPolygon pBufferPoly = (IPolygon)kTopo.Buffer(Shared.RayBuffer);
        //                                //IGeometry pGeomPoly = kTopo.Buffer(Shared.RayBuffer);

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
        //                        IPolygon pBufferPoly = (IPolygon)kTopo.Buffer(Shared.RayBuffer);
        //                        //IGeometry pGeomPoly = kTopo.Buffer(Shared.RayBuffer);

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





        private static IPolygon GetBufferPolygon(IGeometry pGeometry, int bufferDist)
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
                throw new Exception("Error generating buffer polygon"); 
            }
        }
        /// <summary>
        /// Writes out the header row for the CSV containing 
        /// static attributes of supportstructure derived from 
        /// the GIS 
        /// </summary>
        private void WriteOutStaticDataHeader()
        {

            try
            {
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
                throw new Exception("Error generating static data csv header");
            }
        }

        private void WriteOutStaticData(IFeature pRefPoleFeature)
        {

            try
            {
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
                Shared.WriteToLogfile("Entering error handler for " + "WriteOutStaticData" + ex.Message);
                throw new Exception("Error encountered in CalculatePoleSpans: " +
                ex.Message);
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
                Shared.WriteToLogfile("Entering error handler for " + "GetPointInPolygonLandbaseValue" + ex.Message);
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
                Shared.WriteToLogfile("Entering error handler for " + "GetSpanAngleFromAdjacentPoles" + ex.Message);
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
                Shared.WriteToLogfile("Entering error handler for " + "GetSpanLength" + ex.Message);
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
                Shared.WriteToLogfile("Entering error handler for " + "GetSpanPolyline" + ex.Message);
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
                //Imran  - Added Pole elevation
                Shared.WriteToSpanCSV("FROM_POLE_GUID,FROM_POLE_RL,TO_POLE_GUID,TO_POLE_RL,CONDUCTOR_GUID,CONDUCTOR_SPAN_LENGTH,CONDUCTOR_CIRCUIT,CONDUCTOR_CLASS,CONDUCTOR_SUBTYPE,CONDUCTOR_ANGLE"); 
            }
            catch (Exception ex)
            {
                Shared.WriteToLogfile("Entering error handler for " + "WriteSpanCSVHeader" + ex.Message);
                throw new Exception("Error encountered in WriteSpanCSVHeader: " +
                ex.Message);
            }
        }

        private void WriteFullDataCSVHeader()
        {
            try
            {
                //Change by Tina to replace ATTRIBUTENAME to ATTRIBUTE_NM and ATTRIBUTEVALUE to ATTRIBUTE_VAL
                Shared.WriteToFullDataCSV("POLEGUID,PARENTGUID,DEVICEGUID,DEVICETYPE,ATTRIBUTE_NM,ATTRIBUTE_VAL");
            }
            catch (Exception ex)
            {
                Shared.WriteToLogfile("Entering error handler for " + "WriteSpanCSVHeader" + ex.Message);
                throw new Exception("Error encountered in WriteSpanCSVHeader: " +
                ex.Message);
            }
        }

        private void WriteSpanListToCSV(List<Span> pSpanList)
        {
            try
            {
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
                Shared.WriteToLogfile("Entering error handler for " + "SaveSpanInformation" + ex.Message);
                throw new Exception("Error encountered in SpanSpanInformation: " +
                ex.Message);
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
                Shared.WriteToLogfile("Entering error handler for " + "SaveSpanInformation" + ex.Message);
                throw new Exception("Error encountered in SpanSpanInformation: " +
                ex.Message);
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
                        break;
                    case "JOINTOWNER":
                        if (pOC.Fields.FindField("GLOBALID") != -1)
                            hshAttributes.Add("GLOBALID", pOC.Fields.FindField("GLOBALID"));
                        if (pOC.Fields.FindField("JONAME") != -1)
                            hshAttributes.Add("JONAME", pOC.Fields.FindField("JONAME"));
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
                        break;
                    case "STREETLIGHT":
                        if (pOC.Fields.FindField("GLOBALID") != -1)
                            hshAttributes.Add("GLOBALID", pOC.Fields.FindField("GLOBALID"));
                        if (pOC.Fields.FindField("ELEVATION") != -1)
                            hshAttributes.Add("ELEVATION", pOC.Fields.FindField("ELEVATION"));
                        if (pOC.Fields.FindField("OPERATINGVOLTAGE") != -1)
                            hshAttributes.Add("OPERATINGVOLTAGE", pOC.Fields.FindField("OPERATINGVOLTAGE"));
                        break;
                    case "SWITCH":
                        if (pOC.Fields.FindField("GLOBALID") != -1)
                            hshAttributes.Add("GLOBALID", pOC.Fields.FindField("GLOBALID"));
                        if (pOC.Fields.FindField("NUMBEROFPHASES") != -1)
                            hshAttributes.Add("NUMBEROFPHASES", pOC.Fields.FindField("NUMBEROFPHASES"));
                        if (pOC.Fields.FindField("OPERATINGVOLTAGE") != -1)
                            hshAttributes.Add("OPERATINGVOLTAGE", pOC.Fields.FindField("OPERATINGVOLTAGE"));
                        break;
                    case "TRANSFORMER":
                        if (pOC.Fields.FindField("GLOBALID") != -1)
                            hshAttributes.Add("GLOBALID", pOC.Fields.FindField("GLOBALID"));
                        if (pOC.Fields.FindField("UNITCOUNT") != -1)
                            hshAttributes.Add("UNITCOUNT", pOC.Fields.FindField("UNITCOUNT"));
                        //if (pOC.Fields.FindField("PHASEDESIGNATION") != -1)
                        //    hshAttributes.Add("PHASEDESIGNATION", pOC.Fields.FindField("PHASEDESIGNATION"));

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
                    case "PRIOHCONDUCTORINFO":
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

                if (subtypes.HasSubtype)
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
                else
                {
                    pDomain = pRow.Fields.get_Field(fieldIdx).Domain; 
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
                throw new Exception("Error encountered in GetFieldIndexForUpdate: " +
                ex.Message);
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
                throw new Exception("Error encountered in CalculatePoleSpans: " +
                ex.Message);
            }
        }

        private static IFeatureIndex CreateFeatureIndex(IFeatureClass pFeatureClass)
        {
            IFeatureIndex fi = new FeatureIndexClass();
            try
            {

                fi.FeatureClass = pFeatureClass;
                //create the index
                fi.Index(null, ((IGeoDataset)pFeatureClass).Extent);
            }
            catch (Exception ex)
            {
                fi = null;
            }
            return fi;
        }

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


        private IFeature FindAdjacentPole( 
            IPoint pExistingPolePoint,
            IPolygon pRayPolygon, 
            IFeatureClass pSupportStructureFC, 
            int oidOfExistingPole,
            string circuitid//,
            //IFeatureIndex fi,
            //IGeometry pGp
            )
        {
            ITopologicalOperator kTopoOH = null;
            IPolygon pSearchPolyOH = null;
            //string sFound = "NO";
            //int fid = 0;
           // double distance = 0.0;
            
            try
            {
                //code to replace nearest pole search with feature index logic to increase the performance
               // IFeatureIndex ifxidx = CreateFeatureIndex(pSupportStructureFC);
                
              /*  ((IIndexQuery)fi).NearestFeature(pGp, out fid, out distance);
                                
                IFeature pAdjacentPole = null;
                IFeature pAdjacentPoleNull = null;

                if(distance > 0)
                    pAdjacentPole = pSupportStructureFC.GetFeature(fid);


                pAdjacentPole = pSupportStructureFC.GetFeature(fid);
               */

                IProximityOperator pPO = (IProximityOperator)pExistingPolePoint;
                ISpatialFilter pSF = new SpatialFilterClass();
                pSF.Geometry = pRayPolygon;
                pSF.SpatialRel = esriSpatialRelEnum.esriSpatialRelIntersects; 
                IFeatureCursor pFCursor = pSupportStructureFC.Search(pSF, false);
                IFeature pPoleFeature = pFCursor.NextFeature();
                double distAway = 0;
                double closestDistAway = 5000;
                IFeature pAdjacentPole = null;
                IFeature pAdjacentPoleNull = null;


                while (pPoleFeature != null)
                {
                    if (pPoleFeature.OID != oidOfExistingPole)
                    {
                        distAway = pPO.ReturnDistance(pPoleFeature.ShapeCopy);
                        if (distAway < closestDistAway)
                        {
                            pAdjacentPole = pPoleFeature;
                            closestDistAway = distAway; 
                        }
                    }
                    pPoleFeature = pFCursor.NextFeature();
                }
                  

                //if ((pAdjacentPole != null) && distance <= 5000)
                  if (pAdjacentPole != null)
                {
                    Debug.Print("found adjacent pole at distance: " + closestDistAway.ToString());  
                    //Debug.Print("found adjacent pole at distance: " + distance.ToString());  


                    /*     //Circuit match logic
                       //Search in PRIOHCONDUCTOR Feature class
                       IFeatureWorkspace pFileGDB_FWS_POH = (IFeatureWorkspace)Shared.GetWorkspaceByName("output");
                       IFeatureWorkspace pFWS_POH = (IFeatureWorkspace)Shared.GetWorkspaceByName("electric");
                       IFeatureClass pPriOHConductor = pFWS_POH.OpenFeatureClass("edgis.priohconductor");

                       ISpatialFilter pSF_POH = new SpatialFilterClass();
                       kTopoOH = (ITopologicalOperator)pAdjacentPole.ShapeCopy;
                       pSearchPolyOH = (IPolygon)kTopoOH.Buffer(Shared.PoleBuffer);
                       pSF_POH.Geometry = pSearchPolyOH;
                       pSF_POH.SpatialRel = esriSpatialRelEnum.esriSpatialRelIntersects;
                       pSF_POH.GeometryField = "SHAPE";
                       IFeatureCursor pFCursorPOH = pPriOHConductor.Search(pSF_POH, false);

                       IFeature pPOHFeature = pFCursorPOH.NextFeature();

                       while (pPOHFeature != null)
                       {
                           if (circuitid == pPOHFeature.get_Value(pPOHFeature.Fields.FindField("CIRCUITID")).ToString())
                               sFound = "YES";
                           pPOHFeature = pFCursorPOH.NextFeature();
                       }
                       Marshal.FinalReleaseComObject(pFCursorPOH); 

                       //Search in SECOHCONDUCTOR Feature class
                       if (sFound == "NO")
                       {
                           IFeatureWorkspace pFileGDB_FWS_SOH = (IFeatureWorkspace)Shared.GetWorkspaceByName("output");
                           IFeatureWorkspace pFWS_SOH = (IFeatureWorkspace)Shared.GetWorkspaceByName("electric");
                           IFeatureClass pSecOHConductor = pFWS_SOH.OpenFeatureClass("edgis.secohconductor");

                           ISpatialFilter pSF_SOH = new SpatialFilterClass();
                           kTopoOH = (ITopologicalOperator)pAdjacentPole.ShapeCopy;
                           pSearchPolyOH = (IPolygon)kTopoOH.Buffer(Shared.PoleBuffer);
                           pSF_POH.Geometry = pSearchPolyOH;
                           pSF_POH.SpatialRel = esriSpatialRelEnum.esriSpatialRelIntersects;
                           pSF_POH.GeometryField = "SHAPE";
                           IFeatureCursor pFCursorSOH = pSecOHConductor.Search(pSF_POH, false);

                           IFeature pSOHFeature = pFCursorSOH.NextFeature();

                           while (pSOHFeature != null)
                           {
                               if (circuitid == pSOHFeature.get_Value(pSOHFeature.Fields.FindField("CIRCUITID")).ToString())
                                   sFound = "YES";
                               pSOHFeature = pFCursorSOH.NextFeature();
                           }
                           Marshal.FinalReleaseComObject(pFCursorSOH); 
                  

                       } */

                }

               /* if (sFound == "NO")
                {
                    Marshal.FinalReleaseComObject(pFCursor);
                    return pAdjacentPoleNull;
                }
                else
                {*/
                    Marshal.FinalReleaseComObject(pFCursor);
                    return pAdjacentPole;
              //  }
            }
            catch (Exception ex)
            {
                throw new Exception("Error encountered in FindAdjacentPole: " +
                ex.Message);
            }
        }

        private List<RelatedDevice> GetRelatedDevices(
            IFeature pPoleFeature,
            IRelationshipClass pRelatedDeviceRC, 
            ref List<RelatedDevice> pRelatedDevices)
        {
            try
            {
                ISet pSet = null;
                IRow pDeviceRow = null;
                string relDeviceFCName = string.Empty;
                IPoint pDevicePoint = null; 
                                    
                pSet = pRelatedDeviceRC.GetObjectsRelatedToObject((IObject)pPoleFeature);
                pDeviceRow = (IRow)pSet.Next();
                while (pDeviceRow != null)
                {
                    pDevicePoint = null; 
                    relDeviceFCName = GetShortDatasetName(((IDataset)pDeviceRow.Table).Name.ToUpper());
                    if (pDeviceRow is IFeature)
                        pDevicePoint = (IPoint)((IFeature)pDeviceRow).ShapeCopy; 
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
                throw new Exception("Error returning the related devices"); 
            }
        }

        private void InitializeData()
        {
            try
            {
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

                //Device Featureclasses

                m_SupportStructureFC = pElectricFWS.OpenFeatureClass("edgis.supportstructure");
                CacheAttributesForOC(m_SupportStructureFC);
                //CacheDomains(m_SupportStructureFC);

                m_PriOHConductorFC = pElectricFWS.OpenFeatureClass("edgis.priohconductor");
                CacheAttributesForOC(m_PriOHConductorFC);
                //CacheDomains(m_PriOHConductorFC);

                m_PriOHConductorInfoTbl = pElectricFWS.OpenTable("edgis.priohconductorinfo");
                CacheAttributesForOC((IObjectClass)m_PriOHConductorInfoTbl);
                //CacheDomains((IObjectClass)m_PriOHConductorInfoTbl);

                m_SecOHConductorFC = pElectricFWS.OpenFeatureClass("edgis.secohconductor");
                CacheAttributesForOC(m_SecOHConductorFC);
                //CacheDomains((IObjectClass)m_SecOHConductorFC);

                m_SecOHConductorInfoTbl = pElectricFWS.OpenTable("edgis.secohconductorinfo");
                CacheAttributesForOC((IObjectClass)m_SecOHConductorInfoTbl);
                //CacheDomains((IObjectClass)m_SecOHConductorInfoTbl);

                m_TransformerFC = pElectricFWS.OpenFeatureClass("edgis.transformer");
                CacheAttributesForOC(m_TransformerFC);
                //CacheDomains((IObjectClass)m_TransformerFC);

                m_TransformerUnitTbl = pElectricFWS.OpenTable("edgis.transformerunit");
                CacheAttributesForOC((IObjectClass)m_TransformerUnitTbl);
                //CacheDomains((IObjectClass)m_TransformerUnitTbl);

                m_AnchorGuyFC = pElectricFWS.OpenFeatureClass("edgis.anchorguy");
                CacheAttributesForOC((IObjectClass)m_AnchorGuyFC);
                //CacheDomains((IObjectClass)m_AnchorGuyFC);

                m_CapBankFC = pElectricFWS.OpenFeatureClass("edgis.capacitorbank");
                CacheAttributesForOC((IObjectClass)m_CapBankFC);
                //CacheDomains((IObjectClass)m_CapBankFC);

                m_DPDFC = pElectricFWS.OpenFeatureClass("edgis.dynamicprotectivedevice");
                CacheAttributesForOC((IObjectClass)m_DPDFC);
                //CacheDomains((IObjectClass)m_DPDFC);

                m_JointOwnerTbl = pElectricFWS.OpenTable("edgis.jointowner");
                CacheAttributesForOC((IObjectClass)m_JointOwnerTbl);
                //CacheDomains((IObjectClass)m_JointOwnerTbl);

                m_StreetlightFC = pElectricFWS.OpenFeatureClass("edgis.streetlight");
                CacheAttributesForOC((IObjectClass)m_StreetlightFC);
                //CacheDomains((IObjectClass)m_StreetlightFC);

                m_FuseFC = pElectricFWS.OpenFeatureClass("edgis.Fuse");
                CacheAttributesForOC((IObjectClass)m_FuseFC);
                //CacheDomains((IObjectClass)m_FuseFC);

                m_SwitchFC = pElectricFWS.OpenFeatureClass("edgis.Switch");
                CacheAttributesForOC((IObjectClass)m_SwitchFC);
                //CacheDomains((IObjectClass)m_SwitchFC);

                m_VoltageRegulatorFC = pElectricFWS.OpenFeatureClass("edgis.VoltageRegulator");
                CacheAttributesForOC((IObjectClass)m_VoltageRegulatorFC);
                //CacheDomains((IObjectClass)m_VoltageRegulatorFC);


                //Relationshipclasses 
                m_PriOHConductorInfoRC = pElectricFWS.OpenRelationshipClass("EDGIS.PriOHConductor_PriOHConductorInfo"); 
                m_SecOHConductorInfoRC = pElectricFWS.OpenRelationshipClass("EDGIS.SecOHConductor_SecOHConductorInfo");
                m_TransformerRC = pElectricFWS.OpenRelationshipClass("EDGIS.SupportStruct_Transformer");                
                m_TransformerUnitRC = pElectricFWS.OpenRelationshipClass("EDGIS.Transformer_TransformerUnit");                
                m_AnchorGuyRC = pElectricFWS.OpenRelationshipClass("EDGIS.SupportStruct_AnchorGuy");                
                m_CapBankRC = pElectricFWS.OpenRelationshipClass("EDGIS.SupportStruct_CapacitorBank");                
                m_DPDRC = pElectricFWS.OpenRelationshipClass("EDGIS.SupportStruct_DynProtDev");                
                m_JointOwnerRC = pElectricFWS.OpenRelationshipClass("EDGIS.SupportStructure_JointOwner");                
                m_StreetlightRC = pElectricFWS.OpenRelationshipClass("EDGIS.SupportStruct_Streetlight");                
                m_FuseRC = pElectricFWS.OpenRelationshipClass("EDGIS.SupportStruct_Fuse");                
                m_SwitchRC = pElectricFWS.OpenRelationshipClass("EDGIS.SupportStruct_Switch");                
                m_VoltageRegulatorRC = pElectricFWS.OpenRelationshipClass("EDGIS.SupportStruct_VoltReg");
                m_OpenPointRC = pElectricFWS.OpenRelationshipClass("EDGIS.SupportStruct_OpenPoint");
                m_FaultIndicatorRC = pElectricFWS.OpenRelationshipClass("EDGIS.SupportStruct_FaultIndicator");
                m_PrimaryMeterRC = pElectricFWS.OpenRelationshipClass("EDGIS.SupportStruct_PrimaryMeter");
                m_PrimaryRiserRC = pElectricFWS.OpenRelationshipClass("EDGIS.SupportStruct_PriRiser");
                m_StepDownRC = pElectricFWS.OpenRelationshipClass("EDGIS.SupportStruct_StepDown");

            }
            catch (Exception ex)
            {
                throw new Exception("Error returning the related devices");
            }
        }
        /// <summary>
        /// Adds all the domains to memory for performance benefit 
        /// </summary>
        /// <param name="pOC"></param>
        /// <param name="hshAllDomains"></param>
        private void CacheDomains(IObjectClass pOC)
        {
            try
            {
                IDataset pDS = (IDataset)pOC;
                string datasetName = GetShortDatasetName(pDS.Name).ToUpper(); 

                for (int i = 0; i < pOC.Fields.FieldCount; i++)
                {
                    if (pOC.Fields.get_Field(i).Domain != null)
                    {
                        if (pOC.Fields.get_Field(i).Domain is ICodedValueDomain)
                        {
                            if (((Hashtable)_hshAllAttributes[datasetName]).ContainsKey(pOC.Fields.get_Field(i).Name.ToUpper())) 
                            {
                                //Hashtable hshTemp = (Hashtable)_hshAllAttributes[datasetName];
                                Hashtable hshDomain = new Hashtable();
                                ICodedValueDomain pCodedValueDomain = (ICodedValueDomain)pOC.Fields.get_Field(i).Domain;
                                int codeCount = pCodedValueDomain.CodeCount;
                                //Loop through the list of values and print their names
                                for (int k = 0; k < codeCount; k++)
                                {
                                    if (!hshDomain.ContainsKey(pCodedValueDomain.get_Value(k)))
                                        hshDomain.Add(pCodedValueDomain.get_Value(k), pCodedValueDomain.get_Name(k));
                                }
                                //Add the domain using the field name as the key 
                                _hshAllDomains.Add(datasetName + "." + pOC.Fields.get_Field(i).Name.ToUpper(), hshDomain);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error caching domains"); 
            }
        }


        /// <summary>
        /// Determines the subset of poles to be processed and calls 
        /// ProcessPole routine to process each one 
        /// </summary>
        private void ProcessPoles()
        {
            try
            {
                //Write header information for the static data 
                WriteOutStaticDataHeader();
                WriteSpanCSVHeader();
                WriteFullDataCSVHeader(); 
                
                //Get the dem layer for identifying pole RL 
                IMxDocument pMxDoc = (IMxDocument)m_app.Document;
                List<ILayer> pLayers = getLayersFromMap(pMxDoc.FocusMap, "Heights");
                IRasterLayer pRasterLayer = (IRasterLayer)pLayers[0];
                IIdentify pIdentify = (IIdentify)pRasterLayer;

                IWorkspaceFactory workspaceFactory = new ESRI.ArcGIS.DataSourcesFile.ShapefileWorkspaceFactoryClass();
                IFeatureWorkspace featureWorkspace = (IFeatureWorkspace)workspaceFactory.OpenFromFile("C:\\Simon\\Data\\500PoleShapefile\\AOI_v3_20161025", 0);
                IFeatureClass pPoleShapefileFC = featureWorkspace.OpenFeatureClass("Pole_Loading_Pilot_AOI_20161025.shp");
                int fldIdx = -1;
                int sapId = -1; 

                //Loop through the pPoleShapefileFC and write out the attributes 
                IFeatureCursor pFCursor = pPoleShapefileFC.Search(null, false);
                IFeature pFeature = pFCursor.NextFeature();
                Hashtable hshSapIds = new Hashtable(); 

                while (pFeature != null)
                {
                    fldIdx = pFeature.Fields.FindField("SAPEQUIPID");
                    if (pFeature.get_Value(fldIdx) != DBNull.Value)
                    {
                        if (Int32.TryParse(pFeature.get_Value(fldIdx).ToString(), out sapId))
                        {
                            if (!hshSapIds.ContainsKey(sapId))
                                hshSapIds.Add(sapId, 0);
                        }
                    }

                    pFeature = pFCursor.NextFeature();
                }
                Marshal.FinalReleaseComObject(pFCursor);                
                
                int counter = 0;
                foreach (int sapEquipId in hshSapIds.Keys)
                {
                    Debug.Print("Processing pole: " + sapEquipId.ToString());
                    ProcessPole(sapEquipId, pIdentify, true);
                    counter++; 
                    Debug.Print("Finished: " + counter.ToString() + " poles");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error in ProcessPoles: " + ex.Message); 
            }
        }

        /// <summary>
        /// Performs Pole Loading Track 3 extraction for the passed pole 
        /// Including calculation of spans for the pole, determining devices 
        /// that sit on the pole etc. and outputting them to CSV or SQL Server 
        /// </summary>
        private void ProcessPole( 
            int sapEquipId, 
            IIdentify pIdentify, 
            bool displayForm)
        {
            try
            {

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
                IQueryFilter pQF = new QueryFilterClass();
                pQF.WhereClause = "SAPEQUIPID = '" + sapEquipId.ToString() + "'";
                IFeatureCursor pFCursor = m_SupportStructureFC.Search(pQF, false);
                IFeature pRefPoleFeature = pFCursor.NextFeature();
                Marshal.FinalReleaseComObject(pFCursor);
                if (pRefPoleFeature == null)
                {
                    //Exit out as the referebce pole was not found 
                    Debug.Print("Unable to find ref pole with sapequipid: " + sapEquipId.ToString());
                    return;
                }
                IPolygon pMaxSpanPolygon = GetBufferPolygon(
                    (IPoint)pRefPoleFeature.ShapeCopy, Shared.MaxSpanLength);

                //int fldIdx = (int)((Hashtable)_hshAllAttributes["SUPPORTSTRUCTURE"])["GLOBALID"];
                int fldIdx = m_SupportStructureFC.Fields.FindField("globalid");
                string poleGlobalId = pRefPoleFeature.get_Value(fldIdx).ToString();
                Pole pRefPole = new Pole(poleGlobalId, (IPoint)pRefPoleFeature.ShapeCopy);

                List<Conductor> pAllConductors = new List<Conductor>();
                GetAllOverheadConductors(ref pAllConductors, pMaxSpanPolygon);
                Debug.Print("found: " + pAllConductors.Count.ToString() + " overhead conductors in vicinity");

                List<Pole> pAllPoles = new List<Pole>();
                GetAllPoles(ref pAllPoles, pMaxSpanPolygon);
                Debug.Print("found: " + pAllPoles.Count.ToString() + " poles in vicinity");

                List<RelatedDevice> pAllRelatedDevices = new List<RelatedDevice>();
                GetAllRelatedDevices(pRefPoleFeature, ref pAllRelatedDevices);
                Debug.Print("found: " + pAllRelatedDevices.Count.ToString() + " related devices");

                //Create a circle polygon with the tolerance radius 
                //around the geometry of the reference pole (pPoleTolPolygon)
                IPolygon pPoleTolPolygon = GetBufferPolygon(
                    (IPoint)pRefPoleFeature.ShapeCopy, (int)Shared.RayBuffer);
                List<Conductor> pNearbyConductors = new List<Conductor>();
                GetNearbyConductors(
                    pRefPole,
                    pPoleTolPolygon,
                    pAllConductors,
                    pAllRelatedDevices,
                    ref pNearbyConductors);
                Debug.Print("found: " + pNearbyConductors.Count.ToString() + " nearby conductors");

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
                    pAllRelatedDevices,
                    pNearbyConductors,
                    pMaxSpanPolygon,
                    ref pSpans);
                Debug.Print("found: " + pSpans.Count.ToString() + " spans");


                //foreach (Span pSpan in pSpans)
                //{
                //    pGC.DeleteAllElements();
                //    GraphicUtils.DrawPolyline(pmap, pSpan.SpanPolyline, GraphicUtils.GetRGBColour(0, 0, 0), "");
                //    pAV.Refresh();
                //    MessageBox.Show("here is one");
                //}

                //1. Write out static information (and landbase derived attributes  
                //   for example snow loading zone, corrosion zone, RCZ etc 
                WriteOutStaticData(pRefPoleFeature);
                Debug.Print("written static data");

                //2. Write out span angle information 
                WriteSpanListToCSV(pSpans);
                Debug.Print("written span data");

                //3. Write out related conductor / device information 
                WriteRelatedDevicesToCSV(
                    pRefPole,
                    pSpans,
                    pNearbyConductors,
                    pAllRelatedDevices);
                Debug.Print("written full data csv");

                //Can bring up the form to display the pole 
                if (displayForm)
                {
                    frmDisplayPole frm = new frmDisplayPole(
                        sapEquipId.ToString(),
                        pSpans,
                        pNearbyConductors,
                        pAllRelatedDevices, 
                        m_app);
                    frm.Show();
                }
            }
            catch (Exception ex)
            {
                throw ex;
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
            List<RelatedDevice> pAllRelatedDevices, 
            List<Conductor> pNearbyConductors,
            IPolygon pMaxSpanRefPoleBuffer,
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

                foreach (Conductor pConductor in pNearbyConductors)
                {
                    //TODO *** Remove this next line when finished debugging 
                    //pSpans.Clear();
                    int spanCountb4 = pSpans.Count; 
                    TrackSpans( 
                        pAllConductors, 
                        pAllPoles,
                        pIndentify, 
                        pRefPole,
                        pConductor.Shape.FromPoint, 
                        pConductor, 
                        null, 
                        ref pSpans);

                    if ((pSpans.Count - spanCountb4) == 0)
                        Debug.Print("Did not find span for this conductor");
                    else if ((pSpans.Count - spanCountb4) > 1)
                        Debug.Print("Found more than 1 span for this conductor"); 

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
            }
            catch (Exception ex)
            {
                throw new Exception("Error delineating the spans");
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
                bool spanRemoved = true;
                while (spanRemoved == true)
                {
                    spanRemoved = false;
                    spanToRemove = ""; 
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
                                        spanRemoved = true;
                                        if (pSpan.SpanTrunk.Length < pSp.SpanTrunk.Length)
                                            spanToRemove = pSpan.SpanGUID;
                                        else
                                            spanToRemove = pSp.SpanGUID;

                                        break; 
                                    }
                                }
                            }
                            if (spanRemoved)
                                break; 
                        }
                        if (spanRemoved)
                            break;
                    }

                    if (spanRemoved)
                    {
                        //Remove the item
                        //Debug.Print("span count before: " + pSpans.Count.ToString());
                        pSpans.RemoveAll(item => item.SpanGUID == spanToRemove);
                        //Debug.Print("span count after: " + pSpans.Count.ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error in routine RemoveSpansWithDuplicateSegments");
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
            List<Conductor>pAllConductors,
            List<Pole> pAllPoles, 
            IIdentify pIdentify, 
            Pole pRefPole, 
            IPoint pStartPoint, 
            Conductor pCurrentConductor, 
            IPolyline pSpanTrunk,
            ref List<Span> pSpans)
        {
            try
            {
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
                

                //Check for a pole within tolerance of the pCurrentConductor
                double distFromPoleToConductor = 0; 
                Pole pAdjacentPole = GetAdjacentPole(pAllPoles, pCurrentConductor, pRefPole, ref distFromPoleToConductor);

                if (pAdjacentPole != null)
                {
                    //We have found an adjacent pole 

                    //Check the value of the distFromPoleToConductor and if this 
                    //value is > 12 foot then look to see if there is another 
                    //connected conductor that will get us closer to the adjacent 
                    //pole 
                    if (distFromPoleToConductor > 12)
                    {
                        Debug.Print("more than 12 foot to the adjacent pole: " +
                            distFromPoleToConductor.ToString());
                    }

                    //We have found the adjacent pole so create the span created 
                    //between the pRefPole and the pAdjacentPole 

                    //*** TODO put in the RLs for each of the poles 
                    //double fromPoleRL = GetHeightAtPoint(pRefPole.Shape, pIdentify);
                    //double toPoleRL = GetHeightAtPoint(pAdjacentPole.Shape, pIdentify);
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
                        GetHeightAtPoint(pRefPole.Shape, pIdentify),
                        pAdjacentPole.GUID,
                        GetHeightAtPoint(pAdjacentPole.Shape, pIdentify),
                        pCurrentConductor.CircId);
                    pSpans.Add(pSpan);

                }
                else
                {
                    //We did not find an adjacent pole 

                    //Add the current conductor to the trunk and and continue tracking 
                    //and looking for an adjacent pole 

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
                            GetHeightAtPoint(pRefPole.Shape, pIdentify),
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
                            pSpanTrunkExtended.ToPoint,
                            pConductor,
                            pSpanTrunkExtended,
                            ref pSpans); 
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error tracking recursive spans");
            }
        }

        private IPolyline JoinPolylineToTrunk(IPolyline pTrunkPolyline, IPolyline pNewConductor)
        {
            try
            {
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
                    return pTrunkPolylineCopy;  
                }
            }
            catch (Exception ex)
            {
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
        private IPoint GetConductorStartPoint(Pole pRefPole, Conductor pConductor)
        {
            try
            {
                IProximityOperator pProxOp = (IProximityOperator)pRefPole.Shape;
                double distToFromPoint = pProxOp.ReturnDistance(pConductor.Shape.FromPoint);
                double distToToPoint = pProxOp.ReturnDistance(pConductor.Shape.ToPoint);

                //Clone the shape to be safe 
                IPoint pClosestEnd = null;
                IClone pClone = null;
                if (distToFromPoint < distToToPoint)
                    pClone = (IClone)pConductor.Shape.FromPoint;
                else
                    pClone = (IClone)pConductor.Shape.ToPoint;

                pClosestEnd = (IPoint)pClone.Clone();
                pClosestEnd.SpatialReference = pRefPole.Shape.SpatialReference;
                return pClosestEnd; 
            }
            catch (Exception ex)
            {
                throw new Exception("Error finding other end of conductor");
            }
        }

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
                throw new Exception("Error returning the connected conductors");
            }
        }


        private Pole GetAdjacentPole( 
            List<Pole> pAllPoles, 
            Conductor pCurrentConductor, 
            Pole pRefPole, 
            ref double distFromPoleToConductor)
        {
            try
            {
                Pole pAdjacentPole = null;
                Pole pPossibleAdjacentPole = null; 

                IPolygon pConductorBuffer = GetBufferPolygon( 
                    pCurrentConductor.Shape, (int)Shared.RayBuffer);

                //Loop through all the poles to and find any that 
                //lie within the conductorbuffer
                List<Pole> pAdjacentPoles = new List<Pole>(); 
                IRelationalOperator pRelOp = (IRelationalOperator)pConductorBuffer; 
                foreach (Pole pCurrentPole in pAllPoles)
                {
                    if ((pRelOp.Contains(pCurrentPole.Shape)) &&
                        (pCurrentPole.GUID != pRefPole.GUID))
                    {
                        //We have found an adjacent pole so 
                        //add it to a list (because there maybe a few of them) 
                        pPossibleAdjacentPole = new Pole(pCurrentPole.GUID, pCurrentPole.Shape);
                        pAdjacentPoles.Add(pPossibleAdjacentPole); 
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
                    IProximityOperator pPO = (IProximityOperator)pRefPole.Shape; 
                    
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
                    //No adjacent pole found for this conductor 
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
                throw new Exception("Error returning adjacent pole");
            }
        }

        private void GetAllOverheadConductors(
            ref List<Conductor> pConductorList, 
            IPolygon pMaxSpanRefPoleBuffer)
        {
            try
            {
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

            }
            catch (Exception ex)
            {
                throw new Exception("Error returning all overhead conductors");
            }        
        }

        private void GetAllPoles(
                ref List<Pole> pPoleList,
                IPolygon pMaxSpanRefPoleBuffer)
        {
            try
            {
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
                throw new Exception("Error returning all poles");
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
                //      StepDown 

                //JointOwner
                GetRelatedDevices(pPoleFeature, m_JointOwnerRC, ref pRelatedDevices);
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
                //FaultIndicator 
                GetRelatedDevices(pPoleFeature, m_FaultIndicatorRC, ref pRelatedDevices);
                //PrimaryMeter 
                GetRelatedDevices(pPoleFeature, m_PrimaryMeterRC, ref pRelatedDevices);
                //PrimaryRiser 
                GetRelatedDevices(pPoleFeature, m_PrimaryRiserRC, ref pRelatedDevices);
                //StepDown 
                GetRelatedDevices(pPoleFeature, m_StepDownRC, ref pRelatedDevices);
            }
            catch (Exception ex)
            {
                throw new Exception("Error returning all related devices");
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
            List<Conductor> pAllConductorList,
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

                foreach (RelatedDevice pRelDevice in pAllRelatedDevices)
                {
                    lookforConductorsAtPoint = false;
                    switch (pRelDevice.DeviceType)
                    {
                        //lit of all featureclasses within geometric network related 
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
                        foreach (Conductor pConductor in pAllConductorList)
                        {
                            //There should be 2 in most cases 
                            isAtFromPoint = IsConincident(pConductor.Shape.FromPoint, pRelDevice.Shape);
                            isAtToPoint = IsConincident(pConductor.Shape.ToPoint, pRelDevice.Shape);

                            if (isAtFromPoint || isAtToPoint)
                            {
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

                                    //Copy the conductor 
                                    Conductor pNearbyConductor = new Conductor(
                                        pConductor.OID,
                                        pConductor.Subtype,
                                        pConductor.FCName,
                                        pConductor.GUID,
                                        pConductor.CircId,
                                        pConductor.OperatingVoltage,
                                        pConductor.ConstructionType,
                                        pConductor.Spacing,
                                        pPolylineClone,
                                        GetConductorInfoForConductor(pConductor.FCName, pConductor.OID));
                                    pNearbyConductorList.Add(pNearbyConductor);
                                    hshNearbyConductors.Add(conductorKey, "");
                                }
                            }
                        }
                    }
                }

                //2. Get everything within tolerance of the ref pole  
                IProximityOperator pProxOp = (IProximityOperator)pRefPole.Shape; 
                IRelationalOperator pRelOp = (IRelationalOperator)pPoleTolBuffer;
                double distToFrom = 0;
                double distToTo = 0;
                double distToVertex = 0;
                double closestDist = 0;
                IPoint pClosestVertex = null;
                object obj = Type.Missing;
                
                foreach (Conductor pConductor in pAllConductorList)
                {
                    if (!pRelOp.Disjoint(pConductor.Shape))
                    {
                        //Do not add if it is already in the list 
                        conductorKey = pConductor.FCName + pConductor.OID;
                        if (!hshNearbyConductors.ContainsKey(conductorKey))
                        {
                            //Clone to be safe 
                            IClone pClone = (IClone)pConductor.Shape;
                            IPolyline pPolylineClone = (IPolyline)pClone.Clone();
                            pPolylineClone.SpatialReference =
                                pConductor.Shape.SpatialReference;

                            //(a) Look for an endpoint within tolerance of the pole 
                            distToFrom = pProxOp.ReturnDistance(pPolylineClone.FromPoint);
                            distToTo = pProxOp.ReturnDistance(pPolylineClone.ToPoint);
                            if ((distToFrom < Shared.RayBuffer) ||
                                (distToTo < Shared.RayBuffer))
                            {
                                //This is scenario (a) (endpoint within tolerance)

                                //ReverseOrientation if necessary 
                                if (!(distToFrom < distToTo))
                                    pPolylineClone.ReverseOrientation();

                                //Copy the conductor 
                                Conductor pNearbyConductor = new Conductor(
                                    pConductor.OID,
                                    pConductor.Subtype,
                                    pConductor.FCName,
                                    pConductor.GUID,
                                    pConductor.CircId,
                                    pConductor.OperatingVoltage,
                                    pConductor.ConstructionType,
                                    pConductor.Spacing,
                                    pPolylineClone,
                                    GetConductorInfoForConductor(pConductor.FCName, pConductor.OID));
                                pNearbyConductorList.Add(pNearbyConductor);
                                hshNearbyConductors.Add(conductorKey, "");
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
                                Shared.WriteToLogfile(pConductor.FCName + pConductor.OID + ": " + "found vertex (scenario b) at dist: " + closestDist.ToString());

                                if (closestDist < Shared.RayBuffer)
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
                                        Conductor pNearbyConductor = new Conductor(
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
                                        pNearbyConductorList.Add(pNearbyConductor); 
                                    }
                                    hshNearbyConductors.Add(conductorKey, "");
                                }
                                else
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
                                        //Unable to split so throw an error in this case 
                                        throw new Exception("Unable to split conductor at closest point to reference pole");
                                    }

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
                                        Conductor pNearbyConductor = new Conductor(
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

                                        pNearbyConductorList.Add(pNearbyConductor);
                                    }
                                    hshNearbyConductors.Add(conductorKey, "");
                                }
                            }                            
                        }
                    }
                }                
            }
            catch (Exception ex)
            {
                throw new Exception("Error returning all related devices");
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
                string  material = string.Empty;
                string  phase = string.Empty;
                string conductorType = string.Empty;
                Hashtable hshConductorInfoAttributes = null;
                ISet pSet = null;
                int fldIdx = -1; 

                if (conductorFCName.ToUpper() == "PRIOHCONDUCTOR")
                {
                    conductorInfoTblName = "PRIOHCONDUCTORINFO"; 
                    hshConductorInfoAttributes = (Hashtable)_hshAllAttributes[conductorInfoTblName];
                    pConductorFeature = m_PriOHConductorFC.GetFeature(conductorOId);
                    pSet = m_PriOHConductorInfoRC.GetObjectsRelatedToObject(pConductorFeature);
                }

                else if (conductorFCName.ToUpper() == "SECOHCONDUCTOR")
                {
                    conductorInfoTblName = "SECOHCONDUCTORINFO";
                    hshConductorInfoAttributes = (Hashtable)_hshAllAttributes[conductorInfoTblName];
                    pConductorFeature = m_SecOHConductorFC.GetFeature(conductorOId);
                    pSet = m_SecOHConductorInfoRC.GetObjectsRelatedToObject(pConductorFeature);
                }

                //Load the conductor info records 
                pConductorInfos.Clear();
                pSet.Reset();
                IRow pConductorInfoRow = (IRow)pSet.Next();
                    while (pConductorInfoRow != null)
                    {
                        conductorSize = string.Empty;
                        conductorUse = string.Empty;
                        conductorCount = 0;
                        material = string.Empty;
                        phase = string.Empty;
                        conductorType = string.Empty;

                        //GLOBALID
                        fldIdx = (int)hshConductorInfoAttributes["GLOBALID"];
                        conInfoGlobalid = pConductorInfoRow.get_Value(fldIdx).ToString();

                        //CONDUCTORSIZE 
                        fldIdx = (int)hshConductorInfoAttributes["CONDUCTORSIZE"];
                        conductorSize = GetDomainDescriptionFromDomainCode(
                            pConductorInfoRow.Fields.get_Field(fldIdx),
                            pConductorInfoRow.get_Value(fldIdx));

                        //CONDUCTORUSE 
                        fldIdx = (int)hshConductorInfoAttributes["CONDUCTORUSE"];
                        conductorUse = GetDomainDescriptionFromDomainCode(
                            pConductorInfoRow.Fields.get_Field(fldIdx),
                            pConductorInfoRow.get_Value(fldIdx));

                        //CONDUCTORCOUNT
                        fldIdx = (int)hshConductorInfoAttributes["CONDUCTORCOUNT"];
                        if (pConductorInfoRow.get_Value(fldIdx) != DBNull.Value)
                            conductorCount = Convert.ToInt32(pConductorInfoRow.get_Value(fldIdx));

                        //CONDUCTORTYPE Added by Tina
                        fldIdx = (int)hshConductorInfoAttributes["CONDUCTORTYPE"];
                        if (pConductorInfoRow.get_Value(fldIdx) != DBNull.Value)
                            conductorType = pConductorInfoRow.get_Value(fldIdx).ToString();

                        //MATERIAL 
                        fldIdx = (int)hshConductorInfoAttributes["MATERIAL"];
                        material = GetDomainDescriptionFromDomainCode(
                            pConductorInfoRow.Fields.get_Field(fldIdx),
                            pConductorInfoRow.get_Value(fldIdx));

                        //PHASEDESIGNATION 
                        fldIdx = (int)hshConductorInfoAttributes["PHASEDESIGNATION"];
                        phase = GetDomainDescriptionFromDomainCode(
                            pConductorInfoRow.Fields.get_Field(fldIdx),
                            pConductorInfoRow.get_Value(fldIdx));

                        ConductorInfo pConInfo = new ConductorInfo(
                            pConductorInfoRow.OID,
                            conductorInfoTblName,
                            conInfoGlobalid,
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

                //if (pConductorInfos.Count > 1)
                //    Debug.Print("Found one with count greater than 1"); 
                return pConductorInfos; 
            }
            catch (Exception ex)
            {
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
                        if (dist < PoleLoadingConstants.COINCIDENCE_THRESHOLD)
                            isPointConincident = true;
                    }
                }
                return isPointConincident; 
            }
            catch (Exception ex)
            {
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
                IDataset pDS = (IDataset)pConductorFC;
                string conductorFCName = GetShortDatasetName(pDS.Name).ToUpper();
                //pDS = (IDataset)pConductorInfoTbl;
                //string conductorInfoTblName = GetShortDatasetName(pDS.Name).ToUpper();
                ISpatialFilter pSF = new SpatialFilterClass();
                //*** should screen out ones that are not in service 
                pSF.WhereClause = "status = 5"; 
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
                    fldIdx = (int)hshConductorAttributes["CIRCUITID"];
                    if (pConductorFeature.get_Value(fldIdx) != DBNull.Value)
                        circId = pConductorFeature.get_Value(fldIdx).ToString();

                    //CONSTRUCTIONTYPE 
                    if (hshConductorAttributes.ContainsKey("CONSTRUCTIONTYPE"))
                    {
                        fldIdx = (int)hshConductorAttributes["CONSTRUCTIONTYPE"];
                        if (pConductorFeature.get_Value(fldIdx) != DBNull.Value)
                            constructionType = GetFieldValue(pConductorFeature, fldIdx);
                    }
                                        
                    //OPERATINGVOLTAGE 
                    fldIdx = (int)hshConductorAttributes["OPERATINGVOLTAGE"];
                    if (pConductorFeature.get_Value(fldIdx) != DBNull.Value)
                    {
                        operatingVoltage = GetFieldValue(pConductorFeature, fldIdx);
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
                throw new Exception("Error encountered in GetConductorsInPolygon: " +
                ex.Message);
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
        private List<Pole> GetNearbyPoles(
            IFeatureClass pPoleFC,
            IPolygon pSearchPolygon)
        {
            try
            {
                List<Pole> pPoleList = new List<Pole>();
                ISpatialFilter pSF = new SpatialFilterClass();
                pSF.Geometry = pSearchPolygon;
                pSF.SpatialRel = esriSpatialRelEnum.esriSpatialRelIntersects;
                IFeatureCursor pFCursor = pPoleFC.Search(pSF, false);
                IFeature pPoleFeature = pFCursor.NextFeature();

                string globalid = string.Empty;
                int fldIdx = -1;
                fldIdx = pPoleFC.Fields.FindField("globalid"); 
                
                while (pPoleFeature != null)
                {
                    globalid = pPoleFeature.get_Value(fldIdx).ToString();
                    Pole pPole = new Pole(globalid, (IPoint)pPoleFeature.ShapeCopy);
                    pPoleList.Add(pPole); 
                    pPoleFeature = pFCursor.NextFeature();
                }
                Marshal.FinalReleaseComObject(pFCursor);

                return pPoleList; 
            }
            catch (Exception ex)
            {
                throw new Exception("Error encountered in GetNearbyPoles: " +
                ex.Message);
            }
        }

        /// <summary>
        /// Exports the SDE data into a file GDB - you can clip and 
        /// if no clip polygon is supplied it will take all the data 
        /// </summary>
        /// <param name="pClipPolygon"></param>
        /// <param name="maxTries"></param>
        /// <param name="saveClip"></param>
        public void ExtractPoles()
        {
            IWorkspaceEdit pWSE = null;
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();

            try
            {
                Shared.WriteToLogfile("Entering ExtractPoles");

                //Scroll through each dataset exporting each one
                IPolygon pClipPolygon = Shared.GetClipPolygon();  
                string datasetType = string.Empty;
                string mapGeo = string.Empty; 
                bool exportSucceeded = false;
                int fcTotal = 0;
                int fcCount = 0;
                string msgDS = "";
                IFeatureWorkspace pSourceFWS = null;
                IFeatureWorkspace pDestFWS = (IFeatureWorkspace)Shared.GetWorkspaceByName("output");
                pWSE = (IWorkspaceEdit)pDestFWS;
                IFeatureClass pSourceFC = null;
                IFeatureClass pDestFC = null;
                //ITable pSourceTable = null;
                //ITable pSourceJoinTable = null;
                //ITable pDestTable = null;
                string msg = string.Empty;
                string shortSourceName = string.Empty;
                ExtractUtils pExtractUtils = new ExtractUtils(null);
                int j = 0;


                //pWSE.StartEditing(false);
                //foreach (ExtractDataset pExtractDS in _extractDatasets)
                //{
                //    //Open source and destination featureclass/table 
                //    pDestFC = pDestFWS.OpenFeatureClass(pExtractDS.TargetName);
                //    ((ITable)pDestFC).DeleteSearchedRows(null);
                //}
                ////Stop editing saving edits 
                //pWSE.StopEditing(true);                               


                //Determine the number of datasets to be processed 
                pWSE.StartEditing(false);

                foreach (ExtractDataset pExtractDS in Shared.ExtractDatasets)
                {
                    fcCount++;
                    exportSucceeded = false;

                    for (int i = 0; i < Shared.MaxTries ; i++)
                    {
                        msgDS = "Exporting " + datasetType + ": " + pExtractDS.TargetName + " " +
                            fcCount + " of: " + fcTotal.ToString();
                        Shared.WriteToLogfile("======================================================");
                        Shared.WriteToLogfile(msgDS);

                        //Open source and destination featureclass/table 
                        pSourceFWS = (IFeatureWorkspace)Shared.GetWorkspaceByName(pExtractDS.Workspace.ToLower());
                        if (pExtractDS.DatasetType == ExtractDatasetType.etFeatureclass ||
                            pExtractDS.DatasetType == ExtractDatasetType.etAnnoFeatureclass ||
                            pExtractDS.DatasetType == ExtractDatasetType.etJoinFeatureclass)
                        {
                            Shared.WriteToLogfile("opening source " + datasetType + ": " + pExtractDS.SourceName);
                            pSourceFC = pSourceFWS.OpenFeatureClass(pExtractDS.SourceName);
                            Shared.WriteToLogfile("opening target " + datasetType + ": " + pExtractDS.TargetName);
                            pDestFC = pDestFWS.OpenFeatureClass(pExtractDS.TargetName);
                        }

                        try
                        {
                            //Perform the export                                        
                            pExtractUtils.ExportFeatureClassLO(
                            msgDS,
                            pExtractDS.UseClip,
                            true,
                            pSourceFC,
                            pDestFC,
                            pClipPolygon,
                            pExtractDS.Filter);

                            //succeeded so break out 
                            exportSucceeded = true;
                            Shared.WriteToLogfile(datasetType + ": " + pExtractDS.TargetName +
                                " succeeded on attempt: " + (i + 1).ToString());
                            break;
                        }
                        catch (Exception ex)
                        {
                            Shared.WriteToLogfile(datasetType + ": " + pExtractDS.TargetName +
                                " failed on attempt " + (i + 1).ToString() +
                                " with error: " + ex.Message);
                        }
                    }
                    if (!exportSucceeded)
                        throw new Exception("Export of ds: " + pExtractDS.TargetName + " failed");

                }

                //Stop editing saving edits 
                pWSE.StopEditing(true);


                pSourceFWS = null;
                pDestFWS = null;
                pWSE = null;
                pSourceFC = null;
                pDestFC = null;
                //pSourceTable = null;
                //pDestTable = null;

                Shared.WriteToLogfile("Finished entire export for process in: " + 
                    stopWatch.ElapsedMilliseconds + " milliseconds");
                stopWatch.Stop();

            }
            catch (Exception ex)
            {
                Shared.WriteToLogfile("Entering error handler for " + MethodBase.GetCurrentMethod() + " error: " +
                    ex.Message);
                if (pWSE != null)
                    pWSE.StopEditing(false);
                Shared.WriteToLogfile("An error occurred in " + MethodBase.GetCurrentMethod() +
                    "edits have been aborted");
                throw new Exception("Error in ExportGeodatabase_GOLD");
            }
        }
    }

    public class ExtractDataset
    {
        //Property storage variables 
        private ExtractDatasetType _datasetType;
        private string _sourceName;
        private string _targetName;
        private int _processNumber;
        private string _filter;
        private string _sourceFeatureFilter;
        private string _workspace;
        private string _featuredataset;
        private string _sourceJoinTable;
        private string _sourceFeatureRelClass;
        private string _joinWhereClause;
        bool _includeAttributes;
        bool _useClip;
        bool _projectAfterExtract;
        bool _mustHaveRel;

        public ExtractDataset(
            ExtractDatasetType datasetType,
            string sourceName,
            string targetName,
            string filter,
            string sourceFeatureFilter,
            string sourceFeatureRelClass,
            bool mustHaveRel,
            string workspace,
            string featureDataset,
            string sourceJoinTable,
            string joinWhereClause,
            bool useClip,
            bool includeAttribs,
            bool projectAfterExtract,
            int processNumber)
        {
            _datasetType = datasetType;
            _sourceName = sourceName;
            _targetName = targetName;
            _filter = filter;
            _sourceFeatureFilter = sourceFeatureFilter;
            _sourceFeatureRelClass = sourceFeatureRelClass;
            _mustHaveRel = mustHaveRel;
            _workspace = workspace;
            _featuredataset = featureDataset;
            _sourceJoinTable = sourceJoinTable;
            _joinWhereClause = joinWhereClause;
            _useClip = useClip;
            _includeAttributes = includeAttribs;
            _projectAfterExtract = projectAfterExtract;
            _processNumber = processNumber;
        }

        public ExtractDatasetType DatasetType
        {
            get { return _datasetType; }
        }
        //public ExtractFrequency ExtractFrequency
        //{
        //    get { return _extractFrequency; }
        //    set { _extractFrequency = value;}
        //}
        public string SourceName
        {
            get { return _sourceName; }
        }
        public string SourceJoinTable
        {
            get { return _sourceJoinTable; }
        }
        public string JoinWhereClause
        {
            get { return _joinWhereClause; }
        }
        public string TargetName
        {
            get { return _targetName; }
        }
        public string Filter
        {
            get { return _filter; }
        }
        public string Workspace
        {
            get { return _workspace; }
        }
        public string FeatureDataset
        {
            get { return _featuredataset; }
        }
        public int ProcessNumber
        {
            get { return _processNumber; }
        }
        public bool UseClip
        {
            get { return _useClip; }
        }
        public bool IncludeAttributes
        {
            get { return _includeAttributes; }
        }
        public bool ProjectAfterExtract
        {
            get { return _projectAfterExtract; }
        }
        public string SourceFeatureFilter
        {
            get { return _sourceFeatureFilter; }
        }
        public string SourceFeatureRelClass
        {
            get { return _sourceFeatureRelClass; }
        }
        public bool MustHaveRel
        {
            get { return _mustHaveRel; }
        }
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
            _constructionType = constructionType; 
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
        private IPoint _shape;

        public RelatedDevice(
            string deviceType,
            IPoint pPoint, 
            int OId)
        {
            _deviceType = deviceType;
            _shape = pPoint;
            _OId = OId;
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
    }



    public class ConductorInfo
    {
        //Property storage variables 
        private int _oid;
        private string _guid;
        private string _tableName;
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
            get { return _conductorClass ;}
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
