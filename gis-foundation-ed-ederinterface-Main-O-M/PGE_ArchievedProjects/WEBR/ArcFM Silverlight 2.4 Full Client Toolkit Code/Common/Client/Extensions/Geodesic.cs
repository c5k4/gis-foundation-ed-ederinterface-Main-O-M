using System;

using ESRI.ArcGIS.Client.Geometry;

#if SILVERLIGHT
namespace Miner.Server.Client
#elif WPF
namespace Miner.Mobile.Client
#endif
{
    /// <summary>
    /// Extension methods for geodesic calculations.
    /// 
    /// All points must be in WGS decimal degrees. (WKID 4326)
    /// 
    /// Uses meter lengths based on the constant definition for the earths radius.
    /// </summary>
    internal static class Geodesic
    {
        //private const double EarthRadius = 6378.137; //kilometers. Change to miles to return all values in miles instead
        private const double EarthRadius = 6378137; // Meters. Change to other units to get all values in that unit
        private const int WgsId = 4326;

        /// <summary>
        /// Gets the distance between two points.
        /// </summary>
        /// <param name="start">The start point.</param>
        /// <param name="end">The end point.</param>
        /// <returns></returns>
        public static double GetSphericalDistance(this MapPoint start, MapPoint end)
        {
            if (start.SpatialReference.WKID != WgsId) return double.NaN;
            if (end.SpatialReference.WKID != WgsId) return double.NaN;

            // Convert to radians
            double lonS = start.X / 180 * Math.PI;
            double lonF = end.X / 180 * Math.PI;
            double latS = start.Y / 180 * Math.PI;
            double latF = end.Y / 180 * Math.PI;

            double dLon = lonS - lonF;

            // Less precise and with more opportunity for error (however slight that opportunity may be)
            //double dAngle = 2 * Math.Asin(Math.Sqrt(Math.Pow((Math.Sin((latS - latF) / 2)), 2) + Math.Cos(latS) * Math.Cos(latF) * Math.Pow(Math.Sin((lonS - lonF) / 2), 2)));

            double dAngle = Math.Atan2((Math.Sqrt(Math.Pow(Math.Cos(latF) * Math.Sin(dLon), 2) + Math.Pow(Math.Cos(latS) * Math.Sin(latF) - Math.Sin(latS) * Math.Cos(latF) * Math.Cos(dLon), 2))), (Math.Sin(latS) * Math.Sin(latF) + Math.Cos(latS) * Math.Cos(latF) * Math.Cos(dLon)));
            double dist = EarthRadius * dAngle;

            return dist;
        }

        /// <summary>
        /// Returns a polygon with a constant distance from the center point measured on the sphere.
        /// </summary>
        /// <param name="center">The center.</param>
        /// <param name="dist">Radius</param>
        /// <returns></returns>
        public static Polygon GetRadiusAsPolygon(this MapPoint center, double dist)
        {
            if (center.SpatialReference.WKID != WgsId) return new Polygon();
            if (double.IsNaN(dist)) return new Polygon();

            Polyline line = GetRadius(center, dist);
            var poly = new Polygon();

            if (line.Paths.Count > 1)
            {
                PointCollection ring = line.Paths[0];
                MapPoint last = ring[ring.Count - 1];
                for (int i = 1; i < line.Paths.Count; i++)
                {
                    PointCollection pnts = line.Paths[i];
                    ring.Add(new MapPoint(180 * Math.Sign(last.X), 90 * Math.Sign(center.Y), new SpatialReference(WgsId)));
                    last = pnts[0];
                    ring.Add(new MapPoint(180 * Math.Sign(last.X), 90 * Math.Sign(center.Y), new SpatialReference(WgsId)));
                    foreach (MapPoint p in pnts)
                        ring.Add(p);
                    last = pnts[pnts.Count - 1];
                }
                poly.Rings.Add(ring);
                //pnts.Add(first);
            }
            else
            {
                poly.Rings.Add(line.Paths[0]);
            }
            if (dist > EarthRadius * Math.PI / 2 && line.Paths.Count != 2)
            {
                var pnts = new PointCollection
				               {
				                   new MapPoint(-180, -90, new SpatialReference(WgsId)),
				                   new MapPoint(180, -90, new SpatialReference(WgsId)),
				                   new MapPoint(180, 90, new SpatialReference(WgsId)),
				                   new MapPoint(-180, 90, new SpatialReference(WgsId)),
				                   new MapPoint(-180, -90, new SpatialReference(WgsId))
				               };
                poly.Rings.Add(pnts); //Exterior
            }

            return poly;
        }

        /// <summary>
        /// Returns a polyline with a constant distance from the center point measured on the sphere.
        /// </summary>
        /// <param name="center">The center.</param>
        /// <param name="dist">Radius</param>
        /// <returns></returns>
        public static Polyline GetRadius(this MapPoint center, double dist)
        {
            if (center.SpatialReference.WKID != WgsId) return new Polyline();
            if (double.IsNaN(dist)) return new Polyline();

            var line = new Polyline();
            var pnts = new PointCollection();
            line.Paths.Add(pnts);
            for (int i = 0; i < 360; i++)
            {
                //double angle = i / 180.0 * Math.PI;
                MapPoint p = GetPointFromHeading(center, dist, i);
                if (pnts.Count > 0)
                {
                    MapPoint lastPoint = pnts[pnts.Count - 1];
                    int sign = Math.Sign(p.X);
                    if (Math.Abs(p.X - lastPoint.X) > 180)
                    {
                        //We crossed the date line
                        double lat = LatitudeAtLongitude(lastPoint, p, sign * -180);
                        pnts.Add(new MapPoint(sign * -180, lat, new SpatialReference(WgsId)));
                        pnts = new PointCollection();
                        line.Paths.Add(pnts);
                        pnts.Add(new MapPoint(sign * 180, lat, new SpatialReference(WgsId)));
                    }
                }
                pnts.Add(p);
            }
            pnts.Add(line.Paths[0][0]);

            return line;
        }

        /// <summary>
        /// Gets the shortest path line between two points. THe line will be following the great
        /// circle described by the two points.
        /// </summary>
        /// <param name="start">The start point.</param>
        /// <param name="end">The end point.</param>
        /// <returns></returns>
        public static Polyline GetGeodesicLine(this MapPoint start, MapPoint end)
        {
            if (start.SpatialReference.WKID != WgsId) return new Polyline();
            if (end.SpatialReference.WKID != WgsId) return new Polyline();

            var line = new Polyline();
            if (Math.Abs(end.X - start.X) <= 180) // Doesn't cross dateline 
            {
                PointCollection pnts = GetGeodesicPoints(start, end);
                line.Paths.Add(pnts);
            }
            else
            {
                double lon1 = start.X / 180 * Math.PI;
                double lon2 = end.X / 180 * Math.PI;
                double lat1 = start.Y / 180 * Math.PI;
                double lat2 = end.Y / 180 * Math.PI;
                double latA = LatitudeAtLongitude(lat1, lon1, lat2, lon2, Math.PI) / Math.PI * 180;
                //double latB = LatitudeAtLongitude(lat1, lon1, lat2, lon2, -180) / Math.PI * 180;

                line.Paths.Add(GetGeodesicPoints(start, new MapPoint(start.X < 0 ? -180 : 180, latA, new SpatialReference(WgsId))));
                line.Paths.Add(GetGeodesicPoints(new MapPoint(start.X < 0 ? 180 : -180, latA, new SpatialReference(WgsId)), end));
            }

            return line;
        }

        /// <summary>
        /// Returns a polyline with a constant distance from the center point measured on the sphere.
        /// </summary>
        /// <param name="start">The start point.</param>
        /// <param name="end">The end point.</param>
        /// <returns></returns>
        public static Polyline GetQuickCircleLine(this MapPoint start, MapPoint end)
        {
            var line = new Polyline();
            var pnts = new PointCollection();
            line.Paths.Add(pnts);
            line.SpatialReference = start.SpatialReference;

            double radius = Math.Sqrt(Math.Pow(start.X - end.X, 2) + Math.Pow(start.Y - end.Y, 2));

            for (int i = 0; i < 359; i++)
            {
                double degree = i * (Math.PI / 180);
                double x = start.X + Math.Cos(degree) * radius;
                double y = start.Y + Math.Sin(degree) * radius;

                var p = new MapPoint(x, y, start.SpatialReference);
                pnts.Add(p);
            }
            pnts.Add(line.Paths[0][0].Clone());

            return line;
        }

        /// <summary>
        /// Returns a polygon with a constant distance from the center point measured on the sphere.
        /// </summary>
        /// <param name="start">The start point.</param>
        /// <param name="end">The end point.</param>
        /// <returns></returns>
        public static Polygon GetQuickCirclePolygon(this MapPoint start, MapPoint end)
        {
            var poly = new Polygon();
            var pnts = new PointCollection();
            poly.Rings.Add(pnts);
            poly.SpatialReference = start.SpatialReference;

            double radius = Math.Sqrt(Math.Pow(start.X - end.X, 2) + Math.Pow(start.Y - end.Y, 2));

            for (int i = 0; i < 359; i++)
            {
                double degree = i * (Math.PI / 180);
                double x = start.X + Math.Cos(degree) * radius;
                double y = start.Y + Math.Sin(degree) * radius;

                var p = new MapPoint(x, y, start.SpatialReference);
                pnts.Add(p);
            }
            pnts.Add(poly.Rings[0][0].Clone());

            return poly;
        }

        /// <summary>
        /// Returns a polygon with a constant distance from the center point measured on the sphere.
        /// </summary>
        /// <param name="start">The start point.</param>
        /// <param name="end">The end point.</param>
        /// <returns></returns>
        public static Polygon GetQuickOvalPolygon(this MapPoint start, MapPoint end)
        {
            var poly = new Polygon();
            var pnts = new PointCollection();
            poly.Rings.Add(pnts);
            poly.SpatialReference = start.SpatialReference;

            double radiusX = start.X - end.X;
            double radiusY = start.Y - end.Y;

            for (int i = 0; i < 359; i++)
            {
                double degree = i * (Math.PI / 180);
                double x = start.X + Math.Cos(degree) * radiusX;
                double y = start.Y + Math.Sin(degree) * radiusY;

                var p = new MapPoint(x, y, start.SpatialReference);
                pnts.Add(p);
            }
            pnts.Add(poly.Rings[0][0].Clone());

            return poly;
        }

        /// <summary>
        /// Gets the true bearing at a distance from the start point towards the new point.
        /// </summary>
        /// <param name="start">The start point.</param>
        /// <param name="end">The point to get the bearing towards.</param>
        /// <param name="distance">The distance travelled between start and end.</param>
        /// <returns></returns>
        public static double GetTrueBearing(MapPoint start, MapPoint end, double distance)
        {
            if (start.SpatialReference.WKID != WgsId) return double.NaN;
            if (end.SpatialReference.WKID != WgsId) return double.NaN;

            double d = distance / EarthRadius; //Angular distance in radians
            double lon1 = start.X / 180 * Math.PI;
            double lat1 = start.Y / 180 * Math.PI;
            double lon2 = end.X / 180 * Math.PI;
            double lat2 = end.Y / 180 * Math.PI;
            double tc1;
            if (Math.Sin(lon2 - lon1) < 0)
                tc1 = Math.Acos((Math.Sin(lat2) - Math.Sin(lat1) * Math.Cos(d)) / (Math.Sin(d) * Math.Cos(lat1)));
            else
                tc1 = 2 * Math.PI - Math.Acos((Math.Sin(lat2) - Math.Sin(lat1) * Math.Cos(d)) / (Math.Sin(d) * Math.Cos(lat1)));
            return tc1 / Math.PI * 180;
        }

        /// <summary>
        /// Gets the point based on a start point, a heading and a distance.
        /// </summary>
        /// <param name="start">The start.</param>
        /// <param name="distance">The distance.</param>
        /// <param name="heading">The heading.</param>
        /// <returns></returns>
        public static MapPoint GetPointFromHeading(MapPoint start, double distance, double heading)
        {
            if (start.SpatialReference.WKID != WgsId) return new MapPoint();

            double brng = heading / 180 * Math.PI;
            double lon1 = start.X / 180 * Math.PI;
            double lat1 = start.Y / 180 * Math.PI;
            double dR = distance / EarthRadius; //Angular distance in radians
            double lat2 = Math.Asin(Math.Sin(lat1) * Math.Cos(dR) + Math.Cos(lat1) * Math.Sin(dR) * Math.Cos(brng));
            double lon2 = lon1 + Math.Atan2(Math.Sin(brng) * Math.Sin(dR) * Math.Cos(lat1), Math.Cos(dR) - Math.Sin(lat1) * Math.Sin(lat2));
            double lon = lon2 / Math.PI * 180;
            double lat = lat2 / Math.PI * 180;
            while (lon < -180) lon += 360;
            while (lat < -90) lat += 180;
            while (lon > 180) lon -= 360;
            while (lat > 90) lat -= 180;

            return new MapPoint(lon, lat, new SpatialReference(WgsId));
        }

        private static PointCollection GetGeodesicPoints(MapPoint start, MapPoint end)
        {
            double lon1 = start.X / 180 * Math.PI;
            double lon2 = end.X / 180 * Math.PI;
            double lat1 = start.Y / 180 * Math.PI;
            double lat2 = end.Y / 180 * Math.PI;
            double dX = end.X - start.X;
            var points = (int)Math.Floor(Math.Abs(dX));
            dX = lon2 - lon1;
            var pnts = new PointCollection { start };
            for (int i = 1; i < points; i++)
            {
                double lon = lon1 + dX / points * i;
                double lat = LatitudeAtLongitude(lat1, lon1, lat2, lon2, lon);
                pnts.Add(new MapPoint(lon / Math.PI * 180, lat / Math.PI * 180, new SpatialReference(WgsId)));
            }
            pnts.Add(end);
            return pnts;
        }

        /// <summary>
        /// Gets the latitude at a specific longitude for a great circle defined by p1 and p2.
        /// </summary>
        /// <param name="p1">The p1.</param>
        /// <param name="p2">The p2.</param>
        /// <param name="lon">The longitude in degrees.</param>
        /// <returns></returns>
        private static double LatitudeAtLongitude(MapPoint p1, MapPoint p2, double lon)
        {
            double lon1 = p1.X / 180 * Math.PI;
            double lon2 = p2.X / 180 * Math.PI;
            double lat1 = p1.Y / 180 * Math.PI;
            double lat2 = p2.Y / 180 * Math.PI;
            lon = lon / 180 * Math.PI;
            return LatitudeAtLongitude(lat1, lon1, lat2, lon2, lon) / Math.PI * 180;
        }

        /// <summary>
        /// Gets the latitude at a specific longitude for a great circle defined by lat1,lon1 and lat2,lon2.
        /// </summary>
        /// <param name="lat1">The start latitude in radians.</param>
        /// <param name="lon1">The start longitude in radians.</param>
        /// <param name="lat2">The end latitude in radians.</param>
        /// <param name="lon2">The end longitude in radians.</param>
        /// <param name="lon">The longitude in radians for where the latitude is.</param>
        /// <returns></returns>
        private static double LatitudeAtLongitude(double lat1, double lon1, double lat2, double lon2, double lon)
        {
            return Math.Atan((Math.Sin(lat1) * Math.Cos(lat2) * Math.Sin(lon - lon2) - Math.Sin(lat2) * Math.Cos(lat1) * Math.Sin(lon - lon1)) / (Math.Cos(lat1) * Math.Cos(lat2) * Math.Sin(lon1 - lon2)));
        }

        ///// <summary>
        ///// Converts UTM coords to lat/long.  Equations from USGS Bulletin 1532 
        ///// East Longitudes are positive, West longitudes are negative. 
        ///// North latitudes are positive, South latitudes are negative
        ///// Lat and Long are in decimal degrees.  
        ///// </summary>
        ///// <param name="utmNorthing">Northing coordinate</param>
        ///// <param name="utmEasting">Easting coordinate</param>
        ///// <param name="zoneNumber">the zone number</param>
        ///// <param name="latitude">converted latitude</param>
        ///// <param name="longitude">converted longitude</param>
        //private static void UtmToLatLong(double utmEasting, double utmNorthing, int zoneNumber, out double longitude, out double latitude)
        //{
        //    const double pi = Math.PI;
        //    const double rad2Deg = 180.0 / pi;
        //    const double deg2Rad = pi / 180.0;

        //    const double k0 = 0.9996; // Scale along long0 (constant)
        //    const double a = 6378137; // Equatorial Radius for WGS-84
        //    const double b = 6356752.3142; // Polar Radius for WGS-84
        //    //const double eccSquared = 0.00669438; // Square of Eccentricity for WGS-84

        //    double ecc = Math.Sqrt(1.0 - (b * b) / (a * a));
        //    double eccSquared = (ecc * ecc);
        //    double eccPrimeSquared = (eccSquared) / (1 - eccSquared);

        //    double e1 = (1 - Math.Sqrt(1 - eccSquared)) / (1 + Math.Sqrt(1 - eccSquared));

        //    double x = utmEasting - 500000.0; // Relative to central meridian.
        //    double y = utmNorthing;

        //    double longOdeg;

        //    if (zoneNumber <= 30) longOdeg = (30.0 - zoneNumber) * -6.0 - 3.0;
        //    else longOdeg = (zoneNumber - 31.0) * 6.0 + 3.0;

        //    double longOrad = longOdeg * deg2Rad;

        //    double m = y / k0;
        //    double mu = m / (a * (1 - eccSquared / 4 - 3 * eccSquared * eccSquared / 64 - 5 * eccSquared * eccSquared * eccSquared / 256));

        //    double j1 = (3 * e1 / 2 - 27 * e1 * e1 * e1 / 32);
        //    double j2 = (21 * e1 * e1 / 16 - 55 * e1 * e1 * e1 * e1 / 32);
        //    double j3 = (151 * e1 * e1 * e1 / 96);
        //    double j4 = (1097 * e1 * e1 * e1 * e1 / 512);

        //    double fp = mu + j1 * Math.Sin(2 * mu)
        //                     + j2 * Math.Sin(4 * mu)
        //                     + j3 * Math.Sin(6 * mu)
        //                     + j4 * Math.Sin(8 * mu);

        //    double n1 = a / Math.Sqrt(1 - eccSquared * Math.Sin(fp) * Math.Sin(fp));
        //    double t1 = Math.Tan(fp) * Math.Tan(fp);
        //    double c1 = eccPrimeSquared * Math.Cos(fp) * Math.Cos(fp);
        //    double r1 = a * (1 - eccSquared) / Math.Pow(1 - eccSquared * Math.Sin(fp) * Math.Sin(fp), 1.5);
        //    double d = x / (n1 * k0);

        //    double q1 = (n1 * Math.Tan(fp) / r1);
        //    double q2 = (d * d / 2);
        //    double q3 = ((5 + 3 * t1 + 10 * c1 - 4 * c1 * c1 - 9 * eccPrimeSquared) * d * d * d * d / 24);
        //    double q4 = ((61 + 90 * t1 + 298 * c1 + 45 * t1 * t1 - 3 * c1 * c1 - 252 * eccPrimeSquared) * d * d * d * d * d * d / 720);
        //    double q5 = d;
        //    double q6 = ((1 + 2 * t1 + c1) * d * d * d / 6);
        //    double q7 = ((5 - 2 * c1 + 28 * t1 - 3 * c1 * c1 + 8 * eccPrimeSquared + 24 * t1 * t1) * d * d * d * d * d / 120);

        //    latitude = fp - q1 * (q2 - q3 + q4);
        //    latitude = latitude * rad2Deg;

        //    longitude = longOrad + (q5 - q6 + q7) / Math.Cos(fp);
        //    longitude = longitude * rad2Deg;
        //}

        ///// <summary>
        ///// Converts UTM coords to lat/long.  Equations from USGS Bulletin 1532 
        ///// East Longitudes are positive, West longitudes are negative. 
        ///// North latitudes are positive, South latitudes are negative
        ///// Lat and Long are in decimal degrees.  
        ///// </summary>
        ///// <param name="utmNorthing">Northing coordinate</param>
        ///// <param name="utmEasting">Easting coordinate</param>
        ///// <param name="zoneNumber">the zone number</param>
        ///// <param name="latitude">converted latitude</param>
        ///// <param name="longitude">converted longitude</param>
        //private static void LatLongToUtm(double longitude, double latitude, int zoneNumber, out double utmEasting, out double utmNorthing)
        //{
        //    const double pi = Math.PI;
        //    //const double rad2Deg = 180.0 / pi;
        //    const double deg2Rad = pi / 180.0;

        //    const double k0 = 0.9996; // Scale along long0 (constant)
        //    const double a = 6378137; // Equatorial Radius for WGS-84
        //    const double b = 6356752.3142; // Polar Radius for WGS-84
        //    //const double eccSquared = 0.00669438; // Square of Eccentricity for WGS-84

        //    double ecc = Math.Sqrt(1.0 - (b * b) / (a * a));
        //    double eccSquared = (ecc * ecc);
        //    double eccPrimeSquared = (eccSquared) / (1 - eccSquared);

        //    double e1 = (1 - Math.Sqrt(1 - eccSquared)) / (1 + Math.Sqrt(1 - eccSquared));

        //    double lon = longitude * deg2Rad;
        //    double lat = latitude * deg2Rad;

        //    const double n = (a - b)/(a + b);
        //    //double rho = a * (1 - e1 * e1)/Math.Pow(1 - e1 * e1 * Math.Sin(lat) * Math.Sin(lat), 3/2);
        //    double nu = a / Math.Pow(1 - e1 * e1 * Math.Sin(lat) * Math.Sin(lat), 1 / 2);

        //    double longOdeg;

        //    if (zoneNumber <= 30) longOdeg = (30.0 - zoneNumber) * -6.0 - 3.0;
        //    else longOdeg = (zoneNumber - 31.0) * 6.0 + 3.0;

        //    double longOrad = longOdeg * deg2Rad;

        //    double p = lon - longOrad;

        //    //double m = a * ((1 - Math.Pow(e1,2)/4 - 3 * Math.Pow(e1,4)/64 - 5 * Math.Pow(e1,6)/256) * lat
        //    //    - (3 * Math.Pow(e1,2)/8 + 3 * Math.Pow(e1,4)/32 + 45 * Math.Pow(e1,6)/1024) * Math.Sin(2 * lat)
        //    //    + (15 * Math.Pow(e1,4)/256 + 45 * Math.Pow(e1,6)/1024) * Math.Sin(4 * lat)
        //    //    - (35 * Math.Pow(e1,6)/3072) * Math.Sin(6 * lat));

        //    double aP = a * (1 - n + (5/4)*(Math.Pow(n, 2) - Math.Pow(n, 3)) + (81/64)*(Math.Pow(n, 4) - Math.Pow(n, 5)));
        //    double bP = (3 * Math.Tan(lat) / 2)*(1 - n + (7/8)*(Math.Pow(n, 2) - Math.Pow(n, 3)) + (55/64)*(Math.Pow(n, 4) - Math.Pow(n, 5)));
        //    double cP = (15 * Math.Pow(Math.Tan(lat), 2) / 16) * (1 - n + (3 / 4) * (Math.Pow(n, 2) - Math.Pow(n, 3)));
        //    double dP = (35 * Math.Pow(Math.Tan(lat), 3) / 48) * (1 - n + (11 / 16) * (Math.Pow(n, 2) - Math.Pow(n, 3)));
        //    double eP = (315 * Math.Pow(Math.Tan(lat), 4) / 512) * (1 - n);

        //    double s = aP * lat - bP * Math.Sin(2 * lat) + cP * Math.Sin(4 * lat) - dP * Math.Sin(6 * lat) + eP * Math.Sin(8 * lat);

        //    double k1 = s * k0;
        //    double k2 = k0 * nu * Math.Sin(2 * lat)/4;
        //    double k3 = (k0 * nu * Math.Sin(lat) * Math.Pow(Math.Cos(lat), 3) / 24) * (5 - Math.Pow(Math.Tan(lat), 2) + 9 * eccPrimeSquared * Math.Pow(Math.Cos(lat), 2) + 4 * eccPrimeSquared * eccPrimeSquared * Math.Pow(Math.Cos(lat), 4));
        //    double k4 = k0 * nu * Math.Cos(lat);
        //    double k5 = (k0 * nu * Math.Pow(Math.Cos(lat), 3)/6) * (1 - Math.Pow(Math.Tan(lat), 2) + eccPrimeSquared*Math.Pow(Math.Cos(lat), 2));

        //    utmNorthing = k1 + k2 * Math.Pow(p, 2) + k3 * Math.Pow(p, 4);
        //    utmEasting = k4 * p + k5 * Math.Pow(p, 3);

        //    utmEasting += 500000; // Add 500000 meters for conventional.
        //}

        //private static Polyline ConvertToUtmPolyline(Geometry geo, SpatialReference spat)
        //{
        //    if (geo is Polyline)
        //    {
        //        var oldPolyline = geo as Polyline;
        //        var polyline = new Polyline { SpatialReference = spat };

        //        foreach (PointCollection path in oldPolyline.Paths)
        //        {
        //            var newPath = new PointCollection();

        //            foreach (MapPoint point in path)
        //            {
        //                double x;
        //                double y;

        //                LatLongToUtm(point.X, point.Y, 11, out x, out y);

        //                var newPoint = new MapPoint(x, y, spat);
        //                newPath.Add(newPoint);
        //            }

        //            polyline.Paths.Add(newPath);
        //        }

        //        return polyline;

        //    }

        //    return null;
        //}

        //private static Polygon ConvertToUtmPolygon(Geometry geo, SpatialReference spat)
        //{
        //    if (geo is Polygon)
        //    {
        //        var oldPolygon = geo as Polygon;
        //        var polygon = new Polygon { SpatialReference = spat };

        //        foreach (PointCollection ring in oldPolygon.Rings)
        //        {
        //            var newRing = new PointCollection();

        //            foreach (MapPoint point in ring)
        //            {
        //                double x;
        //                double y;

        //                LatLongToUtm(point.X, point.Y, 11, out x, out y);

        //                var newPoint = new MapPoint(x, y, spat);
        //                newRing.Add(newPoint);
        //            }

        //            polygon.Rings.Add(newRing);
        //        }

        //        return polygon;
        //    }

        //    return null;
        //}

    }
}
