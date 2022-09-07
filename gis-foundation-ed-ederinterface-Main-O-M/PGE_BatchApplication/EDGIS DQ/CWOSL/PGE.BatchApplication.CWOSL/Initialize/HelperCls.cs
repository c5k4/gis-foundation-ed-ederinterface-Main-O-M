using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.ADF;
using ESRI.ArcGIS.DataSourcesGDB;
using ESRI.ArcGIS.esriSystem;
using System.Collections;
using System.Configuration;
using System.Runtime.InteropServices;
using Miner.Interop;
using Miner.Interop.Process;
using Miner.Process.GeodatabaseManager.Subtasks;
//using Telvent.Delivery.Diagnostics;
using System.Data.OleDb;
using System.Data;
namespace PGE.BatchApplication.CWOSL
{
    class HelperCls
    {
        #region Coded Values
        public const string TransformerType_OH = "OH";
        public const string PseudoServiceSubtype_OH = "5";
        public const string PseudoServiceSubtype_UG = "6";
        public const string ServiceSubtype_UG = "3";
        public const string StatusDefault = "5";
        public const string JobNumberDefault = "0";
        public const string TransformerSubTypeNetwork = "5";
        public const string SubType_SL = "1";
        #endregion
        #region Geocoder Values

        public const string geocoderValueUnmatched = "U";
        public const string geocoderValueMatched = "M";
        public const string geocoderValueTied = "T";

        public const string geocoderFieldStatus = "Status";
        public const string geocoderFieldScore = "score";
        public const string geocoderFieldShape = "shape";
        public const string geocoderFieldMatchingAddress = "matchingAddress";
        public const string geocoderFieldLocatorType = "locatorType";

        #endregion

        #region Parcel Address Fields

        public const string Parcel_SITE_ADDRESS = "SITE_ADDRESS";
        public const string Parcel_PROPERTY_CITY = "PROPERTY_CITY";
        public const string Parcel_PROPERTY_STATE = "PROPERTY_STATE";
        public const string Parcel_PROPERTY_ZIPCODE = "PROPERTY_ZIPCODE";

        #endregion

        #region Common Field Names

        public const string CGC12_FldName = "cgc12";
        public const string SubTypeCD_FldName = "SUBTYPECD";
        public const string GlobalID_FldName = "GlobalID";
        public const string TransformerType_FldName = "InstallationType";
        public const string CircuitID_FldName = "CircuitID";
        public const string STATUS_FldName = "STATUS";
        public const string INSTALLJOBNUMBER_FldName = "INSTALLJOBNUMBER";
        public const string SERVICEPOINTID_FldName = "SERVICEPOINTID";
        public const string PHASEDESIGNATION_FldName = "PHASEDESIGNATION";

        #endregion

        #region Feature Class Names

        public const string FC_Landbase_ParcelsName = "LBGIS.parcels";
        public const string FC_Landbase_LotLinesName = "LBGIS.LotLines";
        public const string FC_SecondaryLoadPointName = "EDGIS.SecondaryLoadPoint";
        public const string FC_ServicePointName = "EDGIS.ServicePoint";
        public const string FC_ServiceLocationName = "EDGIS.ServiceLocation";
        public const string FC_SecOHConductofName = "EDGIS.SecOHConductor";
        public const string FC_SecUGConductofName = "EDGIS.SecUGConductor";
        public const string RelCls_ServiceLocation_ServicePointName = "EDGIS.ServiceLocation_ServicePoint";
        public const string RelCls_SecLoadPoint_ServicePointName = "EDGIS.SecLoadPoint_ServicePoint";
        public const string FC_TransformerName = "EDGIS.TRANSFORMER";

        #endregion
        private static readonly Log4NetLoggerCWOSL _logger = new Log4NetLoggerCWOSL(MethodBase.GetCurrentMethod().DeclaringType, "CWOSLDefault.log4net.config");

        #region Cursor/Row related function
        public static string GetValue(IRow pRow, int intFldIndex)
        {
            return GetValue(pRow, intFldIndex, null);
        }

