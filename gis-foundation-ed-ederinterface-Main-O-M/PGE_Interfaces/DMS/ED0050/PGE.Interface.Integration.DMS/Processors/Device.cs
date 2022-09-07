using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using Miner.Geodatabase.Integration;
using Miner.Geodatabase.Integration.Electric;
using ESRI.ArcGIS.Geometry;
using PGE.Interface.Integration.DMS.Common;
using PGE.Common.Delivery.Framework;
using PGE.Interface.Integration.DMS.Features;
using PGE.Interface.Integration.DMS.Manager;

namespace PGE.Interface.Integration.DMS.Processors
{
    /// <summary>
    /// Processor for junctions that go into the DEVICE staging table
    /// </summary>
    public class Device : BaseProcessor
    {
        /// <summary>
        /// Initialize the processor
        /// </summary>
        /// <param name="data">The staging schema DataSet with the DEVIE table</param>
        public Device(DataSet data)
            : base(data, "DMSSTAGING.DEVICE")
        {

        }
        /// <summary>
        /// Add a DEVICE row for the junction
        /// </summary>
        /// <param name="junct">The junction to pull the values from in order to populate a new row</param>
        public void AddJunction(JunctionFeatureInfo junct, ControlTable controlTable)
        {
            DataRow row = _table.NewRow();
            ElectricJunction ejunct = (ElectricJunction)junct.Junction;
            string circuit = ejunct.Feeder.FeederID;
            //row["DID"] = circuit + "-" + junct.UniqueID;
            double id = Utilities.getID(junct);
            string uid = id.ToString();
            //row["ABB_INT_ID"] = id;
            row["DFPOS"] = uid;    
            row["LI_KEY"] = Utilities.GetUpstreamLineID(junct);
            row["LINE_END"] = 2;
            row["STATE"] = Utilities.GetState(junct);
            //row["STATUS"] = GetPhase(ejunct);

            //EdgeInfo upEdge = Utilities.GetUpstreamLine(junct);
            //row["LI_KEY"] = Utilities.getID(upEdge);
            //GeoPoint p = Utilities.GetNextUpstreamVertex(upEdge);
            //IPoint p2 = Utilities.MovePoint(junct.Junction.Point.X, junct.Junction.Point.Y, p.X, p.Y);
            //row["XD_1"] = Utilities.ConvertXY(p2.X);
            //row["YD_1"] = Utilities.ConvertXY(p2.Y);
            
            row["XD_1"] = Utilities.ConvertXY(junct.Junction.Point.X);
            row["YD_1"] = Utilities.ConvertXY(junct.Junction.Point.Y);

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
        /// <summary>
        /// Get the phase of the junction. In DMS an open device does not have any phase.
        /// </summary>
        /// <param name="junct">The junction</param>
        /// <returns>The phase or 0 if the junction is normally open</returns>
        public static int GetPhase(ElectricJunction junct)
        {
            if (!Utilities.IsNormallyOpen(junct))
            {
                return Utilities.getPhaseValue(junct);
            }
            return 0;
        }
        

    }
}
