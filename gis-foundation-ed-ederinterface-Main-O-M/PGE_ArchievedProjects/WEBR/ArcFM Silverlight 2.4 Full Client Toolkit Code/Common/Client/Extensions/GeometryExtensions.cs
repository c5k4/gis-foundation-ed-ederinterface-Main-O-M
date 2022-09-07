using System.Collections.ObjectModel;
using System.Globalization;
using System.Text;

using ESRI.ArcGIS.Client.Geometry;

#if SILVERLIGHT
namespace Miner.Server.Client
#elif WPF
namespace Miner.Mobile.Client
#endif
{
    internal static class GeometryExtensions
    {
        internal static bool EqualsPolygon(this Polygon source, Polygon polygon)
        {
            if (source.Extent.Equals(polygon.Extent))
            {
                return source.Rings.EqualsPoints(polygon.Rings);
            }
            return false;
        }

        internal static bool EqualsMultiPoint(this MultiPoint source, MultiPoint points)
        {
            if (source.Extent.Equals(points.Extent))
            {
                return source.Points.EqualsPoints(source.Points);
            }
            return false;
        }

        internal static bool EqualsPolyline(this Polyline source, Polyline polyline)
        {
            if (source.Extent.Equals(polyline.Extent))
            {
                return source.Paths.EqualsPoints(polyline.Paths);
            }
            return false;
        }

        internal static bool EqualsPoints(this ObservableCollection<PointCollection> source, ObservableCollection<PointCollection> points)
        {
            if (source.Count != points.Count) return false;
            for (int i = 0; i < source.Count; i++)
            {
                if (source[i].EqualsPoints(points[i]) == false) return false;
            }

            return true;
        }

        internal static bool EqualsPoints(this PointCollection source, PointCollection points)
        {
            if (source.Count != points.Count) return false;

            for (int i = 0; i < source.Count; i++)
            {
                if (source[i].Equals(points[i]) == false) return false;
            }
            return true;
        }

        internal static string ToJson(this SpatialReference source)
        {
            if (source == null) return string.Empty;

            if ((source.WKID < 1) && !string.IsNullOrEmpty(source.WKT))
            {
                return string.Format("{{\"wkt\":\"{0}\"}}", source.WKT.Replace("\"", "\\\""));
            }
            return string.Format("{{\"wkid\":{0}}}", source.WKID);
        }

        internal static string ToJson(this MapPoint source, bool includeSpatialReference)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("{");
            sb.Append("x:");
            sb.Append(string.Format(CultureInfo.InvariantCulture, "{0}", source.X));
            sb.Append(",");
            sb.Append("y:");
            sb.Append(string.Format(CultureInfo.InvariantCulture, "{0}", source.Y));
            if (includeSpatialReference && (source.SpatialReference != null))
            {
                sb.Append(",");
                sb.Append("spatialReference:");
                sb.Append(source.SpatialReference.ToJson());
            }
            sb.Append("}"); 
            return sb.ToString();
        }
    }
}
