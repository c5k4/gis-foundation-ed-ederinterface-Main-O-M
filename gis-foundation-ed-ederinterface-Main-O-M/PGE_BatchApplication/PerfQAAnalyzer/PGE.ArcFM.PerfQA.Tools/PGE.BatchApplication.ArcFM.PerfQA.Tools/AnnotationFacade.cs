using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;

namespace PGE.BatchApplication.ArcFM_PerfQA_Tools
{
    /// <summary>
    /// Class to Manage Annotation Manipulation
    /// </summary>
    public static class AnnotationFacade
    {
        public const double DoNotRotate = -361;
        public const double DoNotChangeFontSize = -1;
        public const int DoNotChangeBoldSetting = 2;
        public const int DoNotChangeAlignment = 4;

        #region Anno Transformation Methods

        public static void ApplySettingsToNewAnno(IRelationshipClass AnnoRelClass, IObject ProcessObject,
            int alignment, double angle, int annoClassID, bool inlineGroupAnno, double newXOffset, double newYOffset, double fontSize)
        {
            ApplySettingsToNewAnno(AnnoRelClass, ProcessObject, alignment, angle, annoClassID, inlineGroupAnno,
                newXOffset, newYOffset, null, fontSize, DoNotChangeBoldSetting);
        }

        public static void ApplySettingsToNewAnno(IRelationshipClass AnnoRelClass, IObject ProcessObject,
            int alignment, double angle, int annoClassID, bool inlineGroupAnno, double newXOffset, double newYOffset,
            int bold)
        {
            ApplySettingsToNewAnno(AnnoRelClass, ProcessObject, alignment, angle, annoClassID, inlineGroupAnno,
                newXOffset, newYOffset, null, DoNotChangeFontSize, bold);
        }

        public static void ApplySettingsToNewAnno(IRelationshipClass AnnoRelClass, IObject ProcessObject,
            int alignment, int annoClassID, bool inlineGroupAnno, double newXOffset, double newYOffset,
            double fontSize, int bold)
        {
            ApplySettingsToNewAnno(AnnoRelClass, ProcessObject, alignment, DoNotRotate, annoClassID, inlineGroupAnno,
                newXOffset, newYOffset, null, fontSize, bold);
        }

        /// <summary>
        /// Edits all the specified parameters
        /// </summary>
        /// <param name="fontSize">The new font size to set the anno to</param>
        /// <param name="bold">The new bold setting to set the anno to</param>
        public static void ApplySettingsToNewAnno(IRelationshipClass AnnoRelClass, IObject ProcessObject,
            int alignment, double angle, int annoClassID, bool inlineGroupAnno, double newXOffset, double newYOffset,
            double fontSize, int bold)
        {
            ApplySettingsToNewAnno(AnnoRelClass, ProcessObject, alignment, angle, annoClassID, inlineGroupAnno,
                newXOffset, newYOffset, null, fontSize, bold);
        }

        /// <summary>
        /// Given a relationship, loops through each related feature and calculates overall offsets, then
        /// applies the collection's attributes to these features based on the annotation class ID.
        /// Uses offset amounts.
        /// </summary>
        /// <param name="AnnoRelClass">The relationship class between the applicable anno and its related feature class.</param>
        /// <param name="ProcessObject">
        ///     The object to process. If this is the parent feature, the method will be applied to all its related
        ///     annotation. Otherwise, if this is the annotation feature, the method will only be applied to the specified anno.
        /// </param>
        /// <param name="alignment">Integer corresponding to horizontal alignment value. (0 = Left, 1 = Center, 2 = Right)</param>
        /// <param name="angle">The new angle that will be applied to the relvant annotation.</param>
        /// <param name="annoClassID">
        ///     The annotation class ID of the object within the processing set used to determine calculations such as anchor
        ///     points, starting points for offsets, etc.
        /// </param>
        /// <param name="inlineGroupAnno">Whether or not to inline eligible grouped annotation.</param>
        /// <param name="newXOffset">The new X coordinate offset.</param>
        /// <param name="newYOffset">The new Y coordinate offset.</param>
        public static void ApplySettingsToNewAnno(IRelationshipClass AnnoRelClass, IObject ProcessObject,
            int alignment, double angle, int annoClassID, bool inlineGroupAnno, double newXOffset, double newYOffset)
        {
            ApplySettingsToNewAnno(AnnoRelClass, ProcessObject, alignment, angle, annoClassID, inlineGroupAnno, newXOffset, newYOffset, null, 
                DoNotChangeFontSize, DoNotChangeBoldSetting);
        }

