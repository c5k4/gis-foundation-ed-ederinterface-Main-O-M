using System;
using System.Linq;
using System.Runtime.InteropServices;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using Miner.ComCategories;
using Miner.Interop;
using PGE.Common.Delivery.ArcFM;
using PGE.Common.Delivery.Framework;

namespace PGE.Desktop.EDER.AutoUpdaters
{
    [Guid("B1957B08-31D5-4CEC-8AE6-CB00DB915388")]
    [ProgId("PGE.Desktop.EDER.CalculateXY")]
    [ComponentCategory(ComCategory.SpecialAutoUpdateStrategy)]
    public class CalculateXY : BaseSpecialAU
    {
        public CalculateXY()
            : base("PGE Calculate X / Y")
        { }

        protected override bool InternalEnabled(IObjectClass objectClass, mmEditEvent eEvent)
        {
            return (objectClass is IFeatureClass)
                && ((IFeatureClass)objectClass).ShapeType == esriGeometryType.esriGeometryPoint
                && ModelNameFacade.ContainsAllClassModelNames(objectClass, SchemaInfo.General.ClassModelNames.LatitudeLongitude)
                && ModelNameFacade.ContainsAllFieldModelNames(objectClass, SchemaInfo.General.FieldModelNames.Latitude,
                                                                           SchemaInfo.General.FieldModelNames.Longitude);
        }      

        protected override void InternalExecute(IObject obj, mmAutoUpdaterMode eAUMode, mmEditEvent eEvent)
        {
            var point = (obj as IFeature).Shape as IPoint;
            if (point == null)
            {
                return; // Shouldn't happen, due to InternalEnabled checks.
            }
            if (point == null)
            {
                return; // Because for AU's fired in batch mode, fails.
            }
            var latitudeField = obj.GetFields(SchemaInfo.General.FieldModelNames.Latitude).FirstOrDefault();
            var longitudeField = obj.GetFields(SchemaInfo.General.FieldModelNames.Longitude).FirstOrDefault();
            if (latitudeField == null || longitudeField == null)
            {
                return; // Shouldn't happen, due to InternalEnabled checks.
            }

            longitudeField.Value = Math.Round(point.X, 8);
            latitudeField.Value = Math.Round(point.Y, 8);
        }
    }
}
