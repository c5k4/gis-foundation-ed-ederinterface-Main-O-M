using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using Miner.Geodatabase.Integration;
using PGE.Interface.Integration.DMS.Features;
using Miner.Geodatabase.Integration.Electric;
using Miner.Interop;
using PGE.Interface.Integration.DMS.Common;
using PGE.Interface.Integration.DMS.Manager;
using System.Diagnostics;

namespace PGE.Interface.Integration.DMS.Processors
{
    public class Line : BaseProcessor
    {
        private Dictionary<int, bool> _nonjunctions;
        private Dictionary<double, bool> _ids;
        public Line(DataSet data)
            : base(data, "DMSSTAGING.LINE")
        {
            _nonjunctions = new Dictionary<int, bool>();
            _nonjunctions.Add(FCID.Value[FCID.DCRectifier], true);
            _nonjunctions.Add(FCID.Value[FCID.DeliveryPoint], true);
            _nonjunctions.Add(FCID.Value[FCID.SecondaryGeneration], true);
            // Changes for ENOS to SAP migration - DMS commented below line here 
            //_nonjunctions.Add(FCID.Value[FCID.ServiceLocation], true); 
            _nonjunctions.Add(FCID.Value[FCID.SmartMeterNetworkDevice], true);
            _nonjunctions.Add(FCID.Value[FCID.StreetLight], true);
            _ids = new Dictionary<double, bool>();
        }
        public bool shouldIncludeJunction(JunctionInfo junction)
        {
            if (_nonjunctions.ContainsKey(junction.ObjectClassID))
            {
                return false;
            }
            return true;
        }
        public void AddEdge(EdgeFeatureInfo edge, ControlTable controlTable)
        {
            //int dist = Utilities.GetDistrict(edge);
            short state = Utilities.GetState(edge);
            //each edge is a line
            foreach (EdgeInfo e in edge.Edges)
            {
                DataRow row = _table.NewRow();
                //row["ID"] = e.UniqueID;
                double id = Utilities.getID(e);
                string uid = id.ToString();
                row["FPOS"] = uid;
                //row["ABB_INT_ID"] = id;
                row["NO_KEY_1"] = GetNodeID(e.FromJunction, controlTable);
                if (_nonjunctions.ContainsKey(e.FromJunction.ObjectClassID))
                {
                    AddGenericNode(e.FromJunction, e.Line.From);
                }
                row["NO_KEY_2"] = GetNodeID(e.ToJunction, controlTable);
                if (_nonjunctions.ContainsKey(e.ToJunction.ObjectClassID))
                {
                    AddGenericNode(e.ToJunction, e.Line.To);
                }
                //row["PA_KEY_1"] = Utilities.getID(e.ObjectClassID,(uint)e.ObjectID,e.Line.Count-1).ToString();
                //row["PHASE_PERM"] = GetLinePhase(e);
                //row["SUB_AREA"] = dist;
                row["STATE"] = state;
                //row["ANNOPOS_1"] = 12;
                //row["ANNOPOS_2"] = 12;
                //row["BITS"] = 12288;

                //Get any feature class specific values, if any
                if (FeatureValues.FCID.ContainsKey(e.ObjectClassID))
                {
                    FeatureValues.FCID[e.ObjectClassID].getValues(row, edge, controlTable);
                }
                GetElectricEdgeValues((ElectricEdge)e, row);
                GetMappedValues(edge, row);

                _table.Rows.Add(row);

            }
        }
        private string GetNodeID(JunctionInfo junct, ControlTable controlTable)
        {
            //if in a substation we need to check if the junction is a SUBElectricStitchPoint
            if (NAStandalone.Substation)
            {
                if (junct.ObjectClassID == FCID.Value[FCID.SUBStitchPoint])
                {
                    if (junct is ElectricJunction)
                    {
                        FeederInfo feeder = ((ElectricJunction)junct).Feeder;
                        if (feeder.JunctionFeatures.ContainsKey(junct.ObjectKey))
                        {
                            JunctionFeatureInfo jfi = feeder.JunctionFeatures[junct.ObjectKey];
                            //Because at 10.2.1 the substation is in a separate database, we need to build our
                            //relationship mapping based on the spatial coincidence of the subelectricstitchpoint and
                            //the electricstitchpoint features.
                            int elecstitchPointOID = controlTable.GetRelatedElectricStitchPoint(junct.ObjectID);
                            if (elecstitchPointOID > 0)
                            {
                                _log4.Debug("Linked StitchPoint " + elecstitchPointOID);
                                string ID = Utilities.getID(FCID.Value[FCID.StitchPoint], elecstitchPointOID).ToString();
                                return ID;
                            }
                        }
                    }
                    
                }
            }
            return Utilities.getID(junct).ToString();
        }
        public void AddGenericNode(JunctionInfo junct, GeoPoint point)
        {
            double id = Utilities.getID(junct);
            if (!_ids.ContainsKey(id))
            {
                //We need to map the OID for processing schematics information later.
                try
                {
                    CADOPS.ClassIDToObjectIDMap[junct.ObjectClassID].Add(junct.ObjectID);
                }
                catch
                {
                    CADOPS.ClassIDToObjectIDMap.Add(junct.ObjectClassID, new List<int>());
                    CADOPS.ClassIDToObjectIDMap[junct.ObjectClassID].Add(junct.ObjectID);
                }

                _ids.Add(id, true);
                DataRow row = _data.Tables["DMSSTAGING.NODE"].NewRow();
                string uid = id.ToString();
                row["NFPOS"] = uid;
                row["STATE"] = Utilities.GetJunctionState(junct);

                //Fake GUID for right now to provide the Class ID to OID map.  Will be updated with guid later in CADAOPS.cs
                row["GUID"] = junct.ObjectClassID + ":" + junct.ObjectID;

                row["FEATURE_CLASS"] = "SecondaryJunction";
                row["XN_1"] = Utilities.ConvertXY(point.X);
                row["YN_1"] = Utilities.ConvertXY(point.Y);
                if (NAStandalone.Substation)
                {
                    row["SUBSTATION_OBJ_IDC"] = "Y";
                }
                else
                {
                    row["SUBSTATION_OBJ_IDC"] = "N";
                }
                GetElectricJunctionValues((ElectricJunction)junct, row);
                _data.Tables["DMSSTAGING.NODE"].Rows.Add(row);
            }
        }
        public static int GetLinePhase(EdgeInfo edge)
        {
            SetOfPhases phase = ((ElectricEdge)edge).OperationalPhases;
            switch (phase)
            {
                case SetOfPhases.a: return 1004;
                case SetOfPhases.b: return 104;
                case SetOfPhases.c: return 14;
                case SetOfPhases.ab: return 1204;
                case SetOfPhases.ac: return 1034;
                case SetOfPhases.bc: return 234;
                case SetOfPhases.abc: return 1234;
            }
            return 0;
        }
    }
}
