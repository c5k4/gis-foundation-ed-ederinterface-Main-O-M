using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.ArcMapUI;
using ESRI.ArcGIS.Geometry;
using System.Text.RegularExpressions;
using ESRI.ArcGIS.Editor;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Framework;
using ESRI.ArcGIS.Display;

namespace PGE.BatchApplication.ReplaceAnnoation
{
    public partial class AdjustAnnoOrientationFrom : Form
    {
        private List<IFeatureLayer> AnnoFeatLayers = new List<IFeatureLayer>();
        private IMxDocument mxDoc = null;
        private IApplication m_application;
        private int currentMinAngle = 97;
        private int currentMaxAngle = 263;
        IEditor pEditor = null;

        public AdjustAnnoOrientationFrom(IMxDocument doc, IApplication app)
        {
            InitializeComponent();

            m_application = app;
            mxDoc = doc;

            ContextMenuStrip menuStrip = new ContextMenuStrip();
            menuStrip.ItemClicked += new ToolStripItemClickedEventHandler(menuStrip_ItemClicked);
            menuStrip.Items.Add("Zoom To");
            resultsList.ContextMenuStrip = menuStrip;

            ContextMenuStrip failedMenuStrip = new ContextMenuStrip();
            failedMenuStrip.ItemClicked += new ToolStripItemClickedEventHandler(failedMenuStrip_ItemClicked);
            failedMenuStrip.Items.Add("Zoom To");
            failedResultList.ContextMenuStrip = failedMenuStrip;
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
            base.OnClosing(e);
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

                if (e.ClickedItem.ToString() == "Zoom To") { alignment = 1; }

                //Check our angles
                try
                {
                    currentMinAngle = Int32.Parse(txtMinDegrees.Text);
                    currentMaxAngle = Int32.Parse(txtMaxDegrees.Text);
                }
                catch
                {
                    MessageBox.Show("Invalid angles specified");
                    return;
                }

                string selectedItem = resultsList.SelectedItem.ToString();
                string[] selectedItemArray = Regex.Split(selectedItem, "-");

                string featLayer = selectedItemArray[0];
                string featOID = selectedItemArray[1];

                foreach (FeatureLayer featureLayer in AnnoFeatLayers)
                {
                    if (featureLayer.FeatureClass.AliasName == featLayer)
                    {
                        IFeature feature = featureLayer.FeatureClass.GetFeature(Int32.Parse(featOID));
                        IEnvelope envelope = feature.Extent;
                        envelope.XMax += 100;
                        envelope.XMin -= 100;
                        envelope.YMax += 100;
                        envelope.YMin -= 100;
                        mxDoc.ActiveView.Extent = envelope;
                        mxDoc.ActiveView.Refresh();
                    }
                }
            }
            catch (Exception ex)
            {

            }
                
        }

