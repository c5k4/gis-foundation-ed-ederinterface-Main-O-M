using System;
using System.Drawing;
using System.Runtime.InteropServices;
using ESRI.ArcGIS.ADF.BaseClasses;
using ESRI.ArcGIS.ADF.CATIDs;
using ESRI.ArcGIS.Framework;
using ESRI.ArcGIS.ArcMapUI;
using System.Windows.Forms;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.CartoUI;
using ESRI.ArcGIS.Carto;
using System.Collections.Generic;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Editor;
using Miner.ComCategories;
using System.Reflection;
using System.Resources;

namespace PGE.BatchApplication.ReplaceAnnoation
{
    /// <summary>
    /// Summary description for ReplaceAnnotation.
    /// </summary>
    [Guid("e59353cc-ba09-4189-92c2-78367cd0de76")]
    [ClassInterface(ClassInterfaceType.None)]
    [ProgId("PGE.BatchApplication.ReplaceAnnoation.ReplaceAnnotation")]
    [ComponentCategory(ComCategory.ArcMapCommands)]
    public sealed class ReplaceAnnotation : BaseTool
    {
        #region COM Registration Function(s)
        [ComRegisterFunction()]
        [ComVisible(false)]
        static void RegisterFunction(Type registerType)
        {
            // Required for ArcGIS Component Category Registrar support
            ArcGISCategoryRegistration(registerType);

            //
            // TODO: Add any COM registration code here
            //
        }

        [ComUnregisterFunction()]
        [ComVisible(false)]
        static void UnregisterFunction(Type registerType)
        {
            // Required for ArcGIS Component Category Registrar support
            ArcGISCategoryUnregistration(registerType);

            //
            // TODO: Add any COM unregistration code here
            //
        }

        #region ArcGIS Component Category Registrar generated code
        /// <summary>
        /// Required method for ArcGIS Component Category registration -
        /// Do not modify the contents of this method with the code editor.
        /// </summary>
        private static void ArcGISCategoryRegistration(Type registerType)
        {
            string regKey = string.Format("HKEY_CLASSES_ROOT\\CLSID\\{{{0}}}", registerType.GUID);
            MxCommands.Register(regKey);

        }
        /// <summary>
        /// Required method for ArcGIS Component Category unregistration -
        /// Do not modify the contents of this method with the code editor.
        /// </summary>
        private static void ArcGISCategoryUnregistration(Type registerType)
        {
            string regKey = string.Format("HKEY_CLASSES_ROOT\\CLSID\\{{{0}}}", registerType.GUID);
            MxCommands.Unregister(regKey);

        }

        #endregion
        #endregion

        private List<IFeatureLayer> AnnoFeatLayers = new List<IFeatureLayer>();
        private IApplication m_application;
        private IMxDocument mxDoc = null;
        private IPoint RotateAnglePoint = null;
        private IEditor pEditor = null;
        private IPoint point1 = null;
        private IPoint point2 = null;
        private int annoClassID = -1;
        private IPoint newPlacementPoint = null;
        private double XOffset = 0.0;
        private double YOffset = 0.0;
        private IFeature selectedFeature = null;
        private IFeatureLayer selectedFeatureLayer = null;

        public ReplaceAnnotation()
        {
            //
            // TODO: Define values for the public properties
            //
            base.m_category = "PGE Tools"; //localizable text 
            base.m_caption = "Replace Annotation";  //localizable text 
            base.m_message = "Replace Annotation";  //localizable text
            base.m_toolTip = "Replace Annotation";  //localizable text
            base.m_name = "Replace Annotation";   //unique id, non-localizable (e.g. "MyCategory_ArcMapTool")
            try
            {
                //
                // TODO: change resource name if necessary
                //
                string bitmapResourceName = GetType().Name + ".bmp";
                base.m_bitmap = new Bitmap(GetType(), bitmapResourceName);
                base.m_cursor = new System.Windows.Forms.Cursor(GetType(), GetType().Name + ".cur");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine(ex.Message, "Invalid Bitmap");
            }
        }

        #region Overridden Class Methods

        /// <summary>
        /// Occurs when this tool is created
        /// </summary>
        /// <param name="hook">Instance of the application</param>
        public override void OnCreate(object hook)
        {
            m_application = hook as IApplication;

            //Disable if it is not ArcMap
            if (hook is IMxApplication)
            {
                base.m_enabled = true;
                mxDoc = m_application.Document as IMxDocument;
            }
            else
                base.m_enabled = false;

            // TODO:  Add other initialization code
        }



        /// <summary>
        /// Occurs when this tool is clicked
        /// </summary>
        public override void OnClick()
        {

        }


