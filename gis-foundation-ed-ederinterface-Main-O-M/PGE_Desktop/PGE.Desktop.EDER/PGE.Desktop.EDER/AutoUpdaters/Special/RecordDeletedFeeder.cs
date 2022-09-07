using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using Miner.ComCategories;
using PGE.Common.Delivery.ArcFM;
using PGE.Common.Delivery.Diagnostics;
using System.Reflection;
using PGE.Common.Delivery.Framework;
using ESRI.ArcGIS.Geodatabase;
using Miner;
using Miner.Interop;
using ESRI.ArcGIS.Framework;

namespace PGE.Desktop.EDER.AutoUpdaters.Special
{
    /// <summary>
    /// Records information about deleted CircuitSources, including User (USERID), Version Name, and FeederID (CircuitID), POSTDATE, ACTION=DELETE
    /// </summary>
    [ComVisible(true)]
    [Guid("BA2733C5-CECD-4694-B901-4F95628C78B3")]
    [ProgId("PGE.Desktop.EDER.RecordDeletedFeeder")]
    [ComponentCategory(ComCategory.SpecialAutoUpdateStrategy)]
    public class RecordDeletedFeeder : BaseSpecialAU
    {
        private static readonly Log4NetLogger _logger = new Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "EDERDesktop.log4net.config");

        public RecordDeletedFeeder()
            : base("PGE Record Deleted Feeder")
        {

        }

        protected override bool InternalEnabled(IObjectClass objectClass, Miner.Interop.mmEditEvent eEvent)
        {
            return eEvent == Miner.Interop.mmEditEvent.mmEventFeatureDelete
                && ModelNameFacade.ContainsClassModelName(objectClass, SchemaInfo.Electric.ClassModelNames.CircuitSource)
                && ModelNameFacade.ContainsAllFieldModelNames(objectClass, SchemaInfo.Electric.FieldModelNames.FeederID);
        }

        protected override void InternalExecute(IObject obj,
                                                Miner.Interop.mmAutoUpdaterMode eAUMode,
                                                Miner.Interop.mmEditEvent eEvent)
        {
            var featureWorkspace = (IFeatureWorkspace)((IDataset)((IRow)obj).Table).Workspace;

            var feederIdField = FieldInstance.FromModelName(obj, SchemaInfo.Electric.FieldModelNames.FeederID);
            if (feederIdField == null)
                throw new CancelEditException("Could not find Feeder ID field on object with ID " + obj.OID);
            var userId = ((IMMLogin2)Application.FindExtensionByName("MMPROPERTIESEXT")).LoginObject.UserName;

            new ChangedCircuitTable(featureWorkspace).RecordDeleted(feederIdField.Value, userId);
        }
    }
}
