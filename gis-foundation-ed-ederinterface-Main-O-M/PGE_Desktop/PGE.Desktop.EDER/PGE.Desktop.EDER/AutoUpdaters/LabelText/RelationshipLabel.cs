using System;
using System.Runtime.InteropServices;
using ESRI.ArcGIS.Geodatabase;
using Miner.ComCategories;
using Miner.Interop;
using PGE.Common.Delivery.ArcFM;
using PGE.Common.Delivery.Framework;

namespace PGE.Desktop.EDER.AutoUpdaters.LabelText
{
    /// <summary>
    /// A relationship AU that executes all of the label text AUs
    /// </summary>
    [Guid("28E3B9D5-270B-40B5-958C-047757D5212E")]
    [ProgId("PGE.Desktop.EDER.RelationshipLabel")]
    [ComponentCategory(ComCategory.RelationshipAutoupdateStrategy)]
    public class RelationshipLabel : BaseRelationshipAU
    {
        #region Static members for Delete Event
        public static bool PropogateRelationshipDelete = false;
        public static int DeletedOID = -1;
        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="RelationshipLabel"/> class.
        /// </summary>
        public RelationshipLabel()
            : base("PGE Relationship LabelText AU")
        {
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Internal Enabled event for the given relationship.
        /// </summary>
        /// <param name="relClass">The relationship class.</param>
        /// <param name="originClass">The origin class.</param>
        /// <param name="destClass">The destination class.</param>
        /// <param name="eEvent">The edit event.</param>
        /// <returns>
        /// 	<c>true</c> if this AutoUpdater is enabled; otherwise <c>false</c>
        /// </returns>
        /// <remarks>This method will be called from IMMRelationshipAUStrategyEx::get_Enabled
        /// and is wrapped within the exception handling for that method.</remarks>
        protected override bool InternalEnabled(IRelationshipClass relClass, IObjectClass originClass, IObjectClass destClass, mmEditEvent eEvent)
        {
            return ModelNameFacade.ContainsClassModelName(originClass, SchemaInfo.General.ClassModelNames.LabelTextBank)
                && ModelNameFacade.ContainsClassModelName(destClass, SchemaInfo.General.ClassModelNames.LabelTextUnit);
        }

        /// <summary>
        /// Internal Execute event for the given relationship
        /// </summary>
        /// <param name="relationship">The relationship.</param>
        /// <param name="auMode">The au mode.</param>
        /// <param name="eEvent">The edit event.</param>
        /// <remarks>This method will be called from IMMRelationshipAUStrategy::Execute
        /// and is wrapped within the exception handling for that method.</remarks>
        protected override void InternalExecute(IRelationship relationship, mmAutoUpdaterMode auMode, mmEditEvent eEvent)
        {
            if (eEvent == mmEditEvent.mmEventRelationshipDeleted)
            {
                PropogateRelationshipDelete = true;
                DeletedOID = relationship.DestinationObject.OID;
            }
            IObject origObj = relationship.OriginObject;
            //Execute LabelText
            try
            {
                BaseRelationshipAU.IsRunningAsRelAU = true;
                LabelTextHelper.ExecuteAUOnParent(origObj, auMode, eEvent);
                PropogateRelationshipDelete = false;
                DeletedOID = -1;
            }
            finally
            {
                BaseRelationshipAU.IsRunningAsRelAU = false;
            }
        }

        #endregion
    }
}