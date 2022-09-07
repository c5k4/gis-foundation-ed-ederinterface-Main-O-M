using System;
using System.Reflection;
using System.Runtime.InteropServices;
using ESRI.ArcGIS.Geodatabase;
using Miner.ComCategories;
using Miner.Interop;
using PGE.Common.Delivery.ArcFM;
using PGE.Common.Delivery.Framework;

namespace PGE.Desktop.EDER.AutoUpdaters
{
    /// <summary>
    /// A relationship AU that counts related objects.
    /// </summary>
    [ComVisible(true)]
    [Guid("647C801F-8456-426C-B14A-D57970A5C0CD")]
    [ProgId("PGE.Desktop.EDER.RelatedObjectCount")]
    [ComponentCategory(ComCategory.RelationshipAutoupdateStrategy)]
    public class RelatedObjectCount : BaseRelationshipAU
    {
        #region Private
        private static readonly PGE.Common.Delivery.Diagnostics.Log4NetLogger _logger = new PGE.Common.Delivery.Diagnostics.Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "EDERDesktop.log4net.config");

        private const string _count0 = "0";
        private const string _count1 = "1";
        private const string _count2 = "2";
        private const string _count3ormore = "3 or more";

        #endregion Private

        #region Constructors
        /// <summary>
        /// constructor for the  Related Object Count AU
        /// </summary>
        public RelatedObjectCount() : base("PGE Related Object Count")
        {
        }

        #endregion Constructors

        #region RelationshipAU overrides
        /// <summary>
        /// update the count of related objects by 1 if a relationship is created, decrement by 1 if relationship is deleted/unrelated
        /// </summary>
        /// <param name="relationship">the relationship</param>
        /// <param name="auMode">the mode</param>
        /// <param name="eEvent">the event</param>
        protected override void InternalExecute(IRelationship relationship, mmAutoUpdaterMode auMode, mmEditEvent eEvent)
        {
            if (eEvent == mmEditEvent.mmEventRelationshipUpdated)
            {
                _logger.Debug("Relationship Update event no need to execute AU.");
                return;
            }
            IObjectClass objectClass = relationship.OriginObject.Class;
            //relateCountIdx will never be -1 as this AU is enabled only when FieldModelName is present.
            int relateCountIdx = ModelNameFacade.FieldIndexFromModelName(objectClass, SchemaInfo.Electric.FieldModelNames.RelateCount);
            //it is assumed that originObject is SupportStructure feature
            IObject originObject = relationship.OriginObject;
            ESRI.ArcGIS.esriSystem.ISet relatedSet = relationship.RelationshipClass.GetObjectsRelatedToObject(originObject);
            int currentCount = relatedSet.Count;
            _logger.Debug("Total No. of related objecs (" + currentCount + ").");
            string relateCount = _count1;

            //set the loop to check number of jointowners if deleted then still objectclass row is present in Iset so need to takecare.
            //only need to check relationship deleted.  
            //realtionship created already includes the new row in the unit set count.
            if (eEvent == mmEditEvent.mmEventRelationshipDeleted)
            {
                //currentCount = Convert.ToInt32(originObject.get_Value(relateCountIdx));

                _logger.Debug("Relationship Deleted.");
                IRow destRow = relationship.DestinationObject as IRow;
                if (currentCount <= 3)
                {
                    IRow unitRow;
                    for (unitRow = relatedSet.Next() as IRow; unitRow != null; unitRow = relatedSet.Next() as IRow)
                    {
                        //taking care of Relationship is deleted / unrelated.
                        if (unitRow.Equals(destRow))
                        {
                            currentCount -= 1;
                            break;
                        }
                    }
                }
            }

            //************To fix the Joint Pole bug******************
            //************Incident Number INC000003723832************

            switch (currentCount)
            {
                case 0: relateCount = _count1; break;
                case 1: relateCount = _count2; break;
                //case 2: relateCount = _count2; break;
                default:
                    relateCount = _count3ormore; break;
            }

            //*******************************************************
            //*******************************************************


            originObject.set_Value(relateCountIdx, relateCount);
            try
            {
                BaseRelationshipAU.IsRelAUCallingStore = true;
                originObject.Store();
            }
            finally
            {
                BaseRelationshipAU.IsRelAUCallingStore = false;
            }
            _logger.Debug("Successfully set the value of Related Objects Count.");
        }

        /// <summary>
        /// The model name "PGE_RELATESOURCE" and "PGE_RELATEDESTINATION" must be on the relationship source and destination respectively in order to be enabled
        /// </summary>
        /// <param name="relClass">the relationship class</param>
        /// <param name="originClass">the relationship origin</param>
        /// <param name="destClass">the relationship destination</param>
        /// <param name="eEvent">the relationship event</param>
        /// <returns></returns>
        protected override bool InternalEnabled(IRelationshipClass relClass, IObjectClass originClass, IObjectClass destClass, mmEditEvent eEvent)
        {
            bool enabled = ModelNameFacade.ContainsClassModelName(originClass, new string[] { SchemaInfo.Electric.ClassModelNames.RelateSource });
            _logger.Debug("Class with ModelName: " + SchemaInfo.Electric.ClassModelNames.RelateSource + " Found-" + enabled);
            bool destEnabled = ModelNameFacade.ContainsClassModelName(destClass, new string[] { SchemaInfo.Electric.ClassModelNames.RelateDestination });
            _logger.Debug("Class with ModelName: " + SchemaInfo.Electric.ClassModelNames.RelateDestination + " Found-" + destEnabled);
            bool fieldEnabled = ModelNameFacade.ContainsFieldModelName(originClass, new string[] { SchemaInfo.Electric.FieldModelNames.RelateCount });
            _logger.Debug("Field with ModelName: " + SchemaInfo.Electric.FieldModelNames.RelateCount + " Found-" + fieldEnabled);
            return (enabled && destEnabled && fieldEnabled);
        }

        #endregion RelationshipAU overrides

    }
}
