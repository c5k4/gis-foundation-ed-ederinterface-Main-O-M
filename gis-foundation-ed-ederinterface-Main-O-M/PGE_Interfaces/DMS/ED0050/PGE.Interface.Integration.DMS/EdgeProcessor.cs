using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using Miner.Geodatabase.Integration;
using PGE.Interface.Integration.DMS.Processors;
using PGE.Common.Delivery.Diagnostics;
using PGE.Interface.Integration.DMS.Manager;

namespace PGE.Interface.Integration.DMS
{
    public class EdgeProcessor
    {
        private static Log4NetLogger _log4 = new Log4NetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, "ED50.log4net.config");

        private DataSet _data;
        private Path _path;
        private Line _line;
        private Site _site;

        public Site Sites
        {
            set { _site = value; }
        }

        public EdgeProcessor(DataSet data)
        {
            _data = data;
            _path = new Path(_data);
            _line = new Line(_data);
        }
        public bool shouldIncludeJunction(JunctionInfo junction)
        {
            return _line.shouldIncludeJunction(junction);
        }
        public void AddGenericNode(JunctionInfo fromJunction, GeoPoint geoPoint)
        {
            AddGenericNode(fromJunction, geoPoint);
        }
        public void AddEdge(EdgeFeatureInfo edge, ControlTable controlTable)
        {
            try
            {
                short state = Utilities.GetState(edge);
                bool isIdleEdge = Utilities.IsIdle(edge);
                //It has been requested that idle facilities get processed in the path table as well.
                if (state != 0 || (state == 0 && isIdleEdge))
                {
                    if (isIdleEdge)
                    {
                        string warning = edge.TableName + "(" + edge.ObjectClassID + ") " + edge.ObjectID + ": This feature is idle";
                        _log4.Warn(warning);
                    }
                    if (!NAStandalone.Substation)
                    {
                        if (_site != null)
                        {
                            _site.AddEdge(edge);
                        }
                    }
                    _path.AddEdge(edge);
                }
                else
                {
                    string warning = "Removing " + edge.TableName + "(" + edge.ObjectClassID + ") " + edge.ObjectID + ": Due to current status this will only be added to line table";
                    _log4.Warn(warning);
                }
                _line.AddEdge(edge, controlTable);
            }
            catch (Exception ex)
            {
                //No need to add an error for items with the same key errors.  This means the junction has already been added to our results for
                //output to DMS.  Commented out for now.
                //if (!ex.Message.Contains("An item with the same key has already been added"))
                //{
                string error = edge.TableName + "(" + edge.ObjectClassID + ") " + edge.ObjectID + ": Failed to add line: " + ex.Message;
                _log4.Error(error);
                //}
            }
        }
    }
}
