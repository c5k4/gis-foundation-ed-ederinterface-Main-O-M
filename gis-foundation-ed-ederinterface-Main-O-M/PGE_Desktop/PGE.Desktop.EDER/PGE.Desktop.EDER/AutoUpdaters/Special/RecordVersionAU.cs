using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using Miner.ComCategories;
using PGE.Common.Delivery.ArcFM;
using PGE.Common.Delivery.Diagnostics;
using ESRI.ArcGIS.Geodatabase;
using Miner.Interop;
using System.Reflection;
using PGE.Common.Delivery.Framework;
using PGE.Desktop.EDER.Model;

namespace PGE.Desktop.EDER.AutoUpdaters.Special
{
    [ComVisible(true)]
    [Guid("D82B00C1-16DF-4382-B4A6-2FE53A2C3F46")]
    [ProgId("PGE.Desktop.EDER.RecordVersion")]
    [ComponentCategory(ComCategory.SpecialAutoUpdateStrategy)]
    public class RecordVersionAU : BaseSpecialAU
    {
        private static readonly Log4NetLogger _logger = new Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "EDERDesktop.log4net.config");

        public RecordVersionAU()
            : base("PGE Record Version") {}

        /// <summary>
        /// If the object class contains the PGE_VERSIONNAME on one of its fields and contains the
        /// PGE_EDSCHEM_CHANGEDETECTION class model name on itself, then we can act on that object class
        /// </summary>
        /// <param name="objectClass">The object class on which we are checking class/field model names</param>
        /// <param name="eEvent">The event that generated a call to the AU</param>
        /// <returns>True if the object class meets the criteria for the AU to be enabled, false otherwise</returns>
        protected override bool InternalEnabled(IObjectClass objectClass, Miner.Interop.mmEditEvent eEvent)
        {
            return ModelNameFacade.ContainsClassModelName
                (objectClass, SchemaInfo.Electric.ClassModelNames.SchematicsChangeDetection)
                && ModelNameFacade.ContainsFieldModelName
                (objectClass, SchemaInfo.Electric.FieldModelNames.VersionName);
        }

        /// <summary>
        /// This method is a wrapper that calls two different methods in the RecordVersionChange class.
        /// If the event is a create/update, the VersionName field is updated to reflect the version
        /// in which the feature is being created/updated. If the event is a delete, a table is updated
        /// to reference the deleted feature so that we can detect which feature was deleted.
        /// </summary>
        /// <param name="obj">The object that the AU is called on</param>
        /// <param name="eAUMode">AU mode, does not execute if mode is FeederManager mode</param>
        /// <param name="eEvent">Event that called the AU. The method uses this to determine what action to take</param>
        protected override void InternalExecute(IObject obj,
                                                Miner.Interop.mmAutoUpdaterMode eAUMode,
                                                Miner.Interop.mmEditEvent eEvent)
        {
            if (eAUMode != mmAutoUpdaterMode.mmAUMFeederManager)
            {

                if (eEvent == mmEditEvent.mmEventFeatureCreate || eEvent == mmEditEvent.mmEventFeatureUpdate)
                {
                    RecordVersionChange.RecordInsertUpdate(obj as IFeature);
                }
                else if (eEvent == mmEditEvent.mmEventFeatureDelete)
                {
                    //Do not execute Delete logic if status=proposed
                    int statusVal;
                    obj.get_Value(obj.Fields.FindField(SchemaInfo.Electric.Status)).TryCast<int>(out statusVal);

                    if (statusVal != SchemaInfo.Electric.Subtypes.Status.ProposedChange &&
                        statusVal != SchemaInfo.Electric.Subtypes.Status.ProposedDeactivated &&
                        statusVal != SchemaInfo.Electric.Subtypes.Status.ProposedInstall &&
                        statusVal != SchemaInfo.Electric.Subtypes.Status.ProposedRemove)
                    {
                        RecordVersionChange.RecordDelete(obj as IFeature);
                    }
                }
            }
        }
    }
}
