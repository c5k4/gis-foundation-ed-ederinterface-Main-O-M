using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using Miner.Geodatabase.Integration;
using Miner.Geodatabase.Integration.Electric;
using PGE.Interface.Integration.DMS.Common;
using System.Diagnostics;

namespace PGE.Interface.Integration.DMS.Processors
{
    public class Node : BaseProcessor
    {
        private int _junction, _subjunction;
        private int _tie;
        private int _openpoint;
        private int _riser;
        private Dictionary<int, Dictionary<int, int[]>> _scoordinates;
        private Dictionary<string, bool> _devicegroups;

        public Dictionary<int, Dictionary<int, int[]>> Scoordinates
        {
            get { return _scoordinates; }
            set { _scoordinates = value; }
        }
        public Node(DataSet data)
            : base(data, "DMSSTAGING.NODE")
        {
            _junction = FCID.Value[FCID.Junction];
            _subjunction = FCID.Value[FCID.SUBJunction];
            _tie = FCID.Value[FCID.Tie];
            _openpoint = FCID.Value[FCID.OpenPoint];
            _riser = FCID.Value[FCID.PrimaryRiser];
            _devicegroups = new Dictionary<string, bool>();
        }
        public void AddJunction(JunctionFeatureInfo junct)
        {
            ElectricJunction ej = (ElectricJunction)junct.Junction;
            DataRow row = _table.NewRow();
            //row["NID"] = ej.Feeder.FeederID + "-" + junct.UniqueID;
            double id = Utilities.getID(junct);
            string uid = id.ToString();
            //row["ABB_INT_ID"] = id;
            row["NFPOS"] = uid;
            //int dist = GetNodeDisctrict(junct);
            //row["SUB_AREA"] = dist;

            if (junct.ObjectClassID != _junction && junct.ObjectClassID != _subjunction)
            {
                row["STATE"] = Utilities.GetState(junct);

                //RBAE - 11/13/13 - replace LOCAL_OFFICE code with Columns.xml
                //in 7.4.7 riser does not have local office, it may have it in the future
                //if (junct.ObjectClassID != _riser)
                //{
                //    row["LOCAL_OFFICE"] = Utilities.GetDBFieldValue(junct, "LOCALOFFICEID");
                //}
            }
            else
            {
                row["STATE"] = Utilities.GetJunctionState(junct.Junction);
            }

            //row["NBITS"] = 12288;
            //row["NANNOPOS_1"] = 3;
            //row["NANNOPOS_2"] = 3;
            
            row["XN_1"] = Utilities.ConvertXY(junct.Junction.Point.X); 
            row["YN_1"] = Utilities.ConvertXY(junct.Junction.Point.Y);
            row["XN_2"] = SchematicExtractor.GetX(_scoordinates, junct);
            row["YN_2"] = SchematicExtractor.GetY(_scoordinates, junct);
            int circuitid = 0;
            if (Int32.TryParse(ej.Feeder.FeederID, out circuitid))
            {
                row["SUB_ID"] = circuitid;
            }

            //row["SG_KEY_1"] = GetSGKey(junct);
            if (NAStandalone.Substation)//need to copy substation name if in substation
            {
                if (junct.ObjectClassID != _subjunction)
                {
                    //row["SUBSTATION_NAME"] = Utilities.GetDBFieldValue(junct, "SUBSTATIONNAME");
                    //row["FDR_SUB_ID"] = Utilities.GetDBFieldValue(junct, "SUBSTATIONID");
                }
            }

            GetElectricJunctionValues(ej, row);
            GetMappedValues(junct, row);
            _table.Rows.Add(row);

            //Only add to device group table if the junction is not deactivated.  It has also been requested that idle facilities get
            //processed like everything else.
            if (!Utilities.IsDeactivated(junct) || (Utilities.IsDeactivated(junct) && Utilities.IsIdle(junct))) { AddDeviceGroup(junct); }
        }
        private void AddDeviceGroup(JunctionFeatureInfo junct)
        {
            ObjectInfo devgroup = Utilities.getRelatedObject(junct, FCID.Value[FCID.DeviceGroup]);
            if (devgroup != null)
            {
                string id = devgroup.Fields["GLOBALID"].FieldValue.ToString();

                try
                {
                    CADOPS.junctionClassIDToGlobalIDs[devgroup.ObjectClassID].Add(id);
                }
                catch
                {
                    if (!CADOPS.junctionClassIDToGlobalIDs.ContainsKey(devgroup.ObjectClassID))
                    {
                        CADOPS.junctionClassIDToGlobalIDs.Add(devgroup.ObjectClassID, new List<string>());
                    }
                    CADOPS.junctionClassIDToGlobalIDs[devgroup.ObjectClassID].Add(id);
                }

                if (!_devicegroups.ContainsKey(id))
                {
                    _devicegroups.Add(id, true);
                    DataRow row = _table.NewRow();
                    double sid = Utilities.getID(devgroup);
                    string uid = sid.ToString();
                    row["NFPOS"] = uid;
                    row["SITE_KEY"] = uid;
                    row["LOCAL_OFFICE"] = Utilities.GetDBFieldValue(devgroup, "LOCALOFFICEID");
                    row["DISTRICT"] = Utilities.GetDBFieldValue(devgroup, "DISTRICT", "District Name");
                    row["STATE"] = Utilities.GetState(devgroup);
                    string devtype = Utilities.GetDeviceGroupType(devgroup);
                    if (devtype != null)
                    {
                        row["DEVICE_GROUP_DESC"] = devtype;
                    }
                    SimpleFeatureInfo sfi = (SimpleFeatureInfo)devgroup;
                    Point2D point = (Point2D)sfi.Shape;
                    row["XN_1"] = Utilities.ConvertXY(point.X);
                    row["YN_1"] = Utilities.ConvertXY(point.Y);
                    GetMappedValues(devgroup, row);
                    _table.Rows.Add(row);
                }
            }
        }
        private short GetSGKey(JunctionFeatureInfo junct)
        {
            if (junct.ObjectClassID == _tie)
            {
                return 3;
            }
            if (junct.ObjectClassID == _openpoint)
            {
                object subtype = Utilities.GetFieldValue(junct, "SUBTYPECD");
                if (subtype != null)
                {
                    if ((int)subtype == 9)
                    {
                        return 4;
                    }
                }
            }
            return 41;
        }

        ///RBAE - 11/13/13 - Not needed
        //private int GetNodeDisctrict(JunctionFeatureInfo junct)
        //{
        //    int dist = -1;
        //    if (junct.ObjectClassID == _junction || junct.ObjectClassID == _subjunction)//if it is a junction there is no localoffice so we have to look at the upstream line
        //    {
        //        EdgeInfo e = Utilities.GetUpstreamLine(junct);
        //        if (e != null)
        //        {
        //            if (CurFeederInfo.EdgeFeatures.ContainsKey(e.ObjectKey))
        //            {
        //                dist = Utilities.GetDistrict(CurFeederInfo.EdgeFeatures[e.ObjectKey]);
        //            }

        //        }
        //    }
        //    else
        //    {
        //        dist = Utilities.GetDistrict(junct);
        //    }
        //    return dist;
        //}


        public void RemapNodes(Dictionary<string, string> map)
        {
            //should be faster this way since NFPOS is not indexed that way there is only one pass through the nodes table instead of search for each node
            foreach (DataRow node in _table.Rows)
            {
                string nodeID = node["NFPOS"].ToString();
                if (map.ContainsKey(nodeID))
                {
                    node["SITE_KEY"] = map[nodeID];
                }
            }
        }
    }
}
