using System;
using System.Collections.Generic;
using ESRI.ArcGIS.Geodatabase;
namespace PGE.BatchApplication.PSPSMapProduction
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
        /// Gets the data from the Geodatabase. Given a list of Sting in Key format. The Key format should match the Key provided by IMPGDBData
        /// </summary>
        /// <param name="filterKeys">Lsit of Keys to filter data. If this is not provided will return all the rows from Geodatabase</param>
        /// <returns></returns>
        List<IMPGDBData> GetMapData(params string[] filterKeys);

        /// <summary>
        /// Retrieve the feature class from an SDE Oracle connection file and the feature class name.
        /// </summary>
        /// <param name="sdeConnectionFile">SDE Connection file</param>
        /// <param name="layerName">Layer Name</param>
        /// <returns>FeatureClass</returns>
        IFeatureClass GetGDBLayer(string sdeConnectionFile, string layerName);
        /// <summary>
        /// Circuit ID field index
        /// </summary>
        int CircuitIdFieldIndex { get; }
        /// <summary>
        /// Circuit Name field index
        /// </summary>
        int CircuitNameFieldIndex { get; }
        /// <summary>
        /// PSPS Name field index
        /// </summary>
        int PSPSNameFieldIndex { get; }
        /// <summary>
        /// The Primary OH Conductor featureclass from the Workspace
        /// </summary>
        IFeatureClass PriOHConductorFeatureClass { get; }
        /// <summary>
        /// Field indices for all the fields 
        /// </summary>
        Dictionary<string, int> OtherFieldIndices { get; }
    }
}
