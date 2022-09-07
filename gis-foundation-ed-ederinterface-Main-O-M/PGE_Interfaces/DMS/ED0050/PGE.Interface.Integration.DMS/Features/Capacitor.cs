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
    /// Get field values specific to a Dynamic Protective Device that require custom logic
    /// </summary>
    public class Capacitor : IFeatureValues
    {
        /// <summary>
        /// Get field values specific to a Dynamic Protective Device. Pulls values from a related controller
        /// </summary>
        /// <param name="row">The DEVICE row to populate the values on</param>
        /// <param name="info">The Dynamic Protective Device object</param>
        public void getValues(System.Data.DataRow row, Miner.Geodatabase.Integration.FeatureInfo info, ControlTable controlTable)
        {
            ObjectInfo scada = Utilities.getRelatedObject(info, FCID.Value[FCID.SCADA]);
            if (scada != null)
            {
                row["FLISR_IDC"] = Utilities.GetDBFieldValue(scada, "FLISRAUTOMATIONDEVICEIDC");
            }
        }
    }
}
