#region Header
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.NetworkAnalysis;
using Miner.Interop;
using PGE_DBPasswordManagement;
#endregion

namespace PGE.BatchApplication.CircuitSource_Populate
{
    class Program
    {
        // Trace Direction
        enum TraceDirection
        {
            Downstream = 1,
            Upstream   = 2,
            Connected  = 3
        }
        //ESRI Licence Initializer
        private static LicenseInitializer                m_AOLicenseInitializer = new PGE.BatchApplication.CircuitSource_Populate.LicenseInitializer();
        private static IFeatureWorkspace                 pFWorskspace_EDER      = default(IFeatureWorkspace),
                                                         pFWorskspace_EDERSUB   = default(IFeatureWorkspace);
        private static Dictionary<string, IFeatureClass> pFC_All                = new Dictionary<string, IFeatureClass>();
        private static Dictionary<string, ITable>        pTable_All             = new Dictionary<string, ITable>();
        //LogFile PAth. If null in Config, it will take EXE running location
        private static string                            sPath_Log              = (String.IsNullOrEmpty(ConfigurationManager.AppSettings["LOGPATH"]) ?
                                                                                  System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) :
                                                                                  ConfigurationManager.AppSettings["LOGPATH"]) + "\\Logfile_" + DateTime.Now.Ticks.ToString() + ".log";
        private static StreamWriter                      pSWriter               = default(StreamWriter);
        private static double                            dBuffer_Search         = Convert.ToDouble(ConfigurationManager.AppSettings["SUB_BUFFER"]);

        //ArcFM Licence Initializer
        private static IMMAppInitialize                  arcFMAppInitialize     = new MMAppInitializeClass();

        //Mapping for Load to Source
        private static SourceFeederMapping               pSFMap                 = new SourceFeederMapping();
        private static IList<SourceFeederMapping>        pList_SFMap            = new List<SourceFeederMapping>();

        /// <summary>
        /// Method to compute Load to Source relationship for Substations
        /// This relationship would be save to DB that would be used with EDGIS.PGE_FEEDERFEDNETWORK_TRACE
        /// as Materialiazed View EDGIS.PGE_FEEDERFEDNETWORK_TRACE_VW
        /// </summary>
        /// <param name="args">No Arguments required</param>
        [STAThread()]
        static void Main(string[] args)
        {
            try
            {
                (pSWriter = File.CreateText(sPath_Log)).Close();
                //ESRI License Initializer generated code.
                WriteLine("PGE.BatchApplication.CircuitSource_Populate.Program.Main : Getting Licence Initialized");
                m_AOLicenseInitializer.InitializeApplication(new esriLicenseProductCode[] { esriLicenseProductCode.esriLicenseProductCodeBasic, esriLicenseProductCode.esriLicenseProductCodeStandard, esriLicenseProductCode.esriLicenseProductCodeAdvanced },
                new esriLicenseExtensionCode[] { });
                mmLicenseStatus licenseStatus = CheckOutLicenses(mmLicensedProductCode.mmLPArcFM);
                // Get all Required FeatureClasses in a Collection at first.
                GetRequiredClasses();

                // Get all required tables in a collection at first.
                GetRequiredTables();

                //Get Required Trace network at first
                GetNetworks((IWorkspace)pFWorskspace_EDERSUB);

                //Main Compilation
                Main1();

                //Write Computed values to DB based on the credentials of SDE file provided
                WriteDataTable();

                //Shutdown
                WriteLine("Process completed. Releasing licence and objects");
                m_AOLicenseInitializer.ShutdownApplication();
            }
            catch (Exception ex)
            {
                WriteLine("PGE.BatchApplication.CircuitSource_Populate.Program.Main : Error : " + ex.Message);
            }
            finally
            {
                if (pFWorskspace_EDER != null) Marshal.FinalReleaseComObject(pFWorskspace_EDER);
                if (pFWorskspace_EDERSUB != null) Marshal.FinalReleaseComObject(pFWorskspace_EDERSUB);
                foreach(IFeatureClass pFClass in pFC_All.Values)
                    if (pFClass != null) Marshal.FinalReleaseComObject(pFClass);
                pFC_All.Clear();
                GC.Collect();
            }
        }

