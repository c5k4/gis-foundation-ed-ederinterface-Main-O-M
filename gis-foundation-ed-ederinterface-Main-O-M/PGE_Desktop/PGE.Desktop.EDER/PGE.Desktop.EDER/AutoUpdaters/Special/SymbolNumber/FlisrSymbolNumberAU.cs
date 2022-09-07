using System;
using System.Reflection;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

using ESRI.ArcGIS.ArcMapUI;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geodatabase;

using Miner.ComCategories;
using Miner.Interop;
using Miner.Framework;

using PGE.Common.Delivery.ArcFM;
using PGE.Common.Delivery.Diagnostics;
using PGE.Common.Delivery.Framework;

namespace PGE.Desktop.EDER.AutoUpdaters
{
    [Guid("817aef0e-f621-4955-8c23-2b7848ff9a15")]
    [ComponentCategory(ComCategory.SpecialAutoUpdateStrategy)]
    [ComVisible(true)]
    [ProgId("PGE.Desktop.EDER.FLISRSymbolNumberAU")]
    public class FlisrSymbolNumberAU : BaseSpecialAU
    {
          #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="SymbolNumberAU"/> class.  
        /// </summary>
        public FlisrSymbolNumberAU() : base("PGE FLISR Symbol Number AU") { }
        #endregion Constructor

        #region overrides for SymbolNumberAU
        /// <summary>
        /// Enable the AU when the create feature or the update feature AU category is selected.
        /// </summary>
        /// <param name="pObjectClass">The selected object class.</param>
        /// <param name="eEvent">The AU event type.</param>
        /// <returns></returns>
        protected override bool InternalEnabled(IObjectClass pObjectClass, Miner.Interop.mmEditEvent eEvent)
        {
            _logger.Debug("Event is:" + eEvent.ToString() + ", and pObjectClass is IFeatureClass=" + (pObjectClass is IFeatureClass) + ".");
            //check if event is Feature Create or Update and pObjectClass is type of IObjectClass
            if ((eEvent == Miner.Interop.mmEditEvent.mmEventFeatureCreate ||
                eEvent == Miner.Interop.mmEditEvent.mmEventFeatureUpdate))
            {
                _logger.Debug("returning true.");
                return true;
            }

            _logger.Debug("returning false.");
            return false;

        }

        /// <summary>
        /// Executes the SymbolNumber AU.  Symbol Number AU queries 
        /// </summary>
        /// <param name="pObject">The object being updated.</param>
        /// <param name="eAUMode">The AU mode.</param>
        /// <param name="eEvent">The edit event.</param>
        protected override void InternalExecute(IObject pObject, Miner.Interop.mmAutoUpdaterMode eAUMode, Miner.Interop.mmEditEvent eEvent)
        {
            try
            {
                //Get reference to RuntimeEnvironment for ActiveView
                IMMArcGISRuntimeEnvironment rte = new ArcGISRuntimeEnvironment();

                //check ifobject received is type of IObject
               
                 if (pObject is IObject)
                {
                    _logger.Debug("pObject is type of IObject.");
                    //get the relationship classes from Relationship Role
                    _logger.Debug("Getting the relationship classes for role esriRelRoleAny.");
                    IEnumRelationshipClass relClasses = pObject.Class.get_RelationshipClasses(esriRelRole.esriRelRoleAny);
                    //reset the enum
                    relClasses.Reset();

                    IRelationshipClass relClass;
                    _logger.Debug("Iterating through all the relationship classes.");
                    //iterate through all relationship classes
                    while ((relClass = relClasses.Next()) != null)
                    {
                        _logger.Debug("Checking the OriginClass for field model name PGE_FLISR.");
                        //check if relationship class origin class cpntains model name then
                        if (ModelNameFacade.ContainsClassModelName(relClass.OriginClass, new string[] { SchemaInfo.Electric.ClassModelNames.PGESwitch, SchemaInfo.Electric.ClassModelNames.DynamicProtectiveDevice}))
                        {
                           // _logger.Debug(string.Format("PGE_SYMBOLNUMBER field model name exist for {0}.", relClass.OriginClass.AliasName));
                            //get all related objects 
                            _logger.Debug(string.Format("Getting the related objects for {0} using this relationship class.", relClass.OriginClass.AliasName));
                            ESRI.ArcGIS.esriSystem.ISet relatedObjects = relClass.GetObjectsRelatedToObject(pObject);
                            //reset the ISet
                            relatedObjects.Reset();

                            _logger.Debug(string.Format("Got related objects : {0}", relatedObjects.Count));
                            object originObject;
                            //iterate through all related objects
                            _logger.Debug("Iterating through all related objects.");

                            while ((originObject = relatedObjects.Next()) != null)
                            {
                                //check if origin object is type of IFeature
                                _logger.Debug("checking if iterating object is type of Ifeature");
                                if (originObject is IFeature)
                                {
                                    IFeature feature = originObject as IFeature;
                                    _logger.Debug("Iterating object is type of Ifeature, updating Date modified.");
                                    //then update Date Modified
                                    updateFLISR(feature);

                                   
                                    //av.Refresh();
                                }
                                else
                                {
                                    _logger.Debug("Same Symbol number found not updating field");
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Debug("exception occurred while updating symbol number.", ex);
            }
        }
        #endregion overrides for SymbolNumberAU

        #region Private

        private static readonly PGE.Common.Delivery.Diagnostics.Log4NetLogger _logger = new PGE.Common.Delivery.Diagnostics.Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "EDERDesktop.log4net.config");

        /// <summary>
        /// Gets valid symbol number for the feature passed, and updates it.
        /// </summary>
        /// <param name="feature">The feature.</param>
        private void updateFLISR(IFeature feature)
        {
           
            
            _logger.Debug("Getting field index from field model name.");
            //get the field index for field model name PGE_SYMBOLNUMBER
           
                    //set the symbol number at fieldIx position
                int fieldIx = ModelNameFacade.FieldIndexFromModelName(feature.Class, "PGE_SYMBOLNUMBER");
                _logger.Debug("Got field model name PGE_SYMBOLNUMBER at " + fieldIx + " index");
                //set the null value for field with field model neme PGE_SYMBOLNUMBER
                feature.set_Value(fieldIx, null);
                _logger.Debug("Set field with field model name PGE_SYMBOLNUMBER value to <Null>.");
                feature.Store();
                _logger.Debug("successfully set symbol number.");
            
            
        }
        #endregion Private

    }
}