        /// <summary>
        /// Given a relationship, loops through each related feature and calculates overall offsets, then
        /// applies the collection's attributes to these features based on the annotation class ID.
        /// Determines offsets based on a new desired location.
        /// </summary>
        /// <param name="AnnoRelClass">The relationship class between the applicable anno and its related feature class.</param>
        /// <param name="ProcessObject">
        ///     The object to process. If this is the parent feature, the method will be applied to all its related
        ///     annotation. Otherwise, if this is the annotation feature, the method will only be applied to the specified anno.
        /// </param>
        /// <param name="alignment">Integer corresponding to horizontal alignment value. (0 = Left, 1 = Center, 2 = Right)</param>
        /// <param name="angle">The new angle that will be applied to the relvant annotation.</param>
        /// <param name="annoClassID">
        ///     The annotation class ID of the object within the processing set used to determine calculations such as anchor
        ///     points, starting points for offsets, etc.
        /// </param>
        /// <param name="newPlacementPoint">The specific position to which the anno will be moved.</param>
        public static void ApplySettingsToNewAnno(IRelationshipClass AnnoRelClass, IObject ProcessObject,
            int alignment, double angle, int annoClassID, IPoint newPlacementPoint)
        {
            ApplySettingsToNewAnno(AnnoRelClass, ProcessObject, alignment, angle, annoClassID, false, 0, 0, newPlacementPoint, DoNotChangeFontSize,
                DoNotChangeBoldSetting);
        }

        /// <summary>
        /// Given a relationship, loops through each related feature and calculates overall offsets, then
        /// applies the collection's attributes to these features based on the annotation class ID.
        /// This private method includes all options and is where the core logic actually takes place.
        /// </summary>
        /// <param name="AnnoRelClass">The relationship class between the applicable anno and its related feature class.</param>
        /// <param name="ProcessObject">
        ///     The object to process. If this is the parent feature, the method will be applied to all its related
        ///     annotation. Otherwise, if this is the annotation feature, the method will only be applied to the specified anno.
        /// </param>
        /// <param name="alignment">Integer corresponding to horizontal alignment value. (0 = Left, 1 = Center, 2 = Right)</param>
        /// <param name="angle">The new angle that will be applied to the relvant annotation.</param>
        /// <param name="annoClassID">
        ///     The annotation class ID of the object within the processing set used to determine calculations such as anchor
        ///     points, starting points for offsets, etc.
        /// </param>
        /// <param name="inlineGroupAnno">Whether or not to inline eligible grouped annotation.</param>
        /// <param name="newXOffset">The new X coordinate offset.</param>
        /// <param name="newYOffset">The new Y coordinate offset.</param>
        /// <param name="newPlacementPoint">
        ///     Overrides manual coordinate offsets if specified, in favor of this specific position to which the anno will be moved.
        /// </param>
        private static void ApplySettingsToNewAnno(IRelationshipClass AnnoRelClass, IObject ProcessObject, int alignment,
            double angle, int annoClassID, bool inlineGroupAnno, double newXOffset, double newYOffset, IPoint newPlacementPoint,
            double fontSize, int bold)
        {
            //Code=3 in the horizontalalignment domain is used for 'full'. However we do not use that code in this class so change it to no change.
            if (alignment == 3) alignment = DoNotChangeAlignment;

            IRelatedObjectClassEvents originEvents = AnnoRelClass.OriginClass.Extension as IRelatedObjectClassEvents;
            IRelatedObjectClassEvents destEvents = AnnoRelClass.DestinationClass.Extension as IRelatedObjectClassEvents;

            if (destEvents != null || originEvents != null)
            {
                //Find candidates for processing.
                List<IObject> processCandidates = new List<IObject>();
                Dictionary<IObject, int> eligibleObjects = new Dictionary<IObject, int>();
                //If we passed in the anno itself, we'll just apply these setting to it alone. If we passed in the parent, we'll
                //apply the settings to every applicable child of that parent in this relationship.
                if (AnnoRelClass.OriginClass.ObjectClassID == ProcessObject.Class.ObjectClassID)
                {
                    ISet relObjs = AnnoRelClass.GetObjectsRelatedToObject(ProcessObject);
                    
                    for (IObject relObj = relObjs.Next() as IObject; relObj != null; relObj = relObjs.Next() as IObject)
                    {
                        processCandidates.Add(relObj);
                    }
                }
                else
                {
                    processCandidates.Add(ProcessObject);
                }

                //Loop 1: Remove any non-applicable anno.
                foreach(IObject processCandidate in processCandidates)
                {
                    //We've seen cases where expressions mess up. Take a note if this happens.
                    bool annoError = false;
                    if (AnnoShapeExists(processCandidate, out annoError) && !eligibleObjects.ContainsKey(processCandidate))
                    {
                        //Get anno class ID
                        int currentAnnoClassID = 0;
                        AnnotationFacade.FindAnnoClass(processCandidate as IFeature, out currentAnnoClassID);

                        //Add to dictionary
                        eligibleObjects.Add(processCandidate, currentAnnoClassID);
                    }
                }

                //No need to continue if we don't have any objects.
                if (eligibleObjects.Count == 0)
                    return;

                //If for some reason the annotation class ID that was passed in as a "base" no longer exists, choose another one.
                if (!eligibleObjects.Values.Contains(annoClassID) && eligibleObjects.Count > 0)
                {
                    annoClassID = eligibleObjects.First().Value;
                }

                IPoint rotateAnglePoint = null;
                if (angle != DoNotRotate)
                {
                    
                    //Determine the current rotation and get the anchorpoint which will be used for subsequent angle rotations.
                    double currentAngle = 0.0;
                    foreach (KeyValuePair<IObject, int> relObjDuple in eligibleObjects)
                    {
                        if (annoClassID == relObjDuple.Value)
                        {
                            currentAngle = GetCurrentAngle(relObjDuple.Key as IFeature);
                            rotateAnglePoint = GetAnchorPoint(relObjDuple.Key as IFeature, currentAngle);
                            break;
                        }
                    }

                    //Apply an angle rotation to set everything to an effective angle rotation of 0
                    foreach (IObject relObj in eligibleObjects.Keys)
                    {
                        IFeature relFeat = relObj as IFeature;
                        ResetAngleToZero(relFeat, rotateAnglePoint);
                    }
                }

                //Apply the horizontal alignment to all features
                foreach (IObject relObj in eligibleObjects.Keys)
                {
                    ApplyHorizontalAlignment(relObj, alignment);
                }

                //Adjust annotations to offset them to still be aligned properly based on their horizontal alignment
                foreach (KeyValuePair<IObject, int> relObjDuple in eligibleObjects)
                {
                    if (annoClassID == relObjDuple.Value)
                    {
                        IPoint point1 = GetAnchorPoint(relObjDuple.Key as IFeature, alignment, 0);
                        //Adjust offsets to ensure the annotations are still lining up after the alignment may have changed
                        foreach (KeyValuePair<IObject, int> relObjDuple2 in eligibleObjects)
                        {
                            if (annoClassID != relObjDuple2.Value)
                            {
                                IPoint point2 = GetAnchorPoint(relObjDuple2.Key as IFeature, alignment, 0);
                                double xOffset = 0.0;
                                xOffset = point1.X - point2.X;

                                double yOffset = 0.0;

                                UpdateOffsets(relObjDuple2.Key, xOffset, yOffset);
                            }
                        }
                    }
                }

                if (angle != DoNotRotate)
                {
                    //Loop 2: Apply horizontal index, get anchor point on new anno, apply angle rotation, find offsets.
                    foreach (IObject relObj in eligibleObjects.Keys)
                        ApplyAngleRotation(relObj, alignment, angle, rotateAnglePoint);
                }

                //If we have a point, find the offsets. Otherwise use the specified offsets.
                if (newPlacementPoint != null)
                {
                    foreach (KeyValuePair<IObject, int> relObjDuple in eligibleObjects)
                    {
                        if (annoClassID == relObjDuple.Value)
                        {
                            FindOffsets(relObjDuple.Key as IFeature, alignment, angle, newPlacementPoint, out newXOffset, out newYOffset);
                            break;
                        }
                    }
                }

                //Loop 3: Apply the offsets.
                foreach (IObject relObj in eligibleObjects.Keys)
                    UpdateOffsets(relObj, newXOffset, newYOffset);

                //Apply font size change and bold change, if any
                foreach (IObject relObj in eligibleObjects.Keys)
                {
                    ApplyFontSize(relObj, fontSize);
                    ApplyBold(relObj, bold);
                }
                
                //Inline any eligible annotation if specified.
                if (inlineGroupAnno)
                    CheckAndInlineEligibleAnno(eligibleObjects.Keys.ToList());
            }
        }

