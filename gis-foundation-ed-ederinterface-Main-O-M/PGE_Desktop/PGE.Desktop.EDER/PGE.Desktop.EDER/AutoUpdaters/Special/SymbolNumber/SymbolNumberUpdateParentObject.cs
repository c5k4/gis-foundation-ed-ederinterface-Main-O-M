using System;
using System.Reflection;
using System.Runtime.InteropServices;
using ESRI.ArcGIS.Geodatabase;
using Miner.ComCategories;
using PGE.Common.Delivery.ArcFM;
using PGE.Common.Delivery.Diagnostics;
using PGE.Common.Delivery.Framework;
namespace PGE.Desktop.EDER.AutoUpdaters
{
    /// <summary>
    /// Reads XML configuration documents from the database and selects a symbol number for a new or updated feature by evaluating the rules defined within.
    /// </summary>
    [Guid("2924A74B-291A-4A25-ABCF-EFFC991481DD")]
    [ProgId("PGE.Desktop.EDER.SymbolNumberUpdateParentObject")]
    [ComponentCategory(ComCategory.RelationshipAutoupdateStrategy)]
    [ComVisible(true)]
    public class SymbolNumberUpdateParentObject : BaseRelationshipAU
    {
        #region Private
        /// <summary>
        /// logger to log all the information, warning and errors
        /// </summary>
        private static readonly PGE.Common.Delivery.Diagnostics.Log4NetLogger _logger = new PGE.Common.Delivery.Diagnostics.Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "EDERDesktop.log4net.config");
        #endregion Private

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="SymbolNumberAU"/> class.  
        /// </summary>
        public SymbolNumberUpdateParentObject() : base("PGE Symbol Number Update Related Feature AU") { }
        #endregion Constructor

        #region Override for Relationship AU
        /// <summary>
        /// Determine if this relationship AU is visible in selected relationship class.
        /// </summary>
        /// <param name="relClass">The relationship class.</param>
        /// <param name="originClass">The origin class.</param>
        /// <param name="destClass">The destinatin class.</param>
        /// <param name="eEvent">The event.</param>
        /// <returns></returns>
        protected override bool InternalEnabled(IRelationshipClass relClass, IObjectClass originClass, IObjectClass destClass, Miner.Interop.mmEditEvent eEvent)
        {
            _logger.Debug("Checking for enabled conditions.");
            //check if event is either relationship created or deleted and object is type of Ifeatureclass
            if ((eEvent == Miner.Interop.mmEditEvent.mmEventRelationshipCreated ||
                eEvent == Miner.Interop.mmEditEvent.mmEventRelationshipDeleted) &&
                (originClass is IFeatureClass))
            {
                _logger.Debug("Enabled conditions are satisfied.");
                return true;
            }

            _logger.Debug("Enabled conditions are not satisfied.");
            return false;
        }

        /// <summary>
        /// Executes the AU logic based on certain conditions.
        /// </summary>
        /// <param name="relationship">The relationship.</param>
        /// <param name="auMode">The auto updater mode.</param>
        /// <param name="eEvent">The event.</param>
        protected override void InternalExecute(IRelationship relationship, Miner.Interop.mmAutoUpdaterMode auMode, Miner.Interop.mmEditEvent eEvent)
        {
            try
            {
                //This AU is not required during the PGE Asset Replacement 
                if (EvaluationEngine.Instance.EnableSymbolNumberAU)
                   return;

                //check if event which caused this execution is Relationship Delete
                if (eEvent == Miner.Interop.mmEditEvent.mmEventRelationshipDeleted)
                {
                    //lock the oid for deletion
                    _logger.Debug("Adding OID:" + (relationship.DestinationObject.OID) + " to lock for deletion.");
                    RelatedObjectFacade.Instance.addOID(relationship.DestinationObject.Class, (relationship.DestinationObject.OID));
                }
                //get the field index of field model name PGE_SYMBOLNUMBER
                int fieldIx = ModelNameFacade.FieldIndexFromModelName(relationship.OriginObject.Class, "PGE_SYMBOLNUMBER");
                _logger.Debug("Got field model name PGE_SYMBOLNUMBER at " + fieldIx + " index");
                //set the null value for field with field model neme PGE_SYMBOLNUMBER
                relationship.OriginObject.set_Value(fieldIx, null);
                _logger.Debug("Set field with field model name PGE_SYMBOLNUMBER value to <Null>.");

                /*
                EvaluationEngine.Instance.GetSymbolNumber(relationship.OriginObject as IFeature);

                int fieldIx = ModelNameFacade.FieldIndexFromModelName(relationship.OriginObject.Class, "PGE_SYMBOLNUMBER");

                if (symbolNumber >= 0)
                {
                    relationship.OriginObject.set_Value(fieldIx, symbolNumber);
                }
                else
                {
                    relationship.OriginObject.set_Value(fieldIx, DBNull.Value);
                }*/
                //execute store on origin object
                try
                {
                    BaseRelationshipAU.IsRelAUCallingStore = true;
                    relationship.OriginObject.Store();
                }
                finally
                {
                    BaseRelationshipAU.IsRelAUCallingStore = false;
                }

                if (eEvent == Miner.Interop.mmEditEvent.mmEventRelationshipDeleted)
                {
                    //unlock the oid for deletion
                    _logger.Debug("Removing OID:" + (relationship.DestinationObject.OID) + " from lock for deletion.");
                    RelatedObjectFacade.Instance.removeOID(relationship.DestinationObject.Class, (relationship.DestinationObject.OID));
                }

            }
            catch (Exception ex)
            {
                //log the exception occurred if any
                _logger.Warn("Error occurred while executing SymbolNumberRelAU.", ex);
            }
        }

        #endregion Override for Relationship AU
    }
}
