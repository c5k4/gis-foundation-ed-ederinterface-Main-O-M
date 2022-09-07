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
    [Guid("C84FEB6B-BEA3-4CF9-ADF7-31DB44E8BB1C")]
    [ProgId("PGE.Desktop.EDER.SumUnitCountsRelAU")]
    [ComponentCategory(ComCategory.RelationshipAutoupdateStrategy)]
    public class SumUnitCountsRelAU : BaseRelationshipAU
    {

        #region private

        private static readonly PGE.Common.Delivery.Diagnostics.Log4NetLogger _logger = new PGE.Common.Delivery.Diagnostics.Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "EDERDesktop.log4net.config");
        private static ConductorAttributeFacade _conductorAttribFacade = new ConductorAttributeFacade();

        #endregion private

       #region Constructors
        /// <summary>
        /// constructor for the SumUnitCountsRelAU
        /// </summary>
        public SumUnitCountsRelAU()
            : base("PGE Sum Unit Counts Rel AU")
        {
        }

        #endregion Constructors

        #region Relationship AU overrides
        /// <summary>
        /// update the sum of count of objects
        /// </summary>
        /// <param name="relationship">the relationship</param>
        /// <param name="auMode">the mode</param>
        /// <param name="eEvent">the event</param>
        protected override void InternalExecute(IRelationship relationship, mmAutoUpdaterMode auMode, mmEditEvent eEvent)
        {
            _conductorAttribFacade.ExecuteForSumUnitRelation(relationship, eEvent);
        }

        /// <summary>
        /// The model name "PRIMARYCONDUCTOR" or "FEEDERSECONDARY" must be on the relationship source in order to be enabled
        /// </summary>
        /// <param name="relClass">the relationship class</param>
        /// <param name="originClass">the relationship origin</param>
        /// <param name="destClass">the relationship destination</param>
        /// <param name="eEvent">the relationship event</param>
        /// <returns></returns>
        protected override bool InternalEnabled(IRelationshipClass relClass, IObjectClass originClass, IObjectClass destClass, mmEditEvent eEvent)
        {
            string[] _modelNames = new string[] { SchemaInfo.Electric.ClassModelNames.PrimaryConductor, SchemaInfo.Electric.ClassModelNames.SecondaryFeeder };
            bool enabled = ModelNameFacade.ContainsClassModelName(originClass, _modelNames);
            _logger.Debug(this._Name + ":" + enabled);
            return enabled;
        }
        #endregion Relationship AU overrides

    }
}
