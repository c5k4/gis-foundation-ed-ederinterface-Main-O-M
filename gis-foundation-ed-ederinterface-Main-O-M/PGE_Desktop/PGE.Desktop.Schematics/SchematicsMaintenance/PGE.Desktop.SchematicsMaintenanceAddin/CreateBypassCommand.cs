using System;

using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.Schematic;
using ESRI.ArcGIS.SchematicControls;
using ESRI.ArcGIS.esriSystem;
using System.Windows.Forms;
using ESRI.ArcGIS.ArcMapUI;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.Desktop.AddIns;


namespace Esri.SchematicsMaintenance
{
    public class CreateBypassCommand : ESRI.ArcGIS.Desktop.AddIns.Button
    {
        ESRI.ArcGIS.Framework.IApplication m_app = null;
        ESRI.ArcGIS.esriSystem.IExtension m_schematicExtension = null;
        ISchematicLayer m_schematicLayer;


        public CreateBypassCommand()
        {
            Enabled = false;
            m_app = ArcMap.Application;
        }

        ~CreateBypassCommand()
        {
            m_schematicExtension = null;
            m_app = null;
        }


        /// <summary>
        /// Enables or disables the command
        /// </summary>
        protected override void OnUpdate()
        {
            // the tool is enable only if the diagram is in memory
            try
            {
                if (ArcMap.Application == null)
                {
                    Enabled = false;
                    return;
                }

                SetTargetLayer();

                if (m_schematicLayer == null)
                {
                    Enabled = false;
                    return;
                }

                if (m_schematicLayer.IsEditingSchematicDiagram() == false)
                {
                    Enabled = false;
                    return;
                }

                ISchematicInMemoryDiagram inMemoryDiagram = m_schematicLayer.SchematicInMemoryDiagram;
                if (inMemoryDiagram == null)
                    Enabled = false;
                else
                    Enabled = true;
            }
            catch (System.Exception e)
            {
                System.Windows.Forms.MessageBox.Show(e.Message);
            }
            return;
        }