        /// <summary>
        /// Write all computed data to "EDGIS.PGE_CircuitSource"
        /// </summary>
        private static void WriteDataTable()
        {
            string sTableName = ConfigurationManager.AppSettings["EDER_FEEDERTABLENAME"];
            WriteLine("PGE.BatchApplication.CircuitSource_Populate.Program.WriteDataTable");
            ((IWorkspace)pFWorskspace_EDER).ExecuteSQL("TRUNCATE TABLE " + sTableName);
            WriteLine(sTableName + " table truncated"); ;
            for (int iCount = 0; iCount < pList_SFMap.Count; ++iCount)
            {
                WriteLine("Inserting record " + Convert.ToString(iCount) + "/" + Convert.ToString(pList_SFMap.Count));
                string sSQL = "INSERT INTO " + sTableName + " (FROM_CIRCUITID,FROM_LINE_FC_NAME,FROM_LINE_GLOBALID,FROM_POINT_FC_GUID,FROM_POINT_FC_NAME,SUBSTATION_GLOBALID,SUBSTATIONNAME,TO_CIRCUITID,TO_LINE_FC_NAME,TO_LINE_GLOBALID,TO_POINT_FC_GUID,TO_POINT_FC_NAME,TX_BANK_GLOBALID,TX_BANK_CODE)";
                sSQL += "VALUES";
                sSQL += "('" + pList_SFMap[iCount].FROM_CIRCUITID + "',";
                sSQL += "'" + pList_SFMap[iCount].FROM_LINE_FC_NAME + "',";
                sSQL += "'" + pList_SFMap[iCount].FROM_LINE_GLOBALID + "',";
                sSQL += "'" + pList_SFMap[iCount].FROM_POINT_FC_GUID + "',";
                sSQL += "'" + pList_SFMap[iCount].FROM_POINT_FC_NAME + "',";
                sSQL += "'" + pList_SFMap[iCount].SUBSTATION_GLOBALID + "',";
                sSQL += "'" + pList_SFMap[iCount].SUBSTATION_NAME + "',";
                sSQL += "'" + pList_SFMap[iCount].TO_CIRCUITID + "',";
                sSQL += "'" + pList_SFMap[iCount].TO_LINE_FC_NAME + "',";
                sSQL += "'" + pList_SFMap[iCount].TO_LINE_GLOBALID + "',";
                sSQL += "'" + pList_SFMap[iCount].TO_POINT_FC_GUID + "',";
                sSQL += "'" + pList_SFMap[iCount].TO_POINT_FC_NAME + "',";
                sSQL += "'" + pList_SFMap[iCount].TX_BANK_GLOBALID + "',";
                sSQL += "'" + pList_SFMap[iCount].TX_BANK_CODE + "')";
                ((IWorkspace)pFWorskspace_EDER).ExecuteSQL(sSQL);
            }
        }


