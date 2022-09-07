using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PGE.Interface.Integration.DMS.Manager;

namespace PGE.Interface.Integration.DMS.Features
{
    /// <summary>
    /// Get field values specific to a Substation Bus Bar that require custom logic
    /// </summary>
    public class SubBusBar : IFeatureValues
    {
        /// <summary>
        /// Currently does not do anything
        /// </summary>
        /// <param name="row">The LINE row to populate the values on</param>
        /// <param name="info">The Substation Bus Bar object</param>
        public void getValues(System.Data.DataRow row, Miner.Geodatabase.Integration.FeatureInfo info, ControlTable controlTable)
        {
            
        }
    }
}
