using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

using Miner.Interop;
using Miner.Geodatabase;
using Miner.ComCategories;

using PGE.Common.Delivery.ArcFM;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.esriSystem;

namespace PGE.Desktop.EDER.ValidationRules
{
    [ComVisible(true)]
    [Guid("C68F5051-B5BD-452E-A24E-9846BA3C5D98")]
    [ProgId("PGE.Desktop.EDER.CapacitorAnnotation")]
    [ComponentCategory(ComCategory.MMValidationRules)]
    public class CapacitorAnnotation : BaseValidationRule
    {
        public CapacitorAnnotation()
            : base("PGE Validate Capacitor Annotation", SchemaInfo.Electric.ClassModelNames.CapacitorBank)
        { }

        protected override ID8List InternalIsValid(IRow row)
        {

            var obj = row as IObject;
            if (obj == null)
            {
                return _ErrorList;
            }

            var relatedAnno = obj.GetRelatedObjects(null, modelNames: SchemaInfo.General.ClassModelNames.Annotation).Any();
            if (!relatedAnno) AddError("CapacitorBank should have a related annotation feature.");

            return _ErrorList;
        }
    }
}
