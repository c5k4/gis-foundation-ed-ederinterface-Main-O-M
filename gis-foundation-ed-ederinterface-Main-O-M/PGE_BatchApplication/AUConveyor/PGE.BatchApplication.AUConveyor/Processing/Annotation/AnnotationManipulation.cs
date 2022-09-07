using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;

namespace PGE.BatchApplication.AUConveyor.Processing.Annotation
{
    /// <summary>
    /// Provides methods to offset and rotate annotation.
    /// </summary>
    public static class AnnotationManipulation
    {
        public static int AnnoErrors = 0;

        /// <summary>
        /// Apply the specified angle to the given annotation based on the alignment of the feature to determine anchor point
        /// </summary>
        /// <param name="newAnno">Annotation to apply rotation to</param>
        /// <param name="alignment">Alignment of the annotation (used to calculate anchor point)</param>
        /// <param name="angle">Angle to rotate</param>
        public static void ApplyAngleRotation(IObject newAnno, int alignment, double angle, IPoint RotateAnglePoint)
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
        /// Update offsets based on the current geometry.
        /// </summary>
        /// <param name="obj">
        ///     The current object to update.
        /// </param>
        public static void UpdateOffsets(IObject obj, double xOffset, double yOffset)
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
        /// Applies the horizontal alignment to the specified feature
        /// </summary>
        /// <param name="newAnno">Annotation to apply alignment to</param>
        /// <param name="alignment">Alignment to assign</param>
        public static void ApplyHorizontalAlignment(IObject newAnno, int alignment)
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
        /// Determines the current angle of the given feature
        /// </summary>
        /// <param name="feat">IFeature to calculate current angle</param>
        /// <returns></returns>
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
                    IPoint maxXPoint = AnnotationManipulation.GetMaxXPoint(feat, ref maxXIndex);
                    IPoint maxYPoint = AnnotationManipulation.GetMaxYPoint(feat, ref maxYIndex);
                    IPoint minXPoint = AnnotationManipulation.GetMinXPoint(feat, ref minXIndex);
                    IPoint minYPoint = AnnotationManipulation.GetMinYPoint(feat, ref minYIndex);

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

        public static void ApplyAngleRotation(IFeature feature, double angle, IPoint aboutPoint)
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

