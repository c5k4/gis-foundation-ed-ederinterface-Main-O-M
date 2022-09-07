using System;
using System.Collections.Generic;
using ESRI.ArcGIS.Geodatabase;
using System.Data;
namespace PGE.Common.Delivery.Geodatabase.ChangeDetection
{
    /// <summary>
    /// 
    /// </summary>
    public interface IFilterDifferenceManager
    {
        /// <summary>
        /// 
        /// </summary>
        void ClearFilterCache();
        /// <summary>
        /// 
        /// </summary>
        IEnumerable<string> ExcludeClassModelNames { get; set; }
        /// <summary>
        /// 
        /// </summary>
        IEnumerable<esriDatasetType> ExcludeDatasetTypes { get; set; }
        /// <summary>
        /// 
        /// </summary>
        IEnumerable<esriFeatureType> ExcludeFeatureTypes { get; set; }
        /// <summary>
        /// 
        /// </summary>
        IEnumerable<string> ExcludeFieldModelNames { get; set; }
        //DifferenceDictionary FilteredUpdates { get; }
        //DataSet FilteredUpdatesDataSet(DifferenceDictionaryWorkspace workspaceType);
        /// <summary>
        /// 
        /// </summary>
        IEnumerable<FilterType> FilterList { get; set; }
        /// <summary>
        /// 
        /// </summary>
        IEnumerable<string> IncludeClassModelNames { get; set; }
        /// <summary>
        /// 
        /// </summary>
        IEnumerable<esriDatasetType> IncludeDatasetTypes { get; set; }
        /// <summary>
        /// 
        /// </summary>
        IEnumerable<esriFeatureType> IncludeFeatureTypes { get; set; }
        /// <summary>
        /// 
        /// </summary>
        IEnumerable<string> IncludeFieldModelNames { get; set; }
        /// <summary>
        /// 
        /// </summary>
        IEnumerable<string> TableNames { get; set; }
        /// <summary>
        /// 
        /// </summary>
        Dictionary<string, bool> WasFilteredResults { get; }
    }
}
