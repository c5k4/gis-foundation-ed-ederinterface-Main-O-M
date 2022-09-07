using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Miner.Interop;
using ESRI.ArcGIS.GeoDatabaseDistributed;

namespace PGE.Common.Delivery.Geodatabase
{
    /// <summary>
    /// 
    /// </summary>
    public class VersionDiffD8List:D8ListClass,IVersionDiff
    {
        /// <summary>
        /// 
        /// </summary>
        public esriDataChangeType DataChangeType
        {
            get;
            set;
        }
        /// <summary>
        /// 
        /// </summary>
        public override string DisplayName
        {
            get
            {
                if (DataChangeType == esriDataChangeType.esriDataChangeTypeDelete)
                    return "DELETE";
                else if (DataChangeType == esriDataChangeType.esriDataChangeTypeInsert)
                    return "INSERT";
                else if (DataChangeType == esriDataChangeType.esriDataChangeTypeUpdate)
                    return "UPDATES";
                return string.Empty;
            }
        }
    }
}