        public static void SetOffsetForPoints(IFeature feature, double offsetX, double offsetY)
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
        /// Determines whether or not an anno shape exists, determining whether or not it can be manipulated.
        /// </summary>
        /// <param name="annoObj">The anno object to check.</param>
        /// <param name="annoObj">Will return <c>true</c> if the feature shape is not null, but the annotation element is.</param>
        /// <returns>Boolean indicating whether or not a shape is truly present.</returns>
        public static bool AnnoShapeExists(IObject annoObj, out bool nullAnnoElement)
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
        /// If horizontal alignment is 0, return the first point.
        /// 1 return the 5th point
        /// 2 return the 3rd point
        /// </summary>
        /// <param name="feature"></param>
        /// <param name="HorizontalAlignment"></param>
        public static IPoint GetAnchorPoint(IFeature feature, int HorizontalAlignment, double angle)
        {
            int refIntValue = -1;
            if (feature.Shape.GeometryType == esriGeometryType.esriGeometryPolygon)
            {
                IPointCollection pointCollection = feature.Shape as IPointCollection;
                if (HorizontalAlignment == 0 || HorizontalAlignment == 3)
                {
                    //Bottom right quadrant
                    if ((angle > 270 && angle < 360) || (angle > -90 && angle < 0))
                    {
                        return GetMaxYPoint(feature, ref refIntValue);
                    }
                    //Bottom left quadrant
                    else if ((angle > 180 && angle < 270) || (angle > -180 && angle < -90))
                    {
                        return GetMaxXPoint(feature, ref refIntValue);
                    }
                    //Top left quadrant
                    else if ((angle > 90 && angle < 180) || (angle > -270 && angle < -180))
                    {
                        return GetMinYPoint(feature, ref refIntValue);
                    }
                    //Top Right quadrant
                    else if ((angle > 0 && angle < 90) || (angle > -360 && angle < -270))
                    {
                        return GetMinXPoint(feature, ref refIntValue);
                    }
                    else if (angle == 0 || angle == 360 || angle == -360)
                    {
                        IPoint YPoint = GetMaxYPoint(feature, ref refIntValue);
                        IPoint XPoint = GetMinXPoint(feature, ref refIntValue);
                        IPoint newPoint = new PointClass();
                        newPoint.SpatialReference = YPoint.SpatialReference;
                        newPoint.X = XPoint.X;
                        newPoint.Y = YPoint.Y;
                        return newPoint;
                    }
                    else if (angle == 90 || angle == -270)
                    {
                        IPoint YPoint = GetMinYPoint(feature, ref refIntValue);
                        IPoint XPoint = GetMinXPoint(feature, ref refIntValue);
                        IPoint newPoint = new PointClass();
                        newPoint.SpatialReference = YPoint.SpatialReference;
                        newPoint.X = XPoint.X;
                        newPoint.Y = YPoint.Y;
                        return newPoint;
                    }
                    else if (angle == 180 || angle == -180)
                    {
                        IPoint YPoint = GetMinYPoint(feature, ref refIntValue);
                        IPoint XPoint = GetMaxXPoint(feature, ref refIntValue);
                        IPoint newPoint = new PointClass();
                        newPoint.SpatialReference = YPoint.SpatialReference;
                        newPoint.X = XPoint.X;
                        newPoint.Y = YPoint.Y;
                        return newPoint;
                    }
                    else if (angle == 270 || angle == -90)
                    {
                        IPoint YPoint = GetMaxYPoint(feature, ref refIntValue);
                        IPoint XPoint = GetMaxXPoint(feature, ref refIntValue);
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
                    IPoint YMaxPoint = GetMaxYPoint(feature, ref refIntValue);
                    IPoint XMaxPoint = GetMaxXPoint(feature, ref refIntValue);
                    IPoint YMinPoint = GetMinYPoint(feature, ref refIntValue);
                    IPoint XMinPoint = GetMinXPoint(feature, ref refIntValue);
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
                        return GetMaxXPoint(feature, ref refIntValue);
                    }
                    //Bottom left quadrant
                    else if ((angle > 180 && angle < 270) || (angle > -180 && angle < -90))
                    {
                        return GetMinYPoint(feature, ref refIntValue);
                    }
                    //Top left quadrant
                    else if ((angle > 90 && angle < 180) || (angle > -270 && angle < -180))
                    {
                        return GetMinXPoint(feature, ref refIntValue);
                    }
                    //Top Right quadrant
                    else if ((angle > 0 && angle < 90) || (angle > -360 && angle < -270))
                    {
                        return GetMaxYPoint(feature, ref refIntValue);
                    }
                    else if (angle == 0 || angle == 360 || angle == -360)
                    {
                        IPoint YPoint = GetMaxYPoint(feature, ref refIntValue);
                        IPoint XPoint = GetMaxXPoint(feature, ref refIntValue);
                        IPoint newPoint = new PointClass();
                        newPoint.SpatialReference = YPoint.SpatialReference;
                        newPoint.X = XPoint.X;
                        newPoint.Y = YPoint.Y;
                        return newPoint;
                    }
                    else if (angle == 90 || angle == -270)
                    {
                        IPoint YPoint = GetMaxYPoint(feature, ref refIntValue);
                        IPoint XPoint = GetMinXPoint(feature, ref refIntValue);
                        IPoint newPoint = new PointClass();
                        newPoint.SpatialReference = YPoint.SpatialReference;
                        newPoint.X = XPoint.X;
                        newPoint.Y = YPoint.Y;
                        return newPoint;
                    }
                    else if (angle == 180 || angle == -180)
                    {
                        IPoint YPoint = GetMinYPoint(feature, ref refIntValue);
                        IPoint XPoint = GetMinXPoint(feature, ref refIntValue);
                        IPoint newPoint = new PointClass();
                        newPoint.SpatialReference = YPoint.SpatialReference;
                        newPoint.X = XPoint.X;
                        newPoint.Y = YPoint.Y;
                        return newPoint;
                    }
                    else if (angle == 270 || angle == -90)
                    {
                        IPoint YPoint = GetMinYPoint(feature, ref refIntValue);
                        IPoint XPoint = GetMaxXPoint(feature, ref refIntValue);
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
        public static IPoint GetMaxYPoint(IFeature feature, ref int maxYIndex)
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
        public static IPoint GetMinYPoint(IFeature feature, ref int minYIndex)
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
        public static IPoint GetMaxXPoint(IFeature feature, ref int maxXIndex)
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
        public static IPoint GetMinXPoint(IFeature feature, ref int minXPoint)
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
    }
}