        /// <summary>
        /// Finds an annotation's parent object - used in other methods.
        /// </summary>
        /// <param name="annoToSearch">The annotation object for which a parent needs to be found.</param>
        /// <param name="rel">The relationship class for the specified annotation object.</param>
        /// <returns>The parent object found for the referenced object.</returns>
        public static IObject FindAnnoParent(IObject annoToSearch, IRelationshipClass rel)
        {
            IRelatedObjectClassEvents originEvents = null;
            IRelatedObjectClassEvents destEvents = null;

            //Cast as IRelatedObjectClassEvents to ensure that the relationship can be recreated.
            originEvents = rel.OriginClass.Extension as IRelatedObjectClassEvents;
            destEvents = rel.DestinationClass.Extension as IRelatedObjectClassEvents;

            IObject parentObject = null;
            if (destEvents != null || originEvents != null)
            {
                ISet relatedFeatures = rel.GetObjectsRelatedToObject(annoToSearch);
                parentObject = relatedFeatures.Next() as IObject;
            }

            return parentObject;
        }

        /// <summary>
        /// Returns the properties of the annotation class for a given annotation feature, including its name.
        /// </summary>
        /// <param name="annoFeature">The annotation feature for which to obtain a class.</param>
        /// <param name="annoClassID">Populated with the annotation class ID of the annotation.</param>
        /// <returns>The properties of the annotation class.</returns>
        public static IAnnotateLayerProperties FindAnnoClass(IFeature annoFeature, out int annoClassID)
        {
            annoClassID = 0;
            Int32.TryParse(annoFeature.get_Value(annoFeature.Fields.FindField("ANNOTATIONCLASSID")).ToString(), out annoClassID);

            return (((annoFeature.Class as IFeatureClass).Extension as IAnnotationClassExtension).AnnoProperties as IAnnotateLayerPropertiesCollection2).get_Properties(annoClassID);
        }

        /// <summary>
        /// Drops all the annotation objects for a given annotation's feature.
        /// </summary>
        /// <param name="parentObject">The parent object of the annotation. All annotation with the specified relationship class under this parent will be deleted.</param>
        /// <param name="rel">The relationship tying the parent object to its annotation.</param>
        public static void DeleteAnno(IObject parentObject, IRelationshipClass rel)
        {
            DeleteAnno(parentObject, rel, true, null);
        }
       

