using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using PGE.BatchApplication.AUConveyor.Utilities;

namespace PGE.BatchApplication.AUConveyor.Processing.Annotation
{
    /// <summary>
    /// Captures annotation details and contains the logic to reapply them to newly created anno features.
    /// Some attributes are reapplied commonly between all anno features and others are specific to the feature.
    /// Used in the case where anno features are deleted and reapplied to maintain attributes and properties of
    /// the previous anno while still gaining the benefits of the recreated anno.
    /// This collection is specific to one given annotation feature class.
    /// </summary>
    public class AnnotationStore : KeyedCollection<int, AnnotationPlacement>
    {
        IRelationshipClass AnnoRelClass;
        IObject ParentObject;

        double? XOffset;
        double? YOffset;
        IPoint RotateAnglePoint = null;

        #region Properties
        private double angle = 0;
        private bool angleSet = false;

        /// <summary>
        /// The angle to use for new annotation, obtained from the first populated angle in the collection
        /// </summary>
        public double Angle
        {
            get
            {
                if (!angleSet && this.Count > 0)
                {
                    var firstAnno = this.Where(a => a.Angle.HasValue);
                    if (firstAnno.Count() > 0)
                        angle = firstAnno.First().Angle.Value;

                    angleSet = true;
                }

                return angle;
            }
        }

        private int horizontalAlignment = -1;

        public int HorizontalAlignment
        {
            get
            {
                if (horizontalAlignment == -1 && this.Count > 0)
                {
                    horizontalAlignment = 0;
                    var firstAnno = this.Where(a => a.HorizontalAlignment >= 0);
                    if (firstAnno.Count() > 0)
                        horizontalAlignment = firstAnno.First().HorizontalAlignment;
                }

                return horizontalAlignment;
            }
        }
        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="annoRelClass">The relationship class between the parent object and the annotation class.</param>
        /// <param name="parentObject">The parent object that the annotation relates to.</param>
        public AnnotationStore(IRelationshipClass annoRelClass, IObject parentObject) : base()
        {
            AnnoRelClass = annoRelClass;
            ParentObject = parentObject;
        }

        protected override int GetKeyForItem(AnnotationPlacement item)
        {
            return item.AnnotationClassID;
        }

        /// <summary>
        /// Adds the object to the collection if it's an annotation feature.
        /// </summary>
        /// <param name="obj"></param>
        public void AddIfAnno(IObject obj)
        {
            if (obj is IAnnotationFeature2 && (obj as IAnnotationFeature2).Annotation != null)
            {
                int annoClassID = Convert.ToInt32(obj.get_Value(obj.Fields.FindField(AnnotationPlacement.FLD_ANNOCLASSID)).ToString());
                if (!this.Contains(annoClassID))
                    this.Add(new AnnotationPlacement(annoClassID, obj));
            }
        }

        /// <summary>
        /// Given a relationship, loops through each related feature and calculates overall offsets, then
        /// applies the collection's attributes to these features based on the annotation class ID.
        /// </summary>
        public void ApplySettingsToNewAnno()
        {
            //Leave this method if there are no settings to apply.
            if (this.Count == 0)
                return;

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
                    if (AnnotationManipulation.AnnoShapeExists(relObj, out annoError))
                    {
                        eligibleObjects.Add(relObj);
                    }
                    if (annoError)
                    {
                        LogManager.FileLogger.IndentLevel++;
                        LogManager.FileLogger.WriteLine("ERROR: This annotation element is null. The annotation was created but cannot be moved/rotated. Processing will continue. (" + (relObj.Class as IDataset).Name + " OID " + relObj.OID + ")");
                        AnnotationManipulation.AnnoErrors++;
                        LogManager.FileLogger.IndentLevel--;
                    }
                }

                //Determine the current rotation and get the anchorpoint which will be used for subsequent angle rotations.
                double currentAngle = 0.0;
                foreach (IObject relObj in eligibleObjects)
                {
                    if (RotateAnglePoint == null)
                    {
                        currentAngle = AnnotationManipulation.GetCurrentAngle(relObj as IFeature);
                        RotateAnglePoint = AnnotationManipulation.GetAnchorPoint(relObj as IFeature, HorizontalAlignment, currentAngle);
                        break;
                    }
                }

                //Apply an angle rotation to set everything to an effective angle rotation of 0
                foreach (IObject relObj in eligibleObjects)
                {
                    double adjustmentAngleToZero = 0 - currentAngle;
                    AnnotationManipulation.ApplyAngleRotation(relObj as IFeature, adjustmentAngleToZero, RotateAnglePoint);
                    relObj.Store();
                }

                //Apply the horizontal alignment to all features
                foreach (IObject relObj in eligibleObjects)
                {
                    AnnotationManipulation.ApplyHorizontalAlignment(relObj, HorizontalAlignment);
                }

