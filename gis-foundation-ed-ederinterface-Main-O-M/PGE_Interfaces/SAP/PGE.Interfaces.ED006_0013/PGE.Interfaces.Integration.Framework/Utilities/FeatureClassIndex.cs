// ========================================================================
// Company: PGE USA Inc., PGE Utilities Group
// Client : PG&E
// $Revision: 1 $
// ========================================================================
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geodatabase;
using System.Runtime.InteropServices;
using PGE.Common.ChangeDetectionAPI;

namespace PGE.Interfaces.Integration.Framework.Utilities
{
    /// <summary>
    /// Contains methods for quickly performing spatial queries
    /// </summary>
    public class FeatureClassIndex
    {
        /// <summary>
        /// Default Constructor
        /// </summary>
        public FeatureClassIndex()
        {
        }
        /// <summary>
        /// The feature class to get results from
        /// </summary>
        public IFeatureClass Featureclass
        {
            get;
            set;
        }
        /// <summary>
        /// The spatial index of the FeatureClass
        /// </summary>
        public IIndexQuery2 IndexQuery
        {
            get;
            set;
        }
        /// <summary>
        /// Get the feature spatially related to the input feature
        /// </summary>
        /// <param name="feature">The feature used in the query</param>
        /// <param name="spatialRelation">The type of spatial relationship</param>
        /// <returns>The result feature related to the input</returns>
        public List<IFeature> GetFeatures(IFeature feature, SpatialIndexRelation spatialRelation)
        {
            List<IFeature> retVal = new List<IFeature>();
            int oid = -1;
            double distance=0.0;
            IFeatureCursor featCursor=null;
            try
            {
                switch (spatialRelation)
                {
                    case SpatialIndexRelation.Nearest:
                        IndexQuery.NearestFeature(feature.Shape, out oid, out distance);
                        if (oid != -1)
                        {
                            retVal.Add(Featureclass.GetFeature(oid));
                        }
                        break;
                    case SpatialIndexRelation.WithinFeature:
                        oid = IndexQuery.WithinFeature(feature.Shape);
                        if (oid != -1)
                        {
                            retVal.Add(Featureclass.GetFeature(oid));
                        }
                        break;
                    case SpatialIndexRelation.Intersect:
                        object oids = null;
                        IndexQuery.IntersectedFeatures(feature.Shape, out oids);
                        featCursor = Featureclass.GetFeatures(oids, false);
                        IFeature retFeature = null;
                        while ((feature = featCursor.NextFeature()) != null)
                        {
                            retVal.Add(retFeature);
                        }
                        break;
                }
                return retVal;
            }
            finally
            {
                if (featCursor != null)
                {
                    while (Marshal.ReleaseComObject(featCursor) > 0) { };
                }
            }
        }

        /// <summary>
        /// Get the feature spatially related to the input feature
        /// </summary>
        /// <param name="feature">The feature used in the query</param>
        /// <param name="spatialRelation">The type of spatial relationship</param>
        /// <returns>The result feature related to the input</returns>
        public List<IFeature> GetFeatures(DeleteFeat feature, SpatialIndexRelation spatialRelation)
        {
            List<IFeature> retVal = new List<IFeature>();
            int oid = -1;
            double distance = 0.0;
            IFeatureCursor featCursor = null;
            IFeature feat = null;
            try
            {
                switch (spatialRelation)
                {
                    case SpatialIndexRelation.Nearest:
                        IndexQuery.NearestFeature(feature.geometry_Old, out oid, out distance);
                        if (oid != -1)
                        {
                            retVal.Add(Featureclass.GetFeature(oid));
                        }
                        break;
                    case SpatialIndexRelation.WithinFeature:
                        oid = IndexQuery.WithinFeature(feature.geometry_Old);
                        if (oid != -1)
                        {
                            retVal.Add(Featureclass.GetFeature(oid));
                        }
                        break;
                    case SpatialIndexRelation.Intersect:
                        object oids = null;
                        IndexQuery.IntersectedFeatures(feature.geometry_Old, out oids);
                        featCursor = Featureclass.GetFeatures(oids, false);
                        IFeature retFeature = null;
                        while ((feat = featCursor.NextFeature()) != null)
                        {
                            retVal.Add(retFeature);
                        }
                        break;
                }
                return retVal;
            }
            finally
            {
                if (featCursor != null)
                {
                    while (Marshal.ReleaseComObject(featCursor) > 0) { };
                }
            }
        }
    }
    /// <summary>
    /// How two objects are related spatially
    /// </summary>
    public enum SpatialIndexRelation
    {
        /// <summary>
        /// The query feature touches result feature
        /// </summary>
        Intersect,
        /// <summary>
        /// The query feature is inside the result feature
        /// </summary>
        WithinFeature,
        /// <summary>
        /// The nearest result feature to the query feature
        /// </summary>
        Nearest
    }
}