        /// <summary>
        /// When the user clicks down, if they have already selected the annotation object, a rubber band will draw
        /// from the first click to specify the new location, then the second click to specify the angle, and then a
        /// popup for the user to click and specify the horizontal alignment.
        /// </summary>
        /// <param name="Button"></param>
        /// <param name="Shift"></param>
        /// <param name="X"></param>
        /// <param name="Y"></param>
        public override void OnMouseDown(int Button, int Shift, int X, int Y)
        {
            if (selectedFeature == null) { return; }

            //Begin drawing out line for the user to guage their angle
            IPolyline polyLine = GetPolylineFromMouseClicks(mxDoc.ActiveView);
            IPointCollection pointCollection = polyLine as IPointCollection;
            point1 = pointCollection.get_Point(0);
            point2 = pointCollection.get_Point(1);

            ContextMenuStrip menuStrip = new ContextMenuStrip();
            menuStrip.ItemClicked += new ToolStripItemClickedEventHandler(menuStrip_ItemClicked);
            menuStrip.Items.Add("Left Aligned");
            menuStrip.Items.Add("Right Aligned");
            menuStrip.Items.Add("Center Aligned");
            MouseCursor cursor = new MouseCursorClass();
            menuStrip.Show(System.Windows.Forms.Cursor.Position.X, System.Windows.Forms.Cursor.Position.Y);

        }

        public override bool Deactivate()
        {
            selectedFeature = null;
            selectedFeatureLayer = null;
            return base.Deactivate();
        }

        /// <summary>
        /// Event to handle when the user choses a horizontal alignment.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void menuStrip_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            try
            {
                int alignment = 0;

                if (e.ClickedItem.ToString() == "Center Aligned") { alignment = 1; }
                else if (e.ClickedItem.ToString() == "Right Aligned") { alignment = 2; }

                newPlacementPoint = point1;

                //Calculate the angle
                double dy = point2.Y - point1.Y;
                double dx = point2.X - point1.X;
                double theta = 0.0;
                if (dx != 0)
                {
                    double tan = dy / dx;
                    theta = Math.Atan(tan);
                    theta *= 180 / Math.PI;
                }
                else
                {
                    theta = 90;
                }

                if (theta <= -85 && theta >= -90)
                {
                    theta += 180;
                }

                annoClassID = Convert.ToInt32(selectedFeature.get_Value(selectedFeature.Fields.FindField("ANNOTATIONCLASSID")));

                IRelatedObjectClassEvents originEvents = null;
                IRelatedObjectClassEvents destEvents = null;
                IObject parentObject = null;

                pEditor.StartOperation();

                IEnumRelationshipClass relClasses = selectedFeature.Class.get_RelationshipClasses(esriRelRole.esriRelRoleAny);
                IRelationshipClass rel = relClasses.Next();
                //Loop through and delete.
                if (rel != null)
                {
                    //Cast as IRelatedObjectClassEvents to ensure that the relationship can be recreated.
                    originEvents = rel.OriginClass.Extension as IRelatedObjectClassEvents;
                    destEvents = rel.DestinationClass.Extension as IRelatedObjectClassEvents;

                    if (destEvents != null || originEvents != null)
                    {
                        ISet relatedFeatures = rel.GetObjectsRelatedToObject(selectedFeature);
                        parentObject = relatedFeatures.Next() as IObject;
                        if (parentObject != null)
                        {
                            ISet relObjs = rel.GetObjectsRelatedToObject(parentObject);
                            for (IObject relObj = relObjs.Next() as IObject; relObj != null; relObj = relObjs.Next() as IObject)
                            {
                                try
                                {
                                    relObj.Delete();
                                }
                                catch (Exception e2)
                                {

                                }
                            }
                        }
                        else
                        {
                            MessageBox.Show("Could not find parent feature");
                            selectedFeature = null;
                            return;
                        }
                    }

                    pEditor.StopOperation("Related Objects Deleted");

                    pEditor.StartOperation();
                    //Now we can recreate our related objects
                    if (destEvents != null)
                        destEvents.RelatedObjectCreated(rel, parentObject);
                    
                    if (originEvents != null)
                        originEvents.RelatedObjectCreated(rel, parentObject);
                    pEditor.StopOperation("Recreated related objects");


                    ApplySettingsToNewAnno(rel, parentObject, alignment, theta);

                    //pEditor.StopOperation("Recreate Annotation");

                    mxDoc.ActiveView.PartialRefresh(esriViewDrawPhase.esriViewGeography, selectedFeatureLayer, mxDoc.ActiveView.Extent);
                }
            }
            catch (Exception ex)
            {
                pEditor.AbortOperation();
                MessageBox.Show("Error during execution: " + ex.Message);
            }

