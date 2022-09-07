using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PGE.Common.Delivery.ArcFM;
using Miner.ComCategories;
using System.Runtime.InteropServices;
using PGE.Common.Delivery.Diagnostics;
using System.Reflection;
using PGE.Common.Delivery.Framework;
using ESRI.ArcGIS.Geodatabase;
using Miner.Interop;
using Miner;

namespace PGE.Desktop.EDER.AutoUpdaters.Special
{
    [ComVisible(true)]
    [Guid("FA225C24-2644-4D95-A05E-707A452F45D1")]
    [ProgId("PGE.Desktop.EDER.RecordDeletedNetworkFeature")]
    [ComponentCategory(ComCategory.SpecialAutoUpdateStrategy)]
    public class RecordDeletedNetworkFeature : BaseSpecialAU
    {
        private static readonly Log4NetLogger _logger = new Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "EDERDesktop.log4net.config");

        public RecordDeletedNetworkFeature()
            : base("PGE Record Deleted Network Feature")
        {

        }

        protected override bool InternalEnabled(ESRI.ArcGIS.Geodatabase.IObjectClass objectClass, Miner.Interop.mmEditEvent eEvent)
        {
            return eEvent == Miner.Interop.mmEditEvent.mmEventFeatureDelete
                // Must have TraceWeight
                && ModelNameFacade.ContainsAllFieldModelNames(objectClass, SchemaInfo.Electric.FieldModelNames.TraceWeight)
                // And either LastFedFeeder or FeederID
                && ModelNameFacade.ContainsFieldModelName(objectClass, SchemaInfo.Electric.FieldModelNames.RetainedFeederIdModelName, SchemaInfo.Electric.FieldModelNames.FeederID);
        }

        protected override void InternalExecute(ESRI.ArcGIS.Geodatabase.IObject obj, Miner.Interop.mmAutoUpdaterMode eAUMode, Miner.Interop.mmEditEvent eEvent)
        {
            var feederIdField = FieldInstance.FromModelName(obj, SchemaInfo.Electric.FieldModelNames.RetainedFeederIdModelName);

            // Default to FeederID field if no LastFedFeeder found.
            if(feederIdField == null || feederIdField.Value.Convert(string.Empty) == string.Empty)
                feederIdField = FieldInstance.FromModelName(obj, SchemaInfo.Electric.FieldModelNames.FeederID);

            if (feederIdField == null)
                throw new CancelEditException("Could not find Feeder ID field on object with ID " + obj.OID);

            // Do nothing if both fields are blank or null.
            if (feederIdField.Value.Convert(string.Empty) == string.Empty)
                return;

            var userId = ((IMMLogin2)Application.FindExtensionByName("MMPROPERTIESEXT")).LoginObject.UserName;
            var featureWorkspace = (IFeatureWorkspace)((IDataset)((IRow)obj).Table).Workspace;
            new ChangedCircuitTable(featureWorkspace).RecordDeleted(feederIdField.Value, userId);
        }
    }
}
