using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using ESRI.ArcGIS.Geodatabase;
using Miner.Interop;
using PGE.Common.Delivery.Diagnostics;
using PGE.Desktop.EDER.Utility;

namespace PGE.Desktop.EDER.AutoUpdaters.Substation.BaseClasses
{
    public enum AnnoHorizontalAlignment
    {
        Left = 0,
        Center = 1,
        Right = 2,
        Full = 3
    }

    public abstract class SubstationLabelTextSpecialAU : IMMSpecialAUStrategyEx
    {
        #region Constants
        #region Field Constants
        public const string FLD_ANNO_LABELTEXT = "TEXTSTRING";
        public const string FLD_ANNO_HORIZTONALALIGNMENT = "HORIZONTALALIGNMENT";
        #endregion

        protected readonly string OVERFLOW_TEXT = " [...]";
        #endregion

        #region Readonly Variables
        private static readonly Log4NetLogger _logger = new Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "EDERDesktop.log4net.config");
        #endregion

        bool _storeObject = false;

        #region Abstract Properties
        /// <summary>
        /// The name of this Substation AU.
        /// </summary>
        public abstract string AutoUpdaterName { get; }

        /// <summary>
        /// The model name of the parent feature - the feature that the annotation is linked to and that is used as a base for the AU.
        /// </summary>
        public abstract string UniqueFeatureModelName { get; }

        /// <summary>
        /// The horizontal alignment of this annotation.
        /// </summary>
        public abstract AnnoHorizontalAlignment AnnoAlignment { get; }
        #endregion

        public SubstationLabelTextSpecialAU()
        {
            // Clear caches.
            RelationshipManager.ClearRelCache();
            FieldManager.ClearIdxCache();
        }

        /// <param name="forceStore"><c>true</c> if the object should be stored (in the event of a manual execution)</param>
        public SubstationLabelTextSpecialAU(bool forceStore)
        {
            _storeObject = forceStore;
        }

        #region Abstract Methods
        /// <summary>
        /// Creates the new label text value for the given feature.
        /// </summary>
        /// <param name="featureObject">The parent object - the feature that the annotation is attached to.</param>
        /// <returns>A value to place into the LABELTEXT field of the feature.</returns>
        public abstract string GenerateLabelText(IObject featureObject);

        /// <summary>
        /// Checks to ensure that all fields from the object which are needed to determine label text are found.
        /// </summary>
        /// <param name="featureObject">The parent object - the feature that the annotation is attached to.</param>
        /// <returns><c>true</c> if all fields were found, otherwise <c>false</c></returns>
        public abstract bool AllFieldsFound(IObjectClass featureObjectClass);
        #endregion

        #region Standard AU Methods
        public void Execute(IObject executingObject, mmAutoUpdaterMode auMode, mmEditEvent editEvent)
        {
            if (auMode == mmAutoUpdaterMode.mmAUMFeederManager)
                return;

            try
            {
                _logger.Debug("Starting " + Name + ".");

                // Traverse to the bank from wherever this method was called, and if necessary, update the indicator to show whether or not to call Store().
                bool fromParent = true;
                if (!FindSubAUParent(UniqueFeatureModelName, ref executingObject, out fromParent))
                {
                    _logger.Warn("Couldn't find the parent for this object: " + executingObject.Class.AliasName + " OID:" + executingObject.OID);
                    return;
                }
                if (!fromParent)
                    _storeObject = true;

                _logger.Debug("The calling object was " + (fromParent ? "" : "not ") + "the qualified parent object. The parent feature will " + (_storeObject ? "" : "not ") + "be stored due to execution context.");

                string labelText = GenerateLabelText(executingObject);

                _logger.Debug("LABELTEXT constructed:" + (Environment.NewLine + labelText.ToString() + Environment.NewLine).Replace(Environment.NewLine, Environment.NewLine + "    "));

                // We have our string. Let's set the new LABELTEXT value.
                string fldNameLabelText = ModelNames.Manager.FieldNamesFromModelName(executingObject.Class, ModelNames.F_LABELTEXT).Next();
                string originalLabelTextValue = FieldManager.GetValue(executingObject, fldNameLabelText).ToString();
                // Make sure this doesn't exceed our field length - shouldn't be seen with good field lengths but should be included anyway.
                int maxFieldLength = executingObject.Fields.get_Field(FieldManager.GetIndex(executingObject.Class, fldNameLabelText)).Length;
                string newLabelTextValue = labelText.ToString();
                if (newLabelTextValue.Length > maxFieldLength)
                {
                    newLabelTextValue = labelText.ToString().Substring(0, maxFieldLength - OVERFLOW_TEXT.Length) + OVERFLOW_TEXT;
                    _logger.Warn("The default value of the labeltext field has exceeded the field's maximum length and will be truncated.");
                }
                executingObject.set_Value(FieldManager.GetIndex(executingObject.Class, fldNameLabelText), newLabelTextValue);
                if (_storeObject)
                {
                    // Turn off AUs.
                    AUDisabler AUD = new AUDisabler();
                    AUD.AUsEnabled = false;

                    executingObject.Store();

                    // Turn AUs back on.
                    AUD.AUsEnabled = true;
                }

                _logger.Debug("The label text has been set.");

                UpdateAnnoAlignment(executingObject, originalLabelTextValue);

                if (originalLabelTextValue != newLabelTextValue && auMode == mmAutoUpdaterMode.mmAUMArcMap)
                {
                    _logger.Warn("Updating Anno...");
                    executingObject.RefreshAnnotation();
                }

            }
            catch (Exception e)
            {
                _logger.Error("Exception in Execute() for " + Name + ":" + e.ToString());
            }
            finally
            {
                _logger.Debug("Finished executing " + Name + ".");
            }
        }

