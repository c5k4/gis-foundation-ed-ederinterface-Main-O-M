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
    /// Get attributes specific to a voltage regulator
    /// </summary>
    public class VoltageRegulator : IFeatureValues
    {
        /// <summary>
        /// Get attributes specific to a voltage regulator
        /// </summary>
        /// <param name="row">The row to populate the values on</param>
        /// <param name="info">The Voltage Regulator junction</param>
        public void getValues(System.Data.DataRow row, Miner.Geodatabase.Integration.FeatureInfo info, ControlTable controlTable)
        {
            ObjectInfo unit = Utilities.getRelatedObject(info, FCID.Value[FCID.VoltageRegulatorUnit]);
            if (unit != null)
            {
                string controller = Utilities.GetControllerType(unit, controlTable);
                if (controller != null)
                {
                    row["CONTROL_TYPE"] = controller;
                }
            }

            ObjectInfo scada = Utilities.getRelatedObject(info, FCID.Value[FCID.SCADA]);
            if (scada != null)
            {
                row["FLISR_IDC"] = Utilities.GetDBFieldValue(scada, "FLISRAUTOMATIONDEVICEIDC");
            }

        }
    }
}