        /// <summary>
        /// Method to compute Load to Source relationship for Substations
        /// This relationship would be save to DB that would be used with EDGIS.PGE_FEEDERFEDNETWORK_TRACE
        /// as Materialiazed View EDGIS.PGE_FEEDERFEDNETWORK_TRACE_VW
        /// </summary>
        private static void Main1()
        {
            WriteLine("PGE.BatchApplication.CircuitSource_Populate.Program.Main1");
            IFeatureCursor pFCur_Substation = default(IFeatureCursor),
                           pFCur_Conductor  = default(IFeatureCursor),
                           pFCur_Stitch     = default(IFeatureCursor);
            IFeature       pFeat_Substation = default(IFeature),
                           pFeat_EderStitch = default(IFeature),
                           pFeat_Conductor  = default(IFeature);
            ISpatialFilter pSFilter         = new SpatialFilterClass();
            int            iCount_Total_E   = 0,
                           iCount_Current_E = 0,
                           iCount_Total_S   = 0,
                           iCount_Current_S = 0;
            try
            {
                //Count of Substations and respective features
                iCount_Total_S = pFC_All["EDER_SUBSTATION_FC"].FeatureCount(String.IsNullOrEmpty(ConfigurationManager.AppSettings["SUBSTATION_FILTER"]) ? null : new QueryFilter() { WhereClause = ConfigurationManager.AppSettings["SUBSTATION_FILTER"] });
                pFCur_Substation = pFC_All["EDER_SUBSTATION_FC"].Search(String.IsNullOrEmpty(ConfigurationManager.AppSettings["SUBSTATION_FILTER"]) ? null : new QueryFilter() { WhereClause = ConfigurationManager.AppSettings["SUBSTATION_FILTER"] }, false);

                //Loop through all Substations
                while ((pFeat_Substation = pFCur_Substation.NextFeature()) != null)
                {
                    WriteLine("");
                    WriteLine("");
                    WriteLine("");
                    WriteLine("Checking Substation " + Convert.ToString(++iCount_Current_S) + "/" + Convert.ToString(iCount_Total_S) + " NAME: " + Convert.ToString(pFeat_Substation.get_Value(pFC_All["EDER_SUBSTATION_FC"].FindField("NAME"))));
                    iCount_Current_E = 0;

                    //Finding All Electric Stitch Point or CircuitSurces within agreed 600ft buffer
                    pSFilter = new SpatialFilterClass();
                    pSFilter.Geometry = ((ITopologicalOperator)pFeat_Substation.ShapeCopy).Buffer(dBuffer_Search);
                    pSFilter.GeometryField = pFC_All["EDER_ELECSTICHPOINT_FC"].ShapeFieldName;
                    pSFilter.SpatialRel = esriSpatialRelEnum.esriSpatialRelIntersects;
                    pSFilter.SubFields = pFC_All["EDER_ELECSTICHPOINT_FC"].OIDFieldName + "," + pFC_All["EDER_ELECSTICHPOINT_FC"].ShapeFieldName + ",GLOBALID,SUBTYPECD";
                    //Should be InService
                    pSFilter.WhereClause = "STATUS > 0"; //SUBTYPECD = 2 AND 
                    iCount_Total_E = pFC_All["EDER_ELECSTICHPOINT_FC"].FeatureCount(pSFilter);
                    pFCur_Stitch = pFC_All["EDER_ELECSTICHPOINT_FC"].Search(pSFilter, false);

                    //Looping through all CircuitSource found as per above criteria
                    while ((pFeat_EderStitch = pFCur_Stitch.NextFeature()) != null)
                    {
                        //Object with all Mapping Information
                        pSFMap = new SourceFeederMapping();
                        pSFMap.SUBSTATION_NAME = Convert.ToString(pFeat_Substation.get_Value(pFC_All["EDER_SUBSTATION_FC"].FindField("NAME")));
                        pSFMap.SUBSTATION_GLOBALID = Convert.ToString(pFeat_Substation.get_Value(pFC_All["EDER_SUBSTATION_FC"].FindField("GLOBALID")));
                        pSFMap.TO_POINT_FC_NAME = ((IDataset)pFC_All["EDER_ELECSTICHPOINT_FC"]).BrowseName;
                        pSFMap.TO_POINT_FC_GUID = Convert.ToString(pFeat_EderStitch.get_Value(pFC_All["EDER_ELECSTICHPOINT_FC"].FindField("GLOBALID")));

                        WriteLine("Processing Electric Stich Point " + Convert.ToString(++iCount_Current_E) + "/" + Convert.ToString(iCount_Total_E) + " GUID: " + pSFMap.TO_POINT_FC_GUID);

                        //Finding Attached PriUG or PriOH Conductors to have information for Loading side
                        pSFilter = new SpatialFilterClass();
                        pSFilter.Geometry = pFeat_EderStitch.Shape;
                        pSFilter.GeometryField = pFC_All["EDER_PRIUG_FC"].ShapeFieldName;
                        pSFilter.SpatialRel = esriSpatialRelEnum.esriSpatialRelIntersects;
                        pSFilter.SubFields = pFC_All["EDER_PRIUG_FC"].OIDFieldName + "," + pFC_All["EDER_PRIUG_FC"].ShapeFieldName + ",GLOBALID,CIRCUITID";
                        pSFilter.WhereClause = "CIRCUITID IS NOT NULL";
                        pFCur_Conductor = pFC_All["EDER_PRIUG_FC"].Search(pSFilter, false);

                        if ((pFeat_Conductor = pFCur_Conductor.NextFeature()) != null)
                        {
                            pSFMap.TO_LINE_FC_NAME = ((IDataset)pFeat_Conductor.Class).BrowseName;
                            pSFMap.TO_LINE_GLOBALID = Convert.ToString(pFeat_Conductor.get_Value(pFeat_Conductor.Fields.FindField("GLOBALID")));
                            pSFMap.TO_CIRCUITID = Convert.ToString(pFeat_Conductor.get_Value(pFeat_Conductor.Fields.FindField("CIRCUITID")));
                        }
                        else
                        {
                            pFCur_Conductor = pFC_All["EDER_PRIOH_FC"].Search(pSFilter, false);
                            if ((pFeat_Conductor = pFCur_Conductor.NextFeature()) != null)
                            {
                                pSFMap.TO_LINE_FC_NAME = ((IDataset)pFeat_Conductor.Class).BrowseName;
                                pSFMap.TO_LINE_GLOBALID = Convert.ToString(pFeat_Conductor.get_Value(pFeat_Conductor.Fields.FindField("GLOBALID")));
                                pSFMap.TO_CIRCUITID = Convert.ToString(pFeat_Conductor.get_Value(pFeat_Conductor.Fields.FindField("CIRCUITID")));
                            }
                        }

                        //Finding BusBar connected to start UpStream Trace from Substation Element
                        pSFilter = new SpatialFilterClass();
                        pSFilter.Geometry = ((ITopologicalOperator)pFeat_EderStitch.Shape).Buffer(Convert.ToDouble(ConfigurationManager.AppSettings["STITCHPOINT_BUS_BUFFER"]));
                        pSFilter.GeometryField = pFC_All["SUB_BUSBAR_FC"].ShapeFieldName;
                        pSFilter.SpatialRel = esriSpatialRelEnum.esriSpatialRelIntersects;
                        pSFilter.SubFields = pFC_All["SUB_BUSBAR_FC"].OIDFieldName + "," + pFC_All["SUB_BUSBAR_FC"].ShapeFieldName;
                        pSFilter.WhereClause = "CIRCUITID IS NOT NULL";
                        pFCur_Conductor = pFC_All["SUB_BUSBAR_FC"].Search(pSFilter, false);
                        if ((pFeat_Conductor = pFCur_Conductor.NextFeature()) != null)
                            //Starting Upstream Trace from BusBar
                            Trace(pFeat_Conductor, ConfigurationManager.AppSettings["SUB_GEOMNETWORK"].ToUpper(), TraceDirection.Upstream);
                        //Confirming if ElectricStitchPoint is a Load (Subtypecd =2) or
                        //if 1 then checking for exceptional cases
                        //then adding mapping object to list
                        if (Convert.ToInt32(pFeat_EderStitch.get_Value(pFeat_EderStitch.Fields.FindField("SUBTYPECD"))) == 2 ||
                            (Convert.ToInt32(pFeat_EderStitch.get_Value(pFeat_EderStitch.Fields.FindField("SUBTYPECD"))) == 1 &&
                               pFeat_Conductor != null &&
                               !String.IsNullOrEmpty(pSFMap.TO_LINE_GLOBALID) &&
                               Convert.ToString(pSFMap.TO_LINE_GLOBALID) != Convert.ToString(pSFMap.FROM_LINE_GLOBALID)))
                            pList_SFMap.Add(pSFMap);
                        GC.Collect();
                    }
                }
            }
            catch (Exception ex)
            {
                WriteLine("PGE.BatchApplication.CircuitSource_Populate.Program.Main1 : Error : " + ex.Message);
            }
            finally
            {
                if (pFCur_Substation != null) Marshal.FinalReleaseComObject(pFCur_Substation);
                if (pFCur_Conductor != null) Marshal.FinalReleaseComObject(pFCur_Conductor);
                if (pFCur_Stitch != null) Marshal.FinalReleaseComObject(pFCur_Stitch);
                if (pFeat_Substation != null) Marshal.FinalReleaseComObject(pFeat_Substation);
                if (pFeat_EderStitch != null) Marshal.FinalReleaseComObject(pFeat_EderStitch);
                if (pFeat_Conductor != null) Marshal.FinalReleaseComObject(pFeat_Conductor);
                if (pSFilter != null) Marshal.FinalReleaseComObject(pSFilter);
            }
        }