        public static string GetValue(IRow pRow, int intFldIndex, string strFieldName)
        {
            try
            {
                if (intFldIndex == -1)
                {
                    intFldIndex = pRow.Fields.FindField(strFieldName);
                }
                if (intFldIndex == -1)
                    throw new Exception("Missing field name: " + strFieldName);

                if (pRow.get_Value(intFldIndex) != System.DBNull.Value)
                {
                    object obj = pRow.get_Value(intFldIndex);
                    string strValue = null;
                    if (obj != null)
                    {
                        strValue = pRow.get_Value(intFldIndex).ToString();
                    }
                    if (!string.IsNullOrEmpty(strValue))
                        return strValue;
                }
                return null;
            }
            catch (Exception ex)
            {
                _logger.Error(System.Reflection.MethodInfo.GetCurrentMethod().Name + ". Field Name: " + strFieldName + "," + ex.Message);
                return null;
            }
        }
        public static bool GetCursor(out ICursor cur, ITable pTable, string strWhereClause, string fields = "*")
        {
            return GetCursor(out cur, pTable, strWhereClause, false, fields);
        }
        public static bool GetCursor(out ICursor cur, ITable pTable, string strWhereClause, bool blnRecycleCursor, string fields = "*")
        {
            //_logger.Debug("");
            cur = null;
            try
            {
                //if (strWhereClause.Length == 0)
                //    return false;   
                IQueryFilter pQFilter = new QueryFilter();
                pQFilter.WhereClause = strWhereClause;
                pQFilter.SubFields = fields;
                cur = pTable.Search(pQFilter, blnRecycleCursor);
                return true;
            }
            catch (Exception ex)
            { _logger.Error(ex.ToString()); return false; }
        }
        internal static bool GetFeatureCursor(out IFeatureCursor featCursor, IFeatureClass pFC, string strWhereClause)
        {
            featCursor = null;
            try
            {
                if (strWhereClause.Length == 0)
                    return false;
                IQueryFilter pQFilter = new QueryFilter();
                pQFilter.WhereClause = strWhereClause;
                featCursor = pFC.Search(pQFilter, false);
                return true;
            }
            catch (Exception ex)
            { _logger.Error(ex.ToString()); return false; }
        }
        internal static IFeatureCursor SpatialQuery(IFeatureClass featureClassIN, IGeometry searchGeo, esriSpatialRelEnum spatialRelation, string whereClause)
        {
            try
            {
                string strShpFld;
                IFeatureCursor featCur;
                IQueryFilter queryFilter;

                //Set the search geometry and shapefieldname.
                ISpatialFilter spatialFilter = new SpatialFilter();
                spatialFilter.Geometry = searchGeo;
                strShpFld = featureClassIN.ShapeFieldName;
                spatialFilter.GeometryField = strShpFld;

                spatialFilter.SpatialRel = spatialRelation;
                //If description != "" Then pSpatialFilter.SpatialRelDescription = description;
                if (!string.IsNullOrEmpty(whereClause)) spatialFilter.WhereClause = whereClause;

                //pSpatialFilter.SearchOrder = esriSearchOrderSpatial;
                queryFilter = spatialFilter;
                featCur = featureClassIN.Search(queryFilter, false);
                return (IFeatureCursor)featCur;
            }
            catch (Exception ex)
            { _logger.Error(ex.ToString()); return null; }
        }
        //Changes from 1368195 Start
        public static bool GetCursorQueryDef(out ICursor cur, ITable pTable, string strWhereClause, string fields = "*")
        {
            cur = null;
            try
            {

                Type t = null;
                IWorkspaceFactory workspaceFactory = null;
                IWorkspace workspace = null;
                t = Type.GetTypeFromProgID("esriDataSourcesGDB.SdeWorkspaceFactory");
                workspaceFactory = (IWorkspaceFactory)Activator.CreateInstance(t);
                workspace = workspaceFactory.OpenFromFile("C:\\Users\\k4bj\\AppData\\Roaming\\ESRI\\Desktop10.2\\ArcCatalog\\edgist1d@sde.sde", 0);
                IFeatureWorkspace featureworkspace = (IFeatureWorkspace)workspace;
                IQueryDef queryDef = (IQueryDef)featureworkspace.CreateQueryDef();
                queryDef.Tables = "EDGIS.ServicePoint,EDGIS.ServiceLocation,EDGIS.Transformer";
                queryDef.SubFields = "*";
                queryDef.WhereClause = "EDGIS.Transformer.GLOBALID = EDGIS.ServicePoint.TRANSFORMERGUID";
                //EDGIS.ServicePoint.servicelocationguid = EDGIS.ServiceLocation.globalid AND EDGIS.ServicePoint.ServiceLocationGUID is null
                cur = queryDef.Evaluate();
                return true;
            }
            catch (Exception ex)
            { _logger.Error(ex.ToString()); return false; }
        }
        public static bool GetData(string sqlSP1, out DataTable Dt, string connstring)
        {
            Dt = null;
            try
            {
                string ConnString = connstring; //"provider=OraOLEDB.Oracle;Data Source=edgisq9q;User ID=edgis;Password=edgis!Q9Qu";
                Dt = new DataTable();
                using (OleDbConnection conn = new OleDbConnection(ConnString))
                {
                    conn.Open();
                    string sql = sqlSP1;
                    OleDbCommand cmd = new OleDbCommand(sql, conn);
                    OleDbDataAdapter adapter = new OleDbDataAdapter(cmd);
                    adapter.Fill(Dt);
                    OleDbDataReader o_dr = cmd.ExecuteReader();
                    conn.Close();
                }
                return true;
            }
            catch
            {
                return false;
            }
        }
        //Changes from 1368195 Stop
        //1368195 Change in protection level in function
        public static IWorkspace OpenSDEWorkspacefromsdefile(string sdefilenamewithpath)
        {
            Type t = null;
            IWorkspaceFactory workspaceFactory = null;
            IWorkspace workspace = null;
            try
            {

                t = Type.GetTypeFromProgID("esriDataSourcesGDB.SdeWorkspaceFactory");
                workspaceFactory = (IWorkspaceFactory)Activator.CreateInstance(t);
                workspace = workspaceFactory.OpenFromFile(sdefilenamewithpath, 0);
            }
            catch (Exception exp)
            {
            }
            return workspace;

        }
        public static void EnsureFolderExists(string folder, bool ensureIsWriteable = false)
        {
            _logger.Debug("folder [ " + folder + " ]");

            if (!System.IO.Directory.Exists(folder))
            {
                _logger.Warn("Folder [ " + folder + " ] doesn't exist -- creating");
                DirectoryInfo di = Directory.CreateDirectory(folder);
            }

            if (ensureIsWriteable)
            {
                _logger.Debug("Testing writeability...");
                string TEST_FILE = DateTime.Now.ToString("yyyy_MM_dd_ss") + ".txt";
                File.WriteAllText(TEST_FILE, "badgers");
                File.Delete(TEST_FILE);
            }
        }
        public static void SetFldVl(IRow pRow, string strFieldName, string strValue, bool blnStore, int intFldIndex)
        {
            try
            {
                if (intFldIndex == -1)
                    intFldIndex = pRow.Fields.FindField(strFieldName);
                if (intFldIndex == -1)
                    throw new Exception("Missing field name: " + strFieldName);

                if (strValue != null && strValue != "")
                {
                    pRow.set_Value(intFldIndex, strValue);
                }
                else
                {
                    pRow.set_Value(intFldIndex, System.DBNull.Value);
                }
                if (blnStore)
                    pRow.Store();
            }
            catch (Exception ex)
            { _logger.Error(System.Reflection.MethodInfo.GetCurrentMethod().Name + ". Field Name: " + strFieldName + ", Field Value: " + strValue + ". Err Msg: " + ex.Message); }
        }
        public static void GetFldIdx(ITable table, string strFieldName, out int intFldIndex)
        {
            intFldIndex = -1;
            try
            {
                ITable pPassedTable = null;
                if (table != null)
                    pPassedTable = table as ITable;
                //else if (pTable != null)
                //    pPassedTable = pTable;
                else
                    throw new Exception("Null ObjectClass.");

                intFldIndex = pPassedTable.Fields.FindField(strFieldName);
                if (intFldIndex == -1)
                    throw new Exception("Missing field name: " + strFieldName);
            }
            catch (Exception ex)
            { _logger.Error(System.Reflection.MethodInfo.GetCurrentMethod().Name + ". Field Name: " + strFieldName + ". Err Msg: " + ex.Message); }
        }
        #endregion

