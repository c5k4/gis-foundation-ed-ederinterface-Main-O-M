using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Runtime.Serialization.Json;
using System.IO;
using PGE.Common.Delivery.Diagnostics;
using PGE.Common.Delivery.Framework;
using System.Reflection;
using ESRI.ArcGIS.ArcMapUI;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.DataSourcesRaster;
using ESRI.ArcGIS.Framework;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.Geoprocessing;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.esriSystem;
using Miner.Geodatabase.Edit;
using Miner.Interop;
using System.Runtime.InteropServices;

namespace PGE.Desktop.EDER.PLC
{
    public class StartAnalysisCall
    {
        private static readonly Log4NetLogger _logger = new Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "EDERDesktop.log4net.config");
        static Double _returnedPLDID;
        double _dLongitude = 0;
        double _dLatitude = 0;
        IApplication _app = null;
        IIdentify _pIdentify = null;
        public String _pldbidColName = String.Empty;
        IFeature _newFeatureSA = null;
        static String SLookupTable = "EDGIS.PGE_PLC_CONFIG";

        public double Createpldbid(double X, double Y, IFeature iFeat, String strJobnumber)
        {
            double pldbid = 0.0;
            IRow codeRow = null;
            QueryFilter queryFilter = null;
            string SBQuery = string.Empty;
            try
            {
                //Get arcfm User LanID
                string sUserLanID = string.Empty;
                IMMDefaultConnection _defaultConnection = null;
                Type type = Type.GetTypeFromProgID("esriSystem.ExtensionManager");
                object objExt = Activator.CreateInstance(type);
                IExtensionManager extMgr = objExt as IExtensionManager;
                if (extMgr != null)
                {
                    _defaultConnection = (IMMDefaultConnection)extMgr.FindExtension("MMPropertiesExt");
                }
                //IMMDefaultConnection _defaultConnection;
                //string mmproprtiesextension = "MMPropertiesExt";
                //_defaultConnection = (IMMDefaultConnection)m_application.FindExtensionByName(mmproprtiesextension);
                if (_defaultConnection != null)
                {
                    sUserLanID = _defaultConnection.UserName;
                }
                else
                {
                    _logger.Info("PGE_LANID user name is not updated.");
                }
                //Fix : 22-Oct-20 :  Call from ED11 interface
                IWorkspace wSpace = null; 
                if (PGE.Desktop.EDER.AutoUpdaters.Special.PLC.PGEUpdateElevationAU.callByInterfaceED11)
                {
                    //m4jf edgisrearch 415 - Getting ED11 interface username
                    sUserLanID = PGE.Desktop.EDER.AutoUpdaters.Special.PLC.PGEUpdateElevationAU.ED11interfaceUser;
                    wSpace = PGE.Desktop.EDER.AutoUpdaters.Special.PLC.PGEUpdateElevationAU.PTTSessionWksp;
                }
                else
                {
                    wSpace = Editor.EditWorkspace;
                }
                IWorkspaceEdit wkspcEdit = (IWorkspaceEdit)wSpace;
                IFeatureWorkspace iFeatWs = (IFeatureWorkspace)wkspcEdit;

                //Getting data from Config table
                ITable tPLCLookup = new MMTableUtilsClass().OpenTable(SLookupTable, iFeatWs);

                if (tPLCLookup == null)
                    throw new Exception("Failed to load table " + SLookupTable);

                queryFilter = new QueryFilterClass();
                queryFilter.WhereClause = string.Format("ClassData='{0}'", "CALL_PLDBID");
                codeRow = new Extensions.CursorEnumerator(() => tPLCLookup.Search(queryFilter, false)).FirstOrDefault();
                if (codeRow == null) throw new Exception("Configuration Data not found for CALL_PLDBID in " + SLookupTable + " table");


                _newFeatureSA = iFeat;
                XY_LatLong(iFeat);
                _pldbidColName = (codeRow.get_Value(codeRow.Fields.FindField("Pldbid_ColName"))).ToString();

               

                //Get Elevation
                double dElev = getElevation2(_dLongitude, _dLatitude);
                Uri uri = new Uri((codeRow.get_Value(codeRow.Fields.FindField("StartAnalysis"))).ToString());
                pldbid = Callpldbid(strJobnumber, _dLatitude, _dLongitude, dElev, uri, (codeRow.get_Value(codeRow.Fields.FindField("AuthorizationValue"))).ToString(), sUserLanID);
                //******************************
                //whenever startanalysis service will hit, update the PGE_PLDBID_AUDIT_DATA table
                //YXA6->JEERAID-EDGISREARC-389

                if (pldbid > 0)
                {

                    SBQuery = "insert into PGEDATA.PGE_PLDBID_AUDIT_DATA values('"
                          + sUserLanID + "','" + iFeat.get_Value(iFeat.Fields.FindField("GLOBALID")).ToString() +
                          "','" + System.DateTime.Now.ToShortDateString() + "','" + pldbid + "','SUCCESS','"
                          + strJobnumber + "','" + System.Environment.MachineName + "')";

                    wSpace.ExecuteSQL(SBQuery);

                }
                else
                {
                    SBQuery = "insert into PGEDATA.PGE_PLDBID_AUDIT_DATA values('"
                  + sUserLanID + "','" + iFeat.get_Value(iFeat.Fields.FindField("GLOBALID")).ToString() +
                  "','" + System.DateTime.Now.ToShortDateString() + "','" + pldbid + "','FAILED','"
                  + strJobnumber + "','" + System.Environment.MachineName + "')";

                    wSpace.ExecuteSQL(SBQuery.ToString());
                }
                //*************************************

            }
            catch (Exception ex)
            {
                _logger.Error("PG&E PLC StartAnalysisCall, Create PLDBID function", ex);
            }
            finally
            {
                //release all objects goes here
                ReleaseAllTheThings(codeRow, queryFilter);
            }

            return pldbid;
        }

