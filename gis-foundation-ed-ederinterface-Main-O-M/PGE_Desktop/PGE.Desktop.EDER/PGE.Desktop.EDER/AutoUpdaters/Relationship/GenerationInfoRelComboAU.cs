using System;
using System.Reflection;
using System.Runtime.InteropServices;
using ESRI.ArcGIS.Geodatabase;
using Miner.ComCategories;
using Miner.Geodatabase;
using Miner.Interop;
using PGE.Common.Delivery.ArcFM;
using PGE.Common.Delivery.Framework;
using System.Windows.Forms;
using ESRI.ArcGIS.esriSystem;

// Changes for ENOS to SAP migration - Code changes for ENOS to SAP migration
namespace PGE.Desktop.EDER.AutoUpdaters.Relationship
{
    /// <summary>
    ///Populate GenerationInfo Rel Combo AU
    /// </summary>
    [ComVisible(true)]
    [Guid("4A42788E-E4F6-0122-E053-0AF4574D789A")]
    [ProgId("PGE.Desktop.EDER.GenerationInfoRelComboAU")]
    [ComponentCategory(ComCategory.RelationshipAutoupdateStrategy)]
    public class GenerationInfoRelComboAU : BaseRelationshipAU
    {
        #region private
        private static readonly PGE.Common.Delivery.Diagnostics.Log4NetLogger _logger = new PGE.Common.Delivery.Diagnostics.Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "EDERDesktop.log4net.config");
        IMMRelationshipAUStrategy _pgeManageGWOCrelAU = new PGEManageGWOCRelAU() as IMMRelationshipAUStrategy;
        IMMRelationshipAUStrategy _pgeUpdateLavelTextRelAU = new PGEUpdateLabelTextRelAU() as IMMRelationshipAUStrategy;

        #endregion private

        #region Constructors
        /// <summary>
        /// constructor for the Populate GenerationInfo Rel Combo AU
        /// </summary>
        public GenerationInfoRelComboAU()
            : base("PGE Manage GWOC/ PGE Update Label Text.")
        {
        }

        #endregion Constructors
        #region Relationship AU overrides
        /// <summary>
        /// PGE Manage GWOC/ PGE Update Label Text relationship auto updater will be called 
        /// </summary>
        /// <param name="relationship">the relationship</param>
        /// <param name="auMode">the mode</param>
        /// <param name="eEvent">the event</param>
        protected override void InternalExecute(IRelationship relationship, mmAutoUpdaterMode auMode, mmEditEvent eEvent)
        {
            if (eEvent == mmEditEvent.mmEventRelationshipDeleted)
            {
                _logger.Debug("Executing PGEManageGWOCRelAU from GenerationInfoRelComboAU.");
                //Execute SumUnitCounts 
                try
                {

                    BaseRelationshipAU.IsRunningAsRelAU = true;
                    _pgeManageGWOCrelAU.Execute(relationship, auMode, eEvent);

                    _logger.Debug("Executed PGEManageGWOCRelAU from GenerationInfoRelComboAU.");
                }
                finally
                {
                    BaseRelationshipAU.IsRunningAsRelAU = false;
                }
            }

            _logger.Debug("Executing PGEUpdateLabelTextRelAU from GenerationInfoRelComboAU..");
            //Execute PopulateConductorPhaseRelAU 
            try
            {
                BaseRelationshipAU.IsRunningAsRelAU = true;
                _pgeUpdateLavelTextRelAU.Execute(relationship, auMode, eEvent);
                _logger.Debug("Executed PGEUpdateLabelTextRelAU from GenerationInfoRelComboAU..");
            }
            finally
            {
                BaseRelationshipAU.IsRunningAsRelAU = false;
            }
        }
      
        /// <summary>
        /// 
        /// </summary>
        /// <param name="relClass">the relationship class</param>
        /// <param name="originClass">the relationship origin</param>
        /// <param name="destClass">the relationship destination</param>
        /// <param name="eEvent">the relationship event</param>
        /// <returns></returns>
        protected override bool InternalEnabled(IRelationshipClass relClass, IObjectClass originClass, IObjectClass destClass, mmEditEvent eEvent)
        {
            bool originClassEnabled = ModelNameFacade.ContainsClassModelName(originClass, new string[] { SchemaInfo.Electric.ClassModelNames.ServicePoint });
            bool destClassEnabled = ModelNameFacade.ContainsClassModelName(destClass, new string[] { SchemaInfo.Electric.ClassModelNames.GenerationInfo });
            _logger.Debug(string.Format("ClassModelName : {0} exist({2}) on ObjectClass{1}", SchemaInfo.Electric.ClassModelNames.ServicePoint, originClass.AliasName, originClassEnabled));
            _logger.Debug(string.Format("Returning Visible : {0}", (destClassEnabled && originClassEnabled)));
            return (destClassEnabled && originClassEnabled && (eEvent == mmEditEvent.mmEventRelationshipDeleted || eEvent == mmEditEvent.mmEventRelationshipCreated));
        }

        #endregion Relationship AU overrides

    }
}