        /// <summary>
        /// Method to get all Featureclasses at first
        /// from config file and respective Workspaces
        /// </summary>
        private static void GetRequiredClasses()
        {
            WriteLine("PGE.BatchApplication.CircuitSource_Populate.Program.GetRequiredClasses");
            // m4jf edgisrearch 919 - get sde files using Password management tool
            //pFWorskspace_EDER    = GetFWorkspace_SDE(ConfigurationManager.AppSettings["EDER_SDE_FILE"]);
           // pFWorskspace_EDERSUB = GetFWorkspace_SDE(ConfigurationManager.AppSettings["SUB_SDE_FILE"]);

            pFWorskspace_EDER = GetFWorkspace_SDE(ReadEncryption.GetSDEPath(ConfigurationManager.AppSettings["EDER_SDEConnection"].ToUpper()));
            pFWorskspace_EDERSUB = GetFWorkspace_SDE(ReadEncryption.GetSDEPath(ConfigurationManager.AppSettings["EDERSUB_SDEConnection"].ToUpper()));

            pFC_All.Add("EDER_ELECSTICHPOINT_FC", GetFeatureClass(ConfigurationManager.AppSettings["EDER_ELECSTICHPOINT_FC"]));
            pFC_All.Add("SUB_ELECSTICHPOINT_FC", GetFeatureClass(ConfigurationManager.AppSettings["SUB_ELECSTICHPOINT_FC"]));
            pFC_All.Add("SUB_BUSBAR_FC", GetFeatureClass(ConfigurationManager.AppSettings["SUB_BUSBAR_FC"]));
            pFC_All.Add("EDER_PRIOH_FC", GetFeatureClass(ConfigurationManager.AppSettings["EDER_PRIOH_FC"]));
            pFC_All.Add("EDER_PRIUG_FC", GetFeatureClass(ConfigurationManager.AppSettings["EDER_PRIUG_FC"]));
            pFC_All.Add("EDER_SUBSTATION_FC", GetFeatureClass(ConfigurationManager.AppSettings["EDER_SUBSTATION_FC"]));
            pFC_All.Add("SUB_OHCONDUCTOR_FC", GetFeatureClass(ConfigurationManager.AppSettings["SUB_OHCONDUCTOR_FC"]));
            pFC_All.Add("SUB_UGCONDUCTOR_FC", GetFeatureClass(ConfigurationManager.AppSettings["SUB_UGCONDUCTOR_FC"]));
            pFC_All.Add("SUB_TRANSFORMER_BANK_FC", GetFeatureClass(ConfigurationManager.AppSettings["SUB_TRANSFORMER_BANK_FC"]));
        }
        /// <summary>
        /// Method to get tables 
        /// </summary>
        private static void GetRequiredTables()
        {
            pTable_All.Add("PGE_FEEDERFEDNETWORK_TRACE_TBL", pFWorskspace_EDER.OpenTable(ConfigurationManager.AppSettings["PGE_FEEDERFEDNETWORK_TRACE_TBL"]));
        }

        /// <summary>
        /// Method to connect to SDE file
        /// </summary>
        /// <param name="connectionFile">SDE File path</param>
        /// <returns>FeatureWorkspace from SDE file</returns>
        private static IFeatureWorkspace GetFWorkspace_SDE(String connectionFile)
        {
            return (IFeatureWorkspace)((IWorkspaceFactory)Activator.CreateInstance(Type.GetTypeFromProgID("esriDataSourcesGDB.SdeWorkspaceFactory"))).
                OpenFromFile(connectionFile, 0);
        }

