using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Miner.Geodatabase.Integration;
using PGE.Interface.Integration.DMS.Common;
using PGE.Interface.Integration.DMS.Manager;

namespace PGE.Interface.Integration.DMS.Features
{
    /// <summary>
    /// Get field values specific to a Substation underground conductor that require custom logic
    /// </summary>
    public class SubUG : IFeatureValues
    {
        /// <summary>
        /// Get values from the related conductor info record
        /// </summary>
        /// <param name="row">The LINE row to populate the values on</param>
        /// <param name="info">The Substation underground conductor object</param>
        public void getValues(System.Data.DataRow row, Miner.Geodatabase.Integration.FeatureInfo info, ControlTable controlTable)
        {
            //set conductor info values
            ObjectInfo conductorinfo = Utilities.getRelatedObject(info, FCID.Value[FCID.SUBUGInfo]);
            if (conductorinfo != null)
            {
                row["MATERIAL"] = Utilities.GetDBFieldValue(conductorinfo, "MATERIAL","SUB UG Conductor Material");
                row["CONDUCTOR_SIZE"] = Utilities.GetDBFieldValue(conductorinfo, "CABLESIZE", "SUB UG Cable Size");
                row["PGE_CONDUCTOR_CODE"] = Utilities.GetDBFieldValue(conductorinfo, "PGECONDUCTORCODE", "SUB Conductor Code - UG");
                row["INSULATION"] = Utilities.GetDBFieldValue(conductorinfo, "CABLEINSULATION");
            }
        }
    }
}
