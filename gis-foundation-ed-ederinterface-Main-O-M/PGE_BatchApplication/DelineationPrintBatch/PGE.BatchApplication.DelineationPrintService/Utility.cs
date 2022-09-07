using System;

using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geometry;

namespace PGE.BatchApplication.DelineationPrintService
{
    class Utility
    {
        protected static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private static InitLicenseManager m_AOLicenseInitializer = new InitLicenseManager();

        private const string FIELD_OBJECTID = "OBJECTID";
        private const string FIELD_GLOBALID = "GLOBALID";
        private const string FIELD_DATEREPAIRED = "DATEREPAIRED";
        private const string FIELD_DATECANCELLED = "DATECANCELLED";

        public enum ModType
        {
            New,
            Modify,
            Delete,
            Error,
        }
        public enum GeoType
        {
            Add,
            NON,
            XY,
        }

        public static bool InitializeLicense()
        {
            ///Need to get license before doing anything else
            ///We need both license, ArcGIS and ArcFM
            bool flag = false;
            m_AOLicenseInitializer = new InitLicenseManager();
            try
            {
                try
                {
                    if (m_AOLicenseInitializer.Initialize(esriLicenseProductCode.esriLicenseProductCodeAdvanced, (Miner.Interop.mmLicensedProductCode)5))
                    {
                        flag = true;
                        Log.Info("SUCCESSFULLY GET LICENSE: " + flag);
                    }

                }
                catch (Exception Ex)
                {
                    Log.Error("Error unable to get license: ", Ex);
                    return false;

                }
                return flag;
            }
            finally
            {
                //_licenseManager.Shutdown();
            }
        }

        public static void ShutdownLicense()
        {
            m_AOLicenseInitializer.Shutdown();
        }

        public static IFeatureWorkspace ConnectToSDE(string sdeConnection)
        {
            ///Opening workspace and return feature work space. The connection is from the configuration file
            try
            {
                if (string.IsNullOrEmpty(sdeConnection))
                {
                    Log.Info("Cannot read sde file path from configuration file");
                    return null;
                }

                Log.Info("Try to connect to SDE: " + sdeConnection);
                Type t = Type.GetTypeFromProgID("esriDataSourcesGDB.SdeWorkspaceFactory");
                IWorkspaceFactory wsf = Activator.CreateInstance(t) as IWorkspaceFactory;
                IFeatureWorkspace featureWorkspace = (IFeatureWorkspace)wsf.OpenFromFile(sdeConnection, 0);
                Log.Info("Successfully connected to database");

                return featureWorkspace;
            }
            catch (Exception Ex)
            {
                Log.Error("Error Fail to connect to workspace: ", Ex);
                return null;
            }

        }

        public static esriWorkspaceConnectionStatus CheckConnectionStatus(IWorkspaceFactory wsf, ref IWorkspace ws)
        {
            //Find the connection again to reconnect
            IWorkspaceFactoryStatus wsfs = (IWorkspaceFactoryStatus)wsf;

            IWorkspaceStatus wss = wsfs.PingWorkspaceStatus(ws);

            if (wss.ConnectionStatus == esriWorkspaceConnectionStatus.esriWCSUp)
                return esriWorkspaceConnectionStatus.esriWCSUp;
            else if (wss.ConnectionStatus == esriWorkspaceConnectionStatus.esriWCSDown)
                return esriWorkspaceConnectionStatus.esriWCSDown;
            else //available
            {
                ws = wsfs.OpenAvailableWorkspace(wss);
                return esriWorkspaceConnectionStatus.esriWCSAvailable;
            }
        }


        public static object GetValueFromField(IRow row, string fieldName)
        {
            bool UpdateOk = false;
            try
            {
                int fldIndex = -1;

                fldIndex = row.Fields.FindField(fieldName.Trim());

                if (fldIndex == -1) return UpdateOk;
                object value = row.get_Value(fldIndex);
                UpdateOk = true;
                return value;
            }
            catch (Exception Ex)
            {
                Log.Error("Error: ", Ex);
                return null;
            }
        }

        public static void CopyFieldAttributes(IRow fromRow, IRow toRow)
        {
            try
            {
                //Loop through all fields and copy them to leak feature
                IFields fields = fromRow.Fields;
                for (int fieldIndexFrom = 0; fieldIndexFrom < fields.FieldCount; fieldIndexFrom++)
                {
                    string fieldName = fields.Field[fieldIndexFrom].Name;
                    if (fieldName != FIELD_OBJECTID && fieldName != FIELD_GLOBALID)
                    {
                        int fldIndexTo = toRow.Fields.FindField(fieldName);
                        if (fldIndexTo > -1)
                        {
                            toRow.set_Value(fldIndexTo, fromRow.get_Value(fieldIndexFrom));
                        }
                    }
                }

            }
            catch (Exception Ex)
            {
                Log.Error(Ex);
            }
        }