        /// <summary>
        /// Get FeatureClass
        /// </summary>
        /// <param name="sClassName">FeatureClass Name from Config</param>
        /// <returns>FeatureClass</returns>
        private static IFeatureClass GetFeatureClass(string sClassName)
        {
            return ((sClassName.ToUpper().StartsWith("EDGIS.SUB") && !sClassName.ToUpper().Contains("SUBSTATION")) ? pFWorskspace_EDERSUB : pFWorskspace_EDER).OpenFeatureClass(sClassName);
        }

        /// <summary>
        /// Write Log
        /// </summary>
        /// <param name="sMsg">Message to write</param>
        private static void WriteLine(string sMsg)
        {
            sMsg = !String.IsNullOrEmpty(sMsg) ? DateTime.Now.ToLongTimeString() + " -- " + sMsg : sMsg;
            pSWriter = File.AppendText(sPath_Log);
            pSWriter.WriteLine(sMsg);
            Console.WriteLine(sMsg);
            pSWriter.Close();
        }

        /// <summary>
        /// Trace Network
        /// Also poputate the Mapping object From's values
        /// </summary>
        /// <param name="pFeat_Start">Start Feature</param>
        /// <param name="sGeomNetworkName">Geometric Network Name</param>
        /// <param name="tracedirection">Trace Direction</param>
        private static void Trace(IFeature pFeat_Start, string sGeomNetworkName, TraceDirection tracedirection)
        {
            WriteLine("Trace Args: Feat OID " + pFeat_Start.OID.ToString() + " GeomNet Name " + sGeomNetworkName + " " + tracedirection);
            IMMElectricNetworkTracing         elecNetTracing           = default(IMMElectricNetworkTracing);
            IMMElectricTraceSettingsEx        elecTraceSettingsEx      = default(IMMElectricTraceSettingsEx);
            IEnumNetEID                       juncEIDs                 = default(IEnumNetEID),
                                              edgeEIDs                 = default(IEnumNetEID);
            IList<IMMNetworkFeatureID>        edgeFeatureIds           = new List<IMMNetworkFeatureID>(),
                                              juncFeatureIds           = new List<IMMNetworkFeatureID>();
            IMMNetworkAnalysisExtForFramework MMNetAnalExtForFramework = default(IMMNetworkAnalysisExtForFramework);
            IMMElectricTraceSettings          elecTraceSetttings       = default(IMMElectricTraceSettings);
            IEIDHelper                        pEIDHelper               = default(IEIDHelper);
            IEnumEIDInfo                      pEnumEIDInfo             = default(IEnumEIDInfo);
            IEIDInfo                          pEIDInfo                 = default(IEIDInfo);
            IFeature                          pFeat_Conductor          = default(IFeature),
                                              pFeat_TxBank             = default(IFeature);
            ISpatialFilter                    pSFilter                 = new SpatialFilterClass();
            IFeatureCursor                    pFCur_Conductor          = default(IFeatureCursor);
            ICursor                           pCur_FeederFedNet        = default(ICursor);
            IQueryFilter                      pQFilter                 = new QueryFilter();
            try
            {

                //Initialize Network Analysis Extension Framework object to assign Barriers.
                MMNetAnalExtForFramework = new MMNetworkAnalysisExtForFrameworkClass();
                if (edgeFeatureIds.Count > 0) MMNetAnalExtForFramework.EdgeBarriers = edgeFeatureIds.ToArray<IMMNetworkFeatureID>();
                if (juncFeatureIds.Count > 0) MMNetAnalExtForFramework.JunctionBarriers = juncFeatureIds.ToArray<IMMNetworkFeatureID>();

                //Initialize Object of Trace settings.
                elecTraceSetttings = new MMElectricTraceSettings();
                elecTraceSetttings.RespectConductorPhasing = true;
                elecTraceSetttings.RespectEnabledField = true;

                //Query Interface Trace Settings object to TraceSettingEx
                elecTraceSettingsEx = (IMMElectricTraceSettingsEx)elecTraceSetttings;
                elecTraceSettingsEx.UseFeederManagerCircuitSources = true;
                elecTraceSettingsEx.RespectESRIBarriers = true;

                elecNetTracing = new MMFeederTracerClass();
                juncEIDs = default(IEnumNetEID);
                edgeEIDs = default(IEnumNetEID);

                //Tracing 
                if (tracedirection == TraceDirection.Downstream)
                    elecNetTracing.TraceDownstream(geomNetworks[sGeomNetworkName], MMNetAnalExtForFramework, elecTraceSettingsEx, (pFeat_Start.Shape.GeometryType == esriGeometryType.esriGeometryPoint) ? ((ISimpleJunctionFeature)pFeat_Start).EID : ((ISimpleEdgeFeature)pFeat_Start).EID,
                        ((pFeat_Start.Shape.GeometryType == esriGeometryType.esriGeometryPoint) ? esriElementType.esriETJunction : esriElementType.esriETEdge),
                        mmPhasesToTrace.mmPTT_Any, out juncEIDs, out edgeEIDs);
                else if (tracedirection == TraceDirection.Upstream)
                    elecNetTracing.TraceUpstream(geomNetworks[sGeomNetworkName], MMNetAnalExtForFramework, elecTraceSettingsEx, (pFeat_Start.Shape.GeometryType == esriGeometryType.esriGeometryPoint) ? ((ISimpleJunctionFeature)pFeat_Start).EID : ((ISimpleEdgeFeature)pFeat_Start).EID,
                        ((pFeat_Start.Shape.GeometryType == esriGeometryType.esriGeometryPoint) ? esriElementType.esriETJunction : esriElementType.esriETEdge),
                        mmPhasesToTrace.mmPTT_Any, out juncEIDs, out edgeEIDs);
                else if (tracedirection == TraceDirection.Connected)
                    elecNetTracing.DistributionCircuitTrace(geomNetworks[sGeomNetworkName], MMNetAnalExtForFramework, elecTraceSettingsEx, (pFeat_Start.Shape.GeometryType == esriGeometryType.esriGeometryPoint) ? ((ISimpleJunctionFeature)pFeat_Start).EID : ((ISimpleEdgeFeature)pFeat_Start).EID,
                    ((pFeat_Start.Shape.GeometryType == esriGeometryType.esriGeometryPoint) ? esriElementType.esriETJunction : esriElementType.esriETEdge),
                    mmPhasesToTrace.mmPTT_Any, out juncEIDs, out edgeEIDs);

                pEIDHelper = new EIDHelperClass()
                {
                    GeometricNetwork = geomNetworks[sGeomNetworkName],
                    ReturnGeometries = true,
                    ReturnFeatures = true
                };
                pEnumEIDInfo = pEIDHelper.CreateEnumEIDInfo(juncEIDs);
                pEIDInfo = default(IEIDInfo);
                Decimal dTreeLevelMax = 0;
                int iCount = 0;
                //Looping through all trace features
                while ((pEIDInfo = pEnumEIDInfo.Next()) != null)
                {
                    //setting Mapping object properties for From's Cosnductor, SubElectricStichpoint
                    if (((IDataset)pEIDInfo.Feature.Class).Name == ((IDataset)pFC_All["SUB_ELECSTICHPOINT_FC"]).BrowseName)
                    {
                        pSFMap.FROM_POINT_FC_NAME = ((IDataset)pFC_All["SUB_ELECSTICHPOINT_FC"]).BrowseName;
                        pSFMap.FROM_POINT_FC_GUID = Convert.ToString(pFC_All["SUB_ELECSTICHPOINT_FC"].GetFeature(pEIDInfo.Feature.OID).get_Value(pFC_All["SUB_ELECSTICHPOINT_FC"].FindField("GLOBALID")));

                        pSFilter = new SpatialFilterClass();
                        pSFilter.Geometry = pEIDInfo.Geometry;
                        pSFilter.GeometryField = pFC_All["EDER_PRIUG_FC"].ShapeFieldName;
                        pSFilter.SearchOrder = esriSearchOrder.esriSearchOrderSpatial;
                        pSFilter.SpatialRel = esriSpatialRelEnum.esriSpatialRelTouches;
                        pSFilter.SubFields = "CIRCUITID,GLOBALID";

                        if ((pFeat_Conductor = pFC_All["EDER_PRIUG_FC"].Search(pSFilter, false).NextFeature()) != null)
                        {
                            pSFMap.FROM_LINE_FC_NAME = ((IDataset)pFeat_Conductor.Class).BrowseName;
                            pSFMap.FROM_LINE_GLOBALID = Convert.ToString(pFeat_Conductor.get_Value(pFeat_Conductor.Fields.FindField("GLOBALID")));
                            pSFMap.FROM_CIRCUITID = Convert.ToString(pFeat_Conductor.get_Value(pFeat_Conductor.Fields.FindField("CIRCUITID")));
                        }
                        else if ((pFeat_Conductor = pFC_All["EDER_PRIOH_FC"].Search(pSFilter, false).NextFeature()) != null)
                        {
                            pSFMap.FROM_LINE_FC_NAME = ((IDataset)pFeat_Conductor.Class).BrowseName;
                            pSFMap.FROM_LINE_GLOBALID = Convert.ToString(pFeat_Conductor.get_Value(pFeat_Conductor.Fields.FindField("GLOBALID")));
                            pSFMap.FROM_CIRCUITID = Convert.ToString(pFeat_Conductor.get_Value(pFeat_Conductor.Fields.FindField("CIRCUITID")));
                        }
                        else if ((pFeat_Conductor = pFC_All["SUB_OHCONDUCTOR_FC"].Search(pSFilter, false).NextFeature()) != null)
                        {
                            pSFMap.FROM_LINE_FC_NAME = ((IDataset)pFeat_Conductor.Class).BrowseName;
                            pSFMap.FROM_LINE_GLOBALID = Convert.ToString(pFeat_Conductor.get_Value(pFeat_Conductor.Fields.FindField("GLOBALID")));
                            pSFMap.FROM_CIRCUITID = Convert.ToString(pFeat_Conductor.get_Value(pFeat_Conductor.Fields.FindField("CIRCUITID")));
                        }
                        else if ((pFeat_Conductor = pFC_All["SUB_UGCONDUCTOR_FC"].Search(pSFilter, false).NextFeature()) != null)
                        {
                            pSFMap.FROM_LINE_FC_NAME = ((IDataset)pFeat_Conductor.Class).BrowseName;
                            pSFMap.FROM_LINE_GLOBALID = Convert.ToString(pFeat_Conductor.get_Value(pFeat_Conductor.Fields.FindField("GLOBALID")));
                            pSFMap.FROM_CIRCUITID = Convert.ToString(pFeat_Conductor.get_Value(pFeat_Conductor.Fields.FindField("CIRCUITID")));
                        }
                        else if ((pFCur_Conductor = pFC_All["SUB_BUSBAR_FC"].Search(pSFilter, false)) != null)
                        {
                            while ((pFeat_Conductor = pFCur_Conductor.NextFeature()) != null)
                            {
                                if (!string.IsNullOrEmpty(Convert.ToString(pFeat_Conductor.get_Value(pFeat_Conductor.Fields.FindField("CIRCUITID")))))
                                    continue;
                                pSFMap.FROM_LINE_FC_NAME = ((IDataset)pFeat_Conductor.Class).BrowseName;
                                pSFMap.FROM_LINE_GLOBALID = Convert.ToString(pFeat_Conductor.get_Value(pFeat_Conductor.Fields.FindField("GLOBALID")));
                                pSFMap.FROM_CIRCUITID = Convert.ToString(pFeat_Conductor.get_Value(pFeat_Conductor.Fields.FindField("CIRCUITID")));
                                break;
                            }
                        }
                        //break;
                    }
                    //Getting TransformerBank Information
                    else if (((IDataset)pEIDInfo.Feature.Class).Name == ((IDataset)pFC_All["SUB_TRANSFORMER_BANK_FC"]).BrowseName)
                    {
                        pFeat_TxBank = pFC_All["SUB_TRANSFORMER_BANK_FC"].GetFeature(pEIDInfo.Feature.OID);
                        iCount++;
                        if (iCount == 1)
                        {
                            pSFMap.TX_BANK_GLOBALID = Convert.ToString(pFeat_TxBank.get_Value(pFeat_TxBank.Fields.FindField("GLOBALID")));
                            pSFMap.TX_BANK_CODE = Convert.ToString(pFeat_TxBank.get_Value(pFeat_TxBank.Fields.FindField("BANKCD")));
                        }
                        else
                        {
                            string sGlobalid_TX_BANK = Convert.ToString(pFeat_TxBank.get_Value(pFeat_TxBank.Fields.FindField("GLOBALID")));
                            pQFilter = new QueryFilter();
                            pQFilter.SubFields = "TO_FEATURE_GLOBALID, TREELEVEL";
                            pQFilter.WhereClause = "TO_FEATURE_GLOBALID IN ('" + sGlobalid_TX_BANK + "', '" + pSFMap.TX_BANK_GLOBALID + "')";
                            pCur_FeederFedNet = pTable_All["PGE_FEEDERFEDNETWORK_TRACE_TBL"].Search(pQFilter, false);
                            IRow pRow = pCur_FeederFedNet.NextRow();
                            dTreeLevelMax = Convert.ToDecimal(pRow.get_Value(pTable_All["PGE_FEEDERFEDNETWORK_TRACE_TBL"].FindField("TREELEVEL")));
                            while (pRow != null)
                            {
                                decimal dTreeLevel = Convert.ToDecimal(pRow.get_Value(pTable_All["PGE_FEEDERFEDNETWORK_TRACE_TBL"].FindField("TREELEVEL")));
                                if (dTreeLevel > dTreeLevelMax && pSFMap.TX_BANK_GLOBALID != Convert.ToString(pRow.get_Value(pTable_All["PGE_FEEDERFEDNETWORK_TRACE_TBL"].FindField("TO_FEATURE_GLOBALID"))))
                                {
                                    WriteLine("Previous Tree Level = " + dTreeLevelMax.ToString());
                                    WriteLine("Previous GOBALID  = " + pSFMap.TX_BANK_GLOBALID);
                                    WriteLine("Previous BANK CODE  = " + pSFMap.TX_BANK_CODE);
                                    dTreeLevelMax = dTreeLevel;
                                    pSFMap.TX_BANK_GLOBALID = Convert.ToString(pFeat_TxBank.get_Value(pFeat_TxBank.Fields.FindField("GLOBALID")));
                                    pSFMap.TX_BANK_CODE = Convert.ToString(pFeat_TxBank.get_Value(pFeat_TxBank.Fields.FindField("BANKCD")));
                                    WriteLine("Tree Level of TX Bank # " + iCount.ToString() + " = " + dTreeLevel.ToString());
                                    WriteLine("Updated GOBALID = " + pSFMap.TX_BANK_GLOBALID);
                                    WriteLine("Updated BANK CODE  = " + pSFMap.TX_BANK_CODE);
                                    WriteLine(string.Empty);
                                }
                                pRow = pCur_FeederFedNet.NextRow();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                WriteLine("PGE.BatchApplication.CircuitSource_Populate.Program.Trace : Error : " + ex.Message);
            }
            finally
            {
                if (elecNetTracing != null) Marshal.FinalReleaseComObject(elecNetTracing);
                if (elecTraceSettingsEx != null) Marshal.FinalReleaseComObject(elecTraceSettingsEx);
                if (juncEIDs != null) Marshal.FinalReleaseComObject(juncEIDs);
                if (edgeEIDs != null) Marshal.FinalReleaseComObject(edgeEIDs);
                if (MMNetAnalExtForFramework != null) Marshal.FinalReleaseComObject(MMNetAnalExtForFramework);
                if (elecTraceSetttings != null) Marshal.FinalReleaseComObject(elecTraceSetttings);
                if (pEIDHelper != null) Marshal.FinalReleaseComObject(pEIDHelper);
                if (pEnumEIDInfo != null) Marshal.FinalReleaseComObject(pEnumEIDInfo);
                if (pEIDInfo != null) Marshal.FinalReleaseComObject(pEIDInfo);
                if (pFeat_Conductor != null) Marshal.FinalReleaseComObject(pFeat_Conductor);
                if (pSFilter != null) Marshal.FinalReleaseComObject(pSFilter);
                if (pFCur_Conductor != null) Marshal.FinalReleaseComObject(pFCur_Conductor);
                if (pFeat_TxBank != null) Marshal.FinalReleaseComObject(pFeat_TxBank);
                if (pCur_FeederFedNet != null) Marshal.FinalReleaseComObject(pCur_FeederFedNet);
                if (pQFilter != null) Marshal.FinalReleaseComObject(pQFilter);
            }
        }

        /// <summary>
        /// List of Geomtric Networks
        /// </summary>
        private static Dictionary<string, IGeometricNetwork> geomNetworks = new Dictionary<string, IGeometricNetwork>();

        /// <summary>
        /// Get Network information for Trace available in the Workspace
        /// </summary>
        /// <param name="ws">Workspace used for Tracing</param>
        private static void GetNetworks(IWorkspace ws)
        {
            //IGeometricNetwork geomNetwork = null;
            IEnumDataset enumDataset = ws.get_Datasets(esriDatasetType.esriDTFeatureDataset);
            enumDataset.Reset();

            IDataset dsName = enumDataset.Next();
            while (dsName != null)
            {
                IFeatureDataset featureDataset = dsName as IFeatureDataset;
                if (featureDataset != null)
                {
                    IEnumDataset geomDatasets = featureDataset.Subsets;
                    geomDatasets.Reset();
                    IDataset ds = null;
                    while ((ds = geomDatasets.Next()) != null)
                    {
                        if (ds is IGeometricNetwork)
                        {
                            geomNetworks.Add(ds.BrowseName.ToUpper(), ds as IGeometricNetwork);
                        }
                    }
                }
                dsName = enumDataset.Next();
            }

            if (geomNetworks.Count > 0)
            {
                return;
            }
            else
            {
                throw new Exception("Could not find the geometric networks in the specified database");
            }
        }

        /// <summary>
        /// ArcFM Licence Initializer
        /// </summary>
        /// <param name="productCode">Code to checkout</param>
        /// <returns>Licence Status</returns>
        private static mmLicenseStatus CheckOutLicenses(mmLicensedProductCode productCode)
        {
            mmLicenseStatus licenseStatus;
            licenseStatus = arcFMAppInitialize.IsProductCodeAvailable(productCode);
            if (licenseStatus == mmLicenseStatus.mmLicenseAvailable)
            {
                licenseStatus = arcFMAppInitialize.Initialize(productCode);
            }
            return licenseStatus;
        }
    }

    /// <summary>
    /// Mapping Class
    /// </summary>
    class SourceFeederMapping
    {
        public string SUBSTATION_NAME { get; set; }
        public string SUBSTATION_GLOBALID { get; set; }
        public string TO_LINE_FC_NAME { get; set; }
        public string TO_LINE_GLOBALID { get; set; }
        public string TO_CIRCUITID { get; set; }
        public string FROM_LINE_FC_NAME { get; set; }
        public string FROM_LINE_GLOBALID { get; set; }
        public string FROM_CIRCUITID { get; set; }
        public string FROM_POINT_FC_NAME { get; set; }
        public string FROM_POINT_FC_GUID { get; set; }
        public string TO_POINT_FC_NAME { get; set; }
        public string TO_POINT_FC_GUID { get; set; }
        public string TX_BANK_GLOBALID { get; set; }
        public string TX_BANK_CODE { get; set; }
    }
}
