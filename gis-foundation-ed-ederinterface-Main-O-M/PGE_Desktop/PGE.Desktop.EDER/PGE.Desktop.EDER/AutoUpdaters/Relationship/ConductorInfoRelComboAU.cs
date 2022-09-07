using System;
using System.Reflection;
using System.Runtime.InteropServices;
using ESRI.ArcGIS.Geodatabase;
using Miner.ComCategories;
using Miner.Interop;
using PGE.Common.Delivery.ArcFM;


    
namespace PGE.Desktop.EDER.AutoUpdaters.Relationship
{
    /// <summary>
    /// A relationship AU that Runs all the AUs that should be assigned on Conductor_ConductorInfo relationships.
    /// Executes LabelText and SumUnitCount AUs.
    /// </summary>
    [ComVisible(true)]
    [Guid("F6722C5D-2CC3-4E0F-90B5-D47E0AD878B9")]
    [ProgId("PGE.Desktop.EDER.ConductorInfoRelComboAU")]
    [ComponentCategory(ComCategory.RelationshipAutoupdateStrategy)]
    public class ConductorInfoRelComboAU : BaseRelationshipAU
    {
        #region Private
        private static readonly PGE.Common.Delivery.Diagnostics.Log4NetLogger _logger = new PGE.Common.Delivery.Diagnostics.Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "EDERDesktop.log4net.config");
        IMMRelationshipAUStrategy _populateConductorPhaseAU = new PopulateConductorPhaseRelAU() as IMMRelationshipAUStrategy;
        IMMRelationshipAUStrategy _sumUnitAU = new SumUnitCountsRelAU() as IMMRelationshipAUStrategy;
        IMMRelationshipAUStrategy _labelTextAU = new LabelText.RelationshipLabel() as IMMRelationshipAUStrategy;
        IMMRelationshipAUStrategy _cleanupOrphanRows = new CleanupOrphanRows() as IMMRelationshipAUStrategy;
        #endregion Private

        #region Contructors
        public ConductorInfoRelComboAU() : base("PGE Sum Unit / Label Text / Populate Phase / Orphan Cleanup Rel AU") 
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
            _logger.Debug("Executing sumUnitCountRelAU from ConductorInfoRelComboAU.");
            //Execute SumUnitCounts 
            try
            {
                BaseRelationshipAU.IsRunningAsRelAU = true;
                _sumUnitAU.Execute(relationship, auMode, eEvent);
                
                _logger.Debug("Executed sumUnitCountRelAU from ConductorInfoRelComboAU.");
            }
            finally
            {
                BaseRelationshipAU.IsRunningAsRelAU = false;
            }

            _logger.Debug("Executing populateConductorPhaseRelAU from ConductorInfoRelComboAU..");
            //Execute PopulateConductorPhaseRelAU 
            try
            {
                BaseRelationshipAU.IsRunningAsRelAU = true;
                _populateConductorPhaseAU.Execute(relationship, auMode, eEvent);
                _logger.Debug("Executed populateConductorPhaseRelAU from ConductorInfoRelComboAU..");
            }
            finally
            {
                BaseRelationshipAU.IsRunningAsRelAU = false;
            }

            try
            {
                _logger.Debug("Executing labelTextRelAU from ConductorInfoRelComboAU..");
                //Execute LabelText
                BaseRelationshipAU.IsRunningAsRelAU = true;
                _labelTextAU.Execute(relationship, auMode, eEvent);
            }
            finally
            {
                BaseRelationshipAU.IsRunningAsRelAU = false;
                _logger.Debug("Executed labelTextRelAU from ConductorInfoRelComboAU.");
            }


            //Added by PGE IT responding to incident INC000004018909 
            try
            {
                //Execute Cleanup Orphan Rows AU
                _logger.Debug("Executing cleanupOrphanRowsAU from ConductorInfoRelComboAU.."); 
                BaseRelationshipAU.IsRunningAsRelAU = true;
                _cleanupOrphanRows.Execute(relationship, auMode, eEvent);
            }
            finally
            {
                BaseRelationshipAU.IsRunningAsRelAU = false;
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
            IMMRelationshipAUStrategyEx _populateConductorPhaseAUEx = _populateConductorPhaseAU as IMMRelationshipAUStrategyEx;
            IMMRelationshipAUStrategyEx sumUnitAUEx = _sumUnitAU as IMMRelationshipAUStrategyEx; 
            IMMRelationshipAUStrategyEx labelTextAUEx = _labelTextAU as IMMRelationshipAUStrategyEx;
            IMMRelationshipAUStrategyEx cleanupOrphanAUEx = _cleanupOrphanRows as IMMRelationshipAUStrategyEx;
            
            return (sumUnitAUEx.get_Enabled(relClass, originClass, destClass, eEvent) && 
                labelTextAUEx.get_Enabled(relClass, originClass, destClass, eEvent) &&
                _populateConductorPhaseAUEx.get_Enabled(relClass, originClass, destClass, eEvent) &&
                cleanupOrphanAUEx.get_Enabled(relClass, originClass, destClass, eEvent));
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
