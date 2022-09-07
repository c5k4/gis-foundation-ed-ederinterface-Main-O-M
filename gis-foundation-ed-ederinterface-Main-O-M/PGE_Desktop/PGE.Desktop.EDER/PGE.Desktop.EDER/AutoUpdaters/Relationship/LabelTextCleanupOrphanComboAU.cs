using System;
using System.Reflection;
using System.Runtime.InteropServices;
using ESRI.ArcGIS.Geodatabase;
using Miner.ComCategories;
using Miner.Interop;
using PGE.Common.Delivery.ArcFM;
using PGE.Desktop.EDER.AutoUpdaters.LabelText;

namespace PGE.Desktop.EDER.AutoUpdaters.Relationship
{
    [ComVisible(true)]
    [Guid("b124cf5f-a008-4275-81c7-15d6d47255d3")]
    [ProgId("PGE.Desktop.EDER.LabelTextCleanupOrphanComboAU")]
    [ComponentCategory(ComCategory.RelationshipAutoupdateStrategy)]
    public class LabelTextCleanupOrphanComboAU : BaseRelationshipAU
    {

        #region Private
        private static readonly PGE.Common.Delivery.Diagnostics.Log4NetLogger _logger =
            new PGE.Common.Delivery.Diagnostics.Log4NetLogger(
                MethodBase.GetCurrentMethod().DeclaringType, "EDERDesktop.log4net.config");

        IMMRelationshipAUStrategy _relationshipLabelAU = new RelationshipLabel() as IMMRelationshipAUStrategy;
        IMMRelationshipAUStrategy _cleanupOrhpansRowsAU = new CleanupOrphanRows() as IMMRelationshipAUStrategy;

        #endregion Private

        #region Contructors
        public LabelTextCleanupOrphanComboAU()
            : base("Label Text / Cleanup Orphan Combo Rel AU")
        {

        }
        #endregion Contructors

        #region ComboAU Overrides
        /// <summary>
        /// Executes Combo AU for Label Text AU and Sum Unit Count AU.
        /// </summary>
        /// <param name="relationship">the relationship</param>
        /// <param name="auMode">the AU Mode.</param>
        /// <param name="eEvent">the Event (Create/Delete).</param>
        protected override void InternalExecute(IRelationship relationship, mmAutoUpdaterMode auMode, mmEditEvent eEvent)
        {
            _logger.Debug("Executing RelationshipLabel from LabelTextCleanupOrphanComboAU.");
            //Execute SumUnitCounts 
            try
            {
                BaseRelationshipAU.IsRunningAsRelAU = true;
                _relationshipLabelAU.Execute(relationship, auMode, eEvent);

                _logger.Debug("Executed RelationshipLabel from LabelTextCleanupOrphanComboAU.");
            }
            finally
            {
                BaseRelationshipAU.IsRunningAsRelAU = false;
            }

            try
            {
                _logger.Debug("Executing cleanupOrphanRowsRelAU from LabelTextCleanupOrphanComboAU..");
                //Execute LabelText
                BaseRelationshipAU.IsRunningAsRelAU = true;
                _cleanupOrhpansRowsAU.Execute(relationship, auMode, eEvent);
            }
            finally
            {
                BaseRelationshipAU.IsRunningAsRelAU = false;
                _logger.Debug("Executed cleanupOrphanRowsRelAU from LabelTextCleanupOrphanComboAU.");
            }
        }

        /// <summary>
        /// Enabled if both Label Text AU and Sum Unit Count AU are enabled.
        /// </summary>
        /// <param name="relClass">the Relationship Class.</param>
        /// <param name="originClass">the OriginClass from RelationshipClas.</param>
        /// <param name="destClass">the DestinationClass from RelationshipClas.</param>
        /// <param name="eEvent">the Event (Create/Delete).</param>
        /// <returns>True if Enabled.</returns>
        protected override bool InternalEnabled(IRelationshipClass relClass, IObjectClass originClass, IObjectClass destClass, mmEditEvent eEvent)
        {
            IMMRelationshipAUStrategyEx relLabelAU = (IMMRelationshipAUStrategyEx)_relationshipLabelAU;
            IMMRelationshipAUStrategyEx cleanupOrphAU = (IMMRelationshipAUStrategyEx)_cleanupOrhpansRowsAU;

            return (relLabelAU.get_Enabled(relClass, originClass, destClass, eEvent) &&
                cleanupOrphAU.get_Enabled(relClass, originClass, destClass, eEvent));
        }

        protected override bool IsCombo
        {
            get
            {
                return true;
            }
        }
        #endregion ComboAU Overrides
    }
}