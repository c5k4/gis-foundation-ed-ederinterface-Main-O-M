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
    public interface IVersionDiff
    {
        /// <summary>
        /// 
        /// </summary>
        esriDataChangeType DataChangeType { get; set; }
    }
}
