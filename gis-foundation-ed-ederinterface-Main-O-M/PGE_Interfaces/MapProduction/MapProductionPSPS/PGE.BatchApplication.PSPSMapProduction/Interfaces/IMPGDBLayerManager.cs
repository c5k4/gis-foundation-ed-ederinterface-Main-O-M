using System;
using System.Collections.Generic;
using ESRI.ArcGIS.Geodatabase;
namespace PGE.BatchApplication.PSPSMapProduction
{
    /// <summary>
    /// Interface to access the Geodatabase and get required IMPGDBData
    /// </summary>
    public interface IMPGDBLayerManager
    {
        /// <summary>
        /// The Workspace from where the Map Production Toolset should get the Map Polygons
        /// </summary>
        string SdeConnectionFile { get; }
        /// <summary>
        /// The Map Polygon Data structure to get the required fields
        /// </summary>
        string FeatureClassName { get; }

        /// <summary>
        /// Gets the data from the Geodatabase. Given a field names, RIS Name and circuit name
        /// </summary>
        /// <param name="fieldNames">Lsit of field names</param>
        /// <param name="riaName">Circuit Id</param>
        /// <param name="circuitName">Circuit Name</param>
        /// <returns></returns>
        List<PSPSSegmentRecord> GetLayerData(string fieldNames, string circuitId);
    }
}
