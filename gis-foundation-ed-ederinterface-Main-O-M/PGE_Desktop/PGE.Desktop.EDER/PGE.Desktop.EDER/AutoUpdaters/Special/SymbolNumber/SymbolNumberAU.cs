using System;
using System.Reflection;
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
    /// <summary>
    /// Reads XML cofiguration documents from the database and selects a symbol number for a new or updated feature by evaluating the rules defined within.
    /// </summary>
    [Guid("4716A42E-A35B-4DB8-B365-54B1340CDBD0")]
    [ProgId("PGE.Desktop.EDER.SymbolNumberAU")]
    [ComponentCategory(ComCategory.SpecialAutoUpdateStrategy)]
    [ComVisible(true)]
    public class SymbolNumberAU : BaseSpecialAU
    {
        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="SymbolNumberAU"/> class.  
        /// </summary>
        public SymbolNumberAU() : base("PGE Symbol Number AU") { }
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
                
                //check ifobject received is type of IFeature
                if (pObject is IFeature)
                {
                    _logger.Debug("pObject is type of Ifeature, updating symbol number.");
                    //then update symbolnumber
                    if (updateSymbolNumber(pObject as IFeature, false))
                    {
                        _logger.Debug("Updated symbol number.");

                        _logger.Debug("Partially refreshing the active view.");
                        //partially refresh the activeview
                        if (rte != null && rte.ActiveView != null)
                        {
                            IActiveView av = rte.ActiveView;
                            av.PartialRefresh(esriViewDrawPhase.esriViewAll, (pObject as IFeature).Extent, null);
                            _logger.Debug("Partial refresh competed.");
                        }
                    }
                    else
                    {
                        _logger.Debug("Same Symbol number found not updating field");
                    }
                    //av.Refresh();
                }
                else if (pObject is IObject)
                {
                    _logger.Debug("pObject is type of IObject.");
                    //get the relationship classes from Relationship Role
                    _logger.Debug("Getting the relationship classes for role esriRelRoleDestination.");
                    IEnumRelationshipClass relClasses = pObject.Class.get_RelationshipClasses(esriRelRole.esriRelRoleDestination);
                    //reset the enum
                    relClasses.Reset();

                    IRelationshipClass relClass;
                    _logger.Debug("Iterating through all the relationship classes.");
                    //iterate through all relationship classes
                    while ((relClass = relClasses.Next()) != null)
                    {
                        _logger.Debug("Checking the OriginClass for field model name PGE_SYMBOLNUMBER.");
                        //check if relationship class origin class cpntains model name then
                        if (ModelNameFacade.ContainsFieldModelName(relClass.OriginClass, new string[] { "PGE_SYMBOLNUMBER" }))
                        {
                            _logger.Debug(string.Format("PGE_SYMBOLNUMBER field model name exist for {0}.", relClass.OriginClass.AliasName));
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

                                    _logger.Debug("Iterating object is type of Ifeature, updating symbol number.");
                                    //then update symbolnumber
                                    if (updateSymbolNumber(originObject as IFeature, true))
                                    {
                                        _logger.Debug("Updated symbol number.");

                                        _logger.Debug("Partially refreshing the active view.");
                                        //partially refresh the active view
                                        IActiveView av = rte.ActiveView;
                                        av.PartialRefresh(esriViewDrawPhase.esriViewAll, (originObject as IFeature).Extent, null);
                                        _logger.Debug("Partial refresh completed.");
                                    }
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
        private bool updateSymbolNumber(IFeature feature, bool callStore)
        {
            bool retVal = false;
            _logger.Debug("Getting symbol number based on criteria from EvaluationEngine.");
            //get the valid symbol number depend on criteria defined in external xml config
            int symbolNumber = EvaluationEngine.Instance.GetSymbolNumber(feature);
            _logger.Debug("Got Symbol number from EvaluationEngine:" + symbolNumber);

            _logger.Debug("Getting field index from field model name.");
            //get the field index for field model name PGE_SYMBOLNUMBER
            int fieldIx = ModelNameFacade.FieldIndexFromModelName(feature.Class, "PGE_SYMBOLNUMBER");
            _logger.Debug("Got field index from model name:" + fieldIx);

            if (fieldIx > -1)
            {
                object origValue = feature.get_Value(fieldIx);
                if (origValue != null || origValue != DBNull.Value)
                {
                    if (origValue.ToString().ToUpper() == symbolNumber.ToString())
                        return retVal;
                }
                //check if SYMBOLNUMBER value is >0 (only valid value).
                if (symbolNumber >= 0)
                {
                    //set the symbol number at fieldIx position
                    _logger.Debug("setting value of symbol number field to" + symbolNumber + ".");
                    feature.set_Value(fieldIx, symbolNumber);
                    if (callStore) feature.Store();
                    retVal = true;
                }
                else
                {
                    //if valid symbolnumber is not found set null to the fieldix position
                    _logger.Debug("setting value of symbol number field to <Null>.");
                    feature.set_Value(fieldIx, DBNull.Value);
                    if (callStore) feature.Store();
                    retVal = true;
                }
                _logger.Debug("successfully set symbol number.");
            }
            return retVal;
        }
        #endregion Private
    }
}