        void ReleaseAllTheThings(params object[] comObjects)
        {
            foreach (var item in comObjects)
            {
                if (item is IDisposable)
                    ((IDisposable)item).Dispose();
                else if (item != null && Marshal.IsComObject(item))
                    while (Marshal.ReleaseComObject(item) > 0) ;
            }
        }


        public double getElevation2(double XCoord, double YCoord)
        {
            try
            {
                //Create a point from the lat, long 
                ISpatialReferenceFactory2 srFactory = new SpatialReferenceEnvironmentClass();
                int NAD27_Geograpic = (4326);
                ISpatialReference GeographicSR = srFactory.CreateSpatialReference(NAD27_Geograpic);
                IPoint pPoint = new PointClass();
                pPoint.SpatialReference = GeographicSR;
                pPoint.PutCoords(XCoord, YCoord);
                const string demLayerName = "lbgis.ca_dem_30m";

                if (_pIdentify == null)
                {
                    //Fix : 22-Oct-20 : Call by ED11 interface 
                    IRasterWorkspaceEx pRasterWS = null;
                    if (PGE.Desktop.EDER.AutoUpdaters.Special.PLC.PGEUpdateElevationAU.callByInterfaceED11)
                    {
                        pRasterWS = PGE.Desktop.EDER.AutoUpdaters.Special.PLC.PGEUpdateElevationAU.PTTlandbaseWksp as IRasterWorkspaceEx;
                    }
                    else
                    {
                        if (_app == null)
                            _app = (IApplication)Activator.CreateInstance(
                                Type.GetTypeFromProgID(
                                "esriFramework.AppRef"));
                        IMxDocument pMxDoc = (IMxDocument)_app.Document;
                        IMap pMap = (IMap)pMxDoc.FocusMap;
                        pRasterWS = GetLandbaseRasterWorkspace(pMap);
                    }
                    //Marshal.FinalReleaseComObject(pMxDoc); //s2nn: Fixing a break where ArcFM Locator getting disabled because of this code
                    IRasterDataset pRDS = pRasterWS.OpenRasterDataset(demLayerName);
                    IRaster pDemRaster = ((IRasterDataset2)pRDS).CreateDefaultRaster();
                    IRasterLayer pRasterLayer = new RasterLayerClass();
                    pRasterLayer.CreateFromRaster(pDemRaster);
                    _pIdentify = (IIdentify)pRasterLayer;
                }
                return GetHeightAtPoint(pPoint, _pIdentify);

            }
            catch (Exception ex)
            {
                _logger.Error("PG&E PLC StartAnalysisCall, Error in getElevation2 function", ex);
                throw new Exception("Error returning elevation");
            }
        }

        private IRasterWorkspaceEx GetLandbaseRasterWorkspace(IMap map)
        {
            if (map == null) return null;
            //get all layers in Map
            IEnumLayer enumLayer = map.get_Layers(CartoFacade.UIDFacade.FeatureLayers, true);
            IFeatureLayer featLayer = null;
            ILayer layer;
            IFeatureClass pFeatClass = null;
            IDataset pDS = null;
            IRasterWorkspaceEx pRasterWS = null;
            const string lanbasePrefix = "lbgis.";

            List<IFeatureLayer> featlist = new List<IFeatureLayer>();

            while ((layer = enumLayer.Next()) != null)//loop through each layer
            {
                //Validate the layer and check layer name
                if (layer.Valid) //check layer name matching
                {
                    featLayer = layer as IFeatureLayer;
                    pFeatClass = featLayer.FeatureClass;
                    pDS = (IDataset)pFeatClass;

                    if (pDS.Name.ToLower().StartsWith(lanbasePrefix))
                    {
                        pRasterWS = (IRasterWorkspaceEx)((IDataset)featLayer.FeatureClass).Workspace;
                        break;
                    }
                }
            }

            //Release object
            if (enumLayer != null) Marshal.ReleaseComObject(enumLayer);

            if (pRasterWS == null)
                throw new Exception("Unable to find any lanbase layers in active map");
            return pRasterWS;
        }