        public static void CopyExistingLeakData(IRow fromRow, IRow toRow, object dateRepaired, object dateCancelled)
        {
            try
            {
                //Loop through all fields and copy them to leak feature
                IFields fields = fromRow.Fields;
                for (int fieldIndexFrom = 0; fieldIndexFrom < fields.FieldCount; fieldIndexFrom++)
                {
                    string fieldName = fields.Field[fieldIndexFrom].Name;
                    if (fieldName != FIELD_OBJECTID && fieldName != FIELD_GLOBALID)
                    {
                        int fldIndexTo = toRow.Fields.FindField(fieldName);
                        if (fldIndexTo > -1)
                        {
                            //if there's a daterepaired. do not overwrite
                            if (fieldName == FIELD_DATEREPAIRED)
                            {
                                if (dateRepaired != null)
                                    toRow.set_Value(fldIndexTo, dateRepaired);
                            }
                            else if (fieldName == FIELD_DATECANCELLED)
                            {
                                if (dateCancelled != null)
                                    toRow.set_Value(fldIndexTo, dateCancelled);
                            }
                            else
                            {
                                toRow.set_Value(fldIndexTo, fromRow.get_Value(fieldIndexFrom));
                            }
                        }
                    }
                }
            }
            catch (Exception Ex)
            {
                Log.Error(Ex);
            }
        }

        public static bool SetValueFromField(ref IRow row2Update, string Update2FldName, object UpdateValue)
        {
            bool UpdateOk = false;
            try
            {
                int Update2FldIdx = -1;

                Update2FldIdx = row2Update.Fields.FindField(Update2FldName.Trim());

                if (Update2FldIdx == -1) return UpdateOk;

                row2Update.set_Value(Update2FldIdx, UpdateValue);
                UpdateOk = true;
            }
            catch (Exception Ex)
            {
                Log.Error("Error: ", Ex);
                //throw (new Exception(Ex.ToString(), Ex));
            }
            return UpdateOk;
        }

        public static void LogAuditTrail(ITable auditTrailTable, IFeature feature, IFeatureWorkspace fworkspace)
        {
            try
            {
                string editType = "A";
                string featureID = feature.OID.ToString();
                string lanID = ((IWorkspace)fworkspace).ConnectionProperties.GetProperty("USER").ToString();

                IVersion version = (IVersion)fworkspace;

                string versionName = "SDE.DEFAULT";
                if (version != null)
                    versionName = version.VersionName;

                //AuditTrailUtils.CreateAuditTrailRecord(auditTrailTable, feature, editType, versionName, lanID);
            }
            catch (Exception ex)
            {
                Log.Error("Unable to write audit trail for WIP record " + feature.OID, ex);
            }
        }


        public static string GetStatusType(ModType modType)
        {
            switch (modType)
            {
                case ModType.Delete:
                    return "D";
                case ModType.Modify:
                    return "M";
                case ModType.New:
                    return "N";
                case ModType.Error:
                    return "E";
            }

            return "";
        }

        public static string GetGeoType(GeoType geoType)
        {
            switch (geoType)
            {
                case GeoType.Add:
                    return "ADD";
                case GeoType.XY:
                    return "XY";
                case GeoType.NON:
                    return "NON";
            }

            return "";
        }

        private static IPoint GetMidPoint(IFeature feature)
        {
            try
            {
                //pGeometry = pFeature.Shape
                IPoint point = new ESRI.ArcGIS.Geometry.Point();
                //if (feature.Shape.GeometryType == esriGeometryType.esriGeometryPoint
                switch (feature.Shape.GeometryType)
                {
                    case esriGeometryType.esriGeometryPoint:
                        point = (IPoint)feature.Shape;
                        //Use Point as point
                        break;
                    case esriGeometryType.esriGeometryPolyline:
                        //Get the midpoint of the selected polyline.        
                        ICurve curve = feature.Shape as ICurve;
                        //IPoint curveMidPoint = new ESRI.ArcGIS.Geometry.Point();        
                        curve.QueryPoint(esriSegmentExtension.esriNoExtension, 0.5, true, point);
                        break;
                    case esriGeometryType.esriGeometryPolygon:
                        //Get midpoint of selected polygon
                        IArea area = feature.Shape as IArea;
                        area.QueryCentroid(point);
                        break;
                }

                return point;
            }
            catch (Exception Ex)
            {
                Log.Error(Ex);
                return null;
            }
        }
    }
}
