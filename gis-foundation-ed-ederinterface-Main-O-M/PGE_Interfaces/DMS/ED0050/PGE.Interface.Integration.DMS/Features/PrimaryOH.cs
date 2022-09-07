using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Miner.Geodatabase.Integration;
using PGE.Interface.Integration.DMS.Common;
using PGE.Common.Delivery.Framework;
using System.Diagnostics;
using PGE.Interface.Integration.DMS.Manager;

namespace PGE.Interface.Integration.DMS.Features
{
    public class PrimaryOH : IFeatureValues
    {
        public void getValues(System.Data.DataRow row, Miner.Geodatabase.Integration.FeatureInfo info, ControlTable controlTable)
        {
            //set conductor info values
            ObjectInfo conductorinfo = Utilities.getRelatedObject(info, FCID.Value[FCID.OHInfo]);
            if (conductorinfo != null)
            {
//#if DEBUG
//                Debugger.Launch();
//#endif
                //string phaseDesignation = Utilities.GetDBFieldValue(conductorinfo, "PHASEDESIGNATION", "Phase Designation - Conductor Info").ToString();
                row["MATERIAL"] = Utilities.GetDBFieldValue(conductorinfo, "MATERIAL", "Conductor Material - OH");              
                row["CONDUCTOR_SIZE"] = Utilities.GetDBFieldValue(conductorinfo, "CONDUCTORSIZE", "Conductor Size");
                row["CONDUCTOR_TYPE"] = Utilities.GetDBFieldValue(conductorinfo, "CONDUCTORTYPE", "Conductor Type - OH");
                row["PGE_CONDUCTOR_CODE"] = Utilities.GetDBFieldValue(conductorinfo, "PGE_CONDUCTORCODE", "Conductor Code - OH");
                row["INSULATION"] = Utilities.GetDBFieldValue(conductorinfo, "INSULATION", "Conductor Insulation - OH");
                row["CONDUCTOR_USE"] = Utilities.GetDBFieldValue(conductorinfo, "CONDUCTORUSE", "ConductorUse - PriOH");
            }
        }
    }
}
