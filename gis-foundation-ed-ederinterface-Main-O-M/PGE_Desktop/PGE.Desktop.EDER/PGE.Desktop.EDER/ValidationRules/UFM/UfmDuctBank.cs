using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;

using ESRI.ArcGIS.ArcMapUI;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.Framework;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;

using Miner.Interop;

using PGE.Desktop.EDER;
using PGE.Desktop.EDER.UFM;
using PGE.Common.Delivery.Diagnostics;

namespace PGE.Desktop.EDER.ValidationRules.UFM
{
    /// <summary>
    /// Stores a representation of a duct bank that can then be compared with another representation.
    /// Each representation just has a list of ducts ordered by duct position. Each duct tracks the following
    /// attributes:
    ///     Occupied: If the duct has something in it or not
    ///     xDirection: The relative position in the x-axis of the duct compared to the 
    ///                 previous duct (ordered by duct position). ie: +1 is to the right 
    ///                 of, -1 is to the left of and 0 is in the same position
    ///     yDirection: The relative position in the y-axis of the duct compare to the
    ///                 previous duct (ordered by duct position)
    /// Representations can be added from a duct bank feature class or from a conduits Configuration field. 
    /// The xDir and yDir are relevant because it allows us to determine if two diagrams are drawn the same
    /// without getting into pixel level precision. It also allows us to ensure that one diagram has the 
    /// 'opposite' representation of another (ie: if its +1 in one diagram, it should be -1 in the other). 
    /// Note that duct bank features are rotated to match the conduit configuration rotation (of zero degrees 
    /// or 'upright') so that the relative positions work out correctly for the comparisons.
    /// </summary>
    public class UfmDuctBank
    {
        #region Constants

        private const string DIR_NORTH = "N";
        private const string DIR_NORTH_EAST = "NE";
        private const string DIR_EAST = "E";
        private const string DIR_SOUTH_EAST = "SE";
        private const string DIR_SOUTH = "S";
        private const string DIR_SOUTH_WEST = "SW";
        private const string DIR_WEST = "W";
        private const string DIR_NORTH_WEST = "NW";
        
        #endregion

        #region Member vars

        // For debug logging
        private static readonly Log4NetLogger _logger = new Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "EDERDesktop.log4net.config");

        // For keeping track of the list of ducts in the duct bank keyed by duct position
        private IDictionary<string, UfmDuct> ducts;

        #endregion

        #region Constructor

        /// <summary>
        /// Create a duct bank representation of the supplied duct bank configuration
        /// </summary>
        /// <param name="dbc"></param>
        public UfmDuctBank(IMMDuctBankConfig dbc)
        {
            // Create a new empty set of ducts to work with
            ducts = new Dictionary<string, UfmDuct>();

            // Add ducts based on the supplied configuration
            CreateFromConfiguration(dbc);
        }

