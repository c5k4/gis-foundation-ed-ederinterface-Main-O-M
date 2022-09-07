using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Miner.Geodatabase.Integration;
using System.Data;
using Miner.Geodatabase.Integration.Electric;
using PGE.Interface.Integration.DMS.Common;
using PGE.Interface.Integration.DMS.Manager;

namespace PGE.Interface.Integration.DMS.Features
{
    /// <summary>
    /// Get field values specific to a Transformer that require custom logic
    /// </summary>
    public class Transformer : IFeatureValues
    {
        /// <summary>
        /// Get field values specific to a Transformer. Pulls values from a Transformer's units
        /// </summary>
        /// <param name="row">The LOAD row to populate the values on</param>
        /// <param name="info">The Transformer object</param>
        public void getValues(System.Data.DataRow row, Miner.Geodatabase.Integration.FeatureInfo info, ControlTable controlTable)
        {
            //row["SG_KEY"] = 26;
            PopulateKVAR(info, row);
        }
        /// <summary>
        /// Populate the RATING fields based on the RATEDKVA of the Transformer and its units. Also grabs other fields
        /// from the last unit processed that is not null
        /// </summary>
        /// <param name="junct">The Transformer object</param>
        /// <param name="row">The LOAD row to populate the values on</param>
        private void PopulateKVAR(FeatureInfo junct, DataRow row)
        {
            object total = Utilities.GetFieldValue(junct, "RATEDKVA");
            if (total != null)
            {
                row["RATING_1"] = total;
            }
            List<ObjectInfo> units = Utilities.getRelatedObjects(junct, FCID.Value[FCID.TransformerUnit]);
            foreach (ObjectInfo unit in units)
            {
                object manufacturer = Utilities.GetFieldValue(unit, "MANUFACTURER");
                if (manufacturer != null)
                {
                    row["MANUFACTURER"] = manufacturer;
                    row["SERIAL_NO"] = Utilities.GetDBFieldValue(unit, "SERIALNUMBER");
                    row["MANUFACTURED_DATE"] = Utilities.GetDBFieldValue(unit, "YEARMANUFACTURED");
                }
                object ophase = Utilities.GetFieldValue(unit, "PHASEDESIGNATION");
                if (ophase != null)
                {
                    object unitkva = Utilities.GetFieldValue(unit, "RATEDKVA");
                    if (unitkva != null)
                    {
                        ElectricJunction ejunct = (ElectricJunction)((JunctionFeatureInfo)junct).Junction;
                        int phase = (int)ophase;
                        if ((phase & 4) == 4)
                        {
                            row["RATING_2"] = unitkva;
                        }
                        if ((phase & 2) == 2)
                        {
                            row["RATING_3"] = unitkva;
                        }
                        if ((phase & 1) == 1)
                        {
                            row["RATING_4"] = unitkva;
                        }
                    }
                }
            }
        }
    }
}
