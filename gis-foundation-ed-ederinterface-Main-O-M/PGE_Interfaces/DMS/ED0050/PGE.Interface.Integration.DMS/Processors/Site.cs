using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using Miner.Geodatabase.Integration;
using PGE.Interface.Integration.DMS.Common;
using PGE.Common.Delivery.Framework.Exceptions;

namespace PGE.Interface.Integration.DMS.Processors
{
    public class Site : BaseProcessor
    {
        private Dictionary<string, DataRow> _sites;
        private Dictionary<string, SiteCoordinates> _coord;
        private Dictionary<string, string> _replacedNodes;

        public Dictionary<string, string> ReplacedNodes
        {
            get { return _replacedNodes; }
        }

        public Site(DataSet data)
            : base(data, "DMSSTAGING.SITE")
        {
            _sites = new Dictionary<string, DataRow>();
            _coord = new Dictionary<string, SiteCoordinates>();
            _replacedNodes = new Dictionary<string, string>();
        }
        public void AddJunction(JunctionFeatureInfo junct)
        {
            AddObject(junct);
        }
        public void AddEdge(EdgeFeatureInfo edge)
        {
            AddObject(edge);
        }
        public void AddObject(ObjectInfo info)
        {
            if (_table != null)
            {
                ObjectInfo devgroup = Utilities.getRelatedObject(info, FCID.Value[FCID.DeviceGroup]);
                if (devgroup != null)
                {
                    string devgroupid = devgroup.Fields["GLOBALID"].FieldValue.ToString();
                    DataRow row = null;
                    SiteCoordinates coord = null;
                    if (_sites.ContainsKey(devgroupid))
                    {
                        row = _sites[devgroupid];
                        coord = _coord[devgroupid];
                    }
                    else
                    {
                        row = _table.NewRow();
                        row["SIID"] = devgroupid;
                        row["SINAME"] = Utilities.GetDBFieldValue(devgroup, "DEVICEGROUPNAME");
                        double devid = Utilities.getID(devgroup);
                        row["SIFPOS"] = devid;
                        row["NO_KEY"] = devid;
                        row["GSITE_ANG"] = 0;
                        _table.Rows.Add(row);
                        _sites.Add(devgroupid, row);
                        coord = new SiteCoordinates(devgroup);
                        _coord.Add(devgroupid, coord);

                        //check if this site is static
                        int subtype = Convert.ToInt32(devgroup.Fields["SUBTYPECD"].FieldValue);
                        int devtype = 0;
                        object devtypeValue = Utilities.GetFieldValue(devgroup, "DEVICEGROUPTYPE");
                        if (devtypeValue != null)
                        {
                            devtype = Convert.ToInt32(devtypeValue);
                        }
                        else
                        {
                            throw new InvalidDataException("DeviceGroup " + devgroupid + " has a null DEVICEGROUPTYPE.");
                        }
                        SiteSize size = StaticSites.Instance.GetSize(subtype, devtype);
                        if (size != null)
                        {
                            coord.AddStaticSize(size);
                            //if it is static we will use the rotation angle
                            object angle = Utilities.GetDBFieldValue(devgroup, "SYMBOLROTATION");
                            if (!(angle is DBNull))
                            {
                                int iangle = Convert.ToInt32(angle);
                                angle = 360 - iangle;
                            }
                            row["GSITE_ANG"] = angle;
                        }
                    }
                    if (info is JunctionFeatureInfo)
                    {
                        JunctionFeatureInfo junct = (JunctionFeatureInfo)info;
                        coord.addJunction(junct);
                        //check if it is coincident, if it is change the node this site points to to the device
                        if (coord.IsCoincident(junct.Junction.Point))
                        {
                            double jID = Utilities.getID(junct);
                            row["NO_KEY"] = jID;
                            ReplaceNode(row, jID.ToString());
                        }

                    }
                    else
                    {
                        coord.AddEdge((EdgeFeatureInfo)info);
                    }
                    row["GXSITE"] = coord.X;
                    row["GYSITE"] = coord.Y;
                    row["GSITEH"] = coord.Height;
                    row["GSITEW"] = coord.Width;
                }
            }
        }
        public void CheckCoincidence(Dictionary<CoincidentNode, CoincidentNode> junctions)
        {
            //check each site to see if it is coincident
            foreach (string deviceGUID in _sites.Keys)
            {
                SiteCoordinates coord = _coord[deviceGUID];
                if (junctions.ContainsKey(coord.CoinNode))
                {
                    //we need to switch the node the site points to
                    double jID = junctions[coord.CoinNode].ID;
                    DataRow deviceGroup = _sites[deviceGUID];
                    deviceGroup["NO_KEY"] = jID;
                    ReplaceNode(deviceGroup, jID.ToString());
                }
            }
        }
        private void ReplaceNode(DataRow deviceGroup, string nodeID)
        {
            string devID = deviceGroup["SIFPOS"].ToString();
            if (!_replacedNodes.ContainsKey(nodeID))
            {
                _replacedNodes.Add(nodeID, devID);
            }
        }


    }
}