        /// <summary>
        /// Create a duct bank representation of the supplied duct bank feature class
        /// </summary>
        /// <param name="ductBank"></param>
        /// <param name="opposite"></param>
        public UfmDuctBank(IFeature ductBank, double angle, bool opposite)
        {
            // Create a new empty set of ducts to work with
            ducts = new Dictionary<string, UfmDuct>();

            // Add ducts based on the supplied duct bank feature
            CreateFromDuctBank(ductBank, angle, opposite);
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Compares the current duct bank representation with the supplied duct bank representation by 
        /// comparing each duct to ensure they have the same properties.
        /// </summary>
        /// <param name="compareDuctBank"></param>
        /// <returns>true if the number of ducts are the same and each duct has the same properties</returns>
        public bool Compare(UfmDuctBank compareDuctBank)
        {
            bool match = true;

            // If they have the same number of ducts...
            if (this.ducts.Count == compareDuctBank.ducts.Count)
            {
                // For each duct in the bank...
                foreach (string key in ducts.Keys)
                {
                    // If the duct bank being compared also has that duct...
                    if (compareDuctBank.ducts.ContainsKey(key) == true)
                    {
                        // Make sure they match
                        if (this.ducts[key].Compare(compareDuctBank.ducts[key]) == false)
                        {
                            match = false;
                            break;
                        }
                    }
                    else
                    {
                        // Duct banks are different
                        match = false;
                        break;
                    }
                }
            }
            else
            {
                // Duct banks are different
                match = false;
            }

            // Return the result
            return match;
        }

        /// <summary>
        /// Compares the current duct bank representation with the supplied duct bank representation by 
        /// comparing each duct within and ensuring they are opposite each other in the x-axis.
        /// </summary>
        /// <param name="compareDuctBank"></param>
        /// <returns></returns>
        public bool IsMirrored(UfmDuctBank compareDuctBank)
        {
            // If they have the same number of ducts...
            if (ducts.Count != compareDuctBank.ducts.Count) return false;

            // For each duct in the bank...
            foreach (string key in this.ducts.Keys)
            {
                // If the duct bank being compared also has that duct...
                // Make sure they are opposite from each other
                if (!compareDuctBank.ducts.ContainsKey(key) || !ducts[key].IsMirrored(compareDuctBank.ducts[key]))
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Draws the current duct bank representation to the output window for debugging
        /// </summary>
        /// <param name="compareDuctBank"></param>
        /// <returns></returns>
        public void Draw()
        {
            foreach (string key in ducts.Keys)
            {
                int x = ducts[key].xDir;
                int y = ducts[key].yDir;
                if (y == 1)
                {
                    Debug.WriteLine("");
                    Debug.Write("^");
                }
                else if (y == -1)
                {
                    Debug.WriteLine("");
                    Debug.Write("v");
                }
                else
                {
                    if (x == 1)
                    {
                        Debug.Write(">");
                    }
                    else if (x == -1)
                    {
                        Debug.Write("<");
                    }
                    else
                    {
                        Debug.Write("0");
                    }
                }
            }
            Debug.WriteLine("");
        }

        #endregion

        #region Private methods

        private void CreateFromConfiguration(IMMDuctBankConfig dbc)
        {
            try
            {
                // Reset the config
                ID8List dbcList = dbc as ID8List;
                dbcList.Reset();

                // Get the list of ducts
                ID8List ductViewList = (ID8List)dbcList.Next(false);
                ductViewList.Reset();

                // For each duct in the list...
                ID8ListItem listItem = ductViewList.Next(false);
                IMMDuctDefinition previousDuctDef = null;
                while (listItem != null)
                {
                    // Double check it is a duct
                    if (listItem is IMMDuctDefinition)
                    {
                        // Determine its occupancy
                        IMMDuctDefinition currentDuctDef = listItem as IMMDuctDefinition;
                        bool occupied = IsDuctOccupied(listItem as ID8List);
                        Add(currentDuctDef, previousDuctDef);
                        previousDuctDef = currentDuctDef;
                    }

                    // Move to the next duct
                    listItem = ductViewList.Next(false);
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Failed to build blob diagram: " + ex.ToString());
                throw ex;
            }

        }

        private void CreateFromDuctBank(IFeature ductBank, double angle, bool opposite)
        {
            try
            {
                // Get a list of ducts within the bank that are keyed by duct position
                IFeatureCursor fcDucts = UfmHelper.GetDucts(ductBank);
                IDictionary<int, IFeature> orderedDucts = GetOrderedList(fcDucts);

                // Determine the origin and rotation angle of the duct bank feature
                IPoint origin = GetRotationPoint(ductBank);

                // For each duct in dictionary...
                IFeature previousDuct = null;
                for (int i = 1; i <= orderedDucts.Count; i++)
                {
                    // Apply a fake rotation so that the duct is facing up (as it would be shown in the config editor)
                    IFeature newDuct = orderedDucts[i];
                    IFeature rotated = RotateDuct(newDuct, origin, angle, false);

                    //Rotate 180 deg and reset calculations to place '1' duct on top left
                    if (i == 2 && rotated.Shape.Envelope.XMin < previousDuct.Shape.Envelope.XMin)
                    {
                        angle += 180;
                        i = 0;
                        previousDuct = null;
                        ducts = new Dictionary<string, UfmDuct>();
                        continue;
                    }

                    // Add the duct to the butterfly and keep track of the previous one
                    Add(rotated, previousDuct);
                    //RotateDuct(newDuct, origin, -angle, false);
                    previousDuct = rotated;
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Failed to get diagram from duct bank" + ex.ToString());
                throw ex;
            }
        }

        /// <summary>
        /// Add a new duct to the duct bank representation based on the supplied duct feature
        /// </summary>
        /// <param name="position"></param>
        /// <param name="ductPresent"></param>
        /// <param name="ductOccupied"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        private UfmDuct Add(IFeature duct, IFeature previousDuct)
        {
            UfmDuct newDuct = null;

            try
            {
                // Create a new duct
                newDuct = new UfmDuct();

                // Set occupancy
                newDuct.DuctOccupied = false;
                object occupied = UfmHelper.GetFieldValue(duct as IRow, SchemaInfo.UFM.FieldModelNames.Occupied);
                if (occupied != null)
                {
                    if (int.Parse(occupied.ToString()) == 1)
                    {
                        newDuct.DuctOccupied = true;
                    }
                }

                // Determine relative direction from adjacent duct in the bank
                if (previousDuct != null)
                {
                    newDuct.xDir = GetDirection(duct.Shape.Envelope.XMin, previousDuct.Shape.Envelope.XMin, 1);
                    newDuct.yDir = GetDirection(duct.Shape.Envelope.YMin, previousDuct.Shape.Envelope.YMin, 1);
                }
                else
                {
                    newDuct.xDir = 0;
                    newDuct.yDir = 0;
                }

                // Get the key (duct ID) and add to the duct bank
                object ductPosition = UfmHelper.GetFieldValue(duct as IRow, SchemaInfo.UFM.FieldModelNames.DuctId);
                if (ductPosition != null)
                {
                    ducts.Add(ductPosition.ToString(), newDuct);
                }
            }
            catch (Exception ex)
            {
                _logger.Warn("Failed to add duct: " + ex.ToString());
                throw ex;
            }

            // Return the new duct
            return newDuct;
        }

        /// <summary>
        /// Adds a new duct to the duct bank representation based on the supplied duct definition
        /// </summary>
        /// <param name="duct"></param>
        /// <param name="previousDuct"></param>
        /// <returns></returns>
        private UfmDuct Add(IMMDuctDefinition duct, IMMDuctDefinition previousDuct)
        {
            UfmDuct newDuct = null;

            try
            {
                // Create a new duct
                newDuct = new UfmDuct();

                // Set occupancy
                newDuct.DuctOccupied = IsDuctOccupied(duct as ID8List);

                // Determine relative direction from adjacent duct in the bank
                if (previousDuct != null)
                {
                    newDuct.xDir = GetDirection(duct.xCoordinate, previousDuct.xCoordinate, 2);
                    newDuct.yDir = GetDirection(duct.yCoordinate, previousDuct.yCoordinate, 2) * -1;
                }
                else
                {
                    newDuct.xDir = 0;
                    newDuct.yDir = 0;
                }

                // Get the key (duct ID) and add to the duct bank
                ducts.Add(duct.ductID, newDuct);
            }
            catch (Exception ex)
            {
                _logger.Warn("Failed to add duct: " + ex.ToString());
                throw ex;
            }

            // Return the new duct
            return newDuct;
        }

        /// <summary>
        /// Returns true if the supplied duct detail (from the blob) has a conductor object within it
        /// </summary>
        /// <param name="ductDetailList"></param>
        /// <returns></returns>
        private bool IsDuctOccupied(ID8List ductDetailList)
        {
            bool occupied = false;

            try
            {
                if (ductDetailList != null)
                {

                    // Get a list of the contents of the duct
                    ductDetailList.Reset();
                    ID8ListItem ductDetailListItem = ductDetailList.Next(false);

                    // While there are more items...
                    while (ductDetailListItem != null)
                    {
                        // Check to see if we found a cable or phase
                        if (ductDetailListItem is IMMDuctCable || ductDetailListItem is IMMDuctPhase)
                        {
                            // Its occupied... save and bail out
                            occupied = true;
                            break;
                        }

                        // Look at the next item
                        ductDetailListItem = ductDetailList.Next(false);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Warn("Failed to check duct occupancy: " + ex.ToString());
                throw ex;
            }

            // Return the result
            return occupied;
        }

        /// <summary>
        /// Returns the vertex from the supplied duct bank feature that is nearest to the center of the UFM floor
        /// that the duct bank is associated with. This point will be used to rotate the ducts in the duct bank
        /// so that they are at the same angle as they would be in the configuration editor (so we can more easily
        /// compare them)
        /// </summary>
        /// <param name="ductBank"></param>
        /// <returns></returns>
        private IPoint GetRotationPoint(IFeature ductBank)
        {
            IPoint nearestPoint = null;
            double nearestPointDistance = 0;

            try
            {
                // Get the centroid of the floor of the butterfly the duct is in
                IFeature ufmFloor = UfmHelper.GetUfmFloor(ductBank);
                IArea area = ufmFloor.Shape as IArea;
                IPoint floorCentroid = area.Centroid;

                // Get the list of vertices from the duct bank
                IPointCollection ductBankVertices = ductBank.Shape as IPointCollection;

                // For each vertex in the duct bank            
                for (int pointIndex = 0; pointIndex < ductBankVertices.PointCount; pointIndex++)
                {
                    // Calculate distance between current point and centroid
                    IPoint currPoint = ductBankVertices.Point[pointIndex];
                    IProximityOperator op = currPoint as IProximityOperator;
                    double currDistance = op.ReturnDistance(floorCentroid as IGeometry);

                    // If this is the first point...
                    if (nearestPoint == null)
                    {
                        // Its the nearest so far!
                        nearestPoint = currPoint;
                        nearestPointDistance = currDistance;
                    }
                    else
                    {
                        // See if the new point is closer than the closest point encountered so far
                        if (currDistance < nearestPointDistance)
                        {
                            // If so, store it
                            nearestPoint = currPoint;
                            nearestPointDistance = currDistance;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Failed to find nearest point for rotation: " + ex.ToString());
                throw ex;
            }

            // Return the result
            return nearestPoint;
        }

        /// <summary>
        /// Returns a hashtable of ducts, keyed by duct position, for each duct in the supplied cursor
        /// </summary>
        /// <param name="cursorDucts"></param>
        /// <returns></returns>
        private IDictionary<int, IFeature> GetOrderedList(IFeatureCursor cursorDucts)
        {
            IDictionary<int, IFeature> orderedDucts = new Dictionary<int, IFeature>();

            // Grab the first duct
            IFeature duct = cursorDucts.NextFeature();
            
            // While there are more ducts...
            while (duct != null)
            {
                // Get the duct ID
                object ductId = UfmHelper.GetFieldValue(duct, SchemaInfo.UFM.FieldModelNames.DuctId);
                if (ductId != null)
                {
                    // Add it to the ordered duct list
                    int val = int.Parse(ductId.ToString());
                    orderedDucts.Add(val, duct);
                }

                // Move to the next duct
                duct = cursorDucts.NextFeature();
            }

            // Return the resulting list
            return orderedDucts;
        }

        /// <summary>
        /// Calculates and returns the rotation angle needed to rotate the supplied duct bank feature
        /// to make it appear 'upright'.
        /// </summary>
        /// <param name="ductBank"></param>
        /// <returns></returns>
        public static double CalculateRotationAngle(IFeature ductBank)
        {
            double angle = 0;
            try
            {
                // Get the centroid of the floor of the butterfly the duct is in
                IFeature ufmFloor = UfmHelper.GetUfmFloor(ductBank);
                IArea area = ufmFloor.Shape as IArea;
                IPoint floorCentroid = area.Centroid;

                // Get the first two vertices of the duct bank
                IPointCollection ductBankVertices = ductBank.Shape as IPointCollection;
                IPoint vertex1 = GetVertexPointByDistance(floorCentroid, ductBank.Shape as IPointCollection, 1);
                IPoint vertex2 = GetVertexPointByDistance(floorCentroid, ductBank.Shape as IPointCollection, 2);

                // Ensure the vertices we use run from left to right
                if (vertex2.X < vertex1.X)
                {
                    IPoint vertex3 = vertex1;
                    vertex1 = vertex2;
                    vertex2 = vertex3;
                }

#if DEBUG
                // Use the following code to visualize the line for debugging
                IPointCollection collection = new PolylineClass();
                collection.AddPoint(vertex1);
                collection.AddPoint(vertex2);
                AddGraphicToMap(collection as IGeometry);
#endif

                // Determine the angle of the line formed by the two vertices
                double x1 = vertex1.X;
                double y1 = vertex1.Y;

                double x2 = vertex2.X;
                double y2 = vertex2.Y;

                double xDiff = x2 - x1;
                double yDiff = y2 - y1;
                angle = Math.Atan2(yDiff, xDiff) * 180.0 / Math.PI;

                // Apply a 180 degree rotation if were on the bottom half of the butterfly
                /*if (floorCentroid.Y > vertex1.Y)
                {
                    angle = angle - 180;
                }*/
            }
            catch (Exception ex)
            {
                _logger.Error("Failed to calculate angle for rotation: " + ex.ToString());
                throw ex;
            }

            return angle;
        }

        /// <summary>
        /// Returns the n-th vertex, identified by index, from the supplied point collection after the 
        /// collection has been ordered by distance from the point identified by origin.
        /// </summary>
        /// <param name="origin"></param>
        /// <param name="vertices"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        private static IPoint GetVertexPointByDistance(IPoint origin, IPointCollection vertices, int index)
        {
            IPoint vertex = null;
            
            // Get a list of the points ordered by shortest first
            IList<IPoint> orderedVertices = GetListOrderedByDistance(origin, vertices);
            
            // Our list is zero based, but this call is not, so subtract one and pull the vertex
            index--;
            if (index < orderedVertices.Count)
            {
                vertex = orderedVertices[index];
            }

            // Return the result
            return vertex;
        }

        /// <summary>
        /// Returns a list of points from the supplied point collection in order of distance 
        /// (nearest first).
        /// </summary>
        /// <param name="origin"></param>
        /// <param name="vertices"></param>
        /// <returns></returns>
        private static IList<IPoint> GetListOrderedByDistance(IPoint origin, IPointCollection vertices)
        {
            IList<double> orderedDistances = new List<double>();
            IList<IPoint> orderedPoints = new List<IPoint>();

            try
            {
                // For each vertex in the duct bank            
                for (int pointIndex = 0; pointIndex < vertices.PointCount - 1; pointIndex++)
                {
                    // Calculate distance between current point and centroid
                    IPoint currPoint = vertices.Point[pointIndex];
                    IProximityOperator op = currPoint as IProximityOperator;
                    double currDistance = op.ReturnDistance(origin as IGeometry);

                    // If this is the first point...
                    if (orderedPoints.Count == 0)
                    {
                        // Its the nearest so far!
                        orderedPoints.Add(currPoint);
                        orderedDistances.Add(currDistance);
                    }
                    else
                    {                        
                        // See if the new point is closer than the closest point encountered so far
                        bool bAdded = false;
                        for (int index = 0; index < orderedDistances.Count; index++)
                        {
                            if (currDistance < orderedDistances[index])
                            {
                                // Its the nearest so far!
                                orderedPoints.Insert(index, currPoint);
                                orderedDistances.Insert(index, currDistance);
                                bAdded = true;
                                break;
                            }
                        }

                        if (bAdded == false)
                        {
                            orderedPoints.Add(currPoint);
                            orderedDistances.Add(currDistance);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Failed to find nearest point for rotation: " + ex.ToString());
                throw ex;
            }

            // Return the result
            return orderedPoints;
        }

        /// <summary>
        /// Rotates the supplied duct feature class about the origin point by angle degress.
        /// This rotation is not persisted unless the save flag is set.
        /// </summary>
        /// <param name="duct"></param>
        /// <param name="origin"></param>
        /// <param name="angle"></param>
        /// <param name="save"></param>
        private IFeature RotateDuct(IFeature duct, IPoint origin, double rotationAngle, bool save)
        {
            if (rotationAngle == 0) return duct;

            //Convert to radians
            double radAngle = rotationAngle * 0.0174532925;
            
            /*
            ITransform2D transform = duct.Shape as ITransform2D;
            transform.Rotate(origin, angle);
            duct.Shape = transform as IGeometry;
            return duct;
            */
            
            // Rotate the duct about the origin
            IGeometry ductShapeCopy = duct.ShapeCopy;
            ITransform2D transform2 = (ITransform2D) ductShapeCopy;
            transform2.Rotate(origin, radAngle);

            IFeatureBuffer feat = ((IFeatureClass) duct.Class).CreateFeatureBuffer();
            feat.Shape = transform2 as IGeometry;

            IFields fields = duct.Fields;

            int fieldCount = fields.FieldCount;

            // count from 2 so we don't get the oid/globalid.
            for (int i = 2; i < fieldCount; i++)
            {
                if (feat.Fields.FindField("SHAPE") == i) continue;
                try
                {
                    feat.Value[i] = duct.Value[i];
                }
                catch (Exception)
                { }
            }
             

            return (IFeature) feat;
            
            // Store it back
            //duct.Shape = transform as IGeometry;
#if DEBUG
            //save = true;
#endif

            // To visualize the rotation (for debug purposes, set save = true)
            if (save)
            {
                // Add origin point
                //AddGraphicToMap(origin as IGeometry);

                // Create a edit op and save the rotation
                IWorkspace ws = (duct.Class as IDataset).Workspace;
                IWorkspaceEdit workspaceEdit = ws as IWorkspaceEdit;
                workspaceEdit.StartEditOperation();
                duct.Store();
                workspaceEdit.StopEditOperation();

                // Refresh the view
                RefreshView();
            }

        }

        /// <summary>
        /// Calculates the relative direction of the current number compared to the previous number
        /// </summary>
        /// <param name="current"></param>
        /// <param name="previous"></param>
        /// <returns></returns>
        private int GetDirection(double current, double previous, int noOfDecimalsToRound)
        {
            int direction = 0;

            // Calculate the delta between the two, rounding to eliminate small discrepencies
            double delta = current - previous;
            delta = Math.Round(delta, noOfDecimalsToRound);

            // Calculate relative direction based (0 = same, 1 = greater, -1 = lower)
            if (delta == 0)
            {
                direction = 0;
            }
            else if (delta > 0)
            {
                direction = 1;
            }
            else
            {
                direction = -1;
            }

            // Return the result
            return direction;
        }

        #region Debug code for visualizing rotation

        ///<summary>Draw a specified graphic on the map using the supplied colors.</summary>
        ///      
        ///<param name="map">An IMap interface.</param>
        ///<param name="geometry">An IGeometry interface. It can be of the geometry type: esriGeometryPoint, esriGeometryPolyline, or esriGeometryPolygon.</param>
        ///<param name="rgbColor">An IRgbColor interface. The color to draw the geometry.</param>
        ///<param name="outlineRgbColor">An IRgbColor interface. For those geometry's with an outline it will be this color.</param>
        ///      
        ///<remarks>Calling this function will not automatically make the graphics appear in the map area. Refresh the map area after after calling this function with Methods like IActiveView.Refresh or IActiveView.PartialRefresh.</remarks>
        public static void AddGraphicToMap(IGeometry geometry)
        {
            Type appRefType = Type.GetTypeFromProgID("esriFramework.AppRef");
            object appRefObj = Activator.CreateInstance(appRefType);
            IApplication arcMapApp = appRefObj as IApplication;
            IMxDocument mxDoc = (IMxDocument)arcMapApp.Document;
            IMap map = mxDoc.FocusMap;

            ESRI.ArcGIS.Carto.IGraphicsContainer graphicsContainer = (ESRI.ArcGIS.Carto.IGraphicsContainer)map; // Explicit Cast
            ESRI.ArcGIS.Carto.IElement element = null;

            IRgbColor color = new RgbColorClass();
            color.Blue = 200;
            color.Red = 10;
            color.Green = 10;

            if ((geometry.GeometryType) == ESRI.ArcGIS.Geometry.esriGeometryType.esriGeometryPoint)
            {
                // Marker symbols
                ESRI.ArcGIS.Display.ISimpleMarkerSymbol simpleMarkerSymbol = new ESRI.ArcGIS.Display.SimpleMarkerSymbolClass();
                simpleMarkerSymbol.Color = color;
                simpleMarkerSymbol.Outline = false;
                simpleMarkerSymbol.Size = 1;
                simpleMarkerSymbol.Style = ESRI.ArcGIS.Display.esriSimpleMarkerStyle.esriSMSCircle;

                ESRI.ArcGIS.Carto.IMarkerElement markerElement = new ESRI.ArcGIS.Carto.MarkerElementClass();
                markerElement.Symbol = simpleMarkerSymbol;
                element = (ESRI.ArcGIS.Carto.IElement)markerElement; // Explicit Cast
            }
            else if ((geometry.GeometryType) == ESRI.ArcGIS.Geometry.esriGeometryType.esriGeometryPolyline)
            {
                //  Line elements
                ESRI.ArcGIS.Display.ISimpleLineSymbol simpleLineSymbol = new ESRI.ArcGIS.Display.SimpleLineSymbolClass();
                simpleLineSymbol.Color = color;
                simpleLineSymbol.Style = ESRI.ArcGIS.Display.esriSimpleLineStyle.esriSLSSolid;
                simpleLineSymbol.Width = 1;

                ESRI.ArcGIS.Carto.ILineElement lineElement = new ESRI.ArcGIS.Carto.LineElementClass();
                lineElement.Symbol = simpleLineSymbol;
                element = (ESRI.ArcGIS.Carto.IElement)lineElement; // Explicit Cast
            }
            else if ((geometry.GeometryType) == ESRI.ArcGIS.Geometry.esriGeometryType.esriGeometryPolygon)
            {
                // Polygon elements
                ESRI.ArcGIS.Display.ISimpleFillSymbol simpleFillSymbol = new ESRI.ArcGIS.Display.SimpleFillSymbolClass();
                simpleFillSymbol.Color = color;
                simpleFillSymbol.Style = ESRI.ArcGIS.Display.esriSimpleFillStyle.esriSFSForwardDiagonal;
                ESRI.ArcGIS.Carto.IFillShapeElement fillShapeElement = new ESRI.ArcGIS.Carto.PolygonElementClass();
                fillShapeElement.Symbol = simpleFillSymbol;
                element = (ESRI.ArcGIS.Carto.IElement)fillShapeElement; // Explicit Cast
            }
            if (!(element == null))
            {
                element.Geometry = geometry;
                graphicsContainer.AddElement(element, 0);
            }
        }

        private void RefreshView()
        {
            try
            {
                // Do a view refresh
                Type appRefType = Type.GetTypeFromProgID("esriFramework.AppRef");
                object appRefObj = Activator.CreateInstance(appRefType);
                IApplication arcMapApp = appRefObj as IApplication;
                if (arcMapApp != null)
                {
                    IMxDocument mxDoc = (IMxDocument)arcMapApp.Document;
                    (mxDoc.FocusMap as IActiveView).Refresh();
                }
            }
            catch (Exception ex)
            {
                // Not the end of the world, log and move on
                _logger.Warn("Failed to refresh view: " + ex.ToString());
            }
        }

        #endregion

        #endregion
    }
}