            //Clear our selected feature now as we are finished with it and need to prep for next run
            selectedFeatureLayer = null;
            selectedFeature = null;
        }

        /// <summary>
        /// Given a relationship, loops through each related feature and calculates overall offsets, then
        /// applies the collection's attributes to these features based on the annotation class ID.
        /// </summary>
        private void ApplySettingsToNewAnno(IRelationshipClass AnnoRelClass, IObject ParentObject, int alignment, double angle)
        {

            IRelatedObjectClassEvents originEvents = AnnoRelClass.OriginClass.Extension as IRelatedObjectClassEvents;
            IRelatedObjectClassEvents destEvents = AnnoRelClass.DestinationClass.Extension as IRelatedObjectClassEvents;

            if (destEvents != null || originEvents != null)
            {
                //Loop 1: Remove any non-applicable anno.
                ISet relObjs = AnnoRelClass.GetObjectsRelatedToObject(ParentObject);
                List<IObject> eligibleObjects = new List<IObject>();
                for (IObject relObj = relObjs.Next() as IObject; relObj != null; relObj = relObjs.Next() as IObject)
                {
                    //We've seen cases where expressions mess up. Take a note if this happens.
                    bool annoError = false;
                    if (AnnoShapeExists(relObj, out annoError))
                    {
                        eligibleObjects.Add(relObj);
                    }
                }

                //Determine the current rotation and get the anchorpoint which will be used for subsequent angle rotations.
                double currentAngle = 0.0;
                foreach (IObject relObj in eligibleObjects)
                {
                    if (annoClassID == Convert.ToInt32(relObj.get_Value(relObj.Fields.FindField("ANNOTATIONCLASSID"))))
                    {
                        currentAngle = GetCurrentAngle(relObj as IFeature);
                        RotateAnglePoint = GetAnchorPoint(relObj as IFeature, alignment, currentAngle);
                        break;
                    }
                }

                pEditor.StartOperation();
                //Apply an angle rotation to set everything to an effective angle rotation of 0
                foreach (IObject relObj in eligibleObjects)
                {
                    double adjustmentAngleToZero = 0 - currentAngle;
                    ApplyAngleRotation(relObj as IFeature, adjustmentAngleToZero, RotateAnglePoint);
                    relObj.Store();
                }
                pEditor.StopOperation("Set angle to 0");

                pEditor.StartOperation();
                //Apply the horizontal alignment to all features
                foreach (IObject relObj in eligibleObjects)
                {
                    ApplyHorizontalAlignment(relObj, alignment);
                }
                pEditor.StopOperation("Set horizontal alignment");

                pEditor.StartOperation();
                //Ajust annotations to offset them to still be aligned properly based on their horizontal alignment
                foreach (IObject relObj in eligibleObjects)
                {
                    if (annoClassID == Convert.ToInt32(relObj.get_Value(relObj.Fields.FindField("ANNOTATIONCLASSID"))))
                    {
                        IPoint point1 = GetAnchorPoint(relObj as IFeature, alignment, 0);
                        //Adjust offsets to ensure the annotations are still lining up after the alignment may have changed
                        foreach (IObject relObj2 in eligibleObjects)
                        {
                            if (annoClassID != Convert.ToInt32(relObj2.get_Value(relObj2.Fields.FindField("ANNOTATIONCLASSID"))))
                            {
                                IPoint point2 = GetAnchorPoint(relObj2 as IFeature, alignment, 0);
                                double xOffset = 0.0;
                                xOffset = point1.X - point2.X;

                                double yOffset = 0.0;

                                UpdateOffsets(relObj2, xOffset, yOffset);
                            }
                        }
                    }
                }
                pEditor.StopOperation("Apply offsets");

                pEditor.StartOperation();
                //Loop 2: Apply horizontal index, get anchor point on new anno, apply angle rotation, find offsets.
                foreach (IObject relObj in eligibleObjects)
                    ApplyAngleRotation(relObj, alignment, angle);

                pEditor.StopOperation("Apply angle rotation");

                foreach (IObject relObj in eligibleObjects)
                {
                    if (annoClassID == Convert.ToInt32(relObj.get_Value(relObj.Fields.FindField("ANNOTATIONCLASSID"))))
                    {
                        FindOffsets(relObj as IFeature, alignment, angle);
                        break;
                    }
                }

                pEditor.StartOperation();
                //Loop 3: Apply the offsets.
                foreach (IObject relObj in eligibleObjects)
                    UpdateOffsets(relObj);
                pEditor.StopOperation("Apply final offsets");
            }
        }