        /// <summary>
        /// Respond to click event 
        /// </summary>
        protected override void OnClick()
        {
            base.OnClick();

            // Get the length from the combo
            SetLinkLengthCombo combo = AddIn.FromID<SetLinkLengthCombo>(ThisAddIn.IDs.SetLinkLengthCombo);
            double linkLength = combo.Length;
            if (linkLength <= 0)
            {
                System.Windows.Forms.MessageBox.Show("Invalid link length encountered. Unable to continue.", "Link Perpendicular Length Command", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (m_schematicLayer == null)
            {
                System.Windows.Forms.MessageBox.Show("Schematic target not found. Unable to continue.", "Create Bypass Command", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Get the in-memory diagram
            ISchematicInMemoryDiagram inMemoryDiagram = m_schematicLayer.SchematicInMemoryDiagram;
            if (inMemoryDiagram == null)
            {
                MessageBox.Show("Schematic in memory diagram not found. Unable to continue.", "Create Bypass Command", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            ISchematicOperation schematicOperation = (ESRI.ArcGIS.Schematic.ISchematicOperation)new ESRI.ArcGIS.SchematicControls.SchematicOperation();
            try
            {

                // Get the selected schematic features
                IEnumSchematicFeature enumSchematicFeature = m_schematicLayer.GetSchematicSelectedFeatures(false);
                if (enumSchematicFeature == null || enumSchematicFeature.Count < 1)
                {
                    MessageBox.Show("No schematic selected features. Unable to continue.", "Create Bypass Command", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Make certain the right schematic features are selected
                ISchematicInMemoryFeatureNode offsetInMemoryFeatureNode = this.Validate(enumSchematicFeature);
                if (offsetInMemoryFeatureNode == null)
                {
                    return;
                }

                // Get the bypass links from the bypass node
                IEnumSchematicInMemoryFeatureLink enumSchematicInMemoryFeatureLink = offsetInMemoryFeatureNode.GetIncidentLinks(esriSchematicEndPointType.esriSchematicOriginOrExtremityNode);
                
                // Modify the bypass geometry
                ISchematicInMemoryFeatureLink connectedSchematicInMemoryFeatureLink1 = enumSchematicInMemoryFeatureLink.Next(); // The first link
                IPoint fromPoint = this.ModifyBypassLink(offsetInMemoryFeatureNode, connectedSchematicInMemoryFeatureLink1, linkLength);
                if (fromPoint != null)
                {

                    ISchematicInMemoryFeatureLink connectedSchematicInMemoryFeatureLink2 = enumSchematicInMemoryFeatureLink.Next(); // The second link
                    IPoint toPoint = this.ModifyBypassLink(offsetInMemoryFeatureNode, connectedSchematicInMemoryFeatureLink2, linkLength);

                    this.ModifyOffsetNode(offsetInMemoryFeatureNode, fromPoint, toPoint);
                }

                // Refresh the diagram
                RefreshView();
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occured while modifying the bypass. " + ex.Message, "Create Bypass Command", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

        }

        /// <summary>
        /// Moves the bypass node to the line that connects the bypass links
        /// </summary>
        /// <param name="offsetInMemoryFeatureNode"></param>
        /// <param name="fromPoint"></param>
        /// <param name="toPoint"></param>
        private void ModifyOffsetNode(ISchematicInMemoryFeatureNode offsetInMemoryFeatureNode, IPoint fromPoint, IPoint toPoint )
        {
            ISchematicOperation schematicOperation = (ESRI.ArcGIS.Schematic.ISchematicOperation)new ESRI.ArcGIS.SchematicControls.SchematicOperation();
            bool abortOperation = true;

            try
            {
                IPolyline bypassLine = new PolylineClass();
                bypassLine.SpatialReference = offsetInMemoryFeatureNode.Shape.SpatialReference;
                bypassLine.FromPoint = fromPoint;
                bypassLine.ToPoint = toPoint;

                // Add the operation to the stack so it will respond to undo/redo
                IMxDocument doc = (IMxDocument)ArcMap.Application.Document;
                ESRI.ArcGIS.SystemUI.IOperationStack operationStack;
                operationStack = doc.OperationStack;
                operationStack.Do(schematicOperation);
                schematicOperation.StartOperation("Move Bypass Node", ArcMap.Application, m_schematicLayer, true);

                abortOperation = true;

                IPoint offsetPoint = offsetInMemoryFeatureNode.Shape as IPoint;
                IPoint nearestPoint = new PointClass();
                nearestPoint.SpatialReference = offsetInMemoryFeatureNode.Shape.SpatialReference;
                IProximityOperator proximityOperator = bypassLine as IProximityOperator;
                proximityOperator.QueryNearestPoint(offsetPoint, esriSegmentExtension.esriNoExtension, nearestPoint);
                offsetInMemoryFeatureNode.Shape = nearestPoint;
                offsetInMemoryFeatureNode.Store();

                // Stop editing
                abortOperation = false;
                schematicOperation.StopOperation();
            }
            catch (Exception ex)
            {
                if (abortOperation && (schematicOperation != null))
                {
                    schematicOperation.AbortOperation();
                }

                MessageBox.Show("An error occured while modifying the bypass offset node. " + ex.Message, "Create Bypass Command", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }


        /// <summary>
        /// Changes the geometry of a bypass line by adding a vertex which makes the link perpendicular to the main line
        /// </summary>
        /// <param name="offsetInMemoryFeatureNode"></param>
        /// <param name="connectedSchematicInMemoryFeatureLink"></param>
        /// <param name="linkLength"></param>
        /// <returns></returns>
        private IPoint ModifyBypassLink(ISchematicInMemoryFeatureNode offsetInMemoryFeatureNode, ISchematicInMemoryFeatureLink connectedSchematicInMemoryFeatureLink, double linkLength)
        {
            // Prepare to abort the operation if an error occurs
            bool abortOperation = true;
            ISchematicOperation schematicOperation = (ESRI.ArcGIS.Schematic.ISchematicOperation)new ESRI.ArcGIS.SchematicControls.SchematicOperation();

            try
            {

                // Get the perpendicular node
                ISchematicInMemoryFeatureNode perpendicularInMemoryFeatureNode = this.GetOppositeEndNode(offsetInMemoryFeatureNode, connectedSchematicInMemoryFeatureLink);
                if (perpendicularInMemoryFeatureNode == null)
                {
                    return null;
                }

                // Get the main link
                ISchematicInMemoryFeatureLink mainInMemoryFeatureLink = this.GetMainLink(connectedSchematicInMemoryFeatureLink, perpendicularInMemoryFeatureNode);

                // Determine which side of the main line the link is on
                IPolyline mainInMemoryPolylineCopy = mainInMemoryFeatureLink.ShapeCopy as IPolyline; // Not correct
                bool isOnRightSide = IsRightSide(mainInMemoryPolylineCopy, offsetInMemoryFeatureNode.ShapeCopy as IPoint);
                if (!isOnRightSide)
                {
                    linkLength *= -1;
                }

                // Get the distance of the perpendicular point along the main line
                double distanceAlongLine = this.GetDistanceAlongLine(mainInMemoryPolylineCopy, perpendicularInMemoryFeatureNode.ShapeCopy as IPoint);

                // Get the perpendicular line at the perpendicular point 
                ILine perpendicularLine = GetPerpendicularLine(mainInMemoryPolylineCopy, distanceAlongLine, linkLength);

                // Add the operation to the stack so it will respond to undo/redo
                IMxDocument doc = (IMxDocument)ArcMap.Application.Document;
                ESRI.ArcGIS.SystemUI.IOperationStack operationStack;
                operationStack = doc.OperationStack;
                operationStack.Do(schematicOperation);
                schematicOperation.StartOperation("Create Bypass Link", ArcMap.Application, m_schematicLayer, true);

                // Remove all vertices from the link - removes the interior vertices, leaves the end points
                this.RemoveAllVertices(connectedSchematicInMemoryFeatureLink);

                // Add a vertex to the link
                this.AddVertex(connectedSchematicInMemoryFeatureLink, perpendicularLine.ToPoint);

                // Store the changes to the in-memory feature link 
                connectedSchematicInMemoryFeatureLink.Store();

                // Stop editing
                abortOperation = false;
                schematicOperation.StopOperation();

                return perpendicularLine.ToPoint;

            }
            catch (Exception ex)
            {
                if (abortOperation && (schematicOperation != null))
                {
                    schematicOperation.AbortOperation();
                }

                MessageBox.Show("An error occured while modifying the bypass geometry. " + ex.Message, "Create Bypass Command", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }


        }

        /// <summary>
        /// Makes certain the selected schematic features are part of a bypass
        /// </summary>
        /// <param name="enumSchematicFeature"></param>
        /// <returns></returns>
        private ISchematicInMemoryFeatureNode Validate(IEnumSchematicFeature enumSchematicFeature)
        {
            ISchematicInMemoryFeatureNode schematicInMemoryFeatureNode = null;
            ISchematicInMemoryFeatureLink schematicInMemoryFeatureLink1 = null;
            ISchematicInMemoryFeatureLink schematicInMemoryFeatureLink2 = null;

            try
            {
                // Iterate through the selected features to find the selected links
                int linkCount = 0;
                enumSchematicFeature.Reset();
                ISchematicFeature schematicFeature = enumSchematicFeature.Next();
                while (schematicFeature != null)
                {
                    ISchematicInMemoryFeatureLinkGeometry schematicLinkGeom = schematicFeature as ISchematicInMemoryFeatureLinkGeometry;
                    if (schematicLinkGeom != null)
                    {
                        linkCount++;
                        if (linkCount == 1)
                        {
                            schematicInMemoryFeatureLink1 = schematicFeature as ISchematicInMemoryFeatureLink;
                        }
                        else if (linkCount == 2)
                        {
                            schematicInMemoryFeatureLink2 = schematicFeature as ISchematicInMemoryFeatureLink;
                        }
                    }
                    schematicFeature = enumSchematicFeature.Next();
                }
                if (linkCount != 2)
                {
                    MessageBox.Show("Incorrect number of links selected. Two links must be selected.", "Create Bypass Command", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return null;
                }

                // Iterate through the selected features to find the selected node
                int nodeCount = 0;
                enumSchematicFeature.Reset();
                schematicFeature = enumSchematicFeature.Next();
                while (schematicFeature != null)
                {
                    ISchematicInMemoryFeatureNodeGeometry schematicNodeGeom = schematicFeature as ISchematicInMemoryFeatureNodeGeometry;
                    if (schematicNodeGeom != null)
                    {
                        schematicInMemoryFeatureNode = schematicFeature as ISchematicInMemoryFeatureNode;
                        ++nodeCount;
                    }
                    schematicFeature = enumSchematicFeature.Next();
                }

                // The node is not selected - get it anyway
                if (nodeCount == 0)
                {
                    if (schematicInMemoryFeatureLink1.ToNode == schematicInMemoryFeatureLink2.FromNode)
                    {
                        schematicInMemoryFeatureNode = schematicInMemoryFeatureLink1.ToNode;
                        ++nodeCount;
                    }
                    if (schematicInMemoryFeatureLink1.FromNode == schematicInMemoryFeatureLink2.ToNode)
                    {
                        schematicInMemoryFeatureNode = schematicInMemoryFeatureLink1.FromNode;
                        ++nodeCount;
                    }
                }

                if (nodeCount != 1)
                {
                    MessageBox.Show("Incorrect number of nodes selected. One node must be selected.", "Create Bypass Command", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return null;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occured while validating the selected links and node. " + ex.Message, "Create Bypass Command", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            IEnumSchematicInMemoryFeatureLink enumSchematicInMemoryFeatureLink = schematicInMemoryFeatureNode.GetIncidentLinks(esriSchematicEndPointType.esriSchematicOriginOrExtremityNode);
            if (enumSchematicInMemoryFeatureLink.Count != 2)
            {
                MessageBox.Show("Invalid connections at the selected node. ", "Create Bypass Command", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return null;
            }

            try
            {
                int numberofLinks = enumSchematicInMemoryFeatureLink.Count;
                for (int i = 0; i < numberofLinks; i++)
                {
                    ISchematicInMemoryFeatureLink connectedSchematicInMemoryFeatureLink = enumSchematicInMemoryFeatureLink.Next();
                    if (i == 0 && connectedSchematicInMemoryFeatureLink != schematicInMemoryFeatureLink1)
                    {
                        MessageBox.Show("Invalid connection between the selected node and the first link. ", "Create Bypass Command", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return null;
                    }
                    if (i == 1 && connectedSchematicInMemoryFeatureLink != schematicInMemoryFeatureLink2)
                    {
                        MessageBox.Show("Invalid connection between the selected node and the second link. ", "Create Bypass Command", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return null;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occured while validating the bypass node. " + ex.Message, "Create Bypass Command", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return schematicInMemoryFeatureNode;
        }


        /// <summary>
        /// Determines the distance along a line to a point on the line
        /// </summary>
        /// <param name="inMemoryPolylineCopy"></param>
        /// <param name="point"></param>
        /// <returns></returns>
        private double GetDistanceAlongLine(IPolyline inMemoryPolylineCopy, IPoint point)
        {
            double distanceAlongLine = 0;
            double distanceFromLine = 0;
            bool isRightSide = true;
            IPoint outPoint = new PointClass();

            try
            {
                inMemoryPolylineCopy.QueryPointAndDistance(esriSegmentExtension.esriNoExtension, point, false, outPoint, ref distanceAlongLine, ref distanceFromLine, ref isRightSide);
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occured while performing query point and distance operation. " + ex.Message, "Create Bypass Command", MessageBoxButtons.OK, MessageBoxIcon.Error);
                throw ex;
            }

            return distanceAlongLine;
        }

        /// <summary>
        /// Gets a perpendicualr line to a line at a point.
        /// </summary>
        /// <param name="inMemoryPolylineCopy"></param>
        /// <param name="distanceAlongLine"></param>
        /// <param name="linkLength"></param>
        /// <returns></returns>
        private ILine GetPerpendicularLine(IPolyline inMemoryPolylineCopy, double distanceAlongLine, double linkLength)
        {
            ILine outLine = new LineClass();

            try
            {
                inMemoryPolylineCopy.QueryNormal(esriSegmentExtension.esriNoExtension, distanceAlongLine, false, linkLength, outLine);
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occured while performing query normal operation. " + ex.Message, "Create Bypass Command", MessageBoxButtons.OK, MessageBoxIcon.Error);
                throw ex;
            }
            return outLine;
        }

        /// <summary>
        /// Determines if a point is on the right side of a line
        /// </summary>
        /// <param name="inMemoryPolylineCopy"></param>
        /// <param name="point"></param>
        /// <returns></returns>
        private bool IsRightSide(IPolyline inMemoryPolylineCopy, IPoint point)
        {
            IPoint outPoint = new PointClass();
            double distanceAlongLine = 0;
            double distanceFromLine = 0;
            bool isRightSide = true;

            try
            {
                inMemoryPolylineCopy.QueryPointAndDistance(esriSegmentExtension.esriNoExtension, point, true, outPoint, ref distanceAlongLine, ref distanceFromLine, ref isRightSide);
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occured while determining line orientation. " + ex.Message, "Create Bypass Command", MessageBoxButtons.OK, MessageBoxIcon.Error);
                throw ex;
            }

            return isRightSide;
        }

        /// <summary>
        /// Gets the other node on a link
        /// </summary>
        /// <param name="aNode"></param>
        /// <param name="connectedSchematicInMemoryFeatureLink"></param>
        /// <returns></returns>
        private ISchematicInMemoryFeatureNode GetOppositeEndNode(ISchematicInMemoryFeatureNode aNode, ISchematicInMemoryFeatureLink connectedSchematicInMemoryFeatureLink)
        {
            try
            {
                if (connectedSchematicInMemoryFeatureLink.FromNode == aNode)
                {
                    return connectedSchematicInMemoryFeatureLink.ToNode;
                }
                if (connectedSchematicInMemoryFeatureLink.ToNode == aNode)
                {
                    return connectedSchematicInMemoryFeatureLink.FromNode;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occured while getting the perpendicular node. " + ex.Message, "Create Bypass Command", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return null;
        }

        /// <summary>
        /// Gets the first in memory link connected to an in memory link at an in memory node
        /// </summary>
        /// <param name="schematicInMemoryFeatureLink"></param>
        /// <param name="schematicInMemoryFeatureNode"></param>
        /// <returns></returns>
        private ISchematicInMemoryFeatureLink GetMainLink(ISchematicInMemoryFeatureLink schematicInMemoryFeatureLink, ISchematicInMemoryFeatureNode schematicInMemoryFeatureNode)
        {
            IEnumSchematicInMemoryFeatureLink enumSchematicInMemoryFeatureLink = schematicInMemoryFeatureNode.GetIncidentLinks(esriSchematicEndPointType.esriSchematicOriginOrExtremityNode);
            if (enumSchematicInMemoryFeatureLink.Count == 1)
            {
                MessageBox.Show("Invalid connection at the selected node. ", "Create Bypass Command", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return null;
            }

            int numberofLinks = enumSchematicInMemoryFeatureLink.Count;
            for (int i = 0; i < numberofLinks; i++)
            {
                ISchematicInMemoryFeatureLink connectedSchematicInMemoryFeatureLink = enumSchematicInMemoryFeatureLink.Next();

                if (connectedSchematicInMemoryFeatureLink != schematicInMemoryFeatureLink)
                {
                    return connectedSchematicInMemoryFeatureLink;
                }

            }

            MessageBox.Show("Main link not found.", "Create Bypass Command", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return null;
        }

        /// <summary>
        /// Removes all interion vertices from an in memory link. End points are not removed.
        /// </summary>
        /// <param name="schematicInMemoryFeatureLink"></param>
        private void RemoveAllVertices(ISchematicInMemoryFeatureLink schematicInMemoryFeatureLink)
        {
            try
            {
                ISchematicInMemoryFeatureLinkGeometry schematicLinkGeom = schematicInMemoryFeatureLink as ISchematicInMemoryFeatureLinkGeometry;
                schematicLinkGeom.RemoveAllVertices();
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occured while removing vertices. " + ex.Message, "Create Bypass Command", MessageBoxButtons.OK, MessageBoxIcon.Error);
                throw ex;
            }
        }

        /// <summary>
        /// Adds a vertex to the link
        /// </summary>
        /// <param name="schematicInMemoryFeatureLink"></param>
        private void AddVertex(ISchematicInMemoryFeatureLink schematicInMemoryFeatureLink, IPoint newVertex)
        {
            try
            {
                ISchematicInMemoryFeatureLinkGeometry schematicLinkGeom = schematicInMemoryFeatureLink as ISchematicInMemoryFeatureLinkGeometry;
                schematicLinkGeom.AddVertex(newVertex);
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occured while adding a vertex. " + ex.Message, "Create Bypass Command", MessageBoxButtons.OK, MessageBoxIcon.Error);
                throw ex;
            }
        }

        /// <summary>
        /// Refreshes the view containing the schematic diagram
        /// </summary>
        private void RefreshView()
        {
            try
            {
                IMxDocument mxDocument2 = (IMxDocument)m_app.Document;
                if (mxDocument2 == null)
                    return;

                IMap map = mxDocument2.FocusMap;
                IActiveView activeView = (IActiveView)map;

                if (activeView != null)
                    activeView.Refresh();

                //refresh viewer window
                IApplicationWindows applicationWindows = m_app as IApplicationWindows;

                ISet mySet = applicationWindows.DataWindows;
                if (mySet != null)
                {
                    mySet.Reset();
                    IMapInsetWindow dataWindow = (IMapInsetWindow)mySet.Next();
                    while (dataWindow != null)
                    {
                        dataWindow.Refresh();
                        dataWindow = (IMapInsetWindow)mySet.Next();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occured while refreshing the diagram. " + ex.Message, "Create Bypass Command", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// get the current schematic layer and the schematic extension
        /// </summary>
        private void SetTargetLayer()
        {
            try
            {
                if (m_schematicLayer == null)
                {
                    IExtension extention = null;
                    IExtensionManager extensionManager;

                    extensionManager = (IExtensionManager)m_app;
                    extention = extensionManager.FindExtension("SchematicUI.SchematicExtension");

                    if (extention == null)
                        Enabled = false;
                    else
                    {
                        m_schematicExtension = extention;
                        ISchematicTarget target = m_schematicExtension as ISchematicTarget;
                        if (target != null)
                            m_schematicLayer = target.SchematicTarget;
                    }
                }
            }
            catch (System.Exception e)
            {
                System.Windows.Forms.MessageBox.Show(e.Message);
            }
        }
    }

}


