using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Miner.Geodatabase.Integration.Electric;
using PGE.Interface.Integration.DMS.Common;
using Miner.Geodatabase.Integration;
using PGE.Interface.Integration.DMS.Manager;

namespace PGE.Interface.Integration.DMS.Features
{
    public class SubStitchPoint : IFeatureValues
    {
        public void getValues(System.Data.DataRow row, Miner.Geodatabase.Integration.FeatureInfo info, ControlTable controlTable)
        {
            //row["SOID"] = ej.Feeder.FeederID;
            row["NOMINAL_VOLTAGE"] = Utilities.GetDBFieldValue(info, "NOMINALVOLTAGE"); //rbae - TFS 9783

            //set conductor info values
            ObjectInfo circuitsource = Utilities.getRelatedObject(info, FCID.Value[FCID.SUBCircuitSource]);
            if (circuitsource != null)
            {
                row["FDR_SUB_ID"] = Utilities.GetDBFieldValue(circuitsource, "SUBSTATIONID");
                row["POSSEQR"] = Utilities.GetDBFieldValue(circuitsource, "MAXPOSITIVESEQUENCERESISTANCE");
                row["POSSEQX"] = Utilities.GetDBFieldValue(circuitsource, "MAXPOSITIVESEQUENCEREACTANCE");
                row["ZEROSEQR"] = Utilities.GetDBFieldValue(circuitsource, "MAXZEROSEQUENCERESISTANCE");
                row["ZEROSEQX"] = Utilities.GetDBFieldValue(circuitsource, "MAXZEROSEQUENCEREACTANCE");
                row["NEGSEQX"] = Utilities.GetDBFieldValue(circuitsource, "MAXPOSITIVESEQUENCEREACTANCE");
                //row["NOMINAL_VOLTAGE"] = Utilities.GetDBFieldValue(circuitsource, "NOMINALVOLTAGE", "Primary Voltage");
            }
            ObjectInfo EDStitchPoint = Utilities.getRelatedObject(info, FCID.Value[FCID.StitchPoint]);
            if (EDStitchPoint != null)
            {

            }
        }
    }
}
