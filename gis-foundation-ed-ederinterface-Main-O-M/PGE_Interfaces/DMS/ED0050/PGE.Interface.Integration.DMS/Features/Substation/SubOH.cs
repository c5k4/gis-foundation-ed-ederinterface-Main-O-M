using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Miner.Geodatabase.Integration;
using PGE.Interface.Integration.DMS.Common;
using PGE.Interface.Integration.DMS.Manager;

namespace PGE.Interface.Integration.DMS.Features
{
    public class SubOH : IFeatureValues
    {
        public void getValues(System.Data.DataRow row, Miner.Geodatabase.Integration.FeatureInfo info, ControlTable controlTable)
        {
            //set conductor info values
            ObjectInfo conductorinfo = Utilities.getRelatedObject(info, FCID.Value[FCID.SUBOHInfo]);
            if (conductorinfo != null)
            {
                row["MATERIAL"] = Utilities.GetDBFieldValue(conductorinfo, "MATERIAL", "SUB OH Conductor Material");
                row["CONDUCTOR_SIZE"] = Utilities.GetDBFieldValue(conductorinfo, "CONDUCTORSIZE", "SUB OH Conductor Size");
                row["PGE_CONDUCTOR_CODE"] = Utilities.GetDBFieldValue(conductorinfo, "PGECONDUCTORCODE", "SUB Conductor Code - OH");
            }
        }
    }
}
