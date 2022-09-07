using System;
using System.Collections.Generic;
using System.Text;

using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geodatabase;
using Miner.Interop;
using Miner.Framework;


namespace PGE.Common.Delivery.Framework
{
    /// <summary>
    /// Geometry Facade is a set of functionalities that can be used to work with geometries. 
    /// Find nearest feature, create a geometry, identify geometry types etc.
    /// </summary>
    public class GeometryFacade
    {

        #region Public Methods

        /// <summary>
        /// The GetClosestFeatureToPoint can be used to find a nearest feature in a featureclass given a point.
        /// This method recursively builds a featurecache starting with a initialsize until it reaches the MaxSize
        /// and finds the nearest feature in a featureclass. If there are no features found it returns a null.
        /// </summary>
        /// <param name="feCache">feCache is a parametr of type IFeatureCache. When initially called this can be null</param>
        /// <param name="pointToSearchFrom">pointToSearchFrom is a parameter of type IPoint.The point from which the feature should be found</param>
        /// <param name="featureClassToSearch">featureClassToSerach is a parameter of type IFeatureClass.The featureclass to be searched</param>
        /// <param name="initalSize">initialSize is a parameter of type double.The initial distance that should be used for searching the feature</param>
        /// <param name="maxSize">maxSize is a parameter of type double.The max distance upto which the feature should be searched.</param>
        /// <returns>Returns the nearest feature,of type IFeature, found with in the given intial distance and max distance from the point specified</returns>
        public static IFeature GetClosestFeatureToPoint(ref IFeatureCache feCache, IPoint pointToSearchFrom, IFeatureClass featureClassToSearch, double initalSize, double maxSize)
        {
            try
            {
                //It would reach here only when there are no features found with in the mx search distance
                if (initalSize > maxSize) return null; 
                if (feCache == null)
                {
                    //Debug Info
                    Console.WriteLine("About to init feature class");
                    if (InitFeatureCache(ref feCache, pointToSearchFrom, featureClassToSearch, initalSize, maxSize) == false)
                    {
                        return null;
                    }
                    Console.WriteLine(feCache.Count + " features in the feature cache");
                }
                //Get the closest feature in the FeatureCache and its distance.  
                IProximityOperator proxOp = (IProximityOperator)pointToSearchFrom;
                IFeature closestFeature = null;
                double closestDistance = initalSize;
                for (int i = 0; i < feCache.Count; i++)
                {
                    IFeature nextFeature = feCache.get_Feature(i);
                    double testDist = proxOp.ReturnDistance(nextFeature.Shape);
                    if (testDist < closestDistance && testDist < maxSize)
                    {
                        closestDistance = testDist;
                        closestFeature = nextFeature;
                    }
                }
                //If we couldn't find a feature in the feature cache, then set the cache to null and re-call the procedure
                if (closestFeature == null)
                {
                    feCache = null;
                    return GetClosestFeatureToPoint(ref feCache, pointToSearchFrom, featureClassToSearch, initalSize * 2, maxSize);
                }
                //NOTE!!  The closest feature in the feature cache is NOT necessarily the closest feature overall.  We have to test to make sure 
                //that there can't possibly be any features outside of the cache that are closer.  We can do that by looking north, south, east, and
                //west from the point at the "closestDistance".  If all four of those points are in the cache, then we are OK.  If not, then we need to
                //recursively call this procedure and double the initialSize of the cache
                IPoint northPoint = new PointClass();
                northPoint.PutCoords(pointToSearchFrom.X, pointToSearchFrom.Y + closestDistance);
                IPoint southPoint = new PointClass();
                southPoint.PutCoords(pointToSearchFrom.X, pointToSearchFrom.Y - closestDistance);
                IPoint eastPoint = new PointClass();
                eastPoint.PutCoords(pointToSearchFrom.X + closestDistance, pointToSearchFrom.Y);
                IPoint westPoint = new PointClass();
                westPoint.PutCoords(pointToSearchFrom.X - closestDistance, pointToSearchFrom.Y);
                if (!(feCache.Contains(northPoint) && feCache.Contains(southPoint) && feCache.Contains(eastPoint) && feCache.Contains(westPoint)))
                {
                    feCache = null;
                    return GetClosestFeatureToPoint(ref feCache, pointToSearchFrom, featureClassToSearch, initalSize * 2, maxSize);
                }
                else
                {
                    return closestFeature;
                }
            }
            catch (Exception ex)
            {
                //Add loggin information after talking to the TEAM.
                //EventLogger.Error(ex);
                string msg = ex.Message;
                return null;
            }
        }

        /// <summary>
        /// Given an IObject checks if it is a Feature and if the Shape property of the Feature changed.
        /// </summary>
        /// <param name="obj">IObject that should be cheked for</param>
        /// <returns>True if the passed in object is of type IFeature and the shape proerty of the IFeature has been changed</returns>
        public static bool ShapeChanged(IObject obj)
        {
            bool retVal = false;
            if (obj is IFeature)
            {
                IFeatureChanges featureChanges = (IFeatureChanges)obj;
                if (featureChanges.ShapeChanged) retVal=true;
            }
            return retVal;
        }