        /// <summary>
        /// Event to handle when the user choses a horizontal alignment.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void failedMenuStrip_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            try
            {
                int alignment = 0;

                if (e.ClickedItem.ToString() == "Zoom To") { alignment = 1; }

                //Check our angles
                try
                {
                    currentMinAngle = Int32.Parse(txtMinDegrees.Text);
                    currentMaxAngle = Int32.Parse(txtMaxDegrees.Text);
                }
                catch
                {
                    MessageBox.Show("Invalid angles specified");
                    return;
                }

                string selectedItem = failedResultList.SelectedItem.ToString();
                string[] selectedItemArray = Regex.Split(selectedItem, "-");

                string featLayer = selectedItemArray[0];
                string featOID = selectedItemArray[1];

                foreach (FeatureLayer featureLayer in AnnoFeatLayers)
                {
                    if (featureLayer.FeatureClass.AliasName == featLayer)
                    {
                        IFeature feature = featureLayer.FeatureClass.GetFeature(Int32.Parse(featOID));
                        IEnvelope envelope = feature.Extent;
                        envelope.XMax += 100;
                        envelope.XMin -= 100;
                        envelope.YMax += 100;
                        envelope.YMin -= 100;
                        mxDoc.ActiveView.Extent = envelope;
                        mxDoc.ActiveView.Refresh();
                    }
                }
            }
            catch (Exception ex)
            {

            }

        }

        private void btnFixExtent_Click(object sender, EventArgs e)
        {
            if (!IsEditing()) { return; }

            //Check our angles
            try
            {
                currentMinAngle = Int32.Parse(txtMinDegrees.Text);
                currentMaxAngle = Int32.Parse(txtMaxDegrees.Text);
            }
            catch
            {
                MessageBox.Show("Invalid angles specified");
                return;
            }

            GetAnnoLayers();

            IEnvelope currentExtent = mxDoc.ActiveView.Extent;
            RecreateAnno(currentExtent);
        }

        private bool IsEditing()
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
                return false;
            }
            else
            {
                return true;
            }
        }

        private void RecreateAnno(IEnvelope selectedEnvelope)
        {
            #region Get annotation feature layers for lines
            if (AnnoFeatLayers.Count < 1)
            {
                //Get all of the IFeatureLayers
                ESRI.ArcGIS.esriSystem.UID uid = new ESRI.ArcGIS.esriSystem.UIDClass();
                uid.Value = "{40A9E885-5533-11D0-98BE-00805F7CED21}";
                IEnumLayer featLayers = mxDoc.FocusMap.get_Layers(uid);
                IFeatureLayer featLayer = featLayers.Next() as IFeatureLayer;
                while (featLayer != null)
                {
                    try
                    {
                        if (featLayer.FeatureClass.FeatureType == esriFeatureType.esriFTAnnotation)
                        {
                            IEnumRelationshipClass relClasses = featLayer.FeatureClass.get_RelationshipClasses(esriRelRole.esriRelRoleAny);
                            relClasses.Reset();
                            IRelationshipClass relClass = relClasses.Next();
                            if (relClass.OriginClass is IFeatureClass)
                            {
                                IFeatureClass featClass = relClass.OriginClass as IFeatureClass;
                                if (featClass.FeatureType != esriFeatureType.esriFTComplexEdge &&
                                    featClass.FeatureType != esriFeatureType.esriFTSimpleEdge)
                                {
                                    featLayer = featLayers.Next() as IFeatureLayer;
                                    continue;
                                }
                                else
                                {
                                    AnnoFeatLayers.Add(featLayer);
                                }
                            }
                        }
                    }
                    catch (Exception e) { }
                    featLayer = featLayers.Next() as IFeatureLayer;
                }
            }
            #endregion

            #region Get annotation under mouse click

            if (selectedEnvelope == null) { return; }

            Dictionary<IFeatureClass, List<int>> selectedFeatures = new Dictionary<IFeatureClass, List<int>>();
            List<IFeatureLayer> selectedFeatureLayers = new List<IFeatureLayer>();
            foreach (IFeatureLayer featLayer in AnnoFeatLayers)
            {
                IIdentify identify = featLayer as IIdentify;
                IArray identified = identify.Identify(selectedEnvelope);
                if (identified != null && identified.Count > 0)
                {
                    selectedFeatureLayers.Add(featLayer);

                    if (!selectedFeatures.ContainsKey(featLayer.FeatureClass))
                    {
                        selectedFeatures.Add(featLayer.FeatureClass, new List<int>());
                    }

                    for (int i = 0; i < identified.Count; i++)
                    {
                        IFeatureIdentifyObj identifiedObject = identified.get_Element(i) as IFeatureIdentifyObj;
                        IRowIdentifyObject identifyObject = identifiedObject as IRowIdentifyObject;
                        int oid = identifyObject.Row.OID;
                        if (!selectedFeatures[featLayer.FeatureClass].Contains(oid)) { selectedFeatures[featLayer.FeatureClass].Add(oid); }
                    }
                }
            }
            #endregion

            if (selectedFeatures.Count <= 0) { return; }

            bool inOperation = false;

            try
            {
                pEditor.StartOperation();
                inOperation = true;
                foreach (KeyValuePair<IFeatureClass, List<int>> kvp in selectedFeatures)
                {
                    IFeatureClass featClass = kvp.Key;

                    foreach (int oid in kvp.Value)
                    {
                        IFeature selectedFeature = featClass.GetFeature(oid);

                        if (selectedFeature == null) { continue; }

                        //Now we can perform the work to flip the anno if necessary
                        double currentAngle = GetCurrentAngle(selectedFeature);

                        //Only need to flip it if it is appearing to be upsides down.
                        if ((currentAngle > currentMinAngle && currentAngle < currentMaxAngle))
                        {
                            IPoint orignalCenterPoint = getCenterPoint(selectedFeature);
                            inOperation = true;
                            RecreateCurrentAnno(selectedFeature, currentAngle);
                            if (resultsList.Items.Contains(featClass.AliasName + "-" + oid)) 
                            { 
                                resultsList.Items.Remove(featClass.AliasName + "-" + oid);
                                lblResultsCount.Text = resultsList.Items.Count + " Features Found";
                            }
                        }
                    }
                }
                pEditor.StopOperation("Adjust Annotation Orientation");
            }
            catch (Exception e)
            {
                if (inOperation) { pEditor.AbortOperation(); }
            }

            foreach (IFeatureLayer featLayer in selectedFeatureLayers)
            {
                mxDoc.ActiveView.PartialRefresh(esriViewDrawPhase.esriViewGeography, featLayer, mxDoc.ActiveView.Extent);
                mxDoc.ActiveView.PartialRefresh(esriViewDrawPhase.esriViewGeoSelection, featLayer, mxDoc.ActiveView.Extent);
            }
        }

        private void RecreateAnno()
        {
            #region Get annotation feature layers for lines
            if (AnnoFeatLayers.Count < 1)
            {
                //Get all of the IFeatureLayers
                ESRI.ArcGIS.esriSystem.UID uid = new ESRI.ArcGIS.esriSystem.UIDClass();
                uid.Value = "{40A9E885-5533-11D0-98BE-00805F7CED21}";
                IEnumLayer featLayers = mxDoc.FocusMap.get_Layers(uid);
                IFeatureLayer featLayer = featLayers.Next() as IFeatureLayer;
                while (featLayer != null)
                {
                    try
                    {
                        if (featLayer.FeatureClass.FeatureType == esriFeatureType.esriFTAnnotation)
                        {
                            IEnumRelationshipClass relClasses = featLayer.FeatureClass.get_RelationshipClasses(esriRelRole.esriRelRoleAny);
                            relClasses.Reset();
                            IRelationshipClass relClass = relClasses.Next();
                            if (relClass.OriginClass is IFeatureClass)
                            {
                                IFeatureClass featClass = relClass.OriginClass as IFeatureClass;
                                if (featClass.FeatureType != esriFeatureType.esriFTComplexEdge &&
                                    featClass.FeatureType != esriFeatureType.esriFTSimpleEdge)
                                {
                                    featLayer = featLayers.Next() as IFeatureLayer;
                                    continue;
                                }
                                else
                                {
                                    AnnoFeatLayers.Add(featLayer);
                                }
                            }
                        }
                    }
                    catch (Exception e) { }
                    featLayer = featLayers.Next() as IFeatureLayer;
                }
            }
            #endregion

            bool inOperation = false;

            try
            {
                pEditor.StartOperation();
                inOperation = true;

                List<string> copyOfItems = new List<string>();
                foreach (string item in resultsList.Items)
                {
                    copyOfItems.Add(item);
                }

                int featuresProcessed = 0;

                foreach (string item in copyOfItems)
                {
                    string[] selectedItemArray = Regex.Split(item, "-");

                    string featLayer = selectedItemArray[0];
                    string featOID = selectedItemArray[1];

                    foreach (FeatureLayer featureLayer in AnnoFeatLayers)
                    {
                        if (featureLayer.FeatureClass.AliasName == featLayer)
                        {
                            IFeature selectedFeature = featureLayer.FeatureClass.GetFeature(Int32.Parse(featOID));

                            if (selectedFeature == null) { continue; }

                            //Now we can perform the work to flip the anno if necessary
                            double currentAngle = GetCurrentAngle(selectedFeature);

                            //Only need to flip it if it is appearing to be upsides down.  These min and max angles are
                            //specified by the user.
                            if ((currentAngle > currentMinAngle && currentAngle < currentMaxAngle))
                            {
                                IPoint orignalCenterPoint = getCenterPoint(selectedFeature);
                                RecreateCurrentAnno(selectedFeature, currentAngle);
                                if (resultsList.Items.Contains(featLayer + "-" + featOID)) 
                                { 
                                    resultsList.Items.Remove(featLayer + "-" + featOID);
                                    lblResultsCount.Text = resultsList.Items.Count + " Features Found";
                                }
                                featuresProcessed++;
                                if ((featuresProcessed % 10) == 0)
                                {
                                    int currentProgress = Int32.Parse(Math.Floor(((((double)featuresProcessed) / ((double)copyOfItems.Count)) * 100.0)).ToString());
                                    progressBar.Value = currentProgress;
                                    lblAnalyze.Text = "Repairing Features: " + currentProgress + "%";
                                }
                                Application.DoEvents();
                            }
                        }
                    }
                }

                progressBar.Value = 100;
                lblAnalyze.Text = "";

                pEditor.StopOperation("Adjust Annotation Orientation");
            }
            catch (Exception e)
            {
                if (inOperation) { pEditor.AbortOperation(); }
                MessageBox.Show("Failed to fix all annotation features. Message: " + e.Message);
            }

            mxDoc.ActiveView.Refresh();
        }

        private void RecreateCurrentAnno(IFeature feat, double origAngle)
        {
            IAnnotationFeature2 annoFeat = feat as IAnnotationFeature2;
            if (annoFeat == null) { return; }

            int origAnnoClassID = annoFeat.AnnotationClassID;
            IPoint initialCenterPoint = getCenterPoint(feat);
            Dictionary<int, List<int>> initialRelatedObjects = new Dictionary<int, List<int>>();

            IRelatedObjectClassEvents originEvents = null;
            IRelatedObjectClassEvents destEvents = null;
            IObject parentObject = null;
            IEnumRelationshipClass relClasses = feat.Class.get_RelationshipClasses(esriRelRole.esriRelRoleAny);
            IRelationshipClass rel = relClasses.Next();
            //Loop through and delete.
            if (rel != null)
            {
                //Cast as IRelatedObjectClassEvents to ensure that the relationship can be recreated.
                originEvents = rel.OriginClass.Extension as IRelatedObjectClassEvents;
                destEvents = rel.DestinationClass.Extension as IRelatedObjectClassEvents;

                if (destEvents != null || originEvents != null)
                {
                    ISet relatedFeatures = rel.GetObjectsRelatedToObject(feat);
                    parentObject = relatedFeatures.Next() as IObject;
                    if (parentObject != null)
                    {
                        ISet relObjs = rel.GetObjectsRelatedToObject(parentObject);
                        for (IObject relObj = relObjs.Next() as IObject; relObj != null; relObj = relObjs.Next() as IObject)
                        {
                            try
                            {
                                if (relObj is IAnnotationFeature2)
                                {
                                    IAnnotationFeature2 tempAnnoFeat2 = (IAnnotationFeature2)relObj;
                                    if (!initialRelatedObjects.ContainsKey(tempAnnoFeat2.AnnotationClassID))
                                    {
                                        initialRelatedObjects.Add(tempAnnoFeat2.AnnotationClassID, new List<int>());
                                    }
                                    initialRelatedObjects[tempAnnoFeat2.AnnotationClassID].Add(relObj.OID);
                                }
                                if (relObj.OID == feat.OID)
                                {
                                    relObj.Delete();
                                }
                            }
                            catch (Exception e2)
                            {

                            }
                        }
                    }
                    else
                    {
                        MessageBox.Show("Could not find parent feature");
                        return;
                    }
                }

                //Now we can recreate our related object
                if (destEvents != null) { destEvents.RelatedObjectCreated(rel, parentObject); }

                if (originEvents != null) { originEvents.RelatedObjectCreated(rel, parentObject); }

                //Check if there was anything extra created
                if (parentObject != null)
                {
                    ISet relObjs = rel.GetObjectsRelatedToObject(parentObject);
                    for (IObject relObj = relObjs.Next() as IObject; relObj != null; relObj = relObjs.Next() as IObject)
                    {
                        try
                        {
                            if (relObj is IAnnotationFeature2)
                            {
                                IAnnotationFeature2 tempAnnoFeat2 = (IAnnotationFeature2)relObj;
                                if (tempAnnoFeat2.AnnotationClassID == origAnnoClassID)
                                {
                                    //It is possible there are multiple annotation feature with the same classID
                                    //We only want to adjust the new one.
                                    if (initialRelatedObjects.ContainsKey(tempAnnoFeat2.AnnotationClassID))
                                    {
                                        if (!initialRelatedObjects[tempAnnoFeat2.AnnotationClassID].Contains(relObj.OID))
                                        {
                                            MoveBackToOrigCenterPoint(relObj as IFeature, initialCenterPoint);
                                            relObj.Store();

                                            ApplyAngleRotation(relObj as IFeature, origAngle);
                                            relObj.Store();
                                        }
                                    }
                                }
                                else if (!initialRelatedObjects.ContainsKey(tempAnnoFeat2.AnnotationClassID))
                                {
                                    //This one didn't exist before so let's delete it.
                                    relObj.Delete();
                                }
                                else if (initialRelatedObjects.ContainsKey(tempAnnoFeat2.AnnotationClassID))
                                {
                                    if (!initialRelatedObjects[tempAnnoFeat2.AnnotationClassID].Contains(relObj.OID))
                                    {
                                        //This one didn't exist before so let's delete it.
                                        relObj.Delete();
                                    }
                                }

                            }
                        }
                        catch (Exception e2)
                        {

                        }
                    }
                }
            }
        }


        private void MoveBackToOrigCenterPoint(IFeature feature, IPoint origCenterPoint)
        {
            IPoint centerPoint = getCenterPoint(feature);
            double dx = origCenterPoint.X - centerPoint.X;
            double dy = origCenterPoint.Y - centerPoint.Y;

            IAnnotationFeature2 annoFeat2 = feature as IAnnotationFeature2;
            IElement annoElement = annoFeat2.Annotation;

            IGeometry annoGeom = annoElement.Geometry;

            ITransform2D transform2D = annoGeom as ITransform2D;
            transform2D.Move(dx, dy);

            annoElement.Geometry = transform2D as IGeometry;

            ITextElement textElement = annoElement as ITextElement;

            annoFeat2.Annotation = textElement as IElement;
        }

        /// <summary>
        /// Applies the specified angle to the given feature using the specified anchor point
        /// </summary>
        /// <param name="feature">IFeature to apply angle rotation to</param>
        /// <param name="angle">Angle to rotate by</param>
        /// <param name="aboutPoint">IPoint to rotate about</param>
        private void ApplyAngleRotation(IFeature feature, double origAngle)
        {
            double currentAngle = GetCurrentAngle(feature);
            double adjustmentAngle = origAngle - currentAngle + 180;
            IPoint aboutPoint = getCenterPoint(feature);

            IAnnotationFeature2 annoFeat2 = feature as IAnnotationFeature2;
            IElement annoElement = annoFeat2.Annotation;

            IGeometry annoGeom = annoElement.Geometry;

            ITransform2D transform2D = annoGeom as ITransform2D;
            double radianAngle = (Math.PI / 180.0) * adjustmentAngle;
            transform2D.Rotate(aboutPoint, radianAngle);

            annoElement.Geometry = transform2D as IGeometry;

            ITextElement textElement = annoElement as ITextElement;

            annoFeat2.Annotation = textElement as IElement;
        }

        private IPoint getCenterPoint(IFeature feature)
        {
            //Determine the center point
            int indexRef = -1;
            IPoint YMaxPoint = GetMaxYPoint(feature, ref indexRef);
            IPoint XMaxPoint = GetMaxXPoint(feature, ref indexRef);
            IPoint YMinPoint = GetMinYPoint(feature, ref indexRef);
            IPoint XMinPoint = GetMinXPoint(feature, ref indexRef);
            IPoint centerPoint = new PointClass();
            centerPoint.SpatialReference = YMaxPoint.SpatialReference;
            centerPoint.X = (XMinPoint.X + ((XMaxPoint.X - XMinPoint.X) / 2));
            centerPoint.Y = (YMinPoint.Y + ((YMaxPoint.Y - YMinPoint.Y) / 2));
            return centerPoint;
        }

        private void btnAnalyze_Click(object sender, EventArgs e)
        {
            resultsList.Items.Clear();
            failedResultList.Items.Clear();
            progressBar.Value = 0;

            GetAnnoLayers();

            Dictionary<IFeatureClass, List<int>> failedAnnotationfeatures = new Dictionary<IFeatureClass, List<int>>();
            Dictionary<IFeatureClass, List<int>> affectedAnnotationfeatures = new Dictionary<IFeatureClass, List<int>>();
            foreach (IFeatureLayer featLayer in AnnoFeatLayers)
            {
                int currentProgress = 0;
                lblAnalyze.Text = "Analyzing layer \"" + featLayer.Name + ": " + currentProgress;
                List<int> currentOIDList = new List<int>();
                List<int> currentFailedList = new List<int>();
                if (!affectedAnnotationfeatures.ContainsKey(featLayer.FeatureClass))
                {
                    affectedAnnotationfeatures.Add(featLayer.FeatureClass, null);
                }
                else
                {
                    currentOIDList = affectedAnnotationfeatures[featLayer.FeatureClass];
                }
                if (!failedAnnotationfeatures.ContainsKey(featLayer.FeatureClass))
                {
                    failedAnnotationfeatures.Add(featLayer.FeatureClass, null);
                }
                else
                {
                    currentFailedList = failedAnnotationfeatures[featLayer.FeatureClass];
                }

                IQueryFilter qf = new QueryFilterClass();
                qf.AddField("SHAPE");
                qf.AddField("OBJECTID");

                IFeatureClass featClass = featLayer.FeatureClass;
                int featCount = featClass.FeatureCount(qf);
                int currentCount = 0;
                IFeatureCursor featCursor = featClass.Search(qf, true);
                IFeature currentFeature = null;
                while ((currentFeature = featCursor.NextFeature()) != null)
                {
                    //Now we can perform the work to flip the anno if necessary
                    double currentAngle = GetCurrentAngle(currentFeature);

                    if (currentAngle == -1)
                    {
                        currentFailedList.Add(currentFeature.OID);
                    }
                    else
                    {
                        //Only need to flip it if it is appearing to be upsides down.
                        if ((currentAngle > 97 && currentAngle < 263))
                        {
                            currentOIDList.Add(currentFeature.OID);
                        }
                    }
                    currentCount++;
                    
                    if ((currentCount % 1000) == 0) 
                    {
                        currentProgress = Int32.Parse(Math.Floor(((((double)currentCount) / ((double)featCount)) * 100.0)).ToString());
                        progressBar.Value = currentProgress;
                        lblAnalyze.Text = "Analyzing layer \"" + featLayer.Name + "\": " + currentProgress + "%";
                        Application.DoEvents(); 
                    }
                }
                currentOIDList = currentOIDList.Distinct().ToList();
                currentFailedList = currentFailedList.Distinct().ToList();
                string featLayerName = featLayer.FeatureClass.AliasName;
                foreach (int oid in currentOIDList)
                {
                    resultsList.Items.Add(featLayerName + "-" + oid);
                }
                foreach (int oid in currentFailedList)
                {
                    failedResultList.Items.Add(featLayerName + "-" + oid);
                }
                affectedAnnotationfeatures[featLayer.FeatureClass] = currentOIDList;
                failedAnnotationfeatures[featLayer.FeatureClass] = currentFailedList;
            }

            lblFailedResultsCount.Text = failedResultList.Items.Count + " Features Found";
            lblResultsCount.Text = resultsList.Items.Count + " Features Found";
            lblAnalyze.Text = "";

            progressBar.Value = 100;
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
                    if (feat.Shape.GeometryType == esriGeometryType.esriGeometryPolygon)
                    {
                        IPointCollection pointCollection = feat.Shape as IPointCollection;
                        if (pointCollection.PointCount <= 5)
                        {
                            //Determine the angle from the first two points.
                            IPoint firstPoint = pointCollection.get_Point(0);
                            IPoint secondPoint = pointCollection.get_Point(1);
                            double dx = secondPoint.X - firstPoint.X;
                            double dy = secondPoint.Y - firstPoint.Y;

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

                            //For anything from 90 to 270 degrees we need to adjust the angle for as ATan only returns between -90 and 90
                            if (dx < 0 && dy > 0) { theta += 90; }
                            else if (dx < 0 && dy < 0) { theta += 90; }
                            else if (dx > 0 && dy > 0) { theta -= 90; }
                            else if (dx > 0 && dy < 0) { theta -= 90; }

                            while (theta < 0)
                            {
                                theta += 360;
                            }

                            return theta;
                        }
                        else
                        {
                            IAnnotationFeature2 annoFeat2 = feat as IAnnotationFeature2;
                            IElement annoElement = annoFeat2.Annotation;
                            IGeometry annoGeom = annoElement.Geometry;
                            
                            if (annoGeom.GeometryType == esriGeometryType.esriGeometryBag)
                            {
                                List<double> anglesCalculated = new List<double>();
                                IGeometryCollection bag = annoGeom as IGeometryCollection;
                                for (int i = 0; i < bag.GeometryCount; i++)
                                {
                                    IGeometry testGeom = bag.get_Geometry(i);

                                    if (testGeom.GeometryType == esriGeometryType.esriGeometryPolyline ||
                                        testGeom.GeometryType == esriGeometryType.esriGeometryMultipoint)
                                    {
                                        IPointCollection testGeomPointCollection = testGeom as IPointCollection;
                                        if (testGeomPointCollection.PointCount < 2) { continue; }

                                        //Determine the angle from the first and second point
                                        IPoint firstPoint = testGeomPointCollection.get_Point(0);
                                        IPoint secondPoint = testGeomPointCollection.get_Point(1);
                                        double dx = secondPoint.X - firstPoint.X;
                                        double dy = secondPoint.Y - firstPoint.Y;

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

                                        //For anything from 90 to 270 degrees we need to adjust the angle for as ATan only returns between -90 and 90
                                        if (dx < 0 && dy > 0) { theta += 180; }
                                        else if (dx < 0 && dy < 0) { theta += 180; }
                                        else if (dx > 0 && dy > 0) { theta += 0; }
                                        else if (dx > 0 && dy < 0) { theta += 0; }

                                        while (theta < 0)
                                        {
                                            theta += 360;
                                        }

                                        anglesCalculated.Add(theta);
                                    }
                                }

                                double origAngle = -1;
                                double totalOfAngles = 0.0;
                                if (anglesCalculated.Count < 1) { return -1; }
                                else { origAngle = anglesCalculated[0]; }

                                foreach (double angle in anglesCalculated)
                                {
                                    totalOfAngles += angle;
                                    double difference = angle - origAngle;

                                    if (difference > 6.0 || difference < -6.0)
                                    {
                                        return -1;
                                    }
                                }

                                return totalOfAngles / anglesCalculated.Count;
                            }
                            else if (annoGeom.GeometryType == esriGeometryType.esriGeometryPolyline)
                            {
                                IPointCollection testGeomPointCollection = annoGeom as IPointCollection;
                                if (testGeomPointCollection.PointCount < 2) { return -1; }

                                //Determine the angle from the first and second point
                                IPoint firstPoint = testGeomPointCollection.get_Point(0);
                                IPoint secondPoint = testGeomPointCollection.get_Point(1);
                                double dx = secondPoint.X - firstPoint.X;
                                double dy = secondPoint.Y - firstPoint.Y;

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

                                //For anything from 90 to 270 degrees we need to adjust the angle for as ATan only returns between -90 and 90
                                if (dx < 0 && dy > 0) { theta += 180; }
                                else if (dx < 0 && dy < 0) { theta += 180; }
                                else if (dx > 0 && dy > 0) { theta += 0; }
                                else if (dx > 0 && dy < 0) { theta += 0; }

                                while (theta < 0)
                                {
                                    theta += 360;
                                }

                                return theta;
                            }
                            else
                            {
                                return -1;
                            }
                        }
                    }
                }
            }
            catch { }
            return currentAngle;
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

        private void GetAnnoLayers()
        {
            AnnoFeatLayers.Clear();
            #region Get annotation feature layers for lines
            if (AnnoFeatLayers.Count < 1)
            {
                //Get all of the IFeatureLayers
                ESRI.ArcGIS.esriSystem.UID uid = new ESRI.ArcGIS.esriSystem.UIDClass();
                uid.Value = "{40A9E885-5533-11D0-98BE-00805F7CED21}";
                IEnumLayer featLayers = mxDoc.FocusMap.get_Layers(uid);
                IFeatureLayer featLayer = featLayers.Next() as IFeatureLayer;
                while (featLayer != null)
                {
                    try
                    {
                        if (featLayer.FeatureClass.FeatureType == esriFeatureType.esriFTAnnotation)
                        {
                            IEnumRelationshipClass relClasses = featLayer.FeatureClass.get_RelationshipClasses(esriRelRole.esriRelRoleAny);
                            relClasses.Reset();
                            IRelationshipClass relClass = relClasses.Next();
                            if (relClass.OriginClass is IFeatureClass)
                            {
                                IFeatureClass featClass = relClass.OriginClass as IFeatureClass;
                                if (featClass.FeatureType != esriFeatureType.esriFTComplexEdge &&
                                    featClass.FeatureType != esriFeatureType.esriFTSimpleEdge)
                                {
                                    featLayer = featLayers.Next() as IFeatureLayer;
                                    continue;
                                }
                                else
                                {
                                    AnnoFeatLayers.Add(featLayer);
                                }
                            }

                        }
                    }
                    catch (Exception e) { }
                    featLayer = featLayers.Next() as IFeatureLayer;
                }
            }
            #endregion
        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void btnFixAll_Click(object sender, EventArgs e)
        {
            if (!IsEditing()) { return; }

            //Check our angles
            try
            {
                currentMinAngle = Int32.Parse(txtMinDegrees.Text);
                currentMaxAngle = Int32.Parse(txtMaxDegrees.Text);
            }
            catch
            {
                MessageBox.Show("Invalid angles specified");
                return;
            }

            GetAnnoLayers();

            RecreateAnno();
        }

        
    }
}
