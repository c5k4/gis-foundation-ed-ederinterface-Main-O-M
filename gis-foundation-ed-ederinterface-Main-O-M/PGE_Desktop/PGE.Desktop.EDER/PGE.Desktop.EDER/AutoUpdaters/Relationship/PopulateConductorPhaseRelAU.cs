using System;
using System.Reflection;
using System.Runtime.InteropServices;
using ESRI.ArcGIS.Geodatabase;
using Miner.ComCategories;
using Miner.Interop;
using PGE.Common.Delivery.ArcFM;
using PGE.Common.Delivery.Framework;

namespace PGE.Desktop.EDER.AutoUpdaters.Relationship
{
    /// <summary>
    /// A relationship AU that counts related objects.
    /// </summary>
    [ComVisible(true)]
    [Guid("26B5E7CA-47D0-4C2D-A00D-DCB16771718A")]
    [ProgId("PGE.Desktop.EDER.PopulateConductorPhaseRelAU")]
    [ComponentCategory(ComCategory.RelationshipAutoupdateStrategy)]
    public class PopulateConductorPhaseRelAU : BaseRelationshipAU
    {

        #region Constructors
        /// <summary>
        /// constructor for the Populate Conductor Phase Rel AU
        /// </summary>
        public PopulateConductorPhaseRelAU()
            : base("PGE Populate Conductor Phase Rel AU")
        {
        }

        #endregion Constructors

        #region private
        private static readonly PGE.Common.Delivery.Diagnostics.Log4NetLogger _logger = new PGE.Common.Delivery.Diagnostics.Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "EDERDesktop.log4net.config");
        private static ConductorAttributeFacade _ConductorAttFacade = new ConductorAttributeFacade();
        #endregion private

        #region Relationship AU overrides
        /// <summary>
        /// update the sum of count of objects
        /// </summary>
        /// <param name="relationship">the relationship</param>
        /// <param name="auMode">the mode</param>
        /// <param name="eEvent">the event</param>
        protected override void InternalExecute(IRelationship relationship, mmAutoUpdaterMode auMode, mmEditEvent eEvent)
        {
            _logger.Debug("Executing PopulateConductorPhaseRelAU");
            _ConductorAttFacade.ExecuteForPhaseDesignationRelation(relationship, eEvent);
            _logger.Debug("Executed PopulateConductorPhaseRelAU");
        }

        /// <summary>
        /// The model name "CONDUCTORINFO" must be on the relationship dest in order to be enabled
        /// </summary>
        /// <param name="relClass">the relationship class</param>
        /// <param name="originClass">the relationship origin</param>
        /// <param name="destClass">the relationship destination</param>
        /// <param name="eEvent">the relationship event</param>
        /// <returns></returns>
        protected override bool InternalEnabled(IRelationshipClass relClass, IObjectClass originClass, IObjectClass destClass, mmEditEvent eEvent)
        {
            bool destClassEnabled = ModelNameFacade.ContainsClassModelName(destClass, new string[] { SchemaInfo.Electric.ClassModelNames.ConductorInfo });
            bool originClassEnabled = ModelNameFacade.ContainsClassModelName(originClass, new string[] { SchemaInfo.Electric.ClassModelNames.Conductor });
            _logger.Debug(string.Format("ClassModelName : {0} exist({2}) on ObjectClass{1}", SchemaInfo.Electric.ClassModelNames.ConductorInfo, destClass.AliasName, destClassEnabled));
            _logger.Debug(string.Format("ClassModelName : {0} exist({2}) on ObjectClass{1}", SchemaInfo.Electric.ClassModelNames.Conductor, originClass.AliasName, originClassEnabled));
            _logger.Debug(string.Format("Returning Visible : {0}", (destClassEnabled && originClassEnabled)));
            return (destClassEnabled && originClassEnabled);
        }

        #endregion Relationship AU overrides

    }
}
