using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Miner.Geodatabase.Integration;
using Miner.Geodatabase.Integration.Electric;
using PGE.Interface.Integration.DMS.Common;
using PGE.Interface.Integration.DMS.Manager;

namespace PGE.Interface.Integration.DMS.Features
{
    public class StitchPoint : IFeatureValues
    {
        public void getValues(System.Data.DataRow row, Miner.Geodatabase.Integration.FeatureInfo info, ControlTable controlTable)
        {
            ElectricJunction ej = (ElectricJunction)((JunctionFeatureInfo)info).Junction;
            //row["SOID"] = ej.Feeder.FeederID;
            row["SONAME"] = ej.CircuitName;
            //row["SG_KEY"] = 36;
            //row["STATUS"] = 111;
            row["STYPE"] = 0;
            //get related circuit source
            ObjectInfo circuitsource = Utilities.getRelatedObject(info, FCID.Value[FCID.SUBCircuitSource]);
            if (circuitsource != null)
            {
                row["NOMINAL_VOLTAGE"] = Utilities.GetDBFieldValue(circuitsource, "NOMINALVOLTAGE", "Primary Voltage");
            }
        }
    }
}
