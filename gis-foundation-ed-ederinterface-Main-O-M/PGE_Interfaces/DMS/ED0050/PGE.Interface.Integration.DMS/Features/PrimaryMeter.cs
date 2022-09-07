using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PGE.Interface.Integration.DMS.Manager;

namespace PGE.Interface.Integration.DMS.Features
{
    /// <summary>
    /// Get field values specific to a Primary Meter that require custom logic
    /// </summary>
    public class PrimaryMeter : IFeatureValues
    {
        /// <summary>
        /// Currently does not do anything
        /// </summary>
        /// <param name="row">The LOAD row to populate the values on</param>
        /// <param name="info">The Primary Meter object</param>
        public void getValues(System.Data.DataRow row, Miner.Geodatabase.Integration.FeatureInfo info, ControlTable controlTable)
        {
            //row["SG_KEY"] = 5;
        }
    }
}
