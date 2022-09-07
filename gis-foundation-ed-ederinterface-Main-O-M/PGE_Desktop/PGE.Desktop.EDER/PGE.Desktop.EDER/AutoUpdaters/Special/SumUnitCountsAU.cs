using System;
using System.Reflection;
using System.Runtime.InteropServices;
using ESRI.ArcGIS.Geodatabase;

using Miner.ComCategories;

using PGE.Common.Delivery.ArcFM;
using PGE.Common.Delivery.Framework;

using PGE.Desktop.EDER.AutoUpdaters.LabelText;

namespace PGE.Desktop.EDER.AutoUpdaters.Special
{
    /// <summary>
    /// Reads XML cofiguration documents from the database and selects a symbol number for a updated object by evaluating the rules defined within.
    /// </summary>
    [Guid("DAD9561C-2833-4936-9631-6492E365EFD3")]
    [ProgId("PGE.Desktop.EDER.SumUnitCountsAU")]
    [ComponentCategory(ComCategory.SpecialAutoUpdateStrategy)]
    [ComVisible(true)]
    public class SumUnitCountsAU : BaseSpecialAU
    {

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="SumUnitCountsAU"/> class.  
        /// </summary>
        public SumUnitCountsAU() : base("PGE Sum Unit Counts AU") { }
        #endregion Constructors

        #region Privates
        
        private static readonly PGE.Common.Delivery.Diagnostics.Log4NetLogger _logger = new PGE.Common.Delivery.Diagnostics.Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "EDERDesktop.log4net.config");
        private static ConductorAttributeFacade _conductorAttribFacade = new ConductorAttributeFacade();
        
        #endregion Privates

        #region Special AU Overrides
        /// <summary>
        /// Enable the AU when the update object AU category is selected.
        /// </summary>
        /// <param name="pObjectClass">The selected object class.</param>
        /// <param name="eEvent">The AU event type.</param>
        /// <returns></returns>
        protected override bool InternalEnabled(IObjectClass pObjectClass, Miner.Interop.mmEditEvent eEvent)
        {
            string[] modelNames = new string[] { SchemaInfo.Electric.ClassModelNames.ConductorInfo };
            bool enabled = ModelNameFacade.ContainsClassModelName(pObjectClass, modelNames);

            if ((eEvent == Miner.Interop.mmEditEvent.mmEventFeatureUpdate) && (enabled))
            {
                return true;
            }
            return false;
        }

        protected override bool CanExecute(Miner.Interop.mmAutoUpdaterMode eAUMode)
        {
            if (eAUMode == Miner.Interop.mmAutoUpdaterMode.mmAUMArcMap)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Executes the SumUnitCounts AU.
        /// </summary>
        /// <param name="pObject">The object being updated.</param>
        /// <param name="eAUMode">The AU mode.</param>
        /// <param name="eEvent">The edit event.</param>
        protected override void InternalExecute(IObject pObject, Miner.Interop.mmAutoUpdaterMode eAUMode, Miner.Interop.mmEditEvent eEvent)
        {
            //Do not run LabelText AUs again in response ot Rel AU or Unit AU .Store() call on parent feature.
            if ((BaseRelationshipAU.IsRunningAsRelAU == true && BaseRelationshipAU.IsRelAUCallingStore == true) ||
                (BaseLabelTextAU.IsRunningAsUnitAU == true && BaseSpecialAU.IsUnitCallingStore == true))
            {
                _logger.Debug("Relationship AU or Unit AU is calling store.  No need to update, exiting.");
                return;
            }

            _conductorAttribFacade.ExecuteForSumUnitInfoUpdate(pObject, eEvent);

        }
        #endregion Special AU Overrides
    }
}
