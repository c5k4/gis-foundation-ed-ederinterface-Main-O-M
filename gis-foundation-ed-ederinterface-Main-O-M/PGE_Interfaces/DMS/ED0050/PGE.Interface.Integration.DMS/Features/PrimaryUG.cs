using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Miner.Geodatabase.Integration;
using PGE.Interface.Integration.DMS.Common;
using PGE.Common.Delivery.Framework;
using PGE.Interface.Integration.DMS.Manager;

namespace PGE.Interface.Integration.DMS.Features
{
    public class PrimaryUG : IFeatureValues
    {
        public void getValues(System.Data.DataRow row, Miner.Geodatabase.Integration.FeatureInfo info, ControlTable controlTable)
        {
            //set conductor info values
            ObjectInfo conductorinfo = Utilities.getRelatedObject(info, FCID.Value[FCID.UGInfo]);
            if (conductorinfo != null)
            {
                row["MATERIAL"] = Utilities.GetDBFieldValue(conductorinfo, "MATERIAL", "Conductor Material - UG");
                row["CONDUCTOR_SIZE"] = Utilities.GetDBFieldValue(conductorinfo, "CONDUCTORSIZE", "Conductor Size");
                row["PGE_CONDUCTOR_CODE"] = Utilities.GetDBFieldValue(conductorinfo, "PGE_CONDUCTORCODE", "Conductor Code - UG");
                row["INSULATION"] = Utilities.GetDBFieldValue(conductorinfo, "INSULATION", "Conductor Insulation - UG");
                row["CONDUCTOR_USE"] = Utilities.GetDBFieldValue(conductorinfo, "CONDUCTORUSE", "ConductorUse - PriUG");
            }
        }
    }
}