        /// <summary>
        /// Drops all the annotation objects for a given annotation's feature.
        /// </summary>
        /// <param name="parentObject">The parent object of the annotation. All annotation with the specified relationship class under this parent will be deleted.</param>
        /// <param name="rel">The relationship tying the parent object to its annotation.</param>
        /// <param name="deleteAll">If <c>false</c>, only deletes annotation matching the <c>classNameMatchParameter</c></param>
        /// <param name="classNameMatch">If specified and <c>deleteAll</c> is false, only the annotation classes containing this name will be deleted.</param>
        /// <returns>A list of OIDs that were deleted as a result of this method.</returns>
        public static List<int> DeleteAnno(IObject parentObject, IRelationshipClass rel, bool deleteAll, string classNameMatch)
        {
            if (!deleteAll && classNameMatch.IsNullOrWhitespace() ) return new List<int>();

            List<int> deletedOIDs = new List<int>();

            IRelatedObjectClassEvents originEvents = null;
            IRelatedObjectClassEvents destEvents = null;

            //Cast as IRelatedObjectClassEvents to ensure that the relationship can be recreated.
            originEvents = rel.OriginClass.Extension as IRelatedObjectClassEvents;
            destEvents = rel.DestinationClass.Extension as IRelatedObjectClassEvents;

            if (parentObject != null)
            {
                ISet relObjs = rel.GetObjectsRelatedToObject(parentObject);
                for (IObject relObj = relObjs.Next() as IObject; relObj != null; relObj = relObjs.Next() as IObject)
                {
                    try
                    {
                        //Get anno class information
                        int annoClassID = 0;
                        string annoClassName = AnnotationFacade.FindAnnoClass(relObj as IFeature, out annoClassID).Class;

                        if (deleteAll || (!classNameMatch.IsNullOrWhitespace() && annoClassName.ToUpper().Contains(classNameMatch.ToUpper())))
                        {
                            int deletedOID = relObj.OID;
                            relObj.Delete();
                            deletedOIDs.Add(deletedOID);
                        }
                    }
                    catch (Exception)
                    {

                    }
                }
            }

            return deletedOIDs;
        }

        /// <summary>
        /// Any annotation objects under the parent with an annotation ID matching <c>classNameMatch</c> will be returned as eligible for deletion.
        /// </summary>
        /// <param name="parentObject">The parent object of the annotation. All annotation with the specified relationship class under this parent will be marked for deletion.</param>
        /// <param name="rel">The relationship tying the parent object to its annotation.</param>
        /// <param name="classNameMatch">Only the annotation classes containing this name will be marked for deletion.</param>
        /// <returns>A list of OIDs that were marked for deletion as a result of this method.</returns>
        public static List<int> GetAnnosForDeletion(IObject parentObject, IRelationshipClass rel, string classNameMatch)
        {
            if (classNameMatch.IsNullOrWhitespace()) return new List<int>();

            List<int> deletedOIDs = new List<int>();

            IRelatedObjectClassEvents originEvents = null;
            IRelatedObjectClassEvents destEvents = null;

            //Cast as IRelatedObjectClassEvents to ensure that the relationship can be recreated.
            originEvents = rel.OriginClass.Extension as IRelatedObjectClassEvents;
            destEvents = rel.DestinationClass.Extension as IRelatedObjectClassEvents;

            if (parentObject != null)
            {
                ISet relObjs = rel.GetObjectsRelatedToObject(parentObject);
                for (IObject relObj = relObjs.Next() as IObject; relObj != null; relObj = relObjs.Next() as IObject)
                {
                    try
                    {
                        //Get anno class information
                        int annoClassID = 0;
                        string annoClassName = AnnotationFacade.FindAnnoClass(relObj as IFeature, out annoClassID).Class;

                        if (!classNameMatch.IsNullOrWhitespace()  && annoClassName.ToUpper().Contains(classNameMatch.ToUpper()))
                        {
                            int deletedOID = relObj.OID;
                            deletedOIDs.Add(deletedOID);
                        }
                    }
                    catch (Exception)
                    {

                    }
                }
            }

            return deletedOIDs;
        }

        /// <summary>
        /// Drops and recreates the annotation objects for a given feature.
        /// </summary>
        /// <param name="parentObject">The parent object of the annotation. All annotation with the specified relationship class under this parent will be recreated.</param>
        /// <param name="rel">The relationship tying the parent object to its annotation.</param>
        public static void RecreateAnno(IObject parentObject, IRelationshipClass rel)
        {
            try
            {
                IRelatedObjectClassEvents originEvents = null;
                IRelatedObjectClassEvents destEvents = null;

                //Cast as IRelatedObjectClassEvents to ensure that the relationship can be recreated.
                originEvents = rel.OriginClass.Extension as IRelatedObjectClassEvents;
                destEvents = rel.DestinationClass.Extension as IRelatedObjectClassEvents;

                if (parentObject != null)
                {
                    ISet relObjs = rel.GetObjectsRelatedToObject(parentObject);
                    for (IObject relObj = relObjs.Next() as IObject; relObj != null; relObj = relObjs.Next() as IObject)
                    {
                        try
                        {
                            relObj.Delete();
                        }
                        catch (Exception)
                        {

                        }
                    }
                }

                //Now we can recreate our related objects
                if (destEvents != null)
                    destEvents.RelatedObjectCreated(rel, parentObject);

                if (originEvents != null)
                    originEvents.RelatedObjectCreated(rel, parentObject);
            }
            catch (Exception)
            {
                
                throw;
            }
           
        }

