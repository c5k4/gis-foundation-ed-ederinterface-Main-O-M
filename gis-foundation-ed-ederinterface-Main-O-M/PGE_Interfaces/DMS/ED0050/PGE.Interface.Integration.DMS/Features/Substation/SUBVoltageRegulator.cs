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
    /// Get field values specific to a Substation Voltage Regulator that require custom logic
    /// </summary>
    public class SUBVoltageRegulator : IFeatureValues
    {
        /// <summary>
        /// Gets values from the first related unit
        /// </summary>
        /// <param name="row">The DEVICE row to populate the values on</param>
        /// <param name="info">The Substation Voltage Regulator object</param>
        public void getValues(System.Data.DataRow row, Miner.Geodatabase.Integration.FeatureInfo info, ControlTable controlTable)
        {
            //get the first related unit
            ObjectInfo unit = Utilities.getRelatedObject(info, FCID.Value[FCID.SUBVoltageRegulatorUnit]);
            if (unit != null)
            {
                row["MAXINTERRUPTING_CURRENT"] = Utilities.GetDBFieldValue(unit, "INTERRUPTINGCURRENTRATING");
                row["SWITCH_TYPE"] = Utilities.GetDBFieldValue(unit, "SWITCHTYPE");
                row["BOOST_PERCENT"] = Utilities.GetDBFieldValue(unit, "BOOSTPERCENT", "SUB Boost/Buck Percent");
                row["BUCK_PERCENT"] = Utilities.GetDBFieldValue(unit, "BUCKPERCENT", "SUB Boost/Buck Percent");
            }
        }
    }
}
