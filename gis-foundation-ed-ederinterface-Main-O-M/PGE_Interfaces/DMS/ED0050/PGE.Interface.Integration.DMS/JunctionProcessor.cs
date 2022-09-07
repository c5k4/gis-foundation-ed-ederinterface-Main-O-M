using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using Miner.Geodatabase.Integration;
using PGE.Interface.Integration.DMS.Processors;
using PGE.Interface.Integration.DMS.Common;
using Miner.Geodatabase.Integration.Electric;
using PGE.Interface.Integration.DMS.Tracers;
using PGE.Common.Delivery.Framework.Exceptions;
using PGE.Common.Delivery.Diagnostics;
using PGE.Interface.Integration.DMS.Manager;

namespace PGE.Interface.Integration.DMS
{
    public class JunctionProcessor
    {
        private static Log4NetLogger _log4 = new Log4NetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, "ED50.log4net.config");

        private DataSet _data;
        private Node _node;

        private Source _source;
        private int _isource, _isubsource;
        private int _igen;
        /*Changes for ENOS to SAP migration - DMS ..Adding variable for _iServloc */
        private int _iserviceloc;

        private Site _site;
        private int _itrans, _isubtrans;
        private int _imeter;
        private Load _load;
        private int _icap, _isubcap;
        private Capacitor _cap;
        private Device _dev;
        private FeederInfo _curFeederInfo;
        private List<BaseProcessor> _procs; //used to give each processor a reference to the current feeder
        private Dictionary<int, Dictionary<int, int[]>> _scoordinates;
        private Dictionary<int, bool> _junctions;
        private Dictionary<CoincidentNode, CoincidentNode> _coincident;
        private Dictionary<CoincidentNode, CoincidentNode> _coincidentAllJuncts;
        private NADownstreamTrace _trace;

        public NADownstreamTrace Trace
        {
            get { return _trace; }
        }

        public Site Sites
        {
            set { _site = value; }
        }

        public Dictionary<int, Dictionary<int, int[]>> Scoordinates
        {
            get { return _scoordinates; }
            set
            {
                _scoordinates = value;
                _node.Scoordinates = _scoordinates;
            }
        }

        public FeederInfo CurFeederInfo
        {
            get { return _curFeederInfo; }
            set
            {
                _curFeederInfo = value;
                foreach (BaseProcessor bp in _procs)
                {
                    bp.CurFeederInfo = _curFeederInfo;
                }
            }
        }