        #region Edit Related Function
        public static void EnsureInEditSession(IWorkspace workspace)
        {
            _logger.Debug(MethodBase.GetCurrentMethod().Name);
            if (workspace == null) return;

            if (!((IWorkspaceEdit)workspace).IsBeingEdited())
            {
                ((IWorkspaceEdit)workspace).StartEditing(false);
            }
        }
        public static void EnsureCloseEditSession(IWorkspace workspace, bool saveEdits = true)
        {
            _logger.Debug(MethodBase.GetCurrentMethod().Name);
            if (workspace == null) return;

            if (((IWorkspaceEdit)workspace).IsBeingEdited())
            {
                ((IWorkspaceEdit)workspace).StopEditing(saveEdits);
            }
        }
        public static void StartEditOperation(IWorkspace workspace)
        {
            _logger.Debug(MethodBase.GetCurrentMethod().Name);
            if (workspace == null) return;

            if (((IWorkspaceEdit)workspace).IsBeingEdited())
                ((IWorkspaceEdit)workspace).StartEditOperation();
        }
        public static void StopEditOperation(IWorkspace workspace)
        {
            _logger.Debug(MethodBase.GetCurrentMethod().Name);
            if (workspace == null) return;

            if (((IWorkspaceEdit)workspace).IsBeingEdited())
                ((IWorkspaceEdit)workspace).StopEditOperation();
        }
        #endregion

