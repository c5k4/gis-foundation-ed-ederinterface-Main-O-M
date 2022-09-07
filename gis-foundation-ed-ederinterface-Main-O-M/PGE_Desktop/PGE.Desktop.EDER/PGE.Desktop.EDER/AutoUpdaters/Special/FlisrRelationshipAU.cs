using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using Miner.ComCategories;
using Telvent.Delivery.ArcFM;
using Telvent.Delivery.Framework;
using ESRI.ArcGIS.Geodatabase;
using Miner.Interop;
using Telvent.PGE.ED.Desktop.AutoUpdaters.LabelText;

namespace Telvent.PGE.ED.Desktop.AutoUpdaters
{
    [ComVisible(true)]
    [Guid("810b7ca1-84fb-422f-aadb-de9b8c8a60bc")]    
    [ProgId("Telvent.PGE.ED.Desktop.AutoUpdaters.Special.FlisrRelationshipAU")]
    [ComponentCategory(ComCategory.RelationshipAutoupdateStrategy)]
    public class FlisrRelationshipAU : BaseRelationshipAU
    {

        #region Contructors
        public FlisrRelationshipAU()
            : base("PGE FLISR Relationship AU")
        {

        }
        #endregion Contructors


        #region AU Overrides
        /// <summary>
        /// Executes Combo AU fo Relationship Label AU and Execute Update Parent AU
        /// </summary>
        /// <param name="relationship">the relationship</param>
        /// <param name="auMode">the AU Mode.</param>
        /// <param name="eEvent">the Event (Create/Delete).</param>
        protected override void InternalExecute(IRelationship relationship, mmAutoUpdaterMode auMode, mmEditEvent eEvent)
        {
            //Execute Relationship Label AU
            if ((relationship.OriginObject.HasModelName(SchemaInfo.Electric.ClassModelNames.Switch)) || 
                (relationship.OriginObject.HasModelName(SchemaInfo.Electric.ClassModelNames.DynamicProtectiveDevice)) || 
                (relationship.OriginObject.HasModelName(SchemaInfo.Electric.ClassModelNames.CapacitorBank))) 
            {
                //get the field index of field model name PGE_SYMBOLNUMBER
                int fieldIx = ModelNameFacade.FieldIndexFromModelName(relationship.OriginObject.Class, "PGE_SYMBOLNUMBER");
                if (fieldIx != -1)
                {
                    //set the null value for field with field model neme PGE_SYMBOLNUMBER
                    relationship.OriginObject.set_Value(fieldIx, null);
                    relationship.OriginObject.Store();
                }
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
            //enable for relationship create / delete 
            if ((eEvent == Miner.Interop.mmEditEvent.mmEventRelationshipCreated ||
                eEvent == Miner.Interop.mmEditEvent.mmEventRelationshipDeleted))
            {
                return true;
            }
            return false;
        }

        #endregion AU Overrides
    }
}
