using System;
using System.Runtime.InteropServices;

using ESRI.ArcGIS.Geodatabase;

using Miner.ComCategories;
using Miner.Interop;
using PGE.Common.Delivery.ArcFM;
using PGE.Desktop.EDER.AutoUpdaters.LabelText;

using PGE.Common.Delivery.Framework;
using PGE.Common.Delivery.Diagnostics;
using System.Reflection;

namespace PGE.Desktop.EDER.AutoUpdaters
{

    [Guid("09C90139-E0CC-4FA3-9F75-80EC25CA319D")]
    [ProgId("PGE.Desktop.EDER.GeneratorName")]
    [ComponentCategory(ComCategory.RelationshipAutoupdateStrategy)]
    public class GeneratorName : BaseRelationshipAU
    {

        private static readonly Log4NetLogger _logger = new Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "EDERDesktop.log4net.config");

        public GeneratorName()
            : base("PGE Populate Generator Name AU")
        {
        }

        protected override bool InternalEnabled(IRelationshipClass relClass, IObjectClass originClass, IObjectClass destClass, mmEditEvent eEvent)
        {
            try
            {
                return ModelNameFacade.ContainsClassModelName(originClass, SchemaInfo.Electric.ClassModelNames.Generation)
                    && ModelNameFacade.ContainsClassModelName(destClass, SchemaInfo.Electric.ClassModelNames.ServicePoint)
                    && ModelNameFacade.ContainsFieldModelName(originClass, SchemaInfo.Electric.FieldModelNames.GeneratorName)
                    && ModelNameFacade.ContainsFieldModelName(destClass, SchemaInfo.General.FieldModelNames.MailName)
                    && (eEvent == mmEditEvent.mmEventRelationshipCreated);
            }
            catch (Exception ex)
            {
                _logger.Error("PGE Populate Generator Name AU failed to check enabled condition. Assumed disabled.", ex);
                return false;
            }
        }

        protected override void InternalExecute(IRelationship relationship, mmAutoUpdaterMode auMode, mmEditEvent eEvent)
        {
            try
            {
                BaseRelationshipAU.IsRunningAsRelAU = true;
                var fromField = relationship.DestinationObject.FieldInstanceFromModelName(SchemaInfo.General.FieldModelNames.MailName);
                var toField = relationship.OriginObject.FieldInstanceFromModelName(SchemaInfo.Electric.FieldModelNames.GeneratorName);
                var currentValue = toField.Value.Convert(string.Empty);
                if (!currentValue.IsNullOrWhitespace())
                    return;
                BaseRelationshipAU.IsRelAUCallingStore = true;
                toField.StoreValue = fromField.Value;

            }
            catch (Exception ex)
            {
                _logger.Error("PGE Populate Generator Name AU execution failed.", ex);
                throw;
            }
            finally
            {
                BaseRelationshipAU.IsRelAUCallingStore = false;
                BaseRelationshipAU.IsRunningAsRelAU = false;
            }
        }
    }
}