        /// <summary>
        /// Has an IPoint, a new Geographic Coordinate System and a new Spatial Reference passed in 
        /// and returns an IPoint reprojected to the new Geographic Coordinate system.
        /// The esriTransformDirection should be set to esriTransformReverse if the PointToConvert is in a
        /// Projected Coordinate System i.e. X and Y values.  (Currently used by the Get Coordinates)
        /// The esriTransformDirection should be set to esriTransformForward if the PointToConvert is in a
        /// Geographic Coordinate System i.e. Decimal Degree values. (Currently used by the Goto Coordinates)
        /// NOTE:  This method does not work for Projected Coordinate Systems.
        /// </summary>
        /// <returns>
        /// The reprojected IPoint
        /// </returns>
        public static IPoint Project(IPoint pointToConvert, esriSRGeoCSType newGeoCoordSystem, ISpatialReference newSpatialReference, esriTransformDirection transformDirection)
        {
            //Create a projected coordinate system using the selected projected coordinate systems
            ISpatialReferenceFactory spatialReferenceFactory = new SpatialReferenceEnvironmentClass();

            //Create a Geographic Coordinate System based on the parameter passed in
            IGeographicCoordinateSystem geographicCoordinateSystem = spatialReferenceFactory.CreateGeographicCoordinateSystem((int)newGeoCoordSystem);
            //Create the Geographic transformation
            IGeoTransformation pGeoTransformation = (IGeoTransformation)spatialReferenceFactory.CreateGeoTransformation((int)esriSRGeoTransformation3Type.esriSRGeoTransformation_NAD_1983_harn_To_WGS_1984);
            //QI from the Geographic Coordinate System to the ISpatialReference
            ISpatialReference spatialReferenceGCS = geographicCoordinateSystem;
            //Create a new point
            IPoint newPoint = new Point();
            //If transforming from X/Y to Decimal Degrees
            if (transformDirection == esriTransformDirection.esriTransformReverse)
            {
                //Set the GCS Spatial Reference False Origin and Units
                spatialReferenceGCS.SetFalseOriginAndUnits(-180, -90, 1000000);
                //Set the coordinate system of the point to the passed in Spatial Reference
                newPoint.SpatialReference = newSpatialReference;
            }
            else
            {
                //Set the coordinate system of the point
                newPoint.SpatialReference = spatialReferenceGCS;
            }
            //Set in the coordinates of the new point
            newPoint.PutCoords(pointToConvert.X, pointToConvert.Y);
            //Get the geometry of this point.
            IGeometry2 pointGeom2 = (IGeometry2)newPoint;
            //Now project to new Coordinate system
            //If transforming from X/Y to Decimal Degrees
            if (transformDirection == esriTransformDirection.esriTransformReverse)
            {
                pointGeom2.ProjectEx(geographicCoordinateSystem, transformDirection, pGeoTransformation, false, 0, 0);
            }
            else
            {
                pointGeom2.ProjectEx(newSpatialReference, transformDirection, pGeoTransformation, false, 0, 0);
            }
            return newPoint;
        }

        /// <summary>
        /// Has an IPoint and a new Geographic Coordinate System  passed in 
        /// and reprojects the point to the new coordinate system.
        /// Use this method if want to use the existing maps Spatial Reference as the new Spatial Reference
        /// Overloaded method with only 3 parameter2 passed in
        /// </summary>
        /// <returns>
        /// The reprojected IPoint
        /// </returns>        
        public static IPoint Project(IPoint pointToConvert, esriSRGeoCSType newGeoCoordSystem, esriTransformDirection transformDirection)
        {
            //Instantiate the ArcGIS RunTimeEnvironment object
            IMMArcGISRuntimeEnvironment rte = new ArcGISRuntimeEnvironment();
            if (rte == null || rte.ActiveView == null) return null;
            ISpatialReference CurrSpatRef = (ISpatialReference)rte.ActiveView.FocusMap.SpatialReference;
            return Project(pointToConvert, newGeoCoordSystem, CurrSpatRef, transformDirection);
        }

        /// <summary>
        /// Has an IPoint passed in and reprojects the point to the new coordinate system.
        /// Use this method if want to use the existing maps Spatial Reference as the new Spatial Reference
        /// and want to use the esriSRGeoCSType.esriSRGeoCS_WGS1984 as the Geographic Coordinate System.
        /// Overloaded method with only 2 parameters passed in
        /// </summary>
        /// <returns>
        /// The reprojected IPoint
        /// </returns>
        public static IPoint Project(IPoint pointToConvert, esriTransformDirection transformDirection)
        {
            return Project(pointToConvert, esriSRGeoCSType.esriSRGeoCS_WGS1984, transformDirection);
        }

