using System;
using ESRI.ArcGIS.Geodatabase;
using System.Data;
namespace PGE.Common.Delivery.Geodatabase.ChangeDetection
{
    /// <summary>
    /// 
    /// </summary>
    public interface IDifferenceManager
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="inputDict"></param>
        /// <returns></returns>
        DifferenceDictionary AddObjectsRelatedToObjects(DifferenceDictionary inputDict);
        /// <summary>
        /// 
        /// </summary>
        IVersion CommonAncestorVersion { get; }
        /// <summary>
        /// 
        /// </summary>
        DifferenceDictionary Deletes { get; }
        /// <summary>
        /// 
        /// </summary>
        DifferenceDictionary Inserts { get; }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="differenceTypes"></param>
        /// <returns></returns>
        bool LoadDifferences(bool loadFeederManager20Updates, params ESRI.ArcGIS.GeoDatabaseDistributed.esriDataChangeType[] differenceTypes);
        /// <summary>
        /// 
        /// </summary>
        IVersion SourceVersion { get; }
        /// <summary>
        /// 
        /// </summary>
        IVersion TargetVersion { get; }
        /// <summary>
        /// 
        /// </summary>
        DifferenceDictionary Updates { get; }
    }
}