        /// <summary>
        /// Update offsets based on the current geometry.
        /// </summary>
        /// <param name="obj">
        ///     The current object to update.
        /// </param>
        private void UpdateOffsets(IObject obj)
        {
            if (!(obj is IAnnotationFeature2))
                return;

            IAnnotationFeature2 annoFeat2 = obj as IAnnotationFeature2;
            IFeature feat = annoFeat2 as IFeature;

            if (feat.Shape != null)
            {
                SetOffsetForPoints(feat, XOffset, YOffset);
                feat.Store();
            }
        }

        /// <summary>
        /// Update offsets based on the current geometry.
        /// </summary>
        /// <param name="obj">
        ///     The current object to update.
        /// </param>
        private void UpdateOffsets(IObject obj, double xOffset, double yOffset)
        {
            if (!(obj is IAnnotationFeature2))
                return;

            IAnnotationFeature2 annoFeat2 = obj as IAnnotationFeature2;
            IFeature feat = annoFeat2 as IFeature;

            if (feat.Shape != null)
            {
                SetOffsetForPoints(feat, xOffset, yOffset);
                feat.Store();
            }
        }

        /// <summary>
        /// Apply offsets to the given feature
        /// </summary>
        /// <param name="feature">IFeature to apply offsets to</param>
        /// <param name="offsetX">X Offset</param>
        /// <param name="offsetY">Y Offset</param>
        private void SetOffsetForPoints(IFeature feature, double offsetX, double offsetY)
        {
            IAnnotationFeature2 annoFeat2 = feature as IAnnotationFeature2;
            IElement annoElement = annoFeat2.Annotation;

            IGeometry annoGeom = annoElement.Geometry;

            ITransform2D transform2D = annoGeom as ITransform2D;
            transform2D.Move(offsetX, offsetY);

            annoElement.Geometry = transform2D as IGeometry;

            ITextElement textElement = annoElement as ITextElement;

            annoFeat2.Annotation = textElement as IElement;
        }

        /// <summary>
        /// Determines the current angle of the given feature
        /// </summary>
        /// <param name="feat">IFeature to calculate current angle</param>
        /// <returns></returns>
        private double GetCurrentAngle(IFeature feat)
        {
            double currentAngle = 0;

            try
            {
                if (feat.Shape != null)
                {
                    int maxXIndex = -1;
                    int maxYIndex = -1;
                    int minXIndex = -1;
                    int minYIndex = -1;
                    IPoint maxXPoint = GetMaxXPoint(feat, ref maxXIndex);
                    IPoint maxYPoint = GetMaxYPoint(feat, ref maxYIndex);
                    IPoint minXPoint = GetMinXPoint(feat, ref minXIndex);
                    IPoint minYPoint = GetMinYPoint(feat, ref minYIndex);

                    if (!(maxXIndex == maxYIndex || maxXIndex == minXIndex || maxXIndex == minYIndex ||
                        maxYIndex == minXIndex || maxYIndex == minYIndex ||
                        minXIndex == minYIndex))
                    {
                        double dx = maxXPoint.X - minYPoint.X;
                        double dy = maxXPoint.Y - minYPoint.Y;

                        double theta = 0.0;
                        if (dx != 0)
                        {
                            double tan = dy / dx;
                            theta = Math.Atan(tan);
                            theta *= 180 / Math.PI;
                        }
                        else
                        {
                            theta = 90;
                        }

                        currentAngle = theta;
                    }

                    //Quadrant 1
                    if (minYIndex == 0)
                    {
                        //No adjustment necessary
                    }
                    //Quadrant 2
                    else if (maxXIndex == 0)
                    {
                        currentAngle += 90;
                    }
                    //Quadrant 3
                    else if (maxYIndex == 0)
                    {
                        currentAngle += 180;
                    }
                    //Quadrant 4
                    else if (minXIndex == 0)
                    {
                        currentAngle += 270;
                    }
                }
            }
            catch { }
            return currentAngle;
        }

        /// <summary>
        /// Apply the specified angle to the given annotation based on the alignment of the feature to determine anchor point
        /// </summary>
        /// <param name="newAnno">Annotation to apply rotation to</param>
        /// <param name="alignment">Alignment of the annotation (used to calculate anchor point)</param>
        /// <param name="angle">Angle to rotate</param>
        private void ApplyAngleRotation(IObject newAnno, int alignment, double angle)
        {
            IAnnotationFeature2 annoFeat2 = newAnno as IAnnotationFeature2;
            IElement annoElement = annoFeat2.Annotation;
            IFeature feat = annoFeat2 as IFeature;

            if (feat.Shape != null)
            {
                double currentAngle = 0;
                currentAngle = GetCurrentAngle(feat);

                double adjustmentAngle = angle;
                //Adjust the angle if there was a current angle found other than 0.
                if (currentAngle != 0)
                {
                    adjustmentAngle = angle - currentAngle;
                }

                ApplyAngleRotation(feat, adjustmentAngle, RotateAnglePoint);

                feat.Store();
            }
        }