        public JunctionProcessor(DataSet data)
        {
            
            _data = data;
            _node = new Node(_data);

            _junctions = new Dictionary<int, bool>();
            _junctions.Add(FCID.Value[FCID.Junction], true);
            _junctions.Add(FCID.Value[FCID.SUBJunction], true);
            _junctions.Add(FCID.Value[FCID.Tie], true);
            _junctions.Add(FCID.Value[FCID.SUBTie], true);
            _junctions.Add(FCID.Value[FCID.PrimaryRiser], true);
            _junctions.Add(FCID.Value[FCID.SUBRiser], true);
            _junctions.Add(FCID.Value[FCID.SUBCurrentTransformer], true);

            _source = new Source(_data);
            _isource = FCID.Value[FCID.StitchPoint];
            _isubsource = FCID.Value[FCID.SUBStitchPoint];
            /*Changes for ENOS to SAP migration - DMS ..set value for _iServloc */
            //_igen = FCID.Value[FCID.PrimaryGen];
            _iserviceloc = FCID.Value[FCID.ServiceLocation];
            //_site = new Site(_data);
            _itrans = FCID.Value[FCID.Transformer];
            _isubtrans = FCID.Value[FCID.SUBStationTransformer];
            _imeter = FCID.Value[FCID.PrimaryMeter];
            _load = new Load(_data);
            _icap = FCID.Value[FCID.Capacitor];
            _isubcap = FCID.Value[FCID.SUBCapacitor];
            _cap = new Capacitor(_data);
            _dev = new Device(_data);

            _procs = new List<BaseProcessor>();
            _procs.Add(_node);
            _coincident = new Dictionary<CoincidentNode, CoincidentNode>();
            _coincidentAllJuncts = new Dictionary<CoincidentNode, CoincidentNode>();
            _trace = new NADownstreamTrace();
            _trace.IgnoredEdgeClass.Add(FCID.Value[FCID.DCConductor], true);
            _trace.IgnoredEdgeClass.Add(FCID.Value[FCID.SecondaryOH], true);
            _trace.IgnoredEdgeClass.Add(FCID.Value[FCID.SecondaryUG], true);
            _trace.IgnoredEdgeClass.Add(FCID.Value[FCID.TransformerLead], true);
        }
        public void AddJunction(JunctionFeatureInfo junct, ControlTable controlTable)
        {
            try
            {
                int fcid = junct.ObjectClassID;

                try
                {
                    CoincidentNode cnode = new CoincidentNode(junct);
                    _coincidentAllJuncts.Add(cnode, cnode);
                }
                catch (Exception e) 
                {
                    if (FCID.Junction == fcid || FCID.SUBJunction == fcid)
                    {
                        //No need to worry about a default junction that is conincident with another
                        //feature. We already added the actual feature rather than this default junction.
                        return;
                    }
                }

                // all junctions are nodes
                _node.AddJunction(junct);

                if (_junctions.ContainsKey(fcid))
                {
                    //need to keep track of junctions to see if they are coincident with device groups
                    CoincidentNode cnode = new CoincidentNode(junct);
                    _coincident.Add(cnode, cnode);//let this fail, two junctions should never be coincident
                    return; // do nothing for network junctions since they have no attributes
                }
                
                //If this junction is deactivated then it doesn't need to be added to any table other than node.
                //It has also been requested that idle facilities to be processed just like the rest of the nodes
                bool isIdle = Utilities.IsIdle(junct);
                if (!Utilities.IsDeactivated(junct) || (Utilities.IsDeactivated(junct) && isIdle))
                {
                    if (isIdle)
                    {
                        string warning = junct.TableName + "(" + junct.ObjectClassID + ") " + junct.ObjectID + ": This feature is idle: ";
                        _log4.Warn(warning);
                    }
                    //most ED junctions could participate in a site
                    if (!NAStandalone.Substation)
                    {
                        if (_site != null)
                        {
                            _site.AddJunction(junct);
                        }
                    }
                    //determine what table the device goes in
                    if (fcid == _itrans || fcid == _imeter || fcid == _isubtrans)//load feature classes
                    {
                        _load.AddJunction(junct, controlTable);
                        if (fcid == _imeter)//if it is a primary meter find all the downstream devices
                        {
                            _trace.Trace(junct);
                            if (_trace.Loops)
                            {
                                throw new InvalidDataException("Circuit has loops.");
                            }
                        }
                    }
                    else if (fcid == _icap || fcid == _isubcap)//capacitor feature classes
                    {
                        _cap.AddJunction(junct, controlTable);
                    }
                    /*Changes for ENOS to SAP migration - DMS ..Changed below condition to include _iServloc */
                    //else if (fcid == _isource || fcid == _igen || fcid == _isubsource) //source feature classes
                    else if (fcid == _isource || fcid == _igen || fcid == _isubsource || fcid == _iserviceloc)
                    {
                        _source.AddJunction(junct, controlTable);
                    }
                    else //everything else is a device
                    {
                        _dev.AddJunction(junct, controlTable);
                    }
                }
                else
                {
                    //Warn that this junction is Idle, so it won't be added any device tables
                    string warning = junct.TableName + "(" + junct.ObjectClassID + ") " + junct.ObjectID + ": Due to current status this will only be added to node table";
                    _log4.Warn(warning);
                }
            }
            catch (Exception ex)
            {
                //No need to add an error for items with the same key errors.  This means the junction has already been added to our results for
                //output to DMS.  Commented out for now
                //if (!ex.Message.Contains("An item with the same key has already been added"))
                //{
                string error = junct.TableName + "(" + junct.ObjectClassID + ") " + junct.ObjectID + ": Failed to add junction: " + ex.Message;
                    _log4.Error(error);
                //}
            }
        }
        public void CheckCoincidence()
        {
            _site.CheckCoincidence(_coincident);
        }
        public void RemapSites()
        {
            _node.RemapNodes(_site.ReplacedNodes);
        }
    }
}
