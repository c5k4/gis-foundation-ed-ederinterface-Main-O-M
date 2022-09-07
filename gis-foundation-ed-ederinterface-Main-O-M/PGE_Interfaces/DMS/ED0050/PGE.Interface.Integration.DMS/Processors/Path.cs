using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using Miner.Geodatabase.Integration;

namespace PGE.Interface.Integration.DMS.Processors
{
    public class Path : BaseProcessor
    {
        public static Dictionary<int, List<string>> ClassIDToUIDMap = new Dictionary<int, List<string>>();
        public static Dictionary<int, List<int>> InvalidGeometryByClassID = new Dictionary<int, List<int>>();

        public Path(DataSet data)
            : base(data, "DMSSTAGING.PATH")
        {

        }
        public void AddEdge(EdgeFeatureInfo edge)
        {
            bool isInvalidGeomForSchem = false;
            if (!CADOPS.isSubstation && edge.Edges.Count > 1)
            {
                isInvalidGeomForSchem = true;
                //Invalid geometry if there are more than one edge.  Complex edges really shouldn't be used.
                try
                {
                    InvalidGeometryByClassID[edge.ObjectClassID].Add(edge.ObjectID);
                }
                catch
                {
                    InvalidGeometryByClassID.Add(edge.ObjectClassID, new List<int>());
                    InvalidGeometryByClassID[edge.ObjectClassID].Add(edge.ObjectID);
                }
            }
            foreach (EdgeInfo e in edge.Edges)
            {                
                double id = Utilities.getID(e);
                string uid = id.ToString();

                if (!CADOPS.isSubstation && !isInvalidGeomForSchem)
                {
                    try
                    {
                        ClassIDToUIDMap[e.ObjectClassID].Add(uid);
                    }
                    catch
                    {
                        ClassIDToUIDMap.Add(e.ObjectClassID, new List<string>());
                        ClassIDToUIDMap[e.ObjectClassID].Add(uid);
                    }
                }

                //go through the vertices
                for (int i = 1; i <= e.Line.Count; i++)
                {
                    GeoPoint p = e.Line[i-1];

                    DataRow row = _table.NewRow();
                    row["LINE_GUID"] = uid;
                    row["XDIFF"] = Utilities.ConvertXY(p.X);
                    row["YDIFF"] = Utilities.ConvertXY(p.Y);
                    row["ORDER_NUM"] = i;
                    if (NAStandalone.Substation)
                    {
                        row["SUBSTATION_OBJ_IDC"] = "Y";
                    }
                    else
                    {
                        row["SUBSTATION_OBJ_IDC"] = "N";
                    }
                    row["TYPE"] = "G";
                    _table.Rows.Add(row);
                }
            }
        }
        [Obsolete]
        public void AddEdgeOLD(EdgeFeatureInfo edge)
        {

            foreach (EdgeInfo e in edge.Edges)
            {
                int subid = 0;
                double nextid = 0;
                //go through the vertices in reverse order
                for (int i = e.Line.Count - 1; i >= 0; i--)
                {
                    GeoPoint p = e.Line[i];
                    double id = Utilities.getID(e.ObjectClassID, e.ObjectID, e.SubID, subid);
                    DataRow row = _table.NewRow();
                    row["PAFPOS"] = id;
                    row["XDIFF"] = Utilities.ConvertXY(p.X);
                    row["YDIFF"] = Utilities.ConvertXY(p.Y);
                    if (subid > 0)
                    {
                        row["NEXTP"] = nextid;
                    }
                    _table.Rows.Add(row);
                    nextid = id;
                    subid++;
                }

            }

        }
    }
}
