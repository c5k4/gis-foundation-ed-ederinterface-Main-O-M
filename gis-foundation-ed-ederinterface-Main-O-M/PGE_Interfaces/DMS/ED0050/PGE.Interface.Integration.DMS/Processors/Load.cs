using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using Miner.Geodatabase.Integration;
using PGE.Interface.Integration.DMS.Features;
using ESRI.ArcGIS.Geometry;
using PGE.Interface.Integration.DMS.Common;
using Miner.Geodatabase.Integration.Electric;
using PGE.Interface.Integration.DMS.Manager;

namespace PGE.Interface.Integration.DMS.Processors
{
    public class Load : BaseProcessor
    {

        public Load(DataSet data)
            : base(data, "DMSSTAGING.LOAD")
        {

        }
        public void AddJunction(JunctionFeatureInfo junct, ControlTable controlTable)
        {
            DataRow row = _table.NewRow();
            ElectricJunction ejunct = (ElectricJunction)junct.Junction;
            double id = Utilities.getID(junct);
            string uid = id.ToString();
            //row["ABB_INT_ID"] = id;
            row["LOFPOS"] = uid;
            row["NO_KEY"] = uid;
            row["STATE"] = Utilities.GetState(junct);
            //row["PHASE"] = Utilities.getOpPhaseValue(junct);
            //row["STATUS"] = Utilities.getPhaseValue(junct);
            //row["DESCRIPTIVE_LOC"] = Utilities.GetDBFieldValue(junct, "LOCDESC");
            row["LONAME"] = Utilities.GetDBFieldValue(junct, "OPERATINGNUMBER");
            object cgc = junct.Fields["CGC12"].FieldValue;
            if (cgc != null && !(cgc is DBNull))
            {
                row["LOID"] = cgc.ToString();
            }
            else
            {
                row["LOID"] = junct.UniqueID;
            }
            //get latitude and longitude
            IPoint point = Utilities.ProjectPoint(junct.Junction.Point.X, junct.Junction.Point.Y);
            row["LATITUDE"] = point.Y;
            row["LONGITUDE"] = point.X;
            //check for a device group
            GetDeviceGroupValues(junct, row);
            if (FeatureValues.FCID.ContainsKey(junct.ObjectClassID))
            {
                FeatureValues.FCID[junct.ObjectClassID].getValues(row, junct, controlTable);
            }
            GetElectricJunctionValues(ejunct, row);
            GetMappedValues(junct, row);
            _table.Rows.Add(row);
        }

        
    }
}