        public double GetHeightAtPoint(IPoint pPoint, IIdentify pIdentify)
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
                _logger.Error("Error returning elevation at given point", ex);
                throw new Exception("Error returning the height at point");
            }
        }

        //Function to get PLDBID
        public double Callpldbid(String sJobnum, double dLat, double dLong, double dElevation, Uri strStartAnalysisUrl, String AuthValue,string sLanID)
        {
            double pldbid = 0.0;
            try
            {
               


                PoleData poleDataObj = new PoleData();
                poleDataObj.PLDBID = 0;
                poleDataObj.Latitude = dLat;
                poleDataObj.Longitude = dLong;
                poleDataObj.Elevation = dElevation;
                poleDataObj.PGE_NotificationNumber = Convert.ToUInt64(sJobnum);
                poleDataObj.PGE_GLOBALID = "00000000-0000-0000-0000-000000000000";
                poleDataObj.PGE_LANID = sLanID;

                DataContractJsonSerializer js = new DataContractJsonSerializer(typeof(PoleData));
                MemoryStream msObj = new MemoryStream();
                js.WriteObject(msObj, poleDataObj);
                msObj.Position = 0;
                StreamReader sr = new StreamReader(msObj);

                string jsonPgeDataBlock = sr.ReadToEnd();

                //Pole Header JSON
                PoleHeader poleHeaderObj = new PoleHeader();
                poleHeaderObj.Source = "GIS";
                poleHeaderObj.TrackingID = Guid.NewGuid().ToString();
                poleHeaderObj.Timestamp = DateTime.Now.ToString();

                js = new DataContractJsonSerializer(typeof(PoleHeader));
                msObj = new MemoryStream();
                js.WriteObject(msObj, poleHeaderObj);
                msObj.Position = 0;
                sr = new StreamReader(msObj);

                string jsonPgeHeader = sr.ReadToEnd();

                sr.Close();
                msObj.Close();

                string finaljson = "{\"Header\": " + jsonPgeHeader + ",\n\"PoleData\": " + jsonPgeDataBlock + "}";
                _logger.Debug("StrtAnalysisCall Final Json : " + finaljson);
                //Call to web service
                WebClient cnt = new WebClient();
                cnt.UploadStringCompleted += new UploadStringCompletedEventHandler(c_UploadStringCompleted);
                cnt.Headers["Content-type"] = "application/json";
                cnt.Encoding = Encoding.UTF8;
                cnt.Headers[HttpRequestHeader.Authorization] = "Basic " + AuthValue;
                
                string pldbidStr = cnt.UploadString(strStartAnalysisUrl, "POST", finaljson);
                _returnedPLDID = Convert.ToDouble(pldbidStr.Split(':')[1].Split('}')[0]);
                pldbid = _returnedPLDID;
            }
            catch (Exception ex)
            {
                _logger.Error("PG&E PLC StartAnalysisCall , Call to PLDBID Async function ", ex);
            }
            return pldbid;
        }

        //Function to convert XY to Latitude longitude
        public void XY_LatLong(IFeature feature)
        {
            try
            {
                ISpatialReferenceFactory2 srFactory = new SpatialReferenceEnvironmentClass();
                int NAD27_Geograpic = (4326);
                ISpatialReference GeographicSR = srFactory.CreateSpatialReference(NAD27_Geograpic);

                IFeatureChanges pFeatChange = (IFeatureChanges)feature;
                // M4JF - edgisrearch 415 - Updated to include ed11 interface points coordinates also .
                if (pFeatChange.ShapeChanged || PGE.Desktop.EDER.AutoUpdaters.Special.PLC.PGEUpdateElevationAU.callByInterfaceED11)
                {
                    IPoint pPoint = (IPoint)feature.ShapeCopy;
                    ISpatialReference sr1 = pPoint.SpatialReference;
                    ISpatialReferenceResolution srRes = sr1 as ISpatialReferenceResolution;
                    IPoint geographicPoint = new ESRI.ArcGIS.Geometry.Point();
                    geographicPoint.X = pPoint.X;
                    geographicPoint.Y = pPoint.Y;
                    geographicPoint.Z = pPoint.Z;
                    geographicPoint.SpatialReference = sr1; // PCS

                    geographicPoint.Project(GeographicSR);

                    _dLatitude = geographicPoint.Y;
                    _dLongitude = geographicPoint.X;
                }
            }
            catch (Exception ex)
            {
                _logger.Error("PG&E PLC StartAnalysisCall , Error in XY_LatLong function ", ex);
            }
        }

        private void c_UploadStringCompleted(object sender, UploadStringCompletedEventArgs e)
        {
            try
            {
                _returnedPLDID = Convert.ToDouble(e.Result.Split(':')[1].Split('}')[0]);
            }
            catch (Exception ex)
            {
                _logger.Error("PG&E PLC StartAnalysisCall, Error in c_UploadStringCompleted function", ex);
            }
        }
    }
}