        public bool get_Enabled(IObjectClass objClass, mmEditEvent editEvent)
        {
            try
            {
                _logger.Debug("Checking Enabled for " + Name + " on object class " + objClass.AliasName + ".");

                // Get topmost parent class, or return false if we can't find it.
                bool fromParent = true;
                if (!FindSubAUParentClass(UniqueFeatureModelName, ref objClass, out fromParent)) return false;

                if (!objClass.HasOID) return false;

                //Ensure that label text field exists.
                if (ModelNames.Manager.FieldNamesFromModelName(objClass, ModelNames.F_LABELTEXT).Next() == null) return false;

                // Clear caches.
                RelationshipManager.ClearRelCache();
                FieldManager.ClearIdxCache();

                if (!AllFieldsFound(objClass)) return false;

                _logger.Debug("Everything checks out.");
                return true;
            }
            catch (Exception e)
            {
                _logger.Error("Exception in Enabled() for " + Name + ":" + e.ToString());
                return false;
            }
            finally
            {
                _logger.Debug("Finished checking enabled for " + Name + ".");
            }
        }
        #endregion

        #region Standard AU Properties
        public string Name
        {
            get { return AutoUpdaterName; }
        }
        #endregion

        #region Anno Alignment
        /// <summary>
        /// Due to an anno bug, update the horizontal alignment for the anno being added to this feature.
        /// </summary>
        /// <param name="featureObj">The feature that the anno is being added to.</param>
        /// <param name="annoObj">The annotation object that is being added.</param>
        public void UpdateAnnoAlignment(IObject featureObj, IObject annoObj)
        {
            try
            {
                string featureFldNameLabelText = ModelNames.Manager.FieldNamesFromModelName(featureObj.Class, ModelNames.F_LABELTEXT).Next();

                string annoLabelTxtValue = FieldManager.GetValue(annoObj, FLD_ANNO_LABELTEXT).ToString().Trim();
                string featureLabelTextValue = FieldManager.GetValue(featureObj, featureFldNameLabelText).ToString().Trim();

                // Check to ensure that the annotation is using a labeltext value.
                if (annoLabelTxtValue == featureLabelTextValue)
                {
                    // Explicitly set the horiztonal alignment on the annotation.
                    annoObj.set_Value(FieldManager.GetIndex(annoObj.Class, FLD_ANNO_HORIZTONALALIGNMENT), (int)AnnoAlignment);
                    annoObj.Store();

                    _logger.Debug("A related annotation's horiztonal alignment was reset.");
                }
            }
            catch (Exception e)
            {
                _logger.Warn("(AU will continue) Exception in Execute() during anno alignment reset for " + Name + ":" + e.ToString());
            }
        }

        /// <summary>
        /// Due to an anno bug, find the anno for this feature and update the horizontal alignment.
        /// </summary>
        /// <param name="featureObj">The feature that the anno is being added to.</param>
        /// <param name="featureLabelTextValue">The original value of the feature's labeltext value before the current execution of any AUs.</param>
        public void UpdateAnnoAlignment(IObject featureObj, string featureLabelTextValue)
        {
            try
            {

                List<IObject> annos = RelationshipManager.GetRelated(featureObj, esriRelRole.esriRelRoleOrigin, ModelNames.ANNO, null);
                foreach (IObject annoObj in annos)
                {
                    string annoLabelTxtValue = FieldManager.GetValue(annoObj, FLD_ANNO_LABELTEXT).ToString().Trim();

                    // Check to ensure that the annotation is using a labeltext value.
                    if (annoLabelTxtValue == featureLabelTextValue.Trim())
                    {
                        // Explicitly set the horiztonal alignment on the annotation.
                        annoObj.set_Value(FieldManager.GetIndex(annoObj.Class, FLD_ANNO_HORIZTONALALIGNMENT), (int)AnnoAlignment);
                        annoObj.Store();

                        _logger.Debug("A related annotation's horiztonal alignment was reset.");
                    }
                }
            }
            catch (Exception e)
            {
                _logger.Warn("(AU will continue) Exception in Execute() during anno alignment reset for " + Name + ":" + e.ToString());
            }
        }
        #endregion

