using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

using ESRI.ArcGIS.Geodatabase;

using Miner.Interop;
using Miner.ComCategories;

using PGE.Common.Delivery.ArcFM;
using PGE.Common.Delivery.Framework;
using PGE.Desktop.EDER.AutoUpdaters.LabelText;

namespace PGE.Desktop.EDER.AutoUpdaters
{
    /// A relationship AU that counts related objects.
    /// </summary>
    [ComVisible(true)]
    [Guid("3567EA31-150C-4AD3-9D7F-3CFA01628D78")]
    [ProgId("PGE.Desktop.EDER.SupportStructureJointOwnerComboRelAU")]
    [ComponentCategory(ComCategory.RelationshipAutoupdateStrategy)]
    public class SupportStructure_JointOwnerComboRelAU : BaseRelationshipAU
    {
        #region Contructors
        public SupportStructure_JointOwnerComboRelAU()
            : base("PGE SupportStructure_JointOwner Combo Rel AU")
        {

        }
        #endregion Contructors

        IMMRelationshipAUStrategy updateSymbolNumberParentAU = new SymbolNumberUpdateParentObject() as IMMRelationshipAUStrategy;
        IMMRelationshipAUStrategy relationshipLabelAU = new RelationshipLabel() as IMMRelationshipAUStrategy;
        IMMRelationshipAUStrategy relatedObjectCountAU = new RelatedObjectCount() as IMMRelationshipAUStrategy;
        IMMRelationshipAUStrategy cleanupOrphanRows = new CleanupOrphanRows() as IMMRelationshipAUStrategy;

        #region ComboAU Overrides
        /// <summary>
        /// Executes Combo AU fo Relationship Label AU and Execute Update Parent AU
        /// </summary>
        /// <param name="relationship">the relationship</param>
        /// <param name="auMode">the AU Mode.</param>
        /// <param name="eEvent">the Event (Create/Delete).</param>
        protected override void InternalExecute(IRelationship relationship, mmAutoUpdaterMode auMode, mmEditEvent eEvent)
        {
            try
            {
                //Execute Relationship Label AU
                BaseRelationshipAU.IsRunningAsRelAU = true;
                relationshipLabelAU.Execute(relationship, auMode, eEvent);
            }
            finally
            {
                BaseRelationshipAU.IsRunningAsRelAU = false;
            }

            try
            {
                //Execute Update Parent AU
                BaseRelationshipAU.IsRunningAsRelAU = true;
                updateSymbolNumberParentAU.Execute(relationship, auMode, eEvent);
            }
            finally
            {
                BaseRelationshipAU.IsRunningAsRelAU = false;
            }

            try
            {
                //Execute Update Parent AU
                BaseRelationshipAU.IsRunningAsRelAU = true;
                relatedObjectCountAU.Execute(relationship, auMode, eEvent);
            }
            finally
            {
                BaseRelationshipAU.IsRunningAsRelAU = false;
            }

            try
            {
                //Execute Cleanup JointOwner 
                BaseRelationshipAU.IsRunningAsRelAU = true;
                cleanupOrphanRows.Execute(relationship, auMode, eEvent);
            }
            finally
            {
                BaseRelationshipAU.IsRunningAsRelAU = false;
            }
        }

        /// <summary>
        /// Enabled if source feature is Transformer and Destination feature is TransformerUnit
        /// </summary>
        /// <param name="relClass">the Relationship Class.</param>
        /// <param name="originClass">the OriginClass from RelationshipClas.</param>
        /// <param name="destClass">the DestinationClass from RelationshipClas.</param>
        /// <param name="eEvent">the Event (Create/Delete).</param>
        /// <returns>True if Enabled.</returns>
        protected override bool InternalEnabled(IRelationshipClass relClass, IObjectClass originClass, IObjectClass destClass, mmEditEvent eEvent)
        {
            bool destClassEnabled = ModelNameFacade.ContainsClassModelName(destClass, new string[] { SchemaInfo.Electric.ClassModelNames.JointOwner });
            bool originClassEnabled = ModelNameFacade.ContainsClassModelName(originClass, new string[] { SchemaInfo.Electric.ClassModelNames.SupportStructure });

            return (originClassEnabled && destClassEnabled);
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
