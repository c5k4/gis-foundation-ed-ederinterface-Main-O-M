using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ESRI.ArcGIS.Schematic;
using ESRI.ArcGIS.Geodatabase;
using PGE.Common.Delivery.Diagnostics;
using System.Diagnostics;
using ESRI.ArcGIS.Geometry;
using Miner.Geodatabase.Integration;

namespace PGE.Interface.Integration.DMS
{
    /// <summary>
    /// This is prototype code for extracting schematics. It should not be used in Production.
    /// </summary>
    public class SchematicExtractor
    {
        private static Log4NetLogger _log = new Log4NetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, "ED50.log4net.config");
        ISchematicConnection _connection;
        ISchematicWorkspace _workspace;
        ISchematicDataset _dataset;
        ISchematicDiagramClass _class;
        /// <summary>
        /// Initialize the extractor with the workspace that contains the schematics
        /// </summary>
        /// <param name="ws">The workspace that contains the schematics</param>
        public SchematicExtractor(IWorkspace ws)
        {
            try
            {
                ISchematicWorkspaceFactory swsf = new SchematicWorkspaceFactory();
                _workspace = swsf.Open(ws);
                if (_workspace != null)
                {
                    _dataset = _workspace.get_SchematicDatasetByName("DMS");
                    if (_dataset != null)
                    {
                        _class = ((ISchematicDiagramClassContainer)_dataset).GetSchematicDiagramClass("DMS");
                    }
                }
            }
            catch (Exception ex)
            {
                _log.Error("Error initializing Schematics.", ex);
            }

        }

        public Dictionary<int, Dictionary<int, int[]>> GetCircuitCoordinates(string circuit)
        {
            Dictionary<int, Dictionary<int, int[]>> output = null;
            if (_dataset != null && _class != null)
            {
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();
                try
                {
                    output = new Dictionary<int, Dictionary<int, int[]>>();
                    ISchematicDiagram diagram = ((ISchematicDiagramContainer)_dataset).get_SchematicDiagramByName(_class, circuit);
                    if (diagram != null)
                    {
                        ISchematicElementContainer elements = (ISchematicElementContainer)diagram;
                        //we only need nodes
                        IEnumSchematicElement enumElements = elements.get_SchematicElementsByType(esriSchematicElementType.esriSchematicNodeType);
                        enumElements.Reset();
                        ISchematicElement element = enumElements.Next();
                        while (element != null)
                        {
                            IFeature feat = (IFeature)element;
                            ISchematicElementAssociatedObject aobj = (ISchematicElementAssociatedObject)element;
                            
                            
                            int FCID = -1;
                            int OID = -1;

                            //first look in the schematics fields for the FCID and OID
                            int UCID = ((IRow)element).Fields.FindField("UCID");
                            if (UCID >= 0)
                            {
                                FCID = (int)((IRow)element).get_Value(UCID);
                            }
                            int UOID = ((IRow)element).Fields.FindField("UOID");
                            if (UOID >= 0)
                            {
                                OID = (int)((IRow)element).get_Value(UOID);
                            }

                            if (FCID == -1 || OID == -1)
                            {
                                //looking up the related feature is slow and is done as a last resort
                                IObject obj = aobj.AssociatedObject;
                                if (obj != null)
                                {
                                    FCID = obj.Class.ObjectClassID;
                                    OID = obj.OID;
                                }
                            }
                            if (FCID == -1)
                            {
                                //these are the schematic IDs not the feature's IDs
                                FCID = feat.Class.ObjectClassID;
                                OID = feat.OID;
                            }
                            IPoint point = (IPoint)feat.Shape;
                            int X = Utilities.ConvertXY(point.X);
                            int Y =  Utilities.ConvertXY(point.Y);
                            if (!output.ContainsKey(FCID))
                            {
                                output.Add(FCID, new Dictionary<int, int[]>());
                            }
                            output[FCID].Add(OID, new int[] { X, Y });
                            element = enumElements.Next();
                        }
                    }
                }
                catch (Exception ex)
                {
                    _log.Error("Error Extracting Schematics for Circuit:" + circuit, ex);
                }

                stopwatch.Stop();
                string time = stopwatch.Elapsed.ToString();
                _log.Debug("Schematics for Circuit " + circuit + " Processed. Total Time: " + time);
            }
            return output;
        }

        public static int GetX(Dictionary<int, Dictionary<int, int[]>> coordinates, JunctionFeatureInfo junct)
        {
            return getCoordinate(coordinates, junct, 0);
        }
        public static int GetY(Dictionary<int, Dictionary<int, int[]>> coordinates, JunctionFeatureInfo junct)
        {
            return getCoordinate(coordinates, junct, 1);
        }
        private static int getCoordinate(Dictionary<int, Dictionary<int, int[]>> coordinates, JunctionFeatureInfo junct, int index)
        {
            if (coordinates != null)
            {
                if (coordinates.ContainsKey(junct.ObjectClassID))
                {
                    if (coordinates[junct.ObjectClassID].ContainsKey(junct.ObjectID))
                    {
                        return coordinates[junct.ObjectClassID][junct.ObjectID][index];
                    }
                }
            }
            return 0;
        }
        public SchematicExtractor()
        {
            Initialize("c:\\schematics.sde");
        }
        public SchematicExtractor(string connection)
        {

        }
        private void Initialize(string connection)
        {
            _connection = (ISchematicConnection)new SchematicGDBSdeConnection();
            _connection.InitString = connection;
            _connection.Open();
        }
    }
}
