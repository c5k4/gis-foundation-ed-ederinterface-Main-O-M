using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

using Miner.ComCategories;
using Miner.Interop;
using PGE.Common.Delivery.ArcFM;
using PGE.Common.Delivery.Framework;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;

namespace PGE.Desktop.EDER.AutoUpdaters
{
    [Guid("8D85B001-8171-4547-9F0F-A73D225636AD")]
    [ProgId("PGE.Desktop.EDER.CalculateLatitudeLongitude")]
    [ComponentCategory(ComCategory.SpecialAutoUpdateStrategy)]
    public class CalculateLatitudeLongitude : BaseSpecialAU
    {
        public CalculateLatitudeLongitude()
            : base("PGE Calculate Latitude / Longitude")
        { }

        protected override bool InternalEnabled(IObjectClass objectClass, mmEditEvent eEvent)
        {
            return (objectClass is IFeatureClass)
                && ((IFeatureClass)objectClass).ShapeType == esriGeometryType.esriGeometryPoint
                && ModelNameFacade.ContainsAllClassModelNames(objectClass, SchemaInfo.General.ClassModelNames.LatitudeLongitude)
                && ModelNameFacade.ContainsAllFieldModelNames(objectClass, SchemaInfo.General.FieldModelNames.Latitude,
                                                                           SchemaInfo.General.FieldModelNames.Longitude);
        }

        static IPoint ProjectToLatitudeAndLongitude(IPoint point)
        {
            return GeometryFacade.Project(point,
                                          esriSRGeoCSType.esriSRGeoCS_NAD1983HARN,
                                          esriTransformDirection.esriTransformReverse);
        }

        protected override void InternalExecute(IObject obj, mmAutoUpdaterMode eAUMode, mmEditEvent eEvent)
        {
            var point = (obj as IFeature).Shape as IPoint;
            if (point == null)
            {
                return; // Shouldn't happen, due to InternalEnabled checks.
            }
            point = ProjectToLatitudeAndLongitude(point); // ME Q3 2018 requirment.
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
