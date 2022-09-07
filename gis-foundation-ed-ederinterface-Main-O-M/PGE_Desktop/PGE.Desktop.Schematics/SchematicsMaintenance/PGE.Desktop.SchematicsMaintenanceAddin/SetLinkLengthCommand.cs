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
    public class SetLinkLengthCommand : ESRI.ArcGIS.Desktop.AddIns.Button
    {
        ESRI.ArcGIS.Framework.IApplication m_app = null;
        ESRI.ArcGIS.esriSystem.IExtension m_schematicExtension = null;
        ISchematicLayer m_schematicLayer;


        public SetLinkLengthCommand()
        {
            Enabled = false;
            m_app = ArcMap.Application;
        }

        ~SetLinkLengthCommand()
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

      // Should get the length from a combo - hardcode it for this sample
      SetLinkLengthCombo combo = AddIn.FromID<SetLinkLengthCombo>(ThisAddIn.IDs.SetLinkLengthCombo);
      double linkLength = combo.Length;
      if (linkLength <= 0)
      {
        System.Windows.Forms.MessageBox.Show("Invalid link length encountered. Unable to continue.", "Link Length Command", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        return;
      }

      bool abortOperation = false;

      if (m_schematicLayer == null)
      {
        System.Windows.Forms.MessageBox.Show("Schematic target not found. Unable to continue.", "Link Length Command", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        return;
      }

      // Get the in-memory diagram
      ISchematicInMemoryDiagram inMemoryDiagram = m_schematicLayer.SchematicInMemoryDiagram;
      if (inMemoryDiagram == null)
      {
        MessageBox.Show("Schematic in memory diagram not found. Unable to continue.", "Link Length Command", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        return;
      }

      ISchematicOperation schematicOperation = (ESRI.ArcGIS.Schematic.ISchematicOperation) new ESRI.ArcGIS.SchematicControls.SchematicOperation();
      try
      {
        // Get the selected schematic features
        IEnumSchematicFeature enumSchematicFeature = m_schematicLayer.GetSchematicSelectedFeatures(false);
        if (enumSchematicFeature == null || enumSchematicFeature.Count < 1)
        {
            MessageBox.Show("No schematic selected features. Unable to continue.", "Link Length Command", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        // Note the diagram is in edition - we work directly on InMemory features
        ISchematicInMemoryFeatureLink schematicInMemoryFeatureLink = GetSelectedLink(enumSchematicFeature);
        if (schematicInMemoryFeatureLink == null)
        {
            // Message is displayed in GetSelectedLink
            return;
        }

        if (schematicInMemoryFeatureLink.FromNode == null || schematicInMemoryFeatureLink.ToNode == null)
        {
            MessageBox.Show("Selected link has bad connectivity. Unable to continue.", "Link Length Command", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        // Check to see if either (or both) of the nodes are selected
        bool isFromNodeSelected = false;
        if (!IsNodeSelected(enumSchematicFeature, schematicInMemoryFeatureLink.ToNode))
        {
            isFromNodeSelected = IsNodeSelected(enumSchematicFeature, schematicInMemoryFeatureLink.FromNode);
        }

        // Prepare to abort the operation if an error occurs
        abortOperation = true;

        // Get a copy of the the in-memory link geometry
        IPolyline inMemoryPolylineCopy = schematicInMemoryFeatureLink.ShapeCopy as IPolyline;
        if (inMemoryPolylineCopy == null)
        {
            MessageBox.Show("Null link geometry encountered. Unable to continue.", "Link Length Command", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        double inMemoryPolylineCopyLength = inMemoryPolylineCopy.Length;
        if (inMemoryPolylineCopyLength <= 0.0)
        {
            MessageBox.Show("Link line length less than zero. Unable to continue.", "Link Length Command", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        ICurve curve = inMemoryPolylineCopy as ICurve;
        if (curve == null)
        {
            MessageBox.Show("Invalid geometry encountered. Unable to continue.", "Link Length Command", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return; // Cannot work on reflexive link (same FROM and TO)
        }

        // Modify the link geometry
        ICurve outputCurve;
        //distance by ratio (0 = start of curve and 1 = end of curve, outside curve either < 0 or > 1)
        double ratioDistance = linkLength / inMemoryPolylineCopyLength;
        if (ratioDistance < 1)
        {
          // Reduce geometry
          if (isFromNodeSelected)
            curve.GetSubcurve((1.0 - ratioDistance), 1.0, true, out outputCurve);  // reduce at from node
          else
            curve.GetSubcurve(0.0, ratioDistance, true, out outputCurve); // reduce at to node
        }
        else if (ratioDistance > 1)
        {
            // Extend geometry
            IClone clone = curve as IClone;
            outputCurve = clone.Clone() as ICurve;
            IPointCollection points = outputCurve as IPointCollection;
            IPoint tangentPoint = new PointClass(); // must be co-created prior to get it from the curve

            if (isFromNodeSelected)
            {
                curve.QueryPoint(esriSegmentExtension.esriExtendTangentAtFrom, (1.0 - ratioDistance), true, tangentPoint); // extend first segment at from node
                points.UpdatePoint(0, tangentPoint);
            }
            else
            {
                curve.QueryPoint(esriSegmentExtension.esriExtendTangentAtTo, ratioDistance, true, tangentPoint); // extend last segment at to node
                points.UpdatePoint((points.PointCount - 1), tangentPoint);
            }
        }
        else
        {
            return; // Nothing to do (exact length)
        }

        // the output curve holds the new link geometry
        if (outputCurve == null || outputCurve.IsEmpty)
        {
            MessageBox.Show("Modified geometry is invalid. Unable to continue.", "Link Length Command", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }

        // Add the operation to the stack so it will respond to undo/redo
        IMxDocument doc = (IMxDocument)ArcMap.Application.Document;
        ESRI.ArcGIS.SystemUI.IOperationStack operationStack;
        operationStack = doc.OperationStack;
        operationStack.Do(schematicOperation);
        schematicOperation.StartOperation("Set Link Length", ArcMap.Application, m_schematicLayer, true);

        // Store the changes to the in-memory feature link 
        IGeometry newGeom = outputCurve as IGeometry;
        schematicInMemoryFeatureLink.Shape = newGeom;
        schematicInMemoryFeatureLink.Store();

        // Store the new node position
        IPoint newPoint = new PointClass(); // newPoint must be co-created prior to get it from the curve
        ISchematicInMemoryFeatureNode schematicInMemoryFeatureNode;
        if (isFromNodeSelected)
        {
          outputCurve.QueryFromPoint(newPoint);
          schematicInMemoryFeatureNode = schematicInMemoryFeatureLink.FromNode;
        }
        else
        {
          outputCurve.QueryToPoint(newPoint);
          schematicInMemoryFeatureNode = schematicInMemoryFeatureLink.ToNode;
        }
        schematicInMemoryFeatureNode.Shape = newPoint;
        schematicInMemoryFeatureNode.Store();

        // Stop editing
        abortOperation = false;
        schematicOperation.StopOperation();

        // Refresh the diagram
        RefreshView();
      }
      catch (Exception ex)
      {
        if (abortOperation && (schematicOperation != null))
        {
          schematicOperation.AbortOperation();
        }

        MessageBox.Show("An error occured while modifying the link length. " + ex.Message, "Link Length Command", MessageBoxButtons.OK, MessageBoxIcon.Error);
        return;
      }
    }

        /// <summary>
        /// Get the selected in memory feature link
        /// </summary>
        /// <param name="enumSchematicFeature"></param>
        /// <returns></returns>
        private ISchematicInMemoryFeatureLink GetSelectedLink(IEnumSchematicFeature enumSchematicFeature)
        {
            ISchematicInMemoryFeatureLink schematicLinkFeature = null;

            try
            {
                // Iterate through the selected features to find a link
                enumSchematicFeature.Reset();
                ISchematicFeature schematicFeature = enumSchematicFeature.Next();
                while (schematicFeature != null)
                {
                    ISchematicInMemoryFeatureLinkGeometry schematicLinkGeom = schematicFeature as ISchematicInMemoryFeatureLinkGeometry;
                    if (schematicLinkGeom != null)
                    {
                        if (schematicLinkFeature == null)
                        {
                            schematicLinkFeature = schematicFeature as ISchematicInMemoryFeatureLink;
                        }
                        else
                        {
                            string message = "More than one schematic link is currently selected. Only select one and try again.";
                            MessageBox.Show(message, "Find Schematic Annotation Tool", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return null;
                        }
                    }
                    schematicFeature = enumSchematicFeature.Next();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occured while getting the selected link. " + ex.Message, "Link Length Command", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }

            return schematicLinkFeature;
        }


        /// <summary>
        /// Checks to see if the schematic in memory Node is selected
        /// </summary>
        /// <param name="enumSchematicFeature"></param>
        /// <param name="schematicNode"></param>
        /// <returns></returns>
        private bool IsNodeSelected(IEnumSchematicFeature enumSchematicFeature, ISchematicInMemoryFeatureNode schematicNode)
        {
            if (schematicNode == null)
                return false;

            try
            {

                // Iterate through the selected features to test each node
                enumSchematicFeature.Reset();
                ISchematicFeature schematicFeature = enumSchematicFeature.Next();
                while (schematicFeature != null)
                {
                    ISchematicInMemoryFeatureNode schematicInMemoryNode = schematicFeature as ISchematicInMemoryFeatureNode;
                    if (schematicInMemoryNode != null && schematicInMemoryNode == schematicNode)
                        return true;

                    schematicFeature = enumSchematicFeature.Next();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occured while determing if the 'Node' is selected. " + ex.Message, "Link Length Command", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            return false;
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
                MessageBox.Show("An error occured while refreshing the diagram. " + ex.Message, "Link Length Command", MessageBoxButtons.OK, MessageBoxIcon.Error);
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




//////using System;
//////using System.Windows.Forms;

//////using ESRI.ArcGIS.Geodatabase;
//////using ESRI.ArcGIS.Schematic;
//////using ESRI.ArcGIS.SchematicUI;
//////using ESRI.ArcGIS.Carto;
//////using ESRI.ArcGIS.SchematicControls;
//////using ESRI.ArcGIS.esriSystem;
//////using ESRI.ArcGIS.Geometry;
//////using ESRI.ArcGIS.ArcMapUI;
//////using ESRI.ArcGIS.Display;
//////using ESRI.ArcGIS.Desktop.AddIns;

//////namespace Esri.SchematicsMaintenance
//////{
//////    public class SetLinkLengthCommand : ESRI.ArcGIS.Desktop.AddIns.Button
//////    {

//////        SchematicExtension m_schematicExtension = null;
//////        ISchematicLayer m_schematicLayer;


//////        public SetLinkLengthCommand()
//////        {
//////            Enabled = false;
//////        }

//////        ~SetLinkLengthCommand()
//////        {
//////            m_schematicExtension = null;
//////        }

//////        protected override void OnClick()
//////        {
//////            base.OnClick();

//////            SetLinkLengthCombo combo = AddIn.FromID<SetLinkLengthCombo>(ThisAddIn.IDs.SetLinkLengthCombo);
//////            double linkLength = combo.Length;
//////            if (linkLength <= 0)
//////            {
//////                System.Windows.Forms.MessageBox.Show("Invalid link length encountered. Unable to continue.", "Link Length Command", MessageBoxButtons.OK, MessageBoxIcon.Warning);
//////                return;
//////            }

//////            bool abortOperation = false;

//////            if (m_schematicLayer == null)
//////            {
//////                System.Windows.Forms.MessageBox.Show("Schematic target not found. Unable to continue.", "Link Length Command", MessageBoxButtons.OK, MessageBoxIcon.Warning);
//////                return;
//////            }

//////            // Get the in-memory diagram
//////            ISchematicInMemoryDiagram inMemoryDiagram = m_schematicLayer.SchematicInMemoryDiagram;
//////            if (inMemoryDiagram == null)
//////            {
//////                MessageBox.Show("Schematic in memory diagram not found. Unable to continue.", "Link Length Command", MessageBoxButtons.OK, MessageBoxIcon.Warning);
//////                return;
//////            }
//////            ISchematicInMemoryFeatureClassContainer schematicInMemoryFeatureClassContainer = (ISchematicInMemoryFeatureClassContainer)inMemoryDiagram;
//////            if (schematicInMemoryFeatureClassContainer == null)
//////            {
//////                MessageBox.Show("Schematic in memory feature class container not found. Unable to continue.", "Link Length Command", MessageBoxButtons.OK, MessageBoxIcon.Warning);
//////                return;
//////            }

//////            ISchematicOperation schematicOperation = (ESRI.ArcGIS.Schematic.ISchematicOperation)new ESRI.ArcGIS.SchematicControls.SchematicOperation();
          
//////            try
//////            {

//////                // Get the selected schematic features
//////                IEnumSchematicFeature enumSchematicFeature = m_schematicLayer.GetSchematicSelectedFeatures(false);
//////                if (this.HasSelection(enumSchematicFeature) == false)
//////                {
//////                    MessageBox.Show("No schematic selected features. Unable to continue.", "Link Length Command", MessageBoxButtons.OK, MessageBoxIcon.Warning);
//////                    return;
//////                }

//////                // Get the selected link feature
//////                ISchematicElementContainer schematicElementContainer = m_schematicLayer.SchematicDiagram as ISchematicElementContainer;
//////                ISchematicFeature schematicFeature = GetSelectedLink(schematicElementContainer, enumSchematicFeature);
//////                if (schematicFeature == null)
//////                {
//////                    MessageBox.Show("No selected link found. Unable to continue.", "Link Length Command", MessageBoxButtons.OK, MessageBoxIcon.Warning);
//////                    return;
//////                }

//////                // Check to see if either (or both) of the nodes are selected
//////                ISchematicElement schematicElement = schematicElementContainer.get_SchematicElementByName(esriSchematicElementType.esriSchematicLinkType, schematicFeature.Name);
//////                ISchematicLink schematicLink = schematicElement as ISchematicLink;
//////                bool isToNodeSelected = IsToNodeSelected(schematicElementContainer, enumSchematicFeature, schematicLink);
//////                bool isFromNodeSelected = IsFromNodeSelected(schematicElementContainer, enumSchematicFeature, schematicLink);

//////                // Get the in-memory feature
//////                ISchematicInMemoryFeature schematicInMemoryFeature = schematicFeature as ISchematicInMemoryFeature;
//////                ISchematicInMemoryFeatureLink schematicInMemoryFeatureLink = schematicInMemoryFeature as ISchematicInMemoryFeatureLink;

//////                // Prepare to abort the operation if an error occurs
//////                abortOperation = true;

//////                try
//////                {

//////                    // Add the operation to the stack so it will respond to undo/redo
//////                    IMxDocument doc = (IMxDocument)ArcMap.Application.Document;
//////                    ESRI.ArcGIS.SystemUI.IOperationStack operationStack;
//////                    operationStack = doc.OperationStack;
//////                    operationStack.Do(schematicOperation);
//////                    schematicOperation.StartOperation("Set Link Length", ArcMap.Application, m_schematicLayer, true);

//////                    // Get the in-memory polyline
//////                    IPolyline inMemoryPolyline = schematicInMemoryFeatureLink.Shape as IPolyline;
//////                    IPointCollection pointCollectionInMemory = inMemoryPolyline as IPointCollection;

//////                    // Get a copy of the in-memory polyline
//////                    IPolyline inMemoryPolylineCopy = schematicInMemoryFeatureLink.ShapeCopy as IPolyline;
//////                    IPointCollection pointCollectionCopyInMemory = inMemoryPolylineCopy as IPointCollection;
//////                    double inMemoryPolylineCopyLength = inMemoryPolylineCopy.Length;
//////                    ISchematicInMemoryFeatureLinkGeometry schematicInMemoryFeatureLinkGeometryCopy = schematicInMemoryFeatureLink as ISchematicInMemoryFeatureLinkGeometry;
//////                    int vertexCountCopy = schematicInMemoryFeatureLinkGeometryCopy.VerticesCount;

//////                    // Iterate until the copy of the line only has two points or it is shorter than the link length
//////                    while (inMemoryPolylineCopyLength > linkLength && pointCollectionCopyInMemory.PointCount > 2)
//////                    {
//////                        if (isToNodeSelected == false && isFromNodeSelected)
//////                        {
//////                            pointCollectionCopyInMemory.RemovePoints(0, 1); // Remove the first node from the copy
//////                        }
//////                        else
//////                        {
//////                            pointCollectionCopyInMemory.RemovePoints((pointCollectionInMemory.PointCount - 1), 1); // Remove the last node from the copy
//////                        }

//////                        // Need to make a new polyline - using the in-memory copy did not work correctly (length did not change?)
//////                        IPolyline polyline = new PolylineClass();
//////                        IPointCollection points = polyline as IPointCollection;
//////                        points.AddPointCollection(pointCollectionCopyInMemory);

//////                        // Only remove the point from the in-memory polyline if the line length is greater than the link length
//////                        inMemoryPolylineCopyLength = polyline.Length;
//////                        if (inMemoryPolylineCopyLength > linkLength) // This is why a copy was used (to determine the new line length before adjusting the in-memory polyline)
//////                        {
//////                            if (isToNodeSelected == false && isFromNodeSelected)
//////                            {
//////                                pointCollectionInMemory.RemovePoints(0, 1); // Remove the first node
//////                            }
//////                            else
//////                            {
//////                                pointCollectionInMemory.RemovePoints((pointCollectionInMemory.PointCount - 1), 1); // Remove the last node
//////                            }
//////                        }                   
//////                    }

//////                    // Store the changes to the in-memory feature
//////                    //IPolyline newpolyline = new PolylineClass();
//////                    //IPointCollection newpoints = newpolyline as IPointCollection;
//////                    //newpoints.AddPointCollection(pointCollectionInMemory);
//////                    //schematicInMemoryFeature.Shape = newpolyline;
//////                    schematicInMemoryFeature.Store();

//////                    // Move the node to the correct position so that the line length equals the link length
//////                    ISchematicInMemoryFeatureNode schematicInMemoryFeatureNode = null;
//////                    IConstructPoint contructionPoint = new PointClass();
//////                    if (isToNodeSelected == false && isFromNodeSelected)
//////                    {
//////                        schematicInMemoryFeatureNode = schematicInMemoryFeatureLink.FromNode;
//////                        double distance = inMemoryPolyline.Length - linkLength;
//////                        contructionPoint.ConstructAlong(inMemoryPolyline as ICurve, esriSegmentExtension.esriExtendAtFrom, distance, false);
//////                    }
//////                    else
//////                    {
//////                        schematicInMemoryFeatureNode = schematicInMemoryFeatureLink.ToNode;
//////                        contructionPoint.ConstructAlong(inMemoryPolyline as ICurve, esriSegmentExtension.esriExtendAtTo, linkLength, false);
//////                    }

//////                    // Store the change to the node
//////                    schematicInMemoryFeatureNode.UpdateStatus = esriSchematicUpdateStatus.esriSchematicUpdateStatusLocked;
//////                    schematicInMemoryFeatureNode.Shape = contructionPoint as IPoint;
//////                    schematicInMemoryFeatureNode.Store();

//////                    // Stop editing
//////                    abortOperation = false;
//////                    schematicOperation.StopOperation();

//////                    // Refresh the diagram
//////                    RefreshView();
//////                }
//////                catch (Exception ex)
//////                {

//////                }

//////            }
//////            catch (Exception ex)
//////            {
//////                if (abortOperation && (schematicOperation != null))
//////                {
//////                    schematicOperation.AbortOperation();
//////                }
//////                MessageBox.Show("An error occured while modifying the link length. " + ex.Message, "Link Length Command", MessageBoxButtons.OK, MessageBoxIcon.Error);
//////                return;
//////            }

//////        }


//////        /// <summary>
//////        /// Enables or disables the command
//////        /// </summary>
//////        protected override void OnUpdate()
//////        {
//////            // the tool is enable only if the diagram is in memory
//////            try
//////            {
//////                if (ArcMap.Application == null)
//////                {
//////                    Enabled = false;
//////                    return;
//////                }

//////                if (m_schematicExtension == null)
//////                {
//////                    m_schematicExtension = Utils.GetSchematicExtension();
//////                    Enabled = false;
//////                    return;
//////                }

//////                SetTargetLayer();

//////                if (m_schematicLayer == null)
//////                {
//////                    Enabled = false;
//////                    return;
//////                }

//////                if (m_schematicLayer.IsEditingSchematicDiagram() == false)
//////                {
//////                    Enabled = false;
//////                    return;
//////                }

//////                if (m_schematicLayer.SchematicInMemoryDiagram == null)
//////                {
//////                    Enabled = false;
//////                    return;
//////                }

//////                Enabled = true;
//////            }
//////            catch (System.Exception e)
//////            {
//////                //System.Windows.Forms.MessageBox.Show(e.Message);
//////            }
//////            return;

//////        }

//////        /// <summary>
//////        /// Check for selected features
//////        /// </summary>
//////        /// <param name="enumSchematicFeature"></param>
//////        /// <returns></returns>
//////        private bool HasSelection(IEnumSchematicFeature enumSchematicFeature)
//////        {
//////            try
//////            {
//////                // Iterate through the enumneration to determine if there are any selected schematic features
//////                enumSchematicFeature.Reset();
//////                ISchematicFeature schematicFeature = enumSchematicFeature.Next();
//////                if (schematicFeature != null)
//////                {
//////                    return true;
//////                }
//////            }
//////            catch (Exception ex)
//////            {
//////                MessageBox.Show("An error occured while checking for selected schematic features. " + ex.Message, "Link Length Command", MessageBoxButtons.OK, MessageBoxIcon.Error);
//////                return false;
//////            }
//////            return false;
//////        }

//////        /// <summary>
//////        /// Get the selected link
//////        /// </summary>
//////        /// <param name="schematicElementContainer"></param>
//////        /// <param name="enumSchematicFeature"></param>
//////        /// <returns></returns>
//////        private ISchematicFeature GetSelectedLink(ISchematicElementContainer schematicElementContainer, IEnumSchematicFeature enumSchematicFeature)
//////        {
//////            ISchematicFeature schematicLinkFeature = null;

//////            try
//////            {
//////                // Iterate through the selected features to find a link
//////                enumSchematicFeature.Reset();
//////                ISchematicFeature schematicFeature = enumSchematicFeature.Next();
//////                while (schematicFeature != null)
//////                {
//////                    // Test to see if the schematic feature is a link
//////                    ISchematicElement schematicElement = schematicElementContainer.get_SchematicElementByName(esriSchematicElementType.esriSchematicLinkType, schematicFeature.Name);
//////                    if (schematicElement is ISchematicLink)
//////                    {
//////                        if (schematicLinkFeature == null)
//////                        {
//////                            schematicLinkFeature = schematicFeature;
//////                        }
//////                        else
//////                        {
//////                            string message = "More than one schematic link is currently selected. Only select one and try again.";
//////                            MessageBox.Show(message, "Find Schematic Annotation Tool", MessageBoxButtons.OK, MessageBoxIcon.Warning);
//////                            return null;
//////                        }
//////                    }
//////                    schematicFeature = enumSchematicFeature.Next();
//////                }
//////            }
//////            catch (Exception ex)
//////            {
//////                MessageBox.Show("An error occured while getting the selected link. " + ex.Message, "Link Length Command", MessageBoxButtons.OK, MessageBoxIcon.Error);
//////                return null;
//////            }

//////            return schematicLinkFeature as ISchematicFeature;
//////        }

//////        /// <summary>
//////        /// Checks to see of the To Node on the link is selected
//////        /// </summary>
//////        /// <param name="schematicElementContainer"></param>
//////        /// <param name="enumSchematicFeature"></param>
//////        /// <param name="schematicFeatureLink"></param>
//////        /// <returns></returns>
//////        private bool IsToNodeSelected(ISchematicElementContainer schematicElementContainer, IEnumSchematicFeature enumSchematicFeature, ISchematicLink schematicFeatureLink)
//////        {
//////            try
//////            {
//////                // Iterate through the selected features to test each node
//////                enumSchematicFeature.Reset();
//////                ISchematicFeature schematicFeature = enumSchematicFeature.Next();
//////                while (schematicFeature != null)
//////                {
//////                    ISchematicElement schematicElement = schematicElementContainer.get_SchematicElementByName(esriSchematicElementType.esriSchematicNodeType, schematicFeature.Name);
//////                    if (schematicElement == null)
//////                    {
//////                        schematicElement = schematicElementContainer.get_SchematicElementByName(esriSchematicElementType.esriSchematicNodeOnLinkType, schematicFeature.Name);
//////                    }
//////                    if (schematicElement != null)
//////                    {
//////                        // Determine if the selected node matches the ToNode on the link
//////                        if (schematicElement.Class.CLSID.Value.Equals(schematicFeatureLink.ToNode.Class.CLSID.Value))
//////                        {
//////                            if (schematicElement.OID == schematicFeatureLink.ToNode.OID)
//////                            {
//////                                return true;
//////                            }
//////                        }
//////                    }
//////                    schematicFeature = enumSchematicFeature.Next();
//////                }
//////            }
//////            catch (Exception ex)
//////            {
//////                MessageBox.Show("An error occured while determing if the 'ToNode' is selected. " + ex.Message, "Link Length Command", MessageBoxButtons.OK, MessageBoxIcon.Error);
//////                return false;
//////            }
//////            return false;
//////        }

//////        /// <summary>
//////        /// Checks to see of the From Node on the link is selected
//////        /// </summary>
//////        /// <param name="schematicElementContainer"></param>
//////        /// <param name="enumSchematicFeature"></param>
//////        /// <param name="schematicFeatureLink"></param>
//////        /// <returns></returns>
//////        private bool IsFromNodeSelected(ISchematicElementContainer schematicElementContainer, IEnumSchematicFeature enumSchematicFeature, ISchematicLink schematicFeatureLink)
//////        {
//////            try
//////            {
//////                // Iterate through the selected features to test each node
//////                enumSchematicFeature.Reset();
//////                ISchematicFeature schematicFeature = enumSchematicFeature.Next();
//////                while (schematicFeature != null)
//////                {
//////                    ISchematicElement schematicElement = schematicElementContainer.get_SchematicElementByName(esriSchematicElementType.esriSchematicNodeType, schematicFeature.Name);
//////                    if (schematicElement == null)
//////                    {
//////                        schematicElement = schematicElementContainer.get_SchematicElementByName(esriSchematicElementType.esriSchematicNodeOnLinkType, schematicFeature.Name);
//////                    }
//////                    if (schematicElement != null)
//////                    {
//////                        // Determine if the selected node matches the FromNode on the link
//////                        if (schematicElement.Class.CLSID.Value.Equals(schematicFeatureLink.FromNode.Class.CLSID.Value))
//////                        {
//////                            if (schematicElement.OID == schematicFeatureLink.FromNode.OID)
//////                            {
//////                                return true;
//////                            }
//////                        }
//////                    }
//////                    schematicFeature = enumSchematicFeature.Next();
//////                }
//////            }
//////            catch (Exception ex)
//////            {
//////                MessageBox.Show("An error occured while determing if the 'FromNode' is selected. " + ex.Message, "Link Length Command", MessageBoxButtons.OK, MessageBoxIcon.Error);
//////                return false;
//////            }

//////            return false;
//////        }

//////        /// <summary>
//////        /// Refreshes the view containing the schematic diagram
//////        /// </summary>
//////        private void RefreshView()
//////        {
//////            try
//////            {
//////                // Get the document and the active view
//////                IMxDocument mxDocument2 = ArcMap.Application.Document as IMxDocument;
//////                if (mxDocument2 == null)
//////                    return;

//////                IMap map = mxDocument2.FocusMap;
//////                IActiveView activeView = (IActiveView)map;

//////                if (activeView != null)
//////                    activeView.Refresh();

//////                // Refresh viewer window
//////                IApplicationWindows applicationWindows = ArcMap.Application as IApplicationWindows;

//////                // Refresh the data window
//////                ISet mySet = applicationWindows.DataWindows;
//////                if (mySet != null)
//////                {
//////                    mySet.Reset();
//////                    IMapInsetWindow dataWindow = (IMapInsetWindow)mySet.Next();
//////                    while (dataWindow != null)
//////                    {
//////                        dataWindow.Refresh();
//////                        dataWindow = (IMapInsetWindow)mySet.Next();
//////                    }
//////                }
//////            }
//////            catch (Exception ex)
//////            {
//////                MessageBox.Show("An error occured while refreshing the diagram. " + ex.Message, "Link Length Command", MessageBoxButtons.OK, MessageBoxIcon.Error);
//////                return;
//////            }

//////        }

//////        /// <summary>
//////        /// Returns the schematic layer
//////        /// </summary>
//////        private void SetTargetLayer()
//////        {
//////            try
//////            {
//////                m_schematicLayer = this.GetCurrentSchematicLayer();
//////            }
//////            catch
//////            {
//////                //System.Windows.Forms.MessageBox.Show(e.Message);
//////            }
//////        }

//////        /// <summary>
//////        /// Gets the schematic layer from the extension
//////        /// </summary>
//////        /// <returns></returns>
//////        private ISchematicLayer GetCurrentSchematicLayer()
//////        {
//////            try
//////            {
//////                // Get the schematic target from the Schematic extension
//////                ISchematicTarget target = m_schematicExtension as ISchematicTarget;
//////                if (target != null)
//////                {
//////                    ISchematicLayer schematicLayer = target.SchematicTarget;
//////                    return schematicLayer;
//////                }
//////            }
//////            catch
//////            {
//////                //MessageBox.Show("An error occured while getting the schematic target. " + ex.Message, "Link Length Command", MessageBoxButtons.OK, MessageBoxIcon.Error);
//////                return null;
//////            }

//////            return null;
//////        }


//////    }

    


//////}

