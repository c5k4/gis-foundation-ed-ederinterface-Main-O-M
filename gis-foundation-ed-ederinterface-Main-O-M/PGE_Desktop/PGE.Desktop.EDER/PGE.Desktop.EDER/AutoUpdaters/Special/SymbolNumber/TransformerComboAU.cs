using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using Miner.ComCategories;
using PGE.Common.Delivery.ArcFM;
using ESRI.ArcGIS.Geodatabase;
using Miner.Interop;
using PGE.Desktop.EDER.AutoUpdaters.LabelText;
using PGE.Desktop.EDER.AutoUpdaters.Relationship;

namespace PGE.Desktop.EDER.AutoUpdaters
{
    /// A relationship AU that counts related objects.
    /// </summary>
    [ComVisible(true)]
    [Guid("973DFE7F-39CD-4555-A81D-D061C3D7EEA6")]
    [ProgId("PGE.Desktop.EDER.TransformerComboAU")]
    [ComponentCategory(ComCategory.RelationshipAutoupdateStrategy)]

    public class TransformerComboAU : BaseRelationshipAU
    {
        #region Contructors
        public TransformerComboAU()
            : base("PGE Transformer Relationship Combo AU")
        {

        }
        #endregion Contructors

        IMMRelationshipAUStrategy updateSymbolNumberParentAU = new SymbolNumberUpdateParentObject() as IMMRelationshipAUStrategy;
        IMMRelationshipAUStrategy relationshipLabelAU = new RelationshipLabel() as IMMRelationshipAUStrategy;
        IMMRelationshipAUStrategy relationshipBankCodeAU = new RelationshipBankCode() as IMMRelationshipAUStrategy;

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
                relationshipBankCodeAU.Execute(relationship, auMode, eEvent);
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
            string originClassName = ((IDataset)originClass).Name;
            string destClassName = ((IDataset)destClass).Name;

            return originClassName == "EDGIS.Transformer" && destClassName == "EDGIS.TransformerUnit";

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