        #endregion Anno Transformation Methods

        #region Offset Methods

        /// <summary>
        /// Find the offsets from the anchor point of the given feature to the new location to be moved to
        /// </summary>
        /// <param name="feat">IFeature to get current anchor point from</param>
        /// <param name="alignment">Horizontal alignment of the feature</param>
        /// <param name="angle">Angle of rotation of the feature</param>
        /// <param name="newPlacementPoint">The new location to be moved to</param>
        /// <param name="xOffset">The X Offset of the feature</param>
        /// <param name="yOffset">The Y Offset of the feature</param>
        private static void FindOffsets(IFeature feat, int alignment, double angle, IPoint newPlacementPoint, out double xOffset, out double yOffset)
        {
            if (!(feat is IAnnotationFeature2))
            {
                xOffset = 0;
                yOffset = 0;
                return;
            }

            if (alignment == DoNotChangeAlignment)
                Int32.TryParse(feat.get_Value(feat.Fields.FindField("HORIZONTALALIGNMENT")).ToString(), out alignment);

            IAnnotationFeature2 annoFeat = feat as IAnnotationFeature2;
            IGeometry currentGeometry = annoFeat.Annotation.Geometry;

            IPoint point = GetAnchorPoint(feat, alignment, angle);
            double newX = point.X;
            double newY = point.Y;

            xOffset = newPlacementPoint.X - newX;
            yOffset = newPlacementPoint.Y - newY;
        }

        /// <summary>
        /// Update offsets based on the current geometry.
        /// </summary>
        /// <param name="obj">The current object to update.</param>
        /// <param name="xOffset">The X Offset of the feature</param>
        /// <param name="yOffset">The Y Offset of the feature</param>
        private static void UpdateOffsets(IObject obj, double xOffset, double yOffset)
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
        private static void SetOffsetForPoints(IFeature feature, double offsetX, double offsetY)
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

        #endregion Offset Methods

        #region Rotation Methods

        /// <summary>
        /// Determines the current angle of the given feature
        /// </summary>
        /// <param name="feat">IFeature to calculate current angle</param>
        /// <returns>The current rotation angle of the given feature.</returns>
        public static double GetCurrentAngle(IFeature feat)
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

        private static void ResetAngleToZero(IFeature feature, IPoint aboutPoint)
        {
            double startAngle = GetCurrentAngle(feature);

            double adjustmentAngleToZero = 0 - startAngle;
            ApplyAngleRotation(feature, adjustmentAngleToZero, aboutPoint);
            feature.Store();

            //Sometimes this rotates the base point of the anno but doesn't actually rotate the text itself.
            //Check for this and set the feature's "ANGLE" property if needed, in order to reset this.
            if (startAngle != 0 && Math.Round(GetCurrentAngle(feature), 2) != 0)
            {
                feature.set_Value(feature.Fields.FindField("ANGLE"), 0);
                feature.Store();
            }
        }

        /// <summary>
        /// Apply the specified angle to the given annotation based on the alignment of the feature to determine anchor point
        /// </summary>
        /// <param name="newAnno">Annotation to apply rotation to</param>
        /// <param name="alignment">Alignment of the annotation (used to calculate anchor point)</param>
        /// <param name="angle">Angle to rotate</param>
        /// <param name="aboutPoint">IPoint to rotate about</param>
        private static void ApplyAngleRotation(IObject newAnno, int alignment, double angle, IPoint aboutPoint)
        {
            IAnnotationFeature2 annoFeat2 = newAnno as IAnnotationFeature2;
            IElement annoElement = annoFeat2.Annotation;
            IFeature feat = annoFeat2 as IFeature;

            if (feat.Shape != null)
            {
                double startAngle = 0;
                startAngle = GetCurrentAngle(feat);

                double adjustmentAngle = angle;
                //Adjust the angle if there was a current angle found other than 0.
                if (startAngle != 0)
                {
                    adjustmentAngle = angle - startAngle;
                }

                ApplyAngleRotation(feat, adjustmentAngle, aboutPoint);
                feat.Store();

                //Check to see if the current angle actually changed. If not, attempt to change the actual angle.
                if (startAngle != angle && Math.Round(startAngle, 2) == Math.Round(GetCurrentAngle(feat), 2))
                {
                    feat.set_Value(feat.Fields.FindField("ANGLE"), angle);
                    feat.Store();

                    //Work around for ESRIbug. Must call store twice
                    feat.Store();
                }
            }
        }

