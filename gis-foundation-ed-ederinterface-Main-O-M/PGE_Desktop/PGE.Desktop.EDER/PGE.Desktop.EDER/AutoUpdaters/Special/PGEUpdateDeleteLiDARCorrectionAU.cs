using System;
using System.Reflection;
using System.Runtime.InteropServices;
using Miner.ComCategories;
using Miner.Interop;
using PGE.Common.Delivery.ArcFM;
using PGE.Common.Delivery.Framework;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.DataSourcesRaster;

namespace PGE.Desktop.EDER.AutoUpdaters.Special
{
    [Guid("2D3E67AC-5092-4103-88DA-A2DAE95E7023")]
    [ProgId("PGE.Desktop.EDER.PGEUpdateDeleteLiDARCorrectionAU")]
    [ComponentCategory(ComCategory.SpecialAutoUpdateStrategy)]
    [ComVisible(true)]
    public class PGEUpdateDeleteLiDARCorrectionAU : BaseSpecialAU
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="PGEUpdateDeleteLiDARCorrectionAU"/> class.  
        /// </summary>
        public PGEUpdateDeleteLiDARCorrectionAU() : base("PGE UpdateOrDelete LiDARCorrection AU") { }
        #endregion Constructors

        #region ClassVariables

        private static readonly PGE.Common.Delivery.Diagnostics.Log4NetLogger _logger = new PGE.Common.Delivery.Diagnostics.Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "EDERDesktop.log4net.config");
        
        #endregion ClassVariables

        #region Special AU Overrides
        /// <summary>
        /// Enable Autoupdater in case of onDelete/onUpdate event and class contans SupportStucture model name
        /// </summary>
        /// <param name="pObjectClass">The selected object class.</param>
        /// <param name="eEvent">The AU event type.</param>
        /// <returns></returns>
        protected override bool InternalEnabled(IObjectClass pObjectClass, Miner.Interop.mmEditEvent eEvent)
        {
            string[] modelNames = new string[] { SchemaInfo.Electric.ClassModelNames.SupportStructure };
            bool enabled = ModelNameFacade.ContainsClassModelName(pObjectClass, modelNames);
            _logger.Debug(string.Format("ClassModelName : {0}, exist:{1}", SchemaInfo.Electric.ClassModelNames.SupportStructure, enabled));
            if ((eEvent == Miner.Interop.mmEditEvent.mmEventFeatureDelete && enabled) || (eEvent == Miner.Interop.mmEditEvent.mmEventFeatureUpdate && enabled))
            {
                _logger.Debug("Returning Visible: true.");
                return true;
            }
            _logger.Debug("Returning Visible: false.");
            return false;
            
        }

        protected override void InternalExecute(IObject pObject, Miner.Interop.mmAutoUpdaterMode eAUMode, Miner.Interop.mmEditEvent eEvent)
        {
            //AU will run only on FeatureUpdate/ on FeatureDelete events
            if (eEvent == mmEditEvent.mmEventFeatureUpdate || eEvent == mmEditEvent.mmEventFeatureDelete)
            {
                IFeatureClass LiDarCorrFC = ModelNameFacade.FeatureClassByModelName((pObject.Class as IDataset).Workspace, SchemaInfo.Electric.ClassModelNames.LiDARCORRECTIONS);                
                IQueryFilter qf = new QueryFilterClass();
                qf.WhereClause = "RELATEDSUPPORTSTRUCTUREGUID = '"+ pObject.GetFieldValue("GLOBALID",false,null)+"'";
                IFeatureCursor featureCur = LiDarCorrFC.Search(qf, false);
                IFeature lidarFeat = featureCur.NextFeature();
                
                //Deleting the LiDar Correction Feature
                if (eEvent == mmEditEvent.mmEventFeatureDelete)
                {
                    if (lidarFeat != null)
                    {
                        lidarFeat.Delete();
                    }
                }

                //Updating the LiDar Correction Feature, if feature does not exist then create one 
                if (eEvent == mmEditEvent.mmEventFeatureUpdate)
                {                    
                    IField measuredLatitudeField = ModelNameFacade.FieldFromModelName(pObject.Class, SchemaInfo.Electric.FieldModelNames.MeasuredLatitude);                    
                    IField measuredLongitudeField = ModelNameFacade.FieldFromModelName(pObject.Class, SchemaInfo.Electric.FieldModelNames.MeasuredLongitude);                    
                    IField PLDBID = ModelNameFacade.FieldFromModelName(pObject.Class, SchemaInfo.Electric.FieldModelNames.PLDBID);                    
                    IField STATUS = ModelNameFacade.FieldFromModelName(pObject.Class, SchemaInfo.Electric.FieldModelNames.Status);
                    
                    object measuredLatitude = (object)pObject.GetFieldValue(measuredLatitudeField.Name, true, SchemaInfo.Electric.FieldModelNames.MeasuredLatitude);                    
                    object measuredLongitude = (object)pObject.GetFieldValue(measuredLongitudeField.Name, true, SchemaInfo.Electric.FieldModelNames.MeasuredLongitude);
                    object pldbid = (object)pObject.GetFieldValue(PLDBID.Name, true, SchemaInfo.Electric.FieldModelNames.PLDBID);
                    object status = (object)pObject.GetFieldValue(STATUS.Name, false, SchemaInfo.Electric.FieldModelNames.Status);                    
                    object globalidPole = pObject.get_Value(pObject.Fields.FindField("GLOBALID"));

                   

                    //Process only if measured lat/long value available in support structure
                    if (measuredLatitude != null && measuredLongitude != null)
                    {
                        IFeature sp = pObject as IFeature;
                        IPoint from = new Point();
                        from = sp.Shape as IPoint;

                        double lat1;
                        double long1;
                        XY_LatLong(from, out lat1, out long1);

                        IPoint to = GetPointFromLatLong((double)measuredLatitude, (double)measuredLongitude, from);
                        double distance = getDistancebtwTwoPoint(from, to);
                        
                        double angle = getAngle((double)measuredLatitude, (double)measuredLongitude, lat1, long1);

                        //Create Feature
                        if (lidarFeat == null)
                        {
                            from.PutCoords(from.X, from.Y);
                            to.PutCoords(to.X, to.Y);
                            IPolyline line = new PolylineClass();
                            line.FromPoint = from; line.ToPoint = to;
                            lidarFeat = LiDarCorrFC.CreateFeature();
                            line.SpatialReference = lidarFeat.Shape.SpatialReference;
                            line.Project(lidarFeat.Shape.SpatialReference);
                            lidarFeat.Shape = (IGeometry)line;
                            lidarFeat.Store();

                            IField measuredLatitudeFieldLiDAR = ModelNameFacade.FieldFromModelName(((IObject)lidarFeat).Class, SchemaInfo.Electric.FieldModelNames.MeasuredLatitude);
                            IField measuredLongitudeFieldLiDAR = ModelNameFacade.FieldFromModelName(((IObject)lidarFeat).Class, SchemaInfo.Electric.FieldModelNames.MeasuredLongitude);
                            IField PLDBIDLiDAR = ModelNameFacade.FieldFromModelName(((IObject)lidarFeat).Class, SchemaInfo.Electric.FieldModelNames.PLDBID);
                            IField STATUSLiDAR = ModelNameFacade.FieldFromModelName(((IObject)lidarFeat).Class, SchemaInfo.Electric.FieldModelNames.Status);
                            IField AngleLiDAR = ModelNameFacade.FieldFromModelName(((IObject)lidarFeat).Class, SchemaInfo.Electric.FieldModelNames.BearingAngle);
                            IField LengthLiDAR = ModelNameFacade.FieldFromModelName(((IObject)lidarFeat).Class, SchemaInfo.Electric.FieldModelNames.RangetoShape);

                            lidarFeat.set_Value(lidarFeat.Fields.FindField(measuredLatitudeFieldLiDAR.Name), measuredLatitude);
                            lidarFeat.set_Value(lidarFeat.Fields.FindField(measuredLongitudeFieldLiDAR.Name), measuredLongitude);
                            lidarFeat.set_Value(lidarFeat.Fields.FindField(PLDBIDLiDAR.Name), pldbid);
                            lidarFeat.set_Value(lidarFeat.Fields.FindField(STATUSLiDAR.Name), status);
                            lidarFeat.set_Value(lidarFeat.Fields.FindField("RELATEDSUPPORTSTRUCTUREGUID"), globalidPole);
                            lidarFeat.set_Value(lidarFeat.Fields.FindField(LengthLiDAR.Name), distance);
                            lidarFeat.set_Value(lidarFeat.Fields.FindField(AngleLiDAR.Name), angle);
                            
                        }
                        //Update the Feature
                        else
                        {
                        IFeature feat = pObject as IFeature;
                        IFeatureChanges featureChange = feat as IFeatureChanges;
                        //If feature shape changes then re-create the LiDAR Corrections feature
                        if (featureChange.ShapeChanged)
                        {
                            lidarFeat.Delete();

                            from.PutCoords(from.X, from.Y);
                            to.PutCoords(to.X, to.Y);
                            IPolyline line = new PolylineClass();
                            line.FromPoint = from; line.ToPoint = to;
                            lidarFeat = LiDarCorrFC.CreateFeature();
                            line.SpatialReference = lidarFeat.Shape.SpatialReference;
                            line.Project(lidarFeat.Shape.SpatialReference);
                            lidarFeat.Shape = (IGeometry)line;
                            lidarFeat.Store();
                            
                        }
                        IField measuredLatitudeFieldLiDAR = ModelNameFacade.FieldFromModelName(((IObject)lidarFeat).Class, SchemaInfo.Electric.FieldModelNames.MeasuredLatitude);
                        IField measuredLongitudeFieldLiDAR = ModelNameFacade.FieldFromModelName(((IObject)lidarFeat).Class, SchemaInfo.Electric.FieldModelNames.MeasuredLongitude);
                        IField PLDBIDLiDAR = ModelNameFacade.FieldFromModelName(((IObject)lidarFeat).Class, SchemaInfo.Electric.FieldModelNames.PLDBID);
                        IField STATUSLiDAR = ModelNameFacade.FieldFromModelName(((IObject)lidarFeat).Class, SchemaInfo.Electric.FieldModelNames.Status);
                        IField AngleLiDAR = ModelNameFacade.FieldFromModelName(((IObject)lidarFeat).Class, SchemaInfo.Electric.FieldModelNames.BearingAngle);
                        IField LengthLiDAR = ModelNameFacade.FieldFromModelName(((IObject)lidarFeat).Class, SchemaInfo.Electric.FieldModelNames.RangetoShape);

                        lidarFeat.set_Value(lidarFeat.Fields.FindField(measuredLatitudeFieldLiDAR.Name), measuredLatitude);
                        lidarFeat.set_Value(lidarFeat.Fields.FindField(measuredLongitudeFieldLiDAR.Name), measuredLongitude);
                        lidarFeat.set_Value(lidarFeat.Fields.FindField(PLDBIDLiDAR.Name), pldbid);
                        lidarFeat.set_Value(lidarFeat.Fields.FindField(STATUSLiDAR.Name), status);
                        lidarFeat.set_Value(lidarFeat.Fields.FindField("RELATEDSUPPORTSTRUCTUREGUID"), globalidPole);
                        lidarFeat.set_Value(lidarFeat.Fields.FindField(LengthLiDAR.Name), distance);
                        lidarFeat.set_Value(lidarFeat.Fields.FindField(AngleLiDAR.Name), angle);
                        }

                        //Refresh the Map
                        ESRI.ArcGIS.ArcMapUI.IMxApplication mxap = PGEExtension._mxapp;
                        ESRI.ArcGIS.Framework.IApplication app = mxap as ESRI.ArcGIS.Framework.IApplication;
                        ESRI.ArcGIS.ArcMapUI.IMxDocument mxd = app.Document as ESRI.ArcGIS.ArcMapUI.IMxDocument;
                        mxd.ActiveView.PartialRefresh(ESRI.ArcGIS.Carto.esriViewDrawPhase.esriViewAll, null, null);
                    }
                }
            }
        }

        #endregion

        #region methods
        /// <summary>
        /// Create Point feature from latitude and longitude values with respect to map spatial reference
        /// </summary>
        /// <param name="latitude">Latitude Value</param>
        /// <param name="longitude">Longitude Value</param>
        /// <param name="pPoint">Input feature</param>
        /// <returns>Point from latitude and longitude in map's spatial reference</returns>
        public static IPoint GetPointFromLatLong(double latitude, double longitude, IPoint pPoint)
        {
            try
            {
                ISpatialReferenceFactory srFactory = new SpatialReferenceEnvironmentClass();
                ISpatialReference sr1;
                //GCS to project from           
                IGeographicCoordinateSystem gcs = srFactory.CreateGeographicCoordinateSystem(4326);
                sr1 = gcs;
                //Projected Coordinate System to project into           
                IProjectedCoordinateSystem pcs = srFactory.CreateProjectedCoordinateSystem(26910);
                ISpatialReference sr2 = pPoint.SpatialReference;
                // sr2 = pcs;
                //Point to project            
                IPoint point = new PointClass() as IPoint;
                point.PutCoords(longitude, latitude);
                //Geometry Interface to do actual project           
                IGeometry geometry;
                geometry = point;
                geometry.SpatialReference = sr1;
                geometry.Project(sr2);
                point = geometry as IPoint;
                return point;
            }
            catch (Exception ex)
            {
                _logger.Warn(ex.Message);
                return null;
            }



        }

        //get X Y 
        public void XY_LatLong(IPoint pPoint, out double _dLatitude, out double _dLongitude)
        {
            _dLatitude = 0;
            _dLongitude = 0;
            try
            {
                ISpatialReferenceFactory2 srFactory = new SpatialReferenceEnvironmentClass();
                int NAD27_Geograpic = (4326);
                ISpatialReference GeographicSR = srFactory.CreateSpatialReference(NAD27_Geograpic);

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

            catch (Exception ex)
            {
                _logger.Debug("Error while converting from Point to Latitude and Longitude");
                throw ex;
            }
        }

        //Get the diatnce between two points
        public double getDistancebtwTwoPoint(IPoint pointFromLatLong, IPoint pPoint)
        {
            double distance = 0;
            IProximityOperator pProximityOperator = (IProximityOperator)pointFromLatLong;
            distance = pProximityOperator.ReturnDistance(pPoint);
            distance = Math.Ceiling(distance * 100) / 100;
            
            return distance;

        }

        public double getAngle(double lat1, double lon1, double lat2, double lon2)
        {
                   
            try
            {
                //double lat1 = pointFromLatLong.Y; double lon1 = pointFromLatLong.X; double lat2 = pPoint.Y; double lon2 = pPoint.X;
                //double lat2 = pointFromLatLong.X; double lon2 = pointFromLatLong.Y; double lat1 = pPoint.X; double lon1 = pPoint.Y;
                var dLon = ToRad(lon2 - lon1);
                var dPhi = Math.Log(
                    Math.Tan(ToRad(lat2) / 2 + Math.PI / 4) / Math.Tan(ToRad(lat1) / 2 + Math.PI / 4));
                if (Math.Abs(dLon) > Math.PI)
                    dLon = dLon > 0 ? -(2 * Math.PI - dLon) : (2 * Math.PI + dLon);
                var degree = ToBearing(Math.Atan2(dLon, dPhi));

                return degree;

              
            }
            catch (Exception ex)
            {
               
                _logger.Warn("Error while calculating bearing angle:"+ ex.Message);
                return 0;
            }
                       
        }

        /// <summary>
        /// Converts to radian
        /// </summary>
        /// <param name="degrees">Degree value</param>
        /// <returns>Radian value</returns>
        public static double ToRad(double degrees)
        {
            return degrees * (Math.PI / 180);
        }
        /// <summary>
        /// Convert to degrees
        /// </summary>
        /// <param name="radians">Radian value</param>
        /// <returns>Degree values</returns>
        public static double ToDegrees(double radians)
        {
            return radians * 180 / Math.PI;
        }
        public static double ToBearing(double radians)
        {
            // convert radians to degrees (as bearing: 0...360)
            return (ToDegrees(radians) + 360) % 360;
        }

        #endregion
    }
}


