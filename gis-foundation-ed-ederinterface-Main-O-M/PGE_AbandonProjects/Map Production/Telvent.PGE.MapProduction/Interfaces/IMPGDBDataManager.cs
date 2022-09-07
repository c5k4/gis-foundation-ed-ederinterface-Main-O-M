using System;
using System.Collections.Generic;
using ESRI.ArcGIS.Geodatabase;
namespace Telvent.PGE.MapProduction
{
    /// <summary>
    /// Interface to access the Geodatabase and get required IMPGDBData
    /// </summary>
    public interface IMPGDBDataManager
    {
        /// <summary>
        /// The Workspace from where the Map Production Toolset should get the Map Polygons
        /// </summary>
        IWorkspace Workspace { get; }
        /// <summary>
        /// The Map Polygon Data structure to get the required fields
        /// </summary>
        IMPGDBFieldLookUp GDBFieldLookUp { get; }
        /// <summary>
        /// Gets the data from the Geodatabase. Given a list of Sting in Key format. Tghe Key format should match the Key provided by IMPGDBData
        /// </summary>
        /// <param name="filterKeys">Lsit of Keys to filter data. If this is not provided will return all the rows from Geodatabase</param>
        /// <returns></returns>
        List<IMPGDBData> GetMapData(params string[] filterKeys);
        /// <summary>
        /// Map Number field index
        /// </summary>
        int MapNumberFieldIndex { get; }
        /// <summary>
        /// The Map Polygon featureclass from the Workspace
        /// </summary>
        IFeatureClass MapPolygonFeatureClass { get; }
        /// <summary>
        /// Field indices for all the fields 
        /// </summary>
        Dictionary<string, int> OtherFieldIndices { get; }
    }
}