        /// <summary>
        /// Applies the specified angle to the given feature using the specified anchor point
        /// </summary>
        /// <param name="feature">IFeature to apply angle rotation to</param>
        /// <param name="angle">Angle to rotate by</param>
        /// <param name="aboutPoint">IPoint to rotate about</param>
        private static void ApplyAngleRotation(IFeature feature, double angle, IPoint aboutPoint)
        {
            IAnnotationFeature annoFeat2 = feature as IAnnotationFeature;
            IElement annoElement = annoFeat2.Annotation;

            IGeometry annoGeom = annoElement.Geometry;

            ITransform2D transform2D = annoGeom as ITransform2D;
            double radianAngle = (Math.PI / 180.0) * angle;
            transform2D.Rotate(aboutPoint, radianAngle);

            annoElement.Geometry = transform2D as IGeometry;

            ITextElement textElement = annoElement as ITextElement;

            annoFeat2.Annotation = textElement as IElement;
        }

        #endregion Rotation Methods

        #region AttributeEdit Methods

        private static void ApplyBold(IObject anno, int bold)
        {
            //No change
            if (bold == DoNotChangeBoldSetting) return;

            IFeature feat = anno as IFeature;
            feat.set_Value(feat.Fields.FindField("BOLD"), bold);
            feat.Store();
        }

        private static void ApplyFontSize(IObject anno, double fontSize)
        {
            IFeature feat = anno as IFeature;

            if (fontSize != DoNotChangeFontSize)
            {
                feat.set_Value(feat.Fields.FindField("FONTSIZE"), fontSize);
                feat.Store();
            }
        }

