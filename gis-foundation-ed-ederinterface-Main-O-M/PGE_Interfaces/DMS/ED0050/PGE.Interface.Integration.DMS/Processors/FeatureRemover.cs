using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using Miner.Geodatabase.Integration;
using PGE.Interface.Integration.DMS.Common;
using PGE.Common.Delivery.Diagnostics;

namespace PGE.Interface.Integration.DMS.Processors
{
    /// <summary>
    /// Removes rows from the staging schema dataset using the unique IDs
    /// </summary>
    public class FeatureRemover
    {
        private static Log4NetLogger _log4 = new Log4NetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, "ED50.log4net.config");
        private DataSet _data;
        /// <summary>
        /// Initialize the FeatureRemover with a reference to the staging schema dataset
        /// </summary>
        /// <param name="data">The staging schema dataset</param>
        public FeatureRemover(DataSet data)
        {
            _data = data;

        }
        /// <summary>
        /// Remove edges from the corresponding staging tables. No error is thrown if the edge does not exist.
        /// </summary>
        /// <param name="edges">A list of edges</param>
        public void RemoveEdges(List<EdgeInfo> edges)
        {
            foreach (EdgeInfo edge in edges)
            {
                
                int fcid = edge.ObjectClassID;
                double jid = Utilities.getID(edge);
                removeRow(_data.Tables["DMSSTAGING.LINE"], "FPOS", jid);
                removeRow(_data.Tables["DMSSTAGING.PATH"], "LINE_GUID", jid);
                string debugMessage = "Removed edge " + edge.ObjectClassID + " - " + edge.ObjectID;
                _log4.Debug(debugMessage);
            }
        }
        /// <summary>
        /// Remove junctions from the corresponding staging tables. No error is thrown if the junction does not exist.
        /// </summary>
        /// <param name="junctions">The list of junctions to remove.</param>
        public void RemoveJunctions(List<JunctionInfo> junctions)
        {
            foreach (JunctionInfo junction in junctions)
            {
                int fcid = junction.ObjectClassID;
                double jid = Utilities.getID(junction);
                removeRow(_data.Tables["DMSSTAGING.NODE"], "NFPOS", jid);
                //if it is only in the node table we don't need to remove it from any other table
                if (fcid == FCID.Value[FCID.Junction] || fcid == FCID.Value[FCID.Tie] || fcid == FCID.Value[FCID.PrimaryRiser])
                {
                    continue;
                }

                /*Changes for ENOS to SAP migration - DMS  Changes .. Start */
                //if (fcid == FCID.Value[FCID.PrimaryGen] || fcid == FCID.Value[FCID.StitchPoint])
                if (fcid == FCID.Value[FCID.ServiceLocation] || fcid == FCID.Value[FCID.StitchPoint])
                {
                    removeRow(_data.Tables["DMSSTAGING.SOURCE"], "SOFPOS", jid);
                }
                /*Changes for ENOS to SAP migration - DMS  Changes .. End */
                else if (fcid == FCID.Value[FCID.Transformer] || fcid == FCID.Value[FCID.PrimaryMeter])
                {
                    removeRow(_data.Tables["DMSSTAGING.LOAD"], "LOFPOS", jid);
                }
                else if (fcid == FCID.Value[FCID.Capacitor])
                {
                    removeRow(_data.Tables["DMSSTAGING.CAPACITOR"], "CAFPOS", jid);
                }
                else
                {
                    removeRow(_data.Tables["DMSSTAGING.DEVICE"], "DFPOS", jid);
                }
                string debugMessage = "Removed junction " + junction.ObjectClassID + " - " + junction.ObjectID;
                _log4.Debug(debugMessage);

            }
        }
        /// <summary>
        /// Remove the row from the table
        /// </summary>
        /// <param name="table">The table to remove the row from</param>
        /// <param name="key">The field that contains the unique ID</param>
        /// <param name="id">The unique ID</param>
        private void removeRow(DataTable table, string key, double id)
        {
            DataRow[] rows = table.Select(key + "='" + id + "'");
            if (rows != null)
            {
                foreach (DataRow row in rows)
                {
                    table.Rows.Remove(row);
                }
            }
        }
    }
}
