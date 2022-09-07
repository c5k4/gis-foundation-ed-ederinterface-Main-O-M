using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Miner.Geodatabase.Integration;
using PGE.Interface.Integration.DMS.Common;
using PGE.Interface.Integration.DMS.Manager;
using PGE.Common.Delivery.Diagnostics;

namespace PGE.Interface.Integration.DMS.Features
{
    /// <summary>
    /// Get field values specific to a Switch that require custom logic
    /// </summary>
    public class Switch : IFeatureValues
    {
        private static Log4NetLogger _log4 = new Log4NetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, "ED50.log4net.config");

        /// <summary>
        /// Gets the controller type if the switch has a related controller
        /// </summary>
        /// <param name="row">The DEVICE row to populate the values on</param>
        /// <param name="info">The Switch object</param>
        public void getValues(System.Data.DataRow row, Miner.Geodatabase.Integration.FeatureInfo info, ControlTable controlTable)
        {
            string controller = Utilities.GetControllerType(info, controlTable);
            if (controller != null)
            {
                row["CONTROL_TYPE"] = controller;
            }

            ObjectInfo scada = Utilities.getRelatedObject(info, FCID.Value[FCID.SCADA]);
            if (scada != null)
            {
                row["FLISR_IDC"] = Utilities.GetDBFieldValue(scada, "FLISRAUTOMATIONDEVICEIDC");
            }

            try
            {
                Dictionary<string, string> columnNameMap = new Dictionary<string, string>();
                string globalID = info.Fields["GLOBALID"].FieldValue.ToString();
                string tableName = "PGEDATA.PGEDATA_SM_SWITCH_EAD";
                columnNameMap.Add("SECTIONALIZING_FEATURE", "SECTIONALIZING_FEATURE");
                columnNameMap.Add("SWITCH_TYPE", "SWITCH_TYPE");

                foreach (KeyValuePair<string, string> kvp in columnNameMap)
                {
                    try
                    {
                        //Determine the OKToBypass field from settings
                        string value = controlTable.GetSettingsValue(globalID, tableName, kvp.Value, "SM_SWITCH");
                        row[kvp.Key] = value;
                    }
                    catch (Exception ex)
                    {
                        _log4.Error("Could not determine " + kvp.Value + " value from PGEData schema: " + ex.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                _log4.Error("Unable to get values from PGEData schema: " + ex.Message);
            }

        }
    }
}