        #region validation chk for Upstream transformer
        //Find Upstream transformer from input point and validate it matches with processing servicepoint feeding transformer
        //false means Upstream not matches,true means Upstream matches with processing servicepoint feeding transformer
        public static Boolean findFeedTrans(IPoint Servicept, string servicePointcgc12, string servicePointOID, IGeometricNetwork geoNetwork, out IPoint transformerShape)
        {
            transformerShape = null;
            bool bolMatchWithFeedTrans = false;
            bool found = false;
            IMMFeedPath[] feedPaths = new IMMFeedPath[0];
            IMMFeedPath feedPath;
            string strUpstream_Xfmr_CedsaDeviceId = string.Empty;
            string strUpstream_Xfmr_CGC12 = string.Empty;
            IFeature pFeat = null;
            try
            {

                IEnumFeature pEnum = null;
                pEnum = geoNetwork.SearchForNetworkFeature(Servicept, esriFeatureType.esriFTSimpleJunction);
                pEnum.Reset();
                pFeat = pEnum.Next();

                if (pFeat == null)
                {
                    bolMatchWithFeedTrans = false;
                    return bolMatchWithFeedTrans;
                }

                //get feedpaths from inpur feature
                feedPaths = TraceMM2(pFeat, geoNetwork);

                if (feedPaths.Count() != 0)
                {
                    int intPathCnt = feedPaths.Count();
                    //iterate feedpaths until get first Transformer
                    for (int feedPath_ct = 0; feedPath_ct <= feedPaths.Count() - 1; ++feedPath_ct)
                    {
                        if (found == true)
                            break;
                        feedPath = feedPaths[feedPath_ct];
                        for (int ele_ct = 1; ele_ct < feedPath.PathElements.Count(); ++ele_ct)
                        {
                            IMMPathElement pathElement = feedPath.PathElements[ele_ct];
                            if (pathElement.ElementType == esriElementType.esriETJunction)
                            {
                                IFeature junction = GetFeature(pathElement.EID, esriElementType.esriETJunction, geoNetwork);
                                if (((IDataset)junction.Class).Name.ToUpper() == FC_TransformerName)
                                {
                                    found = true;
                                    int CGC12FldIdx = -1;
                                    GetFldIdx(Initialize.TransformerFC as ITable, CGC12_FldName, out CGC12FldIdx);

                                    strUpstream_Xfmr_CGC12 = GetValue(junction, CGC12FldIdx);
                                    if (strUpstream_Xfmr_CGC12 == servicePointcgc12)
                                    {
                                        bolMatchWithFeedTrans = true;
                                        ele_ct = feedPath.PathElements.Count();
                                        transformerShape = (IPoint)junction.ShapeCopy;//return transformer shape.
                                    }
                                    break;//Found the transformer, so break.
                                }
                            }
                        }
                    }
                    //Report conductor if feeding Transformer not found  
                    if (found == false)
                    {
                        _logger.Info("Processing Servicepoint_OID:" + servicePointOID + ".No transformer found in Upstream from X:" + Servicept.X + ",Y:" + Servicept.Y);
                    }
                }
                else
                {
                    _logger.Info("Processing Servicepoint_OID:" + servicePointOID + ".No feedpaths returned by upstream trace.from X:" + Servicept.X + ",Y:" + Servicept.Y);
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Error on findFeedTrans. Processing Servicepoint_OID:" + servicePointOID + "  " + ex.Message);
            }
            return bolMatchWithFeedTrans;
        }

        private static IFeature GetFeature(int eid, esriElementType elType, IGeometricNetwork geoNetwork)
        {
            if (eid == 0) return null;

            INetElements netElements = (INetElements)geoNetwork.Network;

            int classID, userID, subID;
            netElements.QueryIDs(eid, elType, out classID, out userID, out subID);

            IFeatureClass fclass = ((IFeatureClassContainer)geoNetwork).get_ClassByID(classID);

            //Logs.writeLine("FeatureClass:" + fclass.AliasName + ":," + userID.ToString() + "EleType:" + elType.ToString() + ",Eid:" + eid.ToString());
            return fclass.GetFeature(userID);
        }

        private static IMMFeedPath[] TraceMM2(IFeature pFeat, IGeometricNetwork geoNetwork)
        {
            ISimpleJunctionFeature pSimJun = (ISimpleJunctionFeature)pFeat;
            int this_EID = pSimJun.EID;

            int[] barrierJncts = new int[0];
            int[] barrierEdges = new int[0];
            IMMFeedPath[] feedPath = new IMMFeedPath[0];
            IMMCurrentStatus currentStatus = null;
            IMMTraceStopper[] traceStopperJunctions = null;
            IMMTraceStopper[] traceStopperEdges = null;
            IMMElectricTraceSettings settings = new MMElectricTraceSettings();
            settings.RespectEnabledField = true;
            IMMElectricTracing elecTrace = new MMFeederTracerClass();
            //
            try
            {
                // New
                elecTrace.FindFeedPaths(
                    geoNetwork,
                    settings,
                    currentStatus,
                    this_EID,
                    esriElementType.esriETJunction,
                    SetOfPhases.abc,
                    barrierJncts,
                    barrierEdges,
                    out feedPath,
                    out traceStopperJunctions,
                    out traceStopperEdges);
            }
            catch (Exception ex)
            {
                _logger.Error("Error on TraceMM2  " + pFeat.Class.AliasName + ":" + pFeat.get_Value(pFeat.Fields.FindField("GLOBALID")).ToString() + " " + ex.Message);
                return feedPath;
            }
            return feedPath;
        }

        #endregion

        public static double GetPointDistance(IPoint p1, IPoint p2)
        {
            return ((IProximityOperator)p1).ReturnDistance(p2);
        }
        public static bool ApplicationHasExceededMaxMinutesToRun(DateTime appStart, int maxMinutesToRun)
        {
            try
            {
                if (maxMinutesToRun > 0 && (DateTime.Now - appStart).TotalMinutes >= maxMinutesToRun)
                {
                    _logger.Info("Application has exceeded max minutes to run.");
                    _logger.Info("App start time: " + Initialize.AppStartDateTime.ToString());
                    _logger.Info("Max minutes to run: " + maxMinutesToRun.ToString());
                    return true;
                }
            }
            catch (Exception ex)
            { _logger.Error(System.Reflection.MethodInfo.GetCurrentMethod().Name + ". Err Msg: " + ex.Message); }
            return false;
        }
        public static bool GetClosestFeatureToPoint(IProximityOperator proxOp, IGeometry geometryTo, ref double closestDistanceToBeat)
        {
            try
            {
                //Check if the passed geometry is closer thatn the passed distance.  
                //IProximityOperator proxOp = (IProximityOperator)pointToSearchFrom;
                double currentDistance = 0;
                currentDistance = proxOp.ReturnDistance(geometryTo);
                if (currentDistance < closestDistanceToBeat)
                {
                    closestDistanceToBeat = currentDistance;
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            { _logger.Error(System.Reflection.MethodInfo.GetCurrentMethod().Name + ". Err Msg: " + ex.Message); }
            return false;
        }

    }

}
