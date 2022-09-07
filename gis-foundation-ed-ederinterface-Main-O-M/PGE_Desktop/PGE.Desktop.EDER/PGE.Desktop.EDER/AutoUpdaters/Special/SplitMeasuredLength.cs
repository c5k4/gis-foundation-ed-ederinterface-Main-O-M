using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.esriSystem;

using Miner.Interop;
using Miner.ComCategories;

namespace PGE.Desktop.EDER.AutoUpdaters
{
    /// <summary>
    /// Split Measured length proportionally.
    /// </summary>
    [ComVisible(true)]
    [Guid("C834D14B-16A8-4CA0-B1AA-EF468E4223B5")]
    [ProgId("PGE.Desktop.EDER.SplitMeasuredLength")]
    [ComponentCategory(ComCategory.SpecialAutoUpdateStrategy)]
    public class SplitMeasuredLength : BaseSplitAU
    {

        #region Constuctor

        public SplitMeasuredLength() : base("PGE Split Measured Length") { }

        #endregion

        #region BaseSplitAU Overrides

        protected override void InternalExecuteExtended(IObject obj, mmAutoUpdaterMode eAUMode, mmEditEvent eEvent)
        {
            if (eEvent == mmEditEvent.mmEventBeforeFeatureSplit) { return; }

            IMMSplitCGO splitCGO = obj as IMMSplitCGO;
            if (splitCGO == null) return;

            //get shape length value of ORIGINAL feature
            int shapFieldIdx = obj.Fields.FindField(((IFeatureClass)obj.Class).ShapeFieldName);
            object originalShape = _originalObjectFields[shapFieldIdx];
            object originalOID = _originalObjectFields[0];

            FieldInstance originalMeasuredLengthField = obj.FieldInstanceFromModelName(SchemaInfo.General.FieldModelNames.MeasuredLength);
            object origMLValue = _originalObjectFields[originalMeasuredLengthField.Index];

            if (origMLValue == DBNull.Value || Convert.ToInt32(origMLValue) <= 1)
            {
                _logger.Debug("Original Measured Length is null or too small to split.");
                return;
            }

            double originalMeasuredLength = Convert.ToDouble(origMLValue);

            double originalShapeLength = 0;
            if (originalShape is IPolyline)
            {
                originalShapeLength = ((IPolyline)originalShape).Length;
                _logger.Debug("OriginalFeature OID: " + _originalObjectFields[0].ToString() + " | Shape Length: " + originalShapeLength.ToString() + " | Line Length: " + originalMeasuredLength.ToString());
            }

            //loop through the new edges and calculate the new measuredlength
            ISet newEdges = splitCGO.GetNewEdges();
            newEdges.Reset();
            IObject newEdge = newEdges.Next() as IObject;

            while (newEdge != null)
            {
                object newShape = ((IFeature)newEdge).ShapeCopy;

                //calculate percentage from split segment
                //originalShapLength = Convert.ToDouble(_originalObjectFields[shapFieldIdx]);
                double newShapeLength = 0;
                if (newShape is IPolyline)
                {
                    newShapeLength = ((IPolyline)newShape).Length;
                    _logger.Debug("NewFeature OID: " + newEdge.OID.ToString() + " | ShapLength: " + newShapeLength.ToString());
                }

                double percent = newShapeLength / originalShapeLength;

                int newMeasuredLength = Convert.ToInt32(Math.Round(originalMeasuredLength * percent, 0, MidpointRounding.AwayFromZero));

                _logger.Debug("Percent of original Line: " + percent.ToString() + " | New Measured Length: " + newMeasuredLength.ToString());

                FieldInstance newMeasuredLengthField = newEdge.FieldInstanceFromModelName(SchemaInfo.General.FieldModelNames.MeasuredLength);

                //set value on new feature.
                //newEdge.set_Value(newMeasuredLengthField.Index, newMeasuredLength as object);
                newMeasuredLengthField.Value = newMeasuredLength;

                newEdge = (IObject)newEdges.Next();
            }
        }

        #endregion
    }
}
