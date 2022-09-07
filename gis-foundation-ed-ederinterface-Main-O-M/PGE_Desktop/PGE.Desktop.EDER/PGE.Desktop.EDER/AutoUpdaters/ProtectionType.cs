using System;
using System.Runtime.InteropServices;

using ESRI.ArcGIS.Geodatabase;

using Miner.ComCategories;
using Miner.Interop;
using PGE.Common.Delivery.ArcFM;
using PGE.Desktop.EDER.AutoUpdaters.LabelText;
using System.Linq;
using PGE.Common.Delivery.Framework;

namespace PGE.Desktop.EDER.AutoUpdaters
{
    [Guid("2447C8C5-5F75-484A-9E48-41996D018CB2")]
    [ProgId("PGE.Desktop.EDER.ProtectionType")]
    [ComponentCategory(ComCategory.RelationshipAutoupdateStrategy)]
    public class ProtectionType : BaseRelationshipAU
    {
        public ProtectionType()
            : base("PGE Populate Protection Type AU")
        { }


        protected override bool InternalEnabled(IRelationshipClass relClass, IObjectClass originClass, IObjectClass destClass, mmEditEvent eEvent)
        {
            try
            {
                return ModelNameFacade.ContainsClassModelName(destClass,   SchemaInfo.Electric.ClassModelNames.Generator)
                    && ModelNameFacade.ContainsClassModelName(originClass, SchemaInfo.Electric.ClassModelNames.ProtectiveDevice)
                    && ModelNameFacade.ContainsFieldModelName(originClass, SchemaInfo.Electric.FieldModelNames.ProtectionType) && (eEvent== mmEditEvent.mmEventRelationshipCreated);
            }
            catch (Exception ex)
            {
                PGE.Common.Delivery.Diagnostics.Logger.WriteLine(ex);
                return false;
            }
        }

        protected override void InternalExecute(IRelationship relationship, mmAutoUpdaterMode auMode, mmEditEvent eEvent)
        {
            var protectiveDevice = relationship.OriginObject;
            var generator = relationship.DestinationObject;
            try
            {
                FieldInstance protectiveDeviceProtectionType;
                protectiveDeviceProtectionType = protectiveDevice.FieldInstanceFromModelName(SchemaInfo.Electric.FieldModelNames.ProtectionType);
                if (protectiveDeviceProtectionType == null || protectiveDeviceProtectionType.Value.Convert(string.Empty).Length != 0)
                    return;
                protectiveDeviceProtectionType.StoreValue = generator.SubtypeNameContains("DC")
                                                       ? "Inverter"
                                                       : "Unspecified";
                
                
            }
            catch (Exception ex)
            {
                PGE.Common.Delivery.Diagnostics.Logger.WriteLine(ex);
            }
        }
    }
}