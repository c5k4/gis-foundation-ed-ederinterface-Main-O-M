using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PGE.Interface.Integration.DMS.Manager;

namespace PGE.Interface.Integration.DMS.Features
{
    /// <summary>
    /// Get field values specific to a ED Bus Bar that require custom logic
    /// </summary>
    public class DistrBusBar : IFeatureValues
    {
        /// <summary>
        /// Get field values specific to a ED Bus Bar. Sets the length to 10
        /// </summary>
        /// <param name="row">The LINE row to populate the values on</param>
        /// <param name="info">The ED Bus Bar object</param>
        public void getValues(System.Data.DataRow row, Miner.Geodatabase.Integration.FeatureInfo info, ControlTable controlTable)
        {
            row["LENGTH"] = 10;
        }
    }
}