        internal string BuildPhasePrefix(IObject obj)
        {
            string phasePrefix = "";
            try
            {
                _logger.Debug(MethodBase.GetCurrentMethod().Name + " on object class [ " + obj.Class.AliasName + " ] for OID [ " + obj.OID.ToString() + " ]" );

                string numberOfUnits = FieldManager.GetValue(obj, SchemaInfo.General.Fields.NumberOfUnits).ToString().Trim();
                int phaseDesignation = FieldManager.GetValue(obj, SchemaInfo.General.Fields.PhaseDesignation).Convert<int>(0);

                if (!string.IsNullOrEmpty(numberOfUnits))
                {
                    phasePrefix += numberOfUnits + "-";
                    if (numberOfUnits == "1" & phaseDesignation == 7) // 7 == ABC phase in coded domain for Phase Designation
                    {
                        phasePrefix += "3P";
                    }
                    else
                    {
                        phasePrefix += "1P";
                    }
                }                
            }
            catch (Exception e)
            {
                _logger.Warn("(AU will continue) Exception in Execute() during labeltext set for " + Name + ":" + e.ToString());
            }

            return phasePrefix;
        }

        #region Parent Finder
        /// <summary>
        /// Given an IObject, converts the object pointer to its AU-eligible parent based on relationships and model names.
        /// </summary>
        /// <param name="searchOnObject">The object used to find the parent. This reference will be changed to point to the parent by this method.</param>
        /// <param name="fromParent">
        ///     Indicates whether or not the object was already the parent, in which case a Store() call is normally not required.
        /// </param>
        /// <returns><c>true</c> if a parent was found, <c>false</c> otherwise</returns>
        internal static bool FindSubAUParent(string parentModelName, ref IObject searchOnObject, out bool fromParent)
        {
            fromParent = false;
            if (ModelNames.Manager.ContainsClassModelName(searchOnObject.Class, parentModelName))
            {
                fromParent = true;
            }
            else if (ModelNames.Manager.ContainsClassModelName(searchOnObject.Class, ModelNames.UNIT)
                || ModelNames.Manager.ContainsClassModelName(searchOnObject.Class, ModelNames.SUBLOADTAPCHANGER))
            {
                searchOnObject = RelationshipManager.GetRelated(searchOnObject, esriRelRole.esriRelRoleDestination, parentModelName, null)[0];
            }
            else if (ModelNames.Manager.ContainsClassModelName(searchOnObject.Class, ModelNames.SUBRATING))
            {
                searchOnObject = RelationshipManager.GetRelated(searchOnObject, esriRelRole.esriRelRoleDestination, ModelNames.UNIT, null)[0];
                return FindSubAUParent(parentModelName, ref searchOnObject, out fromParent);
            }
            else
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Given an IObjectClass, converts the object class pointer to its AU-eligible parent based on relationships and model names.
        /// </summary>
        /// <param name="parentModelName">The model name used to identify the specific parent class used.</param>
        /// <param name="searchOnObjectClass">The object class used to find the parent class. This reference will be changed to point to the parent by this method.</param>
        /// <param name="fromParent">
        ///     Indicates whether or not the object class was already the parent, in which case a Store() call is normally not required.
        /// </param>
        /// <returns><c>true</c> if a parent class was found, <c>false</c> otherwise</returns>
        internal static bool FindSubAUParentClass(string parentModelName, ref IObjectClass searchOnObjectClass, out bool fromParent)
        {
            fromParent = false;
            if (ModelNames.Manager.ContainsClassModelName(searchOnObjectClass, parentModelName))
            {
                fromParent = true;
            }
            else if (ModelNames.Manager.ContainsClassModelName(searchOnObjectClass, ModelNames.UNIT)
                || ModelNames.Manager.ContainsClassModelName(searchOnObjectClass, ModelNames.SUBLOADTAPCHANGER))
            {
                List<IRelationshipClass> rels = RelationshipManager.GetRelCache(searchOnObjectClass, esriRelRole.esriRelRoleDestination, parentModelName);
                if (rels.Count > 0)
                    searchOnObjectClass = rels[0].OriginClass;
                else
                    return false;
            }
            else if (ModelNames.Manager.ContainsClassModelName(searchOnObjectClass, ModelNames.SUBRATING))
            {
                List<IRelationshipClass> rels = RelationshipManager.GetRelCache(searchOnObjectClass, esriRelRole.esriRelRoleDestination, ModelNames.UNIT);
                if (rels.Count > 0)
                    searchOnObjectClass = rels[0].OriginClass;
                else
                    return false;

                return FindSubAUParentClass(parentModelName, ref searchOnObjectClass, out fromParent);
            }
            else
            {
                return false;
            }

            return true;
        }
        #endregion
    }
}