        /// <summary>
        /// Applies the horizontal alignment to the specified feature
        /// </summary>
        /// <param name="newAnno">Annotation to apply alignment to</param>
        /// <param name="alignment">Alignment to assign</param>
        private static void ApplyHorizontalAlignment(IObject newAnno, int alignment)
        {
            if (alignment == DoNotChangeAlignment) return;

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
        /// This method sets the alignment to baseline and stores the
        /// feature to prepare it for inline processing.
        /// </summary>
        /// <param name="annoFeat">The feature whose alignment will be reset to baseline.</param>
        private static void ResetVerticalAlignment(IFeature annoFeat)
        {
            IAnnotationFeature2 annoFeat2 = annoFeat as IAnnotationFeature2;
            IElement annoElement = annoFeat2.Annotation;
            //cast IElement object to ISymbolCollectionElement
            ISymbolCollectionElement sce = annoElement as ISymbolCollectionElement;
            if (sce != null)
            {
                // Reset Alignment
                sce.VerticalAlignment = ESRI.ArcGIS.Display.esriTextVerticalAlignment.esriTVABaseline;
                annoFeat2.Annotation = sce as IElement;
                annoFeat = annoFeat2 as IFeature;
                annoFeat.Store();
            }
        }

        #endregion Alignment Methods

        #region Shape Methods

        /// <summary>
        /// Gets the anchor point of the given feature based on the angle of rotation of the feature.
        /// Attempts to determine the feature's alignment using annotation fields.
        /// </summary>
        /// <param name="feature">IFeature to get the anchor point for</param>
        /// <param name="angle">Angle of rotation of the feature</param>
        /// <returns>The anchor point of the given feature.</returns>
        private static IPoint GetAnchorPoint(IFeature feature, double angle)
        {
            int alignment = Convert.ToInt32(feature.get_Value(feature.Fields.FindField("HORIZONTALALIGNMENT")));
            return GetAnchorPoint(feature, alignment, angle);
        }

        /// <summary>
        /// Gets the anchor point of the given feature based on the horizontal alignment and the angle of rotation of the feature
        /// </summary>
        /// <param name="feature">IFeature to get the anchor point for</param>
        /// <param name="HorizontalAlignment">Horizontal alignment of the feature</param>
        /// <param name="angle">Angle of rotation of the feature</param>
        /// <returns>The anchor point of the given feature.</returns>
        private static IPoint GetAnchorPoint(IFeature feature, int HorizontalAlignment, double angle)
        {
            int indexHolder = -1;
            if (feature.Shape.GeometryType == esriGeometryType.esriGeometryPolygon)
            {
                if (HorizontalAlignment == DoNotChangeAlignment)
                    Int32.TryParse(feature.get_Value(feature.Fields.FindField("HORIZONTALALIGNMENT")).ToString(), out HorizontalAlignment);

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
        /// <returns>An IPoint which holds the maximum Y coordinate.</returns>
        private static IPoint GetMaxYPoint(IFeature feature, ref int maxYIndex)
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
        /// <returns>An IPoint which holds the minimum Y coordinate.</returns>
        private static IPoint GetMinYPoint(IFeature feature, ref int minYIndex)
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
        /// <returns>An IPoint which holds the maximum X coordinate.</returns>
        private static IPoint GetMaxXPoint(IFeature feature, ref int maxXIndex)
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
        /// <returns>An IPoint which holds the minimum X coordinate.</returns>
        private static IPoint GetMinXPoint(IFeature feature, ref int minXPoint)
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
        private static bool AnnoShapeExists(IObject annoObj, out bool nullAnnoElement)
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

        #endregion Shape Methods

        #region Inline Methods

        /// <summary>
        /// Checks to see if the parameter list of annotation can be inlined, and does so if they can.
        /// Derived mostly from the AnnoModification code provided by the Schematics team.
        /// </summary>
        /// <param name="annoObjects">The list of objects passed in to process, that will be inlined if they fit the eligibility requirements.</param>
        private static void CheckAndInlineEligibleAnno(List<IObject> annoObjects)
        {
            // 2014.03.18 cblakeborough: Per the initial Schematics code, we were limiting inlinine to specific classes; however, it appears that the
            //  expectation is to allow any class to be inlined.

            // 2014.03.18 cblakeborough: Sort the anno objects by their anno class ID and inline each anno with the previous one.
            SortedDictionary<int, IFeature> orderedFeatures = new SortedDictionary<int, IFeature>();
            foreach(IObject annoObj in annoObjects)
            {
                //Get anno class ID
                int currentAnnoClassID = 0;
                AnnotationFacade.FindAnnoClass(annoObj as IFeature, out currentAnnoClassID);

                if (!orderedFeatures.ContainsKey(currentAnnoClassID))
                    orderedFeatures.Add(currentAnnoClassID, annoObj as IFeature);
            }

            if (orderedFeatures.Count < 2)
                return;

            IFeature previousAnnoFeat = null;
            foreach (IFeature currFeat in orderedFeatures.Values.ToList())
            {
                if (previousAnnoFeat == null)
                {
                    previousAnnoFeat = currFeat;
                    continue;
                }

                InlineEligibleAnno(previousAnnoFeat, currFeat);

                previousAnnoFeat = currFeat;
            }
        }

        /// <summary>
        /// Inlines two pieces of annotation.
        /// Used from the AnnoModification code provided by the Schematics team.
        /// </summary>
        /// <param name="pSrcFeat">The "source" feature - the base feature to base the inline off of.</param>
        /// <param name="pTarFeat">The "target" feature - the feature to inline off of the source.</param>
        private static void InlineEligibleAnno(IFeature pSrcFeat, IFeature pTarFeat)
        {
            IPoint pntProj = new PointClass();
            IPoint pntProj1 = new PointClass();
            IPoint ActProj = new PointClass();
            IPoint ActProj1 = new PointClass();

            IGeometry pSrcGeometry = pSrcFeat.ShapeCopy; // pAnnoFeature;
            IPolygon pSrcPolygon = (IPolygon)pSrcGeometry;
            IPointCollection pSrcPointCollection = (IPointCollection)pSrcPolygon;
            IAnnotationFeature2 pSrcAnno = pSrcFeat as IAnnotationFeature2;
            IGeometry pSrcAnnotationGeometry = pSrcAnno.Annotation.Geometry;

            double dblRotAng = InlineGetAngleBetweenPoints(pSrcPointCollection.get_Point(0), pSrcPointCollection.get_Point(3));
            pntProj = InlineGetProjectionPoint(pSrcPointCollection.get_Point(3), dblRotAng, 5);

            IAnnotationFeature2 pTarAnno = pTarFeat as IAnnotationFeature2;
            // 2014.03.18 cblakeborough: Not all geometries here are polylines. Making a path to retrieve the necessary data if that isn't the case.
            if (pTarAnno.Annotation.Geometry.GeometryType != esriGeometryType.esriGeometryPolyline)
                ConvertAnnotationPointToPolyline(pTarFeat);
            IGeometry pTarAnnotationGeometry = pTarAnno.Annotation.Geometry;
            IGeometry pTarGeometry = pTarFeat.ShapeCopy;

            IPoint p0 = null;
            double dblGeoAng = 0, dbltarGeoLg = 0;
            
            IPolyline pTarPolyline = (IPolyline)pTarAnnotationGeometry;
            IPointCollection pTarPointColl = (IPointCollection)pTarPolyline;
            p0 = pTarPointColl.get_Point(0);

            dblGeoAng = InlineGetAngleBetweenPoints(pTarPointColl.get_Point(0), pTarPointColl.get_Point(1));
            dbltarGeoLg = InlineGetPointDistance(pTarPointColl.get_Point(0), pTarPointColl.get_Point(1));

            IPolygon pTarPolygon = (IPolygon)pTarGeometry;
            IPointCollection pTarPointCollection = (IPointCollection)pTarPolygon;
            double dblMinDist1 = InlineGetPointDistance(pTarPointCollection.get_Point(0), pntProj);

            double dblAng1 = InlineGetAngleBetweenPoints(pTarPointCollection.get_Point(0), pntProj);

            ActProj = InlineGetProjectionPoint(p0, dblAng1, dblMinDist1);
            ActProj1 = InlineGetProjectionPoint(ActProj, dblGeoAng, dbltarGeoLg);

            IPolyline pLine = new PolylineClass();
            pLine.ToPoint = ActProj1;
            pLine.FromPoint = ActProj;



            int intClassId = pTarAnno.AnnotationClassID;
            IElement pElement = pTarAnno.Annotation;
            pElement.Geometry = pLine as IGeometry;

            ITextElement pTxtElm1 = pElement as ITextElement;
            pTarAnno.Annotation = pTxtElm1 as IElement;
            pTarAnno.AnnotationClassID = intClassId;

            pTarFeat.Store();
        }

        /// <summary>
        /// Finds the distance between two points.
        /// Used from the AnnoModification code provided by the Schematics team.
        /// </summary>
        /// <param name="p1">The first point to measure.</param>
        /// <param name="p2">The second point to measure.</param>
        /// <returns>The distance between the two points.</returns>
        private static double InlineGetPointDistance(IPoint p1, IPoint p2)
        {
            return ((IProximityOperator)p1).ReturnDistance(p2);
        }

        /// <summary>
        /// Finds the angle between two points.
        /// Used from the AnnoModification code provided by the Schematics team.
        /// </summary>
        /// <param name="pStrtPnt">The start point.</param>
        /// <param name="pEndPnt">The end point.</param>
        /// <returns>The angle between the two points.</returns>
        private static double InlineGetAngleBetweenPoints(IPoint pStrtPnt, IPoint pEndPnt)
        {
            double x1 = pStrtPnt.X;
            double y1 = pStrtPnt.Y;
            double x2 = pEndPnt.X;
            double y2 = pEndPnt.Y;
            double pxRes = x2 - x1;
            double pyRes = y2 - y1;
            double angle = 0.0;
            // Calculate the angle 
            if (pxRes == 0.0)
            {
                if (pyRes == 0.0)
                    angle = 0.0;
                else if (pyRes > 0.0) angle = System.Math.PI / 2.0;
                else

                    angle = System.Math.PI * 3.0 / 2.0;
            }
            else if (pyRes == 0.0)
            {
                if (pxRes > 0.0)
                    angle = 0.0;
                else
                    angle = System.Math.PI;
            }
            else
            {
                if (pxRes < 0.0)
                    angle = System.Math.Atan(pyRes / pxRes) + System.Math.PI;

                else if (pyRes < 0.0) angle = System.Math.Atan(pyRes / pxRes) + (2 * System.Math.PI);
                else

                    angle = System.Math.Atan(pyRes / pxRes);
            }
            // Convert to degrees 
            //angle = angle * 180 / System.Math.PI; 
            return angle;
        }

        /// <summary>
        /// Finds the projection point used within the inline annotation process.
        /// Used from the AnnoModification code provided by the Schematics team.
        /// </summary>
        /// <param name="pInputPnt">The point to use as a base for the projection.</param>
        /// <param name="Inputangle">The angle of the projection.</param>
        /// <param name="distance">The distance from the input point to project.</param>
        /// <returns>The projection point to use when inlining the target feature.</returns>
        private static IPoint InlineGetProjectionPoint(IPoint pInputPnt, double Inputangle, double distance)
        {
            IPoint pOutputPnt = new PointClass();

            try
            {
                pOutputPnt.PutCoords(pInputPnt.X + (distance * Math.Cos(Inputangle)), pInputPnt.Y + (distance * Math.Sin(Inputangle)));

                return pOutputPnt;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// The existing annotation inline code presumed that annotation elements would be polyline elements; however, this isn't always the case.
        /// Where an annotation element is an IPoint, we need to "convert" it to a polyline. We'll do this by using the angle and distance
        /// of the existing annotation to create a line that is close in position to where it already was.
        /// Angle logic is extrapolated from the GetCurrentAngle() method.
        /// </summary>
        /// <param name="feat">The annotation feature to convert.</param>
        private static void ConvertAnnotationPointToPolyline(IFeature feat)
        {
            double angleRad = 0;
            double distance = 0;
            IPoint basePoint = null;

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
                        }
                        else
                        {
                            theta = (Math.PI / 2); //90 degrees
                        }

                        angleRad = theta;
                    }

                    basePoint = (feat.Shape as IPointCollection).get_Point(0);

                    //Quadrant 1
                    if (minYIndex == 0)
                    {
                        distance = InlineGetPointDistance(minXPoint, maxYPoint);
                    }
                    //Quadrant 2
                    else if (maxXIndex == 0)
                    {
                        distance = InlineGetPointDistance(minYPoint, minXPoint);
                        angleRad += (Math.PI / 2);
                    }
                    //Quadrant 3
                    else if (maxYIndex == 0)
                    {
                        distance = InlineGetPointDistance(maxXPoint, minYPoint);
                        angleRad += Math.PI;
                    }
                    //Quadrant 4
                    else if (minXIndex == 0)
                    {
                        distance = InlineGetPointDistance(maxYPoint, maxXPoint);
                        angleRad += (3 * Math.PI / 2);
                    }
                }
            }
            catch { }


            IPoint ActProj1 = InlineGetProjectionPoint(basePoint, angleRad, distance);

            IPolyline pLine = new PolylineClass();
            pLine.ToPoint = ActProj1;
            pLine.FromPoint = basePoint;

            IAnnotationFeature2 pTarAnno = feat as IAnnotationFeature2;

            int intClassId = pTarAnno.AnnotationClassID;
            IElement pElement = pTarAnno.Annotation;
            pElement.Geometry = pLine as IGeometry;

            ITextElement pTxtElm1 = pElement as ITextElement;
            pTarAnno.Annotation = pTxtElm1 as IElement;
            pTarAnno.AnnotationClassID = intClassId;

            feat.Store();
        }

        #endregion Inline Methods
    }
}