        /// <summary>
        /// Given the XMin, YMin, XMax, YMax will create a polygon with the following point ombination
        /// XMin,YMin - XMin,YMax - XMax,YMax - XMax,YMin - XMin,YMin
        /// </summary>
        /// <param name="xmin">Minimum X Position</param>
        /// <param name="ymin">Minimum Y Position</param>
        /// <param name="xmax">Maximum X Position</param>
        /// <param name="ymax">Maximum Y Position</param>
        /// <returns>IPolygon</returns>
        public static IPolygon PolygonFromBounds(double xmin, double ymin, double xmax, double ymax)
        {
            IPointCollection4 pointColl = new PolygonClass();
            IGeometryBridge2 geomBridge = new GeometryEnvironmentClass();
            WKSPoint[] pointBuffer = new WKSPoint[5];
            pointBuffer[0] = new WKSPoint();
            pointBuffer[0].X = xmin;
            pointBuffer[0].Y = ymin;
            pointBuffer[1] = new WKSPoint();
            pointBuffer[1].X = xmin;
            pointBuffer[1].Y = ymax;
            pointBuffer[2] = new WKSPoint();
            pointBuffer[2].X = xmax;
            pointBuffer[2].Y = ymax;
            pointBuffer[3] = new WKSPoint();
            pointBuffer[3].X = xmax;
            pointBuffer[3].Y = ymin;
            pointBuffer[4] = new WKSPoint();
            pointBuffer[4].X = xmin;
            pointBuffer[4].Y = ymin;
            geomBridge.AddWKSPoints(pointColl, ref pointBuffer);  
            return (IPolygon)pointColl;
        }

        /// <summary>
        /// Given the envelope will create a polygon for the Envelope
        /// </summary>
        /// <param name="envelope">Envelope to which the polygon should be constructed</param>
        /// <returns>IPolygon. Returns null if the passed in Envelope is NULL or the pass in envelope IsEmpty</returns>
        public static IPolygon PolygonFromEnvelope(IEnvelope envelope)
        {
            if (envelope==null||envelope.IsEmpty) return null;
            return PolygonFromBounds(envelope.XMin, envelope.YMin, envelope.XMax, envelope.YMax);  
        }

        /// <summary>
        /// Given a list of IPoints will create a polygon
        /// </summary>
        /// <param name="points">List of points from which the polygon should be created</param>
        /// <returns>Return a IPolygon. If the number of points is less than or equal to 2 then returns null</returns>
        public static IPolygon PolygonFromPoints(List<IPoint> points)
        {
            if (points==null || points.Count <= 2) return null;
            IPointCollection4 pointColl = new PolygonClass();
            IGeometryBridge2 geomBridge = new GeometryEnvironmentClass();
            WKSPoint[] wkspoints = new WKSPoint[points.Count];
            
            for (int i=0;i<points.Count;i++)
            {
                wkspoints[i] = new WKSPoint();
                wkspoints[i].X = points[i].X;
                wkspoints[i].Y = points[i].Y;
            }
            geomBridge.AddWKSPoints(pointColl, ref wkspoints);
            return (IPolygon)pointColl;
        }
        #endregion

        #region Private Methods
        /// <summary>
        ///Initializes the cache with a given size and location on the featureclass.
        /// The featurecache is reinitialized until a feature is located or the MaxDistance value is reached, which ever occurs earlier.
        /// </summary>
        /// <param name="feCache">feCache is a parameter of type IFEatureCache</param>
        /// <param name="startPoint">startPoint is a parameter of type IPoint</param>
        /// <param name="featureClassToSearch"></param>
        /// <param name="initialDistance">initialDistance is a parameter of type double - This is a distance with which the featurecahce will be built initially</param>
        /// <param name="maxDist">maxDist is a parameter of type double - This is the maximum distance before the featurecache creation stops.</param>
        /// <returns>Returns true if a feature is found with in the given maxdistance</returns>
        private static bool InitFeatureCache(ref IFeatureCache feCache, IPoint startPoint, IFeatureClass featureClassToSearch, double initialDistance, double maxDist)
        {
            double size = initialDistance;
            bool firstTime = true;
            //Keep looping until we have found at least one feature
            int i = 1;
            //Console.WriteLine("Initializing " + featureClassToSearch.AliasName);
            feCache = new FeatureCacheClass();
            while (feCache.Count == 0)
            {
                if (firstTime == false)
                {
                    size = initialDistance * (Math.Pow(2, i)); //if initialDistance = 20, then...20,40,80,160,
                }
                feCache.Initialize(startPoint, size);
                feCache.AddFeatures(featureClassToSearch);
                if ((i > 20) || (feCache.Count > 0))
                {
                    return true;
                }
                if (size >= maxDist)
                {
                    return false;
                }
                i++;
                firstTime = false;
            }
            return false;
        }

        #endregion

    }
}
