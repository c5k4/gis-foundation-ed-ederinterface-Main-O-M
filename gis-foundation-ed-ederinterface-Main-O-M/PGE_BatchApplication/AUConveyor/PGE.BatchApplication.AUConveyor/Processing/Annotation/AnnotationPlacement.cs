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
    /// The individual annotation details for an annotation feature.
    /// </summary>
    public class AnnotationPlacement
    {
        public const string FLD_ANNOCLASSID = "ANNOTATIONCLASSID";
        public const string FLD_ANGLE = "ANGLE";
        public const string FLD_HORIZONTAL = "HORIZONTALALIGNMENT";

        public int AnnotationClassID;
        public double? Angle;
        public int HorizontalAlignment = -1;
        public double X;
        public double Y;

        public AnnotationPlacement(int annotationClassID, IObject annoObject)
        {
            AnnotationClassID = annotationClassID;
            //object rawAngle = annoObject.get_Value(annoObject.Fields.FindField(AnnotationPlacement.FLD_ANGLE));
            object rawAngle = AnnotationManipulation.GetCurrentAngle(annoObject as IFeature);
            double angle = 0;
            if (Double.TryParse(rawAngle.ToString(), out angle))
                Angle = angle;
            object rawHorizontalAlignment = annoObject.get_Value(annoObject.Fields.FindField(AnnotationPlacement.FLD_HORIZONTAL));
            Int32.TryParse(rawHorizontalAlignment.ToString(), out HorizontalAlignment);

            IAnnotationFeature2 annoFeat = annoObject as IAnnotationFeature2;
            IElement annoElement = annoFeat.Annotation;

            IPoint origPoint = AnnotationManipulation.GetAnchorPoint(annoFeat as IFeature, HorizontalAlignment, angle);
            X = origPoint.X;
            Y = origPoint.Y;
        }

        
    }
}
