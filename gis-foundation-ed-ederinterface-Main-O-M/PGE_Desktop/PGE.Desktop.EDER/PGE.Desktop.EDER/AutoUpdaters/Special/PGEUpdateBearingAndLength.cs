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
    /// <summary>
    /// Autoupdater to update the value of BearingAngle and ShapetoRange field based on Measured latitude and measured longitude
    /// </summary>
    [Guid("4A8F24AD-B542-4F07-AD44-43300DDBE42E")]
    [ProgId("PGE.Desktop.EDER.PGEUpdateBearingAndLength")]
    [ComponentCategory(ComCategory.SpecialAutoUpdateStrategy)]
    [ComVisible(true)]
    public class PGEUpdateBearingAndLength:BaseSpecialAU
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="PGEUpdateBearingAndLength"/> class.  
        /// </summary>
        public PGEUpdateBearingAndLength() : base("PGE Update Bearing and Length") { }
        #endregion Constructors


        #region ClassVariables

        private static readonly PGE.Common.Delivery.Diagnostics.Log4NetLogger _logger = new PGE.Common.Delivery.Diagnostics.Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "EDERDesktop.log4net.config");
        #endregion ClassVariables

        #region Special AU Overrides
        /// <summary>
        /// Enable Autoupdater in case of onCreate/onUpdate event and class contans SupportStucture model name
        /// </summary>
        /// <param name="pObjectClass">The selected object class.</param>
        /// <param name="eEvent">The AU event type.</param>
        /// <returns></returns>
        protected override bool InternalEnabled(IObjectClass pObjectClass, Miner.Interop.mmEditEvent eEvent)
        {
            string[] modelNames = new string[] { SchemaInfo.Electric.ClassModelNames.SupportStructure };
            bool enabled = ModelNameFacade.ContainsClassModelName(pObjectClass, modelNames);
            _logger.Debug(string.Format("ClassModelName : {0}, exist:{1}", SchemaInfo.Electric.ClassModelNames.SupportStructure, enabled));
            if ((eEvent == Miner.Interop.mmEditEvent.mmEventFeatureCreate && enabled) || (eEvent == Miner.Interop.mmEditEvent.mmEventFeatureUpdate && enabled))
            {
                _logger.Debug("Returning Visible: true.");
                return true;
            }
            _logger.Debug("Returning Visible: false.");
            return false;
        }

        /// <summary>
        /// Dunctionality of PGEUpdateBearingAndLength Autoupdater.
        /// </summary>
        /// <param name="pObject">The object being updated.</param>
        /// <param name="eAUMode">The AU mode.</param>
        /// <param name="eEvent">The edit event.</param>
        protected override void InternalExecute(IObject pObject, Miner.Interop.mmAutoUpdaterMode eAUMode, Miner.Interop.mmEditEvent eEvent)
        {
            AUMain_Public(pObject, eEvent);
        }

        public void AUMain_Public(IObject pObject, mmEditEvent eEvent)
        {
            //Check AU is executing only on Support Structure FeatureClass
            if (ModelNameFacade.ContainsClassModelName(pObject.Class, SchemaInfo.Electric.ClassModelNames.SupportStructure))
            {
                //AU will run only on FeatureCreate/ on FeatureUpdate events
                if (eEvent == mmEditEvent.mmEventFeatureCreate || eEvent == mmEditEvent.mmEventFeatureUpdate)
                {
                    try
                    {
                        //Code to detect shape changes for Support Structure feature, If pole moves than make SourceAccuracy field as null
                        IFeature feat = pObject as IFeature;
                        IFeatureChanges featureChange = feat as IFeatureChanges;
                        if (featureChange.ShapeChanged)
                        {
                            Console.WriteLine("Shape Changed");
                            IField sourceAccuracyField = ModelNameFacade.FieldFromModelName(pObject.Class, SchemaInfo.Electric.FieldModelNames.SourceAccuracy);
                            int sourceAccuracyFieldIndex = pObject.Class.FindField(sourceAccuracyField.Name);
                            if (sourceAccuracyFieldIndex > -1) pObject.set_Value(sourceAccuracyFieldIndex, (object)System.DBNull.Value);

                        }

                        //measuredlatitude field
                        IField measuredLatitudeField = ModelNameFacade.FieldFromModelName(pObject.Class, SchemaInfo.Electric.FieldModelNames.MeasuredLatitude);
                        //measuredlongitude field
                        IField measuredLongitudeField = ModelNameFacade.FieldFromModelName(pObject.Class, SchemaInfo.Electric.FieldModelNames.MeasuredLongitude);
                        //bearing angle field
                        IField bearingAngleField = ModelNameFacade.FieldFromModelName(pObject.Class, SchemaInfo.Electric.FieldModelNames.BearingAngle);
                        //range to shape field
                        IField rangeToShapeField = ModelNameFacade.FieldFromModelName(pObject.Class, SchemaInfo.Electric.FieldModelNames.RangetoShape);
                        //Check model names assigned to MeasureLatitude and Longitude Field or not.
                        if (measuredLatitudeField != null && measuredLongitudeField != null && bearingAngleField != null && rangeToShapeField != null)
                        {
                            //Find the index of RangeToShape_FT to store distance value in feet.
                            int indexRangeToShape = pObject.Class.FindField(rangeToShapeField.Name);
                            //Find index of bearing angle
                            int indexBearingAngle = pObject.Class.FindField(bearingAngleField.Name);
                            //value of measured latitude field populated from PLDB
                            object measuredLatitude = (object)pObject.GetFieldValue(measuredLatitudeField.Name, true, SchemaInfo.Electric.FieldModelNames.MeasuredLatitude);
                            //value of measured longitude field populated from PLDB
                            object measuredLongitude = (object)pObject.GetFieldValue(measuredLongitudeField.Name, true, SchemaInfo.Electric.FieldModelNames.MeasuredLongitude);

                            //Check if measured latitude and longitude values are null.
                            if (measuredLatitude != null && measuredLongitude != null && measuredLatitude != DBNull.Value && measuredLongitude != DBNull.Value)
                            {
                                //check all measuredlatitude and measuredlongitude fields values are within the range of domains
                                bool validateDomainValues = ValidateDomainValue((double)measuredLatitude, (double)measuredLongitude, measuredLatitudeField, measuredLongitudeField);
                                //If values are not valid than set RangeToShape field as null and return;
                                if (!validateDomainValues)
                                {
                                    pObject.set_Value(indexRangeToShape, null);
                                    pObject.set_Value(indexBearingAngle, null);
                                    return;
                                }
                                //current feature
                                IFeature pFeature = (IFeature)pObject;
                                IPoint pPoint = (IPoint)pFeature.Shape;
                                //get point in map's spatial refernce from latitude and longitude
                                IPoint pointFromLatLong = GetPointFromLatLong((double)measuredLatitude, (double)measuredLongitude, pPoint);
                                //if point created from latitude and longitude is not correct than set RangeToShape field as null and return;
                                if (pointFromLatLong == null)
                                {
                                    pObject.set_Value(indexRangeToShape, null);
                                    pObject.set_Value(indexBearingAngle, null);
                                    return;
                                }

                                //Get the diatnce between two points
                                IProximityOperator pProximityOperator = (IProximityOperator)pointFromLatLong;
                                double distance = pProximityOperator.ReturnDistance(pPoint);
                                distance = Math.Ceiling(distance * 100) / 100;
                                //set Distance value to Range to Shape Field
                                pObject.set_Value(indexRangeToShape, distance);

                                double lat1;
                                double long1;
                                XY_LatLong(pPoint, out lat1, out long1);

                                _logger.Debug("Geometry co-ordinates after projection is Lat:" + lat1 + " Long: " + long1);
                                //Calculate bearing angle between latitude and longitudes
                                //double degreeBearingAngle = DegreeBearing(lat1, long1, (double)measuredLatitude, (double)measuredLongitude);
                                double degreeBearingAngle = DegreeBearing((double)measuredLatitude, (double)measuredLongitude, lat1, long1);
                                degreeBearingAngle = Math.Ceiling(degreeBearingAngle * 100) / 100;
                                //Set the value of Bearing Angle field                               
                                pObject.set_Value(indexBearingAngle, degreeBearingAngle);
                                _logger.Debug("Bearing angle updated: " + degreeBearingAngle);

                            }
                            else
                            {
                                //Set the value of Range to Shape and Bearing Angle field values as null
                                pObject.set_Value(indexRangeToShape, null);
                                pObject.set_Value(indexBearingAngle, null);
                                _logger.Warn("Latitude and Longitude can not be null.");
                            }



                        }
                        else
                        {
                            _logger.Debug(string.Format("Field ModelNames {0},{1},{2},{3} not assigned properly", SchemaInfo.Electric.FieldModelNames.MeasuredLatitude, SchemaInfo.Electric.FieldModelNames.MeasuredLongitude, SchemaInfo.Electric.FieldModelNames.RangetoShape, SchemaInfo.Electric.FieldModelNames.BearingAngle));
                        }

                    }
                    catch (Exception ex)
                    {
                        // Log a warning
                        _logger.Warn(ex.Message);
                    }


                }
                else
                {
                    _logger.Debug("Autoupdater will either run onCreate or onUpdate events only..");
                    return;
                }
            }
        }

        #endregion Special AU Overrides

        /// <summary>
        /// Get bearing angle using Math formula thru latitude and longitudes
        /// </summary>
        /// <param name="lat1">Latitude 1</param>
        /// <param name="lon1">Longitude 1</param>
        /// <param name="lat2">Latitude 2</param>
        /// <param name="lon2">Longitude 2</param>
        /// <returns>Bearing angle</returns>
        static double DegreeBearing(double lat1, double lon1,double lat2, double lon2)
        {
            try
            {
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

        /// <summary>
        /// Convert Point to Latitude and Longitude
        /// </summary>
        /// <param name="pPoint">Input point</param>
        /// <param name="_dLatitude">Latitude</param>
        /// <param name="_dLongitude">Longitude</param>
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
                _logger.Debug("Error while converting from Point to Latitude and Longitude" );
                throw ex;
            }
        }
        /// <summary>
        /// Validate MeasuredLatitude and MeasuredLongitude field values based on range domain
        /// </summary>
        /// <param name="latitude">MeasuredLatitude value</param>
        /// <param name="longitude">MeasuredLongitude value</param>
        /// <param name="measuredLatitudeField">MeasuredLatitude Field</param>
        /// <param name="measuredLongitudeField">MeasuredLongitude Field</param>
        /// <returns>True if all values correct otherwise false.</returns>
        static bool ValidateDomainValue(double latitude, double longitude, IField measuredLatitudeField, IField measuredLongitudeField)
        { 
            try{
                if (measuredLatitudeField.Domain != null && measuredLongitudeField.Domain != null)
                {
                    if (measuredLatitudeField.Domain is IRangeDomain && measuredLatitudeField.Domain is IRangeDomain)
                    {
                        IRangeDomain pRangeDomainLatitude = (IRangeDomain)measuredLatitudeField.Domain;
                        IRangeDomain pRangeDomainLongitude = (IRangeDomain)measuredLongitudeField.Domain;
                        if (latitude >= (double)pRangeDomainLatitude.MinValue && latitude <= (double)pRangeDomainLatitude.MaxValue)
                            if (longitude >= (double)pRangeDomainLongitude.MinValue && longitude <= (double)pRangeDomainLongitude.MaxValue)
                                return true;
                        
                    }
                    _logger.Warn("Measured latitude and Measured Longitude values are not with in the range of domain.");
                    return false;
                }
                else
                {
                    _logger.Warn("Domain not assigned properly on Measuredlatitude and MeasuredLongitude Field");
                    return false;
                }

            }
            catch(Exception ex)
            {
                return false;
            }
            
        
        }
        /// <summary>
        /// Create Point feature from latitude and longitude values with respect to map spatial reference
        /// </summary>
        /// <param name="latitude">Latitude Value</param>
        /// <param name="longitude">Longitude Value</param>
        /// <param name="pPoint">Input feature</param>
        /// <returns>Point from latitude and longitude in map's spatial reference</returns>
        static IPoint GetPointFromLatLong(double latitude, double longitude, IPoint pPoint)
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
    }
}
