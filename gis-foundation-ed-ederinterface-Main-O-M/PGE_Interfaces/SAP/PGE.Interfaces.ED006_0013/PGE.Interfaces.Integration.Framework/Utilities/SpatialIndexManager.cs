// ========================================================================
// Company: PGE USA Inc., PGE Utilities Group
// Client : PG&E
// $Revision: 1 $
// ========================================================================
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Carto;

namespace PGE.Interfaces.Integration.Framework.Utilities
{
    /// <summary>
    /// Utility for managing Spatial Indexes which greatly improve the performance of spatial searches.
    /// </summary>
    public static class SpatialIndexManager
    {
        [ThreadStatic]
        private static Dictionary<string, FeatureClassIndex> _spatialCache = new Dictionary<string, FeatureClassIndex>();

        /// <summary>
        /// Builds a Spatial Index
        /// </summary>
        /// <param name="featureclassName">The name of the class for which to build the index</param>
        /// <param name="workspace">A reference to an open workspace, i.e. database connection</param>
        /// <returns>The spatial index</returns>
        [STAThread]
        public static FeatureClassIndex GetFeatureclassIndex(string featureclassName,IWorkspace workspace)
        {
            FeatureClassIndex retVal = null;
            IFeatureClass indexFeatureClass=((IFeatureWorkspace)workspace).OpenFeatureClass(featureclassName);
            if (_spatialCache == null)
            {
                _spatialCache = new Dictionary<string, FeatureClassIndex>();
            }
            if (_spatialCache.ContainsKey(featureclassName.ToUpper()))
            {
                retVal = _spatialCache[featureclassName.ToUpper()];
            }
            else
            {
                retVal=new FeatureClassIndex();
                retVal.Featureclass = indexFeatureClass;
                IFeatureIndex2 featureIndex = new FeatureIndexClass();
                featureIndex.FeatureClass = indexFeatureClass;
                featureIndex.Index(null, null);
                retVal.IndexQuery = (IIndexQuery2)featureIndex; 
                _spatialCache.Add(featureclassName.ToUpper(), retVal);
            }
            return retVal;
        }
    }
}