        /// <summary>
        /// Applies the horizontal alignment to the specified feature
        /// </summary>
        /// <param name="newAnno">Annotation to apply alignment to</param>
        /// <param name="alignment">Alignment to assign</param>
        private void ApplyHorizontalAlignment(IObject newAnno, int alignment)
        {
            IAnnotationFeature2 annoFeat2 = newAnno as IAnnotationFeature2;
            IElement annoElement = annoFeat2.Annotation;
            IFeature feat = annoFeat2 as IFeature;

            IElementProperties3 elementProperties = annoElement as IElementProperties3;
            if (alignment == 0)
            {
                elementProperties.AnchorPoint = esriAnchorPointEnum.esriTopLeftCorner;
            }
            else if (alignment == 1)
            {
                elementProperties.AnchorPoint = esriAnchorPointEnum.esriTopMidPoint;
            }
            else if (alignment == 2)
            {
                elementProperties.AnchorPoint = esriAnchorPointEnum.esriTopRightCorner;
            }

            feat.set_Value(feat.Fields.FindField("HORIZONTALALIGNMENT"), alignment);
            feat.Store();
        }

        /// <summary>
        /// Applies the specified angle to the given feature using the specified anchor point
        /// </summary>
        /// <param name="feature">IFeature to apply angle rotation to</param>
        /// <param name="angle">Angle to rotate by</param>
        /// <param name="aboutPoint">IPoint to rotate about</param>
        private void ApplyAngleRotation(IFeature feature, double angle, IPoint aboutPoint)
        {
            IAnnotationFeature2 annoFeat2 = feature as IAnnotationFeature2;
            IElement annoElement = annoFeat2.Annotation;

            IGeometry annoGeom = annoElement.Geometry;

            ITransform2D transform2D = annoGeom as ITransform2D;
            double radianAngle = (Math.PI / 180.0) * angle;
            transform2D.Rotate(aboutPoint, radianAngle);

            annoElement.Geometry = transform2D as IGeometry;

            ITextElement textElement = annoElement as ITextElement;

            annoFeat2.Annotation = textElement as IElement;
        }



        /// <summary>
        /// Find the offsets from the anchor point of the given feature to the new location to be moved to
        /// </summary>
        /// <param name="feat">IFeature to get current anchor point from</param>
        /// <param name="alignment">Horizontal alignment of the feature</param>
        /// <param name="angle">Angle of rotation of the feature</param>
        private void FindOffsets(IFeature feat, int alignment, double angle)
        {
            if (!(feat is IAnnotationFeature2))
                return;

            IAnnotationFeature2 annoFeat = feat as IAnnotationFeature2;
            IGeometry currentGeometry = annoFeat.Annotation.Geometry;

            IPoint point = GetAnchorPoint(feat, alignment, angle);
            double newX = point.X;
            double newY = point.Y;

            XOffset = newPlacementPoint.X - newX;
            YOffset = newPlacementPoint.Y - newY;
        }

