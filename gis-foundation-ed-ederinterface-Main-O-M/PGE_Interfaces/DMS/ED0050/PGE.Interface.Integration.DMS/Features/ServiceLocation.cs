using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Miner.Geodatabase.Integration;
using Miner.Geodatabase.Integration.Electric;
using PGE.Interface.Integration.DMS.Manager;

namespace PGE.Interface.Integration.DMS.Features
{

    /// <summary>
    ///  /*Changes for ENOS to SAP migration - DMS ..Adding class for Service location features..*/
    /// </summary>
    public class ServiceLocation : IFeatureValues
    {
        public void getValues(System.Data.DataRow row, Miner.Geodatabase.Integration.FeatureInfo info, ControlTable controlTable)
        {
            //row["SOID"] = Utilities.getID(info);
            //row["SG_KEY"] = 7; //fix
            //row["STATUS"] = 111;
            row["STYPE"] = 2;

        }
    }
}
