using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using Miner.Geodatabase.Integration;
using Miner.Geodatabase.Integration.Electric;
using Miner.Interop;
using PGE.Interface.Integration.DMS.Features;
using PGE.Interface.Integration.DMS.Manager;

namespace PGE.Interface.Integration.DMS.Processors
{
    /// <summary>
    /// Processor for converting Capacitor junctions in rows in the CAPACITOR staging table
    /// </summary>
    public class Capacitor : BaseProcessor
    {
        /// <summary>
        /// Initialize the processor
        /// </summary>
        /// <param name="data">The staging schema dataset with the CAPACITOR table</param>
        public Capacitor(DataSet data)
            : base(data, "DMSSTAGING.CAPACITOR")
        {

        }
        /// <summary>
        /// Create a new CAPACITOR row for the Capacitor junction and populate Capacitor specific values
        /// </summary>
        /// <param name="junct">The Capacitor Junction</param>
        public void AddJunction(JunctionFeatureInfo junct, ControlTable controlTable)
        {
            DataRow row = _table.NewRow();
            ElectricJunction ejunct = (ElectricJunction)junct.Junction;
            //row["CAID"] = ejunct.Feeder.FeederID + "-" + junct.UniqueID;
            row["CANAME"] = junct.Fields["OPERATINGNUMBER"].FieldValue;
            double id = Utilities.getID(junct);
            string uid = id.ToString();
            row["CAFPOS"] = uid;
            row["NO_KEY"] = uid;
            row["STATE"] = Utilities.GetState(junct);
            //row["ABB_INT_ID"] = id;
            PopulateKVAR(junct, row);
            string controller = Utilities.GetControllerType(junct, controlTable);
            if (controller != null)
            {
                row["CONTROLLER_TYPE"] = controller;
            }
            //check for a device group
            GetDeviceGroupValues(junct, row);
            try
            {
                FeatureValues.FCID[junct.ObjectClassID].getValues(row, junct, controlTable);
            }
            catch { }
            GetElectricJunctionValues(ejunct, row);
            GetMappedValues(junct, row);
            _table.Rows.Add(row);
        }
        /// <summary>
        /// Populate the KVAR Ratings based on the phase of the capacitor
        /// </summary>
        /// <param name="junct">The Capacitor junction</param>
        /// <param name="row">The row to populate the data on</param>
        private void PopulateKVAR(JunctionFeatureInfo junct, DataRow row)
        {
            object total = Utilities.GetFieldValue(junct, "TOTALKVAR");
            if (total != null)
            {
                row["RATING_1"] = total;
            }
            object unit = Utilities.GetFieldValue(junct, "UNITKVAR");
            if (unit != null)
            {
                //check for the presence of each phase and set the corresponding Rating
                ElectricJunction ejunct = (ElectricJunction)junct.Junction;
                int phase = (int)ejunct.OperationalPhases;
                if ((phase & 1) == 1)
                {
                    row["RATING_2"] = unit;
                }
                if ((phase & 2) == 2)
                {
                    row["RATING_3"] = unit;
                }
                if ((phase & 4) == 4)
                {
                    row["RATING_4"] = unit;
                }
            }
        }
    }
}