        /// <summary>
        /// Gets the anchor point of the given feature based on the horizontal alignment and the angle of rotation of the feature
        /// </summary>
        /// <param name="feature">IFeature to get the anchor point for</param>
        /// <param name="HorizontalAlignment">Horizontal alignment of the feature</param>
        /// <param name="angle">Angle of rotation of the feature</param>
        /// <returns></returns>
        private IPoint GetAnchorPoint(IFeature feature, int HorizontalAlignment, double angle)
        {
            int indexHolder = -1;
            if (feature.Shape.GeometryType == esriGeometryType.esriGeometryPolygon)
            {
                IPointCollection pointCollection = feature.Shape as IPointCollection;
                if (HorizontalAlignment == 0 || HorizontalAlignment == 3)
                {
                    //Bottom right quadrant
                    if ((angle > 270 && angle < 360) || (angle > -90 && angle < 0))
                    {
                        return GetMaxYPoint(feature, ref indexHolder);
                    }
                    //Bottom left quadrant
                    else if ((angle > 180 && angle < 270) || (angle > -180 && angle < -90))
                    {
                        return GetMaxXPoint(feature, ref indexHolder);
                    }
                    //Top left quadrant
                    else if ((angle > 90 && angle < 180) || (angle > -270 && angle < -180))
                    {
                        return GetMinYPoint(feature, ref indexHolder);
                    }
                    //Top Right quadrant
                    else if ((angle > 0 && angle < 90) || (angle > -360 && angle < -270))
                    {
                        return GetMinXPoint(feature, ref indexHolder);
                    }
                    else if (angle == 0 || angle == 360 || angle == -360)
                    {
                        IPoint YPoint = GetMaxYPoint(feature, ref indexHolder);
                        IPoint XPoint = GetMinXPoint(feature, ref indexHolder);
                        IPoint newPoint = new PointClass();
                        newPoint.SpatialReference = YPoint.SpatialReference;
                        newPoint.X = XPoint.X;
                        newPoint.Y = YPoint.Y;
                        return newPoint;
                    }
                    else if (angle == 90 || angle == -270)
                    {
                        IPoint YPoint = GetMinYPoint(feature, ref indexHolder);
                        IPoint XPoint = GetMinXPoint(feature, ref indexHolder);
                        IPoint newPoint = new PointClass();
                        newPoint.SpatialReference = YPoint.SpatialReference;
                        newPoint.X = XPoint.X;
                        newPoint.Y = YPoint.Y;
                        return newPoint;
                    }
                    else if (angle == 180 || angle == -180)
                    {
                        IPoint YPoint = GetMinYPoint(feature, ref indexHolder);
                        IPoint XPoint = GetMaxXPoint(feature, ref indexHolder);
                        IPoint newPoint = new PointClass();
                        newPoint.SpatialReference = YPoint.SpatialReference;
                        newPoint.X = XPoint.X;
                        newPoint.Y = YPoint.Y;
                        return newPoint;
                    }
                    else if (angle == 270 || angle == -90)
                    {
                        IPoint YPoint = GetMaxYPoint(feature, ref indexHolder);
                        IPoint XPoint = GetMaxXPoint(feature, ref indexHolder);
                        IPoint newPoint = new PointClass();
                        newPoint.SpatialReference = YPoint.SpatialReference;
                        newPoint.X = XPoint.X;
                        newPoint.Y = YPoint.Y;
                        return newPoint;
                    }
                }
                //Center point
                else if (HorizontalAlignment == 1)
                {
                    IPoint YMaxPoint = GetMaxYPoint(feature, ref indexHolder);
                    IPoint XMaxPoint = GetMaxXPoint(feature, ref indexHolder);
                    IPoint YMinPoint = GetMinYPoint(feature, ref indexHolder);
                    IPoint XMinPoint = GetMinXPoint(feature, ref indexHolder);
                    IPoint newPoint = new PointClass();
                    newPoint.SpatialReference = YMaxPoint.SpatialReference;
                    newPoint.X = (XMinPoint.X + ((XMaxPoint.X - XMinPoint.X) / 2));
                    newPoint.Y = (YMinPoint.Y + ((YMaxPoint.Y - YMinPoint.Y) / 2));
                    return newPoint;
                }
                //Right alignment
                else if (HorizontalAlignment == 2)
                {
                    //Bottom right quadrant
                    if ((angle > 270 && angle < 360) || (angle > -90 && angle < 0))
                    {
                        return GetMaxXPoint(feature, ref indexHolder);
                    }
                    //Bottom left quadrant
                    else if ((angle > 180 && angle < 270) || (angle > -180 && angle < -90))
                    {
                        return GetMinYPoint(feature, ref indexHolder);
                    }
                    //Top left quadrant
                    else if ((angle > 90 && angle < 180) || (angle > -270 && angle < -180))
                    {
                        return GetMinXPoint(feature, ref indexHolder);
                    }
                    //Top Right quadrant
                    else if ((angle > 0 && angle < 90) || (angle > -360 && angle < -270))
                    {
                        return GetMaxYPoint(feature, ref indexHolder);
                    }
                    else if (angle == 0 || angle == 360 || angle == -360)
                    {
                        IPoint YPoint = GetMaxYPoint(feature, ref indexHolder);
                        IPoint XPoint = GetMaxXPoint(feature, ref indexHolder);
                        IPoint newPoint = new PointClass();
                        newPoint.SpatialReference = YPoint.SpatialReference;
                        newPoint.X = XPoint.X;
                        newPoint.Y = YPoint.Y;
                        return newPoint;
                    }
                    else if (angle == 90 || angle == -270)
                    {
                        IPoint YPoint = GetMaxYPoint(feature, ref indexHolder);
                        IPoint XPoint = GetMinXPoint(feature, ref indexHolder);
                        IPoint newPoint = new PointClass();
                        newPoint.SpatialReference = YPoint.SpatialReference;
                        newPoint.X = XPoint.X;
                        newPoint.Y = YPoint.Y;
                        return newPoint;
                    }
                    else if (angle == 180 || angle == -180)
                    {
                        IPoint YPoint = GetMinYPoint(feature, ref indexHolder);
                        IPoint XPoint = GetMinXPoint(feature, ref indexHolder);
                        IPoint newPoint = new PointClass();
                        newPoint.SpatialReference = YPoint.SpatialReference;
                        newPoint.X = XPoint.X;
                        newPoint.Y = YPoint.Y;
                        return newPoint;
                    }
                    else if (angle == 270 || angle == -90)
                    {
                        IPoint YPoint = GetMinYPoint(feature, ref indexHolder);
                        IPoint XPoint = GetMaxXPoint(feature, ref indexHolder);
                        IPoint newPoint = new PointClass();
                        newPoint.SpatialReference = YPoint.SpatialReference;
                        newPoint.X = XPoint.X;
                        newPoint.Y = YPoint.Y;
                        return newPoint;
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Returns an IPoint which holds the maximum Y coordinate
        /// </summary>
        /// <param name="feature">IFeature to get the max Y coordinate</param>
        /// <param name="maxYIndex">Index from the IPointCollection of the polygon</param>
        /// <returns></returns>
        private IPoint GetMaxYPoint(IFeature feature, ref int maxYIndex)
        {
            int maxYPointIndex = 0;
            double maxY = 0.0;

            if (feature.Shape.GeometryType == esriGeometryType.esriGeometryPolygon)
            {
                IPointCollection pointCollection = feature.Shape as IPointCollection;
                for (int i = 0; i < pointCollection.PointCount; i++)
                {
                    IPoint point = pointCollection.get_Point(i);
                    double y = point.Y;

                    if (i == 0)
                    {
                        maxY = y;
                    }
                    else if (y > maxY)
                    {
                        maxY = y;
                        maxYPointIndex = i;
                    }
                }
                maxYIndex = maxYPointIndex;
                return pointCollection.get_Point(maxYPointIndex);
            }
            return null;
        }

        /// <summary>
        /// Returns an IPoint which holds the minimum Y coordinate
        /// </summary>
        /// <param name="feature">IFeature to get the min Y coordinate</param>
        /// <param name="maxYIndex">Index from the IPointCollection of the polygon</param>
        /// <returns></returns>
        private IPoint GetMinYPoint(IFeature feature, ref int minYIndex)
        {
            int minYPointIndex = 0;
            double minY = 0.0;

            if (feature.Shape.GeometryType == esriGeometryType.esriGeometryPolygon)
            {
                IPointCollection pointCollection = feature.Shape as IPointCollection;
                for (int i = 0; i < pointCollection.PointCount; i++)
                {
                    IPoint point = pointCollection.get_Point(i);
                    double y = point.Y;

                    if (i == 0)
                    {
                        minY = y;
                    }
                    else if (y < minY)
                    {
                        minY = y;
                        minYPointIndex = i;
                    }
                }
                minYIndex = minYPointIndex;
                return pointCollection.get_Point(minYPointIndex);
            }
            return null;
        }

        /// <summary>
        /// Returns an IPoint which holds the maximum X coordinate
        /// </summary>
        /// <param name="feature">IFeature to get the max X coordinate</param>
        /// <param name="maxYIndex">Index from the IPointCollection of the polygon</param>
        /// <returns></returns>
        private IPoint GetMaxXPoint(IFeature feature, ref int maxXIndex)
        {
            int maxXPointIndex = 0;
            double maxX = 0.0;

            if (feature.Shape.GeometryType == esriGeometryType.esriGeometryPolygon)
            {
                IPointCollection pointCollection = feature.Shape as IPointCollection;
                for (int i = 0; i < pointCollection.PointCount; i++)
                {
                    IPoint point = pointCollection.get_Point(i);
                    double x = point.X;

                    if (i == 0)
                    {
                        maxX = x;
                    }
                    else if (x > maxX)
                    {
                        maxX = x;
                        maxXPointIndex = i;
                    }
                }
                maxXIndex = maxXPointIndex;
                return pointCollection.get_Point(maxXPointIndex);
            }
            return null;
        }

        /// <summary>
        /// Returns an IPoint which holds the minimum X coordinate
        /// </summary>
        /// <param name="feature">IFeature to get the min X coordinate</param>
        /// <param name="maxYIndex">Index from the IPointCollection of the polygon</param>
        /// <returns></returns>
        private IPoint GetMinXPoint(IFeature feature, ref int minXPoint)
        {
            int minXPointIndex = 0;
            double minX = 0.0;

            if (feature.Shape.GeometryType == esriGeometryType.esriGeometryPolygon)
            {
                IPointCollection pointCollection = feature.Shape as IPointCollection;
                for (int i = 0; i < pointCollection.PointCount; i++)
                {
                    IPoint point = pointCollection.get_Point(i);
                    double x = point.X;

                    if (i == 0)
                    {
                        minX = x;
                    }
                    else if (x < minX)
                    {
                        minX = x;
                        minXPointIndex = i;
                    }
                }
                minXPoint = minXPointIndex;
                return pointCollection.get_Point(minXPointIndex);
            }
            return null;
        }

        /// <summary>
        /// Determines whether or not an anno shape exists, determining whether or not it can be manipulated.
        /// </summary>
        /// <param name="annoObj">The anno object to check.</param>
        /// <param name="annoObj">Will return <c>true</c> if the feature shape is not null, but the annotation element is.</param>
        /// <returns>Boolean indicating whether or not a shape is truly present.</returns>
        private bool AnnoShapeExists(IObject annoObj, out bool nullAnnoElement)
        {
            nullAnnoElement = false;

            if (!(annoObj is IAnnotationFeature2))
                return false;

            if (!(annoObj is IFeature))
                return false;

            if ((annoObj as IFeature).Shape == null)
                return false;

            if ((annoObj as IAnnotationFeature2).Annotation == null)
            {
                nullAnnoElement = true;
                return false;
            }

            return true;
        }

        /// <summary>
        /// Method that will draw the rubber band for the user to specify the angle
        /// </summary>
        /// <param name="activeView">IActiveView of the focusmap</param>
        /// <returns></returns>
        private IPolyline GetPolylineFromMouseClicks(IActiveView activeView)
        {
            IScreenDisplay screenDisplay = activeView.ScreenDisplay;

            IRubberBand rubberBand = new RubberLineClass();
            IGeometry geometry = rubberBand.TrackNew(screenDisplay, null);

            IPolyline polyline = (IPolyline)geometry;

            return polyline;
        }



        public override void OnMouseMove(int Button, int Shift, int X, int Y)
        {
        }



        public override void OnMouseUp(int Button, int Shift, int X, int Y)
        {
            if (selectedFeature == null)
            {
                if (pEditor == null)
                {
                    UID pID = new UIDClass();
                    pID.Value = "esriEditor.Editor";
                    pEditor = m_application.FindExtensionByCLSID(pID) as IEditor;
                }
                if (pEditor.EditState == esriEditState.esriStateNotEditing)
                {
                    MessageBox.Show("Must be editing");
                    return;
                }

                if (AnnoFeatLayers.Count < 1)
                {
                    //Get all of the IFeatureLayers
                    ESRI.ArcGIS.esriSystem.UID uid = new ESRI.ArcGIS.esriSystem.UIDClass();
                    uid.Value = "{40A9E885-5533-11D0-98BE-00805F7CED21}";
                    IEnumLayer featLayers = mxDoc.FocusMap.get_Layers(uid);
                    IFeatureLayer featLayer = featLayers.Next() as IFeatureLayer;
                    while (featLayer != null)
                    {
                        if (featLayer.FeatureClass.FeatureType == esriFeatureType.esriFTAnnotation)
                        {
                            AnnoFeatLayers.Add(featLayer);
                        }
                        featLayer = featLayers.Next() as IFeatureLayer;
                    }
                }

                ESRI.ArcGIS.ArcMapUI.IMxApplication mxApp = m_application as IMxApplication;
                ESRI.ArcGIS.Display.IAppDisplay appDisplay;
                IScreenDisplay screenDisplay;
                IDisplay display;
                IDisplayTransformation transform;

                appDisplay = mxApp.Display;

                if (appDisplay == null)
                    return;

                screenDisplay = appDisplay.FocusScreen;
                display = screenDisplay;

                if (display == null)
                    return;

                transform = display.DisplayTransformation;

                if (transform == null)
                    return;


                IPoint geom = transform.ToMapPoint(X, Y);

                foreach (IFeatureLayer featLayer in AnnoFeatLayers)
                {
                    IIdentify identify = featLayer as IIdentify;
                    IArray identified = identify.Identify(geom);
                    if (identified != null && identified.Count > 0)
                    {
                        IFeatureIdentifyObj identifiedObject = identified.get_Element(0) as IFeatureIdentifyObj;
                        IRowIdentifyObject identifyObject = identifiedObject as IRowIdentifyObject;
                        int oid = identifyObject.Row.OID;
                        selectedFeature = featLayer.FeatureClass.GetFeature(oid);
                        if (selectedFeature != null)
                        {
                            selectedFeatureLayer = featLayer;
                            IIdentifyObj flashObj = identifiedObject as IIdentifyObj;
                            flashObj.Flash(mxDoc.ActiveView.ScreenDisplay);
                            break;
                        }
                    }
                }
            }
            else
            {

                //Clear our selected feature now as we are finished with it and need to prep for next run
                selectedFeatureLayer = null;
                selectedFeature = null;
            }
        }
        #endregion
    }
}
