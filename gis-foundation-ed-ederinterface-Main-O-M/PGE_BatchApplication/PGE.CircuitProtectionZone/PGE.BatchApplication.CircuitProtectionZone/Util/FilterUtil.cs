using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;

namespace PGE.BatchApplication.CircuitProtectionZone.Util
{
    public class FilterUtil
    {
        public static IQueryFilter CreateSpatialFilterFromGeometry(IGeometry geometry, IFeatureClass featureClass)
        {
            IGeoDataset geoDataset = (IGeoDataset)featureClass;

            SpatialFilterClass spatialFilter = new SpatialFilterClass();
            spatialFilter.Geometry = geometry;

            string shpfldnm = featureClass.ShapeFieldName;
            spatialFilter.GeometryField = shpfldnm;
            spatialFilter.set_OutputSpatialReference(shpfldnm, geoDataset.SpatialReference);
            spatialFilter.SpatialRel = esriSpatialRelEnum.esriSpatialRelIntersects;

            return spatialFilter;
        }

        public static IQueryFilter CreateQueryFilter(IFeatureClass featureClass, string whereClause)
        {
            QueryFilterClass filter = null;

            if (string.IsNullOrEmpty(whereClause) == false)
            {
                filter = new QueryFilterClass();
                filter.WhereClause = whereClause;
            }

            return filter;
        }

        public static IQueryFilter CreateQueryFilter(ITable table, string whereClause)
        {
            QueryFilterClass filter = null;

            if (string.IsNullOrEmpty(whereClause) == false)
            {
                filter = new QueryFilterClass();
                filter.WhereClause = whereClause;
            }

            return filter;
        }

        public static string CreateSimpleWhereClause(IFeatureClass featureClass, string fieldName, object fieldValue)
        {
            string querySyntax = "";

            if (EsriFieldsUtil.IsFieldSupported(featureClass, fieldName) == true)
            {
                querySyntax = EsriFieldsUtil.GetFieldValueSyntax(featureClass, fieldName, fieldValue);
            }

            return querySyntax;
        }

        public static string CreateSimpleWhereClause(ITable table, string fieldName, object fieldValue)
        {
            string querySyntax = "";

            if (EsriFieldsUtil.IsFieldSupported(table, fieldName) == true)
            {
                querySyntax = EsriFieldsUtil.GetFieldValueSyntax(table, fieldName, fieldValue);
            }

            return querySyntax;
        }
    }
}