                //Ajust annotations to offset them to still be aligned properly based on their horizontal alignment
                if (eligibleObjects.Count > 0)
                {
                    IObject relObj = eligibleObjects[0];
                    int annoClassID = Convert.ToInt32(relObj.get_Value(relObj.Fields.FindField("ANNOTATIONCLASSID")));
                    IPoint point1 = AnnotationManipulation.GetAnchorPoint(relObj as IFeature, HorizontalAlignment, 0);
                    //Adjust offsets to ensure the annotations are still lining up after the alignment may have changed
                    foreach (IObject relObj2 in eligibleObjects)
                    {
                        if (annoClassID != Convert.ToInt32(relObj2.get_Value(relObj2.Fields.FindField("ANNOTATIONCLASSID"))))
                        {
                            IPoint point2 = AnnotationManipulation.GetAnchorPoint(relObj2 as IFeature, HorizontalAlignment, 0);
                            double xOffset = 0.0;
                            xOffset = point1.X - point2.X;

                            double yOffset = 0.0;

                            AnnotationManipulation.UpdateOffsets(relObj2, xOffset, yOffset);
                        }
                    }
                }

                //Apply our angle rotation
                foreach (IObject relObj in eligibleObjects)
                    AnnotationManipulation.ApplyAngleRotation(relObj, HorizontalAlignment, Angle, RotateAnglePoint);
                
                //Loop 2: Apply horizontal index, get anchor point on new anno, apply angle rotation, find offsets.
                //foreach (IObject relObj in eligibleObjects)
                //    this.ApplySettingsAndRotationToNewAnno(relObj);

                //Loop 3: Apply the offsets.
                foreach (IObject relObj in eligibleObjects)
                    this.UpdateOffsets(relObj);
            }
        }

        /// <summary>
        /// Applies the collection's cached attributes to an annotation feature.
        /// </summary>
        /// <param name="newAnno">The annotation feature on which to apply attributes.</param>
        private void ApplySettingsAndRotationToNewAnno(IObject newAnno)
        {
            IAnnotationFeature2 annoFeat2 = newAnno as IAnnotationFeature2;
            IElement annoElement = annoFeat2.Annotation;
            IFeature feat = annoFeat2 as IFeature;

            if (feat.Shape != null)
            {
                if (HorizontalAlignment >= 0)
                {
                    feat.set_Value(feat.Fields.FindField(AnnotationPlacement.FLD_HORIZONTAL), HorizontalAlignment);
                    feat.Store();
                }

                if (RotateAnglePoint == null)
                    RotateAnglePoint = AnnotationManipulation.GetAnchorPoint(feat, HorizontalAlignment, 0);

                AnnotationManipulation.ApplyAngleRotation(feat, Angle, RotateAnglePoint);

                feat.Store();

                FindOffsets(feat, false);
            }
        }

        /// <summary>
        /// Find overall offset values based on the current geometry.
        /// Only updates offset values if an offset doesn't already exist.
        /// </summary>
        /// <param name="feat">The new feature on which to attempt to find offsets.</param>
        /// <param name="compareAnyClass">
        ///     Whether or not to compare any annotation class instead of looking for 
        ///     only the matching annotation class. Used if no annotation classes match
        ///     between the previous anno classes and the new classes.
        ///     (i.e. if the feature previously had classes 0 and 1, but after placement
        ///         only has class 2, this will be used as an effort to compare class 2
        ///         to one of the classes above)
        /// </param>
        public void FindOffsets(IFeature feat, bool compareAnyClass)
        {
            if (!(feat is IAnnotationFeature2))
                return;

            if (XOffset.HasValue && YOffset.HasValue)
                return;

            AnnotationPlacement comparisonPlacement = null;

            int annoClassID = Convert.ToInt32(feat.get_Value(feat.Fields.FindField(AnnotationPlacement.FLD_ANNOCLASSID)));
            if (!this.Contains(annoClassID))
            {
                if (!compareAnyClass)
                    return;
                else
                    comparisonPlacement = this.First();
            }
            else
                comparisonPlacement = this[annoClassID];

            IAnnotationFeature2 annoFeat = feat as IAnnotationFeature2;
            IGeometry currentGeometry = annoFeat.Annotation.Geometry;

            IPoint point = AnnotationManipulation.GetAnchorPoint(feat, HorizontalAlignment, Angle);
            double newX = point.X;
            double newY = point.Y;

            XOffset = comparisonPlacement.X - newX;
            YOffset = comparisonPlacement.Y - newY;
        }

        /// <summary>
        /// Update offsets based on the current geometry.
        /// </summary>
        /// <param name="obj">
        ///     The current object to update.
        /// </param>
        public void UpdateOffsets(IObject obj)
        {
            if (!(obj is IAnnotationFeature2))
                return;

            if (!XOffset.HasValue || !YOffset.HasValue)
            {
                //No matched offset was found, so take an unmatched offset.
                FindOffsets(obj as IFeature, true);
            }

            IAnnotationFeature2 annoFeat2 = obj as IAnnotationFeature2;
            IFeature feat = annoFeat2 as IFeature;

            if (feat.Shape != null)
            {
                AnnotationManipulation.SetOffsetForPoints(feat, XOffset.Value, YOffset.Value);
                feat.Store();
            }
        }
    }
}
